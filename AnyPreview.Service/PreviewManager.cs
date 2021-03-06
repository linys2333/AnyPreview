﻿using AnyPreview.Core.Common;
using AnyPreview.Core.Settings;
using AnyPreview.Service.Aliyun;
using AnyPreview.Service.Dtos;
using AnyPreview.Service.Redis;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using static AnyPreview.Core.Common.CommonConstants;
using Credentials = Aliyun.Acs.Core.Auth.Sts.AssumeRoleResponse.AssumeRole_Credentials;

namespace AnyPreview.Service
{
    public class PreviewManager
    {
        protected readonly IMMSetting m_IMMSetting;
        protected readonly IMMService m_IMMService;
        protected readonly OSSService m_OSSService;
        protected readonly STSService m_STSService;
        protected readonly PreviewSetting m_PreviewSetting;
        protected readonly PreviewRedisService m_PreviewRedisService;
        protected readonly STSTokenRedisService m_STSTokenRedisService;
        
        public PreviewManager
        (
            IMMSetting immSetting,
            PreviewSetting previewSetting,
            IMMService immService,
            OSSService ossService,
            STSService stsService,
            PreviewRedisService previewRedisService,
            STSTokenRedisService stsTokenRedisService
        )
        {
            m_IMMSetting = immSetting;
            m_IMMService = immService;
            m_OSSService = ossService;
            m_STSService = stsService;
            m_PreviewSetting = previewSetting;
            m_PreviewRedisService = previewRedisService;
            m_STSTokenRedisService = stsTokenRedisService;
        }

        public virtual async Task<SimplyResult<DocConvertResultDto>> GenerateAsync(OSSObjectDto ossObjectDto, bool isRegenerate)
        {
            Requires.NotNull(ossObjectDto, nameof(ossObjectDto));

            var generateResult = await m_PreviewRedisService.GetAsync(ossObjectDto.HashPath);
            if ((generateResult?.ETag ?? string.Empty) != ossObjectDto.ETag)
            {
                isRegenerate = true;
            }

            if (isRegenerate)
            {
                generateResult = m_PreviewSetting.IsSyncConvert
                    ? m_IMMService.Convert(ossObjectDto)
                    : m_IMMService.CreateConvertTask(ossObjectDto);
            }

            if (generateResult?.Status == DocConvertStatus.Running && m_PreviewSetting.TaskPollingSpend > 0)
            {
                generateResult = await QueryConvertTaskAsync(generateResult.TaskId);
            }

            switch (generateResult?.Status)
            {
                case DocConvertStatus.Running:
                case DocConvertStatus.Finished:
                    generateResult.PreviewUrl = m_IMMSetting.GetPreviewUrl(ossObjectDto.IMMKey);
                    generateResult.FileType = ossObjectDto.FileType;

                    await m_PreviewRedisService.SetAsync(ossObjectDto.HashPath, generateResult);

                    var token = await GetTokenAsync(m_IMMSetting.Bucket,
                        $"{m_IMMSetting.GetPrefix(ossObjectDto.IMMKey)}/*", ossObjectDto.HashPath, isRegenerate);

                    generateResult.PreviewUrl = GetFullPreviewUrl(generateResult.PreviewUrl, token);
                    return SimplyResult.Ok(generateResult);
                default:
                    await m_PreviewRedisService.DeleteAsync(ossObjectDto.HashPath);
                    return SimplyResult.Fail<DocConvertResultDto>("GenerateFail", "文档转换失败");
            }
        }
        
        public virtual SimplyResult<OSSObjectDto> GetOSSObject(string ossPath)
        {
            Requires.NotNullOrEmpty(ossPath, nameof(ossPath));

            var ossObject = new OSSObjectDto(ossPath);

            var ossObjectMetadata = m_OSSService.GetObjectMetadata(ossObject.Bucket, ossObject.FilePath);
            if (ossObjectMetadata == null)
            {
                return SimplyResult.Fail<OSSObjectDto>("FileNoExist", "文档不存在");
            }

            var ossFileType = string.Empty;
            if (!string.IsNullOrEmpty(ossObjectMetadata.ContentDisposition))
            {
                var fileName = Regexs.ContentDispositionFileNameRegex.Match(ossObjectMetadata.ContentDisposition).Value;
                ossFileType = Path.GetExtension(fileName).TrimStart('.');
            }

            if (string.IsNullOrEmpty(ossFileType))
            {
                Resources.ContentTypeDict.TryGetValue(ossObjectMetadata.ContentType.Split(';')[0].ToLower(), out ossFileType);
            }

            if (string.IsNullOrEmpty(ossFileType) || !Resources.IMMFileTypeDict.TryGetValue(ossFileType, out var immFileType))
            {
                return SimplyResult.Fail<OSSObjectDto>("UnsupportedFile", "不支持的文件类型");
            }

            ossObject.FileType = immFileType;
            ossObject.ETag = ossObjectMetadata.ETag;

            return SimplyResult.Ok(ossObject);
        }

        protected virtual async Task<Credentials> GetTokenAsync(string bucket, string filePath, string cacheKey, bool isRefresh)
        {
            Requires.NotNullOrEmpty(bucket, nameof(bucket));
            Requires.NotNullOrEmpty(filePath, nameof(filePath));
            Requires.NotNullOrEmpty(cacheKey, nameof(cacheKey));

            var token = await m_STSTokenRedisService.GetAsync(cacheKey);
            if (token == null || isRefresh)
            {
                token = m_STSService.GetToken(GetTokenPolicy(bucket, filePath), m_IMMSetting.TokenExpiry);
                if (token != null)
                {
                    await m_STSTokenRedisService.SetAsync(cacheKey, token);
                }
            }
            return token;
        }

        protected virtual string GetTokenPolicy(string bucket, string filePath)
        {
            return $@"
{{
    'Version': '1',
    'Statement': [
      {{
        'Effect': 'Allow',
        'Action': [
          'oss:*'
        ],
        'Resource': [
          'acs:oss:*:*:{bucket}/{filePath}'
        ]
      }},
      {{
        'Effect': 'Allow',
        'Action': [
          'oss:ListObjects'
        ],
        'Resource': [
          'acs:oss:*:*:{bucket}'
        ],
        'Condition': {{
          'StringLike': {{
            'oss:Prefix': '{filePath}'
          }}
        }}
      }}
    ]
}}".Replace("'", "\"");
        }

        protected virtual string GetFullPreviewUrl(string tgtLoc, Credentials token)
        {
            if (string.IsNullOrEmpty(tgtLoc) || token == null)
            {
                return string.Empty;
            }
            
            var paramters = new List<string>
            {
                $"url={tgtLoc}",
                $"accessKeyId={token.AccessKeyId}",
                $"accessKeySecret={token.AccessKeySecret}",
                $"stsToken={HttpUtility.UrlEncode(token.SecurityToken)}",
                $"region=oss-{m_IMMSetting.Region}",
                $"bucket={m_IMMSetting.Bucket}"
            };
            return $"{m_IMMSetting.PreviewIndexPath}?{string.Join("&", paramters)}";
        }

        protected virtual async Task<DocConvertResultDto> QueryConvertTaskAsync(string taskId)
        {
            using (var cancellTokenSource = new CancellationTokenSource(m_PreviewSetting.TaskPollingSpend))
            {
                DocConvertResultDto generateResult = null;

                while (!cancellTokenSource.IsCancellationRequested)
                {
                    // 官方建议
                    Thread.Sleep(m_PreviewSetting.IMMRequestInterval);

                    generateResult = m_IMMService.QueryConvertTask(taskId);
                    if (generateResult?.Status != DocConvertStatus.Running)
                    {
                        break;
                    }
                }

                return generateResult;
            }
        }
    }
}

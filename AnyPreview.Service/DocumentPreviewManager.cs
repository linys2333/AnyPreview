using AnyPreview.Core.Aliyun;
using AnyPreview.Service.Aliyun;
using AnyPreview.Service.Common;
using AnyPreview.Service.Dtos;
using AnyPreview.Service.Redis;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Credentials = Aliyun.Acs.Core.Auth.Sts.AssumeRoleResponse.AssumeRole_Credentials;

namespace AnyPreview.Service
{
    public class DocumentPreviewManager
    {
        protected readonly IMMSetting m_IMMSetting;
        protected readonly IMMService m_IMMService;
        protected readonly OSSService m_OSSService;
        protected readonly STSService m_STSService;
        protected readonly DocumentPreviewRedisService m_DocumentPreviewRedisService;
        protected readonly STSTokenRedisService m_STSTokenRedisService;
        
        public DocumentPreviewManager
        (
            IMMSetting immSetting,
            IMMService immService,
            OSSService ossService,
            STSService stsService,
            DocumentPreviewRedisService documentPreviewRedisService,
            STSTokenRedisService stsTokenRedisService
        )
        {
            m_IMMSetting = immSetting;
            m_IMMService = immService;
            m_OSSService = ossService;
            m_STSService = stsService;
            m_DocumentPreviewRedisService = documentPreviewRedisService;
            m_STSTokenRedisService = stsTokenRedisService;
        }

        public virtual async Task<SimplyResult<DocumentConvertResultDto>> GenerateAsync(OSSObjectDto ossObjectDto, bool isRegenerate)
        {
            Requires.NotNull(ossObjectDto, nameof(ossObjectDto));

            var generateResult = await m_DocumentPreviewRedisService.GetAsync(ossObjectDto.HashPath);

            if (generateResult == null)
            {
                if (m_OSSService.IsExist(m_IMMSetting.Bucket, m_IMMSetting.GetDocumentMetaPath(ossObjectDto.IMMKey)))
                {
                    generateResult = new DocumentConvertResultDto
                    {
                        PreviewUrl = m_IMMSetting.GetTgtUri(ossObjectDto.IMMKey),
                        Status = DocumentConvertStatus.Finished
                    };
                }
                else
                {
                    isRegenerate = true;
                }
            }

            if (isRegenerate)
            {
                generateResult = m_IMMService.Convert(ossObjectDto);
            }
            else if(generateResult.Status == DocumentConvertStatus.Running)
            {
                generateResult = m_IMMService.Query(generateResult.TaskId);
            }

            switch (generateResult?.Status)
            {
                case DocumentConvertStatus.Running:
                case DocumentConvertStatus.Finished:
                    generateResult.FileType = ossObjectDto.FileType;
                    await m_DocumentPreviewRedisService.SetAsync(ossObjectDto.HashPath, generateResult);

                    var token = await GetTokenAsync(m_IMMSetting.Bucket,
                        $"{m_IMMSetting.GetPrefix(ossObjectDto.IMMKey)}/*", ossObjectDto.HashPath, isRegenerate);

                    generateResult.PreviewUrl = GetPreviewUrl(generateResult.PreviewUrl, token);
                    return SimplyResult.Ok(generateResult);
                default:
                    await m_DocumentPreviewRedisService.DeleteAsync(ossObjectDto.HashPath);
                    return SimplyResult.Fail<DocumentConvertResultDto>("GenerateFail", "文档转换失败");
            }
        }

        public virtual SimplyResult<string> GetFileType(string bucket, string filePath)
        {
            Requires.NotNullOrEmpty(bucket, nameof(bucket));
            Requires.NotNullOrEmpty(filePath, nameof(filePath));

            var ossObjectMetadata = m_OSSService.GetObjectMetadata(bucket, filePath);
            if (ossObjectMetadata == null)
            {
                return SimplyResult.Fail<string>("FileNoExist", "文档不存在");
            }

            var fielType = DocumentPreviewConstants.ContentTypeDict
                .GetValueOrDefault(ossObjectMetadata.ContentType.Split(";")[0].ToLower());

            return string.IsNullOrEmpty(fielType)
                ? SimplyResult.Fail<string>("FileTypeError", "不支持的文档类型")
                : SimplyResult.Ok(fielType);
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

        protected virtual string GetPreviewUrl(string tgtLoc, Credentials token)
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
            return $"{m_IMMSetting.PreviewIndexPath}?{string.Join('&', paramters)}";
        }
    }
}

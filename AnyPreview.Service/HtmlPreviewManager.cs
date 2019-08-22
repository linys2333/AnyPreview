using Aliyun.OSS;
using AnyPreview.Core.Common;
using AnyPreview.Core.Settings;
using AnyPreview.Service.Aliyun;
using AnyPreview.Service.Dtos;
using AnyPreview.Service.Redis;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;
using System.Threading.Tasks;
using Credentials = Aliyun.Acs.Core.Auth.Sts.AssumeRoleResponse.AssumeRole_Credentials;

namespace AnyPreview.Service
{
    public class HtmlPreviewManager : PreviewManager
    {
        public HtmlPreviewManager
        (
            IMMSetting immSetting,
            PreviewSetting previewSetting,
            IMMService immService,
            OSSService ossService,
            STSService stsService,
            PreviewRedisService previewRedisService,
            STSTokenRedisService stsTokenRedisService
        ) : base(immSetting, previewSetting, immService, ossService, stsService, previewRedisService, stsTokenRedisService)
        {
        }

        public override async Task<SimplyResult<DocConvertResultDto>> GenerateAsync(OSSObjectDto ossObjectDto, bool isRegenerate)
        {
            Requires.NotNull(ossObjectDto, nameof(ossObjectDto));

            var token = await GetTokenAsync(ossObjectDto.Bucket, ossObjectDto.FilePath, ossObjectDto.HashPath, isRegenerate);
            var generateResult = new DocConvertResultDto
            {
                PreviewUrl = GetPreviewUrl(ossObjectDto.Bucket, ossObjectDto.FilePath, token),
                Status = DocConvertStatus.Finished,
                FileType = ossObjectDto.FileType
            };

            return SimplyResult.Ok(generateResult);
        }

        protected override string GetTokenPolicy(string bucket, string filePath)
        {
            return $@"
{{
    'Version': '1',
    'Statement': [
      {{
        'Effect': 'Allow',
        'Action': [
          'oss:GetObject'
        ],
        'Resource': [
          'acs:oss:*:*:{bucket}/{filePath}'
        ]
      }}
    ]
}}".Replace("'", "\"");
        }

        protected virtual string GetPreviewUrl(string bucket, string filePath, Credentials token)
        {
            if (token == null)
            {
                return string.Empty;
            }

            m_OSSService.OSSClient = new OssClient(m_IMMSetting.OSSEndpoint, token.AccessKeyId, token.AccessKeySecret, token.SecurityToken);
            return m_OSSService.GetPreviewUrl(bucket, filePath, m_IMMSetting.TokenExpiry, "text/html");
        }
    }
}

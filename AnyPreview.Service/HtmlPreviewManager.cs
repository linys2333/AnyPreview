using Aliyun.OSS;
using AnyPreview.Core.Aliyun;
using AnyPreview.Service.Aliyun;
using AnyPreview.Service.Dtos;
using AnyPreview.Service.Redis;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;
using System.Threading.Tasks;
using Credentials = Aliyun.Acs.Core.Auth.Sts.AssumeRoleResponse.AssumeRole_Credentials;

namespace AnyPreview.Service
{
    public class HtmlPreviewManager : DocumentPreviewManager
    {
        public HtmlPreviewManager
        (
            IMMSetting immSetting,
            IMMService immService,
            OSSService ossService,
            STSService stsService,
            DocumentPreviewRedisService documentPreviewRedisService,
            STSTokenRedisService stsTokenRedisService
        ) : base(immSetting, immService, ossService, stsService, documentPreviewRedisService, stsTokenRedisService)
        {
        }

        public override async Task<SimplyResult<DocumentConvertResultDto>> GenerateAsync(OSSObjectDto ossObjectDto, bool isRegenerate)
        {
            Requires.NotNull(ossObjectDto, nameof(ossObjectDto));

            var token = await GetTokenAsync(ossObjectDto.Bucket, ossObjectDto.FilePath, ossObjectDto.HashPath, isRegenerate);
            var generateResult = new DocumentConvertResultDto
            {
                PreviewUrl = GetPreviewUrl(ossObjectDto.Bucket, ossObjectDto.FilePath, token),
                Status = DocumentConvertStatus.Finished,
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
            return m_OSSService.GetPreviewUrl(bucket, filePath, m_IMMSetting.TokenExpiry);
        }
    }
}

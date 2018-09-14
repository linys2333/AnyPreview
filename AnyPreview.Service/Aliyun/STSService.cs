using Aliyun.Acs.Core.Auth.Sts;
using Aliyun.Acs.Core.Http;
using AnyPreview.Core.Aliyun;
using AnyPreview.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;
using System;
using Credentials = Aliyun.Acs.Core.Auth.Sts.AssumeRoleResponse.AssumeRole_Credentials;

namespace AnyPreview.Service.Aliyun
{
    public class STSService
    {
        private readonly STSClient m_STSClient;
        private readonly ILogger m_Logger;
        private readonly IMMSetting m_IMMSetting;

        public STSService(STSClient stsClient, ILogger logger, IMMSetting immSetting)
        {
            m_STSClient = stsClient;
            m_Logger = logger;
            m_IMMSetting = immSetting;
        }

        public Credentials GetToken(string policy, int expiry)
        {
            Requires.NotNullOrEmpty(policy, nameof(policy));

            var request = new AssumeRoleRequest
            {
                AcceptFormat = FormatType.JSON,
                RoleArn = m_IMMSetting.RoleArn,
                RoleSessionName = m_IMMSetting.Project,
                DurationSeconds = expiry,
                Policy = policy
            };

            try
            {
                return m_STSClient.GetAcsResponse(request)?.Credentials;
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, "获取STSToken错误");
                return null;
            }
        }
    }
}

using Aliyun.OSS;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;
using System;

namespace AnyPreview.Service.Aliyun
{
    public class OSSService
    {
        private OssClient m_OSSClient;
        private readonly ILogger m_Logger;

        public OSSService(OssClient ossClient, ILogger logger)
        {
            m_OSSClient = ossClient;
            m_Logger = logger;
        }

        public OssClient OSSClient
        {
            set => m_OSSClient = value;
        }

        public ObjectMetadata GetObjectMetadata(string bucket, string filePath)
        {
            Requires.NotNullOrEmpty(bucket, nameof(bucket));
            Requires.NotNullOrEmpty(filePath, nameof(filePath));

            try
            {
                return m_OSSClient.GetObjectMetadata(bucket, filePath);
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, "GetObjectMetadata调用错误");
                return null;
            }
        }

        public string GetPreviewUrl(string bucket, string filePath, int expiry)
        {
            Requires.NotNullOrEmpty(bucket, nameof(bucket));
            Requires.NotNullOrEmpty(filePath, nameof(filePath));

            try
            {
                var req = new GeneratePresignedUriRequest(bucket, filePath, SignHttpMethod.Get)
                {
                    Expiration = DateTime.Now.AddSeconds(expiry),
                    ResponseHeaders = new ResponseHeaderOverrides
                    {
                        ContentDisposition = "inline"
                    }
                };
                return m_OSSClient.GeneratePresignedUri(req).AbsoluteUri;
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, "GeneratePresignedUri调用错误");
                return string.Empty;
            }
        }

        public bool IsExist(string bucket, string filePath)
        {
            Requires.NotNullOrEmpty(bucket, nameof(bucket));
            Requires.NotNullOrEmpty(filePath, nameof(filePath));

            try
            {
                return m_OSSClient.DoesObjectExist(bucket, filePath);
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, "DoesObjectExist调用错误");
                return false;
            }
        }
    }
}

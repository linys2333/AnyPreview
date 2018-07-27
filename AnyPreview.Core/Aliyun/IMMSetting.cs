namespace AnyPreview.Core.Aliyun
{
    public class IMMSetting
    {
        public string AccessKeyId { get; set; }

        public string AccessKeySecret { get; set; }
        
        public string Region { get; set; }

        public string Project { get; set; }

        public string Bucket { get; set; }

        public string PreviewPath { get; set; }

        public string KeyPrefix { get; set; }

        public string RoleArn { get; set; }

        public string ConvertType { get; set; }
        
        public int TokenExpiry { get; set; }

        public string IMMEndpoint => $"imm.{Region}.aliyuncs.com";

        public string STSEndpoint => $"sts.{Region}.aliyuncs.com";

        public string OSSEndpoint => $"oss-{Region}.aliyuncs.com";

        public string PreviewIndexPath => $"http://{Bucket}.{OSSEndpoint}/{PreviewPath}/index.html";

        public string GetPrefix(string key) => $"{KeyPrefix}/{key}/{ConvertType}";

        public string GetTgtUri(string key) => $"oss://{Bucket}/{GetPrefix(key)}";

        public string GetDocumentMetaPath(string key) => $"{GetPrefix(key)}/doc/meta.json";
    }
}

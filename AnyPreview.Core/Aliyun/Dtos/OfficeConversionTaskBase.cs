using Aliyun.Acs.Core;

namespace AnyPreview.Core.Aliyun
{
    public class OfficeConversionTaskBase : AcsResponse
    {
        public class OfficeFailDetail
        {
            public string Code { get; set; }
        }

        public string TaskId { get; set; }
        
        public string Status { get; set; }

        public OfficeFailDetail FailDetail { get; set; }

        public bool IsFail => Status == "Failed" && !string.IsNullOrEmpty(FailDetail.Code);
    }
}

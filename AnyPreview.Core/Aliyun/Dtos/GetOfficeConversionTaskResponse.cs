namespace AnyPreview.Core.Aliyun
{
    public class GetOfficeConversionTaskResponse : OfficeConversionTaskBase
    {
        public string TgtUri { get; set; }
        
        public string CreateTime { get; set; }

        public string FinishTime { get; set; }
    }
}

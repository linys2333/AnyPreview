using Aliyun.Acs.Core.Transform;

namespace AnyPreview.Core.Aliyun
{
    public class GetOfficeConversionTaskResponseUnmarshaller
    {
        public static GetOfficeConversionTaskResponse Unmarshall(UnmarshallerContext context)
        {
            var assumeRoleResponse = new GetOfficeConversionTaskResponse
            {
                RequestId = context.StringValue("GetOfficeConversionTask.RequestId"),
                TaskId = context.StringValue("GetOfficeConversionTask.TaskId"),
                TgtUri = context.StringValue("GetOfficeConversionTask.TgtUri"),
                Status = context.StringValue("GetOfficeConversionTask.Status"),
                CreateTime = context.StringValue("GetOfficeConversionTask.CreateTime"),
                FinishTime = context.StringValue("GetOfficeConversionTask.FinishTime"),
                FailDetail = new OfficeConversionTaskBase.OfficeFailDetail
                {
                    Code = context.StringValue("GetOfficeConversionTask.FailDetail.Code"),
                }
            };

            return assumeRoleResponse;
        }
    }
}

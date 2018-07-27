using Aliyun.Acs.Core.Transform;

namespace AnyPreview.Core.Aliyun
{
    public class CreateOfficeConversionTaskResponseUnmarshaller
    {
        public static CreateOfficeConversionTaskResponse Unmarshall(UnmarshallerContext context)
        {
            var assumeRoleResponse = new CreateOfficeConversionTaskResponse
            {
                RequestId = context.StringValue("CreateOfficeConversionTask.RequestId"),
                TaskId = context.StringValue("CreateOfficeConversionTask.TaskId"),
                TgtLoc = context.StringValue("CreateOfficeConversionTask.TgtLoc"),
                Status = context.StringValue("CreateOfficeConversionTask.Status"),
                CreateTime = context.StringValue("CreateOfficeConversionTask.CreateTime"),
                FailDetail = new OfficeConversionTaskBase.OfficeFailDetail
                {
                    Code = context.StringValue("CreateOfficeConversionTask.FailDetail.Code"),
                }
            };

            return assumeRoleResponse;
        }
    }
}

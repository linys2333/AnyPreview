using Aliyun.Acs.imm.Model.V20170906;
using System;

namespace AnyPreview.Service.Dtos
{
    public class DocConvertResultDto
    {
        public DocConvertResultDto()
        {
        }

        public DocConvertResultDto(ConvertOfficeFormatResponse response)
        {
            RequestId = response.RequestId;
            Status = DocConvertStatus.Finished;
        }
        
        public DocConvertResultDto(CreateOfficeConversionTaskResponse response)
        {
            TaskId = response.TaskId;
            RequestId = response.RequestId;
            Status = (DocConvertStatus)Enum.Parse(typeof(DocConvertStatus), response.Status);
        }

        public DocConvertResultDto(GetOfficeConversionTaskResponse response)
        {
            TaskId = response.TaskId;
            RequestId = response.RequestId;
            Status = (DocConvertStatus)Enum.Parse(typeof(DocConvertStatus), response.Status);
        }

        public string PreviewUrl { get; set; }

        public string FileType { get; set; }

        public string TaskId { get; set; }

        public string RequestId { get; set; }

        public DocConvertStatus Status { get; set; }

        public string ETag { get; set; }
    }
}

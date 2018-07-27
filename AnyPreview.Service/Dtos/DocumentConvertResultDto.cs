using AnyPreview.Core.Aliyun;
using System;

namespace AnyPreview.Service.Dtos
{
    public class DocumentConvertResultDto
    {
        public DocumentConvertResultDto()
        {
        }

        public DocumentConvertResultDto(CreateOfficeConversionTaskResponse response)
        {
            TaskId = response.TaskId;
            PreviewUrl = response.TgtLoc;

            if (Enum.TryParse<DocumentConvertStatus>(response.Status, out var status))
            {
                Status = status;
            }
        }

        public DocumentConvertResultDto(GetOfficeConversionTaskResponse response)
        {
            TaskId = response.TaskId;
            PreviewUrl = response.TgtUri;

            if (Enum.TryParse<DocumentConvertStatus>(response.Status, out var status))
            {
                Status = status;
            }
        }

        public string TaskId { get; set; } 
        
        public string PreviewUrl { get; set; }

        public string FileType { get; set; }

        public DocumentConvertStatus Status { get; set; }
    }
}

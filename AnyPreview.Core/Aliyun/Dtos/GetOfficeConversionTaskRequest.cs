using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Utils;

namespace AnyPreview.Core.Aliyun
{
    public class GetOfficeConversionTaskRequest : RpcAcsRequest<GetOfficeConversionTaskResponse>
    {
        public GetOfficeConversionTaskRequest()
            : base("Imm", "2017-09-06", "GetOfficeConversionTask")
        {
            Protocol = global::Aliyun.Acs.Core.Http.ProtocolType.HTTPS;
        }
        
        private string m_Project;
        private string m_TaskId;

        public string Project
        {
            get => m_Project;
            set
            {
                m_Project = value;
                DictionaryUtil.Add(QueryParameters, "Project", value);
            }
        }

        public string TaskId
        {
            get => m_TaskId;
            set
            {
                m_TaskId = value;
                DictionaryUtil.Add(QueryParameters, "TaskId", value);
            }
        }
        
        public override GetOfficeConversionTaskResponse GetResponse(global::Aliyun.Acs.Core.Transform.UnmarshallerContext unmarshallerContext)
        {
            return GetOfficeConversionTaskResponseUnmarshaller.Unmarshall(unmarshallerContext);
        }
    }
}

using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Utils;

namespace AnyPreview.Core.Aliyun
{
    public class CreateOfficeConversionTaskRequest : RpcAcsRequest<CreateOfficeConversionTaskResponse>
    {
        public CreateOfficeConversionTaskRequest()
            : base("Imm", "2017-09-06", "CreateOfficeConversionTask")
        {
            Protocol = global::Aliyun.Acs.Core.Http.ProtocolType.HTTPS;
        }

        private string m_Project;
        private string m_SrcUri;
        private string m_SrcType;
        private string m_TgtType;
        private string m_TgtUri;

        public string Project
        {
            get => m_Project;
            set
            {
                m_Project = value;
                DictionaryUtil.Add(QueryParameters, "Project", value);
            }
        }

        public string SrcUri
        {
            get => m_SrcUri;
            set
            {
                m_SrcUri = value;
                DictionaryUtil.Add(QueryParameters, "SrcUri", value);
            }
        }

        public string SrcType
        {
            get => m_SrcType;
            set
            {
                m_SrcType = value;
                DictionaryUtil.Add(QueryParameters, "SrcType", value);
            }
        }

        public string TgtType
        {
            get => m_TgtType;
            set
            {
                m_TgtType = value;
                DictionaryUtil.Add(QueryParameters, "TgtType", value);
            }
        }

        public string TgtUri
        {
            get => m_TgtUri;
            set
            {
                m_TgtUri = value;
                DictionaryUtil.Add(QueryParameters, "TgtUri", value);
            }
        }

        public override CreateOfficeConversionTaskResponse GetResponse(global::Aliyun.Acs.Core.Transform.UnmarshallerContext unmarshallerContext)
        {
            return CreateOfficeConversionTaskResponseUnmarshaller.Unmarshall(unmarshallerContext);
        }
    }
}
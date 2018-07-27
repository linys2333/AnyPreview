using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;

namespace AnyPreview.Core.Aliyun
{
    public class STSClient : DefaultAcsClient
    {
        public STSClient(IClientProfile profile) : base(profile)
        {
        }
    }
}

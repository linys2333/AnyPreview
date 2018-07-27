using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;

namespace AnyPreview.Core.Aliyun
{
    public class IMMClient : DefaultAcsClient
    {
        public IMMClient(IClientProfile profile) : base(profile)
        {
        }
    }
}

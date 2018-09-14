using Aliyun.Acs.Core.Profile;
using Aliyun.OSS;
using AnyPreview.Core.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;

namespace AnyPreview.Core.Aliyun
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection UseAliyunService(this IServiceCollection services, IMMSetting setting)
        {
            Requires.NotNull(setting, nameof(setting));
            
            DefaultProfile.AddEndpoint(setting.Region, setting.Region, "Imm", setting.IMMEndpoint);
            var immProfile = DefaultProfile.GetProfile(setting.Region, setting.AccessKeyId, setting.AccessKeySecret);
            var immClient = new IMMClient(immProfile);

            DefaultProfile.AddEndpoint(setting.Region, setting.Region, "Sts", setting.STSEndpoint);
            var stsProfile = DefaultProfile.GetProfile(setting.Region, setting.AccessKeyId, setting.AccessKeySecret);
            var stsClient = new STSClient(stsProfile);

            var ossClient = new OssClient(setting.OSSEndpoint, setting.AccessKeyId, setting.AccessKeySecret);

            services.AddSingleton(setting);
            services.AddSingleton(immClient);
            services.AddSingleton(stsClient);
            services.AddSingleton(ossClient);

            return services;
        }
    }
}

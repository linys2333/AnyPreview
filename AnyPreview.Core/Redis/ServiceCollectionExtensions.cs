using AnyPreview.Core.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;
using Newtonsoft.Json;

namespace AnyPreview.Core.Redis
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services, RedisSetting setting)
        {
            Requires.NotNull(setting, nameof(setting));

            var cSRedisClient = new CSRedis.CSRedisClient($"{setting.Configuration},prefix={setting.InstanceName}");
            RedisHelper.Initialization(cSRedisClient,
                value => JsonConvert.SerializeObject(value),
                deserialize: (data, type) => JsonConvert.DeserializeObject(data, type));

            services.AddSingleton<IDistributedCache>(new CSRedisCache(RedisHelper.Instance));
            services.AddSingleton(setting);

            return services;
        }
    }
}

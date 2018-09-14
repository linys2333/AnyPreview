using AnyPreview.Core.Settings;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Credentials = Aliyun.Acs.Core.Auth.Sts.AssumeRoleResponse.AssumeRole_Credentials;

namespace AnyPreview.Service.Redis
{
    public class STSTokenRedisService
    {
        private readonly IMMSetting m_IMMSetting;
        
        public STSTokenRedisService(IMMSetting immSetting)
        {
            m_IMMSetting = immSetting;
        }

        public Task<bool> SetAsync(string cacheKey, Credentials generateResult)
        {
            var key = GetKey(cacheKey);
            var value = JsonConvert.SerializeObject(generateResult);
            return RedisHelper.SetAsync(key, value, TimeSpan.FromSeconds(m_IMMSetting.TokenExpiry - 10).Seconds);
        }
        
        public async Task<Credentials> GetAsync(string cacheKey)
        {
            var key = GetKey(cacheKey);
            var value = await RedisHelper.GetAsync(key);
            return string.IsNullOrEmpty(value) ? null : JsonConvert.DeserializeObject<Credentials>(value);
        }
        
        private string GetKey(string cacheKey) => $"STSToken:{cacheKey}";
    }
}

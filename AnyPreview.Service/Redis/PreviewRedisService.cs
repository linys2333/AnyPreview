using AnyPreview.Service.Dtos;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace AnyPreview.Service.Redis
{
    public class PreviewRedisService
    {
        public Task<bool> SetAsync(string cacheKey, DocConvertResultDto generateResult)
        {
            var key = GetKey(cacheKey);
            var value = JsonConvert.SerializeObject(generateResult);
            return RedisHelper.SetAsync(key, value, TimeSpan.FromDays(7).Seconds);
        }
        
        public async Task<DocConvertResultDto> GetAsync(string cacheKey)
        {
            var key = GetKey(cacheKey);
            var value = await RedisHelper.GetAsync(key);
            return string.IsNullOrEmpty(value) ? null : JsonConvert.DeserializeObject<DocConvertResultDto>(value);
        }

        public async Task DeleteAsync(string cacheKey)
        {
            var key = GetKey(cacheKey);
            await RedisHelper.RemoveAsync(key);
        }

        private string GetKey(string cacheKey) => $"ConvertResult:{cacheKey}";
    }
}

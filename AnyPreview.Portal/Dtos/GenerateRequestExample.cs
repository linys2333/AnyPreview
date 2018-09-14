using Swashbuckle.AspNetCore.Examples;

namespace AnyPreview.Portal.Dtos
{
    public class GenerateRequestExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new GenerateRequest
            {
                OSSPath = "oss://{bucket}/{prefix}/{key}",
                IsRegenerate = false
            };
        }
    }
}
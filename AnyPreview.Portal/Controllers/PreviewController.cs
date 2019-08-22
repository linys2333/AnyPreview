using AnyPreview.Core.Web;
using AnyPreview.Portal.Dtos;
using AnyPreview.Portal.Web;
using AnyPreview.Service;
using AnyPreview.Service.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Examples;
using System.Threading.Tasks;

namespace AnyPreview.Portal.Controllers
{
    public class PreviewController : WebApiController
    {
        private readonly PreviewManager m_PreviewManager;
        private readonly HtmlPreviewManager m_HtmlPreviewManager;

        public PreviewController
        (
            PreviewManager previewManager,
            HtmlPreviewManager htmlPreviewManager
        )
        {
            m_PreviewManager = previewManager;
            m_HtmlPreviewManager = htmlPreviewManager;
        }

        /// <summary>
        /// 生成文档预览路径
        /// </summary>
        [SwaggerRequestExample(typeof(GenerateRequest), typeof(GenerateRequestExample))]
        [ProducesResponseType(typeof(WebApiResponse<DocConvertResultDto>), 200)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Generate([FromBody]GenerateRequest generateRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var getOSSObjectResult = m_PreviewManager.GetOSSObject(generateRequest.OSSPath);
            if (!getOSSObjectResult.IsSuccess)
            {
                return Json(getOSSObjectResult);
            }

            var ossObjectDto = getOSSObjectResult.Data;
            var result = ossObjectDto.IsHtml
                ? await m_HtmlPreviewManager.GenerateAsync(ossObjectDto, generateRequest.IsRegenerate)
                : await m_PreviewManager.GenerateAsync(ossObjectDto, generateRequest.IsRegenerate);
            
            return Json(result);
        }
    }
}
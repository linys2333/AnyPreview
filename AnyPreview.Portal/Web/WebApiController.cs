using AnyPreview.Core.Common;
using AnyPreview.Core.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace AnyPreview.Portal.Web
{
    [Produces("application/json")]
    [ProducesResponseType(typeof(WebApiResponse<object>), 200)]
    [Route("[controller]")]
    public abstract class WebApiController : Controller
    {
        private static readonly JsonSerializerSettings _JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatString = CommomConstants.DateTimeFormatter.HyphenLongDateTime
        };

        protected new JsonResult BadRequest(ModelStateDictionary modelState)
        {
            var errorMessage = string.Join(",", modelState.Values.SelectMany(ms =>
                ms.Errors.Where(e => !string.IsNullOrEmpty(e.ErrorMessage) || e.Exception != null).Select(e =>
                    string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage)));
            return Json("BadRequest", errorMessage);
        }

        protected JsonResult Ok<T>(T data)
        {
            return Json(WebApiResponse.Ok(data));
        }

        protected JsonResult Json<T>(IEnumerable<SimplyResult<T>> results)
        {
            return Json(results.Select(r => r.IsSuccess
                ? WebApiResponse.Ok(r.Data)
                : WebApiResponse.Fail(new ErrorDescriber {Code = r.ErrorCode, Description = r.ErrorMessage}, r.Data)));
        }

        protected JsonResult Json(IEnumerable<SimplyResult> results)
        {
            return Json(results.Select(r => r.IsSuccess
                ? WebApiResponse.Ok()
                : WebApiResponse.Fail(new ErrorDescriber {Code = r.ErrorCode, Description = r.ErrorMessage})));
        }

        protected JsonResult Json<T>(SimplyResult<T> result)
        {
            return result.IsSuccess
                ? Json(WebApiResponse.Ok(result.Data))
                : Json(result.ErrorCode, result.ErrorMessage, result.Data);
        }

        protected JsonResult Json(SimplyResult result)
        {
            return result.IsSuccess
                ? Json(WebApiResponse.Ok())
                : Json(result.ErrorCode, result.ErrorMessage);
        }

        private JsonResult Json(string errorCode, string errorMessage)
        {
            var error = new ErrorDescriber { Code = errorCode, Description = errorMessage };
            return Json(WebApiResponse.Fail(error));
        }

        private JsonResult Json<T>(string errorCode, string errorMessage, T result)
        {
            var error = new ErrorDescriber { Code = errorCode, Description = errorMessage };
            return Json(WebApiResponse.Fail(error, result));
        }

        private JsonResult Json(WebApiResponse data)
        {
            return Json(data, _JsonSerializerSettings);
        }
    }
}
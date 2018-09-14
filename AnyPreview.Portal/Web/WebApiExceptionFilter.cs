using AnyPreview.Core.Common;
using AnyPreview.Core.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace AnyPreview.Portal.Web
{
    public class WebApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger m_Logger;

        public WebApiExceptionFilter(ILoggerFactory loggerFactory)
        {
            m_Logger = loggerFactory.CreateLogger("WebApiException");
        }

        public void OnException(ExceptionContext context)
        {
            var httpStatusCode = HttpStatusCode.InternalServerError;
            if (context.Exception is ArgumentNullException)
            {
                httpStatusCode = HttpStatusCode.BadRequest;
            }

            var response = new WebApiResponse
            {
                Success = false,
                Error = new ErrorDescriber
                {
                    Code = httpStatusCode.ToString(),
                    Description = context.Exception.GetErrorMessage()
                }
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = (int)HttpStatusCode.OK,
                DeclaredType = typeof(WebApiResponse)
            };

            if (httpStatusCode != HttpStatusCode.BadRequest)
            {
                m_Logger.LogError(context.Exception, response.Error.Description);
            }
        }
    }
}

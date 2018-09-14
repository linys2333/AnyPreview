using System;

namespace AnyPreview.Core.Common
{
    public static class ExceptionExtensions
    {
        public static string GetErrorMessage(this Exception exception)
        {
            return exception.InnerException != null ? GetErrorMessage(exception.InnerException) : exception.Message;
        }
    }
}
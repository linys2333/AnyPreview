namespace AnyPreview.Core.Web
{
    public class WebApiResponse
    {
        public bool Success { get; set; }
        
        public ErrorDescriber Error { get; set; }

        public static WebApiResponse Fail(ErrorDescriber error)
        {
            return new WebApiResponse { Success = false, Error = error };
        }

        public static WebApiResponse Fail<T>(ErrorDescriber error, T data)
        {
            return new WebApiResponse<T> { Success = false, Error = error, Data = data };
        }

        public static WebApiResponse Ok()
        {
            return new WebApiResponse { Success = true };
        }

        public static WebApiResponse Ok<T>(T data)
        {
            return new WebApiResponse<T> { Success = true, Data = data };
        }
    }

    public class WebApiResponse<T> : WebApiResponse
    {
        public T Data { get; set; }
    }
}
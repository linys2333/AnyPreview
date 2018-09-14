namespace AnyPreview.Core.Common
{
    public class SimplyResult
    {
        public bool IsSuccess { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
        
        public static SimplyResult Ok() => new SimplyResult { IsSuccess = true };

        public static SimplyResult Fail(string code, string msg) =>
            new SimplyResult {IsSuccess = false, ErrorCode = code, ErrorMessage = msg};

        public static SimplyResult<T> Ok<T>(T data) => new SimplyResult<T> { IsSuccess = true, Data = data };

        public static SimplyResult<T> Fail<T>(string code, string msg, T data = default(T)) =>
            new SimplyResult<T> {IsSuccess = false, ErrorCode = code, ErrorMessage = msg, Data = data};
    }

    public class SimplyResult<T> : SimplyResult
    {
        public T Data { get; set; }
    }
}
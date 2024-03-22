namespace PushNotifications.Api
{
    /// <summary>
    /// Wrapper for returning the generic ApiResult object. It is used for better understandability of the code by having concrete type
    /// </summary>
    public class ApiSuccessResult<T> : ApiResult<T> where T : class
    {
        public ApiSuccessResult() : base() { }

        public ApiSuccessResult(T result) : base(result) { }
    }
}

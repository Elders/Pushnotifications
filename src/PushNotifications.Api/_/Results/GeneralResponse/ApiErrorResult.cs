namespace PushNotifications.Api
{
    /// <summary>
    /// Wrapper for returning the generic ApiResult object. It is used for better understandability of the code by having concrete type
    /// </summary>
    public class ApiErrorResult : ApiResult<object>
    {
        public ApiErrorResult(ExtendedProblemDetails error) : base(error)
        {
        }
    }
}

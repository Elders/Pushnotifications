namespace PushNotifications.Api
{
    /// <summary>
    /// General response object. Unifies the response under common format
    /// </summary>
    /// <typeparam name="T">The type of the object which is added to the 'result' property in case of successful result</typeparam>
    public abstract class ApiResult<T> where T : class
    {
        public ApiResult() { }

        public ApiResult(ExtendedProblemDetails error)
        {
            Error = error ?? throw new System.ArgumentNullException(nameof(error));
        }

        public ApiResult(T result)
        {
            Result = result ?? throw new System.ArgumentNullException(nameof(result));
        }

        public ExtendedProblemDetails Error { get; set; }

        public T Result { get; set; }

        public bool IsSuccess => Error is null;
    }
}

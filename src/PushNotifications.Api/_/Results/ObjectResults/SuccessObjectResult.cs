namespace PushNotifications.Api
{
    public class SuccessObjectResult<T> : Microsoft.AspNetCore.Mvc.ObjectResult where T : class
    {
        public SuccessObjectResult(T value) : base(new ApiSuccessResult<T>(value))
        {
            StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status200OK;
        }

        public SuccessObjectResult() : base(new ApiSuccessResult<T>())
        {
            StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status200OK;
        }
    }
}

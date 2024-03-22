namespace PushNotifications.Api
{
    public class ProblemObjectResult : Microsoft.AspNetCore.Mvc.ObjectResult
    {
        public ProblemObjectResult(ExtendedProblemDetails problemDetails)
            : base(new ApiErrorResult(problemDetails))
        {
            StatusCode = problemDetails.Status;
        }
    }
}

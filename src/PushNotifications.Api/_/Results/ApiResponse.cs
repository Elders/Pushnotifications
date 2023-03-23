using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PushNotifications.Api
{
    public class ApiResponse
    {
        protected readonly IHttpContextAccessor httpContextAccessor;

        public ApiResponse(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Success(object data)
        {
            return new SuccessObjectResult<object>(data);
        }

        public IActionResult NotFoundProblem(string details)
        {
            return new ProblemObjectResult(new ResourceNotFoundProblemDetails(httpContextAccessor.HttpContext, details));
        }

        public IActionResult AccessViolationProblem(string details)
        {
            return new ProblemObjectResult(new AccessViolationProblemDetails(httpContextAccessor.HttpContext, details));
        }

        public IActionResult ValidationProblem(string details)
        {
            return new ProblemObjectResult(new ValidationProblemDetails(httpContextAccessor.HttpContext, details));
        }

        public IActionResult ConflictProblem(string details, string existingResourceId)
        {
            return new ProblemObjectResult(new ResourceAlreadyExistsProblemDetails(httpContextAccessor.HttpContext, details, existingResourceId));
        }

        public IActionResult UnknownProblem(string details)
        {
            return new ProblemObjectResult(new UnknownProblemDetails(httpContextAccessor.HttpContext, details));
        }
    }
}

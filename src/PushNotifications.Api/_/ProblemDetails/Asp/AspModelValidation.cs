using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PushNotifications.Api
{
    /// <summary>
    /// Registered as 'InvalidModelStateResponseFactory' in 'ApiBehaviorOptions' so that it can intercept
    /// all validation made on request models and in case of errors return correct ProblemDetail response.
    /// Used whenever asp.net returns invalid model while still trying to bind the model to the action
    /// </summary>
    /// <seealso cref="IActionResult" />
    public class AspModelValidation : IActionResult
    {
        string message = "Check the validationErrors[] for more details.";

        public Task ExecuteResultAsync(ActionContext context)
        {
            var modelStateEntries = context.ModelState.Where(e => e.Value.Errors.Count > 0).ToArray();
            var errors = new List<ValidationError>();

            if (modelStateEntries.Any())
            {
                if (RequestHasFundamentalProblem(modelStateEntries))
                {
                    message = modelStateEntries[0].Value.Errors[0].ErrorMessage;
                }
                else
                {
                    foreach (var modelStateEntry in modelStateEntries)
                    {
                        foreach (var modelStateError in modelStateEntry.Value.Errors)
                        {
                            var error = new ValidationError
                            {
                                Name = modelStateEntry.Key,
                                Description = modelStateError.ErrorMessage
                            };

                            errors.Add(error);
                        }
                    }
                }
            }

            var problemDetails = new ValidationProblemDetails(context.HttpContext, message, errors);

            var response = new ApiErrorResult(problemDetails);
            var responseAsString = JsonSerializer.Serialize(response);

            if (response.Error.Status.HasValue)
                context.HttpContext.Response.StatusCode = response.Error.Status.Value;

            context.HttpContext.Response.ContentType = "application/problem+json";
            context.HttpContext.Response.WriteAsync(responseAsString).Wait();

            return Task.CompletedTask;
        }

        private static bool RequestHasFundamentalProblem(KeyValuePair<string, Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateEntry>[] modelStateEntries)
        {
            return modelStateEntries.Length == 1 && modelStateEntries[0].Value.Errors.Count == 1 && modelStateEntries[0].Key == string.Empty;
        }
    }
}

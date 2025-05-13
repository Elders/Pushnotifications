using Elders.Cronus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace PushNotifications.Api
{
    public class ApiCqrsResponse : ApiResponse
    {
        private readonly IPublisher<ICommand> publisher;
        private readonly IPublisher<ISignal> signalPublisher;
        private readonly ILogger<ApiCqrsResponse> log;

        public ApiCqrsResponse(IHttpContextAccessor httpContextAccessor, IPublisher<ICommand> publisher, IPublisher<ISignal> signalPublisher, ILogger<ApiCqrsResponse> log) : base(httpContextAccessor)
        {
            this.publisher = publisher;
            this.signalPublisher = signalPublisher;
            this.log = log;
        }

        public IActionResult FromProjection<TProjection, TResponse>(ReadResult<TProjection> result, Func<ReadResult<TProjection>, TResponse> onSuccess, Func<TResponse> onNotFound = null)
            where TResponse : class, new()
        {
            if (result.IsSuccess)
            {
                TResponse response = onSuccess(result);
                if (response is IActionResult innerResponse)
                    return innerResponse;
                return Success(response);
            }

            if (result.NotFound)
            {
                log.LogWarning($"Instance of projection {typeof(TProjection).Name} was not found. " + result.NotFoundHint);
                if (onNotFound is null)
                    return new ProblemObjectResult(new ResourceNotFoundProblemDetails(httpContextAccessor.HttpContext, $"Instance of projection {typeof(TProjection).Name} was not found. {result.NotFoundHint}"));
                else
                    return Success(onNotFound());
            }

            if (result.HasError)
            {
                log.LogError($"Failed to load instance of projection {typeof(TProjection).Name}. {result.Error}");
                return new ProblemObjectResult(new ProjectionProblemDetails(httpContextAccessor.HttpContext, new string[] { result.Error }));
            }

            return Success(new TResponse());
        }

        public IActionResult FromPublishCommand(ICommand command)
        {
            return FromPublishCommand(command, () => true);
        }

        public IActionResult FromPublishCommand(ICommand command, string id)
        {
            return FromPublishCommand(command, () => new PublishedId(id));
        }

        public IActionResult FromPublishCommand(ICommand command, Func<object> response)
        {
            try
            {
                bool published = publisher.Publish(command);

                return published
                    ? Success(response())
                    : PublisherProblem(command);
            }
            catch (Exception ex)
            {
                log.LogError(ex,ex.Message);
                return PublisherProblem(command);
            }
        }

        public IActionResult FromPublishSignal(ISignal signal)
        {
            try
            {
                bool published = signalPublisher.Publish(signal);

                return published
                    ? Success(true)
                    : PublisherProblem(signal);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                return PublisherProblem(signal);
            }
        }

        private IActionResult PublisherProblem(IMessage message)
        {
            var problemDetails = new PublisherProblemDetails(httpContextAccessor.HttpContext, message);

            log.LogError(System.Text.Json.JsonSerializer.Serialize(problemDetails));

            return new ProblemObjectResult(problemDetails);
        }

        public class PublishedId
        {
            public PublishedId(string id)
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentException("message", nameof(id));

                Id = id;
            }

            public string Id { get; set; }
        }
    }
}

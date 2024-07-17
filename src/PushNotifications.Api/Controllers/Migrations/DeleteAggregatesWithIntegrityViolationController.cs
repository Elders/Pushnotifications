using Elders.Cronus;
using Microsoft.AspNetCore.Mvc;
using PushNotifications.MigrationSignals;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PushNotifications.Api.Controllers.Migrations
{
    [Route("Migration")]
    public class DeleteAggregatesWithIntegrityViolationController : ApiControllerBase
    {
        private readonly IPublisher<ISignal> publisher;
        private readonly ApiCqrsResponse response;

        public DeleteAggregatesWithIntegrityViolationController(IPublisher<ISignal> publisher, ApiCqrsResponse response)
        {
            this.publisher = publisher;
            this.response = response;
        }

        [HttpPost, Route("DeleteAggregatesWithIntegrityViolation")]
        public Task<IActionResult> DeleteAggregatesWithIntegrityViolation(DeleteAggregatesWithIntegrityViolationRequestModel request)
        {
            var signal = new DeleteAggregatesWithIntegrityViolationSignal(request.Tenant, request.DryRun.Value);
            bool success = publisher.Publish(signal);
            if (success)
            {
                return Task.FromResult(response.Success($"{nameof(DeleteAggregatesWithIntegrityViolation)} - OK"));
            }
            else
            {
                return Task.FromResult(response.UnknownProblem($"{nameof(DeleteAggregatesWithIntegrityViolation)} - FAIL"));
            }
        }

        public class DeleteAggregatesWithIntegrityViolationRequestModel
        {
            [Required]
            public string Tenant { get; set; }

            [Required]
            public bool? DryRun { get; set; } = true;
        }
    }
}

using System;
using Elders.Cronus;
using Microsoft.AspNetCore.Mvc;
using PushNotifications.Api.Controllers;
using PushNotifications.Api.Controllers.Subscriptions.Commands;
using PushNotifications.MigrationSignals;

namespace PushNotifications.Api
{
    [Route("RefreshPnEvents")]
    public class GavrailDeletesUnsubscribeDublicateController : ApiControllerBase
    {
        private readonly IPublisher<ISignal> _publisher;

        public GavrailDeletesUnsubscribeDublicateController(IPublisher<ISignal> publisher)
        {
            _publisher = publisher;
        }

        [HttpGet]
        public IActionResult RefreshPNEvents()
        {
            GavrailDeletesUnsubscribeDublicateEventsSignal signal = new GavrailDeletesUnsubscribeDublicateEventsSignal("pruvit", true, DateTimeOffset.UtcNow);
            _publisher.Publish(signal);

            return Ok("Aideeeeee");
        }
    }
}

﻿using System;
using Elders.Cronus;
using Microsoft.AspNetCore.Mvc;
using PushNotifications.Api.Controllers.Subscriptions.Commands;
using PushNotifications.MigrationSignals;

namespace PushNotifications.Api.Controllers
{
    [Route("Events")]
    public class GavrailDeletesUnsubscribeDublicateController : ApiControllerBase
    {
        private readonly IPublisher<ISignal> _publisher;

        public GavrailDeletesUnsubscribeDublicateController(IPublisher<ISignal> publisher)
        {
            _publisher = publisher;
        }

        [HttpPost, Route("clearDuplicates")]
        public IActionResult RefreshPNEvents([FromBody] EventsCleanRequest request)
        {
            GavrailDeletesUnsubscribeDublicateEventsSignal signal = new GavrailDeletesUnsubscribeDublicateEventsSignal(request.Tenant, request.DryRun, DateTimeOffset.UtcNow);
            _publisher.Publish(signal);

            return Ok();
        }

        public class EventsCleanRequest
        {
            public string Tenant { get; set; }
            public bool DryRun { get; set; }
        }
    }
}

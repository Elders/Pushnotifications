using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Discovery.Contracts;
using Elders.Web.Api;
using Elders.Web.Api.RExamples;
using PushNotifications.Api.Extensions;

namespace Vapt.Api.Discovery
{
    public class NormalizedDiscoveryController : ApiController
    {
        public IDiscoveryReader DiscoveryReader { get; set; }

        const string currentBoundedContextName = "Pushnotifications";

        /// <summary>
        /// Normalized Discovery endpoint
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpHead, HttpGet, Route("Discovery/normalized")]
        public IHttpActionResult Discovery()
        {
            DiscoveryReaderResponseModel model = DiscoveryReader.Get(currentBoundedContextName);
            return Ok(new ResponseResult<DiscoveryReaderResponseModel>(model))
                .SetLastModifiedHeader(DateTime.FromFileTimeUtc(model.UpdatedAt));
        }

        public class Examples : IProvideRExamplesFor<NormalizedDiscoveryController>
        {
            public IEnumerable<IRExample> GetRExamples()
            {
                var endpoints = new List<DiscoverableEndpoint>
            {
                new DiscoverableEndpoint("watch", new Uri("http://www.test.com/v1/watch"), "singlemenBoundedContext", new DiscoveryVersion("v1")),
                new DiscoverableEndpoint("watch", new Uri("http://www.test.com/v2/watch"), "singlemenBoundedContext", new DiscoveryVersion("v2")),
                new DiscoverableEndpoint("happyfaptime", new Uri("http://www.test.com/v1/happyfaptime"), "singlemenBoundedContext", new DiscoveryVersion("v1")),
                new DiscoverableEndpoint("nun101", new Uri("https://anunslife.org/how-to-become-a-nun"), "catholicBoundedContext", new DiscoveryVersion("v1"))
            };
                DiscoveryReaderResponseModel model = new DiscoveryReaderResponseModel(DateTime.UtcNow.ToFileTimeUtc(), endpoints);
                yield return new StatusRExample(HttpStatusCode.OK, model);
            }
        }
    }
}

using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Discovery.Contracts;
using Elders.Web.Api;
using Elders.Web.Api.RExamples;

namespace PushNotifications.Api.Controllers.Discovery
{
    public class HealthCheckController : ApiController
    {
        public IDiscoveryReader DiscoveryReader { get; set; }

        /// <summary>
        /// Health Check endpoint
        /// </summary>
        /// <returns>Ok</returns>
        [AllowAnonymous]
        [HttpGet, Route("HealthCheck")]
        public IHttpActionResult Discovery()
        {
            return Ok(new ResponseResult());
        }

        public class Examples : IProvideRExamplesFor<HealthCheckController>
        {
            public IEnumerable<IRExample> GetRExamples()
            {
                yield return new StatusRExample(HttpStatusCode.OK, new ResponseResult());
            }
        }
    }
}

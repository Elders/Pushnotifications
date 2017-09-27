using System.Web.Http;
using Discovery;
using Elders.Web.Api;

namespace Vapt.Api.Discovery
{
    public class HealthCheckController : ApiController
    {
        public IDiscoveryReader DiscoveryReader { get; set; }

        /// <summary>
        /// PN Health Check envpoint
        /// </summary>
        /// <returns>Ok</returns>
        [AllowAnonymous]
        [HttpGet, Route("HealthCheck")]
        public IHttpActionResult Discovery()
        {
            return Ok(new ResponseResult());
        }
    }
}

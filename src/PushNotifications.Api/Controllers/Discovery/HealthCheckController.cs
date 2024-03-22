using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PushNotifications.Api.Controllers.Discovery
{
    public class HealthCheckController : ApiControllerBase
    {
        /// <summary>
        /// Health Check endpoint
        /// </summary>
        /// <returns>Ok</returns>
        [AllowAnonymous]
        [HttpGet, Route("HealthCheck")]
        public IActionResult Discovery()
        {
            return Ok();
        }
    }
}

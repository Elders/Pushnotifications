using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elders.Cronus;
using Elders.Discovery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace PushNotifications.Api.Controllers.Discovery
{
    public class DiscoveryController : ApiControllerBase
    {
        private readonly ApiResponse response;
        private readonly IDiscoveryReader discoveryReader;
        private readonly BoundedContext boundedContext;

        public DiscoveryController(ApiResponse response, IDiscoveryReader discoveryReader, IOptionsMonitor<BoundedContext> optionsMonitor)
        {
            this.response = response;
            this.discoveryReader = discoveryReader;
            this.boundedContext = optionsMonitor.CurrentValue;
        }

        /// <summary>
        /// Discovery endpoint
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpHead, HttpGet, Route("Discovery")]
        public async Task<IActionResult> DiscoveryAsync()
        {
            DiscoveryResponse model = await discoveryReader.GetAsync(boundedContext.Name);

            return response.Success(model.BoundedContextsToDictionary()).SetLastModifiedHeader(DateTimeOffset.FromFileTime(model.UpdatedAt));
        }
    }

    static class DiscoveryReaderResponseModelExtension
    {
        public static Dictionary<string, Dictionary<string, Dictionary<string, string>>> BoundedContextsToDictionary(this DiscoveryResponse model)
        {
            var result = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

            var groupedByBc = model.Endpoints.GroupBy(x => x.BoundedContext, (k, v) => new { BoundedContext = k, Endpoints = v });

            foreach (var bc in groupedByBc)
            {
                var boundedContextWithVersions = new Dictionary<string, Dictionary<string, string>>();

                var groupedByVersion = bc.Endpoints.GroupBy(x => x.Version.IntroducedAtVersion, (k, v) => new { IntroducedAtVersion = k, Endpoints = v }).OrderBy(x => x.IntroducedAtVersion);

                foreach (var version in groupedByVersion)
                {
                    var versionWithEndpoints = new Dictionary<string, string>();
                    foreach (var endpoint in version.Endpoints)
                    {
                        if (versionWithEndpoints.ContainsKey(endpoint.Name) == false)
                            versionWithEndpoints.Add(endpoint.Name, endpoint.Url.ToString());
                    }
                    if (boundedContextWithVersions.ContainsKey(version.IntroducedAtVersion) == false)
                        boundedContextWithVersions.Add(version.IntroducedAtVersion, versionWithEndpoints);
                }

                if (result.ContainsKey(bc.BoundedContext) == false)
                    result.Add(bc.BoundedContext, boundedContextWithVersions);

            }
            return result;
        }
    }
}

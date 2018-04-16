using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Discovery.Contracts;
using Elders.Web.Api;
using Elders.Web.Api.RExamples;
using PushNotifications.Api;

namespace Vapt.Api.Discovery
{
    public class DiscoveryController : ApiController
    {
        public IDiscoveryReader DiscoveryReader { get; set; }

        const string currentBoundedContextName = "Pushnotifications";

        /// <summary>
        /// Discovery endpoint
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpHead, HttpGet, Route("Discovery")]
        public IHttpActionResult Discovery()
        {
            DiscoveryReaderResponseModel model = DiscoveryReader.Get(currentBoundedContextName);
            return Ok(new ResponseResult<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(model.BoundedContextsToDictionary())).SetLastModifiedHeader(DateTime.FromFileTimeUtc(model.UpdatedAt));
        }

        public class Examples : IProvideRExamplesFor<DiscoveryController>
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
                yield return new StatusRExample(HttpStatusCode.OK, new ResponseResult<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(model.BoundedContextsToDictionary()));
            }
        }
    }

    public static class DiscoveryReaderResponseModelExtension
    {
        public static Dictionary<string, Dictionary<string, Dictionary<string, string>>> BoundedContextsToDictionary(this DiscoveryReaderResponseModel model)
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

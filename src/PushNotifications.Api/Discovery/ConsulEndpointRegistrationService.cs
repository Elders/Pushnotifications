using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Consul;
using Discovery;
using Elders.Cronus.DomainModeling;

namespace PushNotifications.Api.Discovery
{
    public class ConsulEndpointRegistrationService
    {
        readonly ConsulClient client;

        readonly string consulNodeIp;

        public ConsulEndpointRegistrationService(ConsulClient client)
        {
            if (ReferenceEquals(null, client) == true) throw new ArgumentNullException(nameof(client));
            this.client = client;

            this.consulNodeIp = GetCurrentNodeIp();
            if (string.IsNullOrEmpty(consulNodeIp) == true) throw new ArgumentNullException(nameof(consulNodeIp));
        }

        public void RegisterServices(HttpConfiguration config, Assembly assembly, Uri boundedContextBaseUri)
        {
            var boundedContextName = assembly.GetBoundedContext().BoundedContextName;

            var methodsWithDiscoverableAttribute = assembly.GetTypes()
              .SelectMany(t => t.GetMethods())
              .Where(m => m.GetCustomAttributes(typeof(DiscoverableAttribute), false).Length > 0)
              .Select(m => new { Method = m, Attr = (DiscoverableAttribute)m.GetCustomAttribute(typeof(DiscoverableAttribute), false) })
              .ToArray();

            var httpCheck = new AgentServiceCheck { Interval = TimeSpan.FromMinutes(5), HTTP = boundedContextBaseUri.ToString() };

            foreach (var methodWithDiscoverableAttribute in methodsWithDiscoverableAttribute)
            {
                var method = methodWithDiscoverableAttribute.Method;
                var attr = methodWithDiscoverableAttribute.Attr;

                var endpoint = new DiscoverableEndpoint(attr.EndpointName, new Uri(boundedContextBaseUri, GetUrl(config, method)), boundedContextName, new DiscoveryVersion(attr.Version, attr.DepricateVersion));
                AppendToConsul(endpoint, httpCheck);
            }
        }

        public void RegisterService(DiscoverableEndpoint endpoint, Uri boundedContextBaseUri)
        {
            var httpCheck = new AgentServiceCheck { Interval = TimeSpan.FromMinutes(5), HTTP = boundedContextBaseUri.ToString() };
            AppendToConsul(endpoint, httpCheck);
        }

        void AppendToConsul(DiscoverableEndpoint endpoint, AgentServiceCheck check)
        {
            var bcTag = $"{ConsulTags.BoundedContext}{ConsulTags.Separator}{endpoint.BoundedContext}";
            var publicTag = $"{ConsulTags.Visability}{ConsulTags.Separator}public";
            var timeTag = $"{ConsulTags.UpdatedAt}{ConsulTags.Separator}{DateTime.UtcNow.ToFileTimeUtc()}";

            var introducedAtVersionTag = $"{ConsulTags.IntroducedAtVersion}{ConsulTags.Separator}{endpoint.Version.IntroducedAtVersion}";
            var depricatedAtVersionTag = $"{ConsulTags.DepricatedAtVersion}{ConsulTags.Separator}{endpoint.Version.DepricatedAtVersion}";
            var endpointNameTag = $"{ConsulTags.EndpointName}{ConsulTags.Separator}{endpoint.Name}";
            var endpointUrlTag = $"{ConsulTags.EndpointUrl}{ConsulTags.Separator}{endpoint.Url}";

            var registration = new AgentServiceRegistration()
            {
                ID = $"{endpoint.BoundedContext}-{endpoint.Name}-{endpoint.Version.IntroducedAtVersion}",
                Name = $"{endpoint.BoundedContext}-{endpoint.Name}-{endpoint.Version.IntroducedAtVersion}",
                Address = consulNodeIp,
                Tags = new[] { bcTag, introducedAtVersionTag, depricatedAtVersionTag, endpointUrlTag, endpointNameTag, timeTag, publicTag },
                Check = check
            };
            var result = client.Agent.ServiceRegister(registration).Result;
            //var result = client.Catalog.Services().Result;
            //foreach (var item in result.Response)
            //{
            //    client.Agent.ServiceDeregister(item.Key);
            //}
        }

        string GetCurrentNodeIp()
        {
            var self = client.Agent.Self().Result;
            if (ReferenceEquals(null, self) == true) return string.Empty;

            var consulCfg = self.Response.Where(x => x.Key == "Config").FirstOrDefault();
            if (ReferenceEquals(null, consulCfg) == true) return string.Empty;

            var clientAddrCfg = consulCfg.Value.Where(x => x.Key == "ClientAddr").FirstOrDefault();
            if (ReferenceEquals(null, clientAddrCfg) == true) return string.Empty;

            var ip = clientAddrCfg.Value;
            return ip;
        }

        static string GetUrl(HttpConfiguration config, MethodInfo method)
        {
            return config
                   .Services
                   .GetApiExplorer()
                   .ApiDescriptions.Where(x => x.ActionDescriptor.ActionName == method.Name && x.ActionDescriptor.ControllerDescriptor.ControllerType == method.DeclaringType && x.HttpMethod != HttpMethod.Head)
                   .Single().Route.RouteTemplate;
        }
    }
}

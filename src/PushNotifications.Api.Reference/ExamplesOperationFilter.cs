using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;
using Elders.Web.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Swagger;
using Elders.Web.Api.RExamples;
using System.Net.Http;
using PushNotifications.Api.Attributes;
using System;
using Newtonsoft.Json.Linq;

namespace PushNotifications.Api.Reference
{
    public class ExamplesOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            SetRequestModelExamples(operation, schemaRegistry, apiDescription);
            SetResponseModelExamples(operation, schemaRegistry, apiDescription);
        }

        private static void SetRequestModelExamples(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            IEnumerable<IRExample> rexamples = RExamplesRegistry.GetExamples(apiDescription);

            foreach (var rex in rexamples)
            {
                var schema = schemaRegistry.GetOrRegister(rex.RType);
                var request = operation?.parameters?.FirstOrDefault(p => p.@in == "body" && p.schema.@ref == schema.@ref);

                if (request != null)
                {
                    var parts = schema.@ref.Split('/');
                    if (parts == null)
                        continue;

                    var name = parts.Last();

                    var definitionToUpdate = schemaRegistry.Definitions[name];

                    if (definitionToUpdate != null)
                    {
                        var formattedAsJson = FormatAsJson(rex.Example);
                        definitionToUpdate.example = formattedAsJson;
                    }
                }
            }

            if (apiDescription.HttpMethod == HttpMethod.Get && operation.parameters != null)
            {
                var originalParameters = new List<Parameter>(operation.parameters);
                operation.parameters.Clear();
                var fromHeaderParams = apiDescription.ActionDescriptor.GetParameters();
                foreach (var param in fromHeaderParams)
                {
                    var rex = rexamples.SingleOrDefault(x => x.RType == param.ParameterType);
                    if (rex != null)
                    {
                        Newtonsoft.Json.Linq.JObject formatted = JObject.FromObject(FormatAsJson(rex.Example));
                        if (ReferenceEquals(null, formatted) == true) continue;

                        var propertiesFromToken = rex.RType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Where(x => x.GetCustomAttributes(false).Any(a => a is ClaimsIdentityAttribute)).ToList().Select(x => x.Name.ToLower());
                        var requiiredProperties = rex.RType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Where(x => x.GetCustomAttributes(false).Any(a => a is System.ComponentModel.DataAnnotations.RequiredAttribute || a is ClaimsIdentityAttribute)).ToList().Select(x => x.Name.ToLower());
                        foreach (var item in formatted)
                        {
                            var @in = propertiesFromToken.Contains(item.Key.ToLower()) ? "token or query" : "query";
                            var paramuhj = new Parameter()
                            {
                                type = "string",
                                name = item.Key,
                                description = $"Example: {item.Value.ToString()}",
                                required = requiiredProperties.Contains(item.Key.ToLower()),
                                @in = @in,
                            };

                            operation.parameters.Add(paramuhj);
                        }
                    }
                    else
                    {
                        var currentParam = originalParameters.Where(x => x.name.ToLower() == param.ParameterName.ToLower()).SingleOrDefault();
                        if (currentParam != null)
                            operation.parameters.Add((Parameter)currentParam);
                    }
                }
            }
        }

        private static void SetResponseModelExamples(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (apiDescription.HttpMethod == HttpMethod.Head) return;

            IEnumerable<IRExample> rexamples = RExamplesRegistry.GetExamples(apiDescription);

            var responseStatus = rexamples.Where(x => x is StatusRExample).Select(x => x as StatusRExample).OrderBy(x => (int)x.StatusCode);
            if (responseStatus.Count() > 0)
                operation.responses.Clear();

            foreach (var rs in responseStatus)
            {
                operation.responses.Add(((int)rs.StatusCode).ToString(), new Response
                {
                    description = rs.StatusCode.ToString(),
                    schema = schemaRegistry.GetOrRegister(rs.Example.GetType())
                });
            }

            foreach (var rex in rexamples)
            {
                var schema = schemaRegistry.GetOrRegister(rex.RType);

                var response =
                    operation.responses.FirstOrDefault(
                        x => x.Value != null && x.Value.schema != null && x.Value.schema.@ref == schema.@ref);

                if (response.Equals(default(KeyValuePair<string, Response>)) == false)
                {
                    if (response.Value != null)
                    {
                        response.Value.examples = FormatAsJson(rex.Example);
                    }
                }
            }
        }

        private static object FormatAsJson(object example)
        {
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var assemblies = new[]
            {
                typeof(PushNotificationsApiAssembly).Assembly
            };
            var converters = assemblies.SelectMany(x => x.GetTypes())
                .Where(x => typeof(JsonConverter).IsAssignableFrom(x) && x.IsAbstract == false);

            foreach (var item in converters)
            {
                settings.Converters.Add(Activator.CreateInstance(item) as JsonConverter);
            }

            var jsonString = JsonConvert.SerializeObject(example, settings);
            var result = JsonConvert.DeserializeObject(jsonString);
            return result;
        }
    }
}

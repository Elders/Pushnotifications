using System;
using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PushNotifications.Api.Converters
{
    public class UrlBinder : IModelBinder
    {
        static JsonSerializer serializer;
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var converters = this.GetType().Assembly.GetTypes().Where(x => typeof(JsonConverter).IsAssignableFrom(x) && x.IsAbstract == false).Select(x => Activator.CreateInstance(x) as JsonConverter).ToList();
            var instance = Activator.CreateInstance(bindingContext.ModelType);
            var properties = bindingContext.ModelType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var obj = JObject.FromObject(instance);

            foreach (var item in properties)
            {
                ValueProviderResult val = bindingContext.ValueProvider.GetValue(item.Name.ToLowerInvariant());
                if (val == null)
                    continue;
                //var converter = converters.First(x => x.CanConvert(item.PropertyType) && x.CanRead);
                bool isOfType = val.RawValue is string || val.RawValue is int;
                int intValue;
                if (int.TryParse(val.RawValue.ToString(), out intValue))
                {
                    obj[item.Name] = JToken.FromObject(intValue);
                }
                else
                    obj[item.Name] = JToken.FromObject(val.RawValue);

            }
            if (serializer == null)
                serializer = JsonSerializer.CreateDefault(actionContext.RequestContext.Configuration.Formatters.JsonFormatter.SerializerSettings);
            //actionContext.RequestContext.Configuration.Formatters.JsonFormatter.CreateJsonSerializer()
            // var token = JToken.FromObject(obj);
            var result = serializer.Deserialize(JToken.Parse(obj.ToString(Formatting.None)).CreateReader(), bindingContext.ModelType);
            if (result == null)
                return false;
            bindingContext.Model = result;

            return true;
        }
    }
}

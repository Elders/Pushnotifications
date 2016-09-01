using Newtonsoft.Json;

namespace PushNotifications.Api.Client.Infrastructure
{
    public static class SerializerFactory
    {
        static JsonSerializer serializer;

        public static JsonSerializer GetSerializer()
        {
            if (serializer == null)
            {
                var settings = DefaultSettings();
                serializer = JsonSerializer.Create(settings);
            }

            return serializer;
        }

        static JsonSerializerSettings DefaultSettings()
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            settings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

            return settings;
        }
    }
}

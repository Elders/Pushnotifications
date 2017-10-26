using System;
using System.Web.Http;
using Newtonsoft.Json;
using System.Text;
using PushNotifications.Api.Host.Logging;

namespace PushNotifications.Api.Host
{
    public class ErrorConverterWithExpandedErrorMessage : JsonConverter
    {
        static readonly ILog log = LogProvider.GetLogger(typeof(ErrorConverterWithExpandedErrorMessage));

        readonly Func<Microsoft.Owin.IOwinContext> getOwinContext;

        public ErrorConverterWithExpandedErrorMessage(Func<Microsoft.Owin.IOwinContext> getOwinContext)
        {
            if (getOwinContext == null)
                throw new ArgumentNullException(nameof(getOwinContext));

            this.getOwinContext = getOwinContext;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var casted = value as HttpError;

            var ctx = getOwinContext();

            bool includeStackTrace = false;
#if DEBUG
            includeStackTrace = true;
#endif
            var response = new Elders.Web.Api.ResponseResult(GetString(casted, includeStackTrace));

            log.Error("[RequestError]" + GetString(casted, ctx, true));

            var jObject = Newtonsoft.Json.Linq.JObject.FromObject(response, serializer);
            jObject.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HttpError);
        }

        string GetString(HttpError error, Microsoft.Owin.IOwinContext ctx, bool includeStackTrace)
        {
            var sb = new StringBuilder();
            sb.Append(Environment.NewLine);
            sb.Append(ctx.Request.Uri.AbsoluteUri);
            sb.Append(Environment.NewLine);
            sb.Append(ctx.Request.Method);
            foreach (var item in ctx.Request.Headers)
            {
                foreach (var value in item.Value)
                {
                    sb.Append(Environment.NewLine);
                    sb.Append(item.Key + ": " + value);
                }
            }
            if (error.ModelState != null)
            {
                sb.Append(Environment.NewLine);
                sb.Append(JsonConvert.SerializeObject(error.ModelState));
            }

            sb.Append(Environment.NewLine);
            sb.Append(GetString(error, includeStackTrace));
            return sb.ToString();
        }

        string GetString(HttpError error, bool includeStackTrace)
        {
            var sb = new StringBuilder();

            if (ReferenceEquals(null, error.Message) == false && error.Message.Length > 0)
            {
                sb.Append(Environment.NewLine);
                sb.Append("Message: " + error.Message);
            }

            if (ReferenceEquals(null, error.ExceptionType) == false && error.ExceptionType.Length > 0)
            {
                sb.Append(Environment.NewLine);
                sb.Append("ExeceptionType: " + error.ExceptionType);
            }

            if (ReferenceEquals(null, error.ExceptionMessage) == false && error.ExceptionMessage.Length > 0)
            {
                sb.Append(Environment.NewLine);
                sb.Append("ExeceptionMessage: " + error.ExceptionMessage);
            }

            if (includeStackTrace == true && ReferenceEquals(null, error.StackTrace) == false && error.StackTrace.Length > 0)
            {
                sb.Append(Environment.NewLine);
                sb.Append("Strack trace: " + error.StackTrace);
            }

            if (ReferenceEquals(null, error.InnerException) == false)
            {
                sb.Append(Environment.NewLine);
                sb.Append("InnerExeception: " + GetString(error.InnerException, includeStackTrace));
            }

            return sb.ToString();
        }
    }
}

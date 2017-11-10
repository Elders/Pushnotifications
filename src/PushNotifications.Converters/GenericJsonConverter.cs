using System;
using System.Collections.Generic;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PushNotifications.Converters
{
    public abstract class GenericJsonConverter<TFrom, TObject> : JsonConverter
    {
        public abstract object GetValue(TObject instance);

        public abstract TObject Convert(TFrom jObject, Type objectType, IEnumerable<Claim> claims);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken token = JToken.FromObject(value);

            if (token.Type != JTokenType.Object)
            {
                token.WriteTo(writer);
            }
            else
            {
                object id = null;
                if (!ReferenceEquals(null, value))
                {
                    id = GetValue((TObject)value);
                }

                writer.WriteValue(id);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value;
            if (CanConvertValue(reader.ValueType, ClaimsPrincipal.Current.Claims) == false)
            {
                return null;
            }

            return Convert((TFrom)System.Convert.ChangeType(reader.Value, typeof(TFrom), System.Globalization.CultureInfo.InvariantCulture), objectType, ClaimsPrincipal.Current.Claims);
        }

        public virtual bool CanConvertValue(Type valueType, IEnumerable<Claim> claims)
        {
            return valueType == typeof(TFrom);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public Type BindingType
        {
            get { return typeof(TObject); }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(TObject).IsAssignableFrom(objectType);
        }
    }


    public abstract class GenericJsonConverter<TObject> : JsonConverter
    {
        public abstract object GetValue(TObject instance);

        public abstract TObject Convert(JsonReader jObject, Type objectType, IEnumerable<Claim> claims);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken token = JToken.FromObject(value);

            if (token.Type != JTokenType.Object)
            {
                token.WriteTo(writer);
            }
            else
            {
                object id = null;
                if (!ReferenceEquals(null, value))
                {
                    id = GetValue((TObject)value);
                }
                writer.WriteValue(id);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return Convert(reader, objectType, ClaimsPrincipal.Current.Claims);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public Type BindingType
        {
            get { return typeof(TObject); }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(TObject).IsAssignableFrom(objectType);
        }
    }
}

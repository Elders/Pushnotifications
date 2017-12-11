using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;

namespace PushNotifications.Api.Host
{
    public static class Cert
    {
        public static byte[] GetCertBytes(string authority)
        {
            var builder = new UriBuilder(authority);
            builder.Path = "/.well-known/jwks";

            var request = WebRequest.Create(builder.Uri);

            using (var reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                var responseFromServer = reader.ReadToEnd();
                var response = JsonConvert.DeserializeObject<JwksResponse>(responseFromServer);
                var key = response.Keys.FirstOrDefault();
                if (ReferenceEquals(null, key))
                    throw new InvalidOperationException("Cannot get cert keys from " + authority);

                var utf8 = new UTF8Encoding();
                return utf8.GetBytes(key.x5c[0]);
            }
        }

        class JwksResponse
        {
            public IEnumerable<CertKey> Keys { get; set; }
        }

        class CertKey
        {
            public string kty { get; set; }
            public string use { get; set; }
            public string kid { get; set; }
            public string x5t { get; set; }
            public string e { get; set; }
            public string n { get; set; }
            public string[] x5c { get; set; }
        }
    }
}

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
    public class Cert
    {
        public static X509Certificate2 Load(string certThumbprint)
        {
            X509Certificate2 certificate = null;
            var store = new X509Store(StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            foreach (var cert in store.Certificates)
            {
                if (string.Compare(cert.Thumbprint, certThumbprint, System.StringComparison.OrdinalIgnoreCase) == 0)
                {
                    certificate = cert;
                    break;
                }
            }

            store.Close();
            return certificate;
        }

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
                    throw new InvalidOperationException("Cannot get Qore.Iaa cert keys");

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
            public string kty;
            public string use;
            public string kid;
            public string x5t;
            public string e;
            public string n;
            public string[] x5c;
        }
    }
}

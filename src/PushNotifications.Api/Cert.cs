using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace PushNotifications.Api
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

        private static byte[] ReadStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}

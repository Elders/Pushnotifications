using Elders.Pandora;

namespace PushNotifications.Api.Host.Extensions
{
    public static class PandoraExtensions
    {
        public static bool TryGet(this Pandora pandora, string key, out string value)
        {
            try
            { value = pandora.Get(key); return true; }
            catch (System.Collections.Generic.KeyNotFoundException) { value = null; return false; }
            catch (System.Exception) { throw; }
        }
    }
}

using Microsoft.Extensions.Configuration;
using OptionsExtensions;
using System.ComponentModel.DataAnnotations;

namespace PushNotifications.Delivery.Pushy
{
    public class PushyOptions
    {
        [Required]
        public string ApiKey { get; set; }
    }

    public class PushyOptionsProvider : OptionsProviderBase<PushyOptions>
    {
        public const string SettingKey = "pushy";

        public PushyOptionsProvider(IConfiguration configuration) : base(configuration) { }

        public override void Configure(PushyOptions options)
        {
            configuration.GetSection(SettingKey).Bind(options);
        }
    }
}

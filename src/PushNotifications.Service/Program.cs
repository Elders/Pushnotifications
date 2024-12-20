using System;
using Elders.Cronus;
using Elders.Pandora;
using Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OptionsExtensions;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Delivery.FireBase;
using PushNotifications.Delivery.Pushy;
using PushNotifications.Ports;
using Serilog;

namespace PushNotifications.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Microsoft.ApplicationInsights.Extensibility.Implementation.TelemetryDebugWriter.IsTracingDisabled = true;

            Environment.SetEnvironmentVariable("pandora_application", "push-notifications.ws", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("log_name", App.LogName, EnvironmentVariableTarget.Process);

            Start.WithStartupDiagnostics(App.Name, () => CreateHostBuilder(args).Build().Run());
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(cfg => cfg.Add(new PandoraConsulConfigurationSource(Environment.GetEnvironmentVariable("CONSUL_ADDRESS", EnvironmentVariableTarget.Process))))
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.AddLogging();
                    services.AddHostedService<Worker>();
                    services.AddOption<PushyOptions, PushyOptionsProvider>();
                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.AddApplicationInsightsTelemetryCronus();
                    services.AddCronus(hostContext.Configuration);

                    services.AddSingleton<LoggingHandler>();
                    services.AddOption<FireBaseOptions, FireBaseOptionsProvider>();

                    services.AddTransient<PushyApiKeyInjectHandler>();
                    services.AddHttpClient<PushyClient>(client =>
                    {
                        client.BaseAddress = new Uri("https://api.pushy.me/");
                    })
                    .AddHttpMessageHandler<PushyApiKeyInjectHandler>();

                    services.AddSingleton<FirebaseNotificationService>();
                    services.AddSingleton<FirebaseAppOptionsContainer>();
                    services.AddSingleton<IPushNotificationDelivery, FireBaseDelivery>();
                    services.AddSingleton<IPushNotificationDelivery, PushyDelivery>(); // TODO: This is using HTTP client, so it shouldn't be singleton !?
                    services.AddSingleton<MultiPlatformDelivery, MultiPlatformDelivery>();

                    services.AddSingleton<ITopicSubscriptionManager, FireBaseTopicSubscriptionManager>();
                    services.AddSingleton<ITopicSubscriptionManager, PushyTopicSubscriptionManager>();
                })
                .UseSerilog(SerilogConfiguration.Configure);
    }
}

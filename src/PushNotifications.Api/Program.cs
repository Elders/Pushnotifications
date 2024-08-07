using System;
using Elders.Pandora;
using Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace PushNotifications.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("pandora_application", "push-notifications.api", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("log_name", App.LogName, EnvironmentVariableTarget.Process);

            Start.WithStartupDiagnostics(App.Name, () => CreateHostBuilder(args).Build().Run());
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(cfg => cfg.Add(new PandoraConsulConfigurationSource(Environment.GetEnvironmentVariable("CONSUL_ADDRESS", EnvironmentVariableTarget.Process))))
                .ConfigureAppConfiguration((context, configApp) =>
                {
                    configApp.AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                    options.ValidateScopes = false;
                    options.ValidateOnBuild = false;
                })
                .UseSerilog(SerilogConfiguration.Configure);
    }
}

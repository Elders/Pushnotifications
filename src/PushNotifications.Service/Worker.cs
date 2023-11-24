using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Elders.Cronus;
using Elders.Cronus.Api;
using Elders.Cronus.Persistence.Cassandra.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PushNotifications.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> log;
        private readonly IServiceProvider provider;
        private readonly ICronusHost cronusHost;
        private IHost cronusApi;
        private readonly CronusApplicationInsightsProvider observer;
        IDisposable subscription;

        public Worker(ILogger<Worker> log, IServiceProvider provider, ICronusHost cronusHost, CronusApplicationInsightsProvider observer)
        {
            this.log = log;
            this.provider = provider;
            this.cronusHost = cronusHost;
            this.observer = observer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            log.LogInformation("Starting service...");

            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            subscription = DiagnosticListener.AllListeners.Subscribe(observer);

            await cronusHost.StartAsync();
            cronusApi = CronusApi.GetHost(builder =>
            {
                builder.AdditionalConfigurationSource = new Elders.Pandora.PandoraConsulConfigurationSource(Environment.GetEnvironmentVariable("CONSUL_ADDRESS", EnvironmentVariableTarget.Process));
            });
            await cronusApi.RunAsync(stoppingToken);

            log.LogInformation("Service started!");

            log.LogInformation("Migration started!");
            await provider.GetRequiredService<MigrateEventStore>().RunMigratorAsync("pruvit");
            log.LogInformation("Migration finished!");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            log.LogInformation("Stopping service...");

            await cronusHost.StopAsync();
            await cronusApi.StopAsync(TimeSpan.FromSeconds(1));

            log.LogInformation("Service stopped");
        }
    }
}

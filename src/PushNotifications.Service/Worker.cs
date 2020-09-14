using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Elders.Cronus;
using Elders.Cronus.Api;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PushNotifications.Service
{
    public class Worker : BackgroundService
    {
        private IHost cronusDashboard;
        private readonly ILogger<Worker> log;
        private readonly ICronusHost cronusHost;
        private readonly CronusApplicationInsightsProvider observer;
        IDisposable subscription;

        public Worker(ILogger<Worker> log, IServiceProvider provider, ICronusHost cronusHost)
        {
            this.log = log;
            this.cronusHost = cronusHost;
            //this.observer = observer;
            CronusBooter.BootstrapCronus(provider);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            log.LogInformation("Starting service...");

            //Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            //subscription = DiagnosticListener.AllListeners.Subscribe(observer);

            cronusHost.Start();
            cronusDashboard = CronusApi.GetHost();
            //await cronusDashboard.StartAsync();

            log.LogInformation("Service started!");
            //return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            log.LogInformation("Stopping service...");

            cronusHost.Stop();
            cronusDashboard.StopAsync(TimeSpan.FromSeconds(1));

            log.LogInformation("Service stopped");
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            StopAsync(CancellationToken.None).GetAwaiter().GetResult();
            subscription?.Dispose();
        }
    }
}

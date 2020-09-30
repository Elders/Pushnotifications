using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace PushNotifications.Service
{
    public static class CronusDiagnosticsServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationInsightsTelemetryCronus(this IServiceCollection services)
        {
            services.AddSingleton<CronusApplicationInsightsProvider>();

            return services;
        }
    }

    public sealed class CronusApplicationInsightsProvider : IObserver<DiagnosticListener>, IObserver<KeyValuePair<string, object>>
    {
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private readonly TelemetryClient telemetryClient;

        public CronusApplicationInsightsProvider(TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
        }

        void IObserver<DiagnosticListener>.OnNext(DiagnosticListener diagnosticListener)
        {
            var subscription = diagnosticListener.Subscribe(this);
            _subscriptions.Add(subscription);
        }
        void IObserver<DiagnosticListener>.OnError(Exception error) { }
        void IObserver<DiagnosticListener>.OnCompleted()
        {
            _subscriptions.ForEach(x => x.Dispose());
            _subscriptions.Clear();
        }

        void IObserver<KeyValuePair<string, object>>.OnCompleted() { }

        void IObserver<KeyValuePair<string, object>>.OnError(Exception error) { }

        void IObserver<KeyValuePair<string, object>>.OnNext(KeyValuePair<string, object> pair)
        {
            Write(pair.Key, pair.Value);
        }

        private void Write(string name, object value)
        {
            Activity activity = value as Activity;
            if (activity is null) return;

            var telemetry = ActivityToTelemetry<RequestTelemetry>(activity);
            telemetryClient.Track(telemetry);

        }

        private static T ActivityToTelemetry<T>(Activity activity) where T : OperationTelemetry, new()
        {
            Debug.Assert(activity.Id != null, "Activity must be started prior calling this method");

            var telemetry = new T { Name = activity.OperationName };
            telemetry.Duration = activity.Duration;

            OperationContext operationContext = telemetry.Context.Operation;
            operationContext.Name = activity.OperationName;

            if (activity.IdFormat == ActivityIdFormat.W3C)
            {
                operationContext.Id = activity.TraceId.ToHexString();
                telemetry.Id = activity.SpanId.ToHexString();

                if (string.IsNullOrEmpty(operationContext.ParentId) && activity.ParentSpanId != default)
                {
                    operationContext.ParentId = activity.ParentSpanId.ToHexString();
                }
            }
            else
            {
                operationContext.Id = activity.RootId;
                operationContext.ParentId = activity.ParentId;
                telemetry.Id = activity.Id;
            }

            foreach (var item in activity.Baggage)
            {
                if (!telemetry.Properties.ContainsKey(item.Key))
                {
                    telemetry.Properties.Add(item);
                }
            }

            foreach (var item in activity.Tags)
            {
                if (!telemetry.Properties.ContainsKey(item.Key))
                {
                    telemetry.Properties.Add(item);
                }
            }

            return telemetry;
        }
    }

}

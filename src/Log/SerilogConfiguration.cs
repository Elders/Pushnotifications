using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Text;

namespace Logging
{
    public static class SerilogConfiguration
    {
        public static void Configure(HostBuilderContext context, LoggerConfiguration config)
        {
            var builder = new ConfigurationBuilder();

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                builder.SetBasePath(Directory.GetCurrentDirectory());

            builder
                .AddEnvironmentVariables()
                .AddJsonFile($"serilog.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"serilog.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);

            config
                .MinimumLevel.Debug()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.WithMachineName()
                .Enrich.FromLogContext();
        }
    }

    public static class Start
    {
        public static void WithStartupDiagnostics(string applicationName, Action programExecution)
        {
            var builder = new ConfigurationBuilder();
            BuildConfiguration(builder);

            Serilog.Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                Serilog.Log.Information(new ServerInfo().ToString());

                Serilog.Log.Information($"Starting {applicationName} host...");

                programExecution();

                Serilog.Log.Information($"Host {applicationName} has stopped gracefully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                Serilog.Log.Fatal(ex, $"{applicationName} host terminated unexpectedly!");
            }
            finally
            {
                Serilog.Log.CloseAndFlush();
            }
        }

        static void BuildConfiguration(ConfigurationBuilder builder)
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                builder.SetBasePath(Directory.GetCurrentDirectory());
            };

            builder
                .AddEnvironmentVariables()
                .AddJsonFile("serilog.json", optional: false, reloadOnChange: true);

            string environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(environment) == false)
            {
                builder.AddJsonFile($"serilog.{environment}.json", optional: true);
            }
        }

        public class ServerInfo
        {
            public ServerInfo()
            {
                Environment = $"{System.Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT", EnvironmentVariableTarget.Process)} {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}";
                TimeZone = TimeZoneInfo.Local.ToSerializedString();
                OS = $"{System.Runtime.InteropServices.RuntimeInformation.OSDescription} {System.Runtime.InteropServices.RuntimeInformation.OSArchitecture}";
                Framework = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
            }

            public string Environment { get; set; }

            public string TimeZone { get; set; }

            public string OS { get; set; }

            public string Framework { get; set; }

            public override string ToString()
            {
                StringBuilder buffer = new StringBuilder();
                buffer.AppendLine($"Environment: {Environment}");
                buffer.AppendLine($"OS: {OS}");
                buffer.AppendLine($"Framework: {Framework}");
                buffer.AppendLine($"TimeZone: {TimeZone}");

                return buffer.ToString();
            }
        }
    }
}

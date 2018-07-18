using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using Serilog;
using Serilog.Events;

namespace HouseService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logFolder = Directory.CreateDirectory("logs");
            var logFileName = Path.Combine(logFolder.FullName, $"{DateTime.Now.ToString("HHmmss")}_.log");
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .Enrich.FromLogContext()
                            .WriteTo.File(logFileName,
                                                    outputTemplate: "{Timestamp:yyyy-MM-dd'T'HH:mm:ss.fff}|{Level}|[{SourceContext:1}]{RequestId}{Message}{NewLine}{Exception}",
                                                    flushToDiskInterval: new TimeSpan(0, 10, 0),
                                                    fileSizeLimitBytes: 10 * 1024 * 1024,
                                                    rollOnFileSizeLimit: true,
                                                    retainedFileCountLimit: 7,
                                                    rollingInterval: RollingInterval.Day,
                                                    shared: true)
                            .WriteTo.Console()
                            .CreateLogger();

            try
            {
                BuildWebHost(args).Run();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();
    }
}

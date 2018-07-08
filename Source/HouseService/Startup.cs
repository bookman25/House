using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using HouseService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using System.IO;
using System;

namespace HouseService
{
    public class Startup
    {
        private AutomationEngine Engine { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public async void ConfigureServices(IServiceCollection services)
        {
            var logFolder = Directory.CreateDirectory("logs");
            var logFileName = Path.Combine(logFolder.FullName, $"{DateTime.Now.ToString("HHmmss")}_.log");
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .WriteTo.File(logFileName,
                                                    outputTemplate: "{Timestamp:yyyy-MM-dd'T'HH:mm:ss.fff}|{Level}|{RequestId}{Message}{NewLine}{Exception}",
                                                    flushToDiskInterval: new TimeSpan(0, 10, 0),
                                                    fileSizeLimitBytes: 10 * 1024 * 1024,
                                                    rollOnFileSizeLimit: true,
                                                    retainedFileCountLimit: 5,
                                                    rollingInterval: RollingInterval.Day,
                                                    shared: true)
                            .WriteTo.Console()
                            .Enrich.FromLogContext()
                            .CreateLogger();

            services.AddMvc();

            Engine = new AutomationEngine(Configuration.GetSection("Hassio").Get<HassioOptions>());
            await Engine.StartAsync(default);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}

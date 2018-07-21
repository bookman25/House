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
using Autofac;
using HassSDK;
using HouseService.AutomationBase;
using HouseService.Automations;
using Autofac.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights.Extensibility;
using HouseService.ElasticSearch;
using HouseService.ViewModels;

namespace HouseService
{
    public class Startup
    {
        private IContainer ApplicationContainer { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            TelemetryConfiguration.Active.DisableTelemetry = true;

            services.AddMvc();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            containerBuilder.RegisterModule<AppModule>();

            ApplicationContainer = containerBuilder.Build();
            var provider = new AutofacServiceProvider(ApplicationContainer);

            return provider;
            //Engine = new AutomationEngine(Configuration.GetSection("Hassio").Get<HassioOptions>());
            //await Engine.StartAsync(default);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
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

            applicationLifetime.ApplicationStarted.Register(() => ApplicationContainer.Resolve<AutomationEngine>().StartAsync(default));
        }
    }

    public class AppModule : Module
    {
        protected override void Load(ContainerBuilder services)
        {
            AddSingleton<HassClient>(services);
            AddSingleton<SubscriptionClient>(services);
            AddSingleton<HassService>(services);
            AddSingleton<SensorService>(services);
            AddSingleton<AutomationEngine>(services);

            AddSingleton<Automation, KitchenLights>(services);
            AddSingleton<Automation, UpstairsClimate>(services);
            AddSingleton<Automation, DownstairsClimate>(services);
            AddSingleton<Automation, LivingRoomLights>(services);

            AddSingleton<ElasticSearchService>(services);
            AddSingleton<ElasticIndex, DownstairsThermostatIndex, DownstairsThermostatIndex>(services);
            AddSingleton<ElasticIndex, UpstairsThermostatIndex, UpstairsThermostatIndex>(services);

            AddSingleton<WebViewModel>(services);
        }

        private void AddSingleton<TImplementation>(ContainerBuilder services)
            where TImplementation : class
        {
            services.RegisterType<TImplementation>().SingleInstance();
        }

        private void AddSingleton<TService, TImplementation>(ContainerBuilder services)
            where TService : class
            where TImplementation : class, TService
        {
            services.RegisterType<TImplementation>()
               .As<TService>()
               .SingleInstance();
        }

        private void AddSingleton<TService, TService2, TImplementation>(ContainerBuilder services)
            where TService : class
            where TService2 : class
            where TImplementation : class, TService
        {
            services.RegisterType<TImplementation>()
               .As<TService>()
               .As<TService2>()
               .SingleInstance();
        }
    }
}

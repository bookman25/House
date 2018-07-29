using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using HassSDK;
using HouseService.Api.Models;
using HouseService.Api.Repositories;
using HouseService.AutomationBase;
using HouseService.Automations;
using HouseService.ElasticSearch;
using HouseService.Services;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Source
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });

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

            AddSingleton<AutomationRepository>(services);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MassTransmitDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MassTransmitDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            #region MassTransit
            services.AddScoped<IExampleService, ExampleService>();
            services.AddScoped<ExampleConsumer>();

            services.AddMassTransit(x =>
            {
                // add the consumer, for LoadFrom
                x.AddConsumer<ExampleConsumer>();
            });

            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host("localhost", "/", h => { });

                cfg.ReceiveEndpoint(host, "web-service-home-endpoint", e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(x => x.Interval(2, 100));

                    e.LoadFrom(provider);

                    EndpointConvention.Map<ExampleRequest>(e.InputAddress);
                });
            }));

            services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());

            services.AddScoped(provider => provider.GetRequiredService<IBus>().CreateRequestClient<ExampleRequest>());

            services.AddSingleton<IHostedService, EventBusService>();
            #endregion MassTransit
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}

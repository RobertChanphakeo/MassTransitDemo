using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MassTransit;
using MassTransit.Util;
using MassTransitDemo.Consumer2.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MassTransitDemo.Consumer2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IContainer container { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            #region MassTransit
            var builder = new ContainerBuilder();
            builder.Register(c =>
            {
                var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint(host, "AddUser1", e =>
                    {
                        e.Consumer<AddUserConsumer>();
                    });
                });
                return busControl;

            })
            .As<IBusControl>()
            .As<IBus>()
            .As<IPublishEndpoint>()
            .SingleInstance();
            builder.Populate(services);
            container = builder.Build();

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(container);
            #endregion MassTransit
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
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

            #region MassTransit
            //resolve the bus from the container
            var bus = container.Resolve<IBusControl>();
            //start the bus
            var busHandle = TaskUtil.Await(() => bus.StartAsync());

            //register an action to call when the application is shutting down
            lifetime.ApplicationStopping.Register(() => busHandle.Stop());
            #endregion MassTransit
        }
    }
}

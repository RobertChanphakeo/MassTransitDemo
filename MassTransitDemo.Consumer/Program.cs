using MassTransit;
using MassTransitDemo.Consumer.Services;
using System;

namespace MassTransitDemo.Consumer
{
    class Program
    {
        static void Main(string[] args)
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
            busControl.Start();
        }
    }
}

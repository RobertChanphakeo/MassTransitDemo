using MassTransit;
using MassTransitDemo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MassTransitDemo.Consumer.Services
{
    public class AddUserConsumer : IConsumer<AddUser>
    {
        public Task Consume(ConsumeContext<AddUser> context)
        {
            Console.WriteLine($"Added user {context.Message.FirstName} {context.Message.LastName}");
            return Task.CompletedTask;
        }
    }
}

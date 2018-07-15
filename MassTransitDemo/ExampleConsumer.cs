using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransmitDemo.Services;

namespace MassTransmitDemo
{
    public class ExampleConsumer : IConsumer<ExampleRequest>
    {
        private readonly IExampleService _exampleService;

        public ExampleConsumer(IExampleService exampleService)
        {
            _exampleService = exampleService;
        }

        public async Task Consume(ConsumeContext<ExampleRequest> context)
        {
            await _exampleService.Create(context.Message.Value);

            await context.RespondAsync<ExampleResponse>(new
            {
                Value = $"Received: {context.Message.Value}"
            });
        }
    }
}

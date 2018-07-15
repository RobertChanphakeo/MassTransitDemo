using System.Threading.Tasks;
using GreenPipes.Util;

namespace MassTransmitDemo.Services
{
    public class ExampleService : IExampleService
    {
        public Task Create(string value)
        {
            return TaskUtil.Completed;
        }
    }
}
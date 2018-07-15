using System.Threading.Tasks;

namespace MassTransmitDemo.Services
{
    public interface IExampleService
    {
        Task Create(string value);
    }
}
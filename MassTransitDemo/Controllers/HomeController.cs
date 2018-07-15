using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransmitDemo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MassTransmitDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IRequestClient<ExampleRequest> _requestClient;

        public HomeController(IRequestClient<ExampleRequest> requestClient)
        {
            _requestClient = requestClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            try
            {
                var request = _requestClient.Create(new { Value = "Hello, World." }, cancellationToken);

                var response = await request.GetResponse<ExampleResponse>();

                return Content($"{response.Message.Value}, MessageId: {response.MessageId:D}");
            }
            catch (RequestTimeoutException exception)
            {
                return StatusCode((int)HttpStatusCode.RequestTimeout);
            }
        }
    }
}
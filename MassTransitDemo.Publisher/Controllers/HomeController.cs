using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransitDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MassTransitDemo.Publisher.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IBus _bus;
        public HomeController(IBus bus)
        {
            _bus = bus;
        }

        [Route("/")]
        public async Task<IActionResult> Index()
        {
            var addUserEndpoint = await _bus.GetSendEndpoint(new Uri("rabbitmq://localhost/AddUser1"));
            await addUserEndpoint.Send<AddUser>(new
            {
                FirstName = "Robert",
                LastName = "Chanphakeo",
                EmailAddress = "robert@domain.com",
                Password = "P4ssw0rd1"
            });

            return Ok();
        }
    }
}
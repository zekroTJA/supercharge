using DDragonAccessLayer;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Filter;
using System.Net.Mime;

namespace RestAPI.Controllers
{
    [Route("[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [RateLimitFilter(1, 10)]
    [HeaderFilter]
    [ProxyAddressFilter]
    [ApiController]
    public class ResourcesController : ControllerBase
    {
        private readonly DataDragonWrapper ddragon;

        public ResourcesController(DataDragonWrapper ddragon)
        {
            this.ddragon = ddragon;
        }

        [HttpGet("[action]")]
        public IActionResult Version() =>
            Ok(new { version = ddragon.Version });

        [HttpGet("[action]")]
        public IActionResult Champions() =>
            Ok(ddragon.Champions);
    }
}
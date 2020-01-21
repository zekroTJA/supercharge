using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using DDragonAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Filter;

namespace RestAPI.Controllers
{
    [Route("[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [HeaderFilter]
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
using DatabaseAccessLayer;
using DDragonAccessLayer;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Filter;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace RestAPI.Controllers
{
    [Route("[controller]/{server}/{summonerName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [HeaderFilter]
    [ApiController]
    public class SummonerController : ControllerBase
    {
        private readonly DatabaseAccess dal;
        private readonly DataDragonWrapper ddragon;

        public SummonerController(DatabaseAccess dal, DataDragonWrapper ddragon)
        {
            this.dal = dal;
            this.ddragon = ddragon;
        }

        [HttpGet]
        public async Task<IActionResult> Summoner([FromRoute] string server, [FromRoute] string summonerName)
        {
            var user = await dal.GetUserByNameAsync(server, summonerName);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> Points(
            [FromRoute] string server, 
            [FromRoute] string summonerName,
            [FromQuery] string[] championNames)
        {
            var user = await dal.GetUserByNameAsync(server, summonerName);
            if (user == null)
                return NotFound();

            var championIds = championNames.Select(n => ddragon.GetChampionByName(n));

            var points = await dal.GetPointsViewAsync(user.Id, championIds);

            return Ok(points);
        }

        [HttpGet("history")]
        public async Task<IActionResult> History(
            [FromRoute] string server,
            [FromRoute] string summonerName,
            [FromQuery] string[] championNames,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var user = await dal.GetUserByNameAsync(server, summonerName);
            if (user == null)
                return NotFound();

            var championIds = championNames.Select(n => ddragon.GetChampionByName(n));

            var history = await dal.GetPointsLogViewAsync(user.Id, championIds, from, to);

            return Ok(history);
        }
    }
}
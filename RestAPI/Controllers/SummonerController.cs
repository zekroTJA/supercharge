using DatabaseAccessLayer;
using DDragonAccessLayer;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Filter;
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

            var points = await dal.GetPointsModelAsync(user.Id, championIds);

            return Ok(points);
        }
    }
}
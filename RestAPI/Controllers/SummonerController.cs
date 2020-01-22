using DatabaseAccessLayer;
using DDragonAccessLayer;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Filter;
using RestAPI.Models;
using RiotAPIAccessLayer;
using RiotAPIAccessLayer.Exceptions;
using RiotAPIAccessLayer.Models;
using System;
using System.Linq;
using System.Net;
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
        private readonly RiotAPIWrapper wrapper;
        private readonly DataDragonWrapper ddragon;

        public SummonerController(DatabaseAccess dal, RiotAPIWrapper wrapper, DataDragonWrapper ddragon)
        {
            this.dal = dal;
            this.wrapper = wrapper;
            this.ddragon = ddragon;
        }

        [HttpGet]
        public async Task<IActionResult> Summoner([FromRoute] string server, [FromRoute] string summonerName)
        {
            UserModel user = new UserModel();

            try
            {
                user = await wrapper.GetSummonerByName(server, summonerName);
            }
            catch (ResponseException e)
            {
                if (e.Response.StatusCode == HttpStatusCode.NotFound)
                    return NotFound();
            }

            var dbUser = await dal.GetUserByNameAsync(server, summonerName);

            var userView = new UserViewModel(user)
            {
                Registered = dbUser != null,
                Watch = dbUser?.Watch,
                Created = dbUser?.Created,
            };

            return Ok(userView);
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
using DatabaseAccessLayer;
using DDragonAccessLayer;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Filter;
using RestAPI.Models;
using RiotAPIAccessLayer;
using Shared.Time;
using System.Net.Mime;

namespace RestAPI.Controllers
{
    [Route("[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [HeaderFilter]
    [ProxyAddressFilter]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly DatabaseAccess dal;

        public StatusController(DatabaseAccess dal)
        {
            this.dal = dal;
        }

        [HttpGet("[action]")]
        [RateLimitFilter(10, 2)]
        public IActionResult Counts()
        {
            var counts = new CountsModel
            {
                Users = dal.GetUsersCount((_) => true).ToString(),
                UsersWatching = dal.GetUsersCount((u) => u.Watch).ToString(),
                Points = dal.GetPointsCount((_) => true).ToString(),
                PointsLog = dal.GetPointsLogCount((_) => true).ToString(),
            };

            return Ok(counts);
        }

        [HttpGet("[action]")]
        public IActionResult Versions()
        {
            var versions = new VersionsModel
            {
                RestAPI = GetVersion<Startup>(),
                DatabaseAccessLayer = GetVersion<DatabaseAccess>(),
                DDragonAccessLayer = GetVersion<DataDragonWrapper>(),
                RiotAPIAccessLayer = GetVersion<RiotAPIWrapper>(),
                Shared = GetVersion<TimeUtils>(),
            };

            return Ok(versions);
        }


        private string GetVersion<T>() =>
            typeof(T).Assembly.GetName().Version.ToString();
    }
}
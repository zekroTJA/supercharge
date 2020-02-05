using DatabaseAccessLayer;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Filter;
using RestAPI.Models;
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

        [HttpGet]
        [RateLimitFilter(10, 2)]
        public IActionResult Get()
        {
            var status = new StatusModel
            {
                Counts = new CountsModel
                {
                    Users = dal.GetUsersCount((_) => true).ToString(),
                    UsersWatching = dal.GetUsersCount((u) => u.Watch).ToString(),
                    Points = dal.GetPointsCount((_) => true).ToString(),
                    PointsLog = dal.GetPointsLogCount((_) => true).ToString(),
                }
            };

            return Ok(status);
        }

    }
}
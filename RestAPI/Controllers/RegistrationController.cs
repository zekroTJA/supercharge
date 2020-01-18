using DatabaseAccessLayer;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Caching;
using RestAPI.Models;
using RiotAPIAccessLayer;
using RiotAPIAccessLayer.Exceptions;
using RiotAPIAccessLayer.Models;
using Shared.Random;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace RestAPI.Controllers
{
    [Route("[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly DatabaseAccess dal;
        private readonly RiotAPIWrapper wrapper;
        private readonly RegistrationCache registrationCache;

        public RegistrationController(DatabaseAccess dal, RiotAPIWrapper wrapper, RegistrationCache registrationCache)
        {
            this.dal = dal;
            this.wrapper = wrapper;
            this.registrationCache = registrationCache;
        }

        [HttpGet("code/{server}/{userName}")]
        public IActionResult GetCode([FromRoute] string server, [FromRoute] string userName)
        {
            var code = RandomString.GetRandomString(16);
            registrationCache.RegistrationCodeCache[$"{server}/{userName}"] = code;

            return Ok(new RegistrationCodeModel
            {
                Code = code,
            });
        }

        [HttpPost("watch/{server}/{userName}")]
        public async Task<IActionResult> SetWatch(
            [FromRoute] string server, 
            [FromRoute] string userName)
        {
            var key = $"{server}/{userName}";

            if (!registrationCache.RegistrationCodeCache.ContainsKey(key))
                return Unauthorized();

            var stateCode = registrationCache.RegistrationCodeCache[key];

            UserModel user = new UserModel();
            string obtainedCode;

            try
            {
                user = await wrapper.GetSummonerByName(server, userName);
            }
            catch (ResponseException e)
            {
                if (e.Response.StatusCode == HttpStatusCode.NotFound)
                    return BadRequest("user not found");
            }

            try
            {
                obtainedCode = await wrapper.GetThirdPartyCode(server, user.Id);
                if (obtainedCode != stateCode)
                    return Unauthorized();
            }
            catch (ResponseException e)
            {
                if (e.Response.StatusCode == HttpStatusCode.NotFound)
                    return Unauthorized();
            }

            var dbUser = await dal.GetUserByNameAsync(server, user.Name);

            if (dbUser == null)
            {
                dbUser = new DatabaseAccessLayer.Models.UserModel
                {
                    Server = server,
                    Username = user.Name,
                    SummonerID = user.Id,
                    Watch = true,
                };

                dal.Add(dbUser);
            }
            else
            {
                dbUser.SummonerID = user.Id;
                dbUser.Watch = true;

                dal.Update(dbUser);
            }

            await dal.CommitChangesAsync();

            return Ok();
        }
    }
}
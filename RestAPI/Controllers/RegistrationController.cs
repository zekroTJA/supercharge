using DatabaseAccessLayer;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Caching;
using RestAPI.Models;
using Shared;
using Shared.Exceptions;
using Shared.Models;
using Shared.Random;
using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using RestAPI.Filter;

namespace RestAPI.Controllers
{
    [Route("[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [HeaderFilter]
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

        [HttpGet("[action]/{server}/{userName}")]
        public IActionResult Code([FromRoute] string server, [FromRoute] string userName)
        {
            var code = RandomString.GetRandomString(16);
            var expVal = registrationCache.RegistrationCodeCache.Set(
                $"{server}/{userName}", code, TimeSpan.FromMinutes(10));

            return Ok(new RegistrationCodeModel
            {
                Code = code,
                Expires = expVal.Expires,
            });
        }

        [HttpPost("[action]/{server}/{userName}")]
        public async Task<IActionResult> Watch(
            [FromRoute] string server, 
            [FromRoute] string userName)
        {
            var key = $"{server}/{userName}";

            if (!registrationCache.RegistrationCodeCache.ContainsKey(key))
                return Unauthorized();

            var stateCode = registrationCache.RegistrationCodeCache.Get(key);

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
                    SummonerId = user.Id,
                    Watch = true,
                };

                dal.Add(dbUser);
            }
            else
            {
                dbUser.SummonerId = user.Id;
                dbUser.Watch = true;

                dal.Update(dbUser);
            }

            await dal.CommitChangesAsync();

            return Ok();
        }

        [HttpPost("[action]/{server}/{userName}")]
        public async Task<IActionResult> UnWatch(
            [FromRoute] string server,
            [FromRoute] string userName)
        {
            var key = $"{server}/{userName}";

            if (!registrationCache.RegistrationCodeCache.ContainsKey(key))
                return Unauthorized();

            var stateCode = registrationCache.RegistrationCodeCache.Get(key);

            string obtainedCode;

            var user = await dal.GetUserByNameAsync(server, userName);

            if (user == null)
                return NotFound();

            try
            {
                obtainedCode = await wrapper.GetThirdPartyCode(server, user.SummonerId);
                if (obtainedCode != stateCode)
                    return Unauthorized();
            }
            catch (ResponseException e)
            {
                if (e.Response.StatusCode == HttpStatusCode.NotFound)
                    return Unauthorized();
            }

            user.Watch = false;

            dal.Update(user);

            await dal.CommitChangesAsync();

            return Ok();
        }
    }
}
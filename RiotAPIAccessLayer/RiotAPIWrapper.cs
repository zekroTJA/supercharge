using Microsoft.Extensions.Configuration;
using Shared.Models;
using System.Threading.Tasks;

namespace Shared
{
    public class RiotAPIWrapper
    {
        private readonly Requests requests;

        public RiotAPIWrapper(IConfiguration config)
        {
            requests = new Requests(config.GetValue<string>("RiotAPI:Secret"));
        }

        public async Task<UserModel> GetSummonerByName(string server, string username)
        {
            var res = await requests.Get<UserModel>(server, APIs.SUMMONER, "v4", $"summoners/by-name/{username}");

            return res;
        }

        public async Task<PointsModel[]> GetSummonerPoints(string server, string userId)
        {
            var res = await requests.Get<PointsModel[]>(server, APIs.CHAMPION_MASTERY, "v4", $"champion-masteries/by-summoner/{userId}");

            return res;
        }

        public async Task<string> GetThirdPartyCode(string server, string userId)
        {
            var res = await requests.Get<string>(server, APIs.PLATFORM, "v4", $"third-party-code/by-summoner/{userId}");

            return res;
        }
    }
}

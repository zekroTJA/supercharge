using Crawler.RiotAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Crawler.RiotAPI
{
    class RiotAPIWrapper
    {
        private readonly Requests requests;

        public RiotAPIWrapper(string apiToken)
        {
            requests = new Requests(apiToken);
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
    }
}

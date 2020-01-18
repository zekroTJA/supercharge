using Crawler.Exceptions;
using Crawler.RiotAPI;
using DatabaseAccessLayer;
using DatabaseAccessLayer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler
{

    class PointsCrawler
    {
        private readonly RiotAPIWrapper wrapper;
        private readonly List<DateTime> execOn;
        private readonly DatabaseAccess dal;
        private readonly ILogger<PointsCrawler> logger;

        private Timer timer;
        private readonly Dictionary<DateTime, DateTime> latelyExecuted = new Dictionary<DateTime, DateTime>();

        public PointsCrawler(IConfiguration config, DatabaseAccess dal, ILogger<PointsCrawler> logger)
        {
            execOn = config.GetSection("Crawler:ExecOn")
                .AsEnumerable()
                .Where(v => v.Value != null)
                .Select(v => DateTime.Parse(v.Value))
                .ToList();

            this.logger = logger;
            this.dal = dal;
            wrapper = new RiotAPIWrapper(config.GetValue<string>("RiotAPI:Secret"));
        }

        public void StartLoop()
        {
            logger.LogInformation(
                $"Loop initialized for following times: {string.Join(", ", execOn.Select(dt => $"{dt.Hour}:{dt.Minute}"))}");

            timer = new Timer(new TimerCallback(OnTimerElapse), null, 1000, 10_000);
        }

        private bool CheckTime(DateTime dt)
        {
            var now = DateTime.Now;
            if (now.Hour != dt.Hour || now.Minute != dt.Minute)
                return false;


            if (!latelyExecuted.ContainsKey(dt) || 
                now - latelyExecuted[dt] > TimeSpan.FromHours(23)) 
            {
                latelyExecuted[dt] = now; 
                return true;
            }

            return false;
        }

        private async void OnTimerElapse(object v)
        {
            if (execOn.FirstOrDefault(CheckTime) != default)
            {
                 await Exec();
            }
        }

        private async Task Exec()
        {
            logger.LogInformation("Executing loop function...");

            var users = await dal.GetUsersAsync();

            foreach (var user in users.Where(u => u.Watch))
            {
                /* If the Users ID, which is collected from the Riot API,
                 * null, so the ID will be firstly determinated by requesting
                 * the user profile from the Riot API.
                 * 
                 * If the response returns a 404 code, which identicates that
                 * the user account does not or does no more exist and the "Watch"
                 * flag in the database will be set to false.
                 */
                if (user.SummonerID == null)
                {
                    try
                    {
                        var resUser = await wrapper.GetSummonerByName(user.Server, user.Username);
                        user.SummonerID = resUser.Id;
                        logger.LogInformation($"Requested missing SummonerID of summoner '{user.Username}': ${user.SummonerID}");
                    }
                    catch (ResponseException e)
                    {
                        if (e.Response.StatusCode == HttpStatusCode.NotFound)
                        {
                            user.Watch = false;
                            logger.LogError("user could not be found by API - settings 'Watch' flag to false");
                        }
                    }
                    catch (Exception e) {
                        logger.LogError(e.ToString());
                    }
                }

                await dal.CommitChangesAsync();
                logger.LogInformation("Changed commited to database");
            }
        }
    }
}

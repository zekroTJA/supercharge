using DatabaseAccessLayer;
using DatabaseAccessLayer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RiotAPIAccessLayer;
using RiotAPIAccessLayer.Exceptions;
using Shared.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public PointsCrawler(IConfiguration config, DatabaseAccess dal, RiotAPIWrapper wrapper, ILogger<PointsCrawler> logger)
        {
            execOn = config.GetSection("Crawler:ExecOn")
                .AsEnumerable()
                .Where(v => v.Value != null)
                .Select(v => DateTime.Parse(v.Value))
                .ToList();

            this.logger = logger;
            this.dal = dal;
            this.wrapper = wrapper;
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
                try
                {
                    await Exec();
                }
                catch (Exception e)
                {
                    logger.LogError(e.ToString());
                }
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

                    dal.Update(user);
                }

                var pointsRes = await wrapper.GetSummonerPoints(user.Server, user.SummonerID);
                var pointsDb = await dal.GetPointsAsync(user.Id);

                Array.ForEach(pointsRes, p =>
                {
                    var pChamp = pointsDb.FirstOrDefault(pdb => pdb.ChampionId == p.ChampionId);

                    if (pChamp == null)
                    {
                        pChamp = new PointsModel
                        {
                            ChampionId = p.ChampionId,
                            ChampionLevel = p.ChampionLevel,
                            ChampionPoints = p.ChampionPoints,
                            LastPlayed = TimeUtils.UnixToDateTime(p.LastPlayed),
                            User = user,
                        };

                        dal.Add(pChamp);
                    } 
                    else
                    {
                        pChamp.ChampionLevel = p.ChampionLevel;
                        pChamp.ChampionPoints = p.ChampionPoints;
                        pChamp.LastPlayed = TimeUtils.UnixToDateTime(p.LastPlayed);
                        pChamp.Updated = DateTime.Now;

                        dal.Update(pChamp);
                    }

                    var pointsLog = new PointsLogModel
                    {
                        ChampionLevel = p.ChampionLevel,
                        ChampionId = p.ChampionId,
                        ChampionPoints = p.ChampionPoints,
                        User = user,
                    };

                    dal.Add(pointsLog);
                });

                await dal.CommitChangesAsync();
                logger.LogInformation("Changed commited to database");
            }
        }
    }
}

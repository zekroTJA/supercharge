using DatabaseAccessLayer;
using DatabaseAccessLayer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RiotAPIAccessLayer;
using Shared.Exceptions;
using Shared.Time;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler
{

    class PointsCrawler
    {
        private readonly RiotAPIWrapper wrapper;
        private readonly DatabaseAccess dal;
        private readonly ILogger<PointsCrawler> logger;
        private readonly Scheduler scheduler;

        private readonly int maxRequestsPerSecond = 18;
        private int requestCount = 0;

        public PointsCrawler(IConfiguration config, DatabaseAccess dal, RiotAPIWrapper wrapper, ILogger<PointsCrawler> logger)
        {
            var statusTimes = config.GetSection("Crawler:ExecOn:GetStatus")
                .AsEnumerable()
                .Where(v => v.Value != null)
                .Select(v => DateTime.Parse(v.Value))
                .ToList();

            var logTimes = config.GetSection("Crawler:ExecOn:GetLog")
                .AsEnumerable()
                .Where(v => v.Value != null)
                .Select(v => DateTime.Parse(v.Value))
                .ToList();

            maxRequestsPerSecond = config.GetValue<int>("Crawler:MaxRequestsPerSecond", maxRequestsPerSecond);

            scheduler = new Scheduler(TimeSpan.FromHours(23))
                .DoAt(async () => await GetStats(addToLog: false), statusTimes)
                .DoAt(async () => await GetStats(addToLog: true), logTimes);

            this.logger = logger;
            this.dal = dal;
            this.wrapper = wrapper;
        }

        public void StartLoop()
        {
            logger.LogInformation(
                $"Loop initialized");

            scheduler.Start(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
        } 

        private async Task GetSummonerID(UserModel user)
        {
            try
            {
                if (requestCount++ > maxRequestsPerSecond)
                {
                    logger.LogInformation("Max request coun exceed, waiting until continuing...");
                    Thread.Sleep(1_000);
                    requestCount = 0;
                }

                var resUser = await wrapper.GetSummonerByName(user.Server, user.Username);
                user.SummonerId = resUser.Id;
                logger.LogInformation($"Requested missing SummonerID of summoner '{user.Username}': ${user.SummonerId}");
            }
            catch (Exception e)
            {
                if (e is ResponseException && 
                    !(e as ResponseException).Response.IsSuccessStatusCode &&
                    (e as ResponseException).Response.StatusCode != HttpStatusCode.TooManyRequests)
                {
                    user.Watch = false;
                    logger.LogError("User could not be found by API - settings 'Watch' flag to false");
                } 
                else
                    logger.LogError($"An unexpected error occured during request: {e.Message}");
            }

            dal.Update(user);
        }

        private async Task GetStats(bool addToLog = false, bool isRetry = false)
        {
            logger.LogInformation($"Getting stats{(addToLog ? " and add to log" : "")}...");

            var users = await dal.GetUsersAsync();

            foreach (var user in users.Where(u => u.Watch))
            {
                if (requestCount++ > maxRequestsPerSecond)
                {
                    logger.LogInformation("Max request coun exceed, waiting until continuing...");
                    Thread.Sleep(1_000);
                    requestCount = 0;
                }

                if (user.SummonerId == null)
                    await GetSummonerID(user);

                RiotAPIAccessLayer.Models.PointsModel[] pointsRes;

                try
                {
                    pointsRes = await wrapper.GetSummonerPoints(user.Server, user.SummonerId);
                }
                catch(Exception e)
                {
                    if (e is ResponseException &&
                        !(e as ResponseException).Response.IsSuccessStatusCode &&
                        (e as ResponseException).Response.StatusCode != HttpStatusCode.TooManyRequests)
                    {
                        // If the data can not be fetched by the summoner ID saved in the databse, 
                        // then firstly try to collect the user ID of the user from the API again
                        // and retry this function with isRetry passed as true.
                        // If isRetry is true, the user will be 'Watch' flag of the user will be set
                        // to false to prevent further errors.
                        if (!isRetry)
                        {
                            logger.LogInformation($"Requesting stats of user '{user.Username}' failed: ${(e as ResponseException).Response.StatusCode}");
                            logger.LogInformation($"Trying to refresh users ID and retry getting stats...");
                            await GetSummonerID(user);
                            await GetStats(addToLog, true);
                        }
                        else
                        {
                            user.Watch = false;
                            logger.LogError("retry getting stats failed - settings 'Watch' flag to false");
                            dal.Update(user);
                        }
                    }
                    else
                        logger.LogError($"An unexpected error occured during request: {e.Message}");

                    continue;
                }


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

                    if (addToLog)
                    {
                        var pointsLog = new PointsLogModel
                        {
                            ChampionLevel = p.ChampionLevel,
                            ChampionId = p.ChampionId,
                            ChampionPoints = p.ChampionPoints,
                            User = user,
                        };

                        dal.Add(pointsLog);
                    }
                });

                await dal.CommitChangesAsync();
                logger.LogInformation("Changed commited to database");
            }
        }
    }
}

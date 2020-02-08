using CommandLine;
using Crawler;
using DatabaseAccessLayer;
using Microsoft.Extensions.Logging;
using RiotAPIAccessLayer;
using System.Linq;
using System.Threading.Tasks;

namespace CLI.Modules
{
    [Verb("points", HelpText = "Manage mastery points")]
    class PointsModuleOptions
    {
        [Option('I', "info", Group = "action", HelpText = "show points info")]
        public bool ActionInfo { get; set; }

        [Option('F', "fetch", Group = "action", HelpText = "fetch data from Riot API and save to DB")]
        public bool ActionFetch { get; set; }

        [Option('s', "server", Required = false, HelpText = "server")]
        public string Server { get; set; }

        [Option('n', "name", Required = false, HelpText = "summoner name")]
        public string SummonerName { get; set; }
    }

    class PointsModule
    {
        private readonly RiotAPIWrapper riotApi;
        private readonly DatabaseAccess dal;
        private readonly PointsCrawler crawler;
        private readonly ILogger<SummonerModule> logger;

        public PointsModule(RiotAPIWrapper riotApi, DatabaseAccess dal, PointsCrawler crawler, ILogger<SummonerModule> logger)
        {
            this.riotApi = riotApi;
            this.dal = dal;
            this.crawler = crawler;
            this.logger = logger;
        }

        public int Exec(PointsModuleOptions opts)
        {
            if (opts.ActionInfo)
                return Info(opts).Result;
            if (opts.ActionFetch)
                return Fetch(opts).Result;


            logger.LogError("Action must be specified");
            return 1;
        }

        #region actions

        private async Task<int> Info(PointsModuleOptions opts)
        {
            if (opts.Server != null && opts.SummonerName != null)
            {
                var summoner = await Shared.GetSummoner(opts.Server, opts.SummonerName, riotApi, logger);
                if (summoner == null)
                    return 1;

                var summonerDb = await Shared.GetDbSummoner(summoner.Id, dal);

                var entries = await dal.GetPointsViewAsync(summonerDb?.Id ?? default);
                var count = entries?.Count ?? 0;
                var lastEntry = entries?.ToArray()[0];

                logger.LogInformation(
                    "\n" +
                    $"For summoner '{summoner.Name}' ({summonerDb.Id}):\n\n" +
                    $"Data count:   {count}\n" +
                    $"Last entry:   {(lastEntry == null ? "[nothing collected yet]" : lastEntry.Updated.ToString())}"
                );
            } 
            else
            {
                var count = dal.GetPointsCount((_) => true);
                var lastEntry = (await dal.GetPointsViewAsync(default, null, 1, 0)).ToArray();

                logger.LogInformation(
                    "\n" +
                    $"Data count:   {count}\n" +
                    $"Last entry:   {(lastEntry.Length < 1 ? "[nothing collected yet]" : lastEntry[0].Updated.ToString())}"
                );
            }

            return 0;
        }

        private async Task<int> Fetch(PointsModuleOptions opts)
        {
            if (opts.Server != null && opts.SummonerName != null)
            {
                var summoner = await Shared.GetSummoner(opts.Server, opts.SummonerName, riotApi, logger);
                if (summoner == null)
                    return 1;

                var summonerDb = await Shared.GetDbSummoner(summoner.Id, dal);

                if (summonerDb == null)
                {
                    logger.LogError("This summoner is not registered in the database");
                    return 1;
                }

                await crawler.GetUserStats(summonerDb, addToLog: false);
            }
            else
            {
                await crawler.GetStats(addToLog: false);
            }

            return 0;
        }

        #endregion
    }
}

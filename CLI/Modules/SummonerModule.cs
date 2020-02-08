using CommandLine;
using DatabaseAccessLayer;
using Microsoft.Extensions.Logging;
using RiotAPIAccessLayer;
using RiotAPIAccessLayer.Models;
using System;
using System.Threading.Tasks;

namespace CLI.Modules
{
    [Verb("summoner", HelpText = "Manage summoners")]
    public class SummonerModuleOptions
    {
        //[Value(2, Required = true, HelpText = "Action [info, watch, unwatch, prune]")]
        //public string Action { get; set; }

        //[Value(3, Required = true, HelpText = "server")]
        //public string Server { get; set; }

        //[Value(4, Required = true, HelpText = "summoner name")]
        //public string SummonerName { get; set; }

        [Option('I', "info", Group = "action", HelpText = "show summoenr info")]
        public bool ActionInfo { get; set; }

        [Option('W', "watch", Group = "action", HelpText = "set or register a user as watching")]
        public bool ActionWatch { get; set; }

        [Option('U', "unwatch", Group = "action", HelpText = "set a user as unwatch")]
        public bool ActionUnwatch { get; set; }

        [Option('P', "prune", Group = "action", HelpText = "prune all data of a user")]
        public bool ActionPrune { get; set; }

        [Option('s', "server", Required = true, Group = "summoner specification", HelpText = "server")]
        public string Server { get; set; }

        [Option('n', "name", Required = true, Group = "summoner specification", HelpText = "summoner name")]
        public string SummonerName { get; set; }

        [Option("yeah-delete-all--collected-data-from-this-user", Required = false, Hidden = true)]
        public bool SurelyPrune { get; set; }
    }

    class SummonerModule
    {
        private readonly RiotAPIWrapper riotApi;
        private readonly DatabaseAccess dal;
        private readonly ILogger<SummonerModule> logger;

        public SummonerModule(RiotAPIWrapper riotApi, DatabaseAccess dal, ILogger<SummonerModule> logger)
        {
            this.riotApi = riotApi;
            this.dal = dal;
            this.logger = logger;
        }

        public int Exec(SummonerModuleOptions opts)
        {
            var summoner = GetSummoner(opts).Result;
            if (summoner == null)
                return 1;

            if (opts.ActionInfo)
                return Info(summoner);
            if (opts.ActionWatch)
                return Watch(summoner, opts.Server);
            if (opts.ActionUnwatch)
                return Unwatch(summoner);
            if (opts.ActionPrune)
                return Prune(summoner, opts.SurelyPrune);
            
            logger.LogError("Action must be specified");
            return 1;
        }

        #region actions

        private int Info(UserModel summoner)
        {
            var summonerDb = GetDbSummoner(summoner.Id).Result;

            var info = "\n" +
                    $"Summoner Id:     {summoner.Id}\n" +
                    $"Account Id:      {summoner.AccountId}\n" +
                    $"Name:            {summoner.Name}\n" +
                    $"Level:           {summoner.SummonerLevel}\n" +
                    $"Profile Icon Id: {summoner.ProfileIconId}\n\n" +
                    
                    $"Status:          {(summonerDb == null ? "unregistered" : (summonerDb.Watch ? "watching" : "not watching"))}";

            if (summonerDb != null)
            {
                info += "\n" +
                    $"Registered:      {summonerDb.Created.ToString()}\n" +
                    $"Database Id:     {summonerDb.Id}";
            }

            logger.LogInformation(info);

            return 0;
        }

        private int Watch(UserModel summoner, string server)
        {
            var summonerDb = GetDbSummoner(summoner.Id).Result;

            if (summonerDb != null)
            {
                if (summonerDb.Watch)
                {
                    logger.LogInformation("Summoner is already set to watching. Skipping action.");
                    return 0;
                }

                summonerDb.Watch = true;
                dal.Update(summonerDb);
            } 
            else
            {
                summonerDb = new DatabaseAccessLayer.Models.UserModel
                {
                    Server = server.ToLower(),
                    SummonerId = summoner.Id,
                    Username = summoner.Name,
                    Watch = true,
                };
                dal.Add(summonerDb);
            }

            CommitDatabaseChanges().Wait();

            return 0;
        }

        private int Unwatch(UserModel summoner)
        {
            var summonerDb = GetDbSummoner(summoner.Id).Result;

            if (summonerDb == null)
            {
                logger.LogError("Summoner is not registered. Skipping action.");
                return 1;
            }

            if (!summonerDb.Watch)
            {
                logger.LogInformation("Summoner is already set to not watching. Skipping action.");
                return 0;
            }

            summonerDb.Watch = false;
            dal.Update(summonerDb);

            CommitDatabaseChanges().Wait();

            return 0;
        }

        private int Prune(UserModel summoner, bool surely)
        {
            if (!surely)
            {
                logger.LogWarning(
                    "\n"+ 
                    "!!! ATTENTION !!!\n" +
                    "THIS ACTION WILL DELETE ALL COLLECTED DATA CORRESPONDING TO " +
                    "THE SPECIFIED USER AND THIS CAN NOT BE UNDONE!\n\n" +
                    "Re-execute this command with the flag '--yeah-delete-all--collected-data-from-this-user' " +
                    "if you are sure to perform this command."
                );
                return 0;
            }

            var summonerDb = GetDbSummoner(summoner.Id).Result;

            if (summonerDb == null)
            {
                logger.LogError("Summoner is not registered. Skipping action.");
                return 1;
            }

            dal.DeletePointsLog(summonerDb.Id);
            dal.DeletePoints(summonerDb.Id);
            dal.DeleteUser(summonerDb);

            CommitDatabaseChanges().Wait();

            return 0;
        }

        #endregion

        #region helpers

        private async Task<UserModel> GetSummoner(SummonerModuleOptions opts)
        {
            try
            {
                return await riotApi.GetSummonerByName(opts.Server, opts.SummonerName);
            }
            catch (Exception e)
            {
                logger.LogError($"Failed getting summoner: {e}");
            }

            return null;
        }

        private async Task<DatabaseAccessLayer.Models.UserModel> GetDbSummoner(string id) =>
            await dal.GetUserBySummonerIdAsync(id);

        private async Task CommitDatabaseChanges()
        {
            logger.LogInformation("Committing changes to database...");
            await dal.CommitChangesAsync();
        }

        #endregion
    }
}

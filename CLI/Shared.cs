using DatabaseAccessLayer;
using Microsoft.Extensions.Logging;
using RiotAPIAccessLayer;
using RiotAPIAccessLayer.Models;
using System;
using System.Threading.Tasks;

namespace CLI
{
    public static class Shared
    {
        public static async Task<UserModel> GetSummoner(string server, string name, RiotAPIWrapper riotApi, ILogger logger)
        {
            try
            {
                return await riotApi.GetSummonerByName(server, name);
            }
            catch (Exception e)
            {
                logger.LogError($"Failed getting summoner: {e}");
            }

            return null;
        }

        public static async Task<DatabaseAccessLayer.Models.UserModel> GetDbSummoner(string id, DatabaseAccess dal) =>
            await dal.GetUserBySummonerIdAsync(id);

        public static async Task CommitDatabaseChanges(DatabaseAccess dal, ILogger logger)
        {
            logger.LogInformation("Committing changes to database...");
            await dal.CommitChangesAsync();
        }
    }
}

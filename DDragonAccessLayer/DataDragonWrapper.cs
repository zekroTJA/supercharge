using DDragonAccessLayer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDragonAccessLayer
{
    public class DataDragonWrapper
    {
        private readonly Requests requests;

        public List<ChampionModel> Champions { get; private set; }
        public string Version { get; private set; }

        public DataDragonWrapper(IConfiguration config, ILogger<DataDragonWrapper>? logger)
        {
            requests = new Requests();

            var version = config.GetValue<string>("DDragon:Version");

            if (version != null)
                Version = version;
            else
                FetchVersionAsync().Wait();

            logger.LogInformation($"Using version {Version}");

            FetchChampionsAsync().Wait();

            logger.LogInformation($"Fetched information of {Champions.Count()} champions");
        }

        public string GetChampionNameById(int id)
        {
            var idStr = id.ToString();
            return Champions.FirstOrDefault(c => c.Key.Equals(idStr))?.Id;
        }

        public int GetChampionByName(string championName)
        {
            if (championName == null)
                return -1;

            var champ = Champions.FirstOrDefault(c => c.Id.ToLower().Equals(championName.ToLower()));
            return champ != null ? int.Parse(champ.Key) : -1;
        }

        private async Task FetchVersionAsync()
        {
            var versionData = await requests.Get<string[]>("api/versions.json");
            Version = versionData[0];
        }

        private async Task FetchChampionsAsync()
        {
            var championData = await requests.Get<ChampionResponseModel>($"cdn/{Version}/data/en_US/champion.json");
            Champions = championData.Data.Values.ToList();
        }
    }
}

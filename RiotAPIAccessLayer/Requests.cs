using RiotAPIAccessLayer.Exceptions;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RiotAPIAccessLayer
{
    class Requests
    {
        private const string ROOT_URI = "https://{0}.api.riotgames.com";

        private readonly HttpClient client = new HttpClient();

        public Requests(string apiToken)
        {
            client.DefaultRequestHeaders.Add("X-Riot-Token", apiToken);
            client.DefaultRequestHeaders.Add("User-Agent", "MasteryPointsStats Crawler Service");
        }

        public async Task<T> Get<T>(
            string server, 
            string api, 
            string apiVersion, 
            string path)
        {
            var res = await client.GetAsync($"{string.Format(ROOT_URI, server)}/{api}/{apiVersion}/{path}");

            if (!res.IsSuccessStatusCode)
            {
                throw new ResponseException(res);
            }

            var bodyStream = await res.Content.ReadAsStreamAsync();
            var data = await JsonSerializer.DeserializeAsync<T>(bodyStream);

            return data;
        }
    }
}

using RiotAPIAccessLayer.Exceptions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace DDragonAccessLayer
{
    class Requests
    {
        private const string ROOT_URI = "https://ddragon.leagueoflegends.com";

        private readonly HttpClient client = new HttpClient();

        public Requests()
        {
            client.DefaultRequestHeaders.Add("User-Agent", "MasteryPointsStats Crawler Service");
        }

        public async Task<T> Get<T>(
            string path)
        {
            var res = await client.GetAsync($"{ROOT_URI}/{path}");

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

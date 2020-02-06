using Shared.Exceptions;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RiotAPIAccessLayer
{
    class Requests
    {
        private const string ROOT_URI = "https://{0}.api.riotgames.com";
        private const int haltBufferSecs = 1;

        private readonly HttpClient client = new HttpClient();

        private DateTime haltUntil;

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
            var now = DateTime.Now;
            if (haltUntil > DateTime.Now)
            {
                await Task.Delay(haltUntil - now);
                return await Get<T>(server, api, apiVersion, path);
            }

            var res = await client.GetAsync($"{string.Format(ROOT_URI, server)}/{api}/{apiVersion}/{path}");

            if (res.StatusCode == HttpStatusCode.TooManyRequests)
            {
                var waitSecs = int.Parse(res.Headers.GetValues("Retry-After").FirstOrDefault());
                var wait = TimeSpan.FromSeconds(waitSecs + haltBufferSecs);
                haltUntil = DateTime.Now.Add(wait);

                await Task.Delay(wait);
                return await Get<T>(server, api, apiVersion, path);
            }

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

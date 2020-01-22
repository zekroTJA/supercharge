using System.Text.Json.Serialization;

namespace RiotAPIAccessLayer.Models
{
    public class UserModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("summonerLevel")]
        public int SummonerLevel { get; set; }

        [JsonPropertyName("accountId")]
        public string AccountId { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("profileIconId")]
        public int ProfileIconId { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace RiotAPIAccessLayer.Models
{
    public class PointsModel
    {
        [JsonPropertyName("championLevel")]
        public int ChampionLevel { get; set; }

        [JsonPropertyName("championPoints")]
        public int ChampionPoints { get; set; }

        [JsonPropertyName("championId")]
        public int ChampionId { get; set; }

        [JsonPropertyName("lastPlayTime")]
        public long LastPlayed { get; set; }
    }
}

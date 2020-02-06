using System.Text.Json.Serialization;

namespace RestAPI.Models
{
    public class CountsModel
    {
        [JsonPropertyName("users")]
        public string Users { get; set; }

        [JsonPropertyName("usersWatching")]
        public string UsersWatching { get; set; }

        [JsonPropertyName("points")]
        public string Points { get; set; }

        [JsonPropertyName("pointsLog")]
        public string PointsLog { get; set; }
    }

    public class VersionsModel
    {
        [JsonPropertyName("restApi")]
        public string RestAPI { get; set; }

        [JsonPropertyName("databaseAccessLayer")]
        public string DatabaseAccessLayer { get; set; }

        [JsonPropertyName("ddragonAccessLayer")]
        public string DDragonAccessLayer { get; set; }

        [JsonPropertyName("riotApiAccessLayer")]
        public string RiotAPIAccessLayer { get; set; }

        [JsonPropertyName("shared")]
        public string Shared { get; set; }
    }

    public class StatusModel
    {
        [JsonPropertyName("counts")]
        public CountsModel Counts { get; set; }

        [JsonPropertyName("versions")]
        public VersionsModel Versions { get; set; }
    }
}

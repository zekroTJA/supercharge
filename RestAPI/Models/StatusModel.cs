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

    public class StatusModel
    {
        [JsonPropertyName("counts")]
        public CountsModel Counts { get; set; }
    }
}

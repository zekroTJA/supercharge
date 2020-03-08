using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DatabaseAccessLayer.Models
{
    public class PointsLogViewModel
    {
        [JsonPropertyName("championId")]
        public int ChampionId { get; set; }

        [JsonPropertyName("championLevel")]
        public int ChampionLevel { get; set; }

        [JsonPropertyName("championPoints")]
        public int ChampionPoints { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("predicted")]
        [NotMapped]
        public bool Predicted { get; set; }


        public PointsLogViewModel()
        {
        }

        public PointsLogViewModel(PointsLogModel model)
        {
            ChampionId = model.ChampionId;
            ChampionLevel = model.ChampionLevel;
            ChampionPoints = model.ChampionPoints;
            Timestamp = model.Timestamp;
        }
    }
}

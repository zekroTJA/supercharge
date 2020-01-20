using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DatabaseAccessLayer.Models
{
    public class PointsViewModel
    {
        [JsonPropertyName("championId")]
        public int ChampionId { get; set; }

        [JsonPropertyName("championLevel")]
        public int ChampionLevel { get; set; }

        [JsonPropertyName("championPoints")]
        public int ChampionPoints { get; set; }

        [JsonPropertyName("lastPlayed")]
        public DateTime LastPlayed { get; set; }

        [JsonPropertyName("updated")]
        public DateTime Updated { get; set; }


        public PointsViewModel()
        {
        }

        public PointsViewModel(PointsModel model)
        {
            ChampionId = model.ChampionId;
            ChampionLevel = model.ChampionLevel;
            ChampionPoints = model.ChampionPoints;
            LastPlayed = model.LastPlayed;
            Updated = model.Updated;
        }
    }
}

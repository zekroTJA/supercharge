using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DDragonAccessLayer.Models
{
    public class ChampionResponseModel
    {
        [JsonPropertyName("data")]
        public Dictionary<string, ChampionModel> Data { get; set; }
    }
}

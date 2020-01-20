using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DDragonAccessLayer.Models
{
    public class ChampionModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}

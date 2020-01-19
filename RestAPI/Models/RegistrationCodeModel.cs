using System;
using System.Text.Json.Serialization;

namespace RestAPI.Models
{
    public class RegistrationCodeModel
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("expires")]
        public DateTime Expires { get; set; }
    }
}

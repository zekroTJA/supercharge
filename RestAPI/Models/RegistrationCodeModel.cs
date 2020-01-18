using System.Text.Json.Serialization;

namespace RestAPI.Models
{
    public class RegistrationCodeModel
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
    }
}

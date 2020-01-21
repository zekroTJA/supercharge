using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DatabaseAccessLayer.Models
{
    public class UserModel
    {
        [Key]
        [JsonIgnore]
        public Guid Id { get; set; }

        [StringLength(100)]
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("summonerId")]
        public string SummonerId { get; set; }

        [StringLength(10)]
        [JsonPropertyName("server")]
        public string Server { get; set; }

        [JsonPropertyName("watch")]
        public bool Watch { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        public UserModel()
        {
            Id = Guid.NewGuid();
            Created = DateTime.Now;
        }
    }
}

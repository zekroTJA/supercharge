using RiotAPIAccessLayer.Models;
using System;
using System.Text.Json.Serialization;

namespace RestAPI.Models
{
    public class UserViewModel : UserModel
    {
        [JsonPropertyName("registered")]
        public bool Registered { get; set; }

        [JsonPropertyName("watch")]
        public bool? Watch { get; set; }

        [JsonPropertyName("created")]
        public DateTime? Created { get; set; }

        public UserViewModel()
        {
        }

        public UserViewModel(UserModel model)
        {
            Name = model.Name;
            SummonerLevel = model.SummonerLevel;
            AccountId = model.AccountId;
            Id = model.Id;
            ProfileIconId = model.ProfileIconId;
        }
    }
}

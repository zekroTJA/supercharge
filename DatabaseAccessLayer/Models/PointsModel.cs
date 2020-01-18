using System;
using System.ComponentModel.DataAnnotations;

namespace DatabaseAccessLayer.Models
{
    public class PointsModel
    {
        [Key]
        public Guid Id { get; set; }

        public UserModel User { get; set; }

        public int ChampionId { get; set; }

        public int ChampionLevel { get; set; }

        public int ChampionPoints { get; set; }

        public DateTime LastPlayed { get; set; }

        public DateTime Updated { get; set; }


        public PointsModel()
        {
            Id = Guid.NewGuid();
            Updated = DateTime.Now;
        }
    }
}

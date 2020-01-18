using System;
using System.ComponentModel.DataAnnotations;

namespace DatabaseAccessLayer.Models
{
    public class PointsLogModel
    {
        [Key]
        public Guid Id { get; set; }

        public UserModel User { get; set; }

        public int ChampionId { get; set; }

        public int ChampionLevel { get; set; }

        public int ChampionPoints { get; set; }

        public DateTime Timestamp { get; set; }


        public PointsLogModel()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.Now;
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace DatabaseAccessLayer.Models
{
    public class UserModel
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(100)]
        public string Username { get; set; }

        public string SummonerID { get; set; }

        [StringLength(10)]
        public string Server { get; set; }

        public bool Watch { get; set; }

        public DateTime Created { get; set; }

        public UserModel()
        {
            Id = Guid.NewGuid();
            Created = DateTime.Now;
        }
    }
}

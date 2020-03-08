using DatabaseAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestAPI.Modules
{
    public class HistoryPrediction
    {
        private readonly List<PointsLogViewModel> seedData;

        public HistoryPrediction(ICollection<PointsLogViewModel> seedData)
        {
            this.seedData = seedData
                .OrderBy(d => d.Timestamp)
                .ToList();
        }
        
        public ICollection<PointsLogViewModel> Predict(TimeSpan duration)
        {
            var champs = new HashSet<int>();
            seedData.ForEach(d => champs.Add(d.ChampionId));

            champs.ToList().ForEach(c => PredictChamp(c, duration));

            return seedData;
        }

        private void PredictChamp(int champId, TimeSpan duration)
        {
            var p1 = seedData.First(d => d.ChampionId == champId);
            var p2 = seedData.Last(d => d.ChampionId == champId);

            if (p1 == null || p2 == null)
                return;

            var ascent = (p2.ChampionPoints - p1.ChampionPoints) /
                         (p2.Timestamp - p1.Timestamp).Days;

            var date = p2.Timestamp;
            var until = date.Add(duration).AddDays(2);
            var points = p2.ChampionPoints;

            while (until >= date)
            {
                seedData.Add(new PointsLogViewModel
                {
                    ChampionId = champId,
                    Predicted = true,
                    ChampionLevel = 0,
                    
                    ChampionPoints = points,
                    Timestamp = date,
                });

                date = date.AddDays(1);
                points += ascent;
            }
        }
    }
}

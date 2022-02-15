using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMCompetitionTeamResults
    {
        public long Position { get; set; }
        public int CompetitionTeamId { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public int TotalCQPoints { get; set; }
        public string Note { get; set; }
        public int Points { get; set; }
        public double TeamIndex { get; set; }
        public string Color { get; set; }
        public int PreviousRank { get; set; }

        public long RankingTrend //ranking gained or lost since last bikerace
        {
            get
            {
                if (Position == 0)
                {
                    return 0;
                }
                return PreviousRank - Position;
            }
        }
    }
}
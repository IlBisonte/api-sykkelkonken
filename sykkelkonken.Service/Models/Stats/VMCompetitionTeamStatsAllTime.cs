using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMCompetitionTeamStatsAllTime
    {
        public string Name { get; set; }

        public int Points { get; set; }

        public double TeamIndex { get; set; }

        public int NoOfParticipations { get; set; }

        public int Victories { get; set; }

        public string TeamIndexTwoDecimals
        {
            get
            {
                if (TeamIndex > 0)
                {
                    return string.Format("{0}", TeamIndex.ToString("0.###"));
                }
                return "";
            }
        }
    }
}
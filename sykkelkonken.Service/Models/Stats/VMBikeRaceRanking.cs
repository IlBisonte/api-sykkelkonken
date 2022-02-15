using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMBikeRaceRanking
    {
        public int BikeRaceDetailId { get; set; }
        public string BikeRaceName { get; set; }
        public int Year { get; set; }
        public int MinPoints
        {
            get
            {
                if (CompetitionTeams != null)
                {
                    return CompetitionTeams.Min(ct => ct.Points);
                }
                return 0;
            }
        }
        public int MaxPoints
        {
            get
            {
                if (CompetitionTeams != null)
                {
                    return CompetitionTeams.Max(ct => ct.Points);
                }
                return 0;
            }
        }

        public IList<BRCompetitionTeam> CompetitionTeams { get; set; }

        public VMBikeRaceRanking(int bikeRaceDetailId, string bikeRaceName, int year)
        {
            this.BikeRaceDetailId = bikeRaceDetailId;
            this.BikeRaceName = bikeRaceName;
            this.Year = year;
            this.CompetitionTeams = new List<BRCompetitionTeam>();
        }
    }

    public class BRCompetitionTeam
    {
        public int CompetitionTeamId { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public int Ranking { get; set; }
    }
}
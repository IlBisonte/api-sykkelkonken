using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMCompetitionTeamPointsPerBikeRace
    {
        public int CompetitionTeamId { get; set; }
        public string CompetitionTeamName { get; set; }
        public int BikeRaceDetailId { get; set; }
        public int BikeRaceId { get; set; }
        public string BikeRaceName { get; set; }
        public DateTime StartDate { get; set; }
        public int Year { get; set; }
        public int TotalPoints { get; set; }
    }
}
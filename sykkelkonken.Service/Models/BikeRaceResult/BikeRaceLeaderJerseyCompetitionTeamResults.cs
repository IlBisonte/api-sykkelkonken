using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class BikeRaceLeaderJerseyCompetitionTeamResults
    {
        public int CompetitionTeamId { get; set; }
        public string CompetitionTeamName { get; set; }
        public int BikeRaceId { get; set; }
        public string BikeRaceName { get; set; }
        public int? LeaderJerseyPoints { get; set; }
        public string LeaderJerseyName { get; set; }
        public string BikeRiders { get; set; }
    }
}
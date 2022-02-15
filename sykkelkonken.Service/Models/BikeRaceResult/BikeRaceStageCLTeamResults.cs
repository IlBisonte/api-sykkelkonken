using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class BikeRaceStageCLTeamResults
    {
        public int ChampionsLeagueTeamId { get; set; }
        public string CompetitionTeamName { get; set; }
        public int BikeRaceId { get; set; }
        public string BikeRaceName { get; set; }
        public int StagePoints { get; set; }
        public int StageNo { get; set; }
        public string BikeRiders { get; set; }
    }
}
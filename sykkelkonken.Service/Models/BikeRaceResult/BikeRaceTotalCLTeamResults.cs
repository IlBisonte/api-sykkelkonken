using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class BikeRaceTotalCLTeamResults
    {
        public long Position { get; set; }
        public int ChampionsLeagueTeamId { get; set; }
        public string CompetitionTeamName { get; set; }
        public int? TotalPoints { get; set; }
        public string BikeRiders { get; set; }
    }
}
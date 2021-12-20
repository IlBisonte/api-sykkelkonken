using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMChampionsLeagueTeamBikeRider
    {
        public int ChampionsLeagueTeamId { get; set; }
        public string TeamName { get; set; }
        public int BikeRiderId { get; set; }
        public string BikeRiderName { get; set; }
        public int CQPoints { get; set; }
        public string BikeRiderTeamCode { get; set; }
        public string BikeRiderTeamName { get; set; }
        public int Year { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class BikeRaceGCCLTeamResults
    {
        public long Position { get; set; }
        public int ChampionsLeagueTeamId { get; set; }
        public string CompetitionTeamName { get; set; }
        public int? GCPoints { get; set; }
        public string BikeRiders { get; set; }
    }
}
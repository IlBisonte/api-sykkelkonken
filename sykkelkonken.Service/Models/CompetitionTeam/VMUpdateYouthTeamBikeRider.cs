using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models.CompetitionTeam
{
    public class VMUpdateYouthTeamBikeRider
    {
        public int YouthTeamId { get; set; }
        public int OrigBikeRiderId { get; set; }
        public int NewBikeRiderId { get; set; }
        public int Year { get; set; }
    }
}
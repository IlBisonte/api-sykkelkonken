using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMCompetitionTeamPointsPerStage
    {
        public int CompetitionTeamId { get; set; }
        public int BikeRaceDetailId { get; set; }
        public int StageNo { get; set; }
        public int StagePoints { get; set; }
    }
}
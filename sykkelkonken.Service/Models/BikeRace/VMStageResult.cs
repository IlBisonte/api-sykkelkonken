using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMStageResult
    {
        public int BikeRaceDetailId { get; set; }
        public int BikeRaceId { get; set; }
        public int StageNo { get; set; }
        public int BikeRiderId { get; set; }
        public string BikeRiderName { get; set; }
        public int StagePosition { get; set; }
        public int Points { get; set; }
    }
}
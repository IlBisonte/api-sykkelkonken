using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMLeaderJerseyResult
    {
        public int BikeRaceDetailId { get; set; }
        public int BikeRaceId { get; set; }
        public int BikeRaceCategoryId { get; set; }
        public int BikeRiderId { get; set; }
        public string BikeRiderName { get; set; }
        public int LeaderJerseyId { get; set; }
        public string LeaderJerseyName { get; set; }
        public int LeaderJerseyPosition { get; set; }
        public int Points { get; set; }
    }
}
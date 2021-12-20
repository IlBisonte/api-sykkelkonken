using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMBikeRaceResult
    {
        public int BikeRaceDetailId { get; set; }
        public int BikeRaceId { get; set; }
        public int BikeRiderId { get; set; }
        public string BikeRiderName { get; set; }
        public int Position { get; set; }
        public int Points { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMBikeRaceSeasonPlacement
    {
        public int BikeRaceSeasonPlacementId { get; set; }
        public int Year { get; set; }
        public int BikeRaceDetailId { get; set; }
        public string Name { get; set; }

        public VMBikeRaceSeasonPlacement()
        {

        }
    }
}
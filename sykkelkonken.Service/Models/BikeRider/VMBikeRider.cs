using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMBikeRider
    {
        public int BikeRiderDetailId { get; set; }

        public int BikeRiderId { get; set; }

        public string BikeRiderName { get; set; }

        //public int BikeTeamId { get; set; }

        public string BikeTeamCode { get; set; }

        public string BikeTeamName { get; set; }

        public string Nationality { get; set; }

        public int CQPoints { get; set; }

        public int Year { get; set; }
    }
}
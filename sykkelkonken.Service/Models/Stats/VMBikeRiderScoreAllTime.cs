using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMBikeRiderScoreAllTime
    {
        public int BikeRiderId { get; set; }

        public string BikeRiderName { get; set; }

        public int Points { get; set; }

        public double RiderIndex { get; set; }

        public string RiderIndexTwoDecimals
        {
            get
            {
                if (RiderIndex > 0)
                {
                    return string.Format("{0}", RiderIndex.ToString("0.###"));
                }
                return "";
            }
        }
    }
}
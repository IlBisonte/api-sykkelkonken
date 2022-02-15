using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMBikeRiderStats
    {
        public int BikeRiderId { get; set; }
        public int BikeRiderDetailId { get; set; }
        public string BikeRiderName { get; set; }
        public string BikeTeamCode { get; set; }
        public int? Points { get; set; }
        public int NoOfSelections { get; set; }
        public double SelectedBy { get; set; }
        public int CQPoints { get; set; }
        public double RiderIndex { get; set; }
        public string CLTeamName { get; set; }
        public string Color { get; set; }

        public string SelectedByPercent
        {
            get
            {
                if (SelectedBy > 0)
                {
                    return string.Format("{0}%", SelectedBy.ToString("0.##"));
                }
                return "";
            }
        }

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
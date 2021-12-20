using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMBikeRiderResults
    {
        public int BikeRiderDetailId { get; set; }
        public int BikeRiderId { get; set; }
        public string BikeRiderName { get; set; }
        public int BikeRaceId { get; set; }
        public string BikeRaceName { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public int? GCPosition { get; set; }
        public int? GCPoints { get; set; }
        public int? StagePosition { get; set; }
        public int? StagePoints { get; set; }
        public int? StageNo { get; set; }
        public int? LeaderJerseyPosition { get; set; }
        public int? LeaderJerseyPoints { get; set; }
        public string LeaderJerseyName { get; set; }
        public int? Position { get; set; }
        public int? Points { get; set; }

        public string Date_ToString
        {
            get
            {
                if (StartDate != FinishDate)
                {
                    return string.Format("{0} - {1}", StartDate.ToString("dd.MM.yyyy"), FinishDate.ToString("dd.MM.yyyy"));
                }
                else
                {
                    return StartDate != null ? StartDate.ToString("dd.MM.yyyy") : "";
                }
            }
        }
    }
}
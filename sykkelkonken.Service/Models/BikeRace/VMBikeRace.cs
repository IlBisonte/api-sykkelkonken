using sykkelkonken.Data;
using sykkelkonken.Service.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMBikeRaceDetail
    {
        public int BikeRaceDetailId { get; set; }

        public int BikeRaceId { get; set; }

        public string Name { get; set; }

        public int Year { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }

        public string CountryName { get; set; }

        public int BikeRaceCategoryId { get; set; }

        public int NoOfStages { get; set; }

        public bool HasTTT { get; set; }

        public bool IsCalculated { get; set; }

        public string BikeRiderWinner { get; set; }

        public bool Cancelled { get; set; }

        public VMBikeRaceDetail()
        {

        }

        public VMBikeRaceDetail(sykkelkonken.Data.BikeRaceDetail bikeRace)
        {
            this.BikeRaceDetailId = bikeRace.BikeRaceDetailId;
            this.BikeRaceId = bikeRace.BikeRaceId;
            this.Name = bikeRace.Name;
            this.Year = bikeRace.Year;
            this.StartDate = bikeRace.StartDate ?? new DateTime();
            this.FinishDate = bikeRace.FinishDate ?? new DateTime();
            this.CountryName = bikeRace.CountryName;
            this.BikeRaceCategoryId = bikeRace.BikeRaceCategoryId ?? -1;
            this.NoOfStages = bikeRace.NoOfStages ?? 0;
            this.HasTTT = bikeRace.HasTTT ?? false;
            this.IsCalculated = bikeRace.IsCalculated ?? false;
            this.BikeRiderWinner = "";
            this.Cancelled = bikeRace.Cancelled ?? false;
            if (bikeRace.BikeRaceResults.Count > 0)
            {
                var winner = bikeRace.BikeRaceResults.FirstOrDefault(r => r.Position == 1);
                if (winner != null)
                {
                    this.BikeRiderWinner = winner.BikeRider.BikeRiderName;
                }
            }

            if (!this.IsCalculated)
            {
                IList<BikeRaceResult> bikeRaceResults = bikeRace.BikeRaceResults != null ? bikeRace.BikeRaceResults.ToList() : new List<BikeRaceResult>();
                IList<StageResult> stageResults = bikeRace.StageResults != null ? bikeRace.StageResults.ToList() : new List<StageResult>();
                IList<LeaderJerseyResult> leaderJerseyResults = bikeRace.LeaderJerseyResults != null ? bikeRace.LeaderJerseyResults.ToList() : new List<LeaderJerseyResult>();
                if (bikeRaceResults != null)
                {
                    switch (this.BikeRaceCategoryId)
                    {
                        case (int)BikeRaceCategory.BikeRaceCategoryIdEnum.OneDay:
                            if (bikeRaceResults.Count == 10)
                            {
                                this.IsCalculated = true;
                            }
                            break;
                        case (int)BikeRaceCategory.BikeRaceCategoryIdEnum.Monument:
                            if (bikeRaceResults.Count == 12)
                            {
                                this.IsCalculated = true;
                            }
                            break;
                        case (int)BikeRaceCategory.BikeRaceCategoryIdEnum.StageRace:
                            if (stageResults != null)
                            {
                                int noOfStagesToCalculate = this.HasTTT ? this.NoOfStages - 1 : this.NoOfStages;
                                if (bikeRaceResults.Count == 10 && stageResults.Count == noOfStagesToCalculate * 3)
                                {
                                    this.IsCalculated = true;
                                }
                            }
                            break;
                        case (int)BikeRaceCategory.BikeRaceCategoryIdEnum.GiroVuelta:
                            if (stageResults != null)
                            {
                                int noOfStagesToCalculate = this.HasTTT ? this.NoOfStages - 1 : this.NoOfStages;
                                if (bikeRaceResults.Count == 20 && stageResults.Count == noOfStagesToCalculate * 5 && leaderJerseyResults.Count == 3 * 3)
                                {
                                    this.IsCalculated = true;
                                }
                            }
                            break;
                        case (int)BikeRaceCategory.BikeRaceCategoryIdEnum.TourDeFrance:
                            if (stageResults != null)
                            {
                                int noOfStagesToCalculate = this.HasTTT ? this.NoOfStages - 1 : this.NoOfStages;
                                if (bikeRaceResults.Count == 20 && stageResults.Count == noOfStagesToCalculate * 6 && leaderJerseyResults.Count == 3 * 3)
                                {
                                    this.IsCalculated = true;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                bikeRace.IsCalculated = this.IsCalculated;
            }
        }

        public string StartDateToddMMM
        {
            get
            {
                return this.StartDate.ToString("dd MMM");
            }
        }
        public string StartDateToddMMyyyy
        {
            get
            {
                return this.StartDate.ToString("dd.MM.yyyy");
            }
        }

        public string FinishDateToddMMM
        {
            get
            {
                return this.FinishDate.ToString("dd MMM");
            }
        }

        public string FinishDateToddMMyyyy
        {
            get
            {
                return this.FinishDate.ToString("dd.MM.yyyy");
            }
        }

        public bool IsMonument
        {
            get
            {
                return BikeRaceCategoryId == (int)BikeRaceCategory.BikeRaceCategoryIdEnum.Monument;
            }
        }
    }
}
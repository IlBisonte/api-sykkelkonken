using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMCompetitionTeamPointsByBikeRace
    {
        public int CompetitionTeamId { get; set; }
        public IList<CompetitionTeamPointsByBikeRace> PointsByBikeRace { get; set; }
        public IList<DataItem> DataItems { get; set; }

        private string total = "Total";
        private string oneday = "Endagsritt";
        private string onedayOther = "Andre endagsritt";
        private string monument = "Monument";
        private string stagerace = "Etapperitt";
        private string stageraceOther = "Andre etapperitt";
        private string gt = "Grand Tour";
        private string girovuelta = "Giro og Vuelta";
        private string tdf = "TdF";

        public VMCompetitionTeamPointsByBikeRace(int competitionTeamId, List<CompetitionTeamPointsByBikeRace> competitionTeamPointsByBikeRace)
        {
            this.CompetitionTeamId = competitionTeamId;
            this.PointsByBikeRace = competitionTeamPointsByBikeRace;
            this.DataItems = new List<DataItem>();
            //totalpoints
            DataItemsOneDay();
            //DataItemsStageRace();
            foreach (var bikeRace in PointsByBikeRace)
            {
                string sCategory = "";
                switch (bikeRace.BikeRaceCategoryId)
                {
                    case (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.OneDay:
                        sCategory = onedayOther;
                        break;
                    case (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.Monument:
                        sCategory = monument;
                        break;
                    case (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.StageRace:
                        sCategory = stagerace;
                        break;
                    case (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.GiroVuelta:
                        sCategory = gt;
                        break;
                    case (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.TourDeFrance:
                        sCategory = gt;
                        break;
                    default:
                        break;
                }
                this.DataItems.Add(new DataItem()
                {
                    Category = sCategory,
                    TargetCategory = bikeRace.BikeRaceName,
                    Points = bikeRace.Points,
                });
            }

        }

        private void DataItemsOneDay()
        {
            this.DataItems.Add(new DataItem()
            {
                Category = oneday,
                TargetCategory = monument,
                Points = this.TotPointsMonument
            });
            this.DataItems.Add(new DataItem()
            {
                Category = oneday,
                TargetCategory = onedayOther,
                Points = this.TotPointsOneDayRaceNormal
            });
        }

        private void DataItemsStageRace()
        {
            this.DataItems.Add(new DataItem()
            {
                Category = stagerace,
                TargetCategory = gt,
                Points = this.TotPointsGT
            });
            this.DataItems.Add(new DataItem()
            {
                Category = stagerace,
                TargetCategory = stageraceOther,
                Points = this.TotPointsStageRaceNormal
            });
        }

        public int TotPoints
        {
            get
            {
                return this.PointsByBikeRace.Sum(p => p.Points);
            }
        }

        public int TotPointsOneDayRace { get { return TotPointsOneDayRaceNormal + TotPointsMonument; } }

        public int TotPointsOneDayRaceNormal
        {
            get
            {

                return this.PointsByBikeRace.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.OneDay)
                                                        .Sum(p => p.Points);
            }
        }

        public int TotPointsMonument
        {
            get
            {

                return this.PointsByBikeRace.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.Monument)
                                                        .Sum(p => p.Points);
            }
        }

        public int TotPointsStageRace { get { return TotPointsStageRaceNormal + TotPointsGiroVuelta + TotPointsTdf; } }

        public int TotPointsStageRaceNormal
        {
            get
            {

                return this.PointsByBikeRace.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.StageRace)
                                                        .Sum(p => p.Points);
            }
        }

        public int TotPointsStageRaceNormalGCPoints
        {
            get
            {

                return this.PointsByBikeRace.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.StageRace)
                                                        .Sum(p => p.GCPoints);
            }
        }

        public int TotPointsStageRaceNormalStagePoints
        {
            get
            {

                return this.PointsByBikeRace.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.StageRace)
                                                        .Sum(p => p.StagePoints);
            }
        }
        public int TotPointsGT { get { return TotPointsGiroVuelta + TotPointsTdf; } }


        public int TotPointsGiroVuelta
        {
            get
            {

                return this.PointsByBikeRace.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.GiroVuelta)
                                                        .Sum(p => p.Points);
            }
        }

        public int TotPointsGiroVueltaGCPoints
        {
            get
            {

                return this.PointsByBikeRace.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.GiroVuelta)
                                                        .Sum(p => p.GCPoints);
            }
        }

        public int TotPointsGiroVueltaStagePoints
        {
            get
            {

                return this.PointsByBikeRace.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.GiroVuelta)
                                                        .Sum(p => p.StagePoints);
            }
        }

        public int TotPointsGiroVueltaLeaderJerseyPoints
        {
            get
            {

                return this.PointsByBikeRace.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.GiroVuelta)
                                                        .Sum(p => p.LeaderJerseyPoints);
            }
        }

        public int TotPointsTdf
        {
            get
            {

                return this.PointsByBikeRace.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.TourDeFrance)
                                                        .Sum(p => p.Points);
            }
        }

        public int TotPointsTdfGCPoints
        {
            get
            {

                return this.PointsByBikeRace.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.TourDeFrance)
                                                        .Sum(p => p.GCPoints);
            }
        }

        public int TotPointsTdfStagePoints
        {
            get
            {

                return this.PointsByBikeRace.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.TourDeFrance)
                                                        .Sum(p => p.StagePoints);
            }
        }

        public int TotPointsTdfLeaderJerseyPoints
        {
            get
            {

                return this.PointsByBikeRace.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.TourDeFrance)
                                                        .Sum(p => p.LeaderJerseyPoints);
            }
        }
    }


    public class CompetitionTeamPointsByBikeRace
    {
        public int CompetitionTeamId { get; set; }
        public string Name { get; set; }
        public int BikeRaceCategoryId { get; set; }
        public int BikeRaceDetailId { get; set; }
        public string BikeRaceName { get; set; }
        public int Year { get; set; }

        public int Points { get; set; }
        public int GCPoints { get; set; }
        public int StagePoints { get; set; }
        public int LeaderJerseyPoints { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
//Sankey chart: 
//                                                    antall poeng vanlige endagsritt /
//                        antall poeng endagsritt  -> antall poeng monumenter        /   antall poeng sammenlagt
//  Antall poeng totalt ->                            antall poeng vanlige etapperitt -> antall poeng etapper
//                        antall poeng etapperitt -> 
//                                                   antall poeng GT -> antall poeng sammenlagt
//                                                                      antall poeng etapper
//                                                                      antall poeng ledertrøyer
    public class VMCompetitionTeamPointsByBikeRaceCategory
    {
        public int CompetitionTeamId { get; set; }
        public IList<CompetitionTeamPointsByBikeRaceCategory> PointsByCategory { get; set; }
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



        public VMCompetitionTeamPointsByBikeRaceCategory(int competitionTeamId, List<CompetitionTeamPointsByBikeRaceCategory> competitionTeamPointsByBikeRaceCategories)
        {
            this.CompetitionTeamId = competitionTeamId;
            this.PointsByCategory = competitionTeamPointsByBikeRaceCategories;
            this.DataItems = new List<DataItem>();
            //totalpoints
            DataItemsTotal();

        }

        private void DataItemsTotal()
        {
            this.DataItems.Add(new DataItem()
            {
                Category = total,
                TargetCategory = oneday,
                Points = this.TotPointsOneDayRace
            });
            this.DataItems.Add(new DataItem()
            {
                Category = total,
                TargetCategory = stagerace,
                Points = this.TotPointsStageRace
            });
            DataItemsOneDay();
            DataItemsStageRace();
        }

        private void DataItemsOneDay()
        {
            this.DataItems.Add(new DataItem()
            {
                Category = oneday,
                TargetCategory = onedayOther,
                Points = this.TotPointsOneDayRaceNormal
            });
            this.DataItems.Add(new DataItem()
            {
                Category = oneday,
                TargetCategory = monument,
                Points = this.TotPointsMonument
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
            //DataItemsStageRaceNormal();
            //DataItemsStageRaceGT();
        }

        private void DataItemsStageRaceNormal()
        {
            this.DataItems.Add(new DataItem()
            {
                Category = stageraceOther,
                TargetCategory = "StageRaceNormalGC",
                Points = this.TotPointsStageRaceNormalGCPoints
            });
            this.DataItems.Add(new DataItem()
            {
                Category = stageraceOther,
                TargetCategory = "StageRaceNormalStagePoints",
                Points = this.TotPointsStageRaceNormalStagePoints
            });
        }

        private void DataItemsStageRaceGT()
        {
            this.DataItems.Add(new DataItem()
            {
                Category = gt,
                TargetCategory = girovuelta,
                Points = this.TotPointsGiroVuelta
            });
            this.DataItems.Add(new DataItem()
            {
                Category = gt,
                TargetCategory = tdf,
                Points = this.TotPointsTdf
            });
            //DataItemsGiroVuelta();
            //DataItemsTdf();
        }

        private void DataItemsGiroVuelta()
        {
            this.DataItems.Add(new DataItem()
            {
                Category = girovuelta,
                TargetCategory = "GiroVueltaGC",
                Points = this.TotPointsGiroVueltaGCPoints
            });
            this.DataItems.Add(new DataItem()
            {
                Category = girovuelta,
                TargetCategory = "GiroVueltaLeaderJersey",
                Points = this.TotPointsGiroVueltaLeaderJerseyPoints
            });
            this.DataItems.Add(new DataItem()
            {
                Category = girovuelta,
                TargetCategory = "GiroVueltaStagePoints",
                Points = this.TotPointsGiroVueltaStagePoints
            });
        }

        private void DataItemsTdf()
        {
            this.DataItems.Add(new DataItem()
            {
                Category = tdf,
                TargetCategory = "TdfGC",
                Points = this.TotPointsTdfGCPoints
            });
            this.DataItems.Add(new DataItem()
            {
                Category = tdf,
                TargetCategory = "TdfLeaderJersey",
                Points = this.TotPointsTdfLeaderJerseyPoints
            });
            this.DataItems.Add(new DataItem()
            {
                Category = tdf,
                TargetCategory = "TdfStagePoints",
                Points = this.TotPointsTdfStagePoints
            });
        }

        public int TotPoints 
        { 
            get
            {
                return this.PointsByCategory.Sum(p => p.Points);
            }
        }

        public int TotPointsOneDayRace { get { return TotPointsOneDayRaceNormal + TotPointsMonument; } }

        public int TotPointsOneDayRaceNormal
        {
            get
            {
                
                return this.PointsByCategory.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.OneDay)
                                                        .Sum(p => p.Points);
            }
        }

        public int TotPointsMonument
        {
            get
            {

                return this.PointsByCategory.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.Monument)
                                                        .Sum(p => p.Points);
            }
        }

        public int TotPointsStageRace { get { return TotPointsStageRaceNormal + TotPointsGiroVuelta + TotPointsTdf; } }

        public int TotPointsStageRaceNormal
        {
            get
            {

                return this.PointsByCategory.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.StageRace)
                                                        .Sum(p => p.Points);
            }
        }

        public int TotPointsStageRaceNormalGCPoints
        {
            get
            {

                return this.PointsByCategory.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.StageRace)
                                                        .Sum(p => p.GCPoints);
            }
        }

        public int TotPointsStageRaceNormalStagePoints
        {
            get
            {

                return this.PointsByCategory.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.StageRace)
                                                        .Sum(p => p.StagePoints);
            }
        }
        public int TotPointsGT { get { return TotPointsGiroVuelta + TotPointsTdf; } }


        public int TotPointsGiroVuelta
        {
            get
            {

                return this.PointsByCategory.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.GiroVuelta)
                                                        .Sum(p => p.Points);
            }
        }

        public int TotPointsGiroVueltaGCPoints
        {
            get
            {

                return this.PointsByCategory.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.GiroVuelta)
                                                        .Sum(p => p.GCPoints);
            }
        }

        public int TotPointsGiroVueltaStagePoints
        {
            get
            {

                return this.PointsByCategory.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.GiroVuelta)
                                                        .Sum(p => p.StagePoints);
            }
        }

        public int TotPointsGiroVueltaLeaderJerseyPoints
        {
            get
            {

                return this.PointsByCategory.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.GiroVuelta)
                                                        .Sum(p => p.LeaderJerseyPoints);
            }
        }

        public int TotPointsTdf
        {
            get
            {

                return this.PointsByCategory.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.TourDeFrance)
                                                        .Sum(p => p.Points);
            }
        }

        public int TotPointsTdfGCPoints
        {
            get
            {

                return this.PointsByCategory.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.TourDeFrance)
                                                        .Sum(p => p.GCPoints);
            }
        }

        public int TotPointsTdfStagePoints
        {
            get
            {

                return this.PointsByCategory.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.TourDeFrance)
                                                        .Sum(p => p.StagePoints);
            }
        }

        public int TotPointsTdfLeaderJerseyPoints
        {
            get
            {

                return this.PointsByCategory.Where(c => c.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.TourDeFrance)
                                                        .Sum(p => p.LeaderJerseyPoints);
            }
        }
    }

    public class CompetitionTeamPointsByBikeRaceCategory
    {
        public int CompetitionTeamId { get; set; }
        public string Name { get; set; }
        public int BikeRaceCategoryId { get; set; }
        public int Year { get; set; }

        public int Points { get; set; }
        public int GCPoints { get; set; }
        public int StagePoints { get; set; }
        public int LeaderJerseyPoints { get; set; }
    }

    public class DataItem
    {
        public string Category { get; set; }
        public string TargetCategory { get; set; }
        public int Points { get; set; }
    }
}
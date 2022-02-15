using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMBikeRiderPointsByBikeRace
    {
        public int BikeRaceDetailId { get; set; }
        public IList<BikeRiderPointsByBikeRace> PointsByBikeRider { get; set; }
        public IList<DataItem> DataItems { get; set; }

        public VMBikeRiderPointsByBikeRace()
        {

        }

        public VMBikeRiderPointsByBikeRace(int bikeRaceDetailId, string bikeRaceName, List<BikeRiderPointsByBikeRace> bikeRiderPointsByBikeRace)
        {
            this.BikeRaceDetailId = bikeRaceDetailId;
            this.PointsByBikeRider = bikeRiderPointsByBikeRace;
            this.DataItems = new List<DataItem>();
            foreach (var bikeRider in PointsByBikeRider)
            {
                this.DataItems.Add(new DataItem()
                {
                    Category = bikeRaceName,
                    TargetCategory = bikeRider.BikeRiderName,
                    Points = bikeRider.Points,
                });
            }

        }
    }
    public class VMBikeRacePointsByBikeRider
    {
        public int BikeRiderDetailId { get; set; }
        public IList<BikeRiderPointsByBikeRace> PointsByBikeRace { get; set; }
        public IList<DataItem> DataItems { get; set; }

        public VMBikeRacePointsByBikeRider()
        {

        }

        public VMBikeRacePointsByBikeRider(int bikeRiderDetailId, string bikeRiderName, List<BikeRiderPointsByBikeRace> bikeRacePointsByBikeRider)
        {
            this.BikeRiderDetailId = bikeRiderDetailId;
            this.PointsByBikeRace = bikeRacePointsByBikeRider;
            this.DataItems = new List<DataItem>();
            foreach (var bikeRace in PointsByBikeRace)
            {
                this.DataItems.Add(new DataItem()
                {
                    Category = bikeRiderName,
                    TargetCategory = bikeRace.BikeRaceName,
                    Points = bikeRace.Points,
                });
            }

        }
    }
    public class VMBikeRiderPointsByCompetitionTeam
    {
        public int CompetitionTeamId { get; set; }
        public IList<BikeRiderPointsByBikeRace> PointsByCompetitionTeam { get; set; }
        public IList<DataItem> DataItems { get; set; }

        public VMBikeRiderPointsByCompetitionTeam()
        {

        }

        public VMBikeRiderPointsByCompetitionTeam(int competitionTeamId, string competitionTeamName, List<BikeRiderPointsByBikeRace> bikeRiderPointsByCompetitionTeam)
        {
            this.CompetitionTeamId = competitionTeamId;
            this.PointsByCompetitionTeam = bikeRiderPointsByCompetitionTeam;
            this.DataItems = new List<DataItem>();
            foreach (var bikeRider in PointsByCompetitionTeam.GroupBy(ct => ct.BikeRiderName))
            {
                this.DataItems.Add(new DataItem()
                {
                    Category = competitionTeamName,
                    TargetCategory = bikeRider.Key,
                    Points = bikeRider.Sum(br => br.Points),
                });
            }
            this.DataItems = this.DataItems.OrderByDescending(d => d.Points).ToList();

        }
    }

    public class BikeRiderPointsByBikeRace
    {
        public int CompetitionTeamId { get; set; }
        public string Name { get; set; }
        public int BikeRiderDetailId { get; set; }
        public string BikeRiderName { get; set; }
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
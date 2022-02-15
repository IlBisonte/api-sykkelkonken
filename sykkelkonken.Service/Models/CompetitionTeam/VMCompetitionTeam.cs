using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMCompetitionTeam
    {
        public int CompetitionTeamId { get; set; }
        public string TeamName { get; set; }
        public int TotalCQPoints { get; set; }
        public string Note { get; set; }

        public IList<VMBikeRider> BikeRiders { get; set; }

        public VMCompetitionTeam()
        {
            this.BikeRiders = new List<VMBikeRider>();
        }

        public VMCompetitionTeam(sykkelkonken.Data.CompetitionTeam competitionTeam)
        {
            this.CompetitionTeamId = competitionTeam.CompetitionTeamId;
            this.TeamName = competitionTeam.Name;
            this.TotalCQPoints = competitionTeam.TotalCQPoints;
            this.Note = competitionTeam.Note;
            this.BikeRiders = competitionTeam.CompetitionTeamBikeRiders.Select(ctbr => new VMBikeRider()
            {
                BikeRiderId = ctbr.BikeRiderId,
                BikeRiderName = ctbr.BikeRider.BikeRiderName,
                CQPoints = ctbr.BikeRider.CQPoints ?? 0,
                BikeTeamCode = ctbr.BikeRider.BikeTeamCode,
                Nationality = ctbr.BikeRider.Nationality,
            }).ToList();
        }

        public VMCompetitionTeam(sykkelkonken.Data.ChampionsLeagueTeam championsLeagueTeam)
        {

            this.CompetitionTeamId = championsLeagueTeam.ChampionsLeagueTeamId;
            this.TeamName = championsLeagueTeam.Name;
            if (championsLeagueTeam.TotalCQPoints > 0)
            {
                this.TotalCQPoints = championsLeagueTeam.TotalCQPoints;
            }
            else
            {
                this.TotalCQPoints = championsLeagueTeam.ChampionsLeagueTeamBikeRiders.Sum(br => br.BikeRiderDetail.CQPoints);
            }
            this.Note = championsLeagueTeam.Note;

        }

        #region GET

        public int SumCQ
        {
            get
            {
                if (this.BikeRiders != null)
                {
                    return this.BikeRiders.Sum(br => br.CQPoints);
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion
    }

    public class VMViewCompetitionTeam
    {
        public int CompetitionTeamId { get; set; }
        public string Name { get; set; }
        public int TotalCQPoints { get; set; }

        public int BikeRiderDetailId { get; set; }

        public int BikeRiderId { get; set; }

        public string BikeRiderName { get; set; }

        public string BikeTeamCode { get; set; }

        public string Nationality { get; set; }

        public int CQPoints { get; set; }

        public int Year { get; set; }

    }
}
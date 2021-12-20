using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMChampionsLeagueTeam
    {
        public int ChampionsLeagueTeamId { get; set; }
        public string TeamName { get; set; }
        public int TotalCQPoints { get; set; }
        public string Note { get; set; }
        public string Color { get; set; }
        public int Year { get; set; }

        public VMChampionsLeagueTeam()
        {

        }

        public VMChampionsLeagueTeam(sykkelkonken.Data.ChampionsLeagueTeam championsLeagueTeam)
        {
            this.ChampionsLeagueTeamId = championsLeagueTeam.ChampionsLeagueTeamId;
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
            this.Color = championsLeagueTeam.Color;
        }
    }
}
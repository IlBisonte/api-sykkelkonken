using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models.CompetitionTeam
{
    public class VMLotteryTeam
    {
        public int LotteryTeamId { get; set; }
        public string Name { get; set; }
        public string Initials { get; set; }
        public int TotalCQPoints { get; set; }
        public string Note { get; set; }
        public string Color { get; set; }
        public int Year { get; set; }

        public VMLotteryTeam()
        {

        }

        public VMLotteryTeam(sykkelkonken.Data.LotteryTeam lotteryTeam)
        {
            this.LotteryTeamId = lotteryTeam.LotteryTeamId;
            this.Name = lotteryTeam.Name;
            if (lotteryTeam.TotalCQPoints > 0)
            {
                this.TotalCQPoints = lotteryTeam.TotalCQPoints;
            }
            else
            {
                this.TotalCQPoints = lotteryTeam.LotteryTeamBikeRiders.Sum(br => br.BikeRiderDetail.CQPoints);
            }
            this.Note = lotteryTeam.Note;
            this.Color = lotteryTeam.Color;
        }
    }
}
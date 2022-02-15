using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models.CompetitionTeam
{
    public class VMYouthTeam
    {
        public int YouthTeamId { get; set; }
        public string Name { get; set; }
        public string Initials { get; set; }
        public int TotalCQPoints { get; set; }
        public string Note { get; set; }
        public int Year { get; set; }

        public VMYouthTeam()
        {

        }

        public VMYouthTeam(sykkelkonken.Data.YouthTeam youthTeam)
        {
            this.YouthTeamId = youthTeam.YouthTeamId;
            this.Name = youthTeam.Name;
            if (youthTeam.TotalCQPoints > 0)
            {
                this.TotalCQPoints = youthTeam.TotalCQPoints;
            }
            else
            {
                this.TotalCQPoints = youthTeam.YouthTeamBikeRiders.Sum(br => br.BikeRiderDetail.CQPoints);
            }
            this.Note = youthTeam.Note;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VmNoOfVictoriesByCompTeam
    {
        public int CompetitionTeamId { get; set; }
        public string Name { get; set; }
        public int NoOfVictories { get; set; }
        public int NoOfSecondPlace { get; set; }
        public int NoOfThirdPlace { get; set; }
        public int NoOfStageWins { get; set; }

        public List<string> BikeRacesWon { get; set; }

        public string BikeRacesWonToString 
        { 
            get
            {
                string bikeRacesWon = "";
                foreach (var br in this.BikeRacesWon)
                {
                    if (bikeRacesWon.Length == 0)
                    {
                        bikeRacesWon = br;
                    }
                    else
                    {
                        bikeRacesWon = string.Format("{0}, {1}", bikeRacesWon, br);
                    }
                }
                return bikeRacesWon;
            }
        }
    }
}
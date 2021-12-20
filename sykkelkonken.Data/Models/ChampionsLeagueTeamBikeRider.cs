using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Data
{
    public class ChampionsLeagueTeamBikeRider
    {
        [Key]
        public int ChampionsLeagueTeamBikeRiderId { get; set; }

        public int ChampionsLeagueTeamId { get; set; }

        public int BikeRiderDetailId { get; set; }

        #region Navigation

        [ForeignKey("ChampionsLeagueTeamId")]
        public virtual ChampionsLeagueTeam ChampionsLeagueTeam { get; set; }

        [ForeignKey("BikeRiderDetailId")]
        public virtual BikeRiderDetail BikeRiderDetail { get; set; }

        #endregion
    }
}

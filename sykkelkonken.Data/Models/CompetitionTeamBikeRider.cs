using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Data
{
    public class CompetitionTeamBikeRider
    {
        [Key]
        public int CompetitionTeamBikeRiderId { get; set; }

        public int CompetitionTeamId { get; set; }

        public int BikeRiderId { get; set; }

        #region Navigation

        [ForeignKey("CompetitionTeamId")]
        public virtual CompetitionTeam CompetitionTeam { get; set; }

        [ForeignKey("BikeRiderId")]
        public virtual BikeRider BikeRider { get; set; }

        #endregion
    }
}

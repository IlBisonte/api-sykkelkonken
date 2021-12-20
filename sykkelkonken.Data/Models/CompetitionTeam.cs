using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Data
{
    public class CompetitionTeam
    {
        [Key]
        public int CompetitionTeamId { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public int TotalCQPoints { get; set; }
        public string Note { get; set; }
        public int Year { get; set; }

        #region Navigation

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        
        public virtual ICollection<CompetitionTeamBikeRider> CompetitionTeamBikeRiders { get; set; }

        #endregion
    }
}

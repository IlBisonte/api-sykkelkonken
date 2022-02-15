
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sykkelkonken.Data
{
    public class YouthTeam
    {
        [Key]
        public int YouthTeamId { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public int TotalCQPoints { get; set; }
        public string Note { get; set; }
        public int Year { get; set; }
        #region Navigation

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual ICollection<YouthTeamBikeRider> YouthTeamBikeRiders { get; set; }

        #endregion
    }
}

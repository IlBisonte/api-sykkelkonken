
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sykkelkonken.Data
{
    public class YouthTeamBikeRider
    {
        [Key]
        public int YouthTeamBikeRiderId { get; set; }

        public int YouthTeamId { get; set; }

        public int BikeRiderDetailId { get; set; }

        #region Navigation

        [ForeignKey("YouthTeamId")]
        public virtual YouthTeam YouthTeam { get; set; }

        [ForeignKey("BikeRiderDetailId")]
        public virtual BikeRiderDetail BikeRiderDetail { get; set; }

        #endregion
    }
}


using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sykkelkonken.Data
{
    public class LotteryTeamBikeRider
    {
        [Key]
        public int LotteryTeamBikeRiderId { get; set; }

        public int LotteryTeamId { get; set; }

        public int BikeRiderDetailId { get; set; }

        #region Navigation

        [ForeignKey("LotteryTeamId")]
        public virtual LotteryTeam LotteryTeam { get; set; }

        [ForeignKey("BikeRiderDetailId")]
        public virtual BikeRiderDetail BikeRiderDetail { get; set; }

        #endregion
    }
}

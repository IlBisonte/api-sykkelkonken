
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sykkelkonken.Data
{
    public class LotteryTeam
    {
        [Key]
        public int LotteryTeamId { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public string Initials { get; set; }
        public int TotalCQPoints { get; set; }
        public string Note { get; set; }
        public string Color { get; set; }
        public int Year { get; set; }
        #region Navigation

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual ICollection<LotteryTeamBikeRider> LotteryTeamBikeRiders { get; set; }

        #endregion
    }
}

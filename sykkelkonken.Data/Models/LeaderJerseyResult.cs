using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Data
{
    public class LeaderJerseyResult
    {
        [Key, Column(Order = 0)]
        public int BikeRaceDetailId { get; set; }

        [Key, Column(Order = 1)]
        public int LeaderJerseyPosition { get; set; }

        [Key, Column(Order = 2)]
        public int LeaderJerseyId { get; set; }

        public int BikeRiderId { get; set; }

        #region Navigation

        [ForeignKey("BikeRaceDetailId")]
        public virtual BikeRaceDetail BikeRaceDetail { get; set; }

        [ForeignKey("LeaderJerseyId")]
        public virtual LeaderJersey LeaderJersey { get; set; }

        [ForeignKey("BikeRiderId")]
        public virtual BikeRider BikeRider { get; set; }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sykkelkonken.Data
{
    public class BikeRaceResult
    {
        [Key, Column(Order = 0)]
        public int BikeRaceDetailId { get; set; }
        
        public int BikeRiderId { get; set; }

        [Key, Column(Order = 1)]
        public int Position { get; set; }

        #region Navigation

        [ForeignKey("BikeRaceDetailId")]
        public virtual BikeRaceDetail BikeRaceDetail { get; set; }

        [ForeignKey("BikeRiderId")]
        public virtual BikeRider BikeRider { get; set; }

        #endregion
    }
}

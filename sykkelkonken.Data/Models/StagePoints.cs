using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sykkelkonken.Data
{
    public class StagePoints
    {
        [Key, Column(Order = 0)]
        public int BikeRaceCategoryId { get; set; }

        [Key, Column(Order = 1)]
        public int StagePosition { get; set; }

        public int Points { get; set; }

        #region Navigation

        [ForeignKey("BikeRaceCategoryId")]
        public virtual BikeRaceCategory BikeRaceCategory { get; set; }

        #endregion
    }
}

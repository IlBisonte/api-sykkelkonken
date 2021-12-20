using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sykkelkonken.Data
{
    public class BikeRaceCategoryPoints
    {
        [Key]
        public int BikeRaceCategoryPointsId { get; set; }

        public int BikeRaceCategoryId { get; set; }

        public int Position { get; set; }

        public int Points { get; set; }
        
        #region Navigation

        [ForeignKey("BikeRaceCategoryId")]
        public virtual BikeRaceCategory BikeRaceCategory { get; set; }

        #endregion
    }
}

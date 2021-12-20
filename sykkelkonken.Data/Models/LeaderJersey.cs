using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Data
{
    public class LeaderJersey
    {
        [Key]
        public int LeaderJerseyId { get; set; }
        
        public int BikeRaceCategoryId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        #region Navigation

        public virtual ICollection<LeaderJerseyPoints> LeaderJerseyPoints { get; set; }

        [ForeignKey("BikeRaceCategoryId")]
        public virtual BikeRaceCategory BikeRaceCategory { get; set; }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Data
{
    public class BikeRiderDetail
    {
        [Key]
        public int BikeRiderDetailId { get; set; }

        public int BikeRiderId { get; set; }

        public string BikeTeamCode { get; set; }

        public string BikeTeamName { get; set; }

        public int CQPoints { get; set; }

        public int Year { get; set; }

        #region Navigation

        [ForeignKey("BikeRiderId")]
        public virtual BikeRider BikeRider { get; set; }

        #endregion
    }
}

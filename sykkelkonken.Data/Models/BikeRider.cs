using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Data
{
    public class BikeRider
    {
        [Key]
        public int BikeRiderId { get; set; }

        public string BikeRiderName { get; set; }

        //public int BikeTeamId { get; set; }

        public string BikeTeamCode { get; set; }

        public string BikeTeamName { get; set; }

        public string Nationality { get; set; }

        public int? CQPoints { get; set; }

        public DateTime? BirthDate { get; set; }

        #region Navigation

        public virtual ICollection<BikeRaceResult> BikeRaceResults { get; set; }

        #endregion

    }
}

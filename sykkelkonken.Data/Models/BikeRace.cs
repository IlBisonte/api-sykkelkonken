using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Data
{
    public class BikeRace
    {
        [Key]
        public int BikeRaceId { get; set; }

        public string Name { get; set; }

        public string CountryName { get; set; }

        #region Navigation
        public virtual ICollection<BikeRaceDetail> BikeRaceDetails { get; set; }
        #endregion
    }
}

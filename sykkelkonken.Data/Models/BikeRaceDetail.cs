using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sykkelkonken.Data
{
    public class BikeRaceDetail
    {
        [Key]
        public int BikeRaceDetailId { get; set; }

        public int BikeRaceId { get; set; }

        public int Year { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? FinishDate { get; set; }

        public int? BikeRaceCategoryId { get; set; }

        public int? NoOfStages { get; set; }

        public bool? HasTTT { get; set; }

        public bool? IsCalculated { get; set; }

        public string Name { get; set; }//race may change name from year to year

        public string BikeRaceNameShort { get; set; }

        public bool? Cancelled { get; set; }
        public string CountryName { get; set; }//race may change name from year to year


        #region Navigation

        [ForeignKey("BikeRaceId")]
        public virtual BikeRace BikeRace { get; set; }

        [ForeignKey("BikeRaceCategoryId")]
        public virtual BikeRaceCategory BikeRaceCategory { get; set; }

        public virtual ICollection<BikeRaceResult> BikeRaceResults { get; set; }

        public virtual ICollection<StageResult> StageResults { get; set; }

        public virtual ICollection<LeaderJerseyResult> LeaderJerseyResults { get; set; }

        #endregion
    }
}

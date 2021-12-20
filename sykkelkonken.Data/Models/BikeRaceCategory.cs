using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sykkelkonken.Data
{
    public class BikeRaceCategory
    {
        [Key]
        public int BikeRaceCategoryId { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public bool IsStageRace { get; set; }

        public enum BikeRaceCategoryIdEnum { OneDay = 1, Monument = 2, StageRace = 3, TourDeFrance = 4, GiroVuelta = 5 }

        #region Navigation

        public virtual ICollection<BikeRaceDetail> BikeRaces { get; set; }

        public virtual ICollection<BikeRaceCategoryPoints> BikeRaceCategoryPoints { get; set; }

        public virtual ICollection<StagePoints> StagePointss { get; set; }

        public virtual ICollection<LeaderJersey> LeaderJersey { get; set; }

        #endregion
    }
}

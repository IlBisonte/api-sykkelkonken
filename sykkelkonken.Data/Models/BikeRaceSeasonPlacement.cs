using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sykkelkonken.Data
{
    public class BikeRaceSeasonPlacement
    {
        [Key]
        public int BikeRaceSeasonPlacementId { get; set; }

        public int Year { get; set; }

        public int BikeRaceDetailId { get; set; }

        [ForeignKey("BikeRaceDetailId")]
        public virtual BikeRaceDetail BikeRaceDetail { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sykkelkonken.Data
{
    public class BikeRiderResult
    {
        [Key]
        public int BikeRiderResultId { get; set; }

        public int BikeRiderId { get; set; }

        public int Year { get; set; }

        public int Points { get; set; }

        public float Index { get; set; }
    }
}

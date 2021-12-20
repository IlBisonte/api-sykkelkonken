using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Data
{
    public class LeaderJerseyPoints
    {
        [Key, Column(Order = 0)]
        public int LeaderJerseyId { get; set; }

        [Key, Column(Order = 1)]
        public int Position { get; set; }

        public int? Points { get; set; }

        #region Navigation

        [ForeignKey("LeaderJerseyId")]
        public virtual LeaderJersey LeaderJersey { get; set; }

        #endregion
    }
}

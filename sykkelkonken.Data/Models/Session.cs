using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Data
{
    public class Session
    {
        [Key]
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastAccessedDate { get; set; }
        public DateTime ExpireDate { get; set; }


        #region Navigation

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMStage
    {
        public int BikeRaceId { get; set; }
        public int StageNo { get; set; }

        public IList<VMStageResult> StageResults { get; set; }
    }
}
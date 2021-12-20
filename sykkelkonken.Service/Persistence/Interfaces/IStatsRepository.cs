using sykkelkonken.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Service.Persistence
{
    public interface IStatsRepository
    {
        IList<VMBikeRiderStats> GetBikeRiderStats(int year);

    }
}

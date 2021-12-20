using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using sykkelkonken.Data;
using sykkelkonken.Service.Models;

namespace sykkelkonken.Service.Persistence
{
    public class StatsRepository : IStatsRepository
    {
        private readonly Context _context;

        public StatsRepository(Context context)
        {
            _context = context;
        }

        public IList<VMBikeRiderStats> GetBikeRiderStats(int year)
        {
            var res = _context.Database.SqlQuery<VMBikeRiderStats>(string.Format(
                @"SELECT      BikeRiderId, BikeRiderName, BikeTeamCode, Year, Points, NoOfSelections, SelectedBy, CQPoints, RiderIndex, CLTeamName, Color
                FROM            v_BikeRiderStats
                WHERE (Points > 0 OR NoOfSelections > 0) AND Year = {0}
                ORDER BY Points DESC, CQPoints DESC", year)).ToList();
            return res;
        }
    }
}
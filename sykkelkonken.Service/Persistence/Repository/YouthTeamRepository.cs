using sykkelkonken.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Persistence
{
    public class YouthTeamRepository : IYouthTeamRepository
    {
        private readonly Context _context;

        public YouthTeamRepository(Context context)
        {
            _context = context;
        }

        public IList<YouthTeam> Get(int year)
        {
            return this._context.YouthTeams.Where(clt => clt.Year == year).ToList();
        }

        public YouthTeam GetById(int youthTeamId)
        {
            return this._context.YouthTeams.SingleOrDefault(cl => cl.YouthTeamId == youthTeamId);
        }

        public IList<YouthTeamBikeRider> GetBikeRidersYouthTeam(int youthTeamId)
        {
            return this._context.YouthTeams.SingleOrDefault(cl => cl.YouthTeamId == youthTeamId).YouthTeamBikeRiders.ToList();
        }

        public int AddYouthTeam(YouthTeam youthTeam)
        {
            YouthTeam ct = _context.YouthTeams.Add(new YouthTeam()
            {
                UserId = youthTeam.UserId,
                Name = youthTeam.Name,
                TotalCQPoints = youthTeam.TotalCQPoints,
                Year = youthTeam.Year
            });

            this._context.SaveChanges();
            return ct.YouthTeamId;
        }

        public void UpdateYouthTeam(int youthTeamId, string teamName)
        {
            var clTeam = _context.YouthTeams.SingleOrDefault(cl => cl.YouthTeamId == youthTeamId);
            if (clTeam != null)
            {
                clTeam.Name = teamName;
            }
        }

        public void RemoveYouthTeam(int youthTeamId)
        {
            var youthTeam = this._context.YouthTeams.SingleOrDefault(cl => cl.YouthTeamId == youthTeamId);
            if (youthTeam != null)
            {
                this._context.YouthTeamBikeRiders.RemoveRange(youthTeam.YouthTeamBikeRiders);
                this._context.YouthTeams.Remove(youthTeam);
            }
        }

        public void RemoveYouthTeams()
        {
            //this._context.Database.ExecuteSqlCommand("truncate table dbo.YouthTeamBikeRider");
            //this._context.Database.ExecuteSqlCommand("truncate table dbo.YouthTeam");
        }

        public void AddBikeRiderToYouthTeam(int youthTeamId, int bikeRiderDetailId)
        {
            YouthTeamBikeRider youthTeamBikeRider = new YouthTeamBikeRider()
            {
                YouthTeamId = youthTeamId,
                BikeRiderDetailId = bikeRiderDetailId,
            };
            _context.YouthTeamBikeRiders.Add(youthTeamBikeRider);
        }

        public void UpdateRiderYouthTeam(int youthTeamId, int origBikeRiderDetailId, int newBikeRiderDetailId)
        {
            var youthTeam = _context.YouthTeamBikeRiders.FirstOrDefault(r => r.YouthTeamId == youthTeamId && r.BikeRiderDetailId == origBikeRiderDetailId);
            if (youthTeam != null)
            {
                youthTeam.BikeRiderDetailId = newBikeRiderDetailId;
            }
        }

        public void RemoveRiderYouthTeam(int youthTeamId, int bikeRiderDetailId)
        {
            var youthTeamBikeRider = _context.YouthTeamBikeRiders.FirstOrDefault(r => r.YouthTeamId == youthTeamId && r.BikeRiderDetailId == bikeRiderDetailId);
            if (youthTeamBikeRider != null)
            {
                this._context.YouthTeamBikeRiders.Remove(youthTeamBikeRider);
            }
        }
    }
}
using sykkelkonken.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Persistence
{
    public class LotteryTeamRepository : ILotteryTeamRepository
    {
        private readonly Context _context;

        public LotteryTeamRepository(Context context)
        {
            _context = context;
        }

        public IList<LotteryTeam> Get(int year)
        {
            return this._context.LotteryTeams.Where(clt => clt.Year == year).ToList();
        }

        public LotteryTeam GetById(int lotteryTeamId)
        {
            return this._context.LotteryTeams.SingleOrDefault(cl => cl.LotteryTeamId == lotteryTeamId);
        }

        public IList<LotteryTeamBikeRider> GetBikeRidersLotteryTeam(int lotteryTeamId)
        {
            return this._context.LotteryTeams.SingleOrDefault(cl => cl.LotteryTeamId == lotteryTeamId).LotteryTeamBikeRiders.ToList();
        }

        public int AddLotteryTeam(LotteryTeam lotteryTeam)
        {
            LotteryTeam ct = _context.LotteryTeams.Add(new LotteryTeam()
            {
                UserId = lotteryTeam.UserId,
                Name = lotteryTeam.Name,
                Initials = lotteryTeam.Initials,
                TotalCQPoints = lotteryTeam.TotalCQPoints,
                Color = lotteryTeam.Color,
                Year = lotteryTeam.Year
            });

            this._context.SaveChanges();
            return ct.LotteryTeamId;
        }

        public void UpdateLotteryTeam(int lotteryTeamId, string teamName, string color)
        {
            var clTeam = _context.LotteryTeams.SingleOrDefault(cl => cl.LotteryTeamId == lotteryTeamId);
            if (clTeam != null)
            {
                clTeam.Name = teamName;
                clTeam.Color = color;
            }
        }

        public void RemoveLotteryTeam(int lotteryTeamId)
        {
            var lotteryTeam = this._context.LotteryTeams.SingleOrDefault(cl => cl.LotteryTeamId == lotteryTeamId);
            if (lotteryTeam != null)
            {
                this._context.LotteryTeamBikeRiders.RemoveRange(lotteryTeam.LotteryTeamBikeRiders);
                this._context.LotteryTeams.Remove(lotteryTeam);
            }
        }

        public void RemoveLotteryTeams()
        {
            //this._context.Database.ExecuteSqlCommand("truncate table dbo.LotteryTeamBikeRider");
            //this._context.Database.ExecuteSqlCommand("truncate table dbo.LotteryTeam");
        }

        public void AddBikeRiderToLotteryTeam(int lotteryTeamId, int bikeRiderDetailId)
        {
            LotteryTeamBikeRider lotteryTeamBikeRider = new LotteryTeamBikeRider()
            {
                LotteryTeamId = lotteryTeamId,
                BikeRiderDetailId = bikeRiderDetailId,
            };
            _context.LotteryTeamBikeRiders.Add(lotteryTeamBikeRider);
        }

        public void UpdateRiderLotteryTeam(int lotteryTeamId, int origBikeRiderDetailId, int newBikeRiderDetailId)
        {
            var lotteryTeam = _context.LotteryTeamBikeRiders.FirstOrDefault(r => r.LotteryTeamId == lotteryTeamId && r.BikeRiderDetailId == origBikeRiderDetailId);
            if (lotteryTeam != null)
            {
                lotteryTeam.BikeRiderDetailId = newBikeRiderDetailId;
            }
        }

        public void RemoveRiderLotteryTeam(int lotteryTeamId, int bikeRiderDetailId)
        {
            var lotteryTeamBikeRider = _context.LotteryTeamBikeRiders.FirstOrDefault(r => r.LotteryTeamId == lotteryTeamId && r.BikeRiderDetailId == bikeRiderDetailId);
            if (lotteryTeamBikeRider != null)
            {
                this._context.LotteryTeamBikeRiders.Remove(lotteryTeamBikeRider);
            }
        }
    }
}
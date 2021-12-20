using sykkelkonken.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Persistence
{
    public class ChampionsLeagueTeamRepository : IChampionsLeagueTeamRepository
    {
        private readonly Context _context;

        public ChampionsLeagueTeamRepository(Context context)
        {
            _context = context;
        }

        public IList<ChampionsLeagueTeam> Get(int year)
        {
            return this._context.ChampionsLeagueTeams.Where(clt => clt.Year == year).ToList();
        }

        public ChampionsLeagueTeam GetById(int clTeamId)
        {
            return this._context.ChampionsLeagueTeams.SingleOrDefault(cl => cl.ChampionsLeagueTeamId == clTeamId);
        }

        public IList<ChampionsLeagueTeamBikeRider> GetBikeRidersCLTeam(int championsLeagueTeamId)
        {
            return this._context.ChampionsLeagueTeams.SingleOrDefault(cl => cl.ChampionsLeagueTeamId == championsLeagueTeamId).ChampionsLeagueTeamBikeRiders.ToList();
        }

        public int AddChampionsLeagueTeam(ChampionsLeagueTeam championsLeagueTeam)
        {
            ChampionsLeagueTeam ct = _context.ChampionsLeagueTeams.Add(new ChampionsLeagueTeam()
            {
                UserId = championsLeagueTeam.UserId,
                Name = championsLeagueTeam.Name,
                TotalCQPoints = championsLeagueTeam.TotalCQPoints,
                Color = championsLeagueTeam.Color,
                Year = championsLeagueTeam.Year
            });

            this._context.SaveChanges();
            return ct.ChampionsLeagueTeamId;
        }

        public int AddCompetitionTeam(CompetitionTeam competitionTeam)
        {
            CompetitionTeam ct = _context.CompetitionTeams.Add(new CompetitionTeam()
            {
                UserId = competitionTeam.UserId,
                Name = competitionTeam.Name,
                TotalCQPoints = competitionTeam.TotalCQPoints,
            });

            this._context.SaveChanges();
            return ct.CompetitionTeamId;
        }

        public void UpdateChampionsLeagueTeam(int championsLeagueTeamId, string teamName, string color)
        {
            var clTeam = _context.ChampionsLeagueTeams.SingleOrDefault(cl => cl.ChampionsLeagueTeamId == championsLeagueTeamId);
            if (clTeam != null)
            {
                clTeam.Name = teamName;
                clTeam.Color = color;
            }
        }

        public void RemoveChampionsLeagueTeam(int championsLeagueTeamId)
        {
            var championsLeagueTeam = this._context.ChampionsLeagueTeams.SingleOrDefault(cl => cl.ChampionsLeagueTeamId == championsLeagueTeamId);
            if (championsLeagueTeam != null)
            {
                this._context.ChampionsLeagueTeamBikeRiders.RemoveRange(championsLeagueTeam.ChampionsLeagueTeamBikeRiders);
                this._context.ChampionsLeagueTeams.Remove(championsLeagueTeam);
            }
        }

        public void RemoveChampionsLeagueTeams()
        {
            this._context.Database.ExecuteSqlCommand("truncate table dbo.ChampionsLeagueTeamBikeRider");
            this._context.Database.ExecuteSqlCommand("truncate table dbo.ChampionsLeagueTeam");
        }

        public void AddBikeRiderToChampionsLeagueTeam(int championsLeagueTeamId, int bikeRiderDetailId)
        {
            ChampionsLeagueTeamBikeRider championsLeagueTeamBikeRider = new ChampionsLeagueTeamBikeRider()
            {
                ChampionsLeagueTeamId = championsLeagueTeamId,
                BikeRiderDetailId = bikeRiderDetailId,
            };
            _context.ChampionsLeagueTeamBikeRiders.Add(championsLeagueTeamBikeRider);
        }

        public void UpdateRiderChampionsLeagueTeam(int championsLeagueTeamId, int origBikeRiderDetailId, int newBikeRiderDetailId)
        {
            var clTeam = _context.ChampionsLeagueTeamBikeRiders.FirstOrDefault(r => r.ChampionsLeagueTeamId == championsLeagueTeamId && r.BikeRiderDetailId == origBikeRiderDetailId);
            if (clTeam != null)
            {
                clTeam.BikeRiderDetailId = newBikeRiderDetailId;
            }
        }

        public void RemoveRiderChampionsLeagueTeam(int championsLeagueTeamId, int bikeRiderDetailId)
        {
            var clTeamBikeRider = _context.ChampionsLeagueTeamBikeRiders.FirstOrDefault(r => r.ChampionsLeagueTeamId == championsLeagueTeamId && r.BikeRiderDetailId == bikeRiderDetailId);
            if (clTeamBikeRider != null)
            {
                this._context.ChampionsLeagueTeamBikeRiders.Remove(clTeamBikeRider);
            }
        }
    }
}
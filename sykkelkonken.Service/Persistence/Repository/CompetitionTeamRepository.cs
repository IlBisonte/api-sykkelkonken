using sykkelkonken.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Persistence
{
    public class CompetitionTeamRepository : ICompetitionTeamRepository
    {
        private readonly Context _context;

        public CompetitionTeamRepository(Context context)
        {
            _context = context;
        }

        public void AddBikeRiderToCompetitionTeam(int competitionTeamId, int bikeRiderId)
        {
            CompetitionTeamBikeRider competitionTeamBikeRider = new CompetitionTeamBikeRider()
            {
                CompetitionTeamId = competitionTeamId,
                BikeRiderId = bikeRiderId,
            };
            _context.CompetitionTeamBikeRiders.Add(competitionTeamBikeRider);
        }

        public int AddCompetitionTeam(CompetitionTeam competitionTeam)
        {
            CompetitionTeam ct = _context.CompetitionTeams.Add(new CompetitionTeam()
            {
                UserId = competitionTeam.UserId,
                Name = competitionTeam.Name,
                TotalCQPoints = competitionTeam.TotalCQPoints,
                Year = competitionTeam.Year,
            });

            this._context.SaveChanges();
            return ct.CompetitionTeamId;
        }

        public IList<CompetitionTeam> Get(int year)
        {
            return this._context.CompetitionTeams.Where(ct => ct.Year == year).ToList();
        }

        public IList<Models.VMViewCompetitionTeam> GetCompetitionTeamsFromView(int year)
        {

            var res = _context.Database.SqlQuery<Models.VMViewCompetitionTeam>(string.Format(
                @"SELECT       *
                FROM            v_CompetitionTeamBikeRider
                WHERE Year = {0}", year)).ToList();
            return res;
            //return this._context.CompetitionTeams.Where(ct => ct.Year == year).ToList();
        }

        public CompetitionTeam GetByCompetitionTeamId(int competitionTeamId)
        {
            return this._context.CompetitionTeams.Where(ct => ct.CompetitionTeamId == competitionTeamId).SingleOrDefault();
        }

        public void RemoveCompetitionTeams()
        {
            this._context.Database.ExecuteSqlCommand("truncate table dbo.CompetitionTeamBikeRider");
            this._context.Database.ExecuteSqlCommand("truncate table dbo.CompetitionTeam");
        }
    }
}
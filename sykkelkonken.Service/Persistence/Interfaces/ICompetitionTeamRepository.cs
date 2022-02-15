using sykkelkonken.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Service.Persistence
{
    public interface ICompetitionTeamRepository
    {
        IList<CompetitionTeam> Get(int year);
        IList<Models.VMViewCompetitionTeam> GetCompetitionTeamsFromView(int year);
        CompetitionTeam GetByCompetitionTeamId(int competitionTeamId);
        int AddCompetitionTeam(CompetitionTeam competitionTeam);//Add CompetitionTeam and return new CompetitionTeamId
        void AddBikeRiderToCompetitionTeam(int competitionTeamId, int bikeRiderId);
        void RemoveCompetitionTeams();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Service.Persistence
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; set; }
        IBikeRaceRepository BikeRaces { get; set; }
        IBikeRiderRepository BikeRiders { get; set; }
        ICompetitionTeamRepository CompetitionTeams { get; set; }
        IChampionsLeagueTeamRepository ChampionsLeagueTeams { get; set; }
        ILotteryTeamRepository LotteryTeams { get; set; }
        IYouthTeamRepository YouthTeams { get; set; }
        IResultRepository Results { get; set; }
        IStatsRepository Stats { get; set; }
        ISessionRepository Session { get; set; }
        void Complete();
    }
}

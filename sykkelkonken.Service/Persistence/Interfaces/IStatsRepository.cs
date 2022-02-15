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

        IList<VMBikeRiderScoreAllTime> GetBikeRiderScoreAllTime();



        IList<VMCompetitionTeamStatsAllTime> GetCompetitionTeamStatsAllTime();

        IList<VMChampionsLeagueTeamStatsAllTime> GetChampionsLeagueTeamStatsAllTime();

        IList<VMCompetitionTeamPointsPerBikeRace> GetCompetitionTeamResultPerBikeRace(int year);
        IList<VMCompetitionTeamPointsPerBikeRace> GetCompetitionTeamPointsPerBikeRace(int year, int bikeRaceDetailId);

        IList<VMCompetitionTeamPointsPerStage> GetCompetitionTeamPointsPerStage(int year);

        IList<CompetitionTeamPointsByBikeRaceCategory> GetCompetitionTeamPointsByBikeRaceCategory(int year, int competitionTeamId);
        IList<CompetitionTeamPointsByBikeRace> GetCompetitionTeamPointsByCompetitionTeam(int year, int competitionTeamId);

        IList<BikeRiderPointsByBikeRace> GetBikeRiderPointsByCompetitionTeam(int year, int competitionTeamId);
        IList<BikeRiderPointsByBikeRace> GetBikeRiderPointsByBikeRace(int year, int competitionTeamId, int bikeRaceDetailId);
        IList<BikeRiderPointsByBikeRace> GetBikeRacePointsByBikeRider(int year, int bikeRiderDetailId);
        IList<string> GetCompTeamsWithSelectedBikeRider(int bikeRiderDetailId);
    }
}

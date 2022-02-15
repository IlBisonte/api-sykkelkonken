using sykkelkonken.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Service.Persistence
{
    public interface IResultRepository
    {
        IList<VMCompetitionTeamResults> GetCompetitionTeamResults(int year);
        IList<VMCompetitionTeamBikeRiderResults> GetCompetitionTeamBikeRiderResults(int competitionTeamId);
        IList<VMBikeRiderResults> GetBikeRiderResults(int bikeRiderId, int year);
        IList<BikeRaceTotalCompetitionTeamResults> GetBikeRaceTotalCompetitionTeamResults(int bikeRaceDetailId);
        IList<BikeRaceGCCompetitionTeamResults> GetBikeRaceGCCompetitionTeamResults(int bikeRaceDetailId);
        IList<BikeRaceStageCompetitionTeamResults> GetBikeRaceStageCompetitionTeamResults(int bikeRaceDetailId);
        IList<BikeRaceLeaderJerseyCompetitionTeamResults> GetBikeRaceLeaderJerseyCompetitionTeamResults(int bikeRaceDetailId);
        IList<BikeRaceTotalCLTeamResults> GetBikeRaceTotalCLTeamResults(int bikeRaceDetailId);
        IList<BikeRaceGCCLTeamResults> GetBikeRaceGCCLTeamResults(int bikeRaceDetailId);
        IList<BikeRaceStageCLTeamResults> GetBikeRaceStageCLTeamResults(int bikeRaceDetailId);
        IList<BikeRaceLeaderJerseyCLTeamResults> GetBikeRaceLeaderJerseyCLTeamResults(int bikeRaceDetailId);
        IList<VMCompetitionTeamResults> GetCompetitionTeamResults_Monuments(int year);
        IList<VMCompetitionTeamBikeRiderResults> GetCompetitionTeamBikeRiderResults_Monuments(int competitionTeamId);
        IList<VMBikeRiderResults> GetBikeRiderResults_Monuments(int bikeRiderDetailId, int year);
        IList<VMCompetitionTeamResults> GetCLTeamResults(int year);
        IList<VMCompetitionTeamBikeRiderResults> GetCLTeamBikeRiderResults(int clTeamId);
        IList<VMCompetitionTeamResults> GetLotteryTeamResults(int year);
        IList<VMCompetitionTeamBikeRiderResults> GetLotteryTeamBikeRiderResults(int lotteryTeamId);
        IList<VMCompetitionTeamResults> GetYouthTeamResults(int year);
        IList<VMCompetitionTeamBikeRiderResults> GetYouthTeamBikeRiderResults(int youthTeamId);
    }
}

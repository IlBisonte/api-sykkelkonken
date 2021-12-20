using sykkelkonken.Service.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMBikeRaceCompetitionTeamResults
    {
        private readonly IUnitOfWork _unitOfWork;
        public IList<BikeRaceTotalCompetitionTeamResults> TotalResults { get; set; }
        public IList<BikeRaceGCCompetitionTeamResults> GCResults { get; set; }
        public IList<BikeRaceStageCompetitionTeamResults> StageResults { get; set; }
        public IList<BikeRaceLeaderJerseyCompetitionTeamResults> LeaderJerseyResults { get; set; }

        public VMBikeRaceCompetitionTeamResults(int bikeRaceDetailId)
        {
            _unitOfWork = new UnitOfWork();
            TotalResults = _unitOfWork.Results.GetBikeRaceTotalCompetitionTeamResults(bikeRaceDetailId).Where(r => r.TotalPoints > 0).OrderByDescending(r => r.TotalPoints).ThenBy(r => r.CompetitionTeamName).ToList();
            GCResults = _unitOfWork.Results.GetBikeRaceGCCompetitionTeamResults(bikeRaceDetailId).OrderByDescending(r => r.GCPoints).ThenBy(r => r.CompetitionTeamName).ToList();
            StageResults = _unitOfWork.Results.GetBikeRaceStageCompetitionTeamResults(bikeRaceDetailId).OrderBy(r => r.StageNo).ThenByDescending(r => r.StagePoints).ToList();
            LeaderJerseyResults = _unitOfWork.Results.GetBikeRaceLeaderJerseyCompetitionTeamResults(bikeRaceDetailId).OrderByDescending(r => r.LeaderJerseyPoints).ToList();
        }
    }
}
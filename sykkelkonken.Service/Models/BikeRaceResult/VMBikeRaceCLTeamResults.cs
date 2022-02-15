using sykkelkonken.Service.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMBikeRaceCLTeamResults
    {
        private readonly IUnitOfWork _unitOfWork;
        public IList<BikeRaceTotalCLTeamResults> TotalResults { get; set; }
        public IList<BikeRaceGCCLTeamResults> GCResults { get; set; }
        public IList<BikeRaceStageCLTeamResults> StageResults { get; set; }
        public IList<BikeRaceLeaderJerseyCLTeamResults> LeaderJerseyResults { get; set; }

        public VMBikeRaceCLTeamResults(int bikeRaceDetailId)
        {
            _unitOfWork = new UnitOfWork();
            TotalResults = _unitOfWork.Results.GetBikeRaceTotalCLTeamResults(bikeRaceDetailId).Where(r => r.TotalPoints > 0).OrderByDescending(r => r.TotalPoints).ThenBy(r => r.CompetitionTeamName).ToList();
            GCResults = _unitOfWork.Results.GetBikeRaceGCCLTeamResults(bikeRaceDetailId).OrderByDescending(r => r.GCPoints).ThenBy(r => r.CompetitionTeamName).ToList();
            StageResults = _unitOfWork.Results.GetBikeRaceStageCLTeamResults(bikeRaceDetailId).OrderBy(r => r.StageNo).ThenByDescending(r => r.StagePoints).ToList();
            LeaderJerseyResults = _unitOfWork.Results.GetBikeRaceLeaderJerseyCLTeamResults(bikeRaceDetailId).OrderByDescending(r => r.LeaderJerseyPoints).ToList();
        }
    }
}
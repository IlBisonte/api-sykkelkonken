using sykkelkonken.Service.Models;
using sykkelkonken.Service.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace sykkelkonken.Service.Controllers
{
    public class ResultController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ResultController()
        {
            _unitOfWork = new UnitOfWork();
        }

        [HttpGet]
        public IList<VMCompetitionTeamResults> GetCompetitionTeamResults(int year)
        {
            List<VMCompetitionTeamResults> lstCompetitionTeamResults = _unitOfWork.Results.GetCompetitionTeamResults(year).Select(ct => new VMCompetitionTeamResults()
            {
                Position = ct.Position,
                CompetitionTeamId = ct.CompetitionTeamId,
                Name = ct.Name,
                TotalCQPoints = ct.TotalCQPoints,
                Note = ct.Note,
                Points = ct.Points,
                TeamIndex = ct.TeamIndex,
            }).ToList();

            var lastCalculatedBikeRace = _unitOfWork.BikeRaces.GetLastCalculatedBikeRace(year);
            if (lastCalculatedBikeRace != null)
            {
                var bikeRaceResults = _unitOfWork.Stats.GetCompetitionTeamPointsPerBikeRace(year, lastCalculatedBikeRace.BikeRaceDetailId);
                List<CompetitionTeamPoints> lstPreviousPoints = new List<CompetitionTeamPoints>();
                foreach (var compTeam in lstCompetitionTeamResults)
                {
                    var lastBikeRaceResultPoints = bikeRaceResults.Where(brr => brr.CompetitionTeamId == compTeam.CompetitionTeamId).Sum(brr => brr.TotalPoints);
                    int previousPoints = compTeam.Points - lastBikeRaceResultPoints;
                    lstPreviousPoints.Add(new CompetitionTeamPoints()
                    {
                        CompetitionTeamId = compTeam.CompetitionTeamId,
                        Points = previousPoints
                    });
                }
                int previousRank = 1;
                foreach (var previousPoints in lstPreviousPoints.OrderByDescending(p => p.Points))
                {
                    lstCompetitionTeamResults.SingleOrDefault(ct => ct.CompetitionTeamId == previousPoints.CompetitionTeamId).PreviousRank = previousRank;
                    if (previousPoints.Points > 0)
                    {
                        previousRank++;
                    }
                }
            }

            return lstCompetitionTeamResults;
        }

        [HttpGet]
        public IList<VMCompetitionTeamBikeRiderResults> GetCompetitionTeamBikeRiderResults(int competitionTeamId)
        {
            return _unitOfWork.Results.GetCompetitionTeamBikeRiderResults(competitionTeamId).Select(ct => new VMCompetitionTeamBikeRiderResults()
            {
                CompetitionTeamId = ct.CompetitionTeamId,
                Name = ct.Name,
                Points = ct.Points,
                RiderIndex = ct.RiderIndex,
                BikeRiderId = ct.BikeRiderId,
                BikeRiderDetailId = ct.BikeRiderDetailId,
                BikeRiderName = ct.BikeRiderName,
                CQPoints = ct.CQPoints,
            }).ToList();
        }

        [HttpGet]
        public IList<VMBikeRiderResults> GetBikeRiderResults(int bikeRiderDetailId, int year)
        {
            var bikeRiderResults = _unitOfWork.Results.GetBikeRiderResults(bikeRiderDetailId, year).Select(ct => new VMBikeRiderResults()
            {
                BikeRaceId = ct.BikeRaceId,
                BikeRaceName = ct.BikeRaceName,
                Description = ct.Description,
                BikeRiderId = ct.BikeRiderId,
                BikeRiderDetailId = ct.BikeRiderDetailId,
                BikeRiderName = ct.BikeRiderName,
                StartDate = ct.StartDate,
                FinishDate = ct.FinishDate,
                GCPosition = ct.GCPosition,
                GCPoints = ct.GCPoints,
                StageNo = ct.StageNo,
                StagePosition = ct.StagePosition,
                StagePoints = ct.StagePoints,
                LeaderJerseyName = ct.LeaderJerseyName,
                LeaderJerseyPoints = ct.LeaderJerseyPoints,
                LeaderJerseyPosition = ct.LeaderJerseyPosition,
                Position = ct.Position,
                Points = ct.Points,
            }).ToList();
            return bikeRiderResults;
        }

        [HttpGet]
        public IList<VMBikeRaceDetail> GetBikeRaces(int year)
        {
            var bikeRaces = _unitOfWork.BikeRaces.GetBikeRaceDetails(year).Where(r => !r.Cancelled.HasValue || r.Cancelled == false).Select(br => new VMBikeRaceDetail(br)).OrderBy(br => br.StartDate).ToList();
            return bikeRaces;
        }

        [HttpGet]
        public VMBikeRaceCompetitionTeamResults GetBikeRaceCompetitionTeamResults(int bikeRaceId, int year)
        {
            var bikeRaceDetail = _unitOfWork.BikeRaces.GetBikeRaceDetail(bikeRaceId, year);
            VMBikeRaceCompetitionTeamResults vmBikeRaceCompetitionTeamResults = new VMBikeRaceCompetitionTeamResults(bikeRaceDetail.BikeRaceDetailId);
            return vmBikeRaceCompetitionTeamResults;
        }

        [HttpGet]
        public VMBikeRaceCLTeamResults GetBikeRaceCLTeamResults(int bikeRaceId, int year)
        {
            var bikeRaceDetail = _unitOfWork.BikeRaces.GetBikeRaceDetail(bikeRaceId, year);
            VMBikeRaceCLTeamResults vmBikeRaceCompetitionTeamResults = new VMBikeRaceCLTeamResults(bikeRaceDetail.BikeRaceDetailId);
            return vmBikeRaceCompetitionTeamResults;
        }

        [HttpGet]
        public IList<VMCompetitionTeamResults> GetCompetitionTeamResults_Monuments(int year)
        {
            return _unitOfWork.Results.GetCompetitionTeamResults_Monuments(year).Select(ct => new VMCompetitionTeamResults()
            {
                Position = ct.Position,
                CompetitionTeamId = ct.CompetitionTeamId,
                Name = ct.Name,
                TotalCQPoints = ct.TotalCQPoints,
                Note = ct.Note,
                Points = ct.Points,
                TeamIndex = ct.TeamIndex,
            }).ToList();
        }

        [HttpGet]
        public IList<VMCompetitionTeamBikeRiderResults> GetCompetitionTeamBikeRiderResults_Monuments(int competitionTeamId)
        {
            return _unitOfWork.Results.GetCompetitionTeamBikeRiderResults_Monuments(competitionTeamId).Select(ct => new VMCompetitionTeamBikeRiderResults()
            {
                CompetitionTeamId = ct.CompetitionTeamId,
                Name = ct.Name,
                Points = ct.Points,
                RiderIndex = ct.RiderIndex,
                BikeRiderId = ct.BikeRiderId,
                BikeRiderDetailId = ct.BikeRiderDetailId,
                BikeRiderName = ct.BikeRiderName,
                CQPoints = ct.CQPoints,
            }).ToList();
        }

        [HttpGet]
        public IList<VMBikeRiderResults> GetBikeRiderResults_Monuments(int bikeRiderDetailId, int year)
        {
            var bikeRiderResults = _unitOfWork.Results.GetBikeRiderResults_Monuments(bikeRiderDetailId, year).Select(ct => new VMBikeRiderResults()
            {
                BikeRaceId = ct.BikeRaceId,
                BikeRaceName = ct.BikeRaceName,
                BikeRiderId = ct.BikeRiderId,
                BikeRiderName = ct.BikeRiderName,
                GCPosition = ct.GCPosition,
                GCPoints = ct.GCPoints,
                StageNo = ct.StageNo,
                StagePosition = ct.StagePosition,
                StagePoints = ct.StagePoints,
            }).ToList();
            return bikeRiderResults;
        }

        [HttpGet]
        public IList<VMCompetitionTeamResults> GetCLTeamResults(int year)
        {
            return _unitOfWork.Results.GetCLTeamResults(year).Select(ct => new VMCompetitionTeamResults()
            {
                Position = ct.Position,
                CompetitionTeamId = ct.CompetitionTeamId,
                Name = ct.Name,
                TotalCQPoints = ct.TotalCQPoints,
                Note = ct.Note,
                Points = ct.Points,
                TeamIndex = ct.TeamIndex,
                Color = ct.Color,
            }).ToList();
        }

        [HttpGet]
        public IList<VMCompetitionTeamBikeRiderResults> GetCLTeamBikeRiderResults(int clTeamId)
        {
            return _unitOfWork.Results.GetCLTeamBikeRiderResults(clTeamId).Select(ct => new VMCompetitionTeamBikeRiderResults()
            {
                CompetitionTeamId = ct.CompetitionTeamId,
                Name = ct.Name,
                Points = ct.Points,
                RiderIndex = ct.RiderIndex,
                BikeRiderId = ct.BikeRiderId,
                BikeRiderDetailId = ct.BikeRiderDetailId,
                BikeRiderName = ct.BikeRiderName,
                CQPoints = ct.CQPoints,
            }).ToList();
        }

        [HttpGet]
        public IList<VMCompetitionTeamResults> GetLotteryTeamResults(int year)
        {
            return _unitOfWork.Results.GetLotteryTeamResults(year).Select(ct => new VMCompetitionTeamResults()
            {
                Position = ct.Position,
                CompetitionTeamId = ct.CompetitionTeamId,
                Name = ct.Name,
                TotalCQPoints = ct.TotalCQPoints,
                Note = ct.Note,
                Points = ct.Points,
                TeamIndex = ct.TeamIndex,
                Color = ct.Color,
            }).ToList();
        }

        [HttpGet]
        public IList<VMCompetitionTeamBikeRiderResults> GetLotteryTeamBikeRiderResults(int lotteryTeamId)
        {
            return _unitOfWork.Results.GetLotteryTeamBikeRiderResults(lotteryTeamId).Select(ct => new VMCompetitionTeamBikeRiderResults()
            {
                CompetitionTeamId = ct.CompetitionTeamId,
                Name = ct.Name,
                Points = ct.Points,
                RiderIndex = ct.RiderIndex,
                BikeRiderId = ct.BikeRiderId,
                BikeRiderDetailId = ct.BikeRiderDetailId,
                BikeRiderName = ct.BikeRiderName,
                CQPoints = ct.CQPoints,
            }).ToList();
        }

        [HttpGet]
        public IList<VMCompetitionTeamResults> GetYouthTeamResults(int year)
        {
            return _unitOfWork.Results.GetYouthTeamResults(year).Select(ct => new VMCompetitionTeamResults()
            {
                Position = ct.Position,
                CompetitionTeamId = ct.CompetitionTeamId,
                Name = ct.Name,
                TotalCQPoints = ct.TotalCQPoints,
                Note = ct.Note,
                Points = ct.Points,
                TeamIndex = ct.TeamIndex,
                Color = ct.Color,
            }).ToList();
        }

        [HttpGet]
        public IList<VMCompetitionTeamBikeRiderResults> GetYouthTeamBikeRiderResults(int youthTeamId)
        {
            return _unitOfWork.Results.GetYouthTeamBikeRiderResults(youthTeamId).Select(ct => new VMCompetitionTeamBikeRiderResults()
            {
                CompetitionTeamId = ct.CompetitionTeamId,
                Name = ct.Name,
                Points = ct.Points,
                RiderIndex = ct.RiderIndex,
                BikeRiderId = ct.BikeRiderId,
                BikeRiderDetailId = ct.BikeRiderDetailId,
                BikeRiderName = ct.BikeRiderName,
                CQPoints = ct.CQPoints,
            }).ToList();
        }
    }
}

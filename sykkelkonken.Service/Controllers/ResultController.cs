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
            return _unitOfWork.Results.GetCompetitionTeamResults(year).Select(ct => new VMCompetitionTeamResults()
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
        public IList<VMCompetitionTeamBikeRiderResults> GetCompetitionTeamBikeRiderResults(int competitionTeamId)
        {
            return _unitOfWork.Results.GetCompetitionTeamBikeRiderResults(competitionTeamId).Select(ct => new VMCompetitionTeamBikeRiderResults()
            {
                CompetitionTeamId = ct.CompetitionTeamId,
                Name = ct.Name,
                Points = ct.Points,
                RiderIndex = ct.RiderIndex,
                BikeRiderId = ct.BikeRiderId,
                BikeRiderName = ct.BikeRiderName,
                CQPoints = ct.CQPoints,
            }).ToList();
        }

        [HttpGet]
        public IList<VMBikeRiderResults> GetBikeRiderResults(int bikeRiderId, int year)
        {
            var bikeRiderResults = _unitOfWork.Results.GetBikeRiderResults(bikeRiderId, year).Select(ct => new VMBikeRiderResults()
            {
                BikeRaceId = ct.BikeRaceId,
                BikeRaceName = ct.BikeRaceName,
                Description = ct.Description,
                BikeRiderId = ct.BikeRiderId,
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
            var bikeRaces = _unitOfWork.BikeRaces.GetBikeRaceDetails(year).Select(br => new VMBikeRaceDetail(br)).OrderBy(br => br.StartDate).ToList();
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
                BikeRiderName = ct.BikeRiderName,
                CQPoints = ct.CQPoints,
            }).ToList();
        }

        [HttpGet]
        public IList<VMBikeRiderResults> GetBikeRiderResults_Monuments(int bikeRiderId, int year)
        {
            var bikeRiderResults = _unitOfWork.Results.GetBikeRiderResults_Monuments(bikeRiderId, year).Select(ct => new VMBikeRiderResults()
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
                BikeRiderName = ct.BikeRiderName,
                CQPoints = ct.CQPoints,
            }).ToList();
        }
    }
}

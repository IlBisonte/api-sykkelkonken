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
    public class StatsCompetitionTeamController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public StatsCompetitionTeamController()
        {
            _unitOfWork = new UnitOfWork();
        }

        [HttpGet]
        public IList<BRCompetitionTeam> Get(int year)
        {
            return _unitOfWork.CompetitionTeams.Get(year).Select(ct => new BRCompetitionTeam()
            {
                CompetitionTeamId = ct.CompetitionTeamId,
                Name = ct.Name,
            }).ToList();
        }

        [HttpGet]
        public IList<VMCompetitionTeamStatsAllTime> GetCompetitionTeamStatsAllTime()
        {
            return _unitOfWork.Stats.GetCompetitionTeamStatsAllTime().OrderByDescending(s => s.Points).ToList();
        }

        [HttpGet]
        public IList<VMChampionsLeagueTeamStatsAllTime> GetChampionsLeagueTeamStatsAllTime()
        {
            return _unitOfWork.Stats.GetChampionsLeagueTeamStatsAllTime().OrderByDescending(s => s.Points).ToList();
        }

        [HttpGet]
        public IList<VMBikeRaceRanking> GetCompetitionTeamResultPerBikeRace(int year)
        {
            List<VMBikeRaceRanking> bikeRaceRankings = new List<VMBikeRaceRanking>();
            var results = _unitOfWork.Stats.GetCompetitionTeamResultPerBikeRace(year);
            var bikeraces = _unitOfWork.BikeRaces.GetBikeRaceDetails(year);
            var bikeraceSeasonPlacement = _unitOfWork.BikeRaces.GetBikeRaceSeasonPlacements(year).OrderBy(b => b.BikeRaceDetail.FinishDate).ThenBy(b => b.BikeRaceDetail.StartDate);
            var competitionTeams = _unitOfWork.CompetitionTeams.Get(year);
            List<BRCompetitionTeam> lstCompetitionTeamTotals = new List<BRCompetitionTeam>();
            foreach (var bikerace in bikeraces.OrderBy(br => br.FinishDate).ThenBy(br => br.StartDate))
            {
                string bikeRaceName = bikerace.BikeRaceNameShort != null ? bikerace.BikeRaceNameShort : bikerace.Name;
                VMBikeRaceRanking brRanking = new VMBikeRaceRanking(bikerace.BikeRaceDetailId, bikeRaceName, year);
                foreach (var compTeam in competitionTeams)
                {
                    BRCompetitionTeam brCompTeam = new BRCompetitionTeam();
                    if (lstCompetitionTeamTotals.Count(ct => ct.CompetitionTeamId == compTeam.CompetitionTeamId) > 0)
                    {
                        brCompTeam = lstCompetitionTeamTotals.FirstOrDefault(ct => ct.CompetitionTeamId == compTeam.CompetitionTeamId);
                    }
                    else
                    {
                        brCompTeam.CompetitionTeamId = compTeam.CompetitionTeamId;
                        brCompTeam.Name = compTeam.Name;
                        lstCompetitionTeamTotals.Add(brCompTeam);
                    }
                    var brResults = results.Where(r => r.BikeRaceDetailId == bikerace.BikeRaceDetailId && r.CompetitionTeamId == compTeam.CompetitionTeamId);
                    if (brResults != null)
                    {
                        foreach (var result in brResults.OrderByDescending(r => r.TotalPoints))
                        {
                            brCompTeam.Points += result.TotalPoints;
                        }
                    }
                    brRanking.CompetitionTeams.Add(new BRCompetitionTeam()
                    {
                        CompetitionTeamId = brCompTeam.CompetitionTeamId,
                        Name = brCompTeam.Name,
                        Points = brCompTeam.Points,
                    });
                }
                int rank = 1;
                foreach (var compTeam in brRanking.CompetitionTeams.OrderByDescending(ct => ct.Points))
                {
                    compTeam.Ranking = rank;
                    rank++;
                }

                if (bikeraceSeasonPlacement.Any(br => br.BikeRaceDetailId == bikerace.BikeRaceDetailId))
                {
                    bikeRaceRankings.Add(brRanking);
                }
            }

            return bikeRaceRankings;
        }

        [HttpGet]
        public IList<VmNoOfVictoriesByCompTeam> GetNoOfVictoriesCompTeams(int year)
        {
            List<VmNoOfVictoriesByCompTeam> lstNoOfVictories = new List<VmNoOfVictoriesByCompTeam>();
            var results = _unitOfWork.Stats.GetCompetitionTeamResultPerBikeRace(year);
            var stageresults = _unitOfWork.Stats.GetCompetitionTeamPointsPerStage(year);
            var bikeraces = _unitOfWork.BikeRaces.GetBikeRaceDetails(year).OrderBy(b => b.FinishDate);
            var competitionTeams = _unitOfWork.CompetitionTeams.Get(year);
            competitionTeams = competitionTeams.Where(ct => !ct.Name.ToLower().Equals("folkerekken")).ToList();
            lstNoOfVictories = competitionTeams.Select(ct => new VmNoOfVictoriesByCompTeam()
            {
                CompetitionTeamId = ct.CompetitionTeamId,
                Name = ct.Name,
                NoOfVictories = 0,
                BikeRacesWon = new List<string>()
            }).ToList();
            foreach (var bikerace in bikeraces)
            {
                VMBikeRaceRanking brRanking = new VMBikeRaceRanking(bikerace.BikeRaceDetailId, bikerace.BikeRaceNameShort, year);
                var brResults = results.Where(r => r.BikeRaceDetailId == bikerace.BikeRaceDetailId);
                if (brResults != null && brResults.Count() > 0)
                {
                    //1st place
                    var firstPlacePoints = brResults.Max(r => r.TotalPoints);
                    var victoryResults = brResults.Where(r => r.TotalPoints == firstPlacePoints).ToList();
                    int noOfVictoryResults = victoryResults.Count;
                    foreach (var result in victoryResults)
                    {
                        var victoryTeam = lstNoOfVictories.SingleOrDefault(t => t.CompetitionTeamId == result.CompetitionTeamId);
                        victoryTeam.NoOfVictories++;
                        string bikeRaceName = bikerace.BikeRaceNameShort != null ? bikerace.BikeRaceNameShort.Trim() : bikerace.Name.Trim();
                        victoryTeam.BikeRacesWon.Add(bikeRaceName);
                    }
                    if (noOfVictoryResults < 3)
                    {
                        if (brResults.Count() >= 2)
                        {
                            if (brResults.Where(r => r.TotalPoints < firstPlacePoints).Count() > 0)
                            {
                                //2nd place
                                var secondPlacePoints = brResults.Where(r => r.TotalPoints < firstPlacePoints).Max(x => x.TotalPoints);
                                var secondPlaceResult = brResults.Where(r => r.TotalPoints == secondPlacePoints).ToList();
                                noOfVictoryResults = noOfVictoryResults + secondPlaceResult.Count;
                                foreach (var result in secondPlaceResult)
                                {
                                    var secondPlaceTeam = lstNoOfVictories.SingleOrDefault(t => t.CompetitionTeamId == result.CompetitionTeamId);
                                    secondPlaceTeam.NoOfSecondPlace++;
                                }
                                if (noOfVictoryResults < 3)
                                {
                                    if (brResults.Count() >= 3)
                                    {
                                        if (brResults.Where(r => r.TotalPoints < secondPlacePoints).Count() > 0)
                                        {
                                            var thirdPlacePoints = brResults.Where(r => r.TotalPoints < secondPlacePoints).Max(x => x.TotalPoints);
                                            var thirdPlaceResult = brResults.Where(r => r.TotalPoints == thirdPlacePoints).ToList();
                                            foreach (var result in thirdPlaceResult)
                                            {
                                                var thirdPlaceTeam = lstNoOfVictories.SingleOrDefault(t => t.CompetitionTeamId == result.CompetitionTeamId);
                                                thirdPlaceTeam.NoOfThirdPlace++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                var stagesBikeRace = stageresults.Where(s => s.BikeRaceDetailId == bikerace.BikeRaceDetailId).ToList();
                if (stagesBikeRace.Count > 0)
                {
                    foreach (var stageResult in stagesBikeRace.OrderBy(s => s.StageNo).GroupBy(s => s.StageNo))
                    {
                        int stageWinnerPoints = stageResult.Max(s => s.StagePoints);
                        var stageWinnerResult = stageResult.Where(s => s.StagePoints == stageWinnerPoints);
                        foreach (var result in stageWinnerResult)
                        {
                            var stageWinnerTeam = lstNoOfVictories.SingleOrDefault(t => t.CompetitionTeamId == result.CompetitionTeamId);
                            stageWinnerTeam.NoOfStageWins++;
                        }
                    }
                }
            }

            return lstNoOfVictories;
        }

        [HttpGet]
        public VMCompetitionTeamPointsByBikeRaceCategory GetCompetitionTeamPointsByBikeRaceCategory(int year, int competitionTeamId)
        {
            var pointsByBikeRaceCategory = _unitOfWork.Stats.GetCompetitionTeamPointsByBikeRaceCategory(year, competitionTeamId).OrderByDescending(s => s.Points).ToList();
            VMCompetitionTeamPointsByBikeRaceCategory vmCompetitionTeamPointsByBikeRaceCategory = new VMCompetitionTeamPointsByBikeRaceCategory(competitionTeamId, pointsByBikeRaceCategory);
            return vmCompetitionTeamPointsByBikeRaceCategory;
        }

        [HttpGet]
        public VMCompetitionTeamPointsByBikeRace GetCompetitionTeamPointsByOneDayRaces(int year, int competitionTeamId)
        {
            var pointsByBikeRace = _unitOfWork.Stats.GetCompetitionTeamPointsByCompetitionTeam(year, competitionTeamId).OrderByDescending(s => s.Points).ToList();
            VMCompetitionTeamPointsByBikeRace vmCompetitionTeamPointsByBikeRaceCategory = new VMCompetitionTeamPointsByBikeRace(competitionTeamId,
                pointsByBikeRace.Where(br => br.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.OneDay ||
                                                     br.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.Monument).ToList());
            return vmCompetitionTeamPointsByBikeRaceCategory;
        }

        [HttpGet]
        public VMCompetitionTeamPointsByBikeRace GetCompetitionTeamPointsByStageRaces(int year, int competitionTeamId)
        {
            var pointsByBikeRace = _unitOfWork.Stats.GetCompetitionTeamPointsByCompetitionTeam(year, competitionTeamId).OrderByDescending(s => s.Points).ToList();
            VMCompetitionTeamPointsByBikeRace vmCompetitionTeamPointsByBikeRaceCategory = new VMCompetitionTeamPointsByBikeRace(competitionTeamId,
                pointsByBikeRace.Where(br => br.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.StageRace ||
                                             br.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.GiroVuelta ||
                                             br.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.TourDeFrance).ToList());
            return vmCompetitionTeamPointsByBikeRaceCategory;
        }

        [HttpGet]
        public VMCompetitionTeamPointsByBikeRace GetCompetitionTeamPointsByOtherOneDayRaces(int year, int competitionTeamId)
        {
            var pointsByBikeRace = _unitOfWork.Stats.GetCompetitionTeamPointsByCompetitionTeam(year, competitionTeamId).OrderByDescending(s => s.Points).ToList();
            VMCompetitionTeamPointsByBikeRace vmCompetitionTeamPointsByBikeRaceCategory = new VMCompetitionTeamPointsByBikeRace(competitionTeamId,
                pointsByBikeRace.Where(br => br.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.OneDay).ToList());
            return vmCompetitionTeamPointsByBikeRaceCategory;
        }

        [HttpGet]
        public VMCompetitionTeamPointsByBikeRace GetCompetitionTeamPointsByMonuments(int year, int competitionTeamId)
        {
            var pointsByBikeRace = _unitOfWork.Stats.GetCompetitionTeamPointsByCompetitionTeam(year, competitionTeamId).OrderByDescending(s => s.Points).ToList();
            VMCompetitionTeamPointsByBikeRace vmCompetitionTeamPointsByBikeRaceCategory = new VMCompetitionTeamPointsByBikeRace(competitionTeamId,
                pointsByBikeRace.Where(br => br.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.Monument).ToList());
            return vmCompetitionTeamPointsByBikeRaceCategory;
        }

        [HttpGet]
        public VMCompetitionTeamPointsByBikeRace GetCompetitionTeamPointsByOtherStageRaces(int year, int competitionTeamId)
        {
            var pointsByBikeRace = _unitOfWork.Stats.GetCompetitionTeamPointsByCompetitionTeam(year, competitionTeamId).OrderByDescending(s => s.Points).ToList();
            VMCompetitionTeamPointsByBikeRace vmCompetitionTeamPointsByBikeRaceCategory = new VMCompetitionTeamPointsByBikeRace(competitionTeamId,
                pointsByBikeRace.Where(br => br.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.StageRace).ToList());
            return vmCompetitionTeamPointsByBikeRaceCategory;
        }

        [HttpGet]
        public VMCompetitionTeamPointsByBikeRace GetCompetitionTeamPointsByGTs(int year, int competitionTeamId)
        {
            var pointsByBikeRace = _unitOfWork.Stats.GetCompetitionTeamPointsByCompetitionTeam(year, competitionTeamId).OrderByDescending(s => s.Points).ToList();
            VMCompetitionTeamPointsByBikeRace vmCompetitionTeamPointsByBikeRaceCategory = new VMCompetitionTeamPointsByBikeRace(competitionTeamId,
                pointsByBikeRace.Where(br => br.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.GiroVuelta ||
                                             br.BikeRaceCategoryId == (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.TourDeFrance).ToList());
            return vmCompetitionTeamPointsByBikeRaceCategory;
        }

        [HttpGet]
        public VMBikeRiderPointsByBikeRace GetBikeRiderPointsByBikeRace(int year, int competitionTeamId, string bikeRaceName)
        {
            var bikeRace = _unitOfWork.BikeRaces.GetBikeRaceDetails(year).Where(br => br.BikeRace.Name.ToLower().Contains(bikeRaceName.ToLower())).FirstOrDefault();
            if (bikeRace != null)
            {
                var pointsByBikeRace = _unitOfWork.Stats.GetBikeRiderPointsByBikeRace(year, competitionTeamId, bikeRace.BikeRaceDetailId).OrderByDescending(s => s.Points).ToList();
                VMBikeRiderPointsByBikeRace vmBikeRiderPointsByBikeRace = new VMBikeRiderPointsByBikeRace(bikeRace.BikeRaceDetailId, bikeRaceName, pointsByBikeRace);
                return vmBikeRiderPointsByBikeRace;
            }
            else
            {
                return new VMBikeRiderPointsByBikeRace();
            }
        }

        [HttpGet]
        public VMBikeRacePointsByBikeRider GetBikeRacePointsByBikeRider(int year, string bikeRiderName)
        {
            var bikeRider = _unitOfWork.BikeRiders.GetBikeRiderDetailByName(bikeRiderName, year);
            if (bikeRider != null)
            {
                var pointsByBikeRace = _unitOfWork.Stats.GetBikeRacePointsByBikeRider(year, bikeRider.BikeRiderDetailId).OrderByDescending(s => s.Points).ToList();
                VMBikeRacePointsByBikeRider vmBikeRacePointsByBikeRider = new VMBikeRacePointsByBikeRider(bikeRider.BikeRiderDetailId, bikeRiderName, pointsByBikeRace);
                return vmBikeRacePointsByBikeRider;
            }
            else
            {
                return new VMBikeRacePointsByBikeRider();
            }
        }

        [HttpGet]
        public VMBikeRacePointsByBikeRider GetBikeRacePointsByBikeRider(int year, int bikeRiderDetailId)
        {
            var bikeRider = _unitOfWork.BikeRiders.GetBikeRiderDetail(bikeRiderDetailId);
            if (bikeRider != null)
            {
                var pointsByBikeRace = _unitOfWork.Stats.GetBikeRacePointsByBikeRider(year, bikeRider.BikeRiderDetailId).OrderByDescending(s => s.Points).ToList();
                VMBikeRacePointsByBikeRider vmBikeRacePointsByBikeRider = new VMBikeRacePointsByBikeRider(bikeRider.BikeRiderDetailId, bikeRider.BikeRider.BikeRiderName, pointsByBikeRace);
                return vmBikeRacePointsByBikeRider;
            }
            else
            {
                return new VMBikeRacePointsByBikeRider();
            }
        }

        [HttpGet]
        public VMBikeRiderPointsByCompetitionTeam GetBikeRiderPointsByCompetitionTeam(int year, int competitionTeamId)
        {
            var competitionTeam = _unitOfWork.CompetitionTeams.GetByCompetitionTeamId(competitionTeamId);
            if (competitionTeam != null)
            {
                var pointsByBikeRace = _unitOfWork.Stats.GetBikeRiderPointsByCompetitionTeam(year, competitionTeamId).OrderByDescending(s => s.Points).ToList();
                VMBikeRiderPointsByCompetitionTeam vmBikeRacePointsByBikeRider = new VMBikeRiderPointsByCompetitionTeam(competitionTeam.CompetitionTeamId, competitionTeam.Name, pointsByBikeRace);
                return vmBikeRacePointsByBikeRider;
            }
            else
            {
                return new VMBikeRiderPointsByCompetitionTeam();
            }
        }
    }
}
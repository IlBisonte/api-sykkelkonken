using sykkelkonken.Data;
using sykkelkonken.Service.Filters;
using sykkelkonken.Service.Models;
using sykkelkonken.Service.Persistence;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace sykkelkonken.Service.Controllers
{
    public class BikeRaceController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public BikeRaceController()
        {
            _unitOfWork = new UnitOfWork();
        }

        [HttpGet]
        public IList<VMBikeRaceDetail> Get(int year, bool showCancelled)
        {
            IList<VMBikeRaceDetail> lstVMBikeRaces = new List<VMBikeRaceDetail>();
            var bikeRaces = _unitOfWork.BikeRaces.GetBikeRaceDetails(year).ToList();
            foreach (var bikeRace in bikeRaces)
            {
                if ((bikeRace.Cancelled ?? false) && !showCancelled)
                {
                    continue;
                }
                VMBikeRaceDetail vmBikeRace = new VMBikeRaceDetail(bikeRace);
                if (vmBikeRace.IsCalculated)
                {
                    bikeRace.IsCalculated = true;
                    _unitOfWork.Complete();
                }
                lstVMBikeRaces.Add(vmBikeRace);
            }
            return lstVMBikeRaces.OrderBy(br => br.StartDate).ToList();
        }

        [JwtAuthentication]
        [HttpPost]
        public IHttpActionResult Add(VMBikeRaceDetail bikeRace, string startDate, string finishDate)
        {
            try
            {

                DateTime dtStart = new DateTime();
                DateTime.TryParseExact(startDate, "dd'.'MM'.'yyyy",
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None,
                           out dtStart);
                DateTime dtFinish = new DateTime();
                if (finishDate != null && finishDate.Length > 0)
                {
                    DateTime.TryParseExact(finishDate, "dd'.'MM'.'yyyy",
                               CultureInfo.InvariantCulture,
                               DateTimeStyles.None,
                               out dtFinish);
                }
                TimeSpan tsDate = bikeRace.FinishDate - bikeRace.StartDate;
                int bikeRaceCategoryId = bikeRace.IsMonument ? (int)BikeRaceCategory.BikeRaceCategoryIdEnum.Monument : tsDate.TotalDays > 0 ? (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.StageRace : (int)BikeRaceCategory.BikeRaceCategoryIdEnum.OneDay;
                var modelBikeRace = _unitOfWork.BikeRaces.GetBikeRace(bikeRace.Name);
                if (modelBikeRace == null)
                {
                    modelBikeRace = new BikeRace()
                    {
                        Name = bikeRace.Name,
                        CountryName = bikeRace.CountryName,
                    };
                    _unitOfWork.BikeRaces.AddBikeRace(modelBikeRace);
                }
                BikeRaceDetail modelBikeRaceDetail = new BikeRaceDetail()
                {
                    BikeRaceId = modelBikeRace.BikeRaceId,
                    Name = modelBikeRace.Name,
                    CountryName = modelBikeRace.CountryName,
                    Year = bikeRace.Year,
                    StartDate = dtStart,
                    FinishDate = dtFinish != new DateTime() ? dtFinish : dtStart,
                    NoOfStages = bikeRace.NoOfStages > 0 ? bikeRace.NoOfStages : 0,
                    BikeRaceCategoryId = bikeRaceCategoryId,
                    Cancelled = bikeRace.Cancelled,

                };
                _unitOfWork.BikeRaces.AddBikeRaceDetail(modelBikeRaceDetail);
                _unitOfWork.Complete();
            }
            catch (Exception)
            {
            }
            return Ok();
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult Update(VMBikeRaceDetail bikeRace)
        {
            BikeRace modelBikeRace = _unitOfWork.BikeRaces.GetBikeRace(bikeRace.BikeRaceId);
            if (modelBikeRace != null)
            {
                modelBikeRace.Name = bikeRace.Name;
                modelBikeRace.CountryName = bikeRace.CountryName;

                BikeRaceDetail modelBikeRaceDetail = _unitOfWork.BikeRaces.GetBikeRaceDetail(bikeRace.BikeRaceDetailId);

                if (modelBikeRaceDetail != null)
                {
                    TimeSpan tsDate = bikeRace.FinishDate - bikeRace.StartDate;
                    int bikeRaceCategoryId = bikeRace.IsMonument ? (int)BikeRaceCategory.BikeRaceCategoryIdEnum.Monument : tsDate.TotalDays > 0 ? (int)Data.BikeRaceCategory.BikeRaceCategoryIdEnum.StageRace : (int)BikeRaceCategory.BikeRaceCategoryIdEnum.OneDay;
                    if (bikeRace.NoOfStages == 21 && bikeRace.Name.Contains("France"))
                    {
                        bikeRaceCategoryId = (int)BikeRaceCategory.BikeRaceCategoryIdEnum.TourDeFrance;
                    }
                    else if (bikeRace.NoOfStages > 15)
                    {
                        bikeRaceCategoryId = (int)BikeRaceCategory.BikeRaceCategoryIdEnum.GiroVuelta;
                    }
                    modelBikeRaceDetail.StartDate = bikeRace.StartDate;
                    modelBikeRaceDetail.FinishDate = bikeRace.FinishDate;
                    modelBikeRaceDetail.NoOfStages = bikeRace.NoOfStages;
                    modelBikeRaceDetail.BikeRaceCategoryId = bikeRaceCategoryId;
                    modelBikeRaceDetail.Cancelled = bikeRace.Cancelled;

                }
                _unitOfWork.Complete();
            }
            return Ok();
        }

        [JwtAuthentication]
        [HttpDelete]
        public IHttpActionResult Delete(int bikeRaceDetailId)
        {
            BikeRaceDetail modelBikeRace = _unitOfWork.BikeRaces.GetBikeRaceDetail(bikeRaceDetailId);

            if (modelBikeRace != null)
            {
                _unitOfWork.BikeRaces.DeleteBikeRaceDetail(modelBikeRace);
                _unitOfWork.Complete();
            }
            return Ok();
        }

        [HttpGet]
        public IList<VMBikeRaceResult> GetResultsToCalculate(int bikeRaceDetailId)
        {
            BikeRaceDetail bikeRaceDetail = _unitOfWork.BikeRaces.GetBikeRaceDetail(bikeRaceDetailId);
            IList<VMBikeRaceResult> lstBikeRaceResults = new List<VMBikeRaceResult>();
            if (bikeRaceDetail != null)
            {
                foreach (var brCatPoints in bikeRaceDetail.BikeRaceCategory.BikeRaceCategoryPoints.OrderBy(brcp => brcp.Position))
                {
                    var bikeRaceResult = bikeRaceDetail.BikeRaceResults.Where(brr => brr.Position == brCatPoints.Position).SingleOrDefault();
                    if (bikeRaceResult != null)
                    {
                        lstBikeRaceResults.Add(new VMBikeRaceResult()
                        {
                            BikeRaceDetailId = bikeRaceResult.BikeRaceDetailId,
                            BikeRaceId = bikeRaceResult.BikeRaceDetail.BikeRaceId,
                            BikeRiderId = bikeRaceResult.BikeRiderId > 0 ? bikeRaceResult.BikeRiderId : -1,
                            BikeRiderName = bikeRaceResult.BikeRiderId > 0 ? bikeRaceResult.BikeRider.BikeRiderName : "",
                            Position = bikeRaceResult.Position,
                            Points = brCatPoints.Points,
                        });
                    }
                    else
                    {
                        lstBikeRaceResults.Add(new VMBikeRaceResult()
                        {
                            BikeRaceDetailId = bikeRaceDetail.BikeRaceDetailId,
                            BikeRaceId = bikeRaceDetail.BikeRaceId,
                            BikeRiderId = -1,
                            BikeRiderName = "",
                            Position = brCatPoints.Position,
                            Points = brCatPoints.Points,
                        });
                    }
                }
            }

            return lstBikeRaceResults.OrderBy(brr => brr.Position).ToList();
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult UpdateBikeRaceResultBikeRider(VMBikeRaceResult bikeRaceResult)
        {
            _unitOfWork.BikeRaces.UpdateBikeRaceResult(bikeRaceResult.BikeRaceDetailId, bikeRaceResult.BikeRiderId, bikeRaceResult.Position);
            _unitOfWork.Complete();
            return Ok();
        }

        [HttpGet]
        public IList<VMStageResult> GetStageResults(int bikeRaceDetailId)
        {
            BikeRaceDetail bikeRaceDetail = _unitOfWork.BikeRaces.GetBikeRaceDetail(bikeRaceDetailId);
            IList<VMStageResult> lstStages = new List<VMStageResult>();
            if (bikeRaceDetail != null)
            {
                try
                {
                    for (int i = 1; i <= bikeRaceDetail.NoOfStages; i++)
                    {
                        //VMStage vmStage = new VMStage()
                        //{
                        //    BikeRaceId = bikeRaceId,
                        //    StageNo = i,
                        //    StageResults = new List<VMStageResult>(),
                        //};
                        if (bikeRaceDetail.StageResults != null)
                        {
                            foreach (var brCatStagePoints in bikeRaceDetail.BikeRaceCategory.StagePointss.OrderBy(brcp => brcp.StagePosition))
                            {
                                var stageresult = bikeRaceDetail.StageResults.SingleOrDefault(s => s.StageNo == i && s.StagePosition == brCatStagePoints.StagePosition);
                                if (stageresult != null && stageresult.BikeRiderId > 0)
                                {
                                    lstStages.Add(new VMStageResult()
                                    {
                                        BikeRaceDetailId = bikeRaceDetail.BikeRaceDetailId,
                                        BikeRaceId = bikeRaceDetail.BikeRaceId,
                                        StageNo = i,
                                        StagePosition = brCatStagePoints.StagePosition,
                                        BikeRiderId = stageresult.BikeRiderId,
                                        BikeRiderName = stageresult.BikeRider.BikeRiderName,
                                        Points = brCatStagePoints.Points
                                    });
                                }
                                else
                                {
                                    lstStages.Add(new VMStageResult()
                                    {
                                        BikeRaceDetailId = bikeRaceDetail.BikeRaceDetailId,
                                        BikeRaceId = bikeRaceDetail.BikeRaceId,
                                        StageNo = i,
                                        StagePosition = brCatStagePoints.StagePosition,
                                        BikeRiderId = -1,
                                        BikeRiderName = "",
                                        Points = brCatStagePoints.Points
                                    });
                                }
                            }
                        }

                        //lstStages.Add(vmStage);
                    }
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                    {

                    }
                }
            }

            return lstStages;
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult UpdateStageResult(VMStageResult stageResult)
        {
            _unitOfWork.BikeRaces.UpdateStageResult(stageResult.BikeRaceDetailId, stageResult.StageNo, stageResult.StagePosition, stageResult.BikeRiderId);
            _unitOfWork.Complete();
            return Ok();
        }

        [HttpGet]
        public IList<VMLeaderJerseyResult> GetLeaderJerseyResultsToCalculate(int bikeRaceDetailId)
        {
            BikeRaceDetail bikeRaceDetail = _unitOfWork.BikeRaces.GetBikeRaceDetail(bikeRaceDetailId);
            IList<VMLeaderJerseyResult> lstLeaderJerseyResults = new List<VMLeaderJerseyResult>();
            if (bikeRaceDetail != null)
            {
                foreach (var leaderJersey in bikeRaceDetail.BikeRaceCategory.LeaderJersey)
                {
                    foreach (var leaderJerseyPoints in leaderJersey.LeaderJerseyPoints)
                    {
                        var leaderJerseyResult = bikeRaceDetail.LeaderJerseyResults.Where(brr => brr.LeaderJerseyPosition == leaderJerseyPoints.Position && brr.LeaderJerseyId == leaderJerseyPoints.LeaderJerseyId).SingleOrDefault();
                        if (leaderJerseyResult != null)
                        {
                            lstLeaderJerseyResults.Add(new VMLeaderJerseyResult()
                            {
                                BikeRaceDetailId = leaderJerseyResult.BikeRaceDetail.BikeRaceDetailId,
                                BikeRaceId = leaderJerseyResult.BikeRaceDetail.BikeRaceId,
                                BikeRiderId = leaderJerseyResult.BikeRiderId > 0 ? leaderJerseyResult.BikeRiderId : -1,
                                BikeRiderName = leaderJerseyResult.BikeRiderId > 0 ? leaderJerseyResult.BikeRider.BikeRiderName : "",
                                LeaderJerseyPosition = leaderJerseyResult.LeaderJerseyPosition,
                                Points = leaderJerseyPoints.Points ?? 0,
                                BikeRaceCategoryId = leaderJersey.BikeRaceCategoryId,
                                LeaderJerseyId = leaderJersey.LeaderJerseyId,
                                LeaderJerseyName = leaderJersey.Name,
                            });
                        }
                        else
                        {
                            lstLeaderJerseyResults.Add(new VMLeaderJerseyResult()
                            {
                                BikeRaceDetailId = bikeRaceDetail.BikeRaceDetailId,
                                BikeRaceId = bikeRaceDetail.BikeRaceId,
                                BikeRiderId = -1,
                                BikeRiderName = "",
                                LeaderJerseyPosition = leaderJerseyPoints.Position,
                                Points = leaderJerseyPoints.Points ?? 0,
                                BikeRaceCategoryId = leaderJersey.BikeRaceCategoryId,
                                LeaderJerseyId = leaderJersey.LeaderJerseyId,
                                LeaderJerseyName = leaderJersey.Name,
                            });
                        }
                    }

                }
            }

            return lstLeaderJerseyResults.OrderBy(brr => brr.LeaderJerseyId).ThenByDescending(ljr => ljr.Points).ToList();
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult UpdateLeaderJerseyResult(VMLeaderJerseyResult leaderJerseyResult)
        {
            _unitOfWork.BikeRaces.UpdateLeaderJerseyResult(leaderJerseyResult.BikeRaceDetailId, leaderJerseyResult.BikeRiderId, leaderJerseyResult.LeaderJerseyPosition, leaderJerseyResult.LeaderJerseyId);
            _unitOfWork.Complete();
            return Ok();
        }

        [JwtAuthentication]
        [HttpGet]
        public IList<VMBikeRaceSeasonPlacement> GetBikeRaceSeasonPlacement(int year)
        {
            IList<VMBikeRaceSeasonPlacement> lstVMBikeRaces = new List<VMBikeRaceSeasonPlacement>();
            var bikeRaces = _unitOfWork.BikeRaces.GetBikeRaceSeasonPlacements(year).ToList();
            foreach (var bikeRace in bikeRaces.OrderBy(br => br.BikeRaceDetail.StartDate))
            {
                if (bikeRace.BikeRaceDetail == null)
                {
                    bikeRace.BikeRaceDetail = _unitOfWork.BikeRaces.GetBikeRaceDetail(bikeRace.BikeRaceDetailId);
                }
                VMBikeRaceSeasonPlacement vmBikeRace = new VMBikeRaceSeasonPlacement()
                {
                    BikeRaceSeasonPlacementId = bikeRace.BikeRaceSeasonPlacementId,
                    BikeRaceDetailId = bikeRace.BikeRaceDetailId,
                    Year = year,
                    Name = bikeRace.BikeRaceDetail != null ? bikeRace.BikeRaceDetail.Name : "Fikk ikke hentet navn",
                };
                lstVMBikeRaces.Add(vmBikeRace);
            }

            return lstVMBikeRaces;
        }

        [JwtAuthentication]
        [HttpPost, HttpGet]
        public IHttpActionResult UpdateBikeRaceSeasonPlacement(int year, [FromUri] int[] bikeRaceDetailIds)
        {
            try
            {
                var bikeRaces = _unitOfWork.BikeRaces.GetBikeRaceSeasonPlacements(year).ToList();
                List<int> bikeRaceSeasonPlacementBikeRaceDetailIds = bikeRaces.Select(br => br.BikeRaceDetailId).ToList();
                var bikeRacesToAdd = bikeRaceDetailIds.Where(brdid => !bikeRaceSeasonPlacementBikeRaceDetailIds.Contains(brdid));
                foreach (var brdIdToAdd in bikeRacesToAdd)
                {
                    this._unitOfWork.BikeRaces.AddBikeRaceSeasonPlacement(new BikeRaceSeasonPlacement()
                    {
                        BikeRaceDetailId = brdIdToAdd,
                        Year = year,
                    });
                }
                foreach (var bikeRace in bikeRaces)
                {
                    if (!bikeRaceDetailIds.Contains(bikeRace.BikeRaceDetailId))
                    {
                        this._unitOfWork.BikeRaces.RemoveBikeRaceSeasonPlacement(bikeRace);
                    }
                }
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
            }

            return Ok();
        }
    }
}

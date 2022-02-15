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
    public class StatsController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public StatsController()
        {
            _unitOfWork = new UnitOfWork();
        }

        [HttpGet]
        public IList<VMBikeRiderStats> GetBikeRiderStats(int year)
        {
            return _unitOfWork.Stats.GetBikeRiderStats(year).ToList();
        }

        [HttpGet]
        public IList<VMBikeRiderScoreAllTime> GetBikeRiderScoreAllTime()
        {
            return _unitOfWork.Stats.GetBikeRiderScoreAllTime().OrderByDescending(s => s.Points).ToList();
        }

        [HttpGet]
        public IList<string> GetCompTeamsWithSelectedBikeRider(int bikeRiderDetailId)
        {
            var compTeams = _unitOfWork.Stats.GetCompTeamsWithSelectedBikeRider(bikeRiderDetailId).ToList();
            return compTeams;
        }
    }
}

using sykkelkonken.Service.Filters;
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
    public class BikeRiderController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public BikeRiderController()
        {
            _unitOfWork = new UnitOfWork();
        }

        [HttpGet]
        public IEnumerable<VMBikeRider> GetByIds(string bikeRiderIds)
        {
            if (bikeRiderIds != null)
            {
                string[] str_arr = bikeRiderIds.Split(',').ToArray();

                int[] brIds = Array.ConvertAll(str_arr, Int32.Parse);
                var bikeRiders = _unitOfWork.BikeRiders.Get(brIds);

                return bikeRiders.Select(br => new VMBikeRider()
                {
                    BikeRiderId = br.BikeRiderId,
                    BikeRiderName = br.BikeRiderName,
                    BikeTeamCode = br.BikeTeamCode,
                    BikeTeamName = br.BikeTeamName,
                    CQPoints = br.CQPoints,
                    Nationality = br.Nationality,
                });
            }
            return new List<VMBikeRider>();
        }


        [HttpGet]
        public VMBikeRider Get(int id)
        {
            var br = _unitOfWork.BikeRiders.Get(id);

            if (br == null)
            {
                return new VMBikeRider();
            }
            return new VMBikeRider()
            {
                BikeRiderId = br.BikeRiderId,
                BikeRiderName = br.BikeRiderName,
                BikeTeamCode = br.BikeTeamCode,
                BikeTeamName = br.BikeTeamName,
                CQPoints = br.CQPoints,
                Nationality = br.Nationality,
            };
        }

        [HttpGet]
        public IEnumerable<VMBikeRider> GetBySearchText(string searchtext)
        {
            var bikeRiders = _unitOfWork.BikeRiders.GetBySearchText(searchtext);

            return bikeRiders.Select(br => new VMBikeRider()
            {
                BikeRiderId = br.BikeRiderId,
                BikeRiderName = br.BikeRiderName,
                BikeTeamCode = br.BikeTeamCode,
                BikeTeamName = br.BikeTeamName,
                CQPoints = br.CQPoints,
                Nationality = br.Nationality,
            });
        }
    }
}

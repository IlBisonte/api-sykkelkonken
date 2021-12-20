using sykkelkonken.Data;
using sykkelkonken.Service.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models
{
    public class VMBikeRaceResults
    {
        private IUnitOfWork unitOfWork;
        public int BikeRaceId { get; set; }

        public string Name { get; set; }

        public IList<VMBikeRaceResult> LstResults { get; set; }

        public VMBikeRaceResults(int bikeRaceId, int year)
        {
            unitOfWork = new UnitOfWork();

            BikeRaceDetail bikeRace = unitOfWork.BikeRaces.GetBikeRaceDetail(bikeRaceId, year);
            if (bikeRace != null)
            {
                //bikeRace.
            }

        }
    }
}
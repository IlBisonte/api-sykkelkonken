using sykkelkonken.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Service.Persistence
{
    public interface IBikeRiderRepository
    {
        void DeleteAllBikeRiders();
        void AddBikeRidersInBulk(System.Data.DataTable dtBikeRiders, string connectionstring);
        BikeRider GetBikeRiderByName(string name);
        BikeRider GetBikeRiderByNameNationality(string name, string nationality);
        BikeRiderDetail GetBikeRiderDetailByYear(int bikeRiderId, int year);
        BikeRiderDetail GetBikeRiderDetail(int bikeRiderDetailId);
        BikeRiderDetail GetBikeRiderDetailByName(string name, int year);
        BikeRiderDetail GetBikeRiderDetailByMainLastName(string mainlastname, int year);
        IList<BikeRiderDetail> GetBikeRiderDetails(Expression<Func<BikeRiderDetail, bool>> filter);
        BikeRider Get(int id);
        IList<BikeRider> Get(int[] ids);
        IEnumerable<BikeRider> GetBySearchText(string searchtext);
        IEnumerable<Models.VMBikeRider> GetBySearchText(string searchtext, int year);
        IEnumerable<Models.VMBikeRider> GetBySearchTextYouth(string searchtext, int year);
        void AddBikeRider(BikeRider bikeRider);
        int AddBikeRiderSave(BikeRider bikeRider);
        void AddBikeRiderDetail(BikeRiderDetail bikeRiderDetail);
    }
}

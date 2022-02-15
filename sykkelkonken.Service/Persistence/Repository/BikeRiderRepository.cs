using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using sykkelkonken.Data;
using sykkelkonken.Service.Models;

namespace sykkelkonken.Service.Persistence
{
    public class BikeRiderRepository : IBikeRiderRepository
    {
        private readonly Context _context;

        public BikeRiderRepository(Context context)
        {
            _context = context;
        }

        public void DeleteAllBikeRiders()
        {
            this._context.Database.ExecuteSqlCommand("truncate table dbo.BikeRider");
        }

        public void AddBikeRidersInBulk(System.Data.DataTable dtBikeRiders, string connectionstring)
        {
            using (System.Data.SqlClient.SqlBulkCopy s = new System.Data.SqlClient.SqlBulkCopy(connectionstring))
            {
                s.BulkCopyTimeout = 120;
                s.BatchSize = 5000;
                s.DestinationTableName = "dbo.BikeRider";
                s.ColumnMappings.Add("Name", "BikeRiderName");
                s.ColumnMappings.Add("Team", "BikeTeamCode");
                s.ColumnMappings.Add("Nationality", "Nationality");
                s.ColumnMappings.Add("CQ", "CQPoints");

                s.WriteToServer(dtBikeRiders);
                s.Close();
            }
        }

        public BikeRider GetBikeRiderByName(string name)
        {
            return this._context.BikeRiders.FirstOrDefault(br => br.BikeRiderName == name);
        }

        public BikeRider GetBikeRiderByNameNationality(string name, string nationality)
        {
            IList<BikeRider> bikeRiders = this._context.BikeRiders.Where(br => br.BikeRiderName == name).ToList();
            if (bikeRiders.Count > 0)
            {
                if (bikeRiders.Count == 1)
                {
                    return bikeRiders.FirstOrDefault(br => br.BikeRiderName == name);
                }
                else
                {
                    if (bikeRiders.Where(br => br.Nationality == nationality).Count() > 1)
                    {
                        foreach (var bikeRider in bikeRiders.Where(br => br.Nationality == nationality))
                        {
                            bikeRider.BikeTeamName = "Check Duplicate Names";
                        }
                        _context.SaveChanges();
                    }
                    return bikeRiders.Where(br => br.Nationality == nationality).FirstOrDefault();
                }
            }
            return null;
        }

        public BikeRider Get(int id)
        {
            return this._context.BikeRiders.SingleOrDefault(br => br.BikeRiderId == id);
        }

        public IList<BikeRider> Get(int[] ids)
        {
            return _context.BikeRiders.Where(br => ids.Contains(br.BikeRiderId)).ToList();
        }

        public IEnumerable<BikeRider> GetBySearchText(string searchtext)
        {
            return _context.BikeRiders.Where(br => br.BikeRiderName.ToLower().Contains(searchtext.ToLower()) || br.BikeTeamName.ToLower().Contains(searchtext.ToLower()) || br.BikeTeamCode.ToLower().Contains(searchtext.ToLower()) || br.Nationality.ToLower().Contains(searchtext.ToLower())).ToList();
        }

        public IEnumerable<Models.VMBikeRider> GetBySearchText(string searchtext, int year)
        {

            var res = _context.Database.SqlQuery<Models.VMBikeRider>(string.Format(

                @"select br.BikeRiderId, br.BikeRiderName, br.Nationality, det.Year, det.CQPoints, det.BikeTeamCode
                    from BikeRider br
                    inner join BikeRiderDetail det on det.BikeRiderId = br.BikeRiderId
                    where det.Year = {0} and (br.BikeRiderName like '%{1}%' or br.Nationality like '%{1}%' or det.BikeTeamCode like '%{1}%' or det.BikeTeamName like '%{1}%')
                ORDER BY det.CQPoints DESC", year, searchtext)).ToList();
            return res;
        }

        public IEnumerable<VMBikeRider> GetBySearchTextYouth(string searchtext, int year)
        {
            var youthLimitYear = year - 25;
            var res = _context.Database.SqlQuery<Models.VMBikeRider>(string.Format(
                @"select br.BikeRiderId, br.BikeRiderName, br.Nationality, det.Year, det.CQPoints, det.BikeTeamCode
                    from BikeRider br
                    inner join BikeRiderDetail det on det.BikeRiderId = br.BikeRiderId
                    where br.BirthDate is not null AND br.BirthDate >= '{2}' AND det.Year = {0} and (br.BikeRiderName like '%{1}%' or br.Nationality like '%{1}%' or det.BikeTeamCode like '%{1}%' or det.BikeTeamName like '%{1}%')
                ORDER BY det.CQPoints DESC", year, searchtext, string.Format("{0}0101", youthLimitYear))).ToList();
            return res;
        }

        public void AddBikeRider(BikeRider bikeRider)
        {
            _context.BikeRiders.Add(bikeRider);
        }

        public int AddBikeRiderSave(BikeRider bikeRider)
        {
            _context.BikeRiders.Add(bikeRider);
            _context.SaveChanges();
            return bikeRider.BikeRiderId;
        }

        public void AddBikeRiderDetail(BikeRiderDetail bikeRiderDetail)
        {
            _context.BikeRiderDetails.Add(bikeRiderDetail);
        }

        public BikeRiderDetail GetBikeRiderDetailByYear(int bikeRiderId, int year)
        {
            return this._context.BikeRiderDetails.SingleOrDefault(br => br.BikeRiderId == bikeRiderId && br.Year == year);
        }

        public IList<BikeRiderDetail> GetBikeRiderDetails(Expression<Func<BikeRiderDetail, bool>> filter)
        {
            return _context.BikeRiderDetails.Where(filter).ToList();
        }

        public BikeRiderDetail GetBikeRiderDetailByName(string name, int year)
        {
            return this._context.BikeRiderDetails.FirstOrDefault(br => br.BikeRider.BikeRiderName == name && br.Year == year);
        }

        public BikeRiderDetail GetBikeRiderDetailByMainLastName(string mainlastname, int year)//søker f.eks etter kun valgren og ikke valgren hundahl (andersen)
        {
            return this._context.BikeRiderDetails.FirstOrDefault(br => br.BikeRider.BikeRiderName.Contains(mainlastname) && br.Year == year);
        }

        public BikeRiderDetail GetBikeRiderDetail(int bikeRiderDetailId)
        {
            return this._context.BikeRiderDetails.FirstOrDefault(br => br.BikeRiderDetailId == bikeRiderDetailId);
        }
    }
}
using sykkelkonken.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Persistence
{
    public class BikeRaceRepository : IBikeRaceRepository
    {
        private readonly Context _context;

        public BikeRaceRepository(Context context)
        {
            _context = context;
        }

        public void DeleteBikeRacesByYear(int year)
        {
            this._context.Database.ExecuteSqlCommand(string.Format("delete from dbo.BikeRaceDetailId Where Year = {0}", year));
        }

        public void DeleteAllBikeRaces()
        {
            this._context.Database.ExecuteSqlCommand("truncate table dbo.BikeRace");
        }

        public void AddBikeRacesInBulk(DataTable dtBikeRaces, string connectionstring)
        {
            using (System.Data.SqlClient.SqlBulkCopy s = new System.Data.SqlClient.SqlBulkCopy(connectionstring))
            {
                s.BulkCopyTimeout = 120;
                s.BatchSize = 5000;
                s.DestinationTableName = "dbo.BikeRace";
                s.ColumnMappings.Add("Name", "Name");
                s.ColumnMappings.Add("Date From", "StartDate");
                s.ColumnMappings.Add("Date To", "FinishDate");
                s.ColumnMappings.Add("Country", "CountryName");
                s.ColumnMappings.Add("BikeRaceCategoryId", "BikeRaceCategoryId");

                s.WriteToServer(dtBikeRaces);
                s.Close();
            }
        }

        public IList<BikeRaceDetail> GetBikeRaceDetails()
        {
            return this._context.BikeRaceDetails.ToList();
        }

        public IList<BikeRaceDetail> GetBikeRaceDetails(int year)
        {
            return this._context.BikeRaceDetails.Where(b => b.Year == year).ToList();
        }

        public BikeRace GetBikeRace(int bikeRaceId)
        {
            return this._context.BikeRaces.SingleOrDefault(br => br.BikeRaceId == bikeRaceId);
        }

        public BikeRace GetBikeRace(string name)
        {
            return this._context.BikeRaces.SingleOrDefault(br => br.Name == name);
        }

        public BikeRaceDetail GetBikeRaceDetail(int bikeRaceDetailId)
        {
            return this._context.BikeRaceDetails.SingleOrDefault(br => br.BikeRaceDetailId == bikeRaceDetailId);
        }

        public BikeRaceDetail GetBikeRaceDetail(int bikeRaceId, int year)
        {
            return this._context.BikeRaceDetails.SingleOrDefault(br => br.BikeRaceId == bikeRaceId && br.Year == year);
        }

        public void UpdateBikeRaceResult(int bikeRaceDetailId, int bikeRiderId, int position)
        {
            var bikeRaceResult = this._context.BikeRaceResults.SingleOrDefault(brr => brr.BikeRaceDetailId == bikeRaceDetailId && brr.Position == position);
            if (bikeRaceResult != null)
            {
                if (bikeRiderId > 0)
                {
                    //update
                    bikeRaceResult.BikeRiderId = bikeRiderId;
                }
                else
                {
                    //delete if bikeRiderId = -1
                    this._context.BikeRaceResults.Remove(bikeRaceResult);
                }
            }
            else if (bikeRiderId > 0 && position > 0)
            {

                //add
                bikeRaceResult = new BikeRaceResult()
                {
                    BikeRaceDetailId = bikeRaceDetailId,
                    BikeRiderId = bikeRiderId,
                    Position = position,
                };
                this._context.BikeRaceResults.Add(bikeRaceResult);
            }
            //this._context.SaveChanges();
        }


        public void UpdateStageResult(int bikeRaceDetailId, int stageNo, int stagePosition, int bikeRiderId)
        {
            var stageResult = this._context.StageResults.SingleOrDefault(sr => sr.BikeRaceDetailId == bikeRaceDetailId && sr.StageNo == stageNo && sr.StagePosition == stagePosition);
            if (stageResult != null)
            {
                if (bikeRiderId > 0)
                {
                    //update
                    stageResult.BikeRiderId = bikeRiderId;
                }
                else
                {
                    //delete if bikeRiderId = -1
                    this._context.StageResults.Remove(stageResult);
                }
            }
            else if (bikeRiderId > 0 && stagePosition > 0)
            {
                //add
                stageResult = new StageResult()
                {
                    BikeRaceDetailId = bikeRaceDetailId,
                    BikeRiderId = bikeRiderId,
                    StagePosition = stagePosition,
                    StageNo = stageNo
                };
                this._context.StageResults.Add(stageResult);
            }
            //this._context.SaveChanges();
        }

        public void UpdateLeaderJerseyResult(int bikeRaceDetailId, int bikeRiderId, int position, int leaderJerseyId)
        {
            var leaderJerseyResult = this._context.LeaderJerseyResults.SingleOrDefault(brr => brr.BikeRaceDetailId == bikeRaceDetailId && brr.LeaderJerseyPosition == position && brr.LeaderJerseyId == leaderJerseyId);
            if (leaderJerseyResult != null)
            {
                if (bikeRiderId > 0)
                {
                    //update
                    leaderJerseyResult.BikeRiderId = bikeRiderId;
                }
                else
                {
                    //delete if bikeRiderId = -1
                    this._context.LeaderJerseyResults.Remove(leaderJerseyResult);
                }
            }
            else if (bikeRiderId > 0 && position > 0)
            {
                //add
                leaderJerseyResult = new LeaderJerseyResult()
                {
                    BikeRaceDetailId = bikeRaceDetailId,
                    BikeRiderId = bikeRiderId,
                    LeaderJerseyPosition = position,
                    LeaderJerseyId = leaderJerseyId,
                };
                this._context.LeaderJerseyResults.Add(leaderJerseyResult);
            }
            //this._context.SaveChanges();
        }

        public void AddBikeRace(BikeRace bikeRace)
        {
            _context.BikeRaces.Add(bikeRace);
        }

        public int AddBikeRaceSave(BikeRace bikeRace)
        {
            _context.BikeRaces.Add(bikeRace);
            _context.SaveChanges();
            return bikeRace.BikeRaceId;
        }

        public void AddBikeRaceDetail(BikeRaceDetail bikeRaceDetail)
        {
            _context.BikeRaceDetails.Add(bikeRaceDetail);
        }

        public int AddBikeRaceDetailSave(BikeRaceDetail bikeRaceDetail)
        {
            _context.BikeRaceDetails.Add(bikeRaceDetail);
            _context.SaveChanges();
            return bikeRaceDetail.BikeRaceDetailId;
        }

        public void DeleteBikeRaceDetail(BikeRaceDetail bikeRaceDetail)
        {
            //remove results first
            if (bikeRaceDetail.BikeRaceResults.Count > 0)
            {
                _context.BikeRaceResults.RemoveRange(bikeRaceDetail.BikeRaceResults);
            }
            if (bikeRaceDetail.StageResults.Count > 0)
            {
                _context.StageResults.RemoveRange(bikeRaceDetail.StageResults);
            }
            if (bikeRaceDetail.LeaderJerseyResults.Count > 0)
            {
                _context.LeaderJerseyResults.RemoveRange(bikeRaceDetail.LeaderJerseyResults);
            }
            _context.BikeRaceDetails.Remove(bikeRaceDetail);
        }
    }
}
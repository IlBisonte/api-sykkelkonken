using sykkelkonken.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Service.Persistence
{
    public interface IBikeRaceRepository
    {
        void DeleteBikeRacesByYear(int year);
        void DeleteAllBikeRaces();
        void AddBikeRacesInBulk(System.Data.DataTable dtBikeRaces, string connectionstring);
        IList<BikeRaceDetail> GetBikeRaceDetails();
        IList<BikeRaceDetail> GetBikeRaceDetails(int year);
        BikeRace GetBikeRace(int bikeRaceId);
        BikeRace GetBikeRace(string name);
        BikeRaceDetail GetBikeRaceDetail(int bikeRaceDetailId);
        BikeRaceDetail GetBikeRaceDetail(int bikeRaceId, int year);
        BikeRaceDetail GetLastCalculatedBikeRace(int year);
        void AddBikeRace(BikeRace bikeRace);
        int AddBikeRaceSave(BikeRace bikeRace);
        void AddBikeRaceDetail(BikeRaceDetail bikeRaceDetail);
        int AddBikeRaceDetailSave(BikeRaceDetail bikeRaceDetail);
        void DeleteBikeRaceDetail(BikeRaceDetail bikeRaceDetail);
        void UpdateBikeRaceResult(int bikeRaceDetailId, int bikeRiderId, int position);
        void UpdateStageResult(int bikeRaceDetailId, int stageNo, int stagePosition, int bikeRiderId);
        void UpdateLeaderJerseyResult(int bikeRaceDetailId, int bikeRiderId, int position, int leaderJerseyId);
        IList<BikeRaceSeasonPlacement> GetBikeRaceSeasonPlacements(int year);
        void AddBikeRaceSeasonPlacement(BikeRaceSeasonPlacement bikeRaceSeasonPlacement);
        void RemoveBikeRaceSeasonPlacement(BikeRaceSeasonPlacement bikeRaceSeasonPlacement);
    }
}

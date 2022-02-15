using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sykkelkonken.Data;

namespace sykkelkonken.Service.Persistence
{
    public interface IYouthTeamRepository
    {
        IList<YouthTeam> Get(int year);
        YouthTeam GetById(int youthTeamId);
        IList<YouthTeamBikeRider> GetBikeRidersYouthTeam(int youthTeamTeamId);
        int AddYouthTeam(YouthTeam youthTeam);//Add YouthTeamTeam and return new YouthTeamTeamId
        void UpdateYouthTeam(int youthTeamId, string teamName);//Update YouthTeamTeam 
        void AddBikeRiderToYouthTeam(int youthTeamId, int bikeRiderDetailId);
        void UpdateRiderYouthTeam(int youthTeamId, int origBikeRiderDetailId, int newBikeRiderDetailId);//Update YouthTeamTeamBikeRider
        void RemoveRiderYouthTeam(int youthTeamId, int bikeRiderDetailId);//remove bikerider from youth team
        void RemoveYouthTeams();
        void RemoveYouthTeam(int youthTeamId);
    }
}

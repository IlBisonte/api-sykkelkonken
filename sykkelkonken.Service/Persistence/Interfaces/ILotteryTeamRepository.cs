using sykkelkonken.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Service.Persistence
{
    public interface ILotteryTeamRepository
    {
        IList<LotteryTeam> Get(int year);
        LotteryTeam GetById(int lotteryTeamId);
        IList<LotteryTeamBikeRider> GetBikeRidersLotteryTeam(int lotteryTeamTeamId);
        int AddLotteryTeam(LotteryTeam lotteryTeam);//Add LotteryTeamTeam and return new LotteryTeamTeamId
        void UpdateLotteryTeam(int lotteryTeamId, string teamName, string color);//Update LotteryTeamTeam 
        void AddBikeRiderToLotteryTeam(int lotteryTeamId, int bikeRiderDetailId);
        void UpdateRiderLotteryTeam(int lotteryTeamId, int origBikeRiderDetailId, int newBikeRiderDetailId);//Update LotteryTeamTeamBikeRider
        void RemoveRiderLotteryTeam(int lotteryTeamId, int bikeRiderDetailId);//remove bikerider from lottery team
        void RemoveLotteryTeams();
        void RemoveLotteryTeam(int lotteryTeamId);
    }
}

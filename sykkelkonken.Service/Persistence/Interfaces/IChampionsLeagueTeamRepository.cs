using sykkelkonken.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Service.Persistence
{
    public interface IChampionsLeagueTeamRepository
    {
        IList<ChampionsLeagueTeam> Get(int year);
        ChampionsLeagueTeam GetById(int clTeamId);
        IList<ChampionsLeagueTeamBikeRider> GetBikeRidersCLTeam(int championsLeagueTeamId);
        int AddChampionsLeagueTeam(ChampionsLeagueTeam championsLeagueTeam);//Add ChampionsLeagueTeam and return new ChampionsLeagueTeamId
        void UpdateChampionsLeagueTeam(int championsLeagueTeamId, string teamName, string color);//Update ChampionsLeagueTeam 
        void AddBikeRiderToChampionsLeagueTeam(int championsLeagueTeamId, int bikeRiderDetailId);
        void UpdateRiderChampionsLeagueTeam(int championsLeagueTeamId, int origBikeRiderDetailId, int newBikeRiderDetailId);//Update ChampionsLeagueTeamBikeRider
        void RemoveRiderChampionsLeagueTeam(int championsLeagueTeamId, int bikeRiderDetailId);//remove bikerider from cl team
        void RemoveChampionsLeagueTeams();
        void RemoveChampionsLeagueTeam(int championsLeagueTeamId);
    }
}

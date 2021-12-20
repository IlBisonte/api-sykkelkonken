using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using sykkelkonken.Data;
using sykkelkonken.Service.Models;

namespace sykkelkonken.Service.Persistence
{
    public class ResultRepository : IResultRepository
    {
        private readonly Context _context;

        public ResultRepository(Context context)
        {
            _context = context;
        }

        public IList<VMCompetitionTeamResults> GetCompetitionTeamResults(int year)
        {

            var res = _context.Database.SqlQuery<VMCompetitionTeamResults>(string.Format(
                @"SELECT      CompetitionTeamId, Name, TotalCQPoints, Note, Year, Points, TeamIndex, RANK() OVER(ORDER BY Points DESC) AS Position

                FROM            dbo.v_CompetitionTeamPoints
                WHERE Year = {0}
                ORDER BY Points DESC", year)).ToList();
            return res;
        }

        public IList<VMCompetitionTeamBikeRiderResults> GetCompetitionTeamBikeRiderResults(int competitionTeamId)
        {
            var res = _context.Database.SqlQuery<VMCompetitionTeamBikeRiderResults>(string.Format(
                @"SELECT      CompetitionTeamId, Name, BikeRiderId, BikeRiderName, CQPoints, Points, RiderIndex  
                FROM            dbo.v_CompetitionTeamPointsDetail
                WHERE CompetitionTeamId = {0}
                ORDER BY CQPoints DESC", competitionTeamId)).ToList();
            return res;
        }

        public IList<VMBikeRiderResults> GetBikeRiderResults(int bikeRiderId, int year)
        {
            var res = _context.Database.SqlQuery<VMBikeRiderResults>(string.Format(
                @"SELECT       BikeRaceDetailId, BikeRaceId, BikeRaceName, dbo.GetBikeRaceResultText(GCPosition, StagePosition, StageNo, LeaderJerseyName) AS Description, Year,
				StartDate, BikeRiderId, 
				CASE WHEN GCPosition IS NULL AND StagePosition IS NULL THEN LeaderJerseyPosition ELSE (CASE WHEN GCPosition IS NULL THEN StagePosition ELSE GCPosition END) END AS Position, 
				CASE WHEN GCPoints IS NULL AND StagePoints IS NULL THEN LeaderJerseyPoints ELSE (CASE WHEN GCPoints IS NULL THEN StagePoints ELSE GCPoints END) END AS Points
                FROM            dbo.v_BikeRaceResults 
                WHERE BikeRiderId = {0} AND Year = {1}
                ORDER BY StartDate, GCPosition, LeaderJerseyPosition, StageNo", bikeRiderId, year)).ToList();
            return res;
        }

        public IList<BikeRaceTotalCompetitionTeamResults> GetBikeRaceTotalCompetitionTeamResults(int bikeRaceDetailId)
        {
            var res = _context.Database.SqlQuery<BikeRaceTotalCompetitionTeamResults>(string.Format(
                @"SELECT        CompetitionTeamId, CompetitionTeamName, BikeRaceDetailId, BikeRaceId, BikeRaceName, TotalPoints, BikeRiders, RANK() OVER(ORDER BY SUM(TotalPoints) DESC) AS Position
                FROM            v_BikeRaceCompetitionTeamResults_Total
                WHERE BikeRaceDetailId = {0} 
                GROUP BY CompetitionTeamId, CompetitionTeamName, BikeRaceDetailId, BikeRaceId, BikeRaceName, BikeRiders, TotalPoints
                ", bikeRaceDetailId)).ToList();
            return res;
        }

        public IList<BikeRaceGCCompetitionTeamResults> GetBikeRaceGCCompetitionTeamResults(int bikeRaceDetailId)
        {
            var res = _context.Database.SqlQuery<BikeRaceGCCompetitionTeamResults>(string.Format(
                @"SELECT       CompetitionTeamId, CompetitionTeamName, GCPoints, '(' + BikeRiders + ')' AS BikeRiders, RANK() OVER(ORDER BY GCPoints DESC) AS Position
                FROM            dbo.v_BikeRaceCompetitionTeamResults_GC 
                WHERE BikeRaceDetailId = {0} AND GCPoints IS NOT NULL
                ORDER BY GCPoints desc", bikeRaceDetailId)).ToList();
            return res;
        }

        public IList<BikeRaceStageCompetitionTeamResults> GetBikeRaceStageCompetitionTeamResults(int bikeRaceDetailId)
        {
            var res = _context.Database.SqlQuery<BikeRaceStageCompetitionTeamResults>(string.Format(
                @"SELECT       CompetitionTeamId, CompetitionTeamName, BikeRaceId, BikeRaceName, StagePoints, StageNo, '(' + BikeRiders + ')' AS BikeRiders
                FROM            dbo.v_BikeRaceCompetitionTeamResults_Stage 
                WHERE BikeRaceDetailId = {0} AND StagePoints IS NOT NULL
                ORDER BY StageNo, StagePoints desc", bikeRaceDetailId)).ToList();
            return res;
        }

        public IList<BikeRaceLeaderJerseyCompetitionTeamResults> GetBikeRaceLeaderJerseyCompetitionTeamResults(int bikeRaceDetailId)
        {
            var res = _context.Database.SqlQuery<BikeRaceLeaderJerseyCompetitionTeamResults>(string.Format(
                @"SELECT        CompetitionTeamId, CompetitionTeamName, BikeRaceId, BikeRaceName, LeaderJerseyPoints, LeaderJerseyName, '(' + BikeRiders + ')' AS BikeRiders
                FROM            v_BikeRaceCompetitionTeamResults_LeaderJersey
                WHERE BikeRaceDetailId = {0} AND LeaderJerseyPoints IS NOT NULL
                ORDER BY LeaderJerseyPoints desc", bikeRaceDetailId)).ToList();
            return res;
        }

        public IList<VMCompetitionTeamResults> GetCompetitionTeamResults_Monuments(int year)
        {
            var res = _context.Database.SqlQuery<VMCompetitionTeamResults>(string.Format(
                @"SELECT      CompetitionTeamId, Name, TotalCQPoints, Note, Year, Points, TeamIndex, RANK() OVER(ORDER BY Points DESC) AS Position
                FROM            dbo.v_CompetitionTeamPoints_Monuments
                WHERE Year = {0}
                ORDER BY Points DESC", year)).ToList();
            return res;
        }

        public IList<VMCompetitionTeamBikeRiderResults> GetCompetitionTeamBikeRiderResults_Monuments(int competitionTeamId)
        {
            var res = _context.Database.SqlQuery<VMCompetitionTeamBikeRiderResults>(string.Format(
                @"SELECT      CompetitionTeamId, Name, BikeRiderId, BikeRiderName, CQPoints, Points, RiderIndex  
                FROM            dbo.v_CompetitionTeamPointsDetail_Monuments
                WHERE CompetitionTeamId = {0}
                ORDER BY CQPoints DESC", competitionTeamId)).ToList();
            return res;
        }

        public IList<VMBikeRiderResults> GetBikeRiderResults_Monuments(int bikeRiderId, int year)
        {
            var res = _context.Database.SqlQuery<VMBikeRiderResults>(string.Format(
                @"SELECT       BikeRaceId, BikeRaceName, BikeRiderId, GCPosition, GCPoints
                FROM            dbo.v_BikeRaceResults
                WHERE BikeRiderId = {0} AND Year = {1} AND BikeRaceCategoryId = 2
                ORDER BY BikeRaceId, GCPosition, StageNo", bikeRiderId, year)).ToList();
            return res;
        }

        public IList<VMCompetitionTeamResults> GetCLTeamResults(int year)
        {

            var res = _context.Database.SqlQuery<VMCompetitionTeamResults>(string.Format(
                @"SELECT     ChampionsLeagueTeamId AS CompetitionTeamId, Name, Year, TotalCQPoints, Note, Color, Points, TeamIndex, RANK() OVER(ORDER BY Points DESC) AS Position
                FROM            dbo.v_ChampionsLeagueTeamPoints
                WHERE   Year = {0}
                ORDER BY Points DESC", year)).ToList();
            return res;
        }

        public IList<VMCompetitionTeamBikeRiderResults> GetCLTeamBikeRiderResults(int clTeamId)
        {
            var res = _context.Database.SqlQuery<VMCompetitionTeamBikeRiderResults>(string.Format(
                @"SELECT      ChampionsLeagueTeamId AS CompetitionTeamId, Name, BikeRiderId, BikeRiderDetailId, BikeRiderName, CQPoints, Points, RiderIndex  
                FROM            dbo.v_ChampionsLeagueTeamPointsDetail
                WHERE ChampionsLeagueTeamId = {0}
                ORDER BY CQPoints DESC", clTeamId)).ToList();
            return res;
        }

        public IList<VMCompetitionTeamResults> GetLotteryTeamResults(int year)
        {

            var res = _context.Database.SqlQuery<VMCompetitionTeamResults>(string.Format(
                @"SELECT     LotteryTeamId AS CompetitionTeamId, Name, Year, TotalCQPoints, Note, Color, Points, TeamIndex, RANK() OVER(ORDER BY Points DESC) AS Position
                FROM            dbo.v_LotteryTeamPoints
                WHERE   Year = {0}
                ORDER BY Points DESC", year)).ToList();
            return res;
        }

        public IList<VMCompetitionTeamBikeRiderResults> GetLotteryTeamBikeRiderResults(int lotteryTeamId)
        {
            var res = _context.Database.SqlQuery<VMCompetitionTeamBikeRiderResults>(string.Format(
                @"SELECT      LotteryTeamId AS CompetitionTeamId, Name, BikeRiderId, BikeRiderDetailId, BikeRiderName, CQPoints, Points, RiderIndex  
                FROM            dbo.v_LotteryTeamPointsDetail
                WHERE LotteryTeamId = {0}
                ORDER BY CQPoints DESC", lotteryTeamId)).ToList();
            return res;
        }
    }
}
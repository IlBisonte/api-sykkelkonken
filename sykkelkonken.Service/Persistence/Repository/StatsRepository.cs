using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using sykkelkonken.Data;
using sykkelkonken.Service.Models;

namespace sykkelkonken.Service.Persistence
{
    public class StatsRepository : IStatsRepository
    {
        private readonly Context _context;

        public StatsRepository(Context context)
        {
            _context = context;
        }

        public IList<VMBikeRiderStats> GetBikeRiderStats(int year)
        {
            var res = _context.Database.SqlQuery<VMBikeRiderStats>(string.Format(
                @"SELECT      BikeRiderId, BikeRiderDetailId, BikeRiderName, BikeTeamCode, Year, Points, NoOfSelections, SelectedBy, CQPoints, RiderIndex, CLTeamName, Color
                FROM            v_BikeRiderStats
                WHERE (Points > 0 OR NoOfSelections > 0) AND Year = {0}
                ORDER BY Points DESC, CQPoints DESC", year)).ToList();
            return res;
        }

        public IList<VMBikeRiderScoreAllTime> GetBikeRiderScoreAllTime()
        {
            var res = _context.Database.SqlQuery<VMBikeRiderScoreAllTime>(string.Format(
                @"SELECT      BikeRiderId, BikeRiderName, Points, RiderIndex
                FROM            v_SumBikeRider_AllTime")).ToList();
            return res;
        }

        public IList<VMCompetitionTeamStatsAllTime> GetCompetitionTeamStatsAllTime()
        {
            var res = _context.Database.SqlQuery<VMCompetitionTeamStatsAllTime>(string.Format(
                @"SELECT      Name, Points, TeamIndex, NoOfParticipations, Victories
                FROM            v_SumCompetitionTeam_AllTime")).ToList();
            return res;
        }

        public IList<VMChampionsLeagueTeamStatsAllTime> GetChampionsLeagueTeamStatsAllTime()
        {
            var res = _context.Database.SqlQuery<VMChampionsLeagueTeamStatsAllTime>(string.Format(
                @"SELECT      Name, Points, TeamIndex, NoOfParticipations, Victories
                FROM            v_SumChampionsLeague_AllTime")).ToList();
            return res;
        }

        public IList<VMCompetitionTeamPointsPerBikeRace> GetCompetitionTeamResultPerBikeRace(int year)
        {
            var res = _context.Database.SqlQuery<VMCompetitionTeamPointsPerBikeRace>(string.Format(
                @"SELECT  CompetitionTeamId, CompetitionTeamName, BikeRaceDetailId, BikeRaceId, BikeRaceName, StartDate, Year, TotalPoints
                FROM            v_CompetitionTeamPointsPerBikeRace
                WHERE (TotalPoints is not null) AND Year = {0}", year)).ToList();
            return res;
        }

        public IList<VMCompetitionTeamPointsPerBikeRace> GetCompetitionTeamPointsPerBikeRace(int year, int bikeRaceDetailId)
        {
            var res = _context.Database.SqlQuery<VMCompetitionTeamPointsPerBikeRace>(string.Format(
                @"SELECT        CompetitionTeamId, CompetitionTeamName, BikeRaceDetailId, BikeRaceId, BikeRaceName, StartDate, Year, TotalPoints
                FROM            v_CompetitionTeamPointsPerBikeRace
                WHERE Year = {0} and BikeRaceDetailId = {1}", year, bikeRaceDetailId)).ToList();
            return res;
        }

        public IList<VMCompetitionTeamPointsPerStage> GetCompetitionTeamPointsPerStage(int year)
        {
            var res = _context.Database.SqlQuery<VMCompetitionTeamPointsPerStage>(string.Format(
                @"SELECT  CompetitionTeamId, BikeRaceDetailId, StageNo, StagePoints
                FROM            v_CompetitionTeamPointsPerStage
                WHERE Year = {0}", year)).ToList();
            return res;
        }

        public IList<CompetitionTeamPointsByBikeRaceCategory> GetCompetitionTeamPointsByBikeRaceCategory(int year, int competitionTeamId)
        {
            var res = _context.Database.SqlQuery<CompetitionTeamPointsByBikeRaceCategory>(string.Format(
                @"SELECT  CompetitionTeamId, Name, BikeRaceCategoryId, Year, Points, GCPoints, StagePoints, LeaderJerseyPoints
                FROM            v_CompetitionTeamPoints_PerBikeRaceCategory
                WHERE Year = {0} and CompetitionTeamId = {1}", year, competitionTeamId)).ToList();
            return res;
        }

        public IList<CompetitionTeamPointsByBikeRace> GetCompetitionTeamPointsByCompetitionTeam(int year, int competitionTeamId)
        {
            var res = _context.Database.SqlQuery<CompetitionTeamPointsByBikeRace>(string.Format(
                @"SELECT        CompetitionTeamId, Name, BikeRaceCategoryId, BikeRaceDetailId, BikeRaceName, Year, Points, GCPoints, StagePoints, LeaderJerseyPoints
                FROM            v_CompetitionTeamPoints_PerBikeRace
                WHERE Year = {0} and CompetitionTeamId = {1}", year, competitionTeamId)).ToList();
            return res;
        }

        public IList<BikeRiderPointsByBikeRace> GetBikeRiderPointsByCompetitionTeam(int year, int competitionTeamId)
        {
            var res = _context.Database.SqlQuery<BikeRiderPointsByBikeRace>(string.Format(
                @"SELECT        CompetitionTeamId, Name, BikeRiderDetailId, BikeRiderName, BikeRaceCategoryId, BikeRaceDetailId, BikeRaceName, Year, Points, GCPoints, StagePoints, LeaderJerseyPoints
                FROM            v_CompetitionTeamPoints_BikeRider_PerBikeRace
                WHERE Year = {0} and CompetitionTeamId = {1}", year, competitionTeamId)).ToList();
            return res;
        }

        public IList<BikeRiderPointsByBikeRace> GetBikeRiderPointsByBikeRace(int year, int competitionTeamId, int bikeRaceDetailId)
        {
            var res = _context.Database.SqlQuery<BikeRiderPointsByBikeRace>(string.Format(
                @"SELECT        CompetitionTeamId, Name, BikeRiderDetailId, BikeRiderName, BikeRaceCategoryId, BikeRaceDetailId, BikeRaceName, Year, Points, GCPoints, StagePoints, LeaderJerseyPoints
                FROM            v_CompetitionTeamPoints_BikeRider_PerBikeRace
                WHERE Year = {0} and CompetitionTeamId = {1} and BikeRaceDetailId = {2}", year, competitionTeamId, bikeRaceDetailId)).ToList();
            return res;
        }

        public IList<BikeRiderPointsByBikeRace> GetBikeRacePointsByBikeRider(int year, int bikeRiderDetailId)
        {
            var res = _context.Database.SqlQuery<BikeRiderPointsByBikeRace>(string.Format(
                @"SELECT        BikeRiderDetailId, BikeRiderName, BikeRaceCategoryId, BikeRaceDetailId, BikeRaceName, Year, Points, GCPoints, StagePoints, LeaderJerseyPoints
                FROM            v_BikeRiderPoints_PerBikeRace
                WHERE Year = {0}  and BikeRiderDetailId = {1}", year, bikeRiderDetailId)).ToList();
            return res;
        }

        public IList<string> GetCompTeamsWithSelectedBikeRider(int bikeRiderDetailId)
        {
            var res = _context.Database.SqlQuery<string>(string.Format(
                @"SELECT       DISTINCT Name
                FROM            v_CompetitionTeamBikeRider
                WHERE BikeRiderDetailId = {0}", bikeRiderDetailId)).ToList();
            return res;
        }
    }
}
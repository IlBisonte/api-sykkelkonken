using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Data
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }

        public DbSet<BikeRace> BikeRaces { get; set; }
        public DbSet<BikeRaceDetail> BikeRaceDetails { get; set; }
        public DbSet<BikeRaceCategory> BikeRaceCategories { get; set; }
        public DbSet<BikeRaceCategoryPoints> BikeRaceCategoryPointss { get; set; }
        public DbSet<BikeRaceResult> BikeRaceResults { get; set; }
        public DbSet<BikeRider> BikeRiders { get; set; }
        public DbSet<BikeRiderDetail> BikeRiderDetails { get; set; }
        public DbSet<BikeRiderResult> BikeRiderResults { get; set; }
        public DbSet<ChampionsLeagueTeam> ChampionsLeagueTeams { get; set; }
        public DbSet<ChampionsLeagueTeamBikeRider> ChampionsLeagueTeamBikeRiders { get; set; }
        public DbSet<CompetitionTeam> CompetitionTeams { get; set; }
        public DbSet<CompetitionTeamBikeRider> CompetitionTeamBikeRiders { get; set; }
        public DbSet<StagePoints> StagePointss { get; set; }
        public DbSet<StageResult> StageResults { get; set; }
        public DbSet<LeaderJersey> LeaderJerseys { get; set; }
        public DbSet<LeaderJerseyPoints> LeaderJerseyPointss { get; set; }
        public DbSet<LeaderJerseyResult> LeaderJerseyResults { get; set; }
        public DbSet<LotteryTeam> LotteryTeams { get; set; }
        public DbSet<LotteryTeamBikeRider> LotteryTeamBikeRiders { get; set; }

        public Context()
            : base("name=Context")
        {
        }

        #region Overrides

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            /* Map  classes to tables */
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Session>().ToTable("Session");

            modelBuilder.Entity<BikeRace>().ToTable("BikeRace");
            modelBuilder.Entity<BikeRaceDetail>().ToTable("BikeRaceDetail");
            modelBuilder.Entity<BikeRaceCategory>().ToTable("BikeRaceCategory");
            modelBuilder.Entity<BikeRaceCategoryPoints>().ToTable("BikeRaceCategoryPoints");
            modelBuilder.Entity<BikeRaceResult>().ToTable("BikeRaceResult");
            modelBuilder.Entity<BikeRider>().ToTable("BikeRider");
            modelBuilder.Entity<BikeRiderDetail>().ToTable("BikeRiderDetail");
            modelBuilder.Entity<BikeRiderResult>().ToTable("BikeRiderResult");
            modelBuilder.Entity<ChampionsLeagueTeam>().ToTable("ChampionsLeagueTeam");
            modelBuilder.Entity<ChampionsLeagueTeamBikeRider>().ToTable("ChampionsLeagueTeamBikeRider");
            modelBuilder.Entity<CompetitionTeam>().ToTable("CompetitionTeam");
            modelBuilder.Entity<CompetitionTeamBikeRider>().ToTable("CompetitionTeamBikeRider");
            modelBuilder.Entity<StagePoints>().ToTable("StagePoints");
            modelBuilder.Entity<StageResult>().ToTable("StageResult");
            modelBuilder.Entity<LeaderJersey>().ToTable("LeaderJersey");
            modelBuilder.Entity<LeaderJerseyPoints>().ToTable("LeaderJerseyPoints");
            modelBuilder.Entity<LeaderJerseyResult>().ToTable("LeaderJerseyResult");
            modelBuilder.Entity<LotteryTeam>().ToTable("LotteryTeam");
            modelBuilder.Entity<LotteryTeamBikeRider>().ToTable("LotteryTeamBikeRider");
        }

        #endregion
    }
}

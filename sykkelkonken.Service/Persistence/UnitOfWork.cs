using sykkelkonken.Data;

namespace sykkelkonken.Service.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Context _context;

        public IUserRepository Users { get; set; }
        public IBikeRaceRepository BikeRaces { get; set; }
        public IBikeRiderRepository BikeRiders { get; set; }
        public ICompetitionTeamRepository CompetitionTeams { get; set; }
        public IChampionsLeagueTeamRepository ChampionsLeagueTeams { get; set; }
        public ILotteryTeamRepository LotteryTeams { get; set; }
        public IYouthTeamRepository YouthTeams { get; set; }
        public IResultRepository Results { get; set; }
        public IStatsRepository Stats { get; set; }
        public ISessionRepository Session { get; set; }

        public UnitOfWork()
        {
            _context = new Context();

            Users = new UserRepository(_context);
            BikeRaces = new BikeRaceRepository(_context);
            BikeRiders = new BikeRiderRepository(_context);
            CompetitionTeams = new CompetitionTeamRepository(_context);
            ChampionsLeagueTeams = new ChampionsLeagueTeamRepository(_context);
            LotteryTeams = new LotteryTeamRepository(_context);
            YouthTeams = new YouthTeamRepository(_context);
            Results = new ResultRepository(_context);
            Stats = new StatsRepository(_context);
            Session = new SessionRepository(_context);
        }

        public void Complete()
        {
            _context.SaveChanges();
        }
    }
}
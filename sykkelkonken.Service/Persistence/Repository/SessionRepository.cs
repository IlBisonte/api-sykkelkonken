using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using sykkelkonken.Data;

namespace sykkelkonken.Service.Persistence
{
    public class SessionRepository : ISessionRepository
    {
        private readonly Context _context;

        public SessionRepository(Context context)
        {
            _context = context;
        }

        public void Add(Session session)
        {
            _context.Sessions.Add(session);
        }

        public Session GetLastSessionByUserId(int userId)
        {
            return _context.Sessions.Where(s => s.UserId == userId).OrderByDescending(s => s.ExpireDate).FirstOrDefault();
        }
    }
}
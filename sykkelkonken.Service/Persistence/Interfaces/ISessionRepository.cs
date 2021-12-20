using sykkelkonken.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Persistence
{
    public interface ISessionRepository
    {
        Session GetLastSessionByUserId(int userId);
        void Add(Session session);
    }
}
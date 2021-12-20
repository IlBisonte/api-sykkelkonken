using sykkelkonken.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sykkelkonken.Service.Persistence
{
    public interface IUserRepository
    {
        User GetUserByUserId(int userId);
        User GetUserByUserName(string username);
        void SignUpUser(string username, string hashedPassword, byte[] salt, byte[] hash);
        string GetRefreshToken(string username);
        void DeleteRefreshToken(string username, string refreshtoken);
        void SaveRefreshToken(string username, string refreshtoken);
    }
}

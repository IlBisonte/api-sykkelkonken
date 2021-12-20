using sykkelkonken.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly Context _context;

        public UserRepository(Context context)
        {
            _context = context;
        }

        public User GetUserByUserId(int userId)
        {
            return _context.Users.SingleOrDefault(u => u.UserId == userId);
        }

        public User GetUserByUserName(string username)
        {
            return _context.Users.SingleOrDefault(u => u.UserName == username);
        }

        public void SignUpUser(string username, string hashedPassword, byte[] salt, byte[] hash)
        {
            var user = new User();
            user.UserName = username;
            user.Password = hashedPassword;
            //user.Salt = salt;//no need to store salt, it is in the hashedPassword
            _context.Users.Add(user);
        }

        public void DeleteRefreshToken(string username, string refreshtoken)
        {
            var user = GetUserByUserName(username);
            if (user != null)
            {
                if (user.RefreshToken == refreshtoken)
                {
                    user.RefreshToken = "";
                }
            }
        }

        public void SaveRefreshToken(string username, string refreshtoken)
        {
            var user = GetUserByUserName(username);
            if (user != null)
            {
                user.RefreshToken = refreshtoken;
            }
        }

        public string GetRefreshToken(string username)
        {
            var user = GetUserByUserName(username);
            if (user != null)
            {
                return user.RefreshToken;
            }
            else
            {
                return "";
            }
        }
    }
}
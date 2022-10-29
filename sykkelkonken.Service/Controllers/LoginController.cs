using sykkelkonken.Data;
using sykkelkonken.Service.Models.User;
using sykkelkonken.Service.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;

namespace sykkelkonken.Service.Controllers
{
    public class LoginController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoginController()
        {
            _unitOfWork = new UnitOfWork();
        }

        [HttpGet]
        public VMUser Login(string username, string password)
        {
            VMUser returnUser = new VMUser();
            returnUser.IsLoggedIn = false;
            var user = _unitOfWork.Users.GetUserByUserName(username);
            if (user != null)
            {
                string passwordHash = user.Password;
                byte[] hashBytes = Convert.FromBase64String(passwordHash);
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000);
                byte[] hash = pbkdf2.GetBytes(20);

                bool bPasswordOk = true;
                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                    {
                        bPasswordOk = false;
                    }
                }

                if (bPasswordOk)
                {
                    returnUser.UserId = user.UserId;
                    returnUser.UserName = user.UserName;
                    returnUser.IsAdmin = user.IsAdmin;
                    returnUser.IsLoggedIn = true;

                    var token = JwtManager.GenerateToken(username);
                    if (token != null)
                    {
                        returnUser.JwtToken = token;
                    }
                    var refreshToken = JwtManager.GenerateRefreshToken();
                    if (refreshToken != null)
                    {
                        returnUser.RefreshToken = refreshToken;
                        user.RefreshToken = refreshToken;//save hash in db
                    }

                    //create new session
                    //Session session = new Session();
                    //session.UserId = user.UserId;
                    //session.CreatedDate = DateTime.Now;
                    //session.LastAccessedDate = DateTime.Now;
                    //session.ExpireDate = DateTime.Now + TimeSpan.FromDays(1);
                    //_unitOfWork.Session.Add(session);
                    _unitOfWork.Complete();
                }
            }
            return returnUser;
        }

        [HttpPost]
        public void SignUp(User user)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(user.Password, salt, 1000);

            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            string savedPasswordHash = Convert.ToBase64String(hashBytes);

            _unitOfWork.Users.SignUpUser(user.UserName, savedPasswordHash, salt, hash);
            _unitOfWork.Complete();
        }

        //[HttpGet]
        //public VMUser RefreshToken(int userId)
        //{
        //    VMUser returnUser = new VMUser();
        //    returnUser.IsLoggedIn = false;
        //    var user = _unitOfWork.Users.GetUserByUserId(userId);
        //    if (user != null)
        //    {

        //    }
        //}

        [HttpPost]
        public VMRefreshToken Refresh(VMRefreshToken refreshToken)
        {
            var principal = JwtManager.GetPrincipalFromExpiredToken(refreshToken.Token);
            var username = principal.Identity.Name;
            var savedRefreshToken = _unitOfWork.Users.GetRefreshToken(username); //retrieve the refresh token from a data store

            if (refreshToken.RefreshToken != savedRefreshToken)
                throw new SecurityTokenException("Invalid refresh token");

            var newJwtToken = JwtManager.GenerateToken(username);
            var newRefreshToken = JwtManager.GenerateRefreshToken();
            _unitOfWork.Users.DeleteRefreshToken(username, refreshToken.RefreshToken);
            _unitOfWork.Users.SaveRefreshToken(username, newRefreshToken);
            _unitOfWork.Complete();

            return new VMRefreshToken()
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}

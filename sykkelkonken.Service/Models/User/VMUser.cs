using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models.User
{
    public class VMUser
    {
        public bool IsLoggedIn { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
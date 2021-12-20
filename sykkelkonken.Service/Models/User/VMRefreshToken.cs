using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sykkelkonken.Service.Models.User
{
    public class VMRefreshToken
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
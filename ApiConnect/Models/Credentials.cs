using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApiConnect.Models
{

    public class Credentials
    {   
        [Display]
        public string username { get; set; }

       
        [DataType(DataType.Password)]
        public string password { get; set; }

       
        public string host { get; set; }

        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public string TokenDec { get; set; }

        public string RefreshTokenDEc { get; set; }

        public string MessageAddress { get; set; }
    }
}
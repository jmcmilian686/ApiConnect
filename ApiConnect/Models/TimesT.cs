using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiConnect.Models
{
    public class TimesT
    {
        public string TimeStamp {
            get {

               return DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");
                
            }
           
        }
    }
}
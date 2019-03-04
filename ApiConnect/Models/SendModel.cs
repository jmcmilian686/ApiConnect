using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiConnect.Models
{
    public class SendModel
    {
        public int OrdQty { get; set; }

        public int Timer { get; set; }

        public List<OrdMess> Messages { get; set; }
    }
}
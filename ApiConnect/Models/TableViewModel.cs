using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiConnect.Models
{
    public class TableViewModel
    {

        public int ID { get; set; }

        public List<Elements> DataValue { get; set; }
    }
}
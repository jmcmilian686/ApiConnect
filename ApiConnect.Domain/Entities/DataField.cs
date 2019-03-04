using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiConnect.Domain.Entities
{
    public class DataField
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public int? Link_P { get; set; }

        public int? Link_S { get; set; }

        public int? FieldID { get; set; }

        public virtual Field Field { get; set; }
    }
}
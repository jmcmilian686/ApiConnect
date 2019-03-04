using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApiConnect.Domain.Entities
{
    public class Field
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Field Name")]
        public string Name { get; set; }

        public int Counter { get; set; }

        public int Level { get; set; }

        public bool UniqueV { get; set; }

        public bool HasData { get; set; }

        public bool Mandatory { get; set; }

        [Display(Name = "Field Hierarchy")]
        public int FLevel { get; set; }

        ICollection<DataField> DataField { get; set; }
    }
}
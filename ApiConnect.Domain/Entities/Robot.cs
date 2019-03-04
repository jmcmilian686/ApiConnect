using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApiConnect.Domain.Entities
{
    public class Robot
    {
        public int ID { get; set; }

        [Required]
        public int Name { get; set; }

        public string IP { get; set; }
    }
}
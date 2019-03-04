using ApiConnect.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ApiConnect.Domain.Concrete
{
    public class EFDbContext : DbContext
    {
        public DbSet<Field> Fields { get; set; }
        public DbSet<DataField> DataFields { get; set; }
        public DbSet<Robot> Robots { get; set; }
    }
}
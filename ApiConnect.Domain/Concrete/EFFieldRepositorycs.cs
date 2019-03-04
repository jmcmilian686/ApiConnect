using ApiConnect.Domain.Abstract;
using ApiConnect.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiConnect.Domain.Concrete
{
    public class EFFieldRepository : IFieldsRepository
    {
        //Entity Framework context
        private EFDbContext context = new EFDbContext();

        // Get Method Implementation
        public IQueryable<Field> Fields
        {

            get { return context.Fields; }
        }

        // Save Method Implementation
        public void SaveField(Field field)
        {
            if (field.ID == 0)
            {
                context.Fields.Add(field);
            }
            else
            {
                Field fieldDb = context.Fields.Find(field.ID);
                if (fieldDb != null)
                {
                    fieldDb.Name = field.Name;
                    fieldDb.HasData = field.HasData;
                    fieldDb.Counter = field.Counter;
                    fieldDb.Level = field.Level;
                    fieldDb.FLevel = field.FLevel;
                    fieldDb.UniqueV = field.UniqueV;
                    fieldDb.Mandatory = field.Mandatory;
                   
                }
            }
            context.SaveChanges();
        }

        // Delete Method Implementation
        public Field DeleteField(int id)
        {
            Field fieldDb = context.Fields.Find(id);
            if (fieldDb != null)
            {
                context.Fields.Remove(fieldDb);
            }

            context.SaveChanges();

            return fieldDb;


        }
    }
}
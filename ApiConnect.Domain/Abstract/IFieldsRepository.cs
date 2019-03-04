using ApiConnect.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiConnect.Domain.Abstract
{
    public interface IFieldsRepository
    {

        IQueryable<Field> Fields { get; }

        void SaveField(Field field);

        Field DeleteField(int id);
    }
}
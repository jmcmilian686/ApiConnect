using ApiConnect.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiConnect.Domain.Abstract
{
    public interface IDataFieldRepository
    {

        IQueryable<DataField> DataFields { get; }

        void SaveDataField(DataField datafield);

        DataField DeleteDataField(int id);
    }
}
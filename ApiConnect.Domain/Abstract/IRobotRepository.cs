using ApiConnect.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiConnect.Domain.Abstract
{
    public interface IRobotRepository
    {
        IQueryable<Robot> Robots { get; }

        void SaveRobot(Robot robot);

        Robot DeleteRobot(int id);
    }
}
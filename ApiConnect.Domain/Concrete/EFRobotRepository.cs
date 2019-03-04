using ApiConnect.Domain.Abstract;
using ApiConnect.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiConnect.Domain.Concrete
{
    public class EFRobotRepository : IRobotRepository
    {
        //Entity Framework context
        private EFDbContext context = new EFDbContext();

        // Get Method Implementation
        public IQueryable<Robot> Robots
        {
            get { return context.Robots; }
        }

        // Save Method Implementation
        public void SaveRobot(Robot robot)
        {
            if (robot.ID == 0)
            {
                context.Robots.Add(robot);
            }
            else
            {
                Robot robotDb = context.Robots.Find(robot.ID);
                if (robotDb != null)
                {
                    robotDb.Name = robot.Name;
                    robotDb.IP = robot.IP;
                  

                }
            }
            context.SaveChanges();
        }

        public Robot DeleteRobot(int id)
        {
            Robot robotDb = context.Robots.Find(id);
            if (robotDb != null)
            {
                context.Robots.Remove(robotDb);
            }

            context.SaveChanges();

            return robotDb;


        }
    }
}
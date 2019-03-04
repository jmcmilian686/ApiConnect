using ApiConnect.Domain.Abstract;
using ApiConnect.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ApiConnect.Controllers
{
    public class RobotsController : Controller
    {
        private IRobotRepository robotsRepo;

        public RobotsController(IRobotRepository robotsrepository)
        {
            this.robotsRepo = robotsrepository;

        }

        // GET: Robots
        public ActionResult Index()
        {
            return View();
        }

        // GET: Field/Details/5
        public ActionResult Details()
        {
            IEnumerable<Robot> robots = robotsRepo.Robots.ToList();
            return PartialView("_Table", robots);
        }

        // GET: Field/Create

        [HttpGet]
        public ActionResult Create()
        {


            Robot model = new Robot();
            return PartialView("_Create", model);


        }

        // POST: Field/Create
        [HttpPost]
        public ActionResult Create(Robot robot)
        {
            try
            {


                if (ModelState.IsValid)
                {


                    if (robot.ID > 0)
                    {
                        ViewBag.Action = "Edited";
                    }
                    else
                    {
                        ViewBag.Action = "Created";
                    }

                    robotsRepo.SaveRobot(robot);
                    return PartialView("_RobotEditSuccess");
                }
                else
                {
                    return PartialView("_Create", robot);
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Server side error occurred", ex.Message.ToString());
                return PartialView("_Create", robot);
            }
        }

        // GET: Field/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                if (id >= 0)
                {
                    Robot robot = robotsRepo.Robots.Where(f => f.ID == id).FirstOrDefault();
                    if (robot != null)
                    {
                        return PartialView("_Create", robot);
                    }

                }
                return PartialView("_Create");

            }
            catch (Exception ex)
            {

                return View(ex.Message);

            }


        }



        // GET: Field/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                if (id >= 0)
                {
                    Robot robot = robotsRepo.Robots.Where(f => f.ID == id).FirstOrDefault();
                    if (robot != null)
                    {
                        robotsRepo.DeleteRobot(robot.ID);
                    }

                }
                return Json("ok");

            }
            catch (Exception ex)
            {

                return View(ex.Message);

            }
        }

        
    }
}
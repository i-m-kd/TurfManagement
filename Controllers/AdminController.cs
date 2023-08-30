using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using TurfManagement.ConnectionHelper;
using TurfManagement.Models;

namespace TurfManagement.Controllers
{
    [Authorize] // Ensure only admin users can access this controller
    public class AdminController : Controller
    {
        private readonly ConnectionHelper.AdminHelper _adminHelper;

        public AdminController()
        {
            _adminHelper = new AdminHelper();
        }

        public ActionResult Index()
        {
            return View();
        }

        #region AddNewTurf
        [HttpPost]
        public ActionResult AddTurf(TurfModel turf)
        {
            int result = _adminHelper.AddTurf(turf);

            if (result == 0)
            {
                TempData["TurfSuccessMessage"] = "Turf added successfully";
            }
            else if (result == -1)
            {
                TempData["TurfErrorMessage"] = "Turf with the same name and location already exists.";
            }
            else
            {
                TempData["TurfErrorMessage"] = "An error occurred while adding the turf.";
            }

            return RedirectToAction("Index");
        } 
        #endregion

        [HttpPost]
        public ActionResult AddSport(SportModel sport)
        {
            int result = _adminHelper.AddSport(sport);

            if (result == 0)
            {
                TempData["SportSuccessMessage"] = "Sport added successfully";
            }
            else if (result == -1)
            {
                TempData["SportErrorMessage"] = "Sport with the same name already exists for this turf.";
            }
            else
            {
                TempData["SportErrorMessage"] = "An error occurred while adding the sport.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddTimeSlot(TimeSlotModel timeSlot)
        {
            int result = _adminHelper.AddTimeSlot(timeSlot);

            if (result == 0)
            {
                TempData["TimeslotSuccessMessage"] = "Timeslot added successfully";
            }
            else if(result == -1)
            {
                TempData["TimeslotErrorMessage"] = "Timeslot already exist for same Sport.";
            }
            else
            {
                TempData["TimeslotErrorMessage"] ="An error occured during adding the timeslot";
            }
            return RedirectToAction("Index");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TurfManagement.ConnectionHelper;
using TurfManagement.Models;

namespace TurfManagement.Controllers
{
    [Authorize]
    public class TurfController : Controller
    {
        private readonly BookingHelper _bookingHelper;

        public TurfController()
        {
            _bookingHelper = new BookingHelper();
        }

        #region ListTurfName
        public ActionResult ListTurf()
        {
            List<TurfModel> turfs = _bookingHelper.GetTurf();

            TurfModel turfView = new TurfModel
            {
                Turfs = turfs,
                SelectedTurfId = 0
            };
            string userEmail = User.Identity.Name;
            int userId = _bookingHelper.GetUserIdByEmail(userEmail);

            ViewBag.UserId = userId;
            return View(turfView);
        }
        #endregion

        [HttpPost]
        public ActionResult ListTurf(TurfModel model)
        {
            int SelectedTurfId = model.SelectedTurfId;
            return View(model);
        }

        [HttpGet]
        public ActionResult GetSports(int turfId)
        {
            List<SportModel> sports = _bookingHelper.GetSport(turfId);
            return Json(sports, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAvailableTimeSlots(int turfId, int sportId, DateTime date)
        {
            Debug.WriteLine($"GetAvailableTimeSlots called with turfId: {turfId}, sportId: {sportId}");

            List<TimeSlotModel> timeSlots = _bookingHelper.GetAvailableTimeSlots(turfId, sportId,date);
            return Json(timeSlots, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult BookTimeSlots(List<int> selectedTimeSlots, int userId, DateTime bookingDate, int turfId)
        {
            bool success = _bookingHelper.BookTimeSlots(selectedTimeSlots, userId,turfId, bookingDate);
            return Json(new { success = success });
        }

    }
}

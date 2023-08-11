using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TurfManagement.ConnectionHelper;
using TurfManagement.Models;

namespace TurfManagement.Controllers
{
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
    }
}

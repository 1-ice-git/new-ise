using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.dtObj;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewISE.Models.ViewModel;
using NewISE.EF;
using System.Web.Script.Serialization;

namespace NewISE.Models
{
    public class CalendariEventiController : Controller
    {
        // GET: CalendariEventi
        bool admin = false;
        public ActionResult Index()
        {
            try
            {
                admin = Utility.Amministratore();
                ViewBag.Amministratore = admin;
            }
            catch (Exception)
            {
                return View("Error");
            }
            return View();
        }
        public ActionResult GetCalendarioEvento(DateTime? DataInizio)
        {
            DateTime? myDate;
            if (DataInizio.HasValue)
                myDate = DataInizio;
            else myDate = null;
            CalendarioEventiModel obj = new CalendarioEventiModel();
            //obj.DataInizioEvento = (DateTime)myDate;
            return PartialView(obj);
        }

        //[HttpPost]
        public JsonResult GetDiaryEvents(DateTime start, DateTime end)
        {
            List<CalendarViewModel> tmp = new List<CalendarViewModel>();
            try
            {
                using (dtCalendarioEventi dtcal = new dtCalendarioEventi())
                {
                    for (DateTime i = start; i < end; i = i.AddDays(1))
                    {
                        tmp.AddRange(dtcal.GetConteggioStatiAttivita(i));
                    }
                }

            }
            catch
            {
                return null;
            }


            var eventList = from e in tmp
                            select new
                            {
                                id = e.id,
                                title = e.title,
                                start = e.start,
                                end = e.end,
                                color = e.color,
                                someKey = 1,
                                // allDay = false
                            };
            var rows = eventList.ToArray();



            return Json(rows, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetDetailsCalendarEvents(DateTime DataInizio, string stato)
        {

            List<ElencoElementiHome> tmp = new List<ElencoElementiHome>();
            try
            {
                using (dtCalendarioEventi dtcal = new dtCalendarioEventi())
                {
                    tmp.AddRange(dtcal.GetDetailsCalendarEvents(DataInizio, stato));
                }
            }
            catch
            {
                return null;
            }
            return PartialView(tmp);
        }
    }
}
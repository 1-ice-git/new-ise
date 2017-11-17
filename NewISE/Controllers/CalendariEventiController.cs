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
        //public string Init()
        //{
        //    bool rslt = Utils.InitialiseDiary();
        //    return rslt.ToString();
        //}
        
        public JsonResult GetDiaryEvents(DateTime start)
        {
            List<CalendarViewModel> tmp = new List<CalendarViewModel>();
                       try
            {
                using (dtCalendarioEventi dtcal = new dtCalendarioEventi())
                {
                    tmp = dtcal.GetConteggioStatiAttivita(start);
                }
            }
            catch 
            {
                return null;
            }
            //CalendarViewModel cvm = new CalendarViewModel()
            //{
            //    id = 1,
            //    title = "Prova 1",
            //    start = Convert.ToDateTime("2017-11-04"),               
            //};
            //            CalendarViewModel cvm2 = new CalendarViewModel()
            //{
            //    id = 2,
            //    title = "Prova 2",
            //    start = Convert.ToDateTime("12/11/2017"),            
            //};
            //lcvm.Add(cvm);
            //lcvm.Add(cvm2);          
            var eventList = from e in tmp
                            select new CalendarViewModel
                            {
                                id = e.id,
                                title = e.title,
                               // Attivi = e.Attivi == null ? "" : e.Attivi,
                               // Completati = e.Completati==null?"": e.Completati,
                                start = e.start,
                                //end=e.end,
                               // Scaduti = e.Scaduti == null ? "" : e.Scaduti,
                            };                            
            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);            
        }
    }
}
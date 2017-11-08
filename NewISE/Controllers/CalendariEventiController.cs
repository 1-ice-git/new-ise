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

namespace NewISE.Models
{
    public class CalendariEventiController : Controller
    {
        // GET: CalendariEventi
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetCalendarioEvento(DateTime? DataInizio)
        {
             DateTime? myDate;
            if (DataInizio.HasValue)
                myDate = DataInizio;
            else myDate = null;
            CalendarioEventiModel obj = new CalendarioEventiModel();
            obj.DataInizioEvento = (DateTime)myDate;
            return PartialView(obj);
        }
        
    }
}
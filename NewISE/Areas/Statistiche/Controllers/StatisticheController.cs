
//using NewISE.Areas.Statistiche.Models.dtObj;
using NewISE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NewISE.Areas.Statistiche.Controllers
{
    public class StatisticheController : Controller
    {
        // GET: Statistiche/Statistiche
        public ActionResult Index()
        {
            
            return View("Index");

        }
        
    }
}
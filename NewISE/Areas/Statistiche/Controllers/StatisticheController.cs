using Microsoft.Reporting.WebForms;
using NewISE.Areas.Statistiche.Models;
using NewISE.Areas.Statistiche.Models.dtObj;
using NewISE.Areas.Statistiche.RPTDataSet;
using NewISE.EF;
using NewISE.Models;
using NewISE.Models.DBModel.dtObj;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace NewISE.Areas.Statistiche.Controllers
{
    public class StatisticheController : Controller
    {
        // GET: Statistiche/Statistiche
        [Authorize(Roles = "1 ,2")]
        public ActionResult Index()
        {
            return View("Index");
        }


        [Authorize(Roles = "1 ,2")]
        public ActionResult AttivitaStatistiche()
        {
            return PartialView();
        }

    }
}
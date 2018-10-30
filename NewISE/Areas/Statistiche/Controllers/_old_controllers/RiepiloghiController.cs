using Microsoft.Reporting.WebForms;
using NewISE.Areas.Statistiche.Models;
using NewISE.Areas.Statistiche.Models.dtObj;
using NewISE.Areas.Statistiche.RPTDataSet;
using NewISE.Models;
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
    public class RiepiloghiController : Controller
    {
        // GET: Statistiche/Riepiloghi
        public ActionResult Index()
        {

            var t = new List<SelectListItem>();

            try
            {
                t.Add(new SelectListItem() { Text = "", Value = "", Selected = true });
                t.Add(new SelectListItem() { Text = "ISE Mensile", Value = "0" });
                t.Add(new SelectListItem() { Text = "Maggiorazione Abitazione", Value = "1" });
                t.Add(new SelectListItem() { Text = "Altre Spese", Value = "2" });
                t.Add(new SelectListItem() { Text = "Altre Indennità", Value = "3" });

                ViewBag.Riepiloghi = t;
                return PartialView();

            }

            catch (Exception ex)
            {
                HandleErrorInfo her = new HandleErrorInfo(ex, "Riepiloghi", "Index");

                return View("Error", her);

            }


        }

        public ActionResult IseMensile()
        {
            return View();
        }

        public ActionResult MaggAbitazione()
        {
            return View();
        }

        public ActionResult AltreIndennita()
        {
            return View();
        }

        public ActionResult AltreSpese()
        {
            return View();
        }
    }
}
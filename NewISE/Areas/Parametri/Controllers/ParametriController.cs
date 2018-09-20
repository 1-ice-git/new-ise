using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParametriController : Controller
    {

        [HttpGet]
        [Authorize(Roles = "1 ,2")]
        public ActionResult Index()
        {
            var r = new List<SelectListItem>();
            try
            {
                r.Add(new SelectListItem() { Text = "", Value = "", Selected = true });
                r.Add(new SelectListItem() { Text = "Aliquote Contributive", Value = "0" });
                r.Add(new SelectListItem() { Text = "Percentuale Chilometrica", Value = "1" });
                r.Add(new SelectListItem() { Text = "Coefficiente di Richiamo", Value = "2" });
                r.Add(new SelectListItem() { Text = "Coefficiente di Sede", Value = "3" });
                r.Add(new SelectListItem() { Text = "Indennita Base", Value = "4" });
                r.Add(new SelectListItem() { Text = "Indennità Primo Segretario", Value = "5" });
                r.Add(new SelectListItem() { Text = "Indennità Sistemazione", Value = "6" });
                r.Add(new SelectListItem() { Text = "Maggiorazioni Annuali", Value = "7" });
                r.Add(new SelectListItem() { Text = "Percentuale Maggiorazione Abitazione", Value = "8" });
                r.Add(new SelectListItem() { Text = "Percentuale Maggiorazione Coniuge", Value = "9" });
                r.Add(new SelectListItem() { Text = "Percentuale Maggiorazione Figli", Value = "10" });
                r.Add(new SelectListItem() { Text = "Riduzioni", Value = "11" });
                r.Add(new SelectListItem() { Text = "TFR", Value = "12" });
                r.Add(new SelectListItem() { Text = "Valute", Value = "13" });
                r.Add(new SelectListItem() { Text = "Valuta Ufficio", Value = "14" });
                r.Add(new SelectListItem() { Text = "Percentuale Disagio", Value = "15" });
                r.Add(new SelectListItem() { Text = "Percentuale Anticipo TE", Value = "16" });
                r.Add(new SelectListItem() { Text = "Percentuale Condivisione MAB", Value = "17" });

                r = r.OrderBy(x => x.Text).ToList();

                ViewBag.ParametriList = r;
                return View();
            }
            catch (Exception ex)
            {
                HandleErrorInfo her = new HandleErrorInfo(ex, "Parametri", "index");

                return View("Error", her);
            }
        }
    }
}
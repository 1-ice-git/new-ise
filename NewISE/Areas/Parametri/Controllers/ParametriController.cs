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
                //r.Add(new SelectListItem() { Text = "", Value= "", Selected = true});
                //r.Add(new SelectListItem() { Text = "Indennita Base", Value = "0" });
                //r.Add(new SelectListItem() { Text = "Riduzioni", Value = "1" });
                //r.Add(new SelectListItem() { Text = "Coefficente di Sede", Value = "2" });
                //r.Add(new SelectListItem() { Text = "Percentuale di Disagio", Value = "3" });
                //r.Add(new SelectListItem() { Text = "Maggiorazione Coniuge", Value = "4" });
                //r.Add(new SelectListItem() { Text = "Maggiorazione Figli", Value = "5" });
                //r.Add(new SelectListItem() { Text = "Indennità Primo Segretario", Value = "6" });
                //r.Add(new SelectListItem() { Text = "Indennità Sistemazione", Value = "7" });
                //r.Add(new SelectListItem() { Text = "Aliquote Contributive", Value = "8" });
                //r.Add(new SelectListItem() { Text = "Percentuale Maggiorazione Abitazione", Value = "9" });
                //r.Add(new SelectListItem() { Text = "Indennità di Richiamo", Value = "10" });
                //r.Add(new SelectListItem() { Text = "Valute", Value = "11" });
                //r.Add(new SelectListItem() { Text = "Fascia Chilometrica", Value = "12" });
                //r.Add(new SelectListItem() { Text = "Coefficente Fascia Chilometrica", Value = "13" });
                //r.Add(new SelectListItem() { Text = "Maggiorazioni Annuali", Value = "14" });

                r.Add(new SelectListItem() { Text = "", Value = "", Selected = true });
                r.Add(new SelectListItem() { Text = "Aliquote Contributive", Value = "0" });
                r.Add(new SelectListItem() { Text = "Coefficente di Fascia Chilometrica", Value = "1" });
                r.Add(new SelectListItem() { Text = "Coefficente di Richiamo", Value = "2" });
                r.Add(new SelectListItem() { Text = "Coefficente di Sede", Value = "3" });
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

                ViewBag.ParametriList = r;
                return View();

            }
            catch (Exception ex)
            {
                return View("Error");

            }
            
        }

        
    }
}
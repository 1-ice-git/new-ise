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
                r.Add(new SelectListItem() { Text = "", Value= "", Selected = true});
                r.Add(new SelectListItem() { Text = "Indennita Base", Value = "0" });
                r.Add(new SelectListItem() { Text = "Riduzioni", Value = "1" });
                r.Add(new SelectListItem() { Text = "Coefficente di Sede", Value = "2" });
                r.Add(new SelectListItem() { Text = "Percentuale di Disagio", Value = "3" });
                r.Add(new SelectListItem() { Text = "Maggiorazione Coniuge", Value = "4" });
                r.Add(new SelectListItem() { Text = "Maggiorazione Figli", Value = "5" });
                r.Add(new SelectListItem() { Text = "Indennità Primo Segretario", Value = "6" });
                r.Add(new SelectListItem() { Text = "Indennità Sistemazione", Value = "7" });
                r.Add(new SelectListItem() { Text = "Aliquote Contributive", Value = "8" });
                r.Add(new SelectListItem() { Text = "Percentuale Maggiorazione Abitazione", Value = "9" });
                r.Add(new SelectListItem() { Text = "Indennità di Richiamo", Value = "10" });
                r.Add(new SelectListItem() { Text = "Valute", Value = "11" });
                r.Add(new SelectListItem() { Text = "Fascia Chilometrica", Value = "12" });
                r.Add(new SelectListItem() { Text = "Coefficente Fascia Chilometrica", Value = "13" });
                r.Add(new SelectListItem() { Text = "Maggiorazioni Annuali", Value = "14" });

                ViewBag.ParametriList = r;
                return View();

            }
            catch (Exception ex)
            {
                return View("Error");

            }
            
        }

        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public ActionResult IndennitaBase(bool escludiAnnullati)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

            try
            {
                

                using (dtIndennitaBase dtib=new dtIndennitaBase())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListIndennitaBase(escludiAnnullati).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListIndennitaBase().OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }

            return PartialView(libm);
        }

        [HttpGet]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaIndennitaBase()
        {
            var r = new List<SelectListItem>();

            using (dtLivelli dtl=new dtLivelli())
            {
                var llm = dtl.GetLivelli().ToList();

                if (llm != null && llm.Count > 0)
                {
                    r = (from t in llm
                         select new SelectListItem()
                         {
                             Text = t.DescLivello,
                             Value = t.idLivello.ToString()
                         }).ToList();
                    r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                }

                ViewBag.LivelliList = r;

            }

            return PartialView();
        }



    }
}
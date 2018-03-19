using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using NewISE.Models;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamPercCondivisioneMABController : Controller
    {
        // GET: Parametri/ParamPrimoIndSegr/PrimoSegretario
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult PercCondivisioneMAB(bool escludiAnnullati)
        {
            List<percCondivisioneMABModel> libm = new List<percCondivisioneMABModel>();
            var r = new List<SelectListItem>();
            List<percCondivisioneMABModel> llm = new List<percCondivisioneMABModel>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                using (dtCondivisioneMAB dtib = new dtCondivisioneMAB())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_IndCondivisioneMABAnnullato();
                    libm = dtib.getListCondivisioneMAB(escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewBag.escludiAnnullati = escludiAnnullati;
            return PartialView(libm);
        }

        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public ActionResult PercCondivisioneMABLivello(bool escludiAnnullati)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<percCondivisioneMABModel> libm = new List<percCondivisioneMABModel>();
            var r = new List<SelectListItem>();
            List<percCondivisioneMABModel> llm = new List<percCondivisioneMABModel>();

            try
            {
                using (dtCondivisioneMAB dtib = new dtCondivisioneMAB())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_IndCondivisioneMABAnnullato();
                    libm = dtib.getListCondivisioneMAB(escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    return PartialView("PercCondivisioneMAB", libm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaCondivisioneMab(bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                //using (dtCondivisioneMAB dtl = new dtCondivisioneMAB())
                //{
                //    var lm = dtl.getIndennitaPrimoSegretario(idIndPrimoSegr);
                //    ViewBag.idIndPrimoSegr = lm;
                //}
                ViewBag.escludiAnnullati = escludiAnnullati;
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult InserisciCondivisioneMAB(percCondivisioneMABModel ibm, bool escludiAnnullati = true,bool aggiornaTutto=false)
        {
            var r = new List<SelectListItem>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<percCondivisioneMABModel> libm = new List<percCondivisioneMABModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtCondivisioneMAB dtib = new dtCondivisioneMAB())
                    {
                        dtib.SetCondivisioneMAB(ibm,aggiornaTutto);
                    }
                    using (dtCondivisioneMAB dtib = new dtCondivisioneMAB())
                    {
                        ViewBag.idMinimoNonAnnullato = dtib.Get_Id_IndCondivisioneMABAnnullato();
                        libm = dtib.getListCondivisioneMAB(escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    return PartialView("PercCondivisioneMAB", libm);
                    //return RedirectToAction("PrimoSegretario", new { escludiAnnullati = escludiAnnullati, idIndPrimoSegr = ibm.idIndPrimoSegr });
                }
                else
                {                    
                    ViewBag.escludiAnnullati = escludiAnnullati;
                    return PartialView("NuovaCondivisioneMAB", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaCondivisioneMAB(bool escludiAnnullati, decimal idIndPrimoSegr)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<percCondivisioneMABModel> libm = new List<percCondivisioneMABModel>();
            try
            {
                using (dtCondivisioneMAB dtib = new dtCondivisioneMAB())
                {
                    dtib.DelIndennitaPrimoSegretario(idIndPrimoSegr);
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_IndCondivisioneMABAnnullato();
                    libm = dtib.getListCondivisioneMAB(escludiAnnullati).OrderBy(a=>a.dataFineValidita).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
                return PartialView("PercCondivisioneMAB", libm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
    }
}
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
    public class ParamPrimoIndSegrController : Controller
    {
        // GET: Parametri/ParamPrimoIndSegr/PrimoSegretario
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult PrimoSegretario(bool escludiAnnullati)//, decimal idLivello = 0)
        {
            List<IndennitaPrimoSegretModel> libm = new List<IndennitaPrimoSegretModel>();
            var r = new List<SelectListItem>();
            List<IndennitaPrimoSegretModel> llm = new List<IndennitaPrimoSegretModel>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                //using (dtIndPrimoSegr dtl = new dtIndPrimoSegr())
                //{
                //    llm = dtl.getIndennitaPrimoSegretario().OrderBy(a => a.indennita).ToList();

                //    if (llm != null && llm.Count > 0)
                //    {
                //        r = (from t in llm
                //             select new SelectListItem()
                //             {
                //                 Text = t.indennita.ToString(),
                //                 Value = t.idIndPrimoSegr.ToString()
                //             }).ToList();

                //        if (idLivello == 0)
                //        {
                //            r.First().Selected = true;
                //            idLivello = Convert.ToDecimal(r.First().Value);
                //        }
                //        else
                //        {
                //            r.Where(a => a.Value == idLivello.ToString()).First().Selected = true;
                //        }
                //    }

                //    ViewBag.PrimoSegretarioList = r;
                //}

                using (dtIndPrimoSegr dtib = new dtIndPrimoSegr())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_IndPrimoSegretarioNonAnnullato();
                    libm = dtib.getListIndennitaPrimoSegretario(escludiAnnullati).OrderBy(a => a.idIndPrimoSegr).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
        public ActionResult IndennitaPrimoSegretarioLivello(decimal idIndPrimoSegr, bool escludiAnnullati)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<IndennitaPrimoSegretModel> libm = new List<IndennitaPrimoSegretModel>();
            var r = new List<SelectListItem>();
            List<IndennitaPrimoSegretModel> llm = new List<IndennitaPrimoSegretModel>();

            try
            {
                using (dtIndPrimoSegr dtl = new dtIndPrimoSegr())
                {
                    llm = dtl.getIndennitaPrimoSegretario().OrderBy(a => a.indennita).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.indennita.ToString(),
                                 Value = t.idIndPrimoSegr.ToString()
                             }).ToList();
                        r.Where(a => a.Value == idIndPrimoSegr.ToString()).First().Selected = true;
                    }
                    ViewBag.PrimoSegretarioList = r;
                }
                using (dtIndPrimoSegr dtib = new dtIndPrimoSegr())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_IndPrimoSegretarioNonAnnullato();
                    libm = dtib.getListIndennitaPrimoSegretario(llm.Where(a => a.idIndPrimoSegr == idIndPrimoSegr).First().idIndPrimoSegr, escludiAnnullati).OrderBy(a => a.idIndPrimoSegr).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.escludiAnnullati = escludiAnnullati;
            return PartialView("PrimoSegretario", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaIndennitaPrimoSegretario(bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                //using (dtIndPrimoSegr dtl = new dtIndPrimoSegr())
                //{
                //    var lm = dtl.getIndennitaPrimoSegretario(idIndPrimoSegr);
                //    ViewBag.idIndPrimoSegr = lm;
                //}
                //ViewBag.escludiAnnullati = escludiAnnullati;
                return PartialView("NuovaIndennitaPrimoSegretario");
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult InserisciIndennitaPrimoSegretario(IndennitaPrimoSegretModel ibm, bool escludiAnnullati = true)
        {
            var r = new List<SelectListItem>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<IndennitaPrimoSegretModel> libm = new List<IndennitaPrimoSegretModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtIndPrimoSegr dtib = new dtIndPrimoSegr())
                    {
                        dtib.SetIndennitaPrimoSegretario(ibm);
                    }
                    using (dtIndPrimoSegr dtib = new dtIndPrimoSegr())
                    {
                        ViewBag.idMinimoNonAnnullato = dtib.Get_Id_IndPrimoSegretarioNonAnnullato();
                        libm = dtib.getListIndennitaPrimoSegretario(escludiAnnullati).OrderBy(a => a.idIndPrimoSegr).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    return PartialView("PrimoSegretario",libm);
                    //return RedirectToAction("PrimoSegretario", new { escludiAnnullati = escludiAnnullati, idIndPrimoSegr = ibm.idIndPrimoSegr });
                }
                else
                {
                    //using (dtIndPrimoSegr dtl = new dtIndPrimoSegr())
                    //{
                    //    var lm = dtl.getIndennitaPrimoSegretario(ibm.idIndPrimoSegr);
                    //    ViewBag.idIndPrimoSegr = lm;
                    //}
                    ViewBag.escludiAnnullati = escludiAnnullati;
                    return PartialView("NuovaIndennitaPrimoSegretario", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaIndennitaPrimoSegretario(bool escludiAnnullati, decimal idIndPrimoSegr)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<IndennitaPrimoSegretModel> libm = new List<IndennitaPrimoSegretModel>();
            try
            {
                using (dtIndPrimoSegr dtib = new dtIndPrimoSegr())
                {
                    dtib.DelIndennitaPrimoSegretario(idIndPrimoSegr);                
                    libm = dtib.getListIndennitaPrimoSegretario(escludiAnnullati).OrderBy(a => a.idIndPrimoSegr).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
                return PartialView("PrimoSegretario", libm);
                //return RedirectToAction("PrimoSegretario", new { escludiAnnullati = escludiAnnullati, idIndPrimoSegr = idIndPrimoSegr });
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
    }
}
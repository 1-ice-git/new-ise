using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
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
        public ActionResult PrimoSegretario(bool escludiAnnullati, decimal idIndPrimoSegr = 0)
        {
            List<IndennitaPrimoSegretModel> libm = new List<IndennitaPrimoSegretModel>();
            var r = new List<SelectListItem>();
            List<IndennitaPrimoSegretModel> llm = new List<IndennitaPrimoSegretModel>();

            try
            {
                using (dtParIndPrimoSegr dtl = new dtParIndPrimoSegr())
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

                        if (idIndPrimoSegr == 0)
                        {
                            r.First().Selected = true;
                            idIndPrimoSegr = Convert.ToDecimal(r.First().Value);
                        }
                        else
                        {
                            r.Where(a => a.Value == idIndPrimoSegr.ToString()).First().Selected = true;
                        }
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtParIndPrimoSegr dtib = new dtParIndPrimoSegr())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListIndennitaPrimoSegretario(idIndPrimoSegr, escludiAnnullati).OrderBy(a => a.idIndPrimoSegr).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListIndennitaPrimoSegretario(idIndPrimoSegr).OrderBy(a => a.idIndPrimoSegr).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }

            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView(libm);
        }

        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public ActionResult IndennitaPrimoSegretarioLivello(decimal idIndPrimoSegr, bool escludiAnnullati)
        {
            List<IndennitaPrimoSegretModel> libm = new List<IndennitaPrimoSegretModel>();
            var r = new List<SelectListItem>();
            List<IndennitaPrimoSegretModel> llm = new List<IndennitaPrimoSegretModel>();

            try
            {
                using (dtParIndPrimoSegr dtl = new dtParIndPrimoSegr())
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

                    ViewBag.LivelliList = r;
                }

                using (dtParIndPrimoSegr dtib = new dtParIndPrimoSegr())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListIndennitaPrimoSegretario(llm.Where(a => a.idIndPrimoSegr == idIndPrimoSegr).First().idIndPrimoSegr, escludiAnnullati).OrderBy(a => a.idIndPrimoSegr).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListIndennitaPrimoSegretario(llm.Where(a => a.idIndPrimoSegr == idIndPrimoSegr).First().idIndPrimoSegr).OrderBy(a => a.idIndPrimoSegr).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("PrimoSegretario", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaIndennitaPrimoSegretario(decimal idIndPrimoSegr, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();
            

            try
            {
                using (dtParIndPrimoSegr dtl = new dtParIndPrimoSegr())
                {
                    var lm = dtl.getIndennitaPrimoSegretario(idIndPrimoSegr);
                    ViewBag.idIndPrimoSegr = lm;
                }
                ViewBag.escludiAnnullati = escludiAnnullati;
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult InserisciIndennitaPrimoSegretario(IndennitaPrimoSegretModel ibm, bool escludiAnnullati = true)
        {
            var r = new List<SelectListItem>();

            try
            {
                if (ModelState.IsValid)
                {
                    using (dtParIndPrimoSegr dtib = new dtParIndPrimoSegr())
                    {
                        dtib.SetIndennitaPrimoSegretario(ibm);
                    }

                    return RedirectToAction("PrimoSegretario", new { escludiAnnullati = escludiAnnullati, idIndPrimoSegr = ibm.idIndPrimoSegr });
                }
                else
                {
                    using (dtParIndPrimoSegr dtl = new dtParIndPrimoSegr())
                    {
                        var lm = dtl.getIndennitaPrimoSegretario(ibm.idIndPrimoSegr);
                        ViewBag.idIndPrimoSegr = lm;
                    }
                    ViewBag.escludiAnnullati = escludiAnnullati;
                    return PartialView("NuovaIndennitaPrimoSegretario", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaIndennitaPrimoSegretario(bool escludiAnnullati, decimal idIndPrimoSegr)
        {

            try
            {
                using (dtParIndPrimoSegr dtib = new dtParIndPrimoSegr())
                {
                    dtib.DelIndennitaPrimoSegretario(idIndPrimoSegr);
                }

                return RedirectToAction("PrimoSegretario", new { escludiAnnullati = escludiAnnullati, idIndPrimoSegr = idIndPrimoSegr });
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial");
            }


        }
    }
}
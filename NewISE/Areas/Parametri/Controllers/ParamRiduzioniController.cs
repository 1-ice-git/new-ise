using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamRiduzioniController : Controller
    {
        // GET: /Parametri/ParamRiduzioni/Riduzioni

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult Riduzioni(bool escludiAnnullati, decimal idRiduzioni = 0)
        {
            List<RiduzioniModel> libm = new List<RiduzioniModel>();
            var r = new List<SelectListItem>();
            List<RiduzioniModel> llm1 = new List<RiduzioniModel>();

            try
            {
                using (dtRiduzioni dtl = new dtRiduzioni())
                {
                    llm1 = dtl.GetRiduzioni().OrderBy(a => a.percentuale).ToList();

                    if (llm1 != null && llm1.Count > 0)
                    {
                        r = (from t in llm1
                             select new SelectListItem()
                             {
                                 Text = t.percentuale.ToString(),
                                 Value = t.idRiduzioni.ToString()
                                 
                             }).ToList();

                        if (idRiduzioni == 0)
                        {
                            r.First().Selected = true;
                            idRiduzioni = Convert.ToDecimal(r.First().Value);
                        }
                        else
                        {
                            r.Where(a => a.Value == idRiduzioni.ToString()).First().Selected = true;
                        }
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtRiduzioni dtib = new dtRiduzioni())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListRiduzioni(idRiduzioni, escludiAnnullati).OrderBy(a => a.idRiduzioni).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListRiduzioni(idRiduzioni).OrderBy(a => a.idRiduzioni).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
        public ActionResult RiduzioniLivello(decimal idRiduzioni, bool escludiAnnullati)
        {
            List<RiduzioniModel> libm = new List<RiduzioniModel>();
            var r = new List<SelectListItem>();
            List<RiduzioniModel> llm1 = new List<RiduzioniModel>();

            try
            {
                using (dtRiduzioni dtl = new dtRiduzioni())
                {
                    llm1 = dtl.GetRiduzioni().OrderBy(a => a.percentuale).ToList();

                    if (llm1 != null && llm1.Count > 0)
                    {
                        r = (from t in llm1
                             select new SelectListItem()
                             {
                               
                                 Text = t.percentuale.ToString(),
                                 Value = t.idRiduzioni.ToString()
                             }).ToList();
                        r.Where(a => a.Value == idRiduzioni.ToString()).First().Selected = true;
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtRiduzioni dtib = new dtRiduzioni())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListRiduzioni(llm1.Where(a => a.idRiduzioni == idRiduzioni).First().idRiduzioni, escludiAnnullati).OrderBy(a => a.idRiduzioni).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListRiduzioni(llm1.Where(a => a.idRiduzioni == idRiduzioni).First().idRiduzioni).OrderBy(a => a.idRiduzioni).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("Riduzioni", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuoveRiduzioni(decimal idRiduzioni, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();
          

            try
            {
                using (dtRiduzioni dtl = new dtRiduzioni())
                {
                    var lm = dtl.GetRiduzioni(idRiduzioni);
                    ViewBag.percentuale = lm;
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
        public ActionResult InserisciRiduzioni(RiduzioniModel ibm, bool escludiAnnullati = true)
        {
            var r = new List<SelectListItem>();

            try
            {
                if (ModelState.IsValid)
                {
                    using (dtRiduzioni dtib = new dtRiduzioni())
                    {
                        

                        dtib.SetRiduzioni(ibm);
                    }

                    return RedirectToAction("Riduzioni", new { escludiAnnullati = escludiAnnullati, idRiduzioni = ibm.idRiduzioni });
                }
                else
                {
                    using (dtRiduzioni dtl = new dtRiduzioni())
                    {
                        var lm = dtl.GetRiduzioni(ibm.idRiduzioni);
                        ViewBag.percentuale = lm;
                    }
                    ViewBag.escludiAnnullati = escludiAnnullati;
                    return PartialView("NuoveRiduzioni", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaRiduzioni(bool escludiAnnullati, decimal idRiduzioni)
        {

            try
            {
                using (dtRiduzioni dtib = new dtRiduzioni())
                {
                    dtib.DelRiduzioni(idRiduzioni);
                }

                return RedirectToAction("Riduzioni", new { escludiAnnullati = escludiAnnullati, idRiduzioni = idRiduzioni });
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial");
            }


        }
    }
}
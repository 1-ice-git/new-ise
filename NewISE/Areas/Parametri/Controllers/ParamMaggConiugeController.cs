using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamMaggConiugeController : Controller
    {
        // GET: Parametri/ParamMaggConiuge/MaggiorazioneConiuge
        
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult MaggiorazioneConiuge(bool escludiAnnullati, decimal idTipologiaConiuge = 0)
        {
            List<MaggiorazioneConiugeModel> libm = new List<MaggiorazioneConiugeModel>();
            var r = new List<SelectListItem>();
            List<TipologiaConiugeModel> llm = new List<TipologiaConiugeModel>();

            try
            {
                using (dtTipologiaConiuge dtl = new dtTipologiaConiuge())
                {
                    llm = dtl.GetTipologiaConiuge().OrderBy(a => a.tipologiaConiuge).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.tipologiaConiuge,
                                 Value = t.idTipologiaConiuge.ToString()
                             }).ToList();

                        if (idTipologiaConiuge == 0)
                        {
                            r.First().Selected = true;
                            idTipologiaConiuge = Convert.ToDecimal(r.First().Value);
                        }
                        else
                        {
                            r.Where(a => a.Value == idTipologiaConiuge.ToString()).First().Selected = true;
                        }
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtMaggConiuge dtib = new dtMaggConiuge())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListMaggiorazioneConiuge(idTipologiaConiuge, escludiAnnullati).OrderBy(a => a.idTipologiaConiuge).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListMaggiorazioneConiuge(idTipologiaConiuge).OrderBy(a => a.idTipologiaConiuge).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
        public ActionResult MaggiorazioneConiugeLivello(decimal idTipologiaConiuge, bool escludiAnnullati)
        {
            List<MaggiorazioneConiugeModel> libm = new List<MaggiorazioneConiugeModel>();
            var r = new List<SelectListItem>();
            List<TipologiaConiugeModel> llm = new List<TipologiaConiugeModel>();

            try
            {
                using (dtTipologiaConiuge dtl = new dtTipologiaConiuge())
                {
                    llm = dtl.GetTipologiaConiuge().OrderBy(a => a.tipologiaConiuge).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.tipologiaConiuge,
                                 Value = t.idTipologiaConiuge.ToString()
                             }).ToList();
                        r.Where(a => a.Value == idTipologiaConiuge.ToString()).First().Selected = true;
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtMaggConiuge dtib = new dtMaggConiuge())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListMaggiorazioneConiuge(llm.Where(a => a.idTipologiaConiuge == idTipologiaConiuge).First().idTipologiaConiuge, escludiAnnullati).OrderBy(a => a.idTipologiaConiuge).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListMaggiorazioneConiuge(llm.Where(a => a.idTipologiaConiuge == idTipologiaConiuge).First().idTipologiaConiuge).OrderBy(a => a.idTipologiaConiuge).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("MaggiorazioneConiuge", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaMaggiorazioneConiuge(decimal idTipologiaConiuge, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();

            try
            {
                using (dtTipologiaConiuge dtl = new dtTipologiaConiuge())
                {
                    var lm = dtl.GetTipologiaConiuge(idTipologiaConiuge);
                    ViewBag.Coniuge = lm;
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
        public ActionResult InserisciMaggiorazioneConiuge(MaggiorazioneConiugeModel ibm, bool escludiAnnullati = true)
        {
            var r = new List<SelectListItem>();

            try
            {
                if (ModelState.IsValid)
                {
                    using (dtMaggConiuge dtib = new dtMaggConiuge())
                    {
                        dtib.SetMaggiorazioneConiuge(ibm);
                    }

                    return RedirectToAction("MaggiorazioneConiuge", new { escludiAnnullati = escludiAnnullati, idTipologiaConiuge = ibm.idTipologiaConiuge });
                }
                else
                {
                    using (dtTipologiaConiuge dtl = new dtTipologiaConiuge())
                    {
                        var lm = dtl.GetTipologiaConiuge(ibm.idTipologiaConiuge);
                        ViewBag.Livello = lm;
                    }
                    ViewBag.escludiAnnullati = escludiAnnullati;
                    return PartialView("NuovaMaggiorazioneConiuge", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaMaggiorazioneConiuge(bool escludiAnnullati, decimal idTipologiaConiuge, decimal idMaggConiuge)
        {

            try
            {
                using (dtMaggConiuge dtib = new dtMaggConiuge())
                {
                    dtib.DelMaggiorazioneConiuge(idMaggConiuge);
                }

                return RedirectToAction("MaggiorazioneConiuge", new { escludiAnnullati = escludiAnnullati, idTipologiaConiuge = idTipologiaConiuge });
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial");
            }


        }



    }




}
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
    public class ParamMaggConiugeController : Controller
    {
        // GET: Parametri/ParamMaggConiuge/MaggiorazioniConiuge

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult PercMaggiorazioneConiuge(bool escludiAnnullati, decimal idTipologiaConiuge = 0)
        {
            List<PercentualeMagConiugeModel> libm = new List<PercentualeMagConiugeModel>();
            var r = new List<SelectListItem>();
            List<TipologiaConiugeModel> llm = new List<TipologiaConiugeModel>();

            try
            {
                using (dtParTipologiaConiuge dtl = new dtParTipologiaConiuge())
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

                using (dtParMaggConiuge dtib = new dtParMaggConiuge())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListPercMagConiuge(idTipologiaConiuge, escludiAnnullati).OrderBy(a => a.idTipologiaConiuge).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListPercMagConiuge(idTipologiaConiuge).OrderBy(a => a.idTipologiaConiuge).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
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
        public ActionResult PercMaggiorazioneConiugeLivello(decimal idTipologiaConiuge, bool escludiAnnullati)
        {
            List<PercentualeMagConiugeModel> libm = new List<PercentualeMagConiugeModel>();
            var r = new List<SelectListItem>();
            List<TipologiaConiugeModel> llm = new List<TipologiaConiugeModel>();

            try
            {
                using (dtParTipologiaConiuge dtl = new dtParTipologiaConiuge())
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

                using (dtParMaggConiuge dtib = new dtParMaggConiuge())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListPercMagConiuge(llm.Where(a => a.idTipologiaConiuge == idTipologiaConiuge).First().idTipologiaConiuge, escludiAnnullati).OrderBy(a => a.idTipologiaConiuge).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListPercMagConiuge(llm.Where(a => a.idTipologiaConiuge == idTipologiaConiuge).First().idTipologiaConiuge).OrderBy(a => a.idTipologiaConiuge).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("PercMaggiorazioneConiuge", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaPercMaggiorazioneConiuge(decimal idTipologiaConiuge, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();

            try
            {
                using (dtParTipologiaConiuge dtl = new dtParTipologiaConiuge())
                {
                    var lm = dtl.GetTipologiaConiuge(idTipologiaConiuge);
                    ViewBag.Coniuge = lm;
                }
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
        public ActionResult InserisciMaggiorazioneConiuge(PercentualeMagConiugeModel ibm, bool escludiAnnullati = true)
        {
            var r = new List<SelectListItem>();

            try
            {
                if (ModelState.IsValid)
                {
                    using (dtParMaggConiuge dtib = new dtParMaggConiuge())
                    {
                        dtib.SetPercMagConiuge(ibm);
                    }

                    return RedirectToAction("PercMaggiorazioneConiuge", new { escludiAnnullati = escludiAnnullati, idTipologiaConiuge = ibm.idTipologiaConiuge });
                }
                else
                {
                    using (dtParTipologiaConiuge dtl = new dtParTipologiaConiuge())
                    {
                        var lm = dtl.GetTipologiaConiuge((decimal)ibm.idTipologiaConiuge);
                        ViewBag.Livello = lm;
                    }
                    ViewBag.escludiAnnullati = escludiAnnullati;
                    return PartialView("NuovaPercMaggiorazioneConiuge", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaPercMaggiorazioneConiuge(bool escludiAnnullati, decimal idTipologiaConiuge, decimal idMaggConiuge)
        {

            try
            {
                using (dtParMaggConiuge dtib = new dtParMaggConiuge())
                {
                    dtib.DelPercMagConiuge(idMaggConiuge);
                }

                return RedirectToAction("PercMaggiorazioneConiuge", new { escludiAnnullati = escludiAnnullati, idTipologiaConiuge = idTipologiaConiuge });
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }



    }




}
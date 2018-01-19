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
        public ActionResult PercMaggiorazioneConiuge(bool escludiAnnullati, decimal idLivello = 0)
        {
            List<PercentualeMagConiugeModel> libm = new List<PercentualeMagConiugeModel>();
            var r = new List<SelectListItem>();
            List<TipologiaConiugeModel> llm = new List<TipologiaConiugeModel>();
            ViewBag.escludiAnnullati = escludiAnnullati;
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

                        if (idLivello == 0)
                        {
                            r.First().Selected = true;
                            idLivello = Convert.ToDecimal(r.First().Value);
                        }
                        else
                        {
                            var temp = r.Where(a => a.Value == idLivello.ToString()).ToList();
                            if (temp.Count == 0)
                            {
                                r.First().Selected = true;
                                idLivello = Convert.ToDecimal(r.First().Value);
                            }
                            else
                                r.Where(a => a.Value == idLivello.ToString()).First().Selected = true;
                        }
                    }
                    ViewBag.LivelliList = r;
                    
                }

                using (dtParMaggConiuge dtib = new dtParMaggConiuge())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_PercentualMagConiugePrimoNonAnnullato(idLivello);
                    libm = dtib.getListPercMagConiuge(idLivello, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

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
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_PercentualMagConiugePrimoNonAnnullato(idTipologiaConiuge);
                    libm = dtib.getListPercMagConiuge(idTipologiaConiuge, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                using (dtParTipologiaConiuge dtl = new dtParTipologiaConiuge())
                {
                    var lm = dtl.GetTipologiaConiuge(idTipologiaConiuge);
                    ViewBag.Coniuge = lm;
                }
               
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult InserisciMaggiorazioneConiuge(PercentualeMagConiugeModel ibm,decimal idTipologiaConiuge, bool escludiAnnullati = true)
        {
            var r = new List<SelectListItem>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<PercentualeMagConiugeModel> libm = new List<PercentualeMagConiugeModel>();
            List<TipologiaConiugeModel> llm = new List<TipologiaConiugeModel>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtMaggConiuge dtib = new dtMaggConiuge())
                    {
                        dtib.SetPercMagConiuge(ibm, idTipologiaConiuge);
                    }
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

                            r.Where(a => a.Value == idTipologiaConiuge.ToString()).First().Selected= true;
                        }
                        ViewBag.LivelliList = r;
                    }
                    using (dtParMaggConiuge dtib = new dtParMaggConiuge())
                    {
                        ViewBag.idMinimoNonAnnullato = dtib.Get_Id_PercentualMagConiugePrimoNonAnnullato(Convert.ToDecimal(idTipologiaConiuge));
                        libm = dtib.getListPercMagConiuge(Convert.ToDecimal(ibm.idTipologiaConiuge), escludiAnnullati).OrderBy(a => a.idTipologiaConiuge).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    return PartialView("PercMaggiorazioneConiuge",libm);
                   // return RedirectToAction("PercMaggiorazioneConiuge", new { escludiAnnullati = escludiAnnullati, idTipologiaConiuge = ibm.idTipologiaConiuge });
                }
                else
                {
                    using (dtParTipologiaConiuge dtl = new dtParTipologiaConiuge())
                    {
                        var lm = dtl.GetTipologiaConiuge((decimal)ibm.idTipologiaConiuge);
                        ViewBag.Coniuge = lm;
                    }
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
            var r = new List<SelectListItem>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<PercentualeMagConiugeModel> libm = new List<PercentualeMagConiugeModel>();
            List<TipologiaConiugeModel> llm = new List<TipologiaConiugeModel>();
            try
            {
                using (dtParMaggConiuge dtib = new dtParMaggConiuge())
                {
                    dtib.DelPercMagConiuge(idMaggConiuge);
                }
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
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_PercentualMagConiugePrimoNonAnnullato(Convert.ToDecimal(idTipologiaConiuge));
                    libm = dtib.getListPercMagConiuge(Convert.ToDecimal(idTipologiaConiuge), escludiAnnullati).OrderBy(a => a.idTipologiaConiuge).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
                return PartialView("PercMaggiorazioneConiuge", libm);
                //return RedirectToAction("PercMaggiorazioneConiuge", new { escludiAnnullati = escludiAnnullati, idTipologiaConiuge = idTipologiaConiuge });
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
    }
}
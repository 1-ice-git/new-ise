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
    public class ParamMaggFigliController : Controller
    {
        // GET: Parametri/ParamMaggFigli/MaggiorazioneFigli

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]

        public ActionResult MaggiorazioneFigli(bool escludiAnnullati, decimal idTipologiaFiglio = 0)
        {
            List<PercentualeMagFigliModel> libm = new List<PercentualeMagFigliModel>();
            var r = new List<SelectListItem>();
            List<TipologiaFiglioModel> llm = new List<TipologiaFiglioModel>();

            try
            {
                using (dtParTipologiaFiglio dtl = new dtParTipologiaFiglio())
                {
                    llm = dtl.GetTipologiaFiglio().OrderBy(a => a.tipologiaFiglio).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.tipologiaFiglio,
                                 Value = t.idTipologiaFiglio.ToString()
                             }).ToList();

                        if (idTipologiaFiglio == 0)
                        {
                            r.First().Selected = true;
                            idTipologiaFiglio = Convert.ToDecimal(r.First().Value);
                        }
                        else
                        {
                            r.Where(a => a.Value == idTipologiaFiglio.ToString()).First().Selected = true;
                        }
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtParMaggFigli dtib = new dtParMaggFigli())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListMaggiorazioneFiglio(idTipologiaFiglio, escludiAnnullati).OrderBy(a => a.idTipologiaFiglio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListMaggiorazioneFiglio(idTipologiaFiglio).OrderBy(a => a.idTipologiaFiglio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
        public ActionResult MaggiorazioneFiglioLivello(decimal idTipologiaFiglio, bool escludiAnnullati)
        {
            List<PercentualeMagFigliModel> libm = new List<PercentualeMagFigliModel>();
            var r = new List<SelectListItem>();
            List<TipologiaFiglioModel> llm = new List<TipologiaFiglioModel>();

            try
            {
                using (dtParTipologiaFiglio dtl = new dtParTipologiaFiglio())
                {
                    llm = dtl.GetTipologiaFiglio().OrderBy(a => a.tipologiaFiglio).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.tipologiaFiglio,
                                 Value = t.idTipologiaFiglio.ToString()
                             }).ToList();
                        r.Where(a => a.Value == idTipologiaFiglio.ToString()).First().Selected = true;
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtParMaggFigli dtib = new dtParMaggFigli())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListMaggiorazioneFiglio(llm.Where(a => a.idTipologiaFiglio == idTipologiaFiglio).First().idTipologiaFiglio, escludiAnnullati).OrderBy(a => a.idTipologiaFiglio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListMaggiorazioneFiglio(llm.Where(a => a.idTipologiaFiglio == idTipologiaFiglio).First().idTipologiaFiglio).OrderBy(a => a.idTipologiaFiglio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("MaggiorazioneFigli", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaMaggiorazioneFiglio(decimal idTipologiaFiglio, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();

            try
            {
                using (dtParTipologiaFiglio dtl = new dtParTipologiaFiglio())
                {
                    var lm = dtl.GetTipologiaFiglio(idTipologiaFiglio);
                    ViewBag.Figlio = lm;
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
        public ActionResult InserisciMaggiorazioneFiglio(PercentualeMagFigliModel ibm, bool escludiAnnullati = true)
        {
            var r = new List<SelectListItem>();

            try
            {
                if (ModelState.IsValid)
                {
                    using (dtParMaggFigli dtib = new dtParMaggFigli())
                    {
                        dtib.SetMaggiorazioneFiglio(ibm);
                    }

                    return RedirectToAction("MaggiorazioneFigli", new { escludiAnnullati = escludiAnnullati, idTipologiaFiglio = ibm.idTipologiaFiglio });
                }
                else
                {
                    using (dtParTipologiaFiglio dtl = new dtParTipologiaFiglio())
                    {
                        var lm = dtl.GetTipologiaFiglio((decimal)ibm.idTipologiaFiglio);
                        ViewBag.Figlio = lm;
                    }
                    ViewBag.escludiAnnullati = escludiAnnullati;
                    return PartialView("NuovaMaggiorazioneFiglio", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaMaggiorazioneFiglio(bool escludiAnnullati, decimal idTipologiaFiglio, decimal idMaggFiglio)
        {

            try
            {
                using (dtParMaggFigli dtib = new dtParMaggFigli())
                {
                    dtib.DelMaggiorazioneFiglio(idMaggFiglio);
                }

                return RedirectToAction("MaggiorazioneFigli", new { escludiAnnullati = escludiAnnullati, idTipologiaFiglio = idTipologiaFiglio });
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }
    }
}
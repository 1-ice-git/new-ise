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
    public class ParamRiduzioniController : Controller
    {
        // GET: /Parametri/ParamRiduzioni/Riduzioni

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult Riduzioni(bool escludiAnnullati, decimal idRegola = 0)
        {
            List<RiduzioniModel> libm = new List<RiduzioniModel>();
            var r = new List<SelectListItem>();
            List<RegoleCalcoloModel> llm = new List<RegoleCalcoloModel>();

            try
            {
                using (dtRegoleCalcolo dtl = new dtRegoleCalcolo())
                {
                    llm = dtl.GetRegoleCalcolo().OrderBy(a => a.formulaRegolaCalcolo).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.formulaRegolaCalcolo,
                                 Value = t.idRegola.ToString()
                             }).ToList();

                        if (idRegola == 0)
                        {
                            r.First().Selected = true;
                            idRegola = Convert.ToDecimal(r.First().Value);
                        }
                        else
                        {
                            r.Where(a => a.Value == idRegola.ToString()).First().Selected = true;
                        }
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtRiduzioni dtib = new dtRiduzioni())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListRiduzioni(idRegola, escludiAnnullati).OrderBy(a => a.idRegola).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListRiduzioni(idRegola).OrderBy(a => a.idRegola).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
        public ActionResult RiduzioniLivello(decimal idRegola, bool escludiAnnullati)
        {
            List<RiduzioniModel> libm = new List<RiduzioniModel>();
            var r = new List<SelectListItem>();
            List<RegoleCalcoloModel> llm = new List<RegoleCalcoloModel>();

            try
            {
                using (dtRegoleCalcolo dtl = new dtRegoleCalcolo())
                {
                    llm = dtl.GetRegoleCalcolo().OrderBy(a => a.formulaRegolaCalcolo).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.formulaRegolaCalcolo,
                                 Value = t.idRegola.ToString()
                             }).ToList();
                        r.Where(a => a.Value == idRegola.ToString()).First().Selected = true;
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtRiduzioni dtib = new dtRiduzioni())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListRiduzioni(llm.Where(a => a.idRegola == idRegola).First().idRegola, escludiAnnullati).OrderBy(a => a.idRegola).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListRiduzioni(llm.Where(a => a.idRegola == idRegola).First().idRegola).OrderBy(a => a.idRegola).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
        public ActionResult NuoveRiduzioni(decimal idRegola, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();

            try
            {
                using (dtRegoleCalcolo dtl = new dtRegoleCalcolo())
                {
                    var lm = dtl.GetRegoleCalcolo(idRegola);
                    ViewBag.FormulaRegolaCalcolo = lm;
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
        public ActionResult InserisciRiduzione(RiduzioniModel ibm, bool escludiAnnullati = true)
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

                    return RedirectToAction("Riduzioni", new { escludiAnnullati = escludiAnnullati, idRegola = ibm.idRegola });
                }
                else
                {
                    using (dtRegoleCalcolo dtl = new dtRegoleCalcolo())
                    {
                        var lm = dtl.GetRegoleCalcolo(ibm.idRegola);
                        ViewBag.FormulaRegolaCalcolo = lm;
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
        public ActionResult EliminaRiduzione(bool escludiAnnullati, decimal idRegola, decimal idRiduzioni)
        {

            try
            {
                using (dtRiduzioni dtib = new dtRiduzioni())
                {
                    dtib.DelRiduzioni(idRiduzioni);
                }

                return RedirectToAction("Riduzioni", new { escludiAnnullati = escludiAnnullati, idRegola = idRegola });
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial");
            }


        }


    }
}
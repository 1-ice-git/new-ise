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
        decimal CaricaComboTipoFiglio(decimal idLivello)
        {
            var r = new List<SelectListItem>();
            List<TipologiaFiglioModel> llm = new List<TipologiaFiglioModel>();
            using (dtParTipologiaFiglio dtl = new dtParTipologiaFiglio())
            {
                llm = dtl.GetTipologiaFiglio().OrderBy(a => a.idTipologiaFiglio).ToList();

                if (llm != null && llm.Count > 0)
                {
                    r = (from t in llm
                         select new SelectListItem()
                         {
                             Text = t.tipologiaFiglio,
                             Value = t.idTipologiaFiglio.ToString()
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
            return idLivello;
        }
        public ActionResult MaggiorazioneFigli(bool escludiAnnullati, decimal idLivello = 0)
        {
            List<PercMagFigliModel> libm = new List<PercMagFigliModel>();
            var r = new List<SelectListItem>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                idLivello = CaricaComboTipoFiglio(idLivello);
                using (dtMaggFigli dtib = new dtMaggFigli())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_PercentualFiglioPrimoNonAnnullato(idLivello);
                    libm = dtib.getListMaggiorazioneFiglio(idLivello, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
           
            return PartialView(libm);
        }

        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public ActionResult PercMaggiorazioneFiglioLivello(decimal idTipologiaFiglio, bool escludiAnnullati)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<PercMagFigliModel> libm = new List<PercMagFigliModel>();
            var r = new List<SelectListItem>();
            List<TipologiaFiglioModel> llm = new List<TipologiaFiglioModel>();
            try
            {
                idTipologiaFiglio = CaricaComboTipoFiglio(idTipologiaFiglio);
                using (dtMaggFigli dtib = new dtMaggFigli())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_PercentualFiglioPrimoNonAnnullato(idTipologiaFiglio);
                    libm = dtib.getListMaggiorazioneFiglio(idTipologiaFiglio, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
           
            return PartialView("MaggiorazioneFigli", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaMaggiorazioneFiglio(decimal idTipologiaFiglio, bool escludiAnnullati)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            var r = new List<SelectListItem>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                using (dtTipologiaFiglio dtl = new dtTipologiaFiglio())
                {
                    var lm = dtl.GetTipologiaFiglio(idTipologiaFiglio);
                    ViewBag.Figlio = lm;
                }
              
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult InserisciPercMaggiorazioneFiglio(PercMagFigliModel ibm, bool escludiAnnullati = true,bool aggiornaTutto=false)
        {
            var r = new List<SelectListItem>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtMaggFigli dtib = new dtMaggFigli())
                    {
                        dtib.SetMaggiorazioneFiglio(ibm, aggiornaTutto);
                    }
                    List<PercMagFigliModel> libm = new List<PercMagFigliModel>();
                    decimal idLivello = CaricaComboTipoFiglio(ibm.idTipologiaFiglio);
                    using (dtMaggFigli dtib = new dtMaggFigli())
                    {
                        ViewBag.idMinimoNonAnnullato = dtib.Get_Id_PercentualFiglioPrimoNonAnnullato(idLivello);
                        libm = dtib.getListMaggiorazioneFiglio(idLivello, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    return PartialView("MaggiorazioneFigli", libm);
                }
                else
                {
                    using (dtTipologiaFiglio dtl = new dtTipologiaFiglio())
                    {
                        var lm = dtl.GetTipologiaFiglio(ibm.idTipologiaFiglio);
                        ViewBag.Figlio = lm;
                    }
                    ViewBag.escludiAnnullati = escludiAnnullati;
                    return PartialView("NuovaMaggiorazioneFiglio", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaPercMaggiorazioneFiglio(bool escludiAnnullati, decimal idTipologiaFiglio, decimal idMaggFiglio)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<PercMagFigliModel> libm = new List<PercMagFigliModel>();
            try
            {
                using (dtMaggFigli dtib = new dtMaggFigli())
                {
                    dtib.DelMaggiorazioneFiglio(idMaggFiglio);
                
                    idTipologiaFiglio = CaricaComboTipoFiglio(idTipologiaFiglio);
                
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_PercentualFiglioPrimoNonAnnullato(idTipologiaFiglio);
                    libm = dtib.getListMaggiorazioneFiglio(idTipologiaFiglio, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
                return PartialView("MaggiorazioneFigli", libm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }
    }
}
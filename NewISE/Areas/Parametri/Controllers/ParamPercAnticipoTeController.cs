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
    public class ParamPercAnticipoTeController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        decimal CaricaComboTipoAnticipoTraspEff(decimal idLivello)
        {
            var r = new List<SelectListItem>();
            List<TipoAnticipoTrasportoEffettiModel> llm = new List<TipoAnticipoTrasportoEffettiModel>();
            using (dtParPercAnticipoTE dtl = new dtParPercAnticipoTE())
            {
                llm = dtl.GetTipoAnticipoTraspEffetti().OrderBy(a => a.idTipoAnticipoTrasportEff).ToList();

                if (llm != null && llm.Count > 0)
                {
                    r = (from t in llm
                         select new SelectListItem()
                         {
                             Text = t.tipoAnticipoTraspEffetti,
                             Value = t.idTipoAnticipoTrasportEff.ToString()
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
        public ActionResult PercAnticipoTE(bool escludiAnnullati = true, decimal idLivello = 0)
        {
            //escludiAnnullati: chk, idLivello: idLivello, idUfficio: idUfficio
            List<PercAnticipoTEModel> libm = new List<PercAnticipoTEModel>();
            var r = new List<SelectListItem>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                idLivello = CaricaComboTipoAnticipoTraspEff(idLivello);
                using (dtParPercAnticipoTE dtib = new dtParPercAnticipoTE())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_PercAnticipoTEPrimoNonAnnullato(idLivello);
                    libm = dtib.getListPercAnticipoTE(idLivello, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
        public ActionResult PercAnticipoTELivello(decimal idTipologiaAnticipo, bool escludiAnnullati)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<PercAnticipoTEModel> libm = new List<PercAnticipoTEModel>();
            var r = new List<SelectListItem>();
            List<TipoAnticipoTrasportoEffettiModel> llm = new List<TipoAnticipoTrasportoEffettiModel>();
            try
            {
                idTipologiaAnticipo = CaricaComboTipoAnticipoTraspEff(idTipologiaAnticipo);
                using (dtParPercAnticipoTE dtib = new dtParPercAnticipoTE())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_PercAnticipoTEPrimoNonAnnullato(idTipologiaAnticipo);
                    libm = dtib.getListPercAnticipoTE(idTipologiaAnticipo, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }

            return PartialView("PercAnticipoTE", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaPercAnticipoTE(decimal idTipologiaAnticipo, bool escludiAnnullati)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            var r = new List<SelectListItem>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                using (dtParPercAnticipoTE dtl = new dtParPercAnticipoTE())
                {
                    var lm = dtl.GetTipoAnticipoTraspEffetti(idTipologiaAnticipo);
                    ViewBag.Anticipo = lm;
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
        [ValidateAntiForgeryToken]
        public ActionResult InserisciPercAnticipoTE(PercAnticipoTEModel ibm, bool escludiAnnullati = true, bool aggiornaTutto = false)
        {
            var r = new List<SelectListItem>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<PercAnticipoTEModel> libm = new List<PercAnticipoTEModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtParPercAnticipoTE dtib = new dtParPercAnticipoTE())
                    {
                        dtib.SetPercAnticipoTE(ibm, aggiornaTutto);
                    }

                    decimal idLivello = CaricaComboTipoAnticipoTraspEff(ibm.idTipoAnticipo);
                    using (dtParPercAnticipoTE dtib = new dtParPercAnticipoTE())
                    {
                        ViewBag.idMinimoNonAnnullato = dtib.Get_Id_PercAnticipoTEPrimoNonAnnullato(ibm.idTipoAnticipo);
                        libm = dtib.getListPercAnticipoTE(idLivello, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    return PartialView("PercAnticipoTE", libm);
                }
                else
                {
                    using (dtParPercAnticipoTE dtl = new dtParPercAnticipoTE())
                    {
                        var lm = dtl.GetTipoAnticipoTraspEffetti(ibm.idTipoAnticipo);
                        ViewBag.Anticipo = lm;
                        ViewBag.idMinimoNonAnnullato = dtl.Get_Id_PercAnticipoTEPrimoNonAnnullato(ibm.idTipoAnticipo);
                        libm = dtl.getListPercAnticipoTE(ibm.idTipoAnticipo, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    ViewBag.escludiAnnullati = escludiAnnullati;
                    return PartialView("NuovaPercAnticipoTE", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaPercAnticipoTE(bool escludiAnnullati, decimal idTipoAnticipo, decimal idPercAnticipo)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<PercAnticipoTEModel> libm = new List<PercAnticipoTEModel>();
            try
            {
                using (dtParPercAnticipoTE dtib = new dtParPercAnticipoTE())
                {
                    dtib.DelPercAnticipoTE(idPercAnticipo);
                    idTipoAnticipo = CaricaComboTipoAnticipoTraspEff(idTipoAnticipo);
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_PercAnticipoTEPrimoNonAnnullato(idTipoAnticipo);
                    libm = dtib.getListPercAnticipoTE(idTipoAnticipo, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
                return PartialView("PercAnticipoTE", libm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }
    }
}
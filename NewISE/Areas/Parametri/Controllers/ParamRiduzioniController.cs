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
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult Riduzioni(bool escludiAnnullati, decimal idLivello = 0)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<RiduzioniModel> libm = new List<RiduzioniModel>();
            var r = new List<SelectListItem>();
            List<FunzioneRiduzioneModel> llm = new List<FunzioneRiduzioneModel>();
            try
            {
                idLivello = CaricaComboFunzioniRiduzione(idLivello);
                using (dtRiduzioni dtib = new dtRiduzioni())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_RiduzionePrimoNonAnnullato(idLivello);
                    libm = dtib.getListRiduzioni(idLivello, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch
            {
                return PartialView("ErrorPartial");
            }
            return PartialView(libm);
        }
        decimal CaricaComboFunzioniRiduzione(decimal idFunzioneRiduzione)
        {
            var r = new List<SelectListItem>();
            List<FunzioneRiduzioneModel> llm = new List<FunzioneRiduzioneModel>();
            using (dtRiduzioni dtl = new dtRiduzioni())
            {
                llm = dtl.GetFunzioniRiduzione().OrderBy(a => a.DescFunzione).ToList();

                if (llm != null && llm.Count > 0)
                {
                    r = (from t in llm
                         select new SelectListItem()
                         {
                             Text = t.DescFunzione,
                             Value = t.idFunzioneRiduzione.ToString()
                         }).ToList();

                    if (idFunzioneRiduzione == 0)
                    {
                        r.First().Selected = true;
                        idFunzioneRiduzione = Convert.ToDecimal(r.First().Value);
                    }
                    else
                    {
                        var temp = r.Where(a => a.Value == idFunzioneRiduzione.ToString()).ToList();
                        if (temp.Count == 0)
                        {
                            r.First().Selected = true;
                            idFunzioneRiduzione = Convert.ToDecimal(r.First().Value);
                        }
                        else
                            r.Where(a => a.Value == idFunzioneRiduzione.ToString()).First().Selected = true;
                    }
                }
                ViewBag.LivelliList = r;
            }
            return idFunzioneRiduzione;
        }
        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public ActionResult RiduzioniLivello(decimal idFunzioneRiduzione, bool escludiAnnullati)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<RiduzioniModel> libm = new List<RiduzioniModel>();
            var r = new List<SelectListItem>();
            List<FunzioneRiduzioneModel> llm = new List<FunzioneRiduzioneModel>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                idFunzioneRiduzione = CaricaComboFunzioniRiduzione(idFunzioneRiduzione);
                using (dtRiduzioni dtib = new dtRiduzioni())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_RiduzionePrimoNonAnnullato(idFunzioneRiduzione);
                    libm = dtib.getListRiduzioni(idFunzioneRiduzione, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                MsgErr msg = new MsgErr()
                {
                    msg = ex.Message
                };
                return PartialView("ErrorPartial", msg);
            }
            return PartialView("Riduzioni", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuoveRiduzioni(decimal idFunzioneRiduzione, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                CaricaComboFunzioniRiduzione(idFunzioneRiduzione);
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
        public ActionResult InserisciRiduzione(RiduzioniModel ibm, bool escludiAnnullati = true, bool aggiornaTutto = false)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            var r = new List<SelectListItem>();
            List<RiduzioniModel> libm = new List<RiduzioniModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtRiduzioni dtib = new dtRiduzioni())
                    {
                        dtib.SetRiduzioni(ibm, aggiornaTutto);
                    }
                    decimal idFunzioneRiduzione = CaricaComboFunzioniRiduzione(ibm.idFunzioneRiduzione);
                    using (dtRiduzioni dtib = new dtRiduzioni())
                    {
                        ViewBag.idMinimoNonAnnullato = dtib.Get_Id_RiduzionePrimoNonAnnullato(idFunzioneRiduzione);
                        libm = dtib.getListRiduzioni(idFunzioneRiduzione, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    return PartialView("Riduzioni", libm);
                }
                else
                {
                    decimal idFunzioneRiduzione = CaricaComboFunzioniRiduzione(ibm.idFunzioneRiduzione);
                    using (dtRiduzioni dtib = new dtRiduzioni())
                    {
                        ViewBag.idMinimoNonAnnullato = dtib.Get_Id_RiduzionePrimoNonAnnullato(idFunzioneRiduzione);
                        libm = dtib.getListRiduzioni(idFunzioneRiduzione, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
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
        public ActionResult EliminaRiduzione(bool escludiAnnullati, decimal idFunzioneRiduzione, decimal idRiduzioni)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<RiduzioniModel> libm = new List<RiduzioniModel>();
            try
            {
                using (dtRiduzioni dtib = new dtRiduzioni())
                {
                    dtib.DelRiduzioni(idRiduzioni);
                }
                idFunzioneRiduzione = CaricaComboFunzioniRiduzione(idFunzioneRiduzione);
                using (dtRiduzioni dtib = new dtRiduzioni())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_RiduzionePrimoNonAnnullato(idFunzioneRiduzione);
                    libm = dtib.getListRiduzioni(idFunzioneRiduzione, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
                return PartialView("Riduzioni", libm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }
    }
}
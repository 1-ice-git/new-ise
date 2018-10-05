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
    public class ParamIndSistemazController : Controller
    {
        // GET: Parametri/ParamIndSistemaz/IndennitaSistemazione

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult IndennitaSistemazione(bool escludiAnnullati, decimal idLivello = 0, bool aggiornaTutto = false)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            var r = new List<SelectListItem>();
            List<TipoTrasferimentoModel> llm = new List<TipoTrasferimentoModel>();
            try
            {
                idLivello = CaricaComboTipoTrasferimento(idLivello);
                List<IndennitaSistemazioneModel> libm = new List<IndennitaSistemazioneModel>();
                using (dtParIndSist dtib = new dtParIndSist())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_IndSistemNonAnnullato(idLivello);
                    libm = dtib.getListIndennitaSistemazione(idLivello, escludiAnnullati).OrderBy(a => a.idTipoTrasferimento).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
                return PartialView(libm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        public decimal CaricaComboTipoTrasferimento(decimal idTipoTrasf = 0)
        {
            //  List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();
            var r = new List<SelectListItem>();
            List<TipoTrasferimentoModel> llm = new List<TipoTrasferimentoModel>();
            using (dtParTipoTrasferimento dtl = new dtParTipoTrasferimento())
            {
                llm = dtl.GetTrasferimenti().OrderBy(a => a.idTipoTrasferimento).ToList();
                if (llm != null && llm.Count > 0)
                {
                    r = (from t in llm
                         select new SelectListItem()
                         {
                             Text = t.descTipoTrasf,
                             Value = t.idTipoTrasferimento.ToString()
                         }).ToList();
                    if (idTipoTrasf == 0)
                    {
                        r.First().Selected = true;
                        idTipoTrasf = Convert.ToDecimal(r.First().Value);
                    }
                    else
                    {
                        var temp = r.Where(a => a.Value == idTipoTrasf.ToString()).ToList();
                        if (temp.Count == 0)
                        {
                            r.First().Selected = true;
                            idTipoTrasf = Convert.ToDecimal(r.First().Value);
                        }
                        else
                            r.Where(a => a.Value == idTipoTrasf.ToString()).First().Selected = true;
                    }
                }
                ViewBag.ListaTipoTrasferimento = r;
            }
            return idTipoTrasf;
        }
        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public ActionResult IndennitaSistemazioneLivello(decimal idTipoTrasferimento, bool escludiAnnullati)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<IndennitaSistemazioneModel> libm = new List<IndennitaSistemazioneModel>();
            var r = new List<SelectListItem>();
            List<TipoTrasferimentoModel> llm = new List<TipoTrasferimentoModel>();

            try
            {
                CaricaComboTipoTrasferimento(idTipoTrasferimento);
                using (dtParIndSist dtib = new dtParIndSist())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_IndSistemNonAnnullato(idTipoTrasferimento);
                    libm = dtib.getListIndennitaSistemazione(idTipoTrasferimento, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.escludiAnnullati = escludiAnnullati;
            return PartialView("IndennitaSistemazione", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaIndennitaSistemazione(decimal idTipoTrasferimento, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                using (dtParTipoTrasferimento dtl = new dtParTipoTrasferimento())
                {
                    var lm = dtl.GetTrasferimenti(idTipoTrasferimento);
                    ViewBag.Trasferimento = lm;
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
        [ValidateAntiForgeryToken]
        public ActionResult InserisciIndennitaSistemazione(IndennitaSistemazioneModel ibm, bool escludiAnnullati, bool aggiornaTutto = false)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtParIndSist dtib = new dtParIndSist())
                    {
                        dtib.SetIndennitaSistemazione(ibm, aggiornaTutto);
                    }

                    decimal idtipTras = CaricaComboTipoTrasferimento(ibm.idTipoTrasferimento);
                    List<IndennitaSistemazioneModel> libm = new List<IndennitaSistemazioneModel>();
                    using (dtParIndSist dtib = new dtParIndSist())
                    {
                        ViewBag.idMinimoNonAnnullato = dtib.Get_Id_IndSistemNonAnnullato(idtipTras);
                        libm = dtib.getListIndennitaSistemazione(idtipTras, escludiAnnullati).OrderBy(a => a.idTipoTrasferimento).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    return PartialView("IndennitaSistemazione", libm);
                    //   return RedirectToAction("IndennitaSistemazione", new { escludiAnnullati = escludiAnnullati, idTipoTrasferimento = ibm.idTipoTrasferimento });
                }
                else
                {
                    using (dtParTipoTrasferimento dtl = new dtParTipoTrasferimento())
                    {
                        var lm = dtl.GetTrasferimenti(ibm.idTipoTrasferimento);
                        ViewBag.Trasferimento = lm;
                    }
                    ViewBag.escludiAnnullati = escludiAnnullati;
                    return PartialView("NuovaIndennitaSistemazione", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaIndennitaSistemazione(bool escludiAnnullati, decimal idTipoTrasferimento, decimal idIndSist)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                using (dtParIndSist dtib = new dtParIndSist())
                {
                    dtib.DelIndennitaSistemazione(idIndSist);
                }

                idTipoTrasferimento = CaricaComboTipoTrasferimento(idTipoTrasferimento);
                List<IndennitaSistemazioneModel> libm = new List<IndennitaSistemazioneModel>();
                using (dtParIndSist dtib = new dtParIndSist())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_IndSistemNonAnnullato(idTipoTrasferimento);
                    libm = dtib.getListIndennitaSistemazione(idTipoTrasferimento, escludiAnnullati).OrderBy(a => a.idTipoTrasferimento).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
                return PartialView("IndennitaSistemazione", libm);
                // return RedirectToAction("IndennitaSistemazione", new { escludiAnnullati = escludiAnnullati, idTipoTrasferimento = idTipoTrasferimento });
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
    }
}
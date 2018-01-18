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
        public ActionResult IndennitaSistemazione(bool escludiAnnullati, decimal idLivello = 0)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;           
            var r = new List<SelectListItem>();
            List<TipoTrasferimentoModel> llm = new List<TipoTrasferimentoModel>();
            try
            {
                using (dtParTipoTrasferimento dtl = new dtParTipoTrasferimento())
                {
                    llm = dtl.GetTrasferimenti().OrderBy(a => a.descTipoTrasf).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.descTipoTrasf,
                                 Value = t.idTipoTrasferimento.ToString()
                             }).ToList();

                        if (idLivello == 0)
                        {
                            r.First().Selected = true;
                            idLivello = Convert.ToDecimal(r.First().Value);
                        }
                        else
                        {
                            var temp = r.Where(a => a.Value == idLivello.ToString()).ToList();   
                            if(temp.Count!=0)                         
                                r.Where(a => a.Value == idLivello.ToString()).First().Selected = true;
                        }
                    }
                    ViewBag.LivelliList = r;
                }
                List<IndennitaSistemazioneModel> libm = new List<IndennitaSistemazioneModel>();
                using (dtParIndSist dtib = new dtParIndSist())
                {                   
                     libm = dtib.getListIndennitaSistemazione(idLivello, escludiAnnullati).OrderBy(a => a.idTipoTrasferimento).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
                return PartialView(libm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
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
                using (dtParTipoTrasferimento dtl = new dtParTipoTrasferimento())
                {
                    llm = dtl.GetTrasferimenti().OrderBy(a => a.descTipoTrasf).ToList();
                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.descTipoTrasf,
                                 Value = t.idTipoTrasferimento.ToString()
                             }).ToList();
                        r.Where(a => a.Value == idTipoTrasferimento.ToString()).First().Selected = true;
                    }
                    ViewBag.LivelliList = r;
                }
                using (dtParIndSist dtib = new dtParIndSist())
                {
                    libm = dtib.getListIndennitaSistemazione(llm.Where(a => a.idTipoTrasferimento == idTipoTrasferimento).First().idTipoTrasferimento, escludiAnnullati).OrderBy(a => a.idTipoTrasferimento).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
        public ActionResult InserisciIndennitaSistemazione(IndennitaSistemazioneModel ibm, bool escludiAnnullati)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtParIndSist dtib = new dtParIndSist())
                    {
                        dtib.SetIndennitaSistemazione(ibm);
                    }
                    using (dtParTipoTrasferimento dtl = new dtParTipoTrasferimento())
                    {
                        List<TipoTrasferimentoModel> llm = new List<TipoTrasferimentoModel>();
                        var r = new List<SelectListItem>();
                        llm = dtl.GetTrasferimenti().OrderBy(a => a.descTipoTrasf).ToList();
                        if (llm != null && llm.Count > 0)
                        {
                            r = (from t in llm
                                 select new SelectListItem()
                                 {
                                     Text = t.descTipoTrasf,
                                     Value = t.idTipoTrasferimento.ToString()
                                 }).ToList();
                            r.Where(a => a.Value == ibm.idTipoTrasferimento.ToString()).First().Selected = true;
                        }
                        ViewBag.LivelliList = r;
                    }
                    List<IndennitaSistemazioneModel> libm = new List<IndennitaSistemazioneModel>();
                    using (dtParIndSist dtib = new dtParIndSist())
                    {
                        libm = dtib.getListIndennitaSistemazione(ibm.idTipoTrasferimento, escludiAnnullati).OrderBy(a => a.idTipoTrasferimento).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    return PartialView("IndennitaSistemazione",libm);
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
                using (dtParTipoTrasferimento dtl = new dtParTipoTrasferimento())
                {
                    var r = new List<SelectListItem>();
                    List<TipoTrasferimentoModel> llm = new List<TipoTrasferimentoModel>();
                    llm = dtl.GetTrasferimenti().OrderBy(a => a.descTipoTrasf).ToList();
                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.descTipoTrasf,
                                 Value = t.idTipoTrasferimento.ToString()
                             }).ToList();
                        r.Where(a => a.Value == idTipoTrasferimento.ToString()).First().Selected = true;
                    }
                    ViewBag.LivelliList = r;
                }
                List<IndennitaSistemazioneModel> libm = new List<IndennitaSistemazioneModel>();
                using (dtParIndSist dtib = new dtParIndSist())
                {
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
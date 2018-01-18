using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamMaggAnnualiController : Controller
    {
        // GET: Parametri/ParamMaggAnnuali/MaggiorazioniAnnuali
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult MaggiorazioniAnnuali(bool escludiAnnullati, decimal idLivello = 0)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<MaggiorazioniAnnualiModel> libm = new List<MaggiorazioniAnnualiModel>();
            var r = new List<SelectListItem>();
            List<UfficiModel> llm = new List<UfficiModel>();
            try
            {
                using (dtUffici dtl = new dtUffici())
                {
                    llm = dtl.GetUffici().OrderBy(a => a.descUfficio).ToList();
                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.descUfficio,
                                 Value = t.idUfficio.ToString()
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
                using (dtParMaggAnnuali dtib = new dtParMaggAnnuali())
                {                    
                    libm = dtib.getListMaggiorazioneAnnuale(idLivello, escludiAnnullati).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
        public ActionResult MaggiorazioneAnnualeLivello(decimal idUfficio, bool escludiAnnullati)
        {
            List<MaggiorazioniAnnualiModel> libm = new List<MaggiorazioniAnnualiModel>();
            var r = new List<SelectListItem>();
            List<UfficiModel> llm = new List<UfficiModel>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                using (dtUffici dtl = new dtUffici())
                {
                    llm = dtl.GetUffici().OrderBy(a => a.descUfficio).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.descUfficio,
                                 Value = t.idUfficio.ToString()
                             }).ToList();
                        r.Where(a => a.Value == idUfficio.ToString()).First().Selected = true;
                    }
                    ViewBag.LivelliList = r;
                }
                using (dtParMaggAnnuali dtib = new dtParMaggAnnuali())
                {
                   libm = dtib.getListMaggiorazioneAnnuale(llm.Where(a => a.idUfficio == idUfficio).First().idUfficio, escludiAnnullati).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.escludiAnnullati = escludiAnnullati;
            return PartialView("MaggiorazioniAnnuali", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaMaggiorazioneAnnuale(decimal idUfficio, bool escludiAnnullati)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            var r = new List<SelectListItem>();
            try
            {
                using (dtUffici dtl = new dtUffici())
                {
                    var lm = dtl.GetUffici(idUfficio);
                    ViewBag.Descrizione = lm;
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
        public ActionResult InserisciMaggiorazioneAnnuale(MaggiorazioniAnnualiModel ibm, bool escludiAnnullati = true)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            var r = new List<SelectListItem>();
            List<UfficiModel> llm = new List<UfficiModel>();
            List<MaggiorazioniAnnualiModel> libm = new List<MaggiorazioniAnnualiModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtParMaggAnnuali dtib = new dtParMaggAnnuali())
                    {
                        dtib.SetMaggiorazioneAnnuale(ibm);
                    }
                    using (dtParMaggAnnuali dtib = new dtParMaggAnnuali())
                    {
                        libm = dtib.getListMaggiorazioneAnnuale(ibm.idUfficio, escludiAnnullati).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    using (dtUffici dtl = new dtUffici())
                    {
                        llm = dtl.GetUffici().OrderBy(a => a.descUfficio).ToList();

                        if (llm != null && llm.Count > 0)
                        {
                            r = (from t in llm
                                 select new SelectListItem()
                                 {
                                     Text = t.descUfficio,
                                     Value = t.idUfficio.ToString()
                                 }).ToList();
                            r.Where(a => a.Value == ibm.idUfficio.ToString()).First().Selected = true;
                        }
                        ViewBag.LivelliList = r;
                    }
                    using (dtParMaggAnnuali dtib = new dtParMaggAnnuali())
                    {
                        libm = dtib.getListMaggiorazioneAnnuale(ibm.idUfficio, escludiAnnullati).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    return PartialView("MaggiorazioniAnnuali",libm);
                   // return RedirectToAction("MaggiorazioniAnnuali", new { escludiAnnullati = escludiAnnullati, idUfficio = ibm.idUfficio });
                }
                else
                {
                    using (dtUffici dtl = new dtUffici())
                    {
                        var lm = dtl.GetUffici(ibm.idUfficio);
                        ViewBag.Descrizione = lm;
                    }
                  
                    return PartialView("NuovaMaggiorazioneAnnuale", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaMaggiorazioneAnnuale(bool escludiAnnullati, decimal idUfficio, decimal idMaggAnnuale)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            var r = new List<SelectListItem>();
            List<UfficiModel> llm = new List<UfficiModel>();
            List<MaggiorazioniAnnualiModel> libm = new List<MaggiorazioniAnnualiModel>();
            try
            {
                using (dtParMaggAnnuali dtib = new dtParMaggAnnuali())
                {
                    dtib.DelMaggiorazioneAnnuale(idMaggAnnuale);
                }
                using (dtUffici dtl = new dtUffici())
                {
                    llm = dtl.GetUffici().OrderBy(a => a.descUfficio).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.descUfficio,
                                 Value = t.idUfficio.ToString()
                             }).ToList();
                        r.Where(a => a.Value == idUfficio.ToString()).First().Selected = true;
                    }
                    ViewBag.LivelliList = r;
                }
                using (dtParMaggAnnuali dtib = new dtParMaggAnnuali())
                {
                    libm = dtib.getListMaggiorazioneAnnuale(idUfficio, escludiAnnullati).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
                return PartialView("MaggiorazioniAnnuali",libm);
             //   return RedirectToAction("MaggiorazioneAnnuale", new { escludiAnnullati = escludiAnnullati, idUfficio = idUfficio });
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
    }
}
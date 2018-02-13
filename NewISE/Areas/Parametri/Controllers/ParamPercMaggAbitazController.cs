using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamPercMaggAbitazController : Controller
    {
        // GET: Parametri/ParamPercMaggAbitaz/PercentualeMaggAbitazione
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult PercentualeMaggAbitazione(bool escludiAnnullati, decimal idLivello = 0, decimal idUfficio = 0)
        {
            
            var r = new List<SelectListItem>();
            List<LivelloModel> llm = new List<LivelloModel>();
            List<UfficiModel> llm1 = new List<UfficiModel>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<PercMaggAbitazModel> libm = new List<PercMaggAbitazModel>();
            try
            {
                decimal[] tmp = AggiornaListaPerCombo(idLivello, idUfficio);
                using (dtParPercMaggAbitazione dtib = new dtParPercMaggAbitazione())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_MaggAbitazioneNonAnnullato(tmp[0], tmp[1]);
                    libm = dtib.getListMaggiorazioneAbitazione(tmp[0], tmp[1], escludiAnnullati).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
        public ActionResult PercentualeMaggiorazioneAbitazioneLivello(decimal idLivello, decimal idUfficio, bool escludiAnnullati)
        {
            List<PercMaggAbitazModel> libm = new List<PercMaggAbitazModel>();
            var r = new List<SelectListItem>();
            List<LivelloModel> llm = new List<LivelloModel>();
            List<UfficiModel> llm1 = new List<UfficiModel>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                decimal[] tmp = AggiornaListaPerCombo(idLivello, idUfficio);

                using (dtParPercMaggAbitazione dtib = new dtParPercMaggAbitazione())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_MaggAbitazioneNonAnnullato(tmp[0],tmp[1]);
                    libm = dtib.getListMaggiorazioneAbitazione(tmp[0], tmp[1], escludiAnnullati).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("PercentualeMaggAbitazione", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaPercentualeMaggiorazioneAbitazione(decimal idLivello, decimal idUfficio, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();            
            try
            {
                using (dtLivelli dtl = new dtLivelli())
                {
                    var lm = dtl.GetLivelli(idLivello);
                    ViewBag.Livello = lm;
                }

                using (dtUffici dtl1 = new dtUffici())
                {
                    var lm1 = dtl1.GetUffici(idUfficio);
                    ViewBag.Ufficio = lm1;
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
        public ActionResult InserisciMaggiorazioneAbitazione(PercMaggAbitazModel ibm, bool escludiAnnullati = true,bool aggiornaTutto=false)
        {
            var r = new List<SelectListItem>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<PercMaggAbitazModel> libm = new List<PercMaggAbitazModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtParPercMaggAbitazione dtib = new dtParPercMaggAbitazione())
                    {                        
                        dtib.SetMaggiorazioneAbitazione(ibm, aggiornaTutto);
                    }
                    decimal[] tmp = AggiornaListaPerCombo(ibm.idLivello,ibm.idUfficio);
                    using (dtParPercMaggAbitazione dtib = new dtParPercMaggAbitazione())
                    {
                        ViewBag.idMinimoNonAnnullato = dtib.Get_Id_MaggAbitazioneNonAnnullato(tmp[0], tmp[1]);
                        libm = dtib.getListMaggiorazioneAbitazione(tmp[0], tmp[1], escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    return PartialView("PercentualeMaggAbitazione",libm);
                  //  return RedirectToAction("PercentualeMaggAbitazione", new { escludiAnnullati = escludiAnnullati, idLivello = ibm.idLivello });
                }
                else
                {
                    using (dtLivelli dtl = new dtLivelli())
                    {
                        var lm = dtl.GetLivelli(ibm.idLivello);
                        ViewBag.Livello = lm;
                    }
                    using (dtParPercMaggAbitazione dtib = new dtParPercMaggAbitazione())
                    {
                        libm = dtib.getListMaggiorazioneAbitazione(ibm.idLivello, ibm.idUfficio, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    return PartialView("NuovaPercentualeMaggAbitazione", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaMaggiorazioneAbitazione(bool escludiAnnullati, decimal idLivello, decimal idPercMabAbitaz,decimal idUfficio)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<PercMaggAbitazModel> libm = new List<PercMaggAbitazModel>();
            try
            {
                using (dtParPercMaggAbitazione dtib = new dtParPercMaggAbitazione())
                {
                    dtib.DelMaggiorazioneAbitazione(idPercMabAbitaz);
                }
                decimal[] tmp = AggiornaListaPerCombo(idLivello, idUfficio);
                using (dtParPercMaggAbitazione dtib = new dtParPercMaggAbitazione())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_MaggAbitazioneNonAnnullato(tmp[0], tmp[1]);
                    libm = dtib.getListMaggiorazioneAbitazione(tmp[0], tmp[1], escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
                return PartialView("PercentualeMaggAbitazione", libm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        decimal[] AggiornaListaPerCombo(decimal idLivello, decimal idUfficio )
        {  
            var r = new List<SelectListItem>();
            List<LivelloModel> llm = new List<LivelloModel>();
            List<UfficiModel> llm1 = new List<UfficiModel>();
           
                using (dtLivelli dtl = new dtLivelli())
                {
                    llm = dtl.GetLivelli().OrderBy(a => a.DescLivello).ToList();
                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.DescLivello,
                                 Value = t.idLivello.ToString()
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
                r = new List<SelectListItem>();
                using (dtUffici dtl1 = new dtUffici())
                {
                    llm1 = dtl1.GetUffici().OrderBy(a => a.descUfficio).ToList();

                    if (llm1 != null && llm1.Count > 0)
                    {
                        r = (from t in llm1
                             select new SelectListItem()
                             {
                                 Text = t.descUfficio,
                                 Value = t.idUfficio.ToString()
                             }).ToList();

                        if (idUfficio == 0)
                        {
                            r.First().Selected = true;
                            idUfficio = Convert.ToDecimal(r.First().Value);
                        }
                        else
                        {
                            var temp = r.Where(a => a.Value == idUfficio.ToString()).ToList();
                            if (temp.Count == 0)
                            {
                                r.First().Selected = true;
                                idUfficio = Convert.ToDecimal(r.First().Value);
                            }
                            else
                                r.Where(a => a.Value == idUfficio.ToString()).First().Selected = true;
                        }
                    }
                    ViewBag.UfficiList = r;
                }
            return new decimal[] { idLivello, idUfficio };
            }
    }
}
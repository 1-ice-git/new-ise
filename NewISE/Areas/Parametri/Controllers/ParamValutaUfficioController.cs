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
    public class ParamValutaUfficioController : Controller
    {
        // GET: Parametri/ParamPercMaggAbitaz/PercentualeMaggAbitazione
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult ValutaUfficio(bool escludiAnnullati, decimal idLivello = 0, decimal idUfficio = 0)
        {
            var r = new List<SelectListItem>();
            List<ValuteModel> llm = new List<ValuteModel>();
            List<UfficiModel> llm1 = new List<UfficiModel>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<ValutaUfficioModel> libm = new List<ValutaUfficioModel>();
            try
            {
                decimal tmp = AggiornaListaPerCombo(idLivello, idUfficio);
                using (dtParValutaUfficio dtib = new dtParValutaUfficio())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_ValutaUfficioNonAnnullato(tmp);
                    libm = dtib.getListValutaUfficio(tmp, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
        public ActionResult ValutaUfficioLivello(bool escludiAnnullati, decimal idValuta = 0, decimal idUfficio = 0)
        {
            List<ValutaUfficioModel> libm = new List<ValutaUfficioModel>();
            var r = new List<SelectListItem>();
            List<ValuteModel> llm = new List<ValuteModel>();
            List<UfficiModel> llm1 = new List<UfficiModel>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                decimal tmp = AggiornaListaPerCombo(idValuta, idUfficio);
                using (dtParValutaUfficio dtib = new dtParValutaUfficio())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_ValutaUfficioNonAnnullato(tmp);
                    libm = dtib.getListValutaUfficio( tmp, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView("ValutaUfficio", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaValutaUfficio(decimal idValuta, decimal idUfficio, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();            
            try
            {
                AggiornaListaPerCombo(idValuta, idUfficio);
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
        public ActionResult InserisciValutaUfficio(ValutaUfficioModel ibm, bool escludiAnnullati = true,bool aggiornaTutto=false)
        {
            var r = new List<SelectListItem>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<ValutaUfficioModel> libm = new List<ValutaUfficioModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtParValutaUfficio dtib = new dtParValutaUfficio())
                    {                        
                        dtib.SetValutaUfficio(ibm, aggiornaTutto);
                    }
                    decimal tmp = AggiornaListaPerCombo(ibm.idValuta,ibm.idUfficio);
                    using (dtParValutaUfficio dtib = new dtParValutaUfficio())
                    {
                        ViewBag.idMinimoNonAnnullato = dtib.Get_Id_ValutaUfficioNonAnnullato(tmp);
                        libm = dtib.getListValutaUfficio(tmp, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    return PartialView("ValutaUfficio",libm);
                  //  return RedirectToAction("PercentualeMaggAbitazione", new { escludiAnnullati = escludiAnnullati, idLivello = ibm.idLivello });
                }
                else
                {
                    //using (dtParValute dtl = new dtParValute())
                    //{
                    //    var lm =dtl.GetValute(ibm.idValuta);
                    //    ViewBag.Livello = lm;
                    //}
                    decimal tmp = AggiornaListaPerCombo(ibm.idValuta, ibm.idUfficio);
                    using (dtParValutaUfficio dtib = new dtParValutaUfficio())
                    {
                        libm = dtib.getListValutaUfficio(ibm.idUfficio, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    return PartialView("NuovaValutaUfficio", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaValutaUfficio(bool escludiAnnullati, decimal idValuta, decimal idValutaUfficio,decimal idUfficio)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<ValutaUfficioModel> libm = new List<ValutaUfficioModel>();
            try
            {
                using (dtParValutaUfficio dtib = new dtParValutaUfficio())
                {
                    dtib.DelValutaUfficio(idValutaUfficio);
                }
                decimal tmp = AggiornaListaPerCombo(idValuta, idUfficio);
                using (dtParValutaUfficio dtib = new dtParValutaUfficio())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_ValutaUfficioNonAnnullato(tmp);
                    libm = dtib.getListValutaUfficio(tmp, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
                return PartialView("ValutaUfficio", libm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        //decimal[] AggiornaListaPerCombo(decimal idValuta, decimal idUfficio )
        decimal AggiornaListaPerCombo(decimal idValuta, decimal idUfficio)
        {
            var r = new List<SelectListItem>();
            List<ValuteModel> llm = new List<ValuteModel>();
            idValuta = 0;
            using (NewISE.Areas.Parametri.Models.dtObj.dtValute dtl = new NewISE.Areas.Parametri.Models.dtObj.dtValute())
            {
                llm = dtl.getListValute().OrderBy(a => a.descrizioneValuta).ToList();
                if (llm != null && llm.Count > 0)
                {
                    r = (from t in llm
                         select new SelectListItem
                         {
                             Text = t.descrizioneValuta,
                             Value = t.idValuta.ToString()
                         }).ToList();

                    if (idValuta == 0)
                    {
                        r.First().Selected = true;
                        idValuta = Convert.ToDecimal(r.First().Value);
                    }
                    else
                    {
                        var temp = r.Where(a => a.Value == idValuta.ToString()).ToList();
                        if (temp.Count == 0)
                        {
                            r.First().Selected = true;
                            idValuta = Convert.ToDecimal(r.First().Value);
                        }
                        else
                            r.Where(a => a.Value == idValuta.ToString()).First().Selected = true;
                    }
                }
                ViewBag.ValutaList = r;
            }

            List<UfficiModel> llm1 = new List<UfficiModel>();
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
            //return new decimal[] { idValuta, idUfficio };
            return  idUfficio ;
        }
    }
}
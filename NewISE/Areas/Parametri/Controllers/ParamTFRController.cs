using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NewISE.Models;
using NewISE.Models.dtObj;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamTFRController : Controller
    {
        // GET: Parametri/ParamTFR
        public ActionResult TFR(bool escludiAnnullati, decimal idLivello = 0)
        {
            List<TFRModel> libm = new List<TFRModel>();
            var r = new List<SelectListItem>();
            List<ValuteModel> llm = new List<ValuteModel>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                idLivello = CaricaComboFunzioniTFR(idLivello);
                using (dtTfr dtib = new dtTfr())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_TFRPrimoNonAnnullato(idLivello);
                    libm = dtib.getListTfr(idLivello, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
            return PartialView(libm);
        }
        decimal CaricaComboFunzioniTFR(decimal idValuta)
        {
            var r = new List<SelectListItem>();
            List<ValuteModel> llm = new List<ValuteModel>();
            using (dtValute dtl = new dtValute())
            {
                llm = dtl.getListValute().OrderBy(a => a.descrizioneValuta).ToList();
                if (llm != null && llm.Count > 0)
                {
                    r = (from t in llm
                         select new SelectListItem()
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
                ViewBag.LivelliList = r;
            }
            return idValuta;
        }
        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public ActionResult TfrLivello(decimal idValuta, bool escludiAnnullati)
        {
            List<TFRModel> libm = new List<TFRModel>();
            var r = new List<SelectListItem>();
            List<ValuteModel> llm = new List<ValuteModel>();

            try
            {
                idValuta = CaricaComboFunzioniTFR(idValuta);
                using (dtTfr dtib = new dtTfr())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_TFRPrimoNonAnnullato(idValuta);
                    libm = dtib.getListTfr(idValuta, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("TFR", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovoTFR(decimal idValuta, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();

            try
            {
                using (dtValute dtl = new dtValute())
                {
                    var lm = dtl.GetValute(idValuta);
                    ViewBag.DescrizioneValuta = lm;
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
        [ValidateAntiForgeryToken]
        public ActionResult InserisciTFR(TFRModel ibm, bool escludiAnnullati = true, bool aggiornaTutto = false)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            //var r = new List<SelectListItem>();
            List<TFRModel> libm = new List<TFRModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtTfr dtib = new dtTfr())
                    {
                        dtib.SetTfr(ibm, aggiornaTutto);

                    }
                    decimal idLivello = CaricaComboFunzioniTFR(ibm.idValuta);
                    using (dtTfr dtib = new dtTfr())
                    {
                        ViewBag.idMinimoNonAnnullato = dtib.Get_Id_TFRPrimoNonAnnullato(idLivello);
                        libm = dtib.getListTfr(idLivello, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    return PartialView("TFR", libm);
                }
                else
                {
                    using (dtValute dtl = new dtValute())
                    {
                        var lm = dtl.GetValute(ibm.idValuta);
                        ViewBag.DescrizioneValuta = lm;
                    }
                    CaricaComboFunzioniTFR(ibm.idValuta);
                    return PartialView("NuovoTFR", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaTFR(bool escludiAnnullati, decimal idValuta, decimal idTFR)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<TFRModel> libm = new List<TFRModel>();
            try
            {
                using (dtTfr dtib = new dtTfr())
                {
                    dtib.DelTfr(idTFR);
                }
                decimal idLivello = CaricaComboFunzioniTFR(idValuta);
                using (dtTfr dtib = new dtTfr())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_TFRPrimoNonAnnullato(idLivello);
                    libm = dtib.getListTfr(idLivello, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
                return PartialView("TFR", libm);
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial");
            }


        }
    }
}
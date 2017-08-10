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
    public class ParamPercMaggAbitazController : Controller
    {
        // GET: Parametri/ParamPercMaggAbitaz/PercentualeMaggAbitazione

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult PercentualeMaggAbitazione(bool escludiAnnullati, decimal idLivello = 0)
        {
            List<PercMaggAbitazModel> libm = new List<PercMaggAbitazModel>();
            var r = new List<SelectListItem>();
            List<LivelloModel> llm = new List<LivelloModel>();

            try
            {
                using (dtParLivelli dtl = new dtParLivelli())
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
                            r.Where(a => a.Value == idLivello.ToString()).First().Selected = true;
                        }
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtParPercMaggAbitazione dtib = new dtParPercMaggAbitazione())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListMaggiorazioneAbitazione(idLivello, escludiAnnullati).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListMaggiorazioneAbitazione(idLivello).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
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
        public ActionResult PercentualeMaggiorazioneAbitazioneLivello(decimal idLivello, bool escludiAnnullati)
        {
            List<PercMaggAbitazModel> libm = new List<PercMaggAbitazModel>();
            var r = new List<SelectListItem>();
            List<LivelloModel> llm = new List<LivelloModel>();

            try
            {
                using (dtParLivelli dtl = new dtParLivelli())
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
                        r.Where(a => a.Value == idLivello.ToString()).First().Selected = true;
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtParPercMaggAbitazione dtib = new dtParPercMaggAbitazione())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListMaggiorazioneAbitazione(llm.Where(a => a.idLivello == idLivello).First().idLivello, escludiAnnullati).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListMaggiorazioneAbitazione(llm.Where(a => a.idLivello == idLivello).First().idLivello).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("IndennitaBase", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaPercentualeMaggiorazioneAbitazione(decimal idLivello, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();


            try
            {
                using (dtParLivelli dtl = new dtParLivelli())
                {
                    var lm = dtl.GetLivelli(idLivello);
                    ViewBag.Livello = lm;
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
        public ActionResult InserisciMaggiorazioneAbitazione(PercMaggAbitazModel ibm, bool escludiAnnullati = true)
        {
            var r = new List<SelectListItem>();

            try
            {
                if (ModelState.IsValid)
                {
                    using (dtParPercMaggAbitazione dtib = new dtParPercMaggAbitazione())
                    {



                        dtib.SetMaggiorazioneAbitazione(ibm);
                    }

                    return RedirectToAction("PercentualeMaggAbitazione", new { escludiAnnullati = escludiAnnullati, idLivello = ibm.idLivello });
                }
                else
                {
                    using (dtParLivelli dtl = new dtParLivelli())
                    {
                        var lm = dtl.GetLivelli(ibm.idLivello);
                        ViewBag.Livello = lm;
                    }
                    ViewBag.escludiAnnullati = escludiAnnullati;
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
        public ActionResult EliminaMaggiorazioneAbitazione(bool escludiAnnullati, decimal idLivello, decimal idPercMabAbitaz)
        {

            try
            {
                using (dtParPercMaggAbitazione dtib = new dtParPercMaggAbitazione())
                {
                    dtib.DelMaggiorazioneAbitazione(idPercMabAbitaz);
                }

                return RedirectToAction("PercentualeMaggAbitazione", new { escludiAnnullati = escludiAnnullati, idLivello = idLivello });
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }
    }
}
using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
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
            List<PercMaggAbitazModel> libm = new List<PercMaggAbitazModel>();
            var r = new List<SelectListItem>();
            List<LivelloModel> llm = new List<LivelloModel>();
            List<UfficiModel> llm1 = new List<UfficiModel>();

            try
            {
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
                            r.Where(a => a.Value == idLivello.ToString()).First().Selected = true;
                        }
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtUffici dtl = new dtUffici())
                {
                    llm1 = dtl.GetUffici().OrderBy(a => a.DescUfficio).ToList();

                    if (llm1 != null && llm1.Count > 0)
                    {
                        r = (from t in llm1
                             select new SelectListItem()
                             {
                                 Text = t.DescUfficio,
                                 Value = t.idUfficio.ToString()
                             }).ToList();

                        if (idUfficio == 0)
                        {
                            r.First().Selected = true;
                            idUfficio = Convert.ToDecimal(r.First().Value);
                        }
                        else
                        {
                            r.Where(a => a.Value == idUfficio.ToString()).First().Selected = true;
                        }
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtPercMaggAbitazione dtib = new dtPercMaggAbitazione())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListPercMaggAbitazione(idLivello, idUfficio, escludiAnnullati).OrderBy(a => a.idLivello).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListPercMaggAbitazione(idLivello, idUfficio).OrderBy(a => a.idLivello).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }

            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView(libm);
        }

        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public ActionResult PercMaggAbitazioneLivelloUfficio(decimal idLivello, decimal idUfficio, bool escludiAnnullati)
        {
            List<PercMaggAbitazModel> libm = new List<PercMaggAbitazModel>();
            var r = new List<SelectListItem>();
            List<LivelloModel> llm = new List<LivelloModel>();
            List<UfficiModel> llm1 = new List<UfficiModel>();

            try
            {
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
                        r.Where(a => a.Value == idLivello.ToString()).First().Selected = true;
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtUffici dtl = new dtUffici())
                {
                    llm1 = dtl.GetUffici().OrderBy(a => a.DescUfficio).ToList();

                    if (llm1 != null && llm1.Count > 0)
                    {
                        r = (from t in llm1
                             select new SelectListItem()
                             {
                                 Text = t.DescUfficio,
                                 Value = t.idUfficio.ToString()
                             }).ToList();
                        r.Where(a => a.Value == idUfficio.ToString()).First().Selected = true;
                    }

                    ViewBag.LivelliList = r;
                }


                using (dtPercMaggAbitazione dtib = new dtPercMaggAbitazione())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListPercMaggAbitazione(idLivello, idUfficio, escludiAnnullati).OrderBy(a => a.idLivello).OrderBy(a=> a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListPercMaggAbitazione(idLivello, idUfficio).OrderBy(a => a.idLivello).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("IndennitaBase", libm);
        }

        //public ActionResult PercentualeMaggiorazioneAbitazioneUfficio(decimal idUfficio, bool escludiAnnullati)
        //{
        //    List<PercMaggAbitazModel> libm = new List<PercMaggAbitazModel>();
        //    var r = new List<SelectListItem>();
           
        //    List<UfficiModel> llm = new List<UfficiModel>();

        //    try
        //    {
        //        using (dtUffici dtl = new dtUffici())
        //        {
        //            llm = dtl.GetUffici().OrderBy(a => a.DescUfficio).ToList();

        //            if (llm != null && llm.Count > 0)
        //            {
        //                r = (from t in llm
        //                     select new SelectListItem()
        //                     {
        //                         Text = t.DescUfficio,
        //                         Value = t.idUfficio.ToString()
        //                     }).ToList();
        //                r.Where(a => a.Value == idUfficio.ToString()).First().Selected = true;
        //            }

        //            ViewBag.LivelliList = r;
        //        }

        //        using (dtPercMaggAbitazione dtib = new dtPercMaggAbitazione())
        //        {
        //            if (escludiAnnullati)
        //            {
        //                escludiAnnullati = false;
        //                //libm = dtib.getListPercMaggAbitazione(idLivello, idUfficio, escludiAnnullati).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
        //            }
        //            else
        //            {
        //                //libm = dtib.getListPercMaggAbitazione(idLivello, idUfficio).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return PartialView("ErrorPartial");
        //    }
        //    ViewBag.escludiAnnullati = escludiAnnullati;

        //    return PartialView("IndennitaBase", libm);
        //}

        


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

                using (dtUffici dtl = new dtUffici())
                {
                    var lm = dtl.GetUffici(idUfficio);
                    ViewBag.Descrizione = lm;
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
        public ActionResult InserisciPercentualeMaggiorazioneAbitazione(PercMaggAbitazModel ibm, bool escludiAnnullati = true)
        {
            var r = new List<SelectListItem>();

            try
            {
                if (ModelState.IsValid)
                {
                    using (dtPercMaggAbitazione dtib = new dtPercMaggAbitazione())
                    {
                        dtib.SetPercMaggAbitazione(ibm);
                    }

                    return RedirectToAction("PercentualeMaggAbitazione", new { escludiAnnullati = escludiAnnullati, idLivello = ibm.idLivello });
                }
                else
                {
                    using (dtLivelli dtl = new dtLivelli())
                    {
                        var lm = dtl.GetLivelli(ibm.idLivello);
                        ViewBag.Livello = lm;
                    }

                    using (dtUffici dtl = new dtUffici())
                    {
                        var lm = dtl.GetUffici(ibm.idUfficio);
                        ViewBag.Descrizione = lm;
                    }

                    ViewBag.escludiAnnullati = escludiAnnullati;
                    return PartialView("NuovaPercentualeMaggAbitazione", ibm);
                    

                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaPercMaggAbitazione(bool escludiAnnullati, decimal idLivello, decimal idUfficio, decimal idPercMabAbitaz)
        {

            try
            {
                using (dtPercMaggAbitazione dtib = new dtPercMaggAbitazione())
                {
                    dtib.DelPercMaggAbitazione(idPercMabAbitaz);
                }

                return RedirectToAction("PercentualeMaggAbitazione", new { escludiAnnullati = escludiAnnullati, idLivello = idLivello, idUfficio = idUfficio });
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial");
            }


        }
    }
}
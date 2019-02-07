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
    public class ParamPercDisagioController : Controller
    {
        // GET: /Parametri/ParamPercDisagio/PercentualeDisagio

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult PercentualeDisagio(bool escludiAnnullati, decimal idLivello = 0)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<PercentualeDisagioModel> libm = new List<PercentualeDisagioModel>();
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

                using (dtParPercentualeDisagio dtib = new dtParPercentualeDisagio())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_PercentualeDisaggioNonAnnullato(idLivello);
                    libm = dtib.getListPercentualeDisaggio(idLivello, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
        public ActionResult PercentualeDisagioLivello(decimal idUfficio, bool escludiAnnullati)
        {
            List<PercentualeDisagioModel> libm = new List<PercentualeDisagioModel>();
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
                    ViewBag.LivelliList = r;
                }

                using (dtParPercentualeDisagio dtib = new dtParPercentualeDisagio())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_PercentualeDisaggioNonAnnullato(idUfficio);
                    libm = dtib.getListPercentualeDisaggio(idUfficio, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("PercentualeDisagio", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaPercentualeDisagio(decimal idUfficio, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();

            try
            {
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
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciPercentualeDisagio(PercentualeDisagioModel ibm, bool escludiAnnullati = true, bool aggiornaTutto = false)
        {
            var r = new List<SelectListItem>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<PercentualeDisagioModel> libm = new List<PercentualeDisagioModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtParPercentualeDisagio dtib = new dtParPercentualeDisagio())
                    {
                        dtib.SetPercentualeDisagio(ibm, aggiornaTutto);
                    }
                    AggiornaLivelliList(ibm.idUfficio);
                    //return RedirectToAction("PercentualeDisagio", new { escludiAnnullati = escludiAnnullati, idUfficio = ibm.idUfficio });
                    using (dtParPercentualeDisagio dtib = new dtParPercentualeDisagio())
                    {
                        ViewBag.idMinimoNonAnnullato = dtib.Get_Id_PercentualeDisaggioNonAnnullato(ibm.idUfficio);
                        libm = dtib.getListPercentualeDisaggio(ibm.idUfficio, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    return PartialView("PercentualeDisagio", libm);
                }
                else
                {
                    using (dtUffici dtl = new dtUffici())
                    {
                        var lm = dtl.GetUffici(ibm.idUfficio);
                        ViewBag.Descrizione = lm;
                    }
                    ViewBag.escludiAnnullati = escludiAnnullati;

                    return PartialView("NuovaPercentualeDisagio", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]

        public ActionResult EliminaPercentualeDisagio(bool escludiAnnullati, decimal idUfficio, decimal idPercDisagio)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<PercentualeDisagioModel> libm = new List<PercentualeDisagioModel>();
            try
            {
                using (dtParPercentualeDisagio dtib = new dtParPercentualeDisagio())
                {
                    dtib.DelPercentualeDisagio(idPercDisagio);
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_PercentualeDisaggioNonAnnullato(idUfficio);
                    libm = dtib.getListPercentualeDisaggio(idUfficio, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
                AggiornaLivelliList(idUfficio);

                return PartialView("PercentualeDisagio", libm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        private void AggiornaLivelliList(decimal idUfficio)
        {
            var r = new List<SelectListItem>();
            List<PercentualeDisagioModel> libm = new List<PercentualeDisagioModel>();
            using (dtUffici dtl = new dtUffici())
            {
                var llm = dtl.GetUffici().OrderBy(a => a.descUfficio).ToList();

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
        }

    }
}
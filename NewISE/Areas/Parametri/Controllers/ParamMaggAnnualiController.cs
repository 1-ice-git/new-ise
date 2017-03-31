using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
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
        public ActionResult MaggiorazioniAnnuali(bool escludiAnnullati, decimal idUfficio = 0)
        {
            List<MaggiorazioniAnnualiModel> libm = new List<MaggiorazioniAnnualiModel>();
            var r = new List<SelectListItem>();
            List<UfficiModel> llm = new List<UfficiModel>();

            try
            {
                using (dtUffici dtl = new dtUffici())
                {
                    llm = dtl.GetUffici().OrderBy(a => a.DescUfficio).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
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

                using (dtMaggAnnuali dtib = new dtMaggAnnuali())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListMaggiorazioneAnnuale(idUfficio, escludiAnnullati).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListMaggiorazioneAnnuale(idUfficio).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
        public ActionResult MaggiorazioneAnnualeLivello(decimal idUfficio, bool escludiAnnullati)
        {
            List<MaggiorazioniAnnualiModel> libm = new List<MaggiorazioniAnnualiModel>();
            var r = new List<SelectListItem>();
            List<UfficiModel> llm = new List<UfficiModel>();

            try
            {
                using (dtUffici dtl = new dtUffici())
                {
                    llm = dtl.GetUffici().OrderBy(a => a.DescUfficio).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.DescUfficio,
                                 Value = t.idUfficio.ToString()
                             }).ToList();
                        r.Where(a => a.Value == idUfficio.ToString()).First().Selected = true;
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtMaggAnnuali dtib = new dtMaggAnnuali())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListMaggiorazioneAnnuale(llm.Where(a => a.idUfficio == idUfficio).First().idUfficio, escludiAnnullati).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListMaggiorazioneAnnuale(llm.Where(a => a.idUfficio == idUfficio).First().idUfficio).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("MaggiorazioniAnnuali", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaMaggiorazioneAnnuale(decimal idUfficio, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();

            //var r = new SelectList(new[]
            //{
            //    new { ID = "0", Name = "NO" },
            //    new { ID = "1", Name = "SI" },
                
            //},
            //    "ID", "Name", 1);

            

            try
            {
                using (dtUffici dtl = new dtUffici())
                {
                    var lm = dtl.GetUffici(idUfficio);
                    ViewBag.Descrizione = lm;
                }


                ViewBag.escludiAnnullati = escludiAnnullati;
                //ViewData["list"] = r;
                return PartialView();


                
                //return View();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult InserisciMaggiorazioneAnnuale(MaggiorazioniAnnualiModel ibm, bool escludiAnnullati = true)
        {
            var r = new List<SelectListItem>();

            try
            {
                if (ModelState.IsValid)
                {
                    using (dtMaggAnnuali dtib = new dtMaggAnnuali())
                    {
                        dtib.SetMaggiorazioneAnnuale(ibm);
                    }

                    return RedirectToAction("MaggiorazioniAnnuali", new { escludiAnnullati = escludiAnnullati, idUfficio = ibm.idUfficio });
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
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaMaggiorazioneAnnuale(bool escludiAnnullati, decimal idUfficio, decimal idMaggAnnuale)
        {
            try
            {
                using (dtMaggAnnuali dtib = new dtMaggAnnuali())
                {
                    dtib.DelMaggiorazioneAnnuale(idMaggAnnuale);
                }

                return RedirectToAction("MaggiorazioneAnnuale", new { escludiAnnullati = escludiAnnullati, idUfficio = idUfficio });
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial");
            }


        }






    }
}
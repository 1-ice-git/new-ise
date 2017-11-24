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
    public class ParamCoeffIndRichiamoController : Controller
    {
        // GET: Parametri/ParamCoeffIndRichiamo

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult CoeffIndRichiamo(bool escludiAnnullati, decimal idRiduzioni = 0)
        {
            List<CoefficienteRichiamoModel> libm = new List<CoefficienteRichiamoModel>();
            var r = new List<SelectListItem>();
            List<RiduzioniModel> llm = new List<RiduzioniModel>();

            try
            {
                using (dtParCoeffRiduzioni dtl = new dtParCoeffRiduzioni())
                {
                    llm = dtl.GetCoeffRiduzioni().OrderBy(a => a.idRiduzioni).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.percentuale.ToString(),
                                 Value = t.idRiduzioni.ToString()
                             }).ToList();

                        if (idRiduzioni == 0)
                        {
                            r.First().Selected = true;
                            idRiduzioni = Convert.ToDecimal(r.First().Value);
                        }
                        else
                        {
                            r.Where(a => a.Value == idRiduzioni.ToString()).First().Selected = true;
                        }
                    }

                    ViewBag.CoeffIndRichiamo = r;
                }



                using (dtParCoeffIndRichiamo dtib = new dtParCoeffIndRichiamo())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        //libm = dtib.getListCoeffFasciaKm(idDefKm, escludiAnnullati).OrderBy(a => a.idDefKm).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        //libm = dtib.getListCoeffFasciaKm(idDefKm).OrderBy(a => a.idDefKm).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
        public ActionResult CoeffIndRichiamoLivello(decimal idRiduzioni, bool escludiAnnullati)
        {
            List<CoefficienteRichiamoModel> libm = new List<CoefficienteRichiamoModel>();
            var r = new List<SelectListItem>();
            List<RiduzioniModel> llm = new List<RiduzioniModel>();

            try
            {
                using (dtParCoeffRiduzioni dtl = new dtParCoeffRiduzioni())
                {
                    llm = dtl.GetCoeffRiduzioni().OrderBy(a => a.percentuale).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.percentuale.ToString(),
                                 Value = t.idRiduzioni.ToString()
                             }).ToList();
                        r.Where(a => a.Value == idRiduzioni.ToString()).First().Selected = true;
                    }

                    ViewBag.CoeffIndRichiamo = r;
                }

                using (dtParCoeffIndRichiamo dtib = new dtParCoeffIndRichiamo())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        //libm = dtib.getListIndennitaBase(llm.Where(a => a.idLivello == idLivello).First().idLivello, escludiAnnullati).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        //libm = dtib.getListIndennitaBase(llm.Where(a => a.idLivello == idLivello).First().idLivello).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("CoeffIndRichiamo", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovoCoeffIndRichiamo(decimal idRiduzioni, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();
            CoefficienteRichiamoModel ibm = new CoefficienteRichiamoModel();

            try
            {
                using (dtParCoeffRiduzioni dtl = new dtParCoeffRiduzioni())
                {
                    var lm = dtl.GetCoeffRiduzioni(idRiduzioni);
                    ViewBag.CoeffIndRichiamo = lm;
                }
                ViewBag.escludiAnnullati = escludiAnnullati;
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
    }
}
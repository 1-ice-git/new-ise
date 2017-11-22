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
    public class ParamCoeffFasciaKmController : Controller
    {
        // GET: Parametri/ParamCoeffFasciaKm/CoefficienteFasciaKm

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        
        public ActionResult CoefficienteFasciaKm(bool escludiAnnullati, decimal idDefKm = 0)
        {
            List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();
            var r = new List<SelectListItem>();
            List<DefFasciaKmModel> llm = new List<DefFasciaKmModel>();

            try
            {
                using (DefFasciaKm dtl = new DefFasciaKm())
                {
                    llm = dtl.GetFasciaKm().OrderBy(a => a.km).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {

                                 Text = t.km,
                                 Value = t.idDefKm.ToString()

                             }).ToList();

                        if (idDefKm == 0)
                        {
                            r.First().Selected = true;
                            idDefKm = Convert.ToDecimal(r.First().Value);
                        }
                        else
                        {
                            r.Where(a => a.Value == idDefKm.ToString()).First().Selected = true;
                        }
                    }

                    ViewBag.CoeffFasciaKm = r;
                }

                using (DefFasciaKm dtib = new DefFasciaKm())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        //libm = dtib.getListAliquoteContributive(idAliqContr, escludiAnnullati).OrderBy(a => a.idAliqContr).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        //libm = dtib.getListAliquoteContributive(idAliqContr).OrderBy(a => a.idAliqContr).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
    }
}
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
                using (dtParDefFasciaKm dtl = new dtParDefFasciaKm())
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

                

                using (dtParCoefficienteKm dtib = new dtParCoefficienteKm())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListCoeffFasciaKm(idDefKm, escludiAnnullati).OrderBy(a => a.idDefKm).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListCoeffFasciaKm(idDefKm).OrderBy(a => a.idDefKm).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
        public ActionResult CoefficienteFasciaKmLivello(decimal idDefKm, bool escludiAnnullati)
        {
            List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();
            var r = new List<SelectListItem>();
            List<DefFasciaKmModel> llm = new List<DefFasciaKmModel>();

            try
            {
                using (dtParDefFasciaKm dtl = new dtParDefFasciaKm())
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
                        r.Where(a => a.Value == idDefKm.ToString()).First().Selected = true;
                    }

                    ViewBag.CoeffFasciaKm = r;
                }

                using (dtParCoefficienteKm dtib = new dtParCoefficienteKm())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListCoeffFasciaKm(llm.Where(a => a.idDefKm == idDefKm).First().idDefKm, escludiAnnullati).OrderBy(a => a.idDefKm).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListCoeffFasciaKm(llm.Where(a => a.idDefKm == idDefKm).First().idDefKm).OrderBy(a => a.idDefKm).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("CoefficienteFasciaKm", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovoCoefficienteFasciaKm(decimal idDefKm, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();
            
            CoeffFasciaKmModel ibm = new CoeffFasciaKmModel();

            try
            {
                using (dtParDefFasciaKm dtl = new dtParDefFasciaKm())
                {
                    var lm = dtl.GetFasciaKm(idDefKm);
                    ViewBag.CoeffFasciaKm = lm;
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
        public ActionResult InserisciCoeffFasciaKm(CoeffFasciaKmModel ibm, bool escludiAnnullati = true)
        {
            var r = new List<SelectListItem>();

            try
            {
                if (ModelState.IsValid)
                {
                    using (dtParCoefficienteKm dtib = new dtParCoefficienteKm())
                    {
                        dtib.SetCoeffFasciaKm(ibm);
                    }

                    return RedirectToAction("CoefficienteFasciaKm", new { escludiAnnullati = escludiAnnullati, idDefKm = ibm.idDefKm });
                }
                else
                {
                    using (dtParDefFasciaKm dtl = new dtParDefFasciaKm())
                    {
                        var lm = dtl.GetFasciaKm(ibm.idDefKm);
                        ViewBag.CoeffFasciaKm = lm;
                    }
                    ViewBag.escludiAnnullati = escludiAnnullati;
                    return PartialView("NuovoCoeffFasciakm", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaIndennitaBase(bool escludiAnnullati, decimal idCfKm, decimal idDefKm)
        {

            try
            {
                using (dtParCoefficienteKm dtib = new dtParCoefficienteKm())
                {
                    dtib.DelCoeffFasciaKm(idDefKm);
                }

                return RedirectToAction("CoefficienteFasciaKm", new { escludiAnnullati = escludiAnnullati, idDefKm = idDefKm });
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }
    }
}
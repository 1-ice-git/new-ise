using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class CoefficientiSedeController : Controller
    {
        // GET: /Parametri/CoefficientiSede/CoefficientiSede

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult CoefficientiSede(bool escludiAnnullati, decimal idUfficio = 0)
        {
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();
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

                using (dtCoefficientiSede dtib = new dtCoefficientiSede())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListCoefficientiSede(idUfficio, escludiAnnullati).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListCoefficientiSede(idUfficio).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
        public ActionResult CoefficientiSedeLivello(decimal idUfficio, bool escludiAnnullati)
        {
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();
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

                using (dtCoefficientiSede dtib = new dtCoefficientiSede())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListCoefficientiSede(llm.Where(a => a.idUfficio == idUfficio).First().idUfficio, escludiAnnullati).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListCoefficientiSede(llm.Where(a => a.idUfficio == idUfficio).First().idUfficio).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("CoefficientiSede", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovoCoefficienteSede(decimal idUfficio, bool escludiAnnullati)
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
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult InserisciCoefficientiSede(CoefficientiSedeModel ibm, bool escludiAnnullati = true)
        {
            var r = new List<SelectListItem>();

            try
            {
                if (ModelState.IsValid)
                {
                    using (dtCoefficientiSede dtib = new dtCoefficientiSede())
                    {
                        dtib.SetCoefficientiSede(ibm);
                    }

                    return RedirectToAction("CoefficientiSede", new { escludiAnnullati = escludiAnnullati, idUfficio = ibm.idUfficio });
                }
                else
                {
                    using (dtUffici dtl = new dtUffici())
                    {
                        var lm = dtl.GetUffici(ibm.idUfficio);
                        ViewBag.Descrizione = lm;
                    }
                    ViewBag.escludiAnnullati = escludiAnnullati;
                    return PartialView("NuovoCoefficienteSede", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaCoefficientiSede(bool escludiAnnullati, decimal idUfficio, decimal idPercDisagio)
        {
            try
            {
                using (dtCoefficientiSede dtib = new dtCoefficientiSede())
                {
                    dtib.DelCoefficientiSede(idPercDisagio);
                }

                return RedirectToAction("CoefficientiSede", new { escludiAnnullati = escludiAnnullati, idUfficio = idUfficio });
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial");
            }


        }

    }
}
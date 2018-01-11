using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using NewISE.EF;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
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
        public ActionResult CoefficientiSede(bool escludiAnnullati, decimal idLivello = 0)
        {
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();
            var r = new List<SelectListItem>();
            List<UfficiModel> llm = new List<UfficiModel>();
            ViewBag.escludiAnnullati = escludiAnnullati;
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
                            r.Where(a => a.Value == idLivello.ToString()).First().Selected = true;
                        }
                    }
                    ViewBag.LivelliList = r;
                }

                using (dtParCoefficientiSede dtib = new dtParCoefficientiSede())
                {
                    //if (escludiAnnullati)
                    //{
                    //    escludiAnnullati = false;
                        libm = dtib.getListCoefficientiSede(idLivello, escludiAnnullati).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    
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
        public ActionResult CoefficientiSedeLivello(decimal idUfficio, bool escludiAnnullati)
        {
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();
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
                        r.Where(a => a.Value == idUfficio.ToString()).First().Selected = true;
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtParCoefficientiSede dtib = new dtParCoefficientiSede())
                {
                    //if (escludiAnnullati)
                    //{
                    //    escludiAnnullati = false;
                        libm = dtib.getListCoefficientiSede(llm.Where(a => a.idUfficio == idUfficio).First().idUfficio, escludiAnnullati).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    //}
                    //else
                    //{
                    //    libm = dtib.getListCoefficientiSede(llm.Where(a => a.idUfficio == idUfficio).First().idUfficio).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    //}
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("CoefficientiSede", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovoCoefficienteSede(decimal idUfficio, bool escludiAnnullati)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
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
        public ActionResult InserisciCoefficientiSede(CoefficientiSedeModel ibm, bool escludiAnnullati = true)
        {
            var r = new List<SelectListItem>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();
            try 
            {
                if (ModelState.IsValid)
                {
                    using (dtParCoefficientiSede dtib = new dtParCoefficientiSede())
                    {
                        dtib.SetCoefficientiSede(ibm);
                                         
                        libm = dtib.getListCoefficientiSede(ibm.idUfficio, escludiAnnullati).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
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
                            r.Where(a => a.Value == ibm.idUfficio.ToString()).First().Selected = true;
                        }
                        ViewBag.LivelliList = r;
                    }
                    return PartialView("CoefficientiSede", libm);
                    //return RedirectToAction("CoefficientiSede", new { escludiAnnullati = escludiAnnullati, idUfficio = ibm.idUfficio });
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
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        private void AggiornaLivelliList(decimal idUfficio)
        {
            var r = new List<SelectListItem>();
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();
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
        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaCoefficienteSede(bool escludiAnnullati, decimal idUfficio, decimal idCoefficienteSede)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();
            try
            {
                using (dtParCoefficientiSede dtib = new dtParCoefficientiSede())
                {
                    dtib.DelCoefficientiSede(idCoefficienteSede);
                    libm = dtib.getListCoefficientiSede(idUfficio, escludiAnnullati).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
                AggiornaLivelliList(idUfficio);
                return PartialView("CoefficientiSede", libm);
                //return RedirectToAction("CoefficientiSede", new { escludiAnnullati = escludiAnnullati, idUfficio = idUfficio });
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
    }
}
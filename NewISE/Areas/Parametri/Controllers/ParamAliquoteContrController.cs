using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamAliquoteContrController : Controller
    {
        // GET: Parametri/ParamAliquoteContr/AliquoteContributive

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult AliquoteContributive(bool escludiAnnullati, decimal idAliqContr = 0)
        {
            List<AliquoteContributiveModel> libm = new List<AliquoteContributiveModel>();
            var r = new List<SelectListItem>();
            List<TipoAliquoteContributiveModel> llm = new List<TipoAliquoteContributiveModel>();

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

                using (dtIndennitaBase dtib = new dtIndennitaBase())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListIndennitaBase(idLivello, escludiAnnullati).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListIndennitaBase(idLivello).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
        public ActionResult AliquoteContributiveLivello(decimal idAliqContr, bool escludiAnnullati)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();
            var r = new List<SelectListItem>();
            List<LivelloModel> llm = new List<LivelloModel>();

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

                using (dtIndennitaBase dtib = new dtIndennitaBase())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListIndennitaBase(llm.Where(a => a.idLivello == idLivello).First().idLivello, escludiAnnullati).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListIndennitaBase(llm.Where(a => a.idLivello == idLivello).First().idLivello).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaAliquoteContributive(decimal idAliqContr, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();
           
            try
            {
                using (dtLivelli dtl = new dtLivelli())
                {
                    var lm = dtl.GetLivelli(idLivello);
                    ViewBag.Livello = lm;
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
        public ActionResult InserisciAliquoteContributive(IndennitaBaseModel ibm, bool escludiAnnullati = true)
        {
            var r = new List<SelectListItem>();

            try
            {
                if (ModelState.IsValid)
                {
                    using (dtIndennitaBase dtib = new dtIndennitaBase())
                    {
                        
                        dtib.SetIndennitaDiBase(ibm);
                    }

                    return RedirectToAction("IndennitaBase", new { escludiAnnullati = escludiAnnullati, idLivello = ibm.idLivello });
                }
                else
                {
                    using (dtLivelli dtl = new dtLivelli())
                    {
                        var lm = dtl.GetLivelli(ibm.idLivello);
                        ViewBag.Livello = lm;
                    }
                    ViewBag.escludiAnnullati = escludiAnnullati;
                    return PartialView("NuovaIndennitaBase", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaAliquoteContributive(bool escludiAnnullati, decimal idAliqContr, decimal idTipoAliqContr)
        {

            try
            {
                using (dtIndennitaBase dtib = new dtIndennitaBase())
                {
                    dtib.DelIndennitaDiBase(idIndBase);
                }

                return RedirectToAction("IndennitaBase", new { escludiAnnullati = escludiAnnullati, idLivello = idLivello });
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial");
            }


        }


    }
}
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
                using (dtParTipoAliquoteContributive dtl = new dtParTipoAliquoteContributive())
                {
                    llm = dtl.GetTipoAliquote().OrderBy(a => a.descrizione).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {

                                 Text = t.descrizione,
                                 Value = t.idTipoAliqContr.ToString()

                             }).ToList();

                        if (idAliqContr == 0)
                        {
                            r.First().Selected = true;
                            idAliqContr = Convert.ToDecimal(r.First().Value);
                        }
                        else
                        {
                            r.Where(a => a.Value == idAliqContr.ToString()).First().Selected = true;
                        }
                    }

                    ViewBag.Aliquote = r;
                }

                using (dtAliquoteContr dtib = new dtAliquoteContr())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListAliquoteContributive(idAliqContr, escludiAnnullati).OrderBy(a => a.idAliqContr).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListAliquoteContributive(idAliqContr).OrderBy(a => a.idAliqContr).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
        public ActionResult AliquoteContributiveLivello(decimal idTipoContributo, bool escludiAnnullati)
        {
            List<AliquoteContributiveModel> libm = new List<AliquoteContributiveModel>();
            var r = new List<SelectListItem>();
            List<TipoAliquoteContributiveModel> llm = new List<TipoAliquoteContributiveModel>();

            try
            {
                using (dtTipoAliquoteContributive dtl = new dtTipoAliquoteContributive())
                {
                    llm = dtl.GetTipoAliquote().OrderBy(a => a.descrizione).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.descrizione,
                                 Value = t.idTipoAliqContr.ToString()
                             }).ToList();
                        r.Where(a => a.Value == idTipoContributo.ToString()).First().Selected = true;
                    }

                    ViewBag.Aliquote = r;
                }

                using (dtAliquoteContr dtib = new dtAliquoteContr())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListAliquoteContributive(llm.Where(a => a.idTipoAliqContr == idTipoContributo).First().idTipoAliqContr, escludiAnnullati).OrderBy(a => a.idTipoContributo).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListAliquoteContributive(llm.Where(a => a.idTipoAliqContr == idTipoContributo).First().idTipoAliqContr).OrderBy(a => a.idTipoContributo).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("AliquoteContributive", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaAliquotaContributiva(decimal idTipoContributo, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();

            try
            {
                using (dtTipoAliquoteContributive dtl = new dtTipoAliquoteContributive())
                {
                    var lm = dtl.GetTipoAliquote(idTipoContributo);
                    ViewBag.descrizione = lm;
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
        public ActionResult InserisciAliquoteContributive(AliquoteContributiveModel ibm, bool escludiAnnullati = true)
        {
            var r = new List<SelectListItem>();

            try
            {
                if (ModelState.IsValid)
                {
                    using (dtAliquoteContr dtib = new dtAliquoteContr())
                    {

                        dtib.SetAliquoteContributive(ibm);
                    }

                    return RedirectToAction("AliquoteContributive", new { escludiAnnullati = escludiAnnullati, idTipoAliqContr = ibm.idTipoContributo });
                }
                else
                {
                    using (dtParTipoAliquoteContributive dtl = new dtParTipoAliquoteContributive())
                    {
                        var lm = dtl.GetTipoAliquote(ibm.idTipoContributo);
                        ViewBag.Livello = lm;
                    }
                    ViewBag.escludiAnnullati = escludiAnnullati;
                    return PartialView("NuovaAliquotaContributiva", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaAliquoteContributive(bool escludiAnnullati, decimal idAliqContr, decimal idTipoContributo)
        {

            try
            {
                using (dtParAliquoteContr dtib = new dtParAliquoteContr())
                {
                    dtib.DelAliquoteContributive(idTipoContributo);
                }

                return RedirectToAction("AliquoteContributive", new { escludiAnnullati = escludiAnnullati, idAliqContr = idAliqContr });
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }


    }
}
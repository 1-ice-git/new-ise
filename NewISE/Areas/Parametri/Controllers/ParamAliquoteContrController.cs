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

                    ViewBag.LivelliList = r;
                }

                using (dtParParAliquoteContr dtib = new dtParParAliquoteContr())
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
        public ActionResult AliquoteContributiveLivello(decimal idTipoAliqContr, bool escludiAnnullati)
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
                        r.Where(a => a.Value == idTipoAliqContr.ToString()).First().Selected = true;
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtParParAliquoteContr dtib = new dtParParAliquoteContr())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListAliquoteContributive(llm.Where(a => a.idTipoAliqContr == idTipoAliqContr).First().idTipoAliqContr, escludiAnnullati).OrderBy(a => a.idTipoContributo).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListAliquoteContributive(llm.Where(a => a.idTipoAliqContr == idTipoAliqContr).First().idTipoAliqContr).OrderBy(a => a.idTipoContributo).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("AliquoteContributive", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaAliquotaContributiva(decimal idTipoAliqContr, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();

            try
            {
                using (dtParTipoAliquoteContributive dtl = new dtParTipoAliquoteContributive())
                {
                    var lm = dtl.GetTipoAliquote(idTipoAliqContr);
                    ViewBag.descrizione = lm;
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
        public ActionResult InserisciAliquoteContributive(AliquoteContributiveModel ibm, bool escludiAnnullati = true)
        {
            var r = new List<SelectListItem>();

            try
            {
                if (ModelState.IsValid)
                {
                    using (dtParParAliquoteContr dtib = new dtParParAliquoteContr())
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
        public ActionResult EliminaAliquoteContributive(bool escludiAnnullati, decimal idAliqContr, decimal idTipoAliqContr)
        {

            try
            {
                using (dtParParAliquoteContr dtib = new dtParParAliquoteContr())
                {
                    dtib.DelAliquoteContributive(idTipoAliqContr);
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
using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamTFRController : Controller
    {
        // GET: Parametri/ParamTFR
        public ActionResult TFR(bool escludiAnnullati, decimal idValuta = 0)
        {
            List<TFRModel> libm = new List<TFRModel>();
            var r = new List<SelectListItem>();
            List<ValuteModel> llm = new List<ValuteModel>();

            try
            {
                using (dtValute dtl = new dtValute())
                {
                    llm = dtl.GetValute().OrderBy(a => a.descrizioneValuta).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.descrizioneValuta,
                                 Value = t.idValuta.ToString()
                             }).ToList();

                        if (idValuta == 0)
                        {
                            r.First().Selected = true;
                            idValuta = Convert.ToDecimal(r.First().Value);
                        }
                        else
                        {
                            r.Where(a => a.Value == idValuta.ToString()).First().Selected = true;
                        }
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtTfr dtib = new dtTfr())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListTfr(idValuta, escludiAnnullati).OrderBy(a => a.idValuta).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListTfr(idValuta).OrderBy(a => a.idValuta).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
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
        public ActionResult TfrLivello(decimal idValuta, bool escludiAnnullati)
        {
            List<TFRModel> libm = new List<TFRModel>();
            var r = new List<SelectListItem>();
            List<ValuteModel> llm = new List<ValuteModel>();

            try
            {
                using (dtValute dtl = new dtValute())
                {
                    llm = dtl.GetValute().OrderBy(a => a.descrizioneValuta).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.descrizioneValuta,
                                 Value = t.idValuta.ToString()
                             }).ToList();
                        r.Where(a => a.Value == idValuta.ToString()).First().Selected = true;
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtTfr dtib = new dtTfr())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListTfr(llm.Where(a => a.idValuta == idValuta).First().idValuta, escludiAnnullati).OrderBy(a => a.idValuta).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListTfr(llm.Where(a => a.idValuta == idValuta).First().idValuta).OrderBy(a => a.idValuta).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("TFR", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovoTFR(decimal idValuta, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();

            try
            {
                using (dtValute dtl = new dtValute())
                {
                    var lm = dtl.GetValute(idValuta);
                    ViewBag.DescrizioneValuta = lm;
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
        public ActionResult InserisciTFR(TFRModel ibm, bool escludiAnnullati = true)
        {
            var r = new List<SelectListItem>();

            try
            {
                if (ModelState.IsValid)
                {
                    using (dtTfr dtib = new dtTfr())
                    {

                        dtib.SetTfr(ibm);
                    }

                    return RedirectToAction("TFR", new { escludiAnnullati = escludiAnnullati, idValuta = ibm.idValuta });
                }
                else
                {
                    using (dtValute dtl = new dtValute())
                    {
                        var lm = dtl.GetValute(ibm.idValuta);
                        ViewBag.DescrizioneValuta = lm;
                    }
                    ViewBag.escludiAnnullati = escludiAnnullati;
                    return PartialView("NuovoTFR", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaTFR(bool escludiAnnullati, decimal idValuta, decimal idTFR)
        {

            try
            {
                using (dtTfr dtib = new dtTfr())
                {
                    dtib.DelTfr(idTFR);
                }

                return RedirectToAction("TFR", new { escludiAnnullati = escludiAnnullati, idValuta = idValuta });
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial");
            }


        }
    }
}
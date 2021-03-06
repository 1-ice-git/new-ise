﻿using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NewISE.Models;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamValuteController : Controller
    {
        // GET: Parametri/ParamValute/ParametriValute

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult Valute(decimal idValuta = 0)
        {
            List<ValuteModel> libm = new List<ValuteModel>();
            var r = new List<SelectListItem>();
            List<ValuteModel> llm = new List<ValuteModel>();

            try
            {
                ListeValute();
                using (dtValute dtib = new dtValute())
                {
                    libm = dtib.getListValute(idValuta).OrderBy(a => a.idValuta).ToList();
                }
            }
            catch (Exception ex)
            {
                MsgErr msg = new MsgErr()
                {
                    msg = ex.Message
                };
                return PartialView("ErrorPartial", msg);
            }
            return PartialView(libm);
        }

        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public ActionResult ValutaLivello(decimal idValuta)
        {
            List<ValuteModel> libm = new List<ValuteModel>();
            var r = new List<SelectListItem>();
            List<ValuteModel> llm = new List<ValuteModel>();
            try
            {
                ListeValute();
                using (dtValute dtib = new dtValute())
                {
                    libm = dtib.getListValute(idValuta).OrderBy(a => a.idValuta).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
            //ViewBag.escludiAnnullati = escludiAnnullati;
            return PartialView("Valute", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuoveValute(decimal idValuta)
        {
            var r = new List<SelectListItem>();
            try
            {
                using (dtValute dtl = new dtValute())
                {
                    var lm = dtl.GetValute(idValuta);
                    ViewBag.descrizionevaluta = lm;
                }

                ViewBag.idValuta = idValuta;
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }
        public decimal ListeValute()
        {
            decimal idValuta = 0;
            List<ValuteModel> llm = new List<ValuteModel>();
            var r = new List<SelectListItem>();
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
                ViewBag.Valute = r;
            }
            return idValuta;
        }
        [HttpPost]
        [Authorize(Roles = "1, 2")]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciValuta(ValuteModel ibm)
        {
            var r = new List<SelectListItem>();
            List<ValuteModel> libm = new List<ValuteModel>();
            List<ValuteModel> llm = new List<ValuteModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtValute dtib = new dtValute())
                    {
                        dtib.SetValute(ibm);
                    }
                    ListeValute();
                    using (dtValute dtib = new dtValute())
                    {
                        libm = dtib.getListValute().OrderBy(a => a.idValuta).ToList();
                    }
                    return PartialView("Valute", libm);
                }
                else
                {
                    using (dtValute dtl = new dtValute())
                    {
                        var lm = dtl.GetValute(ibm.idValuta);
                        ViewBag.descrizionevaluta = lm;
                    }
                    return PartialView("NuoveValute", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaValuta(decimal idValuta)
        {
            List<ValuteModel> libm = new List<ValuteModel>();
            try
            {
                using (dtValute dtib = new dtValute())
                {
                    dtib.DelValute(idValuta);
                }
                ListeValute();
                using (dtValute dtib = new dtValute())
                {
                    libm = dtib.getListValute(idValuta).OrderBy(a => a.descrizioneValuta).ToList();
                }
                return PartialView("Valute", libm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }
    }

}
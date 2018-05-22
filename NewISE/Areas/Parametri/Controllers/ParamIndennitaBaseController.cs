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
    public class ParamIndennitaBaseController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult IndennitaBase(bool escludiAnnullati, decimal idLivello = 0)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();
            var r = new List<SelectListItem>();
            List<LivelloModel> llm = new List<LivelloModel>();
            List<RiduzioniModel> lrm = new List<RiduzioniModel>();
            try
            {
                idLivello = CaricaComboLivelli(idLivello);
                using (dtParIndennitaBase dtib = new dtParIndennitaBase())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_IndennitaBaseNonAnnullato(idLivello);
                    libm = dtib.getListIndennitaBase(idLivello, escludiAnnullati).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.idLivello = idLivello;
            ViewBag.escludiAnnullati = escludiAnnullati;
            return PartialView(libm);
        }

        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public ActionResult IndennitaBaseLivello(decimal idLivello, bool escludiAnnullati)
        {
            ViewBag.idLivello = idLivello;
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();
            var r = new List<SelectListItem>();
            List<LivelloModel> llm = new List<LivelloModel>();

            try
            {
                CaricaComboLivelli(idLivello);
                using (dtParIndennitaBase dtib = new dtParIndennitaBase())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_IndennitaBaseNonAnnullato(idLivello);
                    libm = dtib.getListIndennitaBase(idLivello, escludiAnnullati).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.escludiAnnullati = escludiAnnullati;
            return PartialView("IndennitaBase", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaIndennitaBase(decimal idLivello, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();
            //IndennitaBaseModel ibm = new IndennitaBaseModel();
            try
            {
                using (dtParLivelli dtl = new dtParLivelli())
                {
                    var lm = dtl.GetLivelli(idLivello);
                    ViewBag.Livello = lm;
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
        public ActionResult InserisciIndennitaBase(IndennitaBaseModel ibm, bool escludiAnnullati = true, bool aggiornaTutto = false)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            var r = new List<SelectListItem>();
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtParIndennitaBase dtib = new dtParIndennitaBase())
                    {
                        dtib.SetIndennitaDiBase(ibm, aggiornaTutto);
                    }
                    CaricaComboLivelli(ibm.idLivello);
                    using (dtParIndennitaBase dtib = new dtParIndennitaBase())
                    {
                        ViewBag.idMinimoNonAnnullato = dtib.Get_Id_IndennitaBaseNonAnnullato(ibm.idLivello);
                        libm = dtib.getListIndennitaBase(ibm.idLivello, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    return PartialView("IndennitaBase", libm);
                }
                else
                {
                    using (dtParLivelli dtl = new dtParLivelli())
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
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaIndennitaBase(bool escludiAnnullati, decimal idLivello, decimal idIndBase)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();
            try
            {
                using (dtParIndennitaBase dtib = new dtParIndennitaBase())
                {
                    dtib.DelIndennitaDiBase(idIndBase);
                }
                CaricaComboLivelli(idLivello);
                using (dtParIndennitaBase dtib = new dtParIndennitaBase())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_IndennitaBaseNonAnnullato(idLivello);
                    libm = dtib.getListIndennitaBase(idLivello, escludiAnnullati).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
                return PartialView("IndennitaBase", libm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        public decimal CaricaComboLivelli(decimal idLivello = 0)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();
            var r = new List<SelectListItem>();
            List<LivelloModel> llm = new List<LivelloModel>();
            using (dtParLivelli dtl = new dtParLivelli())
            {
                llm = dtl.GetLivelli().OrderBy(a => a.idLivello).ToList();
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
                        var temp = r.Where(a => a.Value == idLivello.ToString()).ToList();
                        if (temp.Count == 0)
                        {
                            r.First().Selected = true;
                            idLivello = Convert.ToDecimal(r.First().Value);
                        }
                        else
                            r.Where(a => a.Value == idLivello.ToString()).First().Selected = true;
                    }
                }
                ViewBag.LivelliList = r;
            }
            return idLivello;
        }
    }
}
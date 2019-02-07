using NewISE.Areas.Parametri.Models.dtObj;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.Enumeratori;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamCoeffIndRichiamoController : Controller
    {
        // GET: Parametri/ParamCoeffIndRichiamo

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult CoeffIndRichiamo(bool escludiAnnullati, decimal idLivello = 0, decimal idUfficio = 0)
        {
            List<CoefficienteRichiamoModel> crm = new List<CoefficienteRichiamoModel>();
            List<RiduzioniModel> llm = new List<RiduzioniModel>();
            var r = new List<SelectListItem>();
            List<TipoCoefficienteRichiamoModel> ltcrm = new List<TipoCoefficienteRichiamoModel>();

            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                using (dtParCoeffIndRichiamo dtpcir = new dtParCoeffIndRichiamo())
                {
                    ltcrm = dtpcir.ListTipoCoeffIndRichiamo().ToList();

                    if (ltcrm != null && ltcrm.Count > 0)
                    {
                        r = (from tcrm in ltcrm
                             select new SelectListItem()
                             {
                                 Text = tcrm.descrizione,
                                 Value = tcrm.idTipoCoefficienteRichiamo.ToString()
                             }).ToList();
                    }

                    ViewBag.TipoCoeffRichiamoList = r;

                    decimal idTipoCoeffRichiamo = idLivello > 0 ? idLivello : (decimal)EnumTipoCoefficienteRichiamo.CoefficienteMaggiorazione;

                    ViewBag.idTipoCoeffRichiamo = idTipoCoeffRichiamo;
                    ViewBag.idMinimoNonAnnullato = dtpcir.Get_Id_CoeffIndRichiamoPrimoNonAnnullato(idTipoCoeffRichiamo);

                    crm = dtpcir.GetListCoeffRichiamo_x_Tipo(idTipoCoeffRichiamo, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.escludiAnnullati = escludiAnnullati;
            return PartialView(crm);
        }

        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public ActionResult CoeffTipoRichiamo(decimal idTipoCoeffRichiamo, bool escludiAnnullati)
        {
            List<CoefficienteRichiamoModel> lcrm = new List<CoefficienteRichiamoModel>();
            var r = new List<SelectListItem>();
            List<TipoCoefficienteRichiamoModel> ltcrm = new List<TipoCoefficienteRichiamoModel>();

            ViewBag.escludiAnnullati = escludiAnnullati;

            ViewBag.idTipoCoeffRichiamo = idTipoCoeffRichiamo;
            try
            {
                using (dtParCoeffIndRichiamo dtpcir = new dtParCoeffIndRichiamo())
                {
                    ltcrm = dtpcir.ListTipoCoeffIndRichiamo().ToList();

                    if (ltcrm != null && ltcrm.Count > 0)
                    {
                        r = (from tcrm in ltcrm
                             select new SelectListItem()
                             {
                                 Text = tcrm.descrizione,
                                 Value = tcrm.idTipoCoefficienteRichiamo.ToString()
                             }).ToList();
                    }

                    ViewBag.TipoCoeffRichiamoList = r;

                    lcrm = dtpcir.GetListCoeffRichiamo_x_Tipo(idTipoCoeffRichiamo, escludiAnnullati)
                                                    .OrderBy(a => a.dataInizioValidita)
                                                    .ThenBy(a => a.dataFineValidita)
                                                    .ToList();

                    ViewBag.idMinimoNonAnnullato = dtpcir.Get_Id_CoeffIndRichiamoPrimoNonAnnullato(idTipoCoeffRichiamo);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("CoeffIndRichiamo", lcrm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovoCoeffIndRichiamo(decimal idTipoCoeffRichiamo, bool escludiAnnullati)
        {
            try
            {
                ViewBag.escludiAnnullati = escludiAnnullati;
                using (dtParCoeffIndRichiamo dtpcir = new dtParCoeffIndRichiamo())
                {
                    ViewBag.idMinimoNonAnnullato = dtpcir.Get_Id_CoeffIndRichiamoPrimoNonAnnullato(idTipoCoeffRichiamo);
                    var tcr = dtpcir.GetTipoCoeffRichiamo(idTipoCoeffRichiamo);
                    ViewBag.TipoCoeffRichiamo = tcr.DESCRIZIONE;
                    ViewBag.idTipoCoeffRichiamo = tcr.IDTIPOCOEFFICIENTERICHIAMO;
                }

                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaCoeffIndRichiamo(bool escludiAnnullati, decimal idCoeffIndRichiamo, decimal idTipoCoeffIndRichiamo)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            List<CoefficienteRichiamoModel> libm = new List<CoefficienteRichiamoModel>();
            List<RiduzioniModel> llm = new List<RiduzioniModel>();
            var r = new List<SelectListItem>();
            try
            {
                using (dtParCoeffIndRichiamo dtpcir = new dtParCoeffIndRichiamo())
                {
                    dtpcir.DelCoefficienteRichiamo(idCoeffIndRichiamo);

                    List<TipoCoefficienteRichiamoModel> ltcrm = new List<TipoCoefficienteRichiamoModel>();

                    ViewBag.escludiAnnullati = escludiAnnullati;

                    ltcrm = dtpcir.ListTipoCoeffIndRichiamo().ToList();

                    if (ltcrm != null && ltcrm.Count > 0)
                    {
                        r = (from tcrm in ltcrm
                             select new SelectListItem()
                             {
                                 Text = tcrm.descrizione,
                                 Value = tcrm.idTipoCoefficienteRichiamo.ToString()
                             }).ToList();
                    }

                    ViewBag.TipoCoeffRichiamoList = r;

                    ViewBag.idTipoCoeffRichiamo = idTipoCoeffIndRichiamo;

                    ViewBag.idMinimoNonAnnullato = dtpcir.Get_Id_CoeffIndRichiamoPrimoNonAnnullato(idTipoCoeffIndRichiamo);

                    libm = dtpcir.GetListCoeffRichiamo_x_Tipo(idTipoCoeffIndRichiamo, escludiAnnullati)
                                        .OrderBy(a => a.dataInizioValidita)
                                        .ThenBy(a => a.dataFineValidita)
                                        .ToList();
                }
                return PartialView("CoeffIndRichiamo", libm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciCoeffIndRichiamo(CoefficienteRichiamoModel ibm, decimal idTipoCoeffRichiamo, bool escludiAnnullati = true, bool aggiornaTutto = false)
        {
            var libm = new List<CoefficienteRichiamoModel>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtParCoeffIndRichiamo dtpcir = new dtParCoeffIndRichiamo())
                    {
                        dtpcir.SetCoefficienteRichiamo(ibm, idTipoCoeffRichiamo, aggiornaTutto);

                        var r = new List<SelectListItem>();
                        List<TipoCoefficienteRichiamoModel> ltcrm = new List<TipoCoefficienteRichiamoModel>();

                        ViewBag.escludiAnnullati = escludiAnnullati;

                        ltcrm = dtpcir.ListTipoCoeffIndRichiamo().ToList();

                        if (ltcrm != null && ltcrm.Count > 0)
                        {
                            r = (from tcrm in ltcrm
                                 select new SelectListItem()
                                 {
                                     Text = tcrm.descrizione,
                                     Value = tcrm.idTipoCoefficienteRichiamo.ToString()
                                 }).ToList();
                        }

                        ViewBag.TipoCoeffRichiamoList = r;

                        ViewBag.idMinimoNonAnnullato = dtpcir.Get_Id_CoeffIndRichiamoPrimoNonAnnullato(idTipoCoeffRichiamo);
                        libm = dtpcir.GetListCoeffRichiamo_x_Tipo(idTipoCoeffRichiamo, escludiAnnullati)
                                                                .OrderBy(a => a.dataInizioValidita)
                                                                .ThenBy(a => a.dataFineValidita)
                                                                .ToList();

                        ViewBag.idTipoCoeffRichiamo = idTipoCoeffRichiamo;
                    }
                    return PartialView("CoeffIndRichiamo", libm);
                }
                else
                {
                    ViewBag.escludiAnnullati = escludiAnnullati;
                    using (dtParCoeffIndRichiamo dtpcir = new dtParCoeffIndRichiamo())
                    {
                        ViewBag.idMinimoNonAnnullato = dtpcir.Get_Id_CoeffIndRichiamoPrimoNonAnnullato(idTipoCoeffRichiamo);
                        var tcr = dtpcir.GetTipoCoeffRichiamo(idTipoCoeffRichiamo);
                        ViewBag.TipoCoeffRichiamo = tcr.DESCRIZIONE;
                        ViewBag.idTipoCoeffRichiamo = tcr.IDTIPOCOEFFICIENTERICHIAMO;
                    }

                    return PartialView("NuovoCoeffIndRichiamo", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
    }
}
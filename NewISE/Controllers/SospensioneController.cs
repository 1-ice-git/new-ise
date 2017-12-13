﻿using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.MappingViews;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NewISE.EF;
using NewISE.Models;
using NewISE.Models.Tools;

namespace NewISE.Controllers
{
    public class SospensioneController : Controller
    {
        // GET: Sospensione
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult VerificaSospensione(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            try
            {
                if (idTrasferimento <= 0)
                {
                    throw new Exception(" non valorizzato");
                }
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    dipInfoTrasferimentoModel trm = dtt.GetInfoTrasferimento(idTrasferimento);
                    if (trm != null && (trm.statoTrasferimento == EnumStatoTraferimento.Attivo ||
                        trm.statoTrasferimento == EnumStatoTraferimento.Da_Attivare))
                    {
                        ViewData["idTrasferimento"] = idTrasferimento;
                        return Json(new { VerificaSospensione = 1 });
                    }
                    else
                    {
                        return Json(new { VerificaSospensione = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult DeleteSospensione(decimal idSospensione)
        {
            decimal idTrasferimento = 0;
            ViewData["idSospensione"] = idSospensione;
            ViewBag.idSospensione = idSospensione;
            SospensioneModel tmp = new SospensioneModel();
            try
            {
                using (dtSospensione ds = new dtSospensione())
                {
                    tmp = ds.GetSospensionePerEliminazione(idSospensione);
                }
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tm = dtt.GetTrasferimentoByIdSosp(idSospensione);
                    idTrasferimento = tm.idTrasferimento;
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewData["idSospensione"] = idSospensione;
            ViewData.Add("idTrasferimento", idTrasferimento);
            // ViewBag.matricola = matricola;
            return PartialView(tmp);
        }
        public void Elimina_Sospensione(decimal idSospensione, bool permesso = true)
        {

            //decimal idSospensione =(decimal)ViewBag.idSospensione;
            SospensioneModel tmp = new SospensioneModel();
            using (dtSospensione ds = new dtSospensione())
            {
                ds.Delete_Sospensione(idSospensione, permesso);
            }
            // return PartialView("AttivitaSospensioni");
        }

        public ActionResult DatiTabElencoSospensione(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            List<SospensioneModel> tmp = new List<SospensioneModel>();
            try
            {
                using (dtSospensione dtcal = new dtSospensione())
                {
                    tmp.AddRange(dtcal.GetLista_Sospensioni(idTrasferimento));
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView(tmp);

        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult ElencoSospensioni(decimal idTrasferimento)
        {
            //  ViewData["idTrasferimento"] = idTrasferimento;
            ViewBag.idTrasferimento = idTrasferimento;
            try
            {
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult NuovaSospensione(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            List<SelectListItem> lTipologiaSospensione;
            try
            {
            lTipologiaSospensione = new List<SelectListItem>();
            var r = new List<SelectListItem>();
                using (dtSospensione dttc = new dtSospensione())
                {
                    var ltcm = dttc.GetListTipologiaSospensione();

                    if (ltcm != null && ltcm.Count > 0)
                    {
                        r = (from t in ltcm
                             select new SelectListItem()
                             {
                                 Text = t.Descrizione,
                                 Value = t.idTipologiaSospensione.ToString()
                             }).ToList();
                        //  r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                    }
                    lTipologiaSospensione = r;
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.lTipologiaSospensione = lTipologiaSospensione;
            SospensioneModel tmp = new SospensioneModel();
            tmp.idTrasferimento = (decimal)ViewData["idTrasferimento"];
            return PartialView(tmp);
        }

        public ActionResult AttivitaSospensione(decimal idTrasferimento)
        {
            try
            {
                 ViewData["idTrasferimento"] = idTrasferimento;
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("AttivitaSospensione");
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult InserisciSospensione(SospensioneModel sm, decimal idTrasferimento, decimal id_TipoSospensione)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            try
            {
                using (dtSospensione dtsosp = new dtSospensione())
                {
                    dtsosp.InserisciSospensione(sm, id_TipoSospensione);

                }
                //if (true)
                //{
                //    //ModelState.AddModelError("ErroreRangeDate", "Impossibile inserire una sopsensione con il periodo già presente su una sopsensione esistente.");
                //    ViewBag.ModelMsg = ModelloMessaggi;
                //}
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView("AttivitaSospensione");
        }
        public ActionResult EditSospensione(decimal idSospensione, decimal idTrasferimento)
        {
            ViewData["idSospensione"] = idSospensione;
            ViewData["idTrasferimento"] = idTrasferimento;
            SospensioneModel tmp = new SospensioneModel();
            using (dtSospensione dts = new dtSospensione())
            {
                tmp = dts.getSospensioneById(idSospensione);
            }
            List<SelectListItem> lTipologiaSospensione = new List<SelectListItem>();
            var r = new List<SelectListItem>();
            try
            {
                using (dtSospensione dttc = new dtSospensione())
                {
                    var ltcm = dttc.GetListTipologiaSospensione();

                    if (ltcm != null && ltcm.Count > 0)
                    {
                        r = (from t in ltcm
                             select new SelectListItem()
                             {
                                 Text = t.Descrizione,
                                 Value = t.idTipologiaSospensione.ToString()
                             }).ToList();
                        //  r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                    }
                    lTipologiaSospensione = r;
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.lTipologiaSospensione = lTipologiaSospensione;
            tmp.idTrasferimento = (decimal)ViewData["idTrasferimento"];
            return PartialView(tmp);
        }
        public ActionResult ModificaSospensione(SospensioneModel sm, decimal idSospensione, decimal id_TipoSospensione, decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            try
            {
                using (dtSospensione ds = new dtSospensione())
                {
                    //SospensioneModel sm = ds.getSospensioneById(idSospensione);
                    ds.Modifica_Sospensione(sm);
                }
                ViewBag.idTrasferimento = idTrasferimento;
                return PartialView("AttivitaSospensione");
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
    }
}
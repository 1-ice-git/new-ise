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
            try
            {
                if (idTrasferimento > 0)
                {
                    throw new Exception("Il trasferimento non risulta valorizzato.");
                }
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    TrasferimentoModel trm = dtt.GetSoloTrasferimentoById(idTrasferimento);

                    if (trm != null && trm.HasValue())
                    {
                        using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                        {
                            trm.RuoloUfficio = dtrd.GetRuoloDipendenteByIdTrasferimento(trm.idTrasferimento, DateTime.Now).RuoloUfficio;
                            trm.idRuoloUfficio = trm.RuoloUfficio.idRuoloUfficio;
                        }
                        using (dtDocumenti dtd = new dtDocumenti())
                        {
                            DocumentiModel dm = dtd.GetDocumentoByIdTrasferimento(trm.idTrasferimento);
                            trm.Documento = dm;
                            trm.idDocumento = dm.idDocumenti;
                        }

                        if (trm.idTipoTrasferimento > 0 &&
                            trm.idUfficio > 0 &&
                            trm.idStatoTrasferimento == (decimal)EnumStatoTraferimento.Attivo &&
                            trm.idDipendente > 0 &&
                            trm.idTipoCoan > 0 &&
                            trm.dataPartenza > DateTime.MinValue &&
                            trm.idRuoloUfficio > 0
                            //&&
                            //trm.protocolloLettera != string.Empty &&
                            //trm.dataLettera > DateTime.MinValue &&
                            //trm.idDocumento > 0
                            )
                        {
                            return Json(new { VerificaSospensione = 1 });
                        }
                        else
                        {
                            return Json(new { VerificaSospensione = 0 });
                        }
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
        public ActionResult ElencoSospensioni(decimal idTrasferimento)
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
        public ActionResult NuovaSospensione(decimal idTrasferimento)
        {
            List<SospensioneModel> tmp = new List<SospensioneModel>();
            try
            {
                using (dtSospensione dtcal = new dtSospensione())
                {
                    tmp.AddRange(dtcal.GetLista_Sospensioni(idTrasferimento));
                    ViewBag.idTrasferimento = idTrasferimento;
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView(tmp);
        }

        public ActionResult AttivitaSospensione(decimal idTrasferimento)
        {
            using (dtTrasferimento dtt = new dtTrasferimento())
            {
                var tr = dtt.GetSoloTrasferimentoById(idTrasferimento);

                using (dtDipendenti dtd = new dtDipendenti())
                {
                    var d = dtd.GetDipendenteByIDTrasf(idTrasferimento);
                    if (tr != null && tr.HasValue())
                    {
                        ViewBag.idTrasferimento = tr.idTrasferimento;
                    }
                    else
                    {
                        throw new Exception("Nessun trasferimento per la matricola (" + d.matricola + ")");
                    }
                }


            }

            return PartialView("AttivitaSospensione");
        }


    }
}
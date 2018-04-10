using NewISE.Models.DBModel;
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
using NewISE.Interfacce;
using System.Web.Helpers;
using MaggiorazioniFamiliariModel = NewISE.Models.DBModel.MaggiorazioniFamiliariModel;
using System.IO;

namespace NewISE.Controllers
{

    //public enum EnumCallElenco
    //{
    //    Coniuge = 1,
    //    Figli = 2,
    //    Formulari = 3
    //}

    public class ViaggiCongedoController : Controller
    {

        //[NonAction]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult VerificaViaggiCongedo(decimal idTrasferimento)
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
                    if (trm != null && (trm.statoTrasferimento == EnumStatoTraferimento.Attivo))
                    {
                        ViewData["idTrasferimento"] = idTrasferimento;
                        return Json(new { VerificaViaggiCongedo = 1 });
                    }
                    else
                    {
                        return Json(new { VerificaViaggiCongedo = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }

        public ActionResult ElencoPreventiviDiViaggio(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            List<ViaggioCongedoModel> lpv = new List<ViaggioCongedoModel>();
            try
            {
                using (dtViaggiCongedo dttv = new dtViaggiCongedo())
                {
                    lpv = dttv.GetUltimiPreventiviViaggio(idTrasferimento);
                    bool admin = Utility.Amministratore();
                    ViewBag.Amministratore = admin;
                    return PartialView(lpv);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        public ActionResult PreventiviDiViaggio(decimal idTrasferimento)
        {
            try
            {
                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    var idTitoliViaggio = dttv.GetIdTitoliViaggio(idTrasferimento);

                    ViewData.Add("idTitoliViaggio", idTitoliViaggio);
                    ViewData.Add("idTrasferimento", idTrasferimento);

                    return PartialView();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        public ActionResult ElencoUploadDocumentiViaggiCongedo(decimal idViaggioCongedo)
        {
            try
            {

                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    bool notificaEseguita = false;
                    bool richiediNotifica = false;
                    bool richiediAttivazione = false;
                    bool richiediConiuge = false;
                    bool richiediRichiedente = false;
                    bool richiediFigli = false;
                    bool DocTitoliViaggio = false;
                    bool DocCartaImbarco = false;
                    bool inLavorazione = false;
                    bool trasfAnnullato = false;

                    //var nDocCartaImbarco = dttv.GetNumDocumenti(idViaggioCongedo, EnumTipoDoc.Carta_Imbarco);
                    //var nDocTitoliViaggio = dttv.GetNumDocumenti(idViaggioCongedo, EnumTipoDoc.Titolo_Viaggio);

                    var nDocCartaImbarco = 0;// dttv.GetNumDocumenti(idViaggioCongedo, EnumTipoDoc.Carta_Imbarco);
                    var nDocTitoliViaggio = 0;// dttv.GetNumDocumenti(idViaggioCongedo, EnumTipoDoc.Titolo_Viaggio);

                    //var atv_notificata = dttv.GetUltimaAttivazioneNotificata(idTitoliViaggio);

                    //dttv.SituazioneTitoliViaggio(idViaggioCongedo,
                    //               out richiediNotifica, out richiediAttivazione,
                    //               out richiediConiuge, out richiediRichiedente,
                    //               out richiediFigli, out DocTitoliViaggio,
                    //               out DocCartaImbarco, out inLavorazione, out trasfAnnullato);

                    if (richiediAttivazione)
                    {
                        notificaEseguita = true;
                    }

                    ViewData.Add("notificaEseguita", notificaEseguita);
                    ViewData.Add("idViaggioCongedo", idViaggioCongedo);
                    ViewData.Add("nDocCartaImbarco", nDocCartaImbarco);
                    ViewData.Add("nDocTitoliViaggio", nDocTitoliViaggio);

                    return PartialView();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        public ActionResult ElencoUploadPreventiviViaggiCongedo(decimal idViaggioCongedo,decimal idTrasferimento)
        {
            ViewData["idViaggioCongedo"] = idViaggioCongedo;
            ViewData["idTrasferimento"] = idTrasferimento;
            return PartialView();
        }
        
        public ActionResult NuovoDocumentoPreventivi(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            try
            {
                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    var idTitoliViaggio = dttv.GetIdTitoliViaggio(idTrasferimento);

                  //  ViewData.Add("idTitoliViaggio", idTitoliViaggio);
                  //  ViewData.Add("idTrasferimento", idTrasferimento);

                    return PartialView();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        //
        public ActionResult SalvaPreventivi(decimal idTrasferimento)
        {
            List<SelectDocVc> lDocVC = new List<SelectDocVc>();
            SelectDocVc DocVC = new SelectDocVc();
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    //throw new Exception("Simulazione errore");
                    using (dtViaggiCongedo dtvc = new dtViaggiCongedo())
                    {
                        foreach (string item in Request.Files)
                        {
                            HttpPostedFileBase file = Request.Files[item] as HttpPostedFileBase;


                            DocumentiModel dm = new DocumentiModel();
                            bool esisteFile = false;
                            bool gestisceEstensioni = false;
                            bool dimensioneConsentita = false;
                            string dimensioneMaxConsentita = string.Empty;

                            Utility.PreSetDocumento(file, out dm, out esisteFile, out gestisceEstensioni,
                                out dimensioneConsentita, out dimensioneMaxConsentita, EnumTipoDoc.Preventivo_Viaggio);

                            if (esisteFile)
                            {
                                if (gestisceEstensioni == false)
                                {
                                    throw new Exception(
                                        "Il documento selezionato non è nel formato consentito. Il formato supportato è: pdf.");
                                }

                                if (dimensioneConsentita)
                                {
                                    dtvc.SetPreventiviViaggiCongedio(ref dm, db, idTrasferimento);
                                    DocVC = new SelectDocVc();DocVC.idDocumento = dm.idDocumenti;DocVC.DocSelezionato = false;
                                    lDocVC.Add(DocVC);
                                }
                                else
                                {
                                    throw new Exception(
                                        "Il documento selezionato supera la dimensione massima consentita (" +
                                        dimensioneMaxConsentita + " Mb).");
                                }
                            }
                            else
                            {
                                throw new Exception("Il documento è obbligatorio.");
                            }

                        }
                        //AGGIORNAMENTO TABELLE
                        dtvc.AggiornaTabelleCorrellate(idTrasferimento, lDocVC, db);                        
                    }
                    db.Database.CurrentTransaction.Commit();
                    return Json(new { });
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    return Json(new { error = ex.Message });
                };
            }
        }
        public ActionResult LeggiViaggiCongedoPDF(decimal id)
        {
            byte[] Blob;
            DocumentiModel documento = new DocumentiModel();
            using (dtDocumenti dtd = new dtDocumenti())
            {
                documento = dtd.GetDatiDocumentoById(id);
                Blob = dtd.GetDocumentoByteById(id);

                Response.AddHeader("Content-Disposition", "inline; filename=" + documento.idDocumenti + documento.estensione.ToLower() + ";");

                switch (documento.estensione.ToLower())
                {
                    case ".pdf":
                        return File(Blob, "application/pdf");
                    default:
                        return File(Blob, "application/pdf");
                }
            }
        }
    }
}
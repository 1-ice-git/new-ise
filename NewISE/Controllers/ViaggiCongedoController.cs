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
using NewISE.Interfacce.Modelli;
using NewISE.Models.DBModel.Enum;

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
           // List<ViaggioCongedoModel> lpv = new List<ViaggioCongedoModel>();
            try
            {
                bool admin = Utility.Amministratore();
                ViewBag.Amministratore = admin;

                using (dtViaggiCongedo dvc = new dtViaggiCongedo())
                {
                    decimal idFaseInCorso = 0;
                    decimal id_Viaggio_Congedo = dvc.Identifica_Id_UltimoViaggioCongedoDisponibile(idTrasferimento);
                    List<AttivazioniViaggiCongedoModel> lAttiv_Viaggio_Congedo = new List<AttivazioniViaggiCongedoModel>();
                    if (id_Viaggio_Congedo == 0)
                    {
                        id_Viaggio_Congedo = dvc.Crea_Viaggi_Congedo(idTrasferimento);
                        idFaseInCorso = (decimal)EnumFaseViaggioCongedo.Preventivi;
                    }
                    else
                    {
                        idFaseInCorso = dvc.Restituisci_LivelloFase_Da_ATT_Viagg_CONG(id_Viaggio_Congedo);
                    }
                    decimal id_Attiv_Viaggio_Congedo = 0;
                    lAttiv_Viaggio_Congedo = dvc.Cerca_Id_AttivazioniViaggiCongedoDisponibile(id_Viaggio_Congedo, idFaseInCorso);
                    decimal id_Attiva_Viaggio_Congedo = 0;
                    if (lAttiv_Viaggio_Congedo.Count == 0)
                    {
                        id_Attiva_Viaggio_Congedo = dvc.Crea_Attivazioni_Viaggi_Congedo(id_Viaggio_Congedo, idFaseInCorso);
                    }
                    else
                        id_Attiv_Viaggio_Congedo = lAttiv_Viaggio_Congedo.First().idAttivazioneVC;

                    ViewData["id_Viaggio_Congedo"] = id_Viaggio_Congedo;
                    ViewData["id_Attiv_Viaggio_Congedo"] = id_Attiv_Viaggio_Congedo;

                    //lpv = dvc.GetUltimiPreventiviViaggio(id_Attiv_Viaggio_Congedo);
                    var lpv = dvc.GetUltimiPreventiviViaggio(id_Attiv_Viaggio_Congedo);

                    bool AttivazioneInviata = dvc.AttivazionePreventiviInviata(id_Attiv_Viaggio_Congedo, idFaseInCorso);
                    ViewData["AttivazioneInviata"] = AttivazioneInviata;

                    bool NotificaInviata = dvc.NotificaPreventiviInviata(id_Attiv_Viaggio_Congedo, idFaseInCorso);
                    ViewData["NotificaInviata"] = NotificaInviata;

                    ViewData["idFaseInCorso"] = idFaseInCorso;

                }

                return PartialView();
              
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        public ActionResult ListaPreventiviDiViaggio(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            List<ViaggioCongedoModel> lpv = new List<ViaggioCongedoModel>();
            List<ViaggioCongedoModel> lpvFinale = new List<ViaggioCongedoModel>();
            try
            {
                using (dtViaggiCongedo dvc = new dtViaggiCongedo())
                {
                    decimal idFaseInCorso = 0;
                    decimal id_Viaggio_Congedo = dvc.Identifica_Id_UltimoViaggioCongedoDisponibile(idTrasferimento);                    
                    List<AttivazioniViaggiCongedoModel> lAttiv_Viaggio_Congedo = new List<AttivazioniViaggiCongedoModel>();
                    if (id_Viaggio_Congedo == 0)
                    {
                        id_Viaggio_Congedo = dvc.Crea_Viaggi_Congedo(idTrasferimento);
                        idFaseInCorso = (decimal)EnumFaseViaggioCongedo.Preventivi;
                    }
                    else
                    {
                        idFaseInCorso = dvc.Restituisci_LivelloFase_Da_ATT_Viagg_CONG(id_Viaggio_Congedo);
                    }
                    decimal id_Attiv_Viaggio_Congedo = 0;
                    lAttiv_Viaggio_Congedo = dvc.Cerca_Id_AttivazioniViaggiCongedoDisponibile(id_Viaggio_Congedo, idFaseInCorso);
                    decimal id_Attiva_Viaggio_Congedo = 0;
                    if (lAttiv_Viaggio_Congedo.Count == 0)
                    {
                        id_Attiva_Viaggio_Congedo = dvc.Crea_Attivazioni_Viaggi_Congedo(id_Viaggio_Congedo, idFaseInCorso);
                    }
                    else
                        id_Attiv_Viaggio_Congedo = lAttiv_Viaggio_Congedo.First().idAttivazioneVC;

                    ViewData["id_Viaggio_Congedo"] = id_Viaggio_Congedo;
                    ViewData["id_Attiv_Viaggio_Congedo"] = id_Attiv_Viaggio_Congedo;

                    if (idFaseInCorso == (decimal)EnumFaseViaggioCongedo.Documenti_di_Viaggio)
                    {
                        lpv = dvc.GetPrecedentiPreventiviViaggio(id_Attiv_Viaggio_Congedo);
                        lpvFinale.AddRange(lpv);
                    }
                    lpv = dvc.GetUltimiPreventiviViaggio(id_Attiv_Viaggio_Congedo);
                    lpvFinale.AddRange(lpv);
                    bool admin = Utility.Amministratore();
                    ViewBag.Amministratore = admin;

                    bool AttivazioneInviata = dvc.AttivazionePreventiviInviata(id_Attiv_Viaggio_Congedo, idFaseInCorso);
                    ViewData["AttivazioneInviata"] = AttivazioneInviata;

                    bool NotificaInviata = dvc.NotificaPreventiviInviata(id_Attiv_Viaggio_Congedo, idFaseInCorso);
                    ViewData["NotificaInviata"] = NotificaInviata;
                    
                    ViewData["idFaseInCorso"] = idFaseInCorso;

                    return PartialView(lpvFinale);
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

        public ActionResult ElencoUploadDocumentiViaggiCongedo(decimal idViaggioCongedo ,decimal  idAttivViaggioCongedo,decimal idFaseInCorso,decimal idTrasferimento)
        {
            try
            {
                    //bool notificaEseguita = false;
                    //bool richiediNotifica = false;
                    //bool richiediAttivazione = false;
                    //bool richiediConiuge = false;
                    //bool richiediRichiedente = false;
                    //bool richiediFigli = false;
                    //bool DocTitoliViaggio = false;
                    //bool DocCartaImbarco = false;
                    //bool inLavorazione = false;
                    //bool trasfAnnullato = false;

                    //var nDocCartaImbarco = dttv.GetNumDocumenti(idViaggioCongedo, EnumTipoDoc.Carta_Imbarco);
                    //var nDocTitoliViaggio = dttv.GetNumDocumenti(idViaggioCongedo, EnumTipoDoc.Titolo_Viaggio);

                  //  var nDocCartaImbarco = 0;// dttv.GetNumDocumenti(idViaggioCongedo, EnumTipoDoc.Carta_Imbarco);
                  //  var nDocTitoliViaggio = 0;// dttv.GetNumDocumenti(idViaggioCongedo, EnumTipoDoc.Titolo_Viaggio);

                    //var atv_notificata = dttv.GetUltimaAttivazioneNotificata(idTitoliViaggio);

                    //dttv.SituazioneTitoliViaggio(idViaggioCongedo,
                    //               out richiediNotifica, out richiediAttivazione,
                    //               out richiediConiuge, out richiediRichiedente,
                    //               out richiediFigli, out DocTitoliViaggio,
                    //               out DocCartaImbarco, out inLavorazione, out trasfAnnullato);

                    //if (richiediAttivazione)
                    //{
                    //    notificaEseguita = true;
                    //}

                    //ViewData.Add("notificaEseguita", notificaEseguita);
                    //ViewData.Add("idViaggioCongedo", idViaggioCongedo);
                    //ViewData.Add("nDocCartaImbarco", nDocCartaImbarco);
                    //ViewData.Add("nDocTitoliViaggio", nDocTitoliViaggio);

                    ViewData["id_Viaggio_Congedo"] = idViaggioCongedo;
                    ViewData["id_Attiv_Viaggio_Congedo"] = idAttivViaggioCongedo;
                    ViewData["idTrasferimento"] = idTrasferimento;
                    using (dtViaggiCongedo dvc = new dtViaggiCongedo())
                    {
                        bool NotificaInviata = dvc.NotificaPreventiviInviata(idAttivViaggioCongedo, idFaseInCorso);
                        ViewData["NotificaInviata"] = NotificaInviata;
                        bool AttivazioneInviata = dvc.AttivazionePreventiviInviata(idAttivViaggioCongedo, idFaseInCorso);
                        ViewData["AttivazioneInviata"] = AttivazioneInviata;
                        ViewData["idFaseInCorso"] = idFaseInCorso;

                        var nDocCartaImbarco = dvc.GetNumDocumenti(idAttivViaggioCongedo, EnumTipoDoc.Carta_Imbarco);
                        var nDocTitoliViaggio = dvc.GetNumDocumenti(idAttivViaggioCongedo, EnumTipoDoc.Titolo_Viaggio);
                        ViewData.Add("nDocCartaImbarco", nDocCartaImbarco);
                        ViewData.Add("nDocTitoliViaggio", nDocTitoliViaggio);
                }
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        public ActionResult ElencoUploadPreventiviViaggiCongedo(decimal idViaggioCongedo,decimal idAttivViaggioCongedo, decimal idTrasferimento,decimal idFaseInCorso)
        {
            ViewData["id_Viaggio_Congedo"] = idViaggioCongedo;
            ViewData["id_Attiv_Viaggio_Congedo"] = idAttivViaggioCongedo;
            ViewData["idTrasferimento"] = idTrasferimento;
            using (dtViaggiCongedo dvc =new dtViaggiCongedo())
            {
                bool NotificaInviata = dvc.NotificaPreventiviInviata(idAttivViaggioCongedo, idFaseInCorso);
                ViewData["NotificaInviata"] = NotificaInviata;
                bool AttivazioneInviata = dvc.AttivazionePreventiviInviata(idAttivViaggioCongedo, idFaseInCorso);
                ViewData["AttivazioneInviata"] = AttivazioneInviata;
                ViewData["idFaseInCorso"] = idFaseInCorso;
            }
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
            decimal id_Viaggio_Congedo=0;
            decimal id_Attiv_Viaggio_Congedo = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    //throw new Exception("Simulazione errore");
                    using (dtViaggiCongedo dtvc = new dtViaggiCongedo())
                    {
                        //AGGIORNAMENTO TABELLE
                        id_Viaggio_Congedo = dtvc.Identifica_Id_UltimoViaggioCongedoDisponibile(idTrasferimento);
                      //  var Attiv_Viaggio_Congedo = dtvc.Cerca_Id_AttivazioniViaggiCongedoDisponibile(id_Viaggio_Congedo);
                        //if (Attiv_Viaggio_Congedo?.Any()??true)
                        if (id_Viaggio_Congedo == 0)
                        {
                            id_Viaggio_Congedo= dtvc.Crea_Viaggi_Congedo(idTrasferimento);
                          //  id_Attiv_Viaggio_Congedo = dtvc.Crea_Attivazioni_Viaggi_Congedo(id_Viaggio_Congedo);
                        }
                      
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
                        foreach (var x in lDocVC)
                        {
                            decimal idFaseInCorso = 0;
                            id_Attiv_Viaggio_Congedo = x.idAttivazioneVC;
                            if(id_Attiv_Viaggio_Congedo==0)
                            {
                                id_Attiv_Viaggio_Congedo = dtvc.Crea_Attivazioni_Viaggi_Congedo(id_Viaggio_Congedo,idFaseInCorso);
                            }
                            dtvc.AggiornaTabellaCorrellata(id_Attiv_Viaggio_Congedo, lDocVC, db);
                        }

                    }
                 //   db.Database.CurrentTransaction.Commit();
                    return Json(new { });
                }
                catch (Exception ex)
                {
                    //db.Database.CurrentTransaction.Rollback();
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
        public ActionResult PulsantiPreventiviDiViaggio(decimal idViaggioCongedo , decimal idAttivViaggioCongedo , decimal idTrasferimento)
        {
            bool admin = Utility.Amministratore();
            ViewBag.Amministratore = admin;
            List<AttivazioniViaggiCongedoModel> lAttiv_Viaggio_Congedo = new List<AttivazioniViaggiCongedoModel>();
            using (dtViaggiCongedo dtvc = new dtViaggiCongedo())
            {
                decimal id_Viaggio_Congedo = dtvc.Identifica_Id_UltimoViaggioCongedoDisponibile(idTrasferimento);
                decimal idFaseInCorso = dtvc.Restituisci_LivelloFase_Da_ATT_Viagg_CONG(id_Viaggio_Congedo);
                lAttiv_Viaggio_Congedo = dtvc.Cerca_Id_AttivazioniViaggiCongedoDisponibile(id_Viaggio_Congedo,idFaseInCorso);
                idAttivViaggioCongedo = lAttiv_Viaggio_Congedo.First().idAttivazioneVC;
                bool NotificaInviata = dtvc.NotificaPreventiviInviata(idAttivViaggioCongedo, idFaseInCorso);
                ViewData["id_Attiv_Viaggio_Congedo"] = idAttivViaggioCongedo;
                ViewData["NotificaInviata"] = NotificaInviata;
                ViewData["ConteggioElementi"] = dtvc.ConteggiaPreventiviRichiesta(idAttivViaggioCongedo);
                bool AttivazioneInviata = dtvc.AttivazionePreventiviInviata(idAttivViaggioCongedo, idFaseInCorso);
                ViewData["AttivazioneInviata"] = AttivazioneInviata;
            }
            return PartialView();
        }
        //GestionePulsantiNotificaAttivaAnnulla
        public ActionResult GestionePulsantiNotificaAttivaAnnulla(decimal idTrasferimento)
        {
            return PartialView();
        }

        public ActionResult ConfermaAttivaPreventiviRichiesta(decimal idAttivazioneVC,decimal idDocumento)
        {
            try
            {
                using (dtViaggiCongedo dtvc = new dtViaggiCongedo())
                {
                    decimal tmp= dtvc.AttivaPreventiviRichiesta(idAttivazioneVC, idDocumento);
                    bool admin = Utility.Amministratore();
                    ViewBag.Amministratore = admin;
                    decimal idFaseInCorso = dtvc.Restituisci_LivelloFase_Da_ATT_Viagg_CONG(dtvc.Restituisci_ID_Viagg_CONG_DA(idAttivazioneVC));
                    bool NotificaInviata = dtvc.NotificaPreventiviInviata(idAttivazioneVC, idFaseInCorso);
                    ViewData["id_Attiv_Viaggio_Congedo"] = tmp;// idAttivazioneVC; il nuova ma non il vecchio
                    ViewData["NotificaInviata"] = NotificaInviata;
                    ViewData["ConteggioElementi"] = dtvc.ConteggiaPreventiviRichiesta(idAttivazioneVC);
                    ViewData["id_Viaggio_Congedo"] = 0;
                    //Inviare la mail
                }
                //return PartialView("PulsantiPreventiviDiViaggio");
                return Json(new { err = "" });
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }
        public ActionResult ConfermaAnnullaPreventiviRichiesta(decimal idAttivazioneVC)
        {
            try
            {
                using (dtViaggiCongedo dtvc = new dtViaggiCongedo())
                {

                }
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        public JsonResult ConfermaNotificaPreventiviRichiesta(decimal idAttivazioneVC)
        {
            try
            {
                using (dtViaggiCongedo dtvc = new dtViaggiCongedo())
                {
                    dtvc.NotificaPreventiviRichiesta(idAttivazioneVC);
                
                bool admin = Utility.Amministratore();
                ViewBag.Amministratore = admin;

                    decimal idFaseInCorso = dtvc.Restituisci_LivelloFase_Da_ATT_Viagg_CONG(dtvc.Restituisci_ID_Viagg_CONG_DA(idAttivazioneVC));
                    bool NotificaInviata = dtvc.NotificaPreventiviInviata(idAttivazioneVC, idFaseInCorso);
                    ViewData["id_Attiv_Viaggio_Congedo"] = idAttivazioneVC;
                    ViewData["NotificaInviata"] = NotificaInviata;
                    ViewData["ConteggioElementi"] = dtvc.ConteggiaPreventiviRichiesta(idAttivazioneVC);
                    ViewData["id_Viaggio_Congedo"] = 0;
                    //Inviare la mail
                }
                //return PartialView("PulsantiPreventiviDiViaggio");
                return Json(new { err = "" });
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }
       

      //  public ActionResult ElencoDocumentiVC(decimal idTipoDocumento, decimal idAttivazioneVC,decimal idTrasferimento)
        public ActionResult ElencoDocumentiVC( decimal idTrasferimento, decimal idTipoDocumento, decimal idAttivViaggioCongedo)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            try
            {
                using (dtViaggiCongedo dtvc = new dtViaggiCongedo())
                {
                    bool admin = Utility.Amministratore();
                    ViewBag.Amministratore = admin;
                    decimal idFaseInCorso = dtvc.Restituisci_LivelloFase_Da_ATT_Viagg_CONG(dtvc.Restituisci_ID_Viagg_CONG_DA(idAttivViaggioCongedo));
                    bool NotificaInviata = dtvc.NotificaPreventiviInviata(idAttivViaggioCongedo, idFaseInCorso);
                    ViewData["id_Attiv_Viaggio_Congedo"] = idAttivViaggioCongedo;// idAttivazioneVC; il nuova ma non il vecchio
                    ViewData["NotificaInviata"] = NotificaInviata;
                    ViewData["ConteggioElementi"] = dtvc.ConteggiaPreventiviRichiesta(idAttivViaggioCongedo);
                    ViewData["id_Viaggio_Congedo"] = dtvc.Restituisci_ID_Viagg_CONG_DA(idAttivViaggioCongedo);
                    ViewData["idFaseInCorso"] = idFaseInCorso;
                    ViewData["idTipoDocumento"] = idTipoDocumento;
                    //Inviare la mail
                }
            
                string DescrizioneTV = "";
                using (dtDocumenti dtd = new dtDocumenti())
                {
               //     DescrizioneTV = dtd.GetDescrizioneTipoDocumentoByIdTipoDocumento(idTipoDocumento);
                }
                //  bool richiestaNotificata = false;
                List<SelectListItem> lDataAttivazione = new List<SelectListItem>();
                List<ViaggioCongedoModel> latvm = new List<ViaggioCongedoModel>();
                using (dtViaggiCongedo dttv = new dtViaggiCongedo())
                {
                   // latvm = dttv.GetListDocumentiViaggioCongedoByTipoDoc(idAttivazioneVC, idTipoDocumento).OrderBy(a=>a.idDocumento).ToList();
                    //var i = latvm.Count();
                    //var i = 1;

                    //foreach (var atv in latvm)
                    //{
                    //    if (dttv.AttivazioneNotificata(atv.idAttivazioneTitoliViaggio))
                    //    {
                    //        richiestaNotificata = true;
                    //    }

                    //    bool inLavorazione = dttv.AttivazioneTitoliViaggioInLavorazione(atv.idAttivazioneTitoliViaggio, idAttivazioneVC);

                    //    if (inLavorazione)
                    //    {
                    //        lDataAttivazione.Insert(0, new SelectListItem() { Text = "(" + i.ToString() + ") " + atv.dataAggiornamento.ToString() + " (In Lavorazione)", Value = atv.idAttivazioneTitoliViaggio.ToString() });
                    //    }
                    //    else
                    //    {
                    //        lDataAttivazione.Insert(0, new SelectListItem() { Text = "(" + i.ToString() + ") " + atv.dataAggiornamento.ToString(), Value = atv.idAttivazioneTitoliViaggio.ToString() });
                    //    }
                    //    //i--;
                    //    i++;
                    //}

                    //lDataAttivazione.Insert(0, new SelectListItem() { Text = "(TUTTE)", Value = "" });
                    //ViewData.Add("lDataAttivazione", lDataAttivazione);

                    //using (dtTrasferimento dtt = new dtTrasferimento())
                    //{
                    //    var t = dtt.GetTrasferimentoByIdTitoloViaggio(idAttivazioneVC);
                    //    EnumStatoTraferimento statoTrasferimento = (EnumStatoTraferimento)t.idStatoTrasferimento;
                    //    ViewData.Add("statoTrasferimento", statoTrasferimento);
                    //}

                    //ViewData.Add("DescrizioneTV", DescrizioneTV);
                    //ViewData.Add("idTipoDocumento", idTipoDocumento);
                    //ViewData.Add("idAttivazioneVC", idAttivazioneVC);
                    //ViewData.Add("richiestaNotificata", richiestaNotificata);
                }

                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        //
        public ActionResult PulsanteNuoviDocViaggiCongedo2Fase(decimal idViaggioCongedo, decimal idAttivViaggioCongedo, decimal idTrasferimento, decimal  idFaseInCorso,decimal idTipoDocumento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            ViewData["tipoDoc"] = idTipoDocumento;
            //bool NotificaInviata = Convert.ToBoolean(ViewData["NotificaInviata"]);
            //bool AttivazioneInviata = Convert.ToBoolean(ViewData["AttivazioneInviata"]);
            ViewData["idFaseInCorso"] = idFaseInCorso;
            ViewData["idViaggioCongedo"] = idViaggioCongedo;
            ViewData["idAttivViaggioCongedo"] = idAttivViaggioCongedo;
            try
            {
                using (dtViaggiCongedo dtvc = new dtViaggiCongedo())
                {
                    bool NotificaInviata = dtvc.NotificaPreventiviInviata(idAttivViaggioCongedo, idFaseInCorso);
                    ViewData["id_Attiv_Viaggio_Congedo"] = idAttivViaggioCongedo;// idAttivazioneVC; il nuova ma non il vecchio
                    ViewData["NotificaInviata"] = NotificaInviata;
                }
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        public ActionResult ListaDocViaggiCongedio2Fase(decimal idViaggioCongedo, decimal idAttivViaggioCongedo, decimal idTrasferimento, decimal idFaseInCorso, decimal idTipoDocumento)
        {
            try
            {
                using (dtViaggiCongedo dtvc = new dtViaggiCongedo())
                {

                }
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
    }

}
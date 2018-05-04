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
                    bool nuovoVC = dvc.DeterminaSeNuovo(id_Viaggio_Congedo);
                    //if (nuovoVC == true) id_Viaggio_Congedo = 0;
                    List<AttivazioniViaggiCongedoModel> lAttiv_Viaggio_Congedo = new List<AttivazioniViaggiCongedoModel>();
                    if (id_Viaggio_Congedo == 0)
                    {
                        id_Viaggio_Congedo = dvc.Crea_Viaggi_Congedo(idTrasferimento);                        
                    }
                    else
                    {
                        idFaseInCorso = dvc.Restituisci_LivelloFase_Da_ATT_Viagg_CONG(id_Viaggio_Congedo);
                    }
                    // if (idFaseInCorso == (decimal)EnumFaseViaggioCongedo.Documenti_di_Viaggio)
                    ViewData["NuovoViaggiCongedo"] = nuovoVC;
                    idFaseInCorso = (decimal)EnumFaseViaggioCongedo.Preventivi;
                    decimal id_Attiv_Viaggio_Congedo = 0;
                    lAttiv_Viaggio_Congedo = dvc.Cerca_Id_AttivazioniViaggiCongedoDisponibile(id_Viaggio_Congedo, idFaseInCorso);
                    
                    if (lAttiv_Viaggio_Congedo.Count == 0)
                    {
                        id_Attiv_Viaggio_Congedo = dvc.Crea_Attivazioni_Viaggi_Congedo(id_Viaggio_Congedo, idFaseInCorso,idTrasferimento);
                    }
                    else
                        id_Attiv_Viaggio_Congedo = lAttiv_Viaggio_Congedo.First().idAttivazioneVC;

                    ViewData["id_Viaggio_Congedo"] = id_Viaggio_Congedo;
                    ViewData["id_Attiv_Viaggio_Congedo"] = id_Attiv_Viaggio_Congedo;

                    //lpv = dvc.GetUltimiPreventiviViaggio(id_Attiv_Viaggio_Congedo);
                    var lpv = dvc.GetUltimiPreventiviViaggio(id_Attiv_Viaggio_Congedo,idTrasferimento, id_Viaggio_Congedo);

                    //bool AttivazioneInviata = dvc.AttivazionePreventiviInviata(id_Attiv_Viaggio_Congedo, idFaseInCorso, idTrasferimento);
                    //ViewData["AttivazioneInviata"] = AttivazioneInviata;

                    //bool NotificaInviata = dvc.NotificaPreventiviInviata(id_Attiv_Viaggio_Congedo, idFaseInCorso);
                    //ViewData["NotificaInviata"] = NotificaInviata;
                   

                    ViewData["idFaseInCorso"] = idFaseInCorso;
                    var nDocCartaImbarco = dvc.GetNumDocumenti(id_Attiv_Viaggio_Congedo, EnumTipoDoc.Carta_Imbarco);
                    var nDocTitoliViaggio = dvc.GetNumDocumenti(id_Attiv_Viaggio_Congedo, EnumTipoDoc.Titolo_Viaggio);
                    ViewData.Add("nDocCartaImbarco", nDocCartaImbarco);
                    ViewData.Add("nDocTitoliViaggio", nDocTitoliViaggio);
                    AggiornaTuttiViewData(idTrasferimento);
                }

                return PartialView();
              
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        public ActionResult ListaPreventiviDiViaggio(decimal idTrasferimento, bool nuovVC)
        {
            ViewData["NuovoViaggiCongedo"] = nuovVC;
            ViewData["idTrasferimento"] = idTrasferimento;
            List<ViaggioCongedoModel> lpv = new List<ViaggioCongedoModel>();
            List<ViaggioCongedoModel> lpvFinale = new List<ViaggioCongedoModel>();
            try
            {
                using (dtViaggiCongedo dvc = new dtViaggiCongedo())
                {
                    decimal idFaseInCorso = dvc.Restituisci_IdAttivazioniVC_IdFaseInCorso_Attuale(idTrasferimento)[1];// (decimal)EnumFaseViaggioCongedo.Preventivi;
                    decimal id_Viaggio_Congedo = dvc.Identifica_Id_UltimoViaggioCongedoDisponibile(idTrasferimento);                    
                    List<AttivazioniViaggiCongedoModel> lAttiv_Viaggio_Congedo = new List<AttivazioniViaggiCongedoModel>();
                    if (id_Viaggio_Congedo == 0)
                    {
                        id_Viaggio_Congedo = dvc.Crea_Viaggi_Congedo(idTrasferimento);
                    }
                    decimal id_Attiv_Viaggio_Congedo = dvc.Restituisci_IdAttivazioniVC_IdFaseInCorso_Attuale(idTrasferimento)[0];
                    //lAttiv_Viaggio_Congedo = dvc.Cerca_Id_AttivazioniViaggiCongedoDisponibile(id_Viaggio_Congedo, idFaseInCorso);
                    if(id_Attiv_Viaggio_Congedo==0)
                        id_Attiv_Viaggio_Congedo = dvc.Crea_Attivazioni_Viaggi_Congedo(id_Viaggio_Congedo, idFaseInCorso, idTrasferimento);
                    //if (lAttiv_Viaggio_Congedo.Count() == 0)
                    //{
                    //    id_Attiv_Viaggio_Congedo = dvc.Crea_Attivazioni_Viaggi_Congedo(id_Viaggio_Congedo, idFaseInCorso, idTrasferimento);
                    //}
                    //else
                    //    id_Attiv_Viaggio_Congedo = lAttiv_Viaggio_Congedo.First().idAttivazioneVC;

                    ViewData["id_Viaggio_Congedo"] = id_Viaggio_Congedo;
                    ViewData["id_Attiv_Viaggio_Congedo"] = id_Attiv_Viaggio_Congedo;

                    //if (idFaseInCorso == (decimal)EnumFaseViaggioCongedo.Documenti_di_Viaggio)
                    //{
                    //    lpv = dvc.GetPrecedentiPreventiviViaggio(id_Attiv_Viaggio_Congedo);
                    //    lpvFinale.AddRange(lpv);
                    //}
                    lpv = dvc.GetUltimiPreventiviViaggio(id_Attiv_Viaggio_Congedo,idTrasferimento, id_Viaggio_Congedo);
                    lpvFinale.AddRange(lpv);
                    bool admin = Utility.Amministratore();
                    ViewBag.Amministratore = admin;

                    //bool AttivazioneInviata = dvc.AttivazionePreventiviInviata(id_Attiv_Viaggio_Congedo, idFaseInCorso, idTrasferimento);
                    //ViewData["AttivazioneInviata"] = AttivazioneInviata;

                    //bool NotificaInviata = dvc.NotificaPreventiviInviata(id_Attiv_Viaggio_Congedo, idFaseInCorso);
                    //ViewData["NotificaInviata"] = NotificaInviata;
                    
                    //ViewData["idFaseInCorso"] = idFaseInCorso;

                    AggiornaTuttiViewData(idTrasferimento);

                    return PartialView(lpvFinale);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        
        public ActionResult ElencoUploadDocumentiViaggiCongedo(decimal idViaggioCongedo ,decimal  idAttivViaggioCongedo,decimal idFaseInCorso,decimal idTrasferimento, bool nuovVC)
        {
            try
            {
                ViewData["NuovoViaggiCongedo"] = nuovVC;
                ViewData["id_Viaggio_Congedo"] = idViaggioCongedo;
                //if (idFaseInCorso == (decimal)EnumFaseViaggioCongedo.Documenti_di_Viaggio)
                //    idFaseInCorso = (decimal)EnumFaseViaggioCongedo.Preventivi;

                AggiornaTuttiViewData(idTrasferimento);

                using (dtViaggiCongedo dvc = new dtViaggiCongedo())
                {
                        //idAttivViaggioCongedo = dvc.Crea_Attivazioni_Viaggi_Congedo(idViaggioCongedo, idFaseInCorso);
                        //ViewData["id_Attiv_Viaggio_Congedo"] = idAttivViaggioCongedo;
                        //bool NotificaInviata = dvc.NotificaPreventiviInviata(idAttivViaggioCongedo, idFaseInCorso);
                        //ViewData["NotificaInviata"] = NotificaInviata;
                        //bool AttivazioneInviata = dvc.AttivazionePreventiviInviata(idAttivViaggioCongedo, idFaseInCorso);
                        //ViewData["AttivazioneInviata"] = AttivazioneInviata;
                        //ViewData["idFaseInCorso"] = idFaseInCorso;
                        decimal idAttivViaggioCongedo2 = dvc.Crea_Attivazioni_Viaggi_Congedo(idViaggioCongedo, (decimal)EnumFaseViaggioCongedo.Documenti_di_Viaggio, idTrasferimento);
                        var nDocCartaImbarco = dvc.GetNumDocumenti(idAttivViaggioCongedo2, EnumTipoDoc.Carta_Imbarco);
                        var nDocTitoliViaggio = dvc.GetNumDocumenti(idAttivViaggioCongedo2, EnumTipoDoc.Titolo_Viaggio);
                        ViewData.Add("nDocCartaImbarco", nDocCartaImbarco);
                        ViewData.Add("nDocTitoliViaggio", nDocTitoliViaggio);
                        AggiornaTuttiViewData(idTrasferimento);
                }
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        public ActionResult ElencoUploadPreventiviViaggiCongedo(decimal idViaggioCongedo, decimal idAttivViaggioCongedo, decimal idTrasferimento, decimal idFaseInCorso, bool nuovVC)
        {
            try
            {
                ViewData["NuovoViaggiCongedo"] = nuovVC;
                ViewData["id_Viaggio_Congedo"] = idViaggioCongedo;

                ViewData["id_Attiv_Viaggio_Congedo"] = idAttivViaggioCongedo;
                ViewData["idTrasferimento"] = idTrasferimento;
                ViewData["NuovoViaggiCongedo"] = nuovVC;

                AggiornaTuttiViewData(idTrasferimento);

                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
    public void AggiornaTuttiViewData(decimal idTrasferimento)
    {
            using (dtViaggiCongedo dvc = new dtViaggiCongedo())
            {
                decimal[] tmp = dvc.Restituisci_IdAttivazioniVC_IdFaseInCorso_Attuale(idTrasferimento);
                //decimal[] tmp = new decimal[] { 0, 0 ,0,0};
                //idAttivazioneVC , idFaseInCorso, NOTIFCATA, ATTIVATA
                //       [0]             [1]           [2]       [3]
                if (tmp[0] == 0 && tmp[1] == 0 && tmp[1] == 0 && tmp[3] == 0)
                    tmp= dvc.Restituisci_IdAttivazioniVC_IdFaseInCorso_Attuale(idTrasferimento);

                decimal idFaseInCorso = tmp[1];

                bool AttivaPulsanteNuovo = false;
                bool NotificaInviata2 = false;
                bool NotificaInviata = false;
                bool AttivazioneInviata = false;
                bool AttivazioneInviata2 = false;

                if (tmp[1] == 2 && tmp[2] != 0 && tmp[3] != 0)
                {
                    AttivaPulsanteNuovo = true;
                    NotificaInviata = true;
                    NotificaInviata2 = true;
                    AttivazioneInviata = true;
                    AttivazioneInviata2 = true;
                }

                if (tmp[1] == 2 && tmp[2] != 0 && tmp[3] == 0)
                {
                    NotificaInviata2 = true;
                    NotificaInviata = true;
                    AttivazioneInviata = true;
                }

                if (tmp[1] == 1 && tmp[2] != 0 && tmp[3] == 0)
                    NotificaInviata = true;


                decimal idAttivViaggioCongedo = tmp[0];//dvc.Restituisci_Id_Attivazioni_Viaggi_Congedo_DA(idViaggioCongedo, idFaseInCorso);

                if ((tmp[1] == 1 && tmp[3] != 0) || (tmp[1] == 2 && tmp[2] == 0 && tmp[3] == 0))
                {
                    AttivazioneInviata = true;
                    NotificaInviata = true;
                }

                if (tmp[1] == 2 && tmp[3] != 0)
                {
                    AttivazioneInviata2 = true;
                    AttivaPulsanteNuovo = true;
                    NotificaInviata = true;
                    NotificaInviata2 = true;
                    AttivazioneInviata = true;
                }

                //if (tmp[0] == 0 && tmp[1] == 0 && tmp[1] == 0 && tmp[3] == 0)
                //{
                //    AttivazioneInviata2 = false;
                //    AttivaPulsanteNuovo = false;
                //    NotificaInviata = false;
                //    NotificaInviata2 = false;
                //    AttivazioneInviata = false;
                //   // idFaseInCorso = 1;
                //}

                ViewData["AttivazioneInviata"] = AttivazioneInviata;
                ViewData["NotificaInviata"] = NotificaInviata;
                ViewData["NotificaInviata2"] = NotificaInviata2;
                ViewData["AttivaPulsanteNuovo"] = AttivaPulsanteNuovo;
                ViewData["AttivazioneInviata2"] = AttivazioneInviata2;
                ViewData["id_Attiv_Viaggio_Congedo"] = idAttivViaggioCongedo;
                ViewData["idFaseInCorso"] = idFaseInCorso;
                ViewData["idTrasferimento"] = idTrasferimento;
            }
    }
    public ActionResult NuovoDocumentoPreventivi(decimal idTrasferimento)
    {
        ViewData["idTrasferimento"] = idTrasferimento;
        try
        {
                AggiornaTuttiViewData(idTrasferimento);
                return PartialView();
        }
        catch (Exception ex)
        {
            return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
        }
    }
        //
        public JsonResult SalvaPreventivi(decimal idTrasferimento)
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
                            decimal idFaseInCorso = dtvc.Restituisci_LivelloFase_Da_ATT_Viagg_CONG(id_Viaggio_Congedo); 
                            id_Attiv_Viaggio_Congedo = x.idAttivazioneVC;
                            if(id_Attiv_Viaggio_Congedo==0)
                            {
                                id_Attiv_Viaggio_Congedo = dtvc.Crea_Attivazioni_Viaggi_Congedo(id_Viaggio_Congedo,idFaseInCorso,idTrasferimento);
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
        public ActionResult PulsantiPreventiviDiViaggio(decimal idViaggioCongedo , decimal idAttivViaggioCongedo , decimal idTrasferimento, bool nuovVC)
        {
            ViewData["NuovoViaggiCongedo"]= nuovVC ;
            ViewData["idTrasferimento"] = idTrasferimento;
            bool admin = Utility.Amministratore();
            ViewBag.Amministratore = admin;
            List<AttivazioniViaggiCongedoModel> lAttiv_Viaggio_Congedo = new List<AttivazioniViaggiCongedoModel>();
            using (dtViaggiCongedo dtvc = new dtViaggiCongedo())
            {
                decimal id_Viaggio_Congedo = dtvc.Identifica_Id_UltimoViaggioCongedoDisponibile(idTrasferimento);
                ViewData["id_Viaggio_Congedo"] = id_Viaggio_Congedo;
                 decimal idFaseInCorso = dtvc.Restituisci_LivelloFase_Da_ATT_Viagg_CONG(id_Viaggio_Congedo);
                decimal idFaseInCorso_1 = dtvc.Restituisci_IdAttivazioniVC_IdFaseInCorso_Attuale(idTrasferimento)[1];

                  lAttiv_Viaggio_Congedo = dtvc.Cerca_Id_AttivazioniViaggiCongedoDisponibile(id_Viaggio_Congedo,idFaseInCorso);
                if(lAttiv_Viaggio_Congedo?.Any()??false)
                    idAttivViaggioCongedo = lAttiv_Viaggio_Congedo.First().idAttivazioneVC;
                bool NotificaInviata = dtvc.NotificaPreventiviInviata(idAttivViaggioCongedo, idFaseInCorso);
                ViewData["id_Attiv_Viaggio_Congedo"] = idAttivViaggioCongedo;
                ViewData["NotificaInviata"] = NotificaInviata;
                ViewData["ConteggioElementi"] = dtvc.ConteggiaPreventiviRichiesta(idAttivViaggioCongedo);
                bool AttivazioneInviata = dtvc.AttivazionePreventiviInviata(idAttivViaggioCongedo, idFaseInCorso, idTrasferimento);
                ViewData["AttivazioneInviata"] = AttivazioneInviata;
                decimal idAttivViaggioCongedo2 = dtvc.Crea_Attivazioni_Viaggi_Congedo(idViaggioCongedo, (decimal)EnumFaseViaggioCongedo.Documenti_di_Viaggio,idTrasferimento);
                var nDocCartaImbarco = dtvc.GetNumDocumenti(idAttivViaggioCongedo2, EnumTipoDoc.Carta_Imbarco);
                var nDocTitoliViaggio = dtvc.GetNumDocumenti(idAttivViaggioCongedo2, EnumTipoDoc.Titolo_Viaggio);
                ViewData["nDocCartaImbarco"] = nDocCartaImbarco;
                // ViewData.Add("nDocCartaImbarco", nDocCartaImbarco);
                //  ViewData.Add("nDocTitoliViaggio", nDocTitoliViaggio);
                ViewData["nDocTitoliViaggio"] = nDocTitoliViaggio;
               // ViewData.Add("idFaseInCorso", idFaseInCorso);
                ViewData["idFaseInCorso"] = idFaseInCorso;
                bool CaricatiElementiFASE2 = dtvc.CaricatiElementiFASE2(idAttivViaggioCongedo, id_Viaggio_Congedo);
                ViewData["CaricatiElementiFASE2"] = CaricatiElementiFASE2;
                AggiornaTuttiViewData(idTrasferimento);
            }
            return PartialView();
        }
        //GestionePulsantiNotificaAttivaAnnulla
        public ActionResult GestionePulsantiNotificaAttivaAnnulla(decimal idTrasferimento)
        {
            AggiornaTuttiViewData(idTrasferimento);
            return PartialView();
        }

        public JsonResult ConfermaAttivaPreventiviRichiesta(decimal idAttivazioneVC,decimal idDocumento,decimal idFaseInCorso,decimal idTrasferimento)
        {
            bool admin = Utility.Amministratore();
            ViewBag.Amministratore = admin;           
            try
            {
                using (dtViaggiCongedo dtvc = new dtViaggiCongedo())
                {
                    decimal idViaggioCongedo = dtvc.Restituisci_ID_Viagg_CONG_DA_Trasferimento(idTrasferimento);
                    bool AttivazioneInviata = false; decimal tmp = 0;
                    decimal[] x = dtvc.Restituisci_IdAttivazioniVC_IdFaseInCorso_Attuale(idTrasferimento);
                    idFaseInCorso = x[1]; idAttivazioneVC = x[0];
                    if (idFaseInCorso == (decimal)EnumFaseViaggioCongedo.Preventivi)
                    {
                        AttivazioneInviata = dtvc.AttivazionePreventiviInviata(idAttivazioneVC, idFaseInCorso, idTrasferimento);
                        if (AttivazioneInviata == false)
                        {
                            tmp = dtvc.AttivaPreventiviRichiesta(idAttivazioneVC, idDocumento, idFaseInCorso, idTrasferimento);
                            
                            if (idFaseInCorso == 0)
                                idFaseInCorso = dtvc.Restituisci_LivelloFase_Da_ATT_Viagg_CONG(dtvc.Restituisci_ID_Viagg_CONG_DA(idAttivazioneVC, idTrasferimento));

                            //NotificaInviata = dtvc.NotificaPreventiviInviata(idAttivazioneVC, idFaseInCorso);
                            //ViewData["id_Attiv_Viaggio_Congedo"] = tmp;// idAttivazioneVC; il nuova ma non il vecchio
                            //ViewData["NotificaInviata"] = NotificaInviata;
                            //ViewData["ConteggioElementi"] = dtvc.ConteggiaPreventiviRichiesta(idAttivazioneVC);
                            //ViewData["id_Viaggio_Congedo"] = idViaggioCongedo;                           
                            //ViewData["idFaseInCorso"] = idFaseInCorso;
                            AggiornaTuttiViewData(idTrasferimento);

                            string oggetto = Resources.msgEmail.OggettoAttivaViaggiCongedo;
                            string corpoMessaggio = Resources.msgEmail.MessaggioAttivazioneViaggiCongedo;
                            InviaMailViaggioCongedo(idAttivazioneVC, idDocumento, corpoMessaggio, oggetto);
                        }                        
                    }
                    else
                    {
                        idDocumento = 0;
                        tmp = dtvc.AttivaPreventiviRichiesta(idAttivazioneVC, idDocumento, idFaseInCorso, idTrasferimento);
                        //NotificaInviata = dtvc.NotificaPreventiviInviata(idAttivazioneVC, idFaseInCorso);
                        //ViewData["id_Attiv_Viaggio_Congedo"] = tmp;// idAttivazioneVC; il nuova ma non il vecchio
                        //ViewData["NotificaInviata"] = NotificaInviata;
                        //ViewData["ConteggioElementi"] = dtvc.ConteggiaPreventiviRichiesta(idAttivazioneVC);
                        //ViewData["id_Viaggio_Congedo"] = 0;
                        //ViewData["idFaseInCorso"] = idFaseInCorso;
                        AggiornaTuttiViewData(idTrasferimento);
                        string oggetto = Resources.msgEmail.OggettoAttivaViaggiCongedo;
                        string corpoMessaggio = Resources.msgEmail.MessaggioAttivazioneViaggiCongedo;
                        InviaMailViaggioCongedo(idAttivazioneVC, 0, corpoMessaggio, oggetto);
                    }
                }
                //Inviare la mail
                
                return Json(new { err = "" });
            }
            catch (Exception ex)
            {
                return Json(new { err = "Errore documento non selezionato" });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAttivazioneVC"></param>
        /// <returns></returns>
        /// //  
        public JsonResult ConfermaEliminaDocumentoPreventivo(decimal idPreventivoDocumento)
        {
            try
            {
                using (dtViaggiCongedo dtvc = new dtViaggiCongedo())
                {
                    dtvc.EliminaDocumentoPreventivo(idPreventivoDocumento);
                }
                return Json(new { err = "" });
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }
        public JsonResult ConfermaAnnullaPreventiviRichiesta(decimal idAttivazioneVC,decimal idFaseInCorso,string corpoMessaggioVC,decimal idTrasferimento)
        {
            try
            {
                using (dtViaggiCongedo dtvc = new dtViaggiCongedo())
                {
                    dtvc.AnnullaPreventiviRichiesta(idAttivazioneVC, idFaseInCorso, idTrasferimento);
                }
                string oggetto = Resources.msgEmail.OggettoAnnullaViaggiCongedo;
                string corpoMessaggio = Resources.msgEmail.MessaggioAnnullamentoViaggiCongedo;
                if(corpoMessaggioVC.Trim()!="")corpoMessaggio = corpoMessaggioVC;
                InviaMailViaggioCongedo(idAttivazioneVC, 0, corpoMessaggio, oggetto);
                return Json(new { err = "" });
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }
        public JsonResult ConfermaNotificaPreventiviRichiesta(decimal idAttivazioneVC,decimal idTrasferimento)
        {
            try
            {

                using (dtViaggiCongedo dtvc = new dtViaggiCongedo())
                {
                    decimal id_Viaggio_Congedo=dtvc.Restituisci_ID_Viagg_CONG_DA(idAttivazioneVC, idTrasferimento);
                    decimal idFaseInCorso = dtvc.Restituisci_LivelloFase_Da_ATT_Viagg_CONG(id_Viaggio_Congedo);
                    dtvc.NotificaPreventiviRichiesta(idAttivazioneVC, idFaseInCorso);                
                    //decimal idFaseInCorso = dtvc.Restituisci_LivelloFase_Da_ATT_Viagg_CONG(dtvc.Restituisci_ID_Viagg_CONG_DA(idAttivazioneVC));
                    bool NotificaInviata = dtvc.NotificaPreventiviInviata(idAttivazioneVC, idFaseInCorso);
                    ViewData["id_Attiv_Viaggio_Congedo"] = idAttivazioneVC;
                    ViewData["NotificaInviata"] = NotificaInviata;
                    ViewData["ConteggioElementi"] = dtvc.ConteggiaPreventiviRichiesta(idAttivazioneVC);
                    ViewData["id_Viaggio_Congedo"] = id_Viaggio_Congedo;
                    ViewData["idFaseInCorso"] = idFaseInCorso;

                    //Inviare la mail

                    string oggetto = Resources.msgEmail.OggettoNotificaViaggiCongedo;
                    string corpoMessaggio = Resources.msgEmail.MessaggioNotificaViaggiCongedo;
                    InviaMailViaggioCongedo(idAttivazioneVC,0,corpoMessaggio,oggetto);
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
                    decimal idViaggioCongedio = dtvc.Restituisci_ID_Viagg_CONG_DA(idAttivViaggioCongedo, idTrasferimento);
                    bool admin = Utility.Amministratore();
                    ViewBag.Amministratore = admin;
                    decimal idFaseInCorso = dtvc.Restituisci_LivelloFase_Da_ATT_Viagg_CONG(idViaggioCongedio);
                    if(idFaseInCorso==(decimal)EnumFaseViaggioCongedo.Preventivi)
                    {
                        idFaseInCorso = (decimal)EnumFaseViaggioCongedo.Documenti_di_Viaggio;                       
                    }
                    idAttivViaggioCongedo = dtvc.Crea_Attivazioni_Viaggi_Congedo(idViaggioCongedio, idFaseInCorso,idTrasferimento);
                    bool NotificaInviata = dtvc.NotificaPreventiviInviata(idAttivViaggioCongedo, idFaseInCorso);
                    ViewData["id_Attiv_Viaggio_Congedo"] = idAttivViaggioCongedo;// idAttivazioneVC; il nuova ma non il vecchio
                    ViewData["NotificaInviata"] = NotificaInviata;
                    ViewData["ConteggioElementi"] = dtvc.ConteggiaPreventiviRichiesta(idAttivViaggioCongedo);
                    ViewData["id_Viaggio_Congedo"] = dtvc.Restituisci_ID_Viagg_CONG_DA(idAttivViaggioCongedo, idTrasferimento);
                   
                    ViewData["idFaseInCorso"] = idFaseInCorso;
                    ViewData["idTipoDocumento"] = idTipoDocumento;
                    if (idTipoDocumento == (decimal)EnumTipoDoc.Carta_Imbarco)
                        ViewData["EtichettaTipoDocumento"] = "Carte d'Imbarco";
                    if (idTipoDocumento == (decimal)EnumTipoDoc.Titolo_Viaggio)
                        ViewData["EtichettaTipoDocumento"] = "Titoli di Viaggio";

                    var nDocCartaImbarco = dtvc.GetNumDocumenti(idAttivViaggioCongedo, EnumTipoDoc.Carta_Imbarco);
                    var nDocTitoliViaggio = dtvc.GetNumDocumenti(idAttivViaggioCongedo, EnumTipoDoc.Titolo_Viaggio);
                    ViewData.Add("nDocCartaImbarco", nDocCartaImbarco);
                    ViewData.Add("nDocTitoliViaggio", nDocTitoliViaggio);
                    bool CaricatiElementiFASE2 = dtvc.CaricatiElementiFASE2(idAttivViaggioCongedo, idViaggioCongedio);
                    ViewData["CaricatiElementiFASE2"] = CaricatiElementiFASE2;

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
            
           
            if (idTipoDocumento == (decimal)EnumTipoDoc.Carta_Imbarco)
                ViewData["EtichettaTipoDocumento"] = "Carte d'Imbarco";
            if (idTipoDocumento == (decimal)EnumTipoDoc.Titolo_Viaggio)
                ViewData["EtichettaTipoDocumento"] = "Titoli di Viaggio";
            try
            {
                using (dtViaggiCongedo dtvc = new dtViaggiCongedo())
                {
                    decimal idViaggioCongedio = dtvc.Restituisci_ID_Viagg_CONG_DA(idAttivViaggioCongedo, idTrasferimento);
                    bool admin = Utility.Amministratore();
                    ViewBag.Amministratore = admin;
                    decimal[] tmp = dtvc.Restituisci_IdAttivazioniVC_IdFaseInCorso_Attuale(idTrasferimento);

                    idFaseInCorso =tmp[1];// dtvc.Restituisci_LivelloFase_Da_ATT_Viagg_CONG(idViaggioCongedio);
                    if (idFaseInCorso == (decimal)EnumFaseViaggioCongedo.Preventivi)
                    {
                        idFaseInCorso = (decimal)EnumFaseViaggioCongedo.Documenti_di_Viaggio;                        
                    }
                    idAttivViaggioCongedo = dtvc.Crea_Attivazioni_Viaggi_Congedo(idViaggioCongedio, idFaseInCorso, idTrasferimento);
                    bool NotificaInviata = dtvc.NotificaPreventiviInviata(idAttivViaggioCongedo, idFaseInCorso);
                    ViewData["id_Attiv_Viaggio_Congedo"] = idAttivViaggioCongedo;// idAttivazioneVC; il nuova ma non il vecchio
                    ViewData["NotificaInviata"] = NotificaInviata;
                    ViewData["idFaseInCorso"] = idFaseInCorso;
                    ViewData["idViaggioCongedo"] = idViaggioCongedo;
                    ViewData["idAttivViaggioCongedo"] = idAttivViaggioCongedo;
                    AggiornaTuttiViewData(idTrasferimento);
                    bool CaricatiElementiFASE2 = dtvc.CaricatiElementiFASE2(tmp[0], idViaggioCongedio);
                    ViewData["CaricatiElementiFASE2"] = CaricatiElementiFASE2;
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
            ViewData["idTrasferimento"] = idTrasferimento;
            List<ViaggioCongedoModel> lpv = new List<ViaggioCongedoModel>();
            List<ViaggioCongedoModel> lpvFinale = new List<ViaggioCongedoModel>();
            try
            {
                using (dtViaggiCongedo dvc = new dtViaggiCongedo())
                {
                    ViewData["idFaseInCorso"] = idFaseInCorso;
                    ViewData["id_Viaggio_Congedo"] = idViaggioCongedo;
                    ViewData["id_Attiv_Viaggio_Congedo"] = idAttivViaggioCongedo;
                    decimal[] tmp = dvc.Restituisci_IdAttivazioniVC_IdFaseInCorso_Attuale(idTrasferimento);
                    idFaseInCorso = tmp[1];
                    idAttivViaggioCongedo = tmp[0];

                    if (idFaseInCorso == (decimal)EnumFaseViaggioCongedo.Documenti_di_Viaggio)
                    {
                        lpv = dvc.GetDocFase2Viaggio(idAttivViaggioCongedo,idTipoDocumento);
                        lpvFinale.AddRange(lpv);
                    }
                    //lpv = dvc.GetUltimiPreventiviViaggio(idAttivViaggioCongedo);
                    //lpvFinale.AddRange(lpv);
                    bool admin = Utility.Amministratore();
                    ViewBag.Amministratore = admin;

                    bool AttivazioneInviata = dvc.AttivazionePreventiviInviata(idAttivViaggioCongedo, idFaseInCorso, idTrasferimento);
                    ViewData["AttivazioneInviata"] = AttivazioneInviata;

                    bool NotificaInviata = dvc.NotificaPreventiviInviata(idAttivViaggioCongedo, idFaseInCorso);
                    ViewData["NotificaInviata"] = NotificaInviata;

                    ViewData["idFaseInCorso"] = (decimal)EnumFaseViaggioCongedo.Documenti_di_Viaggio;

                    var nDocCartaImbarco = dvc.GetNumDocumenti(idAttivViaggioCongedo, EnumTipoDoc.Carta_Imbarco);
                    var nDocTitoliViaggio = dvc.GetNumDocumenti(idAttivViaggioCongedo, EnumTipoDoc.Titolo_Viaggio);
                    ViewData.Add("nDocCartaImbarco", nDocCartaImbarco);
                    ViewData.Add("nDocTitoliViaggio", nDocTitoliViaggio);
                    AggiornaTuttiViewData(idTrasferimento);
                    return PartialView(lpvFinale);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        public ActionResult NuovoDocumentoFase2(decimal idTrasferimento, decimal idTipoDocumento, decimal idAttivViaggioCongedo)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            ViewData["idTipoDocumento"] = idTipoDocumento;
            ViewData["idAttivViaggioCongedo"] = idAttivViaggioCongedo;
            AggiornaTuttiViewData(idTrasferimento);
            try
            {
                 return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        public JsonResult SalvaDocumentiFase2(decimal idTrasferimento,decimal idAttivViaggioCongedo,decimal idTipoDocumento)
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
                        //AGGIORNAMENTO TABELLE
                        //id_Viaggio_Congedo = dtvc.Identifica_Id_UltimoViaggioCongedoDisponibile(idTrasferimento);
                        //if (id_Viaggio_Congedo == 0)
                        //{
                        //    id_Viaggio_Congedo = dtvc.Crea_Viaggi_Congedo(idTrasferimento);
                        //}
                        foreach (string item in Request.Files)
                        {
                            HttpPostedFileBase file = Request.Files[item] as HttpPostedFileBase;
                            DocumentiModel dm = new DocumentiModel();
                            bool esisteFile = false;
                            bool gestisceEstensioni = false;
                            bool dimensioneConsentita = false;
                            string dimensioneMaxConsentita = string.Empty;
                            Utility.PreSetDocumento(file, out dm, out esisteFile, out gestisceEstensioni,
                                out dimensioneConsentita, out dimensioneMaxConsentita, (EnumTipoDoc)idTipoDocumento);

                            if (esisteFile)
                            {
                                if (gestisceEstensioni == false)
                                {
                                    throw new Exception(
                                        "Il documento selezionato non è nel formato consentito. Il formato supportato è: pdf.");
                                }

                                if (dimensioneConsentita)
                                {
                                    dtvc.SetDocumentiFase2ViaggiCongedio(ref dm, db, idAttivViaggioCongedo);
                                    DocVC = new SelectDocVc(); DocVC.idDocumento = dm.idDocumenti; DocVC.DocSelezionato = false;
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
        public JsonResult ConfermaNuovoViaggiCongedo(decimal idTrasferimento,Boolean nuovVC)
        {
            ViewData["idTrasferimento"]= idTrasferimento;
            ViewData["NuovoViaggiCongedo"] = nuovVC;
            try
            {
                using (dtViaggiCongedo dtvc = new dtViaggiCongedo())
                {
                    decimal  idViaggiCongedo=dtvc.Crea_Nuovo_Id_ViaggiCongedo(idTrasferimento);
                    ViewData["idViaggiCongedo"] = idViaggiCongedo;
                    AggiornaTuttiViewData(idTrasferimento);
                }
                 return Json(new { err = "" });
               // return PartialView("ElencoPreventiviDiViaggio");
            }
            catch (Exception ex)
            {
                 return Json(new { err = ex.Message });                
            }
        }
        public void InviaMailViaggioCongedo(decimal idAttivazioneVC, decimal idDocumento,string corpoMessaggio, string oggetto)
        {
            UtentiAutorizzatiModel uta = null;
            decimal idMittenteLogato = Utility.UtenteAutorizzato().idDipendente;
            ViewBag.idMittenteLogato = idMittenteLogato;
            NotificheModel nmod = new NotificheModel();
            using (dtViaggiCongedo dtn = new dtViaggiCongedo())
            {
                using (GestioneEmail gmail = new GestioneEmail())
                {
                    ModelloAllegatoMail allegato = new ModelloAllegatoMail();
                    Destinatario dest = new Destinatario();
                    Destinatario destToCc = new Destinatario();
                    ModelloMsgMail modMSGmail = new ModelloMsgMail();

                    if (idDocumento != 0)
                    {
                        var docByte = dtn.GetAllegatoVC(idAttivazioneVC, idDocumento);
                        Stream streamDoc = new MemoryStream(docByte);
                        DocumentiModel dm = dtn.GetDatiDocumentoById(idDocumento);
                        allegato.nomeFile = dm.nomeDocumento + "." + dm.estensione;
                        allegato.allegato = streamDoc;
                        modMSGmail.allegato.Add(allegato);
                    }
                    modMSGmail.oggetto = oggetto;
                    modMSGmail.corpoMsg = corpoMessaggio;
                    Mittente mitt = new Mittente();
                    mitt.EmailMittente = dtn.GetEmailByIdDipendente(idMittenteLogato);
                    decimal id_dip = dtn.RestituisciIDdestinatarioDaEmail(mitt.EmailMittente);
                    DipendentiModel dmod = dtn.RestituisciDipendenteByID(id_dip);
                    mitt.Nominativo = dmod.nome + " " + dmod.cognome;

                    decimal idDestinatario = dtn.Restituisci_ID_Destinatario(idAttivazioneVC);
                    string nome_ = dtn.RestituisciDipendenteByID(idDestinatario).nome;
                    string cognome_ = dtn.RestituisciDipendenteByID(idDestinatario).cognome;
                    string nominativo_ = nome_ + " " + cognome_;
                    dest = new Destinatario();
                    dest.EmailDestinatario = dtn.GetEmailByIdDipendente(idDestinatario);
                    dest.Nominativo = nominativo_;
                    modMSGmail.destinatario.Add(dest);

                    //il mittente deve anche ricevere in coppia la mail
                    destToCc = new Destinatario();                    
                    destToCc.EmailDestinatario = mitt.EmailMittente;
                    string nominativo_c =mitt.Nominativo;
                    destToCc.Nominativo = nominativo_c;
                    modMSGmail.cc.Add(destToCc);

                    //Qui mi assicuro che tutti gli amminsitratori siano inclusi in ToCc
                    var lls = dtn.GetListaDipendentiAutorizzati((decimal)EnumRuoloAccesso.Amministratore);
                    foreach (var x in lls)
                    {
                        bool found = false;
                        if (modMSGmail.cc.Count != 0)
                        {
                            var tmp = modMSGmail.cc.Where(a => a.EmailDestinatario.ToUpper().Trim() == x.email.ToUpper().Trim()).ToList();
                            if (tmp.Count() != 0) found = true;
                        }
                        if (found == false)
                        {
                            destToCc = new Destinatario();
                            string nome_cc = x.nome;
                            string cognome_cc = x.cognome;
                            destToCc.EmailDestinatario = x.email;
                            string nominativo_cc = nome_cc + " " + cognome_cc;
                            destToCc.Nominativo = nominativo_cc;
                            modMSGmail.cc.Add(destToCc);
                        }
                    }
                    ///////////////////////////////////////////////////////
                    modMSGmail.mittente = mitt;
                    gmail.sendMail(modMSGmail);
                }
            }
        }
    }

}
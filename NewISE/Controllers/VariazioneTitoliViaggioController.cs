using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewISE.EF;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;
using NewISE.Interfacce;
using System.Web.Helpers;
using NewISE.Models.Enumeratori;


namespace NewISE.Controllers
{
    public class VariazioneTitoliViaggioController : Controller
    {
        public ActionResult VariazioneTV(decimal idTrasferimento)
        {

            try
            {
                using (dtVariazioneTitoliViaggi dtvtv = new dtVariazioneTitoliViaggi())
                {
                    var idTitoliViaggio = dtvtv.GetIdTitoliViaggio(idTrasferimento);

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

        public ActionResult ElaborazioneTV(decimal idTitoliViaggio)
        {
            ViewData.Add("idTitoliViaggio", idTitoliViaggio);

            return PartialView();
        }

        public ActionResult TVCompletati(decimal idTitoliViaggio)
        {
            ViewData.Add("idTitoliViaggio", idTitoliViaggio);

            return PartialView();
        }

        [HttpPost]
        public ActionResult ElencoVariazioneTV(decimal idTitoliViaggio)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioneTitoliViaggi dtvtv = new dtVariazioneTitoliViaggi())
                    {
                        List<ElencoTitoliViaggioModel> ltvm = new List<ElencoTitoliViaggioModel>();

                        var atv = dtvtv.GetAttivazioneTV(idTitoliViaggio,db);
                        var atv_attivata = dtvtv.GetUltimaAttivazioneVariazioneAttivata(idTitoliViaggio, db);
                        bool richiestaEseguita = false;
                        bool richiestaDocumentiEseguita = false;
                        bool faseRichiestaDocumenti = false;
                        bool faseRichiesta = false;

                        //se esistono documenti assoctai all'attivazione vuol dire che sto 
                        //nella fase di invio documenti di viaggio
                        //altrimenti sto nella fase di richiesta titoli viaggio
                        if (atv_attivata.IDATTIVAZIONETITOLIVIAGGIO > 0)
                        {
                            if (dtvtv.VerificaDocumentiAttivazioneTV(atv_attivata.IDATTIVAZIONETITOLIVIAGGIO, db) == false)
                            {
                                faseRichiestaDocumenti = true;
                                if (atv.NOTIFICARICHIESTA == false && atv.ATTIVAZIONERICHIESTA == false)
                                {
                                    //elenco di tutti i familiari di cui ho richiesto il TV
                                    ltvm = dtvtv.ElencoTVDocumentiDaNotificare(atv);
                                }

                                if (atv.NOTIFICARICHIESTA && atv.ATTIVAZIONERICHIESTA == false)
                                {
                                    //elenco di tutti i familiari associati all'attivazione
                                    ltvm = dtvtv.ElencoTVDocumentiDaAttivare(atv);
                                    richiestaDocumentiEseguita = true;
                                }
                            }
                            else
                            {
                                faseRichiesta = true;

                                if (atv.NOTIFICARICHIESTA && atv.ATTIVAZIONERICHIESTA == false)
                                {
                                    //elenco di tutti i familiari associati all'attivazione
                                    ltvm = dtvtv.ElencoTVDaAttivare(atv);
                                    richiestaEseguita = true;
                                }

                                if (atv.NOTIFICARICHIESTA == false && atv.ATTIVAZIONERICHIESTA == false)
                                {
                                    //elenco di tutti i familiari che non hanno richiesto TV
                                    ltvm = dtvtv.ElencoTVDaRichiedere(atv, db);
                                }

                            }
                        }else
                        {
                            faseRichiesta = true;

                            if (atv.NOTIFICARICHIESTA && atv.ATTIVAZIONERICHIESTA == false)
                            {
                                //elenco di tutti i familiari associati all'attivazione
                                ltvm = dtvtv.ElencoTVDaAttivare(atv);
                                richiestaEseguita = true;
                            }

                            if (atv.NOTIFICARICHIESTA == false && atv.ATTIVAZIONERICHIESTA == false)
                            {
                                //elenco di tutti i familiari che non hanno richiesto TV
                                ltvm = dtvtv.ElencoTVDaRichiedere(atv, db);
                            }
                        }

                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            var t = dtt.GetTrasferimentoByIdTitoloViaggio(idTitoliViaggio);
                            EnumStatoTraferimento statoTrasferimento = t.idStatoTrasferimento;
                            ViewData.Add("statoTrasferimento", statoTrasferimento);
                        }


                        //bool richiestaEseguita = dtvtv.richiestaEseguita(idTitoliViaggio);

                        ViewData.Add("richiestaEseguita", richiestaEseguita);
                        ViewData.Add("faseRichiesta", faseRichiesta);
                        ViewData.Add("faseRichiestaDocumenti", faseRichiestaDocumenti);

                        ViewData.Add("richiestaDocumentiEseguita", richiestaDocumentiEseguita);
                        ViewData.Add("idTitoliViaggio", idTitoliViaggio);
                        ViewData.Add("idAttivazioneTV", atv.IDATTIVAZIONETITOLIVIAGGIO);


                        return PartialView(ltvm);
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }


        [HttpPost]
        public ActionResult ElencoTV_Completati(decimal idTitoliViaggio)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioneTitoliViaggi dtvtv = new dtVariazioneTitoliViaggi())
                    {
                        List<ElencoTitoliViaggioModel> ltvm = new List<ElencoTitoliViaggioModel>();

                        //elenco di tutti i familiari con documanti TV attivati
                        ltvm = dtvtv.ElencoTVDocumentiAttivati(idTitoliViaggio);
                        ViewData.Add("idTitoliViaggio", idTitoliViaggio);

                        return PartialView(ltvm);
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ElencoUploadVariazioneTV(decimal idTitoliViaggio)
        {
            try
            {

                using (dtVariazioneTitoliViaggi dtvtv = new dtVariazioneTitoliViaggi())
                {
                    bool notificaEseguita = false;
                    bool richiediNotifica = false;
                    bool richiediAttivazione = false;
                    bool richiediConiuge = false;
                    bool richiediFigli = false;
                    bool DocTitoliViaggio = false;
                    bool DocCartaImbarco = false;
                    bool inLavorazione = false;
                    bool trasfAnnullato = false;

                    var nDocCartaImbarco = dtvtv.GetNumDocumenti(idTitoliViaggio, EnumTipoDoc.Carta_Imbarco);
                    var nDocTitoliViaggio = dtvtv.GetNumDocumenti(idTitoliViaggio, EnumTipoDoc.Titolo_Viaggio);


                    dtvtv.SituazioneTitoliViaggio(idTitoliViaggio,
                                   out richiediNotifica, out richiediAttivazione,
                                   out richiediConiuge, out richiediFigli, out DocTitoliViaggio,
                                   out DocCartaImbarco, out inLavorazione, out trasfAnnullato);

                    if (richiediAttivazione)
                    {
                        notificaEseguita = true;
                    }

                    ViewData.Add("notificaEseguita", notificaEseguita);
                    ViewData.Add("idTitoliViaggio", idTitoliViaggio);
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

        public ActionResult GestionePulsantiNotificaAttiva_VariazioneTV(decimal idTitoliViaggio)
        {
            GestPulsantiVariazioneTVModel gpvtvm = new GestPulsantiVariazioneTVModel();

            try
            {
                using (dtVariazioneTitoliViaggi dtvtv = new dtVariazioneTitoliViaggi())
                {
                    gpvtvm = dtvtv.GestionePulsantiVariazioneTV(idTitoliViaggio);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(gpvtvm);
        }

        public JsonResult AggiornaStatoRichiediTV(decimal idParentela, decimal idAttivazioneTV, decimal idFamiliare)
        {
            try
            {
                using (dtVariazioneTitoliViaggi dtvtv = new dtVariazioneTitoliViaggi())
                {
                    dtvtv.Aggiorna_RichiediTV(idParentela, idAttivazioneTV, idFamiliare);
                }
            }
            catch (Exception ex)
            {
                return Json(new { errore = ex.Message, msg = "" });
            }
            return Json(new { errore = "", msg = "Aggiornamento eseguito correttamwente." });
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult AltriDatiFamiliariConiuge(decimal idTitoliViaggio, decimal idConiuge, decimal idTabTV)
        {
            AltriDatiFamConiugeModel adfcm = new AltriDatiFamConiugeModel();
            TitoloViaggioModel tvm = new TitoloViaggioModel();
            List<ElencoTitoliViaggioModel> ltvm = new List<ElencoTitoliViaggioModel>();

            try
            {

                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioneTitoliViaggi dtvtv = new dtVariazioneTitoliViaggi())
                    {

                        adfcm = dtvtv.GetAltriDatiFamiliariConiuge(idTitoliViaggio, idConiuge);

                        decimal idAttivazioneTV = dtvtv.GetAttivazioneTV(idTitoliViaggio, db).IDATTIVAZIONETITOLIVIAGGIO;

                        ViewData.Add("idTitoliViaggio", idTitoliViaggio);
                        ViewData.Add("idAttivazioneTV", idAttivazioneTV);
                        ViewData.Add("idTabTV", idTabTV);

                        using (dtConiuge dtc = new dtConiuge())
                        {
                            ConiugeModel c = dtc.GetConiugebyID(idConiuge);
                            if (c != null && c.HasValue())
                            {
                                switch (c.idTipologiaConiuge)
                                {
                                    case EnumTipologiaConiuge.Residente:
                                        adfcm.residente = true;
                                        adfcm.ulterioreMagConiuge = false;
                                        break;

                                    case EnumTipologiaConiuge.NonResidente_A_Carico:
                                        adfcm.residente = false;
                                        adfcm.ulterioreMagConiuge = true;
                                        break;

                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                        }
                    }

                    if (adfcm != null && adfcm.HasValue())
                    {
                        using (dtConiuge dtc = new dtConiuge())
                        {
                            var cm = dtc.GetConiugebyID(idConiuge);
                            adfcm.Coniuge = cm;
                        }
                    }
                }

                return PartialView(adfcm);

            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

        }

        public ActionResult AltriDatiFamiliariFiglio(decimal idTitoliViaggio, decimal idFiglio,decimal idTabTV)
        {
            AltriDatiFamFiglioModel adffm = new AltriDatiFamFiglioModel();
            TitoloViaggioModel tvm = new TitoloViaggioModel();
            List<ElencoTitoliViaggioModel> ltvm = new List<ElencoTitoliViaggioModel>();

            try
            {

                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioneTitoliViaggi dtvtv = new dtVariazioneTitoliViaggi())
                    {

                        adffm = dtvtv.GetAltriDatiFamiliariFiglio(idTitoliViaggio, idFiglio);

                        decimal idAttivazioneTV = dtvtv.GetAttivazioneTV(idTitoliViaggio,db).IDATTIVAZIONETITOLIVIAGGIO;

                        ViewData.Add("idTitoliViaggio", idTitoliViaggio);
                        ViewData.Add("idAttivazioneTV", idAttivazioneTV);
                        ViewData.Add("idTabTV", idTabTV);

                        using (dtFigli dtf = new dtFigli())
                        {
                            FigliModel f = dtf.GetFigliobyID(idFiglio);
                            if (f != null && f.HasValue())
                            {
                                switch (f.idTipologiaFiglio)
                                {
                                    case EnumTipologiaFiglio.Residente:
                                        adffm.residente = true;
                                        adffm.studente = false;
                                        break;
                                    case EnumTipologiaFiglio.StudenteResidente:
                                        adffm.studente = true;
                                        adffm.residente = true;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                        }

                    }

                    if (adffm != null && adffm.HasValue())
                    {
                        using (dtFigli dtc = new dtFigli())
                        {

                            var fm = dtc.GetFigliobyID(idFiglio);
                            adffm.Figli = fm;

                        }
                    }
                }

                return PartialView(adffm);

            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }


        public JsonResult ConfermaNotificaRichiestaVariazioneTV(decimal idTitoliViaggio)
        {
            string errore = "";

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioneTitoliViaggi dtvtv = new dtVariazioneTitoliViaggi())
                    {
                        decimal idAttivazione = dtvtv.GetAttivazioneTV(idTitoliViaggio, db).IDATTIVAZIONETITOLIVIAGGIO;

                        dtvtv.NotificaRichiestaTV(idAttivazione,db);
                    }
                }
            }
            catch (Exception ex)
            {

                errore = ex.Message;
            }

            return
                Json(
                    new
                    {
                        err = errore
                    });
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ConfermaAnnullaRichiestaVariazioneTV(FormCollection fc)
        {
            string errore = "";
            FormCollection collection = new FormCollection(Request.Unvalidated().Form);

            decimal idTitoliViaggio = Convert.ToDecimal(collection["idTitoliViaggio"]);
            string testoAnnulla = collection["msg"];


            try
            {
                using (dtVariazioneTitoliViaggi dtvtv = new dtVariazioneTitoliViaggi())
                {
                    decimal idAttivazione_notificata = dtvtv.GetUltimaAttivazioneNotificata(idTitoliViaggio).IDATTIVAZIONETITOLIVIAGGIO;

                    dtvtv.AnnullaRichiestaTV(idAttivazione_notificata, testoAnnulla);
                }
            }
            catch (Exception ex)
            {

                errore = ex.Message;
            }

            return
                Json(
                    new
                    {
                        err = errore
                    });
        }

        public JsonResult ConfermaAttivaRichiestaVariazioneTV(decimal idTitoliViaggio)
        {
            string errore = "";

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioneTitoliViaggi dtvtv = new dtVariazioneTitoliViaggi())
                    {
                        decimal idAttivazione = dtvtv.GetAttivazioneTV(idTitoliViaggio, db).IDATTIVAZIONETITOLIVIAGGIO;

                        if(dtvtv.VerificaDocumentiAttivazioneTV(idAttivazione,db))
                        {
                            dtvtv.AttivaRichiestaDocumentiTV(idAttivazione, db);
                        }else
                        {
                            dtvtv.AttivaRichiestaTV(idAttivazione, db);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                errore = ex.Message;
            }

            return
                Json(
                    new
                    {
                        err = errore
                    });
        }

        public ActionResult MessaggioAnnullaVariazioneTV(decimal idTitoliViaggio)
        {
            ModelloMsgMail msg = new ModelloMsgMail();

            try
            {

                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioneTitoliViaggi dtvtv = new dtVariazioneTitoliViaggi())
                    {
                        using (dtDipendenti dtd = new dtDipendenti())
                        {
                            using (dtTrasferimento dtt = new dtTrasferimento())
                            {
                                using (dtUffici dtu = new dtUffici())
                                {
                                    var t = dtt.GetTrasferimentoByIdTitoloViaggio(idTitoliViaggio);

                                    if (t?.idTrasferimento > 0)
                                    {
                                        var dip = dtd.GetDipendenteByID(t.idDipendente);
                                        var uff = dtu.GetUffici(t.idUfficio);

                                        var ultima_att_var_attivata = dtvtv.GetUltimaAttivazioneVariazioneAttivata(idTitoliViaggio, db);
                                        //var conta_attivazioni = dtvtv.GetNumAttivazioniTV(idTitoliViaggio, db);

                                        string messaggioAnnulla = "";

                                        if (ultima_att_var_attivata.IDATTIVAZIONETITOLIVIAGGIO > 0)
                                        {
                                            if (dtvtv.VerificaDocumentiAttivazioneTV(ultima_att_var_attivata.IDATTIVAZIONETITOLIVIAGGIO, db))
                                            {
                                                messaggioAnnulla = Resources.msgEmail.MessaggioAnnullaRichiestaInizialeTitoloViaggio;
                                            }
                                            else
                                            {
                                                messaggioAnnulla = Resources.msgEmail.MessaggioAnnullaRichiestaSuccessivaTitoloViaggio;
                                            }
                                        }
                                        else
                                        {
                                            messaggioAnnulla = Resources.msgEmail.MessaggioAnnullaRichiestaInizialeTitoloViaggio;
                                        }

                                        msg.corpoMsg = string.Format(messaggioAnnulla, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString());
                                        ViewBag.idTrasferimento = t.idTrasferimento;
                                        ViewBag.idTitoliViaggio = idTitoliViaggio;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView(msg);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DocumentoTV(
                                        decimal idTitoliViaggio, 
                                        decimal idFamiliare, 
                                        decimal idAttivazione, 
                                        decimal idParentela, 
                                        decimal idTipoDoc,
                                        decimal idConiugeTV,
                                        decimal idFigliTV,
                                        decimal idDocTV)
        {
            TrasferimentoModel trm = new TrasferimentoModel();
            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    using (dtDocumenti dtd = new dtDocumenti())
                    {
                        trm = dtt.GetTrasferimentoByIdTitoloViaggio(idTitoliViaggio);

                        var DescDocumento = dtd.GetDescrizioneTipoDocumentoByIdTipoDocumento(idTipoDoc);

                        ViewData.Add("idFamiliare", idFamiliare);
                        ViewData.Add("idAttivazione", idAttivazione);
                        ViewData.Add("idTipoDoc", idTipoDoc);
                        ViewData.Add("idParentela", idParentela);
                        ViewData.Add("idTrasferimento", trm.idTrasferimento);
                        ViewData.Add("idConiugeTV", idConiugeTV);
                        ViewData.Add("idFigliTV", idFigliTV);
                        ViewData.Add("DescDocumento", DescDocumento);
                        ViewData.Add("idDocTV", idDocTV);

                        return PartialView(trm);
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult InserisciDocumentoTV(
                                    decimal idTrasferimento, 
                                    decimal idTipoDocumento, 
                                    HttpPostedFileBase file, 
                                    decimal idFamiliare,
                                    decimal idAttivazione, 
                                    decimal idParentela,
                                    decimal idConiugeTV,
                                    decimal idFigliTV,
                                    decimal idDocTV)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                CONIUGETITOLIVIAGGIO ctv = new CONIUGETITOLIVIAGGIO();
                FIGLITITOLIVIAGGIO ftv = new FIGLITITOLIVIAGGIO();
                try
                {
                    db.Database.BeginTransaction();

                    using (dtVariazioneTitoliViaggi dtvtv = new dtVariazioneTitoliViaggi())
                    {

                        using (dtDocumenti dtd = new dtDocumenti())
                        {
                            DocumentiModel dm = new DocumentiModel();
                            bool esisteFile = false;
                            bool gestisceEstensioni = false;
                            bool dimensioneConsentita = false;
                            string dimensioneMaxConsentita = string.Empty;

                            Utility.PreSetDocumento(file, out dm, out esisteFile, out gestisceEstensioni,
                                out dimensioneConsentita, out dimensioneMaxConsentita,
                                (EnumTipoDoc)idTipoDocumento);

                            if (esisteFile)
                            {
                                if (gestisceEstensioni == false)
                                {
                                    throw new Exception(
                                        "Il documento selezionato non è nel formato consentito. Il formato supportato è: pdf.");
                                }

                                if (dimensioneConsentita)
                                {
                                    //verifica se il documento è gia presente ritornando l'eventuale id
                                    //decimal idDocumentoEsistente = dtvtv.VerificaEsistenzaDocumentoTV(idTrasferimento, idTipoDocumento, idParentela, idFamiliare);
                                    decimal idDocumentoEsistente = idDocTV;

                                    if (idDocumentoEsistente > 0)
                                    {
                                        //se già esiste lo sostituisco (imposto modificato=true su quello esistente e ne inserisco una altro)
                                        dtvtv.SostituisciDocumentoTV(ref dm, idDocumentoEsistente, idAttivazione, db);
                                    }
                                    else
                                    {
                                        //se non esiste lo inserisco
                                        dtvtv.SetDocumentoTV(ref dm, idAttivazione, db);
                                    }

                                    switch ((EnumParentela)idParentela)
                                    {
                                        case EnumParentela.Coniuge:
                                            dtvtv.AssociaDocumento_ConiugeTV(idConiugeTV, dm.idDocumenti, db);
                                            dtd.AssociaDocumentoConiuge(idFamiliare, dm.idDocumenti, db);
                                            break;
                                        case EnumParentela.Figlio:
                                            dtvtv.AssociaDocumento_FigliTV(idFigliTV, dm.idDocumenti, db);
                                            dtd.AssociaDocumentoFiglio(idFamiliare, dm.idDocumenti, db);
                                            break;
                                    }

                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (maggiorazione abitazione).", "Documenti", db, idTrasferimento, dm.idDocumenti);

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
                    return Json(new { msg = "Il documento è stato inserito." });
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    return Json(new { err = ex.Message });
                }
            }
        }

    }
}
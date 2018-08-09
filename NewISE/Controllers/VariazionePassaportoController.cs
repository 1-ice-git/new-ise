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
    public class VariazionePassaportoController : Controller
    {

        public ActionResult VariazionePassaporto(decimal idTrasferimento)
        {
            //leggo in che fase mi trovo
            using (dtVariazionePassaporto dtvp = new dtVariazionePassaporto())
            {
                //decimal idFasePassaportiCorrente = (decimal)dtvp.GetFasePassaporti_Corrente(idTrasferimento);

                ViewData.Add("idTrasferimento", idTrasferimento);
                //ViewData.Add("idFasePassaportiCorrente", idFasePassaportiCorrente);

                return PartialView();
            }

        }

        public ActionResult Passaporti_Richiesta(decimal idTrasferimento)
        {
            ViewData.Add("idTrasferimento", idTrasferimento);

            return PartialView();
        }

        public ActionResult PassaportoCompletato(decimal idTrasferimento)
        {
            ViewData.Add("idTrasferimento", idTrasferimento);

            return PartialView();
        }

        public ActionResult Passaporti_Invio(decimal idTrasferimento)
        {
            ViewData.Add("idTrasferimento", idTrasferimento);

            return PartialView();
        }


        public ActionResult ElaborazionePassaporti(decimal idTrasferimento)
        {
            //leggo in che fase mi trovo
            using (dtVariazionePassaporto dtvp = new dtVariazionePassaporto())
            {
                decimal idFasePassaportidaElaborare = (decimal)dtvp.GetFasePassaporti_Da_Elaborare(idTrasferimento);

                ViewData.Add("idTrasferimento", idTrasferimento);
                ViewData.Add("idFasePassaportidaElaborare", idFasePassaportidaElaborare);

                return PartialView();
            }
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ElencoFamiliariPassaporti_Richiesta(decimal idTrasferimento)
        {
            List<ElencoFamiliariPassaportoModel> lefm = new List<ElencoFamiliariPassaportoModel>();

            try
            {
                using (dtVariazionePassaporto dtvp = new dtVariazionePassaporto())
                {

                    lefm = dtvp.GetFamiliariRichiestaPassaporto(idTrasferimento).ToList();

                    ViewData.Add("idTrasferimento", idTrasferimento);
                    
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(lefm);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ElencoFamiliariPassaporti_Invio(decimal idTrasferimento)
        {
            List<ElencoFamiliariPassaportoModel> lefm = new List<ElencoFamiliariPassaportoModel>();

            try
            {
                using (dtVariazionePassaporto dtvp = new dtVariazionePassaporto())
                {

                    lefm = dtvp.GetFamiliariInvioPassaporto(idTrasferimento).ToList();

                    ViewData.Add("idTrasferimento", idTrasferimento);

                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(lefm);
        }




        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ColonnaElencoDoc(decimal idAttivazionePassaporto, decimal idFamiliarePassaporto, EnumParentela parentela, decimal idFaseCorrente)
        {
            ElencoFamiliariPassaportoModel efm = new ElencoFamiliariPassaportoModel();
            try
            { 
                using (dtVariazionePassaporto dtvp = new dtVariazionePassaporto())
                {
                    efm = dtvp.GetDatiForColElencoDoc(idAttivazionePassaporto, idFamiliarePassaporto, parentela);
                    ViewData.Add("idFaseCorrente", idFaseCorrente);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(efm);
        }

        [HttpPost]
        public ActionResult ColonnaElencoDocPassaporti(decimal idAttivazionePassaporto, decimal idFamiliarePassaporto, EnumParentela parentela)
        {
            ElencoFamiliariPassaportoModel efm = new ElencoFamiliariPassaportoModel();
            try
            {
                using (dtVariazionePassaporto dtvp = new dtVariazionePassaporto())
                {
                    efm = dtvp.GetDatiForColElencoDoc(idAttivazionePassaporto, idFamiliarePassaporto, parentela);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(efm);
        }

        public JsonResult ConfermaIncludiEscludiPassaporto_var(decimal id, EnumParentela parentela)
        {
            string errore = string.Empty;
            bool chk = false;
            decimal idAttivazioniPassaporto = 0;
            try
            {
                switch (parentela)
                {
                    case EnumParentela.Coniuge:
                        using (dtConiugePassaporto dtcp = new dtConiugePassaporto())
                        {
                            dtcp.SetIncludiEscludiPassaporto(id, ref chk, ref idAttivazioniPassaporto);
                        }
                        break;
                    case EnumParentela.Figlio:
                        using (dtFigliPassaporto dtfp = new dtFigliPassaporto())
                        {
                            dtfp.SetIncludiEscludiPassaporto(id, ref chk, ref idAttivazioniPassaporto);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("parentela");
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
                        chk = chk,
                        idAttivazioniPassaporto = idAttivazioniPassaporto,
                        err = errore
                    });

        }

        public ActionResult GestPulsantiNotificaAndPraticaConclusa_Richiesta(decimal idTrasferimento)
        {
            GestPulsantiAttConclModel gppm = new GestPulsantiAttConclModel();

            try
            {
                using (dtVariazionePassaporto dtvp = new dtVariazionePassaporto())
                {
                    gppm = dtvp.GestionePulsantiAttivazionePassaporto_Richiesta(idTrasferimento);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(gppm);
        }

        public ActionResult GestPulsantiNotificaAndPraticaConclusa_Invio(decimal idTrasferimento)
        {
            GestPulsantiAttConclModel gppm = new GestPulsantiAttConclModel();

            try
            {
                using (dtVariazionePassaporto dtvp = new dtVariazionePassaporto())
                {
                    gppm = dtvp.GestionePulsantiAttivazionePassaporto_Invio(idTrasferimento);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(gppm);
        }

        public JsonResult LeggiStatusPratichePassaporto(decimal idPassaporto)
        {
            string errore = string.Empty;
            GestPulsantiAttConclModel gppm = new GestPulsantiAttConclModel();
            bool notificaRichiesta = false;
            bool praticaConclusa = false;


            try
            {
                using (dtVariazionePassaporto dtvp = new dtVariazionePassaporto())
                {
                    //gppm = dtvp.GestionePulsantiPassaportoById(idPassaporto);
                    if (gppm != null)
                    {
                        notificaRichiesta = gppm.notificaRichiesta;
                        praticaConclusa = gppm.praticaConclusa;
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
                        err = errore,
                        notificaRichiesta = notificaRichiesta,
                        praticaConclusa = praticaConclusa
                    });
        }


        public ActionResult ChkIncludiPassaporto(decimal idAttivitaPassaporto, decimal idFamiliarePassaporto, EnumParentela parentela, bool esisteDoc, bool includiPassaporto, bool notificato)
        {
            GestioneChkincludiPassaportoModel gcip = new GestioneChkincludiPassaportoModel();

            try
            {
                using (dtVariazionePassaporto dtvp = new dtVariazionePassaporto())
                {
                    gcip = dtvp.GetGestioneInludiPassaporto_var(idAttivitaPassaporto, idFamiliarePassaporto, parentela, esisteDoc, includiPassaporto, notificato);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(gcip);
        }


        public JsonResult NotificaRichiesta(decimal idAttivazionePassaporto)
        {
            string errore = "";
            string msg = string.Empty;

            try
            {
                using (dtVariazionePassaporto dtvp = new dtVariazionePassaporto())
                {
                    dtvp.NotificaRichiestaPassaporto(idAttivazionePassaporto);
                    msg = "Notifica effettuata con successo";
                }
            }
            catch (Exception ex)
            {
                errore = ex.Message;
            }

            return Json(new { err = errore, msg = msg });
        }

        public JsonResult NotificaInvio(decimal idAttivazionePassaporto)
        {
            string errore = "";
            string msg = string.Empty;

            try
            {
                using (dtVariazionePassaporto dtvp = new dtVariazionePassaporto())
                {
                    dtvp.NotificaInvioPassaporto(idAttivazionePassaporto);
                    msg = "Notifica effettuata con successo";
                }
            }
            catch (Exception ex)
            {
                errore = ex.Message;
            }

            return Json(new { err = errore, msg = msg });
        }


        [HttpPost]
        [ValidateInput(false)]
        public JsonResult AnnullaRichiesta(FormCollection fc)
        {
            string errore = "";
            string msg = string.Empty;
            FormCollection collection = new FormCollection(Request.Unvalidated().Form);

            decimal idAttivazionePassaporto = Convert.ToDecimal(collection["idAttivazionePassaporto"]);
            string testoAnnulla = collection["msg"];

            try
            {
                using (dtVariazionePassaporto dtvp = new dtVariazionePassaporto())
                {
                    dtvp.AnnullaRichiestaPassaporto(idAttivazionePassaporto,testoAnnulla);
                    msg = "Annullamento effettuato con successo";
                }
            }
            catch (Exception ex)
            {
                errore = ex.Message;
            }

            return Json(new { err = errore, msg = msg });

        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult AnnullaInvio(FormCollection fc)
        {
            string errore = "";
            string msg = string.Empty;
            FormCollection collection = new FormCollection(Request.Unvalidated().Form);

            decimal idAttivazionePassaporto = Convert.ToDecimal(collection["idAttivazionePassaporto"]);
            string testoAnnulla = collection["msg"];

            try
            {
                using (dtVariazionePassaporto dtvp = new dtVariazionePassaporto())
                {
                    dtvp.AnnullaInvioPassaporto(idAttivazionePassaporto, testoAnnulla);
                    msg = "Annullamento effettuato con successo";
                }
            }
            catch (Exception ex)
            {
                errore = ex.Message;
            }

            return Json(new { err = errore, msg = msg });

        }


        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ConfermaRichiesta(FormCollection fc)
        {
            string errore = "";
            string msg = string.Empty;

            FormCollection collection = new FormCollection(Request.Unvalidated().Form);

            decimal idAttivazionePassaporto = Convert.ToDecimal(collection["idAttivazionePassaporto"]);
            string testoAttiva = collection["msg"];


            try
            {
                using (dtVariazionePassaporto dtvp = new dtVariazionePassaporto())
                {
                    dtvp.ConfermaRichiestaPassaporto(idAttivazionePassaporto, testoAttiva);
                    msg = "Pratica passaporto conclusa con successo";
                }
            }
            catch (Exception ex)
            {
                errore = ex.Message;
            }

            return Json(new { err = errore, msg = msg });
        }

        public JsonResult ConfermaInvio(decimal idAttivazionePassaporto)
        {
            string errore = "";
            string msg = string.Empty;

            try
            {
                using (dtVariazionePassaporto dtvp = new dtVariazionePassaporto())
                {
                    dtvp.ConfermaInvioPassaporto(idAttivazionePassaporto);
                    msg = "Pratica passaporto conclusa con successo";
                }
            }
            catch (Exception ex)
            {
                errore = ex.Message;
            }

            return Json(new { err = errore, msg = msg });
        }


        public ActionResult MessaggioAnnullaRichiestaPassaporto(decimal idAttivazionePassaporto)
        {
            ModelloMsgMail msg = new ModelloMsgMail();

            try
            {
                using (dtDipendenti dtd = new dtDipendenti())
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        using (dtUffici dtu = new dtUffici())
                        {
                            var t = dtt.GetTrasferimentoByIdAttPassaporto(idAttivazionePassaporto);

                            if (t?.idTrasferimento > 0)
                            {
                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                var uff = dtu.GetUffici(t.idUfficio);

                                msg.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaPassaporto, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString());
                                ViewBag.idTrasferimento = t.idTrasferimento;
                                ViewBag.idAttivazionePassaporto = idAttivazionePassaporto;
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

        public ActionResult MessaggioAttivaRichiestaPassaporto(decimal idAttivazionePassaporto)
        {
            ModelloMsgMail msg = new ModelloMsgMail();

            try
            {
                using (dtDipendenti dtd = new dtDipendenti())
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        using (dtUffici dtu = new dtUffici())
                        {
                            var t = dtt.GetTrasferimentoByIdAttPassaporto(idAttivazionePassaporto);

                            if (t?.idTrasferimento > 0)
                            {
                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                var uff = dtu.GetUffici(t.idUfficio);

                                msg.corpoMsg = string.Format(Resources.msgEmail.MessaggioRichiestaPratichePassaportoConcluse, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString());
                                ViewBag.idTrasferimento = t.idTrasferimento;
                                ViewBag.idAttivazionePassaporto = idAttivazionePassaporto;
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

        public ActionResult MessaggioAnnullaInvioPassaporto(decimal idAttivazionePassaporto)
        {
            ModelloMsgMail msg = new ModelloMsgMail();

            try
            {
                using (dtDipendenti dtd = new dtDipendenti())
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        using (dtUffici dtu = new dtUffici())
                        {
                            var t = dtt.GetTrasferimentoByIdAttPassaporto(idAttivazionePassaporto);

                            if (t?.idTrasferimento > 0)
                            {
                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                var uff = dtu.GetUffici(t.idUfficio);

                                msg.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaInvioPassaporto, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString());
                                ViewBag.idTrasferimento = t.idTrasferimento;
                                ViewBag.idAttivazionePassaporto = idAttivazionePassaporto;
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
        public ActionResult DocumentoPassaporto(decimal idTipoDocumento, decimal idTrasferimento)
        {
            TrasferimentoModel trm = new TrasferimentoModel();
            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    using (dtDocumenti dtd = new dtDocumenti())
                    {
                        trm = dtt.GetTrasferimentoById(idTrasferimento);

                        var DescDocumento = dtd.GetDescrizioneTipoDocumentoByIdTipoDocumento(idTipoDocumento);

                        ViewData.Add("idTrasferimento", idTrasferimento);
                        ViewData.Add("DescDocumento", DescDocumento);

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
        public JsonResult InserisciDocumentoPassaporto(decimal idTrasferimento, decimal idTipoDocumento, HttpPostedFileBase file, decimal idFamiliarePassaporto, decimal idParentela)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                PASSAPORTORICHIEDENTE pr = new PASSAPORTORICHIEDENTE();
                CONIUGEPASSAPORTO cp = new CONIUGEPASSAPORTO();
                FIGLIPASSAPORTO fp = new FIGLIPASSAPORTO();
                try
                {
                    db.Database.BeginTransaction();

                    using (dtVariazionePassaporto dtvp = new dtVariazionePassaporto())
                    {
                        AttivazionePassaportiModel apm = new AttivazionePassaportiModel();

                        //cerco l'attivazione della seconda fase in corso
                        apm = dtvp.GetAttivazioneInvioPassaportiInLavorazione(idTrasferimento, db);

                        //se non esiste segnala errore
                        if ((apm != null && apm.idAttivazioniPassaporti > 0) == false)
                        {
                            throw new Exception("Fase Invio Passaporto non trovata");

                        }

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
                                    decimal idDocumentoEsistente = dtvp.VerificaEsistenzaDocumentoPassaporto(idTrasferimento, idTipoDocumento,idParentela, idFamiliarePassaporto);

                                    if (idDocumentoEsistente > 0)
                                    {
                                        //se già esiste lo sostituisco (imposto modificato=true su quello esistente e ne inserisco una altro)
                                        dtvp.SostituisciDocumentoPassaporto(ref dm, idDocumentoEsistente, apm.idAttivazioniPassaporti, db);

                                    }
                                    else
                                    {
                                        //se non esiste lo inserisco
                                        dtvp.SetDocumentoPassaporto(ref dm, apm.idAttivazioniPassaporti, db);
                                    }

                                    switch ((EnumParentela)idParentela)
                                    {
                                        case EnumParentela.Coniuge:
                                            dtvp.AssociaDocumentoPassaportoConiuge(idFamiliarePassaporto, dm.idDocumenti, db);
                                            break;
                                        case EnumParentela.Figlio:
                                            dtvp.AssociaDocumentoPassaportoFiglio(idFamiliarePassaporto, dm.idDocumenti, db);
                                            break;
                                        case EnumParentela.Richiedente:
                                            dtvp.GetPassaportoRichiedente_Invio(ref pr, apm.idAttivazioniPassaporti, db);
                                            dtvp.AssociaDocumentoPassaportoRichiedente(pr.IDPASSAPORTORICHIEDENTE, dm.idDocumenti, db);
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

        [HttpPost]
        public ActionResult AltriDatiFamiliariConiugePassaporti(decimal idAltriDati, decimal idFaseCorrente)
        {
            AltriDatiFamConiugeModel adf = new AltriDatiFamConiugeModel();

            TrasferimentoModel tm = new TrasferimentoModel();

            try
            {
                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                {
                    adf = dtadf.GetAltriDatiFamiliariConiuge(idAltriDati);
                }

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    tm = dtt.GetTrasferimentoByIdConiuge(adf.idConiuge);
                }

                ViewData.Add("idFasePassaportiCorrente", idFaseCorrente);


            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


            ViewData.Add("idTrasferimento", tm.idTrasferimento);


            return PartialView(adf);
        }

        [HttpPost]
        public ActionResult AltriDatiFamiliariFiglioPassaporto(decimal idAltriDati, decimal idFaseCorrente)
        {
            AltriDatiFamFiglioModel adf = new AltriDatiFamFiglioModel();

            TrasferimentoModel tm = new TrasferimentoModel();

            try
            {
                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                {
                    adf = dtadf.GetAltriDatiFamiliariFiglio(idAltriDati);
                }

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    tm = dtt.GetTrasferimentoByIdFiglio(adf.idFigli);
                }
                
                ViewData.Add("idFasePassaportiCorrente", idFaseCorrente);

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewData.Add("idTrasferimento", tm.idTrasferimento);

            return PartialView(adf);
        }

        public ActionResult ElencoDocumentiPassaporto(decimal idFamiliarePassaporto, EnumTipoDoc tipoDoc, EnumParentela parentela, decimal idFaseCorrente)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();
            AttivazionePassaportiModel apm = new AttivazionePassaportiModel();
            bool solaLettura = false;
            decimal idTrasferimento = 0;
            EnumStatoTraferimento statoTrasferimento;

            try
            {
                using (dtAttivazionePassaporto dtap = new dtAttivazionePassaporto())
                {

                    using (dtDocumenti dtd = new dtDocumenti())
                    {
                        switch (parentela)
                        {
                            case EnumParentela.Coniuge:

                                ldm = dtd.GetDocumentiIdentitaConiugePassaporto(idFamiliarePassaporto).ToList();
                                apm = dtap.GetAttivazionePassaportiByIdConiugePassaporto(idFamiliarePassaporto);
                                break;

                            case EnumParentela.Figlio:
                                ldm = dtd.GetDocumentiIdentitaFiglioPassaporto(idFamiliarePassaporto).ToList();
                                apm = dtap.GetAttivazionePassaportiByIdFiglioPassaporto(idFamiliarePassaporto);
                                break;

                            case EnumParentela.Richiedente:
                                ldm = dtd.GetDocumentiIdentitaRichiedentePassaporto(idFamiliarePassaporto).ToList();
                                apm = dtap.GetAttivazionePassaportiByIdRichiedente(idFamiliarePassaporto);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("parentela");
                        }
                    }

                }

                using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                {
                    bool notificaRichiesta = false;
                    bool attivazioneRichiesta = false;
                    bool annullaRichiesta = false;

                    dtpp.SituazionePassaporto(apm.idAttivazioniPassaporti, out notificaRichiesta, out attivazioneRichiesta, out annullaRichiesta);

                    if (notificaRichiesta == true || attivazioneRichiesta == true)
                    {
                        solaLettura = true;
                    }
                    else
                    {
                        solaLettura = false;
                    }

                    //var idFasePassaportiCorrente = dtpp.GetFasePassaporti_Corrente(apm.idPassaporti);
                    if (idFaseCorrente == (decimal)EnumFasePassaporti.Invio_Passaporti)
                    {
                        solaLettura = true;
                    }
                 
                }

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var t = dtt.GetTrasferimentoByIdAttPassaporto(apm.idAttivazioniPassaporti);
                    idTrasferimento = t.idTrasferimento;
                    statoTrasferimento = t.idStatoTrasferimento;

                    if (statoTrasferimento == EnumStatoTraferimento.Attivo || statoTrasferimento == EnumStatoTraferimento.Annullato)
                    {
                        solaLettura = true;
                    }

                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewData.Add("solaLettura", solaLettura);
            ViewData.Add("idFamiliarePassaporto", idFamiliarePassaporto);
            ViewData.Add("tipoDoc", (decimal)tipoDoc);
            ViewData.Add("idAttivazionePassaporto", apm.idAttivazioniPassaporti);
            ViewData.Add("parentela", (decimal)parentela);
            ViewData.Add("chiamante", (decimal)EnumChiamante.Passaporti);
            ViewData.Add("idTrasferimento", idTrasferimento);
            ViewData.Add("idFaseCorrente", idFaseCorrente);

            return PartialView(ldm);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ElencoPassaporto_Completato(decimal idTrasferimento)
        {
            List<ElencoFamiliariPassaportoModel> lefm = new List<ElencoFamiliariPassaportoModel>();

            try
            {
                using (dtVariazionePassaporto dtvp = new dtVariazionePassaporto())
                {

                    lefm = dtvp.GetFamiliariPassaportoCompletato(idTrasferimento).ToList();

                    ViewData.Add("idTrasferimento", idTrasferimento);

                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(lefm);
        }

    }
}
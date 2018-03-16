using System.Web.Routing;
using NewISE.EF;
using NewISE.Models.Tools;
using MaggiorazioniFamiliariModel = NewISE.Models.DBModel.MaggiorazioniFamiliariModel;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.ViewModel;
using NewISE.Interfacce;
using System.Web.Helpers;

namespace NewISE.Controllers
{
    public class TitoliViaggioController : Controller
    {

        public JsonResult AggiornaStatoRichiediTitoloViaggio(decimal idParentela, decimal idAttivazioneTitoliViaggio, decimal idFamiliare)
        {
            try
            {
                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    dttv.Aggiorna_RichiediTitoloViaggio(idParentela, idAttivazioneTitoliViaggio, idFamiliare);
                }
            }
            catch (Exception ex)
            {
                return Json(new { errore = ex.Message, msg = "" });
            }
            return Json(new { errore = "", msg = "Aggiornamento eseguito correttamwente." });
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult AltriDatiFamiliariConiuge(decimal idTitoliViaggio, decimal idConiuge)
        {
            AltriDatiFamConiugeModel adfcm = new AltriDatiFamConiugeModel();
            TitoloViaggioModel tvm = new TitoloViaggioModel();
            List<ElencoTitoliViaggioModel> ltvm = new List<ElencoTitoliViaggioModel>();

            try
            {

                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {

                    adfcm = dttv.GetAltriDatiFamiliariConiuge(idTitoliViaggio, idConiuge);

                    decimal idAttivazioneTitoliViaggio = dttv.GetUltimaAttivazioneTitoliViaggio(idTitoliViaggio).IDATTIVAZIONETITOLIVIAGGIO;

                    ViewData.Add("idTitoliViaggio", idTitoliViaggio);
                    ViewData.Add("idAttivazioneTitoliViaggio", idAttivazioneTitoliViaggio);

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

                return PartialView(adfcm);

            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

        }

        public ActionResult AltriDatiFamiliariFiglio(decimal idTitoliViaggio, decimal idFiglio)
        {
            AltriDatiFamFiglioModel adffm = new AltriDatiFamFiglioModel();
            TitoloViaggioModel tvm = new TitoloViaggioModel();
            List<ElencoTitoliViaggioModel> ltvm = new List<ElencoTitoliViaggioModel>();

            try
            {

                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {

                    adffm = dttv.GetAltriDatiFamiliariFiglio(idTitoliViaggio, idFiglio);

                    decimal idAttivazioneTitoliViaggio = dttv.GetUltimaAttivazioneTitoliViaggio(idTitoliViaggio).IDATTIVAZIONETITOLIVIAGGIO;

                    ViewData.Add("idTitoliViaggio", idTitoliViaggio);
                    ViewData.Add("idAttivazioneTitoliViaggio", idAttivazioneTitoliViaggio);

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

                return PartialView(adffm);

            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

        }


        [HttpPost]
        public ActionResult TitoliViaggio(decimal idTrasferimento)
        {
            using (dtTitoliViaggi dttv = new dtTitoliViaggi())
            {
                var idTitoliViaggio = dttv.GetIdTitoliViaggio(idTrasferimento);

                ViewData.Add("idTitoliViaggio", idTitoliViaggio);
                ViewData.Add("idTrasferimento", idTrasferimento);

                return PartialView();
            }
        }

        [HttpPost]
        public ActionResult ElencoUploadTitoliViaggio(decimal idTitoliViaggio)
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

                var nDocCartaImbarco = dttv.GetNumDocumenti(idTitoliViaggio, EnumTipoDoc.Carta_Imbarco);
                var nDocTitoliViaggio = dttv.GetNumDocumenti(idTitoliViaggio, EnumTipoDoc.Titolo_Viaggio);

                //var atv_notificata = dttv.GetUltimaAttivazioneNotificata(idTitoliViaggio);

                dttv.SituazioneTitoliViaggio(idTitoliViaggio,
                               out richiediNotifica, out richiediAttivazione,
                               out richiediConiuge, out richiediRichiedente,
                               out richiediFigli, out DocTitoliViaggio,
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


        [HttpPost]
        public ActionResult ElencoTitoliViaggio(decimal idTitoliViaggio)
        {
            using (dtTitoliViaggi dttv = new dtTitoliViaggi())
            {
                List<ElencoTitoliViaggioModel> ltvm = new List<ElencoTitoliViaggioModel>();

                var atv = dttv.GetUltimaAttivazioneTitoliViaggio(idTitoliViaggio);

                decimal idAttivazioneTitoliViaggio = atv.IDATTIVAZIONETITOLIVIAGGIO;

                if (idAttivazioneTitoliViaggio > 0)
                {
                    ltvm = dttv.ElencoTitoliViaggio(idTitoliViaggio);
                }

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var t = dtt.GetTrasferimentoByIdTitoloViaggio(idTitoliViaggio);
                    EnumStatoTraferimento statoTrasferimento = t.idStatoTrasferimento;
                    ViewData.Add("statoTrasferimento", statoTrasferimento);
                }
 

                bool richiestaEseguita = dttv.richiestaEseguita(idTitoliViaggio);

                ViewData.Add("richiestaEseguita", richiestaEseguita);
                ViewData.Add("idTitoliViaggio", idTitoliViaggio);
                ViewData.Add("idAttivazioneTitoliViaggio", idAttivazioneTitoliViaggio);
    

                return PartialView(ltvm);
            }
        }

        public ActionResult ElencoDocumentiTV(decimal idTipoDocumento, decimal idTitoliViaggio)
        {
            string DescrizioneTV = "";

            using (dtDocumenti dtd = new dtDocumenti())
            {
                DescrizioneTV = dtd.GetDescrizioneTipoDocumentoByIdTipoDocumento(idTipoDocumento);
            }

            bool richiestaNotificata = false;

            List<SelectListItem> lDataAttivazione = new List<SelectListItem>();
            List<AttivazioneTitoliViaggioModel> latvm = new List<AttivazioneTitoliViaggioModel>();
            try
            {
                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    latvm = dttv.GetListAttivazioniTitoliViaggioByTipoDoc(idTitoliViaggio,idTipoDocumento).OrderBy(a => a.idAttivazioneTitoliViaggio).ToList();

                    //var i = latvm.Count();
                    var i = 1;

                    foreach (var atv in latvm)
                    {
                        if(dttv.AttivazioneNotificata(atv.idAttivazioneTitoliViaggio))
                        {
                            richiestaNotificata = true;
                        }

                        bool inLavorazione = dttv.AttivazioneTitoliViaggioInLavorazione(atv.idAttivazioneTitoliViaggio, idTitoliViaggio);

                        if (inLavorazione)
                        {
                            lDataAttivazione.Insert(0, new SelectListItem() { Text = "(" + i.ToString() + ") " + atv.dataAggiornamento.ToString() + " (In Lavorazione)", Value = atv.idAttivazioneTitoliViaggio.ToString() });
                        }else
                        {
                            lDataAttivazione.Insert(0, new SelectListItem() { Text = "(" + i.ToString() + ") " + atv.dataAggiornamento.ToString(), Value = atv.idAttivazioneTitoliViaggio.ToString() });
                        }
                        //i--;
                        i++;
                    }

                    lDataAttivazione.Insert(0, new SelectListItem() { Text = "(TUTTE)", Value = "" });
                    ViewData.Add("lDataAttivazione", lDataAttivazione);

                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var t = dtt.GetTrasferimentoByIdTitoloViaggio(idTitoliViaggio);
                        EnumStatoTraferimento statoTrasferimento = (EnumStatoTraferimento)t.idStatoTrasferimento;
                        ViewData.Add("statoTrasferimento", statoTrasferimento);
                    }

                    ViewData.Add("DescrizioneTV", DescrizioneTV);
                    ViewData.Add("idTipoDocumento", idTipoDocumento);
                    ViewData.Add("idTitoliViaggio", idTitoliViaggio);
                    ViewData.Add("richiestaNotificata", richiestaNotificata);
                }

                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }


        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult TabDocumentiTVInseriti(decimal idTitoliViaggio, decimal idTipoDocumento)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            string DescrizioneTV = "";

            try
            {

                using (dtTitoliViaggi dtd = new dtTitoliViaggi())
                {
                    ldm = dtd.GetDocumentiTV(idTitoliViaggio, idTipoDocumento);
                }


                using (dtDocumenti dtd = new dtDocumenti())
                {
                    DescrizioneTV = dtd.GetDescrizioneTipoDocumentoByIdTipoDocumento(idTipoDocumento);
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewData.Add("DescrizioneTV", DescrizioneTV);
            ViewData.Add("idTipoDocumento", idTipoDocumento);
            ViewData.Add("idTitoliViaggio", idTitoliViaggio);

            return PartialView(ldm);
        }


        public ActionResult NuovoDocumentoTV(EnumTipoDoc idTipoDocumento, decimal idTitoliViaggio)
        {
            string titoloPagina = string.Empty;

            using (dtTitoliViaggi dtmf = new dtTitoliViaggi())
            {
                switch (idTipoDocumento)
                {
                    case EnumTipoDoc.Carta_Imbarco:
                        titoloPagina = "Carta d'imbarco";
                        break;

                    case EnumTipoDoc.Titolo_Viaggio:
                        titoloPagina = "Titolo di viaggio";
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("idTipoDocumento");
                }
            }

            ViewData.Add("titoloPagina", titoloPagina);
            ViewData.Add("idTipoDocumento", (decimal)idTipoDocumento);
            ViewData.Add("idTitoliViaggio", idTitoliViaggio);

            return PartialView();
        }

        public static void PreSetDocumentoTV(HttpPostedFileBase file, out DocumentiModel dm, out bool esisteFile, out bool gestisceEstensioni, out bool dimensioneConsentita, out string dimensioneMaxDocumento, decimal idTipoDocumento)
        {
            dm = new DocumentiModel();
            gestisceEstensioni = false;
            dimensioneConsentita = false;
            esisteFile = false;
            dimensioneMaxDocumento = string.Empty;

            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    esisteFile = true;

                    var estensioniGestite = new[] { ".pdf" };
                    var estensione = Path.GetExtension(file.FileName);
                    var nomeFileNoEstensione = Path.GetFileNameWithoutExtension(file.FileName);
                    if (!estensioniGestite.Contains(estensione.ToLower()))
                    {
                        gestisceEstensioni = false;
                    }
                    else
                    {
                        gestisceEstensioni = true;
                    }
                    var keyDimensioneDocumento = System.Configuration.ConfigurationManager.AppSettings["DimensioneDocumento"];

                    dimensioneMaxDocumento = keyDimensioneDocumento;

                    if (file.ContentLength / 1024 <= Convert.ToInt32(keyDimensioneDocumento))
                    {
                        dm.nomeDocumento = nomeFileNoEstensione;
                        dm.estensione = estensione;
                        dm.tipoDocumento = (EnumTipoDoc)idTipoDocumento;
                        dm.dataInserimento = DateTime.Now;
                        dm.file = file;
                        dimensioneConsentita = true;
                    }
                    else
                    {
                        dimensioneConsentita = false;
                    }

                }
                else
                {
                    esisteFile = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalvaDocumentoTV(decimal idTipoDocumento, decimal idTitoliViaggio)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    foreach (string item in Request.Files)
                    {

                        HttpPostedFileBase file = Request.Files[item] as HttpPostedFileBase;

                        using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                        {
                            using (dtDocumenti dtd = new dtDocumenti())
                            {
                                DocumentiModel dm = new DocumentiModel();
                                bool esisteFile = false;
                                bool gestisceEstensioni = false;
                                bool dimensioneConsentita = false;
                                string dimensioneMaxConsentita = string.Empty;

                                PreSetDocumentoTV(file, out dm, out esisteFile, out gestisceEstensioni,
                                    out dimensioneConsentita, out dimensioneMaxConsentita, idTipoDocumento);

                                if (esisteFile)
                                {
                                    if (gestisceEstensioni == false)
                                    {
                                        throw new Exception(
                                        "Il documento selezionato non è nel formato consentito. Il formato supportato è: pdf.");
                                    }

                                    if (dimensioneConsentita)
                                    {
                                        dttv.SetDocumentoTV(ref dm, idTitoliViaggio, db, idTipoDocumento);

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

        public JsonResult EliminaDocumentoTV(decimal idDocumento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                    {
                        dttv.DeleteDocumentoTV(idDocumento);
                    }
                    db.Database.CurrentTransaction.Commit();
                    return Json(new { msg = "Il titolo di viaggio è stato eliminato." });
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    return Json(new { err = ex.Message });
                }
            }
        }

        public ActionResult FiltraDocumentiTV(decimal idTitoliViaggio, decimal idAttivazioneTV, decimal idTipoDocumento)
        {

            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            string DescrizioneTV = "";

            try
            {

                using (dtTitoliViaggi dtd = new dtTitoliViaggi())
                {
                    ldm = dtd.GetDocumentiTVbyIdAttivazioneTV(idTitoliViaggio, idAttivazioneTV, idTipoDocumento);
                }

                using (dtDocumenti dtd = new dtDocumenti())
                {
                    DescrizioneTV = dtd.GetDescrizioneTipoDocumentoByIdTipoDocumento(idTipoDocumento);
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewData.Add("DescrizioneTV", DescrizioneTV);
            ViewData.Add("idTipoDocumento", idTipoDocumento);
            ViewData.Add("idTitoliViaggio", idTitoliViaggio);

            return PartialView("TabDocumentiTVInseriti", ldm);
        }

        public JsonResult GestionePulsantiNotificaAttivaAnnulla(decimal idTitoliViaggio)
        {

            bool amministratore = false;
            string errore = "";
            bool richiediAttivazione = false;
            bool richiediAnnulla = false;
            bool richiediNotifica = false;
            bool richiediRichiedente = false;
            bool richiediConiuge = false;
            bool richiediFigli = false;
            bool DocTitoliViaggio = false;
            bool DocCartaImbarco = false;
            bool inLavorazione = false;
            bool trasfAnnullato = false;

            try
            {
                amministratore = Utility.Amministratore();

                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {

                    dttv.SituazioneTitoliViaggio(idTitoliViaggio,
                               out richiediNotifica, out richiediAttivazione,
                               out richiediConiuge, out richiediRichiedente,
                               out richiediFigli, out DocTitoliViaggio,
                               out DocCartaImbarco, out inLavorazione, out trasfAnnullato);
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
                        admin = amministratore,
                        richiediAttivazione = richiediAttivazione,
                        richiediNotifica = richiediNotifica,
                        richiediAnnulla=richiediAnnulla,
                        inLavorazione = inLavorazione,
                        trasfAnnullato = trasfAnnullato,
                        err = errore
                    });

        }


        public JsonResult ConfermaNotificaRichiestaTV(decimal idTitoliViaggio)
        {
            string errore = "";

            try
            {
                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    decimal idAttivazione = dttv.GetUltimaAttivazioneTitoliViaggio(idTitoliViaggio).IDATTIVAZIONETITOLIVIAGGIO;

                    dttv.NotificaRichiestaTV(idAttivazione);
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
        public JsonResult ConfermaAnnullaRichiestaTV(FormCollection fc)
        {
            string errore = "";
            FormCollection collection = new FormCollection(Request.Unvalidated().Form);

            decimal idTitoliViaggio = Convert.ToDecimal(collection["idTitoliViaggio"]);
            string testoAnnulla = collection["msg"];


            try
            {
                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    decimal idAttivazione_notificata = dttv.GetUltimaAttivazioneNotificata(idTitoliViaggio).IDATTIVAZIONETITOLIVIAGGIO;

                    dttv.AnnullaRichiestaTitoliViaggio(idAttivazione_notificata, testoAnnulla);
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

        public JsonResult ConfermaAttivaRichiestaTV(decimal idTitoliViaggio)
        {
            string errore = "";

            try
            {
                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    decimal idAttivazione = dttv.GetUltimaAttivazioneTitoliViaggio(idTitoliViaggio).IDATTIVAZIONETITOLIVIAGGIO;

                    dttv.AttivaRichiestaTV(idAttivazione);
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

        public ActionResult MessaggioAnnullaTV(decimal idTitoliViaggio)
        {
            ModelloMsgMail msg = new ModelloMsgMail();

            try
            {

                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtTitoliViaggi dttv = new dtTitoliViaggi())
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

                                        var conta_attivazioni = dttv.GetNumAttivazioniTV(idTitoliViaggio, db);
                                        string messaggioAnnulla = "";

                                        if (conta_attivazioni == 1)
                                        {
                                            messaggioAnnulla = Resources.msgEmail.MessaggioAnnullaRichiestaInizialeTitoloViaggio;
                                        }
                                        else
                                        {
                                            messaggioAnnulla = Resources.msgEmail.MessaggioAnnullaRichiestaSuccessivaTitoloViaggio;
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


    }
}
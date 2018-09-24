using System.Web.Routing;
using NewISE.EF;
using NewISE.Models.Tools;
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
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.Models.Enumeratori;

namespace NewISE.Controllers
{
    public class TrasportoEffettiController : Controller
    {
        [HttpPost]
        public ActionResult TrasportoEffetti(decimal idTrasferimento)
        {
            using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
            {
                ViewData.Add("idTrasportoEffettiPartenza", idTrasferimento);
                ViewData.Add("idTrasferimento", idTrasferimento);

                return PartialView();
            }
        }



        [HttpPost]
        public ActionResult TrasportoEffettiPartenza(decimal idTrasportoEffettiPartenza)
        {
            try
            {
                using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        bool richiestaTE = false;
                        bool attivazioneTE = false;
                        bool DocContributo = false;
                        bool trasfAnnullato = false;
                        bool rinunciaTEPartenza = false;

                        TrasportoEffettiPartenzaModel tepm = new TrasportoEffettiPartenzaModel();

                        var atep = dtte.GetUltimaAttivazioneTEPartenza(idTrasportoEffettiPartenza);

                        dtte.SituazioneTEPartenza(idTrasportoEffettiPartenza,
                                                    out richiestaTE, out attivazioneTE,
                                                    out DocContributo,
                                                    out trasfAnnullato, out rinunciaTEPartenza);

                        var tm = dtt.GetTrasferimentoByIdTEPartenza(idTrasportoEffettiPartenza);

                        CalcoliIndennita ci = new CalcoliIndennita(tm.idTrasferimento, tm.dataPartenza);

                        tepm.indennitaPrimaSistemazione = Math.Round(ci.IndennitaSistemazioneLorda, 2);
                        tepm.percKM = ci.PercentualeFKMPartenza;
                        tepm.contributoLordo = Math.Round(ci.TotaleContributoOmnicomprensivoPartenza, 2);
                        var PercentualeAnticipoTE = dtte.GetPercentualeAnticipoTEPartenza(idTrasportoEffettiPartenza, (decimal)EnumTipoAnticipoTE.Partenza);
                        tepm.percAnticipo = PercentualeAnticipoTE.PERCENTUALE;
                        tepm.anticipo = Math.Round(tepm.percAnticipo * tepm.contributoLordo / 100, 2);

                        ViewData.Add("rinunciaTEPartenza", rinunciaTEPartenza);
                        ViewData.Add("richiestaTE", richiestaTE);
                        ViewData.Add("attivazioneTE", attivazioneTE);
                        ViewData.Add("DocContributo", DocContributo);
                        ViewData.Add("idTrasportoEffettiPartenza", idTrasportoEffettiPartenza);

                        return PartialView(tepm);
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

        }

        public JsonResult ConfermaNotificaRichiestaTEPartenza(decimal idTrasportoEffettiPartenza)
        {
            string errore = "";

            try
            {
                using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                {
                    decimal idAttivitaTEPartenza = dtte.GetUltimaAttivazioneTEPartenza(idTrasportoEffettiPartenza).IDATEPARTENZA;
                    dtte.NotificaRichiestaTEPartenza(idAttivitaTEPartenza);
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

        public ActionResult ElencoDocumentiTEPartenza(decimal idTipoDocumento, decimal idTrasportoEffettiPartenza)
        {
            try
            {
                string DescrizioneTE = "";
                bool richiestaTEPartenza = false;
                bool attivazioneTEPartenza = false;
                decimal NumAttivazioniTEPartenza = 0;
                decimal idStatoTrasferimento = 0;
                bool rinunciaTEPartenza = false;

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var t = dtt.GetTrasferimentoByIdTEPartenza(idTrasportoEffettiPartenza);
                    idStatoTrasferimento = (decimal)t.idStatoTrasferimento;
                }

                using (dtDocumenti dtd = new dtDocumenti())
                {
                    DescrizioneTE = dtd.GetDescrizioneTipoDocumentoByIdTipoDocumento(idTipoDocumento);
                }

                using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                {
                    var atep = dtte.GetUltimaAttivazioneTEPartenza(idTrasportoEffettiPartenza);
                    if (atep.RICHIESTATRASPORTOEFFETTI && atep.ATTIVAZIONETRASPORTOEFFETTI == false)
                    {
                        richiestaTEPartenza = true;
                    }
                    if (atep.RICHIESTATRASPORTOEFFETTI && atep.ATTIVAZIONETRASPORTOEFFETTI)
                    {
                        attivazioneTEPartenza = true;
                        richiestaTEPartenza = true;
                    }

                    using (ModelDBISE db = new ModelDBISE())
                    {
                        var rtep = dtte.GetRinunciaTEPartenza(atep.IDATEPARTENZA, db);
                        if (rtep.idATEPartenza > 0)
                        {
                            rinunciaTEPartenza = rtep.rinunciaTE;
                        }
                    }

                    NumAttivazioniTEPartenza = dtte.GetNumAttivazioniTEPartenza(idTrasportoEffettiPartenza);
                }


                ViewData.Add("DescrizioneTE", DescrizioneTE);
                ViewData.Add("idTipoDocumento", idTipoDocumento);
                ViewData.Add("idTrasportoEffettiPartenza", idTrasportoEffettiPartenza);
                ViewData.Add("idStatoTrasferimento", idStatoTrasferimento);
                ViewData.Add("richiestaTEPartenza", richiestaTEPartenza);
                ViewData.Add("attivazioneTEPartenza", attivazioneTEPartenza);
                ViewData.Add("NumAttivazioniTEPartenza", NumAttivazioniTEPartenza);
                ViewData.Add("rinunciaTEPartenza", rinunciaTEPartenza);


                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        public ActionResult TabDocumentiTEPartenzaInseriti(decimal idTrasportoEffettiPartenza, decimal idTipoDocumento)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            string DescrizioneTE = "";

            try
            {

                using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                {
                    ldm = dtte.GetDocumentiTEPartenza(idTrasportoEffettiPartenza, idTipoDocumento);
                }


                using (dtDocumenti dtd = new dtDocumenti())
                {
                    DescrizioneTE = dtd.GetDescrizioneTipoDocumentoByIdTipoDocumento(idTipoDocumento);
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewData.Add("DescrizioneTE", DescrizioneTE);
            ViewData.Add("idTipoDocumento", idTipoDocumento);
            ViewData.Add("idTrasportoEffettiPartenza", idTrasportoEffettiPartenza);

            return PartialView(ldm);
        }

        public JsonResult GestionePulsantiNotificaAttivaAnnullaTEPartenza(decimal idTrasportoEffettiPartenza)
        {

            bool amministratore = false;
            string errore = "";
            bool richiestaTE = false;
            bool attivazioneTE = false;
            bool DocContributo = false;
            bool trasfAnnullato = false;
            bool rinunciaTE = false;

            try
            {
                amministratore = Utility.Amministratore();

                using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                {

                    dtte.SituazioneTEPartenza(idTrasportoEffettiPartenza,
                                            out richiestaTE,
                                            out attivazioneTE,
                                            out DocContributo,
                                            out trasfAnnullato,
                                            out rinunciaTE);
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
                        richiestaTE = richiestaTE,
                        attivazioneTE = attivazioneTE,
                        DocContributo = DocContributo,
                        trasfAnnullato = trasfAnnullato,
                        rinunciaTE = rinunciaTE,
                        err = errore
                    });

        }

        public ActionResult NuovoDocumentoTEPartenza(EnumTipoDoc idTipoDocumento, decimal idTrasportoEffettiPartenza)
        {
            try
            {
                string titoloPagina = string.Empty;

                using (dtDocumenti dtd = new dtDocumenti())
                {
                    titoloPagina = dtd.GetDescrizioneTipoDocumentoByIdTipoDocumento((decimal)idTipoDocumento);
                }

                ViewData.Add("titoloPagina", titoloPagina);
                ViewData.Add("idTipoDocumento", (decimal)idTipoDocumento);
                ViewData.Add("idTrasportoEffettiPartenza", idTrasportoEffettiPartenza);

                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }


        public JsonResult SalvaDocumentoTEPartenza(decimal idTipoDocumento, decimal idTrasportoEffettiPartenza)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    foreach (string item in Request.Files)
                    {

                        HttpPostedFileBase file = Request.Files[item] as HttpPostedFileBase;

                        using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                        {
                            using (dtDocumenti dtd = new dtDocumenti())
                            {
                                DocumentiModel dm = new DocumentiModel();
                                bool esisteFile = false;
                                bool gestisceEstensioni = false;
                                bool dimensioneConsentita = false;
                                string dimensioneMaxConsentita = string.Empty;

                                PreSetDocumentoTEPartenza(file, out dm, out esisteFile, out gestisceEstensioni,
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
                                        dtte.SetDocumentoTEPartenza(ref dm, idTrasportoEffettiPartenza, db, idTipoDocumento);

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

        public static void PreSetDocumentoTEPartenza(HttpPostedFileBase file, out DocumentiModel dm, out bool esisteFile, out bool gestisceEstensioni, out bool dimensioneConsentita, out string dimensioneMaxDocumento, decimal idTipoDocumento)
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

        public JsonResult EliminaDocumentoTEPartenza(decimal idDocumento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                    {
                        dtte.DeleteDocumentoTE(idDocumento);
                    }
                    db.Database.CurrentTransaction.Commit();
                    return Json(new { msg = "Il documento relativo al trasporto effetti (partenza) è stato eliminato." });
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    return Json(new { err = ex.Message });
                }
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ConfermaAnnullaRichiestaTEPartenza(decimal idTrasportoEffettiPartenza, string msg)
        {
            string errore = "";

            try
            {
                using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                {
                    decimal idAttivazione_notificata = dtte.GetUltimaAttivazioneTEPartenza(idTrasportoEffettiPartenza).IDATEPARTENZA;

                    dtte.AnnullaRichiestaTrasportoEffetti(idAttivazione_notificata, msg);
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

        public JsonResult ConfermaAttivaRichiestaTEPartenza(decimal idTrasportoEffettiPartenza)
        {
            string errore = "";

            try
            {
                using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                {
                    decimal idAttivazione = dtte.GetUltimaAttivazioneTEPartenza(idTrasportoEffettiPartenza).IDATEPARTENZA;

                    dtte.AttivaRichiestaTEPartenza(idAttivazione);
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

        public ActionResult MessaggioAnnullaTEPartenza(decimal idTrasportoEffettiPartenza)
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
                            var t = dtt.GetTrasferimentoByIdTEPartenza(idTrasportoEffettiPartenza);

                            if (t?.idTrasferimento > 0)
                            {
                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                var uff = dtu.GetUffici(t.idUfficio);

                                msg.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaTrasportoEffettiPartenzaAnticipo, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza);
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

        public ActionResult GestioneRinunciaTEPartenza(decimal idTrasportoEffettiPartenza)
        {
            RinunciaTEPartenzaModel rtepm = new RinunciaTEPartenzaModel();
            bool soloLettura = false;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                    {
                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            var atep = dtte.GetUltimaAttivazioneTEPartenza(idTrasportoEffettiPartenza);
                            if (atep.RICHIESTATRASPORTOEFFETTI == true || atep.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Saldo)
                            {
                                soloLettura = true;
                            }

                            rtepm = dtte.GetRinunciaTEPartenza(atep.IDATEPARTENZA, db);

                            EnumStatoTraferimento statoTrasferimento = 0;
                            var t = dtt.GetTrasferimentoByIdTEPartenza(idTrasportoEffettiPartenza);
                            statoTrasferimento = t.idStatoTrasferimento;
                            if (statoTrasferimento == EnumStatoTraferimento.Annullato || statoTrasferimento == EnumStatoTraferimento.Attivo)
                            {
                                soloLettura = true;
                            }

                            var n_att = dtte.GetNumAttivazioniTEPartenza(idTrasportoEffettiPartenza);

                            if (n_att > 0)
                            {
                                soloLettura = true;
                            }

                            ViewData.Add("soloLettura", soloLettura);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(rtepm);
        }

        public JsonResult AggiornaRinunciaTEPartenza(decimal idATEPartenza)
        {
            try
            {
                using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                {
                    dtte.Aggiorna_RinunciaTEPartenza(idATEPartenza);
                }
            }
            catch (Exception ex)
            {
                return Json(new { errore = ex.Message, msg = "" });
            }
            return Json(new { errore = "", msg = "Aggiornamento eseguito correttamente." });
        }


    }
}
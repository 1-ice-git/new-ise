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
    public class VariazioneTrasportoEffettiController : Controller
    {
        [HttpPost]
        public ActionResult VariazioneTE(decimal idTrasferimento)
        {
            try
            {
                using (dtVariazioneTrasportoEffetti dtvte = new dtVariazioneTrasportoEffetti())
                {
                    var idTEPartenza = dtvte.GetTEPartenzaByIdTrasferimento(idTrasferimento).idTEPartenza;
                    if (idTEPartenza > 0)
                    {
                        ViewData.Add("idTrasferimento", idTrasferimento);
                        ViewData.Add("idTEPartenza", idTEPartenza);
                    }

                    return PartialView();

                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }



        [HttpPost]
        public ActionResult VariazioneTEP(decimal idTEPartenza)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioneTrasportoEffetti dtvte = new dtVariazioneTrasportoEffetti())
                    {
                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            bool richiestaTE = false;
                            bool attivazioneTE = false;
                            bool DocAttestazione = false;
                            bool trasfAnnullato = false;
                            bool siAnticipo = false;
                            bool rinunciaTE = false;
                            decimal anticipoPercepito = 0;

                            VariazioneTEPartenzaModel vtepm = new VariazioneTEPartenzaModel();

                            var atep = dtvte.GetUltimaAttivazioneTEPartenza(idTEPartenza, db);

                            dtvte.SituazioneTEPartenza(idTEPartenza,
                                                        out richiestaTE,
                                                        out attivazioneTE,
                                                        out DocAttestazione,
                                                        out siAnticipo,
                                                        out anticipoPercepito,
                                                        out rinunciaTE,
                                                        out trasfAnnullato);

                            var tm = dtt.GetTrasferimentoByIdTEPartenza(idTEPartenza);

                            CalcoliIndennita ci = new CalcoliIndennita(tm.idTrasferimento, tm.dataPartenza);

                            vtepm.indennitaPrimaSistemazione = Math.Round(ci.IndennitaSistemazioneLorda, 2);
                            vtepm.percKM = ci.PercentualeFKMPartenza;
                            vtepm.contributoLordo = Math.Round(ci.TotaleContributoOmnicomprensivoPartenza, 2);
                            vtepm.anticipoPercepito = anticipoPercepito;
                            vtepm.saldo = Math.Round(vtepm.contributoLordo - vtepm.anticipoPercepito, 2);

                            //se ho rinunciato imposto la form con dati a zero e blocco l'inserimento di documenti
                            if (rinunciaTE)
                            {
                                siAnticipo = false;
                                vtepm.anticipoPercepito = 0;
                                vtepm.saldo = 0;
                            }

                            ////TEST (anche su gestione pulsanti)
                            //siAnticipo = true;
                            //if (siAnticipo)
                            //{
                            //    var PercentualeAnticipoTE = dtvte.GetPercentualeAnticipoTEPartenza(idTEPartenza, (decimal)EnumTipoAnticipoTE.Partenza);
                            //    var percAnticipo = PercentualeAnticipoTE.PERCENTUALE;
                            //    vtepm.anticipoPercepito = Math.Round(percAnticipo * vtepm.contributoLordo / 100, 2);
                            //    vtepm.saldo = Math.Round(vtepm.contributoLordo - vtepm.anticipoPercepito, 2);
                            //}
                            ////FINE TEST

                            vtepm.siAnticipo = siAnticipo;

                            ViewData.Add("richiestaTE", richiestaTE);
                            ViewData.Add("rinunciaTE", rinunciaTE);
                            ViewData.Add("attivazioneTE", attivazioneTE);
                            ViewData.Add("DocAttestazione", DocAttestazione);
                            ViewData.Add("idTEPartenza", idTEPartenza);

                            return PartialView(vtepm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

        }

        public JsonResult ConfermaNotificaRichiestaVariazioneTEP(decimal idTEPartenza)
        {
            string errore = "";

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioneTrasportoEffetti dtvte = new dtVariazioneTrasportoEffetti())
                    {
                        decimal idAttivitaTEPartenza = dtvte.GetUltimaAttivazioneTEPartenza(idTEPartenza, db).IDATEPARTENZA;
                        dtvte.NotificaRichiestaVariazioneTEP(idAttivitaTEPartenza, db);
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

        public ActionResult ElencoDocumentiVariazioneTEP(decimal idTipoDocumento, decimal idTEPartenza)
        {
            try
            {
                string DescrizioneTE = "";
                bool richiestaVariazioneTEPartenza = false;
                bool attivazioneVariazioneTEPartenza = false;
                //decimal NumAttivazioniTEPartenza = 0;
                decimal idStatoTrasferimento = 0;
                //bool rinunciaTEPartenza = false;

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var t = dtt.GetTrasferimentoByIdTEPartenza(idTEPartenza);
                    idStatoTrasferimento = (decimal)t.idStatoTrasferimento;
                }

                using (dtDocumenti dtd = new dtDocumenti())
                {
                    DescrizioneTE = dtd.GetDescrizioneTipoDocumentoByIdTipoDocumento(idTipoDocumento);
                }

                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioneTrasportoEffetti dtvte = new dtVariazioneTrasportoEffetti())
                    {
                        var atep = dtvte.GetUltimaAttivazioneTEPartenza(idTEPartenza, db);
                        if (atep.RICHIESTATRASPORTOEFFETTI && atep.ATTIVAZIONETRASPORTOEFFETTI == false)
                        {
                            richiestaVariazioneTEPartenza = true;
                        }
                        if (atep.RICHIESTATRASPORTOEFFETTI && atep.ATTIVAZIONETRASPORTOEFFETTI)
                        {
                            attivazioneVariazioneTEPartenza = true;
                            richiestaVariazioneTEPartenza = true;
                        }

                        //NumAttivazioniTEPartenza = dtvte.GetNumAttivazioniTEPartenza(idTEPartenza);
                    }
                }

                ViewData.Add("DescrizioneTE", DescrizioneTE);
                ViewData.Add("idTipoDocumento", idTipoDocumento);
                ViewData.Add("idTEPartenza", idTEPartenza);
                ViewData.Add("idStatoTrasferimento", idStatoTrasferimento);
                ViewData.Add("richiestaVariazioneTEPartenza", richiestaVariazioneTEPartenza);
                ViewData.Add("attivazioneVariazioneTEPartenza", attivazioneVariazioneTEPartenza);
                //ViewData.Add("NumAttivazioniTEPartenza", NumAttivazioniTEPartenza);


                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        public ActionResult TabDocumentiVariazioneTEPInseriti(decimal idTEPartenza, decimal idTipoDocumento)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            string DescrizioneTE = "";

            try
            {

                using (dtVariazioneTrasportoEffetti dtvte = new dtVariazioneTrasportoEffetti())
                {
                    ldm = dtvte.GetDocumentiTEPartenza(idTEPartenza, idTipoDocumento);
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
            ViewData.Add("idTEPartenza", idTEPartenza);

            return PartialView(ldm);
        }

        public JsonResult GestionePulsantiNotificaAttivaAnnullaVariazioneTEP(decimal idTEPartenza)
        {

            bool amministratore = false;
            string errore = "";
            bool richiestaTE = false;
            bool attivazioneTE = false;
            bool DocAttestazione = false;
            bool trasfAnnullato = false;
            bool siAnticipo = false;
            bool rinunciaTE = false;
            decimal anticipoPercepito = 0;

            try
            {
                amministratore = Utility.Amministratore();

                using (dtVariazioneTrasportoEffetti dtvte = new dtVariazioneTrasportoEffetti())
                {
                    dtvte.SituazioneTEPartenza(idTEPartenza,
                                            out richiestaTE,
                                            out attivazioneTE,
                                            out DocAttestazione,
                                            out siAnticipo,
                                            out anticipoPercepito,
                                            out rinunciaTE,
                                            out trasfAnnullato);

                    ////TEST
                    //siAnticipo = true;
                    ////
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
                        DocAttestazione = DocAttestazione,
                        trasfAnnullato = trasfAnnullato,
                        siAnticipo = siAnticipo,
                        rinunciaTE = rinunciaTE,
                        anticipoPercepito = anticipoPercepito,
                        err = errore
                    });

        }

        public ActionResult NuovoDocumentoVariazioneTEP(EnumTipoDoc idTipoDocumento, decimal idTEPartenza)
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
                ViewData.Add("idTEPartenza", idTEPartenza);

                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }


        public JsonResult SalvaDocumentoVariazioneTEP(decimal idTipoDocumento, decimal idTEPartenza)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    foreach (string item in Request.Files)
                    {

                        HttpPostedFileBase file = Request.Files[item] as HttpPostedFileBase;

                        using (dtVariazioneTrasportoEffetti dtvte = new dtVariazioneTrasportoEffetti())
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
                                        dtvte.SetDocumentoVariazioniTEP(ref dm, idTEPartenza, db, idTipoDocumento);

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

        public JsonResult EliminaDocumentoVariazioneTEP(decimal idDocumento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (dtVariazioneTrasportoEffetti dtvte = new dtVariazioneTrasportoEffetti())
                    {
                        dtvte.DeleteDocumentoVariazioneTE(idDocumento);
                    }
                    db.Database.CurrentTransaction.Commit();
                    return Json(new { msg = "Il documento relativo al saldo trasporto effetti (partenza) è stato eliminato." });
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
        public JsonResult ConfermaAnnullaRichiestaVariazioneTEP(decimal idTEPartenza, string msg)
        {
            string errore = "";

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioneTrasportoEffetti dtvte = new dtVariazioneTrasportoEffetti())
                    {
                        decimal idAttivazione_notificata = dtvte.GetUltimaAttivazioneTEPartenza(idTEPartenza, db).IDATEPARTENZA;

                        dtvte.AnnullaRichiestaVariazioneTEP(idAttivazione_notificata, msg, db);
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

        public JsonResult ConfermaAttivaRichiestaVariazioneTEP(decimal idTEPartenza)
        {
            string errore = "";

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioneTrasportoEffetti dtvte = new dtVariazioneTrasportoEffetti())
                    {
                        decimal idAttivazione = dtvte.GetUltimaAttivazioneTEPartenza(idTEPartenza, db).IDATEPARTENZA;

                        dtvte.AttivaRichiestaVariazioneTEP(idAttivazione, db);
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

        public ActionResult MessaggioAnnullaVariazioneTEP(decimal idTEPartenza)
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
                            var t = dtt.GetTrasferimentoByIdTEPartenza(idTEPartenza);

                            if (t?.idTrasferimento > 0)
                            {
                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                var uff = dtu.GetUffici(t.idUfficio);

                                msg.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaTrasportoEffettiPartenzaSaldo, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza);
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

        //public ActionResult GestioneRinunciaTEPartenza(decimal idTrasportoEffettiPartenza)
        //{
        //    RinunciaTEPartenzaModel rtepm = new RinunciaTEPartenzaModel();
        //    bool soloLettura = false;

        //    try
        //    {
        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
        //            {
        //                using (dtTrasferimento dtt = new dtTrasferimento())
        //                {
        //                    var atep = dtte.GetUltimaAttivazioneTEPartenza(idTrasportoEffettiPartenza);
        //                    if (atep.RICHIESTATRASPORTOEFFETTI == true || atep.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Saldo)
        //                    {
        //                        soloLettura = true;
        //                    }

        //                    rtepm = dtte.GetRinunciaTEPartenza(atep.IDATEPARTENZA, db);

        //                    EnumStatoTraferimento statoTrasferimento = 0;
        //                    var t = dtt.GetTrasferimentoByIdTEPartenza(idTrasportoEffettiPartenza);
        //                    statoTrasferimento = t.idStatoTrasferimento;
        //                    if (statoTrasferimento == EnumStatoTraferimento.Annullato || statoTrasferimento == EnumStatoTraferimento.Attivo)
        //                    {
        //                        soloLettura = true;
        //                    }

        //                    var n_att = dtte.GetNumAttivazioniTEPartenza(idTrasportoEffettiPartenza);

        //                    if (n_att > 0)
        //                    {
        //                        soloLettura = true;
        //                    }

        //                    ViewData.Add("soloLettura", soloLettura);
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
        //    }

        //    return PartialView(rtepm);
        //}

        //public JsonResult AggiornaRinunciaTEPartenza(decimal idATEPartenza)
        //{
        //    try
        //    {
        //        using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
        //        {
        //            dtte.Aggiorna_RinunciaTEPartenza(idATEPartenza);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { errore = ex.Message, msg = "" });
        //    }
        //    return Json(new { errore = "", msg = "Aggiornamento eseguito correttamente." });
        //}


    }
}
﻿using System.Web.Routing;
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
            using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
            {
                bool richiestaTE = false;
                bool attivazioneTE = false;
                bool DocContributo = false;
                bool DocAttestazione = false;
                decimal NumAttivazioni = 0;

                TrasportoEffettiPartenzaModel tepm = new TrasportoEffettiPartenzaModel();

                dtte.SituazioneTEPartenza(idTrasportoEffettiPartenza,
                               out richiestaTE, out attivazioneTE,
                               out DocContributo, out DocAttestazione, out NumAttivazioni);


                using (dtTrasferimento dtt = new dtTrasferimento())
                {

                    var tm = dtt.GetTrasferimentoByIdTEPartenza(idTrasportoEffettiPartenza);

                    CalcoliIndennita ci = new CalcoliIndennita(tm.idTrasferimento, tm.dataPartenza);

                    tepm.indennitaPrimaSistemazione =Math.Round(ci.indennitaSistemazioneLorda,2);
                    tepm.percKM = ci.percentualeFasciaKmTrasferimento;
                    tepm.contributoLordo = Math.Round(ci.contributoOmnicomprensivoTrasferimentoAnticipo,2);

                    ViewData.Add("richiestaTE", richiestaTE);
                    ViewData.Add("attivazioneTE", attivazioneTE);
                    ViewData.Add("DocContributo", DocContributo);
                    ViewData.Add("DocAttestazione", DocAttestazione);
                    ViewData.Add("idTrasportoEffettiPartenza", idTrasportoEffettiPartenza);
                }




                return PartialView(tepm);
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

                    NumAttivazioniTEPartenza = dtte.GetNumAttivazioniTEPartenza(idTrasportoEffettiPartenza);
                }


                ViewData.Add("DescrizioneTE", DescrizioneTE);
                ViewData.Add("idTipoDocumento", idTipoDocumento);
                ViewData.Add("idTrasportoEffettiPartenza", idTrasportoEffettiPartenza);
                ViewData.Add("idStatoTrasferimento", idStatoTrasferimento);
                ViewData.Add("richiestaTEPartenza", richiestaTEPartenza);
                ViewData.Add("attivazioneTEPartenza", attivazioneTEPartenza);
                ViewData.Add("NumAttivazioniTEPartenza", NumAttivazioniTEPartenza);


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
            bool DocAttestazione = false;
            decimal NumAttivazioni = 0;


            try
            {
                amministratore = Utility.Amministratore();

                using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                {

                    dtte.SituazioneTEPartenza(idTrasportoEffettiPartenza,
                                            out richiestaTE,
                                            out attivazioneTE,
                                            out DocContributo,
                                            out DocAttestazione,
                                            out NumAttivazioni);
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
                        DocAttestazione = DocAttestazione,
                        NumAttivazioni = NumAttivazioni,
                        err = errore
                    });

        }

        public ActionResult NuovoDocumentoTEPartenza(EnumTipoDoc idTipoDocumento, decimal idTrasportoEffettiPartenza)
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


        public ActionResult SalvaDocumentoTEPartenza(decimal idTipoDocumento, decimal idTrasportoEffettiPartenza)
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
        public JsonResult ConfermaAnnullaRichiestaTEPartenza(decimal idTrasportoEffettiPartenza)
        {
            string errore = "";

            try
            {
                using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                {
                    decimal idAttivazione_notificata = dtte.GetUltimaAttivazioneTEPartenza(idTrasportoEffettiPartenza).IDATEPARTENZA;

                    dtte.AnnullaRichiestaTrasportoEffetti(idAttivazione_notificata);
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


    }
}
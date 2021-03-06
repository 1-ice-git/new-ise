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
        public ActionResult TERientro(decimal idTrasferimento)
        {
            using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
            {
                ViewData.Add("idTERientro", idTrasferimento);
                ViewData.Add("idTrasferimento", idTrasferimento);

                return PartialView();
            }
        }


        [HttpPost]
        public ActionResult TrasportoEffettiPartenza(decimal idTrasportoEffettiPartenza)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
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

                            var t = dtt.GetTrasferimento(tm.idTrasferimento, db);
                        
                            //legge indennita PS su TEORICI
                            var lteorici = t.TEORICI.Where(x =>
                                           x.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
                                           //x.ELABORATO &&
                                           x.DIRETTO == false &&
                                           x.IDVOCI == (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131 &&
                                           x.INSERIMENTOMANUALE == false &&
                                           x.ANNULLATO == false &&
                                           x.ELABTRASPEFFETTI.CONGUAGLIO == false &&
                                           x.ELABTRASPEFFETTI.ANTICIPO &&
                                           x.ELABTRASPEFFETTI.IDTEPARTENZA>0 &&
                                           x.ANNORIFERIMENTO == t.DATAPARTENZA.Year &&
                                           x.MESERIFERIMENTO == t.DATAPARTENZA.Month)
                                       .ToList();

                            decimal indennitaPS = 0;
                            decimal percentualeFKMPartenza = 0;
                            decimal contributoLordo = 0;
                            decimal percentualeAnticipoTE = 0;
                            if (lteorici?.Any() ?? false)
                            {
                                var teorici = lteorici.First();
                                indennitaPS = teorici.IMPORTOLORDO;
                                percentualeFKMPartenza = teorici.ELABTRASPEFFETTI.PERCENTUALEFK;
                                contributoLordo = indennitaPS * percentualeFKMPartenza / 100;
                                percentualeAnticipoTE = teorici.ELABTRASPEFFETTI.PERCENTUALEANTICIPOSALDO;
                            }
                            else
                            {
                                CalcoliIndennita ci = new CalcoliIndennita(tm.idTrasferimento, tm.dataPartenza);

                                indennitaPS = ci.IndennitaSistemazioneLorda;
                                percentualeFKMPartenza = ci.PercentualeFKMPartenza;
                                //contributoLordo = ci.TotaleContributoOmnicomprensivoPartenza;
                                contributoLordo = indennitaPS * percentualeFKMPartenza / 100;
                                percentualeAnticipoTE = dtte.GetPercentualeAnticipoTEPartenza(idTrasportoEffettiPartenza, (decimal)EnumTipoAnticipoTE.Partenza).PERCENTUALE;
                            }



                            tepm.indennitaPrimaSistemazione = Math.Round(indennitaPS, 2);
                            tepm.percKM = percentualeFKMPartenza;
                            tepm.contributoLordo = Math.Round(contributoLordo, 2);
                            tepm.percAnticipo = percentualeAnticipoTE;
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
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

        }

        [HttpPost]
        public ActionResult TrasportoEffettiRientro(decimal idTERientro)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                    {
                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            bool richiestaTER = false;
                            bool attivazioneTER = false;
                            bool DocContributo = false;
                            bool trasfAnnullato = false;
                            bool rinunciaTERientro = false;

                            TrasportoEffettiRientroModel term = new TrasportoEffettiRientroModel();

                            var atep = dtte.GetUltimaAttivazioneTERientro(idTERientro);

                            dtte.SituazioneTERientro(idTERientro,
                                                        out richiestaTER, out attivazioneTER,
                                                        out DocContributo,
                                                        out trasfAnnullato, out rinunciaTERientro);

                            var tm = dtt.GetTrasferimentoByIdTERientro(idTERientro);

                            var t = dtt.GetTrasferimento(tm.idTrasferimento, db);

                            //legge indennita PS su TEORICI
                            var lteorici = t.TEORICI.Where(x =>
                                           x.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
                                           //x.ELABORATO &&
                                           x.DIRETTO == false &&
                                           x.IDVOCI == (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131 &&
                                           x.INSERIMENTOMANUALE == false &&
                                           x.ANNULLATO == false &&
                                           x.ELABTRASPEFFETTI.CONGUAGLIO == false &&
                                           x.ELABTRASPEFFETTI.ANTICIPO &&
                                           x.ELABTRASPEFFETTI.IDTERIENTRO > 0 &&
                                           x.ANNORIFERIMENTO == t.DATARIENTRO.Year &&
                                           x.MESERIFERIMENTO == t.DATARIENTRO.Month)
                                       .ToList();

                            decimal indennitaRichiamo = 0;
                            decimal percentualeFKMRientro = 0;
                            decimal percentualeAnticipoTE = 0;
                            decimal contributoLordo = 0;

                            if (lteorici?.Any() ?? false)
                            {
                                var teorici = lteorici.First();
                                indennitaRichiamo = teorici.IMPORTOLORDO;
                                percentualeFKMRientro = teorici.ELABTRASPEFFETTI.PERCENTUALEFK;
                                contributoLordo = indennitaRichiamo * percentualeFKMRientro / 100;
                                percentualeAnticipoTE = teorici.ELABTRASPEFFETTI.PERCENTUALEANTICIPOSALDO;
                            }
                            else
                            {
                                CalcoliIndennita ci = new CalcoliIndennita(tm.idTrasferimento, tm.dataRientro);

                                indennitaRichiamo = ci.IndennitaRichiamoLordo;
                                percentualeFKMRientro = ci.PercentualeFKMRientro;
                                contributoLordo = indennitaRichiamo * percentualeFKMRientro / 100;
                                percentualeAnticipoTE = dtte.GetPercentualeAnticipoTERientro(idTERientro, (decimal)EnumTipoAnticipoTE.Rientro).PERCENTUALE;
                            }

                            term.indennitaRichiamo = Math.Round(indennitaRichiamo, 2);
                            term.percKM = percentualeFKMRientro;
                            term.contributoLordo = Math.Round(contributoLordo, 2);
                            term.percAnticipo = percentualeAnticipoTE;
                            term.anticipo = Math.Round(term.percAnticipo * term.contributoLordo / 100, 2);

                            ViewData.Add("rinunciaTERientro", rinunciaTERientro);
                            ViewData.Add("richiestaTER", richiestaTER);
                            ViewData.Add("attivazioneTER", attivazioneTER);
                            ViewData.Add("DocContributo", DocContributo);
                            ViewData.Add("idTERientro", idTERientro);

                            return PartialView(term);
                        }
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

        public JsonResult ConfermaNotificaRichiestaTERientro(decimal idTERientro)
        {
            string errore = "";

            try
            {
                using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                {
                    decimal idAttivitaTERientro = dtte.GetUltimaAttivazioneTERientro(idTERientro).IDATERIENTRO;
                    dtte.NotificaRichiestaTERientro(idAttivitaTERientro);
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

        public ActionResult ElencoDocumentiTERientro(decimal idTipoDocumento, decimal idTERientro)
        {
            try
            {
                string DescrizioneTER = "";
                bool richiestaTERientro = false;
                bool attivazioneTERientro = false;
                decimal NumAttivazioniTERientro = 0;
                decimal idStatoTrasferimento = 0;
                bool rinunciaTERientro = false;

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var t = dtt.GetTrasferimentoByIdTERientro(idTERientro);
                    idStatoTrasferimento = (decimal)t.idStatoTrasferimento;
                }

                using (dtDocumenti dtd = new dtDocumenti())
                {
                    DescrizioneTER = dtd.GetDescrizioneTipoDocumentoByIdTipoDocumento(idTipoDocumento);
                }

                using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                {
                    var ater = dtte.GetUltimaAttivazioneTERientro(idTERientro);
                    if (ater.RICHIESTATRASPORTOEFFETTI && ater.ATTIVAZIONETRASPORTOEFFETTI == false)
                    {
                        richiestaTERientro = true;
                    }
                    if (ater.RICHIESTATRASPORTOEFFETTI && ater.ATTIVAZIONETRASPORTOEFFETTI)
                    {
                        attivazioneTERientro = true;
                        richiestaTERientro = true;
                    }

                    using (ModelDBISE db = new ModelDBISE())
                    {
                        var rter = dtte.GetRinunciaTERientro(ater.IDATERIENTRO, db);
                        if (rter.idATERientro > 0)
                        {
                            rinunciaTERientro = rter.rinunciaTE;
                        }
                    }

                    NumAttivazioniTERientro = dtte.GetNumAttivazioniTERientro(idTERientro);
                }


                ViewData.Add("DescrizioneTER", DescrizioneTER);
                ViewData.Add("idTipoDocumento", idTipoDocumento);
                ViewData.Add("idTERientro", idTERientro);
                ViewData.Add("idStatoTrasferimento", idStatoTrasferimento);
                ViewData.Add("richiestaTERientro", richiestaTERientro);
                ViewData.Add("attivazioneTERientro", attivazioneTERientro);
                ViewData.Add("NumAttivazioniTERientro", NumAttivazioniTERientro);
                ViewData.Add("rinunciaTERientro", rinunciaTERientro);


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

        public ActionResult TabDocumentiTERientroInseriti(decimal idTERientro, decimal idTipoDocumento)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            string DescrizioneTER = "";

            try
            {

                using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                {
                    ldm = dtte.GetDocumentiTERientro(idTERientro, idTipoDocumento);
                }


                using (dtDocumenti dtd = new dtDocumenti())
                {
                    DescrizioneTER = dtd.GetDescrizioneTipoDocumentoByIdTipoDocumento(idTipoDocumento);
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewData.Add("DescrizioneTER", DescrizioneTER);
            ViewData.Add("idTipoDocumento", idTipoDocumento);
            ViewData.Add("idTERientro", idTERientro);

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

        public JsonResult GestionePulsantiNotificaAttivaAnnullaTERientro(decimal idTERientro)
        {

            bool amministratore = false;
            string errore = "";
            bool richiestaTER = false;
            bool attivazioneTER = false;
            bool DocContributo = false;
            bool trasfAnnullato = false;
            bool rinunciaTER = false;

            try
            {
                amministratore = Utility.Amministratore();

                using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                {

                    dtte.SituazioneTERientro(idTERientro,
                                            out richiestaTER,
                                            out attivazioneTER,
                                            out DocContributo,
                                            out trasfAnnullato,
                                            out rinunciaTER);
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
                        richiestaTER = richiestaTER,
                        attivazioneTER = attivazioneTER,
                        DocContributo = DocContributo,
                        trasfAnnullato = trasfAnnullato,
                        rinunciaTER = rinunciaTER,
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

        public ActionResult NuovoDocumentoTERientro(EnumTipoDoc idTipoDocumento, decimal idTERientro)
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
                ViewData.Add("idTERientro", idTERientro);

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

        public JsonResult SalvaDocumentoTERientro(decimal idTipoDocumento, decimal idTERientro)
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

                                PreSetDocumentoTERientro(file, out dm, out esisteFile, out gestisceEstensioni,
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
                                        dtte.SetDocumentoTERientro(ref dm, idTERientro, db, idTipoDocumento);

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

        public static void PreSetDocumentoTERientro(HttpPostedFileBase file, out DocumentiModel dm, out bool esisteFile, out bool gestisceEstensioni, out bool dimensioneConsentita, out string dimensioneMaxDocumento, decimal idTipoDocumento)
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

        public JsonResult EliminaDocumentoTERientro(decimal idDocumento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                    {
                        dtte.DeleteDocumentoTER(idDocumento);
                    }
                    db.Database.CurrentTransaction.Commit();
                    return Json(new { msg = "Il documento relativo al trasporto effetti (rientro) è stato eliminato." });
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

                    dtte.AnnullaRichiestaTrasportoEffettiPartenza(idAttivazione_notificata, msg);
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
        public JsonResult ConfermaAnnullaRichiestaTERientro(decimal idTERientro, string msg)
        {
            string errore = "";

            try
            {
                using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                {
                    decimal idAttivazione_notificata = dtte.GetUltimaAttivazioneTERientro(idTERientro).IDATERIENTRO;

                    dtte.AnnullaRichiestaTrasportoEffettiRientro(idAttivazione_notificata, msg);
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

        public JsonResult ConfermaAttivaRichiestaTERientro(decimal idTERientro)
        {
            string errore = "";

            try
            {
                using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                {
                    decimal idAttivazione = dtte.GetUltimaAttivazioneTERientro(idTERientro).IDATERIENTRO;

                    dtte.AttivaRichiestaTERientro(idAttivazione);
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

        public ActionResult MessaggioAnnullaTERientro(decimal idTERientro)
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
                            var t = dtt.GetTrasferimentoByIdTERientro(idTERientro);

                            if (t?.idTrasferimento > 0)
                            {
                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                var uff = dtu.GetUffici(t.idUfficio);

                                msg.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaTrasportoEffettiRientroAnticipo, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza);
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

        public ActionResult GestioneRinunciaTERientro(decimal idTERientro)
        {
            RinunciaTERientroModel rterm = new RinunciaTERientroModel();
            bool soloLettura = false;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                    {
                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            var ater = dtte.GetUltimaAttivazioneTERientro(idTERientro);
                            if (ater.RICHIESTATRASPORTOEFFETTI == true || ater.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Saldo)
                            {
                                soloLettura = true;
                            }

                            rterm = dtte.GetRinunciaTERientro(ater.IDATERIENTRO, db);

                            EnumStatoTraferimento statoTrasferimento = 0;
                            var t = dtt.GetTrasferimentoByIdTERientro(idTERientro);
                            statoTrasferimento = t.idStatoTrasferimento;
                            if (statoTrasferimento == EnumStatoTraferimento.Annullato || statoTrasferimento == EnumStatoTraferimento.Attivo)
                            {
                                soloLettura = true;
                            }

                            var n_att = dtte.GetNumAttivazioniTERientro(idTERientro);

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

            return PartialView(rterm);
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

        public JsonResult AggiornaRinunciaTERientro(decimal idATERientro)
        {
            try
            {
                using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                {
                    dtte.Aggiorna_RinunciaTERientro(idATERientro);
                }
            }
            catch (Exception ex)
            {
                return Json(new { errore = ex.Message, msg = "" });
            }
            return Json(new { errore = "", msg = "Aggiornamento eseguito correttamente." });
        }

        public JsonResult VerificaTERientroAnticipo(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            decimal tmp = 0;
            try
            {
                if (idTrasferimento <= 0)
                {
                    throw new Exception("Trasferimento non valorizzato");
                }
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    using (dtRichiamo dtr = new dtRichiamo())
                    {
                        TrasferimentoModel trm = dtt.GetTrasferimentoById(idTrasferimento);
                        if (trm != null)
                        {
                            if (trm.idStatoTrasferimento == EnumStatoTraferimento.Attivo || trm.idStatoTrasferimento == EnumStatoTraferimento.Terminato)
                            {
                                if (dtr.GetRichiamoByIdTrasf(idTrasferimento).idTrasferimento > 0)
                                {
                                    tmp = 1;
                                }
                            }
    
                        }
                    }
                }
                return Json(new { VerificaTERientroAnticipo = tmp });
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }



    }
}
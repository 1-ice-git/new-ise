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

namespace NewISE.Controllers
{
    public class MaggiorazioneAbitazioneController : Controller
    {

        [HttpPost]
        public ActionResult MaggiorazioneAbitazione(decimal idTrasferimento)
        {
            ViewData.Add("idTrasferimento", idTrasferimento);

            return PartialView();
        }

        public ActionResult ElencoDocumentiFormularioMAB()
        {
            return PartialView();
        }


        public ActionResult AttivitaMAB(decimal idTrasferimento)
        {
            List<MaggiorazioneAbitazioneViewModel> mavml = new List<MaggiorazioneAbitazioneViewModel>();
            MaggiorazioneAbitazioneViewModel mavm = new MaggiorazioneAbitazioneViewModel();
            MaggiorazioneAbitazioneModel mam = new MaggiorazioneAbitazioneModel();

            try
            {
                using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                {
                    bool soloLettura = false;
                    bool siDati = false;
                    EnumStatoTraferimento statoTrasferimento = 0;


                    AttivazioneMABModel amm = dtma.GetAttivazioneMAB(idTrasferimento);

                    if (amm != null && amm.idAttivazioneMAB > 0)
                    {
                        if (amm.notificaRichiesta)
                        {
                            soloLettura = true;
                        }

                        mam = dtma.GetMaggiorazioneAbitazione(amm);

                        if (mam != null && mam.idMAB > 0)
                        {
                            mavm.idAttivazioneMAB = mam.idAttivazioneMAB;
                            mavm.idMAB = mam.idMAB;

                            CANONEMAB cm = dtma.GetCanoneMAB(mam);

                            mavm.importo_canone = cm.IMPORTOCANONE;
                            mavm.dataInizioMAB = mam.dataInizioMAB;
                            mavm.dataFineMAB = mam.dataFineMAB;
                            mavm.AnticipoAnnuale = mam.AnticipoAnnuale;

                            if (cm.IDCANONE > 0)
                            {
                                using (dtValute dtv = new dtValute())
                                {
                                    var v = dtv.GetValutaByIdCanone(cm.IDCANONE);

                                    mavm.descrizioneValuta = v.descrizioneValuta;
                                    mavm.id_Valuta = v.idValuta;
                                }
                            }

                            var lpc = dtma.GetListPagatoCondivisoMAB(mavm);

                            if (lpc.Count() > 0)
                            {
                                var pc = lpc.First();
                                mavm.canone_pagato = pc.PAGATO;
                                mavm.canone_condiviso = pc.CONDIVISO;
                            }

                            mavml.Add(mavm);

                            siDati = true;
                        }

                    }

                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var t = dtt.GetTrasferimentoById(idTrasferimento);
                        statoTrasferimento = t.idStatoTrasferimento;
                        if (statoTrasferimento == EnumStatoTraferimento.Annullato)
                        {
                            soloLettura = true;
                        }
                    }

                    ViewData.Add("soloLettura", soloLettura);
                    ViewData.Add("siDati", siDati);
                    ViewData.Add("idAttivazioneMAB", amm.idAttivazioneMAB);
                    ViewData.Add("idMAB", mam.idMAB);
                    ViewData.Add("idTrasferimento", idTrasferimento);
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(mavml);
        }

        public ActionResult GestioneMAB(decimal idTrasferimento)
        {
            try
            {
                ViewData.Add("idTrasferimento", idTrasferimento);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }


        public ActionResult FormulariMAB(decimal idTrasferimento)
        {
            bool siDocModulo1 = false;
            bool siDocModulo2 = false;
            bool siDocModulo3 = false;
            bool siDocModulo4 = false;
            bool siDocModulo5 = false;
            bool siDocCopiaContratto = false;
            bool siDocCopiaRicevuta = false;
            decimal idDocModulo1 = 0;
            decimal idDocModulo2 = 0;
            decimal idDocModulo3 = 0;
            decimal idDocModulo4 = 0;
            decimal idDocModulo5 = 0;
            decimal idDocCopiaContratto = 0;
            decimal idDocCopiaRicevuta = 0;
            EnumStatoTraferimento statoTrasferimento = 0;


            MaggiorazioneAbitazioneModel mam = new MaggiorazioneAbitazioneModel();

            try
            {
                using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                {
                    bool soloLettura = false;

                    AttivazioneMABModel amm = dtma.GetAttivazioneMAB(idTrasferimento);

                    if (amm != null && amm.idAttivazioneMAB > 0)
                    {
                        dtma.VerificaDocumentiPartenza(amm, out siDocCopiaContratto,
                                                            out siDocCopiaRicevuta,
                                                            out siDocModulo1,
                                                            out siDocModulo2,
                                                            out siDocModulo3,
                                                            out siDocModulo4,
                                                            out siDocModulo5,
                                                            out idDocCopiaContratto,
                                                            out idDocCopiaRicevuta,
                                                            out idDocModulo1,
                                                            out idDocModulo2,
                                                            out idDocModulo3,
                                                            out idDocModulo4,
                                                            out idDocModulo5);

                        if (amm.notificaRichiesta)
                        {
                            soloLettura = true;
                        }

                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            var t = dtt.GetTrasferimentoById(idTrasferimento);
                            statoTrasferimento = t.idStatoTrasferimento;
                            if (statoTrasferimento == EnumStatoTraferimento.Annullato)
                            {
                                soloLettura = true;
                            }
                        }

                    }
                    decimal NumAttivazioni = dtma.GetNumAttivazioniMAB(idTrasferimento);

                    ViewData.Add("idTrasferimento", idTrasferimento);
                    ViewData.Add("siDocCopiaContratto", siDocCopiaContratto);
                    ViewData.Add("siDocCopiaRicevuta", siDocCopiaRicevuta);
                    ViewData.Add("siDocModulo1", siDocModulo1);
                    ViewData.Add("siDocModulo2", siDocModulo2);
                    ViewData.Add("siDocModulo3", siDocModulo3);
                    ViewData.Add("siDocModulo4", siDocModulo4);
                    ViewData.Add("siDocModulo5", siDocModulo5);
                    ViewData.Add("idDocCopiaContratto", idDocCopiaContratto);
                    ViewData.Add("idDocCopiaRicevuta", idDocCopiaRicevuta);
                    ViewData.Add("idDocModulo1", idDocModulo1);
                    ViewData.Add("idDocModulo2", idDocModulo2);
                    ViewData.Add("idDocModulo3", idDocModulo3);
                    ViewData.Add("idDocModulo4", idDocModulo4);
                    ViewData.Add("idDocModulo5", idDocModulo5);
                    ViewData.Add("soloLettura", soloLettura);
                    ViewData.Add("NumAttivazioni", NumAttivazioni);
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }


        public ActionResult GestionePulsantiMAB(decimal idTrasferimento)
        {
            AttivazioneMABModel amm = new AttivazioneMABModel();
            MaggiorazioneAbitazioneModel mam = new MaggiorazioneAbitazioneModel();

            try
            {
                bool amministratore = Utility.Amministratore();

                string disabledNotificaRichiesta = "disabled";
                string hiddenNotificaRichiesta = "";
                string disabledAttivaRichiesta = "disabled";
                string hiddenAttivaRichiesta = "hidden";
                string disabledAnnullaRichiesta = "disabled";
                string hiddenAnnullaRichiesta = "hidden";
                decimal num_attivazioni = 0;
                bool esisteMod1 = false;
                EnumStatoTraferimento statoTrasferimento = 0;

                using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                {
                    amm = dtma.GetUltimaAttivazioneMAB(idTrasferimento);
                    num_attivazioni = dtma.GetNumAttivazioniMAB(idTrasferimento);
                    mam = dtma.GetMaggiorazioneAbitazione(amm);
                    var ldocModulo1 = dtma.GetDocumentiMABbyTipoDoc(amm.idAttivazioneMAB, (decimal)EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione);
                    if (ldocModulo1.Count > 0)
                    {
                        esisteMod1 = true;
                    }
                }
                var idAttivazioneMAB = amm.idAttivazioneMAB;

                bool esisteMAB = mam.idMAB > 0 ? true : false;

                bool notificaRichiesta = amm.notificaRichiesta;
                bool attivaRichiesta = amm.Attivazione;

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var t = dtt.GetTrasferimentoById(idTrasferimento);
                    statoTrasferimento = t.idStatoTrasferimento;
                }

                //se non esiste nessuma MAB non esegue nessun controllo
                if (esisteMAB)
                {
                    //se amministratore vedo i pulsanti altrimenti solo notifica
                    if (amministratore)
                    {
                        hiddenAttivaRichiesta = "";
                        hiddenAnnullaRichiesta = "";

                        if (num_attivazioni == 0)
                        {
                            if (notificaRichiesta && attivaRichiesta == false && esisteMod1 && statoTrasferimento != EnumStatoTraferimento.Annullato)
                            {
                                disabledAttivaRichiesta = "";
                                disabledAnnullaRichiesta = "";
                            }
                            if (notificaRichiesta == false && attivaRichiesta == false && esisteMod1 && statoTrasferimento != EnumStatoTraferimento.Annullato)
                            {
                                disabledNotificaRichiesta = "";
                            }
                        }
                    }
                    else
                    {
                        if (num_attivazioni == 0)
                        {
                            if (notificaRichiesta == false && attivaRichiesta == false && esisteMod1 && statoTrasferimento != EnumStatoTraferimento.Annullato)
                            {
                                disabledNotificaRichiesta = "";
                            }
                        }
                    }
                }
                else
                {
                    if (amministratore)
                    {
                        hiddenAttivaRichiesta = "";
                        hiddenAnnullaRichiesta = "";
                    }
                }

                ViewData.Add("idTrasferimento", idTrasferimento);
                ViewData.Add("idAttivazioneMAB", idAttivazioneMAB);
                ViewData.Add("disabledAnnullaRichiesta", disabledAnnullaRichiesta);
                ViewData.Add("disabledAttivaRichiesta", disabledAttivaRichiesta);
                ViewData.Add("disabledNotificaRichiesta", disabledNotificaRichiesta);
                ViewData.Add("hiddenAnnullaRichiesta", hiddenAnnullaRichiesta);
                ViewData.Add("hiddenAttivaRichiesta", hiddenAttivaRichiesta);
                ViewData.Add("hiddenNotificaRichiesta", hiddenNotificaRichiesta);
                ViewData.Add("amministratore", amministratore);

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(amm);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DocumentoMAB(decimal idTipoDocumento, decimal idTrasferimento)
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

                        ViewData.Add("idTipoDocumento", idTipoDocumento);
                        ViewData.Add("DescDocumento", DescDocumento);

                        return PartialView(trm);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult InserisciDocumentoMAB(decimal idTrasferimento, decimal idTipoDocumento, HttpPostedFileBase file)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                    {
                        //leggo la prima attivazione
                        AttivazioneMABModel amm = dtma.GetAttivazioneMAB(idTrasferimento);

                        //se non esiste la creo
                        if ((amm != null && amm.idAttivazioneMAB > 0) == false)
                        {
                            ATTIVAZIONEMAB am = dtma.CreaAttivazioneMAB(idTrasferimento, db);
                            amm.idAttivazioneMAB = am.IDATTIVAZIONEMAB;
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
                                    decimal idDocumentoEsistente = dtma.VerificaEsistenzaDocumentoMAB(idTrasferimento, (EnumTipoDoc)idTipoDocumento);

                                    if (idDocumentoEsistente > 0)
                                    {
                                        //se già esiste lo sostituisco (imposto modificato=true su quello esistente e ne inserisco una altro)
                                        dtma.SostituisciDocumentoMAB(ref dm, idDocumentoEsistente, amm.idAttivazioneMAB, db);

                                    }
                                    else
                                    {
                                        //se non esiste lo inserisco
                                        dtma.SetDocumentoMAB(ref dm, amm.idAttivazioneMAB, db);
                                    }

                                    dtma.AssociaDocumentoAttivazione(amm.idAttivazioneMAB, dm.idDocumenti, db);
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

        public JsonResult ConfermaAttivaRichiestaMAB(decimal idAttivazioneMAB)
        {
            string errore = "";

            try
            {
                using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                {


                    dtma.AttivaRichiestaMAB(idAttivazioneMAB);
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
        public JsonResult ConfermaAnnullaRichiestaMAB(decimal idAttivazioneMAB, string msg)
        {
            string errore = "";

            try
            {
                using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                {
                    dtma.AnnullaRichiestaMAB(idAttivazioneMAB, msg);
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


        public JsonResult ConfermaNotificaRichiestaMAB(decimal idAttivazioneMAB)
        {
            string errore = "";

            try
            {
                using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                {

                    dtma.NotificaRichiestaMAB(idAttivazioneMAB);
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

        public ActionResult NuovaMAB(decimal idTrasferimento)
        {
            MaggiorazioneAbitazioneViewModel mam = new MaggiorazioneAbitazioneViewModel();

            List<SelectListItem> lValute = new List<SelectListItem>();

            var r = new List<SelectListItem>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                    {
                        using (dtValute dtv = new dtValute())
                        {
                            var lv = dtv.GetElencoValute(db);

                            if (lv != null && lv.Count > 0)
                            {
                                r = (from v in lv
                                     select new SelectListItem()
                                     {
                                         Text = v.DESCRIZIONEVALUTA,
                                         Value = v.IDVALUTA.ToString()
                                     }).ToList();

                                r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                            }

                            lValute = r;
                            ViewBag.lValute = lValute;

                            var vm = dtv.GetValutaUfficiale(db);

                            mam.id_Valuta = vm.idValuta;
                        }
                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            var t = dtt.GetTrasferimentoById(idTrasferimento);
                            mam.dataInizioMAB = t.dataPartenza;
                            mam.idTrasferimento = idTrasferimento;
                            mam.dataPartenza = t.dataPartenza;
                        }
                        mam.dataFineMAB = Utility.DataFineStop();
                        mam.ut_dataFineMAB = null;
                        mam.importo_canone = 0;

                        mam.AnticipoAnnuale = false;
                        var mann = dtma.GetMaggiorazioneAnnuale(mam, db);
                        mam.idMagAnnuali = mann.idMagAnnuali;

                        ViewData.Add("idTrasferimento", idTrasferimento);
                        ViewBag.lValute = lValute;

                        return PartialView(mam);
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

        }


        public ActionResult ModificaMAB(decimal idMAB)
        {
            MaggiorazioneAbitazioneViewModel mam = new MaggiorazioneAbitazioneViewModel();

            List<SelectListItem> lValute = new List<SelectListItem>();

            var r = new List<SelectListItem>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                    {
                        using (dtValute dtv = new dtValute())
                        {
                            using (dtTrasferimento dtt = new dtTrasferimento())
                            {

                                var lv = dtv.GetElencoValute(db);

                                if (lv != null && lv.Count > 0)
                                {
                                    r = (from v in lv
                                         select new SelectListItem()
                                         {
                                             Text = v.DESCRIZIONEVALUTA,
                                             Value = v.IDVALUTA.ToString()
                                         }).ToList();

                                    r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                                }

                                lValute = r;
                                ViewBag.lValute = lValute;

                                var ma = dtma.GetMaggiorazioneAbitazioneByID(idMAB);
                                var t = dtt.GetTrasferimentoById(ma.IDTRASFERIMENTO);
                                mam.idMAB = ma.IDMAB;
                                mam.dataPartenza = t.dataPartenza;
                                mam.idTrasferimento = ma.IDTRASFERIMENTO;
                                mam.idAttivazioneMAB = ma.IDATTIVAZIONEMAB;

                                mam.dataInizioMAB = ma.DATAINIZIOMAB;
                                mam.dataFineMAB = ma.DATAFINEMAB;
                                mam.ut_dataFineMAB = ma.DATAFINEMAB;
                                mam.AnticipoAnnuale = ma.ANTICIPOANNUALE;

                                mam.idMagAnnuali = 0;
                                var mann = dtma.GetMaggiorazioneAnnuale(mam, db);
                                if (mann.idMagAnnuali > 0)
                                {
                                    mam.idMagAnnuali = mann.idMagAnnuali;
                                }

                                var lpc = dtma.GetListPagatoCondivisoMAB(mam);

                                if (lpc.Count() > 0)
                                {
                                    var pc = lpc.First();
                                    mam.canone_condiviso = pc.CONDIVISO;
                                    mam.canone_pagato = pc.PAGATO;

                                    //#region inserisce eventuale pagato condiviso
                                    //if (mam.canone_condiviso)
                                    //{
                                    //    //PAGATOCONDIVISOMAB pc = dtma.SetPagatoCondivisoMAB(mvm, db);

                                    //    pc.PAGATO = mam.canone_pagato;
                                    //    pc.CONDIVISO = mam.canone_condiviso;
                                    //    pc.DATAINIZIOVALIDITA = mam.dataInizioMAB;
                                    //    pc.DATAFINEVALIDITA = mam.dataFineMAB;
                                    //    pc.DATAAGGIORNAMENTO = DateTime.Now;

                                    //    if (db.SaveChanges() > 0)
                                    //    {

                                    //        #region associa percentuale condivisione
                                    //        var lpercCond = dtma.GetListaPercentualeCondivisione(pc.DATAINIZIOVALIDITA, pc.DATAFINEVALIDITA, db);
                                    //        if (lpercCond?.Any() ?? false)
                                    //        {
                                    //            //rimuovo precedenti associazioni percentuali
                                    //            dtma.RimuoviAssociazionePagatoCondiviso_PercentualeCondivisione(pc.IDPAGATOCONDIVISO, db);

                                    //            //riassocio le percentuali
                                    //            foreach (var percCond in lpercCond)
                                    //            {
                                    //                dtma.Associa_PagatoCondivisoMAB_PercentualeCondivisione(pc.IDPAGATOCONDIVISO, percCond.IDPERCCOND, db);
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            throw new Exception("Non è stata trovata la percentuale condivisione della maggiorazione abitazione per il periodo richiesto.");
                                    //        }
                                    //        #endregion
                                    //    }
                                    //    else
                                    //    {
                                    //        throw new Exception("Errore nell'aggiornamento di PAgatoCondivisoMAB.");
                                    //    }


                                    //}
                                    //#endregion
                                }

                                if (ma.DATAFINEMAB == Utility.DataFineStop())
                                {
                                    mam.ut_dataFineMAB = null;
                                }


                                var canone = dtma.GetCanoneMAB(mam);

                                if (canone.IDCANONE > 0)
                                {
                                    mam.importo_canone = canone.IMPORTOCANONE;
                                    var v = dtv.GetValutaByIdCanone(canone.IDCANONE);
                                    mam.id_Valuta = v.idValuta;
                                }

                                ViewData.Add("idMAB", idMAB);
                                ViewData.Add("idTrasferimento", mam.idTrasferimento);
                                ViewBag.lValute = lValute;

                                return PartialView(mam);
                            }
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
        [ValidateAntiForgeryToken]
        public ActionResult ConfermaNuovaMAB(MaggiorazioneAbitazioneViewModel mvm, decimal idTrasferimento)
        {
            MaggiorazioneAbitazioneViewModel mam = new MaggiorazioneAbitazioneViewModel();

            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                        {
                            dtma.InserisciMAB(mvm, idTrasferimento);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.Message);

                        List<SelectListItem> lValute = new List<SelectListItem>();

                        var r = new List<SelectListItem>();

                        using (ModelDBISE db = new ModelDBISE())
                        {
                            using (dtValute dtv = new dtValute())
                            {
                                var lv = dtv.GetElencoValute(db);

                                if (lv != null && lv.Count > 0)
                                {
                                    r = (from v in lv
                                         select new SelectListItem()
                                         {
                                             Text = v.DESCRIZIONEVALUTA,
                                             Value = v.IDVALUTA.ToString()
                                         }).ToList();

                                    r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                                }

                                lValute = r;
                                ViewBag.lValute = lValute;
                                ViewData.Add("idTrasferimento", idTrasferimento);

                                return PartialView("NuovaMAB", mvm);

                            }
                        }
                    }
                }
                else
                {
                    List<SelectListItem> lValute = new List<SelectListItem>();

                    var r = new List<SelectListItem>();

                    using (ModelDBISE db = new ModelDBISE())
                    {
                        using (dtValute dtv = new dtValute())
                        {
                            var lv = dtv.GetElencoValute(db);

                            if (lv != null && lv.Count > 0)
                            {
                                r = (from v in lv
                                     select new SelectListItem()
                                     {
                                         Text = v.DESCRIZIONEVALUTA,
                                         Value = v.IDVALUTA.ToString()
                                     }).ToList();

                                r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                            }

                            lValute = r;
                            ViewBag.lValute = lValute;

                            ViewData.Add("idTrasferimento", idTrasferimento);

                            return PartialView("NuovaMAB", mvm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return RedirectToAction("GestioneMAB", new { idTrasferimento = idTrasferimento });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfermaModificaMAB(MaggiorazioneAbitazioneViewModel mvm, decimal idTrasferimento, decimal idMAB)
        {
            MaggiorazioneAbitazioneViewModel mam = new MaggiorazioneAbitazioneViewModel();

            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                        {
                            dtma.AggiornaMAB(mvm, idTrasferimento, idMAB);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.Message);

                        List<SelectListItem> lValute = new List<SelectListItem>();

                        var r = new List<SelectListItem>();

                        using (ModelDBISE db = new ModelDBISE())
                        {
                            using (dtValute dtv = new dtValute())
                            {
                                var lv = dtv.GetElencoValute(db);

                                if (lv != null && lv.Count > 0)
                                {
                                    r = (from v in lv
                                         select new SelectListItem()
                                         {
                                             Text = v.DESCRIZIONEVALUTA,
                                             Value = v.IDVALUTA.ToString()
                                         }).ToList();

                                    r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                                }

                                lValute = r;
                                ViewBag.lValute = lValute;

                                ViewData.Add("idMAB", idMAB);
                                ViewData.Add("idTrasferimento", idTrasferimento);

                                return PartialView("ModificaMAB", mvm);
                            }
                        }
                    }
                }
                else
                {
                    List<SelectListItem> lValute = new List<SelectListItem>();

                    var r = new List<SelectListItem>();

                    using (ModelDBISE db = new ModelDBISE())
                    {
                        using (dtValute dtv = new dtValute())
                        {
                            var lv = dtv.GetElencoValute(db);

                            if (lv != null && lv.Count > 0)
                            {
                                r = (from v in lv
                                     select new SelectListItem()
                                     {
                                         Text = v.DESCRIZIONEVALUTA,
                                         Value = v.IDVALUTA.ToString()
                                     }).ToList();

                                r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                            }

                            lValute = r;
                            ViewBag.lValute = lValute;

                            ViewData.Add("idMAB", idMAB);
                            ViewData.Add("idTrasferimento", idTrasferimento);

                            return PartialView("ModificaMAB", mvm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return RedirectToAction("GestioneMAB", new { idTrasferimento = idTrasferimento });
        }

        public ActionResult MessaggioAnnullaMAB(decimal idTrasferimento)
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
                            var t = dtt.GetTrasferimentoById(idTrasferimento);

                            if (t?.idTrasferimento > 0)
                            {
                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                var uff = dtu.GetUffici(t.idUfficio);

                                msg.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaMaggiorazioneAbitazione, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString());
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
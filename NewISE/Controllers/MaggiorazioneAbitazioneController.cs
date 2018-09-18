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
using NewISE.Models.Enumeratori;

namespace NewISE.Controllers
{
    public class MaggiorazioneAbitazioneController : Controller
    {

        [HttpPost]
        public ActionResult MaggiorazioneAbitazione(decimal idTrasferimento)
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

        public ActionResult ElencoDocumentiFormularioMAB()
        {
            return PartialView();
        }


        public ActionResult AttivitaMAB(decimal idTrasferimento)
        {
            List<MABViewModel> mavml = new List<MABViewModel>();
            MABViewModel mavm = new MABViewModel();
            //MaggiorazioneAbitazioneModel mam = new MaggiorazioneAbitazioneModel();
            MAB mab = new MAB();
            PERIODOMAB pmab = new PERIODOMAB();
            ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();
            AttivazioneMABModel amm = new AttivazioneMABModel();
            ANTICIPOANNUALEMAB aa = new ANTICIPOANNUALEMAB();
            CANONEMAB cm = new CANONEMAB();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    bool soloLettura = false;
                    bool siDati = false;
                    EnumStatoTraferimento statoTrasferimento = 0;
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {

                        using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                        {

                            amm = dtma.GetAttivazionePartenzaMAB(idTrasferimento);

                            if (amm != null && amm.idAttivazioneMAB > 0)
                            {
                                if (amm.notificaRichiesta)
                                {
                                    soloLettura = true;
                                }
                            }
                            mavm.idAttivazioneMAB = amm.idAttivazioneMAB;

                            mab = dtma.GetMABPartenza(idTrasferimento, db);

                            pmab = dtma.GetPeriodoMABPartenza(mab.IDMAB, db);

                            mavm.idMAB = mab.IDMAB;
                            mavm.idPeriodoMAB = pmab.IDPERIODOMAB;

                            mavm.rinunciaMAB = mab.RINUNCIAMAB;

                            cm = dtma.GetCanoneMABPartenza(mab, db);

                            aa = dtma.GetAnticipoAnnualeMABPartenza(mab, db);
                            mavm.importo_canone = cm.IMPORTOCANONE;
                            mavm.dataInizioMAB = pmab.DATAINIZIOMAB;
                            mavm.dataFineMAB = pmab.DATAFINEMAB;
                            if (aa.IDANTICIPOANNUALEMAB > 0)
                            {
                                mavm.anticipoAnnuale = aa.ANTICIPOANNUALE;
                            }
                            else
                            {
                                mavm.anticipoAnnuale = false;

                                aa = dtma.CreaAnticipoAnnualePartenza(mavm, db);
                            }

                            mavm.id_Valuta = cm.IDVALUTA;

                            using (dtValute dtv = new dtValute())
                            {
                                var v = dtv.GetValutaModel(mavm.id_Valuta, db);
                                mavm.descrizioneValuta = v.descrizioneValuta;
                            }

                            var lpc = dtma.GetListPagatoCondivisoMABPartenza(mavm);

                            if (lpc.Count() > 0)
                            {
                                var pc = lpc.First();
                                mavm.canone_pagato = pc.PAGATO;
                                mavm.canone_condiviso = pc.CONDIVISO;
                            }

                            mavml.Add(mavm);

                            siDati = true;

                            var t = dtt.GetTrasferimentoById(idTrasferimento);
                            statoTrasferimento = t.idStatoTrasferimento;
                            if (statoTrasferimento == EnumStatoTraferimento.Annullato)
                            {
                                soloLettura = true;
                            }

                            if (mab.RINUNCIAMAB)
                            {
                                soloLettura = true;
                            }

                            ViewData.Add("soloLettura", soloLettura);
                            ViewData.Add("siDati", siDati);
                            ViewData.Add("idAttivazioneMAB", amm.idAttivazioneMAB);
                            ViewData.Add("idMAB", mab.IDMAB);
                            ViewData.Add("idTrasferimento", idTrasferimento);
                        }
                    }

                    db.Database.CurrentTransaction.Commit();

                }

                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
                }

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
            bool chkRinuncia = false;
            decimal idDocModulo1 = 0;
            decimal idDocModulo2 = 0;
            decimal idDocModulo3 = 0;
            decimal idDocModulo4 = 0;
            decimal idDocModulo5 = 0;
            decimal idDocCopiaContratto = 0;
            decimal idDocCopiaRicevuta = 0;
            EnumStatoTraferimento statoTrasferimento = 0;


            //MaggiorazioneAbitazioneModel mam = new MaggiorazioneAbitazioneModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                    {
                        bool soloLettura = false;

                        AttivazioneMABModel amm = dtma.GetAttivazionePartenzaMAB(idTrasferimento);

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

                            var ma = dtma.GetMABPartenza(idTrasferimento, db);
                            //var rmab = dtma.GetRinunciaMAB(ma);
                            if (ma.RINUNCIAMAB)
                            {
                                chkRinuncia = true;
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
                        ViewData.Add("chkRinuncia", chkRinuncia);
                        ViewData.Add("NumAttivazioni", NumAttivazioni);
                    }
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
            MAB ma = new MAB();
            ANTICIPOANNUALEMAB aa = new ANTICIPOANNUALEMAB(); 
            //VariazioniMABModel vmam = new VariazioniMABModel();
            //RinunciaMABModel rmab = new RinunciaMABModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                    {
                        using (dtTrasferimento dtt = new dtTrasferimento())
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


                            amm = dtma.GetAttivazionePartenzaMAB(idTrasferimento);
                            num_attivazioni = dtma.GetNumAttivazioniMAB(idTrasferimento);
                            ma = dtma.GetMABPartenza(idTrasferimento, db);

                            var ldocModulo1 = dtma.GetDocumentiMABbyTipoDoc(amm.idAttivazioneMAB, (decimal)EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione);
                            if (ldocModulo1.Count > 0)
                            {
                                esisteMod1 = true;
                            }

                            var idAttivazioneMAB = amm.idAttivazioneMAB;

                            bool esisteMAB = ma.IDMAB > 0 ? true : false;
                            //bool esisteVMAB = vmam.idVariazioniMAB > 0 ? true : false;

                            bool notificaRichiesta = amm.notificaRichiesta;
                            bool attivaRichiesta = amm.Attivazione;


                            var t = dtt.GetTrasferimentoById(idTrasferimento);
                            statoTrasferimento = t.idStatoTrasferimento;


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
                                        if (notificaRichiesta == false && attivaRichiesta == false && statoTrasferimento != EnumStatoTraferimento.Annullato)
                                        {
                                            if (esisteMod1)
                                            {
                                                disabledNotificaRichiesta = "";
                                            }
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
                            //gestione pulsanti in caso di rinuncia
                            if (notificaRichiesta && attivaRichiesta == false && ma.RINUNCIAMAB && statoTrasferimento != EnumStatoTraferimento.Annullato)
                            {
                                disabledAttivaRichiesta = "";
                                disabledAnnullaRichiesta = "";
                            }
                            if (ma.RINUNCIAMAB && notificaRichiesta == false && attivaRichiesta == false && statoTrasferimento != EnumStatoTraferimento.Annullato)
                            {
                                disabledNotificaRichiesta = "";
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
                    }
                }

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
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
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
                        AttivazioneMABModel amm = new AttivazioneMABModel();

                        //leggo la prima attivazione
                        amm = dtma.GetAttivazionePartenzaMAB(idTrasferimento);

                        //se non esiste la creo
                        if ((amm != null && amm.idAttivazioneMAB > 0) == false)
                        {
                            amm = dtma.CreaAttivazioneMAB(amm.idMAB, db);
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

        //public ActionResult NuovaMAB(decimal idTrasferimento)
        //{
        //    MABViewModel mam = new MABViewModel();

        //    List<SelectListItem> lValute = new List<SelectListItem>();

        //    var r = new List<SelectListItem>();

        //    try
        //    {
        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
        //            {
        //                using (dtValute dtv = new dtValute())
        //                {
        //                    var lv = dtv.GetElencoValute(db);

        //                    if (lv != null && lv.Count > 0)
        //                    {
        //                        r = (from v in lv
        //                             select new SelectListItem()
        //                             {
        //                                 Text = v.DESCRIZIONEVALUTA,
        //                                 Value = v.IDVALUTA.ToString()
        //                             }).ToList();

        //                        r.Insert(0, new SelectListItem() { Text = "", Value = "" });
        //                    }

        //                    lValute = r;
        //                    ViewBag.lValute = lValute;

        //                    var vm = dtv.GetValutaUfficiale(db);

        //                    mam.id_Valuta = vm.idValuta;
        //                }
        //                using (dtTrasferimento dtt = new dtTrasferimento())
        //                {
        //                    var t = dtt.GetTrasferimentoById(idTrasferimento);
        //                    //mam.dataInizioMAB = t.dataPartenza;
        //                    mam.idTrasferimento = idTrasferimento;
        //                    //mam.dataPartenza = t.dataPartenza;
        //                }
        //                //mam.dataFineMAB = Utility.DataFineStop();
        //                mam.ut_dataFineMAB = null;
        //                mam.importo_canone = 0;

        //                //mam.AnticipoAnnuale = false;
        //                //var mann = dtma.GetMaggiorazioneAnnuale(mam, db);
        //                //mam.idMagAnnuali = mann.idMagAnnuali;

        //                ViewData.Add("idTrasferimento", idTrasferimento);
        //                ViewBag.lValute = lValute;

        //                return PartialView(mam);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
        //    }

        //}


        public ActionResult ModificaMAB(decimal idMAB)
        {
            MABViewModel mam = new MABViewModel();

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
                                var m = db.MAB.Find(idMAB);
                                var t = dtt.GetTrasferimentoById(m.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO);

                                var mab = dtma.GetMABPartenza(t.idTrasferimento, db);

                                var pmm = dtma.GetPeriodoMABModelPartenza(mab.IDMAB, db);

                                CANONEMAB cm = dtma.GetCanoneMABPartenza(mab, db);

                                var aa = dtma.GetAnticipoAnnualeMABPartenza(mab, db);

                                mam.importo_canone = cm.IMPORTOCANONE;
                                mam.dataInizioMAB = pmm.dataInizioMAB;
                                mam.ut_dataInizioMAB = pmm.dataInizioMAB;
                                mam.dataFineMAB = pmm.dataFineMAB;
                                mam.anticipoAnnuale = aa.ANTICIPOANNUALE;
                                mam.id_Valuta = cm.IDVALUTA;

                                var vm = dtv.GetValutaModel(mam.id_Valuta, db);
                                mam.descrizioneValuta = vm.descrizioneValuta;

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

                                mam.idMAB = m.IDMAB;
                                mam.idTrasferimento = t.idTrasferimento;
                                var amab = dtma.GetAttivazionePartenzaMAB(t.idTrasferimento);
                                mam.idAttivazioneMAB = amab.idAttivazioneMAB;

                                mam.idMagAnnuali = 0;
                                var mann = dtma.GetMaggiorazioneAnnuale(mab, db);
                                if (mann.IDMAGANNUALI > 0)
                                {
                                    mam.idMagAnnuali = mann.IDMAGANNUALI;
                                }

                                var lpc = dtma.GetListPagatoCondivisoMABPartenza(mam);

                                if (lpc.Count() > 0)
                                {
                                    var pc = lpc.First();
                                    mam.canone_condiviso = pc.CONDIVISO;
                                    mam.canone_pagato = pc.PAGATO;
                                }

                                mam.ut_dataFineMAB = pmm.dataFineMAB;
                                if (pmm.dataFineMAB == Utility.DataFineStop())
                                {
                                    mam.ut_dataFineMAB = null;
                                }

                                ViewData.Add("idMAB", idMAB);
                                ViewData.Add("idTrasferimento", t.idTrasferimento);
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


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult ConfermaNuovaMAB(MABViewModel mvm, decimal idTrasferimento)
        //{
        //    MABViewModel mam = new MABViewModel();

        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
        //                {
        //                    dtma.InserisciMAB(mvm, idTrasferimento);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                ModelState.AddModelError("", ex.Message);

        //                List<SelectListItem> lValute = new List<SelectListItem>();

        //                var r = new List<SelectListItem>();

        //                using (ModelDBISE db = new ModelDBISE())
        //                {
        //                    using (dtValute dtv = new dtValute())
        //                    {
        //                        var lv = dtv.GetElencoValute(db);

        //                        if (lv != null && lv.Count > 0)
        //                        {
        //                            r = (from v in lv
        //                                 select new SelectListItem()
        //                                 {
        //                                     Text = v.DESCRIZIONEVALUTA,
        //                                     Value = v.IDVALUTA.ToString()
        //                                 }).ToList();

        //                            r.Insert(0, new SelectListItem() { Text = "", Value = "" });
        //                        }

        //                        lValute = r;
        //                        ViewBag.lValute = lValute;
        //                        ViewData.Add("idTrasferimento", idTrasferimento);

        //                        return PartialView("NuovaMAB", mvm);

        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            List<SelectListItem> lValute = new List<SelectListItem>();

        //            var r = new List<SelectListItem>();

        //            using (ModelDBISE db = new ModelDBISE())
        //            {
        //                using (dtValute dtv = new dtValute())
        //                {
        //                    var lv = dtv.GetElencoValute(db);

        //                    if (lv != null && lv.Count > 0)
        //                    {
        //                        r = (from v in lv
        //                             select new SelectListItem()
        //                             {
        //                                 Text = v.DESCRIZIONEVALUTA,
        //                                 Value = v.IDVALUTA.ToString()
        //                             }).ToList();

        //                        r.Insert(0, new SelectListItem() { Text = "", Value = "" });
        //                    }

        //                    lValute = r;
        //                    ViewBag.lValute = lValute;

        //                    ViewData.Add("idTrasferimento", idTrasferimento);

        //                    return PartialView("NuovaMAB", mvm);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
        //    }

        //    return RedirectToAction("GestioneMAB", new { idTrasferimento = idTrasferimento });
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfermaModificaMAB(MABViewModel mvm, decimal idTrasferimento, decimal idMAB)
        {
            MABViewModel mam = new MABViewModel();

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

        public ActionResult GestioneRinunciaMABPartenza(decimal idTrasferimento)
        {
            MABModel mabm = new MABModel();
            bool soloLettura = false;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtMaggiorazioneAbitazione dtmab = new dtMaggiorazioneAbitazione())
                    {
                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            var t = dtt.GetTrasferimentoById(idTrasferimento);

                            mabm = dtmab.GetMABModelPartenza(idTrasferimento, db);

                            var amm = dtmab.GetAttivazionePartenzaMAB(idTrasferimento);

                            EnumStatoTraferimento statoTrasferimento = 0;
                            statoTrasferimento = t.idStatoTrasferimento;
                            if (statoTrasferimento == EnumStatoTraferimento.Annullato || statoTrasferimento == EnumStatoTraferimento.Attivo)
                            {
                                soloLettura = true;
                            }
                            if (mabm.rinunciaMAB && amm.notificaRichiesta)
                            {
                                soloLettura = true;
                            }
                            if (amm.notificaRichiesta)
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

            return PartialView(mabm);
        }

        public JsonResult AggiornaRinunciaMABPartenza(decimal idMAB)
        {
            try
            {
                using (dtMaggiorazioneAbitazione dtmab = new dtMaggiorazioneAbitazione())
                {
                    dtmab.Aggiorna_RinunciaMABPartenza(idMAB);
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
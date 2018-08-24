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
    public class VariazioneMaggiorazioneAbitazioneController : Controller
    {

        [HttpPost]
        public ActionResult VariazioneMaggiorazioneAbitazione(decimal idTrasferimento)
        {
            ViewData.Add("idTrasferimento", idTrasferimento);

            return PartialView();
        }

        public ActionResult ElencoDocumentiFormularioMAB_var()
        {
            return PartialView();
        }

        public ActionResult AttivitaMAB_var(decimal idTrasferimento)
        {
            List<MABViewModel> lmabvm = new List<MABViewModel>();
            MABViewModel mabvm = new MABViewModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                    {
                        bool soloLettura = true;
                        bool siDati = false;
                        EnumStatoTraferimento statoTrasferimento = 0;

                        //AttivazioneMABModel amabm = dtvma.GetUltimaAttivazioneMABmodel(idTrasferimento);

                        //if (amabm != null && amabm.idAttivazioneMAB > 0)
                        //{
                        //    if (amabm.notificaRichiesta && amabm.Attivazione == false)
                        //    {
                        //        soloLettura = true;
                        //    }

                        List<MABModel> lmabm = dtvma.GetElencoMABModel(idTrasferimento);

                        if (lmabm?.Any()??false)
                        {
                            foreach (var mabm in lmabm)
                            {

                                mabvm.idAttivazioneMAB = mabm.idAttivazioneMAB;
                                mabvm.idMAB = mabm.idMAB;

                                var pmm = dtvma.GetPeriodoMABModel(mabm.idMAB);

                                //CANONEMAB cm = dtvma.GetUltimoCanoneMAB_var(mabm);

                                //mabvm.importo_canone = cm.IMPORTOCANONE;

                                //MAB mab = dtvma.GetUltimaMAB(idTrasferimento);
                                mabvm.dataInizioMAB = pmm.dataInizioMAB;
                                mabvm.dataFineMAB = pmm.dataFineMAB;
                                if (mabvm.dataFineMAB<Utility.DataFineStop())
                                {
                                    soloLettura = false;
                                }

                                mabvm.anticipoAnnuale = dtvma.AnticipoAnnualeMAB(mabm.idMAB,db);

                                //if (cm.IDCANONE > 0)
                                //{
                                //    using (dtValute dtv = new dtValute())
                                //    {
                                //        var v = dtv.GetValuta(cm.IDVALUTA);

                                //        mabvm.descrizioneValuta = v.descrizioneValuta;
                                //        mabvm.id_Valuta = v.idValuta;
                                //    }
                                //}

                                //var lpc = dtvma.GetListPagatoCondivisoMAB_var(mabvm);

                                //if (lpc.Count() > 0)
                                //{
                                //    var pc = lpc.First();
                                //    mabvm.canone_pagato = pc.PAGATO;
                                //    mabvm.canone_condiviso = pc.CONDIVISO;
                                //}

                                lmabvm.Add(mabvm);

                                siDati = true;
                            }
                        }

                        //}

                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            var t = dtt.GetTrasferimentoById(idTrasferimento);
                            statoTrasferimento = t.idStatoTrasferimento;
                            if (statoTrasferimento == EnumStatoTraferimento.Annullato ||
                                statoTrasferimento == EnumStatoTraferimento.Terminato)
                            {
                                soloLettura = true;
                            }
                        }

                        ViewData.Add("soloLettura", soloLettura);
                        ViewData.Add("siDati", siDati);
                        ViewData.Add("idAttivazioneMAB", mabvm.idAttivazioneMAB);
                        //ViewData.Add("idMAB", mabvm.idMAB);
                        ViewData.Add("idTrasferimento", idTrasferimento);
                    }
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(lmabvm);
        }

        public ActionResult ElencoCanoneMAB(decimal idTrasferimento, decimal idMab)
        {
            List<CanoneMABViewModel> lcmabvm = new List<CanoneMABViewModel>();
            CanoneMABViewModel cmabvm = new CanoneMABViewModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                    {
                        bool soloLettura = true;
                        bool siDati = false;
                        EnumStatoTraferimento statoTrasferimento = 0;

                        var mab = dtvma.GetMAB_ByID_var(idMab, db);

                        CANONEMAB canonePartenza = new CANONEMAB();
                        using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                        {
                            canonePartenza = dtma.GetCanoneMABPartenza(mab, db);
                        }

                        //AttivazioneMABModel amabm = dtvma.GetUltimaAttivazioneMABmodel(idTrasferimento);

                        //if (amabm != null && amabm.idAttivazioneMAB > 0)
                        //{
                        //    if (amabm.notificaRichiesta && amabm.Attivazione == false)
                        //    {
                        //        soloLettura = true;
                        //    }

                        List<CanoneMABModel> lcmabm = dtvma.GetCanoneMABModel(idMab,db);

                        if (lcmabm?.Any() ?? false)
                        {
                            foreach (var cmabm in lcmabm)
                            {
                                cmabvm.canonePartenza = false;

                                if(cmabm.idCanone==canonePartenza.IDCANONE)
                                {
                                    cmabvm.canonePartenza = true;
                                }

                                cmabvm.idCanone = cmabm.idCanone;
                                cmabvm.IDMAB = cmabm.IDMAB;

                                cmabvm.DataInizioValidita = cmabm.DataInizioValidita;
                                cmabvm.DataFineValidita = cmabm.DataFineValidita;
                                //if (mabvm.dataFineMAB < Utility.DataFineStop())
                                //{
                                //    soloLettura = false;
                                //}

                                //mabvm.anticipoAnnuale = dtvma.AnticipoAnnualeMAB(mabm.idMAB, db);

                                //if (cm.IDCANONE > 0)
                                //{
                                //    using (dtValute dtv = new dtValute())
                                //    {
                                //        var v = dtv.GetValuta(cm.IDVALUTA);

                                //        mabvm.descrizioneValuta = v.descrizioneValuta;
                                //        mabvm.id_Valuta = v.idValuta;
                                //    }
                                //}

                                //var lpc = dtvma.GetListPagatoCondivisoMAB_var(mabvm);

                                //if (lpc.Count() > 0)
                                //{
                                //    var pc = lpc.First();
                                //    mabvm.canone_pagato = pc.PAGATO;
                                //    mabvm.canone_condiviso = pc.CONDIVISO;
                                //}

                                lcmabvm.Add(cmabvm);

                                siDati = true;
                            }
                        }

                        //}

                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            var t = dtt.GetTrasferimentoById(idTrasferimento);
                            statoTrasferimento = t.idStatoTrasferimento;
                            if (statoTrasferimento == EnumStatoTraferimento.Annullato ||
                                statoTrasferimento == EnumStatoTraferimento.Terminato)
                            {
                                soloLettura = true;
                            }
                           
                        }
                        ViewData.Add("idTrasferimento", idTrasferimento);
                        ViewData.Add("soloLettura", soloLettura);
                        //ViewData.Add("siDati", siDati);
                        ViewData.Add("idMab", idMab);
                    }
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(lcmabvm);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NuovoImportoCanone(decimal idMab, decimal idTrasferimento)
        {
            ViewData.Add("idMab", idMab);
            ViewData.Add("idTrasferimento", idTrasferimento);
            return PartialView();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciImportoCanone(CanoneMABModel cmabm, decimal idMab, decimal idTrasferimento)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (ModelDBISE db = new ModelDBISE())
                    {
                        db.Database.BeginTransaction();
                        try
                        {
                            using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                            {

                                try
                                {
                                    dtvma.VerificaDataInizioCanoneMAB(idMab, cmabm.DataInizioValidita);
                                }
                                catch (Exception ex)
                                {
                                    ModelState.AddModelError("", ex.Message);
                                    ViewData.Add("idMab", idMab);
                                    ViewData.Add("idTrasferimento", idTrasferimento);

                                    return PartialView("NuovoImportoCanone", cmabm);
                                }
                                cmabm.DataAggiornamento = DateTime.Now;
                                cmabm.idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione;



                                ATTIVAZIONEMAB attmab_aperta = new ATTIVAZIONEMAB();

                                var attmab_rif = cmabm.IDAttivazioneMAB;
                                var attmab = dtvma.GetAttivazioneAperta(idTrasferimento);

                                // se non esiste attivazione aperta la creo altrimenti la uso
                                if (attmab.IDATTIVAZIONEMAB == 0)
                                {
                                    ATTIVAZIONEMAB new_amab = dtvma.CreaAttivazione(idTrasferimento, db);
                                    attmab_aperta = new_amab;
                                }
                                else
                                {
                                    attmab_aperta = attmab;
                                }

                                DateTime dataRientro = db.TRASFERIMENTO.Find(idTrasferimento).DATARIENTRO;

                                //controlla data inserita superiore a dataRientro
                                if (cmabm.DataInizioValidita > dataRientro)
                                {
                                    cmabm.DataInizioValidita = dataRientro;
                                }

                                if (cmabm.chkAggiornaTutti == false)
                                {
                                    dtvma.SetNuovoImportoCanoneMAB(cmabm, idMab, attmab_aperta.IDATTIVAZIONEMAB, dataRientro, db);
                                }
                                else
                                {
                                    //inserisce periodo e annulla i periodi successivi (fino al primo buco temporale o fino a dataRientro)
                                    dtvma.SetNuovoImportoCanone_AggiornaTutti(cmabm, idMab, attmab_aperta.IDATTIVAZIONEMAB, dataRientro, db);
                                }
                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento nuovo importo canone MAB (" + idMab + ")", "CANONEMAB", db, idTrasferimento, cmabm.idCanone);
                            }
                            
                            db.Database.CurrentTransaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            db.Database.CurrentTransaction.Rollback();
                            throw ex;
                        }
                    }
                }
                else
                {
                    return PartialView("NuovoImportoCanone", cmabm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return RedirectToAction("ElencoCanoneMAB", new {idTrasferimento=idTrasferimento, idMab = idMab });
        }


        public ActionResult GestioneMAB_var(decimal idTrasferimento)
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


        public ActionResult FormulariMAB_var(decimal idTrasferimento)
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
                using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                {
                    bool soloLettura = false;

                    ATTIVAZIONEMAB am = dtvma.GetUltimaAttivazioneMAB(idTrasferimento);

                    if (am != null && am.IDATTIVAZIONEMAB > 0)
                    {
                        dtvma.VerificaDocumenti_var(am,
                                                    out siDocCopiaContratto,
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

                        if (am.NOTIFICARICHIESTA && am.ATTIVAZIONE == false)
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
                    //decimal NumAttivazioni = dtvma.GetNumAttivazioniMAB_var(idTrasferimento);

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
                    //ViewData.Add("NumAttivazioni", NumAttivazioni);
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }


        public ActionResult GestionePulsantiMAB_var(decimal idTrasferimento)
        {
            AttivazioneMABModel amm = new AttivazioneMABModel();
            ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();
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

                using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                {
                    amm = dtvma.GetUltimaAttivazioneMABmodel(idTrasferimento);
                    am = dtvma.GetUltimaAttivazioneMAB(idTrasferimento);
                    //num_attivazioni = dtvma.GetNumAttivazioniMAB_var(idTrasferimento);
                    //var ldocModulo1 = dtvma.GetDocumentiMABbyTipoDoc_var(amm.idAttivazioneMAB, (decimal)EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione);
                    //if (ldocModulo1.Count > 0)
                    //{
                    //    esisteMod1 = true;
                    //}

                    var idAttivazioneMAB = amm.idAttivazioneMAB;

                    //bool esisteMAB = mam.idMAB > 0 ? true : false;
                    bool esisteMAB = false;
                    bool esistePagatoCondivisoMAB = false;
                    bool esisteCanoneMAB = false;
                    bool siDocCopiaContratto = false;
                    bool siDocCopiaRicevuta = false;
                    bool siDocModulo1 = false;
                    bool siDocModulo2 = false;
                    bool siDocModulo3 = false;
                    bool siDocModulo4 = false;
                    bool siDocModulo5 = false;
                    decimal idDocCopiaContratto = 0;
                    decimal idDocCopiaRicevuta = 0;
                    decimal idDocModulo1 = 0;
                    decimal idDocModulo2 = 0;
                    decimal idDocModulo3 = 0;
                    decimal idDocModulo4 = 0;
                    decimal idDocModulo5 = 0;


                    bool notificaRichiesta = amm.notificaRichiesta;
                    bool attivaRichiesta = amm.Attivazione;

                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var t = dtt.GetTrasferimentoById(idTrasferimento);
                        statoTrasferimento = t.idStatoTrasferimento;
                    }

                    ////se non esiste nessuna MAB non esegue nessun controllo
                    //if (esisteMAB)
                    //{

                    //VERIFICA MAB
                    esisteMAB = dtvma.VerificaMAB(idAttivazioneMAB);

                    //verifica Pagato Condiviso MAB
                    esistePagatoCondivisoMAB = dtvma.VerificaPagatoCondivisoMAB(idAttivazioneMAB);

                    //verifica Canone MAB
                    esisteCanoneMAB = dtvma.VerificaCanoneMAB(idAttivazioneMAB);
                    dtvma.VerificaDocumenti_var(am,
                                                out siDocCopiaContratto,
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


                    //se amministratore vedo i pulsanti altrimenti solo notifica
                    if (amministratore)
                    {
                        hiddenAttivaRichiesta = "";
                        hiddenAnnullaRichiesta = "";

                        //if (num_attivazioni == 0)
                        //{
                        if (notificaRichiesta &&
                            attivaRichiesta == false &&
                            statoTrasferimento != EnumStatoTraferimento.Annullato
                            )
                        {
                            disabledAttivaRichiesta = "";
                            disabledAnnullaRichiesta = "";
                        }
                        if (notificaRichiesta == false &&
                            attivaRichiesta == false &&
                            statoTrasferimento != EnumStatoTraferimento.Annullato &&
                            (
                                esisteCanoneMAB ||
                                esistePagatoCondivisoMAB ||
                                esisteMAB ||
                                siDocCopiaContratto ||
                                siDocCopiaRicevuta ||
                                siDocModulo1 ||
                                siDocModulo2 ||
                                siDocModulo3 ||
                                siDocModulo4 ||
                                siDocModulo5
                            )
                            )
                        {
                            disabledNotificaRichiesta = "";
                        }
                        //}
                    }
                    else
                    {
                        //if (num_attivazioni == 0)
                        //{
                        if (
                                notificaRichiesta == false &&
                                attivaRichiesta == false &&
                                statoTrasferimento != EnumStatoTraferimento.Annullato &&
                                (
                                    esisteCanoneMAB ||
                                    esistePagatoCondivisoMAB ||
                                    esisteMAB ||
                                    siDocCopiaContratto ||
                                    siDocCopiaRicevuta ||
                                    siDocModulo1 ||
                                    siDocModulo2 ||
                                    siDocModulo3 ||
                                    siDocModulo4 ||
                                    siDocModulo5
                                )
                            )
                        {
                            disabledNotificaRichiesta = "";
                        }
                        //}
                    }
                    //}
                    //else
                    //{
                    //    if (amministratore)
                    //    {
                    //        hiddenAttivaRichiesta = "";
                    //        hiddenAnnullaRichiesta = "";
                    //    }
                    //}

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
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(amm);
        }

        //        [AcceptVerbs(HttpVerbs.Post)]
        //        public ActionResult DocumentoMAB_var(decimal idTipoDocumento, decimal idTrasferimento)
        //        {
        //            TrasferimentoModel trm = new TrasferimentoModel();
        //            try
        //            {
        //                using (dtTrasferimento dtt = new dtTrasferimento())
        //                {
        //                    using (dtDocumenti dtd = new dtDocumenti())
        //                    {
        //                        trm = dtt.GetTrasferimentoById(idTrasferimento);

        //                        var DescDocumento = dtd.GetDescrizioneTipoDocumentoByIdTipoDocumento(idTipoDocumento);

        //                        ViewData.Add("idTipoDocumento", idTipoDocumento);
        //                        ViewData.Add("DescDocumento", DescDocumento);

        //                        return PartialView(trm);
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }


        //        [AcceptVerbs(HttpVerbs.Post)]
        //        public JsonResult InserisciDocumentoMAB_var(decimal idTrasferimento, decimal idTipoDocumento, HttpPostedFileBase file)
        //        {
        //            using (ModelDBISE db = new ModelDBISE())
        //            {
        //                try
        //                {
        //                    db.Database.BeginTransaction();

        //                    using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
        //                    {
        //                        //leggo l'ultima attivazione
        //                        AttivazioneMABModel amm = dtvma.GetUltimaAttivazioneMABmodel(idTrasferimento);

        //                        //verifica se è in lavorazione e se non lo è la creo
        //                        if (amm.notificaRichiesta && amm.Attivazione)
        //                        {
        //                            ATTIVAZIONEMAB am = dtvma.CreaAttivazioneMAB_var(idTrasferimento, db);
        //                            amm.idAttivazioneMAB = am.IDATTIVAZIONEMAB;
        //                        }

        //                        using (dtDocumenti dtd = new dtDocumenti())
        //                        {
        //                            DocumentiModel dm = new DocumentiModel();
        //                            bool esisteFile = false;
        //                            bool gestisceEstensioni = false;
        //                            bool dimensioneConsentita = false;
        //                            string dimensioneMaxConsentita = string.Empty;

        //                            Utility.PreSetDocumento(file, out dm, out esisteFile, out gestisceEstensioni,
        //                                out dimensioneConsentita, out dimensioneMaxConsentita,
        //                                (EnumTipoDoc)idTipoDocumento);

        //                            if (esisteFile)
        //                            {
        //                                if (gestisceEstensioni == false)
        //                                {
        //                                    throw new Exception(
        //                                        "Il documento selezionato non è nel formato consentito. Il formato supportato è: pdf.");
        //                                }

        //                                if (dimensioneConsentita)
        //                                {
        //                                    //verifica se il documento è gia presente ritornando l'eventuale id
        //                                    decimal idDocumentoEsistente = dtvma.VerificaEsistenzaDocumentoMAB_var(idTrasferimento, (EnumTipoDoc)idTipoDocumento);

        //                                    if (idDocumentoEsistente > 0)
        //                                    {
        //                                        //se già esiste lo sostituisco (imposto modificato=true su quello esistente e ne inserisco una altro)
        //                                        dtvma.SostituisciDocumentoMAB_var(ref dm, idDocumentoEsistente, amm.idAttivazioneMAB, db);

        //                                    }
        //                                    else
        //                                    {
        //                                        //se non esiste lo inserisco
        //                                        dtvma.SetDocumentoMAB_var(ref dm, amm.idAttivazioneMAB, db);
        //                                    }

        //                                    //dtvma.AssociaDocumentoAttivazione_var(amm.idAttivazioneMAB, dm.idDocumenti, db);
        //                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (maggiorazione abitazione).", "Documenti", db, idTrasferimento, dm.idDocumenti);

        //                                }
        //                                else
        //                                {
        //                                    throw new Exception(
        //                                        "Il documento selezionato supera la dimensione massima consentita (" +
        //                                        dimensioneMaxConsentita + " Mb).");
        //                                }
        //                            }
        //                            else
        //                            {
        //                                throw new Exception("Il documento è obbligatorio.");
        //                            }
        //                        }
        //                    }

        //                    db.Database.CurrentTransaction.Commit();
        //                    return Json(new { msg = "Il documento è stato inserito." });
        //                }
        //                catch (Exception ex)
        //                {
        //                    db.Database.CurrentTransaction.Rollback();
        //                    return Json(new { err = ex.Message });
        //                }
        //            }
        //        }

        //        //        public JsonResult ConfermaAttivaRichiestaMAB_var(decimal idAttivazioneMAB)
        //        //        {
        //        //            string errore = "";

        //        //            try
        //        //            {
        //        //                using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
        //        //                {


        //        //                    dtvma.AttivaRichiestaMAB_var(idAttivazioneMAB);
        //        //                }
        //        //            }
        //        //            catch (Exception ex)
        //        //            {

        //        //                errore = ex.Message;
        //        //            }

        //        //            return
        //        //                Json(
        //        //                    new
        //        //                    {
        //        //                        err = errore
        //        //                    });
        //        //        }

        //        //        [HttpPost]
        //        //        [ValidateInput(false)]
        //        //        public JsonResult ConfermaAnnullaRichiestaMAB_var(decimal idAttivazioneMAB, string msg)
        //        //        {
        //        //            string errore = "";

        //        //            try
        //        //            {
        //        //                using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
        //        //                {
        //        //                    dtvma.AnnullaRichiestaMAB_var(idAttivazioneMAB, msg);
        //        //                }
        //        //            }
        //        //            catch (Exception ex)
        //        //            {

        //        //                errore = ex.Message;
        //        //            }

        //        //            return
        //        //                Json(
        //        //                    new
        //        //                    {
        //        //                        err = errore
        //        //                    });
        //        //        }


        //        //        public JsonResult ConfermaNotificaRichiestaMAB_var(decimal idAttivazioneMAB)
        //        //        {
        //        //            string errore = "";

        //        //            try
        //        //            {
        //        //                using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
        //        //                {

        //        //                    dtvma.NotificaRichiestaMAB_var(idAttivazioneMAB);
        //        //                }
        //        //            }
        //        //            catch (Exception ex)
        //        //            {

        //        //                errore = ex.Message;
        //        //            }

        //        //            return
        //        //                Json(
        //        //                    new
        //        //                    {
        //        //                        err = errore
        //        //                    });
        //        //        }

        //        public ActionResult NuovaMAB_var(decimal idTrasferimento)
        //        {
        //            MaggiorazioneAbitazioneViewModel mam = new MaggiorazioneAbitazioneViewModel();

        //            List<SelectListItem> lValute = new List<SelectListItem>();

        //            var r = new List<SelectListItem>();

        //            try
        //            {
        //                using (ModelDBISE db = new ModelDBISE())
        //                {
        //                    using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
        //                    {
        //                        using (dtValute dtv = new dtValute())
        //                        {
        //                            var lv = dtv.GetElencoValute(db);

        //                            if (lv != null && lv.Count > 0)
        //                            {
        //                                r = (from v in lv
        //                                     select new SelectListItem()
        //                                     {
        //                                         Text = v.DESCRIZIONEVALUTA,
        //                                         Value = v.IDVALUTA.ToString()
        //                                     }).ToList();

        //                                r.Insert(0, new SelectListItem() { Text = "", Value = "" });
        //                            }

        //                            lValute = r;
        //                            ViewBag.lValute = lValute;

        //                            var vm = dtv.GetValutaUfficiale(db);

        //                            mam.id_Valuta = vm.idValuta;
        //                        }
        //                        using (dtTrasferimento dtt = new dtTrasferimento())
        //                        {
        //                            var t = dtt.GetTrasferimentoById(idTrasferimento);
        //                            mam.dataInizioMAB = t.dataPartenza;
        //                            mam.dataFineMAB = t.dataRientro.Value;
        //                            mam.idTrasferimento = idTrasferimento;
        //                        }
        //                        mam.ut_dataFineMAB = null;
        //                        if (mam.dataFineMAB < Utility.DataFineStop())
        //                        {
        //                            mam.ut_dataFineMAB=mam.dataFineMAB;
        //                        }
        //                        mam.importo_canone = 0;

        //                        mam.anticipoAnnuale = false;

        //                        ViewData.Add("idTrasferimento", idTrasferimento);
        //                        ViewBag.lValute = lValute;

        //                        return PartialView(mam);
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
        //            }

        //        }


        public ActionResult ModificaMAB_var(decimal idMAB)
        {
            MABViewModel mabvm = new MABViewModel();
            //VARIAZIONIMAB vmab = new VARIAZIONIMAB();

            //List<SelectListItem> lValute = new List<SelectListItem>();

            var r = new List<SelectListItem>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                    {
                        //using (dtValute dtv = new dtValute())
                        //{
                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            var mab = dtvma.GetMAB_ByID_var(idMAB, db);

                            if (mab.IDMAB > 0)
                            {
                                mabvm.idMAB = mab.IDMAB;

                                //mam.dataPartenza = t.dataPartenza;
                                mabvm.idTrasferimento = mab.IDMAGABITAZIONE;
                                mabvm.idAttivazioneMAB = mab.IDATTIVAZIONEMAB;

                                var pmm = dtvma.GetPeriodoMABModel(mab.IDMAB);

                                PeriodoMABModel pm_partenza = new PeriodoMABModel();
                                using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                                {
                                    pm_partenza = dtma.GetPeriodoMABModelPartenza(mab.IDMAB, db);
                                }

                                bool periodo_partenza = false;
                                if(pmm.idPeriodoMAB==pm_partenza.idPeriodoMAB)
                                {
                                    periodo_partenza = true;
                                }
                                mabvm.periodopartenza = periodo_partenza;

                                mabvm.dataInizioMAB = pmm.dataInizioMAB;
                                mabvm.dataFineMAB = pmm.dataFineMAB;
                                mabvm.rinunciaMAB = mab.RINUNCIAMAB;
                                mabvm.anticipoAnnuale = dtvma.AnticipoAnnualeMAB(idMAB, db);

                                mabvm.annualita= false;
                                var aa = dtvma.GetMaggiorazioneAnnuale_var(mabvm, db);
                                if (aa.IDMAGANNUALI > 0)
                                {
                                    mabvm.annualita = true;
                                }
                                ViewData.Add("idMAB", idMAB);
                                ViewData.Add("idTrasferimento", mabvm.idTrasferimento);
                                //ViewBag.lValute = lValute;

                                return PartialView(mabvm);

                            }
                            else
                            {
                                throw new Exception("Errore nella lettura del dettaglio MAB");
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


        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        public ActionResult ConfermaNuovaMAB_var(MaggiorazioneAbitazioneViewModel mvm, decimal idTrasferimento)
        //        {
        //            MaggiorazioneAbitazioneViewModel mam = new MaggiorazioneAbitazioneViewModel();

        //            try
        //            {
        //                if (ModelState.IsValid)
        //                {
        //                    try
        //                    {
        //                        using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
        //                        {
        //                            dtvma.InserisciMAB_var(mvm, idTrasferimento);
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ModelState.AddModelError("", ex.Message);

        //                        List<SelectListItem> lValute = new List<SelectListItem>();

        //                        var r = new List<SelectListItem>();

        //                        using (ModelDBISE db = new ModelDBISE())
        //                        {
        //                            using (dtValute dtv = new dtValute())
        //                            {
        //                                var lv = dtv.GetElencoValute(db);

        //                                if (lv != null && lv.Count > 0)
        //                                {
        //                                    r = (from v in lv
        //                                         select new SelectListItem()
        //                                         {
        //                                             Text = v.DESCRIZIONEVALUTA,
        //                                             Value = v.IDVALUTA.ToString()
        //                                         }).ToList();

        //                                    r.Insert(0, new SelectListItem() { Text = "", Value = "" });
        //                                }

        //                                lValute = r;
        //                                ViewBag.lValute = lValute;
        //                                ViewData.Add("idTrasferimento", idTrasferimento);

        //                                return PartialView("NuovaMAB_var", mvm);

        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    List<SelectListItem> lValute = new List<SelectListItem>();

        //                    var r = new List<SelectListItem>();

        //                    using (ModelDBISE db = new ModelDBISE())
        //                    {
        //                        using (dtValute dtv = new dtValute())
        //                        {
        //                            var lv = dtv.GetElencoValute(db);

        //                            if (lv != null && lv.Count > 0)
        //                            {
        //                                r = (from v in lv
        //                                     select new SelectListItem()
        //                                     {
        //                                         Text = v.DESCRIZIONEVALUTA,
        //                                         Value = v.IDVALUTA.ToString()
        //                                     }).ToList();

        //                                r.Insert(0, new SelectListItem() { Text = "", Value = "" });
        //                            }

        //                            lValute = r;
        //                            ViewBag.lValute = lValute;

        //                            ViewData.Add("idTrasferimento", idTrasferimento);

        //                            return PartialView("NuovaMAB_var", mvm);
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
        //            }

        //            return RedirectToAction("GestioneMAB_var", new { idTrasferimento = idTrasferimento });
        //        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfermaModificaMAB_var(MABViewModel mvm, decimal idTrasferimento, decimal idMAB)
        {
            MABViewModel mam = new MABViewModel();

            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                        {
                            dtvma.AggiornaMAB_var(mvm, idTrasferimento, idMAB);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.Message);


                        ViewData.Add("idMAB", idMAB);
                        ViewData.Add("idTrasferimento", idTrasferimento);

                        return PartialView("ModificaMAB_var", mvm);
                    }
                }
                else
                {

                    ViewData.Add("idMAB", idMAB);
                    ViewData.Add("idTrasferimento", idTrasferimento);

                    return PartialView("ModificaMAB_var", mvm);
                }
            }
            catch (Exception ex)
            {
                PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return RedirectToAction("GestioneMAB_var", new { idTrasferimento = idTrasferimento });
        }

        //        public ActionResult MessaggioAnnullaMAB_var(decimal idTrasferimento)
        //        {
        //            ModelloMsgMail msg = new ModelloMsgMail();

        //            try
        //            {
        //                using (dtDipendenti dtd = new dtDipendenti())
        //                {
        //                    using (dtTrasferimento dtt = new dtTrasferimento())
        //                    {
        //                        using (dtUffici dtu = new dtUffici())
        //                        {
        //                            var t = dtt.GetTrasferimentoById(idTrasferimento);

        //                            if (t?.idTrasferimento > 0)
        //                            {
        //                                var dip = dtd.GetDipendenteByID(t.idDipendente);
        //                                var uff = dtu.GetUffici(t.idUfficio);

        //                                msg.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaMaggiorazioneAbitazione, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString());
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
        //            }
        //            return PartialView(msg);
        //        }


    }
}
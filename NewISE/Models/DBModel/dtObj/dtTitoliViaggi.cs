using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using NewISE.EF;
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtTitoliViaggi : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private void InvioEmailPraticaConclusaTitoliViaggio(decimal idTitoloViaggio, ModelDBISE db)
        {
            AccountModel am = new AccountModel();
            TitoloViaggioModel tvm = new TitoloViaggioModel();
            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();
            string nominativiDellaRichiesta = string.Empty;

            try
            {
                tvm = this.GetTitoloViaggioByID(idTitoloViaggio, db);
                if (tvm != null && tvm.HasValue())
                {
                    if (tvm.notificaRichiesta == true && tvm.praticaConclusa == true)
                    {
                        using (GestioneEmail gmail = new GestioneEmail())
                        {
                            using (ModelloMsgMail msgMail = new ModelloMsgMail())
                            {
                                using (dtDipendenti dtd = new dtDipendenti())
                                {
                                    var destUggs = System.Configuration.ConfigurationManager.AppSettings["EmailUfficioGestioneGiuridicaEsviluppo"];
                                    msgMail.destinatario.Add(new Destinatario() { Nominativo = "Ufficio Gestione Giuridica e Sviluppo", EmailDestinatario = destUggs });
                                    using (dtUtentiAutorizzati dtua = new dtUtentiAutorizzati())
                                    {
                                        luam = dtua.GetUtentiByRuolo(EnumRuoloAccesso.Amministratore, db).ToList();
                                        if (luam?.Any() ?? false)
                                        {

                                            foreach (var uam in luam)
                                            {
                                                var dm = dtd.GetDipendenteByMatricola(uam.matricola, db);

                                                if (dm != null && dm.HasValue() && dm.email != string.Empty)
                                                {
                                                    msgMail.destinatario.Add(new Destinatario() { Nominativo = dm.Nominativo, EmailDestinatario = dm.email });
                                                }

                                            }


                                        }
                                    }

                                    am = Utility.UtenteAutorizzato();
                                    msgMail.cc.Add(new Destinatario() { Nominativo = am.nominativo, EmailDestinatario = am.eMail });

                                    using (dtTrasferimento dttr = new dtTrasferimento())
                                    {
                                        var trm = dttr.GetSoloTrasferimentoById(tvm.idTitoloViaggio);
                                        if (trm != null && trm.idTrasferimento > 0)
                                        {
                                            var dm = dtd.GetDipendenteByID(trm.idDipendente, db);
                                            if (dm != null && dm.idDipendente > 0)
                                            {
                                                nominativiDellaRichiesta = dm.Nominativo;

                                            }
                                        }
                                    }

                                    using (dtConiuge dtc = new dtConiuge())
                                    {
                                        var lcm = dtc.GetListaConiugeByIdTitoloViaggio(tvm.idTitoloViaggio, db).ToList();

                                        if (lcm?.Any() ?? false)
                                        {
                                            nominativiDellaRichiesta = lcm.Aggregate(nominativiDellaRichiesta,
                                                (current, cm) => current + (", " + cm.nominativo));
                                        }
                                    }

                                    using (dtFigli dtf = new dtFigli())
                                    {
                                        var lfm = dtf.GetListaFigliByIdTitoloViaggio(tvm.idTitoloViaggio, db).ToList();
                                        if (lfm?.Any() ?? false)
                                        {
                                            nominativiDellaRichiesta += lfm.Aggregate(nominativiDellaRichiesta,
                                                (current, fm) => current + (", " + fm.nominativo));
                                        }
                                    }

                                    if (msgMail.destinatario?.Any() ?? false)
                                    {
                                        msgMail.oggetto = Resources.msgEmail.OggettoPraticaConclusaTitoloViaggio;
                                        msgMail.corpoMsg = string.Format(
                                            Resources.msgEmail.MessaggioPraticaConclusaTitoloViaggio, nominativiDellaRichiesta);
                                        gmail.sendMail(msgMail);
                                    }
                                    else
                                    {
                                        throw new Exception("Non è stato possibile inviare l'email.");
                                    }

                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Non è stato possibile inviare l'email.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void InvioEmailTitoliViaggioRichiesta(decimal idTitoloViaggio, ModelDBISE db)
        {
            AccountModel am = new AccountModel();
            TitoloViaggioModel tvm = new TitoloViaggioModel();
            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();
            string nominativiDellaRichiesta = string.Empty;

            try
            {
                tvm = this.GetTitoloViaggioByID(idTitoloViaggio, db);
                if (tvm != null && tvm.HasValue())
                {
                    if (tvm.notificaRichiesta == true && tvm.praticaConclusa == false)
                    {
                        using (GestioneEmail gmail = new GestioneEmail())
                        {
                            using (ModelloMsgMail msgMail = new ModelloMsgMail())
                            {
                                using (dtDipendenti dtd = new dtDipendenti())
                                {
                                    var destUggs = System.Configuration.ConfigurationManager.AppSettings["EmailUfficioGestioneGiuridicaEsviluppo"];
                                    msgMail.destinatario.Add(new Destinatario() { Nominativo = "Ufficio Gestione Giuridica e Sviluppo", EmailDestinatario = destUggs });

                                    using (dtUtentiAutorizzati dtua = new dtUtentiAutorizzati())
                                    {
                                        luam = dtua.GetUtentiByRuolo(EnumRuoloAccesso.Amministratore, db).ToList();
                                        if (luam?.Any() ?? false)
                                        {

                                            foreach (var uam in luam)
                                            {
                                                var dm = dtd.GetDipendenteByMatricola(uam.matricola, db);

                                                if (dm != null && dm.HasValue() && dm.email != string.Empty)
                                                {
                                                    msgMail.destinatario.Add(new Destinatario() { Nominativo = dm.Nominativo, EmailDestinatario = dm.email });
                                                }

                                            }


                                        }
                                    }

                                    am = Utility.UtenteAutorizzato();
                                    msgMail.cc.Add(new Destinatario() { Nominativo = am.nominativo, EmailDestinatario = am.eMail });

                                    using (dtTrasferimento dttr = new dtTrasferimento())
                                    {
                                        var trm = dttr.GetSoloTrasferimentoById(tvm.idTitoloViaggio);
                                        if (trm != null && trm.idTrasferimento > 0)
                                        {
                                            var dm = dtd.GetDipendenteByID(trm.idDipendente, db);
                                            if (dm != null && dm.idDipendente > 0)
                                            {
                                                nominativiDellaRichiesta = dm.Nominativo;

                                            }
                                        }
                                    }
                                }

                                using (dtConiuge dtc = new dtConiuge())
                                {
                                    var lcm = dtc.GetListaConiugeByIdTitoloViaggio(tvm.idTitoloViaggio, db).ToList();

                                    if (lcm?.Any() ?? false)
                                    {
                                        nominativiDellaRichiesta = lcm.Aggregate(nominativiDellaRichiesta,
                                            (current, cm) => current + (", " + cm.nominativo));
                                    }
                                }

                                using (dtFigli dtf = new dtFigli())
                                {
                                    var lfm = dtf.GetListaFigliByIdTitoloViaggio(tvm.idTitoloViaggio, db).ToList();
                                    if (lfm?.Any() ?? false)
                                    {
                                        nominativiDellaRichiesta += lfm.Aggregate(nominativiDellaRichiesta,
                                            (current, fm) => current + (", " + fm.nominativo));
                                    }
                                }

                                if (msgMail.destinatario?.Any() ?? false)
                                {
                                    msgMail.oggetto = Resources.msgEmail.OggettoRichiestaTitoloViaggio;
                                    msgMail.corpoMsg = string.Format(
                                        Resources.msgEmail.MessaggioRichiestaTitoloViaggio, nominativiDellaRichiesta);
                                    gmail.sendMail(msgMail);
                                }
                                else
                                {
                                    throw new Exception("Non è stato possibile inviare l'email.");
                                }


                            }
                        }

                    }
                    else
                    {
                        throw new Exception("Non è stato possibile inviare l'email.");
                    }
                }
                else
                {
                    throw new Exception("Non è stato possibile inviare l'email.");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void SetPraticaConclusa(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var tr = db.TRASFERIMENTO.Find(idTrasferimento);

                    var tv = tr.TITOLIVIAGGIO.Where(a => a.NOTIFICARICHIESTA == true && a.PRATICACONCLUSA == false).OrderByDescending(a => a.IDTITOLOVIAGGIO).First();

                    if (tv != null && tv.IDTITOLOVIAGGIO > 0)
                    {
                        tv.PRATICACONCLUSA = true;
                        tv.DATAPRATICACONCLUSA = DateTime.Now;

                        int i = db.SaveChanges();

                        if (i <= 0)
                        {
                            throw new Exception("Non è stato possibile concludere la pratica per i titoli di viaggio.");
                        }
                        else
                        {
                            Utility.PreSetLogAttivita(EnumAttivitaCrud.Modifica,
                                "Conclusione della pratica per i titoli di viaggio", "TITOLIVIAGGIO", db,
                                idTrasferimento, tv.IDTITOLOVIAGGIO);

                            this.InvioEmailPraticaConclusaTitoliViaggio(tv.IDTITOLOVIAGGIO, db);

                        }
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

        public void SetNotificaRichiesta(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var tr = db.TRASFERIMENTO.Find(idTrasferimento);

                    var tv = tr.TITOLIVIAGGIO.Where(a => a.NOTIFICARICHIESTA == false && a.PRATICACONCLUSA == false).OrderByDescending(a => a.IDTITOLOVIAGGIO).First();

                    if (tv != null && tv.IDTITOLOVIAGGIO > 0)
                    {
                        tv.NOTIFICARICHIESTA = true;
                        tv.DATANOTIFICARICHIESTA = DateTime.Now;

                        int i = db.SaveChanges();

                        if (i <= 0)
                        {
                            throw new Exception("Non è stato possibile inserire la notifica di richiesta per le pratiche di passaporto.");
                        }
                        else
                        {
                            Utility.PreSetLogAttivita(EnumAttivitaCrud.Modifica,
                                "Notifica della richiesta dei titoli di viaggio.", "TITOLIVIAGGIO", db,
                                idTrasferimento, tv.IDTITOLOVIAGGIO);

                            this.InvioEmailTitoliViaggioRichiesta(tv.IDTITOLOVIAGGIO, db);

                            var lc =
                            tv.CONIUGE.Where(
                                a =>
                                    a.ANNULLATO == false && a.ESCLUDITITOLOVIAGGIO == false &&
                                    a.DATANOTIFICATV.HasValue == false).ToList();
                            if (lc?.Any() ?? false)
                            {
                                foreach (var c in lc)
                                {

                                    c.DATANOTIFICATV = DateTime.Now;

                                    Utility.PreSetLogAttivita(EnumAttivitaCrud.Modifica,
                                        "Notifica della richiesta dei titoli di viaggio.", "CONIUGE", db,
                                        idTrasferimento, c.IDCONIUGE);

                                }
                            }

                            var lf =
                                tv.FIGLI.Where(
                                    a =>
                                        a.ANNULLATO == false && a.ESCLUDITITOLOVIAGGIO == false &&
                                        a.DATANOTIFICATV.HasValue == false).ToList();
                            if (lf?.Any() ?? false)
                            {
                                foreach (var f in lf)
                                {
                                    f.DATANOTIFICATV = DateTime.Now;

                                    Utility.PreSetLogAttivita(EnumAttivitaCrud.Modifica,
                                       "Notifica della richiesta dei titoli di viaggio.", "Figli", db,
                                       idTrasferimento, f.IDFIGLI);
                                }
                            }
                            if ((lc?.Any() ?? false) || (lf?.Any() ?? false))
                            {

                                int j = db.SaveChanges();

                                if (j <= 0)
                                {
                                    //var log = db.Database.Log;

                                    throw new Exception("Non è stato possibile inserire la notifica di richiesta per le pratiche di passaporto.");
                                }
                            }


                        }
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


        public GestPulsantiAttConclRvModel GestionePulsantiTitoliViaggi(decimal idTrasferimento)
        {
            GestPulsantiAttConclRvModel gptv = new GestPulsantiAttConclRvModel();

            bool chkPesonalmente = false;
            bool pulsanteNotificaRichiesta = false;
            bool pulsantePraticaConclusa = false;

            bool personalmente = false;
            bool notificaRichiesta = false;
            bool praticaConclusa = false;


            bool escludiTitoloViaggioRichiedente = false;
            bool docTitoliViaggiInseritiRichiedente = false;

            bool richiestaAttivaConiuge = false;
            bool esistonoDocumentiConiuge = false;

            bool richiestaAttivaFiglio = false;
            bool esistonoDocumentiFiglio = false;


            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                if (t != null && t.IDTRASFERIMENTO > 0)
                {



                    if (personalmente == true)
                    {
                        chkPesonalmente = true;
                        pulsanteNotificaRichiesta = false;
                        pulsantePraticaConclusa = false;
                    }
                    else
                    {
                        chkPesonalmente = false;

                        if (notificaRichiesta == true)
                        {
                            pulsanteNotificaRichiesta = false;
                            if (praticaConclusa == true)
                            {
                                pulsantePraticaConclusa = false;
                            }
                            else
                            {

                                if (escludiTitoloViaggioRichiedente == false)
                                {
                                    if (docTitoliViaggiInseritiRichiedente == true)
                                    {
                                        pulsantePraticaConclusa = true;
                                    }
                                    else
                                    {
                                        pulsantePraticaConclusa = false;

                                        goto Finish;
                                    }

                                }

                                if (richiestaAttivaConiuge == true)
                                {
                                    if (esistonoDocumentiConiuge == true)
                                    {
                                        pulsantePraticaConclusa = true;
                                    }
                                    else
                                    {
                                        pulsantePraticaConclusa = false;
                                        goto Finish;
                                    }
                                }

                                if (richiestaAttivaFiglio == true)
                                {
                                    if (esistonoDocumentiFiglio == true)
                                    {
                                        pulsantePraticaConclusa = true;
                                    }
                                    else
                                    {
                                        pulsantePraticaConclusa = false;
                                        goto Finish;
                                    }
                                }






                            }
                        }
                        else
                        {
                            pulsantePraticaConclusa = false;

                            if (escludiTitoloViaggioRichiedente == false || richiestaAttivaConiuge == true || richiestaAttivaFiglio == true)
                            {
                                pulsanteNotificaRichiesta = true;
                            }
                            else
                            {
                                pulsanteNotificaRichiesta = false;
                            }


                        }
                    }

                }
                Finish:
                gptv = new GestPulsantiAttConclRvModel()
                {
                    chkPesonalmente = chkPesonalmente,
                    pulsanteNotificaRichiesta = pulsanteNotificaRichiesta,
                    pulsantePraticaConclusa = pulsantePraticaConclusa
                };


            }



            return gptv;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="idFamiliare">Per il coniuge è l'idConiuge, per il figlio è l'idFiglio, per il richiedente è l'id trasferimento o idTitoloViaggio per via del riferimento uno ad uno.</param>
        /// <param name="parentela"></param>
        /// <returns></returns>
        public ElencoFamiliariModel GetDatiForColElencoDoc(decimal idFamiliare, EnumParentela parentela)
        {
            ElencoFamiliariModel efm = new ElencoFamiliariModel();
            TrasferimentoModel trm;
            MaggiorazioniFamiliariModel mfm;
            TitoloViaggioModel tvm = new TitoloViaggioModel();

            using (dtTrasferimento dttr = new dtTrasferimento())
            {
                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                {
                    using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                    {
                        using (dtDocumenti dtdoc = new dtDocumenti())
                        {
                            switch (parentela)
                            {
                                case EnumParentela.Coniuge:
                                    using (dtConiuge dtc = new dtConiuge())
                                    {
                                        var cm = dtc.GetConiugebyID(idFamiliare);
                                        if (cm != null && cm.HasValue())
                                        {

                                            mfm = dtmf.GetMaggiorazioniFamiliaribyConiuge(cm.idConiuge);
                                            trm = dttr.GetTrasferimentoByIDMagFam(mfm.idMaggiorazioniFamiliari);
                                            tvm = dttv.GetTitoloViaggioByID(trm.idTrasferimento);

                                            List<DocumentiModel> ldm = new List<DocumentiModel>();

                                            ldm.AddRange(dtdoc.GetDocumentiByIdTable(tvm.idTitoloViaggio,
                                                EnumTipoDoc.Carta_Imbarco, EnumParentela.Coniuge));
                                            ldm.AddRange(dtdoc.GetDocumentiByIdTable(tvm.idTitoloViaggio,
                                                EnumTipoDoc.Titolo_Viaggio, EnumParentela.Coniuge));

                                            efm = new ElencoFamiliariModel()
                                            {
                                                idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari,
                                                idFamiliare = cm.idConiuge,
                                                idTitoloViaggio = tvm.idTitoloViaggio,
                                                Nominativo = cm.nominativo,
                                                CodiceFiscale = cm.codiceFiscale,
                                                dataInizio = cm.dataInizio,
                                                dataFine = cm.dataFine,
                                                parentela = EnumParentela.Coniuge,
                                                idAltriDati = 0,
                                                Documenti = ldm,
                                                escludiTitoloViaggio = cm.escludiTitoloViaggio,
                                                personalmente = tvm.personalmente
                                            };
                                        }
                                    }
                                    break;
                                case EnumParentela.Figlio:
                                    using (dtFigli dtf = new dtFigli())
                                    {
                                        var fm = dtf.GetFigliobyID(idFamiliare);
                                        if (fm != null && fm.HasValue())
                                        {
                                            mfm = dtmf.GetMaggiorazioniFamiliaribyFiglio(fm.idFigli);
                                            trm = dttr.GetTrasferimentoByIDMagFam(mfm.idMaggiorazioniFamiliari);
                                            tvm = dttv.GetTitoloViaggioByID(trm.idTrasferimento);

                                            List<DocumentiModel> ldm = new List<DocumentiModel>();

                                            ldm.AddRange(dtdoc.GetDocumentiByIdTable(tvm.idTitoloViaggio,
                                                EnumTipoDoc.Carta_Imbarco, EnumParentela.Figlio));
                                            ldm.AddRange(dtdoc.GetDocumentiByIdTable(tvm.idTitoloViaggio,
                                                EnumTipoDoc.Titolo_Viaggio, EnumParentela.Figlio));

                                            efm = new ElencoFamiliariModel()
                                            {
                                                idMaggiorazioniFamiliari = fm.idMaggiorazioniFamiliari,
                                                idFamiliare = fm.idFigli,
                                                idTitoloViaggio = tvm.idTitoloViaggio,
                                                Nominativo = fm.nominativo,
                                                CodiceFiscale = fm.codiceFiscale,
                                                dataInizio = fm.dataInizio,
                                                dataFine = fm.dataFine,
                                                parentela = EnumParentela.Figlio,
                                                idAltriDati = 0,
                                                Documenti = ldm,
                                                escludiTitoloViaggio = fm.escludiTitoloViaggio,
                                                personalmente = tvm.personalmente,
                                            };
                                        }
                                    }
                                    break;
                                case EnumParentela.Richiedente:
                                    using (dtDipendenti dtd = new dtDipendenti())
                                    {
                                        trm = dttr.GetTrasferimentoById(idFamiliare);
                                        var lmfm = dtmf.GetListaMaggiorazioniFamiliariByIDTrasf(trm.idTrasferimento).OrderBy(a => a.idMaggiorazioniFamiliari);
                                        mfm = lmfm.First();
                                        var dm = dtd.GetDipendenteByIDTrasf(trm.idTrasferimento);
                                        tvm = dttv.GetTitoloViaggioByID(trm.idTrasferimento);

                                        List<DocumentiModel> ldm = new List<DocumentiModel>();

                                        ldm.AddRange(dtdoc.GetDocumentiByIdTable(tvm.idTitoloViaggio,
                                            EnumTipoDoc.Carta_Imbarco, EnumParentela.Richiedente));
                                        ldm.AddRange(dtdoc.GetDocumentiByIdTable(tvm.idTitoloViaggio,
                                            EnumTipoDoc.Titolo_Viaggio, EnumParentela.Richiedente));

                                        efm = new ElencoFamiliariModel()
                                        {
                                            idMaggiorazioniFamiliari = mfm.idMaggiorazioniFamiliari,
                                            idFamiliare = trm.idTrasferimento,
                                            ///In questo caso portiamo l'id del trasferimento interessato perché inserire l'id del dipendente potrebbe portare errori per via che un dipendente può avere molti trasferimenti.
                                            idTitoloViaggio = tvm.idTitoloViaggio,
                                            Nominativo = dm.Nominativo,
                                            CodiceFiscale = string.Empty,
                                            dataInizio = trm.dataPartenza,
                                            dataFine = trm.dataRientro,
                                            parentela = EnumParentela.Richiedente,
                                            idAltriDati = 0,
                                            Documenti = ldm,
                                            escludiTitoloViaggio = tvm.escludiTitoloViaggio,
                                            personalmente = tvm.personalmente
                                        };
                                    }

                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException("parentela");
                            }
                        }
                    }
                }
            }


            return efm;
        }


        public void SetEscludiTitoloViaggio(decimal idTrasferimento, ref bool chk)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var tr = db.TRASFERIMENTO.Find(idTrasferimento);

                var tv = tr.TITOLIVIAGGIO.Where(a => a.ESCLUDITITOLOVIAGGIO == false).OrderByDescending(a => a.IDTITOLOVIAGGIO).First();

                if (tv != null && tv.IDTITOLOVIAGGIO > 0)
                {
                    tv.ESCLUDITITOLOVIAGGIO = tv.ESCLUDITITOLOVIAGGIO == false ? true : false;
                    int i = db.SaveChanges();

                    if (i <= 0)
                    {
                        throw new Exception("Non è stato possibile modificare lo stato di escludi titolo viaggio.");
                    }
                    else
                    {
                        chk = tv.ESCLUDITITOLOVIAGGIO;
                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                            "Esclusione dalla richiesta del titolo di viaggio.", "TitoliViaggio", db, idTrasferimento,
                            idTrasferimento);
                    }
                }
            }
        }


        public TitoloViaggioModel GetTitoloViaggioByIdFiglio(decimal idFiglio)
        {
            TitoloViaggioModel tvm = new TitoloViaggioModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tv = db.FIGLI.Find(idFiglio).TITOLIVIAGGIO;

                if (tv != null && tv.IDTITOLOVIAGGIO > 0)
                {
                    tvm = new TitoloViaggioModel()
                    {
                        idTitoloViaggio = tv.IDTITOLOVIAGGIO,
                        notificaRichiesta = tv.NOTIFICARICHIESTA,
                        dataNotificaRichiesta = tv.DATANOTIFICARICHIESTA,
                        praticaConclusa = tv.PRATICACONCLUSA,
                        dataPraticaConclusa = tv.DATAPRATICACONCLUSA,
                        personalmente = tv.PERSONALMENTE,
                        escludiTitoloViaggio = tv.ESCLUDITITOLOVIAGGIO,
                    };
                }
            }

            return tvm;
        }


        public TitoloViaggioModel GetTitoloViaggioByIdConiuge(decimal idConiuge)
        {
            TitoloViaggioModel tvm = new TitoloViaggioModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tv = db.CONIUGE.Find(idConiuge).TITOLIVIAGGIO;

                if (tv != null && tv.IDTITOLOVIAGGIO > 0)
                {
                    tvm = new TitoloViaggioModel()
                    {
                        idTitoloViaggio = tv.IDTITOLOVIAGGIO,
                        notificaRichiesta = tv.NOTIFICARICHIESTA,
                        dataNotificaRichiesta = tv.DATANOTIFICARICHIESTA,
                        praticaConclusa = tv.PRATICACONCLUSA,
                        dataPraticaConclusa = tv.DATAPRATICACONCLUSA,
                        personalmente = tv.PERSONALMENTE,
                        escludiTitoloViaggio = tv.ESCLUDITITOLOVIAGGIO,
                    };
                }
            }

            return tvm;
        }


        public IList<ElencoFamiliariModel> GetDipendentiTitoliViaggio(decimal idTrasferimento)
        {
            List<ElencoFamiliariModel> lefm = new List<ElencoFamiliariModel>();
            TrasferimentoModel trm = new TrasferimentoModel();
            DipendentiModel dm = new DipendentiModel();
            MaggiorazioniFamiliariModel mf = new MaggiorazioniFamiliariModel();
            TitoloViaggioModel tvm = new TitoloViaggioModel();

            using (dtDipendenti dtd = new dtDipendenti())
            {
                using (dtTrasferimento dttr = new dtTrasferimento())
                {
                    using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                    {
                        using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                        {
                            using (dtDocumenti dtdoc = new dtDocumenti())
                            {
                                trm = dttr.GetTrasferimentoById(idTrasferimento);
                                if (trm != null && trm.HasValue())
                                {
                                    dm = dtd.GetDipendenteByIDTrasf(trm.idTrasferimento);
                                    var lmf =
                                        dtmf.GetListaMaggiorazioniFamiliariByIDTrasf(trm.idTrasferimento)
                                            .Where(
                                                a =>
                                                    a.richiestaAttivazione == true && a.attivazioneMaggiorazioni == true)
                                            .OrderByDescending(a => a.idMaggiorazioniFamiliari)
                                            .ToList();
                                    if (lmf?.Any() ?? false)
                                    {
                                        mf = lmf.First();

                                        tvm = dttv.GetTitoloViaggioByID(trm.idTrasferimento);

                                        #region Titoli di viaggio richiedente

                                        if (dm != null && dm.HasValue())
                                        {
                                            List<DocumentiModel> ldm = new List<DocumentiModel>();

                                            ldm.AddRange(dtdoc.GetDocumentiByIdTable(tvm.idTitoloViaggio,
                                                EnumTipoDoc.Carta_Imbarco, EnumParentela.Richiedente));
                                            ldm.AddRange(dtdoc.GetDocumentiByIdTable(tvm.idTitoloViaggio,
                                                EnumTipoDoc.Titolo_Viaggio, EnumParentela.Richiedente));
                                            ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                            {
                                                idMaggiorazioniFamiliari = mf.idMaggiorazioniFamiliari,
                                                idFamiliare = idTrasferimento,
                                                ///In questo caso portiamo l'id del trasferimento interessato perché inserire l'id del dipendente potrebbe portare errori per via che un dipendente può avere n trasferimenti.
                                                idTitoloViaggio = tvm.idTitoloViaggio,
                                                Nominativo = dm.Nominativo,
                                                CodiceFiscale = string.Empty,
                                                dataInizio = trm.dataPartenza,
                                                dataFine = trm.dataRientro,
                                                parentela = EnumParentela.Richiedente,
                                                idAltriDati = 0,
                                                Documenti = ldm,
                                                personalmente = tvm.personalmente,
                                                escludiTitoloViaggio = tvm.escludiTitoloViaggio
                                            };

                                            lefm.Add(efm);
                                        }

                                        #endregion

                                        #region Titoli viaggio familiari

                                        if (mf != null && mf.HasValue())
                                        {
                                            if (mf.attivazioneMaggiorazioni == true)
                                            {
                                                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                                                {
                                                    #region Coniuge

                                                    using (dtConiuge dtc = new dtConiuge())
                                                    {
                                                        var lcm =
                                                            dtc.GetListaConiugeByIdMagFam(mf.idMaggiorazioniFamiliari)
                                                                .Where(
                                                                    a =>
                                                                        a.idTipologiaConiuge ==
                                                                        EnumTipologiaConiuge.Residente)
                                                                .ToList();
                                                        if (lcm?.Any() ?? false)
                                                        {
                                                            foreach (var cm in lcm)
                                                            {
                                                                List<DocumentiModel> ldm = new List<DocumentiModel>();

                                                                ldm.AddRange(dtdoc.GetDocumentiByIdTable(
                                                                    tvm.idTitoloViaggio, EnumTipoDoc.Carta_Imbarco,
                                                                    EnumParentela.Coniuge));
                                                                ldm.AddRange(dtdoc.GetDocumentiByIdTable(
                                                                    tvm.idTitoloViaggio, EnumTipoDoc.Titolo_Viaggio,
                                                                    EnumParentela.Coniuge));

                                                                ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                                                {
                                                                    idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari,
                                                                    idFamiliare = cm.idConiuge,
                                                                    idTitoloViaggio = cm.idTitoloViaggio,
                                                                    Nominativo = cm.nominativo,
                                                                    CodiceFiscale = cm.codiceFiscale,
                                                                    dataInizio = cm.dataInizio,
                                                                    dataFine = cm.dataFine,
                                                                    parentela = EnumParentela.Coniuge,
                                                                    idAltriDati =
                                                                        dtadf.GetAlttriDatiFamiliariConiuge(cm.idConiuge)
                                                                            .idAltriDatiFam,
                                                                    Documenti = ldm,
                                                                    personalmente = tvm.personalmente,
                                                                    escludiTitoloViaggio = cm.escludiTitoloViaggio,
                                                                };

                                                                lefm.Add(efm);
                                                            }
                                                        }
                                                    }

                                                    #endregion

                                                    #region Figli

                                                    using (dtFigli dtf = new dtFigli())
                                                    {
                                                        var lfm =
                                                            dtf.GetListaFigli(mf.idMaggiorazioniFamiliari)
                                                                .Where(
                                                                    a =>
                                                                        new[]
                                                                        {
                                                                        EnumTipologiaFiglio.Residente,
                                                                        EnumTipologiaFiglio.StudenteResidente,
                                                                        }.Contains
                                                                            (a.idTipologiaFiglio))
                                                                .ToList();
                                                        if (lfm?.Any() ?? false)
                                                        {
                                                            foreach (var fm in lfm)
                                                            {
                                                                List<DocumentiModel> ldm = new List<DocumentiModel>();

                                                                ldm.AddRange(dtdoc.GetDocumentiByIdTable(
                                                                    tvm.idTitoloViaggio, EnumTipoDoc.Carta_Imbarco,
                                                                    EnumParentela.Figlio));
                                                                ldm.AddRange(dtdoc.GetDocumentiByIdTable(
                                                                    tvm.idTitoloViaggio, EnumTipoDoc.Titolo_Viaggio,
                                                                    EnumParentela.Figlio));

                                                                ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                                                {
                                                                    idMaggiorazioniFamiliari = fm.idMaggiorazioniFamiliari,
                                                                    idFamiliare = fm.idFigli,
                                                                    idTitoloViaggio = fm.idTitoloViaggio,
                                                                    Nominativo = fm.nominativo,
                                                                    CodiceFiscale = fm.codiceFiscale,
                                                                    dataInizio = fm.dataInizio,
                                                                    dataFine = fm.dataFine,
                                                                    parentela = EnumParentela.Figlio,
                                                                    idAltriDati =
                                                                        dtadf.GetAlttriDatiFamiliariConiuge(fm.idFigli)
                                                                            .idAltriDatiFam,
                                                                    Documenti = ldm,
                                                                    personalmente = tvm.personalmente,
                                                                    escludiTitoloViaggio = fm.escludiTitoloViaggio
                                                                };

                                                                lefm.Add(efm);
                                                            }
                                                        }
                                                    }

                                                    #endregion
                                                }
                                            }
                                        }

                                        #endregion
                                    }



                                }
                            }
                        }
                    }
                }
            }

            return lefm;
        }

        public TitoloViaggioModel GetTitoloViaggioByID(decimal idBiglietto)
        {
            TitoloViaggioModel bm = new TitoloViaggioModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var b = db.TITOLIVIAGGIO.Find(idBiglietto);

                if (b != null && b.IDTITOLOVIAGGIO > 0)
                {
                    bm = new TitoloViaggioModel()
                    {
                        idTitoloViaggio = b.IDTITOLOVIAGGIO,
                        notificaRichiesta = b.NOTIFICARICHIESTA,
                        dataNotificaRichiesta = b.DATANOTIFICARICHIESTA,
                        praticaConclusa = b.PRATICACONCLUSA,
                        dataPraticaConclusa = b.DATAPRATICACONCLUSA,
                        personalmente = b.PERSONALMENTE,
                        escludiTitoloViaggio = b.ESCLUDITITOLOVIAGGIO,
                    };
                }
            }

            return bm;
        }


        public TitoloViaggioModel GetTitoloViaggioByID(decimal idBiglietto, ModelDBISE db)
        {
            TitoloViaggioModel bm = new TitoloViaggioModel();


            var b = db.TITOLIVIAGGIO.Find(idBiglietto);

            if (b != null && b.IDTITOLOVIAGGIO > 0)
            {
                bm = new TitoloViaggioModel()
                {
                    idTitoloViaggio = b.IDTITOLOVIAGGIO,
                    notificaRichiesta = b.NOTIFICARICHIESTA,
                    dataNotificaRichiesta = b.DATANOTIFICARICHIESTA,
                    praticaConclusa = b.PRATICACONCLUSA,
                    dataPraticaConclusa = b.DATAPRATICACONCLUSA,
                    personalmente = b.PERSONALMENTE,
                    escludiTitoloViaggio = b.ESCLUDITITOLOVIAGGIO,
                };
            }


            return bm;
        }


        public void PreSetTitoloViaggio(decimal idTrasferimento, ModelDBISE db)
        {
            TITOLIVIAGGIO tv = new TITOLIVIAGGIO()
            {
                IDTRASFERIMENTO = idTrasferimento,
                NOTIFICARICHIESTA = false,
                PRATICACONCLUSA = false,
                PERSONALMENTE = false,
            };

            db.TITOLIVIAGGIO.Add(tv);
            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Errore nella fase d'inserimento dei dati per la gestione dei titoli di viaggio.");
            }
            else
            {
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                    "Inserimento dei dati di gestione per i titoli di viaggio.", "TITOLIVIAGGIO", db, idTrasferimento,
                    tv.IDTITOLOVIAGGIO);
            }
        }

        public TitoloViaggioModel GetTitoloViaggioInLavorazioneByIdTrasf(decimal idTrasferimento)
        {
            TitoloViaggioModel tvm = new TitoloViaggioModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var tv =
                    t.TITOLIVIAGGIO.Where(a => a.NOTIFICARICHIESTA == false && a.PRATICACONCLUSA == false)
                        .OrderByDescending(a => a.IDTITOLOVIAGGIO)
                        .First();

                tvm = new TitoloViaggioModel()
                {
                    idTitoloViaggio = tv.IDTITOLOVIAGGIO,
                    idTrasferimento = tv.IDTRASFERIMENTO,
                    notificaRichiesta = tv.NOTIFICARICHIESTA,
                    dataNotificaRichiesta = tv.DATANOTIFICARICHIESTA,
                    praticaConclusa = tv.PRATICACONCLUSA,
                    personalmente = tv.PERSONALMENTE,
                    escludiTitoloViaggio = tv.ESCLUDITITOLOVIAGGIO
                };
            }

            return tvm;
        }

    }
}

using System.Web;
using NewISE.EF;
using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models.ModelRest;
using NewISE.Models.Tools;
using System.Diagnostics;
using System.IO;

using NewISE.Models.Config;
using NewISE.Models.Config.s_admin;
using NewISE.Models.DBModel.Enum;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtVariazioniMaggiorazioneFamiliare : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }



        public void SituazioneMagFamVariazione(decimal idMaggiorazioniFamiliari, out bool rinunciaMagFam,
                                       out bool richiestaAttivazione, out bool Attivazione,
                                       out bool datiConiuge, out bool datiParzialiNuovoConiuge,
                                       out bool datiFigli, out bool datiParzialiNuoviFigli,
                                       out bool siDocNuovoConiuge, out bool siDocNuoviFigli,
                                       out bool docFormulario, out bool inLavorazione,
                                       out bool trasfSolaLettura, out bool datiParziali,
                                       out bool siDocIdentita, out bool datiNuovoConiuge, out bool datiNuoviFigli,
                                       out bool siDocFormulari, out bool siPensioneConiuge)
        {
            rinunciaMagFam = false;
            richiestaAttivazione = false;
            Attivazione = false;
            datiConiuge = false;
            datiNuovoConiuge = false;
            datiParzialiNuovoConiuge = false;
            datiParziali = true;
            datiFigli = false;
            datiNuoviFigli = false;
            datiParzialiNuoviFigli = false;
            siDocNuovoConiuge = true;
            siDocIdentita = false;
            siDocNuoviFigli = true;
            docFormulario = false;
            inLavorazione = false;
            trasfSolaLettura = false;
            siDocFormulari = false;
            siPensioneConiuge = false;

            List<ATTIVAZIONIMAGFAM> lamf = new List<ATTIVAZIONIMAGFAM>();
            ATTIVAZIONIMAGFAM amf = new ATTIVAZIONIMAGFAM();
            ATTIVAZIONIMAGFAM amf_pens = new ATTIVAZIONIMAGFAM();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                    #region stato trasferimento
                    decimal IDstatoTrasf = mf.TRASFERIMENTO.IDSTATOTRASFERIMENTO;
                    if (IDstatoTrasf == (decimal)EnumStatoTraferimento.Annullato)
                    {
                        trasfSolaLettura = true;
                    }
                    #endregion

                    #region attivazione
                    //cerco l'ultima attivazione
                    //amf = GetAttivazioneById(idMaggiorazioniFamiliari, EnumTipoTabella.MaggiorazioniFamiliari);
                    ////lamf = mf.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false && a.ATTIVAZIONEMAGFAM == false && a.RICHIESTAATTIVAZIONE == false).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();
                    lamf = mf.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();
                    if (lamf?.Any() ?? false)
                    {
                        amf = lamf.First();
                    }
                    richiestaAttivazione = amf.RICHIESTAATTIVAZIONE;
                    Attivazione = amf.ATTIVAZIONEMAGFAM;
                    #endregion

                    #region coniuge
                    //controlla se esiste coniuge associato
                    if (amf.CONIUGE != null)
                    {
                        var lc = amf.CONIUGE.ToList();
                        if (lc?.Any() ?? false)
                        {
                            datiConiuge = true;
                            inLavorazione = true;
                            //controlla se eventuali nuovi coniugi sono completi
                            foreach (var c in lc)
                            {
                                if (c.IDSTATORECORD==(decimal)EnumStatoRecord.In_Lavorazione && !(c.FK_IDCONIUGE>0))
                                {
                                    datiNuovoConiuge = true;
                                    var nadfc = c.ALTRIDATIFAM.Count(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione);
                                    if (nadfc == 0)
                                    {
                                        datiParzialiNuovoConiuge = true;
                                    }
                                    var ndoc = c.DOCUMENTI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione && a.IDTIPODOCUMENTO==(decimal)EnumTipoDoc.Documento_Identita).Count();
                                    if (ndoc == 0)
                                    {
                                        //siDocConiuge = true;
                                        siDocNuovoConiuge = false;
                                    }

                                    //controlla eventuale pensione
                                    var lpc = c.PENSIONE.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();
                                    if (lpc?.Any() ?? false)
                                    {
                                        var pc = lpc.First();

                                        amf_pens = GetAttivazioneById(pc.IDPENSIONE, EnumTipoTabella.Pensione);

                                    }
                                    else
                                    {
                                        amf_pens = GetAttivazioneById(c.IDCONIUGE, EnumTipoTabella.Coniuge);
                                    }
                                    if (amf_pens.RICHIESTAATTIVAZIONE == false || amf_pens.ATTIVAZIONEMAGFAM)
                                    {
                                        siPensioneConiuge = true;
                                        inLavorazione = true;
                                    }

                                }
                            }
                        }
                    }
                    #endregion

                    #region altri dati
                    var nadc = amf.ALTRIDATIFAM.Count(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.IDSTATORECORD != (decimal)EnumStatoRecord.Attivato);
                    if (nadc > 0)
                    {
                        //datiParzialiConiuge = false;
                        datiParziali = false;
                        inLavorazione = true;
                    }
                    #endregion altri dati

                    #region documenti
                    // controlla documenti identita associati 
                    var ndocIdentita = amf.DOCUMENTI.Where(a => 
                        a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.IDSTATORECORD != (decimal)EnumStatoRecord.Attivato &&
                        a.IDTIPODOCUMENTO==(decimal)EnumTipoDoc.Documento_Identita).Count();
                    if (ndocIdentita > 0)
                    {
                        //siDocConiuge = true;
                        siDocIdentita = true;
                        inLavorazione = true;
                    }

                    // controlla esistenza assoluta di almeno un formulario 
                    foreach (var att in lamf)
                    {
                        var ndocFormulari = att.DOCUMENTI.Where(a =>
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari).Count();
                        if (ndocFormulari > 0)
                        {
                            siDocFormulari = true;
                        }
                    }
                    #endregion

                    #region figli
                    //controlla figli associati
                    if (amf.FIGLI != null)
                    {
                        var lf = amf.FIGLI.ToList();

                        if (lf?.Any() ?? false)
                        {
                            datiFigli = true;
                            inLavorazione = true;
                            //controlla se eventuali nuovi coniugi sono completi
                            foreach (var f in lf)
                            {
                                if (f.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione && !(f.FK_IDFIGLI > 0))
                                {
                                    datiNuoviFigli = true;
                                    var nadff = f.ALTRIDATIFAM.Count(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione);
                                    if (nadff == 0)
                                    {
                                        datiParzialiNuoviFigli = true;
                                    }
                                    var ndoc = f.DOCUMENTI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita).Count();
                                    if (ndoc == 0)
                                    {
                                        siDocNuoviFigli = false;
                                    }
                                }
                            }
                        }
                    }
                    #endregion figli

                    #region pensioni
                    //controlla eventuale pensione
                    var lp = amf.PENSIONE.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();
                    if (lp?.Any() ?? false)
                    {
                        var p = lp.First();

                        amf_pens = GetAttivazioneById(p.IDPENSIONE, EnumTipoTabella.Pensione);
                        if (amf_pens.RICHIESTAATTIVAZIONE == false || amf_pens.ATTIVAZIONEMAGFAM)
                        {
                            siPensioneConiuge = true;
                            inLavorazione = true;
                        }
                    }
                  
                    //var npens = amf.PENSIONE.Where(a =>
                    //    a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.IDSTATORECORD != (decimal)EnumStatoRecord.Attivato).Count();
                    //if (npens > 0)
                    //{
                    //    siPensioneConiuge = true;
                    //    inLavorazione = true;
                    //}
                    #endregion


                    #region controllo in lavorazione
                    if (richiestaAttivazione == false && (
                        datiConiuge || 
                        siDocIdentita || 
                        datiParziali == false || 
                        datiFigli))
                    {
                        inLavorazione = true;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SituazioneAttivazioneMagFamById(decimal idAttivazione, out bool rinunciaMagFam,
                                       out bool richiestaAttivazione, out bool Attivazione,
                                       out bool datiConiuge, out bool datiParzialiConiuge,
                                       out bool datiFigli, out bool datiParzialiFigli,
                                       out bool siDocConiuge, out bool siDocFigli, out bool siPensioniConiuge,
                                       out bool docFormulario, out bool siDocIdentita, out bool siAdf)
        {
            rinunciaMagFam = false;
            richiestaAttivazione = false;
            Attivazione = false;
            datiConiuge = false;
            datiParzialiConiuge = false;
            datiFigli = false;
            datiParzialiFigli = false;
            siDocIdentita = false;
            siAdf = false;
            siDocConiuge = false;
            siDocFigli = false;
            siPensioniConiuge = false;
            docFormulario = false;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazione);

                    var mf = amf.MAGGIORAZIONIFAMILIARI;

                    if (amf != null && amf.IDATTIVAZIONEMAGFAM > 0)
                    {
                        var rmf = mf.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                    .OrderByDescending(a => a.IDRINUNCIAMAGFAM).First();

                        rinunciaMagFam = rmf.RINUNCIAMAGGIORAZIONI;
                        richiestaAttivazione = amf.RICHIESTAATTIVAZIONE;
                        Attivazione = amf.ATTIVAZIONEMAGFAM;

                        var ld = amf.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari && a.IDSTATORECORD!=(decimal)EnumStatoRecord.Annullato).ToList();
                        if (ld?.Any() ?? false)
                        {
                            docFormulario = true;
                        }

                        if (amf.CONIUGE != null)
                        {
                            var lc = amf.CONIUGE.ToList();
                            if (lc?.Any() ?? false)
                            {
                                datiConiuge = true;
                                foreach (var c in lc)
                                {
                                    var nadc = c.ALTRIDATIFAM.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).Count();
                                    if (nadc > 0)
                                    {
                                        datiParzialiConiuge = false;
                                    }
                                    else
                                    {
                                        datiParzialiConiuge = true;
                                    }

                                    var ndocc = c.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita && a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).Count();
                                    if (ndocc > 0)
                                    {
                                        siDocConiuge = true;
                                    }

                                    var npc = c.PENSIONE.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).Count();
                                    if (npc > 0)
                                    {
                                        siPensioniConiuge = true;
                                    }
                                }
                            }
                        }

                        if (amf.FIGLI != null)
                        {
                            var lf = amf.FIGLI.ToList();

                            if (lf?.Any() ?? false)
                            {
                                datiFigli = true;
                                foreach (var f in lf)
                                {
                                    var nadf = f.ALTRIDATIFAM.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).Count();
                                    if (nadf > 0)
                                    {
                                        datiParzialiFigli = false;
                                    }
                                    else
                                    {
                                        datiParzialiFigli = true;
                                    }

                                    var ndocf = f.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita && a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).Count();
                                    if (ndocf > 0)
                                    {
                                        siDocFigli = true;
                                    }
                                    else
                                    {
                                        siDocFigli = false;
                                    }
                                }
                            }
                        }
                        //controlla se c'e' solo un documento variato
                        var ldf = amf.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita && a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();
                        if (ldf?.Any() ?? false)
                        {
                            siDocIdentita = true;
                        }

                        //controlla se c'e' solo altri dati familiari variato
                        var ladf = amf.ALTRIDATIFAM.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();
                        if (ladf?.Any() ?? false)
                        {
                            siAdf = true;
                        }

                        //controlla se c'e' la sola pensione variata (da fare)

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void ModificaConiuge(ConiugeModel cm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();
                try
                {
                    this.EditConiuge(cm, db);

                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public void EditConiuge(ConiugeModel cm, ModelDBISE db)
        {
            try
            {

                var c = db.CONIUGE.Find(cm.idConiuge);

                bool rinunciaMagFam = false;
                decimal idMaggiorazioniFamiliari = c.IDMAGGIORAZIONIFAMILIARI;
                bool richiestaAttivazione = false;
                bool attivazione = false;
                bool datiConiuge = false;
                bool datiParzialiConiuge = false;
                bool datiFigli = false;
                bool datiParzialiFigli = false;
                bool siDocConiuge = false;
                bool siDocFigli = false;
                bool docFormulario = false;
                bool inLavorazione = false;
                bool trasfSolaLettura = false;
                bool siDoc = false;
                bool datiParziali = true;
                bool datiNuovoConiuge = false;
                bool datiNuovoFigli = false;
                bool siDocFormulario = false;
                bool siPensioniConiuge = false;

                DateTime dtIni = cm.dataInizio.Value;
                DateTime dtFin = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop();

                if (c != null && c.IDCONIUGE > 0)
                {
                    if (c.DATAINIZIOVALIDITA != cm.dataInizio.Value || c.DATAFINEVALIDITA != dtFin ||
                        c.IDTIPOLOGIACONIUGE != (decimal)cm.idTipologiaConiuge || c.NOME != cm.nome || c.COGNOME != cm.cognome ||
                        c.CODICEFISCALE != cm.codiceFiscale)
                    {
                        c.DATAAGGIORNAMENTO = DateTime.Now;

                        this.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                            out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                            out datiFigli, out datiParzialiFigli, out siDocConiuge, 
                            out siDocFigli, out docFormulario, out inLavorazione, 
                            out trasfSolaLettura, out datiParziali, out siDoc, 
                            out datiNuovoConiuge, out datiNuovoFigli, out siDocFormulario, 
                            out siPensioniConiuge);

                        //leggo l'ultima attivazione valida
                        var last_attivazione_coniuge = this.GetAttivazioneById(cm.idConiuge, EnumTipoTabella.Coniuge);

                        //leggo se esiste una attivazione in lavorazione
                        var attivazione_aperta = this.GetAttivazioneAperta(cm.idMaggiorazioniFamiliari);
                        if (attivazione_aperta != null && attivazione_aperta.IDATTIVAZIONEMAGFAM > 0)
                        {
                            if (attivazione_aperta.IDATTIVAZIONEMAGFAM != last_attivazione_coniuge.IDATTIVAZIONEMAGFAM)
                            {
                                #region coniuge
                                //crea nuovo coniuge e associa attivazione in lavorazione
                                ConiugeModel newcm = new ConiugeModel()
                                {
                                    idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari,
                                    idTipologiaConiuge = cm.idTipologiaConiuge,
                                    nome = cm.nome,
                                    cognome = cm.cognome,
                                    codiceFiscale = cm.codiceFiscale,
                                    dataInizio = cm.dataInizio.Value,
                                    dataFine = dtFin,
                                    dataAggiornamento = DateTime.Now,
                                    escludiPassaporto = cm.escludiPassaporto,
                                    dataNotificaPP = cm.dataNotificaPP,
                                    escludiTitoloViaggio = cm.escludiTitoloViaggio,
                                    dataNotificaTV = cm.dataNotificaTV,
                                    FK_idConiuge = cm.idConiuge,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione
                                };

                                decimal new_idconiuge = this.SetConiuge(ref newcm, db, attivazione_aperta.IDATTIVAZIONEMAGFAM);
                                var newc = db.CONIUGE.Find(new_idconiuge);
                                #endregion

                                #region altri dati familiari
                                decimal idAdf = 0;
                                //cerco altri dati familiari
                                var adfcm = this.GetAdfValidiByIDConiuge(cm.idConiuge);
                                AssociaAltriDatiFamiliariConiuge(new_idconiuge, adfcm.idAltriDatiFam, db);

                                idAdf = adfcm.idAltriDatiFam;
                                #endregion

                                #region documenti
                                //riassocia documenti
                                var ldc = db.CONIUGE.Find(cm.idConiuge).DOCUMENTI.Where(x => x.MODIFICATO == false && x.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();
                                foreach (var dc in ldc)
                                {
                                    this.Associa_Doc_Coniuge_ById(dc.IDDOCUMENTO, new_idconiuge, db);
                                }
                                //rimuovo precedenti associazioni di documenti in lavorazione al coniuge attivo
                                //this.RimuoviAssociazione_Coniuge_DocumentiInLavorazione(cm.idConiuge, db);
                                #endregion

                                #region pensioni
                                //riassocia eventuali pensioni
                                var lpc = db.CONIUGE.Find(cm.idConiuge).PENSIONE.Where(x => x.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                                foreach (var pc in lpc)
                                {
                                    this.Associa_Pensioni_Coniuge_ById(pc.IDPENSIONE, new_idconiuge, db);
                                }
                                #endregion

                                #region perc maggiorazioni
                                //associa le percentuali maggiorazioni
                                using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                                {

                                    List<PercentualeMagConiugeModel> lpmcm =
                                        dtpc.GetListaPercentualiMagConiugeByRangeDate(newcm.idTipologiaConiuge, dtIni, dtFin, db)
                                            .ToList();

                                    if (lpmcm?.Any() ?? false)
                                    {
                                        foreach (var pmcm in lpmcm)
                                        {
                                            dtpc.AssociaPercentualeMaggiorazioneConiuge(new_idconiuge, pmcm.idPercentualeConiuge, db);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Non è presente nessuna percentuale del coniuge.");
                                    }
                                }
                                #endregion

                                #region titoli viaggio
                                //replico eventuali titoli di viaggio del coniuge e li riassocio
                                var lctv = db.CONIUGE.Find(cm.idConiuge).CONIUGETITOLIVIAGGIO.Where(a => a.ANNULLATO == false).ToList();
                                if (lctv?.Any() ?? false)
                                {
                                    foreach (var ctv in lctv)
                                    {
                                        ctv.ANNULLATO = true;
                                        if (db.SaveChanges() > 0)
                                        {
                                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                                "Annullamento record Coniuge Titoli Viaggio", "CONIUGETITOLIVIAGGIO", db,
                                                ctv.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO, ctv.IDCONIUGETITOLIVIAGGIO);
                                        }
                                        else
                                        {
                                            throw new Exception("Errore in fase di annullamento del titolo viaggio coniuge.");
                                        }
                                        //replica titolo di viaggio e associa il nuovo coniuge
                                        CONIUGETITOLIVIAGGIO ctv_new = new CONIUGETITOLIVIAGGIO()
                                        {
                                            IDCONIUGE = new_idconiuge,
                                            IDTITOLOVIAGGIO = ctv.IDTITOLOVIAGGIO,
                                            IDATTIVAZIONETITOLIVIAGGIO = ctv.IDATTIVAZIONETITOLIVIAGGIO,
                                            RICHIEDITITOLOVIAGGIO = ctv.RICHIEDITITOLOVIAGGIO,
                                            DATAAGGIORNAMENTO = DateTime.Now,
                                            ANNULLATO = false
                                        };
                                        //------------------------
                                        db.CONIUGETITOLIVIAGGIO.Add(ctv_new);
                                        if (db.SaveChanges()>0)
                                        {
                                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                "Inserimento record Coniuge Titoli Viaggio.", "CONIUGETITOLIVIAGGIO", db,
                                                ctv_new.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO, ctv_new.IDATTIVAZIONETITOLIVIAGGIO);
                                        }
                                        else
                                        {
                                            throw new Exception("Errore in fase di inserimento titolo viaggio coniuge.");
                                        }
                                            
                                    }

                                }
                                #endregion

                                #region passaporti
                                //replico eventuali passaporti e li associo al nuovo coniuge
                                var lcp = db.CONIUGE.Find(cm.idConiuge).CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false).ToList();
                                if (lcp?.Any() ?? false)
                                {
                                    foreach (var cp in lcp)
                                    {
                                        cp.ANNULLATO = true;
                                        if (db.SaveChanges() > 0)
                                        {
                                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                                "Annullamento record Coniuge Passaporto", "CONIUGEPASSAPORTO", db,
                                                cp.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, cp.IDCONIUGEPASSAPORTO);
                                        }
                                        else
                                        {
                                            throw new Exception("Errore in fase di annullamento del passaporto coniuge.");
                                        }
                                        //replica passaporto associato al nuovo coniuge
                                        CONIUGEPASSAPORTO cp_new = new CONIUGEPASSAPORTO()
                                        {
                                            IDCONIUGE = new_idconiuge,
                                            IDPASSAPORTI = cp.IDPASSAPORTI,
                                            IDATTIVAZIONIPASSAPORTI= cp.IDATTIVAZIONIPASSAPORTI,
                                            INCLUDIPASSAPORTO = cp.INCLUDIPASSAPORTO,
                                            DATAAGGIORNAMENTO = DateTime.Now,
                                            ANNULLATO = false
                                        };
                                        //------------------------
                                        db.CONIUGEPASSAPORTO.Add(cp_new);
                                        if (db.SaveChanges() > 0)
                                        {
                                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                "Inserimento record Coniuge Passaporto.", "CONIUGEPASSAPORTO", db,
                                                cp_new.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, cp_new.IDATTIVAZIONIPASSAPORTI);
                                        }
                                        else
                                        {
                                            throw new Exception("Errore in fase di inserimento passaporto coniuge.");
                                        }
                                    }
                                }
                                #endregion
                            
                            }
                            else
                            {
                                #region coniuge
                                c.DATAINIZIOVALIDITA = cm.dataInizio.Value;
                                c.DATAFINEVALIDITA = dtFin;
                                c.IDTIPOLOGIACONIUGE = (decimal)cm.idTipologiaConiuge;
                                c.NOME = cm.nome;
                                c.COGNOME = cm.cognome;
                                c.CODICEFISCALE = cm.codiceFiscale;
                                c.DATAAGGIORNAMENTO = DateTime.Now;

                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Impossibile modificare il coniuge.");
                                }
                                #endregion

                                #region maggiorazioni familiari
                                //associa le percentuali maggiorazioni
                                using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                                {

                                    List<PercentualeMagConiugeModel> lpmcm =
                                        dtpc.GetListaPercentualiMagConiugeByRangeDate(cm.idTipologiaConiuge, dtIni, dtFin, db)
                                            .ToList();

                                    if (lpmcm?.Any() ?? false)
                                    {
                                        foreach (var pmcm in lpmcm)
                                        {
                                            dtpc.AssociaPercentualeMaggiorazioneConiuge(cm.idConiuge, pmcm.idPercentualeConiuge,
                                                db);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Non è presente nessuna percentuale del coniuge.");
                                    }
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            #region crea attivazione
                            //crea una nuova attivazione
                            ATTIVAZIONIMAGFAM newmf = new ATTIVAZIONIMAGFAM()
                            {
                                IDMAGGIORAZIONIFAMILIARI = cm.idMaggiorazioniFamiliari,
                                RICHIESTAATTIVAZIONE = false,
                                DATARICHIESTAATTIVAZIONE = null,
                                ATTIVAZIONEMAGFAM = false,
                                DATAATTIVAZIONEMAGFAM = null,
                                ANNULLATO = false,
                                DATAVARIAZIONE = DateTime.Now,
                                DATAAGGIORNAMENTO = DateTime.Now,
                            };
                            db.ATTIVAZIONIMAGFAM.Add(newmf);

                            int idx = db.SaveChanges();

                            if (idx <= 0)
                            {
                                throw new Exception("Impossibile modificare il coniuge.");
                            }
                            #endregion

                            #region crea coniuge
                            ConiugeModel newcm = new ConiugeModel()
                            {
                                idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari,
                                idTipologiaConiuge = cm.idTipologiaConiuge,
                                nome = cm.nome,
                                cognome = cm.cognome,
                                codiceFiscale = cm.codiceFiscale,
                                dataInizio = cm.dataInizio.Value,
                                dataFine = dtFin,
                                dataAggiornamento=DateTime.Now,
                                escludiPassaporto = cm.escludiPassaporto,
                                dataNotificaPP = cm.dataNotificaPP,
                                escludiTitoloViaggio = cm.escludiTitoloViaggio,
                                dataNotificaTV = cm.dataNotificaTV,
                                FK_idConiuge = cm.idConiuge,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione
                            };

                            decimal new_idconiuge = this.SetConiuge(ref newcm, db, newmf.IDATTIVAZIONEMAGFAM);
                            var newc = db.CONIUGE.Find(new_idconiuge);
                            #endregion

                            #region altri dati familiari
                            decimal idAdf = 0;
                            //cerco altri dati familiari
                            var adfc = this.GetAdfValidiByIDConiuge(cm.idConiuge);
                            idAdf = adfc.idAltriDatiFam;
                            AssociaAltriDatiFamiliariConiuge(new_idconiuge, idAdf, db);
                            #endregion

                            #region documenti
                            //riassocia documenti
                            var ldc = db.CONIUGE.Find(cm.idConiuge).DOCUMENTI.Where(x => x.MODIFICATO == false && x.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();
                            foreach (var dc in ldc)
                            {
                                this.Associa_Doc_Coniuge_ById(dc.IDDOCUMENTO, new_idconiuge, db);
                                //this.AssociaDocumentoAttivazione(newmf.IDATTIVAZIONEMAGFAM, dc.IDDOCUMENTO, db);
                            }
                            ////rimuovo precedenti associazioni di documenti in lavorazione al coniuge attivo
                            //this.RimuoviAssociazione_Coniuge_DocumentiInLavorazione(cm.idConiuge, db);
                            #endregion

                            #region pensioni
                            //riassocia eventuali pensioni
                            var lpc = db.CONIUGE.Find(cm.idConiuge).PENSIONE.Where(x => x.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                            foreach (var pc in lpc)
                            {
                                this.Associa_Pensioni_Coniuge_ById(pc.IDPENSIONE, new_idconiuge, db);
                            }
                            #endregion

                            #region perc maggiorazioni
                            //associa le percentuali maggiorazioni
                            using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                            {

                                List<PercentualeMagConiugeModel> lpmcm =
                                    dtpc.GetListaPercentualiMagConiugeByRangeDate(cm.idTipologiaConiuge, dtIni, dtFin, db)
                                        .ToList();

                                if (lpmcm?.Any() ?? false)
                                {
                                    foreach (var pmcm in lpmcm)
                                    {
                                        dtpc.AssociaPercentualeMaggiorazioneConiuge(cm.idConiuge, pmcm.idPercentualeConiuge, db);
                                    }
                                }
                                else
                                {
                                    throw new Exception("Non è presente nessuna percentuale del coniuge.");
                                }
                            }
                            #endregion

                            #region titoli viaggio
                            //replico eventuali titoli di viaggio del coniuge e li riassocio
                            var lctv = db.CONIUGE.Find(cm.idConiuge).CONIUGETITOLIVIAGGIO.Where(a => a.ANNULLATO == false).ToList();
                            if (lctv?.Any() ?? false)
                            {
                                foreach (var ctv in lctv)
                                {
                                    ctv.ANNULLATO = true;
                                    if (db.SaveChanges() > 0)
                                    {
                                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                            "Annullamento record Coniuge Titoli Viaggio", "CONIUGETITOLIVIAGGIO", db,
                                            ctv.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO, ctv.IDCONIUGETITOLIVIAGGIO);
                                    }
                                    else
                                    {
                                        throw new Exception("Errore in fase di annullamento del titolo viaggio coniuge.");
                                    }
                                    //replica titolo di viaggio e associa il nuovo coniuge
                                    CONIUGETITOLIVIAGGIO ctv_new = new CONIUGETITOLIVIAGGIO()
                                    {
                                        IDCONIUGE = new_idconiuge,
                                        IDTITOLOVIAGGIO = ctv.IDTITOLOVIAGGIO,
                                        IDATTIVAZIONETITOLIVIAGGIO = ctv.IDATTIVAZIONETITOLIVIAGGIO,
                                        RICHIEDITITOLOVIAGGIO = ctv.RICHIEDITITOLOVIAGGIO,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        ANNULLATO = false
                                    };
                                    //------------------------
                                    db.CONIUGETITOLIVIAGGIO.Add(ctv_new);
                                    if (db.SaveChanges() > 0)
                                    {
                                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                            "Inserimento record Coniuge Titoli Viaggio.", "CONIUGETITOLIVIAGGIO", db,
                                            ctv_new.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO, ctv_new.IDATTIVAZIONETITOLIVIAGGIO);
                                    }
                                    else
                                    {
                                        throw new Exception("Errore in fase di inserimento titolo viaggio coniuge.");
                                    }

                                }

                            }
                            #endregion

                            #region passaporti
                            //replico eventuali passaporti e li associo al nuovo coniuge
                            var lcp = db.CONIUGE.Find(cm.idConiuge).CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false).ToList();
                            if (lcp?.Any() ?? false)
                            {
                                foreach (var cp in lcp)
                                {
                                    cp.ANNULLATO = true;
                                    if (db.SaveChanges() > 0)
                                    {
                                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                            "Annullamento record Coniuge Passaporto", "CONIUGEPASSAPORTO", db,
                                            cp.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, cp.IDCONIUGEPASSAPORTO);
                                    }
                                    else
                                    {
                                        throw new Exception("Errore in fase di annullamento del passaporto coniuge.");
                                    }
                                    //replica passaporto associato al nuovo coniuge
                                    CONIUGEPASSAPORTO cp_new = new CONIUGEPASSAPORTO()
                                    {
                                        IDCONIUGE = new_idconiuge,
                                        IDPASSAPORTI = cp.IDPASSAPORTI,
                                        IDATTIVAZIONIPASSAPORTI = cp.IDATTIVAZIONIPASSAPORTI,
                                        INCLUDIPASSAPORTO = cp.INCLUDIPASSAPORTO,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        ANNULLATO = false
                                    };
                                    //------------------------
                                    db.CONIUGEPASSAPORTO.Add(cp_new);
                                    if (db.SaveChanges() > 0)
                                    {
                                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                            "Inserimento record Coniuge Passaporto.", "CONIUGEPASSAPORTO", db,
                                            cp_new.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, cp_new.IDATTIVAZIONIPASSAPORTI);
                                    }
                                    else
                                    {
                                        throw new Exception("Errore in fase di inserimento passaporto coniuge.");
                                    }
                                }
                            }
                            #endregion
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                //db.Database.CurrentTransaction.Rollback();
                throw ex;
            }
        }

        public decimal SetConiuge(ref ConiugeModel cm, ModelDBISE db, decimal idAttivazione)
        {
            try
            {
                var idAtt = idAttivazione;

                CONIUGE c = new CONIUGE()
                {
                    IDMAGGIORAZIONIFAMILIARI = cm.idMaggiorazioniFamiliari,
                    IDTIPOLOGIACONIUGE = (decimal)cm.idTipologiaConiuge,
                    NOME = cm.nome.ToUpper(),
                    COGNOME = cm.cognome.ToUpper(),
                    CODICEFISCALE = cm.codiceFiscale.ToUpper(),
                    DATAINIZIOVALIDITA = cm.dataInizio.Value,
                    DATAFINEVALIDITA = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop(),
                    DATAAGGIORNAMENTO = DateTime.Now,
                    FK_IDCONIUGE = cm.FK_idConiuge,
                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                };

                db.CONIUGE.Add(c);

                if (db.SaveChanges() <= 0)
                {
                    throw new Exception("Non è stato possibile inserire il coniuge.");
                }
                else
                {
                    var amf = db.ATTIVAZIONIMAGFAM.Find(idAtt);
                    if (amf != null && amf.IDATTIVAZIONEMAGFAM > 0)
                    {
                        if (amf.ATTIVAZIONEMAGFAM)
                        {
                            //crea una nuova attivazione
                            ATTIVAZIONIMAGFAM newmf = new ATTIVAZIONIMAGFAM()
                            {
                                IDMAGGIORAZIONIFAMILIARI = cm.idMaggiorazioniFamiliari,
                                RICHIESTAATTIVAZIONE = false,
                                DATARICHIESTAATTIVAZIONE = null,
                                ATTIVAZIONEMAGFAM = false,
                                DATAATTIVAZIONEMAGFAM = null,
                                ANNULLATO = false,
                                DATAVARIAZIONE = DateTime.Now,
                                DATAAGGIORNAMENTO = DateTime.Now,
                            };
                            db.ATTIVAZIONIMAGFAM.Add(newmf);

                            int idx = db.SaveChanges();

                            if (idx <= 0)
                            {
                                throw new Exception("Impossibile inserire un nuovo coniuge.");
                            }

                            cm.idConiuge = c.IDCONIUGE;
                            idAtt = newmf.IDATTIVAZIONEMAGFAM;
                        }
                    }
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento del coniuge", "CONIUGE", db,
                        cm.idMaggiorazioniFamiliari, c.IDCONIUGE);

                    using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                    {
                        dtamf.AssociaConiugeAttivazione(idAtt, c.IDCONIUGE, db);
                    }

                    return c.IDCONIUGE;

                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RimuoviAssociazione_Coniuge_DocumentiInLavorazione(decimal idConuige, ModelDBISE db)
        {
            var c = db.CONIUGE.Find(idConuige);
            var ldc = c.DOCUMENTI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
            if (ldc?.Any() ?? false)
            {
                foreach (var dc in ldc)
                {
                    c.DOCUMENTI.Remove(dc);
                }

                db.SaveChanges();
            }

        }

        public void RimuoviAssociazione_Figlio_DocumentiInLavorazione(decimal idFiglio, ModelDBISE db)
        {
            var f = db.FIGLI.Find(idFiglio);
            var ldf = f.DOCUMENTI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
            if (ldf?.Any() ?? false)
            {
                foreach (var df in ldf)
                {
                    f.DOCUMENTI.Remove(df);
                }

                db.SaveChanges();
            }

        }

        public void ModificaFiglio(FigliModel fm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();
                try
                {
                    using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                    {
                        dtvmf.EditFiglio(fm, db);
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


        public void EditFiglio(FigliModel fm, ModelDBISE db)
        {
            try
            {
                var f = db.FIGLI.Find(fm.idFigli);

                bool rinunciaMagFam = false;
                decimal idMaggiorazioniFamiliari = f.IDMAGGIORAZIONIFAMILIARI;
                bool richiestaAttivazione = false;
                bool attivazione = false;
                bool datiConiuge = false;
                bool datiParzialiConiuge = false;
                bool datiFigli = false;
                bool datiParzialiFigli = false;
                bool siDocConiuge = false;
                bool siDocFigli = false;
                bool docFormulario = false;
                bool inLavorazione = false;
                bool trasfSolaLettura = false;
                bool siDoc = false;
                bool datiParziali = true;
                bool datiNuovoConiuge = false;
                bool datiNuovoFigli = false;
                bool siDocFormulario = false;
                bool siPensioniConiuge = false;


                DateTime dtIni = fm.dataInizio.Value;
                DateTime dtFin = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop();

                if (f != null && f.IDFIGLI > 0)
                {
                    if (f.DATAINIZIOVALIDITA != fm.dataInizio.Value || f.DATAFINEVALIDITA != dtFin ||
                        f.IDTIPOLOGIAFIGLIO != (decimal)fm.idTipologiaFiglio || f.NOME != fm.nome || f.COGNOME != fm.cognome ||
                        f.CODICEFISCALE != fm.codiceFiscale)
                    {
                        f.DATAAGGIORNAMENTO = DateTime.Now;

                        this.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                            out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                            out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione, out trasfSolaLettura, out datiParziali, out siDoc, out datiNuovoConiuge, out datiNuovoFigli, out siDocFormulario, out siPensioniConiuge);

                        //leggo l'ultima attivazione valida
                        var last_attivazione_figlio = this.GetAttivazioneById(fm.idFigli, EnumTipoTabella.Figli);

                        //leggo se esiste una attivazione in lavorazione
                        var attivazione_aperta = this.GetAttivazioneAperta(fm.idMaggiorazioniFamiliari);
                        if (attivazione_aperta != null && attivazione_aperta.IDATTIVAZIONEMAGFAM > 0)
                        {
                            if (attivazione_aperta.IDATTIVAZIONEMAGFAM != last_attivazione_figlio.IDATTIVAZIONEMAGFAM)
                            {
                                #region figlio
                                // crea nuovo figlio e associa attivazione in lavorazione
                                FigliModel newfm = new FigliModel()
                                {
                                    idMaggiorazioniFamiliari = fm.idMaggiorazioniFamiliari,
                                    idTipologiaFiglio = fm.idTipologiaFiglio,
                                    nome = fm.nome,
                                    cognome = fm.cognome,
                                    codiceFiscale = fm.codiceFiscale,
                                    dataInizio = fm.dataInizio.Value,
                                    dataFine = dtFin,
                                    escludiPassaporto = fm.escludiPassaporto,
                                    dataNotificaPP = fm.dataNotificaPP,
                                    escludiTitoloViaggio = fm.escludiTitoloViaggio,
                                    dataNotificaTV = fm.dataNotificaTV,
                                    FK_IdFigli = fm.idFigli,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    dataAggiornamento=DateTime.Now
                                };

                                decimal new_idfiglio = this.SetFiglio(ref newfm, db, attivazione_aperta.IDATTIVAZIONEMAGFAM);
                                var newf = db.FIGLI.Find(new_idfiglio);
                                #endregion

                                #region altri dati familiari
                                decimal idAdf = 0;
                                //cerco se ci sono già altri dati familiari modificati
                                var adffm = this.GetAdfValidiByIDFiglio(fm.idFigli);
                                idAdf = adffm.idAltriDatiFam;

                                AssociaAltriDatiFamiliariFiglio(new_idfiglio, adffm.idAltriDatiFam, db);
                                #endregion

                                #region documenti
                                //riassocia documenti
                                var ldf = db.FIGLI.Find(fm.idFigli).DOCUMENTI.Where(x => x.MODIFICATO == false && x.IDSTATORECORD!=(decimal)EnumStatoRecord.Annullato).ToList();
                                foreach (var df in ldf)
                                {
                                    this.Associa_Doc_Figlio_ById(df.IDDOCUMENTO, new_idfiglio, db);
                                }
                                //rimuovo precedenti associazioni di documenti in lavorazione al figlio attivo
                                //this.RimuoviAssociazione_Figlio_DocumentiInLavorazione(fm.idFigli, db);
                                #endregion

                                #region perc maggiorazioni
                                //associa le percentuali maggiorazioni
                                using (dtPercentualeMagFigli dtpf = new dtPercentualeMagFigli())
                                {

                                    List<PercentualeMagFigliModel> lpmfm =
                                        dtpf.GetPercentualeMaggiorazioneFigli((EnumTipologiaFiglio)fm.idTipologiaFiglio, dtIni, dtFin, db).ToList();

                                    if (lpmfm?.Any() ?? false)
                                    {
                                        foreach (var pmfm in lpmfm)
                                        {
                                            dtpf.AssociaPercentualeMaggiorazioneFigli(new_idfiglio, pmfm.idPercMagFigli, db);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Non è presente nessuna percentuale del figlio.");
                                    }

                                }
                                #endregion

                                #region primo segretario
                                //associa eventuali indennita primo segretario
                                dtIni = fm.dataInizio.Value;
                                dtFin = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop();
                                using (dtIndennitaPrimoSegretario dtips = new dtIndennitaPrimoSegretario())
                                {
                                    List<IndennitaPrimoSegretModel> lipsm =
                                        dtips.GetIndennitaPrimoSegretario(dtIni, dtFin, db).ToList();

                                    if (lipsm?.Any() ?? false)
                                    {
                                        foreach (var ipsm in lipsm)
                                        {
                                            dtips.AssociaIndennitaPrimoSegretarioFiglio(new_idfiglio, ipsm.idIndPrimoSegr, db);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception(
                                            "Non è presente nessuna indennità di primo segretario per il figlio che si vuole inserire.");
                                    }
                                }
                                #endregion

                                #region titoli viaggio
                                //replico eventuali titoli di viaggio del figlio e li riassocio
                                var lftv = db.FIGLI.Find(fm.idFigli).FIGLITITOLIVIAGGIO.Where(a => a.ANNULLATO == false).ToList();
                                if (lftv?.Any() ?? false)
                                {
                                    foreach (var ftv in lftv)
                                    {
                                        ftv.ANNULLATO = true;
                                        if (db.SaveChanges() > 0)
                                        {
                                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                                "Annullamento record Figli Titoli Viaggio", "CONIUGETITOLIVIAGGIO", db,
                                                ftv.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO, ftv.IDFIGLITITOLIVIAGGIO);
                                        }
                                        else
                                        {
                                            throw new Exception("Errore in fase di inserimento titolo viaggio figli.");
                                        }
                                        //replica titolo di viaggio e associa il nuovo figlio
                                        FIGLITITOLIVIAGGIO ftv_new = new FIGLITITOLIVIAGGIO()
                                        {
                                            IDFIGLI = new_idfiglio,
                                            IDTITOLOVIAGGIO = ftv.IDTITOLOVIAGGIO,
                                            IDATTIVAZIONETITOLIVIAGGIO = ftv.IDATTIVAZIONETITOLIVIAGGIO,
                                            RICHIEDITITOLOVIAGGIO = ftv.RICHIEDITITOLOVIAGGIO,
                                            DATAAGGIORNAMENTO = DateTime.Now,
                                            ANNULLATO = false
                                        };
                                        //------------------------
                                        db.FIGLITITOLIVIAGGIO.Add(ftv_new);
                                        if (db.SaveChanges() > 0)
                                        {
                                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                "Inserimento record Figli Titoli Viaggio.", "FIGLITITOLIVIAGGIO", db,
                                                ftv_new.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO, ftv_new.IDATTIVAZIONETITOLIVIAGGIO);

                                        }
                                        else
                                        {
                                            throw new Exception("Errore in fase di inserimento titolo viaggio figli.");
                                        }

                                    }
                                }
                                #endregion

                                #region passaporti
                                //replico eventuali passaporti e li associo al nuovo figlio
                                var lfp = db.FIGLI.Find(fm.idFigli).FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false).ToList();
                                if (lfp?.Any() ?? false)
                                {
                                    foreach (var fp in lfp)
                                    {
                                        fp.ANNULLATO = true;
                                        if (db.SaveChanges() > 0)
                                        {
                                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                                "Annullamento record Figli Passaporto", "FIGLIPASSAPORTO", db,
                                                fp.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, fp.IDFIGLIPASSAPORTO);
                                        }
                                        else
                                        {
                                            throw new Exception("Errore in fase di annullamento del passaporto figlio.");
                                        }
                                        //replica passaporto associato al nuovo coniuge
                                        FIGLIPASSAPORTO fp_new = new FIGLIPASSAPORTO()
                                        {
                                            IDFIGLI = new_idfiglio,
                                            IDPASSAPORTI = fp.IDPASSAPORTI,
                                            IDATTIVAZIONIPASSAPORTI = fp.IDATTIVAZIONIPASSAPORTI,
                                            INCLUDIPASSAPORTO = fp.INCLUDIPASSAPORTO,
                                            DATAAGGIORNAMENTO = DateTime.Now,
                                            ANNULLATO = false
                                        };
                                        db.FIGLIPASSAPORTO.Add(fp_new);
                                        if (db.SaveChanges() > 0)
                                        {
                                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                "Inserimento record Figli Passaporto.", "FIGLIPASSAPORTO", db,
                                                fp_new.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, fp_new.IDATTIVAZIONIPASSAPORTI);
                                        }
                                        else
                                        {
                                            throw new Exception("Errore in fase di inserimento passaporto figli.");
                                        }

                                    }

                                }
                                #endregion

                            }
                            else
                            {
                                #region altri dati familiari
                                f.DATAINIZIOVALIDITA = fm.dataInizio.Value;
                                f.DATAFINEVALIDITA = dtFin;
                                f.IDTIPOLOGIAFIGLIO = (decimal)fm.idTipologiaFiglio;
                                f.NOME = fm.nome;
                                f.COGNOME = fm.cognome;
                                f.CODICEFISCALE = fm.codiceFiscale;
                                f.DATAAGGIORNAMENTO = DateTime.Now;
                                int i = db.SaveChanges();

                                if (i <= 0)
                                {
                                    throw new Exception("Impossibile modificare il figlio.");
                                }
                                #endregion

                                #region maggiorazioni familiari
                                //associa le percentuali maggiorazioni
                                using (dtPercentualeMagFigli dtpf = new dtPercentualeMagFigli())
                                {

                                    List<PercentualeMagFigliModel> lpmfm = dtpf.GetPercentualeMaggiorazioneFigli((EnumTipologiaFiglio)fm.idTipologiaFiglio, dtIni, dtFin, db).ToList();

                                    if (lpmfm?.Any() ?? false)
                                    {
                                        foreach (var pmfm in lpmfm)
                                        {
                                            dtpf.AssociaPercentualeMaggiorazioneFigli(fm.idFigli, pmfm.idPercMagFigli, db);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Non è presente nessuna percentuale del figlio.");
                                    }
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            //crea una nuova attivazione
                            var newamf = this.CreaAttivazione(idMaggiorazioniFamiliari,db);

                            #region crea figlio
                            FigliModel newfm = new FigliModel()
                            {
                                idMaggiorazioniFamiliari = fm.idMaggiorazioniFamiliari,
                                idTipologiaFiglio = fm.idTipologiaFiglio,
                                nome = fm.nome,
                                cognome = fm.cognome,
                                codiceFiscale = fm.codiceFiscale,
                                dataInizio = fm.dataInizio.Value,
                                dataFine = dtFin,
                                escludiPassaporto = fm.escludiPassaporto,
                                dataNotificaPP = fm.dataNotificaPP,
                                escludiTitoloViaggio = fm.escludiTitoloViaggio,
                                dataNotificaTV = fm.dataNotificaTV,
                                FK_IdFigli = fm.idFigli,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                dataAggiornamento=DateTime.Now
                            };

                            decimal new_idfiglio = this.SetFiglio(ref newfm, db, newamf.IDATTIVAZIONEMAGFAM);
                            var newf = db.FIGLI.Find(new_idfiglio);
                            #endregion

                            #region altri dati familiari
                            decimal idAdf = 0;
                            //cerco se ci sono già altri dati familiari modificati
                            var adff = this.GetAdfValidiByIDFiglio(fm.idFigli);

                            AssociaAltriDatiFamiliariFiglio(new_idfiglio, adff.idAltriDatiFam, db);
                            #endregion

                            #region documenti
                            //riassocia documenti
                            var ldf = db.FIGLI.Find(fm.idFigli).DOCUMENTI.Where(x => x.MODIFICATO == false && x.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();
                            foreach (var df in ldf)
                            {
                                this.Associa_Doc_Figlio_ById(df.IDDOCUMENTO, new_idfiglio, db);
                                //this.AssociaDocumentoAttivazione(newamf.IDATTIVAZIONEMAGFAM, df.IDDOCUMENTO, db);
                            }
                            #endregion

                            #region perc maggiorazioni
                            //associa le percentuali maggiorazioni
                            using (dtPercentualeMagFigli dtpf = new dtPercentualeMagFigli())
                            {

                                List<PercentualeMagFigliModel> lpmfm = dtpf.GetPercentualeMaggiorazioneFigli((EnumTipologiaFiglio)fm.idTipologiaFiglio, dtIni, dtFin, db).ToList();

                                if (lpmfm?.Any() ?? false)
                                {
                                    foreach (var pmfm in lpmfm)
                                    {
                                        dtpf.AssociaPercentualeMaggiorazioneFigli(fm.idFigli, pmfm.idPercMagFigli, db);
                                    }
                                }
                                else
                                {
                                    throw new Exception("Non è presente nessuna percentuale del figlio.");
                                }
                            }
                            #endregion

                            #region passaporti
                            //replico eventuali titoli di viaggio del figlio e li riassocio
                            var lftv = db.FIGLI.Find(fm.idFigli).FIGLITITOLIVIAGGIO.Where(a => a.ANNULLATO == false).ToList();
                            if (lftv?.Any() ?? false)
                            {
                                foreach (var ftv in lftv)
                                {
                                    ftv.ANNULLATO = true;
                                    if (db.SaveChanges() > 0)
                                    {
                                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                            "Annullamento record Figli Titoli Viaggio", "CONIUGETITOLIVIAGGIO", db,
                                            ftv.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO, ftv.IDFIGLITITOLIVIAGGIO);
                                    }
                                    else
                                    {
                                        throw new Exception("Errore in fase di inserimento titolo viaggio figli.");
                                    }
                                    //replica titolo di viaggio e associa il nuovo figlio
                                    FIGLITITOLIVIAGGIO ftv_new = new FIGLITITOLIVIAGGIO()
                                    {
                                        IDFIGLI = new_idfiglio,
                                        IDTITOLOVIAGGIO = ftv.IDTITOLOVIAGGIO,
                                        IDATTIVAZIONETITOLIVIAGGIO = ftv.IDATTIVAZIONETITOLIVIAGGIO,
                                        RICHIEDITITOLOVIAGGIO = ftv.RICHIEDITITOLOVIAGGIO,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        ANNULLATO = false
                                    };
                                    //------------------------
                                    db.FIGLITITOLIVIAGGIO.Add(ftv_new);
                                    if (db.SaveChanges() > 0)
                                    {
                                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                            "Inserimento record Figli Titoli Viaggio.", "FIGLITITOLIVIAGGIO", db,
                                            ftv_new.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO, ftv_new.IDATTIVAZIONETITOLIVIAGGIO);

                                    }
                                    else
                                    {
                                        throw new Exception("Errore in fase di inserimento titolo viaggio figli.");
                                    }

                                }

                            }
                            #endregion

                            #region passaporti
                            //replico eventuali passaporti e li associo al nuovo figlio
                            var lfp = db.FIGLI.Find(fm.idFigli).FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false).ToList();
                            if (lfp?.Any() ?? false)
                            {
                                foreach (var fp in lfp)
                                {
                                    fp.ANNULLATO = true;
                                    if (db.SaveChanges() > 0)
                                    {
                                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                            "Annullamento record Figli Passaporto", "FIGLIPASSAPORTO", db,
                                            fp.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, fp.IDFIGLIPASSAPORTO);
                                    }
                                    else
                                    {
                                        throw new Exception("Errore in fase di annullamento del passaporto figlio.");
                                    }
                                    //replica passaporto associato al nuovo coniuge
                                    FIGLIPASSAPORTO fp_new = new FIGLIPASSAPORTO()
                                    {
                                        IDFIGLI = new_idfiglio,
                                        IDPASSAPORTI = fp.IDPASSAPORTI,
                                        IDATTIVAZIONIPASSAPORTI = fp.IDATTIVAZIONIPASSAPORTI,
                                        INCLUDIPASSAPORTO = fp.INCLUDIPASSAPORTO,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        ANNULLATO = false
                                    };
                                    //------------------------
                                    db.FIGLIPASSAPORTO.Add(fp_new);
                                    if (db.SaveChanges() > 0)
                                    {
                                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                            "Inserimento record Figli Passaporto.", "FIGLIPASSAPORTO", db,
                                            fp_new.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, fp_new.IDATTIVAZIONIPASSAPORTI);
                                    }
                                    else
                                    {
                                        throw new Exception("Errore in fase di inserimento passaporto figli.");
                                    }

                                }

                            }
                            #endregion

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal SetFiglio(ref FigliModel fm, ModelDBISE db, decimal idAttivazione)
        {
            try
            {
                var idAtt = idAttivazione;

                FIGLI f = new FIGLI()
                {
                    IDMAGGIORAZIONIFAMILIARI = fm.idMaggiorazioniFamiliari,
                    IDTIPOLOGIAFIGLIO = (decimal)fm.idTipologiaFiglio,
                    NOME = fm.nome.ToUpper(),
                    COGNOME = fm.cognome.ToUpper(),
                    CODICEFISCALE = fm.codiceFiscale.ToUpper(),
                    DATAINIZIOVALIDITA = fm.dataInizio.Value,
                    DATAFINEVALIDITA = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop(),
                    DATAAGGIORNAMENTO = DateTime.Now,
                    FK_IDFIGLI = fm.FK_IdFigli,
                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                };

                db.FIGLI.Add(f);

                if (db.SaveChanges() <= 0)
                {
                    throw new Exception("Non è stato possibile inserire il figlio.");
                }
                else
                {
                    var amf = db.ATTIVAZIONIMAGFAM.Find(idAtt);
                    if (amf != null && amf.IDATTIVAZIONEMAGFAM > 0)
                    {
                        if (amf.ATTIVAZIONEMAGFAM)
                        {
                            //crea una nuova attivazione
                            var newamf = this.CreaAttivazione(fm.idMaggiorazioniFamiliari,db);

                            fm.idFigli = f.IDFIGLI;
                            idAtt = newamf.IDATTIVAZIONEMAGFAM;

                        }
                    }
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento del figlio", "FIGLI", db,
                        fm.idMaggiorazioniFamiliari, f.IDFIGLI);

                    using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                    {
                        dtamf.AssociaFiglioAttivazione(idAtt, f.IDFIGLI, db);
                    }

                    return f.IDFIGLI;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void PreSetAttivazioneVariazioneMaggiorazioniFamiliari(decimal idMaggiorazioniFamiliari, ModelDBISE db)
        {
            using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
            {

                RinunciaMaggiorazioniFamiliariModel rmfm = new RinunciaMaggiorazioniFamiliariModel()
                {
                    idMaggiorazioniFamiliari = idMaggiorazioniFamiliari,
                    rinunciaMaggiorazioni = false,
                    dataAggiornamento = DateTime.Now,
                    FK_IdRinunciaMagFam=null,
                    idStatoRecord=(decimal)EnumStatoRecord.In_Lavorazione
                };

                dtmf.SetRinunciaMaggiorazioniFamiliari(ref rmfm, db);
            }

            AttivazioniMagFamModel amfm = new AttivazioniMagFamModel()
            {
                idMaggiorazioniFamiliari = idMaggiorazioniFamiliari,
                richiestaAttivazione = false,
                attivazioneMagFam = false,
                dataAggiornamento = DateTime.Now,
                annullato = false
            };

            using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
            {
                dtamf.SetAttivaziomeMagFam(ref amfm, db);
            }

        }


        public void SetFormularioVariazioneMaggiorazioniFamiliari(ref DocumentiModel dm, decimal idMaggiorazioniFamiliari, ModelDBISE db)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                DOCUMENTI d = new DOCUMENTI();

                dm.file.InputStream.CopyTo(ms);

                var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                var lamf =
                    mf.ATTIVAZIONIMAGFAM.Where(
                        a => a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == false && a.ATTIVAZIONEMAGFAM == false)
                        .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();
                if (lamf?.Any() ?? false)
                {
                    var amf = lamf.First();

                    d.NOMEDOCUMENTO = dm.nomeDocumento;
                    d.ESTENSIONE = dm.estensione;
                    d.IDTIPODOCUMENTO = (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari;
                    d.DATAINSERIMENTO = dm.dataInserimento;
                    d.FILEDOCUMENTO = ms.ToArray();
                    d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;
                    amf.DOCUMENTI.Add(d);

                    if (db.SaveChanges() > 0)
                    {
                        dm.idDocumenti = d.IDDOCUMENTO;
                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (formulario maggiorazioni familiari).", "Documenti", db, mf.TRASFERIMENTO.IDTRASFERIMENTO, dm.idDocumenti);
                    }
                }
                else
                {
                    // se non trova attivazioni in corso ne crea una nuova
                    var newamf = this.CreaAttivazione(idMaggiorazioniFamiliari,db);

                    // aggiunge il formulario
                    var att = db.ATTIVAZIONIMAGFAM.Find(newamf.IDATTIVAZIONEMAGFAM);
                    if (att.IDATTIVAZIONEMAGFAM > 0)
                    {
                        d.NOMEDOCUMENTO = dm.nomeDocumento;
                        d.ESTENSIONE = dm.estensione;
                        d.IDTIPODOCUMENTO = (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari;
                        d.DATAINSERIMENTO = dm.dataInserimento;
                        d.FILEDOCUMENTO = ms.ToArray();
                        d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;
                        att.DOCUMENTI.Add(d);

                        dm.idDocumenti = d.IDDOCUMENTO;

                        if (db.SaveChanges() > 0)
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (formulario maggiorazioni familiari).", "Documenti", db, mf.TRASFERIMENTO.IDTRASFERIMENTO, dm.idDocumenti);
                        }
                        else
                        {
                            throw new Exception("Errore nella fase di inserimento del formulario maggiorazioni familiari.");
                        }
                    }

                    // associa il formulario all'attivazioneMagFam
                    using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                    {
                        dtamf.AssociaFormulario(newamf.IDATTIVAZIONEMAGFAM, dm.idDocumenti, db);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AltriDatiFamConiugeModel GetAltriDatiFamiliariConiuge(decimal idConiuge)
        {
            AltriDatiFamConiugeModel adfm = new AltriDatiFamConiugeModel();
            List<AltriDatiFamConiugeModel> ladfcm = new List<AltriDatiFamConiugeModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var c = db.CONIUGE.Find(idConiuge);
                    //var att = c.ATTIVAZIONIMAGFAM.Where(x => x.ANNULLATO == false).OrderByDescending(x => x.IDATTIVAZIONEMAGFAM).First(); ;
                    if (c?.IDCONIUGE > 0)
                    {
                        var ladfc = c.ALTRIDATIFAM.Where(z => z.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a=>a.IDALTRIDATIFAM).ToList();

                         //c.ALTRIDATIFAM
                         //.Where(a => a.ATTIVAZIONIMAGFAM.Any(b => b.ANNULLATO == false));


                        //var ladfc = c.ALTRIDATIFAM.Where(a=>a.att
                        //        a.ATTIVAZIONEMAGFAM
                        ////.ALTRIDATIFAM.Where(x => x.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(x => x.IDALTRIDATIFAM).ToList();



                        if (ladfc?.Any() ?? false)
                        {
                            var adfc = ladfc.First();

                            adfm = new AltriDatiFamConiugeModel()
                            {
                                idConiuge=idConiuge,
                                idAltriDatiFam = adfc.IDALTRIDATIFAM,
                                nazionalita = adfc.NAZIONALITA,
                                indirizzoResidenza = adfc.INDIRIZZORESIDENZA,
                                capResidenza = adfc.CAPRESIDENZA,
                                comuneResidenza = adfc.COMUNERESIDENZA,
                                provinciaResidenza = adfc.PROVINCIARESIDENZA,
                                dataAggiornamento = adfc.DATAAGGIORNAMENTO,
                                idStatoRecord = adfc.IDSTATORECORD,
                                FK_idAltriDatiFam=adfc.FK_IDALTRIDATIFAM
                            };
                            ladfcm.Add(adfm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return adfm;
        }

        //public AltriDatiFamConiugeModel GetAltriDatiFamiliariConiuge(decimal idConiuge, EnumStatoRecord StatoRecord)
        //{
        //    AltriDatiFamConiugeModel adfm = new AltriDatiFamConiugeModel();
        //    List<AltriDatiFamConiugeModel> ladfcm = new List<AltriDatiFamConiugeModel>();
        //    ATTIVAZIONIMAGFAM att = new ATTIVAZIONIMAGFAM();
        //    List<ALTRIDATIFAM> ladfc = new List<ALTRIDATIFAM>();
            

        //    try
        //    {
        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            var c = db.CONIUGE.Find(idConiuge);
        //            var mf = db.MAGGIORAZIONIFAMILIARI.Find(c.IDMAGGIORAZIONIFAMILIARI);

        //            if (StatoRecord==EnumStatoRecord.Attivato)
        //            {
        //                //verifico se esiste un coniuge in lavorazione
        //                bool trovato = false;
        //                var lista_att = db.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false && a.IDMAGGIORAZIONIFAMILIARI == c.IDMAGGIORAZIONIFAMILIARI).OrderBy(a => a.IDATTIVAZIONEMAGFAM).ToList();
        //                foreach (var at in lista_att)
        //                {
        //                    var lc_appo = at.CONIUGE;
        //                    foreach (var c_appo in lc_appo)
        //                    {
        //                        if (c_appo.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione)
        //                        {
        //                            trovato = true;
        //                        }
        //                    }
        //                }

        //                //se non esiste prendo id attivazione degli adf modificati
        //                if (trovato == false)
        //                {
        //                    ladfc = mf.ATTIVAZIONIMAGFAM
        //                            .Where(a => a.ATTIVAZIONEMAGFAM == false).
        //                            OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).First()
        //                            .ALTRIDATIFAM.Where(a => a.ANNULLATO == false && a.IDCONIUGE == idConiuge)
        //                            .OrderByDescending(a => a.IDALTRIDATIFAM).ToList();
        //                }
        //                else
        //                {
        //                    ladfc = mf.ATTIVAZIONIMAGFAM
        //                            .Where(a => a.ATTIVAZIONEMAGFAM == true).
        //                            OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).First()
        //                            .ALTRIDATIFAM.Where(a => a.ANNULLATO == false && a.IDCONIUGE == idConiuge)
        //                            .OrderByDescending(a => a.IDALTRIDATIFAM).ToList();                            
        //                }

        //            }
        //            else
        //            {
        //                ladfc = mf.ATTIVAZIONIMAGFAM
        //                        .Where(a => a.ATTIVAZIONEMAGFAM == false).
        //                        OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).First()
        //                        .ALTRIDATIFAM.Where(a => a.ANNULLATO == false && a.IDCONIUGE == idConiuge)
        //                        .OrderByDescending(a => a.IDALTRIDATIFAM).ToList();                     

        //            }

        //            if (ladfc?.Any() ?? false)
        //            {
        //                var adfc = ladfc.First();

        //                adfm = new AltriDatiFamConiugeModel()
        //                {
        //                    idAltriDatiFam = adfc.IDALTRIDATIFAM,
        //                    idConiuge = adfc.IDCONIUGE.Value,
        //                    nazionalita = adfc.NAZIONALITA,
        //                    indirizzoResidenza = adfc.INDIRIZZORESIDENZA,
        //                    capResidenza = adfc.CAPRESIDENZA,
        //                    comuneResidenza = adfc.COMUNERESIDENZA,
        //                    provinciaResidenza = adfc.PROVINCIARESIDENZA,
        //                    dataAggiornamento = adfc.DATAAGGIORNAMENTO,
        //                    annullato = adfc.ANNULLATO
        //                };
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return adfm;
        //}


        public AltriDatiFamFiglioModel GetAltriDatiFamiliariFiglio(decimal idFiglio, decimal idMaggiorazioniFamiliari)
        {
            AltriDatiFamFiglioModel adffm = new AltriDatiFamFiglioModel();
            List<AltriDatiFamFiglioModel> ladffm = new List<AltriDatiFamFiglioModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var f = db.FIGLI.Find(idFiglio);
                    if (f?.IDFIGLI > 0)
                    {
                        var ladff = f.ALTRIDATIFAM.Where(x => x.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(x => x.IDALTRIDATIFAM).ToList();

                        if (ladff?.Any() ?? false)
                        {
                            var adff = ladff.First();

                            adffm = new AltriDatiFamFiglioModel()
                            {
                                idFigli=idFiglio,
                                idAltriDatiFam = adff.IDALTRIDATIFAM,
                                nazionalita = adff.NAZIONALITA,
                                dataNascita = adff.DATANASCITA,
                                comuneNascita = adff.COMUNENASCITA,
                                capNascita = adff.CAPNASCITA,
                                provinciaNascita = adff.PROVINCIANASCITA,
                                indirizzoResidenza = adff.INDIRIZZORESIDENZA,
                                capResidenza = adff.CAPRESIDENZA,
                                comuneResidenza = adff.COMUNERESIDENZA,
                                provinciaResidenza = adff.PROVINCIARESIDENZA,
                                dataAggiornamento = adff.DATAAGGIORNAMENTO,
                                idStatoRecord = adff.IDSTATORECORD,
                                FK_idAltriDatiFam=adff.FK_IDALTRIDATIFAM
                            };
                            //ladffm.Add(adffm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return adffm;
        }

        public AltriDatiFamFiglioModel GetAltriDatiFamiliariFiglioOld(decimal? idFiglioOld, decimal idMaggiorazioniFamiliari)
        {
            AltriDatiFamFiglioModel adffm = new AltriDatiFamFiglioModel();
            List<AltriDatiFamFiglioModel> ladffm = new List<AltriDatiFamFiglioModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var f = db.FIGLI.Find(idFiglioOld);
                    if (f?.IDFIGLI > 0)
                    {
                        var ladff = f.ALTRIDATIFAM.Where(x => x.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(x => x.IDALTRIDATIFAM).ToList();

                        if (ladff?.Any() ?? false)
                        {
                            var adff = ladff.First();

                            adffm = new AltriDatiFamFiglioModel()
                            {
                                idAltriDatiFam = adff.IDALTRIDATIFAM,
                                nazionalita = adff.NAZIONALITA,
                                dataNascita = adff.DATANASCITA,
                                comuneNascita = adff.COMUNENASCITA,
                                capNascita = adff.CAPNASCITA,
                                provinciaNascita = adff.PROVINCIANASCITA,
                                indirizzoResidenza = adff.INDIRIZZORESIDENZA,
                                capResidenza = adff.CAPRESIDENZA,
                                comuneResidenza = adff.COMUNERESIDENZA,
                                provinciaResidenza = adff.PROVINCIARESIDENZA,
                                dataAggiornamento = adff.DATAAGGIORNAMENTO,
                                idStatoRecord = adff.IDSTATORECORD,
                                FK_idAltriDatiFam = adff.FK_IDALTRIDATIFAM
                            };
                            //ladffm.Add(adffm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return adffm;
        }

        public decimal EditVariazioneAltriDatiFamiliariConiuge(AltriDatiFamConiugeModel adfm)
        {
            ALTRIDATIFAM adf_return = new ALTRIDATIFAM();

            const string vConiugeFiglio = "Coniuge";

            using (var db = new ModelDBISE())
            {
                db.Database.BeginTransaction();
                try
                {
                   

                    var adf = db.ALTRIDATIFAM.Find(adfm.idAltriDatiFam);

                    var c = adf.CONIUGE.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDCONIUGE).First(); ;

                    ATTIVAZIONIMAGFAM attmf_aperta = new ATTIVAZIONIMAGFAM();

                    if (adf != null && adf.IDALTRIDATIFAM > 0)
                    {

                        var attmf_rif = this.GetAttivazioneById(adf.IDALTRIDATIFAM, EnumTipoTabella.AltriDatiFamiliari);

                        var attmf = this.GetAttivazioneAperta(attmf_rif.IDMAGGIORAZIONIFAMILIARI);

                        // se non esiste attivazione aperta la creo altrimenti la uso
                        if (attmf == null || attmf.IDATTIVAZIONEMAGFAM == 0)
                        {
                            var new_amf = this.CreaAttivazione(attmf_rif.IDMAGGIORAZIONIFAMILIARI,db);
                            attmf_aperta = new_amf;
                        }
                        else
                        {
                            attmf_aperta = attmf;
                        }

                        if (attmf_aperta.IDATTIVAZIONEMAGFAM != attmf_rif.IDATTIVAZIONEMAGFAM)
                        {
                            decimal idTrasf = attmf_rif.IDMAGGIORAZIONIFAMILIARI;

                            Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione logica altri dati familiari.", "ALTRIDATIFAM", db, idTrasf, adf.IDALTRIDATIFAM);

                            var adfNew = new ALTRIDATIFAM
                            {
                                DATANASCITA = DateTime.MinValue,
                                CAPNASCITA = "VUOTO",
                                COMUNENASCITA = "VUOTO",
                                PROVINCIANASCITA = "VUOTO",
                                NAZIONALITA = adfm.nazionalita,
                                INDIRIZZORESIDENZA = adfm.indirizzoResidenza,
                                CAPRESIDENZA = adfm.capResidenza,
                                COMUNERESIDENZA = adfm.comuneResidenza,
                                PROVINCIARESIDENZA = adfm.provinciaResidenza,
                                DATAAGGIORNAMENTO = DateTime.Now,
                                IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                FK_IDALTRIDATIFAM =adfm.idAltriDatiFam
                            };

                            c.ALTRIDATIFAM.Add(adfNew);

                            if (db.SaveChanges() > 0)
                            {
                                //this.AssociaAltriDatiFamiliariConiuge(adfNew.IDCONIUGE, adfNew.IDALTRIDATIFAM, db);

                                this.AssociaAltriDatiFamiliariAttivazione(adfNew.IDALTRIDATIFAM, attmf_aperta.IDATTIVAZIONEMAGFAM, db);

                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di altri dati familiari (" + vConiugeFiglio + ")", "ALTRIDATIFAM", db, idTrasf, adfNew.IDALTRIDATIFAM);
                                db.Database.CurrentTransaction.Commit();
                                adf_return = adfNew;
                            }
                            else
                            {
                                throw new Exception("L'inserimento del record relativo agli altri dati familiari non è avvenuto.");
                            }
                        }
                        else
                        {
                            adf.NAZIONALITA = adfm.nazionalita;
                            adf.INDIRIZZORESIDENZA = adfm.indirizzoResidenza;
                            adf.CAPRESIDENZA = adfm.capResidenza;
                            adf.COMUNERESIDENZA = adfm.comuneResidenza;
                            adf.PROVINCIARESIDENZA = adfm.provinciaResidenza;
                            adf.DATAAGGIORNAMENTO = DateTime.Now;
                            if (db.SaveChanges() < 0)
                            {
                                throw new Exception("L'aggiornamento della riga relativa agli altri dati familiari non è avvenuta.");
                            }
                            db.Database.CurrentTransaction.Commit();
                            adf_return = adf;
                        }
                    }
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
            return adf_return.IDALTRIDATIFAM;
        }

        public void EditVariazioneAltriDatiFamiliariFiglio(AltriDatiFamFiglioModel adfm)
        {
            const string vConiugeFiglio = "Figlio";

            using (var db = new ModelDBISE())
            {
                db.Database.BeginTransaction();
                try
                {

                    var adf = db.ALTRIDATIFAM.Find(adfm.idAltriDatiFam);
                    var f = db.FIGLI.Find(adfm.idFigli);

                    //var f = adf.FIGLI.Where(a=>a.IDSTATORECORD!=(decimal)EnumStatoRecord.Annullato).OrderByDescending(a=>a.IDFIGLI).First();


                    ATTIVAZIONIMAGFAM attmf_aperta = new ATTIVAZIONIMAGFAM();

                    if (adf != null && adf.IDALTRIDATIFAM > 0)
                    {
                        //var attmf_rif = this.GetAttivazioneById(f.IDFIGLI, EnumTipoTabella.Figli);
                        var attmf_rif = this.GetAttivazioneById(adfm.idAltriDatiFam, EnumTipoTabella.AltriDatiFamiliari);

                        var attmf = this.GetAttivazioneAperta(attmf_rif.IDMAGGIORAZIONIFAMILIARI);

                        // se non esiste attivazione aperta la creo altrimenti la uso
                        if (attmf == null || attmf.IDATTIVAZIONEMAGFAM == 0)
                        {
                            var new_amf = this.CreaAttivazione(attmf_rif.IDMAGGIORAZIONIFAMILIARI,db);
                            attmf_aperta = new_amf;

                        }
                        else
                        {
                            attmf_aperta = attmf;
                        }

                        //if ((!(adf.FK_IDALTRIDATIFAM > 0)) && attmf_aperta.IDATTIVAZIONEMAGFAM != attmf_rif.IDATTIVAZIONEMAGFAM)
                        if (attmf_aperta.IDATTIVAZIONEMAGFAM != attmf_rif.IDATTIVAZIONEMAGFAM)
                        {
                            decimal idTrasf = attmf_rif.IDMAGGIORAZIONIFAMILIARI;

                            Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione logica altri dati familiari.", "ALTRIDATIFAM", db, idTrasf, adf.IDALTRIDATIFAM);

                            var adfNew = new ALTRIDATIFAM
                            {
                                DATANASCITA = adfm.dataNascita.Value,
                                CAPNASCITA = adfm.capNascita,
                                COMUNENASCITA = adfm.comuneNascita,
                                PROVINCIANASCITA = adfm.provinciaNascita,
                                NAZIONALITA = adfm.nazionalita,
                                INDIRIZZORESIDENZA = adfm.indirizzoResidenza,
                                CAPRESIDENZA = adfm.capResidenza,
                                COMUNERESIDENZA = adfm.comuneResidenza,
                                PROVINCIARESIDENZA = adfm.provinciaResidenza,
                                DATAAGGIORNAMENTO = DateTime.Now,
                                IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                FK_IDALTRIDATIFAM=adfm.idAltriDatiFam
                            };

                            f.ALTRIDATIFAM.Add(adfNew);

                            if (db.SaveChanges() > 0)
                            {
                                //this.AssociaAltriDatiFamiliariFiglio(adfNew.IDFIGLI, adfNew.IDALTRIDATIFAM, db);

                                this.AssociaAltriDatiFamiliariAttivazione(adfNew.IDALTRIDATIFAM, attmf_aperta.IDATTIVAZIONEMAGFAM, db);

                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di altri dati familiari (" + vConiugeFiglio + ")", "ALTRIDATIFAM", db, idTrasf, adfNew.IDALTRIDATIFAM);
                                db.Database.CurrentTransaction.Commit();
                            }
                            else
                            {
                                throw new Exception("L'inserimento del record relativo agli altri dati familiari non è avvenuto.");
                            }
                        }
                        else
                        {
                            adf.DATANASCITA = adfm.dataNascita.Value;
                            adf.CAPNASCITA = adfm.capNascita;
                            adf.COMUNENASCITA = adfm.comuneNascita;
                            adf.PROVINCIANASCITA = adfm.provinciaNascita;
                            adf.NAZIONALITA = adfm.nazionalita;
                            adf.INDIRIZZORESIDENZA = adfm.indirizzoResidenza;
                            adf.CAPRESIDENZA = adfm.capResidenza;
                            adf.COMUNERESIDENZA = adfm.comuneResidenza;
                            adf.PROVINCIARESIDENZA = adfm.provinciaResidenza;
                            adf.DATAAGGIORNAMENTO = DateTime.Now;
                            adf.IDSTATORECORD = adfm.idStatoRecord;

                            if (db.SaveChanges() < 0)
                            {
                                throw new Exception("L'aggiornamento della riga relativa agli altri dati familiari non è avvenuta.");
                            }
                            db.Database.CurrentTransaction.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }


        public void AssociaAltriDatiFamiliariConiuge(decimal? idConiuge, decimal idAltriDatiFamiliari, ModelDBISE db)
        {
            try
            {
                var c = db.CONIUGE.Find(idConiuge);
                var item = db.Entry<CONIUGE>(c);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.ALTRIDATIFAM).Load();
                var d = db.ALTRIDATIFAM.Find(idAltriDatiFamiliari);
                c.ALTRIDATIFAM.Add(d);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare altri dati familiari per il coniuge. {0}", c.COGNOME + " " + c.NOME));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaAltriDatiFamiliariFiglio(decimal idFiglio, decimal idAltriDatiFamiliari, ModelDBISE db)
        {
            try
            {
                var f = db.FIGLI.Find(idFiglio);
                var item = db.Entry<FIGLI>(f);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.ALTRIDATIFAM).Load();
                var adf = db.ALTRIDATIFAM.Find(idAltriDatiFamiliari);
                f.ALTRIDATIFAM.Add(adf);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare altri dati familiari per il figlio. {0}", f.COGNOME + " " + f.NOME));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public void AssociaAltriDatiFamiliariAttivazione(decimal idAltriDatiFamiliari, decimal idAttivazioneMagFam, ModelDBISE db)
        {
            try
            {
                var c = db.ALTRIDATIFAM.Find(idAltriDatiFamiliari);
                var item = db.Entry<ALTRIDATIFAM>(c);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.ATTIVAZIONIMAGFAM).Load();
                var d = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);
                c.ATTIVAZIONIMAGFAM.Add(d);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare altri dati familiari alla attivazione maggiorazione familiare."));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public void DeleteDocumentoMagFam(decimal idDocumento, EnumChiamante chiamante)
        {
            TRASFERIMENTO t = new TRASFERIMENTO();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var d = db.DOCUMENTI.Find(idDocumento);
                    var doc = d;

                    switch ((EnumTipoDoc)d.IDTIPODOCUMENTO)
                    {
                        case EnumTipoDoc.Formulario_Maggiorazioni_Familiari:
                            t = d.ATTIVAZIONIMAGFAM.First().MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;
                            break;

                        case EnumTipoDoc.Documento_Identita:
                            switch (chiamante)
                            {
                                case EnumChiamante.Variazione_Maggiorazioni_Familiari:
                                    var lc = d.CONIUGE;
                                    if (lc?.Any() ?? false)
                                    {
                                        t = lc.First().MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;
                                    }
                                    else
                                    {
                                        var lf = d.FIGLI;
                                        if (lf?.Any() ?? false)
                                        {
                                            t = lf.First().MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;
                                        }
                                    }
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException("chiamante");
                            }
                            break;

                        default:
                            t = d.TRASFERIMENTO.OrderByDescending(a => a.IDTRASFERIMENTO).First();
                            break;

                    }

                    if (d != null && d.IDDOCUMENTO > 0)
                    {
                        var a = t.MAGGIORAZIONIFAMILIARI.ATTIVAZIONIMAGFAM.Where(x => x.ANNULLATO == false).OrderByDescending(x => x.IDATTIVAZIONEMAGFAM).First();

                        if (a.ATTIVAZIONEMAGFAM == false && a.RICHIESTAATTIVAZIONE == false)
                        {
                            db.DOCUMENTI.Remove(d);
                        }
                        else
                        {
                            //crea una nuova attivazione
                            ATTIVAZIONIMAGFAM newmf = new ATTIVAZIONIMAGFAM()
                            {
                                IDMAGGIORAZIONIFAMILIARI = a.IDMAGGIORAZIONIFAMILIARI,
                                RICHIESTAATTIVAZIONE = false,
                                DATARICHIESTAATTIVAZIONE = null,
                                ATTIVAZIONEMAGFAM = false,
                                DATAATTIVAZIONEMAGFAM = null,
                                ANNULLATO = false,
                                DATAVARIAZIONE = DateTime.Now,
                                DATAAGGIORNAMENTO = DateTime.Now,
                            };
                            db.ATTIVAZIONIMAGFAM.Add(newmf);

                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception(string.Format("Non è stato possibile effettuare creare una nuova attivazione."));
                            }

                            var idAttivazioneMF_old = a.IDATTIVAZIONEMAGFAM;
                            var idAttivazioneMF_new = newmf.IDATTIVAZIONEMAGFAM;
                            var idConiuge = d.CONIUGE.First().IDCONIUGE;

                            //legge lista documenti
                            var ld = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMF_old).DOCUMENTI;

                            //riassocia tutti i documenti tranne quello da cancellare
                            foreach (var e in ld)
                            {
                                if (e.IDDOCUMENTO != d.IDDOCUMENTO)
                                {
                                    using (dtDocumenti dtd = new dtDocumenti())
                                    {
                                        dtd.AssociaDocumentoConiuge(idConiuge, e.IDDOCUMENTO, db);
                                    }
                                    this.AssociaDocumentoAttivazione(e.IDDOCUMENTO, idAttivazioneMF_new, db);
                                }
                            }

                            var adf = db.CONIUGE.Find(idConiuge).ALTRIDATIFAM.OrderByDescending(x => x.IDALTRIDATIFAM).First();
                            if (adf != null && adf.IDALTRIDATIFAM > 0)
                            {
                                this.AssociaADF_Attivazione(adf.IDALTRIDATIFAM, idAttivazioneMF_new, db);
                                this.AssociaConiugeAttivazione(idConiuge, idAttivazioneMF_new, db);
                            }
                        }

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception(string.Format("Non è stato possibile effettuare l'eliminazione del documento ({0}).", d.NOMEDOCUMENTO + d.ESTENSIONE));
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione di un documento (" + ((EnumTipoDoc)d.IDTIPODOCUMENTO).ToString() + ").", "Documenti", db, t.IDTRASFERIMENTO, d.IDDOCUMENTO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AddDocumentoFromConiuge(ref VariazioneDocumentiModel dm, decimal idConiuge, ModelDBISE db)
        {
            var c = db.CONIUGE.Find(idConiuge);
            var t = c.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;
            var amf = c.ATTIVAZIONIMAGFAM.OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).First(a => a.ANNULLATO == false);

            if (c != null && c.IDCONIUGE > 0)
            {
                MemoryStream ms = new MemoryStream();
                DOCUMENTI d = new DOCUMENTI();
                dm.file.InputStream.CopyTo(ms);

                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();
                d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;

                c.DOCUMENTI.Add(d);

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (" + dm.tipoDocumento.ToString() + ").", "Documenti", db, t.IDTRASFERIMENTO, dm.idDocumenti);
                }
            }
        }

        public void AddDocumentoFromFiglio(ref VariazioneDocumentiModel dm, decimal idFiglio, ModelDBISE db)
        {
            var f = db.FIGLI.Find(idFiglio);
            var t = f.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;

            if (f != null && f.IDFIGLI > 0)
            {
                MemoryStream ms = new MemoryStream();
                DOCUMENTI d = new DOCUMENTI();
                dm.file.InputStream.CopyTo(ms);

                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();
                d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;

                f.DOCUMENTI.Add(d);

                int i = db.SaveChanges();

                if (i > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (" + dm.tipoDocumento.ToString() + ").", "Documenti", db, t.IDTRASFERIMENTO, dm.idDocumenti);
                }
            }
        }





        public void AssociaDocumentoAttivazione(decimal idAttivazione, decimal idDocumento, ModelDBISE db)
        {
            try
            {
                var att = db.ATTIVAZIONIMAGFAM.Find(idAttivazione);
                var item = db.Entry<ATTIVAZIONIMAGFAM>(att);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.DOCUMENTI).Load();
                var d = db.DOCUMENTI.Find(idDocumento);
                att.DOCUMENTI.Add(d);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il documento all'attivazione"));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AssociaADF_Attivazione(decimal idAttivazione, decimal idAltriDatiFamiliari, ModelDBISE db)
        {
            try
            {
                var att = db.ATTIVAZIONIMAGFAM.Find(idAttivazione);
                var item = db.Entry<ATTIVAZIONIMAGFAM>(att);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.ALTRIDATIFAM).Load();
                var d = db.ALTRIDATIFAM.Find(idAltriDatiFamiliari);
                att.ALTRIDATIFAM.Add(d);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare altri dati familiari all'attivazione"));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AssociaConiugeAttivazione(decimal idAttivazione, decimal idConiuge, ModelDBISE db)
        {
            try
            {
                var att = db.ATTIVAZIONIMAGFAM.Find(idAttivazione);
                var item = db.Entry<ATTIVAZIONIMAGFAM>(att);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.CONIUGE).Load();
                var d = db.CONIUGE.Find(idConiuge);
                att.CONIUGE.Add(d);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il coniuge all'attivazione"));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<VariazioneDocumentiModel> GetDocumentiById(decimal id, EnumTipoDoc tipodoc, EnumParentela parentela = EnumParentela.Richiedente)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                List<DOCUMENTI> ld = new List<DOCUMENTI>();
                ATTIVAZIONIMAGFAM attmf = new ATTIVAZIONIMAGFAM();

                switch (tipodoc)
                {
                    case EnumTipoDoc.Documento_Identita:
                        #region enum documento identita
                        switch (parentela)
                        {
                            case EnumParentela.Coniuge:
                                #region enum_coniuge
                                ld = db.CONIUGE.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc && a.MODIFICATO == false).ToList();

                                bool modificabile;

                                foreach (var e in ld)
                                {
                                    attmf = db.DOCUMENTI.Find(e.IDDOCUMENTO).ATTIVAZIONIMAGFAM.First();

                                    modificabile = false;

                                    if (attmf.RICHIESTAATTIVAZIONE == false && attmf.ATTIVAZIONEMAGFAM == false && e.FK_IDDOCUMENTO == null)
                                    {
                                        modificabile = true;
                                    }

                                    var dm = new VariazioneDocumentiModel()
                                    {
                                        idDocumenti = e.IDDOCUMENTO,
                                        dataInserimento = e.DATAINSERIMENTO,
                                        Modificabile = modificabile,
                                        nomeDocumento = e.NOMEDOCUMENTO,
                                        estensione = e.ESTENSIONE
                                    };
                                    ldm.Add(dm);

                                }
                                #endregion
                                break;

                            case EnumParentela.Figlio:
                                ld = db.FIGLI.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc && ((a.FK_IDDOCUMENTO == null && a.MODIFICATO == false) || (a.FK_IDDOCUMENTO > 0 && a.MODIFICATO == false))).ToList();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("parentela");
                        }
                        #endregion
                        break;

                    case EnumTipoDoc.Formulario_Maggiorazioni_Familiari:
                        ld = db.ATTIVAZIONIMAGFAM.Find(id)
                            .DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc)
                            .OrderByDescending(a => a.DATAINSERIMENTO).ToList();

                        break;

                    default:
                        throw new ArgumentOutOfRangeException("tipodoc");
                }

            }
            return ldm;
        }


        public void AssociaDocumentoConiuge(ref VariazioneDocumentiModel dm, decimal idConiuge, ModelDBISE db)
        {
            var c = db.CONIUGE.Find(idConiuge);
            var t = c.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;

            if (c != null && c.IDCONIUGE > 0)
            {
                MemoryStream ms = new MemoryStream();
                DOCUMENTI d = new DOCUMENTI();
                dm.file.InputStream.CopyTo(ms);

                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();
                d.MODIFICATO = dm.Modificato;
                if (dm.fk_iddocumento > 0)
                {
                    d.FK_IDDOCUMENTO = dm.fk_iddocumento;
                }
                d.IDSTATORECORD = dm.idStatoRecord;

                c.DOCUMENTI.Add(d);

                if (dm.fk_iddocumento > 0)
                {
                    var dm_originale = db.DOCUMENTI.Find(dm.fk_iddocumento);
                    dm_originale.MODIFICATO = true;
                }

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (" + dm.tipoDocumento.ToString() + ").", "Documenti", db, t.IDTRASFERIMENTO, dm.idDocumenti);
                }
                else
                {
                    throw new Exception(string.Format("Non è stato possibile sostituire il documento (Errore in fase di associazione documento-coniuge."));
                }
            }
        }

        public void AssociaDocumentoFiglio(ref VariazioneDocumentiModel dm, decimal idFiglio, ModelDBISE db)
        {
            var f = db.FIGLI.Find(idFiglio);
            var t = f.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;

            if (f != null && f.IDFIGLI > 0)
            {
                MemoryStream ms = new MemoryStream();
                DOCUMENTI d = new DOCUMENTI();
                dm.file.InputStream.CopyTo(ms);

                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();
                d.MODIFICATO = dm.Modificato;
                if (dm.fk_iddocumento > 0)
                {
                    d.FK_IDDOCUMENTO = dm.fk_iddocumento;
                }
                d.IDSTATORECORD = dm.idStatoRecord;

                f.DOCUMENTI.Add(d);

                if (dm.fk_iddocumento > 0)
                {
                    var dm_originale = db.DOCUMENTI.Find(dm.fk_iddocumento);
                    dm_originale.MODIFICATO = true;
                }

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (" + dm.tipoDocumento.ToString() + ").", "Documenti", db, t.IDTRASFERIMENTO, dm.idDocumenti);
                }
                else
                {
                    throw new Exception(string.Format("Non è stato possibile sostituire il documento (Errore in fase di associazione documento-figlio."));
                }
            }
        }

        public ATTIVAZIONIMAGFAM GetAttivazioneById(decimal IdChiamante, EnumTipoTabella TabellaChiamante)
        {
            using (var db = new ModelDBISE())
            {
                ATTIVAZIONIMAGFAM attmf = new ATTIVAZIONIMAGFAM();

                switch (TabellaChiamante)
                {
                    case EnumTipoTabella.Documenti:
                        var d = db.DOCUMENTI.Find(IdChiamante);
                        attmf = d.ATTIVAZIONIMAGFAM.Where(x => x.ANNULLATO == false).OrderByDescending(x => x.IDATTIVAZIONEMAGFAM).First();
                        break;

                    case EnumTipoTabella.Coniuge:
                        var c = db.CONIUGE.Find(IdChiamante);
                        attmf = c.ATTIVAZIONIMAGFAM.Where(x => x.ANNULLATO == false).OrderByDescending(x => x.IDATTIVAZIONEMAGFAM).First();
                        break;

                    case EnumTipoTabella.Figli:
                        var f = db.FIGLI.Find(IdChiamante);
                        attmf = f.ATTIVAZIONIMAGFAM.Where(x => x.ANNULLATO == false).OrderByDescending(x => x.IDATTIVAZIONEMAGFAM).First();
                        break;

                    case EnumTipoTabella.MaggiorazioniFamiliari:
                        var m = db.MAGGIORAZIONIFAMILIARI.Find(IdChiamante);
                        attmf = m.ATTIVAZIONIMAGFAM.Where(x => x.ANNULLATO == false).OrderByDescending(x => x.IDATTIVAZIONEMAGFAM).First();
                        break;

                    case EnumTipoTabella.AltriDatiFamiliari:
                        var a = db.ALTRIDATIFAM.Find(IdChiamante);
                        attmf = a.ATTIVAZIONIMAGFAM.Where(x => x.ANNULLATO == false).OrderByDescending(x => x.IDATTIVAZIONEMAGFAM).First();
                        break;

                    case EnumTipoTabella.Pensione:
                        var p = db.PENSIONE.Find(IdChiamante);
                        attmf = p.ATTIVAZIONIMAGFAM.Where(x => x.ANNULLATO == false).OrderByDescending(x => x.IDATTIVAZIONEMAGFAM).First();
                        break;
                }
                return attmf;
            }
        }

        public ATTIVAZIONIMAGFAM GetAttivazioneById(decimal IdChiamante, EnumTipoTabella TabellaChiamante, ModelDBISE db)
        {
            ATTIVAZIONIMAGFAM attmf = new ATTIVAZIONIMAGFAM();

            switch (TabellaChiamante)
            {
                case EnumTipoTabella.Documenti:
                    var d = db.DOCUMENTI.Find(IdChiamante);
                    attmf = d.ATTIVAZIONIMAGFAM.Where(x => x.ANNULLATO == false).OrderByDescending(x => x.IDATTIVAZIONEMAGFAM).First();
                    break;

                case EnumTipoTabella.Coniuge:
                    var c = db.CONIUGE.Find(IdChiamante);
                    attmf = c.ATTIVAZIONIMAGFAM.Where(x => x.ANNULLATO == false).OrderByDescending(x => x.IDATTIVAZIONEMAGFAM).First();
                    break;

                case EnumTipoTabella.Figli:
                    var f = db.FIGLI.Find(IdChiamante);
                    attmf = f.ATTIVAZIONIMAGFAM.Where(x => x.ANNULLATO == false).OrderByDescending(x => x.IDATTIVAZIONEMAGFAM).First();
                    break;

                case EnumTipoTabella.MaggiorazioniFamiliari:
                    var m = db.MAGGIORAZIONIFAMILIARI.Find(IdChiamante);
                    attmf = m.ATTIVAZIONIMAGFAM.Where(x => x.ANNULLATO == false).OrderByDescending(x => x.IDATTIVAZIONEMAGFAM).First();
                    break;

                case EnumTipoTabella.AltriDatiFamiliari:
                    var a = db.ALTRIDATIFAM.Find(IdChiamante);
                    attmf = a.ATTIVAZIONIMAGFAM.Where(x => x.ANNULLATO == false).OrderByDescending(x => x.IDATTIVAZIONEMAGFAM).First();
                    break;

                case EnumTipoTabella.Pensione:
                    var p = db.PENSIONE.Find(IdChiamante);
                    attmf = p.ATTIVAZIONIMAGFAM.Where(x => x.ANNULLATO == false).OrderByDescending(x => x.IDATTIVAZIONEMAGFAM).First();
                    break;
            }
            return attmf;
        }

        public decimal GetIdTrasferimento(decimal idMaggiorazioniFamiliari)
        {
            decimal idTrasferimento = 0;

            using (var db = new ModelDBISE())
            {
                idTrasferimento = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari).TRASFERIMENTO.IDTRASFERIMENTO;
            }
            return idTrasferimento;
        }

        public decimal GetMaggiorazioneFamiliareConiuge(decimal idConiuge)
        {
            decimal idMaggiorazioniFamiliari = 0;

            using (var db = new ModelDBISE())
            {
                var c = db.CONIUGE.Find(idConiuge);
                idMaggiorazioniFamiliari = c.IDMAGGIORAZIONIFAMILIARI;
            }
            return idMaggiorazioniFamiliari;
        }

        public decimal GetMaggiorazioneFamiliareFiglio(decimal idFiglio)
        {
            decimal idMaggiorazioniFamiliari = 0;

            using (var db = new ModelDBISE())
            {
                var f = db.FIGLI.Find(idFiglio);
                idMaggiorazioniFamiliari = f.MAGGIORAZIONIFAMILIARI.IDMAGGIORAZIONIFAMILIARI;
            }
            return idMaggiorazioniFamiliari;
        }

        public decimal GetMaggiorazioneFamiliareDocumento(decimal idDocumento)
        {
            decimal idMaggiorazioniFamiliari = 0;

            using (var db = new ModelDBISE())
            {
                var d = db.DOCUMENTI.Find(idDocumento).ATTIVAZIONIMAGFAM.First();
                idMaggiorazioniFamiliari = d.IDMAGGIORAZIONIFAMILIARI;
            }
            return idMaggiorazioniFamiliari;
        }


        public ATTIVAZIONIMAGFAM GetAttivazioneAperta(decimal IdMaggiorazioneFamiliare)
        {
            using (var db = new ModelDBISE())
            {
                List<ATTIVAZIONIMAGFAM> lattmf = new List<ATTIVAZIONIMAGFAM>();
                ATTIVAZIONIMAGFAM attmf = new ATTIVAZIONIMAGFAM();
                var mf = db.MAGGIORAZIONIFAMILIARI.Find(IdMaggiorazioneFamiliare);

                lattmf = mf.ATTIVAZIONIMAGFAM.Where(x => x.ANNULLATO == false && x.RICHIESTAATTIVAZIONE == false && x.ATTIVAZIONEMAGFAM == false).OrderByDescending(x=>x.IDATTIVAZIONEMAGFAM).ToList();
                if (lattmf?.Any() ?? false)
                {
                    attmf = lattmf.First();
                }
                return attmf;
            }
        }

        public ATTIVAZIONIMAGFAM CreaAttivazione(decimal idMaggiorazioniFamiliari, ModelDBISE db)
        {
            ATTIVAZIONIMAGFAM new_amf = new ATTIVAZIONIMAGFAM()
            {
                IDMAGGIORAZIONIFAMILIARI = idMaggiorazioniFamiliari,
                RICHIESTAATTIVAZIONE = false,
                DATARICHIESTAATTIVAZIONE = null,
                ATTIVAZIONEMAGFAM = false,
                DATAATTIVAZIONEMAGFAM = null,
                ANNULLATO = false,
                DATAVARIAZIONE = DateTime.Now,
                DATAAGGIORNAMENTO = DateTime.Now,
            };
            db.ATTIVAZIONIMAGFAM.Add(new_amf);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception(string.Format("Non è stato possibile creare una nuova attivazione."));
            }
            return new_amf;
        }

        public IList<VariazioneConiugeModel> GetListaAttivazioniConiugeByIdMagFam(decimal idMaggiorazioniFamiliari)
        {
            List<VariazioneConiugeModel> lcm = new List<VariazioneConiugeModel>();
            List<CONIUGE> lc = new List<CONIUGE>();
            CONIUGE c = new CONIUGE();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                var lamf = mf.ATTIVAZIONIMAGFAM.Where(e => e.ANNULLATO == false)
                                               .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                bool modificabile = false;

                if (lamf?.Any() ?? false)
                {
                    foreach (var x in lamf)
                    {
                        lc = x.CONIUGE.ToList();

                        if (lc?.Any() ?? false)
                        {
                            foreach (var e in lc)
                            {
                                VariazioneConiugeModel cm = new VariazioneConiugeModel()
                                {
                                    modificabile = modificabile,
                                    idConiuge = e.IDCONIUGE,
                                    idMaggiorazioniFamiliari = e.IDMAGGIORAZIONIFAMILIARI,
                                    idTipologiaConiuge = (EnumTipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                                    nome = e.NOME,
                                    cognome = e.COGNOME,
                                    codiceFiscale = e.CODICEFISCALE,
                                    dataInizio = e.DATAINIZIOVALIDITA,
                                    dataFine = e.DATAFINEVALIDITA,
                                    dataAggiornamento = e.DATAAGGIORNAMENTO,
                                };
                                lcm.Add(cm);
                                break;
                            }
                        }
                    }
                }
            }
            return lcm;
        }

        public List<PensioneConiugeModel> GetListaPensioniConiugeByIdMagFam(decimal idConiuge)
        {
            List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();
            List<PENSIONE> lp = new List<PENSIONE>();
            CONIUGE c = new CONIUGE();
            ATTIVAZIONIMAGFAM amf = new ATTIVAZIONIMAGFAM();

            using (ModelDBISE db = new ModelDBISE())
            {
                c = db.CONIUGE.Find(idConiuge);

                //amf = GetAttivazioneAperta(c.IDMAGGIORAZIONIFAMILIARI);
                //if(amf.IDATTIVAZIONEMAGFAM>0==false)
                //{
                //    amf = db.MAGGIORAZIONIFAMILIARI.Find(c.IDMAGGIORAZIONIFAMILIARI).ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList().First();
                //}

                //if (amf.RICHIESTAATTIVAZIONE == false)
                //{
                lp = c.PENSIONE.Where(x => x.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                            x.NASCONDI==false).OrderByDescending(a=>a.IDPENSIONE).ToList();
                //    if (lp.Count() > 0 == false)
                //    {
                //        lp = c.PENSIONE.Where(x => x.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                //    }
                //}
                //else if (amf.ATTIVAZIONEMAGFAM == false)
                //{
                //    lp = c.PENSIONE.Where(x => x.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                //}
                //else
                //{
                //    lp = c.PENSIONE.Where(x => x.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                //}


                //                lp = c.PENSIONE.Where(x => x.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();

                //if (lp.Count == 0)
                //{
                //    lp = c.PENSIONE.Where(x => x.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                //}

                if (lp?.Any() ?? false)
                {
                    foreach (var p in lp)
                    {
                        PensioneConiugeModel pcm = new PensioneConiugeModel()
                        {
                            #region variabili
                            idPensioneConiuge = p.IDPENSIONE,
                            importoPensione = p.IMPORTOPENSIONE,
                            dataInizioValidita = p.DATAINIZIO,
                            dataFineValidita = p.DATAFINE,
                            idStatoRecord = p.IDSTATORECORD,
                            dataAggiornamento = p.DATAAGGIORNAMENTO
                            #endregion
                        };
                        lpcm.Add(pcm);
                    }
                }
            }
            return lpcm;
        }

        public List<PensioneConiugeModel> GetListaPensioniPrecedentiConiugeByIdMagFam(decimal idConiuge)
        {
            List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();
            List<PENSIONE> lp = new List<PENSIONE>();
            CONIUGE c = new CONIUGE();
            ATTIVAZIONIMAGFAM amf = new ATTIVAZIONIMAGFAM();

            using (ModelDBISE db = new ModelDBISE())
            {
                c = db.CONIUGE.Find(idConiuge);

                lp = c.PENSIONE.Where(x => x.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();

                if (lp?.Any() ?? false)
                {
                    foreach (var p in lp)
                    {
                        PensioneConiugeModel pcm = new PensioneConiugeModel()
                        {
                            #region variabili
                            idPensioneConiuge = p.IDPENSIONE,
                            importoPensione = p.IMPORTOPENSIONE,
                            dataInizioValidita = p.DATAINIZIO,
                            dataFineValidita = p.DATAFINE,
                            idStatoRecord = p.IDSTATORECORD,
                            dataAggiornamento = p.DATAAGGIORNAMENTO
                            #endregion
                        };
                        lpcm.Add(pcm);
                    }
                }
            }
            return lpcm;
        }



        public IList<VariazioneDocumentiModel> GetDocumentiPrecedenti(decimal? idFamiliare, EnumParentela parentela = EnumParentela.Richiedente, decimal idMaggiorazioniFamiliari = 0)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                List<DOCUMENTI> ld = new List<DOCUMENTI>();
                List<DOCUMENTI> ld_now = new List<DOCUMENTI>();
                List<DOCUMENTI> ld_inlavoraz = new List<DOCUMENTI>();

                string evidenzia = ";border-bottom:solid; border-bottom-color:yellow";

                var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);
                var lamf = mf.ATTIVAZIONIMAGFAM.Where(e => e.ANNULLATO == false && e.ATTIVAZIONEMAGFAM).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();
                switch (parentela)
                {
                    case EnumParentela.Coniuge:
                        var c = db.CONIUGE.Find(idFamiliare);

                        ld = c.DOCUMENTI.Where(a =>
                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                            .OrderByDescending(a => a.IDDOCUMENTO).ToList();
                        break;

                    case EnumParentela.Figlio:
                        var f = db.FIGLI.Find(idFamiliare);
                        ld = f.DOCUMENTI.Where(a =>
                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                        .OrderByDescending(a => a.IDDOCUMENTO).ToList();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("parentela");
                }

                if (ld?.Any() ?? false)
                {
                    foreach (var d in ld)
                    {
                        if(db.DOCUMENTI.Where(a =>a.FK_IDDOCUMENTO == d.IDDOCUMENTO && a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).Count() == 0)
                        {

                            var f = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO);

                            //bool rich = false;
                            //var latt = d.ATTIVAZIONIMAGFAM;
                            //if (latt?.Any()??false)
                            //{
                            //    var att = latt.First();
                            //    rich = att.RICHIESTAATTIVAZIONE;
                            //}

                            VariazioneDocumentiModel vdm_new = new VariazioneDocumentiModel()
                            {
                                idDocumenti = d.IDDOCUMENTO,
                                nomeDocumento = d.NOMEDOCUMENTO,
                                estensione = d.ESTENSIONE,
                                tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
                                dataInserimento = d.DATAINSERIMENTO,
                                file = f,
                                sostituito = (d.MODIFICATO) ? true : false
                            };
                            if (vdm_new.sostituito)
                            {
                                vdm_new.ev_nomedocumento = evidenzia;
                            }
                            else
                            {
                                vdm_new.ev_nomedocumento = "";
                            }

                            ldm.Add(vdm_new);
                        }
                    }
                }
            }
            return ldm;
        }


        public IList<VariazioneDocumentiModel> GetDocumentiByIdTable_MF(decimal id, EnumTipoDoc tipodoc, EnumParentela parentela = EnumParentela.Richiedente, decimal idMaggiorazioniFamiliari = 0)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                List<DOCUMENTI> ld = new List<DOCUMENTI>();

                var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);
                var lamf = mf.ATTIVAZIONIMAGFAM.Where(e => e.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                switch (tipodoc)
                {
                    case EnumTipoDoc.Documento_Identita:
                        switch (parentela)
                        {
                            case EnumParentela.Coniuge:
                                var c = db.CONIUGE.Find(id);
                                ld = c.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc && a.IDSTATORECORD!=(decimal)EnumStatoRecord.Annullato && a.MODIFICATO == false).OrderByDescending(a => a.IDDOCUMENTO).ToList();
                                break;
                            case EnumParentela.Figlio:
                                var f = db.FIGLI.Find(id);
                                ld = f.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc && a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.MODIFICATO == false).OrderByDescending(a => a.IDDOCUMENTO).ToList();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("parentela");
                        }
                        break;
                    case EnumTipoDoc.Formulario_Maggiorazioni_Familiari:
                        ld = db.ATTIVAZIONIMAGFAM.Find(id)
                            .DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc && a.MODIFICATO == false && a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                            .OrderByDescending(a => a.DATAINSERIMENTO).ToList();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("tipodoc");
                }

                if (ld?.Any() ?? false)
                {
                    foreach (var d in ld)
                    {
                        var f = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO);

                        bool rich = false;
                        var latt = d.ATTIVAZIONIMAGFAM;
                        if (latt?.Any() ?? false)
                        {
                            var att = latt.First();
                            rich = att.RICHIESTAATTIVAZIONE;
                        }

                        VariazioneDocumentiModel vdm_new = new VariazioneDocumentiModel()
                        {
                            idDocumenti = d.IDDOCUMENTO,
                            nomeDocumento = d.NOMEDOCUMENTO,
                            estensione = d.ESTENSIONE,
                            tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
                            dataInserimento = d.DATAINSERIMENTO,
                            file = f,
                            Modificabile = (d.MODIFICATO == false && d.FK_IDDOCUMENTO == null && rich == false) ? true : false
                        };
                        ldm.Add(vdm_new);
                        //ldm.AddRange(from d in ld
                        //             let f = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO)
                        //             select new VariazioneDocumentiModel()
                        //             {
                        //                 idDocumenti = d.IDDOCUMENTO,
                        //                 nomeDocumento = d.NOMEDOCUMENTO,
                        //                 estensione = d.ESTENSIONE,
                        //                 tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
                        //                 dataInserimento = d.DATAINSERIMENTO,
                        //                 file = f,
                        //                 Modificabile = (d.MODIFICATO == false && d.FK_IDDOCUMENTO == null && d.ATTIVAZIONIMAGFAM.First().RICHIESTAATTIVAZIONE == false) ? true : false
                        //             });
                    }
                }
            }
            return ldm;
        }


        public void InserisciConiugeVarMagFam(ConiugeModel cm, decimal idMaggiorazioniFamiliari, decimal idAttivazioneMagFam)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var tm = dtt.GetTrasferimentoByIdAttMagFam(cm.idAttivazioneMagFam);
                    }

                    if (cm.idMaggiorazioniFamiliari == 0 && cm.idAttivazioneMagFam > 0)
                    {
                        var amf = db.ATTIVAZIONIMAGFAM.Find(cm.idAttivazioneMagFam);
                        cm.idMaggiorazioniFamiliari = amf.IDMAGGIORAZIONIFAMILIARI;
                    }

                    using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                    {
                        cm.dataAggiornamento = DateTime.Now;

                        decimal new_idconiuge = dtvmf.SetConiuge(ref cm, db, cm.idAttivazioneMagFam);

                        using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                        {
                            DateTime dtIni = cm.dataInizio.Value;
                            DateTime dtFin = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop();

                            List<PercentualeMagConiugeModel> lpmcm =
                                dtpc.GetListaPercentualiMagConiugeByRangeDate(cm.idTipologiaConiuge, dtIni, dtFin, db)
                                    .ToList();

                            if (lpmcm?.Any() ?? false)
                            {
                                foreach (var pmcm in lpmcm)
                                {
                                    dtpc.AssociaPercentualeMaggiorazioneConiuge(new_idconiuge, pmcm.idPercentualeConiuge, db);
                                }
                            }
                            else
                            {
                                throw new Exception("Non è presente nessuna percentuale del coniuge.");
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

        public void InserisciFiglioVarMagFam(FigliModel fm, decimal idMaggiorazioniFamiliari, decimal idAttivazioneMagFam)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {

                    if (fm.idMaggiorazioniFamiliari == 0 && fm.idAttivazioneMagFam > 0)
                    {
                        var amf = db.ATTIVAZIONIMAGFAM.Find(fm.idAttivazioneMagFam);
                        fm.idMaggiorazioniFamiliari = amf.IDMAGGIORAZIONIFAMILIARI;
                    }

                    using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                    {
                        fm.dataAggiornamento = DateTime.Now;

                        decimal new_idfiglio = dtvmf.SetFiglio(ref fm, db, fm.idAttivazioneMagFam);

                        using (dtPercentualeMagFigli dtpmf = new dtPercentualeMagFigli())
                        {
                            DateTime dtIni = fm.dataInizio.Value;
                            DateTime dtFin = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop();

                            IList<PercentualeMagFigliModel> lpmfm = dtpmf.GetPercentualeMaggiorazioneFigli((EnumTipologiaFiglio)fm.idTipologiaFiglio, dtIni, dtFin, db);

                            if (lpmfm?.Any() ?? false)
                            {
                                foreach (var pmfm in lpmfm)
                                {
                                    dtpmf.AssociaPercentualeMaggiorazioneFigli(new_idfiglio, pmfm.idPercMagFigli, db);
                                }
                            }
                            else
                            {
                                throw new Exception("Non è presente nessuna percentuale del figlio.");
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

        public void Associa_Doc_Coniuge_ById(decimal idDocumento, decimal idConiuge, ModelDBISE db)
        {
            try
            {
                var d = db.DOCUMENTI.Find(idDocumento);
                var item = db.Entry<DOCUMENTI>(d);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.CONIUGE).Load();
                var c = db.CONIUGE.Find(idConiuge);
                d.CONIUGE.Add(c);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare documenti coniuge al coniuge."));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Associa_Doc_Figlio_ById(decimal idDocumento, decimal idFiglio, ModelDBISE db)
        {
            try
            {
                var d = db.DOCUMENTI.Find(idDocumento);
                var item = db.Entry<DOCUMENTI>(d);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.FIGLI).Load();
                var f = db.FIGLI.Find(idFiglio);
                d.FIGLI.Add(f);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare documenti figlio al figlio."));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Associa_Pensioni_Coniuge_ById(decimal idPensione, decimal idConiuge, ModelDBISE db)
        {
            try
            {
                var p = db.PENSIONE.Find(idPensione);
                var item = db.Entry<PENSIONE>(p);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.CONIUGE).Load();
                var c = db.CONIUGE.Find(idConiuge);
                p.CONIUGE.Add(c);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare documenti coniuge al coniuge."));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void EliminaConiuge(decimal idConiuge)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {

                    var c = db.CONIUGE.Find(idConiuge);
                    if (c != null && c.IDCONIUGE > 0)
                    {
                        //elimino eventuali documenti
                        var ldc = c.DOCUMENTI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                        foreach (var dc in ldc)
                        {
                            db.DOCUMENTI.Remove(dc);
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception(string.Format("Impossibile eliminare i documenti del coniuge."));
                            }
                        }

                        db.CONIUGE.Remove(c);

                        int i = db.SaveChanges();

                        if (i <= 0)
                        {
                            throw new Exception(string.Format("Impossibile eliminare il coniuge."));
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
        public void AnnullaModConiuge(decimal idConiuge)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var c = db.CONIUGE.Find(idConiuge);
                        if (c != null && c.IDCONIUGE > 0)
                        {
                            //elimino eventuali modifiche adf
                            var ladfc = c.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                            foreach (var adfc in ladfc)
                            {
                                db.ALTRIDATIFAM.Remove(adfc);
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception(string.Format("Impossibile annullare le modifiche Altri Dati Familiari del coniuge."));
                                }
                            }

                            //cerco eventuali documenti sostituiti e rimetto il flag modificato a FALSE
                            var ldc_sost = c.DOCUMENTI.Where(a => a.FK_IDDOCUMENTO > 0 && a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                            //var ldc_sost = c.DOCUMENTI.Where(a => a.MODIFICATO == true && a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                            foreach (var dc_sost in ldc_sost)
                            {
                                var d = db.DOCUMENTI.Find(dc_sost.FK_IDDOCUMENTO);
                                d.MODIFICATO = false;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception(string.Format("Impossibile annullare le modifiche relative ai documenti del coniuge."));
                                }
                            }

                            //elimino eventuali documenti in lavorazione
                            var ldc = c.DOCUMENTI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                            foreach (var dc in ldc)
                            {
                                db.DOCUMENTI.Remove(dc);
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception(string.Format("Impossibile annullare i documenti del coniuge."));
                                }
                            }

                            //elimino eventuali pensioni in lavorazione
                            var lpc = c.PENSIONE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                            foreach (var pc in lpc)
                            {
                                db.PENSIONE.Remove(pc);
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception(string.Format("Impossibile annullare le pensioni del coniuge."));
                                }
                            }
                        }

                        var idMaggiorazioneFamiliare = GetMaggiorazioneFamiliareConiuge(c.IDCONIUGE);
                        var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioneFamiliare);

                        //elimino tutti i record dei coniugi in lavorazione
                        foreach (var coniuge in mf.CONIUGE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList())
                        {
                            db.CONIUGE.Remove(coniuge);
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception(string.Format("Impossibile annullare tutte le modifiche del coniuge."));
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void EliminaFiglio(decimal idFiglio)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {

                    var f = db.FIGLI.Find(idFiglio);
                    if (f != null && f.IDFIGLI > 0)
                    {
                        //elimino eventuali documenti
                        var ldf = f.DOCUMENTI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                        foreach (var df in ldf)
                        {
                            db.DOCUMENTI.Remove(df);
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception(string.Format("Impossibile eliminare i documenti del figlio."));
                            }
                        }

                        db.FIGLI.Remove(f);
                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception(string.Format("Impossibile eliminare il figlio."));
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
        public void AnnullaModFiglio(decimal idFiglio)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {

                        var f = db.FIGLI.Find(idFiglio);
                        if (f != null && f.IDFIGLI > 0)
                        {
                            //elimino eventuali modifiche adf
                            var ladff = f.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                            foreach (var adff in ladff)
                            {
                                db.ALTRIDATIFAM.Remove(adff);
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception(string.Format("Impossibile annullare le modifiche Altri Dati Familiari del figlio."));
                                }
                            }

                            //cerco eventuali documenti sostituiti e rimetto il flag modificato a FALSE
                            var ldf_sost = f.DOCUMENTI.Where(a => a.FK_IDDOCUMENTO > 0 && a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                            //var ldf_sost = f.DOCUMENTI.Where(a => a.MODIFICATO==true && a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                            foreach (var df_sost in ldf_sost)
                            {
                                var d= db.DOCUMENTI.Find(df_sost.FK_IDDOCUMENTO);
                                d.MODIFICATO = false;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception(string.Format("Impossibile annullare le modifiche relative ai documenti del figlio."));
                                }
                            }


                            //elimino eventuali documenti
                            var ldf = f.DOCUMENTI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                            foreach (var df in ldf)
                            {
                                db.DOCUMENTI.Remove(df);
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception(string.Format("Impossibile annullare i documenti del figlio."));
                                }
                            }

                            //se il figlio è stato modificato elimino il record
                            if (f.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione)
                            {
                                //se è collegato a un altro record lo elimino direttamente 
                                //eliminando anche tutti gli eventuali dati collegati
                                db.FIGLI.Remove(f);
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception(string.Format("Impossibile annullare le modifiche del figlio."));
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


            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AttivaRichiestaVariazione(decimal idAttivazioneMagFam, decimal idMaggiorazioneFamiliare)
        {
            int i = 0;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

                        amf.ATTIVAZIONEMAGFAM = true;
                        amf.DATAATTIVAZIONEMAGFAM = DateTime.Now;

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore nella fase di attivazione delle maggiorazioni familiari.");
                        }

                        //cerca coniuge da attivare e lo mette attivato
                        var lc = amf.CONIUGE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                        foreach (var c in lc)
                        {
                            c.IDSTATORECORD = (decimal)EnumStatoRecord.Attivato;
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore in fase di attivazione delle maggiorazioni familiari (coniuge).");
                            }
                            //se deriva da un altro coniuge annullo il precedente
                            if(c.FK_IDCONIUGE>0)
                            {
                                var cold = db.CONIUGE.Find(c.FK_IDCONIUGE);
                                if(cold.IDCONIUGE>0)
                                {
                                    cold.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                                    if(db.SaveChanges()<=0)
                                    {
                                        throw new Exception("Errore in fase di attivazione delle maggiorazioni familiari (annullamento precedente record coniuge).");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Errore in fase di attivazione delle maggiorazioni familiari (lettura precedente record coniuge).");

                                }
                            }

                        }
                        //cerca figlio da attivare e lo mette attivato
                        var lf = amf.FIGLI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                        foreach (var f in lf)
                        {
                            f.IDSTATORECORD = (decimal)EnumStatoRecord.Attivato;
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore in fase di attivazione delle maggiorazioni familiari (figli).");
                            }
                            //se deriva da un altro figlio annullo il precedente
                            if (f.FK_IDFIGLI > 0)
                            {
                                var fold = db.FIGLI.Find(f.FK_IDFIGLI);
                                if (fold.IDFIGLI > 0)
                                {
                                    fold.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore in fase di attivazione delle maggiorazioni familiari (annullamento precedente record figlio).");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Errore in fase di attivazione delle maggiorazioni familiari (lettura precedente record figlio).");
                                }
                            }

                        }
                        //cerca documenti da attivare e lo mette attivato
                        var ld = amf.DOCUMENTI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                        foreach (var d in ld)
                        {
                            d.IDSTATORECORD = (decimal)EnumStatoRecord.Attivato;
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore in fase di attivazione delle maggiorazioni familiari (documenti).");
                            }
                            //se deriva da un altro coniuge annullo il precedente
                            if (d.FK_IDDOCUMENTO > 0)
                            {
                                var dold = db.DOCUMENTI.Find(d.FK_IDDOCUMENTO);
                                if (dold.IDDOCUMENTO > 0)
                                {
                                    dold.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore in fase di attivazione delle maggiorazioni familiari (annullamento precedente record documento).");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Errore in fase di attivazione delle maggiorazioni familiari (lettura precedente record documenti).");

                                }
                            }

                        }
                        //cerca altri dati da attivare e lo mette attivato
                        var ladf = amf.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                        foreach (var adf in ladf)
                        {
                            adf.IDSTATORECORD = (decimal)EnumStatoRecord.Attivato;
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore in fase di attivazione delle maggiorazioni familiari (altri dati familiari).");
                            }
                            //se deriva da un altro adf annullo il precedente
                            if (adf.FK_IDALTRIDATIFAM > 0)
                            {
                                var adfold = db.ALTRIDATIFAM.Find(adf.FK_IDALTRIDATIFAM);
                                if (adfold.IDALTRIDATIFAM > 0)
                                {
                                    adfold.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore in fase di attivazione delle maggiorazioni familiari (annullamento precedente record altri dati familiari).");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Errore in fase di attivazione delle maggiorazioni familiari (lettura precedente record altri dati familiari).");

                                }
                            }

                        }

                        ////PENSIONI: le pensioni vengono gestite tutte insieme
                        ////          quindi le pensioni attive vengono annullate 
                        ////          e le pensioni da attivare vengono attivate
                        ////
                        ////cerca pensioni coniuge attivate e le annulla
                        //var lp_att = amf.PENSIONE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                        //foreach (var p in lp_att)
                        //{
                        //    p.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                        //    if (db.SaveChanges() <= 0)
                        //    {
                        //        throw new Exception("Errore in fase di attivazione delle maggiorazioni familiari (pensione coniuge).");
                        //    }

                        //}
                        //cerca pensioni coniuge da attivare e le mette attivate
                        var lp = amf.PENSIONE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                        
                        foreach (var p in lp)
                        {
                            p.IDSTATORECORD = (decimal)EnumStatoRecord.Attivato;
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore in fase di attivazione delle maggiorazioni familiari (pensione coniuge).");
                            }
                            //se deriva da un altra pensione annullo il precedente
                            if (p.FK_IDPENSIONE > 0)
                            {
                                var pold = db.PENSIONE.Find(p.FK_IDPENSIONE);
                                if (pold.IDPENSIONE > 0)
                                {
                                    pold.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore in fase di attivazione delle maggiorazioni familiari (annullamento precedente record pensione).");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Errore in fase di attivazione delle maggiorazioni familiari (lettura precedente record pensione).");

                                }
                            }

                        }

                        using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                        {
                            dtce.ModificaInCompletatoCalendarioEvento(amf.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari, db);
                        }

                        using (dtDipendenti dtd = new dtDipendenti())
                        {
                            using (dtTrasferimento dtt = new dtTrasferimento())
                            {
                                using (dtUffici dtu = new dtUffici())
                                {
                                    var t = dtt.GetTrasferimentoById(amf.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO);

                                    if (t?.idTrasferimento > 0)
                                    {
                                        var dip = dtd.GetDipendenteByID(t.idDipendente);
                                        var uff = dtu.GetUffici(t.idUfficio);

                                        EmailTrasferimento.EmailAttiva(amf.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                            Resources.msgEmail.OggettoAttivazioneMaggiorazioniFamiliari,
                                                            string.Format(Resources.msgEmail.MessaggioAttivazioneMaggiorazioniFamiliari, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString()),
                                                            db);
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AltriDatiFamConiugeModel GetAdfValidiByIDConiuge(decimal idConiuge)
        {
            bool modificati = false;
            List<ALTRIDATIFAM> adfcl = new List<ALTRIDATIFAM>();
            AltriDatiFamConiugeModel adfm = new AltriDatiFamConiugeModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var c = db.CONIUGE.Find(idConiuge);
                var mf = db.MAGGIORAZIONIFAMILIARI.Find(c.IDMAGGIORAZIONIFAMILIARI);
                var lac = c.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                adfcl = c.ALTRIDATIFAM.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                            .OrderByDescending(a => a.IDALTRIDATIFAM).ToList();

                if (adfcl?.Any() ?? false)
                {
                    var adfc = adfcl.First();

                    adfm = new AltriDatiFamConiugeModel()
                    {
                        idAltriDatiFam = adfc.IDALTRIDATIFAM,
                        nazionalita = adfc.NAZIONALITA,
                        indirizzoResidenza = adfc.INDIRIZZORESIDENZA,
                        capResidenza = adfc.CAPRESIDENZA,
                        comuneResidenza = adfc.COMUNERESIDENZA,
                        provinciaResidenza = adfc.PROVINCIARESIDENZA,
                        dataAggiornamento = adfc.DATAAGGIORNAMENTO,
                        idStatoRecord = adfc.IDSTATORECORD,
                        modificato = modificati
                    };
                }
            }
            return adfm;
        }

        public AltriDatiFamFiglioModel GetAdfValidiByIDFiglio(decimal idFiglio)
        {
            bool modificati = false;
            List<ALTRIDATIFAM> adffl = new List<ALTRIDATIFAM>();
            AltriDatiFamFiglioModel adfm = new AltriDatiFamFiglioModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var f = db.FIGLI.Find(idFiglio);
                var mf = db.MAGGIORAZIONIFAMILIARI.Find(f.IDMAGGIORAZIONIFAMILIARI);
                var laf = f.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                adffl = f.ALTRIDATIFAM.Where(a => a.IDSTATORECORD !=(decimal)EnumStatoRecord.Annullato)
                            .OrderByDescending(a => a.IDALTRIDATIFAM).ToList();

                if (adffl?.Any() ?? false)
                {
                    var adff = adffl.First();

                    adfm = new AltriDatiFamFiglioModel()
                    {
                        idAltriDatiFam = adff.IDALTRIDATIFAM,
                        nazionalita = adff.NAZIONALITA,
                        dataNascita=adff.DATANASCITA,
                        capNascita=adff.CAPNASCITA,
                        comuneNascita=adff.COMUNENASCITA,
                        provinciaNascita=adff.PROVINCIANASCITA,
                        indirizzoResidenza = adff.INDIRIZZORESIDENZA,
                        capResidenza = adff.CAPRESIDENZA,
                        comuneResidenza = adff.COMUNERESIDENZA,
                        provinciaResidenza = adff.PROVINCIARESIDENZA,
                        dataAggiornamento = adff.DATAAGGIORNAMENTO,
                        idStatoRecord = adff.IDSTATORECORD,
                        modificato = modificati
                    };
                }
            }
            return adfm;
        }

        public string GetTipologiaFiglio(decimal idTipologiaFiglio)
        {
            try
            {
                var tipologiaFiglio = "";
                using (ModelDBISE db = new ModelDBISE())
                {
                    var tf = db.TIPOLOGIAFIGLIO.Find(idTipologiaFiglio);
                    if(tf.IDTIPOLOGIAFIGLIO>0)
                    {
                        tipologiaFiglio = tf.TIPOLOGIAFIGLIO1;
                    }
                    else
                    {
                        throw new Exception("Tipologia figlio non trovata.");
                    }
                }

                return tipologiaFiglio;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetTipologiaConiuge(decimal idTipologiaConiuge)
        {
            try
            {
                var tipologiaConiuge = "";
                using (ModelDBISE db = new ModelDBISE())
                {
                    var cf = db.TIPOLOGIACONIUGE.Find(idTipologiaConiuge);
                    if (cf.IDTIPOLOGIACONIUGE > 0)
                    {
                        tipologiaConiuge = cf.TIPOLOGIACONIUGE1;
                    }
                    else
                    {
                        throw new Exception("Tipologia coniuge non trovata.");
                    }
                }

                return tipologiaConiuge;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public VariazioneFigliModel CheckVariazioniAnagraficaFiglio(decimal? idFiglio, decimal idFigli_new)
        {
            try
            {
                VariazioneFigliModel vfm = new VariazioneFigliModel();
                FigliModel fm = new FigliModel();

                using (dtFigli dtf = new dtFigli())
                {
                    if (idFiglio > 0)
                    {
                        fm = dtf.GetFiglioOldbyID(idFiglio);
                    }
                    else
                    {
                        fm = dtf.GetFiglioOldbyID(idFigli_new);
                    }

                    var fm_new = dtf.GetFigliobyID(idFigli_new);

                    //string evidenzia = ";background-color:yellow";
                    string evidenzia = ";border-bottom:solid; border-bottom-color:yellow";
                    string evidenzia_titolo = ";border-bottom:solid;border-bottom-width:4px;border-color:yellow";
                    var tipologiaFiglio = this.GetTipologiaFiglio((decimal)fm.idTipologiaFiglio);
                    var tipologiaFiglio_new = this.GetTipologiaFiglio((decimal)fm_new.idTipologiaFiglio);

                    vfm = new VariazioneFigliModel()
                    {
                        tipologiaFiglio = tipologiaFiglio,
                        ev_Tipologia = (tipologiaFiglio != tipologiaFiglio_new) ? evidenzia : "",
                        codiceFiscale = fm.codiceFiscale.ToUpper(),
                        ev_codiceFiscale = (fm.codiceFiscale != fm_new.codiceFiscale) ? evidenzia : "",
                        nome = fm.nome.ToUpper(),
                        ev_nome = (fm.nome != fm_new.nome) ? evidenzia : "",
                        cognome = fm.cognome.ToUpper(),
                        ev_cognome = (fm.cognome != fm_new.cognome) ? evidenzia : "",
                        dataInizio = fm.dataInizio,
                        ev_dataInizio = (fm.dataInizio != fm_new.dataInizio) ? evidenzia : "",
                        dataFine = fm.dataFine,
                        ev_dataFine = (fm.dataFine != fm_new.dataFine) ? evidenzia : ""
                    };

                    vfm.ev_anagrafica = (vfm.ev_Tipologia != "" ||
                            vfm.ev_codiceFiscale != "" ||
                            vfm.ev_nome != "" ||
                            vfm.ev_cognome != "" ||
                            vfm.ev_dataInizio != "" ||
                            vfm.ev_dataFine != "") ? evidenzia_titolo : "";

                   
                }
                return vfm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public VariazioneConiugeModel CheckVariazioniAnagraficaConiuge(decimal? idConiuge, decimal idConiuge_new)
        {
            try
            {
                VariazioneConiugeModel vcm = new VariazioneConiugeModel();
                ConiugeModel cm = new ConiugeModel();

                using (dtConiuge dtc = new dtConiuge())
                {
                    if(idConiuge>0)
                    {
                        cm = dtc.GetConiugeOldbyID(idConiuge);
                    }
                    else
                    {
                        cm = dtc.GetConiugeOldbyID(idConiuge_new);
                    }


                    var cm_new = dtc.GetConiugebyID(idConiuge_new);

                    //string evidenzia = ";background-color:yellow";
                    string evidenzia = ";border-bottom:solid; border-bottom-color:yellow";
                    string evidenzia_titolo = ";border-bottom:solid;border-bottom-width:4px;border-color:yellow";
                    var tipologiaConiuge = this.GetTipologiaConiuge((decimal)cm.idTipologiaConiuge);
                    var tipologiaConiuge_new = this.GetTipologiaConiuge((decimal)cm_new.idTipologiaConiuge);

                    vcm = new VariazioneConiugeModel()
                    {
                        tipologiaConiuge = tipologiaConiuge,
                        ev_Tipologia = (tipologiaConiuge != tipologiaConiuge_new) ? evidenzia : "",
                        codiceFiscale = cm.codiceFiscale.ToUpper(),
                        ev_codiceFiscale = (cm.codiceFiscale != cm_new.codiceFiscale) ? evidenzia : "",
                        nome = cm.nome.ToUpper(),
                        ev_nome = (cm.nome != cm_new.nome) ? evidenzia : "",
                        cognome = cm.cognome.ToUpper(),
                        ev_cognome = (cm.cognome != cm_new.cognome) ? evidenzia : "",
                        dataInizio = cm.dataInizio,
                        ev_dataInizio = (cm.dataInizio != cm_new.dataInizio) ? evidenzia : "",
                        dataFine = cm.dataFine,
                        ev_dataFine = (cm.dataFine != cm_new.dataFine) ? evidenzia : ""
                    };

                    vcm.ev_anagrafica = (vcm.ev_Tipologia != "" ||
                            vcm.ev_codiceFiscale != "" ||
                            vcm.ev_nome != "" ||
                            vcm.ev_cognome != "" ||
                            vcm.ev_dataInizio != "" ||
                            vcm.ev_dataFine != "") ? evidenzia_titolo : "";

                }
                return vcm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public VariazioneAdfFigliModel CheckVariazioniAdfFiglio(decimal? idAdfFiglio, decimal idAdfFiglio_new)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    VariazioneAdfFigliModel vadffm = new VariazioneAdfFigliModel();
                    ALTRIDATIFAM adff = new ALTRIDATIFAM();
                    ALTRIDATIFAM adff_new = new ALTRIDATIFAM();
                    using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                    {
                        using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                        {
                            adff_new = db.ALTRIDATIFAM.Find(idAdfFiglio_new);
                            if (idAdfFiglio > 0)
                            {
                                adff = db.ALTRIDATIFAM.Find(idAdfFiglio);
                            }
                            else
                            {
                                adff = db.ALTRIDATIFAM.Find(idAdfFiglio_new);

                            }
                            //var adff = dtadf.GetAltriDatiFamiliariFiglio(idAdfFiglio);
                            //var adff = dtvmf.GetAltriDatiFamiliariFiglioOld(idFiglio,idMagFam);
                           // var adff_new = dtadf.GetAltriDatiFamiliariFiglio(idAdfFiglio_new);

                            string evidenzia = ";border-bottom:solid; border-bottom-color:yellow";
                            string evidenzia_titolo = ";border-bottom:solid;border-bottom-width:4px;border-color:yellow";

                            vadffm = new VariazioneAdfFigliModel()
                            {
                                dataNascita = adff.DATANASCITA,
                                ev_datanascita = (adff.DATANASCITA != adff_new.DATANASCITA) ? evidenzia : "",
                                capNascita = adff.CAPNASCITA,
                                ev_capnascita = (adff.CAPNASCITA != adff_new.CAPNASCITA) ? evidenzia : "",
                                comuneNascita = adff.COMUNENASCITA.ToUpper(),
                                ev_comunenascita = (adff.COMUNENASCITA != adff_new.COMUNENASCITA) ? evidenzia : "",
                                provinciaNascita = adff.PROVINCIANASCITA,
                                ev_provincianascita = (adff.PROVINCIANASCITA != adff_new.PROVINCIANASCITA) ? evidenzia : "",
                                nazionalita = adff.NAZIONALITA.ToUpper(),
                                ev_nazionalita = (adff.NAZIONALITA != adff_new.NAZIONALITA) ? evidenzia : "",
                                indirizzoResidenza = adff.INDIRIZZORESIDENZA.ToUpper(),
                                ev_indirizzoresidenza = (adff.INDIRIZZORESIDENZA != adff_new.INDIRIZZORESIDENZA) ? evidenzia : "",
                                capResidenza = adff.CAPRESIDENZA,
                                ev_capresidenza = (adff.CAPRESIDENZA != adff_new.CAPRESIDENZA) ? evidenzia : "",
                                comuneResidenza = adff.COMUNERESIDENZA.ToUpper(),
                                ev_comuneresidenza = (adff.COMUNERESIDENZA != adff_new.COMUNERESIDENZA) ? evidenzia : "",
                                provinciaResidenza = adff.PROVINCIARESIDENZA.ToUpper(),
                                ev_provinciaresidenza = (adff.PROVINCIARESIDENZA != adff_new.PROVINCIARESIDENZA) ? evidenzia : ""
                            };

                            vadffm.ev_altridati = (vadffm.ev_datanascita != "" ||
                                vadffm.ev_capnascita != "" ||
                                vadffm.ev_comunenascita != "" ||
                                vadffm.ev_provincianascita != "" ||
                                vadffm.ev_nazionalita != "" ||
                                vadffm.ev_indirizzoresidenza != "" ||
                                vadffm.ev_capresidenza != "" ||
                                vadffm.ev_comuneresidenza != "" ||
                                vadffm.ev_provinciaresidenza != "") ? evidenzia_titolo : "";
                        }
                    }

                    return vadffm;

                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public VariazioneAdfConiugeModel CheckVariazioniAdfConiuge(decimal? idAdfConiuge, decimal idAdfConiuge_new)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    VariazioneAdfConiugeModel vadfcm = new VariazioneAdfConiugeModel();
                    ALTRIDATIFAM adfc = new ALTRIDATIFAM();
                    ALTRIDATIFAM adfc_new = new ALTRIDATIFAM();
                    using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                    {
                        adfc_new = db.ALTRIDATIFAM.Find(idAdfConiuge_new);
                        if (idAdfConiuge > 0)
                        {
                            adfc = db.ALTRIDATIFAM.Find(idAdfConiuge);
                        }
                        else
                        {
                            adfc = db.ALTRIDATIFAM.Find(idAdfConiuge_new);

                        }

                        string evidenzia = ";border-bottom:solid; border-bottom-color:yellow";
                        string evidenzia_titolo = ";border-bottom:solid;border-bottom-width:4px;border-color:yellow";

                        vadfcm = new VariazioneAdfConiugeModel()
                        {
                            nazionalita = adfc.NAZIONALITA.ToUpper(),
                            ev_nazionalita = (adfc.NAZIONALITA != adfc_new.NAZIONALITA) ? evidenzia : "",
                            indirizzoResidenza = adfc.INDIRIZZORESIDENZA.ToUpper(),
                            ev_indirizzoresidenza = (adfc.INDIRIZZORESIDENZA != adfc_new.INDIRIZZORESIDENZA) ? evidenzia : "",
                            capResidenza = adfc.CAPRESIDENZA,
                            ev_capresidenza = (adfc.CAPRESIDENZA != adfc_new.CAPRESIDENZA) ? evidenzia : "",
                            comuneResidenza = adfc.COMUNERESIDENZA.ToUpper(),
                            ev_comuneresidenza = (adfc.COMUNERESIDENZA != adfc_new.COMUNERESIDENZA) ? evidenzia : "",
                            provinciaResidenza = adfc.PROVINCIARESIDENZA.ToUpper(),
                            ev_provinciaresidenza = (adfc.PROVINCIARESIDENZA != adfc_new.PROVINCIARESIDENZA) ? evidenzia : ""
                        };

                        vadfcm.ev_altridati = (vadfcm.ev_nazionalita != "" ||
                                                vadfcm.ev_indirizzoresidenza != "" ||
                                                vadfcm.ev_capresidenza != "" ||
                                                vadfcm.ev_comuneresidenza != "" ||
                                                vadfcm.ev_provinciaresidenza != "") ? evidenzia_titolo : "";

                    }

                    return vadfcm;

                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AnnullaRichiestaVariazione(decimal idAttivazioneMagFam, out decimal idAttivazioneMagFamNew, string testoAnnullaMF)
        {
            idAttivazioneMagFamNew = 0;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                        {

                            ///Prelevo la riga del ciclo di autorizzazione che si vuole annullare.
                            var amfOld = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

                            if (!(amfOld?.IDATTIVAZIONEMAGFAM > 0))
                            {
                                throw new Exception("Errore durante la lettura della attivazione corrente.");
                            }

                            #region annulla attivazione corrente
                            amfOld.DATAAGGIORNAMENTO = DateTime.Now;
                            amfOld.ANNULLATO = true;///Annullo la riga del ciclo di autorizzazione.


                            if (db.SaveChanges()<= 0)
                            {
                                throw new Exception("Errore nella fase di annullamento della riga di attivazione maggiorazione familiare per l'id: " + amfOld.IDATTIVAZIONEMAGFAM);
                            }
                            #endregion

                            #region crea nuova attivazione
                            ///Creo una nuova riga per il ciclo di autorizzazione.
                            ATTIVAZIONIMAGFAM amfNew = new ATTIVAZIONIMAGFAM()
                            {
                                IDMAGGIORAZIONIFAMILIARI = amfOld.IDMAGGIORAZIONIFAMILIARI,
                                RICHIESTAATTIVAZIONE = false,
                                ATTIVAZIONEMAGFAM = false,
                                DATAVARIAZIONE = DateTime.Now,
                                DATAAGGIORNAMENTO = DateTime.Now,
                                ANNULLATO = false,
                            };

                            db.ATTIVAZIONIMAGFAM.Add(amfNew);///Consolido la riga del ciclo di autorizzazione.

                            if (db.SaveChanges()<= 0)
                            {
                                throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione ciclo autorizzazione.");
                            }
                            #endregion

                            idAttivazioneMagFamNew = amfNew.IDATTIVAZIONEMAGFAM;

                            #region cicla Coniuge
                            ///Cliclo tutte le righe valide per il coniuge collegate alla vecchia riga per il ciclo di autorizzazione.
                            foreach (var cOld in amfOld.CONIUGE)
                            {
                                #region replico coniuge
                                ///Creo una nuova riga per il coniuge con le informazioni della vecchia riga.
                                CONIUGE cNew = new CONIUGE()
                                {
                                    IDTIPOLOGIACONIUGE = cOld.IDTIPOLOGIACONIUGE,
                                    IDMAGGIORAZIONIFAMILIARI = cOld.IDMAGGIORAZIONIFAMILIARI,
                                    NOME = cOld.NOME,
                                    COGNOME = cOld.COGNOME,
                                    CODICEFISCALE = cOld.CODICEFISCALE,
                                    DATAINIZIOVALIDITA = cOld.DATAINIZIOVALIDITA,
                                    DATAFINEVALIDITA = cOld.DATAFINEVALIDITA,
                                    DATAAGGIORNAMENTO = cOld.DATAAGGIORNAMENTO,
                                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                    FK_IDCONIUGE = cOld.FK_IDCONIUGE
                                };

                                amfNew.CONIUGE.Add(cNew);///Inserisco la nuova riga per il coniuge associata alla nuova riga per il ciclo di autorizzazione.
                                cOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione coniuge.");
                                }
                                #endregion

                                #region replico Altri dati familiari coniuge
                                ///Prelevo la vecchia riga di altri dati familiari collegati alla vecchia riga del coniuge.
                                var ladfOld =
                                        cOld.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare)
                                            .OrderByDescending(a => a.IDALTRIDATIFAM).ToList();

                                if (ladfOld?.Any() ?? false)///Esiste questo controllo ma è impossibile che si verifichi il contrario perché gli altri dati familiari sono obbligatori per attivare il ciclo di autorizzazione.
                                {
                                    var adfOld = ladfOld.First();
                                    ///Creo una nuova riga di altri dati familiari identica alla vechia riga.
                                    ALTRIDATIFAM adfNew = new ALTRIDATIFAM()
                                    {
                                        DATANASCITA = adfOld.DATANASCITA,
                                        CAPNASCITA = adfOld.CAPNASCITA,
                                        COMUNENASCITA = adfOld.COMUNENASCITA,
                                        PROVINCIANASCITA = adfOld.PROVINCIANASCITA,
                                        NAZIONALITA = adfOld.NAZIONALITA,
                                        INDIRIZZORESIDENZA = adfOld.INDIRIZZORESIDENZA,
                                        CAPRESIDENZA = adfOld.CAPRESIDENZA,
                                        COMUNERESIDENZA = adfOld.COMUNERESIDENZA,
                                        PROVINCIARESIDENZA = adfOld.PROVINCIARESIDENZA,
                                        DATAAGGIORNAMENTO = adfOld.DATAAGGIORNAMENTO,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                        FK_IDALTRIDATIFAM = adfOld.FK_IDALTRIDATIFAM
                                    };

                                    cNew.ALTRIDATIFAM.Add(adfNew);///La consolido e l'associo al coniuge
                                    adfOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                    if (db.SaveChanges()<= 0)
                                    {
                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione altri dati familiari per il coniuge.");
                                    }

                                    dtamf.AssociaAltriDatiFamiliari(amfNew.IDATTIVAZIONEMAGFAM, adfNew.IDALTRIDATIFAM, db);
                                }
                                //riassocio altri dati familiari validi
                                var ladf_validi =
                                        cOld.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                        .OrderByDescending(a => a.IDALTRIDATIFAM).ToList();
                                foreach (var adf_validi in ladf_validi)
                                {
                                    AssociaAltriDatiFamiliariConiuge(cNew.IDCONIUGE, adf_validi.IDALTRIDATIFAM, db);
                                }
                                #endregion

                                #region Documenti identità coniuge
                                ///Prelevo tutti i vecchi documenti d'identità.
                                var ldOld =
                                    cOld.DOCUMENTI.Where(
                                        a =>
                                            a.MODIFICATO==false &&
                                            a.IDSTATORECORD==(decimal)EnumStatoRecord.Da_Attivare &&
                                            a.IDTIPODOCUMENTO ==(decimal)EnumTipoDoc.Documento_Identita);

                                foreach (DOCUMENTI dOld in ldOld)
                                {
                                    ///Creo la nuova riga per il documento con l'informazioni della vecchia riga.
                                    DOCUMENTI dNew = new DOCUMENTI()
                                    {
                                        IDTIPODOCUMENTO = dOld.IDTIPODOCUMENTO,
                                        NOMEDOCUMENTO = dOld.NOMEDOCUMENTO,
                                        ESTENSIONE = dOld.ESTENSIONE,
                                        FILEDOCUMENTO = dOld.FILEDOCUMENTO,
                                        DATAINSERIMENTO = dOld.DATAINSERIMENTO,
                                        MODIFICATO = dOld.MODIFICATO,
                                        FK_IDDOCUMENTO = dOld.FK_IDDOCUMENTO,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                    };

                                    cNew.DOCUMENTI.Add(dNew);///Consolido il documento associandolo alla nuova riga del coniuge.
                                    dOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                    if (db.SaveChanges()<= 0)
                                    {
                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione documenti d'identità");
                                    }
                                    dtamf.AssociaDocumentoAttivazione(amfNew.IDATTIVAZIONEMAGFAM, dNew.IDDOCUMENTO, db);

                                }
                                //cerco documenti attivi non modificati e li riassocio al coniuge
                                ldOld =
                                    cOld.DOCUMENTI.Where(a =>
                                      a.MODIFICATO == false &&
                                      a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                      a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita);
                                foreach (var d in ldOld)
                                {
                                    Associa_Doc_Coniuge_ById(d.IDDOCUMENTO, cNew.IDCONIUGE, db);
                                }
                                #endregion

                                #region Pensioni
                                var lpOld = cOld.PENSIONE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare);

                                foreach (PENSIONE pOld in lpOld)
                                {
                                    PENSIONE pNew = new PENSIONE()
                                    {
                                        IMPORTOPENSIONE = pOld.IMPORTOPENSIONE,
                                        DATAINIZIO = pOld.DATAINIZIO,
                                        DATAFINE = pOld.DATAFINE,
                                        DATAAGGIORNAMENTO = pOld.DATAAGGIORNAMENTO,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                        FK_IDPENSIONE = null
                                    };

                                    cNew.PENSIONE.Add(pNew);
                                    pOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                    if (db.SaveChanges()<= 0)
                                    {
                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione pensioni.");
                                    }
                                    //if (pOld.ATTIVAZIONIMAGFAM?.Any(a => a.IDATTIVAZIONEMAGFAM == amfOld.IDATTIVAZIONEMAGFAM) ?? false)
                                    //{
                                    dtamf.AssociaPensioneAttivazione(amfNew.IDATTIVAZIONEMAGFAM, pNew.IDPENSIONE, db);
                                    //}
                                   
                                }

                                #endregion

                                #region Percentuale maggiorazione coniuge
                                using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                                {
                                    DateTime dtIni = cNew.DATAINIZIOVALIDITA;
                                    DateTime dtFin = cNew.DATAFINEVALIDITA;

                                    List<PercentualeMagConiugeModel> lpmcm =
                                        dtpc.GetListaPercentualiMagConiugeByRangeDate((EnumTipologiaConiuge)cNew.IDTIPOLOGIACONIUGE, dtIni, dtFin, db).ToList();

                                    if (lpmcm?.Any() ?? false)
                                    {
                                        foreach (var pmcm in lpmcm)
                                        {
                                            dtpc.AssociaPercentualeMaggiorazioneConiuge(cNew.IDCONIUGE, pmcm.idPercentualeConiuge, db);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Errore nella fase di annulla richiesta. Non è presente nessuna percentuale del coniuge.");
                                    }
                                }
                                #endregion
                                            
                            }
                            #endregion

                            #region Figli
                            foreach (var fOld in amfOld.FIGLI)
                            {
                                #region replico figli
                                FIGLI fNew = new FIGLI()
                                {
                                    IDTIPOLOGIAFIGLIO = fOld.IDTIPOLOGIAFIGLIO,
                                    IDMAGGIORAZIONIFAMILIARI = fOld.IDMAGGIORAZIONIFAMILIARI,
                                    NOME = fOld.NOME,
                                    COGNOME = fOld.COGNOME,
                                    CODICEFISCALE = fOld.CODICEFISCALE,
                                    DATAINIZIOVALIDITA = fOld.DATAINIZIOVALIDITA,
                                    DATAFINEVALIDITA = fOld.DATAFINEVALIDITA,
                                    DATAAGGIORNAMENTO = fOld.DATAAGGIORNAMENTO,
                                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                    FK_IDFIGLI = fOld.FK_IDFIGLI
                                };

                                amfNew.FIGLI.Add(fNew);
                                fOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                if (db.SaveChanges()<= 0)
                                {
                                    throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione figli.");
                                }
                                #endregion

                                #region Altri dati familiari
                                ///Prelevo la vecchia riga di altri dati familiari collegati alla vecchia riga del coniuge.
                                var ladfOld =
                                        fOld.ALTRIDATIFAM.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                            .OrderByDescending(a => a.IDALTRIDATIFAM);

                                if (ladfOld?.Any() ?? false)///Esiste questo controllo ma è impossibile che si verifichi il contrario perché gli altri dati familiari sono obbligatori per attivare il ciclo di autorizzazione.
                                {
                                    var adfOld = ladfOld.First();
                                    ///Creo una nuova riga di altri dati familiari identica alla vechia riga.
                                    ALTRIDATIFAM adfNew = new ALTRIDATIFAM()
                                    {
                                        DATANASCITA = adfOld.DATANASCITA,
                                        CAPNASCITA = adfOld.CAPNASCITA,
                                        COMUNENASCITA = adfOld.COMUNENASCITA,
                                        PROVINCIANASCITA = adfOld.PROVINCIANASCITA,
                                        NAZIONALITA = adfOld.NAZIONALITA,
                                        INDIRIZZORESIDENZA = adfOld.INDIRIZZORESIDENZA,
                                        CAPRESIDENZA = adfOld.CAPRESIDENZA,
                                        COMUNERESIDENZA = adfOld.COMUNERESIDENZA,
                                        PROVINCIARESIDENZA = adfOld.PROVINCIARESIDENZA,
                                        DATAAGGIORNAMENTO = adfOld.DATAAGGIORNAMENTO,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                    };

                                    fNew.ALTRIDATIFAM.Add(adfNew);///La consolido e l'associo al figlio
                                    adfOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione altri dati familiari per i figli.");
                                    }
                                    dtamf.AssociaAltriDatiFamiliari(amfNew.IDATTIVAZIONEMAGFAM, adfNew.IDALTRIDATIFAM, db);
                                }
                                //riassocio eventuali altri dati familiari validi
                                var ladf_validi =
                                        fOld.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                        .OrderByDescending(a => a.IDALTRIDATIFAM).ToList();
                                foreach (var adf_validi in ladf_validi)
                                {
                                    AssociaAltriDatiFamiliariFiglio(fNew.IDFIGLI, adf_validi.IDALTRIDATIFAM, db);
                                }

                                #endregion

                                #region Documenti
                                ///Prelevo tutti i vecchi documenti d'identità.
                                var ldOld =
                                    fOld.DOCUMENTI.Where(
                                        a =>
                                            a.MODIFICATO == false &&
                                            a.IDSTATORECORD==(decimal)EnumStatoRecord.Da_Attivare &&
                                            a.IDTIPODOCUMENTO ==
                                            (decimal)EnumTipoDoc.Documento_Identita);

                                foreach (var dOld in ldOld)
                                {
                                    ///Creo la nuova riga per il documento con l'informazioni della vecchia riga.
                                    DOCUMENTI dNew = new DOCUMENTI()
                                    {
                                        IDTIPODOCUMENTO = dOld.IDTIPODOCUMENTO,
                                        NOMEDOCUMENTO = dOld.NOMEDOCUMENTO,
                                        ESTENSIONE = dOld.ESTENSIONE,
                                        FILEDOCUMENTO = dOld.FILEDOCUMENTO,
                                        DATAINSERIMENTO = dOld.DATAINSERIMENTO,
                                        MODIFICATO = dOld.MODIFICATO,
                                        FK_IDDOCUMENTO = dOld.FK_IDDOCUMENTO,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                    };

                                    fNew.DOCUMENTI.Add(dNew);///Consolido il documento associandolo alla nuova riga del coniuge.
                                    dOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione documenti d'identità");
                                    }
                                    dtamf.AssociaDocumentoAttivazione(amfNew.IDATTIVAZIONEMAGFAM, dNew.IDDOCUMENTO, db);
                                }
                                //cerco documenti attivi non modificati e li riassocio al figlio
                                ldOld =
                                    fOld.DOCUMENTI.Where(a =>
                                      a.MODIFICATO == false &&
                                      a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                      a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita);
                                foreach (var d in ldOld)
                                {
                                    Associa_Doc_Figlio_ById(d.IDDOCUMENTO, fNew.IDFIGLI, db);
                                }
                                #endregion

                                #region Indennità primo segretario
                                using (dtIndennitaPrimoSegretario dtips = new dtIndennitaPrimoSegretario())
                                {
                                    DateTime dtIni = fNew.DATAINIZIOVALIDITA;
                                    DateTime dtFin = fNew.DATAFINEVALIDITA;

                                    List<IndennitaPrimoSegretModel> lipsm =
                                        dtips.GetIndennitaPrimoSegretario(dtIni, dtFin, db).ToList();

                                    if (lipsm?.Any() ?? false)
                                    {
                                        foreach (var ipsm in lipsm)
                                        {
                                            dtips.AssociaIndennitaPrimoSegretarioFiglio(fNew.IDFIGLI, ipsm.idIndPrimoSegr, db);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception(
                                            "Errore nella fase di annulla richiesta. Non è presente nessuna indennità di primo segretario per il figlio che si vuole inserire.");
                                    }
                                }
                                #endregion

                                #region Percentuale maggiorazioni figli
                                using (dtPercentualeMagFigli dtpf = new dtPercentualeMagFigli())
                                {
                                    DateTime dtIni = fNew.DATAINIZIOVALIDITA;
                                    DateTime dtFin = fNew.DATAFINEVALIDITA;

                                    List<PercentualeMagFigliModel> lpmfm =
                                        dtpf.GetPercentualeMaggiorazioneFigli((EnumTipologiaFiglio)fNew.IDTIPOLOGIAFIGLIO, dtIni, dtFin, db).ToList();

                                    if (lpmfm?.Any() ?? false)
                                    {
                                        foreach (var pmfm in lpmfm)
                                        {
                                            dtpf.AssociaPercentualeMaggiorazioneFigli(fNew.IDFIGLI, pmfm.idPercMagFigli, db);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Errore nella fase di annulla richiesta. Non è presente nessuna percentuale per il figlio.");
                                    }
                                }
                                #endregion

                            }
                            #endregion

                            #region Formulari
                            var ldFormulariOld =
                                amfOld.DOCUMENTI.Where(
                                    a =>
                                        a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari &&
                                        a.IDSTATORECORD==(decimal)EnumStatoRecord.Da_Attivare);

                            foreach (var d in ldFormulariOld)
                            {
                                DOCUMENTI dNew = new DOCUMENTI()
                                {
                                    IDTIPODOCUMENTO = d.IDTIPODOCUMENTO,
                                    NOMEDOCUMENTO = d.NOMEDOCUMENTO,
                                    ESTENSIONE = d.ESTENSIONE,
                                    FILEDOCUMENTO = d.FILEDOCUMENTO,
                                    DATAINSERIMENTO = d.DATAINSERIMENTO,
                                    MODIFICATO = d.MODIFICATO,
                                    FK_IDDOCUMENTO = d.FK_IDDOCUMENTO,
                                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                };

                                amfNew.DOCUMENTI.Add(dNew);
                                d.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore nella fase di creazione del documento nel ciclo di annullamento.");
                                }

                                //dtamf.AssociaFormulario(amfNew.IDATTIVAZIONEMAGFAM, d.IDDOCUMENTO, db);
                            }
                            #endregion

                            #region annulla solo residui adf
                            var ladf_Old =
                                amfOld.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare)
                                                .OrderByDescending(a => a.IDALTRIDATIFAM);

                            if (ladf_Old?.Any() ?? false)///Esiste questo controllo ma è impossibile che si verifichi il contrario perché gli altri dati familiari sono obbligatori per attivare il ciclo di autorizzazione.
                            {
                                foreach (var adf_Old in ladf_Old)
                                {
                                    //var adfOld = ladfOld.First();

                                    ///Creo una nuova riga di altri dati familiari identica alla vecchia riga.
                                    ALTRIDATIFAM adfNew = new ALTRIDATIFAM()
                                    {
                                        DATANASCITA = adf_Old.DATANASCITA,
                                        CAPNASCITA = adf_Old.CAPNASCITA,
                                        COMUNENASCITA = adf_Old.COMUNENASCITA,
                                        PROVINCIANASCITA = adf_Old.PROVINCIANASCITA,
                                        NAZIONALITA = adf_Old.NAZIONALITA,
                                        INDIRIZZORESIDENZA = adf_Old.INDIRIZZORESIDENZA,
                                        CAPRESIDENZA = adf_Old.CAPRESIDENZA,
                                        COMUNERESIDENZA = adf_Old.COMUNERESIDENZA,
                                        PROVINCIARESIDENZA = adf_Old.PROVINCIARESIDENZA,
                                        DATAAGGIORNAMENTO = adf_Old.DATAAGGIORNAMENTO,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                        FK_IDALTRIDATIFAM = adf_Old.FK_IDALTRIDATIFAM
                                    };
                                    amfNew.ALTRIDATIFAM.Add(adfNew);

                                    adf_Old.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                    if (db.SaveChanges()<= 0)
                                    {
                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione altri dati familiari.");
                                    }
                                    if (adf_Old.CONIUGE.Count() > 0)
                                    {
                                        var c = adf_Old.CONIUGE.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDCONIUGE).First();
                                        AssociaAltriDatiFamiliariConiuge(c.IDCONIUGE,adfNew.IDALTRIDATIFAM,db);
                                    }
                                    if (adf_Old.FIGLI.Count() > 0)
                                    {
                                        var f = adf_Old.FIGLI.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDFIGLI).First();
                                        AssociaAltriDatiFamiliariFiglio(f.IDFIGLI, adfNew.IDALTRIDATIFAM, db);
                                    }
                                    /// associo la nuova riga di altri dati familiari alla nuova riga del ciclo di autorizzazione.
                                    //dtamf.AssociaAltriDatiFamiliari(amfNew.IDATTIVAZIONEMAGFAM, adfNew.IDALTRIDATIFAM, db);
                                        
                                }
                            }
                            #endregion

                            #region annulla solo doc identita
                            ///Prelevo tutti i vecchi documenti d'identità.
                            var ldocOld =
                                amfOld.DOCUMENTI.Where(
                                    a =>
                                        a.MODIFICATO == false &&
                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare &&
                                        a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita);

                            foreach (DOCUMENTI docOld in ldocOld)
                            {
                                ///Creo la nuova riga per il documento con l'informazioni della vecchia riga.
                                DOCUMENTI dNew = new DOCUMENTI()
                                {
                                    IDTIPODOCUMENTO = docOld.IDTIPODOCUMENTO,
                                    NOMEDOCUMENTO = docOld.NOMEDOCUMENTO,
                                    ESTENSIONE = docOld.ESTENSIONE,
                                    FILEDOCUMENTO = docOld.FILEDOCUMENTO,
                                    DATAINSERIMENTO = docOld.DATAINSERIMENTO,
                                    MODIFICATO = docOld.MODIFICATO,
                                    FK_IDDOCUMENTO = docOld.FK_IDDOCUMENTO,
                                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                };

                                amfNew.DOCUMENTI.Add(dNew);///Consolido il documento associandolo alla nuova riga del coniuge.
                                docOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione documenti d'identità");
                                }
                                //dtamf.AssociaDocumentoAttivazione(amfNew.IDATTIVAZIONEMAGFAM, dNew.IDDOCUMENTO, db);

                                using (dtDocumenti dtd = new dtDocumenti())
                                {
                                    if (docOld.CONIUGE.Count() > 0)
                                    {
                                        var c = docOld.CONIUGE.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDCONIUGE).First();
                                        dtd.AssociaDocumentoConiuge(c.IDCONIUGE, dNew.IDDOCUMENTO, db);
                                    }
                                    if (docOld.FIGLI.Count() > 0)
                                    {
                                        var f = docOld.FIGLI.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDFIGLI).First();
                                        dtd.AssociaDocumentoFiglio(f.IDFIGLI, dNew.IDDOCUMENTO, db);
                                    }
                                }
                            }
                            #endregion

                            #region annulla solo pensione (da fare)
                            #endregion

                               

                            EmailTrasferimento.EmailAnnulla(amfOld.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                            Resources.msgEmail.OggettoAnnullaRichiestaMaggiorazioniFamiliari,
                                                            testoAnnullaMF,
                                                            db);
                            //this.EmailAnnullaRichiesta(idAttivazioneMagFam, testoAnnullaTrasf, db);

                            using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                            {
                                dtce.AnnullaMessaggioEvento(amfOld.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari, db);
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void AnnullaRichiestaVariazione_old(decimal idAttivazioneMagFam, out decimal idAttivazioneMagFamNew, string testoAnnullaMF)
        {
            idAttivazioneMagFamNew = 0;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        ///Prelevo la riga del ciclo di autorizzazione che si vuole annullare.
                        var amfOld = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

                        if (amfOld?.IDATTIVAZIONEMAGFAM > 0)
                        {

                            amfOld.DATAAGGIORNAMENTO = DateTime.Now;
                            amfOld.ANNULLATO = true;///Annullo la riga del ciclo di autorizzazione.

                            int i = db.SaveChanges();

                            if (i > 0)
                            {
                                #region attivazione
                                ///Creo una nuova riga per il ciclo di autorizzazione.
                                ATTIVAZIONIMAGFAM amfNew = new ATTIVAZIONIMAGFAM()
                                {
                                    IDMAGGIORAZIONIFAMILIARI = amfOld.IDMAGGIORAZIONIFAMILIARI,
                                    RICHIESTAATTIVAZIONE = false,
                                    ATTIVAZIONEMAGFAM = false,
                                    DATAVARIAZIONE = DateTime.Now,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    ANNULLATO = false,
                                };

                                db.ATTIVAZIONIMAGFAM.Add(amfNew);///Consolido la riga del ciclo di autorizzazione.
                                #endregion

                                int j = db.SaveChanges();

                                if (j > 0)
                                {
                                    idAttivazioneMagFamNew = amfNew.IDATTIVAZIONEMAGFAM;

                                    using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                                    {
                                        #region Coniuge
                                        ///Cliclo tutte le righe valide per il coniuge collegate alla vecchia riga per il ciclo di autorizzazione.
                                        foreach (var cOld in amfOld.CONIUGE)
                                        {
                                            //dtamf.AssociaConiugeAttivazione(amfNew.IDATTIVAZIONEMAGFAM, cOld.IDCONIUGE, db);

                                            ///Creo una nuova riga per il coniuge con le informazioni della vecchia riga.
                                            CONIUGE cNew = new CONIUGE()
                                            {
                                                IDTIPOLOGIACONIUGE = cOld.IDTIPOLOGIACONIUGE,
                                                IDMAGGIORAZIONIFAMILIARI = cOld.IDMAGGIORAZIONIFAMILIARI,
                                                NOME = cOld.NOME,
                                                COGNOME = cOld.COGNOME,
                                                CODICEFISCALE = cOld.CODICEFISCALE,
                                                DATAINIZIOVALIDITA = cOld.DATAINIZIOVALIDITA,
                                                DATAFINEVALIDITA = cOld.DATAFINEVALIDITA,
                                                DATAAGGIORNAMENTO = cOld.DATAAGGIORNAMENTO,
                                                IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                                FK_IDCONIUGE = cOld.FK_IDCONIUGE
                                            };

                                            amfNew.CONIUGE.Add(cNew);///Inserisco la nuova riga per il coniuge associata alla nuova riga per il ciclo di autorizzazione.
                                            cOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                            int j2 = db.SaveChanges();

                                            if (j2 > 0)
                                            {
                                                #region Altri dati familiari coniuge
                                                ///Prelevo la vecchia riga di altri dati familiari collegati alla vecchia riga del coniuge.
                                                var ladfOld =
                                                    cOld.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare)
                                                        .OrderByDescending(a => a.IDALTRIDATIFAM).ToList();

                                                if (ladfOld?.Any() ?? false)///Esiste questo controllo ma è impossibile che si verifichi il contrario perché gli altri dati familiari sono obbligatori per attivare il ciclo di autorizzazione.
                                                {
                                                    var adfOld = ladfOld.First();
                                                    ///Creo una nuova riga di altri dati familiari identica alla vechia riga.
                                                    ALTRIDATIFAM adfNew = new ALTRIDATIFAM()
                                                    {
                                                        DATANASCITA = adfOld.DATANASCITA,
                                                        CAPNASCITA = adfOld.CAPNASCITA,
                                                        COMUNENASCITA = adfOld.COMUNENASCITA,
                                                        PROVINCIANASCITA = adfOld.PROVINCIANASCITA,
                                                        NAZIONALITA = adfOld.NAZIONALITA,
                                                        INDIRIZZORESIDENZA = adfOld.INDIRIZZORESIDENZA,
                                                        CAPRESIDENZA = adfOld.CAPRESIDENZA,
                                                        COMUNERESIDENZA = adfOld.COMUNERESIDENZA,
                                                        PROVINCIARESIDENZA = adfOld.PROVINCIARESIDENZA,
                                                        DATAAGGIORNAMENTO = adfOld.DATAAGGIORNAMENTO,
                                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                                        FK_IDALTRIDATIFAM = adfOld.FK_IDALTRIDATIFAM
                                                    };

                                                    cNew.ALTRIDATIFAM.Add(adfNew);///La consolido e l'associo al coniuge
                                                    adfOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                                    int j3 = db.SaveChanges();

                                                    if (j3 > 0)
                                                    {
                                                        ///Verifico se la vecchia riga di altri dati familiari era collegata alla vecchia riga del ciclo di autorizzazione,
                                                        /// se si associo la nuova riga di altri dati familiari alla nuova riga del ciclo di autorizzazione.
                                                        if (adfOld.ATTIVAZIONIMAGFAM?.Any(a => a.IDATTIVAZIONEMAGFAM == amfOld.IDATTIVAZIONEMAGFAM) ?? false)
                                                        {
                                                            dtamf.AssociaAltriDatiFamiliari(amfNew.IDATTIVAZIONEMAGFAM, adfNew.IDALTRIDATIFAM, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione altri dati familiari per il coniuge.");
                                                    }

                                                }
                                                #endregion

                                                #region Documenti identità coniuge
                                                ///Prelevo tutti i vecchi documenti d'identità.
                                                var ldOld =
                                                    cOld.DOCUMENTI.Where(
                                                        a =>
                                                            a.MODIFICATO == false &&
                                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare &&
                                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita);

                                                foreach (DOCUMENTI dOld in ldOld)
                                                {
                                                    ///Creo la nuova riga per il documento con l'informazioni della vecchia riga.
                                                    DOCUMENTI dNew = new DOCUMENTI()
                                                    {
                                                        IDTIPODOCUMENTO = dOld.IDTIPODOCUMENTO,
                                                        NOMEDOCUMENTO = dOld.NOMEDOCUMENTO,
                                                        ESTENSIONE = dOld.ESTENSIONE,
                                                        FILEDOCUMENTO = dOld.FILEDOCUMENTO,
                                                        DATAINSERIMENTO = dOld.DATAINSERIMENTO,
                                                        MODIFICATO = dOld.MODIFICATO,
                                                        FK_IDDOCUMENTO = dOld.FK_IDDOCUMENTO,
                                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                                    };

                                                    cNew.DOCUMENTI.Add(dNew);///Consolido il documento associandolo alla nuova riga del coniuge.
                                                    dOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                                    int j4 = db.SaveChanges();

                                                    if (j4 > 0)
                                                    {
                                                        ///Verifico se il vecchio documento era associato al vecchio ciclo di autorizzazione,
                                                        /// se si, la nuova riga del documento l'associo alla nuova riga per il ciclo di autorizzazione.
                                                        if (dOld.ATTIVAZIONIMAGFAM?.Any(a => a.IDATTIVAZIONEMAGFAM == amfOld.IDATTIVAZIONEMAGFAM) ?? false)
                                                        {
                                                            dtamf.AssociaDocumentoAttivazione(amfNew.IDATTIVAZIONEMAGFAM, dNew.IDDOCUMENTO, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione documenti d'identità");
                                                    }
                                                }
                                                #endregion

                                                #region Pensioni

                                                var lpOld = cOld.PENSIONE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare);

                                                foreach (PENSIONE pOld in lpOld)
                                                {
                                                    PENSIONE pNew = new PENSIONE()
                                                    {
                                                        IMPORTOPENSIONE = pOld.IMPORTOPENSIONE,
                                                        DATAINIZIO = pOld.DATAINIZIO,
                                                        DATAFINE = pOld.DATAFINE,
                                                        DATAAGGIORNAMENTO = pOld.DATAAGGIORNAMENTO,
                                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                                        FK_IDPENSIONE = null
                                                    };

                                                    cNew.PENSIONE.Add(pNew);
                                                    pOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                                    int j5 = db.SaveChanges();

                                                    if (j5 > 0)
                                                    {
                                                        if (pOld.ATTIVAZIONIMAGFAM?.Any(a => a.IDATTIVAZIONEMAGFAM == amfOld.IDATTIVAZIONEMAGFAM) ?? false)
                                                        {
                                                            dtamf.AssociaPensioneAttivazione(amfNew.IDATTIVAZIONEMAGFAM, pNew.IDPENSIONE, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione pensioni.");
                                                    }
                                                }

                                                #endregion

                                                #region Percentuale maggiorazione coniuge
                                                using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                                                {
                                                    DateTime dtIni = cNew.DATAINIZIOVALIDITA;
                                                    DateTime dtFin = cNew.DATAFINEVALIDITA;

                                                    List<PercentualeMagConiugeModel> lpmcm =
                                                        dtpc.GetListaPercentualiMagConiugeByRangeDate((EnumTipologiaConiuge)cNew.IDTIPOLOGIACONIUGE, dtIni, dtFin, db).ToList();

                                                    if (lpmcm?.Any() ?? false)
                                                    {
                                                        foreach (var pmcm in lpmcm)
                                                        {
                                                            dtpc.AssociaPercentualeMaggiorazioneConiuge(cNew.IDCONIUGE, pmcm.idPercentualeConiuge, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Non è presente nessuna percentuale del coniuge.");
                                                    }
                                                }
                                                #endregion

                                            }
                                            else
                                            {
                                                throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione coniuge.");
                                            }
                                        }
                                        #endregion

                                        #region Figli
                                        foreach (var fOld in amfOld.FIGLI)
                                        {
                                            FIGLI fNew = new FIGLI()
                                            {
                                                IDTIPOLOGIAFIGLIO = fOld.IDTIPOLOGIAFIGLIO,
                                                IDMAGGIORAZIONIFAMILIARI = fOld.IDMAGGIORAZIONIFAMILIARI,
                                                NOME = fOld.NOME,
                                                COGNOME = fOld.COGNOME,
                                                CODICEFISCALE = fOld.CODICEFISCALE,
                                                DATAINIZIOVALIDITA = fOld.DATAINIZIOVALIDITA,
                                                DATAFINEVALIDITA = fOld.DATAFINEVALIDITA,
                                                DATAAGGIORNAMENTO = fOld.DATAAGGIORNAMENTO,
                                                IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                                FK_IDFIGLI = fOld.FK_IDFIGLI
                                            };

                                            amfNew.FIGLI.Add(fNew);
                                            fOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                            int x = db.SaveChanges();

                                            if (x > 0)
                                            {
                                                #region Altri dati familiari
                                                ///Prelevo la vecchia riga di altri dati familiari collegati alla vecchia riga del coniuge.
                                                var ladfOld =
                                                    fOld.ALTRIDATIFAM.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                                        .OrderByDescending(a => a.IDALTRIDATIFAM);

                                                if (ladfOld?.Any() ?? false)///Esiste questo controllo ma è impossibile che si verifichi il contrario perché gli altri dati familiari sono obbligatori per attivare il ciclo di autorizzazione.
                                                {
                                                    var adfOld = ladfOld.First();
                                                    ///Creo una nuova riga di altri dati familiari identica alla vechia riga.
                                                    ALTRIDATIFAM adfNew = new ALTRIDATIFAM()
                                                    {
                                                        DATANASCITA = adfOld.DATANASCITA,
                                                        CAPNASCITA = adfOld.CAPNASCITA,
                                                        COMUNENASCITA = adfOld.COMUNENASCITA,
                                                        PROVINCIANASCITA = adfOld.PROVINCIANASCITA,
                                                        NAZIONALITA = adfOld.NAZIONALITA,
                                                        INDIRIZZORESIDENZA = adfOld.INDIRIZZORESIDENZA,
                                                        CAPRESIDENZA = adfOld.CAPRESIDENZA,
                                                        COMUNERESIDENZA = adfOld.COMUNERESIDENZA,
                                                        PROVINCIARESIDENZA = adfOld.PROVINCIARESIDENZA,
                                                        DATAAGGIORNAMENTO = adfOld.DATAAGGIORNAMENTO,
                                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                                    };

                                                    fNew.ALTRIDATIFAM.Add(adfNew);///La consolido e l'associo al figlio
                                                    adfOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                                    int x2 = db.SaveChanges();

                                                    if (x2 > 0)
                                                    {
                                                        ///Verifico se la vecchia riga di altri dati familiari era collegata alla vecchia riga del ciclo di autorizzazione,
                                                        /// se si associo la nuova riga di altri dati familiari alla nuova riga del ciclo di autorizzazione.
                                                        if (adfOld.ATTIVAZIONIMAGFAM?.Any(a => a.IDATTIVAZIONEMAGFAM == amfOld.IDATTIVAZIONEMAGFAM) ?? false)
                                                        {
                                                            dtamf.AssociaAltriDatiFamiliari(amfNew.IDATTIVAZIONEMAGFAM, adfNew.IDALTRIDATIFAM, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione altri dati familiari per i figli.");
                                                    }
                                                }
                                                #endregion

                                                #region Documenti
                                                ///Prelevo tutti i vecchi documenti d'identità.
                                                var ldOld =
                                                    fOld.DOCUMENTI.Where(
                                                        a =>
                                                            a.MODIFICATO == false &&
                                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare &&
                                                            a.IDTIPODOCUMENTO ==
                                                            (decimal)EnumTipoDoc.Documento_Identita);

                                                foreach (var dOld in ldOld)
                                                {
                                                    ///Creo la nuova riga per il documento con l'informazioni della vecchia riga.
                                                    DOCUMENTI dNew = new DOCUMENTI()
                                                    {
                                                        IDTIPODOCUMENTO = dOld.IDTIPODOCUMENTO,
                                                        NOMEDOCUMENTO = dOld.NOMEDOCUMENTO,
                                                        ESTENSIONE = dOld.ESTENSIONE,
                                                        FILEDOCUMENTO = dOld.FILEDOCUMENTO,
                                                        DATAINSERIMENTO = dOld.DATAINSERIMENTO,
                                                        MODIFICATO = dOld.MODIFICATO,
                                                        FK_IDDOCUMENTO = dOld.FK_IDDOCUMENTO,
                                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                                    };

                                                    fNew.DOCUMENTI.Add(dNew);///Consolido il documento associandolo alla nuova riga del coniuge.
                                                    dOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                                    int j4 = db.SaveChanges();

                                                    if (j4 > 0)
                                                    {
                                                        ///Verifico se il vecchio documento era associato al vecchio ciclo di autorizzazione,
                                                        /// se si, la nuova riga del documento l'associo alla nuova riga per il ciclo di autorizzazione.
                                                        if (dOld.ATTIVAZIONIMAGFAM?.Any(a => a.IDATTIVAZIONEMAGFAM == amfOld.IDATTIVAZIONEMAGFAM) ?? false)
                                                        {
                                                            dtamf.AssociaDocumentoAttivazione(amfNew.IDATTIVAZIONEMAGFAM, dNew.IDDOCUMENTO, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione documenti d'identità");
                                                    }
                                                }
                                                #endregion

                                                #region Indennità primo segretario
                                                using (dtIndennitaPrimoSegretario dtips = new dtIndennitaPrimoSegretario())
                                                {
                                                    DateTime dtIni = fNew.DATAINIZIOVALIDITA;
                                                    DateTime dtFin = fNew.DATAFINEVALIDITA;

                                                    List<IndennitaPrimoSegretModel> lipsm =
                                                        dtips.GetIndennitaPrimoSegretario(dtIni, dtFin, db).ToList();

                                                    if (lipsm?.Any() ?? false)
                                                    {
                                                        foreach (var ipsm in lipsm)
                                                        {
                                                            dtips.AssociaIndennitaPrimoSegretarioFiglio(fNew.IDFIGLI, ipsm.idIndPrimoSegr, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception(
                                                            "Errore nella fase di annulla richiesta. Non è presente nessuna indennità di primo segretario per il figlio che si vuole inserire.");
                                                    }
                                                }
                                                #endregion

                                                #region Percentuale maggiorazioni figli
                                                using (dtPercentualeMagFigli dtpf = new dtPercentualeMagFigli())
                                                {
                                                    DateTime dtIni = fNew.DATAINIZIOVALIDITA;
                                                    DateTime dtFin = fNew.DATAFINEVALIDITA;

                                                    List<PercentualeMagFigliModel> lpmfm =
                                                        dtpf.GetPercentualeMaggiorazioneFigli((EnumTipologiaFiglio)fNew.IDTIPOLOGIAFIGLIO, dtIni, dtFin, db).ToList();

                                                    if (lpmfm?.Any() ?? false)
                                                    {
                                                        foreach (var pmfm in lpmfm)
                                                        {
                                                            dtpf.AssociaPercentualeMaggiorazioneFigli(fNew.IDFIGLI, pmfm.idPercMagFigli, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Non è presente nessuna percentuale per il figlio.");
                                                    }
                                                }
                                                #endregion

                                            }
                                            else
                                            {
                                                throw new Exception();
                                            }


                                        }
                                        #endregion

                                        #region Formulari

                                        var ldFormulariOld =
                                            amfOld.DOCUMENTI.Where(
                                                a =>
                                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari &&
                                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare);

                                        foreach (var d in ldFormulariOld)
                                        {
                                            DOCUMENTI dNew = new DOCUMENTI()
                                            {
                                                IDTIPODOCUMENTO = d.IDTIPODOCUMENTO,
                                                NOMEDOCUMENTO = d.NOMEDOCUMENTO,
                                                ESTENSIONE = d.ESTENSIONE,
                                                FILEDOCUMENTO = d.FILEDOCUMENTO,
                                                DATAINSERIMENTO = d.DATAINSERIMENTO,
                                                MODIFICATO = d.MODIFICATO,
                                                FK_IDDOCUMENTO = d.FK_IDDOCUMENTO,
                                                IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                            };

                                            amfNew.DOCUMENTI.Add(dNew);
                                            d.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                            if (db.SaveChanges() <= 0)
                                            {
                                                throw new Exception("Errore nella fase di creazione del documento nel ciclo di annullamento.");
                                            }

                                            //dtamf.AssociaFormulario(amfNew.IDATTIVAZIONEMAGFAM, d.IDDOCUMENTO, db);
                                        }
                                        #endregion

                                        #region annulla solo residui adf
                                        var ladf_Old =
                                            amfOld.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare)
                                                            .OrderByDescending(a => a.IDALTRIDATIFAM);

                                        if (ladf_Old?.Any() ?? false)///Esiste questo controllo ma è impossibile che si verifichi il contrario perché gli altri dati familiari sono obbligatori per attivare il ciclo di autorizzazione.
                                        {
                                            foreach (var adf_Old in ladf_Old)
                                            {
                                                //var adfOld = ladfOld.First();
                                                ///Creo una nuova riga di altri dati familiari identica alla vechia riga.
                                                ALTRIDATIFAM adfNew = new ALTRIDATIFAM()
                                                {
                                                    DATANASCITA = adf_Old.DATANASCITA,
                                                    CAPNASCITA = adf_Old.CAPNASCITA,
                                                    COMUNENASCITA = adf_Old.COMUNENASCITA,
                                                    PROVINCIANASCITA = adf_Old.PROVINCIANASCITA,
                                                    NAZIONALITA = adf_Old.NAZIONALITA,
                                                    INDIRIZZORESIDENZA = adf_Old.INDIRIZZORESIDENZA,
                                                    CAPRESIDENZA = adf_Old.CAPRESIDENZA,
                                                    COMUNERESIDENZA = adf_Old.COMUNERESIDENZA,
                                                    PROVINCIARESIDENZA = adf_Old.PROVINCIARESIDENZA,
                                                    DATAAGGIORNAMENTO = adf_Old.DATAAGGIORNAMENTO,
                                                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                                    FK_IDALTRIDATIFAM = adf_Old.FK_IDALTRIDATIFAM
                                                };

                                                db.ALTRIDATIFAM.Add(adfNew);///La consolido e l'associo al coniuge attivo
                                                adf_Old.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                                int j3 = db.SaveChanges();

                                                if (j3 > 0)
                                                {
                                                    /// associo la nuova riga di altri dati familiari alla nuova riga del ciclo di autorizzazione.
                                                    dtamf.AssociaAltriDatiFamiliari(amfNew.IDATTIVAZIONEMAGFAM, adfNew.IDALTRIDATIFAM, db);
                                                }
                                                else
                                                {
                                                    throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione altri dati familiari per il coniuge.");
                                                }
                                            }
                                        }
                                        #endregion

                                        //#region annulla solo residui adf figli
                                        //if (amfOld.FIGLI.Count() == 0)
                                        //{
                                        //    var ladfOld_figli =
                                        //        amfOld.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare)
                                        //                        .OrderByDescending(a => a.IDALTRIDATIFAM);

                                        //    if (ladfOld_figli?.Any() ?? false)///Esiste questo controllo ma è impossibile che si verifichi il contrario perché gli altri dati familiari sono obbligatori per attivare il ciclo di autorizzazione.
                                        //    {
                                        //        foreach (var adfOld_figli in ladfOld_figli)
                                        //        {
                                        //            //var adfOld = ladfOld.First();
                                        //            ///Creo una nuova riga di altri dati familiari identica alla vechia riga.
                                        //            ALTRIDATIFAM adfNew_figli = new ALTRIDATIFAM()
                                        //            {
                                        //                IDFIGLI = adfOld_figli.IDFIGLI,
                                        //                DATANASCITA = adfOld_figli.DATANASCITA,
                                        //                CAPNASCITA = adfOld_figli.CAPNASCITA,
                                        //                COMUNENASCITA = adfOld_figli.COMUNENASCITA,
                                        //                PROVINCIANASCITA = adfOld_figli.PROVINCIANASCITA,
                                        //                NAZIONALITA = adfOld_figli.NAZIONALITA,
                                        //                INDIRIZZORESIDENZA = adfOld_figli.INDIRIZZORESIDENZA,
                                        //                CAPRESIDENZA = adfOld_figli.CAPRESIDENZA,
                                        //                COMUNERESIDENZA = adfOld_figli.COMUNERESIDENZA,
                                        //                PROVINCIARESIDENZA = adfOld_figli.PROVINCIARESIDENZA,
                                        //                DATAAGGIORNAMENTO = adfOld_figli.DATAAGGIORNAMENTO,
                                        //                IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                        //                FK_IDALTRIDATIFAM = adfOld_figli.FK_IDALTRIDATIFAM
                                        //            };

                                        //            db.ALTRIDATIFAM.Add(adfNew_figli);///La consolido e l'associo al coniuge attivo
                                        //            adfOld_figli.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                        //            int j3 = db.SaveChanges();

                                        //            if (j3 > 0)
                                        //            {
                                        //                /// associo la nuova riga di altri dati familiari alla nuova riga del ciclo di autorizzazione.
                                        //                dtamf.AssociaAltriDatiFamiliari(amfNew.IDATTIVAZIONEMAGFAM, adfNew_figli.IDALTRIDATIFAM, db);
                                        //            }
                                        //            else
                                        //            {
                                        //                throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione altri dati familiari per i figli.");
                                        //            }
                                        //        }
                                        //    }
                                        //}
                                        //#endregion


                                        #region annulla solo doc identita
                                        ///Prelevo tutti i vecchi documenti d'identità.
                                        var ldocOld =
                                            amfOld.DOCUMENTI.Where(
                                                a =>
                                                    a.MODIFICATO == false &&
                                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare &&
                                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita);

                                        foreach (DOCUMENTI docOld in ldocOld)
                                        {
                                            ///Creo la nuova riga per il documento con l'informazioni della vecchia riga.
                                            DOCUMENTI dNew = new DOCUMENTI()
                                            {
                                                IDTIPODOCUMENTO = docOld.IDTIPODOCUMENTO,
                                                NOMEDOCUMENTO = docOld.NOMEDOCUMENTO,
                                                ESTENSIONE = docOld.ESTENSIONE,
                                                FILEDOCUMENTO = docOld.FILEDOCUMENTO,
                                                DATAINSERIMENTO = docOld.DATAINSERIMENTO,
                                                MODIFICATO = docOld.MODIFICATO,
                                                FK_IDDOCUMENTO = docOld.FK_IDDOCUMENTO,
                                                IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                            };

                                            db.DOCUMENTI.Add(dNew);///Consolido il documento associandolo alla nuova riga del coniuge.
                                            docOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                            int j4 = db.SaveChanges();

                                            if (j4 > 0)
                                            {
                                                dtamf.AssociaDocumentoAttivazione(amfNew.IDATTIVAZIONEMAGFAM, dNew.IDDOCUMENTO, db);
                                                using (dtDocumenti dtd = new dtDocumenti())
                                                {
                                                    if (docOld.CONIUGE.Count() > 0)
                                                    {
                                                        dtd.AssociaDocumentoConiuge(docOld.CONIUGE.First().IDCONIUGE, dNew.IDDOCUMENTO, db);
                                                    }
                                                    if (docOld.FIGLI.Count() > 0)
                                                    {
                                                        dtd.AssociaDocumentoFiglio(docOld.FIGLI.First().IDFIGLI, dNew.IDDOCUMENTO, db);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione documenti d'identità");
                                            }
                                        }
                                        #endregion

                                        #region annulla solo pensione (da fare)
                                        #endregion

                                    }
                                }
                                else
                                {
                                    throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione ciclo autorizzazione.");
                                }

                                EmailTrasferimento.EmailAnnulla(amfOld.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                                Resources.msgEmail.OggettoAnnullaRichiestaMaggiorazioniFamiliari,
                                                                testoAnnullaMF,
                                                                db);
                                //this.EmailAnnullaRichiesta(idAttivazioneMagFam, testoAnnullaTrasf, db);

                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.AnnullaMessaggioEvento(amfOld.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari, db);
                                }

                            }
                            else
                            {
                                throw new Exception("Errore nella fase di annullamento della riga di attivazione maggiorazione familiare per l'id: " + amfOld.IDATTIVAZIONEMAGFAM);
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
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void VerificaPensioniAttiveInLavorazione(out PensioneConiugeModel pcm_dacanc, PensioneConiugeModel pcm, decimal idConiuge, ModelDBISE db)
        {
            try
            {
                PensioneConiugeModel pcm_appo = new PensioneConiugeModel();

                var c = db.CONIUGE.Find(idConiuge);

                var amf = GetAttivazioneById(idConiuge, EnumTipoTabella.Coniuge);
                if (amf.RICHIESTAATTIVAZIONE)
                {
                    amf = GetAttivazioneAperta(c.IDMAGGIORAZIONIFAMILIARI);
                    if (amf.IDATTIVAZIONEMAGFAM > 0 == false)
                    {
                        amf = CreaAttivazione(c.IDMAGGIORAZIONIFAMILIARI, db);
                    }
                }

            
                var lpc = c.PENSIONE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                if (lpc.Count() == 0)
                {
                    lpc = c.PENSIONE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                    foreach (var pc in lpc)
                    {
                        PENSIONE new_pc = new PENSIONE()
                        {
                            IMPORTOPENSIONE = pc.IMPORTOPENSIONE,
                            DATAINIZIO = pc.DATAINIZIO,
                            DATAFINE = pc.DATAFINE,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                            FK_IDPENSIONE = pc.FK_IDPENSIONE
                        };
                        c.PENSIONE.Add(new_pc);
                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore in fase di inserimento pensione.");
                        }
                        using (dtPensione dtp = new dtPensione())
                        {
                            dtp.AssociaPensioneAttivazioneMagFam(amf.IDATTIVAZIONEMAGFAM, new_pc.IDPENSIONE, db);
                        }
                        if (pc.IDPENSIONE==pcm.idPensioneConiuge)
                        {
                            pcm_appo = new PensioneConiugeModel()
                            {
                                dataFineValidita=new_pc.DATAFINE,
                                dataInizioValidita=new_pc.DATAINIZIO,
                                idPensioneConiuge=new_pc.IDPENSIONE,
                                dataAggiornamento=DateTime.Now,
                                idStatoRecord=new_pc.IDSTATORECORD,
                                importoPensione=new_pc.IMPORTOPENSIONE
                            };
                        }
                    }
                }

                pcm_dacanc = pcm_appo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void NotificaRichiestaVariazione(decimal idAttivazioneMagFam)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);
                        amf.RICHIESTAATTIVAZIONE = true;
                        amf.DATARICHIESTAATTIVAZIONE = DateTime.Now;
                        amf.DATAAGGIORNAMENTO = DateTime.Now;

                        var i = db.SaveChanges();
                        if (i <= 0)
                        {
                            throw new Exception("Errore nella fase d'inserimento per la richiesta attivazione per le maggiorazioni familiari.");
                        }
                        else
                        {
                            //cerca coniuge in lavorazione e lo mette da attivare
                            var lc = amf.CONIUGE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                            foreach (var c in lc)
                            {
                                c.IDSTATORECORD = (decimal)EnumStatoRecord.Da_Attivare;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore in fase di richiesta attivazione per le maggiorazioni familiari (coniuge).");
                                }

                            }
                            //cerca figlio in lavorazione e lo mette da attivare
                            var lf = amf.FIGLI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                            foreach (var f in lf)
                            {
                                f.IDSTATORECORD = (decimal)EnumStatoRecord.Da_Attivare;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore in fase di richiesta attivazione per le maggiorazioni familiari (figli).");
                                }

                            }
                            //cerca documenti in lavorazione e lo mette da attivare
                            var ld = amf.DOCUMENTI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                            foreach (var d in ld)
                            {
                                d.IDSTATORECORD = (decimal)EnumStatoRecord.Da_Attivare;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore in fase di richiesta attivazione per le maggiorazioni familiari (documenti).");
                                }

                            }
                            //cerca altri dati in lavorazione e lo mette da attivare
                            var ladf = amf.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                            foreach (var adf in ladf)
                            {
                                adf.IDSTATORECORD = (decimal)EnumStatoRecord.Da_Attivare;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore in fase di richiesta attivazione per le maggiorazioni familiari (altri dati familiari).");
                                }

                            }
                            // manca pensione coniuge
                            //cerca pensione in lavorazione e lo mette da attivare
                            var lp = amf.PENSIONE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                            foreach (var p in lp)
                            {
                                p.IDSTATORECORD = (decimal)EnumStatoRecord.Da_Attivare;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore in fase di richiesta attivazione per le maggiorazioni familiari (pensione).");
                                }

                            }



                            using (dtDipendenti dtd = new dtDipendenti())
                            {
                                using (dtTrasferimento dtt = new dtTrasferimento())
                                {
                                    using (dtUffici dtu = new dtUffici())
                                    {
                                        var t = dtt.GetTrasferimentoById(amf.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO);

                                        if (t?.idTrasferimento > 0)
                                        {
                                            var dip = dtd.GetDipendenteByID(t.idDipendente);
                                            var uff = dtu.GetUffici(t.idUfficio);

                                            EmailTrasferimento.EmailNotifica(EnumChiamante.Maggiorazioni_Familiari,
                                                                            amf.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                                            Resources.msgEmail.OggettoNotificaRichiestaMaggiorazioniFamiliari,
                                                                            string.Format(Resources.msgEmail.MessaggioNotificaRichiestaMaggiorazioniFamiliari, dip.cognome + " " + dip.nome + " (" + dip.matricola + ")", t.dataPartenza.ToShortDateString(), uff.descUfficio + " (" + uff.codiceUfficio + ")"),
                                                                            db);
                                        }
                                    }
                                }
                            }


                            //this.EmailNotificaRichiesta(idAttivazioneMagFam, db);

                            using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                            {
                                CalendarioEventiModel cem = new CalendarioEventiModel()
                                {
                                    idFunzioneEventi = EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari,
                                    idTrasferimento = amf.MAGGIORAZIONIFAMILIARI.IDMAGGIORAZIONIFAMILIARI,
                                    DataInizioEvento = DateTime.Now.Date,
                                    DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaMaggiorazioniFamiliari)).Date,
                                };

                                dtce.InsertCalendarioEvento(ref cem, db);
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AltriDatiFamConiugeModel GetAltriDatiFamiliariConiugeByID(decimal idAltriDatiFam)
        {
            AltriDatiFamConiugeModel adfm = new AltriDatiFamConiugeModel();
            DateTime dt = DateTime.Now;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var adf = db.ALTRIDATIFAM.Find(idAltriDatiFam);

                    if (adf?.IDALTRIDATIFAM > 0)
                    {
                        var latt = adf.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO==false).OrderByDescending(a=>a.IDATTIVAZIONEMAGFAM).ToList();

                        if (latt?.Any() ?? false)
                        {
                            var att = latt.First();

                            var lc = adf.CONIUGE.Where(a => a.IDSTATORECORD!=(decimal)EnumStatoRecord.Annullato).OrderByDescending(a=>a.IDCONIUGE).ToList();

                            if (lc?.Any() ?? false)
                            {
                                var c = lc.First();

                                adfm = new AltriDatiFamConiugeModel()
                                {
                                    idAltriDatiFam = adf.IDALTRIDATIFAM,
                                    idConiuge = c.IDCONIUGE,
                                    nazionalita = adf.NAZIONALITA,
                                    indirizzoResidenza = adf.INDIRIZZORESIDENZA,
                                    capResidenza = adf.CAPRESIDENZA,
                                    comuneResidenza = adf.COMUNERESIDENZA,
                                    provinciaResidenza = adf.PROVINCIARESIDENZA,
                                    dataAggiornamento = adf.DATAAGGIORNAMENTO,
                                    idStatoRecord = adf.IDSTATORECORD,
                                    FK_idAltriDatiFam = adf.FK_IDALTRIDATIFAM,
                                    Coniuge = new ConiugeModel()
                                    {
                                        idConiuge = c.IDCONIUGE,
                                        idTipologiaConiuge = (EnumTipologiaConiuge)c.IDTIPOLOGIACONIUGE,
                                        idMaggiorazioniFamiliari = c.IDMAGGIORAZIONIFAMILIARI,
                                        nome = c.NOME,
                                        cognome = c.COGNOME,
                                        codiceFiscale = c.CODICEFISCALE,
                                        dataInizio = c.DATAINIZIOVALIDITA,
                                        dataFine = c.DATAFINEVALIDITA,
                                        dataAggiornamento = c.DATAAGGIORNAMENTO,
                                        idStatoRecord = c.IDSTATORECORD,
                                        FK_idConiuge = c.FK_IDCONIUGE
                                    }
                                };

                                switch ((EnumTipologiaConiuge)adfm.Coniuge.idTipologiaConiuge)
                                {
                                    case EnumTipologiaConiuge.Residente:
                                        adfm.residente = true;
                                        adfm.ulterioreMagConiuge = false;
                                        break;
                                    case EnumTipologiaConiuge.NonResidente_A_Carico:
                                        adfm.residente = false;
                                        adfm.ulterioreMagConiuge = true;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                            else
                            {
                                throw new Exception("Errore nella ricerca del coniuge.");
                            }
                        }
                    }
                }

                return adfm;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public AltriDatiFamFiglioModel GetAltriDatiFamiliariFiglioByID(decimal idAltriDatiFam)
        {
            AltriDatiFamFiglioModel adfm = new AltriDatiFamFiglioModel();
            //DateTime dt = DateTime.Now;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var adf = db.ALTRIDATIFAM.Find(idAltriDatiFam);

                    if (adf?.IDALTRIDATIFAM > 0)
                    {
                        var latt = adf.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                        if (latt?.Any() ?? false)
                        {
                            var att = latt.First();

                            var lf = adf.FIGLI.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDFIGLI).ToList();

                            if (lf?.Any() ?? false)
                            {
                                var f = lf.First();

                                adfm = new AltriDatiFamFiglioModel()
                                {
                                    idAltriDatiFam = adf.IDALTRIDATIFAM,
                                    idFigli = f.IDFIGLI,
                                    dataNascita = adf.DATANASCITA,
                                    capNascita = adf.CAPNASCITA,
                                    comuneNascita = adf.COMUNENASCITA,
                                    provinciaNascita = adf.PROVINCIANASCITA,
                                    nazionalita = adf.NAZIONALITA,
                                    indirizzoResidenza = adf.INDIRIZZORESIDENZA,
                                    capResidenza = adf.CAPRESIDENZA,
                                    comuneResidenza = adf.COMUNERESIDENZA,
                                    provinciaResidenza = adf.PROVINCIARESIDENZA,
                                    dataAggiornamento = adf.DATAAGGIORNAMENTO,
                                    idStatoRecord = adf.IDSTATORECORD,
                                    FK_idAltriDatiFam = adf.FK_IDALTRIDATIFAM,
                                    Figli = new FigliModel()
                                    {
                                        idFigli = f.IDFIGLI,
                                        idTipologiaFiglio = (EnumTipologiaFiglio)f.IDTIPOLOGIAFIGLIO,
                                        idMaggiorazioniFamiliari = f.IDMAGGIORAZIONIFAMILIARI,
                                        nome = f.NOME,
                                        cognome = f.COGNOME,
                                        codiceFiscale = f.CODICEFISCALE,
                                        dataInizio = f.DATAINIZIOVALIDITA,
                                        dataFine = f.DATAFINEVALIDITA,
                                        dataAggiornamento = f.DATAAGGIORNAMENTO,
                                        idStatoRecord = f.IDSTATORECORD,
                                        FK_IdFigli = f.FK_IDFIGLI
                                    }
                                };


                                switch ((EnumTipologiaFiglio)adfm.Figli.idTipologiaFiglio)
                                {
                                    case EnumTipologiaFiglio.Residente:
                                        adfm.residente = true;
                                        adfm.studente = false;
                                        break;
                                    case EnumTipologiaFiglio.StudenteResidente:
                                        adfm.residente = true;
                                        adfm.studente = true;
                                        break;
                                    case EnumTipologiaFiglio.StudenteNonResidente:
                                        adfm.residente = false;
                                        adfm.studente = true;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }

                            }
                            else
                            {
                                throw new Exception("Errore nella ricerca del figlio.");
                            }
                        }





                    }
                }

                return adfm;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool ConiugeModificabile(decimal idConiuge, decimal idMaggiorazioniFamiliari)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    bool modificabile = true;


                    var lc = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari)
                                   .CONIUGE.Where(y =>
                                           y.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato
                                   ).OrderByDescending(a=>a.IDCONIUGE).ToList();
                    var last_coniuge = lc.First();
                    if (db.CONIUGE.Find(idConiuge).DATAFINEVALIDITA != Utility.DataFineStop() && idConiuge != last_coniuge.IDCONIUGE)
                    {
                        modificabile = false;
                    }
                    return modificabile;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
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
                                       out bool datiConiuge, out bool datiParzialiConiuge,
                                       out bool datiFigli, out bool datiParzialiFigli,
                                       out bool siDocConiuge, out bool siDocFigli,
                                       out bool docFormulario, out bool inLavorazione,
                                       out bool trasfSolaLettura)
        {
            rinunciaMagFam = false;
            richiestaAttivazione = false;
            Attivazione = false;
            datiConiuge = false;
            datiParzialiConiuge = false;
            datiFigli = false;
            datiParzialiFigli = false;
            siDocConiuge = false;
            siDocFigli = false;
            docFormulario = false;
            inLavorazione = false;
            trasfSolaLettura = false;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);
                    var lrmf = mf.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a=>a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();
                    if(lrmf?.Any()??false)
                    {
                        var rmf = lrmf.First();
                        if(rmf.RINUNCIAMAGGIORAZIONI)
                        {
                            rinunciaMagFam = true;
                        }
                    }

                    decimal IDstatoTrasf = mf.TRASFERIMENTO.IDSTATOTRASFERIMENTO;
                    if (IDstatoTrasf == (decimal)EnumStatoTraferimento.Annullato)
                    {
                        trasfSolaLettura = true;
                    }

                    var conta_attivazioni = mf.ATTIVAZIONIMAGFAM.Where(a => (a.ANNULLATO == false || (a.RICHIESTAATTIVAZIONE == true && a.ATTIVAZIONEMAGFAM == true))).Count();

                    if (conta_attivazioni > 1)
                    {
                        //legge l'ultima attivazione valida
                        var last_amf = mf.ATTIVAZIONIMAGFAM.Where(a => (a.ANNULLATO == false || (a.RICHIESTAATTIVAZIONE == true && a.ATTIVAZIONEMAGFAM == true))).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).First();

                        if (last_amf != null || last_amf.IDATTIVAZIONEMAGFAM > 0)
                        {
                            //elenca le attivazioni aperte
                            var lamf = mf.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false && a.ATTIVAZIONEMAGFAM == false && a.RICHIESTAATTIVAZIONE == false).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                            //se ci sono esegue i controlli
                            if (lamf?.Any() ?? false)
                            {
                                foreach (var amf in lamf)
                                {
                                    if (amf != null && amf.IDATTIVAZIONEMAGFAM > 0)
                                    {
                                        var rmf =
                                            mf.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                                .OrderByDescending(a => a.IDRINUNCIAMAGFAM)
                                                .First();

                                        rinunciaMagFam = rmf.RINUNCIAMAGGIORAZIONI;
                                        richiestaAttivazione = amf.RICHIESTAATTIVAZIONE;
                                        Attivazione = amf.ATTIVAZIONEMAGFAM;

                                        var ld = amf.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari).ToList();
                                        if (ld?.Any() ?? false)
                                        {
                                            docFormulario = true;
                                        }

                                        if (mf.CONIUGE != null)
                                        {
                                            var lc = mf.CONIUGE.ToList();
                                            if (lc?.Any() ?? false)
                                            {
                                                datiConiuge = true;

                                                foreach (var c in lc)
                                                {
                                                    var nadc = c.ALTRIDATIFAM.Count(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato);

                                                    if (nadc > 0)
                                                    {
                                                        datiParzialiConiuge = false;
                                                    }
                                                    else
                                                    {
                                                        datiParzialiConiuge = true;
                                                        break;
                                                    }
                                                }
                                                foreach (var c in lc)
                                                {
                                                    var ndocc = c.DOCUMENTI.Count;

                                                    if (ndocc > 0)
                                                    {
                                                        siDocConiuge = true;
                                                    }
                                                    else
                                                    {
                                                        siDocConiuge = false;
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                datiConiuge = false;
                                            }
                                        }

                                        if (mf.FIGLI != null)
                                        {
                                            var lf = mf.FIGLI.ToList();

                                            if (lf?.Any() ?? false)
                                            {
                                                datiFigli = true;

                                                foreach (var f in lf)
                                                {
                                                    var nadf = f.ALTRIDATIFAM.Count(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato);

                                                    if (nadf > 0)
                                                    {
                                                        datiParzialiFigli = false;
                                                    }
                                                    else
                                                    {
                                                        datiParzialiFigli = true;
                                                        break;
                                                    }
                                                }

                                                foreach (var f in lf)
                                                {
                                                    var ndocf = f.DOCUMENTI.Count;
                                                    if (ndocf > 0)
                                                    {
                                                        siDocFigli = true;
                                                    }
                                                    else
                                                    {
                                                        siDocFigli = false;
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                datiFigli = false;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // se non ci sono in lavorazione imposta i controlli di notifica
                                var rmf = mf.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                               .OrderByDescending(a => a.IDRINUNCIAMAGFAM)
                                               .First();
                                rinunciaMagFam = rmf.RINUNCIAMAGGIORAZIONI;
                                richiestaAttivazione = last_amf.RICHIESTAATTIVAZIONE;
                                Attivazione = last_amf.ATTIVAZIONEMAGFAM;

                                //comunque esegue i controlli sui dati dell'attivazione
                                var ld = last_amf.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari).ToList();
                                if (ld?.Any() ?? false)
                                {
                                    docFormulario = true;
                                }

                                if (mf.CONIUGE != null)
                                {
                                    var lc = mf.CONIUGE.ToList();
                                    if (lc?.Any() ?? false)
                                    {
                                        datiConiuge = true;

                                        foreach (var c in lc)
                                        {
                                            var nadc = c.ALTRIDATIFAM.Count(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato);

                                            if (nadc > 0)
                                            {
                                                datiParzialiConiuge = false;
                                            }
                                            else
                                            {
                                                datiParzialiConiuge = true;
                                                break;
                                            }
                                        }
                                        foreach (var c in lc)
                                        {
                                            var ndocc = c.DOCUMENTI.Count;

                                            if (ndocc > 0)
                                            {
                                                siDocConiuge = true;
                                            }
                                            else
                                            {
                                                siDocConiuge = false;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        datiConiuge = false;
                                    }
                                }

                                if (mf.FIGLI != null)
                                {
                                    var lf = mf.FIGLI.ToList();

                                    if (lf?.Any() ?? false)
                                    {
                                        datiFigli = true;

                                        foreach (var f in lf)
                                        {
                                            var nadf = f.ALTRIDATIFAM.Count(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato);

                                            if (nadf > 0)
                                            {
                                                datiParzialiFigli = false;
                                            }
                                            else
                                            {
                                                datiParzialiFigli = true;
                                                break;
                                            }
                                        }

                                        foreach (var f in lf)
                                        {
                                            var ndocf = f.DOCUMENTI.Count;
                                            if (ndocf > 0)
                                            {
                                                siDocFigli = true;
                                            }
                                            else
                                            {
                                                siDocFigli = false;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        datiFigli = false;
                                    }
                                }
                            }
                        }
                        if (richiestaAttivazione == false)
                        {
                            if (
                                (
                                    (datiConiuge && siDocConiuge && datiParzialiConiuge == false) || (datiFigli && siDocFigli && datiParzialiFigli == false)
                                ) ||
                                (
                                    (!datiConiuge && !siDocConiuge && datiParzialiConiuge) || (!datiFigli && !siDocFigli && datiParzialiFigli) && docFormulario
                                )
                               )
                            {
                                inLavorazione = true;
                            }
                        }

                    }
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
                                       out bool docFormulario)
        {
            rinunciaMagFam = false;
            richiestaAttivazione = false;
            Attivazione = false;
            datiConiuge = false;
            datiParzialiConiuge = false;
            datiFigli = false;
            datiParzialiFigli = false;
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

                        var ld = amf.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari).ToList();
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
                                    var nadc = c.ALTRIDATIFAM.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.IDCONIUGE > 0).Count();
                                    if (nadc > 0)
                                    {
                                        datiParzialiConiuge = false;
                                    }
                                    else
                                    {
                                        datiParzialiConiuge = true;
                                    }

                                    var ndocc = c.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita).Count();
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
                                    var nadf = f.ALTRIDATIFAM.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.IDFIGLI > 0).Count();
                                    if (nadf > 0)
                                    {
                                        datiParzialiFigli = false;
                                    }
                                    else
                                    {
                                        datiParzialiFigli = true;
                                    }

                                    var ndocf = f.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita).Count();
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
                            out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione, out trasfSolaLettura);

                        //leggo l'ultima attivazione valida
                        var last_attivazione_coniuge = this.GetAttivazioneById(cm.idConiuge, EnumTipoTabella.Coniuge);

                        //leggo se esiste una attivazione in lavorazione
                        var attivazione_aperta = this.GetAttivazioneAperta(cm.idMaggiorazioniFamiliari);
                        if (attivazione_aperta != null && attivazione_aperta.IDATTIVAZIONEMAGFAM > 0)
                        {
                            if (attivazione_aperta.IDATTIVAZIONEMAGFAM != last_attivazione_coniuge.IDATTIVAZIONEMAGFAM)
                            {
                                // crea nuovo coniuge e associa attivazione in lavorazione
                                ConiugeModel newc = new ConiugeModel()
                                {
                                    idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari,
                                    idTipologiaConiuge = cm.idTipologiaConiuge,
                                    nome = cm.nome,
                                    cognome = cm.cognome,
                                    codiceFiscale = cm.codiceFiscale,
                                    dataInizio = cm.dataInizio.Value,
                                    dataFine = dtFin,
                                    escludiPassaporto = cm.escludiPassaporto,
                                    dataNotificaPP = cm.dataNotificaPP,
                                    escludiTitoloViaggio = cm.escludiTitoloViaggio,
                                    dataNotificaTV = cm.dataNotificaTV,
                                    FK_idConiuge = cm.idConiuge,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione
                                };

                                decimal new_idconiuge = this.SetConiuge(ref newc, db, attivazione_aperta.IDATTIVAZIONEMAGFAM);

                                //replico eventuali altri dati familiari e li associo
                                var adfc = this.GetAdfValidiByIDConiuge(cm.idConiuge, cm.idStatoRecord);

                                //var adfc = c.ALTRIDATIFAM.OrderByDescending(a=>a.IDALTRIDATIFAM).First();
                                ALTRIDATIFAM adf_new = new ALTRIDATIFAM()
                                {
                                    IDCONIUGE = new_idconiuge,
                                    CAPNASCITA="VUOTO",
                                    COMUNENASCITA="VUOTO",
                                    PROVINCIANASCITA="VUOTO",
                                    PROVINCIARESIDENZA = adfc.provinciaResidenza,
                                    COMUNERESIDENZA = adfc.comuneResidenza,
                                    NAZIONALITA = adfc.nazionalita,
                                    INDIRIZZORESIDENZA = adfc.indirizzoResidenza,
                                    CAPRESIDENZA = adfc.capResidenza,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                };
                                db.ALTRIDATIFAM.Add(adf_new);

                                if (db.SaveChanges() > 0)
                                {
                                    this.AssociaAltriDatiFamiliariConiuge(new_idconiuge, adf_new.IDALTRIDATIFAM, db);

                                    using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                                    {
                                        dtamf.AssociaAltriDatiFamiliari(attivazione_aperta.IDATTIVAZIONEMAGFAM, adf_new.IDALTRIDATIFAM, db);
                                    }

                                    //riassocia eventuali documenti
                                    var ldc = db.CONIUGE.Find(cm.idConiuge).DOCUMENTI.Where(x => x.MODIFICATO == false).ToList();
                                    foreach (var dc in ldc)
                                    {
                                        this.Associa_Doc_Coniuge_ById(dc.IDDOCUMENTO, new_idconiuge, db);
                                    }
                                    //riassocia eventuali pensioni
                                    var lpc = db.CONIUGE.Find(cm.idConiuge).PENSIONE.Where(x => x.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();
                                    foreach (var pc in lpc)
                                    {
                                        this.Associa_Pensioni_Coniuge_ById(pc.IDPENSIONE, new_idconiuge, db);
                                    }

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
                                                ANNULLATO = ctv.ANNULLATO
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
                                                ANNULLATO = cp.ANNULLATO
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


                                }
                            }
                            else
                            {
                                c.DATAINIZIOVALIDITA = cm.dataInizio.Value;
                                c.DATAFINEVALIDITA = dtFin;
                                c.IDTIPOLOGIACONIUGE = (decimal)cm.idTipologiaConiuge;
                                c.NOME = cm.nome;
                                c.COGNOME = cm.cognome;
                                c.CODICEFISCALE = cm.codiceFiscale;
                                int i = db.SaveChanges();

                                if (i <= 0)
                                {
                                    throw new Exception("Impossibile modificare il coniuge.");
                                }
                                else
                                {
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
                                }
                            }
                        }
                        else
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
                                throw new Exception("Impossibile modificare il coniuge.");
                            }

                            ConiugeModel newc = new ConiugeModel()
                            {
                                idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari,
                                idTipologiaConiuge = cm.idTipologiaConiuge,
                                nome = cm.nome,
                                cognome = cm.cognome,
                                codiceFiscale = cm.codiceFiscale,
                                dataInizio = cm.dataInizio.Value,
                                dataFine = dtFin,
                                escludiPassaporto = cm.escludiPassaporto,
                                dataNotificaPP = cm.dataNotificaPP,
                                escludiTitoloViaggio = cm.escludiTitoloViaggio,
                                dataNotificaTV = cm.dataNotificaTV,
                                FK_idConiuge = cm.idConiuge,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione
                            };

                            decimal new_idconiuge = this.SetConiuge(ref newc, db, newmf.IDATTIVAZIONEMAGFAM);

                            //replico eventuali altri dati familiari e li associo
                            var adfc = this.GetAdfValidiByIDConiuge(cm.idConiuge, cm.idStatoRecord);
                            //var adfc = c.ALTRIDATIFAM.OrderByDescending(a=>a.IDALTRIDATIFAM).First();
                            ALTRIDATIFAM adf_new = new ALTRIDATIFAM()
                            {
                                IDCONIUGE = new_idconiuge,
                                CAPNASCITA = "VUOTO",
                                COMUNENASCITA = "VUOTO",
                                PROVINCIANASCITA = "VUOTO",
                                PROVINCIARESIDENZA = adfc.provinciaResidenza,
                                COMUNERESIDENZA = adfc.comuneResidenza,
                                NAZIONALITA = adfc.nazionalita,
                                INDIRIZZORESIDENZA = adfc.indirizzoResidenza,
                                CAPRESIDENZA = adfc.capResidenza,
                                DATAAGGIORNAMENTO = DateTime.Now,
                                IDSTATORECORD = adfc.idStatoRecord
                            };
                            db.ALTRIDATIFAM.Add(adf_new);

                            if (db.SaveChanges() > 0)
                            {
                                this.AssociaAltriDatiFamiliariConiuge(new_idconiuge, adf_new.IDALTRIDATIFAM, db);

                                using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                                {
                                    dtamf.AssociaAltriDatiFamiliari(newmf.IDATTIVAZIONEMAGFAM, adf_new.IDALTRIDATIFAM, db);
                                }


                                //riassocia eventuali documenti
                                var ldc = db.CONIUGE.Find(cm.idConiuge).DOCUMENTI.Where(x => x.MODIFICATO == false).ToList();
                                foreach (var dc in ldc)
                                {
                                    this.Associa_Doc_Coniuge_ById(dc.IDDOCUMENTO, new_idconiuge, db);
                                }
                                //riassocia eventuali pensioni
                                var lpc = db.CONIUGE.Find(cm.idConiuge).PENSIONE.Where(x => x.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();
                                foreach (var pc in lpc)
                                {
                                    this.Associa_Pensioni_Coniuge_ById(pc.IDPENSIONE, new_idconiuge, db);
                                }

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
                                            ANNULLATO = ctv.ANNULLATO
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
                                            ANNULLATO = cp.ANNULLATO
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
                            }
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
                    DATAAGGIORNAMENTO = cm.dataAggiornamento,
                    FK_IDCONIUGE = cm.FK_idConiuge,
                    IDSTATORECORD = cm.idStatoRecord
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
                            out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione, out trasfSolaLettura);

                        //leggo l'ultima attivazione valida
                        var last_attivazione_figlio = this.GetAttivazioneById(fm.idFigli, EnumTipoTabella.Figli);

                        //leggo se esiste una attivazione in lavorazione
                        var attivazione_aperta = this.GetAttivazioneAperta(fm.idMaggiorazioniFamiliari);
                        if (attivazione_aperta != null && attivazione_aperta.IDATTIVAZIONEMAGFAM > 0)
                        {
                            if (attivazione_aperta.IDATTIVAZIONEMAGFAM != last_attivazione_figlio.IDATTIVAZIONEMAGFAM)
                            {
                                // crea nuovo figlio e associa attivazione in lavorazione
                                FigliModel newf = new FigliModel()
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
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione
                                };

                                decimal new_idfiglio = this.SetFiglio(ref newf, db, attivazione_aperta.IDATTIVAZIONEMAGFAM);

                                //replico eventuali altri dati familiari e li associo
                                var adff = f.ALTRIDATIFAM.First();
                                ALTRIDATIFAM adf_new = new ALTRIDATIFAM()
                                {
                                    IDFIGLI = new_idfiglio,
                                    DATANASCITA = adff.DATANASCITA,
                                    CAPNASCITA = adff.CAPNASCITA,
                                    COMUNENASCITA = adff.COMUNENASCITA,
                                    PROVINCIANASCITA = adff.PROVINCIANASCITA,
                                    PROVINCIARESIDENZA = adff.PROVINCIARESIDENZA,
                                    COMUNERESIDENZA = adff.COMUNERESIDENZA,
                                    NAZIONALITA = adff.NAZIONALITA,
                                    INDIRIZZORESIDENZA = adff.INDIRIZZORESIDENZA,
                                    CAPRESIDENZA = adff.CAPRESIDENZA,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                };
                                db.ALTRIDATIFAM.Add(adf_new);

                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore durante la creazione nuovo record figli.");
                                }

                                this.AssociaAltriDatiFamiliariFiglio(new_idfiglio, adf_new.IDALTRIDATIFAM, db);

                                //riassocia eventuali documenti
                                var ldf = db.FIGLI.Find(fm.idFigli).DOCUMENTI.Where(x => x.MODIFICATO == false).ToList();
                                foreach (var df in ldf)
                                {
                                    this.Associa_Doc_Figlio_ById(df.IDDOCUMENTO, new_idfiglio, db);
                                }

                                //associa le percentuali maggiorazioni
                                using (dtPercentualeMagFigli dtpf = new dtPercentualeMagFigli())
                                {

                                    List<PercentualeMagFigliModel> lpmfm =
                                        dtpf.GetPercentualeMaggiorazioneFigli((EnumTipologiaFiglio)fm.idTipologiaFiglio, dtIni, dtFin, db).ToList();

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
                                            ANNULLATO = ftv.ANNULLATO
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
                                            ANNULLATO = fp.ANNULLATO
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

                            }
                            else
                            {
                                f.DATAINIZIOVALIDITA = fm.dataInizio.Value;
                                f.DATAFINEVALIDITA = dtFin;
                                f.IDTIPOLOGIAFIGLIO = (decimal)fm.idTipologiaFiglio;
                                f.NOME = fm.nome;
                                f.COGNOME = fm.cognome;
                                f.CODICEFISCALE = fm.codiceFiscale;
                                int i = db.SaveChanges();

                                if (i <= 0)
                                {
                                    throw new Exception("Impossibile modificare il figlio.");
                                }

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
                            }
                        }
                        else
                        {
                            //crea una nuova attivazione
                            var newamf = this.CreaAttivazione(idMaggiorazioniFamiliari,db);

                            f.IDSTATORECORD = (decimal)EnumStatoRecord.Attivato;

                            int idx = db.SaveChanges();

                            if (idx <= 0)
                            {
                                throw new Exception("Impossibile modificare il figlio.");
                            }

                            FigliModel newf = new FigliModel()
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
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione
                            };

                            decimal new_idfiglio = this.SetFiglio(ref newf, db, newamf.IDATTIVAZIONEMAGFAM);

                            //replico eventuali altri dati familiari e li associo
                            var adff = f.ALTRIDATIFAM.First();
                            ALTRIDATIFAM adf_new = new ALTRIDATIFAM()
                            {
                                IDFIGLI = new_idfiglio,
                                DATANASCITA = adff.DATANASCITA,
                                CAPNASCITA = adff.CAPNASCITA,
                                COMUNENASCITA = adff.COMUNENASCITA,
                                PROVINCIANASCITA = adff.PROVINCIANASCITA,
                                PROVINCIARESIDENZA = adff.PROVINCIARESIDENZA,
                                COMUNERESIDENZA = adff.COMUNERESIDENZA,
                                NAZIONALITA = adff.NAZIONALITA,
                                INDIRIZZORESIDENZA = adff.INDIRIZZORESIDENZA,
                                CAPRESIDENZA = adff.CAPRESIDENZA,
                                DATAAGGIORNAMENTO = DateTime.Now,
                                IDSTATORECORD = adff.IDSTATORECORD
                            };
                            db.ALTRIDATIFAM.Add(adf_new);

                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore durante la creazione del nuovo record altri dati familiari (figlio).");
                            }

                            this.AssociaAltriDatiFamiliariFiglio(new_idfiglio, adf_new.IDALTRIDATIFAM, db);

                            //riassocia eventuali documenti
                            var ldf = db.FIGLI.Find(fm.idFigli).DOCUMENTI.Where(x => x.MODIFICATO == false).ToList();
                            foreach (var df in ldf)
                            {
                                this.Associa_Doc_Figlio_ById(df.IDDOCUMENTO, new_idfiglio, db);
                            }

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
                                        ANNULLATO = ftv.ANNULLATO
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
                                        ANNULLATO = fp.ANNULLATO
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
                    DATAAGGIORNAMENTO = fm.dataAggiornamento,
                    FK_IDFIGLI = fm.FK_IdFigli,
                    IDSTATORECORD = fm.idStatoRecord
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
                    annullato = false
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
                        var ladfc = c.ALTRIDATIFAM.Where(x => x.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(x => x.IDALTRIDATIFAM).ToList();


                        if (ladfc?.Any() ?? false)
                        {
                            var adfc = ladfc.First();

                            adfm = new AltriDatiFamConiugeModel()
                            {
                                idAltriDatiFam = adfc.IDALTRIDATIFAM,
                                idConiuge = adfc.IDCONIUGE.Value,
                                nazionalita = adfc.NAZIONALITA,
                                indirizzoResidenza = adfc.INDIRIZZORESIDENZA,
                                capResidenza = adfc.CAPRESIDENZA,
                                comuneResidenza = adfc.COMUNERESIDENZA,
                                provinciaResidenza = adfc.PROVINCIARESIDENZA,
                                dataAggiornamento = adfc.DATAAGGIORNAMENTO,
                                idStatoRecord = adfc.IDSTATORECORD
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
                                idAltriDatiFam = adff.IDALTRIDATIFAM,
                                idFigli = adff.IDFIGLI.Value,
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
                                idStatoRecord = adff.IDSTATORECORD
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
                    var c = db.CONIUGE.Find(adfm.idConiuge);

                    var adf = db.ALTRIDATIFAM.Find(adfm.idAltriDatiFam);

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
                                IDCONIUGE = adfm.idConiuge,
                                DATANASCITA = DateTime.MinValue,
                                CAPNASCITA = "VUOTO",
                                COMUNENASCITA = "VUOTO",
                                PROVINCIANASCITA = "VUOTO",
                                NAZIONALITA = adfm.nazionalita,
                                INDIRIZZORESIDENZA = adfm.indirizzoResidenza,
                                CAPRESIDENZA = adfm.capResidenza,
                                COMUNERESIDENZA = adfm.comuneResidenza,
                                PROVINCIARESIDENZA = adfm.provinciaResidenza,
                                DATAAGGIORNAMENTO = adfm.dataAggiornamento,
                                IDSTATORECORD = adfm.idStatoRecord
                            };

                            db.ALTRIDATIFAM.Add(adfNew);

                            if (db.SaveChanges() > 0)
                            {
                                this.AssociaAltriDatiFamiliariConiuge(adfNew.IDCONIUGE, adfNew.IDALTRIDATIFAM, db);

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
                    var f = db.FIGLI.Find(adfm.idFigli);

                    var adf = db.ALTRIDATIFAM.Find(adfm.idAltriDatiFam);

                    ATTIVAZIONIMAGFAM attmf_aperta = new ATTIVAZIONIMAGFAM();

                    if (adf != null && adf.IDALTRIDATIFAM > 0)
                    {
                        var attmf_rif = this.GetAttivazioneById(f.IDFIGLI, EnumTipoTabella.Figli);

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
                                IDFIGLI = adfm.idFigli,
                                DATANASCITA = DateTime.MinValue,
                                CAPNASCITA = adfm.capNascita,
                                COMUNENASCITA = adfm.comuneNascita,
                                PROVINCIANASCITA = adfm.provinciaNascita,
                                NAZIONALITA = adfm.nazionalita,
                                INDIRIZZORESIDENZA = adfm.indirizzoResidenza,
                                CAPRESIDENZA = adfm.capResidenza,
                                COMUNERESIDENZA = adfm.comuneResidenza,
                                PROVINCIARESIDENZA = adfm.provinciaResidenza,
                                DATAAGGIORNAMENTO = adfm.dataAggiornamento,
                                IDSTATORECORD = adfm.idStatoRecord
                            };

                            db.ALTRIDATIFAM.Add(adfNew);

                            if (db.SaveChanges() > 0)
                            {
                                this.AssociaAltriDatiFamiliariFiglio(adfNew.IDFIGLI, adfNew.IDALTRIDATIFAM, db);

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

        public void AssociaAltriDatiFamiliariFiglio(decimal? idFiglio, decimal idAltriDatiFamiliari, ModelDBISE db)
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
                idMaggiorazioniFamiliari = c.MAGGIORAZIONIFAMILIARI.IDMAGGIORAZIONIFAMILIARI;
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

                lattmf = db.ATTIVAZIONIMAGFAM.Where(x => x.ANNULLATO == false && x.IDMAGGIORAZIONIFAMILIARI == IdMaggiorazioneFamiliare && x.RICHIESTAATTIVAZIONE == false && x.ATTIVAZIONEMAGFAM == false).ToList();
                if (lattmf?.Any() ?? false)
                {
                    attmf = db.ATTIVAZIONIMAGFAM.Where(x => x.ANNULLATO == false && x.IDMAGGIORAZIONIFAMILIARI == IdMaggiorazioneFamiliare && x.RICHIESTAATTIVAZIONE == false && x.ATTIVAZIONEMAGFAM == false).OrderByDescending(x => x.IDATTIVAZIONEMAGFAM).First();
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

            using (ModelDBISE db = new ModelDBISE())
            {
                c = db.CONIUGE.Find(idConiuge);

                lp = c.PENSIONE.Where(x => x.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();

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
                                ld = c.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc && a.MODIFICATO == false).OrderByDescending(a => a.IDDOCUMENTO).ToList();
                                break;
                            case EnumParentela.Figlio:
                                var f = db.FIGLI.Find(id);
                                ld = f.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc && a.MODIFICATO == false).OrderByDescending(a => a.IDDOCUMENTO).ToList();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("parentela");
                        }
                        break;
                    case EnumTipoDoc.Formulario_Maggiorazioni_Familiari:
                        ld = db.ATTIVAZIONIMAGFAM.Find(id)
                            .DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc && a.MODIFICATO == false)
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
                        if (latt?.Any()??false)
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
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var c = db.CONIUGE.Find(idConiuge);
                    if (c != null && c.IDCONIUGE > 0)
                    {
                        db.CONIUGE.Remove(c);

                        int i = db.SaveChanges();

                        if (i <= 0)
                        {
                            throw new Exception(string.Format("Impossibile eliminare il coniuge."));
                        }
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
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var f = db.FIGLI.Find(idFiglio);
                    if (f != null && f.IDFIGLI > 0)
                    {
                        db.FIGLI.Remove(f);

                        int i = db.SaveChanges();

                        if (i <= 0)
                        {
                            throw new Exception(string.Format("Impossibile eliminare il figlio."));
                        }
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
            bool rinunciaMagFam = false;
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

            int i = 0;

            this.SituazioneMagFamVariazione(idMaggiorazioneFamiliare, out rinunciaMagFam,
                out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione, out trasfSolaLettura);

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        if (rinunciaMagFam == true && richiestaAttivazione == true && attivazione == false)
                        {
                            var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

                            amf.ATTIVAZIONEMAGFAM = true;
                            amf.DATAATTIVAZIONEMAGFAM = DateTime.Now;

                            i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore nella fase di attivazione delle maggiorazioni familiari.");
                            }
                            else
                            {
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
                            }
                        }
                        else if (rinunciaMagFam == false && richiestaAttivazione == true && attivazione == false)
                        {
                            if (datiConiuge == true || datiFigli == true)
                            {
                                if (datiParzialiConiuge == false && datiParzialiFigli == false)
                                {
                                    if (datiConiuge == true && siDocConiuge == true || datiFigli == true && siDocFigli == true)
                                    {
                                        var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

                                        amf.ATTIVAZIONEMAGFAM = true;
                                        amf.DATAATTIVAZIONEMAGFAM = DateTime.Now;

                                        i = db.SaveChanges();

                                        if (i <= 0)
                                        {
                                            throw new Exception("Errore nella fase di attivazione delle maggiorazioni familiari.");
                                        }
                                        else
                                        {
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
                                        }
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

        public AltriDatiFamConiugeModel GetAdfValidiByIDConiuge(decimal idConiuge, decimal idStatoRecord)
        {
            bool modificati = false;
            List<ALTRIDATIFAM> adfcl = new List<ALTRIDATIFAM>();
            AltriDatiFamConiugeModel adfm = new AltriDatiFamConiugeModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var c = db.CONIUGE.Find(idConiuge);
                var mf = db.MAGGIORAZIONIFAMILIARI.Find(c.IDMAGGIORAZIONIFAMILIARI);

                if (idStatoRecord == (decimal)EnumStatoRecord.Attivato)
                {
                    //verifico se esiste un coniuge in lavorazione
                    bool trovato = false;
                    var lista_att = db.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false && a.IDMAGGIORAZIONIFAMILIARI == c.IDMAGGIORAZIONIFAMILIARI).OrderBy(a => a.IDATTIVAZIONEMAGFAM).ToList();
                    foreach (var at in lista_att)
                    {
                        var lc_appo = at.CONIUGE;
                        foreach (var c_appo in lc_appo)
                        {
                            if (c_appo.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione)
                            {
                                trovato = true;
                            }
                        }
                    }

                    //se non esiste prendo id attivazione degli adf modificati
                    if (trovato == false)
                    {
                        adfcl = mf.ATTIVAZIONIMAGFAM
                                .Where(a => a.ATTIVAZIONEMAGFAM == false).
                                OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).First()
                                .ALTRIDATIFAM.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.IDCONIUGE == idConiuge)
                                .OrderByDescending(a => a.IDALTRIDATIFAM).ToList();
                        if(adfcl?.Any()??false)
                        {
                            modificati = true;
                        }
                    }
                    else
                    {
                        adfcl = mf.ATTIVAZIONIMAGFAM
                                .Where(a => a.ATTIVAZIONEMAGFAM == true).
                                OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).First()
                                .ALTRIDATIFAM.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.IDCONIUGE == idConiuge)
                                .OrderByDescending(a => a.IDALTRIDATIFAM).ToList();
                    }

                }
                else
                {
                    adfcl = mf.ATTIVAZIONIMAGFAM
                            .Where(a => a.ATTIVAZIONEMAGFAM == false).
                            OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).First()
                            .ALTRIDATIFAM.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.IDCONIUGE == idConiuge)
                            .OrderByDescending(a => a.IDALTRIDATIFAM).ToList();
                    if (adfcl?.Any() ?? false)
                    {
                        modificati = true;
                    }

                }

                if (adfcl?.Any() ?? false)
                {
                    var adfc = adfcl.First();

                    adfm = new AltriDatiFamConiugeModel()
                    {
                        idAltriDatiFam = adfc.IDALTRIDATIFAM,
                        idConiuge = adfc.IDCONIUGE.Value,
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


    }
}
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
                                       out bool docFormulario, out bool inLavorazione)
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


            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                var conta_attivazioni = mf.ATTIVAZIONIMAGFAM.Where(a => (a.ANNULLATO == false || (a.RICHIESTAATTIVAZIONE == true && a.ATTIVAZIONEMAGFAM == true))).Count();

                if (conta_attivazioni > 1)
                {
                    //legge l'ultima attivazione valida
                    var last_amf = mf.ATTIVAZIONIMAGFAM.Where(a => (a.ANNULLATO == false || (a.RICHIESTAATTIVAZIONE == true && a.ATTIVAZIONEMAGFAM == true))).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).First();

                    if (last_amf!=null || last_amf.IDATTIVAZIONEMAGFAM>0)
                    {
                        //elenca le attivazioni aperte
                        var lamf = mf.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false && a.ATTIVAZIONEMAGFAM == false && a.RICHIESTAATTIVAZIONE == false).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);

                        //se ci sono esegue i controlli
                        if (lamf?.Any() ?? false)
                        {
                            foreach (var amf in lamf)
                            {
                                if (amf != null && amf.IDATTIVAZIONEMAGFAM > 0)
                                {
                                    var rmf =
                                        mf.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a => a.ANNULLATO == false)
                                            .OrderByDescending(a => a.IDRINUNCIAMAGFAM)
                                            .First();
    
                                    rinunciaMagFam = rmf.RINUNCIAMAGGIORAZIONI;
                                    richiestaAttivazione = amf.RICHIESTAATTIVAZIONE;
                                    Attivazione = amf.ATTIVAZIONEMAGFAM;
    
                                    var ld = amf.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari);
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
                                                var nadc = c.ALTRIDATIFAM.Count(a => a.ANNULLATO == false);
    
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
                                                var nadf = f.ALTRIDATIFAM.Count(a => a.ANNULLATO == false);
    
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
                            var rmf = mf.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a => a.ANNULLATO == false)
                                           .OrderByDescending(a => a.IDRINUNCIAMAGFAM)
                                           .First();
                            rinunciaMagFam = rmf.RINUNCIAMAGGIORAZIONI;
                            richiestaAttivazione = last_amf.RICHIESTAATTIVAZIONE;
                            Attivazione = last_amf.ATTIVAZIONEMAGFAM;

                            //comunque esegue i controlli sui dati dell'attivazione
                            var ld = last_amf.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari);
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
                                        var nadc = c.ALTRIDATIFAM.Count(a => a.ANNULLATO == false);

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
                                        var nadf = f.ALTRIDATIFAM.Count(a => a.ANNULLATO == false);

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
                    if ((((datiConiuge && siDocConiuge && datiParzialiConiuge == false) || (datiFigli && siDocFigli && datiParzialiFigli == false)) && docFormulario) && richiestaAttivazione==false)
                    {
                        inLavorazione = true;
                    }

                }
            }

        }

        public void ModificaConiuge(ConiugeModel cm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();
                try
                {
                    using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                    {
                        dtvmf.EditConiuge(cm, db);
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
                            out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione);

                        //leggo l'ultima attivazione valida
                        var last_attivazione_coniuge = this.GetAttivazioneById(cm.idConiuge, EnumTipoTabella.Coniuge, db);

                        //leggo se esiste una attivazione in lavorazione
                        var attivazione_aperta = this.GetAttivazioneAperta(cm.idMaggiorazioniFamiliari, db);
                        if (attivazione_aperta != null && attivazione_aperta.IDATTIVAZIONEMAGFAM > 0)
                        {
                            if (attivazione_aperta.IDATTIVAZIONEMAGFAM != last_attivazione_coniuge.IDATTIVAZIONEMAGFAM)
                            {
                                // crea nuovo coniuge e associa attivazione in lavorazione
                                ConiugeModel newc = new ConiugeModel()
                                {
                                    idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari,
                                    idTipologiaConiuge = cm.idTipologiaConiuge,
                                    idPassaporti = cm.idPassaporti,
                                    idTitoloViaggio = cm.idTitoloViaggio,
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
                                    Modificato = false
                                };

                                decimal new_idconiuge = this.SetConiuge(ref newc, db, attivazione_aperta.IDATTIVAZIONEMAGFAM);

                                //replico eventuali altri dati familiari e li associo
                                var adfc = c.ALTRIDATIFAM.First();
                                ALTRIDATIFAM adf_new = new ALTRIDATIFAM()
                                {
                                    IDCONIUGE = new_idconiuge,
                                    DATANASCITA = adfc.DATANASCITA,
                                    CAPNASCITA = adfc.CAPNASCITA,
                                    COMUNENASCITA = adfc.COMUNENASCITA,
                                    PROVINCIANASCITA = adfc.PROVINCIANASCITA,
                                    PROVINCIARESIDENZA = adfc.PROVINCIARESIDENZA,
                                    COMUNERESIDENZA = adfc.COMUNERESIDENZA,
                                    NAZIONALITA = adfc.NAZIONALITA,
                                    INDIRIZZORESIDENZA = adfc.INDIRIZZORESIDENZA,
                                    CAPRESIDENZA = adfc.CAPRESIDENZA,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    ANNULLATO = adfc.ANNULLATO
                                };
                                db.ALTRIDATIFAM.Add(adf_new);

                                c.MODIFICATO = true;

                                if (db.SaveChanges() > 0)
                                {
                                    this.AssociaAltriDatiFamiliariConiuge(new_idconiuge, adf_new.IDALTRIDATIFAM, db);
                                    
                                    ////riassocia eventuali documenti
                                    //var ldc = db.CONIUGE.Find(cm.idConiuge).DOCUMENTI.Where(x => x.MODIFICATO == false).ToList();
                                    //foreach (var dc in ldc)
                                    //{
                                    //    this.Associa_Doc_Coniuge_ById(dc.IDDOCUMENTO, new_idconiuge, db);
                                    //}
                                    ////riassocia eventuali pensioni
                                    //var lpc = db.CONIUGE.Find(cm.idConiuge).PENSIONE.Where(x => x.ANNULLATO == false).ToList();
                                    //foreach (var pc in lpc)
                                    //{
                                    //    this.Associa_Pensioni_Coniuge_ById(pc.IDPENSIONE, new_idconiuge, db);
                                    //}
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

                            c.MODIFICATO = true;

                            int idx = db.SaveChanges();

                            if (idx <= 0)
                            {
                                throw new Exception("Impossibile modificare il coniuge.");
                            }

                            ConiugeModel newc = new ConiugeModel()
                            {
                                idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari,
                                idTipologiaConiuge = cm.idTipologiaConiuge,
                                idPassaporti = cm.idPassaporti,
                                idTitoloViaggio = cm.idTitoloViaggio,
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
                                Modificato = false
                            };

                            decimal new_idconiuge = this.SetConiuge(ref newc, db, newmf.IDATTIVAZIONEMAGFAM);

                            //replico eventuali altri dati familiari e li associo
                            var adfc = c.ALTRIDATIFAM.First();
                            ALTRIDATIFAM adf_new = new ALTRIDATIFAM()
                            {
                                IDCONIUGE = new_idconiuge,
                                DATANASCITA = adfc.DATANASCITA,
                                CAPNASCITA = adfc.CAPNASCITA,
                                COMUNENASCITA = adfc.COMUNENASCITA,
                                PROVINCIANASCITA = adfc.PROVINCIANASCITA,
                                PROVINCIARESIDENZA = adfc.PROVINCIARESIDENZA,
                                COMUNERESIDENZA = adfc.COMUNERESIDENZA,
                                NAZIONALITA = adfc.NAZIONALITA,
                                INDIRIZZORESIDENZA = adfc.INDIRIZZORESIDENZA,
                                CAPRESIDENZA = adfc.CAPRESIDENZA,
                                DATAAGGIORNAMENTO = DateTime.Now,
                                ANNULLATO = adfc.ANNULLATO
                            };
                            db.ALTRIDATIFAM.Add(adf_new);

                            if (db.SaveChanges() > 0)
                            {
                                //riassocia altri eventuali coniugi
                                var lcon = db.ATTIVAZIONIMAGFAM.Find(last_attivazione_coniuge.IDATTIVAZIONEMAGFAM).CONIUGE.Where(x => x.MODIFICATO == false && x.IDCONIUGE != cm.idConiuge).ToList();
                                foreach (var con in lcon)
                                {
                                    this.AssociaConiugeAttivazione(newmf.IDATTIVAZIONEMAGFAM, con.IDCONIUGE, db);
                                }

                                ////riassocia formulari
                                //var ld = db.ATTIVAZIONIMAGFAM.Find(last_attivazione_coniuge.IDATTIVAZIONEMAGFAM).DOCUMENTI.Where(x => x.MODIFICATO == false && x.FK_IDDOCUMENTO == null && x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari).ToList();
                                //foreach (var d in ld)
                                //{
                                //    this.AssociaDocumentoAttivazione(newmf.IDATTIVAZIONEMAGFAM, d.IDDOCUMENTO, db);
                                //}

                                //this.AssociaAltriDatiFamiliariConiuge(new_idconiuge, adf_new.IDALTRIDATIFAM, db);

                                ////riassocia eventuali documenti
                                //var ldc = db.CONIUGE.Find(cm.idConiuge).DOCUMENTI.Where(x => x.MODIFICATO == false).ToList();
                                //foreach (var dc in ldc)
                                //{
                                //    this.Associa_Doc_Coniuge_ById(dc.IDDOCUMENTO, new_idconiuge, db);
                                //}
                                ////riassocia eventuali pensioni
                                //var lpc = db.CONIUGE.Find(cm.idConiuge).PENSIONE.Where(x => x.ANNULLATO == false).ToList();
                                //foreach (var pc in lpc)
                                //{
                                //    this.Associa_Pensioni_Coniuge_ById(pc.IDPENSIONE, new_idconiuge, db);
                                //}
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
                    MODIFICATO = false
                };

                db.CONIUGE.Add(c);

                //if (cm.FK_idConiuge>0)
                //{
                //    cm.Modificato = true;      
                //}

                if (db.SaveChanges() <= 0)
                {
                    throw new Exception("Non è stato possibile inserire il coniuge.");
                }
                else
                {
                    //var mf = db.MAGGIORAZIONIFAMILIARI.Find(cm.idMaggiorazioniFamiliari);
                    //var att = mf.ATTIVAZIONIMAGFAM.Where(a => (a.ANNULLATO == false || (a.RICHIESTAATTIVAZIONE == true && a.ATTIVAZIONEMAGFAM == true))).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).First();
                    //decimal idAttivazione = att.IDATTIVAZIONEMAGFAM;

                    //cm.idConiuge = c.IDCONIUGE;

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento del coniuge", "CONIUGE", db,
                        cm.idMaggiorazioniFamiliari, c.IDCONIUGE);

                    using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                    {
                        dtamf.AssociaConiugeAttivazione(idAttivazione, c.IDCONIUGE, db);
                    }

                    return c.IDCONIUGE;

                }

            }

            catch (Exception ex)
            {
                //db.Database.CurrentTransaction.Rollback();
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
                        //vmf.EditFiglio(fm, db);
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


        //public void EditFiglio(FigliModel fm, ModelDBISE db)
        //{
        //    try
        //    {
        //        var f = db.FIGLI.Find(fm.idFigli);

        //        DateTime dtIni = fm.dataInizio.Value;
        //        DateTime dtFin = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop();

        //        if (f != null && f.IDFIGLI > 0)
        //        {
        //            if (f.DATAINIZIOVALIDITA != fm.dataInizio.Value || f.DATAFINEVALIDITA != dtFin)
        //            //c.IDTIPOLOGIACONIUGE != (decimal)cm.idTipologiaConiuge || c.NOME != cm.nome || c.COGNOME != cm.cognome ||
        //            //c.CODICEFISCALE != cm.codiceFiscale || c.IDPASSAPORTI != cm.idPassaporti || c.IDTITOLOVIAGGIO != cm.idTitoloViaggio)
        //            {
        //                f.DATAAGGIORNAMENTO = DateTime.Now;
        //                f.ANNULLATO = true;

        //                int i = db.SaveChanges();

        //                if (i <= 0)
        //                {
        //                    throw new Exception("Impossibile modificare il figlio.");
        //                }
        //                else
        //                {
        //                    decimal idTrasferimento = db.MAGGIORAZIONIFAMILIARI.Find(f.IDMAGGIORAZIONIFAMILIARI).TRASFERIMENTO.IDTRASFERIMENTO;
        //                    Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Annulla la riga", "FIGLI", db, idTrasferimento, f.IDFIGLI);

        //                    FigliModel newf = new FigliModel()
        //                    {
        //                        idMaggiorazioniFamiliari = fm.idMaggiorazioniFamiliari,
        //                        idTipologiaFiglio = fm.idTipologiaFiglio,
        //                        idPassaporti = fm.idPassaporti,
        //                        idTitoloViaggio = fm.idTitoloViaggio,
        //                        nome = fm.nome,
        //                        cognome = fm.cognome,
        //                        codiceFiscale = fm.codiceFiscale,
        //                        dataInizio = fm.dataInizio.Value,
        //                        dataFine = dtFin,
        //                        escludiPassaporto = fm.escludiPassaporto,
        //                        dataNotificaPP = fm.dataNotificaPP,
        //                        escludiTitoloViaggio = fm.escludiTitoloViaggio,
        //                        dataNotificaTV = fm.dataNotificaTV,
        //                        dataAggiornamento = DateTime.Now
        //                    };

        //                    this.SetFiglio(ref newf, db);

        //                    //if (c.DATAINIZIOVALIDITA != cm.dataInizio.Value || c.DATAFINEVALIDITA != dtFin)
        //                    //{
        //                    using (dtPercentualeMagFigli dtpf = new dtPercentualeMagFigli())
        //                    {
        //                        PercentualeMagFigliModel lpmfm = dtpf.GetPercentualeMaggiorazioneFigli(newf.idFigli, DateTime.Now);

        //                        if (lpmfm != null && lpmfm.idPercMagFigli > 0)
        //                        {
        //                            dtpf.AssociaPercentualeMaggiorazioneFigli(newf.idFigli, lpmfm.idPercMagFigli, db);
        //                        }
        //                        else
        //                        {
        //                            throw new Exception("Non è presente nessuna percentuale del coniuge.");
        //                        }
        //                    }

        //                    //altri dati
        //                    using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
        //                    {
        //                        AltriDatiFamModel adfm = dtadf.GetAlttriDatiFamiliariFiglio(fm.idFigli);

        //                        if (adfm != null && adfm.idFigli > 0)
        //                        {
        //                            dtadf.AssociaAltriDatiFamiliariFiglio(newf.idFigli, adfm.idAltriDatiFam, db);
        //                        }
        //                        else
        //                        {
        //                            throw new Exception("Non sono presenti altri dati familiari del figlio.");
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (f.DATAINIZIOVALIDITA == fm.dataInizio.Value && f.DATAFINEVALIDITA == dtFin &&
        //                   (f.IDTIPOLOGIAFIGLIO != (decimal)fm.idTipologiaFiglio || f.NOME != fm.nome || f.COGNOME != fm.cognome ||
        //                    f.CODICEFISCALE != fm.codiceFiscale || f.IDPASSAPORTI != fm.idPassaporti || f.IDTITOLOVIAGGIO != fm.idTitoloViaggio))
        //                {
        //                    f.NOME = fm.nome.ToUpper();
        //                    f.COGNOME = fm.cognome.ToUpper();
        //                    f.CODICEFISCALE = fm.codiceFiscale.ToUpper();
        //                    f.DATAAGGIORNAMENTO = DateTime.Now;

        //                    int i = db.SaveChanges();

        //                    if (i <= 0)
        //                    {
        //                        throw new Exception("Impossibile modificare il figlio.");
        //                    }
        //                }
        //            }

        //            //db.Database.CurrentTransaction.Commit();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //db.Database.CurrentTransaction.Rollback();
        //        throw ex;
        //    }
        //}


        public void SetFiglio(ref FigliModel fm, ModelDBISE db)
        {
            try
            {
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

                };

                db.FIGLI.Add(f);

                if (db.SaveChanges() <= 0)
                {
                    throw new Exception("Non è stato possibile inserire il figlio.");
                }
                else
                {
                    decimal idTrasferimento = db.MAGGIORAZIONIFAMILIARI.Find(f.IDMAGGIORAZIONIFAMILIARI).TRASFERIMENTO.IDTRASFERIMENTO;
                    fm.idFigli = f.IDFIGLI;

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento del figlio", "FIGLIO", db,
                        idTrasferimento, f.IDFIGLI);

                    using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                    {
                        AttivazioniMagFamModel amfm = new AttivazioniMagFamModel();

                        //var lamfm = dtamf.GetUltimaAttivazioneMagFam()


                    }
                }

            }

            catch (Exception ex)
            {
                //db.Database.CurrentTransaction.Rollback();
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
            MemoryStream ms = new MemoryStream();
            DOCUMENTI d = new DOCUMENTI();

            dm.file.InputStream.CopyTo(ms);

            var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

            var lamf =
                mf.ATTIVAZIONIMAGFAM.Where(
                    a => a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == false && a.ATTIVAZIONEMAGFAM == false)
                    .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);
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
                this.PreSetAttivazioneVariazioneMaggiorazioniFamiliari(idMaggiorazioniFamiliari, db);

                // aggiunge il formulario
                mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                lamf = mf.ATTIVAZIONIMAGFAM.Where(
                        a => a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == false && a.ATTIVAZIONEMAGFAM == false)
                        .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);
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



                // associa il formulario all'attivazioneMagFam
                using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                {
                    AttivazioniMagFamModel amfm = new AttivazioniMagFamModel();

                    amfm = dtamf.GetAttivazioneMagFamDaLavorare(idMaggiorazioniFamiliari, db);

                    dtamf.AssociaFormulario(amfm.idAttivazioneMagFam, dm.idDocumenti, db);

                }

                //throw new Exception("Errore nella fase di inserimento del formulario maggiorazioni familiari.");
            }

            //else
            //{
            //    throw new Exception("Errore nella fase di inserimento del formulario maggiorazioni familiari.");
            //}


        }

        public AltriDatiFamConiugeModel GetAltriDatiFamiliariConiuge(decimal idConiuge, decimal idMaggiorazioniFamiliari)
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
                        var ladfc = c.ALTRIDATIFAM.Where(x => x.ANNULLATO == false).OrderByDescending(x => x.IDALTRIDATIFAM);


                        if (ladfc?.Any() ?? false)
                        {
                            var adfc = ladfc.First();

                            adfm = new AltriDatiFamConiugeModel()
                            {
                                idAltriDatiFam = adfc.IDALTRIDATIFAM,
                                idConiuge = adfc.IDCONIUGE,
                                nazionalita = adfc.NAZIONALITA,
                                indirizzoResidenza = adfc.INDIRIZZORESIDENZA,
                                capResidenza = adfc.CAPRESIDENZA,
                                comuneResidenza = adfc.COMUNERESIDENZA,
                                provinciaResidenza = adfc.PROVINCIARESIDENZA,
                                dataAggiornamento = adfc.DATAAGGIORNAMENTO,
                                annullato = adfc.ANNULLATO
                            };
                            ladfcm.Add(adfm);
                        }
                    }



                    //var lamf = this.GetListaAttivazioniConiugeByIdMagFam(idMaggiorazioniFamiliari).OrderByDescending(x =>x.idAttivazioneMagFam).ToList();
                    //foreach(var a in lamf)
                    //{
                    //    var adfc = db.ALTRIDATIFAM.Find(a.idConiuge).Where(x => x.ANNULLATO == false).ToList();
                    //}
                    //var c = db.CONIUGE.Find(idConiuge);
                    //var c_appo = c;
                    //var ladfc = c.ALTRIDATIFAM.OrderByDescending(x => x.IDALTRIDATIFAM).Where(x => x.ANNULLATO == false).ToList();
                    //while(ladfc==null || c_appo.FK_IDCONIUGE>0 )
                    //{
                    //    c = db.CONIUGE.Find(c_appo.FK_IDCONIUGE);
                    //    ladfc = c.ALTRIDATIFAM.OrderByDescending(x => x.IDALTRIDATIFAM).Where(x => x.ANNULLATO == false).ToList();
                    //    c_appo = c;
                    //}

                    //if (ladfc?.Any() ?? false)
                    //{
                    //    var adfc = ladfc.First();

                    //    adfm = new AltriDatiFamConiugeModel()
                    //    {
                    //        idAltriDatiFam = adfc.IDALTRIDATIFAM,
                    //        idConiuge = adfc.IDCONIUGE,
                    //        nazionalita = adfc.NAZIONALITA,
                    //        indirizzoResidenza = adfc.INDIRIZZORESIDENZA,
                    //        capResidenza = adfc.CAPRESIDENZA,
                    //        comuneResidenza = adfc.COMUNERESIDENZA,
                    //        provinciaResidenza = adfc.PROVINCIARESIDENZA,
                    //        dataAggiornamento = adfc.DATAAGGIORNAMENTO,
                    //        annullato = adfc.ANNULLATO
                    //    };
                    //    ladfcm.Add(adfm);

                    //}

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return adfm;
        }

        public void EditVariazioneAltriDatiFamiliariConiuge(AltriDatiFamConiugeModel adfm)
        {
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

                        var attmf_rif = this.GetAttivazioneById(adfm.idAltriDatiFam, EnumTipoTabella.AltriDatiFamiliari, db);

                        //decimal idMaggiorazioniFamiliari = 0;
                        //idMaggiorazioniFamiliari = adf.CONIUGE.MAGGIORAZIONIFAMILIARI.IDMAGGIORAZIONIFAMILIARI;
                        //var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                        //var lamf = mf.ATTIVAZIONIMAGFAM
                        //            .Where(e => (e.RICHIESTAATTIVAZIONE == true && e.ATTIVAZIONEMAGFAM == true) || e.ANNULLATO == false)
                        //            .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);

                        //var amf = lamf.First();
                        var attmf = this.GetAttivazioneAperta(attmf_rif.IDMAGGIORAZIONIFAMILIARI, db);

                        // se non esiste attivazione aperta la creo altrimenti la uso
                        if (attmf == null)
                        {
                            ATTIVAZIONIMAGFAM new_amf = new ATTIVAZIONIMAGFAM()
                            {
                                IDMAGGIORAZIONIFAMILIARI = attmf_rif.IDMAGGIORAZIONIFAMILIARI,
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
                            attmf_aperta = new_amf;

                        }
                        else
                        {
                            attmf_aperta = attmf;
                        }

                        if (attmf_aperta.IDATTIVAZIONEMAGFAM != attmf_rif.IDATTIVAZIONEMAGFAM)
                        {
                            adf.ANNULLATO = true;
                            if (db.SaveChanges() > 0)
                            {
                                decimal idTrasf = attmf_rif.IDMAGGIORAZIONIFAMILIARI;
                                //decimal idTrasf = 0;
                                //if (adf.IDCONIUGE != null && adf.IDCONIUGE > 0)
                                //{
                                //    idTrasf = adf.CONIUGE.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO;
                                //}

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
                                    ANNULLATO = adfm.annullato
                                };

                                db.ALTRIDATIFAM.Add(adfNew);

                                if (db.SaveChanges() > 0)
                                {
                                    this.AssociaAltriDatiFamiliariConiuge(adfNew.IDCONIUGE, adfNew.IDALTRIDATIFAM, db);

                                    this.AssociaAltriDatiFamiliariAttivazione(adfNew.IDALTRIDATIFAM, attmf_rif.IDATTIVAZIONEMAGFAM, db);

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
                                throw new Exception("La modifica per la riga relativa agli altri dati familiari non è avvenuta.");
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
                //item.Collection(a => a.DOCUMENTI).Load();
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


        public void AssociaAltriDatiFamiliariAttivazione(decimal idAltriDatiFamiliari, decimal idAttivazioneMagFam, ModelDBISE db)
        {
            try
            {
                var c = db.ALTRIDATIFAM.Find(idAltriDatiFamiliari);
                var item = db.Entry<ALTRIDATIFAM>(c);
                item.State = System.Data.Entity.EntityState.Modified;
                //item.Collection(a => a.DOCUMENTI).Load();
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
                                case EnumChiamante.VariazioneMaggiorazioniFamiliari:
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
                                //ld = db.CONIUGE.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc && ((a.FK_IDDOCUMENTO == null && a.MODIFICATO == false) || (a.FK_IDDOCUMENTO > 0 && a.MODIFICATO == false))).ToList();
                                ld = db.CONIUGE.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc && a.MODIFICATO == false).ToList();

                                bool modificabile;

                                foreach (var e in ld)
                                {
                                    attmf = db.DOCUMENTI.Find(e.IDDOCUMENTO).ATTIVAZIONIMAGFAM.First();
                                    //attmf = e.ATTIVAZIONIMAGFAM.First();

                                    //var num_atttivazioni = db.DOCUMENTI.Where(x => x.IDDOCUMENTO == e.IDDOCUMENTO).Count();
                                    //var num_atttivazioni = db.DOCUMENTI.Find(e.IDDOCUMENTO).CONIUGE.Count();
                                    modificabile = false;

                                    //if (num_atttivazioni == 1 && e.FK_IDDOCUMENTO == null)
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
                        //da rivedere
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

        public ATTIVAZIONIMAGFAM GetAttivazioneAperta(decimal IdMaggiorazioneFamiliare, ModelDBISE db)
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
                                    #region variabili
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
                                    #endregion
                                };
                                lcm.Add(cm);
                                break;
                            }
                            #region commento
                            //}
                            //else
                            //{
                            //    var con = lc.First();
                            //    VariazioneConiugeModel cm = new VariazioneConiugeModel()
                            //    {
                            //        modificabile = modificabile,
                            //        idConiuge = con.IDCONIUGE,
                            //        idMaggiorazioniFamiliari = con.IDMAGGIORAZIONIFAMILIARI,
                            //        idTipologiaConiuge = (EnumTipologiaConiuge)con.IDTIPOLOGIACONIUGE,
                            //        idPassaporti = con.IDPASSAPORTI,
                            //        idTitoloViaggio = con.IDTITOLOVIAGGIO,
                            //        nome = con.NOME,
                            //        cognome = con.COGNOME,
                            //        codiceFiscale = con.CODICEFISCALE,
                            //        dataInizio = con.DATAINIZIOVALIDITA,
                            //        dataFine = con.DATAFINEVALIDITA,
                            //        dataAggiornamento = con.DATAAGGIORNAMENTO,
                            //        escludiPassaporto = con.ESCLUDIPASSAPORTO,
                            //        dataNotificaPP = con.DATANOTIFICAPP,
                            //        escludiTitoloViaggio = con.ESCLUDITITOLOVIAGGIO,
                            //        dataNotificaTV = con.DATANOTIFICATV
                            //    };
                            //lcm.Add(cm);
                            //}
                            //}
                            #endregion
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

                lp = c.PENSIONE.Where(x => x.ANNULLATO == false).ToList();

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
                            annullato = p.ANNULLATO,
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


                //foreach(var e in lamf)
                //{
                switch (tipodoc)
                {
                    case EnumTipoDoc.Documento_Identita:
                        switch (parentela)
                        {
                            case EnumParentela.Coniuge:
                                var c = db.CONIUGE.Find(id);
                                ld = c.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc && a.MODIFICATO == false).OrderByDescending(a => a.IDDOCUMENTO).ToList();
                                //var lc = e.CONIUGE.ToList();

                                //foreach (var d in ldc)
                                //{
                                //var ldc = c.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc && a.MODIFICATO==false)
                                //                .OrderByDescending(a => a.IDDOCUMENTO).ToList();
                                //foreach(var dc in ldc)
                                //{
                                //ld.Add(d);
                                //}
                                //}
                                break;
                            case EnumParentela.Figlio:
                                //da modificare
                                var f = db.FIGLI.Find(id);
                                ld = f.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc && a.ATTIVAZIONIMAGFAM.Any(b => b.ANNULLATO == false))
                                                .OrderByDescending(a => a.IDDOCUMENTO).ToList();
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
                //}


                if (ld?.Any() ?? false)
                {
                    ldm.AddRange(from d in ld
                                 let f = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO)
                                 select new VariazioneDocumentiModel()
                                 {
                                     idDocumenti = d.IDDOCUMENTO,
                                     nomeDocumento = d.NOMEDOCUMENTO,
                                     estensione = d.ESTENSIONE,
                                     tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
                                     dataInserimento = d.DATAINSERIMENTO,
                                     file = f,
                                     Modificabile = (d.MODIFICATO == false && d.FK_IDDOCUMENTO == null) ? true : false
                                 });
                }


            }

            return ldm;
        }



        public void InserisciConiugeVarMagFam(ConiugeModel cm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {

                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var tm = dtt.GetTrasferimentoByIdAttMagFam(cm.idAttivazioneMagFam);

                        using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                        {
                            var p = dtpp.GetPassaportoInLavorazioneByIdTrasf(tm.idTrasferimento);
                            cm.idPassaporti = p.idPassaporto;
                        }

                        using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                        {
                            var tvm = dttv.GetTitoloViaggioInLavorazioneByIdTrasf(tm.idTrasferimento);
                            cm.idTitoloViaggio = tvm.idTitoloViaggio;
                        }

                    }


                    if (cm.idMaggiorazioniFamiliari == 0 && cm.idAttivazioneMagFam > 0)
                    {
                        var amf = db.ATTIVAZIONIMAGFAM.Find(cm.idAttivazioneMagFam);
                        cm.idMaggiorazioniFamiliari = amf.IDMAGGIORAZIONIFAMILIARI;
                    }

                    using (dtConiuge dtc = new dtConiuge())
                    {
                        cm.dataAggiornamento = DateTime.Now;

                        dtc.SetConiuge(ref cm, db);

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
                //item.Collection(a => a.DOCUMENTI).Load();
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

        public void Associa_Pensioni_Coniuge_ById(decimal idPensione, decimal idConiuge, ModelDBISE db)
        {
            try
            {
                var p = db.PENSIONE.Find(idPensione);
                var item = db.Entry<PENSIONE>(p);
                item.State = System.Data.Entity.EntityState.Modified;
                //item.Collection(a => a.DOCUMENTI).Load();
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

        public void EliminaConiuge(CONIUGE c, ModelDBISE db)
        {
            try
            {
                db.CONIUGE.Remove(c);

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile eliminare il coniuge."));
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

            int i = 0;

            

            this.SituazioneMagFamVariazione(idMaggiorazioneFamiliare, out rinunciaMagFam,
                out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario,out inLavorazione);

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





    }

}
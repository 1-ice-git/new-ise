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
                    var lamf = mf.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false && a.ATTIVAZIONEMAGFAM == false && a.RICHIESTAATTIVAZIONE == false).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);

                    if (lamf?.Any() ?? false)
                    {
                        //inLavorazione = true;

                        foreach (var amf in lamf)
                        {
                            //   var amf = lamf.First();

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
                                    inLavorazione = true;

                                }


                                if (mf.CONIUGE != null)
                                {
                                    var lc = mf.CONIUGE.ToList();
                                    if (lc?.Any() ?? false)
                                    {
                                        datiConiuge = true;
                                        inLavorazione = true;

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
                                        inLavorazione = true;

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
                        c.CODICEFISCALE != cm.codiceFiscale || c.IDPASSAPORTI != cm.idPassaporti || c.IDTITOLOVIAGGIO != cm.idTitoloViaggio)
                    {
                        c.DATAAGGIORNAMENTO = DateTime.Now;

                        this.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                            out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                            out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione);

                        //leggo l'ultima attivazione valida
                        var mf = db.MAGGIORAZIONIFAMILIARI.Find(cm.idMaggiorazioniFamiliari);
                        var last_attivazione = mf.ATTIVAZIONIMAGFAM.Where(a => (a.ANNULLATO == false || (a.RICHIESTAATTIVAZIONE == true && a.ATTIVAZIONEMAGFAM == true))).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).First();
                        var last_attivazione_coniuge = c.ATTIVAZIONIMAGFAM.Where(a => (a.ANNULLATO == false || (a.RICHIESTAATTIVAZIONE == true && a.ATTIVAZIONEMAGFAM == true))).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).First();
                        var attivazione_da_associare = last_attivazione;

                        if (last_attivazione.IDATTIVAZIONEMAGFAM != last_attivazione_coniuge.IDATTIVAZIONEMAGFAM
                            //&& last_attivazione_coniuge.RICHIESTAATTIVAZIONE 
                            //&& last_attivazione_coniuge.ATTIVAZIONEMAGFAM
                            )
                        {
                            attivazione_da_associare = last_attivazione_coniuge;
                        }

                        if (attivazione_da_associare.ATTIVAZIONEMAGFAM && attivazione_da_associare.RICHIESTAATTIVAZIONE && attivazione_da_associare.ANNULLATO==false)
                        {
                            //crea una nuova attivazione
                            ATTIVAZIONIMAGFAM newmf = new ATTIVAZIONIMAGFAM()
                            {
                                IDMAGGIORAZIONIFAMILIARI = cm.idMaggiorazioniFamiliari,
                                RICHIESTAATTIVAZIONE=false,
                                DATARICHIESTAATTIVAZIONE=null,
                                ATTIVAZIONEMAGFAM=false,
                                DATAATTIVAZIONEMAGFAM=null,
                                ANNULLATO=false,
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

                            attivazione_da_associare = newmf;
                       
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
                                FK_idConiuge=cm.idConiuge,
                                Modificato=false
                            };

                            //int i = db.SaveChanges();

                            //if (i <= 0)
                            //{
                            //    throw new Exception("Impossibile creare una nuova attivazione.");
                            //}

                            decimal new_idconiuge=this.SetConiuge(ref newc, db, attivazione_da_associare.IDATTIVAZIONEMAGFAM);

                            #region Riassocia eventuali figli

                            var lf = last_attivazione.FIGLI;

                            FIGLI figli = new FIGLI();

                            if (lf?.Any() ?? false)
                            {
                                foreach (var f in lf)
                                {
                                    figli = f;

                                    using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                                    {
                                        dtamf.AssociaFiglio(last_attivazione.IDATTIVAZIONEMAGFAM, figli.IDFIGLI, db);
                                    }
                                }

                            }
                            #endregion

                            #region Associa documenti
                            var ld = c.DOCUMENTI;

                            if (ld?.Any() ?? false)
                            {
                                using (dtDocumenti dtd = new dtDocumenti())
                                {
                                    foreach (var d in ld)
                                    {
                                        dtd.AssociaDocumentoConiuge(new_idconiuge, d.IDDOCUMENTO, db);
                                    }
                                }
                            }
                            #endregion


                            #region AltriDatiFamiliari

                            //var ladf =
                            //    c.ALTRIDATIFAM.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDALTRIDATIFAM);

                            //if (ladf?.Any() ?? false)
                            //{
                            //    var adf = ladf.First();
                            //    adf.DATAAGGIORNAMENTO = DateTime.Now;
                            //    adf.ANNULLATO = true;

                            //    int j = db.SaveChanges();
                            //    if (j > 0)
                            //    {
                            //        AltriDatiFamConiugeModel adfm = new AltriDatiFamConiugeModel()
                            //        {
                            //            idConiuge = newc.idConiuge,
                            //            nazionalita = adf.NAZIONALITA,
                            //            indirizzoResidenza = adf.INDIRIZZORESIDENZA,
                            //            capResidenza = adf.CAPRESIDENZA,
                            //            comuneResidenza = adf.COMUNERESIDENZA,
                            //            provinciaResidenza = adf.PROVINCIARESIDENZA,
                            //            dataAggiornamento = DateTime.Now,
                            //            annullato = false
                            //        };
                            //        using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                            //        {
                            //            dtadf.SetAltriDatiFamiliariConiuge(adfm, db);
                            //        }
                            //    }

                            //}
                            #endregion

                            #region Associa Percentuali maggiorazioni coniuge
                            //using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                            //{
                            //    List<PercentualeMagConiugeModel> lpmcm =
                            //        dtpc.GetListaPercentualiMagConiugeByRangeDate(cm.idTipologiaConiuge, dtIni, dtFin, db)
                            //            .ToList();

                            //    if (lpmcm?.Any() ?? false)
                            //    {
                            //        foreach (var pmcm in lpmcm)
                            //        {
                            //            dtpc.AssociaPercentualeMaggiorazioneConiuge(newc.idConiuge, pmcm.idPercentualeConiuge, db);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        throw new Exception("Non è presente nessuna percentuale del coniuge.");
                            //    }
                            //}
                            #endregion


                            #region Associa Pensioni
                            //using (dtPensione dtp = new dtPensione())
                            //{
                            //    List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();

                            //    lpcm = dtp.GetPensioniByIdConiuge(cm.idConiuge, db).OrderBy(a => a.dataInizioValidita).ToList();

                            //    if (lpcm?.Any() ?? false)
                            //    {
                            //        var pcmFirst = lpcm.First();
                            //        var pcmLast = lpcm.Last();

                            //        if (pcmFirst.dataInizioValidita < cm.dataInizio.Value)
                            //        {
                            //            pcmFirst.dataInizioValidita = cm.dataInizio.Value;
                            //            dtp.SetNuovoImportoPensione(pcmFirst, newc.idConiuge, db);
                            //        }

                            //        if (pcmLast.dataFineValidita > cm.dataFine)
                            //        {
                            //            pcmLast.dataFineValidita = cm.dataFine;
                            //            dtp.SetNuovoImportoPensione(pcmLast, newc.idConiuge, db);
                            //        }

                            //        foreach (var pcm in lpcm)
                            //        {

                            //            if (pcm.idPensioneConiuge != pcmFirst.idPensioneConiuge && pcm.idPensioneConiuge != pcmLast.idPensioneConiuge)
                            //            {
                            //                dtp.SetNuovoImportoPensione(pcm, newc.idConiuge, db);
                            //            }


                            //        }


                            //    }

                            //}
                            #endregion

                        }
                        else
                        {

                            c.DATAINIZIOVALIDITA = cm.dataInizio.Value;
                            c.DATAFINEVALIDITA = dtFin;
                            c.IDTIPOLOGIACONIUGE = (decimal)cm.idTipologiaConiuge;
                            c.NOME = cm.nome;
                            c.COGNOME = cm.cognome;
                            c.CODICEFISCALE = cm.codiceFiscale;
                            c.IDPASSAPORTI = cm.idPassaporti;
                            c.IDTITOLOVIAGGIO = cm.idTitoloViaggio;
                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Impossibile modificare il coniuge.");
                            }
                        }
                    }
                    //db.Database.CurrentTransaction.Commit();
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
                    IDPASSAPORTI = cm.idPassaporti,
                    IDTITOLOVIAGGIO = cm.idTitoloViaggio,
                    NOME = cm.nome.ToUpper(),
                    COGNOME = cm.cognome.ToUpper(),
                    CODICEFISCALE = cm.codiceFiscale.ToUpper(),
                    DATAINIZIOVALIDITA = cm.dataInizio.Value,
                    DATAFINEVALIDITA = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop(),
                    DATAAGGIORNAMENTO = cm.dataAggiornamento,
                    ESCLUDIPASSAPORTO = cm.escludiPassaporto,
                    DATANOTIFICAPP = cm.dataNotificaPP,
                    ESCLUDITITOLOVIAGGIO = cm.escludiTitoloViaggio,
                    DATANOTIFICATV = cm.dataNotificaTV,
                    FK_IDCONIUGE = cm.FK_idConiuge,
                    MODIFICATO = false
                };

                db.CONIUGE.Add(c);

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
                        dtamf.AssociaConiugeAttivazione(idAttivazione, c.IDCONIUGE,db);
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
                    IDPASSAPORTI = fm.idPassaporti,
                    IDTITOLOVIAGGIO = fm.idTitoloViaggio,
                    NOME = fm.nome.ToUpper(),
                    COGNOME = fm.cognome.ToUpper(),
                    CODICEFISCALE = fm.codiceFiscale.ToUpper(),
                    DATAINIZIOVALIDITA = fm.dataInizio.Value,
                    DATAFINEVALIDITA = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop(),
                    DATAAGGIORNAMENTO = fm.dataAggiornamento,
                    ESCLUDIPASSAPORTO = fm.escludiPassaporto,
                    DATANOTIFICAPP = fm.dataNotificaPP,
                    ESCLUDITITOLOVIAGGIO = fm.escludiTitoloViaggio,
                    DATANOTIFICATV = fm.dataNotificaTV
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

                    var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                    var lamf = mf.ATTIVAZIONIMAGFAM
                                .Where(e => (e.RICHIESTAATTIVAZIONE == true && e.ATTIVAZIONEMAGFAM == true) || e.ANNULLATO == false)
                                .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);

                   // var lamf = db.ATTIVAZIONIMAGFAM.Where(a => a.IDMAGGIORAZIONIFAMILIARI == idMaggiorazioniFamiliari).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);

                    
                    foreach (var e in lamf)
                    {
                        if(e.ANNULLATO==false || (e.ATTIVAZIONEMAGFAM && e.RICHIESTAATTIVAZIONE))
                        {
                            var n_adf = e.ALTRIDATIFAM.Count();

                            if (n_adf>0)
                            {
                                var adf = e.ALTRIDATIFAM.First();

                                if (adf.IDALTRIDATIFAM>0 && adf != null)
                                {
                                    if(adf.IDCONIUGE>0 && adf.IDFIGLI==null)
                                    {
                                        adfm = new AltriDatiFamConiugeModel()
                                        {
                                            idAltriDatiFam = adf.IDALTRIDATIFAM,
                                            idConiuge = adf.IDCONIUGE,
                                            nazionalita = adf.NAZIONALITA,
                                            indirizzoResidenza = adf.INDIRIZZORESIDENZA,
                                            capResidenza = adf.CAPRESIDENZA,
                                            comuneResidenza = adf.COMUNERESIDENZA,
                                            provinciaResidenza = adf.PROVINCIARESIDENZA,
                                            dataAggiornamento = adf.DATAAGGIORNAMENTO,
                                            annullato = adf.ANNULLATO
                                        };
                                        ladfcm.Add(adfm);
                                    }
                                }
                            }
                        }
                    }
                    return adfm;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

                    //var amf = db.ATTIVAZIONIMAGFAM.Find(c.IDMAGGIORAZIONIFAMILIARI);

                    var lamf = c.ATTIVAZIONIMAGFAM.Where(
                      a => a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == false && a.ATTIVAZIONEMAGFAM == false)
                      .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);

                    if (lamf?.Any() ?? false)
                    {
                        var amf = lamf.First();

                        if (amf != null && amf.IDATTIVAZIONEMAGFAM>0)
                        {
                            var adf = db.ALTRIDATIFAM.Find(adfm.idAltriDatiFam);
    
                            if (adf != null && adfm.idAltriDatiFam > 0)
                            {
                                if (amf.ATTIVAZIONEMAGFAM && amf.RICHIESTAATTIVAZIONE && amf.ANNULLATO==false)
                                {
                                    adf.ANNULLATO = true;
                                    if (db.SaveChanges() > 0)
                                    {
                                        decimal idTrasf = 0;
    
                                        if (adf.IDCONIUGE != null && adf.IDCONIUGE > 0)
                                        {
                                            idTrasf = adf.CONIUGE.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO;
                                        }
    
                                        Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione logica altri dati familiari.", "ALTRIDATIFAM", db, idTrasf, adf.IDALTRIDATIFAM);
    
                                        var adfNew = new ALTRIDATIFAM
                                        {
                                            IDCONIUGE = adfm.idConiuge,
                                            DATANASCITA =  DateTime.MinValue,
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
                                            this.AssociaAltriDatiFamiliariConiuge(adfNew.IDCONIUGE, adfNew.IDALTRIDATIFAM, db );
    
                                            this.AssociaAltriDatiFamiliariAttivazione(adfNew.IDALTRIDATIFAM, amf.IDATTIVAZIONEMAGFAM, db);
    
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
                                    adf.ANNULLATO = adfm.annullato;
                                }
                            }
                            else
                            {
                                throw new Exception("L'oggetto altri dati familiari passato non è valorizzato.");
                            }
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


    }

}
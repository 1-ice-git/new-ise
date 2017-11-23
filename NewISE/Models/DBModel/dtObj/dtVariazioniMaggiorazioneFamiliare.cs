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
            docFormulario = false;


            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                var lamf = mf.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);

                if (lamf?.Any() ?? false)
                {
                    var amf = lamf.First();


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


        //public void EditConiuge(ConiugeModel cm, ModelDBISE db)
        //{
        //    try
        //    {
        //        var c = db.CONIUGE.Find(cm.idConiuge);

        //        DateTime dtIni = cm.dataInizio.Value;
        //        DateTime dtFin = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop();

        //        if (c != null && c.IDCONIUGE > 0)
        //        {
        //            if (c.DATAINIZIOVALIDITA != cm.dataInizio.Value || c.DATAFINEVALIDITA != dtFin)
        //            //c.IDTIPOLOGIACONIUGE != (decimal)cm.idTipologiaConiuge || c.NOME != cm.nome || c.COGNOME != cm.cognome ||
        //            //c.CODICEFISCALE != cm.codiceFiscale || c.IDPASSAPORTI != cm.idPassaporti || c.IDTITOLOVIAGGIO != cm.idTitoloViaggio)
        //            {
        //                c.DATAAGGIORNAMENTO = DateTime.Now;
        //                c.ANNULLATO = true;

        //                int i = db.SaveChanges();

        //                if (i <= 0)
        //                {
        //                    throw new Exception("Impossibile modificare il coniuge.");
        //                }
        //                else
        //                {
        //                    decimal idTrasferimento = db.MAGGIORAZIONIFAMILIARI.Find(c.IDMAGGIORAZIONIFAMILIARI).TRASFERIMENTO.IDTRASFERIMENTO;
        //                    Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Annulla la riga", "CONIUGE", db, idTrasferimento, c.IDCONIUGE);

        //                    ConiugeModel newc = new ConiugeModel()
        //                    {
        //                        idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari,
        //                        idTipologiaConiuge = cm.idTipologiaConiuge,
        //                        idPassaporti = cm.idPassaporti,
        //                        idTitoloViaggio = cm.idTitoloViaggio,
        //                        nome = cm.nome,
        //                        cognome = cm.cognome,
        //                        codiceFiscale = cm.codiceFiscale,
        //                        dataInizio = cm.dataInizio.Value,
        //                        dataFine = dtFin,
        //                        escludiPassaporto = cm.escludiPassaporto,
        //                        dataNotificaPP = cm.dataNotificaPP,
        //                        escludiTitoloViaggio = cm.escludiTitoloViaggio,
        //                        dataNotificaTV = cm.dataNotificaTV,
        //                        dataAggiornamento = DateTime.Now
        //                    };

        //                    this.SetConiuge(ref newc, db);

        //                    //if (c.DATAINIZIOVALIDITA != cm.dataInizio.Value || c.DATAFINEVALIDITA != dtFin)
        //                    //{
        //                    using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
        //                    {
        //                        List<PercentualeMagConiugeModel> lpmcm =
        //                            dtpc.GetListaPercentualiMagConiugeByRangeDate(newc.idTipologiaConiuge, dtIni, dtFin, db)
        //                                .ToList();

        //                        if (lpmcm?.Any() ?? false)
        //                        {
        //                            foreach (var pmcm in lpmcm)
        //                            {
        //                                dtpc.AssociaPercentualeMaggiorazioneConiuge(newc.idConiuge, pmcm.idPercentualeConiuge, db);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            throw new Exception("Non è presente nessuna percentuale del coniuge.");
        //                        }
        //                    }

        //                    using (dtPensione dtp = new dtPensione())
        //                    {
        //                        List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();

        //                        lpcm = dtp.GetPensioniByIdConiuge(cm.idConiuge, db).OrderBy(a => a.dataInizioValidita).ToList();

        //                        if (lpcm?.Any() ?? false)
        //                        {
        //                            var pcmFirst = lpcm.First();
        //                            var pcmLast = lpcm.Last();

        //                            if (pcmFirst.dataInizioValidita < newc.dataInizio.Value)
        //                            {
        //                                pcmFirst.dataInizioValidita = newc.dataInizio.Value;
        //                                dtp.SetNuovoImportoPensione(pcmFirst, newc.idConiuge, db);
        //                            }

        //                            if (pcmLast.dataFineValidita > newc.dataFine)
        //                            {
        //                                pcmLast.dataFineValidita = newc.dataFine;
        //                                dtp.SetNuovoImportoPensione(pcmLast, newc.idConiuge, db);
        //                            }

        //                            foreach (var pcm in lpcm)
        //                            {

        //                                if (pcm.idPensioneConiuge != pcmFirst.idPensioneConiuge && pcm.idPensioneConiuge != pcmLast.idPensioneConiuge)
        //                                {
        //                                    dtp.SetNuovoImportoPensione(pcm, newc.idConiuge, db);
        //                                }
        //                            }
        //                        }
        //                    }

        //                    //associa altri dati coniuge duplicando il record sulla tabella AltriDatiConiuge
        //                    using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
        //                    {
        //                        AltriDatiFamModel adfm = dtadf.GetAlttriDatiFamiliariConiuge(cm.idConiuge);

        //                        if (adfm != null && adfm.idConiuge > 0)
        //                        {
        //                            dtadf.AssociaAltriDatiFamiliariConiuge(newc.idConiuge, adfm.idAltriDatiFam, db);
        //                        }
        //                        else
        //                        {
        //                            throw new Exception("Non sono presenti altri dati familiari del coniuge.");
        //                        }
        //                    }

        //                    //associa documenti coniuge duplicando il record sulla tabella Documenti creando l'associazione su Coniuge_Doc
        //                    using (dtDocumenti dtd = new dtDocumenti())
        //                    {
        //                        DocumentiModel dm = dtd.GetDatiDocumentoByIdConiuge(cm.idConiuge);

        //                        if (dm != null && dm.idDocumenti > 0)
        //                        {
        //                            dtd.AssociaDocumentiConiuge(newc.idConiuge, dm.idDocumenti, db);
        //                        }
        //                        else
        //                        {
        //                            throw new Exception("Non sono presenti documenti del coniuge.");
        //                        }
        //                    }


        //                }
        //            }
        //            else
        //            {
        //                if (c.DATAINIZIOVALIDITA == cm.dataInizio.Value && c.DATAFINEVALIDITA == dtFin &&
        //                   (c.IDTIPOLOGIACONIUGE != (decimal)cm.idTipologiaConiuge || c.NOME != cm.nome || c.COGNOME != cm.cognome ||
        //                    c.CODICEFISCALE != cm.codiceFiscale || c.IDPASSAPORTI != cm.idPassaporti || c.IDTITOLOVIAGGIO != cm.idTitoloViaggio))
        //                {
        //                    c.NOME = cm.nome.ToUpper();
        //                    c.COGNOME = cm.cognome.ToUpper();
        //                    c.CODICEFISCALE = cm.codiceFiscale.ToUpper();
        //                    c.DATAAGGIORNAMENTO = DateTime.Now;

        //                    int i = db.SaveChanges();

        //                    if (i <= 0)
        //                    {
        //                        throw new Exception("Impossibile modificare il coniuge.");
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

        public void EditConiuge(ConiugeModel cm, ModelDBISE db)
        {
            try
            {
                var c = db.CONIUGE.Find(cm.idConiuge);

                DateTime dtIni = cm.dataInizio.Value;
                DateTime dtFin = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop();

                if (c != null && c.IDCONIUGE > 0)
                {
                    if (c.DATAINIZIOVALIDITA != cm.dataInizio.Value || c.DATAFINEVALIDITA != dtFin ||
                        c.IDTIPOLOGIACONIUGE != (decimal)cm.idTipologiaConiuge || c.NOME != cm.nome || c.COGNOME != cm.cognome ||
                        c.CODICEFISCALE != cm.codiceFiscale || c.IDPASSAPORTI != cm.idPassaporti || c.IDTITOLOVIAGGIO != cm.idTitoloViaggio)
                    {
                        c.DATAAGGIORNAMENTO = DateTime.Now;

                        int i = db.SaveChanges();

                        if (i <= 0)
                        {
                            throw new Exception("Impossibile modificare il coniuge.");
                        }
                        else
                        {


                            decimal idTrasferimento = db.MAGGIORAZIONIFAMILIARI.Find(c.IDMAGGIORAZIONIFAMILIARI).TRASFERIMENTO.IDTRASFERIMENTO;
                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Annulla la riga", "CONIUGE", db, idTrasferimento, c.IDCONIUGE);




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
                                dataNotificaTV = cm.dataNotificaTV
                            };

                            this.SetConiuge(ref newc, db);

                            #region AltriDatiFamiliari

                            var ladf =
                                c.ALTRIDATIFAM.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDALTRIDATIFAM);
                            if (ladf?.Any() ?? false)
                            {
                                var adf = ladf.First();
                                adf.DATAAGGIORNAMENTO = DateTime.Now;
                                adf.ANNULLATO = true;

                                int j = db.SaveChanges();
                                if (j > 0)
                                {
                                    AltriDatiFamConiugeModel adfm = new AltriDatiFamConiugeModel()
                                    {
                                        idConiuge = newc.idConiuge,
                                        nazionalita = adf.NAZIONALITA,
                                        indirizzoResidenza = adf.INDIRIZZORESIDENZA,
                                        capResidenza = adf.CAPRESIDENZA,
                                        comuneResidenza = adf.COMUNERESIDENZA,
                                        provinciaResidenza = adf.PROVINCIARESIDENZA,
                                        dataAggiornamento = DateTime.Now,
                                        annullato = false
                                    };
                                    using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                                    {
                                        dtadf.SetAltriDatiFamiliariConiuge(adfm, db);
                                    }
                                }

                            }
                            #endregion

                            #region Associa Percentuali maggiorazioni coniuge
                            using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                            {
                                List<PercentualeMagConiugeModel> lpmcm =
                                    dtpc.GetListaPercentualiMagConiugeByRangeDate(cm.idTipologiaConiuge, dtIni, dtFin, db)
                                        .ToList();

                                if (lpmcm?.Any() ?? false)
                                {
                                    foreach (var pmcm in lpmcm)
                                    {
                                        dtpc.AssociaPercentualeMaggiorazioneConiuge(newc.idConiuge, pmcm.idPercentualeConiuge, db);
                                    }
                                }
                                else
                                {
                                    throw new Exception("Non è presente nessuna percentuale del coniuge.");
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
                                        dtd.AssociaDocumentoConiuge(newc.idConiuge, d.IDDOCUMENTO, db);
                                    }
                                }

                            }

                            #endregion

                            #region Associa Pensioni
                            using (dtPensione dtp = new dtPensione())
                            {
                                List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();

                                lpcm = dtp.GetPensioniByIdConiuge(cm.idConiuge, db).OrderBy(a => a.dataInizioValidita).ToList();

                                if (lpcm?.Any() ?? false)
                                {
                                    var pcmFirst = lpcm.First();
                                    var pcmLast = lpcm.Last();

                                    if (pcmFirst.dataInizioValidita < cm.dataInizio.Value)
                                    {
                                        pcmFirst.dataInizioValidita = cm.dataInizio.Value;
                                        dtp.SetNuovoImportoPensione(pcmFirst, newc.idConiuge, db);
                                    }

                                    if (pcmLast.dataFineValidita > cm.dataFine)
                                    {
                                        pcmLast.dataFineValidita = cm.dataFine;
                                        dtp.SetNuovoImportoPensione(pcmLast, newc.idConiuge, db);
                                    }

                                    foreach (var pcm in lpcm)
                                    {

                                        if (pcm.idPensioneConiuge != pcmFirst.idPensioneConiuge && pcm.idPensioneConiuge != pcmLast.idPensioneConiuge)
                                        {
                                            dtp.SetNuovoImportoPensione(pcm, newc.idConiuge, db);
                                        }


                                    }


                                }

                            }
                            #endregion


                        }
                    }
                }

                //db.Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                //db.Database.CurrentTransaction.Rollback();
                throw ex;
            }
        }



        public void SetConiuge(ref ConiugeModel cm, ModelDBISE db)
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
                    DATANOTIFICATV = cm.dataNotificaTV
                };

                db.CONIUGE.Add(c);

                if (db.SaveChanges() <= 0)
                {
                    throw new Exception("Non è stato possibile inserire il coniuge.");
                }
                else
                {
                    decimal idTrasferimento = db.MAGGIORAZIONIFAMILIARI.Find(c.IDMAGGIORAZIONIFAMILIARI).TRASFERIMENTO.IDTRASFERIMENTO;
                    cm.idConiuge = c.IDCONIUGE;

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento del coniuge", "CONIUGE", db,
                        idTrasferimento, c.IDCONIUGE);

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



    }

}
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

                        //verifica se ultima attivazione valida coniuge e' notificata
                        if (last_attivazione_coniuge.ATTIVAZIONEMAGFAM && last_attivazione_coniuge.RICHIESTAATTIVAZIONE && last_attivazione_coniuge.ANNULLATO == false)
                        {
                            //verifica se l'ultima attivazione non e' notificata
                            if (last_attivazione.ATTIVAZIONEMAGFAM && last_attivazione.RICHIESTAATTIVAZIONE && last_attivazione.ANNULLATO == false)
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

                            }
                            else
                            {
                                // altrimenti crea nuovo coniuge e associa attivazione in lavorazione
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

                                decimal new_idconiuge = this.SetConiuge(ref newc, db, last_attivazione.IDATTIVAZIONEMAGFAM);
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
                            c.IDPASSAPORTI = cm.idPassaporti;
                            c.IDTITOLOVIAGGIO = cm.idTitoloViaggio;
                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Impossibile modificare il coniuge.");
                            }

                            //db.Database.CurrentTransaction.Commit();
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
                dataVariazione = DateTime.Now,
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
                        if (e.ANNULLATO == false || (e.ATTIVAZIONEMAGFAM && e.RICHIESTAATTIVAZIONE))
                        {
                            var n_adf = e.ALTRIDATIFAM.Count();

                            if (n_adf > 0)
                            {
                                var adf = e.ALTRIDATIFAM.OrderByDescending(a => a.IDALTRIDATIFAM).First();

                                if (adf.IDALTRIDATIFAM > 0 && adf != null)
                                {
                                    if (adf.IDCONIUGE > 0 && adf.IDFIGLI == null)
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

                    //var lamf = c.ATTIVAZIONIMAGFAM.ToList();

                    //if (lamf?.Any() ?? false)
                    //{
                    //    var amf = lamf.First();

                    //    if (amf != null && amf.IDATTIVAZIONEMAGFAM>0)
                    //    {
                    var adf = db.ALTRIDATIFAM.Find(adfm.idAltriDatiFam);

                    if (adf != null && adf.IDALTRIDATIFAM > 0)
                    {


                        decimal idMaggiorazioniFamiliari = 0;
                        idMaggiorazioniFamiliari = adf.CONIUGE.MAGGIORAZIONIFAMILIARI.IDMAGGIORAZIONIFAMILIARI;
                        var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                        var lamf = mf.ATTIVAZIONIMAGFAM
                                    .Where(e => (e.RICHIESTAATTIVAZIONE == true && e.ATTIVAZIONEMAGFAM == true) || e.ANNULLATO == false)
                                    .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);

                        var amf = lamf.First();

                        if (amf.ATTIVAZIONEMAGFAM && amf.RICHIESTAATTIVAZIONE && amf.ANNULLATO == false)
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
                            if (db.SaveChanges() < 0)
                            {
                                throw new Exception("L'aggiornamento della riga relativa agli altri dati familiari non è avvenuta.");
                            }
                            db.Database.CurrentTransaction.Commit();
                        }
                    }
                }
                //else
                //{
                //    throw new Exception("L'oggetto altri dati familiari passato non è valorizzato.");
                //}
                //}

                //    }
                //}
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
                        case EnumTipoDoc.Carta_Imbarco:
                        case EnumTipoDoc.Titolo_Viaggio:
                        case EnumTipoDoc.Formulario_Titoli_Viaggio:
                            t = d.TITOLIVIAGGIO.OrderByDescending(a => a.IDTITOLOVIAGGIO).First().TRASFERIMENTO;
                            break;
                        case EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione:
                        case EnumTipoDoc.Dichiarazione_Costo_Locazione:
                        case EnumTipoDoc.Attestazione_Spese_Abitazione:
                        case EnumTipoDoc.Clausole_Contratto_Alloggio:
                        case EnumTipoDoc.Copia_Contratto_Locazione:
                            t = d.MAGGIORAZIONEABITAZIONE.OrderByDescending(a => a.IDMAB).First().TRASFERIMENTO;
                            break;
                        case EnumTipoDoc.Contributo_Fisso_Omnicomprensivo:
                            t = d.TRASPORTOEFFETTI.OrderByDescending(a => a.IDTRASPORTOEFFETTI).First().TRASFERIMENTO;
                            break;
                        case EnumTipoDoc.Attestazione_Trasloco:
                            break;
                        case EnumTipoDoc.Lettera_Trasferimento:
                            t = d.TRASFERIMENTO.OrderByDescending(a => a.IDTRASFERIMENTO).First();
                            break;
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
                        //legge id attivazione corrente
                        //var la = d.ATTIVAZIONIMAGFAM.OrderByDescending(x => x.IDATTIVAZIONEMAGFAM);
                        //var trasf = d.CONIUGE.OrderByDescending(x => x.IDCONIUGE).First().MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;


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

                            //AssociaDocumentiAttivazione(cm,)
                            //    AssociaConiugeAttivazione()
                            //    AssociaDocumentiConiuge

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
                        //db.DOCUMENTI.Remove(d);

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

        public IList<DocumentiModel> GetDocumentiById(decimal id, EnumTipoDoc tipodoc, EnumParentela parentela = EnumParentela.Richiedente)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                List<DOCUMENTI> ld = new List<DOCUMENTI>();

                switch (tipodoc)
                {
                    case EnumTipoDoc.Documento_Identita:

                        switch (parentela)
                        {
                            case EnumParentela.Coniuge:
                                ld = db.CONIUGE.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();
                                break;
                            case EnumParentela.Figlio:
                                ld = db.FIGLI.Find(id).DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc).ToList();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("parentela");
                        }
                        break;
                    case EnumTipoDoc.Formulario_Maggiorazioni_Familiari:

                        ld = db.ATTIVAZIONIMAGFAM.Find(id)
                            .DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipodoc)
                            .OrderByDescending(a => a.DATAINSERIMENTO).ToList();

                        break;
                    default:
                        throw new ArgumentOutOfRangeException("tipodoc");
                }


                if (ld?.Any() ?? false)
                {
                    ldm.AddRange(from d in ld
                                 let f = (HttpPostedFileBase)new MemoryPostedFile(d.FILEDOCUMENTO)
                                 select new DocumentiModel()
                                 {
                                     idDocumenti = d.IDDOCUMENTO,
                                     nomeDocumento = d.NOMEDOCUMENTO,
                                     estensione = d.ESTENSIONE,
                                     tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
                                     dataInserimento = d.DATAINSERIMENTO,
                                     file = f
                                 });
                }


            }

            return ldm;
        }


        public void AssociaDocumentoConiuge(ref DocumentiModel dm, decimal idConiuge, ModelDBISE db)
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

                c.DOCUMENTI.Add(d);

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (" + dm.tipoDocumento.ToString() + ").", "Documenti", db, t.IDTRASFERIMENTO, dm.idDocumenti);
                }
            }


        }



    }

}
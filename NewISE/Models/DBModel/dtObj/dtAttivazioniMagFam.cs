﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.Tools;
using Newtonsoft.Json.Schema;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtAttivazioniMagFam : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public AttivazioniMagFamModel GetAttivazioneMagFamByIdConiuge(decimal idConiuge)
        {
            AttivazioniMagFamModel amfm = new AttivazioniMagFamModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var amf =
                    db.CONIUGE.Find(idConiuge)
                        .ATTIVAZIONIMAGFAM.OrderByDescending(a => a.IDATTIVAZIONEMAGFAM)
                        .First(a => a.ANNULLATO == false);

                amfm = new AttivazioniMagFamModel()
                {
                    idAttivazioneMagFam = amf.IDATTIVAZIONEMAGFAM,
                    idMaggiorazioniFamiliari = amf.IDMAGGIORAZIONIFAMILIARI,
                    richiestaAttivazione = amf.RICHIESTAATTIVAZIONE,
                    dataRichiestaAttivazione = amf.DATARICHIESTAATTIVAZIONE,
                    attivazioneMagFam = amf.ATTIVAZIONEMAGFAM,
                    dataAttivazioneMagFam = amf.DATAATTIVAZIONEMAGFAM,
                    dataVariazione = amf.DATAVARIAZIONE,
                    dataAggiornamento = amf.DATAAGGIORNAMENTO,
                    annullato = amf.ANNULLATO
                };

            }

            return amfm;

        }

        public AttivazioniMagFamModel GetAttivazioneMagFamByID(decimal idAttivazioneMagFam)
        {
            AttivazioniMagFamModel amfm = new AttivazioniMagFamModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

                amfm = new AttivazioniMagFamModel()
                {
                    idAttivazioneMagFam = amf.IDATTIVAZIONEMAGFAM,
                    idMaggiorazioniFamiliari = amf.IDMAGGIORAZIONIFAMILIARI,
                    richiestaAttivazione = amf.RICHIESTAATTIVAZIONE,
                    dataRichiestaAttivazione = amf.DATARICHIESTAATTIVAZIONE,
                    attivazioneMagFam = amf.ATTIVAZIONEMAGFAM,
                    dataAttivazioneMagFam = amf.DATAATTIVAZIONEMAGFAM,
                    dataVariazione = amf.DATAVARIAZIONE,
                    dataAggiornamento = amf.DATAAGGIORNAMENTO,
                    annullato = amf.ANNULLATO
                };

            }

            return amfm;
        }

        public AttivazioniMagFamModel GetAttivazioneMagFamIniziale(decimal idMaggiorazioneFamiliare)
        {
            AttivazioniMagFamModel amfm = new AttivazioniMagFamModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioneFamiliare);

                if (mf?.IDMAGGIORAZIONIFAMILIARI > 0)
                {
                    var lamf =
                        mf.ATTIVAZIONIMAGFAM.Where(
                            a =>
                                (
                                    (a.RICHIESTAATTIVAZIONE == true && a.ATTIVAZIONEMAGFAM == true && a.ANNULLATO == false) ||
                                    (a.RICHIESTAATTIVAZIONE == false && a.ATTIVAZIONEMAGFAM == false && a.ANNULLATO == false) ||
                                    (a.RICHIESTAATTIVAZIONE == true && a.ATTIVAZIONEMAGFAM == false && a.ANNULLATO == false))
                                ).OrderBy(a => a.IDATTIVAZIONEMAGFAM);
                    if (lamf?.Any() ?? false)
                    {
                        var amf = lamf.First();

                        amfm = new AttivazioniMagFamModel()
                        {
                            idAttivazioneMagFam = amf.IDATTIVAZIONEMAGFAM,
                            idMaggiorazioniFamiliari = amf.IDMAGGIORAZIONIFAMILIARI,
                            richiestaAttivazione = amf.RICHIESTAATTIVAZIONE,
                            dataRichiestaAttivazione = amf.DATARICHIESTAATTIVAZIONE,
                            attivazioneMagFam = amf.ATTIVAZIONEMAGFAM,
                            dataAttivazioneMagFam = amf.DATAATTIVAZIONEMAGFAM,
                            dataVariazione = amf.DATAVARIAZIONE,
                            dataAggiornamento = amf.DATAAGGIORNAMENTO,
                            annullato = amf.ANNULLATO
                        };

                    }
                }

            }

            return amfm;
        }

        public IList<AttivazioniMagFamModel> GetListAttivazioniMagFamByIdMagFam(decimal idMaggiorazioniFamiliari)
        {
            List<AttivazioniMagFamModel> lamfm = new List<AttivazioniMagFamModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lamf = db.ATTIVAZIONIMAGFAM.Where(a => a.IDMAGGIORAZIONIFAMILIARI == idMaggiorazioniFamiliari).OrderBy(a => a.IDATTIVAZIONEMAGFAM);
                if (lamf?.Any() ?? false)
                {
                    lamfm = (from e in lamf
                             select new AttivazioniMagFamModel()
                             {
                                 idAttivazioneMagFam = e.IDATTIVAZIONEMAGFAM,
                                 idMaggiorazioniFamiliari = e.IDMAGGIORAZIONIFAMILIARI,
                                 richiestaAttivazione = e.RICHIESTAATTIVAZIONE,
                                 dataRichiestaAttivazione = e.DATARICHIESTAATTIVAZIONE,
                                 attivazioneMagFam = e.ATTIVAZIONEMAGFAM,
                                 dataAttivazioneMagFam = e.DATAATTIVAZIONEMAGFAM,
                                 dataAggiornamento = e.DATAAGGIORNAMENTO,
                                 annullato = e.ANNULLATO,
                                 dataVariazione = e.DATAVARIAZIONE
                             }).ToList();
                }
            }

            return lamfm;
        }

        public AttivazioniMagFamModel GetAttivazioniMagFamById(decimal idAttivazioneMagFam)
        {
            AttivazioniMagFamModel amfm = new AttivazioniMagFamModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);
                if (amf != null && amf.IDATTIVAZIONEMAGFAM > 0)
                {
                    amfm = new AttivazioniMagFamModel()
                    {
                        idAttivazioneMagFam = amf.IDATTIVAZIONEMAGFAM,
                        idMaggiorazioniFamiliari = amf.IDMAGGIORAZIONIFAMILIARI,
                        richiestaAttivazione = amf.RICHIESTAATTIVAZIONE,
                        dataRichiestaAttivazione = amf.DATARICHIESTAATTIVAZIONE,
                        attivazioneMagFam = amf.ATTIVAZIONEMAGFAM,
                        dataAttivazioneMagFam = amf.DATAATTIVAZIONEMAGFAM,
                        dataAggiornamento = amf.DATAAGGIORNAMENTO,
                        annullato = amf.ANNULLATO
                    };
                }

            }

            return amfm;
        }

        public AttivazioniMagFamModel GetUltimaAttivazioneMagFam(decimal idMaggiorazioneFamiliare)
        {
            AttivazioniMagFamModel amfm = new AttivazioniMagFamModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                var lamf = db.ATTIVAZIONIMAGFAM.Where(a => a.IDMAGGIORAZIONIFAMILIARI == idMaggiorazioneFamiliare).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);

                if (lamf?.Any() ?? false)
                {
                    var amf = lamf.First();
                    amfm = new AttivazioniMagFamModel()
                    {
                        idAttivazioneMagFam = amf.IDATTIVAZIONEMAGFAM,
                        idMaggiorazioniFamiliari = amf.IDMAGGIORAZIONIFAMILIARI,
                        richiestaAttivazione = amf.RICHIESTAATTIVAZIONE,
                        dataRichiestaAttivazione = amf.DATARICHIESTAATTIVAZIONE,
                        attivazioneMagFam = amf.ATTIVAZIONEMAGFAM,
                        dataAttivazioneMagFam = amf.DATAATTIVAZIONEMAGFAM,
                        dataAggiornamento = amf.DATAAGGIORNAMENTO,
                        annullato = amf.ANNULLATO
                    };
                }

            }

            return amfm;
        }

        public AttivazioniMagFamModel GetUltimaAttivazioneMagFam(decimal idMaggiorazioneFamiliare, ModelDBISE db)
        {
            AttivazioniMagFamModel amfm = new AttivazioniMagFamModel();

            var lamf = db.ATTIVAZIONIMAGFAM.Where(a => a.IDMAGGIORAZIONIFAMILIARI == idMaggiorazioneFamiliare).OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);

            if (lamf?.Any() ?? false)
            {
                var amf = lamf.First();
                amfm = new AttivazioniMagFamModel()
                {
                    idAttivazioneMagFam = amf.IDATTIVAZIONEMAGFAM,
                    idMaggiorazioniFamiliari = amf.IDMAGGIORAZIONIFAMILIARI,
                    richiestaAttivazione = amf.RICHIESTAATTIVAZIONE,
                    dataRichiestaAttivazione = amf.DATARICHIESTAATTIVAZIONE,
                    attivazioneMagFam = amf.ATTIVAZIONEMAGFAM,
                    dataAttivazioneMagFam = amf.DATAATTIVAZIONEMAGFAM,
                    dataAggiornamento = amf.DATAAGGIORNAMENTO,
                    annullato = amf.ANNULLATO
                };
            }

            return amfm;
        }

        public void SetAttivaziomeMagFam(ref AttivazioniMagFamModel amfm, ModelDBISE db)
        {
            ATTIVAZIONIMAGFAM amf = new ATTIVAZIONIMAGFAM()
            {
                IDMAGGIORAZIONIFAMILIARI = amfm.idMaggiorazioniFamiliari,
                RICHIESTAATTIVAZIONE = amfm.richiestaAttivazione,
                DATARICHIESTAATTIVAZIONE = amfm.dataRichiestaAttivazione,
                ATTIVAZIONEMAGFAM = amfm.attivazioneMagFam,
                DATAATTIVAZIONEMAGFAM = amfm.dataAttivazioneMagFam,
                DATAVARIAZIONE = amfm.dataVariazione,
                DATAAGGIORNAMENTO = amfm.dataAggiornamento,
                ANNULLATO = amfm.annullato
            };

            db.ATTIVAZIONIMAGFAM.Add(amf);
            int i = db.SaveChanges();

            if (1 <= 0)
            {
                throw new Exception("Errore inserimento 'ATTIVAZIONIMAGFAM'");
            }
            else
            {
                amfm.idAttivazioneMagFam = amf.IDATTIVAZIONEMAGFAM;
                var t = amf.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                    "Inserimento di un record di attivazione maggiorazioni familiari.", "ATTIVAZIONIMAGFAM", db,
                    t.IDTRASFERIMENTO, amf.IDATTIVAZIONEMAGFAM);


            }


        }

        /// <summary>
        /// Preleva il record di attivazione familiare non ancora lavorato.
        /// </summary>
        /// <param name="idMaggiorazioniFamiliari"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public AttivazioniMagFamModel GetAttivazioneMagFamDaLavorare(decimal idMaggiorazioniFamiliari, ModelDBISE db)
        {
            AttivazioniMagFamModel amfm = new AttivazioniMagFamModel();

            var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

            if (mf != null && mf.IDMAGGIORAZIONIFAMILIARI > 0)
            {
                var lamf =
                    mf.ATTIVAZIONIMAGFAM.Where(a => a.RICHIESTAATTIVAZIONE == false && a.ATTIVAZIONEMAGFAM == false && a.ANNULLATO == false)
                        .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);
                if (lamf?.Any() ?? false)
                {
                    var amf = lamf.First();
                    amfm = new AttivazioniMagFamModel()
                    {
                        idAttivazioneMagFam = amf.IDATTIVAZIONEMAGFAM,
                        idMaggiorazioniFamiliari = amf.IDMAGGIORAZIONIFAMILIARI,
                        richiestaAttivazione = amf.RICHIESTAATTIVAZIONE,
                        dataRichiestaAttivazione = amf.DATARICHIESTAATTIVAZIONE,
                        attivazioneMagFam = amf.ATTIVAZIONEMAGFAM,
                        dataAttivazioneMagFam = amf.DATAATTIVAZIONEMAGFAM,
                        dataAggiornamento = amf.DATAAGGIORNAMENTO,
                        annullato = amf.ANNULLATO,
                    };
                }
                else
                {
                    throw new Exception("Errore, Attivazione Maggiorazioni familiari relative alle maggiorazioni familiari con id: " + idMaggiorazioniFamiliari + " non trovate.");
                }
            }
            else
            {
                throw new Exception("Errore, Maggiorazione familiare con id: " + idMaggiorazioniFamiliari + " non trovata.");
            }

            return amfm;
        }

        public void AssociaRinunciaMagFam(decimal idAttivazioneFamiliare, decimal idRinunciaMagFam, ModelDBISE db)
        {
            try
            {
                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneFamiliare);
                var item = db.Entry<ATTIVAZIONIMAGFAM>(amf);
                item.State = EntityState.Modified;
                item.Collection(a => a.RINUNCIAMAGGIORAZIONIFAMILIARI).Load();
                var r = db.RINUNCIAMAGGIORAZIONIFAMILIARI.Find(idRinunciaMagFam);
                amf.RINUNCIAMAGGIORAZIONIFAMILIARI.Add(r);

                int i = db.SaveChanges();


                if (i <= 0)
                {
                    throw new Exception("Impossibile associare l'informazione di rinuncia delle maggiorazioni all'attivazione familiare.");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaFormulario(decimal idAttivazioneFamiliare, decimal idDocumento, ModelDBISE db)
        {
            try
            {
                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneFamiliare);
                var item = db.Entry<ATTIVAZIONIMAGFAM>(amf);
                item.State = EntityState.Modified;
                item.Collection(a => a.DOCUMENTI).Load();
                var d = db.DOCUMENTI.Find(idDocumento);
                amf.DOCUMENTI.Add(d);

                int i = db.SaveChanges();


                if (i <= 0)
                {
                    throw new Exception("Impossibile associare il formulario per l'attivazione familiare.");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaConiugeAttivazione(decimal idAttivazioneFamiliare, decimal idConiuge, ModelDBISE db)
        {
            try
            {

                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneFamiliare);
                var item = db.Entry<ATTIVAZIONIMAGFAM>(amf);
                item.State = EntityState.Modified;
                item.Collection(a => a.CONIUGE).Load();
                var c = db.CONIUGE.Find(idConiuge);
                amf.CONIUGE.Add(c);

                int i = db.SaveChanges();


                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il coniuge per l'attivazione familiare {0}.", c.COGNOME + " " + c.NOME));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void AssociaFiglioAttivazione(decimal idAttivazioneFamiliare, decimal idFiglio, ModelDBISE db)
        {
            try
            {

                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneFamiliare);
                var item = db.Entry<ATTIVAZIONIMAGFAM>(amf);
                item.State = EntityState.Modified;
                item.Collection(a => a.FIGLI).Load();
                var f = db.FIGLI.Find(idFiglio);
                amf.FIGLI.Add(f);

                int i = db.SaveChanges();


                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il figlio per l'attivazione familiare {0}.", f.COGNOME + " " + f.NOME));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public void AssociaFiglio(decimal idAttivazioneFamiliare, decimal idFiglio, ModelDBISE db)
        {
            try
            {
                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneFamiliare);
                var item = db.Entry<ATTIVAZIONIMAGFAM>(amf);
                item.State = EntityState.Modified;
                item.Collection(a => a.FIGLI).Load();
                var f = db.FIGLI.Find(idFiglio);
                amf.FIGLI.Add(f);

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il figlio per l'attivazione familiare {0}.", f.COGNOME + " " + f.NOME));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaAltriDatiFamiliari(decimal idAttivazioneFamiliare, decimal idAltriDatiFamiliari, ModelDBISE db)
        {
            try
            {
                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneFamiliare);
                var item = db.Entry<ATTIVAZIONIMAGFAM>(amf);
                item.State = EntityState.Modified;
                item.Collection(a => a.ALTRIDATIFAM).Load();
                var adf = db.ALTRIDATIFAM.Find(idAltriDatiFamiliari);
                amf.ALTRIDATIFAM.Add(adf);

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il i dati per l'attivazione familiare {0}.", idAttivazioneFamiliare));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaAltriDatiFamiliari_Coniuge(decimal idConiuge, decimal idAltriDatiFamiliari, ModelDBISE db)
        {
            try
            {
                var c = db.CONIUGE.Find(idConiuge);
                var item = db.Entry<CONIUGE>(c);
                item.State = EntityState.Modified;
                item.Collection(a => a.ALTRIDATIFAM).Load();
                var adf = db.ALTRIDATIFAM.Find(idAltriDatiFamiliari);
                c.ALTRIDATIFAM.Add(adf);

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Impossibile associare il coniuge a altri dati familiari");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaDocumentoAttivazione(decimal idAttivazioneMagFam, decimal idDocumento, ModelDBISE db)
        {
            try
            {
                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);
                var item = db.Entry<ATTIVAZIONIMAGFAM>(amf);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.DOCUMENTI).Load();
                var d = db.DOCUMENTI.Find(idDocumento);
                amf.DOCUMENTI.Add(d);

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il documento per la fase di attivazione. {0}", idAttivazioneMagFam));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssociaPensioneAttivazione(decimal idAttivazioneMagFam, decimal idPensione, ModelDBISE db)
        {
            try
            {
                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);
                var item = db.Entry<ATTIVAZIONIMAGFAM>(amf);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.PENSIONE).Load();
                var p = db.PENSIONE.Find(idPensione);
                amf.PENSIONE.Add(p);

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare la pensione per la fase di attivazione. {0}", idAttivazioneMagFam));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
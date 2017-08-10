using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using NewISE.Models.Tools;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtPensione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<PensioneConiugeModel> GetPensioniByIdConiuge(decimal idConiuge)
        {
            List<PensioneConiugeModel> lpc = new List<PensioneConiugeModel>();
            using (ModelDBISE db = new ModelDBISE())
            {
                var lp =
                    db.CONIUGE.Find(idConiuge)
                        .PENSIONE.Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.DATAINIZIO)
                        .ToList();
                if (lp?.Any() ?? false)
                {
                    lpc = (from e in lp
                           select new PensioneConiugeModel()
                           {
                               idPensioneConiuge = e.IDPENSIONE,
                               importoPensione = e.IMPORTOPENSIONE,
                               dataInizioValidita = e.DATAINIZIO,
                               dataFineValidita = e.DATAFINE,
                               dataAggiornamento = e.DATAAGGIORNAMENTO,
                               annullato = e.ANNULLATO
                           }).ToList();
                }

                return lpc;
            }
        }

        public IList<PensioneConiugeModel> GetPensioniByIdConiuge(decimal idConiuge, ModelDBISE db)
        {
            List<PensioneConiugeModel> lpc = new List<PensioneConiugeModel>();

            var lp =
                db.CONIUGE.Find(idConiuge)
                    .PENSIONE.Where(a => a.ANNULLATO == false)
                    .OrderBy(a => a.DATAINIZIO)
                    .ToList();
            if (lp?.Any() ?? false)
            {
                lpc = (from e in lp
                       select new PensioneConiugeModel()
                       {
                           idPensioneConiuge = e.IDPENSIONE,
                           importoPensione = e.IMPORTOPENSIONE,
                           dataInizioValidita = e.DATAINIZIO,
                           dataFineValidita = e.DATAFINE,
                           dataAggiornamento = e.DATAAGGIORNAMENTO,
                           annullato = e.ANNULLATO
                       }).ToList();
            }

            return lpc;

        }



        public PensioneConiugeModel GetPensioneByID(decimal idPensione)
        {
            PensioneConiugeModel pcm = new PensioneConiugeModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var pc = db.PENSIONE.Find(idPensione);

                pcm = new PensioneConiugeModel()
                {
                    idPensioneConiuge = pc.IDPENSIONE,
                    importoPensione = pc.IMPORTOPENSIONE,
                    dataInizioValidita = pc.DATAINIZIO,
                    dataFineValidita = pc.DATAFINE,
                    dataAggiornamento = pc.DATAAGGIORNAMENTO,
                    annullato = pc.ANNULLATO,
                    //Coniugi = (from e in pc.CONIUGE
                    //           select new ConiugeModel()
                    //           {

                    //           }).ToList();
                };


                return pcm;
            }
        }

        public void EliminaImportoPensione(PensioneConiugeModel pcm, decimal idConiuge)
        {
            PensioneConiugeModel pcmPrecedente = new PensioneConiugeModel();


            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    pcmPrecedente = PrelevaMovimentoPrecedente(pcm, idConiuge, db);

                    if (pcmPrecedente != null && pcmPrecedente.HasValue())
                    {
                        pcm.Annulla(db);
                        pcmPrecedente.Annulla(db);

                        PensioneConiugeModel pcmNew = new PensioneConiugeModel()
                        {

                            dataInizioValidita = pcmPrecedente.dataInizioValidita,
                            dataFineValidita = pcm.dataFineValidita,
                            importoPensione = pcmPrecedente.importoPensione,
                            dataAggiornamento = DateTime.Now,
                            annullato = false,
                        };

                        SetPensioneConiuge(ref pcmNew, idConiuge, db);
                    }
                    else
                    {
                        pcm.Annulla(db);
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

        public void EditImportoPensione(PensioneConiugeModel pcm, decimal idConiuge)
        {
            PensioneConiugeModel pcmPrecedente = new PensioneConiugeModel();
            PensioneConiugeModel pcmSuccessivo = new PensioneConiugeModel();
            List<PensioneConiugeModel> lpcmInteressati = new List<PensioneConiugeModel>();


            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var pcmDB = db.PENSIONE.Find(pcm.idPensioneConiuge);

                    if (pcmDB != null && pcmDB.IDPENSIONE > 0)
                    {
                        if (pcmDB.DATAINIZIO != pcm.dataInizioValidita || pcmDB.DATAFINE != pcm.dataFineValidita)
                        {
                            lpcmInteressati = PrelevaMovPensioneInteressati(idConiuge, pcm.dataInizioValidita, pcm.dataFineValidita.Value, db).OrderBy(a => a.dataInizioValidita).ToList();
                            if (lpcmInteressati != null && lpcmInteressati.Count > 0)
                            {
                                lpcmInteressati.ForEach(a => a.Annulla(db));

                                pcmPrecedente = lpcmInteressati.First();
                                pcmSuccessivo = lpcmInteressati.Last();

                                if (pcm.dataInizioValidita > pcmPrecedente.dataInizioValidita)
                                {
                                    PensioneConiugeModel pcmLav = new PensioneConiugeModel()
                                    {

                                        importoPensione = pcmPrecedente.importoPensione,
                                        dataInizioValidita = pcmPrecedente.dataInizioValidita,
                                        dataFineValidita = pcm.dataInizioValidita.AddDays(-1),
                                        dataAggiornamento = DateTime.Now,
                                        annullato = false,
                                    };

                                    SetPensioneConiuge(ref pcmLav, idConiuge, db);
                                }

                                SetPensioneConiuge(ref pcm, idConiuge, db);

                                if (pcm.dataFineValidita < pcmSuccessivo.dataFineValidita)
                                {
                                    PensioneConiugeModel pcmLav = new PensioneConiugeModel()
                                    {

                                        importoPensione = pcmSuccessivo.importoPensione,
                                        dataInizioValidita = pcm.dataFineValidita.Value.AddDays(1),
                                        dataFineValidita = pcmSuccessivo.dataFineValidita,
                                        dataAggiornamento = DateTime.Now,
                                        annullato = false,
                                    };

                                    SetPensioneConiuge(ref pcmLav, idConiuge, db);
                                }

                            }
                            else
                            {
                                SetPensioneConiuge(ref pcm, idConiuge, db);
                            }
                        }
                        else
                        {
                            if (pcmDB.IMPORTOPENSIONE != pcm.importoPensione && pcm.importoPensione > 0)
                            {
                                pcm.Annulla(db);

                                PensioneConiugeModel pcmLav = new PensioneConiugeModel()
                                {

                                    importoPensione = pcm.importoPensione,
                                    dataInizioValidita = pcm.dataInizioValidita,
                                    dataFineValidita = pcm.dataFineValidita,
                                    dataAggiornamento = DateTime.Now,
                                    annullato = false,
                                };

                                SetPensioneConiuge(ref pcmLav, idConiuge, db);
                            }
                        }



                        db.Database.CurrentTransaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public void SetNuovoImportoPensione(PensioneConiugeModel pcm, decimal idConiuge, ModelDBISE db)
        {

            PensioneConiugeModel pcmPrecedente = new PensioneConiugeModel();
            PensioneConiugeModel pcmSuccessivo = new PensioneConiugeModel();
            List<PensioneConiugeModel> lpcmInteressati = new List<PensioneConiugeModel>();


            db.Database.BeginTransaction();

            try
            {

                lpcmInteressati =
                    PrelevaMovPensioneInteressati(idConiuge, pcm.dataInizioValidita, pcm.dataFineValidita.Value, db)
                        .OrderBy(a => a.dataInizioValidita)
                        .ToList();


                if (lpcmInteressati != null && lpcmInteressati.Count > 0)
                {
                    lpcmInteressati.ForEach(a => a.Annulla(db));

                    pcmPrecedente = lpcmInteressati.First();
                    pcmSuccessivo = lpcmInteressati.Last();

                    if (pcm.dataInizioValidita > pcmPrecedente.dataInizioValidita)
                    {
                        PensioneConiugeModel pcmLav = new PensioneConiugeModel()
                        {

                            importoPensione = pcmPrecedente.importoPensione,
                            dataInizioValidita = pcmPrecedente.dataInizioValidita,
                            dataFineValidita = pcm.dataInizioValidita.AddDays(-1),
                            dataAggiornamento = DateTime.Now,
                            annullato = false,
                        };

                        SetPensioneConiuge(ref pcmLav, idConiuge, db);
                    }

                    SetPensioneConiuge(ref pcm, idConiuge, db);

                    if (pcm.dataFineValidita < pcmSuccessivo.dataFineValidita)
                    {
                        PensioneConiugeModel pcmLav = new PensioneConiugeModel()
                        {

                            importoPensione = pcmSuccessivo.importoPensione,
                            dataInizioValidita = pcm.dataFineValidita.Value.AddDays(1),
                            dataFineValidita = pcmSuccessivo.dataFineValidita,
                            dataAggiornamento = DateTime.Now,
                            annullato = false,
                        };

                        SetPensioneConiuge(ref pcmLav, idConiuge, db);
                    }

                }
                else
                {
                    SetPensioneConiuge(ref pcm, idConiuge, db);
                }

                db.Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                db.Database.CurrentTransaction.Rollback();
                throw ex;
            }

        }

        public void SetNuovoImportoPensione(ref PensioneConiugeModel pcm, decimal idConiuge)
        {

            PensioneConiugeModel pcmPrecedente = new PensioneConiugeModel();
            PensioneConiugeModel pcmSuccessivo = new PensioneConiugeModel();
            List<PensioneConiugeModel> lpcmInteressati = new List<PensioneConiugeModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {

                    lpcmInteressati =
                        PrelevaMovPensioneInteressati(idConiuge, pcm.dataInizioValidita, pcm.dataFineValidita.Value, db)
                            .OrderBy(a => a.dataInizioValidita)
                            .ToList();


                    if (lpcmInteressati != null && lpcmInteressati.Count > 0)
                    {
                        lpcmInteressati.ForEach(a => a.Annulla(db));

                        pcmPrecedente = lpcmInteressati.First();
                        pcmSuccessivo = lpcmInteressati.Last();

                        if (pcm.dataInizioValidita > pcmPrecedente.dataInizioValidita)
                        {
                            PensioneConiugeModel pcmLav = new PensioneConiugeModel()
                            {

                                importoPensione = pcmPrecedente.importoPensione,
                                dataInizioValidita = pcmPrecedente.dataInizioValidita,
                                dataFineValidita = pcm.dataInizioValidita.AddDays(-1),
                                dataAggiornamento = DateTime.Now,
                                annullato = false,
                            };

                            SetPensioneConiuge(ref pcmLav, idConiuge, db);
                        }

                        SetPensioneConiuge(ref pcm, idConiuge, db);

                        if (pcm.dataFineValidita < pcmSuccessivo.dataFineValidita)
                        {
                            PensioneConiugeModel pcmLav = new PensioneConiugeModel()
                            {

                                importoPensione = pcmSuccessivo.importoPensione,
                                dataInizioValidita = pcm.dataFineValidita.Value.AddDays(1),
                                dataFineValidita = pcmSuccessivo.dataFineValidita,
                                dataAggiornamento = DateTime.Now,
                                annullato = false,
                            };

                            SetPensioneConiuge(ref pcmLav, idConiuge, db);
                        }

                    }
                    else
                    {
                        SetPensioneConiuge(ref pcm, idConiuge, db);
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

        private PensioneConiugeModel PrelevaMovimentoPrecedente(PensioneConiugeModel pcm, decimal idConiuge, ModelDBISE db)
        {
            PensioneConiugeModel pcmPrecedente = new PensioneConiugeModel();

            try
            {
                var lpc = db.CONIUGE.Find(idConiuge).PENSIONE.Where(a => a.ANNULLATO == false && a.DATAFINE < pcm.dataInizioValidita).OrderByDescending(a => a.DATAFINE).ToList();

                if (lpc != null && lpc.Count > 0)
                {
                    var pc = lpc.First();

                    pcmPrecedente = new PensioneConiugeModel()
                    {
                        idPensioneConiuge = pc.IDPENSIONE,
                        importoPensione = pc.IMPORTOPENSIONE,
                        dataInizioValidita = pc.DATAINIZIO,
                        dataFineValidita = pc.DATAFINE,
                        dataAggiornamento = pc.DATAAGGIORNAMENTO,
                        annullato = pc.ANNULLATO

                    };
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }

            return pcmPrecedente;

        }

        private IList<PensioneConiugeModel> PrelevaMovPensioneInteressati(decimal idConiuge, DateTime dtIni, DateTime dtFin, ModelDBISE db)
        {
            List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();

            var lpc =
                db.CONIUGE.Find(idConiuge)
                    .PENSIONE.Where(
                        a =>
                            a.ANNULLATO == false && a.DATAINIZIO <= dtFin &&
                            a.DATAFINE >= dtIni)
                    .OrderBy(a => a.DATAINIZIO)
                    .ToList();


            if (lpc?.Any() ?? false)
            {
                lpcm = (from e in lpc
                        select new PensioneConiugeModel()
                        {
                            idPensioneConiuge = e.IDPENSIONE,
                            importoPensione = e.IMPORTOPENSIONE,
                            dataInizioValidita = e.DATAINIZIO,
                            dataFineValidita = e.DATAFINE,
                            dataAggiornamento = e.DATAAGGIORNAMENTO,
                            annullato = e.ANNULLATO
                        }).ToList();
            }

            return lpcm;
        }

        public void SetPensioneConiuge(ref PensioneConiugeModel pcm, decimal idConiuge, ModelDBISE db)
        {
            try
            {
                var c = db.CONIUGE.Find(idConiuge);
                var item = db.Entry<CONIUGE>(c);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.PENSIONE).Load();
                PENSIONE pc = new PENSIONE()
                {
                    IMPORTOPENSIONE = pcm.importoPensione,
                    DATAINIZIO = pcm.dataInizioValidita,
                    DATAFINE = pcm.dataFineValidita.Value,
                    DATAAGGIORNAMENTO = pcm.dataAggiornamento,
                    ANNULLATO = pcm.annullato
                };

                c.PENSIONE.Add(pc);

                int i = db.SaveChanges();

                if (i > 0)
                {
                    pcm.idPensioneConiuge = pc.IDPENSIONE;

                    decimal idTrasferimento = pc.CONIUGE.First().MAGGIORAZIONEFAMILIARI.IDTRASFERIMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un importo pensione", "PENSIONE", db, idTrasferimento, pc.IDPENSIONE);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetPensione(ref PensioneConiugeModel pcm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                PENSIONE p = new PENSIONE()
                {
                    IMPORTOPENSIONE = pcm.importoPensione,
                    DATAINIZIO = pcm.dataInizioValidita,
                    DATAFINE = pcm.dataFineValidita.HasValue ? pcm.dataFineValidita.Value : Utility.DataFineStop(),
                    DATAAGGIORNAMENTO = pcm.dataAggiornamento,
                    ANNULLATO = pcm.annullato
                };

                db.PENSIONE.Add(p);

                if (db.SaveChanges() > 0)
                {
                    pcm.idPensioneConiuge = p.IDPENSIONE;
                }
            }
        }

        public void SetPensione(ref PensioneConiugeModel pcm, ModelDBISE db)
        {
            PENSIONE p = new PENSIONE()
            {
                IMPORTOPENSIONE = pcm.importoPensione,
                DATAINIZIO = pcm.dataInizioValidita,
                DATAFINE = pcm.dataFineValidita.HasValue ? pcm.dataFineValidita.Value : Utility.DataFineStop(),
                DATAAGGIORNAMENTO = pcm.dataAggiornamento,
                ANNULLATO = pcm.annullato
            };

            db.PENSIONE.Add(p);

            if (db.SaveChanges() > 0)
            {
                pcm.idPensioneConiuge = p.IDPENSIONE;
            }
        }

        public IList<PensioneConiugeModel> GetListaPensioneConiugeByIdConiuge(decimal idConiuge)
        {
            List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var c = db.CONIUGE.Find(idConiuge);

                if (c != null && c.IDCONIUGE > 0)
                {
                    var lpc = c.PENSIONE.Where(a => a.ANNULLATO == false).OrderBy(a => a.DATAINIZIO).ToList();

                    if (lpc?.Any() ?? false)
                    {
                        lpcm = (from e in lpc
                                select new PensioneConiugeModel()
                                {
                                    idPensioneConiuge = e.IDPENSIONE,
                                    importoPensione = e.IMPORTOPENSIONE,
                                    dataInizioValidita = e.DATAINIZIO,
                                    dataFineValidita = e.DATAFINE,
                                    dataAggiornamento = e.DATAAGGIORNAMENTO,
                                    annullato = e.ANNULLATO
                                }).ToList();
                    }
                }
            }

            return lpcm;
        }

        /// <summary>
        /// Preleva una lista di importi pensione per l'idMaggiorazioneConiuge maggiori alla data di inizio passata
        /// </summary>
        /// <param name="idMaggiorazioneConiuge"></param>
        /// <param name="dtIni"></param>
        /// <returns></returns>
        public IList<PensioneConiugeModel> GetListaPensioneConiugeByMaggiorazioneConiuge(decimal idMaggiorazioneConiuge, DateTime dtIni)
        {
            List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();

            //using (ModelDBISE db = new ModelDBISE())
            //{
            //    var mc = db.MAGGIORAZIONECONIUGE.Find(idMaggiorazioneConiuge);

            //    if (mc != null && mc.IDMAGGIORAZIONECONIUGE > 0)
            //    {
            //        var lpc = mc.PENSIONE.Where(a => a.ANNULLATO == false && a.DATAINIZIO >= dtIni).OrderBy(a => a.DATAINIZIO).ToList();

            //        if (lpc != null && lpc.Count > 0)
            //        {
            //            lpcm = (from e in lpc
            //                    select new PensioneConiugeModel()
            //                    {
            //                        idMaggiorazioneConiuge = idMaggiorazioneConiuge,
            //                        idPensioneConiuge = e.IDPENSIONE,
            //                        importoPensione = e.IMPORTOPENSIONE,
            //                        dataInizioValidita = e.DATAINIZIO,
            //                        dataFineValidita = e.DATAFINE,
            //                        dataAggiornamento = e.DATAAGGIORNAMENTO,
            //                        annullato = e.ANNULLATO
            //                    }).ToList();
            //        }
            //    }
            //}

            return lpcm;
        }

        public bool HasPensione(decimal idConiuge)
        {
            bool ret = false;

            using (ModelDBISE db = new ModelDBISE())
            {
                var c = db.CONIUGE.Find(idConiuge);

                if (c != null && c.IDCONIUGE > 0)
                {
                    var lpc = c.PENSIONE.Where(a => a.ANNULLATO == false).ToList();
                    if (lpc?.Any() ?? false)
                    {
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
            }

            return ret;
        }

        public void AnnullaMovimentiPensione(IList<PensioneConiugeModel> lpcm, ModelDBISE db)
        {
            try
            {
                foreach (var i in lpcm)
                {
                    var pc = db.PENSIONE.Find(i.idPensioneConiuge);
                    if (pc != null && pc.IDPENSIONE > 0)
                    {
                        pc.ANNULLATO = true;

                        db.SaveChanges();
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
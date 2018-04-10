using NewISE.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime;
using NewISE.Models.Tools;
using NewISE.Models.DBModel.Enum;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtPensione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public void VerificaDataInizioPensione(decimal idConiuge, DateTime dataInizioPensione)
        {
            //bool ret = false;

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.CONIUGE.Find(idConiuge).MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;

                if (dataInizioPensione < t.DATAPARTENZA)
                {
                    throw new Exception(string.Format("La data d'inizio validità per la pensione non può essere inferiore alla data di partenza del trasferimento ({0}).", t.DATAPARTENZA.ToShortDateString()));
                }
            }


        }

        /// <summary>
        /// Preleva le pensioni del coniuge passato come id, relatve all'attivazione passata come ID
        /// </summary>
        /// <param name="idConiuge">ID del coniufe</param>
        /// <param name="idAttivazioneMagFam">ID dell'attivazione</param>
        /// <returns></returns>
        public IList<PensioneConiugeModel> GetPensioniConiuge(decimal idConiuge, decimal idAttivazioneMagFam)
        {
            List<PensioneConiugeModel> lpc = new List<PensioneConiugeModel>();
            using (ModelDBISE db = new ModelDBISE())
            {
                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

                var c = amf.CONIUGE.First(a => a.IDCONIUGE == idConiuge);

                if (amf.RICHIESTAATTIVAZIONE == false)
                {
                    var p_inlav = c.PENSIONE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).OrderBy(a => a.DATAINIZIO);
                    var min_data_inlav = c.DATAINIZIOVALIDITA;
                    if (p_inlav?.Any() ?? false)
                    {
                        min_data_inlav = p_inlav.First().DATAINIZIO;
                    }
                    p_inlav = c.PENSIONE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).OrderByDescending(a => a.DATAFINE);
                    var max_data_inlav = c.DATAFINEVALIDITA;
                    if (p_inlav?.Any() ?? false)
                    {
                        max_data_inlav = p_inlav.First().DATAFINE;
                    }

                    //aggiunge i record attivi antecedenti a quelli in lavorazione
                    var lp = c.PENSIONE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato
                                                && a.DATAINIZIO < min_data_inlav)
                                .OrderBy(a => a.DATAINIZIO)
                                .ToList();
                    if (lp?.Any() ?? false)
                    {
                        lpc.AddRange(from e in lp
                                     select new PensioneConiugeModel()
                                     {
                                         idPensioneConiuge = e.IDPENSIONE,
                                         importoPensione = e.IMPORTOPENSIONE,
                                         dataInizioValidita = e.DATAINIZIO,
                                         dataFineValidita = e.DATAFINE,
                                         dataAggiornamento = e.DATAAGGIORNAMENTO,
                                         idStatoRecord = e.IDSTATORECORD
                                     });
                    }


                    //aggiunge i record in lavorazione
                    lp = c.PENSIONE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione)
                        .OrderBy(a => a.DATAINIZIO)
                        .ToList();
                    if (lp?.Any() ?? false)
                    {
                        lpc.AddRange(from e in lp
                                     select new PensioneConiugeModel()
                                     {
                                         idPensioneConiuge = e.IDPENSIONE,
                                         importoPensione = e.IMPORTOPENSIONE,
                                         dataInizioValidita = e.DATAINIZIO,
                                         dataFineValidita = e.DATAFINE,
                                         dataAggiornamento = e.DATAAGGIORNAMENTO,
                                         idStatoRecord = e.IDSTATORECORD
                                     });
                    }

                    //aggiunge i record attivi successivi a quelli in lavorazione
                    lp = c.PENSIONE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato
                                                && a.DATAINIZIO > max_data_inlav)
                                .OrderBy(a => a.DATAINIZIO)
                                .ToList();
                    if (lp?.Any() ?? false)
                    {
                        lpc.AddRange(from e in lp
                                     select new PensioneConiugeModel()
                                     {
                                         idPensioneConiuge = e.IDPENSIONE,
                                         importoPensione = e.IMPORTOPENSIONE,
                                         dataInizioValidita = e.DATAINIZIO,
                                         dataFineValidita = e.DATAFINE,
                                         dataAggiornamento = e.DATAAGGIORNAMENTO,
                                         idStatoRecord = e.IDSTATORECORD
                                     });
                    }
                }
                else
                {
                    //aggiunge i record non annullati
                    var lp = c.PENSIONE.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                        .OrderBy(a => a.DATAINIZIO)
                        .ToList();
                    if (lp?.Any() ?? false)
                    {
                        lpc.AddRange(from e in lp
                                     select new PensioneConiugeModel()
                                     {
                                         idPensioneConiuge = e.IDPENSIONE,
                                         importoPensione = e.IMPORTOPENSIONE,
                                         dataInizioValidita = e.DATAINIZIO,
                                         dataFineValidita = e.DATAFINE,
                                         dataAggiornamento = e.DATAAGGIORNAMENTO,
                                         idStatoRecord = e.IDSTATORECORD
                                     });
                    }
                }
                return lpc;
            }
        }


        /// <summary>
        /// Preleva la pensione valida alla data passata.
        /// </summary>
        /// <param name="idConiuge"></param>
        /// <param name="dt"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public PensioneConiugeModel GetPensioniByIdConiuge(decimal idConiuge, DateTime dt, ModelDBISE db)
        {
            PensioneConiugeModel pc = new PensioneConiugeModel();

            var lp =
                db.CONIUGE.Find(idConiuge)
                    .PENSIONE.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && dt >= a.DATAINIZIO && dt <= a.DATAFINE)
                    .OrderByDescending(a => a.DATAINIZIO)
                    .ToList();
            if (lp?.Any() ?? false)
            {

                var lpc = (from e in lp
                           select new PensioneConiugeModel()
                           {
                               idPensioneConiuge = e.IDPENSIONE,
                               importoPensione = e.IMPORTOPENSIONE,
                               dataInizioValidita = e.DATAINIZIO,
                               dataFineValidita = e.DATAFINE,
                               dataAggiornamento = e.DATAAGGIORNAMENTO,
                               idStatoRecord = e.IDSTATORECORD
                           }).ToList();

                pc = lpc.First();
            }

            return pc;

        }

        public IList<PensioneConiugeModel> GetPensioniByIdConiuge(decimal idConiuge, ModelDBISE db)
        {
            List<PensioneConiugeModel> lpc = new List<PensioneConiugeModel>();

            var lp =
                db.CONIUGE.Find(idConiuge)
                    .PENSIONE.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
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
                           idStatoRecord = e.IDSTATORECORD
                       }).ToList();
            }

            return lpc;

        }


        public IList<PensioneConiugeModel> GetPensioniByIdConiuge(decimal idConiuge)
        {
            List<PensioneConiugeModel> lpc = new List<PensioneConiugeModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lp =
                db.CONIUGE.Find(idConiuge)
                    .PENSIONE.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
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
                               idStatoRecord = e.IDSTATORECORD
                           }).ToList();
                }
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
                    idStatoRecord = pc.IDSTATORECORD,
                    //Coniugi = (from e in pc.CONIUGE
                    //           select new ConiugeModel()
                    //           {

                    //           }).ToList();
                };


                return pcm;
            }
        }

        public void EliminaImportoPensione(PensioneConiugeModel pcm, decimal idConiuge, decimal idAttivazioneMagFam)
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
                            idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                        };

                        SetPensioneConiuge(ref pcmNew, idConiuge, idAttivazioneMagFam, db);
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

        public void EditImportoPensione(PensioneConiugeModel pcm, decimal idConiuge, decimal idAttivazioneMagFam)
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
                                        idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    };

                                    SetPensioneConiuge(ref pcmLav, idConiuge, idAttivazioneMagFam, db);
                                }

                                SetPensioneConiuge(ref pcm, idConiuge, idAttivazioneMagFam, db);

                                if (pcm.dataFineValidita < pcmSuccessivo.dataFineValidita)
                                {
                                    PensioneConiugeModel pcmLav = new PensioneConiugeModel()
                                    {

                                        importoPensione = pcmSuccessivo.importoPensione,
                                        dataInizioValidita = pcm.dataFineValidita.Value.AddDays(1),
                                        dataFineValidita = pcmSuccessivo.dataFineValidita,
                                        dataAggiornamento = DateTime.Now,
                                        idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione
                                    };

                                    SetPensioneConiuge(ref pcmLav, idConiuge, idAttivazioneMagFam, db);
                                }

                            }
                            else
                            {
                                SetPensioneConiuge(ref pcm, idConiuge, idAttivazioneMagFam, db);
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
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione
                                };

                                SetPensioneConiuge(ref pcmLav, idConiuge, idAttivazioneMagFam, db);
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

        public void SetNuovoImportoPensione(PensioneConiugeModel pcm, decimal idConiuge, decimal idAttivazioneMagFam, ModelDBISE db)
        {

            PensioneConiugeModel pcmPrecedente = new PensioneConiugeModel();
            PensioneConiugeModel pcmSuccessivo = new PensioneConiugeModel();
            List<PensioneConiugeModel> lpcmInteressati = new List<PensioneConiugeModel>();


            //db.Database.BeginTransaction();

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
                            idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                        };

                        SetPensioneConiuge(ref pcmLav, idConiuge, idAttivazioneMagFam, db);
                    }

                    SetPensioneConiuge(ref pcm, idConiuge, idAttivazioneMagFam, db);

                    if (pcm.dataFineValidita < pcmSuccessivo.dataFineValidita)
                    {
                        PensioneConiugeModel pcmLav = new PensioneConiugeModel()
                        {

                            importoPensione = pcmSuccessivo.importoPensione,
                            dataInizioValidita = pcm.dataFineValidita.Value.AddDays(1),
                            dataFineValidita = pcmSuccessivo.dataFineValidita,
                            dataAggiornamento = DateTime.Now,
                            idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                        };

                        SetPensioneConiuge(ref pcmLav, idConiuge, idAttivazioneMagFam, db);
                    }

                }
                else
                {
                    SetPensioneConiuge(ref pcm, idConiuge, idAttivazioneMagFam, db);
                }

                //db.Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                //db.Database.CurrentTransaction.Rollback();
                throw ex;
            }

        }

        public void SetNuovoImportoPensione(ref PensioneConiugeModel pcm, decimal idConiuge, decimal idAttivazioneMagFam)
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
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                            };

                            SetPensioneConiuge(ref pcmLav, idConiuge, idAttivazioneMagFam, db);
                        }

                        SetPensioneConiuge(ref pcm, idConiuge, idAttivazioneMagFam, db);

                        if (pcm.dataFineValidita < pcmSuccessivo.dataFineValidita)
                        {
                            PensioneConiugeModel pcmLav = new PensioneConiugeModel()
                            {

                                importoPensione = pcmSuccessivo.importoPensione,
                                dataInizioValidita = pcm.dataFineValidita.Value.AddDays(1),
                                dataFineValidita = pcmSuccessivo.dataFineValidita,
                                dataAggiornamento = DateTime.Now,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                            };

                            SetPensioneConiuge(ref pcmLav, idConiuge, idAttivazioneMagFam, db);
                        }

                    }
                    else
                    {
                        SetPensioneConiuge(ref pcm, idConiuge, idAttivazioneMagFam, db);
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
                var lpc =
                    db.CONIUGE.Find(idConiuge)
                        .PENSIONE.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.DATAFINE < pcm.dataInizioValidita)
                        .OrderByDescending(a => a.DATAFINE)
                        .ToList();

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
                        idStatoRecord = pc.IDSTATORECORD

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
                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && 
                            a.DATAINIZIO <= dtFin &&
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
                            idStatoRecord = e.IDSTATORECORD
                        }).ToList();
            }

            return lpcm;
        }

        public void SetPensioneConiuge(ref PensioneConiugeModel pcm, decimal idConiuge, decimal idAttivazioneMagFam, ModelDBISE db)
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
                    IDSTATORECORD = pcm.idStatoRecord
                };

                c.PENSIONE.Add(pc);

                int i = db.SaveChanges();

                if (i > 0)
                {
                    pcm.idPensioneConiuge = pc.IDPENSIONE;

                    decimal idTrasferimento = pc.CONIUGE.First().MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un importo pensione", "PENSIONE", db, idTrasferimento, pc.IDPENSIONE);

                    this.AssociaPensioneAttivazioneMagFam(idAttivazioneMagFam, pc.IDPENSIONE, db);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AssociaPensioneAttivazioneMagFam(decimal idAttivazioneMagFam, decimal idPensione, ModelDBISE db)
        {
            try
            {
                var at = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);
                var item = db.Entry<ATTIVAZIONIMAGFAM>(at);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.PENSIONE).Load();
                var p = db.PENSIONE.Find(idPensione);

                at.PENSIONE.Add(p);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Impossibile associare la pensione all'arrivazione " + idAttivazioneMagFam);
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
                    IDSTATORECORD = pcm.idStatoRecord
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
                IDSTATORECORD = pcm.idStatoRecord
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
                    var lpc = c.PENSIONE.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderBy(a => a.DATAINIZIO).ToList();

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
                                    idStatoRecord = e.IDSTATORECORD
                                }).ToList();
                    }
                }
            }

            return lpcm;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idMaggiorazioneConiuge"></param>
        /// <param name="dtIni"></param>
        /// <returns></returns>
        public IList<PensioneConiugeModel> GetListaPensioneConiugeByMaggiorazioneConiuge(decimal idConiuge, DateTime dtIni)
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
                    var lpc = c.PENSIONE.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();
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
        /// <summary>
        /// Verifica se è presente la pensione per l'attività passata.
        /// </summary>
        /// <param name="idConiuge"></param>
        /// <param name="idAttivitaMagFam"></param>
        /// <returns></returns>
        public bool HasPensione(decimal idConiuge, decimal idAttivitaMagFam)
        {
            bool ret = false;

            using (ModelDBISE db = new ModelDBISE())
            {
                var c = db.CONIUGE.Find(idConiuge);

                if (c != null && c.IDCONIUGE > 0)
                {
                    var lpc = c.PENSIONE.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.ATTIVAZIONIMAGFAM.Any(b => b.IDATTIVAZIONEMAGFAM == idAttivitaMagFam)).ToList();
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
                        pc.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool PensioneModificataByIDConiuge(decimal idConiuge)
        {
            bool modificata = false;

            using (ModelDBISE db = new ModelDBISE())
            {
                var c = db.CONIUGE.Find(idConiuge);

                var last_pcl = c.PENSIONE.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDPENSIONE).ToList();

                if (last_pcl?.Any() ?? false)
                {
                    var last_pc = last_pcl.First();

                    var attl = last_pc.ATTIVAZIONIMAGFAM.OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);
                    if (attl?.Any() ?? false)
                    {
                        var att = attl.First();
                        if (att.ATTIVAZIONEMAGFAM == false)
                        {
                            modificata = true;
                        }
                    }

                }
            }

            return modificata;
            
        }

    }
}
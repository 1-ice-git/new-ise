using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtPensione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public void SetNuovoImportoPensione(PensioneConiugeModel pcm)
        {
            List<PensioneConiugeModel> lpcmSuccessivi = new List<PensioneConiugeModel>();
            PensioneConiugeModel pcmNew = new PensioneConiugeModel();
            PensioneConiugeModel pcmPrecedente = new PensioneConiugeModel();
            List<PensioneConiugeModel> lpcmInteressati = new List<PensioneConiugeModel>();


            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {

                    bool prima = EsistonoMovimentiPrima(pcm, db);
                    bool dopo = EsistonoMovimentiSuccessivi(pcm, db);

                    if (prima)
                    {
                        pcmPrecedente = PrelevaMovimentoAntecedente(pcm, db);
                    }

                    if (dopo)
                    {
                        lpcmSuccessivi = PrelevaMovimentiSuccessivi(pcm, db).ToList();
                    }

                    lpcmInteressati = PrelevaMovimentiInteressati(pcm, db).ToList();

                    if (lpcmInteressati != null && lpcmInteressati.Count > 0)
                    {
                        lpcmInteressati.ForEach(a => a.annullato = true);

                    }

                    if (prima)
                    {
                        PensioneConiugeModel pcmLav = new PensioneConiugeModel()
                        {
                            importoPensione = lpcmInteressati.First().importoPensione,
                            dataInizioValidita = lpcmInteressati.First().dataInizioValidita,
                            dataFineValidita = pcm.dataInizioValidita.AddDays(-1),
                            dataAggiornamento = DateTime.Now,
                            annullato = false,

                        };

                        SetPensioneMaggiorazioneConiuge(ref pcmLav, db);

                    }

                    SetPensioneMaggiorazioneConiuge(ref pcm, db);

                    if (dopo)
                    {
                        PensioneConiugeModel pcmLav = new PensioneConiugeModel()
                        {
                            importoPensione = lpcmInteressati.Last().importoPensione,
                            dataInizioValidita = pcm.dataFineValidita.Value.AddDays(1),
                            dataFineValidita = lpcmInteressati.Last().dataFineValidita,
                            dataAggiornamento = DateTime.Now,
                            annullato = false,

                        };

                        SetPensioneMaggiorazioneConiuge(ref pcmLav, db);
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

        public bool EsistonoMovimentiPrima(PensioneConiugeModel pcm, ModelDBISE db)
        {
            return db.MAGGIORAZIONECONIUGE.Find(pcm.idMaggiorazioneConiuge).PENSIONE.Where(a => a.ANNULLATO == false && a.DATAINIZIO < pcm.dataInizioValidita).Count() > 0 ? true : false;

        }


        public bool EsistonoMovimentiSuccessivi(PensioneConiugeModel pcm, ModelDBISE db)
        {

            if (pcm.dataFineValidita.HasValue)
            {
                return db.MAGGIORAZIONECONIUGE.Find(pcm.idMaggiorazioneConiuge).PENSIONE.Where(a => a.ANNULLATO == false && a.DATAINIZIO > pcm.dataFineValidita).Count() > 0 ? true : false;
            }
            else
            {
                return false;
            }

        }

        private PensioneConiugeModel PrelevaMovimentoAntecedente(PensioneConiugeModel pcm, ModelDBISE db)
        {
            PensioneConiugeModel retPcm = new PensioneConiugeModel();

            var lpc = db.MAGGIORAZIONECONIUGE.Find(pcm.idMaggiorazioneConiuge).PENSIONE.Where(a => a.DATAFINE < pcm.dataInizioValidita).OrderByDescending(a => a.DATAFINE).ToList();

            if (lpc != null && lpc.Count > 0)
            {
                var pc = lpc.First();

                retPcm = new PensioneConiugeModel()
                {
                    idPensioneConiuge = pc.IDPENSIONE,
                    importoPensione = pc.IMPORTOPENSIONE,
                    dataInizioValidita = pc.DATAINIZIO,
                    dataFineValidita = pc.DATAFINE,
                    dataAggiornamento = pc.DATAAGGIORNAMENTO,
                    annullato = pc.ANNULLATO,
                    idMaggiorazioneConiuge = pcm.idMaggiorazioneConiuge

                };


            }

            return retPcm;
        }

        private IList<PensioneConiugeModel> PrelevaMovimentiSuccessivi(PensioneConiugeModel pcm, ModelDBISE db)
        {
            List<PensioneConiugeModel> lPensioniConiuge = new List<PensioneConiugeModel>();

            var lpc = db.MAGGIORAZIONECONIUGE.Find(pcm.idMaggiorazioneConiuge).PENSIONE.Where(a => a.ANNULLATO == false && a.DATAINIZIO > pcm.dataFineValidita).OrderBy(a => a.DATAINIZIO).ToList();

            if (lpc != null && lpc.Count > 0)
            {
                lPensioniConiuge = (from e in lpc
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


            return lPensioniConiuge;
        }

        private IList<PensioneConiugeModel> PrelevaMovimentiInteressati(PensioneConiugeModel pcm, ModelDBISE db)
        {
            List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();

            var lpc = db.MAGGIORAZIONECONIUGE.Find(pcm.idMaggiorazioneConiuge).PENSIONE.Where(a => a.ANNULLATO == false && a.DATAINIZIO >= pcm.dataFineValidita && a.DATAFINE >= pcm.dataInizioValidita).OrderBy(a => a.DATAINIZIO).ToList();
            if (lpc != null && lpc.Count > 0)
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

        public void SetPensioneMaggiorazioneConiuge(ref PensioneConiugeModel pcm, ModelDBISE db)
        {
            try
            {
                var mc = db.MAGGIORAZIONECONIUGE.Find(pcm.idMaggiorazioneConiuge);
                var item = db.Entry<MAGGIORAZIONECONIUGE>(mc);
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

                mc.PENSIONE.Add(pc);

                if (db.SaveChanges() > 0)
                {
                    pcm.idPensioneConiuge = pc.IDPENSIONE;
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
                    DATAFINE = pcm.dataFineValidita.HasValue ? pcm.dataFineValidita.Value : Convert.ToDateTime("31/12/9999"),
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
                DATAFINE = pcm.dataFineValidita.HasValue ? pcm.dataFineValidita.Value : Convert.ToDateTime("31/12/9999"),
                DATAAGGIORNAMENTO = pcm.dataAggiornamento,
                ANNULLATO = pcm.annullato
            };

            db.PENSIONE.Add(p);

            if (db.SaveChanges() > 0)
            {
                pcm.idPensioneConiuge = p.IDPENSIONE;
            }

        }

        public IList<PensioneConiugeModel> GetListaPensioneConiugeByMaggiorazioneConiuge(decimal idMaggiorazioneConiuge)
        {
            List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mc = db.MAGGIORAZIONECONIUGE.Find(idMaggiorazioneConiuge);

                if (mc != null && mc.IDMAGGIORAZIONECONIUGE > 0)
                {
                    var lpc = mc.PENSIONE.Where(a => a.ANNULLATO == false).OrderBy(a => a.DATAINIZIO).ToList();

                    if (lpc != null && lpc.Count > 0)
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

            using (ModelDBISE db = new ModelDBISE())
            {
                var mc = db.MAGGIORAZIONECONIUGE.Find(idMaggiorazioneConiuge);

                if (mc != null && mc.IDMAGGIORAZIONECONIUGE > 0)
                {
                    var lpc = mc.PENSIONE.Where(a => a.ANNULLATO == false && a.DATAINIZIO >= dtIni).OrderBy(a => a.DATAINIZIO).ToList();

                    if (lpc != null && lpc.Count > 0)
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


        public bool HasPensione(decimal idMaggiorazioneConiuge)
        {
            bool ret = false;

            using (ModelDBISE db = new ModelDBISE())
            {
                var mc = db.MAGGIORAZIONECONIUGE.Find(idMaggiorazioneConiuge);

                if (mc != null && mc.IDMAGGIORAZIONECONIUGE > 0)
                {
                    var lpc = mc.PENSIONE.Where(a => a.ANNULLATO == false).ToList();
                    if (lpc != null && lpc.Count > 0)
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

    }
}
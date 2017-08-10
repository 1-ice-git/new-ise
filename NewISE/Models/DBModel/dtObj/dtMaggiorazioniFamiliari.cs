using NewISE.EF;
using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Models.Tools;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtMaggiorazioniFamiliari : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public MaggiorazioniFamiliariModel GetMaggiorazioniFamiliariByID(decimal idMaggiorazioneFamiliari)
        {
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.MAGGIORAZIONEFAMILIARI.Find(idMaggiorazioneFamiliari);

                if (mf != null && mf.IDMAGGIORAZIONEFAMILIARI > 0)
                {
                    mcm = new MaggiorazioniFamiliariModel()
                    {
                        idMaggiorazioneFamiliari = mf.IDMAGGIORAZIONEFAMILIARI,
                        idTrasferimento = mf.IDTRASFERIMENTO,
                        rinunciaMaggiorazioni = mf.RINUNCIAMAGGIORAZIONI,
                        richiestaAttivazione = mf.RICHIESTAATTIVAZIONE,
                        attivazioneMaggiorazioni = mf.ATTIVAZIONEMAGGIOARAZIONI,
                        dataAggiornamento = mf.DATAAGGIORNAMENTO,
                        annullato = mf.ANNULLATO,
                    };
                }
            }

            return mcm;

        }

        public MaggiorazioniFamiliariModel GetMaggiorazioniFamiliariByIDTrasf(decimal idTrasferimento)
        {
            MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lmf =
                    db.MAGGIORAZIONEFAMILIARI.Where(
                        a => a.ANNULLATO == false && a.IDTRASFERIMENTO == idTrasferimento)
                        .OrderByDescending(a => a.DATAAGGIORNAMENTO)
                        .ToList();

                if (lmf?.Any() ?? false)
                {
                    var mf = lmf.First();
                    mfm = new MaggiorazioniFamiliariModel()
                    {
                        idMaggiorazioneFamiliari = mf.IDMAGGIORAZIONEFAMILIARI,
                        idTrasferimento = mf.IDTRASFERIMENTO,
                        rinunciaMaggiorazioni = mf.RINUNCIAMAGGIORAZIONI,
                        richiestaAttivazione = mf.RICHIESTAATTIVAZIONE,
                        attivazioneMaggiorazioni = mf.ATTIVAZIONEMAGGIOARAZIONI,
                        dataAggiornamento = mf.DATAAGGIORNAMENTO,
                        annullato = mf.ANNULLATO,
                    };
                }


            }

            return mfm;

        }

        public void SetMaggiorazioneFamiliari(ref MaggiorazioniFamiliariModel mfm, ModelDBISE db)
        {
            MAGGIORAZIONEFAMILIARI mf = new MAGGIORAZIONEFAMILIARI()
            {
                IDTRASFERIMENTO = mfm.idTrasferimento,
                RINUNCIAMAGGIORAZIONI = mfm.rinunciaMaggiorazioni,
                RICHIESTAATTIVAZIONE = mfm.richiestaAttivazione,
                ATTIVAZIONEMAGGIOARAZIONI = mfm.attivazioneMaggiorazioni,
                DATAAGGIORNAMENTO = mfm.dataAggiornamento,
                ANNULLATO = mfm.annullato
            };

            db.MAGGIORAZIONEFAMILIARI.Add(mf);

            if (db.SaveChanges() > 0)
            {
                mfm.idMaggiorazioneFamiliari = mf.IDMAGGIORAZIONEFAMILIARI;

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento  delle Maggiorazioni familiari", "MAGGIORAZIONEFAMILIARI", db, mfm.idTrasferimento, mf.IDMAGGIORAZIONEFAMILIARI);
            }
        }




        public void InserisciConiuge(ConiugeModel cm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (dtConiuge dtc = new dtConiuge())
                    {
                        cm.dataAggiornamento = DateTime.Now;
                        cm.annullato = false;
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
                                    dtpc.AssociaPercentualeMaggiorazioneConiuge(cm.idConiuge, pmcm.idPercentualeConiuge, db);
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

        public void ModificaConiuge(ConiugeModel cm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();
                try
                {
                    using (dtConiuge dtc = new dtConiuge())
                    {
                        dtc.EditConiuge(cm, db);
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

        //public MaggiorazioniFamiliariModel GetMaggiorazioneFamiliare(decimal idTrasferimento, DateTime dt)
        //{
        //    MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();

        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        var lmf = db.MAGGIORAZIONEFAMILIARI.Where(a => a.ANNULLATO == false && a.PRATICACONCLUSA == false && a.IDTRASFERIMENTO == idTrasferimento).OrderByDescending(a => a.DATACONCLUSIONE).ToList();
        //        if (lmf?.Any() ?? false)
        //        {
        //            var mc = lmf.First();
        //            var lpmg = mc.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();
        //            if (lpmg != null && lpmg.Count > 0)
        //            {
        //                var pmg = lpmg.First();

        //                var lpc = mc.PENSIONE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIO && dt <= a.DATAFINE).OrderByDescending(a => a.DATAINIZIO).ToList();

        //                if (lpc != null && lpc.Count > 0)
        //                {
        //                    //var pc = lpc.First();

        //                    mcm = new MaggiorazioniFamiliariModel()
        //                    {
        //                        idMaggiorazioneConiuge = mc.IDMAGGIORAZIONECONIUGE,
        //                        idTrasferimento = mc.IDTRASFERIMENTO,
        //                        idPercentualeMaggiorazioneConiuge = pmg.IDPERCMAGCONIUGE,
        //                        lPensioneConiuge = (from e in lpc
        //                                            select new PensioneConiugeModel()
        //                                            {
        //                                                idPensioneConiuge = e.IDPENSIONE,
        //                                                importoPensione = e.IMPORTOPENSIONE,
        //                                                dataInizioValidita = e.DATAINIZIO,
        //                                                dataFineValidita = e.DATAFINE,
        //                                                dataAggiornamento = e.DATAAGGIORNAMENTO,
        //                                                annullato = e.ANNULLATO
        //                                            }).ToList(),
        //                        dataInizioValidita = mc.DATAINIZIOVALIDITA,
        //                        dataFineValidita = mc.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : mc.DATAFINEVALIDITA,
        //                        dataAggiornamento = mc.DATAAGGIORNAMENTO,
        //                        annullato = mc.ANNULLATO,
        //                    };
        //                }
        //                else
        //                {
        //                    mcm = new MaggiorazioniFamiliariModel()
        //                    {
        //                        idMaggiorazioneConiuge = mc.IDMAGGIORAZIONECONIUGE,
        //                        idTrasferimento = mc.IDTRASFERIMENTO,
        //                        idPercentualeMaggiorazioneConiuge = pmg.IDPERCMAGCONIUGE,
        //                        dataInizioValidita = mc.DATAINIZIOVALIDITA,
        //                        dataFineValidita = mc.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : mc.DATAFINEVALIDITA,
        //                        dataAggiornamento = mc.DATAAGGIORNAMENTO,
        //                        annullato = mc.ANNULLATO,
        //                    };
        //                }
        //            }
        //        }
        //    }

        //    return mcm;
        //}




    }
}
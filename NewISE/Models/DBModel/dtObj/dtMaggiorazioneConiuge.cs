using NewISE.EF;
using NewISE.Models.ViewModel;
using System;
using System.Linq;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtMaggiorazioneConiuge : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public MaggiorazioneConiugeModel GetMaggiorazioneConiuge(decimal idMaggiorazioneConiuge)
        {
            MaggiorazioneConiugeModel mcm = new MaggiorazioneConiugeModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mc = db.MAGGIORAZIONECONIUGE.Find(idMaggiorazioneConiuge);

                if (mc != null && mc.IDMAGGIORAZIONECONIUGE > 0)
                {
                    mcm = new MaggiorazioneConiugeModel()
                    {
                        idMaggiorazioneConiuge = mc.IDMAGGIORAZIONECONIUGE,
                        idTrasferimento = mc.IDTRASFERIMENTO,
                        dataInizioValidita = mc.DATAINIZIOVALIDITA,
                        dataFineValidita = mc.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : mc.DATAFINEVALIDITA,
                        dataAggiornamento = mc.DATAAGGIORNAMENTO,
                        annullato = mc.ANNULLATO,
                    };
                }
            }

            return mcm;

        }

        public MaggiorazioneConiugeModel GetMaggiorazioneConiuge(decimal idTrasferimento, DateTime dt)
        {
            MaggiorazioneConiugeModel mcm = new MaggiorazioneConiugeModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lmc = db.MAGGIORAZIONECONIUGE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA && a.IDTRASFERIMENTO == idTrasferimento).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();
                if (lmc != null && lmc.Count > 0)
                {
                    var mc = lmc.First();
                    var lpmg = mc.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();
                    if (lpmg != null && lpmg.Count > 0)
                    {
                        var pmg = lpmg.First();

                        var lpc = mc.PENSIONE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIO && dt <= a.DATAFINE).OrderByDescending(a => a.DATAINIZIO).ToList();

                        if (lpc != null && lpc.Count > 0)
                        {
                            //var pc = lpc.First();

                            mcm = new MaggiorazioneConiugeModel()
                            {
                                idMaggiorazioneConiuge = mc.IDMAGGIORAZIONECONIUGE,
                                idTrasferimento = mc.IDTRASFERIMENTO,
                                idPercentualeMaggiorazioneConiuge = pmg.IDPERCMAGCONIUGE,
                                lPensioneConiuge = (from e in lpc
                                                    select new PensioneConiugeModel()
                                                    {
                                                        idPensioneConiuge = e.IDPENSIONE,
                                                        importoPensione = e.IMPORTOPENSIONE,
                                                        dataInizioValidita = e.DATAINIZIO,
                                                        dataFineValidita = e.DATAFINE,
                                                        dataAggiornamento = e.DATAAGGIORNAMENTO,
                                                        annullato = e.ANNULLATO
                                                    }).ToList(),
                                dataInizioValidita = mc.DATAINIZIOVALIDITA,
                                dataFineValidita = mc.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : mc.DATAFINEVALIDITA,
                                dataAggiornamento = mc.DATAAGGIORNAMENTO,
                                annullato = mc.ANNULLATO,
                            };
                        }
                        else
                        {
                            mcm = new MaggiorazioneConiugeModel()
                            {
                                idMaggiorazioneConiuge = mc.IDMAGGIORAZIONECONIUGE,
                                idTrasferimento = mc.IDTRASFERIMENTO,
                                idPercentualeMaggiorazioneConiuge = pmg.IDPERCMAGCONIUGE,
                                dataInizioValidita = mc.DATAINIZIOVALIDITA,
                                dataFineValidita = mc.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : mc.DATAFINEVALIDITA,
                                dataAggiornamento = mc.DATAAGGIORNAMENTO,
                                annullato = mc.ANNULLATO,
                            };
                        }
                    }
                }
            }

            return mcm;
        }

        public void InserisciConiuge(MaggiorazioneConiugeVModel mcvm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    MaggiorazioneConiugeModel mcm = new MaggiorazioneConiugeModel();

                    mcm.idTrasferimento = mcvm.idTrasferimento;
                    mcm.dataInizioValidita = mcvm.dataInizioValidita.Value;
                    mcm.dataFineValidita = mcvm.dataFineValidita;
                    mcm.dataAggiornamento = DateTime.Now;
                    mcm.annullato = false;

                    SetMaggiorazioneConiuge(ref mcm, db);

                    mcvm.idMaggiorazioneConiuge = mcm.idMaggiorazioneConiuge;
                    mcvm.dataFineValidita = mcm.dataFineValidita;

                    ConiugeModel cm = new ConiugeModel()
                    {
                        idMaggiorazioneConiuge = mcvm.idMaggiorazioneConiuge,
                        nome = mcvm.nome,
                        cognome = mcvm.cognome,
                        codiceFiscale = mcvm.codiceFiscale
                    };
                    using (dtConiuge dtc = new dtConiuge())
                    {
                        dtc.SetConiuge(cm, db);

                        using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                        {
                            PercentualeMagConiugeModel pmcm = dtpc.GetPercentualeMaggiorazioneConiuge(mcvm.idTipologiaConiuge, mcvm.dataInizioValidita.Value, db);

                            if (pmcm != null && pmcm.HasValue())
                            {
                                dtpc.AssociaPercentualeMaggiorazioneConiuge(mcvm.idMaggiorazioneConiuge, pmcm.idPercentualeConiuge, db);
                                ///Verifico la presenza di ulteriori percentuali di maggiorazione coniuge con data successiva a quella prelevata.
                                ///Se la data di fine validità è minore del 31/12/9999 si presume che ci siano ulteriori percentuali.
                                if (pmcm.dataFineValidita.HasValue && pmcm.dataFineValidita.Value < Convert.ToDateTime("31/12/9999"))
                                {
                                    ///Prelevo le percentuali passando come range di date la data di fine più un giorno della data di fine della percentuale precedentemente prelevata
                                    ///e la data di fine della maggiorazione coniuge
                                    var lpmcm = dtpc.GetListaPercentualiMagConiugeByRangeDate(mcvm.idTipologiaConiuge, pmcm.dataFineValidita.Value.AddDays(1), mcvm.dataFineValidita.Value, db);
                                    if (lpmcm != null && lpmcm.Count > 0)
                                    {
                                        foreach (var e in lpmcm)
                                        {
                                            dtpc.AssociaPercentualeMaggiorazioneConiuge(mcvm.idMaggiorazioneConiuge, e.idPercentualeConiuge, db);
                                        }
                                    }
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

        public void SetMaggiorazioneConiuge(ref MaggiorazioneConiugeModel mcm, ModelDBISE db)
        {
            MAGGIORAZIONECONIUGE mc = new MAGGIORAZIONECONIUGE()
            {
                IDTRASFERIMENTO = mcm.idTrasferimento,
                DATAINIZIOVALIDITA = mcm.dataInizioValidita,
                DATAFINEVALIDITA = mcm.dataFineValidita.HasValue ? mcm.dataFineValidita.Value : Convert.ToDateTime("31/12/9999"),
                DATAAGGIORNAMENTO = mcm.dataAggiornamento,
                ANNULLATO = mcm.annullato,
            };

            db.MAGGIORAZIONECONIUGE.Add(mc);

            if (db.SaveChanges() > 0)
            {
                mcm.idMaggiorazioneConiuge = mc.IDMAGGIORAZIONECONIUGE;
                mcm.dataFineValidita = mc.DATAFINEVALIDITA;
            }
        }
    }
}
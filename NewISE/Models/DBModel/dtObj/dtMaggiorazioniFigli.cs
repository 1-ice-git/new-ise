using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.Models.ViewModel;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtMaggiorazioniFigli : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public void IserisciFiglio(Figli_V_Model fvm, decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    if (fvm.idMaggiorazioneFigli > 0)
                    {
                        using (dtFigli dtf = new dtFigli())
                        {
                            FigliModel fm = new FigliModel()
                            {
                                idMaggiorazioneFigli = fvm.idMaggiorazioneFigli,
                                nome = fvm.nome,
                                cognome = fvm.cognome,
                                codiceFiscale = fvm.codiceFiscale,
                                dataInizio = fvm.dataInizio,
                                dataFine = fvm.dataFine,
                                dataAggiornamento = DateTime.Now,
                                Annullato = false,
                            };

                            dtf.SetFiglio(ref fm, db);

                            using (dtPercentualeMagFigli dtpmf = new dtPercentualeMagFigli())
                            {
                                List<PercentualeMagFigliModel> lpfm = new List<PercentualeMagFigliModel>();
                                DateTime dtFin = fvm.dataFine.HasValue
                                    ? fvm.dataFine.Value
                                    : Convert.ToDateTime("31/12/9999");


                                lpfm =
                                    dtpmf.GetPercentualeMaggiorazioneFigli(fvm.idTipologiaFiglio, fvm.dataInizio.Value,
                                        dtFin, db).ToList();

                                if (lpfm?.Any() ?? false)
                                {
                                    foreach (var pfm in lpfm)
                                    {
                                        dtpmf.AssociaPercentualeMaggiorazioneFigli(fm.idFigli, pfm.idPercMagFigli, db);
                                    }
                                }
                            }

                            using (dtIndennitaPrimoSegretario dtips = new dtIndennitaPrimoSegretario())
                            {
                                List<IndennitaPrimoSegretModel> lipsm = new List<IndennitaPrimoSegretModel>();
                                DateTime dtFin = fvm.dataFine.HasValue
                                    ? fvm.dataFine.Value
                                    : Convert.ToDateTime("31/12/9999");

                                lipsm = dtips.GetIndennitaPrimoSegretario(fvm.dataInizio.Value, dtFin, db).ToList();

                                if (lipsm?.Any() ?? false)
                                {
                                    foreach (var ipsm in lipsm)
                                    {
                                        dtips.AssociaIndennitaPrimoSegretarioFiglio(fm.idFigli, ipsm.idIndPrimoSegr, db);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        MaggiorazioniFigliModel mfm = new MaggiorazioniFigliModel()
                        {
                            idTrasferimento = idTrasferimento,
                            dataAggiornamento = DateTime.Now,
                            annullato = false,
                        };

                        this.SetMaggiorazioneFigli(ref mfm, db);

                        using (dtFigli dtf = new dtFigli())
                        {
                            FigliModel fm = new FigliModel()
                            {
                                idMaggiorazioneFigli = mfm.idMaggiorazioneFigli,
                                nome = fvm.nome,
                                cognome = fvm.cognome,
                                codiceFiscale = fvm.codiceFiscale,
                                dataInizio = fvm.dataInizio,
                                dataFine = fvm.dataFine,
                                dataAggiornamento = mfm.dataAggiornamento,
                                Annullato = mfm.annullato,
                            };

                            dtf.SetFiglio(ref fm, db);

                            using (dtPercentualeMagFigli dtpmf = new dtPercentualeMagFigli())
                            {
                                List<PercentualeMagFigliModel> lpfm = new List<PercentualeMagFigliModel>();
                                DateTime dtFin = fvm.dataFine.HasValue
                                    ? fvm.dataFine.Value
                                    : Convert.ToDateTime("31/12/9999");


                                lpfm =
                                    dtpmf.GetPercentualeMaggiorazioneFigli(fvm.idTipologiaFiglio, fvm.dataInizio.Value,
                                        dtFin, db).ToList();
                                if (lpfm?.Any() ?? false)
                                {
                                    foreach (var pfm in lpfm)
                                    {
                                        dtpmf.AssociaPercentualeMaggiorazioneFigli(fm.idFigli, pfm.idPercMagFigli, db);
                                    }
                                }
                            }

                            using (dtIndennitaPrimoSegretario dtips = new dtIndennitaPrimoSegretario())
                            {
                                List<IndennitaPrimoSegretModel> lipsm = new List<IndennitaPrimoSegretModel>();
                                DateTime dtFin = fvm.dataFine.HasValue
                                    ? fvm.dataFine.Value
                                    : Convert.ToDateTime("31/12/9999");

                                lipsm = dtips.GetIndennitaPrimoSegretario(fvm.dataInizio.Value, dtFin, db).ToList();

                                if (lipsm?.Any() ?? false)
                                {
                                    foreach (var ipsm in lipsm)
                                    {
                                        dtips.AssociaIndennitaPrimoSegretarioFiglio(fm.idFigli, ipsm.idIndPrimoSegr, db);
                                    }
                                }
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

        public void SetMaggiorazioneFigli(ref MaggiorazioniFigliModel mfm, ModelDBISE db)
        {
            MAGGIORAZIONEFIGLI mf = new MAGGIORAZIONEFIGLI()
            {
                IDTRASFERIMENTO = mfm.idTrasferimento,
                DATAAGGIORNAMENTO = mfm.dataAggiornamento,
                ANNULLATO = mfm.annullato,
            };

            db.MAGGIORAZIONEFIGLI.Add(mf);

            int i = db.SaveChanges();

            if (i > 0)
            {
                mfm.idMaggiorazioneFigli = mf.IDMAGGIORAZIONEFIGLI;
            }
            else
            {
                throw new Exception("Non è stato possibile inserire la maggiorazione per i figli.");
            }
        }

        public MaggiorazioniFigliModel GetMaggiorazioneFigli(decimal idFiglio)
        {
            MaggiorazioniFigliModel mfm = new MaggiorazioniFigliModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var f = db.FIGLI.Find(idFiglio);
                if (f != null && f.IDFIGLI > 0)
                {
                    var mf = f.MAGGIORAZIONEFIGLI;

                    mfm = new MaggiorazioniFigliModel()
                    {
                        idMaggiorazioneFigli = mf.IDMAGGIORAZIONEFIGLI,
                        idTrasferimento = mf.IDTRASFERIMENTO,
                        dataAggiornamento = mf.DATAAGGIORNAMENTO,
                        annullato = mf.ANNULLATO,
                    };
                }
            }

            return mfm;
        }


        public MaggiorazioniFigliModel GetMaggiorazioneFigli(decimal idTrasferimento, DateTime dt)
        {
            MaggiorazioniFigliModel mfm = new MaggiorazioniFigliModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lmf =
                    db.MAGGIORAZIONEFIGLI.Where(a => a.ANNULLATO == false && a.IDTRASFERIMENTO == idTrasferimento)
                        .OrderByDescending(a => a.DATAAGGIORNAMENTO)
                        .ToList();
                if (lmf != null && lmf.Count > 0)
                {
                    var mf = lmf.First();
                    mfm = new MaggiorazioniFigliModel()
                    {
                        idMaggiorazioneFigli = mf.IDMAGGIORAZIONEFIGLI,
                        idTrasferimento = mf.IDTRASFERIMENTO,
                        dataAggiornamento = mf.DATAAGGIORNAMENTO,
                        annullato = mf.ANNULLATO,
                    };
                }
            }

            return mfm;
        }
    }
}
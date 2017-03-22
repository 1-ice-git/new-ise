using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtIndennitaBase : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<IndennitaBaseModel> getListIndennitaBase()
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.INDENNITABASE.ToList();

                    libm = (from e in lib
                            select new IndennitaBaseModel()
                            {
                                idIndennitaBase = e.IDINDENNITABASE,
                                idLivello = e.IDLIVELLO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaBaseModel().dataFineValidita,
                                valore = e.VALORE,
                                valoreResponsabile = e.VALORERESP,
                                annullato = e.ANNULLATO,
                                Livello = new LivelloModel()
                                {
                                    idLivello = e.LIVELLI.IDLIVELLO,
                                    DescLivello = e.LIVELLI.LIVELLO
                                }
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<IndennitaBaseModel> getListIndennitaBase(decimal idLivello)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.INDENNITABASE.Where(a => a.IDLIVELLO == idLivello).ToList();

                    libm = (from e in lib
                            select new IndennitaBaseModel()
                            {
                                idIndennitaBase = e.IDINDENNITABASE,
                                idLivello = e.IDLIVELLO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaBaseModel().dataFineValidita,
                                valore = e.VALORE,
                                valoreResponsabile = e.VALORERESP,
                                annullato = e.ANNULLATO,
                                Livello = new LivelloModel()
                                {
                                    idLivello = e.LIVELLI.IDLIVELLO,
                                    DescLivello = e.LIVELLI.LIVELLO
                                }
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<IndennitaBaseModel> getListIndennitaBase(bool escludiAnnullati = false)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.INDENNITABASE.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new IndennitaBaseModel()
                            {
                                idIndennitaBase = e.IDINDENNITABASE,
                                idLivello = e.IDLIVELLO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaBaseModel().dataFineValidita,
                                valore = e.VALORE,
                                valoreResponsabile = e.VALORERESP,
                                annullato = e.ANNULLATO,
                                Livello = new LivelloModel()
                                {
                                    idLivello = e.LIVELLI.IDLIVELLO,
                                    DescLivello = e.LIVELLI.LIVELLO
                                }
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<IndennitaBaseModel> getListIndennitaBase(decimal idLivello, bool escludiAnnullati = false)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.INDENNITABASE.Where(a => a.IDLIVELLO == idLivello && a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new IndennitaBaseModel()
                            {
                                idIndennitaBase = e.IDINDENNITABASE,
                                idLivello = e.IDLIVELLO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaBaseModel().dataFineValidita,
                                valore = e.VALORE,
                                valoreResponsabile = e.VALORERESP,
                                annullato = e.ANNULLATO,
                                Livello = new LivelloModel()
                                {
                                    idLivello = e.LIVELLI.IDLIVELLO,
                                    DescLivello = e.LIVELLI.LIVELLO
                                }
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetIndennitaDiBase(IndennitaBaseModel ibm)
        {
            List<INDENNITABASE> libNew = new List<INDENNITABASE>();

            INDENNITABASE ibNew = new INDENNITABASE();

            INDENNITABASE ibPrecedente = new INDENNITABASE();

            List<INDENNITABASE> lArchivioIB = new List<INDENNITABASE>();

            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                try
                {
                    if (ibm.dataFineValidita.HasValue)
                    {
                        if (EsistonoMovimentiSuccessivi(ibm))
                        {
                            ibNew = new INDENNITABASE()
                            {
                                IDLIVELLO = ibm.idLivello,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                VALORE = ibm.valore,
                                VALORERESP = ibm.valoreResponsabile,
                                ANNULLATO = ibm.annullato
                            };
                        }
                        else
                        {
                            ibNew = new INDENNITABASE()
                            {
                                IDLIVELLO = ibm.idLivello,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                                VALORE = ibm.valore,
                                VALORERESP = ibm.valoreResponsabile,
                                ANNULLATO = ibm.annullato
                            };
                        }
                    }
                    else
                    {
                        ibNew = new INDENNITABASE()
                        {
                            IDLIVELLO = ibm.idLivello,
                            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                            VALORE = ibm.valore,
                            VALORERESP = ibm.valoreResponsabile,
                            ANNULLATO = ibm.annullato
                        };
                    }

                    db.Database.BeginTransaction();

                    var recordInteressati = db.INDENNITABASE.Where(a => a.ANNULLATO == false && a.IDLIVELLO == ibNew.IDLIVELLO)
                                                            .Where(a => a.DATAINIZIOVALIDITA >= ibNew.DATAINIZIOVALIDITA || a.DATAFINEVALIDITA >= ibNew.DATAINIZIOVALIDITA)
                                                            .Where(a => a.DATAINIZIOVALIDITA <= ibNew.DATAFINEVALIDITA || a.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
                                                            .ToList();

                    recordInteressati.ForEach(a => a.ANNULLATO = true);
                    db.SaveChanges();

                    if (recordInteressati.Count > 0)
                    {
                        foreach (var item in recordInteressati)
                        {
                            INDENNITABASE ib = new INDENNITABASE()
                            {
                                IDINDENNITABASE = item.IDINDENNITABASE,
                                IDLIVELLO = item.IDLIVELLO,
                                ANNULLATO = false,
                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                VALORE = item.VALORE,
                                VALORERESP = item.VALORERESP,
                            };

                            libNew.Add(ib);
                        }

                        libNew.Add(ibNew);
                        

                        libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).Distinct().ToList();

                        for (int i = 0; i < libNew.Count; i++)
                        {
                            INDENNITABASE ib = libNew[i];
                            if ((i + 1) < libNew.Count)
                            {
                                ib.DATAFINEVALIDITA = libNew[i + 1].DATAINIZIOVALIDITA.AddDays(-1);
                            }

                            db.INDENNITABASE.Add(ib);
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        db.INDENNITABASE.Add(ibNew);
                        db.SaveChanges();
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

        public bool EsistonoMovimentiSuccessivi(IndennitaBaseModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.INDENNITABASE.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiPrima(IndennitaBaseModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                return db.INDENNITABASE.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita).Count() > 0 ? true : false;
            }
        }

        public void DelIndennitaDiBase(decimal idIndbase)
        {
            INDENNITABASE precedenteIB = new INDENNITABASE();
            INDENNITABASE successivaIB = new INDENNITABASE();
            INDENNITABASE delIB = new INDENNITABASE();

            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                var lib = db.INDENNITABASE.Where(a => a.IDINDENNITABASE == idIndbase);

                if (lib.Count() > 0)
                {
                    delIB = lib.First();

                    //precedenteIB = db.INDENNITABASE.Where(a=>a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.DATAFINEVALIDITA == db.INDENNITABASE)

                    db.INDENNITABASE.Remove(db.INDENNITABASE.Find(idIndbase));
                    db.SaveChanges();
                }
            }
        }
    }
}
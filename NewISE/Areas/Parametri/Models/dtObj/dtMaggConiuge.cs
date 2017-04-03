using NewISE.Models.dtObj.objB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtMaggConiuge : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public IList<MaggiorazioneConiugeModel> getListMaggiorazioneConiuge()
        {
            List<MaggiorazioneConiugeModel> libm = new List<MaggiorazioneConiugeModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.MAGGIORAZIONECONIUGE.ToList();

                    libm = (from e in lib
                            select new MaggiorazioneConiugeModel()
                            {
                                
                                idMaggiorazioneConiuge = e.IDMAGGIORAZIONECONIUGE,
                                idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new MaggiorazioneConiugeModel().dataFineValidita,
                                percentualeConiuge = e.PERCENTUALECONIUGE,
                                annullato = e.ANNULLATO,
                                Coniuge = new TipologiaConiugeModel()
                                {
                                    idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                                    tipologiaConiuge = e.TIPOLOGIACONIUGE.ToString()
                                   
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

        public IList<MaggiorazioneConiugeModel> getListMaggiorazioneConiuge(decimal idTipologiaConiuge)
        {
            List<MaggiorazioneConiugeModel> libm = new List<MaggiorazioneConiugeModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.MAGGIORAZIONECONIUGE.Where(a => a.IDMAGGIORAZIONECONIUGE == idTipologiaConiuge).ToList();

                    libm = (from e in lib
                            select new MaggiorazioneConiugeModel()
                            {
                                
                                idMaggiorazioneConiuge = e.IDMAGGIORAZIONECONIUGE,
                                idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new MaggiorazioneConiugeModel().dataFineValidita,
                                percentualeConiuge = e.PERCENTUALECONIUGE,
                                annullato = e.ANNULLATO,
                                Coniuge = new TipologiaConiugeModel()
                                {
                                    idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                                    tipologiaConiuge = e.TIPOLOGIACONIUGE.ToString()

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

        public IList<MaggiorazioneConiugeModel> getListMaggiorazioneConiuge(bool escludiAnnullati = false)
        {
            List<MaggiorazioneConiugeModel> libm = new List<MaggiorazioneConiugeModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.MAGGIORAZIONECONIUGE.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new MaggiorazioneConiugeModel()
                            {
                                
                                idMaggiorazioneConiuge = e.IDMAGGIORAZIONECONIUGE,
                                idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new MaggiorazioneConiugeModel().dataFineValidita,
                                percentualeConiuge = e.PERCENTUALECONIUGE,
                                annullato = e.ANNULLATO,
                                Coniuge = new TipologiaConiugeModel()
                                {
                                    idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                                    tipologiaConiuge = e.TIPOLOGIACONIUGE.ToString()

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

        public IList<MaggiorazioneConiugeModel> getListMaggiorazioneConiuge(decimal idTipologiaConiuge, bool escludiAnnullati = false)
        {
            List<MaggiorazioneConiugeModel> libm = new List<MaggiorazioneConiugeModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.MAGGIORAZIONECONIUGE.Where(a => a.IDTIPOLOGIACONIUGE == idTipologiaConiuge && a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new MaggiorazioneConiugeModel()
                            {
                                
                                idMaggiorazioneConiuge = e.IDMAGGIORAZIONECONIUGE,
                                idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new MaggiorazioneConiugeModel().dataFineValidita,
                                percentualeConiuge = e.PERCENTUALECONIUGE,
                                annullato = e.ANNULLATO,
                                Coniuge = new TipologiaConiugeModel()
                                {
                                    idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                                    tipologiaConiuge = e.TIPOLOGIACONIUGE.ToString()

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ibm"></param>
        public void SetMaggiorazioneConiuge(MaggiorazioneConiugeModel ibm)
        {
            List<MAGGIORAZIONECONIUGE> libNew = new List<MAGGIORAZIONECONIUGE>();

            MAGGIORAZIONECONIUGE ibNew = new MAGGIORAZIONECONIUGE();

            MAGGIORAZIONECONIUGE ibPrecedente = new MAGGIORAZIONECONIUGE();

            List<MAGGIORAZIONECONIUGE> lArchivioIB = new List<MAGGIORAZIONECONIUGE>();

            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                try
                {
                    if (ibm.dataFineValidita.HasValue)
                    {
                        if (EsistonoMovimentiSuccessiviUguale(ibm))
                        {
                            ibNew = new MAGGIORAZIONECONIUGE()
                            {
                                
                                IDTIPOLOGIACONIUGE = ibm.idTipologiaConiuge,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                PERCENTUALECONIUGE = ibm.percentualeConiuge,
                                ANNULLATO = ibm.annullato
                            };
                        }
                        else
                        {
                            ibNew = new MAGGIORAZIONECONIUGE()
                            {
                                IDTIPOLOGIACONIUGE = ibm.idTipologiaConiuge,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                                PERCENTUALECONIUGE = ibm.percentualeConiuge,
                                ANNULLATO = ibm.annullato
                            };
                        }
                    }
                    else
                    {
                        ibNew = new MAGGIORAZIONECONIUGE()
                        {
                            
                            IDTIPOLOGIACONIUGE = ibm.idTipologiaConiuge,
                            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                            PERCENTUALECONIUGE = ibm.percentualeConiuge,
                            ANNULLATO = ibm.annullato
                        };
                    }

                    db.Database.BeginTransaction();

                    var recordInteressati = db.MAGGIORAZIONECONIUGE.Where(a => a.ANNULLATO == false && a.IDTIPOLOGIACONIUGE == ibNew.IDTIPOLOGIACONIUGE)
                                                            .Where(a => a.DATAINIZIOVALIDITA >= ibNew.DATAINIZIOVALIDITA || a.DATAFINEVALIDITA >= ibNew.DATAINIZIOVALIDITA)
                                                            .Where(a => a.DATAINIZIOVALIDITA <= ibNew.DATAFINEVALIDITA || a.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
                                                            .ToList();

                    recordInteressati.ForEach(a => a.ANNULLATO = true);
                    //db.SaveChanges();

                    if (recordInteressati.Count > 0)
                    {
                        foreach (var item in recordInteressati)
                        {

                            if (item.DATAINIZIOVALIDITA < ibNew.DATAINIZIOVALIDITA)
                            {
                                if (item.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new MAGGIORAZIONECONIUGE()
                                    {
                                        IDTIPOLOGIACONIUGE = ibm.idTipologiaConiuge,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        PERCENTUALECONIUGE = item.PERCENTUALECONIUGE,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);

                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new MAGGIORAZIONECONIUGE()
                                    {
                                        
                                        IDTIPOLOGIACONIUGE = item.IDTIPOLOGIACONIUGE,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        PERCENTUALECONIUGE = item.PERCENTUALECONIUGE,
                                        ANNULLATO = false
                                    };

                                    var ibOld2 = new MAGGIORAZIONECONIUGE()
                                    {
                                        
                                        IDTIPOLOGIACONIUGE = item.IDTIPOLOGIACONIUGE,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(+1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        PERCENTUALECONIUGE = item.PERCENTUALECONIUGE,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                    libNew.Add(ibOld2);

                                }

                            }
                            else if (item.DATAINIZIOVALIDITA == ibNew.DATAINIZIOVALIDITA)
                            {
                                if (item.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
                                {
                                    //Non preleva il record old
                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new MAGGIORAZIONECONIUGE()
                                    {
                                        
                                        IDTIPOLOGIACONIUGE = item.IDTIPOLOGIACONIUGE,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        PERCENTUALECONIUGE = item.PERCENTUALECONIUGE,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                }
                            }
                            else if (item.DATAINIZIOVALIDITA > ibNew.DATAINIZIOVALIDITA)
                            {
                                if (item.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
                                {
                                    //Non preleva il record old
                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new MAGGIORAZIONECONIUGE()
                                    {
                                        
                                        IDTIPOLOGIACONIUGE = item.IDTIPOLOGIACONIUGE,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        PERCENTUALECONIUGE = item.PERCENTUALECONIUGE,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                }
                            }
                        }

                        libNew.Add(ibNew);
                        libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        db.MAGGIORAZIONECONIUGE.AddRange(libNew);
                    }
                    else
                    {
                        db.MAGGIORAZIONECONIUGE.Add(ibNew);

                    }
                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro di maggiorazione coniuge.", "MAGGIORAZIONECONIUGE", ibNew.IDMAGGIORAZIONECONIUGE);
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

        public bool EsistonoMovimentiPrima(MaggiorazioneConiugeModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                return db.MAGGIORAZIONECONIUGE.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDTIPOLOGIACONIUGE == ibm.idTipologiaConiuge).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(MaggiorazioneConiugeModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.MAGGIORAZIONECONIUGE.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDTIPOLOGIACONIUGE == ibm.idTipologiaConiuge).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(MaggiorazioneConiugeModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.MAGGIORAZIONECONIUGE.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDTIPOLOGIACONIUGE == ibm.idTipologiaConiuge).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }



        public bool EsistonoMovimentiPrimaUguale(MaggiorazioneConiugeModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                return db.MAGGIORAZIONECONIUGE.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDTIPOLOGIACONIUGE == ibm.idTipologiaConiuge).Count() > 0 ? true : false;
            }
        }

        public void DelMaggiorazioneConiuge(decimal idMaggConiuge)
        {
            MAGGIORAZIONECONIUGE precedenteIB = new MAGGIORAZIONECONIUGE();
            MAGGIORAZIONECONIUGE delIB = new MAGGIORAZIONECONIUGE();


            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.MAGGIORAZIONECONIUGE.Where(a => a.IDMAGGIORAZIONECONIUGE == idMaggConiuge);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.MAGGIORAZIONECONIUGE.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new MAGGIORAZIONECONIUGE()
                            {
                                
                                IDTIPOLOGIACONIUGE = precedenteIB.IDTIPOLOGIACONIUGE,
                                DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                PERCENTUALECONIUGE = precedenteIB.PERCENTUALECONIUGE,
                                ANNULLATO = false
                            };

                            db.MAGGIORAZIONECONIUGE.Add(ibOld1);
                        }

                        db.SaveChanges();

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di maggiorazione coniuge.", "MAGGIORAZIONECONIUGE", idMaggConiuge);
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
    }
}
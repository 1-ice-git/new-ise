using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParMaggConiuge : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public IList<PercentualeMagConiugeModel> getListPercMagConiuge()
        {
            List<PercentualeMagConiugeModel> libm = new List<PercentualeMagConiugeModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEMAGCONIUGE.ToList();

                    libm = (from e in lib
                            select new PercentualeMagConiugeModel()
                            {

                                idPercentualeConiuge = e.IDPERCMAGCONIUGE,
                                idTipologiaConiuge = (TipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new PercentualeMagConiugeModel().dataFineValidita,
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

        public IList<PercentualeMagConiugeModel> getListPercMagConiuge(decimal idTipologiaConiuge)
        {
            List<PercentualeMagConiugeModel> libm = new List<PercentualeMagConiugeModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEMAGCONIUGE.Where(a => a.IDTIPOLOGIACONIUGE == idTipologiaConiuge).ToList();

                    libm = (from e in lib
                            select new PercentualeMagConiugeModel()
                            {

                                idPercentualeConiuge = e.IDPERCMAGCONIUGE,
                                idTipologiaConiuge = (TipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new PercentualeMagConiugeModel().dataFineValidita,
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

        public IList<PercentualeMagConiugeModel> getListPercMagConiuge(bool escludiAnnullati = false)
        {
            List<PercentualeMagConiugeModel> libm = new List<PercentualeMagConiugeModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new PercentualeMagConiugeModel()
                            {

                                idPercentualeConiuge = e.IDPERCMAGCONIUGE,
                                idTipologiaConiuge = (TipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new PercentualeMagConiugeModel().dataFineValidita,
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

        public IList<PercentualeMagConiugeModel> getListPercMagConiuge(decimal idTipologiaConiuge, bool escludiAnnullati = false)
        {
            List<PercentualeMagConiugeModel> libm = new List<PercentualeMagConiugeModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEMAGCONIUGE.Where(a => a.IDTIPOLOGIACONIUGE == idTipologiaConiuge && a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new PercentualeMagConiugeModel()
                            {

                                idPercentualeConiuge = e.IDPERCMAGCONIUGE,
                                idTipologiaConiuge = (TipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new PercentualeMagConiugeModel().dataFineValidita,
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
        public void SetPercMagConiuge(PercentualeMagConiugeModel ibm)
        {
            List<PERCENTUALEMAGCONIUGE> libNew = new List<PERCENTUALEMAGCONIUGE>();

            PERCENTUALEMAGCONIUGE ibNew = new PERCENTUALEMAGCONIUGE();

            PERCENTUALEMAGCONIUGE ibPrecedente = new PERCENTUALEMAGCONIUGE();

            List<PERCENTUALEMAGCONIUGE> lArchivioIB = new List<PERCENTUALEMAGCONIUGE>();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    if (ibm.dataFineValidita.HasValue)
                    {
                        if (EsistonoMovimentiSuccessiviUguale(ibm))
                        {
                            ibNew = new PERCENTUALEMAGCONIUGE()
                            {

                                IDTIPOLOGIACONIUGE = (decimal)ibm.idTipologiaConiuge,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                PERCENTUALECONIUGE = ibm.percentualeConiuge,
                                ANNULLATO = ibm.annullato
                            };
                        }
                        else
                        {
                            ibNew = new PERCENTUALEMAGCONIUGE()
                            {
                                IDTIPOLOGIACONIUGE = (decimal)ibm.idTipologiaConiuge,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                                PERCENTUALECONIUGE = ibm.percentualeConiuge,
                                ANNULLATO = ibm.annullato
                            };
                        }
                    }
                    else
                    {
                        ibNew = new PERCENTUALEMAGCONIUGE()
                        {

                            IDTIPOLOGIACONIUGE = (decimal)ibm.idTipologiaConiuge,
                            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                            PERCENTUALECONIUGE = ibm.percentualeConiuge,
                            ANNULLATO = ibm.annullato
                        };
                    }

                    db.Database.BeginTransaction();

                    var recordInteressati = db.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false && a.IDTIPOLOGIACONIUGE == ibNew.IDTIPOLOGIACONIUGE)
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
                                    var ibOld1 = new PERCENTUALEMAGCONIUGE()
                                    {
                                        IDTIPOLOGIACONIUGE = (decimal)ibm.idTipologiaConiuge,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        PERCENTUALECONIUGE = item.PERCENTUALECONIUGE,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);

                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new PERCENTUALEMAGCONIUGE()
                                    {

                                        IDTIPOLOGIACONIUGE = item.IDTIPOLOGIACONIUGE,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        PERCENTUALECONIUGE = item.PERCENTUALECONIUGE,
                                        ANNULLATO = false
                                    };

                                    var ibOld2 = new PERCENTUALEMAGCONIUGE()
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
                                    var ibOld1 = new PERCENTUALEMAGCONIUGE()
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
                                    var ibOld1 = new PERCENTUALEMAGCONIUGE()
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

                        db.PERCENTUALEMAGCONIUGE.AddRange(libNew);
                    }
                    else
                    {
                        db.PERCENTUALEMAGCONIUGE.Add(ibNew);

                    }
                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro di maggiorazione coniuge.", "MAGGIORAZIONECONIUGE", ibNew.IDPERCMAGCONIUGE);
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

        public bool EsistonoMovimentiPrima(PercentualeMagConiugeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEMAGCONIUGE.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDTIPOLOGIACONIUGE == (decimal)ibm.idTipologiaConiuge).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(PercentualeMagConiugeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.PERCENTUALEMAGCONIUGE.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDTIPOLOGIACONIUGE == (decimal)ibm.idTipologiaConiuge).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(PercentualeMagConiugeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.PERCENTUALEMAGCONIUGE.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDTIPOLOGIACONIUGE == (decimal)ibm.idTipologiaConiuge).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }



        public bool EsistonoMovimentiPrimaUguale(PercentualeMagConiugeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEMAGCONIUGE.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDTIPOLOGIACONIUGE == (decimal)ibm.idTipologiaConiuge).Count() > 0 ? true : false;
            }
        }

        public void DelPercMagConiuge(decimal idPercMaggConiuge)
        {
            PERCENTUALEMAGCONIUGE precedenteIB = new PERCENTUALEMAGCONIUGE();
            PERCENTUALEMAGCONIUGE delIB = new PERCENTUALEMAGCONIUGE();


            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.PERCENTUALEMAGCONIUGE.Where(a => a.IDPERCMAGCONIUGE == idPercMaggConiuge);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.PERCENTUALEMAGCONIUGE.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new PERCENTUALEMAGCONIUGE()
                            {

                                IDTIPOLOGIACONIUGE = precedenteIB.IDTIPOLOGIACONIUGE,
                                DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                PERCENTUALECONIUGE = precedenteIB.PERCENTUALECONIUGE,
                                ANNULLATO = false
                            };

                            db.PERCENTUALEMAGCONIUGE.Add(ibOld1);
                        }

                        db.SaveChanges();

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di percentuale maggiorazione coniuge.", "PERCENTUALEMAGCONIUGE", idPercMaggConiuge);
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
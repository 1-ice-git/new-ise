using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParMaggFigli : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<PercentualeMagFigliModel> getListMaggiorazioneFiglio()
        {
            List<PercentualeMagFigliModel> libm = new List<PercentualeMagFigliModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEMAGFIGLI.ToList();

                    libm = (from e in lib
                            select new PercentualeMagFigliModel()
                            {

                                idPercMagFigli = e.IDPERCMAGFIGLI,
                                idTipologiaFiglio = (TipologiaFiglio)e.IDTIPOLOGIAFIGLIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new PercentualeMagFigliModel().dataFineValidita,
                                percentualeFigli = e.PERCENTUALEFIGLI,

                                annullato = e.ANNULLATO,
                                tipologiaFiglio = new TipologiaFiglioModel()
                                {
                                    idTipologiaFiglio = e.IDTIPOLOGIAFIGLIO,
                                    tipologiaFiglio = e.TIPOLOGIAFIGLIO.ToString()
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

        public IList<PercentualeMagFigliModel> getListMaggiorazioneFiglio(decimal idTipologiaFiglio)
        {
            List<PercentualeMagFigliModel> libm = new List<PercentualeMagFigliModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEMAGFIGLI.Where(a => a.IDTIPOLOGIAFIGLIO == idTipologiaFiglio).ToList();

                    libm = (from e in lib
                            select new PercentualeMagFigliModel()
                            {

                                idPercMagFigli = e.IDPERCMAGFIGLI,
                                idTipologiaFiglio = (TipologiaFiglio)e.IDTIPOLOGIAFIGLIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new PercentualeMagFigliModel().dataFineValidita,
                                percentualeFigli = e.PERCENTUALEFIGLI,
                                annullato = e.ANNULLATO,
                                tipologiaFiglio = new TipologiaFiglioModel()
                                {
                                    idTipologiaFiglio = e.IDTIPOLOGIAFIGLIO,
                                    tipologiaFiglio = e.TIPOLOGIAFIGLIO.ToString()

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

        public IList<PercentualeMagFigliModel> getListMaggiorazioneFiglio(bool escludiAnnullati = false)
        {
            List<PercentualeMagFigliModel> libm = new List<PercentualeMagFigliModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEMAGFIGLI.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new PercentualeMagFigliModel()
                            {

                                idPercMagFigli = e.IDPERCMAGFIGLI,
                                idTipologiaFiglio = (TipologiaFiglio)e.IDTIPOLOGIAFIGLIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new PercentualeMagFigliModel().dataFineValidita,
                                percentualeFigli = e.PERCENTUALEFIGLI,
                                annullato = e.ANNULLATO,
                                tipologiaFiglio = new TipologiaFiglioModel()
                                {
                                    idTipologiaFiglio = e.IDTIPOLOGIAFIGLIO,
                                    tipologiaFiglio = e.TIPOLOGIAFIGLIO.ToString()

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

        public IList<PercentualeMagFigliModel> getListMaggiorazioneFiglio(decimal idTipologiaFiglio, bool escludiAnnullati = false)
        {
            List<PercentualeMagFigliModel> libm = new List<PercentualeMagFigliModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEMAGFIGLI.Where(a => a.IDTIPOLOGIAFIGLIO == idTipologiaFiglio && a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new PercentualeMagFigliModel()
                            {

                                idPercMagFigli = e.IDPERCMAGFIGLI,
                                idTipologiaFiglio = (TipologiaFiglio)e.IDTIPOLOGIAFIGLIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new PercentualeMagFigliModel().dataFineValidita,
                                percentualeFigli = e.PERCENTUALEFIGLI,
                                annullato = e.ANNULLATO,
                                tipologiaFiglio = new TipologiaFiglioModel()
                                {
                                    idTipologiaFiglio = e.IDTIPOLOGIAFIGLIO,
                                    tipologiaFiglio = e.TIPOLOGIAFIGLIO.ToString()

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
        public void SetMaggiorazioneFiglio(PercentualeMagFigliModel ibm)
        {
            List<PERCENTUALEMAGFIGLI> libNew = new List<PERCENTUALEMAGFIGLI>();

            PERCENTUALEMAGFIGLI ibNew = new PERCENTUALEMAGFIGLI();

            PERCENTUALEMAGFIGLI ibPrecedente = new PERCENTUALEMAGFIGLI();

            List<PERCENTUALEMAGFIGLI> lArchivioIB = new List<PERCENTUALEMAGFIGLI>();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    if (ibm.dataFineValidita.HasValue)
                    {
                        if (EsistonoMovimentiSuccessiviUguale(ibm))
                        {
                            ibNew = new PERCENTUALEMAGFIGLI()
                            {
                                IDPERCMAGFIGLI = ibm.idPercMagFigli,
                                IDTIPOLOGIAFIGLIO = (decimal)ibm.idTipologiaFiglio,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                PERCENTUALEFIGLI = ibm.percentualeFigli,
                                DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                                ANNULLATO = ibm.annullato
                            };
                        }
                        else
                        {
                            ibNew = new PERCENTUALEMAGFIGLI()
                            {
                                IDPERCMAGFIGLI = ibm.idPercMagFigli,
                                IDTIPOLOGIAFIGLIO = (decimal)ibm.idTipologiaFiglio,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                                PERCENTUALEFIGLI = ibm.percentualeFigli,
                                DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                                ANNULLATO = ibm.annullato
                            };
                        }
                    }
                    else
                    {
                        ibNew = new PERCENTUALEMAGFIGLI()
                        {
                            IDPERCMAGFIGLI = ibm.idPercMagFigli,
                            IDTIPOLOGIAFIGLIO = (decimal)ibm.idTipologiaFiglio,
                            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                            PERCENTUALEFIGLI = ibm.percentualeFigli,
                            DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                            ANNULLATO = ibm.annullato
                        };
                    }

                    db.Database.BeginTransaction();

                    var recordInteressati = db.PERCENTUALEMAGFIGLI.Where(a => a.ANNULLATO == false && a.IDTIPOLOGIAFIGLIO == ibNew.IDTIPOLOGIAFIGLIO)
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
                                    var ibOld1 = new PERCENTUALEMAGFIGLI()
                                    {
                                        IDPERCMAGFIGLI = item.IDPERCMAGFIGLI,
                                        IDTIPOLOGIAFIGLIO = item.IDTIPOLOGIAFIGLIO,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        PERCENTUALEFIGLI = item.PERCENTUALEFIGLI,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);

                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new PERCENTUALEMAGFIGLI()
                                    {
                                        IDPERCMAGFIGLI = item.IDPERCMAGFIGLI,
                                        IDTIPOLOGIAFIGLIO = item.IDTIPOLOGIAFIGLIO,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        PERCENTUALEFIGLI = item.PERCENTUALEFIGLI,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    var ibOld2 = new PERCENTUALEMAGFIGLI()
                                    {
                                        IDPERCMAGFIGLI = item.IDPERCMAGFIGLI,
                                        IDTIPOLOGIAFIGLIO = item.IDTIPOLOGIAFIGLIO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(+1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        PERCENTUALEFIGLI = item.PERCENTUALEFIGLI,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
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
                                    var ibOld1 = new PERCENTUALEMAGFIGLI()
                                    {
                                        IDPERCMAGFIGLI = item.IDPERCMAGFIGLI,
                                        IDTIPOLOGIAFIGLIO = item.IDTIPOLOGIAFIGLIO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        PERCENTUALEFIGLI = item.PERCENTUALEFIGLI,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
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
                                    var ibOld1 = new PERCENTUALEMAGFIGLI()
                                    {
                                        IDPERCMAGFIGLI = item.IDPERCMAGFIGLI,
                                        IDTIPOLOGIAFIGLIO = item.IDTIPOLOGIAFIGLIO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        PERCENTUALEFIGLI = item.PERCENTUALEFIGLI,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                }
                            }
                        }

                        libNew.Add(ibNew);
                        libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        db.PERCENTUALEMAGFIGLI.AddRange(libNew);
                    }
                    else
                    {
                        db.PERCENTUALEMAGFIGLI.Add(ibNew);

                    }
                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro per la percentuale di maggiorazione figli.", "PERCENTUALEMAGFIGLI", ibNew.IDPERCMAGFIGLI);
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

        public bool EsistonoMovimentiPrima(PercentualeMagFigliModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEMAGFIGLI.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDTIPOLOGIAFIGLIO == (decimal)ibm.idTipologiaFiglio).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(PercentualeMagFigliModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.PERCENTUALEMAGFIGLI.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDTIPOLOGIAFIGLIO == (decimal)ibm.idTipologiaFiglio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(PercentualeMagFigliModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.PERCENTUALEMAGFIGLI.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDTIPOLOGIAFIGLIO == (decimal)ibm.idTipologiaFiglio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }



        public bool EsistonoMovimentiPrimaUguale(PercentualeMagFigliModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEMAGFIGLI.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDTIPOLOGIAFIGLIO == (decimal)ibm.idTipologiaFiglio).Count() > 0 ? true : false;
            }
        }

        public void DelMaggiorazioneFiglio(decimal idPercMaggFiglio)
        {
            PERCENTUALEMAGFIGLI precedenteIB = new PERCENTUALEMAGFIGLI();
            PERCENTUALEMAGFIGLI delIB = new PERCENTUALEMAGFIGLI();


            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.PERCENTUALEMAGFIGLI.Where(a => a.IDPERCMAGFIGLI == idPercMaggFiglio);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.PERCENTUALEMAGFIGLI.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new PERCENTUALEMAGFIGLI()
                            {
                                IDPERCMAGFIGLI = precedenteIB.IDPERCMAGFIGLI,
                                IDTIPOLOGIAFIGLIO = precedenteIB.IDTIPOLOGIAFIGLIO,
                                DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                PERCENTUALEFIGLI = precedenteIB.PERCENTUALEFIGLI,
                                DATAAGGIORNAMENTO = precedenteIB.DATAAGGIORNAMENTO,
                                ANNULLATO = false
                            };

                            db.PERCENTUALEMAGFIGLI.Add(ibOld1);
                        }

                        db.SaveChanges();

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro per la percentuale di maggiorazione figlio.", "PERCENTUALEMAGFIGLI", idPercMaggFiglio);
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
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtMaggFigli : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<MaggiorazioneFigliModel> getListMaggiorazioneFiglio()
        {
            List<MaggiorazioneFigliModel> libm = new List<MaggiorazioneFigliModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.MAGGIORAZIONEFIGLI.ToList();

                    libm = (from e in lib
                            select new MaggiorazioneFigliModel()
                            {
                                
                                idMaggiorazioneFigli = e.IDMAGGIORAZIONEFIGLI,
                                idTipologiaFiglio =e.IDTIPOLOGIAFIGLIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new MaggiorazioneFigliModel().dataFineValidita,
                                percentualeFigli = e.PERCENTUALEFIGLI,
                                
                                annullato = e.ANNULLATO,
                                Figlio = new TipologiaFiglioModel()
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

        public IList<MaggiorazioneFigliModel> getListMaggiorazioneFiglio(decimal idTipologiaFiglio)
        {
            List<MaggiorazioneFigliModel> libm = new List<MaggiorazioneFigliModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.MAGGIORAZIONEFIGLI.Where(a => a.IDTIPOLOGIAFIGLIO == idTipologiaFiglio).ToList();

                    libm = (from e in lib
                            select new MaggiorazioneFigliModel()
                            {
                                
                                idMaggiorazioneFigli = e.IDMAGGIORAZIONEFIGLI,
                                idTipologiaFiglio = e.IDTIPOLOGIAFIGLIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new MaggiorazioneFigliModel().dataFineValidita,
                                percentualeFigli = e.PERCENTUALEFIGLI,
                                annullato = e.ANNULLATO,
                                Figlio = new TipologiaFiglioModel()
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

        public IList<MaggiorazioneFigliModel> getListMaggiorazioneFiglio(bool escludiAnnullati = false)
        {
            List<MaggiorazioneFigliModel> libm = new List<MaggiorazioneFigliModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.MAGGIORAZIONEFIGLI.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new MaggiorazioneFigliModel()
                            {
                                
                                idMaggiorazioneFigli =e.IDMAGGIORAZIONEFIGLI,
                                idTipologiaFiglio = e.IDTIPOLOGIAFIGLIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new MaggiorazioneFigliModel().dataFineValidita,
                                percentualeFigli = e.PERCENTUALEFIGLI,
                                annullato = e.ANNULLATO,
                                Figlio = new TipologiaFiglioModel()
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

        public IList<MaggiorazioneFigliModel> getListMaggiorazioneFiglio(decimal idTipologiaFiglio, bool escludiAnnullati = false)
        {
            List<MaggiorazioneFigliModel> libm = new List<MaggiorazioneFigliModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.MAGGIORAZIONEFIGLI.Where(a => a.IDTIPOLOGIAFIGLIO == idTipologiaFiglio && a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new MaggiorazioneFigliModel()
                            {

                                idMaggiorazioneFigli = e.IDMAGGIORAZIONEFIGLI,
                                idTipologiaFiglio = e.IDTIPOLOGIAFIGLIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new MaggiorazioneConiugeModel().dataFineValidita,
                                percentualeFigli = e.PERCENTUALEFIGLI,
                                annullato = e.ANNULLATO,
                                Figlio = new TipologiaFiglioModel()
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
        public void SetMaggiorazioneFiglio(MaggiorazioneFigliModel ibm)
        {
            List<MAGGIORAZIONEFIGLI> libNew = new List<MAGGIORAZIONEFIGLI>();

            MAGGIORAZIONEFIGLI ibNew = new MAGGIORAZIONEFIGLI();

            MAGGIORAZIONEFIGLI ibPrecedente = new MAGGIORAZIONEFIGLI();

            List<MAGGIORAZIONEFIGLI> lArchivioIB = new List<MAGGIORAZIONEFIGLI>();

            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                try
                {
                    if (ibm.dataFineValidita.HasValue)
                    {
                        if (EsistonoMovimentiSuccessiviUguale(ibm))
                        {
                            ibNew = new MAGGIORAZIONEFIGLI()
                            {
                                
                                IDTIPOLOGIAFIGLIO = ibm.idTipologiaFiglio,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                PERCENTUALEFIGLI = ibm.percentualeFigli,
                                ANNULLATO = ibm.annullato
                            };
                        }
                        else
                        {
                            ibNew = new MAGGIORAZIONEFIGLI()
                            {
                                
                                IDTIPOLOGIAFIGLIO = ibm.idTipologiaFiglio,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                                PERCENTUALEFIGLI = ibm.percentualeFigli,
                                ANNULLATO = ibm.annullato
                            };
                        }
                    }
                    else
                    {
                        ibNew = new MAGGIORAZIONEFIGLI()
                        {
                            
                            IDTIPOLOGIAFIGLIO = ibm.idTipologiaFiglio,
                            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                            PERCENTUALEFIGLI = ibm.percentualeFigli,
                            ANNULLATO = ibm.annullato
                        };
                    }

                    db.Database.BeginTransaction();

                    var recordInteressati = db.MAGGIORAZIONEFIGLI.Where(a => a.ANNULLATO == false && a.IDTIPOLOGIAFIGLIO == ibNew.IDTIPOLOGIAFIGLIO)
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
                                    var ibOld1 = new MAGGIORAZIONEFIGLI()
                                    {
                                        
                                        IDTIPOLOGIAFIGLIO = ibm.idTipologiaFiglio,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        PERCENTUALEFIGLI = item.PERCENTUALEFIGLI,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);

                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new MAGGIORAZIONEFIGLI()
                                    {
                                        
                                        IDTIPOLOGIAFIGLIO = item.IDTIPOLOGIAFIGLIO,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        PERCENTUALEFIGLI = item.PERCENTUALEFIGLI,
                                        ANNULLATO = false
                                    };

                                    var ibOld2 = new MAGGIORAZIONEFIGLI()
                                    {

                                        IDTIPOLOGIAFIGLIO = item.IDTIPOLOGIAFIGLIO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(+1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        PERCENTUALEFIGLI = item.PERCENTUALEFIGLI,
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
                                    var ibOld1 = new MAGGIORAZIONEFIGLI()
                                    {
                                        
                                        IDTIPOLOGIAFIGLIO = item.IDTIPOLOGIAFIGLIO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        PERCENTUALEFIGLI = item.PERCENTUALEFIGLI,
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
                                    var ibOld1 = new MAGGIORAZIONEFIGLI()
                                    {

                                        IDTIPOLOGIAFIGLIO = item.IDTIPOLOGIAFIGLIO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        PERCENTUALEFIGLI = item.PERCENTUALEFIGLI,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                }
                            }
                        }

                        libNew.Add(ibNew);
                        libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        db.MAGGIORAZIONEFIGLI.AddRange(libNew);
                    }
                    else
                    {
                        db.MAGGIORAZIONEFIGLI.Add(ibNew);

                    }
                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro di maggiorazione figli.", "MAGGIORAZIONEFIGLI", ibNew.IDMAGGIORAZIONEFIGLI);
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

        public bool EsistonoMovimentiPrima(MaggiorazioneFigliModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                return db.MAGGIORAZIONEFIGLI.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDTIPOLOGIAFIGLIO == ibm.idTipologiaFiglio).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(MaggiorazioneFigliModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.MAGGIORAZIONEFIGLI.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDTIPOLOGIAFIGLIO == ibm.idTipologiaFiglio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(MaggiorazioneFigliModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.MAGGIORAZIONEFIGLI.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDTIPOLOGIAFIGLIO == ibm.idTipologiaFiglio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }



        public bool EsistonoMovimentiPrimaUguale(MaggiorazioneFigliModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                return db.MAGGIORAZIONEFIGLI.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDTIPOLOGIAFIGLIO == ibm.idTipologiaFiglio).Count() > 0 ? true : false;
            }
        }

        public void DelMaggiorazioneFiglio(decimal idMaggFiglio)
        {
            MAGGIORAZIONEFIGLI precedenteIB = new MAGGIORAZIONEFIGLI();
            MAGGIORAZIONEFIGLI delIB = new MAGGIORAZIONEFIGLI();


            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.MAGGIORAZIONEFIGLI.Where(a => a.IDMAGGIORAZIONEFIGLI == idMaggFiglio);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.MAGGIORAZIONEFIGLI.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new MAGGIORAZIONEFIGLI()
                            {
                                
                                IDTIPOLOGIAFIGLIO = precedenteIB.IDTIPOLOGIAFIGLIO,
                                DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                PERCENTUALEFIGLI = precedenteIB.PERCENTUALEFIGLI,
                                ANNULLATO = false
                            };

                            db.MAGGIORAZIONEFIGLI.Add(ibOld1);
                        }

                        db.SaveChanges();

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di maggiorazione figlio.", "MAGGIORAZIONEFIGLIO", idMaggFiglio);
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
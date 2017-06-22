using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using NewISE.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParIndPrimoSegr : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<IndennitaPrimoSegretModel> getListIndennitaPrimoSegretario()
        {
            List<IndennitaPrimoSegretModel> libm = new List<IndennitaPrimoSegretModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.INDENNITAPRIMOSEGRETARIO.ToList();

                    libm = (from e in lib
                            select new IndennitaPrimoSegretModel()
                            {
                                
                                idIndPrimoSegr = e.IDINDPRIMOSEGR,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaPrimoSegretModel().dataFineValidita,
                                indennita = e.INDENNITA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<IndennitaPrimoSegretModel> getListIndennitaPrimoSegretario(decimal idIndPrimoSegr)
        {
            List<IndennitaPrimoSegretModel> libm = new List<IndennitaPrimoSegretModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.INDENNITAPRIMOSEGRETARIO.Where(a => a.IDINDPRIMOSEGR == idIndPrimoSegr).ToList();

                    libm = (from e in lib
                            select new IndennitaPrimoSegretModel()
                            {
                                
                                idIndPrimoSegr = e.IDINDPRIMOSEGR,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaPrimoSegretModel().dataFineValidita,
                                indennita = e.INDENNITA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<IndennitaPrimoSegretModel> getListIndennitaPrimoSegretario(bool escludiAnnullati = false)
        {
            List<IndennitaPrimoSegretModel> libm = new List<IndennitaPrimoSegretModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.INDENNITAPRIMOSEGRETARIO.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new IndennitaPrimoSegretModel()
                            {
                                
                                idIndPrimoSegr = e.IDINDPRIMOSEGR,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaPrimoSegretModel().dataFineValidita,
                                indennita = e.INDENNITA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                //Livello = new LivelloModel()
                                //{
                                //    idLivello = e.LIVELLI.IDLIVELLO,
                                //    DescLivello = e.LIVELLI.LIVELLO
                                //}
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<IndennitaPrimoSegretModel> getListIndennitaPrimoSegretario(decimal idIndPrimoSegr, bool escludiAnnullati = false)
        {
            List<IndennitaPrimoSegretModel> libm = new List<IndennitaPrimoSegretModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.INDENNITAPRIMOSEGRETARIO.Where(a => a.IDINDPRIMOSEGR == idIndPrimoSegr && a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new IndennitaPrimoSegretModel()
                            {
                                
                                idIndPrimoSegr = e.IDINDPRIMOSEGR,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaPrimoSegretModel().dataFineValidita,
                                indennita = e.INDENNITA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                //Livello = new LivelloModel()
                                //{
                                //    idLivello = e.LIVELLI.IDLIVELLO,
                                //    DescLivello = e.LIVELLI.LIVELLO
                                //}
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
        public void SetIndennitaPrimoSegretario(IndennitaPrimoSegretModel ibm)
        {
            List<INDENNITAPRIMOSEGRETARIO> libNew = new List<INDENNITAPRIMOSEGRETARIO>();

            INDENNITAPRIMOSEGRETARIO ibNew = new INDENNITAPRIMOSEGRETARIO();

            INDENNITAPRIMOSEGRETARIO ibPrecedente = new INDENNITAPRIMOSEGRETARIO();

            List<INDENNITAPRIMOSEGRETARIO> lArchivioIB = new List<INDENNITAPRIMOSEGRETARIO>();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    if (ibm.dataFineValidita.HasValue)
                    {
                        if (EsistonoMovimentiSuccessiviUguale(ibm))
                        {
                            ibNew = new INDENNITAPRIMOSEGRETARIO()
                            {
                                
                                IDINDPRIMOSEGR = ibm.idIndPrimoSegr,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                INDENNITA = ibm.indennita,
                                DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                                ANNULLATO = ibm.annullato
                            };
                        }
                        else
                        {
                            ibNew = new INDENNITAPRIMOSEGRETARIO()
                            {
                                
                                IDINDPRIMOSEGR = ibm.idIndPrimoSegr,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                                INDENNITA = ibm.indennita,
                                DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                                ANNULLATO = ibm.annullato
                            };
                        }
                    }
                    else
                    {
                        ibNew = new INDENNITAPRIMOSEGRETARIO()
                        {
                            
                            IDINDPRIMOSEGR = ibm.idIndPrimoSegr,
                            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                            INDENNITA = ibm.indennita,
                            DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                            ANNULLATO = ibm.annullato
                        };
                    }

                    db.Database.BeginTransaction();

                    var recordInteressati = db.INDENNITAPRIMOSEGRETARIO.Where(a => a.ANNULLATO == false && a.IDINDPRIMOSEGR == ibNew.IDINDPRIMOSEGR)
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
                                    var ibOld1 = new INDENNITAPRIMOSEGRETARIO()
                                    {
                                        
                                        IDINDPRIMOSEGR = item.IDINDPRIMOSEGR,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        INDENNITA = item.INDENNITA,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);

                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new INDENNITAPRIMOSEGRETARIO()
                                    {
                                        
                                        IDINDPRIMOSEGR = item.IDINDPRIMOSEGR,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        INDENNITA = item.INDENNITA,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    var ibOld2 = new INDENNITAPRIMOSEGRETARIO()
                                    {
                                        
                                        IDINDPRIMOSEGR = item.IDINDPRIMOSEGR,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(+1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        INDENNITA = item.INDENNITA,
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
                                    var ibOld1 = new INDENNITAPRIMOSEGRETARIO()
                                    {
                                        
                                        IDINDPRIMOSEGR = item.IDINDPRIMOSEGR,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        INDENNITA = item.INDENNITA,
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
                                    var ibOld1 = new INDENNITAPRIMOSEGRETARIO()
                                    {
                                        
                                        IDINDPRIMOSEGR = item.IDINDPRIMOSEGR,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        INDENNITA = item.INDENNITA,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                }
                            }
                        }

                        libNew.Add(ibNew);
                        libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        db.INDENNITAPRIMOSEGRETARIO.AddRange(libNew);
                    }
                    else
                    {
                        db.INDENNITAPRIMOSEGRETARIO.Add(ibNew);

                    }
                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro di indennità di primo segretario.", "INDENNITAPRIMOSEGRETARIO", ibNew.IDINDPRIMOSEGR);
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
        public bool EsistonoMovimentiPrima(IndennitaPrimoSegretModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.INDENNITAPRIMOSEGRETARIO.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDINDPRIMOSEGR == ibm.idIndPrimoSegr).Count() > 0 ? true : false;
            }
        }
        public bool EsistonoMovimentiSuccessivi(IndennitaPrimoSegretModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.INDENNITAPRIMOSEGRETARIO.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDINDPRIMOSEGR == ibm.idIndPrimoSegr).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool EsistonoMovimentiSuccessiviUguale(IndennitaPrimoSegretModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.INDENNITAPRIMOSEGRETARIO.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDINDPRIMOSEGR == ibm.idIndPrimoSegr).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool EsistonoMovimentiPrimaUguale(IndennitaPrimoSegretModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.INDENNITAPRIMOSEGRETARIO.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDINDPRIMOSEGR == ibm.idIndPrimoSegr).Count() > 0 ? true : false;
            }
        }
        public void DelIndennitaPrimoSegretario(decimal idIndPrimoSegr)
        {
            INDENNITAPRIMOSEGRETARIO precedenteIB = new INDENNITAPRIMOSEGRETARIO();
            INDENNITAPRIMOSEGRETARIO delIB = new INDENNITAPRIMOSEGRETARIO();


            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.INDENNITAPRIMOSEGRETARIO.Where(a => a.IDINDPRIMOSEGR == idIndPrimoSegr);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.INDENNITAPRIMOSEGRETARIO.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new INDENNITAPRIMOSEGRETARIO()
                            {
                                
                                IDINDPRIMOSEGR = precedenteIB.IDINDPRIMOSEGR,
                                DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                INDENNITA = precedenteIB.INDENNITA,
                                ANNULLATO = false
                            };

                            db.INDENNITAPRIMOSEGRETARIO.Add(ibOld1);
                        }

                        db.SaveChanges();

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di indennità primo segretario.", "INDENNITAPRIMOSEGRETARIO", idIndPrimoSegr);
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
        public IList<IndennitaPrimoSegretModel> getIndennitaPrimoSegretario()
        {
            List<IndennitaPrimoSegretModel> llm = new List<IndennitaPrimoSegretModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ll = db.INDENNITAPRIMOSEGRETARIO.ToList();

                    llm = (from e in ll
                           select new IndennitaPrimoSegretModel()
                           {
                               
                               idIndPrimoSegr = e.IDINDPRIMOSEGR,
                               indennita = e.INDENNITA
                               
                           }).ToList();
                }

                return llm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IndennitaPrimoSegretModel getIndennitaPrimoSegretario(decimal idIndPrimoSegr)
        {
            IndennitaPrimoSegretModel lm = new IndennitaPrimoSegretModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var liv = db.INDENNITAPRIMOSEGRETARIO.Find(idIndPrimoSegr);

                    lm = new IndennitaPrimoSegretModel()
                    {
                        idIndPrimoSegr = liv.IDINDPRIMOSEGR,
                        indennita = liv.INDENNITA
                        
                    };
                }

                return lm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
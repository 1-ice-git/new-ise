﻿using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
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
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.INDENNITABASE.ToList();

                    libm = (from e in lib
                            select new IndennitaBaseModel()
                            {
                                idIndennitaBase = e.IDINDENNITABASE,
                                idLivello = e.IDLIVELLO,
                                idRiduzioni = e.IDRIDUZIONI,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaBaseModel().dataFineValidita,
                                valore = e.VALORE,
                                valoreResponsabile = e.VALORERESP,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                Livello = new LivelloModel()
                                {
                                    idLivello = e.LIVELLI.IDLIVELLO,
                                    DescLivello = e.LIVELLI.LIVELLO
                                },
                                Riduzioni = new RiduzioniModel()
                                {
                                    idRegola = e.RIDUZIONI.IDREGOLA,
                                    idRiduzioni = e.RIDUZIONI.IDRIDUZIONI
                                    
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
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.INDENNITABASE.Where(a => a.IDLIVELLO == idLivello).ToList();

                    libm = (from e in lib
                            select new IndennitaBaseModel()
                            {
                                idIndennitaBase = e.IDINDENNITABASE,
                                idLivello = e.IDLIVELLO,
                                idRiduzioni = e.IDRIDUZIONI,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaBaseModel().dataFineValidita,
                                valore = e.VALORE,
                                valoreResponsabile = e.VALORERESP,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                Livello = new LivelloModel()
                                {
                                    idLivello = e.LIVELLI.IDLIVELLO,
                                    DescLivello = e.LIVELLI.LIVELLO
                                },
                                Riduzioni = new RiduzioniModel()
                                {
                                    idRegola = e.RIDUZIONI.IDREGOLA,
                                    idRiduzioni = e.RIDUZIONI.IDRIDUZIONI

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
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.INDENNITABASE.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new IndennitaBaseModel()
                            {
                                idIndennitaBase = e.IDINDENNITABASE,
                                idLivello = e.IDLIVELLO,
                                idRiduzioni = e.IDRIDUZIONI,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaBaseModel().dataFineValidita,
                                valore = e.VALORE,
                                valoreResponsabile = e.VALORERESP,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                Livello = new LivelloModel()
                                {
                                    idLivello = e.LIVELLI.IDLIVELLO,
                                    DescLivello = e.LIVELLI.LIVELLO
                                },
                                Riduzioni = new RiduzioniModel()
                                {
                                    idRegola = e.RIDUZIONI.IDREGOLA,
                                    idRiduzioni = e.RIDUZIONI.IDRIDUZIONI

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
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.INDENNITABASE.Where(a => a.IDLIVELLO == idLivello && a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new IndennitaBaseModel()
                            {
                                idIndennitaBase = e.IDINDENNITABASE,
                                idLivello = e.IDLIVELLO,
                                idRiduzioni = e.IDRIDUZIONI,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaBaseModel().dataFineValidita,
                                valore = e.VALORE,
                                valoreResponsabile = e.VALORERESP,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                Livello = new LivelloModel()
                                {
                                    idLivello = e.LIVELLI.IDLIVELLO,
                                    DescLivello = e.LIVELLI.LIVELLO
                                },
                                Riduzioni = new RiduzioniModel()
                                {
                                    idRegola = e.RIDUZIONI.IDREGOLA,
                                    idRiduzioni = e.RIDUZIONI.IDRIDUZIONI

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
        public void SetIndennitaDiBase(IndennitaBaseModel ibm)
        {
            List<INDENNITABASE> libNew = new List<INDENNITABASE>();

            INDENNITABASE ibNew = new INDENNITABASE();

            INDENNITABASE ibPrecedente = new INDENNITABASE();

            List<INDENNITABASE> lArchivioIB = new List<INDENNITABASE>();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    if (ibm.dataFineValidita.HasValue)
                    {
                        if (EsistonoMovimentiSuccessiviUguale(ibm))
                        {
                            ibNew = new INDENNITABASE()
                            {
                                IDINDENNITABASE = ibm.idIndennitaBase,
                                IDLIVELLO = ibm.idLivello,
                                IDRIDUZIONI = ibm.idRiduzioni,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                VALORE = ibm.valore,
                                VALORERESP = ibm.valoreResponsabile,
                                DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                                ANNULLATO = ibm.annullato
                            };
                        }
                        else
                        {
                            ibNew = new INDENNITABASE()
                            {
                                IDINDENNITABASE = ibm.idIndennitaBase,
                                IDLIVELLO = ibm.idLivello,
                                IDRIDUZIONI = ibm.idRiduzioni,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                                VALORE = ibm.valore,
                                VALORERESP = ibm.valoreResponsabile,
                                DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                                ANNULLATO = ibm.annullato
                            };
                        }
                    }
                    else
                    {
                        ibNew = new INDENNITABASE()
                        {
                            IDINDENNITABASE = ibm.idIndennitaBase,
                            IDLIVELLO = ibm.idLivello,
                            IDRIDUZIONI = ibm.idRiduzioni,
                            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                            VALORE = ibm.valore,
                            VALORERESP = ibm.valoreResponsabile,
                            DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                            ANNULLATO = ibm.annullato
                        };
                    }

                    db.Database.BeginTransaction();

                    var recordInteressati = db.INDENNITABASE.Where(a => a.ANNULLATO == false && a.IDLIVELLO == ibNew.IDLIVELLO)
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
                                    var ibOld1 = new INDENNITABASE()
                                    {
                                        IDINDENNITABASE =item.IDINDENNITABASE,
                                        IDLIVELLO = item.IDLIVELLO,
                                        IDRIDUZIONI = item.IDRIDUZIONI,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        VALORE = item.VALORE,
                                        VALORERESP = item.VALORERESP,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                    
                                }
                                else if(item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new INDENNITABASE()
                                    {
                                        IDINDENNITABASE = item.IDINDENNITABASE,
                                        IDLIVELLO = item.IDLIVELLO,
                                        IDRIDUZIONI = item.IDRIDUZIONI,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        VALORE = item.VALORE,
                                        VALORERESP = item.VALORERESP,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    var ibOld2 = new INDENNITABASE()
                                    {
                                        IDINDENNITABASE = item.IDINDENNITABASE,
                                        IDLIVELLO = item.IDLIVELLO,
                                        IDRIDUZIONI = item.IDRIDUZIONI,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(+ 1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        VALORE = item.VALORE,
                                        VALORERESP = item.VALORERESP,
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
                                    var ibOld1 = new INDENNITABASE()
                                    {
                                        IDINDENNITABASE = item.IDINDENNITABASE,
                                        IDLIVELLO = item.IDLIVELLO,
                                        IDRIDUZIONI = item.IDRIDUZIONI,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        VALORE = item.VALORE,
                                        VALORERESP = item.VALORERESP,
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
                                    var ibOld1 = new INDENNITABASE()
                                    {
                                        IDINDENNITABASE = item.IDINDENNITABASE,
                                        IDLIVELLO = item.IDLIVELLO,
                                        IDRIDUZIONI = item.IDRIDUZIONI,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        VALORE = item.VALORE,
                                        VALORERESP = item.VALORERESP,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                }
                            }
                        }

                        libNew.Add(ibNew); 
                        libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        db.INDENNITABASE.AddRange(libNew);
                    }
                    else
                    {
                        db.INDENNITABASE.Add(ibNew);
                        
                    }
                    db.SaveChanges();

                    using (objLogAttivita log=new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro di indennità di base.", "INDENNITABASE", ibNew.IDINDENNITABASE);
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

        public bool EsistonoMovimentiPrima(IndennitaBaseModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.INDENNITABASE.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDLIVELLO == ibm.idLivello).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(IndennitaBaseModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.INDENNITABASE.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDLIVELLO == ibm.idLivello).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(IndennitaBaseModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.INDENNITABASE.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDLIVELLO == ibm.idLivello).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

       

        public bool EsistonoMovimentiPrimaUguale(IndennitaBaseModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.INDENNITABASE.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDLIVELLO == ibm.idLivello).Count() > 0 ? true : false;
            }
        }

        public void DelIndennitaDiBase(decimal idIndbase)
        {
            INDENNITABASE precedenteIB = new INDENNITABASE();            
            INDENNITABASE delIB = new INDENNITABASE();


            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.INDENNITABASE.Where(a => a.IDINDENNITABASE == idIndbase);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.INDENNITABASE.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new INDENNITABASE()
                            {
                                IDINDENNITABASE = precedenteIB.IDINDENNITABASE,
                                IDLIVELLO = precedenteIB.IDLIVELLO,
                                IDRIDUZIONI = precedenteIB.IDRIDUZIONI,
                                DATAINIZIOVALIDITA = precedenteIB.DATAFINEVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                VALORE = precedenteIB.VALORE,
                                VALORERESP = precedenteIB.VALORERESP,
                                ANNULLATO = false
                            };

                            db.INDENNITABASE.Add(ibOld1);
                        }

                        db.SaveChanges();

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di indennità di base.", "INDENNITABASE", idIndbase);
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
﻿using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParPercMaggAbitazione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }



        public IList<PercMaggAbitazModel> getListMaggiorazioneAbitazione()
        {
            List<PercMaggAbitazModel> libm = new List<PercMaggAbitazModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.PERCENTUALEMAB.ToList();

                    libm = (from e in lib
                            select new PercMaggAbitazModel()
                            {
                                
                                idPercMabAbitaz = e.IDPERCMAB,
                                idLivello = e.IDLIVELLO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new PercMaggAbitazModel().dataFineValidita,
                                percentuale = e.PERCENTUALE,
                                percentualeResponsabile = e.PERCENTUALERESPONSABILE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
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

        public IList<PercMaggAbitazModel> getListMaggiorazioneAbitazione(decimal idLivello)
        {
            List<PercMaggAbitazModel> libm = new List<PercMaggAbitazModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.PERCENTUALEMAB.Where(a => a.IDLIVELLO == idLivello).ToList();

                    libm = (from e in lib
                            select new PercMaggAbitazModel()
                            {
                                idPercMabAbitaz = e.IDPERCMAB,
                                idLivello = e.IDLIVELLO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new PercMaggAbitazModel().dataFineValidita,
                                percentuale = e.PERCENTUALE,
                                percentualeResponsabile = e.PERCENTUALERESPONSABILE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
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

        public IList<PercMaggAbitazModel> getListMaggiorazioneAbitazione(bool escludiAnnullati = false)
        {
            List<PercMaggAbitazModel> libm = new List<PercMaggAbitazModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.PERCENTUALEMAB.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new PercMaggAbitazModel()
                            {
                                idPercMabAbitaz = e.IDPERCMAB,
                                idLivello = e.IDLIVELLO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaBaseModel().dataFineValidita,
                                percentuale = e.PERCENTUALE,
                                percentualeResponsabile = e.PERCENTUALERESPONSABILE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
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

        public IList<PercMaggAbitazModel> getListMaggiorazioneAbitazione(decimal idLivello, bool escludiAnnullati = false)
        {
            List<PercMaggAbitazModel> libm = new List<PercMaggAbitazModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.PERCENTUALEMAB.Where(a => a.IDLIVELLO == idLivello && a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new PercMaggAbitazModel()
                            {
                                idPercMabAbitaz = e.IDPERCMAB,
                                idLivello = e.IDLIVELLO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaBaseModel().dataFineValidita,
                                percentuale = e.PERCENTUALE,
                                percentualeResponsabile = e.PERCENTUALERESPONSABILE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ibm"></param>
        public void SetMaggiorazioneAbitazione(PercMaggAbitazModel ibm)
        {
            List<PERCENTUALEMAB> libNew = new List<PERCENTUALEMAB>();

            PERCENTUALEMAB ibNew = new PERCENTUALEMAB();

            PERCENTUALEMAB ibPrecedente = new PERCENTUALEMAB();

            List<PERCENTUALEMAB> lArchivioIB = new List<PERCENTUALEMAB>();

            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                try
                {
                    if (ibm.dataFineValidita.HasValue)
                    {
                        if (EsistonoMovimentiSuccessiviUguale(ibm))
                        {
                            ibNew = new PERCENTUALEMAB()
                            {
                                IDLIVELLO = ibm.idLivello,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                PERCENTUALE = ibm.percentuale,
                                PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                                DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                                ANNULLATO = ibm.annullato
                            };
                        }
                        else
                        {
                            ibNew = new PERCENTUALEMAB()
                            {
                                IDLIVELLO = ibm.idLivello,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                                PERCENTUALE = ibm.percentuale,
                                PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                                DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                                ANNULLATO = ibm.annullato
                            };
                        }
                    }
                    else
                    {
                        ibNew = new PERCENTUALEMAB()
                        {
                            IDLIVELLO = ibm.idLivello,
                            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                            PERCENTUALE = ibm.percentuale,
                            PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                            DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                            ANNULLATO = ibm.annullato
                        };
                    }

                    db.Database.BeginTransaction();

                    var recordInteressati = db.PERCENTUALEMAB.Where(a => a.ANNULLATO == false && a.IDLIVELLO == ibNew.IDLIVELLO)
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
                                    var ibOld1 = new PERCENTUALEMAB()
                                    {
                                        IDLIVELLO = item.IDLIVELLO,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        PERCENTUALE = item.PERCENTUALE,
                                        PERCENTUALERESPONSABILE = item.PERCENTUALERESPONSABILE,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);

                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new PERCENTUALEMAB()
                                    {
                                        IDLIVELLO = item.IDLIVELLO,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        PERCENTUALE = item.PERCENTUALE,
                                        PERCENTUALERESPONSABILE = item.PERCENTUALERESPONSABILE,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    var ibOld2 = new PERCENTUALEMAB()
                                    {
                                        IDLIVELLO = item.IDLIVELLO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(+1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        PERCENTUALE = item.PERCENTUALE,
                                        PERCENTUALERESPONSABILE = item.PERCENTUALERESPONSABILE,
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
                                    var ibOld1 = new PERCENTUALEMAB()
                                    {
                                        IDLIVELLO = item.IDLIVELLO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        PERCENTUALE = item.PERCENTUALE,
                                        PERCENTUALERESPONSABILE = item.PERCENTUALERESPONSABILE,
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
                                    var ibOld1 = new PERCENTUALEMAB()
                                    {
                                        IDLIVELLO = item.IDLIVELLO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        PERCENTUALE = item.PERCENTUALE,
                                        PERCENTUALERESPONSABILE = item.PERCENTUALERESPONSABILE,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                }
                            }
                        }

                        libNew.Add(ibNew);
                        libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        db.PERCENTUALEMAB.AddRange(libNew);
                    }
                    else
                    {
                        db.PERCENTUALEMAB.Add(ibNew);

                    }
                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro di indennità di base.", "PERCENTUALEMAB", ibNew.IDPERCMAB);
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

        public bool EsistonoMovimentiPrima(PercMaggAbitazModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                return db.PERCENTUALEMAB.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDLIVELLO == ibm.idLivello).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(PercMaggAbitazModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.PERCENTUALEMAB.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDLIVELLO == ibm.idLivello).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(PercMaggAbitazModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.PERCENTUALEMAB.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDLIVELLO == ibm.idLivello).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }



        public bool EsistonoMovimentiPrimaUguale(PercMaggAbitazModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                return db.PERCENTUALEMAB.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDLIVELLO == ibm.idLivello).Count() > 0 ? true : false;
            }
        }

        public void DelMaggiorazioneAbitazione(decimal idPercMabAbitaz)
        {
            PERCENTUALEMAB precedenteIB = new PERCENTUALEMAB();
            PERCENTUALEMAB delIB = new PERCENTUALEMAB();


            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.PERCENTUALEMAB.Where(a => a.IDPERCMAB == idPercMabAbitaz);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.PERCENTUALEMAB.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new PERCENTUALEMAB()
                            {
                                IDLIVELLO = precedenteIB.IDLIVELLO,
                                DATAINIZIOVALIDITA = precedenteIB.DATAFINEVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                PERCENTUALE = precedenteIB.PERCENTUALE,
                                PERCENTUALERESPONSABILE = precedenteIB.PERCENTUALERESPONSABILE,
                                DATAAGGIORNAMENTO = precedenteIB.DATAAGGIORNAMENTO,
                                ANNULLATO = false
                            };

                            db.PERCENTUALEMAB.Add(ibOld1);
                        }

                        db.SaveChanges();

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di indennità di base.", "PERCENTUALEMAB", idPercMabAbitaz);
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
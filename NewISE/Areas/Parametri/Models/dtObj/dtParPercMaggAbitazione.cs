using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.dtObj;

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
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEMAB.ToList();

                    libm = (from e in lib
                            select new PercMaggAbitazModel()
                            {

                                idPercMabAbitaz = e.IDPERCMAB,
                                idLivello = e.IDLIVELLO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,// != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new PercMaggAbitazModel().dataFineValidita,
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
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEMAB.Where(a => a.IDLIVELLO == idLivello).ToList();

                    libm = (from e in lib
                            select new PercMaggAbitazModel()
                            {
                                idPercMabAbitaz = e.IDPERCMAB,
                                idLivello = e.IDLIVELLO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,//!= Utility.DataFineStop() ? e.DATAFINEVALIDITA : new PercMaggAbitazModel().dataFineValidita,
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
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEMAB.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new PercMaggAbitazModel()
                            {
                                idPercMabAbitaz = e.IDPERCMAB,
                                idLivello = e.IDLIVELLO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,//!= Utility.DataFineStop() ? e.DATAFINEVALIDITA : new IndennitaBaseModel().dataFineValidita,
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

        public IList<PercMaggAbitazModel> getListMaggiorazioneAbitazione(decimal idLivello, decimal idUfficio, bool escludiAnnullati = false)
        {
            List<PercMaggAbitazModel> libm = new List<PercMaggAbitazModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<PERCENTUALEMAB> lib = new List<PERCENTUALEMAB>();
                    if (escludiAnnullati == true)
                        lib = db.PERCENTUALEMAB.Where(a => a.IDLIVELLO == idLivello && a.IDUFFICIO == idUfficio && a.ANNULLATO == false).ToList();
                    else
                        lib = db.PERCENTUALEMAB.Where(a => a.IDLIVELLO == idLivello && a.IDUFFICIO == idUfficio).ToList();

                    libm = (from e in lib
                            select new PercMaggAbitazModel()
                            {
                                idPercMabAbitaz = e.IDPERCMAB,
                                idLivello = e.IDLIVELLO,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,//!= Utility.DataFineStop() ? e.DATAFINEVALIDITA : new IndennitaBaseModel().dataFineValidita,
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
        //public void SetMaggiorazioneAbitazione(PercMaggAbitazModel ibm)
        //{
        //    List<PERCENTUALEMAB> libNew = new List<PERCENTUALEMAB>();

        //    PERCENTUALEMAB ibNew = new PERCENTUALEMAB();

        //    PERCENTUALEMAB ibPrecedente = new PERCENTUALEMAB();

        //    List<PERCENTUALEMAB> lArchivioIB = new List<PERCENTUALEMAB>();

        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        try
        //        {
        //            if (ibm.dataFineValidita.HasValue)
        //            {
        //                if (EsistonoMovimentiSuccessiviUguale(ibm))
        //                {
        //                    ibNew = new PERCENTUALEMAB()
        //                    {
        //                        IDLIVELLO = ibm.idLivello,
        //                        IDUFFICIO=ibm.idUfficio,
        //                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
        //                        DATAFINEVALIDITA = ibm.dataFineValidita.Value,
        //                        PERCENTUALE = ibm.percentuale,
        //                        PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
        //                        DATAAGGIORNAMENTO = DateTime.Now,
        //                        ANNULLATO = ibm.annullato
        //                    };
        //                }
        //                else
        //                {
        //                    ibNew = new PERCENTUALEMAB()
        //                    {
        //                        IDLIVELLO = ibm.idLivello,
        //                        IDUFFICIO = ibm.idUfficio,
        //                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
        //                        DATAFINEVALIDITA = Utility.DataFineStop(),
        //                        PERCENTUALE = ibm.percentuale,
        //                        PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
        //                        DATAAGGIORNAMENTO = DateTime.Now,
        //                        ANNULLATO = ibm.annullato
        //                    };
        //                }
        //            }
        //            else
        //            {
        //                ibNew = new PERCENTUALEMAB()
        //                {
        //                    IDLIVELLO = ibm.idLivello,
        //                    IDUFFICIO = ibm.idUfficio,
        //                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
        //                    DATAFINEVALIDITA = Utility.DataFineStop(),
        //                    PERCENTUALE = ibm.percentuale,
        //                    PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
        //                    DATAAGGIORNAMENTO = DateTime.Now,
        //                    ANNULLATO = ibm.annullato
        //                };
        //            }

        //            db.Database.BeginTransaction();

        //            var recordInteressati = db.PERCENTUALEMAB.Where(a => a.ANNULLATO == false && a.IDLIVELLO == ibNew.IDLIVELLO)
        //                                                    .Where(a => a.DATAINIZIOVALIDITA >= ibNew.DATAINIZIOVALIDITA || a.DATAFINEVALIDITA >= ibNew.DATAINIZIOVALIDITA)
        //                                                    .Where(a => a.DATAINIZIOVALIDITA <= ibNew.DATAFINEVALIDITA || a.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
        //                                                    .ToList();

        //            recordInteressati.ForEach(a => a.ANNULLATO = true);
        //            //db.SaveChanges();

        //            if (recordInteressati.Count > 0)
        //            {
        //                foreach (var item in recordInteressati)
        //                {

        //                    if (item.DATAINIZIOVALIDITA < ibNew.DATAINIZIOVALIDITA)
        //                    {
        //                        if (item.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
        //                        {
        //                            var ibOld1 = new PERCENTUALEMAB()
        //                            {
        //                                IDLIVELLO = item.IDLIVELLO,
        //                                IDUFFICIO = ibm.idUfficio,
        //                                DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
        //                                DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
        //                                PERCENTUALE = item.PERCENTUALE,
        //                                PERCENTUALERESPONSABILE = item.PERCENTUALERESPONSABILE,
        //                                DATAAGGIORNAMENTO = DateTime.Now,
        //                                ANNULLATO = false
        //                            };

        //                            libNew.Add(ibOld1);

        //                        }
        //                        else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
        //                        {
        //                            var ibOld1 = new PERCENTUALEMAB()
        //                            {
        //                                IDLIVELLO = item.IDLIVELLO,
        //                                IDUFFICIO = ibm.idUfficio,
        //                                DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
        //                                DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
        //                                PERCENTUALE = item.PERCENTUALE,
        //                                PERCENTUALERESPONSABILE = item.PERCENTUALERESPONSABILE,
        //                                DATAAGGIORNAMENTO = DateTime.Now,
        //                                ANNULLATO = false
        //                            };

        //                            var ibOld2 = new PERCENTUALEMAB()
        //                            {
        //                                IDLIVELLO = item.IDLIVELLO,
        //                                IDUFFICIO = ibm.idUfficio,
        //                                DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(+1),
        //                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,
        //                                PERCENTUALE = item.PERCENTUALE,
        //                                PERCENTUALERESPONSABILE = item.PERCENTUALERESPONSABILE,
        //                                DATAAGGIORNAMENTO = DateTime.Now,
        //                                ANNULLATO = false
        //                            };

        //                            libNew.Add(ibOld1);
        //                            libNew.Add(ibOld2);

        //                        }

        //                    }
        //                    else if (item.DATAINIZIOVALIDITA == ibNew.DATAINIZIOVALIDITA)
        //                    {
        //                        if (item.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
        //                        {
        //                            //Non preleva il record old
        //                        }
        //                        else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
        //                        {
        //                            var ibOld1 = new PERCENTUALEMAB()
        //                            {
        //                                IDLIVELLO = item.IDLIVELLO,
        //                                IDUFFICIO = ibm.idUfficio,
        //                                DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
        //                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,
        //                                PERCENTUALE = item.PERCENTUALE,
        //                                PERCENTUALERESPONSABILE = item.PERCENTUALERESPONSABILE,
        //                                DATAAGGIORNAMENTO = DateTime.Now,
        //                                ANNULLATO = false
        //                            };

        //                            libNew.Add(ibOld1);
        //                        }
        //                    }
        //                    else if (item.DATAINIZIOVALIDITA > ibNew.DATAINIZIOVALIDITA)
        //                    {
        //                        if (item.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
        //                        {
        //                            //Non preleva il record old
        //                        }
        //                        else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
        //                        {
        //                            var ibOld1 = new PERCENTUALEMAB()
        //                            {
        //                                IDLIVELLO = item.IDLIVELLO,
        //                                IDUFFICIO = ibm.idUfficio,
        //                                DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
        //                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,
        //                                PERCENTUALE = item.PERCENTUALE,
        //                                PERCENTUALERESPONSABILE = item.PERCENTUALERESPONSABILE,
        //                                DATAAGGIORNAMENTO = DateTime.Now,
        //                                ANNULLATO = false
        //                            };

        //                            libNew.Add(ibOld1);
        //                        }
        //                    }
        //                }

        //                libNew.Add(ibNew);
        //                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

        //                db.PERCENTUALEMAB.AddRange(libNew);
        //            }
        //            else
        //            {
        //                db.PERCENTUALEMAB.Add(ibNew);

        //            }
        //            db.SaveChanges();

        //            using (objLogAttivita log = new objLogAttivita())
        //            {
        //                log.Log(enumAttivita.Inserimento, "Inserimento parametro di indennità di base.", "PERCENTUALEMAB", ibNew.IDPERCMAB);
        //            }

        //            db.Database.CurrentTransaction.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            db.Database.CurrentTransaction.Rollback();
        //            throw ex;
        //        }
        //    }
        //}
        public void SetMaggiorazioneAbitazione(PercMaggAbitazModel ibm, bool aggiornaTutto)
        {
            List<PERCENTUALEMAB> libNew = new List<PERCENTUALEMAB>();

            //PERCENTUALEMAB ibPrecedente = new PERCENTUALEMAB();
            PERCENTUALEMAB ibNew1 = new PERCENTUALEMAB();
            PERCENTUALEMAB ibNew2 = new PERCENTUALEMAB();
            //List<PERCENTUALEMAB> lArchivioIB = new List<PERCENTUALEMAB>();
            List<string> lista = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                bool giafatta = false;
                try
                {
                    using (dtPercMaggAbitazione dtal = new dtPercMaggAbitazione())
                    {
                        //Se la data variazione coincide con una data inizio esistente
                        lista = dtal.DataVariazioneCoincideConDataInizio(ibm.dataInizioValidita, ibm.idLivello, ibm.idUfficio);
                        if (lista.Count != 0)
                        {
                            giafatta = true;
                            decimal idIntervalloFirst = Convert.ToDecimal(lista[0]);
                            DateTime dataInizioFirst = Convert.ToDateTime(lista[1]);
                            DateTime dataFineFirst = Convert.ToDateTime(lista[2]);
                            //decimal PercentualeFirst = Convert.ToDecimal(lista[3]);
                            //decimal PercentualeRespFirst = Convert.ToDecimal(lista[4]);

                            ibNew1 = new PERCENTUALEMAB()
                            {
                                IDLIVELLO = ibm.idLivello,
                                IDUFFICIO = ibm.idUfficio,
                                DATAINIZIOVALIDITA = dataInizioFirst,
                                DATAFINEVALIDITA = dataFineFirst,
                                PERCENTUALE = ibm.percentuale,
                                PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                                DATAAGGIORNAMENTO = DateTime.Now
                            };

                            if (aggiornaTutto)
                            {
                                ibNew1 = new PERCENTUALEMAB()
                                {
                                    IDLIVELLO = ibm.idLivello,
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = dataInizioFirst,
                                    DATAFINEVALIDITA = Utility.DataFineStop(),
                                    PERCENTUALE = ibm.percentuale,
                                    PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };
                                //qui annullo tutti i record rimanenti dalla data inizio inserita
                                libNew = db.PERCENTUALEMAB.Where(a => a.IDLIVELLO == ibm.idLivello && a.IDUFFICIO == ibm.idUfficio
                                && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > dataInizioFirst).ToList();
                                foreach (var elem in libNew)
                                {
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPERCMAB), db);
                                }
                            }
                            db.Database.BeginTransaction();
                            db.PERCENTUALEMAB.Add(ibNew1);
                            db.SaveChanges();
                            RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloFirst), db);

                            using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                            {
                                dtrp.AssociaMAB_VMAB(ibNew1.IDPERCMAB, db, ibm.dataInizioValidita);
                            }

                            db.Database.CurrentTransaction.Commit();
                        }
                        ///se la data variazione coincide con una data fine esistente(diversa da 31/12/9999)
                        if (giafatta == false)
                        {
                            lista = dtal.DataVariazioneCoincideConDataFine(ibm.dataInizioValidita, ibm.idLivello, ibm.idUfficio);
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervalloLast = Convert.ToDecimal(lista[0]);
                                DateTime dataInizioLast = Convert.ToDateTime(lista[1]);
                                DateTime dataFineLast = Convert.ToDateTime(lista[2]);
                                // decimal aliquotaLast = Convert.ToDecimal(lista[3]);
                                //decimal PercentualeLast = Convert.ToDecimal(lista[3]);
                                //decimal PercentualeRespLast = Convert.ToDecimal(lista[4]);

                                ibNew1 = new PERCENTUALEMAB()
                                {
                                    IDLIVELLO = ibm.idLivello,
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = dataInizioLast,
                                    DATAFINEVALIDITA = dataFineLast.AddDays(-1),
                                    PERCENTUALE = ibm.percentuale,
                                    PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                                    DATAAGGIORNAMENTO = DateTime.Now

                                };
                                ibNew2 = new PERCENTUALEMAB()
                                {
                                    IDLIVELLO = ibm.idLivello,
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = ibm.dataInizioValidita,//è uguale alla data Inizio
                                    PERCENTUALE = ibm.percentuale,
                                    PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };
                                if (aggiornaTutto)
                                {
                                    ibNew2 = new PERCENTUALEMAB()
                                    {
                                        IDLIVELLO = ibm.idLivello,
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        PERCENTUALE = ibm.percentuale,
                                        PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.PERCENTUALEMAB.Where(a => a.IDLIVELLO == ibm.idLivello && a.IDUFFICIO == ibm.idUfficio
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPERCMAB), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                db.Database.BeginTransaction();
                                db.PERCENTUALEMAB.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloLast), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var pmab in libNew)
                                    {
                                        dtrp.AssociaMAB_VMAB(pmab.IDPERCMAB, db, ibm.dataInizioValidita);
                                    }

                                }

                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //Se il nuovo record si trova in un intervallo non annullato con data fine non uguale al 31/12/9999
                        if (giafatta == false)
                        {
                            lista = dtal.RestituisciIntervalloDiUnaData(ibm.dataInizioValidita, ibm.idLivello, ibm.idUfficio);
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervallo = Convert.ToDecimal(lista[0]);
                                DateTime dataInizio = Convert.ToDateTime(lista[1]);
                                DateTime dataFine = Convert.ToDateTime(lista[2]);
                                decimal percentuale = Convert.ToDecimal(lista[3]);
                                decimal percentualeResponsabile = Convert.ToDecimal(lista[4]);

                                DateTime NewdataFine1 = ibm.dataInizioValidita.AddDays(-1);

                                ibNew1 = new PERCENTUALEMAB()
                                {
                                    IDLIVELLO = ibm.idLivello,
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = dataInizio,
                                    DATAFINEVALIDITA = NewdataFine1,
                                    // COEFFICIENTEKM = aliquota,
                                    PERCENTUALE = percentuale,
                                    PERCENTUALERESPONSABILE = percentualeResponsabile,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new PERCENTUALEMAB()
                                {
                                    IDLIVELLO = ibm.idLivello,
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = dataFine,
                                    PERCENTUALE = ibm.percentuale,
                                    PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };

                                if (aggiornaTutto)
                                {
                                    ibNew2 = new PERCENTUALEMAB()
                                    {
                                        IDLIVELLO = ibm.idLivello,
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        PERCENTUALE = ibm.percentuale,
                                        PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.PERCENTUALEMAB.Where(a => a.IDLIVELLO == ibm.idLivello && a.IDUFFICIO == ibm.idUfficio
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPERCMAB), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                db.Database.BeginTransaction();
                                db.PERCENTUALEMAB.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervallo), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var pmab in libNew)
                                    {
                                        dtrp.AssociaMAB_VMAB(pmab.IDPERCMAB, db, ibm.dataInizioValidita);
                                    }

                                }

                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //CASO DELL'ULTIMA RIGA CON LA DATA FINE UGUALE A 31/12/9999
                        if (giafatta == false)
                        {
                            //Attenzione qui se la lista non contiene nessun elemento
                            //significa che non esiste nessun elemento corrispondentemente al livello selezionato
                            lista = dtal.RestituisciLaRigaMassima(ibm.idLivello, ibm.idUfficio);

                            if (lista.Count == 0)
                            {
                                ibNew1 = new PERCENTUALEMAB()
                                {
                                    IDLIVELLO = ibm.idLivello,
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = Convert.ToDateTime(Utility.DataFineStop()),
                                    PERCENTUALE = ibm.percentuale,
                                    PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                                    DATAAGGIORNAMENTO = DateTime.Now,

                                };
                                libNew.Add(ibNew1);
                                db.Database.BeginTransaction();
                                db.PERCENTUALEMAB.Add(ibNew1);
                                db.SaveChanges();

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {

                                    dtrp.AssociaMAB_VMAB(ibNew1.IDPERCMAB, db, ibm.dataInizioValidita);

                                }

                                db.Database.CurrentTransaction.Commit();
                            }

                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                //se il nuovo record rappresenta la data variazione uguale alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                //occorre annullare il record esistente in questione ed aggiungere un nuovo con la stessa data inizio e l'altro campo da aggiornare con il nuovo

                                decimal idIntervalloUltimo = Convert.ToDecimal(lista[0]);
                                DateTime dataInizioUltimo = Convert.ToDateTime(lista[1]);
                                DateTime dataFineUltimo = Convert.ToDateTime(lista[2]);
                                decimal PercentoUltimo = Convert.ToDecimal(lista[3]);
                                decimal PercentoRespUltimo = Convert.ToDecimal(lista[4]);
                                if (dataInizioUltimo == ibm.dataInizioValidita)
                                {
                                    ibNew1 = new PERCENTUALEMAB()
                                    {
                                        IDLIVELLO = ibm.idLivello,
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = dataFineUltimo,
                                        PERCENTUALE = ibm.percentuale,
                                        PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,//nuova aliquota rispetto alla vecchia registrata
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.PERCENTUALEMAB.Add(ibNew1);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {

                                        dtrp.AssociaMAB_VMAB(ibNew1.IDPERCMAB, db, ibm.dataInizioValidita);

                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                                //se il nuovo record rappresenta la data variazione superiore alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                if (ibm.dataInizioValidita > dataInizioUltimo)
                                {
                                    ibNew1 = new PERCENTUALEMAB()
                                    {
                                        IDLIVELLO = ibm.idLivello,
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = ibm.dataInizioValidita.AddDays(-1),
                                        PERCENTUALE = PercentoUltimo,
                                        PERCENTUALERESPONSABILE = PercentoRespUltimo,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    ibNew2 = new PERCENTUALEMAB()
                                    {
                                        IDLIVELLO = ibm.idLivello,
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        PERCENTUALE = ibm.percentuale,//nuova aliquota rispetto alla vecchia registrata
                                        PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1); libNew.Add(ibNew2);
                                    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                    db.Database.BeginTransaction();
                                    db.PERCENTUALEMAB.AddRange(libNew);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        foreach (var pmab in libNew)
                                        {
                                            dtrp.AssociaMAB_VMAB(pmab.IDPERCMAB, db, ibm.dataInizioValidita);
                                        }


                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                            }
                        }
                        // db.Database.CurrentTransaction.Commit();
                    }
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
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEMAB.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDLIVELLO == ibm.idLivello).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(PercMaggAbitazModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
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
            using (ModelDBISE db = new ModelDBISE())
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
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEMAB.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDLIVELLO == ibm.idLivello).Count() > 0 ? true : false;
            }
        }

        //public void DelMaggiorazioneAbitazione(decimal idPercMabAbitaz)
        //{
        //    PERCENTUALEMAB precedenteIB = new PERCENTUALEMAB();
        //    PERCENTUALEMAB delIB = new PERCENTUALEMAB();


        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        try
        //        {
        //            db.Database.BeginTransaction();

        //            var lib = db.PERCENTUALEMAB.Where(a => a.IDPERCMAB == idPercMabAbitaz);

        //            if (lib.Count() > 0)
        //            {
        //                delIB = lib.First();
        //                delIB.ANNULLATO = true;

        //                var lprecIB = db.PERCENTUALEMAB.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

        //                if (lprecIB.Count > 0)
        //                {
        //                    precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
        //                    precedenteIB.ANNULLATO = true;

        //                    var ibOld1 = new PERCENTUALEMAB()
        //                    {
        //                        IDLIVELLO = precedenteIB.IDLIVELLO,
        //                        DATAINIZIOVALIDITA = precedenteIB.DATAFINEVALIDITA,
        //                        DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
        //                        PERCENTUALE = precedenteIB.PERCENTUALE,
        //                        PERCENTUALERESPONSABILE = precedenteIB.PERCENTUALERESPONSABILE,
        //                        DATAAGGIORNAMENTO = precedenteIB.DATAAGGIORNAMENTO,
        //                        ANNULLATO = false
        //                    };

        //                    db.PERCENTUALEMAB.Add(ibOld1);
        //                }

        //                db.SaveChanges();

        //                using (objLogAttivita log = new objLogAttivita())
        //                {
        //                    log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di indennità di base.", "PERCENTUALEMAB", idPercMabAbitaz);
        //                }
        //                db.Database.CurrentTransaction.Commit();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            db.Database.CurrentTransaction.Rollback();
        //            throw ex;
        //        }
        //    }
        //}
        public void DelMaggiorazioneAbitazione(decimal IDPERCMAB)
        {
            PERCENTUALEMAB precedenteIB = new PERCENTUALEMAB();
            PERCENTUALEMAB delIB = new PERCENTUALEMAB();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.PERCENTUALEMAB.Where(a => a.IDPERCMAB == IDPERCMAB);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;
                        RendiAnnullatoUnRecord(delIB.IDPERCMAB, db);
                        precedenteIB = RestituisciIlRecordPrecedente(IDPERCMAB);
                        RendiAnnullatoUnRecord(precedenteIB.IDPERCMAB, db);

                        var NuovoPrecedente = new PERCENTUALEMAB()
                        {
                            IDUFFICIO = precedenteIB.IDUFFICIO,
                            IDLIVELLO = precedenteIB.IDLIVELLO,
                            DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                            //ALIQUOTA = precedenteIB.ALIQUOTA,
                            PERCENTUALE = precedenteIB.PERCENTUALE,
                            PERCENTUALERESPONSABILE = precedenteIB.PERCENTUALERESPONSABILE,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            ANNULLATO = false
                        };
                        db.PERCENTUALEMAB.Add(NuovoPrecedente);

                        db.SaveChanges();

                        using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                        {

                            dtrp.AssociaMAB_VMAB(NuovoPrecedente.IDPERCMAB, db, delIB.DATAINIZIOVALIDITA);

                        }

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione percentuale maggiorazione abitazione.", "PERCENTUALEMAB", IDPERCMAB);
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

        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as PercMaggAbitazModel;

            if (fm != null)
            {
                DateTime d = DataInizioMinimaNonAnnullataMaggAbitazione(fm.idUfficio, fm.idUfficio);
                if (fm.dataInizioValidita < d)
                {
                    vr = new ValidationResult(string.Format("Impossibile inserire la data di inizio validità minore alla data di Base ({0}).", d.ToShortDateString()));
                }
                else
                {
                    vr = ValidationResult.Success;
                }
            }
            else
            {
                vr = new ValidationResult("La data di inizio validità è richiesta.");
            }
            return vr;
        }
        public static DateTime DataInizioMinimaNonAnnullataMaggAbitazione(decimal idLivello, decimal idUfficio)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.PERCENTUALEMAB.Where(a => a.ANNULLATO == false &&
                a.IDLIVELLO == idLivello &&
                a.IDUFFICIO == idUfficio).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }
        public decimal Get_Id_MaggAbitazioneNonAnnullato(decimal IDLIVELLO, decimal IDUFFICIO)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEMAB> libm = new List<PERCENTUALEMAB>();
                libm = db.PERCENTUALEMAB.Where(a => a.ANNULLATO == false
                && a.IDLIVELLO == IDLIVELLO && a.IDUFFICIO == IDUFFICIO).OrderBy(a => a.DATAINIZIOVALIDITA).ThenBy(a => a.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDPERCMAB;
            }
            return tmp;
        }

        public static ValidationResult VerificaPercentualeResponsabile(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as PercMaggAbitazModel;

            if (fm != null)
            {
                if (fm.percentualeResponsabile > 100)
                {
                    vr = new ValidationResult(string.Format("Impossibile inserire percentuale  maggiore di 100 ({0}).", fm.percentualeResponsabile.ToString()));
                }
                else
                {
                    vr = ValidationResult.Success;
                }
            }
            else
            {
                vr = new ValidationResult("La percentuale  è richiesta.");
            }
            return vr;
        }
        public static ValidationResult VerificaPercentuale(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as PercMaggAbitazModel;

            if (fm != null)
            {
                if (fm.percentuale > 100)
                {
                    vr = new ValidationResult(string.Format("Impossibile inserire percentuale KM maggiore di 100 ({0}).", fm.percentuale.ToString()));
                }
                else
                {
                    vr = ValidationResult.Success;
                }
            }
            else
            {
                vr = new ValidationResult("La percentuale responsabile è richiesta.");
            }
            return vr;
        }

        public PERCENTUALEMAB RestituisciIlRecordPrecedente(decimal IDCFKM)
        {
            PERCENTUALEMAB tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                PERCENTUALEMAB interessato = new PERCENTUALEMAB();
                interessato = db.PERCENTUALEMAB.Find(IDCFKM);
                tmp = db.PERCENTUALEMAB.Where(a => a.IDLIVELLO == interessato.IDLIVELLO && a.IDUFFICIO == interessato.IDUFFICIO
                && a.ANNULLATO == false).ToList().Where(b => b.DATAFINEVALIDITA == interessato.DATAINIZIOVALIDITA.AddDays(-1)).ToList().First();
            }
            return tmp;
        }
        public List<string> RestituisciIntervalloDiUnaData(DateTime DataCampione, decimal IDLIVELLO, decimal IDUFFICIO)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEMAB> libm = new List<PERCENTUALEMAB>();
                libm = db.PERCENTUALEMAB.Where(a => a.ANNULLATO == false
                && a.IDLIVELLO == IDLIVELLO && a.IDUFFICIO == IDUFFICIO).ToList().Where(b =>
                 b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())
                 && DataCampione > b.DATAINIZIOVALIDITA
                 && DataCampione < b.DATAFINEVALIDITA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCMAB.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALE.ToString());
                    tmp.Add(libm[0].PERCENTUALERESPONSABILE.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione, decimal IDLIVELLO, decimal IDUFFICIO)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEMAB> libm = new List<PERCENTUALEMAB>();
                libm = db.PERCENTUALEMAB.Where(a => a.ANNULLATO == false
                && a.IDLIVELLO == IDLIVELLO && a.IDUFFICIO == IDUFFICIO).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().Where(b => DataCampione == b.DATAINIZIOVALIDITA &&
                   b.DATAFINEVALIDITA != Utility.DataFineStop()).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCMAB.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALE.ToString());
                    tmp.Add(libm[0].PERCENTUALERESPONSABILE.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione, decimal IDLIVELLO, decimal IDUFFICIO)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEMAB> libm = new List<PERCENTUALEMAB>();
                libm = db.PERCENTUALEMAB.Where(a => a.ANNULLATO == false
                && a.IDLIVELLO == IDLIVELLO && a.IDUFFICIO == IDUFFICIO).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().
                Where(b => DataCampione == b.DATAFINEVALIDITA
                && b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())).ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCMAB.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALE.ToString());
                    tmp.Add(libm[0].PERCENTUALERESPONSABILE.ToString());
                }
            }
            return tmp;
        }
        public List<string> RestituisciLaRigaMassima(decimal IDLIVELLO, decimal IDUFFICIO)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEMAB> libm = new List<PERCENTUALEMAB>();
                libm = db.PERCENTUALEMAB.Where(a => a.ANNULLATO == false
                && a.IDLIVELLO == IDLIVELLO && a.IDUFFICIO == IDUFFICIO).ToList().Where(b =>
                 b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop())).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCMAB.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALE.ToString());
                    tmp.Add(libm[0].PERCENTUALERESPONSABILE.ToString());
                }
            }
            return tmp;
        }
        public void RendiAnnullatoUnRecord(decimal IDPERCMAB, ModelDBISE db)
        {
            PERCENTUALEMAB entita = new PERCENTUALEMAB();
            entita = db.PERCENTUALEMAB.Find(IDPERCMAB);
            entita.ANNULLATO = true;
            db.SaveChanges();
        }
    }
}
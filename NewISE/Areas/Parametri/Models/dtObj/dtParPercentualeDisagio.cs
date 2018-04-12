using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.Models.Tools;
using System.ComponentModel.DataAnnotations;
using NewISE.Models.dtObj;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParPercentualeDisagio : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }



        //public IList<PercentualeDisagioModel> getListPercentualeDisagio(decimal idUfficio)
        //{
        //    List<PercentualeDisagioModel> libm = new List<PercentualeDisagioModel>();

        //    try
        //    {
        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            var lib = db.PERCENTUALEDISAGIO.Where(a => a.IDUFFICIO == idUfficio).ToList();

        //            libm = (from e in lib
        //                    select new PercentualeDisagioModel()
        //                    {
        //                        idPercentualeDisagio = e.IDPERCENTUALEDISAGIO,
        //                        idUfficio = e.IDUFFICIO,
        //                        dataInizioValidita = e.DATAINIZIOVALIDITA,
        //                        dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new PercentualeDisagioModel().dataFineValidita,
        //                        percentuale = e.PERCENTUALE,
        //                        annullato = e.ANNULLATO,
        //                        Ufficio = new UfficiModel()
        //                        {
        //                            idUfficio = e.UFFICI.IDUFFICIO,
        //                            descUfficio = e.UFFICI.DESCRIZIONEUFFICIO
        //                        }
        //                    }).ToList();
        //        }

        //        return libm;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public IList<PercentualeDisagioModel> getListPercentualeDisagio(bool escludiAnnullati = false)
        //{
        //    List<PercentualeDisagioModel> libm = new List<PercentualeDisagioModel>();

        //    try
        //    {
        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            var lib = db.PERCENTUALEDISAGIO.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

        //            libm = (from e in lib
        //                    select new PercentualeDisagioModel()
        //                    {
        //                        idPercentualeDisagio = e.IDPERCENTUALEDISAGIO,
        //                        idUfficio = e.IDUFFICIO,
        //                        dataInizioValidita = e.DATAINIZIOVALIDITA,
        //                        dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new PercentualeDisagioModel().dataFineValidita,
        //                        percentuale = e.PERCENTUALE,
        //                        annullato = e.ANNULLATO,
        //                        Ufficio = new UfficiModel()
        //                        {
        //                            idUfficio = e.UFFICI.IDUFFICIO,
        //                            descUfficio = e.UFFICI.DESCRIZIONEUFFICIO
        //                        }
        //                    }).ToList();
        //        }

        //        return libm;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public IList<PercentualeDisagioModel> getListPercentualeDisagio(decimal idUfficio, bool escludiAnnullati = false)
        //{
        //    List<PercentualeDisagioModel> libm = new List<PercentualeDisagioModel>();

        //    try
        //    {
        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            var lib = db.PERCENTUALEDISAGIO.Where(a => a.IDUFFICIO == idUfficio && a.ANNULLATO == escludiAnnullati).ToList();

        //            libm = (from e in lib
        //                    select new PercentualeDisagioModel()
        //                    {
        //                        idPercentualeDisagio = e.IDPERCENTUALEDISAGIO,
        //                        idUfficio = e.IDUFFICIO,
        //                        dataInizioValidita = e.DATAINIZIOVALIDITA,
        //                        dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new PercentualeDisagioModel().dataFineValidita,
        //                        percentuale = e.PERCENTUALE,
        //                        annullato = e.ANNULLATO,
        //                        Ufficio = new UfficiModel()
        //                        {
        //                            idUfficio = e.UFFICI.IDUFFICIO,
        //                            descUfficio = e.UFFICI.DESCRIZIONEUFFICIO
        //                        }
        //                    }).ToList();
        //        }

        //        return libm;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ibm"></param>

        public void SetPercentualeDisagio(PercentualeDisagioModel ibm, bool aggiornaTutto)
        {
            List<PERCENTUALEDISAGIO> libNew = new List<PERCENTUALEDISAGIO>();

            //PERCENTUALEDISAGIO ibPrecedente = new PERCENTUALEDISAGIO();
            PERCENTUALEDISAGIO ibNew1 = new PERCENTUALEDISAGIO();
            PERCENTUALEDISAGIO ibNew2 = new PERCENTUALEDISAGIO();
            //List<PERCENTUALEDISAGIO> lArchivioIB = new List<PERCENTUALEDISAGIO>();
            List<string> lista = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                bool giafatta = false;
                try
                {
                    using (dtParPercentualeDisagio dtal = new dtParPercentualeDisagio())
                    {
                        //Se la data variazione coincide con una data inizio esistente
                        lista = dtal.DataVariazioneCoincideConDataInizio(ibm.dataInizioValidita, ibm.idUfficio);

                        if (lista.Count != 0)
                        {
                            giafatta = true;
                            decimal idIntervalloFirst = Convert.ToDecimal(lista[0]);
                            DateTime dataInizioFirst = Convert.ToDateTime(lista[1]);
                            DateTime dataFineFirst = Convert.ToDateTime(lista[2]);
                            //decimal aliquotaFirst = Convert.ToDecimal(lista[3]);

                            ibNew1 = new PERCENTUALEDISAGIO()
                            {
                                IDUFFICIO = ibm.idUfficio,
                                DATAINIZIOVALIDITA = dataInizioFirst,
                                DATAFINEVALIDITA = dataFineFirst,
                                // ALIQUOTA = ibm.aliquota,
                                PERCENTUALE = ibm.percentuale,
                                DATAAGGIORNAMENTO = DateTime.Now,
                            };

                            if (aggiornaTutto)
                            {
                                ibNew1 = new PERCENTUALEDISAGIO()
                                {
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = dataInizioFirst,
                                    DATAFINEVALIDITA = Utility.DataFineStop(),
                                    // ALIQUOTA = ibm.aliquota,
                                    PERCENTUALE = ibm.percentuale,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                //qui annullo tutti i record rimanenti dalla data inizio inserita
                                libNew = db.PERCENTUALEDISAGIO.Where(a => a.IDUFFICIO == ibm.idUfficio
                                && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > dataInizioFirst).ToList();
                                foreach (var elem in libNew)
                                {
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPERCENTUALEDISAGIO), db);
                                }
                            }
                            db.Database.BeginTransaction();
                            db.PERCENTUALEDISAGIO.Add(ibNew1);
                            db.SaveChanges();
                            RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloFirst), db);

                            using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                            {
                                dtrp.AssociaIndenita_PD(ibNew1.IDPERCENTUALEDISAGIO, db);
                            }

                            db.Database.CurrentTransaction.Commit();
                        }
                        ///se la data variazione coincide con una data fine esistente(diversa da 31/12/9999)
                        if (giafatta == false)
                        {
                            lista = dtal.DataVariazioneCoincideConDataFine(ibm.dataInizioValidita, ibm.idUfficio);

                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervalloLast = Convert.ToDecimal(lista[0]);
                                DateTime dataInizioLast = Convert.ToDateTime(lista[1]);
                                DateTime dataFineLast = Convert.ToDateTime(lista[2]);
                                decimal aliquotaLast = Convert.ToDecimal(lista[3]);

                                ibNew1 = new PERCENTUALEDISAGIO()
                                {
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = dataInizioLast,
                                    DATAFINEVALIDITA = dataFineLast.AddDays(-1),
                                    PERCENTUALE = aliquotaLast,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new PERCENTUALEDISAGIO()
                                {
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = ibm.dataInizioValidita,//è uguale alla data Inizio
                                    PERCENTUALE = ibm.percentuale,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };
                                if (aggiornaTutto)
                                {
                                    ibNew2 = new PERCENTUALEDISAGIO()
                                    {
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        PERCENTUALE = ibm.percentuale,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.PERCENTUALEDISAGIO.Where(a => a.IDUFFICIO == ibm.idUfficio
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPERCENTUALEDISAGIO), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                db.Database.BeginTransaction();
                                db.PERCENTUALEDISAGIO.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloLast), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var pd in libNew)
                                    {
                                        dtrp.AssociaIndenita_PD(pd.IDPERCENTUALEDISAGIO, db);
                                    }

                                }

                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //Se il nuovo record si trova in un intervallo non annullato con data fine non uguale al 31/12/9999
                        if (giafatta == false)
                        {
                            lista = dtal.RestituisciIntervalloDiUnaData(ibm.dataInizioValidita, ibm.idUfficio);

                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervallo = Convert.ToDecimal(lista[0]);
                                DateTime dataInizio = Convert.ToDateTime(lista[1]);
                                DateTime dataFine = Convert.ToDateTime(lista[2]);
                                decimal aliquota = Convert.ToDecimal(lista[3]);

                                DateTime NewdataFine1 = ibm.dataInizioValidita.AddDays(-1);

                                ibNew1 = new PERCENTUALEDISAGIO()
                                {
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = dataInizio,
                                    DATAFINEVALIDITA = NewdataFine1,
                                    //ALIQUOTA = aliquota,
                                    PERCENTUALE = aliquota,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new PERCENTUALEDISAGIO()
                                {
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = dataFine,
                                    // ALIQUOTA = ibm.aliquota,
                                    PERCENTUALE = ibm.percentuale,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };

                                if (aggiornaTutto)
                                {
                                    ibNew2 = new PERCENTUALEDISAGIO()
                                    {
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        // ALIQUOTA = ibm.aliquota,
                                        PERCENTUALE = ibm.percentuale,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.PERCENTUALEDISAGIO.Where(a => a.IDUFFICIO == ibm.idUfficio
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPERCENTUALEDISAGIO), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                db.Database.BeginTransaction();
                                db.PERCENTUALEDISAGIO.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervallo), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var pd in libNew)
                                    {
                                        dtrp.AssociaIndenita_PD(pd.IDPERCENTUALEDISAGIO, db);
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
                            lista = dtal.RestituisciLaRigaMassima(ibm.idUfficio);
                            if (lista.Count == 0)
                            {
                                ibNew1 = new PERCENTUALEDISAGIO()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = Convert.ToDateTime(Utility.DataFineStop()),
                                    PERCENTUALE = ibm.percentuale,
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                libNew.Add(ibNew1);
                                db.Database.BeginTransaction();
                                db.PERCENTUALEDISAGIO.Add(ibNew1);
                                db.SaveChanges();

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {

                                    dtrp.AssociaIndenita_PD(ibNew1.IDPERCENTUALEDISAGIO, db);

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
                                decimal aliquotaUltimo = Convert.ToDecimal(lista[3]);

                                if (dataInizioUltimo == ibm.dataInizioValidita)
                                {
                                    ibNew1 = new PERCENTUALEDISAGIO()
                                    {
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = dataFineUltimo,
                                        // ALIQUOTA = ibm.aliquota,//nuova aliquota rispetto alla vecchia registrata
                                        PERCENTUALE = ibm.percentuale,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.PERCENTUALEDISAGIO.Add(ibNew1);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {

                                        dtrp.AssociaIndenita_PD(ibNew1.IDPERCENTUALEDISAGIO, db);

                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                                //se il nuovo record rappresenta la data variazione superiore alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                if (ibm.dataInizioValidita > dataInizioUltimo)
                                {
                                    ibNew1 = new PERCENTUALEDISAGIO()
                                    {
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = ibm.dataInizioValidita.AddDays(-1),
                                        PERCENTUALE = aliquotaUltimo,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    ibNew2 = new PERCENTUALEDISAGIO()
                                    {
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        PERCENTUALE = ibm.percentuale,//nuova aliquota rispetto alla vecchia registrata
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1); libNew.Add(ibNew2);
                                    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                    db.Database.BeginTransaction();
                                    db.PERCENTUALEDISAGIO.AddRange(libNew);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        foreach (var pd in libNew)
                                        {
                                            dtrp.AssociaIndenita_PD(pd.IDPERCENTUALEDISAGIO, db);
                                        }

                                    }

                                    db.Database.CurrentTransaction.Commit();
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

        public bool EsistonoMovimentiPrima(PercentualeDisagioModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEDISAGIO.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(PercentualeDisagioModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.PERCENTUALEDISAGIO.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(PercentualeDisagioModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.PERCENTUALEDISAGIO.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiPrimaUguale(PercentualeDisagioModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEDISAGIO.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
            }
        }

        public void DelPercentualeDisagio(decimal idPercDisagio)
        {
            PERCENTUALEDISAGIO precedenteIB = new PERCENTUALEDISAGIO();
            PERCENTUALEDISAGIO delIB = new PERCENTUALEDISAGIO();


            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.PERCENTUALEDISAGIO.Where(a => a.IDPERCENTUALEDISAGIO == idPercDisagio);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.PERCENTUALEDISAGIO.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new PERCENTUALEDISAGIO()
                            {
                                IDUFFICIO = precedenteIB.IDUFFICIO,

                                DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                //DATAFINEVALIDITA = Utility.DataFineStop(),
                                PERCENTUALE = precedenteIB.PERCENTUALE,

                                ANNULLATO = false
                            };

                            db.PERCENTUALEDISAGIO.Add(ibOld1);

                            db.SaveChanges();

                            using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                            {

                                dtrp.AssociaIndenita_PD(ibOld1.IDPERCENTUALEDISAGIO, db);

                            }

                            using (objLogAttivita log = new objLogAttivita())
                            {
                                log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di percentuale di disagio.", "PERCENTUALEDISAGIO", idPercDisagio);
                            }
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

        public decimal Get_Id_PercentualeDisaggioNonAnnullato(decimal idLivello)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEDISAGIO> libm = new List<PERCENTUALEDISAGIO>();
                libm = db.PERCENTUALEDISAGIO.Where(a => a.ANNULLATO == false
                && a.IDUFFICIO == idLivello).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDPERCENTUALEDISAGIO;
            }
            return tmp;
        }
        //
        public IList<PercentualeDisagioModel> getListPercentualeDisaggio(decimal idUfficio, bool escludiAnnullati = false)
        {
            List<PercentualeDisagioModel> libm = new List<PercentualeDisagioModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<PERCENTUALEDISAGIO> lib = new List<PERCENTUALEDISAGIO>();
                    if (escludiAnnullati == true)
                        lib = db.PERCENTUALEDISAGIO.Where(a => a.IDUFFICIO == idUfficio && a.ANNULLATO == false).ToList();
                    else
                        lib = db.PERCENTUALEDISAGIO.Where(a => a.IDUFFICIO == idUfficio).ToList();

                    libm = (from e in lib
                            select new PercentualeDisagioModel()
                            {
                                idPercentualeDisagio = e.IDPERCENTUALEDISAGIO,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                percentuale = e.PERCENTUALE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                Ufficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    descUfficio = e.UFFICI.DESCRIZIONEUFFICIO
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
        public bool PercentualeDisaggioAnnullato(PercentualeDisagioModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEDISAGIO.Where(a => a.IDUFFICIO == ibm.idUfficio && a.IDPERCENTUALEDISAGIO == ibm.idPercentualeDisagio).First().ANNULLATO == true ? true : false;
            }
        }
        public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione, decimal idUfficio)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEDISAGIO> libm = new List<PERCENTUALEDISAGIO>();
                libm = db.PERCENTUALEDISAGIO.Where(a => a.ANNULLATO == false
                && a.IDUFFICIO == idUfficio).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().Where(b => DataCampione == b.DATAINIZIOVALIDITA &&
                 b.DATAFINEVALIDITA != Utility.DataFineStop()).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCENTUALEDISAGIO.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALE.ToString());
                }
            }
            return tmp;
        }
        public void RendiAnnullatoUnRecord(decimal idCoeffSede, ModelDBISE db)
        {
            PERCENTUALEDISAGIO entita = new PERCENTUALEDISAGIO();
            entita = db.PERCENTUALEDISAGIO.Find(idCoeffSede);
            entita.ANNULLATO = true;
            db.SaveChanges();
        }
        public List<string> RestituisciLaRigaMassima(decimal idUfficio)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEDISAGIO> libm = new List<PERCENTUALEDISAGIO>();
                libm = db.PERCENTUALEDISAGIO.Where(a => a.ANNULLATO == false
                && a.IDUFFICIO == idUfficio).ToList().Where(b =>
                b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop())).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCENTUALEDISAGIO.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALE.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione, decimal idUfficio)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEDISAGIO> libm = new List<PERCENTUALEDISAGIO>();
                libm = db.PERCENTUALEDISAGIO.Where(a => a.ANNULLATO == false
                && a.IDUFFICIO == idUfficio).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().
                Where(b => DataCampione == b.DATAFINEVALIDITA
                && b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())).ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCENTUALEDISAGIO.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALE.ToString());
                }
            }
            return tmp;
        }
        public List<string> RestituisciIntervalloDiUnaData(DateTime DataCampione, decimal idUfficio)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEDISAGIO> libm = new List<PERCENTUALEDISAGIO>();
                libm = db.PERCENTUALEDISAGIO.Where(a => a.ANNULLATO == false
                && a.IDUFFICIO == idUfficio).ToList().Where(b =>
                b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())
                && DataCampione > b.DATAINIZIOVALIDITA
                && DataCampione < b.DATAFINEVALIDITA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCENTUALEDISAGIO.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALE.ToString());
                }
            }
            return tmp;
        }
        public static ValidationResult VerificaPercentuale(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as PercentualeDisagioModel;

            if (fm != null)
            {
                if (fm.percentuale > 100)
                {
                    vr = new ValidationResult(string.Format("Impossibile inserire percentuale  maggiore di 100 ({0}).", fm.percentuale.ToString()));
                }
                else
                {
                    vr = ValidationResult.Success;
                }
            }
            else
            {
                vr = new ValidationResult("La percentuale è richiesta.");
            }
            return vr;
        }
    }
}
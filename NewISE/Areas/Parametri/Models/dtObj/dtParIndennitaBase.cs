using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using NewISE.Models.dtObj;
using NewISE.Models.DBModel.dtObj;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParIndennitaBase : IDisposable
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
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new IndennitaBaseModel().dataFineValidita,
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
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.INDENNITABASE.Where(a => a.IDLIVELLO == idLivello).ToList();

                    libm = (from e in lib
                            select new IndennitaBaseModel()
                            {
                                idIndennitaBase = e.IDINDENNITABASE,
                                idLivello = e.IDLIVELLO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new IndennitaBaseModel().dataFineValidita,
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
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.INDENNITABASE.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new IndennitaBaseModel()
                            {
                                idIndennitaBase = e.IDINDENNITABASE,
                                idLivello = e.IDLIVELLO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new IndennitaBaseModel().dataFineValidita,
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
                using (ModelDBISE db = new ModelDBISE())
                {
                    bool Allincluded = true;
                    if (escludiAnnullati) Allincluded = false;
                    List<INDENNITABASE> lib = new List<INDENNITABASE>();
                    if (!Allincluded)
                        lib = db.INDENNITABASE.Where(a => a.IDLIVELLO == idLivello && a.ANNULLATO == false).ToList();
                    else
                        lib = db.INDENNITABASE.Where(a => a.IDLIVELLO == idLivello).ToList();

                    libm = (from e in lib
                            select new IndennitaBaseModel()
                            {
                                idIndennitaBase = e.IDINDENNITABASE,
                                idLivello = e.IDLIVELLO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new IndennitaBaseModel().dataFineValidita,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ibm"></param>
        public void SetIndennitaDiBase_000(IndennitaBaseModel ibm)
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
                                DATAFINEVALIDITA = Utility.DataFineStop(),
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
                            DATAFINEVALIDITA = Utility.DataFineStop(),
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
                                        IDLIVELLO = item.IDLIVELLO,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        VALORE = item.VALORE,
                                        VALORERESP = item.VALORERESP,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);

                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new INDENNITABASE()
                                    {
                                        IDLIVELLO = item.IDLIVELLO,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        VALORE = item.VALORE,
                                        VALORERESP = item.VALORERESP,
                                        ANNULLATO = false
                                    };

                                    var ibOld2 = new INDENNITABASE()
                                    {
                                        IDLIVELLO = item.IDLIVELLO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(+1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        VALORE = item.VALORE,
                                        VALORERESP = item.VALORERESP,
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
                                        IDLIVELLO = item.IDLIVELLO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        VALORE = item.VALORE,
                                        VALORERESP = item.VALORERESP,
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
                                        IDLIVELLO = item.IDLIVELLO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        VALORE = item.VALORE,
                                        VALORERESP = item.VALORERESP,
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
                        ///////////////////////////////////////// ///////////////////////////////////////// ////Jean Rick////////////////////////////////////////
                        var entitaLista = db.INDENNITABASE.Where(a => a.ANNULLATO == false && a.IDLIVELLO == ibNew.IDLIVELLO).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                        if (entitaLista.Count != 0)
                        {
                            var entita = entitaLista.First();
                            var ibInMezzo = new INDENNITABASE()
                            {
                                IDLIVELLO = ibNew.IDLIVELLO,
                                DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                DATAFINEVALIDITA = entita.DATAINIZIOVALIDITA.AddDays(-1),
                                VALORE = 0,
                                VALORERESP = 0,
                                ANNULLATO = false
                            };
                            db.INDENNITABASE.Add(ibInMezzo);
                        }
                        //////////////////////////////////////////////////////////////////////////////////////fine Jean Rick/////////////////////////////////////////                        
                    }
                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
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


        public void SetIndennitaDiBase(IndennitaBaseModel ibm, bool aggiornaTutto)
        {
            List<INDENNITABASE> libNew = new List<INDENNITABASE>();

            //INDENNITABASE ibPrecedente = new INDENNITABASE();
            INDENNITABASE ibNew1 = new INDENNITABASE();
            INDENNITABASE ibNew2 = new INDENNITABASE();
            //List<INDENNITABASE> lArchivioIB = new List<INDENNITABASE>();
            List<string> lista = new List<string>();

            DateTime dataVariazione = ibm.dataInizioValidita;


            using (ModelDBISE db = new ModelDBISE())
            {
                bool giafatta = false;
                try
                {                   
                    using (dtParIndennitaBase dtal = new dtParIndennitaBase())
                    {
                        //Se la data variazione coincide con una data inizio esistente
                        lista = dtal.DataVariazioneCoincideConDataInizio(ibm.dataInizioValidita, ibm.idLivello);
                        if (lista.Count != 0)
                        {
                            giafatta = true;
                            decimal idIntervalloFirst = Convert.ToDecimal(lista[0]);
                            DateTime dataInizioFirst = Convert.ToDateTime(lista[1]);
                            DateTime dataFineFirst = Convert.ToDateTime(lista[2]);
                            //   decimal aliquotaFirst = Convert.ToDecimal(lista[3]);
                            //decimal valoreFirst = Convert.ToDecimal(lista[3]);
                            //decimal valoreRespFirst = Convert.ToDecimal(lista[4]);

                            ibNew1 = new INDENNITABASE()
                            {
                                DATAINIZIOVALIDITA = dataInizioFirst,
                                DATAFINEVALIDITA = dataFineFirst,
                                VALORE = ibm.valore,
                                VALORERESP = ibm.valoreResponsabile,
                                DATAAGGIORNAMENTO = DateTime.Now,
                                IDLIVELLO = ibm.idLivello,
                            };

                            if (aggiornaTutto)
                            {
                                ibNew1 = new INDENNITABASE()
                                {
                                    DATAINIZIOVALIDITA = dataInizioFirst,
                                    DATAFINEVALIDITA = Utility.DataFineStop(),
                                    VALORE = ibm.valore,
                                    VALORERESP = ibm.valoreResponsabile,
                                    // ALIQUOTA = ibm.aliquota,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    IDLIVELLO = ibm.idLivello,
                                };
                                //qui annullo tutti i record rimanenti dalla data inizio inserita
                                libNew = db.INDENNITABASE.Where(a => a.ANNULLATO == false && a.IDLIVELLO == ibm.idLivello).ToList().Where(a => a.DATAINIZIOVALIDITA > dataInizioFirst).ToList();
                                foreach (var elem in libNew)
                                {
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDINDENNITABASE), db);
                                }
                            }
                            db.Database.BeginTransaction();
                            db.INDENNITABASE.Add(ibNew1);
                            db.SaveChanges();
                            RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloFirst), db);

                            using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                            {
                                dtrp.AssociaIndennitaBase_IB(ibNew1.IDINDENNITABASE, db, ibm.dataInizioValidita);
                                dtrp.AssociaRiduzioniIB(ibNew1.IDINDENNITABASE, db, ibm.dataInizioValidita);
                            }

                            db.Database.CurrentTransaction.Commit();
                        }
                        ///se la data variazione coincide con una data fine esistente(diversa da 31/12/9999)
                        if (giafatta == false)
                        {
                            lista = dtal.DataVariazioneCoincideConDataFine(ibm.dataInizioValidita, ibm.idLivello);
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervalloLast = Convert.ToDecimal(lista[0]);
                                DateTime dataInizioLast = Convert.ToDateTime(lista[1]);
                                DateTime dataFineLast = Convert.ToDateTime(lista[2]);
                                decimal valoreLast = Convert.ToDecimal(lista[3]);
                                decimal valoreRespLast = Convert.ToDecimal(lista[4]);

                                ibNew1 = new INDENNITABASE()
                                {
                                    DATAINIZIOVALIDITA = dataInizioLast,
                                    DATAFINEVALIDITA = dataFineLast.AddDays(-1),
                                    VALORE = valoreLast,
                                    VALORERESP = valoreRespLast,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    IDLIVELLO = ibm.idLivello,
                                };
                                ibNew2 = new INDENNITABASE()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = ibm.dataInizioValidita,//è uguale alla data Inizio
                                    VALORE = ibm.valore,
                                    VALORERESP = ibm.valoreResponsabile,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    IDLIVELLO = ibm.idLivello,
                                };
                                if (aggiornaTutto)
                                {
                                    ibNew2 = new INDENNITABASE()
                                    {
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        VALORE = ibm.valore,
                                        VALORERESP = ibm.valoreResponsabile,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        IDLIVELLO = ibm.idLivello,
                                    };
                                    libNew = db.INDENNITABASE.Where(a => a.ANNULLATO == false && a.IDLIVELLO == ibm.idLivello).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDINDENNITABASE), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                db.Database.BeginTransaction();
                                db.INDENNITABASE.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloLast), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var ib in libNew)
                                    {
                                        dtrp.AssociaIndennitaBase_IB(ib.IDINDENNITABASE, db, ibm.dataInizioValidita);
                                        dtrp.AssociaRiduzioniIB(ib.IDINDENNITABASE, db, ibm.dataInizioValidita);
                                    }

                                }

                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //Se il nuovo record si trova in un intervallo non annullato con data fine non uguale al 31/12/9999
                        if (giafatta == false)
                        {
                            lista = dtal.RestituisciIntervalloDiUnaData(ibm.dataInizioValidita, ibm.idLivello);
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervallo = Convert.ToDecimal(lista[0]);
                                DateTime dataInizio = Convert.ToDateTime(lista[1]);
                                DateTime dataFine = Convert.ToDateTime(lista[2]);
                                //decimal aliquota = Convert.ToDecimal(lista[3]);
                                decimal valore = Convert.ToDecimal(lista[3]);
                                decimal valoreResp = Convert.ToDecimal(lista[4]);

                                DateTime NewdataFine1 = ibm.dataInizioValidita.AddDays(-1);

                                ibNew1 = new INDENNITABASE()
                                {
                                    DATAINIZIOVALIDITA = dataInizio,
                                    DATAFINEVALIDITA = NewdataFine1,
                                    // ALIQUOTA = aliquota,
                                    VALORE = valore,
                                    VALORERESP = valoreResp,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    IDLIVELLO = ibm.idLivello,
                                };
                                ibNew2 = new INDENNITABASE()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = dataFine,
                                    // ALIQUOTA = ibm.aliquota,
                                    VALORE = ibm.valore,
                                    VALORERESP = ibm.valoreResponsabile,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    IDLIVELLO = ibm.idLivello,
                                };

                                if (aggiornaTutto)
                                {
                                    ibNew2 = new INDENNITABASE()
                                    {
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        // ALIQUOTA = ibm.aliquota,
                                        VALORE = ibm.valore,
                                        VALORERESP = ibm.valoreResponsabile,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        IDLIVELLO = ibm.idLivello,
                                    };
                                    libNew = db.INDENNITABASE.Where(a => a.ANNULLATO == false && a.LIVELLI.IDLIVELLO == ibm.idLivello).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDINDENNITABASE), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                db.Database.BeginTransaction();
                                db.INDENNITABASE.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervallo), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var ib in libNew)
                                    {
                                        dtrp.AssociaIndennitaBase_IB(ib.IDINDENNITABASE, db, ibm.dataInizioValidita);
                                        dtrp.AssociaRiduzioniIB(ib.IDINDENNITABASE, db, ibm.dataInizioValidita);
                                    }

                                }

                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //CASO DELL'ULTIMA RIGA CON LA DATA FINE UGUALE A 31/12/9999
                        if (giafatta == false)
                        {
                            lista = dtal.RestituisciLaRigaMassima(ibm.idLivello);
                            //Attenzione qui se la lista non contiene nessun elemento
                            //significa che non esiste nessun elemento corrispondentemente al livello selezionato
                            if (lista.Count == 0)
                            {
                                ibNew1 = new INDENNITABASE()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = Convert.ToDateTime(Utility.DataFineStop()),
                                    VALORE = ibm.valore,
                                    VALORERESP = ibm.valoreResponsabile,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    IDLIVELLO = ibm.idLivello,
                                };
                                libNew.Add(ibNew1);
                                db.Database.BeginTransaction();
                                db.INDENNITABASE.Add(ibNew1);
                                db.SaveChanges();
                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    dtrp.AssociaIndennitaBase_IB(ibNew1.IDINDENNITABASE, db, ibm.dataInizioValidita);
                                    dtrp.AssociaRiduzioniIB(ibNew1.IDINDENNITABASE, db, ibm.dataInizioValidita);

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
                                //decimal aliquotaUltimo = Convert.ToDecimal(lista[3]);
                                decimal valoreUltimo = Convert.ToDecimal(lista[3]);
                                decimal valoreRespUltimo = Convert.ToDecimal(lista[4]);

                                if (dataInizioUltimo == ibm.dataInizioValidita)
                                {
                                    ibNew1 = new INDENNITABASE()
                                    {
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = dataFineUltimo,
                                        //ALIQUOTA = ibm.aliquota,//nuova aliquota rispetto alla vecchia registrata
                                        VALORE = ibm.valore,
                                        VALORERESP = ibm.valoreResponsabile,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.INDENNITABASE.Add(ibNew1);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        dtrp.AssociaIndennitaBase_IB(ibNew1.IDINDENNITABASE, db, ibm.dataInizioValidita);
                                        dtrp.AssociaRiduzioniIB(ibNew1.IDINDENNITABASE, db, ibm.dataInizioValidita);

                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                                //se il nuovo record rappresenta la data variazione superiore alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                if (ibm.dataInizioValidita > dataInizioUltimo)
                                {
                                    ibNew1 = new INDENNITABASE()
                                    {
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = ibm.dataInizioValidita.AddDays(-1),
                                        // ALIQUOTA = aliquotaUltimo,
                                        VALORE = valoreUltimo,
                                        VALORERESP = valoreRespUltimo,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        IDLIVELLO = ibm.idLivello,
                                    };
                                    ibNew2 = new INDENNITABASE()
                                    {
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        //ALIQUOTA = ibm.aliquota,//nuova aliquota rispetto alla vecchia registrata
                                        VALORE = ibm.valore,
                                        VALORERESP = ibm.valoreResponsabile,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        IDLIVELLO = ibm.idLivello,
                                    };
                                    libNew.Add(ibNew1); libNew.Add(ibNew2);
                                    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                    db.Database.BeginTransaction();
                                    db.INDENNITABASE.AddRange(libNew);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        foreach (var ib in libNew)
                                        {
                                            dtrp.AssociaIndennitaBase_IB(ib.IDINDENNITABASE, db, ibm.dataInizioValidita);
                                            dtrp.AssociaRiduzioniIB(ib.IDINDENNITABASE, db, ibm.dataInizioValidita);
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


        public bool EsistonoMovimentiPrima(IndennitaBaseModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.INDENNITABASE.Where(a => a.ANNULLATO == false && a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDLIVELLO == ibm.idLivello).Count() > 0 ? true : false;
            }
        }
        public bool IndennitaBaseAnnullato(IndennitaBaseModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.INDENNITABASE.Where(a => a.IDLIVELLO == ibm.idLivello && a.IDINDENNITABASE == ibm.idIndennitaBase).First().ANNULLATO == true ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(IndennitaBaseModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.INDENNITABASE.Where(a => a.ANNULLATO == false && a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDLIVELLO == ibm.idLivello).Count() > 0 ? true : false;
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
                    return db.INDENNITABASE.Where(a => a.ANNULLATO == false && a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDLIVELLO == ibm.idLivello).Count() > 0 ? true : false;
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
                return db.INDENNITABASE.Where(a => a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDLIVELLO == ibm.idLivello).Count() > 0 ? true : false;
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

                        var lprecIB = db.INDENNITABASE.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA
                        && a.ANNULLATO == false && a.IDLIVELLO == delIB.IDLIVELLO).OrderByDescending(a => a.IDINDENNITABASE).ToList();//corretto

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;
                            delIB.ANNULLATO = true;//corretto
                            var ibOld1 = new INDENNITABASE()
                            {
                                IDLIVELLO = precedenteIB.IDLIVELLO,
                                // DATAINIZIOVALIDITA = precedenteIB.DATAFINEVALIDITA,
                                DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,//corretto
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                VALORE = precedenteIB.VALORE,
                                VALORERESP = precedenteIB.VALORERESP,
                                ANNULLATO = false
                            };

                            db.INDENNITABASE.Add(ibOld1);

                            db.SaveChanges();

                            using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                            {
                                dtrp.AssociaIndennitaBase_IB(ibOld1.IDINDENNITABASE, db, delIB.DATAINIZIOVALIDITA);
                                dtrp.AssociaRiduzioniIB(ibOld1.IDINDENNITABASE, db, delIB.DATAINIZIOVALIDITA);
                            }
                        }



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
        // Get_Id_IndennitaBaseNonAnnullato
        public decimal Get_Id_IndennitaBaseNonAnnullato(decimal idLivello)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<INDENNITABASE> libm = new List<INDENNITABASE>();
                libm = db.INDENNITABASE.Where(a => a.ANNULLATO == false
                && a.IDLIVELLO == idLivello).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDINDENNITABASE;
            }
            return tmp;
        }

        public INDENNITABASE RestituisciIlRecordPrecedente(decimal idIndennitaBase, decimal idLivello)
        {
            INDENNITABASE tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                INDENNITABASE interessato = new INDENNITABASE();
                interessato = db.INDENNITABASE.Find(idIndennitaBase);
                tmp = db.INDENNITABASE.Where(a => a.ANNULLATO == false
                && a.IDLIVELLO == idLivello).ToList().Where(b => b.DATAFINEVALIDITA == interessato.DATAINIZIOVALIDITA.AddDays(-1)).ToList().First();
            }
            return tmp;
        }
        public List<string> RestituisciIntervalloDiUnaData(DateTime DataCampione, decimal idLivello)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<INDENNITABASE> libm = new List<INDENNITABASE>();
                libm = db.INDENNITABASE.Where(a => a.ANNULLATO == false
                && a.IDLIVELLO == idLivello).ToList().Where(b =>
                 b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())
                 && DataCampione > b.DATAINIZIOVALIDITA
                 && DataCampione < b.DATAFINEVALIDITA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDINDENNITABASE.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].VALORE.ToString());
                    tmp.Add(libm[0].VALORERESP.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione, decimal idLivello)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<INDENNITABASE> libm = new List<INDENNITABASE>();
                libm = db.INDENNITABASE.Where(a => a.ANNULLATO == false
                && a.IDLIVELLO == idLivello).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().Where(b => DataCampione == b.DATAINIZIOVALIDITA).ToList();// &&
                  //b.DATAFINEVALIDITA != Utility.DataFineStop()).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDINDENNITABASE.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].VALORE.ToString());
                    tmp.Add(libm[0].VALORERESP.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione, decimal idLivello)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<INDENNITABASE> libm = new List<INDENNITABASE>();
                libm = db.INDENNITABASE.Where(a => a.ANNULLATO == false
                && a.IDLIVELLO == idLivello).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().
                Where(b => DataCampione == b.DATAFINEVALIDITA
                && b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())).ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDINDENNITABASE.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].VALORE.ToString());
                    tmp.Add(libm[0].VALORERESP.ToString());
                }
            }
            return tmp;
        }
        public List<string> RestituisciLaRigaMassima(decimal idLivello)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<INDENNITABASE> libm = new List<INDENNITABASE>();
                libm = db.INDENNITABASE.Where(a => a.ANNULLATO == false
                && a.IDLIVELLO == idLivello).ToList().Where(b =>
                 b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop())).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDINDENNITABASE.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].VALORE.ToString());
                    tmp.Add(libm[0].VALORERESP.ToString());
                }
            }
            return tmp;
        }
        public void RendiAnnullatoUnRecord(decimal idIdennitaBase, ModelDBISE db)
        {
            INDENNITABASE entita = new INDENNITABASE();
            entita = db.INDENNITABASE.Find(idIdennitaBase);
            entita.ANNULLATO = true;
            db.SaveChanges();
        }

    }
}
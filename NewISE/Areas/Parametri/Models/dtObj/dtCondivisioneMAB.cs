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
    public class dtCondivisioneMAB : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<percCondivisioneMABModel> getListIndennitaPrimoSegretario()
        {
            List<percCondivisioneMABModel> libm = new List<percCondivisioneMABModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALECONDIVISIONE.ToList();

                    libm = (from e in lib
                            select new percCondivisioneMABModel()
                            {
                                idPercCond = e.IDPERCCOND,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                Percentuale = e.PERCENTUALE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO
                            }).ToList();
                }
                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<percCondivisioneMABModel> getListIndennitaPrimoSegretario(decimal idPercCond)
        {
            List<percCondivisioneMABModel> libm = new List<percCondivisioneMABModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALECONDIVISIONE.Where(a => a.IDPERCCOND == idPercCond).ToList();

                    libm = (from e in lib
                            select new percCondivisioneMABModel()
                            {
                                idPercCond = e.IDPERCCOND,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,// != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new percCondivisioneMABModel().dataFineValidita,
                                Percentuale = e.PERCENTUALE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO
                            }).ToList();
                }
                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<percCondivisioneMABModel> getListCondivisioneMAB(bool escludiAnnullati = false)
        {
            List<percCondivisioneMABModel> libm = new List<percCondivisioneMABModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<PERCENTUALECONDIVISIONE> lib = new List<PERCENTUALECONDIVISIONE>();
                    if (escludiAnnullati == true)
                        lib = db.PERCENTUALECONDIVISIONE.Where(a => a.ANNULLATO == false).ToList();
                    else
                        lib = db.PERCENTUALECONDIVISIONE.ToList();

                    libm = (from e in lib
                            select new percCondivisioneMABModel()
                            {
                                idPercCond = e.IDPERCCOND,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                Percentuale = e.PERCENTUALE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO
                            }).OrderBy(a => a.dataInizioValidita).ToList();
                }
                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<percCondivisioneMABModel> getListIndennitaPrimoSegretario(decimal idPercCond, bool escludiAnnullati = false)
        {
            List<percCondivisioneMABModel> libm = new List<percCondivisioneMABModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<PERCENTUALECONDIVISIONE> lib = new List<PERCENTUALECONDIVISIONE>();
                    if (escludiAnnullati == true)
                        lib = db.PERCENTUALECONDIVISIONE.Where(a => a.IDPERCCOND == idPercCond && a.ANNULLATO == false).ToList();
                    else
                        lib = db.PERCENTUALECONDIVISIONE.Where(a => a.IDPERCCOND == idPercCond).ToList();

                    libm = (from e in lib
                            select new percCondivisioneMABModel()
                            {
                                idPercCond = e.IDPERCCOND,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = Utility.DataFineStop(),
                                Percentuale = e.PERCENTUALE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO
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

        public bool EsistonoMovimentiPrima(percCondivisioneMABModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALECONDIVISIONE.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDPERCCOND == ibm.idPercCond).Count() > 0 ? true : false;
            }
        }
        public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALECONDIVISIONE> libm = new List<PERCENTUALECONDIVISIONE>();
                libm = db.PERCENTUALECONDIVISIONE.Where(a => a.ANNULLATO == false).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().
                Where(b => DataCampione == b.DATAFINEVALIDITA
                && b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())).ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCCOND.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALE.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALECONDIVISIONE> libm = new List<PERCENTUALECONDIVISIONE>();
                libm = db.PERCENTUALECONDIVISIONE.Where(a => a.ANNULLATO == false).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().Where(b => DataCampione == b.DATAINIZIOVALIDITA &&
                 b.DATAFINEVALIDITA != Utility.DataFineStop()).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCCOND.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALE.ToString());
                }
            }
            return tmp;
        }
        public void SetCondivisioneMAB(percCondivisioneMABModel ibm, bool aggiornaTutto)
        {
            List<PERCENTUALECONDIVISIONE> libNew = new List<PERCENTUALECONDIVISIONE>();

            //PERCENTUALECONDIVISIONE ibPrecedente = new PERCENTUALECONDIVISIONE();
            PERCENTUALECONDIVISIONE ibNew1 = new PERCENTUALECONDIVISIONE();
            PERCENTUALECONDIVISIONE ibNew2 = new PERCENTUALECONDIVISIONE();
            //List<PERCENTUALECONDIVISIONE> lArchivioIB = new List<PERCENTUALECONDIVISIONE>();
            List<string> lista = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                bool giafatta = false;
                try
                {
                    using (dtCondivisioneMAB dtal = new dtCondivisioneMAB())
                    {
                        //Se la data variazione coincide con una data inizio esistente
                        lista = dtal.DataVariazioneCoincideConDataInizio(ibm.dataInizioValidita);
                        if (lista.Count != 0)
                        {
                            giafatta = true;
                            decimal idIntervalloFirst = Convert.ToDecimal(lista[0]);
                            DateTime dataInizioFirst = Convert.ToDateTime(lista[1]);
                            DateTime dataFineFirst = Convert.ToDateTime(lista[2]);
                            //decimal COEFFICIENTERICHIAMO = Convert.ToDecimal(lista[3]);
                            //   decimal COEFFICIENTEINDBASE = Convert.ToDecimal(lista[4]);

                            ibNew1 = new PERCENTUALECONDIVISIONE()
                            {
                                DATAINIZIOVALIDITA = dataInizioFirst,
                                DATAFINEVALIDITA = dataFineFirst,
                                PERCENTUALE = ibm.Percentuale,
                                DATAAGGIORNAMENTO = DateTime.Now,
                            };

                            if (aggiornaTutto)
                            {
                                ibNew1 = new PERCENTUALECONDIVISIONE()
                                {
                                    DATAINIZIOVALIDITA = dataInizioFirst,
                                    DATAFINEVALIDITA = Utility.DataFineStop(),
                                    PERCENTUALE = ibm.Percentuale,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                //qui annullo tutti i record rimanenti dalla data inizio inserita
                                libNew = db.PERCENTUALECONDIVISIONE.Where(a => a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > dataInizioFirst).ToList();
                                foreach (var elem in libNew)
                                {
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPERCCOND), db);
                                }
                            }
                            db.Database.BeginTransaction();
                            db.PERCENTUALECONDIVISIONE.Add(ibNew1);
                            db.SaveChanges();
                            RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloFirst), db);

                            using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                            {
                                dtrp.AssociaPagatoCondivisoMAB(ibNew1.IDPERCCOND, db);
                            }

                            db.Database.CurrentTransaction.Commit();
                        }
                        ///se la data variazione coincide con una data fine esistente(diversa da 31/12/9999)
                        if (giafatta == false)
                        {
                            lista = dtal.DataVariazioneCoincideConDataFine(ibm.dataInizioValidita);
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervalloLast = Convert.ToDecimal(lista[0]);
                                DateTime dataInizioLast = Convert.ToDateTime(lista[1]);
                                DateTime dataFineLast = Convert.ToDateTime(lista[2]);
                                decimal aliquotaLast = Convert.ToDecimal(lista[3]);

                                ibNew1 = new PERCENTUALECONDIVISIONE()
                                {
                                    DATAINIZIOVALIDITA = dataInizioLast,
                                    DATAFINEVALIDITA = dataFineLast.AddDays(-1),
                                    //COEFFICIENTEKM = aliquotaLast,
                                    PERCENTUALE = aliquotaLast,
                                };
                                ibNew2 = new PERCENTUALECONDIVISIONE()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = ibm.dataInizioValidita,//è uguale alla data Inizio
                                    //COEFFICIENTEKM = ibm.coefficienteKm,
                                    PERCENTUALE = ibm.Percentuale,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };
                                if (aggiornaTutto)
                                {
                                    ibNew2 = new PERCENTUALECONDIVISIONE()
                                    {
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        // COEFFICIENTEKM = ibm.coefficienteKm, 
                                        PERCENTUALE = ibm.Percentuale,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.PERCENTUALECONDIVISIONE.Where(a => a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPERCCOND), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                db.Database.BeginTransaction();
                                db.PERCENTUALECONDIVISIONE.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloLast), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var pc in libNew)
                                    {
                                        dtrp.AssociaPagatoCondivisoMAB(pc.IDPERCCOND, db);
                                    }

                                }

                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //Se il nuovo record si trova in un intervallo non annullato con data fine non uguale al 31/12/9999
                        if (giafatta == false)
                        {

                            lista = dtal.RestituisciIntervalloDiUnaData(ibm.dataInizioValidita);
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervallo = Convert.ToDecimal(lista[0]);
                                DateTime dataInizio = Convert.ToDateTime(lista[1]);
                                DateTime dataFine = Convert.ToDateTime(lista[2]);
                                //  decimal aliquota = Convert.ToDecimal(lista[3]);
                                decimal Percentuale = Convert.ToDecimal(lista[3]);
                                //  decimal COEFFICIENTEINDBASE = Convert.ToDecimal(lista[4]);
                                DateTime NewdataFine1 = ibm.dataInizioValidita.AddDays(-1);

                                ibNew1 = new PERCENTUALECONDIVISIONE()
                                {
                                    DATAINIZIOVALIDITA = dataInizio,
                                    DATAFINEVALIDITA = NewdataFine1,
                                    // COEFFICIENTEKM = aliquota,
                                    PERCENTUALE = Percentuale,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new PERCENTUALECONDIVISIONE()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = dataFine,
                                    //COEFFICIENTEKM = ibm.coefficienteKm,
                                    PERCENTUALE = ibm.Percentuale,
                                    //  COEFFICIENTEINDBASE = ibm.coefficienteIndBase,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };

                                if (aggiornaTutto)
                                {
                                    ibNew2 = new PERCENTUALECONDIVISIONE()
                                    {
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        // COEFFICIENTEKM = ibm.coefficienteKm,
                                        PERCENTUALE = ibm.Percentuale,
                                        // COEFFICIENTEINDBASE = ibm.coefficienteIndBase,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.PERCENTUALECONDIVISIONE.Where(a => a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPERCCOND), db);
                                    }
                                }
                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                db.Database.BeginTransaction();
                                db.PERCENTUALECONDIVISIONE.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervallo), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var pc in libNew)
                                    {
                                        dtrp.AssociaPagatoCondivisoMAB(pc.IDPERCCOND, db);
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
                            lista = dtal.RestituisciLaRigaMassima();
                            if (lista.Count == 0)
                            {
                                ibNew1 = new PERCENTUALECONDIVISIONE()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = Convert.ToDateTime(Utility.DataFineStop()),
                                    PERCENTUALE = ibm.Percentuale,
                                    DATAAGGIORNAMENTO = DateTime.Now,

                                };
                                libNew.Add(ibNew1);
                                db.Database.BeginTransaction();
                                db.PERCENTUALECONDIVISIONE.Add(ibNew1);
                                db.SaveChanges();

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {

                                    dtrp.AssociaPagatoCondivisoMAB(ibNew1.IDPERCCOND, db);

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
                                //  decimal aliquotaUltimo = Convert.ToDecimal(lista[3]);
                                decimal PERCENTUALE_Ultimo = Convert.ToDecimal(lista[3]);
                                // decimal COEFFICIENTEINDBASE_Ultimo = Convert.ToDecimal(lista[4]);

                                if (dataInizioUltimo == ibm.dataInizioValidita)
                                {
                                    ibNew1 = new PERCENTUALECONDIVISIONE()
                                    {
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = dataFineUltimo,
                                        // COEFFICIENTEKM = ibm.coefficienteKm,//nuova aliquota rispetto alla vecchia registrata
                                        PERCENTUALE = ibm.Percentuale,
                                        //  COEFFICIENTEINDBASE = ibm.coefficienteIndBase,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.PERCENTUALECONDIVISIONE.Add(ibNew1);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        dtrp.AssociaPagatoCondivisoMAB(ibNew1.IDPERCCOND, db);
                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                                //se il nuovo record rappresenta la data variazione superiore alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                if (ibm.dataInizioValidita > dataInizioUltimo)
                                {
                                    ibNew1 = new PERCENTUALECONDIVISIONE()
                                    {
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = ibm.dataInizioValidita.AddDays(-1),
                                        // COEFFICIENTEKM = aliquotaUltimo,
                                        PERCENTUALE = PERCENTUALE_Ultimo,
                                        //  COEFFICIENTEINDBASE = COEFFICIENTEINDBASE_Ultimo,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    ibNew2 = new PERCENTUALECONDIVISIONE()
                                    {

                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        // COEFFICIENTEKM = ibm.coefficienteKm,//nuova aliquota rispetto alla vecchia registrata
                                        PERCENTUALE = ibm.Percentuale,
                                        //  COEFFICIENTEINDBASE = ibm.coefficienteIndBase,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1); libNew.Add(ibNew2);
                                    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                    db.Database.BeginTransaction();
                                    db.PERCENTUALECONDIVISIONE.AddRange(libNew);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        foreach (var pc in libNew)
                                        {
                                            dtrp.AssociaPagatoCondivisoMAB(pc.IDPERCCOND, db);
                                        }

                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                            }
                        }
                        //INSERIMENTO DATI NELLA TABELLA CIR_R PER LE RELAZIONI MOLTI A MOLTI
                        decimal idPercCond = ibNew1.IDPERCCOND;
                        //   decimal idRiduzione1 = dataInizioValiditaAccettataPerRiduzione(ibNew1.DATAINIZIOVALIDITA).First().idRiduzioni;

                        //   this.Associa_Riduzione_CoeffIndRichiamo(idRiduzione1, idCoeffIndRichiamo1, db);

                        //if (ibNew2.IDCOEFINDRICHIAMO != 0)
                        //{
                        //    decimal idCoeffIndRichiamo2 = ibNew2.IDCOEFINDRICHIAMO;
                        //    decimal idRiduzione2 = dataInizioValiditaAccettataPerRiduzione(ibNew2.DATAINIZIOVALIDITA).First().idRiduzioni;
                        //    this.Associa_Riduzione_CoeffIndRichiamo(idRiduzione2, idCoeffIndRichiamo2, db);
                        //}
                    }
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }
        public static List<RiduzioniModel> dataInizioValiditaAccettataPerRiduzione(DateTime d)
        {
            List<RiduzioniModel> tmp = new List<RiduzioniModel>();
            using (dtRiduzioni dtRid = new dtRiduzioni())
            {
                tmp = dtRid.getListRiduzioni().Where(a => a.annullato == false).ToList().Where(b => d >= b.dataInizioValidita && d <= b.dataFineValidita).OrderBy(a => a.dataInizioValidita).ThenBy(b => b.dataFineValidita).ToList();
            }
            return tmp;
        }
        public List<string> RestituisciIntervalloDiUnaData(DateTime DataCampione)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALECONDIVISIONE> libm = new List<PERCENTUALECONDIVISIONE>();
                libm = db.PERCENTUALECONDIVISIONE.Where(a => a.ANNULLATO == false).ToList().Where(b =>
                b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())
                && DataCampione > b.DATAINIZIOVALIDITA
                && DataCampione < b.DATAFINEVALIDITA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCCOND.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALE.ToString());
                }
            }
            return tmp;
        }

        public PERCENTUALECONDIVISIONE RestituisciIlRecordPrecedente(decimal idPercCond)
        {
            PERCENTUALECONDIVISIONE tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                PERCENTUALECONDIVISIONE interessato = new PERCENTUALECONDIVISIONE();
                interessato = db.PERCENTUALECONDIVISIONE.Find(idPercCond);
                tmp = db.PERCENTUALECONDIVISIONE.Where(a => a.ANNULLATO == false).ToList().Where(b => b.DATAFINEVALIDITA == interessato.DATAINIZIOVALIDITA.AddDays(-1)).ToList().First();
            }
            return tmp;
        }
        public void DelIndennitaPrimoSegretario(decimal idPercCond)
        {
            PERCENTUALECONDIVISIONE precedenteIB = new PERCENTUALECONDIVISIONE();
            PERCENTUALECONDIVISIONE delIB = new PERCENTUALECONDIVISIONE();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    var lib = db.PERCENTUALECONDIVISIONE.Where(a => a.IDPERCCOND == idPercCond);
                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;
                        RendiAnnullatoUnRecord(delIB.IDPERCCOND, db);
                        precedenteIB = RestituisciIlRecordPrecedente(idPercCond);
                        RendiAnnullatoUnRecord(precedenteIB.IDPERCCOND, db);

                        var NuovoPrecedente = new PERCENTUALECONDIVISIONE()
                        {
                            DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                            // ALIQUOTA = precedenteIB.ALIQUOTA,
                            //COEFFICIENTERICHIAMO = precedenteIB.COEFFICIENTERICHIAMO,
                            //COEFFICIENTEINDBASE = precedenteIB.COEFFICIENTEINDBASE,
                            PERCENTUALE = precedenteIB.PERCENTUALE,
                            DATAAGGIORNAMENTO = DateTime.Now,// precedenteIB.DATAAGGIORNAMENTO,
                            ANNULLATO = false
                        };
                        db.PERCENTUALECONDIVISIONE.Add(NuovoPrecedente);

                        db.SaveChanges();

                        using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                        {
                            dtrp.AssociaPagatoCondivisoMAB(NuovoPrecedente.IDPERCCOND, db);
                        }

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di Idennità Primo Segretario.", "PERCENTUALECONDIVISIONE", idPercCond);
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

        public IList<percCondivisioneMABModel> getIndennitaPrimoSegretario()
        {
            List<percCondivisioneMABModel> llm = new List<percCondivisioneMABModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ll = db.PERCENTUALECONDIVISIONE.ToList();

                    llm = (from e in ll
                           select new percCondivisioneMABModel()
                           {
                               idPercCond = e.IDPERCCOND,
                               Percentuale = e.PERCENTUALE
                           }).ToList();
                }

                return llm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public percCondivisioneMABModel getIndennitaPrimoSegretario(decimal idPercCond)
        {
            percCondivisioneMABModel lm = new percCondivisioneMABModel();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var liv = db.PERCENTUALECONDIVISIONE.Find(idPercCond);

                    lm = new percCondivisioneMABModel()
                    {
                        idPercCond = liv.IDPERCCOND,
                        Percentuale = liv.PERCENTUALE
                    };
                }
                return lm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as percCondivisioneMABModel;

            if (fm != null)
            {
                DateTime d = DataInizioMinimaNonAnnullataIndennitaPrimoSegr();
                if (fm.dataInizioValidita < d)
                {
                    vr = new ValidationResult(string.Format("Impossibile inserire la data di inizio validità minore alla data di Base ({0}).", d.ToShortDateString()));
                }
                else
                {
                    //if(dataInizioValiditaAccettataPerRiduzione(fm.dataInizioValidita).Count!=0)
                    vr = ValidationResult.Success;
                    //else
                    //    vr = new ValidationResult(string.Format("Impossibile inserire la data di inizio validità non registrata in un intervallo nella Riduzione {0}.",""));
                }
            }
            else
            {
                vr = new ValidationResult("La data di inizio validità è richiesta.");
            }
            return vr;
        }
        public static DateTime DataInizioMinimaNonAnnullataIndennitaPrimoSegr()
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.PERCENTUALECONDIVISIONE.Where(a => a.ANNULLATO == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }
        public bool IndCondivisioneMABAnnullato(percCondivisioneMABModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALECONDIVISIONE.Where(a => a.IDPERCCOND == ibm.idPercCond).First().ANNULLATO == true ? true : false;
            }
        }

        public decimal Get_Id_IndCondivisioneMABAnnullato()
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALECONDIVISIONE> libm = new List<PERCENTUALECONDIVISIONE>();
                libm = db.PERCENTUALECONDIVISIONE.Where(a => a.ANNULLATO == false).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDPERCCOND;
            }
            return tmp;
        }
        public void RendiAnnullatoUnRecord(decimal idIdennitaPrimoSegr, ModelDBISE db)
        {
            PERCENTUALECONDIVISIONE entita = new PERCENTUALECONDIVISIONE();
            entita = db.PERCENTUALECONDIVISIONE.Find(idIdennitaPrimoSegr);
            entita.ANNULLATO = true;
            db.SaveChanges();
        }
        public List<string> RestituisciLaRigaMassima()
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALECONDIVISIONE> libm = new List<PERCENTUALECONDIVISIONE>();
                libm = db.PERCENTUALECONDIVISIONE.Where(a => a.ANNULLATO == false).ToList().Where(b =>
                b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop())).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCCOND.ToString());
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
            var fm = context.ObjectInstance as percCondivisioneMABModel;

            if (fm != null)
            {
                if (fm.Percentuale > 100)
                {
                    vr = new ValidationResult(string.Format("Impossibile inserire percentuale maggiore di 100 ({0}).", fm.Percentuale.ToString()));
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
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
    public class dtIndPrimoSegr : IDisposable
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
                                dataFineValidita = e.DATAFINEVALIDITA,
                                indennita = e.INDENNITA,
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
                                dataFineValidita = e.DATAFINEVALIDITA,// != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaPrimoSegretModel().dataFineValidita,
                                indennita = e.INDENNITA,
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
        public IList<IndennitaPrimoSegretModel> getListIndennitaPrimoSegretario(bool escludiAnnullati = false)
        {
            List<IndennitaPrimoSegretModel> libm = new List<IndennitaPrimoSegretModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<INDENNITAPRIMOSEGRETARIO> lib = new List<INDENNITAPRIMOSEGRETARIO>();
                    if (escludiAnnullati == true)
                        lib = db.INDENNITAPRIMOSEGRETARIO.Where(a => a.ANNULLATO == false).ToList();
                    else
                        lib = db.INDENNITAPRIMOSEGRETARIO.ToList();

                    libm = (from e in lib
                            select new IndennitaPrimoSegretModel()
                            {
                                idIndPrimoSegr = e.IDINDPRIMOSEGR,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                indennita = e.INDENNITA,
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
        public IList<IndennitaPrimoSegretModel> getListIndennitaPrimoSegretario(decimal idIndPrimoSegr, bool escludiAnnullati = false)
        {
            List<IndennitaPrimoSegretModel> libm = new List<IndennitaPrimoSegretModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<INDENNITAPRIMOSEGRETARIO> lib = new List<INDENNITAPRIMOSEGRETARIO>();
                    if (escludiAnnullati == true)
                        lib = db.INDENNITAPRIMOSEGRETARIO.Where(a => a.IDINDPRIMOSEGR == idIndPrimoSegr && a.ANNULLATO == false).ToList();
                    else
                        lib = db.INDENNITAPRIMOSEGRETARIO.Where(a => a.IDINDPRIMOSEGR == idIndPrimoSegr).ToList();

                    libm = (from e in lib
                            select new IndennitaPrimoSegretModel()
                            {
                                idIndPrimoSegr = e.IDINDPRIMOSEGR,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = Utility.DataFineStop(),
                                indennita = e.INDENNITA,
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

        public bool EsistonoMovimentiPrima(IndennitaPrimoSegretModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.INDENNITAPRIMOSEGRETARIO.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDINDPRIMOSEGR == ibm.idIndPrimoSegr).Count() > 0 ? true : false;
            }
        }
        public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<INDENNITAPRIMOSEGRETARIO> libm = new List<INDENNITAPRIMOSEGRETARIO>();
                libm = db.INDENNITAPRIMOSEGRETARIO.Where(a => a.ANNULLATO == false).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().
                Where(b => DataCampione == b.DATAFINEVALIDITA
                && b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())).ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDINDPRIMOSEGR.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].INDENNITA.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<INDENNITAPRIMOSEGRETARIO> libm = new List<INDENNITAPRIMOSEGRETARIO>();
                libm = db.INDENNITAPRIMOSEGRETARIO.Where(a => a.ANNULLATO == false).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().Where(b => DataCampione == b.DATAINIZIOVALIDITA &&
                 b.DATAFINEVALIDITA != Utility.DataFineStop()).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDINDPRIMOSEGR.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].INDENNITA.ToString());
                }
            }
            return tmp;
        }
        public void SetIndennitaPrimoSegretario(IndennitaPrimoSegretModel ibm, bool aggiornaTutto)
        {
            List<INDENNITAPRIMOSEGRETARIO> libNew = new List<INDENNITAPRIMOSEGRETARIO>();

            //INDENNITAPRIMOSEGRETARIO ibPrecedente = new INDENNITAPRIMOSEGRETARIO();
            INDENNITAPRIMOSEGRETARIO ibNew1 = new INDENNITAPRIMOSEGRETARIO();
            INDENNITAPRIMOSEGRETARIO ibNew2 = new INDENNITAPRIMOSEGRETARIO();
            //List<INDENNITAPRIMOSEGRETARIO> lArchivioIB = new List<INDENNITAPRIMOSEGRETARIO>();
            List<string> lista = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                bool giafatta = false;

                try
                {
                    using (dtIndPrimoSegr dtal = new dtIndPrimoSegr())
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

                            ibNew1 = new INDENNITAPRIMOSEGRETARIO()
                            {
                                DATAINIZIOVALIDITA = dataInizioFirst,
                                DATAFINEVALIDITA = dataFineFirst,
                                INDENNITA = ibm.indennita,
                                DATAAGGIORNAMENTO = DateTime.Now,
                            };

                            if (aggiornaTutto)
                            {
                                ibNew1 = new INDENNITAPRIMOSEGRETARIO()
                                {
                                    DATAINIZIOVALIDITA = dataInizioFirst,
                                    DATAFINEVALIDITA = Utility.DataFineStop(),
                                    INDENNITA = ibm.indennita,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                //qui annullo tutti i record rimanenti dalla data inizio inserita
                                libNew = db.INDENNITAPRIMOSEGRETARIO.Where(a => a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > dataInizioFirst).ToList();
                                foreach (var elem in libNew)
                                {
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDINDPRIMOSEGR), db);
                                }
                            }
                            db.Database.BeginTransaction();
                            db.INDENNITAPRIMOSEGRETARIO.Add(ibNew1);
                            db.SaveChanges();
                            RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloFirst), db);

                            using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                            {
                                dtrp.AssociaFigli_IPS(ibNew1.IDINDPRIMOSEGR, db, ibm.dataInizioValidita);
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

                                ibNew1 = new INDENNITAPRIMOSEGRETARIO()
                                {
                                    DATAINIZIOVALIDITA = dataInizioLast,
                                    DATAFINEVALIDITA = dataFineLast.AddDays(-1),
                                    //COEFFICIENTEKM = aliquotaLast,
                                    INDENNITA = aliquotaLast,
                                };
                                ibNew2 = new INDENNITAPRIMOSEGRETARIO()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = ibm.dataInizioValidita,//è uguale alla data Inizio
                                    //COEFFICIENTEKM = ibm.coefficienteKm,
                                    INDENNITA = ibm.indennita,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };
                                if (aggiornaTutto)
                                {
                                    ibNew2 = new INDENNITAPRIMOSEGRETARIO()
                                    {
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        // COEFFICIENTEKM = ibm.coefficienteKm, 
                                        INDENNITA = ibm.indennita,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.INDENNITAPRIMOSEGRETARIO.Where(a => a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDINDPRIMOSEGR), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                db.Database.BeginTransaction();
                                db.INDENNITAPRIMOSEGRETARIO.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloLast), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var ips in libNew)
                                    {
                                        dtrp.AssociaFigli_IPS(ips.IDINDPRIMOSEGR, db, ibm.dataInizioValidita);
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
                                decimal indennita = Convert.ToDecimal(lista[3]);
                                //  decimal COEFFICIENTEINDBASE = Convert.ToDecimal(lista[4]);
                                DateTime NewdataFine1 = ibm.dataInizioValidita.AddDays(-1);

                                ibNew1 = new INDENNITAPRIMOSEGRETARIO()
                                {
                                    DATAINIZIOVALIDITA = dataInizio,
                                    DATAFINEVALIDITA = NewdataFine1,
                                    // COEFFICIENTEKM = aliquota,
                                    INDENNITA = indennita,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new INDENNITAPRIMOSEGRETARIO()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = dataFine,
                                    //COEFFICIENTEKM = ibm.coefficienteKm,
                                    INDENNITA = ibm.indennita,
                                    //  COEFFICIENTEINDBASE = ibm.coefficienteIndBase,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };

                                if (aggiornaTutto)
                                {
                                    ibNew2 = new INDENNITAPRIMOSEGRETARIO()
                                    {
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        // COEFFICIENTEKM = ibm.coefficienteKm,
                                        INDENNITA = ibm.indennita,
                                        // COEFFICIENTEINDBASE = ibm.coefficienteIndBase,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.INDENNITAPRIMOSEGRETARIO.Where(a => a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDINDPRIMOSEGR), db);
                                    }
                                }
                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                db.Database.BeginTransaction();
                                db.INDENNITAPRIMOSEGRETARIO.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervallo), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var ips in libNew)
                                    {
                                        dtrp.AssociaFigli_IPS(ips.IDINDPRIMOSEGR, db, ibm.dataInizioValidita);
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
                                ibNew1 = new INDENNITAPRIMOSEGRETARIO()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = Convert.ToDateTime(Utility.DataFineStop()),
                                    INDENNITA = ibm.indennita,
                                    DATAAGGIORNAMENTO = DateTime.Now,

                                };
                                libNew.Add(ibNew1);
                                db.Database.BeginTransaction();
                                db.INDENNITAPRIMOSEGRETARIO.Add(ibNew1);
                                db.SaveChanges();

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    dtrp.AssociaFigli_IPS(ibNew1.IDINDPRIMOSEGR, db, ibm.dataInizioValidita);
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
                                decimal INDENNITA_Ultimo = Convert.ToDecimal(lista[3]);
                                // decimal COEFFICIENTEINDBASE_Ultimo = Convert.ToDecimal(lista[4]);

                                if (dataInizioUltimo == ibm.dataInizioValidita)
                                {
                                    ibNew1 = new INDENNITAPRIMOSEGRETARIO()
                                    {
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = dataFineUltimo,
                                        // COEFFICIENTEKM = ibm.coefficienteKm,//nuova aliquota rispetto alla vecchia registrata
                                        INDENNITA = ibm.indennita,
                                        //  COEFFICIENTEINDBASE = ibm.coefficienteIndBase,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.INDENNITAPRIMOSEGRETARIO.Add(ibNew1);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        dtrp.AssociaFigli_IPS(ibNew1.IDINDPRIMOSEGR, db, ibm.dataInizioValidita);
                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                                //se il nuovo record rappresenta la data variazione superiore alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                if (ibm.dataInizioValidita > dataInizioUltimo)
                                {
                                    ibNew1 = new INDENNITAPRIMOSEGRETARIO()
                                    {
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = ibm.dataInizioValidita.AddDays(-1),
                                        // COEFFICIENTEKM = aliquotaUltimo,
                                        INDENNITA = INDENNITA_Ultimo,
                                        //  COEFFICIENTEINDBASE = COEFFICIENTEINDBASE_Ultimo,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    ibNew2 = new INDENNITAPRIMOSEGRETARIO()
                                    {

                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        // COEFFICIENTEKM = ibm.coefficienteKm,//nuova aliquota rispetto alla vecchia registrata
                                        INDENNITA = ibm.indennita,
                                        //  COEFFICIENTEINDBASE = ibm.coefficienteIndBase,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1); libNew.Add(ibNew2);
                                    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                    db.Database.BeginTransaction();
                                    db.INDENNITAPRIMOSEGRETARIO.AddRange(libNew);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        foreach (var ips in libNew)
                                        {
                                            dtrp.AssociaFigli_IPS(ips.IDINDPRIMOSEGR, db, ibm.dataInizioValidita);
                                        }

                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                            }
                        }
                        //INSERIMENTO DATI NELLA TABELLA CIR_R PER LE RELAZIONI MOLTI A MOLTI
                        //decimal idIndPrimoSegr = ibNew1.IDINDPRIMOSEGR;
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
                List<INDENNITAPRIMOSEGRETARIO> libm = new List<INDENNITAPRIMOSEGRETARIO>();
                libm = db.INDENNITAPRIMOSEGRETARIO.Where(a => a.ANNULLATO == false).ToList().Where(b =>
                b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())
                && DataCampione > b.DATAINIZIOVALIDITA
                && DataCampione < b.DATAFINEVALIDITA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDINDPRIMOSEGR.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].INDENNITA.ToString());
                }
            }
            return tmp;
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
        public void DelIndennitaPrimoSegretario_00(decimal idIndPrimoSegr)
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
        public INDENNITAPRIMOSEGRETARIO RestituisciIlRecordPrecedente(decimal idIndennitaPrimoSegr)
        {
            INDENNITAPRIMOSEGRETARIO tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                INDENNITAPRIMOSEGRETARIO interessato = new INDENNITAPRIMOSEGRETARIO();
                interessato = db.INDENNITAPRIMOSEGRETARIO.Find(idIndennitaPrimoSegr);
                tmp = db.INDENNITAPRIMOSEGRETARIO.Where(a => a.ANNULLATO == false).ToList().Where(b => b.DATAFINEVALIDITA == interessato.DATAINIZIOVALIDITA.AddDays(-1)).ToList().First();
            }
            return tmp;
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
                        RendiAnnullatoUnRecord(delIB.IDINDPRIMOSEGR, db);
                        precedenteIB = RestituisciIlRecordPrecedente(idIndPrimoSegr);
                        RendiAnnullatoUnRecord(precedenteIB.IDINDPRIMOSEGR, db);

                        var NuovoPrecedente = new INDENNITAPRIMOSEGRETARIO()
                        {
                            DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                            // ALIQUOTA = precedenteIB.ALIQUOTA,
                            //COEFFICIENTERICHIAMO = precedenteIB.COEFFICIENTERICHIAMO,
                            //COEFFICIENTEINDBASE = precedenteIB.COEFFICIENTEINDBASE,
                            INDENNITA = precedenteIB.INDENNITA,
                            DATAAGGIORNAMENTO = DateTime.Now,// precedenteIB.DATAAGGIORNAMENTO,
                            ANNULLATO = false
                        };
                        db.INDENNITAPRIMOSEGRETARIO.Add(NuovoPrecedente);

                        db.SaveChanges();

                        using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                        {
                            dtrp.AssociaFigli_IPS(NuovoPrecedente.IDINDPRIMOSEGR, db, delIB.DATAINIZIOVALIDITA);
                        }
                    }


                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di Idennità Primo Segretario.", "INDENNITAPRIMOSEGRETARIO", idIndPrimoSegr);
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

        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as IndennitaPrimoSegretModel;

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
                var TuttiNonAnnullati = db.INDENNITAPRIMOSEGRETARIO.Where(a => a.ANNULLATO == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }
        public bool IndPrimoSegretarioAnnullato(IndennitaPrimoSegretModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.INDENNITAPRIMOSEGRETARIO.Where(a => a.IDINDPRIMOSEGR == ibm.idIndPrimoSegr).First().ANNULLATO == true ? true : false;
            }
        }

        public decimal Get_Id_IndPrimoSegretarioNonAnnullato()
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<INDENNITAPRIMOSEGRETARIO> libm = new List<INDENNITAPRIMOSEGRETARIO>();
                libm = db.INDENNITAPRIMOSEGRETARIO.Where(a => a.ANNULLATO == false).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDINDPRIMOSEGR;
            }
            return tmp;
        }
        public void RendiAnnullatoUnRecord(decimal idIdennitaPrimoSegr, ModelDBISE db)
        {
            INDENNITAPRIMOSEGRETARIO entita = new INDENNITAPRIMOSEGRETARIO();
            entita = db.INDENNITAPRIMOSEGRETARIO.Find(idIdennitaPrimoSegr);
            entita.ANNULLATO = true;
            db.SaveChanges();
        }
        public List<string> RestituisciLaRigaMassima()
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<INDENNITAPRIMOSEGRETARIO> libm = new List<INDENNITAPRIMOSEGRETARIO>();
                libm = db.INDENNITAPRIMOSEGRETARIO.Where(a => a.ANNULLATO == false).ToList().Where(b =>
                b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop())).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDINDPRIMOSEGR.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].INDENNITA.ToString());
                }
            }
            return tmp;
        }
    }
}
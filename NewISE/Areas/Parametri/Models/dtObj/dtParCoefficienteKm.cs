using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Dynamic;
using NewISE.Models.dtObj;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParCoefficienteKm : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<CoeffFasciaKmModel> getListCoeffFasciaKm()
        {
            List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEFKM.ToList();

                    libm = (from e in lib
                            select new CoeffFasciaKmModel()
                            {
                                idCfKm = e.IDPFKM,
                                idDefKm = e.IDFKM,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                // dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new CoeffFasciaKmModel().dataFineValidita,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                coefficienteKm = e.COEFFICIENTEKM,
                                annullato = e.ANNULLATO,
                                //km = new DefFasciaKmModel()
                                //{
                                //    idfKm = e.FASCIA_KM.IDFKM,
                                //    km = e.FASCIA_KM.KM
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

        public IList<CoeffFasciaKmModel> getListCoeffFasciaKm(decimal idCfKm)
        {
            List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEFKM.Where(a => a.IDPFKM == idCfKm).ToList();
                    libm = (from e in lib
                            select new CoeffFasciaKmModel()
                            {
                                idCfKm = e.IDPFKM,
                                idDefKm = e.IDFKM,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                // dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop ? e.DATAFINEVALIDITA : new CoeffFasciaKmModel().dataFineValidita,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                coefficienteKm = e.COEFFICIENTEKM,
                                annullato = e.ANNULLATO,
                                //km = new DefFasciaKmModel()
                                //{
                                //    idfKm = e.FASCIA_KM.IDFKM,
                                //    km = e.FASCIA_KM.KM
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

        public IList<CoeffFasciaKmModel> getListCoeffFasciaKm(bool escludiAnnullati = false)
        {
            List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEFKM.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new CoeffFasciaKmModel()
                            {
                                idCfKm = e.IDPFKM,
                                idDefKm = e.IDFKM,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                // dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new CoeffFasciaKmModel().dataFineValidita,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                coefficienteKm = e.COEFFICIENTEKM,
                                annullato = e.ANNULLATO,
                                //km = new DefFasciaKmModel()
                                //{
                                //    idfKm = e.FASCIA_KM.IDFKM,
                                //    km = e.FASCIA_KM.KM
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
        public IList<CoeffFasciaKmModel> getListCoeffFasciaKm(decimal IDFKM, bool escludiAnnullati = false)
        {
            List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<PERCENTUALEFKM> lib = new List<PERCENTUALEFKM>();
                    if (escludiAnnullati == true)
                        lib = db.PERCENTUALEFKM.Where(a => a.IDFKM == IDFKM && a.ANNULLATO == false).ToList();
                    else
                        lib = db.PERCENTUALEFKM.Where(a => a.IDFKM == IDFKM).ToList();

                    libm = (from e in lib
                            select new CoeffFasciaKmModel()
                            {
                                idCfKm = e.IDPFKM,
                                idDefKm = e.IDFKM,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                // dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new CoeffFasciaKmModel().dataFineValidita,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                coefficienteKm = e.COEFFICIENTEKM,
                                annullato = e.ANNULLATO,
                                //km = new DefFasciaKmModel()
                                //{
                                //    idfKm = e.FASCIA_KM.IDFKM,
                                //    km = e.FASCIA_KM.KM,
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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="ibm"></param>



        public void SetCoeffFasciaKm(CoeffFasciaKmModel ibm, bool aggiornaTutto)
        {
            List<PERCENTUALEFKM> libNew = new List<PERCENTUALEFKM>();

            //PERCENTUALEFKM ibPrecedente = new PERCENTUALEFKM();
            PERCENTUALEFKM ibNew1 = new PERCENTUALEFKM();
            PERCENTUALEFKM ibNew2 = new PERCENTUALEFKM();
            //List<PERCENTUALEFKM> lArchivioIB = new List<PERCENTUALEFKM>();
            List<string> lista = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                bool giafatta = false;
                try
                {
                    using (dtParCoefficienteKm dtal = new dtParCoefficienteKm())
                    {
                        //Se la data variazione coincide con una data inizio esistente
                        lista = dtal.DataVariazioneCoincideConDataInizio(ibm.dataInizioValidita, ibm.idDefKm);
                        if (lista.Count != 0)
                        {
                            giafatta = true;
                            decimal idIntervalloFirst = Convert.ToDecimal(lista[0]);
                            DateTime dataInizioFirst = Convert.ToDateTime(lista[1]);
                            DateTime dataFineFirst = Convert.ToDateTime(lista[2]);
                            //decimal aliquotaFirst = Convert.ToDecimal(lista[3]);

                            ibNew1 = new PERCENTUALEFKM()
                            {
                                IDFKM = ibm.idDefKm,
                                DATAINIZIOVALIDITA = dataInizioFirst,
                                DATAFINEVALIDITA = dataFineFirst,
                                COEFFICIENTEKM = ibm.coefficienteKm,
                                DATAAGGIORNAMENTO = DateTime.Now,
                            };

                            if (aggiornaTutto)
                            {
                                ibNew1 = new PERCENTUALEFKM()
                                {
                                    IDFKM = ibm.idDefKm,
                                    DATAINIZIOVALIDITA = dataInizioFirst,
                                    DATAFINEVALIDITA = Utility.DataFineStop(),
                                    COEFFICIENTEKM = ibm.idDefKm,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                //qui annullo tutti i record rimanenti dalla data inizio inserita
                                libNew = db.PERCENTUALEFKM.Where(a => a.IDFKM == ibm.idDefKm
                                && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > dataInizioFirst).ToList();
                                foreach (var elem in libNew)
                                {
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPFKM), db);
                                }
                            }
                            db.Database.BeginTransaction();
                            db.PERCENTUALEFKM.Add(ibNew1);
                            db.SaveChanges();
                            RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloFirst), db);

                            using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                            {
                                dtrp.AssociaPrimaSistemazione_PKM(ibNew1.IDPFKM, db, ibm.dataInizioValidita);
                                dtrp.AssociaRichiamo_PKM(ibNew1.IDPFKM, db, ibm.dataInizioValidita);
                            }


                            db.Database.CurrentTransaction.Commit();
                        }
                        ///se la data variazione coincide con una data fine esistente(diversa da 31/12/9999)
                        if (giafatta == false)
                        {
                            lista = dtal.DataVariazioneCoincideConDataFine(ibm.dataInizioValidita, ibm.idDefKm);
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervalloLast = Convert.ToDecimal(lista[0]);
                                DateTime dataInizioLast = Convert.ToDateTime(lista[1]);
                                DateTime dataFineLast = Convert.ToDateTime(lista[2]);
                                decimal aliquotaLast = Convert.ToDecimal(lista[3]);

                                ibNew1 = new PERCENTUALEFKM()
                                {
                                    IDFKM = ibm.idDefKm,
                                    DATAINIZIOVALIDITA = dataInizioLast,
                                    DATAFINEVALIDITA = dataFineLast.AddDays(-1),
                                    COEFFICIENTEKM = aliquotaLast,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new PERCENTUALEFKM()
                                {
                                    IDFKM = ibm.idDefKm,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = ibm.dataInizioValidita,//è uguale alla data Inizio
                                    COEFFICIENTEKM = ibm.coefficienteKm,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };
                                if (aggiornaTutto)
                                {
                                    ibNew2 = new PERCENTUALEFKM()
                                    {
                                        IDFKM = ibm.idDefKm,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        COEFFICIENTEKM = ibm.coefficienteKm,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.PERCENTUALEFKM.Where(a => a.IDFKM == ibm.idDefKm
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPFKM), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                db.Database.BeginTransaction();
                                db.PERCENTUALEFKM.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloLast), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var cfkm in libNew)
                                    {
                                        dtrp.AssociaPrimaSistemazione_PKM(cfkm.IDPFKM, db, ibm.dataInizioValidita);
                                        dtrp.AssociaRichiamo_PKM(cfkm.IDPFKM, db, ibm.dataInizioValidita);
                                    }

                                }

                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //Se il nuovo record si trova in un intervallo non annullato con data fine non uguale al 31/12/9999
                        if (giafatta == false)
                        {
                            lista = dtal.RestituisciIntervalloDiUnaData(ibm.dataInizioValidita, ibm.idDefKm);
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervallo = Convert.ToDecimal(lista[0]);
                                DateTime dataInizio = Convert.ToDateTime(lista[1]);
                                DateTime dataFine = Convert.ToDateTime(lista[2]);
                                decimal aliquota = Convert.ToDecimal(lista[3]);

                                DateTime NewdataFine1 = ibm.dataInizioValidita.AddDays(-1);

                                ibNew1 = new PERCENTUALEFKM()
                                {
                                    IDFKM = ibm.idDefKm,
                                    DATAINIZIOVALIDITA = dataInizio,
                                    DATAFINEVALIDITA = NewdataFine1,
                                    COEFFICIENTEKM = aliquota,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new PERCENTUALEFKM()
                                {
                                    IDFKM = ibm.idDefKm,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = dataFine,
                                    COEFFICIENTEKM = ibm.coefficienteKm,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };

                                if (aggiornaTutto)
                                {
                                    ibNew2 = new PERCENTUALEFKM()
                                    {
                                        IDFKM = ibm.idDefKm,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        COEFFICIENTEKM = ibm.coefficienteKm,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.PERCENTUALEFKM.Where(a => a.IDFKM == ibm.idDefKm
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPFKM), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                db.Database.BeginTransaction();
                                db.PERCENTUALEFKM.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervallo), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var cfkm in libNew)
                                    {
                                        dtrp.AssociaPrimaSistemazione_PKM(cfkm.IDPFKM, db, ibm.dataInizioValidita);
                                        dtrp.AssociaRichiamo_PKM(cfkm.IDPFKM, db, ibm.dataInizioValidita);
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
                            lista = dtal.RestituisciLaRigaMassima(ibm.idDefKm);
                            if (lista.Count == 0)
                            {
                                ibNew1 = new PERCENTUALEFKM()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = Convert.ToDateTime(Utility.DataFineStop()),
                                    COEFFICIENTEKM = ibm.coefficienteKm,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    IDFKM = ibm.idDefKm,
                                };
                                libNew.Add(ibNew1);
                                db.Database.BeginTransaction();
                                db.PERCENTUALEFKM.Add(ibNew1);
                                int i = db.SaveChanges();

                                if (i > 0)
                                {
                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        dtrp.AssociaPrimaSistemazione_PKM(ibNew1.IDPFKM, db, ibm.dataInizioValidita);
                                        dtrp.AssociaRichiamo_PKM(ibNew1.IDPFKM, db, ibm.dataInizioValidita);
                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                                else
                                {
                                    throw new Exception("Errore nella fase di inserimento della percentuale di fascia chilometrica.");
                                }

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
                                    ibNew1 = new PERCENTUALEFKM()
                                    {
                                        IDFKM = ibm.idDefKm,
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = dataFineUltimo,
                                        COEFFICIENTEKM = ibm.coefficienteKm,//nuova aliquota rispetto alla vecchia registrata
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.PERCENTUALEFKM.Add(ibNew1);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        dtrp.AssociaPrimaSistemazione_PKM(ibNew1.IDPFKM, db, ibm.dataInizioValidita);
                                        dtrp.AssociaRichiamo_PKM(ibNew1.IDPFKM, db, ibm.dataInizioValidita);
                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                                //se il nuovo record rappresenta la data variazione superiore alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                if (ibm.dataInizioValidita > dataInizioUltimo)
                                {
                                    ibNew1 = new PERCENTUALEFKM()
                                    {
                                        IDFKM = ibm.idDefKm,
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = ibm.dataInizioValidita.AddDays(-1),
                                        COEFFICIENTEKM = aliquotaUltimo,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    ibNew2 = new PERCENTUALEFKM()
                                    {
                                        IDFKM = ibm.idDefKm,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        COEFFICIENTEKM = ibm.coefficienteKm,//nuova aliquota rispetto alla vecchia registrata
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1); libNew.Add(ibNew2);
                                    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                    db.Database.BeginTransaction();
                                    db.PERCENTUALEFKM.AddRange(libNew);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        foreach (var cfkm in libNew)
                                        {
                                            dtrp.AssociaPrimaSistemazione_PKM(cfkm.IDPFKM, db, ibm.dataInizioValidita);
                                            dtrp.AssociaRichiamo_PKM(cfkm.IDPFKM, db, ibm.dataInizioValidita);
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



        public void DelCoeffFasciaKm(decimal IDPFKM)
        {
            PERCENTUALEFKM precedenteIB = new PERCENTUALEFKM();
            PERCENTUALEFKM delIB = new PERCENTUALEFKM();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    var lib = db.PERCENTUALEFKM.Where(a => a.IDPFKM == IDPFKM);
                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;
                        RendiAnnullatoUnRecord(delIB.IDPFKM, db);
                        precedenteIB = RestituisciIlRecordPrecedente(IDPFKM);
                        RendiAnnullatoUnRecord(precedenteIB.IDPFKM, db);

                        var NuovoPrecedente = new PERCENTUALEFKM()
                        {
                            IDFKM = precedenteIB.IDFKM,
                            DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                            COEFFICIENTEKM = precedenteIB.COEFFICIENTEKM,
                            DATAAGGIORNAMENTO = DateTime.Now,// precedenteIB.DATAAGGIORNAMENTO,
                            ANNULLATO = false
                        };
                        db.PERCENTUALEFKM.Add(NuovoPrecedente);

                        db.SaveChanges();
                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di Percentuala KM.", "PERCENTUALEFKM", IDPFKM);
                        }

                        using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                        {
                            dtrp.AssociaPrimaSistemazione_PKM(NuovoPrecedente.IDPFKM, db, delIB.DATAINIZIOVALIDITA);
                            dtrp.AssociaRichiamo_PKM(NuovoPrecedente.IDPFKM, db, delIB.DATAINIZIOVALIDITA);
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
        public bool CoefficienteFasciaKmAnnullato(CoeffFasciaKmModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEFKM.Find(ibm.idCfKm).ANNULLATO == true ? true : false;
            }
        }


        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as CoeffFasciaKmModel;

            if (fm != null)
            {
                DateTime d = DataInizioMinimaNonAnnullataCoeffKM(fm.idDefKm);
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
        //
        public static ValidationResult VerificaPercentualeKM(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as CoeffFasciaKmModel;

            if (fm != null)
            {
                if (fm.coefficienteKm > 100)
                {
                    vr = new ValidationResult(string.Format("Impossibile inserire percentuale KM maggiore di 100 ({0}).", fm.coefficienteKm.ToString()));
                }
                else
                {
                    vr = ValidationResult.Success;
                }
            }
            else
            {
                vr = new ValidationResult("La percentuale KM è richiesta.");
            }
            return vr;
        }
        public static DateTime DataInizioMinimaNonAnnullataCoeffKM(decimal idLivello)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.PERCENTUALEFKM.Where(a => a.ANNULLATO == false && a.IDFKM == idLivello).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }
        public PERCENTUALEFKM RestituisciIlRecordPrecedente(decimal IDCFKM)
        {
            PERCENTUALEFKM tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                PERCENTUALEFKM interessato = new PERCENTUALEFKM();
                interessato = db.PERCENTUALEFKM.Find(IDCFKM);
                tmp = db.PERCENTUALEFKM.Where(a => a.IDFKM == interessato.IDFKM
                && a.ANNULLATO == false).ToList().Where(b => b.DATAFINEVALIDITA == interessato.DATAINIZIOVALIDITA.AddDays(-1)).ToList().First();
            }
            return tmp;
        }
        public List<string> RestituisciIntervalloDiUnaData(DateTime DataCampione, decimal IDFKM)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEFKM> libm = new List<PERCENTUALEFKM>();
                libm = db.PERCENTUALEFKM.Where(a => a.ANNULLATO == false
                && a.IDFKM == IDFKM).ToList().Where(b =>
                b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())
                && DataCampione > b.DATAINIZIOVALIDITA
                && DataCampione < b.DATAFINEVALIDITA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPFKM.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTEKM.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione, decimal IDFKM)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEFKM> libm = new List<PERCENTUALEFKM>();
                libm = db.PERCENTUALEFKM.Where(a => a.ANNULLATO == false
                && a.IDFKM == IDFKM).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().Where(b => DataCampione == b.DATAINIZIOVALIDITA &&
                 b.DATAFINEVALIDITA != Utility.DataFineStop()).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPFKM.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTEKM.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione, decimal IDFKM)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEFKM> libm = new List<PERCENTUALEFKM>();
                libm = db.PERCENTUALEFKM.Where(a => a.ANNULLATO == false
                && a.IDFKM == IDFKM).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().
                Where(b => DataCampione == b.DATAFINEVALIDITA
                && b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())).ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPFKM.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTEKM.ToString());
                }
            }
            return tmp;
        }
        public List<string> RestituisciLaRigaMassima(decimal IDFKM)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEFKM> libm = new List<PERCENTUALEFKM>();
                libm = db.PERCENTUALEFKM.Where(a => a.ANNULLATO == false
                && a.IDFKM == IDFKM).ToList().Where(b =>
                b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop())).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPFKM.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTEKM.ToString());
                }
            }
            return tmp;
        }
        public void RendiAnnullatoUnRecord(decimal IDCFKM, ModelDBISE db)
        {
            PERCENTUALEFKM entita = new PERCENTUALEFKM();
            entita = db.PERCENTUALEFKM.Find(IDCFKM);
            entita.ANNULLATO = true;
            db.SaveChanges();
        }
        public decimal Get_Id_CoefficienteFasciaKmNonAnnullato(decimal IDFKM)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEFKM> libm = new List<PERCENTUALEFKM>();
                libm = db.PERCENTUALEFKM.Where(a => a.ANNULLATO == false
                && a.IDFKM == IDFKM).OrderBy(a => a.DATAINIZIOVALIDITA).ThenBy(a => a.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDPFKM;
            }
            return tmp;
        }

    }
}
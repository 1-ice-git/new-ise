using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.Models.Tools;
using System.ComponentModel.DataAnnotations;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtCoefficienteRichiamo : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        
        public bool CoeffIndRichiamoAnnullato(CoefficienteRichiamoModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.COEFFICIENTEINDRICHIAMO.Where(a => a.IDCOEFINDRICHIAMO == ibm.idCoefIndRichiamo).First().ANNULLATO == true ? true : false;
            }
        }
        
        public IList<CoefficienteRichiamoModel> getListCoefficienteRichiamo()
        {
            List<CoefficienteRichiamoModel> libm = new List<CoefficienteRichiamoModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.COEFFICIENTEINDRICHIAMO.ToList();

                    libm = (from e in lib
                            select new CoefficienteRichiamoModel()
                            {
                                idCoefIndRichiamo = e.IDCOEFINDRICHIAMO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,// e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new CoefficienteRichiamoModel().dataFineValidita,
                                coefficienteRichiamo = e.COEFFICIENTERICHIAMO,
                                coefficienteIndBase = e.COEFFICIENTEINDBASE,
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

        public IList<CoefficienteRichiamoModel> getListCoefficienteRichiamo(decimal idCoefIndRichiamo)
        {
            List<CoefficienteRichiamoModel> libm = new List<CoefficienteRichiamoModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    //var lib = db.COEFFICIENTEINDRICHIAMO.Where(a => a.IDCOEFINDRICHIAMO == idCoefIndRichiamo).ToList();

                    var lib = db.COEFFICIENTEINDRICHIAMO.ToList();

                    libm = (from e in lib
                            select new CoefficienteRichiamoModel()
                            {
                                idCoefIndRichiamo = e.IDCOEFINDRICHIAMO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,// e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new CoefficienteRichiamoModel().dataFineValidita,
                                coefficienteRichiamo = e.COEFFICIENTERICHIAMO,
                                coefficienteIndBase = e.COEFFICIENTEINDBASE,
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
        public IList<CoefficienteRichiamoModel> getListCoefficienteRichiamo(bool escludiAnnullati = false)
        {
            List<CoefficienteRichiamoModel> libm = new List<CoefficienteRichiamoModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new CoefficienteRichiamoModel()
                            {
                                idCoefIndRichiamo = e.IDCOEFINDRICHIAMO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,// e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new CoefficienteRichiamoModel().dataFineValidita,
                                coefficienteRichiamo = e.COEFFICIENTERICHIAMO,
                                coefficienteIndBase = e.COEFFICIENTEINDBASE,
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

        public IList<CoefficienteRichiamoModel> getListCoefficienteRichiamo(decimal idCoefIndRichiamo, bool escludiAnnullati = false)
        {
            List<CoefficienteRichiamoModel> libm = new List<CoefficienteRichiamoModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    //var lib = db.COEFFICIENTEINDRICHIAMO.Where(a => a.IDCOEFINDRICHIAMO == idCoefIndRichiamo && a.ANNULLATO == escludiAnnullati).ToList();

                    //var lib = db.COEFFICIENTEINDRICHIAMO.Where(a => a.IDCOEFINDRICHIAMO == idCoefIndRichiamo).ToList();

                    //var lib = db.COEFFICIENTEINDRICHIAMO.ToList();

                    var lib = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new CoefficienteRichiamoModel()
                            {
                                idCoefIndRichiamo = e.IDCOEFINDRICHIAMO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,// e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new CoefficienteRichiamoModel().dataFineValidita,
                                coefficienteRichiamo = e.COEFFICIENTERICHIAMO,
                                coefficienteIndBase = e.COEFFICIENTEINDBASE,
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
       

        public void SetCoefficienteRichiamo(CoefficienteRichiamoModel ibm, bool aggiornaTutto)
        {
            List<COEFFICIENTEINDRICHIAMO> libNew = new List<COEFFICIENTEINDRICHIAMO>();

            COEFFICIENTEINDRICHIAMO ibPrecedente = new COEFFICIENTEINDRICHIAMO();
            COEFFICIENTEINDRICHIAMO ibNew1 = new COEFFICIENTEINDRICHIAMO();
            COEFFICIENTEINDRICHIAMO ibNew2 = new COEFFICIENTEINDRICHIAMO();
            List<COEFFICIENTEINDRICHIAMO> lArchivioIB = new List<COEFFICIENTEINDRICHIAMO>();
            List<string> lista = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                bool giafatta = false;
                try
                {
                    using (dtCoefficienteRichiamo dtal = new dtCoefficienteRichiamo())
                    {
                        //Se la data variazione coincide con una data inizio esistente
                        lista = dtal.DataVariazioneCoincideConDataInizio(ibm.dataInizioValidita);
                        if (lista.Count != 0)
                        {
                            giafatta = true;
                            decimal idIntervalloFirst = Convert.ToDecimal(lista[0]);
                            DateTime dataInizioFirst = Convert.ToDateTime(lista[1]);
                            DateTime dataFineFirst = Convert.ToDateTime(lista[2]);
                            decimal COEFFICIENTERICHIAMO = Convert.ToDecimal(lista[3]);
                            decimal COEFFICIENTEINDBASE = Convert.ToDecimal(lista[4]);

                            ibNew1 = new COEFFICIENTEINDRICHIAMO()
                            {
                                DATAINIZIOVALIDITA = dataInizioFirst,
                                DATAFINEVALIDITA = dataFineFirst,
                                COEFFICIENTERICHIAMO = ibm.coefficienteRichiamo,
                                COEFFICIENTEINDBASE = ibm.coefficienteIndBase,
                                DATAAGGIORNAMENTO = DateTime.Now,
                            };

                            if (aggiornaTutto)
                            {
                                ibNew1 = new COEFFICIENTEINDRICHIAMO()
                                {
                                    DATAINIZIOVALIDITA = dataInizioFirst,
                                    DATAFINEVALIDITA = Utility.DataFineStop(),
                                    COEFFICIENTERICHIAMO = ibm.coefficienteRichiamo,
                                    COEFFICIENTEINDBASE = ibm.coefficienteIndBase,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                //qui annullo tutti i record rimanenti dalla data inizio inserita
                                libNew = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > dataInizioFirst).ToList();
                                foreach (var elem in libNew)
                                {
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDCOEFINDRICHIAMO), db);
                                }
                            }
                            db.Database.BeginTransaction();
                            db.COEFFICIENTEINDRICHIAMO.Add(ibNew1);
                            db.SaveChanges();
                            RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloFirst), db);

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

                                decimal COEFFICIENTERICHIAMO_last = Convert.ToDecimal(lista[3]);
                                decimal COEFFICIENTEINDBASE_last = Convert.ToDecimal(lista[4]);

                                ibNew1 = new COEFFICIENTEINDRICHIAMO()
                                {
                                    DATAINIZIOVALIDITA = dataInizioLast,
                                    DATAFINEVALIDITA = dataFineLast.AddDays(-1),
                                    //COEFFICIENTEKM = aliquotaLast,
                                    COEFFICIENTERICHIAMO = COEFFICIENTERICHIAMO_last,
                                    COEFFICIENTEINDBASE = COEFFICIENTEINDBASE_last,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new COEFFICIENTEINDRICHIAMO()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = ibm.dataInizioValidita,//è uguale alla data Inizio
                                   //COEFFICIENTEKM = ibm.coefficienteKm,
                                    COEFFICIENTERICHIAMO = ibm.coefficienteRichiamo,
                                    COEFFICIENTEINDBASE = ibm.coefficienteIndBase,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };
                                if (aggiornaTutto)
                                {
                                    ibNew2 = new COEFFICIENTEINDRICHIAMO()
                                    {                                        
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        // COEFFICIENTEKM = ibm.coefficienteKm,
                                        COEFFICIENTERICHIAMO = ibm.coefficienteRichiamo,
                                        COEFFICIENTEINDBASE = ibm.coefficienteIndBase,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDCOEFINDRICHIAMO), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                db.Database.BeginTransaction();
                                db.COEFFICIENTEINDRICHIAMO.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloLast), db);
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
                                decimal COEFFICIENTERICHIAMO = Convert.ToDecimal(lista[3]);
                                decimal COEFFICIENTEINDBASE = Convert.ToDecimal(lista[4]);
                                DateTime NewdataFine1 = ibm.dataInizioValidita.AddDays(-1);

                                ibNew1 = new COEFFICIENTEINDRICHIAMO()
                                {
                                    DATAINIZIOVALIDITA = dataInizio,
                                    DATAFINEVALIDITA = NewdataFine1,
                                    // COEFFICIENTEKM = aliquota,
                                    COEFFICIENTERICHIAMO = COEFFICIENTERICHIAMO,
                                    COEFFICIENTEINDBASE = COEFFICIENTEINDBASE,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new COEFFICIENTEINDRICHIAMO()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = dataFine,
                                    //COEFFICIENTEKM = ibm.coefficienteKm,
                                    COEFFICIENTERICHIAMO = ibm.coefficienteRichiamo,
                                    COEFFICIENTEINDBASE = ibm.coefficienteIndBase,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };

                                if (aggiornaTutto)
                                {
                                    ibNew2 = new COEFFICIENTEINDRICHIAMO()
                                    {
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        // COEFFICIENTEKM = ibm.coefficienteKm,
                                        COEFFICIENTERICHIAMO = ibm.coefficienteRichiamo,
                                        COEFFICIENTEINDBASE = ibm.coefficienteIndBase,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDCOEFINDRICHIAMO), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                db.Database.BeginTransaction();
                                db.COEFFICIENTEINDRICHIAMO.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervallo), db);
                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //CASO DELL'ULTIMA RIGA CON LA DATA FINE UGUALE A 31/12/9999
                        if (giafatta == false)
                        {
                            lista = dtal.RestituisciLaRigaMassima();
                            if (lista.Count == 0)
                                {
                                    ibNew1 = new COEFFICIENTEINDRICHIAMO()
                                    {
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                        // COEFFICIENTEKM = ibm.coefficienteKm,//nuova aliquota rispetto alla vecchia registrata
                                        COEFFICIENTERICHIAMO = ibm.coefficienteRichiamo,
                                        COEFFICIENTEINDBASE = ibm.coefficienteIndBase,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.COEFFICIENTEINDRICHIAMO.Add(ibNew1);
                                    db.SaveChanges();
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
                                decimal COEFFICIENTERICHIAMO_Ultimo = Convert.ToDecimal(lista[3]);
                                decimal COEFFICIENTEINDBASE_Ultimo = Convert.ToDecimal(lista[4]);
                                if (dataInizioUltimo == ibm.dataInizioValidita)
                                {
                                    ibNew1 = new COEFFICIENTEINDRICHIAMO()
                                    {                                       
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = dataFineUltimo,
                                        // COEFFICIENTEKM = ibm.coefficienteKm,//nuova aliquota rispetto alla vecchia registrata
                                        COEFFICIENTERICHIAMO = ibm.coefficienteRichiamo,
                                        COEFFICIENTEINDBASE = ibm.coefficienteIndBase,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.COEFFICIENTEINDRICHIAMO.Add(ibNew1);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);
                                    db.Database.CurrentTransaction.Commit();
                                }
                                //se il nuovo record rappresenta la data variazione superiore alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                if (ibm.dataInizioValidita > dataInizioUltimo)
                                {
                                    ibNew1 = new COEFFICIENTEINDRICHIAMO()
                                    {
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = ibm.dataInizioValidita.AddDays(-1),
                                        // COEFFICIENTEKM = aliquotaUltimo,
                                        COEFFICIENTERICHIAMO = COEFFICIENTERICHIAMO_Ultimo,
                                        COEFFICIENTEINDBASE = COEFFICIENTEINDBASE_Ultimo,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    ibNew2 = new COEFFICIENTEINDRICHIAMO()
                                    {
                                        
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        // COEFFICIENTEKM = ibm.coefficienteKm,//nuova aliquota rispetto alla vecchia registrata
                                        COEFFICIENTERICHIAMO = ibm.coefficienteRichiamo,
                                        COEFFICIENTEINDBASE = ibm.coefficienteIndBase,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1); libNew.Add(ibNew2);
                                    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                    db.Database.BeginTransaction();
                                    db.COEFFICIENTEINDRICHIAMO.AddRange(libNew);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);
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
        public bool EsistonoMovimentiPrima(CoefficienteRichiamoModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.COEFFICIENTEINDRICHIAMO.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDCOEFINDRICHIAMO == ibm.idCoefIndRichiamo).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(CoefficienteRichiamoModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.COEFFICIENTEINDRICHIAMO.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDCOEFINDRICHIAMO == ibm.idCoefIndRichiamo).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(CoefficienteRichiamoModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.COEFFICIENTEINDRICHIAMO.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDCOEFINDRICHIAMO == ibm.idCoefIndRichiamo).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiPrimaUguale(CoefficienteRichiamoModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.COEFFICIENTEINDRICHIAMO.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDCOEFINDRICHIAMO == ibm.idCoefIndRichiamo).Count() > 0 ? true : false;
            }
        }

        
        public void DelCoefficienteRichiamo(decimal idCoefIndRichiamo)
        {
            COEFFICIENTEINDRICHIAMO precedenteIB = new COEFFICIENTEINDRICHIAMO();
            COEFFICIENTEINDRICHIAMO delIB = new COEFFICIENTEINDRICHIAMO();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    var lib = db.COEFFICIENTEINDRICHIAMO.Where(a => a.IDCOEFINDRICHIAMO == idCoefIndRichiamo);
                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;
                        RendiAnnullatoUnRecord(delIB.IDCOEFINDRICHIAMO, db);
                        precedenteIB = RestituisciIlRecordPrecedente(idCoefIndRichiamo);
                        RendiAnnullatoUnRecord(precedenteIB.IDCOEFINDRICHIAMO, db);

                        var NuovoPrecedente = new COEFFICIENTEINDRICHIAMO()
                        {
                            DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                           // ALIQUOTA = precedenteIB.ALIQUOTA,
                           COEFFICIENTERICHIAMO=precedenteIB.COEFFICIENTERICHIAMO,
                           COEFFICIENTEINDBASE=precedenteIB.COEFFICIENTEINDBASE,
                           DATAAGGIORNAMENTO = DateTime.Now,// precedenteIB.DATAAGGIORNAMENTO,
                           ANNULLATO = false
                        };
                        db.COEFFICIENTEINDRICHIAMO.Add(NuovoPrecedente);
                    }
                    db.SaveChanges();
                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di aliquote contributive.", "COEFFICIENTEINDRICHIAMO", idCoefIndRichiamo);
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
        public static DateTime DataInizioMinimaNonAnnullataCoeffIndRichiamo()
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }
        public static List<RiduzioniModel> dataInizioValiditaAccettataPerRiduzione(DateTime d)
        {
            List<RiduzioniModel> tmp = new List<RiduzioniModel>();
            using (dtRiduzioni dtRid = new dtRiduzioni())
            {
              tmp = dtRid.getListRiduzioni().Where(a=>a.annullato==false).ToList().Where(b=> d>=b.dataInizioValidita && d<= b.dataFineValidita).OrderBy(a=>a.dataInizioValidita).ThenBy(b=>b.dataFineValidita).ToList();
            }
            return tmp;
        }
        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as CoefficienteRichiamoModel;

            if (fm != null)
            {
                DateTime d = DataInizioMinimaNonAnnullataCoeffIndRichiamo();
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

        public static DateTime DataInizioMinimaNonAnnullataIndennitaBase()
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }
        public COEFFICIENTEINDRICHIAMO RestituisciIlRecordPrecedente(decimal IDCOEFINDRICHIAMO)
        {
            COEFFICIENTEINDRICHIAMO tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                COEFFICIENTEINDRICHIAMO interessato = new COEFFICIENTEINDRICHIAMO();
                interessato = db.COEFFICIENTEINDRICHIAMO.Find(IDCOEFINDRICHIAMO);
                tmp = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).ToList().Where(b => b.DATAFINEVALIDITA == interessato.DATAINIZIOVALIDITA.AddDays(-1)).ToList().First();
            }
            return tmp;
        }
        public  List<string> RestituisciIntervalloDiUnaData(DateTime DataCampione)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTEINDRICHIAMO> libm = new List<COEFFICIENTEINDRICHIAMO>();
                libm = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).ToList().Where(b =>
                b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())
                && DataCampione > b.DATAINIZIOVALIDITA
                && DataCampione < b.DATAFINEVALIDITA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDCOEFINDRICHIAMO.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTERICHIAMO.ToString());
                    tmp.Add(libm[0].COEFFICIENTEINDBASE.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTEINDRICHIAMO> libm = new List<COEFFICIENTEINDRICHIAMO>();
                libm = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().Where(b => DataCampione == b.DATAINIZIOVALIDITA &&
                 b.DATAFINEVALIDITA != Utility.DataFineStop()).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDCOEFINDRICHIAMO.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTERICHIAMO.ToString());
                    tmp.Add(libm[0].COEFFICIENTEINDBASE.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTEINDRICHIAMO> libm = new List<COEFFICIENTEINDRICHIAMO>();
                libm = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().
                Where(b => DataCampione == b.DATAFINEVALIDITA
                && b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())).ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDCOEFINDRICHIAMO.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTERICHIAMO.ToString());
                    tmp.Add(libm[0].COEFFICIENTEINDBASE.ToString());
                }
            }
            return tmp;
        }
        public List<string> RestituisciLaRigaMassima()
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTEINDRICHIAMO> libm = new List<COEFFICIENTEINDRICHIAMO>();
                libm = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).ToList().Where(b =>
                b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop())).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDCOEFINDRICHIAMO.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTERICHIAMO.ToString());
                    tmp.Add(libm[0].COEFFICIENTEINDBASE.ToString());
                }
            }
            return tmp;
        }
        public void RendiAnnullatoUnRecord(decimal IDCOEFINDRICHIAMO, ModelDBISE db)
        {
            COEFFICIENTEINDRICHIAMO entita = new COEFFICIENTEINDRICHIAMO();
            entita = db.COEFFICIENTEINDRICHIAMO.Find(IDCOEFINDRICHIAMO);
            entita.ANNULLATO = true;
            db.SaveChanges();
        }
        public decimal Get_Id_CoefficienteIndRichiamoNonAnnullato(decimal IDCOEFINDRICHIAMO)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTEINDRICHIAMO> libm = new List<COEFFICIENTEINDRICHIAMO>();
                libm = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false
                && a.IDCOEFINDRICHIAMO == IDCOEFINDRICHIAMO).OrderBy(a => a.DATAINIZIOVALIDITA).ThenBy(a => a.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDCOEFINDRICHIAMO;
            }
            return tmp;
        }
    }
}
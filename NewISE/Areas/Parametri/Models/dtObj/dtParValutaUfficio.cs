using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParValutaUfficio : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }        
        public IList<ValutaUfficioModel> getListValutaUfficio()
        {
            List<ValutaUfficioModel> libm = new List<ValutaUfficioModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.VALUTAUFFICIO.ToList();

                    libm = (from e in lib
                            select new ValutaUfficioModel()
                            {
                                
                                //idPercMab = e.IDVALUTAUFFICIO,
                                idValuta = e.IDVALUTA,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA ,//!= Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new ValutaUfficioModel().dataFineValidita,
                             //   percentuale = e.PERCENTUALE,
                             //   percentualeResponsabile = e.PERCENTUALERESPONSABILE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                Valuta = new ValuteModel()
                                {
                                     idValuta= e.VALUTE.IDVALUTA,
                                    descrizioneValuta = e.VALUTE.DESCRIZIONEVALUTA
                                },
                                Ufficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    //DescUfficio = e.UFFICI.DESCRIZIONEUFFICIO
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

        public IList<ValutaUfficioModel> getListValutaUfficio(decimal idLivello, decimal idUfficio)
        {
            List<ValutaUfficioModel> libm = new List<ValutaUfficioModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.VALUTAUFFICIO.Where(a => a.IDVALUTA == idLivello).ToList();

                    libm = (from e in lib
                            select new ValutaUfficioModel()
                            {
                                //idPercMab = e.IDVALUTAUFFICIO,
                                idValuta = e.IDVALUTA,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                               // percentuale = e.PERCENTUALE,
                             //   percentualeResponsabile = e.PERCENTUALERESPONSABILE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                Valuta = new ValuteModel()
                                {
                                    idValuta = e.VALUTE.IDVALUTA,
                                    descrizioneValuta = e.VALUTE.DESCRIZIONEVALUTA
                                },
                                Ufficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    //DescUfficio = e.UFFICI.DESCRIZIONEUFFICIO
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

        public IList<ValutaUfficioModel> getListValutaUfficio(bool escludiAnnullati = false)
        {
            List<ValutaUfficioModel> libm = new List<ValutaUfficioModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.VALUTAUFFICIO.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new ValutaUfficioModel()
                            {
                                //idPercMab = e.IDVALUTAUFFICIO,
                                idValuta = e.IDVALUTA,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                             //   percentuale = e.PERCENTUALE,
                             //   percentualeResponsabile = e.PERCENTUALERESPONSABILE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                Valuta = new ValuteModel()
                                {
                                    idValuta = e.VALUTE.IDVALUTA,
                                    descrizioneValuta = e.VALUTE.DESCRIZIONEVALUTA
                                },
                                Ufficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    //DescUfficio = e.UFFICI.DESCRIZIONEUFFICIO
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

        public IList<ValutaUfficioModel> getListValutaUfficio(decimal idLivello, decimal idUfficio, bool escludiAnnullati = false)
        {
            List<ValutaUfficioModel> libm = new List<ValutaUfficioModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    //     var lib = db.VALUTAUFFICIO.Where(a => a.IDVALUTA == idLivello && a.ANNULLATO == escludiAnnullati).ToList();

                    List<VALUTAUFFICIO> lib = new List<VALUTAUFFICIO>();
                    if(escludiAnnullati==true)
                        lib= db.VALUTAUFFICIO.Where(a => a.IDVALUTA == idLivello && a.IDUFFICIO==idUfficio 
                       &&  a.ANNULLATO == false).ToList();
                    else
                        lib= db.VALUTAUFFICIO.Where(a => a.IDVALUTA == idLivello).ToList();
                    libm = (from e in lib
                            select new ValutaUfficioModel()
                            {
                                idValutaUfficio = e.IDVALUTAUFFICIO,
                                idValuta = e.IDVALUTA,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA ,
                              //  percentuale = e.PERCENTUALE,
                              //  percentualeResponsabile = e.PERCENTUALERESPONSABILE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                Valuta = new ValuteModel()
                                {
                                    idValuta = e.VALUTE.IDVALUTA,
                                    descrizioneValuta = e.VALUTE.DESCRIZIONEVALUTA
                                },
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ibm"></param>
      

        public bool EsistonoMovimentiPrima(ValutaUfficioModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.VALUTAUFFICIO.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDVALUTA == ibm.idValuta && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(ValutaUfficioModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.VALUTAUFFICIO.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDVALUTA == ibm.idValuta && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(ValutaUfficioModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.VALUTAUFFICIO.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDVALUTA == ibm.idValuta && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }
        
        public bool EsistonoMovimentiPrimaUguale(ValutaUfficioModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.VALUTAUFFICIO.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDVALUTA == ibm.idValuta && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
            }
        }
       
        public void DelValutaUfficio(decimal IDVALUTAUFFICIO)
        {
            VALUTAUFFICIO precedenteIB = new VALUTAUFFICIO();
            VALUTAUFFICIO delIB = new VALUTAUFFICIO();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    var lib = db.VALUTAUFFICIO.Where(a => a.IDVALUTAUFFICIO == IDVALUTAUFFICIO);
                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;
                        RendiAnnullatoUnRecord(delIB.IDVALUTAUFFICIO, db);
                        precedenteIB = RestituisciIlRecordPrecedente(IDVALUTAUFFICIO);
                        RendiAnnullatoUnRecord(precedenteIB.IDVALUTAUFFICIO, db);

                        var NuovoPrecedente = new VALUTAUFFICIO()
                        {
                            IDUFFICIO = precedenteIB.IDUFFICIO,
                            IDVALUTA = precedenteIB.IDVALUTA,
                            DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                            //ALIQUOTA = precedenteIB.ALIQUOTA,
                            //PERCENTUALE = precedenteIB.PERCENTUALE,
                            //PERCENTUALERESPONSABILE = precedenteIB.PERCENTUALERESPONSABILE,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            ANNULLATO = false
                        };
                        db.VALUTAUFFICIO.Add(NuovoPrecedente);
                    }
                    db.SaveChanges();
                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di Valuta Ufficio", "VALUTAUFFICIO", IDVALUTAUFFICIO);
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

        public bool ParValutaUfficioAnnullato(ValutaUfficioModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.VALUTAUFFICIO.Where(a => a.IDVALUTAUFFICIO == ibm.idValutaUfficio).First().ANNULLATO == true ? true : false;
            }
        }
        public void SetValutaUfficio(ValutaUfficioModel ibm, bool aggiornaTutto)
        {
            List<VALUTAUFFICIO> libNew = new List<VALUTAUFFICIO>();

            VALUTAUFFICIO ibPrecedente = new VALUTAUFFICIO();
            VALUTAUFFICIO ibNew1 = new VALUTAUFFICIO();
            VALUTAUFFICIO ibNew2 = new VALUTAUFFICIO();
            List<VALUTAUFFICIO> lArchivioIB = new List<VALUTAUFFICIO>();
            List<string> lista = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                bool giafatta = false;
                try
                {
                    using (dtParValutaUfficio dtal = new dtParValutaUfficio())
                    {
                        //Se la data variazione coincide con una data inizio esistente
                        lista = dtal.DataVariazioneCoincideConDataInizio(ibm.dataInizioValidita,ibm.idValuta ,ibm.idUfficio);
                        if (lista.Count != 0)
                        {
                            giafatta = true;
                            decimal idIntervalloFirst = Convert.ToDecimal(lista[0]);
                            DateTime dataInizioFirst = Convert.ToDateTime(lista[1]);
                            DateTime dataFineFirst = Convert.ToDateTime(lista[2]);
                            //decimal PercentualeFirst = Convert.ToDecimal(lista[3]);
                            //decimal PercentualeRespFirst = Convert.ToDecimal(lista[4]);

                            ibNew1 = new VALUTAUFFICIO()
                            {
                                IDVALUTA = ibm.idValuta,
                                IDUFFICIO=ibm.idUfficio,
                                DATAINIZIOVALIDITA = dataInizioFirst,
                                DATAFINEVALIDITA = dataFineFirst,
                                //PERCENTUALE = ibm.percentuale,
                                //PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                                DATAAGGIORNAMENTO=DateTime.Now
                            };

                            if (aggiornaTutto)
                            {
                                ibNew1 = new VALUTAUFFICIO()
                                {
                                    IDVALUTA = ibm.idValuta,
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = dataInizioFirst,
                                    DATAFINEVALIDITA = Utility.DataFineStop(),
                                    //PERCENTUALE = ibm.percentuale,
                                    //PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };
                                //qui annullo tutti i record rimanenti dalla data inizio inserita
                                libNew = db.VALUTAUFFICIO.Where(a => a.IDVALUTA == ibm.idValuta && a.IDUFFICIO==ibm.idUfficio 
                                && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > dataInizioFirst).ToList();
                                foreach (var elem in libNew)
                                {
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDVALUTAUFFICIO), db);
                                }
                            }
                            db.Database.BeginTransaction();
                            db.VALUTAUFFICIO.Add(ibNew1);
                            db.SaveChanges();
                            RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloFirst), db);

                            db.Database.CurrentTransaction.Commit();
                        }
                        ///se la data variazione coincide con una data fine esistente(diversa da 31/12/9999)
                        if (giafatta == false)
                        {
                            lista = dtal.DataVariazioneCoincideConDataFine(ibm.dataInizioValidita, ibm.idValuta,ibm.idUfficio);
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervalloLast = Convert.ToDecimal(lista[0]);
                                DateTime dataInizioLast = Convert.ToDateTime(lista[1]);
                                DateTime dataFineLast = Convert.ToDateTime(lista[2]);
                                // decimal aliquotaLast = Convert.ToDecimal(lista[3]);
                                decimal PercentualeLast = Convert.ToDecimal(lista[3]);
                                decimal PercentualeRespLast = Convert.ToDecimal(lista[4]);

                                ibNew1 = new VALUTAUFFICIO()
                                {
                                    IDVALUTA = ibm.idValuta,
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = dataInizioLast,
                                    DATAFINEVALIDITA = dataFineLast.AddDays(-1),
                                    //PERCENTUALE = ibm.percentuale,
                                    //PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                                    DATAAGGIORNAMENTO = DateTime.Now

                                };
                                ibNew2 = new VALUTAUFFICIO()
                                {
                                    IDVALUTA = ibm.idValuta,
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = ibm.dataInizioValidita,//è uguale alla data Inizio
                                    //PERCENTUALE = ibm.percentuale,
                                    //PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };
                                if (aggiornaTutto)
                                {
                                    ibNew2 = new VALUTAUFFICIO()
                                    {
                                        IDVALUTA = ibm.idValuta,
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        //PERCENTUALE = ibm.percentuale,
                                        //PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.VALUTAUFFICIO.Where(a => a.IDVALUTA == ibm.idValuta && a.IDUFFICIO ==ibm.idUfficio 
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDVALUTAUFFICIO), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                db.Database.BeginTransaction();
                                db.VALUTAUFFICIO.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloLast), db);
                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //Se il nuovo record si trova in un intervallo non annullato con data fine non uguale al 31/12/9999
                        if (giafatta == false)
                        {
                            lista = dtal.RestituisciIntervalloDiUnaData(ibm.dataInizioValidita, ibm.idValuta,ibm.idUfficio);
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervallo = Convert.ToDecimal(lista[0]);
                                DateTime dataInizio = Convert.ToDateTime(lista[1]);
                                DateTime dataFine = Convert.ToDateTime(lista[2]);
                               // decimal percentuale = Convert.ToDecimal(lista[3]);
                              //  decimal percentualeResponsabile = Convert.ToDecimal(lista[4]);

                                DateTime NewdataFine1 = ibm.dataInizioValidita.AddDays(-1);

                                ibNew1 = new VALUTAUFFICIO()
                                {
                                    IDVALUTA = ibm.idValuta,
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = dataInizio,
                                    DATAFINEVALIDITA = NewdataFine1,
                                    // COEFFICIENTEKM = aliquota,
                                    //PERCENTUALE = percentuale,
                                    //PERCENTUALERESPONSABILE = percentualeResponsabile,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new VALUTAUFFICIO()
                                {
                                    IDVALUTA = ibm.idValuta,
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = dataFine,
                                    //PERCENTUALE = ibm.percentuale,
                                    //PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };

                                if (aggiornaTutto)
                                {
                                    ibNew2 = new VALUTAUFFICIO()
                                    {
                                        IDVALUTA = ibm.idValuta,
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        //PERCENTUALE = ibm.percentuale,
                                        //PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.VALUTAUFFICIO.Where(a => a.IDVALUTA == ibm.idValuta && a.IDUFFICIO==ibm.idUfficio 
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDVALUTAUFFICIO), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                db.Database.BeginTransaction();
                                db.VALUTAUFFICIO.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervallo), db);
                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //CASO DELL'ULTIMA RIGA CON LA DATA FINE UGUALE A 31/12/9999
                        if (giafatta == false)
                        {
                            //Attenzione qui se la lista non contiene nessun elemento
                            //significa che non esiste nessun elemento corrispondentemente al livello selezionato
                            lista = dtal.RestituisciLaRigaMassima(ibm.idValuta,ibm.idUfficio);
                            if (lista.Count == 0)
                            {
                                ibNew1 = new VALUTAUFFICIO()
                                {
                                    IDVALUTA = ibm.idValuta,
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = Convert.ToDateTime(Utility.DataFineStop()),
                                    //PERCENTUALE = ibm.percentuale,
                                    //PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    
                                };
                                libNew.Add(ibNew1);
                                db.Database.BeginTransaction();
                                db.VALUTAUFFICIO.Add(ibNew1);
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
                                //decimal PercentoUltimo = Convert.ToDecimal(lista[3]);
                                //decimal PercentoRespUltimo = Convert.ToDecimal(lista[4]);
                                if (dataInizioUltimo == ibm.dataInizioValidita)
                                {
                                    ibNew1 = new VALUTAUFFICIO()
                                    {
                                        IDVALUTA = ibm.idValuta,
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = dataFineUltimo,
                                        //PERCENTUALE = ibm.percentuale,
                                        //PERCENTUALERESPONSABILE = ibm.percentualeResponsabile,//nuova aliquota rispetto alla vecchia registrata
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.VALUTAUFFICIO.Add(ibNew1);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);
                                    db.Database.CurrentTransaction.Commit();
                                }
                                //se il nuovo record rappresenta la data variazione superiore alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                if (ibm.dataInizioValidita > dataInizioUltimo)
                                {
                                    ibNew1 = new VALUTAUFFICIO()
                                    {
                                        IDVALUTA = ibm.idValuta,
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = ibm.dataInizioValidita.AddDays(-1),
                                        //PERCENTUALE = PercentoUltimo,
                                        //PERCENTUALERESPONSABILE= PercentoRespUltimo,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    ibNew2 = new VALUTAUFFICIO()
                                    {
                                        IDVALUTA = ibm.idValuta,
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                       // PERCENTUALE = ibm.percentuale,//nuova aliquota rispetto alla vecchia registrata
                                       // PERCENTUALERESPONSABILE=ibm.percentualeResponsabile,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1); libNew.Add(ibNew2);
                                    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                    db.Database.BeginTransaction();
                                    db.VALUTAUFFICIO.AddRange(libNew);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);
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
        
        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as ValutaUfficioModel;

            if (fm != null)
            {
                DateTime d = DataInizioMinimaNonAnnullataValutaUfficio(fm.idValuta,fm.idUfficio);
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
        
        //public static ValidationResult VerificaPercentualeResponsabile(string v, ValidationContext context)
        //{
        //    ValidationResult vr = ValidationResult.Success;
        //    var fm = context.ObjectInstance as ValutaUfficioModel;

        //    if (fm != null)
        //    {
        //        if (fm.percentualeResponsabile> 100)
        //        {
        //            vr = new ValidationResult(string.Format("Impossibile inserire percentuale  maggiore di 100 ({0}).", fm.percentualeResponsabile.ToString()));
        //        }
        //        else
        //        {
        //            vr = ValidationResult.Success;
        //        }
        //    }
        //    else
        //    {
        //        vr = new ValidationResult("La percentuale  è richiesta.");
        //    }
        //    return vr;
        //}
        //public static ValidationResult VerificaPercentuale(string v, ValidationContext context)
        //{
        //    ValidationResult vr = ValidationResult.Success;
        //    var fm = context.ObjectInstance as ValutaUfficioModel;

        //    if (fm != null)
        //    {
        //        if (fm.percentuale> 100)
        //        {
        //            vr = new ValidationResult(string.Format("Impossibile inserire percentuale KM maggiore di 100 ({0}).", fm.percentuale.ToString()));
        //        }
        //        else
        //        {
        //            vr = ValidationResult.Success;
        //        }
        //    }
        //    else
        //    {
        //        vr = new ValidationResult("La percentuale responsabile è richiesta.");
        //    }
        //    return vr;
        //}
        public static DateTime DataInizioMinimaNonAnnullataValutaUfficio(decimal idLivello, decimal idUfficio)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.VALUTAUFFICIO.Where(a => a.ANNULLATO == false && 
                a.IDVALUTA == idLivello &&
                a.IDUFFICIO==idUfficio).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }
        public VALUTAUFFICIO RestituisciIlRecordPrecedente(decimal IDCFKM)
        {
            VALUTAUFFICIO tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                VALUTAUFFICIO interessato = new VALUTAUFFICIO();
                interessato = db.VALUTAUFFICIO.Find(IDCFKM);
                tmp = db.VALUTAUFFICIO.Where(a => a.IDVALUTA == interessato.IDVALUTA && a.IDUFFICIO == interessato.IDUFFICIO 
                && a.ANNULLATO == false).ToList().Where(b => b.DATAFINEVALIDITA == interessato.DATAINIZIOVALIDITA.AddDays(-1)).ToList().First();
            }
            return tmp;
        }
        public List<string> RestituisciIntervalloDiUnaData(DateTime DataCampione, decimal IDVALUTA, decimal IDUFFICIO)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<VALUTAUFFICIO> libm = new List<VALUTAUFFICIO>();
                libm = db.VALUTAUFFICIO.Where(a => a.ANNULLATO == false
                && a.IDVALUTA == IDVALUTA && a.IDUFFICIO== IDUFFICIO).ToList().Where(b =>
                b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())
                && DataCampione > b.DATAINIZIOVALIDITA
                && DataCampione < b.DATAFINEVALIDITA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDVALUTAUFFICIO.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    //tmp.Add(libm[0].PERCENTUALE.ToString());
                    //tmp.Add(libm[0].PERCENTUALERESPONSABILE.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione, decimal IDVALUTA, decimal IDUFFICIO)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<VALUTAUFFICIO> libm = new List<VALUTAUFFICIO>();
                libm = db.VALUTAUFFICIO.Where(a => a.ANNULLATO == false
                && a.IDVALUTA == IDVALUTA && a.IDUFFICIO==IDUFFICIO).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().Where(b => DataCampione == b.DATAINIZIOVALIDITA &&
                 b.DATAFINEVALIDITA != Utility.DataFineStop()).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDVALUTAUFFICIO.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    //tmp.Add(libm[0].PERCENTUALE.ToString());
                    //tmp.Add(libm[0].PERCENTUALERESPONSABILE.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione, decimal IDVALUTA, decimal IDUFFICIO)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<VALUTAUFFICIO> libm = new List<VALUTAUFFICIO>();
                libm = db.VALUTAUFFICIO.Where(a => a.ANNULLATO == false
                && a.IDVALUTA == IDVALUTA && a.IDUFFICIO==IDUFFICIO).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().
                Where(b => DataCampione == b.DATAFINEVALIDITA
                && b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())).ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDVALUTAUFFICIO.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                  //  tmp.Add(libm[0].PERCENTUALE.ToString());
                   // tmp.Add(libm[0].PERCENTUALERESPONSABILE.ToString());
                }
            }
            return tmp;
        }
        public List<string> RestituisciLaRigaMassima(decimal IDVALUTA, decimal IDUFFICIO)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<VALUTAUFFICIO> libm = new List<VALUTAUFFICIO>();
                libm = db.VALUTAUFFICIO.Where(a => a.ANNULLATO == false
                && a.IDVALUTA == IDVALUTA && a.IDUFFICIO== IDUFFICIO).ToList().Where(b =>
                b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop())).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDVALUTAUFFICIO.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
              //      tmp.Add(libm[0].PERCENTUALE.ToString());
                   // tmp.Add(libm[0].PERCENTUALERESPONSABILE.ToString());
                }
            }
            return tmp;
        }
        public void RendiAnnullatoUnRecord(decimal IDVALUTAUFFICIO, ModelDBISE db)
        {
            VALUTAUFFICIO entita = new VALUTAUFFICIO();
            entita = db.VALUTAUFFICIO.Find(IDVALUTAUFFICIO);
            entita.ANNULLATO = true;
            db.SaveChanges();
        }
        public decimal Get_Id_ValutaUfficioNonAnnullato(decimal IDVALUTA, decimal IDUFFICIO)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<VALUTAUFFICIO> libm = new List<VALUTAUFFICIO>();
                libm = db.VALUTAUFFICIO.Where(a => a.ANNULLATO == false
                && a.IDVALUTA == IDVALUTA && a.IDUFFICIO == IDUFFICIO).OrderBy(a => a.DATAINIZIOVALIDITA).ThenBy(a => a.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDVALUTAUFFICIO;
            }
            return tmp;
        }
    }

}
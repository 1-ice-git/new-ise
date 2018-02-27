using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.Tools;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtAliquoteContr : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<AliquoteContributiveModel> getListAliquoteContributive()
        {
            List<AliquoteContributiveModel> libm = new List<AliquoteContributiveModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.ALIQUOTECONTRIBUTIVE.ToList();

                    libm = (from e in lib
                            select new AliquoteContributiveModel()
                            {
                                
                                idAliqContr = e.IDALIQCONTR,
                                idTipoContributo = e.IDTIPOCONTRIBUTO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new AliquoteContributiveModel().dataFineValidita,
                                aliquota = e.ALIQUOTA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                descrizione = new TipoAliquoteContributiveModel()
                                {
                                    idTipoAliqContr = e.TIPOALIQUOTECONTRIBUTIVE.IDTIPOALIQCONTR,
                                    descrizione = e.TIPOALIQUOTECONTRIBUTIVE.DESCRIZIONE
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

        public IList<AliquoteContributiveModel> getListAliquoteContributive(decimal idTipoContributo)
        {
            List<AliquoteContributiveModel> libm = new List<AliquoteContributiveModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.ALIQUOTECONTRIBUTIVE.Where(a => a.IDTIPOCONTRIBUTO == idTipoContributo).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                    libm = (from e in lib
                            select new AliquoteContributiveModel()
                            {
                                
                                idAliqContr = e.IDALIQCONTR,
                                idTipoContributo = e.IDTIPOCONTRIBUTO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new AliquoteContributiveModel().dataFineValidita,
                                aliquota = e.ALIQUOTA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                descrizione = new TipoAliquoteContributiveModel()
                                {
                                    idTipoAliqContr = e.TIPOALIQUOTECONTRIBUTIVE.IDTIPOALIQCONTR,
                                    descrizione = e.TIPOALIQUOTECONTRIBUTIVE.DESCRIZIONE
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

        public IList<AliquoteContributiveModel> getListAliquoteContributive(bool escludiAnnullati = false)
        {
            List<AliquoteContributiveModel> libm = new List<AliquoteContributiveModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.ALIQUOTECONTRIBUTIVE.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new AliquoteContributiveModel()
                            {
                                
                                idAliqContr = e.IDALIQCONTR,
                                idTipoContributo = e.IDTIPOCONTRIBUTO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new AliquoteContributiveModel().dataFineValidita,
                                aliquota = e.ALIQUOTA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                descrizione = new TipoAliquoteContributiveModel()
                                {
                                    idTipoAliqContr = e.TIPOALIQUOTECONTRIBUTIVE.IDTIPOALIQCONTR,
                                    descrizione = e.TIPOALIQUOTECONTRIBUTIVE.DESCRIZIONE
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

        public IList<AliquoteContributiveModel> getListAliquoteContributive(decimal idTipoContributo, bool escludiAnnullati = false)
        {
            List<AliquoteContributiveModel> libm = new List<AliquoteContributiveModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    //   var lib = db.ALIQUOTECONTRIBUTIVE.Where(a => a.IDTIPOCONTRIBUTO == idTipoContributo && a.ANNULLATO == escludiAnnullati).ToList();

                    List<ALIQUOTECONTRIBUTIVE> lib = new List<ALIQUOTECONTRIBUTIVE>();
                        if(escludiAnnullati==true)
                        lib = db.ALIQUOTECONTRIBUTIVE.Where(a => a.IDTIPOCONTRIBUTO == idTipoContributo && a.ANNULLATO == false).OrderBy(a=>a.DATAINIZIOVALIDITA).ToList();
                    else
                        lib = db.ALIQUOTECONTRIBUTIVE.Where(a => a.IDTIPOCONTRIBUTO == idTipoContributo).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                    libm = (from e in lib
                            select new AliquoteContributiveModel()
                            {
                                idAliqContr = e.IDALIQCONTR,
                                idTipoContributo = e.IDTIPOCONTRIBUTO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA ,//!= Convert.ToDateTime(Utility.DataFineStop()) ? e.DATAFINEVALIDITA : new AliquoteContributiveModel().dataFineValidita,
                                aliquota = e.ALIQUOTA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                descrizione = new TipoAliquoteContributiveModel()
                                {
                                    idTipoAliqContr = e.TIPOALIQUOTECONTRIBUTIVE.IDTIPOALIQCONTR,
                                    descrizione = e.TIPOALIQUOTECONTRIBUTIVE.DESCRIZIONE
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

       
        public void SetAliquoteContributive(AliquoteContributiveModel ibm,bool aggiornaTutto)
        {
            List<ALIQUOTECONTRIBUTIVE> libNew = new List<ALIQUOTECONTRIBUTIVE>();

            ALIQUOTECONTRIBUTIVE ibPrecedente = new ALIQUOTECONTRIBUTIVE();
            ALIQUOTECONTRIBUTIVE ibNew1 = new ALIQUOTECONTRIBUTIVE();
            ALIQUOTECONTRIBUTIVE ibNew2 = new ALIQUOTECONTRIBUTIVE();
            List<ALIQUOTECONTRIBUTIVE> lArchivioIB = new List<ALIQUOTECONTRIBUTIVE>();
            List<string> lista = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                bool giafatta = false;
                try
                {
                    using (dtAliquoteContr dtal = new dtAliquoteContr())
                    {
                        //Se la data variazione coincide con una data inizio esistente
                        lista=dtal.DataVariazioneCoincideConDataInizio(ibm.dataInizioValidita, ibm.idTipoContributo);
                        if (lista.Count != 0)
                        {
                            giafatta = true;
                            decimal idIntervalloFirst = Convert.ToDecimal(lista[0]);
                            DateTime dataInizioFirst = Convert.ToDateTime(lista[1]);
                            DateTime dataFineFirst = Convert.ToDateTime(lista[2]);
                            decimal aliquotaFirst = Convert.ToDecimal(lista[3]);

                            ibNew1 = new ALIQUOTECONTRIBUTIVE()
                            {
                                IDTIPOCONTRIBUTO = ibm.idTipoContributo,
                                DATAINIZIOVALIDITA = dataInizioFirst,
                                DATAFINEVALIDITA = dataFineFirst,
                                ALIQUOTA = ibm.aliquota,
                                DATAAGGIORNAMENTO = DateTime.Now,
                            };
                               
                            if (aggiornaTutto)
                            {
                                ibNew1 = new ALIQUOTECONTRIBUTIVE()
                                {
                                    IDTIPOCONTRIBUTO = ibm.idTipoContributo,
                                    DATAINIZIOVALIDITA = dataInizioFirst,
                                    DATAFINEVALIDITA = Utility.DataFineStop(),
                                    ALIQUOTA = ibm.aliquota,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                //qui annullo tutti i record rimanenti dalla data inizio inserita
                                libNew = db.ALIQUOTECONTRIBUTIVE.Where(a => a.IDTIPOCONTRIBUTO == ibm.idTipoContributo
                                && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > dataInizioFirst).ToList();
                                foreach(var elem in libNew)
                                {
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDALIQCONTR), db);
                                }
                            }
                            db.Database.BeginTransaction();
                            db.ALIQUOTECONTRIBUTIVE.Add(ibNew1);
                            db.SaveChanges();
                            RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloFirst), db);
                            
                            db.Database.CurrentTransaction.Commit();
                        }
                        ///se la data variazione coincide con una data fine esistente(diversa da 31/12/9999)
                        if (giafatta == false)
                        {
                            lista = dtal.DataVariazioneCoincideConDataFine(ibm.dataInizioValidita, ibm.idTipoContributo);
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervalloLast = Convert.ToDecimal(lista[0]);
                                DateTime dataInizioLast = Convert.ToDateTime(lista[1]);
                                DateTime dataFineLast = Convert.ToDateTime(lista[2]);
                                decimal aliquotaLast = Convert.ToDecimal(lista[3]);

                                ibNew1 = new ALIQUOTECONTRIBUTIVE()
                                {
                                    IDTIPOCONTRIBUTO = ibm.idTipoContributo,
                                    DATAINIZIOVALIDITA = dataInizioLast,
                                    DATAFINEVALIDITA = dataFineLast.AddDays(-1),
                                    ALIQUOTA = aliquotaLast,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new ALIQUOTECONTRIBUTIVE()
                                {
                                    IDTIPOCONTRIBUTO = ibm.idTipoContributo,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = ibm.dataInizioValidita,//è uguale alla data Inizio
                                    ALIQUOTA = ibm.aliquota,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };
                                if (aggiornaTutto)
                                {
                                    ibNew2 = new ALIQUOTECONTRIBUTIVE()
                                    {
                                        IDTIPOCONTRIBUTO = ibm.idTipoContributo,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        ALIQUOTA = ibm.aliquota,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.ALIQUOTECONTRIBUTIVE.Where(a => a.IDTIPOCONTRIBUTO == ibm.idTipoContributo
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDALIQCONTR), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                
                                db.Database.BeginTransaction();
                                db.ALIQUOTECONTRIBUTIVE.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloLast), db);
                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //Se il nuovo record si trova in un intervallo non annullato con data fine non uguale al 31/12/9999
                        if (giafatta == false)
                        {
                            lista = dtal.RestituisciIntervalloDiUnaData(ibm.dataInizioValidita, ibm.idTipoContributo);
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervallo = Convert.ToDecimal(lista[0]);
                                DateTime dataInizio = Convert.ToDateTime(lista[1]);
                                DateTime dataFine = Convert.ToDateTime(lista[2]);
                                decimal aliquota = Convert.ToDecimal(lista[3]);

                                DateTime NewdataFine1 = ibm.dataInizioValidita.AddDays(-1);

                                ibNew1 = new ALIQUOTECONTRIBUTIVE()
                                {
                                    IDTIPOCONTRIBUTO = ibm.idTipoContributo,
                                    DATAINIZIOVALIDITA = dataInizio,
                                    DATAFINEVALIDITA = NewdataFine1,
                                    ALIQUOTA = aliquota,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new ALIQUOTECONTRIBUTIVE()
                                {
                                    IDTIPOCONTRIBUTO = ibm.idTipoContributo,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = dataFine,
                                    ALIQUOTA = ibm.aliquota,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };

                                if (aggiornaTutto)
                                {
                                    ibNew2 = new ALIQUOTECONTRIBUTIVE()
                                    {
                                        IDTIPOCONTRIBUTO = ibm.idTipoContributo,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        ALIQUOTA = ibm.aliquota,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.ALIQUOTECONTRIBUTIVE.Where(a => a.IDTIPOCONTRIBUTO == ibm.idTipoContributo
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDALIQCONTR), db);
                                    }
                                }
                                
                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                db.Database.BeginTransaction();
                                db.ALIQUOTECONTRIBUTIVE.AddRange(libNew);
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
                            lista = dtal.RestituisciLaRigaMassima(ibm.idTipoContributo);
                            if (lista.Count == 0)
                            {
                                ibNew1 = new ALIQUOTECONTRIBUTIVE()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = Convert.ToDateTime(Utility.DataFineStop()),
                                    ALIQUOTA = ibm.aliquota,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    IDTIPOCONTRIBUTO = ibm.idTipoContributo, 
                                };
                                libNew.Add(ibNew1);
                                db.Database.BeginTransaction();
                                db.ALIQUOTECONTRIBUTIVE.Add(ibNew1);
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
                                decimal aliquotaUltimo = Convert.ToDecimal(lista[3]);
                                if (dataInizioUltimo == ibm.dataInizioValidita)
                                {
                                    ibNew1 = new ALIQUOTECONTRIBUTIVE()
                                    {
                                        IDTIPOCONTRIBUTO = ibm.idTipoContributo,
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = dataFineUltimo,
                                        ALIQUOTA = ibm.aliquota,//nuova aliquota rispetto alla vecchia registrata
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.ALIQUOTECONTRIBUTIVE.Add(ibNew1);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);
                                    db.Database.CurrentTransaction.Commit();
                                }
                                //se il nuovo record rappresenta la data variazione superiore alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                if (ibm.dataInizioValidita > dataInizioUltimo)
                                {
                                    ibNew1 = new ALIQUOTECONTRIBUTIVE()
                                    {
                                        IDTIPOCONTRIBUTO = ibm.idTipoContributo,
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = ibm.dataInizioValidita.AddDays(-1),
                                        ALIQUOTA = aliquotaUltimo,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    ibNew2 = new ALIQUOTECONTRIBUTIVE()
                                    {
                                        IDTIPOCONTRIBUTO = ibm.idTipoContributo,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        ALIQUOTA = ibm.aliquota,//nuova aliquota rispetto alla vecchia registrata
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1); libNew.Add(ibNew2);
                                    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                    db.Database.BeginTransaction();
                                    db.ALIQUOTECONTRIBUTIVE.AddRange(libNew);
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
        public void AnnullaAliquotaContr(decimal idAliquotaContr)
        { }
        public bool EsistonoMovimentiPrima(AliquoteContributiveModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.ALIQUOTECONTRIBUTIVE.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDTIPOCONTRIBUTO == ibm.idTipoContributo).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(AliquoteContributiveModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.ALIQUOTECONTRIBUTIVE.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDTIPOCONTRIBUTO == ibm.idTipoContributo).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(AliquoteContributiveModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.ALIQUOTECONTRIBUTIVE.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDTIPOCONTRIBUTO == ibm.idTipoContributo).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiPrimaUguale(AliquoteContributiveModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.ALIQUOTECONTRIBUTIVE.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDALIQCONTR == ibm.idAliqContr).Count() > 0 ? true : false;
            }
        }

        public void DelAliquoteContributive(decimal idAliqContr)
        {
            ALIQUOTECONTRIBUTIVE precedenteIB = new ALIQUOTECONTRIBUTIVE();
            ALIQUOTECONTRIBUTIVE delIB = new ALIQUOTECONTRIBUTIVE();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    var lib = db.ALIQUOTECONTRIBUTIVE.Where(a => a.IDALIQCONTR == idAliqContr);
                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;
                        RendiAnnullatoUnRecord(delIB.IDALIQCONTR, db);
                        precedenteIB = RestituisciIlRecordPrecedente(idAliqContr);
                        RendiAnnullatoUnRecord(precedenteIB.IDALIQCONTR, db);

                        var NuovoPrecedente = new ALIQUOTECONTRIBUTIVE()
                        {
                            IDTIPOCONTRIBUTO = precedenteIB.IDTIPOCONTRIBUTO,
                            DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                            ALIQUOTA = precedenteIB.ALIQUOTA,
                            DATAAGGIORNAMENTO = DateTime.Now,// precedenteIB.DATAAGGIORNAMENTO,
                            ANNULLATO = false
                        };
                        db.ALIQUOTECONTRIBUTIVE.Add(NuovoPrecedente);
                    }
                    db.SaveChanges();
                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di aliquote contributive.", "ALIQUOTECONTRIBUTIVE", idAliqContr);
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
        public bool AliquoteContributiveAnnullato(AliquoteContributiveModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.ALIQUOTECONTRIBUTIVE.Where(a => a.IDALIQCONTR == ibm.idAliqContr && a.IDTIPOCONTRIBUTO == ibm.idTipoContributo).First().ANNULLATO == true ? true : false;
            }
        }
        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as AliquoteContributiveModel;

            if (fm != null)
            {
                DateTime d = DataInizioMinimaNonAnnullataIndennitaBase(fm.idTipoContributo);
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
        public decimal Get_Id_AliquoteContributivePrimoNonAnnullato(decimal idTipoContributo)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<ALIQUOTECONTRIBUTIVE> libm = new List<ALIQUOTECONTRIBUTIVE>();
                libm = db.ALIQUOTECONTRIBUTIVE.Where(a => a.ANNULLATO == false
                && a.IDTIPOCONTRIBUTO == idTipoContributo).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDALIQCONTR;
            }
            return tmp;
        }
        public static DateTime DataInizioMinimaNonAnnullataIndennitaBase(decimal idLivello)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.ALIQUOTECONTRIBUTIVE.Where(a => a.ANNULLATO == false && a.IDTIPOCONTRIBUTO == idLivello).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }
        public ALIQUOTECONTRIBUTIVE RestituisciIlRecordPrecedente(decimal idAliqContr)
        {           
            ALIQUOTECONTRIBUTIVE tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                ALIQUOTECONTRIBUTIVE interessato = new ALIQUOTECONTRIBUTIVE();               
                interessato = db.ALIQUOTECONTRIBUTIVE.Find(idAliqContr);               
                tmp = db.ALIQUOTECONTRIBUTIVE.Where(a=>a.IDTIPOCONTRIBUTO == interessato.IDTIPOCONTRIBUTO  
                && a.ANNULLATO==false).ToList().Where(b => b.DATAFINEVALIDITA == interessato.DATAINIZIOVALIDITA.AddDays(-1)).ToList().First();
            }
            return tmp;
        }
        public List<string> RestituisciIntervalloDiUnaData(DateTime DataCampione,decimal idTipoContributo)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<ALIQUOTECONTRIBUTIVE> libm = new List<ALIQUOTECONTRIBUTIVE>();
                libm = db.ALIQUOTECONTRIBUTIVE.Where(a => a.ANNULLATO == false
                && a.IDTIPOCONTRIBUTO == idTipoContributo).ToList().Where(b=>
                b.DATAFINEVALIDITA!=Convert.ToDateTime(Utility.DataFineStop())
                && DataCampione > b.DATAINIZIOVALIDITA
                && DataCampione < b.DATAFINEVALIDITA).OrderBy(b=>b.DATAINIZIOVALIDITA).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDALIQCONTR.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].ALIQUOTA.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione, decimal idTipoContributo)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<ALIQUOTECONTRIBUTIVE> libm = new List<ALIQUOTECONTRIBUTIVE>();
                libm = db.ALIQUOTECONTRIBUTIVE.Where(a => a.ANNULLATO == false
                && a.IDTIPOCONTRIBUTO == idTipoContributo).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().Where(b=> DataCampione == b.DATAINIZIOVALIDITA && 
                 b.DATAFINEVALIDITA != Utility.DataFineStop()).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDALIQCONTR.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].ALIQUOTA.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione, decimal idTipoContributo)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<ALIQUOTECONTRIBUTIVE> libm = new List<ALIQUOTECONTRIBUTIVE>();
                libm = db.ALIQUOTECONTRIBUTIVE.Where(a => a.ANNULLATO == false
                && a.IDTIPOCONTRIBUTO == idTipoContributo).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().
                Where(b=> DataCampione == b.DATAFINEVALIDITA
                && b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())).ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDALIQCONTR.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].ALIQUOTA.ToString());
                }
            }
            return tmp;
        }
        public List<string> RestituisciLaRigaMassima( decimal idTipoContributo)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<ALIQUOTECONTRIBUTIVE> libm = new List<ALIQUOTECONTRIBUTIVE>();
                libm = db.ALIQUOTECONTRIBUTIVE.Where(a => a.ANNULLATO == false
                && a.IDTIPOCONTRIBUTO == idTipoContributo).ToList().Where(b=>
                b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop())).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDALIQCONTR.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].ALIQUOTA.ToString());
                }
            }
            return tmp;
        }
        public void RendiAnnullatoUnRecord(decimal idAliqContr, ModelDBISE db)
        {
            ALIQUOTECONTRIBUTIVE entita = new ALIQUOTECONTRIBUTIVE();               
                entita = db.ALIQUOTECONTRIBUTIVE.Find(idAliqContr);
                entita.ANNULLATO = true;
                db.SaveChanges();               
        }
    }
}
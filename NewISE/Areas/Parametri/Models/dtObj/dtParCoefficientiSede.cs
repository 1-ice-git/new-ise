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
    public class dtParCoefficientiSede : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<CoefficientiSedeModel> getListCoefficientiSede()
        {
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.COEFFICIENTESEDE.ToList();

                    libm = (from e in lib
                            select new CoefficientiSedeModel()
                            {

                                idCoefficientiSede = e.IDCOEFFICIENTESEDE,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,//!= Utility.DataFineStop() ? e.DATAFINEVALIDITA : new CoefficientiSedeModel().dataFineValidita,
                                valore = e.VALORECOEFFICIENTE,
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

        public IList<CoefficientiSedeModel> getListCoefficientiSede(decimal idUfficio)
        {
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.COEFFICIENTESEDE.Where(a => a.IDUFFICIO == idUfficio).ToList();

                    libm = (from e in lib
                            select new CoefficientiSedeModel()
                            {

                                idCoefficientiSede = e.IDCOEFFICIENTESEDE,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new CoefficientiSedeModel().dataFineValidita,
                                valore = e.VALORECOEFFICIENTE,
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

        public IList<CoefficientiSedeModel> getListCoefficientiSede(bool escludiAnnullati = false)
        {
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.COEFFICIENTESEDE.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new CoefficientiSedeModel()
                            {
                                idCoefficientiSede = e.IDCOEFFICIENTESEDE,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new CoefficientiSedeModel().dataFineValidita,
                                valore = e.VALORECOEFFICIENTE,
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

        public IList<CoefficientiSedeModel> getListCoefficientiSede(decimal idUfficio, bool escludiAnnullati = false)
        {
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<COEFFICIENTESEDE> lib = new List<COEFFICIENTESEDE>();
                    if (escludiAnnullati == true)
                        lib = db.COEFFICIENTESEDE.Where(a => a.IDUFFICIO == idUfficio && a.ANNULLATO == false).ToList();
                    else
                        lib = db.COEFFICIENTESEDE.Where(a => a.IDUFFICIO == idUfficio).ToList();

                    libm = (from e in lib
                            select new CoefficientiSedeModel()
                            {
                                idCoefficientiSede = e.IDCOEFFICIENTESEDE,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,//e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new CoefficientiSedeModel().dataFineValidita,
                                valore = e.VALORECOEFFICIENTE,
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
        public decimal Get_Id_CoefficientiSedeNonAnnullato(decimal idLivello)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTESEDE> libm = new List<COEFFICIENTESEDE>();
                libm = db.COEFFICIENTESEDE.Where(a => a.ANNULLATO == false
                && a.IDUFFICIO == idLivello).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDCOEFFICIENTESEDE;
            }
            return tmp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ibm"></param>
        public void SetCoefficientiSede__00(CoefficientiSedeModel ibm)
        {
            List<COEFFICIENTESEDE> libNew = new List<COEFFICIENTESEDE>();

            COEFFICIENTESEDE ibNew = new COEFFICIENTESEDE();

            COEFFICIENTESEDE ibPrecedente = new COEFFICIENTESEDE();

            List<COEFFICIENTESEDE> lArchivioIB = new List<COEFFICIENTESEDE>();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    if (ibm.dataFineValidita.HasValue)
                    {
                        if (EsistonoMovimentiSuccessiviUguale(ibm))
                        {
                            ibNew = new COEFFICIENTESEDE()
                            {

                                IDUFFICIO = ibm.idUfficio,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                VALORECOEFFICIENTE = ibm.valore,
                                DATAAGGIORNAMENTO = DateTime.Now,// ibm.dataAggiornamento,
                                ANNULLATO = ibm.annullato
                            };
                        }
                        else
                        {
                            ibNew = new COEFFICIENTESEDE()
                            {
                                IDUFFICIO = ibm.idUfficio,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = Utility.DataFineStop(),
                                VALORECOEFFICIENTE = ibm.valore,
                                DATAAGGIORNAMENTO = DateTime.Now,// ibm.dataAggiornamento,
                                ANNULLATO = ibm.annullato
                            };
                        }
                    }
                    else
                    {
                        ibNew = new COEFFICIENTESEDE()
                        {
                            IDUFFICIO = ibm.idUfficio,
                            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            DATAFINEVALIDITA = Utility.DataFineStop(),
                            VALORECOEFFICIENTE = ibm.valore,
                            DATAAGGIORNAMENTO = ibm.dataAggiornamento,//rnamento,
                            ANNULLATO = ibm.annullato
                        };
                    }

                    db.Database.BeginTransaction();

                    var recordInteressati = db.COEFFICIENTESEDE.Where(a => a.ANNULLATO == false && a.IDUFFICIO == ibNew.IDUFFICIO)
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
                                    var ibOld1 = new COEFFICIENTESEDE()
                                    {
                                        IDUFFICIO = item.IDUFFICIO,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(-1),
                                        VALORECOEFFICIENTE = item.VALORECOEFFICIENTE,
                                        DATAAGGIORNAMENTO = DateTime.Now,// item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);

                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new COEFFICIENTESEDE()
                                    {
                                        IDUFFICIO = item.IDUFFICIO,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(-1),
                                        DATAAGGIORNAMENTO = DateTime.Now,// item.DATAAGGIORNAMENTO,
                                        VALORECOEFFICIENTE = item.VALORECOEFFICIENTE,
                                        ANNULLATO = false
                                    };

                                    var ibOld2 = new COEFFICIENTESEDE()
                                    {
                                        IDUFFICIO = item.IDUFFICIO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(+1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        DATAAGGIORNAMENTO = DateTime.Now,// item.DATAAGGIORNAMENTO,
                                        VALORECOEFFICIENTE = item.VALORECOEFFICIENTE,
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
                                    var ibOld1 = new COEFFICIENTESEDE()
                                    {
                                        IDUFFICIO = item.IDUFFICIO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        VALORECOEFFICIENTE = item.VALORECOEFFICIENTE,
                                        DATAAGGIORNAMENTO = DateTime.Now,// item.DATAAGGIORNAMENTO,
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
                                    var ibOld1 = new COEFFICIENTESEDE()
                                    {
                                        IDUFFICIO = item.IDUFFICIO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        VALORECOEFFICIENTE = item.VALORECOEFFICIENTE,
                                        DATAAGGIORNAMENTO = DateTime.Now,// item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                }
                            }
                        }

                        libNew.Add(ibNew);
                        libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        db.COEFFICIENTESEDE.AddRange(libNew);
                    }
                    else
                    {
                        db.COEFFICIENTESEDE.Add(ibNew);

                    }
                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro coefficiente di sede.", "COEFFICENTISEDE", ibNew.IDCOEFFICIENTESEDE);
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


        public void SetCoefficientiSede(CoefficientiSedeModel ibm, bool aggiornaTutto)
        {
            List<COEFFICIENTESEDE> libNew = new List<COEFFICIENTESEDE>();

            //COEFFICIENTESEDE ibPrecedente = new COEFFICIENTESEDE();
            COEFFICIENTESEDE ibNew1 = new COEFFICIENTESEDE();
            COEFFICIENTESEDE ibNew2 = new COEFFICIENTESEDE();
            //List<COEFFICIENTESEDE> lArchivioIB = new List<COEFFICIENTESEDE>();
            List<string> lista = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                bool giafatta = false;
                try
                {
                    using (dtParCoefficientiSede dtal = new dtParCoefficientiSede())
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

                            ibNew1 = new COEFFICIENTESEDE()
                            {
                                IDUFFICIO = ibm.idUfficio,
                                DATAINIZIOVALIDITA = dataInizioFirst,
                                DATAFINEVALIDITA = dataFineFirst,
                                // ALIQUOTA = ibm.aliquota,
                                VALORECOEFFICIENTE = ibm.valore,
                                DATAAGGIORNAMENTO = DateTime.Now,
                            };

                            if (aggiornaTutto)
                            {
                                ibNew1 = new COEFFICIENTESEDE()
                                {
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = dataInizioFirst,
                                    DATAFINEVALIDITA = Utility.DataFineStop(),
                                    // ALIQUOTA = ibm.aliquota,
                                    VALORECOEFFICIENTE = ibm.valore,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                //qui annullo tutti i record rimanenti dalla data inizio inserita
                                libNew = db.COEFFICIENTESEDE.Where(a => a.IDUFFICIO == ibm.idUfficio
                                && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > dataInizioFirst).ToList();
                                foreach (var elem in libNew)
                                {
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDCOEFFICIENTESEDE), db);
                                }
                            }
                            db.Database.BeginTransaction();
                            db.COEFFICIENTESEDE.Add(ibNew1);
                            db.SaveChanges();
                            RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloFirst), db);

                            using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                            {
                                dtrp.AssociaIndennita_CS(ibNew1.IDCOEFFICIENTESEDE, db, ibm.dataInizioValidita);
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

                                ibNew1 = new COEFFICIENTESEDE()
                                {
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = dataInizioLast,
                                    DATAFINEVALIDITA = dataFineLast.AddDays(-1),
                                    VALORECOEFFICIENTE = aliquotaLast,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new COEFFICIENTESEDE()
                                {
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = ibm.dataInizioValidita,//è uguale alla data Inizio
                                    VALORECOEFFICIENTE = ibm.valore,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };
                                if (aggiornaTutto)
                                {
                                    ibNew2 = new COEFFICIENTESEDE()
                                    {
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        VALORECOEFFICIENTE = ibm.valore,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.COEFFICIENTESEDE.Where(a => a.IDUFFICIO == ibm.idUfficio
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDCOEFFICIENTESEDE), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                db.Database.BeginTransaction();
                                db.COEFFICIENTESEDE.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloLast), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var cs in libNew)
                                    {
                                        dtrp.AssociaIndennita_CS(cs.IDCOEFFICIENTESEDE, db, ibm.dataInizioValidita);
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

                                ibNew1 = new COEFFICIENTESEDE()
                                {
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = dataInizio,
                                    DATAFINEVALIDITA = NewdataFine1,
                                    //ALIQUOTA = aliquota,
                                    VALORECOEFFICIENTE = aliquota,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new COEFFICIENTESEDE()
                                {
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = dataFine,
                                    // ALIQUOTA = ibm.aliquota,
                                    VALORECOEFFICIENTE = ibm.valore,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };

                                if (aggiornaTutto)
                                {
                                    ibNew2 = new COEFFICIENTESEDE()
                                    {
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        // ALIQUOTA = ibm.aliquota,
                                        VALORECOEFFICIENTE = ibm.valore,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.COEFFICIENTESEDE.Where(a => a.IDUFFICIO == ibm.idUfficio
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDCOEFFICIENTESEDE), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                db.Database.BeginTransaction();
                                db.COEFFICIENTESEDE.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervallo), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var cs in libNew)
                                    {
                                        dtrp.AssociaIndennita_CS(cs.IDCOEFFICIENTESEDE, db, ibm.dataInizioValidita);
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
                                ibNew1 = new COEFFICIENTESEDE()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = Convert.ToDateTime(Utility.DataFineStop()),
                                    VALORECOEFFICIENTE = ibm.valore,
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                libNew.Add(ibNew1);
                                db.Database.BeginTransaction();
                                db.COEFFICIENTESEDE.Add(ibNew1);
                                db.SaveChanges();

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    dtrp.AssociaIndennita_CS(ibNew1.IDCOEFFICIENTESEDE, db, ibm.dataInizioValidita);
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
                                    ibNew1 = new COEFFICIENTESEDE()
                                    {
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = dataFineUltimo,
                                        // ALIQUOTA = ibm.aliquota,//nuova aliquota rispetto alla vecchia registrata
                                        VALORECOEFFICIENTE = ibm.valore,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.COEFFICIENTESEDE.Add(ibNew1);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        dtrp.AssociaIndennita_CS(ibNew1.IDCOEFFICIENTESEDE, db, ibm.dataInizioValidita);
                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                                //se il nuovo record rappresenta la data variazione superiore alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                if (ibm.dataInizioValidita > dataInizioUltimo)
                                {
                                    ibNew1 = new COEFFICIENTESEDE()
                                    {
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = ibm.dataInizioValidita.AddDays(-1),
                                        VALORECOEFFICIENTE = aliquotaUltimo,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    ibNew2 = new COEFFICIENTESEDE()
                                    {
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        VALORECOEFFICIENTE = ibm.valore,//nuova aliquota rispetto alla vecchia registrata
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1); libNew.Add(ibNew2);
                                    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                    db.Database.BeginTransaction();
                                    db.COEFFICIENTESEDE.AddRange(libNew);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        foreach (var cs in libNew)
                                        {
                                            dtrp.AssociaIndennita_CS(cs.IDCOEFFICIENTESEDE, db, ibm.dataInizioValidita);
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

        public bool EsistonoMovimentiPrima(CoefficientiSedeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.COEFFICIENTESEDE.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDUFFICIO == ibm.idUfficio && a.ANNULLATO == false).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(CoefficientiSedeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.COEFFICIENTESEDE.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(CoefficientiSedeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.COEFFICIENTESEDE.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiPrimaUguale(CoefficientiSedeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.COEFFICIENTESEDE.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
            }
        }

        public void DelCoefficientiSede_000(decimal idCoefficientiSede)
        {
            COEFFICIENTESEDE precedenteIB = new COEFFICIENTESEDE();
            COEFFICIENTESEDE delIB = new COEFFICIENTESEDE();


            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.COEFFICIENTESEDE.Where(a => a.IDCOEFFICIENTESEDE == idCoefficientiSede);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.COEFFICIENTESEDE.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new COEFFICIENTESEDE()
                            {
                                IDUFFICIO = precedenteIB.IDUFFICIO,

                                DATAINIZIOVALIDITA = precedenteIB.DATAFINEVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                VALORECOEFFICIENTE = precedenteIB.VALORECOEFFICIENTE,


                                ANNULLATO = false
                            };

                            db.COEFFICIENTESEDE.Add(ibOld1);
                        }

                        db.SaveChanges();

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro del coefficiente di sede.", "COEFFICENTISEDE", idCoefficientiSede);
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

        public void DelCoefficientiSede(decimal idCoefSede)
        {
            COEFFICIENTESEDE precedenteIB = new COEFFICIENTESEDE();
            COEFFICIENTESEDE delIB = new COEFFICIENTESEDE();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    var lib = db.COEFFICIENTESEDE.Where(a => a.IDCOEFFICIENTESEDE == idCoefSede);
                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;
                        RendiAnnullatoUnRecord(delIB.IDCOEFFICIENTESEDE, db);
                        precedenteIB = RestituisciIlRecordPrecedente(idCoefSede);
                        RendiAnnullatoUnRecord(precedenteIB.IDCOEFFICIENTESEDE, db);

                        var NuovoPrecedente = new COEFFICIENTESEDE()
                        {
                            IDUFFICIO = precedenteIB.IDUFFICIO,
                            DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                            //ALIQUOTA = precedenteIB.ALIQUOTA,
                            VALORECOEFFICIENTE = precedenteIB.VALORECOEFFICIENTE,
                            DATAAGGIORNAMENTO = DateTime.Now,// precedenteIB.DATAAGGIORNAMENTO,
                            ANNULLATO = false
                        };
                        db.COEFFICIENTESEDE.Add(NuovoPrecedente);

                        db.SaveChanges();

                        using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                        {
                            dtrp.AssociaIndennita_CS(NuovoPrecedente.IDCOEFFICIENTESEDE, db, delIB.DATAINIZIOVALIDITA);
                        }
                    }


                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di aliquote contributive.", "COEFFICIENTESEDE", idCoefSede);
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
            var fm = context.ObjectInstance as CoefficientiSedeModel;

            if (fm != null)
            {
                DateTime d = DataInizioMinimaNonAnnullataCoeffSede(fm.idUfficio);
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

        public bool CoefficientiSedeAnnullato(CoefficientiSedeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.COEFFICIENTESEDE.Where(a => a.IDCOEFFICIENTESEDE == ibm.idCoefficientiSede && a.IDUFFICIO == ibm.idUfficio).First().ANNULLATO == true ? true : false;
            }
        }

        public void DelCOEFFICIENTESEDE(decimal idCoefSede)
        {
            COEFFICIENTESEDE precedenteIB = new COEFFICIENTESEDE();
            COEFFICIENTESEDE delIB = new COEFFICIENTESEDE();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    var lib = db.COEFFICIENTESEDE.Where(a => a.IDCOEFFICIENTESEDE == idCoefSede);
                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;
                        RendiAnnullatoUnRecord(delIB.IDCOEFFICIENTESEDE, db);
                        precedenteIB = RestituisciIlRecordPrecedente(idCoefSede);
                        RendiAnnullatoUnRecord(precedenteIB.IDCOEFFICIENTESEDE, db);

                        var NuovoPrecedente = new COEFFICIENTESEDE()
                        {
                            IDUFFICIO = precedenteIB.IDUFFICIO,
                            DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                            VALORECOEFFICIENTE = precedenteIB.VALORECOEFFICIENTE,
                            DATAAGGIORNAMENTO = DateTime.Now,// precedenteIB.DATAAGGIORNAMENTO,
                            ANNULLATO = false
                        };
                        db.COEFFICIENTESEDE.Add(NuovoPrecedente);
                    }
                    db.SaveChanges();
                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di aliquote contributive.", "COEFFICIENTESEDE", idCoefSede);
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


        public bool COEFFICIENTESEDEAnnullato(CoefficientiSedeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.COEFFICIENTESEDE.Where(a => a.IDCOEFFICIENTESEDE == ibm.idCoefficientiSede && a.IDUFFICIO == ibm.idUfficio).First().ANNULLATO == true ? true : false;
            }
        }

        public static DateTime DataInizioMinimaNonAnnullataCoeffSede(decimal idLivello)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.COEFFICIENTESEDE.Where(a => a.ANNULLATO == false && a.IDUFFICIO == idLivello).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }

        public decimal Get_Id_COEFFICIENTESEDEPrimoNonAnnullato(decimal idUfficio)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTESEDE> libm = new List<COEFFICIENTESEDE>();
                libm = db.COEFFICIENTESEDE.Where(a => a.ANNULLATO == false
                && a.IDUFFICIO == idUfficio).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDCOEFFICIENTESEDE;
            }
            return tmp;
        }

        public COEFFICIENTESEDE RestituisciIlRecordPrecedente(decimal idCoefSede)
        {
            COEFFICIENTESEDE tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                COEFFICIENTESEDE interessato = new COEFFICIENTESEDE();
                interessato = db.COEFFICIENTESEDE.Find(idCoefSede);
                tmp = db.COEFFICIENTESEDE.Where(a => a.IDUFFICIO == interessato.IDUFFICIO
                && a.ANNULLATO == false).ToList().Where(b => b.DATAFINEVALIDITA == interessato.DATAINIZIOVALIDITA.AddDays(-1)).ToList().First();
            }
            return tmp;
        }
        public List<string> RestituisciIntervalloDiUnaData(DateTime DataCampione, decimal idUfficio)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTESEDE> libm = new List<COEFFICIENTESEDE>();
                libm = db.COEFFICIENTESEDE.Where(a => a.ANNULLATO == false
                && a.IDUFFICIO == idUfficio).ToList().Where(b =>
                b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())
                && DataCampione > b.DATAINIZIOVALIDITA
                && DataCampione < b.DATAFINEVALIDITA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDCOEFFICIENTESEDE.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].VALORECOEFFICIENTE.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione, decimal idUfficio)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTESEDE> libm = new List<COEFFICIENTESEDE>();
                libm = db.COEFFICIENTESEDE.Where(a => a.ANNULLATO == false
                && a.IDUFFICIO == idUfficio).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().Where(b => DataCampione == b.DATAINIZIOVALIDITA &&
                 b.DATAFINEVALIDITA != Utility.DataFineStop()).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDCOEFFICIENTESEDE.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].VALORECOEFFICIENTE.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione, decimal idUfficio)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTESEDE> libm = new List<COEFFICIENTESEDE>();
                libm = db.COEFFICIENTESEDE.Where(a => a.ANNULLATO == false
                && a.IDUFFICIO == idUfficio).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().
                Where(b => DataCampione == b.DATAFINEVALIDITA
                && b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())).ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDCOEFFICIENTESEDE.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].VALORECOEFFICIENTE.ToString());
                }
            }
            return tmp;
        }
        public List<string> RestituisciLaRigaMassima(decimal idUfficio)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTESEDE> libm = new List<COEFFICIENTESEDE>();
                libm = db.COEFFICIENTESEDE.Where(a => a.ANNULLATO == false
                && a.IDUFFICIO == idUfficio).ToList().Where(b =>
                b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop())).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDCOEFFICIENTESEDE.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].VALORECOEFFICIENTE.ToString());
                }
            }
            return tmp;
        }
        public void RendiAnnullatoUnRecord(decimal idCoeffSede, ModelDBISE db)
        {
            COEFFICIENTESEDE entita = new COEFFICIENTESEDE();
            entita = db.COEFFICIENTESEDE.Find(idCoeffSede);
            entita.ANNULLATO = true;
            db.SaveChanges();
        }
    }
}
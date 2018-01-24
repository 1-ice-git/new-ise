using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Dynamic;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParCoefficienteKm: IDisposable
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
                    var lib = db.COEFFICIENTEFKM.ToList();

                    libm = (from e in lib
                            select new CoeffFasciaKmModel()
                            {
                                idCfKm = e.IDCFKM,
                                idDefKm = e.IDDEFKM,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                               // dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new CoeffFasciaKmModel().dataFineValidita,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                coefficienteKm = e.COEFFICIENTEKM,
                                annullato = e.ANNULLATO,
                                km = new DefFasciaKmModel()
                                {
                                    idDefKm = e.DEFFASCIACHILOMETRICA.IDDEFKM,
                                    km = e.DEFFASCIACHILOMETRICA.KM
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

        public IList<CoeffFasciaKmModel> getListCoeffFasciaKm(decimal idCfKm)
        {
            List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.COEFFICIENTEFKM.Where(a => a.IDCFKM == idCfKm).ToList();

                    libm = (from e in lib
                            select new CoeffFasciaKmModel()
                            {
                                idCfKm = e.IDCFKM,
                                idDefKm = e.IDDEFKM,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                               // dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop ? e.DATAFINEVALIDITA : new CoeffFasciaKmModel().dataFineValidita,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                coefficienteKm = e.COEFFICIENTEKM,
                                annullato = e.ANNULLATO,
                                km = new DefFasciaKmModel()
                                {
                                    idDefKm = e.DEFFASCIACHILOMETRICA.IDDEFKM,
                                    km = e.DEFFASCIACHILOMETRICA.KM
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

        public IList<CoeffFasciaKmModel> getListCoeffFasciaKm(bool escludiAnnullati = false)
        {
            List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new CoeffFasciaKmModel()
                            {
                                idCfKm = e.IDCFKM,
                                idDefKm = e.IDDEFKM,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                               // dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new CoeffFasciaKmModel().dataFineValidita,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                coefficienteKm = e.COEFFICIENTEKM,
                                annullato = e.ANNULLATO,
                                km = new DefFasciaKmModel()
                                {
                                    idDefKm = e.DEFFASCIACHILOMETRICA.IDDEFKM,
                                    km = e.DEFFASCIACHILOMETRICA.KM
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
       
        public IList<CoeffFasciaKmModel> getListCoeffFasciaKm(decimal iddefkm, bool escludiAnnullati = false)
        {
            List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<COEFFICIENTEFKM> lib = new List<COEFFICIENTEFKM>();
                    
                    if(escludiAnnullati==true)
                        lib = db.COEFFICIENTEFKM.Where(a => a.IDDEFKM == iddefkm && a.ANNULLATO == false).ToList();
                    else
                        lib = db.COEFFICIENTEFKM.Where(a => a.IDDEFKM == iddefkm).ToList();

                    libm = (from e in lib
                            select new CoeffFasciaKmModel()
                            {
                                idCfKm = e.IDCFKM,
                                idDefKm = e.IDDEFKM,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                               // dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new CoeffFasciaKmModel().dataFineValidita,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                dataAggiornamento=e.DATAAGGIORNAMENTO,
                                coefficienteKm = e.COEFFICIENTEKM,
                                annullato = e.ANNULLATO,
                                km = new DefFasciaKmModel()
                                {
                                    idDefKm = e.DEFFASCIACHILOMETRICA.IDDEFKM,
                                    km = e.DEFFASCIACHILOMETRICA.KM
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
        public void SetCoeffFasciaKm___000(CoeffFasciaKmModel ibm)
        {
            List<COEFFICIENTEFKM> libNew = new List<COEFFICIENTEFKM>();

            COEFFICIENTEFKM ibNew = new COEFFICIENTEFKM();

            COEFFICIENTEFKM ibPrecedente = new COEFFICIENTEFKM();

            List<COEFFICIENTEFKM> lArchivioIB = new List<COEFFICIENTEFKM>();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    if (ibm.dataFineValidita.HasValue)
                    {
                        if (EsistonoMovimentiSuccessiviUguale(ibm))
                        {
                            ibNew = new COEFFICIENTEFKM()
                            {
                                //IDCFKM = ibm.idCfKm,
                                IDDEFKM = ibm.idDefKm,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                COEFFICIENTEKM = ibm.coefficienteKm,
                                DATAAGGIORNAMENTO = DateTime.Now,
                                ANNULLATO = ibm.annullato
                            };
                        }
                        else
                        {
                            ibNew = new COEFFICIENTEFKM()
                            {
                                //IDCFKM = ibm.idCfKm,
                                IDDEFKM = ibm.idDefKm,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                COEFFICIENTEKM = ibm.coefficienteKm,
                                DATAAGGIORNAMENTO = DateTime.Now,
                                ANNULLATO = ibm.annullato

                            };
                        }
                    }
                    else
                    {
                        ibNew = new COEFFICIENTEFKM()
                        {
                         //   IDCFKM = ibm.idCfKm,
                            IDDEFKM = ibm.idDefKm,
                            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            DATAFINEVALIDITA = ibm.dataFineValidita==null?Convert.ToDateTime(Utility.DataFineStop()):ibm.dataFineValidita.Value,
                            COEFFICIENTEKM = ibm.coefficienteKm,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            ANNULLATO = ibm.annullato
                        };
                    }

                    db.Database.BeginTransaction();

                    var recordInteressati = db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == false && a.IDCFKM == ibNew.IDCFKM)
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
                                    var ibOld1 = new COEFFICIENTEFKM()
                                    {
                                        
                                        //IDCFKM = item.IDCFKM,
                                        IDDEFKM =item.IDDEFKM,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(-1),
                                        COEFFICIENTEKM = item.COEFFICIENTEKM,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);

                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new COEFFICIENTEFKM()
                                    {
                                        //IDCFKM = item.IDCFKM,
                                        IDDEFKM = item.IDDEFKM,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(-1),
                                        COEFFICIENTEKM = item.COEFFICIENTEKM,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        ANNULLATO = false
                                    };

                                    var ibOld2 = new COEFFICIENTEFKM()
                                    {
                                        //IDCFKM = item.IDCFKM,
                                        IDDEFKM = item.IDDEFKM,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(-1),
                                        COEFFICIENTEKM = item.COEFFICIENTEKM,
                                        DATAAGGIORNAMENTO = DateTime.Now,
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
                                    var ibOld1 = new COEFFICIENTEFKM()
                                    {
                                        //IDCFKM = item.IDCFKM,
                                        IDDEFKM = item.IDDEFKM,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        COEFFICIENTEKM = item.COEFFICIENTEKM,
                                        DATAAGGIORNAMENTO = DateTime.Now,
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
                                    var ibOld1 = new COEFFICIENTEFKM()
                                    {
                                        //IDCFKM = item.IDCFKM,
                                        IDDEFKM = item.IDDEFKM,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        COEFFICIENTEKM = item.COEFFICIENTEKM,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                }
                            }
                        }

                        libNew.Add(ibNew);
                        libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        db.COEFFICIENTEFKM.AddRange(libNew);
                    }
                    else
                    {
                        db.COEFFICIENTEFKM.Add(ibNew);

                    }
                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro coefficiente di fascia km.", "COEFFICIENTEFKM", ibNew.IDCFKM);
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


        public void SetCoeffFasciaKm(CoeffFasciaKmModel ibm, bool aggiornaTutto)
        {
            List<COEFFICIENTEFKM> libNew = new List<COEFFICIENTEFKM>();

            COEFFICIENTEFKM ibPrecedente = new COEFFICIENTEFKM();
            COEFFICIENTEFKM ibNew1 = new COEFFICIENTEFKM();
            COEFFICIENTEFKM ibNew2 = new COEFFICIENTEFKM();
            List<COEFFICIENTEFKM> lArchivioIB = new List<COEFFICIENTEFKM>();
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
                            decimal aliquotaFirst = Convert.ToDecimal(lista[3]);

                            ibNew1 = new COEFFICIENTEFKM()
                            {
                                IDDEFKM = ibm.idDefKm,
                                DATAINIZIOVALIDITA = dataInizioFirst,
                                DATAFINEVALIDITA = dataFineFirst,
                                COEFFICIENTEKM = ibm.coefficienteKm,
                                DATAAGGIORNAMENTO = DateTime.Now,
                            };

                            if (aggiornaTutto)
                            {
                                ibNew1 = new COEFFICIENTEFKM()
                                {
                                    IDDEFKM = ibm.idDefKm,
                                    DATAINIZIOVALIDITA = dataInizioFirst,
                                    DATAFINEVALIDITA = Utility.DataFineStop(),
                                    COEFFICIENTEKM = ibm.idDefKm,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                //qui annullo tutti i record rimanenti dalla data inizio inserita
                                libNew = db.COEFFICIENTEFKM.Where(a => a.IDDEFKM == ibm.idDefKm
                                && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > dataInizioFirst).ToList();
                                foreach (var elem in libNew)
                                {
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDCFKM), db);
                                }
                            }
                            db.Database.BeginTransaction();
                            db.COEFFICIENTEFKM.Add(ibNew1);
                            db.SaveChanges();
                            RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloFirst), db);

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

                                ibNew1 = new COEFFICIENTEFKM()
                                {
                                    IDDEFKM = ibm.idDefKm,
                                    DATAINIZIOVALIDITA = dataInizioLast,
                                    DATAFINEVALIDITA = dataFineLast.AddDays(-1),
                                    COEFFICIENTEKM = aliquotaLast,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new COEFFICIENTEFKM()
                                {
                                    IDDEFKM = ibm.idDefKm,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = ibm.dataInizioValidita,//è uguale alla data Inizio
                                    COEFFICIENTEKM = ibm.coefficienteKm,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };
                                if (aggiornaTutto)
                                {
                                    ibNew2 = new COEFFICIENTEFKM()
                                    {
                                        IDDEFKM = ibm.idDefKm,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        COEFFICIENTEKM = ibm.coefficienteKm,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.COEFFICIENTEFKM.Where(a => a.IDDEFKM == ibm.idDefKm
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDCFKM), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                db.Database.BeginTransaction();
                                db.COEFFICIENTEFKM.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloLast), db);
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

                                ibNew1 = new COEFFICIENTEFKM()
                                {
                                    IDDEFKM = ibm.idDefKm,
                                    DATAINIZIOVALIDITA = dataInizio,
                                    DATAFINEVALIDITA = NewdataFine1,
                                    COEFFICIENTEKM = aliquota,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new COEFFICIENTEFKM()
                                {
                                    IDDEFKM = ibm.idDefKm,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = dataFine,
                                    COEFFICIENTEKM = ibm.coefficienteKm,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };

                                if (aggiornaTutto)
                                {
                                    ibNew2 = new COEFFICIENTEFKM()
                                    {
                                        IDDEFKM = ibm.idDefKm,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        COEFFICIENTEKM = ibm.coefficienteKm,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.COEFFICIENTEFKM.Where(a => a.IDDEFKM == ibm.idDefKm
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDCFKM), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                db.Database.BeginTransaction();
                                db.COEFFICIENTEFKM.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervallo), db);
                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //CASO DELL'ULTIMA RIGA CON LA DATA FINE UGUALE A 31/12/9999
                        if (giafatta == false)
                        {
                            lista = dtal.RestituisciLaRigaMassima(ibm.idDefKm);
                            decimal idIntervalloUltimo = Convert.ToDecimal(lista[0]);
                            DateTime dataInizioUltimo = Convert.ToDateTime(lista[1]);
                            DateTime dataFineUltimo = Convert.ToDateTime(lista[2]);
                            decimal aliquotaUltimo = Convert.ToDecimal(lista[3]);
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                //se il nuovo record rappresenta la data variazione uguale alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                //occorre annullare il record esistente in questione ed aggiungere un nuovo con la stessa data inizio e l'altro campo da aggiornare con il nuovo
                                if (dataInizioUltimo == ibm.dataInizioValidita)
                                {
                                    ibNew1 = new COEFFICIENTEFKM()
                                    {
                                        IDDEFKM = ibm.idDefKm,
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = dataFineUltimo,
                                        COEFFICIENTEKM = ibm.coefficienteKm,//nuova aliquota rispetto alla vecchia registrata
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.COEFFICIENTEFKM.Add(ibNew1);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);
                                    db.Database.CurrentTransaction.Commit();
                                }
                                //se il nuovo record rappresenta la data variazione superiore alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                if (ibm.dataInizioValidita > dataInizioUltimo)
                                {
                                    ibNew1 = new COEFFICIENTEFKM()
                                    {
                                        IDDEFKM = ibm.idDefKm,
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = ibm.dataInizioValidita.AddDays(-1),
                                        COEFFICIENTEKM = aliquotaUltimo,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    ibNew2 = new COEFFICIENTEFKM()
                                    {
                                        IDDEFKM = ibm.idDefKm,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        COEFFICIENTEKM = ibm.coefficienteKm,//nuova aliquota rispetto alla vecchia registrata
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1); libNew.Add(ibNew2);
                                    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                    db.Database.BeginTransaction();
                                    db.COEFFICIENTEFKM.AddRange(libNew);
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

        public bool EsistonoMovimentiPrima(CoeffFasciaKmModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == false && a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDCFKM == ibm.idCfKm).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(CoeffFasciaKmModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == false && a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDCFKM == ibm.idCfKm).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(CoeffFasciaKmModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == false && a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDCFKM == ibm.idCfKm).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiPrimaUguale(CoeffFasciaKmModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDCFKM == ibm.idCfKm).Count() > 0 ? true : false;
            }
        }
       
        public void DelCoeffFasciaKm(decimal IDCFKM)
        {
            COEFFICIENTEFKM precedenteIB = new COEFFICIENTEFKM();
            COEFFICIENTEFKM delIB = new COEFFICIENTEFKM();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    var lib = db.COEFFICIENTEFKM.Where(a => a.IDCFKM == IDCFKM);
                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;
                        RendiAnnullatoUnRecord(delIB.IDCFKM, db);
                        precedenteIB = RestituisciIlRecordPrecedente(IDCFKM);
                        RendiAnnullatoUnRecord(precedenteIB.IDCFKM, db);

                        var NuovoPrecedente = new COEFFICIENTEFKM()
                        {
                            IDDEFKM = precedenteIB.IDDEFKM,
                            DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                            COEFFICIENTEKM = precedenteIB.COEFFICIENTEKM,
                            DATAAGGIORNAMENTO = DateTime.Now,// precedenteIB.DATAAGGIORNAMENTO,
                            ANNULLATO = false
                        };
                        db.COEFFICIENTEFKM.Add(NuovoPrecedente);
                    }
                    db.SaveChanges();
                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di Coefficiente Fascia KM.", "ALIQUOTECONTRIBUTIVE", IDCFKM);
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
                return db.COEFFICIENTEFKM.Where(a => a.IDCFKM == ibm.idCfKm && a.IDDEFKM == ibm.idDefKm).First().ANNULLATO == true ? true : false;
            }
        }

        
        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as CoeffFasciaKmModel;

            if (fm != null)
            {
                DateTime d = DataInizioMinimaNonAnnullataIndennitaBase(fm.idDefKm);
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


        public static DateTime DataInizioMinimaNonAnnullataIndennitaBase(decimal idLivello)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == false && a.IDDEFKM == idLivello).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }
        public COEFFICIENTEFKM RestituisciIlRecordPrecedente(decimal IDCFKM)
        {
            COEFFICIENTEFKM tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                COEFFICIENTEFKM interessato = new COEFFICIENTEFKM();
                interessato = db.COEFFICIENTEFKM.Find(IDCFKM);
                tmp = db.COEFFICIENTEFKM.Where(a => a.IDDEFKM == interessato.IDDEFKM
                && a.ANNULLATO == false).ToList().Where(b => b.DATAFINEVALIDITA == interessato.DATAINIZIOVALIDITA.AddDays(-1)).ToList().First();
            }
            return tmp;
        }
        public List<string> RestituisciIntervalloDiUnaData(DateTime DataCampione, decimal IDDEFKM)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTEFKM> libm = new List<COEFFICIENTEFKM>();
                libm = db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == false
                && a.IDDEFKM == IDDEFKM).ToList().Where(b =>
                b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())
                && DataCampione > b.DATAINIZIOVALIDITA
                && DataCampione < b.DATAFINEVALIDITA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDCFKM.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTEKM.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione, decimal IDDEFKM)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTEFKM> libm = new List<COEFFICIENTEFKM>();
                libm = db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == false
                && a.IDDEFKM == IDDEFKM).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().Where(b => DataCampione == b.DATAINIZIOVALIDITA &&
                 b.DATAFINEVALIDITA != Utility.DataFineStop()).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDCFKM.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTEKM.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione, decimal IDDEFKM)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            { 
                List<COEFFICIENTEFKM> libm = new List<COEFFICIENTEFKM>();
                libm = db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == false
                && a.IDDEFKM == IDDEFKM).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().
                Where(b => DataCampione == b.DATAFINEVALIDITA
                && b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())).ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDCFKM.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTEKM.ToString());
                }
            }
            return tmp;
        }
        public List<string> RestituisciLaRigaMassima(decimal IDDEFKM)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTEFKM> libm = new List<COEFFICIENTEFKM>();
                libm = db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == false
                && a.IDDEFKM == IDDEFKM).ToList().Where(b =>
                b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop())).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDCFKM.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTEKM.ToString());
                }
            }
            return tmp;
        }
        public void RendiAnnullatoUnRecord(decimal IDCFKM, ModelDBISE db)
        {
            COEFFICIENTEFKM entita = new COEFFICIENTEFKM();
            entita = db.COEFFICIENTEFKM.Find(IDCFKM);
            entita.ANNULLATO = true;
            db.SaveChanges();
        }
        public decimal Get_Id_CoefficienteFasciaKmNonAnnullato(decimal iddefkm)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTEFKM> libm = new List<COEFFICIENTEFKM>();
                libm = db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == false
                && a.IDDEFKM == iddefkm).OrderBy(a => a.DATAINIZIOVALIDITA).ThenBy(a => a.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDCFKM;
            }
            return tmp;
        }

    }
}
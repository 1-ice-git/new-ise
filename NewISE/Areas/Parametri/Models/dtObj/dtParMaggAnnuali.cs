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
    public class dtParMaggAnnuali : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public IList<MaggiorazioniAnnualiModel> getListMaggiorazioneAnnuale()
        {
            List<MaggiorazioniAnnualiModel> libm = new List<MaggiorazioniAnnualiModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.MAGGIORAZIONIANNUALI.ToList();

                    libm = (from e in lib
                            select new MaggiorazioniAnnualiModel()
                            {
                                idMagAnnuali = e.IDMAGANNUALI,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,// != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new MaggiorazioniAnnualiModel().dataFineValidita,
                                annualita = e.ANNUALITA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                DescrizioneUfficio = new UfficiModel()
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

        public IList<MaggiorazioniAnnualiModel> getListMaggiorazioneAnnuale(decimal idUfficio)
        {
            List<MaggiorazioniAnnualiModel> libm = new List<MaggiorazioniAnnualiModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.MAGGIORAZIONIANNUALI.Where(a => a.IDUFFICIO == idUfficio).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();

                    libm = (from e in lib
                            select new MaggiorazioniAnnualiModel()
                            {
                                idMagAnnuali = e.IDMAGANNUALI,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,// != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new MaggiorazioniAnnualiModel().dataFineValidita,
                                annualita = e.ANNUALITA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                DescrizioneUfficio = new UfficiModel()
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

        public IList<MaggiorazioniAnnualiModel> getListMaggiorazioneAnnuale(bool escludiAnnullati = false)
        {
            List<MaggiorazioniAnnualiModel> libm = new List<MaggiorazioniAnnualiModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<MAGGIORAZIONIANNUALI> lib = new List<MAGGIORAZIONIANNUALI>();
                    if (escludiAnnullati == true)
                        db.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
                    else
                        db.MAGGIORAZIONIANNUALI.ToList().OrderBy(b => b.DATAINIZIOVALIDITA);

                    libm = (from e in lib
                            select new MaggiorazioniAnnualiModel()
                            {
                                idMagAnnuali = e.IDMAGANNUALI,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,// != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new MaggiorazioniAnnualiModel().dataFineValidita,
                                annualita = e.ANNUALITA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                DescrizioneUfficio = new UfficiModel()
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

        public IList<MaggiorazioniAnnualiModel> getListMaggiorazioneAnnuale(decimal idUfficio, bool escludiAnnullati = false)
        {
            List<MaggiorazioniAnnualiModel> libm = new List<MaggiorazioniAnnualiModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<MAGGIORAZIONIANNUALI> lib = new List<MAGGIORAZIONIANNUALI>();
                    if (escludiAnnullati == true)
                        lib = db.MAGGIORAZIONIANNUALI.Where(a => a.IDUFFICIO == idUfficio && a.ANNULLATO == false).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
                    else
                        lib = db.MAGGIORAZIONIANNUALI.Where(a => a.IDUFFICIO == idUfficio).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();

                    libm = (from e in lib
                            select new MaggiorazioniAnnualiModel()
                            {
                                idMagAnnuali = e.IDMAGANNUALI,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,//!= Utility.DataFineStop() ? e.DATAFINEVALIDITA : new MaggiorazioniAnnualiModel().dataFineValidita,
                                annualita = e.ANNUALITA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                DescrizioneUfficio = new UfficiModel()
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
        public void SetMaggiorazioneAnnuale(MaggiorazioniAnnualiModel ibm, bool aggiornaTutto)
        {
            List<MAGGIORAZIONIANNUALI> libNew = new List<MAGGIORAZIONIANNUALI>();

            //MAGGIORAZIONIANNUALI ibPrecedente = new MAGGIORAZIONIANNUALI();
            MAGGIORAZIONIANNUALI ibNew1 = new MAGGIORAZIONIANNUALI();
            MAGGIORAZIONIANNUALI ibNew2 = new MAGGIORAZIONIANNUALI();
            //List<MAGGIORAZIONIANNUALI> lArchivioIB = new List<MAGGIORAZIONIANNUALI>();

            List<string> lista = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                bool giafatta = false;
                try
                {
                    using (dtParMaggAnnuali dtal = new dtParMaggAnnuali())
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

                            ibNew1 = new MAGGIORAZIONIANNUALI()
                            {
                                IDUFFICIO = ibm.idUfficio,
                                DATAINIZIOVALIDITA = dataInizioFirst,
                                DATAFINEVALIDITA = dataFineFirst,
                                ANNUALITA = ibm.annualita,
                                DATAAGGIORNAMENTO = DateTime.Now,
                            };

                            if (aggiornaTutto)
                            {
                                ibNew1 = new MAGGIORAZIONIANNUALI()
                                {
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = dataInizioFirst,
                                    DATAFINEVALIDITA = Utility.DataFineStop(),
                                    ANNUALITA = ibm.annualita,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                //qui annullo tutti i record rimanenti dalla data inizio inserita
                                libNew = db.MAGGIORAZIONIANNUALI.Where(a => a.IDUFFICIO == ibm.idUfficio
                                && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > dataInizioFirst).ToList();
                                foreach (var elem in libNew)
                                {
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDMAGANNUALI), db);
                                }
                            }
                            db.Database.BeginTransaction();
                            db.MAGGIORAZIONIANNUALI.Add(ibNew1);
                            db.SaveChanges();
                            RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloFirst), db);

                            using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                            {
                                dtrp.AssociaMaggiorazioniAbitazione_MA(ibNew1.IDMAGANNUALI, db, ibm.dataInizioValidita);
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
                                decimal COEFFICIENTELast = Convert.ToDecimal(lista[3]);

                                ibNew1 = new MAGGIORAZIONIANNUALI()
                                {
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = dataInizioLast,
                                    DATAFINEVALIDITA = dataFineLast.AddDays(-1),
                                    ANNUALITA = ibm.annualita,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new MAGGIORAZIONIANNUALI()
                                {
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = ibm.dataInizioValidita,//è uguale alla data Inizio
                                    ANNUALITA = ibm.annualita,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };
                                if (aggiornaTutto)
                                {
                                    ibNew2 = new MAGGIORAZIONIANNUALI()
                                    {
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        ANNUALITA = ibm.annualita,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.MAGGIORAZIONIANNUALI.Where(a => a.IDUFFICIO == ibm.idUfficio
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDMAGANNUALI), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                db.Database.BeginTransaction();
                                db.MAGGIORAZIONIANNUALI.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloLast), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var ma in libNew)
                                    {
                                        dtrp.AssociaMaggiorazioniAbitazione_MA(ma.IDMAGANNUALI, db, ibm.dataInizioValidita);
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
                                //bool Annualita = Convert.ToBoolean(Convert.ToDecimal(lista[3]));

                                DateTime NewdataFine1 = ibm.dataInizioValidita.AddDays(-1);

                                ibNew1 = new MAGGIORAZIONIANNUALI()
                                {
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = dataInizio,
                                    DATAFINEVALIDITA = NewdataFine1,
                                    ANNUALITA = ibm.annualita,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new MAGGIORAZIONIANNUALI()
                                {
                                    IDUFFICIO = ibm.idUfficio,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = dataFine,
                                    ANNUALITA = ibm.annualita,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };

                                if (aggiornaTutto)
                                {
                                    ibNew2 = new MAGGIORAZIONIANNUALI()
                                    {
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        ANNUALITA = ibm.annualita,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.MAGGIORAZIONIANNUALI.Where(a => a.IDUFFICIO == ibm.idUfficio
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDMAGANNUALI), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                db.Database.BeginTransaction();
                                db.MAGGIORAZIONIANNUALI.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervallo), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var ma in libNew)
                                    {
                                        dtrp.AssociaMaggiorazioniAbitazione_MA(ma.IDMAGANNUALI, db, ibm.dataInizioValidita);
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
                                ibNew1 = new MAGGIORAZIONIANNUALI()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = Convert.ToDateTime(Utility.DataFineStop()),
                                    ANNUALITA = ibm.annualita,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    IDUFFICIO = ibm.idUfficio,
                                };
                                libNew.Add(ibNew1);
                                db.Database.BeginTransaction();
                                db.MAGGIORAZIONIANNUALI.Add(ibNew1);
                                db.SaveChanges();

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    dtrp.AssociaMaggiorazioniAbitazione_MA(ibNew1.IDMAGANNUALI, db, ibm.dataInizioValidita);
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
                                bool AnnualitaUltimo = Convert.ToBoolean(Convert.ToDecimal(lista[3]));
                                if (dataInizioUltimo == ibm.dataInizioValidita)
                                {
                                    ibNew1 = new MAGGIORAZIONIANNUALI()
                                    {
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = dataFineUltimo,
                                        ANNUALITA = ibm.annualita,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.MAGGIORAZIONIANNUALI.Add(ibNew1);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        dtrp.AssociaMaggiorazioniAbitazione_MA(ibNew1.IDMAGANNUALI, db, ibm.dataInizioValidita);
                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                                //se il nuovo record rappresenta la data variazione superiore alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                if (ibm.dataInizioValidita > dataInizioUltimo)
                                {
                                    ibNew1 = new MAGGIORAZIONIANNUALI()
                                    {
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = ibm.dataInizioValidita.AddDays(-1),
                                        ANNUALITA = AnnualitaUltimo,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    ibNew2 = new MAGGIORAZIONIANNUALI()
                                    {
                                        IDUFFICIO = ibm.idUfficio,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        ANNUALITA = ibm.annualita,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1); libNew.Add(ibNew2);
                                    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                    db.Database.BeginTransaction();
                                    db.MAGGIORAZIONIANNUALI.AddRange(libNew);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        foreach (var ma in libNew)
                                        {
                                            dtrp.AssociaMaggiorazioniAbitazione_MA(ma.IDMAGANNUALI, db, ibm.dataInizioValidita);
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


        public bool EsistonoMovimentiPrima(MaggiorazioniAnnualiModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.MAGGIORAZIONIANNUALI.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(MaggiorazioniAnnualiModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.MAGGIORAZIONIANNUALI.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(MaggiorazioniAnnualiModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.MAGGIORAZIONIANNUALI.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiPrimaUguale(MaggiorazioniAnnualiModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.MAGGIORAZIONIANNUALI.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
            }
        }

        public void DelMaggiorazioneAnnuale(decimal idMagAnnuali)
        {
            MAGGIORAZIONIANNUALI precedenteIB = new MAGGIORAZIONIANNUALI();
            MAGGIORAZIONIANNUALI delIB = new MAGGIORAZIONIANNUALI();


            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.MAGGIORAZIONIANNUALI.Where(a => a.IDMAGANNUALI == idMagAnnuali);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.MAGGIORAZIONIANNUALI.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new MAGGIORAZIONIANNUALI()
                            {
                                IDUFFICIO = precedenteIB.IDUFFICIO,

                                DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                ANNUALITA = precedenteIB.ANNUALITA,
                                DATAAGGIORNAMENTO = precedenteIB.DATAAGGIORNAMENTO,
                                ANNULLATO = false
                            };

                            db.MAGGIORAZIONIANNUALI.Add(ibOld1);

                            db.SaveChanges();

                            using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                            {
                                dtrp.AssociaMaggiorazioniAbitazione_MA(ibOld1.IDMAGANNUALI, db, delIB.DATAINIZIOVALIDITA);
                            }
                        }



                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro maggiorazioni annuali.", "MAGGIORAZIONIANNUALI", idMagAnnuali);
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
        public bool MaggiorazioniAnnualiAnnullato(MaggiorazioniAnnualiModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.MAGGIORAZIONIANNUALI.Where(a => a.IDMAGANNUALI == ibm.idMagAnnuali && a.IDUFFICIO == ibm.idUfficio).First().ANNULLATO == true ? true : false;
            }
        }

        public decimal Get_Id_MaggAnnualiNonAnnullato(decimal idLivello)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<MAGGIORAZIONIANNUALI> libm = new List<MAGGIORAZIONIANNUALI>();
                libm = db.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false
                && a.IDUFFICIO == idLivello).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDMAGANNUALI;
            }
            return tmp;
        }

        public void DelIndennitaSistemazione(decimal IDMAGANNUALI)
        {
            MAGGIORAZIONIANNUALI precedenteIB = new MAGGIORAZIONIANNUALI();
            MAGGIORAZIONIANNUALI delIB = new MAGGIORAZIONIANNUALI();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    var lib = db.MAGGIORAZIONIANNUALI.Where(a => a.IDMAGANNUALI == IDMAGANNUALI);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.MAGGIORAZIONIANNUALI.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new MAGGIORAZIONIANNUALI()
                            {

                                IDUFFICIO = precedenteIB.IDUFFICIO,
                                DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                //  = precedenteIB.,
                                DATAAGGIORNAMENTO = precedenteIB.DATAAGGIORNAMENTO,
                                ANNULLATO = false
                            };
                            db.MAGGIORAZIONIANNUALI.Add(ibOld1);
                        }
                        db.SaveChanges();
                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di Maggiorazioni annuali", "MAGGIORAZIONIANNUALI", IDMAGANNUALI);
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

        public void DelMAGGIORAZIONIANNUALI(decimal IDMAGANNUALI)
        {
            MAGGIORAZIONIANNUALI precedenteIB = new MAGGIORAZIONIANNUALI();
            MAGGIORAZIONIANNUALI delIB = new MAGGIORAZIONIANNUALI();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    var lib = db.MAGGIORAZIONIANNUALI.Where(a => a.IDMAGANNUALI == IDMAGANNUALI);
                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;
                        RendiAnnullatoUnRecord(delIB.IDMAGANNUALI, db);
                        precedenteIB = RestituisciIlRecordPrecedente(IDMAGANNUALI);
                        RendiAnnullatoUnRecord(precedenteIB.IDMAGANNUALI, db);

                        var NuovoPrecedente = new MAGGIORAZIONIANNUALI()
                        {
                            IDUFFICIO = precedenteIB.IDUFFICIO,
                            DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                            // COEFFICIENTE = precedenteIB.COEFFICIENTE,
                            DATAAGGIORNAMENTO = DateTime.Now,// precedenteIB.DATAAGGIORNAMENTO,
                            ANNULLATO = false
                        };
                        db.MAGGIORAZIONIANNUALI.Add(NuovoPrecedente);
                    }
                    db.SaveChanges();
                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di Maggiorazioni annuali", "MAGGIORAZIONIANNUALI", IDMAGANNUALI);
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
        public bool MAGGIORAZIONIANNUALIAnnullato(MaggiorazioniAnnualiModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.MAGGIORAZIONIANNUALI.Where(a => a.IDMAGANNUALI == ibm.idMagAnnuali && a.IDUFFICIO == ibm.idUfficio).First().ANNULLATO == true ? true : false;
            }
        }
        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as MaggiorazioniAnnualiModel;

            if (fm != null)
            {
                DateTime d = DataInizioMinimaNonAnnullataIndennitaBase(fm.idUfficio);
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

        public static DateTime DataInizioMinimaNonAnnullataIndennitaBase(decimal idUfficio)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false && a.IDUFFICIO == idUfficio).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }
        public MAGGIORAZIONIANNUALI RestituisciIlRecordPrecedente(decimal idIndSist)
        {
            MAGGIORAZIONIANNUALI tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                MAGGIORAZIONIANNUALI interessato = new MAGGIORAZIONIANNUALI();
                interessato = db.MAGGIORAZIONIANNUALI.Find(idIndSist);
                tmp = db.MAGGIORAZIONIANNUALI.Where(a => a.IDUFFICIO == interessato.IDUFFICIO
                && a.ANNULLATO == false).ToList().Where(b => b.DATAFINEVALIDITA == interessato.DATAINIZIOVALIDITA.AddDays(-1)).ToList().First();
            }
            return tmp;
        }
        public List<string> RestituisciIntervalloDiUnaData(DateTime DataCampione, decimal idUfficio)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<MAGGIORAZIONIANNUALI> libm = new List<MAGGIORAZIONIANNUALI>();
                libm = db.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false
                && a.IDUFFICIO == idUfficio).ToList().Where(b =>
                b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())
                && DataCampione > b.DATAINIZIOVALIDITA
                && DataCampione < b.DATAFINEVALIDITA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDMAGANNUALI.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(Convert.ToDecimal(libm[0].ANNUALITA).ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione, decimal idUfficio)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<MAGGIORAZIONIANNUALI> libm = new List<MAGGIORAZIONIANNUALI>();
                libm = db.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false
                && a.IDUFFICIO == idUfficio).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().Where(b => DataCampione == b.DATAINIZIOVALIDITA &&
                 b.DATAFINEVALIDITA != Utility.DataFineStop()).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDMAGANNUALI.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(Convert.ToDecimal(libm[0].ANNUALITA).ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione, decimal idUfficio)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<MAGGIORAZIONIANNUALI> libm = new List<MAGGIORAZIONIANNUALI>();
                libm = db.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false
                && a.IDUFFICIO == idUfficio).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().
                Where(b => DataCampione == b.DATAFINEVALIDITA
                && b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())).ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDMAGANNUALI.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(Convert.ToDecimal(libm[0].ANNUALITA).ToString());
                }
            }
            return tmp;
        }
        public List<string> RestituisciLaRigaMassima(decimal idUfficio)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<MAGGIORAZIONIANNUALI> libm = new List<MAGGIORAZIONIANNUALI>();
                libm = db.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false
                && a.IDUFFICIO == idUfficio).ToList().Where(b =>
                b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop())).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDMAGANNUALI.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(Convert.ToDecimal(libm[0].ANNUALITA).ToString());
                }
            }
            return tmp;
        }
        public void RendiAnnullatoUnRecord(decimal idIndMaggAnn, ModelDBISE db)
        {
            MAGGIORAZIONIANNUALI entita = new MAGGIORAZIONIANNUALI();
            entita = db.MAGGIORAZIONIANNUALI.Find(idIndMaggAnn);
            entita.ANNULLATO = true;
            db.SaveChanges();
        }
    }
}
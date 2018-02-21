using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Ricalcoli;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParIndSist : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public IList<IndennitaSistemazioneModel> getListIndennitaSistemazione()
        {
            List<IndennitaSistemazioneModel> libm = new List<IndennitaSistemazioneModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.INDENNITASISTEMAZIONE.ToList();

                    libm = (from e in lib
                            select new IndennitaSistemazioneModel()
                            {
                                idIndSist = e.IDINDSIST,
                                idTipoTrasferimento = e.IDTIPOTRASFERIMENTO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,// != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new IndennitaSistemazioneModel().dataFineValidita,
                                coefficiente = e.COEFFICIENTE,
                                dataAggiornamento = System.DateTime.Now,
                                annullato = e.ANNULLATO,
                                TipoTrasferimento = new TipoTrasferimentoModel()
                                {
                                    idTipoTrasferimento = e.IDTIPOTRASFERIMENTO,
                                    descTipoTrasf = e.TIPOTRASFERIMENTO.ToString()
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

        public IList<IndennitaSistemazioneModel> getListIndennitaSistemazione(decimal idTipoTrasferimento)
        {
            List<IndennitaSistemazioneModel> libm = new List<IndennitaSistemazioneModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.INDENNITASISTEMAZIONE.Where(a => a.IDTIPOTRASFERIMENTO == idTipoTrasferimento).ToList();

                    libm = (from e in lib
                            select new IndennitaSistemazioneModel()
                            {

                                idIndSist = e.IDINDSIST,
                                idTipoTrasferimento = e.IDTIPOTRASFERIMENTO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,// != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new IndennitaSistemazioneModel().dataFineValidita,
                                coefficiente = e.COEFFICIENTE,
                                dataAggiornamento = System.DateTime.Now,
                                annullato = e.ANNULLATO,
                                TipoTrasferimento = new TipoTrasferimentoModel()
                                {
                                    idTipoTrasferimento = e.IDTIPOTRASFERIMENTO,
                                    descTipoTrasf = e.TIPOTRASFERIMENTO.ToString()
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

        public IList<IndennitaSistemazioneModel> getListIndennitaSistemazione(bool escludiAnnullati = false)
        {
            List<IndennitaSistemazioneModel> libm = new List<IndennitaSistemazioneModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.INDENNITASISTEMAZIONE.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new IndennitaSistemazioneModel()
                            {

                                idIndSist = e.IDINDSIST,
                                idTipoTrasferimento = e.IDTIPOTRASFERIMENTO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,//!= Utility.DataFineStop() ? e.DATAFINEVALIDITA : new IndennitaSistemazioneModel().dataFineValidita,
                                coefficiente = e.COEFFICIENTE,
                                dataAggiornamento = System.DateTime.Now,
                                annullato = e.ANNULLATO,
                                TipoTrasferimento = new TipoTrasferimentoModel()
                                {
                                    idTipoTrasferimento = e.IDTIPOTRASFERIMENTO,
                                    descTipoTrasf = e.TIPOTRASFERIMENTO.ToString()
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

        public IList<IndennitaSistemazioneModel> getListIndennitaSistemazione(decimal idTipoTrasferimento, bool escludiAnnullati = false)
        {
            List<IndennitaSistemazioneModel> libm = new List<IndennitaSistemazioneModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<INDENNITASISTEMAZIONE> lib = new List<INDENNITASISTEMAZIONE>();
                    if (escludiAnnullati == true)
                        lib = db.INDENNITASISTEMAZIONE.Where(a => a.IDTIPOTRASFERIMENTO == idTipoTrasferimento && a.ANNULLATO == false).ToList();
                    else
                        lib = db.INDENNITASISTEMAZIONE.Where(a => a.IDTIPOTRASFERIMENTO == idTipoTrasferimento).ToList();

                    libm = (from e in lib
                            select new IndennitaSistemazioneModel()
                            {
                                idIndSist = e.IDINDSIST,
                                idTipoTrasferimento = e.IDTIPOTRASFERIMENTO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,// != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new IndennitaSistemazioneModel().dataFineValidita,
                                coefficiente = e.COEFFICIENTE,
                                dataAggiornamento = DateTime.Now,
                                annullato = e.ANNULLATO,
                                TipoTrasferimento = new TipoTrasferimentoModel()
                                {
                                    idTipoTrasferimento = e.TIPOTRASFERIMENTO.IDTIPOTRASFERIMENTO,
                                    descTipoTrasf = e.TIPOTRASFERIMENTO.TIPOTRASFERIMENTO1
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

        public void SetIndennitaSistemazione(IndennitaSistemazioneModel ibm, bool aggiornaTutto)
        {
            List<INDENNITASISTEMAZIONE> libNew = new List<INDENNITASISTEMAZIONE>();

            //INDENNITASISTEMAZIONE ibPrecedente = new INDENNITASISTEMAZIONE();
            INDENNITASISTEMAZIONE ibNew1 = new INDENNITASISTEMAZIONE();
            INDENNITASISTEMAZIONE ibNew2 = new INDENNITASISTEMAZIONE();
            //List<INDENNITASISTEMAZIONE> lArchivioIB = new List<INDENNITASISTEMAZIONE>();
            List<string> lista = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                bool giafatta = false;
                try
                {
                    using (dtParIndSist dtal = new dtParIndSist())
                    {
                        //Se la data variazione coincide con una data inizio esistente
                        lista = dtal.DataVariazioneCoincideConDataInizio(ibm.dataInizioValidita, ibm.idTipoTrasferimento);
                        if (lista.Count != 0)
                        {
                            giafatta = true;
                            decimal idIntervalloFirst = Convert.ToDecimal(lista[0]);
                            DateTime dataInizioFirst = Convert.ToDateTime(lista[1]);
                            DateTime dataFineFirst = Convert.ToDateTime(lista[2]);
                            //decimal aliquotaFirst = Convert.ToDecimal(lista[3]);

                            ibNew1 = new INDENNITASISTEMAZIONE()
                            {
                                IDTIPOTRASFERIMENTO = ibm.idTipoTrasferimento,
                                DATAINIZIOVALIDITA = dataInizioFirst,
                                DATAFINEVALIDITA = dataFineFirst,
                                COEFFICIENTE = ibm.coefficiente,
                                DATAAGGIORNAMENTO = DateTime.Now,
                            };

                            if (aggiornaTutto)
                            {
                                ibNew1 = new INDENNITASISTEMAZIONE()
                                {
                                    IDTIPOTRASFERIMENTO = ibm.idTipoTrasferimento,
                                    DATAINIZIOVALIDITA = dataInizioFirst,
                                    DATAFINEVALIDITA = Utility.DataFineStop(),
                                    COEFFICIENTE = ibm.coefficiente,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                //qui annullo tutti i record rimanenti dalla data inizio inserita
                                libNew = db.INDENNITASISTEMAZIONE.Where(a => a.IDTIPOTRASFERIMENTO == ibm.idTipoTrasferimento
                                && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > dataInizioFirst).ToList();
                                foreach (var elem in libNew)
                                {
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDINDSIST), db);
                                }
                            }
                            db.Database.BeginTransaction();
                            db.INDENNITASISTEMAZIONE.Add(ibNew1);
                            db.SaveChanges();
                            RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloFirst), db);

                            using (RicalcoloPrimaSistemazione rps = new RicalcoloPrimaSistemazione())
                            {
                                rps.RicalcoloPS(ibm.dataInizioValidita, Utility.DataFineStop(), (EnumTipoTrasferimento)ibm.idTipoTrasferimento, db);
                            }

                            db.Database.CurrentTransaction.Commit();
                        }
                        ///se la data variazione coincide con una data fine esistente(diversa da 31/12/9999)
                        if (giafatta == false)
                        {
                            lista = dtal.DataVariazioneCoincideConDataFine(ibm.dataInizioValidita, ibm.idTipoTrasferimento);
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervalloLast = Convert.ToDecimal(lista[0]);
                                DateTime dataInizioLast = Convert.ToDateTime(lista[1]);
                                DateTime dataFineLast = Convert.ToDateTime(lista[2]);
                                decimal COEFFICIENTELast = Convert.ToDecimal(lista[3]);

                                ibNew1 = new INDENNITASISTEMAZIONE()
                                {
                                    IDTIPOTRASFERIMENTO = ibm.idTipoTrasferimento,
                                    DATAINIZIOVALIDITA = dataInizioLast,
                                    DATAFINEVALIDITA = dataFineLast.AddDays(-1),
                                    COEFFICIENTE = COEFFICIENTELast,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new INDENNITASISTEMAZIONE()
                                {
                                    IDTIPOTRASFERIMENTO = ibm.idTipoTrasferimento,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = ibm.dataInizioValidita,//è uguale alla data Inizio
                                    COEFFICIENTE = ibm.coefficiente,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };
                                if (aggiornaTutto)
                                {
                                    ibNew2 = new INDENNITASISTEMAZIONE()
                                    {
                                        IDTIPOTRASFERIMENTO = ibm.idTipoTrasferimento,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        COEFFICIENTE = ibm.coefficiente,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.INDENNITASISTEMAZIONE.Where(a => a.IDTIPOTRASFERIMENTO == ibm.idTipoTrasferimento
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDINDSIST), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                db.Database.BeginTransaction();
                                db.INDENNITASISTEMAZIONE.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloLast), db);


                                using (RicalcoloPrimaSistemazione rps = new RicalcoloPrimaSistemazione())
                                {
                                    rps.RicalcoloPS(ibm.dataInizioValidita, Utility.DataFineStop(), (EnumTipoTrasferimento)ibm.idTipoTrasferimento, db);
                                }

                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //Se il nuovo record si trova in un intervallo non annullato con data fine non uguale al 31/12/9999
                        if (giafatta == false)
                        {
                            lista = dtal.RestituisciIntervalloDiUnaData(ibm.dataInizioValidita, ibm.idTipoTrasferimento);
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervallo = Convert.ToDecimal(lista[0]);
                                DateTime dataInizio = Convert.ToDateTime(lista[1]);
                                DateTime dataFine = Convert.ToDateTime(lista[2]);
                                decimal COEFFICIENTE = Convert.ToDecimal(lista[3]);

                                DateTime NewdataFine1 = ibm.dataInizioValidita.AddDays(-1);

                                ibNew1 = new INDENNITASISTEMAZIONE()
                                {
                                    IDTIPOTRASFERIMENTO = ibm.idTipoTrasferimento,
                                    DATAINIZIOVALIDITA = dataInizio,
                                    DATAFINEVALIDITA = NewdataFine1,
                                    COEFFICIENTE = COEFFICIENTE,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new INDENNITASISTEMAZIONE()
                                {
                                    IDTIPOTRASFERIMENTO = ibm.idTipoTrasferimento,
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = dataFine,
                                    COEFFICIENTE = ibm.coefficiente,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };

                                if (aggiornaTutto)
                                {
                                    ibNew2 = new INDENNITASISTEMAZIONE()
                                    {
                                        IDTIPOTRASFERIMENTO = ibm.idTipoTrasferimento,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        COEFFICIENTE = ibm.coefficiente,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.INDENNITASISTEMAZIONE.Where(a => a.IDTIPOTRASFERIMENTO == ibm.idTipoTrasferimento
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDINDSIST), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                db.Database.BeginTransaction();
                                db.INDENNITASISTEMAZIONE.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervallo), db);


                                using (RicalcoloPrimaSistemazione rps = new RicalcoloPrimaSistemazione())
                                {
                                    rps.RicalcoloPS(ibm.dataInizioValidita, Utility.DataFineStop(), (EnumTipoTrasferimento)ibm.idTipoTrasferimento, db);
                                }

                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //CASO DELL'ULTIMA RIGA CON LA DATA FINE UGUALE A 31/12/9999
                        if (giafatta == false)
                        {
                            //Attenzione qui se la lista non contiene nessun elemento
                            //significa che non esiste nessun elemento corrispondentemente al livello selezionato
                            lista = dtal.RestituisciLaRigaMassima(ibm.idTipoTrasferimento);
                            if (lista.Count == 0)
                            {
                                ibNew1 = new INDENNITASISTEMAZIONE()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = Convert.ToDateTime(Utility.DataFineStop()),
                                    COEFFICIENTE = ibm.coefficiente,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    IDTIPOTRASFERIMENTO = ibm.idTipoTrasferimento,
                                };
                                libNew.Add(ibNew1);
                                db.Database.BeginTransaction();
                                db.INDENNITASISTEMAZIONE.Add(ibNew1);
                                db.SaveChanges();


                                using (RicalcoloPrimaSistemazione rps = new RicalcoloPrimaSistemazione())
                                {
                                    rps.RicalcoloPS(ibm.dataInizioValidita, Utility.DataFineStop(), (EnumTipoTrasferimento)ibm.idTipoTrasferimento, db);
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
                                decimal COEFFICIENTEUltimo = Convert.ToDecimal(lista[3]);
                                if (dataInizioUltimo == ibm.dataInizioValidita)
                                {
                                    ibNew1 = new INDENNITASISTEMAZIONE()
                                    {
                                        IDTIPOTRASFERIMENTO = ibm.idTipoTrasferimento,
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = dataFineUltimo,
                                        COEFFICIENTE = ibm.coefficiente,//nuova aliquota rispetto alla vecchia registrata
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.INDENNITASISTEMAZIONE.Add(ibNew1);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (RicalcoloPrimaSistemazione rps = new RicalcoloPrimaSistemazione())
                                    {
                                        rps.RicalcoloPS(ibm.dataInizioValidita, Utility.DataFineStop(), (EnumTipoTrasferimento)ibm.idTipoTrasferimento, db);
                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                                //se il nuovo record rappresenta la data variazione superiore alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                if (ibm.dataInizioValidita > dataInizioUltimo)
                                {
                                    ibNew1 = new INDENNITASISTEMAZIONE()
                                    {
                                        IDTIPOTRASFERIMENTO = ibm.idTipoTrasferimento,
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = ibm.dataInizioValidita.AddDays(-1),
                                        COEFFICIENTE = COEFFICIENTEUltimo,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    ibNew2 = new INDENNITASISTEMAZIONE()
                                    {
                                        IDTIPOTRASFERIMENTO = ibm.idTipoTrasferimento,
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        COEFFICIENTE = ibm.coefficiente,//nuova aliquota rispetto alla vecchia registrata
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1); libNew.Add(ibNew2);
                                    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                    db.Database.BeginTransaction();
                                    db.INDENNITASISTEMAZIONE.AddRange(libNew);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (RicalcoloPrimaSistemazione rps = new RicalcoloPrimaSistemazione())
                                    {
                                        rps.RicalcoloPS(ibm.dataInizioValidita, Utility.DataFineStop(), (EnumTipoTrasferimento)ibm.idTipoTrasferimento, db);
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

        public bool EsistonoMovimentiPrima(IndennitaSistemazioneModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.INDENNITASISTEMAZIONE.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDTIPOTRASFERIMENTO == ibm.idTipoTrasferimento).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(IndennitaSistemazioneModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.INDENNITASISTEMAZIONE.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDTIPOTRASFERIMENTO == ibm.idTipoTrasferimento).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(IndennitaSistemazioneModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.INDENNITASISTEMAZIONE.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDTIPOTRASFERIMENTO == ibm.idTipoTrasferimento).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiPrimaUguale(IndennitaSistemazioneModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.INDENNITASISTEMAZIONE.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDTIPOTRASFERIMENTO == ibm.idTipoTrasferimento).Count() > 0 ? true : false;
            }
        }

        public void DelIndennitaSistemazione(decimal idIndSist)
        {
            INDENNITASISTEMAZIONE precedenteIB = new INDENNITASISTEMAZIONE();
            INDENNITASISTEMAZIONE delIB = new INDENNITASISTEMAZIONE();


            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.INDENNITASISTEMAZIONE.Where(a => a.IDINDSIST == idIndSist);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.INDENNITASISTEMAZIONE.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new INDENNITASISTEMAZIONE()
                            {

                                IDTIPOTRASFERIMENTO = precedenteIB.IDTIPOTRASFERIMENTO,
                                DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                COEFFICIENTE = precedenteIB.COEFFICIENTE,
                                DATAAGGIORNAMENTO = precedenteIB.DATAAGGIORNAMENTO,
                                ANNULLATO = false
                            };

                            db.INDENNITASISTEMAZIONE.Add(ibOld1);
                        }

                        db.SaveChanges();

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di indennità di sistemazione.", "INDENNITASISTEMAZIONE", idIndSist);
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
        public decimal Get_Id_IndSistemNonAnnullato(decimal idTipoTrasferimento)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<INDENNITASISTEMAZIONE> libm = new List<INDENNITASISTEMAZIONE>();
                libm = db.INDENNITASISTEMAZIONE.Where(a => a.ANNULLATO == false
                && a.IDTIPOTRASFERIMENTO == idTipoTrasferimento).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDINDSIST;
            }
            return tmp;
        }

        public void DelINDENNITASISTEMAZIONE(decimal idIndSist)
        {
            INDENNITASISTEMAZIONE precedenteIB = new INDENNITASISTEMAZIONE();
            INDENNITASISTEMAZIONE delIB = new INDENNITASISTEMAZIONE();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    var lib = db.INDENNITASISTEMAZIONE.Where(a => a.IDINDSIST == idIndSist);
                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;
                        RendiAnnullatoUnRecord(delIB.IDINDSIST, db);
                        precedenteIB = RestituisciIlRecordPrecedente(idIndSist);
                        RendiAnnullatoUnRecord(precedenteIB.IDINDSIST, db);

                        var NuovoPrecedente = new INDENNITASISTEMAZIONE()
                        {
                            IDTIPOTRASFERIMENTO = precedenteIB.IDTIPOTRASFERIMENTO,
                            DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                            COEFFICIENTE = precedenteIB.COEFFICIENTE,
                            DATAAGGIORNAMENTO = DateTime.Now,// precedenteIB.DATAAGGIORNAMENTO,
                            ANNULLATO = false
                        };
                        db.INDENNITASISTEMAZIONE.Add(NuovoPrecedente);
                    }
                    db.SaveChanges();
                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di Indennita Sistemazione", "INDENNITASISTEMAZIONE", idIndSist);
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
        public bool INDENNITASISTEMAZIONEAnnullato(IndennitaSistemazioneModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.INDENNITASISTEMAZIONE.Where(a => a.IDINDSIST == ibm.idIndSist && a.IDTIPOTRASFERIMENTO == ibm.idTipoTrasferimento).First().ANNULLATO == true ? true : false;
            }
        }
        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as IndennitaSistemazioneModel;

            if (fm != null)
            {
                DateTime d = DataInizioMinimaNonAnnullataIndennitaBase(fm.idTipoTrasferimento);
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
        public decimal Get_Id_INDENNITASISTEMAZIONEPrimoNonAnnullato(decimal idTipoTrasferimento)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<INDENNITASISTEMAZIONE> libm = new List<INDENNITASISTEMAZIONE>();
                libm = db.INDENNITASISTEMAZIONE.Where(a => a.ANNULLATO == false
                && a.IDTIPOTRASFERIMENTO == idTipoTrasferimento).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDINDSIST;
            }
            return tmp;
        }
        public static DateTime DataInizioMinimaNonAnnullataIndennitaBase(decimal idLivello)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.INDENNITASISTEMAZIONE.Where(a => a.ANNULLATO == false && a.IDTIPOTRASFERIMENTO == idLivello).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }
        public INDENNITASISTEMAZIONE RestituisciIlRecordPrecedente(decimal idIndSist)
        {
            INDENNITASISTEMAZIONE tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                INDENNITASISTEMAZIONE interessato = new INDENNITASISTEMAZIONE();
                interessato = db.INDENNITASISTEMAZIONE.Find(idIndSist);
                tmp = db.INDENNITASISTEMAZIONE.Where(a => a.IDTIPOTRASFERIMENTO == interessato.IDTIPOTRASFERIMENTO
                && a.ANNULLATO == false).ToList().Where(b => b.DATAFINEVALIDITA == interessato.DATAINIZIOVALIDITA.AddDays(-1)).ToList().First();
            }
            return tmp;
        }
        public List<string> RestituisciIntervalloDiUnaData(DateTime DataCampione, decimal idTipoTrasferimento)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<INDENNITASISTEMAZIONE> libm = new List<INDENNITASISTEMAZIONE>();
                libm = db.INDENNITASISTEMAZIONE.Where(a => a.ANNULLATO == false
                && a.IDTIPOTRASFERIMENTO == idTipoTrasferimento).ToList().Where(b =>
                b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())
                && DataCampione > b.DATAINIZIOVALIDITA
                && DataCampione < b.DATAFINEVALIDITA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDINDSIST.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTE.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione, decimal idTipoTrasferimento)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<INDENNITASISTEMAZIONE> libm = new List<INDENNITASISTEMAZIONE>();
                libm = db.INDENNITASISTEMAZIONE.Where(a => a.ANNULLATO == false
                && a.IDTIPOTRASFERIMENTO == idTipoTrasferimento).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().Where(b => DataCampione == b.DATAINIZIOVALIDITA &&
                 b.DATAFINEVALIDITA != Utility.DataFineStop()).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDINDSIST.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTE.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione, decimal idTipoTrasferimento)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<INDENNITASISTEMAZIONE> libm = new List<INDENNITASISTEMAZIONE>();
                libm = db.INDENNITASISTEMAZIONE.Where(a => a.ANNULLATO == false
                && a.IDTIPOTRASFERIMENTO == idTipoTrasferimento).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().
                Where(b => DataCampione == b.DATAFINEVALIDITA
                && b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())).ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDINDSIST.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTE.ToString());
                }
            }
            return tmp;
        }
        public List<string> RestituisciLaRigaMassima(decimal idTipoTrasferimento)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<INDENNITASISTEMAZIONE> libm = new List<INDENNITASISTEMAZIONE>();
                libm = db.INDENNITASISTEMAZIONE.Where(a => a.ANNULLATO == false
                && a.IDTIPOTRASFERIMENTO == idTipoTrasferimento).ToList().Where(b =>
                b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop())).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDINDSIST.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTE.ToString());
                }
            }
            return tmp;
        }
        public void RendiAnnullatoUnRecord(decimal idIndSist, ModelDBISE db)
        {
            INDENNITASISTEMAZIONE entita = new INDENNITASISTEMAZIONE();
            entita = db.INDENNITASISTEMAZIONE.Find(idIndSist);
            entita.ANNULLATO = true;
            db.SaveChanges();
        }
    }
}
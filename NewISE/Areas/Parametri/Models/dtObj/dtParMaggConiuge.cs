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
using NewISE.Models.Enumeratori;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParMaggConiuge : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public IList<PercentualeMagConiugeModel> getListPercMagConiuge()
        {
            List<PercentualeMagConiugeModel> libm = new List<PercentualeMagConiugeModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEMAGCONIUGE.ToList();

                    libm = (from e in lib
                            select new PercentualeMagConiugeModel()
                            {

                                idPercentualeConiuge = e.IDPERCMAGCONIUGE,
                                idTipologiaConiuge = (EnumTipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new PercentualeMagConiugeModel().dataFineValidita,
                                percentualeConiuge = e.PERCENTUALECONIUGE,
                                annullato = e.ANNULLATO,
                                Coniuge = new TipologiaConiugeModel()
                                {
                                    idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                                    tipologiaConiuge = e.TIPOLOGIACONIUGE.ToString()

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

        public IList<PercentualeMagConiugeModel> getListPercMagConiuge(decimal idTipologiaConiuge)
        {
            List<PercentualeMagConiugeModel> libm = new List<PercentualeMagConiugeModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEMAGCONIUGE.Where(a => a.IDTIPOLOGIACONIUGE == idTipologiaConiuge).ToList();

                    libm = (from e in lib
                            select new PercentualeMagConiugeModel()
                            {

                                idPercentualeConiuge = e.IDPERCMAGCONIUGE,
                                idTipologiaConiuge = (EnumTipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,// != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new PercentualeMagConiugeModel().dataFineValidita,
                                percentualeConiuge = e.PERCENTUALECONIUGE,
                                annullato = e.ANNULLATO,
                                Coniuge = new TipologiaConiugeModel()
                                {
                                    idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                                    tipologiaConiuge = e.TIPOLOGIACONIUGE.ToString()

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

        public IList<PercentualeMagConiugeModel> getListPercMagConiuge(bool escludiAnnullati = false)
        {
            List<PercentualeMagConiugeModel> libm = new List<PercentualeMagConiugeModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new PercentualeMagConiugeModel()
                            {

                                idPercentualeConiuge = e.IDPERCMAGCONIUGE,
                                idTipologiaConiuge = (EnumTipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,//!= Utility.DataFineStop() ? e.DATAFINEVALIDITA : new PercentualeMagConiugeModel().dataFineValidita,
                                percentualeConiuge = e.PERCENTUALECONIUGE,
                                annullato = e.ANNULLATO,
                                Coniuge = new TipologiaConiugeModel()
                                {
                                    idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                                    tipologiaConiuge = e.TIPOLOGIACONIUGE.ToString()

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

        public IList<PercentualeMagConiugeModel> getListPercMagConiuge(decimal idTipologiaConiuge, bool escludiAnnullati = false)
        {
            List<PercentualeMagConiugeModel> libm = new List<PercentualeMagConiugeModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    //var lib = db.PERCENTUALEMAGCONIUGE.Where(a => a.IDTIPOLOGIACONIUGE == idTipologiaConiuge && a.ANNULLATO == escludiAnnullati).ToList();
                    List<PERCENTUALEMAGCONIUGE> lib = new List<PERCENTUALEMAGCONIUGE>();
                    if (escludiAnnullati == true)
                        lib = db.PERCENTUALEMAGCONIUGE.Where(a => a.IDTIPOLOGIACONIUGE == idTipologiaConiuge && a.ANNULLATO == false).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                    else
                        lib = db.PERCENTUALEMAGCONIUGE.Where(a => a.IDTIPOLOGIACONIUGE == idTipologiaConiuge).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();

                    libm = (from e in lib
                            select new PercentualeMagConiugeModel()
                            {

                                idPercentualeConiuge = e.IDPERCMAGCONIUGE,
                                idTipologiaConiuge = (EnumTipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,// != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new PercentualeMagConiugeModel().dataFineValidita,
                                percentualeConiuge = e.PERCENTUALECONIUGE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                Coniuge = new TipologiaConiugeModel()
                                {
                                    idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                                    tipologiaConiuge = e.TIPOLOGIACONIUGE.ToString()

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
        //public void SetPercMagConiuge(PercentualeMagConiugeModel ibm)
        //{
        //    List<PERCENTUALEMAGCONIUGE> libNew = new List<PERCENTUALEMAGCONIUGE>();

        //    PERCENTUALEMAGCONIUGE ibNew = new PERCENTUALEMAGCONIUGE();

        //    PERCENTUALEMAGCONIUGE ibPrecedente = new PERCENTUALEMAGCONIUGE();

        //    List<PERCENTUALEMAGCONIUGE> lArchivioIB = new List<PERCENTUALEMAGCONIUGE>();

        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        try
        //        {
        //            if (ibm.dataFineValidita.HasValue)
        //            {
        //                if (EsistonoMovimentiSuccessiviUguale(ibm))
        //                {
        //                    ibNew = new PERCENTUALEMAGCONIUGE()
        //                    {

        //                        IDTIPOLOGIACONIUGE = (decimal)ibm.idTipologiaConiuge,
        //                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
        //                        DATAFINEVALIDITA = ibm.dataFineValidita.Value,
        //                        PERCENTUALECONIUGE = ibm.percentualeConiuge,
        //                        ANNULLATO = ibm.annullato
        //                    };
        //                }
        //                else
        //                {
        //                    ibNew = new PERCENTUALEMAGCONIUGE()
        //                    {
        //                        IDTIPOLOGIACONIUGE = (decimal)ibm.idTipologiaConiuge,
        //                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
        //                        DATAFINEVALIDITA = Utility.DataFineStop(),
        //                        PERCENTUALECONIUGE = ibm.percentualeConiuge,
        //                        ANNULLATO = ibm.annullato
        //                    };
        //                }
        //            }
        //            else
        //            {
        //                ibNew = new PERCENTUALEMAGCONIUGE()
        //                {

        //                    IDTIPOLOGIACONIUGE = (decimal)ibm.idTipologiaConiuge,
        //                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
        //                    DATAFINEVALIDITA = Utility.DataFineStop(),
        //                    PERCENTUALECONIUGE = ibm.percentualeConiuge,
        //                    ANNULLATO = ibm.annullato
        //                };
        //            }

        //            db.Database.BeginTransaction();

        //            var recordInteressati = db.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false && a.IDTIPOLOGIACONIUGE == ibNew.IDTIPOLOGIACONIUGE)
        //                                                    .Where(a => a.DATAINIZIOVALIDITA >= ibNew.DATAINIZIOVALIDITA || a.DATAFINEVALIDITA >= ibNew.DATAINIZIOVALIDITA)
        //                                                    .Where(a => a.DATAINIZIOVALIDITA <= ibNew.DATAFINEVALIDITA || a.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
        //                                                    .ToList();

        //            recordInteressati.ForEach(a => a.ANNULLATO = true);
        //            //db.SaveChanges();

        //            if (recordInteressati.Count > 0)
        //            {
        //                foreach (var item in recordInteressati)
        //                {

        //                    if (item.DATAINIZIOVALIDITA < ibNew.DATAINIZIOVALIDITA)
        //                    {
        //                        if (item.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
        //                        {
        //                            var ibOld1 = new PERCENTUALEMAGCONIUGE()
        //                            {
        //                                IDTIPOLOGIACONIUGE = (decimal)ibm.idTipologiaConiuge,
        //                                DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
        //                                DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
        //                                PERCENTUALECONIUGE = item.PERCENTUALECONIUGE,
        //                                ANNULLATO = false
        //                            };

        //                            libNew.Add(ibOld1);

        //                        }
        //                        else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
        //                        {
        //                            var ibOld1 = new PERCENTUALEMAGCONIUGE()
        //                            {

        //                                IDTIPOLOGIACONIUGE = item.IDTIPOLOGIACONIUGE,
        //                                DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
        //                                DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
        //                                PERCENTUALECONIUGE = item.PERCENTUALECONIUGE,
        //                                ANNULLATO = false
        //                            };

        //                            var ibOld2 = new PERCENTUALEMAGCONIUGE()
        //                            {

        //                                IDTIPOLOGIACONIUGE = item.IDTIPOLOGIACONIUGE,
        //                                DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(+1),
        //                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,
        //                                PERCENTUALECONIUGE = item.PERCENTUALECONIUGE,
        //                                ANNULLATO = false
        //                            };

        //                            libNew.Add(ibOld1);
        //                            libNew.Add(ibOld2);

        //                        }

        //                    }
        //                    else if (item.DATAINIZIOVALIDITA == ibNew.DATAINIZIOVALIDITA)
        //                    {
        //                        if (item.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
        //                        {
        //                            //Non preleva il record old
        //                        }
        //                        else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
        //                        {
        //                            var ibOld1 = new PERCENTUALEMAGCONIUGE()
        //                            {

        //                                IDTIPOLOGIACONIUGE = item.IDTIPOLOGIACONIUGE,
        //                                DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
        //                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,
        //                                PERCENTUALECONIUGE = item.PERCENTUALECONIUGE,
        //                                ANNULLATO = false
        //                            };

        //                            libNew.Add(ibOld1);
        //                        }
        //                    }
        //                    else if (item.DATAINIZIOVALIDITA > ibNew.DATAINIZIOVALIDITA)
        //                    {
        //                        if (item.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
        //                        {
        //                            //Non preleva il record old
        //                        }
        //                        else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
        //                        {
        //                            var ibOld1 = new PERCENTUALEMAGCONIUGE()
        //                            {

        //                                IDTIPOLOGIACONIUGE = item.IDTIPOLOGIACONIUGE,
        //                                DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
        //                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,
        //                                PERCENTUALECONIUGE = item.PERCENTUALECONIUGE,
        //                                ANNULLATO = false
        //                            };

        //                            libNew.Add(ibOld1);
        //                        }
        //                    }
        //                }

        //                libNew.Add(ibNew);
        //                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

        //                db.PERCENTUALEMAGCONIUGE.AddRange(libNew);
        //            }
        //            else
        //            {
        //                db.PERCENTUALEMAGCONIUGE.Add(ibNew);

        //            }
        //            db.SaveChanges();

        //            using (objLogAttivita log = new objLogAttivita())
        //            {
        //                log.Log(enumAttivita.Inserimento, "Inserimento parametro di maggiorazione coniuge.", "MAGGIORAZIONECONIUGE", ibNew.IDPERCMAGCONIUGE);
        //            }

        //            db.Database.CurrentTransaction.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            db.Database.CurrentTransaction.Rollback();
        //            throw ex;
        //        }
        //    }
        //}


        public void SetPercMagConiuge(PercentualeMagConiugeModel ibm, bool aggiornaTutto)
        {
            List<PERCENTUALEMAGCONIUGE> libNew = new List<PERCENTUALEMAGCONIUGE>();

            //PERCENTUALEMAGCONIUGE ibPrecedente = new PERCENTUALEMAGCONIUGE();
            PERCENTUALEMAGCONIUGE ibNew1 = new PERCENTUALEMAGCONIUGE();
            PERCENTUALEMAGCONIUGE ibNew2 = new PERCENTUALEMAGCONIUGE();
            //List<PERCENTUALEMAGCONIUGE> lArchivioIB = new List<PERCENTUALEMAGCONIUGE>();
            List<string> lista = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                bool giafatta = false;
                try
                {
                    using (dtParMaggConiuge dtal = new dtParMaggConiuge())
                    {
                        //Se la data variazione coincide con una data inizio esistente
                        lista = dtal.DataVariazioneCoincideConDataInizio(ibm.dataInizioValidita, Convert.ToDecimal(ibm.idTipologiaConiuge));

                        if (lista.Count != 0)
                        {
                            giafatta = true;
                            decimal idIntervalloFirst = Convert.ToDecimal(lista[0]);
                            DateTime dataInizioFirst = Convert.ToDateTime(lista[1]);
                            DateTime dataFineFirst = Convert.ToDateTime(lista[2]);
                            //decimal percConiugeFirst = Convert.ToDecimal(lista[3]);

                            ibNew1 = new PERCENTUALEMAGCONIUGE()
                            {
                                IDTIPOLOGIACONIUGE = Convert.ToDecimal(ibm.idTipologiaConiuge),
                                DATAINIZIOVALIDITA = dataInizioFirst,
                                DATAFINEVALIDITA = dataFineFirst,
                                // ALIQUOTA = ibm.aliquota,
                                PERCENTUALECONIUGE = ibm.percentualeConiuge,
                                DATAAGGIORNAMENTO = DateTime.Now,
                            };

                            if (aggiornaTutto)
                            {
                                ibNew1 = new PERCENTUALEMAGCONIUGE()
                                {
                                    IDTIPOLOGIACONIUGE = Convert.ToDecimal(ibm.idTipologiaConiuge),
                                    DATAINIZIOVALIDITA = dataInizioFirst,
                                    DATAFINEVALIDITA = Utility.DataFineStop(),
                                    // ALIQUOTA = ibm.aliquota,
                                    PERCENTUALECONIUGE = ibm.percentualeConiuge,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                //qui annullo tutti i record rimanenti dalla data inizio inserita
                                libNew = db.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false).ToList()
                                    .Where(a => a.DATAINIZIOVALIDITA > dataInizioFirst &&
                                    a.IDTIPOLOGIACONIUGE == Convert.ToDecimal(ibm.idTipologiaConiuge)).ToList();
                                foreach (var elem in libNew)
                                {
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPERCMAGCONIUGE), db);
                                }
                            }
                            db.Database.BeginTransaction();
                            db.PERCENTUALEMAGCONIUGE.Add(ibNew1);
                            db.SaveChanges();
                            RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloFirst), db);

                            using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                            {
                                dtrp.AssociaConiuge_PMC(ibNew1.IDPERCMAGCONIUGE, db, ibm.dataInizioValidita);
                            }

                            db.Database.CurrentTransaction.Commit();
                        }
                        ///se la data variazione coincide con una data fine esistente(diversa da 31/12/9999)
                        if (giafatta == false)
                        {
                            lista = dtal.DataVariazioneCoincideConDataFine(ibm.dataInizioValidita, Convert.ToDecimal(ibm.idTipologiaConiuge));

                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervalloLast = Convert.ToDecimal(lista[0]);
                                DateTime dataInizioLast = Convert.ToDateTime(lista[1]);
                                DateTime dataFineLast = Convert.ToDateTime(lista[2]);
                                decimal PERCENTUALECONIUGE = Convert.ToDecimal(lista[3]);

                                ibNew1 = new PERCENTUALEMAGCONIUGE()
                                {
                                    IDTIPOLOGIACONIUGE = Convert.ToDecimal(ibm.idTipologiaConiuge),
                                    DATAINIZIOVALIDITA = dataInizioLast,
                                    DATAFINEVALIDITA = dataFineLast.AddDays(-1),
                                    PERCENTUALECONIUGE = PERCENTUALECONIUGE,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new PERCENTUALEMAGCONIUGE()
                                {
                                    IDTIPOLOGIACONIUGE = Convert.ToDecimal(ibm.idTipologiaConiuge),
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = ibm.dataInizioValidita,//è uguale alla data Inizio
                                    PERCENTUALECONIUGE = ibm.percentualeConiuge,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };
                                if (aggiornaTutto)
                                {
                                    ibNew2 = new PERCENTUALEMAGCONIUGE()
                                    {
                                        IDTIPOLOGIACONIUGE = Convert.ToDecimal(ibm.idTipologiaConiuge),
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        PERCENTUALECONIUGE = ibm.percentualeConiuge,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.PERCENTUALEMAGCONIUGE.Where(a => a.IDTIPOLOGIACONIUGE == Convert.ToDecimal(ibm.idTipologiaConiuge)
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPERCMAGCONIUGE), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                db.Database.BeginTransaction();
                                db.PERCENTUALEMAGCONIUGE.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloLast), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var pmag in libNew)
                                    {
                                        dtrp.AssociaConiuge_PMC(pmag.IDPERCMAGCONIUGE, db, ibm.dataInizioValidita);
                                    }
                                }

                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //Se il nuovo record si trova in un intervallo non annullato con data fine non uguale al 31/12/9999
                        if (giafatta == false)
                        {
                            lista = dtal.RestituisciIntervalloDiUnaData(ibm.dataInizioValidita, Convert.ToDecimal(ibm.idTipologiaConiuge));
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervallo = Convert.ToDecimal(lista[0]);
                                DateTime dataInizio = Convert.ToDateTime(lista[1]);
                                DateTime dataFine = Convert.ToDateTime(lista[2]);
                                decimal PERCENTUALECONIUGE = Convert.ToDecimal(lista[3]);

                                DateTime NewdataFine1 = ibm.dataInizioValidita.AddDays(-1);

                                ibNew1 = new PERCENTUALEMAGCONIUGE()
                                {
                                    IDTIPOLOGIACONIUGE = Convert.ToDecimal(ibm.idTipologiaConiuge),
                                    DATAINIZIOVALIDITA = dataInizio,
                                    DATAFINEVALIDITA = NewdataFine1,
                                    //ALIQUOTA = aliquota,
                                    PERCENTUALECONIUGE = PERCENTUALECONIUGE,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new PERCENTUALEMAGCONIUGE()
                                {
                                    IDTIPOLOGIACONIUGE = Convert.ToDecimal(ibm.idTipologiaConiuge),
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = dataFine,
                                    // ALIQUOTA = ibm.aliquota,
                                    PERCENTUALECONIUGE = ibm.percentualeConiuge,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };

                                if (aggiornaTutto)
                                {
                                    ibNew2 = new PERCENTUALEMAGCONIUGE()
                                    {
                                        IDTIPOLOGIACONIUGE = Convert.ToDecimal(ibm.idTipologiaConiuge),
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        // ALIQUOTA = ibm.aliquota,
                                        PERCENTUALECONIUGE = ibm.percentualeConiuge,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    decimal tmpii = Convert.ToDecimal(ibm.idTipologiaConiuge);
                                    libNew = db.PERCENTUALEMAGCONIUGE.Where(a => a.IDTIPOLOGIACONIUGE == tmpii
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPERCMAGCONIUGE), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                db.Database.BeginTransaction();
                                db.PERCENTUALEMAGCONIUGE.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervallo), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var pmag in libNew)
                                    {
                                        dtrp.AssociaConiuge_PMC(pmag.IDPERCMAGCONIUGE, db, ibm.dataInizioValidita);
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
                            lista = dtal.RestituisciLaRigaMassima(Convert.ToDecimal(ibm.idTipologiaConiuge));
                            if (lista.Count == 0)
                            {
                                ibNew1 = new PERCENTUALEMAGCONIUGE()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = Convert.ToDateTime(Utility.DataFineStop()),
                                    PERCENTUALECONIUGE = ibm.percentualeConiuge,
                                    IDTIPOLOGIACONIUGE = Convert.ToDecimal(ibm.idTipologiaConiuge),
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                libNew.Add(ibNew1);
                                db.Database.BeginTransaction();
                                db.PERCENTUALEMAGCONIUGE.Add(ibNew1);
                                db.SaveChanges();

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {

                                    dtrp.AssociaConiuge_PMC(ibNew1.IDPERCMAGCONIUGE, db, ibm.dataInizioValidita);

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
                                decimal percentualeUltimo = Convert.ToDecimal(lista[3]);

                                if (dataInizioUltimo == ibm.dataInizioValidita)
                                {
                                    ibNew1 = new PERCENTUALEMAGCONIUGE()
                                    {
                                        IDTIPOLOGIACONIUGE = Convert.ToDecimal(ibm.idTipologiaConiuge),
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = dataFineUltimo,
                                        // ALIQUOTA = ibm.aliquota,//nuova aliquota rispetto alla vecchia registrata
                                        PERCENTUALECONIUGE = ibm.percentualeConiuge,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.PERCENTUALEMAGCONIUGE.Add(ibNew1);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {

                                        dtrp.AssociaConiuge_PMC(ibNew1.IDPERCMAGCONIUGE, db, ibm.dataInizioValidita);

                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                                //se il nuovo record rappresenta la data variazione superiore alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                if (ibm.dataInizioValidita > dataInizioUltimo)
                                {
                                    ibNew1 = new PERCENTUALEMAGCONIUGE()
                                    {
                                        IDTIPOLOGIACONIUGE = Convert.ToDecimal(ibm.idTipologiaConiuge),
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = ibm.dataInizioValidita.AddDays(-1),
                                        PERCENTUALECONIUGE = percentualeUltimo,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    ibNew2 = new PERCENTUALEMAGCONIUGE()
                                    {
                                        IDTIPOLOGIACONIUGE = Convert.ToDecimal(ibm.idTipologiaConiuge),
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        PERCENTUALECONIUGE = ibm.percentualeConiuge,//nuova aliquota rispetto alla vecchia registrata
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1); libNew.Add(ibNew2);
                                    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                    db.Database.BeginTransaction();
                                    db.PERCENTUALEMAGCONIUGE.AddRange(libNew);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        foreach (var pmag in libNew)
                                        {
                                            dtrp.AssociaConiuge_PMC(pmag.IDPERCMAGCONIUGE, db, ibm.dataInizioValidita);
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


        public bool EsistonoMovimentiPrima(PercentualeMagConiugeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEMAGCONIUGE.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDTIPOLOGIACONIUGE == (decimal)ibm.idTipologiaConiuge).Count() > 0 ? true : false;
            }
        }
        public bool EsistonoMovimentiSuccessivi(PercentualeMagConiugeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.PERCENTUALEMAGCONIUGE.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDTIPOLOGIACONIUGE == (decimal)ibm.idTipologiaConiuge).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool EsistonoMovimentiSuccessiviUguale(PercentualeMagConiugeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.PERCENTUALEMAGCONIUGE.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDTIPOLOGIACONIUGE == (decimal)ibm.idTipologiaConiuge).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool EsistonoMovimentiPrimaUguale(PercentualeMagConiugeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEMAGCONIUGE.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDTIPOLOGIACONIUGE == (decimal)ibm.idTipologiaConiuge).Count() > 0 ? true : false;
            }
        }


        public void DelPercMagConiuge(decimal idMagCon)
        {
            PERCENTUALEMAGCONIUGE precedenteIB = new PERCENTUALEMAGCONIUGE();
            PERCENTUALEMAGCONIUGE delIB = new PERCENTUALEMAGCONIUGE();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    var lib = db.PERCENTUALEMAGCONIUGE.Where(a => a.IDPERCMAGCONIUGE == idMagCon);
                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;
                        RendiAnnullatoUnRecord(delIB.IDPERCMAGCONIUGE, db);
                        precedenteIB = RestituisciIlRecordPrecedente(idMagCon);
                        RendiAnnullatoUnRecord(precedenteIB.IDPERCMAGCONIUGE, db);

                        var NuovoPrecedente = new PERCENTUALEMAGCONIUGE()
                        {
                            IDTIPOLOGIACONIUGE = precedenteIB.IDTIPOLOGIACONIUGE,
                            DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                            //ALIQUOTA = precedenteIB.ALIQUOTA,
                            PERCENTUALECONIUGE = precedenteIB.PERCENTUALECONIUGE,
                            DATAAGGIORNAMENTO = DateTime.Now,// precedenteIB.DATAAGGIORNAMENTO,
                            ANNULLATO = false
                        };
                        db.PERCENTUALEMAGCONIUGE.Add(NuovoPrecedente);

                        db.SaveChanges();

                        using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                        {
                            dtrp.AssociaConiuge_PMC(NuovoPrecedente.IDPERCMAGCONIUGE, db, delIB.DATAINIZIOVALIDITA);
                        }

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di Percentuale Maggiorazione Coniugi.", "PERCENTUALECONIUGE", idMagCon);
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



        public static DateTime DataInizioMinimaNonAnnullata(decimal idLivello)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false && a.IDTIPOLOGIACONIUGE == idLivello).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }
        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as PercentualeMagConiugeModel;
            if (fm != null)
            {
                DateTime d = DataInizioMinimaNonAnnullata(Convert.ToDecimal(fm.idTipologiaConiuge));
                if (fm.dataInizioValidita < d)
                {
                    vr = new ValidationResult(string.Format("Impossibile inserire la data di inizio validità minore alla data di Base ({0}).", d.ToShortDateString()));
                }
                else
                {
                    if (fm.dataFineValidita < fm.dataInizioValidita)
                    {
                        vr = new ValidationResult(string.Format("Impossibile inserire la data di inizio validità maggiore alla data base ({0}).", fm.dataFineValidita.Value.ToShortDateString()));
                    }
                    else
                    {
                        vr = ValidationResult.Success;
                    }
                }
            }
            else
            {
                vr = new ValidationResult("La data di inizio validità è richiesta.");
            }
            return vr;
        }
        public static ValidationResult VerificaPercentualeConiuge(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as PercentualeMagConiugeModel;

            if (fm != null)
            {
                if (fm.percentualeConiuge > 100)
                {
                    vr = new ValidationResult(string.Format("Impossibile inserire percentuale maggiore di 100 ({0}).", fm.percentualeConiuge.ToString()));
                }
                else
                {
                    vr = ValidationResult.Success;
                }
            }
            else
            {
                vr = new ValidationResult("La percentuale responsabile è richiesta.");
            }
            return vr;
        }

        public bool PercMaggiorazioneConiugeAnnullato(PercentualeMagConiugeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEMAGCONIUGE.Where(a => a.IDPERCMAGCONIUGE == ibm.idPercentualeConiuge).First().ANNULLATO == true ? true : false;
            }
        }
        public decimal Get_Id_PercentualMagConiugePrimoNonAnnullato(decimal idTipologiaConiuge)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEMAGCONIUGE> libm = new List<PERCENTUALEMAGCONIUGE>();
                libm = db.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false
                && a.IDTIPOLOGIACONIUGE == idTipologiaConiuge).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDPERCMAGCONIUGE;
            }
            return tmp;
        }

        public PERCENTUALEMAGCONIUGE RestituisciIlRecordPrecedente(decimal idMagCon)
        {
            PERCENTUALEMAGCONIUGE tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                PERCENTUALEMAGCONIUGE interessato = new PERCENTUALEMAGCONIUGE();
                interessato = db.PERCENTUALEMAGCONIUGE.Find(idMagCon);
                tmp = db.PERCENTUALEMAGCONIUGE.Where(a => a.IDTIPOLOGIACONIUGE == interessato.IDTIPOLOGIACONIUGE
                && a.ANNULLATO == false).ToList().Where(b => b.DATAFINEVALIDITA == interessato.DATAINIZIOVALIDITA.AddDays(-1)).ToList().First();
            }
            return tmp;
        }
        public static DateTime DataInizioMinimaNonAnnullataMagConiuge(decimal idLivello)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false && a.IDTIPOLOGIACONIUGE == idLivello).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }

        public decimal Get_Id_PERCENTUALEMAGCONIUGEPrimoNonAnnullato(decimal IDTIPOLOGIACONIUGE)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEMAGCONIUGE> libm = new List<PERCENTUALEMAGCONIUGE>();
                libm = db.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false
                && a.IDTIPOLOGIACONIUGE == IDTIPOLOGIACONIUGE).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDPERCMAGCONIUGE;
            }
            return tmp;
        }

        public List<string> RestituisciIntervalloDiUnaData(DateTime DataCampione, decimal IDTIPOLOGIACONIUGE)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEMAGCONIUGE> libm = new List<PERCENTUALEMAGCONIUGE>();
                libm = db.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false
                && a.IDTIPOLOGIACONIUGE == IDTIPOLOGIACONIUGE).ToList().Where(b =>
                b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())
                && DataCampione > b.DATAINIZIOVALIDITA
                && DataCampione < b.DATAFINEVALIDITA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCMAGCONIUGE.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALECONIUGE.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione, decimal IDTIPOLOGIACONIUGE)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEMAGCONIUGE> libm = new List<PERCENTUALEMAGCONIUGE>();
                libm = db.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false
                && a.IDTIPOLOGIACONIUGE == IDTIPOLOGIACONIUGE).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().Where(b => DataCampione == b.DATAINIZIOVALIDITA &&
                 b.DATAFINEVALIDITA != Utility.DataFineStop()).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCMAGCONIUGE.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALECONIUGE.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione, decimal IDTIPOLOGIACONIUGE)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEMAGCONIUGE> libm = new List<PERCENTUALEMAGCONIUGE>();
                libm = db.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false
                && a.IDTIPOLOGIACONIUGE == IDTIPOLOGIACONIUGE).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().
                Where(b => DataCampione == b.DATAFINEVALIDITA
                && b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())).ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCMAGCONIUGE.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALECONIUGE.ToString());
                }
            }
            return tmp;
        }
        public List<string> RestituisciLaRigaMassima(decimal IDTIPOLOGIACONIUGE)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEMAGCONIUGE> libm = new List<PERCENTUALEMAGCONIUGE>();
                libm = db.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false
                && a.IDTIPOLOGIACONIUGE == IDTIPOLOGIACONIUGE).ToList().Where(b =>
                b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop())).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCMAGCONIUGE.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALECONIUGE.ToString());
                }
            }
            return tmp;
        }
        public void RendiAnnullatoUnRecord(decimal idPercMagCon, ModelDBISE db)
        {
            PERCENTUALEMAGCONIUGE entita = new PERCENTUALEMAGCONIUGE();
            entita = db.PERCENTUALEMAGCONIUGE.Find(idPercMagCon);
            entita.ANNULLATO = true;
            db.SaveChanges();
        }
    }
}
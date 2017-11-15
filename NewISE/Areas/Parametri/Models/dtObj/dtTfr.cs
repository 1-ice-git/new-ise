using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtTfr : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<TFRModel> getListTfr()
        {
            List<TFRModel> libm = new List<TFRModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.TFR.ToList();

                    libm = (from e in lib
                            select new TFRModel()
                            {   
                                idTFR = e.IDTFR,
                                idValuta = e.IDVALUTA,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new TFRModel().dataFineValidita,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                Annullato = e.ANNULLATO,
                                //DescrizioneValuta = new ValuteModel()
                                //{
                                //    idValuta = e.VALUTE.IDVALUTA,
                                //    descrizioneValuta = e.VALUTE.DESCRIZIONEVALUTA,
                                //    valutaUfficiale = e.VALUTE.VALUTAUFFICIALE
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

        public IList<TFRModel> getListTfr(decimal idValuta)
        {
            List<TFRModel> libm = new List<TFRModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.TFR.Where(a => a.IDVALUTA == idValuta).ToList();

                    libm = (from e in lib
                            select new TFRModel()
                            {
                                idTFR = e.IDTFR,
                                idValuta = e.IDVALUTA,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new TFRModel().dataFineValidita,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                Annullato = e.ANNULLATO,
                                //DescrizioneValuta = new ValuteModel()
                                //{
                                //    idValuta = e.VALUTE.IDVALUTA,
                                //    descrizioneValuta = e.VALUTE.DESCRIZIONEVALUTA,
                                //    valutaUfficiale = e.VALUTE.VALUTAUFFICIALE
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

        public IList<TFRModel> getListTfr(bool escludiAnnullati = false)
        {
            List<TFRModel> libm = new List<TFRModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.TFR.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new TFRModel()
                            {
                                idTFR = e.IDTFR,
                                idValuta = e.IDVALUTA,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new TFRModel().dataFineValidita,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                Annullato = e.ANNULLATO,
                                //DescrizioneValuta = new ValuteModel()
                                //{
                                //    idValuta = e.VALUTE.IDVALUTA,
                                //    descrizioneValuta = e.VALUTE.DESCRIZIONEVALUTA,
                                //    valutaUfficiale = e.VALUTE.VALUTAUFFICIALE
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

        public IList<TFRModel> getListTfr(decimal idValuta, bool escludiAnnullati = false)
        {
            List<TFRModel> libm = new List<TFRModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.TFR.Where(a => a.IDVALUTA == idValuta && a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new TFRModel()
                            {
                                idTFR = e.IDTFR,
                                idValuta = e.IDVALUTA,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new TFRModel().dataFineValidita,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                Annullato = e.ANNULLATO,
                                //DescrizioneValuta = new ValuteModel()
                                //{
                                //    idValuta = e.VALUTE.IDVALUTA,
                                //    descrizioneValuta = e.VALUTE.DESCRIZIONEVALUTA,
                                //    valutaUfficiale = e.VALUTE.VALUTAUFFICIALE
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ibm"></param>
        public void SetTfr(TFRModel ibm)
        {
            List<TFR> libNew = new List<TFR>();

            TFR ibNew = new TFR();

            TFR ibPrecedente = new TFR();

            List<TFR> lArchivioIB = new List<TFR>();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    if (ibm.dataFineValidita.HasValue)
                    {
                        if (EsistonoMovimentiSuccessiviUguale(ibm))
                        {
                            ibNew = new TFR()
                            {   
                                IDTFR =ibm.idValuta,
                                IDVALUTA =ibm.idValuta,
                                TASSOCAMBIO =ibm.tassoCambio,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                                ANNULLATO = ibm.Annullato
                            };
                        }
                        else
                        {
                            ibNew = new TFR()
                            {

                                IDTFR = ibm.idValuta,
                                IDVALUTA = ibm.idValuta,
                                TASSOCAMBIO = ibm.tassoCambio,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                                DATAAGGIORNAMENTO = System.DateTime.Now,
                                ANNULLATO = ibm.Annullato
                            };
                        }
                    }
                    else
                    {
                        ibNew = new TFR()
                        {
                            IDTFR = ibm.idValuta,
                            IDVALUTA = ibm.idValuta,
                            TASSOCAMBIO = ibm.tassoCambio,
                            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                            DATAAGGIORNAMENTO = System.DateTime.Now,
                            ANNULLATO = ibm.Annullato
                        };
                    }

                    db.Database.BeginTransaction();

                    var recordInteressati = db.TFR.Where(a => a.ANNULLATO == false && a.IDVALUTA == ibNew.IDVALUTA)
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
                                    var ibOld1 = new TFR()
                                    {
                                        
                                        IDTFR =item.IDTFR,
                                        IDVALUTA = item.IDVALUTA,
                                        TASSOCAMBIO = item.TASSOCAMBIO,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        DATAAGGIORNAMENTO = System.DateTime.Now,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);

                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new TFR()
                                    {
                                        IDTFR = item.IDTFR,
                                        IDVALUTA = item.IDVALUTA,
                                        TASSOCAMBIO = item.TASSOCAMBIO,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        DATAAGGIORNAMENTO = System.DateTime.Now,
                                        ANNULLATO = false
                                    };

                                    var ibOld2 = new TFR()
                                    {
                                        IDTFR = item.IDTFR,
                                        IDVALUTA = item.IDVALUTA,
                                        TASSOCAMBIO = item.TASSOCAMBIO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(+1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        DATAAGGIORNAMENTO = System.DateTime.Now,
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
                                    var ibOld1 = new TFR()
                                    {
                                        IDTFR = item.IDTFR,
                                        IDVALUTA = item.IDVALUTA,
                                        TASSOCAMBIO = item.TASSOCAMBIO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        DATAAGGIORNAMENTO = System.DateTime.Now,
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
                                    var ibOld1 = new TFR()
                                    {
                                        IDTFR = item.IDTFR,
                                        IDVALUTA = item.IDVALUTA,
                                        TASSOCAMBIO = item.TASSOCAMBIO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        DATAAGGIORNAMENTO = System.DateTime.Now,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                }
                            }
                        }

                        libNew.Add(ibNew);
                        libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        db.TFR.AddRange(libNew);
                    }
                    else
                    {
                        db.TFR.Add(ibNew);

                    }
                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro di indennità di sistemazione.", "TFR", ibNew.IDTFR);
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

        public bool EsistonoMovimentiPrima(TFRModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.TFR.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDVALUTA == ibm.idValuta).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(TFRModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.TFR.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDVALUTA == ibm.idValuta).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(TFRModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.TFR.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDVALUTA == ibm.idValuta).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }



        public bool EsistonoMovimentiPrimaUguale(TFRModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.TFR.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDVALUTA == ibm.idValuta).Count() > 0 ? true : false;
            }
        }

        public void DelTfr(decimal idTfr)
        {
            TFR precedenteIB = new TFR();
            TFR delIB = new TFR();


            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.TFR.Where(a => a.IDTFR == idTfr);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.TFR.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new TFR()
                            {
                                IDTFR = precedenteIB.IDTFR,
                                IDVALUTA = precedenteIB.IDVALUTA,
                                TASSOCAMBIO = precedenteIB.TASSOCAMBIO,
                                DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                DATAAGGIORNAMENTO = precedenteIB.DATAAGGIORNAMENTO,
                                ANNULLATO = false
                            };

                            db.TFR.Add(ibOld1);
                        }

                        db.SaveChanges();

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di TFR.", "TFR", idTfr);
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


    }
}
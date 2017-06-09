using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParValute : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<ValuteModel> getListValute()
        {
            List<ValuteModel> libm = new List<ValuteModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.VALUTE.ToList();

                    libm = (from e in lib
                            select new ValuteModel()
                            {
                                idValuta = e.IDVALUTA,
                                valutaUfficiale = e.VALUTAUFFICIALE,
                                descrizioneValuta = e.DESCRIZIONEVALUTA
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<ValuteModel> getListValute(decimal idValute)
        {
            List<ValuteModel> libm = new List<ValuteModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.VALUTE.Where(a => a.IDVALUTA == idValute).ToList();

                    libm = (from e in lib
                            select new ValuteModel()
                            {
                                idValuta = e.IDVALUTA,
                                valutaUfficiale = e.VALUTAUFFICIALE,
                                descrizioneValuta = e.DESCRIZIONEVALUTA
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<ValuteModel> getListValute(bool escludiAnnullati = false)
        {
            List<ValuteModel> libm = new List<ValuteModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    //var lib = db.VALUTE.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    //libm = (from e in lib
                    //        select new IndennitaBaseModel()
                    //        {
                    //            idIndennitaBase = e.IDINDENNITABASE,
                    //            idLivello = e.IDLIVELLO,
                    //            dataInizioValidita = e.DATAINIZIOVALIDITA,
                    //            dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaBaseModel().dataFineValidita,
                    //            valore = e.VALORE,
                    //            valoreResponsabile = e.VALORERESP,
                    //            annullato = e.ANNULLATO,
                    //            Livello = new LivelloModel()
                    //            {
                    //                idLivello = e.LIVELLI.IDLIVELLO,
                    //                DescLivello = e.LIVELLI.LIVELLO
                    //            }
                    //        }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public IList<ValuteModel> getListValute(decimal idValute, bool escludiAnnullati = false)
        //{
        //    List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

        //    try
        //    {
        //        using (EntitiesDBISE db = new EntitiesDBISE())
        //        {
        //            var lib = db.INDENNITABASE.Where(a => a.IDLIVELLO == idLivello && a.ANNULLATO == escludiAnnullati).ToList();

        //            libm = (from e in lib
        //                    select new IndennitaBaseModel()
        //                    {
        //                        idIndennitaBase = e.IDINDENNITABASE,
        //                        idLivello = e.IDLIVELLO,
        //                        dataInizioValidita = e.DATAINIZIOVALIDITA,
        //                        dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaBaseModel().dataFineValidita,
        //                        valore = e.VALORE,
        //                        valoreResponsabile = e.VALORERESP,
        //                        annullato = e.ANNULLATO,
        //                        Livello = new LivelloModel()
        //                        {
        //                            idLivello = e.LIVELLI.IDLIVELLO,
        //                            DescLivello = e.LIVELLI.LIVELLO
        //                        }
        //                    }).ToList();
        //        }

        //        return libm;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ibm"></param>
        public void SetValute(ValuteModel ibm)
        {
            List<VALUTE> libNew = new List<VALUTE>();

            VALUTE ibNew = new VALUTE();

            //INDENNITABASE ibPrecedente = new INDENNITABASE();

            //List<INDENNITABASE> lArchivioIB = new List<INDENNITABASE>();

            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                try
                {
                    //if (ibm.dataFineValidita.HasValue)
                    //{
                    //    if (EsistonoMovimentiSuccessiviUguale(ibm))
                    //    {
                    //        ibNew = new INDENNITABASE()
                    //        {
                    //            IDLIVELLO = ibm.idLivello,
                    //            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                    //            DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                    //            VALORE = ibm.valore,
                    //            VALORERESP = ibm.valoreResponsabile,
                    //            ANNULLATO = ibm.annullato
                    //        };
                    //    }
                    //    else
                    //    {
                    //        ibNew = new INDENNITABASE()
                    //        {
                    //            IDLIVELLO = ibm.idLivello,
                    //            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                    //            DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                    //            VALORE = ibm.valore,
                    //            VALORERESP = ibm.valoreResponsabile,
                    //            ANNULLATO = ibm.annullato
                    //        };
                    //    }
                    //}
                    //else
                    //{
                    //    ibNew = new INDENNITABASE()
                    //    {
                    //        IDLIVELLO = ibm.idLivello,
                    //        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                    //        DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                    //        VALORE = ibm.valore,
                    //        VALORERESP = ibm.valoreResponsabile,
                    //        ANNULLATO = ibm.annullato
                    //    };
                    //}

                    db.Database.BeginTransaction();

                    //var recordInteressati = db.INDENNITABASE.Where(a => a.ANNULLATO == false && a.IDLIVELLO == ibNew.IDLIVELLO)
                    //                                        .Where(a => a.DATAINIZIOVALIDITA >= ibNew.DATAINIZIOVALIDITA || a.DATAFINEVALIDITA >= ibNew.DATAINIZIOVALIDITA)
                    //                                        .Where(a => a.DATAINIZIOVALIDITA <= ibNew.DATAFINEVALIDITA || a.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
                    //                                        .ToList();

                    //recordInteressati.ForEach(a => a.ANNULLATO = true);
                    ////db.SaveChanges();

                    //if (recordInteressati.Count > 0)
                    //{
                    //    foreach (var item in recordInteressati)
                    //    {

                    //        if (item.DATAINIZIOVALIDITA < ibNew.DATAINIZIOVALIDITA)
                    //        {
                    //            if (item.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
                    //            {
                    //                var ibOld1 = new INDENNITABASE()
                    //                {
                    //                    IDLIVELLO = item.IDLIVELLO,
                    //                    DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                    //                    DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                    //                    VALORE = item.VALORE,
                    //                    VALORERESP = item.VALORERESP,
                    //                    ANNULLATO = false
                    //                };

                    //                libNew.Add(ibOld1);

                    //            }
                    //            else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                    //            {
                    //                var ibOld1 = new INDENNITABASE()
                    //                {
                    //                    IDLIVELLO = item.IDLIVELLO,
                    //                    DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                    //                    DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                    //                    VALORE = item.VALORE,
                    //                    VALORERESP = item.VALORERESP,
                    //                    ANNULLATO = false
                    //                };

                    //                var ibOld2 = new INDENNITABASE()
                    //                {
                    //                    IDLIVELLO = item.IDLIVELLO,
                    //                    DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(+1),
                    //                    DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                    //                    VALORE = item.VALORE,
                    //                    VALORERESP = item.VALORERESP,
                    //                    ANNULLATO = false
                    //                };

                    //                libNew.Add(ibOld1);
                    //                libNew.Add(ibOld2);

                    //            }

                    //        }
                    //        else if (item.DATAINIZIOVALIDITA == ibNew.DATAINIZIOVALIDITA)
                    //        {
                    //            if (item.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
                    //            {
                    //                //Non preleva il record old
                    //            }
                    //            else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                    //            {
                    //                var ibOld1 = new INDENNITABASE()
                    //                {
                    //                    IDLIVELLO = item.IDLIVELLO,
                    //                    DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                    //                    DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                    //                    VALORE = item.VALORE,
                    //                    VALORERESP = item.VALORERESP,
                    //                    ANNULLATO = false
                    //                };

                    //                libNew.Add(ibOld1);
                    //            }
                    //        }
                    //        else if (item.DATAINIZIOVALIDITA > ibNew.DATAINIZIOVALIDITA)
                    //        {
                    //            if (item.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
                    //            {
                    //                //Non preleva il record old
                    //            }
                    //            else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                    //            {
                    //                var ibOld1 = new INDENNITABASE()
                    //                {
                    //                    IDLIVELLO = item.IDLIVELLO,
                    //                    DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                    //                    DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                    //                    VALORE = item.VALORE,
                    //                    VALORERESP = item.VALORERESP,
                    //                    ANNULLATO = false
                    //                };

                    //                libNew.Add(ibOld1);
                    //            }
                    //        }
                    //    }

                    //    libNew.Add(ibNew);
                    //    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        db.VALUTE.AddRange(libNew);
                    //}
                    //else
                    //{
                        db.VALUTE.Add(ibNew);

                    //}



                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro valute.", "VALUTE", ibNew.IDVALUTA);
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

        //public bool EsistonoMovimentiPrima(ValuteModel ibm)
        //{
        //    using (EntitiesDBISE db = new EntitiesDBISE())
        //    {
        //        return db.INDENNITABASE.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDLIVELLO == ibm.idLivello).Count() > 0 ? true : false;
        //    }
        //}

        //public bool EsistonoMovimentiSuccessivi(ValuteModel ibm)
        //{
        //    using (EntitiesDBISE db = new EntitiesDBISE())
        //    {
        //        if (ibm.dataFineValidita.HasValue)
        //        {
        //            return db.INDENNITABASE.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDLIVELLO == ibm.idLivello).Count() > 0 ? true : false;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}

        //public bool EsistonoMovimentiSuccessiviUguale(ValuteModel ibm)
        //{
        //    using (EntitiesDBISE db = new EntitiesDBISE())
        //    {
        //        if (ibm.dataFineValidita.HasValue)
        //        {
        //            return db.INDENNITABASE.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDLIVELLO == ibm.idLivello).Count() > 0 ? true : false;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}



        //public bool EsistonoMovimentiPrimaUguale(ValuteModel ibm)
        //{
        //    using (EntitiesDBISE db = new EntitiesDBISE())
        //    {
        //        return db.INDENNITABASE.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDLIVELLO == ibm.idLivello).Count() > 0 ? true : false;
        //    }
        //}

        public void DelValute(decimal idValuta)
        {
            VALUTE precedenteIB = new VALUTE();
            VALUTE delIB = new VALUTE();


            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.VALUTE.Where(a => a.IDVALUTA == idValuta);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        //delIB.ANNULLATO = true;

                        //var lprecIB = db.INDENNITABASE.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        //if (lprecIB.Count > 0)
                        //{
                        //    precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                        //    precedenteIB.ANNULLATO = true;

                        //    var ibOld1 = new INDENNITABASE()
                        //    {
                        //        IDLIVELLO = precedenteIB.IDLIVELLO,
                        //        DATAINIZIOVALIDITA = precedenteIB.DATAFINEVALIDITA,
                        //        DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                        //        VALORE = precedenteIB.VALORE,
                        //        VALORERESP = precedenteIB.VALORERESP,
                        //        ANNULLATO = false
                        //    };

                        //    db.INDENNITABASE.Add(ibOld1);
                        //}

                        db.SaveChanges();

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro valute.", "VALUTE", idValuta);
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






        public IList<ValuteModel> GetValute()
        {
            List<ValuteModel> llm = new List<ValuteModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var ll = db.VALUTE.ToList();

                    llm = (from e in ll
                           select new ValuteModel()
                           {
                               
                               idValuta = e.IDVALUTA,
                               descrizioneValuta = e.DESCRIZIONEVALUTA
                               
                               
                           }).ToList();
                }

                return llm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ValuteModel GetValute(decimal idValuta)
        {
            ValuteModel lm = new ValuteModel();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var liv = db.VALUTE.Find(idValuta);

                    lm = new ValuteModel()
                    {
                        idValuta = liv.IDVALUTA,
                        descrizioneValuta = liv.DESCRIZIONEVALUTA
                    };
                }

                return lm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
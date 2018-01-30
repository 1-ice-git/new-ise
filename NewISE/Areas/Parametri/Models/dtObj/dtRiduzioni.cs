using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtRiduzioni : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<RiduzioniModel> getListRiduzioni()
        {
            List<RiduzioniModel> libm = new List<RiduzioniModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.RIDUZIONI.ToList();

                    libm = (from e in lib
                            select new RiduzioniModel()
                            {
                                idRiduzioni = e.IDRIDUZIONI,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                percentuale = e.PERCENTUALE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                idFunzioneRiduzione=e.IDFUNZIONERIDUZIONE
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public IList<RiduzioniModel> getListRiduzioni(decimal idRegola)
        //{
        //    List<RiduzioniModel> libm = new List<RiduzioniModel>();

        //    try
        //    {
        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            var lib = db.RIDUZIONI.Where(a => a.IDREGOLA == idRegola).ToList();

        //            libm = (from e in lib
        //                    select new RiduzioniModel()
        //                    {

        //                        idRiduzioni = e.IDRIDUZIONI,
        //                        idRegola = e.IDREGOLA,
        //                        dataInizioValidita = e.DATAINIZIOVALIDITA,
        //                        dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new RiduzioniModel().dataFineValidita,
        //                        percentuale = e.PERCENTUALE,
        //                        dataAggiornamento = e.DATAAGGIORNAMENTO,
        //                        annullato = e.ANNULLATO
        //                        //FormulaRegolaCalcolo = new RegoleCalcoloModel()
        //                        //{
        //                        //    idRegola = e.REGOLECALCOLO.IDREGOLA,
        //                        //    FormulaRegolaCalcolo = e.REGOLECALCOLO.FORMULAREGOLACALCOLO
        //                        //}
        //                    }).ToList();
        //        }

        //        return libm;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public IList<RiduzioniModel> getListRiduzioni(bool escludiAnnullati = false)
        {
            List<RiduzioniModel> libm = new List<RiduzioniModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<RIDUZIONI> lib = new List<RIDUZIONI>();

                    if(escludiAnnullati==true)
                       lib= db.RIDUZIONI.Where(a => a.ANNULLATO == false).ToList();
                    else
                       lib= db.RIDUZIONI.ToList();

                    libm = (from e in lib
                            select new RiduzioniModel()
                            {
                                idRiduzioni = e.IDRIDUZIONI,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA ,
                                percentuale = e.PERCENTUALE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                idFunzioneRiduzione = e.IDFUNZIONERIDUZIONE
                            }).ToList();
                }
                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public IList<RiduzioniModel> getListRiduzioni(decimal idRegola, bool escludiAnnullati = false)
        //{
        //    List<RiduzioniModel> libm = new List<RiduzioniModel>();

        //    try
        //    {
        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            var lib = db.RIDUZIONI.Where(a => a.IDREGOLA == idRegola && a.ANNULLATO == escludiAnnullati).ToList();

        //            libm = (from e in lib
        //                    select new RiduzioniModel()
        //                    {

        //                        idRiduzioni = e.IDRIDUZIONI,
        //                        idRegola = e.IDREGOLA,
        //                        dataInizioValidita = e.DATAINIZIOVALIDITA,
        //                        dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new RiduzioniModel().dataFineValidita,
        //                        percentuale = e.PERCENTUALE,
        //                        dataAggiornamento = e.DATAAGGIORNAMENTO,
        //                        annullato = e.ANNULLATO,
        //                        //FormulaRegolaCalcolo = new RegoleCalcoloModel()
        //                        //{
        //                        //    idRegola = e.REGOLECALCOLO.IDREGOLA,
        //                        //    FormulaRegolaCalcolo = e.REGOLECALCOLO.FORMULAREGOLACALCOLO
        //                        //}
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
        public void SetRiduzioni(RiduzioniModel ibm)
        {
            List<RIDUZIONI> libNew = new List<RIDUZIONI>();

            RIDUZIONI ibNew = new RIDUZIONI();

            RIDUZIONI ibPrecedente = new RIDUZIONI();

            List<RIDUZIONI> lArchivioIB = new List<RIDUZIONI>();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    if (ibm.dataFineValidita.HasValue)
                    {
                        if (EsistonoMovimentiSuccessiviUguale(ibm))
                        {
                            ibNew = new RIDUZIONI()
                            {

                                IDRIDUZIONI = ibm.idRiduzioni,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                PERCENTUALE = ibm.percentuale,
                                DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                                ANNULLATO = ibm.annullato
                            };
                        }
                        else
                        {
                            ibNew = new RIDUZIONI()
                            {

                                IDRIDUZIONI = ibm.idRiduzioni,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                                PERCENTUALE = ibm.percentuale,
                                DATAAGGIORNAMENTO = System.DateTime.Now,
                                ANNULLATO = ibm.annullato
                            };
                        }
                    }
                    else
                    {
                        ibNew = new RIDUZIONI()
                        {

                            IDRIDUZIONI = ibm.idRiduzioni,
                            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                            PERCENTUALE = ibm.percentuale,
                            DATAAGGIORNAMENTO = System.DateTime.Now,
                            ANNULLATO = ibm.annullato
                        };
                    }

                    db.Database.BeginTransaction();

                    var recordInteressati = db.RIDUZIONI.Where(a => a.ANNULLATO == false)
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
                                    var ibOld1 = new RIDUZIONI()
                                    {
                                        IDRIDUZIONI = item.IDRIDUZIONI,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        PERCENTUALE = item.PERCENTUALE,
                                        DATAAGGIORNAMENTO = System.DateTime.Now,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);

                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new RIDUZIONI()
                                    {

                                        IDRIDUZIONI = item.IDRIDUZIONI,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        PERCENTUALE = item.PERCENTUALE,
                                        DATAAGGIORNAMENTO = System.DateTime.Now,
                                        ANNULLATO = false
                                    };

                                    var ibOld2 = new RIDUZIONI()
                                    {
                                        IDRIDUZIONI = item.IDRIDUZIONI,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(+1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        PERCENTUALE = item.PERCENTUALE,
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
                                    var ibOld1 = new RIDUZIONI()
                                    {

                                        IDRIDUZIONI = item.IDRIDUZIONI,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        PERCENTUALE = item.PERCENTUALE,
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
                                    var ibOld1 = new RIDUZIONI()
                                    {

                                        IDRIDUZIONI = item.IDRIDUZIONI,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        PERCENTUALE = item.PERCENTUALE,
                                        DATAAGGIORNAMENTO = System.DateTime.Now,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                }
                            }
                        }

                        libNew.Add(ibNew);
                        libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        db.RIDUZIONI.AddRange(libNew);
                    }
                    else
                    {
                        db.RIDUZIONI.Add(ibNew);

                    }
                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro di riduzione.", "RIDUZIONI", ibNew.IDRIDUZIONI);
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

        public bool EsistonoMovimentiPrima(RiduzioniModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.RIDUZIONI.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(RiduzioniModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.RIDUZIONI.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(RiduzioniModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.RIDUZIONI.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }



        public bool EsistonoMovimentiPrimaUguale(RiduzioniModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.RIDUZIONI.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita).Count() > 0 ? true : false;
            }
        }

        public void DelRiduzioni(decimal idRiduzioni)
        {
            RIDUZIONI precedenteIB = new RIDUZIONI();
            RIDUZIONI delIB = new RIDUZIONI();


            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.RIDUZIONI.Where(a => a.IDRIDUZIONI == idRiduzioni);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.RIDUZIONI.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new RIDUZIONI()
                            {

                                IDRIDUZIONI = precedenteIB.IDRIDUZIONI,
                                DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                PERCENTUALE = precedenteIB.PERCENTUALE,
                                DATAAGGIORNAMENTO = precedenteIB.DATAAGGIORNAMENTO,
                                ANNULLATO = false
                            };

                            db.RIDUZIONI.Add(ibOld1);
                        }

                        db.SaveChanges();

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di riduzione.", "RIDUZIONI", idRiduzioni);
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
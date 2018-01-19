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
    public class dtMaggConiuge : IDisposable
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
                                //idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new PercentualeMagConiugeModel().dataFineValidita,
                                percentualeConiuge = e.PERCENTUALECONIUGE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                Coniuge = new TipologiaConiugeModel()
                                {
                                    idTipologiaConiuge = e.TIPOLOGIACONIUGE.IDTIPOLOGIACONIUGE,
                                    tipologiaConiuge = e.TIPOLOGIACONIUGE.TIPOLOGIACONIUGE1
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
                                //idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new PercentualeMagConiugeModel().dataFineValidita,
                                percentualeConiuge = e.PERCENTUALECONIUGE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                Coniuge = new TipologiaConiugeModel()
                                {
                                    idTipologiaConiuge = e.TIPOLOGIACONIUGE.IDTIPOLOGIACONIUGE,
                                    tipologiaConiuge = e.TIPOLOGIACONIUGE.TIPOLOGIACONIUGE1
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
                                //idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new PercentualeMagConiugeModel().dataFineValidita,
                                percentualeConiuge = e.PERCENTUALECONIUGE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                Coniuge = new TipologiaConiugeModel()
                                {
                                    idTipologiaConiuge = e.TIPOLOGIACONIUGE.IDTIPOLOGIACONIUGE,
                                    tipologiaConiuge = e.TIPOLOGIACONIUGE.TIPOLOGIACONIUGE1
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
                    List<PERCENTUALEMAGCONIUGE> lib = new List<PERCENTUALEMAGCONIUGE>();
                    if(escludiAnnullati==true)
                        lib = db.PERCENTUALEMAGCONIUGE.Where(a => a.IDTIPOLOGIACONIUGE == idTipologiaConiuge && a.ANNULLATO == false).ToList();
                    else
                        lib = db.PERCENTUALEMAGCONIUGE.Where(a => a.IDTIPOLOGIACONIUGE == idTipologiaConiuge).ToList();

                    libm = (from e in lib
                            select new PercentualeMagConiugeModel()
                            {
                                idPercentualeConiuge = e.IDPERCMAGCONIUGE,
                                //idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA ,//!= Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new PercentualeMagConiugeModel().dataFineValidita,
                                percentualeConiuge = e.PERCENTUALECONIUGE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                Coniuge = new TipologiaConiugeModel()
                                {
                                    idTipologiaConiuge = e.TIPOLOGIACONIUGE.IDTIPOLOGIACONIUGE,
                                    tipologiaConiuge = e.TIPOLOGIACONIUGE.TIPOLOGIACONIUGE1
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
        public void SetPercMagConiuge(PercentualeMagConiugeModel ibm,decimal idTipologiaConiuge)
        {
            List<PERCENTUALEMAGCONIUGE> libNew = new List<PERCENTUALEMAGCONIUGE>();

            PERCENTUALEMAGCONIUGE ibNew = new PERCENTUALEMAGCONIUGE();

            PERCENTUALEMAGCONIUGE ibPrecedente = new PERCENTUALEMAGCONIUGE();

            List<PERCENTUALEMAGCONIUGE> lArchivioIB = new List<PERCENTUALEMAGCONIUGE>();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    if (ibm.dataFineValidita.HasValue)
                    {
                        if (EsistonoMovimentiSuccessiviUguale(ibm))
                        {
                            ibNew = new PERCENTUALEMAGCONIUGE()
                            {
                              //  IDPERCMAGCONIUGE = ibm.idPercentualeConiuge,
                                IDTIPOLOGIACONIUGE = idTipologiaConiuge,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                PERCENTUALECONIUGE = ibm.percentualeConiuge,
                                DATAAGGIORNAMENTO = DateTime.Now,
                                ANNULLATO = ibm.annullato
                            };
                        }
                        else
                        {
                            ibNew = new PERCENTUALEMAGCONIUGE()
                            {
                            //    IDPERCMAGCONIUGE = ibm.idPercentualeConiuge,
                                IDTIPOLOGIACONIUGE = idTipologiaConiuge,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA =ibm.dataFineValidita.Value,//Utility.DataFineStop(),// Convert.ToDateTime("31/12/9999"),
                                PERCENTUALECONIUGE = ibm.percentualeConiuge,
                                DATAAGGIORNAMENTO = DateTime.Now,
                                ANNULLATO = ibm.annullato
                            };
                        }
                    }
                    else
                    {
                        ibNew = new PERCENTUALEMAGCONIUGE()
                        {
                          //  IDPERCMAGCONIUGE = ibm.idPercentualeConiuge,
                            IDTIPOLOGIACONIUGE =idTipologiaConiuge,
                            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            DATAFINEVALIDITA = Utility.DataFineStop(),//Convert.ToDateTime("31/12/9999"),
                            PERCENTUALECONIUGE = ibm.percentualeConiuge,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            ANNULLATO = ibm.annullato
                        };
                    }

                    db.Database.BeginTransaction();

                    var recordInteressati = db.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false && a.IDTIPOLOGIACONIUGE == ibNew.IDTIPOLOGIACONIUGE)
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
                                    var ibOld1 = new PERCENTUALEMAGCONIUGE()
                                    {
                                       // IDPERCMAGCONIUGE = ibm.idPercentualeConiuge,
                                        IDTIPOLOGIACONIUGE = idTipologiaConiuge,// item.IDTIPOLOGIACONIUGE,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        PERCENTUALECONIUGE = item.PERCENTUALECONIUGE,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);

                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new PERCENTUALEMAGCONIUGE()
                                    {
                                    //    IDPERCMAGCONIUGE = ibm.idPercentualeConiuge,
                                        IDTIPOLOGIACONIUGE = idTipologiaConiuge,//item.IDTIPOLOGIACONIUGE,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        PERCENTUALECONIUGE = item.PERCENTUALECONIUGE,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    var ibOld2 = new PERCENTUALEMAGCONIUGE()
                                    {
                                      //  IDPERCMAGCONIUGE = ibm.idPercentualeConiuge,
                                        IDTIPOLOGIACONIUGE = idTipologiaConiuge,// item.IDTIPOLOGIACONIUGE,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(+1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        PERCENTUALECONIUGE = item.PERCENTUALECONIUGE,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
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
                                    var ibOld1 = new PERCENTUALEMAGCONIUGE()
                                    {
                                      //  IDPERCMAGCONIUGE = ibm.idPercentualeConiuge,
                                        IDTIPOLOGIACONIUGE = idTipologiaConiuge,// item.IDTIPOLOGIACONIUGE,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        PERCENTUALECONIUGE = item.PERCENTUALECONIUGE,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
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
                                    var ibOld1 = new PERCENTUALEMAGCONIUGE()
                                    {
                                       // IDPERCMAGCONIUGE = ibm.idPercentualeConiuge,
                                        IDTIPOLOGIACONIUGE = idTipologiaConiuge,// item.IDTIPOLOGIACONIUGE,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        PERCENTUALECONIUGE = item.PERCENTUALECONIUGE,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                }
                            }
                        }

                        libNew.Add(ibNew);
                        libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        db.PERCENTUALEMAGCONIUGE.AddRange(libNew);
                    }
                    else
                    {
                        db.PERCENTUALEMAGCONIUGE.Add(ibNew);

                    }
                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro di percentuale maggiorazione coniuge.", "PERCENTUALEMAGCONIUGE", ibNew.IDPERCMAGCONIUGE);
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

        public bool EsistonoMovimentiPrima(PercentualeMagConiugeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEMAGCONIUGE.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(PercentualeMagConiugeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.PERCENTUALEMAGCONIUGE.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value).Count() > 0 ? true : false;
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
                    return db.PERCENTUALEMAGCONIUGE.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value).Count() > 0 ? true : false;
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
                return db.PERCENTUALEMAGCONIUGE.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita).Count() > 0 ? true : false;
            }
        }

        public void DelPercMagConiuge(decimal idPercentualeConiuge)
        {
            PERCENTUALEMAGCONIUGE precedenteIB = new PERCENTUALEMAGCONIUGE();
            PERCENTUALEMAGCONIUGE delIB = new PERCENTUALEMAGCONIUGE();


            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.PERCENTUALEMAGCONIUGE.Where(a => a.IDPERCMAGCONIUGE == idPercentualeConiuge);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.PERCENTUALEMAGCONIUGE.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new PERCENTUALEMAGCONIUGE()
                            {
                                IDPERCMAGCONIUGE = precedenteIB.IDPERCMAGCONIUGE,
                                IDTIPOLOGIACONIUGE = precedenteIB.IDTIPOLOGIACONIUGE,
                                DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                PERCENTUALECONIUGE = precedenteIB.PERCENTUALECONIUGE,
                                ANNULLATO = false
                            };

                            db.PERCENTUALEMAGCONIUGE.Add(ibOld1);
                        }

                        db.SaveChanges();

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di percentuale maggiorazione coniuge.", "PERCENTUALEMAGCONIUGE", idPercentualeConiuge);
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
                DateTime d = DataInizioMinimaNonAnnullata(fm.id_TipologiaConiuge);
                if (fm.dataInizioValidita < d)
                {
                    vr = new ValidationResult(string.Format("Impossibile inserire la data di inizio validità minore alla data di Base ({0}).", d.ToShortDateString()));
                }
                else
                {
                    if (fm.dataFineValidita < fm.dataInizioValidita)
                    {
                        vr = new ValidationResult(string.Format("Impossibile inserire la data di inizio validità maggiore alla data di partenza del trasferimento ({0}).", fm.dataFineValidita.Value.ToShortDateString()));
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
    }
}
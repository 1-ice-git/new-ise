using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtCoeffFasciaKm : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        ////////public IList<CoeffFasciaKmModel> getListCoeffFasciaKm()
        ////////{
        ////////    List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();

        ////////    try
        ////////    {
        ////////        using (ModelDBISE db = new ModelDBISE())
        ////////        {
        ////////            var lib = db.COEFFICIENTEFKM.ToList();

        ////////            libm = (from e in lib
        ////////                    select new CoeffFasciaKmModel()
        ////////                    {   
        ////////                        idCfKm = e.IDCFKM,
        ////////                        idDefKm = e.IDDEFKM,
        ////////                        dataInizioValidita = e.DATAINIZIOVALIDITA,
        ////////                        dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new CoeffFasciaKmModel().dataFineValidita,
        ////////                        coefficienteKm = e.COEFFICIENTEKM,
        ////////                        dataAggiornamento = e.DATAAGGIORNAMENTO,
        ////////                        annullato = e.ANNULLATO,
        ////////                        km = new DefFasciaKmModel()
        ////////                        {
        ////////                            idDefKm = e.IDDEFKM,
        ////////                            km = e.DEFFASCIACHILOMETRICA.KM
        ////////                        }
        ////////                    }).ToList();
        ////////        }

        ////////        return libm;
        ////////    }
        ////////    catch (Exception ex)
        ////////    {
        ////////        throw ex;
        ////////    }
        ////////}

        //////public IList<CoeffFasciaKmModel> getListCoeffFasciaKm(decimal idDefKm)
        //////{
        //////    List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();

        //////    try
        //////    {
        //////        using (ModelDBISE db = new ModelDBISE())
        //////        {
        //////            var lib = db.COEFFICIENTEFKM.Where(a => a.IDDEFKM == idDefKm).ToList();

        //////            libm = (from e in lib
        //////                    select new CoeffFasciaKmModel()
        //////                    {
        //////                        idCfKm = e.IDCFKM,
        //////                        idDefKm = e.IDDEFKM,
        //////                        dataInizioValidita = e.DATAINIZIOVALIDITA,
        //////                        dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new CoeffFasciaKmModel().dataFineValidita,
        //////                        coefficienteKm = e.COEFFICIENTEKM,
        //////                        dataAggiornamento = e.DATAAGGIORNAMENTO,
        //////                        annullato = e.ANNULLATO,
        //////                        km = new DefFasciaKmModel()
        //////                        {
        //////                            idDefKm = e.IDDEFKM,
        //////                            km = e.DEFFASCIACHILOMETRICA.KM
        //////                        }
        //////                    }).ToList();
        //////        }

        //////        return libm;
        //////    }
        //////    catch (Exception ex)
        //////    {
        //////        throw ex;
        //////    }
        //////}

        ////public IList<CoeffFasciaKmModel> getListCoeffFasciaKm(bool escludiAnnullati = false)
        ////{
        ////    List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();

        ////    try
        ////    {
        ////        using (ModelDBISE db = new ModelDBISE())
        ////        {
        ////            var lib = db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

        ////            libm = (from e in lib
        ////                    select new CoeffFasciaKmModel()
        ////                    {
        ////                        idCfKm = e.IDCFKM,
        ////                        idDefKm = e.IDDEFKM,
        ////                        dataInizioValidita = e.DATAINIZIOVALIDITA,
        ////                        dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new CoeffFasciaKmModel().dataFineValidita,
        ////                        coefficienteKm = e.COEFFICIENTEKM,
        ////                        dataAggiornamento = e.DATAAGGIORNAMENTO,
        ////                        annullato = e.ANNULLATO,
        ////                        km = new DefFasciaKmModel()
        ////                        {
        ////                            idDefKm = e.IDDEFKM,
        ////                            km = e.DEFFASCIACHILOMETRICA.KM
        ////                        }
        ////                    }).ToList();
        ////        }

        ////        return libm;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        throw ex;
        ////    }
        ////}

        //public IList<CoeffFasciaKmModel> getListCoeffFasciaKm(decimal idDefKm, bool escludiAnnullati = false)
        //{
        //    List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();

        //    try
        //    {
        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            var lib = db.COEFFICIENTEFKM.Where(a => a.IDDEFKM == idDefKm && a.ANNULLATO == escludiAnnullati).ToList();

        //            libm = (from e in lib
        //                    select new CoeffFasciaKmModel()
        //                    {
        //                        idCfKm = e.IDCFKM,
        //                        idDefKm = e.IDDEFKM,
        //                        dataInizioValidita = e.DATAINIZIOVALIDITA,
        //                        dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new CoeffFasciaKmModel().dataFineValidita,
        //                        coefficienteKm = e.COEFFICIENTEKM,
        //                        dataAggiornamento = e.DATAAGGIORNAMENTO,
        //                        annullato = e.ANNULLATO,
        //                        km = new DefFasciaKmModel()
        //                        {
        //                            idDefKm = e.DEFFASCIACHILOMETRICA.IDDEFKM,
        //                            km = e.DEFFASCIACHILOMETRICA.KM
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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="ibm"></param>
        //public void SetCoeffFasciaKm(CoeffFasciaKmModel ibm)
        //{
        //    List<COEFFICIENTEFKM> libNew = new List<COEFFICIENTEFKM>();

        //    COEFFICIENTEFKM ibNew = new COEFFICIENTEFKM();

        //    COEFFICIENTEFKM ibPrecedente = new COEFFICIENTEFKM();

        //    List<COEFFICIENTEFKM> lArchivioIB = new List<COEFFICIENTEFKM>();

        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        try
        //        {
        //            if (ibm.dataFineValidita.HasValue)
        //            {
        //                if (EsistonoMovimentiSuccessiviUguale(ibm))
        //                {
        //                    ibNew = new COEFFICIENTEFKM()
        //                    {
        //                        IDCFKM = ibm.idCfKm,
        //                        IDDEFKM = ibm.idDefKm,
        //                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
        //                        DATAFINEVALIDITA = ibm.dataFineValidita.Value,
        //                        COEFFICIENTEKM = ibm.coefficienteKm,
        //                        DATAAGGIORNAMENTO = DateTime.Now,
        //                        ANNULLATO = ibm.annullato
        //                    };
        //                }
        //                else
        //                {
        //                    ibNew = new COEFFICIENTEFKM()
        //                    {
        //                        IDCFKM = ibm.idCfKm,
        //                        IDDEFKM = ibm.idDefKm,
        //                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
        //                        DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
        //                        COEFFICIENTEKM = ibm.coefficienteKm,
        //                        DATAAGGIORNAMENTO = DateTime.Now,
        //                        ANNULLATO = ibm.annullato
        //                    };
        //                }
        //            }
        //            else
        //            {
        //                ibNew = new COEFFICIENTEFKM()
        //                {

        //                    IDCFKM = ibm.idCfKm,
        //                    IDDEFKM = ibm.idDefKm,
        //                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
        //                    DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
        //                    COEFFICIENTEKM = ibm.coefficienteKm,
        //                    DATAAGGIORNAMENTO = DateTime.Now,
        //                    ANNULLATO = ibm.annullato
        //                };
        //            }

        //            db.Database.BeginTransaction();

        //            var recordInteressati = db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == false && a.IDDEFKM == ibNew.IDDEFKM)
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
        //                            var ibOld1 = new COEFFICIENTEFKM()
        //                            {
        //                                IDCFKM = item.IDCFKM,
        //                                IDDEFKM = item.IDDEFKM,
        //                                DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
        //                                DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
        //                                COEFFICIENTEKM = item.COEFFICIENTEKM,
        //                                DATAAGGIORNAMENTO = DateTime.Now,
        //                                ANNULLATO = false
        //                            };

        //                            libNew.Add(ibOld1);

        //                        }
        //                        else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
        //                        {
        //                            var ibOld1 = new COEFFICIENTEFKM()
        //                            {
        //                                IDCFKM = item.IDCFKM,
        //                                IDDEFKM = item.IDDEFKM,
        //                                DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
        //                                DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
        //                                COEFFICIENTEKM = item.COEFFICIENTEKM,
        //                                DATAAGGIORNAMENTO = DateTime.Now,
        //                                ANNULLATO = false
        //                            };

        //                            var ibOld2 = new COEFFICIENTEFKM()
        //                            {
        //                                IDCFKM = item.IDCFKM,
        //                                IDDEFKM = item.IDDEFKM,
        //                                DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(+1),
        //                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,
        //                                COEFFICIENTEKM = item.COEFFICIENTEKM,
        //                                DATAAGGIORNAMENTO = DateTime.Now,
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
        //                            var ibOld1 = new COEFFICIENTEFKM()
        //                            {
        //                                IDCFKM = item.IDCFKM,
        //                                IDDEFKM = item.IDDEFKM,
        //                                DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
        //                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,
        //                                COEFFICIENTEKM = item.COEFFICIENTEKM,
        //                                DATAAGGIORNAMENTO = DateTime.Now,
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
        //                            var ibOld1 = new COEFFICIENTEFKM()
        //                            {
        //                                IDCFKM = item.IDCFKM,
        //                                IDDEFKM = item.IDDEFKM,
        //                                DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
        //                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,
        //                                COEFFICIENTEKM = item.COEFFICIENTEKM,
        //                                DATAAGGIORNAMENTO = DateTime.Now,
        //                                ANNULLATO = false
        //                            };

        //                            libNew.Add(ibOld1);
        //                        }
        //                    }
        //                }

        //                libNew.Add(ibNew);
        //                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

        //                db.COEFFICIENTEFKM.AddRange(libNew);
        //            }
        //            else
        //            {
        //                db.COEFFICIENTEFKM.Add(ibNew);

        //            }
        //            db.SaveChanges();

        //            using (objLogAttivita log = new objLogAttivita())
        //            {
        //                log.Log(enumAttivita.Inserimento, "Inserimento parametro di coefficiente di fascia kilometerica.", "COEFFICIENTEFKM", ibNew.IDCFKM);
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

        //public bool EsistonoMovimentiPrima(CoeffFasciaKmModel ibm)
        //{
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        return db.COEFFICIENTEFKM.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita).Count() > 0 ? true : false;
        //    }
        //}

        //public bool EsistonoMovimentiSuccessivi(CoeffFasciaKmModel ibm)
        //{
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        if (ibm.dataFineValidita.HasValue)
        //        {
        //            return db.COEFFICIENTEFKM.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value).Count() > 0 ? true : false;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}

        //public bool EsistonoMovimentiSuccessiviUguale(CoeffFasciaKmModel ibm)
        //{
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        if (ibm.dataFineValidita.HasValue)
        //        {
        //            return db.COEFFICIENTEFKM.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value).Count() > 0 ? true : false;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}
        //public bool EsistonoMovimentiPrimaUguale(CoeffFasciaKmModel ibm)
        //{
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        return db.COEFFICIENTEFKM.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita).Count() > 0 ? true : false;
        //    }
        //}

        //public void DelCoeffFasciaKm(decimal idCfKm)
        //{
        //    COEFFICIENTEFKM precedenteIB = new COEFFICIENTEFKM();
        //    COEFFICIENTEFKM delIB = new COEFFICIENTEFKM();


        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        try
        //        {
        //            db.Database.BeginTransaction();

        //            var lib = db.COEFFICIENTEFKM.Where(a => a.IDCFKM == idCfKm);

        //            if (lib.Count() > 0)
        //            {
        //                delIB = lib.First();
        //                delIB.ANNULLATO = true;

        //                var lprecIB = db.COEFFICIENTEFKM.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

        //                if (lprecIB.Count > 0)
        //                {
        //                    precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
        //                    precedenteIB.ANNULLATO = true;

        //                    var ibOld1 = new COEFFICIENTEFKM()
        //                    {
        //                        IDCFKM = precedenteIB.IDCFKM,
        //                        IDDEFKM = precedenteIB.IDDEFKM,
        //                        DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
        //                        DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
        //                        COEFFICIENTEKM = precedenteIB.COEFFICIENTEKM,
        //                        DATAAGGIORNAMENTO = precedenteIB.DATAAGGIORNAMENTO,
        //                        ANNULLATO = false
        //                    };

        //                    db.COEFFICIENTEFKM.Add(ibOld1);
        //                }

        //                db.SaveChanges();

        //                using (objLogAttivita log = new objLogAttivita())
        //                {
        //                    log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di coefficiente fascia km.", "COEFFICIENTEFKM", idCfKm);
        //                }


        //                db.Database.CurrentTransaction.Commit();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            db.Database.CurrentTransaction.Rollback();
        //            throw ex;
        //        }

        //    }

        //}

        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as CoeffFasciaKmModel;
            if (fm != null)
            {
                if (fm.dataFineValidita < fm.dataInizioValidita)
                {
                    vr = new ValidationResult(string.Format("Impossibile inserire la data di inizio validità minore alla data di partenza del trasferimento ({0}).", fm.dataFineValidita.Value.ToShortDateString()));
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
    }
}
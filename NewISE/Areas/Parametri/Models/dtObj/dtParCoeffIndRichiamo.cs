﻿using NewISE.EF;
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
    public class dtParCoeffIndRichiamo : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        //public IList<CoefficienteRichiamoModel> getListCoeffIndRichiamo()
        //{
        //    List<CoefficienteRichiamoModel> libm = new List<CoefficienteRichiamoModel>();

        //    try
        //    {
        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            var lib = db.COEFFICIENTEINDRICHIAMO.OrderBy(a => a.DATAINIZIOVALIDITA).OrderBy(a => a.DATAFINEVALIDITA).ToList();

        //            libm = (from e in lib
        //                    select new CoefficienteRichiamoModel()
        //                    {
        //                        idCoefIndRichiamo = e.COEFFICIENTERICHIAMO,
        //                        dataInizioValidita = e.DATAINIZIOVALIDITA,
        //                        dataFineValidita = e.DATAFINEVALIDITA,
        //                        coefficienteRichiamo = e.COEFFICIENTERICHIAMO,
        //                        dataAggiornamento = e.DATAAGGIORNAMENTO,
        //                        annullato = e.ANNULLATO,
        //                    }).ToList();
        //        }

        //        return libm;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public IList<CoefficienteRichiamoModel> getListCoeffIndRichiamo(decimal idRiduzioni)
        //{
        //    List<CoefficienteRichiamoModel> libm = new List<CoefficienteRichiamoModel>();
        //    try
        //    {
        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            var lib = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).OrderBy(a => a.DATAINIZIOVALIDITA).OrderBy(a => a.DATAFINEVALIDITA).ToList();

        //            libm = (from e in lib
        //                    select new CoefficienteRichiamoModel()
        //                    {
        //                        idCoefIndRichiamo = e.COEFFICIENTERICHIAMO,
        //                        dataInizioValidita = e.DATAINIZIOVALIDITA,
        //                        dataFineValidita = e.DATAFINEVALIDITA,
        //                        coefficienteRichiamo = e.COEFFICIENTERICHIAMO,
        //                        dataAggiornamento = e.DATAAGGIORNAMENTO,
        //                        annullato = e.ANNULLATO,
        //                    }).ToList();
        //        }
        //        return libm;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public bool CoeffIndRichiamoAnnullato(CoefficienteRichiamoModel ibm)
        //{
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        return db.COEFFICIENTEINDRICHIAMO.Where(a => a.IDCOEFINDRICHIAMO == ibm.idCoefIndRichiamo).First().ANNULLATO == true ? true : false;
        //    }
        //}
        //public IList<CoefficienteRichiamoModel> getListCoeffIndRichiamo(bool escludiAnnullati = false)
        //{
        //    List<CoefficienteRichiamoModel> libm = new List<CoefficienteRichiamoModel>();
        //    try
        //    {
        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            List<COEFFICIENTEINDRICHIAMO> lib = new List<COEFFICIENTEINDRICHIAMO>();
        //            if (escludiAnnullati == true)
        //                lib = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).OrderBy(a => a.DATAINIZIOVALIDITA).OrderBy(a => a.DATAFINEVALIDITA).ToList();
        //            else
        //                lib = db.COEFFICIENTEINDRICHIAMO.OrderBy(a => a.DATAINIZIOVALIDITA).OrderBy(a => a.DATAFINEVALIDITA).ToList();

        //            libm = (from e in lib
        //                    select new CoefficienteRichiamoModel()
        //                    {
        //                        idCoefIndRichiamo = e.IDCOEFINDRICHIAMO,
        //                        dataInizioValidita = e.DATAINIZIOVALIDITA,
        //                        dataFineValidita = e.DATAFINEVALIDITA,
        //                        coefficienteRichiamo = e.COEFFICIENTERICHIAMO,
        //                        dataAggiornamento = e.DATAAGGIORNAMENTO,
        //                        annullato = e.ANNULLATO,
        //                    }).ToList();
        //        }
        //        return libm;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public IList<CoefficienteRichiamoModel> getListCoeffIndRichiamo(decimal idRiduzioni, bool escludiAnnullati = false)
        //{
        //    List<CoefficienteRichiamoModel> libm = new List<CoefficienteRichiamoModel>();

        //    try
        //    {
        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            List<COEFFICIENTEINDRICHIAMO> lib = new List<COEFFICIENTEINDRICHIAMO>();
        //            if (escludiAnnullati == true)
        //                lib = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).OrderBy(a => a.DATAINIZIOVALIDITA).OrderBy(a => a.DATAFINEVALIDITA).ToList();
        //            else
        //                lib = db.COEFFICIENTEINDRICHIAMO.OrderBy(a => a.DATAINIZIOVALIDITA).OrderBy(a => a.DATAFINEVALIDITA).ToList();


        //            libm = (from e in lib
        //                    select new CoefficienteRichiamoModel()
        //                    {
        //                        idCoefIndRichiamo = e.IDCOEFINDRICHIAMO,
        //                        dataInizioValidita = e.DATAINIZIOVALIDITA,
        //                        dataFineValidita = e.DATAFINEVALIDITA,
        //                        coefficienteRichiamo = e.COEFFICIENTERICHIAMO,
        //                        dataAggiornamento = e.DATAAGGIORNAMENTO,
        //                        annullato = e.ANNULLATO,
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
        //public void SetListCoeffIndRichiamo(CoefficienteRichiamoModel ibm)
        //{
        //    List<COEFFICIENTEINDRICHIAMO> libNew = new List<COEFFICIENTEINDRICHIAMO>();
        //    COEFFICIENTEINDRICHIAMO ibNew = new COEFFICIENTEINDRICHIAMO();
        //    COEFFICIENTEINDRICHIAMO ibPrecedente = new COEFFICIENTEINDRICHIAMO();
        //    List<COEFFICIENTEINDRICHIAMO> lArchivioIB = new List<COEFFICIENTEINDRICHIAMO>();

        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        try
        //        {
        //            if (ibm.dataFineValidita.HasValue)
        //            {
        //                if (EsistonoMovimentiSuccessiviUguale(ibm))
        //                {
        //                    ibNew = new COEFFICIENTEINDRICHIAMO()
        //                    {
        //                        IDCOEFINDRICHIAMO = ibm.idCoefIndRichiamo,
        //                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
        //                        DATAFINEVALIDITA = ibm.dataFineValidita.Value,
        //                        DATAAGGIORNAMENTO = ibm.dataAggiornamento,
        //                        ANNULLATO = ibm.annullato
        //                    };
        //                }
        //                else
        //                {
        //                    ibNew = new COEFFICIENTEINDRICHIAMO()
        //                    {

        //                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
        //                        DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
        //                        DATAAGGIORNAMENTO = ibm.dataAggiornamento,
        //                        ANNULLATO = ibm.annullato
        //                    };
        //                }
        //            }
        //            else
        //            {
        //                ibNew = new COEFFICIENTEINDRICHIAMO()
        //                {

        //                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
        //                    DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
        //                    DATAAGGIORNAMENTO = ibm.dataAggiornamento,
        //                    ANNULLATO = ibm.annullato
        //                };
        //            }

        //            db.Database.BeginTransaction();

        //            var recordInteressati = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false)
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
        //                            var ibOld1 = new COEFFICIENTEINDRICHIAMO()
        //                            {


        //                                DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
        //                                DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),

        //                                DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
        //                                ANNULLATO = false
        //                            };

        //                            libNew.Add(ibOld1);

        //                        }
        //                        else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
        //                        {
        //                            var ibOld1 = new COEFFICIENTEINDRICHIAMO()
        //                            {


        //                                DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
        //                                DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),

        //                                DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
        //                                ANNULLATO = false
        //                            };

        //                            var ibOld2 = new COEFFICIENTEINDRICHIAMO()
        //                            {


        //                                DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(+1),
        //                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,

        //                                DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
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
        //                            var ibOld1 = new COEFFICIENTEINDRICHIAMO()
        //                            {
        //                                DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
        //                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,

        //                                DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
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
        //                            var ibOld1 = new COEFFICIENTEINDRICHIAMO()
        //                            {


        //                                DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
        //                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,

        //                                DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
        //                                ANNULLATO = false
        //                            };

        //                            libNew.Add(ibOld1);
        //                        }
        //                    }
        //                }

        //                libNew.Add(ibNew);
        //                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

        //                db.COEFFICIENTEINDRICHIAMO.AddRange(libNew);
        //            }
        //            else
        //            {
        //                db.COEFFICIENTEINDRICHIAMO.Add(ibNew);

        //            }
        //            db.SaveChanges();

        //            using (objLogAttivita log = new objLogAttivita())
        //            {
        //                log.Log(enumAttivita.Inserimento, "Inserimento parametro di Coeff. Ind. Richiamo", "COEFFICIENTEINDRICHIAMO", ibNew.IDCOEFINDRICHIAMO);
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

        //public bool EsistonoMovimentiPrima(CoefficienteRichiamoModel ibm)
        //{
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        return db.COEFFICIENTEINDRICHIAMO.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita).Count() > 0 ? true : false;
        //    }
        //}

        //public bool EsistonoMovimentiSuccessivi(CoefficienteRichiamoModel ibm)
        //{
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        if (ibm.dataFineValidita.HasValue)
        //        {
        //            return db.COEFFICIENTEINDRICHIAMO.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value).Count() > 0 ? true : false;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}

        //public bool EsistonoMovimentiSuccessiviUguale(CoefficienteRichiamoModel ibm)
        //{
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        if (ibm.dataFineValidita.HasValue)
        //        {
        //            return db.COEFFICIENTEINDRICHIAMO.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value).Count() > 0 ? true : false;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}

        //public bool EsistonoMovimentiPrimaUguale(CoefficienteRichiamoModel ibm)
        //{
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        return db.COEFFICIENTEINDRICHIAMO.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita).Count() > 0 ? true : false;
        //    }
        //}

        //public void DelListCoeffIndRichiamo(decimal idCoeffIndRichiamo)
        //{
        //    COEFFICIENTEINDRICHIAMO precedenteIB = new COEFFICIENTEINDRICHIAMO();
        //    COEFFICIENTEINDRICHIAMO delIB = new COEFFICIENTEINDRICHIAMO();


        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        try
        //        {
        //            db.Database.BeginTransaction();

        //            var lib = db.COEFFICIENTEINDRICHIAMO.Where(a => a.IDCOEFINDRICHIAMO == idCoeffIndRichiamo);

        //            if (lib.Count() > 0)
        //            {
        //                delIB = lib.First();
        //                delIB.ANNULLATO = true;

        //                var lprecIB = db.COEFFICIENTEINDRICHIAMO.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

        //                if (lprecIB.Count > 0)
        //                {
        //                    precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
        //                    precedenteIB.ANNULLATO = true;

        //                    var ibOld1 = new COEFFICIENTEINDRICHIAMO()
        //                    {
        //                        IDCOEFINDRICHIAMO = precedenteIB.IDCOEFINDRICHIAMO,
        //                        DATAINIZIOVALIDITA = precedenteIB.DATAFINEVALIDITA,
        //                        DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
        //                        COEFFICIENTERICHIAMO = precedenteIB.COEFFICIENTERICHIAMO,
        //                        ANNULLATO = false
        //                    };

        //                    db.COEFFICIENTEINDRICHIAMO.Add(ibOld1);
        //                }

        //                db.SaveChanges();

        //                using (objLogAttivita log = new objLogAttivita())
        //                {
        //                    log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di coeff. Ind. Richiamo", "COEFFICIENTEINDRICHIAMO", idCoeffIndRichiamo);
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
        public static DateTime DataInizioMinimaNonAnnullataCoeffIndRichiamo()
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }

        public decimal Get_Id_CoeffIndRichiamoPrimoNonAnnullato(decimal idTipoCoeffindRichiamo)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTEINDRICHIAMO> libm = new List<COEFFICIENTEINDRICHIAMO>();
                libm = db.COEFFICIENTEINDRICHIAMO
                                    .Where(a => 
                                                a.ANNULLATO == false &&
                                                a.IDTIPOCOEFFICIENTERICHIAMO==idTipoCoeffindRichiamo)
                                    .OrderBy(b => b.DATAINIZIOVALIDITA)
                                    .ThenBy(c => c.DATAFINEVALIDITA)
                                    .ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDCOEFINDRICHIAMO;
            }
            return tmp;
        }


        public void DelCoefficienteRichiamo(decimal idCoefIndRichiamo)
        {
            COEFFICIENTEINDRICHIAMO precedenteIB = new COEFFICIENTEINDRICHIAMO();
            COEFFICIENTEINDRICHIAMO delIB = new COEFFICIENTEINDRICHIAMO();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    var cir = db.COEFFICIENTEINDRICHIAMO.Find(idCoefIndRichiamo);
                    if (cir.IDCOEFINDRICHIAMO > 0)
                    {
                        var idTipoCoeffRichiamo = cir.IDTIPOCOEFFICIENTERICHIAMO;
                        delIB = cir;

                        delIB.ANNULLATO = true;
                        RendiAnnullatoUnRecord(delIB.IDCOEFINDRICHIAMO, db);
                        precedenteIB = RestituisciIlRecordPrecedente(idCoefIndRichiamo);
                        RendiAnnullatoUnRecord(precedenteIB.IDCOEFINDRICHIAMO, db);

                        var NuovoPrecedente = new COEFFICIENTEINDRICHIAMO()
                        {
                            DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                            COEFFICIENTERICHIAMO = precedenteIB.COEFFICIENTERICHIAMO,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            ANNULLATO = false,
                            IDTIPOCOEFFICIENTERICHIAMO=precedenteIB.IDTIPOCOEFFICIENTERICHIAMO
                        };
                        db.COEFFICIENTEINDRICHIAMO.Add(NuovoPrecedente);

                        db.SaveChanges();

                        using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                        {
                            dtrp.AssociaRichiamo_CR(NuovoPrecedente.IDCOEFINDRICHIAMO, db,delIB.DATAINIZIOVALIDITA);
                            dtrp.AssociaRiduzioni_CR(NuovoPrecedente.IDCOEFINDRICHIAMO, db, delIB.DATAINIZIOVALIDITA);
                        }
                    }

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Eliminazione, "Eliminazione parametro coefficiente indennita di richiamo.", "COEFFICIENTEINDRICHIAMO", idCoefIndRichiamo);
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

        //public static List<RiduzioniModel> dataInizioValiditaAccettataPerRiduzione(DateTime d)
        //{
        //    List<RiduzioniModel> tmp = new List<RiduzioniModel>();
        //    using (dtRiduzioni dtRid = new dtRiduzioni())
        //    {
        //        tmp = dtRid.getListRiduzioni().Where(a => a.annullato == false).ToList().Where(b => d >= b.dataInizioValidita && d <= b.dataFineValidita).OrderBy(a => a.dataInizioValidita).ThenBy(b => b.dataFineValidita).ToList();
        //    }
        //    return tmp;
        //}
        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as CoefficienteRichiamoModel;

            if (fm != null)
            {
                DateTime d = DataInizioMinimaNonAnnullataCoeffIndRichiamo();
                if (fm.dataInizioValidita < d)
                {
                    vr = new ValidationResult(string.Format("Impossibile inserire la data di inizio validità minore alla data di Base ({0}).", d.ToShortDateString()));
                }
                else
                {
                    //if(dataInizioValiditaAccettataPerRiduzione(fm.dataInizioValidita).Count!=0)
                    vr = ValidationResult.Success;
                    //else
                    //    vr = new ValidationResult(string.Format("Impossibile inserire la data di inizio validità non registrata in un intervallo nella Riduzione {0}.",""));
                }
            }
            else
            {
                vr = new ValidationResult("La data di inizio validità è richiesta.");
            }
            return vr;
        }

        //public static DateTime DataInizioMinimaNonAnnullataIndennitaBase()
        //{
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        var TuttiNonAnnullati = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
        //        if (TuttiNonAnnullati.Count > 0)
        //        {
        //            return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
        //        }
        //    }
        //    return Utility.GetData_Inizio_Base();
        //}
        public COEFFICIENTEINDRICHIAMO RestituisciIlRecordPrecedente(decimal IDCOEFINDRICHIAMO)
        {
            COEFFICIENTEINDRICHIAMO tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                COEFFICIENTEINDRICHIAMO interessato = new COEFFICIENTEINDRICHIAMO();
                interessato = db.COEFFICIENTEINDRICHIAMO.Find(IDCOEFINDRICHIAMO);
                tmp = db.COEFFICIENTEINDRICHIAMO
                                        .Where(a => a.ANNULLATO == false && 
                                                    a.IDTIPOCOEFFICIENTERICHIAMO==interessato.IDTIPOCOEFFICIENTERICHIAMO)
                                        .ToList()
                                        .Where(b => b.DATAFINEVALIDITA == interessato.DATAINIZIOVALIDITA.AddDays(-1))
                                        .ToList()
                                        .First();
            }
            return tmp;
        }
        //public List<string> RestituisciIntervalloDiUnaData(DateTime DataCampione)
        //{
        //    List<string> tmp = new List<string>();
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        List<COEFFICIENTEINDRICHIAMO> libm = new List<COEFFICIENTEINDRICHIAMO>();
        //        libm = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).ToList().Where(b =>
        //        b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())
        //        && DataCampione > b.DATAINIZIOVALIDITA
        //        && DataCampione < b.DATAFINEVALIDITA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
        //        if (libm.Count != 0)
        //        {
        //            tmp.Add(libm[0].IDCOEFINDRICHIAMO.ToString());
        //            tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
        //            tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
        //            tmp.Add(libm[0].COEFFICIENTERICHIAMO.ToString());
        //        }
        //    }
        //    return tmp;
        //}
        //public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione)
        //{
        //    List<string> tmp = new List<string>();
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        List<COEFFICIENTEINDRICHIAMO> libm = new List<COEFFICIENTEINDRICHIAMO>();
        //        libm = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().Where(b => DataCampione == b.DATAINIZIOVALIDITA &&
        //         b.DATAFINEVALIDITA != Utility.DataFineStop()).ToList();
        //        if (libm.Count != 0)
        //        {
        //            tmp.Add(libm[0].IDCOEFINDRICHIAMO.ToString());
        //            tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
        //            tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
        //            tmp.Add(libm[0].COEFFICIENTERICHIAMO.ToString());
        //        }
        //    }
        //    return tmp;
        //}
        //public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione)
        //{
        //    List<string> tmp = new List<string>();
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        List<COEFFICIENTEINDRICHIAMO> libm = new List<COEFFICIENTEINDRICHIAMO>();
        //        libm = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().
        //        Where(b => DataCampione == b.DATAFINEVALIDITA
        //        && b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())).ToList();

        //        if (libm.Count != 0)
        //        {
        //            tmp.Add(libm[0].IDCOEFINDRICHIAMO.ToString());
        //            tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
        //            tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
        //            tmp.Add(libm[0].COEFFICIENTERICHIAMO.ToString());
        //        }
        //    }
        //    return tmp;
        //}
        //public List<string> RestituisciLaRigaMassima()
        //{
        //    List<string> tmp = new List<string>();
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        List<COEFFICIENTEINDRICHIAMO> libm = new List<COEFFICIENTEINDRICHIAMO>();
        //        libm = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).ToList().Where(b =>
        //        b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop())).ToList();
        //        if (libm.Count != 0)
        //        {
        //            tmp.Add(libm[0].IDCOEFINDRICHIAMO.ToString());
        //            tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
        //            tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
        //            tmp.Add(libm[0].COEFFICIENTERICHIAMO.ToString());
        //        }
        //    }
        //    return tmp;
        //}
        public void RendiAnnullatoUnRecord(decimal IDCOEFINDRICHIAMO, ModelDBISE db)
        {
            COEFFICIENTEINDRICHIAMO entita = new COEFFICIENTEINDRICHIAMO();
            entita = db.COEFFICIENTEINDRICHIAMO.Find(IDCOEFINDRICHIAMO);
            entita.ANNULLATO = true;
            db.SaveChanges();
        }
        //public decimal Get_Id_CoefficienteIndRichiamoNonAnnullato(decimal IDCOEFINDRICHIAMO)
        //{
        //    decimal tmp = 0;
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        List<COEFFICIENTEINDRICHIAMO> libm = new List<COEFFICIENTEINDRICHIAMO>();
        //        libm = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false
        //        && a.IDCOEFINDRICHIAMO == IDCOEFINDRICHIAMO).OrderBy(a => a.DATAINIZIOVALIDITA).ThenBy(a => a.DATAFINEVALIDITA).ToList();
        //        if (libm.Count != 0)
        //            tmp = libm.First().IDCOEFINDRICHIAMO;
        //    }
        //    return tmp;
        //}
        public void SetCoefficienteRichiamo(CoefficienteRichiamoModel ibm, decimal idTipoCoeffRichiamo, bool aggiornaTutto)
        {
            List<COEFFICIENTEINDRICHIAMO> libNew = new List<COEFFICIENTEINDRICHIAMO>();

            COEFFICIENTEINDRICHIAMO ibNew1 = new COEFFICIENTEINDRICHIAMO();
            COEFFICIENTEINDRICHIAMO ibNew2 = new COEFFICIENTEINDRICHIAMO();

            List<string> lista = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                bool giafatta = false;

                try
                {
                    using (dtCoefficienteRichiamo dtal = new dtCoefficienteRichiamo())
                    {
                        //Se la data variazione coincide con una data inizio esistente
                        lista = dtal.DataVariazioneCoincideConDataInizio(ibm.dataInizioValidita, idTipoCoeffRichiamo);
                        if (lista.Count != 0)
                        {
                            giafatta = true;
                            decimal idIntervalloFirst = Convert.ToDecimal(lista[0]);
                            DateTime dataInizioFirst = Convert.ToDateTime(lista[1]);
                            DateTime dataFineFirst = Convert.ToDateTime(lista[2]);

                            ibNew1 = new COEFFICIENTEINDRICHIAMO()
                            {
                                DATAINIZIOVALIDITA = dataInizioFirst,
                                DATAFINEVALIDITA = dataFineFirst,
                                COEFFICIENTERICHIAMO = ibm.coefficienteRichiamo,
                                IDTIPOCOEFFICIENTERICHIAMO=idTipoCoeffRichiamo,
                                DATAAGGIORNAMENTO = DateTime.Now,
                                ANNULLATO=false
                            };

                            if (aggiornaTutto)
                            {
                                ibNew1 = new COEFFICIENTEINDRICHIAMO()
                                {
                                    DATAINIZIOVALIDITA = dataInizioFirst,
                                    DATAFINEVALIDITA = Utility.DataFineStop(),
                                    COEFFICIENTERICHIAMO = ibm.coefficienteRichiamo,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    ANNULLATO=false,
                                    IDTIPOCOEFFICIENTERICHIAMO=idTipoCoeffRichiamo
                                };
                                //qui annullo tutti i record rimanenti dalla data inizio inserita
                                libNew = db.COEFFICIENTEINDRICHIAMO
                                                        .Where(a => 
                                                                    a.ANNULLATO == false &&
                                                                    a.IDTIPOCOEFFICIENTERICHIAMO==idTipoCoeffRichiamo)
                                                        .ToList()
                                                        .Where(a => a.DATAINIZIOVALIDITA > dataInizioFirst)
                                                        .ToList();

                                foreach (var elem in libNew)
                                {
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDCOEFINDRICHIAMO), db);
                                }
                            }
                            db.Database.BeginTransaction();
                            db.COEFFICIENTEINDRICHIAMO.Add(ibNew1);
                            db.SaveChanges();
                            RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloFirst), db);

                            using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                            {
                                dtrp.AssociaRichiamo_CR(ibNew1.IDCOEFINDRICHIAMO, db, ibm.dataInizioValidita);
                                dtrp.AssociaRiduzioni_CR(ibNew1.IDCOEFINDRICHIAMO, db, ibm.dataInizioValidita);
                            }

                            db.Database.CurrentTransaction.Commit();
                        }
                        ///se la data variazione coincide con una data fine esistente(diversa da 31/12/9999)
                        if (giafatta == false)
                        {
                            lista = dtal.DataVariazioneCoincideConDataFine(ibm.dataInizioValidita, idTipoCoeffRichiamo);
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervalloLast = Convert.ToDecimal(lista[0]);
                                DateTime dataInizioLast = Convert.ToDateTime(lista[1]);
                                DateTime dataFineLast = Convert.ToDateTime(lista[2]);
                                decimal aliquotaLast = Convert.ToDecimal(lista[3]);

                                decimal COEFFICIENTERICHIAMO_last = Convert.ToDecimal(lista[3]);
                                decimal COEFFICIENTEINDBASE_last = Convert.ToDecimal(lista[4]);

                                ibNew1 = new COEFFICIENTEINDRICHIAMO()
                                {
                                    DATAINIZIOVALIDITA = dataInizioLast,
                                    DATAFINEVALIDITA = dataFineLast.AddDays(-1),
                                    COEFFICIENTERICHIAMO = COEFFICIENTERICHIAMO_last,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    IDTIPOCOEFFICIENTERICHIAMO=idTipoCoeffRichiamo
                                };
                                ibNew2 = new COEFFICIENTEINDRICHIAMO()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = ibm.dataInizioValidita,//è uguale alla data Inizio
                                    COEFFICIENTERICHIAMO = ibm.coefficienteRichiamo,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    IDTIPOCOEFFICIENTERICHIAMO=idTipoCoeffRichiamo
                                };
                                if (aggiornaTutto)
                                {
                                    ibNew2 = new COEFFICIENTEINDRICHIAMO()
                                    {
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        // COEFFICIENTEKM = ibm.coefficienteKm,
                                        COEFFICIENTERICHIAMO = ibm.coefficienteRichiamo,
                                        //COEFFICIENTEINDBASE = ibm.coefficienteIndBase,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        IDTIPOCOEFFICIENTERICHIAMO=idTipoCoeffRichiamo
                                    };
                                    libNew = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false && a.IDTIPOCOEFFICIENTERICHIAMO==idTipoCoeffRichiamo).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDCOEFINDRICHIAMO), db);
                                    }
                                }

                                libNew.Add(ibNew1);
                                libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                db.Database.BeginTransaction();
                                db.COEFFICIENTEINDRICHIAMO.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloLast), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var cr in libNew)
                                    {
                                        dtrp.AssociaRichiamo_CR(cr.IDCOEFINDRICHIAMO, db, ibm.dataInizioValidita);
                                        dtrp.AssociaRiduzioni_CR(cr.IDCOEFINDRICHIAMO, db, ibm.dataInizioValidita);
                                    }

                                }

                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //Se il nuovo record si trova in un intervallo non annullato con data fine non uguale al 31/12/9999
                        if (giafatta == false)
                        {
                            lista = dtal.RestituisciIntervalloDiUnaData(ibm.dataInizioValidita, idTipoCoeffRichiamo);
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervallo = Convert.ToDecimal(lista[0]);
                                DateTime dataInizio = Convert.ToDateTime(lista[1]);
                                DateTime dataFine = Convert.ToDateTime(lista[2]);
                                decimal COEFFICIENTERICHIAMO = Convert.ToDecimal(lista[3]);
                                DateTime NewdataFine1 = ibm.dataInizioValidita.AddDays(-1);

                                ibNew1 = new COEFFICIENTEINDRICHIAMO()
                                {
                                    DATAINIZIOVALIDITA = dataInizio,
                                    DATAFINEVALIDITA = NewdataFine1,
                                    COEFFICIENTERICHIAMO = COEFFICIENTERICHIAMO,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    IDTIPOCOEFFICIENTERICHIAMO=idTipoCoeffRichiamo,
                                    ANNULLATO=false
                                };
                                ibNew2 = new COEFFICIENTEINDRICHIAMO()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = dataFine,
                                    COEFFICIENTERICHIAMO = ibm.coefficienteRichiamo,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    IDTIPOCOEFFICIENTERICHIAMO=idTipoCoeffRichiamo
                                };

                                if (aggiornaTutto)
                                {
                                    ibNew2 = new COEFFICIENTEINDRICHIAMO()
                                    {
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        COEFFICIENTERICHIAMO = ibm.coefficienteRichiamo,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        IDTIPOCOEFFICIENTERICHIAMO=idTipoCoeffRichiamo
                                    };
                                    libNew = db.COEFFICIENTEINDRICHIAMO
                                                        .Where(a => 
                                                                    a.ANNULLATO == false &&
                                                                    a.IDTIPOCOEFFICIENTERICHIAMO==idTipoCoeffRichiamo)
                                                        .ToList()
                                                        .Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita)
                                                        .ToList();

                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDCOEFINDRICHIAMO), db);
                                    }
                                }
                                libNew.Add(ibNew1);
                                libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                db.Database.BeginTransaction();
                                db.COEFFICIENTEINDRICHIAMO.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervallo), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var cr in libNew)
                                    {
                                        dtrp.AssociaRichiamo_CR(cr.IDCOEFINDRICHIAMO, db, ibm.dataInizioValidita);
                                        dtrp.AssociaRiduzioni_CR(cr.IDCOEFINDRICHIAMO, db, ibm.dataInizioValidita);
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
                            lista = dtal.RestituisciLaRigaMassima(idTipoCoeffRichiamo);
                            if (lista.Count == 0)
                            {
                                ibNew1 = new COEFFICIENTEINDRICHIAMO()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = Convert.ToDateTime(Utility.DataFineStop()),
                                    COEFFICIENTERICHIAMO = ibm.coefficienteRichiamo,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    ANNULLATO=false,
                                    IDTIPOCOEFFICIENTERICHIAMO=idTipoCoeffRichiamo
                                };
                                libNew.Add(ibNew1);
                                db.Database.BeginTransaction();
                                db.COEFFICIENTEINDRICHIAMO.Add(ibNew1);
                                db.SaveChanges();
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
                                decimal COEFFICIENTERICHIAMO_Ultimo = Convert.ToDecimal(lista[3]);
                                decimal COEFFICIENTEINDBASE_Ultimo = Convert.ToDecimal(lista[4]);

                                if (dataInizioUltimo == ibm.dataInizioValidita)
                                {
                                    ibNew1 = new COEFFICIENTEINDRICHIAMO()
                                    {
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = dataFineUltimo,
                                        COEFFICIENTERICHIAMO = ibm.coefficienteRichiamo,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        IDTIPOCOEFFICIENTERICHIAMO=idTipoCoeffRichiamo,
                                        ANNULLATO=false
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.COEFFICIENTEINDRICHIAMO.Add(ibNew1);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        dtrp.AssociaRichiamo_CR(ibNew1.IDCOEFINDRICHIAMO, db, ibm.dataInizioValidita);
                                        dtrp.AssociaRiduzioni_CR(ibNew1.IDCOEFINDRICHIAMO, db, ibm.dataInizioValidita);
                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                                //se il nuovo record rappresenta la data variazione superiore alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                if (ibm.dataInizioValidita > dataInizioUltimo)
                                {
                                    ibNew1 = new COEFFICIENTEINDRICHIAMO()
                                    {
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = ibm.dataInizioValidita.AddDays(-1),
                                        COEFFICIENTERICHIAMO = COEFFICIENTERICHIAMO_Ultimo,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        IDTIPOCOEFFICIENTERICHIAMO=idTipoCoeffRichiamo,
                                        ANNULLATO=false
                                    };
                                    ibNew2 = new COEFFICIENTEINDRICHIAMO()
                                    {
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        // COEFFICIENTEKM = ibm.coefficienteKm,//nuova aliquota rispetto alla vecchia registrata
                                        COEFFICIENTERICHIAMO = ibm.coefficienteRichiamo,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        IDTIPOCOEFFICIENTERICHIAMO=idTipoCoeffRichiamo
                                    };
                                    libNew.Add(ibNew1);
                                    libNew.Add(ibNew2);
                                    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                    db.Database.BeginTransaction();
                                    db.COEFFICIENTEINDRICHIAMO.AddRange(libNew);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        foreach (var cr in libNew)
                                        {
                                            dtrp.AssociaRichiamo_CR(cr.IDCOEFINDRICHIAMO, db, ibm.dataInizioValidita);
                                            dtrp.AssociaRiduzioni_CR(cr.IDCOEFINDRICHIAMO, db, ibm.dataInizioValidita);
                                        }

                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                            }
                        }

                        //TODO: Verificare con RICK l'utilità di questo codice.
                        //INSERIMENTO DATI NELLA TABELLA CIR_R PER LE RELAZIONI MOLTI A MOLTI
                        //decimal idCoeffIndRichiamo1 = ibNew1.IDCOEFINDRICHIAMO;
                        //decimal idRiduzione1 = dataInizioValiditaAccettataPerRiduzione(ibNew1.DATAINIZIOVALIDITA).First().idRiduzioni;

                        //this.Associa_Riduzione_CoeffIndRichiamo(idRiduzione1, idCoeffIndRichiamo1, db);

                        //if (ibNew2.IDCOEFINDRICHIAMO != 0)
                        //{
                        //    decimal idCoeffIndRichiamo2 = ibNew2.IDCOEFINDRICHIAMO;
                        //    decimal idRiduzione2 = dataInizioValiditaAccettataPerRiduzione(ibNew2.DATAINIZIOVALIDITA).First().idRiduzioni;
                        //    this.Associa_Riduzione_CoeffIndRichiamo(idRiduzione2, idCoeffIndRichiamo2, db);
                        //}
                    }
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }
        //decimal RestituisciIdRecordNuovo(DateTime d1, DateTime d2)
        //{
        //    decimal tmp = 0;

        //    return tmp;
        //}
        //public void Associa_Riduzione_CoeffIndRichiamo(decimal idRiduzioni, decimal idCoeffIndRichiamo, ModelDBISE db)
        //{
        //    try
        //    {
        //        var r = db.RIDUZIONI.Find(idRiduzioni);
        //        var item = db.Entry<RIDUZIONI>(r);
        //        item.State = System.Data.Entity.EntityState.Modified;
        //        //item.Collection(a => a.DOCUMENTI).Load();
        //        var c = db.COEFFICIENTEINDRICHIAMO.Find(idCoeffIndRichiamo);
        //        r.COEFFICIENTEINDRICHIAMO.Add(c);
        //        int i = db.SaveChanges();
        //        if (i <= 0)
        //        {
        //            throw new Exception(string.Format("Impossibile associare Riduzioni al Coeffiente Ind Richiamo."));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public IList<CoefficienteRichiamoModel> GetListCoeffRichiamo_x_Tipo(decimal idTipoCoeffRichiamo, bool escludiAnnullati = false)
        {
            List<CoefficienteRichiamoModel> lcrm = new List<CoefficienteRichiamoModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<COEFFICIENTEINDRICHIAMO> lcr = new List<COEFFICIENTEINDRICHIAMO>();
                    if (escludiAnnullati)
                        lcr = db.COEFFICIENTEINDRICHIAMO
                                                    .Where(a => a.ANNULLATO == false &&
                                                                a.IDTIPOCOEFFICIENTERICHIAMO==idTipoCoeffRichiamo)
                                                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                    .OrderBy(a => a.DATAFINEVALIDITA)
                                                    .ToList();
                    else
                        lcr = db.COEFFICIENTEINDRICHIAMO
                                                .Where(a=>a.IDTIPOCOEFFICIENTERICHIAMO == idTipoCoeffRichiamo)
                                                .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                .OrderBy(a => a.DATAFINEVALIDITA)
                                                .ToList();
                            

                    lcrm = (from e in lcr
                            select new CoefficienteRichiamoModel()
                            {
                                idCoefIndRichiamo = e.IDCOEFINDRICHIAMO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                coefficienteRichiamo = e.COEFFICIENTERICHIAMO,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                            }).ToList();
                }
                return lcrm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<TipoCoefficienteRichiamoModel> ListTipoCoeffIndRichiamo()
        {
            List<TipoCoefficienteRichiamoModel> ltcrm = new List<TipoCoefficienteRichiamoModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ltcr = db.TIPOCOEFFICIENTERICHIAMO.OrderBy(a => a.DESCRIZIONE).ToList();

                    ltcrm = (from e in ltcr
                            select new TipoCoefficienteRichiamoModel()
                            {
                                idTipoCoefficienteRichiamo=e.IDTIPOCOEFFICIENTERICHIAMO,
                                descrizione=e.DESCRIZIONE
                            }).ToList();
                }

                return ltcrm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TIPOCOEFFICIENTERICHIAMO GetTipoCoeffRichiamo(decimal idTipoCoeffIndRichiamo)
        {
            TIPOCOEFFICIENTERICHIAMO tcir = new TIPOCOEFFICIENTERICHIAMO();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    tcir = db.TIPOCOEFFICIENTERICHIAMO.Find(idTipoCoeffIndRichiamo);
                    if(!(tcir.IDTIPOCOEFFICIENTERICHIAMO>0))
                    {
                        throw new Exception("Tipo Coefficiente di Richiamo non trovato.");
                    }
                }
                        

                return tcir;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.Models.Tools;
using System.ComponentModel.DataAnnotations;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParCoeffIndRichiamo : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<CoefficienteRichiamoModel> getListCoeffIndRichiamo()
        {
            List<CoefficienteRichiamoModel> libm = new List<CoefficienteRichiamoModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.COEFFICIENTEINDRICHIAMO.ToList();

                    libm = (from e in lib
                            select new CoefficienteRichiamoModel()
                            {
                                idCoefIndRichiamo = e.COEFFICIENTERICHIAMO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                               // dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new CoefficienteRichiamoModel().dataFineValidita,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                coefficienteRichiamo = e.COEFFICIENTERICHIAMO,
                                coefficienteIndBase = e.COEFFICIENTEINDBASE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<CoefficienteRichiamoModel> getListCoeffIndRichiamo(decimal idRiduzioni)
        {
            List<CoefficienteRichiamoModel> libm = new List<CoefficienteRichiamoModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).ToList();

                    libm = (from e in lib
                            select new CoefficienteRichiamoModel()
                            {
                                idCoefIndRichiamo = e.COEFFICIENTERICHIAMO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                //dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new CoefficienteRichiamoModel().dataFineValidita,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                coefficienteRichiamo = e.COEFFICIENTERICHIAMO,
                                coefficienteIndBase = e.COEFFICIENTEINDBASE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                            }).ToList();
                }
                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool CoeffIndRichiamoAnnullato(CoefficienteRichiamoModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.COEFFICIENTEINDRICHIAMO.Where(a => a.IDCOEFINDRICHIAMO == ibm.idCoefIndRichiamo).First().ANNULLATO == true ? true : false;
            }
        }
        public IList<CoefficienteRichiamoModel> getListCoeffIndRichiamo(bool escludiAnnullati = false)
        {
            List<CoefficienteRichiamoModel> libm = new List<CoefficienteRichiamoModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<COEFFICIENTEINDRICHIAMO> lib = new List<COEFFICIENTEINDRICHIAMO>();
                    if (escludiAnnullati == true)
                        lib = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).ToList();
                    else
                        lib = db.COEFFICIENTEINDRICHIAMO.ToList();

                    libm = (from e in lib
                            select new CoefficienteRichiamoModel()
                            {
                                idCoefIndRichiamo = e.IDCOEFINDRICHIAMO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                coefficienteRichiamo = e.COEFFICIENTERICHIAMO,
                                coefficienteIndBase = e.COEFFICIENTEINDBASE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                            }).ToList();
            }
                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<CoefficienteRichiamoModel> getListCoeffIndRichiamo(decimal idRiduzioni, bool escludiAnnullati = false)
        {
            List<CoefficienteRichiamoModel> libm = new List<CoefficienteRichiamoModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<COEFFICIENTEINDRICHIAMO> lib = new List<COEFFICIENTEINDRICHIAMO>();
                    if (escludiAnnullati == true)
                        lib = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).ToList();
                    else
                        lib = db.COEFFICIENTEINDRICHIAMO.ToList();


                    libm = (from e in lib
                            select new CoefficienteRichiamoModel()
                            {
                                idCoefIndRichiamo = e.IDCOEFINDRICHIAMO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                // dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new CoefficienteRichiamoModel().dataFineValidita,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                coefficienteRichiamo = e.COEFFICIENTERICHIAMO,
                                coefficienteIndBase = e.COEFFICIENTEINDBASE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
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
        public void SetListCoeffIndRichiamo(CoefficienteRichiamoModel ibm)
        {
            List<COEFFICIENTEINDRICHIAMO> libNew = new List<COEFFICIENTEINDRICHIAMO>();

            COEFFICIENTEINDRICHIAMO ibNew = new COEFFICIENTEINDRICHIAMO();

            COEFFICIENTEINDRICHIAMO ibPrecedente = new COEFFICIENTEINDRICHIAMO();

            List<COEFFICIENTEINDRICHIAMO> lArchivioIB = new List<COEFFICIENTEINDRICHIAMO>();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    if (ibm.dataFineValidita.HasValue)
                    {
                        if (EsistonoMovimentiSuccessiviUguale(ibm))
                        {
                            ibNew = new COEFFICIENTEINDRICHIAMO()
                            {
                                IDCOEFINDRICHIAMO = ibm.idCoefIndRichiamo,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                                ANNULLATO = ibm.annullato
                            };
                        }
                        else
                        {
                            ibNew = new COEFFICIENTEINDRICHIAMO()
                            {

                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                                DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                                ANNULLATO = ibm.annullato
                            };
                        }
                    }
                    else
                    {
                        ibNew = new COEFFICIENTEINDRICHIAMO()
                        {

                            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                            DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                            ANNULLATO = ibm.annullato
                        };
                    }

                    db.Database.BeginTransaction();

                    var recordInteressati = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false)
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
                                    var ibOld1 = new COEFFICIENTEINDRICHIAMO()
                                    {


                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),

                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);

                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new COEFFICIENTEINDRICHIAMO()
                                    {


                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),

                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    var ibOld2 = new COEFFICIENTEINDRICHIAMO()
                                    {


                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(+1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,

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
                                    var ibOld1 = new COEFFICIENTEINDRICHIAMO()
                                    {


                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,

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
                                    var ibOld1 = new COEFFICIENTEINDRICHIAMO()
                                    {


                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,

                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                }
                            }
                        }

                        libNew.Add(ibNew);
                        libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        db.COEFFICIENTEINDRICHIAMO.AddRange(libNew);
                    }
                    else
                    {
                        db.COEFFICIENTEINDRICHIAMO.Add(ibNew);

                    }
                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro di Coeff. Ind. Richiamo", "COEFFICIENTEINDRICHIAMO", ibNew.IDCOEFINDRICHIAMO);
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

        public bool EsistonoMovimentiPrima(CoefficienteRichiamoModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.COEFFICIENTEINDRICHIAMO.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(CoefficienteRichiamoModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.COEFFICIENTEINDRICHIAMO.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(CoefficienteRichiamoModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.COEFFICIENTEINDRICHIAMO.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiPrimaUguale(CoefficienteRichiamoModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.COEFFICIENTEINDRICHIAMO.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita).Count() > 0 ? true : false;
            }
        }

        public void DelListCoeffIndRichiamo(decimal idCoeffIndRichiamo)
        {
            COEFFICIENTEINDRICHIAMO precedenteIB = new COEFFICIENTEINDRICHIAMO();
            COEFFICIENTEINDRICHIAMO delIB = new COEFFICIENTEINDRICHIAMO();


            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.COEFFICIENTEINDRICHIAMO.Where(a => a.IDCOEFINDRICHIAMO == idCoeffIndRichiamo);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.COEFFICIENTEINDRICHIAMO.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new COEFFICIENTEINDRICHIAMO()
                            {
                                IDCOEFINDRICHIAMO = precedenteIB.IDCOEFINDRICHIAMO,
                                DATAINIZIOVALIDITA = precedenteIB.DATAFINEVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                COEFFICIENTERICHIAMO = precedenteIB.COEFFICIENTERICHIAMO,
                                COEFFICIENTEINDBASE = precedenteIB.COEFFICIENTEINDBASE,
                                ANNULLATO = false
                            };

                            db.COEFFICIENTEINDRICHIAMO.Add(ibOld1);
                        }

                        db.SaveChanges();

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di coeff. Ind. Richiamo", "COEFFICIENTEINDRICHIAMO", idCoeffIndRichiamo);
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
        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as CoefficienteRichiamoModel;
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
        
        public decimal Get_Id_CoeffIndRichiamoPrimoNonAnnullato()
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTEINDRICHIAMO> libm = new List<COEFFICIENTEINDRICHIAMO>();
                libm = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDCOEFINDRICHIAMO;
            }
            return tmp;
        }

    }
}
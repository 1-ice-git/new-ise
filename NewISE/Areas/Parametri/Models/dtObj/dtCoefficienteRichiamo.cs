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
    public class dtCoefficienteRichiamo : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool CoeffIndRichiamoAnnullato(CoefficienteRichiamoModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.COEFFICIENTEINDRICHIAMO.Where(a => a.IDCOEFINDRICHIAMO == ibm.idCoefIndRichiamo).First().ANNULLATO == true ? true : false;
            }
        }

        public IList<CoefficienteRichiamoModel> getListCoefficienteRichiamo()
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
                                idCoefIndRichiamo = e.IDCOEFINDRICHIAMO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,// e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new CoefficienteRichiamoModel().dataFineValidita,
                                coefficienteRichiamo = e.COEFFICIENTERICHIAMO,
                                //coefficienteIndBase = e.COEFFICIENTEINDBASE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO
                            }).ToList();
                }
                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<CoefficienteRichiamoModel> getListCoefficienteRichiamo(decimal idCoefIndRichiamo)
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
                                idCoefIndRichiamo = e.IDCOEFINDRICHIAMO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                coefficienteRichiamo = e.COEFFICIENTERICHIAMO,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<CoefficienteRichiamoModel> getListCoefficienteRichiamo(bool escludiAnnullati = false)
        {
            List<CoefficienteRichiamoModel> libm = new List<CoefficienteRichiamoModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new CoefficienteRichiamoModel()
                            {
                                idCoefIndRichiamo = e.IDCOEFINDRICHIAMO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                coefficienteRichiamo = e.COEFFICIENTERICHIAMO,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<CoefficienteRichiamoModel> getListCoefficienteRichiamo(decimal idCoefIndRichiamo, bool escludiAnnullati = false)
        {
            List<CoefficienteRichiamoModel> libm = new List<CoefficienteRichiamoModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var lib = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new CoefficienteRichiamoModel()
                            {
                                idCoefIndRichiamo = e.IDCOEFINDRICHIAMO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                coefficienteRichiamo = e.COEFFICIENTERICHIAMO,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO
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



        public bool EsistonoMovimentiPrima(CoefficienteRichiamoModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.COEFFICIENTEINDRICHIAMO.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDCOEFINDRICHIAMO == ibm.idCoefIndRichiamo).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(CoefficienteRichiamoModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.COEFFICIENTEINDRICHIAMO.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDCOEFINDRICHIAMO == ibm.idCoefIndRichiamo).Count() > 0 ? true : false;
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
                    return db.COEFFICIENTEINDRICHIAMO.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDCOEFINDRICHIAMO == ibm.idCoefIndRichiamo).Count() > 0 ? true : false;
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
                return db.COEFFICIENTEINDRICHIAMO.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDCOEFINDRICHIAMO == ibm.idCoefIndRichiamo).Count() > 0 ? true : false;
            }
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
                    var lib = db.COEFFICIENTEINDRICHIAMO.Where(a => a.IDCOEFINDRICHIAMO == idCoefIndRichiamo);
                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;
                        RendiAnnullatoUnRecord(delIB.IDCOEFINDRICHIAMO, db);
                        precedenteIB = RestituisciIlRecordPrecedente(idCoefIndRichiamo);
                        RendiAnnullatoUnRecord(precedenteIB.IDCOEFINDRICHIAMO, db);

                        var NuovoPrecedente = new COEFFICIENTEINDRICHIAMO()
                        {
                            DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                            // ALIQUOTA = precedenteIB.ALIQUOTA,
                            COEFFICIENTERICHIAMO = precedenteIB.COEFFICIENTERICHIAMO,
                            DATAAGGIORNAMENTO = DateTime.Now,// precedenteIB.DATAAGGIORNAMENTO,
                            ANNULLATO = false
                        };
                        db.COEFFICIENTEINDRICHIAMO.Add(NuovoPrecedente);
                    }
                    db.SaveChanges();
                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di aliquote contributive.", "COEFFICIENTEINDRICHIAMO", idCoefIndRichiamo);
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
        public static List<RiduzioniModel> dataInizioValiditaAccettataPerRiduzione(DateTime d)
        {
            List<RiduzioniModel> tmp = new List<RiduzioniModel>();
            using (dtRiduzioni dtRid = new dtRiduzioni())
            {
                tmp = dtRid.getListRiduzioni().Where(a => a.annullato == false).ToList().Where(b => d >= b.dataInizioValidita && d <= b.dataFineValidita).OrderBy(a => a.dataInizioValidita).ThenBy(b => b.dataFineValidita).ToList();
            }
            return tmp;
        }
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
                    vr = ValidationResult.Success;
                }
            }
            else
            {
                vr = new ValidationResult("La data di inizio validità è richiesta.");
            }
            return vr;
        }

        public static DateTime DataInizioMinimaNonAnnullataIndennitaBase()
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
        public COEFFICIENTEINDRICHIAMO RestituisciIlRecordPrecedente(decimal IDCOEFINDRICHIAMO)
        {
            COEFFICIENTEINDRICHIAMO tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                COEFFICIENTEINDRICHIAMO interessato = new COEFFICIENTEINDRICHIAMO();
                interessato = db.COEFFICIENTEINDRICHIAMO.Find(IDCOEFINDRICHIAMO);
                tmp = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false).ToList().Where(b => b.DATAFINEVALIDITA == interessato.DATAINIZIOVALIDITA.AddDays(-1)).ToList().First();
            }
            return tmp;
        }
        public List<string> RestituisciIntervalloDiUnaData(DateTime DataCampione, decimal idTipoCoeffindRichiamo)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTEINDRICHIAMO> libm = new List<COEFFICIENTEINDRICHIAMO>();
                libm = db.COEFFICIENTEINDRICHIAMO
                                        .Where(a => 
                                                    a.ANNULLATO == false && 
                                                    a.IDTIPOCOEFFICIENTERICHIAMO==idTipoCoeffindRichiamo)
                                        .ToList()
                                        .Where(b =>
                                                    b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop()) && 
                                                    DataCampione > b.DATAINIZIOVALIDITA && 
                                                    DataCampione < b.DATAFINEVALIDITA)
                                        .OrderBy(b => b.DATAINIZIOVALIDITA)
                                        .ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDCOEFINDRICHIAMO.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTERICHIAMO.ToString());
                    tmp.Add(libm[0].IDTIPOCOEFFICIENTERICHIAMO.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione, decimal idTipoCoeffRichiamo)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTEINDRICHIAMO> libm = new List<COEFFICIENTEINDRICHIAMO>();
                libm = db.COEFFICIENTEINDRICHIAMO
                                    .Where(a => 
                                                a.ANNULLATO == false && 
                                                a.IDTIPOCOEFFICIENTERICHIAMO==idTipoCoeffRichiamo)
                                    .OrderBy(b => b.DATAINIZIOVALIDITA)
                                    .ToList()
                                    .Where(b => 
                                                DataCampione == b.DATAINIZIOVALIDITA &&
                                                b.DATAFINEVALIDITA != Utility.DataFineStop())
                                    .ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDCOEFINDRICHIAMO.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTERICHIAMO.ToString());
                    tmp.Add(libm[0].IDTIPOCOEFFICIENTERICHIAMO.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione, decimal idTipoCoeffRichiamo)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTEINDRICHIAMO> libm = new List<COEFFICIENTEINDRICHIAMO>();
                libm = db.COEFFICIENTEINDRICHIAMO
                                            .Where(a => 
                                                        a.ANNULLATO == false && 
                                                        a.IDTIPOCOEFFICIENTERICHIAMO==idTipoCoeffRichiamo)
                                            .OrderBy(b => b.DATAINIZIOVALIDITA)
                                            .ToList()
                                            .Where(b => 
                                                        DataCampione == b.DATAFINEVALIDITA && 
                                                        b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop()))
                                            .ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDCOEFINDRICHIAMO.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTERICHIAMO.ToString());
                    tmp.Add(libm[0].IDTIPOCOEFFICIENTERICHIAMO.ToString());
                }
            }
            return tmp;
        }
        public List<string> RestituisciLaRigaMassima(decimal idTipoCoeffIndRichiamo)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTEINDRICHIAMO> libm = new List<COEFFICIENTEINDRICHIAMO>();
                libm = db.COEFFICIENTEINDRICHIAMO
                                            .Where(a => a.ANNULLATO == false && a.IDTIPOCOEFFICIENTERICHIAMO==idTipoCoeffIndRichiamo)
                                            .ToList()
                                            .Where(b => b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop()))
                                            .ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDCOEFINDRICHIAMO.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].COEFFICIENTERICHIAMO.ToString());
                    tmp.Add(libm[0].IDTIPOCOEFFICIENTERICHIAMO.ToString());
                }
            }
            return tmp;
        }
        public void RendiAnnullatoUnRecord(decimal IDCOEFINDRICHIAMO, ModelDBISE db)
        {
            COEFFICIENTEINDRICHIAMO entita = new COEFFICIENTEINDRICHIAMO();
            entita = db.COEFFICIENTEINDRICHIAMO.Find(IDCOEFINDRICHIAMO);
            entita.ANNULLATO = true;
            db.SaveChanges();
        }
        public decimal Get_Id_CoefficienteIndRichiamoNonAnnullato(decimal IDCOEFINDRICHIAMO)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<COEFFICIENTEINDRICHIAMO> libm = new List<COEFFICIENTEINDRICHIAMO>();
                libm = db.COEFFICIENTEINDRICHIAMO.Where(a => a.ANNULLATO == false
                && a.IDCOEFINDRICHIAMO == IDCOEFINDRICHIAMO).OrderBy(a => a.DATAINIZIOVALIDITA).ThenBy(a => a.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDCOEFINDRICHIAMO;
            }
            return tmp;
        }
    }
}
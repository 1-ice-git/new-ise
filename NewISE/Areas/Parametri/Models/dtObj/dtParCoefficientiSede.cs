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
    public class dtParCoefficientiSede : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<CoefficientiSedeModel> getListCoefficientiSede()
        {
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.COEFFICIENTESEDE.ToList();

                    libm = (from e in lib
                            select new CoefficientiSedeModel()
                            {

                                idCoefficientiSede = e.IDCOEFFICIENTESEDE,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new CoefficientiSedeModel().dataFineValidita,
                                valore = e.VALORECOEFFICIENTE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato=e.ANNULLATO,
                                Ufficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    descUfficio = e.UFFICI.DESCRIZIONEUFFICIO
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

        public IList<CoefficientiSedeModel> getListCoefficientiSede(decimal idUfficio)
        {
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.COEFFICIENTESEDE.Where(a => a.IDUFFICIO == idUfficio).ToList();

                    libm = (from e in lib
                            select new CoefficientiSedeModel()
                            {

                                idCoefficientiSede = e.IDCOEFFICIENTESEDE,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new CoefficientiSedeModel().dataFineValidita,
                                valore = e.VALORECOEFFICIENTE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                Ufficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    descUfficio = e.UFFICI.DESCRIZIONEUFFICIO
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

        public IList<CoefficientiSedeModel> getListCoefficientiSede(bool escludiAnnullati = false)
        {
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.COEFFICIENTESEDE.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new CoefficientiSedeModel()
                            {
                                idCoefficientiSede = e.IDCOEFFICIENTESEDE,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new CoefficientiSedeModel().dataFineValidita,
                                valore = e.VALORECOEFFICIENTE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                Ufficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    descUfficio = e.UFFICI.DESCRIZIONEUFFICIO
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

        public IList<CoefficientiSedeModel> getListCoefficientiSede(decimal idUfficio, bool escludiAnnullati = false)
        {
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                   List<COEFFICIENTESEDE> lib = new List<COEFFICIENTESEDE>();
                    if (escludiAnnullati == true)
                        lib = db.COEFFICIENTESEDE.Where(a => a.IDUFFICIO == idUfficio && a.ANNULLATO == false).ToList();
                    else
                        lib = db.COEFFICIENTESEDE.Where(a => a.IDUFFICIO == idUfficio).ToList();
                    
                    libm = (from e in lib
                            select new CoefficientiSedeModel()
                            {
                                idCoefficientiSede = e.IDCOEFFICIENTESEDE,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita =e.DATAFINEVALIDITA ,//e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new CoefficientiSedeModel().dataFineValidita,
                                valore = e.VALORECOEFFICIENTE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                Ufficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    descUfficio = e.UFFICI.DESCRIZIONEUFFICIO
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
        public void SetCoefficientiSede(CoefficientiSedeModel ibm)
        {
            List<COEFFICIENTESEDE> libNew = new List<COEFFICIENTESEDE>();

            COEFFICIENTESEDE ibNew = new COEFFICIENTESEDE();

            COEFFICIENTESEDE ibPrecedente = new COEFFICIENTESEDE();

            List<COEFFICIENTESEDE> lArchivioIB = new List<COEFFICIENTESEDE>();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    if (ibm.dataFineValidita.HasValue)
                    {
                        if (EsistonoMovimentiSuccessiviUguale(ibm))
                        {
                            ibNew = new COEFFICIENTESEDE()
                            {

                                IDUFFICIO = ibm.idUfficio,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                VALORECOEFFICIENTE = ibm.valore,
                                DATAAGGIORNAMENTO = DateTime.Now,// ibm.dataAggiornamento,
                                ANNULLATO = ibm.annullato
                            };
                        }
                        else
                        {
                            ibNew = new COEFFICIENTESEDE()
                            {
                                IDUFFICIO = ibm.idUfficio,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,//Utility.DataFineStop(),
                                VALORECOEFFICIENTE = ibm.valore,
                                DATAAGGIORNAMENTO = DateTime.Now,// ibm.dataAggiornamento,
                                ANNULLATO = ibm.annullato
                            };
                        }
                    }
                    else
                    {
                        ibNew = new COEFFICIENTESEDE()
                        {
                            IDUFFICIO = ibm.idUfficio,
                            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            DATAFINEVALIDITA =ibm.dataFineValidita.Value,// Utility.DataFineStop(),
                            VALORECOEFFICIENTE = ibm.valore,
                            DATAAGGIORNAMENTO = ibm.dataAggiornamento,//rnamento,
                            ANNULLATO = ibm.annullato
                        };
                    }

                    db.Database.BeginTransaction();

                    var recordInteressati = db.COEFFICIENTESEDE.Where(a => a.ANNULLATO == false && a.IDUFFICIO == ibNew.IDUFFICIO)
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
                                    var ibOld1 = new COEFFICIENTESEDE()
                                    {
                                        IDUFFICIO = item.IDUFFICIO,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(-1),
                                        VALORECOEFFICIENTE = item.VALORECOEFFICIENTE,
                                        DATAAGGIORNAMENTO = DateTime.Now,// item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);

                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new COEFFICIENTESEDE()
                                    {
                                        IDUFFICIO = item.IDUFFICIO,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(-1),
                                        DATAAGGIORNAMENTO = DateTime.Now,// item.DATAAGGIORNAMENTO,
                                        VALORECOEFFICIENTE = item.VALORECOEFFICIENTE,
                                        ANNULLATO = false
                                    };

                                    var ibOld2 = new COEFFICIENTESEDE()
                                    {
                                        IDUFFICIO = item.IDUFFICIO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(+1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        DATAAGGIORNAMENTO = DateTime.Now,// item.DATAAGGIORNAMENTO,
                                        VALORECOEFFICIENTE = item.VALORECOEFFICIENTE,
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
                                    var ibOld1 = new COEFFICIENTESEDE()
                                    {
                                        IDUFFICIO = item.IDUFFICIO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        VALORECOEFFICIENTE = item.VALORECOEFFICIENTE,
                                        DATAAGGIORNAMENTO = DateTime.Now,// item.DATAAGGIORNAMENTO,
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
                                    var ibOld1 = new COEFFICIENTESEDE()
                                    {
                                        IDUFFICIO = item.IDUFFICIO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        VALORECOEFFICIENTE = item.VALORECOEFFICIENTE,
                                        DATAAGGIORNAMENTO = DateTime.Now,// item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                }
                            }
                        }

                        libNew.Add(ibNew);
                        libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        db.COEFFICIENTESEDE.AddRange(libNew);
                    }
                    else
                    {
                        db.COEFFICIENTESEDE.Add(ibNew);

                    }
                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro coefficiente di sede.", "COEFFICENTISEDE", ibNew.IDCOEFFICIENTESEDE);
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

        public bool EsistonoMovimentiPrima(CoefficientiSedeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.COEFFICIENTESEDE.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(CoefficientiSedeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.COEFFICIENTESEDE.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(CoefficientiSedeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.COEFFICIENTESEDE.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiPrimaUguale(CoefficientiSedeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.COEFFICIENTESEDE.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
            }
        }

        public void DelCoefficientiSede(decimal idCoefficientiSede)
        {
            COEFFICIENTESEDE precedenteIB = new COEFFICIENTESEDE();
            COEFFICIENTESEDE delIB = new COEFFICIENTESEDE();


            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.COEFFICIENTESEDE.Where(a => a.IDCOEFFICIENTESEDE == idCoefficientiSede);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.COEFFICIENTESEDE.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new COEFFICIENTESEDE()
                            {
                                IDUFFICIO = precedenteIB.IDUFFICIO,

                                DATAINIZIOVALIDITA = precedenteIB.DATAFINEVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                VALORECOEFFICIENTE = precedenteIB.VALORECOEFFICIENTE,


                                ANNULLATO = false
                            };

                            db.COEFFICIENTESEDE.Add(ibOld1);
                        }

                        db.SaveChanges();

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro del coefficiente di sede.", "COEFFICENTISEDE", idCoefficientiSede);
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
            var fm = context.ObjectInstance as CoefficientiSedeModel;
            if (fm != null)
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
            else
            {
                vr = new ValidationResult("La data di inizio validità è richiesta.");
            }
            return vr;
        }

        public bool CoefficientiSedeAnnullato(CoefficientiSedeModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.COEFFICIENTESEDE.Where(a => a.IDCOEFFICIENTESEDE == ibm.idCoefficientiSede && a.IDUFFICIO == ibm.idUfficio).First().ANNULLATO == true ? true : false;
            }
        }
    }
}
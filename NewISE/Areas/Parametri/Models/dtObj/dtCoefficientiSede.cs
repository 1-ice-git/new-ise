using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtCoefficientiSede : IDisposable
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
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.COEFFICENTISEDE.ToList();

                    libm = (from e in lib
                            select new CoefficientiSedeModel()
                            {
                                
                                idCoefficientiSede = e.IDCOEFFICENTESEDE,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new CoefficientiSedeModel().dataFineValidita,
                                valoreCoefficiente = e.VALORECOEFFICENTE,
                                DescrizioneUfficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    DescUfficio = e.UFFICI.DESCRIZIONEUFFICIO
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
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.COEFFICENTISEDE.Where(a => a.IDUFFICIO == idUfficio).ToList();

                    libm = (from e in lib
                            select new CoefficientiSedeModel()
                            {

                                idCoefficientiSede = e.IDCOEFFICENTESEDE,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new CoefficientiSedeModel().dataFineValidita,
                                valoreCoefficiente = e.VALORECOEFFICENTE,
                                DescrizioneUfficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    DescUfficio = e.UFFICI.DESCRIZIONEUFFICIO
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
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.COEFFICENTISEDE.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new CoefficientiSedeModel()
                            {
                                idCoefficientiSede = e.IDCOEFFICENTESEDE,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new CoefficientiSedeModel().dataFineValidita,
                                valoreCoefficiente = e.VALORECOEFFICENTE,
                                DescrizioneUfficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    DescUfficio = e.UFFICI.DESCRIZIONEUFFICIO
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
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.COEFFICENTISEDE.Where(a => a.IDUFFICIO == idUfficio && a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new CoefficientiSedeModel()
                            {
                                idCoefficientiSede = e.IDCOEFFICENTESEDE,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new CoefficientiSedeModel().dataFineValidita,
                                valoreCoefficiente = e.VALORECOEFFICENTE,
                                DescrizioneUfficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    DescUfficio = e.UFFICI.DESCRIZIONEUFFICIO
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
            List<COEFFICENTISEDE> libNew = new List<COEFFICENTISEDE>();

            COEFFICENTISEDE ibNew = new COEFFICENTISEDE();

            COEFFICENTISEDE ibPrecedente = new COEFFICENTISEDE();

            List<COEFFICENTISEDE> lArchivioIB = new List<COEFFICENTISEDE>();

            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                try
                {
                    if (ibm.dataFineValidita.HasValue)
                    {
                        if (EsistonoMovimentiSuccessiviUguale(ibm))
                        {
                            ibNew = new COEFFICENTISEDE()
                            {

                                IDUFFICIO = ibm.idUfficio,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                VALORECOEFFICENTE = ibm.valoreCoefficiente,
                                ANNULLATO = ibm.annullato
                            };
                        }
                        else
                        {
                            ibNew = new COEFFICENTISEDE()
                            {
                                IDUFFICIO = ibm.idUfficio,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                                VALORECOEFFICENTE = ibm.valoreCoefficiente,
                                ANNULLATO = ibm.annullato
                            };
                        }
                    }
                    else
                    {
                        ibNew = new COEFFICENTISEDE()
                        {
                            IDUFFICIO = ibm.idUfficio,
                            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                            VALORECOEFFICENTE = ibm.valoreCoefficiente,
                            ANNULLATO = ibm.annullato
                        };
                    }

                    db.Database.BeginTransaction();

                    var recordInteressati = db.COEFFICENTISEDE.Where(a => a.ANNULLATO == false && a.IDUFFICIO == ibNew.IDUFFICIO)
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
                                    var ibOld1 = new COEFFICENTISEDE()
                                    {
                                        IDUFFICIO = item.IDUFFICIO,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        VALORECOEFFICENTE = item.VALORECOEFFICENTE,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);

                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new COEFFICENTISEDE()
                                    {
                                        IDUFFICIO = item.IDUFFICIO,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        VALORECOEFFICENTE = item.VALORECOEFFICENTE,
                                        ANNULLATO = false
                                    };

                                    var ibOld2 = new COEFFICENTISEDE()
                                    {
                                        IDUFFICIO = item.IDUFFICIO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(+1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        VALORECOEFFICENTE = item.VALORECOEFFICENTE,
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
                                    var ibOld1 = new COEFFICENTISEDE()
                                    {
                                        IDUFFICIO = item.IDUFFICIO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        VALORECOEFFICENTE = item.VALORECOEFFICENTE,
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
                                    var ibOld1 = new COEFFICENTISEDE()
                                    {
                                        IDUFFICIO = item.IDUFFICIO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        VALORECOEFFICENTE = item.VALORECOEFFICENTE,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                }
                            }
                        }

                        libNew.Add(ibNew);
                        libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        db.COEFFICENTISEDE.AddRange(libNew);
                    }
                    else
                    {
                        db.COEFFICENTISEDE.Add(ibNew);

                    }
                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro coefficiente di sede.", "COEFFICENTISEDE", ibNew.IDCOEFFICENTESEDE);
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
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                return db.COEFFICENTISEDE.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(CoefficientiSedeModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.COEFFICENTISEDE.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(CoefficientiSedeModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.COEFFICENTISEDE.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiPrimaUguale(CoefficientiSedeModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                return db.COEFFICENTISEDE.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
            }
        }

        public void DelCoefficientiSede(decimal idCoefficientiSede)
        {
            COEFFICENTISEDE precedenteIB = new COEFFICENTISEDE();
            COEFFICENTISEDE delIB = new COEFFICENTISEDE();


            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.COEFFICENTISEDE.Where(a => a.IDCOEFFICENTESEDE == idCoefficientiSede);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.COEFFICENTISEDE.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new COEFFICENTISEDE()
                            {
                                IDUFFICIO = precedenteIB.IDUFFICIO,

                                DATAINIZIOVALIDITA = precedenteIB.DATAFINEVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                VALORECOEFFICENTE = precedenteIB.VALORECOEFFICENTE,
                                

                                ANNULLATO = false
                            };

                            db.COEFFICENTISEDE.Add(ibOld1);
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

    }
}
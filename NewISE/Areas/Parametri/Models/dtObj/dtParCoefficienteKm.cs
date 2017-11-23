using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParCoefficienteKm: IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<CoeffFasciaKmModel> getListCoeffFasciaKm()
        {
            List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.COEFFICIENTEFKM.ToList();

                    libm = (from e in lib
                            select new CoeffFasciaKmModel()
                            {
                                idCfKm = e.IDCFKM,
                                idDefKm = e.IDDEFKM,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                coefficienteKm = e.COEFFICIENTEKM,
                                annullato = e.ANNULLATO,
                                km = new DefFasciaKmModel()
                                {
                                    idDefKm = e.DEFFASCIACHILOMETRICA.IDDEFKM,
                                    km = e.DEFFASCIACHILOMETRICA.KM
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

        public IList<CoeffFasciaKmModel> getListCoeffFasciaKm(decimal idCfKm)
        {
            List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.COEFFICIENTEFKM.Where(a => a.IDCFKM == idCfKm).ToList();

                    libm = (from e in lib
                            select new CoeffFasciaKmModel()
                            {
                                idCfKm = e.IDCFKM,
                                idDefKm = e.IDDEFKM,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                coefficienteKm = e.COEFFICIENTEKM,
                                annullato = e.ANNULLATO,
                                km = new DefFasciaKmModel()
                                {
                                    idDefKm = e.DEFFASCIACHILOMETRICA.IDDEFKM,
                                    km = e.DEFFASCIACHILOMETRICA.KM
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

        public IList<CoeffFasciaKmModel> getListCoeffFasciaKm(bool escludiAnnullati = false)
        {
            List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new CoeffFasciaKmModel()
                            {
                                idCfKm = e.IDCFKM,
                                idDefKm = e.IDDEFKM,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                coefficienteKm = e.COEFFICIENTEKM,
                                annullato = e.ANNULLATO,
                                km = new DefFasciaKmModel()
                                {
                                    idDefKm = e.DEFFASCIACHILOMETRICA.IDDEFKM,
                                    km = e.DEFFASCIACHILOMETRICA.KM
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

        public IList<CoeffFasciaKmModel> getListCoeffFasciaKm(decimal idCfKm, bool escludiAnnullati = false)
        {
            List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.COEFFICIENTEFKM.Where(a => a.IDCFKM == idCfKm && a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new CoeffFasciaKmModel()
                            {
                                idCfKm = e.IDCFKM,
                                idDefKm = e.IDDEFKM,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                coefficienteKm = e.COEFFICIENTEKM,
                                annullato = e.ANNULLATO,
                                km = new DefFasciaKmModel()
                                {
                                    idDefKm = e.DEFFASCIACHILOMETRICA.IDDEFKM,
                                    km = e.DEFFASCIACHILOMETRICA.KM
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
        public void SetCoeffFasciaKm(CoeffFasciaKmModel ibm)
        {
            List<COEFFICIENTEFKM> libNew = new List<COEFFICIENTEFKM>();

            COEFFICIENTEFKM ibNew = new COEFFICIENTEFKM();

            COEFFICIENTEFKM ibPrecedente = new COEFFICIENTEFKM();

            List<COEFFICIENTEFKM> lArchivioIB = new List<COEFFICIENTEFKM>();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    if (ibm.dataFineValidita.HasValue)
                    {
                        if (EsistonoMovimentiSuccessiviUguale(ibm))
                        {
                            ibNew = new COEFFICIENTEFKM()
                            {
                                IDCFKM = ibm.idCfKm,
                                IDDEFKM = ibm.idDefKm,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                COEFFICIENTEKM = ibm.coefficienteKm,
                                DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                                ANNULLATO = ibm.annullato
                            };
                        }
                        else
                        {
                            ibNew = new COEFFICIENTEFKM()
                            {
                                //IDLIVELLO = ibm.idLivello,
                                //DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                //DATAFINEVALIDITA = Utility.DataFineStop(),
                                //VALORE = ibm.valore,
                                //VALORERESP = ibm.valoreResponsabile,
                                //ANNULLATO = ibm.annullato

                                IDCFKM = ibm.idCfKm,
                                IDDEFKM = ibm.idDefKm,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                COEFFICIENTEKM = ibm.coefficienteKm,
                                DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                                ANNULLATO = ibm.annullato

                            };
                        }
                    }
                    else
                    {
                        ibNew = new COEFFICIENTEFKM()
                        {
                            //IDLIVELLO = ibm.idLivello,
                            //DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            //DATAFINEVALIDITA = Utility.DataFineStop(),
                            //VALORE = ibm.valore,
                            //VALORERESP = ibm.valoreResponsabile,
                            //ANNULLATO = ibm.annullato

                            IDCFKM = ibm.idCfKm,
                            IDDEFKM = ibm.idDefKm,
                            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                            COEFFICIENTEKM = ibm.coefficienteKm,
                            DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                            ANNULLATO = ibm.annullato
                        };
                    }

                    db.Database.BeginTransaction();

                    var recordInteressati = db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == false && a.IDCFKM == ibNew.IDCFKM)
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
                                    var ibOld1 = new COEFFICIENTEFKM()
                                    {
                                        
                                        IDCFKM = item.IDCFKM,
                                        IDDEFKM =item.IDDEFKM,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(-1),
                                        COEFFICIENTEKM = item.COEFFICIENTEKM,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);

                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new COEFFICIENTEFKM()
                                    {
                                        IDCFKM = item.IDCFKM,
                                        IDDEFKM = item.IDDEFKM,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(-1),
                                        COEFFICIENTEKM = item.COEFFICIENTEKM,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    var ibOld2 = new COEFFICIENTEFKM()
                                    {
                                        //IDLIVELLO = item.IDLIVELLO,
                                        //DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(+1),
                                        //DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        //VALORE = item.VALORE,
                                        //VALORERESP = item.VALORERESP,
                                        //ANNULLATO = false

                                        IDCFKM = item.IDCFKM,
                                        IDDEFKM = item.IDDEFKM,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(-1),
                                        COEFFICIENTEKM = item.COEFFICIENTEKM,
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
                                    var ibOld1 = new COEFFICIENTEFKM()
                                    {
                                        IDCFKM = item.IDCFKM,
                                        IDDEFKM = item.IDDEFKM,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        COEFFICIENTEKM = item.COEFFICIENTEKM,
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
                                    var ibOld1 = new COEFFICIENTEFKM()
                                    {
                                        
                                        IDCFKM = item.IDCFKM,
                                        IDDEFKM = item.IDDEFKM,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        COEFFICIENTEKM = item.COEFFICIENTEKM,
                                        DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                }
                            }
                        }

                        libNew.Add(ibNew);
                        libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        db.COEFFICIENTEFKM.AddRange(libNew);
                    }
                    else
                    {
                        db.COEFFICIENTEFKM.Add(ibNew);

                    }
                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro coefficiente di fascia km.", "COEFFICIENTEFKM", ibNew.IDCFKM);
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

        public bool EsistonoMovimentiPrima(CoeffFasciaKmModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == false && a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDCFKM == ibm.idCfKm).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(CoeffFasciaKmModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == false && a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDCFKM == ibm.idCfKm).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(CoeffFasciaKmModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == false && a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDCFKM == ibm.idCfKm).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }



        public bool EsistonoMovimentiPrimaUguale(CoeffFasciaKmModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.COEFFICIENTEFKM.Where(a => a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDCFKM == ibm.idCfKm).Count() > 0 ? true : false;
            }
        }

        public void DelCoeffFasciaKm(decimal idCfKm)
        {
            COEFFICIENTEFKM precedenteIB = new COEFFICIENTEFKM();
            COEFFICIENTEFKM delIB = new COEFFICIENTEFKM();
            
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.COEFFICIENTEFKM.Where(a => a.IDCFKM == idCfKm);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.COEFFICIENTEFKM.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new COEFFICIENTEFKM()
                            {
                                
                                IDCFKM = precedenteIB.IDCFKM,
                                IDDEFKM = precedenteIB.IDDEFKM,
                                DATAINIZIOVALIDITA = precedenteIB.DATAFINEVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                COEFFICIENTEKM = precedenteIB.COEFFICIENTEKM,
                                ANNULLATO = false
                            };

                            db.COEFFICIENTEFKM.Add(ibOld1);
                        }

                        db.SaveChanges();

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di Coefficiente Km.", "COEFFICIENTEFKM", idCfKm);
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
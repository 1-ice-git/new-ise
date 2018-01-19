using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.Models.Tools;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParAliquoteContr : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<AliquoteContributiveModel> getListAliquoteContributive()
        {
            List<AliquoteContributiveModel> libm = new List<AliquoteContributiveModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.ALIQUOTECONTRIBUTIVE.ToList();

                    libm = (from e in lib
                            select new AliquoteContributiveModel()
                            {

                                idAliqContr = e.IDALIQCONTR,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,// != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new AliquoteContributiveModel().dataFineValidita,
                                aliquota = e.ALIQUOTA,
                                annullato = e.ANNULLATO,
                                descrizione = new TipoAliquoteContributiveModel()
                                {
                                    idTipoAliqContr = e.IDTIPOCONTRIBUTO,
                                    descrizione = e.ALIQUOTA.ToString()

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

        public IList<AliquoteContributiveModel> getListAliquoteContributive(decimal idAliqContr)
        {
            List<AliquoteContributiveModel> libm = new List<AliquoteContributiveModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.ALIQUOTECONTRIBUTIVE.Where(a => a.IDALIQCONTR == idAliqContr).ToList();

                    libm = (from e in lib
                            select new AliquoteContributiveModel()
                            {

                                idAliqContr = e.IDALIQCONTR,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA ,//!= Utility.DataFineStop() ? e.DATAFINEVALIDITA : new AliquoteContributiveModel().dataFineValidita,
                                aliquota = e.ALIQUOTA,
                                annullato = e.ANNULLATO,
                                descrizione = new TipoAliquoteContributiveModel()
                                {
                                    idTipoAliqContr = e.IDTIPOCONTRIBUTO,
                                    descrizione = e.ALIQUOTA.ToString()

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

        public IList<AliquoteContributiveModel> getListAliquoteContributive(bool escludiAnnullati = false)
        {
            List<AliquoteContributiveModel> libm = new List<AliquoteContributiveModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.ALIQUOTECONTRIBUTIVE.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new AliquoteContributiveModel()
                            {

                                idAliqContr = e.IDALIQCONTR,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new AliquoteContributiveModel().dataFineValidita,
                                aliquota = e.ALIQUOTA,
                                annullato = e.ANNULLATO,
                                descrizione = new TipoAliquoteContributiveModel()
                                {
                                    idTipoAliqContr = e.IDTIPOCONTRIBUTO,
                                    descrizione = e.ALIQUOTA.ToString()

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

        public IList<AliquoteContributiveModel> getListAliquoteContributive(decimal idAliqContr, bool escludiAnnullati = false)
        {
            List<AliquoteContributiveModel> libm = new List<AliquoteContributiveModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.ALIQUOTECONTRIBUTIVE.Where(a => a.IDALIQCONTR == idAliqContr && a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new AliquoteContributiveModel()
                            {

                                idAliqContr = e.IDALIQCONTR,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Utility.DataFineStop() ? e.DATAFINEVALIDITA : new AliquoteContributiveModel().dataFineValidita,
                                aliquota = e.ALIQUOTA,
                                annullato = e.ANNULLATO,
                                descrizione = new TipoAliquoteContributiveModel()
                                {
                                    idTipoAliqContr = e.IDTIPOCONTRIBUTO,
                                    descrizione = e.ALIQUOTA.ToString()

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
        public void SetAliquoteContributive(AliquoteContributiveModel ibm)
        {
            List<ALIQUOTECONTRIBUTIVE> libNew = new List<ALIQUOTECONTRIBUTIVE>();

            ALIQUOTECONTRIBUTIVE ibNew = new ALIQUOTECONTRIBUTIVE();

            ALIQUOTECONTRIBUTIVE ibPrecedente = new ALIQUOTECONTRIBUTIVE();

            List<ALIQUOTECONTRIBUTIVE> lArchivioIB = new List<ALIQUOTECONTRIBUTIVE>();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    if (ibm.dataFineValidita.HasValue)
                    {
                        if (EsistonoMovimentiSuccessiviUguale(ibm))
                        {
                            ibNew = new ALIQUOTECONTRIBUTIVE()
                            {

                                IDALIQCONTR = ibm.idAliqContr,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                ALIQUOTA = ibm.aliquota,
                                ANNULLATO = ibm.annullato
                            };
                        }
                        else
                        {
                            ibNew = new ALIQUOTECONTRIBUTIVE()
                            {

                                IDALIQCONTR = ibm.idAliqContr,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                //DATAFINEVALIDITA = Utility.DataFineStop(),
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                ALIQUOTA = ibm.aliquota,
                                ANNULLATO = ibm.annullato
                            };
                        }
                    }
                    else
                    {
                        ibNew = new ALIQUOTECONTRIBUTIVE()
                        {

                            IDALIQCONTR = ibm.idAliqContr,
                            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            //DATAFINEVALIDITA = Utility.DataFineStop(),
                            DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                            ALIQUOTA = ibm.aliquota,
                            ANNULLATO = ibm.annullato
                        };
                    }

                    db.Database.BeginTransaction();

                    var recordInteressati = db.ALIQUOTECONTRIBUTIVE.Where(a => a.ANNULLATO == false && a.IDALIQCONTR == ibNew.IDALIQCONTR)
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
                                    var ibOld1 = new ALIQUOTECONTRIBUTIVE()
                                    {

                                        IDALIQCONTR = item.IDALIQCONTR,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(-1),
                                        ALIQUOTA = item.ALIQUOTA,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);

                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new ALIQUOTECONTRIBUTIVE()
                                    {

                                        IDALIQCONTR = item.IDALIQCONTR,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(-1),
                                        ALIQUOTA = item.ALIQUOTA,
                                        ANNULLATO = false
                                    };

                                    var ibOld2 = new ALIQUOTECONTRIBUTIVE()
                                    {

                                        IDALIQCONTR = item.IDALIQCONTR,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(+1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        ALIQUOTA = item.ALIQUOTA,
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
                                    var ibOld1 = new ALIQUOTECONTRIBUTIVE()
                                    {

                                        IDALIQCONTR = item.IDALIQCONTR,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        ALIQUOTA = item.ALIQUOTA,
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
                                    var ibOld1 = new ALIQUOTECONTRIBUTIVE()
                                    {

                                        IDALIQCONTR = item.IDALIQCONTR,
                                        DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        ALIQUOTA = item.ALIQUOTA,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                }
                            }
                        }

                        libNew.Add(ibNew);
                        libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        db.ALIQUOTECONTRIBUTIVE.AddRange(libNew);
                    }
                    else
                    {
                        db.ALIQUOTECONTRIBUTIVE.Add(ibNew);

                    }
                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro aliquote contributive.", "ALIQUOTECONTRIBUTIVE", ibNew.IDALIQCONTR);
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

        public bool EsistonoMovimentiPrima(AliquoteContributiveModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.ALIQUOTECONTRIBUTIVE.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDALIQCONTR == ibm.idAliqContr).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(AliquoteContributiveModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.ALIQUOTECONTRIBUTIVE.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDALIQCONTR == ibm.idAliqContr).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(AliquoteContributiveModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.ALIQUOTECONTRIBUTIVE.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDALIQCONTR == ibm.idAliqContr).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }
        
        public bool EsistonoMovimentiPrimaUguale(AliquoteContributiveModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.ALIQUOTECONTRIBUTIVE.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDALIQCONTR == ibm.idAliqContr).Count() > 0 ? true : false;
            }
        }

        // public void DelAliquoteContributive(decimal idTipoAliqContr)
        public void DelAliquoteContributive(decimal idAliqContr)
        {
            ALIQUOTECONTRIBUTIVE precedenteIB = new ALIQUOTECONTRIBUTIVE();
            ALIQUOTECONTRIBUTIVE delIB = new ALIQUOTECONTRIBUTIVE();
            
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    //var lib = db.ALIQUOTECONTRIBUTIVE.Where(a => a.IDTIPOCONTRIBUTO == idTipoAliqContr);
                    var lib = db.ALIQUOTECONTRIBUTIVE.Where(a => a.IDALIQCONTR == idAliqContr);
                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.ALIQUOTECONTRIBUTIVE.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new ALIQUOTECONTRIBUTIVE()
                            {

                              //  IDALIQCONTR = precedenteIB.IDALIQCONTR,
                                DATAINIZIOVALIDITA = precedenteIB.DATAFINEVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                ALIQUOTA = precedenteIB.ALIQUOTA,
                                IDTIPOCONTRIBUTO= precedenteIB.IDTIPOCONTRIBUTO,
                                ANNULLATO = false
                            };

                            db.ALIQUOTECONTRIBUTIVE.Add(ibOld1);
                        }

                        db.SaveChanges();

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            //log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di aliquote contributive.", "ALIQUOTECONTRIBUTIVE", idTipoAliqContr);
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di aliquote contributive.", "ALIQUOTECONTRIBUTIVE", idAliqContr);
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
        //public decimal Get_Id_PercentualMagConiugePrimoNonAnnullato(decimal idTipologiaConiuge)
        //{
        //    decimal tmp = 0;
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        List<ALIQUOTECONTRIBUTIVE> libm = new List<ALIQUOTECONTRIBUTIVE>();
        //        libm = db.ALIQUOTECONTRIBUTIVE.Where(a => a.ANNULLATO == false
        //        && a.IDTIPOCONTRIBUTO == idTipologiaConiuge).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
        //        if (libm.Count != 0)
        //            //tmp = libm.First().IDPERCMAGCONIUGE;
        //    }
        //    return tmp;
        //}
    }
}
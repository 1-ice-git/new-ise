using NewISE.Models.dtObj.objB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtPercentualeDisagio : IDisposable
    {
        
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<PercentualeDisagioModel> getListPercentualeDisagio()
        {
            List<PercentualeDisagioModel> libm = new List<PercentualeDisagioModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.PERCENTUALEDISAGIO.ToList();

                    libm = (from e in lib
                            select new PercentualeDisagioModel()
                            {
                                idPercentualeDisagio = e.IDPERCENTUALEDISAGIO,
                                //idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new PercentualeDisagioModel().dataFineValidita,
                                percentuale = e.PERCENTUALE,
                                annullato = e.ANNULLATO,
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

        public IList<PercentualeDisagioModel> getListPercentualeDisagio(decimal idUfficio)
        {
            List<PercentualeDisagioModel> libm = new List<PercentualeDisagioModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                  
                    var lib = db.PERCENTUALEDISAGIO.Where(a => a.IDUFFICIO == idUfficio).ToList();

                    libm = (from e in lib
                            select new PercentualeDisagioModel()
                            {
                                idPercentualeDisagio = e.IDPERCENTUALEDISAGIO,
                                idUfficio = e.IDUFFICIO.Value,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new PercentualeDisagioModel().dataFineValidita,
                                percentuale = e.PERCENTUALE,
                                annullato = e.ANNULLATO,
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

        public IList<PercentualeDisagioModel> getListPercentualeDisagio(bool escludiAnnullati = false)
        {
            List<PercentualeDisagioModel> libm = new List<PercentualeDisagioModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.PERCENTUALEDISAGIO.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new PercentualeDisagioModel()
                            {
                                idPercentualeDisagio = e.IDPERCENTUALEDISAGIO,
                                idUfficio = e.IDUFFICIO.Value,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new PercentualeDisagioModel().dataFineValidita,
                                percentuale = e.PERCENTUALE,
                                annullato = e.ANNULLATO,
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

        public IList<PercentualeDisagioModel> getListPercentualeDisagio(decimal idUfficio, bool escludiAnnullati = false)
        {
            List<PercentualeDisagioModel> libm = new List<PercentualeDisagioModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.PERCENTUALEDISAGIO.Where(a => a.IDUFFICIO == idUfficio && a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new PercentualeDisagioModel()
                            {
                                idPercentualeDisagio = e.IDPERCENTUALEDISAGIO,
                                idUfficio = e.IDUFFICIO.Value,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new PercentualeDisagioModel().dataFineValidita,
                                percentuale = e.PERCENTUALE,
                                annullato = e.ANNULLATO,
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
        /// 
        public void SetPercentualeDisagio(PercentualeDisagioModel ibm)
        {
            List<PERCENTUALEDISAGIO> libNew = new List<PERCENTUALEDISAGIO>();

            PERCENTUALEDISAGIO ibNew = new PERCENTUALEDISAGIO();

            PERCENTUALEDISAGIO ibPrecedente = new PERCENTUALEDISAGIO();

            List<PERCENTUALEDISAGIO> lArchivioIB = new List<PERCENTUALEDISAGIO>();

            using (EntitiesDBISE db = new EntitiesDBISE())
            {
            //    try
            //    {
            //        if (ibm.dataFineValidita.HasValue)
            //        {
            //            if (EsistonoMovimentiSuccessiviUguale(ibm))
            //            {
            //                ibNew = new PERCENTUALEDISAGIO()
            //                {
            //                    IDLIVELLO = ibm.idLivello,
            //                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
            //                    DATAFINEVALIDITA = ibm.dataFineValidita.Value,
            //                    VALORE = ibm.valore,
            //                    VALORERESP = ibm.valoreResponsabile,
            //                    ANNULLATO = ibm.annullato
            //                };
            //            }
            //            else
            //            {
            //                ibNew = new PERCENTUALEDISAGIO()
            //                {
            //                    IDLIVELLO = ibm.idLivello,
            //                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
            //                    DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
            //                    VALORE = ibm.valore,
            //                    VALORERESP = ibm.valoreResponsabile,
            //                    ANNULLATO = ibm.annullato
            //                };
            //            }
            //        }
            //        else
            //        {
            //            ibNew = new INDENNITABASE()
            //            {
            //                IDLIVELLO = ibm.idLivello,
            //                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
            //                DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
            //                VALORE = ibm.valore,
            //                VALORERESP = ibm.valoreResponsabile,
            //                ANNULLATO = ibm.annullato
            //            };
            //        }

            //        db.Database.BeginTransaction();

            //        var recordInteressati = db.PERCENTUALEDISAGIO.Where(a => a.ANNULLATO == false && a.IDLIVELLO == ibNew.IDLIVELLO)
            //                                                .Where(a => a.DATAINIZIOVALIDITA >= ibNew.DATAINIZIOVALIDITA || a.DATAFINEVALIDITA >= ibNew.DATAINIZIOVALIDITA)
            //                                                .Where(a => a.DATAINIZIOVALIDITA <= ibNew.DATAFINEVALIDITA || a.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
            //                                                .ToList();

            //        recordInteressati.ForEach(a => a.ANNULLATO = true);
            //        //db.SaveChanges();

            //        if (recordInteressati.Count > 0)
            //        {
            //            foreach (var item in recordInteressati)
            //            {

            //                if (item.DATAINIZIOVALIDITA < ibNew.DATAINIZIOVALIDITA)
            //                {
            //                    if (item.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
            //                    {
            //                        var ibOld1 = new INDENNITABASE()
            //                        {
            //                            IDLIVELLO = item.IDLIVELLO,
            //                            DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
            //                            DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
            //                            VALORE = item.VALORE,
            //                            VALORERESP = item.VALORERESP,
            //                            ANNULLATO = false
            //                        };

            //                        libNew.Add(ibOld1);

            //                    }
            //                    else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
            //                    {
            //                        var ibOld1 = new INDENNITABASE()
            //                        {
            //                            IDLIVELLO = item.IDLIVELLO,
            //                            DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
            //                            DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
            //                            VALORE = item.VALORE,
            //                            VALORERESP = item.VALORERESP,
            //                            ANNULLATO = false
            //                        };

            //                        var ibOld2 = new INDENNITABASE()
            //                        {
            //                            IDLIVELLO = item.IDLIVELLO,
            //                            DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(+1),
            //                            DATAFINEVALIDITA = item.DATAFINEVALIDITA,
            //                            VALORE = item.VALORE,
            //                            VALORERESP = item.VALORERESP,
            //                            ANNULLATO = false
            //                        };

            //                        libNew.Add(ibOld1);
            //                        libNew.Add(ibOld2);

            //                    }

            //                }
            //                else if (item.DATAINIZIOVALIDITA == ibNew.DATAINIZIOVALIDITA)
            //                {
            //                    if (item.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
            //                    {
            //                        //Non preleva il record old
            //                    }
            //                    else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
            //                    {
            //                        var ibOld1 = new INDENNITABASE()
            //                        {
            //                            IDLIVELLO = item.IDLIVELLO,
            //                            DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
            //                            DATAFINEVALIDITA = item.DATAFINEVALIDITA,
            //                            VALORE = item.VALORE,
            //                            VALORERESP = item.VALORERESP,
            //                            ANNULLATO = false
            //                        };

            //                        libNew.Add(ibOld1);
            //                    }
            //                }
            //                else if (item.DATAINIZIOVALIDITA > ibNew.DATAINIZIOVALIDITA)
            //                {
            //                    if (item.DATAFINEVALIDITA <= ibNew.DATAFINEVALIDITA)
            //                    {
            //                        //Non preleva il record old
            //                    }
            //                    else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
            //                    {
            //                        var ibOld1 = new INDENNITABASE()
            //                        {
            //                            IDLIVELLO = item.IDLIVELLO,
            //                            DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
            //                            DATAFINEVALIDITA = item.DATAFINEVALIDITA,
            //                            VALORE = item.VALORE,
            //                            VALORERESP = item.VALORERESP,
            //                            ANNULLATO = false
            //                        };

            //                        libNew.Add(ibOld1);
            //                    }
            //                }
            //            }

            //            libNew.Add(ibNew);
            //            libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

            //            db.PERCENTUALEDISAGIO.AddRange(libNew);
            //        }
            //        else
            //        {
            //            db.PERCENTUALEDISAGIO.Add(ibNew);

            //        }
            //        db.SaveChanges();

            //        using (objLogAttivita log = new objLogAttivita())
            //        {
            //            log.Log(enumAttivita.Inserimento, "Inserimento parametro di percentuale di disagio.", "PERCENTUALEDISAGIO", ibNew.IDPERCENTUALEDISAGIO);
            //        }

            //        db.Database.CurrentTransaction.Commit();
            //    }
            //    catch (Exception ex)
            //    {
            //        db.Database.CurrentTransaction.Rollback();
            //        throw ex;
            //    }
            }
        }
        public bool EsistonoMovimentiPrima(PercentualeDisagioModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                return db.PERCENTUALEDISAGIO.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDPERCENTUALEDISAGIO == ibm.idPercentualeDisagio).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(PercentualeDisagioModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.PERCENTUALEDISAGIO.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool EsistonoMovimentiSuccessiviUguale(PercentualeDisagioModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.PERCENTUALEDISAGIO.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool EsistonoMovimentiPrimaUguale(PercentualeDisagioModel ibm)
        {
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                return db.PERCENTUALEDISAGIO.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDUFFICIO == ibm.idUfficio).Count() > 0 ? true : false;
            }
        }
        public void DelPercentualeDisagio(decimal idPercDisagio)
        {
            PERCENTUALEDISAGIO precedenteIB = new PERCENTUALEDISAGIO();
            PERCENTUALEDISAGIO delIB = new PERCENTUALEDISAGIO();


            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                //try
                //{
                //    db.Database.BeginTransaction();

                //    var lib = db.PERCENTUALEDISAGIO.Where(a => a.IDPERCENTUALEDISAGIO == idIndbase);

                //    if (lib.Count() > 0)
                //    {
                //        delIB = lib.First();
                //        delIB.ANNULLATO = true;

                //        var lprecIB = db.PERCENTUALEDISAGIO.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                //        if (lprecIB.Count > 0)
                //        {
                //            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                //            precedenteIB.ANNULLATO = true;

                //            var ibOld1 = new INDENNITABASE()
                //            {
                //                IDLIVELLO = precedenteIB.IDLIVELLO,
                //                DATAINIZIOVALIDITA = precedenteIB.DATAFINEVALIDITA,
                //                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                //                VALORE = precedenteIB.VALORE,
                //                VALORERESP = precedenteIB.VALORERESP,
                //                ANNULLATO = false
                //            };

                //            db.INDENNITABASE.Add(ibOld1);
                //        }

                //        db.SaveChanges();

                //        using (objLogAttivita log = new objLogAttivita())
                //        {
                //            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di indennità di base.", "PERCENTUALEDISAGIO", idIndbase);
                //        }


                //        db.Database.CurrentTransaction.Commit();
                //    }
                //}
                //catch (Exception ex)
                //{
                //    db.Database.CurrentTransaction.Rollback();
                //    throw ex;
                //}

            }

        }

    }
}
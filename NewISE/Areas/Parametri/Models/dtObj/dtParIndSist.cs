using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParIndSist : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public IList<IndennitaSistemazioneModel> getListIndennitaSistemazione()
        {
            List<IndennitaSistemazioneModel> libm = new List<IndennitaSistemazioneModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.INDENNITASISTEMAZIONE.ToList();

                    libm = (from e in lib
                            select new IndennitaSistemazioneModel()
                            {
                                
                                idIndSist= e.IDINDSIST,
                                idTipoTrasferimento = e.IDTIPOTRASFERIMENTO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaSistemazioneModel().dataFineValidita,
                                coefficiente = e.COEFFICIENTE,
                                dataAggiornamento = System.DateTime.Now,
                                annullato = e.ANNULLATO,
                                TipoTrasferimento = new TipoTrasferimentoModel()
                                {
                                    
                                    idTipoTrasferimento = e.IDTIPOTRASFERIMENTO,
                                    descTipoTrasf = e.TIPOTRASFERIMENTO.ToString()
                                    
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

        public IList<IndennitaSistemazioneModel> getListIndennitaSistemazione(decimal idTipoTrasferimento)
        {
            List<IndennitaSistemazioneModel> libm = new List<IndennitaSistemazioneModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.INDENNITASISTEMAZIONE.Where(a => a.IDTIPOTRASFERIMENTO == idTipoTrasferimento).ToList();

                    libm = (from e in lib
                            select new IndennitaSistemazioneModel()
                            {
                                
                                idIndSist = e.IDINDSIST,
                                idTipoTrasferimento = e.IDTIPOTRASFERIMENTO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaSistemazioneModel().dataFineValidita,
                                coefficiente = e.COEFFICIENTE,
                                dataAggiornamento = System.DateTime.Now,
                                annullato = e.ANNULLATO,
                                TipoTrasferimento = new TipoTrasferimentoModel()
                                {
                                    idTipoTrasferimento = e.IDTIPOTRASFERIMENTO,
                                    descTipoTrasf = e.TIPOTRASFERIMENTO.ToString()
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

        public IList<IndennitaSistemazioneModel> getListIndennitaSistemazione(bool escludiAnnullati = false)
        {
            List<IndennitaSistemazioneModel> libm = new List<IndennitaSistemazioneModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.INDENNITASISTEMAZIONE.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new IndennitaSistemazioneModel()
                            {
                                
                                idIndSist = e.IDINDSIST,
                                idTipoTrasferimento = e.IDTIPOTRASFERIMENTO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaSistemazioneModel().dataFineValidita,
                                coefficiente = e.COEFFICIENTE,
                                dataAggiornamento = System.DateTime.Now,
                                annullato = e.ANNULLATO,
                                TipoTrasferimento = new TipoTrasferimentoModel()
                                {
                                    idTipoTrasferimento = e.IDTIPOTRASFERIMENTO,
                                    descTipoTrasf = e.TIPOTRASFERIMENTO.ToString()
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

        public IList<IndennitaSistemazioneModel> getListIndennitaSistemazione(decimal idTipoTrasferimento, bool escludiAnnullati = false)
        {
            List<IndennitaSistemazioneModel> libm = new List<IndennitaSistemazioneModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.INDENNITASISTEMAZIONE.Where(a => a.IDTIPOTRASFERIMENTO == idTipoTrasferimento && a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new IndennitaSistemazioneModel()
                            {
                                idIndSist = e.IDINDSIST,
                                idTipoTrasferimento = e.IDTIPOTRASFERIMENTO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new IndennitaSistemazioneModel().dataFineValidita,
                                coefficiente = e.COEFFICIENTE,
                                dataAggiornamento = System.DateTime.Now,
                                annullato = e.ANNULLATO,
                                TipoTrasferimento = new TipoTrasferimentoModel()
                                {
                                    idTipoTrasferimento = e.TIPOTRASFERIMENTO.IDTIPOTRASFERIMENTO,
                                    descTipoTrasf = e.TIPOTRASFERIMENTO.TIPOTRASFERIMENTO1
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
        public void SetIndennitaSistemazione(IndennitaSistemazioneModel ibm)
        {
            List<INDENNITASISTEMAZIONE> libNew = new List<INDENNITASISTEMAZIONE>();

            INDENNITASISTEMAZIONE ibNew = new INDENNITASISTEMAZIONE();

            INDENNITASISTEMAZIONE ibPrecedente = new INDENNITASISTEMAZIONE();

            List<INDENNITASISTEMAZIONE> lArchivioIB = new List<INDENNITASISTEMAZIONE>();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    if (ibm.dataFineValidita.HasValue)
                    {
                        if (EsistonoMovimentiSuccessiviUguale(ibm))
                        {
                            ibNew = new INDENNITASISTEMAZIONE()
                            {
                                
                                IDINDSIST = ibm.idIndSist,
                                IDTIPOTRASFERIMENTO = ibm.idTipoTrasferimento,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = ibm.dataFineValidita.Value,
                                COEFFICIENTE = ibm.coefficiente,
                                DATAAGGIORNAMENTO = ibm.dataAggiornamento,
                                ANNULLATO = ibm.annullato
                            };
                        }
                        else
                        {
                            ibNew = new INDENNITASISTEMAZIONE()
                            {
                                
                                IDINDSIST = ibm.idIndSist,
                                IDTIPOTRASFERIMENTO = ibm.idTipoTrasferimento,
                                DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                                COEFFICIENTE = ibm.coefficiente,
                                DATAAGGIORNAMENTO = System.DateTime.Now,
                                ANNULLATO = ibm.annullato
                            };
                        }
                    }
                    else
                    {
                        ibNew = new INDENNITASISTEMAZIONE()
                        {
                            
                            IDINDSIST = ibm.idIndSist,
                            IDTIPOTRASFERIMENTO = ibm.idTipoTrasferimento,
                            DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                            DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
                            COEFFICIENTE = ibm.coefficiente,
                            DATAAGGIORNAMENTO = System.DateTime.Now,
                            ANNULLATO = ibm.annullato
                        };
                    }

                    db.Database.BeginTransaction();

                    var recordInteressati = db.INDENNITASISTEMAZIONE.Where(a => a.ANNULLATO == false && a.IDTIPOTRASFERIMENTO == ibNew.IDTIPOTRASFERIMENTO)
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
                                    var ibOld1 = new INDENNITASISTEMAZIONE()
                                    {
                                        IDTIPOTRASFERIMENTO = item.IDTIPOTRASFERIMENTO,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        COEFFICIENTE = item.COEFFICIENTE,
                                        DATAAGGIORNAMENTO = System.DateTime.Now,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);

                                }
                                else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
                                {
                                    var ibOld1 = new INDENNITASISTEMAZIONE()
                                    {
                                        IDTIPOTRASFERIMENTO = item.IDTIPOTRASFERIMENTO,
                                        DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
                                        DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
                                        COEFFICIENTE = item.COEFFICIENTE,
                                        DATAAGGIORNAMENTO = System.DateTime.Now,
                                        ANNULLATO = false
                                    };

                                    var ibOld2 = new INDENNITASISTEMAZIONE()
                                    {
                                        IDTIPOTRASFERIMENTO = item.IDTIPOTRASFERIMENTO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(+1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        COEFFICIENTE = item.COEFFICIENTE,
                                        DATAAGGIORNAMENTO = System.DateTime.Now,
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
                                    var ibOld1 = new INDENNITASISTEMAZIONE()
                                    {
                                        IDTIPOTRASFERIMENTO = item.IDTIPOTRASFERIMENTO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        COEFFICIENTE = item.COEFFICIENTE,
                                        DATAAGGIORNAMENTO = System.DateTime.Now,
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
                                    var ibOld1 = new INDENNITASISTEMAZIONE()
                                    {
                                        IDTIPOTRASFERIMENTO = item.IDTIPOTRASFERIMENTO,
                                        DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
                                        DATAFINEVALIDITA = item.DATAFINEVALIDITA,
                                        COEFFICIENTE = item.COEFFICIENTE,
                                        DATAAGGIORNAMENTO = System.DateTime.Now,
                                        ANNULLATO = false
                                    };

                                    libNew.Add(ibOld1);
                                }
                            }
                        }

                        libNew.Add(ibNew);
                        libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        db.INDENNITASISTEMAZIONE.AddRange(libNew);
                    }
                    else
                    {
                        db.INDENNITASISTEMAZIONE.Add(ibNew);

                    }
                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro di indennità di sistemazione.", "INDENNITASISTEMAZIONE", ibNew.IDINDSIST);
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

        public bool EsistonoMovimentiPrima(IndennitaSistemazioneModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.INDENNITASISTEMAZIONE.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDTIPOTRASFERIMENTO == ibm.idTipoTrasferimento).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(IndennitaSistemazioneModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.INDENNITASISTEMAZIONE.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDTIPOTRASFERIMENTO == ibm.idTipoTrasferimento).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(IndennitaSistemazioneModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.INDENNITASISTEMAZIONE.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDTIPOTRASFERIMENTO == ibm.idTipoTrasferimento).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }



        public bool EsistonoMovimentiPrimaUguale(IndennitaSistemazioneModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.INDENNITASISTEMAZIONE.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDTIPOTRASFERIMENTO == ibm.idTipoTrasferimento).Count() > 0 ? true : false;
            }
        }

        public void DelIndennitaSistemazione(decimal idIndSist)
        {
            INDENNITASISTEMAZIONE precedenteIB = new INDENNITASISTEMAZIONE();
            INDENNITASISTEMAZIONE delIB = new INDENNITASISTEMAZIONE();


            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.INDENNITASISTEMAZIONE.Where(a => a.IDINDSIST == idIndSist);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;

                        var lprecIB = db.INDENNITASISTEMAZIONE.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

                        if (lprecIB.Count > 0)
                        {
                            precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
                            precedenteIB.ANNULLATO = true;

                            var ibOld1 = new INDENNITASISTEMAZIONE()
                            {
                                
                                IDTIPOTRASFERIMENTO = precedenteIB.IDTIPOTRASFERIMENTO,
                                DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                                DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                                COEFFICIENTE = precedenteIB.COEFFICIENTE,
                                DATAAGGIORNAMENTO = precedenteIB.DATAAGGIORNAMENTO,
                                ANNULLATO = false
                            };

                            db.INDENNITASISTEMAZIONE.Add(ibOld1);
                        }

                        db.SaveChanges();

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di indennità di sistemazione.", "INDENNITASISTEMAZIONE", idIndSist);
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
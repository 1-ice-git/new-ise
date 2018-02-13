using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtMaggFigli : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<PercMagFigliModel> getListMaggiorazioneFiglio()
        {
            List<PercMagFigliModel> libm = new List<PercMagFigliModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEMAGFIGLI.ToList();

                    libm = (from e in lib
                            select new PercMagFigliModel()
                            {
                                idPercMagFigli = e.IDPERCMAGFIGLI,
                                idTipologiaFiglio = e.IDTIPOLOGIAFIGLIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                percentualeFigli = e.PERCENTUALEFIGLI,
                                annullato = e.ANNULLATO,
                                Figlio = new TipologiaFiglioModel()
                                {
                                    idTipologiaFiglio = e.TIPOLOGIAFIGLIO.IDTIPOLOGIAFIGLIO,
                                    tipologiaFiglio = e.TIPOLOGIAFIGLIO.TIPOLOGIAFIGLIO1
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

        public IList<PercMagFigliModel> getListMaggiorazioneFiglio(decimal idTipologiaFiglio)
        {
            List<PercMagFigliModel> libm = new List<PercMagFigliModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEMAGFIGLI.Where(a => a.IDTIPOLOGIAFIGLIO == idTipologiaFiglio).ToList();

                    libm = (from e in lib
                            select new PercMagFigliModel()
                            {

                                idPercMagFigli = e.IDPERCMAGFIGLI,
                                idTipologiaFiglio = e.IDTIPOLOGIAFIGLIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                percentualeFigli = e.PERCENTUALEFIGLI,
                                annullato = e.ANNULLATO,
                                Figlio = new TipologiaFiglioModel()
                                {
                                    idTipologiaFiglio = e.TIPOLOGIAFIGLIO.IDTIPOLOGIAFIGLIO,
                                    tipologiaFiglio = e.TIPOLOGIAFIGLIO.TIPOLOGIAFIGLIO1
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

        public IList<PercMagFigliModel> getListMaggiorazioneFiglio(bool escludiAnnullati = false)
        {
            List<PercMagFigliModel> libm = new List<PercMagFigliModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEMAGFIGLI.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new PercMagFigliModel()
                            {

                                idPercMagFigli = e.IDPERCMAGFIGLI,
                                idTipologiaFiglio = (decimal)e.IDTIPOLOGIAFIGLIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                percentualeFigli = e.PERCENTUALEFIGLI,
                                annullato = e.ANNULLATO,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                Figlio = new TipologiaFiglioModel()
                                {
                                    idTipologiaFiglio = e.TIPOLOGIAFIGLIO.IDTIPOLOGIAFIGLIO,
                                    tipologiaFiglio = e.TIPOLOGIAFIGLIO.TIPOLOGIAFIGLIO1
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

        public IList<PercMagFigliModel> getListMaggiorazioneFiglio(decimal idTipologiaFiglio, bool escludiAnnullati = false)
        {
            List<PercMagFigliModel> libm = new List<PercMagFigliModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<PERCENTUALEMAGFIGLI> lib = new List<PERCENTUALEMAGFIGLI>();
                    if (escludiAnnullati == true)
                        lib = db.PERCENTUALEMAGFIGLI.Where(a => a.IDTIPOLOGIAFIGLIO == idTipologiaFiglio && a.ANNULLATO == false).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                    else
                        lib = db.PERCENTUALEMAGFIGLI.Where(a => a.IDTIPOLOGIAFIGLIO == idTipologiaFiglio).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();

                    libm = (from e in lib
                            select new PercMagFigliModel()
                            {
                                idPercMagFigli = e.IDPERCMAGFIGLI,
                                idTipologiaFiglio = e.IDTIPOLOGIAFIGLIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                percentualeFigli = e.PERCENTUALEFIGLI,
                                annullato = e.ANNULLATO,
                                dataAggiornamento=e.DATAAGGIORNAMENTO,
                                Figlio = new TipologiaFiglioModel()
                                {
                                    idTipologiaFiglio = e.TIPOLOGIAFIGLIO.IDTIPOLOGIAFIGLIO,
                                    tipologiaFiglio = e.TIPOLOGIAFIGLIO.TIPOLOGIAFIGLIO1
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
        //public void SetMaggiorazioneFiglio(PercMagFigliModel ibm)
        //{
        //    List<PERCENTUALEMAGFIGLI> libNew = new List<PERCENTUALEMAGFIGLI>();

        //    PERCENTUALEMAGFIGLI ibNew = new PERCENTUALEMAGFIGLI();

        //    PERCENTUALEMAGFIGLI ibPrecedente = new PERCENTUALEMAGFIGLI();

        //    List<PERCENTUALEMAGFIGLI> lArchivioIB = new List<PERCENTUALEMAGFIGLI>();

        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        try
        //        {
        //            if (ibm.dataFineValidita.HasValue)
        //            {
        //                if (EsistonoMovimentiSuccessiviUguale(ibm))
        //                {
        //                    ibNew = new PERCENTUALEMAGFIGLI()
        //                    {
        //                        IDPERCMAGFIGLI = ibm.idPercMagFigli,
        //                        IDTIPOLOGIAFIGLIO = ibm.idTipologiaFiglio,
        //                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
        //                        DATAFINEVALIDITA = ibm.dataFineValidita.Value,
        //                        PERCENTUALEFIGLI = ibm.percentualeFigli,
        //                        DATAAGGIORNAMENTO = ibm.dataAggiornamento,
        //                        ANNULLATO = ibm.annullato
        //                    };
        //                }
        //                else
        //                {
        //                    ibNew = new PERCENTUALEMAGFIGLI()
        //                    {
        //                        IDPERCMAGFIGLI = ibm.idPercMagFigli,
        //                        IDTIPOLOGIAFIGLIO = ibm.idTipologiaFiglio,
        //                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
        //                        DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
        //                        PERCENTUALEFIGLI = ibm.percentualeFigli,
        //                        DATAAGGIORNAMENTO = ibm.dataAggiornamento,
        //                        ANNULLATO = ibm.annullato
        //                    };
        //                }
        //            }
        //            else
        //            {
        //                ibNew = new PERCENTUALEMAGFIGLI()
        //                {
        //                    IDPERCMAGFIGLI = ibm.idPercMagFigli,
        //                    IDTIPOLOGIAFIGLIO = ibm.idTipologiaFiglio,
        //                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
        //                    DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
        //                    PERCENTUALEFIGLI = ibm.percentualeFigli,
        //                    DATAAGGIORNAMENTO = ibm.dataAggiornamento,
        //                    ANNULLATO = ibm.annullato
        //                };
        //            }

        //            db.Database.BeginTransaction();

        //            var recordInteressati = db.PERCENTUALEMAGFIGLI.Where(a => a.ANNULLATO == false && a.IDTIPOLOGIAFIGLIO == ibNew.IDTIPOLOGIAFIGLIO)
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
        //                            var ibOld1 = new PERCENTUALEMAGFIGLI()
        //                            {
        //                                IDPERCMAGFIGLI = item.IDPERCMAGFIGLI,
        //                                IDTIPOLOGIAFIGLIO = item.IDTIPOLOGIAFIGLIO,
        //                                DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
        //                                DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
        //                                PERCENTUALEFIGLI = item.PERCENTUALEFIGLI,
        //                                DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
        //                                ANNULLATO = false
        //                            };

        //                            libNew.Add(ibOld1);

        //                        }
        //                        else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
        //                        {
        //                            var ibOld1 = new PERCENTUALEMAGFIGLI()
        //                            {
        //                                IDPERCMAGFIGLI = item.IDPERCMAGFIGLI,
        //                                IDTIPOLOGIAFIGLIO = item.IDTIPOLOGIAFIGLIO,
        //                                DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
        //                                DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
        //                                PERCENTUALEFIGLI = item.PERCENTUALEFIGLI,
        //                                DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
        //                                ANNULLATO = false
        //                            };

        //                            var ibOld2 = new PERCENTUALEMAGFIGLI()
        //                            {
        //                                IDPERCMAGFIGLI = item.IDPERCMAGFIGLI,
        //                                IDTIPOLOGIAFIGLIO = item.IDTIPOLOGIAFIGLIO,
        //                                DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(+1),
        //                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,
        //                                PERCENTUALEFIGLI = item.PERCENTUALEFIGLI,
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
        //                            var ibOld1 = new PERCENTUALEMAGFIGLI()
        //                            {
        //                                IDPERCMAGFIGLI = item.IDPERCMAGFIGLI,
        //                                IDTIPOLOGIAFIGLIO = item.IDTIPOLOGIAFIGLIO,
        //                                DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
        //                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,
        //                                PERCENTUALEFIGLI = item.PERCENTUALEFIGLI,
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
        //                            var ibOld1 = new PERCENTUALEMAGFIGLI()
        //                            {
        //                                IDPERCMAGFIGLI = item.IDPERCMAGFIGLI,
        //                                IDTIPOLOGIAFIGLIO = item.IDTIPOLOGIAFIGLIO,
        //                                DATAINIZIOVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(1),
        //                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,
        //                                PERCENTUALEFIGLI = item.PERCENTUALEFIGLI,
        //                                DATAAGGIORNAMENTO = item.DATAAGGIORNAMENTO,
        //                                ANNULLATO = false
        //                            };

        //                            libNew.Add(ibOld1);
        //                        }
        //                    }
        //                }

        //                libNew.Add(ibNew);
        //                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

        //                db.PERCENTUALEMAGFIGLI.AddRange(libNew);
        //            }
        //            else
        //            {
        //                db.PERCENTUALEMAGFIGLI.Add(ibNew);

        //            }
        //            db.SaveChanges();

        //            using (objLogAttivita log = new objLogAttivita())
        //            {
        //                log.Log(enumAttivita.Inserimento, "Inserimento parametro per la percentuale di maggiorazione figli.", "PERCENTUALEMAGFIGLI", ibNew.IDPERCMAGFIGLI);
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

        public bool EsistonoMovimentiPrima(PercMagFigliModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEMAGFIGLI.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDTIPOLOGIAFIGLIO == (decimal)ibm.idTipologiaFiglio).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(PercMagFigliModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.PERCENTUALEMAGFIGLI.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDTIPOLOGIAFIGLIO == (decimal)ibm.idTipologiaFiglio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(PercMagFigliModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.PERCENTUALEMAGFIGLI.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDTIPOLOGIAFIGLIO == (decimal)ibm.idTipologiaFiglio).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiPrimaUguale(PercMagFigliModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEMAGFIGLI.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDTIPOLOGIAFIGLIO == (decimal)ibm.idTipologiaFiglio).Count() > 0 ? true : false;
            }
        }

        //public void DelMaggiorazioneFiglio(decimal idPercMagFigli)
        //{
        //    PERCENTUALEMAGFIGLI precedenteIB = new PERCENTUALEMAGFIGLI();
        //    PERCENTUALEMAGFIGLI delIB = new PERCENTUALEMAGFIGLI();


        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        try
        //        {
        //            db.Database.BeginTransaction();

        //            var lib = db.PERCENTUALEMAGFIGLI.Where(a => a.IDPERCMAGFIGLI == idPercMagFigli);

        //            if (lib.Count() > 0)
        //            {
        //                delIB = lib.First();
        //                delIB.ANNULLATO = true;

        //                var lprecIB = db.PERCENTUALEMAGFIGLI.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

        //                if (lprecIB.Count > 0)
        //                {
        //                    precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
        //                    precedenteIB.ANNULLATO = true;

        //                    var ibOld1 = new PERCENTUALEMAGFIGLI()
        //                    {
        //                        IDPERCMAGFIGLI = precedenteIB.IDPERCMAGFIGLI,
        //                        IDTIPOLOGIAFIGLIO = precedenteIB.IDTIPOLOGIAFIGLIO,
        //                        DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
        //                        DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
        //                        PERCENTUALEFIGLI = precedenteIB.PERCENTUALEFIGLI,
        //                        DATAAGGIORNAMENTO = precedenteIB.DATAAGGIORNAMENTO,
        //                        ANNULLATO = false
        //                    };

        //                    db.PERCENTUALEMAGFIGLI.Add(ibOld1);
        //                }

        //                db.SaveChanges();

        //                using (objLogAttivita log = new objLogAttivita())
        //                {
        //                    log.Log(enumAttivita.Eliminazione, "Eliminazione parametro per la percentuale di maggiorazione figlio.", "PERCENTUALEMAGFIGLI", idPercMagFigli);
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
        
        public bool PercMaggiorazioneFiglioAnnullato(PercMagFigliModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEMAGFIGLI.Where(a => a.IDPERCMAGFIGLI == ibm.idPercMagFigli).First().ANNULLATO == true ? true : false;
            }
        }
       
        public decimal Get_Id_PercentualFiglioPrimoNonAnnullato(decimal idTipologiaFiglio)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEMAGFIGLI> libm = new List<PERCENTUALEMAGFIGLI>();
                libm = db.PERCENTUALEMAGFIGLI.Where(a => a.ANNULLATO == false
                && a.IDTIPOLOGIAFIGLIO == idTipologiaFiglio).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDPERCMAGFIGLI;
            }
            return tmp;
        }

        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as PercMagFigliModel;
            if (fm != null)
            {
                DateTime d = DataInizioMinimaNonAnnullata(Convert.ToDecimal(fm.idTipologiaFiglio));
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
        public static ValidationResult VerificaPercentualeFiglio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as PercMagFigliModel;

            if (fm != null)
            {
                if (fm.percentualeFigli > 100)
                {
                    vr = new ValidationResult(string.Format("Impossibile inserire percentuale maggiore di 100 ({0}).", fm.percentualeFigli.ToString()));
                }
                else
                {
                    vr = ValidationResult.Success;
                }
            }
            else
            {
                vr = new ValidationResult("La percentuale è richiesta.");
            }
            return vr;
        }
        public static DateTime DataInizioMinimaNonAnnullata(decimal idLivello)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.PERCENTUALEMAGFIGLI.Where(a => a.ANNULLATO == false && a.IDTIPOLOGIAFIGLIO == idLivello).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }


        ///////////////////////////////
        public bool PercMaggiorazioneFiglioAnnullato(PercentualeMagFigliModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEMAGFIGLI.Where(a => a.IDPERCMAGFIGLI == ibm.idPercMagFigli).First().ANNULLATO == true ? true : false;
            }
        }
        public decimal Get_Id_PercentualMagFiglioPrimoNonAnnullato(decimal idTipologiaFiglio)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEMAGFIGLI> libm = new List<PERCENTUALEMAGFIGLI>();
                libm = db.PERCENTUALEMAGFIGLI.Where(a => a.ANNULLATO == false
                && a.IDTIPOLOGIAFIGLIO == idTipologiaFiglio).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDPERCMAGFIGLI;
            }
            return tmp;
        }

        public PERCENTUALEMAGFIGLI RestituisciIlRecordPrecedente(decimal idMagCon)
        {
            PERCENTUALEMAGFIGLI tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                PERCENTUALEMAGFIGLI interessato = new PERCENTUALEMAGFIGLI();
                interessato = db.PERCENTUALEMAGFIGLI.Find(idMagCon);
                tmp = db.PERCENTUALEMAGFIGLI.Where(a => a.IDTIPOLOGIAFIGLIO == interessato.IDTIPOLOGIAFIGLIO
                && a.ANNULLATO == false).ToList().Where(b => b.DATAFINEVALIDITA == interessato.DATAINIZIOVALIDITA.AddDays(-1)).ToList().First();
            }
            return tmp;
        }
        public static DateTime DataInizioMinimaNonAnnullataMagFiglio(decimal idLivello)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.PERCENTUALEMAGFIGLI.Where(a => a.ANNULLATO == false && a.IDTIPOLOGIAFIGLIO == idLivello).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }

        public decimal Get_Id_PERCENTUALEMAGFIGLIPrimoNonAnnullato(decimal IDTIPOLOGIAFIGLIO)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEMAGFIGLI> libm = new List<PERCENTUALEMAGFIGLI>();
                libm = db.PERCENTUALEMAGFIGLI.Where(a => a.ANNULLATO == false
                && a.IDTIPOLOGIAFIGLIO == IDTIPOLOGIAFIGLIO).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDPERCMAGFIGLI;
            }
            return tmp;
        }

        public List<string> RestituisciIntervalloDiUnaData(DateTime DataCampione, decimal IDTIPOLOGIAFIGLIO)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEMAGFIGLI> libm = new List<PERCENTUALEMAGFIGLI>();
                libm = db.PERCENTUALEMAGFIGLI.Where(a => a.ANNULLATO == false
                && a.IDTIPOLOGIAFIGLIO == IDTIPOLOGIAFIGLIO).ToList().Where(b =>
                b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())
                && DataCampione > b.DATAINIZIOVALIDITA
                && DataCampione < b.DATAFINEVALIDITA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCMAGFIGLI.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALEFIGLI.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione, decimal IDTIPOLOGIAFIGLIO)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEMAGFIGLI> libm = new List<PERCENTUALEMAGFIGLI>();
                libm = db.PERCENTUALEMAGFIGLI.Where(a => a.ANNULLATO == false
                && a.IDTIPOLOGIAFIGLIO == IDTIPOLOGIAFIGLIO).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().Where(b => DataCampione == b.DATAINIZIOVALIDITA &&
                 b.DATAFINEVALIDITA != Utility.DataFineStop()).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCMAGFIGLI.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALEFIGLI.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione, decimal IDTIPOLOGIAFIGLIO)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEMAGFIGLI> libm = new List<PERCENTUALEMAGFIGLI>();
                libm = db.PERCENTUALEMAGFIGLI.Where(a => a.ANNULLATO == false
                && a.IDTIPOLOGIAFIGLIO == IDTIPOLOGIAFIGLIO).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().
                Where(b => DataCampione == b.DATAFINEVALIDITA
                && b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())).ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCMAGFIGLI.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALEFIGLI.ToString());
                }
            }
            return tmp;
        }
        public List<string> RestituisciLaRigaMassima(decimal IDTIPOLOGIAFIGLIO)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEMAGFIGLI> libm = new List<PERCENTUALEMAGFIGLI>();
                libm = db.PERCENTUALEMAGFIGLI.Where(a => a.ANNULLATO == false
                && a.IDTIPOLOGIAFIGLIO == IDTIPOLOGIAFIGLIO).ToList().Where(b =>
                b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop())).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCMAGFIGLI.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALEFIGLI.ToString());
                }
            }
            return tmp;
        }
        public void RendiAnnullatoUnRecord(decimal idPercMagCon, ModelDBISE db)
        {
            PERCENTUALEMAGFIGLI entita = new PERCENTUALEMAGFIGLI();
            entita = db.PERCENTUALEMAGFIGLI.Find(idPercMagCon);
            entita.ANNULLATO = true;
            db.SaveChanges();
        }

        public void DelMaggiorazioneFiglio(decimal idMagCon)
        {
            PERCENTUALEMAGFIGLI precedenteIB = new PERCENTUALEMAGFIGLI();
            PERCENTUALEMAGFIGLI delIB = new PERCENTUALEMAGFIGLI();
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    var lib = db.PERCENTUALEMAGFIGLI.Where(a => a.IDPERCMAGFIGLI == idMagCon);
                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;
                        RendiAnnullatoUnRecord(delIB.IDPERCMAGFIGLI, db);
                        precedenteIB = RestituisciIlRecordPrecedente(idMagCon);
                        RendiAnnullatoUnRecord(precedenteIB.IDPERCMAGFIGLI, db);

                        var NuovoPrecedente = new PERCENTUALEMAGFIGLI()
                        {
                            IDTIPOLOGIAFIGLIO = precedenteIB.IDTIPOLOGIAFIGLIO,
                            DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                            //ALIQUOTA = precedenteIB.ALIQUOTA,
                            PERCENTUALEFIGLI = precedenteIB.PERCENTUALEFIGLI,
                            DATAAGGIORNAMENTO = DateTime.Now,// precedenteIB.DATAAGGIORNAMENTO,
                            ANNULLATO = false
                        };
                        db.PERCENTUALEMAGFIGLI.Add(NuovoPrecedente);
                    }
                    db.SaveChanges();
                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di Percentuale Maggiorazione figli.", "PERCENTUALEFIGLI", idMagCon);
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

        public void SetMaggiorazioneFiglio(PercMagFigliModel ibm, bool aggiornaTutto)
        {
            List<PERCENTUALEMAGFIGLI> libNew = new List<PERCENTUALEMAGFIGLI>();
            PERCENTUALEMAGFIGLI ibPrecedente = new PERCENTUALEMAGFIGLI();
            PERCENTUALEMAGFIGLI ibNew1 = new PERCENTUALEMAGFIGLI();
            PERCENTUALEMAGFIGLI ibNew2 = new PERCENTUALEMAGFIGLI();
            List<PERCENTUALEMAGFIGLI> lArchivioIB = new List<PERCENTUALEMAGFIGLI>();
            List<string> lista = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                bool giafatta = false;
                try
                {
                    using (dtMaggFigli dtal = new dtMaggFigli())
                    {
                        //Se la data variazione coincide con una data inizio esistente
                        lista = dtal.DataVariazioneCoincideConDataInizio(ibm.dataInizioValidita, Convert.ToDecimal(ibm.idTipologiaFiglio));
                        if (lista.Count != 0)
                        {
                            giafatta = true;
                            decimal idIntervalloFirst = Convert.ToDecimal(lista[0]);
                            DateTime dataInizioFirst = Convert.ToDateTime(lista[1]);
                            DateTime dataFineFirst = Convert.ToDateTime(lista[2]);
                            decimal percConiugeFirst = Convert.ToDecimal(lista[3]);

                            ibNew1 = new PERCENTUALEMAGFIGLI()
                            {
                                IDTIPOLOGIAFIGLIO = Convert.ToDecimal(ibm.idTipologiaFiglio),
                                DATAINIZIOVALIDITA = dataInizioFirst,
                                DATAFINEVALIDITA = dataFineFirst,
                                // ALIQUOTA = ibm.aliquota,
                                PERCENTUALEFIGLI = ibm.percentualeFigli,
                                DATAAGGIORNAMENTO = DateTime.Now,
                            };

                            if (aggiornaTutto)
                            {
                                ibNew1 = new PERCENTUALEMAGFIGLI()
                                {
                                    IDTIPOLOGIAFIGLIO = Convert.ToDecimal(ibm.idTipologiaFiglio),
                                    DATAINIZIOVALIDITA = dataInizioFirst,
                                    DATAFINEVALIDITA = Utility.DataFineStop(),
                                    // ALIQUOTA = ibm.aliquota,
                                    PERCENTUALEFIGLI = ibm.percentualeFigli,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                //qui annullo tutti i record rimanenti dalla data inizio inserita
                                libNew = db.PERCENTUALEMAGFIGLI.Where(a => a.ANNULLATO == false).ToList()
                                    .Where(a => a.DATAINIZIOVALIDITA > dataInizioFirst &&
                                    a.IDTIPOLOGIAFIGLIO == Convert.ToDecimal(ibm.idTipologiaFiglio)).ToList();
                                foreach (var elem in libNew)
                                {
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPERCMAGFIGLI), db);
                                }
                            }
                            db.Database.BeginTransaction();
                            db.PERCENTUALEMAGFIGLI.Add(ibNew1);
                            db.SaveChanges();
                            RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloFirst), db);

                            db.Database.CurrentTransaction.Commit();
                        }
                        ///se la data variazione coincide con una data fine esistente(diversa da 31/12/9999)
                        if (giafatta == false)
                        {
                            lista = dtal.DataVariazioneCoincideConDataFine(ibm.dataInizioValidita, Convert.ToDecimal(ibm.idTipologiaFiglio));
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervalloLast = Convert.ToDecimal(lista[0]);
                                DateTime dataInizioLast = Convert.ToDateTime(lista[1]);
                                DateTime dataFineLast = Convert.ToDateTime(lista[2]);
                                decimal PERCENTUALEFIGLI = Convert.ToDecimal(lista[3]);

                                ibNew1 = new PERCENTUALEMAGFIGLI()
                                {
                                    IDTIPOLOGIAFIGLIO = Convert.ToDecimal(ibm.idTipologiaFiglio),
                                    DATAINIZIOVALIDITA = dataInizioLast,
                                    DATAFINEVALIDITA = dataFineLast.AddDays(-1),
                                    PERCENTUALEFIGLI = PERCENTUALEFIGLI,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new PERCENTUALEMAGFIGLI()
                                {
                                    IDTIPOLOGIAFIGLIO = Convert.ToDecimal(ibm.idTipologiaFiglio),
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = ibm.dataInizioValidita,//è uguale alla data Inizio
                                    PERCENTUALEFIGLI = ibm.percentualeFigli,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };
                                if (aggiornaTutto)
                                {
                                    ibNew2 = new PERCENTUALEMAGFIGLI()
                                    {
                                        IDTIPOLOGIAFIGLIO = Convert.ToDecimal(ibm.idTipologiaFiglio),
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        PERCENTUALEFIGLI = ibm.percentualeFigli,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.PERCENTUALEMAGFIGLI.Where(a => a.IDTIPOLOGIAFIGLIO == Convert.ToDecimal(ibm.idTipologiaFiglio)
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPERCMAGFIGLI), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                db.Database.BeginTransaction();
                                db.PERCENTUALEMAGFIGLI.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloLast), db);
                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //Se il nuovo record si trova in un intervallo non annullato con data fine non uguale al 31/12/9999
                        if (giafatta == false)
                        {
                            lista = dtal.RestituisciIntervalloDiUnaData(ibm.dataInizioValidita, Convert.ToDecimal(ibm.idTipologiaFiglio));
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervallo = Convert.ToDecimal(lista[0]);
                                DateTime dataInizio = Convert.ToDateTime(lista[1]);
                                DateTime dataFine = Convert.ToDateTime(lista[2]);
                                decimal PERCENTUALEFIGLI = Convert.ToDecimal(lista[3]);

                                DateTime NewdataFine1 = ibm.dataInizioValidita.AddDays(-1);

                                ibNew1 = new PERCENTUALEMAGFIGLI()
                                {
                                    IDTIPOLOGIAFIGLIO = Convert.ToDecimal(ibm.idTipologiaFiglio),
                                    DATAINIZIOVALIDITA = dataInizio,
                                    DATAFINEVALIDITA = NewdataFine1,
                                    //ALIQUOTA = aliquota,
                                    PERCENTUALEFIGLI = PERCENTUALEFIGLI,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new PERCENTUALEMAGFIGLI()
                                {
                                    IDTIPOLOGIAFIGLIO = Convert.ToDecimal(ibm.idTipologiaFiglio),
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = dataFine,
                                    // ALIQUOTA = ibm.aliquota,
                                    PERCENTUALEFIGLI = ibm.percentualeFigli,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };

                                if (aggiornaTutto)
                                {
                                    ibNew2 = new PERCENTUALEMAGFIGLI()
                                    {
                                        IDTIPOLOGIAFIGLIO = Convert.ToDecimal(ibm.idTipologiaFiglio),
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        // ALIQUOTA = ibm.aliquota,
                                        PERCENTUALEFIGLI = ibm.percentualeFigli,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    decimal tmpii = Convert.ToDecimal(ibm.idTipologiaFiglio);
                                    libNew = db.PERCENTUALEMAGFIGLI.Where(a => a.IDTIPOLOGIAFIGLIO == tmpii
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPERCMAGFIGLI), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                db.Database.BeginTransaction();
                                db.PERCENTUALEMAGFIGLI.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervallo), db);
                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //CASO DELL'ULTIMA RIGA CON LA DATA FINE UGUALE A 31/12/9999
                        if (giafatta == false)
                        {
                            //Attenzione qui se la lista non contiene nessun elemento
                            //significa che non esiste nessun elemento corrispondentemente al livello selezionato
                            lista = dtal.RestituisciLaRigaMassima(Convert.ToDecimal(ibm.idTipologiaFiglio));
                            if (lista.Count == 0)
                            {
                                ibNew1 = new PERCENTUALEMAGFIGLI()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = Convert.ToDateTime(Utility.DataFineStop()),
                                    PERCENTUALEFIGLI = ibm.percentualeFigli,
                                    IDTIPOLOGIAFIGLIO = Convert.ToDecimal(ibm.idTipologiaFiglio),
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                libNew.Add(ibNew1);
                                db.Database.BeginTransaction();
                                db.PERCENTUALEMAGFIGLI.Add(ibNew1);
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
                                decimal percentualeUltimo = Convert.ToDecimal(lista[3]);

                                if (dataInizioUltimo == ibm.dataInizioValidita)
                                {
                                    ibNew1 = new PERCENTUALEMAGFIGLI()
                                    {
                                        IDTIPOLOGIAFIGLIO = Convert.ToDecimal(ibm.idTipologiaFiglio),
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = dataFineUltimo,
                                        // ALIQUOTA = ibm.aliquota,//nuova aliquota rispetto alla vecchia registrata
                                        PERCENTUALEFIGLI = ibm.percentualeFigli,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.PERCENTUALEMAGFIGLI.Add(ibNew1);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);
                                    db.Database.CurrentTransaction.Commit();
                                }
                                //se il nuovo record rappresenta la data variazione superiore alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                if (ibm.dataInizioValidita > dataInizioUltimo)
                                {
                                    ibNew1 = new PERCENTUALEMAGFIGLI()
                                    {
                                        IDTIPOLOGIAFIGLIO = Convert.ToDecimal(ibm.idTipologiaFiglio),
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = ibm.dataInizioValidita.AddDays(-1),
                                        PERCENTUALEFIGLI = percentualeUltimo,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    ibNew2 = new PERCENTUALEMAGFIGLI()
                                    {
                                        IDTIPOLOGIAFIGLIO = Convert.ToDecimal(ibm.idTipologiaFiglio),
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        PERCENTUALEFIGLI = ibm.percentualeFigli,//nuova aliquota rispetto alla vecchia registrata
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1); libNew.Add(ibNew2);
                                    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                    db.Database.BeginTransaction();
                                    db.PERCENTUALEMAGFIGLI.AddRange(libNew);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);
                                    db.Database.CurrentTransaction.Commit();
                                }
                            }
                        }
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
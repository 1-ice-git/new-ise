using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.dtObj;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtRiduzioni : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        //RiduzioniAnnullate
        public bool RiduzioniAnnullate(RiduzioniModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.RIDUZIONI.Where(a => a.IDRIDUZIONI == ibm.idRiduzioni).First().ANNULLATO == true ? true : false;
            }
        }
        public IList<RiduzioniModel> getListRiduzioni()
        {
            List<RiduzioniModel> libm = new List<RiduzioniModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.RIDUZIONI.ToList();

                    libm = (from e in lib
                            select new RiduzioniModel()
                            {
                                idRiduzioni = e.IDRIDUZIONI,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                percentuale = e.PERCENTUALE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                idFunzioneRiduzione = e.IDFUNZIONERIDUZIONE
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<FunzioneRiduzioneModel> GetFunzioniRiduzione()
        {
            List<FunzioneRiduzioneModel> libm = new List<FunzioneRiduzioneModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.FUNZIONERIDUZIONE.ToList();

                    libm = (from e in lib
                            select new FunzioneRiduzioneModel()
                            {
                                idFunzioneRiduzione = e.IDFUNZIONERIDUZIONE,
                                DescFunzione = e.DESCFUNZIONE
                            }).ToList();
                }
                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<RiduzioniModel> getListRiduzioni(decimal idFunzioneRiduzione, bool escludiAnnullati = false)
        {
            List<RiduzioniModel> libm = new List<RiduzioniModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<RIDUZIONI> lib = new List<RIDUZIONI>();
                    if (escludiAnnullati == true)
                        lib = db.RIDUZIONI.Where(a => a.ANNULLATO == false &&
                         a.IDFUNZIONERIDUZIONE == idFunzioneRiduzione).ToList();
                    else
                        lib = db.RIDUZIONI.Where(a => a.IDFUNZIONERIDUZIONE == idFunzioneRiduzione).ToList();

                    libm = (from e in lib
                            select new RiduzioniModel()
                            {
                                idRiduzioni = e.IDRIDUZIONI,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                percentuale = e.PERCENTUALE,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                annullato = e.ANNULLATO,
                                idFunzioneRiduzione = e.IDFUNZIONERIDUZIONE
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
        //public void SetRiduzioni(RiduzioniModel ibm)
        //{
        //    List<RIDUZIONI> libNew = new List<RIDUZIONI>();

        //    RIDUZIONI ibNew = new RIDUZIONI();

        //    RIDUZIONI ibPrecedente = new RIDUZIONI();

        //    List<RIDUZIONI> lArchivioIB = new List<RIDUZIONI>();

        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        try
        //        {
        //            if (ibm.dataFineValidita.HasValue)
        //            {
        //                if (EsistonoMovimentiSuccessiviUguale(ibm))
        //                {
        //                    ibNew = new RIDUZIONI()
        //                    {

        //                        IDRIDUZIONI = ibm.idRiduzioni,
        //                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
        //                        DATAFINEVALIDITA = ibm.dataFineValidita.Value,
        //                        PERCENTUALE = ibm.percentuale,
        //                        DATAAGGIORNAMENTO = ibm.dataAggiornamento,
        //                        ANNULLATO = ibm.annullato
        //                    };
        //                }
        //                else
        //                {
        //                    ibNew = new RIDUZIONI()
        //                    {

        //                        IDRIDUZIONI = ibm.idRiduzioni,
        //                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
        //                        DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
        //                        PERCENTUALE = ibm.percentuale,
        //                        DATAAGGIORNAMENTO = System.DateTime.Now,
        //                        ANNULLATO = ibm.annullato
        //                    };
        //                }
        //            }
        //            else
        //            {
        //                ibNew = new RIDUZIONI()
        //                {

        //                    IDRIDUZIONI = ibm.idRiduzioni,
        //                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
        //                    DATAFINEVALIDITA = Convert.ToDateTime("31/12/9999"),
        //                    PERCENTUALE = ibm.percentuale,
        //                    DATAAGGIORNAMENTO = System.DateTime.Now,
        //                    ANNULLATO = ibm.annullato
        //                };
        //            }

        //            db.Database.BeginTransaction();

        //            var recordInteressati = db.RIDUZIONI.Where(a => a.ANNULLATO == false)
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
        //                            var ibOld1 = new RIDUZIONI()
        //                            {
        //                                IDRIDUZIONI = item.IDRIDUZIONI,
        //                                DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
        //                                DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
        //                                PERCENTUALE = item.PERCENTUALE,
        //                                DATAAGGIORNAMENTO = System.DateTime.Now,
        //                                ANNULLATO = false
        //                            };

        //                            libNew.Add(ibOld1);

        //                        }
        //                        else if (item.DATAFINEVALIDITA > ibNew.DATAFINEVALIDITA)
        //                        {
        //                            var ibOld1 = new RIDUZIONI()
        //                            {

        //                                IDRIDUZIONI = item.IDRIDUZIONI,
        //                                DATAINIZIOVALIDITA = item.DATAINIZIOVALIDITA,
        //                                DATAFINEVALIDITA = (ibNew.DATAINIZIOVALIDITA).AddDays(-1),
        //                                PERCENTUALE = item.PERCENTUALE,
        //                                DATAAGGIORNAMENTO = System.DateTime.Now,
        //                                ANNULLATO = false
        //                            };

        //                            var ibOld2 = new RIDUZIONI()
        //                            {
        //                                IDRIDUZIONI = item.IDRIDUZIONI,
        //                                DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(+1),
        //                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,
        //                                PERCENTUALE = item.PERCENTUALE,
        //                                DATAAGGIORNAMENTO = System.DateTime.Now,
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
        //                            var ibOld1 = new RIDUZIONI()
        //                            {

        //                                IDRIDUZIONI = item.IDRIDUZIONI,
        //                                DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
        //                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,
        //                                PERCENTUALE = item.PERCENTUALE,
        //                                DATAAGGIORNAMENTO = System.DateTime.Now,
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
        //                            var ibOld1 = new RIDUZIONI()
        //                            {

        //                                IDRIDUZIONI = item.IDRIDUZIONI,
        //                                DATAINIZIOVALIDITA = (ibNew.DATAFINEVALIDITA).AddDays(1),
        //                                DATAFINEVALIDITA = item.DATAFINEVALIDITA,
        //                                PERCENTUALE = item.PERCENTUALE,
        //                                DATAAGGIORNAMENTO = System.DateTime.Now,
        //                                ANNULLATO = false
        //                            };

        //                            libNew.Add(ibOld1);
        //                        }
        //                    }
        //                }

        //                libNew.Add(ibNew);
        //                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

        //                db.RIDUZIONI.AddRange(libNew);
        //            }
        //            else
        //            {
        //                db.RIDUZIONI.Add(ibNew);

        //            }
        //            db.SaveChanges();

        //            using (objLogAttivita log = new objLogAttivita())
        //            {
        //                log.Log(enumAttivita.Inserimento, "Inserimento parametro di riduzione.", "RIDUZIONI", ibNew.IDRIDUZIONI);
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

        public bool EsistonoMovimentiPrima(RiduzioniModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.RIDUZIONI.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(RiduzioniModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.RIDUZIONI.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(RiduzioniModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.RIDUZIONI.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }



        public bool EsistonoMovimentiPrimaUguale(RiduzioniModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.RIDUZIONI.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita).Count() > 0 ? true : false;
            }
        }

        public decimal Get_Id_RiduzionePrimoNonAnnullato(decimal idFunzioneRiduzione)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<RIDUZIONI> libm = new List<RIDUZIONI>();
                libm = db.RIDUZIONI.Where(a => a.ANNULLATO == false
                && a.IDFUNZIONERIDUZIONE == idFunzioneRiduzione).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDRIDUZIONI;
            }
            return tmp;
        }
        //
        public static ValidationResult VerificaPercentualeRiduzione(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as RiduzioniModel;

            if (fm != null)
            {
                if (fm.percentuale > 100)
                {
                    vr = new ValidationResult(string.Format("Impossibile inserire percentuale maggiore di 100 ({0}).", fm.percentuale.ToString()));
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
                var TuttiNonAnnullati = db.RIDUZIONI.Where(a => a.ANNULLATO == false && a.IDFUNZIONERIDUZIONE == idLivello).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }
        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as RiduzioniModel;
            if (fm != null)
            {
                DateTime d = DataInizioMinimaNonAnnullata(fm.idFunzioneRiduzione);
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

        public decimal Get_Id_PercentualMagFiglioPrimoNonAnnullato(decimal idFunzioneRiduzione)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<RIDUZIONI> libm = new List<RIDUZIONI>();
                libm = db.RIDUZIONI.Where(a => a.ANNULLATO == false
                && a.IDFUNZIONERIDUZIONE == idFunzioneRiduzione).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDRIDUZIONI;
            }
            return tmp;
        }
        public RIDUZIONI RestituisciIlRecordPrecedente(decimal idMagCon)
        {
            RIDUZIONI tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                RIDUZIONI interessato = new RIDUZIONI();
                interessato = db.RIDUZIONI.Find(idMagCon);
                tmp = db.RIDUZIONI.Where(a => a.IDFUNZIONERIDUZIONE == interessato.IDFUNZIONERIDUZIONE
                && a.ANNULLATO == false).ToList().Where(b => b.DATAFINEVALIDITA == interessato.DATAINIZIOVALIDITA.AddDays(-1)).ToList().First();
            }
            return tmp;
        }
        public static DateTime DataInizioMinimaNonAnnullataMagFiglio(decimal idLivello)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.RIDUZIONI.Where(a => a.ANNULLATO == false && a.IDFUNZIONERIDUZIONE == idLivello).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }
        public List<string> RestituisciIntervalloDiUnaData(DateTime DataCampione, decimal IDFUNZIONERIDUZIONE)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<RIDUZIONI> libm = new List<RIDUZIONI>();
                libm = db.RIDUZIONI.Where(a => a.ANNULLATO == false
                && a.IDFUNZIONERIDUZIONE == IDFUNZIONERIDUZIONE).ToList().Where(b =>
                b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())
                && DataCampione > b.DATAINIZIOVALIDITA
                && DataCampione < b.DATAFINEVALIDITA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDRIDUZIONI.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALE.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione, decimal IDFUNZIONERIDUZIONE)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<RIDUZIONI> libm = new List<RIDUZIONI>();
                libm = db.RIDUZIONI.Where(a => a.ANNULLATO == false
                && a.IDFUNZIONERIDUZIONE == IDFUNZIONERIDUZIONE).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().Where(b => DataCampione == b.DATAINIZIOVALIDITA &&
                 b.DATAFINEVALIDITA != Utility.DataFineStop()).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDRIDUZIONI.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALE.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione, decimal IDFUNZIONERIDUZIONE)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<RIDUZIONI> libm = new List<RIDUZIONI>();
                libm = db.RIDUZIONI.Where(a => a.ANNULLATO == false
                && a.IDFUNZIONERIDUZIONE == IDFUNZIONERIDUZIONE).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().
                Where(b => DataCampione == b.DATAFINEVALIDITA
                && b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())).ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDRIDUZIONI.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALE.ToString());
                }
            }
            return tmp;
        }
        public List<string> RestituisciLaRigaMassima(decimal IDFUNZIONERIDUZIONE)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<RIDUZIONI> libm = new List<RIDUZIONI>();
                libm = db.RIDUZIONI.Where(a => a.ANNULLATO == false
                && a.IDFUNZIONERIDUZIONE == IDFUNZIONERIDUZIONE).ToList().Where(b =>
                b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop())).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDRIDUZIONI.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALE.ToString());
                }
            }
            return tmp;
        }
        public void RendiAnnullatoUnRecord(decimal idPercMagCon, ModelDBISE db)
        {
            RIDUZIONI entita = new RIDUZIONI();
            entita = db.RIDUZIONI.Find(idPercMagCon);
            entita.ANNULLATO = true;
            db.SaveChanges();
        }
        public void DelRiduzioni(decimal idMagCon)
        {
            RIDUZIONI precedenteIB = new RIDUZIONI();
            RIDUZIONI delIB = new RIDUZIONI();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    var lib = db.RIDUZIONI.Where(a => a.IDRIDUZIONI == idMagCon);
                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;
                        RendiAnnullatoUnRecord(delIB.IDRIDUZIONI, db);
                        precedenteIB = RestituisciIlRecordPrecedente(idMagCon);
                        RendiAnnullatoUnRecord(precedenteIB.IDRIDUZIONI, db);

                        var NuovoPrecedente = new RIDUZIONI()
                        {
                            IDFUNZIONERIDUZIONE = precedenteIB.IDFUNZIONERIDUZIONE,
                            DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                            //ALIQUOTA = precedenteIB.ALIQUOTA,
                            PERCENTUALE = precedenteIB.PERCENTUALE,
                            DATAAGGIORNAMENTO = DateTime.Now,// precedenteIB.DATAAGGIORNAMENTO,
                            ANNULLATO = false
                        };
                        db.RIDUZIONI.Add(NuovoPrecedente);
                        db.SaveChanges();

                        using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                        {
                            dtrp.AssociaCoefficienteRichiamo_Riduzioni(NuovoPrecedente.IDRIDUZIONI, db,delIB.DATAINIZIOVALIDITA);
                            dtrp.AssociaIndennitaBase_Riduzioni(NuovoPrecedente.IDRIDUZIONI, db, delIB.DATAINIZIOVALIDITA);
                            dtrp.AssociaIndennitaSistemazione_Riduzioni(NuovoPrecedente.IDRIDUZIONI, db, delIB.DATAINIZIOVALIDITA);
                        }
                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di Percentuale Maggiorazione figli.", "PERCENTUALE", idMagCon);
                        }
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
        public void SetRiduzioni(RiduzioniModel ibm, bool aggiornaTutto)
        {
            List<RIDUZIONI> libNew = new List<RIDUZIONI>();
            //RIDUZIONI ibPrecedente = new RIDUZIONI();
            RIDUZIONI ibNew1 = new RIDUZIONI();
            RIDUZIONI ibNew2 = new RIDUZIONI();
            //List<RIDUZIONI> lArchivioIB = new List<RIDUZIONI>();
            List<string> lista = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                bool giafatta = false;
                try
                {
                    using (dtRiduzioni dtal = new dtRiduzioni())
                    {
                        //Se la data variazione coincide con una data inizio esistente
                        lista = dtal.DataVariazioneCoincideConDataInizio(ibm.dataInizioValidita, Convert.ToDecimal(ibm.idFunzioneRiduzione));
                        if (lista.Count != 0)
                        {
                            giafatta = true;
                            decimal idIntervalloFirst = Convert.ToDecimal(lista[0]);
                            DateTime dataInizioFirst = Convert.ToDateTime(lista[1]);
                            DateTime dataFineFirst = Convert.ToDateTime(lista[2]);
                            //decimal percConiugeFirst = Convert.ToDecimal(lista[3]);

                            ibNew1 = new RIDUZIONI()
                            {
                                IDFUNZIONERIDUZIONE = Convert.ToDecimal(ibm.idFunzioneRiduzione),
                                DATAINIZIOVALIDITA = dataInizioFirst,
                                DATAFINEVALIDITA = dataFineFirst,
                                // ALIQUOTA = ibm.aliquota,
                                PERCENTUALE = ibm.percentuale,
                                DATAAGGIORNAMENTO = DateTime.Now,
                            };

                            if (aggiornaTutto)
                            {
                                ibNew1 = new RIDUZIONI()
                                {
                                    IDFUNZIONERIDUZIONE = Convert.ToDecimal(ibm.idFunzioneRiduzione),
                                    DATAINIZIOVALIDITA = dataInizioFirst,
                                    DATAFINEVALIDITA = Utility.DataFineStop(),
                                    // ALIQUOTA = ibm.aliquota,
                                    PERCENTUALE = ibm.percentuale,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                //qui annullo tutti i record rimanenti dalla data inizio inserita
                                libNew = db.RIDUZIONI.Where(a => a.ANNULLATO == false).ToList()
                                    .Where(a => a.DATAINIZIOVALIDITA > dataInizioFirst &&
                                    a.IDFUNZIONERIDUZIONE == Convert.ToDecimal(ibm.idFunzioneRiduzione)).ToList();
                                foreach (var elem in libNew)
                                {
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDRIDUZIONI), db);
                                }
                            }
                            db.Database.BeginTransaction();
                            db.RIDUZIONI.Add(ibNew1);
                            db.SaveChanges();

                            RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloFirst), db);

                            using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                            {
                                dtrp.AssociaCoefficienteRichiamo_Riduzioni(ibNew1.IDRIDUZIONI, db, ibm.dataInizioValidita);
                                dtrp.AssociaIndennitaBase_Riduzioni(ibNew1.IDRIDUZIONI, db, ibm.dataInizioValidita);
                                dtrp.AssociaIndennitaSistemazione_Riduzioni(ibNew1.IDRIDUZIONI, db, ibm.dataInizioValidita);
                            }

                            db.Database.CurrentTransaction.Commit();
                        }
                        ///se la data variazione coincide con una data fine esistente(diversa da 31/12/9999)
                        if (giafatta == false)
                        {
                            lista = dtal.DataVariazioneCoincideConDataFine(ibm.dataInizioValidita, Convert.ToDecimal(ibm.idFunzioneRiduzione));
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervalloLast = Convert.ToDecimal(lista[0]);
                                DateTime dataInizioLast = Convert.ToDateTime(lista[1]);
                                DateTime dataFineLast = Convert.ToDateTime(lista[2]);
                                decimal PERCENTUALE = Convert.ToDecimal(lista[3]);

                                ibNew1 = new RIDUZIONI()
                                {
                                    IDFUNZIONERIDUZIONE = Convert.ToDecimal(ibm.idFunzioneRiduzione),
                                    DATAINIZIOVALIDITA = dataInizioLast,
                                    DATAFINEVALIDITA = dataFineLast.AddDays(-1),
                                    PERCENTUALE = PERCENTUALE,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new RIDUZIONI()
                                {
                                    IDFUNZIONERIDUZIONE = Convert.ToDecimal(ibm.idFunzioneRiduzione),
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = ibm.dataInizioValidita,//è uguale alla data Inizio
                                    PERCENTUALE = ibm.percentuale,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };
                                if (aggiornaTutto)
                                {
                                    ibNew2 = new RIDUZIONI()
                                    {
                                        IDFUNZIONERIDUZIONE = Convert.ToDecimal(ibm.idFunzioneRiduzione),
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        PERCENTUALE = ibm.percentuale,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.RIDUZIONI.Where(a => a.IDFUNZIONERIDUZIONE == Convert.ToDecimal(ibm.idFunzioneRiduzione)
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDRIDUZIONI), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                db.Database.BeginTransaction();
                                db.RIDUZIONI.AddRange(libNew);
                                db.SaveChanges();

                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloLast), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var r in libNew)
                                    {

                                        dtrp.AssociaCoefficienteRichiamo_Riduzioni(r.IDRIDUZIONI, db, ibm.dataInizioValidita);
                                        dtrp.AssociaIndennitaBase_Riduzioni(r.IDRIDUZIONI, db, ibm.dataInizioValidita);
                                        dtrp.AssociaIndennitaSistemazione_Riduzioni(r.IDRIDUZIONI, db, ibm.dataInizioValidita);
                                    }
                                }

                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //Se il nuovo record si trova in un intervallo non annullato con data fine non uguale al 31/12/9999
                        if (giafatta == false)
                        {
                            lista = dtal.RestituisciIntervalloDiUnaData(ibm.dataInizioValidita, Convert.ToDecimal(ibm.idFunzioneRiduzione));
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervallo = Convert.ToDecimal(lista[0]);
                                DateTime dataInizio = Convert.ToDateTime(lista[1]);
                                DateTime dataFine = Convert.ToDateTime(lista[2]);
                                decimal PERCENTUALE = Convert.ToDecimal(lista[3]);

                                DateTime NewdataFine1 = ibm.dataInizioValidita.AddDays(-1);

                                ibNew1 = new RIDUZIONI()
                                {
                                    IDFUNZIONERIDUZIONE = Convert.ToDecimal(ibm.idFunzioneRiduzione),
                                    DATAINIZIOVALIDITA = dataInizio,
                                    DATAFINEVALIDITA = NewdataFine1,
                                    //ALIQUOTA = aliquota,
                                    PERCENTUALE = PERCENTUALE,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new RIDUZIONI()
                                {
                                    IDFUNZIONERIDUZIONE = Convert.ToDecimal(ibm.idFunzioneRiduzione),
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = dataFine,
                                    // ALIQUOTA = ibm.aliquota,
                                    PERCENTUALE = ibm.percentuale,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };

                                if (aggiornaTutto)
                                {
                                    ibNew2 = new RIDUZIONI()
                                    {
                                        IDFUNZIONERIDUZIONE = Convert.ToDecimal(ibm.idFunzioneRiduzione),
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        // ALIQUOTA = ibm.aliquota,
                                        PERCENTUALE = ibm.percentuale,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    decimal tmpii = Convert.ToDecimal(ibm.idFunzioneRiduzione);
                                    libNew = db.RIDUZIONI.Where(a => a.IDFUNZIONERIDUZIONE == tmpii
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDRIDUZIONI), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                db.Database.BeginTransaction();
                                db.RIDUZIONI.AddRange(libNew);
                                db.SaveChanges();

                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervallo), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var r in libNew)
                                    {

                                        dtrp.AssociaCoefficienteRichiamo_Riduzioni(r.IDRIDUZIONI, db, ibm.dataInizioValidita);
                                        dtrp.AssociaIndennitaBase_Riduzioni(r.IDRIDUZIONI, db, ibm.dataInizioValidita);
                                        dtrp.AssociaIndennitaSistemazione_Riduzioni(r.IDRIDUZIONI, db, ibm.dataInizioValidita);
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
                            lista = dtal.RestituisciLaRigaMassima(Convert.ToDecimal(ibm.idFunzioneRiduzione));
                            if (lista.Count == 0)
                            {
                                ibNew1 = new RIDUZIONI()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = Convert.ToDateTime(Utility.DataFineStop()),
                                    PERCENTUALE = ibm.percentuale,
                                    IDFUNZIONERIDUZIONE = Convert.ToDecimal(ibm.idFunzioneRiduzione),
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                libNew.Add(ibNew1);
                                db.Database.BeginTransaction();
                                db.RIDUZIONI.Add(ibNew1);
                                db.SaveChanges();

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    dtrp.AssociaCoefficienteRichiamo_Riduzioni(ibNew1.IDRIDUZIONI, db, ibm.dataInizioValidita);
                                    dtrp.AssociaIndennitaBase_Riduzioni(ibNew1.IDRIDUZIONI, db, ibm.dataInizioValidita);
                                    dtrp.AssociaIndennitaSistemazione_Riduzioni(ibNew1.IDRIDUZIONI, db, ibm.dataInizioValidita);
                                }

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
                                    ibNew1 = new RIDUZIONI()
                                    {
                                        IDFUNZIONERIDUZIONE = Convert.ToDecimal(ibm.idFunzioneRiduzione),
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = dataFineUltimo,
                                        // ALIQUOTA = ibm.aliquota,//nuova aliquota rispetto alla vecchia registrata
                                        PERCENTUALE = ibm.percentuale,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.RIDUZIONI.Add(ibNew1);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        dtrp.AssociaCoefficienteRichiamo_Riduzioni(ibNew1.IDRIDUZIONI, db, ibm.dataInizioValidita);
                                        dtrp.AssociaIndennitaBase_Riduzioni(ibNew1.IDRIDUZIONI, db, ibm.dataInizioValidita);
                                        dtrp.AssociaIndennitaSistemazione_Riduzioni(ibNew1.IDRIDUZIONI, db, ibm.dataInizioValidita);
                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                                //se il nuovo record rappresenta la data variazione superiore alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                if (ibm.dataInizioValidita > dataInizioUltimo)
                                {
                                    ibNew1 = new RIDUZIONI()
                                    {
                                        IDFUNZIONERIDUZIONE = Convert.ToDecimal(ibm.idFunzioneRiduzione),
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = ibm.dataInizioValidita.AddDays(-1),
                                        PERCENTUALE = percentualeUltimo,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    ibNew2 = new RIDUZIONI()
                                    {
                                        IDFUNZIONERIDUZIONE = Convert.ToDecimal(ibm.idFunzioneRiduzione),
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        PERCENTUALE = ibm.percentuale,//nuova aliquota rispetto alla vecchia registrata
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1); libNew.Add(ibNew2);
                                    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                    db.Database.BeginTransaction();
                                    db.RIDUZIONI.AddRange(libNew);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        foreach (var r in libNew)
                                        {

                                            dtrp.AssociaCoefficienteRichiamo_Riduzioni(r.IDRIDUZIONI, db, ibm.dataInizioValidita);
                                            dtrp.AssociaIndennitaBase_Riduzioni(r.IDRIDUZIONI, db, ibm.dataInizioValidita);
                                            dtrp.AssociaIndennitaSistemazione_Riduzioni(r.IDRIDUZIONI, db, ibm.dataInizioValidita);
                                        }
                                    }

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
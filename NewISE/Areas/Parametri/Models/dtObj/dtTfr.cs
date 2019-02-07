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
    public class dtTfr : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<TFRModel> getListTfr()
        {
            List<TFRModel> libm = new List<TFRModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.TFR.ToList();

                    libm = (from e in lib
                            select new TFRModel()
                            {
                                idTFR = e.IDTFR,
                                idValuta = e.IDVALUTA,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new TFRModel().dataFineValidita,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                Annullato = e.ANNULLATO,
                                DescrizioneValuta = new ValuteModel()
                                {
                                    idValuta = e.VALUTE.IDVALUTA,
                                    descrizioneValuta = e.VALUTE.DESCRIZIONEVALUTA,
                                    valutaUfficiale = e.VALUTE.VALUTAUFFICIALE
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

        public IList<TFRModel> getListTfr(decimal idValuta)
        {
            List<TFRModel> libm = new List<TFRModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.TFR.Where(a => a.IDVALUTA == idValuta).ToList();

                    libm = (from e in lib
                            select new TFRModel()
                            {
                                idTFR = e.IDTFR,
                                idValuta = e.IDVALUTA,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new TFRModel().dataFineValidita,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                Annullato = e.ANNULLATO,
                                DescrizioneValuta = new ValuteModel()
                                {
                                    idValuta = e.VALUTE.IDVALUTA,
                                    descrizioneValuta = e.VALUTE.DESCRIZIONEVALUTA,
                                    valutaUfficiale = e.VALUTE.VALUTAUFFICIALE
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

        public IList<TFRModel> getListTfr(bool escludiAnnullati = false)
        {
            List<TFRModel> libm = new List<TFRModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.TFR.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new TFRModel()
                            {
                                idTFR = e.IDTFR,
                                idValuta = e.IDVALUTA,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new TFRModel().dataFineValidita,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                Annullato = e.ANNULLATO,
                                DescrizioneValuta = new ValuteModel()
                                {
                                    idValuta = e.VALUTE.IDVALUTA,
                                    descrizioneValuta = e.VALUTE.DESCRIZIONEVALUTA,
                                    valutaUfficiale = e.VALUTE.VALUTAUFFICIALE
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

        public IList<TFRModel> getListTfr(decimal idValuta, bool escludiAnnullati = false)
        {
            List<TFRModel> libm = new List<TFRModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<TFR> lib = new List<TFR>();
                    if (escludiAnnullati == true)
                        lib = db.TFR.Where(a => a.ANNULLATO == false && a.IDVALUTA == idValuta).ToList();
                    else
                        lib = db.TFR.Where(a => a.IDVALUTA == idValuta).ToList();

                    libm = (from e in lib
                            select new TFRModel()
                            {
                                idTFR = e.IDTFR,
                                idValuta = e.IDVALUTA,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                Annullato = e.ANNULLATO,
                                tassoCambio = e.TASSOCAMBIO,
                                DescrizioneValuta = new ValuteModel()
                                {
                                    idValuta = e.VALUTE.IDVALUTA,
                                    descrizioneValuta = e.VALUTE.DESCRIZIONEVALUTA,
                                    valutaUfficiale = e.VALUTE.VALUTAUFFICIALE
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


        public bool EsistonoMovimentiPrima(TFRModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.TFR.Where(a => a.DATAINIZIOVALIDITA < ibm.dataInizioValidita && a.IDVALUTA == ibm.idValuta).Count() > 0 ? true : false;
            }
        }

        public bool EsistonoMovimentiSuccessivi(TFRModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.TFR.Where(a => a.DATAINIZIOVALIDITA > ibm.dataFineValidita.Value && a.IDVALUTA == ibm.idValuta).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiSuccessiviUguale(TFRModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                if (ibm.dataFineValidita.HasValue)
                {
                    return db.TFR.Where(a => a.DATAINIZIOVALIDITA >= ibm.dataFineValidita.Value && a.IDVALUTA == ibm.idValuta).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EsistonoMovimentiPrimaUguale(TFRModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.TFR.Where(a => a.DATAINIZIOVALIDITA <= ibm.dataInizioValidita && a.IDVALUTA == ibm.idValuta).Count() > 0 ? true : false;
            }
        }

        //public void DelTfr(decimal idTfr)
        //{
        //    TFR precedenteIB = new TFR();
        //    TFR delIB = new TFR();


        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        try
        //        {
        //            db.Database.BeginTransaction();

        //            var lib = db.TFR.Where(a => a.IDTFR == idTfr);

        //            if (lib.Count() > 0)
        //            {
        //                delIB = lib.First();
        //                delIB.ANNULLATO = true;

        //                var lprecIB = db.TFR.Where(a => a.DATAFINEVALIDITA < delIB.DATAINIZIOVALIDITA && a.ANNULLATO == false).ToList();

        //                if (lprecIB.Count > 0)
        //                {
        //                    precedenteIB = lprecIB.Where(a => a.DATAFINEVALIDITA == lprecIB.Max(b => b.DATAFINEVALIDITA)).First();
        //                    precedenteIB.ANNULLATO = true;

        //                    var ibOld1 = new TFR()
        //                    {
        //                        IDTFR = precedenteIB.IDTFR,
        //                        IDVALUTA = precedenteIB.IDVALUTA,
        //                        TASSOCAMBIO = precedenteIB.TASSOCAMBIO,
        //                        DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
        //                        DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
        //                        DATAAGGIORNAMENTO = precedenteIB.DATAAGGIORNAMENTO,
        //                        ANNULLATO = false
        //                    };

        //                    db.TFR.Add(ibOld1);
        //                }

        //                db.SaveChanges();

        //                using (objLogAttivita log = new objLogAttivita())
        //                {
        //                    log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di TFR.", "TFR", idTfr);
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
        public bool TFRAnnullate(TFRModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.TFR.Where(a => a.IDTFR == ibm.idTFR).First().ANNULLATO == true ? true : false;
            }
        }
        public decimal Get_Id_TFRPrimoNonAnnullato(decimal idValuta)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<TFR> libm = new List<TFR>();
                libm = db.TFR.Where(a => a.ANNULLATO == false
                && a.IDVALUTA == idValuta).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDTFR;
            }
            return tmp;
        }

        public static DateTime DataInizioMinimaNonAnnullata(decimal idLivello)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.TFR.Where(a => a.ANNULLATO == false && a.IDVALUTA == idLivello).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
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
            var fm = context.ObjectInstance as TFRModel;
            if (fm != null)
            {
                DateTime d = DataInizioMinimaNonAnnullata(fm.idValuta);
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

        public TFR RestituisciIlRecordPrecedente(decimal idMagCon)
        {
            TFR tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                TFR interessato = new TFR();
                interessato = db.TFR.Find(idMagCon);
                tmp = db.TFR.Where(a => a.IDVALUTA == interessato.IDVALUTA
                && a.ANNULLATO == false).ToList().Where(b => b.DATAFINEVALIDITA == interessato.DATAINIZIOVALIDITA.AddDays(-1)).ToList().First();
            }
            return tmp;
        }
        public static DateTime DataInizioMinimaNonAnnullataMagFiglio(decimal idLivello)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.TFR.Where(a => a.ANNULLATO == false && a.IDVALUTA == idLivello).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }
        public List<string> RestituisciIntervalloDiUnaData(DateTime DataCampione, decimal IDVALUTA)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<TFR> libm = new List<TFR>();
                libm = db.TFR.Where(a => a.ANNULLATO == false
                && a.IDVALUTA == IDVALUTA).ToList().Where(b =>
                b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())
                && DataCampione > b.DATAINIZIOVALIDITA
                && DataCampione < b.DATAFINEVALIDITA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDTFR.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].TASSOCAMBIO.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione, decimal IDVALUTA)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<TFR> libm = new List<TFR>();
                libm = db.TFR.Where(a => a.ANNULLATO == false
                && a.IDVALUTA == IDVALUTA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().Where(b => DataCampione == b.DATAINIZIOVALIDITA &&
                 b.DATAFINEVALIDITA != Utility.DataFineStop()).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDTFR.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].TASSOCAMBIO.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione, decimal IDVALUTA)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<TFR> libm = new List<TFR>();
                libm = db.TFR.Where(a => a.ANNULLATO == false
                && a.IDVALUTA == IDVALUTA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().
                Where(b => DataCampione == b.DATAFINEVALIDITA
                && b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())).ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDTFR.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].TASSOCAMBIO.ToString());
                }
            }
            return tmp;
        }
        public List<string> RestituisciLaRigaMassima(decimal IDVALUTA)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<TFR> libm = new List<TFR>();
                libm = db.TFR.Where(a => a.ANNULLATO == false
                && a.IDVALUTA == IDVALUTA).ToList().Where(b =>
                b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop())).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDTFR.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].TASSOCAMBIO.ToString());
                }
            }
            return tmp;
        }
        public void RendiAnnullatoUnRecord(decimal idPercMagCon, ModelDBISE db)
        {
            TFR entita = new TFR();
            entita = db.TFR.Find(idPercMagCon);
            entita.ANNULLATO = true;
            db.SaveChanges();
        }
        public void DelTfr(decimal idMagCon)
        {
            TFR precedenteIB = new TFR();
            TFR delIB = new TFR();
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    var lib = db.TFR.Where(a => a.IDTFR == idMagCon);
                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;
                        RendiAnnullatoUnRecord(delIB.IDTFR, db);
                        precedenteIB = RestituisciIlRecordPrecedente(idMagCon);
                        RendiAnnullatoUnRecord(precedenteIB.IDTFR, db);

                        var NuovoPrecedente = new TFR()
                        {
                            IDVALUTA = precedenteIB.IDVALUTA,
                            DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                            //ALIQUOTA = precedenteIB.ALIQUOTA,
                            TASSOCAMBIO = precedenteIB.TASSOCAMBIO,
                            DATAAGGIORNAMENTO = DateTime.Now,// precedenteIB.DATAAGGIORNAMENTO,
                            ANNULLATO = false
                        };
                        db.TFR.Add(NuovoPrecedente);
                        db.SaveChanges();
                        using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                        {
                            dtrp.AssociaCanoneMAB_TFR(NuovoPrecedente.IDTFR, db,delIB.DATAINIZIOVALIDITA);
                            dtrp.AssociaIndennita_TFR(NuovoPrecedente.IDTFR, db, delIB.DATAINIZIOVALIDITA);
                        }
                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di Percentuale TFR", "TFR", idMagCon);
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
        public void SetTfr(TFRModel ibm, bool aggiornaTutto)
        {
            List<TFR> libNew = new List<TFR>();
            //TFR ibPrecedente = new TFR();
            TFR ibNew1 = new TFR();
            TFR ibNew2 = new TFR();
            //List<TFR> lArchivioIB = new List<TFR>();
            List<string> lista = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                bool giafatta = false;
                try
                {
                    using (dtTfr dtal = new dtTfr())
                    {
                        //Se la data variazione coincide con una data inizio esistente
                        lista = dtal.DataVariazioneCoincideConDataInizio(ibm.dataInizioValidita, Convert.ToDecimal(ibm.idValuta));
                        if (lista.Count != 0)
                        {
                            giafatta = true;
                            decimal idIntervalloFirst = Convert.ToDecimal(lista[0]);
                            DateTime dataInizioFirst = Convert.ToDateTime(lista[1]);
                            DateTime dataFineFirst = Convert.ToDateTime(lista[2]);
                            //decimal percConiugeFirst = Convert.ToDecimal(lista[3]);

                            ibNew1 = new TFR()
                            {
                                IDVALUTA = Convert.ToDecimal(ibm.idValuta),
                                DATAINIZIOVALIDITA = dataInizioFirst,
                                DATAFINEVALIDITA = dataFineFirst,
                                // ALIQUOTA = ibm.aliquota,
                                TASSOCAMBIO = ibm.tassoCambio,
                                DATAAGGIORNAMENTO = DateTime.Now,
                            };

                            if (aggiornaTutto)
                            {
                                ibNew1 = new TFR()
                                {
                                    IDVALUTA = Convert.ToDecimal(ibm.idValuta),
                                    DATAINIZIOVALIDITA = dataInizioFirst,
                                    DATAFINEVALIDITA = Utility.DataFineStop(),
                                    // ALIQUOTA = ibm.aliquota,
                                    TASSOCAMBIO = ibm.tassoCambio,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                //qui annullo tutti i record rimanenti dalla data inizio inserita
                                libNew = db.TFR.Where(a => a.ANNULLATO == false).ToList()
                                    .Where(a => a.DATAINIZIOVALIDITA > dataInizioFirst &&
                                    a.IDVALUTA == Convert.ToDecimal(ibm.idValuta)).ToList();
                                foreach (var elem in libNew)
                                {
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDTFR), db);
                                }
                            }
                            db.Database.BeginTransaction();
                            db.TFR.Add(ibNew1);
                            db.SaveChanges();



                            RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloFirst), db);

                            using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                            {
                                dtrp.AssociaCanoneMAB_TFR(ibNew1.IDTFR, db, ibm.dataInizioValidita);
                                dtrp.AssociaIndennita_TFR(ibNew1.IDTFR, db, ibm.dataInizioValidita);
                            }

                            db.Database.CurrentTransaction.Commit();
                        }
                        ///se la data variazione coincide con una data fine esistente(diversa da 31/12/9999)
                        if (giafatta == false)
                        {
                            lista = dtal.DataVariazioneCoincideConDataFine(ibm.dataInizioValidita, Convert.ToDecimal(ibm.idValuta));
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervalloLast = Convert.ToDecimal(lista[0]);
                                DateTime dataInizioLast = Convert.ToDateTime(lista[1]);
                                DateTime dataFineLast = Convert.ToDateTime(lista[2]);
                                decimal TASSOCAMBIO = Convert.ToDecimal(lista[3]);

                                ibNew1 = new TFR()
                                {
                                    IDVALUTA = Convert.ToDecimal(ibm.idValuta),
                                    DATAINIZIOVALIDITA = dataInizioLast,
                                    DATAFINEVALIDITA = dataFineLast.AddDays(-1),
                                    TASSOCAMBIO = TASSOCAMBIO,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new TFR()
                                {
                                    IDVALUTA = Convert.ToDecimal(ibm.idValuta),
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = ibm.dataInizioValidita,//è uguale alla data Inizio
                                    TASSOCAMBIO = ibm.tassoCambio,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };
                                if (aggiornaTutto)
                                {
                                    ibNew2 = new TFR()
                                    {
                                        IDVALUTA = Convert.ToDecimal(ibm.idValuta),
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        TASSOCAMBIO = ibm.tassoCambio,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.TFR.Where(a => a.IDVALUTA == Convert.ToDecimal(ibm.idValuta)
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDTFR), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                db.Database.BeginTransaction();
                                db.TFR.AddRange(libNew);
                                db.SaveChanges();

                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloLast), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var tfr in libNew)
                                    {
                                        dtrp.AssociaCanoneMAB_TFR(tfr.IDTFR, db, ibm.dataInizioValidita);
                                        dtrp.AssociaIndennita_TFR(tfr.IDTFR, db, ibm.dataInizioValidita);
                                    }

                                }

                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //Se il nuovo record si trova in un intervallo non annullato con data fine non uguale al 31/12/9999
                        if (giafatta == false)
                        {
                            lista = dtal.RestituisciIntervalloDiUnaData(ibm.dataInizioValidita, Convert.ToDecimal(ibm.idValuta));
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervallo = Convert.ToDecimal(lista[0]);
                                DateTime dataInizio = Convert.ToDateTime(lista[1]);
                                DateTime dataFine = Convert.ToDateTime(lista[2]);
                                decimal TASSOCAMBIO = Convert.ToDecimal(lista[3]);

                                DateTime NewdataFine1 = ibm.dataInizioValidita.AddDays(-1);

                                ibNew1 = new TFR()
                                {
                                    IDVALUTA = Convert.ToDecimal(ibm.idValuta),
                                    DATAINIZIOVALIDITA = dataInizio,
                                    DATAFINEVALIDITA = NewdataFine1,
                                    //ALIQUOTA = aliquota,
                                    TASSOCAMBIO = TASSOCAMBIO,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new TFR()
                                {
                                    IDVALUTA = Convert.ToDecimal(ibm.idValuta),
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = dataFine,
                                    // ALIQUOTA = ibm.aliquota,
                                    TASSOCAMBIO = ibm.tassoCambio,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };

                                if (aggiornaTutto)
                                {
                                    ibNew2 = new TFR()
                                    {
                                        IDVALUTA = Convert.ToDecimal(ibm.idValuta),
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        // ALIQUOTA = ibm.aliquota,
                                        TASSOCAMBIO = ibm.tassoCambio,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    decimal tmpii = Convert.ToDecimal(ibm.idValuta);
                                    libNew = db.TFR.Where(a => a.IDVALUTA == tmpii
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDTFR), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                db.Database.BeginTransaction();
                                db.TFR.AddRange(libNew);
                                db.SaveChanges();

                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervallo), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var tfr in libNew)
                                    {
                                        dtrp.AssociaCanoneMAB_TFR(tfr.IDTFR, db, ibm.dataInizioValidita);
                                        dtrp.AssociaIndennita_TFR(tfr.IDTFR, db, ibm.dataInizioValidita);
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
                            lista = dtal.RestituisciLaRigaMassima(Convert.ToDecimal(ibm.idValuta));
                            if (lista.Count == 0)
                            {
                                ibNew1 = new TFR()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = Convert.ToDateTime(Utility.DataFineStop()),
                                    TASSOCAMBIO = ibm.tassoCambio,
                                    IDVALUTA = Convert.ToDecimal(ibm.idValuta),
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                libNew.Add(ibNew1);
                                db.Database.BeginTransaction();
                                db.TFR.Add(ibNew1);
                                db.SaveChanges();
                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    dtrp.AssociaCanoneMAB_TFR(ibNew1.IDTFR, db, ibm.dataInizioValidita);
                                    dtrp.AssociaIndennita_TFR(ibNew1.IDTFR, db, ibm.dataInizioValidita);
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
                                    ibNew1 = new TFR()
                                    {
                                        IDVALUTA = Convert.ToDecimal(ibm.idValuta),
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = dataFineUltimo,
                                        // ALIQUOTA = ibm.aliquota,//nuova aliquota rispetto alla vecchia registrata
                                        TASSOCAMBIO = ibm.tassoCambio,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.TFR.Add(ibNew1);
                                    db.SaveChanges();

                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        dtrp.AssociaCanoneMAB_TFR(ibNew1.IDTFR, db, ibm.dataInizioValidita);
                                        dtrp.AssociaIndennita_TFR(ibNew1.IDTFR, db, ibm.dataInizioValidita);
                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                                //se il nuovo record rappresenta la data variazione superiore alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                if (ibm.dataInizioValidita > dataInizioUltimo)
                                {
                                    ibNew1 = new TFR()
                                    {
                                        IDVALUTA = Convert.ToDecimal(ibm.idValuta),
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = ibm.dataInizioValidita.AddDays(-1),
                                        TASSOCAMBIO = percentualeUltimo,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    ibNew2 = new TFR()
                                    {
                                        IDVALUTA = Convert.ToDecimal(ibm.idValuta),
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        TASSOCAMBIO = ibm.tassoCambio,//nuova aliquota rispetto alla vecchia registrata
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1); libNew.Add(ibNew2);
                                    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                    db.Database.BeginTransaction();
                                    db.TFR.AddRange(libNew);
                                    db.SaveChanges();

                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        foreach (var tfr in libNew)
                                        {
                                            dtrp.AssociaCanoneMAB_TFR(tfr.IDTFR, db, ibm.dataInizioValidita);
                                            dtrp.AssociaIndennita_TFR(tfr.IDTFR, db, ibm.dataInizioValidita);
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
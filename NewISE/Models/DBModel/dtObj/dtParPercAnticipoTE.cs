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
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Enumeratori;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParPercAnticipoTE : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        //
        public IList<TipoAnticipoTrasportoEffettiModel> GetTipoAnticipoTraspEffetti()
        {
            List<TipoAnticipoTrasportoEffettiModel> libm = new List<TipoAnticipoTrasportoEffettiModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.TIPOANTICIPOTRASPORTOEFFETTI.ToList();

                    libm = (from e in lib
                            select new TipoAnticipoTrasportoEffettiModel()
                            {
                                idTipoAnticipoTrasportEff = e.IDTIPOANTICIPOTE,
                                tipoAnticipoTraspEffetti = e.TIPOANTICIPO

                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TipoAnticipoTrasportoEffettiModel GetTipoAnticipoTraspEffetti(decimal idTipologiaAnticipo)
        {
            TipoAnticipoTrasportoEffettiModel lm = new TipoAnticipoTrasportoEffettiModel();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var liv = db.TIPOANTICIPOTRASPORTOEFFETTI.Find(idTipologiaAnticipo);

                    lm = new TipoAnticipoTrasportoEffettiModel()
                    {
                        idTipoAnticipoTrasportEff = liv.IDTIPOANTICIPOTE,
                        tipoAnticipoTraspEffetti = liv.TIPOANTICIPO,
                    };
                }
                return lm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<PercAnticipoTEModel> getListPercAnticipoTE()
        {
            List<PercAnticipoTEModel> libm = new List<PercAnticipoTEModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.PERCENTUALEANTICIPOTE.ToList();

                    libm = (from e in lib
                            select new PercAnticipoTEModel()
                            {
                                idPercAnticipoTE = e.IDPERCANTICIPOTM,
                                idTipoAnticipo = e.IDTIPOANTICIPOTE,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                percentuale = e.PERCENTUALE,
                                annullato = e.ANNULLATO,
                                dataAggiornamento = e.DATAAGGIORNAMENTO
                            }).ToList();
                }
                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public IList<PercAnticipoTEModel> getListMaggiorazioneFiglio(decimal idTipoAnticipoTE)
        //{
        //    List<PercAnticipoTEModel> libm = new List<PercAnticipoTEModel>();

        //    try
        //    {
        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            var lib = db.PERCENTUALEANTICIPOTE.Where(a => a.IDTIPOANTICIPOTE == idTipoAnticipoTE).ToList();

        //            libm = (from e in lib
        //                    select new PercAnticipoTEModel()
        //                    {

        //                        idPercAnticipoTE = e.IDPERCANTICIPOTM,
        //                        idTipoAnticipoTE = e.IDTIPOANTICIPOTE,
        //                        dataInizioValidita = e.DATAINIZIOVALIDITA,
        //                        dataFineValidita = e.DATAFINEVALIDITA,
        //                        percentuale = e.PERCENTUALE,
        //                        annullato = e.ANNULLATO,
        //                        Figlio = new TipologiaFiglioModel()
        //                        {
        //                            idTipoAnticipoTE = e.TIPOLOGIAFIGLIO.IDTIPOANTICIPOTE,
        //                            tipologiaFiglio = e.TIPOLOGIAFIGLIO.TIPOLOGIAFIGLIO1
        //                        }
        //                    }).ToList();
        //        }

        //        return libm;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public IList<PercAnticipoTEModel> getListMaggiorazioneFiglio(bool escludiAnnullati = false)
        //{
        //    List<PercAnticipoTEModel> libm = new List<PercAnticipoTEModel>();

        //    try
        //    {
        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            var lib = db.PERCENTUALEANTICIPOTE.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

        //            libm = (from e in lib
        //                    select new PercAnticipoTEModel()
        //                    {

        //                        idPercAnticipoTE = e.IDPERCANTICIPOTM,
        //                        idTipoAnticipoTE = (decimal)e.IDTIPOANTICIPOTE,
        //                        dataInizioValidita = e.DATAINIZIOVALIDITA,
        //                        dataFineValidita = e.DATAFINEVALIDITA,
        //                        percentuale = e.PERCENTUALE,
        //                        annullato = e.ANNULLATO,
        //                        dataAggiornamento = e.DATAAGGIORNAMENTO,
        //                        Figlio = new TipologiaFiglioModel()
        //                        {
        //                            idTipoAnticipoTE = e.TIPOLOGIAFIGLIO.IDTIPOANTICIPOTE,
        //                            tipologiaFiglio = e.TIPOLOGIAFIGLIO.TIPOLOGIAFIGLIO1
        //                        }
        //                    }).ToList();
        //        }

        //        return libm;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public IList<PercAnticipoTEModel> getListPercAnticipoTE(decimal idTipoAnticipoTE, bool escludiAnnullati = true)
        {
            List<PercAnticipoTEModel> libm = new List<PercAnticipoTEModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<PERCENTUALEANTICIPOTE> lib = new List<PERCENTUALEANTICIPOTE>();
                    if (escludiAnnullati == true)
                        lib = db.PERCENTUALEANTICIPOTE.Where(a => a.IDTIPOANTICIPOTE == idTipoAnticipoTE && a.ANNULLATO == false).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                    else
                        lib = db.PERCENTUALEANTICIPOTE.Where(a => a.IDTIPOANTICIPOTE == idTipoAnticipoTE).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();

                    libm = (from e in lib
                            select new PercAnticipoTEModel()
                            {
                                idPercAnticipoTE = e.IDPERCANTICIPOTM,
                                idTipoAnticipo = e.IDTIPOANTICIPOTE,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                percentuale = e.PERCENTUALE,
                                annullato = e.ANNULLATO,
                                dataAggiornamento = e.DATAAGGIORNAMENTO
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



        public bool PercAnticipoTEAnnullato(PercAnticipoTEModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEANTICIPOTE.Where(a => a.IDPERCANTICIPOTM == ibm.idPercAnticipoTE).First().ANNULLATO == true ? true : false;
            }
        }

        public decimal Get_Id_PercAnticipoTEPrimoNonAnnullato(decimal idTipoAnticipoTE)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEANTICIPOTE> libm = new List<PERCENTUALEANTICIPOTE>();
                libm = db.PERCENTUALEANTICIPOTE.Where(a => a.ANNULLATO == false
                && a.IDTIPOANTICIPOTE == idTipoAnticipoTE).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
                if (libm.Count != 0)
                    tmp = libm.First().IDPERCANTICIPOTM;
            }
            return tmp;
        }

        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as PercAnticipoTEModel;
            if (fm != null)
            {
                DateTime d = DataInizioMinimaNonAnnullata(Convert.ToDecimal(fm.idTipoAnticipo));
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
        public static ValidationResult VerificaPercentuale(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as PercAnticipoTEModel;

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
                var TuttiNonAnnullati = db.PERCENTUALEANTICIPOTE.Where(a => a.ANNULLATO == false && a.IDTIPOANTICIPOTE == idLivello).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }

        public bool PercMaggiorazioneFiglioAnnullato(PercAnticipoTEModel ibm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                return db.PERCENTUALEANTICIPOTE.Where(a => a.IDPERCANTICIPOTM == ibm.idPercAnticipoTE).First().ANNULLATO == true ? true : false;
            }
        }
        public PERCENTUALEANTICIPOTE RestituisciIlRecordPrecedente(decimal idMagCon)
        {
            PERCENTUALEANTICIPOTE tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                PERCENTUALEANTICIPOTE interessato = new PERCENTUALEANTICIPOTE();
                interessato = db.PERCENTUALEANTICIPOTE.Find(idMagCon);
                tmp = db.PERCENTUALEANTICIPOTE.Where(a => a.IDTIPOANTICIPOTE == interessato.IDTIPOANTICIPOTE
                && a.ANNULLATO == false).ToList().Where(b => b.DATAFINEVALIDITA == interessato.DATAINIZIOVALIDITA.AddDays(-1)).ToList().First();
            }
            return tmp;
        }
        public static DateTime DataInizioMinimaNonAnnullataMagFiglio(decimal idLivello)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.PERCENTUALEANTICIPOTE.Where(a => a.ANNULLATO == false && a.IDTIPOANTICIPOTE == idLivello).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return Utility.GetData_Inizio_Base();
        }
        public List<string> RestituisciIntervalloDiUnaData(DateTime DataCampione, decimal IDTIPOANTICIPOTE)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEANTICIPOTE> libm = new List<PERCENTUALEANTICIPOTE>();
                libm = db.PERCENTUALEANTICIPOTE.Where(a => a.ANNULLATO == false
                && a.IDTIPOANTICIPOTE == IDTIPOANTICIPOTE).ToList().Where(b =>
                b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())
                && DataCampione > b.DATAINIZIOVALIDITA
                && DataCampione < b.DATAFINEVALIDITA).OrderBy(b => b.DATAINIZIOVALIDITA).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCANTICIPOTM.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALE.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataInizio(DateTime DataCampione, decimal IDTIPOANTICIPOTE)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEANTICIPOTE> libm = new List<PERCENTUALEANTICIPOTE>();
                libm = db.PERCENTUALEANTICIPOTE.Where(a => a.ANNULLATO == false
                && a.IDTIPOANTICIPOTE == IDTIPOANTICIPOTE).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().Where(b => DataCampione == b.DATAINIZIOVALIDITA &&
                 b.DATAFINEVALIDITA != Utility.DataFineStop()).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCANTICIPOTM.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALE.ToString());
                }
            }
            return tmp;
        }
        public List<string> DataVariazioneCoincideConDataFine(DateTime DataCampione, decimal IDTIPOANTICIPOTE)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEANTICIPOTE> libm = new List<PERCENTUALEANTICIPOTE>();
                libm = db.PERCENTUALEANTICIPOTE.Where(a => a.ANNULLATO == false
                && a.IDTIPOANTICIPOTE == IDTIPOANTICIPOTE).OrderBy(b => b.DATAINIZIOVALIDITA).ToList().
                Where(b => DataCampione == b.DATAFINEVALIDITA
                && b.DATAFINEVALIDITA != Convert.ToDateTime(Utility.DataFineStop())).ToList();

                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCANTICIPOTM.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALE.ToString());
                }
            }
            return tmp;
        }
        public List<string> RestituisciLaRigaMassima(decimal IDTIPOANTICIPOTE)
        {
            List<string> tmp = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                List<PERCENTUALEANTICIPOTE> libm = new List<PERCENTUALEANTICIPOTE>();
                libm = db.PERCENTUALEANTICIPOTE.Where(a => a.ANNULLATO == false
                && a.IDTIPOANTICIPOTE == IDTIPOANTICIPOTE).ToList().Where(b =>
                b.DATAFINEVALIDITA == Convert.ToDateTime(Utility.DataFineStop())).ToList();
                if (libm.Count != 0)
                {
                    tmp.Add(libm[0].IDPERCANTICIPOTM.ToString());
                    tmp.Add(libm[0].DATAINIZIOVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].DATAFINEVALIDITA.ToShortDateString());
                    tmp.Add(libm[0].PERCENTUALE.ToString());
                }
            }
            return tmp;
        }
        public void RendiAnnullatoUnRecord(decimal idPercMagCon, ModelDBISE db)
        {
            PERCENTUALEANTICIPOTE entita = new PERCENTUALEANTICIPOTE();
            entita = db.PERCENTUALEANTICIPOTE.Find(idPercMagCon);
            entita.ANNULLATO = true;
            db.SaveChanges();
        }

        public void DelPercAnticipoTE(decimal idMagCon)
        {
            PERCENTUALEANTICIPOTE precedenteIB = new PERCENTUALEANTICIPOTE();
            PERCENTUALEANTICIPOTE delIB = new PERCENTUALEANTICIPOTE();
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    var lib = db.PERCENTUALEANTICIPOTE.Where(a => a.IDPERCANTICIPOTM == idMagCon);
                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        delIB.ANNULLATO = true;
                        RendiAnnullatoUnRecord(delIB.IDPERCANTICIPOTM, db);
                        precedenteIB = RestituisciIlRecordPrecedente(idMagCon);
                        RendiAnnullatoUnRecord(precedenteIB.IDPERCANTICIPOTM, db);

                        var NuovoPrecedente = new PERCENTUALEANTICIPOTE()
                        {
                            IDTIPOANTICIPOTE = precedenteIB.IDTIPOANTICIPOTE,
                            DATAINIZIOVALIDITA = precedenteIB.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = delIB.DATAFINEVALIDITA,
                            //ALIQUOTA = precedenteIB.ALIQUOTA,
                            PERCENTUALE = precedenteIB.PERCENTUALE,
                            DATAAGGIORNAMENTO = DateTime.Now,// precedenteIB.DATAAGGIORNAMENTO,
                            ANNULLATO = false
                        };
                        db.PERCENTUALEANTICIPOTE.Add(NuovoPrecedente);

                        db.SaveChanges();

                        using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                        {

                            switch ((EnumTipoAnticipoTE)NuovoPrecedente.IDTIPOANTICIPOTE)
                            {
                                case EnumTipoAnticipoTE.Partenza:
                                    dtrp.AssociaPercentualeAnticipoTEP(NuovoPrecedente.IDPERCANTICIPOTM, db, delIB.DATAINIZIOVALIDITA);
                                    break;
                                case EnumTipoAnticipoTE.Rientro:
                                    dtrp.AssociaPercentualeAnticipoTER(NuovoPrecedente.IDPERCANTICIPOTM, db,delIB.DATAINIZIOVALIDITA);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }


                        }
                    }



                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Eliminazione, "Eliminazione parametro di Percentuale.", "PERCENTUALE", idMagCon);
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

        public void SetPercAnticipoTE(PercAnticipoTEModel ibm, bool aggiornaTutto)
        {
            List<PERCENTUALEANTICIPOTE> libNew = new List<PERCENTUALEANTICIPOTE>();
            //PERCENTUALEANTICIPOTE ibPrecedente = new PERCENTUALEANTICIPOTE();
            PERCENTUALEANTICIPOTE ibNew1 = new PERCENTUALEANTICIPOTE();
            PERCENTUALEANTICIPOTE ibNew2 = new PERCENTUALEANTICIPOTE();
            //List<PERCENTUALEANTICIPOTE> lArchivioIB = new List<PERCENTUALEANTICIPOTE>();
            List<string> lista = new List<string>();
            using (ModelDBISE db = new ModelDBISE())
            {
                bool giafatta = false;
                try
                {
                    using (dtParPercAnticipoTE dtal = new dtParPercAnticipoTE())
                    {
                        //Se la data variazione coincide con una data inizio esistente
                        lista = dtal.DataVariazioneCoincideConDataInizio(ibm.dataInizioValidita, Convert.ToDecimal(ibm.idTipoAnticipo));
                        if (lista.Count != 0)
                        {
                            giafatta = true;
                            decimal idIntervalloFirst = Convert.ToDecimal(lista[0]);
                            DateTime dataInizioFirst = Convert.ToDateTime(lista[1]);
                            DateTime dataFineFirst = Convert.ToDateTime(lista[2]);
                            //decimal percConiugeFirst = Convert.ToDecimal(lista[3]);

                            ibNew1 = new PERCENTUALEANTICIPOTE()
                            {
                                IDTIPOANTICIPOTE = Convert.ToDecimal(ibm.idTipoAnticipo),
                                DATAINIZIOVALIDITA = dataInizioFirst,
                                DATAFINEVALIDITA = dataFineFirst,
                                // ALIQUOTA = ibm.aliquota,
                                PERCENTUALE = ibm.percentuale,
                                DATAAGGIORNAMENTO = DateTime.Now,
                            };

                            if (aggiornaTutto)
                            {
                                ibNew1 = new PERCENTUALEANTICIPOTE()
                                {
                                    IDTIPOANTICIPOTE = Convert.ToDecimal(ibm.idTipoAnticipo),
                                    DATAINIZIOVALIDITA = dataInizioFirst,
                                    DATAFINEVALIDITA = Utility.DataFineStop(),
                                    // ALIQUOTA = ibm.aliquota,
                                    PERCENTUALE = ibm.percentuale,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                //qui annullo tutti i record rimanenti dalla data inizio inserita
                                libNew = db.PERCENTUALEANTICIPOTE.Where(a => a.ANNULLATO == false).ToList()
                                    .Where(a => a.DATAINIZIOVALIDITA > dataInizioFirst &&
                                    a.IDTIPOANTICIPOTE == Convert.ToDecimal(ibm.idTipoAnticipo)).ToList();
                                foreach (var elem in libNew)
                                {
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPERCANTICIPOTM), db);
                                }
                            }
                            db.Database.BeginTransaction();
                            db.PERCENTUALEANTICIPOTE.Add(ibNew1);
                            db.SaveChanges();
                            RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloFirst), db);

                            using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                            {
                                switch ((EnumTipoAnticipoTE)ibNew1.IDTIPOANTICIPOTE)
                                {
                                    case EnumTipoAnticipoTE.Partenza:
                                        dtrp.AssociaPercentualeAnticipoTEP(ibNew1.IDPERCANTICIPOTM, db, ibm.dataInizioValidita);
                                        break;
                                    case EnumTipoAnticipoTE.Rientro:
                                        dtrp.AssociaPercentualeAnticipoTER(ibNew1.IDPERCANTICIPOTM, db, ibm.dataInizioValidita);
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }

                            }

                            db.Database.CurrentTransaction.Commit();
                        }
                        ///se la data variazione coincide con una data fine esistente(diversa da 31/12/9999)
                        if (giafatta == false)
                        {
                            lista = dtal.DataVariazioneCoincideConDataFine(ibm.dataInizioValidita, Convert.ToDecimal(ibm.idTipoAnticipo));
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervalloLast = Convert.ToDecimal(lista[0]);
                                DateTime dataInizioLast = Convert.ToDateTime(lista[1]);
                                DateTime dataFineLast = Convert.ToDateTime(lista[2]);
                                decimal PERCENTUALE = Convert.ToDecimal(lista[3]);

                                ibNew1 = new PERCENTUALEANTICIPOTE()
                                {
                                    IDTIPOANTICIPOTE = Convert.ToDecimal(ibm.idTipoAnticipo),
                                    DATAINIZIOVALIDITA = dataInizioLast,
                                    DATAFINEVALIDITA = dataFineLast.AddDays(-1),
                                    PERCENTUALE = PERCENTUALE,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new PERCENTUALEANTICIPOTE()
                                {
                                    IDTIPOANTICIPOTE = Convert.ToDecimal(ibm.idTipoAnticipo),
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = ibm.dataInizioValidita,//è uguale alla data Inizio
                                    PERCENTUALE = ibm.percentuale,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };
                                if (aggiornaTutto)
                                {
                                    ibNew2 = new PERCENTUALEANTICIPOTE()
                                    {
                                        IDTIPOANTICIPOTE = Convert.ToDecimal(ibm.idTipoAnticipo),
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        PERCENTUALE = ibm.percentuale,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew = db.PERCENTUALEANTICIPOTE.Where(a => a.IDTIPOANTICIPOTE == Convert.ToDecimal(ibm.idTipoAnticipo)
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPERCANTICIPOTM), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                db.Database.BeginTransaction();
                                db.PERCENTUALEANTICIPOTE.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloLast), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var pa in libNew)
                                    {
                                        switch ((EnumTipoAnticipoTE)pa.IDTIPOANTICIPOTE)
                                        {
                                            case EnumTipoAnticipoTE.Partenza:
                                                dtrp.AssociaPercentualeAnticipoTEP(pa.IDPERCANTICIPOTM, db, ibm.dataInizioValidita);
                                                break;
                                            case EnumTipoAnticipoTE.Rientro:
                                                dtrp.AssociaPercentualeAnticipoTER(pa.IDPERCANTICIPOTM, db, ibm.dataInizioValidita);
                                                break;
                                            default:
                                                throw new ArgumentOutOfRangeException();
                                        }
                                    }


                                }

                                db.Database.CurrentTransaction.Commit();
                            }
                        }
                        //Se il nuovo record si trova in un intervallo non annullato con data fine non uguale al 31/12/9999
                        if (giafatta == false)
                        {
                            lista = dtal.RestituisciIntervalloDiUnaData(ibm.dataInizioValidita, Convert.ToDecimal(ibm.idTipoAnticipo));
                            if (lista.Count != 0)
                            {
                                giafatta = true;
                                decimal idIntervallo = Convert.ToDecimal(lista[0]);
                                DateTime dataInizio = Convert.ToDateTime(lista[1]);
                                DateTime dataFine = Convert.ToDateTime(lista[2]);
                                decimal PERCENTUALE = Convert.ToDecimal(lista[3]);

                                DateTime NewdataFine1 = ibm.dataInizioValidita.AddDays(-1);

                                ibNew1 = new PERCENTUALEANTICIPOTE()
                                {
                                    IDTIPOANTICIPOTE = Convert.ToDecimal(ibm.idTipoAnticipo),
                                    DATAINIZIOVALIDITA = dataInizio,
                                    DATAFINEVALIDITA = NewdataFine1,
                                    //ALIQUOTA = aliquota,
                                    PERCENTUALE = PERCENTUALE,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                ibNew2 = new PERCENTUALEANTICIPOTE()
                                {
                                    IDTIPOANTICIPOTE = Convert.ToDecimal(ibm.idTipoAnticipo),
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = dataFine,
                                    // ALIQUOTA = ibm.aliquota,
                                    PERCENTUALE = ibm.percentuale,
                                    DATAAGGIORNAMENTO = DateTime.Now
                                };

                                if (aggiornaTutto)
                                {
                                    ibNew2 = new PERCENTUALEANTICIPOTE()
                                    {
                                        IDTIPOANTICIPOTE = Convert.ToDecimal(ibm.idTipoAnticipo),
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        // ALIQUOTA = ibm.aliquota,
                                        PERCENTUALE = ibm.percentuale,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    decimal tmpii = Convert.ToDecimal(ibm.idTipoAnticipo);
                                    libNew = db.PERCENTUALEANTICIPOTE.Where(a => a.IDTIPOANTICIPOTE == tmpii
                                    && a.ANNULLATO == false).ToList().Where(a => a.DATAINIZIOVALIDITA > ibm.dataInizioValidita).ToList();
                                    foreach (var elem in libNew)
                                    {
                                        RendiAnnullatoUnRecord(Convert.ToDecimal(elem.IDPERCANTICIPOTM), db);
                                    }
                                }

                                libNew.Add(ibNew1); libNew.Add(ibNew2);
                                libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                db.Database.BeginTransaction();
                                db.PERCENTUALEANTICIPOTE.AddRange(libNew);
                                db.SaveChanges();
                                //annullare l'intervallo trovato
                                RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervallo), db);

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {
                                    foreach (var pa in libNew)
                                    {
                                        switch ((EnumTipoAnticipoTE)pa.IDTIPOANTICIPOTE)
                                        {
                                            case EnumTipoAnticipoTE.Partenza:
                                                dtrp.AssociaPercentualeAnticipoTEP(pa.IDPERCANTICIPOTM, db, ibm.dataInizioValidita);
                                                break;
                                            case EnumTipoAnticipoTE.Rientro:
                                                dtrp.AssociaPercentualeAnticipoTER(pa.IDPERCANTICIPOTM, db, ibm.dataInizioValidita);
                                                break;
                                            default:
                                                throw new ArgumentOutOfRangeException();
                                        }
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
                            lista = dtal.RestituisciLaRigaMassima(Convert.ToDecimal(ibm.idTipoAnticipo));
                            if (lista.Count == 0)
                            {
                                ibNew1 = new PERCENTUALEANTICIPOTE()
                                {
                                    DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                    DATAFINEVALIDITA = Convert.ToDateTime(Utility.DataFineStop()),
                                    PERCENTUALE = ibm.percentuale,
                                    IDTIPOANTICIPOTE = Convert.ToDecimal(ibm.idTipoAnticipo),
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                };
                                libNew.Add(ibNew1);
                                db.Database.BeginTransaction();
                                db.PERCENTUALEANTICIPOTE.Add(ibNew1);
                                db.SaveChanges();

                                using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                {

                                    switch ((EnumTipoAnticipoTE)ibNew1.IDTIPOANTICIPOTE)
                                    {
                                        case EnumTipoAnticipoTE.Partenza:
                                            dtrp.AssociaPercentualeAnticipoTEP(ibNew1.IDPERCANTICIPOTM, db, ibm.dataInizioValidita);
                                            break;
                                        case EnumTipoAnticipoTE.Rientro:
                                            dtrp.AssociaPercentualeAnticipoTER(ibNew1.IDPERCANTICIPOTM, db, ibm.dataInizioValidita);
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }



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
                                    ibNew1 = new PERCENTUALEANTICIPOTE()
                                    {
                                        IDTIPOANTICIPOTE = Convert.ToDecimal(ibm.idTipoAnticipo),
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = dataFineUltimo,
                                        // ALIQUOTA = ibm.aliquota,//nuova aliquota rispetto alla vecchia registrata
                                        PERCENTUALE = ibm.percentuale,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1);
                                    db.Database.BeginTransaction();
                                    db.PERCENTUALEANTICIPOTE.Add(ibNew1);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {

                                        switch ((EnumTipoAnticipoTE)ibNew1.IDTIPOANTICIPOTE)
                                        {
                                            case EnumTipoAnticipoTE.Partenza:
                                                dtrp.AssociaPercentualeAnticipoTEP(ibNew1.IDPERCANTICIPOTM, db, ibm.dataInizioValidita);
                                                break;
                                            case EnumTipoAnticipoTE.Rientro:
                                                dtrp.AssociaPercentualeAnticipoTER(ibNew1.IDPERCANTICIPOTM, db, ibm.dataInizioValidita);
                                                break;
                                            default:
                                                throw new ArgumentOutOfRangeException();
                                        }



                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                                //se il nuovo record rappresenta la data variazione superiore alla data inizio dell'ultima riga ( record corrispondente alla data fine uguale 31/12/9999)
                                if (ibm.dataInizioValidita > dataInizioUltimo)
                                {
                                    ibNew1 = new PERCENTUALEANTICIPOTE()
                                    {
                                        IDTIPOANTICIPOTE = Convert.ToDecimal(ibm.idTipoAnticipo),
                                        DATAINIZIOVALIDITA = dataInizioUltimo,
                                        DATAFINEVALIDITA = ibm.dataInizioValidita.AddDays(-1),
                                        PERCENTUALE = percentualeUltimo,
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    ibNew2 = new PERCENTUALEANTICIPOTE()
                                    {
                                        IDTIPOANTICIPOTE = Convert.ToDecimal(ibm.idTipoAnticipo),
                                        DATAINIZIOVALIDITA = ibm.dataInizioValidita,
                                        DATAFINEVALIDITA = Utility.DataFineStop(),
                                        PERCENTUALE = ibm.percentuale,//nuova aliquota rispetto alla vecchia registrata
                                        DATAAGGIORNAMENTO = DateTime.Now
                                    };
                                    libNew.Add(ibNew1); libNew.Add(ibNew2);
                                    libNew = libNew.OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                    db.Database.BeginTransaction();
                                    db.PERCENTUALEANTICIPOTE.AddRange(libNew);
                                    db.SaveChanges();
                                    RendiAnnullatoUnRecord(Convert.ToDecimal(idIntervalloUltimo), db);

                                    using (DtRicalcoloParametri dtrp = new DtRicalcoloParametri())
                                    {
                                        foreach (var pa in libNew)
                                        {
                                            switch ((EnumTipoAnticipoTE)pa.IDTIPOANTICIPOTE)
                                            {
                                                case EnumTipoAnticipoTE.Partenza:
                                                    dtrp.AssociaPercentualeAnticipoTEP(pa.IDPERCANTICIPOTM, db, ibm.dataInizioValidita);
                                                    break;
                                                case EnumTipoAnticipoTE.Rientro:
                                                    dtrp.AssociaPercentualeAnticipoTER(pa.IDPERCANTICIPOTM, db, ibm.dataInizioValidita);
                                                    break;
                                                default:
                                                    throw new ArgumentOutOfRangeException();
                                            }
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
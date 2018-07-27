
using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Models.Tools;
using System.ComponentModel.DataAnnotations;
using NewISE.Models.dtObj.ModelliCalcolo;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtIndennitaBase : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void AssociaIndennitaBase_Indennita(decimal idTrasferimento, decimal idIndennitaBase, ModelDBISE db)
        {

            try
            {
                var i = db.INDENNITA.Find(idTrasferimento);

                var item = db.Entry<INDENNITA>(i);

                item.State = System.Data.Entity.EntityState.Modified;

                item.Collection(a => a.INDENNITABASE).Load();

                var l = db.INDENNITABASE.Find(idIndennitaBase);

                i.INDENNITABASE.Add(l);

                db.SaveChanges();


            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void RimuoviAssciazioniIndennitaBase_Indennita(decimal idTrasferimento, DateTime dtIni, DateTime dtFin, ModelDBISE db)
        {
            var i = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA;
            var lib =
                i.INDENNITABASE.Where(
                    a => a.ANNULLATO == false && a.DATAFINEVALIDITA >= dtIni && a.DATAINIZIOVALIDITA <= dtFin)
                    .OrderBy(a => a.DATAINIZIOVALIDITA);

            if (lib?.Any() ?? false)
            {
                foreach (var ib in lib)
                {
                    i.INDENNITABASE.Remove(ib);
                }

                db.SaveChanges();
            }


        }

        public void RimuoviAssociazioneIndennitaBase_Indennita(decimal idTrasferimento, DateTime dt, ModelDBISE db)
        {
            //var i = db.INDENNITA.Find(idTrasferimento);

            //var item = db.Entry<INDENNITA>(i);

            //item.State = System.Data.Entity.EntityState.Modified;

            //item.Collection(a => a.INDENNITABASE).Load();

            //var n = i.INDENNITABASE.ToList().RemoveAll(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA);

            //if (n > 0)
            //{
            //    db.SaveChanges();
            //}

            var i = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA;
            var lit = i.INDENNITABASE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA).ToList();

            foreach (var item in lit)
            {
                i.INDENNITABASE.Remove(item);
            }
            db.SaveChanges();


        }
        public IList<EvoluzioneIndennitaModel> GetIndennitaBaseComune(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var indennita = trasferimento.INDENNITA;

                    List<DateTime> lDateVariazioni = new List<DateTime>();
                                        
                    var ll = 
                        db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.INDENNITABASE
                        .Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.IDLIVELLO)
                        .ThenBy(a => a.DATAINIZIOVALIDITA)
                        .ThenBy(a => a.DATAFINEVALIDITA)
                        .ToList();
                    
                    using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                    {   
                        RuoloDipendenteModel rdm = dtrd.GetRuoloDipendenteByIdIndennita(idTrasferimento);

                                using (dtTrasferimento dttrasf = new dtTrasferimento())
                                {
                                    dipInfoTrasferimentoModel dipInfoTrasf = dttrasf.GetInfoTrasferimento(idTrasferimento);

                                    eim = (from e in ll
                                        select new EvoluzioneIndennitaModel()
                                        {
                                            idIndennitaBase = e.IDINDENNITABASE,
                                            idLivello = e.IDLIVELLO,
                                            dataInizioValidita = e.DATAINIZIOVALIDITA,
                                            dataFineValidita = e.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : e.DATAFINEVALIDITA,
                                            valore = e.VALORE,
                                            valoreResponsabile = e.VALORERESP,
                                            dataAggiornamento = e.DATAAGGIORNAMENTO,
                                            annullato = e.ANNULLATO,
                                            Livello = new LivelloModel()
                                            {
                                                idLivello = e.LIVELLI.IDLIVELLO,
                                                DescLivello = e.LIVELLI.LIVELLO
                                            }, 
                                            RuoloUfficio = new RuoloUfficioModel()
                                            {
                                                idRuoloUfficio = rdm.RuoloUfficio.idRuoloUfficio,
                                                DescrizioneRuolo = rdm.RuoloUfficio.DescrizioneRuolo
                                            },
                                            dipInfoTrasferimento = new dipInfoTrasferimentoModel
                                            {
                                                Decorrenza = dipInfoTrasf.Decorrenza,
                                                indennitaServizio = dipInfoTrasf.indennitaServizio

                                            }

                                        }).ToList();



                            //foreach (var ib in ll)
                            //{
                            //    DateTime dtVar = new DateTime();

                            //    if (ib.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                            //    {
                            //        dtVar = trasferimento.DATAPARTENZA;
                            //    }
                            //    else
                            //    {
                            //        dtVar = ib.DATAINIZIOVALIDITA;
                            //    }


                            //    if (!lDateVariazioni.Contains(dtVar))
                            //    {
                            //        lDateVariazioni.Add(dtVar);
                            //    }
                            //}

                            //if (lDateVariazioni?.Any() ?? false)
                            //{
                            //    lDateVariazioni =
                            //        lDateVariazioni.OrderBy(a => a.Year).ThenBy(a => a.Month).ThenBy(a => a.Day).ToList();

                            //    for (int j = 0; j < lDateVariazioni.Count; j++)
                            //    {
                            //        DateTime dv = lDateVariazioni[j];

                            //        //using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                            //        //{


                            //        //    var pd = new EvoluzioneIndennitaModel
                            //        //    {

                            //        //        IndennitaBase = ci.IndennitaDiBase,
                            //        //        percentuale = ci.PercentualeDisagio,
                            //        //        //valore = ci.CoefficienteDiSede,
                            //        //        Coefficiente = ci.CoefficienteDiSede,
                            //        //        IndennitaServizio = ci.IndennitaDiServizio
                            //        //    };




                            //        //    eim.Add(pd);
                            //        //}
                            //    }
                            //}



                        }

                        



                    }
                    
                    return eim;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IndennitaBaseModel GetIndennitaBaseByIdTrasf(decimal idTrasferimento, DateTime dt)
        {
            IndennitaBaseModel ibm = new IndennitaBaseModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                var lib = db.INDENNITA.Find(idTrasferimento).INDENNITABASE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();
                if (lib != null && lib.Count > 0)
                {
                    var ib = lib.First();

                    ibm = new IndennitaBaseModel()
                    {
                        idIndennitaBase = ib.IDINDENNITABASE,
                        idLivello = ib.IDLIVELLO,
                        dataInizioValidita = ib.DATAINIZIOVALIDITA,
                        dataFineValidita = ib.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : ib.DATAFINEVALIDITA,
                        valore = ib.VALORE,
                        valoreResponsabile = ib.VALORERESP,
                        dataAggiornamento = ib.DATAAGGIORNAMENTO,
                        annullato = ib.ANNULLATO,
                        Livello = new LivelloModel()
                        {
                            idLivello = ib.LIVELLI.IDLIVELLO,
                            DescLivello = ib.LIVELLI.LIVELLO
                        },
                    };
                }
            }

            return ibm;
        }

        public IndennitaBaseModel GetIndennitaBaseByIdTrasf(decimal idTrasferimento, DateTime dt, ModelDBISE db)
        {
            IndennitaBaseModel ibm = new IndennitaBaseModel();

            var lib =
                db.INDENNITA.Find(idTrasferimento)
                    .INDENNITABASE.Where(
                        a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA)
                    .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                    .ToList();

            if (lib != null && lib.Count > 0)
            {
                var ib = lib.First();

                ibm = new IndennitaBaseModel()
                {
                    idIndennitaBase = ib.IDINDENNITABASE,
                    idLivello = ib.IDLIVELLO,
                    dataInizioValidita = ib.DATAINIZIOVALIDITA,
                    dataFineValidita = ib.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : ib.DATAFINEVALIDITA,
                    valore = ib.VALORE,
                    valoreResponsabile = ib.VALORERESP,
                    dataAggiornamento = ib.DATAAGGIORNAMENTO,
                    annullato = ib.ANNULLATO,
                    Livello = new LivelloModel()
                    {
                        idLivello = ib.LIVELLI.IDLIVELLO,
                        DescLivello = ib.LIVELLI.LIVELLO
                    },
                };


            }

            return ibm;
        }

        public IndennitaBaseModel GetIndennitaBase(decimal idIndennitaBase)
        {
            IndennitaBaseModel ibm = new IndennitaBaseModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var ib = db.INDENNITABASE.Find(idIndennitaBase);

                if (ib != null && ib.IDINDENNITABASE > 0)
                {
                    ibm = new IndennitaBaseModel()
                    {
                        idIndennitaBase = ib.IDINDENNITABASE,
                        idLivello = ib.IDLIVELLO,
                        dataInizioValidita = ib.DATAINIZIOVALIDITA,
                        dataFineValidita = ib.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : ib.DATAFINEVALIDITA,
                        valore = ib.VALORE,
                        valoreResponsabile = ib.VALORERESP,
                        dataAggiornamento = ib.DATAAGGIORNAMENTO,
                        annullato = ib.ANNULLATO,
                        Livello = new LivelloModel()
                        {
                            idLivello = ib.LIVELLI.IDLIVELLO,
                            DescLivello = ib.LIVELLI.LIVELLO
                        },
                    };
                }
            }

            return ibm;
        }

        public IndennitaBaseModel GetIndennitaBase(decimal idIndennitaBase, ModelDBISE db)
        {
            IndennitaBaseModel ibm = new IndennitaBaseModel();

            var ib = db.INDENNITABASE.Find(idIndennitaBase);

            if (ib != null && ib.IDINDENNITABASE > 0)
            {
                ibm = new IndennitaBaseModel()
                {
                    idIndennitaBase = ib.IDINDENNITABASE,
                    idLivello = ib.IDLIVELLO,
                    dataInizioValidita = ib.DATAINIZIOVALIDITA,
                    dataFineValidita = ib.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : ib.DATAFINEVALIDITA,
                    valore = ib.VALORE,
                    valoreResponsabile = ib.VALORERESP,
                    dataAggiornamento = ib.DATAAGGIORNAMENTO,
                    annullato = ib.ANNULLATO,
                    Livello = new LivelloModel()
                    {
                        idLivello = ib.LIVELLI.IDLIVELLO,
                        DescLivello = ib.LIVELLI.LIVELLO
                    },
                };
            }

            return ibm;
        }

        public IList<IndennitaBaseModel> GetIndennitaBaseByRangeDate(decimal idLivello, DateTime dtIni, DateTime dtFin, ModelDBISE db)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

            var lib =
                db.INDENNITABASE.Where(
                    a =>
                        a.ANNULLATO == false && a.IDLIVELLO == idLivello && a.DATAFINEVALIDITA >= dtIni &&
                        a.DATAINIZIOVALIDITA <= dtFin)
                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                    .ToList();

            if (lib?.Any() ?? false)
            {
                libm = (from ib in lib
                        select new IndennitaBaseModel()
                        {
                            idIndennitaBase = ib.IDINDENNITABASE,
                            idLivello = ib.IDLIVELLO,
                            dataInizioValidita = ib.DATAINIZIOVALIDITA,
                            dataFineValidita =
                                ib.DATAFINEVALIDITA != Utility.DataFineStop() ? ib.DATAFINEVALIDITA : new DateTime?(),
                            valore = ib.VALORE,
                            valoreResponsabile = ib.VALORERESP,
                            dataAggiornamento = ib.DATAAGGIORNAMENTO,
                            annullato = ib.ANNULLATO,
                            Livello = new LivelloModel()
                            {
                                idLivello = ib.LIVELLI.IDLIVELLO,
                                DescLivello = ib.LIVELLI.LIVELLO
                            }
                        }).ToList();
            }

            return libm;
        }


        public IndennitaBaseModel GetIndennitaBaseValida(decimal idLivello, DateTime dt, ModelDBISE db)
        {
            IndennitaBaseModel ibm = new IndennitaBaseModel();

            List<INDENNITABASE> lib = db.INDENNITABASE.Where(a => a.ANNULLATO == false &&
                                                      a.IDLIVELLO == idLivello &&
                                                      dt >= a.DATAINIZIOVALIDITA &&
                                                      dt <= a.DATAFINEVALIDITA)
                                               .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

            if (lib != null && lib.Count > 0)
            {
                var ib = lib.First();

                ibm = new IndennitaBaseModel()
                {
                    idIndennitaBase = ib.IDINDENNITABASE,
                    idLivello = ib.IDLIVELLO,
                    dataInizioValidita = ib.DATAINIZIOVALIDITA,
                    dataFineValidita = ib.DATAFINEVALIDITA != Utility.DataFineStop() ? ib.DATAFINEVALIDITA : new DateTime?(),
                    valore = ib.VALORE,
                    valoreResponsabile = ib.VALORERESP,
                    dataAggiornamento = ib.DATAAGGIORNAMENTO,
                    annullato = ib.ANNULLATO,
                    Livello = new LivelloModel()
                    {
                        idLivello = ib.LIVELLI.IDLIVELLO,
                        DescLivello = ib.LIVELLI.LIVELLO
                    }
                };

            }

            return ibm;
        }
        public static DateTime DataInizioMinimaNonAnnullataIndennitaBase()
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var TuttiNonAnnullati = db.INDENNITABASE.Where(a => a.ANNULLATO == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                if (TuttiNonAnnullati.Count > 0)
                {
                    return (DateTime)TuttiNonAnnullati.First().DATAINIZIOVALIDITA;
                }
            }
            return DateTime.Now;
        }
        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var fm = context.ObjectInstance as IndennitaBaseModel;
            if (fm != null)
            {
                DateTime d = DataInizioMinimaNonAnnullataIndennitaBase();
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
    }
}
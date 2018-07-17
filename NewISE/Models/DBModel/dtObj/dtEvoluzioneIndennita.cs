using NewISE.EF;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Models.Tools;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using NewISE.Models.dtObj.ModelliCalcolo;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtEvoluzioneIndennita : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        //public IList<IndennitaBaseModel> GetIndennita(decimal idTrasferimento)
        //{
        //    List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

        //    try
        //    {
        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
        //            var indennita = trasferimento.INDENNITA;

        //            //List<DateTime> lDateVariazioni = new List<DateTime>();

        //            var ll = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.INDENNITABASE
        //                .Where(a => a.ANNULLATO == false)
        //                .OrderBy(a => a.IDLIVELLO)
        //                .ThenBy(b => b.DATAINIZIOVALIDITA)
        //                .ThenBy(c => c.DATAFINEVALIDITA).ToList();


        //            //List<INDENNITABASE> libm = new List<INDENNITABASE>();
        //            //libm = db.INDENNITABASE.Where(a => a.ANNULLATO == false
        //            //&& a.IDLIVELLO == idLivello).OrderBy(b => b.DATAINIZIOVALIDITA).ThenBy(c => c.DATAFINEVALIDITA).ToList();
        //            //if (libm.Count != 0)
        //            //    tmp = libm.First().IDINDENNITABASE;

        //            using (dtTrasferimento dttrasf = new dtTrasferimento())
        //            {
        //                using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
        //                {
        //                    RuoloDipendenteModel rdm = dtrd.GetRuoloDipendenteByIdIndennita(idTrasferimento);

        //                    using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO))
        //                    {
        //                        dipInfoTrasferimentoModel dipInfoTrasf = dttrasf.GetInfoTrasferimento(idTrasferimento);

        //                            libm = (from e in ll
        //                                    select new IndennitaBaseModel()
        //                                    {
        //                                        idIndennitaBase = e.IDINDENNITABASE,
        //                                        idLivello = e.IDLIVELLO,
        //                                        dataInizioValidita = e.DATAINIZIOVALIDITA,
        //                                        dataFineValidita = e.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : e.DATAFINEVALIDITA,
        //                                        //dataFineValidita = e.DATAFINEVALIDITA,
        //                                        valore = e.VALORE,
        //                                        valoreResponsabile = e.VALORERESP,
        //                                        dataAggiornamento = e.DATAAGGIORNAMENTO,
        //                                        annullato = e.ANNULLATO,
        //                                        Livello = new LivelloModel()
        //                                        {
        //                                            idLivello = e.LIVELLI.IDLIVELLO,
        //                                            DescLivello = e.LIVELLI.LIVELLO
        //                                        },
        //                                        RuoloUfficio = new RuoloUfficioModel()
        //                                        {
        //                                            idRuoloUfficio = rdm.RuoloUfficio.idRuoloUfficio,
        //                                            DescrizioneRuolo = rdm.RuoloUfficio.DescrizioneRuolo
        //                                        },
        //                                        dipInfoTrasferimento = new dipInfoTrasferimentoModel
        //                                        {
        //                                            Decorrenza = dipInfoTrasf.Decorrenza,
        //                                            indennitaPersonale = dipInfoTrasf.indennitaPersonale,
        //                                            indennitaServizio = dipInfoTrasf.indennitaServizio,
        //                                            maggiorazioniFamiliari = dipInfoTrasf.maggiorazioniFamiliari
        //                                        },
        //                                        EvoluzioneIndennita = new EvoluzioneIndennitaModel
        //                                        {
        //                                            IndennitaBase = ci.IndennitaDiBase,
        //                                            PercentualeDisagio = ci.PercentualeDisagio,
        //                                            CoefficienteSede = ci.CoefficienteDiSede,
        //                                            IndennitaServizio = ci.IndennitaDiServizio,
        //                                            MaggiorazioniFamiliari = ci.MaggiorazioniFamiliari,
        //                                            IndennitaPersonale = ci.IndennitaPersonale

        //                                        }
        //                                    }).ToList();
        //                    }
        //                }
        //            }

        //            return libm;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public IList<IndennitaBaseModel> GetIndennita(decimal idTrasferimento)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtTrasferimento dttrasf = new dtTrasferimento())
                    {
                        //var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                        //var indennita = trasferimento.INDENNITA;
                        
                        using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                        {
                            RuoloDipendenteModel rdm = dtrd.GetRuoloDipendenteByIdIndennita(idTrasferimento);

                            List<DateTime> lDateVariazioni = new List<DateTime>();

                            var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                            var indennita = trasferimento.INDENNITA;

                            #region Variazioni Indennita di Base
                            var ll = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.INDENNITABASE
                                .Where(a => a.ANNULLATO == false)
                                .OrderBy(a => a.IDLIVELLO)
                                .ThenBy(a => a.DATAINIZIOVALIDITA)
                                .ThenBy(a => a.DATAFINEVALIDITA).ToList();


                            if (ll?.Any() ?? false)
                            {
                                var indennitabase = ll.First();
                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, trasferimento.DATAPARTENZA, db))
                                {
                                    
                                    libm = (from e in ll
                                            select new IndennitaBaseModel()
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
                                                EvoluzioneIndennita = new EvoluzioneIndennitaModel
                                                {   
                                                    IndennitaBase = ci.IndennitaDiBase,
                                                    PercentualeDisagio = ci.PercentualeDisagio,
                                                    CoefficienteSede = ci.CoefficienteDiSede,
                                                    IndennitaServizio = ci.IndennitaDiServizio,
                                                    MaggiorazioniFamiliari = ci.MaggiorazioniFamiliari,
                                                    IndennitaPersonale = ci.IndennitaPersonale

                                                }
                                            }).ToList();
                                }
                            }

                                    //foreach (var ib in ll)
                                    //{
                                    //    DateTime dtVar = new DateTime();

                                    //    if (ib.DATAINIZIOVALIDITA > trasferimento.DATAPARTENZA)
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
                                    #endregion

                            #region Variazioni del coefficiente di sede

                                    var lCoefSede =
                                        indennita.COEFFICIENTESEDE.Where(a => a.ANNULLATO == false)
                                        .OrderBy(a => a.IDCOEFFICIENTESEDE)
                                        .ThenBy(a => a.DATAINIZIOVALIDITA)
                                        .ThenBy(a => a.DATAFINEVALIDITA).ToList();

                            if (lCoefSede?.Any() ?? false)
                            {
                                var indennitabase = lCoefSede.First();
                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, trasferimento.DATAPARTENZA, db))
                                {

                                    libm = (from e in ll
                                            select new IndennitaBaseModel()
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
                                                EvoluzioneIndennita = new EvoluzioneIndennitaModel
                                                {
                                                    IndennitaBase = ci.IndennitaDiBase,
                                                    PercentualeDisagio = ci.PercentualeDisagio,
                                                    CoefficienteSede = ci.CoefficienteDiSede,
                                                    IndennitaServizio = ci.IndennitaDiServizio,
                                                    MaggiorazioniFamiliari = ci.MaggiorazioniFamiliari,
                                                    IndennitaPersonale = ci.IndennitaPersonale

                                                }
                                            }).ToList();
                                }
                            }

                            #endregion

                            //#region Variazioni percentuale di disagio

                            //var lPercDisagio =
                            //    indennita.PERCENTUALEDISAGIO.Where(a => a.ANNULLATO == false)
                            //    .OrderBy(a => a.IDPERCENTUALEDISAGIO)
                            //    .ThenBy(a => a.DATAINIZIOVALIDITA)
                            //    .ThenBy(a => a.DATAFINEVALIDITA).ToList();

                            //foreach (var pd in lPercDisagio)
                            //{
                            //    DateTime dtVar = new DateTime();

                            //    if (pd.DATAINIZIOVALIDITA > trasferimento.DATAPARTENZA)
                            //    {
                            //        dtVar = trasferimento.DATAPARTENZA;
                            //    }
                            //    else
                            //    {
                            //        dtVar = pd.DATAINIZIOVALIDITA;
                            //    }


                            //    if (!lDateVariazioni.Contains(dtVar))
                            //    {
                            //        lDateVariazioni.Add(dtVar);
                            //    }
                            //}

                            //#endregion

                            //if (lDateVariazioni?.Any() ?? false)
                            //{
                            //    //lDateVariazioni =
                            //    //    lDateVariazioni.OrderBy(a => a.Year).ThenBy(a => a.Month).ThenBy(a => a.Day).ToList();

                            //    for (int j = 0; j < lDateVariazioni.Count; j++)
                            //    {
                            //        DateTime dv = lDateVariazioni[j];

                            //        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO,trasferimento.DATAPARTENZA))
                            //        {
                            //            dipInfoTrasferimentoModel dipInfoTrasf = dttrasf.GetInfoTrasferimento(idTrasferimento);

                            //            libm = (from e in ll
                            //                    select new IndennitaBaseModel()
                            //                    {
                            //                        idIndennitaBase = e.IDINDENNITABASE,
                            //                        idLivello = e.IDLIVELLO,
                            //                        dataInizioValidita = e.DATAINIZIOVALIDITA,
                            //                        dataFineValidita = e.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : e.DATAFINEVALIDITA,
                            //                        valore = e.VALORE,
                            //                        valoreResponsabile = e.VALORERESP,
                            //                        dataAggiornamento = e.DATAAGGIORNAMENTO,
                            //                        annullato = e.ANNULLATO,
                            //                        Livello = new LivelloModel()
                            //                        {
                            //                            idLivello = e.LIVELLI.IDLIVELLO,
                            //                            DescLivello = e.LIVELLI.LIVELLO
                            //                        },
                            //                        RuoloUfficio = new RuoloUfficioModel()
                            //                        {
                            //                            idRuoloUfficio = rdm.RuoloUfficio.idRuoloUfficio,
                            //                            DescrizioneRuolo = rdm.RuoloUfficio.DescrizioneRuolo
                            //                        },
                            //                        //dipInfoTrasferimento = new dipInfoTrasferimentoModel
                            //                        //{
                            //                        //    Decorrenza = dipInfoTrasf.Decorrenza,
                            //                        //    indennitaPersonale = dipInfoTrasf.indennitaPersonale,
                            //                        //    indennitaServizio = dipInfoTrasf.indennitaServizio,
                            //                        //    maggiorazioniFamiliari = dipInfoTrasf.maggiorazioniFamiliari
                            //                        //},
                            //                        EvoluzioneIndennita = new EvoluzioneIndennitaModel
                            //                        {
                            //                            dataTest = dv,
                            //                            IndennitaBase = ci.IndennitaDiBase,
                            //                            PercentualeDisagio = ci.PercentualeDisagio,
                            //                            CoefficienteSede = ci.CoefficienteDiSede,
                            //                            IndennitaServizio = ci.IndennitaDiServizio,
                            //                            MaggiorazioniFamiliari = ci.MaggiorazioniFamiliari,
                            //                            IndennitaPersonale = ci.IndennitaPersonale

                            //                        }
                            //                    }).ToList();
                            //        }
                                    
                            //    }
                            //}
                        }
                    }

                    return libm;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<EvoluzioneIndennitaModel> GetIndennitaEvoluzione(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();
            
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var indennita = trasferimento.INDENNITA;
                    
                    List<DateTime> lDateVariazioni = new List<DateTime>();

                    #region Variazioni di indennità di base

                    var ll =
                        db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.INDENNITABASE
                        .Where(a => a.ANNULLATO == false).OrderBy(a => a.IDLIVELLO)
                            .ThenBy(a => a.DATAINIZIOVALIDITA)
                            .ThenBy(a => a.DATAFINEVALIDITA).ToList();
                    

                    foreach (var ib in ll)
                        {
                            DateTime dtVar = new DateTime();

                            if (ib.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                            {
                                dtVar = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtVar = ib.DATAINIZIOVALIDITA;
                            }


                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                            }
                        }

                    #endregion

                    #region Variazioni del coefficiente di sede

                    var lrd =
                        db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.COEFFICIENTESEDE
                        .Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.IDCOEFFICIENTESEDE)
                        .ThenBy(a => a.DATAINIZIOVALIDITA)
                        .ThenBy(a => a.DATAFINEVALIDITA).ToList();

                    foreach (var cs in lrd)
                        {
                            DateTime dtVar = new DateTime();

                            if (cs.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                            {
                                dtVar = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtVar = cs.DATAINIZIOVALIDITA;
                            }

                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                            }
                        }

                    #endregion

                    #region Variazioni percentuale di disagio

                    var perc =
                        db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.PERCENTUALEDISAGIO
                        .Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.IDPERCENTUALEDISAGIO)
                        .ThenBy(a => a.DATAINIZIOVALIDITA)
                        .ThenBy(a => a.DATAFINEVALIDITA).ToList();
                    

                    foreach (var pd in perc)
                        {
                            DateTime dtVar = new DateTime();

                            if (pd.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                            {
                                dtVar = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtVar = pd.DATAINIZIOVALIDITA;
                            }

                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                            }
                        }




                    

                    #endregion

                    lDateVariazioni.Add(new DateTime(9999, 12, 31));

                    if (lDateVariazioni?.Any() ?? false)
                    {
                        for (int j = 0; j < lDateVariazioni.Count; j++)
                        {
                            DateTime dv = lDateVariazioni[j];

                            if (dv < Utility.DataFineStop())
                            {
                                

                                    using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                    {
                                        EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                                        // Inserire le date di variazione delle Indennità
                                        //xx.dataInizioValidita = pd.DATAINIZIOVALIDITA;
                                        //xx.dataFineValidita = pd.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? pd.DATAFINEVALIDITA : new EvoluzioneIndennitaModel().dataFineValidita;
                                        //xx.valore = pd.VALORE;
                                        //xx.valoreResponsabile = pd.VALORERESP;

                                        xx.dataInizioValidita = dv;
                                        xx.IndennitaBase = ci.IndennitaDiBase;
                                        xx.PercentualeDisagio = ci.PercentualeDisagio;
                                        xx.CoefficienteSede = ci.CoefficienteDiSede;
                                        xx.IndennitaServizio = ci.IndennitaDiServizio;

                                        eim.Add(xx);
                                    }

                                


                                //            foreach (var pd in ll)
                                //            {

                                //                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv))
                                //                {

                                //                    EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();
                                //                    xx.dataInizioValidita = pd.DATAINIZIOVALIDITA;
                                //                    //xx.dataFineValidita = pd.DATAFINEVALIDITA;
                                //                    xx.dataFineValidita = pd.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? pd.DATAFINEVALIDITA : new EvoluzioneIndennitaModel().dataFineValidita;
                                //                    xx.valore = pd.VALORE;
                                //                    xx.valoreResponsabile = pd.VALORERESP;

                                //                    eim.Add(xx);

                                //                }
                                //            }

                                //            foreach (var pd in lrd)
                                //            {
                                //                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv))
                                //                {


                                //                    EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();
                                //                    xx.dataInizioValidita = pd.DATAINIZIOVALIDITA;
                                //                    //xx.dataFineValidita = pd.DATAFINEVALIDITA;
                                //                    xx.dataFineValidita = pd.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? pd.DATAFINEVALIDITA : new EvoluzioneIndennitaModel().dataFineValidita;
                                //                    xx.CoefficienteSede = ci.CoefficienteDiSede;
                                //                    xx.IndennitaServizio = ci.IndennitaDiServizio;

                                //                    eim.Add(xx);
                                //                }
                                //            }

                                //            foreach (var pd in perc)
                                //            {
                                //                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv))
                                //                {

                                //                    EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();
                                //                    xx.dataInizioValidita = pd.DATAINIZIOVALIDITA;
                                //                    //xx.dataFineValidita = pd.DATAFINEVALIDITA;
                                //                    xx.dataFineValidita = pd.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? pd.DATAFINEVALIDITA : new EvoluzioneIndennitaModel().dataFineValidita;
                                //                    xx.PercentualeDisagio = ci.PercentualeDisagio;
                                //                    xx.IndennitaServizio = ci.IndennitaDiServizio;

                                //                    eim.Add(xx);
                                //                }
                                //            }



                            }
                        }
                    }

                }

                return eim;
            }
            catch (Exception)
            {

                throw;
            }
        }
        
    }
}
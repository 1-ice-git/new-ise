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

        public IList<IndennitaBaseModel> GetIndennita(decimal idTrasferimento)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var indennita = trasferimento.INDENNITA;

                    var ll = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.INDENNITABASE
                        .Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.IDLIVELLO)
                        .ThenBy(b => b.DATAINIZIOVALIDITA)
                        .ThenBy(c => c.DATAFINEVALIDITA).ToList();

                    using (dtTrasferimento dttrasf = new dtTrasferimento())
                    {
                        using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                        {
                            RuoloDipendenteModel rdm = dtrd.GetRuoloDipendenteByIdIndennita(idTrasferimento);

                            //using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO))
                            //{
                                dipInfoTrasferimentoModel dipInfoTrasf = dttrasf.GetInfoTrasferimento(idTrasferimento);

                                libm = (from e in ll
                                        select new IndennitaBaseModel()
                                        {
                                            idIndennitaBase = e.IDINDENNITABASE,
                                            idLivello = e.IDLIVELLO,
                                            dataInizioValidita = e.DATAINIZIOVALIDITA,
                                            dataFineValidita = e.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : e.DATAFINEVALIDITA,
                                            //dataFineValidita = e.DATAFINEVALIDITA,
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
                                            //dipInfoTrasferimento = new dipInfoTrasferimentoModel
                                            //{
                                            //    Decorrenza = dipInfoTrasf.Decorrenza,
                                            //    indennitaPersonale = dipInfoTrasf.indennitaPersonale,
                                            //    indennitaServizio = dipInfoTrasf.indennitaServizio,
                                            //    maggiorazioniFamiliari = dipInfoTrasf.maggiorazioniFamiliari
                                            //},
                                            //EvoluzioneIndennita = new EvoluzioneIndennitaModel
                                            //{
                                            //    IndennitaBase = ci.IndennitaDiBase,
                                            //    PercentualeDisagio = ci.PercentualeDisagio,
                                            //    CoefficienteSede = ci.CoefficienteDiSede,
                                            //    IndennitaServizio = ci.IndennitaDiServizio,
                                            //    MaggiorazioniFamiliari = ci.MaggiorazioniFamiliari,
                                            //    IndennitaPersonale = ci.IndennitaPersonale

                                            //}
                                        }).ToList();
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
                        .Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                    

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
                            lDateVariazioni.Sort();
                        }
                        }

                    #endregion

                    #region Variazioni del coefficiente di sede

                    var lrd =
                        db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.COEFFICIENTESEDE
                        .Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

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
                            lDateVariazioni.Sort();
                        }
                        }

                    #endregion

                    #region Variazioni percentuale di disagio

                    var perc =
                        db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.PERCENTUALEDISAGIO
                        .Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                    

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
                            lDateVariazioni.Sort();
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
                                DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                    {
                                        EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                                        xx.dataInizioValidita = dv;
                                        xx.dataFineValidita = dvSucc;
                                        xx.IndennitaBase = ci.IndennitaDiBase;
                                        xx.PercentualeDisagio = ci.PercentualeDisagio;
                                        xx.CoefficienteSede = ci.CoefficienteDiSede;
                                        xx.IndennitaServizio = ci.IndennitaDiServizio;
                                        xx.IndennitaPersonale = ci.IndennitaPersonale;
                                        xx.MaggiorazioniFamiliari = ci.MaggiorazioniFamiliari;

                                        eim.Add(xx);
                                    }
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
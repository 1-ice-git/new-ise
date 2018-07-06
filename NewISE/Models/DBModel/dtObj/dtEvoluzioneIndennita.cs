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

                    #region Lettura Variazioni di indennità di base

                    var ll =
                        db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.INDENNITABASE.Where(
                            a =>
                            a.ANNULLATO == false).OrderBy(a => a.IDLIVELLO)
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

                    #region Lettura Variazioni del coefficiente di sede

                    var lrd = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.COEFFICIENTESEDE.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDCOEFFICIENTESEDE).ThenBy(a => a.DATAINIZIOVALIDITA).ThenBy(a => a.DATAFINEVALIDITA).ToList();

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

                    #region Lettura Variazioni percentuale di disagio

                    var perc = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.PERCENTUALEDISAGIO.Where(a => a.ANNULLATO == false).ToList();

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
                    
                    if (lDateVariazioni?.Any() ?? false)
                    {
                        lDateVariazioni =
                            lDateVariazioni.OrderBy(a => a.Year).ThenBy(a => a.Month).ThenBy(a => a.Day).ToList();

                        for (int j = 0; j < lDateVariazioni.Count; j++)
                        {
                            DateTime dv = lDateVariazioni[j];

                            using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                            {
                                

                                    var pd = new EvoluzioneIndennitaModel
                                    {

                                        IndennitaBase = ci.IndennitaDiBase,
                                        percentuale = ci.PercentualeDisagio,
                                        //valore = ci.CoefficienteDiSede,
                                        Coefficiente = ci.CoefficienteDiSede,
                                        IndennitaServizio = ci.IndennitaDiServizio
                                    };

                                


                                eim.Add(pd);
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
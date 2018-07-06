using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Models.Tools;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using NewISE.Models.dtObj.ModelliCalcolo;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtIndennitaServizio : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public IList<IndennitaBaseModel> GetIndennitaServizio(decimal idTrasferimento)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtCoefficenteSede dtcs = new dtCoefficenteSede())
                    {
                        CoefficientiSedeModel csm = dtcs.GetCoefficenteSedeByIdTrasferimento(idTrasferimento);

                        using (dtPercentualeDisagio dtpd = new dtPercentualeDisagio())
                        {
                            PercentualeDisagioModel pdm = dtpd.GetPercentualeDisagioByIdTrasferimento(idTrasferimento);

                            using (dtTrasferimento dttrasf = new dtTrasferimento())
                            {
                                dipInfoTrasferimentoModel dipInfoTrasf = dttrasf.GetInfoTrasferimento(idTrasferimento);

                                var ll = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.INDENNITABASE.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDLIVELLO).ThenBy(a => a.DATAINIZIOVALIDITA).ThenBy(a => a.DATAFINEVALIDITA).ToList();

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
                                            CoefficenteSede = new CoefficientiSedeModel
                                            {
                                                idCoefficientiSede = csm.idCoefficientiSede,
                                                idUfficio = csm.idUfficio

                                            },
                                            PercentualeDisagio = new PercentualeDisagioModel
                                            {
                                                idPercentualeDisagio = pdm.idPercentualeDisagio,
                                                idUfficio = pdm.idUfficio,
                                                dataInizioValidita = pdm.dataInizioValidita,
                                                dataFineValidita = pdm.dataFineValidita,
                                                dataAggiornamento = pdm.dataAggiornamento,
                                                annullato = pdm.annullato

                                            },
                                            dipInfoTrasferimento = new dipInfoTrasferimentoModel
                                            {
                                                Decorrenza = dipInfoTrasf.Decorrenza,
                                                indennitaServizio = dipInfoTrasf.indennitaServizio

                                            }
                                        }).ToList();
                            }
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


        public IList<CoefficientiSedeModel> GetIndennitaServizio1(decimal idTrasferimento)
        {
            List<CoefficientiSedeModel> csm = new List<CoefficientiSedeModel>();
            IndennitaServizioModel IndennitaServizio = new IndennitaServizioModel();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                   


                    //var t = db.TRASFERIMENTO.Find(idTrasferimento);
                    //var ind = t.INDENNITA;

                    //var coeff = ind.COEFFICIENTESEDE.Where(
                    //          a => a.ANNULLATO == false)
                    //          .OrderBy(a => a.IDCOEFFICIENTESEDE)
                    //          .ThenBy(a => a.DATAINIZIOVALIDITA)
                    //          .ThenBy(a => a.DATAFINEVALIDITA);

                    //if (coeff?.Any() ?? false)
                    //{
                    //    foreach (var pd in coeff)
                    //    {
                    //        var pdm = new CoefficientiSedeModel()
                    //        {
                    //            idCoefficientiSede = pd.IDCOEFFICIENTESEDE,
                    //            idUfficio = pd.IDUFFICIO,
                    //            dataInizioValidita = pd.DATAINIZIOVALIDITA,
                    //            dataFineValidita =
                    //            pd.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : pd.DATAFINEVALIDITA,
                    //            dataAggiornamento = pd.DATAAGGIORNAMENTO,
                    //            annullato = pd.ANNULLATO
                    //        };

                    //        csm.Add(pdm);
                    //    }
                    //}
                    

                    return csm;

                }
            }
            
            catch (Exception)
            {

                throw;
            }
        }
        
        public IList<IndennitaServizioModel> GetIndennitaServizio2(decimal idTrasferimento)
        {
            List<IndennitaServizioModel> lism = new List<IndennitaServizioModel>();
            
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
                                //ELABINDENNITA ei = new ELABINDENNITA()
                                //{
                                //    IDTRASFINDENNITA = trasferimento.IDTRASFERIMENTO,
                                //    INDENNITABASE = ci.IndennitaDiBase,
                                //    COEFFICENTESEDE = ci.CoefficienteDiSede,
                                //    PERCENTUALEDISAGIO = ci.PercentualeDisagio,
                                //    PERCENTUALEMAGCONIUGE = ci.PercentualeMaggiorazioneConiuge,
                                //    PENSIONECONIUGE = ci.PensioneConiuge,
                                //    DATAOPERAZIONE = DateTime.Now,
                                //    ANNULLATO = false
                                //};

                                //    indennita.ELABINDENNITA.Add(ei);

                                var pd = new IndennitaServizioModel
                                {
                                    
                                    IndennitaBase = ci.IndennitaDiBase,
                                    percentuale = ci.PercentualeDisagio,
                                    valore = ci.CoefficienteDiSede,
                                    IndennitaServizio = ci.IndennitaDiServizio
                                };

                                lism.Add(pd);
                            }
                        }
                    }
                }

                return lism;
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}



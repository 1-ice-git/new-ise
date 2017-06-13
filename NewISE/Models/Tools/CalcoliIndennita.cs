using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.Tools
{
    public class CalcoliIndennita : IDisposable
    {
        private TrasferimentoModel trasferimento { get; set; }
        private DateTime dtDatiParametri { get; set; }
        private RuoloUfficioModel ruoloUfficio { get; set; }
        private IndennitaModel indennita { get; set; }
        private IndennitaBaseModel indennitaBase { get; set; }
        private CoefficientiSedeModel coefficenteSede { get; set; }
        private PercentualeDisagioModel percentualeDisagio { get; set; }

        private decimal indennitaBaseNoRiduzione { get; set; }


        public decimal indennitaBaseRiduzione { get; set; }        
        public decimal indennitaServizio { get; set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public CalcoliIndennita(string matricola)
        {
            
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        trasferimento = dtt.GetUltimoTrasferimentoByMatricola(matricola, db);

                        if (trasferimento.dataRientro.HasValue)
                        {
                            dtDatiParametri = trasferimento.dataRientro.Value;
                        }
                        else
                        {
                            dtDatiParametri = DateTime.Now.Date;
                        }

                        using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                        {
                            RuoloUfficioModel rum = new RuoloUfficioModel();
                            ruoloUfficio = dtrd.GetRuoloDipendente(trasferimento.RuoloUfficio.idRuoloUfficio, dtDatiParametri, db).RuoloUfficio;


                        }

                        var lindennita = trasferimento.lIndennita.OrderByDescending(a => a.dataInizio).ToList();

                        if (lindennita != null && lindennita.Count > 0)
                        {
                            indennita = lindennita.First();

                            #region IndennitaDiBase
                            using (dtIndennitaBase dtib = new dtIndennitaBase())
                            {
                                if (ruoloUfficio.idRuoloUfficio == (decimal)EnumRuoloUfficio.Dirigente || ruoloUfficio.idRuoloUfficio == (decimal)EnumRuoloUfficio.Responsabile)
                                {
                                    indennitaBase = dtib.GetIndennitaBase(indennita.idIndennitaBase, db);
                                    if (indennitaBase != null && indennitaBase.idIndennitaBase > 0)
                                    {
                                        indennitaBaseNoRiduzione = indennitaBase.valoreResponsabile;
                                        if (indennitaBase.Riduzioni.percentuale > 0)
                                        {
                                            indennitaBaseRiduzione = indennitaBaseNoRiduzione * (indennitaBase.Riduzioni.percentuale / 100);
                                        }
                                        else
                                        {
                                            indennitaBaseRiduzione = indennitaBaseNoRiduzione;
                                        }

                                    }

                                }

                                else if (ruoloUfficio.idRuoloUfficio == (decimal)EnumRuoloUfficio.Collaboratore || ruoloUfficio.idRuoloUfficio == (decimal)EnumRuoloUfficio.Assistente)
                                {
                                    indennitaBase = dtib.GetIndennitaBase(indennita.idIndennitaBase, db);
                                    if (indennitaBase != null && indennitaBase.idIndennitaBase > 0)
                                    {
                                        indennitaBaseNoRiduzione = indennitaBase.valore;
                                        if (indennitaBase.Riduzioni.percentuale > 0)
                                        {
                                            indennitaBaseRiduzione = indennitaBaseNoRiduzione * (indennitaBase.Riduzioni.percentuale / 100);
                                        }
                                        else
                                        {
                                            indennitaBaseRiduzione = indennitaBaseNoRiduzione;
                                        }

                                    }
                                }

                            }
                            #endregion

                            //coefficenteSede = indennita.CoefficenteSede;

                            using (dtCoefficenteSede dtcs = new dtCoefficenteSede())
                            {
                                coefficenteSede = dtcs.GetCoefficenteSede(indennita.idCoefficenteSede, db);

                            }

                            using (dtPercentualeDisagio dtpd=new dtPercentualeDisagio())
                            {
                                percentualeDisagio = dtpd.GetPercentualeDisagio(indennita.idPercentualeDisagio, db);

                            }

                            indennitaServizio = (((indennitaBaseRiduzione * coefficenteSede.valore) + (indennitaBaseRiduzione)) + (((indennitaBaseRiduzione * coefficenteSede.valore) + (indennitaBaseRiduzione)) * (percentualeDisagio.percentuale / 100)));





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

        


    }
}
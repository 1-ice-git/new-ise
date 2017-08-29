using NewISE.EF;
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
        private decimal indennitaBaseNoRiduzione { get; set; }
        private CoefficientiSedeModel coefficenteSede { get; set; }
        private PercentualeDisagioModel percentualeDisagio { get; set; }
        private MaggiorazioniFamiliariModel maggiorazioniFamiliari { get; set; }
        private ConiugeModel coniuge { get; set; }
        private PercentualeMagConiugeModel percentualeMaggiorazioneConiuge { get; set; }
        private PensioneConiugeModel pensioneConiuge { get; set; }
        private List<FigliModel> figli { get; set; }

        //private DateTime? _dataCalcoloIndennita;
        ///// <summary>
        ///// Proprietà in sola scrittura per il calcolo dell'indennità.
        ///// Se la 
        ///// </summary>
        //public DateTime? DataCalcoloIndennita
        //{
        //    set { _dataCalcoloIndennita = value; }
        //}


        public decimal indennitaBaseRiduzione { get; set; } = 0;
        public decimal indennitaServizio { get; set; } = 0;
        public decimal MaggiorazioneConiuge { get; set; } = 0;
        public decimal MaggiorazioneFigli { get; set; } = 0;
        public decimal MaggiorazioneFamiliari { get; set; } = 0;


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public CalcoliIndennita(string matricola, DateTime? dataCalcoloIndennita = null)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        trasferimento = dtt.GetUltimoTrasferimentoByMatricola(matricola, db);

                        if (dataCalcoloIndennita.HasValue)
                        {
                            if (trasferimento.dataRientro.HasValue)
                            {
                                if (trasferimento.dataRientro.Value < dataCalcoloIndennita.Value)
                                {
                                    dtDatiParametri = trasferimento.dataRientro.Value;
                                }
                                else
                                {
                                    if (trasferimento.dataPartenza > dataCalcoloIndennita.Value)
                                    {
                                        dtDatiParametri = trasferimento.dataPartenza;
                                    }
                                    else
                                    {
                                        dtDatiParametri = dataCalcoloIndennita.Value;
                                    }
                                }
                            }
                            else
                            {
                                if (trasferimento.dataPartenza > dataCalcoloIndennita.Value)
                                {
                                    dtDatiParametri = trasferimento.dataPartenza;
                                }
                                else
                                {
                                    dtDatiParametri = dataCalcoloIndennita.Value;
                                }
                            }
                        }
                        else
                        {
                            if (trasferimento.dataRientro.HasValue)
                            {
                                if (trasferimento.dataRientro.Value < DateTime.Now)
                                {
                                    dtDatiParametri = trasferimento.dataRientro.Value;
                                }
                                else
                                {
                                    if (trasferimento.dataPartenza > DateTime.Now)
                                    {
                                        dtDatiParametri = trasferimento.dataPartenza;
                                    }
                                    else
                                    {
                                        dtDatiParametri = DateTime.Now;
                                    }
                                }
                            }
                            else
                            {
                                //dtDatiParametri = trasferimento.dataPartenza > Utility.GetDtInizioMeseCorrente() ? trasferimento.dataPartenza : Utility.GetDtInizioMeseCorrente();
                                if (trasferimento.dataPartenza > DateTime.Now)
                                {
                                    dtDatiParametri = trasferimento.dataPartenza;
                                }
                                else
                                {
                                    dtDatiParametri = DateTime.Now;
                                }
                            }
                        }


                        using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                        {
                            RuoloUfficioModel rum = new RuoloUfficioModel();
                            ruoloUfficio =
                                dtrd.GetRuoloDipendenteByIdTrasferimento(trasferimento.idTrasferimento, dtDatiParametri,
                                    db).RuoloUfficio;
                        }

                        using (dtIndennita dti = new dtIndennita())
                        {
                            IndennitaModel indennita = dti.GetIndennitaByIdTrasferimento(trasferimento.idTrasferimento,
                                db);

                            using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                            {
                                List<RuoloDipendenteModel> lrdm = new List<RuoloDipendenteModel>();

                                lrdm.Add(dtrd.GetRuoloDipendenteByIdTrasferimento(trasferimento.idTrasferimento,
                                    dtDatiParametri, db));
                                if (lrdm != null && lrdm.Count > 0)
                                {
                                    indennita.RuoloDipendente = lrdm;
                                }
                                else
                                {
                                    throw new Exception("Il ruolo del dipendente non risulta registrato per l'utente " +
                                                        trasferimento.Dipendente.Nominativo + " (" +
                                                        trasferimento.Dipendente.matricola + ")");
                                }
                            }

                            #region IndennitaDiBase

                            using (dtIndennitaBase dtib = new dtIndennitaBase())
                            {
                                if (ruoloUfficio.idRuoloUfficio == (decimal)EnumRuoloUfficio.Dirigente ||
                                    ruoloUfficio.idRuoloUfficio == (decimal)EnumRuoloUfficio.Responsabile)
                                {
                                    indennitaBase = dtib.GetIndennitaBaseByIdTrasf(indennita.idTrasfIndennita,
                                        dtDatiParametri, db);
                                    if (indennitaBase != null && indennitaBase.idIndennitaBase > 0)
                                    {
                                        indennitaBaseNoRiduzione = indennitaBase.valoreResponsabile;
                                        if (indennitaBase.Riduzioni.percentuale > 0)
                                        {
                                            indennitaBaseRiduzione = indennitaBaseNoRiduzione *
                                                                     indennitaBase.Riduzioni.percentuale / 100;
                                        }
                                        else
                                        {
                                            indennitaBaseRiduzione = indennitaBaseNoRiduzione;
                                        }
                                    }
                                }

                                else if (ruoloUfficio.idRuoloUfficio == (decimal)EnumRuoloUfficio.Collaboratore ||
                                         ruoloUfficio.idRuoloUfficio == (decimal)EnumRuoloUfficio.Assistente)
                                {
                                    indennitaBase = dtib.GetIndennitaBaseByIdTrasf(indennita.idTrasfIndennita,
                                        dtDatiParametri, db);
                                    if (indennitaBase != null && indennitaBase.idIndennitaBase > 0)
                                    {
                                        indennitaBaseNoRiduzione = indennitaBase.valore;
                                        if (indennitaBase.Riduzioni.percentuale > 0)
                                        {
                                            indennitaBaseRiduzione = indennitaBaseNoRiduzione *
                                                                     indennitaBase.Riduzioni.percentuale / 100;
                                        }
                                        else
                                        {
                                            indennitaBaseRiduzione = indennitaBaseNoRiduzione;
                                        }
                                    }
                                }
                            }

                            #endregion

                            #region Indennità di servizio

                            using (dtCoefficenteSede dtcs = new dtCoefficenteSede())
                            {
                                coefficenteSede = dtcs.GetCoefficenteSedeByIdTrasf(indennita.idTrasfIndennita,
                                    dtDatiParametri, db);
                            }

                            using (dtPercentualeDisagio dtpd = new dtPercentualeDisagio())
                            {
                                percentualeDisagio = dtpd.GetPercentualeDisagioByIdTrasf(indennita.idTrasfIndennita,
                                    dtDatiParametri, db);
                            }

                            indennitaServizio = (((indennitaBaseRiduzione * coefficenteSede.valore) +
                                                  indennitaBaseRiduzione) +
                                                 (((indennitaBaseRiduzione * coefficenteSede.valore) +
                                                   indennitaBaseRiduzione) / 100 * percentualeDisagio.percentuale));

                            #endregion
                        }

                        #region Maggiorazioni familiari

                        using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                        {
                            maggiorazioniFamiliari =
                                dtmf.GetMaggiorazioniFamiliariByIDTrasf(trasferimento.idTrasferimento, db);
                            if (maggiorazioniFamiliari?.HasValue() ?? false)
                            {
                                if (maggiorazioniFamiliari.attivazioneMaggiorazioni)
                                {
                                    using (dtConiuge dtc = new dtConiuge())
                                    {
                                        coniuge =
                                            dtc.GetConiugeByIdMagFam(maggiorazioniFamiliari.idMaggiorazioniFamiliari,
                                                dtDatiParametri, db);
                                        if (coniuge.HasValue())
                                        {
                                            using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                                            {
                                                percentualeMaggiorazioneConiuge =
                                                    dtpc.GetPercentualeMaggiorazioneConiuge(coniuge.idTipologiaConiuge,
                                                        dtDatiParametri, db);
                                            }

                                            using (dtPensione dtp = new dtPensione())
                                            {
                                                pensioneConiuge = dtp.GetPensioniByIdConiuge(coniuge.idConiuge,
                                                    dtDatiParametri, db);
                                            }
                                            if (percentualeMaggiorazioneConiuge.percentualeConiuge > 0)
                                            {
                                                MaggiorazioneConiuge = (indennitaServizio *
                                                                        percentualeMaggiorazioneConiuge
                                                                            .percentualeConiuge / 100);
                                            }
                                        }
                                    }


                                    using (dtFigli dtf = new dtFigli())
                                    {
                                        figli =
                                            dtf.GetFigliByIdMagFam(maggiorazioniFamiliari.idMaggiorazioniFamiliari,
                                                dtDatiParametri, db).ToList();
                                        if (figli?.Any() ?? false)
                                        {
                                            using (dtPercentualeMagFigli dtpf = new dtPercentualeMagFigli())
                                            {
                                                using (
                                                    dtIndennitaPrimoSegretario dtps = new dtIndennitaPrimoSegretario())
                                                {
                                                    foreach (var f in figli)
                                                    {
                                                        PercentualeMagFigliModel pmfm =
                                                            dtpf.GetPercentualeMaggiorazioneFigli(f.idFigli,
                                                                dtDatiParametri,
                                                                db);
                                                        IndennitaPrimoSegretModel ipsm =
                                                            dtps.GetIndennitaPrimoSegretario(f.idFigli, dtDatiParametri,
                                                                db);

                                                        if (pmfm.percentualeFigli > 0)
                                                        {
                                                            MaggiorazioneFigli += ipsm.indennita * pmfm.percentualeFigli /
                                                                                  100;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    MaggiorazioneFamiliari = MaggiorazioneConiuge + MaggiorazioneFigli;
                                }
                            }
                        }

                        #endregion
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
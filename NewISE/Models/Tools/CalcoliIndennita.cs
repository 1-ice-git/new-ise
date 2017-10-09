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
        private List<MaggiorazioniFamiliariModel> ListamaggiorazioniFamiliari { get; set; }
        private ConiugeModel coniuge { get; set; }
        private PercentualeMagConiugeModel percentualeMaggiorazioneConiuge { get; set; }
        private PensioneConiugeModel pensioneConiuge { get; set; }
        private List<FigliModel> figli { get; set; }
        private IndennitaSistemazioneModel indennitaSistemazione { get; set; }

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
        public decimal maggiorazioneConiuge { get; set; } = 0;
        public decimal maggiorazioneFigli { get; set; } = 0;
        public decimal maggiorazioneFamiliari { get; set; } = 0;
        public decimal indennitaPersonaleTeorica { get; set; } = 0;
        public decimal indennitaSistemazioneLorda { get; set; } = 0;




        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public CalcoliIndennita(string matricola, DateTime? dataCalcoloIndennita = null)
        {
            DateTime? dt;
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    if (dataCalcoloIndennita.HasValue)
                    {
                        dt = dataCalcoloIndennita;
                    }
                    else
                    {
                        dt = DateTime.Now;
                    }

                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        trasferimento = dtt.GetUltimoTrasferimentoByMatricola(matricola, db);

                        if (trasferimento != null && trasferimento.HasValue())
                        {
                            if (trasferimento.dataRientro.HasValue)
                            {
                                if (trasferimento.dataRientro.Value < dt.Value)
                                {
                                    dtDatiParametri = trasferimento.dataRientro.Value;
                                }
                                else
                                {
                                    if (trasferimento.dataPartenza > dt.Value)
                                    {
                                        dtDatiParametri = trasferimento.dataPartenza;
                                    }
                                    else
                                    {
                                        dtDatiParametri = dt.Value;
                                    }
                                }
                            }
                            else
                            {
                                if (trasferimento.dataPartenza > dt.Value)
                                {
                                    dtDatiParametri = trasferimento.dataPartenza;
                                }
                                else
                                {
                                    dtDatiParametri = dt.Value;
                                }
                            }

                            using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                            {
                                RuoloUfficioModel rum = new RuoloUfficioModel();
                                ruoloUfficio =
                                    dtrd.GetRuoloDipendenteByIdTrasferimento(trasferimento.idTrasferimento,
                                        dtDatiParametri,
                                        db).RuoloUfficio;
                            }


                            using (dtIndennita dti = new dtIndennita())
                            {
                                IndennitaModel indennita =
                                    dti.GetIndennitaByIdTrasferimento(trasferimento.idTrasferimento,
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
                                        throw new Exception(
                                            "Il ruolo del dipendente non risulta registrato per l'utente " +
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
                                ListamaggiorazioniFamiliari =
                                    dtmf.GetListaMaggiorazioniFamiliariByIDTrasf(trasferimento.idTrasferimento, db)
                                        .ToList();

                                if (ListamaggiorazioniFamiliari?.Any() ?? false)
                                {
                                    foreach (var maggiorazioniFamiliari in ListamaggiorazioniFamiliari)
                                    {

                                        if (maggiorazioniFamiliari.attivazioneMaggiorazioni)
                                        {
                                            using (dtConiuge dtc = new dtConiuge())
                                            {
                                                coniuge =
                                                    dtc.GetConiugeByIdMagFam(
                                                        maggiorazioniFamiliari.idMaggiorazioniFamiliari,
                                                        dtDatiParametri, db);
                                                if (coniuge.HasValue())
                                                {
                                                    using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                                                    {
                                                        percentualeMaggiorazioneConiuge =
                                                            dtpc.GetPercentualeMaggiorazioneConiuge(
                                                                coniuge.idTipologiaConiuge,
                                                                dtDatiParametri, db);
                                                    }

                                                    using (dtPensione dtp = new dtPensione())
                                                    {
                                                        pensioneConiuge = dtp.GetPensioniByIdConiuge(coniuge.idConiuge,
                                                            dtDatiParametri, db);
                                                    }
                                                    if (percentualeMaggiorazioneConiuge != null &&
                                                        percentualeMaggiorazioneConiuge.percentualeConiuge > 0)
                                                    {
                                                        maggiorazioneConiuge += (indennitaServizio *
                                                                                percentualeMaggiorazioneConiuge
                                                                                    .percentualeConiuge / 100);
                                                    }
                                                    else
                                                    {
                                                        throw new Exception(
                                                            "La percentuale maggiorazione coniuge non è presente.");
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
                                                            dtIndennitaPrimoSegretario dtps =
                                                                new dtIndennitaPrimoSegretario())
                                                        {
                                                            foreach (var f in figli)
                                                            {
                                                                PercentualeMagFigliModel pmfm =
                                                                    dtpf.GetPercentualeMaggiorazioneFigli(f.idFigli,
                                                                        dtDatiParametri,
                                                                        db);
                                                                IndennitaPrimoSegretModel ipsm =
                                                                    dtps.GetIndennitaPrimoSegretario(f.idFigli,
                                                                        dtDatiParametri,
                                                                        db);

                                                                if (pmfm.percentualeFigli > 0)
                                                                {
                                                                    maggiorazioneFigli += ipsm.indennita *
                                                                                          pmfm.percentualeFigli /
                                                                                          100;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }


                                        }

                                    }
                                    maggiorazioneFamiliari = maggiorazioneConiuge + maggiorazioneFigli;
                                }


                            }

                            #endregion

                            #region IndennitaPersonale

                            indennitaPersonaleTeorica = indennitaServizio + maggiorazioneFamiliari;


                            #endregion

                            #region Prima sistemazione

                            using (dtPrimaSistemazione dtps = new dtPrimaSistemazione())
                            {
                                var psm = dtps.GetPrimaSistemazione(trasferimento.idTrasferimento, db);
                                using (dtIndennitaSistemazione dtis = new dtIndennitaSistemazione())
                                {
                                    var ism = dtis.GetIndennitaSistemazione(psm.idPrimaSistemazione,
                                        (EnumTipoTrasferimento)trasferimento.idTipoTrasferimento, dtDatiParametri, db);

                                }
                            }

                            #endregion
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

        public decimal IndennitaPersonalePeriodo(int periodo = 30)
        {
            decimal ipp = 0;

            try
            {
                if (indennitaPersonaleTeorica > 0)
                {
                    ipp = indennitaPersonaleTeorica / 30 * periodo;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return ipp;
        }
    }
}
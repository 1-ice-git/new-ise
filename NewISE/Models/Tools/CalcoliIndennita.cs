using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel.Enum;

namespace NewISE.Models.Tools
{
    public class CalcoliIndennita : IDisposable
    {
        //private TrasferimentoModel trasferimento { get; set; }
        private DateTime dtDatiParametri { get; set; }
        //private RuoloUfficioModel ruoloUfficio { get; set; }
        //private IndennitaModel indennita { get; set; }
        //private IndennitaBaseModel indennitaBase { get; set; }
        //private decimal indennitaBaseNoRiduzione { get; set; }
        //private CoefficientiSedeModel coefficenteSede { get; set; }
        //private PercentualeDisagioModel percentualeDisagio { get; set; }
        //private MaggiorazioniFamiliariModel maggiorazioniFamiliari { get; set; }
        //private ConiugeModel coniuge { get; set; }
        //private PercentualeMagConiugeModel percentualeMaggiorazioneConiuge { get; set; }
        //private PensioneConiugeModel pensioneConiuge { get; set; }
        //private List<FigliModel> figli { get; set; }
        //private IndennitaSistemazioneModel indennitaSistemazione { get; set; }

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

        public decimal maggiorazioneFamiliari
        {
            get { return maggiorazioneConiuge + maggiorazioneFigli; }
        }

        public decimal indennitaPersonaleTeorica { get; set; } = 0;
        public decimal indennitaSistemazioneLorda { get; set; } = 0;

        public decimal anticipoIndennitaSistemazioneLorda { get; set; } = 0;

        public decimal contributoOmnicomprensivoTrasferimentoAnticipo { get; set; } = 0;
        public decimal contributoOmnicomprensivoTrasferimentoSaldo { get; set; } = 0;
        public decimal percentualeFasciaKmTrasferimento { get; set; } = 0;
        public decimal contributoOmnicomprensivoRientroAnticipo { get; set; } = 0;
        public decimal contributoOmnicomprensivoRientroSaldo { get; set; } = 0;
        public decimal percentualeFasciaKmRientro { get; set; } = 0;


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public CalcoliIndennita(decimal idTrasferimento, DateTime? dataCalcoloIndennita = null)
        {
            DateTime? dt;
            RUOLODIPENDENTE ruoloDipendente = new RUOLODIPENDENTE();
            RUOLOUFFICIO ruoloUfficio = new RUOLOUFFICIO();
            RIDUZIONI riduzioniIndennitaBase = new RIDUZIONI();

            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    if (dataCalcoloIndennita.HasValue)
                    {
                        dt = dataCalcoloIndennita;
                    }
                    else
                    {
                        dt = DateTime.Now;
                    }

                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);

                    if (trasferimento.DATARIENTRO.HasValue)
                    {
                        if (trasferimento.DATARIENTRO.Value < dt.Value)
                        {
                            dtDatiParametri = trasferimento.DATARIENTRO.Value;
                        }
                        else
                        {
                            if (trasferimento.DATAPARTENZA > dt.Value)
                            {
                                dtDatiParametri = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtDatiParametri = dt.Value;
                            }
                        }
                    }
                    else
                    {
                        if (trasferimento.DATAPARTENZA > dt.Value)
                        {
                            dtDatiParametri = trasferimento.DATAPARTENZA;
                        }
                        else
                        {
                            dtDatiParametri = dt.Value;
                        }
                    }

                    var indennita = trasferimento.INDENNITA;

                    var lrd =
                        trasferimento.RUOLODIPENDENTE.Where(
                            a =>
                                a.ANNULLATO == false && dtDatiParametri >= a.DATAINZIOVALIDITA &&
                                dtDatiParametri <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINZIOVALIDITA);

                    if (lrd?.Any() ?? false)
                    {
                        ruoloDipendente = lrd.First();
                        ruoloUfficio = ruoloDipendente.RUOLOUFFICIO;

                        #region Indennita di base estera

                        RIDUZIONI riduzioniIB = new RIDUZIONI();

                        var lib =
                            indennita.INDENNITABASE.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    dtDatiParametri >= a.DATAINIZIOVALIDITA && dtDatiParametri <= a.DATAFINEVALIDITA)
                                .OrderByDescending(a => a.DATAINIZIOVALIDITA);

                        if (lib?.Any() ?? false)
                        {

                            var indennitaBase = lib.First();

                            var lr =
                                indennitaBase.RIDUZIONI.Where(
                                    a =>
                                        a.ANNULLATO == false && dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                        dtDatiParametri <= a.DATAFINEVALIDITA)
                                    .OrderByDescending(a => a.DATAINIZIOVALIDITA);

                            if (lr?.Any() ?? false)
                            {
                                riduzioniIB = lr.First();
                            }

                            if (ruoloUfficio.IDRUOLO == (decimal)EnumRuoloUfficio.Dirigente || ruoloUfficio.IDRUOLO == (decimal)EnumRuoloUfficio.Responsabile)
                            {
                                decimal valRespIB = indennitaBase.VALORERESP;
                                decimal valRidIB = 0;

                                if (riduzioniIB?.IDRIDUZIONI > 0)
                                {
                                    valRidIB = riduzioniIB.PERCENTUALE;
                                }
                                if (valRidIB > 0)
                                {
                                    indennitaBaseRiduzione = valRespIB * valRidIB / 100;
                                }
                                else
                                {
                                    indennitaBaseRiduzione = valRespIB;
                                }


                            }
                            else
                            {
                                decimal valIB = indennitaBase.VALORE;
                                decimal valRidIB = 0;

                                if (riduzioniIB?.IDRIDUZIONI > 0)
                                {
                                    valRidIB = riduzioniIB.PERCENTUALE;
                                }
                                if (valRidIB > 0)
                                {
                                    indennitaBaseRiduzione = valIB * valRidIB / 100;
                                }
                                else
                                {
                                    indennitaBaseRiduzione = valIB;
                                }

                            }

                        }


                        #endregion

                        if (indennitaBaseRiduzione > 0)
                        {
                            #region Indennità di servizio

                            var lcs =
                                indennita.COEFFICIENTESEDE.Where(
                                    a =>
                                        a.ANNULLATO == false && dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                        dtDatiParametri <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINIZIOVALIDITA);
                            if (lcs?.Any() ?? false)
                            {
                                var coefficenteSede = lcs.First();

                                var lpd =
                                    indennita.PERCENTUALEDISAGIO.Where(
                                        a =>
                                            a.ANNULLATO == false && dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                            dtDatiParametri <= a.DATAFINEVALIDITA)
                                        .OrderByDescending(a => a.DATAINIZIOVALIDITA);
                                if (lpd?.Any() ?? false)
                                {
                                    var percentualeDisagio = lpd.First();

                                    indennitaServizio = (((indennitaBaseRiduzione * coefficenteSede.VALORECOEFFICIENTE) +
                                                          indennitaBaseRiduzione) +
                                                         (((indennitaBaseRiduzione * coefficenteSede.VALORECOEFFICIENTE) +
                                                           indennitaBaseRiduzione) / 100 * percentualeDisagio.PERCENTUALE));


                                }

                            }

                            #endregion

                            if (indennitaServizio > 0)
                            {
                                #region Maggiorazioni familiari


                                decimal valoreMF = 0;

                                var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                                var lattivazioneMF =
                                    mf.ATTIVAZIONIMAGFAM.Where(
                                        a =>
                                            a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true &&
                                            a.ATTIVAZIONEMAGFAM == true)
                                        .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();


                                if (lattivazioneMF?.Any() ?? false)
                                {
                                    var lc =
                                        mf.CONIUGE.Where(
                                            a =>
                                                a.IDSTATORECORD==(decimal)EnumStatoRecord.Attivato && dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                                dtDatiParametri <= a.DATAFINEVALIDITA)
                                            .OrderByDescending(a => a.DATAINIZIOVALIDITA);

                                    if (lc?.Any() ?? false)
                                    {
                                        var coniuge = lc.First();

                                        var lpmc =
                                            coniuge.PERCENTUALEMAGCONIUGE.Where(
                                                a =>
                                                    a.ANNULLATO == false &&
                                                    a.IDTIPOLOGIACONIUGE == coniuge.IDTIPOLOGIACONIUGE &&
                                                    dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                                    dtDatiParametri <= a.DATAFINEVALIDITA)
                                                .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                        if (lpmc?.Any() ?? false)
                                        {
                                            var percentualeMaggiorazioneConiuge = lpmc.First();

                                            maggiorazioneConiuge = indennitaServizio *
                                                                   percentualeMaggiorazioneConiuge.PERCENTUALECONIUGE /
                                                                   100;
                                        }

                                        var lpensioni =
                                            coniuge.PENSIONE.Where(
                                                a =>
                                                    a.ANNULLATO == false && dtDatiParametri >= a.DATAINIZIO &&
                                                    dtDatiParametri <= a.DATAFINE)
                                                .OrderByDescending(a => a.DATAINIZIO)
                                                .ToList();

                                        if (lpensioni?.Any() ?? false)
                                        {
                                            var pens = lpensioni.First();

                                            if (pens.IMPORTOPENSIONE >= maggiorazioneConiuge)
                                            {
                                                maggiorazioneConiuge = 0;
                                            }
                                            else
                                            {
                                                maggiorazioneConiuge = maggiorazioneConiuge - pens.IMPORTOPENSIONE;
                                            }

                                        }


                                    }

                                    #region Maggiorazioni figli

                                    var lf =
                                        mf.FIGLI.Where(
                                            a =>
                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato && dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                                dtDatiParametri <= a.DATAFINEVALIDITA)
                                            .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                    if (lf?.Any() ?? false)
                                    {
                                        foreach (var f in lf)
                                        {
                                            var lpmf =
                                                f.PERCENTUALEMAGFIGLI.Where(
                                                    a =>
                                                        a.ANNULLATO == false && dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                                        dtDatiParametri <= a.DATAFINEVALIDITA)
                                                    .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                            if (lpmf?.Any() ?? false)
                                            {
                                                var pmf = lpmf.First();

                                                var lips =
                                                    f.INDENNITAPRIMOSEGRETARIO.Where(
                                                        a =>
                                                            a.ANNULLATO == false &&
                                                            dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                                            dtDatiParametri <= a.DATAFINEVALIDITA)
                                                        .OrderByDescending(a => a.DATAINIZIOVALIDITA);
                                                if (lips?.Any() ?? false)
                                                {
                                                    var ips = lips.First();
                                                    decimal indennitaPrimoSegretario = ips.INDENNITA;
                                                    decimal indennitaServizioPS = 0;

                                                    var lcsd =
                                                        indennita.COEFFICIENTESEDE.Where(
                                                            a =>
                                                                a.ANNULLATO == false &&
                                                                dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                                                dtDatiParametri <= a.DATAFINEVALIDITA)
                                                            .OrderByDescending(a => a.DATAINIZIOVALIDITA);
                                                    if (lcsd?.Any() ?? false)
                                                    {
                                                        var coefficenteSede = lcsd.First();

                                                        var lpd =
                                                            indennita.PERCENTUALEDISAGIO.Where(
                                                                a =>
                                                                    a.ANNULLATO == false && dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                                                    dtDatiParametri <= a.DATAFINEVALIDITA)
                                                                .OrderByDescending(a => a.DATAINIZIOVALIDITA);
                                                        if (lpd?.Any() ?? false)
                                                        {
                                                            var percentualeDisagio = lpd.First();

                                                            indennitaServizioPS = (((indennitaPrimoSegretario * coefficenteSede.VALORECOEFFICIENTE) +
                                                                                  indennitaPrimoSegretario) +
                                                                                 (((indennitaPrimoSegretario * coefficenteSede.VALORECOEFFICIENTE) +
                                                                                   indennitaPrimoSegretario) / 100 * percentualeDisagio.PERCENTUALE));

                                                            valoreMF += indennitaServizioPS * pmf.PERCENTUALEFIGLI / 100;


                                                        }

                                                    }

                                                }



                                            }



                                        }

                                        maggiorazioneFigli = valoreMF;
                                    }
                                    #endregion

                                }





                                #endregion

                                #region Indennità personale

                                indennitaPersonaleTeorica = indennitaServizio + maggiorazioneFamiliari;

                                #endregion

                                #region Indennità di sistemazione lorda

                                var primaSistemazione = trasferimento.PRIMASITEMAZIONE;

                                var lis = primaSistemazione.INDENNITASISTEMAZIONE;

                                var lisCond = lis.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.IDTIPOTRASFERIMENTO == trasferimento.IDTIPOTRASFERIMENTO &&
                                        dtDatiParametri >= a.DATAINIZIOVALIDITA &&
                                        dtDatiParametri <= a.DATAFINEVALIDITA)
                                    .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                if (lisCond?.Any() ?? false)
                                {
                                    var indSist = lisCond.First();

                                    //switch ((EnumTipoTrasferimento)trasferimento.IDTIPOTRASFERIMENTO)
                                    //{
                                    //    case EnumTipoTrasferimento.SedeEstero:

                                    //        break;
                                    //    case EnumTipoTrasferimento.EsteroEstero:
                                    //        break;
                                    //    case EnumTipoTrasferimento.EsteroEsteroStessaRegiona:
                                    //        break;
                                    //    default:
                                    //        throw new ArgumentOutOfRangeException();
                                    //}


                                    indennitaSistemazioneLorda = indSist.COEFFICIENTE * indennitaPersonaleTeorica;

                                    #region Anticipo Indennità Sistemazione Lorda

                                    anticipoIndennitaSistemazioneLorda = indSist.COEFFICIENTE * indennitaServizio;

                                    #endregion
                                }


                                #endregion




                            }

                            #region Contributo Omnicomprensivo

                            if (indennitaSistemazioneLorda > 0)
                            {
                                var primaSistemazione = trasferimento.PRIMASITEMAZIONE;

                                var lpercentualeFKM =
                                    primaSistemazione.PERCENTUALEFKM.Where(
                                        a =>
                                            a.ANNULLATO == false && trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                            trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                        .OrderByDescending(a => a.DATAINIZIOVALIDITA);

                                if (lpercentualeFKM?.Any() ?? false)
                                {
                                    var pfkm = lpercentualeFKM.First();
                                    percentualeFasciaKmTrasferimento = pfkm.COEFFICIENTEKM;
                                    contributoOmnicomprensivoTrasferimentoAnticipo = indennitaSistemazioneLorda * (percentualeFasciaKmTrasferimento / 100);
                                    contributoOmnicomprensivoTrasferimentoSaldo = indennitaSistemazioneLorda *
                                                                                  ((100 -
                                                                                    percentualeFasciaKmTrasferimento) /
                                                                                   100);

                                }


                            }
                            #endregion
                        }

                    }




                }
                catch (Exception ex)
                {

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
using NewISE.Areas.Statistiche.Models;
using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.Models.Enumeratori;
using NewISE.Models.Tools;
using NewISE.Models.DBModel.dtObj;

namespace NewISE.Areas.Statistiche.Models.dtObj
{
    public class dtRiepilogoGenerale : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public List<RptRiepilogoGeneraleModel> GetRiepilogoGenerale(decimal MeseDa, decimal AnnoDa, decimal MeseA, decimal AnnoA, ModelDBISE db)
        {
            try
            {
                List<RptRiepilogoGeneraleModel> lrptrgm = new List<RptRiepilogoGeneraleModel>();

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    using (dtStatistiche dts = new dtStatistiche())
                    {

                        string strMeseDa = MeseDa.ToString().PadLeft(2, Convert.ToChar("0"));
                        string strMeseA = MeseA.ToString().PadLeft(2, Convert.ToChar("0"));

                        DateTime dtIni = Convert.ToDateTime("01/" + strMeseDa + "/" + AnnoDa.ToString());
                        DateTime dtFin = Utility.GetDtFineMese(Convert.ToDateTime("01/" + strMeseA + "/" + AnnoA.ToString()));

                        var annoMeseInizio = Convert.ToDecimal(AnnoDa.ToString() + MeseDa.ToString().PadLeft(2, (char)'0'));
                        var annoMeseFine = Convert.ToDecimal(AnnoA.ToString() + MeseA.ToString().PadLeft(2, (char)'0'));

                        var ltrasf = db.TRASFERIMENTO
                                        .Where(a => a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                                    a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Da_Attivare &&
                                                    a.DATARIENTRO >= dtIni &&
                                                    a.DATAPARTENZA <= dtFin &&
                                                    a.INDENNITA.LIVELLIDIPENDENTI
                                                                        .Any(b =>
                                                                                b.ANNULLATO == false &&
                                                                                b.DATAFINEVALIDITA >= dtIni &&
                                                                                b.DATAINIZIOVALIDITA <= dtFin))
                                        .ToList();

                        var ndip = ltrasf.GroupBy(a => a.IDDIPENDENTE).Count();
                        var nuff = ltrasf.GroupBy(a => a.IDUFFICIO).Count();

                        #region ALTRE SPESE

                        #endregion

                        #region INDENNITA
                        var idVoci = (decimal)EnumVociContabili.Ind_Sede_Estera;
                        var lteorici = db.TEORICI.Where(a =>
                                                            a.ELABINDENNITA.Any(b => b.ANNULLATO == false) &&
                                                            a.ANNULLATO == false &&
                                                            a.DIRETTO == false &&
                                                            a.ELABORATO == true &&
                                                            a.INSERIMENTOMANUALE == false &&
                                                            a.IDVOCI == idVoci &&
                                                            a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità)
                                                .ToList();
                                                              
                        var lteorici2 = lteorici.Where(a =>
                                                            Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                                    a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) >= annoMeseInizio &&
                                                            Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                                    a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) <= annoMeseFine
                                                    );

                        decimal importo_indennita = lteorici2.Sum(a => a.IMPORTO);
                        #endregion

                        #region MAB
                        idVoci = (decimal)EnumVociContabili.MAB;
                        lteorici = db.TEORICI.Where(a =>
                                                a.ELABMAB.Any(b => b.ANNULLATO == false) &&
                                                a.ANNULLATO == false &&
                                                a.DIRETTO == false &&
                                                a.ELABORATO == true &&
                                                a.INSERIMENTOMANUALE == false &&
                                                a.IDVOCI == idVoci &&
                                                a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità)
                                            .ToList();

                        lteorici2 = lteorici.Where(a =>
                                        Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                            a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) >= annoMeseInizio &&
                                        Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                            a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) <= annoMeseFine);

                        decimal importo_mab = lteorici2.Sum(a => a.IMPORTO);
                        #endregion

                        #region PRIMA SISTEMAZIONE
                        idVoci = (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380;
                        lteorici = db.TEORICI.Where(a =>
                                                a.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                                                a.ANNULLATO == false &&
                                                a.ELABORATO == true &&
                                                a.INSERIMENTOMANUALE == false &&
                                                a.IDVOCI == idVoci &&
                                                a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe)
                                            .ToList();

                        lteorici2 = lteorici.Where(a =>
                                        Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                            a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) >= annoMeseInizio &&
                                        Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                            a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) <= annoMeseFine);

                        decimal importo_primasistemazione = lteorici2.Sum(a => a.IMPORTO);
                        #endregion

                        #region RICHIAMO
                        idVoci = (decimal)EnumVociContabili.Ind_Richiamo_IRI;
                        lteorici = db.TEORICI.Where(a =>
                                                    a.ELABINDRICHIAMO.ANNULLATO == false &&
                                                    a.ANNULLATO == false &&
                                                    a.DIRETTO == false &&
                                                    a.ELABORATO == true &&
                                                    a.INSERIMENTOMANUALE == false &&
                                                    a.IDVOCI == idVoci &&
                                                    a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità)
                                                .ToList();

                        lteorici2 = lteorici.Where(a =>
                                                    Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                        a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) >= annoMeseInizio &&
                                                    Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                        a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) <= annoMeseFine);

                        decimal importo_richiamo = lteorici2.Sum(a => a.IMPORTO);
                        #endregion

                        #region TRASPORTO EFFETTI
                        idVoci = (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131;
                        lteorici = db.TEORICI.Where(a =>
                                                        a.ELABTRASPEFFETTI.ANNULLATO == false &&
                                                        a.ANNULLATO == false &&
                                                        a.DIRETTO == false &&
                                                        a.ELABORATO == true &&
                                                        a.INSERIMENTOMANUALE == false &&
                                                        a.IDVOCI == idVoci &&
                                                        a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe)
                                                .ToList();

                        lteorici2 = lteorici.Where(a =>
                                                    Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                        a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) >= annoMeseInizio &&
                                                    Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                        a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) <= annoMeseFine);

                        decimal importo_TE = lteorici2.Sum(a => a.IMPORTO);
                        #endregion

                        RptRiepilogoGeneraleModel rptrgm = new RptRiepilogoGeneraleModel()
                        {
                            numUffici = nuff,
                            numDipendenti = ndip,
                            IndPersonale=importo_indennita,
                            IndRichiamo=importo_richiamo,
                            IndMAB=importo_mab,
                            IndTE=importo_TE,
                            IndPS=importo_primasistemazione
                        };
                        lrptrgm.Add(rptrgm);
                    }
                }
                return lrptrgm;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
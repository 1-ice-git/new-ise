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
    public class dtRiepilogoUfficio : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public List<RptRiepilogoUfficioModel> GetRiepilogoUfficio(decimal MeseDa, decimal AnnoDa, decimal MeseA, decimal AnnoA, ModelDBISE db)
        {

            

            try
            {
                List<RptRiepilogoUfficioModel> lrptrum = new List<RptRiepilogoUfficioModel>();

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

                        var lu = db.UFFICI.ToList();

                        foreach (var u in lu)
                        {

                            var datiufficio = u.TRASFERIMENTO.Where(a => 
                                                                a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                                                a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Da_Attivare &&
                                                                a.DATAPARTENZA <= dtFin &&
                                                                a.DATARIENTRO >= dtIni &&
                                                                a.IDUFFICIO == u.IDUFFICIO)
                                                            .ToList();

                            if(datiufficio?.Any()??false)
                            {
                                var ltrasf = db.TRASFERIMENTO
                                                .Where(a => a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                                            a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Da_Attivare &&
                                                            a.DATARIENTRO >= dtIni &&
                                                            a.DATAPARTENZA <= dtFin &&
                                                            a.IDUFFICIO == u.IDUFFICIO)
                                                .ToList();

                                var ndip = ltrasf.GroupBy(a => a.IDDIPENDENTE).Count();

                                #region ALTRE SPESE

                                #endregion

                                #region INDENNITA
                                var idVoci = (decimal)EnumVociContabili.Ind_Sede_Estera;
                                var lteorici = db.TEORICI.Where(a =>
                                                                    a.ELABINDENNITA.Any(b => b.ANNULLATO==false && b.INDENNITA.TRASFERIMENTO.IDUFFICIO==u.IDUFFICIO) &&
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
                                                        a.ELABMAB.Any(b => b.ANNULLATO == false && b.INDENNITA.TRASFERIMENTO.IDUFFICIO == u.IDUFFICIO) &&
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
                                                        a.TRASFERIMENTO.IDUFFICIO== u.IDUFFICIO &&
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
                                                            a.TRASFERIMENTO.IDUFFICIO == u.IDUFFICIO &&
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
                                                                a.TRASFERIMENTO.IDUFFICIO == u.IDUFFICIO &&
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

                                RptRiepilogoUfficioModel rptrum = new RptRiepilogoUfficioModel()
                                {
                                    codice=u.CODICEUFFICIO,
                                    descUfficio=u.DESCRIZIONEUFFICIO,
                                    numDipendenti = ndip,
                                    IndPersonale=importo_indennita,
                                    IndRichiamo=importo_richiamo,
                                    IndMAB=importo_mab,
                                    IndTE=importo_TE,
                                    IndPS=importo_primasistemazione
                                };
                                lrptrum.Add(rptrum);
                            }
                        }
                    }
                }
                return lrptrum;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
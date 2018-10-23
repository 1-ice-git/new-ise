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
    public class dtRiepilogoLivello : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public List<RptRiepilogoLivelloModel> GetRiepilogoLivello(decimal MeseDa, decimal AnnoDa, decimal MeseA, decimal AnnoA, ModelDBISE db)
        {

            

            try
            {
                List<RptRiepilogoLivelloModel> lrptrlm = new List<RptRiepilogoLivelloModel>();

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    using (dtStatistiche dts = new dtStatistiche())
                    {
                        //List<TRASFERIMENTO> lt = new List<TRASFERIMENTO>();
                        //List<TEORICI> lteorici = new List<TEORICI>();

                        string strMeseDa = MeseDa.ToString().PadLeft(2, Convert.ToChar("0"));
                        string strMeseA = MeseA.ToString().PadLeft(2, Convert.ToChar("0"));

                        DateTime dtIni = Convert.ToDateTime("01/" + strMeseDa + "/" + AnnoDa.ToString());
                        DateTime dtFin = Utility.GetDtFineMese(Convert.ToDateTime("01/" + strMeseA + "/" + AnnoA.ToString()));

                        var annoMeseInizio = Convert.ToDecimal(AnnoDa.ToString() + MeseDa.ToString().PadLeft(2, (char)'0'));
                        var annoMeseFine = Convert.ToDecimal(AnnoA.ToString() + MeseA.ToString().PadLeft(2, (char)'0'));


                        #region COMMENTO
                        //#region Test query
                        //var lTrasf = db.TRASFERIMENTO.Where(a => (a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato && a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Da_Attivare) &&
                        //                                                       a.DATARIENTRO >= dtIni &&
                        //                                                       a.DATAPARTENZA <= dtFin).ToList();

                        //foreach (var t in lTrasf)
                        //{
                        //    //var lLiv = db.LIVELLI.Where(a => a.LIVELLIDIPENDENTI.Any(b => b.IDDIPENDENTE == t.IDDIPENDENTE && 
                        //    //                                                         b.ANNULLATO == false && 
                        //    //                                                         b.DATAFINEVALIDITA >= dtIni && 
                        //    //                                                         b.DATAINIZIOVALIDITA <= dtFin)).ToList();

                        //    var livdip = t.INDENNITA.LIVELLIDIPENDENTI.Where(a => a.ANNULLATO == false &&
                        //                                                             a.DATAFINEVALIDITA >= dtIni &&
                        //                                                             a.DATAINIZIOVALIDITA <= dtFin).ToList();

                        //    foreach (var ld in livdip)
                        //    {
                        //        var livello = ld.LIVELLI;
                        //        var desc = livello.LIVELLO;

                        //    }


                        //    //var nUff = db.UFFICI.Where(a => a.TRASFERIMENTO.Any(b => (b.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                        //    //                                                    b.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Da_Attivare) && 
                        //    //                                                    b.INDENNITA.LIVELLIDIPENDENTI.Any(c => c.ANNULLATO == false && 
                        //    //                                                                                      c.DATAFINEVALIDITA >= dtIni && 
                        //    //                                                                                      c.DATAINIZIOVALIDITA <= dtFin))).Count();


                        //}

                        //#endregion




                        //conta uffici
                        //#region Elenco Trasferimenti nel range
                        //var ltrasf = db.TRASFERIMENTO.Where(a =>
                        //                        a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                        //                        a.DATARIENTRO >= dtIni && a.DATAPARTENZA <= dtFin)
                        //                    //.GroupBy(a=>a.IDUFFICIO)
                        //                    .ToList();
                        //#endregion
                        //var sommaUff = 0;
                        #endregion


                        //#region elenco livelli
                        ////var llivdip = db.LIVELLIDIPENDENTI.Where(a => a.ANNULLATO == false &&
                        ////                                        a.DATAFINEVALIDITA >= dtIni &&
                        ////                                        a.DATAINIZIOVALIDITA <= dtFin)
                        ////                    .ToList();
                        //#endregion
                        ////var ll = db.LIVELLI.ToList();

                        var ll = db.LIVELLI.ToList();

                        foreach (var l in ll)
                        {

                            var datilivello = l.LIVELLIDIPENDENTI.Where(a => a.ANNULLATO == false &&
                                                                  a.DATAFINEVALIDITA >= dtIni &&
                                                                  a.DATAINIZIOVALIDITA <= dtFin &&
                                                                  a.IDLIVELLO == l.IDLIVELLO &&
                                                                  a.DIPENDENTI.TRASFERIMENTO
                                                                                  .Any(b =>
                                                                                          b.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                                                                          b.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Da_Attivare &&
                                                                                          b.DATARIENTRO >= dtIni &&
                                                                                          b.DATAPARTENZA <= dtFin)).ToList();

                            //var lTrasf2 = db.TRASFERIMENTO.Where(a => a.INDENNITA.LIVELLIDIPENDENTI.Any(b => b.ANNULLATO == false &&
                            //                                                                            b.DATAFINEVALIDITA >= dtIni &&
                            //                                                                            b.DATAINIZIOVALIDITA <= dtFin &&
                            //                                                                            b.IDLIVELLO == l.IDLIVELLO)).ToList();
                            //int nUff = lTrasf2.Count();



                            if(datilivello?.Any()??false)
                            {

                                var ltrasf = db.TRASFERIMENTO
                                                .Where(a => a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                                            a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Da_Attivare &&
                                                            a.DATARIENTRO >= dtIni &&
                                                            a.DATAPARTENZA <= dtFin &&
                                                            a.INDENNITA.LIVELLIDIPENDENTI
                                                                                .Any(b =>
                                                                                        b.ANNULLATO == false &&
                                                                                        b.DATAFINEVALIDITA >= dtIni &&
                                                                                        b.DATAINIZIOVALIDITA <= dtFin &&
                                                                                        b.IDLIVELLO == l.IDLIVELLO)).ToList();

                                var ndip = ltrasf.GroupBy(a => a.IDDIPENDENTE).Count();
                                var nuff = ltrasf.GroupBy(a => a.IDUFFICIO).Count();

                                #region ALTRE SPESE

                                #endregion

                                #region INDENNITA
                                var idVoci = (decimal)EnumVociContabili.Ind_Sede_Estera;
                                var lteorici = db.TEORICI.Where(a =>
                                                                    a.ELABINDENNITA.Any(b => b.IDLIVELLO == l.IDLIVELLO && b.ANNULLATO==false) &&
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
                                                        a.ELABMAB.Any(b => b.IDLIVELLO == l.IDLIVELLO && b.ANNULLATO == false) &&
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
                                                        a.ELABINDSISTEMAZIONE.IDLIVELLO == l.IDLIVELLO &&
                                                        a.ELABINDSISTEMAZIONE.ANNULLATO==false &&
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
                                                            a.ELABINDRICHIAMO.IDLIVELLO == l.IDLIVELLO &&
                                                            a.ELABINDRICHIAMO.ANNULLATO==false &&
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
                                                                a.ELABTRASPEFFETTI.IDLIVELLO == l.IDLIVELLO &&
                                                                a.ELABTRASPEFFETTI.ANNULLATO==false &&
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

                                RptRiepilogoLivelloModel rptrlm = new RptRiepilogoLivelloModel()
                                {
                                    Livello = l.LIVELLO,
                                    descLivello = (l.LIVELLO == "D") ? "DIRIGENTE" : l.LIVELLO,
                                    numDipendenti = ndip,
                                    numUffici = nuff,
                                    IndPersonale=importo_indennita,
                                    IndRichiamo=importo_richiamo,
                                    IndMAB=importo_mab,
                                    IndTE=importo_TE,
                                    IndPS=importo_primasistemazione
                                };
                                lrptrlm.Add(rptrlm);
                            }
                        }
                    }
                }
                return lrptrlm;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
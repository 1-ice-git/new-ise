using NewISE.Areas.Statistiche.Models;
using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.Models.Enumeratori;
using NewISE.Models.Tools;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtStatistiche : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public string GetDescrizioneCoan(EnumTipologiaCoan tipoCoan, ModelDBISE db)
        {
            string ret = ""; ;
            var tc = db.TIPOLOGIACOAN.Find((decimal)tipoCoan);
            if (tc.IDTIPOCOAN > 0)
            {
                ret = tc.DESCRIZIONE;
            }

            return ret;
        }

        public string GetDescrizioneVoci(decimal idVoci, ModelDBISE db)
        {
            string ret = ""; ;
            var v = db.VOCI.Find(idVoci);
            if (v.IDVOCI > 0)
            {
                ret = v.DESCRIZIONE;
            }

            return ret;
        }

        public List<TEORICI> GetIndennitaSedeEstera(
                        TRASFERIMENTO t,
                        decimal idVoci,
                        decimal idTipoLiquidazione,
                        decimal idLivello,
                        decimal annoMeseInizio,
                        decimal annoMeseFine,
                        ModelDBISE db)
        {
            List<TEORICI> lteorici = t.TEORICI.Where(a =>
                                                a.ELABINDENNITA.Any(b => b.IDLIVELLO == idLivello) &&
                                                a.ANNULLATO == false &&
                                                a.DIRETTO == false &&
                                                a.ELABORATO == true &&
                                                a.INSERIMENTOMANUALE == false &&
                                                a.IDVOCI == idVoci &&
                                                a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                                Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                                    a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) >= annoMeseInizio &&
                                                Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                                    a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) <= annoMeseFine)
                                    .OrderBy(a => a.ANNORIFERIMENTO)
                                    .ThenBy(a => a.MESERIFERIMENTO).ToList();

            return lteorici;
        }

        public List<TEORICI> GetIndennitaMAB(
                       TRASFERIMENTO t,
                       decimal idLivello,
                       decimal idVoci,
                       decimal idTipoLiquidazione,
                       decimal annoMeseInizio,
                       decimal annoMeseFine,
                       ModelDBISE db)
        {
            List<TEORICI> lteorici = t.TEORICI.Where(a =>
                                                        a.ELABMAB.Any(b => b.IDLIVELLO == idLivello) &&
                                                        a.ANNULLATO == false &&
                                                        a.DIRETTO == false &&
                                                        a.ELABORATO == true &&
                                                        a.INSERIMENTOMANUALE == false &&
                                                        a.IDVOCI == idVoci &&
                                                        a.VOCI.IDTIPOLIQUIDAZIONE == idTipoLiquidazione &&
                                                        Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                                            a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) >= annoMeseInizio &&
                                                        Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                                                        a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) <= annoMeseFine)
                                                .OrderBy(a => a.ANNORIFERIMENTO)
                                                .ThenBy(a => a.MESERIFERIMENTO).ToList();

            return lteorici;
        }

        public List<TEORICI> GetIndennitaPS(
                      TRASFERIMENTO t,
                      decimal idLivello,
                      decimal idVoci,
                      decimal idTipoLiquidazione,
                      decimal annoMeseInizio,
                      decimal annoMeseFine,
                      ModelDBISE db)
        {
            List<TEORICI> lteorici = t.TEORICI.Where(a =>
                                                            a.ELABINDSISTEMAZIONE?.IDLIVELLO == idLivello &&
                                                            a.ANNULLATO == false &&
                                                            //a.DIRETTO == false &&
                                                            a.ELABORATO == true &&
                                                            a.INSERIMENTOMANUALE == false &&
                                                            a.IDVOCI == idVoci &&
                                                            a.VOCI.IDTIPOLIQUIDAZIONE == idTipoLiquidazione &&
                                                            Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                                                a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) >= annoMeseInizio &&
                                                            Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                                                a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) <= annoMeseFine)
                                                        .OrderBy(a => a.ANNORIFERIMENTO)
                                                        .ThenBy(a => a.MESERIFERIMENTO).ToList();
            return lteorici;
        }


        public List<TEORICI> GetIndennitaRichiamo(
                     TRASFERIMENTO t,
                     decimal idLivello,
                     decimal idVoci,
                     decimal idTipoLiquidazione,
                     decimal annoMeseInizio,
                     decimal annoMeseFine,
                     ModelDBISE db)
        {
            List<TEORICI> lteorici = t.TEORICI.Where(a =>
                                                                    a.ELABINDRICHIAMO?.IDLIVELLO == idLivello &&
                                                                    a.ANNULLATO == false &&
                                                                    a.DIRETTO == false &&
                                                                    a.ELABORATO == true &&
                                                                    a.INSERIMENTOMANUALE == false &&
                                                                    a.IDVOCI == idVoci &&
                                                                    a.VOCI.IDTIPOLIQUIDAZIONE == idTipoLiquidazione &&
                                                                    Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                                                        a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) >= annoMeseInizio &&
                                                                    Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                                                        a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) <= annoMeseFine)
                                                        .OrderBy(a => a.ANNORIFERIMENTO)
                                                        .ThenBy(a => a.MESERIFERIMENTO).ToList();
            return lteorici;
        }

        public List<TEORICI> GetIndennitaTE(
                 TRASFERIMENTO t,
                 decimal idLivello,
                 decimal idVoci,
                 decimal idTipoLiquidazione,
                 decimal annoMeseInizio,
                 decimal annoMeseFine,
                 ModelDBISE db)
        {
            List<TEORICI> lteorici = t.TEORICI.Where(a =>
                                                        a.ELABTRASPEFFETTI?.IDLIVELLO == idLivello &&
                                                        a.ANNULLATO == false &&
                                                        a.DIRETTO == false &&
                                                        a.ELABORATO == true &&
                                                        a.INSERIMENTOMANUALE == false &&
                                                        a.IDVOCI == idVoci &&
                                                        a.VOCI.IDTIPOLIQUIDAZIONE == idTipoLiquidazione &&
                                                        Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                                            a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) >= annoMeseInizio &&
                                                        Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                                            a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) <= annoMeseFine)
                                                .OrderBy(a => a.ANNORIFERIMENTO)
                                                .ThenBy(a => a.MESERIFERIMENTO).ToList();

            return lteorici;
        }

        public List<TEORICI> GetIndennitaTEP(
         TRASFERIMENTO t,
         decimal idLivello,
         decimal idVoci,
         decimal idTipoLiquidazione,
         decimal annoMeseInizio,
         decimal annoMeseFine,
         ModelDBISE db)
        {
            List<TEORICI> lteorici = t.TEORICI.Where(a =>
                                                        a.ELABTRASPEFFETTI?.IDLIVELLO == idLivello &&
                                                        //a.ELABTRASPEFFETTI.IDTEPARTENZA>0 &&
                                                        a.ANNULLATO == false &&
                                                        a.DIRETTO == false &&
                                                        a.ELABORATO == true &&
                                                        a.INSERIMENTOMANUALE == false &&
                                                        a.IDVOCI == idVoci &&
                                                        a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
                                                        Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                                            a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) >= annoMeseInizio &&
                                                        Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                                            a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) <= annoMeseFine)
                                                .OrderBy(a => a.ANNORIFERIMENTO)
                                                .ThenBy(a => a.MESERIFERIMENTO).ToList();

            

            return lteorici;
        }

    }
}
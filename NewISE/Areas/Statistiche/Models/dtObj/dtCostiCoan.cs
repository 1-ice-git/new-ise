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
    public class dtCostiCoan : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public IList<ElencoCoanModel> GetElencoCoan(ModelDBISE db)
        {
            List<ElencoCoanModel> lecm = new List<ElencoCoanModel>();
            ElencoCoanModel ecm = new ElencoCoanModel();

            #region servizi istituzionali
            var tc = db.TIPOLOGIACOAN.Find((decimal)EnumTipologiaCoan.Servizi_Istituzionali);

            ecm = new ElencoCoanModel()
            {
                idElencoCoan = "00",
                codiceCoan = tc.DESCRIZIONE
            };

            lecm.Add(ecm);
            #endregion

            #region servizi promozionali
            var lcoan = db.TRASFERIMENTO
                        .Where(a => a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                    a.IDTIPOCOAN == (decimal)EnumTipologiaCoan.Servizi_Promozionali)
                        .OrderBy(a => a.COAN)
                        .GroupBy(a => a.COAN)
                        .ToList();

            if (lcoan?.Any() ?? false)
            {
                foreach (var coan in lcoan)
                {
                    foreach (var item in coan)
                    {
                        ecm = new ElencoCoanModel()
                        {
                            idElencoCoan = item.COAN,
                            codiceCoan = item.COAN
                        };
                        lecm.Add(ecm);
                    }
                }
            }
            #endregion

            return lecm;
        }

        public List<RptCostiCoanModel> GetCostiCoan(decimal MeseDa, decimal AnnoDa, decimal MeseA, decimal AnnoA, string codiceCoan, ModelDBISE db)
        {
            try
            {
                List<RptCostiCoanModel> lrptccm = new List<RptCostiCoanModel>();

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    using (dtStatistiche dts = new dtStatistiche())
                    {
                        List<TRASFERIMENTO> lt = new List<TRASFERIMENTO>();
                        List<TEORICI> lteorici = new List<TEORICI>();

                        string strMeseDa = MeseDa.ToString().PadLeft(2, Convert.ToChar("0"));
                        string strMeseA = MeseA.ToString().PadLeft(2, Convert.ToChar("0"));

                        DateTime dtIni = Convert.ToDateTime("01/" + strMeseDa + "/" + AnnoDa.ToString());
                        DateTime dtFin = Utility.GetDtFineMese(Convert.ToDateTime("01/" + strMeseA + "/" + AnnoA.ToString()));

                        string strCoan = codiceCoan;

                        #region Elenco Trasferimenti nel range in base al coan
                        if (codiceCoan.Length < 10)
                        {
                            lt = db.TRASFERIMENTO.Where(a =>
                                                            a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                                            a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Da_Attivare &&
                                                            a.DATARIENTRO >= dtIni && a.DATAPARTENZA <= dtFin &&
                                                            a.IDTIPOCOAN == (decimal)EnumTipologiaCoan.Servizi_Istituzionali)
                                                        .ToList();
                        }
                        else
                        {
                            lt = db.TRASFERIMENTO.Where(a =>
                                        a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                                            a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Da_Attivare &&
                                        a.DATARIENTRO >= dtIni && a.DATAPARTENZA <= dtFin &&
                                        a.COAN == strCoan &&
                                        a.IDTIPOCOAN == (decimal)EnumTipologiaCoan.Servizi_Promozionali)
                                    .ToList();

                        }
                        #endregion


                        #region ciclo trasferimenti    
                        foreach (var t in lt)
                        {
                            var d = t.DIPENDENTI;
                            var nome = d.NOME;
                            var cognome = d.COGNOME;
                            var matricola = d.MATRICOLA;
                            var ufficio = t.UFFICI.DESCRIZIONEUFFICIO;
                            decimal idVoci = 0;
                            if (matricola == 3367)
                            {
                                var a = 0;
                            }

                            #region elenco livelli x trasferimento
                            var llivdip = t.INDENNITA.LIVELLIDIPENDENTI.Where(a => a.ANNULLATO == false &&
                                                                                    a.DATAFINEVALIDITA >= dtIni &&
                                                                                    a.DATAINIZIOVALIDITA <= dtFin
                                                                            )
                                                .ToList();
                            #endregion

                            #region ciclo livelli
                            foreach (var livdip in llivdip)
                            {
                                var liv = livdip.LIVELLI;

                                var annoMeseInizio = Convert.ToDecimal(AnnoDa.ToString() + MeseDa.ToString().PadLeft(2, (char)'0'));
                                var annoMeseFine = Convert.ToDecimal(AnnoA.ToString() + MeseA.ToString().PadLeft(2, (char)'0'));

                                #region INDENNITA
                                idVoci = (decimal)EnumVociContabili.Ind_Sede_Estera;
                                lteorici = t.TEORICI.Where(a =>
                                                                    a.ELABINDENNITA.Any(b => b.IDLIVELLO == livdip.IDLIVELLO) &&
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

                                if (lteorici?.Any() ?? false)
                                {
                                    var importo = lteorici.Sum(a => a.IMPORTO);
                                    var descLivello = livdip.LIVELLI.LIVELLO;
                                    RptCostiCoanModel rptccm = SetCostiCoan(matricola, cognome, nome, descLivello, ufficio, idVoci, importo, db);
                                    lrptccm.Add(rptccm);

                                }
                                #endregion

                                #region MAB
                                idVoci = (decimal)EnumVociContabili.MAB;
                                lteorici = t.TEORICI.Where(a =>
                                                                    a.ELABMAB.Any(b => b.IDLIVELLO == livdip.IDLIVELLO) &&
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

                                if (lteorici?.Any() ?? false)
                                {
                                    var importo = lteorici.Sum(a => a.IMPORTO);
                                    var descLivello = livdip.LIVELLI.LIVELLO;
                                    RptCostiCoanModel rptccm = SetCostiCoan(matricola, cognome, nome, descLivello, ufficio, idVoci, importo, db);
                                    lrptccm.Add(rptccm);
                                }
                                #endregion


                                #region PRIMA SISTEMAZIONE
                                idVoci = (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380;
                                lteorici = t.TEORICI.Where(a =>
                                                                    a.ELABINDSISTEMAZIONE?.IDLIVELLO == livdip.IDLIVELLO &&
                                                                    a.ANNULLATO == false &&
                                                                    //a.DIRETTO == false &&
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

                                if (lteorici?.Any() ?? false)
                                {
                                    var importo = lteorici.Sum(a => a.IMPORTO);
                                    var descLivello = livdip.LIVELLI.LIVELLO;

                                    RptCostiCoanModel rptccm = SetCostiCoan(matricola, cognome, nome, descLivello, ufficio, idVoci, importo, db);
                                    lrptccm.Add(rptccm);

                                }
                                #endregion

                                #region RICHIAMO
                                idVoci = (decimal)EnumVociContabili.Ind_Richiamo_IRI;
                                lteorici = t.TEORICI.Where(a =>
                                                                    a.ELABINDRICHIAMO?.IDLIVELLO == livdip.IDLIVELLO &&
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

                                if (lteorici?.Any() ?? false)
                                {
                                    var importo = lteorici.Sum(a => a.IMPORTO);
                                    var descLivello = livdip.LIVELLI.LIVELLO;
                                    RptCostiCoanModel rptccm = SetCostiCoan(matricola, cognome, nome, descLivello, ufficio, idVoci, importo, db);
                                    lrptccm.Add(rptccm);

                                }
                                #endregion

                                #region TRASPORTO EFFETTI
                                idVoci = (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131;
                                lteorici = t.TEORICI.Where(a =>
                                                                    a.ELABTRASPEFFETTI?.IDLIVELLO == livdip.IDLIVELLO &&
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

                                if (lteorici?.Any() ?? false)
                                {
                                    var importo = lteorici.Sum(a => a.IMPORTO);
                                    var descLivello = livdip.LIVELLI.LIVELLO;
                                    RptCostiCoanModel rptccm = SetCostiCoan(matricola, cognome, nome, descLivello, ufficio, idVoci, importo, db);
                                    lrptccm.Add(rptccm);

                                }
                                #endregion

                            }
                            #endregion

                        }
                        #endregion
                    }
                }
                return lrptccm;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public RptCostiCoanModel SetCostiCoan(int matricola, 
                                                string cognome, 
                                                string nome, 
                                                string descLivello, 
                                                string ufficio, 
                                                decimal idVoci, 
                                                decimal importo, 
                                                ModelDBISE db)
        {
            try
            {
                using (dtStatistiche dts = new dtStatistiche())
                {
                    RptCostiCoanModel rptccm = new RptCostiCoanModel()
                    {
                        Matricola = matricola,
                        Nominativo = cognome + " " + nome,
                        Livello = descLivello,
                        Ufficio = ufficio,
                        Descrizione = dts.GetDescrizioneVoci(idVoci, db),
                        Importo = importo
                    };
                    return rptccm;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
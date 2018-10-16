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
            //var lcoan = db.TRASFERIMENTO
            //.Where(a => a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
            //            a.IDTIPOCOAN == (decimal)EnumTipologiaCoan.Servizi_Istituzionali)
            //.OrderBy(a => a.COAN)
            //.GroupBy(a => a.COAN)
            //.ToList();

            //if (lcoan?.Any() ?? false)
            //{
            //    //aggiunge Servizi Istituzionali se presenti
                var tc = db.TIPOLOGIACOAN.Find((decimal)EnumTipologiaCoan.Servizi_Istituzionali);

                ecm = new ElencoCoanModel()
                {
                    idElencoCoan = "00",
                    codiceCoan = tc.DESCRIZIONE
                };

                lecm.Add(ecm);
            //}
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
                                                        a.DATARIENTRO >= dtIni && a.DATAPARTENZA <= dtFin &&
                                                        a.IDTIPOCOAN == (decimal)EnumTipologiaCoan.Servizi_Istituzionali)
                                                    .ToList();
                    }
                    else
                    {
                        lt = db.TRASFERIMENTO.Where(a =>
                                    a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
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
                            lteorici = t.TEORICI.Where(a =>
                                                                a.ELABINDENNITA.Any(b => b.IDLIVELLO == livdip.IDLIVELLO) &&
                                                                a.ANNULLATO == false &&
                                                                a.DIRETTO == false &&
                                                                a.ELABORATO == true &&
                                                                a.INSERIMENTOMANUALE == false &&
                                                                a.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera &&
                                                                a.VOCI.IDTIPOLIQUIDAZIONE==(decimal)EnumTipoLiquidazione.Contabilità &&
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

                                RptCostiCoanModel rptccm = new RptCostiCoanModel()
                                {
                                    Matricola = matricola,
                                    Nominativo = cognome + " " + nome,
                                    Livello = descLivello,
                                    Ufficio = ufficio,
                                    Descrizione = GetDescrizioneVoci((decimal)EnumVociContabili.Ind_Sede_Estera, db),
                                    Importo = importo
                                };
                                lrptccm.Add(rptccm);

                            }
                            #endregion

                            #region MAB
                            lteorici = t.TEORICI.Where(a =>
                                                                a.ELABMAB.Any(b => b.IDLIVELLO == livdip.IDLIVELLO) &&
                                                                a.ANNULLATO == false &&
                                                                a.DIRETTO == false &&
                                                                a.ELABORATO == true &&
                                                                a.INSERIMENTOMANUALE == false &&
                                                                a.IDVOCI == (decimal)EnumVociContabili.MAB &&
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

                                RptCostiCoanModel rptccm = new RptCostiCoanModel()
                                {
                                    Matricola = matricola,
                                    Nominativo = cognome + " " + nome,
                                    Livello = descLivello,
                                    Ufficio = ufficio,
                                    Descrizione = GetDescrizioneVoci((decimal)EnumVociContabili.MAB, db),
                                    Importo = importo
                                };
                                lrptccm.Add(rptccm);

                            }
                            #endregion


                            #region PRIMA SISTEMAZIONE
                            lteorici = t.TEORICI.Where(a =>
                                                                a.ELABINDSISTEMAZIONE?.IDLIVELLO == livdip.IDLIVELLO &&
                                                                a.ANNULLATO == false &&
                                                                //a.DIRETTO == false &&
                                                                a.ELABORATO == true &&
                                                                a.INSERIMENTOMANUALE == false &&
                                                                a.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
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

                                RptCostiCoanModel rptccm = new RptCostiCoanModel()
                                {
                                    Matricola = matricola,
                                    Nominativo = cognome + " " + nome,
                                    Livello = descLivello,
                                    Ufficio = ufficio,
                                    Descrizione = GetDescrizioneVoci((decimal)EnumVociContabili.Ind_Prima_Sist_IPS, db),
                                    Importo = importo
                                };
                                lrptccm.Add(rptccm);

                            }
                            #endregion

                            #region RICHIAMO
                            lteorici = t.TEORICI.Where(a =>
                                                                a.ELABINDRICHIAMO?.IDLIVELLO == livdip.IDLIVELLO &&
                                                                a.ANNULLATO == false &&
                                                                a.DIRETTO == false &&
                                                                a.ELABORATO == true &&
                                                                a.INSERIMENTOMANUALE == false &&
                                                                a.IDVOCI == (decimal)EnumVociContabili.Ind_Richiamo_IRI &&
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

                                RptCostiCoanModel rptccm = new RptCostiCoanModel()
                                {
                                    Matricola = matricola,
                                    Nominativo = cognome + " " + nome,
                                    Livello = descLivello,
                                    Ufficio = ufficio,
                                    Descrizione = GetDescrizioneVoci((decimal)EnumVociContabili.Ind_Richiamo_IRI, db),
                                    Importo = importo
                                };
                                lrptccm.Add(rptccm);

                            }
                            #endregion

                            #region TRASPORTO EFFETTI
                            lteorici = t.TEORICI.Where(a =>
                                                                a.ELABTRASPEFFETTI?.IDLIVELLO == livdip.IDLIVELLO &&
                                                                a.ANNULLATO == false &&
                                                                a.DIRETTO == false &&
                                                                a.ELABORATO == true &&
                                                                a.INSERIMENTOMANUALE == false &&
                                                                a.IDVOCI == (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131 &&
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

                                RptCostiCoanModel rptccm = new RptCostiCoanModel()
                                {
                                    Matricola = matricola,
                                    Nominativo = cognome + " " + nome,
                                    Livello = descLivello,
                                    Ufficio = ufficio,
                                    Descrizione = GetDescrizioneVoci((decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131, db),
                                    Importo = importo
                                };
                                lrptccm.Add(rptccm);

                            }
                            #endregion

                        }
                        #endregion

                    }
                    #endregion
                }
                return lrptccm;
            }
            catch(Exception ex)
            {
                throw ex;
            }
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

    }
}
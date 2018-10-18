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
    public class dtCostiUfficio : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<RptCostiUfficioModel> GetCostiUfficio(decimal MeseDa, decimal AnnoDa, decimal MeseA, decimal AnnoA, decimal idUfficio, ModelDBISE db)
        {
            try
            {
                List<RptCostiUfficioModel> lrptcum = new List<RptCostiUfficioModel>();

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    List<TRASFERIMENTO> lt = new List<TRASFERIMENTO>();
                    List<TEORICI> lteorici = new List<TEORICI>();

                    string strMeseDa = MeseDa.ToString().PadLeft(2, Convert.ToChar("0"));
                    string strMeseA = MeseA.ToString().PadLeft(2, Convert.ToChar("0"));

                    DateTime dtIni = Convert.ToDateTime("01/" + strMeseDa + "/" + AnnoDa.ToString());
                    DateTime dtFin = Utility.GetDtFineMese(Convert.ToDateTime("01/" + strMeseA + "/" + AnnoA.ToString()));

                    #region Elenco Trasferimenti nel range in base al coan
                    if (idUfficio >0)
                    {
                        lt = db.TRASFERIMENTO.Where(a =>
                                                        a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                                        a.DATARIENTRO >= dtIni && a.DATAPARTENZA <= dtFin &&
                                                        a.IDUFFICIO == idUfficio)
                                                    .ToList();
                    }
                    else
                    {
                        lt = db.TRASFERIMENTO.Where(a =>
                                    a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                    a.DATARIENTRO >= dtIni && a.DATAPARTENZA <= dtFin)
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

                                RptCostiUfficioModel rptcum = new RptCostiUfficioModel()
                                {
                                    Matricola = matricola,
                                    Nominativo = cognome + " " + nome,
                                    Livello = descLivello,
                                    Ufficio = ufficio,
                                    Descrizione = GetDescrizioneVoci(idVoci, db),
                                    Importo = importo
                                };
                                lrptcum.Add(rptcum);

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

                                RptCostiUfficioModel rptcum = new RptCostiUfficioModel()
                                {
                                    Matricola = matricola,
                                    Nominativo = cognome + " " + nome,
                                    Livello = descLivello,
                                    Ufficio = ufficio,
                                    Descrizione = GetDescrizioneVoci(idVoci, db),
                                    Importo = importo
                                };
                                lrptcum.Add(rptcum);

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

                                RptCostiUfficioModel rptcum = new RptCostiUfficioModel()
                                {
                                    Matricola = matricola,
                                    Nominativo = cognome + " " + nome,
                                    Livello = descLivello,
                                    Ufficio = ufficio,
                                    Descrizione = GetDescrizioneVoci(idVoci, db),
                                    Importo = importo
                                };
                                lrptcum.Add(rptcum);

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

                                RptCostiUfficioModel rptcum = new RptCostiUfficioModel()
                                {
                                    Matricola = matricola,
                                    Nominativo = cognome + " " + nome,
                                    Livello = descLivello,
                                    Ufficio = ufficio,
                                    Descrizione = GetDescrizioneVoci(idVoci, db),
                                    Importo = importo
                                };
                                lrptcum.Add(rptcum);

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

                                RptCostiUfficioModel rptcum = new RptCostiUfficioModel()
                                {
                                    Matricola = matricola,
                                    Nominativo = cognome + " " + nome,
                                    Livello = descLivello,
                                    Ufficio = ufficio,
                                    Descrizione = GetDescrizioneVoci(idVoci, db),
                                    Importo = importo
                                };
                                lrptcum.Add(rptcum);

                            }
                            #endregion

                        }
                        #endregion

                    }
                    #endregion
                }
                return lrptcum;
            }
            catch(Exception ex)
            {
                throw ex;
            }
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
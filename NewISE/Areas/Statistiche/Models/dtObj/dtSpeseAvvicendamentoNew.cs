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
    public class dtSpeseAvvicendamentoNew : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<RptSpeseAvvicendamentoNewModel> GetSpeseAvvicendamento(decimal MeseDa, decimal AnnoDa, decimal MeseA, decimal AnnoA, ModelDBISE db)
        {
            try
            {
                List<RptSpeseAvvicendamentoNewModel> lrptsam = new List<RptSpeseAvvicendamentoNewModel>();

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

                        #region Elenco Trasferimenti nel range
                        lt = db.TRASFERIMENTO.Where(a =>
                                                a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                                a.DATARIENTRO >= dtIni && a.DATAPARTENZA <= dtFin)
                                            .ToList();

                        #endregion

                        #region ciclo trasferimenti    
                        foreach (var t in lt)
                        {
                            var d = t.DIPENDENTI;
                            var nome = d.NOME;
                            var cognome = d.COGNOME;
                            var matricola = d.MATRICOLA;
                            var ufficio = t.UFFICI.CODICEUFFICIO + " " + t.UFFICI.DESCRIZIONEUFFICIO;
                            string dataPartenza = t.DATAPARTENZA.ToShortDateString();
                            decimal idVoci = 0;
                            decimal idTipoLiquidazione = 0;

                            #region elenco livelli x trasferimento
                            var llivdip = t.INDENNITA.LIVELLIDIPENDENTI
                                                .Where(a =>
                                                            a.ANNULLATO == false &&
                                                            a.DATAFINEVALIDITA >= dtIni &&
                                                            a.DATAINIZIOVALIDITA <= dtFin)
                                                .ToList();
                            #endregion

                            #region ciclo livelli
                            foreach (var livdip in llivdip)
                            {
                                var liv = livdip.LIVELLI;

                                var annoMeseInizio = Convert.ToDecimal(AnnoDa.ToString() + MeseDa.ToString().PadLeft(2, (char)'0'));
                                var annoMeseFine = Convert.ToDecimal(AnnoA.ToString() + MeseA.ToString().PadLeft(2, (char)'0'));

                                #region PRIMA SISTEMAZIONE
                                idVoci = (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380;
                                idTipoLiquidazione = (decimal)EnumTipoLiquidazione.Paghe;
                                lteorici = dts.GetIndennitaPS(t, livdip.IDLIVELLO, idVoci, idTipoLiquidazione, annoMeseInizio, annoMeseFine, db);

                                if (lteorici?.Any() ?? false)
                                {
                                    var importo = lteorici.Sum(a => a.IMPORTO);
                                    var descLivello = livdip.LIVELLI.LIVELLO;

                                    RptSpeseAvvicendamentoNewModel rptsam = SetSpeseAvvicendamento(matricola, cognome, nome, descLivello, ufficio, idVoci, dataPartenza, importo, db);
                                    lrptsam.Add(rptsam);

                                }
                                #endregion

                                #region TRASPORTO EFFETTI PARTENZA
                                idVoci = (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131;
                                idTipoLiquidazione = (decimal)EnumTipoLiquidazione.Paghe;
                                lteorici = dts.GetIndennitaTEP(t, livdip.IDLIVELLO, idVoci, idTipoLiquidazione, annoMeseInizio, annoMeseFine, db);

                                if (lteorici?.Any() ?? false)
                                {
                                    var importo = lteorici.Sum(a => a.IMPORTO);
                                    var descLivello = livdip.LIVELLI.LIVELLO;

                                    RptSpeseAvvicendamentoNewModel rptsam = SetSpeseAvvicendamento(matricola, cognome, nome, descLivello, ufficio, idVoci, dataPartenza, importo, db);

                                    lrptsam.Add(rptsam);

                                }
                                #endregion

                            }
                            #endregion

                        }
                        #endregion
                    }
                }
                return lrptsam;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public RptSpeseAvvicendamentoNewModel SetSpeseAvvicendamento(
                                                int matricola,
                                                string cognome,
                                                string nome,
                                                string descLivello,
                                                string ufficio,
                                                decimal idVoci,
                                                string dataPartenza,
                                                decimal importo,
                                                ModelDBISE db)
        {
            try
            {
                using (dtStatistiche dts = new dtStatistiche())
                {
                    RptSpeseAvvicendamentoNewModel rptsam = new RptSpeseAvvicendamentoNewModel()
                    {
                        Matricola = matricola,
                        Nominativo = cognome + " " + nome,
                        Livello = descLivello,
                        Ufficio = ufficio,
                        DataPartenza=dataPartenza,
                        Descrizione = dts.GetDescrizioneVoci(idVoci, db),
                        Importo = importo
                    };
                    return rptsam;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
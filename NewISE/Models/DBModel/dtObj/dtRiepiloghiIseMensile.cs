using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.Models.Enumeratori;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;
using System.Net.Configuration;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using NewISE.Models.IseArio.dtObj;
using NewISE.Interfacce.Modelli;
using NewISE.Models.DBModel.bsObj;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;
using NewISE.Areas.Statistiche.Models;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtRiepiloghiIseMensile : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public IList<RptRiepiloghiIseMensileModel> GetRiepiloghiIseMensile(decimal idElabIni, decimal idElabFin, decimal MeseDa, decimal AnnoDa, decimal MeseA, decimal AnnoA, ModelDBISE db)

        {
            List<RptRiepiloghiIseMensileModel> rim = new List<RptRiepiloghiIseMensileModel>();

            List<TRASFERIMENTO> lt = new List<TRASFERIMENTO>();

            string strMeseDa = MeseDa.ToString().PadLeft(2, Convert.ToChar("0"));
            string strMeseA = MeseA.ToString().PadLeft(2, Convert.ToChar("0"));

            DateTime dtIni = Convert.ToDateTime("01/" + strMeseDa + "/" + AnnoDa.ToString());
            DateTime dtFin = Utility.GetDtFineMese(Convert.ToDateTime("01/" + strMeseA + "/" + AnnoA.ToString()));

            lt = db.TRASFERIMENTO
                            .Where(a => 
                                        a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                        a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Da_Attivare &&
                                        a.DATARIENTRO >= dtIni && 
                                        a.DATAPARTENZA <= dtFin)
                            .ToList();

            foreach (var t in lt)
            {
                var d = t.DIPENDENTI;

                var uf = t.UFFICI;

                #region elenco livelli x trasferimento
                var llivdip = t.INDENNITA.LIVELLIDIPENDENTI
                                            .Where(a => a.ANNULLATO == false &&
                                                        a.DATAFINEVALIDITA >= dtIni &&
                                                        a.DATAINIZIOVALIDITA <= dtFin)
                                            .ToList();
                #endregion

                #region ciclo livelli
                foreach (var livdip in llivdip)
                {
                    var annoMeseInizio = Convert.ToDecimal(AnnoDa.ToString() + MeseDa.ToString().PadLeft(2, (char)'0'));
                    var annoMeseFine = Convert.ToDecimal(AnnoA.ToString() + MeseA.ToString().PadLeft(2, (char)'0'));

                    #region lista teorici per livello (tutte le indennita interessate)
                    var lteorici = t.TEORICI.Where(a =>
                                        a.ANNULLATO == false &&
                                        a.ELABORATO == true &&
                                        a.INSERIMENTOMANUALE == false &&
                                        (
                                            a.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS || 
                                            a.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera || 
                                            a.IDVOCI == (decimal)EnumVociContabili.Ind_Richiamo_IRI
                                        ) &&
                                        a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                        Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                            a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) >= annoMeseInizio &&
                                        Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                            a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) <= annoMeseFine)
                            .GroupBy(a=> new {a.ANNORIFERIMENTO, a.MESERIFERIMENTO })
                            .ToList();
                    #endregion

                    #region cicla i gruppi di anno/mese rif teorici
                    foreach (var teorici in lteorici)
                    {
                        decimal prima_sistemazione_anticipo = 0;
                        decimal prima_sistemazione_saldo = 0;
                        decimal prima_sistemazione_unica_sol = 0;
                        decimal indennita = 0;
                        decimal richiamo = 0;

                        MESEANNOELABORAZIONE meseannoElab = new MESEANNOELABORAZIONE();
                        string strMeseAnnoElab = "";
                        string strMeseAnnoRif = "";
                        decimal numMeseRiferimento = 0;
                        decimal numMeseElaborazione = 0;

                        #region cicla le singole righe di teorici
                        foreach (var teorici_row in teorici)
                        {
                            if (teorici_row.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                                    teorici_row.ELABINDSISTEMAZIONE?.IDLIVELLO == livdip.IDLIVELLO &&
                                    teorici_row.ELABINDSISTEMAZIONE.ANTICIPO)
                            {
                                prima_sistemazione_anticipo = teorici_row.IMPORTO;
                            }

                            if (teorici_row.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                                    teorici_row.ELABINDSISTEMAZIONE?.IDLIVELLO == livdip.IDLIVELLO &&
                                    (teorici_row.ELABINDSISTEMAZIONE.SALDO || teorici_row.ELABINDSISTEMAZIONE.CONGUAGLIO))
                            {
                                prima_sistemazione_saldo = teorici_row.IMPORTO;
                            }

                            if (teorici_row.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                                    teorici_row.ELABINDSISTEMAZIONE?.IDLIVELLO == livdip.IDLIVELLO &&
                                    teorici_row.ELABINDSISTEMAZIONE.UNICASOLUZIONE)
                            {
                                prima_sistemazione_unica_sol = teorici_row.IMPORTO;
                            }

                            if (teorici_row.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera &&
                                teorici_row.DIRETTO == false &&
                                teorici_row.ELABINDENNITA.Any(b => b.IDLIVELLO == livdip.IDLIVELLO))
                            {
                                indennita = teorici_row.IMPORTO;
                            }

                            if (teorici_row.IDVOCI == (decimal)EnumVociContabili.Ind_Richiamo_IRI &&
                                teorici_row.ELABINDRICHIAMO?.IDLIVELLO == livdip.IDLIVELLO)

                            {
                                richiamo = teorici_row.IMPORTO;
                            }

                            meseannoElab = db.MESEANNOELABORAZIONE.Find(teorici_row.IDMESEANNOELAB);
                            using (dtElaborazioni dte = new dtElaborazioni())
                            {
                                strMeseAnnoElab = CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)meseannoElab.MESE) + " " + meseannoElab.ANNO.ToString();
                                strMeseAnnoRif = CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)teorici_row.MESERIFERIMENTO) + " " + teorici_row.ANNORIFERIMENTO.ToString();
                            }
                            numMeseRiferimento = Convert.ToDecimal(teorici_row.ANNORIFERIMENTO.ToString() + teorici_row.MESERIFERIMENTO.ToString().ToString().PadLeft(2, (char)'0'));
                            numMeseElaborazione = Convert.ToDecimal(meseannoElab.ANNO.ToString() + meseannoElab.MESE.ToString().PadLeft(2, (char)'0'));
                        }

                        RptRiepiloghiIseMensileModel rptisem = new RptRiepiloghiIseMensileModel()
                        {
                            nominativo = d.COGNOME + " " + d.NOME + " (" + d.MATRICOLA + ")",
                            qualifica = livdip.LIVELLI.LIVELLO,
                            ufficio = uf.DESCRIZIONEUFFICIO,
                            riferimento = strMeseAnnoRif,
                            elaborazione = strMeseAnnoElab,
                            prima_sistemazione_anticipo = prima_sistemazione_anticipo,
                            prima_sistemazione_saldo = prima_sistemazione_saldo,
                            prima_sistemazione_unica_soluz=prima_sistemazione_unica_sol,
                            richiamo = richiamo,
                            indennita_personale = indennita,
                            numannomeseelab = numMeseElaborazione,
                            numannomeserif = numMeseRiferimento
                        };
                        rim.Add(rptisem); 
                        #endregion
                    }
                    #endregion
                }
                #endregion               
            }

            return rim;
        }


    }
}
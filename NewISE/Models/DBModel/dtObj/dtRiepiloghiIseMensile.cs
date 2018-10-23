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
        public IList<RiepiloghiIseMensileModel> GetRiepiloghiIseMensile(decimal idElabIni, decimal idElabFin, decimal MeseDa, decimal AnnoDa, decimal MeseA, decimal AnnoA, ModelDBISE db)

        {   
            List<RiepiloghiIseMensileModel> rim = new List<RiepiloghiIseMensileModel>();

            List<TRASFERIMENTO> lt = new List<TRASFERIMENTO>();
            List<TEORICI> lteorici = new List<TEORICI>();

            string strMeseDa = MeseDa.ToString().PadLeft(2, Convert.ToChar("0"));
            string strMeseA = MeseA.ToString().PadLeft(2, Convert.ToChar("0"));

            DateTime dtIni = Convert.ToDateTime("01/" + strMeseDa + "/" + AnnoDa.ToString());
            DateTime dtFin = Utility.GetDtFineMese(Convert.ToDateTime("01/" + strMeseA + "/" + AnnoA.ToString()));


            //lt = db.TRASFERIMENTO.Where(a => a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
            //                            a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Da_Attivare &&
            //                            a.DATARIENTRO >= dtIni && a.DATAPARTENZA <= dtFin)
            //                            .ToList();


            var lTeorici =
                   db.TEORICI.Where(
                       a =>
                           a.ANNULLATO == false &&
                           a.ELABORATO == true &&
                           a.IDMESEANNOELAB >= idElabIni &&
                           a.IDMESEANNOELAB <= idElabFin &&
                           a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                           a.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera)
                        .ToList();


            foreach (var t in lt)
            {
                var d = t.DIPENDENTI;
                var nome = d.NOME;
                var cognome = d.COGNOME;
                var matricola = d.MATRICOLA;
                var ufficio = t.UFFICI.DESCRIZIONEUFFICIO;                

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

                    
                    lteorici = t.TEORICI.Where(a =>
                                                        a.ELABINDENNITA.Any(b => b.IDLIVELLO == livdip.IDLIVELLO && b.DAL >= dtIni && b.AL <= dtFin) &&
                                                        a.ANNULLATO == false &&
                                                        a.DIRETTO == false &&
                                                        a.ELABORATO == true &&
                                                        a.INSERIMENTOMANUALE == false &&
                                                        a.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera &&
                                                        a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                                        Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                                            a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) >= annoMeseInizio &&
                                                        Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                                                                            a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) <= annoMeseFine)
                                            .OrderBy(a => a.ANNORIFERIMENTO)
                                            .ThenBy(a => a.MESERIFERIMENTO).ToList();

                    foreach (var teorici in lteorici)
                    {
                        var periodoelaborazione = teorici.DATAOPERAZIONE;

                        var mese = teorici.MESERIFERIMENTO;
                        var anno = teorici.ANNORIFERIMENTO;

                        var strMeseRif = CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)teorici.MESERIFERIMENTO) + " " + teorici.ANNORIFERIMENTO.ToString();
                        var meseannoelab = db.MESEANNOELABORAZIONE.Find(teorici.IDMESEANNOELAB);

                        //var strMeseElab = CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)teorici.IDMESEANNOELAB) + " " + teorici.IDMESEANNOELAB.ToString();


                        var Indennita = teorici.IMPORTO;
                        var descLivello = livdip.LIVELLI.LIVELLO;

                        var lelab = teorici.ELABINDENNITA.Where(a => a.ANNULLATO == false && a.DAL >= dtIni && a.AL <= dtFin).ToList();
                        

                            RiepiloghiIseMensileModel ldvm = new RiepiloghiIseMensileModel()
                            {
                                Nominativo = cognome + " " + nome + " (" + matricola + ")",
                                qualifica = descLivello,
                                Ufficio = ufficio,
                                indennita_personale = Indennita.ToString(),
                                //riferimento = mese + " - " + anno,
                                riferimento = strMeseRif,
                                elaborazione = meseannoelab.MESE + " - " + meseannoelab.ANNO


                            };

                            rim.Add(ldvm);

                    }

                    #region PRIMA SISTEMAZIONE
                    lteorici = t.TEORICI.Where(a =>
                                                        a.ELABINDSISTEMAZIONE?.IDLIVELLO == livdip.IDLIVELLO &&
                                                        
                                                        a.ANNULLATO == false &&
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

                    foreach (var teoriciprimasist in lteorici)
                    {
                        var mese = teoriciprimasist.MESERIFERIMENTO;
                        var anno = teoriciprimasist.ANNORIFERIMENTO;

                        var IndennitaPrimaSistemazione = teoriciprimasist.IMPORTO;
                        var descLivello = livdip.LIVELLI.LIVELLO;

                        var anticipo = teoriciprimasist.ELABINDSISTEMAZIONE.ANTICIPO;
                        var saldo = teoriciprimasist.ELABINDSISTEMAZIONE.SALDO;

                        var strMeseRif = CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)teoriciprimasist.MESERIFERIMENTO) + " " + teoriciprimasist.ANNORIFERIMENTO.ToString();
                        var meseannoelab = db.MESEANNOELABORAZIONE.Find(teoriciprimasist.IDMESEANNOELAB);

                        RiepiloghiIseMensileModel ldvm = new RiepiloghiIseMensileModel()
                        {
                            Nominativo = cognome + " " + nome + " (" + matricola + ")",
                            qualifica = descLivello,
                            Ufficio = ufficio,
                            prima_sistemazione = IndennitaPrimaSistemazione.ToString(),
                            //riferimento = mese + " " + anno
                            riferimento = strMeseRif
                        };

                        rim.Add(ldvm);

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

                    foreach (var teoricirichiamo in lteorici)
                    {

                        var mese = teoricirichiamo.MESERIFERIMENTO;
                        var anno = teoricirichiamo.ANNORIFERIMENTO;

                        var IndennitaRichiamo = teoricirichiamo.IMPORTO;
                        var descLivello = livdip.LIVELLI.LIVELLO;

                        var strMeseRif = CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)teoricirichiamo.MESERIFERIMENTO) + " " + teoricirichiamo.ANNORIFERIMENTO.ToString();
                        var meseannoelab = db.MESEANNOELABORAZIONE.Find(teoricirichiamo.IDMESEANNOELAB);

                        RiepiloghiIseMensileModel ldvm = new RiepiloghiIseMensileModel()
                        {   
                            Nominativo = cognome + " " + nome + " (" + matricola + ")",
                            qualifica = descLivello,
                            Ufficio = ufficio,
                            richiamo = IndennitaRichiamo.ToString(),
                            //riferimento = mese + " " + anno
                            riferimento = strMeseRif
                        };

                        rim.Add(ldvm);

                       

                    }
                    #endregion

                    

                }
                #endregion

            }

            

            // Sistema configurato con il DatePicker
            //DateTime dataDal = Convert.ToDateTime(dtIni);
            //DateTime dataAl = Convert.ToDateTime(dtFin);


           




            //lt = db.TRASFERIMENTO.Where(a =>
            //                                            a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
            //                                            a.DATARIENTRO >= Convert.ToDateTime(dtIni) && a.DATAPARTENZA <= Convert.ToDateTime(dtFin) )
            //                                        .ToList();


            //var lTeorici =
            //       db.TEORICI.Where(
            //           a =>
            //               a.ANNULLATO == false &&
            //               a.ELABORATO == true &&
            //               a.IDMESEANNOELAB >= dtIni && 
            //               a.IDMESEANNOELAB <= dtFin &&
            //               a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
            //               a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera).ToList();



            //if (lTeorici?.Any() ?? false)
            //{
            //    foreach (var t in lTeorici)
            //    {   
            //        var ltr = t.ELABINDENNITA.Where(
            //           a =>
            //               a.ANNULLATO == false
            //            ).ToList();

            //            foreach (var tr in ltr)
            //            {
            //                var dip = tr.INDENNITA.TRASFERIMENTO;
            //                var dipendenti = tr.INDENNITA.TRASFERIMENTO.DIPENDENTI;

            //                var eeee = tr.LIVELLI.LIVELLIDIPENDENTI.First();

            //                var xxx = dipendenti.LIVELLIDIPENDENTI.First();

            //            var yyy = xxx.LIVELLI.LIVELLIDIPENDENTI;

            //                var uf = dip.UFFICI;
            //                var tm = t.TIPOMOVIMENTO;
            //                var voce = t.VOCI;
            //                var tl = t.VOCI.TIPOLIQUIDAZIONE;
            //                var tv = t.VOCI.TIPOVOCE;

            //                var qualifica = dip.INDENNITA.LIVELLIDIPENDENTI;

            //                var Qualifica = qualifica.First();
            //                var qualifica1 = Qualifica.LIVELLI.LIVELLO;



            //                    RiepiloghiIseMensileModel ldvm = new RiepiloghiIseMensileModel()
            //                    {
            //                        idTeorici = t.IDTEORICI,
            //                        Nominativo = dipendenti.COGNOME + " " + dipendenti.NOME + " (" + dipendenti.MATRICOLA + ")",
            //                        Ufficio = uf.DESCRIZIONEUFFICIO + " (" + uf.CODICEUFFICIO + ")",
            //                        TipoMovimento = new TipoMovimentoModel()
            //                        {
            //                            idTipoMovimento = tm.IDTIPOMOVIMENTO,
            //                            TipoMovimento = tm.TIPOMOVIMENTO1,
            //                            DescMovimento = tm.DESCMOVIMENTO
            //                        },
            //                        Voci = new VociModel()
            //                        {
            //                            idVoci = voce.IDVOCI,
            //                            codiceVoce = voce.CODICEVOCE,
            //                            descrizione = voce.DESCRIZIONE,
            //                            TipoLiquidazione = new TipoLiquidazioneModel()
            //                            {
            //                                idTipoLiquidazione = tl.IDTIPOLIQUIDAZIONE,
            //                                descrizione = tl.DESCRIZIONE
            //                            },
            //                            TipoVoce = new TipoVoceModel()
            //                            {
            //                                idTipoVoce = tv.IDTIPOVOCE,
            //                                descrizione = tv.DESCRIZIONE
            //                            }
            //                        },
            //                        meseRiferimento = t.MESERIFERIMENTO,
            //                        annoRiferimento = t.ANNORIFERIMENTO,                        
            //                        Importo = t.IMPORTO,
            //                        Elaborato = t.ELABORATO
            //                    };

            //                rim.Add(ldvm);
            //            }
            //    }
            //}

            return rim;
            
        }


    }
}
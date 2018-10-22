using NewISE.Areas.Statistiche.Models;
using NewISE.EF;
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.Models.Enumeratori;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtReportTrasportoEffetti: IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public IList<TrasportoEffettiModel> GetTrasportoEffetti(decimal dtIni, decimal dtFin, decimal annoDa, decimal meseDa, decimal annoA, decimal meseA, ModelDBISE db)
        {
           

            string strMeseDa = meseDa.ToString().PadLeft(2, Convert.ToChar("0"));
            string strMeseA = meseA.ToString().PadLeft(2, Convert.ToChar("0"));

            DateTime dtIni1 = Convert.ToDateTime("01/" + strMeseDa + "/" + annoDa.ToString());
            DateTime dtFin1 = Utility.GetDtFineMese(Convert.ToDateTime("01/" + strMeseA + "/" + annoA.ToString()));

            List<TrasportoEffettiModel> rim = new List<TrasportoEffettiModel>();

            var lTeorici =
                  db.TEORICI.Where(
                      a =>
                          a.ANNULLATO == false &&
                          a.ELABORATO == true &&
                          a.IDMESEANNOELAB >= dtIni &&
                          a.IDMESEANNOELAB <= dtFin &&
                                          a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
                                          a.VOCI.IDTIPOVOCE == (decimal)EnumTipoVoce.Software &&
                                          a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131).ToList();


            foreach (var Teorici in lTeorici)
            {
                //var lelabtraspeffetti = Teorici.ELABTRASPEFFETTI.Where(a => a.ANNULLATO == false &&
                //                                          a.DAL >= dtIni1 &&
                //                                          a.AL <= dtFin1).ToList();

               
                    
                    var tr = Teorici.TRASFERIMENTO;
                    var d = tr.DIPENDENTI;

                    var uf = tr.UFFICI;
                    var tm = Teorici.TIPOMOVIMENTO;
                    var voce = Teorici.VOCI;
                    var tl = Teorici.VOCI.TIPOLIQUIDAZIONE;
                    var tv = Teorici.VOCI.TIPOVOCE;

                    var meseannoElab = db.MESEANNOELABORAZIONE.Find(Teorici.IDMESEANNOELAB);
                    var strMeseAnnoElab = "";
                    var strMeseAnnoRif = "";
                    using (dtElaborazioni dte = new dtElaborazioni())
                    {
                        strMeseAnnoElab = CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)meseannoElab.MESE) + " " + meseannoElab.ANNO.ToString();
                        strMeseAnnoRif = CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)Teorici.MESERIFERIMENTO) + " " + Teorici.ANNORIFERIMENTO.ToString();
                    }
                    decimal numMeseRiferimento = Convert.ToDecimal(Teorici.ANNORIFERIMENTO.ToString() + Teorici.MESERIFERIMENTO.ToString().ToString().PadLeft(2, (char)'0'));
                    decimal numMeseElaborazione = Convert.ToDecimal(meseannoElab.ANNO.ToString() + meseannoElab.MESE.ToString().PadLeft(2, (char)'0'));


                   TrasportoEffettiModel ldvm = new TrasportoEffettiModel()
                   {
                        matricola = d.MATRICOLA,
                        Nominativo = d.NOME + " " + d.COGNOME,
                        Ufficio = uf.DESCRIZIONEUFFICIO,
                        Importo = Teorici.IMPORTO,
                        MeseElaborazione = strMeseAnnoElab,
                        MeseRiferimento = strMeseAnnoRif,
                        numMeseElaborazione = numMeseElaborazione,
                        numMeseRiferimento = numMeseRiferimento


                   };

                   rim.Add(ldvm);
                
            }



            //if (lTeorici?.Any() ?? false)
            //{

            //    foreach (var t in lTeorici)
            //    {

            //            //var dip = new DIPENDENTI();

            //            //if (t.ELABTRASPEFFETTI?.IDELABTRASPEFFETTI > 0)
            //            //{
            //            //    if (t.ELABTRASPEFFETTI?.IDTEPARTENZA > 0)
            //            //    {
            //            //        dip = t.ELABTRASPEFFETTI.TEPARTENZA.TRASFERIMENTO.DIPENDENTI;
            //            //    }
            //            //    else if (t.ELABTRASPEFFETTI?.IDTERIENTRO > 0)
            //            //    {
            //            //        dip = t.ELABTRASPEFFETTI.TERIENTRO.TRASFERIMENTO.DIPENDENTI;
            //            //    }
            //            //}


            //            var tr = t.ELABTRASPEFFETTI.TEPARTENZA.TRASFERIMENTO;
            //            var dip = tr.TEPARTENZA.TRASFERIMENTO;
            //            var dipendenti = tr.TEPARTENZA.TRASFERIMENTO.DIPENDENTI;

            //            var uf = dip.UFFICI;
            //            var tm = t.TIPOMOVIMENTO;
            //            var voce = t.VOCI;
            //            var tl = t.VOCI.TIPOLIQUIDAZIONE;
            //            var tv = t.VOCI.TIPOVOCE;

            //            TrasportoEffettiModel ldvm = new TrasportoEffettiModel()
            //            {
            //                idTeorici = t.IDTEORICI,
            //                Nominativo = dipendenti.COGNOME + " " + dipendenti.NOME + " (" + dipendenti.MATRICOLA + ")",
            //                Ufficio = uf.DESCRIZIONEUFFICIO + " (" + uf.CODICEUFFICIO + ")",
            //                TipoMovimento = new TipoMovimentoModel()
            //                {
            //                    idTipoMovimento = tm.IDTIPOMOVIMENTO,
            //                    TipoMovimento = tm.TIPOMOVIMENTO1,
            //                    DescMovimento = tm.DESCMOVIMENTO
            //                },
            //                Voci = new VociModel()
            //                {
            //                    idVoci = voce.IDVOCI,
            //                    codiceVoce = voce.CODICEVOCE,
            //                    descrizione = voce.DESCRIZIONE,
            //                    TipoLiquidazione = new TipoLiquidazioneModel()
            //                    {
            //                        idTipoLiquidazione = tl.IDTIPOLIQUIDAZIONE,
            //                        descrizione = tl.DESCRIZIONE
            //                    },
            //                    TipoVoce = new TipoVoceModel()
            //                    {
            //                        idTipoVoce = tv.IDTIPOVOCE,
            //                        descrizione = tv.DESCRIZIONE
            //                    }
            //                },
            //                meseRiferimento = t.MESERIFERIMENTO,
            //                annoRiferimento = t.ANNORIFERIMENTO,
            //                Importo = t.IMPORTO,
            //                Elaborato = t.ELABORATO
            //            };

            //            rim.Add(ldvm);

            //    }


            //    foreach (var t in lTeorici)
            //    {

            //        var tr = t.ELABTRASPEFFETTI.TERIENTRO.TRASFERIMENTO;

            //        var dip = tr.TERIENTRO.TRASFERIMENTO;
            //        var dipendenti = tr.TERIENTRO.TRASFERIMENTO.DIPENDENTI;

            //        var uf = dip.UFFICI;
            //        var tm = t.TIPOMOVIMENTO;
            //        var voce = t.VOCI;
            //        var tl = t.VOCI.TIPOLIQUIDAZIONE;
            //        var tv = t.VOCI.TIPOVOCE;

            //        TrasportoEffettiModel ldvm = new TrasportoEffettiModel()
            //        {
            //            idTeorici = t.IDTEORICI,
            //            Nominativo = dipendenti.COGNOME + " " + dipendenti.NOME + " (" + dipendenti.MATRICOLA + ")",
            //            Ufficio = uf.DESCRIZIONEUFFICIO + " (" + uf.CODICEUFFICIO + ")",
            //            TipoMovimento = new TipoMovimentoModel()
            //            {
            //                idTipoMovimento = tm.IDTIPOMOVIMENTO,
            //                TipoMovimento = tm.TIPOMOVIMENTO1,
            //                DescMovimento = tm.DESCMOVIMENTO
            //            },
            //            Voci = new VociModel()
            //            {
            //                idVoci = voce.IDVOCI,
            //                codiceVoce = voce.CODICEVOCE,
            //                descrizione = voce.DESCRIZIONE,
            //                TipoLiquidazione = new TipoLiquidazioneModel()
            //                {
            //                    idTipoLiquidazione = tl.IDTIPOLIQUIDAZIONE,
            //                    descrizione = tl.DESCRIZIONE
            //                },
            //                TipoVoce = new TipoVoceModel()
            //                {
            //                    idTipoVoce = tv.IDTIPOVOCE,
            //                    descrizione = tv.DESCRIZIONE
            //                }
            //            },
            //            meseRiferimento = t.MESERIFERIMENTO,
            //            annoRiferimento = t.ANNORIFERIMENTO,
            //            Importo = t.IMPORTO,
            //            Elaborato = t.ELABORATO
            //        };

            //        rim.Add(ldvm);

            //    }

            //}

            return rim;

        }
    }
}
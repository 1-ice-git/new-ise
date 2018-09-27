using NewISE.Areas.Statistiche.Models;
using NewISE.EF;
using NewISE.Models.Enumeratori;
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
        public IList<TrasportoEffettiModel> GetTrasportoEffetti(decimal dtIni, decimal dtFin, ModelDBISE db)
        {
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


            if (lTeorici?.Any() ?? false)
            {

                foreach (var t in lTeorici)
                {

                        //var dip = new DIPENDENTI();
                    
                        //if (t.ELABTRASPEFFETTI?.IDELABTRASPEFFETTI > 0)
                        //{
                        //    if (t.ELABTRASPEFFETTI?.IDTEPARTENZA > 0)
                        //    {
                        //        dip = t.ELABTRASPEFFETTI.TEPARTENZA.TRASFERIMENTO.DIPENDENTI;
                        //    }
                        //    else if (t.ELABTRASPEFFETTI?.IDTERIENTRO > 0)
                        //    {
                        //        dip = t.ELABTRASPEFFETTI.TERIENTRO.TRASFERIMENTO.DIPENDENTI;
                        //    }
                        //}
                    

                        var tr = t.ELABTRASPEFFETTI.TEPARTENZA.TRASFERIMENTO;
                        var dip = tr.TEPARTENZA.TRASFERIMENTO;
                        var dipendenti = tr.TEPARTENZA.TRASFERIMENTO.DIPENDENTI;

                        var uf = dip.UFFICI;
                        var tm = t.TIPOMOVIMENTO;
                        var voce = t.VOCI;
                        var tl = t.VOCI.TIPOLIQUIDAZIONE;
                        var tv = t.VOCI.TIPOVOCE;

                        TrasportoEffettiModel ldvm = new TrasportoEffettiModel()
                        {
                            idTeorici = t.IDTEORICI,
                            Nominativo = dipendenti.COGNOME + " " + dipendenti.NOME + " (" + dipendenti.MATRICOLA + ")",
                            Ufficio = uf.DESCRIZIONEUFFICIO + " (" + uf.CODICEUFFICIO + ")",
                            TipoMovimento = new TipoMovimentoModel()
                            {
                                idTipoMovimento = tm.IDTIPOMOVIMENTO,
                                TipoMovimento = tm.TIPOMOVIMENTO1,
                                DescMovimento = tm.DESCMOVIMENTO
                            },
                            Voci = new VociModel()
                            {
                                idVoci = voce.IDVOCI,
                                codiceVoce = voce.CODICEVOCE,
                                descrizione = voce.DESCRIZIONE,
                                TipoLiquidazione = new TipoLiquidazioneModel()
                                {
                                    idTipoLiquidazione = tl.IDTIPOLIQUIDAZIONE,
                                    descrizione = tl.DESCRIZIONE
                                },
                                TipoVoce = new TipoVoceModel()
                                {
                                    idTipoVoce = tv.IDTIPOVOCE,
                                    descrizione = tv.DESCRIZIONE
                                }
                            },
                            meseRiferimento = t.MESERIFERIMENTO,
                            annoRiferimento = t.ANNORIFERIMENTO,
                            Importo = t.IMPORTO,
                            Elaborato = t.ELABORATO
                        };

                        rim.Add(ldvm);
                    
                }


                foreach (var t in lTeorici)
                {

                    var tr = t.ELABTRASPEFFETTI.TERIENTRO.TRASFERIMENTO;

                    var dip = tr.TERIENTRO.TRASFERIMENTO;
                    var dipendenti = tr.TERIENTRO.TRASFERIMENTO.DIPENDENTI;

                    var uf = dip.UFFICI;
                    var tm = t.TIPOMOVIMENTO;
                    var voce = t.VOCI;
                    var tl = t.VOCI.TIPOLIQUIDAZIONE;
                    var tv = t.VOCI.TIPOVOCE;

                    TrasportoEffettiModel ldvm = new TrasportoEffettiModel()
                    {
                        idTeorici = t.IDTEORICI,
                        Nominativo = dipendenti.COGNOME + " " + dipendenti.NOME + " (" + dipendenti.MATRICOLA + ")",
                        Ufficio = uf.DESCRIZIONEUFFICIO + " (" + uf.CODICEUFFICIO + ")",
                        TipoMovimento = new TipoMovimentoModel()
                        {
                            idTipoMovimento = tm.IDTIPOMOVIMENTO,
                            TipoMovimento = tm.TIPOMOVIMENTO1,
                            DescMovimento = tm.DESCMOVIMENTO
                        },
                        Voci = new VociModel()
                        {
                            idVoci = voce.IDVOCI,
                            codiceVoce = voce.CODICEVOCE,
                            descrizione = voce.DESCRIZIONE,
                            TipoLiquidazione = new TipoLiquidazioneModel()
                            {
                                idTipoLiquidazione = tl.IDTIPOLIQUIDAZIONE,
                                descrizione = tl.DESCRIZIONE
                            },
                            TipoVoce = new TipoVoceModel()
                            {
                                idTipoVoce = tv.IDTIPOVOCE,
                                descrizione = tv.DESCRIZIONE
                            }
                        },
                        meseRiferimento = t.MESERIFERIMENTO,
                        annoRiferimento = t.ANNORIFERIMENTO,
                        Importo = t.IMPORTO,
                        Elaborato = t.ELABORATO
                    };

                    rim.Add(ldvm);

                }

            }

            return rim;

        }
    }
}
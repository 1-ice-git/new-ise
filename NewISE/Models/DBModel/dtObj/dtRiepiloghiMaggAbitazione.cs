﻿using System;
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
    public class dtRiepiloghiMaggAbitazione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<RiepiloghiMaggAbitazioneModel> GetRiepiloghiMaggAbitazione(decimal dtIni, decimal dtFin, ModelDBISE db)
        {
            List<RiepiloghiMaggAbitazioneModel> rim = new List<RiepiloghiMaggAbitazioneModel>();

            var lTeorici =
                   db.TEORICI.Where(
                       a =>
                           a.ANNULLATO == false &&
                           a.ELABORATO == true &&
                           a.IDMESEANNOELAB >= dtIni &&
                           a.IDMESEANNOELAB <= dtFin &&
                           a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                           a.VOCI.IDVOCI == (decimal)EnumVociContabili.MAB).ToList();



            if (lTeorici?.Any() ?? false)
            {
                foreach (var t in lTeorici)
                {
                    var ltr = t.ELABMAB.Where(
                       a =>
                           a.ANNULLATO == false
                        ).ToList();

                    foreach (var tr in ltr)
                    {
                        var dip = tr.INDENNITA.TRASFERIMENTO;
                        var dipendenti = tr.INDENNITA.TRASFERIMENTO.DIPENDENTI;

                        var uf = dip.UFFICI;
                        var tm = t.TIPOMOVIMENTO;
                        var voce = t.VOCI;
                        var tl = t.VOCI.TIPOLIQUIDAZIONE;
                        var tv = t.VOCI.TIPOVOCE;


                        RiepiloghiMaggAbitazioneModel ldvm = new RiepiloghiMaggAbitazioneModel()
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
            }

            return rim;

        }


    }
}
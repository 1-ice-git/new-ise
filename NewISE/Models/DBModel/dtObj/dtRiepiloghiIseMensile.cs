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
        public IList<RiepiloghiIseMensileModel> GetRiepiloghiIseMensile(decimal dtIni, decimal dtFin, ModelDBISE db)
        {   
            List<RiepiloghiIseMensileModel> rim = new List<RiepiloghiIseMensileModel>();
           

            // Sistema configurato con il DatePicker
            //DateTime dataDal = Convert.ToDateTime(dtIni);
            //DateTime dataAl = Convert.ToDateTime(dtFin);


            //var lTeorici =
            //       db.TEORICI.Where(
            //           a =>
            //               a.ANNULLATO == false &&
            //               a.ELABORATO == true &&
            //               a.ELABINDENNITA.Any(b => b.ANNULLATO == false && b.AL == dataDal && b.DAL == dataAl ) &&
            //               a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera).ToList();


            var lTeorici =
                   db.TEORICI.Where(
                       a =>
                           a.ANNULLATO == false &&
                           a.ELABORATO == true &&
                           a.IDMESEANNOELAB >= dtIni && 
                           a.IDMESEANNOELAB <= dtFin &&
                           a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                           a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera).ToList();
            
            if (lTeorici?.Any() ?? false)
            {
                foreach (var t in lTeorici)
                {
                    //var ei =
                    //    t.ELABINDENNITA.Last(
                    //        a =>
                    //            a.ANNULLATO == false &&
                    //            a.PROGRESSIVO ==
                    //            t.ELABINDENNITA.Where(b => b.ANNULLATO == false).Max(b => b.PROGRESSIVO));


                    //var tr = ei.INDENNITA.TRASFERIMENTO;
                    //var dip = tr.DIPENDENTI;
                    var tm = t.TIPOMOVIMENTO;
                    var voce = t.VOCI;
                    var tl = t.VOCI.TIPOLIQUIDAZIONE;
                    var tv = t.VOCI.TIPOVOCE;
                    //var uf = tr.UFFICI;

                    RiepiloghiIseMensileModel ldvm = new RiepiloghiIseMensileModel()
                    {
                        idTeorici = t.IDTEORICI,
                        //Nominativo = dip.COGNOME + " " + dip.NOME + " (" + dip.MATRICOLA + ")",
                        //Ufficio = uf.DESCRIZIONEUFFICIO + " (" + uf.CODICEUFFICIO + ")",
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
using NewISE.Areas.Statistiche.Models;
using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.Models.Enumeratori;
using NewISE.Models.Tools;
using NewISE.Models.DBModel.dtObj;

using System.Configuration;
using System.Data;
using NewISE.Models.dtObj.ModelliCalcolo;
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
using NewISE.Models.ViewModel;
using NewISE.Models.DBModel;

namespace NewISE.Areas.Statistiche.Models.dtObj
{
    public class dtRiepiloghiMaggAbitazione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<RptRiepiloghiMaggAbitazioneModel> GetRiepiloghiMaggAbitazione(decimal idElabIni, decimal idElabFin, ModelDBISE db)
        {

            //string strMeseDa = MeseDa.ToString().PadLeft(2, Convert.ToChar("0"));
            //string strMeseA = MeseA.ToString().PadLeft(2, Convert.ToChar("0"));

            //DateTime dtIni = Convert.ToDateTime("01/" + strMeseDa + "/" + AnnoDa.ToString());
            //DateTime dtFin = Utility.GetDtFineMese(Convert.ToDateTime("01/" + strMeseA + "/" + AnnoA.ToString()));

            //decimal annoMeseInizio = Convert.ToDecimal(AnnoDa.ToString() + MeseDa.ToString().PadLeft(2, (char)'0'));
            //decimal annoMeseFine = Convert.ToDecimal(AnnoA.ToString() + MeseA.ToString().PadLeft(2, (char)'0'));

            List<RptRiepiloghiMaggAbitazioneModel> rim = new List<RptRiepiloghiMaggAbitazioneModel>();

            var lTeorici =
                   db.TEORICI.Where(
                       a =>
                           a.ANNULLATO == false &&
                           a.ELABORATO == true &&
                           //Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                           //                     a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) >= annoMeseInizio &&
                           //Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                           //                    a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) <= annoMeseFine &&
                           a.IDMESEANNOELAB >= idElabIni &&
                           a.IDMESEANNOELAB <= idElabFin &&
                           a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                           a.IDVOCI == (decimal)EnumVociContabili.MAB)
                        .ToList();

            foreach (var Teorici in lTeorici)
            {
                var lelabmab = Teorici.ELABMAB.Where(a => a.ANNULLATO == false)
                                        .ToList();

                foreach (var elabmab in lelabmab)
                {
                    //var dip = elabmab.INDENNITA.TRASFERIMENTO;
                    //var dipendenti = elabmab.INDENNITA.TRASFERIMENTO.DIPENDENTI;
                    var tr = elabmab.INDENNITA.TRASFERIMENTO;
                    var d = tr.DIPENDENTI;

                    var uf = tr.UFFICI;
                    var tm = Teorici.TIPOMOVIMENTO;
                    var voce = Teorici.VOCI;
                    var tl = Teorici.VOCI.TIPOLIQUIDAZIONE;
                    var tv = Teorici.VOCI.TIPOVOCE;

                    RptRiepiloghiMaggAbitazioneModel ldvm = new RptRiepiloghiMaggAbitazioneModel()
                    {
                        Nominativo = d.COGNOME + " " + d.NOME,
                        Ufficio = uf.DESCRIZIONEUFFICIO + " (" + uf.CODICEUFFICIO + ")",
                        Matricola = d.MATRICOLA.ToString(),
                        DataPartenza  = tr.DATAPARTENZA.ToShortDateString(),
                        DataLettera = tr.DATALETTERA.ToString(),
                        DataOperazione=elabmab.DATAOPERAZIONE.ToString(),
                        Canone=elabmab.CANONELOCAZIONE,
                        percApplicata=elabmab.PERCMAB,
                        Importo = Teorici.IMPORTO,
                    };

                    rim.Add(ldvm);
                }
            }

            return rim;

        }


    }
}
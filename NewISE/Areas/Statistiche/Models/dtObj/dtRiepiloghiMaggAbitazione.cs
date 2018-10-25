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

        public List<RptRiepiloghiMaggAbitazioneModel> GetRiepiloghiMaggAbitazione(decimal idElabIni, decimal idElabFin, decimal annoDa, decimal meseDa, decimal annoA, decimal meseA, ModelDBISE db)
        {

            string strMeseDa = meseDa.ToString().PadLeft(2, Convert.ToChar("0"));
            string strMeseA = meseA.ToString().PadLeft(2, Convert.ToChar("0"));

            DateTime dtIni = Convert.ToDateTime("01/" + strMeseDa + "/" + annoDa.ToString());
            DateTime dtFin = Utility.GetDtFineMese(Convert.ToDateTime("01/" + strMeseA + "/" + annoA.ToString()));

            List<RptRiepiloghiMaggAbitazioneModel> rim = new List<RptRiepiloghiMaggAbitazioneModel>();

            var lTeorici =
                   db.TEORICI.Where(
                       a =>
                           a.ANNULLATO == false &&
                           a.ELABORATO == true &&                    
                           a.IDMESEANNOELAB >= idElabIni &&
                           a.IDMESEANNOELAB <= idElabFin &&
                           a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                           a.IDVOCI == (decimal)EnumVociContabili.MAB)
                        .ToList();

            foreach (var Teorici in lTeorici)
            {
                var lelabmab = Teorici.ELABMAB.Where(a => a.ANNULLATO == false &&
                                                          a.DAL >=dtIni &&
                                                          a.AL<=dtFin)
                                        .ToList();

                foreach (var elabmab in lelabmab)
                {
                    var tr = elabmab.INDENNITA.TRASFERIMENTO;
                    var d = tr.DIPENDENTI;

                    var uf = tr.UFFICI;
                    var tm = Teorici.TIPOMOVIMENTO;
                    var voce = Teorici.VOCI;
                    var tl = Teorici.VOCI.TIPOLIQUIDAZIONE;
                    var tv = Teorici.VOCI.TIPOVOCE;
                    string valuta = "";

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

                    using (dtValute dtv = new dtValute())
                    {
                        valuta = dtv.GetValuta(elabmab.IDVALUTA).descrizioneValuta;
                    }

                    RptRiepiloghiMaggAbitazioneModel ldvm = new RptRiepiloghiMaggAbitazioneModel()
                    {
                        Nominativo = d.COGNOME + " " + d.NOME,
                        Ufficio = uf.DESCRIZIONEUFFICIO,
                        Matricola = d.MATRICOLA.ToString(),
                        MeseElaborazione = strMeseAnnoElab,
                        MeseRiferimento = strMeseAnnoRif,
                        Canone = elabmab.CANONELOCAZIONE,
                        percApplicata = elabmab.PERCMAB,
                        Importo = Teorici.IMPORTO,
                        numMeseElaborazione = numMeseElaborazione,
                        numMeseRiferimento = numMeseRiferimento,
                        tfr = elabmab.TASSOFISSORAGGUAGLIO,
                        Valuta=valuta
                    };

                    rim.Add(ldvm);
                }
            }

            return rim;

        }


    }
}
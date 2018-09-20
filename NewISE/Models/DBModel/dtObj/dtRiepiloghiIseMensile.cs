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
        public IList<RiepiloghiIseMensileModel> GetRiepiloghiIseMensile(string dtIni, string dtFin, ModelDBISE db)
        {   
            List<RiepiloghiIseMensileModel> rim = new List<RiepiloghiIseMensileModel>();
            
            DateTime dataDal = Convert.ToDateTime(dtIni);
            DateTime dataAl = Convert.ToDateTime(dtFin);


            var lTeorici =
                   db.TEORICI.Where(
                       a =>
                           a.ANNULLATO == false &&
                           a.ELABORATO == true &&
                           a.ELABINDENNITA.Any(b => b.ANNULLATO == false && b.AL == dataDal && b.DAL == dataAl ) &&
                           a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera).ToList();
            

            foreach (var t in lTeorici)
            {
                string tipoOperazione = string.Empty;

                
                var dip = t.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO.DIPENDENTI;

                var ldvm = new RiepiloghiIseMensileModel()
                {
                    idTeorici = t.IDTEORICI,
                    Nominativo = dip.COGNOME + " " + dip.NOME + " (" + dip.MATRICOLA + ")",
                    idVoci = t.IDVOCI,
                    Voci = new VociModel()
                    {
                        idVoci = t.VOCI.IDVOCI,
                        idTipoLiquidazione = t.VOCI.IDTIPOLIQUIDAZIONE,
                        idTipoVoce = t.VOCI.IDTIPOVOCE,
                        codiceVoce = t.VOCI.CODICEVOCE,
                        descrizione =
                            t.VOCI.DESCRIZIONE + " (" + t.ELABINDSISTEMAZIONE.PERCANTSALDOUNISOL.ToString() + "% - " + tipoOperazione + ")",
                        flagDiretto = t.DIRETTO,
                        TipoLiquidazione = new TipoLiquidazioneModel()
                        {
                            idTipoLiquidazione = t.VOCI.IDTIPOLIQUIDAZIONE,
                            descrizione = t.VOCI.TIPOLIQUIDAZIONE.DESCRIZIONE,
                        },
                        TipoVoce = new TipoVoceModel()
                        {
                            idTipoVoce = t.VOCI.IDTIPOVOCE,
                            descrizione = t.VOCI.TIPOVOCE.DESCRIZIONE
                        }
                    },
                    Data = t.DATAOPERAZIONE,
                    Importo = t.IMPORTO,
                    Elaborato = t.ELABORATO
                };

                rim.Add(ldvm);
            }



            return rim;
            
        }


    }
}
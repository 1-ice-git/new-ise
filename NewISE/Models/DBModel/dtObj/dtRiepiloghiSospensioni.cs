using NewISE.Areas.Statistiche.Models;
using NewISE.EF;
using NewISE.Models.Enumeratori;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtRiepiloghiSospensioni : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<RiepiloghiSospensioniModel> GetRiepiloghiSospensioni(DateTime idIni, DateTime idFin, ModelDBISE db)
        {
            List<RiepiloghiSospensioniModel> rim = new List<RiepiloghiSospensioniModel>();

            List<TRASFERIMENTO> lt = new List<TRASFERIMENTO>();
            
            lt = db.TRASFERIMENTO.Where(a => a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                        a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Da_Attivare &&
                                        a.DATARIENTRO >= idIni && a.DATAPARTENZA <= idFin)
                                        .ToList();


            foreach (var t in lt)
            {

                var sospensione = t.SOSPENSIONE.Where(a => a.ANNULLATO == false &&
                                                      idIni <= a.DATAFINE && idFin >= a.DATAINIZIO).ToList();

                foreach (var sosp in sospensione)
                {
                    var DataInizioSospensione = sosp.DATAINIZIO;
                    var DataFineSospensione = sosp.DATAFINE;
                    var TipoSospensione = sosp.TIPOSOSPENSIONE.DESCRIZIONE;
                    var Nominativo = sosp.TRASFERIMENTO.DIPENDENTI.NOME + " " + sosp.TRASFERIMENTO.DIPENDENTI.COGNOME;
                    var Ufficio = sosp.TRASFERIMENTO.UFFICI.DESCRIZIONEUFFICIO;
                    

                    RiepiloghiSospensioniModel ldvm = new RiepiloghiSospensioniModel()
                    {
                        DataInizioSospensione = DataInizioSospensione,
                        DataFineSospensione = DataFineSospensione,
                        DataAggiornamento = sosp.DATAAGGIORNAMENTO,
                        TipoSospensione = TipoSospensione.ToString(),
                        Nominativo = Nominativo,
                        Ufficio = Ufficio,
                        NumeroGiorni = Convert.ToInt32((DataFineSospensione - DataInizioSospensione).TotalDays)
                        
                    };

                    rim.Add(ldvm);


                }
                    
            }

            return rim;

        }



    }
}
using NewISE.Areas.Statistiche.Models;
using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Models.Enumeratori;
using NewISE.Models.Tools;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtDipEsteroLivello : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public IList<DipEsteroLivelloNewModel> DipEsteroLivelloNew(DateTime dtIni, DateTime dtFin, decimal idLivello, ModelDBISE db)
        {
            List<DipEsteroLivelloNewModel> rim = new List<DipEsteroLivelloNewModel>();


            var ltrasf = db.TRASFERIMENTO.Where(a => 
                                                    a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                                    a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Da_Attivare &&
                                                    a.DATARIENTRO >= dtIni &&
                                                    a.DATAPARTENZA <= dtFin &&
                                                    a.INDENNITA.LIVELLIDIPENDENTI.Any(b => 
                                                                                            b.ANNULLATO == false &&
                                                                                            b.DATAFINEVALIDITA >= dtIni &&
                                                                                            b.DATAINIZIOVALIDITA <= dtFin &&
                                                                                            b.IDLIVELLO == idLivello))
                                        .ToList();


            var qualifica = db.LIVELLI.Find(idLivello).LIVELLO;

            if (ltrasf?.Any() ?? false)
            {
                foreach (var trasf in ltrasf)
                {
                    var lrd = trasf.RUOLODIPENDENTE.Where(a => a.ANNULLATO == false && a.DATAINZIOVALIDITA == trasf.DATAPARTENZA).OrderBy(a => a.DATAINZIOVALIDITA);
                    var rd = lrd.First();

                    //var ldescruolo = ruolo.First();
                    var descruolo = rd.RUOLOUFFICIO.DESCRUOLO;

                    var d = trasf.DIPENDENTI;
                    var nome = d.NOME;
                    var cognome = d.COGNOME;
                    var matricola = d.MATRICOLA;
                    var ufficio = trasf.UFFICI.DESCRIZIONEUFFICIO;

                    DipEsteroLivelloNewModel ldvm = new DipEsteroLivelloNewModel()
                    {
                        nominativo = d.COGNOME + " " + d.NOME + " (" + d.MATRICOLA + ")",
                        data_trasferimento = trasf.DATAPARTENZA,
                        data_rientro = (trasf.DATARIENTRO < Utility.DataFineStop()) ? Convert.ToDateTime(trasf.DATARIENTRO).ToShortDateString() : null,
                        sede = ufficio,
                        qualifica = qualifica,
                        ruolo_dipendente = descruolo
                    };
                    rim.Add(ldvm);
                }
            }

            return rim;

        }


    }
}
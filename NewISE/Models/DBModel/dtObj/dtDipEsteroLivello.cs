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


            var ltrasf = db.TRASFERIMENTO.Where(a => a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                               a.DATARIENTRO >= dtIni &&
                                               a.DATAPARTENZA <= dtFin &&
                                               a.INDENNITA.LIVELLIDIPENDENTI.Any(b => b.ANNULLATO == false &&
                                               b.DATAFINEVALIDITA >= dtIni &&
                                               b.DATAINIZIOVALIDITA <= dtFin &&
                                               b.IDLIVELLO == idLivello)).ToList();


            var qualifica = db.LIVELLI.Find(idLivello).LIVELLO;

            //var lrd = db.TRASFERIMENTO.Find(idTrasferimento).RUOLODIPENDENTE.Where(a => a.ANNULLATO == false);
            //var ruolo = db.RUOLODIPENDENTE.Find(idLivello).IDRUOLODIPENDENTE;
            //var t = db.TRASFERIMENTO.Find(idTrasferimento);
            //var lrd = t.RUOLODIPENDENTE.Where(a => a.ANNULLATO == false && a.DATAINZIOVALIDITA == t.DATAPARTENZA).OrderBy(a => a.DATAINZIOVALIDITA);

            


            if (ltrasf?.Any() ?? false)
            {
                foreach (var trasf in ltrasf)
                {

                    var livello = ltrasf.First();
                    var dip = livello.DIPENDENTI;

                    var ufficio = livello.UFFICI;
                    var descrUfficio = ufficio.DESCRIZIONEUFFICIO;

                    var lruolo = ltrasf.First();
                    var ruolo = lruolo.RUOLODIPENDENTE;

                    var ldescruolo = ruolo.First();
                    var descruolo = ldescruolo.RUOLOUFFICIO.DESCRUOLO;

                    DipEsteroLivelloNewModel ldvm = new DipEsteroLivelloNewModel()
                    {

                        nominativo = dip.COGNOME + " " + dip.NOME + " (" + dip.MATRICOLA + ")",
                        data_trasferimento = Convert.ToDateTime(trasf.DATAPARTENZA).ToShortDateString(),
                        data_rientro = (trasf.DATARIENTRO < Utility.DataFineStop()) ? Convert.ToDateTime(trasf.DATARIENTRO).ToShortDateString() : null,
                        sede = ufficio.DESCRIZIONEUFFICIO,
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
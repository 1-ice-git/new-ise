using NewISE.Areas.Statistiche.Models;
using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
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
         

            var liv = db.LIVELLIDIPENDENTI
                       .Where(a => a.ANNULLATO == false &&
                       a.IDLIVELLO == idLivello &&
                       a.DATAINIZIOVALIDITA >= dtIni &&
                       a.DATAFINEVALIDITA <= dtFin).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();



            if (liv?.Any() ?? false)
            {
                foreach (var d in liv)
                {

                        var livello = liv.First();

                        var dip = livello.DIPENDENTI;
                        var trasf = dip.TRASFERIMENTO;


                        DipEsteroLivelloNewModel ldvm = new DipEsteroLivelloNewModel()
                        {   
                            nominativo = dip.COGNOME + " " + dip.NOME + " (" + dip.MATRICOLA + ")",
                        };

                        rim.Add(ldvm);

                   

                }
            }

            return rim;

        }


    }
}
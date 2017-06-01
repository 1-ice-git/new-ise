using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Dipendenti.Models.DtObj
{
    public class dtDipLivelli : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public LivelliDipendentiModel GetLivelloDipendente(decimal idDipendente, DateTime data)
        {
            LivelliDipendentiModel ldm = new LivelliDipendentiModel();

            using (EntitiesDBISE db=new EntitiesDBISE())
            {
                var ld = db.LIVELLIDIPENDENTI.Where(a=> a.IDDIPENDENTE == idDipendente && data >= a.DATAINIZIOVALIDITA && data.Date <= a.DATAFINEVALIDITA).ToList();

                ldm = (from e in ld
                       select new LivelliDipendentiModel() {
                           idLivDipendente = e.IDLIVDIPENDENTE,
                           idDipendente = e.IDDIPENDENTE,
                           idLivello = e.IDLIVELLO,
                           dataInizioValdita = e.DATAINIZIOVALIDITA,
                           dataFineValidita = e.DATAFINEVALIDITA,
                           dataAggiornamento = e.DATAAGGIORNAMENTO,
                           annullato = e.ANNULLATO,
                           Livello = new LivelloModel()
                           {
                               idLivello = e.LIVELLI.IDLIVELLO,
                               DescLivello = e.LIVELLI.LIVELLO
                           }
                       }).First();
            }

            return ldm;
        }


    }
}
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.dtObj
{
    public class dtLivelliDipendente :IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public LivelloDipendenteModel GetLivelloDipendente(decimal idDipendente, DateTime data)
        {
            LivelloDipendenteModel ldm = new LivelloDipendenteModel();

            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                var ld = db.LIVELLIDIPENDENTI.Where(a => a.IDDIPENDENTE == idDipendente && data >= a.DATAINIZIOVALIDITA && data.Date <= a.DATAFINEVALIDITA).ToList();

                ldm = (from e in ld
                       select new LivelloDipendenteModel()
                       {
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
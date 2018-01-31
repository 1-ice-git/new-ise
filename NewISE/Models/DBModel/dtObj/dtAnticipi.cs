using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using NewISE.EF;
using Newtonsoft.Json.Schema;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtAnticipi : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public AttivitaAnticipiModel GetAttivitaAnticipi(decimal idPrimaSistemazione)
        {
            AttivitaAnticipiModel aam = new AttivitaAnticipiModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var ps = db.PRIMASITEMAZIONE.Find(idPrimaSistemazione);

                var aal = ps.ATTIVITAANTICIPI.Where(a => a.ANNULLATO == false).OrderByDescending(a=>a.IDATTIVITAANTICIPI).ToList();

                if (aal?.Any()??false)
                {
                    var aa = aal.First();

                    aam = new AttivitaAnticipiModel()
                    {
                        idAttivitaAnticipi=aa.IDPRIMASISTEMAZIONE,
                        idPrimaSistemazione = aa.IDPRIMASISTEMAZIONE,
                        notificaRichiestaAnticipi=aa.NOTIFICARICHIESTA,
                        dataNotificaRichiesta=aa.DATANOTIFICARICHIESTA,
                        attivaRichiestaAnticipi=aa.ATTIVARICHIESTA,
                        dataAttivaRichiesta=aa.DATAATTIVARICHIESTA,
                        dataAggiornamento=aa.DATAAGGIORNAMENTO,
                        annullato=aa.ANNULLATO
                    };
                }
            }

            return aam;
        }



    }
}
using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtNotifiche : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<NotificheModel> GetNotifiche(decimal idMittenteLogato)
        {
            List<NotificheModel> lnot = new List<NotificheModel>();
            using (ModelDBISE db = new ModelDBISE())
            {
                lnot = (from e in db.NOTIFICHE
                        select new NotificheModel()
                        {
                            idNotifica = e.IDNOTIFICA,
                            idMittente = e.IDMITTENTE,
                            idDestinatario=e.DIPENDENTI.IDDIPENDENTE,
                            //idDestinatario=e.
                            Oggetto = e.OGGETTO,
                            corpoMessaggio = e.CORPOMESSAGGIO,
                            dataNotifica = e.DATANOTIFICA,
                            Allegato = e.ALLEGATO,
                            Nominativo = e.DIPENDENTI.NOME + "  " + e.DIPENDENTI.COGNOME,
                        }).Where(a => a.idMittente == idMittenteLogato).OrderByDescending(a => a.dataNotifica).ToList();
            }
            return lnot;
        }
    }
}
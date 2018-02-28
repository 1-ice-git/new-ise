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
        public void AssociaDipendentiNotifiche(decimal? idDipendente, decimal idNotifiche, ModelDBISE db)
        {
            try
            {
                var f = db.DIPENDENTI.Find(idDipendente);
                var item = db.Entry<DIPENDENTI>(f);
                item.State = System.Data.Entity.EntityState.Modified;
                var adf = db.NOTIFICHE.Find(idNotifiche);
                f.NOTIFICHE.Add(adf);
                int i = db.SaveChanges();
                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare altri dati Notifiche per il dipendente. {0}", f.COGNOME + " " + f.NOME));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
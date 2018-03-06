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
        public IList<DestinatarioModel> GetListDestinatari(decimal idNotifica)
        {
            List<DestinatarioModel> ldes = new List<DestinatarioModel>();
            using (ModelDBISE db = new ModelDBISE())
            {
                ldes = (from d in db.DESTINATARI where d.IDNOTIFICA==idNotifica 
                        select new DestinatarioModel()
                        {
                            idNotifica = d.IDNOTIFICA,
                            idDipendente=d.IDDIPENDENTE,
                            Nominativi = d.DIPENDENTI.NOME + "  " + d.DIPENDENTI.COGNOME,
                            ToCc =d.TOCC
                        }).ToList();
            }
            return ldes;
        }
        public IList<NotificheModel> GetNotifiche(decimal idMittenteLogato)
        {
            //List<DestinatarioModel> ldes=GetListDestinatari()
            List<NotificheModel> lnot = new List<NotificheModel>();
           
            using (ModelDBISE db = new ModelDBISE())
            {
                lnot = (from e in db.NOTIFICHE
                        where e.IDMITTENTE == idMittenteLogato
                        orderby e.DATANOTIFICA
                        select new NotificheModel()
                        {
                            idNotifica = e.IDNOTIFICA,
                            idMittente = e.IDMITTENTE,
                            Oggetto = e.OGGETTO,
                            NumeroDestinatari = e.DESTINATARI.Count,
                            //(from e1 in e.DESTINATARI
                            //                select new DestinatarioModel()
                            //                {
                            //                    idDipendente = e1.IDDIPENDENTE,
                            //                    idNotifica = e1.IDNOTIFICA,
                            //                    ToCc = e1.TOCC
                            //                }).ToList().Count,
                            corpoMessaggio = e.CORPOMESSAGGIO,
                            dataNotifica = e.DATANOTIFICA,
                            Allegato = e.ALLEGATO,
                            Nominativo = e.DIPENDENTI.NOME + "  " + e.DIPENDENTI.COGNOME,
                        }).ToList();
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
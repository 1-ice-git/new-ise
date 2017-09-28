using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtPrimaSistemazione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void InserisciPrimaSistemazione()
        {

        }

        public void SetPrimaSistemazione(PrimaSistemazioneModel psm, ModelDBISE db)
        {
            PRIMASITEMAZIONE ps = new PRIMASITEMAZIONE()
            {
                IDPRIMASISTEMAZIONE = psm.idPrimaSistemazione,
                DATAOPERAZIONE = psm.dataOperazione,
                RICALCOLATA = psm.ricalcolata
            };

            db.PRIMASITEMAZIONE.Add(ps);

            int i = db.SaveChanges();
            if (i <= 0)
            {
                throw new Exception("Errore nell'insenrimento della prima sistemazione.");
            }

        }


    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using NewISE.EF;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtAttivazionePassaporto : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public AttivazionePassaportiModel GetAttivazionePassaportiDaLavorare(decimal idPassaporto, ModelDBISE db)
        {
            AttivazionePassaportiModel apm = new AttivazionePassaportiModel();

            var p = db.PASSAPORTI.Find(idPassaporto);

            if (p != null && p.IDPASSAPORTI > 0)
            {
                var lap =
                    p.ATTIVAZIONIPASSAPORTI.Where(
                        a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == false && a.PRATICACONCLUSA == false);

                if (lap?.Any() ?? false)
                {
                    var ap = lap.First();
                    apm = new AttivazionePassaportiModel()
                    {
                        idAttivazioniPassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                        idPassaporti = ap.IDPASSAPORTI,
                        notificaRichiesta = ap.NOTIFICARICHIESTA,
                        dataNotificaRichiesta = ap.DATANOTIFICARICHIESTA,
                        praticaConclusa = ap.PRATICACONCLUSA,
                        dataPraticaConclusa = ap.DATAPRATICACONCLUSA,

                    };
                }
            }


            return apm;

        }

        public void AssociaConiuge(decimal idAttivazionePassaporto, decimal idConiuge, ModelDBISE db)
        {
            try
            {

                var ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazionePassaporto);
                var item = db.Entry<ATTIVAZIONIPASSAPORTI>(ap);
                item.State = EntityState.Modified;
                item.Collection(a => a.CONIUGE).Load();
                var c = db.CONIUGE.Find(idConiuge);
                ap.CONIUGE.Add(c);

                int i = db.SaveChanges();


                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il coniuge per l'attivazione del passaporto per {0}.", c.COGNOME + " " + c.NOME));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



    }
}
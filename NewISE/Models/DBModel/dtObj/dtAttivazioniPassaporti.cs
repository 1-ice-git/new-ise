using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.Tools;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtAttivazioniPassaporti : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void SetAttivazioniPassaporti(AttivazionePassaportiModel apm, ModelDBISE db)
        {
            ATTIVAZIONIPASSAPORTI ap = new ATTIVAZIONIPASSAPORTI()
            {
                IDPASSAPORTI = apm.idPassaporti,
                NOTIFICARICHIESTA = apm.notificaRichiesta,
                DATANOTIFICARICHIESTA = apm.dataNotificaRichiesta,
                PRATICACONCLUSA = apm.praticaConclusa,
                DATAPRATICACONCLUSA = apm.dataPraticaConclusa,
                //ESCLUDIPASSAPORTO = apm.escludiPassaporto,
                DATAAGGIORNAMENTO = apm.dataAggiornamento,
                ANNULLATO = apm.annullato
            };

            var p = db.PASSAPORTI.Find(ap.IDPASSAPORTI);
            p.ATTIVAZIONIPASSAPORTI.Add(ap);

            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Errore nella fase d'inserimento per le attivazioni del passaporto.");
            }
            else
            {
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                    "Inserimento dei dati per le attivazioni del passaporto", "ATTIVAZIONIPASSAPORTI", db,
                    p.TRASFERIMENTO.IDTRASFERIMENTO, ap.IDATTIVAZIONIPASSAPORTI);
            }

        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtAttivazionePassaporto : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public GestioneChkEscludiPassaportoModel GetGestioneEcludiPassaporto(decimal idPassaporto)
        {
            return null;
        }

        public AttivazionePassaportiModel GetLastAttivazionePassaporti(decimal idPassaporto, ModelDBISE db)
        {
            AttivazionePassaportiModel apm = new AttivazionePassaportiModel();

            var p = db.PASSAPORTI.Find(idPassaporto);

            if (p != null && p.IDPASSAPORTI > 0)
            {
                var lap =
                    p.ATTIVAZIONIPASSAPORTI.Where(
                        a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI);

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

        public void AssociaFiglio(decimal idAttivazionePassaporto, decimal idFiglio, ModelDBISE db)
        {
            try
            {

                var ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazionePassaporto);
                var item = db.Entry<ATTIVAZIONIPASSAPORTI>(ap);
                item.State = EntityState.Modified;
                item.Collection(a => a.FIGLI).Load();
                var f = db.FIGLI.Find(idFiglio);
                ap.FIGLI.Add(f);

                int i = db.SaveChanges();


                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il figlio per l'attivazione del passaporto per {0}.", f.COGNOME + " " + f.NOME));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void SetPassaportoRichiedente(ref PassaportoRichiedenteModel prm, ModelDBISE db)
        {
            PASSAPORTORICHIEDENTE pr = new PASSAPORTORICHIEDENTE()
            {
                IDPASSAPORTI = prm.idPassaporti,
                ESCLUDIPASSAPORTO = prm.EscludiPassaporto,
                DATAESCLUDIPASSAPORTO = prm.DataEscludiPassaporto,
                DATAAGGIORNAMENTO = prm.DataAggiornamento,
                ANNULLATO = prm.annullato
            };

            var p = db.PASSAPORTI.Find(prm.idPassaporti);

            p.PASSAPORTORICHIEDENTE.Add(pr);

            int i = db.SaveChanges();

            if (i > 0)
            {
                prm.idPassaportoRichiedente = pr.IDPASSAPORTORICHIEDENTE;

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                    "Inizializzazione dei dati per il passaporto del richiedente", "PASSAPORTORICHIEDENTE", db,
                    pr.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, pr.IDPASSAPORTORICHIEDENTE);


            }




        }

        public void SetAttivazioniPassaporti(ref AttivazionePassaportiModel apm, ModelDBISE db)
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
                apm.idAttivazioniPassaporti = ap.IDATTIVAZIONIPASSAPORTI;

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                    "Inserimento dei dati per le attivazioni del passaporto", "ATTIVAZIONIPASSAPORTI", db,
                    p.TRASFERIMENTO.IDTRASFERIMENTO, ap.IDATTIVAZIONIPASSAPORTI);
            }

        }

    }
}
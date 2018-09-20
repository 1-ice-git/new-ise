using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;
using NewISE.EF;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtAttivazionePassaporto : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public GestioneChkincludiPassaportoModel GetGestioneInludiPassaporto(decimal idAttivazionePassaporto, decimal idFamiliarePassaporto, EnumParentela parentela, bool esisteDoc, bool includiPassaporto)
        {
            GestioneChkincludiPassaportoModel gcip = new GestioneChkincludiPassaportoModel();
            bool dchk = false;

            using (ModelDBISE db = new ModelDBISE())
            {
                using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                {
                    ATTIVAZIONIPASSAPORTI ap = new ATTIVAZIONIPASSAPORTI();

                    try
                    {

                        ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazionePassaporto);

                        if (ap?.IDATTIVAZIONIPASSAPORTI <= 0)
                        {
                            throw new Exception("Ciclo di attivazione non presente.");
                        }

                        //EnumFasePassaporti FasePassaporti = dtpp.GetFasePassaporti(ap.IDPASSAPORTI);
                        var attivazioneFaseRichiesta = dtpp.FaseRichiestaPassaporti(ap.IDPASSAPORTI);
                        var attivazioneFaseInvio = dtpp.FaseInvioPassaporti(ap.IDPASSAPORTI);



                        //bool faseRichiesta = false;
                        bool faseRichiestaNotificata = false;
                        //bool faseRichiestaAttivata = false;
                        //bool faseInvio = false;
                        bool faseInvioNotificata = false;
                        bool faseInvioAttivata = false;
                        if (attivazioneFaseRichiesta.IDATTIVAZIONIPASSAPORTI > 0)
                        {
                            //faseRichiesta = true;
                            //faseRichiestaAttivata = attivazioneFaseRichiesta.PRATICACONCLUSA;
                            faseRichiestaNotificata = attivazioneFaseRichiesta.NOTIFICARICHIESTA;
                        }
                        if (attivazioneFaseInvio.IDATTIVAZIONIPASSAPORTI > 0)
                        {
                            //faseInvio = true;
                            faseInvioNotificata = attivazioneFaseInvio.NOTIFICARICHIESTA;
                            faseInvioAttivata = attivazioneFaseInvio.PRATICACONCLUSA;
                        }

                        if (faseRichiestaNotificata || faseInvioAttivata || faseInvioNotificata || ap.PASSAPORTI.TRASFERIMENTO.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Annullato)
                        //ap.NOTIFICARICHIESTA == true || ap.PRATICACONCLUSA == true || ap.PASSAPORTI.TRASFERIMENTO.IDSTATOTRASFERIMENTO==(decimal)EnumStatoTraferimento.Attivo )
                        {
                            dchk = true;
                        }

                        gcip = new GestioneChkincludiPassaportoModel()
                        {
                            idFamiliare = idFamiliarePassaporto,
                            parentela = parentela,
                            esisteDoc = esisteDoc,
                            includiPassaporto = includiPassaporto,
                            disabilitaChk = dchk,
                        };

                        return gcip;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
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
                        dataVariazione = ap.DATAVARIAZIONE,
                        dataAggiornamento = ap.DATAAGGIORNAMENTO,
                        annullato = ap.ANNULLATO
                    };
                }
            }


            return apm;

        }

        public AttivazionePassaportiModel GetAttivazionePassaportiByIdRichiedente(decimal idPassaportoRichiedente)
        {
            AttivazionePassaportiModel apm = new AttivazionePassaportiModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                var pr = db.PASSAPORTORICHIEDENTE.Find(idPassaportoRichiedente);

                if (pr?.IDPASSAPORTORICHIEDENTE > 0)
                {
                    var ap = pr.ATTIVAZIONIPASSAPORTI;
                    apm = new AttivazionePassaportiModel()
                    {
                        idAttivazioniPassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                        idPassaporti = ap.IDPASSAPORTI,
                        notificaRichiesta = ap.NOTIFICARICHIESTA,
                        dataNotificaRichiesta = ap.DATANOTIFICARICHIESTA,
                        praticaConclusa = ap.PRATICACONCLUSA,
                        dataPraticaConclusa = ap.DATAPRATICACONCLUSA,
                        dataVariazione = ap.DATAVARIAZIONE,
                        dataAggiornamento = ap.DATAAGGIORNAMENTO,
                        annullato = ap.ANNULLATO

                    };
                }
            }


            return apm;
        }


        public AttivazionePassaportiModel GetAttivazionePassaportiByIdConiugePassaporto(decimal idConiugePassaporto)
        {
            AttivazionePassaportiModel apm = new AttivazionePassaportiModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var cp = db.CONIUGEPASSAPORTO.Find(idConiugePassaporto);

                if (cp?.IDCONIUGEPASSAPORTO > 0)
                {
                    var ap = cp.ATTIVAZIONIPASSAPORTI;
                    apm = new AttivazionePassaportiModel()
                    {
                        idAttivazioniPassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                        idPassaporti = ap.IDPASSAPORTI,
                        notificaRichiesta = ap.NOTIFICARICHIESTA,
                        dataNotificaRichiesta = ap.DATANOTIFICARICHIESTA,
                        praticaConclusa = ap.PRATICACONCLUSA,
                        dataPraticaConclusa = ap.DATAPRATICACONCLUSA,
                        dataVariazione = ap.DATAVARIAZIONE,
                        dataAggiornamento = ap.DATAAGGIORNAMENTO,
                        annullato = ap.ANNULLATO

                    };
                }
            }



            return apm;
        }



        public AttivazionePassaportiModel GetAttivazionePassaportiByIdFiglioPassaporto(decimal idFiglioPassaporto)
        {
            AttivazionePassaportiModel apm = new AttivazionePassaportiModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var f = db.FIGLIPASSAPORTO.Find(idFiglioPassaporto);

                if (f?.IDFIGLIPASSAPORTO > 0)
                {
                    var ap = f.ATTIVAZIONIPASSAPORTI;
                    apm = new AttivazionePassaportiModel()
                    {
                        idAttivazioniPassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                        idPassaporti = ap.IDPASSAPORTI,
                        notificaRichiesta = ap.NOTIFICARICHIESTA,
                        dataNotificaRichiesta = ap.DATANOTIFICARICHIESTA,
                        praticaConclusa = ap.PRATICACONCLUSA,
                        dataPraticaConclusa = ap.DATAPRATICACONCLUSA,
                        dataVariazione = ap.DATAVARIAZIONE,
                        dataAggiornamento = ap.DATAAGGIORNAMENTO,
                        annullato = ap.ANNULLATO

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
                //item.Collection(a => a.CONIUGE).Load();
                var c = db.CONIUGE.Find(idConiuge);
                //ap.CONIUGE.Add(c);

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
                //item.Collection(a => a.FIGLI).Load();
                var f = db.FIGLI.Find(idFiglio);
                //ap.FIGLI.Add(f);

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
                IDPASSAPORTI = prm.idPassaporto,
                IDATTIVAZIONIPASSAPORTI = prm.idAttivazionePassaporti,
                INCLUDIPASSAPORTO = prm.includiPassaporto,
                DATAAGGIORNAMENTO = DateTime.Now,
                ANNULLATO = false,

            };

            var ap = db.ATTIVAZIONIPASSAPORTI.Find(prm.idAttivazionePassaporti);

            ap.PASSAPORTORICHIEDENTE.Add(pr);

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
                ANNULLATO = apm.annullato,
                IDFASEPASSAPORTI = apm.idFasePassaporti
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
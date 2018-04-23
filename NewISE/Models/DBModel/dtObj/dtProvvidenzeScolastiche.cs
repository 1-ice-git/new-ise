using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models.Tools;
using NewISE.Models.DBModel.Enum;
using NewISE.Models.ViewModel;
using NewISE.Models.ModelRest;
using System.Diagnostics;
using System.IO;
using NewISE.Models.Config;
using NewISE.Models.Config.s_admin;
using System.Data.Entity;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtProvvidenzeScolastiche : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public ProvvidenzeScolasticheModel GetProvvidenzeScolasticheByID(decimal idTrasfProvScolastiche)
        {
            ProvvidenzeScolasticheModel mcm = new ProvvidenzeScolasticheModel();


            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.PROVVIDENZESCOLASTICHE.Find(idTrasfProvScolastiche);

                if (mf != null && mf.IDTRASFPROVSCOLASTICHE > 0)
                {
                    mcm = new ProvvidenzeScolasticheModel()
                    {
                        idTrasfProvScolastiche = mf.IDTRASFPROVSCOLASTICHE,

                    };
                }
            }

            return mcm;
        }
        public List<VariazioneDocumentiModel> GetDocumentiPS(decimal idTrasfProvScolastiche, decimal idTipoDoc)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tep = db.PROVVIDENZESCOLASTICHE.Find(idTrasfProvScolastiche);
                var statoTrasferimento = tep.TRASFERIMENTO.IDSTATOTRASFERIMENTO;

                var latep = tep.ATTIVAZIONIPROVSCOLASTICHE.Where(a => (a.ATTIVARICHIESTA == true && a.NOTIFICARICHIESTA == true) || a.ANNULLATO == false).OrderBy(a => a.IDTRASFPROVSCOLASTICHE).ToList();


                if (latep?.Any() ?? false)
                {
                    foreach (var atep in latep)
                    {
                        var ld = atep.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == idTipoDoc).OrderByDescending(a => a.DATAINSERIMENTO).ToList();

                        bool modificabile = false;

                        if (statoTrasferimento == (decimal)EnumStatoTraferimento.Annullato)
                        {
                            modificabile = false;
                        }

                        foreach (var doc in ld)
                        {
                            var amf = new VariazioneDocumentiModel()
                            {
                                dataInserimento = doc.DATAINSERIMENTO,
                                estensione = doc.ESTENSIONE,
                                idDocumenti = doc.IDDOCUMENTO,
                                nomeDocumento = doc.NOMEDOCUMENTO,
                                Modificabile = modificabile,
                                //IdAttivazione = atep.IDATEPARTENZA,
                                DataAggiornamento = atep.DATAAGGIORNAMENTO,
                                fk_iddocumento = doc.FK_IDDOCUMENTO,
                                idStatoRecord = doc.IDSTATORECORD
                            };

                            //var amf = new DocumentiModel()
                            //{
                            //    dataInserimento = doc.DATAINSERIMENTO,
                            //    estensione = doc.ESTENSIONE,
                            //    idDocumenti = doc.IDDOCUMENTO,
                            //    nomeDocumento = doc.NOMEDOCUMENTO,
                            //    //DataAggiornamento = atep.DATAAGGIORNAMENTO,
                            //    fk_iddocumento = doc.FK_IDDOCUMENTO,
                            //    idStatoRecord = doc.IDSTATORECORD
                            //};
                            ldm.Add(amf);
                        }

                    }

                }
            }

            return ldm;

        }
        public ATTIVAZIONIPROVSCOLASTICHE CreaAttivitaPS(decimal idTrasfProvScolastiche, ModelDBISE db)
        {

            ATTIVAZIONIPROVSCOLASTICHE new_atep = new ATTIVAZIONIPROVSCOLASTICHE()
            {
                IDTRASFPROVSCOLASTICHE = idTrasfProvScolastiche,
                NOTIFICARICHIESTA = false,
                DATANOTIFICA = null,
                ATTIVARICHIESTA = false,
                DATAATTIVAZIONE = null,
                ANNULLATO = false,
                DATAAGGIORNAMENTO = DateTime.Now,
            };
            db.ATTIVAZIONIPROVSCOLASTICHE.Add(new_atep);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception(string.Format("Non è stato possibile creare una nuova attivazione per le provvidenze scolastiche."));
            }
            else
            {

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova attivazione per le provvidenze scolastiche.", "ATTIVAZIONIPROVSCOLASTICHE", db, new_atep.IDTRASFPROVSCOLASTICHE, new_atep.IDPROVSCOLASTICHE);
                
            }

            return new_atep;
        }
        public void SetDocumentoPS(ref DocumentiModel dm, decimal idTrasfProvScolastiche, ModelDBISE db, decimal idTipoDocumento)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                DOCUMENTI d = new DOCUMENTI();
                ATTIVAZIONIPROVSCOLASTICHE atep = new ATTIVAZIONIPROVSCOLASTICHE();

                dm.file.InputStream.CopyTo(ms);

                var tep = db.PROVVIDENZESCOLASTICHE.Find(idTrasfProvScolastiche);

                var latep =
                    tep.ATTIVAZIONIPROVSCOLASTICHE.Where(
                        a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == false && a.ATTIVARICHIESTA == false)
                        .OrderByDescending(a => a.IDPROVSCOLASTICHE).ToList();
                if (latep?.Any() ?? false)
                {
                    atep = latep.First();
                }
                else
                {
                    atep = this.CreaAttivitaPS(idTrasfProvScolastiche, db);
                }

                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = idTipoDocumento;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();
                d.MODIFICATO = false;
                d.FK_IDDOCUMENTO = null;
                d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;

                atep.DOCUMENTI.Add(d);

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (provvidenze scolastiche).", "Documenti", db, tep.IDTRASFPROVSCOLASTICHE, dm.idDocumenti);
                }
                else
                {
                    throw new Exception("Errore nella fase di inserimento del documento (provvidenze scolastiche).");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void DeleteDocumentoPS(decimal idDocumento)
        {
            PROVVIDENZESCOLASTICHE ps = new PROVVIDENZESCOLASTICHE();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var d = db.DOCUMENTI.Find(idDocumento);

                    if (d != null && d.IDDOCUMENTO > 0)
                    {
                        db.DOCUMENTI.Remove(d);

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception(string.Format("Non è stato possibile effettuare l'eliminazione del documento ({0}).", d.NOMEDOCUMENTO + d.ESTENSIONE));
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione di un documento (" + ((EnumTipoDoc)d.IDTIPODOCUMENTO).ToString() + ").", "Documenti", db, ps.IDTRASFPROVSCOLASTICHE, d.IDDOCUMENTO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public void SituazionePRovvidenzeScolastiche(decimal idTrasfProvScolastiche,
                                       out bool richiestaPS,
                                       out bool attivazionePS,
                                       out bool DocProvvidenzeScolastiche,
                                       out bool trasfAnnullato)
        {

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    richiestaPS = false;
                    attivazionePS = false;
                    DocProvvidenzeScolastiche = false;
                    trasfAnnullato = false;

                    var tps = db.PROVVIDENZESCOLASTICHE.Find(idTrasfProvScolastiche);

                    var idStatoTrasferimento = tps.TRASFERIMENTO.IDSTATOTRASFERIMENTO;
                    if (idStatoTrasferimento == (decimal)EnumStatoTraferimento.Annullato)
                    {
                        trasfAnnullato = true;
                    }

                    if (tps == null)
                    {
                                               
                        // Documenti in attesa di approvazione disabilito il tasto Attiva Richiesta



                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore - Impossibile creare i record su Provvidenze Scolastiche.");
                        }
                        


                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public void AttivaRichiestaProvvidenzeScolastiche(decimal idAttivitaTrasportoEffettiPartenza)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var atep = db.ATTIVITATEPARTENZA.Find(idAttivitaTrasportoEffettiPartenza);
                    if (atep?.IDATEPARTENZA > 0)
                    {
                        if (atep.RICHIESTATRASPORTOEFFETTI == true)
                        {
                            atep.ATTIVAZIONETRASPORTOEFFETTI = true;
                            atep.DATAATTIVAZIONETE = DateTime.Now;
                            atep.DATAAGGIORNAMENTO = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore: Impossibile completare l'attivazione del trasporto effetti in partenza.");
                            }
                            else
                            {
                                #region ciclo attivazione documenti TE
                                var ldte = atep.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                                foreach (var dte in ldte)
                                {
                                    dte.IDSTATORECORD = (decimal)EnumStatoRecord.Attivato;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante il ciclo di attivazione trasporto effetti (attiva documenti)");
                                    }
                                }
                                #endregion



                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Attivazione trasporto effetti in partenza.", "ATTIVITATEPARTENZA", db,
                                    atep.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO, atep.IDATEPARTENZA);
                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.ModificaInCompletatoCalendarioEvento(atep.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaTrasportoEffettiPartenza, db);
                                }


                                var messaggioAttiva = Resources.msgEmail.MessaggioAttivazioneTrasportoEffettiPartenza;
                                var oggettoAttiva = Resources.msgEmail.OggettoAttivazioneTrasportoEffettiPartenza;

                                EmailTrasferimento.EmailAttiva(atep.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO,
                                                    oggettoAttiva,
                                                    messaggioAttiva,
                                                    db);
                                //this.EmailAttivaRichiestaTEPartenza(atep.IDATEPARTENZA, db);

                            }
                        }
                    }

                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public ATTIVAZIONIPROVSCOLASTICHE GetUltimaAttivazioneProvvScolastiche(decimal idTrasfProvScolastiche)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                ATTIVAZIONIPROVSCOLASTICHE atps = new ATTIVAZIONIPROVSCOLASTICHE();


                var tps = db.PROVVIDENZESCOLASTICHE.Find(idTrasfProvScolastiche);

                if (tps != null && tps.IDTRASFPROVSCOLASTICHE > 0)
                {

                    var latps = tps.ATTIVAZIONIPROVSCOLASTICHE
                            .Where(a => a.ANNULLATO == false)
                            .OrderByDescending(a => a.IDPROVSCOLASTICHE).ToList();
                    if (latps?.Any() ?? false)
                    {
                        atps = latps.First();
                    }
                    else
                    {
                        //se non esiste una attivazione
                        //ne creo una 
                        ATTIVAZIONIPROVSCOLASTICHE new_atps = new ATTIVAZIONIPROVSCOLASTICHE()
                        {
                            IDTRASFPROVSCOLASTICHE = idTrasfProvScolastiche,
                            DATANOTIFICA = null,
                            ATTIVARICHIESTA = false,
                            DATAATTIVAZIONE = null,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            ANNULLATO = false

                            

                        };


                        db.ATTIVAZIONIPROVSCOLASTICHE.Add(new_atps);

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore nella fase di creazione dell'attivita provvidenze scolastiche.");
                        }
                        else
                        {
                            //creo la riga relativa alla rinuncia
                            //var rtep = this.CreaRinunciaTEPartenza(new_atep.IDATEPARTENZA, db);

                            ////leggo la percentuale e la associo
                            //var PercentualeAnticipoTE = this.GetPercentualeAnticipoTEPartenza(idTEPartenza, (decimal)EnumTipoAnticipoTE.Partenza);
                            //if (PercentualeAnticipoTE.IDPERCANTICIPOTM > 0)
                            //{
                            //    this.Associa_TEpartenza_perceAnticipoTE(idTEPartenza, PercentualeAnticipoTE.IDPERCANTICIPOTM, db);

                            //    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                            //    "Inserimento attivita trasporto effetti partenza.", "ATTIVITATEPARTENZA", db, idTEPartenza,
                            //    new_atep.IDATEPARTENZA);
                            //}
                        }

                        atps = new_atps;
                    }
                }

                return atps;
            }

        }

        public void NotificaRichiestaPS(decimal idProvScolastiche)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var atps = db.ATTIVAZIONIPROVSCOLASTICHE.Find(idProvScolastiche);
                        atps.NOTIFICARICHIESTA = true;
                        atps.DATAATTIVAZIONE = DateTime.Now;
                        atps.DATAAGGIORNAMENTO = DateTime.Now;


                        var i = db.SaveChanges();
                        if (i <= 0)
                        {
                            throw new Exception("Errore nella fase d'inserimento per la richiesta provvidenze scolastiche.");
                        }
                        

                        #region ciclo attivazione documenti PS
                        var ldps = atps.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                        foreach (var dps in ldps)
                        {
                            dps.IDSTATORECORD = (decimal)EnumStatoRecord.Da_Attivare;
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore durante il ciclo di attivazione provvidenze scolastiche");
                            }
                        }
                        #endregion


                        EmailTrasferimento.EmailNotifica(EnumChiamante.ProvvidenzeScolastiche,
                                                        atps.PROVVIDENZESCOLASTICHE.TRASFERIMENTO.IDTRASFERIMENTO,
                                                        Resources.msgEmail.OggettoNotificaProvvidenzeScolastiche,
                                                        Resources.msgEmail.MessaggioNotificaProvvidenzeScolastiche,
                                                        db);


                        using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                        {
                            CalendarioEventiModel cem = new CalendarioEventiModel()
                            {
                                idFunzioneEventi = EnumFunzioniEventi.RichiestaProvvidenzeScolastiche,
                                idTrasferimento = atps.PROVVIDENZESCOLASTICHE.IDTRASFPROVSCOLASTICHE,
                                DataInizioEvento = DateTime.Now.Date,
                                DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaProvvidenzeScolastiche)).Date,
                            };

                            dtce.InsertCalendarioEvento(ref cem, db);

                        }


                        db.Database.CurrentTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        db.Database.CurrentTransaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
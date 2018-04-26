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
            var NumAttivazioni = this.GetNumAttivazioniProvvidenzeScolastiche(idTrasfProvScolastiche);
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
                                       out decimal NumAttivazioni,
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
                        PROVVIDENZESCOLASTICHE new_tps = new PROVVIDENZESCOLASTICHE()
                        {
                            IDTRASFPROVSCOLASTICHE = idTrasfProvScolastiche
                        };
                        db.PROVVIDENZESCOLASTICHE.Add(new_tps);

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore - Impossibile creare i record su Provvidenze Scolastiche.");
                        }

                        tps = new_tps;
                    }

                    ATTIVAZIONIPROVSCOLASTICHE last_atps = new ATTIVAZIONIPROVSCOLASTICHE();

                    var latps = tps.ATTIVAZIONIPROVSCOLASTICHE
                                .Where(a => a.ANNULLATO == false || (a.NOTIFICARICHIESTA && a.ATTIVARICHIESTA))
                                .OrderByDescending(a => a.IDPROVSCOLASTICHE).ToList();

                    if (latps?.Any() ?? false)
                    {
                        //se esiste verifica se ci sono elementi associati

                        //imposta l'ultima valida
                        last_atps = latps.First();

                        //verifica se è stata richiesta
                        if (last_atps.NOTIFICARICHIESTA && last_atps.ATTIVARICHIESTA == false)
                        {
                            richiestaPS = true;
                        }
                        //verifica se è stata attivata
                        if (last_atps.NOTIFICARICHIESTA && last_atps.ATTIVARICHIESTA)
                        {
                            attivazionePS = true;
                        }

                        foreach (var atps in latps)
                        {
                            //documenti provvidenze scolastiche
                            var ldc = atps.DOCUMENTI.Where(a => (a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Provvidenze_Scolastiche)).ToList();
                            if (ldc?.Any() ?? false)
                            {
                                DocProvvidenzeScolastiche = true;
                            }

                            
                        }

                    }
                    else
                    {
                        last_atps = this.GetUltimaAttivazioneProvvScolastiche(idTrasfProvScolastiche);
                        
                    }


                    NumAttivazioni = GetNumAttivazioniProvvidenzeScolastiche(idTrasfProvScolastiche);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public decimal GetNumAttivazioniProvvidenzeScolastiche(decimal idTrasfProvScolastiche)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var NumAttivazioni = 0;
                NumAttivazioni = db.PROVVIDENZESCOLASTICHE.Find(idTrasfProvScolastiche).ATTIVAZIONIPROVSCOLASTICHE
                                    .Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == true)
                                    .OrderByDescending(a => a.IDPROVSCOLASTICHE).Count();
                return NumAttivazioni;
            }
        }

        public void AttivaRichiestaProvvidenzeScolastiche(decimal idAttivitaProvvidenzeScolastiche)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var atps = db.ATTIVAZIONIPROVSCOLASTICHE.Find(idAttivitaProvvidenzeScolastiche);
                    if (atps?.IDPROVSCOLASTICHE > 0)
                    {
                        if (atps.NOTIFICARICHIESTA == true)
                        {
                            atps.ATTIVARICHIESTA = true;
                            atps.DATAATTIVAZIONE = DateTime.Now;
                            atps.DATAAGGIORNAMENTO = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore: Impossibile completare l'attivazione delle provvidenze scolastiche.");
                            }
                            else
                            {
                                #region ciclo attivazione documenti PS
                                var ldte = atps.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                                foreach (var dte in ldte)
                                {
                                    dte.IDSTATORECORD = (decimal)EnumStatoRecord.Attivato;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante il ciclo di attivazione provvidenze scolastiche (attiva documenti)");
                                    }
                                }
                                #endregion



                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Attivazione provvidenze scolastiche.", "ATTIVAZIONIPROVSCOLASTICHE", db,
                                    atps.PROVVIDENZESCOLASTICHE.TRASFERIMENTO.IDTRASFERIMENTO, atps.IDPROVSCOLASTICHE);
                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.ModificaInCompletatoCalendarioEvento(atps.PROVVIDENZESCOLASTICHE.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaProvvidenzeScolastiche, db);
                                }


                                var messaggioAttiva = Resources.msgEmail.MessaggioAttivazioneProvvidenzeScolastiche;
                                var oggettoAttiva = Resources.msgEmail.OggettoAttivazioneProvvidenzeScolastiche;

                                EmailTrasferimento.EmailAttiva(atps.PROVVIDENZESCOLASTICHE.TRASFERIMENTO.IDTRASFERIMENTO,
                                                    oggettoAttiva,
                                                    messaggioAttiva,
                                                    db);
                               

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
                        using (dtDipendenti dtd = new dtDipendenti())
                        {
                            


                            var atps = db.ATTIVAZIONIPROVSCOLASTICHE.Find(idProvScolastiche);
                        atps.NOTIFICARICHIESTA = true;
                        atps.DATAATTIVAZIONE = DateTime.Now;
                        atps.DATAAGGIORNAMENTO = DateTime.Now;

                            var t = atps.PROVVIDENZESCOLASTICHE.TRASFERIMENTO;

                            var dip = dtd.GetDipendenteByID(t.IDDIPENDENTE);




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
                                                        string.Format(Resources.msgEmail.MessaggioNotificaProvvidenzeScolastiche, dip.cognome + " " + dip.nome + " (" + dip.matricola + ")"),
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

        public void AnnullaRichiestaProvvidenzeScolastiche(decimal idAttivitaProvvidenzeScolastiche, string msg)
        {

            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    
                    var atep_Old = db.ATTIVAZIONIPROVSCOLASTICHE.Find(idAttivitaProvvidenzeScolastiche);

                    if (atep_Old?.IDPROVSCOLASTICHE > 0)
                    {
                        if (atep_Old.NOTIFICARICHIESTA == true && atep_Old.ATTIVARICHIESTA == false && atep_Old.ANNULLATO == false)
                        {
                            atep_Old.ANNULLATO = true;
                            atep_Old.DATAAGGIORNAMENTO = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore - Impossibile annullare la notifica della richiesta provvidenze scolastiche.");
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Annullamento della riga per il ciclo di attivazione delle provvidenze scolastiche",
                                    "PROVVIDENZESCOLASTICHE", db, atep_Old.PROVVIDENZESCOLASTICHE.TRASFERIMENTO.IDTRASFERIMENTO,
                                    atep_Old.IDPROVSCOLASTICHE);

                                var idTrasferimento = atep_Old.PROVVIDENZESCOLASTICHE.TRASFERIMENTO.IDTRASFERIMENTO;
                                
                                ATTIVAZIONIPROVSCOLASTICHE atep_New = new ATTIVAZIONIPROVSCOLASTICHE()
                                {
                                    IDTRASFPROVSCOLASTICHE = atep_Old.IDTRASFPROVSCOLASTICHE,
                                    NOTIFICARICHIESTA = false,
                                    
                                    ATTIVARICHIESTA = false,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    ANNULLATO = false
                                };

                                db.ATTIVAZIONIPROVSCOLASTICHE.Add(atep_New);

                                int j = db.SaveChanges();

                                if (j <= 0)
                                {
                                    throw new Exception("Errore - Impossibile creare il nuovo ciclo di attivazione per le provvidenze scolastiche.");
                                }
                                else
                                {
                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                        "Inserimento di una nuova riga per il ciclo di attivazione relativo alle provvidenze scolastiche.",
                                        "PROVVIDENZESCOLASTICHE", db, atep_New.PROVVIDENZESCOLASTICHE.TRASFERIMENTO.IDTRASFERIMENTO,
                                        atep_New.IDTRASFPROVSCOLASTICHE);

                                    #region ricrea rinunciaTE
                                    //var rtep_old = this.GetRinunciaTEPartenza(atep_Old.IDATEPARTENZA, db);
                                    //RINUNCIA_TE_P rtep_new = new RINUNCIA_TE_P()
                                    //{
                                    //    IDATEPARTENZA = atep_New.IDATEPARTENZA,
                                    //    RINUNCIATE = rtep_old.rinunciaTE,
                                    //    DATAAGGIORNAMENTO = DateTime.Now,
                                    //};
                                    //db.RINUNCIA_TE_P.Add(rtep_new);

                                    //if (db.SaveChanges() <= 0)
                                    //{
                                    //    throw new Exception(string.Format("Non è stato possibile creare una nuova rinuncia trasporto effetti partenza durante il ciclo di annullamento."));
                                    //}
                                    //else
                                    //{
                                    //    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova rinuncia trasporto effetti partenza.", "RINUNCIA_TE_P", db, rtep_new.ATTIVITATEPARTENZA.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO, rtep_new.IDATEPARTENZA);
                                    //}

                                    #endregion


                                    #region documenti
                                    var ldoc_Old =
                                        atep_Old.DOCUMENTI.Where(
                                            a => a.MODIFICATO == false)
                                            .OrderBy(a => a.DATAINSERIMENTO);

                                    if (ldoc_Old?.Any() ?? false)
                                    {
                                        foreach (var doc_Old in ldoc_Old)
                                        {
                                            DOCUMENTI doc_New = new DOCUMENTI()
                                            {
                                                IDTIPODOCUMENTO = doc_Old.IDTIPODOCUMENTO,
                                                NOMEDOCUMENTO = doc_Old.NOMEDOCUMENTO,
                                                ESTENSIONE = doc_Old.ESTENSIONE,
                                                FILEDOCUMENTO = doc_Old.FILEDOCUMENTO,
                                                DATAINSERIMENTO = doc_Old.DATAINSERIMENTO,
                                                MODIFICATO = doc_Old.MODIFICATO,
                                                FK_IDDOCUMENTO = doc_Old.FK_IDDOCUMENTO,
                                                IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                            };

                                            atep_New.DOCUMENTI.Add(doc_New);
                                            doc_Old.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                            int y = db.SaveChanges();

                                            if (y <= 0)
                                            {
                                                throw new Exception("Errore - Impossibile associare il documento per le provvidenze scolastiche. (" + doc_New.NOMEDOCUMENTO + ")");
                                            }
                                            else
                                            {
                                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                    "Inserimento di una nuova riga per il documento relativo alle provvidenze scolastiche.",
                                                    "DOCUMENTI", db,
                                                    atep_New.PROVVIDENZESCOLASTICHE.TRASFERIMENTO.IDTRASFERIMENTO,
                                                    doc_New.IDDOCUMENTO);
                                            }

                                        }


                                    }
                                    #endregion

                                    EmailTrasferimento.EmailAnnulla(idTrasferimento,
                                                                    Resources.msgEmail.OggettoAnnullaRichiestaProvvidenzeScolastiche,
                                                                    msg,
                                                                    db);
                                    
                                    using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                    {
                                        dtce.AnnullaMessaggioEvento(idTrasferimento, EnumFunzioniEventi.RichiestaProvvidenzeScolastiche, db);
                                    }
                                }

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

    }
}
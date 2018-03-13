using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;
using NewISE.Models.ModelRest;
using System.Diagnostics;
using System.IO;
using NewISE.Models.Config;
using NewISE.Models.Config.s_admin;
using System.Data.Entity;


namespace NewISE.Models.DBModel.dtObj
{
    public enum EnumTipoAnticipoTE
    {
        Partenza = 1,
        Rientro = 2
    }


    public class dtTrasportoEffetti : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void PreSetTrasportoEffetti(decimal idTrasferimento, ModelDBISE db)
        {
            TEPARTENZA tep = new TEPARTENZA();
            TERIENTRO ter = new TERIENTRO();

            var t = db.TRASFERIMENTO.Find(idTrasferimento);

            tep = new TEPARTENZA()
            {
                IDTEPARTENZA = t.IDTRASFERIMENTO
            };

            db.TEPARTENZA.Add(tep);

            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Errore nell'inserimento della riga di trasporto effetti fase partenza.");
            }
            else
            {
                ter = new TERIENTRO()
                {
                    IDTERIENTRO = t.IDTRASFERIMENTO
                };

                db.TERIENTRO.Add(ter);

                int j = db.SaveChanges();

                if (j <= 0)
                {
                    throw new Exception("Errore nell'inserimento della riga di trasporto effetti fase rientro.");
                }

            }


        }

        public TrasportoEffettiPartenzaModel GetTEPartenzaByID(decimal idTrasportoEffettiPartenza)
        {
            TrasportoEffettiPartenzaModel tepm = new TrasportoEffettiPartenzaModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tep = db.TEPARTENZA.Find(idTrasportoEffettiPartenza);

                if (tep != null && tep.IDTEPARTENZA > 0)
                {
                    tepm = new TrasportoEffettiPartenzaModel()
                    {
                        idTEPartenza = tep.IDTEPARTENZA
                    };
                }

            }

            return tepm;
        }

        public TERientroModel GetTERientroByID(decimal idTrasportoEffettiRientro)
        {
            TERientroModel term = new TERientroModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var ter = db.TERIENTRO.Find(idTrasportoEffettiRientro);

                if (ter != null && ter.IDTERIENTRO > 0)
                {
                    term = new TERientroModel()
                    {
                        idTERientro = ter.IDTERIENTRO
                    };
                }

            }

            return term;
        }


        public decimal GetNumDocumentiTEPartenza(decimal idTrasportoEffettiPartenza, EnumTipoDoc tipoDocumento)

        {
            using (ModelDBISE db = new ModelDBISE())
            {
                decimal nDoc = 0;

                var tep = db.TEPARTENZA.Find(idTrasportoEffettiPartenza);
                var latep = tep.ATTIVITATEPARTENZA.Where(a => a.ANNULLATO == false).ToList();

                if (latep?.Any() ?? false)
                {
                    foreach (var atep in latep)
                    {
                        var ld = atep.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipoDocumento).ToList();

                        nDoc = nDoc + ld.Count();
                    }
                }
                return nDoc;
            }
        }


        public void SituazioneTEPartenza(decimal idTrasportoEffettiPartenza,
                                        out bool richiestaTE,
                                        out bool attivazioneTE,
                                        out bool DocContributo,
                                        out bool DocAttestazione,
                                        out decimal NumAttivazioni,
                                        out bool trasfAnnullato)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    richiestaTE = false;
                    attivazioneTE = false;
                    DocContributo = false;
                    DocAttestazione = false;
                    trasfAnnullato = false;

                    var tep = db.TEPARTENZA.Find(idTrasportoEffettiPartenza);

                    var idStatoTrasferimento = tep.TRASFERIMENTO.IDSTATOTRASFERIMENTO;
                    if (idStatoTrasferimento == (decimal)EnumStatoTraferimento.Annullato)
                    {
                        trasfAnnullato = true;
                    }

                    if (tep == null)
                    {
                        TEPARTENZA new_tep = new TEPARTENZA()
                        {
                            IDTEPARTENZA = idTrasportoEffettiPartenza
                        };
                        db.TEPARTENZA.Add(new_tep);

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore - Impossibile creare i record su TEPartenza.");
                        }
                        tep = new_tep;
                    }
                    //elenco attivazioni valide
                    var latep = tep.ATTIVITATEPARTENZA
                                .Where(a => a.ANNULLATO == false || (a.RICHIESTATRASPORTOEFFETTI && a.ATTIVAZIONETRASPORTOEFFETTI))
                                .OrderByDescending(a => a.IDATEPARTENZA).ToList();

                    if (latep?.Any() ?? false)
                    {
                        //se esiste verifica se ci sono elementi associati

                        //imposta l'ultima valida
                        var last_atep = latep.First();

                        //verifica se è stata richiesta
                        if (last_atep.RICHIESTATRASPORTOEFFETTI && last_atep.ATTIVAZIONETRASPORTOEFFETTI == false)
                        {
                            richiestaTE = true;
                        }
                        //verifica se è stata attivata
                        if (last_atep.RICHIESTATRASPORTOEFFETTI && last_atep.ATTIVAZIONETRASPORTOEFFETTI)
                        {
                            attivazioneTE = true;
                        }

                        foreach (var atep in latep)
                        {
                            //documenti contributo
                            var ldc = atep.DOCUMENTI.Where(a => (a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Contributo_Fisso_Omnicomprensivo)).ToList();
                            if (ldc?.Any() ?? false)
                            {
                                DocContributo = true;
                            }

                            //documenti attestazione
                            var lda = atep.DOCUMENTI.Where(a => (a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Attestazione_Trasloco)).ToList();
                            if (lda?.Any() ?? false)
                            {
                                DocAttestazione = true;
                            }
                        }

                    }
                    NumAttivazioni = GetNumAttivazioniTEPartenza(idTrasportoEffettiPartenza);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ATTIVITATEPARTENZA GetUltimaAttivazioneTEPartenza(decimal idTEPartenza)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                ATTIVITATEPARTENZA atep = new ATTIVITATEPARTENZA();

                var tep = db.TEPARTENZA.Find(idTEPartenza);

                if (tep != null && tep.IDTEPARTENZA > 0)
                {

                    var latep = tep.ATTIVITATEPARTENZA
                            .Where(a => a.ANNULLATO == false)
                            .OrderByDescending(a => a.IDATEPARTENZA).ToList();
                    if (latep?.Any() ?? false)
                    {
                        atep = latep.First();
                    }
                    else
                    {
                        //se non esiste una attivazione
                        //ne creo una 
                        ATTIVITATEPARTENZA new_atep = new ATTIVITATEPARTENZA()
                        {
                            IDTEPARTENZA = idTEPartenza,
                            IDANTICIPOSALDOTE = (decimal)EnumTipoAnticipoSaldoTE.Anticipo,
                            RICHIESTATRASPORTOEFFETTI = false,
                            DATARICHIESTATRASPORTOEFFETTI = null,
                            ATTIVAZIONETRASPORTOEFFETTI = false,
                            DATAATTIVAZIONETE = null,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            ANNULLATO = false
                        };
                        db.ATTIVITATEPARTENZA.Add(new_atep);

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore nella fase di creazione dell'attivita trasporto effetti.");
                        }
                        else
                        {
                            var PercentualeAnticipoTE = this.GetPercentualeAnticipoTEPartenza(idTEPartenza, (decimal)EnumTipoAnticipoTE.Partenza);
                            if (PercentualeAnticipoTE.IDPERCANTICIPOTM > 0)
                            {
                                this.Associa_TEpartenza_perceAnticipoTE(idTEPartenza, PercentualeAnticipoTE.IDPERCANTICIPOTM, db);

                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                "Inserimento attivita trasporto effetti partenza.", "ATTIVITATEPARTENZA", db, idTEPartenza,
                                new_atep.IDATEPARTENZA);
                            }
                        }

                        atep = new_atep;
                    }
                }

                return atep;
            }

        }

        public void NotificaRichiestaTEPartenza(decimal idAttivitaTEPartenza)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var atep = db.ATTIVITATEPARTENZA.Find(idAttivitaTEPartenza);
                        atep.RICHIESTATRASPORTOEFFETTI = true;
                        atep.DATARICHIESTATRASPORTOEFFETTI = DateTime.Now;
                        atep.DATAAGGIORNAMENTO = DateTime.Now;

                        var i = db.SaveChanges();
                        if (i <= 0)
                        {
                            throw new Exception("Errore nella fase d'inserimento per la richiesta attivazione trasporto effetti partenza.");
                        }
                        else
                        {
                            this.EmailNotificaRichiestaTEPartenza(idAttivitaTEPartenza, db);

                            using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                            {
                                CalendarioEventiModel cem = new CalendarioEventiModel()
                                {
                                    idFunzioneEventi = EnumFunzioniEventi.RichiestaTrasportoEffettiPartenza,
                                    idTrasferimento = atep.TEPARTENZA.IDTEPARTENZA,
                                    DataInizioEvento = DateTime.Now.Date,
                                    DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaTrasportoEffettiPartenza)).Date,
                                };

                                dtce.InsertCalendarioEvento(ref cem, db);

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
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void EmailNotificaRichiestaTEPartenza(decimal idAttivitaTEPartenza, ModelDBISE db)
        {
            TEPARTENZA tep = new TEPARTENZA();
            AccountModel am = new AccountModel();
            Mittente mittente = new Mittente();
            Destinatario to = new Destinatario();
            Destinatario cc = new Destinatario();
            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();


            try
            {
                am = Utility.UtenteAutorizzato();
                mittente.Nominativo = am.nominativo;
                mittente.EmailMittente = am.eMail;

                var atep = db.ATTIVITATEPARTENZA.Find(idAttivitaTEPartenza);

                tep = atep.TEPARTENZA;

                if (tep?.IDTEPARTENZA > 0)
                {
                    TRASFERIMENTO tr = tep.TRASFERIMENTO;
                    DIPENDENTI d = tr.DIPENDENTI;

                    UFFICI u = tr.UFFICI;

                    using (dtUtentiAutorizzati dtua = new dtUtentiAutorizzati())
                    {
                        using (GestioneEmail gmail = new GestioneEmail())
                        {
                            using (ModelloMsgMail msgMail = new ModelloMsgMail())
                            {

                                cc = new Destinatario()
                                {
                                    Nominativo = am.nominativo,
                                    EmailDestinatario = am.eMail
                                };

                                msgMail.mittente = mittente;
                                msgMail.cc.Add(cc);

                                luam.AddRange(dtua.GetUtentiByRuolo(EnumRuoloAccesso.Amministratore).ToList());

                                foreach (var uam in luam)
                                {
                                    var amministratore = db.DIPENDENTI.Find(uam.idDipendente);
                                    if (amministratore != null && amministratore.IDDIPENDENTE > 0)
                                    {
                                        to = new Destinatario()
                                        {
                                            Nominativo = amministratore.COGNOME + " " + amministratore.NOME,
                                            EmailDestinatario = amministratore.EMAIL
                                        };

                                        msgMail.destinatario.Add(to);
                                    }


                                }
                                msgMail.oggetto = Resources.msgEmail.OggettoNotificaTrasportoEffettiPartenza;
                                msgMail.corpoMsg =
                                        string.Format(
                                            Resources.msgEmail.MessaggioNotificaTrasportoEffettiPartenza,
                                            d.COGNOME + " " + d.NOME + " (" + d.MATRICOLA + ")",
                                            tr.DATAPARTENZA.ToLongDateString(),
                                            u.DESCRIZIONEUFFICIO + " (" + u.CODICEUFFICIO + ")");

                                gmail.sendMail(msgMail);

                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public List<VariazioneDocumentiModel> GetDocumentiTEPartenza(decimal idTrasportoEffettiPartenza, decimal idTipoDoc)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tep = db.TEPARTENZA.Find(idTrasportoEffettiPartenza);
                var statoTrasferimento = tep.TRASFERIMENTO.IDSTATOTRASFERIMENTO;

                var latep = tep.ATTIVITATEPARTENZA.Where(a => (a.ATTIVAZIONETRASPORTOEFFETTI == true && a.RICHIESTATRASPORTOEFFETTI == true) || a.ANNULLATO == false).OrderBy(a => a.IDATEPARTENZA).ToList();


                if (latep?.Any() ?? false)
                {
                    foreach (var atep in latep)
                    {
                        var ld = atep.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == idTipoDoc).OrderByDescending(a => a.DATAINSERIMENTO).ToList();

                        var rtep = atep.RINUNCIA_TE_P;

                        bool modificabile = false;
                        if (atep.ATTIVAZIONETRASPORTOEFFETTI == false && atep.RICHIESTATRASPORTOEFFETTI == false && rtep.RINUNCIATE==false)
                        {
                            modificabile = true;
                        }

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
                                IdAttivazione = atep.IDATEPARTENZA,
                                DataAggiornamento = atep.DATAAGGIORNAMENTO
                            };

                            ldm.Add(amf);
                        }

                    }

                }
            }

            return ldm;

        }
        public decimal GetNumAttivazioniTEPartenza(decimal idTrasportoEffettiPartenza)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var NumAttivazioni = 0;
                NumAttivazioni = db.TEPARTENZA.Find(idTrasportoEffettiPartenza).ATTIVITATEPARTENZA
                                    .Where(a => a.ANNULLATO == false && a.RICHIESTATRASPORTOEFFETTI == true)
                                    .OrderByDescending(a => a.IDATEPARTENZA).Count();
                return NumAttivazioni;
            }
        }

        public void SetDocumentoTEPartenza(ref DocumentiModel dm, decimal idTrasportoEffettiPartenza, ModelDBISE db, decimal idTipoDocumento)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                DOCUMENTI d = new DOCUMENTI();
                ATTIVITATEPARTENZA atep = new ATTIVITATEPARTENZA();

                dm.file.InputStream.CopyTo(ms);

                var tep = db.TEPARTENZA.Find(idTrasportoEffettiPartenza);

                var latep =
                    tep.ATTIVITATEPARTENZA.Where(
                        a => a.ANNULLATO == false && a.ATTIVAZIONETRASPORTOEFFETTI == false && a.RICHIESTATRASPORTOEFFETTI == false)
                        .OrderByDescending(a => a.IDTEPARTENZA).ToList();
                if (latep?.Any() ?? false)
                {
                    atep = latep.First();
                }
                else
                {
                    atep = this.CreaAttivitaTEPartenza(idTrasportoEffettiPartenza, db);
                }

                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = idTipoDocumento;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();
                d.MODIFICATO = false;
                d.FK_IDDOCUMENTO = null;

                atep.DOCUMENTI.Add(d);

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (trasporto effetti partenza).", "Documenti", db, tep.IDTEPARTENZA, dm.idDocumenti);
                }
                else
                {
                    throw new Exception("Errore nella fase di inserimento del documento (trasporto effetti partenza).");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ATTIVITATEPARTENZA CreaAttivitaTEPartenza(decimal idTEPartenza, ModelDBISE db)
        {
            var NumAttivazioni = this.GetNumAttivazioniTEPartenza(idTEPartenza);
            ATTIVITATEPARTENZA new_atep = new ATTIVITATEPARTENZA()
            {
                IDTEPARTENZA = idTEPartenza,
                IDANTICIPOSALDOTE = (NumAttivazioni == 0) ? ((decimal)EnumTipoAnticipoSaldoTE.Anticipo) : ((decimal)EnumTipoAnticipoSaldoTE.Saldo),
                RICHIESTATRASPORTOEFFETTI = false,
                DATARICHIESTATRASPORTOEFFETTI = null,
                ATTIVAZIONETRASPORTOEFFETTI = false,
                DATAATTIVAZIONETE = null,
                ANNULLATO = false,
                DATAAGGIORNAMENTO = DateTime.Now,
            };
            db.ATTIVITATEPARTENZA.Add(new_atep);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception(string.Format("Non è stato possibile creare una nuova attivazione per il trasporto effetti (partenza)."));
            }
            else
            {

                var PercentualeAnticipoTE = this.GetPercentualeAnticipoTEPartenza(idTEPartenza, (decimal)EnumTipoAnticipoTE.Partenza);
                if (PercentualeAnticipoTE.IDPERCANTICIPOTM > 0)
                {
                    this.Associa_TEpartenza_perceAnticipoTE(idTEPartenza, PercentualeAnticipoTE.IDPERCANTICIPOTM, db);

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova attivazione trasporto effetti (partenza).", "ATTIVITATEPARTENZA", db, new_atep.IDTEPARTENZA, new_atep.IDATEPARTENZA);
                }
            }

            return new_atep;
        }

        public void DeleteDocumentoTE(decimal idDocumento)
        {
            TEPARTENZA tep = new TEPARTENZA();

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
                            Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione di un documento (" + ((EnumTipoDoc)d.IDTIPODOCUMENTO).ToString() + ").", "Documenti", db, tep.IDTEPARTENZA, d.IDDOCUMENTO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void AnnullaRichiestaTrasportoEffetti(decimal idAttivitaTrasportoEffetti, string msg)
        {

            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var atep_Old = db.ATTIVITATEPARTENZA.Find(idAttivitaTrasportoEffetti);

                    if (atep_Old?.IDATEPARTENZA > 0)
                    {
                        if (atep_Old.RICHIESTATRASPORTOEFFETTI == true && atep_Old.ATTIVAZIONETRASPORTOEFFETTI == false && atep_Old.ANNULLATO == false)
                        {
                            atep_Old.ANNULLATO = true;
                            atep_Old.DATAAGGIORNAMENTO = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore - Impossibile annullare la notifica della richiesta trasporto effetti (partenza).");
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Annullamento della riga per il ciclo di attivazione del trasporto effetti (partenza)",
                                    "ATTIVITATEPARTENZA", db, atep_Old.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO,
                                    atep_Old.IDATEPARTENZA);

                                var idTrasferimento = atep_Old.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO;

                                ATTIVITATEPARTENZA atep_New = new ATTIVITATEPARTENZA()
                                {
                                    IDTEPARTENZA = atep_Old.IDTEPARTENZA,
                                    RICHIESTATRASPORTOEFFETTI = false,
                                    IDANTICIPOSALDOTE = atep_Old.IDANTICIPOSALDOTE,
                                    ATTIVAZIONETRASPORTOEFFETTI = false,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    ANNULLATO = false
                                };

                                db.ATTIVITATEPARTENZA.Add(atep_New);

                                int j = db.SaveChanges();

                                if (j <= 0)
                                {
                                    throw new Exception("Errore - Impossibile creare il nuovo ciclo di attivazione per il trasporto effetti (partenza).");
                                }
                                else
                                {
                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                        "Inserimento di una nuova riga per il ciclo di attivazione relativo al trasporto effetti (partenza).",
                                        "ATTIVITATEPARTENZA", db, atep_New.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO,
                                        atep_New.IDATEPARTENZA);


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
                                                FK_IDDOCUMENTO = doc_Old.FK_IDDOCUMENTO
                                            };

                                            atep_New.DOCUMENTI.Add(doc_New);

                                            int y = db.SaveChanges();

                                            if (y <= 0)
                                            {
                                                throw new Exception("Errore - Impossibile associare il documento per il trasporto effetti in partenza. (" + doc_New.NOMEDOCUMENTO + ")");
                                            }
                                            else
                                            {
                                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                    "Inserimento di una nuova riga per il documento relativo al trasporto effetti in partenza.",
                                                    "DOCUMENTI", db,
                                                    atep_New.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO,
                                                    doc_New.IDDOCUMENTO);
                                            }

                                        }


                                    }
                                    #endregion

                                    EmailTrasferimento.EmailAnnulla(idTrasferimento,
                                                                    Resources.msgEmail.OggettoAnnullaRichiestaTrasportoPartenza,
                                                                    msg,
                                                                    db);
                                    //this.EmailAnnullaRichiestaTEPartenza(atep_New.IDATEPARTENZA, db);
                                    using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                    {
                                        dtce.AnnullaMessaggioEvento(idTrasferimento, EnumFunzioniEventi.RichiestaTrasportoEffettiPartenza, db);
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




        public void EmailAnnullaRichiestaTEPartenza(decimal idAttivitaTrasportoEffettiPartenza, ModelDBISE db)
        {
            AccountModel am = new AccountModel();
            Mittente mittente = new Mittente();
            Destinatario to = new Destinatario();
            Destinatario cc = new Destinatario();

            try
            {
                am = Utility.UtenteAutorizzato();
                mittente.Nominativo = am.nominativo;
                mittente.EmailMittente = am.eMail;

                var atep = db.ATTIVITATEPARTENZA.Find(idAttivitaTrasportoEffettiPartenza);

                if (atep?.IDATEPARTENZA > 0)
                {
                    TRASFERIMENTO tr = atep.TEPARTENZA.TRASFERIMENTO;
                    DIPENDENTI dip = tr.DIPENDENTI;
                    UFFICI uff = tr.UFFICI;

                    using (GestioneEmail gmail = new GestioneEmail())
                    {
                        using (ModelloMsgMail msgMail = new ModelloMsgMail())
                        {
                            cc = new Destinatario()
                            {
                                Nominativo = am.nominativo,
                                EmailDestinatario = am.eMail
                            };

                            to = new Destinatario()
                            {
                                Nominativo = dip.NOME + " " + dip.COGNOME,
                                EmailDestinatario = dip.EMAIL,
                            };

                            msgMail.mittente = mittente;
                            msgMail.cc.Add(cc);
                            msgMail.destinatario.Add(to);

                            msgMail.oggetto =
                            Resources.msgEmail.OggettoAnnullaRichiestaTrasportoPartenza;
                            msgMail.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaTrasportoEffettiPartenza, uff.DESCRIZIONEUFFICIO + " (" + uff.CODICEUFFICIO + ")", tr.DATAPARTENZA.ToLongDateString());

                            gmail.sendMail(msgMail);
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AttivaRichiestaTEPartenza(decimal idAttivitaTrasportoEffettiPartenza)
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
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Attivazione trasporto effetti in partenza.", "ATTIVITATEPARTENZA", db,
                                    atep.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO, atep.IDATEPARTENZA);
                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.ModificaInCompletatoCalendarioEvento(atep.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaTrasportoEffettiPartenza, db);
                                }

                                this.EmailAttivaRichiestaTEPartenza(atep.IDATEPARTENZA, db);

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

        private void EmailAttivaRichiestaTEPartenza(decimal idAttivitaTrasportoEffettiPartenza, ModelDBISE db)
        {
            TEPARTENZA tep = new TEPARTENZA();
            AccountModel am = new AccountModel();
            Mittente mittente = new Mittente();
            Destinatario to = new Destinatario();
            Destinatario cc = new Destinatario();
            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();


            try
            {
                am = Utility.UtenteAutorizzato();
                mittente.Nominativo = am.nominativo;
                mittente.EmailMittente = am.eMail;

                var atep = db.ATTIVITATEPARTENZA.Find(idAttivitaTrasportoEffettiPartenza);

                tep = atep.TEPARTENZA;

                if (tep?.IDTEPARTENZA > 0)
                {
                    TRASFERIMENTO tr = tep.TRASFERIMENTO;
                    DIPENDENTI d = tr.DIPENDENTI;
                    UFFICI u = tr.UFFICI;

                    using (dtUtentiAutorizzati dtua = new dtUtentiAutorizzati())
                    {
                        using (GestioneEmail gmail = new GestioneEmail())
                        {
                            using (ModelloMsgMail msgMail = new ModelloMsgMail())
                            {

                                cc = new Destinatario()
                                {
                                    Nominativo = am.nominativo,
                                    EmailDestinatario = am.eMail
                                };

                                msgMail.mittente = mittente;
                                msgMail.cc.Add(cc);

                                luam.AddRange(dtua.GetUtentiByRuolo(EnumRuoloAccesso.Amministratore).ToList());

                                foreach (var uam in luam)
                                {
                                    var amministratore = db.DIPENDENTI.Find(uam.idDipendente);
                                    if (amministratore != null && amministratore.IDDIPENDENTE > 0)
                                    {
                                        to = new Destinatario()
                                        {
                                            Nominativo = amministratore.COGNOME + " " + amministratore.NOME,
                                            EmailDestinatario = amministratore.EMAIL
                                        };

                                        msgMail.destinatario.Add(to);
                                    }


                                }
                                msgMail.oggetto = Resources.msgEmail.OggettoAttivazioneTrasportoEffettiPartenza;

                                msgMail.corpoMsg =
                                        string.Format(
                                            Resources.msgEmail.MessaggioAttivazioneTrasportoEffettiPartenza,
                                            d.COGNOME + " " + d.NOME + " (" + d.MATRICOLA + ")",
                                            tr.DATAPARTENZA.ToLongDateString(),
                                            u.DESCRIZIONEUFFICIO + " (" + u.CODICEUFFICIO + ")");

                                gmail.sendMail(msgMail);

                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public PERCENTUALEANTICIPOTE GetPercentualeAnticipoTEPartenza(decimal idTEPartenza, decimal idTipoAnticipo)
        {

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var TEpartenza = db.TEPARTENZA.Find(idTEPartenza);
                    var t = TEpartenza.TRASFERIMENTO;

                    List<PERCENTUALEANTICIPOTE> lpatep = new List<PERCENTUALEANTICIPOTE>();
                    PERCENTUALEANTICIPOTE patep = new PERCENTUALEANTICIPOTE();

                    lpatep = TEpartenza.PERCENTUALEANTICIPOTE.Where(a => a.ANNULLATO == false &&
                                                                         a.IDTIPOANTICIPOTE == idTipoAnticipo &&
                                                                         a.DATAINIZIOVALIDITA <= t.DATAPARTENZA)
                        .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                    if (lpatep?.Any() ?? false)
                    {
                        patep = lpatep.First();
                    }
                    else
                    {
                        var lPte = db.PERCENTUALEANTICIPOTE.Where(a => a.ANNULLATO == false &&
                                                                       a.IDTIPOANTICIPOTE == idTipoAnticipo &&
                                                                       a.DATAINIZIOVALIDITA <= t.DATAPARTENZA)
                            .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                        if (lPte?.Any() ?? false)
                        {
                            patep = lPte.First();

                        }
                        else
                        {
                            throw new Exception("Non e' stata trovata nessuna percentuale di anticipo trasporto effetti in partenza per il trasferimento corrente.");
                        }

                    }

                    return patep;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void Associa_TEpartenza_perceAnticipoTE(decimal idTEPartenza, decimal idPercAnticipoTM, ModelDBISE db)
        {
            var tep = db.TEPARTENZA.Find(idTEPartenza);

            var item = db.Entry<TEPARTENZA>(tep);
            item.State = EntityState.Modified;
            item.Collection(a => a.PERCENTUALEANTICIPOTE).Load();
            var percAnticipo = db.PERCENTUALEANTICIPOTE.Find(idPercAnticipoTM);
            tep.PERCENTUALEANTICIPOTE.Add(percAnticipo);
            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Non è stato possibile associare la percentuale anticipo trasporto effetti al Trasporto Effetti in partenza.");
            }

        }

        public RinunciaTEPartenzaModel GetRinunciaTEPartenza(decimal idTEPartenza, ModelDBISE db)
        {
            try
            {
                RinunciaTEPartenzaModel rtepm = new RinunciaTEPartenzaModel();
                var tep = db.TEPARTENZA.Find(idTEPartenza);
                var atep = tep.ATTIVITATEPARTENZA.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATEPARTENZA).First();
                var rtep = atep.RINUNCIA_TE_P;
                if (rtep != null)
                {
                    rtepm = new RinunciaTEPartenzaModel()
                    {
                        idATEPartenza = rtep.IDATEPARTENZA,
                        rinunciaTE = rtep.RINUNCIATE,
                        dataAggiornamento = rtep.DATAAGGIORNAMENTO
                    };
                }
                else
                {
                    var new_rtep = this.CreaRinunciaTEPartenza(atep.IDATEPARTENZA, db);
                    rtepm = new RinunciaTEPartenzaModel()
                    {
                        idATEPartenza = new_rtep.IDATEPARTENZA,
                        rinunciaTE = new_rtep.RINUNCIATE,
                        dataAggiornamento = new_rtep.DATAAGGIORNAMENTO
                    };
                }

                return rtepm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public RINUNCIA_TE_P CreaRinunciaTEPartenza(decimal idATEPartenza, ModelDBISE db)
        {
            try
            {
                RINUNCIA_TE_P new_ratep = new RINUNCIA_TE_P()
                {
                    IDATEPARTENZA = idATEPartenza,
                    RINUNCIATE = false,
                    DATAAGGIORNAMENTO = DateTime.Now,
                };
                db.RINUNCIA_TE_P.Add(new_ratep);

                if (db.SaveChanges() <= 0)
                {
                    throw new Exception(string.Format("Non è stato possibile creare una nuova rinuncia trasporto effetti partenza."));
                }
                else
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova rinuncia trasporto effetti partenza.", "RINUNCIA_TE_P", db, new_ratep.ATTIVITATEPARTENZA.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO, new_ratep.IDATEPARTENZA);
                }

                return new_ratep;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Aggiorna_RinunciaTEPartenza(decimal idATEPArtenza)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var atep = db.ATTIVITATEPARTENZA.Find(idATEPArtenza);
                    var rtep = atep.RINUNCIA_TE_P;

                    if (rtep != null)
                    {
                        var stato_rtep = rtep.RINUNCIATE;
                        if (stato_rtep)
                        {
                            rtep.RINUNCIATE = false;
                            rtep.DATAAGGIORNAMENTO = DateTime.Now;
                        }
                        else
                        {
                            rtep.RINUNCIATE = true;
                            rtep.DATAAGGIORNAMENTO = DateTime.Now;
                        }

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception(string.Format("Impossibile aggiornare lo stato della rinuncia relativo al trasporto effetti in partenza"));
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                "Modifica Rinuncia TE Partenza", "RINUNCIA_TE_P", db, atep.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO,
                                rtep.IDATEPARTENZA);
                        }

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
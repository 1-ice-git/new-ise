using NewISE.EF;
using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Web.Configuration;
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models.ModelRest;
using NewISE.Models.Tools;
using NewISE.Models.Config;
using NewISE.Models.Config.s_admin;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtMaggiorazioniFamiliari : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public void PreSetMaggiorazioniFamiliari(decimal idTrasferimento, ModelDBISE db)
        {
            MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel()
            {
                idMaggiorazioniFamiliari = idTrasferimento
            };

            this.SetMaggiorazioneFamiliari(ref mfm, db);

            AttivazioniMagFamModel amfm = new AttivazioniMagFamModel()
            {
                idMaggiorazioniFamiliari = mfm.idMaggiorazioniFamiliari,
                richiestaAttivazione = false,
                attivazioneMagFam = false,
                dataVariazione = DateTime.Now,
                dataAggiornamento = DateTime.Now,
                annullato = false
            };

            using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
            {
                dtamf.SetAttivaziomeMagFam(ref amfm, db);

                RinunciaMaggiorazioniFamiliariModel rmfm = new RinunciaMaggiorazioniFamiliariModel()
                {
                    idMaggiorazioniFamiliari = mfm.idMaggiorazioniFamiliari,
                    rinunciaMaggiorazioni = false,
                    dataAggiornamento = DateTime.Now,
                    annullato = false
                };

                this.SetRinunciaMaggiorazioniFamiliari(ref rmfm, db);

                dtamf.AssociaRinunciaMagFam(amfm.idAttivazioneMagFam, rmfm.idRinunciaMagFam, db);
            }






        }




        public void SetRinunciaMaggiorazioniFamiliari(ref RinunciaMaggiorazioniFamiliariModel rmfm, ModelDBISE db)
        {
            RINUNCIAMAGGIORAZIONIFAMILIARI rmf = new RINUNCIAMAGGIORAZIONIFAMILIARI()
            {
                IDMAGGIORAZIONIFAMILIARI = rmfm.idMaggiorazioniFamiliari,
                RINUNCIAMAGGIORAZIONI = rmfm.rinunciaMaggiorazioni,
                DATAAGGIORNAMENTO = rmfm.dataAggiornamento,
                ANNULLATO = rmfm.annullato
            };

            db.RINUNCIAMAGGIORAZIONIFAMILIARI.Add(rmf);

            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Errore nella fase d'inserimento dei dati per la gestione della rinuncia maggiorazioni familiari.");
            }
            else
            {
                rmfm.idRinunciaMagFam = rmf.IDRINUNCIAMAGFAM;
                var mf = db.MAGGIORAZIONIFAMILIARI.Find(rmfm.idMaggiorazioniFamiliari);

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento  della rinuncia maggiorazioni familiari.",
                    "RINUNCIAMAGGIORAZIONIFAMILIARI", db, mf.TRASFERIMENTO.IDTRASFERIMENTO, rmf.IDRINUNCIAMAGFAM);
            }


        }

        public void AttivaRichiesta(decimal idAttivazioneMagFam)
        {
            bool rinunciaMagFam = false;
            bool richiestaAttivazione = false;
            bool attivazione = false;
            bool datiConiuge = false;
            bool datiParzialiConiuge = false;
            bool datiFigli = false;
            bool datiParzialiFigli = false;
            bool siDocConiuge = false;
            bool siDocFigli = false;
            bool docFormulario = false;

            int i = 0;

            this.SituazioneMagFamPartenza(idAttivazioneMagFam, out rinunciaMagFam,
                out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario);

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        if (rinunciaMagFam == true && richiestaAttivazione == true && attivazione == false)
                        {
                            var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);


                            amf.ATTIVAZIONEMAGFAM = true;
                            amf.DATAATTIVAZIONEMAGFAM = DateTime.Now;

                            i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore nella fase di attivazione delle maggiorazioni familiari.");
                            }
                            else
                            {
                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.ModificaInCompletatoCalendarioEvento(amf.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari, db);
                                }
                            }
                        }
                        else if (rinunciaMagFam == false && richiestaAttivazione == true && attivazione == false)
                        {
                            if (datiConiuge == true || datiFigli == true)
                            {
                                if (datiParzialiConiuge == false && datiParzialiFigli == false)
                                {
                                    if (datiConiuge == true && siDocConiuge == true || datiFigli == true && siDocFigli == true)
                                    {
                                        var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);


                                        amf.ATTIVAZIONEMAGFAM = true;

                                        i = db.SaveChanges();

                                        if (i <= 0)
                                        {
                                            throw new Exception("Errore nella fase di attivazione delle maggiorazioni familiari.");
                                        }
                                        else
                                        {
                                            using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                            {
                                                dtce.ModificaInCompletatoCalendarioEvento(amf.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari, db);
                                            }
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void EmailNotificaRichiesta(decimal idAttivazioneMagFam, ModelDBISE db)
        {
            MAGGIORAZIONIFAMILIARI mf = new MAGGIORAZIONIFAMILIARI();
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

                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

                mf = amf.MAGGIORAZIONIFAMILIARI;

                if (mf?.IDMAGGIORAZIONIFAMILIARI > 0)
                {
                    TRASFERIMENTO tr = mf.TRASFERIMENTO;
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


                                msgMail.oggetto =
                                    Resources.msgEmail.OggettoNotificaRichiestaMaggiorazioniFamiliari;
                                msgMail.corpoMsg =
                                    string.Format(
                                        Resources.msgEmail.MessaggioNotificaRichiestaMaggiorazioniFamiliari,
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

        private void EmailAnnullaRichiesta(decimal idAttivazioneMagFam, ModelDBISE db)
        {
            MAGGIORAZIONIFAMILIARI mf = new MAGGIORAZIONIFAMILIARI();
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

                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

                if (amf?.IDATTIVAZIONEMAGFAM > 0)
                {
                    mf = amf.MAGGIORAZIONIFAMILIARI;

                    if (mf?.IDMAGGIORAZIONIFAMILIARI > 0)
                    {
                        TRASFERIMENTO tr = mf.TRASFERIMENTO;
                        DIPENDENTI d = tr.DIPENDENTI;
                        UFFICI u = tr.UFFICI;

                        cc = new Destinatario()
                        {
                            Nominativo = am.nominativo,
                            EmailDestinatario = am.eMail
                        };

                        using (dtUtentiAutorizzati dtua = new dtUtentiAutorizzati())
                        {
                            using (GestioneEmail gmail = new GestioneEmail())
                            {
                                using (ModelloMsgMail msgMail = new ModelloMsgMail())
                                {
                                    msgMail.mittente = mittente;
                                    msgMail.cc.Add(cc);


                                    if (am.idRuoloUtente == (decimal)EnumRuoloAccesso.Amministratore || am.idRuoloUtente == (decimal)EnumRuoloAccesso.SuperAmministratore)
                                    {
                                        to = new Destinatario()
                                        {
                                            Nominativo = d.COGNOME + " " + d.NOME,
                                            EmailDestinatario = d.EMAIL
                                        };

                                        msgMail.destinatario.Add(to);
                                    }
                                    else
                                    {
                                        throw new Exception("Errore nella fase di annullamento della richiesta. La richiesta di maggiorazioni familiari può essere annullata soltanto dall'amministratore.");
                                    }


                                    if (msgMail.destinatario?.Any() ?? false)
                                    {

                                        msgMail.oggetto =
                                            Resources.msgEmail.OggettoAnnullaRichiestaMaggiorazioniFamiliari;
                                        msgMail.corpoMsg =
                                            string.Format(
                                                Resources.msgEmail.MessaggioAnnullaRichiestaMaggiorazioniFamiliari,
                                                u.DESCRIZIONEUFFICIO + " (" + u.CODICEUFFICIO + ")",
                                                tr.DATAPARTENZA.ToLongDateString());
                                        gmail.sendMail(msgMail);
                                    }
                                    else
                                    {
                                        throw new Exception("Non è stato possibile inviare l'email.");
                                    }



                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Non è stato possibile inviare l'email.");
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void EmailAttivazioneRichiesta(decimal idMaggiorazioniFamiliari, ModelDBISE db)
        {
            MAGGIORAZIONIFAMILIARI mf = new MAGGIORAZIONIFAMILIARI();
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

                mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                if (mf?.IDMAGGIORAZIONIFAMILIARI > 0)
                {
                    TRASFERIMENTO tr = mf.TRASFERIMENTO;
                    DIPENDENTI d = tr.DIPENDENTI;
                    UFFICI u = tr.UFFICI;

                    cc = new Destinatario()
                    {
                        Nominativo = am.nominativo,
                        EmailDestinatario = am.eMail
                    };

                    using (dtUtentiAutorizzati dtua = new dtUtentiAutorizzati())
                    {
                        using (GestioneEmail gmail = new GestioneEmail())
                        {

                            using (ModelloMsgMail msgMail = new ModelloMsgMail())
                            {
                                msgMail.mittente = mittente;
                                msgMail.cc.Add(cc);

                                if (am.idRuoloUtente == (decimal)EnumRuoloAccesso.Amministratore || am.idRuoloUtente == (decimal)EnumRuoloAccesso.SuperAmministratore)
                                {

                                    luam.AddRange(dtua.GetUtentiByRuolo(EnumRuoloAccesso.Amministratore).ToList());
                                    //luam.AddRange(dtua.GetUtentiByRuolo(EnumRuoloAccesso.SuperAmministratore).ToList());

                                    //if (luam?.Any() ?? false)
                                    //{
                                    foreach (var uam in luam)
                                    {
                                        var amministratore = db.DIPENDENTI.Find(uam.idDipendente);
                                        if (amministratore != null && amministratore.IDDIPENDENTE > 0)
                                        {
                                            cc = new Destinatario()
                                            {
                                                Nominativo = amministratore.COGNOME + " " + amministratore.NOME,
                                                EmailDestinatario = amministratore.EMAIL
                                            };

                                            msgMail.cc.Add(cc);
                                        }


                                    }


                                    //}

                                    to = new Destinatario()
                                    {
                                        Nominativo = d.COGNOME + " " + d.NOME,
                                        EmailDestinatario = d.EMAIL

                                    };

                                    msgMail.destinatario.Add(to);


                                    //if (msgMail.destinatario?.Any() ?? false)
                                    //{

                                    msgMail.oggetto =
                                        Resources.msgEmail.OggettoAttivazioneMaggiorazioniFamiliari;
                                    msgMail.corpoMsg =
                                        string.Format(
                                            Resources.msgEmail.MessaggioAttivazioneMaggiorazioniFamiliari,
                                            u.DESCRIZIONEUFFICIO + " (" + u.CODICEUFFICIO + ")",
                                            tr.DATAPARTENZA.ToLongDateString());
                                    gmail.sendMail(msgMail);
                                    //}
                                    //else
                                    //{
                                    //    throw new Exception("Non è stato possibile inviare l'email.");
                                    //}
                                }
                                else
                                {
                                    throw new Exception("Errore nella fase di attivazione. L'attivazione può essere svolta solo dall'amministratore.");
                                }



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





        public void AnnullaRichiesta(decimal idAttivazioneMagFam, out decimal idAttivazioneMagFamNew)
        {
            idAttivazioneMagFamNew = 0;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        ///Prelevo la riga del ciclo di autorizzazione che si vuole annullare.
                        var amfOld = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

                        if (amfOld?.IDATTIVAZIONEMAGFAM > 0)
                        {

                            amfOld.DATAAGGIORNAMENTO = DateTime.Now;
                            amfOld.ANNULLATO = true;///Annullo la riga del ciclo di autorizzazione.

                            int i = db.SaveChanges();

                            if (i > 0)
                            {
                                ///Creo una nuova riga per il ciclo di autorizzazione.
                                ATTIVAZIONIMAGFAM amfNew = new ATTIVAZIONIMAGFAM()
                                {
                                    IDMAGGIORAZIONIFAMILIARI = amfOld.IDMAGGIORAZIONIFAMILIARI,
                                    RICHIESTAATTIVAZIONE = false,
                                    ATTIVAZIONEMAGFAM = false,
                                    DATAVARIAZIONE = DateTime.Now,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    ANNULLATO = false,
                                };

                                db.ATTIVAZIONIMAGFAM.Add(amfNew);///Consolido la riga del ciclo di autorizzazione.

                                int j = db.SaveChanges();

                                if (j > 0)
                                {
                                    idAttivazioneMagFamNew = amfNew.IDATTIVAZIONEMAGFAM;

                                    using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                                    {
                                        #region Coniuge
                                        ///Cliclo tutte le righe valide per il coniuge collegate alla vecchia riga per il ciclo di autorizzazione.
                                        foreach (var cOld in amfOld.CONIUGE)
                                        {
                                            //dtamf.AssociaConiugeAttivazione(amfNew.IDATTIVAZIONEMAGFAM, cOld.IDCONIUGE, db);

                                            ///Creo una nuova riga per il coniuge con le informazioni della vecchia riga.
                                            CONIUGE cNew = new CONIUGE()
                                            {
                                                IDTIPOLOGIACONIUGE = cOld.IDTIPOLOGIACONIUGE,
                                                IDMAGGIORAZIONIFAMILIARI = cOld.IDMAGGIORAZIONIFAMILIARI,
                                                NOME = cOld.NOME,
                                                COGNOME = cOld.COGNOME,
                                                CODICEFISCALE = cOld.CODICEFISCALE,
                                                DATAINIZIOVALIDITA = cOld.DATAINIZIOVALIDITA,
                                                DATAFINEVALIDITA = cOld.DATAFINEVALIDITA,
                                                DATAAGGIORNAMENTO = cOld.DATAAGGIORNAMENTO,
                                                MODIFICATO = cOld.MODIFICATO,
                                                FK_IDCONIUGE = cOld.FK_IDCONIUGE
                                            };

                                            amfNew.CONIUGE.Add(cNew);///Inserisco la nuova riga per il coniuge associata alla nuova riga per il ciclo di autorizzazione.

                                            int j2 = db.SaveChanges();

                                            if (j2 > 0)
                                            {
                                                #region Altri dati familiari coniuge
                                                ///Prelevo la vecchia riga di altri dati familiari collegati alla vecchia riga del coniuge.
                                                var ladfOld =
                                                    cOld.ALTRIDATIFAM.Where(a => a.ANNULLATO == false)
                                                        .OrderByDescending(a => a.IDALTRIDATIFAM);

                                                if (ladfOld?.Any() ?? false)///Esiste questo controllo ma è impossibile che si verifichi il contrario perché gli altri dati familiari sono obbligatori per attivare il ciclo di autorizzazione.
                                                {
                                                    var adfOld = ladfOld.First();
                                                    ///Creo una nuova riga di altri dati familiari identica alla vechia riga.
                                                    ALTRIDATIFAM adfNew = new ALTRIDATIFAM()
                                                    {
                                                        IDCONIUGE = adfOld.IDCONIUGE,
                                                        DATANASCITA = adfOld.DATANASCITA,
                                                        CAPNASCITA = adfOld.CAPNASCITA,
                                                        COMUNENASCITA = adfOld.COMUNENASCITA,
                                                        PROVINCIANASCITA = adfOld.PROVINCIANASCITA,
                                                        NAZIONALITA = adfOld.NAZIONALITA,
                                                        INDIRIZZORESIDENZA = adfOld.INDIRIZZORESIDENZA,
                                                        CAPRESIDENZA = adfOld.CAPRESIDENZA,
                                                        COMUNERESIDENZA = adfOld.COMUNERESIDENZA,
                                                        PROVINCIARESIDENZA = adfOld.PROVINCIARESIDENZA,
                                                        DATAAGGIORNAMENTO = adfOld.DATAAGGIORNAMENTO,
                                                        ANNULLATO = adfOld.ANNULLATO
                                                    };

                                                    cNew.ALTRIDATIFAM.Add(adfNew);///La consolido e l'associo al coniuge

                                                    int j3 = db.SaveChanges();

                                                    if (j3 > 0)
                                                    {
                                                        ///Verifico se la vecchia riga di altri dati familiari era collegata alla vecchia riga del ciclo di autorizzazione,
                                                        /// se si associo la nuova riga di altri dati familiari alla nuova riga del ciclo di autorizzazione.
                                                        if (adfOld.ATTIVAZIONIMAGFAM?.Any(a => a.IDATTIVAZIONEMAGFAM == amfOld.IDATTIVAZIONEMAGFAM) ?? false)
                                                        {
                                                            dtamf.AssociaAltriDatiFamiliari(amfNew.IDATTIVAZIONEMAGFAM, adfNew.IDALTRIDATIFAM, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione altri dati familiari per il coniuge.");
                                                    }

                                                }
                                                #endregion

                                                #region Documenti identità coniuge
                                                ///Prelevo tutti i vecchi documenti d'identità.
                                                var ldOld =
                                                    cOld.DOCUMENTI.Where(
                                                        a =>
                                                            a.MODIFICATO == false &&
                                                            a.IDTIPODOCUMENTO ==
                                                            (decimal)EnumTipoDoc.Documento_Identita);

                                                foreach (DOCUMENTI dOld in ldOld)
                                                {
                                                    ///Creo la nuova riga per il documento con l'informazioni della vecchia riga.
                                                    DOCUMENTI dNew = new DOCUMENTI()
                                                    {
                                                        IDTIPODOCUMENTO = dOld.IDTIPODOCUMENTO,
                                                        NOMEDOCUMENTO = dOld.NOMEDOCUMENTO,
                                                        ESTENSIONE = dOld.ESTENSIONE,
                                                        FILEDOCUMENTO = dOld.FILEDOCUMENTO,
                                                        DATAINSERIMENTO = dOld.DATAINSERIMENTO,
                                                        MODIFICATO = dOld.MODIFICATO,
                                                        FK_IDDOCUMENTO = dOld.FK_IDDOCUMENTO
                                                    };

                                                    cNew.DOCUMENTI.Add(dNew);///Consolido il documento associandolo alla nuova riga del coniuge.

                                                    int j4 = db.SaveChanges();

                                                    if (j4 > 0)
                                                    {
                                                        ///Verifico se il vecchio documento era associato al vecchio ciclo di autorizzazione,
                                                        /// se si, la nuova riga del documento l'associo alla nuova riga per il ciclo di autorizzazione.
                                                        if (dOld.ATTIVAZIONIMAGFAM?.Any(a => a.IDATTIVAZIONEMAGFAM == amfOld.IDATTIVAZIONEMAGFAM) ?? false)
                                                        {
                                                            dtamf.AssociaDocumentoAttivazione(amfNew.IDATTIVAZIONEMAGFAM, dNew.IDDOCUMENTO, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione documenti d'identità");
                                                    }
                                                }
                                                #endregion

                                                #region Pensioni

                                                var lpOld = cOld.PENSIONE.Where(a => a.ANNULLATO == false);

                                                foreach (PENSIONE pOld in lpOld)
                                                {
                                                    PENSIONE pNew = new PENSIONE()
                                                    {
                                                        IMPORTOPENSIONE = pOld.IMPORTOPENSIONE,
                                                        DATAINIZIO = pOld.DATAINIZIO,
                                                        DATAFINE = pOld.DATAFINE,
                                                        DATAAGGIORNAMENTO = pOld.DATAAGGIORNAMENTO,
                                                        ANNULLATO = pOld.ANNULLATO
                                                    };

                                                    cNew.PENSIONE.Add(pNew);

                                                    int j5 = db.SaveChanges();

                                                    if (j5 > 0)
                                                    {
                                                        if (pOld.ATTIVAZIONIMAGFAM?.Any(a => a.IDATTIVAZIONEMAGFAM == amfOld.IDATTIVAZIONEMAGFAM) ?? false)
                                                        {
                                                            dtamf.AssociaPensioneAttivazione(amfNew.IDATTIVAZIONEMAGFAM, pNew.IDPENSIONE, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione pensioni.");
                                                    }
                                                }

                                                #endregion

                                                #region Percentuale maggiorazione coniuge
                                                using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                                                {
                                                    DateTime dtIni = cNew.DATAINIZIOVALIDITA;
                                                    DateTime dtFin = cNew.DATAFINEVALIDITA;

                                                    List<PercentualeMagConiugeModel> lpmcm =
                                                        dtpc.GetListaPercentualiMagConiugeByRangeDate((EnumTipologiaConiuge)cNew.IDTIPOLOGIACONIUGE, dtIni, dtFin, db).ToList();

                                                    if (lpmcm?.Any() ?? false)
                                                    {
                                                        foreach (var pmcm in lpmcm)
                                                        {
                                                            dtpc.AssociaPercentualeMaggiorazioneConiuge(cNew.IDCONIUGE, pmcm.idPercentualeConiuge, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Non è presente nessuna percentuale del coniuge.");
                                                    }
                                                }
                                                #endregion

                                            }
                                            else
                                            {
                                                throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione coniuge.");
                                            }
                                        }
                                        #endregion

                                        #region Figli
                                        foreach (var fOld in amfOld.FIGLI)
                                        {
                                            FIGLI fNew = new FIGLI()
                                            {
                                                IDTIPOLOGIAFIGLIO = fOld.IDTIPOLOGIAFIGLIO,
                                                IDMAGGIORAZIONIFAMILIARI = fOld.IDMAGGIORAZIONIFAMILIARI,
                                                NOME = fOld.NOME,
                                                COGNOME = fOld.COGNOME,
                                                CODICEFISCALE = fOld.CODICEFISCALE,
                                                DATAINIZIOVALIDITA = fOld.DATAINIZIOVALIDITA,
                                                DATAFINEVALIDITA = fOld.DATAFINEVALIDITA,
                                                DATAAGGIORNAMENTO = fOld.DATAAGGIORNAMENTO,
                                                MODIFICATO = fOld.MODIFICATO,
                                                FK_IDFIGLI = fOld.FK_IDFIGLI
                                            };

                                            amfNew.FIGLI.Add(fNew);

                                            int x = db.SaveChanges();

                                            if (x > 0)
                                            {
                                                #region Altri dati familiari
                                                ///Prelevo la vecchia riga di altri dati familiari collegati alla vecchia riga del coniuge.
                                                var ladfOld =
                                                    fOld.ALTRIDATIFAM.Where(a => a.ANNULLATO == false)
                                                        .OrderByDescending(a => a.IDALTRIDATIFAM);

                                                if (ladfOld?.Any() ?? false)///Esiste questo controllo ma è impossibile che si verifichi il contrario perché gli altri dati familiari sono obbligatori per attivare il ciclo di autorizzazione.
                                                {
                                                    var adfOld = ladfOld.First();
                                                    ///Creo una nuova riga di altri dati familiari identica alla vechia riga.
                                                    ALTRIDATIFAM adfNew = new ALTRIDATIFAM()
                                                    {
                                                        IDCONIUGE = adfOld.IDCONIUGE,
                                                        DATANASCITA = adfOld.DATANASCITA,
                                                        CAPNASCITA = adfOld.CAPNASCITA,
                                                        COMUNENASCITA = adfOld.COMUNENASCITA,
                                                        PROVINCIANASCITA = adfOld.PROVINCIANASCITA,
                                                        NAZIONALITA = adfOld.NAZIONALITA,
                                                        INDIRIZZORESIDENZA = adfOld.INDIRIZZORESIDENZA,
                                                        CAPRESIDENZA = adfOld.CAPRESIDENZA,
                                                        COMUNERESIDENZA = adfOld.COMUNERESIDENZA,
                                                        PROVINCIARESIDENZA = adfOld.PROVINCIARESIDENZA,
                                                        DATAAGGIORNAMENTO = adfOld.DATAAGGIORNAMENTO,
                                                        ANNULLATO = adfOld.ANNULLATO
                                                    };

                                                    fNew.ALTRIDATIFAM.Add(adfNew);///La consolido e l'associo al figlio

                                                    int x2 = db.SaveChanges();

                                                    if (x2 > 0)
                                                    {
                                                        ///Verifico se la vecchia riga di altri dati familiari era collegata alla vecchia riga del ciclo di autorizzazione,
                                                        /// se si associo la nuova riga di altri dati familiari alla nuova riga del ciclo di autorizzazione.
                                                        if (adfOld.ATTIVAZIONIMAGFAM?.Any(a => a.IDATTIVAZIONEMAGFAM == amfOld.IDATTIVAZIONEMAGFAM) ?? false)
                                                        {
                                                            dtamf.AssociaAltriDatiFamiliari(amfNew.IDATTIVAZIONEMAGFAM, adfNew.IDALTRIDATIFAM, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione altri dati familiari per i figli.");
                                                    }
                                                }
                                                #endregion

                                                #region Documenti
                                                ///Prelevo tutti i vecchi documenti d'identità.
                                                var ldOld =
                                                    fOld.DOCUMENTI.Where(
                                                        a =>
                                                            a.MODIFICATO == false &&
                                                            a.IDTIPODOCUMENTO ==
                                                            (decimal)EnumTipoDoc.Documento_Identita);

                                                foreach (var dOld in ldOld)
                                                {
                                                    ///Creo la nuova riga per il documento con l'informazioni della vecchia riga.
                                                    DOCUMENTI dNew = new DOCUMENTI()
                                                    {
                                                        IDTIPODOCUMENTO = dOld.IDTIPODOCUMENTO,
                                                        NOMEDOCUMENTO = dOld.NOMEDOCUMENTO,
                                                        ESTENSIONE = dOld.ESTENSIONE,
                                                        FILEDOCUMENTO = dOld.FILEDOCUMENTO,
                                                        DATAINSERIMENTO = dOld.DATAINSERIMENTO,
                                                        MODIFICATO = dOld.MODIFICATO,
                                                        FK_IDDOCUMENTO = dOld.FK_IDDOCUMENTO
                                                    };

                                                    fNew.DOCUMENTI.Add(dNew);///Consolido il documento associandolo alla nuova riga del coniuge.

                                                    int j4 = db.SaveChanges();

                                                    if (j4 > 0)
                                                    {
                                                        ///Verifico se il vecchio documento era associato al vecchio ciclo di autorizzazione,
                                                        /// se si, la nuova riga del documento l'associo alla nuova riga per il ciclo di autorizzazione.
                                                        if (dOld.ATTIVAZIONIMAGFAM?.Any(a => a.IDATTIVAZIONEMAGFAM == amfOld.IDATTIVAZIONEMAGFAM) ?? false)
                                                        {
                                                            dtamf.AssociaDocumentoAttivazione(amfNew.IDATTIVAZIONEMAGFAM, dNew.IDDOCUMENTO, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione documenti d'identità");
                                                    }
                                                }
                                                #endregion

                                                #region Indennità primo segretario
                                                using (dtIndennitaPrimoSegretario dtips = new dtIndennitaPrimoSegretario())
                                                {
                                                    DateTime dtIni = fNew.DATAINIZIOVALIDITA;
                                                    DateTime dtFin = fNew.DATAFINEVALIDITA;

                                                    List<IndennitaPrimoSegretModel> lipsm =
                                                        dtips.GetIndennitaPrimoSegretario(dtIni, dtFin, db).ToList();

                                                    if (lipsm?.Any() ?? false)
                                                    {
                                                        foreach (var ipsm in lipsm)
                                                        {
                                                            dtips.AssociaIndennitaPrimoSegretarioFiglio(fNew.IDFIGLI, ipsm.idIndPrimoSegr, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception(
                                                            "Errore nella fase di annulla richiesta. Non è presente nessuna indennità di primo segretario per il figlio che si vuole inserire.");
                                                    }
                                                }
                                                #endregion

                                                #region Percentuale maggiorazioni figli
                                                using (dtPercentualeMagFigli dtpf = new dtPercentualeMagFigli())
                                                {
                                                    DateTime dtIni = fNew.DATAINIZIOVALIDITA;
                                                    DateTime dtFin = fNew.DATAFINEVALIDITA;

                                                    List<PercentualeMagFigliModel> lpmfm =
                                                        dtpf.GetPercentualeMaggiorazioneFigli((TipologiaFiglio)fNew.IDTIPOLOGIAFIGLIO, dtIni, dtFin, db).ToList();

                                                    if (lpmfm?.Any() ?? false)
                                                    {
                                                        foreach (var pmfm in lpmfm)
                                                        {
                                                            dtpf.AssociaPercentualeMaggiorazioneFigli(fNew.IDFIGLI, pmfm.idPercMagFigli, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Non è presente nessuna percentuale per il figlio.");
                                                    }
                                                }
                                                #endregion

                                            }
                                            else
                                            {
                                                throw new Exception();
                                            }


                                        }
                                        #endregion

                                        #region Formulari

                                        var ldFormulariOld =
                                            amfOld.DOCUMENTI.Where(
                                                a =>
                                                    a.IDTIPODOCUMENTO ==
                                                    (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari);

                                        foreach (var d in ldFormulariOld)
                                        {
                                            DOCUMENTI dNew = new DOCUMENTI()
                                            {
                                                IDTIPODOCUMENTO = d.IDTIPODOCUMENTO,
                                                NOMEDOCUMENTO = d.NOMEDOCUMENTO,
                                                ESTENSIONE = d.ESTENSIONE,
                                                FILEDOCUMENTO = d.FILEDOCUMENTO,
                                                DATAINSERIMENTO = d.DATAINSERIMENTO,
                                                MODIFICATO = d.MODIFICATO,
                                                FK_IDDOCUMENTO = d.FK_IDDOCUMENTO
                                            };

                                            amfNew.DOCUMENTI.Add(d);

                                            //dtamf.AssociaFormulario(amfNew.IDATTIVAZIONEMAGFAM, d.IDDOCUMENTO, db);
                                        }
                                        #endregion


                                    }
                                }
                                else
                                {
                                    throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione ciclo autorizzazione.");
                                }

                                if (amfOld.CONIUGE.Count <= 0 || amfOld.FIGLI.Count <= 0)
                                {
                                    using (dtRinunciaMagFam dtrmf = new dtRinunciaMagFam())
                                    {
                                        dtrmf.AnnullaRinuncia(idAttivazioneMagFam, idAttivazioneMagFamNew, db);
                                    }
                                }

                                this.EmailAnnullaRichiesta(idAttivazioneMagFam, db);

                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.AnnullaMessaggioEvento(amfOld.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari, db);
                                }

                            }
                            else
                            {
                                throw new Exception("Errore nella fase di annullamento della riga di attivazione maggiorazione familiare per l'id: " + amfOld.IDATTIVAZIONEMAGFAM);
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

        public void NotificaRichiesta(decimal idAttivazioneMagFam)
        {
            bool rinunciaMagFam = false;
            bool richiestaAttivazione = false;
            bool attivazione = false;
            bool datiConiuge = false;
            bool datiParzialiConiuge = false;
            bool datiFigli = false;
            bool datiParzialiFigli = false;
            bool siDocConiuge = false;
            bool siDocFigli = false;
            bool docFormulario = false;
            int i = 0;

            this.SituazioneMagFamPartenza(idAttivazioneMagFam, out rinunciaMagFam,
                out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario);

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);


                        if (amf?.IDATTIVAZIONEMAGFAM > 0)
                        {

                            if (rinunciaMagFam == false && richiestaAttivazione == false && attivazione == false)
                            {
                                if (datiConiuge == false && datiFigli == false)
                                {
                                    var rmf =
                                        amf.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a => a.ANNULLATO == false)
                                            .OrderByDescending(a => a.IDRINUNCIAMAGFAM)
                                            .First();

                                    rmf.RINUNCIAMAGGIORAZIONI = true;
                                    amf.RICHIESTAATTIVAZIONE = true;
                                    amf.DATARICHIESTAATTIVAZIONE = DateTime.Now;

                                    i = db.SaveChanges();
                                    if (i <= 0)
                                    {
                                        throw new Exception("Errore nella fase d'inserimento per la rinuncia delle maggiorazioni familiari.");
                                    }
                                    else
                                    {
                                        this.EmailNotificaRichiesta(idAttivazioneMagFam, db);

                                        using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                        {
                                            CalendarioEventiModel cem = new CalendarioEventiModel()
                                            {
                                                idFunzioneEventi = EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari,
                                                idTrasferimento = amf.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                DataInizioEvento = DateTime.Now.Date,
                                                DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaMaggiorazioniFamiliari)).Date,

                                            };

                                            dtce.InsertCalendarioEvento(ref cem, db);
                                        }
                                    }
                                }
                                else if (datiConiuge == true || datiFigli == true)
                                {
                                    if (datiParzialiConiuge == false && datiParzialiFigli == false)
                                    {
                                        amf.RICHIESTAATTIVAZIONE = true;
                                        amf.DATARICHIESTAATTIVAZIONE = DateTime.Now;

                                        i = db.SaveChanges();
                                        if (i <= 0)
                                        {
                                            throw new Exception("Errore nella fase d'inserimento per la richiesta attivazione per le maggiorazioni familiari.");
                                        }
                                        else
                                        {
                                            this.EmailNotificaRichiesta(idAttivazioneMagFam, db);
                                            using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                            {
                                                CalendarioEventiModel cem = new CalendarioEventiModel()
                                                {
                                                    idFunzioneEventi = EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari,
                                                    idTrasferimento = amf.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                    DataInizioEvento = DateTime.Now.Date,
                                                    DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaMaggiorazioniFamiliari)).Date,

                                                };

                                                dtce.InsertCalendarioEvento(ref cem, db);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Errore nella notifica della richiesta di attivazione per le maggiorazioni familiari, record ATTIVAZIONEMAGFAM non trovato.");
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


        public void SituazioneMagFamPartenza(decimal idAttivazioneMagFam, out bool rinunciaMagFam,
                                       out bool richiestaAttivazione, out bool Attivazione,
                                       out bool datiConiuge, out bool datiParzialiConiuge,
                                       out bool datiFigli, out bool datiParzialiFigli,
                                       out bool siDocConiuge, out bool siDocFigli,
                                       out bool docFormulario)
        {
            rinunciaMagFam = false;
            richiestaAttivazione = false;
            Attivazione = false;
            datiConiuge = false;
            datiParzialiConiuge = false;
            datiFigli = false;
            datiParzialiFigli = false;
            siDocConiuge = false;
            siDocFigli = false;
            docFormulario = false;


            using (ModelDBISE db = new ModelDBISE())
            {


                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

                //var lamf = mf.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false);

                if (amf?.IDATTIVAZIONEMAGFAM > 0)
                {
                    var mf = amf.MAGGIORAZIONIFAMILIARI;

                    if (mf?.IDMAGGIORAZIONIFAMILIARI > 0)
                    {
                        var lrmf =
                            amf.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a => a.ANNULLATO == false)
                                .OrderByDescending(a => a.IDRINUNCIAMAGFAM);

                        if (lrmf?.Any() ?? false)
                        {
                            var rmf = lrmf.First();

                            rinunciaMagFam = rmf.RINUNCIAMAGGIORAZIONI;
                        }
                        else
                        {
                            rinunciaMagFam = false;
                        }


                        richiestaAttivazione = amf.RICHIESTAATTIVAZIONE;
                        Attivazione = amf.ATTIVAZIONEMAGFAM;



                        var ld = amf.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari);
                        if (ld?.Any() ?? false)
                        {
                            docFormulario = true;
                        }

                        if (mf.CONIUGE != null)
                        {
                            var lc = mf.CONIUGE.ToList();
                            if (lc?.Any() ?? false)
                            {
                                datiConiuge = true;
                                foreach (var c in lc)
                                {
                                    var nadc = c.ALTRIDATIFAM.Count(a => a.ANNULLATO == false);

                                    if (nadc > 0)
                                    {
                                        datiParzialiConiuge = false;
                                    }
                                    else
                                    {
                                        datiParzialiConiuge = true;
                                        break;
                                    }
                                }

                                foreach (var c in lc)
                                {
                                    var ndocc = c.DOCUMENTI.Count;

                                    if (ndocc > 0)
                                    {
                                        siDocConiuge = true;
                                    }
                                    else
                                    {
                                        siDocConiuge = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                datiConiuge = false;
                            }

                        }

                        if (mf.FIGLI != null)
                        {
                            var lf = mf.FIGLI.ToList();

                            if (lf?.Any() ?? false)
                            {
                                datiFigli = true;
                                foreach (var f in lf)
                                {
                                    var nadf = f.ALTRIDATIFAM.Count(a => a.ANNULLATO == false);

                                    if (nadf > 0)
                                    {
                                        datiParzialiFigli = false;
                                    }
                                    else
                                    {
                                        datiParzialiFigli = true;
                                        break;
                                    }
                                }

                                foreach (var f in lf)
                                {
                                    var ndocf = f.DOCUMENTI.Count;
                                    if (ndocf > 0)
                                    {
                                        siDocFigli = true;
                                    }
                                    else
                                    {
                                        siDocFigli = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                datiFigli = false;
                            }
                        }
                    }
                }



            }

        }



        public MaggiorazioniFamiliariModel GetMaggiorazioniFamiliariByID(decimal idMaggiorazioniFamiliari, ModelDBISE db)
        {
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();


            var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

            if (mf != null && mf.IDMAGGIORAZIONIFAMILIARI > 0)
            {
                mcm = new MaggiorazioniFamiliariModel()
                {
                    idMaggiorazioniFamiliari = mf.IDMAGGIORAZIONIFAMILIARI,
                };
            }


            return mcm;
        }


        public MaggiorazioniFamiliariModel GetMaggiorazioniFamiliariByID(decimal idMaggiorazioniFamiliari)
        {
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                if (mf != null && mf.IDMAGGIORAZIONIFAMILIARI > 0)
                {
                    mcm = new MaggiorazioniFamiliariModel()
                    {
                        idMaggiorazioniFamiliari = mf.IDMAGGIORAZIONIFAMILIARI,

                    };
                }
            }

            return mcm;
        }








        public MaggiorazioniFamiliariModel GetMaggiorazioniFamiliaribyFiglio(decimal idFiglio)
        {
            MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.FIGLI.Find(idFiglio).MAGGIORAZIONIFAMILIARI;

                if (mf != null && mf.IDMAGGIORAZIONIFAMILIARI > 0)
                {
                    mfm = new MaggiorazioniFamiliariModel()
                    {
                        idMaggiorazioniFamiliari = mf.IDMAGGIORAZIONIFAMILIARI,

                    };
                }
            }

            return mfm;
        }

        public MaggiorazioniFamiliariModel GetMaggiorazioniFamiliaribyConiuge(decimal idConiuge)
        {
            MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.CONIUGE.Find(idConiuge).MAGGIORAZIONIFAMILIARI;

                if (mf != null && mf.IDMAGGIORAZIONIFAMILIARI > 0)
                {
                    mfm = new MaggiorazioniFamiliariModel()
                    {
                        idMaggiorazioniFamiliari = mf.IDMAGGIORAZIONIFAMILIARI,

                    };
                }
            }
            return mfm;
        }


        public void SetMaggiorazioneFamiliari(ref MaggiorazioniFamiliariModel mfm, ModelDBISE db)
        {
            MAGGIORAZIONIFAMILIARI mf = new MAGGIORAZIONIFAMILIARI()
            {
                IDMAGGIORAZIONIFAMILIARI = mfm.idMaggiorazioniFamiliari
            };

            db.MAGGIORAZIONIFAMILIARI.Add(mf);

            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Maggiorazioni familiari non inserite.");
            }

        }


        public void InserisciFiglioMagFam(FigliModel fm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {


                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var tm = dtt.GetTrasferimentoByIDMagFam(fm.idMaggiorazioniFamiliari);

                        using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                        {
                            var p = dtpp.GetPassaportoInLavorazioneByIdTrasf(tm.idTrasferimento);
                            fm.idPassaporti = p.idPassaporto;
                        }

                        using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                        {
                            //var tvm = dttv.GetTitoloViaggioInLavorazioneByIdTrasf(tm.idTrasferimento);
                            //fm.idTitoloViaggio = tvm.idTitoloViaggio;
                        }

                    }



                    using (dtFigli dtf = new dtFigli())
                    {
                        fm.dataAggiornamento = DateTime.Now;



                        dtf.SetFiglio(ref fm, db);
                        using (dtPercentualeMagFigli dtpf = new dtPercentualeMagFigli())
                        {
                            DateTime dtIni = fm.dataInizio.Value;
                            DateTime dtFin = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop();

                            List<PercentualeMagFigliModel> lpmfm =
                                dtpf.GetPercentualeMaggiorazioneFigli((TipologiaFiglio)fm.idTipologiaFiglio, dtIni,
                                    dtFin, db).ToList();

                            if (lpmfm?.Any() ?? false)
                            {
                                foreach (var pmfm in lpmfm)
                                {
                                    dtpf.AssociaPercentualeMaggiorazioneFigli(fm.idFigli, pmfm.idPercMagFigli, db);
                                }
                            }
                            else
                            {
                                throw new Exception("Non è presente nessuna percentuale per il figlio.");
                            }
                        }

                        using (dtIndennitaPrimoSegretario dtips = new dtIndennitaPrimoSegretario())
                        {
                            DateTime dtIni = fm.dataInizio.Value;
                            DateTime dtFin = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop();

                            List<IndennitaPrimoSegretModel> lipsm =
                                dtips.GetIndennitaPrimoSegretario(dtIni, dtFin, db).ToList();

                            if (lipsm?.Any() ?? false)
                            {
                                foreach (var ipsm in lipsm)
                                {
                                    dtips.AssociaIndennitaPrimoSegretarioFiglio(fm.idFigli, ipsm.idIndPrimoSegr, db);
                                }
                            }
                            else
                            {
                                throw new Exception(
                                    "Non è presente nessuna indennità di primo segretario per il figlio che si vuole inserire.");
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

        public void InserisciConiugeMagFam(ConiugeModel cm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {


                    if (cm.idMaggiorazioniFamiliari == 0 && cm.idAttivazioneMagFam > 0)
                    {
                        var amf = db.ATTIVAZIONIMAGFAM.Find(cm.idAttivazioneMagFam);
                        cm.idMaggiorazioniFamiliari = amf.IDMAGGIORAZIONIFAMILIARI;
                    }

                    using (dtConiuge dtc = new dtConiuge())
                    {
                        cm.dataAggiornamento = DateTime.Now;

                        dtc.SetConiuge(ref cm, db);

                        using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                        {
                            DateTime dtIni = cm.dataInizio.Value;
                            DateTime dtFin = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop();

                            List<PercentualeMagConiugeModel> lpmcm =
                                dtpc.GetListaPercentualiMagConiugeByRangeDate(cm.idTipologiaConiuge, dtIni, dtFin, db)
                                    .ToList();

                            if (lpmcm?.Any() ?? false)
                            {
                                foreach (var pmcm in lpmcm)
                                {
                                    dtpc.AssociaPercentualeMaggiorazioneConiuge(cm.idConiuge, pmcm.idPercentualeConiuge,
                                        db);
                                }
                            }
                            else
                            {
                                throw new Exception("Non è presente nessuna percentuale del coniuge.");
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

        public void ModificaConiuge(ConiugeModel cm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();
                try
                {
                    using (dtConiuge dtc = new dtConiuge())
                    {
                        dtc.EditConiugeMagFam(cm, db);
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



        //public void SituazioneMagFamVariazione_old(decimal idMaggiorazioniFamiliari, out bool rinunciaMagFam,
        //                       out bool richiestaAttivazione, out bool Attivazione,
        //                       out bool datiConiuge, out bool datiParzialiConiuge,
        //                       out bool datiFigli, out bool datiParzialiFigli,
        //                       out bool siDocConiuge, out bool siDocFigli,
        //                       out bool docFormulario)
        //{
        //    rinunciaMagFam = false;
        //    richiestaAttivazione = false;
        //    Attivazione = false;
        //    datiConiuge = false;
        //    datiParzialiConiuge = false;
        //    datiFigli = false;
        //    datiParzialiFigli = false;
        //    siDocConiuge = false;
        //    siDocFigli = false;
        //    docFormulario = false;


        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

        //        var lamf =
        //            mf.ATTIVAZIONIMAGFAM.Where(
        //                e => (e.RICHIESTAATTIVAZIONE == false && e.ATTIVAZIONEMAGFAM == false && e.ANNULLATO == false))
        //                .OrderBy(a => a.IDATTIVAZIONEMAGFAM);

        //        //var lamf = mf.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false);

        //        if (lamf?.Any() ?? false)
        //        {
        //            var amf = lamf.First();


        //            if (amf != null && amf.IDATTIVAZIONEMAGFAM > 0)
        //            {

        //                var rmf =
        //                    mf.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a => a.ANNULLATO == false)
        //                        .OrderByDescending(a => a.IDRINUNCIAMAGFAM)
        //                        .First();

        //                rinunciaMagFam = rmf.RINUNCIAMAGGIORAZIONI;
        //                richiestaAttivazione = amf.RICHIESTAATTIVAZIONE;
        //                Attivazione = amf.ATTIVAZIONEMAGFAM;

        //                var ld = amf.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari);
        //                if (ld?.Any() ?? false)
        //                {
        //                    docFormulario = true;
        //                }


        //                if (mf.CONIUGE != null)
        //                {
        //                    var lc = mf.CONIUGE.ToList();
        //                    if (lc?.Any() ?? false)
        //                    {
        //                        datiConiuge = true;
        //                        foreach (var c in lc)
        //                        {
        //                            var nadc = c.ALTRIDATIFAM.Count(a => a.ANNULLATO == false);

        //                            if (nadc > 0)
        //                            {
        //                                datiParzialiConiuge = false;
        //                            }
        //                            else
        //                            {
        //                                datiParzialiConiuge = true;
        //                                break;
        //                            }
        //                        }

        //                        foreach (var c in lc)
        //                        {
        //                            var ndocc = c.DOCUMENTI.Count;

        //                            if (ndocc > 0)
        //                            {
        //                                siDocConiuge = true;
        //                            }
        //                            else
        //                            {
        //                                siDocConiuge = false;
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        datiConiuge = false;
        //                    }

        //                }

        //                if (mf.FIGLI != null)
        //                {
        //                    var lf = mf.FIGLI.ToList();

        //                    if (lf?.Any() ?? false)
        //                    {
        //                        datiFigli = true;
        //                        foreach (var f in lf)
        //                        {
        //                            var nadf = f.ALTRIDATIFAM.Count(a => a.ANNULLATO == false);

        //                            if (nadf > 0)
        //                            {
        //                                datiParzialiFigli = false;
        //                            }
        //                            else
        //                            {
        //                                datiParzialiFigli = true;
        //                                break;
        //                            }
        //                        }

        //                        foreach (var f in lf)
        //                        {
        //                            var ndocf = f.DOCUMENTI.Count;
        //                            if (ndocf > 0)
        //                            {
        //                                siDocFigli = true;
        //                            }
        //                            else
        //                            {
        //                                siDocFigli = false;
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        datiFigli = false;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        public void NotificaRichiestaVariazione(decimal idAttivazioneMagFam)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);
                        amf.RICHIESTAATTIVAZIONE = true;
                        amf.DATARICHIESTAATTIVAZIONE = DateTime.Now.Date;
                        amf.DATAAGGIORNAMENTO = DateTime.Now.Date;

                        var i = db.SaveChanges();
                        if (i <= 0)
                        {
                            throw new Exception("Errore nella fase d'inserimento per la richiesta attivazione per le maggiorazioni familiari.");
                        }
                        else
                        {
                            this.EmailNotificaRichiesta(idAttivazioneMagFam, db);

                            using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                            {
                                CalendarioEventiModel cem = new CalendarioEventiModel()
                                {
                                    idFunzioneEventi = EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari,
                                    idTrasferimento = amf.MAGGIORAZIONIFAMILIARI.IDMAGGIORAZIONIFAMILIARI,
                                    DataInizioEvento = DateTime.Now.Date,
                                    DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaMaggiorazioniFamiliari)).Date,
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



    }
}
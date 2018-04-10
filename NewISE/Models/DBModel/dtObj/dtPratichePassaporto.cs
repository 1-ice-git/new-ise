using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using NewISE.EF;
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models.Enumeratori;
using Newtonsoft.Json.Schema;
using NewISE.Models.Tools;
using RestSharp.Extensions;
using NewISE.Models.DBModel.Enum;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtPratichePassaporto : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        #region Email
        public void EmailCompletaRichiestaPassaporto(decimal idAttivazionePassaporto, ModelDBISE db)
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

                var ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazionePassaporto);

                if (ap?.IDATTIVAZIONIPASSAPORTI > 0)
                {
                    TRASFERIMENTO tr = ap.PASSAPORTI.TRASFERIMENTO;
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
                                Resources.msgEmail.OggettoRichiestaPratichePassaportoConcluse;
                            msgMail.corpoMsg = string.Format(Resources.msgEmail.MessaggioRichiestaPratichePassaportoConcluse, uff.DESCRIZIONEUFFICIO + " (" + uff.CODICEUFFICIO + ")", tr.DATAPARTENZA.ToLongDateString());
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

        public void EmailAnnullaRichiestaPassaporto(decimal idAttivazionePassaporto, ModelDBISE db)
        {
            AccountModel am = new AccountModel();
            Mittente mittente = new Mittente();
            Destinatario to = new Destinatario();
            Destinatario cc = new Destinatario();
            //List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();

            try
            {
                am = Utility.UtenteAutorizzato();
                mittente.Nominativo = am.nominativo;
                mittente.EmailMittente = am.eMail;

                var ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazionePassaporto);

                if (ap?.IDATTIVAZIONIPASSAPORTI > 0)
                {
                    TRASFERIMENTO tr = ap.PASSAPORTI.TRASFERIMENTO;
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
                                Resources.msgEmail.OggettoAnnullaRichiestaPassaporto;
                            msgMail.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaPassaporto, uff.DESCRIZIONEUFFICIO + " (" + uff.CODICEUFFICIO + ")", tr.DATAPARTENZA.ToLongDateString());
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

        public void EmailNotificaRichiestaPassaporto(decimal idAttivazionePassaporto, ModelDBISE db)
        {
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

                var ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazionePassaporto);

                if (ap?.IDATTIVAZIONIPASSAPORTI > 0)
                {
                    TRASFERIMENTO tr = ap.PASSAPORTI.TRASFERIMENTO;
                    DIPENDENTI dip = tr.DIPENDENTI;
                    UFFICI uff = tr.UFFICI;

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

                                var destUggs = System.Configuration.ConfigurationManager.AppSettings["EmailUfficioGestioneGiuridicaEsviluppo"];
                                msgMail.destinatario.Add(new Destinatario() { Nominativo = "Ufficio Gestione Giuridica e Sviluppo", EmailDestinatario = destUggs });


                                msgMail.oggetto =
                                    Resources.msgEmail.OggettoRichiestaPratichePassaporto;
                                msgMail.corpoMsg = string.Format(Resources.msgEmail.MessaggioRichiestaPratichePassaporto, dip.COGNOME + " " + dip.NOME + " (" + dip.MATRICOLA + ")", uff.DESCRIZIONEUFFICIO + " (" + uff.CODICEUFFICIO + ")", tr.DATAPARTENZA.ToLongDateString());
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
        #endregion


        #region Ciclo di attivazione

        public void ConfermaRichiestaPassaporto(decimal idAttivazionePassaporto)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazionePassaporto);
                    if (ap?.IDATTIVAZIONIPASSAPORTI > 0)
                    {
                        if (ap.NOTIFICARICHIESTA == true)
                        {
                            ap.PRATICACONCLUSA = true;
                            ap.DATAPRATICACONCLUSA = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore: Impossibile completare l'approvazione delle pratiche del passaporto.");
                            }
                            else
                            {
                                #region ciclo attivazione doc richiedente
                                var lpr = ap.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false).ToList();
                                if (lpr?.Any() ?? false)
                                {
                                    foreach (var pr in lpr)
                                    {
                                        var ldpr = pr.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                                        foreach (var dpr in ldpr)
                                        {
                                            dpr.IDSTATORECORD = (decimal)EnumStatoRecord.Attivato;
                                            if (db.SaveChanges() <= 0)
                                            {
                                                throw new Exception("Errore durante il ciclo di attivazione passaporti (conferma documento richiedente)");
                                            }
                                        }
                                    }
                                }
                                #endregion

                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Completamento delle pratiche del passaporto.", "ATTIVAZIONIPASSAPORTI", db,
                                    ap.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, ap.IDATTIVAZIONIPASSAPORTI);
                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.ModificaInCompletatoCalendarioEvento(ap.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestePratichePassaporto, db);
                                }

                                using (dtDipendenti dtd = new dtDipendenti())
                                {
                                    using (dtTrasferimento dtt = new dtTrasferimento())
                                    {
                                        using (dtUffici dtu = new dtUffici())
                                        {
                                            var t = dtt.GetTrasferimentoByIdAttPassaporto(idAttivazionePassaporto);

                                            if (t?.idTrasferimento > 0)
                                            {
                                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                                var uff = dtu.GetUffici(t.idUfficio);

                                                EmailTrasferimento.EmailAttiva(t.idTrasferimento,
                                                                    Resources.msgEmail.OggettoRichiestaPratichePassaportoConcluse,
                                                                    string.Format(Resources.msgEmail.MessaggioRichiestaPratichePassaportoConcluse, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString()),
                                                                    db);
                                            }
                                        }
                                    }
                                }
                                //this.EmailCompletaRichiestaPassaporto(ap.IDATTIVAZIONIPASSAPORTI, db);
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

        public void AnnullaRichiestaPassaporto(decimal idAttivazionePassaporto, string testoAnnulla)
        {

            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var apOld = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazionePassaporto);

                    if (apOld?.IDATTIVAZIONIPASSAPORTI > 0)
                    {
                        if (apOld.NOTIFICARICHIESTA == true && apOld.PRATICACONCLUSA == false && apOld.ANNULLATO == false)
                        {
                            apOld.ANNULLATO = true;
                            apOld.DATAAGGIORNAMENTO = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore - Impossibile annullare la notifica della richiesta per il passaporto.");
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Annullamento della riga per il ciclo di attivazione del passaporto",
                                    "ATTIVAZIONIPASSAPORTI", db, apOld.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                    apOld.IDATTIVAZIONIPASSAPORTI);

                                ATTIVAZIONIPASSAPORTI apNew = new ATTIVAZIONIPASSAPORTI()
                                {
                                    IDPASSAPORTI = apOld.IDPASSAPORTI,
                                    NOTIFICARICHIESTA = false,
                                    PRATICACONCLUSA = false,
                                    DATAVARIAZIONE = DateTime.Now,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    ANNULLATO = false,
                                    IDFASEPASSAPORTI=(decimal)EnumFasePassaporti.Richiesta_Passaporti
                                };

                                db.ATTIVAZIONIPASSAPORTI.Add(apNew);

                                int j = db.SaveChanges();

                                if (j <= 0)
                                {
                                    throw new Exception("Errore - Impossibile creare il nuovo ciclo di approvazione per il passaporto.");
                                }
                                else
                                {

                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                        "Inserimento di una nuova riga per il ciclo di attivazione relativo al passaporto.",
                                        "ATTIVAZIONIPASSAPORTI", db, apNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                        apNew.IDATTIVAZIONIPASSAPORTI);

                                    #region Richiedente

                                    var lprOld =
                                        apOld.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false)
                                            .OrderByDescending(a => a.IDPASSAPORTORICHIEDENTE);

                                    if (lprOld?.Any() ?? false)
                                    {
                                        var prOld = lprOld.First();

                                        PASSAPORTORICHIEDENTE prNew = new PASSAPORTORICHIEDENTE()
                                        {
                                            IDPASSAPORTI = prOld.IDPASSAPORTI,
                                            IDATTIVAZIONIPASSAPORTI = apNew.IDATTIVAZIONIPASSAPORTI,
                                            INCLUDIPASSAPORTO = prOld.INCLUDIPASSAPORTO,
                                            DATAAGGIORNAMENTO = prOld.DATAAGGIORNAMENTO,
                                            ANNULLATO = prOld.ANNULLATO
                                        };

                                        apNew.PASSAPORTORICHIEDENTE.Add(prNew);

                                        int k = db.SaveChanges();

                                        if (k <= 0)
                                        {
                                            throw new Exception("Errore - Impossibile inserire i dati del richiedente per il nuovo ciclo di attivazione creato dall'annulla richiesta.");
                                        }
                                        else
                                        {

                                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                "Inserimento di una nuova riga per il richiedente relativo al passaporto.",
                                                "PASSAPORTORICHIEDENTE", db,
                                                prNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                prNew.IDPASSAPORTORICHIEDENTE);

                                            var ldocOld =
                                            prOld.DOCUMENTI.Where(
                                                a =>
                                                    a.MODIFICATO == false &&
                                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                                    a.IDSTATORECORD==(decimal)EnumStatoRecord.Da_Attivare)
                                                .OrderBy(a => a.DATAINSERIMENTO);

                                            if (ldocOld?.Any() ?? false)
                                            {
                                                foreach (var docOld in ldocOld)
                                                {
                                                    DOCUMENTI docNew = new DOCUMENTI()
                                                    {
                                                        IDTIPODOCUMENTO = docOld.IDTIPODOCUMENTO,
                                                        NOMEDOCUMENTO = docOld.NOMEDOCUMENTO,
                                                        ESTENSIONE = docOld.ESTENSIONE,
                                                        FILEDOCUMENTO = docOld.FILEDOCUMENTO,
                                                        DATAINSERIMENTO = docOld.DATAINSERIMENTO,
                                                        MODIFICATO = docOld.MODIFICATO,
                                                        FK_IDDOCUMENTO = docOld.FK_IDDOCUMENTO,
                                                        IDSTATORECORD=(decimal)EnumStatoRecord.In_Lavorazione
                                                    };

                                                    prNew.DOCUMENTI.Add(docNew);
                                                    docOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                                    int y = db.SaveChanges();

                                                    if (y <= 0)
                                                    {
                                                        throw new Exception("Errore - Impossibile associare il documento per il richiedente. (" + docNew.NOMEDOCUMENTO + ")");
                                                    }
                                                    else
                                                    {
                                                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                            "Inserimento di una nuova riga per il documento del richiedente relativo al passaporto.",
                                                            "DOCUMENTI", db,
                                                            prNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                            docNew.IDDOCUMENTO);
                                                    }
                                                }


                                            }
                                        }

                                    }
                                    #endregion

                                    #region Coniuge

                                    var lcpOld =
                                        apOld.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false)
                                            .OrderBy(a => a.IDCONIUGEPASSAPORTO);
                                    if (lcpOld?.Any() ?? false)
                                    {
                                        foreach (var cpOld in lcpOld)
                                        {
                                            CONIUGEPASSAPORTO cpNew = new CONIUGEPASSAPORTO()
                                            {
                                                IDCONIUGE = cpOld.IDCONIUGE,
                                                IDPASSAPORTI = cpOld.IDPASSAPORTI,
                                                IDATTIVAZIONIPASSAPORTI = apNew.IDATTIVAZIONIPASSAPORTI,
                                                INCLUDIPASSAPORTO = cpOld.INCLUDIPASSAPORTO,
                                                DATAAGGIORNAMENTO = cpOld.DATAAGGIORNAMENTO,
                                                ANNULLATO = cpOld.ANNULLATO
                                            };

                                            apNew.CONIUGEPASSAPORTO.Add(cpNew);

                                            int x = db.SaveChanges();

                                            if (x <= 0)
                                            {
                                                throw new Exception("Errore - Impossibile inserire il coniuge per il passaporto da annullamento richiesta. (" + cpNew.CONIUGE.NOME + " " + cpNew.CONIUGE.COGNOME + ")");
                                            }
                                            else
                                            {
                                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                                "Inserimento di una nuova riga per il coniuge passaporto relativo al passaporto.",
                                                                "CONIUGEPASSAPORTO", db,
                                                                apNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                                cpNew.IDCONIUGEPASSAPORTO);
                                            }
                                        }


                                    }

                                    #endregion

                                    #region figli

                                    var lfpOld =
                                        apOld.FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false)
                                            .OrderBy(a => a.IDFIGLIPASSAPORTO);

                                    if (lfpOld?.Any() ?? false)
                                    {
                                        foreach (var fpOld in lfpOld)
                                        {
                                            FIGLIPASSAPORTO fpNew = new FIGLIPASSAPORTO()
                                            {
                                                IDFIGLI = fpOld.IDFIGLI,
                                                IDPASSAPORTI = fpOld.IDPASSAPORTI,
                                                IDATTIVAZIONIPASSAPORTI = apNew.IDATTIVAZIONIPASSAPORTI,
                                                INCLUDIPASSAPORTO = fpOld.INCLUDIPASSAPORTO,
                                                DATAAGGIORNAMENTO = fpOld.DATAAGGIORNAMENTO,
                                                ANNULLATO = fpOld.ANNULLATO
                                            };

                                            apNew.FIGLIPASSAPORTO.Add(fpNew);

                                            int z = db.SaveChanges();

                                            if (z <= 0)
                                            {
                                                throw new Exception("Errore - Impossibile inserire i figli per il passaporto da annullamento richiesta.");
                                            }
                                            else
                                            {
                                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                                "Inserimento di una nuova riga per il figlio del richiedente relativo al passaporto.",
                                                                "FIGLIPASSAPORTO", db,
                                                                apNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                                fpNew.IDFIGLIPASSAPORTO);
                                            }
                                        }


                                    }

                                }
                                #endregion

                                EmailTrasferimento.EmailAnnulla(apNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                               Resources.msgEmail.OggettoAnnullaRichiestaPassaporto,
                                                               testoAnnulla,
                                                               db);

                                //this.EmailAnnullaRichiestaPassaporto(apNew.IDATTIVAZIONIPASSAPORTI, db);
                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.AnnullaMessaggioEvento(apNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestePratichePassaporto, db);
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

        public void NotificaRichiestaPassaporto(decimal idAttivazionePassaporto)
        {

            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazionePassaporto);

                    if (ap?.IDATTIVAZIONIPASSAPORTI > 0)
                    {
                        if (ap.NOTIFICARICHIESTA == false)
                        {
                            ap.NOTIFICARICHIESTA = true;
                            ap.DATANOTIFICARICHIESTA = DateTime.Now;
                            ap.DATAVARIAZIONE = DateTime.Now;
                            ap.DATAAGGIORNAMENTO = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore: Impossibile notificare la richiesta per i passaporti.");
                            }
                            else
                            {
                                #region ciclo attivazione doc richiedente
                                var lpr = ap.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false).ToList();
                                if (lpr?.Any()??false)
                                {
                                    foreach(var pr in lpr )
                                    {
                                        var ldpr = pr.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                                        foreach(var dpr in ldpr)
                                        {
                                            dpr.IDSTATORECORD = (decimal)EnumStatoRecord.Da_Attivare;
                                            if(db.SaveChanges()<=0)
                                            {
                                                throw new Exception("Errore durante il ciclo di attivazione passaporti (notifica documento richiedente)");
                                            }
                                        }
                                    }
                                }
                                #endregion


                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Notifica della richiesta per i passaporti.", "ATTIVAZIONIPASSAPORTI", db,
                                    ap.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, ap.IDATTIVAZIONIPASSAPORTI);
                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    CalendarioEventiModel cem = new CalendarioEventiModel()
                                    {
                                        idFunzioneEventi = EnumFunzioniEventi.RichiestePratichePassaporto,
                                        idTrasferimento = ap.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                        DataInizioEvento = DateTime.Now.Date,
                                        DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaPratichePassaporto)).Date,

                                    };

                                    dtce.InsertCalendarioEvento(ref cem, db);
                                }


                                using (dtDipendenti dtd = new dtDipendenti())
                                {
                                    using (dtTrasferimento dtt = new dtTrasferimento())
                                    {
                                        using (dtUffici dtu = new dtUffici())
                                        {
                                            var t = dtt.GetTrasferimentoByIdAttPassaporto(idAttivazionePassaporto);

                                            if (t?.idTrasferimento > 0)
                                            {
                                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                                var uff = dtu.GetUffici(t.idUfficio);

                                                EmailTrasferimento.EmailNotifica(EnumChiamante.Passaporti,
                                                                                t.idTrasferimento,
                                                                                Resources.msgEmail.OggettoRichiestaPratichePassaporto,
                                                                                string.Format(Resources.msgEmail.MessaggioRichiestaPratichePassaporto, dip.cognome + " " + dip.nome + " (" + dip.matricola + ")", t.dataPartenza.ToShortDateString(), uff.descUfficio + " (" + uff.codiceUfficio + ")"),
                                                                                db);
                                            }
                                        }
                                    }
                                }
                                //this.EmailNotificaRichiestaPassaporto(ap.IDATTIVAZIONIPASSAPORTI, db);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Errore nella fase di intercettazione del ciclo di attivazione.");
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
        #endregion


        public void SituazionePassaporto(decimal idAttivazionePassaporto, out bool NotificaRichiesta, out bool AttivazioneRichiesta, out bool AnnullaRichiesta)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazionePassaporto);

                if (ap.NOTIFICARICHIESTA)
                {
                    NotificaRichiesta = true;
                }
                else
                {
                    NotificaRichiesta = false;
                }

                if (ap.PRATICACONCLUSA)
                {
                    AttivazioneRichiesta = true;
                }
                else
                {
                    AttivazioneRichiesta = false;
                }

                if (ap.ANNULLATO)
                {
                    AnnullaRichiesta = true;
                }
                else
                {
                    AnnullaRichiesta = false;
                }

            }
        }

        public PassaportoRichiedenteModel GetPassaportoRichiedenteByID(decimal id)
        {
            PassaportoRichiedenteModel prm = new PassaportoRichiedenteModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var pr = db.PASSAPORTORICHIEDENTE.Find(id);

                prm = new PassaportoRichiedenteModel()
                {
                    idPassaportoRichiedente = pr.IDPASSAPORTORICHIEDENTE,
                    //idPassaporti = pr.IDPASSAPORTI,
                    //EscludiPassaporto = pr.ESCLUDIPASSAPORTO,
                    //DataEscludiPassapor
                    dataAggiornamento = pr.DATAAGGIORNAMENTO,
                    annullato = pr.ANNULLATO
                };
            }

            return prm;
        }


        public void SetEscludiPassaportoRichiedente(decimal idPassaportoRichiedente, ref bool chk)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var pr = db.PASSAPORTORICHIEDENTE.Find(idPassaportoRichiedente);
                //pr.ESCLUDIPASSAPORTO = true;
                //pr.DATAESCLUDIPASSAPORTO = DateTime.Now;
                pr.DATAAGGIORNAMENTO = DateTime.Now;

                int i = db.SaveChanges();

                if (i > 0)
                {
                    //chk = pr.ESCLUDIPASSAPORTO;
                    //decimal idTrasferimento = pr.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO;
                    //Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                    //        "Esclusione del richiedente dalla richiesta del passaporto/visto.", "PASSAPORTORICHIEDENTE", db,
                    //        idTrasferimento, pr.IDPASSAPORTORICHIEDENTE);
                }
                else
                {
                    throw new Exception("Non è stato possibile modificare lo stato di escludi passaporto per il richiedente.");

                }


            }
        }



        public IList<ElencoFamiliariPassaportoModel> GetFamiliariRichiestaPassaportoPartenza(decimal idTrasferimento)
        {
            List<ElencoFamiliariPassaportoModel> lefm = new List<ElencoFamiliariPassaportoModel>();
            ElencoFamiliariPassaportoModel richiedente = new ElencoFamiliariPassaportoModel();
            List<ElencoFamiliariPassaportoModel> lConiuge = new List<ElencoFamiliariPassaportoModel>();
            List<ElencoFamiliariPassaportoModel> lFiglio = new List<ElencoFamiliariPassaportoModel>();

            ATTIVAZIONIPASSAPORTI ap = new ATTIVAZIONIPASSAPORTI();
            PASSAPORTORICHIEDENTE pr = new PASSAPORTORICHIEDENTE();

            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);


                    if (t != null && t.IDTRASFERIMENTO > 0)
                    {
                        var d = t.DIPENDENTI;

                        var mf = t.MAGGIORAZIONIFAMILIARI;
                        var amf = mf.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true && a.ATTIVAZIONEMAGFAM == true).OrderBy(a => a.IDATTIVAZIONEMAGFAM).First();

                        var p = t.PASSAPORTI;

                        if (p != null && p.IDPASSAPORTI > 0)
                        {
                            var lap =
                                p.ATTIVAZIONIPASSAPORTI.Where(
                                a =>
                                    ((a.NOTIFICARICHIESTA == true && a.PRATICACONCLUSA == true && a.ANNULLATO == false) ||
                                     (a.NOTIFICARICHIESTA == false && a.PRATICACONCLUSA == false && a.ANNULLATO == false) ||
                                     (a.NOTIFICARICHIESTA == true && a.PRATICACONCLUSA == false && a.ANNULLATO == false)))
                                .OrderBy(a => a.IDATTIVAZIONIPASSAPORTI);

                            if (!lap?.Any() ?? false)
                            {
                                p.ATTIVAZIONIPASSAPORTI.Add(new ATTIVAZIONIPASSAPORTI()
                                {
                                    IDPASSAPORTI = p.IDPASSAPORTI,
                                    DATAVARIAZIONE = DateTime.Now
                                });

                                int i = db.SaveChanges();

                                if (i <= 0)
                                {
                                    throw new Exception("Errore nella fase di creazione dell'attivazione per la richiesta di passaporto.");
                                }

                                ap = p.ATTIVAZIONIPASSAPORTI.First();
                            }
                            else
                            {
                                ap = lap.First();
                            }

                            decimal ordine=100;

                            #region Richiedente

                            var lpr =
                                ap.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false)
                                    .OrderBy(a => a.IDPASSAPORTORICHIEDENTE);
                            
                            if (!lpr?.Any() ?? false)
                            {
                                pr = new PASSAPORTORICHIEDENTE()
                                {
                                    IDATTIVAZIONIPASSAPORTI = ap.IDATTIVAZIONIPASSAPORTI
                                };

                                ap.PASSAPORTORICHIEDENTE.Add(pr);

                                int i = db.SaveChanges();

                                if (i <= 0)
                                {
                                    throw new Exception("Errore nella fase di creazione della riga per la richiesta di passaporto per il richiedente.");
                                }

                            }
                            else
                            {
                                pr = lpr.First();
                            }

                            richiedente = new ElencoFamiliariPassaportoModel()
                            {
                                idAttivazionePassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                                idFamiliarePassaporto = pr.IDPASSAPORTORICHIEDENTE,
                                nominativo = d.COGNOME + " " + d.NOME,
                                codiceFiscale = "",
                                dataInizio = pr.ATTIVAZIONIPASSAPORTI.PASSAPORTI.TRASFERIMENTO.DATAPARTENZA,
                                dataFine = pr.ATTIVAZIONIPASSAPORTI.PASSAPORTI.TRASFERIMENTO.DATARIENTRO,
                                parentela = EnumParentela.Richiedente,
                                idAltriDati = 0,
                                richiedi = pr.INCLUDIPASSAPORTO,
                                HasDoc = new HasDoc()
                                {
                                    esisteDoc = pr.DOCUMENTI.Where(a => (a.MODIFICATO == false || !a.FK_IDDOCUMENTO.HasValue) && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita && a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)?.Any() ?? false,
                                    tipoDoc = EnumTipoDoc.Documento_Identita
                                },
                                ordinamento=ordine
                            };

                            lefm.Add(richiedente);

                            #endregion

                            #region Coniuge

                            var lc =
                                amf.CONIUGE.Where(
                                    a =>
                                        (a.IDSTATORECORD == (decimal)EnumStatoTraferimento.Attivo || a.FK_IDCONIUGE.HasValue == false) &&
                                        a.IDTIPOLOGIACONIUGE == (decimal)EnumTipologiaConiuge.Residente)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA);

                            if (lc?.Any() ?? false)
                            {
                                foreach (var c in lc)
                                {
                                    ordine++;

                                    var lcp = c.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDCONIUGEPASSAPORTO);

                                    if (!lcp?.Any() ?? false)
                                    {
                                        CONIUGEPASSAPORTO cp = new CONIUGEPASSAPORTO()
                                        {
                                            IDCONIUGE = c.IDCONIUGE,
                                            IDPASSAPORTI = p.IDPASSAPORTI,
                                            IDATTIVAZIONIPASSAPORTI = ap.IDATTIVAZIONIPASSAPORTI,
                                            INCLUDIPASSAPORTO = false
                                        };

                                        c.CONIUGEPASSAPORTO.Add(cp);

                                        int i = db.SaveChanges();

                                        if (i <= 0)
                                        {
                                            throw new Exception("Errore nella fase di prelievo del coniuge per la richiesta di passaporto.");
                                        }

                                        ElencoFamiliariPassaportoModel coniuge = new ElencoFamiliariPassaportoModel()
                                        {
                                            idAttivazionePassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                                            idFamiliarePassaporto = cp.IDCONIUGEPASSAPORTO,
                                            nominativo = c.COGNOME + " " + c.NOME,
                                            codiceFiscale = c.CODICEFISCALE,
                                            dataInizio = c.DATAINIZIOVALIDITA,
                                            dataFine = c.DATAFINEVALIDITA,
                                            parentela = EnumParentela.Coniuge,
                                            idAltriDati = c.ALTRIDATIFAM.Where(a => a.IDSTATORECORD!= (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDALTRIDATIFAM).First().IDALTRIDATIFAM,
                                            richiedi = cp.INCLUDIPASSAPORTO,
                                            HasDoc = new HasDoc()
                                            {
                                                esisteDoc = c.DOCUMENTI.Where(a => (a.MODIFICATO == false || !a.FK_IDDOCUMENTO.HasValue) && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita && a.IDSTATORECORD== (decimal)EnumStatoRecord.Attivato)?.Any() ?? false,
                                                tipoDoc = EnumTipoDoc.Documento_Identita
                                            },
                                            ordinamento=ordine

                                        };

                                        lConiuge.Add(coniuge);

                                    }
                                    else
                                    {
                                        CONIUGEPASSAPORTO cp = lcp.First();
                                        ElencoFamiliariPassaportoModel coniuge = new ElencoFamiliariPassaportoModel()
                                        {
                                            idAttivazionePassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                                            idFamiliarePassaporto = cp.IDCONIUGEPASSAPORTO,
                                            nominativo = c.COGNOME + " " + c.NOME,
                                            codiceFiscale = c.CODICEFISCALE,
                                            dataInizio = c.DATAINIZIOVALIDITA,
                                            dataFine = c.DATAFINEVALIDITA,
                                            parentela = EnumParentela.Coniuge,
                                            idAltriDati = c.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).OrderByDescending(a => a.IDALTRIDATIFAM).First().IDALTRIDATIFAM,
                                            richiedi = cp.INCLUDIPASSAPORTO,
                                            HasDoc = new HasDoc()
                                            {
                                                esisteDoc = c.DOCUMENTI.Where(a => (a.MODIFICATO == false || !a.FK_IDDOCUMENTO.HasValue) && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita)?.Any() ?? false,
                                                tipoDoc = EnumTipoDoc.Documento_Identita
                                            },
                                            ordinamento=ordine

                                        };

                                        lConiuge.Add(coniuge);
                                    }

                                }

                                if (lConiuge?.Any() ?? false)
                                {
                                    lefm.AddRange(lConiuge);
                                }
                            }

                            #endregion

                            #region Figli

                            var lf =
                                amf.FIGLI.Where(
                                    a =>
                                        (a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato || a.FK_IDFIGLI.HasValue == false) &&
                                        (a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.Residente ||
                                         a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.StudenteResidente))
                                    .OrderBy(a => a.DATAINIZIOVALIDITA);

                            if (lf?.Any() ?? false)
                            {
                                foreach (var f in lf)
                                {
                                    ordine++;

                                    var lfp = f.FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDFIGLIPASSAPORTO);

                                    if (!lfp?.Any() ?? false)
                                    {
                                        FIGLIPASSAPORTO fp = new FIGLIPASSAPORTO()
                                        {
                                            IDFIGLI = f.IDFIGLI,
                                            IDPASSAPORTI = p.IDPASSAPORTI,
                                            IDATTIVAZIONIPASSAPORTI = ap.IDATTIVAZIONIPASSAPORTI,
                                            INCLUDIPASSAPORTO = false
                                        };

                                        f.FIGLIPASSAPORTO.Add(fp);

                                        int i = db.SaveChanges();

                                        if (i <= 0)
                                        {
                                            throw new Exception("Errore nella fase di prelievo del figlio per la richiesta di passaporto.");
                                        }

                                        ElencoFamiliariPassaportoModel figlio = new ElencoFamiliariPassaportoModel()
                                        {
                                            idAttivazionePassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                                            idFamiliarePassaporto = fp.IDFIGLIPASSAPORTO,
                                            nominativo = f.COGNOME + " " + f.NOME,
                                            codiceFiscale = f.CODICEFISCALE,
                                            dataInizio = f.DATAINIZIOVALIDITA,
                                            dataFine = f.DATAFINEVALIDITA,
                                            parentela = EnumParentela.Figlio,
                                            idAltriDati = f.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).OrderByDescending(a => a.IDALTRIDATIFAM).First().IDALTRIDATIFAM,
                                            richiedi = fp.INCLUDIPASSAPORTO,
                                            HasDoc = new HasDoc()
                                            {
                                                esisteDoc = f.DOCUMENTI.Where(a => (a.MODIFICATO == false || !a.FK_IDDOCUMENTO.HasValue) && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita && a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)?.Any() ?? false,
                                                tipoDoc = EnumTipoDoc.Documento_Identita
                                            },
                                            ordinamento=ordine
                                        };

                                        lFiglio.Add(figlio);
                                    }
                                    else
                                    {
                                        FIGLIPASSAPORTO fp = lfp.First();

                                        ElencoFamiliariPassaportoModel figlio = new ElencoFamiliariPassaportoModel()
                                        {
                                            idAttivazionePassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                                            idFamiliarePassaporto = fp.IDFIGLIPASSAPORTO,
                                            nominativo = f.COGNOME + " " + f.NOME,
                                            codiceFiscale = f.CODICEFISCALE,
                                            dataInizio = f.DATAINIZIOVALIDITA,
                                            dataFine = f.DATAFINEVALIDITA,
                                            parentela = EnumParentela.Figlio,
                                            idAltriDati = f.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).OrderByDescending(a => a.IDALTRIDATIFAM).First().IDALTRIDATIFAM,
                                            richiedi = fp.INCLUDIPASSAPORTO,
                                            HasDoc = new HasDoc()
                                            {
                                                esisteDoc = f.DOCUMENTI.Where(a => (a.MODIFICATO == false || !a.FK_IDDOCUMENTO.HasValue) && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita && a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)?.Any() ?? false,
                                                tipoDoc = EnumTipoDoc.Documento_Identita
                                            },
                                            ordinamento=ordine
                                        };

                                        lFiglio.Add(figlio);
                                    }

                                }

                                if (lFiglio?.Any() ?? false)
                                {
                                    lefm.AddRange(lFiglio);
                                }
                            }

                            #endregion


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

            return lefm;
        }


        public PassaportoModel GetPassaportoInLavorazioneByIdTrasf(decimal idTrasferimento)
        {
            PassaportoModel pm = new PassaportoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                var p = t.PASSAPORTI;

                pm = new PassaportoModel()
                {
                    idPassaporto = p.IDPASSAPORTI,
                };

            }

            return pm;
        }

        public void PreSetPassaporto(decimal idTrasferimento, ModelDBISE db)
        {

            PASSAPORTI p = new PASSAPORTI()
            {
                IDPASSAPORTI = idTrasferimento,
            };

            db.PASSAPORTI.Add(p);
            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Errore nella fase d'inserimento dei dati per la gestione del passaporto.");
            }
            else
            {
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                    "Inserimento dei dati di gestione del passaporto.", "PASSAPORTI", db, idTrasferimento,
                    p.IDPASSAPORTI);

                using (dtAttivazionePassaporto dtap = new dtAttivazionePassaporto())
                {
                    AttivazionePassaportiModel apm = new AttivazionePassaportiModel()
                    {
                        idPassaporti = p.IDPASSAPORTI,
                        notificaRichiesta = false,
                        praticaConclusa = false,
                        idFasePassaporti=(decimal)EnumFasePassaporti.Richiesta_Passaporti
                    };

                    dtap.SetAttivazioniPassaporti(ref apm, db);

                    PassaportoRichiedenteModel prm = new PassaportoRichiedenteModel()
                    {
                        idPassaporto = p.IDPASSAPORTI,
                        idAttivazionePassaporti = apm.idAttivazioniPassaporti,
                        includiPassaporto = false,
                        dataAggiornamento = DateTime.Now,
                        annullato = false
                    };

                    dtap.SetPassaportoRichiedente(ref prm, db);

                    //dtap.AssociaRichiedente(apm.idAttivazioniPassaporti, prm.idPassaportoRichiedente, db);


                }


            }

        }

        public PassaportoModel GetPassaportoByID(decimal idPassaporto, ModelDBISE db)
        {
            PassaportoModel pm = new PassaportoModel();

            var p = db.PASSAPORTI.Find(idPassaporto);

            pm = new PassaportoModel()
            {
                idPassaporto = p.IDPASSAPORTI,
            };

            return pm;
        }


        public PassaportoModel GetPassaportoByID(decimal idPassaporto)
        {
            PassaportoModel pm = new PassaportoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var p = db.PASSAPORTI.Find(idPassaporto);

                pm = new PassaportoModel()
                {
                    idPassaporto = p.IDPASSAPORTI,

                };
            }

            return pm;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="idFamiliare">Per il coniuge è l'idConiuge, per il figlio è l'idFiglio, per il richiedente è l'id trasferimento o passaporto per via del riferimento uno ad uno.</param>
        /// <param name="parentela"></param>
        /// <returns></returns>
        public ElencoFamiliariPassaportoModel GetDatiForColElencoDoc(decimal idAttivazionePassaporto, decimal idFamiliarePassaporto, EnumParentela parentela)
        {
            ElencoFamiliariPassaportoModel efm = new ElencoFamiliariPassaportoModel();

            using (ModelDBISE db = new ModelDBISE())
            {

                var ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazionePassaporto);

                switch (parentela)
                {
                    case EnumParentela.Coniuge:
                        var lcp =
                            ap.CONIUGEPASSAPORTO.Where(
                                a => a.ANNULLATO == false && a.IDCONIUGEPASSAPORTO == idFamiliarePassaporto);

                        if (lcp?.Any() ?? false)
                        {
                            var cp = lcp.First();
                            var c = cp.CONIUGE;

                            var ad =
                                c.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                    .OrderByDescending(a => a.IDALTRIDATIFAM)
                                    .First();


                            bool EsisteDoc = false;

                            var lDoc = c.DOCUMENTI.Where(
                                a =>
                                    (a.MODIFICATO == false || a.FK_IDDOCUMENTO.HasValue == false) &&
                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita && a.IDSTATORECORD==(decimal)EnumStatoRecord.Attivato);

                            if (lDoc?.Any() ?? false)
                            {
                                EsisteDoc = true;
                            }
                            else
                            {
                                EsisteDoc = false;
                            }

                            HasDoc hasDoc = new HasDoc()
                            {
                                esisteDoc = EsisteDoc,
                                tipoDoc = EnumTipoDoc.Documento_Identita,
                            };

                            efm = new ElencoFamiliariPassaportoModel()
                            {
                                idAttivazionePassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                                idFamiliarePassaporto = cp.IDCONIUGEPASSAPORTO,
                                nominativo = c.COGNOME + " " + c.NOME,
                                codiceFiscale = c.CODICEFISCALE,
                                dataInizio = c.DATAINIZIOVALIDITA,
                                dataFine = c.DATAFINEVALIDITA,
                                parentela = parentela,
                                idAltriDati = ad.IDALTRIDATIFAM,
                                HasDoc = hasDoc,
                                richiedi = cp.INCLUDIPASSAPORTO
                            };
                        }
                        break;
                    case EnumParentela.Figlio:
                        var lfp =
                            ap.FIGLIPASSAPORTO.Where(
                            a => a.ANNULLATO == false && a.IDFIGLIPASSAPORTO == idFamiliarePassaporto);

                        if (lfp?.Any() ?? false)
                        {
                            var fp = lfp.First();
                            var f = fp.FIGLI;

                            var ad =
                                f.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                    .OrderByDescending(a => a.IDALTRIDATIFAM)
                                    .First();

                            bool EsisteDoc = false;

                            var lDoc = f.DOCUMENTI.Where(
                                a =>
                                    (a.MODIFICATO == false || a.FK_IDDOCUMENTO.HasValue == false) &&
                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita && a.IDSTATORECORD== (decimal)EnumStatoRecord.Attivato);

                            if (lDoc?.Any() ?? false)
                            {
                                EsisteDoc = true;
                            }
                            else
                            {
                                EsisteDoc = false;
                            }

                            HasDoc hasDoc = new HasDoc()
                            {
                                esisteDoc = EsisteDoc,
                                tipoDoc = EnumTipoDoc.Documento_Identita,
                            };

                            efm = new ElencoFamiliariPassaportoModel()
                            {
                                idAttivazionePassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                                idFamiliarePassaporto = fp.IDFIGLIPASSAPORTO,
                                nominativo = f.COGNOME + " " + f.NOME,
                                codiceFiscale = f.CODICEFISCALE,
                                dataInizio = f.DATAINIZIOVALIDITA,
                                dataFine = f.DATAFINEVALIDITA,
                                parentela = parentela,
                                idAltriDati = ad.IDALTRIDATIFAM,
                                HasDoc = hasDoc,
                                richiedi = fp.INCLUDIPASSAPORTO
                            };

                        }
                        break;
                    case EnumParentela.Richiedente:
                        var lpr =
                            ap.PASSAPORTORICHIEDENTE.Where(
                                a => a.ANNULLATO == false && a.IDATTIVAZIONIPASSAPORTI == idAttivazionePassaporto);

                        if (lpr?.Any() ?? false)
                        {
                            var pr = lpr.First();
                            var tr = ap.PASSAPORTI.TRASFERIMENTO;
                            var dip = tr.DIPENDENTI;

                            bool EsisteDoc = false;

                            var lDoc = pr.DOCUMENTI.Where(
                                a =>
                                    (a.MODIFICATO == false || a.FK_IDDOCUMENTO.HasValue == false) &&
                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita && a.IDSTATORECORD!= (decimal)EnumStatoRecord.Annullato);

                            if (lDoc?.Any() ?? false)
                            {
                                EsisteDoc = true;
                            }
                            else
                            {
                                EsisteDoc = false;
                            }

                            HasDoc hasDoc = new HasDoc()
                            {
                                esisteDoc = EsisteDoc,
                                tipoDoc = EnumTipoDoc.Documento_Identita,
                            };

                            efm = new ElencoFamiliariPassaportoModel()
                            {
                                idAttivazionePassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                                idFamiliarePassaporto = pr.IDPASSAPORTORICHIEDENTE,
                                nominativo = dip.COGNOME + " " + dip.NOME,
                                codiceFiscale = "---",
                                dataInizio = tr.DATAPARTENZA,
                                dataFine = tr.DATARIENTRO,
                                parentela = parentela,
                                idAltriDati = 0,
                                HasDoc = hasDoc,
                                richiedi = pr.INCLUDIPASSAPORTO
                            };

                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException("parentela");
                }
            }

            return efm;
        }


        public GestPulsantiAttConclModel GestionePulsantiAttivazionePassaporto(decimal idTrasferimento)
        {
            GestPulsantiAttConclModel gp = new GestPulsantiAttConclModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var p = t.PASSAPORTI;
                var lap = p.ATTIVAZIONIPASSAPORTI.OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI);
                if (lap?.Any() ?? false)
                {
                    var ap = lap.First();

                    gp.idAttivazionePassaporto = ap.IDATTIVAZIONIPASSAPORTI;
                    gp.notificaRichiesta = ap.NOTIFICARICHIESTA;
                    gp.praticaConclusa = ap.PRATICACONCLUSA;
                    gp.annullata = ap.ANNULLATO;

                    var lpr = ap.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false && a.INCLUDIPASSAPORTO == true);

                    if (lpr?.Any() ?? false)
                    {
                        gp.richiedenteIncluso = true;
                    }

                    var lcp = ap.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false && a.INCLUDIPASSAPORTO == true);

                    if (lcp?.Any() ?? false)
                    {
                        gp.coniugeIncluso = true;
                    }

                    var lfp = ap.FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false && a.INCLUDIPASSAPORTO == true);

                    if (lfp?.Any() ?? false)
                    {
                        gp.figliIncluso = true;
                    }

                    gp.statoTrasferimento = (EnumStatoTraferimento)t.IDSTATOTRASFERIMENTO;
                }
            }

            return gp;

        }
    }
}
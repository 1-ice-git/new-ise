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

using System.IO;

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

        public void ConfermaRichiestaPassaporto(decimal idAttivazionePassaporto, string testoAttiva)
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
                                        var ldpr = pr.DOCUMENTI.Where(a => a.MODIFICATO == false &&
                                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare)
                                                    .ToList();
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

                                //PreSetPassaporto(ap.IDPASSAPORTI, (decimal)EnumFasePassaporti.Invio_Passaporti, db);

                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Completamento delle pratiche del passaporto.", "ATTIVAZIONIPASSAPORTI", db,
                                    ap.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, ap.IDATTIVAZIONIPASSAPORTI);
                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.ModificaInCompletatoCalendarioEvento(ap.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaPassaporto, db);
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
                                                                    testoAttiva,
                                                                    //string.Format(Resources.msgEmail.MessaggioRichiestaPratichePassaportoConcluse, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString()),
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

        public void ConfermaInvioPassaporto(decimal idAttivazionePassaporto)
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
                                throw new Exception("Errore: Impossibile completare l'approvazione dell'invio del passaporto.");
                            }
                            else
                            {
                                #region ciclo attivazione passaporto richiedente
                                var lpr = ap.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false).ToList();
                                if (lpr?.Any() ?? false)
                                {
                                    foreach (var pr in lpr)
                                    {
                                        var ldpr = pr.DOCUMENTI.Where(
                                                    a => a.MODIFICATO == false &&
                                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto &&
                                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                                        foreach (var dpr in ldpr)
                                        {
                                            dpr.IDSTATORECORD = (decimal)EnumStatoRecord.Attivato;
                                            if (db.SaveChanges() <= 0)
                                            {
                                                throw new Exception("Errore durante il ciclo di attivazione passaporti (conferma invio passaporto richiedente)");
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region ciclo attivazione passaporto coniuge
                                var lcp = ap.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false).ToList();
                                if (lcp?.Any() ?? false)
                                {
                                    foreach (var cp in lcp)
                                    {
                                        var ldcp = cp.DOCUMENTI.Where(
                                                        a =>
                                                                a.MODIFICATO == false &&
                                                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto &&
                                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                                        foreach (var dcp in ldcp)
                                        {
                                            dcp.IDSTATORECORD = (decimal)EnumStatoRecord.Attivato;
                                            if (db.SaveChanges() <= 0)
                                            {
                                                throw new Exception("Errore durante il ciclo di attivazione passaporti (attiva passaporto coniuge)");
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region ciclo attivazione passaporto figli
                                var lfp = ap.FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false).ToList();
                                if (lfp?.Any() ?? false)
                                {
                                    foreach (var fp in lfp)
                                    {
                                        var ldfp = fp.DOCUMENTI.Where(
                                                    a =>
                                                        a.MODIFICATO == false &&
                                                        a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto &&
                                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                                        foreach (var dfp in ldfp)
                                        {
                                            dfp.IDSTATORECORD = (decimal)EnumStatoRecord.Attivato;
                                            if (db.SaveChanges() <= 0)
                                            {
                                                throw new Exception("Errore durante il ciclo di attivazione passaporti (attiva passaporto figli)");
                                            }
                                        }
                                    }
                                }
                                #endregion

                                //PreSetPassaporto(ap.IDPASSAPORTI, (decimal)EnumFasePassaporti.Invio_Passaporti, db);

                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Completamento invio passaporto.", "ATTIVAZIONIPASSAPORTI", db,
                                    ap.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, ap.IDATTIVAZIONIPASSAPORTI);
                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.ModificaInCompletatoCalendarioEvento(ap.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.InvioPassaporto, db);
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
                                                                    Resources.msgEmail.OggettoInvioPratichePassaportoConcluse,
                                                                    string.Format(Resources.msgEmail.MessaggioInvioPratichePassaportoConcluse, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString()),
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
                                    IDFASEPASSAPORTI = (decimal)EnumFasePassaporti.Richiesta_Passaporti
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
                                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare)
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
                                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
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
                                                throw new Exception("Errore - Impossibile inserire il coniuge per il passaporto da annullamento richiesta.");
                                            }
                                            else
                                            {
                                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                                "Inserimento di una nuova riga per il coniuge passaporto relativo al passaporto.",
                                                                "CONIUGEPASSAPORTO", db,
                                                                apNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                                cpNew.IDCONIUGEPASSAPORTO);

                                            }

                                            //riassocia ConiugePassaporto a Coniuge
                                            var lc = cpOld.CONIUGE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                                            foreach (var c in lc)
                                            {
                                                AssociaConiugePassaportoConiuge(cpNew.IDCONIUGEPASSAPORTO, c.IDCONIUGE, db);
                                            }

                                            //riassocia documento identita coniuge
                                            var ldocIdenC_Old =
                                                cpOld.DOCUMENTI.Where(
                                                    a =>
                                                        a.MODIFICATO == false &&
                                                        a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                                    .OrderBy(a => a.DATAINSERIMENTO);
                                            if (ldocIdenC_Old?.Any() ?? false)
                                            {
                                                foreach (var docIdenC_Old in ldocIdenC_Old)
                                                {
                                                    AssociaDocumentoPassaportoConiuge(cpNew.IDCONIUGEPASSAPORTO, docIdenC_Old.IDDOCUMENTO, db);
                                                }
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

                                            //riassocia FigliPassaporto a Figli
                                            var lf = fpOld.FIGLI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                                            foreach (var f in lf)
                                            {
                                                AssociaFigliPassaportoFigli(fpNew.IDFIGLIPASSAPORTO, f.IDFIGLI, db);
                                            }

                                            //riassocia documento identita coniuge
                                            var ldocIdenF_Old =
                                                fpOld.DOCUMENTI.Where(
                                                    a =>
                                                        a.MODIFICATO == false &&
                                                        a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                                    .OrderBy(a => a.DATAINSERIMENTO);
                                            if (ldocIdenF_Old?.Any() ?? false)
                                            {
                                                foreach (var docIdenF_Old in ldocIdenF_Old)
                                                {
                                                    AssociaDocumentoPassaportoFiglio(fpNew.IDFIGLIPASSAPORTO, docIdenF_Old.IDDOCUMENTO, db);
                                                }
                                            }


                                        }


                                    }
                                    #endregion

                                }


                                EmailTrasferimento.EmailAnnulla(apNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                               Resources.msgEmail.OggettoAnnullaRichiestaPassaporto,
                                                               testoAnnulla,
                                                               db);

                                //this.EmailAnnullaRichiestaPassaporto(apNew.IDATTIVAZIONIPASSAPORTI, db);
                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.AnnullaMessaggioEvento(apNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaPassaporto, db);
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

        public void AnnullaInvioPassaporto(decimal idAttivazionePassaporto, string testoAnnulla)
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
                                throw new Exception("Errore - Impossibile annullare la notifica del'invio per il passaporto.");
                            }
                            else
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Annullamento della riga per il ciclo di attivazione dell'invio passaporto",
                                    "ATTIVAZIONIPASSAPORTI", db, apOld.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                    apOld.IDATTIVAZIONIPASSAPORTI);

                            #region attivazione
                            ATTIVAZIONIPASSAPORTI apNew = new ATTIVAZIONIPASSAPORTI()
                            {
                                IDPASSAPORTI = apOld.IDPASSAPORTI,
                                NOTIFICARICHIESTA = false,
                                PRATICACONCLUSA = false,
                                DATAVARIAZIONE = DateTime.Now,
                                DATAAGGIORNAMENTO = DateTime.Now,
                                ANNULLATO = false,
                                IDFASEPASSAPORTI = (decimal)EnumFasePassaporti.Invio_Passaporti
                            };

                            db.ATTIVAZIONIPASSAPORTI.Add(apNew);

                            int j = db.SaveChanges();

                            if (j <= 0)
                            {
                                throw new Exception("Errore - Impossibile creare il nuovo ciclo di invio passaporto.");
                            }

                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                "Inserimento di una nuova riga per il ciclo di attivazione per invio passaporto.",
                                "ATTIVAZIONIPASSAPORTI", db, apNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                apNew.IDATTIVAZIONIPASSAPORTI);

                            #endregion

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
                                    throw new Exception("Errore - Impossibile inserire i dati del richiedente per il nuovo ciclo di attivazione creato dall'annulla richiesta invio passaporti.");
                                }

                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                    "Inserimento di una nuova riga per il richiedente relativo a invio passaporto.",
                                    "PASSAPORTORICHIEDENTE", db,
                                    prNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                    prNew.IDPASSAPORTORICHIEDENTE);


                                //annulla documento passaporto richiedente
                                var ldocPassR_Old =
                                prOld.DOCUMENTI.Where(
                                    a =>
                                        a.MODIFICATO == false &&
                                        a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto &&
                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare)
                                    .OrderBy(a => a.DATAINSERIMENTO);

                                if (ldocPassR_Old?.Any() ?? false)
                                {
                                    foreach (var docPassR_Old in ldocPassR_Old)
                                    {
                                        DOCUMENTI docPassR_New = new DOCUMENTI()
                                        {
                                            IDTIPODOCUMENTO = docPassR_Old.IDTIPODOCUMENTO,
                                            NOMEDOCUMENTO = docPassR_Old.NOMEDOCUMENTO,
                                            ESTENSIONE = docPassR_Old.ESTENSIONE,
                                            FILEDOCUMENTO = docPassR_Old.FILEDOCUMENTO,
                                            DATAINSERIMENTO = docPassR_Old.DATAINSERIMENTO,
                                            MODIFICATO = docPassR_Old.MODIFICATO,
                                            FK_IDDOCUMENTO = docPassR_Old.FK_IDDOCUMENTO,
                                            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                        };

                                        prNew.DOCUMENTI.Add(docPassR_New);
                                        docPassR_Old.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                        int y = db.SaveChanges();

                                        if (y <= 0)
                                        {
                                            throw new Exception("Errore - Impossibile associare il documento per il richiedente. (" + docPassR_New.NOMEDOCUMENTO + ")");
                                        }
                                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                            "Inserimento di una nuova riga per il documento del richiedente relativo al passaporto.",
                                            "DOCUMENTI", db,
                                            prNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                            docPassR_New.IDDOCUMENTO);
                                    }
                                }

                                //riassocia documento identita richiedente
                                var ldocIdenR_Old =
                                    prOld.DOCUMENTI.Where(
                                        a =>
                                            a.MODIFICATO == false &&
                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                        .OrderBy(a => a.DATAINSERIMENTO);
                                if (ldocIdenR_Old?.Any() ?? false)
                                {
                                    foreach (var docIdenR_Old in ldocIdenR_Old)
                                    {
                                        AssociaDocumentoPassaportoRichiedente(prNew.IDPASSAPORTORICHIEDENTE, docIdenR_Old.IDDOCUMENTO, db);
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
                                        throw new Exception("Errore - Impossibile inserire il coniuge per il passaporto da annullamento richiesta invio passaporto.");
                                    }

                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                            "Inserimento di una nuova riga per il coniuge passaporto relativo all'invio passaporto.",
                                                            "CONIUGEPASSAPORTO", db,
                                                            apNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                            cpNew.IDCONIUGEPASSAPORTO);

                                    var lc = cpOld.CONIUGE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                                    foreach (var c in lc)
                                    {
                                        AssociaConiugePassaportoConiuge(cpNew.IDCONIUGEPASSAPORTO, c.IDCONIUGE, db);
                                    }

                                    var ldocPassC_Old =
                                        cpOld.DOCUMENTI.Where(
                                                        a =>
                                                            a.MODIFICATO == false &&
                                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto &&
                                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare)
                                                        .OrderBy(a => a.DATAINSERIMENTO);

                                    if (ldocPassC_Old?.Any() ?? false)
                                    {
                                        foreach (var docPassC_Old in ldocPassC_Old)
                                        {
                                            DOCUMENTI docPassC_New = new DOCUMENTI()
                                            {
                                                IDTIPODOCUMENTO = docPassC_Old.IDTIPODOCUMENTO,
                                                NOMEDOCUMENTO = docPassC_Old.NOMEDOCUMENTO,
                                                ESTENSIONE = docPassC_Old.ESTENSIONE,
                                                FILEDOCUMENTO = docPassC_Old.FILEDOCUMENTO,
                                                DATAINSERIMENTO = docPassC_Old.DATAINSERIMENTO,
                                                MODIFICATO = docPassC_Old.MODIFICATO,
                                                FK_IDDOCUMENTO = docPassC_Old.FK_IDDOCUMENTO,
                                                IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                            };

                                            cpNew.DOCUMENTI.Add(docPassC_New);
                                            docPassC_Old.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                            int y = db.SaveChanges();

                                            if (y <= 0)
                                            {
                                                throw new Exception("Errore - Impossibile associare il passaporto per il coniuge. (" + docPassC_New.NOMEDOCUMENTO + ")");
                                            }
                                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                "Inserimento di una nuova riga per il passaporto del coniuge relativo all'invio passaporto.",
                                                "DOCUMENTI", db,
                                                cpNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                docPassC_New.IDDOCUMENTO);
                                        }
                                    }

                                    //riassocia i documenti identita coniuge
                                    var ldocIdenC_Old = cpOld.DOCUMENTI.Where(a =>
                                            a.MODIFICATO == false &&
                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                                .OrderBy(a => a.DATAINSERIMENTO);

                                    if (ldocIdenC_Old?.Any() ?? false)
                                    {
                                        foreach (var docIdenC_Old in ldocIdenC_Old)
                                        {
                                            AssociaDocumentoPassaportoConiuge(cpNew.IDCONIUGEPASSAPORTO, docIdenC_Old.IDDOCUMENTO, db);
                                        }

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
                                        throw new Exception("Errore - Impossibile inserire i figli per il passaporto da annullamento richiesta ivio passaporto.");
                                    }

                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                                "Inserimento di una nuova riga per il figlio del richiedente relativo all'invio passaporto.",
                                                                "FIGLIPASSAPORTO", db,
                                                                apNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                                fpNew.IDFIGLIPASSAPORTO);

                                    var lf = fpOld.FIGLI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                                    foreach (var f in lf)
                                    {
                                        AssociaFigliPassaportoFigli(fpNew.IDFIGLIPASSAPORTO, f.IDFIGLI, db);
                                    }

                                    var ldocPassF_Old =
                                        fpOld.DOCUMENTI.Where(
                                            a =>
                                                a.MODIFICATO == false &&
                                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto &&
                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare)
                                            .OrderBy(a => a.DATAINSERIMENTO);

                                    if (ldocPassF_Old?.Any() ?? false)
                                    {
                                        foreach (var docPassF_Old in ldocPassF_Old)
                                        {
                                            DOCUMENTI docPassF_New = new DOCUMENTI()
                                            {
                                                IDTIPODOCUMENTO = docPassF_Old.IDTIPODOCUMENTO,
                                                NOMEDOCUMENTO = docPassF_Old.NOMEDOCUMENTO,
                                                ESTENSIONE = docPassF_Old.ESTENSIONE,
                                                FILEDOCUMENTO = docPassF_Old.FILEDOCUMENTO,
                                                DATAINSERIMENTO = docPassF_Old.DATAINSERIMENTO,
                                                MODIFICATO = docPassF_Old.MODIFICATO,
                                                FK_IDDOCUMENTO = docPassF_Old.FK_IDDOCUMENTO,
                                                IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                            };

                                            fpNew.DOCUMENTI.Add(docPassF_New);
                                            docPassF_Old.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                            int y = db.SaveChanges();

                                            if (y <= 0)
                                            {
                                                throw new Exception("Errore - Impossibile associare il passaporto per il figlio. (" + docPassF_New.NOMEDOCUMENTO + ")");
                                            }

                                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                "Inserimento di una nuova riga per il passaporto del figlio relativo all'invio passaporto.",
                                                "DOCUMENTI", db,
                                                fpNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                docPassF_New.IDDOCUMENTO);
                                        }
                                    }


                                    //riassocio i documenti identita figli
                                    var ldocIdenF_Old =
                                        fpOld.DOCUMENTI.Where(
                                            a =>
                                                a.MODIFICATO == false &&
                                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                            .OrderBy(a => a.DATAINSERIMENTO);

                                    if (ldocIdenF_Old?.Any() ?? false)
                                    {
                                        foreach (var docIdenF_Old in ldocIdenF_Old)
                                        {
                                            AssociaDocumentoPassaportoFiglio(fpNew.IDFIGLIPASSAPORTO, docIdenF_Old.IDDOCUMENTO, db);
                                        }
                                    }
                                }
                            }
                            #endregion

                            EmailTrasferimento.EmailAnnulla(apNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                        Resources.msgEmail.OggettoAnnullaInvioPassaporto,
                                        testoAnnulla,
                                        db);

                            //this.EmailAnnullaRichiestaPassaporto(apNew.IDATTIVAZIONIPASSAPORTI, db);
                            using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                            {
                                dtce.AnnullaMessaggioEvento(apNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.InvioPassaporto, db);
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

                                #region ciclo attivazione documenti richiedente
                                var lpr = ap.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false).ToList();
                                if (lpr?.Any() ?? false)
                                {
                                    foreach (var pr in lpr)
                                    {
                                        var ldpr = pr.DOCUMENTI.Where(a => a.MODIFICATO == false &&
                                                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                                        foreach (var dpr in ldpr)
                                        {
                                            dpr.IDSTATORECORD = (decimal)EnumStatoRecord.Da_Attivare;
                                            if (db.SaveChanges() <= 0)
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
                                        idFunzioneEventi = EnumFunzioniEventi.RichiestaPassaporto,
                                        idTrasferimento = ap.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                        DataInizioEvento = DateTime.Now.Date,
                                        DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaPassaporto)).Date,

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

        public void NotificaInvioPassaporto(decimal idAttivazionePassaporto)
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
                                throw new Exception("Errore: Impossibile notificare l'invio dei passaporti.");
                            }
                            else
                            {
                                #region ciclo attivazione passaporto richiedente
                                var lpr = ap.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false).ToList();
                                if (lpr?.Any() ?? false)
                                {
                                    foreach (var pr in lpr)
                                    {
                                        var ldpr = pr.DOCUMENTI.Where(a => a.MODIFICATO == false &&
                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto &&
                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                                        foreach (var dpr in ldpr)
                                        {
                                            dpr.IDSTATORECORD = (decimal)EnumStatoRecord.Da_Attivare;
                                            if (db.SaveChanges() <= 0)
                                            {
                                                throw new Exception("Errore durante il ciclo di attivazione passaporti (notifica invio passaporto richiedente)");
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region ciclo attivazione passaporto coniuge
                                var lcp = ap.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false).ToList();
                                if (lcp?.Any() ?? false)
                                {
                                    foreach (var cp in lcp)
                                    {
                                        var ldcp = cp.DOCUMENTI.Where(a => a.MODIFICATO == false &&
                                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto &&
                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                                        foreach (var dcp in ldcp)
                                        {
                                            dcp.IDSTATORECORD = (decimal)EnumStatoRecord.Da_Attivare;
                                            if (db.SaveChanges() <= 0)
                                            {
                                                throw new Exception("Errore durante il ciclo di attivazione passaporti (notifica invio passaporto coniuge)");
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region ciclo attivazione passaporto figli
                                var lfp = ap.FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false).ToList();
                                if (lfp?.Any() ?? false)
                                {
                                    foreach (var fp in lfp)
                                    {
                                        var ldfp = fp.DOCUMENTI.Where(a => a.MODIFICATO == false &&
                                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto &&
                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                                        foreach (var dfp in ldfp)
                                        {
                                            dfp.IDSTATORECORD = (decimal)EnumStatoRecord.Da_Attivare;
                                            if (db.SaveChanges() <= 0)
                                            {
                                                throw new Exception("Errore durante il ciclo di attivazione passaporti (notifica invio passaporto figli)");
                                            }
                                        }
                                    }
                                }
                                #endregion


                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Notifica invio passaporti.", "ATTIVAZIONIPASSAPORTI", db,
                                    ap.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, ap.IDATTIVAZIONIPASSAPORTI);
                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    CalendarioEventiModel cem = new CalendarioEventiModel()
                                    {
                                        idFunzioneEventi = EnumFunzioniEventi.InvioPassaporto,
                                        idTrasferimento = ap.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                        DataInizioEvento = DateTime.Now.Date,
                                        DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.InvioPassaporto)).Date,

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
                                                                                Resources.msgEmail.OggettoInvioPratichePassaporto,
                                                                                string.Format(Resources.msgEmail.MessaggioInvioPratichePassaporto, dip.cognome + " " + dip.nome + " (" + dip.matricola + ")", t.dataPartenza.ToShortDateString(), uff.descUfficio + " (" + uff.codiceUfficio + ")"),
                                                                                db);
                                            }
                                        }
                                    }
                                }
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
            CONIUGEPASSAPORTO cp = new CONIUGEPASSAPORTO();
            FIGLIPASSAPORTO fp = new FIGLIPASSAPORTO();

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
                                    (((a.NOTIFICARICHIESTA == true && a.PRATICACONCLUSA == true && a.ANNULLATO == false) ||
                                     (a.NOTIFICARICHIESTA == false && a.PRATICACONCLUSA == false && a.ANNULLATO == false) ||
                                     (a.NOTIFICARICHIESTA == true && a.PRATICACONCLUSA == false && a.ANNULLATO == false))) &&
                                     a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Richiesta_Passaporti)
                                .OrderBy(a => a.IDATTIVAZIONIPASSAPORTI);

                            if (!lap?.Any() ?? false)
                            {
                                p.ATTIVAZIONIPASSAPORTI.Add(new ATTIVAZIONIPASSAPORTI()
                                {
                                    IDPASSAPORTI = p.IDPASSAPORTI,
                                    DATAVARIAZIONE = DateTime.Now,
                                    IDFASEPASSAPORTI = (decimal)EnumFasePassaporti.Richiesta_Passaporti
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


                            decimal ordine = 100;

                            #region Richiedente 

                            var lpr =
                                ap.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false)
                                    .OrderBy(a => a.IDPASSAPORTORICHIEDENTE);

                            if (!lpr?.Any() ?? false)
                            {
                                pr = new PASSAPORTORICHIEDENTE()
                                {
                                    IDATTIVAZIONIPASSAPORTI = ap.IDATTIVAZIONIPASSAPORTI,
                                    INCLUDIPASSAPORTO = false,
                                    IDPASSAPORTI = p.IDPASSAPORTI,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    ANNULLATO = false
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
                                ordinamento = ordine
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
                                        cp = new CONIUGEPASSAPORTO()
                                        {
                                            IDPASSAPORTI = p.IDPASSAPORTI,
                                            IDATTIVAZIONIPASSAPORTI = ap.IDATTIVAZIONIPASSAPORTI,
                                            INCLUDIPASSAPORTO = false,
                                            DATAAGGIORNAMENTO = DateTime.Now,
                                            ANNULLATO = false
                                        };

                                        ap.CONIUGEPASSAPORTO.Add(cp);

                                        int i = db.SaveChanges();

                                        if (i <= 0)
                                        {
                                            throw new Exception("Errore nella fase di prelievo del coniuge per la richiesta di passaporto.");
                                        }

                                        AssociaConiugePassaportoConiuge(cp.IDCONIUGEPASSAPORTO, c.IDCONIUGE, db);

                                        var lDocIdentita = c.DOCUMENTI.Where(a =>
                                                (a.MODIFICATO == false || !a.FK_IDDOCUMENTO.HasValue) &&
                                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                                        if (lDocIdentita.Count > 0)
                                        {
                                            foreach (var docIdentita in lDocIdentita)
                                            {
                                                this.AssociaDocumentoPassaportoConiuge(cp.IDCONIUGEPASSAPORTO, docIdentita.IDDOCUMENTO, db);
                                            }
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
                                            idAltriDati = c.ALTRIDATIFAM.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDALTRIDATIFAM).First().IDALTRIDATIFAM,
                                            richiedi = cp.INCLUDIPASSAPORTO,
                                            HasDoc = new HasDoc()
                                            {
                                                esisteDoc = (lDocIdentita.Count > 0) ? true : false,
                                                tipoDoc = EnumTipoDoc.Documento_Identita
                                            },

                                            ordinamento = ordine,


                                        };

                                        lConiuge.Add(coniuge);

                                    }
                                    else
                                    {

                                        cp = lcp.First();

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
                                                esisteDoc = cp.DOCUMENTI.Where(a => (a.MODIFICATO == false || !a.FK_IDDOCUMENTO.HasValue) && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita)?.Any() ?? false,
                                                tipoDoc = EnumTipoDoc.Documento_Identita
                                            },
                                            ordinamento = ordine,
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

                                    var lfp = f.FIGLIPASSAPORTO.Where(a =>
                                                        a.ANNULLATO == false
                                                        ).OrderByDescending(a => a.IDFIGLIPASSAPORTO);

                                    if (!lfp?.Any() ?? false)
                                    {
                                        fp = new FIGLIPASSAPORTO()
                                        {
                                            IDPASSAPORTI = p.IDPASSAPORTI,
                                            IDATTIVAZIONIPASSAPORTI = ap.IDATTIVAZIONIPASSAPORTI,
                                            INCLUDIPASSAPORTO = false,
                                            DATAAGGIORNAMENTO = DateTime.Now,
                                            ANNULLATO = false
                                        };

                                        ap.FIGLIPASSAPORTO.Add(fp);

                                        int i = db.SaveChanges();

                                        if (i <= 0)
                                        {
                                            throw new Exception("Errore nella fase di prelievo del figlio per la richiesta di passaporto.");
                                        }

                                        AssociaFigliPassaportoFigli(fp.IDFIGLIPASSAPORTO, f.IDFIGLI, db);

                                        var lDocIdentita = f.DOCUMENTI.Where(a =>
                                               (a.MODIFICATO == false || !a.FK_IDDOCUMENTO.HasValue) &&
                                               a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                               a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                                        if (lDocIdentita.Count > 0)
                                        {
                                            foreach (var docIdentita in lDocIdentita)
                                            {
                                                this.AssociaDocumentoPassaportoFiglio(fp.IDFIGLIPASSAPORTO, docIdentita.IDDOCUMENTO, db);
                                            }
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
                                                esisteDoc = (lDocIdentita.Count > 0) ? true : false,// f.DOCUMENTI.Where(a => (a.MODIFICATO == false || !a.FK_IDDOCUMENTO.HasValue) && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita && a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)?.Any() ?? false,
                                                tipoDoc = EnumTipoDoc.Documento_Identita
                                            },
                                            ordinamento = ordine,

                                        };

                                        lFiglio.Add(figlio);
                                    }
                                    else
                                    {

                                        fp = lfp.First();


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
                                                esisteDoc = fp.DOCUMENTI.Where(a => (a.MODIFICATO == false || !a.FK_IDDOCUMENTO.HasValue) && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita && a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)?.Any() ?? false,
                                                tipoDoc = EnumTipoDoc.Documento_Identita
                                            },
                                            ordinamento = ordine,

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


        public IList<ElencoFamiliariPassaportoModel> GetFamiliariInvioPassaportoPartenza(decimal idTrasferimento)
        {
            List<ElencoFamiliariPassaportoModel> lefm = new List<ElencoFamiliariPassaportoModel>();
            ElencoFamiliariPassaportoModel richiedente = new ElencoFamiliariPassaportoModel();
            List<ElencoFamiliariPassaportoModel> lConiuge = new List<ElencoFamiliariPassaportoModel>();
            List<ElencoFamiliariPassaportoModel> lFiglio = new List<ElencoFamiliariPassaportoModel>();

            ATTIVAZIONIPASSAPORTI ap_invio = new ATTIVAZIONIPASSAPORTI();
            ATTIVAZIONIPASSAPORTI ap_richiesta = new ATTIVAZIONIPASSAPORTI();
            PASSAPORTORICHIEDENTE pr = new PASSAPORTORICHIEDENTE();
            PASSAPORTORICHIEDENTE pr_richiesta = new PASSAPORTORICHIEDENTE();
            decimal idDocPassaporto = 0;
            decimal idDocIdentita = 0;
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
                            #region attivazione
                            //legge id della fase di richiesta
                            var lap_richiesta =
                                p.ATTIVAZIONIPASSAPORTI.Where(a =>
                                            a.NOTIFICARICHIESTA == true &&
                                            a.PRATICACONCLUSA == true &&
                                            a.ANNULLATO == false &&
                                            a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Richiesta_Passaporti)
                                            .OrderBy(a => a.IDATTIVAZIONIPASSAPORTI);

                            if (lap_richiesta?.Any() ?? false)
                            {
                                ap_richiesta = lap_richiesta.First();
                            }
                            else
                            {
                                throw new Exception("Fase Richiesta passaporti non trovata.");
                            }

                            var lap_invio =
                                p.ATTIVAZIONIPASSAPORTI.Where(
                                a =>
                                    (((a.NOTIFICARICHIESTA == true && a.PRATICACONCLUSA == true && a.ANNULLATO == false) ||
                                        (a.NOTIFICARICHIESTA == false && a.PRATICACONCLUSA == false && a.ANNULLATO == false) ||
                                        (a.NOTIFICARICHIESTA == true && a.PRATICACONCLUSA == false && a.ANNULLATO == false))) &&
                                        a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Invio_Passaporti)
                                .OrderBy(a => a.IDATTIVAZIONIPASSAPORTI);

                            if (!lap_invio?.Any() ?? false)
                            {
                                var ap_invio_new = new ATTIVAZIONIPASSAPORTI()
                                {
                                    IDPASSAPORTI = p.IDPASSAPORTI,
                                    DATAVARIAZIONE = DateTime.Now,
                                    IDFASEPASSAPORTI = (decimal)EnumFasePassaporti.Invio_Passaporti
                                };

                                p.ATTIVAZIONIPASSAPORTI.Add(ap_invio_new);

                                int i = db.SaveChanges();

                                if (i <= 0)
                                {
                                    throw new Exception("Errore nella fase di creazione dell'attivazione per l'invio del passaporto.");
                                }

                                ap_invio = ap_invio_new;
                            }
                            else
                            {
                                ap_invio = lap_invio.First();
                            }

                            #endregion

                            decimal ordine = 100;

                            #region Richiedente 
                            //verifico se esiste richiesta richiedente
                            var lpr_richiesta =
                                ap_richiesta.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false && a.INCLUDIPASSAPORTO == true)
                                .OrderBy(a => a.IDPASSAPORTORICHIEDENTE);
                            if (lpr_richiesta?.Any() ?? false)
                            {
                                //se esiste verifico se esiste il relativo record della fase 2
                                var lpr =
                                    ap_invio.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false && a.INCLUDIPASSAPORTO == true)
                                    .OrderBy(a => a.IDPASSAPORTORICHIEDENTE);
                                //se non esiste lo creo
                                if (!lpr?.Any() ?? false)
                                {
                                    pr_richiesta = lpr_richiesta.First();
                                    pr = new PASSAPORTORICHIEDENTE()
                                    {
                                        IDPASSAPORTI = pr_richiesta.IDPASSAPORTI,
                                        IDATTIVAZIONIPASSAPORTI = ap_invio.IDATTIVAZIONIPASSAPORTI,
                                        INCLUDIPASSAPORTO = pr_richiesta.INCLUDIPASSAPORTO,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        ANNULLATO = pr_richiesta.ANNULLATO
                                    };
                                    ap_invio.PASSAPORTORICHIEDENTE.Add(pr);
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante la creazione del passaporto richiedente per la fase di invio passaporto.");
                                    }
                                    //associa anche il documento di identita al nuovo record passaportorichiedente
                                    //(leggo l'id del documento)
                                    pr_richiesta = lpr_richiesta.First();

                                    var id_doc_richiedente = pr_richiesta.DOCUMENTI.First().IDDOCUMENTO;
                                    this.AssociaDocumentoPassaportoRichiedente(pr.IDPASSAPORTORICHIEDENTE, id_doc_richiedente, db);

                                }
                                else
                                {
                                    pr = lpr.First();
                                    pr_richiesta = lpr_richiesta.First();
                                }

                                idDocPassaporto = GetIdDocFamiliare((decimal)EnumTipoDoc.Passaporto, ap_invio.IDATTIVAZIONIPASSAPORTI, pr.IDPASSAPORTORICHIEDENTE, (decimal)EnumParentela.Richiedente, db);
                                idDocIdentita = GetIdDocFamiliare((decimal)EnumTipoDoc.Documento_Identita, ap_invio.IDATTIVAZIONIPASSAPORTI, pr.IDPASSAPORTORICHIEDENTE, (decimal)EnumParentela.Richiedente, db);
                                richiedente = new ElencoFamiliariPassaportoModel()
                                {
                                    idAttivazionePassaporti = ap_invio.IDATTIVAZIONIPASSAPORTI,
                                    idFamiliarePassaporto = pr.IDPASSAPORTORICHIEDENTE,
                                    nominativo = d.COGNOME + " " + d.NOME,
                                    codiceFiscale = "",
                                    dataInizio = pr.ATTIVAZIONIPASSAPORTI.PASSAPORTI.TRASFERIMENTO.DATAPARTENZA,
                                    dataFine = pr.ATTIVAZIONIPASSAPORTI.PASSAPORTI.TRASFERIMENTO.DATARIENTRO,
                                    parentela = EnumParentela.Richiedente,
                                    idAltriDati = 0,
                                    notificato = ap_invio.NOTIFICARICHIESTA,
                                    attivato = ap_invio.PRATICACONCLUSA,
                                    HasDoc = new HasDoc()
                                    {
                                        //esisteDoc = pr_richiesta.DOCUMENTI.Where(a => (a.MODIFICATO == false || !a.FK_IDDOCUMENTO.HasValue) && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita && a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)?.Any() ?? false,
                                        esisteDoc = (idDocIdentita > 0) ? true : false,
                                        tipoDoc = EnumTipoDoc.Documento_Identita
                                    },
                                    HasDocPassaporto = new HasDocPassaporto()
                                    {
                                        tipoDocPassaporto = EnumTipoDoc.Passaporto,
                                        idDocPassaporto = idDocPassaporto,
                                        esisteDocPassaporto = (idDocPassaporto > 0) ? true : false
                                    },
                                    ordinamento = ordine
                                    //idAttivazioneInvioPassaporti=ap_invio.IDATTIVAZIONIPASSAPORTI,
                                    //idFamiliareInvioPassaporto=pr_invio.IDPASSAPORTORICHIEDENTE
                                };

                                lefm.Add(richiedente);
                            }

                            #endregion

                            #region Coniuge
                            //verifico se esistono richieste coniuge
                            var lcp_richiesta =
                                ap_richiesta.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false && a.INCLUDIPASSAPORTO == true)
                                .OrderBy(a => a.IDCONIUGEPASSAPORTO);
                            if (lcp_richiesta?.Any() ?? false)
                            {
                                //se esistono verifico se esistono i relativi record della fase 2
                                var lcp =
                                    ap_invio.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false && a.INCLUDIPASSAPORTO == true)
                                    .OrderBy(a => a.IDCONIUGEPASSAPORTO);
                                //se non esistono li creo
                                if (!lcp?.Any() ?? false)
                                {
                                    foreach (var cp_richiesta in lcp_richiesta)
                                    {
                                        var cp_new = new CONIUGEPASSAPORTO()
                                        {
                                            IDPASSAPORTI = cp_richiesta.IDPASSAPORTI,
                                            IDATTIVAZIONIPASSAPORTI = ap_invio.IDATTIVAZIONIPASSAPORTI,
                                            INCLUDIPASSAPORTO = cp_richiesta.INCLUDIPASSAPORTO,
                                            DATAAGGIORNAMENTO = DateTime.Now,
                                            ANNULLATO = cp_richiesta.ANNULLATO
                                        };
                                        ap_invio.CONIUGEPASSAPORTO.Add(cp_new);
                                        if (db.SaveChanges() <= 0)
                                        {
                                            throw new Exception("Errore durante la creazione del passaporto coniuge per la fase di invio passaporto.");
                                        }
                                        var c = cp_richiesta.CONIUGE.First();
                                        idDocIdentita = GetIdDocFamiliare((decimal)EnumTipoDoc.Documento_Identita, ap_richiesta.IDATTIVAZIONIPASSAPORTI, c.IDCONIUGE, (decimal)EnumParentela.Coniuge, db);
                                        AssociaDocumentoPassaportoConiuge(cp_new.IDCONIUGEPASSAPORTO, idDocIdentita, db);

                                        AssociaConiugePassaportoConiuge(cp_new.IDCONIUGEPASSAPORTO, c.IDCONIUGE, db);


                                    }
                                    //rileggo i record appena creati
                                    lcp =
                                        ap_invio.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false && a.INCLUDIPASSAPORTO == true)
                                        .OrderBy(a => a.IDCONIUGEPASSAPORTO);
                                }


                                foreach (var cp in lcp)
                                {
                                    ordine++;
                                    var c = cp.CONIUGE.First();
                                    //ConiugeModel cm = new ConiugeModel();
                                    //using (dtConiuge dtc = new dtConiuge())
                                    //{
                                    //    cm = dtc.GetConiugebyID(cp.IDCONIUGE);
                                    //}

                                    //idDocPassaporto = GetIdDocPassaportoFamiliare(ap_invio.IDATTIVAZIONIPASSAPORTI, cp.IDCONIUGE, (decimal)EnumParentela.Coniuge, db);
                                    idDocPassaporto = GetIdDocFamiliare((decimal)EnumTipoDoc.Passaporto, ap_invio.IDATTIVAZIONIPASSAPORTI, c.IDCONIUGE, (decimal)EnumParentela.Coniuge, db);
                                    idDocIdentita = GetIdDocFamiliare((decimal)EnumTipoDoc.Documento_Identita, ap_invio.IDATTIVAZIONIPASSAPORTI, c.IDCONIUGE, (decimal)EnumParentela.Coniuge, db);


                                    ElencoFamiliariPassaportoModel coniuge = new ElencoFamiliariPassaportoModel()
                                    {
                                        idAttivazionePassaporti = ap_invio.IDATTIVAZIONIPASSAPORTI,
                                        idFamiliarePassaporto = cp.IDCONIUGEPASSAPORTO,
                                        nominativo = c.COGNOME + " " + c.NOME,
                                        codiceFiscale = c.CODICEFISCALE,
                                        dataInizio = c.DATAINIZIOVALIDITA,
                                        dataFine = c.DATAFINEVALIDITA,
                                        parentela = EnumParentela.Coniuge,
                                        idAltriDati = c.ALTRIDATIFAM.Where(a =>
                                                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                                        .OrderByDescending(a => a.IDALTRIDATIFAM).First()
                                                        .IDALTRIDATIFAM,
                                        notificato = ap_invio.NOTIFICARICHIESTA,
                                        attivato = ap_invio.PRATICACONCLUSA,
                                        HasDoc = new HasDoc()
                                        {
                                            //esisteDoc = cp.DOCUMENTI.Where(a => (a.MODIFICATO == false || !a.FK_IDDOCUMENTO.HasValue) && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita && a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)?.Any() ?? false,
                                            esisteDoc = (idDocIdentita > 0) ? true : false,
                                            tipoDoc = EnumTipoDoc.Documento_Identita
                                        },
                                        HasDocPassaporto = new HasDocPassaporto()
                                        {
                                            tipoDocPassaporto = EnumTipoDoc.Passaporto,
                                            idDocPassaporto = idDocPassaporto,
                                            esisteDocPassaporto = (idDocPassaporto > 0) ? true : false
                                        },

                                        ordinamento = ordine,
                                        //idAttivazioneInvioPassaporti = ap_invio.IDATTIVAZIONIPASSAPORTI,
                                        //idFamiliareInvioPassaporto = cp_invio.IDCONIUGEPASSAPORTO


                                    };

                                    lConiuge.Add(coniuge);

                                }

                            }

                            if (lConiuge?.Any() ?? false)
                            {
                                lefm.AddRange(lConiuge);
                            }

                            #endregion

                            #region Figli


                            //verifico se esistono richieste figli
                            var lfp_richiesta =
                                ap_richiesta.FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false && a.INCLUDIPASSAPORTO == true)
                                .OrderBy(a => a.IDFIGLIPASSAPORTO);
                            if (lfp_richiesta?.Any() ?? false)
                            {
                                //se esistono verifico se esistono i relativi record della fase 2
                                var lfp =
                                    ap_invio.FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false && a.INCLUDIPASSAPORTO == true)
                                    .OrderBy(a => a.IDFIGLIPASSAPORTO);
                                //se non esistono li creo
                                if (!lfp?.Any() ?? false)
                                {
                                    foreach (var fp_richiesta in lfp_richiesta)
                                    {
                                        var fp_new = new FIGLIPASSAPORTO()
                                        {
                                            IDPASSAPORTI = fp_richiesta.IDPASSAPORTI,
                                            IDATTIVAZIONIPASSAPORTI = ap_invio.IDATTIVAZIONIPASSAPORTI,
                                            INCLUDIPASSAPORTO = fp_richiesta.INCLUDIPASSAPORTO,
                                            DATAAGGIORNAMENTO = DateTime.Now,
                                            ANNULLATO = fp_richiesta.ANNULLATO
                                        };
                                        ap_invio.FIGLIPASSAPORTO.Add(fp_new);
                                        if (db.SaveChanges() <= 0)
                                        {
                                            throw new Exception("Errore durante la creazione del passaporto figli per la fase di invio passaporto.");
                                        }
                                        var f = fp_richiesta.FIGLI.First();
                                        idDocIdentita = GetIdDocFamiliare((decimal)EnumTipoDoc.Documento_Identita, ap_richiesta.IDATTIVAZIONIPASSAPORTI, f.IDFIGLI, (decimal)EnumParentela.Figlio, db);
                                        AssociaDocumentoPassaportoFiglio(fp_new.IDFIGLIPASSAPORTO, idDocIdentita, db);

                                        AssociaFigliPassaportoFigli(fp_new.IDFIGLIPASSAPORTO, f.IDFIGLI, db);
                                    }
                                    //rileggo i record appena creati
                                    lfp =
                                        ap_invio.FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false && a.INCLUDIPASSAPORTO == true)
                                        .OrderBy(a => a.IDFIGLIPASSAPORTO);
                                }


                                foreach (var fp in lfp)
                                {
                                    ordine++;
                                    var f = fp.FIGLI.First();
                                    //FigliModel fm = new FigliModel();
                                    //using (dtFigli dtf = new dtFigli())
                                    //{
                                    //    fm = dtf.GetFigliobyID(fp.IDFIGLI);
                                    //}

                                    //idDocPassaporto = GetIdDocPassaportoFamiliare(ap_invio.IDATTIVAZIONIPASSAPORTI, fp.IDFIGLI, (decimal)EnumParentela.Figlio, db);
                                    idDocPassaporto = GetIdDocFamiliare((decimal)EnumTipoDoc.Passaporto, ap_invio.IDATTIVAZIONIPASSAPORTI, f.IDFIGLI, (decimal)EnumParentela.Figlio, db);
                                    idDocIdentita = GetIdDocFamiliare((decimal)EnumTipoDoc.Documento_Identita, ap_invio.IDATTIVAZIONIPASSAPORTI, f.IDFIGLI, (decimal)EnumParentela.Figlio, db);

                                    ElencoFamiliariPassaportoModel figlio = new ElencoFamiliariPassaportoModel()
                                    {
                                        idAttivazionePassaporti = ap_invio.IDATTIVAZIONIPASSAPORTI,
                                        idFamiliarePassaporto = fp.IDFIGLIPASSAPORTO,
                                        nominativo = f.COGNOME + " " + f.NOME,
                                        codiceFiscale = f.CODICEFISCALE,
                                        dataInizio = f.DATAINIZIOVALIDITA,
                                        dataFine = f.DATAFINEVALIDITA,
                                        parentela = EnumParentela.Figlio,
                                        idAltriDati = f.ALTRIDATIFAM.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDALTRIDATIFAM).First().IDALTRIDATIFAM,
                                        notificato = ap_invio.NOTIFICARICHIESTA,
                                        attivato = ap_invio.PRATICACONCLUSA,
                                        HasDoc = new HasDoc()
                                        {
                                            //esisteDoc = fp.DOCUMENTI.Where(a => (a.MODIFICATO == false || !a.FK_IDDOCUMENTO.HasValue) && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita && a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)?.Any() ?? false,
                                            esisteDoc = (idDocIdentita > 0) ? true : false,
                                            tipoDoc = EnumTipoDoc.Documento_Identita
                                        },
                                        HasDocPassaporto = new HasDocPassaporto()
                                        {
                                            tipoDocPassaporto = EnumTipoDoc.Passaporto,
                                            idDocPassaporto = idDocPassaporto,
                                            esisteDocPassaporto = (idDocPassaporto > 0) ? true : false
                                        },

                                        ordinamento = ordine,
                                        //idAttivazioneInvioPassaporti = ap_invio.IDATTIVAZIONIPASSAPORTI,
                                        //idFamiliareInvioPassaporto = cp_invio.IDCONIUGEPASSAPORTO


                                    };

                                    lFiglio.Add(figlio);

                                }

                            }

                            if (lFiglio?.Any() ?? false)
                            {
                                lefm.AddRange(lFiglio);
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

        public void PreSetPassaporto(decimal idTrasferimento, decimal idFasePassaporti, ModelDBISE db)
        {
            var t = db.TRASFERIMENTO.Find(idTrasferimento);
            var p = t.PASSAPORTI;
            if (p == null)
            {

                PASSAPORTI p_new = new PASSAPORTI()
                {
                    IDPASSAPORTI = idTrasferimento,
                };

                db.PASSAPORTI.Add(p_new);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nella fase d'inserimento dei dati per la gestione del passaporto.");
                }
                else
                {
                    p = p_new;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                        "Inserimento dei dati di gestione del passaporto.", "PASSAPORTI", db, idTrasferimento,
                        p.IDPASSAPORTI);
                }
            }

            using (dtAttivazionePassaporto dtap = new dtAttivazionePassaporto())
            {
                AttivazionePassaportiModel apm = new AttivazionePassaportiModel()
                {
                    idPassaporti = p.IDPASSAPORTI,
                    notificaRichiesta = false,
                    praticaConclusa = false,
                    idFasePassaporti = idFasePassaporti
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


        public void PreSetConiugePassaporto_Invio(decimal idAttivazioneRichiesta, ModelDBISE db)
        {
            try
            {
                var ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazioneRichiesta);

                using (dtAttivazionePassaporto dtap = new dtAttivazionePassaporto())
                {
                    var lcp = ap.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false).ToList();
                    if (lcp?.Any() ?? false)
                    {
                        foreach (var cp in lcp)
                        {
                            CONIUGEPASSAPORTO cp_invio = new CONIUGEPASSAPORTO()
                            {
                                IDCONIUGEPASSAPORTO = cp.IDCONIUGEPASSAPORTO,
                                IDPASSAPORTI = cp.IDPASSAPORTI,
                                IDATTIVAZIONIPASSAPORTI = cp.IDATTIVAZIONIPASSAPORTI,
                                INCLUDIPASSAPORTO = cp.INCLUDIPASSAPORTO,
                                DATAAGGIORNAMENTO = cp.DATAAGGIORNAMENTO,
                                ANNULLATO = cp.ANNULLATO
                            };
                            ap.CONIUGEPASSAPORTO.Add(cp_invio);
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore nella fase di creazione della fase di invio passaporti per il coniuge.");
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                        "Inserimento Coniuge Passaporto.", "CONIUGEPASSAPORTO", db, ap.IDPASSAPORTI,
                                        cp_invio.IDCONIUGEPASSAPORTO);
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


        public void PreSetFigliPassaporto_Invio(decimal idAttivazioneRichiesta, ModelDBISE db)
        {
            try
            {

                var ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazioneRichiesta);

                using (dtAttivazionePassaporto dtap = new dtAttivazionePassaporto())
                {
                    var lfp = ap.FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false);
                    if (lfp?.Any() ?? false)
                    {
                        foreach (var fp in lfp)
                        {

                            FIGLIPASSAPORTO fp_invio = new FIGLIPASSAPORTO()
                            {
                                IDFIGLIPASSAPORTO = fp.IDFIGLIPASSAPORTO,
                                IDPASSAPORTI = fp.IDPASSAPORTI,
                                IDATTIVAZIONIPASSAPORTI = fp.IDATTIVAZIONIPASSAPORTI,
                                INCLUDIPASSAPORTO = fp.INCLUDIPASSAPORTO,
                                DATAAGGIORNAMENTO = fp.DATAAGGIORNAMENTO,
                                ANNULLATO = fp.ANNULLATO
                            };
                            ap.FIGLIPASSAPORTO.Add(fp_invio);

                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore nella fase creazione della fese di invio passaporti per i figli.");
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                        "Inserimento Figli Passaporto.", "FIGLIPASSAPORTO", db, ap.IDPASSAPORTI,
                                        fp_invio.IDFIGLIPASSAPORTO);
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

        public void GetPassaportoRichiedente_Invio(ref PASSAPORTORICHIEDENTE prm, decimal idAttivazioneInvio, ModelDBISE db)
        {
            try
            {
                List<PASSAPORTORICHIEDENTE> prl = new List<PASSAPORTORICHIEDENTE>();
                PASSAPORTORICHIEDENTE pr = new PASSAPORTORICHIEDENTE();
                PASSAPORTORICHIEDENTE pr_invio = new PASSAPORTORICHIEDENTE();

                var ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazioneInvio);

                using (dtAttivazionePassaporto dtap = new dtAttivazionePassaporto())
                {
                    prl = ap.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDPASSAPORTORICHIEDENTE).ToList();
                    if (prl?.Any() ?? false)
                    {
                        pr = prl.First();
                        pr_invio = new PASSAPORTORICHIEDENTE()
                        {
                            IDPASSAPORTORICHIEDENTE = pr.IDPASSAPORTORICHIEDENTE,
                            IDPASSAPORTI = pr.IDPASSAPORTI,
                            IDATTIVAZIONIPASSAPORTI = pr.IDATTIVAZIONIPASSAPORTI,
                            INCLUDIPASSAPORTO = pr.INCLUDIPASSAPORTO,
                            DATAAGGIORNAMENTO = pr.DATAAGGIORNAMENTO,
                            ANNULLATO = pr.ANNULLATO
                        };

                    }
                    else
                    {
                        throw new Exception("Errore: record relativo a Passaporto Richiedente (fase Invio) non trovato.");
                    }

                    prm = pr_invio;
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
                        var cp =
                            db.CONIUGEPASSAPORTO.Find(idFamiliarePassaporto);
                        //.f.Where(
                        //    a => a.ANNULLATO == false && a.IDCONIUGEPASSAPORTO == idFamiliarePassaporto);

                        if (cp.IDCONIUGEPASSAPORTO > 0)
                        {
                            var c = cp.CONIUGE.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDCONIUGE).First();

                            var ad =
                                c.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                    .OrderByDescending(a => a.IDALTRIDATIFAM)
                                    .First();


                            bool EsisteDoc = false;
                            bool EsisteDocPassaporto = false;
                            decimal idDocPassaporto = 0;

                            var lDoc = c.DOCUMENTI.Where(
                                a =>
                                    (a.MODIFICATO == false || a.FK_IDDOCUMENTO.HasValue == false) &&
                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita && a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato);

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

                            var lDocPassaporto = cp.DOCUMENTI.Where(
                                            a =>
                                            (a.MODIFICATO == false || a.FK_IDDOCUMENTO.HasValue == false) &&
                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto && a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato);


                            if (lDocPassaporto?.Any() ?? false)
                            {
                                EsisteDocPassaporto = true;
                                idDocPassaporto = lDocPassaporto.First().IDDOCUMENTO;
                            }
                            else
                            {
                                EsisteDocPassaporto = false;
                            }

                            HasDocPassaporto hasDocPassaporto = new HasDocPassaporto()
                            {
                                esisteDocPassaporto = EsisteDocPassaporto,
                                tipoDocPassaporto = EnumTipoDoc.Passaporto,
                                idDocPassaporto = idDocPassaporto
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
                                HasDocPassaporto = hasDocPassaporto,
                                richiedi = cp.INCLUDIPASSAPORTO
                            };
                        }
                        break;
                    case EnumParentela.Figlio:
                        var fp =
                           db.FIGLIPASSAPORTO.Find(idFamiliarePassaporto);
                        //.f.Where(
                        //    a => a.ANNULLATO == false && a.IDCONIUGEPASSAPORTO == idFamiliarePassaporto);

                        if (fp.IDFIGLIPASSAPORTO > 0)
                        {
                            var f = fp.FIGLI.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDFIGLI).First();

                            var ad =
                                f.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                    .OrderByDescending(a => a.IDALTRIDATIFAM)
                                    .First();

                            bool EsisteDoc = false;
                            bool EsisteDocPassaporto = false;
                            decimal idDocPassaporto = 0;

                            var lDoc = f.DOCUMENTI.Where(
                                a =>
                                    (a.MODIFICATO == false || a.FK_IDDOCUMENTO.HasValue == false) &&
                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita && a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato);

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


                            var lDocPassaporto = fp.DOCUMENTI.Where(
                                a =>
                                    (a.MODIFICATO == false || a.FK_IDDOCUMENTO.HasValue == false) &&
                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto && a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato);

                            if (lDocPassaporto?.Any() ?? false)
                            {
                                EsisteDocPassaporto = true;
                                idDocPassaporto = lDocPassaporto.First().IDDOCUMENTO;
                            }
                            else
                            {
                                EsisteDocPassaporto = false;
                            }

                            HasDocPassaporto hasDocPassaporto = new HasDocPassaporto()
                            {
                                esisteDocPassaporto = EsisteDocPassaporto,
                                tipoDocPassaporto = EnumTipoDoc.Passaporto,
                                idDocPassaporto = idDocPassaporto
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
                                HasDocPassaporto = hasDocPassaporto,
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
                            bool EsisteDocPassaporto = false;
                            decimal idDocPassaporto = 0;

                            var lDoc = pr.DOCUMENTI.Where(
                                a =>
                                    (a.MODIFICATO == false || a.FK_IDDOCUMENTO.HasValue == false) &&
                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita && a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato);

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

                            var lDocPassaporto = pr.DOCUMENTI.Where(
                                            a =>
                                                (a.MODIFICATO == false || a.FK_IDDOCUMENTO.HasValue == false) &&
                                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto && a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato);

                            if (lDocPassaporto?.Any() ?? false)
                            {
                                EsisteDocPassaporto = true;
                                idDocPassaporto = lDocPassaporto.First().IDDOCUMENTO;
                            }
                            else
                            {
                                EsisteDocPassaporto = false;
                            }

                            HasDocPassaporto hasDocPassaporto = new HasDocPassaporto()
                            {
                                esisteDocPassaporto = EsisteDocPassaporto,
                                tipoDocPassaporto = EnumTipoDoc.Passaporto,
                                idDocPassaporto = idDocPassaporto
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
                                HasDocPassaporto = hasDocPassaporto,
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

        public EnumFasePassaporti GetFaseValida(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var p = t.PASSAPORTI;
                //cerco seconda fase se esiste
                var lap_invio = p.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false &&
                                                        a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Invio_Passaporti).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI);

                //se esiste verifico se è stata notificata
                if (lap_invio?.Any() ?? false)
                {
                    var ap_invio = lap_invio.First();
                    if (ap_invio.NOTIFICARICHIESTA)
                    {
                        return EnumFasePassaporti.Invio_Passaporti;
                    }
                    else
                    {
                        return EnumFasePassaporti.Richiesta_Passaporti;

                    }
                }
                else
                {
                    return EnumFasePassaporti.Richiesta_Passaporti;

                }

            }
        }

        public GestPulsantiAttConclModel GestionePulsantiAttivazionePassaporto_Richiesta(decimal idTrasferimento)
        {
            GestPulsantiAttConclModel gp = new GestPulsantiAttConclModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var p = t.PASSAPORTI;
                var lap = p.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false &&
                            a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Richiesta_Passaporti).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI);
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

                    gp.fasePassaporto = (EnumFasePassaporti)ap.IDFASEPASSAPORTI;

                }
            }

            return gp;

        }

        public GestPulsantiAttConclModel GestionePulsantiAttivazionePassaporto_Invio(decimal idTrasferimento)
        {
            GestPulsantiAttConclModel gp = new GestPulsantiAttConclModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var p = t.PASSAPORTI;
                var lap = p.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false &&
                            a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Invio_Passaporti).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI);
                if (lap?.Any() ?? false)
                {
                    var ap = lap.First();

                    gp.idAttivazionePassaporto = ap.IDATTIVAZIONIPASSAPORTI;
                    gp.notificaRichiesta = ap.NOTIFICARICHIESTA;
                    gp.praticaConclusa = ap.PRATICACONCLUSA;
                    gp.annullata = ap.ANNULLATO;

                    gp.notificabile = false;

                    var lpr = ap.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false && a.INCLUDIPASSAPORTO == true);

                    if (lpr?.Any() ?? false)
                    {
                        gp.richiedenteIncluso = true;

                        var pr = lpr.First();
                        var ldr = pr.DOCUMENTI.Where(a =>
                                                        a.MODIFICATO == false &&
                                                        a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                                        a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto);
                        if (ldr?.Any() ?? false)
                        {
                            gp.passaportoRichiedente = true;
                        }

                        if (gp.passaportoRichiedente && gp.richiedenteIncluso)
                        {
                            gp.notificabile = true;
                        }
                        else
                        {
                            gp.notificabile = false;
                        }
                    }

                    var lcp = ap.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false && a.INCLUDIPASSAPORTO == true);

                    if (lcp?.Any() ?? false)
                    {
                        gp.coniugeIncluso = true;
                        var cp = lcp.First();
                        var ldc = cp.DOCUMENTI.Where(a =>
                                                        a.MODIFICATO == false &&
                                                        a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                                        a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto);
                        if (ldc?.Any() ?? false)
                        {
                            gp.passaportoConiuge = true;
                        }

                        if (gp.passaportoConiuge && gp.coniugeIncluso)
                        {
                            gp.notificabile = true;
                        }
                        else
                        {
                            gp.notificabile = false;
                        }

                    }

                    var lfp = ap.FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false && a.INCLUDIPASSAPORTO == true);

                    if (lfp?.Any() ?? false)
                    {
                        gp.figliIncluso = true;
                        //var fp = lfp.First();
                        //controllo che esistano tutti piassaporti figli
                        var figliPassaportoAll = true;
                        foreach (var fp in lfp)
                        {
                            var ldf = fp.DOCUMENTI.Where(a =>
                                                        a.MODIFICATO == false &&
                                                        a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                                        a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto);
                            if (!(ldf?.Any() ?? false))
                            {
                                figliPassaportoAll = false;
                            }
                        }
                        if (figliPassaportoAll)
                        {
                            gp.passaportoFigli = true;
                            gp.notificabile = true;
                        }
                        else
                        {
                            gp.notificabile = false;
                        }
                    }

                    gp.statoTrasferimento = (EnumStatoTraferimento)t.IDSTATOTRASFERIMENTO;

                    gp.fasePassaporto = (EnumFasePassaporti)ap.IDFASEPASSAPORTI;

                }
            }

            return gp;

        }


        public EnumFasePassaporti GetFasePassaporti_Corrente(decimal idTrasferimento)
        {
            //default
            EnumFasePassaporti fasePassaporti = EnumFasePassaporti.Richiesta_Passaporti;

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var p = t.PASSAPORTI;
                var lap = p.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false && a.IDFASEPASSAPORTI == (decimal)fasePassaporti).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI).ToList();
                if (lap?.Any() ?? false)
                {
                    var ap = lap.First();

                    //se la prima fase è conclusa vuol dire che sto alla seconda fase
                    if (ap.PRATICACONCLUSA)
                    {
                        fasePassaporti = EnumFasePassaporti.Invio_Passaporti;
                    }
                }
            }

            return fasePassaporti;
        }

        public ATTIVAZIONIPASSAPORTI FaseRichiestaPassaporti(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                ATTIVAZIONIPASSAPORTI ap = new ATTIVAZIONIPASSAPORTI();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var p = t.PASSAPORTI;
                var lap = p.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false && a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Richiesta_Passaporti).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI).ToList();
                if (lap?.Any() ?? false)
                {
                    ap = lap.First();
                }
                return ap;
            }
        }

        public ATTIVAZIONIPASSAPORTI FaseInvioPassaporti(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                ATTIVAZIONIPASSAPORTI ap = new ATTIVAZIONIPASSAPORTI();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var p = t.PASSAPORTI;
                var lap = p.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false && a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Invio_Passaporti).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI).ToList();
                if (lap?.Any() ?? false)
                {
                    ap = lap.First();
                }
                return ap;
            }
        }

        public AttivazionePassaportiModel GetAttivazioneInvioPassaportiInLavorazione(decimal idTrasferimento, ModelDBISE db)
        {
            try
            {
                AttivazionePassaportiModel apm = new AttivazionePassaportiModel();
                ATTIVAZIONIPASSAPORTI ap = new ATTIVAZIONIPASSAPORTI();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                var apl = t.PASSAPORTI.ATTIVAZIONIPASSAPORTI
                    .Where(a => a.ANNULLATO == false &&
                            a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Invio_Passaporti &&
                            a.NOTIFICARICHIESTA == false)
                            .OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI).ToList();

                if (apl?.Any() ?? false)
                {
                    ap = apl.First();

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
                        annullato = ap.ANNULLATO,
                        idFasePassaporti = ap.IDFASEPASSAPORTI
                    };
                }

                return apm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AttivazionePassaportiModel GetUltimaAttivazioneRichiestaPassaporti(decimal idTrasferimento, ModelDBISE db)
        {
            try
            {
                AttivazionePassaportiModel apm = new AttivazionePassaportiModel();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                var apl = t.PASSAPORTI.ATTIVAZIONIPASSAPORTI
                    .Where(a => a.ANNULLATO == false &&
                            a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Richiesta_Passaporti)
                            .OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI).ToList();

                if (apl?.Any() ?? false)
                {
                    var ap = apl.First();

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
                        annullato = ap.ANNULLATO,
                        idFasePassaporti = ap.IDFASEPASSAPORTI
                    };
                }

                return apm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ATTIVAZIONIPASSAPORTI GetAttivazioneById(decimal idAttivazione)
        {
            try
            {
                ATTIVAZIONIPASSAPORTI ap = new ATTIVAZIONIPASSAPORTI();

                using (ModelDBISE db = new ModelDBISE())
                {
                    ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazione);

                }

                return ap;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AttivazionePassaportiModel CreaAttivazioneInvioPassaporti(decimal idTrasferimento, ModelDBISE db)
        {
            AttivazionePassaportiModel new_apm = new AttivazionePassaportiModel();

            ATTIVAZIONIPASSAPORTI new_ap = new ATTIVAZIONIPASSAPORTI()
            {
                IDPASSAPORTI = idTrasferimento,
                NOTIFICARICHIESTA = false,
                IDFASEPASSAPORTI = (decimal)EnumFasePassaporti.Invio_Passaporti,
                DATAPRATICACONCLUSA = null,
                PRATICACONCLUSA = false,
                DATANOTIFICARICHIESTA = null,
                ANNULLATO = false,
                DATAVARIAZIONE = DateTime.Now,
                DATAAGGIORNAMENTO = DateTime.Now,
            };
            db.ATTIVAZIONIPASSAPORTI.Add(new_ap);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception(string.Format("Non è stato possibile creare una nuova attivazione per la fese di invio dei passaporti."));
            }
            else
            {
                new_apm = new AttivazionePassaportiModel()
                {
                    idAttivazioniPassaporti = new_ap.IDATTIVAZIONIPASSAPORTI,
                    idPassaporti = new_ap.IDPASSAPORTI,
                    idFasePassaporti = new_ap.IDFASEPASSAPORTI,
                    notificaRichiesta = new_ap.NOTIFICARICHIESTA,
                    dataNotificaRichiesta = new_ap.DATANOTIFICARICHIESTA,
                    praticaConclusa = new_ap.PRATICACONCLUSA,
                    dataPraticaConclusa = new_ap.DATAPRATICACONCLUSA,
                    dataVariazione = new_ap.DATAVARIAZIONE,
                    dataAggiornamento = new_ap.DATAAGGIORNAMENTO,
                    annullato = new_ap.ANNULLATO
                };

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova attivazione maggiorazione abitazione.", "ATTIVITAZIONEMAB", db, idTrasferimento, new_ap.IDATTIVAZIONIPASSAPORTI);
            }

            return new_apm;
        }

        public decimal VerificaEsistenzaDocumentoPassaporto(decimal idTrasferimento, decimal idTipoDocumento, decimal idParentela, decimal idFamiliare)
        {
            try
            {
                ATTIVAZIONIPASSAPORTI ap = new ATTIVAZIONIPASSAPORTI();

                decimal idDocPassaporto = 0;


                using (ModelDBISE db = new ModelDBISE())
                {

                    var t = db.TRASFERIMENTO.Find(idTrasferimento);

                    var apl = t.PASSAPORTI.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false && a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Invio_Passaporti && a.NOTIFICARICHIESTA == false).OrderBy(a => a.IDATTIVAZIONIPASSAPORTI).ToList();

                    if (apl?.Any() ?? false)
                    {
                        ap = apl.First();

                        switch ((EnumParentela)idParentela)
                        {
                            case EnumParentela.Coniuge:
                                //var cpl = ap.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false && a.IDCONIUGE == idFamiliare).ToList();//. && ..DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == (decimal)TipoDocumento).ToList();
                                var cp = db.CONIUGEPASSAPORTO.Find(idFamiliare);

                                //if (cpl?.Any() ?? false)
                                if (cp.IDCONIUGEPASSAPORTO > 0)
                                {
                                    //var cp = cpl.First();
                                    var dcpl = cp.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == idTipoDocumento).ToList();
                                    if (dcpl?.Any() ?? false)
                                    {
                                        if (dcpl.Count() == 1)
                                        {
                                            var dcp = dcpl.First();
                                            idDocPassaporto = dcp.IDDOCUMENTO;
                                        }
                                        else
                                        {
                                            throw new Exception(string.Format("Errore in fase di aggiornamento documentazione Passaporti. Esiste più di un documento del tipo selezionato. Contattare l'amministratore di sistema."));
                                        }

                                    }
                                }
                                break;

                            case EnumParentela.Figlio:
                                //var fpl = ap.FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false && a.IDFIGLI == idFamiliare).ToList();
                                var fp = db.FIGLIPASSAPORTO.Find(idFamiliare);

                                //if (fpl?.Any() ?? false)
                                if (fp.IDFIGLIPASSAPORTO > 0)
                                {
                                    //var fp = fpl.First();
                                    var dfpl = fp.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == idTipoDocumento).ToList();
                                    if (dfpl?.Any() ?? false)
                                    {
                                        if (dfpl.Count() == 1)
                                        {
                                            var dfp = dfpl.First();
                                            idDocPassaporto = dfp.IDDOCUMENTO;
                                        }
                                        else
                                        {
                                            throw new Exception(string.Format("Errore in fase di aggiornamento documentazione Passaporti. Esiste più di un documento del tipo selezionato. Contattare l'amministratore di sistema."));
                                        }

                                    }
                                }

                                break;

                            case EnumParentela.Richiedente:
                                var rpl = ap.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false).ToList();

                                if (rpl?.Any() ?? false)
                                {
                                    var rp = rpl.First();
                                    var drpl = rp.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == idTipoDocumento).ToList();
                                    if (drpl?.Any() ?? false)
                                    {
                                        if (drpl.Count() == 1)
                                        {
                                            var drp = drpl.First();
                                            idDocPassaporto = drp.IDDOCUMENTO;
                                        }
                                        else
                                        {
                                            throw new Exception(string.Format("Errore in fase di aggiornamento documentazione Passaporti. Esiste più di un documento del tipo selezionato. Contattare l'amministratore di sistema."));
                                        }

                                    }
                                }
                                break;
                        }
                    }
                    return idDocPassaporto;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void SostituisciDocumentoPassaporto(ref DocumentiModel dm, decimal idDocumentoOld, decimal idAttivazionePassaporti, ModelDBISE db)
        {
            //inserisce un nuovo documento e imposta il documento sostituito 
            //con MODIFICATO=true e valorizza FK_IDDOCUMENTO

            DOCUMENTI d_new = new DOCUMENTI();
            DOCUMENTI d_old = new DOCUMENTI();
            MemoryStream ms = new MemoryStream();
            dm.file.InputStream.CopyTo(ms);
            var ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazionePassaporti);

            d_new.NOMEDOCUMENTO = dm.nomeDocumento;
            d_new.ESTENSIONE = dm.estensione;
            d_new.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
            d_new.DATAINSERIMENTO = dm.dataInserimento;
            d_new.FILEDOCUMENTO = ms.ToArray();
            d_new.MODIFICATO = false;
            d_new.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;
            d_new.FK_IDDOCUMENTO = null;


            db.DOCUMENTI.Add(d_new);

            if (db.SaveChanges() > 0)
            {
                dm.idDocumenti = d_new.IDDOCUMENTO;
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo documento (passaporto).", "Documenti", db, ap.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, dm.idDocumenti);

                //aggiorno il documento esistente
                d_old = db.DOCUMENTI.Find(idDocumentoOld);
                if (d_old.IDDOCUMENTO > 0)
                {
                    d_old.MODIFICATO = true;
                    d_old.FK_IDDOCUMENTO = d_new.IDDOCUMENTO;

                    if (db.SaveChanges() > 0)
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modificato documento con FK_idDocumento (passaporto).", "Documenti", db, ap.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO, d_old.IDDOCUMENTO);
                    }
                }
            }
        }

        public void SetDocumentoPassaporto(ref DocumentiModel dm, decimal idAttivazionepassaporto, ModelDBISE db)
        {
            MemoryStream ms = new MemoryStream();
            DOCUMENTI d = new DOCUMENTI();

            dm.file.InputStream.CopyTo(ms);
            var ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazionepassaporto);

            d.NOMEDOCUMENTO = dm.nomeDocumento;
            d.ESTENSIONE = dm.estensione;
            d.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
            d.DATAINSERIMENTO = dm.dataInserimento;
            d.FILEDOCUMENTO = ms.ToArray();
            d.MODIFICATO = false;
            d.FK_IDDOCUMENTO = null;
            d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;


            db.DOCUMENTI.Add(d);

            if (db.SaveChanges() > 0)
            {
                dm.idDocumenti = d.IDDOCUMENTO;
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo documento (passaporto).", "Documenti", db, ap.IDPASSAPORTI, dm.idDocumenti);
            }
        }

        public void AssociaDocumentoPassaportoRichiedente(decimal idPassaportoRichiedente, decimal idDocumento, ModelDBISE db)
        {
            try
            {
                var pr = db.PASSAPORTORICHIEDENTE.Find(idPassaportoRichiedente);
                var item = db.Entry<PASSAPORTORICHIEDENTE>(pr);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.DOCUMENTI).Load();
                var d = db.DOCUMENTI.Find(idDocumento);
                pr.DOCUMENTI.Add(d);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il passaporto al richiedente"));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AssociaDocumentoPassaportoConiuge(decimal idConiugePassaporto, decimal idDocumento, ModelDBISE db)
        {
            try
            {
                var cp = db.CONIUGEPASSAPORTO.Find(idConiugePassaporto);
                var item = db.Entry<CONIUGEPASSAPORTO>(cp);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.DOCUMENTI).Load();
                var d = db.DOCUMENTI.Find(idDocumento);
                cp.DOCUMENTI.Add(d);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il passaporto al coniuge"));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AssociaConiugePassaportoConiuge(decimal idConiugePassaporto, decimal idConiuge, ModelDBISE db)
        {
            try
            {
                var cp = db.CONIUGEPASSAPORTO.Find(idConiugePassaporto);
                var item = db.Entry<CONIUGEPASSAPORTO>(cp);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.CONIUGE).Load();
                var c = db.CONIUGE.Find(idConiuge);
                cp.CONIUGE.Add(c);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il ConiugePassaporto al coniuge"));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AssociaFigliPassaportoFigli(decimal idFigliPassaporto, decimal idFigli, ModelDBISE db)
        {
            try
            {
                var fp = db.FIGLIPASSAPORTO.Find(idFigliPassaporto);
                var item = db.Entry<FIGLIPASSAPORTO>(fp);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.FIGLI).Load();
                var f = db.FIGLI.Find(idFigli);
                fp.FIGLI.Add(f);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il FigliPassaporto al figlio"));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AssociaDocumentoPassaportoFiglio(decimal idFigliPassaporto, decimal idDocumento, ModelDBISE db)
        {
            try
            {
                var fp = db.FIGLIPASSAPORTO.Find(idFigliPassaporto);
                var item = db.Entry<FIGLIPASSAPORTO>(fp);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.DOCUMENTI).Load();
                var d = db.DOCUMENTI.Find(idDocumento);
                fp.DOCUMENTI.Add(d);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il passaporto al figlio"));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal GetIdDocPassaportoFamiliare(decimal idAttivazione, decimal idFamiliare, decimal idParentela, ModelDBISE db)
        {
            decimal valore = 0;

            var ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazione);
            switch ((EnumParentela)idParentela)
            {
                case EnumParentela.Richiedente:
                    var lpr = ap.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false);
                    if (lpr?.Any() ?? false)
                    {
                        var pr = lpr.First();
                        var ldr = pr.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto);
                        if (ldr?.Any() ?? false)
                        {
                            var dr = ldr.First();
                            valore = dr.IDDOCUMENTO;
                        }
                    }
                    break;

                case EnumParentela.Coniuge:
                    var lpc = db.CONIUGE.Find(idFamiliare).CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false);
                    if (lpc?.Any() ?? false)
                    {
                        var pc = lpc.First();
                        var ldc = pc.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto);
                        if (ldc?.Any() ?? false)
                        {
                            var dc = ldc.First();
                            valore = dc.IDDOCUMENTO;
                        }
                    }
                    break;

                case EnumParentela.Figlio:
                    var lpf = db.FIGLI.Find(idFamiliare).FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false);
                    if (lpf?.Any() ?? false)
                    {
                        var pf = lpf.First();
                        var ldf = pf.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto);
                        if (ldf?.Any() ?? false)
                        {
                            var df = ldf.First();
                            valore = df.IDDOCUMENTO;
                        }
                    }
                    break;

            }
            return valore;

        }


        public decimal GetIdDocFamiliare(decimal idTipoDoc, decimal idAttivazione, decimal idFamiliare, decimal idParentela, ModelDBISE db)
        {
            decimal valore = 0;

            var ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazione);
            switch ((EnumParentela)idParentela)
            {
                case EnumParentela.Richiedente:
                    var lpr = ap.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false);
                    if (lpr?.Any() ?? false)
                    {
                        var pr = lpr.First();
                        var ldr = pr.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == idTipoDoc);
                        if (ldr?.Any() ?? false)
                        {
                            var dr = ldr.First();
                            valore = dr.IDDOCUMENTO;
                        }
                    }
                    break;

                case EnumParentela.Coniuge:
                    var lpc = db.CONIUGE.Find(idFamiliare).CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDCONIUGEPASSAPORTO); ;
                    if (lpc?.Any() ?? false)
                    {
                        var pc = lpc.First();
                        var ldc = pc.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == idTipoDoc);
                        if (ldc?.Any() ?? false)
                        {
                            var dc = ldc.First();
                            valore = dc.IDDOCUMENTO;
                        }
                    }
                    break;

                case EnumParentela.Figlio:
                    var lpf = db.FIGLI.Find(idFamiliare).FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDFIGLIPASSAPORTO);
                    if (lpf?.Any() ?? false)
                    {
                        var pf = lpf.First();
                        var ldf = pf.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == idTipoDoc);
                        if (ldf?.Any() ?? false)
                        {
                            var df = ldf.First();
                            valore = df.IDDOCUMENTO;
                        }
                    }
                    break;

            }
            return valore;

        }


    }
}
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
    public class dtVariazionePassaporto : IDisposable
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
           
                            //PreSetPassaporto(ap.IDPASSAPORTI, (decimal)EnumFasePassaporti.Invio_Passaporti, db);

 
                            //cerco eventuale attivazione non notificata
                            ATTIVAZIONIPASSAPORTI ap_temp = new ATTIVAZIONIPASSAPORTI();
                            AttivazionePassaportiModel apm_temp = new AttivazionePassaportiModel();
                            var p = db.PASSAPORTI.Find(ap.IDPASSAPORTI);
                            var lap_temp = p.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false && a.IDFASEPASSAPORTI==(decimal)EnumFasePassaporti.Richiesta_Passaporti && a.NOTIFICARICHIESTA == false).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI).ToList();

                            #region coniugi
                            //verifico se esistono coniugi di cui non ho fatto richiesta
                            var lcp = ap.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false && a.INCLUDIPASSAPORTO == false).ToList();
                            if(lcp?.Any()??false)
                            {
                                if (lap_temp?.Any() ?? false)
                                {
                                    ap_temp = lap_temp.First();
                                }else
                                {
                                    apm_temp=CreaAttivazioneRichiestaPassaporti(p.TRASFERIMENTO.IDTRASFERIMENTO, db);
                                    ap_temp = new ATTIVAZIONIPASSAPORTI
                                    {
                                        IDATTIVAZIONIPASSAPORTI = apm_temp.idAttivazioniPassaporti,
                                        IDPASSAPORTI = apm_temp.idPassaporti,
                                        IDFASEPASSAPORTI = apm_temp.idFasePassaporti,
                                        NOTIFICARICHIESTA = false,
                                        PRATICACONCLUSA = false,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        DATAVARIAZIONE = DateTime.Now,
                                        ANNULLATO = false
                                    };
                                }
                                foreach (var cp in lcp)
                                {
                                    CONIUGEPASSAPORTO cp_new = new CONIUGEPASSAPORTO()
                                    {
                                        IDPASSAPORTI = cp.IDPASSAPORTI,
                                        IDATTIVAZIONIPASSAPORTI = ap_temp.IDATTIVAZIONIPASSAPORTI,
                                        INCLUDIPASSAPORTO = cp.INCLUDIPASSAPORTO,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        ANNULLATO = false
                                    };
                                    db.CONIUGEPASSAPORTO.Add(cp_new);

                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore: Impossibile completare l'approvazione delle pratiche del passaporto. Errore in fase di duplicazione ConiugePassaporto");
                                    }

                                    //riassocia documento identita coniuge
                                    var ldocIdenC =
                                        cp.DOCUMENTI.Where(
                                            a =>
                                                a.MODIFICATO == false &&
                                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                            .OrderBy(a => a.DATAINSERIMENTO);
                                    if (ldocIdenC?.Any() ?? false)
                                    {
                                        foreach (var docIdenC in ldocIdenC)
                                        {
                                            AssociaDocumentoPassaportoConiuge(cp_new.IDCONIUGEPASSAPORTO, docIdenC.IDDOCUMENTO, db);
                                        }
                                    }

                                    //riassocia coniugepassaporto a coniuge
                                    var lc =
                                        cp.CONIUGE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                                    if (lc?.Any() ?? false)
                                    {
                                        foreach (var c in lc)
                                        {
                                            AssociaConiugePassaportoConiuge(cp_new.IDCONIUGEPASSAPORTO, c.IDCONIUGE, db);
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region figli
                            lap_temp = p.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false && a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Richiesta_Passaporti && a.NOTIFICARICHIESTA == false).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI).ToList();

                            //verifico se esistono figli di cui non ho fatto richiesta
                            var lfp = ap.FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false && a.INCLUDIPASSAPORTO == false).ToList();
                            if (lfp?.Any() ?? false)
                            {
                                if (lap_temp?.Any() ?? false)
                                {
                                    ap_temp = lap_temp.First();
                                }
                                else
                                {
                                    apm_temp = CreaAttivazioneRichiestaPassaporti(p.TRASFERIMENTO.IDTRASFERIMENTO, db);
                                    ap_temp = new ATTIVAZIONIPASSAPORTI
                                    {
                                        IDATTIVAZIONIPASSAPORTI = apm_temp.idAttivazioniPassaporti,
                                        IDPASSAPORTI = apm_temp.idPassaporti,
                                        IDFASEPASSAPORTI = apm_temp.idFasePassaporti,
                                        NOTIFICARICHIESTA = false,
                                        PRATICACONCLUSA = false,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        DATAVARIAZIONE = DateTime.Now,
                                        ANNULLATO = false
                                    };
                                }
                                foreach (var fp in lfp)
                                {
                                    FIGLIPASSAPORTO fp_new = new FIGLIPASSAPORTO()
                                    {
                                        IDPASSAPORTI = fp.IDPASSAPORTI,
                                        IDATTIVAZIONIPASSAPORTI = ap_temp.IDATTIVAZIONIPASSAPORTI,
                                        INCLUDIPASSAPORTO = fp.INCLUDIPASSAPORTO,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        ANNULLATO = false
                                    };
                                    db.FIGLIPASSAPORTO.Add(fp_new);

                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore: Impossibile completare l'approvazione delle pratiche del passaporto. Errore in fase di duplicazione FigliPassaporto");
                                    }

                                    //riassocia documento identita figli
                                    var ldocIdenF =
                                        fp.DOCUMENTI.Where(
                                            a =>
                                                a.MODIFICATO == false &&
                                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                            .OrderBy(a => a.DATAINSERIMENTO);
                                    if (ldocIdenF?.Any() ?? false)
                                    {
                                        foreach (var docIdenF in ldocIdenF)
                                        {
                                            AssociaDocumentoPassaportoFiglio(fp_new.IDFIGLIPASSAPORTO, docIdenF.IDDOCUMENTO, db);
                                        }
                                    }

                                    //riassocia figlipassaporto a figli
                                    var lf =
                                        fp.FIGLI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                                    if (lf?.Any() ?? false)
                                    {
                                        foreach (var f in lf)
                                        {
                                            AssociaFigliPassaportoFigli(fp_new.IDFIGLIPASSAPORTO, f.IDFIGLI, db);
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region associo coniugi richiesti alla fase 2
                            //verifico se esistono coniugi di cui HO fatto richiesta
                            lcp = ap.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false && 
                                    a.INCLUDIPASSAPORTO).ToList();
                            if (lcp?.Any() ?? false)
                            {
                                apm_temp = CreaAttivazioneInvioPassaporti(p.TRASFERIMENTO.IDTRASFERIMENTO, db);
                                ap_temp = new ATTIVAZIONIPASSAPORTI
                                {
                                    IDATTIVAZIONIPASSAPORTI = apm_temp.idAttivazioniPassaporti,
                                    IDPASSAPORTI = apm_temp.idPassaporti,
                                    IDFASEPASSAPORTI = apm_temp.idFasePassaporti,
                                    NOTIFICARICHIESTA = false,
                                    PRATICACONCLUSA = false,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    DATAVARIAZIONE = DateTime.Now,
                                    ANNULLATO = false
                                };
                                foreach (var cp in lcp)
                                {
                                    CONIUGEPASSAPORTO cp_new = new CONIUGEPASSAPORTO()
                                    {
                                        IDPASSAPORTI = cp.IDPASSAPORTI,
                                        IDATTIVAZIONIPASSAPORTI = ap_temp.IDATTIVAZIONIPASSAPORTI,
                                        INCLUDIPASSAPORTO = cp.INCLUDIPASSAPORTO,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        ANNULLATO = false
                                    };
                                    db.CONIUGEPASSAPORTO.Add(cp_new);

                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore: Impossibile completare l'approvazione delle pratiche del passaporto. Errore in fase di duplicazione ConiugePassaporto");
                                    }

                                    //riassocia documento identita coniuge
                                    var ldocIdenC =
                                        cp.DOCUMENTI.Where(
                                            a =>
                                                a.MODIFICATO == false &&
                                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                            .OrderBy(a => a.DATAINSERIMENTO);
                                    if (ldocIdenC?.Any() ?? false)
                                    {
                                        foreach (var docIdenC in ldocIdenC)
                                        {
                                            AssociaDocumentoPassaportoConiuge(cp_new.IDCONIUGEPASSAPORTO, docIdenC.IDDOCUMENTO, db);
                                        }
                                    }

                                    //riassocia coniugepassaporto a coniuge
                                    var lc =
                                        cp.CONIUGE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                                    if (lc?.Any() ?? false)
                                    {
                                        foreach (var c in lc)
                                        {
                                            AssociaConiugePassaportoConiuge(cp_new.IDCONIUGEPASSAPORTO, c.IDCONIUGE, db);
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region associo figli richiesti alla fase 2
                            //verifico se esistono figli di cui non ho fatto richiesta
                            lfp = ap.FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false && 
                                        a.INCLUDIPASSAPORTO).ToList();
                            if (lfp?.Any() ?? false)
                            {
                                //verifico se esiste una fase 2 da notificare
                                lap_temp = p.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false && 
                                                a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Invio_Passaporti && 
                                                a.NOTIFICARICHIESTA == false)
                                            .OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI)
                                            .ToList();

                                if (lap_temp?.Any() ?? false)
                                {
                                    ap_temp = lap_temp.First();
                                }
                                else
                                {
                                    apm_temp = CreaAttivazioneInvioPassaporti(p.TRASFERIMENTO.IDTRASFERIMENTO, db);
                                    ap_temp = new ATTIVAZIONIPASSAPORTI
                                    {
                                        IDATTIVAZIONIPASSAPORTI = apm_temp.idAttivazioniPassaporti,
                                        IDPASSAPORTI = apm_temp.idPassaporti,
                                        IDFASEPASSAPORTI = apm_temp.idFasePassaporti,
                                        NOTIFICARICHIESTA = false,
                                        PRATICACONCLUSA = false,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        DATAVARIAZIONE = DateTime.Now,
                                        ANNULLATO = false
                                    };
                                }
                                foreach (var fp in lfp)
                                {
                                    FIGLIPASSAPORTO fp_new = new FIGLIPASSAPORTO()
                                    {
                                        IDPASSAPORTI = fp.IDPASSAPORTI,
                                        IDATTIVAZIONIPASSAPORTI = ap_temp.IDATTIVAZIONIPASSAPORTI,
                                        INCLUDIPASSAPORTO = fp.INCLUDIPASSAPORTO,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        ANNULLATO = false
                                    };
                                    db.FIGLIPASSAPORTO.Add(fp_new);

                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore: Impossibile completare l'approvazione delle pratiche del passaporto. Errore in fase di duplicazione FigliPassaporto");
                                    }

                                    //riassocia documento identita figli
                                    var ldocIdenF =
                                        fp.DOCUMENTI.Where(
                                            a =>
                                                a.MODIFICATO == false &&
                                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                            .OrderBy(a => a.DATAINSERIMENTO);
                                    if (ldocIdenF?.Any() ?? false)
                                    {
                                        foreach (var docIdenF in ldocIdenF)
                                        {
                                            AssociaDocumentoPassaportoFiglio(fp_new.IDFIGLIPASSAPORTO, docIdenF.IDDOCUMENTO, db);
                                        }
                                    }

                                    //riassocia figlipassaporto a figli
                                    var lf =
                                        fp.FIGLI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                                    if (lf?.Any() ?? false)
                                    {
                                        foreach (var f in lf)
                                        {
                                            AssociaFigliPassaportoFigli(fp_new.IDFIGLIPASSAPORTO, f.IDFIGLI, db);
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
                                                                db);
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

                                //var apm=CreaAttivazioneRichiestaPassaporti(ap.IDPASSAPORTI, db);

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
                ATTIVAZIONIPASSAPORTI apNew = new ATTIVAZIONIPASSAPORTI();

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

                                //verifico se esiste una attivazione di richista non notificata
                                var p = db.PASSAPORTI.Find(apOld.IDPASSAPORTI);
                                var lap_da_notif = p.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == false).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI).ToList();
                                if (lap_da_notif?.Any() ?? false)
                                {
                                    //se esiste la uso
                                    ATTIVAZIONIPASSAPORTI ap_new = lap_da_notif.First();
                                }
                                else
                                {
                                    //altrimenti la creo
                                    apNew = new ATTIVAZIONIPASSAPORTI()
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
                                }

                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                    "Inserimento di una nuova riga per il ciclo di attivazione relativo al passaporto.",
                                    "ATTIVAZIONIPASSAPORTI", db, apNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                    apNew.IDATTIVAZIONIPASSAPORTI);

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

                                        //riassocia coniugepassaporto a coniuge
                                        var lc_Old =
                                            cpOld.CONIUGE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                                        if (lc_Old?.Any() ?? false)
                                        {
                                            foreach (var c_Old in lc_Old)
                                            {
                                                AssociaConiugePassaportoConiuge(cpNew.IDCONIUGEPASSAPORTO, c_Old.IDCONIUGE, db);
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

                                        //riassocia figlipassaporto a figlio
                                        var lf_Old =
                                            fpOld.FIGLI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                                        if (lf_Old?.Any() ?? false)
                                        {
                                            foreach (var f_Old in lf_Old)
                                            {
                                                AssociaFigliPassaportoFigli(fpNew.IDFIGLIPASSAPORTO, f_Old.IDFIGLI, db);
                                            }
                                        }


                                    }


                                }
                                #endregion



                                EmailTrasferimento.EmailAnnulla(apNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                               Resources.msgEmail.OggettoAnnullaRichiestaPassaporto,
                                                               testoAnnulla,
                                                               db);

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

                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore - Impossibile inserire il coniuge per il passaporto da annullamento richiesta invio passaporto.");
                                    }

                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                            "Inserimento di una nuova riga per il coniuge passaporto relativo all'invio passaporto.",
                                                            "CONIUGEPASSAPORTO", db,
                                                            apNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                            cpNew.IDCONIUGEPASSAPORTO);

                                    //annullo il dato del coniuge
                                    cpOld.ANNULLATO = true;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore - Impossibile annullare il passaporto coniuge.");
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

                                    //riassocia coniugepassaporto a coniuge
                                    var lc_Old =
                                        cpOld.CONIUGE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                                    if (lc_Old?.Any() ?? false)
                                    {
                                        foreach (var c_Old in lc_Old)
                                        {
                                            AssociaConiugePassaportoConiuge(cpNew.IDCONIUGEPASSAPORTO, c_Old.IDCONIUGE, db);
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

                                    //annullo il dato del coniuge
                                    fpOld.ANNULLATO = true;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore - Impossibile annullare il passaporto coniuge.");
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

                                    //riassocia coniugepassaporto a coniuge
                                    var lf_Old =
                                        fpOld.FIGLI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                                    if (lf_Old?.Any() ?? false)
                                    {
                                        foreach (var f_Old in lf_Old)
                                        {
                                            AssociaFigliPassaportoFigli(fpNew.IDFIGLIPASSAPORTO, f_Old.IDFIGLI, db);
                                        }
                                    }
                                }
                            }
                            #endregion

                            EmailTrasferimento.EmailAnnulla(apNew.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO,
                                        Resources.msgEmail.OggettoAnnullaInvioPassaporto,
                                        testoAnnulla,
                                        db);


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



        public IList<ElencoFamiliariPassaportoModel> GetFamiliariRichiestaPassaporto(decimal idTrasferimento)
        {
            List<ElencoFamiliariPassaportoModel> lefm = new List<ElencoFamiliariPassaportoModel>();
            ElencoFamiliariPassaportoModel richiedente = new ElencoFamiliariPassaportoModel();
            List<ElencoFamiliariPassaportoModel> lConiuge = new List<ElencoFamiliariPassaportoModel>();
            List<ElencoFamiliariPassaportoModel> lFiglio = new List<ElencoFamiliariPassaportoModel>();

            ATTIVAZIONIPASSAPORTI ap = new ATTIVAZIONIPASSAPORTI();
            ATTIVAZIONIPASSAPORTI ap_new = new ATTIVAZIONIPASSAPORTI();
            AttivazionePassaportiModel apm = new AttivazionePassaportiModel();
            AttivazionePassaportiModel apm_new = new AttivazionePassaportiModel();

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
                        var p = t.PASSAPORTI;
                        var idPassaporti = p.IDPASSAPORTI;

                        if (p != null && idPassaporti > 0)
                        {
                            var ultima_ap_richiesta = GetUltimaFasePassaporti_Richiesta(idPassaporti, db);

                            if (ultima_ap_richiesta.PRATICACONCLUSA)
                            {
                                #region crea nuova attivazione
                                apm_new = CreaAttivazioneRichiestaPassaporti(idTrasferimento, db);
                                ap_new = db.ATTIVAZIONIPASSAPORTI.Find(apm_new.idAttivazioniPassaporti);
                                #endregion

                                #region eventuali CONIUGI senza passaporto su AP Richiesta attiva
                                //elenco
                                var lc_p = p.CONIUGEPASSAPORTO
                                            .Where(a => a.ANNULLATO == false &&
                                                        a.INCLUDIPASSAPORTO == false &&
                                                        a.IDATTIVAZIONIPASSAPORTI == ultima_ap_richiesta.IDATTIVAZIONIPASSAPORTI)
                                                        .ToList();

                                //li replico su CONIUGEPASSAPORTO e li associo alla nuova attivazione Fase 1
                                if (lc_p?.Any() ?? false)
                                {
                                    foreach (var c_p in lc_p)
                                    {
                                        cp = new CONIUGEPASSAPORTO()
                                        {
                                            IDPASSAPORTI = idPassaporti,
                                            IDATTIVAZIONIPASSAPORTI = ap_new.IDATTIVAZIONIPASSAPORTI,
                                            INCLUDIPASSAPORTO = c_p.INCLUDIPASSAPORTO,
                                            ANNULLATO = c_p.ANNULLATO,
                                            DATAAGGIORNAMENTO = DateTime.Now
                                        };
                                        db.CONIUGEPASSAPORTO.Add(cp);

                                        if (db.SaveChanges() <= 0)
                                        {
                                            throw new Exception("Errore nella fase di creazione di coniugepassaporto (richiesta).");
                                        }
                                        var coniuge = c_p.CONIUGE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).OrderByDescending(a => a.IDCONIUGE).First();

                                        AssociaConiugePassaportoConiuge(cp.IDCONIUGEPASSAPORTO, coniuge.IDCONIUGE, db);

                                        var lDocIdentita = c_p.DOCUMENTI.Where(a =>
                                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();

                                        if (lDocIdentita.Count > 0)
                                        {
                                            foreach (var docIdentita in lDocIdentita)
                                            {
                                                this.AssociaDocumentoPassaportoConiuge(cp.IDCONIUGEPASSAPORTO, docIdentita.IDDOCUMENTO, db);
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region eventuali FIGLI senza passaporto su AP Richiesta attiva
                                var lf_p = p.FIGLIPASSAPORTO
                                            .Where(a => a.ANNULLATO == false &&
                                                        a.INCLUDIPASSAPORTO == false &&
                                                        a.IDATTIVAZIONIPASSAPORTI == ultima_ap_richiesta.IDATTIVAZIONIPASSAPORTI)
                                                        .ToList();

                                if (lf_p?.Any() ?? false)
                                {
                                    foreach (var f_p in lf_p)
                                    {

                                        fp = new FIGLIPASSAPORTO()
                                        {
                                            IDPASSAPORTI = idPassaporti,
                                            IDATTIVAZIONIPASSAPORTI = ap_new.IDATTIVAZIONIPASSAPORTI,
                                            INCLUDIPASSAPORTO = f_p.INCLUDIPASSAPORTO,
                                            ANNULLATO = f_p.ANNULLATO,
                                            DATAAGGIORNAMENTO = DateTime.Now
                                        };
                                        db.FIGLIPASSAPORTO.Add(fp);

                                        if (db.SaveChanges() <= 0)
                                        {
                                            throw new Exception("Errore nella fase di creazione di figlipassaporto (richiesta).");
                                        }
                                        var figlio = f_p.FIGLI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).OrderByDescending(a => a.IDFIGLI).First();

                                        AssociaFigliPassaportoFigli(fp.IDFIGLIPASSAPORTO, figlio.IDFIGLI, db);

                                        var lDocIdentita = f_p.DOCUMENTI.Where(a =>
                                                a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();

                                        if (lDocIdentita.Count > 0)
                                        {
                                            foreach (var docIdentita in lDocIdentita)
                                            {
                                                this.AssociaDocumentoPassaportoFiglio(fp.IDFIGLIPASSAPORTO, docIdentita.IDDOCUMENTO, db);
                                            }
                                        }
                                    }
                                }
                                #endregion

                                ap = ap_new;
                            }
                            else
                            {
                                ap = ultima_ap_richiesta;
                            }

                            #region elenco coniugi
                            decimal ordine = 100;
                            var lc_ap = ap.CONIUGEPASSAPORTO.ToList();

                            foreach (var c_ap in lc_ap)
                            {
                                var coniuge = c_ap.CONIUGE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).OrderByDescending(a => a.IDCONIUGE).First();

                                var lDocIdentita = c_ap.DOCUMENTI.Where(a =>
                                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();

                                ElencoFamiliariPassaportoModel coniuge_no_pp = new ElencoFamiliariPassaportoModel()
                                {
                                    idAttivazionePassaporti = c_ap.IDATTIVAZIONIPASSAPORTI,
                                    idFamiliarePassaporto = c_ap.IDCONIUGEPASSAPORTO,
                                    nominativo = coniuge.COGNOME + " " + coniuge.NOME,
                                    codiceFiscale = coniuge.CODICEFISCALE,
                                    dataInizio = coniuge.DATAINIZIOVALIDITA,
                                    dataFine = coniuge.DATAFINEVALIDITA,
                                    parentela = EnumParentela.Coniuge,
                                    idAltriDati = coniuge.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).OrderByDescending(a => a.IDALTRIDATIFAM).First().IDALTRIDATIFAM,
                                    richiedi = c_ap.INCLUDIPASSAPORTO,
                                    HasDoc = new HasDoc()
                                    {
                                        esisteDoc = (lDocIdentita.Count > 0) ? true : false,
                                        tipoDoc = EnumTipoDoc.Documento_Identita
                                    },

                                    ordinamento = ordine,
                                    notificato = ap.NOTIFICARICHIESTA,
                                    attivato = ap.PRATICACONCLUSA,
                                    idFasePassaporti=(decimal)EnumFasePassaporti.Richiesta_Passaporti
                                };

                                lConiuge.Add(coniuge_no_pp);

                                ordine++;
                            }

                            if (lConiuge?.Any() ?? false)
                            {
                                lefm.AddRange(lConiuge);
                            }
                            #endregion

                            #region elenco figli
                               
                            var lf_ap = ap.FIGLIPASSAPORTO.ToList();

                            foreach (var f_ap in lf_ap)
                            {
                                var figli = f_ap.FIGLI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).OrderByDescending(a => a.IDFIGLI).First();

                                var lDocIdentita = f_ap.DOCUMENTI.Where(a =>
                                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();

                                ElencoFamiliariPassaportoModel figli_no_pp = new ElencoFamiliariPassaportoModel()
                                {
                                    idAttivazionePassaporti = f_ap.IDATTIVAZIONIPASSAPORTI,
                                    idFamiliarePassaporto = f_ap.IDFIGLIPASSAPORTO,
                                    nominativo = figli.COGNOME + " " + figli.NOME,
                                    codiceFiscale = figli.CODICEFISCALE,
                                    dataInizio = figli.DATAINIZIOVALIDITA,
                                    dataFine = figli.DATAFINEVALIDITA,
                                    parentela = EnumParentela.Figlio,
                                    idAltriDati = figli.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).OrderByDescending(a => a.IDALTRIDATIFAM).First().IDALTRIDATIFAM,
                                    richiedi = f_ap.INCLUDIPASSAPORTO,
                                    HasDoc = new HasDoc()
                                    {
                                        esisteDoc = (lDocIdentita.Count > 0) ? true : false,
                                        tipoDoc = EnumTipoDoc.Documento_Identita
                                    },

                                    ordinamento = ordine,
                                    notificato = ap.NOTIFICARICHIESTA,
                                    attivato = ap.PRATICACONCLUSA,
                                    idFasePassaporti = (decimal)EnumFasePassaporti.Richiesta_Passaporti
                                };

                                lFiglio.Add(figli_no_pp);

                                ordine++;
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


        public IList<ElencoFamiliariPassaportoModel> GetFamiliariInvioPassaporto(decimal idTrasferimento)
        {
            List<ElencoFamiliariPassaportoModel> lefm = new List<ElencoFamiliariPassaportoModel>();
            ElencoFamiliariPassaportoModel richiedente = new ElencoFamiliariPassaportoModel();
            List<ElencoFamiliariPassaportoModel> lConiuge = new List<ElencoFamiliariPassaportoModel>();
            List<ElencoFamiliariPassaportoModel> lFiglio = new List<ElencoFamiliariPassaportoModel>();

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
                        var p = t.PASSAPORTI;
                        var idPassaporti = p.IDPASSAPORTI;

                        if (p != null && idPassaporti > 0)
                        {
                            #region elenco fasi 2 non attive valide diverse da partenza
                            var prima_ap_invio = GetFaseInvioPassaporti_Partenza(idTrasferimento);
                            var lap_invio = p.ATTIVAZIONIPASSAPORTI
                                                    .Where(a => a.ANNULLATO == false && 
                                                                a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Invio_Passaporti &&
                                                                a.IDATTIVAZIONIPASSAPORTI!= prima_ap_invio.IDATTIVAZIONIPASSAPORTI &&
                                                                a.PRATICACONCLUSA==false)
                                                    .OrderBy(a=>a.IDATTIVAZIONIPASSAPORTI)
                                                    .ToList();
                            #endregion

                            if (lap_invio?.Any() ?? false)
                            {

                                if (lap_invio.First().PRATICACONCLUSA)
                                {
                                    //imposta grigio
                                }
                                else
                                {
                                    //imposta verde
                                }

                                decimal ordine = 100;

                                foreach (var ap_invio in lap_invio)
                                {
                                    #region coniugi assiociati a ap_invio
                                    var lcp_invio =
                                        ap_invio.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false)
                                            .OrderByDescending(a => a.IDCONIUGEPASSAPORTO);
                                    if (lcp_invio?.Any() ?? false)
                                    {

                                        foreach (var cp_invio in lcp_invio)
                                        {
                                            ordine++;
                                            var c = cp_invio.CONIUGE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).OrderByDescending(a => a.IDCONIUGE).ToList().First();


                                            idDocPassaporto = GetIdDocFamiliare((decimal)EnumTipoDoc.Passaporto, cp_invio.IDATTIVAZIONIPASSAPORTI, c.IDCONIUGE, (decimal)EnumParentela.Coniuge, db);
                                            idDocIdentita = GetIdDocFamiliare((decimal)EnumTipoDoc.Documento_Identita, cp_invio.IDATTIVAZIONIPASSAPORTI, c.IDCONIUGE, (decimal)EnumParentela.Coniuge, db);


                                            ElencoFamiliariPassaportoModel coniuge = new ElencoFamiliariPassaportoModel()
                                            {
                                                idAttivazionePassaporti = ap_invio.IDATTIVAZIONIPASSAPORTI,
                                                idFamiliarePassaporto = cp_invio.IDCONIUGEPASSAPORTO,
                                                nominativo = c.COGNOME + " " + c.NOME,
                                                codiceFiscale = c.CODICEFISCALE,
                                                dataInizio = c.DATAINIZIOVALIDITA,
                                                dataFine = c.DATAFINEVALIDITA,
                                                parentela = EnumParentela.Coniuge,
                                                idAltriDati = c.ALTRIDATIFAM.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                                            .OrderByDescending(a => a.IDALTRIDATIFAM).First()
                                                            .IDALTRIDATIFAM,
                                                notificato = ap_invio.NOTIFICARICHIESTA,
                                                attivato = ap_invio.PRATICACONCLUSA,
                                                // colore : if(attivato)?grigio:verde
                                                HasDoc = new HasDoc()
                                                {
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
                                                idFasePassaporti = (decimal)EnumFasePassaporti.Richiesta_Passaporti
                                            };

                                            lConiuge.Add(coniuge);

                                        }
                                        if (lConiuge?.Any() ?? false)
                                        {
                                            lefm.AddRange(lConiuge);
                                        }

                                    }

                                   

                                    #endregion

                                    #region Figli associati a ap_invio
                                    var lfp_invio =
                                        ap_invio.FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false)
                                        .OrderByDescending(a => a.IDFIGLIPASSAPORTO);
                                    if (lfp_invio?.Any() ?? false)
                                    {
                                        foreach (var fp_invio in lfp_invio)
                                        {
                                            ordine++;
                                            var f = fp_invio.FIGLI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).OrderByDescending(a => a.IDFIGLI).ToList().First();

                                            idDocPassaporto = GetIdDocFamiliare((decimal)EnumTipoDoc.Passaporto, fp_invio.IDATTIVAZIONIPASSAPORTI, f.IDFIGLI, (decimal)EnumParentela.Figlio, db);
                                            idDocIdentita = GetIdDocFamiliare((decimal)EnumTipoDoc.Documento_Identita, fp_invio.IDATTIVAZIONIPASSAPORTI, f.IDFIGLI, (decimal)EnumParentela.Figlio, db);

                                            ElencoFamiliariPassaportoModel figlio = new ElencoFamiliariPassaportoModel()
                                            {
                                                idAttivazionePassaporti = fp_invio.IDATTIVAZIONIPASSAPORTI,
                                                idFamiliarePassaporto = fp_invio.IDFIGLIPASSAPORTO,
                                                nominativo = f.COGNOME + " " + f.NOME,
                                                codiceFiscale = f.CODICEFISCALE,
                                                dataInizio = f.DATAINIZIOVALIDITA,
                                                dataFine = f.DATAFINEVALIDITA,
                                                parentela = EnumParentela.Figlio,
                                                idAltriDati = f.ALTRIDATIFAM.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDALTRIDATIFAM).First().IDALTRIDATIFAM,
                                                notificato = ap_invio.NOTIFICARICHIESTA,
                                                // colore : if(attivato)?grigio:verde
                                                attivato = ap_invio.PRATICACONCLUSA,
                                                HasDoc = new HasDoc()
                                                {
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
                                                idFasePassaporti = (decimal)EnumFasePassaporti.Richiesta_Passaporti
                                            };
                                            lFiglio.Add(figlio);
                                        }
                                        if (lFiglio?.Any() ?? false)
                                        {
                                            lefm.AddRange(lFiglio);
                                        }

                                    }

                                    
                                    #endregion
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
            if (p==null)
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
                    idFasePassaporti=idFasePassaporti
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
                    if (lcp?.Any()??false)
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
                    var lfp = ap.FIGLIPASSAPORTO.Where(a=>a.ANNULLATO==false);
                    if (lfp?.Any()??false)
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
                        var cp = db.CONIUGEPASSAPORTO.Find(idFamiliarePassaporto);

                        if (cp.IDCONIUGEPASSAPORTO>0)
                        {
                            var c = cp.CONIUGE.First();

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
                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita && a.IDSTATORECORD==(decimal)EnumStatoRecord.Attivato).ToList();

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
                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto && a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();


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
                        var fp =db.FIGLIPASSAPORTO.Find(idFamiliarePassaporto);

                        if (fp.IDFIGLIPASSAPORTO>0)
                        {
                            var f = fp.FIGLI.First();

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
                                idDocPassaporto=idDocPassaporto
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
                                                        a.IDFASEPASSAPORTI==(decimal)EnumFasePassaporti.Invio_Passaporti).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI);

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
                var lap = p.ATTIVAZIONIPASSAPORTI.Where(a=>a.ANNULLATO==false && 
                            a.IDFASEPASSAPORTI==(decimal)EnumFasePassaporti.Richiesta_Passaporti).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI).ToList();
                if (lap?.Any() ?? false)
                {
                    var ap = lap.First();

                    gp.idAttivazionePassaporto = ap.IDATTIVAZIONIPASSAPORTI;
                    gp.notificaRichiesta = ap.NOTIFICARICHIESTA;
                    gp.praticaConclusa = ap.PRATICACONCLUSA;
                    gp.annullata = ap.ANNULLATO;

                  

                    var lcp = ap.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false && a.INCLUDIPASSAPORTO == true).ToList();

                    if (lcp?.Any() ?? false)
                    {
                        gp.coniugeIncluso = true;
                    }

                    var lfp = ap.FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false && a.INCLUDIPASSAPORTO == true).ToList();

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

                    //var lpr = ap.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false && a.INCLUDIPASSAPORTO == true);

                    //if (lpr?.Any() ?? false)
                    //{
                    //    gp.richiedenteIncluso = true;

                    //    var pr = lpr.First();
                    //    var ldr = pr.DOCUMENTI.Where(a =>   
                    //                                    a.MODIFICATO == false && 
                    //                                    a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                    //                                    a.IDTIPODOCUMENTO==(decimal)EnumTipoDoc.Passaporto);
                    //    if (ldr?.Any() ?? false)
                    //    {
                    //        gp.passaportoRichiedente = true;
                    //    }

                    //    if(gp.passaportoRichiedente&&gp.richiedenteIncluso)
                    //    {
                    //        gp.notificabile = true;
                    //    }
                    //    else
                    //    {
                    //        gp.notificabile = false;
                    //    }
                    //}

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
                        }else
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


        public EnumFasePassaporti GetFasePassaporti_Da_Elaborare(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                EnumFasePassaporti fasePassaporti= EnumFasePassaporti.Richiesta_Passaporti;

                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var p = t.PASSAPORTI;
                var lap = p.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI).ToList();
                if (lap?.Any()??false)
                {
                    var ap = lap.First();

                    //se è attivata verifico imposto la fese successiva
                    if (ap.PRATICACONCLUSA)
                    {
                        //se ho concluso la fase richiesta imposto la fase invio altrimenti imposto fase richiesta
                        if (ap.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Richiesta_Passaporti)
                        {
                            fasePassaporti = EnumFasePassaporti.Invio_Passaporti;
                        }
                        else
                        {
                            fasePassaporti = EnumFasePassaporti.Richiesta_Passaporti;
                        }

                    }
                    else
                    {
                        //se non è attivata imposto la fase da attivare
                        fasePassaporti = (EnumFasePassaporti)ap.IDFASEPASSAPORTI;
                    }
                }
                return fasePassaporti;
            }
        }


        public GestioneChkincludiPassaportoModel GetGestioneInludiPassaporto_var(decimal idAttivazionePassaporto, decimal idFamiliarePassaporto, EnumParentela parentela, bool esisteDoc, bool includiPassaporto, bool notificato)
        {
            GestioneChkincludiPassaportoModel gcip = new GestioneChkincludiPassaportoModel();
            bool dchk = false;

            using (ModelDBISE db = new ModelDBISE())
            {
                ATTIVAZIONIPASSAPORTI ap = new ATTIVAZIONIPASSAPORTI();

                try
                {
                    ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazionePassaporto);

                    if (ap?.IDATTIVAZIONIPASSAPORTI <= 0)
                    {
                        throw new Exception("Ciclo di attivazione non presente.");
                    }

                    var attivazioneFaseRichiesta = FaseRichiestaPassaporti(ap.IDPASSAPORTI);
                    var attivazioneFaseInvio = FaseInvioPassaporti(ap.IDPASSAPORTI);

                    bool faseRichiesta = false;
                    bool faseRichiestaNotificata = false;
                    bool faseRichiestaAttivata = false;
                    bool faseInvio = false;
                    bool faseInvioNotificata = false;
                    bool faseInvioAttivata = false;
                    if (attivazioneFaseRichiesta.IDATTIVAZIONIPASSAPORTI > 0)
                    {
                        faseRichiesta = true;
                        faseRichiestaAttivata = attivazioneFaseRichiesta.PRATICACONCLUSA;
                        faseRichiestaNotificata = attivazioneFaseRichiesta.NOTIFICARICHIESTA;
                    }
                    if (attivazioneFaseInvio.IDATTIVAZIONIPASSAPORTI > 0)
                    {
                        faseInvio = true;
                        faseInvioNotificata = attivazioneFaseInvio.NOTIFICARICHIESTA;
                        faseInvioAttivata = attivazioneFaseInvio.PRATICACONCLUSA;
                    }

                    if (ap.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Invio_Passaporti)
                    {
                        if (faseInvioAttivata == false && faseInvioNotificata) 
                        {
                            dchk = true;
                        }
                    }


                    if (ap.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Richiesta_Passaporti)
                    {
                        if (faseRichiestaNotificata)
                        {
                            dchk = true;
                        }
                    }
                    //if(includiPassaporto  || parentela == EnumParentela.Richiedente)
                    //{
                    //    dchk = true;
                    //}
                    if(ap.PASSAPORTI.TRASFERIMENTO.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Annullato || ap.PASSAPORTI.TRASFERIMENTO.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato)
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


        public ATTIVAZIONIPASSAPORTI FaseRichiestaPassaporti(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                ATTIVAZIONIPASSAPORTI ap = new ATTIVAZIONIPASSAPORTI();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var p = t.PASSAPORTI;
                var lap = p.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false && a.IDFASEPASSAPORTI==(decimal)EnumFasePassaporti.Richiesta_Passaporti).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI).ToList();
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

        public ATTIVAZIONIPASSAPORTI GetFaseInvioPassaporti_Partenza(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                ATTIVAZIONIPASSAPORTI ap = new ATTIVAZIONIPASSAPORTI();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var p = t.PASSAPORTI;
                ap = p.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false && a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Invio_Passaporti).OrderBy(a => a.IDATTIVAZIONIPASSAPORTI).ToList().First();
                return ap;
            }
        }
        public ATTIVAZIONIPASSAPORTI GetFaseRichiestaPassaporti_Partenza(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                ATTIVAZIONIPASSAPORTI ap = new ATTIVAZIONIPASSAPORTI();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var p = t.PASSAPORTI;
                ap = p.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false && a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Richiesta_Passaporti).OrderBy(a => a.IDATTIVAZIONIPASSAPORTI).ToList().First();
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
                            a.IDFASEPASSAPORTI==(decimal)EnumFasePassaporti.Invio_Passaporti &&
                            a.NOTIFICARICHIESTA==false)
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
                        idFasePassaporti=ap.IDFASEPASSAPORTI
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
                    idFasePassaporti=new_ap.IDFASEPASSAPORTI,
                    notificaRichiesta = new_ap.NOTIFICARICHIESTA,
                    dataNotificaRichiesta = new_ap.DATANOTIFICARICHIESTA,
                    praticaConclusa = new_ap.PRATICACONCLUSA,
                    dataPraticaConclusa = new_ap.DATAPRATICACONCLUSA,
                    dataVariazione = new_ap.DATAVARIAZIONE,
                    dataAggiornamento = new_ap.DATAAGGIORNAMENTO,
                    annullato = new_ap.ANNULLATO
                };

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova attivazione passaporti.", "ATTIVITAZIONEMAB", db, idTrasferimento, new_ap.IDATTIVAZIONIPASSAPORTI);
            }

            return new_apm;
        }

        public AttivazionePassaportiModel CreaAttivazioneRichiestaPassaporti(decimal idTrasferimento, ModelDBISE db)
        {
            AttivazionePassaportiModel new_apm = new AttivazionePassaportiModel();

            ATTIVAZIONIPASSAPORTI new_ap = new ATTIVAZIONIPASSAPORTI()
            {
                IDPASSAPORTI = idTrasferimento,
                NOTIFICARICHIESTA = false,
                IDFASEPASSAPORTI = (decimal)EnumFasePassaporti.Richiesta_Passaporti,
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
                throw new Exception(string.Format("Non è stato possibile creare una nuova attivazione per la fese di richiesta dei passaporti."));
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

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova attivazione passaporti.", "ATTIVITAZIONEPASSAPORTI", db, idTrasferimento, new_ap.IDATTIVAZIONIPASSAPORTI);
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

                    var apl = t.PASSAPORTI.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false && a.IDFASEPASSAPORTI==(decimal)EnumFasePassaporti.Invio_Passaporti && a.NOTIFICARICHIESTA == false).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI).ToList();

                    if (apl?.Any() ?? false)
                    {
                        ap = apl.First();

                        switch ((EnumParentela)idParentela)
                        {
                            case EnumParentela.Coniuge:
                                //var cpl = ap.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false && a.IDCONIUGE == idFamiliare).ToList();//. && ..DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == (decimal)TipoDocumento).ToList();
                                var cp = db.CONIUGEPASSAPORTO.Find(idFamiliare);

                                //if (cpl?.Any() ?? false)
                                if (cp.IDCONIUGEPASSAPORTO>0)
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
                                if (fp.IDFIGLIPASSAPORTO>0)
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
                        var ldr = pr.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO==(decimal)EnumTipoDoc.Passaporto);
                        if (ldr?.Any() ?? false)
                        {
                            var dr = ldr.First();
                            valore= dr.IDDOCUMENTO;
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
                            valore= dc.IDDOCUMENTO;
                        }
                    }
                    break;

                case EnumParentela.Figlio:
                    var lpf =db.FIGLI.Find(idFamiliare).FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false);
                    if (lpf?.Any() ?? false)
                    {
                        var pf = lpf.First();
                        var ldf = pf.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Passaporto);
                        if (ldf?.Any() ?? false)
                        {
                            var df = ldf.First();
                            valore= df.IDDOCUMENTO;
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
                    var lpc = db.CONIUGE.Find(idFamiliare).CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false);
                    if (lpc?.Any() ?? false)
                    {
                        var pc = lpc.First();
                        var ldc = pc.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO ==  idTipoDoc);
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

        public decimal GetNumAttivazioniRichiesta(decimal idPassaporto, ModelDBISE db)
        {
         
            var p = db.PASSAPORTI.Find(idPassaporto);

            var conta = p.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false && a.IDFASEPASSAPORTI==(decimal)EnumFasePassaporti.Richiesta_Passaporti).ToList().Count();

            return conta;
            
        }

        public ATTIVAZIONIPASSAPORTI GetUltimaFasePassaporti_Richiesta(decimal idPassaporti,ModelDBISE db)
        {
            List<ATTIVAZIONIPASSAPORTI> lap = new List<ATTIVAZIONIPASSAPORTI>();
            ATTIVAZIONIPASSAPORTI ap = new ATTIVAZIONIPASSAPORTI();

            var p = db.PASSAPORTI.Find(idPassaporti);
            lap = p.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false && a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Richiesta_Passaporti).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI).ToList();
            if (lap?.Any() ?? false)
            {
                ap = lap.First();
            }
            return ap;
        }

        public ATTIVAZIONIPASSAPORTI GetUltimaFasePassaporti_Invio(decimal idPassaporti, ModelDBISE db)
        {
            List<ATTIVAZIONIPASSAPORTI> lap = new List<ATTIVAZIONIPASSAPORTI>();
            ATTIVAZIONIPASSAPORTI ap = new ATTIVAZIONIPASSAPORTI();

            var p = db.PASSAPORTI.Find(idPassaporti);
            lap = p.ATTIVAZIONIPASSAPORTI.Where(a => a.ANNULLATO == false && a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Invio_Passaporti).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI).ToList();
            if (lap?.Any() ?? false)
            {
                ap = lap.First();
            }
            return ap;
        }

        public IList<ElencoFamiliariPassaportoModel> GetFamiliariPassaportoCompletato(decimal idTrasferimento)
        {
            List<ElencoFamiliariPassaportoModel> lefm = new List<ElencoFamiliariPassaportoModel>();
            List<ElencoFamiliariPassaportoModel> lConiuge = new List<ElencoFamiliariPassaportoModel>();
            List<ElencoFamiliariPassaportoModel> lFiglio = new List<ElencoFamiliariPassaportoModel>();

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
                        var p = t.PASSAPORTI;
                        var idPassaporti = p.IDPASSAPORTI;

                        if (p != null && idPassaporti > 0)
                        {
                            #region elenco fasi 2 attive diverse da partenza
                            var prima_ap_invio = GetFaseInvioPassaporti_Partenza(idTrasferimento);
                            var lap_invio_attiva = p.ATTIVAZIONIPASSAPORTI
                                                    .Where(a => a.ANNULLATO == false &&
                                                                a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Invio_Passaporti &&
                                                                a.IDATTIVAZIONIPASSAPORTI != prima_ap_invio.IDATTIVAZIONIPASSAPORTI &&
                                                                a.PRATICACONCLUSA)
                                                    .OrderBy(a => a.IDATTIVAZIONIPASSAPORTI)
                                                    .ToList();
                            #endregion

                            if (lap_invio_attiva?.Any() ?? false)
                            {
                                decimal ordine = 100;

                                foreach (var ap_invio_attiva in lap_invio_attiva)
                                {
                                    #region coniugi associati a ap_invio_attiva
                                    var lcp_invio_attiva =
                                        ap_invio_attiva.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false)
                                            .OrderByDescending(a => a.IDCONIUGEPASSAPORTO);
                                    if (lcp_invio_attiva?.Any() ?? false)
                                    {
                                        foreach (var cp_invio_attiva in lcp_invio_attiva)
                                        {
                                            ordine++;
                                            var c = cp_invio_attiva.CONIUGE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).OrderByDescending(a=>a.IDCONIUGE).ToList().First();

                                            idDocPassaporto = GetIdDocFamiliare((decimal)EnumTipoDoc.Passaporto, cp_invio_attiva.IDATTIVAZIONIPASSAPORTI, c.IDCONIUGE, (decimal)EnumParentela.Coniuge, db);
                                            idDocIdentita = GetIdDocFamiliare((decimal)EnumTipoDoc.Documento_Identita, cp_invio_attiva.IDATTIVAZIONIPASSAPORTI, c.IDCONIUGE, (decimal)EnumParentela.Coniuge, db);

                                            ElencoFamiliariPassaportoModel coniuge = new ElencoFamiliariPassaportoModel()
                                            {
                                                idAttivazionePassaporti = ap_invio_attiva.IDATTIVAZIONIPASSAPORTI,
                                                idFamiliarePassaporto = cp_invio_attiva.IDCONIUGEPASSAPORTO,
                                                nominativo = c.COGNOME + " " + c.NOME,
                                                codiceFiscale = c.CODICEFISCALE,
                                                dataInizio = c.DATAINIZIOVALIDITA,
                                                dataFine = c.DATAFINEVALIDITA,
                                                parentela = EnumParentela.Coniuge,
                                                idAltriDati = c.ALTRIDATIFAM.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                                            .OrderByDescending(a => a.IDALTRIDATIFAM).First()
                                                            .IDALTRIDATIFAM,
                                                notificato = ap_invio_attiva.NOTIFICARICHIESTA,
                                                attivato = ap_invio_attiva.PRATICACONCLUSA,
                                                HasDoc = new HasDoc()
                                                {
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
                                            };

                                            lefm.Add(coniuge);

                                        }
                                        //if (lConiuge?.Any() ?? false)
                                        //{
                                        //    lefm.AddRange(lConiuge);
                                        //}

                                    }

                                  
                                    #endregion

                                    #region Figli associati a ap_invio_attiva
                                    var lfp_invio_attiva =
                                        ap_invio_attiva.FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false)
                                        .OrderByDescending(a => a.IDFIGLIPASSAPORTO);
                                    if (lfp_invio_attiva?.Any() ?? false)
                                    {
                                        foreach (var fp_invio_attiva in lfp_invio_attiva)
                                        {
                                            ordine++;
                                            var f = fp_invio_attiva.FIGLI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).OrderByDescending(a => a.IDFIGLI).ToList().First();


                                            idDocPassaporto = GetIdDocFamiliare((decimal)EnumTipoDoc.Passaporto, fp_invio_attiva.IDATTIVAZIONIPASSAPORTI, f.IDFIGLI, (decimal)EnumParentela.Figlio, db);
                                            idDocIdentita = GetIdDocFamiliare((decimal)EnumTipoDoc.Documento_Identita, fp_invio_attiva.IDATTIVAZIONIPASSAPORTI, f.IDFIGLI, (decimal)EnumParentela.Figlio, db);

                                            ElencoFamiliariPassaportoModel figlio = new ElencoFamiliariPassaportoModel()
                                            {
                                                idAttivazionePassaporti = fp_invio_attiva.IDATTIVAZIONIPASSAPORTI,
                                                idFamiliarePassaporto = fp_invio_attiva.IDFIGLIPASSAPORTO,
                                                nominativo = f.COGNOME + " " + f.NOME,
                                                codiceFiscale = f.CODICEFISCALE,
                                                dataInizio = f.DATAINIZIOVALIDITA,
                                                dataFine = f.DATAFINEVALIDITA,
                                                parentela = EnumParentela.Figlio,
                                                idAltriDati = f.ALTRIDATIFAM.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDALTRIDATIFAM).First().IDALTRIDATIFAM,
                                                notificato = ap_invio_attiva.NOTIFICARICHIESTA,
                                                attivato = ap_invio_attiva.PRATICACONCLUSA,
                                                HasDoc = new HasDoc()
                                                {
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
                                            };
                                            lefm.Add(figlio);
                                        }
                                        //if (lFiglio?.Any() ?? false)
                                        //{
                                        //    lefm.AddRange(lFiglio);
                                        //}

                                    }

                                    
                                    #endregion
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

            return lefm;
        }

        public ATTIVAZIONIPASSAPORTI GetUltimaFasePassaporti_Richiesta_Da_Notificare(decimal idPassaporti, ModelDBISE db)
        {
            List<ATTIVAZIONIPASSAPORTI> lap = new List<ATTIVAZIONIPASSAPORTI>();
            ATTIVAZIONIPASSAPORTI ap = new ATTIVAZIONIPASSAPORTI();

            var p = db.PASSAPORTI.Find(idPassaporti);
            lap = p.ATTIVAZIONIPASSAPORTI.Where(
                    a => a.ANNULLATO == false && 
                    a.IDFASEPASSAPORTI == (decimal)EnumFasePassaporti.Richiesta_Passaporti &&
                    a.NOTIFICARICHIESTA==false
                    ).OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI).ToList();
            if (lap?.Any() ?? false)
            {
                ap = lap.First();
            }
            return ap;
        }

        public CONIUGEPASSAPORTO GetConiugePassaportoByIDConiuge(decimal id, ModelDBISE db)
        {
            CONIUGEPASSAPORTO cp = new CONIUGEPASSAPORTO();

            var c = db.CONIUGE.Find(id);

            var lcp = c.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDCONIUGEPASSAPORTO).ToList();
            if(lcp?.Any()??false)
            {
                cp = lcp.First();
            }

            return cp;
        }

        public FIGLIPASSAPORTO GetFigliPassaportoByIDFigli(decimal id, ModelDBISE db)
        {
            FIGLIPASSAPORTO fp = new FIGLIPASSAPORTO();

            var f = db.FIGLI.Find(id);

            var lfp = f.FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDFIGLIPASSAPORTO).ToList();
            if (lfp?.Any() ?? false)
            {
                fp = lfp.First();
            }

            return fp;
        }


    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using NewISE.EF;
using Newtonsoft.Json.Schema;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;

using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models.ModelRest;
using System.Diagnostics;
using System.IO;
using NewISE.Models.Config;
using NewISE.Models.Config.s_admin;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtAnticipi : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public ATTIVITAANTICIPI CreaAttivitaAnticipi(decimal idPrimaSistemazione, ModelDBISE db)
        {
            ATTIVITAANTICIPI new_aa = new ATTIVITAANTICIPI()
            {
                IDPRIMASISTEMAZIONE = idPrimaSistemazione,
                NOTIFICARICHIESTA = false,
                DATAATTIVARICHIESTA = null,
                ATTIVARICHIESTA = false,
                DATANOTIFICARICHIESTA = null,
                ANNULLATO = false,
                DATAAGGIORNAMENTO = DateTime.Now,
            };
            db.ATTIVITAANTICIPI.Add(new_aa);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception(string.Format("Non è stato possibile creare una nuova attivazione per gli anticipi."));
            }
            else
            {
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova attivazione anticipi.", "ATTIVITAANTICIPI", db, new_aa.IDPRIMASISTEMAZIONE, new_aa.IDATTIVITAANTICIPI);
            }

            return new_aa;
        }


        public decimal GetNumAttivazioniAnticipi(decimal idPrimaSistemazione)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var NumAttivazioni = 0;
                NumAttivazioni = db.PRIMASITEMAZIONE.Find(idPrimaSistemazione).ATTIVITAANTICIPI
                                    .Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == true)
                                    .OrderByDescending(a => a.IDATTIVITAANTICIPI).Count();
                return NumAttivazioni;
            }
        }



        public ANTICIPI CreaAnticipi(decimal idAttivitaAnticipi, ModelDBISE db)
        {
            ANTICIPI new_a = new ANTICIPI()
            {
                IDATTIVITAANTICIPI = idAttivitaAnticipi,
                IDTIPOLOGIAANTICIPI = (decimal)EnumTipologiaAnticipi.Prima_Sistemazione,
                PERCENTUALEANTICIPO = 0,
                ANNULLATO = false,
                DATAAGGIORNAMENTO = DateTime.Now,
            };
            db.ANTICIPI.Add(new_a);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception(string.Format("Non è stato possibile creare un nuovo anticipo."));
            }
            else
            {
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo anticipo.", "ANTICIPI", db, new_a.ATTIVITAANTICIPI.PRIMASITEMAZIONE.TRASFERIMENTO.IDTRASFERIMENTO, new_a.IDATTIVITAANTICIPI);
            }

            return new_a;
        }


        public AttivitaAnticipiModel GetUltimaAttivitaAnticipi(decimal idPrimaSistemazione)
        {
            try
            {
                AttivitaAnticipiModel aam = new AttivitaAnticipiModel();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var ps = db.PRIMASITEMAZIONE.Find(idPrimaSistemazione);

                    var aal = ps.ATTIVITAANTICIPI.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVITAANTICIPI).ToList();

                    if (aal?.Any() ?? false)
                    {
                        var aa = aal.First();

                        aam = new AttivitaAnticipiModel()
                        {
                            idAttivitaAnticipi = aa.IDATTIVITAANTICIPI,
                            idPrimaSistemazione = aa.IDPRIMASISTEMAZIONE,
                            notificaRichiestaAnticipi = aa.NOTIFICARICHIESTA,
                            dataNotificaRichiesta = aa.DATANOTIFICARICHIESTA,
                            attivaRichiestaAnticipi = aa.ATTIVARICHIESTA,
                            dataAttivaRichiesta = aa.DATAATTIVARICHIESTA,
                            dataAggiornamento = aa.DATAAGGIORNAMENTO,
                            annullato = aa.ANNULLATO
                        };
                    }
                    else
                    {
                        var aa_new = this.CreaAttivitaAnticipi(idPrimaSistemazione, db);

                        aam = new AttivitaAnticipiModel()
                        {
                            idAttivitaAnticipi = aa_new.IDATTIVITAANTICIPI,
                            idPrimaSistemazione = aa_new.IDPRIMASISTEMAZIONE,
                            notificaRichiestaAnticipi = aa_new.NOTIFICARICHIESTA,
                            dataNotificaRichiesta = aa_new.DATANOTIFICARICHIESTA,
                            attivaRichiestaAnticipi = aa_new.ATTIVARICHIESTA,
                            dataAttivaRichiesta = aa_new.DATAATTIVARICHIESTA,
                            dataAggiornamento = aa_new.DATAAGGIORNAMENTO,
                            annullato = aa_new.ANNULLATO
                        };
                    }
                }

                return aam;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public AnticipiViewModel GetAnticipi(decimal idAttivitaAnticipi)
        {
            try
            {
                AnticipiViewModel avm = new AnticipiViewModel();
                ANTICIPI a = new ANTICIPI();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var idTrasferimento = db.ATTIVITAANTICIPI.Find(idAttivitaAnticipi).PRIMASITEMAZIONE.TRASFERIMENTO.IDTRASFERIMENTO;

                    using (CalcoliIndennita ci = new CalcoliIndennita(idTrasferimento))
                    {
                        var importoPrevisto = Math.Round(ci.anticipoIndennitaSistemazioneLorda, 2);

                        var al = db.ANTICIPI.Where(x => x.IDATTIVITAANTICIPI == idAttivitaAnticipi).ToList();

                        if (al?.Any() ?? false)
                        {
                            a = al.First();

                            avm = new AnticipiViewModel()
                            {
                                idAttivitaAnticipi = a.IDATTIVITAANTICIPI,
                                idTipologiaAnticipi = a.IDTIPOLOGIAANTICIPI,
                                dataAggiornamento = a.DATAAGGIORNAMENTO,
                                annullato = a.ANNULLATO,
                                ImportoPrevisto = importoPrevisto,
                                PercentualeAnticipoRichiesto = a.PERCENTUALEANTICIPO
                            };
                        }
                        else
                        {
                            a = this.CreaAnticipi(idAttivitaAnticipi, db);

                            var new_avm = new AnticipiViewModel()
                            {
                                idAttivitaAnticipi = a.IDATTIVITAANTICIPI,
                                idTipologiaAnticipi = a.IDTIPOLOGIAANTICIPI,
                                dataAggiornamento = a.DATAAGGIORNAMENTO,
                                annullato = a.ANNULLATO,
                                ImportoPrevisto = importoPrevisto,
                                PercentualeAnticipoRichiesto = a.PERCENTUALEANTICIPO
                            };

                            avm = new_avm;
                        }
                    }
                }

                return avm;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public decimal CalcolaImportoPercepito(decimal idAttivitaAnticipi, decimal percRichiesta)
        {
            try
            {
                decimal importoPercepito;

                using (ModelDBISE db = new ModelDBISE())
                {
                    var idTrasferimento = db.ATTIVITAANTICIPI.Find(idAttivitaAnticipi).PRIMASITEMAZIONE.TRASFERIMENTO.IDTRASFERIMENTO;

                    using (CalcoliIndennita ci = new CalcoliIndennita(idTrasferimento))
                    {
                        var importoPrevisto = ci.anticipoIndennitaSistemazioneLorda;

                        importoPercepito = Math.Round((importoPrevisto * (percRichiesta / 100)), 2);

                    }
                }

                return importoPercepito;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public void NotificaRichiestaAnticipi(decimal idAttivitaAnticipi, decimal percentualeRichiesta)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var aa = db.ATTIVITAANTICIPI.Find(idAttivitaAnticipi);

                        var a = aa.ANTICIPI.Where(x => x.IDTIPOLOGIAANTICIPI == (decimal)EnumTipologiaAnticipi.Prima_Sistemazione).First();

                        if (a != null && a.IDATTIVITAANTICIPI > 0)
                        {
                            a.PERCENTUALEANTICIPO = percentualeRichiesta;

                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore nella fase di aggiornamento della percentuale di anticipo richiesta.");
                            }
                            else
                            {
                                aa.NOTIFICARICHIESTA = true;
                                aa.DATANOTIFICARICHIESTA = DateTime.Now;
                                aa.DATAAGGIORNAMENTO = DateTime.Now;

                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore nella fase d'inserimento per la richiesta attivazione anticipi.");
                                }
                                else
                                {
                                    this.EmailNotificaRichiestaAnticipi(idAttivitaAnticipi, db);

                                    using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                    {
                                        CalendarioEventiModel cem = new CalendarioEventiModel()
                                        {
                                            idFunzioneEventi = EnumFunzioniEventi.RichiestaAnticipi,
                                            idTrasferimento = aa.PRIMASITEMAZIONE.IDPRIMASISTEMAZIONE,
                                            DataInizioEvento = DateTime.Now.Date,
                                            DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaAnticipi)).Date,
                                        };

                                        dtce.InsertCalendarioEvento(ref cem, db);
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

        public void AnnullaRichiestaAnticipi(decimal idAttivitaAnticipi)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var aa_Old = db.ATTIVITAANTICIPI.Find(idAttivitaAnticipi);

                    if (aa_Old?.IDATTIVITAANTICIPI > 0)
                    {
                        if (aa_Old.NOTIFICARICHIESTA == true && aa_Old.ATTIVARICHIESTA == false && aa_Old.ANNULLATO == false)
                        {
                            aa_Old.ANNULLATO = true;
                            aa_Old.DATAAGGIORNAMENTO = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore - Impossibile annullare la notifica della richiesta anticipo.");
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Annullamento della riga per il ciclo di attivazione della richiesta anticipo",
                                    "ATTIVITAANTICIPI", db, aa_Old.PRIMASITEMAZIONE.TRASFERIMENTO.IDTRASFERIMENTO,
                                    aa_Old.IDATTIVITAANTICIPI);

                                ATTIVITAANTICIPI aa_New = new ATTIVITAANTICIPI()
                                {
                                    IDPRIMASISTEMAZIONE = aa_Old.IDPRIMASISTEMAZIONE,
                                    NOTIFICARICHIESTA = false,
                                    ATTIVARICHIESTA = false,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    ANNULLATO = false
                                };

                                db.ATTIVITAANTICIPI.Add(aa_New);

                                int j = db.SaveChanges();

                                if (j <= 0)
                                {
                                    throw new Exception("Errore - Impossibile creare il nuovo ciclo di attivazione per richiesta anticipi.");
                                }
                                else
                                {
                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                        "Inserimento di una nuova riga per il ciclo di attivazione relativo alla richiesta anticipo.",
                                        "ATTIVITAANTICIPI", db, aa_New.PRIMASITEMAZIONE.TRASFERIMENTO.IDTRASFERIMENTO,
                                        aa_New.IDATTIVITAANTICIPI);

                                    #region anticipo
                                    var ant_Old =
                                        aa_Old.ANTICIPI.Where(
                                            a => a.ANNULLATO == false && a.IDTIPOLOGIAANTICIPI == (decimal)EnumTipologiaAnticipi.Prima_Sistemazione).First();

                                    if (ant_Old != null && ant_Old.IDATTIVITAANTICIPI > 0)
                                    {
                                        ANTICIPI ant_New = new ANTICIPI()
                                        {
                                            IDATTIVITAANTICIPI = aa_New.IDATTIVITAANTICIPI,
                                            IDTIPOLOGIAANTICIPI = ant_Old.IDTIPOLOGIAANTICIPI,
                                            PERCENTUALEANTICIPO = ant_Old.PERCENTUALEANTICIPO,
                                            DATAAGGIORNAMENTO = ant_Old.DATAAGGIORNAMENTO,
                                            ANNULLATO = ant_Old.ANNULLATO
                                        };

                                        db.ANTICIPI.Add(ant_New);

                                        int y = db.SaveChanges();

                                        if (y <= 0)
                                        {
                                            throw new Exception("Errore - Impossibile inserire il record relativo a richiesta anticipo.");
                                        }
                                        else
                                        {
                                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                "Inserimento di una nuova riga per la richiesta anticipo.",
                                                "ANTICIPI", db,
                                                aa_New.PRIMASITEMAZIONE.TRASFERIMENTO.IDTRASFERIMENTO,
                                                ant_New.IDATTIVITAANTICIPI);
                                        }

                                    }


                                }
                                #endregion


                                this.EmailAnnullaRichiestaAnticipi(aa_New.IDATTIVITAANTICIPI, db);
                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.AnnullaMessaggioEvento(aa_New.PRIMASITEMAZIONE.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaAnticipi, db);
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

        public void AttivaRichiestaAnticipi(decimal idAttivitaAnticipi)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var aa = db.ATTIVITAANTICIPI.Find(idAttivitaAnticipi);
                    if (aa?.IDATTIVITAANTICIPI > 0)
                    {
                        if (aa.NOTIFICARICHIESTA == true)
                        {
                            aa.ATTIVARICHIESTA = true;
                            aa.DATAATTIVARICHIESTA = DateTime.Now;
                            aa.DATAAGGIORNAMENTO = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore: Impossibile completare l'attivazione anticipo.");
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Attivazione anticipi.", "ATTIVITAANTICIPI", db,
                                    aa.PRIMASITEMAZIONE.TRASFERIMENTO.IDTRASFERIMENTO, aa.IDATTIVITAANTICIPI);
                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.ModificaInCompletatoCalendarioEvento(aa.PRIMASITEMAZIONE.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaAnticipi, db);
                                }

                                this.EmailAttivaRichiestaAnticipi(aa.IDATTIVITAANTICIPI, db);

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

        public void EmailAnnullaRichiestaAnticipi(decimal idAttivitaAnticipi, ModelDBISE db)
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

                var aa = db.ATTIVITAANTICIPI.Find(idAttivitaAnticipi);

                if (aa?.IDATTIVITAANTICIPI > 0)
                {
                    TRASFERIMENTO tr = aa.PRIMASITEMAZIONE.TRASFERIMENTO;
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
                            Resources.msgEmail.OggettoAnnullaRichiestaAnticipi;
                            msgMail.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaAnticipi, uff.DESCRIZIONEUFFICIO + " (" + uff.CODICEUFFICIO + ")", tr.DATAPARTENZA.ToLongDateString());

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

        private void EmailAttivaRichiestaAnticipi(decimal idAttivitaAnticipi, ModelDBISE db)
        {
            PRIMASITEMAZIONE ps = new PRIMASITEMAZIONE();
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

                var aa = db.ATTIVITAANTICIPI.Find(idAttivitaAnticipi);

                ps = aa.PRIMASITEMAZIONE;

                if (ps?.IDPRIMASISTEMAZIONE > 0)
                {
                    TRASFERIMENTO tr = ps.TRASFERIMENTO;
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
                                msgMail.oggetto = Resources.msgEmail.OggettoAttivaRichiestaAnticipi;

                                msgMail.corpoMsg =
                                        string.Format(
                                            Resources.msgEmail.MessaggioAttivaRichiestaAnticipi,
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

        private void EmailNotificaRichiestaAnticipi(decimal idAttivitaAnticipi, ModelDBISE db)
        {
            PRIMASITEMAZIONE ps = new PRIMASITEMAZIONE();
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

                var aa = db.ATTIVITAANTICIPI.Find(idAttivitaAnticipi);

                ps = aa.PRIMASITEMAZIONE;

                if (ps?.IDPRIMASISTEMAZIONE > 0)
                {
                    TRASFERIMENTO tr = ps.TRASFERIMENTO;
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
                                msgMail.oggetto = Resources.msgEmail.OggettoNotificaRichiestaAnticipi;
                                msgMail.corpoMsg =
                                        string.Format(
                                            Resources.msgEmail.MessaggioNotificaRichiestaAnticipi,
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



    }
}
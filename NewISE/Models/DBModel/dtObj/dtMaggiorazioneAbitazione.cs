using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using NewISE.EF;
using Newtonsoft.Json.Schema;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;
using System.ComponentModel.DataAnnotations;

using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models.ModelRest;
using System.Diagnostics;
using System.IO;
using NewISE.Models.Config;
using NewISE.Models.Config.s_admin;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtMaggiorazioneAbitazione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var ma = context.ObjectInstance as MaggiorazioneAbitazioneModel;
            if (ma != null)
            {
                if (ma.dataInizioMAB < ma.dataPartenza)
                {
                    vr = new ValidationResult(string.Format("Impossibile inserire la data di inizio validità minore alla data di partenza del trasferimento ({0}).", ma.dataPartenza.ToShortDateString()));
                }
                else
                {
                    vr = ValidationResult.Success;
                }
            }
            else
            {
                vr = new ValidationResult("La data di inizio validità è richiesta.");
            }
            return vr;
        }



        public AttivazioneMABModel GetUltimaAttivazioneMAB(decimal idTrasferimento)
        {
            try
            {
                AttivazioneMABModel amm = new AttivazioneMABModel();

                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);

                    var aml = t.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDATTIVAZIONEMAB).ToList();

                    if (aml?.Any() ?? false)
                    {
                        am = aml.First();
                    }
                    else
                    {
                        am = this.CreaAttivazioneMAB(idTrasferimento, db);
                    }

                    amm = new AttivazioneMABModel()
                    {
                        idAttivazioneMAB = am.IDATTIVAZIONEMAB,
                        idTrasferimento = am.IDTRASFERIMENTO,
                        notificaRichiesta = am.NOTIFICARICHIESTA,
                        dataNotificaRichiesta = am.DATANOTIFICARICHIESTA,
                        Attivazione = am.ATTIVAZIONE,
                        dataAttivazione = am.DATAATTIVAZIONE,
                        dataVariazione = am.DATAVARIAZIONE,
                        dataAggiornamento = am.DATAAGGIORNAMENTO,
                        Annullato = am.ANNULLATO
                    };
                }

                return amm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        public MaggiorazioniAnnualiModel GetMaggiorazioneAnnuale(MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db)
        {
            try
            {

                List<MAGGIORAZIONIANNUALI> mal = new List<MAGGIORAZIONIANNUALI>();

                MAGGIORAZIONIANNUALI ma = new MAGGIORAZIONIANNUALI();
                MaggiorazioniAnnualiModel mam = new MaggiorazioniAnnualiModel();
                UfficiModel um = new UfficiModel();

                var t = db.TRASFERIMENTO.Find(mvm.idTrasferimento);
                var u = t.UFFICI;
                um.descUfficio = u.DESCRIZIONEUFFICIO;

                mal = u.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false &&
                                                        a.DATAINIZIOVALIDITA <= mvm.dataInizioMAB &&
                                                        a.DATAFINEVALIDITA >= mvm.dataFineMAB)
                                                        .OrderBy(a => a.IDMAGANNUALI).ToList();

                if (mal?.Any() ?? false)
                {
                    ma = mal.First();

                    mam = new MaggiorazioniAnnualiModel()
                    {
                        annualita = ma.ANNUALITA,
                        idUfficio = ma.IDUFFICIO,
                        idMagAnnuali = ma.IDMAGANNUALI,
                        dataInizioValidita = ma.DATAINIZIOVALIDITA,
                        dataFineValidita = ma.DATAFINEVALIDITA,
                        annullato = ma.ANNULLATO,
                        dataAggiornamento = ma.DATAAGGIORNAMENTO,
                        DescrizioneUfficio = um
                    };
                }

                return mam;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PERCENTUALEMAB> GetListaPercentualeMAB(decimal idTrasferimento, ModelDBISE db)
        {
            try
            {


                PERCENTUALEMAB p = new PERCENTUALEMAB();
                List<PERCENTUALEMAB> pl = new List<PERCENTUALEMAB>();
                UfficiModel um = new UfficiModel();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var ma = t.MAGGIORAZIONEABITAZIONE.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDMAB).ToList().First();

                UFFICI u = t.UFFICI;
                DIPENDENTI d = t.DIPENDENTI;
                var l = d.LIVELLIDIPENDENTI.Where(a => a.ANNULLATO == false).ToList().First();

                um.descUfficio = u.DESCRIZIONEUFFICIO;

                pl = db.PERCENTUALEMAB.Where(a => a.ANNULLATO == false &&
                                                    a.DATAINIZIOVALIDITA >= ma.DATAINIZIOMAB &&
                                                    a.DATAFINEVALIDITA <= ma.DATAFINEMAB &&
                                                    a.IDUFFICIO == u.IDUFFICIO &&
                                                    a.IDLIVELLO == l.IDLIVELLO).ToList();
                return pl;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AttivazioneMABModel GetAttivazioneMAB(decimal idTrasferimento)
        {
            try
            {
                AttivazioneMABModel amm = new AttivazioneMABModel();
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);

                    var aml = t.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDATTIVAZIONEMAB).ToList();

                    if (aml?.Any() ?? false)
                    {
                        am = aml.First();

                        amm = new AttivazioneMABModel()
                        {
                            idAttivazioneMAB = am.IDATTIVAZIONEMAB,
                            idTrasferimento = am.IDTRASFERIMENTO,
                            notificaRichiesta = am.NOTIFICARICHIESTA,
                            dataNotificaRichiesta = am.DATANOTIFICARICHIESTA,
                            Attivazione = am.ATTIVAZIONE,
                            dataAttivazione = am.DATAATTIVAZIONE,
                            dataVariazione = am.DATAVARIAZIONE,
                            dataAggiornamento = am.DATAAGGIORNAMENTO,
                            Annullato = am.ANNULLATO
                        };
                    }

                }

                return amm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal VerificaEsistenzaDocumentoMAB(decimal idTrasferimento, EnumTipoDoc TipoDocumento)
        {
            try
            {
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                decimal idDoc = 0;


                using (ModelDBISE db = new ModelDBISE())
                {
                    bool esiste = false;

                    var t = db.TRASFERIMENTO.Find(idTrasferimento);

                    var aml = t.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == false).OrderBy(a => a.IDATTIVAZIONEMAB).ToList();

                    if (aml?.Any() ?? false)
                    {
                        am = aml.First();

                        var dl = am.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == (decimal)TipoDocumento).ToList();
                        if (dl?.Any() ?? false)
                        {
                            if (dl.Count() == 1)
                            {
                                var d = dl.First();

                                esiste = true;

                                idDoc = d.IDDOCUMENTO;
                            }
                            else
                            {
                                throw new Exception(string.Format("Errore in fase di aggiornamento documentazione Maggiorazione Abitazione. Esiste più di un documento del tipo selezionato. Contattare l'amministratore di sistema."));
                            }

                        }

                    }

                    return idDoc;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public void VerificaDocumentiPartenza(AttivazioneMABModel amm, out bool siDocCopiaContratto,
                                                                        out bool siDocCopiaRicevuta,
                                                                        out bool siDocModulo1,
                                                                        out bool siDocModulo2,
                                                                        out bool siDocModulo3,
                                                                        out bool siDocModulo4,
                                                                        out bool siDocModulo5,
                                                                        out decimal idDocCopiaContratto,
                                                                        out decimal idDocCopiaRicevuta,
                                                                        out decimal idDocModulo1,
                                                                        out decimal idDocModulo2,
                                                                        out decimal idDocModulo3,
                                                                        out decimal idDocModulo4,
                                                                        out decimal idDocModulo5)
        {
            siDocCopiaContratto = false;
            siDocCopiaRicevuta = false;
            siDocModulo1 = false;
            siDocModulo2 = false;
            siDocModulo3 = false;
            siDocModulo4 = false;
            siDocModulo5 = false;
            idDocCopiaContratto = 0;
            idDocCopiaRicevuta = 0;
            idDocModulo1 = 0;
            idDocModulo2 = 0;
            idDocModulo3 = 0;
            idDocModulo4 = 0;
            idDocModulo5 = 0;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var am = db.ATTIVAZIONEMAB.Find(amm.idAttivazioneMAB);

                    var docl = am.DOCUMENTI.Where(a => a.MODIFICATO == false).ToList();

                    if (docl?.Any() ?? false)
                    {
                        foreach (var doc in docl)
                        {
                            switch ((EnumTipoDoc)doc.IDTIPODOCUMENTO)
                            {
                                case EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione:
                                    siDocModulo1 = true;
                                    idDocModulo1 = doc.IDDOCUMENTO;
                                    break;
                                case EnumTipoDoc.MAB_Modulo2_Dichiarazione_Costo_Locazione:
                                    siDocModulo2 = true;
                                    idDocModulo2 = doc.IDDOCUMENTO;
                                    break;
                                case EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore:
                                    siDocModulo3 = true;
                                    idDocModulo3 = doc.IDDOCUMENTO;
                                    break;
                                case EnumTipoDoc.MAB_Modulo4_Dichiarazione_Costo_Locazione:
                                    siDocModulo4 = true;
                                    idDocModulo4 = doc.IDDOCUMENTO;
                                    break;
                                case EnumTipoDoc.Clausole_Contratto_Alloggio:
                                    siDocModulo5 = true;
                                    idDocModulo5 = doc.IDDOCUMENTO;
                                    break;
                                case EnumTipoDoc.Copia_Contratto_Locazione:
                                    siDocCopiaContratto = true;
                                    idDocCopiaContratto = doc.IDDOCUMENTO;
                                    break;
                                case EnumTipoDoc.Copia_Ricevuta_Pagamento_Locazione:
                                    siDocCopiaRicevuta = true;
                                    idDocCopiaRicevuta = doc.IDDOCUMENTO;
                                    break;
                                default:
                                    break;
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



        public CANONEMAB GetCanoneMAB(MaggiorazioneAbitazioneModel mam)
        {
            try
            {
                CANONEMAB cm = new CANONEMAB();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var cml = db.MAGGIORAZIONEABITAZIONE.Find(mam.idMAB)
                                .CANONEMAB.Where(X => X.ANNULLATO == false &&
                                                 X.DATAINIZIOVALIDITA >= mam.dataInizioMAB &&
                                                 X.DATAFINEVALIDITA <= mam.dataFineMAB)
                                .ToList();

                    if (cml?.Any() ?? false)
                    {
                        var cm_row = cml.First();

                        cm = new CANONEMAB()
                        {
                            IDCANONE = cm_row.IDCANONE,
                            IDATTIVAZIONEMAB = cm_row.IDATTIVAZIONEMAB,
                            IDMAB = cm_row.IDMAB,
                            DATAINIZIOVALIDITA = cm_row.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = cm_row.DATAFINEVALIDITA,
                            IMPORTOCANONE = cm_row.IMPORTOCANONE,
                            DATAAGGIORNAMENTO = cm_row.DATAAGGIORNAMENTO,
                            ANNULLATO = cm_row.ANNULLATO
                        };

                    }

                }

                return cm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ATTIVAZIONEMAB CreaAttivazioneMAB(decimal idTrasferimento, ModelDBISE db)
        {
            ATTIVAZIONEMAB new_am = new ATTIVAZIONEMAB()
            {
                IDTRASFERIMENTO = idTrasferimento,
                NOTIFICARICHIESTA = false,
                DATAATTIVAZIONE = null,
                ATTIVAZIONE = false,
                DATANOTIFICARICHIESTA = null,
                ANNULLATO = false,
                DATAVARIAZIONE = DateTime.Now,
                DATAAGGIORNAMENTO = DateTime.Now,
            };
            db.ATTIVAZIONEMAB.Add(new_am);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception(string.Format("Non è stato possibile creare una nuova attivazione per la maggiorazione abitazione."));
            }
            else
            {
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova attivazione maggiorazione abitazione.", "ATTIVITAZIONEMAB", db, new_am.IDTRASFERIMENTO, new_am.IDATTIVAZIONEMAB);
            }

            return new_am;
        }
        public decimal GetNumAttivazioniMAB(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var NumAttivazioni = 0;
                NumAttivazioni = db.TRASFERIMENTO.Find(idTrasferimento).ATTIVAZIONEMAB
                                    .Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == true && a.ATTIVAZIONE == true)
                                    .Count();
                return NumAttivazioni;
            }
        }
        public decimal GetNumNotificheMAB(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var NumNotifiche = 0;
                NumNotifiche = db.TRASFERIMENTO.Find(idTrasferimento).ATTIVAZIONEMAB
                                    .Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == true && a.ATTIVAZIONE == false)
                                    .Count();
                return NumNotifiche;
            }
        }

        public MaggiorazioneAbitazioneModel GetMaggiorazioneAbitazione(AttivazioneMABModel amm)
        {
            try
            {
                MaggiorazioneAbitazioneModel mam = new MaggiorazioneAbitazioneModel();
                //MAGGIORAZIONEABITAZIONE ma = new MAGGIORAZIONEABITAZIONE();

                using (ModelDBISE db = new ModelDBISE())
                {
                    //var t =  db.TRASFERIMENTO.Find(idTrasferimento);

                    var mal = db.ATTIVAZIONEMAB.Find(amm.idAttivazioneMAB).MAGGIORAZIONEABITAZIONE.Where(x => x.ANNULLATO == false).OrderBy(x => x.IDMAB).ToList();
                    //var mal = t.MAGGIORAZIONEABITAZIONE.Where(x => x.ANNULLATO == false).OrderByDescending(x => x.IDMAB).ToList();

                    if (mal?.Any() ?? false)
                    {
                        MAGGIORAZIONEABITAZIONE ma = mal.First();

                        mam = new MaggiorazioneAbitazioneModel()
                        {
                            idMAB = ma.IDMAB,
                            idTrasferimento = ma.IDTRASFERIMENTO,
                            idAttivazioneMAB = ma.IDATTIVAZIONEMAB,
                            dataInizioMAB = ma.DATAINIZIOMAB,
                            dataFineMAB = ma.DATAFINEMAB,
                            AnticipoAnnuale = ma.ANTICIPOANNUALE,
                            dataAggiornamento = ma.DATAAGGIORNAMENTO,
                            Annullato = ma.ANNULLATO
                        };

                    }
                }

                return mam;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MAGGIORAZIONEABITAZIONE GetMaggiorazioneAbitazioneByID(decimal idMAB)
        {
            try
            {
                MAGGIORAZIONEABITAZIONE ma = new MAGGIORAZIONEABITAZIONE();

                using (ModelDBISE db = new ModelDBISE())
                {
                    ma = db.MAGGIORAZIONEABITAZIONE.Find(idMAB);
                }

                return ma;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PAGATOCONDIVISOMAB> GetListPagatoCondivisoMAB(MaggiorazioneAbitazioneViewModel mvm)
        {
            try
            {
                //PAGATOCONDIVISOMAB pc = new PAGATOCONDIVISOMAB();
                List<PAGATOCONDIVISOMAB> lpc = new List<PAGATOCONDIVISOMAB>();

                using (ModelDBISE db = new ModelDBISE())
                {
                    lpc = db.PAGATOCONDIVISOMAB.Where(x => x.ANNULLATO == false &&
                                                        mvm.idMAB == x.IDMAB &&
                                                        mvm.idAttivazioneMAB == x.IDATTIVAZIONEMAB
                                                    ).ToList();
                }

                return lpc;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void SostituisciDocumentoMAB(ref DocumentiModel dm, decimal idDocumentoOld, decimal idAttivazioneMAB, ModelDBISE db)
        {
            //inserisce un nuovo documento e imposta il documento sostituito 
            //con MODIFICATO=true e valorizza FK_IDDOCUMENTO

            DOCUMENTI d_new = new DOCUMENTI();
            DOCUMENTI d_old = new DOCUMENTI();
            MemoryStream ms = new MemoryStream();
            dm.file.InputStream.CopyTo(ms);
            var am = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

            d_new.NOMEDOCUMENTO = dm.nomeDocumento;
            d_new.ESTENSIONE = dm.estensione;
            d_new.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
            d_new.DATAINSERIMENTO = dm.dataInserimento;
            d_new.FILEDOCUMENTO = ms.ToArray();
            d_new.MODIFICATO = false;


            db.DOCUMENTI.Add(d_new);

            if (db.SaveChanges() > 0)
            {
                dm.idDocumenti = d_new.IDDOCUMENTO;
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo documento (maggiorazione abitazione).", "Documenti", db, am.IDTRASFERIMENTO, dm.idDocumenti);

                //aggiorno il documento esistente
                d_old = db.DOCUMENTI.Find(idDocumentoOld);
                if (d_old.IDDOCUMENTO > 0)
                {
                    d_old.MODIFICATO = true;
                    d_old.FK_IDDOCUMENTO = d_new.IDDOCUMENTO;

                    if (db.SaveChanges() > 0)
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modificato documento con FK_idDocumento (maggiorazione abitazione).", "Documenti", db, am.IDTRASFERIMENTO, d_old.IDDOCUMENTO);

                    }

                }
            }
        }

        public void SetDocumentoMAB(ref DocumentiModel dm, decimal idAttivazioneMAB, ModelDBISE db)
        {
            MemoryStream ms = new MemoryStream();
            DOCUMENTI d = new DOCUMENTI();

            dm.file.InputStream.CopyTo(ms);
            var am = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

            d.NOMEDOCUMENTO = dm.nomeDocumento;
            d.ESTENSIONE = dm.estensione;
            d.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
            d.DATAINSERIMENTO = dm.dataInserimento;
            d.FILEDOCUMENTO = ms.ToArray();
            d.MODIFICATO = false;


            db.DOCUMENTI.Add(d);

            if (db.SaveChanges() > 0)
            {
                dm.idDocumenti = d.IDDOCUMENTO;
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo documento (maggiorazione abitazione).", "Documenti", db, am.IDTRASFERIMENTO, dm.idDocumenti);
            }
        }

        public void AssociaDocumentoAttivazione(decimal idAttivazioneMAB, decimal idDocumento, ModelDBISE db)
        {
            try
            {
                var att = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);
                var item = db.Entry<ATTIVAZIONEMAB>(att);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.DOCUMENTI).Load();
                var d = db.DOCUMENTI.Find(idDocumento);
                att.DOCUMENTI.Add(d);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il documento all'attivazione abitazione"));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AttivaRichiestaMAB(decimal idAttivazioneMAB)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var am = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);
                    if (am?.IDATTIVAZIONEMAB > 0)
                    {
                        if (am.NOTIFICARICHIESTA == true)
                        {
                            am.ATTIVAZIONE = true;
                            am.DATAATTIVAZIONE = DateTime.Now;
                            am.DATAAGGIORNAMENTO = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore: Impossibile completare l'attivazione maggiorazione abitazione.");
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Attivazione maggiorazione abitazione.", "ATTIVAZIONEMAB", db,
                                    am.TRASFERIMENTO.IDTRASFERIMENTO, am.IDATTIVAZIONEMAB);
                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.ModificaInCompletatoCalendarioEvento(am.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioneAbitazione, db);
                                }

                                this.EmailAttivaRichiestaMAB(am.IDATTIVAZIONEMAB, db);

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

        private void EmailAttivaRichiestaMAB(decimal idAttivazioneMAB, ModelDBISE db)
        {
            //PRIMASITEMAZIONE ps = new PRIMASITEMAZIONE();
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

                var amab = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                if (amab?.IDTRASFERIMENTO > 0)
                {
                    TRASFERIMENTO tr = amab.TRASFERIMENTO;
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
                                msgMail.oggetto = Resources.msgEmail.OggettoAttivazioneMaggiorazioneAbitazione;

                                msgMail.corpoMsg =
                                        string.Format(
                                            Resources.msgEmail.MessaggioAttivazioneMaggiorazioneAbitazione,
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

        public void AnnullaRichiestaMAB(decimal idAttivazioneMAB)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var am_Old = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                    if (am_Old?.IDATTIVAZIONEMAB > 0)
                    {
                        if (am_Old.NOTIFICARICHIESTA == true && am_Old.ATTIVAZIONE == false && am_Old.ANNULLATO == false)
                        {
                            am_Old.ANNULLATO = true;
                            am_Old.DATAAGGIORNAMENTO = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore - Impossibile annullare la notifica della richiesta maggiorazione abitazione.");
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Annullamento della riga per il ciclo di attivazione della richiesta di maggiorazione abitazione",
                                    "ATTIVAZIONEMAB", db, am_Old.TRASFERIMENTO.IDTRASFERIMENTO,
                                    am_Old.IDATTIVAZIONEMAB);

                                ATTIVAZIONEMAB am_New = new ATTIVAZIONEMAB()
                                {
                                    IDTRASFERIMENTO = am_Old.IDTRASFERIMENTO,
                                    NOTIFICARICHIESTA = false,
                                    ATTIVAZIONE = false,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    ANNULLATO = false
                                };

                                db.ATTIVAZIONEMAB.Add(am_New);

                                int j = db.SaveChanges();

                                if (j <= 0)
                                {
                                    throw new Exception("Errore - Impossibile creare il nuovo ciclo di attivazione per richiesta maggiorazione abitazione.");
                                }
                                else
                                {
                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                        "Inserimento di una nuova riga per il ciclo di attivazione relativo alla richiesta maggiorazione abitazione.",
                                        "ATTIVAZIONEMAB", db, am_New.TRASFERIMENTO.IDTRASFERIMENTO,
                                        am_New.IDATTIVAZIONEMAB);

                                    var mab_Old_l =
                                        am_Old.MAGGIORAZIONEABITAZIONE.Where(
                                            a => a.ANNULLATO == false).ToList();
                                    if (mab_Old_l?.Any() ?? false)
                                    {
                                        #region maggiorazione abitazione
                                        var mab_Old =
                                            am_Old.MAGGIORAZIONEABITAZIONE.Where(
                                                a => a.ANNULLATO == false).First();

                                        if (mab_Old != null && mab_Old.IDATTIVAZIONEMAB > 0)
                                        {
                                            MAGGIORAZIONEABITAZIONE mab_New = new MAGGIORAZIONEABITAZIONE()
                                            {
                                                IDATTIVAZIONEMAB = am_New.IDATTIVAZIONEMAB,
                                                DATAINIZIOMAB = mab_Old.DATAINIZIOMAB,
                                                DATAFINEMAB = mab_Old.DATAFINEMAB,
                                                ANTICIPOANNUALE = mab_Old.ANTICIPOANNUALE,
                                                DATAAGGIORNAMENTO = mab_Old.DATAAGGIORNAMENTO,
                                                ANNULLATO = mab_Old.ANNULLATO,
                                                IDTRASFERIMENTO = mab_Old.IDTRASFERIMENTO
                                            };

                                            db.MAGGIORAZIONEABITAZIONE.Add(mab_New);
                                            mab_Old.ANNULLATO = true;

                                            int y = db.SaveChanges();

                                            if (y <= 0)
                                            {
                                                throw new Exception("Errore - Impossibile inserire il record relativo a richiesta maggiorazione abitazione.");
                                            }
                                            else
                                            {
                                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                    "Inserimento di una nuova riga per la richiesta maggiorazione abitazione.",
                                                    "MAGGIORAZIONEABITAZIONE", db,
                                                    am_New.TRASFERIMENTO.IDTRASFERIMENTO,
                                                    mab_New.IDMAB);
                                            }

                                            #endregion

                                            var old_canone_l = mab_Old.CANONEMAB.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDCANONE).ToList();
                                            if (old_canone_l?.Any() ?? false)
                                            {
                                                #region canone
                                                var old_canone = old_canone_l.First();
                                                CANONEMAB canone_new = new CANONEMAB()
                                                {
                                                    IDATTIVAZIONEMAB = am_New.IDATTIVAZIONEMAB,
                                                    IDMAB = mab_New.IDMAB,
                                                    DATAINIZIOVALIDITA = old_canone.DATAINIZIOVALIDITA,
                                                    DATAFINEVALIDITA = old_canone.DATAFINEVALIDITA,
                                                    IMPORTOCANONE = old_canone.IMPORTOCANONE,
                                                    ANNULLATO = old_canone.ANNULLATO,
                                                    DATAAGGIORNAMENTO = old_canone.DATAAGGIORNAMENTO
                                                };

                                                db.CANONEMAB.Add(canone_new);
                                                old_canone.ANNULLATO = true;

                                                if (db.SaveChanges() <= 0)
                                                {
                                                    throw new Exception("Errore - Impossibile inserire il record relativo al canone nel ciclo di annullamento della richiesta maggiorazione abitazione.");
                                                }
                                                else
                                                {
                                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                        "Inserimento di una nuova riga canone per la richiesta maggiorazione abitazione.",
                                                        "CANONEMAB", db,
                                                        am_New.TRASFERIMENTO.IDTRASFERIMENTO,
                                                        canone_new.IDCANONE);

                                                    #region associa MAB a Magg Annuali
                                                    var m_annuali_l = mab_Old.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false).ToList();
                                                    if (m_annuali_l?.Any() ?? false)
                                                    {
                                                        foreach (var m_annuali in m_annuali_l)
                                                        {
                                                            this.Associa_MAB_MaggiorazioniAnnuali(mab_New.IDMAB, m_annuali.IDMAGANNUALI, db);
                                                        }
                                                    }
                                                    #endregion

                                                    #region associa MAB a percentuale MAB
                                                    var percMAB_l = mab_Old.PERCENTUALEMAB.Where(a => a.ANNULLATO == false).ToList();
                                                    if (percMAB_l?.Any() ?? false)
                                                    {
                                                        foreach (var percMAB in percMAB_l)
                                                        {
                                                            this.Associa_MAB_PercenualeMAB(mab_New.IDMAB, percMAB.IDPERCMAB, db);
                                                        }
                                                    }
                                                    #endregion

                                                    #region canone MAB a TFR
                                                    var TFR_l = old_canone.TFR.Where(a => a.ANNULLATO == false).ToList();
                                                    if (TFR_l?.Any() ?? false)
                                                    {
                                                        foreach (var TFR in TFR_l)
                                                        {
                                                            this.Associa_TFR_CanoneMAB(TFR.IDTFR, canone_new.IDCANONE, db);
                                                        }
                                                    }
                                                    #endregion

                                                }
                                                #endregion

                                                #region documenti
                                                var ld=this.GetDocumentiMAB(idAttivazioneMAB, db);
                                                if (ld?.Any() ?? false)
                                                {
                                                    foreach (var d in ld)
                                                    {
                                                        this.Associa_Documenti_Attivazione(d.IDDOCUMENTO, am_New.IDATTIVAZIONEMAB, db);
                                                    }
                                                }
                                                #endregion
                                            }
                                        }

                                        this.EmailAnnullaRichiestaMAB(am_New.IDATTIVAZIONEMAB, db);
                                        using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                        {
                                            dtce.AnnullaMessaggioEvento(am_New.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioneAbitazione, db);
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

        public void EmailAnnullaRichiestaMAB(decimal idAttivazioneMAB, ModelDBISE db)
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

                var amab = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                if (amab?.IDATTIVAZIONEMAB > 0)
                {
                    TRASFERIMENTO tr = amab.TRASFERIMENTO;
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
                            Resources.msgEmail.OggettoAnnullaRichiestaMaggiorazioneAbitazione;
                            msgMail.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaMaggiorazioneAbitazione, uff.DESCRIZIONEUFFICIO + " (" + uff.CODICEUFFICIO + ")", tr.DATAPARTENZA.ToLongDateString());

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

        public void NotificaRichiestaMAB(decimal idAttivazioneMAB)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var am = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                        if (am?.IDATTIVAZIONEMAB > 0)
                        {
                            am.NOTIFICARICHIESTA = true;
                            am.DATANOTIFICARICHIESTA = DateTime.Now;
                            am.DATAAGGIORNAMENTO = DateTime.Now;

                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore nella fase d'inserimento per la richiesta attivazione maggiorazione abitazione.");
                            }
                            else
                            {
                                this.EmailNotificaRichiestaMAB(idAttivazioneMAB, db);

                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    CalendarioEventiModel cem = new CalendarioEventiModel()
                                    {
                                        idFunzioneEventi = EnumFunzioniEventi.RichiestaMaggiorazioneAbitazione,
                                        idTrasferimento = am.TRASFERIMENTO.IDTRASFERIMENTO,
                                        DataInizioEvento = DateTime.Now.Date,
                                        DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaMaggiorazioneAbitazione)).Date,
                                    };

                                    dtce.InsertCalendarioEvento(ref cem, db);
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

        private void EmailNotificaRichiestaMAB(decimal idAttivazioneMAB, ModelDBISE db)
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

                var amab = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);


                if (amab?.IDTRASFERIMENTO > 0)
                {
                    TRASFERIMENTO tr = amab.TRASFERIMENTO;
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
                                msgMail.oggetto = Resources.msgEmail.OggettoNotificaRichiestaMaggiorazioneAbitazione;
                                msgMail.corpoMsg =
                                        string.Format(
                                            Resources.msgEmail.MessaggioNotificaMaggiorazioneAbitazione,
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

        public decimal SetMaggiorazioneAbitazione(ref MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db, decimal idAttivazioneMAB)
        {
            try
            {
                var idAtt = idAttivazioneMAB;

                DateTime dtFine;

                if (mvm.ut_dataFineMAB == null)
                {
                    dtFine = Utility.DataFineStop();
                }
                else
                {
                    dtFine = mvm.ut_dataFineMAB.Value;
                }

                MAGGIORAZIONEABITAZIONE ma = new MAGGIORAZIONEABITAZIONE()
                {
                    IDTRASFERIMENTO = mvm.idTrasferimento,
                    IDATTIVAZIONEMAB = idAttivazioneMAB,
                    DATAINIZIOMAB = mvm.dataInizioMAB,
                    DATAFINEMAB = dtFine,
                    DATAAGGIORNAMENTO = DateTime.Now,
                    ANNULLATO = false,
                    ANTICIPOANNUALE = mvm.AnticipoAnnuale
                };

                db.MAGGIORAZIONEABITAZIONE.Add(ma);

                if (db.SaveChanges() <= 0)
                {
                    throw new Exception("Non è stato possibile inserire la maggiorazione abitazione.");
                }
                else
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento della maggiorazione abitazione", "MAGGIORAZIONEABITAZIONE", db,
                        ma.IDTRASFERIMENTO, ma.IDMAB);

                    return ma.IDMAB;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateMaggiorazioneAbitazione(ref MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db, decimal idAttivazioneMAB)
        {
            try
            {
                var idAtt = idAttivazioneMAB;

                DateTime dtFine;

                if (mvm.ut_dataFineMAB == null)
                {
                    dtFine = Utility.DataFineStop();
                }
                else
                {
                    dtFine = mvm.ut_dataFineMAB.Value;
                }

                var ma = db.MAGGIORAZIONEABITAZIONE.Find(mvm.idMAB);
                if (ma.IDMAB > 0)
                {
                    ma.DATAINIZIOMAB = mvm.dataInizioMAB;
                    ma.DATAFINEMAB = dtFine;
                    ma.DATAAGGIORNAMENTO = DateTime.Now;
                    ma.ANTICIPOANNUALE = mvm.AnticipoAnnuale;
                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Non è stato possibile aggiornare la maggiorazione abitazione.");
                    }
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica della maggiorazione abitazione", "MAGGIORAZIONEABITAZIONE", db,
                            ma.IDTRASFERIMENTO, ma.IDMAB);
                    }

                }
                else
                {
                    throw new Exception("Impossibile aggiornare la maggiorazione abitazione. Record non trovato.");
                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }


        public CANONEMAB SetCanoneMAB(MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db)
        {
            try
            {
                CANONEMAB cm = new CANONEMAB()
                {
                    IDATTIVAZIONEMAB = mvm.idAttivazioneMAB,
                    IDMAB = mvm.idMAB,
                    DATAINIZIOVALIDITA = mvm.dataInizioMAB,
                    DATAFINEVALIDITA = mvm.dataFineMAB,
                    IMPORTOCANONE = mvm.importo_canone,
                    DATAAGGIORNAMENTO = DateTime.Now,
                    ANNULLATO = false
                };

                db.CANONEMAB.Add(cm);


                if (db.SaveChanges() <= 0)
                {
                    throw new Exception("Non è stato possibile inserire il canone maggiorazione abitazione.");
                }
                else
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento canone maggiorazione abitazione", "CANONEMAB", db,
                        mvm.idTrasferimento, cm.IDCANONE);

                    return cm;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CANONEMAB UpdateCanoneMAB(MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db)
        {
            try
            {
                var lc = db.CANONEMAB.Where(a => a.ANNULLATO == false &&
                                              a.IDATTIVAZIONEMAB == mvm.idAttivazioneMAB &&
                                              a.IDMAB == mvm.idMAB).ToList();
                if (lc?.Any() ?? false)
                {
                    var c = lc.First();
                    if (c.IDCANONE > 0)
                    {

                        c.DATAINIZIOVALIDITA = mvm.dataInizioMAB;
                        c.DATAFINEVALIDITA = mvm.dataFineMAB;
                        c.IMPORTOCANONE = mvm.importo_canone;
                        c.DATAAGGIORNAMENTO = DateTime.Now;
                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Non è stato possibile aggiornare il canone maggiorazione abitazione.");
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Aggiornamento canone maggiorazione abitazione", "CANONEMAB", db,
                                mvm.idTrasferimento, c.IDCANONE);
                            return c;
                        }
                    }
                    else
                    {
                        throw new Exception("Non è stato possibile aggiornare il canone maggiorazione abitazione. Record non trovato.");
                    }

                }
                else
                {
                    throw new Exception("Non è stato possibile aggiornare il canone maggiorazione abitazione. Nessun record non trovato.");
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }


        public PAGATOCONDIVISOMAB SetPagatoCondivisoMAB(MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db)
        {
            try
            {
                PAGATOCONDIVISOMAB pc = new PAGATOCONDIVISOMAB()
                {
                    IDMAB = mvm.idMAB,
                    IDATTIVAZIONEMAB = mvm.idAttivazioneMAB,
                    DATAINIZIOVALIDITA = mvm.dataInizioMAB,
                    DATAFINEVALIDITA = mvm.dataFineMAB,
                    CONDIVISO = mvm.canone_condiviso,
                    PAGATO = mvm.canone_pagato,
                    DATAAGGIORNAMENTO = DateTime.Now,
                    ANNULLATO = false
                };

                db.PAGATOCONDIVISOMAB.Add(pc);


                if (db.SaveChanges() <= 0)
                {
                    throw new Exception("Non è stato possibile inserire il record relativo a PagatoCondivisoMAB.");
                }
                else
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento PagatoCondivisoMAB", "PAGATOCONDIVISOMAB", db,
                        mvm.idTrasferimento, pc.IDPAGATOCONDIVISO);

                    return pc;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdatePagatoCondivisoMAB(MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db)
        {
            try
            {
                var ma = db.MAGGIORAZIONEABITAZIONE.Find(mvm.idMAB);
                var lpc = ma.PAGATOCONDIVISOMAB.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDPAGATOCONDIVISO).ToList();
                if (lpc?.Any() ?? false)
                {
                    var pc = lpc.First();

                    pc.DATAINIZIOVALIDITA = mvm.dataInizioMAB;
                    pc.DATAFINEVALIDITA = mvm.dataFineMAB;
                    pc.CONDIVISO = mvm.canone_condiviso;
                    pc.PAGATO = mvm.canone_pagato;
                    pc.DATAAGGIORNAMENTO = DateTime.Now;

                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore in fase di aggioramento del record relativo a PagatoCondivisoMAB.");
                    }
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica PagatoCondivisoMAB", "PAGATOCONDIVISOMAB", db,
                            mvm.idTrasferimento, pc.IDPAGATOCONDIVISO);
                    }

                }
                else
                {
                    // se non esiste lo creo
                    var pc = SetPagatoCondivisoMAB(mvm, db);
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }



        public void InserisciMAB(MaggiorazioneAbitazioneViewModel mvm, decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    #region ATTIVAZIONE MAB
                    var amm = this.GetAttivazioneMAB(idTrasferimento);
                    if (amm != null && amm.idAttivazioneMAB > 0)
                    {
                        mvm.idAttivazioneMAB = amm.idAttivazioneMAB;
                    }
                    else
                    {
                        var am = this.CreaAttivazioneMAB(idTrasferimento, db);
                        amm = new AttivazioneMABModel()
                        {
                            Annullato = am.ANNULLATO,
                            Attivazione = am.ATTIVAZIONE,
                            dataAggiornamento = am.DATAAGGIORNAMENTO,
                            dataAttivazione = am.DATAATTIVAZIONE,
                            dataNotificaRichiesta = am.DATANOTIFICARICHIESTA,
                            dataVariazione = am.DATAVARIAZIONE,
                            idAttivazioneMAB = am.IDATTIVAZIONEMAB,
                            idTrasferimento = am.IDTRASFERIMENTO,
                            notificaRichiesta = am.NOTIFICARICHIESTA
                        };
                    }
                    #endregion

                    mvm.dataAggiornamento = DateTime.Now;

                    #region nuova MAB
                    decimal new_idMAB = this.SetMaggiorazioneAbitazione(ref mvm, db, amm.idAttivazioneMAB);
                    #endregion

                    DateTime dtIni = mvm.dataInizioMAB;
                    DateTime dtFin = mvm.ut_dataFineMAB == null ? Utility.DataFineStop() : mvm.ut_dataFineMAB.Value;
                    mvm.dataFineMAB = dtFin;
                    mvm.idMAB = new_idMAB;



                    #region anticipo annuale
                    if (mvm.AnticipoAnnuale)
                    {

                        var mann = this.GetMaggiorazioneAnnuale(mvm, db);

                        if (mann.idMagAnnuali > 0)
                        {
                            mvm.AnticipoAnnuale = mann.annualita;
                            //associa MAB a MaggiorazioniAnnuali se esiste
                            this.Associa_MAB_MaggiorazioniAnnuali(new_idMAB, mann.idMagAnnuali, db);
                        }
                        else
                        {
                            mvm.AnticipoAnnuale = false;
                        }

                    }
                    #endregion


                    #region associa MAB a tutte le percentuali MAB trovate
                    var lista_perc = this.GetListaPercentualeMAB(idTrasferimento, db);
                    if (lista_perc?.Any() ?? false)
                    {
                        foreach (var perc in lista_perc)
                        {
                            this.Associa_MAB_PercenualeMAB(new_idMAB, perc.IDPERCMAB, db);
                        }
                    }
                    #endregion

                    #region inserisci CANONE
                    CANONEMAB c = this.SetCanoneMAB(mvm, db);
                    #endregion

                    #region associa canone MAB a TFR
                    using (dtTFR dtt = new dtTFR())
                    {
                        using (dtTrasferimento dttrasf = new dtTrasferimento())
                        {
                            var t = dttrasf.GetTrasferimentoById(idTrasferimento);
                            var ltfr = dtt.GetListaTfrByValuta_RangeDate(t.idUfficio, mvm.id_Valuta, dtIni, dtFin, db);

                            if (ltfr?.Any() ?? false)
                            {
                                foreach (var tfr in ltfr)
                                {
                                    this.Associa_TFR_CanoneMAB(tfr.idTFR, c.IDCANONE, db);
                                }
                            }
                        }
                    }
                    #endregion

                    #region inserisce eventuale pagato condiviso
                    if (mvm.canone_condiviso)
                    {
                        PAGATOCONDIVISOMAB pc = this.SetPagatoCondivisoMAB(mvm, db);
                    }

                    #endregion

                    db.Database.CurrentTransaction.Commit();

                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public void AggiornaMAB(MaggiorazioneAbitazioneViewModel mvm, decimal idTrasferimento, decimal idMAB)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    #region legge ATTIVAZIONE MAB
                    var amm = this.GetAttivazioneMAB(idTrasferimento);
                    if (amm != null && amm.idAttivazioneMAB > 0)
                    {
                        mvm.idAttivazioneMAB = amm.idAttivazioneMAB;
                    }
                    else
                    {
                        throw new Exception(string.Format("Impossibile aggiornare la maggiorazione abitazione."));
                    }
                    #endregion

                    mvm.dataAggiornamento = DateTime.Now;
                    mvm.idMAB = idMAB;
                    mvm.idTrasferimento = idTrasferimento;

                    #region aggiorna MAB
                    this.UpdateMaggiorazioneAbitazione(ref mvm, db, amm.idAttivazioneMAB);
                    #endregion

                    DateTime dtIni = mvm.dataInizioMAB;
                    DateTime dtFin = mvm.ut_dataFineMAB == null ? Utility.DataFineStop() : mvm.ut_dataFineMAB.Value;
                    mvm.dataFineMAB = dtFin;


                    #region aggiorno anticipo annuale
                    //rimuovi precedenti associazioni MAB MaggiorazioniAnnuali
                    this.RimuoviAssociazioneMAB_MaggiorazioniAnnuali(idMAB, db);
                    //se richiesto le riassocio
                    if (mvm.AnticipoAnnuale)
                    {
                        var mann = this.GetMaggiorazioneAnnuale(mvm, db);
                        if (mann.idMagAnnuali > 0)
                        {
                            //associa MAB a MaggiorazioniAnnuali se esiste
                            this.Associa_MAB_MaggiorazioniAnnuali(mvm.idMAB, mann.idMagAnnuali, db);
                        }
                    }
                    #endregion


                    #region associa MAB a tutte le percentuali MAB trovate
                    this.RimuoviAssociazioneMAB_PercentualeMAB(mvm.idMAB, db);
                    var lista_perc = this.GetListaPercentualeMAB(mvm.idTrasferimento, db);
                    if (lista_perc?.Any() ?? false)
                    {
                        foreach (var perc in lista_perc)
                        {
                            this.Associa_MAB_PercenualeMAB(mvm.idMAB, perc.IDPERCMAB, db);
                        }
                    }
                    #endregion

                    #region aggiorna CANONE
                    var canone = this.UpdateCanoneMAB(mvm, db);
                    #endregion

                    #region associa canone MAB a TFR
                    //rimuovi precedenti associazioni CANONE TFR
                    this.RimuoviAssociazioneCanoneMAB_TFR(canone.IDCANONE, db);
                    using (dtTFR dtt = new dtTFR())
                    {
                        using (dtTrasferimento dttrasf = new dtTrasferimento())
                        {
                            var t = dttrasf.GetTrasferimentoById(mvm.idTrasferimento);
                            var ltfr = dtt.GetListaTfrByValuta_RangeDate(t.idUfficio, mvm.id_Valuta, dtIni, dtFin, db);

                            if (ltfr?.Any() ?? false)
                            {
                                foreach (var tfr in ltfr)
                                {
                                    this.Associa_TFR_CanoneMAB(tfr.idTFR, canone.IDCANONE, db);
                                }
                            }
                        }
                    }
                    #endregion

                    #region aggiorna eventuale pagato condiviso
                    //if (mvm.canone_condiviso)
                    //{
                    this.UpdatePagatoCondivisoMAB(mvm, db);
                    //}
                    //else
                    //{
                    //    //se non lo ho richiesto annullo eventuali record precedenti
                    //    var lpc = this.GetListPagatoCondivisoMAB(mvm);

                    //    foreach(var pc in lpc)
                    //    {
                    //        pc.ANNULLATO = true;
                    //    }
                    //}

                    #endregion

                    db.Database.CurrentTransaction.Commit();

                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public void RimuoviAssociazioneMAB_MaggiorazioniAnnuali(decimal idMAB, ModelDBISE db)
        {
            var ma = db.MAGGIORAZIONEABITAZIONE.Find(idMAB);
            var lmann = ma.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false).ToList();
            if (lmann?.Any() ?? false)
            {
                foreach (var mann in lmann)
                {
                    ma.MAGGIORAZIONIANNUALI.Remove(mann);
                }

                db.SaveChanges();
            }
        }

        public void RimuoviAssociazioneMAB_PercentualeMAB(decimal idMAB, ModelDBISE db)
        {
            var ma = db.MAGGIORAZIONEABITAZIONE.Find(idMAB);
            var lpercMAB = ma.PERCENTUALEMAB.Where(a => a.ANNULLATO == false).ToList();
            if (lpercMAB?.Any() ?? false)
            {
                foreach (var percMAB in lpercMAB)
                {
                    ma.PERCENTUALEMAB.Remove(percMAB);
                }

                db.SaveChanges();
            }

        }

        public void RimuoviAssociazioneCanoneMAB_TFR(decimal idCanone, ModelDBISE db)
        {
            var c = db.CANONEMAB.Find(idCanone);
            var lTFR = c.TFR.Where(a => a.ANNULLATO == false).ToList();
            if (lTFR?.Any() ?? false)
            {
                foreach (var TFR in lTFR)
                {
                    c.TFR.Remove(TFR);
                }

                db.SaveChanges();
            }

        }




        public void Associa_MAB_MaggiorazioniAnnuali(decimal idMAB, decimal idMaggiorazioniAnnuali, ModelDBISE db)
        {
            try
            {
                var mab = db.MAGGIORAZIONEABITAZIONE.Find(idMAB);
                var item = db.Entry<MAGGIORAZIONEABITAZIONE>(mab);
                item.State = System.Data.Entity.EntityState.Modified;
                //item.Collection(a => a.DOCUMENTI).Load();
                var ma = db.MAGGIORAZIONIANNUALI.Find(idMaggiorazioniAnnuali);
                mab.MAGGIORAZIONIANNUALI.Add(ma);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare MaggiorazioneAbitazione a MaggiorazioniAnnuali."));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Associa_MAB_PercenualeMAB(decimal idMAB, decimal idPercMAB, ModelDBISE db)
        {
            try
            {
                var mab = db.MAGGIORAZIONEABITAZIONE.Find(idMAB);
                var item = db.Entry<MAGGIORAZIONEABITAZIONE>(mab);
                item.State = System.Data.Entity.EntityState.Modified;
                //item.Collection(a => a.DOCUMENTI).Load();
                var p = db.PERCENTUALEMAB.Find(idPercMAB);
                mab.PERCENTUALEMAB.Add(p);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare MaggiorazioneAbitazione a PercenualeMAB."));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Associa_TFR_CanoneMAB(decimal idTFR, decimal idCanoneMAB, ModelDBISE db)
        {
            try
            {
                var tfr = db.TFR.Find(idTFR);
                var item = db.Entry<TFR>(tfr);
                item.State = System.Data.Entity.EntityState.Modified;
                //item.Collection(a => a.DOCUMENTI).Load();
                var c = db.CANONEMAB.Find(idCanoneMAB);
                tfr.CANONEMAB.Add(c);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il canone MAB a TFR."));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<DOCUMENTI> GetDocumentiMAB(decimal idAttivazioneMAB, ModelDBISE db)
        {
            try
            {


                DOCUMENTI d = new DOCUMENTI();
                List<DOCUMENTI> dl = new List<DOCUMENTI>();

                var a = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                dl = a.DOCUMENTI.Where(x => x.MODIFICATO == false &&
                                        (x.IDTIPODOCUMENTO==(decimal)EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione ||
                                        x.IDTIPODOCUMENTO==(decimal)EnumTipoDoc.MAB_Modulo2_Dichiarazione_Costo_Locazione ||
                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore ||
                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo4_Dichiarazione_Costo_Locazione ||
                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Clausole_Contratto_Alloggio ||
                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Contratto_Locazione ||
                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Ricevuta_Pagamento_Locazione)
                                        ).ToList();
                return dl;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Associa_Documenti_Attivazione(decimal idDocumento, decimal idAttivazioneMAB, ModelDBISE db)
        {
            try
            {
                var a = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);
                var item = db.Entry<ATTIVAZIONEMAB>(a);
                item.State = System.Data.Entity.EntityState.Modified;
                //item.Collection(a => a.DOCUMENTI).Load();
                var d = db.DOCUMENTI.Find(idDocumento);
                a.DOCUMENTI.Add(d);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare i documenti all'attivazione MAB."));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



    }
}
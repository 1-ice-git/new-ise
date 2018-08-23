using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
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
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtViaggiCongedo : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        private void EmailAttivaRichiestaTV(decimal idAttivazioneTitoliViaggio, ModelDBISE db)
        {
            TITOLIVIAGGIO tv = new TITOLIVIAGGIO();
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

                var atv = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoliViaggio);

                var conta_attivazioni = this.GetNumAttivazioniTV(atv.IDTITOLOVIAGGIO, db);

                tv = atv.TITOLIVIAGGIO;

                if (tv?.IDTITOLOVIAGGIO > 0)
                {
                    TRASFERIMENTO tr = tv.TRASFERIMENTO;
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
                                if (conta_attivazioni == 1)
                                {
                                    msgMail.oggetto = Resources.msgEmail.OggettoAttivaRichiestaInizialeTitoloViaggio;
                                    msgMail.corpoMsg =
                                            string.Format(
                                                Resources.msgEmail.MessaggioAttivaRichiestaInizialeTitoliViaggio,
                                                d.COGNOME + " " + d.NOME + " (" + d.MATRICOLA + ")",
                                                tr.DATAPARTENZA.ToLongDateString(),
                                                u.DESCRIZIONEUFFICIO + " (" + u.CODICEUFFICIO + ")");

                                }
                                else
                                {
                                    msgMail.oggetto = Resources.msgEmail.OggettoAttivaRichiestaSuccessivaTitoloViaggio;
                                    msgMail.corpoMsg =
                                            string.Format(
                                                Resources.msgEmail.MessaggioAttivaRichiestaSuccessivaTitoliViaggio,
                                                d.COGNOME + " " + d.NOME + " (" + d.MATRICOLA + ")",
                                                tr.DATAPARTENZA.ToLongDateString(),
                                                u.DESCRIZIONEUFFICIO + " (" + u.CODICEUFFICIO + ")");
                                }
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
        public bool AttivaPulsanteNuovo(decimal idTrasferimento)
        {
            bool tmp = true;
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    //db.Database.BeginTransaction();
                    var VC = db.VIAGGICONGEDO.Where(a => a.IDTRASFERIMENTO == idTrasferimento).OrderByDescending(x => x.IDVIAGGIOCONGEDO).ToList();
                    if (VC?.Any() ?? false)
                    {
                        decimal idVC = VC.First().IDVIAGGIOCONGEDO;
                        var VCelem = db.VIAGGICONGEDO.Find(idVC);
                        var AVC = VCelem.ATTIVAZIONIVIAGGICONGEDO.Where(a => a.ANNULLATO == false).OrderByDescending(x => x.IDFASEVC).ThenBy(y => y.IDATTIVAZIONEVC).ToList();
                        if (AVC?.Any() ?? false)
                        {
                            if (AVC.First().ATTIVARICHIESTA == true)
                                tmp = true;
                            else
                                tmp = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return tmp;
        }
        public bool DeterminaSeNuovo(decimal id_Viaggio_Congedo)
        {
            bool tmp = false;
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    //db.Database.BeginTransaction();
                    var AVC = db.ATTIVAZIONIVIAGGICONGEDO.Where(a => a.IDVIAGGIOCONGEDO == id_Viaggio_Congedo && a.ATTIVARICHIESTA == true && a.ANNULLATO == false
                    && a.IDFASEVC == (decimal)EnumFaseViaggioCongedo.Documenti_di_Viaggio).ToList();
                    if (AVC?.Any() ?? false)
                        tmp = true;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                return tmp;
            }
        }

        public bool NotificaPreventiviInviata(decimal id_Attiv_Viaggio_Congedo, decimal idFaseInCorso)
        {
            bool tmp = false;
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    //db.Database.BeginTransaction();
                    var AVC = db.ATTIVAZIONIVIAGGICONGEDO.Where(a => a.IDATTIVAZIONEVC == id_Attiv_Viaggio_Congedo && a.IDFASEVC == idFaseInCorso && a.ANNULLATO == false).ToList();
                    if (AVC.Count != 0)
                        tmp = Convert.ToBoolean(AVC.First().NOTIFICARICHIESTA);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                return tmp;
            }
        }
        public bool AttivazionePreventiviInviata(decimal id_Attiv_Viaggio_Congedo, decimal idFaseInCorso, decimal idTrasferimento)
        {
            bool tmp = false;
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    //db.Database.BeginTransaction();
                    var AVC = db.ATTIVAZIONIVIAGGICONGEDO.Where(a => a.IDATTIVAZIONEVC == id_Attiv_Viaggio_Congedo && a.IDFASEVC == idFaseInCorso
                    && a.ANNULLATO == false && a.ATTIVARICHIESTA == true).ToList();
                    if (AVC?.Any() ?? false)
                        tmp = Convert.ToBoolean(AVC.First().ATTIVARICHIESTA);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                return tmp;
            }
        }

        public decimal IdentificaDocumentoSelezionato(decimal id_Attiv_Viaggio_Congedo)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    var lSelDoc = db.SELECTDOCVC.Where(x => x.IDATTIVAZIONEVC == id_Attiv_Viaggio_Congedo && x.DOCSELEZIONATO == true).ToList();
                    if (lSelDoc?.Any() ?? false)
                    {
                        tmp = lSelDoc.First().IDDOCUMENTO;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return tmp;
            }
        }


        private void EmailNotificaRichiestaTV(decimal idAttivazioneTitoliViaggio, ModelDBISE db)
        {
            TITOLIVIAGGIO tv = new TITOLIVIAGGIO();
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

                var atv = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoliViaggio);

                var conta_attivazioni = this.GetNumAttivazioniTV(atv.IDTITOLOVIAGGIO, db);

                tv = atv.TITOLIVIAGGIO;

                if (tv?.IDTITOLOVIAGGIO > 0)
                {
                    TRASFERIMENTO tr = tv.TRASFERIMENTO;
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
                                if (conta_attivazioni == 1)
                                {
                                    msgMail.oggetto = Resources.msgEmail.OggettoNotificaRichiestaInizialeTitoloViaggio;
                                    msgMail.corpoMsg =
                                            string.Format(
                                                Resources.msgEmail.MessaggioNotificaRichiestaInizialeTitoliViaggio,
                                                d.COGNOME + " " + d.NOME + " (" + d.MATRICOLA + ")",
                                                tr.DATAPARTENZA.ToLongDateString(),
                                                u.DESCRIZIONEUFFICIO + " (" + u.CODICEUFFICIO + ")");

                                }
                                else
                                {
                                    msgMail.oggetto = Resources.msgEmail.OggettoNotificaRichiestaSuccessivaTitoloViaggio;
                                    msgMail.corpoMsg =
                                            string.Format(
                                                Resources.msgEmail.MessaggioNotificaRichiestaSuccessivaTitoliViaggio,
                                                d.COGNOME + " " + d.NOME + " (" + d.MATRICOLA + ")",
                                                tr.DATAPARTENZA.ToLongDateString(),
                                                u.DESCRIZIONEUFFICIO + " (" + u.CODICEUFFICIO + ")");
                                }
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
        public decimal Crea_Attivazioni_Viaggi_Congedo(decimal id_Viaggio_Congedo, decimal idFaseVC, decimal idTrasferimento)
        {
            decimal tmp = 0;
            ATTIVAZIONIVIAGGICONGEDO atvViaggCong = new ATTIVAZIONIVIAGGICONGEDO();
            atvViaggCong.IDVIAGGIOCONGEDO = id_Viaggio_Congedo;
            atvViaggCong.NOTIFICARICHIESTA = false;
            atvViaggCong.ATTIVARICHIESTA = false;
            atvViaggCong.DATAAGGIORNAMENTO = DateTime.Now;
            atvViaggCong.ANNULLATO = false;
            atvViaggCong.DATAAGGIORNAMENTO = DateTime.Now;
            if (idFaseVC == 0) idFaseVC = 1;
            atvViaggCong.IDFASEVC = idFaseVC;// (decimal)EnumFaseViaggioCongedo.Preventivi;
                                             //  atvViaggCong.VIAGGICONGEDO.IDVIAGGIOCONGEDO = id_Viaggio_Congedo;

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);
                    var vcL = t.VIAGGICONGEDO.OrderByDescending(a => a.IDVIAGGIOCONGEDO).ToList();
                    if (vcL.Count() != 0)
                    {
                        id_Viaggio_Congedo = vcL.First().IDVIAGGIOCONGEDO;
                        var VC = db.VIAGGICONGEDO.Find(id_Viaggio_Congedo);
                        var z = VC.ATTIVAZIONIVIAGGICONGEDO.Where(y => y.IDFASEVC == idFaseVC && y.ANNULLATO == false);
                        if (z.Count() == 0)
                        {
                            VC.ATTIVAZIONIVIAGGICONGEDO.Add(atvViaggCong);
                            db.SaveChanges();
                            //db.Database.CurrentTransaction.Commit();
                            tmp = atvViaggCong.IDATTIVAZIONEVC;
                        }
                        else
                        {
                            decimal idUltmo = Restituisci_IdAttivazioniVC_IdFaseInCorso_Attuale(idTrasferimento)[0];
                            if (idUltmo != 0)
                                tmp = idUltmo;
                            else
                                tmp = z.First().IDATTIVAZIONEVC;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return tmp;
            }
        }

        public decimal Restituisci_Id_Attivazioni_Viaggi_Congedo_DA(decimal id_Viaggio_Congedo, decimal idFaseVC)
        {
            decimal tmp = 0;

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    var VC = db.VIAGGICONGEDO.Find(id_Viaggio_Congedo);
                    var z = VC.ATTIVAZIONIVIAGGICONGEDO.Where(y => y.IDFASEVC == idFaseVC && y.ANNULLATO == false);
                    if (z.Count() != 0)
                    {
                        tmp = z.First().IDATTIVAZIONEVC;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return tmp;
            }
        }

        public decimal[] Restituisci_IdAttivazioniVC_IdFaseInCorso_Attuale(decimal idTrasferimento)
        {
            decimal[] tmp = new decimal[] { 0, 0, 0, 0 };//idAttivazioneVC,idFaseInCorso,NOTIFCATA,ATTIVATA

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);
                    var vcL = t.VIAGGICONGEDO.OrderByDescending(a => a.IDVIAGGIOCONGEDO).ToList();
                    if (vcL.Count() != 0)
                    {
                        var vc = vcL.First();

                        var attVcL = vc.ATTIVAZIONIVIAGGICONGEDO.Where(a => a.ANNULLATO == false &&
                       a.NOTIFICARICHIESTA == true && a.ATTIVARICHIESTA == false).OrderBy(c => c.IDFASEVC).ThenBy(b => b.IDATTIVAZIONEVC).ToList();
                        if (attVcL.Count() != 0)
                        {
                            tmp[0] = attVcL.First().IDATTIVAZIONEVC;
                            tmp[1] = attVcL.First().IDFASEVC;
                            tmp[2] = Convert.ToDecimal(attVcL.First().NOTIFICARICHIESTA);
                            tmp[3] = Convert.ToDecimal(attVcL.First().ATTIVARICHIESTA);
                            return tmp;
                        }

                        attVcL = vc.ATTIVAZIONIVIAGGICONGEDO.Where(a => a.ANNULLATO == false &&
                        a.NOTIFICARICHIESTA == false).OrderBy(b => b.IDFASEVC).ToList();
                        if (attVcL.Count() != 0)
                        {
                            tmp[0] = attVcL.First().IDATTIVAZIONEVC;
                            tmp[1] = attVcL.First().IDFASEVC;
                            tmp[2] = Convert.ToDecimal(attVcL.First().NOTIFICARICHIESTA);
                            tmp[3] = Convert.ToDecimal(attVcL.First().ATTIVARICHIESTA);
                            return tmp;
                        }
                        attVcL = vc.ATTIVAZIONIVIAGGICONGEDO.Where(a => a.ANNULLATO == false &&
                        a.NOTIFICARICHIESTA == true && a.ATTIVARICHIESTA == false).OrderBy(b => b.IDFASEVC).ToList();
                        if (attVcL.Count() != 0)
                        {
                            tmp[0] = attVcL.First().IDATTIVAZIONEVC;
                            tmp[1] = attVcL.First().IDFASEVC;
                            tmp[2] = Convert.ToDecimal(attVcL.First().NOTIFICARICHIESTA);
                            tmp[3] = Convert.ToDecimal(attVcL.First().ATTIVARICHIESTA);
                            return tmp;
                        }
                        attVcL = vc.ATTIVAZIONIVIAGGICONGEDO.Where(a => a.ANNULLATO == false &&
                        a.NOTIFICARICHIESTA == true && a.ATTIVARICHIESTA == true).OrderByDescending(c => c.IDFASEVC).ThenBy(b => b.IDATTIVAZIONEVC).ToList();
                        if (attVcL?.Any() ?? false)
                        {
                            tmp[0] = attVcL.First().IDATTIVAZIONEVC;
                            tmp[1] = attVcL.First().IDFASEVC;
                            tmp[2] = Convert.ToDecimal(attVcL.First().NOTIFICARICHIESTA);
                            tmp[3] = Convert.ToDecimal(attVcL.First().ATTIVARICHIESTA);

                            return tmp;
                        }

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return tmp;
            }
        }

        public decimal Crea_Viaggi_Congedo(decimal idTrasferimento)
        {
            decimal tmp = 0;
            VIAGGICONGEDO VC = new VIAGGICONGEDO();
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();
                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                VC.IDTRASFERIMENTO = idTrasferimento;
                t.VIAGGICONGEDO.Add(VC);
                db.SaveChanges();
                db.Database.CurrentTransaction.Commit();
                tmp = VC.IDVIAGGIOCONGEDO;
            }
            return tmp;
        }

        public void InsertSelectDocVC(decimal idAttivazioneVC, decimal idDocumento)
        {
            SELECTDOCVC selDocVC = new SELECTDOCVC();
            selDocVC.IDATTIVAZIONEVC = idAttivazioneVC;
            selDocVC.IDDOCUMENTO = idDocumento;
            selDocVC.DOCSELEZIONATO = false;
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();
                    db.SELECTDOCVC.Add(selDocVC);
                    db.SaveChanges();
                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public void AggiornaTabellaCorrellata(decimal id_Attiv_Viaggio_Congedo, List<SelectDocVc> lSelDoc, ModelDBISE db)
        {
            try
            {
                foreach (var x in lSelDoc)
                {
                    SELECTDOCVC S = new SELECTDOCVC();
                    S.IDATTIVAZIONEVC = id_Attiv_Viaggio_Congedo;
                    S.IDDOCUMENTO = x.idDocumento;
                    S.DOCSELEZIONATO = false;
                    db.SELECTDOCVC.Add(S);
                    db.SaveChanges();
                    db.Database.CurrentTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                db.Database.CurrentTransaction.Rollback();
                throw ex;
            }
        }
        public decimal Identifica_Id_UltimoViaggioCongedoDisponibile(decimal idTrasferimento)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var vc = t.VIAGGICONGEDO.OrderByDescending(a => a.IDVIAGGIOCONGEDO);
                if (vc?.Any() ?? false)
                {
                    tmp = vc.First().IDVIAGGIOCONGEDO;
                }
            }
            return tmp;
        }
        public decimal Restituisci_ID_Viagg_CONG_DA(decimal idAttivazioneVC, decimal idTrasferimento)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                var atvc = db.ATTIVAZIONIVIAGGICONGEDO.Find(idAttivazioneVC);
                tmp = atvc.IDVIAGGIOCONGEDO;
            }
            return tmp;
        }
        public decimal Restituisci_ID_Viagg_CONG_DA_Trasferimento(decimal idTrasferimento)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var atvc = t.VIAGGICONGEDO.OrderByDescending(a => a.IDVIAGGIOCONGEDO).ToList();
                tmp = atvc.First().IDVIAGGIOCONGEDO;
            }
            return tmp;
        }
        public decimal Restituisci_LivelloFase_Da_ATT_Viagg_CONG(decimal idViaggioCongedio)
        {
            decimal tmp = (decimal)EnumFaseViaggioCongedo.Preventivi;
            using (ModelDBISE db = new ModelDBISE())
            {
                var atvc = db.ATTIVAZIONIVIAGGICONGEDO.Where(x => x.IDVIAGGIOCONGEDO == idViaggioCongedio &&
                x.ATTIVARICHIESTA == false && x.ANNULLATO == false).OrderBy(y => y.IDFASEVC);
                if (atvc?.Any() ?? false)// non lo è
                {
                    tmp = atvc.First().IDFASEVC;
                }
            }
            return tmp;
        }
        //AttivazioniViaggiCongedo
        public decimal Restituisci_Ultimo_ID_Fase_Da_ATT_Viagg_CONG(decimal idAttivazioneViaggioCongedo)
        {
            decimal tmp = (decimal)EnumFaseViaggioCongedo.Preventivi;
            using (ModelDBISE db = new ModelDBISE())
            {
                var atvc = db.ATTIVAZIONIVIAGGICONGEDO.Where(x => x.IDATTIVAZIONEVC == idAttivazioneViaggioCongedo && x.ANNULLATO == false).OrderByDescending(y => y.IDFASEVC);
                if (atvc?.Any() ?? false)// non lo è
                {
                    tmp = atvc.First().IDFASEVC;
                }
            }
            return tmp;
        }
        public List<AttivazioniViaggiCongedoModel> Cerca_Id_AttivazioniViaggiCongedoDisponibile(decimal idViaggiCongedio, decimal idFaseInCorso)
        {
            List<AttivazioniViaggiCongedoModel> tmp = new List<AttivazioniViaggiCongedoModel>();
            using (ModelDBISE db = new ModelDBISE())
            {
                var vc = db.VIAGGICONGEDO.Find(idViaggiCongedio);
                var lavc = vc.ATTIVAZIONIVIAGGICONGEDO.Where(y => y.ANNULLATO == false
                 && y.IDFASEVC == idFaseInCorso).OrderByDescending(a => a.IDATTIVAZIONEVC).ToList();

                tmp = (from e in lavc
                       select new AttivazioniViaggiCongedoModel()
                       {
                           idViaggioCongedo = idViaggiCongedio,
                           Annullato = e.ANNULLATO,
                           AttivaRichiesta = e.ATTIVARICHIESTA,
                           DataAggiornamento = e.DATAAGGIORNAMENTO,
                           //DataAttivaRichiesta= e.DATAATTIVARICHIESTA.Value,
                           //DataNotificaRichiesta=e.DATANOTIFICARICHIESTA.Value,
                           // FK_IDAttivazioneVC=(decimal)e.FK_IDATTIVAZIONEVC,
                           idAttivazioneVC = e.IDATTIVAZIONEVC,
                           idFaseVC = e.IDFASEVC,
                           NotificaRichiesta = e.NOTIFICARICHIESTA

                       }).ToList();
            }
            return tmp;
        }

        public decimal AttivaPreventiviRichiesta(decimal idAttivazioneVC, decimal idDocumento, decimal idFaseInCorso, decimal idTrasferimento)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();
                try
                {
                    var a = db.ATTIVAZIONIVIAGGICONGEDO.Find(idAttivazioneVC);
                    a.ATTIVARICHIESTA = true;
                    a.DATAATTIVARICHIESTA = DateTime.Now;
                    a.DATAAGGIORNAMENTO = DateTime.Now;
                    if (idFaseInCorso == (decimal)EnumFaseViaggioCongedo.Preventivi)
                    {
                        var s = a.SELECTDOCVC.Where(y => y.IDDOCUMENTO == idDocumento).ToList().First();
                        s.DOCSELEZIONATO = true;
                    }
                    if (idFaseInCorso == (decimal)EnumFaseViaggioCongedo.Documenti_di_Viaggio)
                    {
                        var dl = a.DOCUMENTI.ToList();//.Where(c => c.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Preventivo_Viaggio).ToList();
                        foreach (var d in dl)
                        {
                            d.IDSTATORECORD = (decimal)EnumStatoRecord.Attivato;
                        }
                    }
                    db.SaveChanges();

                    var b = db.ATTIVAZIONIVIAGGICONGEDO.Where(x => x.IDVIAGGIOCONGEDO == a.VIAGGICONGEDO.IDVIAGGIOCONGEDO && x.IDFASEVC == idFaseInCorso && x.ANNULLATO == false);
                    if (b.Count() == 0)
                        tmp = Crea_Attivazioni_Viaggi_Congedo(a.VIAGGICONGEDO.IDVIAGGIOCONGEDO, idFaseInCorso, idTrasferimento);

                    using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                    {
                        dtce.ModificaInCompletatoCalendarioEvento(idTrasferimento, EnumFunzioniEventi.RichiestaViaggiCongedo, db);
                    }
                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw new Exception("Selezionare un elemento per l'Attivazione");
                }
            }
            return tmp;
        }

        public bool CaricatiElementiFASE2(decimal idAttivViaggioCongedo, decimal id_Viaggio_Congedo)
        {
            bool tmp = false;
            using (ModelDBISE db = new ModelDBISE())
            {
                //  db.Database.BeginTransaction();
                try
                {
                    //if (idAttivViaggioCongedo == 0)
                    //    idAttivViaggioCongedo = Restituisci_Id_Attivazioni_Viaggi_Congedo_DA(id_Viaggio_Congedo, (decimal)EnumFaseViaggioCongedo.Documenti_di_Viaggio);
                    if (idAttivViaggioCongedo != 0)
                    {
                        var a = db.ATTIVAZIONIVIAGGICONGEDO.Find(idAttivViaggioCongedo);
                        if (a != null)
                        {
                            //if (a.NOTIFICARICHIESTA == false)
                            //{
                            if (a.IDFASEVC != (decimal)EnumFaseViaggioCongedo.Documenti_di_Viaggio)
                            {
                                idAttivViaggioCongedo = Restituisci_Id_Attivazioni_Viaggi_Congedo_DA(a.VIAGGICONGEDO.IDVIAGGIOCONGEDO, (decimal)EnumFaseViaggioCongedo.Documenti_di_Viaggio);
                                a = db.ATTIVAZIONIVIAGGICONGEDO.Find(idAttivViaggioCongedo);
                            }
                            var doc1 = a.DOCUMENTI.Where(b => b.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Carta_Imbarco).ToList();
                            var doc2 = a.DOCUMENTI.Where(b => b.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Titolo_Viaggio).ToList();
                            if (doc1.Count() != 0 && doc2.Count() != 0)
                                tmp = true;
                        }
                        // }
                    }
                }
                catch (Exception ex)
                {
                    //   db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
            return tmp;
        }

        public void NotificaPreventiviRichiesta(decimal idAttivazioneVC, decimal idFaseInCorso)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();
                try
                {
                    var a = db.ATTIVAZIONIVIAGGICONGEDO.Find(idAttivazioneVC);

                    if (a.IDFASEVC != idFaseInCorso)
                    {
                        throw new Exception("Fase non in linea con l'interfaccia...");
                    }

                    a.NOTIFICARICHIESTA = true;
                    a.DATANOTIFICARICHIESTA = DateTime.Now;
                    a.DATAAGGIORNAMENTO = DateTime.Now;

                    var dl = a.DOCUMENTI.Where(b => b.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione && b.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Preventivo_Viaggio).ToList();
                    foreach (var d in dl)
                    {
                        d.IDSTATORECORD = (decimal)EnumStatoRecord.Da_Attivare;
                    }
                    db.SaveChanges();


                    using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                    {
                        CalendarioEventiModel cem = new CalendarioEventiModel()
                        {
                            idFunzioneEventi = EnumFunzioniEventi.RichiestaViaggiCongedo,
                            idTrasferimento = a.VIAGGICONGEDO.IDTRASFERIMENTO,
                            DataInizioEvento = DateTime.Now.Date,
                            DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaViaggiCongedo)).Date,
                        };
                        dtce.InsertCalendarioEvento(ref cem, db);
                    }

                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw new Exception("Errore in fase di Notifica");
                }
            }
        }
        public void AnnullaPreventiviRichiesta(decimal idAttivazioneVC, decimal idFaseInCorso, decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                decimal idAttBK = idAttivazioneVC;
                db.Database.BeginTransaction();
                try
                {
                    decimal idVC = Restituisci_ID_Viagg_CONG_DA(idAttivazioneVC, idTrasferimento);
                    decimal[] tmp = Restituisci_IdAttivazioniVC_IdFaseInCorso_Attuale(idTrasferimento);
                    idFaseInCorso = tmp[1]; idAttivazioneVC = tmp[0];
                    bool fase1Att = false;
                    if (idFaseInCorso == (decimal)EnumFaseViaggioCongedo.Preventivi)
                    {
                        fase1Att = AttivazionePreventiviInviata(idAttivazioneVC, idFaseInCorso, idTrasferimento);
                    }
                    if (fase1Att == true)
                    {
                        idFaseInCorso = (decimal)EnumFaseViaggioCongedo.Documenti_di_Viaggio;
                        idAttivazioneVC = Restituisci_Id_Attivazioni_Viaggi_Congedo_DA(idVC, idFaseInCorso);
                    }
                    decimal IdAtt_VC_Prev_New = DupplicaAttivazionePreventivo(idAttivazioneVC, db);//con salvataggio immediato in DB
                    var a = db.ATTIVAZIONIVIAGGICONGEDO.Find(idAttivazioneVC);
                    a.DATAAGGIORNAMENTO = DateTime.Now;
                    a.ANNULLATO = true;

                    //prima fase
                    if (idFaseInCorso == (decimal)EnumFaseViaggioCongedo.Preventivi)
                    {
                        var selList = a.SELECTDOCVC.ToList();
                        foreach (var docSel in selList)
                        {

                            DOCUMENTI dNewSel = DupplicaDocumento(docSel.IDDOCUMENTO, db);//senza salvataggio immediato in DB
                            var nuovoATTSel = db.ATTIVAZIONIVIAGGICONGEDO.Find(IdAtt_VC_Prev_New);
                            nuovoATTSel.DOCUMENTI.Add(dNewSel);
                            db.SaveChanges();
                            var dsel = db.DOCUMENTI.Find(docSel.IDDOCUMENTO);
                            dsel.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                            db.SaveChanges();
                            //Prima fase devi inserire a manella
                            var idDocNew = dNewSel.IDDOCUMENTO;
                            SELECTDOCVC selNew = new SELECTDOCVC();
                            selNew.IDATTIVAZIONEVC = IdAtt_VC_Prev_New;
                            selNew.IDDOCUMENTO = idDocNew;
                            selNew.DOCSELEZIONATO = false;
                            db.SELECTDOCVC.Add(selNew);
                            db.SaveChanges();
                        }
                    }
                    // var dl = a.DOCUMENTI.Where(b => b.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare && b.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Preventivo_Viaggio).ToList();
                    if (idFaseInCorso == (decimal)EnumFaseViaggioCongedo.Documenti_di_Viaggio)
                    {
                        var bkAtt = db.ATTIVAZIONIVIAGGICONGEDO.Find(idAttivazioneVC);
                        var dl = bkAtt.DOCUMENTI.ToList();

                        // var dl = a.DOCUMENTI.Where(b => b.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Preventivo_Viaggio).ToList();
                        if (dl.Count() == 0)
                        {
                            idFaseInCorso = (decimal)EnumFaseViaggioCongedo.Preventivi;
                            idAttivazioneVC = Restituisci_Id_Attivazioni_Viaggi_Congedo_DA(idVC, idFaseInCorso);
                            a = db.ATTIVAZIONIVIAGGICONGEDO.Find(idAttivazioneVC);
                            dl = a.DOCUMENTI.Where(b => b.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Preventivo_Viaggio).ToList();
                        }
                        foreach (var d in dl)
                        {
                            DOCUMENTI dNew = DupplicaDocumento(d.IDDOCUMENTO, db);//senza salvataggio immediato in DB
                            var nuovoATT = db.ATTIVAZIONIVIAGGICONGEDO.Find(IdAtt_VC_Prev_New);
                            nuovoATT.DOCUMENTI.Add(dNew);
                            d.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                            db.SaveChanges();
                        }
                    }
                    //   db.SaveChanges();
                    using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                    {
                        dtce.AnnullaMessaggioEvento(idTrasferimento, EnumFunzioniEventi.RichiestaViaggiCongedo, db);
                    }
                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw new Exception("Errore in fase di Notifica");
                }
            }
        }

        public decimal DupplicaAttivazionePreventivo(decimal idAttivazioneVC, ModelDBISE db)
        {
            //using (ModelDBISE db1 = new ModelDBISE())
            //{
            //  db1.Database.BeginTransaction();

            var AttOld = db.ATTIVAZIONIVIAGGICONGEDO.Find(idAttivazioneVC);
            ATTIVAZIONIVIAGGICONGEDO an = new ATTIVAZIONIVIAGGICONGEDO();
            an.ANNULLATO = false;
            an.ATTIVARICHIESTA = false;
            an.DATAAGGIORNAMENTO = DateTime.Now;
            an.FK_IDATTIVAZIONEVC = AttOld.IDATTIVAZIONEVC;
            an.IDFASEVC = AttOld.IDFASEVC;
            an.IDVIAGGIOCONGEDO = AttOld.IDVIAGGIOCONGEDO;
            an.NOTIFICARICHIESTA = false;
            db.ATTIVAZIONIVIAGGICONGEDO.Add(an);
            db.SaveChanges();
            // db1.Database.CurrentTransaction.Commit();
            return an.IDATTIVAZIONEVC;
            //}
        }

        public DOCUMENTI DupplicaDocumento(decimal idDocumento, ModelDBISE db)
        {
            var dVecchio = db.DOCUMENTI.Find(idDocumento);
            DOCUMENTI d = new DOCUMENTI();
            d.FK_IDDOCUMENTO = dVecchio.IDDOCUMENTO;//dovrebbe essere l'id vecchio da annullare
            d.IDTIPODOCUMENTO = dVecchio.IDTIPODOCUMENTO;
            d.NOMEDOCUMENTO = dVecchio.NOMEDOCUMENTO;
            d.ESTENSIONE = dVecchio.ESTENSIONE; ;
            d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;
            d.FILEDOCUMENTO = dVecchio.FILEDOCUMENTO;

            return d;
        }

        public decimal ConteggiaPreventiviRichiesta(decimal idAttivazioneVC)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                //db.Database.BeginTransaction();
                try
                {
                    var a = db.ATTIVAZIONIVIAGGICONGEDO.Find(idAttivazioneVC);
                    var sel = a.SELECTDOCVC.ToList();
                    tmp = sel.Count;
                }
                catch (Exception ex)
                {
                    //db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
            return tmp;
        }


        public void PreSetTitoloViaggio(decimal idTrasferimento, ModelDBISE db)
        {

            TITOLIVIAGGIO tv = new TITOLIVIAGGIO();

            tv.IDTITOLOVIAGGIO = idTrasferimento;

            db.TITOLIVIAGGIO.Add(tv);

            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Errore nella fase d'inserimento dei dati per la gestione dei titoli di viaggio.");
            }
            else
            {
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                    "Inserimento dei dati di gestione per i titoli di viaggio.", "TITOLIVIAGGIO", db, idTrasferimento,
                    tv.IDTITOLOVIAGGIO);
            }

        }

        public decimal GetIdAltriDatiFamiliari(decimal idTitoliViaggio, decimal idFamiliare, EnumParentela parentela)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                decimal idAltridatiFamiliari = 0;

                switch (parentela)
                {
                    case EnumParentela.Coniuge:
                        var ctv = db.CONIUGETITOLIVIAGGIO.Where(a => a.IDTITOLOVIAGGIO == idTitoliViaggio && a.ANNULLATO == false).First().CONIUGE.First();
                        var adfc = ctv.ALTRIDATIFAM.First();
                        idAltridatiFamiliari = adfc.IDALTRIDATIFAM;
                        break;

                    case EnumParentela.Figlio:
                        var ftv = db.FIGLITITOLIVIAGGIO.Where(a => a.IDTITOLOVIAGGIO == idTitoliViaggio && a.ANNULLATO == false).First().FIGLI.First();
                        var adff = ftv.ALTRIDATIFAM.First();
                        idAltridatiFamiliari = adff.IDALTRIDATIFAM;
                        break;

                    default:
                        break;
                }

                return idAltridatiFamiliari;
            }
        }

        public List<ElencoTitoliViaggioModel> ElencoTitoliViaggio(decimal idTitoloViaggio)
        {
            List<ElencoTitoliViaggioModel> letvm = new List<ElencoTitoliViaggioModel>();


            using (ModelDBISE db = new ModelDBISE())
            {

                var tv = db.TITOLIVIAGGIO.Find(idTitoloViaggio);

                //richiedente
                var ltvr = tv.TITOLIVIAGGIORICHIEDENTE.Where(a => a.ANNULLATO == false).ToList();
                var t = tv.TRASFERIMENTO;
                var d = t.DIPENDENTI;
                var mf = t.MAGGIORAZIONIFAMILIARI;
                var amf = mf.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true && a.ATTIVAZIONEMAGFAM == true).OrderBy(a => a.IDATTIVAZIONEMAGFAM).First();
                var atv = tv.ATTIVAZIONETITOLIVIAGGIO.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDATTIVAZIONETITOLIVIAGGIO).First();


                if (ltvr?.Any() ?? false)
                {
                    foreach (var tvr in ltvr)
                    {
                        ElencoTitoliViaggioModel etvrm = new ElencoTitoliViaggioModel()
                        {
                            idFamiliare = d.IDDIPENDENTE,
                            Nominativo = d.NOME + " " + d.COGNOME,
                            CodiceFiscale = "",
                            dataInizio = t.DATAPARTENZA,
                            dataFine = t.DATARIENTRO,
                            parentela = EnumParentela.Richiedente,
                            idAltriDati = 0,
                            RichiediTitoloViaggio = tvr.RICHIEDITITOLOVIAGGIO,
                            idAttivazioneTitoloViaggio = tvr.IDATTIVAZIONETITOLIVIAGGIO,
                            idTitoloViaggio = tvr.IDTITOLOVIAGGIO
                        };
                        letvm.Add(etvrm);
                    }
                }

                //coniuge
                var ltvc = atv.CONIUGETITOLIVIAGGIO.Where(a => a.ANNULLATO == false).ToList();

                if (ltvc?.Any() ?? false)
                {
                    foreach (var tvc in ltvc)
                    {
                        var c = tvc.CONIUGE.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).First();

                        ElencoTitoliViaggioModel etvcm = new ElencoTitoliViaggioModel()
                        {
                            idFamiliare = c.IDCONIUGE,
                            Nominativo = c.NOME + " " + c.COGNOME,
                            CodiceFiscale = c.CODICEFISCALE,
                            dataInizio = c.DATAINIZIOVALIDITA,
                            dataFine = c.DATAFINEVALIDITA,
                            parentela = EnumParentela.Coniuge,
                            idAltriDati = this.GetIdAltriDatiFamiliari(tvc.IDTITOLOVIAGGIO, c.IDCONIUGE, EnumParentela.Coniuge),
                            RichiediTitoloViaggio = tvc.RICHIEDITITOLOVIAGGIO,
                            idAttivazioneTitoloViaggio = tvc.IDATTIVAZIONETITOLIVIAGGIO,
                            idTitoloViaggio = tvc.IDTITOLOVIAGGIO
                        };
                        letvm.Add(etvcm);

                    }
                }

                //figli
                var ltvf = atv.FIGLITITOLIVIAGGIO.Where(a => a.ANNULLATO == false).ToList();

                if (ltvf?.Any() ?? false)
                {
                    foreach (var tvf in ltvf)
                    {
                        var f = tvf.FIGLI.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).First();

                        ElencoTitoliViaggioModel etvfm = new ElencoTitoliViaggioModel()
                        {
                            idFamiliare = f.IDFIGLI,
                            Nominativo = f.NOME + " " + f.COGNOME,
                            CodiceFiscale = f.CODICEFISCALE,
                            dataInizio = f.DATAINIZIOVALIDITA,
                            dataFine = f.DATAFINEVALIDITA,
                            parentela = EnumParentela.Figlio,
                            idAltriDati = this.GetIdAltriDatiFamiliari(tvf.IDTITOLOVIAGGIO, f.IDFIGLI, EnumParentela.Figlio),
                            RichiediTitoloViaggio = tvf.RICHIEDITITOLOVIAGGIO,
                            idAttivazioneTitoloViaggio = tvf.IDATTIVAZIONETITOLIVIAGGIO,
                            idTitoloViaggio = tvf.IDTITOLOVIAGGIO
                        };
                        letvm.Add(etvfm);

                    }
                }

            }

            return letvm;
        }

        public decimal GetIdTitoliViaggio(decimal idTrasferimento)

        {
            using (ModelDBISE db = new ModelDBISE())
            {
                decimal idTitoliViaggio = db.TITOLIVIAGGIO.Find(idTrasferimento).IDTITOLOVIAGGIO;
                if (idTitoliViaggio <= 0)
                {
                    throw new Exception("Errore nella lettura dei dati del titolo di viaggio.");
                }

                return idTitoliViaggio;

            }
        }

        public ATTIVAZIONETITOLIVIAGGIO GetUltimaAttivazioneNotificata(decimal idTitoloViaggio)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                ATTIVAZIONETITOLIVIAGGIO atv_notificata = new ATTIVAZIONETITOLIVIAGGIO();
                var latv_notificate = db.TITOLIVIAGGIO.Find(idTitoloViaggio).ATTIVAZIONETITOLIVIAGGIO
                    .Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == true && a.ATTIVAZIONERICHIESTA == false)
                    .OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO).ToList();
                if (latv_notificate?.Any() ?? false)
                {
                    atv_notificata = latv_notificate.First();
                }

                return atv_notificata;
            }
        }

        public decimal Crea_Nuovo_Id_ViaggiCongedo(decimal idTrasferimento)
        {
            decimal tmp = 0;
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    VIAGGICONGEDO VC = new VIAGGICONGEDO();
                    VC.IDTRASFERIMENTO = idTrasferimento;
                    db.VIAGGICONGEDO.Add(VC);
                    db.SaveChanges();
                    tmp = VC.IDVIAGGIOCONGEDO;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tmp;
        }

        public bool AttivazioneNotificata(decimal idAttivazioneTitoliViaggio)
        {
            bool notificata = false;
            using (ModelDBISE db = new ModelDBISE())
            {
                ATTIVAZIONETITOLIVIAGGIO atv = new ATTIVAZIONETITOLIVIAGGIO();
                atv = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoliViaggio);

                if (atv.ATTIVAZIONERICHIESTA == false && atv.NOTIFICARICHIESTA == true && atv.ANNULLATO == false)
                {
                    notificata = true;
                }
                return notificata;
            }
        }
        public decimal GetNumDocumenti(decimal idAttivViaggioCongedo, EnumTipoDoc tipoDocumento)
        {
            decimal nDoc = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                var tv = db.ATTIVAZIONIVIAGGICONGEDO.Find(idAttivViaggioCongedo);
                nDoc = tv.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == (decimal)tipoDocumento).ToList().Count;
            }
            return nDoc;
        }

        public decimal GetNumAttivazioniTV(decimal idTitoliViaggio, ModelDBISE db)
        {
            //using (ModelDBISE db = new ModelDBISE())
            //{
            var NumAttivazioni = 0;
            NumAttivazioni = db.TITOLIVIAGGIO.Find(idTitoliViaggio).ATTIVAZIONETITOLIVIAGGIO
                                .Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == true)
                                .OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO).Count();
            return NumAttivazioni;
            //}
        }

        public List<ViaggioCongedoModel> GetUltimiPreventiviViaggio(List<AttivazioniViaggiCongedoModel> AttivazioniViaggiCongedo)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                List<ViaggioCongedoModel> lvcm = new List<ViaggioCongedoModel>();
                foreach (var x in AttivazioniViaggiCongedo)
                {
                    decimal idAttivazioniViaggiCongedo = x.idAttivazioneVC;
                    var t = db.ATTIVAZIONIVIAGGICONGEDO.Find(idAttivazioniViaggiCongedo);
                    var dvc = t.SELECTDOCVC.ToList();
                    foreach (var avc in dvc)
                    {
                        ViaggioCongedoModel vcm = new ViaggioCongedoModel()
                        {
                            AttivaRichiesta = t.ATTIVARICHIESTA,
                            Estensione = avc.DOCUMENTI.ESTENSIONE,
                            idAttivazioneVC = avc.IDATTIVAZIONEVC,
                            idDocumento = avc.IDDOCUMENTO,
                            idTipoDocumento = avc.DOCUMENTI.IDTIPODOCUMENTO,
                            idTrasferimento = t.VIAGGICONGEDO.IDTRASFERIMENTO,
                            idViaggioCongedo = t.VIAGGICONGEDO.IDVIAGGIOCONGEDO,
                            NomeFile = avc.DOCUMENTI.NOMEDOCUMENTO,
                            NotificaRichiesta = t.NOTIFICARICHIESTA
                        };
                        lvcm.Add(vcm);
                    }
                }
                return lvcm;
            }
        }

        public List<ViaggioCongedoModel> GetDocFase2Viaggio(decimal idAttivazioneVC, decimal idTipoDocumento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                List<ViaggioCongedoModel> lvcm = new List<ViaggioCongedoModel>();
                var t = db.ATTIVAZIONIVIAGGICONGEDO.Find(idAttivazioneVC);
                var d = t.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == idTipoDocumento).ToList();
                foreach (var vc in d)
                {
                    ViaggioCongedoModel vcm = new ViaggioCongedoModel()
                    {
                        AttivaRichiesta = t.ATTIVARICHIESTA,
                        Estensione = vc.ESTENSIONE,
                        idAttivazioneVC = t.IDATTIVAZIONEVC,
                        idDocumento = vc.IDDOCUMENTO,
                        idTipoDocumento = vc.IDTIPODOCUMENTO,
                        idTrasferimento = t.VIAGGICONGEDO.IDTRASFERIMENTO,
                        idViaggioCongedo = t.VIAGGICONGEDO.IDVIAGGIOCONGEDO,
                        NomeFile = vc.NOMEDOCUMENTO,
                        NotificaRichiesta = t.NOTIFICARICHIESTA
                    };
                    lvcm.Add(vcm);
                }
                //}
                return lvcm;
            }
        }

        public decimal Restituisci_ID_Destinatario(decimal idAttivazioneVC)
        {
            decimal tmp = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                var atvc = db.ATTIVAZIONIVIAGGICONGEDO.Find(idAttivazioneVC);
                tmp = atvc.VIAGGICONGEDO.TRASFERIMENTO.IDDIPENDENTE;
            }
            return tmp;
        }

        public List<ViaggioCongedoModel> GetUltimiPreventiviViaggio(decimal id_Attiv_Viaggio_Congedo, decimal idTrasferimento, decimal idViaggiCongedo)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                List<ViaggioCongedoModel> lvcm = new List<ViaggioCongedoModel>();
                // List<AttivazioniViaggiCongedoModel> lvcm = new List<AttivazioniViaggiCongedoModel>();
                //var vc = db.VIAGGICONGEDO.Find(id_Viaggio_Congedo);
                var t = db.ATTIVAZIONIVIAGGICONGEDO.Find(id_Attiv_Viaggio_Congedo);
                if (t.IDFASEVC == (decimal)EnumFaseViaggioCongedo.Documenti_di_Viaggio)
                {
                    //recuperare la prima fase
                    decimal idFase = (decimal)EnumFaseViaggioCongedo.Preventivi;
                    var vc = db.VIAGGICONGEDO.Find(idViaggiCongedo);
                    var t2 = vc.ATTIVAZIONIVIAGGICONGEDO.Where(a => a.IDFASEVC == idFase && a.ANNULLATO == false).ToList();
                    if (t2?.Any() ?? false)
                    {
                        decimal x = t2.First().IDATTIVAZIONEVC;
                        t = db.ATTIVAZIONIVIAGGICONGEDO.Find(x);
                    }
                }
                var lavc = t.SELECTDOCVC.ToList();

                if (lavc?.Any() ?? false)
                {
                    if (t.IDFASEVC == (decimal)EnumFaseViaggioCongedo.Documenti_di_Viaggio)
                    {
                        decimal idFase = (decimal)EnumFaseViaggioCongedo.Preventivi;
                        decimal idVC = Restituisci_ID_Viagg_CONG_DA(id_Attiv_Viaggio_Congedo, idTrasferimento);
                        var VC = db.VIAGGICONGEDO.Find(idVC);
                        var lAttVC = VC.ATTIVAZIONIVIAGGICONGEDO.Where(a => a.IDFASEVC == idFase);
                        if (lAttVC?.Any() ?? false)
                        {
                            id_Attiv_Viaggio_Congedo = lAttVC.First().IDATTIVAZIONEVC;
                            t = db.ATTIVAZIONIVIAGGICONGEDO.Find(id_Attiv_Viaggio_Congedo);
                            lavc = t.SELECTDOCVC.ToList();
                        }
                    }
                }

                foreach (var avc in lavc)
                {
                    ViaggioCongedoModel vcm = new ViaggioCongedoModel()
                    {
                        AttivaRichiesta = t.ATTIVARICHIESTA,
                        Estensione = avc.DOCUMENTI.ESTENSIONE,
                        idAttivazioneVC = avc.IDATTIVAZIONEVC,
                        idDocumento = avc.IDDOCUMENTO,
                        idTipoDocumento = avc.DOCUMENTI.IDTIPODOCUMENTO,
                        idTrasferimento = t.VIAGGICONGEDO.IDTRASFERIMENTO,
                        idViaggioCongedo = t.VIAGGICONGEDO.IDVIAGGIOCONGEDO,
                        NomeFile = avc.DOCUMENTI.NOMEDOCUMENTO,
                        NotificaRichiesta = t.NOTIFICARICHIESTA,
                        DocSelezionato = avc.DOCSELEZIONATO
                    };
                    lvcm.Add(vcm);
                }
                return lvcm;
            }
        }

        public List<ViaggioCongedoModel> GetPrecedentiPreventiviViaggio(decimal id_Attiv_Viaggio_Congedo)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                List<ViaggioCongedoModel> lvcm = new List<ViaggioCongedoModel>();
                var t = db.ATTIVAZIONIVIAGGICONGEDO.Find(id_Attiv_Viaggio_Congedo);
                decimal idViaggioCongedio = t.VIAGGICONGEDO.IDVIAGGIOCONGEDO;
                var z = db.VIAGGICONGEDO.Find(idViaggioCongedio);
                var w = z.ATTIVAZIONIVIAGGICONGEDO.Where(y => y.IDATTIVAZIONEVC != id_Attiv_Viaggio_Congedo && y.IDFASEVC == (decimal)EnumFaseViaggioCongedo.Preventivi && y.ANNULLATO == false).OrderByDescending(x => x.IDATTIVAZIONEVC).ToList();
                foreach (var q in w)
                {
                    var lavc = q.SELECTDOCVC.ToList();
                    foreach (var avc in lavc)
                    {
                        ViaggioCongedoModel vcm = new ViaggioCongedoModel()
                        {
                            AttivaRichiesta = q.ATTIVARICHIESTA,
                            Estensione = avc.DOCUMENTI.ESTENSIONE,
                            idAttivazioneVC = avc.IDATTIVAZIONEVC,
                            idDocumento = avc.IDDOCUMENTO,
                            idTipoDocumento = avc.DOCUMENTI.IDTIPODOCUMENTO,
                            idTrasferimento = q.VIAGGICONGEDO.IDTRASFERIMENTO,
                            idViaggioCongedo = q.VIAGGICONGEDO.IDVIAGGIOCONGEDO,
                            NomeFile = avc.DOCUMENTI.NOMEDOCUMENTO,
                            NotificaRichiesta = q.NOTIFICARICHIESTA,
                            DocSelezionato = avc.DOCSELEZIONATO
                        };
                        lvcm.Add(vcm);
                    }
                }
                return lvcm;
            }
        }
        public bool richiestaEseguita(decimal idTitoliViaggio)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                decimal NumAttivazioni = this.GetNumAttivazioniTV(idTitoliViaggio, db);
                if (NumAttivazioni == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public IList<ViaggioCongedoModel> GetListDocumentiViaggioCongedoByTipoDoc(decimal id_Attiv_Viaggio_Congedo, decimal idTipoDoc)
        {
            List<ViaggioCongedoModel> latvm = new List<ViaggioCongedoModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lavc = db.ATTIVAZIONIVIAGGICONGEDO.Find(id_Attiv_Viaggio_Congedo);
                //var latv = tv.ATTIVAZIONETITOLIVIAGGIO.Where
                //        (a => (a.ATTIVAZIONERICHIESTA == true && a.NOTIFICARICHIESTA == true) || a.ANNULLATO == false)
                //        .OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO)
                //        .ToList();
                var ld = lavc.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == idTipoDoc).ToList();
                if (ld?.Any() ?? false)
                {
                    foreach (var el in ld)
                    {
                        var new_vc = new ViaggioCongedoModel()
                        {
                            idAttivazioneVC = lavc.IDATTIVAZIONEVC,
                            AttivaRichiesta = lavc.ATTIVARICHIESTA,
                            //DocSelezionato = lavc.SELECTDOCVC,
                            Estensione = el.ESTENSIONE,
                            idDocumento = el.IDDOCUMENTO,
                            idTipoDocumento = el.IDTIPODOCUMENTO,
                            idStatoRecord = el.IDSTATORECORD,
                            idViaggioCongedo = lavc.IDVIAGGIOCONGEDO,
                            NomeFile = el.NOMEDOCUMENTO,
                            NotificaRichiesta = lavc.NOTIFICARICHIESTA,
                            idTrasferimento = lavc.VIAGGICONGEDO.IDTRASFERIMENTO
                        };
                        latvm.Add(new_vc);
                    }
                }
            }

            return latvm;
        }


        public void SituazioneAttivazioniTitoliViaggio(decimal idAttivazioneTitoliViaggio, out bool notificaRichiesta, out bool attivazioneRichiesta)
        {
            notificaRichiesta = false;
            attivazioneRichiesta = false;

            using (ModelDBISE db = new ModelDBISE())
            {
                var atv = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoliViaggio);
                if (atv != null && atv.IDATTIVAZIONETITOLIVIAGGIO > 0)
                {
                    notificaRichiesta = atv.NOTIFICARICHIESTA;
                    attivazioneRichiesta = atv.ATTIVAZIONERICHIESTA;

                }
            }
        }


        public List<VariazioneDocumentiModel> GetDocumentiTV(decimal idTitoliViaggio, decimal idTipoDoc)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tv = db.TITOLIVIAGGIO.Find(idTitoliViaggio);

                var latv = tv.ATTIVAZIONETITOLIVIAGGIO.Where(a => (a.ATTIVAZIONERICHIESTA == true && a.NOTIFICARICHIESTA == true) || a.ANNULLATO == false).OrderBy(a => a.IDATTIVAZIONETITOLIVIAGGIO).ToList();

                var i = 1;
                var coloresfondo = "";
                var coloretesto = "";

                if (latv?.Any() ?? false)
                {
                    foreach (var atv in latv)
                    {
                        var ld = atv.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == idTipoDoc).OrderByDescending(a => a.DATAINSERIMENTO).ToList();

                        bool modificabile = false;
                        if (atv.ATTIVAZIONERICHIESTA == false && atv.NOTIFICARICHIESTA == false && atv.TITOLIVIAGGIO.TRASFERIMENTO.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato)
                        {
                            modificabile = true;
                            coloresfondo = Resources.TitoliViaggioColori.AttivazioniTitoloViaggioAbilitate_Sfondo;
                            coloretesto = Resources.TitoliViaggioColori.AttivazioniTitoloViaggioAbilitate_Testo;
                        }
                        else
                        {
                            if (i % 2 == 0)
                            {
                                coloresfondo = Resources.TitoliViaggioColori.AttivazioniTitoloViaggioDisabilitate_SfondoDispari;
                            }
                            else
                            {
                                coloresfondo = Resources.TitoliViaggioColori.AttivazioniTitoloViaggioDisabilitate_SfondoPari;
                            }
                            coloretesto = Resources.TitoliViaggioColori.AttivazioniTitoloViaggioDisabilitate_Testo;
                        }

                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            var t = dtt.GetTrasferimentoByIdTitoloViaggio(idTitoliViaggio);
                            EnumStatoTraferimento statoTrasferimento = t.idStatoTrasferimento;
                            if (statoTrasferimento == EnumStatoTraferimento.Annullato)
                            {
                                modificabile = false;
                            }
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
                                IdAttivazione = atv.IDATTIVAZIONETITOLIVIAGGIO,
                                DataAggiornamento = atv.DATAAGGIORNAMENTO,
                                ColoreSfondo = coloresfondo,
                                ColoreTesto = coloretesto,
                                progressivo = i
                            };

                            ldm.Add(amf);
                        }

                        if (ld.Count > 0)
                        {
                            i++;
                        }

                    }

                }
            }

            return ldm;

        }

        public List<VariazioneDocumentiModel> GetDocumentiTVbyIdAttivazioneTV(decimal idTitoliViaggio, decimal idAttivazioneTitoliViaggio, decimal idTipoDocumento)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tv = db.TITOLIVIAGGIO.Find(idTitoliViaggio);

                var latv = tv.ATTIVAZIONETITOLIVIAGGIO.Where(a => ((a.ATTIVAZIONERICHIESTA == true && a.NOTIFICARICHIESTA == true) || a.ANNULLATO == false)).OrderBy(a => a.IDATTIVAZIONETITOLIVIAGGIO);
                var i = 1;
                var coloretesto = "";
                var coloresfondo = "";

                if (latv?.Any() ?? false)
                {
                    foreach (var e in latv)
                    {
                        var ld = e.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == idTipoDocumento).OrderByDescending(a => a.DATAINSERIMENTO).ToList();

                        if (e.IDATTIVAZIONETITOLIVIAGGIO == idAttivazioneTitoliViaggio)
                        {

                            bool modificabile = false;

                            if (e.ATTIVAZIONERICHIESTA == false && e.NOTIFICARICHIESTA == false && e.TITOLIVIAGGIO.TRASFERIMENTO.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato)
                            {
                                modificabile = true;
                                coloretesto = Resources.TitoliViaggioColori.AttivazioniTitoloViaggioAbilitate_Testo;
                                coloresfondo = Resources.TitoliViaggioColori.AttivazioniTitoloViaggioAbilitate_Sfondo;
                            }
                            else
                            {
                                coloretesto = Resources.TitoliViaggioColori.AttivazioniTitoloViaggioDisabilitate_Testo;
                                coloresfondo = Resources.TitoliViaggioColori.AttivazioniTitoloViaggioDisabilitate_SfondoPari;
                            }

                            foreach (var doc in ld)
                            {
                                var atv = new VariazioneDocumentiModel()
                                {
                                    dataInserimento = doc.DATAINSERIMENTO,
                                    estensione = doc.ESTENSIONE,
                                    idDocumenti = doc.IDDOCUMENTO,
                                    nomeDocumento = doc.NOMEDOCUMENTO,
                                    Modificabile = modificabile,
                                    IdAttivazione = e.IDATTIVAZIONETITOLIVIAGGIO,
                                    DataAggiornamento = e.DATAAGGIORNAMENTO,
                                    ColoreTesto = coloretesto,
                                    ColoreSfondo = coloresfondo,
                                    progressivo = i
                                };

                                ldm.Add(atv);
                            }
                        }
                        if (ld.Count > 0)
                        {
                            i++;
                        }

                    }

                }
            }
            return ldm;
        }

        public ATTIVAZIONETITOLIVIAGGIO CreaAttivazioneTV(decimal idTitoliViaggio, ModelDBISE db)
        {
            ATTIVAZIONETITOLIVIAGGIO new_atv = new ATTIVAZIONETITOLIVIAGGIO()
            {
                IDTITOLOVIAGGIO = idTitoliViaggio,
                ATTIVAZIONERICHIESTA = false,
                DATAATTIVAZIONERICHIESTA = null,
                NOTIFICARICHIESTA = false,
                DATANOTIFICARICHIESTA = null,
                ANNULLATO = false,
                DATAAGGIORNAMENTO = DateTime.Now,
            };
            db.ATTIVAZIONETITOLIVIAGGIO.Add(new_atv);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception(string.Format("Non è stato possibile creare una nuova attivazione per il titolo di viaggio."));
            }
            else
            {
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova attivazione titolo di viaggio.", "ATTIVAZIONETITOLIVIAGGIO", db, new_atv.IDTITOLOVIAGGIO, new_atv.IDATTIVAZIONETITOLIVIAGGIO);
            }

            return new_atv;
        }

        public void SetDocumentoTV(ref DocumentiModel dm, decimal idTitoliViaggio, ModelDBISE db, decimal idTipoDocumento)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                DOCUMENTI d = new DOCUMENTI();
                ATTIVAZIONETITOLIVIAGGIO atv = new ATTIVAZIONETITOLIVIAGGIO();

                dm.file.InputStream.CopyTo(ms);

                var tv = db.TITOLIVIAGGIO.Find(idTitoliViaggio);

                var latv =
                    tv.ATTIVAZIONETITOLIVIAGGIO.Where(
                        a => a.ANNULLATO == false && a.ATTIVAZIONERICHIESTA == false && a.NOTIFICARICHIESTA == false)
                        .OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO).ToList();
                if (latv?.Any() ?? false)
                {
                    atv = latv.First();
                }
                else
                {
                    atv = this.CreaAttivazioneTV(idTitoliViaggio, db);
                }

                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = idTipoDocumento;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();
                d.MODIFICATO = false;
                d.FK_IDDOCUMENTO = null;
                d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;

                atv.DOCUMENTI.Add(d);

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (titolo di viaggio).", "Documenti", db, tv.IDTITOLOVIAGGIO, dm.idDocumenti);
                }
                else
                {
                    throw new Exception("Errore nella fase di inserimento del documento (titolo di viaggio).");
                }

                // associa il titolo di viaggio all'attivazioneTitoloViaggio
                //this.AssociaDocumentoTitoloViaggio(atv.IDATTIVAZIONETITOLIVIAGGIO, dm.idDocumenti, db);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AssociaDocumentoTitoloViaggio(decimal idAttivazioneTitoloViaggio, decimal idDocumento, ModelDBISE db)
        {
            try
            {
                var atv = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoloViaggio);
                var item = db.Entry<ATTIVAZIONETITOLIVIAGGIO>(atv);
                item.State = EntityState.Modified;
                item.Collection(a => a.DOCUMENTI).Load();
                var d = db.DOCUMENTI.Find(idDocumento);
                atv.DOCUMENTI.Add(d);

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Impossibile associare il documento per l'attivazione titolo di viaggio.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AssociaRichiedenteTitoloViaggio(decimal idAttivazioneTitoloViaggio, decimal idTitoloViaggioRichiedente, ModelDBISE db)
        {
            try
            {
                var atv = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoloViaggio);
                var item = db.Entry<ATTIVAZIONETITOLIVIAGGIO>(atv);
                item.State = EntityState.Modified;
                item.Collection(a => a.TITOLIVIAGGIORICHIEDENTE).Load();
                var tvr = db.TITOLIVIAGGIORICHIEDENTE.Find(idTitoloViaggioRichiedente);
                atv.TITOLIVIAGGIORICHIEDENTE.Add(tvr);

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Impossibile associare il richiedente titolo di viaggio all'attivazione titolo di viaggio.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AssociaConiugeTitoloViaggio(decimal idAttivazioneTitoloViaggio, decimal idConiugeTitoloViaggio, ModelDBISE db)
        {
            try
            {
                var atv = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoloViaggio);
                var item = db.Entry<ATTIVAZIONETITOLIVIAGGIO>(atv);
                item.State = EntityState.Modified;
                item.Collection(a => a.CONIUGETITOLIVIAGGIO).Load();
                var ctv = db.CONIUGETITOLIVIAGGIO.Find(idConiugeTitoloViaggio);
                atv.CONIUGETITOLIVIAGGIO.Add(ctv);

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Impossibile associare il titolo di viaggio coniuge all'attivazione titolo di viaggio.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AssociaFigliTitoloViaggio(decimal idAttivazioneTitoloViaggio, decimal idFigliTitoloViaggio, ModelDBISE db)
        {
            try
            {
                var atv = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoloViaggio);
                var item = db.Entry<ATTIVAZIONETITOLIVIAGGIO>(atv);
                item.State = EntityState.Modified;
                item.Collection(a => a.FIGLITITOLIVIAGGIO).Load();
                var ftv = db.FIGLITITOLIVIAGGIO.Find(idFigliTitoloViaggio);
                atv.FIGLITITOLIVIAGGIO.Add(ftv);

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Impossibile associare il titolo di viaggio figlio all'attivazione titolo di viaggio.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void DeleteDocumentoTV(decimal idDocumento)
        {
            TITOLIVIAGGIO tv = new TITOLIVIAGGIO();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var d = db.DOCUMENTI.Find(idDocumento);

                    switch ((EnumTipoDoc)d.IDTIPODOCUMENTO)
                    {
                        case EnumTipoDoc.Carta_Imbarco:
                        case EnumTipoDoc.Titolo_Viaggio:
                        case EnumTipoDoc.Formulario_Titoli_Viaggio:
                            tv = d.ATTIVAZIONETITOLIVIAGGIO.OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO).First().TITOLIVIAGGIO;
                            break;
                        default:
                            tv = d.ATTIVAZIONETITOLIVIAGGIO.OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO).First().TITOLIVIAGGIO;
                            break;

                    }


                    if (d != null && d.IDDOCUMENTO > 0)
                    {
                        db.DOCUMENTI.Remove(d);

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception(string.Format("Non è stato possibile effettuare l'eliminazione del documento ({0}).", d.NOMEDOCUMENTO + d.ESTENSIONE));
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione di un documento (" + ((EnumTipoDoc)d.IDTIPODOCUMENTO).ToString() + ").", "Documenti", db, tv.IDTITOLOVIAGGIO, d.IDDOCUMENTO);
                        }
                    }
                }



            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void SituazioneTitoliViaggio(decimal idTitoliViaggio,
                        out bool richiediNotifica, out bool richiediAttivazione,
                        out bool richiediConiuge, out bool richiediRichiedente,
                        out bool richiediFigli, out bool DocTitoliViaggio,
                        out bool DocCartaImbarco, out bool inLavorazione, out bool trasfAnnullato)
        {
            richiediNotifica = false;
            richiediAttivazione = false;
            richiediConiuge = false;
            richiediRichiedente = false;
            richiediFigli = false;
            DocTitoliViaggio = false;
            DocCartaImbarco = false;
            inLavorazione = false;
            trasfAnnullato = false;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var tv = db.TITOLIVIAGGIO.Find(idTitoliViaggio);

                    var t = tv.TRASFERIMENTO;
                    var statoTrasferimeto = t.IDSTATOTRASFERIMENTO;
                    if (statoTrasferimeto == (decimal)EnumStatoTraferimento.Annullato)
                    {
                        trasfAnnullato = true;
                    }

                    //verifica se esiste una attivazione non notificata e non attivata 
                    var latv = tv.ATTIVAZIONETITOLIVIAGGIO
                                .Where(a => (a.ANNULLATO == false && a.ATTIVAZIONERICHIESTA == false && a.NOTIFICARICHIESTA == false))
                                .OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO).ToList();

                    if (latv?.Any() ?? false)
                    {
                        //se esiste verifica se ci sono elementi associati

                        //imposta l'ultima valida
                        var last_atv = latv.First();

                        //conta le attivazioni eseguite
                        var conta_attivazioni = tv.ATTIVAZIONETITOLIVIAGGIO
                                .Where(a => a.ANNULLATO == false).Count();

                        //documenti titoli viaggio
                        var ldtv = last_atv.DOCUMENTI.Where(a => (a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Titolo_Viaggio)).ToList();
                        if (ldtv?.Any() ?? false)
                        {
                            DocTitoliViaggio = true;
                            inLavorazione = true;
                        }

                        //documenti carta imbarco
                        var ldci = last_atv.DOCUMENTI.Where(a => (a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Carta_Imbarco)).ToList();
                        if (ldci?.Any() ?? false)
                        {
                            DocCartaImbarco = true;
                            inLavorazione = true;
                        }

                        //richiesta richiedente
                        var ltvr = last_atv.TITOLIVIAGGIORICHIEDENTE.Where(a => a.RICHIEDITITOLOVIAGGIO == true).ToList();
                        if (ltvr?.Any() ?? false)
                        {
                            if (conta_attivazioni == 1)
                            {
                                richiediRichiedente = true;
                                inLavorazione = true;
                            }
                        }

                        //richiesta coniuge
                        var ltvc = last_atv.CONIUGETITOLIVIAGGIO.Where(a => a.RICHIEDITITOLOVIAGGIO == true).ToList();
                        if (ltvc?.Any() ?? false)
                        {
                            if (conta_attivazioni == 1)
                            {
                                richiediConiuge = true;
                                inLavorazione = true;
                            }
                        }

                        //richiesta figli
                        var ltvf = last_atv.FIGLITITOLIVIAGGIO.Where(a => a.RICHIEDITITOLOVIAGGIO == true).ToList();
                        if (ltvf?.Any() ?? false)
                        {
                            if (conta_attivazioni == 1)
                            {
                                richiediFigli = true;
                                inLavorazione = true;
                            }
                        }

                        if (conta_attivazioni == 1)
                        {
                            if (richiediFigli || richiediConiuge || richiediRichiedente)
                            {
                                richiediNotifica = true;
                            }
                        }
                        else
                        {
                            if (DocCartaImbarco || DocTitoliViaggio)
                            {
                                richiediNotifica = true;
                            }
                        }
                        if (last_atv.NOTIFICARICHIESTA == true && last_atv.ANNULLATO == false)
                        {
                            richiediAttivazione = true;
                            richiediNotifica = false;
                        }

                    }
                    //in ogni caso verifica se esiste una attivazione da attivare
                    latv = tv.ATTIVAZIONETITOLIVIAGGIO.Where(a => (a.ANNULLATO == false && a.ATTIVAZIONERICHIESTA == false && a.NOTIFICARICHIESTA == true)).OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO).ToList();
                    if (latv?.Any() ?? false)
                    {
                        richiediAttivazione = true;
                        richiediNotifica = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool AttivazioneTitoliViaggioInLavorazione(decimal IdAttivazioneTitoliViaggio, decimal idTitoliViaggio)
        {
            bool inLavorazione = false;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var atv = db.ATTIVAZIONETITOLIVIAGGIO.Find(IdAttivazioneTitoliViaggio);

                    if (atv.ANNULLATO == false && atv.ATTIVAZIONERICHIESTA == false && atv.NOTIFICARICHIESTA == false && atv.TITOLIVIAGGIO.TRASFERIMENTO.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato)
                    {
                        //verifica se ci sono elementi associati

                        //conta le attivazioni eseguite
                        var conta_attivazioni = db.TITOLIVIAGGIO.Find(idTitoliViaggio).ATTIVAZIONETITOLIVIAGGIO
                        .Where(a => a.ANNULLATO == false).Count();

                        //documenti titoli viaggio
                        var ldtv = atv.DOCUMENTI.Where(a => (a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Titolo_Viaggio)).ToList();
                        if (ldtv?.Any() ?? false)
                        {
                            inLavorazione = true;
                        }

                        //documenti carta imbarco
                        var ldci = atv.DOCUMENTI.Where(a => (a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Carta_Imbarco)).ToList();
                        if (ldci?.Any() ?? false)
                        {
                            inLavorazione = true;
                        }

                        //richiesta richiedente
                        var ltvr = atv.TITOLIVIAGGIORICHIEDENTE.Where(a => a.RICHIEDITITOLOVIAGGIO == true).ToList();
                        if (ltvr?.Any() ?? false)
                        {
                            if (conta_attivazioni == 1)
                            {
                                inLavorazione = true;
                            }
                        }

                        //richiesta coniuge
                        var ltvc = atv.CONIUGETITOLIVIAGGIO.Where(a => a.RICHIEDITITOLOVIAGGIO == true).ToList();
                        if (ltvc?.Any() ?? false)
                        {
                            if (conta_attivazioni == 1)
                            {
                                inLavorazione = true;
                            }
                        }

                        //richiesta figli
                        var ltvf = atv.FIGLITITOLIVIAGGIO.Where(a => a.RICHIEDITITOLOVIAGGIO == true).ToList();
                        if (ltvc?.Any() ?? false)
                        {
                            if (conta_attivazioni == 1)
                            {
                                inLavorazione = true;
                            }
                        }
                    }
                }
                return inLavorazione;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AnnullaRichiestaTitoliViaggio(decimal idAttivazioneTitoliViaggio, string testoAnnulla)
        {

            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var atv_Old = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoliViaggio);

                    if (atv_Old?.IDATTIVAZIONETITOLIVIAGGIO > 0)
                    {
                        if (atv_Old.NOTIFICARICHIESTA == true && atv_Old.ATTIVAZIONERICHIESTA == false && atv_Old.ANNULLATO == false)
                        {
                            atv_Old.ANNULLATO = true;
                            atv_Old.DATAAGGIORNAMENTO = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore - Impossibile annullare la notifica della richiesta titoli di viaggio.");
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Annullamento della riga per il ciclo di attivazione dei titoli di viaggio",
                                    "ATTIVAZIONITITOLIVIAGGIO", db, atv_Old.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO,
                                    atv_Old.IDATTIVAZIONETITOLIVIAGGIO);

                                var idTrasferimento = atv_Old.IDTITOLOVIAGGIO;

                                ATTIVAZIONETITOLIVIAGGIO atv_New = new ATTIVAZIONETITOLIVIAGGIO()
                                {
                                    IDTITOLOVIAGGIO = atv_Old.IDTITOLOVIAGGIO,
                                    NOTIFICARICHIESTA = false,
                                    ATTIVAZIONERICHIESTA = false,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    ANNULLATO = false
                                };

                                db.ATTIVAZIONETITOLIVIAGGIO.Add(atv_New);

                                int j = db.SaveChanges();

                                if (j <= 0)
                                {
                                    throw new Exception("Errore - Impossibile creare il nuovo ciclo di attivazione per i titoli di viaggio.");
                                }
                                else
                                {

                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                        "Inserimento di una nuova riga per il ciclo di attivazione relativo ai titoli di viaggio.",
                                        "ATTIVAZIONITITOLIVIAGGIO", db, atv_New.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO,
                                        atv_New.IDATTIVAZIONETITOLIVIAGGIO);

                                    #region Richiedente

                                    var ltvr_Old =
                                    atv_Old.TITOLIVIAGGIORICHIEDENTE.Where(a => a.ANNULLATO == false)
                                        .OrderByDescending(a => a.IDTITOLIVIAGGIORICHIEDENTE);

                                    if (ltvr_Old?.Any() ?? false)
                                    {
                                        var tvr_Old = ltvr_Old.First();

                                        TITOLIVIAGGIORICHIEDENTE tvr_New = new TITOLIVIAGGIORICHIEDENTE()
                                        {
                                            IDTITOLOVIAGGIO = tvr_Old.IDTITOLOVIAGGIO,
                                            IDATTIVAZIONETITOLIVIAGGIO = atv_New.IDATTIVAZIONETITOLIVIAGGIO,
                                            RICHIEDITITOLOVIAGGIO = tvr_Old.RICHIEDITITOLOVIAGGIO,
                                            DATAAGGIORNAMENTO = tvr_Old.DATAAGGIORNAMENTO,
                                            ANNULLATO = tvr_Old.ANNULLATO
                                        };

                                        //db.TITOLIVIAGGIORICHIEDENTE.Add(tvr_New);
                                        atv_New.TITOLIVIAGGIORICHIEDENTE.Add(tvr_New);
                                        tvr_Old.ANNULLATO = true;
                                        int k = db.SaveChanges();

                                        if (k <= 0)
                                        {
                                            throw new Exception("Errore - Impossibile inserire i dati del richiedente per il nuovo ciclo di attivazione creato dall'annulla richiesta titoli di viaggio.");
                                        }
                                        else
                                        {
                                            //    this.AssociaRichiedenteTitoloViaggio(atv_New.IDATTIVAZIONETITOLIVIAGGIO, tvr_New.IDTITOLIVIAGGIORICHIEDENTE,db);

                                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                            "Inserimento di una nuova riga per il richiedente relativo ai titoli di viaggio.",
                                            "TITOLIVIAGGIORICHIEDENTE", db,
                                            tvr_New.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO,
                                            tvr_New.IDTITOLIVIAGGIORICHIEDENTE);

                                        }

                                    }
                                    #endregion

                                    #region Coniuge

                                    var lctv_Old =
                                        atv_Old.CONIUGETITOLIVIAGGIO.Where(a => a.ANNULLATO == false)
                                            .OrderBy(a => a.IDCONIUGETITOLIVIAGGIO);
                                    if (lctv_Old?.Any() ?? false)
                                    {
                                        foreach (var ctv_Old in lctv_Old)
                                        {
                                            CONIUGETITOLIVIAGGIO ctv_New = new CONIUGETITOLIVIAGGIO()
                                            {
                                                IDTITOLOVIAGGIO = ctv_Old.IDTITOLOVIAGGIO,
                                                IDATTIVAZIONETITOLIVIAGGIO = atv_New.IDATTIVAZIONETITOLIVIAGGIO,
                                                RICHIEDITITOLOVIAGGIO = ctv_Old.RICHIEDITITOLOVIAGGIO,
                                                DATAAGGIORNAMENTO = ctv_Old.DATAAGGIORNAMENTO,
                                                ANNULLATO = ctv_Old.ANNULLATO
                                            };

                                            //db.CONIUGETITOLIVIAGGIO.Add(ctv_New);

                                            atv_New.CONIUGETITOLIVIAGGIO.Add(ctv_New);
                                            ctv_Old.ANNULLATO = true;

                                            int x = db.SaveChanges();

                                            if (x <= 0)
                                            {
                                                throw new Exception("Errore - Impossibile inserire il coniuge per il titolo di viaggio da annullamento richiesta.");
                                            }
                                            else
                                            {
                                                //this.AssociaConiugeTitoloViaggio(atv_New.IDATTIVAZIONETITOLIVIAGGIO, ctv_New.IDCONIUGETITOLIVIAGGIO, db);

                                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                                "Inserimento di una nuova riga per il titolo di viaggio coniuge, relativa al titolo di viaggio.",
                                                                "CONIUGETITOLIVIAGGIO", db,
                                                                atv_New.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO,
                                                                ctv_New.IDCONIUGETITOLIVIAGGIO);
                                            }
                                        }


                                    }

                                    #endregion

                                    #region figli
                                    var lftv_Old =
                                        atv_Old.FIGLITITOLIVIAGGIO.Where(a => a.ANNULLATO == false)
                                            .OrderBy(a => a.IDFIGLITITOLIVIAGGIO);

                                    if (lftv_Old?.Any() ?? false)
                                    {
                                        foreach (var ftv_Old in lftv_Old)
                                        {
                                            FIGLITITOLIVIAGGIO ftv_New = new FIGLITITOLIVIAGGIO()
                                            {
                                                IDTITOLOVIAGGIO = ftv_Old.IDTITOLOVIAGGIO,
                                                IDATTIVAZIONETITOLIVIAGGIO = atv_New.IDATTIVAZIONETITOLIVIAGGIO,
                                                RICHIEDITITOLOVIAGGIO = ftv_Old.RICHIEDITITOLOVIAGGIO,
                                                DATAAGGIORNAMENTO = ftv_Old.DATAAGGIORNAMENTO,
                                                ANNULLATO = ftv_Old.ANNULLATO
                                            };

                                            //db.FIGLITITOLIVIAGGIO.Add(ftv_New);
                                            atv_New.FIGLITITOLIVIAGGIO.Add(ftv_New);
                                            ftv_Old.ANNULLATO = true;

                                            int z = db.SaveChanges();

                                            if (z <= 0)
                                            {
                                                throw new Exception("Errore - Impossibile inserire i figli per il titolo di viaggio da annullamento richiesta.");
                                            }
                                            else
                                            {
                                                //this.AssociaFigliTitoloViaggio(atv_New.IDATTIVAZIONETITOLIVIAGGIO, ftv_New.IDFIGLITITOLIVIAGGIO, db);

                                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                                "Inserimento di una nuova riga per il figlio del richiedente relativo al titolo di viaggio.",
                                                                "FIGLITITOLIVIAGGIO", db,
                                                                atv_New.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO,
                                                                ftv_New.IDFIGLITITOLIVIAGGIO);
                                            }
                                        }


                                    }
                                    #endregion

                                    #region documenti
                                    var ldoc_Old =
                                        atv_Old.DOCUMENTI.Where(
                                            a => a.MODIFICATO == false &&
                                            (a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Carta_Imbarco || a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Titolo_Viaggio))
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

                                            //db.DOCUMENTI.Add(doc_New);
                                            atv_New.DOCUMENTI.Add(doc_New);

                                            int y = db.SaveChanges();

                                            if (y <= 0)
                                            {
                                                throw new Exception("Errore - Impossibile associare il documento per il titolo viaggio. (" + doc_New.NOMEDOCUMENTO + ")");
                                            }
                                            else
                                            {
                                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                    "Inserimento di una nuova riga per il documento relativo al trasporto effetti in partenza.",
                                                    "DOCUMENTI", db,
                                                    atv_New.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO,
                                                    doc_New.IDDOCUMENTO);
                                            }

                                        }


                                    }
                                    #endregion

                                    var conta_attivazioni = this.GetNumAttivazioniTV(atv_New.IDTITOLOVIAGGIO, db);
                                    string oggettoAnnulla = "";

                                    if (conta_attivazioni == 1)
                                    {
                                        oggettoAnnulla = Resources.msgEmail.OggettoAnnullaRichiestaInizialeTitoliViaggio;
                                    }
                                    else
                                    {
                                        oggettoAnnulla = Resources.msgEmail.OggettoAnnullaRichiestaSuccessivaTitioliViaggio;
                                    }

                                    EmailTrasferimento.EmailAnnulla(idTrasferimento,
                                                                    oggettoAnnulla,
                                                                    testoAnnulla,
                                                                    db);

                                    //this.EmailAnnullaRichiestaTitoliViaggio(atv_New.IDATTIVAZIONETITOLIVIAGGIO, db);
                                    using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                    {
                                        dtce.AnnullaMessaggioEvento(idTrasferimento, EnumFunzioniEventi.RichiestaTitoliViaggio, db);
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
        public void EmailAnnullaRichiestaTitoliViaggio(decimal idAttivazioneTitoliViaggio, ModelDBISE db)
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

                var atv = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoliViaggio);

                var conta_attivazioni = this.GetNumAttivazioniTV(atv.IDTITOLOVIAGGIO, db);

                if (atv?.IDATTIVAZIONETITOLIVIAGGIO > 0)
                {
                    TRASFERIMENTO tr = atv.TITOLIVIAGGIO.TRASFERIMENTO;
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

                            if (conta_attivazioni == 1)
                            {
                                msgMail.oggetto =
                                Resources.msgEmail.OggettoAnnullaRichiestaInizialeTitoliViaggio;
                                msgMail.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaInizialeTitoloViaggio, uff.DESCRIZIONEUFFICIO + " (" + uff.CODICEUFFICIO + ")", tr.DATAPARTENZA.ToLongDateString());
                            }
                            else
                            {
                                msgMail.oggetto =
                                Resources.msgEmail.OggettoAnnullaRichiestaSuccessivaTitioliViaggio;
                                msgMail.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaSuccessivaTitoloViaggio, uff.DESCRIZIONEUFFICIO + " (" + uff.CODICEUFFICIO + ")", tr.DATAPARTENZA.ToLongDateString());
                            }
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

        public void SetPreventiviViaggiCongedio(ref DocumentiModel dm, ModelDBISE db, decimal idTrasferimento)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                DOCUMENTI d = new DOCUMENTI();
                dm.file.InputStream.CopyTo(ms);

                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();
                d.FK_IDDOCUMENTO = null;
                d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;
                d.MODIFICATO = false;

                db.DOCUMENTI.Add(d);

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (" + dm.tipoDocumento.ToString() + ").", "Documenti", db, idTrasferimento, dm.idDocumenti);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //
        public void SetDocumentiFase2ViaggiCongedio(ref DocumentiModel dm, ModelDBISE db, decimal idAttivViaggioCongedo)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                DOCUMENTI d = new DOCUMENTI();
                dm.file.InputStream.CopyTo(ms);

                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();
                d.FK_IDDOCUMENTO = null;
                d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;
                d.MODIFICATO = false;

                var a = db.ATTIVAZIONIVIAGGICONGEDO.Find(idAttivViaggioCongedo);
                a.DOCUMENTI.Add(d);
                //  db.DOCUMENTI.Add(d);

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (" + dm.tipoDocumento.ToString() + ").", "Documenti", db, a.VIAGGICONGEDO.IDTRASFERIMENTO, dm.idDocumenti);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void EliminaDocumentoPreventivo(decimal idPreventivoDocumento)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var doc = db.DOCUMENTI.Find(idPreventivoDocumento);
                    db.DOCUMENTI.Remove(doc);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public byte[] GetAllegatoVC(decimal idAttivazioneVC, decimal idDocumento)
        {
            byte[] tmp = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                // var attvc = db.ATTIVAZIONIVIAGGICONGEDO.Find(idAttivazioneVC);
                var doc = db.DOCUMENTI.Find(idDocumento);
                tmp = doc.FILEDOCUMENTO;
            }
            return tmp;
        }
        public string GetEmailByIdDipendente(decimal idDipendente)
        {
            string email = "";
            using (ModelDBISE db = new ModelDBISE())
            {
                DIPENDENTI d = db.DIPENDENTI.Find(idDipendente);
                email = d.EMAIL;
            }
            return email;
        }
        public DocumentiModel GetDatiDocumentoById(decimal idDocumento)
        {
            DocumentiModel dm = new DocumentiModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                var d = db.DOCUMENTI.Find(idDocumento);

                if (d != null && d.IDDOCUMENTO > 0)
                {

                    dm = new DocumentiModel()
                    {
                        idDocumenti = d.IDDOCUMENTO,
                        nomeDocumento = d.NOMEDOCUMENTO,
                        estensione = d.ESTENSIONE,
                        tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
                        dataInserimento = d.DATAINSERIMENTO,
                        //file = f
                    };
                }
            }
            return dm;
        }
        public DipendentiModel RestituisciDipendenteByID(decimal idDipendente)
        {
            DipendentiModel dm = new DipendentiModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                DIPENDENTI d = db.DIPENDENTI.Find(idDipendente);
                dm.idDipendente = d.IDDIPENDENTE;
                dm.nome = d.NOME; dm.cognome = d.COGNOME;
                dm.email = d.EMAIL; d.INDIRIZZO = d.INDIRIZZO;
            }
            return dm;
        }
        public decimal RestituisciIDdestinatarioDaEmail(string email)
        {
            decimal idDipendente = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    idDipendente = (from e in db.DIPENDENTI
                                    where e.EMAIL.ToUpper() == email.ToUpper() && e.ABILITATO == true
                                    select new DipendentiModel()
                                    {
                                        idDipendente = e.IDDIPENDENTE,
                                    }).ToList().First().idDipendente;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return idDipendente;
        }
        public DipendentiModel GetMittente(decimal idDipendente)
        {
            DipendentiModel dip = new DipendentiModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                DIPENDENTI dd = db.DIPENDENTI.Find();
                dip.idDipendente = dd.IDDIPENDENTE;
                dip.nome = dd.NOME;
                dip.cognome = dd.COGNOME;
                dip.email = dd.EMAIL;
                dip.matricola = dd.MATRICOLA;
            }
            return dip;
        }
        public List<DipendentiModel> GetListaDipendentiAutorizzati(decimal idRuoloUtente)
        {
            List<DipendentiModel> ldes = new List<DipendentiModel>();
            List<DipendentiModel> ldesdef = new List<DipendentiModel>();
            List<UtentiAutorizzatiModel> uaut = new List<UtentiAutorizzatiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                uaut = (from d in db.UTENTIAUTORIZZATI
                        where d.IDRUOLOUTENTE == idRuoloUtente && d.IDDIPENDENTE > 0
                        select new UtentiAutorizzatiModel()
                        {
                            idDipendente = (decimal)d.IDDIPENDENTE,
                            idRouloUtente = idRuoloUtente,
                            Utente = d.UTENTE,
                            psw = d.PSW
                        }).ToList();

                foreach (var ut in uaut)
                {
                    DipendentiModel dm = new DipendentiModel();
                    if (idRuoloUtente == (Decimal)EnumRuoloAccesso.Utente)
                    {
                        ldes = (from t in db.TRASFERIMENTO
                                where t.IDDIPENDENTE == ut.idDipendente
                                && (t.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo || t.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato)
                                select new DipendentiModel()
                                {
                                    matricola = t.DIPENDENTI.MATRICOLA,
                                    nome = t.DIPENDENTI.NOME == null ? "" : t.DIPENDENTI.NOME,
                                    cognome = t.DIPENDENTI.COGNOME == null ? "" : t.DIPENDENTI.COGNOME,
                                    email = t.DIPENDENTI.EMAIL == null ? "" : t.DIPENDENTI.EMAIL,
                                    idDipendente = t.IDDIPENDENTE,
                                }).ToList();
                    }
                    else if (idRuoloUtente == (Decimal)EnumRuoloAccesso.Amministratore)
                    {
                        ldes = (from t in db.DIPENDENTI
                                where t.IDDIPENDENTE == ut.idDipendente
                                select new DipendentiModel()
                                {
                                    matricola = t.MATRICOLA,
                                    nome = t.NOME == null ? "" : t.NOME,
                                    cognome = t.COGNOME == null ? "" : t.COGNOME,
                                    email = t.EMAIL == null ? "" : t.EMAIL,
                                    idDipendente = t.IDDIPENDENTE
                                }).ToList();
                    }
                    if (ldes.Count != 0)
                    {
                        dm = ldes.First();
                        ldesdef.Add(dm);
                    }
                }
            }
            return ldesdef;
        }
    }
}

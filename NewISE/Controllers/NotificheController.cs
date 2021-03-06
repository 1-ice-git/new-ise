﻿using NewISE.Models.Config;
using NewISE.Models.Config.s_admin;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using System.ComponentModel;
using System.IO;
using NewISE.EF;
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models;
using NewISE.Models.Enumeratori;

namespace NewISE.Controllers
{
    public class NotificheController : Controller
    {
        // GET: Notifiche
        bool admin = false;
        public ActionResult Index()
        {
            try
            {
                admin = Utility.Amministratore();
                ViewBag.Amministratore = admin;
            }
            catch (Exception)
            {
                return View("Error");
            }
            return View();
            //var model = new NotificheModel();
            //return View(model);
        }
        public ActionResult ListaNotifiche()
        {
            decimal idMittenteLogato = Utility.UtenteAutorizzato().idDipendente;
            List<NotificheModel> nm = new List<NotificheModel>();
            using (dtNotifiche dn = new dtNotifiche())
            {
                nm = dn.GetNotifiche(idMittenteLogato).OrderByDescending(x => x.dataNotifica).ToList();
            }
            return PartialView(nm);
        }
        public ActionResult NotificheRicevute()
        {
            decimal idMittenteLogato = Utility.UtenteAutorizzato().idDipendente;
            List<NotificheModel> nm = new List<NotificheModel>();
            using (dtNotifiche dn = new dtNotifiche())
            {
                nm = dn.GetRicevuteNotifiche(idMittenteLogato).OrderByDescending(x => x.dataNotifica).ToList();
            }
            return PartialView(nm);
        }
        public ActionResult NuovaNotifica()
        {
            var r = new List<SelectListItem>();
            var r0 = new List<SelectListItem>();
            var r2 = new List<SelectListItem>();
            UtentiAutorizzatiModel uta = null;
            List<DipendentiModel> dm = new List<DipendentiModel>();
            decimal idMittenteLogato = Utility.UtenteAutorizzato().idDipendente;

            try
            {
                using (dtNotifiche dtn = new dtNotifiche())
                {
                    uta = dtn.RestituisciAutorizzato(idMittenteLogato);

                    if (uta.idRouloUtente == (decimal)EnumRuoloAccesso.Amministratore)
                    {
                        dm.AddRange(dtn.GetListaDipendentiAutorizzati((decimal)EnumRuoloAccesso.Utente));
                    }
                    if (uta.idRouloUtente == (decimal)EnumRuoloAccesso.Utente)
                    {
                        dm.AddRange(dtn.GetListaDipendentiAutorizzati((decimal)EnumRuoloAccesso.Amministratore));
                    }
                    //dm.AddRange(dtn.GetListaDipendentiAutorizzati());
                    if (dm.Count > 0)
                    {
                        var agg = new SelectListItem(); agg.Text = "TUTTI (Trasferimenti attivi)"; agg.Value = "TUTTI";
                        r.Add(agg);
                        r0 = (from t in dm
                              where !string.IsNullOrEmpty(t.email) && t.email.Trim() != ""
                              orderby t.cognome
                              select new SelectListItem()
                              {
                                  Text = t.cognome + " " + t.nome + " ("+t.matricola +")",
                                  //    Value = t.email
                                  Value = t.idDipendente.ToString()
                              }).ToList();
                        r.AddRange(r0);

                        r2 = (from t in dm
                              where !string.IsNullOrEmpty(t.email) && t.email.Trim() != ""
                              orderby t.cognome
                              select new SelectListItem()
                              {
                                  Text = t.cognome + " " + t.nome + " (" + t.matricola + ")",
                                  //  Value = t.email
                                  Value = t.idDipendente.ToString()
                              }).ToList();
                    }
                }
                ViewBag.idMittenteLogato = idMittenteLogato;
                ViewBag.ListaDestinatari = r;
                ViewBag.ListaCc = r2;
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        public ActionResult VisualizzaCorpoMessaggio(decimal idNotifica, string idUtenteLoggato)
        {
            if (string.IsNullOrEmpty(idUtenteLoggato)) idUtenteLoggato = "0";
            decimal idMittenteLogato = Utility.UtenteAutorizzato().idDipendente;
            List<NotificheModel> nm = new List<NotificheModel>();
            NotificheModel elem = new NotificheModel();
            try
            {
                using (dtNotifiche dn = new dtNotifiche())
                {
                    if (idUtenteLoggato != "1")
                        nm = dn.GetNotifiche(idMittenteLogato).Where(a => a.idNotifica == idNotifica).ToList();
                    else
                        nm = dn.GetNotifiche(idMittenteLogato, idNotifica).ToList();
                    if (nm.Count() > 0)
                    {
                        elem = nm.First();
                    }
                }
                return PartialView(elem);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        //public ActionResult InserisciNuovaNotifica(NotificheModel nmod)
        //public ActionResult InserisciNuovaNotifica(HttpPostedFileBase PDFUpload, NotificheModel nmod)
        public ActionResult InserisciNuovaNotifica(string listaMailPrincipale, string listaMailToCc, string Oggetto, string CorpoMessaggio, HttpPostedFileBase file)
        {
            bool InserimentoEffettuatoinDB = false;
            string nomefile = "";
            string[] VettNomeFile = null;
            string nomefileFin = "";
            string estensione = "";
            if (file != null)
            {
                nomefile = file.FileName;
                VettNomeFile = nomefile.Split('\\');
                nomefile = VettNomeFile[VettNomeFile.Length - 1];
                nomefileFin = nomefile.Split('.')[0];
                estensione = nomefile.Split('.')[1];
            }
            AggiornaLista();
            decimal idMittenteLogato = Utility.UtenteAutorizzato().idDipendente;
            ViewBag.idMittenteLogato = idMittenteLogato;
            UtentiAutorizzatiModel uta = null;
            NotificheModel nmod = new NotificheModel();
            //if (nmod.PDFUpload == null || nmod.PDFUpload.ContentLength == 0)
            //{            
            //    ModelState.AddModelError("PDFUpload", "Questo campo è richiesto");
            //}
            List<NotificheModel> lnm = new List<NotificheModel>();
            using (ModelDBISE db = new ModelDBISE())
            {
                using (dtNotifiche dtn = new dtNotifiche())
                {
                    try
                    {
                        db.Database.BeginTransaction();
                        try
                        {

                            //  db.Database.BeginTransaction();
                            // HttpPostedFileBase file = Request.Files[item] as HttpPostedFileBase;
                            bool esisteFile = false;
                            bool gestisceEstensioni = false;
                            bool dimensioneConsentita = false;
                            string dimensioneMaxConsentita = string.Empty;

                            PreSetDocumentoNotifiche(file, out nmod, out esisteFile, out gestisceEstensioni,
                                out dimensioneConsentita, out dimensioneMaxConsentita);

                            nmod.dataNotifica = DateTime.Now;
                            nmod.idMittente = idMittenteLogato;
                            nmod.lDestinatari = listaMailPrincipale.Split(',');
                            nmod.toCc = listaMailToCc.Split(',');
                            nmod.corpoMessaggio = CorpoMessaggio;
                            nmod.Oggetto = Oggetto;
                            nmod.NomeFile = nomefileFin;
                            nmod.Estensione = estensione;
                            if (esisteFile)
                            {
                                if (gestisceEstensioni == false)
                                {
                                    throw new Exception(
                                    "Il documento selezionato non è nel formato consentito. Il formato supportato è: pdf.");
                                }
                                if (!dimensioneConsentita)
                                {
                                    throw new Exception(
                                        "Il documento selezionato supera la dimensione massima consentita (" +
                                        dimensioneMaxConsentita + " Mb).");
                                }
                            }
                            bool tutti = false;

                            uta = dtn.RestituisciAutorizzato(idMittenteLogato);
                            InserimentoEffettuatoinDB = dtn.InsertNotifiche(nmod, db, out tutti);

                            //db.Database.CurrentTransaction.Commit();

                            idMittenteLogato = nmod.idMittente;// Utility.UtenteAutorizzato().idDipendente;

                            #region invia email se tutto ok
                            if (InserimentoEffettuatoinDB)
                            {
                                using (GestioneEmail gmail = new GestioneEmail())
                                {
                                    ModelloAllegatoMail allegato = new ModelloAllegatoMail();
                                    Destinatario dest = new Destinatario();
                                    Destinatario destToCc = new Destinatario();
                                    ModelloMsgMail modMSGmail = new ModelloMsgMail();

                                    if (nmod.Allegato != null)
                                    {
                                        var docByte = dtn.GetDocumentoByteById(nmod.idNotifica, db);
                                        Stream streamDoc = new MemoryStream(docByte);
                                        allegato.nomeFile = nomefileFin + "." + estensione;//DateTime.Now.Ticks.ToString() + ".pdf";
                                        allegato.allegato = streamDoc;
                                        modMSGmail.allegato.Add(allegato);
                                    }
                                    modMSGmail.oggetto = nmod.Oggetto;
                                    modMSGmail.corpoMsg = nmod.corpoMessaggio;
                                    Mittente mitt = new Mittente();
                                    mitt.EmailMittente = dtn.GetEmailByIdDipendente(idMittenteLogato);
                                    decimal id_dip = dtn.RestituisciIDdestinatarioDaEmail(mitt.EmailMittente);
                                    DipendentiModel dmod = dtn.RestituisciDipendenteByID(id_dip);
                                    mitt.Nominativo = dmod.nome + " " + dmod.cognome;
                                    var ddss = dtn.GetListDestinatari(nmod.idNotifica, db);

                                    #region controllo tutti=false
                                    if (tutti == false)
                                    {
                                        foreach (var x in ddss)
                                        {
                                            string nome_ = dtn.RestituisciDipendenteByID(x.idDipendente).nome;
                                            string cognome_ = dtn.RestituisciDipendenteByID(x.idDipendente).cognome;
                                            string nominativo_ = nome_ + " " + cognome_;
                                            using (dtUtenzeDipendenti dtud = new dtUtenzeDipendenti())
                                            {
                                                var les = dtud.GetListaEmailSecondarioDip(x.idDipendente);

                                                if (!x.ToCc)
                                                {
                                                    dest = new Destinatario();
                                                    dest.EmailDestinatario = dtn.GetEmailByIdDipendente(x.idDipendente);
                                                    dest.Nominativo = nominativo_;
                                                    modMSGmail.destinatario.Add(dest);
                                                    #region inserisce eventuali email secondarie
                                                    if (les?.Any()??false)
                                                    {
                                                        foreach (var es in les)
                                                        {
                                                            dest = new Destinatario();
                                                            dest.EmailDestinatario = es.Email;
                                                            dest.Nominativo = nominativo_;
                                                            modMSGmail.destinatario.Add(dest);
                                                        }
                                                    }
                                                    #endregion
                                                }
                                                else
                                                {
                                                    destToCc = new Destinatario();
                                                    destToCc.EmailDestinatario = dtn.GetEmailByIdDipendente(x.idDipendente);
                                                    destToCc.Nominativo = nominativo_;
                                                    modMSGmail.cc.Add(destToCc);
                                                    #region inserisce eventuali email secondarie
                                                    if (les?.Any() ?? false)
                                                    {
                                                        foreach (var es in les)
                                                        {
                                                            destToCc = new Destinatario();
                                                            destToCc.EmailDestinatario = es.Email;
                                                            destToCc.Nominativo = nominativo_;
                                                            modMSGmail.cc.Add(destToCc);
                                                        }
                                                    }
                                                    #endregion
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region controllo tutti=true
                                    if (tutti == true)
                                    {
                                        List<DipendentiModel> listatutti = null;
                                        if (uta.idRouloUtente == (decimal)EnumRuoloAccesso.Amministratore)
                                            listatutti = dtn.TuttiListaDestinatari((decimal)EnumRuoloAccesso.Utente);

                                        if (uta.idRouloUtente == (decimal)EnumRuoloAccesso.Utente)
                                            listatutti = dtn.TuttiListaDestinatari((decimal)EnumRuoloAccesso.Amministratore);

                                        foreach (var elem in listatutti)
                                        {
                                            dest = new Destinatario();
                                            dest.EmailDestinatario = elem.email;
                                            dest.Nominativo = elem.cognome + " " + elem.nome + " (" + elem.matricola + ")";
                                            modMSGmail.destinatario.Add(dest);
                                            using (dtUtenzeDipendenti dtud = new dtUtenzeDipendenti())
                                            {
                                                var les = dtud.GetListaEmailSecondarioDip(elem.idDipendente);
                                                #region inserisce eventuali email secondarie
                                                if (les?.Any() ?? false)
                                                {
                                                    foreach (var es in les)
                                                    {
                                                        dest = new Destinatario();
                                                        dest.EmailDestinatario = es.Email;
                                                        dest.Nominativo = elem.cognome + " " + elem.nome + " (" + elem.matricola + ")";
                                                        modMSGmail.destinatario.Add(destToCc);
                                                    }
                                                }
                                                #endregion
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Qui mi assicuro che tutti gli amministratori siano inclusi in ToCc
                                    if (tutti == false || (tutti == true && uta.idRouloUtente == (decimal)EnumRuoloAccesso.Utente))
                                    {
                                        var lls = dtn.GetListaDipendentiAutorizzati((decimal)EnumRuoloAccesso.Amministratore);
                                        foreach (var x in lls)
                                        {
                                            bool found = false;
                                            if (modMSGmail.cc.Count != 0)
                                            {
                                                var tmp = modMSGmail.cc.Where(a => a.EmailDestinatario.ToUpper().Trim() == x.email.ToUpper().Trim()).ToList();
                                                if (tmp.Count() != 0)
                                                {
                                                    found = true;
                                                }                                               
                                            }
                                            if (found == false)
                                            {
                                                destToCc = new Destinatario();
                                                string nome_cc = x.nome;
                                                string cognome_cc = x.cognome;
                                                destToCc.EmailDestinatario = x.email;
                                                string nominativo_cc = nome_cc + " " + cognome_cc;
                                                destToCc.Nominativo = nominativo_cc;
                                                modMSGmail.cc.Add(destToCc);
                                                using (dtUtenzeDipendenti dtud = new dtUtenzeDipendenti())
                                                {
                                                    var les = dtud.GetListaEmailSecondarioDip(x.idDipendente);
                                                    #region inserisce eventuali email secondarie
                                                    if (les?.Any() ?? false)
                                                    {
                                                        foreach (var es in les)
                                                        {
                                                            destToCc = new Destinatario();
                                                            destToCc.EmailDestinatario = es.Email;
                                                            destToCc.Nominativo = x.cognome + " " + x.nome;
                                                            modMSGmail.cc.Add(destToCc);
                                                        }
                                                    }
                                                    #endregion
                                                }

                                            }
                                        }
                                    }
                                    #endregion

                                    db.Database.CurrentTransaction.Commit();

                                    lnm = dtn.GetNotifiche(idMittenteLogato).ToList();

                                    modMSGmail.mittente = mitt;
                                    gmail.sendMail(modMSGmail);
                                }
                            }
                            #endregion

                        }
                        catch (Exception ex)
                        {
                            db.Database.CurrentTransaction.Rollback();
                            return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
                        }

                        return PartialView("ListaNotifiche", lnm);
                    }
                    catch (Exception ex)
                    {
                        return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
                    }
                }
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RegistraNotifiche(FormCollection CollectionMessaggio)
        {
            try
            {
                FormCollection fc = new FormCollection(Request.Unvalidated().Form);
                string testo = fc["contenutoMessaggio"];
                return PartialView("ListaNotifiche");
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        public ActionResult VisualizzaDestinatari(decimal idNotifica)
        {
            List<DestinatarioModel> nm = new List<DestinatarioModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtNotifiche dn = new dtNotifiche())
                    {
                        nm = dn.GetListDestinatari(idNotifica, db).ToList();
                    }
                }
                return PartialView(nm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        public ActionResult VisualizzaMittente(decimal idNotifica)
        {
            List<DipendentiModel> nm = new List<DipendentiModel>();
            try
            {
                using (dtNotifiche dn = new dtNotifiche())
                {
                    nm = dn.GetMittente(idNotifica).ToList();
                }
                return PartialView(nm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        void AggiornaLista()
        {
            var r = new List<SelectListItem>();
            var r0 = new List<SelectListItem>();
            var r2 = new List<SelectListItem>();
            UtentiAutorizzatiModel uta = null;
            List<DipendentiModel> dm = new List<DipendentiModel>();
            decimal idMittenteLogato = Utility.UtenteAutorizzato().idDipendente;
            using (dtNotifiche dtn = new dtNotifiche())
            {
                uta = dtn.RestituisciAutorizzato(idMittenteLogato);
                if (uta.idRouloUtente == (decimal)EnumRuoloAccesso.Amministratore)
                {
                    dm.AddRange(dtn.GetListaDipendentiAutorizzati((decimal)EnumRuoloAccesso.Utente));
                }
                if (uta.idRouloUtente == (decimal)EnumRuoloAccesso.Utente)
                {
                    dm.AddRange(dtn.GetListaDipendentiAutorizzati((decimal)EnumRuoloAccesso.Amministratore));
                }
                //dm.AddRange(dtn.GetListaDipendentiAutorizzati());
                if (dm.Count > 0)
                {
                    var agg = new SelectListItem(); agg.Text = "TUTTI (Trasferimenti attivi)"; agg.Value = "TUTTI";
                    r.Add(agg);
                    r0 = (from t in dm
                          where !string.IsNullOrEmpty(t.email) && t.email.Trim() != ""
                          orderby t.nome
                          select new SelectListItem()
                          {
                              Text = t.nome + " " + t.cognome,
                             //Value = t.email
                              Value = t.idDipendente.ToString()                               
                          }).ToList();
                    r.AddRange(r0);

                    r2 = (from t in dm
                          where !string.IsNullOrEmpty(t.email) && t.email.Trim() != ""
                          orderby t.nome
                          select new SelectListItem()
                          {
                              Text = t.nome + " " + t.cognome,
                             //Value = t.email
                              Value = t.idDipendente.ToString()
                          }).ToList();
                }
            }
            ViewBag.idMittenteLogato = idMittenteLogato;
            ViewBag.ListaDestinatari = r;
            ViewBag.ListaCc = r2;
        }
         public ActionResult SalvaPDF(string percorso)
        {
            ViewBag.percorso = percorso;
            NotificheModel nm = new NotificheModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                  //  db.Database.BeginTransaction();

                    foreach (string item in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[item] as HttpPostedFileBase;                                                          
                                bool esisteFile = false;
                                bool gestisceEstensioni = false;
                                bool dimensioneConsentita = false;
                                string dimensioneMaxConsentita = string.Empty;

                                PreSetDocumentoNotifiche(file, out nm, out esisteFile, out gestisceEstensioni,
                                    out dimensioneConsentita, out dimensioneMaxConsentita);

                                if (esisteFile)
                                {
                                    if (gestisceEstensioni == false)
                                    {
                                        throw new Exception(
                                        "Il documento selezionato non è nel formato consentito. Il formato supportato è: pdf.");
                                    }

                                    if (dimensioneConsentita)
                                    {
                                        //dttv.SetDocumentoTV(ref dm, idTitoliViaggio, db, idTipoDocumento);
                                        decimal idMittenteLogato = Utility.UtenteAutorizzato().idDipendente;
                                        ViewBag.idMittenteLogato = idMittenteLogato;
                                        AggiornaLista();
                                        ViewBag.Allegato = nm.Allegato;
                                        return PartialView("NuovaNotifica",nm);
                                    }
                                    else
                                    {
                                        throw new Exception(
                                            "Il documento selezionato supera la dimensione massima consentita (" +
                                            dimensioneMaxConsentita + " Mb).");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Il documento è obbligatorio.");
                                }
                    }
                   // db.Database.CurrentTransaction.Commit();
                    return Json(new { });
                }
                catch (Exception ex)
                {
                   // db.Database.CurrentTransaction.Rollback();
                    return Json(new { error = ex.Message });
                };
            }
        }
        public static void PreSetDocumentoNotifiche(HttpPostedFileBase file, out NotificheModel notmod, out bool esisteFile, out bool gestisceEstensioni, out bool dimensioneConsentita, out string dimensioneMaxDocumento)
        {
            notmod = new NotificheModel();
            gestisceEstensioni = false;
            dimensioneConsentita = false;
            esisteFile = false;
            dimensioneMaxDocumento = string.Empty;
           
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    esisteFile = true;

                    var estensioniGestite = new[] { ".pdf" };
                    var estensione = Path.GetExtension(file.FileName);
                    var nomeFileNoEstensione = Path.GetFileNameWithoutExtension(file.FileName);
                    if (!estensioniGestite.Contains(estensione.ToLower()))
                    {
                        gestisceEstensioni = false;
                    }
                    else
                    {
                        gestisceEstensioni = true;
                    }
                    var keyDimensioneDocumento = System.Configuration.ConfigurationManager.AppSettings["DimensioneDocumento"];

                    dimensioneMaxDocumento = keyDimensioneDocumento;

                    if (file.ContentLength / 1024 <= Convert.ToInt32(keyDimensioneDocumento))
                    {                      
                        notmod.PDFUpload = file;
                        MemoryStream ms = new MemoryStream();
                        notmod.PDFUpload.InputStream.CopyTo(ms);
                        notmod.Allegato = ms.ToArray();
                        dimensioneConsentita = true;
                    }
                    else
                    {
                        dimensioneConsentita = false;
                    }
                }
                else
                {
                    esisteFile = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult LeggiNotifichePDF(decimal id)
        {
            byte[] Blob;
            NotificheModel documento = new NotificheModel();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtNotifiche dtd = new dtNotifiche())
                    {
                        documento = dtd.GetDatiDocumentoById(id);
                        Blob = dtd.GetDocumentoByteById(id, db);

                        Response.AddHeader("Content-Disposition", "inline; filename=" + documento.idNotifica + ";");
                        return File(Blob, "application/pdf");
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
    }
}
using NewISE.Models.Config;
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
                nm = dn.GetNotifiche(idMittenteLogato).OrderBy(x => x.dataNotifica).ToList();
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
            using (dtNotifiche dtn = new dtNotifiche())
            {
                uta = dtn.RestituisciAutorizzato(idMittenteLogato);

                if (uta.idRouloUtente == 2) dm.AddRange(dtn.GetListaDipendentiAutorizzati(3));
                if (uta.idRouloUtente == 3) dm.AddRange(dtn.GetListaDipendentiAutorizzati(2));

                if (dm.Count > 0)
                {
                    var agg = new SelectListItem(); agg.Text = "TUTTI"; agg.Value = "TUTTI";
                    r.Add(agg);
                    r0 = (from t in dm
                          where !string.IsNullOrEmpty(t.email) && t.email.Trim() != ""
                          orderby t.nome
                          select new SelectListItem()
                          {
                              Text = t.nome + " " + t.cognome,
                              Value = t.email,
                          }).ToList();
                    r.AddRange(r0);

                    r2 = (from t in dm
                          where !string.IsNullOrEmpty(t.email) && t.email.Trim() != ""
                          orderby t.nome
                          select new SelectListItem()
                          {
                              Text = t.nome + " " + t.cognome,
                              Value = t.email,
                          }).ToList();
                }
            }
            ViewBag.idMittenteLogato = idMittenteLogato;
            ViewBag.ListaDestinatari = r;
            ViewBag.ListaCc = r2;
            return PartialView();
        }
        public ActionResult VisualizzaCorpoMessaggio(decimal idNotifica)
        {
            decimal idMittenteLogato = Utility.UtenteAutorizzato().idDipendente;
            List<NotificheModel> nm = new List<NotificheModel>();
            NotificheModel elem = new NotificheModel();
            using (dtNotifiche dn = new dtNotifiche())
            {
                nm = dn.GetNotifiche(idMittenteLogato).Where(a => a.idNotifica == idNotifica).ToList();
                if (nm.Count() > 0)
                {
                    elem = nm.First();
                }
            }
            return PartialView(elem);
        }
        [HttpPost]
        [ValidateInput(false)]
        //public ActionResult InserisciNuovaNotifica(NotificheModel nmod)
        //public ActionResult InserisciNuovaNotifica(HttpPostedFileBase PDFUpload, NotificheModel nmod)
        public ActionResult InserisciNuovaNotifica(string listaMailPrincipale, string listaMailToCc, string Oggetto, string CorpoMessaggio, HttpPostedFileBase file)
        {
            AggiornaLista();
            decimal idMittenteLogato = Utility.UtenteAutorizzato().idDipendente;ViewBag.idMittenteLogato = idMittenteLogato;
            UtentiAutorizzatiModel uta=null;
            NotificheModel nmod = new NotificheModel();
            //if (nmod.PDFUpload == null || nmod.PDFUpload.ContentLength == 0)
            //{            
            //    ModelState.AddModelError("PDFUpload", "Questo campo è richiesto");
            //}
           
            List<NotificheModel> lnm = new List<NotificheModel>();          
            using (ModelDBISE db = new ModelDBISE())
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
                    using (dtNotifiche dn = new dtNotifiche())
                    {
                        uta = dn.RestituisciAutorizzato(idMittenteLogato);
                        dn.InsertNotifiche(nmod);
                        db.Database.CurrentTransaction.Commit();
                        idMittenteLogato = nmod.idMittente;// Utility.UtenteAutorizzato().idDipendente;
                        lnm = dn.GetNotifiche(idMittenteLogato).ToList();
                    }
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    //return Json(new { error = ex.Message });
                    return PartialView("NuovaNotifica",nmod);
                };
            }
            //*************************************************************************
                            //INVIARE EMAIL SE NON SI E' VERIFICATO NESSUN ERRORE
            //*************************************************************************
            using (GestioneEmail gmail = new GestioneEmail())
            {
                ModelloAllegatoMail allegato = new ModelloAllegatoMail();
                Destinatario dest = new Destinatario();
                Destinatario destToCc = new Destinatario();
                ModelloMsgMail modMSGmail = new ModelloMsgMail();
                using (dtNotifiche dtn = new dtNotifiche())
                {
                    if (nmod.Allegato != null)
                    {
                        var docByte = dtn.GetDocumentoByteById(nmod.idNotifica);
                        Stream streamDoc = new MemoryStream(docByte);
                        allegato.nomeFile = DateTime.Now.Ticks.ToString() + ".pdf";
                        allegato.allegato = streamDoc;
                        modMSGmail.allegato.Add(allegato);
                    }
                    modMSGmail.oggetto = nmod.Oggetto;
                    modMSGmail.corpoMsg = nmod.corpoMessaggio;
                    Mittente mitt = new Mittente();
                    mitt.EmailMittente= dtn.GetEmailByIdDipendente(idMittenteLogato);
                    decimal id_dip = dtn.RestituisciIDdestinatarioDaEmail(mitt.EmailMittente);
                    DipendentiModel dmod = dtn.RestituisciDipendenteByID(id_dip);
                    mitt.Nominativo = dmod.nome + " " + dmod.cognome;
                    var ddss = dtn.GetListDestinatari(nmod.idNotifica);
                    foreach (var x in ddss)
                    {
                        string nome_ = dtn.RestituisciDipendenteByID(x.idDipendente).nome;
                        string cognome_ = dtn.RestituisciDipendenteByID(x.idDipendente).cognome;
                        string nominativo_ = nome_ + " " + cognome_;
                        if (!x.ToCc)
                        {
                            dest.EmailDestinatario = dtn.GetEmailByIdDipendente(x.idDipendente);
                            dest.Nominativo = nominativo_;
                            modMSGmail.destinatario.Add(dest);
                        }
                        else
                        {
                            destToCc.EmailDestinatario = dtn.GetEmailByIdDipendente(x.idDipendente);
                            destToCc.Nominativo = nominativo_;
                            modMSGmail.cc.Add(destToCc);
                        }
                        //qui non dimenticare di aggiungere a toCc tutti gli amministratori non in dest
                    }
                    modMSGmail.mittente = mitt;
                    gmail.sendMail(modMSGmail);
                }
            }
            return PartialView("ListaNotifiche", lnm);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RegistraNotifiche(FormCollection CollectionMessaggio)
        {
            FormCollection fc = new FormCollection(Request.Unvalidated().Form);
            string testo = fc["contenutoMessaggio"];
            return PartialView("ListaNotifiche");
        }
        public ActionResult VisualizzaDestinatari(decimal idNotifica)
        {
            List<DestinatarioModel> nm = new List<DestinatarioModel>();
            using (dtNotifiche dn = new dtNotifiche())
            {
                nm = dn.GetListDestinatari(idNotifica).ToList();
            }
            return PartialView(nm);
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
                //  dm.AddRange(dtn.GetListaDipendentiAutorizzati());
                if (dm.Count > 0)
                {
                    var agg = new SelectListItem(); agg.Text = "TUTTI"; agg.Value = "TUTTI";
                    r.Add(agg);
                    r0 = (from t in dm
                          where !string.IsNullOrEmpty(t.email) && t.email.Trim() != ""
                          orderby t.nome
                          select new SelectListItem()
                          {
                              Text = t.nome + " " + t.cognome,
                              Value = t.email,
                          }).ToList();
                    r.AddRange(r0);

                    r2 = (from t in dm
                          where !string.IsNullOrEmpty(t.email) && t.email.Trim() != ""
                          orderby t.nome
                          select new SelectListItem()
                          {
                              Text = t.nome + " " + t.cognome,
                              Value = t.email,
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

            using (dtNotifiche dtd = new dtNotifiche())
            {
                documento = dtd.GetDatiDocumentoById(id);
                Blob = dtd.GetDocumentoByteById(id);

                Response.AddHeader("Content-Disposition", "inline; filename=" + documento.idNotifica + ";");
                return File(Blob, "application/pdf");
            }
        }
        
    }
}
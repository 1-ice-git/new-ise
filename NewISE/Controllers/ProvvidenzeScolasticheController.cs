using System.Web.Routing;
using NewISE.EF;
using NewISE.Models.Tools;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.ViewModel;
using NewISE.Interfacce;


namespace NewISE.Controllers
{
    public class ProvvidenzeScolasticheController : Controller
    {
        // GET: ProvvidenzeScolastiche
        public ActionResult Index() 
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult ProvvidenzeScolastiche(decimal idTrasfProvScolastiche)
        {
            try
            {
                using (dtProvvidenzeScolastiche dtps = new dtProvvidenzeScolastiche())

                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        bool richiestaPS = false;
                        bool attivazionePS = false;
                        bool DocProvvidenzeScolastiche = false;
                        decimal NumAttivazioni = 0;
                        bool trasfAnnullato = false;

                        ProvvidenzeScolasticheModel tpsm = new ProvvidenzeScolasticheModel();

                        var atps = dtps.GetUltimaAttivazioneProvvScolastiche(idTrasfProvScolastiche);

                        dtps.SituazionePRovvidenzeScolastiche(idTrasfProvScolastiche,
                                                    out richiestaPS, out attivazionePS,
                                                    out DocProvvidenzeScolastiche,
                                                    out NumAttivazioni,
                                                    out trasfAnnullato);

                        List<SelectListItem> lDataAttivazione = new List<SelectListItem>();

                        ViewData.Add("richiestaPS", richiestaPS);
                        ViewData.Add("attivazionePS", attivazionePS);
                        ViewData.Add("DocProvvidenzeScolastiche", DocProvvidenzeScolastiche);
                        ViewData.Add("idTrasfProvScolastiche", idTrasfProvScolastiche);

                        ViewData.Add("lDataAttivazione", lDataAttivazione);

                        return PartialView("AttivitaProvvidenze",tpsm);
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

        }
        public JsonResult VerificaProvvidenze(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;

            try
            {
                if (idTrasferimento <= 0)
                {
                    throw new Exception(" non valorizzato");
                }
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    dipInfoTrasferimentoModel trm = dtt.GetInfoTrasferimento(idTrasferimento);
                    if (trm != null && (trm.statoTrasferimento == EnumStatoTraferimento.Attivo ||
                        trm.statoTrasferimento == EnumStatoTraferimento.Terminato))
                    {
                        ViewData["idTrasferimento"] = idTrasferimento;

                        return Json(new { VerificaProvvidenze = 1 });
                    }
                    else
                    {
                        return Json(new { VerificaProvvidenze = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }
        public ActionResult AttivitaProvvidenze(decimal idTrasferimento)
        {  
            ProvvidenzeScolasticheModel psm = new ProvvidenzeScolasticheModel();

            try
            {
                ViewData["idTrasferimento"] = idTrasferimento;


                using (dtProvvidenzeScolastiche dtps = new dtProvvidenzeScolastiche())
                {
                    psm = dtps.GetProvvidenzeScolasticheByID(idTrasferimento);

                    if (psm?.idTrasfProvScolastiche > 0)
                    {
                        using (dtAttivazioniProvScol dtaps = new dtAttivazioniProvScol())
                        {
                            var aps = dtaps.GetAttivazioneProvScol(psm.idTrasfProvScolastiche);

                            if (aps.idProvScolastiche == 0)
                            {
                                dtaps.CreaAttivazioneProvvidenzeScolastiche(psm.idTrasfProvScolastiche);

                            }

                            ViewData.Add("idTrasfProvScolastiche", aps.idTrasfProvScolastiche);
                            ViewData.Add("idProvScolastiche", aps.idProvScolastiche);
                        }
                    }
                    else
                    {
                        //throw new Exception("Provvidenza scolastica non trovata. IDTrasferimento: " + idTrasferimento);
                        using (dtAttivazioniProvScol dtaps = new dtAttivazioniProvScol())
                        {
                            var aps = dtaps.CreaProvvidenzeScolastiche(idTrasferimento);
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }
        public ActionResult ElencoFormulariInseriti(decimal idTrasferimento, decimal idProvScolastiche, decimal idTrasfProvScolastiche)
        {

            bool richiestaPS = false;
            bool attivazionePS = false;
            bool DocProvvidenzeScolastiche = false;
            //decimal NumAttivazioni = 0;
            bool trasfAnnullato = false;

            using (dtProvvidenzeScolastiche dtps = new dtProvvidenzeScolastiche())
            {
                dtps.SituazioneProvvScolVariazione(idTrasfProvScolastiche, out richiestaPS,
                out attivazionePS, out DocProvvidenzeScolastiche, out trasfAnnullato);

            }
            
            List<SelectListItem> lDataAttivazione = new List<SelectListItem>();
            List<ATTIVAZIONIPROVSCOLASTICHE> laps = new List<ATTIVAZIONIPROVSCOLASTICHE>();

            try
            {
                using (dtProvvidenzeScolastiche dtps = new dtProvvidenzeScolastiche())
                {
                    using (dtAttivazioniProvScol dtaps = new dtAttivazioniProvScol())
                    {
                       laps = dtaps.GetListAttivazioniProvvScolByIdProvvScol(idTrasfProvScolastiche);

                        var i = 1;

                        foreach (var e in laps)
                        {   
                           var ld = e.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Provvidenze_Scolastiche).ToList();
                            if (ld.Count > 0)
                            {

                                dtps.SituazioneAttivazioneProvvScolById(e.IDPROVSCOLASTICHE, out richiestaPS,
                                       out attivazionePS, out DocProvvidenzeScolastiche);
                                
                                    if (richiestaPS == false && DocProvvidenzeScolastiche)
                                    {
                                        lDataAttivazione.Insert(0, new SelectListItem() { Text = "(" + i.ToString() + ") " + e.DATAAGGIORNAMENTO.ToString() + " (In Lavorazione)", Value = e.IDPROVSCOLASTICHE.ToString() });
                                        //solaLettura = false;
                                    }
                                    if (richiestaPS)
                                    {
                                        lDataAttivazione.Insert(0, new SelectListItem() { Text = "(" + i.ToString() + ") " + e.DATAAGGIORNAMENTO.ToString(), Value = e.IDPROVSCOLASTICHE.ToString() });
                                    }
                                }
                            else
                            {
                                lDataAttivazione.Insert(0, new SelectListItem() { Text = "(" + i.ToString() + ") " + e.DATAAGGIORNAMENTO.ToString(), Value = e.IDPROVSCOLASTICHE.ToString() });
                            }
                            i++;

                        }

                        lDataAttivazione.Insert(0, new SelectListItem() { Text = "(TUTTE)", Value = "" });
                        ViewData.Add("lDataAttivazione", lDataAttivazione);

                        ViewData["idTipoDocumento"] = EnumTipoDoc.Formulario_Provvidenze_Scolastiche;
                        ViewData["idTrasferimento"] = idTrasferimento;
                        ViewData["idProvScolastiche"] = idProvScolastiche;
                    }

                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult TabFormulariInseriti(decimal idTrasfProvScolastiche, decimal idTipoDocumento, decimal idProvScolastiche)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();
            string DescrizioneTE = "";

            try
            {

                using (dtProvvidenzeScolastiche dtps = new dtProvvidenzeScolastiche())
                {
                    ldm = dtps.GetDocumentiPS(idTrasfProvScolastiche, idTipoDocumento);
                    
                }

                using (dtDocumenti dtd = new dtDocumenti())
                {
                    DescrizioneTE = dtd.GetDescrizioneTipoDocumentoByIdTipoDocumento(idTipoDocumento);
                }

                using (dtAttivazioniProvScol dtaps = new dtAttivazioniProvScol())
                {
                    var aps = dtaps.GetAttivazioneProvScol(idTrasfProvScolastiche);
                    var richiestaPS = false;

                    if (aps.notificaRichiesta == false)
                    {
                       richiestaPS = true;
                      
                    }

                    ViewData["richiestaPS"] = richiestaPS;
                }

                using (dtDocumenti dtd = new dtDocumenti())
                {
                    ldm = dtd.GetFormulariProvvidenzeScolasticheVariazione(idTrasfProvScolastiche).ToList();
                }


                ViewData["idProvScolastiche"] = idProvScolastiche;


            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(ldm);
            
        }
        public ActionResult TabFormulariInseriti1(decimal idTrasfProvScolastiche)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();
            try
            {

                //bool solaLettura = false;
                //solaLettura = this.SolaLetturaPartenza(idAttivazioneMagFam);
                //ViewData.Add("solaLettura", solaLettura);

                using (dtDocumenti dtd = new dtDocumenti())
                {
                    ldm = dtd.GetFormulariAttivazioneProvvScol(idTrasfProvScolastiche).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("TabFormulariInseriti", ldm);
        }
        public ActionResult ElencoDocumentiFormulario()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult NuovoFormularioPS(decimal idTrasfProvScolastiche)
        {
            try
            {
                ViewData["idTrasfProvScolastiche"] = idTrasfProvScolastiche;
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }
        public ActionResult NuovoDocumentoPS(decimal idTipoDocumento, decimal idTrasfProvScolastiche)
        {
            try
            {
                ViewData.Add("idTipoDocumento", idTipoDocumento);
                ViewData.Add("idTrasfProvScolastiche", idTrasfProvScolastiche);

                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        public static void PreSetDocumentoPS(HttpPostedFileBase file, out DocumentiModel dm, out bool esisteFile, out bool gestisceEstensioni, out bool dimensioneConsentita, out string dimensioneMaxDocumento, decimal idTipoDocumento)
        {
            dm = new DocumentiModel();
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
                        dm.nomeDocumento = nomeFileNoEstensione;
                        dm.estensione = estensione;
                        dm.tipoDocumento = (EnumTipoDoc)idTipoDocumento;
                        dm.dataInserimento = DateTime.Now;
                        dm.file = file;
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
        public JsonResult SalvaDocumentoPS(decimal idTipoDocumento, decimal idTrasfProvScolastiche)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    foreach (string item in Request.Files)
                    {

                        HttpPostedFileBase file = Request.Files[item] as HttpPostedFileBase;

                        using (dtProvvidenzeScolastiche dtps = new dtProvvidenzeScolastiche())
                        {
                            using (dtDocumenti dtd = new dtDocumenti())
                            {
                                DocumentiModel dm = new DocumentiModel();
                                bool esisteFile = false;
                                bool gestisceEstensioni = false;
                                bool dimensioneConsentita = false;
                                string dimensioneMaxConsentita = string.Empty;

                                PreSetDocumentoPS(file, out dm, out esisteFile, out gestisceEstensioni,
                                    out dimensioneConsentita, out dimensioneMaxConsentita, idTipoDocumento);

                                if (esisteFile)
                                {
                                    if (gestisceEstensioni == false)
                                    {
                                        throw new Exception(
                                        "Il documento selezionato non è nel formato consentito. Il formato supportato è: pdf.");
                                    }

                                    if (dimensioneConsentita)
                                    {
                                        dtps.SetDocumentoPS(ref dm, idTrasfProvScolastiche, db, idTipoDocumento);

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
                        }
                    }
                    db.Database.CurrentTransaction.Commit();
                    return Json(new { });
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    return Json(new { error = ex.Message });
                };
            }
        }
        public static void PreSetDocumentoTEPartenza(HttpPostedFileBase file, out DocumentiModel dm, out bool esisteFile, out bool gestisceEstensioni, out bool dimensioneConsentita, out string dimensioneMaxDocumento, decimal idTipoDocumento)
        {
            dm = new DocumentiModel();
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
                        dm.nomeDocumento = nomeFileNoEstensione;
                        dm.estensione = estensione;
                        dm.tipoDocumento = (EnumTipoDoc)idTipoDocumento;
                        dm.dataInserimento = DateTime.Now;
                        dm.file = file;
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
        public ActionResult TabDocumentiPSInseriti(decimal idTrasfProvScolastiche, decimal idTipoDocumento)
        {
            
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();
            string DescrizionePS = "";

            try
            {
                


                using (dtProvvidenzeScolastiche dtps = new dtProvvidenzeScolastiche())
                {
                    ldm = dtps.GetDocumentiPS(idTrasfProvScolastiche, idTipoDocumento);
                }


                using (dtDocumenti dtd = new dtDocumenti())
                {
                    DescrizionePS = dtd.GetDescrizioneTipoDocumentoByIdTipoDocumento(idTipoDocumento);
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewData.Add("DescrizionePS", DescrizionePS);
            ViewData.Add("idTipoDocumento", idTipoDocumento);
            ViewData.Add("idTrasfProvScolastiche", idTrasfProvScolastiche);

            
            return PartialView(ldm);
            
        }
        public ActionResult ElencoDocumentiPS(decimal idTipoDocumento, decimal idTrasfProvScolastiche)
        {
            try
            {
                string DescrizionePS = "";
                decimal idStatoTrasferimento = 0;

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var t = dtt.GetTrasferimentoByIDProvvScolastiche(idTrasfProvScolastiche);
                    idStatoTrasferimento = (decimal)t.idStatoTrasferimento;
                }

                using (dtDocumenti dtd = new dtDocumenti())
                {
                    DescrizionePS = dtd.GetDescrizioneTipoDocumentoByIdTipoDocumento(idTipoDocumento);
                }
                

                ViewData.Add("DescrizionePS", DescrizionePS);
                ViewData.Add("idTipoDocumento", idTipoDocumento);
                ViewData.Add("idTrasfProvScolastiche", idTrasfProvScolastiche);
                ViewData.Add("idStatoTrasferimento", idStatoTrasferimento);
                
                return PartialView("ElencoFormulariInseriti");
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        public JsonResult EliminaDocumentoPS(decimal idDocumento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (dtProvvidenzeScolastiche dtte = new dtProvvidenzeScolastiche())
                    {
                        dtte.DeleteDocumentoPS(idDocumento);
                    }

                    db.Database.CurrentTransaction.Commit();
                    return Json(new { msg = "Il documento relativo alle provvidenze scolastiche è stato eliminato." });
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    return Json(new { err = ex.Message });
                }
            }
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult FiltraTabFormulariInseriti(decimal idTrasfProvScolastiche, decimal idProvScolastiche)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            try
            {
                using (dtDocumenti dtd = new dtDocumenti())
                {
                    //ldm = dtd.GetFormulariProvvidenzeScolasticheByIdAttivazione(idTrasfProvScolastiche, idAttivazione).ToList();
                    ldm = dtd.GetFormulariProvvidenzeScolasticheByIdAttivazioneVariazione(idTrasfProvScolastiche, idProvScolastiche).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("TabFormulariInseriti", ldm);
        }
        public JsonResult GestionePulsantiNotificaAttivaAnnullaProvvidenzeScolastiche(decimal idTrasfProvScolastiche)
        {

            bool amministratore = false;
            string errore = "";
            bool richiestaPS = false;
            bool attivazionePS = false;
            bool DocProvvidenzeScolastiche = false;
            decimal NumAttivazioni = 0;
            bool trasfAnnullato = false;
           

            try
            {
                amministratore = Utility.Amministratore();

                using (dtProvvidenzeScolastiche dtps = new dtProvvidenzeScolastiche())
                {

                    dtps.SituazionePRovvidenzeScolastiche(idTrasfProvScolastiche,
                                            out richiestaPS,
                                            out attivazionePS,
                                            out DocProvvidenzeScolastiche,
                                            out NumAttivazioni,
                                            out trasfAnnullato
                                            );
                }

            }
            catch (Exception ex)
            {
                errore = ex.Message;
            }

            return
                Json(
                    new
                    {
                        admin = amministratore,
                        richiestaPS = richiestaPS,
                        attivazionePS = attivazionePS,
                        DocProvvidenzeScolastiche = DocProvvidenzeScolastiche,
                        NumAttivazioni = NumAttivazioni,
                        trasfAnnullato = trasfAnnullato,
                        err = errore
                    });

        }
        public JsonResult ConfermaNotificaRichiestaPS(decimal idTrasfProvScolastiche)
        {
            string errore = "";

            try
            {  

                using (dtProvvidenzeScolastiche dtps = new dtProvvidenzeScolastiche())
                {
                    decimal idProvScolastiche = dtps.GetUltimaAttivazioneProvvScolastiche(idTrasfProvScolastiche).IDPROVSCOLASTICHE;
                    dtps.NotificaRichiestaPS(idProvScolastiche);

                }

            }
            catch (Exception ex)
            {

                errore = ex.Message;
            }

            return
                Json(
                    new
                    {
                        err = errore
                    });
        }
        public JsonResult ConfermaAttivazioneRichiestaPS(decimal idTrasfProvScolastiche)
        {
            string errore = "";

            try
            {

                using (dtProvvidenzeScolastiche dtps = new dtProvvidenzeScolastiche())
                {
                    decimal idAttivazione = dtps.GetUltimaAttivazioneProvvScolastiche(idTrasfProvScolastiche).IDPROVSCOLASTICHE;
                    dtps.AttivaRichiestaProvvidenzeScolastiche(idAttivazione);

                }

            }
            catch (Exception ex)
            {

                errore = ex.Message;
            }

            return
                Json(
                    new
                    {
                        err = errore
                    });
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ConfermaAnnullaRichiestaPS(decimal idTrasfProvScolastiche, string msg)
        {
            string errore = "";

            try
            {
                

                using (dtProvvidenzeScolastiche dtps = new dtProvvidenzeScolastiche())
                {
                    decimal idAttivazione_notificata = dtps.GetUltimaAttivazioneProvvScolastiche(idTrasfProvScolastiche).IDPROVSCOLASTICHE;
                    dtps.AnnullaRichiestaProvvidenzeScolastiche(idAttivazione_notificata, msg);

                }

            }
            catch (Exception ex)
            {

                errore = ex.Message;
            }

            return
                Json(
                    new
                    {
                        err = errore
                    });
        }
        public ActionResult MessaggioAnnullaProvvidenzeScolastiche(decimal idTrasfProvScolastiche)
        {
            ModelloMsgMail msg = new ModelloMsgMail();

            try
            {
                using (dtDipendenti dtd = new dtDipendenti())
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        using (dtUffici dtu = new dtUffici())
                        {
                            var t = dtt.GetTrasferimentoByIDProvvScolastiche(idTrasfProvScolastiche);

                            if (t?.idTrasferimento > 0)
                            {
                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                var uff = dtu.GetUffici(t.idUfficio);

                                msg.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaProvvidenzeScolastiche, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView(msg);
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]

        public ActionResult TabFormulariProvvScolInseriti(decimal idTrasfProvScolastiche)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            try
            {
               
                using (dtDocumenti dtd = new dtDocumenti())
                {
                   ldm = dtd.GetFormulariProvvidenzeScolasticheVariazione(idTrasfProvScolastiche).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("TabFormulariInseriti", ldm);
        }

    }
}
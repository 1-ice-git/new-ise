using NewISE.EF;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using NewISE.Models.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NewISE.Controllers
{
    public class DocumentiController : Controller
    {
        public ActionResult LeggiDocumento(decimal id)
        {
            byte[] Blob;
            DocumentiModel documento = new DocumentiModel();

            using (dtDocumenti dtd = new dtDocumenti())
            {
                documento = dtd.GetDatiDocumentoById(id);
                Blob = dtd.GetDocumentoByteById(id);

                Response.AddHeader("Content-Disposition", "inline; filename=" + documento.idDocumenti + documento.estensione.ToLower() + ";");

                switch (documento.estensione.ToLower())
                {
                    case ".pdf":
                        return File(Blob, "application/pdf");
                    default:
                        return File(Blob, "application/pdf");
                }
            }
        }


        [Authorize(Roles = "1 ,2")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LetteraDiTrasferimento(decimal idTrasferimento)
        {
            TrasferimentoModel trm = new TrasferimentoModel();
            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    trm = dtt.GetTrasferimentoById(idTrasferimento);

                    return PartialView(trm);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public JsonResult InserisciFormularioTV(decimal idTitoloViaggio, HttpPostedFileBase file)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (dtDocumenti dtd = new dtDocumenti())
                    {
                        DocumentiModel dm = new DocumentiModel();
                        bool esisteFile = false;
                        bool gestisceEstensioni = false;
                        bool dimensioneConsentita = false;
                        string dimensioneMaxConsentita = string.Empty;

                        Utility.PreSetDocumento(file, out dm, out esisteFile, out gestisceEstensioni,
                            out dimensioneConsentita, out dimensioneMaxConsentita,
                            EnumTipoDoc.Formulario_Titoli_Viaggio);

                        if (esisteFile)
                        {
                            if (gestisceEstensioni == false)
                            {
                                throw new Exception(
                                    "Il documento selezionato non è nel formato consentito. Il formato supportato è: pdf.");
                            }

                            if (dimensioneConsentita)
                            {
                                dtd.SetFormularioTitoliViaggio(ref dm, idTitoloViaggio, db);
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


                    db.Database.CurrentTransaction.Commit();
                    return Json(new { msg = "Il formulario è stata inserito." });
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    return Json(new { err = ex.Message });
                }
            }
        }

        public JsonResult EliminaFormularioMF(decimal idDocumento, EnumChiamante chiamante)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (dtDocumenti dtd = new dtDocumenti())
                    {
                        dtd.DeleteDocumento(idDocumento, chiamante, db);
                    }
                    db.Database.CurrentTransaction.Commit();
                    return Json(new { msg = "Il formulario è stata eliminato." });
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    return Json(new { err = ex.Message });
                }
            }
        }

        [HttpPost]
        public JsonResult InserisciFormularioMF(decimal idAttivazioneMagFam, HttpPostedFileBase file)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (dtDocumenti dtd = new dtDocumenti())
                    {
                        DocumentiModel dm = new DocumentiModel();
                        bool esisteFile = false;
                        bool gestisceEstensioni = false;
                        bool dimensioneConsentita = false;
                        string dimensioneMaxConsentita = string.Empty;

                        Utility.PreSetDocumento(file, out dm, out esisteFile, out gestisceEstensioni,
                            out dimensioneConsentita, out dimensioneMaxConsentita,
                            EnumTipoDoc.Formulario_Maggiorazioni_Familiari);

                        if (esisteFile)
                        {
                            if (gestisceEstensioni == false)
                            {
                                throw new Exception(
                                    "Il documento selezionato non è nel formato consentito. Il formato supportato è: pdf.");
                            }

                            if (dimensioneConsentita)
                            {
                                dtd.SetFormularioMaggiorazioniFamiliari(ref dm, idAttivazioneMagFam, db);
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

                    db.Database.CurrentTransaction.Commit();
                    return Json(new { msg = "Il formulario è stata inserito." });
                }
                catch (Exception ex)
                {

                    db.Database.CurrentTransaction.Rollback();
                    return Json(new { err = ex.Message });
                }
            }
        }

        [Authorize(Roles = "1 ,2")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult InserisciLetteraTrasferimento(decimal idTrasferimento, string protocolloLettera,
            DateTime dataLettera, HttpPostedFileBase file)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    if (idTrasferimento <= 0)
                    {
                        throw new Exception("ID del trasferimento mancante.");
                    }

                    if (protocolloLettera == string.Empty)
                    {
                        throw new Exception("Il protocollo della lettera è obbligatorio.");
                    }

                    if (dataLettera == null && dataLettera <= DateTime.MinValue)
                    {
                        throw new Exception("La data della lettera è obbligatoria.");
                    }

                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        TrasferimentoModel trm = dtt.GetSoloTrasferimentoById(idTrasferimento, db);

                        trm.protocolloLettera = protocolloLettera;
                        trm.dataLettera = dataLettera;

                        dtt.EditTrasferimento(trm, db);

                        using (dtDocumenti dtd = new dtDocumenti())
                        {
                            DocumentiModel dm = new DocumentiModel();
                            bool esisteFile = false;
                            bool gestisceEstensioni = false;
                            bool dimensioneConsentita = false;
                            string dimensioneMaxConsentita = string.Empty;

                            Utility.PreSetDocumento(file, out dm, out esisteFile, out gestisceEstensioni,
                                out dimensioneConsentita, out dimensioneMaxConsentita,
                                EnumTipoDoc.Lettera_Trasferimento);

                            if (esisteFile)
                            {
                                if (gestisceEstensioni == false)
                                {
                                    throw new Exception(
                                        "Il documento selezionato non è nel formato consentito. Il formato supportato è: pdf.");
                                }

                                if (dimensioneConsentita)
                                {
                                    dtd.SetLetteraTrasferimento(ref dm, trm.idTrasferimento, db);
                                    //trm.Documento = dm;
                                    //Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (lettera di trasferimento).", "Documenti", db, trm.idTrasferimento, dm.idDocumenti);
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

                    db.Database.CurrentTransaction.Commit();
                    return Json(new { msg = "La lettera di trasferimento è stata inserita." });
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    return Json(new { err = ex.Message });
                }
            }
        }



        [HttpPost]
        public ActionResult NuovoDocumento(EnumTipoDoc tipoDoc, decimal id, EnumParentela parentela, EnumChiamante Chiamante, decimal idAttivazioneMagFam = 0)
        {
            string titoloPagina = string.Empty;
            //decimal idMaggiorazioniFamiliari = 0;
            //decimal idAttivazioneMagFam = 0;

            switch (tipoDoc)
            {
                case EnumTipoDoc.Carta_Imbarco:
                    titoloPagina = "Carta d'imbarco";
                    break;
                case EnumTipoDoc.Titolo_Viaggio:
                    titoloPagina = "Titolo viaggio";
                    break;
                case EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione:
                    titoloPagina = "Prima rata";
                    break;
                case EnumTipoDoc.Dichiarazione_Costo_Locazione:
                    titoloPagina = "Costo locazione";
                    break;
                case EnumTipoDoc.Attestazione_Spese_Abitazione:
                    titoloPagina = "Spese abitazione";
                    break;
                case EnumTipoDoc.Clausole_Contratto_Alloggio:
                    titoloPagina = "Clausole alloggio";
                    break;
                case EnumTipoDoc.Copia_Contratto_Locazione:
                    titoloPagina = "Copia contratto locazione";
                    break;
                case EnumTipoDoc.Contributo_Fisso_Omnicomprensivo:
                    titoloPagina = "Contributo omnicomprensivo";
                    break;
                case EnumTipoDoc.Attestazione_Trasloco:
                    titoloPagina = "Attestazione trasloco";
                    break;
                case EnumTipoDoc.Documento_Identita:
                    switch (parentela)
                    {
                        case EnumParentela.Coniuge:
                            titoloPagina = "Documento d'identità (Coniuge)";
                            //using (dtConiuge dtc = new dtConiuge())
                            //{
                            //    var cm = dtc.GetConiugebyID(id);
                            //    //idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari;
                            //    //using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                            //    //{
                            //    //    var amf = dtamf.GetAttivazioneMagFamByIdConiuge(cm.idConiuge);
                            //    //    idAttivazioneMagFam = amf.idAttivazioneMagFam;
                            //    //}

                            //}
                            break;
                        case EnumParentela.Figlio:
                            titoloPagina = "Documento d'identità (Figlio)";
                            //using (dtFigli dtf = new dtFigli())
                            //{
                            //    var fm = dtf.GetFigliobyID(id);
                            //    //idMaggiorazioniFamiliari = fm.idMaggiorazioniFamiliari;
                            //}
                            break;
                        case EnumParentela.Richiedente:
                            titoloPagina = "Documento d'identità (Richiedente)";

                            break;
                        default:
                            throw new ArgumentOutOfRangeException("parentela");
                    }


                    break;
                case EnumTipoDoc.Lettera_Trasferimento:
                    titoloPagina = "Trasferimento - Lettera trasferimento";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("tipoDoc");
            }

            ViewData.Add("titoloPagina", titoloPagina);
            ViewData.Add("tipoDoc", (decimal)tipoDoc);
            ViewData.Add("ID", id);
            //ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);
            ViewData.Add("parentela", (decimal)parentela);
            ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);

            return PartialView();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalvaDocumento(EnumTipoDoc tipoDoc, decimal id, EnumParentela parentela, decimal idAttivazioneMagFam = 0)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    //throw new Exception("Simulazione errore");

                    foreach (string item in Request.Files)
                    {

                        HttpPostedFileBase file = Request.Files[item] as HttpPostedFileBase;

                        using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                        {
                            using (dtDocumenti dtd = new dtDocumenti())
                            {
                                DocumentiModel dm = new DocumentiModel();
                                bool esisteFile = false;
                                bool gestisceEstensioni = false;
                                bool dimensioneConsentita = false;
                                string dimensioneMaxConsentita = string.Empty;

                                Utility.PreSetDocumento(file, out dm, out esisteFile, out gestisceEstensioni,
                                    out dimensioneConsentita, out dimensioneMaxConsentita, tipoDoc);

                                if (esisteFile)
                                {
                                    if (gestisceEstensioni == false)
                                    {
                                        throw new Exception(
                                            "Il documento selezionato non è nel formato consentito. Il formato supportato è: pdf.");
                                    }

                                    if (dimensioneConsentita)
                                    {
                                        switch (tipoDoc)
                                        {
                                            case EnumTipoDoc.Carta_Imbarco:
                                            case EnumTipoDoc.Titolo_Viaggio:
                                                switch (parentela)
                                                {
                                                    case EnumParentela.Coniuge:
                                                        dtd.AddDocumentoFromConiuge(ref dm, id, db);
                                                        break;
                                                    case EnumParentela.Figlio:
                                                        dtd.AddDocumentoFromFiglio(ref dm, id, db);
                                                        break;
                                                    case EnumParentela.Richiedente:
                                                        dtd.AddDocumentoTitoloViaggioFromRichiedente(ref dm, id, db);
                                                        break;
                                                    default:
                                                        throw new ArgumentOutOfRangeException("parentela");
                                                }
                                                break;
                                            case EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione:
                                                break;
                                            case EnumTipoDoc.Dichiarazione_Costo_Locazione:
                                                break;
                                            case EnumTipoDoc.Attestazione_Spese_Abitazione:
                                                break;
                                            case EnumTipoDoc.Clausole_Contratto_Alloggio:
                                                break;
                                            case EnumTipoDoc.Copia_Contratto_Locazione:
                                                break;
                                            case EnumTipoDoc.Contributo_Fisso_Omnicomprensivo:
                                                break;
                                            case EnumTipoDoc.Attestazione_Trasloco:
                                                break;
                                            case EnumTipoDoc.Documento_Identita:
                                                switch (parentela)
                                                {
                                                    case EnumParentela.Coniuge:
                                                        dtd.AddDocumentoFromConiuge(ref dm, id, db);
                                                        if (idAttivazioneMagFam > 0)
                                                        {
                                                            dtamf.AssociaDocumentoAttivazione(idAttivazioneMagFam, dm.idDocumenti, db);
                                                        }
                                                        break;
                                                    case EnumParentela.Figlio:
                                                        dtd.AddDocumentoFromFiglio(ref dm, id, db);
                                                        if (idAttivazioneMagFam > 0)
                                                        {
                                                            dtamf.AssociaDocumentoAttivazione(idAttivazioneMagFam, dm.idDocumenti, db);
                                                        }
                                                        break;
                                                    case EnumParentela.Richiedente:
                                                        dtd.AddDocumentoPassaportoFromRichiedente(ref dm, id, db);//ID è riferito all'idTrasferimento.
                                                        break;
                                                    default:
                                                        throw new ArgumentOutOfRangeException("parentela");
                                                }

                                                break;
                                            case EnumTipoDoc.Lettera_Trasferimento:
                                                break;

                                            default:
                                                throw new ArgumentOutOfRangeException("tipoDoc");
                                        }
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

        public ActionResult ElencoDocumentiPassaporto(decimal idFamiliarePassaporto, EnumTipoDoc tipoDoc, EnumParentela parentela)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();
            string NomePartialView = "ElencoDocumentiPassaporto";
            try
            {
                using (dtDocumenti dtd = new dtDocumenti())
                {
                    switch (parentela)
                    {
                        case EnumParentela.Coniuge:

                            ldm = dtd.GetDocumentiIdentitaConiugePassaporto(idFamiliarePassaporto).ToList();
                            return PartialView(ldm);

                        case EnumParentela.Figlio:
                            ldm = dtd.GetDocumentiIdentitaFiglioPassaporto(idFamiliarePassaporto).ToList();
                            return PartialView(ldm);

                        case EnumParentela.Richiedente:

                            break;
                        default:
                            throw new ArgumentOutOfRangeException("parentela");
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(ldm);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ElencoDocumenti(decimal id, EnumTipoDoc tipoDoc, EnumParentela parentela, EnumChiamante chiamante, decimal idAttivazioneMagFam = 0)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();
            ConiugeModel cm = new ConiugeModel();
            bool solaLettura = false;
            decimal idTrasferimento = 0;
            decimal idMaggiorazioniFamiliari = 0;


            try
            {

                using (dtDocumenti dtd = new dtDocumenti())
                {
                    ldm =
                        dtd.GetDocumentiByIdTable(id, tipoDoc, parentela, idAttivazioneMagFam)
                            .OrderByDescending(a => a.dataInserimento)
                            .ToList();

                }

                switch (chiamante)
                {
                    case EnumChiamante.Maggiorazioni_Familiari:
                    case EnumChiamante.VariazioneMaggiorazioniFamiliari:

                        switch (parentela)
                        {
                            case EnumParentela.Coniuge:
                                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                                {
                                    var mfm = dtmf.GetMaggiorazioniFamiliaribyConiuge(id);
                                    idMaggiorazioniFamiliari = mfm.idMaggiorazioniFamiliari;
                                    using (dtTrasferimento dtt = new dtTrasferimento())
                                    {
                                        idTrasferimento = dtt.GetTrasferimentoByIDMagFam(idMaggiorazioniFamiliari).idTrasferimento;
                                    }

                                }
                                break;
                            case EnumParentela.Figlio:
                                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                                {
                                    var mfm = dtmf.GetMaggiorazioniFamiliaribyFiglio(id);
                                    idMaggiorazioniFamiliari = mfm.idMaggiorazioniFamiliari;
                                    using (dtTrasferimento dtt = new dtTrasferimento())
                                    {
                                        idTrasferimento = dtt.GetTrasferimentoByIDMagFam(idMaggiorazioniFamiliari).idTrasferimento;
                                    }
                                }
                                break;
                            case EnumParentela.Richiedente:
                                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                                {
                                    var mfm = dtmf.GetMaggiorazioniFamiliariByID(id);
                                    idMaggiorazioniFamiliari = mfm.idMaggiorazioniFamiliari;
                                    using (dtTrasferimento dtt = new dtTrasferimento())
                                    {
                                        idTrasferimento = dtt.GetTrasferimentoByIDMagFam(idMaggiorazioniFamiliari).idTrasferimento;
                                    }
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("parentela");
                        }


                        using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
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


                            if ((parentela == EnumParentela.Coniuge || parentela == EnumParentela.Figlio) && idMaggiorazioniFamiliari > 0)
                            {
                                dtmf.SituazioneMagFamPartenza(idAttivazioneMagFam, out rinunciaMagFam,
                                    out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                                    out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli,
                                    out docFormulario);

                                if (richiestaAttivazione == true)
                                {
                                    solaLettura = true;
                                }
                                else
                                {
                                    solaLettura = false;
                                }
                            }
                            else
                            {
                                solaLettura = false;
                            }

                        }
                        break;
                    case EnumChiamante.Titoli_Viaggio:
                        using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                        {
                            TitoloViaggioModel tvm = new TitoloViaggioModel();

                            switch (parentela)
                            {
                                case EnumParentela.Coniuge:
                                    //tvm = dttv.GetTitoloViaggioByIdConiuge(id);
                                    idTrasferimento = tvm.idTrasferimento;
                                    if (tvm != null && tvm.HasValue())
                                    {
                                        bool notificaRichiesta = tvm.notificaRichiesta;
                                        bool praticaConclusa = tvm.praticaConclusa;

                                        if (notificaRichiesta == true && praticaConclusa == true)
                                        {
                                            solaLettura = true;
                                        }
                                        else
                                        {
                                            solaLettura = false;
                                        }

                                    }
                                    break;
                                case EnumParentela.Figlio:
                                    //tvm = dttv.GetTitoloViaggioByIdFiglio(id);
                                    idTrasferimento = tvm.idTrasferimento;
                                    if (tvm != null && tvm.HasValue())
                                    {
                                        bool notificaRichiesta = tvm.notificaRichiesta;
                                        bool praticaConclusa = tvm.praticaConclusa;

                                        if (notificaRichiesta == true && praticaConclusa == true)
                                        {
                                            solaLettura = true;
                                        }
                                        else
                                        {
                                            solaLettura = false;
                                        }

                                    }
                                    break;
                                case EnumParentela.Richiedente:
                                    //tvm = dttv.GetTitoloViaggioByID(id);
                                    idTrasferimento = tvm.idTrasferimento;
                                    if (tvm != null && tvm.HasValue())
                                    {
                                        bool notificaRichiesta = tvm.notificaRichiesta;
                                        bool praticaConclusa = tvm.praticaConclusa;

                                        if (notificaRichiesta == true && praticaConclusa == true)
                                        {
                                            solaLettura = true;
                                        }
                                        else
                                        {
                                            solaLettura = false;
                                        }

                                    }
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException("parentela");
                            }
                        }
                        break;
                    case EnumChiamante.Trasporto_Effetti:
                        using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                        {
                            var tem = dtte.GetTrasportoEffettiByID(id);
                            idTrasferimento = tem.idTrasferimento;
                        }
                        break;
                    case EnumChiamante.Trasferimento:
                        idTrasferimento = id;
                        break;
                    case EnumChiamante.Passaporti:

                        switch (parentela)
                        {
                            case EnumParentela.Coniuge:
                                using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                                {
                                    var ppm = dtpp.GetPassaportoByIdConiuge(id);
                                    idTrasferimento = ppm.idPassaporto;
                                }
                                break;
                            case EnumParentela.Figlio:
                                using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                                {
                                    var ppm = dtpp.GetPassaportoByIdFiglio(id);
                                    idTrasferimento = ppm.idPassaporto;
                                }
                                break;
                            case EnumParentela.Richiedente:
                                using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                                {
                                    PassaportoRichiedenteModel ppm = dtpp.GetPassaportoRichiedenteByID(id);
                                    idTrasferimento = ppm.idPassaporto;
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("parentela");
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException("chiamante");
                }



            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewData.Add("id", id);
            ViewData.Add("chiamante", chiamante);
            ViewData.Add("tipoDoc", tipoDoc);
            ViewData.Add("parentela", parentela);
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);
            ViewData.Add("solaLettura", solaLettura);
            ViewData.Add("idTrasferimento", idTrasferimento);
            ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);

            return PartialView(ldm);
        }

        [HttpPost]
        public JsonResult NumeroDocumentiSalvati(decimal id, EnumTipoDoc tipoDoc, EnumParentela parentela, decimal idAttivitaMagFam = 0)
        {
            int nDoc = 0;

            try
            {
                using (dtDocumenti dtd = new dtDocumenti())
                {
                    nDoc = dtd.GetDocumentiByIdTable(id, tipoDoc, parentela, idAttivitaMagFam).Count;
                }
            }
            catch (Exception ex)
            {

                return Json(new { errore = ex.Message, nDoc = 0 });
            }

            return Json(new { errore = "", nDoc = nDoc });
        }

        [HttpPost]
        public JsonResult EliminaDocumento(decimal idDocumento, EnumChiamante chiamante)
        {

            try
            {
                using (dtDocumenti dtd = new dtDocumenti())
                {
                    dtd.DeleteDocumento(idDocumento, chiamante);
                }
            }
            catch (Exception ex)
            {

                return Json(new { errore = ex.Message, msg = "" });
            }

            return Json(new { errore = "", msg = "Eliminazione effettuata con successo." });
        }


    }
}

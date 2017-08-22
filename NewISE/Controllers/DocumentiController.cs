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
                                EnumTipoDoc.LetteraTrasferimento_Trasferimento5);

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
        public ActionResult NuovoDocumento(EnumTipoDoc tipoDoc, decimal id)
        {
            string titoloPagina = string.Empty;
            decimal idMaggiorazioniFamiliari = 0;


            switch (tipoDoc)
            {
                case EnumTipoDoc.CartaImbarco_Viaggi1:
                    titoloPagina = "Viaggi - Carta d'imbarco";
                    break;
                case EnumTipoDoc.TitoloViaggio_Viaggi1:
                    titoloPagina = "Viaggi - Titolo viaggio";
                    break;
                case EnumTipoDoc.PrimaRataMab_MAB2:
                    titoloPagina = "Maggiorazione Abitazione - Prima rata";
                    break;
                case EnumTipoDoc.DichiarazioneCostoLocazione_MAB2:
                    titoloPagina = "Maggiorazione Abitazione - Costo locazione";
                    break;
                case EnumTipoDoc.AttestazioneSpeseAbitazione_MAB2:
                    titoloPagina = "Maggiorazione Abitazione - Spese abitazione";
                    break;
                case EnumTipoDoc.ClausoleContrattoAlloggio_MAB2:
                    titoloPagina = "Maggiorazione Abitazione - Clausole alloggio";
                    break;
                case EnumTipoDoc.CopiaContrattoLocazione_MAB2:
                    titoloPagina = "Maggiorazione Abitazione - Copia contratto locazione";
                    break;
                case EnumTipoDoc.ContributoFissoOmnicomprensivo_TrasportoEffetti3:
                    titoloPagina = "Trasporto Effetti - Contributo omnicomprensivo";
                    break;
                case EnumTipoDoc.AttestazioneTrasloco_TrasportoEffetti3:
                    titoloPagina = "Trasporto Effetti - Attestazione trasloco";
                    break;
                case EnumTipoDoc.DocumentoFamiliareConiuge_MaggiorazioniFamiliari4:
                    titoloPagina = "Maggiorazione Familiare - Documento familiare (Coniuge)";
                    using (dtConiuge dtc = new dtConiuge())
                    {
                        var cm = dtc.GetConiugebyID(id);
                        idMaggiorazioniFamiliari = cm.idMaggiorazioneFamiliari;
                    }
                    break;
                case EnumTipoDoc.DocumentoFamiliareFiglio_MaggiorazioniFamiliari4:
                    titoloPagina = "Maggiorazione Familiare - Documento familiare (Figlio)";
                    break;
                case EnumTipoDoc.LetteraTrasferimento_Trasferimento5:
                    titoloPagina = "Trasferimento - Lettera trasferimento";
                    break;
                case EnumTipoDoc.PassaportiVisti_Viaggi1:
                    titoloPagina = "Viaggi - Passaporti visti";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("tipoDoc");
            }

            ViewData.Add("titoloPagina", titoloPagina);
            ViewData.Add("tipoDoc", (decimal)tipoDoc);
            ViewData.Add("ID", id);
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);


            return PartialView();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalvaDocumento(EnumTipoDoc tipoDoc, decimal id)
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
                                        case EnumTipoDoc.CartaImbarco_Viaggi1:
                                            break;
                                        case EnumTipoDoc.TitoloViaggio_Viaggi1:
                                            break;
                                        case EnumTipoDoc.PrimaRataMab_MAB2:
                                            break;
                                        case EnumTipoDoc.DichiarazioneCostoLocazione_MAB2:
                                            break;
                                        case EnumTipoDoc.AttestazioneSpeseAbitazione_MAB2:
                                            break;
                                        case EnumTipoDoc.ClausoleContrattoAlloggio_MAB2:
                                            break;
                                        case EnumTipoDoc.CopiaContrattoLocazione_MAB2:
                                            break;
                                        case EnumTipoDoc.ContributoFissoOmnicomprensivo_TrasportoEffetti3:
                                            break;
                                        case EnumTipoDoc.AttestazioneTrasloco_TrasportoEffetti3:
                                            break;
                                        case EnumTipoDoc.DocumentoFamiliareConiuge_MaggiorazioniFamiliari4:
                                            dtd.AddDocumentoMagFamConiuge(ref dm, id, db);
                                            break;
                                        case EnumTipoDoc.DocumentoFamiliareFiglio_MaggiorazioniFamiliari4:
                                            break;
                                        case EnumTipoDoc.LetteraTrasferimento_Trasferimento5:
                                            break;
                                        case EnumTipoDoc.PassaportiVisti_Viaggi1:
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ElencoDocumenti(decimal id, EnumTipoDoc tipoDoc, decimal idMaggiorazioniFamiliari = 0)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();
            ConiugeModel cm = new ConiugeModel();
            try
            {
                using (dtDocumenti dtd = new dtDocumenti())
                {
                    switch (tipoDoc)
                    {
                        case EnumTipoDoc.CartaImbarco_Viaggi1:
                            break;
                        case EnumTipoDoc.TitoloViaggio_Viaggi1:
                            break;
                        case EnumTipoDoc.PrimaRataMab_MAB2:
                            break;
                        case EnumTipoDoc.DichiarazioneCostoLocazione_MAB2:
                            break;
                        case EnumTipoDoc.AttestazioneSpeseAbitazione_MAB2:
                            break;
                        case EnumTipoDoc.ClausoleContrattoAlloggio_MAB2:
                            break;
                        case EnumTipoDoc.CopiaContrattoLocazione_MAB2:
                            break;
                        case EnumTipoDoc.ContributoFissoOmnicomprensivo_TrasportoEffetti3:
                            break;
                        case EnumTipoDoc.AttestazioneTrasloco_TrasportoEffetti3:
                            break;
                        case EnumTipoDoc.DocumentoFamiliareConiuge_MaggiorazioniFamiliari4:
                            ldm = dtd.GetDocumentiByIdConiuge(id).OrderByDescending(a => a.dataInserimento).ToList();
                            break;
                        case EnumTipoDoc.DocumentoFamiliareFiglio_MaggiorazioniFamiliari4:
                            break;
                        case EnumTipoDoc.LetteraTrasferimento_Trasferimento5:
                            break;
                        case EnumTipoDoc.PassaportiVisti_Viaggi1:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("tipoDoc");
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewData.Add("id", id);
            ViewData.Add("tipoDoc", tipoDoc);
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);
            return PartialView(ldm);
        }

        [HttpPost]
        public JsonResult NumeroDocumentiSalvati(decimal id, EnumTipoDoc tipoDoc)
        {
            int nDoc = 0;

            try
            {
                using (dtDocumenti dtd = new dtDocumenti())
                {
                    switch (tipoDoc)
                    {
                        case EnumTipoDoc.CartaImbarco_Viaggi1:
                            break;
                        case EnumTipoDoc.TitoloViaggio_Viaggi1:
                            break;
                        case EnumTipoDoc.PrimaRataMab_MAB2:
                            break;
                        case EnumTipoDoc.DichiarazioneCostoLocazione_MAB2:
                            break;
                        case EnumTipoDoc.AttestazioneSpeseAbitazione_MAB2:
                            break;
                        case EnumTipoDoc.ClausoleContrattoAlloggio_MAB2:
                            break;
                        case EnumTipoDoc.CopiaContrattoLocazione_MAB2:
                            break;
                        case EnumTipoDoc.ContributoFissoOmnicomprensivo_TrasportoEffetti3:
                            break;
                        case EnumTipoDoc.AttestazioneTrasloco_TrasportoEffetti3:
                            break;
                        case EnumTipoDoc.DocumentoFamiliareConiuge_MaggiorazioniFamiliari4:
                            nDoc = dtd.GetDocumentiByIdConiuge(id).Count;
                            break;
                        case EnumTipoDoc.DocumentoFamiliareFiglio_MaggiorazioniFamiliari4:
                            break;
                        case EnumTipoDoc.LetteraTrasferimento_Trasferimento5:
                            break;
                        case EnumTipoDoc.PassaportiVisti_Viaggi1:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("tipoDoc");
                    }
                }
            }
            catch (Exception ex)
            {

                return Json(new { errore = ex.Message, nDoc = 0 });
            }

            return Json(new { errore = "", nDoc = nDoc });
        }

        [HttpPost]
        public JsonResult EliminaDocumento(decimal idDocumento)
        {

            try
            {
                using (dtDocumenti dtd = new dtDocumenti())
                {
                    dtd.DeleteDocumento(idDocumento);
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

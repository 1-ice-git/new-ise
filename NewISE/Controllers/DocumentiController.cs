using NewISE.EF;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Tools;
using System;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Controllers
{
    public class DocumentiController : Controller
    {
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
                throw;
            }
        }
        [Authorize(Roles = "1 ,2")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult InserisciLetteraTrasferimento(decimal idTrasferimento, string protocolloLettera, DateTime dataLettera, HttpPostedFileBase file)
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

                            Utility.PreSetDocumento(file, out dm, out esisteFile, out gestisceEstensioni, out dimensioneConsentita, out dimensioneMaxConsentita, EnumTipoDoc.LetteraTrasferimento);

                            if (esisteFile)
                            {
                                if (gestisceEstensioni == false)
                                {
                                    throw new Exception("Il documento selezionato non è nel formato consentito. Il formato supportato è: pdf.");
                                }

                                if (dimensioneConsentita)
                                {
                                    dtd.SetLetteraTrasferimento(ref dm, trm.idTrasferimento, db);
                                    //trm.Documento = dm;
                                    //Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (lettera di trasferimento).", "Documenti", db, trm.idTrasferimento, dm.idDocumenti);

                                }
                                else
                                {
                                    throw new Exception("Il documento selezionato supera la dimensione massima consentita (" + dimensioneMaxConsentita + " Mb).");
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



            //using (dtDocumenti dtd = new dtDocumenti())
            //{
            //    DocumentiModel dm = new DocumentiModel();
            //    bool esisteFile = false;
            //    bool gestisceEstensioni = false;
            //    bool dimensioneConsentita = false;

            //    Utility.PreSetDocumento(trm.file, out dm, out esisteFile, out gestisceEstensioni, out dimensioneConsentita);

            //    if (esisteFile)
            //    {
            //        if (gestisceEstensioni == false)
            //        {
            //            var lTipoTrasferimento = new List<SelectListItem>();
            //            var lUffici = new List<SelectListItem>();
            //            var lRuoloUfficio = new List<SelectListItem>();
            //            var lTipologiaCoan = new List<SelectListItem>();

            //            ListeComboNuovoTrasf(out lTipoTrasferimento, out lUffici, out lRuoloUfficio, out lTipologiaCoan);

            //            ViewBag.ListTipoTrasferimento = lTipoTrasferimento;
            //            ViewBag.ListUfficio = lUffici;
            //            ViewBag.ListRuolo = lRuoloUfficio;
            //            ViewBag.ListTipoCoan = lTipologiaCoan;

            //            ViewBag.ricaricaInfoTrasf = ricaricaInfoTrasf;
            //            ViewBag.Matricola = matricola;

            //            using (dtDipendenti dtd2 = new dtDipendenti())
            //            {
            //                var d = dtd2.GetDipendenteByMatricola(Convert.ToInt16(matricola));
            //                ViewBag.Dipendente = d;
            //            }

            //            //ViewBag.Modifica = modifica;

            //            ModelState.AddModelError("file", "Il documento selezionato non è nel formato consentito. \n Il formato supportato è: pdf.");

            //            return PartialView("NuovoTrasferimento", trm);
            //        }

            //        if (dimensioneConsentita)
            //        {
            //            dtd.SetLetteraTrasferimento(ref dm, trm.idTrasferimento, db);

            //            trm.Documento = dm;

            //            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (lettera di trasferimento).", "Documenti", db, trm.idTrasferimento, dm.idDocumenti);
            //        }
            //        else
            //        {
            //            var lTipoTrasferimento = new List<SelectListItem>();
            //            var lUffici = new List<SelectListItem>();
            //            var lRuoloUfficio = new List<SelectListItem>();
            //            var lTipologiaCoan = new List<SelectListItem>();

            //            ListeComboNuovoTrasf(out lTipoTrasferimento, out lUffici, out lRuoloUfficio, out lTipologiaCoan);

            //            ViewBag.ListTipoTrasferimento = lTipoTrasferimento;
            //            ViewBag.ListUfficio = lUffici;
            //            ViewBag.ListRuolo = lRuoloUfficio;
            //            ViewBag.ListTipoCoan = lTipologiaCoan;

            //            ViewBag.ricaricaInfoTrasf = ricaricaInfoTrasf;
            //            ViewBag.Matricola = matricola;

            //            using (dtDipendenti dtd2 = new dtDipendenti())
            //            {
            //                var d = dtd2.GetDipendenteByMatricola(Convert.ToInt16(matricola));
            //                ViewBag.Dipendente = d;
            //            }

            //            //ViewBag.Modifica = modifica;

            //            ModelState.AddModelError("file", "Il documento selezionato supera la dimensione massima consentita. \n Consentiti 5 MB.");

            //            return PartialView("NuovoTrasferimento", trm);
            //        }
            //    }
            //}
        }

        // GET: Documenti
        //public ActionResult NuovoDocumento(EnumDestinazioneDocumento destdoc, object dati)
        //{
        //    switch (destdoc)
        //    {
        //        case EnumDestinazioneDocumento.TrasportoEffettiSistemazione:
        //            break;

        //        case EnumDestinazioneDocumento.MaggiorazioneAbitazione:
        //            break;

        //        case EnumDestinazioneDocumento.NormaCalcolo:
        //            break;

        //        case EnumDestinazioneDocumento.TrasportoEffettiRientro:
        //            break;

        //        case EnumDestinazioneDocumento.Trasferimento:
        //            ViewBag.TrasferimentoModel = dati;
        //            break;

        //        case EnumDestinazioneDocumento.MaggiorazioniFamiliari:
        //            break;

        //        case EnumDestinazioneDocumento.Biglietti:
        //            break;

        //        case EnumDestinazioneDocumento.Passaporti:
        //            break;

        //        default:
        //            break;
        //    }

        //    ViewBag.EnumDestinazioneDocumento = destdoc;
        //    return PartialView();
        //}

        public JsonResult InserisciDocumento()
        {
            DocumentiModel dm = new DocumentiModel();

            return Json(dm);
        }

        //public ActionResult LeggiDocumento(decimal id)
        //{
        //    byte[] Blob;
        //    DocumentiModel documento = new DocumentiModel();

        //    using (dtDocumenti dtd = new dtDocumenti())
        //    {
        //        documento = dtd.GetDatiDocumentoById(id);
        //        Blob = dtd.GetDocumentoByteById(id);

        //        Response.AddHeader("Content-Disposition", "inline; filename=" + documento.idDocumenti + documento.Estensione.ToLower() + ";");

        //        switch (documento.Estensione.ToLower())
        //        {
        //            case ".pdf":
        //                return File(Blob, "application/pdf");
        //                break;

        //            default:
        //                return File(Blob, "application/pdf");
        //                break;
        //        }
        //    }
        //}
    }
}
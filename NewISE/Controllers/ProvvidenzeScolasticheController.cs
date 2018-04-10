using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Controllers
{
    public class ProvvidenzeScolasticheController : Controller
    {
        // GET: ProvvidenzeScolastiche
        public ActionResult Index()
        {
            return View();
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

            //MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel();
            ProvvidenzeScolasticheModel psm = new ProvvidenzeScolasticheModel();

            try
            {
                ViewData["idTrasferimento"] = idTrasferimento;
                // Inserire una funzione GetAttivitàProvvScolastiche (vd GetAttivazioneMagFamIniziale)

                //using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                //{
                //    mfm = dtmf.GetMaggiorazioniFamiliariByID(idTrasferimento);
                //    if (mfm?.idMaggiorazioniFamiliari > 0)
                //    {
                //        using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                //        {
                //            var amf = dtamf.GetAttivazioneMagFamIniziale(mfm.idMaggiorazioniFamiliari);

                //            ViewData.Add("idAttivazioneMagFam", amf.idAttivazioneMagFam);
                //        }
                //    }
                //    else
                //    {
                //        throw new Exception("Maggiorazione familiare non trovata. IDTrasferimento: " + idTrasferimento);
                //    }

                //}

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }

        
        public ActionResult ElencoFormulariInseriti(decimal idTrasferimento)
        {

            try
            {
               //solaLettura = this.SolaLetturaPartenza(idAttivazioneMagFam);

               // ViewData.Add("solaLettura", solaLettura);
                ViewData["idTrasferimento"] = idTrasferimento;
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult TabFormulariInseriti(decimal idTrasfProvScolastiche)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();
            try
            {

                bool solaLettura = false;
                //solaLettura = this.SolaLetturaPartenza(idAttivazioneMagFam);
                ViewData.Add("solaLettura", solaLettura);

                using (dtDocumenti dtd = new dtDocumenti())
                {
                  ldm = dtd.GetFormulariAttivazioneProvvScol(idTrasfProvScolastiche).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(ldm);
            
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
    }
}
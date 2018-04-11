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
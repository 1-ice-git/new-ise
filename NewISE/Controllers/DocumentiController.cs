using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Controllers
{
    public class DocumentiController : Controller
    {
        

        // GET: Documenti
        public ActionResult NuovoDocumento(EnumDestinazioneDocumento destdoc, object dati)
        {
            switch (destdoc)
            {
                case EnumDestinazioneDocumento.TrasportoEffettiSistemazione:
                    break;
                case EnumDestinazioneDocumento.MaggiorazioneAbitazione:
                    break;
                case EnumDestinazioneDocumento.NormaCalcolo:
                    break;
                case EnumDestinazioneDocumento.TrasportoEffettiRientro:
                    break;
                case EnumDestinazioneDocumento.Trasferimento:
                    ViewBag.TrasferimentoModel = dati;
                    break;
                case EnumDestinazioneDocumento.MaggiorazioniFamiliari:
                    break;
                case EnumDestinazioneDocumento.Biglietti:
                    break;
                case EnumDestinazioneDocumento.Passaporti:
                    break;
                default:
                    break;
            }

            ViewBag.EnumDestinazioneDocumento = destdoc;
            return PartialView();
        }

        public JsonResult InserisciDocumento()
        {
            DocumentiModel dm = new DocumentiModel();



            return Json(dm);

        }
    }
}
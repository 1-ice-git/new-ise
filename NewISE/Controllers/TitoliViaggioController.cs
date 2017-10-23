using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.ViewModel;

namespace NewISE.Controllers
{
    public class TitoliViaggioController : Controller
    {
        [HttpPost]
        public ActionResult NuovoFormularioTV(decimal idTitoloViaggio)
        {

            ViewData.Add("idTitoloViaggio", idTitoloViaggio);
            return PartialView();

        }

        [HttpPost]
        public ActionResult TitoliViaggio(decimal idTrasferimento)
        {


            using (dtTitoliViaggi dttv = new dtTitoliViaggi())
            {
                var tvm = dttv.GetTitoloViaggioInLavorazioneByIdTrasf(idTrasferimento);
                ViewData.Add("idTitoloViaggio", tvm.idTitoloViaggio);
            }

            ViewData.Add("idTrasferimento", idTrasferimento);
            return PartialView();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult RimborsoSuccessivo(decimal idTitoloViaggio)
        {
            bool rimborsoSuccessivo = false;

            using (dtTitoliViaggi dttv = new dtTitoliViaggi())
            {
                var tvm = dttv.GetTitoloViaggioByID(idTitoloViaggio);

                rimborsoSuccessivo = tvm.personalmente;

                ViewData.Add("idTitoloViaggio", tvm.idTitoloViaggio);
            }

            ViewData.Add("rimborsoSuccessivo", rimborsoSuccessivo);


            return PartialView();
        }

        [HttpPost]
        public ActionResult ConfermaOAnnullaRimborsoSuccessivo(decimal idTitoloViaggio)
        {
            using (dtTitoliViaggi dttv = new dtTitoliViaggi())
            {
                dttv.ModificaRimborsoSuccessivo(idTitoloViaggio);
            }

            return RedirectToAction("RimborsoSuccessivo", new { idTitoloViaggio = idTitoloViaggio });
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult GestioneFormularioTitoliViaggio(decimal idTitoloViaggio)
        {
            DocumentiModel dm = new DocumentiModel();

            using (dtTrasferimento dtt = new dtTrasferimento())
            {
                var tm = dtt.GetTrasferimentoByIdTitoloViaggio(idTitoloViaggio);

                ViewData.Add("trasferimento", tm);
            }

            using (dtDocumenti dtd = new dtDocumenti())
            {
                dm = dtd.GetFormularioTitoliViaggio(idTitoloViaggio);
            }

            using (dtTitoliViaggi dttv = new dtTitoliViaggi())
            {
                var tvm = dttv.GetTitoloViaggioByID(idTitoloViaggio);
                ViewData.Add("titoliViaggio", tvm);
            }


            //ViewData.Add("idTitoloViaggio", idTitoloViaggio);

            return PartialView(dm);
        }



        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult GestPulsantiNotificaAndPraticaConclusa(decimal idTitoloViaggio)
        {
            GestPulsantiAttConclRvModel gptv = new GestPulsantiAttConclRvModel();

            try
            {
                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    gptv = dttv.GestionePulsantiTitoliViaggi(idTitoloViaggio);
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(gptv);
        }


        [HttpPost]
        public JsonResult NotificaRichiesta(decimal idTitoloViaggio)
        {
            string errore = "";
            string msg = string.Empty;

            try
            {
                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    dttv.SetNotificaRichiesta(idTitoloViaggio);
                    msg = "Notifica effettuata con successo";
                }
            }
            catch (Exception ex)
            {
                errore = ex.Message;
            }

            return Json(new { err = errore, msg = msg });
        }

        [HttpPost]
        public JsonResult ConcludiPratica(decimal idTitoloViaggio)
        {
            string errore = "";
            string msg = string.Empty;

            try
            {
                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    dttv.SetPraticaConclusa(idTitoloViaggio);
                    msg = "Pratica conclusa con successo";
                }
            }
            catch (Exception ex)
            {

                errore = ex.Message;
            }

            return Json(new { err = errore, msg = msg });
        }


        public ActionResult UploadTitoliViaggio(decimal idTitoloViaggio)
        {


            using (dtTitoliViaggi dttv = new dtTitoliViaggi())
            {
                var tvm = dttv.GetTitoloViaggioByID(idTitoloViaggio);
                ViewData.Add("titoliViaggio", tvm);
            }

            return PartialView();
        }




    }
}
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
        // GET: TitoliViaggio
        public ActionResult TitoliViaggio(decimal idTrasferimento)
        {
            ViewData.Add("idTrasferimento", idTrasferimento);
            return PartialView();
        }






        [HttpPost]
        public JsonResult ConfermaEscludiTitoloViaggio(decimal id, EnumParentela parentela)
        {
            string errore = string.Empty;
            bool chk = false;

            try
            {
                switch (parentela)
                {
                    case EnumParentela.Coniuge:
                        using (dtConiuge dtc = new dtConiuge())
                        {
                            dtc.SetEscludiTitoloViaggio(id, ref chk);
                        }
                        break;
                    case EnumParentela.Figlio:
                        using (dtFigli dtf = new dtFigli())
                        {
                            dtf.SetEscludiTitoloViaggio(id, ref chk);
                        }
                        break;
                    case EnumParentela.Richiedente:

                        using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                        {
                            dttv.SetEscludiTitoloViaggio(id, ref chk);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("parentela");
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
                        chk = chk,
                        err = errore
                    });

        }

        [HttpPost]


        public ActionResult GestPulsantiNotificaAndPraticaConclusa(decimal idTrasferimento)
        {
            GestPulsantiAttConclRvModel gptv = new GestPulsantiAttConclRvModel();

            try
            {
                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    gptv = dttv.GestionePulsantiTitoliViaggi(idTrasferimento);
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(gptv);
        }


        [HttpPost]
        public JsonResult NotificaRichiesta(decimal idTrasferimento)
        {
            string errore = "";
            string msg = string.Empty;

            try
            {
                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    dttv.SetNotificaRichiesta(idTrasferimento);
                    msg = "Notifica effettuata con successo";
                }
            }
            catch (Exception ex)
            {
                errore = ex.Message;
            }

            return Json(new { err = errore, msg = msg });
        }


        public JsonResult ConcludiPratica(decimal idTrasferimento)
        {
            string errore = "";
            string msg = string.Empty;

            try
            {
                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    dttv.SetPraticaConclusa(idTrasferimento);
                    msg = "Pratica conclusa con successo";
                }
            }
            catch (Exception ex)
            {

                errore = ex.Message;
            }

            return Json(new { err = errore, msg = msg });
        }


    }
}
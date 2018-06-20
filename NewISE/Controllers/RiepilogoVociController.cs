using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Enumeratori;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Controllers
{
    public class RiepilogoVociController : Controller
    {
        // GET: RiepilogoVoci
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RiepilogoVoci(decimal idTrasferimento)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();
            
            try
            {
                using (dtIndennitaBase dtd = new dtIndennitaBase())
                {
                    libm = dtd.GetIndennitaBaseComune(idTrasferimento).ToList();
                }


                ViewData["idTrasferimento"] = idTrasferimento;
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(libm);


        }
        public JsonResult VerificaRiepilogoVoci(decimal idTrasferimento)
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

                        return Json(new { VerificaRiepilogoVoci = 1 });
                    }
                    else
                    {
                        return Json(new { VerificaRiepilogoVoci = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }

        public ActionResult AttivitaRiepilogoVoci(decimal idTrasferimento)
        {

            try
            {
                ViewData["idTrasferimento"] = idTrasferimento;
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }

        public ActionResult ElencoRiepilogoVoci(decimal idTrasferimento)
        {
            try
            {
                ViewData["idTrasferimento"] = idTrasferimento;
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }

}
}
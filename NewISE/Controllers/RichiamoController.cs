using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.MappingViews;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NewISE.EF;
using NewISE.Models;
using NewISE.Models.Tools;

namespace NewISE.Controllers
{
    public class RichiamoController : Controller
    {
        // GET: Richiamo
        public ActionResult Index()
        {
            return View();
            //return PartialView("Richiamo");
        }
        public ActionResult AttivitaRichiamo(decimal idTrasferimento)
        {
            try
            {
                ViewData["idTrasferimento"] = idTrasferimento;
                ViewData["dataPartenza"] = "Inserire qui la Data come idTrasferimento";
                
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("AttivitaRichiamo");
        }
                
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult ElencoRichiamo(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            ViewData["dataPartenza"] = "Inserire qui la Data come idTrasferimento";


            try
            {
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        public ActionResult Richiamo(decimal idTrasferimento)
        {
            try
            {
                ViewData["idTrasferimento"] = idTrasferimento;
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("Richiamo");
        }
        public JsonResult VerificaRichiamo(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            ViewData["dataPartenza"] = "Inserire qui la Data come idTrasferimento";

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

                        return Json(new { VerificaRichiamo = 1 });
                    }
                    else
                    {
                        return Json(new { VerificaRichiamo = 0 });

                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }
        public ActionResult DatiTabElencoRichiamo(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            List<RichiamoModel> tmp = new List<RichiamoModel>();
            try
            {
                using (dtRichiamo dtcal = new dtRichiamo())
                {
                    //tmp.AddRange(dtcal.GetLista_Richiamo(idTrasferimento));
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView(tmp);

        }
        
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult NuovoRichiamo(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;

            return PartialView();
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult InserisciRichiamo(RichiamoModel ri, decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        using (dtRichiamo dtrichiamo = new dtRichiamo())
                        {
                            using (ModelDBISE db = new ModelDBISE())
                            {
                                try
                                {
                                    db.Database.BeginTransaction();
                                    dtrichiamo.SetRichiamo(ref ri, db);

                                    //dtrichiamo.EditTrasferimento(ref trm, db);

                                    db.Database.CurrentTransaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    db.Database.CurrentTransaction.Rollback();
                                    return PartialView("ErrorPartial", new HandleErrorInfo(ex, "Richiamo", "InserisciRichiamo"));
                                }
                            }
                        }

                        

                        return RedirectToAction("ElencoRichiamo", new { idTrasferimento = ri.idTrasferimento });
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                        return PartialView("ElencoRichiamo", ri);
                    }

                }
                else
                {

                    return PartialView("ElencoRichiamo", ri);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            
        }


    }
}
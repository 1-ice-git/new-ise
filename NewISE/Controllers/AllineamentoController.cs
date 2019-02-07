using NewISE.Models;
using NewISE.Models.ViewModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.dtObj;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewISE.Models.Enumeratori;
using NewISE.EF;

namespace NewISE.Controllers
{
    [Authorize(Roles = "1 ,2")]
    public class AllineamentoController : Controller
    {
        // GET: Allineamento
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Allineamento()
        {
            bool admin = false;
            LogAllineamentoViewModel lavm = new LogAllineamentoViewModel();
            LOG_ALLINEAMENTO la = new LOG_ALLINEAMENTO();

            try
            {
                admin = Utility.Amministratore();

                string msg="";

                using (dtAllineamanto dta = new dtAllineamanto())
                {
                    using (ModelDBISE db = new ModelDBISE())
                    {
                        la = dta.GetLogAllineamento(db);
                        if (la.IDJOB > 0)
                        {
                            lavm.FaseElaborazione = la.FASE_ELABORAZIONE;
                            lavm.StatoElaborazione = la.STATO_ELAB;
                            lavm.InizioJob = la.INIZIO_JOB.Value;
                            lavm.DataOraInizio = null;
                            lavm.DataOraFine = null;
                            if (la.STATO_ELAB == (decimal)EnumStatoElaborazione.Terminata)
                            {
                                lavm.DataOraInizio = DateTime.Now;
                                lavm.DataOraFine = la.FINE_JOB;
                            }

                            if (la.STATO_ELAB == (decimal)EnumStatoElaborazione.Schedulata)
                            {
                                msg = "L'allineamento è schedulato alla ore " + lavm.InizioJob.ToShortTimeString() + " del " + lavm.InizioJob.ToShortDateString();
                                lavm.FaseElaborazione = "Data/ora schedulazione: " + lavm.InizioJob;
                            }
                        }
                        else
                        {
                            lavm.FaseElaborazione = ".n.d.";
                            lavm.DataOraInizio = DateTime.Now;
                            lavm.DataOraFine = null;
                        }
                    }
                }
                ViewBag.Amministratore = admin;
                ViewBag.msg = msg;
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView(lavm);
        }


        public JsonResult AvviaAllineamento(DateTime dataorainizio)
        {
            bool admin = false;
            string ret = "";
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        admin = Utility.Amministratore();

                        if (admin)
                        {
                            using (dtAllineamanto dta = new dtAllineamanto())
                            {
                                var la = dta.GetLogAllineamento(db);
                                if (la.STATO_ELAB == (decimal)EnumStatoElaborazione.Terminata || !(la.IDJOB>0))
                                {
                                    ret = dta.AvviaAllineamento(dataorainizio, db);
                                }
                                else
                                {
                                    throw new Exception("Impossibile eseguire la schedulazione. L'elaborazione non è in stato terminato.");
                                }                                
                            }

                            ViewBag.Amministratore = admin;
                        }
                        db.Database.CurrentTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        db.Database.CurrentTransaction.Rollback();
                        return Json(new { errore = ex.Message, msg = "" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { errore = ex.Message, msg = "" });
            }
            return Json(new { errore = "", msg = ret });
        }
    }
}
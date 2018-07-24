using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewISE.EF;
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Tools;
using NewISE.Models;
using NewISE.Models.ViewModel;

namespace NewISE.Controllers
{
    public class ElaborazioniController : Controller
    {
        [Authorize(Roles = "1 ,2")]
        [HttpGet]
        public ActionResult Index()
        {
            bool admin = false;

            try
            {
                admin = Utility.Amministratore();

                ViewBag.Amministratore = admin;
            }
            catch (Exception)
            {

                return View("Error");
            }


            return View();
        }
        [Authorize(Roles = "1 ,2")]
        [HttpGet]
        public ActionResult SelezionaMeseAnno(int mese = 0, int anno = 0)
        {
            var rMeseAnno = new List<SelectListItem>();
            List<MeseAnnoElaborazioneModel> lmaem = new List<MeseAnnoElaborazioneModel>();

            try
            {
                if (anno == 0)
                {
                    anno = DateTime.Now.Year;
                }

                if (mese == 0)
                {
                    mese = DateTime.Now.Month;
                }

                using (dtElaborazioni dte = new dtElaborazioni())
                {
                    lmaem = dte.PrelevaAnniMesiElaborati().ToList();

                    foreach (var item in lmaem)
                    {

                        rMeseAnno.Add(new SelectListItem()
                        {
                            Text = CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)item.Mese) + "-" + item.Anno.ToString("D4"),
                            Value = item.IdMeseAnnoElab.ToString()
                        });

                    }

                    if (rMeseAnno.Exists(a => a.Text == CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)mese) + "-" + anno.ToString("D4")))
                    {
                        foreach (var item in rMeseAnno)
                        {
                            if (item.Text == CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)mese) + "-" + anno.ToString("D4"))
                            {
                                item.Selected = true;
                            }
                        }
                    }
                    else
                    {
                        rMeseAnno.First().Selected = true;
                    }

                }

                ViewData["ElencoMesiAnniElaborati"] = rMeseAnno;
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }

        [Authorize(Roles = "1 ,2")]
        [HttpGet]
        public ActionResult DipendentiDaElaborare()
        {
            List<ElencoDipendentiDaCalcolareModel> ledcm = new List<ElencoDipendentiDaCalcolareModel>();

            try
            {
                using (dtElaborazioni dte = new dtElaborazioni())
                {
                    ledcm = dte.PrelevaDipendentiDaElaborare().ToList();
                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(ledcm);

        }
        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        public ActionResult CalcolaElaborazioneMensile(List<int> dipendenti, decimal idAnnoMeseElaborato)
        {

            try
            {
                using (dtElaborazioni dte = new dtElaborazioni())
                {
                    if (dipendenti?.Any() ?? false)
                    {
                        dte.Elaborazione(dipendenti, idAnnoMeseElaborato);
                    }
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("err", ex.Message);
            }


            return RedirectToAction("DatiLiquidazioneMensile", "Elaborazioni", new { idAnnoMeseElaborato = idAnnoMeseElaborato });
        }


        [Authorize(Roles = "1 ,2")]
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult DatiLiquidazioniDirette(decimal idAnnoMeseElaborato)
        {

            ViewData.Add("idAnnoMeseElaborato", idAnnoMeseElaborato);
            return PartialView();
        }

        public ActionResult DatiLiquidazioniDiretteDaInviare(decimal idAnnoMeseElaborato)
        {
            List<LiquidazioniDiretteViewModel> lLd = new List<LiquidazioniDiretteViewModel>();

            try
            {
                using (dtElaborazioni dte = new dtElaborazioni())
                {
                    lLd = dte.PrelevaLiquidazioniDirette(idAnnoMeseElaborato).Where(a => a.Elaborato == false).ToList();
                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(lLd);
        }


        public ActionResult DatiLiquidazioniDiretteInviate(decimal idAnnoMeseElaborato)
        {
            List<LiquidazioniDiretteViewModel> lLd = new List<LiquidazioniDiretteViewModel>();

            try
            {
                using (dtElaborazioni dte = new dtElaborazioni())
                {
                    lLd = dte.PrelevaLiquidazioniDirette(idAnnoMeseElaborato).Where(a => a.Elaborato == true).ToList();
                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(lLd);
        }



        [Authorize(Roles = "1 ,2")]
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult DatiLiquidazioneMensile(decimal idAnnoMeseElaborato)
        {
            List<LiquidazioneMensileViewModel> lLm = new List<LiquidazioneMensileViewModel>();
            try
            {
                using (dtElaborazioni dte = new dtElaborazioni())
                {
                    lLm = dte.PrelevaLiquidazioniMensili(idAnnoMeseElaborato).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


            return PartialView(lLm);
        }

        [Authorize(Roles = "1 ,2")]
        public ActionResult PulsantiInvioElaborazione(decimal idAnnoMeseElaborato)
        {
            ViewData["idAnnoMeseElaborato"] = idAnnoMeseElaborato;

            return PartialView();
        }

        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        public JsonResult InviaFlussiDirettiOA(decimal idAnnoMeseElaborato, List<decimal> Teorici)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    using (dtElaborazioni dte = new dtElaborazioni())
                    {
                        try
                        {
                            foreach (decimal teorico in Teorici)
                            {
                                dte.InviaFlussiDirettiContabilita(idAnnoMeseElaborato, teorico, db);
                            }

                            db.Database.CurrentTransaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            db.Database.CurrentTransaction.Rollback();
                            throw ex;
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                return Json(new { msg = "", err = ex.Message });
            }

            return Json(new { msg = "I flussi diretti sono stati inviati.", err = "" });

            //return RedirectToAction("DatiLiquidazioniDirette", "Elaborazioni", new { idAnnoMeseElaborato = idAnnoMeseElaborato });
        }

        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        public JsonResult InviaFlussiMensili(decimal idAnnoMeseElaborato, List<decimal> Teorici)
        {
            List<DIPENDENTI> lDip = new List<DIPENDENTI>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    using (dtElaborazioni dte = new dtElaborazioni())
                    {
                        try
                        {
                            foreach (decimal teorico in Teorici)
                            {
                                dte.InviaFlussiMensili(idAnnoMeseElaborato, teorico, db);
                            }

                            lDip = dte.EstrapolaDipendentiDaTeorici(Teorici, db).ToList();

                            foreach (var dip in lDip)
                            {
                                dte.SetPeriodoElaborazioniDipendente(dip.IDDIPENDENTE, idAnnoMeseElaborato, db);
                            }

                            db.Database.CurrentTransaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            db.Database.CurrentTransaction.Rollback();
                            throw ex;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = "", err = ex.Message });
            }

            return Json(new { msg = "I flussi mensili sono stati inviati.", err = "" });
        }

    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using NewISE.EF;
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Tools;
using NewISE.Models;
using NewISE.Models.ViewModel;
using NewISE.Models.DBModel;
using RestSharp;

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
        [HttpPost]
        public JsonResult VerificaFlussiDirettiDaInviare(decimal idAnnoMeseElaborato)
        {
            bool vfd = false;

            try
            {
                using (dtElaborazioni dte = new dtElaborazioni())
                {
                    vfd = dte.VerificaLiquidazioniDiretteDaInviare(idAnnoMeseElaborato);
                }
            }
            catch (Exception ex)
            {
                return Json(new { vfd = false, err = ex.Message });
            }
            return Json(new { vfd = vfd, err = "" });
        }

        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        public JsonResult PrelevaMesiAnniElab(string search)
        {
            List<Select2Model> ls2 = new List<Select2Model>();
            List<MeseAnnoElaborazioneModel> lmaem = new List<MeseAnnoElaborazioneModel>();

            try
            {
                using (dtElaborazioni dte = new dtElaborazioni())
                {


                    lmaem = dte.PrelevaAnniMesiElaborati().ToList();

                    foreach (var mae in lmaem)
                    {
                        Select2Model s2 = new Select2Model()
                        {
                            id = mae.idMeseAnnoElab.ToString(),
                            text = CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)mae.mese) + "-" + mae.anno.ToString("D4"),
                        };

                        ls2.Add(s2);
                    }


                }

                if (search != null && search != string.Empty)
                {
                    ls2 = ls2.Where(a => a.text.ToUpper().Contains(search.ToUpper())).ToList();

                }
            }
            catch (Exception ex)
            {

                return Json(new { results = new List<Select2Model>(), err = ex.Message });
            }

            return Json(new { results = ls2, err = "" });
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
                            Text = CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)item.mese) + "-" + item.anno.ToString("D4"),
                            Value = item.idMeseAnnoElab.ToString()
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
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult DipendentiDaElaborare()
        {
            List<ElencoDipendentiDaCalcolareModel> ledcm = new List<ElencoDipendentiDaCalcolareModel>();

            try
            {
                using (dtElaborazioni dte = new dtElaborazioni())
                {
                    ledcm = dte.PrelevaDipendentiDaElaborare().ToList();


                    //foreach (var ed in ledcm)
                    //{

                    //}



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
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
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
        public JsonResult GestionePulsanteCalcola(List<decimal> lDipendenti, decimal idAnnoMeseElab)
        {
            bool gpCalcola = false;
            bool gpFlussoMensile = false;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtElaborazioni dte = new dtElaborazioni())
                    {

                        if (lDipendenti?.Any() ?? false)
                        {
                            gpCalcola = !dte.VerificaElaborazioneDipendenti(lDipendenti, idAnnoMeseElab, db);
                        }

                        gpFlussoMensile = dte.VerificaChiusuraPeriodoElab(idAnnoMeseElab, db);
                    }
                }

            }
            catch (Exception ex)
            {
                return Json(new { gpCalcola = gpCalcola, gpFlussoMensile = gpFlussoMensile, err = ex.Message });
            }

            return Json(new { gpCalcola = gpCalcola, gpFlussoMensile = gpFlussoMensile, err = "" });
        }


        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        public JsonResult VerificaChiusuraMeseElab(decimal idAnnoMeseElab)
        {
            bool periodoChiuso = false;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtElaborazioni dte = new dtElaborazioni())
                    {
                        periodoChiuso = dte.VerificaChiusuraPeriodoElab(idAnnoMeseElab, db);
                    }
                }

            }
            catch (Exception ex)
            {
                return Json(new { pc = periodoChiuso, err = ex.Message });
            }

            return Json(new { pc = periodoChiuso, err = "" });
        }

        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        public JsonResult GestionePulsanteInvioFlussiMensili(decimal idAnnoMeseElab)
        {
            bool periodoChiuso = false;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtElaborazioni dte = new dtElaborazioni())
                    {
                        periodoChiuso = dte.VerificaChiusuraPeriodoElab(idAnnoMeseElab, db);
                    }
                }

            }
            catch (Exception ex)
            {
                return Json(new { pc = periodoChiuso, err = ex.Message });
            }

            return Json(new { pc = periodoChiuso, err = "" });
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
                            Teorici = Teorici.OrderBy(a => a).ToList();
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
                                var dip = dte.EstrapolaDipendenteDaTeorico(teorico, db);

                                if (!dip.ELABORAZIONI?.Any(a => a.IDMESEANNOELAB == idAnnoMeseElaborato) ?? false)
                                {
                                    dte.InviaFlussiMensili(idAnnoMeseElaborato, teorico, db);
                                }
                            }

                            lDip = dte.EstrapolaDipendentiDaTeorici(Teorici, db).ToList();

                            foreach (var dip in lDip)
                            {
                                dte.SetPeriodoElaborazioniDipendente(dip.IDDIPENDENTE, idAnnoMeseElaborato, db);
                            }

                            dte.ChiudiPeridoElaborazione(idAnnoMeseElaborato, db);


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
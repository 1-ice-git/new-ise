using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
        public ActionResult ElaborazioneMensile(List<decimal> dipendenti)
        {

            return PartialView();
        }

        public ActionResult DatiLiquidazioniDirette(decimal idAnnoMeseElaborato)
        {
            List<LiquidazioniDiretteViewModel> lLd = new List<LiquidazioniDiretteViewModel>();

            using (dtElaborazioni dte = new dtElaborazioni())
            {
                lLd = dte.PrelevaLiquidazioniDirette(idAnnoMeseElaborato).ToList();
            }

            return PartialView(lLd);
        }



    }
}
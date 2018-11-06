using NewISE.EF;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewISE.Models.Enumeratori;


namespace NewISE.Controllers
{

    public class HomeController : Controller
    {
        // GET: Home
        bool admin = false;
        public ActionResult Index()
        {

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


        public ActionResult GetListaHome(decimal idStatoHome)
        {
            List<ElencoElementiHome> tmp = new List<ElencoElementiHome>();
            try
            {
                using (dtCalendarioEventi dtcal = new dtCalendarioEventi())
                {
                    tmp = dtcal.GetListaElementiHome(idStatoHome).ToList();

                }
                var r = new List<SelectListItem>();
                var elem = new SelectListItem()
                {
                    Text = "ATTIVI",
                    Value = ((decimal)EnumStatoHome.Attivi).ToString(),
                }; r.Add(elem);
                elem = new SelectListItem()
                {
                    Text = "COMPLETATI",
                    Value = ((decimal)EnumStatoHome.Completati).ToString(),
                }; r.Add(elem);
                elem = new SelectListItem()
                {
                    Text = "SCADUTI",
                    Value = ((decimal)EnumStatoHome.Scaduti).ToString(),
                }; r.Add(elem);
                elem = new SelectListItem()
                {
                    Text = "TUTTI",
                    Value = ((decimal)EnumStatoHome.Tutti).ToString(),
                }; r.Add(elem);

                if (idStatoHome == 0)
                {
                    r.First().Selected = true;
                }
                else
                {
                    var temp = r.Where(a => a.Value == idStatoHome.ToString()).ToList();
                    if (temp.Count == 0)
                    {
                        r.First().Selected = true;
                        idStatoHome = Convert.ToDecimal(r.First().Value);
                    }
                    else
                        r.Where(a => a.Value == idStatoHome.ToString()).First().Selected = true;
                }
                ViewBag.idStatoHome = idStatoHome;
                ViewBag.StatoHome = r;
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView(tmp);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult DetailsFunzioneEvento(EnumFunzioniEventi idf, int idd)
        {
            DettagliMessaggio tmp = new DettagliMessaggio();
            try
            {
                using (dtCalendarioEventi dtcal = new dtCalendarioEventi())
                {
                    tmp = dtcal.OgggettoFunzioneEvento(idf, idd);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView(tmp);
        }
    }
}


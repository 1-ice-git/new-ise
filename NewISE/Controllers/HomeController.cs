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

        
        public ActionResult GetListaHome()
        {            
            List<ElencoElementiHome> tmp = new List<ElencoElementiHome>();
            try
            {
                using (dtCalendarioEventi dtcal = new dtCalendarioEventi())
                {
                    tmp = dtcal.GetListaElementiHome().ToList();                    
                }               
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView(tmp);
        }

        [HttpGet]
        public ActionResult DetailsFunzioneEvento(EnumFunzioniEventi idf,int idd)
        {
            DettagliMessaggio tmp = new DettagliMessaggio();
            try
            {
                using (dtCalendarioEventi dtcal = new dtCalendarioEventi())
                {
                   tmp = dtcal.OgggettoFunzioneEvento(idf,idd);
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


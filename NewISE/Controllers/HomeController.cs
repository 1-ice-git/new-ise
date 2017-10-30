using NewISE.Models;
using NewISE.Models.Tools;
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
        
        public ActionResult MsgUtente(int idUtente)
        {
            return null;
        }
        public ActionResult FunzioneEvento(string nomefunzione = "")
        {
            var objList = new List<SelectListItem>();
            bool admin = false;
            List<FunzioneEventoModel> lfe = new List<FunzioneEventoModel>();
            FunzioneEventoModel obj = new FunzioneEventoModel();
            AccountModel ac = new AccountModel();
            try
            {

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(obj);
        }

    }
}
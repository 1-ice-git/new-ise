using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewISE.Models.Tools;

namespace NewISE.Controllers
{
    public class ElaborazioniController : Controller
    {
        // GET: Elaborazioni
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

        public ActionResult SelezionaAnno(int anno = 0)
        {
            var rAnno = new List<SelectListItem>();


            return PartialView();
        }
    }
}
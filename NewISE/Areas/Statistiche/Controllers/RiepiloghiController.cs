using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Areas.Statistiche.Controllers
{
    public class RiepiloghiController : Controller
    {
        // GET: Statistiche/Riepiloghi
        public ActionResult Index()
        {

            var t = new List<SelectListItem>();

            try
            {
                t.Add(new SelectListItem() { Text = "", Value = "", Selected = true });
                t.Add(new SelectListItem() { Text = "ISE Mensile", Value = "0" });
                t.Add(new SelectListItem() { Text = "Maggiorazione Abitazione", Value = "1" });
                t.Add(new SelectListItem() { Text = "Altre Spese", Value = "2" });
                t.Add(new SelectListItem() { Text = "Altre Indennità", Value = "3" });

                ViewBag.Riepiloghi = t;
                return PartialView();

            }

            catch (Exception ex)
            {
                return View("Error");

            }
            

        }

        public ActionResult IseMensile()
        {
            return View();
        }

        public ActionResult MaggAbitazione()
        {
            return View();
        }

        public ActionResult AltreIndennita()
        {
            return View();
        }

        public ActionResult AltreSpese()
        {
            return View();
        }
    }
}
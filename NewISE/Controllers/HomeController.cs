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
        public ActionResult Index()
        {
            List<ElencoElementiHome> leeh = new List<ElencoElementiHome>();

            return View(leeh);
        }
        
        public ActionResult MsgUtente(int idUtente)
        {
            return null;
        }
    }
}
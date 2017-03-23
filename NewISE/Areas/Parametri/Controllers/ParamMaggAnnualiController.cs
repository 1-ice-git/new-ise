using NewISE.Areas.Parametri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamMaggAnnualiController : Controller
    {
        // GET: Parametri/ParamMaggAnnuali/MaggiorazioniAnnuali

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult MaggiorazioniAnnuali()
        {
            List<MaggiorazioniAnnualiModel> libm = new List<MaggiorazioniAnnualiModel>();

            try
            {



            }
            catch (Exception)
            {

                throw;
            }

            return PartialView(libm);
        }
    }
}
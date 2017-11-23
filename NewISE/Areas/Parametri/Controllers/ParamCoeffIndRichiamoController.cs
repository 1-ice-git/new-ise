using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using NewISE.Models;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamCoeffIndRichiamoController : Controller
    {
        // GET: Parametri/ParamCoeffIndRichiamo
        
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult CoeffIndennitaRichiamo()
        {
            List<CoefficienteRichiamoModel> libm = new List<CoefficienteRichiamoModel>();
            var r = new List<SelectListItem>();
            try
            {
                ViewBag.CoeffIndRichiamo = r;
            }

            
            catch (Exception)
            {

                throw;
            }

            return View("CoeffIndRichiamo");
        }

        
    }
}
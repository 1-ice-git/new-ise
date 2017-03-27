using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamPercDisagioController : Controller
    {
        // GET: /Parametri/ParamPercDisagio/PercentualeDisagio

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult PercentualeDisagio()
        {
            List<PercentualeDisagioModel> libm = new List<PercentualeDisagioModel>();
            try
            {

            }
            catch (Exception)
            {

                return PartialView("ErrorPartial");
            }
            return PartialView(libm);
        }
    }
}
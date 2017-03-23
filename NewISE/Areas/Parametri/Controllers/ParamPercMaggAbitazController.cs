using NewISE.Areas.Parametri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamPercMaggAbitazController : Controller
    {
        // GET: Parametri/ParamPercMaggAbitaz/PercentualeMaggAbitazione

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult PercentualeMaggAbitazione()
        {
            List<PercMaggAbitazModel> libm = new List<PercMaggAbitazModel>();

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
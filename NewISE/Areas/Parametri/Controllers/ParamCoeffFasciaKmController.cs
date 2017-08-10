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
    public class ParamCoeffFasciaKmController : Controller
    {
        // GET: Parametri/ParamCoeffFasciaKm/CoefficienteFasciaKm

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult CoefficienteFasciaKm()
        {
            List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();

            try
            {

            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView(libm);
        }
    }
}
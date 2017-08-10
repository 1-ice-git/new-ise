using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NewISE.Models;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamFasciaKmController : Controller
    {
        // GET: Parametri/ParamFasciaKm/FasciaKm

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult FasciaKm()
        {
            List<FasciaChilometricaModel> libm = new List<FasciaChilometricaModel>();
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
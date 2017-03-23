using NewISE.Areas.Parametri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamRiduzioniController : Controller
    {
        // GET: /Parametri/ParamRiduzioni/Riduzioni

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult Riduzioni()
        {
            List<RiduzioniModel> libm = new List<RiduzioniModel>();

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
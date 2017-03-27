using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamMaggFigliController : Controller
    {
        // GET: Parametri/ParamMaggFigli/MaggiorazioneFigli

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult MaggiorazioneFigli()
        {
            List<MaggiorazioneFigliModel> libm = new List<MaggiorazioneFigliModel>();

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
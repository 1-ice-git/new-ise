using NewISE.Areas.Parametri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamMaggConiugeController : Controller
    {
        // GET: Parametri/ParamMaggConiuge/MaggiorazioneConiuge

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult MaggiorazioneConiuge()
        {
            List<MaggiorazioneConiugeModel> libm = new List<MaggiorazioneConiugeModel>();

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
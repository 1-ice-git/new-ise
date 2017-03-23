using NewISE.Areas.Parametri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamAliquoteContrController : Controller
    {
        // GET: Parametri/ParamAliquoteContr/AliquoteContributive

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult AliquoteContributive()
        {
            List<AliquoteContributiveModel> libm = new List<AliquoteContributiveModel>();
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
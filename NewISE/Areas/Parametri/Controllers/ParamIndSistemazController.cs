using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamIndSistemazController : Controller
    {
        // GET: Parametri/ParamIndSistemaz/IndennitaSistemazione

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult IndennitaSistemazione()
        {
            List<IndennitaSistemazioneModel> libm = new List<IndennitaSistemazioneModel>();

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
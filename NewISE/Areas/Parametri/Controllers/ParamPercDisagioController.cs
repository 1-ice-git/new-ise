using NewISE.Areas.Parametri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamPercDisagioController : Controller
    {
        // GET: Parametri/ParamPercDisagio
        public ActionResult Index()
        {
            List<PercentualeDisagioModel> libm = new List<PercentualeDisagioModel>();
            return PartialView(libm);
        }
    }
}
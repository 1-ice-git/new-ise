using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class CoefficientiSedeController : Controller
    {
        // GET: Parametri/CoefficientiSede
        public ActionResult CoefficientiSede()
        {
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();

            try
            {
                using (dtCoefficientiSede dtib = new dtCoefficientiSede())
                {


                }
            }
            catch (Exception)
            {

                return PartialView("ErrorPartial");
            }


            return PartialView(libm);
        }
    }
}
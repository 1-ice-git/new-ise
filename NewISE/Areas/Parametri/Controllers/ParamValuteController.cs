using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamValuteController : Controller
    {
        // GET: Parametri/ParamValute/ParametriValute

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult Valute()
        {
            List<ValuteModel> libm = new List<ValuteModel>();

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
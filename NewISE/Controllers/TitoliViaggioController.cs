using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.ViewModel;

namespace NewISE.Controllers
{
    public class TitoliViaggioController : Controller
    {

        [HttpPost]
        public ActionResult TitoliViaggio(decimal idTrasferimento)
        {


            using (dtTitoliViaggi dttv = new dtTitoliViaggi())
            {

            }

            ViewData.Add("idTrasferimento", idTrasferimento);
            return PartialView();
        }




    }
}
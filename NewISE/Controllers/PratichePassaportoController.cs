using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using NewISE.Models;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.ViewModel;

namespace NewISE.Controllers
{
    public class PratichePassaportoController : Controller
    {

        public ActionResult Passaporti(decimal idTrasferimento)
        {

            ViewData.Add("idTrasferimento", idTrasferimento);
            return PartialView();

        }


        public ActionResult ElencoFamiliariPassaporti(decimal idTrasferimento)
        {
            List<ElencoFamiliariModel> lefm = new List<ElencoFamiliariModel>();

            try
            {
                using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                {
                    lefm = dtpp.GetDipendentiRichiestaPassaporto(idTrasferimento).ToList();

                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(lefm);
        }
    }
}
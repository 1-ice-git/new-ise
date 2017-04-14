using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Areas.Dipendenti.Controllers
{
    public class TrasferimentoController : Controller
    {
        // GET: Dipendenti/Trasferimento
        public ActionResult NuovoTrasferimento(string matricola)
        {
            TrasferimentoModel tm = new TrasferimentoModel();

            try
            {

            }
            catch (Exception ex)
            {

                return PartialView("errorPartial"); ;
            }

            return PartialView(tm);
        }
    }
}
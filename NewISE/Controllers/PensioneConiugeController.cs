using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Controllers
{
    public class PensioneConiugeController : Controller
    {
        public ActionResult ElencoPensioniConiuge(decimal idMaggiorazioneConiuge)
        {
            List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();

            try
            {
                using (dtPensione dtp = new dtPensione())
                {
                    lpcm = dtp.GetListaPensioneConiugeByMaggiorazioneConiuge(idMaggiorazioneConiuge).ToList();
                }

            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial");
            }

            return PartialView(lpcm);
        }
    }
}
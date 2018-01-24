using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewISE.Models.DBModel;
using NewISE.Models;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Tools;

namespace NewISE.Controllers
{
    public class AnticipiController : Controller
    {
        // GET: Anticipi
        public ActionResult Anticipi(decimal idTrasferimento)
        {
            PrimaSistemazioneModel psm = new PrimaSistemazioneModel();

            try
            {
                using (dtPrimaSistemazione dtps = new dtPrimaSistemazione())
                {
                    psm = dtps.GetPrimaSistemazioneBtIdTrasf(idTrasferimento);
                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(psm);
        }

        public ActionResult PrimaSistemazionePrevista(decimal idPrimaSistemazione)
        {

            return null;


            //CalcoliIndennita ci=new CalcoliIndennita();
        }
    }
}
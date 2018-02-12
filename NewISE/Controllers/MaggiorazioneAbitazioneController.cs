using System.Web.Routing;
using NewISE.EF;
using NewISE.Models.Tools;
using Newtonsoft.Json;
using System.IO;
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
    public class MaggiorazioneAbitazioneController : Controller
    {
        [HttpPost]
        public ActionResult MaggiorazioneAbitazione(decimal idTrasferimento)
        {
            ViewData.Add("idTrasferimento", idTrasferimento);

            return PartialView();
        }


        public ActionResult AttivitaMAB(decimal idTrasferimento)
        {
            AttivazioneMABModel amm = new AttivazioneMABModel();
            try
            {
                using (dtMaggiorazioneAbitazione dtm = new dtMaggiorazioneAbitazione())
                {
                    bool soloLettura = false;

                    amm = dtm.GetUltimaAttivitaMAB(idTrasferimento);

                    var idAttivazioneMAB = amm.idAttivazioneMAB;

                    if (amm.notificaRichiesta)
                    {
                        soloLettura = true;
                    }

                    decimal NumAttivazioni = dtm.GetNumAttivazioniMAB(idTrasferimento);

                    ViewData.Add("NumAttivazioni", NumAttivazioni);
                    ViewData.Add("soloLettura", soloLettura);
                    ViewData.Add("idAttivazioneMAB", idAttivazioneMAB);
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(amm);

            //public ActionResult AttivitaMAB(decimal idTrasferimento)
            //{
            //    AttivazioneMABModel amm = new AttivazioneMABModel();
            //    try
            //    {
            //        using (dtMaggiorazioneAbitazione dtm = new dtMaggiorazioneAbitazione())
            //        {
            //            bool soloLettura = false;

            //            amm = dtm.GetUltimaAttivitaMAB(idTrasferimento);

            //            var idAttivazioneMAB = amm.idAttivazioneMAB;

            //            if (amm.notificaRichiesta)
            //            {
            //                soloLettura = true;
            //            }

            //            decimal NumAttivazioni = dtm.GetNumAttivazioniMAB(idTrasferimento);

            //            ViewData.Add("NumAttivazioni", NumAttivazioni);
            //            ViewData.Add("soloLettura", soloLettura);
            //            ViewData.Add("idAttivazioneMAB", idAttivazioneMAB);
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            //    }

            //    return PartialView(amm);
        }
    }
}
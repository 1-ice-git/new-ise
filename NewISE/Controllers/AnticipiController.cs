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
            AttivitaAnticipiModel aam = new AttivitaAnticipiModel();

            try
            {
                using (dtPrimaSistemazione dtps = new dtPrimaSistemazione())
                {
                    psm = dtps.GetPrimaSistemazioneBtIdTrasf(idTrasferimento);
                }

                using (dtAnticipi dta = new dtAnticipi())
                {
                    aam = dta.GetAttivitaAnticipi(idTrasferimento);
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewData["idPrimaSistemazione"] = psm.idPrimaSistemazione;

            return PartialView(psm);
        }


        //public ActionResult AttivitaAnticipi(decimal idPrimaSistemazione)
        //{
        //    AttivitaAnticipiModel aam = new AttivitaAnticipiModel();

        //    try
        //    {
        //        using (dtAnticipi dta = new dtAnticipi())
        //        {
        //            var ltfm = dttf.GetListTipologiaFiglio().ToList();

        //            if (ltfm?.Any() ?? false)
        //            {
        //                r = (from t in ltfm
        //                     select new SelectListItem()
        //                     {
        //                         Text = t.tipologiaFiglio,
        //                         Value = t.idTipologiaFiglio.ToString()
        //                     }).ToList();
        //                r.Insert(0, new SelectListItem() { Text = "", Value = "" });
        //            }

        //            lTipologiaFiglio = r;
        //        }



        //    }
        //    catch (Exception ex)
        //    {
        //        return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
        //    }

        //    ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);

        //    return PartialView(fm);

        //}


        public ActionResult PrimaSistemazionePrevista(decimal idPrimaSistemazione)
        {

            return null;


            //CalcoliIndennita ci=new CalcoliIndennita();
        }
    }
}
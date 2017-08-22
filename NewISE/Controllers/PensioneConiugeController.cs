using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Controllers
{
    public class PensioneConiugeController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ElencoPensioniConiuge(decimal idConiuge)
        {
            List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();

            try
            {
                using (dtPensione dtp = new dtPensione())
                {
                    lpcm = dtp.GetPensioniByIdConiuge(idConiuge).ToList();
                }
                using (dtConiuge dtc = new dtConiuge())
                {
                    decimal idMaggiorazioniFamiliari = dtc.GetConiugebyID(idConiuge).idMaggiorazioneFamiliari;
                    ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);
                }


            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            //ViewData.Add("idTrasferimento", mcm.idTrasferimento);
            ViewData.Add("idConiuge", idConiuge);

            return PartialView(lpcm);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NuovoImportoPensione(decimal idConiuge)
        {
            //PensioneConiugeModel pcm = new PensioneConiugeModel();

            ViewData.Add("idConiuge", idConiuge);
            return PartialView();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciImportoPensione(PensioneConiugeModel pcm, decimal idConiuge)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    using (dtPensione dtp = new dtPensione())
                    {
                        pcm.dataAggiornamento = DateTime.Now;
                        pcm.annullato = false;
                        if (!pcm.dataFineValidita.HasValue)
                        {
                            pcm.dataFineValidita = Utility.DataFineStop();
                        }

                        dtp.SetNuovoImportoPensione(ref pcm, idConiuge);
                    }
                }
                else
                {
                    //ViewData.Add("idMaggiorazioneConiuge", pcm.idMaggiorazioneConiuge);
                    return PartialView("NuovoImportoPensione", pcm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return RedirectToAction("ElencoPensioniConiuge", new { idConiuge = idConiuge });
        }

        //public ActionResult ModificaPensione(decimal idPensioneConiuge)
        //{
        //    PensioneConiugeModel pcm = new PensioneConiugeModel();

        //    try
        //    {
        //        using (dtPensione dtp = new dtPensione())
        //        {
        //            pcm = dtp.GetPensioneByID(idPensioneConiuge);

        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
        //    }

        //    return PartialView(pcm);
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //[ValidateAntiForgeryToken]
        //public ActionResult ModificaImportoPensione(PensioneConiugeModel pcm, decimal idConiuge)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            using (dtPensione dtp = new dtPensione())
        //            {
        //                pcm.dataAggiornamento = DateTime.Now;
        //                pcm.annullato = false;
        //                if (!pcm.dataFineValidita.HasValue)
        //                {
        //                    pcm.dataFineValidita = Utility.DataFineStop();
        //                }

        //                dtp.EditImportoPensione(pcm, idConiuge);
        //            }
        //        }
        //        else
        //        {
        //            return PartialView("ModificaPensione", pcm);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
        //    }

        //    return RedirectToAction("ElencoPensioniConiuge", new { idConiuge = idConiuge });
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EliminaPensione(decimal idPensione, decimal idConiuge)
        {
            PensioneConiugeModel pcm = new PensioneConiugeModel();

            try
            {
                using (dtPensione dtp = new dtPensione())
                {
                    pcm = dtp.GetPensioneByID(idPensione);

                    if (pcm != null && pcm.HasValue())
                    {
                        dtp.EliminaImportoPensione(pcm, idConiuge);
                    }
                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


            return RedirectToAction("ElencoPensioniConiuge", new { idConiuge = idConiuge });
        }

    }
}
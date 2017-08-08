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
        public ActionResult ElencoPensioniConiuge(decimal idMaggiorazioneConiuge)
        {
            List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();

            try
            {
                using (dtPensione dtp = new dtPensione())
                {
                    lpcm = dtp.GetListaPensioneConiugeByMaggiorazioneConiuge(idMaggiorazioneConiuge).ToList();
                }

                using (dtMaggiorazioniFamiliari dtmc = new dtMaggiorazioniFamiliari())
                {


                    //mcm = dtmc.GetMaggiorazioneConiuge(idMaggiorazioneConiuge: idMaggiorazioneConiuge);
                }

            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial");
            }

            ViewData.Add("idTrasferimento", mcm.idTrasferimento);
            ViewData.Add("idMaggiorazioneConiuge", idMaggiorazioneConiuge);

            return PartialView(lpcm);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NuovoImportoPensione(decimal idMaggiorazioneConiuge)
        {
            //PensioneConiugeModel pcm = new PensioneConiugeModel();

            ViewData.Add("idMaggiorazioneConiuge", idMaggiorazioneConiuge);
            return PartialView();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciImportoPensione(PensioneConiugeModel pcm)
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

                        dtp.SetNuovoImportoPensione(pcm);
                    }
                }
                else
                {
                    ViewData.Add("idMaggiorazioneConiuge", pcm.idMaggiorazioneConiuge);
                    return PartialView("NuovoImportoPensione", pcm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }

            return RedirectToAction("ElencoPensioniConiuge", new { idMaggiorazioneConiuge = pcm.idMaggiorazioneConiuge });
        }

        public ActionResult ModificaPensione(decimal idPensioneConiuge)
        {
            PensioneConiugeModel pcm = new PensioneConiugeModel();

            try
            {
                using (dtPensione dtp = new dtPensione())
                {
                    pcm = dtp.GetPensioneByID(idPensioneConiuge);

                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial");
            }

            return PartialView(pcm);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult ModificaImportoPensione(PensioneConiugeModel pcm)
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

                        dtp.EditImportoPensione(pcm);
                    }
                }
                else
                {
                    return PartialView("ModificaPensione", pcm);
                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial");
            }

            return RedirectToAction("ElencoPensioniConiuge", new { idMaggiorazioneConiuge = pcm.idMaggiorazioneConiuge });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EliminaPensione(decimal idPensione)
        {
            PensioneConiugeModel pcm = new PensioneConiugeModel();

            try
            {
                using (dtPensione dtp = new dtPensione())
                {
                    pcm = dtp.GetPensioneByID(idPensione);

                    if (pcm != null && pcm.HasValue())
                    {
                        dtp.EliminaImportoPensione(pcm);
                    }
                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial");
            }


            return RedirectToAction("ElencoPensioniConiuge", new { idMaggiorazioneConiuge = pcm.idMaggiorazioneConiuge });
        }

    }
}
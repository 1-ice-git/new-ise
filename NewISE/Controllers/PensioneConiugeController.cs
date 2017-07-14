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

            ViewData.Add("idMaggiorazioneConiuge", idMaggiorazioneConiuge);

            return PartialView(lpcm);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NuovoImportoPensione(decimal idMaggiorazioneConiuge)
        {

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
                            pcm.dataFineValidita = Convert.ToDateTime("31/12/9999");
                        }

                        //if (dtp.HasPensione(idMaggiorazioneConiuge))
                        //{
                        //    var lpcm = dtp.GetListaPensioneConiugeByMaggiorazioneConiuge(idMaggiorazioneConiuge, pcm.dataInizioValidita).ToList();




                        //}
                        //else
                        //{
                        //    dtp.SetPensione(ref pcm);
                        //}

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
    }
}
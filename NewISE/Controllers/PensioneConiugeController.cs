using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.Enum;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewISE.EF;

namespace NewISE.Controllers
{
    public class PensioneConiugeController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ElencoPensioniConiuge(decimal idConiuge, decimal idAttivazioneMagFam)
        {
            List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();

            try
            {
                using (dtPensione dtp = new dtPensione())
                {
                    lpcm = dtp.GetPensioniConiuge(idConiuge, idAttivazioneMagFam).ToList();
                }
                using (dtConiuge dtc = new dtConiuge())
                {

                    ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);

                    using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                    {
                        bool rinunciaMagFam = false;
                        bool richiestaAttivazione = false;
                        bool attivazione = false;
                        bool datiConiuge = false;
                        bool datiParzialiConiuge = false;
                        bool datiFigli = false;
                        bool datiParzialiFigli = false;
                        bool siDocConiuge = false;
                        bool siDocFigli = false;
                        bool docFormulario = false;

                        bool solaLettura = false;
                        bool trasfSolaLettura = false;

                        dtmf.SituazioneMagFamPartenza(idAttivazioneMagFam, out rinunciaMagFam,
                            out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                            out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out trasfSolaLettura);

                        if (richiestaAttivazione == true || trasfSolaLettura == true)
                        {
                            solaLettura = true;
                        }
                        else
                        {
                            solaLettura = false;
                        }

                        ViewData.Add("solaLettura", solaLettura);
                    }
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
        public ActionResult NuovoImportoPensione(decimal idConiuge, decimal idAttivazioneMagFam)
        {

            try
            {

                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                {
                    var mfm = dtmf.GetMaggiorazioniFamiliaribyConiuge(idConiuge);
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var tm = dtt.GetTrasferimentoByIDMagFam(mfm.idMaggiorazioniFamiliari);

                        ViewData.Add("Trasferimento", tm);
                    }

                }

                ViewData.Add("idConiuge", idConiuge);
                ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);

                return PartialView();
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }



        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciImportoPensione(PensioneConiugeModel pcm, decimal idConiuge, decimal idAttivazioneMagFam)
        {

            try
            {

                if (ModelState.IsValid)
                {

                    using (dtPensione dtp = new dtPensione())
                    {
                        try
                        {
                            dtp.VerificaDataInizioPensione(idConiuge, pcm.dataInizioValidita);
                        }
                        catch (Exception ex)
                        {
                            ViewData.Add("idConiuge", idConiuge);
                            ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);
                            using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                            {
                                var mfm = dtmf.GetMaggiorazioniFamiliaribyConiuge(idConiuge);
                                using (dtTrasferimento dtt = new dtTrasferimento())
                                {
                                    var tm = dtt.GetTrasferimentoByIDMagFam(mfm.idMaggiorazioniFamiliari);

                                    ViewData.Add("Trasferimento", tm);
                                }

                            }
                            ModelState.AddModelError("", ex.Message);
                            return PartialView("NuovoImportoPensione", pcm);
                        }
                        pcm.dataAggiornamento = DateTime.Now;
                        pcm.idStatoRecord = (decimal)EnumStatoRecord.Annullato;
                        if (!pcm.dataFineValidita.HasValue)
                        {
                            pcm.dataFineValidita = Utility.DataFineStop();
                        }

                        dtp.SetNuovoImportoPensione(ref pcm, idConiuge, idAttivazioneMagFam);
                    }
                }
                else
                {
                    using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                    {
                        var mfm = dtmf.GetMaggiorazioniFamiliaribyConiuge(idConiuge);
                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            var tm = dtt.GetTrasferimentoByIDMagFam(mfm.idMaggiorazioniFamiliari);

                            ViewData.Add("Trasferimento", tm);
                        }

                    }
                    ViewData.Add("idConiuge", idConiuge);
                    ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);
                    return PartialView("NuovoImportoPensione", pcm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return RedirectToAction("ElencoPensioniConiuge", new { idConiuge = idConiuge, idAttivazioneMagFam = idAttivazioneMagFam });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EliminaPensione(decimal idPensione, decimal idConiuge, decimal idAttivazioneMagFam)
        {
            PensioneConiugeModel pcm = new PensioneConiugeModel();

            try
            {
                using (dtPensione dtp = new dtPensione())
                {
                    pcm = dtp.GetPensioneByID(idPensione);

                    if (pcm != null && pcm.HasValue())
                    {
                        dtp.EliminaImportoPensione(pcm, idConiuge, idAttivazioneMagFam);
                    }
                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


            return RedirectToAction("ElencoPensioniConiuge", new { idConiuge = idConiuge, idAttivazioneMagFam = idAttivazioneMagFam });
        }

    }
}
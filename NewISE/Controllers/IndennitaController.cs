using NewISE.Areas.Parametri.Models.dtObj;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Controllers
{
    public class IndennitaController : Controller
    {
        // GET: Indennita
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GestioneIndennita(decimal idTrasferimento)
        {
            try
            {
                TrasferimentoModel tm = new TrasferimentoModel();
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    

                    tm = dtt.GetTrasferimentoById(idTrasferimento);

                    var dataPartenza = tm.dataPartenza.ToShortDateString();
                    ViewData.Add("dataPartenza", dataPartenza);
                    ViewData.Add("Trasferimento", tm);
                }

                ViewBag.idTrasferimento = idTrasferimento;

                return PartialView("GestioneIndennita", tm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        public ActionResult AttivitaIndennita(decimal idTrasferimento)
        {
            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tr = dtt.GetTrasferimentoById(idTrasferimento);
                    using (dtDipendenti dtd = new dtDipendenti())
                    {
                        var d = dtd.GetDipendenteByIDTrasf(idTrasferimento);
                        if (tr != null && tr.HasValue())
                        {
                            ViewBag.idTrasferimento = tr.idTrasferimento;
                        }
                        else
                        {
                            throw new Exception("Nessun trasferimento per la matricola (" + d.matricola + ")");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });

            }
            
            return PartialView();
        }
        public JsonResult VerificaIndennita(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;

            try
            {
                if (idTrasferimento <= 0)
                {
                    throw new Exception(" non valorizzato");
                }
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    dipInfoTrasferimentoModel trm = dtt.GetInfoTrasferimento(idTrasferimento);
                    if (trm != null && (trm.statoTrasferimento == EnumStatoTraferimento.Attivo ||
                        trm.statoTrasferimento == EnumStatoTraferimento.Terminato))
                    {
                        ViewData["idTrasferimento"] = idTrasferimento;

                        return Json(new { VerificaIndennita = 1 });
                    }
                    else
                    {
                        return Json(new { VerificaIndennita = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }
        public ActionResult IndennitaBase(decimal idTrasferimento)
        {

            List<TrasferimentoModel> tm = new List<TrasferimentoModel>();

            
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();
            var r = new List<SelectListItem>();
            List<LivelloModel> llm = new List<LivelloModel>();
            List<RiduzioniModel> lrm = new List<RiduzioniModel>();

            try
            {
                // Inserire using

                //using (dtParIndennitaBase dtib = new dtParIndennitaBase())
                //{
                //    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_IndennitaBaseNonAnnullato(idLivello);
                //    libm = dtib.getListIndennitaBase(idLivello, escludiAnnullati).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                //}

                ViewBag.idTrasferimento = idTrasferimento;
                return PartialView(tm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            
        }
        public ActionResult IndennitaServizio(decimal idTrasferimento)
        {

            try
            {


                return PartialView();
            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }
        public ActionResult MaggiorazioniFamiliari(decimal idTrasferimento)
        {

            try
            {


                return PartialView();
            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }
        public ActionResult IndennitaPersonale(decimal idTrasferimento)
        {

            try
            {


                return PartialView();
            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }
        public ActionResult MaggiorazioneAbitazione(decimal idTrasferimento)
        {

            try
            {


                return PartialView();
            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }
        public ActionResult IndennitaPrimaSistemazione(decimal idTrasferimento)
        {

            try
            {


                return PartialView();
            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }

        public ActionResult IndennitadiRichiamo(decimal idTrasferimento)
        {

            try
            {


                return PartialView();
            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }

    }
}
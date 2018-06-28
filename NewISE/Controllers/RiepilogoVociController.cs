using Microsoft.Reporting.WebForms;
using NewISE.EF;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Enumeratori;
using NewISE.Models.Tools;
using NewISE.Views.Dataset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace NewISE.Controllers
{
    public class RiepilogoVociController : Controller
    {
        // GET: RiepilogoVoci
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GestioneRiepilogoVoci(decimal idTrasferimento)
        {
            try
            {
                TrasferimentoModel tm = new TrasferimentoModel();


                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    tm = dtt.GetTrasferimentoById(idTrasferimento);

                   
                }
                ViewBag.idTrasferimento = idTrasferimento;

                return PartialView("GestioneRiepilogoVoci", tm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        public JsonResult VerificaRiepilogoVoci(decimal idTrasferimento)
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

                        return Json(new { VerificaRiepilogoVoci = 1 });
                    }
                    else
                    {
                        return Json(new { VerificaRiepilogoVoci = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }
        public ActionResult AttivitaRiepilogoVoci(decimal idTrasferimento)
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
        public ActionResult ElencoRiepilogoVoci(decimal idTrasferimento)
        {
            List<RiepiloVociModel> lrvm = new List<RiepiloVociModel>();

            try
            {
                using (dtRiepilogoVoci dtrv = new dtRiepilogoVoci())
                {
                    lrvm = dtrv.GetRiepilogoVoci(idTrasferimento).ToList();
                }

                ViewData["idTrasferimento"] = idTrasferimento;
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(lrvm);
        }

        DSRiepilogoVoci DSRiepilogoVoci = new DSRiepilogoVoci();
        public ActionResult RptRiepilogoVoci(decimal idTrasferimento)
        {
            List<RiepiloVociModel> lrvm = new List<RiepiloVociModel>();
            RiepiloVociModel lrvm1 = new RiepiloVociModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var tm = dtt.GetTrasferimentoById(idTrasferimento);

                        //using (dtRiepilogoVoci dtrv = new dtRiepilogoVoci())
                        //{
                        //    lrvm1.indSistLorda = dtrv.GetRiepilogoVoci(idTrasferimento).ToList();
                        //}


                        var lTeorici =
                        db.TEORICI.Where(
                           a =>
                               a.ANNULLATO == false && a.IDINDSISTLORDA == 87 &&
                               (a.ELABINDSISTEMAZIONE.ANTICIPO == true || a.ELABINDSISTEMAZIONE.SALDO == true ||
                                a.ELABINDSISTEMAZIONE.UNICASOLUZIONE == true))
                           .OrderBy(a => a.ELABINDSISTEMAZIONE.IDPRIMASISTEMAZIONE)
                           .ToList();

                        ViewBag.idTrasferimento = idTrasferimento;

                        string Nominativo = tm.Dipendente.Nominativo;

                        if (lTeorici?.Any() ?? false)
                        {

                            foreach (var teorico in lTeorici)
                            {
                                var tr = teorico.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO;
                                var ips = teorico.ELABINDSISTEMAZIONE.IDINDSISTLORDA;
                                var voce = teorico.VOCI;
                                var tl = teorico.VOCI.TIPOLIQUIDAZIONE;
                                var tv = teorico.VOCI.TIPOVOCE;
                                

                                lrvm = (from e in lTeorici
                                        select new RiepiloVociModel()
                                        {
                                            dataOperazione = teorico.DATAOPERAZIONE,
                                            importo = teorico.IMPORTO,
                                            descrizione = teorico.VOCI.DESCRIZIONE,
                                            TipoLiquidazione = new TipoLiquidazioneModel()
                                            {
                                                idTipoLiquidazione = tl.IDTIPOLIQUIDAZIONE,
                                                descrizione = tl.DESCRIZIONE
                                            },
                                            TipoVoce = new TipoVoceModel()
                                            {
                                                idTipoVoce = tv.IDTIPOVOCE,
                                                descrizione = tv.DESCRIZIONE
                                            },
                                        }).ToList();

                            }
                        }

                        // ****************************************************************************

                        ReportViewer reportViewer = new ReportViewer();

                        reportViewer.ProcessingMode = ProcessingMode.Local;
                        reportViewer.SizeToReportContent = true;
                        reportViewer.Width = Unit.Percentage(100);
                        reportViewer.Height = Unit.Percentage(100);
                        
                        var datasource = new ReportDataSource("DSRiepilogoVoci", lTeorici.ToList());
                        reportViewer.Visible = true;
                        reportViewer.ProcessingMode = ProcessingMode.Local;
                        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptRiepilogoVoci.rdlc";
                        reportViewer.LocalReport.DataSources.Clear();
                        reportViewer.LocalReport.DataSources.Add(datasource);
                                
                        reportViewer.LocalReport.Refresh();

                        ReportParameter[] parameterValues = new ReportParameter[]
                        {
                            new ReportParameter ("Nominativo",Nominativo),
                            
                        };

                        reportViewer.LocalReport.SetParameters(parameterValues);
                        ViewBag.ReportViewer = reportViewer;
                        
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("RptRiepilogoVoci");
        }

    }
}
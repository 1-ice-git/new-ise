using Microsoft.Reporting.WebForms;
using NewISE.EF;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.dtObj;
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
            List<RiepilogoVociModel> lrvm = new List<RiepilogoVociModel>();

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
        public ActionResult RptRiepilogoVoci(decimal idTrasferimento, List<decimal> Teorici)
        {
            List<RiepilogoVociModel> lrvm = new List<RiepilogoVociModel>();
            List<RptRiepilogoVociModel> rpt = new List<RptRiepilogoVociModel>();
            
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var tm = dtt.GetTrasferimentoById(idTrasferimento);

                        string Nominativo = tm.Dipendente.Nominativo;
                    

                        using (dtRiepilogoVoci dtrv = new dtRiepilogoVoci())
                        {
                            //lrvm = dtrv.GetRiepilogoVoci(idTrasferimento).ToList();
                            lrvm = dtrv.PrelevaRiepilogoVoci(Teorici).ToList();

                        }

                        MESEANNOELABORAZIONE MeseAnnoElaborato = new MESEANNOELABORAZIONE();

                        if (lrvm?.Any() ?? false)
                        {
                            foreach (var lm in lrvm)
                            {
                                RptRiepilogoVociModel rptds = new RptRiepilogoVociModel()
                                {
                                    dataOperazione = lm.dataOperazione,
                                    descrizione = lm.descrizione,
                                    Voce = lm.Voci.codiceVoce,
                                    movimento = lm.TipoMovimento.DescMovimento,
                                    Inserimento = lm.TipoVoce.descrizione,
                                    Liquidazione = lm.TipoLiquidazione.descrizione,
                                    meseAnnoRiferimento = lm.MeseAnnoRiferimento,
                                    Importo = lm.Importo,
                                    idMeseAnnoElaborato = lm.idMeseAnnoElaborato,
                                    meseAnnoElaborazione=lm.MeseAnnoElaborato,
                                    giornoMeseAnnoElaborazione=lm.GiornoMeseAnnoElaborato
                                };

                                MeseAnnoElaborato = db.MESEANNOELABORAZIONE.Find(rptds.idMeseAnnoElaborato);

                                rpt.Add(rptds);
                            }
                        }
                       
                        decimal annoMeseElaborato =
                            Convert.ToDecimal(MeseAnnoElaborato.ANNO.ToString() + MeseAnnoElaborato.MESE.ToString().PadLeft(2, Convert.ToChar("0")));


                        ReportViewer reportViewer = new ReportViewer();

                        reportViewer.ProcessingMode = ProcessingMode.Local;
                        reportViewer.SizeToReportContent = true;
                        reportViewer.Width = Unit.Percentage(100);
                        reportViewer.Height = Unit.Percentage(100);

                        var datasource = new ReportDataSource("DtRiepilogoVoci");

                        reportViewer.Visible = true;
                        reportViewer.ProcessingMode = ProcessingMode.Local;

                        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptRiepilogoVoci.rdlc";
                        reportViewer.LocalReport.DataSources.Clear();

                        reportViewer.LocalReport.DataSources.Add(datasource);
                        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DtRiepilogoVoci", rpt));
                        reportViewer.LocalReport.Refresh();


                        ReportParameter[] parameterValues = new ReportParameter[]
                        {
                            new ReportParameter("Nominativo",Nominativo),
                            new ReportParameter("parAnnoMeseElab",Convert.ToString(annoMeseElaborato))
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
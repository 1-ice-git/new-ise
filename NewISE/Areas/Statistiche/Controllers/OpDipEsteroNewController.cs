﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewISE.Models.DBModel.dtObj;
using NewISE.EF;
using NewISE.Areas.Statistiche.Models;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using NewISE.Models;
using NewISE.Models.DBModel;

namespace NewISE.Areas.Statistiche.Controllers
{
    public class OpDipEsteroNewController : Controller
    {
        // GET: Statistiche/OpDipEsteroNew
        public ActionResult Index()
        {

            List<SelectListItem> UfficiList = new List<SelectListItem>();
            var r = new List<SelectListItem>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtUffici dtl = new dtUffici())
                    {
                        List<UfficiModel> llm = new List<UfficiModel>();
                        llm = dtl.GetUffici().OrderBy(a => a.descUfficio).ToList();
                        if (llm != null && llm.Count > 0)
                        {
                            r = (from t in llm
                                 select new SelectListItem()
                                 {
                                     Text = t.descUfficio,
                                     Value = t.idUfficio.ToString()
                                 }).ToList();

                            r.Insert(0, new SelectListItem() { Text = "", Value = "" });

                        }
                        ViewBag.UfficiList = r;
                    }

                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


            return PartialView();
        }

        public ActionResult RptOpDipEstero(string dtIni, string dtFin)
        {
            List<RiepiloghiIseMensileModel> rim = new List<RiepiloghiIseMensileModel>();
            List<RptRiepiloghiIseMensileModel> rpt = new List<RptRiepiloghiIseMensileModel>();

            try
            {

                using (ModelDBISE db = new ModelDBISE())
                {
                    //using (dtRiepiloghiIseMensile dtRiepiloghiIseMensile = new dtRiepiloghiIseMensile())
                    //{
                    //    rim = dtRiepiloghiIseMensile.GetRiepiloghiIseMensile(dtIni, dtFin, db).ToList();
                    //}

                    if (rim?.Any() ?? false)
                    {
                        foreach (var lm in rim)
                        {
                            RptRiepiloghiIseMensileModel rptds = new RptRiepiloghiIseMensileModel()
                            {
                                IdTeorici = lm.idTeorici,
                                DescrizioneVoce = lm.Voci.descrizione,
                                Nominativo = lm.Nominativo,
                                Movimento = lm.TipoMovimento.DescMovimento,
                                Liquidazione = lm.Voci.TipoLiquidazione.descrizione,
                                Voce = lm.Voci.codiceVoce,
                                Inserimento = lm.tipoInserimento.ToString(),
                                Importo = lm.Importo,
                                Inviato = lm.Elaborato,
                                meseRiferimento = lm.meseRiferimento

                            };

                            rpt.Add(rptds);
                        }
                    }



                    ReportViewer reportViewer = new ReportViewer();

                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    reportViewer.SizeToReportContent = true;
                    reportViewer.Width = Unit.Percentage(100);
                    reportViewer.Height = Unit.Percentage(100);

                    var datasource = new ReportDataSource("DataSetDipEsteroLivello");

                    reportViewer.Visible = true;
                    reportViewer.ProcessingMode = ProcessingMode.Local;

                    reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\RptDipEstero.rdlc";
                    reportViewer.LocalReport.DataSources.Clear();

                    reportViewer.LocalReport.DataSources.Add(datasource);
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetDipEsteroLivello", rpt));
                    reportViewer.LocalReport.Refresh();

                    // Nel caso in cui passo il DatePicker
                    ReportParameter[] parameterValues = new ReportParameter[]
                       {
                            new ReportParameter ("Dal",Convert.ToString(dtIni)),
                            new ReportParameter ("Al",Convert.ToString(dtFin))
                       };

                    reportViewer.LocalReport.SetParameters(parameterValues);
                    ViewBag.ReportViewer = reportViewer;

                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("RptOpDipEstero");
        }

    }
}
﻿using Microsoft.Reporting.WebForms;
using NewISE.Areas.Statistiche.Models;
using NewISE.Areas.Statistiche.Models.dtObj;
using NewISE.Areas.Statistiche.RPTDataSet;
using NewISE.EF;
using NewISE.Models;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Enumeratori;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace NewISE.Areas.Statistiche.Controllers
{
    public class RiepiloghiIseMensileController : Controller
    {
        // GET: Statistiche/RiepiloghiIseMensile
        public ActionResult Index()
        {
            
            return PartialView();
        }

        public ActionResult RptRiepiloghiIseMensile(string dtIni, string dtFin)
        {
            List<RiepiloghiIseMensileModel> rim = new List<RiepiloghiIseMensileModel>();
            List<RptRiepiloghiIseMensileModel> rpt = new List<RptRiepiloghiIseMensileModel>();

            try
            {

                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtRiepiloghiIseMensile dtRiepiloghiIseMensile = new dtRiepiloghiIseMensile())
                    {   
                        rim = dtRiepiloghiIseMensile.GetRiepiloghiIseMensile(dtIni, dtFin, db).ToList();
                    }

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
                                Inviato = lm.Elaborato
                            };

                            rpt.Add(rptds);
                        }
                    }



                }


                ReportViewer reportViewer = new ReportViewer();

                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);

                var datasource = new ReportDataSource("DataSetRiepiloghiIseMensile");

                reportViewer.Visible = true;
                reportViewer.ProcessingMode = ProcessingMode.Local;
                
                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\RptRiepiloghiIseMensile.rdlc"; 
                reportViewer.LocalReport.DataSources.Clear();

                reportViewer.LocalReport.DataSources.Add(datasource);
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetRiepiloghiIseMensile", rpt));
                reportViewer.LocalReport.Refresh();

                // Nel caso in cui le passo come Data
                //ReportParameter[] parameterValues = new ReportParameter[]
                //   {
                //        new ReportParameter ("Dal",Convert.ToString(dtIni)),
                //        new ReportParameter ("Al",Convert.ToString(dtFin))
                //   };


                ReportParameter[] parameterValues = new ReportParameter[]
                   {
                        new ReportParameter ("Dal",dtIni),
                        new ReportParameter ("Al",dtFin)
                   };

                reportViewer.LocalReport.SetParameters(parameterValues);
                ViewBag.ReportViewer = reportViewer;


            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("RptRiepiloghiIseMensile");
        }

    }
}
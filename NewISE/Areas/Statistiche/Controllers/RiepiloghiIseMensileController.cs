using Microsoft.Reporting.WebForms;
using NewISE.Areas.Statistiche.Models;
using NewISE.Areas.Statistiche.Models.dtObj;
using NewISE.Areas.Statistiche.RPTDataSet;
using NewISE.EF;
using NewISE.Models;
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

        public ActionResult RptRiepiloghiIseMensile(string V_DATA1 = "", string V_DATA2 = "")
        {
            List<RptRiepiloghiIseMensileModel> rpt = new List<RptRiepiloghiIseMensileModel>();

            try
            {

                using (ModelDBISE db = new ModelDBISE())
                {
                    var xx = db.ELABORAZIONI.ToList();

                    List<decimal> lt2 = (from t in db.TEORICI
                                         where t.ANNULLATO == false && t.INSERIMENTOMANUALE == false &&
                                               //t.IDMESEANNOELAB == idAnnoMeseElaborato &&
                                               t.ELABINDENNITA.Any(b => b.ANNULLATO == false) &&
                                               t.DIRETTO == false &&
                                               //t.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                               t.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera
                                         select t.IDTEORICI).ToList();


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

                
                ReportParameter[] parameterValues = new ReportParameter[]
                   {
                        new ReportParameter ("Dal",V_DATA1),
                        new ReportParameter ("Al",V_DATA2)
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
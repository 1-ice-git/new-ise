using Microsoft.Reporting.WebForms;
using NewISE.Areas.Statistiche.Models;
using NewISE.EF;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using NewISE.Areas.Parametri.Models.dtObj;

namespace NewISE.Areas.Statistiche.Controllers
{
    public class DipEsteroLivelloNewController : Controller
    {
        // GET: Statistiche/DipEsteroLivelloNew
        public ActionResult Index()
        {
            List<SelectListItem> LivelliList = new List<SelectListItem>();
            var r = new List<SelectListItem>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtParLivelli dtl = new dtParLivelli())
                    {
                        List<LivelloModel> llm = new List<LivelloModel>();
                        
                        llm = dtl.GetLivelli().OrderBy(a => a.DescLivello).ToList();
                        if (llm != null && llm.Count > 0)
                        {
                            r = (from t in llm
                                 select new SelectListItem()
                                 {
                                     Text = t.DescLivello,
                                     Value = t.idLivello.ToString()
                                 }).ToList();

                            r.Insert(0, new SelectListItem() { Text = "", Value = "" });

                        }

                        ViewBag.LivelliList = r;
                    }
                    
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


            return PartialView();
        }

        public ActionResult RptDipEsteroLivello(DateTime dtIni, DateTime dtFin, decimal idLivello)
        {
            List<DipEsteroLivelloNewModel> rim = new List<DipEsteroLivelloNewModel>();
            List<RptDipEsteroLivelloNewModel> rpt = new List<RptDipEsteroLivelloNewModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtDipEsteroLivello dtd = new dtDipEsteroLivello())
                    {
                        rim = dtd.DipEsteroLivelloNew(dtIni, dtFin, idLivello, db).ToList().OrderBy(a=>a.nominativo).ThenBy(a=>a.data_trasferimento).ToList();
                    }

                    if (rim?.Any() ?? false)
                    {
                        decimal ordinamento = 0;
                        foreach (var lm in rim)
                        {
                            ordinamento++;

                            RptDipEsteroLivelloNewModel rptds = new RptDipEsteroLivelloNewModel()
                            {

                                nominativo = lm.nominativo,
                                data_trasferimento = lm.data_trasferimento.ToShortDateString(),
                                data_rientro = lm.data_rientro,
                                sede = lm.sede,
                                qualifica = lm.qualifica,
                                ruolo_dipendente = lm.ruolo_dipendente,
                                ordinamento = ordinamento
                            };


                            rpt.Add(rptds);
                        }
                    }

                    string strDataOdierna = DateTime.Now.ToShortDateString();

                    ReportViewer reportViewer = new ReportViewer();

                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    reportViewer.SizeToReportContent = true;
                    reportViewer.Width = Unit.Percentage(100);
                    reportViewer.Height = Unit.Percentage(100);

                    var datasource = new ReportDataSource("DataSetDipEsteroLivello");

                    reportViewer.Visible = true;
                    reportViewer.ProcessingMode = ProcessingMode.Local;

                    reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\RptDipEsteroLivello.rdlc";
                    reportViewer.LocalReport.DataSources.Clear();

                    reportViewer.LocalReport.DataSources.Add(datasource);
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetDipEsteroLivello", rpt));
                    reportViewer.LocalReport.Refresh();

                    // Nel caso in cui passo il DatePicker
                    ReportParameter[] parameterValues = new ReportParameter[]
                       {
                            new ReportParameter ("Dal",dtIni.ToShortDateString()),
                            new ReportParameter ("Al",dtFin.ToShortDateString()),
                            new ReportParameter ("DataOdierna", strDataOdierna)
                           };

                    reportViewer.LocalReport.SetParameters(parameterValues);
                    ViewBag.ReportViewer = reportViewer;

                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("RptDipEsteroLivello");
        }

        public JsonResult ValidazioneDateEsteroLivello(DateTime dtIni, DateTime dtFin)
        {
            try
            {
                if(dtIni>dtFin)
                {
                    return Json(new { datevalide = 0, err = "" });
                }
                else
                {
                    return Json(new { datevalide = 1, err = "" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }

    }
}
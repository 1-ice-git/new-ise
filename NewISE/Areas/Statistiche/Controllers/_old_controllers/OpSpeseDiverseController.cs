using Microsoft.Reporting.WebForms;
using NewISE.Areas.Statistiche.Models;
using NewISE.Areas.Statistiche.Models.dtObj;
using NewISE.Areas.Statistiche.RPTDataSet;
using NewISE.Models;
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
    public class OpSpeseDiverseController : Controller
    {
        // GET: Statistiche/OpSpeseDiverse
        public ActionResult Index()
        {
            return View();
        }

        // Operazioni Effettuate - Spese Diverse
        public ActionResult OpSpeseDiverse(string V_DATA = "", string V_DATA1 = "")
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {

                String Sql = "Select Distinct ANADIPE.AND_COGNOME || ' ' || ANADIPE.AND_NOME AS NOMINATIVO,";
                Sql += "SPESEDIVERSE.SPD_MATRICOLA AS MATRICOLA, ";
                Sql += "SEDIESTERE.SED_DESCRIZIONE AS DESCRIZIONE, ";
                Sql += "TIPISPESE.TSP_DESCRIZIONE, ";
                Sql += "SPESEDIVERSE.SPD_DT_DECORRENZA, ";
                Sql += "SPESEDIVERSE.SPD_DT_OPERAZIONE, ";
                Sql += "SPESEDIVERSE.SPD_IMPORTO_LIRE, ";
                Sql += "SPESEDIVERSE.SPD_TIPO_MOVIMENTO, ";
                Sql += "SPESEDIVERSE.SPD_PROG_SPESA, ";
                Sql += "SPESEDIVERSE.SPD_PROG_TRASFERIMENTO ";
                Sql += "From SPESEDIVERSE, SEDIESTERE, ANADIPE, TIPISPESE ";
                Sql += "Where SPD_COD_SEDE = SED_COD_SEDE ";
                Sql += "And SPESEDIVERSE.SPD_COD_SPESA = TIPISPESE.TSP_COD_SPESA ";
                Sql += "And SPESEDIVERSE.SPD_MATRICOLA = ANADIPE.AND_MATRICOLA ";
                Sql += "And(SPD_DT_OPERAZIONE >= To_Date('" + V_DATA + "', 'DD-MM-YYYY') ";
                Sql += "And SPD_DT_OPERAZIONE <= To_Date('" + V_DATA1 + "', 'DD-MM-YYYY')) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "SPD_PROG_TRASFERIMENTO, ";
                Sql += "SPD_DT_DECORRENZA, ";
                Sql += "SPD_PROG_SPESA ";

                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Op_Spese_Diverse> model = new List<Stp_Op_Spese_Diverse>();
                while (rdr.Read())
                {
                    var details = new Stp_Op_Spese_Diverse();
                    details.NOMINATIVO = rdr["NOMINATIVO"].ToString();
                    details.MATRICOLA = rdr["MATRICOLA"].ToString();
                    details.DESCRIZIONE = rdr["DESCRIZIONE"].ToString();
                    details.TSP_DESCRIZIONE = rdr["TSP_DESCRIZIONE"].ToString();
                    //details.SPD_DT_DECORRENZA = rdr["SPD_DT_DECORRENZA"].ToString();
                    //details.SPD_DT_OPERAZIONE = rdr["SPD_DT_OPERAZIONE"].ToString();
                    details.SPD_DT_DECORRENZA = Convert.ToDateTime(rdr["SPD_DT_DECORRENZA"]).ToString("dd/MM/yyyy");
                    details.SPD_DT_OPERAZIONE = Convert.ToDateTime(rdr["SPD_DT_OPERAZIONE"]).ToString("dd/MM/yyyy");
                    details.SPD_IMPORTO_LIRE = rdr["SPD_IMPORTO_LIRE"].ToString();
                    details.SPD_TIPO_MOVIMENTO = rdr["SPD_TIPO_MOVIMENTO"].ToString();
                    details.SPD_PROG_SPESA = rdr["SPD_PROG_SPESA"].ToString();
                    details.SPD_PROG_TRASFERIMENTO = rdr["SPD_PROG_TRASFERIMENTO"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("OpSpeseDiverse", model);
            }
        }

        // Report Operazioni Effettuate - Spese Diverse
        //public ActionResult RptOpSpeseDiverse(string V_DATA = "", string V_DATA1 = "")
        //{
        //    DataSet17 ds17 = new DataSet17();
        //    try
        //    {

        //        ReportViewer reportViewer = new ReportViewer();
        //        reportViewer.ProcessingMode = ProcessingMode.Local;
        //        reportViewer.SizeToReportContent = true;
        //        reportViewer.Width = Unit.Percentage(100);
        //        reportViewer.Height = Unit.Percentage(100);


        //        var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

        //        OracleConnection conx = new OracleConnection(connectionString);
        //        #region MyRegion

        //        String Sql = "Select Distinct ANADIPE.AND_COGNOME || ' ' || ANADIPE.AND_NOME AS NOMINATIVO,";
        //        Sql += "SPESEDIVERSE.SPD_MATRICOLA AS MATRICOLA, ";
        //        Sql += "SEDIESTERE.SED_DESCRIZIONE AS DESCRIZIONE, ";
        //        Sql += "TIPISPESE.TSP_DESCRIZIONE, ";
        //        Sql += "SPESEDIVERSE.SPD_DT_DECORRENZA, ";
        //        Sql += "SPESEDIVERSE.SPD_DT_OPERAZIONE, ";
        //        Sql += "SPESEDIVERSE.SPD_IMPORTO_LIRE, ";
        //        Sql += "SPESEDIVERSE.SPD_TIPO_MOVIMENTO, ";
        //        Sql += "SPESEDIVERSE.SPD_PROG_SPESA, ";
        //        Sql += "SPESEDIVERSE.SPD_PROG_TRASFERIMENTO ";
        //        Sql += "From SPESEDIVERSE, SEDIESTERE, ANADIPE, TIPISPESE ";
        //        Sql += "Where SPD_COD_SEDE = SED_COD_SEDE ";
        //        Sql += "And SPESEDIVERSE.SPD_COD_SPESA = TIPISPESE.TSP_COD_SPESA ";
        //        Sql += "And SPESEDIVERSE.SPD_MATRICOLA = ANADIPE.AND_MATRICOLA ";
        //        Sql += "And(SPD_DT_OPERAZIONE >= To_Date('" + V_DATA + "', 'DD-MM-YYYY') ";
        //        Sql += "And SPD_DT_OPERAZIONE <= To_Date('" + V_DATA1 + "', 'DD-MM-YYYY')) ";
        //        Sql += "Order By NOMINATIVO, ";
        //        Sql += "SPD_PROG_TRASFERIMENTO, ";
        //        Sql += "SPD_DT_DECORRENZA, ";
        //        Sql += "SPD_PROG_SPESA ";
        //        #endregion

        //        OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

        //        //adp.Fill(ds17, ds17.V_OP_EFFETTUATE_SPESE_DIVERSE.TableName);
        //        adp.Fill(ds17, ds17.DataTable17.TableName);

        //        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\RptOpSpeseDiverse.rdlc";
        //        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet17", ds17.Tables[0]));

        //        ReportParameter[] parameterValues = new ReportParameter[]
        //        {
        //            new ReportParameter ("fromDate",V_DATA),
        //            new ReportParameter ("toDate",V_DATA1)
        //        };

        //        reportViewer.LocalReport.SetParameters(parameterValues);
        //        reportViewer.LocalReport.Refresh();

        //        ViewBag.ReportViewer = reportViewer;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }

        //    return View();
        //}
    }
}
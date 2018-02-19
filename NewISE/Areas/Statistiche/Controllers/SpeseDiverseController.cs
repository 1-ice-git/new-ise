using Microsoft.Reporting.WebForms;
using NewISE.Areas.Statistiche.Models;
using NewISE.Areas.Statistiche.Models.dtObj;
using NewISE.Areas.Statistiche.RPTDataSet;
using NewISE.Models;
using Oracle.ManagedDataAccess.Client;
using System;
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
    public class SpeseDiverseController : Controller
    {
        // GET: Statistiche/SpeseDiverse
        public ActionResult Index()
        {
            return View();
        }

        // Spese Diverse
        public ActionResult SpeseDiverse(string V_DATA = "", string V_DATA1 = "")
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {

                //String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                //Sql += "ANADIPE.AND_LIVELLO LIVELLO, ";
                //Sql += "SED_DESCRIZIONE SEDE, ";
                //Sql += "TSP_DESCRIZIONE, ";
                //Sql += "SPESEDIVERSE.* ";
                //Sql += "From ANADIPE, TIPISPESE, SPESEDIVERSE, SEDIESTERE ";
                //Sql += "Where TSP_COD_SPESA = SPD_COD_SPESA ";
                //Sql += "And SPD_MATRICOLA = AND_MATRICOLA ";
                //Sql += "And SPD_COD_SEDE = SED_COD_SEDE ";
                //Sql += "And (SPD_DT_DECORRENZA >= To_Date ('" + V_DATA + "', 'DD-MM-YYYY')  ";
                //Sql += "And SPD_DT_DECORRENZA <= To_Date ('" + V_DATA1 + "', 'DD-MM-YYYY'))  ";

                String Sql = "SELECT DISTINCT ANADIPE.AND_MATRICOLA AS MATRICOLA, ";
                Sql += "ANADIPE.AND_COGNOME || ' ' || ANADIPE.AND_NOME  AS NOMINATIVO, ";
                Sql += "ANADIPE.AND_LIVELLO AS LIVELLO, ";
                Sql += "SEDIESTERE.SED_COD_SEDE AS CODICE_SEDE, ";
                Sql += "SEDIESTERE.SED_DESCRIZIONE AS DESCRIZIONE_SEDE, ";
                Sql += "SPESEDIVERSE.SPD_DT_DECORRENZA AS DATA, ";
                Sql += "TIPISPESE.TSP_DESCRIZIONE AS VOCE_DI_SPESA, ";
                Sql += "SPESEDIVERSE.SPD_IMPORTO_VALUTA AS IMPORTO_VALUTA ";
                Sql += "FROM ANADIPE, ";
                Sql += "TIPISPESE, ";
                Sql += "SPESEDIVERSE, ";
                Sql += "SEDIESTERE ";
                Sql += "WHERE TSP_COD_SPESA = SPD_COD_SPESA ";
                Sql += "AND SPD_MATRICOLA = AND_MATRICOLA ";
                Sql += "AND SPD_COD_SEDE = SED_COD_SEDE ";
                Sql += "AND(SPD_DT_DECORRENZA >= To_Date ('" + V_DATA + "', 'DD-MM-YYYY') ";
                Sql += "AND SPD_DT_DECORRENZA <= To_Date ('" + V_DATA1 + "', 'DD-MM-YYYY')) ";

                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Spese_diverse> model = new List<Stp_Spese_diverse>();
                while (rdr.Read())
                {
                    var details = new Stp_Spese_diverse();
                    details.MATRICOLA = rdr["MATRICOLA"].ToString();
                    details.NOMINATIVO = rdr["NOMINATIVO"].ToString();
                    details.LIVELLO = rdr["LIVELLO"].ToString();
                    details.CODICE_SEDE = rdr["CODICE_SEDE"].ToString();
                    details.DESCRIZIONE_SEDE = rdr["DESCRIZIONE_SEDE"].ToString();
                    details.DATA = Convert.ToDateTime(rdr["DATA"]).ToString("dd/mm/yyyy");
                    details.VOCE_DI_SPESA = rdr["VOCE_DI_SPESA"].ToString();
                    details.IMPORTO_VALUTA = rdr["IMPORTO_VALUTA"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("SpeseDiverse", model);
            }
        }

        // Report Spese Diverse
        public ActionResult RptSpeseDiverse(string V_DATA = "", string V_DATA1 = "")
        {
            DataSet18 ds18 = new DataSet18();

            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);


                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                OracleConnection conx = new OracleConnection(connectionString);


                //String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                //Sql += "ANADIPE.AND_LIVELLO LIVELLO, ";
                //Sql += "SED_DESCRIZIONE SEDE, ";
                //Sql += "TSP_DESCRIZIONE, ";
                //Sql += "SPESEDIVERSE.* ";
                //Sql += "From ANADIPE, TIPISPESE, SPESEDIVERSE, SEDIESTERE ";
                //Sql += "Where TSP_COD_SPESA = SPD_COD_SPESA ";
                //Sql += "And SPD_MATRICOLA = AND_MATRICOLA ";
                //Sql += "And SPD_COD_SEDE = SED_COD_SEDE ";
                //Sql += "And (SPD_DT_DECORRENZA >= To_Date ('" + V_DATA + "', 'DD-MM-YYYY')  ";
                //Sql += "And SPD_DT_DECORRENZA <= To_Date ('" + V_DATA1 + "', 'DD-MM-YYYY'))  ";

                #region MyRegion
                String Sql = "SELECT DISTINCT ANADIPE.AND_MATRICOLA AS MATRICOLA, ";
                Sql += "ANADIPE.AND_COGNOME || ' ' || ANADIPE.AND_NOME  AS NOMINATIVO, ";
                Sql += "ANADIPE.AND_LIVELLO AS LIVELLO, ";
                Sql += "SEDIESTERE.SED_COD_SEDE AS CODICE_SEDE, ";
                Sql += "SEDIESTERE.SED_DESCRIZIONE AS DESCRIZIONE_SEDE, ";
                Sql += "SPESEDIVERSE.SPD_DT_DECORRENZA AS DATA, ";
                Sql += "TIPISPESE.TSP_DESCRIZIONE AS VOCE_DI_SPESA, ";
                Sql += "SPESEDIVERSE.SPD_IMPORTO_VALUTA AS IMPORTO_VALUTA ";
                Sql += "FROM ANADIPE, ";
                Sql += "TIPISPESE, ";
                Sql += "SPESEDIVERSE, ";
                Sql += "SEDIESTERE ";
                Sql += "WHERE TSP_COD_SPESA = SPD_COD_SPESA ";
                Sql += "AND SPD_MATRICOLA = AND_MATRICOLA ";
                Sql += "AND SPD_COD_SEDE = SED_COD_SEDE ";
                Sql += "AND(SPD_DT_DECORRENZA >= To_Date ('" + V_DATA + "', 'DD-MM-YYYY') ";
                Sql += "AND SPD_DT_DECORRENZA <= To_Date ('" + V_DATA1 + "', 'DD-MM-YYYY')) ";
                Sql += "ORDER BY NOMINATIVO ";
                #endregion

                //string sql = "";
                //sql += "SELECT * FROM TABLE WHERE NAME='JOHN SMITH'";
                //OdbcDataAdapter adptr = new OdbcDataAdapter(sql, _connection);
                //DataSet ds = new DataSet();
                //adptr.Fill(ds);
                //return ds;

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);
                adp.Fill(ds18, ds18.DataTable1.TableName);


                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\RptSpeseDiverse.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet18", ds18.Tables[0]));

                ReportParameter[] parameterValues = new ReportParameter[]
                {
                    new ReportParameter ("fromDate",V_DATA),
                    new ReportParameter ("toDate",V_DATA1)
                };

                reportViewer.LocalReport.SetParameters(parameterValues);
                reportViewer.LocalReport.Refresh();

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return View();
        }
    }
}
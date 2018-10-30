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
    public class SpeseAvvicendamentoController : Controller
    {
        // GET: Statistiche/SpeseAvvicendamento
        public ActionResult Index()
        {
            return View();
        }

        // Spese Avvicendamento
        public ActionResult SpeseAvvicendamento(string V_DATA = "", string V_DATA1 = "")
        {

            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {

                String Sql = "SELECT DISTINCT SPESEDIVERSE.SPD_MATRICOLA AS MATRICOLA, ";
                Sql += "ANADIPE.AND_COGNOME || ' ' || ANADIPE.AND_NOME AS NOMINATIVO, ";
                Sql += "ANADIPE.AND_LIVELLO AS LIVELLO, ";
                Sql += "SPESEDIVERSE.SPD_COD_SEDE AS CODICE_SEDE, ";
                Sql += "SEDIESTERE.SED_DESCRIZIONE AS DESCRIZIONE_SEDE, ";
                Sql += "SPESEDIVERSE.SPD_DT_DECORRENZA AS DATA, ";
                Sql += "TIPISPESE.TSP_DESCRIZIONE AS SPESA, ";
                Sql += "SPESEDIVERSE.SPD_IMPORTO_VALUTA AS INDENITA_IN_VALUTA ";
                //Sql += "SPESEDIVERSE.* ";
                Sql += "FROM ANADIPE, ";
                Sql += "TIPISPESE, ";
                Sql += "SPESEDIVERSE, ";
                Sql += "SEDIESTERE ";
                Sql += "WHERE TSP_COD_SPESA = SPD_COD_SPESA ";
                Sql += "AND SPD_MATRICOLA = AND_MATRICOLA ";
                Sql += "AND SPD_COD_SEDE = SED_COD_SEDE ";
                Sql += "AND(SPD_DT_DECORRENZA >= To_Date('" + V_DATA + "', 'DD-MM-YYYY')";
                Sql += "AND SPD_DT_DECORRENZA <= To_Date('" + V_DATA1 + "', 'DD-MM-YYYY')) ";
                Sql += "AND TSP_COD_SPESA NOT IN (1) ";

                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Spese__di_avvicendamento> model = new List<Stp_Spese__di_avvicendamento>();
                while (rdr.Read())
                {
                    var details = new Stp_Spese__di_avvicendamento();
                    details.MATRICOLA = rdr["MATRICOLA"].ToString();
                    details.NOMINATIVO = rdr["NOMINATIVO"].ToString();
                    details.LIVELLO = rdr["LIVELLO"].ToString();
                    details.CODICE_SEDE = rdr["CODICE_SEDE"].ToString();
                    details.DESCRIZIONE_SEDE = rdr["DESCRIZIONE_SEDE"].ToString();
                    //details.DATA = rdr["DATA"].ToString();
                    details.DATA = Convert.ToDateTime(rdr["DATA"]).ToString("dd/MM/yyyy");
                    details.SPESA = rdr["SPESA"].ToString();
                    details.INDENITA_IN_VALUTA = rdr["INDENITA_IN_VALUTA"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("SpeseAvvicendamento", model);
            }
        }

        // Report Spese Avvicendamento
        //public ActionResult RptSpeseAvvicendamento(string V_DATA = "", string V_DATA1 = "")
        //{
        //    DataSet12 ds12 = new DataSet12();
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


        //        String Sql = "SELECT DISTINCT SPESEDIVERSE.SPD_MATRICOLA AS MATRICOLA, ";
        //        Sql += "ANADIPE.AND_COGNOME || ' ' || ANADIPE.AND_NOME AS NOMINATIVO, ";
        //        Sql += "ANADIPE.AND_LIVELLO AS LIVELLO, ";
        //        Sql += "SPESEDIVERSE.SPD_COD_SEDE AS CODICE_SEDE, ";
        //        Sql += "SEDIESTERE.SED_DESCRIZIONE AS DESCRIZIONE_SEDE, ";
        //        Sql += "SPESEDIVERSE.SPD_DT_DECORRENZA AS DATA, ";
        //        Sql += "TIPISPESE.TSP_DESCRIZIONE AS SPESA, ";
        //        Sql += "SPESEDIVERSE.SPD_IMPORTO_VALUTA AS INDENITA_IN_VALUTA ";
        //        //Sql += "--SPESEDIVERSE.* ";
        //        Sql += "FROM ANADIPE, ";
        //        Sql += "TIPISPESE, ";
        //        Sql += "SPESEDIVERSE, ";
        //        Sql += "SEDIESTERE ";
        //        Sql += "WHERE TSP_COD_SPESA = SPD_COD_SPESA ";
        //        Sql += "AND SPD_MATRICOLA = AND_MATRICOLA ";
        //        Sql += "AND SPD_COD_SEDE = SED_COD_SEDE ";
        //        Sql += "AND(SPD_DT_DECORRENZA >= To_Date('" + V_DATA + "', 'DD-MM-YYYY')";
        //        Sql += "AND SPD_DT_DECORRENZA <= To_Date('" + V_DATA1 + "', 'DD-MM-YYYY')) ";
        //        Sql += "AND TSP_COD_SPESA NOT IN (1) ";
        //        Sql += "ORDER BY NOMINATIVO ASC ";
        //        #endregion


        //        // query originale

        //        //Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO,
        //        //       ANADIPE.AND_LIVELLO,
        //        //       SED_DESCRIZIONE,
        //        //       TSP_DESCRIZIONE,
        //        //       SPESEDIVERSE.*
        //        //From ANADIPE, TIPISPESE, SPESEDIVERSE, SEDIESTERE
        //        //Where TSP_COD_SPESA = SPD_COD_SPESA
        //        //And SPD_MATRICOLA = AND_MATRICOLA
        //        //And SPD_COD_SEDE = SED_COD_SEDE
        //        //And(SPD_DT_DECORRENZA >= To_Date('01-gen-2017', 'DD-MON-RRRR') And
        //        //   SPD_DT_DECORRENZA <= To_Date('31-dic-2017', 'DD-MON-RRRR'))
        //        //   And TSP_COD_SPESA Not In (1)
        //        //ORDER BY NOMINATIVO ASC


        //        OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

        //        //adp.Fill(ds12, ds12.V_ISE_STP_CONS_SPESE_AVVICE.TableName);
        //        adp.Fill(ds12, ds12.DataTable12.TableName);


        //        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\RptSpeseAvvicendamento.rdlc";
        //        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet12", ds12.Tables[0]));

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
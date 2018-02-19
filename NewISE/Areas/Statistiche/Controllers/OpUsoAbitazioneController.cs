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
    public class OpUsoAbitazioneController : Controller
    {
        // GET: Statistiche/OpUsoAbitazione
        public ActionResult Index()
        {
            return View();
        }

        // Operazioni Effettuate - Uso Abitazione
        public ActionResult OpUsoAbitazione(string V_DATA = "", string V_DATA1 = "")
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {

                String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                Sql += "USO_MATRICOLA AS MATRICOLA, ";
                Sql += "SED_DESCRIZIONE AS SEDE, ";
                Sql += "VAL_DESCRIZIONE AS VALUTA, ";
                Sql += "USO_DT_DECORRENZA AS DATA_DECORRENZA, ";
                Sql += "USO_DT_LETTERA AS DATA_LETTERA, ";
                Sql += "USO_DT_OPERAZIONE AS DATA_OPERAZIONE, ";
                Sql += "USO_CANONE_VALUTA AS CANONE_VALUTA, ";
                Sql += "(USO_CANONE_VALUTA / USO_CAMBIO_VALUTA_CANONE) CANONE, ";
                Sql += "USO_IMPONIBILE_PREV AS IMPONIBILE_PREVIDENZIALE, ";
                Sql += "USO_PROG_USO_ABITAZIONE, ";
                Sql += "USO_PROG_TRASFERIMENTO ";
                Sql += "From USOABITAZIONE, SEDIESTERE, VALUTE, ANADIPE ";
                Sql += "Where USO_COD_SEDE = SED_COD_SEDE ";
                Sql += "And USO_VALUTA_CANONE = VAL_COD_VALUTA ";
                Sql += "And USO_MATRICOLA = AND_MATRICOLA ";
                Sql += "And(USO_DT_OPERAZIONE >= To_Date ('" + V_DATA + "','DD-MM-YYYY') ";
                Sql += "And USO_DT_OPERAZIONE <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "USO_PROG_TRASFERIMENTO, ";
                Sql += "USO_DT_DECORRENZA, ";
                Sql += "USO_PROG_USO_ABITAZIONE ";


                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Op_Uso_Abitazione> model = new List<Stp_Op_Uso_Abitazione>();
                while (rdr.Read())
                {
                    var details = new Stp_Op_Uso_Abitazione();
                    details.matricola = rdr["MATRICOLA"].ToString();
                    details.nominativo = rdr["NOMINATIVO"].ToString();
                    details.sede = rdr["SEDE"].ToString();
                    details.valuta = rdr["VALUTA"].ToString();
                    //details.data_decorrenza = Convert.ToDateTime(rdr["DATA_DECORRENZA"]).ToString("dd/MM/yyyy");
                    //details.data_lettera = Convert.ToDateTime(rdr["DATA_LETTERA"]).ToString("dd/MM/yyyy");
                    //details.data_operazione = Convert.ToDateTime(rdr["DATA_OPERAZIONE"]).ToString("dd/MM/yyyy");
                    details.data_decorrenza = rdr["DATA_DECORRENZA"] == DBNull.Value ? null : Convert.ToDateTime(rdr["DATA_DECORRENZA"]).ToString("dd/MM/yyyy");
                    //details.data_decorrenza = rdr["DATA_DECORRENZA"].ToString();
                    //details.data_lettera = rdr["DATA_LETTERA"].ToString();
                    details.data_lettera = rdr["DATA_LETTERA"] == DBNull.Value ? null : Convert.ToDateTime(rdr["DATA_LETTERA"]).ToString("dd/MM/yyyy");
                    //details.data_operazione = rdr["DATA_OPERAZIONE"].ToString();
                    details.data_operazione = rdr["DATA_OPERAZIONE"] == DBNull.Value ? null : Convert.ToDateTime(rdr["DATA_OPERAZIONE"]).ToString("dd/MM/yyyy");
                    details.canone_in_valuta = rdr["CANONE_VALUTA"].ToString();
                    //details.canone_in_euro = rdr["CANONE"].ToString();
                    details.imponibile_previdenziale = rdr["IMPONIBILE_PREVIDENZIALE"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("OpUsoAbitazione", model);
            }
        }

        // Report Operazioni Effettuate - Uso Abitazione
        public ActionResult RptOpUsoAbitazione(string V_DATA = "", string V_DATA1 = "")
        {
            DataSet1 ds1 = new DataSet1();
            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);


                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                OracleConnection conx = new OracleConnection(connectionString);
                #region MyRegion

                String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                Sql += "USO_MATRICOLA AS MATRICOLA, ";
                Sql += "SED_DESCRIZIONE AS SEDE, ";
                Sql += "VAL_DESCRIZIONE AS VALUTA, ";
                Sql += "USO_DT_DECORRENZA AS DATA_DECORRENZA, ";
                Sql += "USO_DT_LETTERA AS DATA_LETTERA, ";
                Sql += "USO_DT_OPERAZIONE AS DATA_OPERAZIONE, ";
                Sql += "USO_CANONE_VALUTA AS CANONE_VALUTA, ";
                Sql += "ROUND (USO_CANONE_VALUTA / USO_CAMBIO_VALUTA_CANONE, 6) CANONE, ";
                Sql += "USO_IMPONIBILE_PREV AS IMPONIBILE_PREVIDENZIALE, ";
                Sql += "USO_PROG_USO_ABITAZIONE, ";
                Sql += "USO_PROG_TRASFERIMENTO ";
                Sql += "From USOABITAZIONE, SEDIESTERE, VALUTE, ANADIPE ";
                Sql += "Where USO_COD_SEDE = SED_COD_SEDE ";
                Sql += "And USO_VALUTA_CANONE = VAL_COD_VALUTA ";
                Sql += "And USO_MATRICOLA = AND_MATRICOLA ";
                Sql += "And(USO_DT_OPERAZIONE >= To_Date ('" + V_DATA + "','DD-MM-YYYY') ";
                Sql += "And USO_DT_OPERAZIONE <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "USO_PROG_TRASFERIMENTO, ";
                Sql += "USO_DT_DECORRENZA, ";
                Sql += "USO_PROG_USO_ABITAZIONE ";
                #endregion

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                //adp.Fill(ds1, ds1.V_OP_EFFETTUATE_USO_ABITAZ.TableName);
                adp.Fill(ds1, ds1.DataTable1.TableName);


                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\RptOpUsoAbitazione.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", ds1.Tables[0]));

                ReportParameter[] parameterValues = new ReportParameter[]
               {
                    new ReportParameter ("fromDate",V_DATA),
                    new ReportParameter ("toDate",V_DATA1)
               };

                reportViewer.LocalReport.SetParameters(parameterValues);
                reportViewer.LocalReport.Refresh();

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }

    }
}
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
    public class OpCanoneAnticipatoController : Controller
    {
        // GET: Statistiche/OpCanoneAnticipato
        public ActionResult Index()
        {
            return View();
        }

        // Operazioni Effettuate - Canone Anticipato
        public ActionResult OpCanoneAnticipato(string V_DATA = "", string V_DATA1 = "")
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {

                String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME AS NOMINATIVO, ";
                Sql += "CAN_MATRICOLA AS MATRICOLA, ";
                Sql += "SED_DESCRIZIONE AS SEDE, ";
                Sql += "VAL_DESCRIZIONE AS VALUTA, ";
                Sql += "CAN_DT_DECORRENZA AS DATA_DECORRENZA, ";
                Sql += "CAN_DT_LETTERA AS DATA_LETTERA, ";
                Sql += "CAN_DT_OPERAZIONE AS DATA_OPERAZIONE, ";
                Sql += "CAN_CANONE_ANNUO_VALUTA, ";
                Sql += "DECODE(CAN_CAMBIO_VALUTA_CANONE,0,0, ";
                Sql += "CAN_CANONE_ANNUO_VALUTA / CAN_CAMBIO_VALUTA_CANONE) CANONE, ";
                //Sql += "- (DECODE(CAN_CAMBIO_VALUTA_CANONE,0,0, ";
                //Sql += "CAN_CANONE_ANNUO_VALUTA / CAN_CAMBIO_VALUTA_CANONE) / CAN_N_MESI) AS QUOTA_MENS, ";
                Sql += "DECODE(CAN_CAMBIO_VALUTA_CANONE, 0, 0, CAN_CANONE_ANNUO_VALUTA / CAN_CAMBIO_VALUTA_CANONE / CAN_N_MESI) AS QUOTA_MENS, ";
                Sql += "CAN_PROG_TRASFERIMENTO, ";
                Sql += "CAN_PROG_CAN_ABITAZIONE ";
                Sql += "From CANONEANNUO, SEDIESTERE, VALUTE, ANADIPE ";
                Sql += "Where CAN_COD_SEDE = SED_COD_SEDE ";
                Sql += "And CAN_VALUTA_CANONE = VAL_COD_VALUTA ";
                Sql += "And CAN_MATRICOLA = AND_MATRICOLA ";
                Sql += "And(CAN_DT_OPERAZIONE >= To_Date ('" + V_DATA + "','DD-MM-YYYY') ";
                Sql += "And CAN_DT_OPERAZIONE <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "CAN_PROG_TRASFERIMENTO, ";
                Sql += "CAN_DT_DECORRENZA, ";
                Sql += "CAN_PROG_CAN_ABITAZIONE ";


                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Op_Canone_Anticipato> model = new List<Stp_Op_Canone_Anticipato>();
                while (rdr.Read())
                {
                    var details = new Stp_Op_Canone_Anticipato();
                    details.matricola = rdr["MATRICOLA"].ToString();
                    details.nominativo = rdr["NOMINATIVO"].ToString();
                    details.sede = rdr["SEDE"].ToString();
                    details.valuta = rdr["VALUTA"].ToString();
                    details.anticipo_valuta = rdr["CANONE"].ToString();
                    details.anticipo_euro = rdr["CANONE"].ToString();
                    //details.quota_mensile = rdr["QUOTA_MENS"].ToString();
                    //details.quota_mensile = Convert.ToDecimal(rdr["QUOTA_MENS"].ToString());
                    details.data_decorrenza = rdr["DATA_DECORRENZA"] == DBNull.Value ? null : Convert.ToDateTime(rdr["DATA_DECORRENZA"]).ToString("dd/MM/yyyy");
                    details.data_lettera = rdr["DATA_LETTERA"] == DBNull.Value ? null : Convert.ToDateTime(rdr["DATA_LETTERA"]).ToString("dd/MM/yyyy");
                    details.data_operazione = rdr["DATA_OPERAZIONE"] == DBNull.Value ? null : Convert.ToDateTime(rdr["DATA_OPERAZIONE"]).ToString("dd/MM/yyyy");
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("OpCanoneAnticipato", model);


            }
        }

        // Report Operazioni Effettuate - Canone Anticipato
        public ActionResult RptOpCanoneAnticipato(string V_DATA = "", string V_DATA1 = "")
        {
            //DataSet14 ds14 = new DataSet14();
            DataSet22 ds22 = new DataSet22();
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

                String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME AS NOMINATIVO, ";
                Sql += "CAN_MATRICOLA AS MATRICOLA, ";
                Sql += "SED_DESCRIZIONE AS SEDE, ";
                Sql += "VAL_DESCRIZIONE AS VALUTA, ";
                Sql += "CAN_DT_DECORRENZA AS DATA_DECORRENZA, ";
                Sql += "CAN_DT_LETTERA AS DATA_LETTERA, ";
                Sql += "CAN_DT_OPERAZIONE AS DATA_OPERAZIONE, ";
                Sql += "CAN_CANONE_ANNUO_VALUTA, ";
                Sql += "DECODE(CAN_CAMBIO_VALUTA_CANONE,0,0, ";
                Sql += "CAN_CANONE_ANNUO_VALUTA / CAN_CAMBIO_VALUTA_CANONE) CANONE, ";
                Sql += "- ROUND((DECODE(CAN_CAMBIO_VALUTA_CANONE,0,0, ";
                Sql += "CAN_CANONE_ANNUO_VALUTA / CAN_CAMBIO_VALUTA_CANONE) / CAN_N_MESI),2) AS QUOTA_MENS, ";
                Sql += "CAN_PROG_TRASFERIMENTO, ";
                Sql += "CAN_PROG_CAN_ABITAZIONE ";
                Sql += "From CANONEANNUO, SEDIESTERE, VALUTE, ANADIPE ";
                Sql += "Where CAN_COD_SEDE = SED_COD_SEDE ";
                Sql += "And CAN_VALUTA_CANONE = VAL_COD_VALUTA ";
                Sql += "And CAN_MATRICOLA = AND_MATRICOLA ";
                Sql += "And(CAN_DT_OPERAZIONE >= To_Date ('" + V_DATA + "','DD-MM-YYYY') ";
                Sql += "And CAN_DT_OPERAZIONE <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "CAN_PROG_TRASFERIMENTO, ";
                Sql += "CAN_DT_DECORRENZA, ";
                Sql += "CAN_PROG_CAN_ABITAZIONE ";
                #endregion


                //String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                //Sql += "CAN_MATRICOLA AS MATRICOLA, ";
                //Sql += "SED_DESCRIZIONE AS SEDE, ";
                //Sql += "VAL_DESCRIZIONE AS VALUTA, ";
                //Sql += "CAN_DT_DECORRENZA AS DATA_DECORRENZA, "; 
                //Sql += "CAN_DT_LETTERA AS DATA_DECORRENZA, ";
                //Sql += "CAN_DT_OPERAZIONE AS DATA_OPERAZIONE, ";
                //Sql += "CAN_CANONE_ANNUO_VALUTA, ";
                //Sql += "DECODE(CAN_CAMBIO_VALUTA_CANONE, ";
                //Sql += "0, ";
                //Sql += "0, ";
                //Sql += "CAN_CANONE_ANNUO_VALUTA / CAN_CAMBIO_VALUTA_CANONE) CANONE, ";
                //Sql += "- (DECODE(CAN_CAMBIO_VALUTA_CANONE, ";
                //Sql += "0, ";
                //Sql += "0, ";
                //Sql += "CAN_CANONE_ANNUO_VALUTA / CAN_CAMBIO_VALUTA_CANONE) / ";
                //Sql += "CAN_N_MESI) QUOTA_MENS, ";
                //Sql += "CAN_PROG_TRASFERIMENTO, ";
                //Sql += "CAN_PROG_CAN_ABITAZIONE ";
                //Sql += "From CANONEANNUO, SEDIESTERE, VALUTE, ANADIPE ";
                //Sql += "Where CAN_COD_SEDE = SED_COD_SEDE ";
                //Sql += "And CAN_VALUTA_CANONE = VAL_COD_VALUTA ";
                //Sql += "And CAN_MATRICOLA = AND_MATRICOLA ";
                //Sql += "And(CAN_DT_OPERAZIONE >= To_Date ('" + V_DATA + "','DD-MM-YYYY') And ";
                //Sql += "CAN_DT_OPERAZIONE <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                //Sql += "Order By NOMINATIVO, ";
                //Sql += "CAN_PROG_TRASFERIMENTO, ";
                //Sql += "CAN_DT_DECORRENZA, ";
                //Sql += "CAN_PROG_CAN_ABITAZIONE ";



                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                //adp.Fill(ds14, ds14.V_OP_EFFETTUATE_CANONE_ANTI.TableName);
                adp.Fill(ds22, ds22.DataTable1.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\RptOpCanoneAnticipato.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet22", ds22.Tables[0]));

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
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
    public class OpIndEsteraController : Controller
    {
        // GET: Statistiche/OpIndEstera
        public ActionResult Index()
        {
            return View();
        }

        // Operazioni Effettuate - Indennità di Sede Estera
        public ActionResult OpIndennitaEstera(string V_DATA = "", string V_DATA1 = "")
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                Sql += "IES_MATRICOLA MATRICOLA, ";
                Sql += "IES_COD_QUALIFICA QUALIFICA, ";
                Sql += "SED_DESCRIZIONE SEDE, ";
                Sql += "VAL_DESCRIZIONE VALUTA, ";
                Sql += "TMO_DESCRIZIONE_MOVIMENTO TIPO_MOVIMENTO, ";
                Sql += "IES_COD_TIPO_MOVIMENTO CODICE_TIPO_MOVIMENTO, ";
                Sql += "IES_DT_DECORRENZA DATA_DECORRENZA, ";
                Sql += "IES_DT_LETTERA DATA_LETTERA, ";
                Sql += "IES_DT_OPERAZIONE DATA_OPERAZIONE, ";
                Sql += "decode(IES_flag_valuta, ";
                Sql += "'E', ";
                Sql += "IES_INDEN_PERS * IES_CAMBIO, ";
                Sql += "IES_INDEN_PERS / IES_CAMBIO) ISEP, ";
                Sql += "IES_INDEN_SIS_RIE SISTEMAZIONE_RIENTRO, ";
                Sql += "IES_ANTICIPO ANTICIPO, ";
                Sql += "decode(IES_flag_valuta, ";
                Sql += "'E', ";
                Sql += "IES_INDEN_SIS_RIE_NETTA * IES_CAMBIO, ";
                Sql += "IES_INDEN_SIS_RIE_NETTA / IES_CAMBIO) SISRIENETTA, ";
                Sql += "IES_PROG_TRASFERIMENTO, ";
                Sql += "IES_PROG_MOVIMENTO ";
                Sql += "From INDESTERA, TIPOMOVIMENTO, SEDIESTERE, VALUTE, ANADIPE ";
                Sql += "Where IES_COD_SEDE = SED_COD_SEDE ";
                Sql += "And IES_COD_VALUTA = VAL_COD_VALUTA ";
                Sql += "And IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO ";
                Sql += "And IES_MATRICOLA = AND_MATRICOLA ";
                Sql += "And(IES_DT_OPERAZIONE >= To_Date ('" + V_DATA + "','DD-MM-YYYY')  ";
                Sql += "And IES_DT_OPERAZIONE <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "IES_PROG_TRASFERIMENTO, ";
                Sql += "IES_COD_TIPO_MOVIMENTO, ";
                Sql += "IES_DT_DECORRENZA, ";
                Sql += "IES_PROG_MOVIMENTO ";


                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Op_Indennita_Estera> model = new List<Stp_Op_Indennita_Estera>();
                while (rdr.Read())
                {
                    var details = new Stp_Op_Indennita_Estera();
                    details.matricola = rdr["MATRICOLA"].ToString();
                    details.nominativo = rdr["NOMINATIVO"].ToString();
                    details.qualifica = rdr["QUALIFICA"].ToString();
                    details.sede = rdr["SEDE"].ToString();
                    details.valuta = rdr["VALUTA"].ToString();
                    details.tipo_movimento = rdr["TIPO_MOVIMENTO"].ToString();
                    details.codice_tipo_movimento = rdr["CODICE_TIPO_MOVIMENTO"].ToString();
                    details.data_decorrenza = Convert.ToDateTime(rdr["DATA_DECORRENZA"]).ToString("dd/MM/yyyy");
                    details.data_lettera = Convert.ToDateTime(rdr["DATA_LETTERA"]).ToString("dd/MM/yyyy");
                    details.data_operazione = Convert.ToDateTime(rdr["DATA_OPERAZIONE"]).ToString("dd/MM/yyyy");
                    details.indennita_personale = rdr["ISEP"].ToString();
                    details.sist_rientro_lorda = rdr["SISTEMAZIONE_RIENTRO"].ToString();
                    details.anticipo = rdr["ANTICIPO"].ToString();
                    model.Add(details);
                }
                
                return PartialView("OpIndennitaEstera", model);


            }
        }
                
        // Report Operazioni Effettuate - Indennità di Sede Estera
        public ActionResult RptOpIndennitaEstera(string V_DATA = "", string V_DATA1 = "")
        {
            DataSet7 ds7 = new DataSet7();
            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);


                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                OracleConnection conx = new OracleConnection(connectionString);
                #region RptOpIndennitaEstera

                String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                Sql += "IES_MATRICOLA MATRICOLA, ";
                Sql += "IES_COD_QUALIFICA QUALIFICA, ";
                Sql += "SED_DESCRIZIONE SEDE, ";
                Sql += "VAL_DESCRIZIONE VALUTA, ";
                Sql += "TMO_DESCRIZIONE_MOVIMENTO TIPO_MOVIMENTO, ";
                Sql += "IES_COD_TIPO_MOVIMENTO CODICE_TIPO_MOVIMENTO, ";
                Sql += "IES_DT_DECORRENZA DATA_DECORRENZA, ";
                Sql += "IES_DT_LETTERA DATA_LETTERA, ";
                Sql += "IES_DT_OPERAZIONE DATA_OPERAZIONE, ";
                Sql += "decode(IES_flag_valuta, ";
                Sql += "'E', ";
                Sql += "IES_INDEN_PERS * IES_CAMBIO, ";
                Sql += "IES_INDEN_PERS / IES_CAMBIO) ISEP, ";
                Sql += "IES_INDEN_SIS_RIE SISTEMAZIONE_RIENTRO, ";
                Sql += "IES_ANTICIPO ANTICIPO, ";
                Sql += "decode(IES_flag_valuta, ";
                Sql += "'E', ";
                Sql += "IES_INDEN_SIS_RIE_NETTA * IES_CAMBIO, ";
                Sql += "IES_INDEN_SIS_RIE_NETTA / IES_CAMBIO) SISRIENETTA, ";
                Sql += "IES_PROG_TRASFERIMENTO, ";
                Sql += "IES_PROG_MOVIMENTO ";
                Sql += "From INDESTERA, TIPOMOVIMENTO, SEDIESTERE, VALUTE, ANADIPE ";
                Sql += "Where IES_COD_SEDE = SED_COD_SEDE ";
                Sql += "And IES_COD_VALUTA = VAL_COD_VALUTA ";
                Sql += "And IES_COD_TIPO_MOVIMENTO = TMO_COD_TIPO_MOVIMENTO ";
                Sql += "And IES_MATRICOLA = AND_MATRICOLA ";
                Sql += "And(IES_DT_OPERAZIONE >= To_Date ('" + V_DATA + "','DD-MM-YYYY')  ";
                Sql += "And IES_DT_OPERAZIONE <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "IES_PROG_TRASFERIMENTO, ";
                Sql += "IES_COD_TIPO_MOVIMENTO, ";
                Sql += "IES_DT_DECORRENZA, ";
                Sql += "IES_PROG_MOVIMENTO ";
                #endregion

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                //adp.Fill(ds7, ds7.V_OP_EFFETTUATE_IND_ESTERA.TableName);
                adp.Fill(ds7, ds7.DataTable7.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\RptOpIndEstera.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet7", ds7.Tables[0]));

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
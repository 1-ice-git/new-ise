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
    public class OpMaggiorazioneAbitazioneController : Controller
    {
        // GET: Statistiche/OpMaggiorazioneAbitazione
        public ActionResult Index()
        {
            return View();
        }

        // Operazioni Effettuate - Maggiorazione Abitazione
        public ActionResult OpMaggiorazioneAbitazione(string V_DATA = "", string V_DATA1 = "")
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                String Sql = "SELECT DISTINCT MAB.IES_MATRICOLA MATRICOLA, ";
                Sql += "A.AND_COGNOME || ' ' || A.AND_NOME NOMINATIVO, ";
                Sql += "S.SED_COD_SEDE CodSede, ";
                Sql += "S.SED_DESCRIZIONE SEDE, ";
                Sql += "(MAB.VAL_ID_VALUTACANONE + MAB.VAL_ID_VALUTAUFFICIALE) VALUTA, ";
                Sql += "MAB.MAB_DT_DATADECORRENZA DATADECORRENZA, ";
                Sql += "MAB.MAB_DT_LETTERA DATALETTERA, ";
                Sql += "MAB.MAB_DT_OPERAZIONE DATAOPERAZIONE, ";
                Sql += "MAB.MAB_CANONELOCAZIONE CANONE, ";
                Sql += "RMAB.MAB_RAT_MAGINVIATA RATAINVIATA, ";
                Sql += "P.MAB_PAR_PERCENTUALE PERCENTUALE, ";
                Sql += "MAB.IES_PROG_TRASFERIMENTO ";
                Sql += "FROM MAG_ABITAZIONE MAB, ";
                Sql += "RATEIZZAZIONECONTMAB RMAB, ";
                Sql += "ANADIPE A, ";
                Sql += "SEDIESTERE S, ";
                Sql += "PARAMETRIMAB P ";
                Sql += "WHERE 1 = 1 ";
                Sql += "AND RMAB.MAB_ID = MAB.MAB_ID ";
                Sql += "AND A.AND_MATRICOLA = MAB.IES_MATRICOLA ";
                Sql += "AND MAB.IES_COD_SEDE = S.SED_COD_SEDE ";
                Sql += "AND P.MAB_PAR_ID = MAB.MAB_PAR_ID ";
                Sql += "AND MAB.MAB_FLAG_ANNULLATO = 0 ";
                Sql += "AND RMAB.MAB_RAT_FLAG_ANNULLATO = 0 ";
                Sql += "AND MAB.MAB_CESSAZIONE = 0 ";
                Sql += "AND(RMAB.MAB_RAT_DATARATA >= To_Date ('" + V_DATA + "','DD-MM-YYYY') ";
                Sql += "AND RMAB.MAB_RAT_DATARATA <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "ORDER BY NOMINATIVO, MAB.IES_PROG_TRASFERIMENTO, MAB.MAB_DT_DATADECORRENZA ";


                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Op_Magg_Abitaz> model = new List<Stp_Op_Magg_Abitaz>();
                while (rdr.Read())
                {
                    var details = new Stp_Op_Magg_Abitaz();
                    details.matricola = rdr["MATRICOLA"].ToString();
                    details.nominativo = rdr["NOMINATIVO"].ToString();
                    details.codice_sede = rdr["CodSede"].ToString();
                    details.sede = rdr["SEDE"].ToString();
                    details.valuta = rdr["VALUTA"].ToString();
                    //if (rdr["DATADECORRENZA"] != DBNull.Value)
                    details.data_decorrenza = Convert.ToDateTime(rdr["DATADECORRENZA"]).ToString("dd/MM/yyyy");
                    details.data_lettera = Convert.ToDateTime(rdr["DATALETTERA"]).ToString("dd/MM/yyyy");
                    details.data_operazione = Convert.ToDateTime(rdr["DATAOPERAZIONE"]).ToString("dd/MM/yyyy");
                    details.canone = rdr["CANONE"].ToString();
                    details.importo = rdr["RATAINVIATA"].ToString();
                    details.percentuale_applicata = rdr["PERCENTUALE"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("OpMaggiorazioneAbitazione", model);
            }
        }

        // Report Operazioni Effettuate - Maggiorazione Abitazione
        public ActionResult RptOpMaggiorazioneAbitazione(string V_DATA = "", string V_DATA1 = "")
        {
            DataSet3 ds3 = new DataSet3();
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

                String Sql = "SELECT DISTINCT MAB.IES_MATRICOLA MATRICOLA, ";
                Sql += "A.AND_COGNOME || ' ' || A.AND_NOME NOMINATIVO, ";
                Sql += "S.SED_COD_SEDE CodSede, ";
                Sql += "S.SED_DESCRIZIONE SEDE, ";
                Sql += "(MAB.VAL_ID_VALUTACANONE + MAB.VAL_ID_VALUTAUFFICIALE) VALUTA, ";
                Sql += "MAB.MAB_DT_DATADECORRENZA DATADECORRENZA, ";
                Sql += "MAB.MAB_DT_LETTERA DATALETTERA, ";
                Sql += "MAB.MAB_DT_OPERAZIONE DATAOPERAZIONE, ";
                Sql += "MAB.MAB_CANONELOCAZIONE CANONE, ";
                Sql += "RMAB.MAB_RAT_MAGINVIATA RATAINVIATA, ";
                Sql += "P.MAB_PAR_PERCENTUALE PERCENTUALE, ";
                Sql += "MAB.IES_PROG_TRASFERIMENTO ";
                Sql += "FROM MAG_ABITAZIONE MAB, ";
                Sql += "RATEIZZAZIONECONTMAB RMAB, ";
                Sql += "ANADIPE A, ";
                Sql += "SEDIESTERE S, ";
                Sql += "PARAMETRIMAB P ";
                Sql += "WHERE 1 = 1 ";
                Sql += "AND RMAB.MAB_ID = MAB.MAB_ID ";
                Sql += "AND A.AND_MATRICOLA = MAB.IES_MATRICOLA ";
                Sql += "AND MAB.IES_COD_SEDE = S.SED_COD_SEDE ";
                Sql += "AND P.MAB_PAR_ID = MAB.MAB_PAR_ID ";
                Sql += "AND MAB.MAB_FLAG_ANNULLATO = 0 ";
                Sql += "AND RMAB.MAB_RAT_FLAG_ANNULLATO = 0 ";
                Sql += "AND MAB.MAB_CESSAZIONE = 0 ";
                Sql += "AND(RMAB.MAB_RAT_DATARATA >= To_Date ('" + V_DATA + "','DD-MM-YYYY') ";
                Sql += "AND RMAB.MAB_RAT_DATARATA <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "ORDER BY NOMINATIVO, MAB.IES_PROG_TRASFERIMENTO, MAB.MAB_DT_DATADECORRENZA ";
                #endregion

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                //adp.Fill(ds3, ds3.V_OP_EFFETTUATE_MAGG_ABITAZ.TableName);
                adp.Fill(ds3, ds3.DataTable3.TableName);


                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\RptOpMaggAbitaz.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet3", ds3.Tables[0]));

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
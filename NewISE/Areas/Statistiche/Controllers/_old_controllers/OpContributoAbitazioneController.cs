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
    public class OpContributoAbitazioneController : Controller
    {
        // GET: Statistiche/OpContributoAbitazione
        public ActionResult Index()
        {
            return View();
        }

        // Operazioni Effettuate - Contributo Abitazione
        public ActionResult OpContributoAbitazione(string V_DATA = "", string V_DATA1 = "")
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                Sql += "CON_MATRICOLA, ";
                Sql += "SED_DESCRIZIONE, ";
                Sql += "VAL_DESCRIZIONE, ";
                Sql += "CON_DT_DECORRENZA, ";
                Sql += "CON_DT_LETTERA, ";
                Sql += "CON_DT_OPERAZIONE, ";
                Sql += "CON_CONTRIBUTO_VALUTA, ";
                Sql += "CON_CANONE, ";
                Sql += "CON_VALUTA_UFFICIALE, ";
                Sql += "CON_PERCENTUALE, ";
                Sql += "CON_CONTRIBUTO_LIRE, ";
                Sql += "CON_PROG_CONTRIBUTO_ABITAZIONE, ";
                Sql += "CON_PROG_TRASFERIMENTO ";
                Sql += "From CONTRIBUTOABITAZIONE, SEDIESTERE, VALUTE, ANADIPE ";
                Sql += "Where CON_COD_SEDE = SED_COD_SEDE ";
                Sql += "And CON_VALUTA_UFFICIALE = VAL_COD_VALUTA ";
                Sql += "And CON_MATRICOLA = AND_MATRICOLA ";
                Sql += "And(CON_DT_OPERAZIONE >= To_Date ('" + V_DATA + "','DD-MM-YYYY') ";
                Sql += "And CON_DT_OPERAZIONE <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "Order By NOMINATIVO, ";
                Sql += "CON_PROG_TRASFERIMENTO, ";
                Sql += "CON_DT_DECORRENZA, ";
                Sql += "CON_PROG_CONTRIBUTO_ABITAZIONE ";

                OracleCommand cmd = new OracleCommand(Sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Op_Contributo_Abitazione> model = new List<Stp_Op_Contributo_Abitazione>();
                while (rdr.Read())
                {
                    var details = new Stp_Op_Contributo_Abitazione();
                    details.matricola = rdr["CON_MATRICOLA"].ToString();
                    details.nominativo = rdr["NOMINATIVO"].ToString();
                    details.sede = rdr["SED_DESCRIZIONE"].ToString();
                    details.valuta = rdr["VAL_DESCRIZIONE"].ToString();
                    details.data_decorrenza = rdr["CON_DT_DECORRENZA"] == DBNull.Value ? null : Convert.ToDateTime(rdr["CON_DT_DECORRENZA"]).ToString("dd/MM/yyyy");
                    details.data_lettera = rdr["CON_DT_LETTERA"] == DBNull.Value ? null : Convert.ToDateTime(rdr["CON_DT_LETTERA"]).ToString("dd/MM/yyyy");
                    details.data_operazione = rdr["CON_DT_OPERAZIONE"] == DBNull.Value ? null : Convert.ToDateTime(rdr["CON_DT_OPERAZIONE"]).ToString("dd/MM/yyyy");
                    details.contributo_valuta = rdr["CON_CONTRIBUTO_VALUTA"].ToString();
                    details.contributo_L_E = rdr["CON_CONTRIBUTO_LIRE"].ToString();
                    details.canone = rdr["CON_CANONE"].ToString();
                    details.percentuale_applicata = rdr["CON_PERCENTUALE"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("OpContributoAbitazione", model);
            }
        }

        // Report Operazioni Effettuate - Contributo Abitazione
        //public ActionResult RptOpContributoAbitazione(string V_DATA = "", string V_DATA1 = "")
        //{
        //    DataSet8 ds8 = new DataSet8();
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

        //        String Sql = "Select Distinct AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
        //        Sql += "CON_MATRICOLA, ";
        //        Sql += "SED_DESCRIZIONE, ";
        //        Sql += "VAL_DESCRIZIONE, ";
        //        Sql += "CON_DT_DECORRENZA, ";
        //        Sql += "CON_DT_LETTERA, ";
        //        Sql += "CON_DT_OPERAZIONE, ";
        //        Sql += "CON_CONTRIBUTO_VALUTA, ";
        //        Sql += "CON_CANONE, ";
        //        Sql += "CON_VALUTA_UFFICIALE, ";
        //        Sql += "CON_PERCENTUALE, ";
        //        Sql += "CON_CONTRIBUTO_LIRE, ";
        //        Sql += "CON_PROG_CONTRIBUTO_ABITAZIONE, ";
        //        Sql += "CON_PROG_TRASFERIMENTO ";
        //        Sql += "From CONTRIBUTOABITAZIONE, SEDIESTERE, VALUTE, ANADIPE ";
        //        Sql += "Where CON_COD_SEDE = SED_COD_SEDE ";
        //        Sql += "And CON_VALUTA_UFFICIALE = VAL_COD_VALUTA ";
        //        Sql += "And CON_MATRICOLA = AND_MATRICOLA ";
        //        Sql += "And(CON_DT_OPERAZIONE >= To_Date ('" + V_DATA + "','DD-MM-YYYY') ";
        //        Sql += "And CON_DT_OPERAZIONE <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
        //        Sql += "Order By NOMINATIVO, ";
        //        Sql += "CON_PROG_TRASFERIMENTO, ";
        //        Sql += "CON_DT_DECORRENZA, ";
        //        Sql += "CON_PROG_CONTRIBUTO_ABITAZIONE ";
        //        #endregion

        //        OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

        //        //adp.Fill(ds8, ds8.V_OP_EFFETTUATE_CONTR_ABITAZ.TableName);
        //        adp.Fill(ds8, ds8.DataTable8.TableName);

        //        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\RptOpContrAbitaz.rdlc";
        //        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet8", ds8.Tables[0]));

        //        ReportParameter[] parameterValues = new ReportParameter[]
        //       {
        //            new ReportParameter ("fromDate",V_DATA),
        //            new ReportParameter ("toDate",V_DATA1)
        //       };

        //        reportViewer.LocalReport.SetParameters(parameterValues);
        //        reportViewer.LocalReport.Refresh();

        //        ViewBag.ReportViewer = reportViewer;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //    return View();
        //}

    }
}
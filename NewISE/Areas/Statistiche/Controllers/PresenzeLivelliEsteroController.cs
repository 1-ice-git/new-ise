using Microsoft.Reporting.WebForms;
using NewISE.Areas.Statistiche.Models;
using NewISE.Areas.Statistiche.Models.dtObj;
using NewISE.Areas.Statistiche.RPTDataSet;
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
    public class PresenzeLivelliEsteroController : Controller
    {
        // GET: Statistiche/PresenzeLivelliEstero
        public ActionResult Index()
        {
            return View();
        }


        // Presenze Livelli in servizio all' Estero
        public ActionResult PresenzeLivelli(string codicequalifica = "", string V_DATA = "", string V_DATA1 = "")
        {
            // Combo Qualifiche
            //Select Distinct IBS_DESCRIZIONE From INDENNITABASE Order By IBS_DESCRIZIONE

            ViewBag.ListaDipendentiEsteroLivello = new List<SelectListItem>();
            List<DipEsteroLivelloModel> lcm = new List<DipEsteroLivelloModel>();
            List<SelectListItem> lr = new List<SelectListItem>();
            List<Stp_Presenze_Livelli> lsd = new List<Stp_Presenze_Livelli>();
            List<Stp_Presenze_Livelli> model = new List<Stp_Presenze_Livelli>();

            lcm = Dipendenti.GetAllQualifiche().ToList();

            if (lcm != null && lcm.Any())
            {
                foreach (var item in lcm)
                {
                    SelectListItem r = new SelectListItem()
                    {
                        Text = item.qualifica,
                        Value = item.codicequalifica
                    };

                    lr.Add(r);

                }
                if (codicequalifica == string.Empty)
                {

                    //lr.First().Selected = true;
                    lr.First().Value = "";
                    codicequalifica = lr.First().Value;
                }
                else
                {
                    var lvr = lr.Where(a => a.Value == codicequalifica);
                    if (lvr != null && lvr.Count() > 0)
                    {
                        var r = lvr.First();
                        r.Selected = true;

                    }
                }


                ViewBag.ListaDipendentiEsteroLivello = lr;
            }

            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                // Insert into ISE_STP_LIVELLIESTERI
                //String Sql = "Select distinct CODQUALIFICA, QUALIFICA, NOMINATIVO, MATRICOLA, SEDE, DT_TRASFERIMENTO, DT_RIENTRO, DT_DECORRENZA From ISE_STP_LIVELLIESTERI Order By QUALIFICA, NOMINATIVO";
                //string Data_Oracle = VDATA;

                if (V_DATA == "")
                {
                    V_DATA = "01/01/" + DateTime.Now.Year;
                }


                String Sql = "Select Distinct IES_COD_QUALIFICA, ";
                Sql += "IBS_DESCRIZIONE, ";
                Sql += "IES_MATRICOLA, ";
                Sql += "IES_DT_DECORRENZA, ";
                Sql += "AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                Sql += "SED_DESCRIZIONE, ";
                Sql += "IES_DT_TRASFERIMENTO, ";
                Sql += "TRA_DT_FIN_TRASFERIMENTO, ";
                Sql += "IES_PROG_TRASFERIMENTO, ";
                Sql += "IES_PROG_MOVIMENTO ";
                Sql += "From INDESTERA T1, ";
                Sql += "SEDIESTERE, ";
                Sql += "INDENNITABASE, ";
                Sql += "ANADIPE, ";
                Sql += "TRASFERIMENTO, ";
                Sql += "(Select A.IES_MATRICOLA MATR, Max(A.IES_DT_DECORRENZA) DATAM ";
                Sql += "From INDESTERA A, INDESTERA B ";
                Sql += "Where A.IES_MATRICOLA = B.IES_MATRICOLA ";
                Sql += "And A.IES_PROG_TRASFERIMENTO = B.IES_PROG_TRASFERIMENTO ";
                Sql += "And A.IES_DT_DECORRENZA <= To_Date ('" + V_DATA + "', 'DD-MM-YYYY')  ";
                Sql += "GROUP BY A.IES_MATRICOLA) MAXDATA ";
                Sql += "Where(IES_DT_TRASFERIMENTO <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "And (TRA_DT_FIN_TRASFERIMENTO Is Null Or ";
                Sql += "To_Date (TRA_DT_FIN_TRASFERIMENTO) > ";
                Sql += "To_Date( '" + V_DATA + "', 'DD-MM-YYYY')) ";
                Sql += "And MAXDATA.MATR(+) = IES_MATRICOLA ";
                Sql += "And (IES_DT_DECORRENZA > To_Date('" + V_DATA + "', 'DD-MM-YYYY') Or ";
                Sql += "IES_DT_DECORRENZA = MAXDATA.DATAM) ";
                Sql += "And IES_FLAG_RICALCOLATO Is Null ";
                Sql += "And IES_COD_QUALIFICA = IBS_COD_QUALIFICA ";
                Sql += "And IES_MATRICOLA = AND_MATRICOLA ";
                Sql += "And IES_COD_SEDE = SED_COD_SEDE ";
                Sql += "And IES_MATRICOLA = TRA_MATRICOLA ";
                Sql += "And IES_PROG_TRASFERIMENTO = TRA_PROG_TRASFERIMENTO ";
                if (codicequalifica != "")
                    Sql += "And IES_COD_QUALIFICA = :codqualifica ";
                //Sql += "And IBS_DESCRIZIONE = 'DIRIGENTE' ";
                Sql += "Order By IES_MATRICOLA, ";
                Sql += "IES_PROG_TRASFERIMENTO Desc, ";
                Sql += "IES_DT_DECORRENZA Desc, ";
                Sql += "IES_PROG_MOVIMENTO Desc ";

                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = Sql;
                    cmd.Parameters.Add("codqualifica", codicequalifica);
                    cmd.Connection.Open();

                    using (OracleDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                var details = new Stp_Presenze_Livelli();
                                details.codQualifica = rdr["IES_COD_QUALIFICA"].ToString();
                                details.qualifica = rdr["IBS_DESCRIZIONE"].ToString();
                                details.nominativo = rdr["NOMINATIVO"].ToString();
                                details.matricola = rdr["IES_MATRICOLA"].ToString();
                                details.sede = rdr["SED_DESCRIZIONE"].ToString();
                                //details.dt_Trasferimento = Convert.ToDateTime(rdr["IES_DT_TRASFERIMENTO"]).ToString("dd/MM/yyyy");
                                details.dt_Trasferimento = rdr["IES_DT_TRASFERIMENTO"] == DBNull.Value ? null : Convert.ToDateTime(rdr["IES_DT_TRASFERIMENTO"]).ToString("dd/MM/yyyy");
                                //details.dt_Rientro = Convert.ToDateTime(rdr["TRA_DT_FIN_TRASFERIMENTO"]).ToString("dd/MM/yyyy");
                                details.dt_Rientro = rdr["TRA_DT_FIN_TRASFERIMENTO"] == DBNull.Value ? null : Convert.ToDateTime(rdr["TRA_DT_FIN_TRASFERIMENTO"]).ToString("dd/MM/yyyy");
                                //details.dt_Decorrenza = Convert.ToDateTime(rdr["IES_DT_DECORRENZA"]).ToString("dd/MM/yyyy");
                                details.dt_Decorrenza = rdr["IES_DT_DECORRENZA"] == DBNull.Value ? null : Convert.ToDateTime(rdr["IES_DT_DECORRENZA"]).ToString("dd/MM/yyyy");
                                details.progr_trasferimento = rdr["IES_PROG_TRASFERIMENTO"].ToString();
                                details.progr_movimento = rdr["IES_PROG_MOVIMENTO"].ToString();
                                model.Add(details);
                            }
                        }
                    }

                }

                return PartialView("PresenzeLivelli", model);

                
            }


        }

        public ActionResult PresenzeLivelli1(string codicequalifica = "", string V_DATA = "", string V_DATA1 = "")
        {
            // Combo Qualifiche
            //Select Distinct IBS_DESCRIZIONE From INDENNITABASE Order By IBS_DESCRIZIONE

            ViewBag.ListaDipendentiEsteroLivello = new List<SelectListItem>();
            List<DipEsteroLivelloModel> lcm = new List<DipEsteroLivelloModel>();
            List<SelectListItem> lr = new List<SelectListItem>();
            List<Stp_Presenze_Livelli> lsd = new List<Stp_Presenze_Livelli>();
            List<Stp_Presenze_Livelli> model = new List<Stp_Presenze_Livelli>();

            lcm = Dipendenti.GetAllQualifiche().ToList();

            if (lcm != null && lcm.Any())
            {
                foreach (var item in lcm)
                {
                    SelectListItem r = new SelectListItem()
                    {
                        Text = item.qualifica,
                        Value = item.codicequalifica
                    };

                    lr.Add(r);

                }
                if (codicequalifica == string.Empty)
                {

                    //lr.First().Selected = true;
                    lr.First().Value = "";
                    codicequalifica = lr.First().Value;
                }
                else
                {
                    var lvr = lr.Where(a => a.Value == codicequalifica);
                    if (lvr != null && lvr.Count() > 0)
                    {
                        var r = lvr.First();
                        r.Selected = true;

                    }
                }


                ViewBag.ListaDipendentiEsteroLivello = lr;
            }

            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                // Insert into ISE_STP_LIVELLIESTERI
                //String Sql = "Select distinct CODQUALIFICA, QUALIFICA, NOMINATIVO, MATRICOLA, SEDE, DT_TRASFERIMENTO, DT_RIENTRO, DT_DECORRENZA From ISE_STP_LIVELLIESTERI Order By QUALIFICA, NOMINATIVO";
                //string Data_Oracle = VDATA;

                if (V_DATA == "")
                {
                    V_DATA = "01/01/" + DateTime.Now.Year;
                }


                String Sql = "Select Distinct IES_COD_QUALIFICA, ";
                Sql += "IBS_DESCRIZIONE, ";
                Sql += "IES_MATRICOLA, ";
                Sql += "IES_DT_DECORRENZA, ";
                Sql += "AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                Sql += "SED_DESCRIZIONE, ";
                Sql += "IES_DT_TRASFERIMENTO, ";
                Sql += "TRA_DT_FIN_TRASFERIMENTO, ";
                Sql += "IES_PROG_TRASFERIMENTO, ";
                Sql += "IES_PROG_MOVIMENTO ";
                Sql += "From INDESTERA T1, ";
                Sql += "SEDIESTERE, ";
                Sql += "INDENNITABASE, ";
                Sql += "ANADIPE, ";
                Sql += "TRASFERIMENTO, ";
                Sql += "(Select A.IES_MATRICOLA MATR, Max(A.IES_DT_DECORRENZA) DATAM ";
                Sql += "From INDESTERA A, INDESTERA B ";
                Sql += "Where A.IES_MATRICOLA = B.IES_MATRICOLA ";
                Sql += "And A.IES_PROG_TRASFERIMENTO = B.IES_PROG_TRASFERIMENTO ";
                Sql += "And A.IES_DT_DECORRENZA <= To_Date ('" + V_DATA + "', 'DD-MM-YYYY')  ";
                Sql += "GROUP BY A.IES_MATRICOLA) MAXDATA ";
                Sql += "Where(IES_DT_TRASFERIMENTO <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "And (TRA_DT_FIN_TRASFERIMENTO Is Null Or ";
                Sql += "To_Date (TRA_DT_FIN_TRASFERIMENTO) > ";
                Sql += "To_Date( '" + V_DATA + "', 'DD-MM-YYYY')) ";
                Sql += "And MAXDATA.MATR(+) = IES_MATRICOLA ";
                Sql += "And (IES_DT_DECORRENZA > To_Date('" + V_DATA + "', 'DD-MM-YYYY') Or ";
                Sql += "IES_DT_DECORRENZA = MAXDATA.DATAM) ";
                Sql += "And IES_FLAG_RICALCOLATO Is Null ";
                Sql += "And IES_COD_QUALIFICA = IBS_COD_QUALIFICA ";
                Sql += "And IES_MATRICOLA = AND_MATRICOLA ";
                Sql += "And IES_COD_SEDE = SED_COD_SEDE ";
                Sql += "And IES_MATRICOLA = TRA_MATRICOLA ";
                Sql += "And IES_PROG_TRASFERIMENTO = TRA_PROG_TRASFERIMENTO ";
                if (codicequalifica != "")
                    Sql += "And IES_COD_QUALIFICA = :codqualifica ";
                //Sql += "And IBS_DESCRIZIONE = 'DIRIGENTE' ";
                Sql += "Order By IES_MATRICOLA, ";
                Sql += "IES_PROG_TRASFERIMENTO Desc, ";
                Sql += "IES_DT_DECORRENZA Desc, ";
                Sql += "IES_PROG_MOVIMENTO Desc ";

                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = Sql;
                    cmd.Parameters.Add("codqualifica", codicequalifica);
                    cmd.Connection.Open();

                    using (OracleDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                var details = new Stp_Presenze_Livelli();
                                details.codQualifica = rdr["IES_COD_QUALIFICA"].ToString();
                                details.qualifica = rdr["IBS_DESCRIZIONE"].ToString();
                                details.nominativo = rdr["NOMINATIVO"].ToString();
                                details.matricola = rdr["IES_MATRICOLA"].ToString();
                                details.sede = rdr["SED_DESCRIZIONE"].ToString();
                                //details.dt_Trasferimento = Convert.ToDateTime(rdr["IES_DT_TRASFERIMENTO"]).ToString("dd/MM/yyyy");
                                details.dt_Trasferimento = rdr["IES_DT_TRASFERIMENTO"] == DBNull.Value ? null : Convert.ToDateTime(rdr["IES_DT_TRASFERIMENTO"]).ToString("dd/MM/yyyy");
                                //details.dt_Rientro = Convert.ToDateTime(rdr["TRA_DT_FIN_TRASFERIMENTO"]).ToString("dd/MM/yyyy");
                                details.dt_Rientro = rdr["TRA_DT_FIN_TRASFERIMENTO"] == DBNull.Value ? null : Convert.ToDateTime(rdr["TRA_DT_FIN_TRASFERIMENTO"]).ToString("dd/MM/yyyy");
                                //details.dt_Decorrenza = Convert.ToDateTime(rdr["IES_DT_DECORRENZA"]).ToString("dd/MM/yyyy");
                                details.dt_Decorrenza = rdr["IES_DT_DECORRENZA"] == DBNull.Value ? null : Convert.ToDateTime(rdr["IES_DT_DECORRENZA"]).ToString("dd/MM/yyyy");
                                details.progr_trasferimento = rdr["IES_PROG_TRASFERIMENTO"].ToString();
                                details.progr_movimento = rdr["IES_PROG_MOVIMENTO"].ToString();
                                model.Add(details);
                            }
                        }
                    }

                }

                
                return PartialView(model);
            }


        }

        // Report Presenze Livelli in servizio all' Estero
        public ActionResult RptPresenzeLivelli(string codicequalifica = "", string V_DATA = "", string V_DATA1 = "")
        {
            DataSet13 ds13 = new DataSet13();

            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);


                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                OracleConnection conx = new OracleConnection(connectionString);

                //String Sql = "Select distinct CODQUALIFICA, QUALIFICA, NOMINATIVO, MATRICOLA, SEDE, DT_TRASFERIMENTO, DT_RIENTRO, DT_DECORRENZA From ISE_STP_LIVELLIESTERI Order By QUALIFICA, NOMINATIVO";

                #region MyRegion
                String Sql = "Select Distinct IES_COD_QUALIFICA, ";
                Sql += "IBS_DESCRIZIONE, ";
                Sql += "IES_MATRICOLA, ";
                Sql += "IES_DT_DECORRENZA, ";
                Sql += "AND_COGNOME || ' ' || AND_NOME NOMINATIVO, ";
                Sql += "SED_DESCRIZIONE, ";
                Sql += "IES_DT_TRASFERIMENTO, ";
                Sql += "TRA_DT_FIN_TRASFERIMENTO, ";
                Sql += "IES_PROG_TRASFERIMENTO, ";
                Sql += "IES_PROG_MOVIMENTO ";
                Sql += "From INDESTERA T1, ";
                Sql += "SEDIESTERE, ";
                Sql += "INDENNITABASE, ";
                Sql += "ANADIPE, ";
                Sql += "TRASFERIMENTO, ";
                Sql += "(Select A.IES_MATRICOLA MATR, Max(A.IES_DT_DECORRENZA) DATAM ";
                Sql += "From INDESTERA A, INDESTERA B ";
                Sql += "Where A.IES_MATRICOLA = B.IES_MATRICOLA ";
                Sql += "And A.IES_PROG_TRASFERIMENTO = B.IES_PROG_TRASFERIMENTO ";
                Sql += "And A.IES_DT_DECORRENZA <= To_Date ('" + V_DATA + "', 'DD-MM-YYYY')  ";
                Sql += "GROUP BY A.IES_MATRICOLA) MAXDATA ";
                Sql += "Where(IES_DT_TRASFERIMENTO <= To_Date ('" + V_DATA1 + "','DD-MM-YYYY')) ";
                Sql += "And (TRA_DT_FIN_TRASFERIMENTO Is Null Or ";
                Sql += "To_Date (TRA_DT_FIN_TRASFERIMENTO) > ";
                Sql += "To_Date( '" + V_DATA + "', 'DD-MM-YYYY')) ";
                Sql += "And MAXDATA.MATR(+) = IES_MATRICOLA ";
                Sql += "And (IES_DT_DECORRENZA > To_Date('" + V_DATA + "', 'DD-MM-YYYY') Or ";
                Sql += "IES_DT_DECORRENZA = MAXDATA.DATAM) ";
                Sql += "And IES_FLAG_RICALCOLATO Is Null ";
                Sql += "And IES_COD_QUALIFICA = IBS_COD_QUALIFICA ";
                Sql += "And IES_MATRICOLA = AND_MATRICOLA ";
                Sql += "And IES_COD_SEDE = SED_COD_SEDE ";
                Sql += "And IES_MATRICOLA = TRA_MATRICOLA ";
                Sql += "And IES_PROG_TRASFERIMENTO = TRA_PROG_TRASFERIMENTO ";
                if (codicequalifica != "")
                    //Sql += "And IES_COD_QUALIFICA = :codicequalifica ";
                    Sql += "And IES_COD_QUALIFICA = '" + codicequalifica + "' ";
                //Sql += "And IBS_DESCRIZIONE = 'DIRIGENTE' ";
                Sql += "Order By IES_MATRICOLA, ";
                Sql += "IES_PROG_TRASFERIMENTO Desc, ";
                Sql += "IES_DT_DECORRENZA Desc, ";
                Sql += "IES_PROG_MOVIMENTO Desc ";
                #endregion

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                //adp.Fill(ds13, ds13.V_PRESENZE_LIVELLI.TableName);
                adp.Fill(ds13, ds13.DataTable13.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\RptPresenzeLivelli.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet13", ds13.Tables[0]));

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
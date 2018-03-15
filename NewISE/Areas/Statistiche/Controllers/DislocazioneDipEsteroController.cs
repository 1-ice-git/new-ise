using Microsoft.Reporting.WebForms;
using NewISE.Areas.Statistiche.Models;
using NewISE.Areas.Statistiche.RPTDataSet;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace NewISE.Areas.Statistiche.Controllers
{
    public class DislocazioneDipEsteroController : Controller
    {
        // GET: Statistiche/DislocazioneDipEstero
        public ActionResult Index()
        {
            return View();
        }

        // Dislocazione dei Dipendenti all'Estero
        public ActionResult Dislocazione(string codicesede = "", string V_UTENTE = "", string V_DATA = "", string V_UFFICIO = "")
        {
            ViewBag.ListaDislocazioneDipEstero = new List<SelectListItem>();
            List<DipEsteroModel> lcm = new List<DipEsteroModel>();
            List<SelectListItem> lr = new List<SelectListItem>();
            List<Stp_Dislocazione_dipendenti> lsd = new List<Stp_Dislocazione_dipendenti>();
            List<Stp_Dislocazione_dipendenti> model = new List<Stp_Dislocazione_dipendenti>();

            lcm = Dipendenti.GetAllSedi().ToList();

            if (lcm != null && lcm.Any())
            {
                foreach (var item in lcm)
                {
                    SelectListItem r = new SelectListItem()
                    {
                        Text = item.descrizione,
                        Value = item.codicesede
                    };

                    lr.Add(r);

                }
                if (codicesede == string.Empty)
                {
                    //lr.First().Selected = true;
                    lr.First().Value = "";
                    codicesede = lr.First().Value;
                }
                else
                {
                    var lvr = lr.Where(a => a.Value == codicesede);
                    if (lvr != null && lvr.Count() > 0)
                    {
                        var r = lvr.First();
                        r.Selected = true;

                    }
                }


                ViewBag.ListaDislocazioneDipEstero = lr;
            }

            // Chiamata alla pagina Visual Basic -- Stampa_Elenco_Trasferimenti()
            //StampeISE.frmStampe xx = new frmStampe();
            //xx.Stampa_Consuntivo_Costi_x_Coan("E135A054B1", "01/03/2017", "01/03/2018", "fantomas");


            //StampeISE.Class1 xx = new Class1();
            //xx.Stampa_Elenco_Trasferimenti();


            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {

                OracleCommand cmd1 = new OracleCommand();
                cmd1.Connection = cn;
                cmd1.CommandText = "ISE_STAMPA_ELENCO_TRASF";
                cmd1.CommandType = System.Data.CommandType.StoredProcedure;

                cmd1.Parameters.Add("@V_UTENTE", OracleDbType.Varchar2, 50).Value = V_UTENTE;
                cmd1.Parameters.Add("@V_DATA", OracleDbType.Varchar2, 50).Value = V_DATA;
                cmd1.Parameters.Add("@V_UFFICIO", OracleDbType.Varchar2, 50).Value = codicesede;

                cn.Open();
                cmd1.ExecuteNonQuery();

                //String Sql = "Select distinct SEDE, VALUTA, MATRICOLA, NOMINATIVO, DT_TRASFERIMENTO, QUALIFICA, CONIUGE, FIGLI, ISEP, CONTRIBUTO, USO, ISEP + CONTRIBUTO + USO TOTALE From ISE_STP_ELENCOTRASFERIMENTI, SEDIESTERE WHERE SEDIESTERE.SED_COD_SEDE = '" + codicesede + "' AND ISE_STP_ELENCOTRASFERIMENTI.SEDE = SEDIESTERE.SED_DESCRIZIONE Order By SEDE, NOMINATIVO";

                String Sql = "Select SEDE,VALUTA, ";
                Sql += "MATRICOLA, ";
                Sql += "NOMINATIVO, ";
                Sql += "DT_TRASFERIMENTO, ";
                Sql += "QUALIFICA, ";
                Sql += "CONIUGE, ";
                Sql += "FIGLI, ";
                Sql += "ISEP, ";
                Sql += "CONTRIBUTO, ";
                Sql += "USO, ";
                Sql += "ISEP +CONTRIBUTO + USO TOTALE ";
                Sql += "From ISE_STP_ELENCOTRASFERIMENTI ";
                Sql += "Where UTENTE = '" + V_UTENTE + "' ";
                Sql += "Order By SEDE, NOMINATIVO";



                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = Sql;
                    //cmd.Parameters.Add("codsede", codicesede);
                    //cmd.Connection.Open();

                    using (OracleDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                var details = new Stp_Dislocazione_dipendenti();
                                details.sede = rdr["SEDE"].ToString();
                                details.valuta = rdr["VALUTA"].ToString();
                                details.matricola = rdr["MATRICOLA"].ToString();
                                details.nominativo = rdr["NOMINATIVO"].ToString();
                                details.dataTrasferimento = Convert.ToDateTime(rdr["DT_TRASFERIMENTO"]).ToString("dd/MM/yyyy");
                                details.qualifica = rdr["QUALIFICA"].ToString();
                                details.coniuge = rdr["CONIUGE"].ToString();
                                details.figli = rdr["FIGLI"].ToString();
                                details.isep = rdr["ISEP"].ToString();
                                details.contributo = rdr["CONTRIBUTO"].ToString();
                                details.uso = rdr["USO"].ToString();
                                details.totale = rdr["TOTALE"].ToString();
                                model.Add(details);
                            }
                        }
                    }

                }

                return PartialView("DislocazioneDipEstero", model);
            }



        }

        // Report Dislocazione dei Dipendenti all'Estero
        public ActionResult RptDislocazione(string codicesede = "", string V_UTENTE = "", string V_DATA = "", string V_UFFICIO = "")
        {
            DataSet15 ds15 = new DataSet15();

            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);


                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                OracleConnection conx = new OracleConnection(connectionString);

                OracleCommand cmd1 = new OracleCommand();
                cmd1.Connection = conx;
                cmd1.CommandText = "ISE_STAMPA_ELENCO_TRASF";
                cmd1.CommandType = System.Data.CommandType.StoredProcedure;

                cmd1.Parameters.Add("@V_UTENTE", OracleDbType.Varchar2, 50).Value = V_UTENTE;
                cmd1.Parameters.Add("@V_DATA", OracleDbType.Varchar2, 50).Value = V_DATA;
                cmd1.Parameters.Add("@V_UFFICIO", OracleDbType.Varchar2, 50).Value = codicesede;

                conx.Open();
                cmd1.ExecuteNonQuery();

                //String Sql = "Select distinct SEDE, VALUTA, MATRICOLA, NOMINATIVO, DT_TRASFERIMENTO, QUALIFICA, CONIUGE, FIGLI, ISEP, CONTRIBUTO, USO, ISEP + CONTRIBUTO + USO TOTALE From ISE_STP_ELENCOTRASFERIMENTI, SEDIESTERE WHERE SEDIESTERE.SED_COD_SEDE = '" + codicesede + "' AND ISE_STP_ELENCOTRASFERIMENTI.SEDE = SEDIESTERE.SED_DESCRIZIONE Order By SEDE, NOMINATIVO";

                String Sql = "Select SEDE,VALUTA, ";
                Sql += "MATRICOLA, ";
                Sql += "NOMINATIVO, ";
                Sql += "DT_TRASFERIMENTO, ";
                Sql += "QUALIFICA, ";
                Sql += "CONIUGE, ";
                Sql += "FIGLI, ";
                Sql += "ISEP, ";
                Sql += "CONTRIBUTO, ";
                Sql += "USO, ";
                Sql += "ISEP +CONTRIBUTO + USO TOTALE ";
                Sql += "From ISE_STP_ELENCOTRASFERIMENTI ";
                Sql += "Where UTENTE = '" + V_UTENTE + "' ";
                Sql += "Order By SEDE, NOMINATIVO ";

                OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                //adp.Fill(ds15, ds15.V_ISE_STP_ELENCO_TRASF.TableName);
                adp.Fill(ds15, ds15.DataTable15.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\RptDislocazioneDip.rdlc";
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet15", ds15.Tables[0]));

                ReportParameter[] parameterValues = new ReportParameter[]
                {
                    new ReportParameter ("fromDate",V_DATA)
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
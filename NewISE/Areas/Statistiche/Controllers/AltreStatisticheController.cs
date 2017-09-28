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
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace NewISE.Areas.Statistiche.Controllers
{
    public class AltreStatisticheController : Controller
    {
        // GET: Statistiche/AltreStatistiche
        public ActionResult AltreStatistiche2(decimal idUtenteAutorizzato = 0)
        {

            List<LogAttivitaModel> libm = new List<LogAttivitaModel>();
            var r = new List<SelectListItem>();
            List<UtenteAutorizzatoModel> llm = new List<UtenteAutorizzatoModel>();

            try
            {
                using (dtUtentiAutorizzati dtl = new dtUtentiAutorizzati())
                {
                    llm = dtl.GetUtentiAutorizzati().OrderBy(a => a.matricola).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.matricola,
                                 Value = t.idUtenteAutorizzato.ToString()

                             }).ToList();

                        if (idUtenteAutorizzato == 0)
                        {
                            r.First().Selected = true;
                            idUtenteAutorizzato = Convert.ToDecimal(r.First().Value);
                        }
                        else
                        {
                            r.Where(a => a.Value == idUtenteAutorizzato.ToString()).First().Selected = true;
                        }

                    }

                    ViewBag.LivelliList = r;
                }

                using (dtLogAttivita dtib = new dtLogAttivita())
                {
                    libm = dtib.getListLogAttivita().OrderBy(a => a.idLog).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }


            return PartialView(libm);

        }
        public ActionResult AltreStatistiche(decimal idUtenteAutorizzato = 0)
        {

            List<LogAttivitaModel> libm = new List<LogAttivitaModel>();
            var r = new List<SelectListItem>();
            List<UtenteAutorizzatoModel> llm = new List<UtenteAutorizzatoModel>();

            try
            {
                using (dtUtentiAutorizzati dtl = new dtUtentiAutorizzati())
                {
                    llm = dtl.GetUtentiAutorizzati().OrderBy(a => a.matricola).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.matricola,
                                 Value = t.idUtenteAutorizzato.ToString()

                             }).ToList();

                        if (idUtenteAutorizzato == 0)
                        {
                            r.First().Selected = true;
                            idUtenteAutorizzato = Convert.ToDecimal(r.First().Value);
                        }
                        else
                        {
                            r.Where(a => a.Value == idUtenteAutorizzato.ToString()).First().Selected = true;
                        }

                    }

                    ViewBag.LivelliList = r;
                }

                using (dtLogAttivita dtib = new dtLogAttivita())
                {
                    libm = dtib.getListLogAttivita().OrderBy(a => a.idLog).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }


            return PartialView(libm);

        }
        public ActionResult Index()
        {

            var t = new List<SelectListItem>();

            try
            {
                t.Add(new SelectListItem() { Text = "", Value = "", Selected = true });
                t.Add(new SelectListItem() { Text = "Consuntivo dei Costi", Value = "0" });
                t.Add(new SelectListItem() { Text = "Consuntivo dei Costi per Codice Co.An.", Value = "1" });
                t.Add(new SelectListItem() { Text = "Dislocazione dei dipendenti all'estero", Value = "2" });
                t.Add(new SelectListItem() { Text = "Operazioni effettuate nel periodo", Value = "3" });
                t.Add(new SelectListItem() { Text = "Presenze dei livelli in servizio all'estero", Value = "4" });
                t.Add(new SelectListItem() { Text = "Spese diverse", Value = "5" });
                t.Add(new SelectListItem() { Text = "Spese di avvicendamento", Value = "6" });
                t.Add(new SelectListItem() { Text = "Storia del dipendente", Value = "7" });

                ViewBag.VecchioIse = t;
                return PartialView();

            }
            catch (Exception ex)
            {
                return View("Error");

            }

           

        }
        public ActionResult AltreStatistiche4()
        {

            List<SelectListItem> li = new List<SelectListItem>();
            li.Add(new SelectListItem { Text = "Select", Value = "0" });
            li.Add(new SelectListItem { Text = "India", Value = "1" });
            li.Add(new SelectListItem { Text = "Srilanka", Value = "2" });
            li.Add(new SelectListItem { Text = "China", Value = "3" });
            li.Add(new SelectListItem { Text = "Austrila", Value = "4" });
            li.Add(new SelectListItem { Text = "USA", Value = "5" });
            li.Add(new SelectListItem { Text = "UK", Value = "6" });
            ViewData["country"] = li;
            return PartialView();
            
        }
        
        // Operazioni Effettuate
        public ActionResult OperazioniEffettuate()
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                String sql = "SELECT * FROM ISE_STP_OPERAZIONIEFFETTUATE ";
                OracleCommand cmd = new OracleCommand(sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Operazioni_Effettuate> model = new List<Stp_Operazioni_Effettuate>();
                while (rdr.Read())
                {
                    var details = new Stp_Operazioni_Effettuate();
                    details.matricola = rdr["matricola"].ToString();
                    details.nominativo = rdr["nominativo"].ToString();
                    details.sede = rdr["sede"].ToString();
                    details.valuta = rdr["valuta"].ToString();
                    details.tipomovimento = rdr["tipomovimento"].ToString();
                    details.dataDecorrenza = rdr["dataDecorrenza"].ToString();
                    details.dataLettera = rdr["dataLettera"].ToString();
                    details.importo1 = rdr["importo1"].ToString();
                    details.importo2 = rdr["importo2"].ToString();
                    details.importo3 = rdr["importo3"].ToString();
                    details.dataOperazione = rdr["dataOperazione"].ToString();
                    details.codLivello = rdr["codLivello"].ToString();
                    details.tipoRecord = rdr["tipoRecord"].ToString();
                    details.tipoSpesa = rdr["tipoSpesa"].ToString();
                    details.utente = rdr["utente"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("OperazioniEffettuate", model);
            }

        }
        //DataSet1 ds = new DataSet1();
        public ActionResult ReportEmployee()
        {
            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(900);
                reportViewer.Height = Unit.Percentage(900);

                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                // Operazioni effettuate
                OracleConnection conx = new OracleConnection(connectionString);

                OracleDataAdapter adp = new OracleDataAdapter("SELECT * FROM ISE_STP_OPERAZIONIEFFETTUATE", conx);


                //adp.Fill(ds, ds.ISE_STP_OPERAZIONIEFFETTUATE.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report1.rdlc";
                //reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", ds.Tables[0]));


                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
            }
            
            return View();
        }

        // Dislocazione Dipendenti all'Estero
        public ActionResult Dislocazione()
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                
                // var connection = new SqlConnection("YOUR CONNECTION STRING");
                // var command = new SqlCommand("ConvertLeadToCustomer1",connection)
                // command.CommandType = CommandType.StoredProcedure;
                // command.Parameters.AddWithValue("@CompanyID", id);
                // connection.Open();
                // command.ExcuteNonQuery();

                // connection.Close();
                
                String sql = "Select SEDE,VALUTA,MATRICOLA,NOMINATIVO,DT_TRASFERIMENTO,QUALIFICA,CONIUGE,FIGLI,ISEP,CONTRIBUTO,USO,ISEP +CONTRIBUTO + USO TOTALE From ISE_STP_ELENCOTRASFERIMENTI Order By SEDE, NOMINATIVO";
                OracleCommand cmd = new OracleCommand(sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Dislocazione_dipendenti> model = new List<Stp_Dislocazione_dipendenti>();
                while (rdr.Read())
                {
                    var details = new Stp_Dislocazione_dipendenti();
                    details.sede = rdr["SEDE"].ToString();
                    details.valuta = rdr["VALUTA"].ToString();
                    details.matricola = rdr["MATRICOLA"].ToString();
                    details.nominativo = rdr["NOMINATIVO"].ToString();
                    //details.dataTrasferimento = rdr["DT_TRASFERIMENTO"].ToString();
                    details.qualifica = rdr["QUALIFICA"].ToString();
                    details.coniuge = rdr["CONIUGE"].ToString();
                    details.figli = rdr["FIGLI"].ToString();
                    details.isep = rdr["ISEP"].ToString();
                    details.contributo = rdr["CONTRIBUTO"].ToString();
                    details.uso = rdr["USO"].ToString();
                    details.totale = rdr["TOTALE"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("Dislocazione", model);
            }

        }

        //DataSet2 ds1 = new DataSet2();
        public ActionResult ReportEmployee1()
        {
            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(900);
                reportViewer.Height = Unit.Percentage(900);

                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;
                
                OracleConnection conx = new OracleConnection(connectionString);
                OracleDataAdapter adp = new OracleDataAdapter("SELECT * FROM ISE_STP_ELENCOTRASFERIMENTI", conx);

                //adp.Fill(ds1, ds1.ISE_STP_ELENCOTRASFERIMENTI.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report2.rdlc";
                //reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", ds1.Tables[0]));

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }

        // Consuntivo Costi
        public ActionResult ConsuntivoCosti()
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                String sql = "SELECT * FROM ISE_STP_OPERAZIONIEFFETTUATE ";
                OracleCommand cmd = new OracleCommand(sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Operazioni_Effettuate> model = new List<Stp_Operazioni_Effettuate>();
                while (rdr.Read())
                {
                    var details = new Stp_Operazioni_Effettuate();
                    details.matricola = rdr["matricola"].ToString();
                    details.nominativo = rdr["nominativo"].ToString();
                    details.sede = rdr["sede"].ToString();
                    details.valuta = rdr["valuta"].ToString();
                    details.tipomovimento = rdr["tipomovimento"].ToString();
                    details.dataDecorrenza = rdr["dataDecorrenza"].ToString();
                    details.dataLettera = rdr["dataLettera"].ToString();
                    details.importo1 = rdr["importo1"].ToString();
                    details.importo2 = rdr["importo2"].ToString();
                    details.importo3 = rdr["importo3"].ToString();
                    details.dataOperazione = rdr["dataOperazione"].ToString();
                    details.codLivello = rdr["codLivello"].ToString();
                    details.tipoRecord = rdr["tipoRecord"].ToString();
                    details.tipoSpesa = rdr["tipoSpesa"].ToString();
                    details.utente = rdr["utente"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("OperazioniEffettuate", model);
            }

        }

        // Consuntivo Costi CoAn
        public ActionResult ConsuntivoCostiCoAn()
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                String sql = "SELECT * FROM ISE_STP_OPERAZIONIEFFETTUATE ";
                OracleCommand cmd = new OracleCommand(sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Operazioni_Effettuate> model = new List<Stp_Operazioni_Effettuate>();
                while (rdr.Read())
                {
                    var details = new Stp_Operazioni_Effettuate();
                    details.matricola = rdr["matricola"].ToString();
                    details.nominativo = rdr["nominativo"].ToString();
                    details.sede = rdr["sede"].ToString();
                    details.valuta = rdr["valuta"].ToString();
                    details.tipomovimento = rdr["tipomovimento"].ToString();
                    details.dataDecorrenza = rdr["dataDecorrenza"].ToString();
                    details.dataLettera = rdr["dataLettera"].ToString();
                    details.importo1 = rdr["importo1"].ToString();
                    details.importo2 = rdr["importo2"].ToString();
                    details.importo3 = rdr["importo3"].ToString();
                    details.dataOperazione = rdr["dataOperazione"].ToString();
                    details.codLivello = rdr["codLivello"].ToString();
                    details.tipoRecord = rdr["tipoRecord"].ToString();
                    details.tipoSpesa = rdr["tipoSpesa"].ToString();
                    details.utente = rdr["utente"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("OperazioniEffettuate", model);
            }

        }

        // Presenze Livelli
        public ActionResult PresenzeLivelli()
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                String sql = "Select CODQUALIFICA,QUALIFICA,NOMINATIVO,MATRICOLA,SEDE,DT_TRASFERIMENTO,DT_RIENTRO From ISE_STP_LIVELLIESTERI Order By QUALIFICA, NOMINATIVO ";
                OracleCommand cmd = new OracleCommand(sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Presenze_Livelli> model = new List<Stp_Presenze_Livelli>();
                while (rdr.Read())
                {
                    var details = new Stp_Presenze_Livelli();
                    details.codQualifica = rdr["codQualifica"].ToString();
                    details.qualifica = rdr["qualifica"].ToString();
                    details.nominativo = rdr["nominativo"].ToString();
                    details.matricola = rdr["matricola"].ToString();
                    //details.sede = rdr["sede"].ToString();
                    details.dt_Trasferimento = rdr["dt_Trasferimento"].ToString();
                    details.dt_Rientro = rdr["dt_Rientro"].ToString();
                    //details.utente = rdr["uteadm"].ToString();

                    model.Add(details);
                }
                
                return PartialView("PresenzeLivelli", model);
            }

        }

        //DataSet3 ds3 = new DataSet3();
        public ActionResult ReportEmployee3()
        {
            try
            {

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(900);
                reportViewer.Height = Unit.Percentage(900);

                var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

                OracleConnection conx = new OracleConnection(connectionString);
                OracleDataAdapter adp = new OracleDataAdapter("Select CODQUALIFICA,QUALIFICA,NOMINATIVO,MATRICOLA,SEDE,DT_TRASFERIMENTO,DT_RIENTRO From ISE_STP_LIVELLIESTERI Order By QUALIFICA, NOMINATIVO", conx);

                //adp.Fill(ds3, ds3.ISE_STP_LIVELLIESTERI.TableName);

                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report3.rdlc";
                //reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet3", ds3.Tables[0]));

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }
        
        // Spese Diverse
        public ActionResult SpeseDiverse()
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                String sql = "SELECT * FROM ISE_STP_OPERAZIONIEFFETTUATE ";
                OracleCommand cmd = new OracleCommand(sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Operazioni_Effettuate> model = new List<Stp_Operazioni_Effettuate>();
                while (rdr.Read())
                {
                    var details = new Stp_Operazioni_Effettuate();
                    details.matricola = rdr["matricola"].ToString();
                    details.nominativo = rdr["nominativo"].ToString();
                    details.sede = rdr["sede"].ToString();
                    details.valuta = rdr["valuta"].ToString();
                    details.tipomovimento = rdr["tipomovimento"].ToString();
                    details.dataDecorrenza = rdr["dataDecorrenza"].ToString();
                    details.dataLettera = rdr["dataLettera"].ToString();
                    details.importo1 = rdr["importo1"].ToString();
                    details.importo2 = rdr["importo2"].ToString();
                    details.importo3 = rdr["importo3"].ToString();
                    details.dataOperazione = rdr["dataOperazione"].ToString();
                    details.codLivello = rdr["codLivello"].ToString();
                    details.tipoRecord = rdr["tipoRecord"].ToString();
                    details.tipoSpesa = rdr["tipoSpesa"].ToString();
                    details.utente = rdr["utente"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("OperazioniEffettuate", model);
            }

        }

        // Spese Avvicendamento
        public ActionResult SpeseAvvicendamento()
        {
            using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
            {
                String sql = "SELECT * FROM ISE_STP_OPERAZIONIEFFETTUATE ";
                OracleCommand cmd = new OracleCommand(sql, cn);
                cn.Open();
                OracleDataReader rdr = cmd.ExecuteReader();
                List<Stp_Operazioni_Effettuate> model = new List<Stp_Operazioni_Effettuate>();
                while (rdr.Read())
                {
                    var details = new Stp_Operazioni_Effettuate();
                    details.matricola = rdr["matricola"].ToString();
                    details.nominativo = rdr["nominativo"].ToString();
                    details.sede = rdr["sede"].ToString();
                    details.valuta = rdr["valuta"].ToString();
                    details.tipomovimento = rdr["tipomovimento"].ToString();
                    details.dataDecorrenza = rdr["dataDecorrenza"].ToString();
                    details.dataLettera = rdr["dataLettera"].ToString();
                    details.importo1 = rdr["importo1"].ToString();
                    details.importo2 = rdr["importo2"].ToString();
                    details.importo3 = rdr["importo3"].ToString();
                    details.dataOperazione = rdr["dataOperazione"].ToString();
                    details.codLivello = rdr["codLivello"].ToString();
                    details.tipoRecord = rdr["tipoRecord"].ToString();
                    details.tipoSpesa = rdr["tipoSpesa"].ToString();
                    details.utente = rdr["utente"].ToString();
                    model.Add(details);
                }
                //return View("ViewName", model);
                return PartialView("OperazioniEffettuate", model);
            }

        }

        //// Storia Dipendente
        //public ActionResult StoriaDipendente()
        //{
        //    try
        //    {
        //        using (var cn = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString))
        //        {
        //            // Stored Procedure

        //            //OracleConnection con = new OracleConnection("user id=scott;password=tiger;data source=orcl");
        //            //cn.Open();
        //            //OracleCommand cmd = new OracleCommand();
        //            //cmd.CommandText = "ISE_STAMPA_STORIA_DIPENDENTE";
        //            //cmd.CommandType = CommandType.StoredProcedure;
        //            //cmd.Connection = cn;
        //            //OracleParameter retval = new OracleParameter("myretval", OracleDbType.Varchar2, 50);
        //            //retval.Direction = ParameterDirection.ReturnValue;
        //            //cmd.Parameters.Add(retval);
        //            //cmd.Parameters.Add(new OracleParameter("myfirstparam", OracleDbType.Varchar2, 50)).Value = "MyValue";
        //            //cmd.Parameters.Add(new OracleParameter("mysecondparam", OracleDbType.Decimal)).Value = 1;
        //            //OracleParameter inoutval = new OracleParameter("inoutval", OracleDbType.Varchar2, 50);
        //            //inoutval.Direction = ParameterDirection.InputOutput;
        //            //inoutval.Value = "Hello";
        //            //cmd.Parameters.Add(inoutval);
        //            //cmd.ExecuteNonQuery();
        //            //Console.WriteLine("Return value is {0}", retval.Value);
        //            //Console.WriteLine("InOut value is {0}", inoutval.Value);
        //            //cn.Close();

        //            String sql = "Select * From ISE_STP_STORIADIPENDENTE Order By PROGTRASFERIMENTO, DATADECORRENZA, TIPOMOVIMENTO, SEDE";
        //            OracleCommand cmd = new OracleCommand(sql, cn);
        //            cn.Open();
        //            OracleDataReader rdr = cmd.ExecuteReader();
        //            List<Stp_Storia_Dipendente> model = new List<Stp_Storia_Dipendente>();
        //            while (rdr.Read())
        //            {
        //                var details = new Stp_Storia_Dipendente();
        //                details.matricola = rdr["MATRICOLA"].ToString();
        //                details.nominativo = rdr["NOMINATIVO"].ToString();
        //                details.sede = rdr["SEDE"].ToString();
        //                details.valuta = rdr["VALUTA"].ToString();
        //                details.qualifica = rdr["QUALIFICA"].ToString();
        //                details.tipomovimento = rdr["TIPOMOVIMENTO"].ToString();
        //                details.dataDecorrenza = rdr["DATADECORRENZA"].ToString();
        //                details.dataLettera = rdr["DATALETTERA"].ToString();
        //                details.indennitaBase = rdr["INDENNITADIBASE"].ToString();
        //                details.coeffSede = rdr["COEFFSEDE"].ToString();
        //                details.percDisagio = rdr["PERCDISAGIO"].ToString();
        //                details.percAbbattimento = rdr["PERCABBATTIMENTO"].ToString();
        //                details.percConiuge = rdr["PERCCONIUGE"].ToString();
        //                details.pensioneConiuge = rdr["PENSIONECONIUGE"].ToString();
        //                details.numeroFigli = rdr["NUMEROFIGLI"].ToString();
        //                details.tassoCambio = rdr["TASSODCAMBIO"].ToString();
        //                details.indennitaPersonale = rdr["INDENNITAPERSONALE"].ToString();
        //                details.SistRientro = rdr["INDENNITASISTRIENTRO"].ToString();
        //                details.anticipo = rdr["ANTICIPO"].ToString();
        //                details.SistNetta = rdr["IndennitaSistRientroNetta"].ToString();
        //                details.progrTrasferimento = rdr["PROGTRASFERIMENTO"].ToString();
        //                model.Add(details);
        //            }





        //            //List<SelectListItem> li = new List<SelectListItem>();
        //            //li.Add(new SelectListItem { Text = "Select", Value = "0" });
        //            //li.Add(new SelectListItem { Text = "India", Value = "1" });
        //            //li.Add(new SelectListItem { Text = "Srilanka", Value = "2" });
        //            //li.Add(new SelectListItem { Text = "China", Value = "3" });
        //            //li.Add(new SelectListItem { Text = "Austrila", Value = "4" });
        //            //li.Add(new SelectListItem { Text = "USA", Value = "5" });
        //            //li.Add(new SelectListItem { Text = "UK", Value = "6" });
        //            //ViewData["country"] = li;


        //            List<SelectListItem> li = new List<SelectListItem>();
        //            string constr = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;
        //            OracleDataReader sdr = cmd.ExecuteReader();
        //            using (OracleConnection con = new OracleConnection(constr))
        //            {
        //                string query = " Select Distinct AND_MATRICOLA, AND_COGNOME ||' '|| AND_NOME NOMINATIVO From ANADIPE, TRASFERIMENTO Where AND_MATRICOLA = TRA_MATRICOLA Order By NOMINATIVO";
        //                using (OracleCommand cmd1 = new OracleCommand(query))
        //                {
        //                    cmd1.Connection = con;
        //                    con.Open();
        //                    {
        //                        while (sdr.Read())
        //                        {
        //                            li.Add(new SelectListItem
        //                            {
        //                                Text = sdr["Nominativo"].ToString(),
        //                                Value = sdr["Matricola"].ToString()

        //                            });
        //                        }

        //                        ViewData["Stampe"] = li;

        //                    }
        //                    con.Close();
        //                }
        //            }

        //            //Stp_Dipendenti categories = Dipendenti.GetAllDipendenti();
        //            //return View("BindWithDbValues", categories);



        //            //return PartialView("StoriaDipendente", model);
        //            //Stp_Storia_Dipendente categories = Dipendenti.GetAllDipendenti();
        //            return PartialView("StoriaDipendente", model);
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
            

        //}

        //DataSet4 ds4 = new DataSet4();
        
        //public ActionResult RptStoriaDipendente(string Text1)
        //{
        //    try
        //    {

        //        ReportViewer reportViewer = new ReportViewer();
        //        reportViewer.ProcessingMode = ProcessingMode.Local;
        //        reportViewer.SizeToReportContent = true;
        //        reportViewer.Width = Unit.Percentage(900);
        //        reportViewer.Height = Unit.Percentage(900);

        //        var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;

        //        OracleConnection conx = new OracleConnection(connectionString);
        //        OracleDataAdapter adp = new OracleDataAdapter("Select * From ISE_STP_STORIADIPENDENTE WHERE MATRICOLA = '" + Text1 + "'", conx);

        //        adp.Fill(ds4, ds4.ISE_STP_STORIADIPENDENTE.TableName);

        //        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\Report4.rdlc";
        //        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet4", ds4.Tables[0]));

        //        ViewBag.ReportViewer = reportViewer;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //    return View();
        //}
        

        ////private static List<SelectListItem> StampeDipendenti()
        ////{
        ////    List<SelectListItem> items = new List<SelectListItem>();
        ////    string constr = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;
        ////    using (OracleConnection con = new OracleConnection(constr))
        ////    {
        ////        string query = " SELECT *  FROM ANADIPE";
        ////        using (OracleCommand cmd = new OracleCommand(query))
        ////        {
        ////            cmd.Connection = con;
        ////            con.Open();
        ////            using (OracleDataReader sdr = cmd.ExecuteReader())
        ////            {
        ////                while (sdr.Read())
        ////                {
        ////                    items.Add(new SelectListItem
        ////                    {
        ////                        Text = sdr["Nominativo"].ToString(),
        ////                        Value = sdr["Matricola"].ToString()
        ////                    });
        ////                }
        ////            }
        ////            con.Close();
        ////        }
        ////    }

        ////    return items;
        ////}


        ////private List<Stp_Storia_Dipendente> GetCategories()
        ////{
        ////    string connection = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;
        ////    var categories = new List<Stp_Storia_Dipendente>();
        ////    var con = new OracleConnection(connection);
        ////    con.Open();
        ////    var command = new OracleCommand("Select * From ISE_STP_STORIADIPENDENTE Order By PROGTRASFERIMENTO, DATADECORRENZA, TIPOMOVIMENTO, SEDE", con);

        ////    OracleDataReader reader = command.ExecuteReader();
        ////    while (reader.Read())
        ////    {
        ////        categories.Add(new Stp_Storia_Dipendente { nominativo = Convert.ToString(reader["nominativo"]), matricola = Convert.ToInt32(reader["MATRICOLA"]) });

        ////    }
        ////    con.Close();


        ////    ViewBag.Categories = GetCategories().Where(x => x.Id < 10).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });

        ////    return PartialView("StoriaDipendente", model);


        ////}
        
        //public ActionResult BindWithDbValues()
        //{
        //    Stp_Storia_Dipendente categories = Dipendenti.GetAllDipendenti();
        //    return View("StoriaDipendente", categories);
        //}


        //public ActionResult StoriaDipendente()
        //{
        //    CategoryModel categories = CategoryService.GetAllCategories();
        //    return View("BindWithModel", categories);
        //}
    }
}

using NewISE.EF;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Controllers
{
    public class IndennitaController : Controller
    {
        // GET: Indennita
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GestioneIndennita(decimal idTrasferimento)
        {
            try
            {
                TrasferimentoModel tm = new TrasferimentoModel();
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    tm = dtt.GetTrasferimentoById(idTrasferimento);

                    var dataPartenza = tm.dataPartenza.ToShortDateString();
                    ViewData.Add("dataPartenza", dataPartenza);
                    ViewData.Add("Trasferimento", tm);
                }

                ViewBag.idTrasferimento = idTrasferimento;

                return PartialView("GestioneIndennita", tm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        public ActionResult AttivitaIndennita(decimal idTrasferimento)
        {
            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tr = dtt.GetTrasferimentoById(idTrasferimento);
                    using (dtDipendenti dtd = new dtDipendenti())
                    {
                        var d = dtd.GetDipendenteByIDTrasf(idTrasferimento);
                        if (tr != null && tr.HasValue())
                        {
                            ViewBag.idTrasferimento = tr.idTrasferimento;
                        }
                        else
                        {
                            throw new Exception("Nessun trasferimento per la matricola (" + d.matricola + ")");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });

            }
            
            return PartialView();
        }
        public JsonResult VerificaIndennita(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;

            try
            {
                if (idTrasferimento <= 0)
                {
                    throw new Exception(" non valorizzato");
                }
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    dipInfoTrasferimentoModel trm = dtt.GetInfoTrasferimento(idTrasferimento);
                    if (trm != null && (trm.statoTrasferimento == EnumStatoTraferimento.Attivo ||
                        trm.statoTrasferimento == EnumStatoTraferimento.Terminato))
                    {
                        ViewData["idTrasferimento"] = idTrasferimento;

                        return Json(new { VerificaIndennita = 1 });
                    }
                    else
                    {
                        return Json(new { VerificaIndennita = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }
        public ActionResult IndennitaBase(decimal idTrasferimento)
        {   
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();
             
            try
            {
                using (dtIndennitaBase dtd = new dtIndennitaBase())
                {
                    
                    libm = dtd.GetIndennitaBaseComune(idTrasferimento).ToList();
                }
                
                //ViewBag.idTrasferimento = idTrasferimento;
                return PartialView(libm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            
        }
        // Report Indennita Base
        public ActionResult RptIndennitaBase(decimal idTrasferimento)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

            try
            {

                using (ModelDBISE db = new ModelDBISE())
                {
                    var ll = db.INDENNITABASE.ToList();

                    libm = (from e in ll
                            select new IndennitaBaseModel()
                            {
                                idIndennitaBase = e.IDINDENNITABASE,
                                idLivello = e.IDLIVELLO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : e.DATAFINEVALIDITA,
                                valore = e.VALORE,
                                valoreResponsabile = e.VALORERESP,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                Livello = new LivelloModel()
                                {
                                    idLivello = e.LIVELLI.IDLIVELLO,
                                    DescLivello = e.LIVELLI.LIVELLO
                                },
                            }).ToList();
                }

                // ***************************************************************************
                // I COMMENTO

                //DataClassDataContext db = new DataClassDataContext();
                //var datasource = from c in db.sp_LinqTest(v_strCountry)
                //                 orderby c.CustomerID
                //                 select c;

                //ReportParameter rpCountry = new ReportParameter("p_Country", v_strCountry);
                //this.rdlcreport1.LocalReport.SetParameters(new ReportParameter[] { rpCountry });
                //this.rdlcreport1.LocalReport.DataSources.Add(new ReportDataSource("sp_LinqTestResult", datasource.ToList()));
                //this.rdlcreport1.LocalReport.Refresh();

                // ***************************************************************************


                // ***************************************************************************
                // II COMMENTO

                //ReportViewer reportViewer = new ReportViewer();
                //reportViewer.ProcessingMode = ProcessingMode.Local;
                //reportViewer.SizeToReportContent = true;
                //reportViewer.Width = Unit.Percentage(100);
                //reportViewer.Height = Unit.Percentage(100);

                //var connectionString = ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;
                //OracleConnection conx = new OracleConnection(connectionString);
                //String Sql = "Select * From table_name";

                //OracleDataAdapter adp = new OracleDataAdapter(Sql, conx);

                ////adp.Fill(ds13, ds13.V_PRESENZE_LIVELLI.TableName);
                //adp.Fill(ds13, ds13.DataTable13.TableName);

                //reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\RptPresenzeLivelli.rdlc";
                //reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet13", ds13.Tables[0]));

                //ReportParameter[] parameterValues = new ReportParameter[]
                //   {
                //        new ReportParameter ("fromDate",V_DATA),
                //        new ReportParameter ("toDate",V_DATA1)
                //   };

                //reportViewer.LocalReport.SetParameters(parameterValues);
                //reportViewer.LocalReport.Refresh();

                //ViewBag.ReportViewer = reportViewer;


                return PartialView();
            }
            
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            
        }
        public ActionResult IndennitaServizio(decimal idTrasferimento)
        {

            try
            {


                return PartialView();
            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }
        public ActionResult MaggiorazioniFamiliari(decimal idTrasferimento)
        {

            try
            {


                return PartialView();
            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }
        public ActionResult IndennitaPersonale(decimal idTrasferimento)
        {

            try
            {


                return PartialView();
            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }
        public ActionResult MaggiorazioneAbitazione(decimal idTrasferimento)
        {

            try
            {


                return PartialView();
            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }
        public ActionResult IndennitaPrimaSistemazione(decimal idTrasferimento)
        {

            try
            {


                return PartialView();
            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }
        public ActionResult IndennitadiRichiamo(decimal idTrasferimento)
        {

            try
            {


                return PartialView();
            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }

    }
}
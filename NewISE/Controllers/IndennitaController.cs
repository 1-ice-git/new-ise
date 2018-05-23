
using Microsoft.Reporting.WebForms;
using NewISE.EF;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Tools;
using NewISE.Views.Dataset;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

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
            //List<RuoloUfficioModel> lru = new List<RuoloUfficioModel>();

            try
            {
                using (dtIndennitaBase dtd = new dtIndennitaBase())
                {
                    
                    libm = dtd.GetIndennitaBaseComune(idTrasferimento).ToList();
                }

                //using (dtRuoloDipendente drd = new dtRuoloDipendente())
                //{

                //    lru = drd.GetIndennitaBaseComuneRuoloDipendente(idTrasferimento).ToList();
                //}

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tm = dtt.GetTrasferimentoById(idTrasferimento);
                    using (dtRuoloUfficio dtru = new dtRuoloUfficio())
                    {
                        tm.RuoloUfficio = dtru.GetRuoloUfficioValidoByIdTrasferimento(tm.idTrasferimento);
                        tm.idRuoloUfficio = tm.RuoloUfficio.idRuoloUfficio;
                        ViewBag.idRuoloUfficio = tm.idRuoloUfficio;

                    }
                }
                ViewBag.idTrasferimento = idTrasferimento;
                

                return PartialView(libm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            
        }

        DSIndennitaBase ds = new DSIndennitaBase();
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

                   
                    ReportViewer reportViewer = new ReportViewer();

                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    reportViewer.SizeToReportContent = true;
                    reportViewer.Width = Unit.Percentage(100);
                    reportViewer.Height = Unit.Percentage(100);

                    var datasource = new ReportDataSource("DSIndennitaBase", ll.ToList());
                    ////var datasource = new ReportDataSource("INDENNITABASE", ll.ToList());
                    reportViewer.Visible = true;
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    //reportViewer.LocalReport.ReportPath = @"~/Report/RptIndennitaBase.rdlc";
                    reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"~/Report/RptIndennitaBase.rdlc";
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(datasource);
                    //reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSIndennitaBase", ds.Tables[0]));
                    reportViewer.LocalReport.Refresh();

                    ViewBag.ReportViewer = reportViewer;

                    
                }
            }
            
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return View("RptIndennitaBase2");

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
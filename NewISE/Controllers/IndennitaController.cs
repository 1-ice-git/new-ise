﻿
using Microsoft.Reporting.WebForms;
using NewISE.EF;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.dtObj;
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.Models.Enumeratori;
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

        // Indennità Base + Report di Stampa
        public ActionResult IndennitaBase(decimal idTrasferimento)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();
            List<LivelloDipendenteModel> lldm = new List<LivelloDipendenteModel>();
            

            try
            {
                using (dtIndennitaBase dtd = new dtIndennitaBase())
                {
                    libm = dtd.GetIndennitaBaseComune(idTrasferimento).ToList();
                }


                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tm = dtt.GetTrasferimentoById(idTrasferimento);

                    using (dtRuoloUfficio dtru = new dtRuoloUfficio())
                    {
                        tm.RuoloUfficio = dtru.GetRuoloUfficioValidoByIdTrasferimento(tm.idTrasferimento);
                        tm.idRuoloUfficio = tm.RuoloUfficio.idRuoloUfficio;
                        ViewBag.idRuoloUfficio = tm.idRuoloUfficio;

                    }

                    //using (dtLivelliDipendente dtld = new dtLivelliDipendente())
                    //{
                    //    lldm = dtld.GetLivelloDipendenteByIdTrasferimento(idTrasferimento).ToList();
                    //    //lldm = dtld.GetLivelloDipendenteByIdTrasferimento(idTrasferimento).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValdita).ThenBy(a => a.dataFineValidita).ToList();
                        
                    //}

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
            List<LivelloDipendenteModel> ldm = new List<LivelloDipendenteModel>();

            try
            {

                using (ModelDBISE db = new ModelDBISE())
                {

                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var tm = dtt.GetTrasferimentoById(idTrasferimento);

                        using (dtRuoloUfficio dtru = new dtRuoloUfficio())
                        {

                            using (dtLivelliDipendente dld = new dtLivelliDipendente())
                            {
                                // Ruolo

                                var ll = db.INDENNITA.Find(idTrasferimento).INDENNITABASE.Where(a => a.ANNULLATO == false).ToList();

                                tm.RuoloUfficio = dtru.GetRuoloUfficioValidoByIdTrasferimento(tm.idTrasferimento);
                                tm.idRuoloUfficio = tm.RuoloUfficio.idRuoloUfficio;

                                ViewBag.idRuoloUfficio = tm.idRuoloUfficio;
                                ViewBag.idTrasferimento = idTrasferimento;

                                decimal valore = 0;

                                if (ViewBag.idRuoloUfficio == (decimal)EnumFaseRuoloDipendente.Collaboratore || ViewBag.idRuoloUfficio == (decimal)EnumFaseRuoloDipendente.Assistente)
                                {
                                    valore = ll.First().VALORE;

                                }
                                else
                                {
                                    valore = ll.First().VALORERESP;

                                }

                                // Livello

                                var liv = dld.GetLivelloDipendenteByIdTrasferimento(idTrasferimento);
                                var liv1 = liv.First();

                                string Nominativo = tm.Dipendente.Nominativo;
                                string Ruolo = tm.RuoloUfficio.DescrizioneRuolo;
                                string Livello = liv1.Livello.DescLivello;
                                string Decorrenza = Convert.ToDateTime(tm.dataPartenza).ToShortDateString();


                                libm = (from e in ll
                                        select new IndennitaBaseModel()
                                        {
                                            idIndennitaBase = e.IDINDENNITABASE,
                                            idLivello = e.IDLIVELLO,
                                            dataInizioValidita = e.DATAINIZIOVALIDITA,
                                            dataFineValidita = e.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : e.DATAFINEVALIDITA,
                                            valore = valore,
                                            //valoreResponsabile = e.VALORERESP,
                                            dataAggiornamento = e.DATAAGGIORNAMENTO,
                                            Livello = new LivelloModel()
                                            {
                                                idLivello = e.LIVELLI.IDLIVELLO,
                                                DescLivello = e.LIVELLI.LIVELLO
                                            },
                                        }).ToList();


                                // ****************************************************************************

                                // set param values, not really doing anything except showing up in the report,
                                // after the fact.
                                //string paraStartDate = Convert.ToDateTime("1/1/2009").ToShortDateString();
                                //string paraEndDate = Convert.ToDateTime("12/1/2009").ToShortDateString();
                                //ReportParameter[] param = new ReportParameter[2];
                                //param[0] = new ReportParameter("paraStartDate", paraStartDate, false);
                                //param[1] = new ReportParameter("paraEndDate", paraEndDate, false);
                                //this.reportViewer.LocalReport.SetParameters(param);

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
                                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptIndennitaBase.rdlc";
                                reportViewer.LocalReport.DataSources.Clear();
                                reportViewer.LocalReport.DataSources.Add(datasource);
                                //reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSIndennitaBase", ds.Tables[0]));
                                reportViewer.LocalReport.Refresh();

                                ReportParameter[] parameterValues = new ReportParameter[]
                                {
                                    new ReportParameter ("Nominativo",Nominativo),
                                    new ReportParameter ("Ruolo",Ruolo),
                                    new ReportParameter ("Livello",Livello),
                                    new ReportParameter ("Decorrenza",Decorrenza)
                                };

                                reportViewer.LocalReport.SetParameters(parameterValues);
                                ViewBag.ReportViewer = reportViewer;

                            }

                        }
                    }
                }
            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("RptIndennitaBase");

        }

        // Indennità di Servizio + Report di Stampa
        public ActionResult IndennitaServizio(decimal idTrasferimento)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();
            dipInfoTrasferimentoModel dit = new dipInfoTrasferimentoModel();

            try
            {
                using (dtIndennitaServizio dtd = new dtIndennitaServizio())
                {
                    libm = dtd.GetIndennitaServizio(idTrasferimento).ToList();
                }

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tm = dtt.GetTrasferimentoById(idTrasferimento);
                    using (dtRuoloUfficio dtru = new dtRuoloUfficio())
                    {
                        tm.RuoloUfficio = dtru.GetRuoloUfficioValidoByIdTrasferimento(tm.idTrasferimento);
                        tm.idRuoloUfficio = tm.RuoloUfficio.idRuoloUfficio;
                        ViewBag.idRuoloUfficio = tm.idRuoloUfficio;

                    }

                    using (CalcoliIndennita ci = new CalcoliIndennita(tm.idTrasferimento))
                    {
                        dit.indennitaBase = ci.IndennitaDiBase;
                        dit.indennitaServizio = ci.IndennitaDiServizio;
                        dit.maggiorazioniFamiliari = ci.MaggiorazioniFamiliari;
                        dit.indennitaPersonale = ci.IndennitaPersonale;
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
        public ActionResult RptIndennitaServizio(decimal idTrasferimento)
        {

            try
            {

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tm = dtt.GetTrasferimentoById(idTrasferimento);

                    ViewBag.idTrasferimento = idTrasferimento;

                    string Nominativo = tm.Dipendente.Nominativo;

                    ReportViewer reportViewer = new ReportViewer();

                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    reportViewer.SizeToReportContent = true;
                    reportViewer.Width = Unit.Percentage(100);
                    reportViewer.Height = Unit.Percentage(100);

                    //var datasource = new ReportDataSource("DSRiepilogoVoci", lTeorici.ToList());
                    reportViewer.Visible = true;
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptIndennitaServizio.rdlc";
                    reportViewer.LocalReport.DataSources.Clear();
                    //reportViewer.LocalReport.DataSources.Add(datasource);

                    reportViewer.LocalReport.Refresh();
                    reportViewer.ShowReportBody = true;

                    ReportParameter[] parameterValues = new ReportParameter[]
                    {
                        new ReportParameter ("Nominativo",Nominativo)
                    };

                    reportViewer.LocalReport.SetParameters(parameterValues);
                    ViewBag.ReportViewer = reportViewer;
                }


            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView("RptIndennitaServizio");
        }

        // Maggiorazioni Familiari + Report di Stampa
        public ActionResult MaggiorazioniFamiliari(decimal idTrasferimento)
        {
            List<ConiugeModel> ListaConiuge = new List<ConiugeModel>();
            List<FigliModel> ListaFigli = new List<FigliModel>();


            try
            {

                using (dtMaggiorazioniFamiliari dtd = new dtMaggiorazioniFamiliari())
                {
                    //ListaConiuge = dtd.GetMaggiorazioniFamiliaribyConiuge(idTrasferimento).ToList();
                }



                return PartialView();
            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }
        public ActionResult RptMaggiorazioniFamiliari()
        {
            return View();
        }

        // Indennità Personale + Report di Stampa
        public ActionResult IndennitaPersonale(decimal idTrasferimento)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();
            dipInfoTrasferimentoModel dit = new dipInfoTrasferimentoModel();

            try
            {
                using (dtIndennitaPersonale dtd = new dtIndennitaPersonale())
                {
                    libm = dtd.GetIndennitaPersonale(idTrasferimento).ToList();
                }

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tm = dtt.GetTrasferimentoById(idTrasferimento);
                    

                    using (CalcoliIndennita ci = new CalcoliIndennita(tm.idTrasferimento))
                    {
                        dit.indennitaBase = ci.IndennitaDiBase;
                        dit.indennitaServizio = ci.IndennitaDiServizio;
                        dit.maggiorazioniFamiliari = ci.MaggiorazioniFamiliari;
                        dit.indennitaPersonale = ci.IndennitaPersonale;
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
        public ActionResult RptIndennitaPersonale()
        {
            return View();
        }

        // Maggiorazione Abitazione + Report di Stampa
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

        public ActionResult RptMaggiorazioneAbitazione()
        {
            return View();
        }

        // Indennita di Prima Sistemazione + Report di Stampa
        public ActionResult IndennitaPrimaSistemazione(decimal idTrasferimento)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();
            dipInfoTrasferimentoModel dit = new dipInfoTrasferimentoModel();
            List<IndennitaSistemazioneModel> lism = new List<IndennitaSistemazioneModel>();

            try
            {
                using (dtIndennitaPersonale dtd = new dtIndennitaPersonale())
                {
                    libm = dtd.GetIndennitaPersonale(idTrasferimento).ToList();
                }


                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tm = dtt.GetTrasferimentoById(idTrasferimento);

                    using (CalcoliIndennita ci = new CalcoliIndennita(tm.idTrasferimento))
                    {
                        dit.coefficienteIndennitàSistemazione = ci.CoefficienteIndennitaSistemazione;
                        
                    }

                }
                    return PartialView(libm);
            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }

        public ActionResult RptIndennitaPrimaSistemazione()
        {
            return View();
        }

        // Indennita di Richiamo + Report di Stampa
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

        public ActionResult RptIndennitadiRichiamo()
        {
            return View();
        }
    }
}
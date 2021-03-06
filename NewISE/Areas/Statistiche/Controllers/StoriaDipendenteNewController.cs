﻿
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using NewISE.EF;
using NewISE.Areas.Statistiche.Models;
using System.Web.UI.WebControls;

namespace NewISE.Areas.Statistiche.Controllers
{
    public class StoriaDipendenteNewController : Controller
    {
        // GET: Statistiche/StoriaDipendenteNew
        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult DipendentiGepe(string matricola = "")
        {
            var rMatricola = new List<SelectListItem>();
            var rNominativo = new List<SelectListItem>();
            bool admin = false;
            List<DipendentiModel> ldm = new List<DipendentiModel>();
            DipendentiModel dm = new DipendentiModel();
            AccountModel ac = new AccountModel();
            try
            {

                admin = Utility.Amministratore(out ac);

                using (dtDipendenti dtd = new dtDipendenti())
                {
                    if (!admin)
                    {
                        ldm.Add(dtd.GetDipendenteByMatricola(Convert.ToInt16(ac.utente)));
                    }
                    else if (matricola != string.Empty && admin == false)
                    {
                        ldm.Add(dtd.GetDipendenteByMatricola(Convert.ToInt16(matricola)));
                    }
                    else
                    {
                        ldm = dtd.GetDipendenti().ToList();
                    }

                    if (ldm.Count > 0)
                    {
                        foreach (var item in ldm)
                        {
                            rMatricola.Add(new SelectListItem()
                            {
                                Text = item.matricola.ToString(),
                                Value = item.matricola.ToString()
                            });

                            rNominativo.Add(new SelectListItem()
                            {
                                Text = item.Nominativo + " (" + item.matricola.ToString() + ")",
                                Value = item.matricola.ToString()
                            });

                        }

                        rMatricola.Insert(0, new SelectListItem() { Text = "", Value = "" });

                        rNominativo.Insert(0, new SelectListItem() { Text = "", Value = "" });

                        if (matricola == string.Empty)
                        {
                            rMatricola.First().Selected = true;
                            rNominativo.First().Selected = true;
                        }
                        else
                        {
                            foreach (var item in rMatricola)
                            {
                                if (matricola == item.Value)
                                {
                                    item.Selected = true;
                                }
                            }

                            foreach (var item in rNominativo)
                            {
                                if (matricola == item.Value)
                                {
                                    item.Selected = true;
                                }
                            }
                        }
                    }
                }


                if (matricola != string.Empty)
                {
                    dm = ldm.Where(a => a.matricola == Convert.ToInt16(matricola)).First();

                    using (dtCDCGepe dtcdcg = new dtCDCGepe())
                    {
                        dm.cdcGepe = dtcdcg.GetCDCGepe(dm.idDipendente);
                    }

                    using (dtLivelliDipendente dtpl = new dtLivelliDipendente())
                    {
                        dm.livelloDipendenteValido = dtpl.GetLivelloDipendente(dm.idDipendente, DateTime.Now.Date);
                    }
                }


                //ViewBag.ListDipendentiGepeMatricola = rMatricola.OrderBy(a=>a.Text);
                ViewBag.ListDipendentiGepeNominativo = rNominativo.OrderBy(a => a.Text);
                ViewBag.Amministratore = admin;
                ViewBag.Matricola = dm.matricola;
                ViewBag.Nominativo = dm.Nominativo;
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(dm);
        }
        public ActionResult RptStoriaDipendenteNew(decimal Nominativo)
        {
            List<StoriaDipendenteNewModel> rim = new List<StoriaDipendenteNewModel>();
            List<RptStoriaDipendenteNewModel> rpt = new List<RptStoriaDipendenteNewModel>();

            try
            {

                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtStoriaDipendenteNew dtSD = new dtStoriaDipendenteNew())
                    {
                        rim = dtSD.GetStoriaDipendenteNew(Nominativo, db).ToList().OrderBy(a => a.dataPartenza).ToList();
                    }

                    if (rim?.Any() ?? false)
                    {
                        decimal ordinamento = 0;
                        foreach (var lm in rim)
                        {
                            ordinamento++;

                            RptStoriaDipendenteNewModel rptds = new RptStoriaDipendenteNewModel()
                            {
                                nome = lm.nome,
                                dataAssunzione = lm.dataAssunzione.ToShortDateString(),
                                dataVariazione = lm.dataVariazione,
                                Ufficio = lm.Ufficio,
                                DescLivello =  lm.DescLivello,
                                dataPartenza = lm.dataPartenza.ToShortDateString(),
                                dataRientro = (lm.dataRientro < Utility.DataFineStop()) ? Convert.ToDateTime(lm.dataRientro).ToShortDateString() : " ",
                                valore = lm.valore,
                                percentuale = lm.percentuale,
                                indennita = lm.indennita,
                                ValutaUfficio = lm.ValutaUfficio,
                                IndennitaBase = lm.IndennitaBase,
                                IndennitaPersonale = lm.IndennitaPersonale,
                                IndennitaServizio = lm.IndennitaServizio,
                                MaggiorazioniFamiliari = lm.MaggiorazioniFamiliari,
                                PensioneConiuge = lm.PensioneConiuge,
                                PrimaSistemazione = lm.PrimaSistemazione,
                                ordinamento = ordinamento                               
                            };

                            rpt.Add(rptds);

                        }
                    }

                    string strDataOdierna = DateTime.Now.ToShortDateString();

                    ReportViewer reportViewer = new ReportViewer();

                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    reportViewer.SizeToReportContent = true;
                    reportViewer.Width = Unit.Percentage(100);
                    reportViewer.Height = Unit.Percentage(100);

                    var datasource = new ReportDataSource("DataSetStoriaDipendenteNew");

                    reportViewer.Visible = true;
                    reportViewer.ProcessingMode = ProcessingMode.Local;

                    reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\RptStoriaDipendenteNew.rdlc";

                    ReportParameter[] parameterValues = new ReportParameter[]
                    {
                        new ReportParameter ("paramDataOdierna", strDataOdierna)
                    };
                    reportViewer.LocalReport.SetParameters(parameterValues);

                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(datasource);
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetStoriaDipendenteNew", rpt));
                    reportViewer.LocalReport.Refresh();

                    ViewBag.ReportViewer = reportViewer;

                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("RptStoriaDipendenteNew");
        }

    }
}
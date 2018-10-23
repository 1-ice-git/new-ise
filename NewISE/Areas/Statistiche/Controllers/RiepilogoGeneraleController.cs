﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.EF;
using NewISE.Areas.Statistiche.Models;
using NewISE.Areas.Statistiche.Models.dtObj;
using NewISE.Models.DBModel.dtObj;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.Enumeratori;

namespace NewISE.Areas.Statistiche.Controllers
{
    public class RiepilogoGeneraleController : Controller
    {
        public ActionResult Index()
        {
            var r = new List<SelectListItem>();
            var rMeseAnno = new List<SelectListItem>();
            List<MeseAnnoElaborazioneModel> lmaem = new List<MeseAnnoElaborazioneModel>();

            try
            {
                int anno = DateTime.Now.Year;
                int mese = DateTime.Now.Month;

                using (ModelDBISE db = new ModelDBISE())
                {

                    using (dtElaborazioni dte = new dtElaborazioni())
                    {
                        lmaem = dte.PrelevaAnniMesiElaborati().ToList();

                        foreach (var item in lmaem)
                        {
                            rMeseAnno.Add(new SelectListItem()
                            {
                                Text = CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)item.mese) + "-" + item.anno.ToString("D4"),
                                Value = item.idMeseAnnoElab.ToString()
                            });
                        }

                        if (rMeseAnno.Exists(a => a.Text == CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)mese) + "-" + anno.ToString("D4")))
                        {
                            foreach (var item in rMeseAnno)
                            {
                                if (item.Text == CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)mese) + "-" + anno.ToString("D4"))
                                {
                                    item.Selected = true;
                                }
                            }
                        }
                        else
                        {
                            rMeseAnno.First().Selected = true;
                        }
                    }

                    ViewData["listMesiAnniElaboratiDa"] = rMeseAnno;
                    ViewData["listMesiAnniElaboratiA"] = rMeseAnno;
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }

        public JsonResult PrelevaMesiAnniElab(string search)
        {
            List<Select2Model> ls2 = new List<Select2Model>();
            List<MeseAnnoElaborazioneModel> lmaem = new List<MeseAnnoElaborazioneModel>();

            try
            {
                using (dtElaborazioni dte = new dtElaborazioni())
                {
                    lmaem = dte.PrelevaAnniMesiElaborati().ToList();

                    foreach (var mae in lmaem)
                    {
                        Select2Model s2 = new Select2Model()
                        {
                            id = mae.idMeseAnnoElab.ToString(),
                            text = CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)mae.mese) + "-" + mae.anno.ToString("D4"),
                        };
                        ls2.Add(s2);
                    }
                }

                if (search != null && search != string.Empty)
                {
                    ls2 = ls2.Where(a => a.text.ToUpper().Contains(search.ToUpper())).ToList();
                }
            }
            catch (Exception ex)
            {
                return Json(new { results = new List<Select2Model>(), err = ex.Message });
            }
            return Json(new { results = ls2, err = "" });
        }

        public ActionResult RptRiepilogoGenerale(decimal idMeseAnnoDa, decimal idMeseAnnoA)
        {
            List<RptRiepilogoGeneraleModel> lrpt = new List<RptRiepilogoGeneraleModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var annoMeseElabDa = db.MESEANNOELABORAZIONE.Find(idMeseAnnoDa);
                    decimal annoMeseDa = Convert.ToDecimal(annoMeseElabDa.ANNO.ToString() + annoMeseElabDa.MESE.ToString().PadLeft(2, Convert.ToChar("0")));
                    decimal annoDa = annoMeseElabDa.ANNO;
                    decimal meseDa = annoMeseElabDa.MESE;

                    var annoMeseElabA = db.MESEANNOELABORAZIONE.Find(idMeseAnnoA);
                    decimal annoMeseA = Convert.ToDecimal(annoMeseElabA.ANNO.ToString() + annoMeseElabA.MESE.ToString().PadLeft(2, Convert.ToChar("0")));
                    decimal annoA = annoMeseElabA.ANNO;
                    decimal meseA = annoMeseElabA.MESE;

                    using (dtRiepilogoGenerale dtrg = new dtRiepilogoGenerale())
                    {
                        lrpt = dtrg.GetRiepilogoGenerale(meseDa, annoDa, meseA, annoA, db);
                    }


                    string strMeseAnnoDa = "";
                    string strMeseAnnoA = "";

                    using (dtElaborazioni dte = new dtElaborazioni())
                    {
                        strMeseAnnoDa = CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)meseDa) + " " + annoDa.ToString();
                        strMeseAnnoA = CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)meseA) + " " + annoA.ToString();
                    }

                    ReportViewer reportViewer = new ReportViewer();

                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    reportViewer.SizeToReportContent = true;
                    reportViewer.Width = Unit.Percentage(100);
                    reportViewer.Height = Unit.Percentage(100);
                    reportViewer.Visible = true;
                    reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Areas/Statistiche/RPT/RptRiepilogoGenerale.rdlc";
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.Refresh();

                    ReportParameter[] parameterValues = new ReportParameter[]
                       {
                            new ReportParameter ("paramMeseAnnoDa", strMeseAnnoDa),
                            new ReportParameter ("paramMeseAnnoA",strMeseAnnoA),
                       };

                    reportViewer.LocalReport.SetParameters(parameterValues);
                    ReportDataSource _rsource = new ReportDataSource("dsRiepilogoGenerale", lrpt);
                    reportViewer.LocalReport.DataSources.Add(_rsource);
                    reportViewer.LocalReport.Refresh();

                    ViewBag.ReportViewer = reportViewer;
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }

    }
}
using System;
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
    public class RiepilogoLivelloController : Controller
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
                    using (dtCostiCoan dtcc = new dtCostiCoan())
                    {
                        var ltcm = dtcc.GetElencoCoan(db);

                        if (ltcm != null && ltcm.Count > 0)
                        {
                            r = (from tcm in ltcm
                                 select new SelectListItem()
                                 {
                                     Text = tcm.codiceCoan,
                                     Value = tcm.idElencoCoan
                                 }).ToList();

                            r.First().Selected = true;
                        }
                        ViewBag.ElencoCoanList = r;
                    }

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

        public ActionResult RptCostiCoan(decimal meseAnnoDa, decimal meseAnnoA, string codiceCoan)
        {
            List<RptCostiCoanModel> lrpt = new List<RptCostiCoanModel>();

            try
            {

                using (ModelDBISE db = new ModelDBISE())
                {

                    var annoMeseElabDa = db.MESEANNOELABORAZIONE.Find(meseAnnoDa);
                    decimal annoMeseDa = Convert.ToDecimal(annoMeseElabDa.ANNO.ToString() + annoMeseElabDa.MESE.ToString().PadLeft(2, Convert.ToChar("0")));
                    decimal annoDa = annoMeseElabDa.ANNO;
                    decimal meseDa = annoMeseElabDa.MESE;


                    var annoMeseElabA = db.MESEANNOELABORAZIONE.Find(meseAnnoA);
                    decimal annoMeseA = Convert.ToDecimal(annoMeseElabA.ANNO.ToString() + annoMeseElabA.MESE.ToString().PadLeft(2, Convert.ToChar("0")));
                    decimal annoA = annoMeseElabA.ANNO;
                    decimal meseA = annoMeseElabA.MESE;

                    using (dtCostiCoan dtc = new dtCostiCoan())
                    {
                        lrpt = dtc.GetCostiCoan(meseDa, annoDa, meseA, annoA, codiceCoan, db).OrderBy(a => a.Nominativo).ToList(); ;
                    }

                    using (dtStatistiche dts = new dtStatistiche())
                    {
                        if (codiceCoan.Length < 10)
                        {
                            codiceCoan = dts.GetDescrizioneCoan(EnumTipologiaCoan.Servizi_Istituzionali, db);
                        }
                    }

                    string strMeseAnnoDa = "";
                    string strMeseAnnoA = "";
                    string strTotaleImporto = lrpt.Sum(a => a.Importo).ToString("#,##0.##");

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
                    reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Areas/Statistiche/RPT/RptCostiCoan.rdlc";
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.Refresh();

                    ReportParameter[] parameterValues = new ReportParameter[]
                       {
                            new ReportParameter ("paramMeseAnnoDa", strMeseAnnoDa),
                            new ReportParameter ("paramMeseAnnoA",strMeseAnnoA),
                            new ReportParameter ("paramCoan", codiceCoan),
                            new ReportParameter ("paramTotaleImporto", strTotaleImporto)
                       };

                    reportViewer.LocalReport.SetParameters(parameterValues);
                    ReportDataSource _rsource = new ReportDataSource("dsCostiCoan", lrpt);
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
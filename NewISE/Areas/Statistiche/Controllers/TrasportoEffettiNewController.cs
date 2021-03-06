﻿using Microsoft.Reporting.WebForms;
using NewISE.Areas.Statistiche.Models;
using NewISE.EF;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.dtObj.ModelliCalcolo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace NewISE.Areas.Statistiche.Controllers
{
    public class TrasportoEffettiNewController : Controller
    {
        // GET: Statistiche/TrasportoEffettiNew
        public ActionResult Index()
        {
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
                            text = CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)mae.mese) + "-" + mae.anno.ToString(),
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
        public ActionResult SelezionaMeseAnno(int mese = 0, int anno = 0)
        {
            var rMeseAnno = new List<SelectListItem>();
            List<MeseAnnoElaborazioneModel> lmaem = new List<MeseAnnoElaborazioneModel>();

            try
            {
                if (anno == 0)
                {
                    anno = DateTime.Now.Year;
                }

                if (mese == 0)
                {
                    mese = DateTime.Now.Month;
                }

                using (dtElaborazioni dte = new dtElaborazioni())
                {
                    lmaem = dte.PrelevaAnniMesiElaborati().ToList();

                    foreach (var item in lmaem)
                    {

                        rMeseAnno.Add(new SelectListItem()
                        {
                            Text = CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)item.mese) + "-" + item.anno.ToString(),
                            Value = item.idMeseAnnoElab.ToString()
                        });

                    }

                    if (rMeseAnno.Exists(a => a.Text == CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)mese) + "-" + anno.ToString()))
                    {
                        foreach (var item in rMeseAnno)
                        {
                            if (item.Text == CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)mese) + "-" + anno.ToString())
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

                ViewData["ElencoMesiAnniElaborati"] = rMeseAnno;
                ViewData["ElencoMesiAnniElaborati1"] = rMeseAnno;
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }
        public ActionResult RptTrasportoEffetti(decimal dtIni, decimal dtFin)
        {
            List<TrasportoEffettiModel> rim = new List<TrasportoEffettiModel>();
            List<RptTrasportoEffettiModel> rpt = new List<RptTrasportoEffettiModel>();

            try
            {

                using (ModelDBISE db = new ModelDBISE())
                {

                    var annoMeseElabDa = db.MESEANNOELABORAZIONE.Find(dtIni);
                    decimal annoMeseDa = Convert.ToDecimal(annoMeseElabDa.ANNO.ToString() + annoMeseElabDa.MESE.ToString().PadLeft(2, Convert.ToChar("0")));
                    decimal annoDa = annoMeseElabDa.ANNO;
                    decimal meseDa = annoMeseElabDa.MESE;


                    var annoMeseElabA = db.MESEANNOELABORAZIONE.Find(dtFin);
                    decimal annoMeseA = Convert.ToDecimal(annoMeseElabA.ANNO.ToString() + annoMeseElabA.MESE.ToString().PadLeft(2, Convert.ToChar("0")));
                    decimal annoA = annoMeseElabA.ANNO;
                    decimal meseA = annoMeseElabA.MESE;


                    using (dtReportTrasportoEffetti dtR = new dtReportTrasportoEffetti())
                    {
                        rim = dtR.GetTrasportoEffetti(dtIni, dtFin, annoDa, meseDa, annoA, meseA, db).ToList();
                    }


                    string strMeseAnnoDa = "";
                    string strMeseAnnoA = "";
                    using (dtElaborazioni dte = new dtElaborazioni())
                    {
                        strMeseAnnoDa = CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)meseDa) + " " + annoDa.ToString();
                        strMeseAnnoA = CalcoloMeseAnnoElaborazione.NomeMese((EnumDescrizioneMesi)meseA) + " " + annoA.ToString();
                    }


                    if (rim?.Any() ?? false)
                    {
                        foreach (var lm in rim)
                        {
                            RptTrasportoEffettiModel rptds = new RptTrasportoEffettiModel()
                            {
                                
                                matricola = lm.matricola,
                                Nominativo = lm.Nominativo,
                                Importo = lm.Importo,
                                sede = lm.Ufficio,
                                MeseElaborazione = lm.MeseElaborazione,
                                MeseRiferimento = lm.MeseRiferimento,
                                numMeseElaborazione = lm.numMeseElaborazione,
                                numMeseRiferimento = lm.numMeseRiferimento

                            };

                            rpt.Add(rptds);
                        }
                    }

                    var annoMeseElab = db.MESEANNOELABORAZIONE.Find(dtIni);
                    decimal annoMese = Convert.ToDecimal(annoMeseElab.ANNO.ToString() + annoMeseElab.MESE.ToString().PadLeft(2, Convert.ToChar("0")));

                    var annoMeseElab1 = db.MESEANNOELABORAZIONE.Find(dtFin);
                    decimal annoMese1 = Convert.ToDecimal(annoMeseElab1.ANNO.ToString() + annoMeseElab1.MESE.ToString().PadLeft(2, Convert.ToChar("0")));

                    string strDataOdierna = DateTime.Now.ToShortDateString();

                    ReportViewer reportViewer = new ReportViewer();

                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    reportViewer.SizeToReportContent = true;
                    reportViewer.Width = Unit.Percentage(100);
                    reportViewer.Height = Unit.Percentage(100);

                    var datasource = new ReportDataSource("DataSetTrasportoEffetti");

                    reportViewer.Visible = true;
                    reportViewer.ProcessingMode = ProcessingMode.Local;

                    reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\RptTrasportoEffetti.rdlc";
                    reportViewer.LocalReport.DataSources.Clear();

                    reportViewer.LocalReport.DataSources.Add(datasource);
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetTrasportoEffetti", rpt));
                    reportViewer.LocalReport.Refresh();

                    ReportParameter[] parameterValues = new ReportParameter[]
                    {
                        new ReportParameter ("Dal",strMeseAnnoDa),
                        new ReportParameter ("Al",strMeseAnnoA),
                        new ReportParameter ("DataOdierna", strDataOdierna)
                    };
                    reportViewer.LocalReport.SetParameters(parameterValues);

                    ViewBag.ReportViewer = reportViewer;
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("RptTrasportoEffetti");
        }

    }
}
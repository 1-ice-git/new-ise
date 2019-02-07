using Microsoft.Reporting.WebForms;
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
    public class RiepiloghiSospensioniController : Controller
    {
        // GET: Statistiche/RiepiloghiSospensioni
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

        public ActionResult RptRiepiloghiSospensioni(DateTime idIni, DateTime idFin)
        {
            List<RiepiloghiSospensioniModel> rim = new List<RiepiloghiSospensioniModel>();
            List<RptRiepiloghiSospensioniModel> rpt = new List<RptRiepiloghiSospensioniModel>();

            try
            {

                using (ModelDBISE db = new ModelDBISE())
                {
                    

                    using (dtRiepiloghiSospensioni dtr = new dtRiepiloghiSospensioni())
                    {
                       rim = dtr.GetRiepiloghiSospensioni(idIni, idFin, db).ToList();
                       
                    }


                    if (rim?.Any() ?? false)
                    {
                        foreach (var lm in rim)
                        {
                            RptRiepiloghiSospensioniModel rptds = new RptRiepiloghiSospensioniModel()
                            {
                                DataInizioSospensione = lm.DataInizioSospensione,
                                DataFineSospensione = lm.DataFineSospensione,
                                DataAggiornamento = lm.DataAggiornamento,
                                TipoSospensione = lm.TipoSospensione,
                                Nominativo = lm.Nominativo,
                                Ufficio = lm.Ufficio,
                                NumeroGiorni = lm.NumeroGiorni

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

                    var datasource = new ReportDataSource("DataSetRiepiloghiSospensioni");

                    reportViewer.Visible = true;
                    reportViewer.ProcessingMode = ProcessingMode.Local;

                    reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Areas\Statistiche\RPT\RptSospensioni.rdlc";
                    reportViewer.LocalReport.DataSources.Clear();

                    reportViewer.LocalReport.DataSources.Add(datasource);
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetRiepiloghiSospensioni", rpt));
                    reportViewer.LocalReport.Refresh();


                    ReportParameter[] parameterValues = new ReportParameter[]
                    {
                        new ReportParameter ("Dal",idIni.ToShortDateString()),
                        new ReportParameter ("Al",idFin.ToShortDateString()),
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

            return PartialView();
        }

        public JsonResult ValidazioneDateSospensioni(DateTime dtIni, DateTime dtFin)
        {
            try
            {
                if (dtIni > dtFin)
                {
                    return Json(new { datevalide = 0, err = "" });
                }
                else
                {
                    return Json(new { datevalide = 1, err = "" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }

    }
}
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using NewISE.EF;
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Tools;
using NewISE.Models;
using NewISE.Models.ViewModel;
using NewISE.Models.DBModel;
using NewISE.Models.Enumeratori;
using RestSharp;
using NewISE.Report.Elaborazioni.Modell;
using RestSharp.Extensions;

namespace NewISE.Controllers
{
    public class ElaborazioniController : Controller
    {
        [Authorize(Roles = "1 ,2")]
        [HttpGet]
        public ActionResult Index()
        {
            bool admin = false;

            try
            {
                admin = Utility.Amministratore();

                ViewBag.Amministratore = admin;
            }
            catch (Exception)
            {

                return View("Error");
            }


            return View();
        }

        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        public JsonResult VerificaFlussiDirettiDaInviare(decimal idAnnoMeseElaborato)
        {
            bool vfd = false;

            try
            {
                using (dtElaborazioni dte = new dtElaborazioni())
                {
                    vfd = dte.VerificaLiquidazioniDiretteDaInviare(idAnnoMeseElaborato);
                }
            }
            catch (Exception ex)
            {
                return Json(new { vfd = false, err = ex.Message });
            }
            return Json(new { vfd = vfd, err = "" });
        }

        [Authorize(Roles = "1 ,2")]
        [HttpPost]
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

        [Authorize(Roles = "1 ,2")]
        [HttpGet]
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

                ViewData["ElencoMesiAnniElaborati"] = rMeseAnno;
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }

        [Authorize(Roles = "1 ,2")]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult DipendentiDaElaborare(decimal idMeseAnnoElaborato)
        {
            List<ElencoDipendentiDaCalcolareModel> ledcm = new List<ElencoDipendentiDaCalcolareModel>();

            try
            {
                using (dtElaborazioni dte = new dtElaborazioni())
                {
                    ledcm = dte.PrelevaDipendentiDaElaborare(idMeseAnnoElaborato).ToList();

                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(ledcm);

        }
        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        public ActionResult CalcolaElaborazioneMensile(List<int> dipendenti, decimal idAnnoMeseElaborato)
        {

            try
            {
                using (dtElaborazioni dte = new dtElaborazioni())
                {
                    if (dipendenti?.Any() ?? false)
                    {
                        dte.Elaborazione(dipendenti, idAnnoMeseElaborato);
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


            return RedirectToAction("DatiLiquidazioneMensile", "Elaborazioni", new { idAnnoMeseElaborato = idAnnoMeseElaborato });
        }


        [Authorize(Roles = "1 ,2")]
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult DatiLiquidazioniDirette(decimal idAnnoMeseElaborato)
        {

            ViewData.Add("idAnnoMeseElaborato", idAnnoMeseElaborato);
            return PartialView();
        }

        public ActionResult DatiLiquidazioniDiretteDaInviare(decimal idAnnoMeseElaborato)
        {
            List<LiquidazioniDiretteViewModel> lLd = new List<LiquidazioniDiretteViewModel>();

            try
            {
                using (dtElaborazioni dte = new dtElaborazioni())
                {
                    lLd = dte.PrelevaLiquidazioniDirette(idAnnoMeseElaborato).Where(a => a.Elaborato == false).ToList();
                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewData["idAnnoMeseElaborato"] = idAnnoMeseElaborato;

            return PartialView(lLd);
        }


        public ActionResult DatiLiquidazioniDiretteInviate(decimal idAnnoMeseElaborato)
        {
            List<LiquidazioniDiretteViewModel> lLd = new List<LiquidazioniDiretteViewModel>();

            try
            {
                using (dtElaborazioni dte = new dtElaborazioni())
                {
                    lLd = dte.PrelevaLiquidazioniDirette(idAnnoMeseElaborato).Where(a => a.Elaborato == true).ToList();
                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewData["idAnnoMeseElaborato"] = idAnnoMeseElaborato;

            return PartialView(lLd);
        }



        [Authorize(Roles = "1 ,2")]
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult DatiLiquidazioneMensile(decimal idAnnoMeseElaborato)
        {
            List<LiquidazioneMensileViewModel> lLm = new List<LiquidazioneMensileViewModel>();
            try
            {
                using (dtElaborazioni dte = new dtElaborazioni())
                {
                    lLm = dte.PrelevaLiquidazioniMensili(idAnnoMeseElaborato).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewData["idAnnoMeseElaborato"] = idAnnoMeseElaborato;

            return PartialView(lLm);
        }

        [Authorize(Roles = "1 ,2")]
        public ActionResult PulsantiInvioElaborazione(decimal idAnnoMeseElaborato)
        {
            ViewData["idAnnoMeseElaborato"] = idAnnoMeseElaborato;

            return PartialView();
        }

        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        public JsonResult VerificaDipendentiCalcolati(decimal idMeseAnnoElab)
        {
            bool ret = false;
            try
            {
                using (dtElaborazioni dte = new dtElaborazioni())
                {
                    ret = dte.VerificaElencoDipElab(idMeseAnnoElab);
                }
            }
            catch (Exception ex)
            {

                return Json(new { Calcolati = ret, err = ex.Message });
            }

            return Json(new { Calcolati = ret, err = "" });

        }


        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        public JsonResult GestionePulsanteCalcola(List<decimal> lDipendenti, decimal idAnnoMeseElab)
        {
            bool gpDaCalcolare = false;
            bool gpMeseChiuso = false;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtElaborazioni dte = new dtElaborazioni())
                    {

                        if (lDipendenti?.Any() ?? false)
                        {
                            gpDaCalcolare = dte.VerificaElaborazioneDipendenti(lDipendenti, idAnnoMeseElab, db);
                        }

                        gpMeseChiuso = dte.VerificaChiusuraPeriodoElab(idAnnoMeseElab, db);
                    }
                }

            }
            catch (Exception ex)
            {
                return Json(new { gpDaCalcolare = gpDaCalcolare, gpMeseChiuso = gpMeseChiuso, err = ex.Message });
            }

            return Json(new { gpDaCalcolare = gpDaCalcolare, gpMeseChiuso = gpMeseChiuso, err = "" });
        }


        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        public JsonResult VerificaChiusuraMeseElab(decimal idAnnoMeseElab)
        {
            bool periodoChiuso = false;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtElaborazioni dte = new dtElaborazioni())
                    {
                        periodoChiuso = dte.VerificaChiusuraPeriodoElab(idAnnoMeseElab, db);
                    }
                }

            }
            catch (Exception ex)
            {
                return Json(new { pc = periodoChiuso, err = ex.Message });
            }

            return Json(new { pc = periodoChiuso, err = "" });
        }

        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        public JsonResult GestionePulsanteInvioFlussiMensili(decimal idAnnoMeseElab)
        {
            bool periodoChiuso = false;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtElaborazioni dte = new dtElaborazioni())
                    {
                        periodoChiuso = dte.VerificaChiusuraPeriodoElab(idAnnoMeseElab, db);
                    }
                }

            }
            catch (Exception ex)
            {
                return Json(new { pc = periodoChiuso, err = ex.Message });
            }

            return Json(new { pc = periodoChiuso, err = "" });
        }



        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        public JsonResult InviaFlussiDirettiOA(decimal idAnnoMeseElaborato, List<decimal> Teorici)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    using (dtElaborazioni dte = new dtElaborazioni())
                    {
                        try
                        {
                            Teorici = Teorici.OrderBy(a => a).ToList();
                            foreach (decimal teorico in Teorici)
                            {
                                dte.InviaFlussiDirettiContabilita(idAnnoMeseElaborato, teorico, db);
                            }

                            db.Database.CurrentTransaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            db.Database.CurrentTransaction.Rollback();
                            throw ex;
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                return Json(new { msg = "", err = ex.Message });
            }

            return Json(new { msg = "I flussi diretti sono stati inviati.", err = "" });

            //return RedirectToAction("DatiLiquidazioniDirette", "Elaborazioni", new { idAnnoMeseElaborato = idAnnoMeseElaborato });
        }

        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        public JsonResult InviaFlussiMensili(decimal idAnnoMeseElaborato)
        {
            List<DIPENDENTI> lDip = new List<DIPENDENTI>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    using (dtElaborazioni dte = new dtElaborazioni())
                    {
                        try
                        {
                            List<decimal> lTeorici = new List<decimal>();


                            #region Lettura dati Prima sistemazione
                            List<decimal> lt1 = (from t in db.TEORICI
                                                 where t.ANNULLATO == false &&
                                                       t.IDMESEANNOELAB == idAnnoMeseElaborato &&
                                                       t.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                                                       (t.VOCI.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380 ||
                                                        t.VOCI.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383 ||
                                                        t.VOCI.IDVOCI == (decimal)EnumVociCedolino.Detrazione_086_384 ||
                                                        t.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS) &&
                                                       t.DIRETTO == false
                                                 select t.IDTEORICI).ToList();

                            if (lt1?.Any() ?? false)
                            {
                                lTeorici.AddRange(lt1);
                            }
                            #endregion

                            #region Lettura dati indennità sede estera
                            List<decimal> lt2 = (from t in db.TEORICI
                                                 where t.ANNULLATO == false && t.INSERIMENTOMANUALE == false &&
                                                       t.IDMESEANNOELAB == idAnnoMeseElaborato &&
                                                       t.ELABINDENNITA.Any(b => b.ANNULLATO == false) &&
                                                       t.DIRETTO == false &&
                                                       t.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                                       t.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera
                                                 select t.IDTEORICI).ToList();


                            if (lt2?.Any() ?? false)
                            {
                                lTeorici.AddRange(lt2);
                            }
                            #endregion


                            #region Lettura dei dati trasporto effetti
                            List<decimal> lt3 = (from t in db.TEORICI
                                                 where t.ANNULLATO == false && t.INSERIMENTOMANUALE == false &&
                                                       t.IDMESEANNOELAB == idAnnoMeseElaborato &&
                                                       t.ELABTRASPEFFETTI.ANNULLATO == false &&
                                                       t.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
                                                       t.VOCI.IDTIPOVOCE == (decimal)EnumTipoVoce.Software &&
                                                       t.VOCI.IDVOCI == (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131 &&
                                                       t.DIRETTO == false
                                                 select t.IDTEORICI).ToList();


                            if (lt3?.Any() ?? false)
                            {
                                lTeorici.AddRange(lt3);
                            }
                            #endregion

                            #region Lettura dei dati MAB
                            List<decimal> lt4 = (from a in db.TEORICI
                                                 where a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                                                       a.IDMESEANNOELAB == idAnnoMeseElaborato &&
                                                       a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                                       a.VOCI.IDVOCI == (decimal)EnumVociContabili.MAB &&
                                                       a.DIRETTO == false &&
                                                       a.ELABMAB.Any(b => b.ANNULLATO == false)
                                                 select a.IDTEORICI).ToList();

                            if (lt4?.Any() ?? false)
                            {
                                lTeorici.AddRange(lt4);
                            }
                            #endregion

                            lTeorici = lTeorici.OrderBy(a => a).ToList();

                            List<ElencoDipendentiDaCalcolareModel> ledcm = new List<ElencoDipendentiDaCalcolareModel>();

                            ledcm = dte.PrelevaDipendentiDaElaborare(idAnnoMeseElaborato).ToList();
                            if (ledcm?.Any() ?? false)
                            {
                                lDip = dte.EstrapolaDipendentiDaTeorici(lTeorici, db).ToList();

                                foreach (var edcm in ledcm)
                                {
                                    if (!lDip?.Any(a => a.IDDIPENDENTE == edcm.idDipendente) ?? false)
                                    {
                                        throw new Exception("Prima di inviare i flussi è necessario effettuare l'elaborazione di tutti i dipendenti interessati al calcolo mensile.");
                                    }

                                }

                                foreach (decimal teorico in lTeorici)
                                {
                                    var dip = dte.EstrapolaDipendenteDaTeorico(teorico, db);


                                    if (!dip.ELABORAZIONI?.Any(a => a.IDMESEANNOELAB == idAnnoMeseElaborato) ?? false)
                                    {
                                        dte.InviaFlussiMensili(idAnnoMeseElaborato, teorico, db);
                                    }
                                }



                                foreach (var dip in lDip)
                                {
                                    dte.SetPeriodoElaborazioniDipendente(dip.IDDIPENDENTE, idAnnoMeseElaborato, db);
                                }

                                dte.ChiudiPeridoElaborazione(idAnnoMeseElaborato, db);

                            }




                            db.Database.CurrentTransaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            db.Database.CurrentTransaction.Rollback();
                            throw ex;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = "", err = ex.Message });
            }

            return Json(new { msg = "I flussi mensili sono stati inviati.", err = "" });
        }

        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public JsonResult VerificaPresenzaDatiLiquidazioneDiretta(decimal idAnnoMeseElaborato, bool Elab = false)
        {
            List<LiquidazioniDiretteViewModel> lLd = new List<LiquidazioniDiretteViewModel>();

            bool verDati = false;

            try
            {
                using (dtElaborazioni dte = new dtElaborazioni())
                {
                    lLd = dte.PrelevaLiquidazioniDirette(idAnnoMeseElaborato).Where(a => a.Elaborato == Elab).ToList();

                    if (lLd?.Any() ?? false)
                    {
                        verDati = true;
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { verDati = verDati, err = ex.Message });
            }

            return Json(new { verDati = verDati, err = "" });
        }


        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public ActionResult ReportLiquidazioniDirette(decimal idAnnoMeseElaborato, bool Elab = false)
        {
            List<LiquidazioniDiretteViewModel> lLd = new List<LiquidazioniDiretteViewModel>();
            string Elaborato = "Liquidazioni dirette da elaborare";

            try
            {
                if (Elab)
                {
                    Elaborato = "Liquidazioni dirette elaborate";
                }


                using (dtElaborazioni dte = new dtElaborazioni())
                {
                    lLd = dte.PrelevaLiquidazioniDirette(idAnnoMeseElaborato).Where(a => a.Elaborato == Elab).ToList();
                }

                if (lLd?.Any() ?? false)
                {
                    using (ModelDBISE db = new ModelDBISE())
                    {
                        var annoMeseElab = db.MESEANNOELABORAZIONE.Find(idAnnoMeseElaborato);

                        ReportViewer reportViewer = new ReportViewer();

                        reportViewer.ProcessingMode = ProcessingMode.Local;
                        reportViewer.SizeToReportContent = true;
                        reportViewer.Width = Unit.Percentage(100);
                        reportViewer.Height = Unit.Percentage(100);

                        reportViewer.Visible = true;


                        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/Elaborazioni/rptLiquidazioniDirette.rdlc";
                        reportViewer.LocalReport.DataSources.Clear();
                        reportViewer.LocalReport.Refresh();

                        ReportParameter[] parameterValues = new ReportParameter[]
                        {
                            new ReportParameter("rptParMeseAnnoElab", ("Mese/Anno elaborazione: " + Utility.MeseAnnoTesto((int) annoMeseElab.MESE, (int) annoMeseElab.ANNO)).ToString()),
                            new ReportParameter("rptParElaborato", Elaborato),
                        };

                        List<LiquidazioniDiretteRpt> lldrpt = new List<LiquidazioniDiretteRpt>();

                        foreach (var ld in lLd)
                        {
                            LiquidazioniDiretteRpt ldrpt = new LiquidazioniDiretteRpt()
                            {
                                IdTeorico = ld.idTeorici,
                                Nominativo = ld.Nominativo,
                                DescrizioneVoce = ld.Voci.descrizione,
                                CodiceVoce = ld.Voci.codiceVoce,
                                Importo = ld.Importo,
                                Data = ld.Data,
                            };

                            lldrpt.Add(ldrpt);
                        }

                        reportViewer.LocalReport.SetParameters(parameterValues);

                        ReportDataSource _rsource = new ReportDataSource("DataSet1", lldrpt);

                        reportViewer.LocalReport.DataSources.Add(_rsource);

                        reportViewer.LocalReport.Refresh();

                        ViewBag.ReportViewer = reportViewer;

                    }
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }


        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public ActionResult ReportLiquidazioniMensili(decimal idAnnoMeseElaborato)
        {
            //ViewData["annoMeseElab"] = annoMeseElab;


            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var annoMeseElab = db.MESEANNOELABORAZIONE.Find(idAnnoMeseElaborato);


                    ReportViewer reportViewer = new ReportViewer();

                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    reportViewer.SizeToReportContent = true;
                    reportViewer.Width = Unit.Percentage(100);
                    reportViewer.Height = Unit.Percentage(100);

                    reportViewer.Visible = true;


                    reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/Elaborazioni/rptLiquidazioniMensili.rdlc";
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.Refresh();

                    ReportParameter[] parameterValues = new ReportParameter[]
                    {
                    new ReportParameter("parAnnoMeseElab",
                        ("Mese/Anno elaborazione: " +
                         Utility.MeseAnnoTesto((int) annoMeseElab.MESE, (int) annoMeseElab.ANNO)).ToString())

                    };

                    List<LiquidazioneMensileViewModel> lLm = new List<LiquidazioneMensileViewModel>();
                    List<LiquidazioneMensileElabRpt> lrptds = new List<LiquidazioneMensileElabRpt>();

                    using (dtElaborazioni dte = new dtElaborazioni())
                    {
                        lLm = dte.PrelevaLiquidazioniMensili(idAnnoMeseElaborato).ToList();
                    }

                    if (lLm?.Any() ?? false)
                    {
                        foreach (var lm in lLm)
                        {
                            LiquidazioneMensileElabRpt rptds = new LiquidazioneMensileElabRpt()
                            {
                                IdTeorici = lm.idTeorici,
                                DescrizioneVoce = lm.Voci.descrizione,
                                Nominativo = lm.Nominativo,
                                DataRiferimento = Utility.MeseAnnoTesto((int)lm.meseRiferimento, (int)lm.annoRiferimento),
                                Movimento = lm.TipoMovimento.DescMovimento,
                                Liquidazione = lm.Voci.TipoLiquidazione.descrizione,
                                Voce = lm.Voci.codiceVoce,
                                Inserimento = lm.tipoInserimento.ToString(),
                                Importo = lm.Importo,
                                Inviato = lm.Elaborato
                            };

                            lrptds.Add(rptds);
                        }
                    }
                    reportViewer.LocalReport.SetParameters(parameterValues);

                    ReportDataSource _rsource = new ReportDataSource("DataSet1", lrptds);

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
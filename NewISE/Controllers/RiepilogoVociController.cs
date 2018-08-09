using Microsoft.Reporting.WebForms;
using NewISE.EF;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.dtObj;
using NewISE.Models.Enumeratori;
using NewISE.Models.Tools;
using NewISE.Views.Dataset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace NewISE.Controllers
{
    public class RiepilogoVociController : Controller
    {
        // GET: RiepilogoVoci
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GestioneRiepilogoVoci(decimal idTrasferimento)
        {
            try
            {
                TrasferimentoModel tm = new TrasferimentoModel();



                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    tm = dtt.GetTrasferimentoById(idTrasferimento);


                }
                ViewBag.idTrasferimento = idTrasferimento;

                return PartialView("GestioneRiepilogoVoci", tm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        public JsonResult VerificaRiepilogoVoci(decimal idTrasferimento)
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

                        return Json(new { VerificaRiepilogoVoci = 1 });
                    }
                    else
                    {
                        return Json(new { VerificaRiepilogoVoci = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }
        public ActionResult AttivitaRiepilogoVoci(decimal idTrasferimento)
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
        public ActionResult ElencoRiepilogoVoci(decimal idTrasferimento)
        {
            List<RiepiloVociModel> lrvm = new List<RiepiloVociModel>();

            try
            {
                using (dtRiepilogoVoci dtrv = new dtRiepilogoVoci())
                {
                    lrvm = dtrv.GetRiepilogoVoci(idTrasferimento).ToList();
                }

                ViewData["idTrasferimento"] = idTrasferimento;
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(lrvm);
        }

        DSRiepilogoVoci DSRiepilogoVoci = new DSRiepilogoVoci();
        public ActionResult RptRiepilogoVoci(decimal idTrasferimento)
        {
            List<RiepiloVociModel> lrvm = new List<RiepiloVociModel>();
            RiepiloVociModel lrvm1 = new RiepiloVociModel();


            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var tm = dtt.GetTrasferimentoById(idTrasferimento);

                        using (dtLivelliDipendente dld = new dtLivelliDipendente())
                        {
                            ViewBag.idTrasferimento = idTrasferimento;

                            var liv = dld.GetLivelloDipendenteByIdTrasferimento(idTrasferimento);
                            var liv1 = liv.First();

                            string Nominativo = tm.Dipendente.Nominativo;
                            string Decorrenza = Convert.ToDateTime(tm.dataPartenza).ToShortDateString();
                            string Livello = liv1.Livello.DescLivello;
                            string Ufficio = tm.Ufficio.descUfficio;

                            var t = db.TRASFERIMENTO.Find(idTrasferimento);

                            var ps = t.PRIMASITEMAZIONE;
                            var ind = t.INDENNITA;
                            var mab = ind.MAGGIORAZIONEABITAZIONE;
                            var tep = t.TEPARTENZA;
                            var ter = t.TERIENTRO;

                            //var lTeorici =
                            //db.TEORICI.ToList();

                            var lTeorici =
                            db.TEORICI.Where(
                                a =>
                                    a.ANNULLATO == false && a.ELABORATO == true &&
                                    (a.ELABINDSISTEMAZIONE.IDPRIMASISTEMAZIONE == ps.IDPRIMASISTEMAZIONE) ||
                                    a.ELABINDENNITA.Any(b => b.IDTRASFINDENNITA == ind.IDTRASFINDENNITA) ||
                                    a.ELABMAB.IDMAGABITAZIONE == mab.IDMAGABITAZIONE ||
                                    a.ELABTRASPEFFETTI.IDTEPARTENZA.Value == tep.IDTEPARTENZA ||
                                    a.ELABTRASPEFFETTI.IDTERIENTRO.Value == ter.IDTERIENTRO)
                                .OrderBy(a => a.ANNORIFERIMENTO)
                                .ThenBy(a => a.MESERIFERIMENTO)
                                .ToList();


                            // Indennità Personale
                            //var lTeorici = db.TEORICI.Where(a => a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                            //                         a.IDMESEANNOELAB == mae.IDMESEANNOELAB &&
                            //                         a.ELABINDENNITA.ANNULLATO == false &&
                            //                         a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                            //                         a.VOCI.IDTIPOVOCE == (decimal)EnumTipoVoce.Software &&
                            //                         a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera)
                            //.OrderBy(a => a.ELABINDENNITA.INDENNITA.TRASFERIMENTO.DIPENDENTI.COGNOME)
                            //.ThenBy(a => a.ELABINDENNITA.INDENNITA.TRASFERIMENTO.DIPENDENTI.NOME)
                            //.ThenBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                            //.ToList();


                            // Prima Sistemazione
                            //var lTeorici =
                            //    db.TEORICI.Where(
                            //        a =>
                            //            a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                            //            a.IDMESEANNOELAB == mae.IDMESEANNOELAB && a.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                            //            (a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380 ||
                            //             a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383 ||
                            //             a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Detrazione_086_384 ||
                            //             a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS))
                            //        .OrderBy(a => a.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO.DIPENDENTI.COGNOME)
                            //        .ThenBy(a => a.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO.DIPENDENTI.NOME)
                            //        .ThenBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                            //        .ToList();

                            // Trasporto Effetti
                            //var lTeorici =
                            //    db.TEORICI.Where(
                            //        a =>
                            //            a.ANNULLATO == false && a.INSERIMENTOMANUALE == false && a.IDMESEANNOELAB == mae.IDMESEANNOELAB &&
                            //            a.ELABTRASPEFFETTI.ANNULLATO == false && a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
                            //            a.VOCI.IDTIPOVOCE == (decimal)EnumTipoVoce.Software &&
                            //            a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131)
                            //        .OrderBy(a => a.ELABTRASPEFFETTI.TEPARTENZA.TRASFERIMENTO.DIPENDENTI.COGNOME)
                            //        .ThenBy(a => a.ELABTRASPEFFETTI.TEPARTENZA.TRASFERIMENTO.DIPENDENTI.NOME)
                            //        .ThenBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                            //        .ToList();

                            ViewBag.idTrasferimento = idTrasferimento;

                            //string Nominativo = tm.Dipendente.Nominativo;
                            //string Decorrenza = Convert.ToDateTime(tm.dataPartenza).ToShortDateString();
                            //string Ufficio = tm.Ufficio.descUfficio;


                            if (lTeorici?.Any() ?? false)
                            {

                                foreach (var teorico in lTeorici)
                                {
                                    //var tr =
                                    //    teorico.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO;

                                    //var dip = tr.DIPENDENTI;
                                    var tm1 = teorico.TIPOMOVIMENTO;
                                    var voce = teorico.VOCI;
                                    var tl = teorico.VOCI.TIPOLIQUIDAZIONE;
                                    var tv = teorico.VOCI.TIPOVOCE;
                                    //var uf = tr.UFFICI;
                                    

                                    lrvm = (from e in lTeorici
                                            select new RiepiloVociModel()
                                            {
                                                dataOperazione = teorico.DATAOPERAZIONE,
                                                importo = teorico.IMPORTO,
                                                descrizione = teorico.VOCI.DESCRIZIONE,
                                                TipoMovimento = new TipoMovimentoModel()
                                                {
                                                    idTipoMovimento = tm1.IDTIPOMOVIMENTO,
                                                    TipoMovimento = tm1.TIPOMOVIMENTO1,
                                                    DescMovimento = tm1.DESCMOVIMENTO
                                                },
                                                TipoLiquidazione = new TipoLiquidazioneModel()
                                                {
                                                    idTipoLiquidazione = tl.IDTIPOLIQUIDAZIONE,
                                                    descrizione = tl.DESCRIZIONE
                                                },
                                                TipoVoce = new TipoVoceModel()
                                                {
                                                    idTipoVoce = tv.IDTIPOVOCE,
                                                    descrizione = tv.DESCRIZIONE
                                                },
                                                Voci = new VociModel()
                                                {
                                                    idVoci = voce.IDVOCI,
                                                    codiceVoce = voce.CODICEVOCE,
                                                    descrizione = voce.DESCRIZIONE,
                                                    TipoLiquidazione = new TipoLiquidazioneModel()
                                                    {
                                                        idTipoLiquidazione = tl.IDTIPOLIQUIDAZIONE,
                                                        descrizione = tl.DESCRIZIONE
                                                    },
                                                    TipoVoce = new TipoVoceModel()
                                                    {
                                                        idTipoVoce = tv.IDTIPOVOCE,
                                                        descrizione = tv.DESCRIZIONE
                                                    }
                                                },
                                                meseRiferimento = teorico.MESERIFERIMENTO,
                                                annoRiferimento = teorico.ANNORIFERIMENTO,
                                                giorni = 0,
                                                Importo = teorico.IMPORTO,
                                                Elaborato = teorico.ELABORATO
                                            }).ToList();


                                    //DateTime dataOperazione = teorico.DATAOPERAZIONE;
                                    string descrizione = teorico.VOCI.DESCRIZIONE;
                                    string descrTipoVoce = tl.DESCRIZIONE;

                                    ReportViewer reportViewer = new ReportViewer();

                                    reportViewer.ProcessingMode = ProcessingMode.Local;
                                    reportViewer.SizeToReportContent = true;
                                    reportViewer.Width = Unit.Percentage(100);
                                    reportViewer.Height = Unit.Percentage(100);

                                    var datasource = new ReportDataSource("DSRiepilogoVoci", lTeorici.ToList());
                                    reportViewer.Visible = true;
                                    reportViewer.ProcessingMode = ProcessingMode.Local;
                                    reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptRiepilogoVoci.rdlc";
                                    reportViewer.LocalReport.DataSources.Clear();
                                    reportViewer.LocalReport.DataSources.Add(datasource);

                                    reportViewer.LocalReport.Refresh();

                                    ReportParameter[] parameterValues = new ReportParameter[]
                                    {
                                        new ReportParameter ("Nominativo",Nominativo),
                                        new ReportParameter ("descrizione",descrizione),
                                        new ReportParameter ("descrTipoVoce",descrTipoVoce),
                                        new ReportParameter ("Decorrenza",Decorrenza),
                                        new ReportParameter ("Ufficio",Ufficio),
                                        new ReportParameter ("Livello",Livello)

                                    };

                                    reportViewer.LocalReport.SetParameters(parameterValues);
                                    ViewBag.ReportViewer = reportViewer;
                                }
                            }
                            else
                            {
                                throw new Exception("Nessun Dato Presente");
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("RptRiepilogoVoci");
        }

    }
}
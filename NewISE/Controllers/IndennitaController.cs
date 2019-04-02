
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
using System.Data;
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
                            bool vedi_PS_TE = true;
                            string DataRifIndennita_TE_PS =
                                Convert.ToString(
                                    System.Configuration.ConfigurationManager.AppSettings["DataLimiteIndennita_PS_TE"]);

                            if (tr.dataPartenza < Convert.ToDateTime(DataRifIndennita_TE_PS))
                            {
                                vedi_PS_TE = false;
                            }
                            ViewBag.vedi_PS_TE = vedi_PS_TE;
                            ViewBag.DataRifIndennita = DataRifIndennita_TE_PS;
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

        #region Indennità di Base Estera + Report di Stampa

        public ActionResult IndennitaBase(decimal idTrasferimento)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();
            //List<LivelloDipendenteModel> lldm = new List<LivelloDipendenteModel>();


            try
            {

                using (dtEvoluzioneIndennita dtd = new dtEvoluzioneIndennita())
                {
                    libm = dtd.GetIndennita(idTrasferimento).ToList();
                }


                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tm = dtt.GetTrasferimentoById(idTrasferimento);

                    using (dtRuoloUfficio dtru = new dtRuoloUfficio())
                    {
                        tm.RuoloUfficio = dtru.GetRuoloUfficioValidoByIdTrasferimento(tm.idTrasferimento);
                        tm.idRuoloUfficio = tm.RuoloUfficio.idRuoloUfficio;
                        //tm.RuoloUfficio.DescrizioneRuolo = tm.RuoloUfficio.DescrizioneRuolo;
                        ViewBag.idRuoloUfficio = tm.idRuoloUfficio;
                        ViewBag.RuoloDipendente = tm.RuoloUfficio.DescrizioneRuolo;

                    }

                    //using (dtLivelliDipendente dtld = new dtLivelliDipendente())
                    //{
                    //    lldm = dtld.GetLivelloDipendenteByIdTrasferimento(idTrasferimento).ToList();


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

        public ActionResult RptIndennitaBase(decimal idTrasferimento)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();
            List<RptIndennitaBaseModel> rpt = new List<RptIndennitaBaseModel>();
            List<DateTime> lDateVariazioni = new List<DateTime>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var indennita = trasferimento.INDENNITA;
                    var dipendente = trasferimento.DIPENDENTI;
                    string nominativo = string.Empty;
                    string livello = string.Empty;

                    nominativo = dipendente.COGNOME + " " + dipendente.NOME;

                    string decorrenza = trasferimento.DATAPARTENZA.ToShortDateString();
                    string ufficio = trasferimento.UFFICI.DESCRIZIONEUFFICIO;

                    var lliv =
                        indennita.LIVELLIDIPENDENTI.Where(
                            a =>
                                a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                            .OrderBy(a => a.DATAINIZIOVALIDITA)
                            .ToList();


                    if (lliv?.Any() ?? false)
                    {
                        livello = lliv.Last().LIVELLI.LIVELLO;

                        foreach (var l in lliv)
                        {
                            DateTime dtIni;
                            DateTime dtFin;

                            if (l.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                            {
                                dtIni = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtIni = l.DATAINIZIOVALIDITA;
                            }

                            if (l.DATAFINEVALIDITA > trasferimento.DATARIENTRO)
                            {
                                dtFin = trasferimento.DATARIENTRO;
                            }
                            else
                            {
                                dtFin = l.DATAFINEVALIDITA;
                            }

                            var lib =
                                indennita.INDENNITABASE.Where(
                                    a =>
                                        a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= dtFin &&
                                        a.DATAFINEVALIDITA >= dtIni &&
                                        a.IDLIVELLO == l.IDLIVELLO).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                            foreach (var ib in lib)
                            {
                                DateTime dtVar = new DateTime();

                                if (ib.DATAINIZIOVALIDITA < dtIni)
                                {
                                    dtVar = dtIni;
                                }
                                else
                                {
                                    dtVar = ib.DATAINIZIOVALIDITA;
                                }

                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                }
                            }
                        }
                    }


                    DateTime dataFine;

                    if (trasferimento.DATARIENTRO < Utility.DataFineStop())
                    {
                        dataFine = trasferimento.DATARIENTRO;
                    }
                    else
                    {
                        dataFine = Utility.DataFineStop();
                    }

                    if (!lDateVariazioni.Contains(dataFine))
                    {
                        lDateVariazioni.Add(dataFine);
                    }


                    if (lDateVariazioni?.Any() ?? false)
                    {
                        for (int j = 0; j < lDateVariazioni.Count; j++)
                        {
                            DateTime dv = lDateVariazioni[j];

                            if (dv < dataFine)
                            {
                                DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                if (lDateVariazioni[j + 1] == dataFine)
                                {
                                    dvSucc = lDateVariazioni[j + 1];
                                }

                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db)
                                    )
                                {
                                    RptIndennitaBaseModel rpts = new RptIndennitaBaseModel()
                                    {

                                        IndennitaBase = ci.IndennitaDiBase,
                                        DataInizioValidita = Convert.ToDateTime(dv).ToShortDateString(),
                                        //DataFineValidita = Convert.ToDateTime(dvSucc).ToShortDateString()
                                        DataFineValidita =
                                            (dvSucc < Utility.DataFineStop())
                                                ? Convert.ToDateTime(dvSucc).ToShortDateString()
                                                : null,
                                        DescLivello = ci.Livello.LIVELLO

                                    };

                                    rpt.Add(rpts);
                                }
                            }
                        }
                    }


                    ReportViewer reportViewer = new ReportViewer();

                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    reportViewer.SizeToReportContent = true;
                    reportViewer.Width = Unit.Percentage(100);
                    reportViewer.Height = Unit.Percentage(100);

                    var datasource = new ReportDataSource("DataSetIndennitaBase");

                    reportViewer.Visible = true;
                    reportViewer.ProcessingMode = ProcessingMode.Local;

                    reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) +
                                                          @"/Report/RptIndennitaBase.rdlc";
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(datasource);
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetIndennitaBase", rpt));
                    reportViewer.LocalReport.Refresh();

                    List<ReportParameter> parameterValues = new List<ReportParameter>();
                    parameterValues.Add(new ReportParameter("Nominativo", nominativo));
                    parameterValues.Add(new ReportParameter("Livello", livello));
                    parameterValues.Add(new ReportParameter("Decorrenza", decorrenza));
                    parameterValues.Add(new ReportParameter("Ufficio", ufficio));

                    reportViewer.LocalReport.SetParameters(parameterValues);
                    ViewBag.ReportViewer = reportViewer;

                }

                return PartialView("RptIndennitaBase");


            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

        }
        #endregion

        #region Indennità di Servizio + Report di Stampa
        public ActionResult IndennitaServizio(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {
                using (dtEvoluzioneIndennita dtei = new dtEvoluzioneIndennita())
                {
                    eim = dtei.GetIndennitaEvoluzione(idTrasferimento).ToList();
                }

                ViewBag.idTrasferimento = idTrasferimento;

                return PartialView("IndennitaServizio", eim);

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

        }


        public ActionResult RptIndennitaServizio(decimal idTrasferimento)
        {

            List<RptIndennitaServizioModel> rpt = new List<RptIndennitaServizioModel>();


            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var dipendente = trasferimento.DIPENDENTI;
                    var indennita = trasferimento.INDENNITA;

                    using (dtLivelliDipendente dld = new dtLivelliDipendente())
                    {
                        ViewBag.idTrasferimento = idTrasferimento;

                        var liv = dld.GetLivelloDipendenteByIdTrasferimento(idTrasferimento);
                        var liv1 = liv.OrderBy(a => a.dataInizioValdita).Last();

                        string Nominativo = dipendente.COGNOME + " " + dipendente.NOME;
                        string Decorrenza = Convert.ToDateTime(trasferimento.DATAPARTENZA).ToShortDateString();
                        string Livello = liv1.Livello.DescLivello;
                        string Ufficio = trasferimento.UFFICI.DESCRIZIONEUFFICIO;

                        List<DateTime> lDateVariazioni = new List<DateTime>();

                        var lliv =
                            indennita.LIVELLIDIPENDENTI
                                .Where(a => a.ANNULLATO == false &&
                                            a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                            a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                .OrderBy(a => a.DATAINIZIOVALIDITA)
                                .ToList();

                        foreach (var l in lliv)
                        {

                            DateTime dtIni;
                            DateTime dtFin;

                            if (l.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                            {
                                dtIni = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtIni = l.DATAINIZIOVALIDITA;
                            }

                            if (l.DATAFINEVALIDITA > trasferimento.DATARIENTRO)
                            {
                                dtFin = trasferimento.DATARIENTRO;
                            }
                            else
                            {
                                dtFin = l.DATAFINEVALIDITA;
                            }


                            #region Variazioni di indennità di base

                            var ll =
                                indennita.INDENNITABASE
                                    .Where(
                                        a =>
                                            a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= dtFin &&
                                            a.DATAFINEVALIDITA >= dtIni)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                                    .ToList();


                            foreach (var ib in ll)
                            {
                                DateTime dtVar = new DateTime();

                                if (ib.DATAINIZIOVALIDITA < dtIni)
                                {
                                    dtVar = dtIni;
                                }
                                else
                                {
                                    dtVar = ib.DATAINIZIOVALIDITA;
                                }

                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                }
                            }

                            #endregion

                            #region Variazioni del coefficiente di sede

                            var lrd =
                                indennita.COEFFICIENTESEDE
                                    .Where(
                                        a =>
                                            a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= dtFin &&
                                            a.DATAFINEVALIDITA >= dtIni)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                                    .ToList();

                            foreach (var cs in lrd)
                            {
                                DateTime dtVar = new DateTime();

                                if (cs.DATAINIZIOVALIDITA < dtIni)
                                {
                                    dtVar = dtIni;
                                }
                                else
                                {
                                    dtVar = cs.DATAINIZIOVALIDITA;
                                }

                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                }
                            }

                            #endregion

                            #region Variazioni percentuale di disagio

                            var perc =
                                indennita.PERCENTUALEDISAGIO
                                    .Where(
                                        a =>
                                            a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= dtFin &&
                                            a.DATAFINEVALIDITA >= dtIni)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                                    .ToList();

                            foreach (var pd in perc)
                            {
                                DateTime dtVar = new DateTime();

                                if (pd.DATAINIZIOVALIDITA < dtIni)
                                {
                                    dtVar = dtIni;
                                }
                                else
                                {
                                    dtVar = pd.DATAINIZIOVALIDITA;
                                }

                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                }
                            }

                            #endregion
                        }


                        DateTime dataFine;

                        if (trasferimento.DATARIENTRO < Utility.DataFineStop())
                        {
                            dataFine = trasferimento.DATARIENTRO;
                        }
                        else
                        {
                            dataFine = Utility.DataFineStop();
                        }

                        if (!lDateVariazioni.Contains(dataFine))
                        {
                            lDateVariazioni.Add(dataFine);
                        }

                        if (lDateVariazioni?.Any() ?? false)
                        {
                            for (int j = 0; j < lDateVariazioni.Count; j++)
                            {
                                DateTime dv = lDateVariazioni[j];

                                if (dv < dataFine)
                                {
                                    DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);
                                    //DateTime dvSucc = lDateVariazioni[(j + 1)];

                                    if (lDateVariazioni[j + 1] == dataFine)
                                    {
                                        dvSucc = lDateVariazioni[j + 1];
                                    }

                                    using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                    {

                                        RptIndennitaServizioModel rpts = new RptIndennitaServizioModel()
                                        {

                                            DataInizioValidita = Convert.ToDateTime(dv).ToShortDateString(),
                                            //DataFineValidita = Convert.ToDateTime(dvSucc).ToShortDateString(),
                                            DataFineValidita = (dvSucc < Utility.DataFineStop()) ? Convert.ToDateTime(dvSucc).ToShortDateString() : null,
                                            IndennitaBase = ci.IndennitaDiBase,
                                            CoefficenteSede = ci.CoefficienteDiSede,
                                            PercentualeDisagio = ci.PercentualeDisagio,
                                            IndennitaServizio = ci.IndennitaDiServizio

                                        };

                                        rpt.Add(rpts);

                                    }



                                    ReportViewer reportViewer = new ReportViewer();

                                    reportViewer.ProcessingMode = ProcessingMode.Local;
                                    reportViewer.SizeToReportContent = true;
                                    reportViewer.Width = Unit.Percentage(100);
                                    reportViewer.Height = Unit.Percentage(100);

                                    var datasource = new ReportDataSource("DataSetIndennitaServizio");

                                    reportViewer.Visible = true;
                                    reportViewer.ProcessingMode = ProcessingMode.Local;

                                    reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptIndennitaServizio.rdlc";
                                    reportViewer.LocalReport.DataSources.Clear();
                                    reportViewer.LocalReport.DataSources.Add(datasource);
                                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetIndennitaServizio", rpt));
                                    reportViewer.LocalReport.Refresh();

                                    List<ReportParameter> parameterValues = new List<ReportParameter>();
                                    parameterValues.Add(new ReportParameter("Nominativo", Nominativo));
                                    parameterValues.Add(new ReportParameter("Livello", Livello));
                                    parameterValues.Add(new ReportParameter("Decorrenza", Decorrenza));
                                    parameterValues.Add(new ReportParameter("Ufficio", Ufficio));

                                    reportViewer.LocalReport.SetParameters(parameterValues);
                                    ViewBag.ReportViewer = reportViewer;


                                }
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("RptIndennitaServizio");
        }
        #endregion

        #region Maggiorazioni Coniuge (Maggiorazioni Coniuge)
        public ActionResult MaggiorazioniConiuge(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {
                using (dtEvoluzioneIndennita dtei = new dtEvoluzioneIndennita())
                {
                    eim = dtei.GetMaggiorazioniConiugeEvoluzione(idTrasferimento).ToList();

                }

                ViewBag.idTrasferimento = idTrasferimento;

                return PartialView(eim);
            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }
        public ActionResult RptMaggiorazioniConiuge(decimal idTrasferimento)
        {
            List<RptMaggiorazioniConiuge> rpt = new List<RptMaggiorazioniConiuge>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                        var dipendente = trasferimento.DIPENDENTI;

                        using (dtLivelliDipendente dld = new dtLivelliDipendente())
                        {
                            ViewBag.idTrasferimento = idTrasferimento;

                            var liv = dld.GetLivelloDipendenteByIdTrasferimento(idTrasferimento);
                            var liv1 = liv.OrderBy(a => a.dataInizioValdita).Last();

                            string Nominativo = dipendente.COGNOME + " " + dipendente.NOME;
                            string Decorrenza = Convert.ToDateTime(trasferimento.DATAPARTENZA).ToShortDateString();
                            string Livello = liv1.Livello.DescLivello;
                            string Ufficio = trasferimento.UFFICI.DESCRIZIONEUFFICIO;

                            List<DateTime> lDateVariazioni = new List<DateTime>();

                            var mf = trasferimento.MAGGIORAZIONIFAMILIARI;


                            #region Variazioni Coniuge

                            var lc =
                                mf.CONIUGE.Where(
                                    a =>
                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                        a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                        a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                            if (lc?.Any() ?? false)
                            {
                                foreach (var c in lc)
                                {

                                    DateTime dtVar = new DateTime();

                                    if (c.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                    {
                                        dtVar = trasferimento.DATAPARTENZA;
                                    }
                                    else
                                    {
                                        dtVar = c.DATAINIZIOVALIDITA;
                                    }

                                    if (!lDateVariazioni.Contains(dtVar))
                                    {
                                        lDateVariazioni.Add(dtVar);
                                    }

                                    #region Variazione Percentuale Maggiorazione Coniuge

                                    var lpmc =
                                        c.PERCENTUALEMAGCONIUGE.Where(
                                            a =>
                                                a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= c.DATAFINEVALIDITA &&
                                                a.DATAFINEVALIDITA >= c.DATAINIZIOVALIDITA)
                                            .OrderBy(a => a.DATAINIZIOVALIDITA)
                                            .ToList();
                                    if (lpmc?.Any() ?? false)
                                    {
                                        foreach (var pmc in lpmc)
                                        {
                                            if (pmc.DATAINIZIOVALIDITA < c.DATAINIZIOVALIDITA)
                                            {
                                                dtVar = c.DATAINIZIOVALIDITA;
                                            }
                                            else
                                            {
                                                dtVar = pmc.DATAINIZIOVALIDITA;
                                            }

                                            if (!lDateVariazioni.Contains(dtVar))
                                            {
                                                lDateVariazioni.Add(dtVar);
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Variazioni Pensioni

                                    var lp = c.PENSIONE.Where(a =>
                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                        a.DATAINIZIO <= c.DATAFINEVALIDITA && a.DATAFINE >= c.DATAINIZIOVALIDITA)
                                        .OrderBy(a => a.DATAINIZIO)
                                        .ToList();

                                    if (lp?.Any() ?? false)
                                    {
                                        foreach (var p in lp)
                                        {
                                            if (p.DATAINIZIO < c.DATAINIZIOVALIDITA)
                                            {
                                                dtVar = c.DATAINIZIOVALIDITA;
                                            }
                                            else
                                            {
                                                dtVar = p.DATAINIZIO;
                                            }

                                            if (!lDateVariazioni.Contains(dtVar))
                                            {
                                                lDateVariazioni.Add(dtVar);
                                            }
                                        }
                                    }

                                    #endregion
                                }

                                DateTime dataFine;
                                DateTime dataFinConiuge = lc.Last().DATAFINEVALIDITA;

                                if (trasferimento.DATARIENTRO < Utility.DataFineStop())
                                {
                                    if (dataFinConiuge < trasferimento.DATARIENTRO)
                                    {
                                        dataFine = dataFinConiuge;
                                    }
                                    else
                                    {
                                        dataFine = trasferimento.DATARIENTRO;
                                    }
                                }
                                else
                                {
                                    if (dataFinConiuge < Utility.DataFineStop())
                                    {
                                        dataFine = dataFinConiuge;
                                    }
                                    else
                                    {
                                        dataFine = Utility.DataFineStop();
                                    }
                                }

                                if (!lDateVariazioni.Contains(dataFine))
                                {
                                    lDateVariazioni.Add(dataFine);
                                }

                                lDateVariazioni = lDateVariazioni.OrderBy(a => a.Year).ThenBy(a => a.Month).ThenBy(a => a.Day).ToList();

                                if (lDateVariazioni?.Any() ?? false)
                                {
                                    for (int j = 0; j < lDateVariazioni.Count; j++)
                                    {
                                        DateTime dv = lDateVariazioni[j];

                                        if (dv < dataFine)
                                        {
                                            DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                            if (lDateVariazioni[j + 1] == dataFine)
                                            {
                                                dvSucc = lDateVariazioni[j + 1];
                                            }

                                            using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                            {
                                                RptMaggiorazioniConiuge rpts = new RptMaggiorazioniConiuge()
                                                {
                                                    DataInizioValidita = Convert.ToDateTime(dv).ToShortDateString(),
                                                    DataFineValidita = (dvSucc < Utility.DataFineStop()) ? Convert.ToDateTime(dvSucc).ToShortDateString() : null,
                                                    IndennitaServizio = ci.IndennitaDiServizio,
                                                    PercentualeMaggiorazioniConiuge = ci.PercentualeMaggiorazioneConiuge,
                                                    MaggiorazioniConiuge = ci.MaggiorazioneConiuge
                                                };

                                                rpt.Add(rpts);
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion



                            ReportViewer reportViewer = new ReportViewer();

                            reportViewer.ProcessingMode = ProcessingMode.Local;
                            reportViewer.SizeToReportContent = true;
                            reportViewer.Width = Unit.Percentage(100);
                            reportViewer.Height = Unit.Percentage(100);

                            var datasource = new ReportDataSource("DataSetMaggiorazioniConiuge");

                            reportViewer.Visible = true;
                            reportViewer.ProcessingMode = ProcessingMode.Local;

                            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptMaggiorazioniConiuge.rdlc";
                            reportViewer.LocalReport.DataSources.Clear();
                            reportViewer.LocalReport.DataSources.Add(datasource);
                            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetMaggiorazioniConiuge", rpt));
                            reportViewer.LocalReport.Refresh();

                            List<ReportParameter> parameterValues = new List<ReportParameter>();
                            parameterValues.Add(new ReportParameter("Nominativo", Nominativo));
                            parameterValues.Add(new ReportParameter("Livello", Livello));
                            parameterValues.Add(new ReportParameter("Decorrenza", Decorrenza));
                            parameterValues.Add(new ReportParameter("Ufficio", Ufficio));


                            reportViewer.LocalReport.SetParameters(parameterValues);
                            ViewBag.ReportViewer = reportViewer;

                        }
                    }
                }


            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("RptContributoOmnicomprensivoRientro");
        }

        #endregion

        #region Maggiorazioni Figli (Maggiorazioni Figli) 
        public ActionResult MaggiorazioniFigli(decimal idTrasferimento)
        {

            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {

                using (dtEvoluzioneIndennita dtei = new dtEvoluzioneIndennita())
                {
                    eim = dtei.GetMaggiorazioniFigliEvoluzione(idTrasferimento).ToList();

                }


                ViewBag.idTrasferimento = idTrasferimento;

                return PartialView(eim);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        public ActionResult RptMaggiorazioniFigli(decimal idTrasferimento)
        {
            List<RptMaggiorazioniFigli> rpt = new List<RptMaggiorazioniFigli>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var dipendente = trasferimento.DIPENDENTI;

                    using (dtLivelliDipendente dld = new dtLivelliDipendente())
                    {
                        ViewBag.idTrasferimento = idTrasferimento;

                        var liv = dld.GetLivelloDipendenteByIdTrasferimento(idTrasferimento);
                        var liv1 = liv.OrderBy(a => a.dataInizioValdita).Last();

                        string Nominativo = dipendente.COGNOME + " " + dipendente.NOME;
                        string Decorrenza = Convert.ToDateTime(trasferimento.DATAPARTENZA).ToShortDateString();
                        string Livello = liv1.Livello.DescLivello;
                        string Ufficio = trasferimento.UFFICI.DESCRIZIONEUFFICIO;

                        List<DateTime> lDateVariazioni = new List<DateTime>();
                        var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                        #region Variazioni Maggiorazioni Figli

                        var lf =
                            mf.FIGLI.Where(
                                a =>
                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                    a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                    a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        if (lf?.Any() ?? false)
                        {
                            foreach (var f in lf)
                            {

                                DateTime dtVar = new DateTime();

                                if (f.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                {
                                    dtVar = trasferimento.DATAPARTENZA;
                                }
                                else
                                {
                                    dtVar = f.DATAINIZIOVALIDITA;
                                }

                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                }

                                #region Variazioni Percentuale Maggiorazioni Figli

                                var lpmf =
                                    f.PERCENTUALEMAGFIGLI.Where(
                                        a =>
                                            a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= f.DATAFINEVALIDITA &&
                                            a.DATAFINEVALIDITA >= f.DATAINIZIOVALIDITA)
                                        .OrderBy(a => a.DATAINIZIOVALIDITA)
                                        .ToList();

                                if (lpmf?.Any() ?? false)
                                {
                                    foreach (var pmf in lpmf)
                                    {
                                        if (pmf.DATAINIZIOVALIDITA < f.DATAINIZIOVALIDITA)
                                        {
                                            dtVar = f.DATAINIZIOVALIDITA;
                                        }
                                        else
                                        {
                                            dtVar = pmf.DATAINIZIOVALIDITA;
                                        }

                                        if (!lDateVariazioni.Contains(dtVar))
                                        {
                                            lDateVariazioni.Add(dtVar);
                                        }
                                    }

                                }

                                #endregion

                                #region Variazione Indennità Primo Segretario
                                var lips =
                                            f.INDENNITAPRIMOSEGRETARIO.Where(
                                                a =>
                                                    a.ANNULLATO == false &&
                                                    a.DATAINIZIOVALIDITA <= f.DATAFINEVALIDITA &&
                                                    a.DATAFINEVALIDITA >= f.DATAINIZIOVALIDITA)
                                                .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                .ToList();

                                if (lips?.Any() ?? false)
                                {
                                    foreach (var ips in lips)
                                    {
                                        if (ips.DATAINIZIOVALIDITA < f.DATAINIZIOVALIDITA)
                                        {
                                            dtVar = f.DATAINIZIOVALIDITA;
                                        }
                                        else
                                        {
                                            dtVar = ips.DATAINIZIOVALIDITA;
                                        }

                                        if (!lDateVariazioni.Contains(dtVar))
                                        {
                                            lDateVariazioni.Add(dtVar);
                                        }
                                    }


                                }
                                #endregion


                            }

                            DateTime dataFine;
                            DateTime dtFinFigli = lf.Last().DATAFINEVALIDITA;


                            if (trasferimento.DATARIENTRO < Utility.DataFineStop())
                            {
                                if (dtFinFigli < trasferimento.DATARIENTRO)
                                {
                                    dataFine = dtFinFigli;
                                }
                                else
                                {
                                    dataFine = trasferimento.DATARIENTRO;
                                }

                            }
                            else
                            {
                                if (dtFinFigli < Utility.DataFineStop())
                                {
                                    dataFine = dtFinFigli;
                                }
                                else
                                {
                                    dataFine = Utility.DataFineStop();
                                }
                            }

                            if (!lDateVariazioni.Contains(dataFine))
                            {
                                lDateVariazioni.Add(dataFine);
                            }

                            lDateVariazioni = lDateVariazioni.OrderBy(a => a.Year).ThenBy(a => a.Month).ThenBy(a => a.Day).ToList();

                            if (lDateVariazioni?.Any() ?? false)
                            {
                                for (int j = 0; j < lDateVariazioni.Count; j++)
                                {
                                    DateTime dv = lDateVariazioni[j];

                                    if (dv < dataFine)
                                    {
                                        DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                        if (lDateVariazioni[j + 1] == dataFine)
                                        {
                                            dvSucc = lDateVariazioni[j + 1];
                                        }

                                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                        {

                                            RptMaggiorazioniFigli rpts = new RptMaggiorazioniFigli()
                                            {

                                                DataInizioValidita = Convert.ToDateTime(dv).ToShortDateString(),
                                                DataFineValidita = (dvSucc < Utility.DataFineStop()) ? Convert.ToDateTime(dvSucc).ToShortDateString() : null,
                                                IndennitaServizioPrimoSegretario = ci.IndennitaServizioPrimoSegretario,
                                                PercentualeMaggiorazioniFigli = ci.PercentualeMaggiorazioneFigli,
                                                MaggiorazioniFigli = ci.MaggiorazioneFigli

                                            };
                                            rpt.Add(rpts);
                                        }

                                    }
                                }
                            }
                        }


                        #endregion



                        ReportViewer reportViewer = new ReportViewer();

                        reportViewer.ProcessingMode = ProcessingMode.Local;
                        reportViewer.SizeToReportContent = true;
                        reportViewer.Width = Unit.Percentage(100);
                        reportViewer.Height = Unit.Percentage(100);

                        var datasource = new ReportDataSource("DataSetMaggiorazioniFigli");

                        reportViewer.Visible = true;
                        reportViewer.ProcessingMode = ProcessingMode.Local;

                        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptMaggiorazioniFigli.rdlc";
                        reportViewer.LocalReport.DataSources.Clear();
                        reportViewer.LocalReport.DataSources.Add(datasource);
                        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetMaggiorazioniFigli", rpt));
                        reportViewer.LocalReport.Refresh();

                        List<ReportParameter> parameterValues = new List<ReportParameter>();
                        parameterValues.Add(new ReportParameter("Nominativo", Nominativo));
                        parameterValues.Add(new ReportParameter("Livello", Livello));
                        parameterValues.Add(new ReportParameter("Decorrenza", Decorrenza));
                        parameterValues.Add(new ReportParameter("Ufficio", Ufficio));

                        reportViewer.LocalReport.SetParameters(parameterValues);
                        ViewBag.ReportViewer = reportViewer;
                    }


                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView("RptMaggiorazioniFigli");
        }

        #endregion

        #region Maggiorazioni Familiari (Maggiorazioni Coniuge e Figli) + Report di Stampa
        public ActionResult MaggiorazioniFamiliari(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();
            dipInfoTrasferimentoModel dit = new dipInfoTrasferimentoModel();

            try
            {

                using (dtEvoluzioneIndennita dtei = new dtEvoluzioneIndennita())
                {
                    eim = dtei.GetMaggiorazioniFamiliariEvoluzione(idTrasferimento).ToList();

                }
                ViewBag.idTrasferimento = idTrasferimento;


                return PartialView(eim);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

        }

        public ActionResult RptMaggiorazioniFamiliari(decimal idTrasferimento)
        {

            try
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

                        ReportViewer reportViewer = new ReportViewer();

                        reportViewer.ProcessingMode = ProcessingMode.Local;
                        reportViewer.SizeToReportContent = true;
                        reportViewer.Width = Unit.Percentage(100);
                        reportViewer.Height = Unit.Percentage(100);

                        //var datasource = new ReportDataSource("DSRiepilogoVoci", lTeorici.ToList());
                        reportViewer.Visible = true;
                        reportViewer.ProcessingMode = ProcessingMode.Local;
                        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptMaggiorazioniFamiliari.rdlc";
                        reportViewer.LocalReport.DataSources.Clear();
                        //reportViewer.LocalReport.DataSources.Add(datasource);

                        reportViewer.LocalReport.Refresh();
                        reportViewer.ShowReportBody = true;

                        ReportParameter[] parameterValues = new ReportParameter[]
                        {
                            new ReportParameter ("Nominativo",Nominativo),
                            new ReportParameter ("Livello",Livello),
                            new ReportParameter ("Decorrenza",Decorrenza),
                            new ReportParameter ("Ufficio",Ufficio)

                        };

                        reportViewer.LocalReport.SetParameters(parameterValues);
                        ViewBag.ReportViewer = reportViewer;
                    }
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView("RptMaggiorazioniFamiliari");
        }

        #endregion

        #region Indennità Personale + Report di Stampa
        public ActionResult IndennitaPersonale(decimal idTrasferimento)
        {

            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {

                using (dtEvoluzioneIndennita dtei = new dtEvoluzioneIndennita())
                {
                    eim =
                        dtei.GetIndennitaPersonaleEvoluzione(idTrasferimento).ToList();

                }

                ViewBag.idTrasferimento = idTrasferimento;


                return PartialView("IndennitaPersonale", eim);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

        }
        public ActionResult RptIndennitaPersonale(decimal idTrasferimento)
        {
            List<RptIndennitaPersonaleModel> rpt = new List<RptIndennitaPersonaleModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var dipendente = trasferimento.DIPENDENTI;

                    using (dtLivelliDipendente dld = new dtLivelliDipendente())
                    {

                        ViewBag.idTrasferimento = idTrasferimento;

                        var llivm = dld.GetLivelloDipendenteByIdTrasferimento(idTrasferimento);
                        var livm = llivm.OrderBy(a => a.dataInizioValdita).Last();

                        string Nominativo = dipendente.COGNOME + " " + dipendente.NOME;
                        string Decorrenza = Convert.ToDateTime(trasferimento.DATAPARTENZA).ToShortDateString();
                        string Livello = livm.Livello.DescLivello;
                        string Ufficio = trasferimento.UFFICI.DESCRIZIONEUFFICIO;

                        var indennita = trasferimento.INDENNITA;
                        var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                        List<DateTime> lDateVariazioni = new List<DateTime>();

                        var liv =
                        indennita.LIVELLIDIPENDENTI
                            .Where(a => a.ANNULLATO == false &&
                                        a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                        a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                            .OrderBy(a => a.DATAINIZIOVALIDITA)
                            .ToList();

                        foreach (var l in liv)
                        {
                            DateTime dtIni;
                            DateTime dtFin;

                            if (l.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                            {
                                dtIni = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtIni = l.DATAINIZIOVALIDITA;
                            }

                            if (l.DATAFINEVALIDITA > trasferimento.DATARIENTRO)
                            {
                                dtFin = trasferimento.DATARIENTRO;
                            }
                            else
                            {
                                dtFin = l.DATAFINEVALIDITA;
                            }

                            #region Variazioni di indennità di base

                            var ll =
                                indennita.INDENNITABASE
                                    .Where(a => a.ANNULLATO == false &&
                                                a.DATAINIZIOVALIDITA <= dtFin &&
                                                a.DATAFINEVALIDITA >= dtIni)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                            foreach (var ib in ll)
                            {
                                DateTime dtVar = new DateTime();

                                if (ib.DATAINIZIOVALIDITA < dtIni)
                                {
                                    dtVar = dtIni;
                                }
                                else
                                {
                                    dtVar = ib.DATAINIZIOVALIDITA;
                                }


                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                }
                            }

                            #endregion

                            #region Variazioni del coefficiente di sede

                            var lrd =
                                indennita.COEFFICIENTESEDE
                                    .Where(a => a.ANNULLATO == false &&
                                                a.DATAINIZIOVALIDITA <= dtFin &&
                                                a.DATAFINEVALIDITA >= dtIni)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                            foreach (var cs in lrd)
                            {
                                DateTime dtVar = new DateTime();

                                if (cs.DATAINIZIOVALIDITA < dtIni)
                                {
                                    dtVar = dtIni;
                                }
                                else
                                {
                                    dtVar = cs.DATAINIZIOVALIDITA;
                                }

                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                }
                            }

                            #endregion

                            #region Variazioni percentuale di disagio

                            var perc =
                                indennita.PERCENTUALEDISAGIO
                                    .Where(a => a.ANNULLATO == false &&
                                                a.DATAINIZIOVALIDITA <= dtFin &&
                                                a.DATAFINEVALIDITA >= dtIni)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                            foreach (var pd in perc)
                            {
                                DateTime dtVar = new DateTime();

                                if (pd.DATAINIZIOVALIDITA < dtIni)
                                {
                                    dtVar = dtIni;
                                }
                                else
                                {
                                    dtVar = pd.DATAINIZIOVALIDITA;
                                }

                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                }
                            }

                            #endregion

                        }

                        #region Variazioni Coniuge

                        var lf =
                            mf.CONIUGE.Where(
                                a =>
                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                    a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                    a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        if (lf?.Any() ?? false)
                        {
                            foreach (var c in lf)
                            {

                                DateTime dtVar = new DateTime();

                                if (c.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                {
                                    dtVar = trasferimento.DATAPARTENZA;
                                }
                                else
                                {
                                    dtVar = c.DATAINIZIOVALIDITA;
                                }

                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                }

                                if (c.DATAFINEVALIDITA < Utility.DataFineStop())
                                {
                                    if (c.DATAFINEVALIDITA < trasferimento.DATARIENTRO)
                                    {
                                        dtVar = c.DATAFINEVALIDITA.AddDays(1);
                                    }
                                }

                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                }

                                var lpmc =
                                    c.PERCENTUALEMAGCONIUGE.Where(
                                        a =>
                                            a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= c.DATAFINEVALIDITA &&
                                            a.DATAFINEVALIDITA >= c.DATAINIZIOVALIDITA)
                                        .OrderBy(a => a.DATAINIZIOVALIDITA)
                                        .ToList();
                                if (lpmc?.Any() ?? false)
                                {
                                    foreach (var pmc in lpmc)
                                    {
                                        if (pmc.DATAINIZIOVALIDITA < c.DATAINIZIOVALIDITA)
                                        {
                                            dtVar = c.DATAINIZIOVALIDITA;
                                        }
                                        else
                                        {
                                            dtVar = pmc.DATAINIZIOVALIDITA;
                                        }

                                        if (!lDateVariazioni.Contains(dtVar))
                                        {
                                            lDateVariazioni.Add(dtVar);
                                        }
                                    }
                                }

                            }
                        }

                        #endregion

                        #region Variazioni Figli

                        var lf1 =
                            mf.FIGLI.Where(
                                a =>
                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                    a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                    a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        if (lf1?.Any() ?? false)
                        {
                            foreach (var f in lf1)
                            {

                                DateTime dtVar = new DateTime();

                                if (f.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                {
                                    dtVar = trasferimento.DATAPARTENZA;
                                }
                                else
                                {
                                    dtVar = f.DATAINIZIOVALIDITA;
                                }

                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                }

                                if (f.DATAFINEVALIDITA < Utility.DataFineStop())
                                {
                                    if (f.DATAFINEVALIDITA < trasferimento.DATARIENTRO)
                                    {
                                        dtVar = f.DATAFINEVALIDITA.AddDays(1);
                                    }
                                }

                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                }

                                var lpmf =
                                    f.PERCENTUALEMAGFIGLI.Where(
                                        a =>
                                            a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= f.DATAFINEVALIDITA &&
                                            a.DATAFINEVALIDITA >= f.DATAINIZIOVALIDITA)
                                        .OrderBy(a => a.DATAINIZIOVALIDITA)
                                        .ToList();
                                if (lpmf?.Any() ?? false)
                                {
                                    foreach (var pmf in lpmf)
                                    {
                                        if (pmf.DATAINIZIOVALIDITA < f.DATAINIZIOVALIDITA)
                                        {
                                            dtVar = f.DATAINIZIOVALIDITA;
                                        }
                                        else
                                        {
                                            dtVar = pmf.DATAINIZIOVALIDITA;
                                        }

                                        if (!lDateVariazioni.Contains(dtVar))
                                        {
                                            lDateVariazioni.Add(dtVar);
                                        }
                                    }
                                }

                                var lips =
                                    f.INDENNITAPRIMOSEGRETARIO.Where(
                                        a =>
                                            a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= f.DATAFINEVALIDITA &&
                                            a.DATAFINEVALIDITA >= f.DATAINIZIOVALIDITA)
                                        .OrderBy(a => a.DATAINIZIOVALIDITA)
                                        .ToList();
                                if (lips?.Any() ?? false)
                                {
                                    foreach (var ips in lips)
                                    {
                                        if (ips.DATAINIZIOVALIDITA < f.DATAINIZIOVALIDITA)
                                        {
                                            dtVar = f.DATAINIZIOVALIDITA;
                                        }
                                        else
                                        {
                                            dtVar = ips.DATAINIZIOVALIDITA;
                                        }

                                        if (!lDateVariazioni.Contains(dtVar))
                                        {
                                            lDateVariazioni.Add(dtVar);
                                        }
                                    }
                                }

                            }
                        }
                        #endregion

                        DateTime dataFine;

                        if (trasferimento.DATARIENTRO < Utility.DataFineStop())
                        {
                            dataFine = trasferimento.DATARIENTRO;
                        }
                        else
                        {
                            dataFine = Utility.DataFineStop();
                        }

                        if (!lDateVariazioni.Contains(dataFine))
                        {
                            lDateVariazioni.Add(dataFine);
                        }

                        if (lDateVariazioni?.Any() ?? false)
                        {
                            for (int j = 0; j < lDateVariazioni.Count; j++)
                            {
                                DateTime dv = lDateVariazioni[j];

                                if (dv < dataFine)
                                {
                                    DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                    if (lDateVariazioni[j + 1] == dataFine)
                                    {
                                        dvSucc = lDateVariazioni[j + 1];
                                    }

                                    using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                    {
                                        RptIndennitaPersonaleModel rpts = new RptIndennitaPersonaleModel()
                                        {

                                            DataInizioValidita = Convert.ToDateTime(dv).ToShortDateString(),
                                            //DataFineValidita = Convert.ToDateTime(dvSucc).ToShortDateString(),
                                            DataFineValidita = (dvSucc < Utility.DataFineStop()) ? Convert.ToDateTime(dvSucc).ToShortDateString() : null,
                                            IndennitaBase = ci.IndennitaDiBase,
                                            IndennitaServizio = ci.IndennitaDiServizio,
                                            MaggiorazioneConiuge = ci.MaggiorazioneConiuge,
                                            MaggiorazioneFigli = ci.MaggiorazioneFigli,
                                            IndennitaPersonale = ci.IndennitaPersonale

                                        };

                                        rpt.Add(rpts);



                                    }



                                }
                            }
                        }


                        ReportViewer reportViewer = new ReportViewer();

                        reportViewer.ProcessingMode = ProcessingMode.Local;
                        reportViewer.SizeToReportContent = true;
                        reportViewer.Width = Unit.Percentage(100);
                        reportViewer.Height = Unit.Percentage(100);

                        var datasource = new ReportDataSource("DataSetIndennitaPersonale");

                        reportViewer.Visible = true;
                        reportViewer.ProcessingMode = ProcessingMode.Local;

                        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptIndennitaPersonale.rdlc";
                        reportViewer.LocalReport.DataSources.Clear();
                        reportViewer.LocalReport.DataSources.Add(datasource);
                        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetIndennitaPersonale", rpt));
                        reportViewer.LocalReport.Refresh();

                        List<ReportParameter> parameterValues = new List<ReportParameter>();
                        parameterValues.Add(new ReportParameter("Nominativo", Nominativo));
                        parameterValues.Add(new ReportParameter("Livello", Livello));
                        parameterValues.Add(new ReportParameter("Decorrenza", Decorrenza));
                        parameterValues.Add(new ReportParameter("Ufficio", Ufficio));

                        reportViewer.LocalReport.SetParameters(parameterValues);
                        ViewBag.ReportViewer = reportViewer;


                    }


                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView("RptIndennitaPersonale");
        }
        #endregion

        #region Maggiorazione Abitazione + Report di Stampa
        public ActionResult MaggiorazioneAbitazione(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();
            dipInfoTrasferimentoModel dit = new dipInfoTrasferimentoModel();

            try
            {

                using (dtEvoluzioneIndennita dtei = new dtEvoluzioneIndennita())
                {
                    eim = dtei.GetMaggiorazioneAbitazioneEvoluzione(idTrasferimento).ToList();

                }


                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tm = dtt.GetTrasferimentoById(idTrasferimento);
                    using (dtRuoloUfficio dtru = new dtRuoloUfficio())
                    {
                        tm.RuoloUfficio = dtru.GetRuoloUfficioValidoByIdTrasferimento(tm.idTrasferimento);
                        ViewBag.idRuoloUfficio = tm.idRuoloUfficio;

                    }


                }

                ViewBag.idTrasferimento = idTrasferimento;


                return PartialView(eim);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

        }
        public ActionResult RptMaggiorazioneAbitazione(decimal idTrasferimento)
        {
            List<RptMaggiorazioneAbitazione> rpt = new List<RptMaggiorazioneAbitazione>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var indennita = trasferimento.INDENNITA;
                    var dipendente = trasferimento.DIPENDENTI;


                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        //var tm = dtt.GetTrasferimentoById(idTrasferimento);

                        List<DateTime> lDateVariazioni = new List<DateTime>();

                        //var lmab = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.MAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();

                        using (dtLivelliDipendente dld = new dtLivelliDipendente())
                        {
                            ViewBag.idTrasferimento = idTrasferimento;

                            var llivm = dld.GetLivelloDipendenteByIdTrasferimento(idTrasferimento);
                            var livm = llivm.OrderBy(a => a.dataInizioValdita).Last();

                            string Nominativo = dipendente.COGNOME + " " + dipendente.NOME;
                            string Decorrenza = Convert.ToDateTime(trasferimento.DATAPARTENZA).ToShortDateString();
                            string Livello = livm.Livello.DescLivello;
                            string Ufficio = trasferimento.UFFICI.DESCRIZIONEUFFICIO;

                            var lmab = indennita.MAB
                                .Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                            a.RINUNCIAMAB == false)
                                .OrderBy(a => a.IDMAB)
                                .ToList();

                            if (lmab?.Any() ?? false)
                            {
                                foreach (var mab in lmab)
                                {
                                    var lpmab =
                                        mab.PERIODOMAB.Where(
                                            a =>
                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                a.DATAINIZIOMAB <= trasferimento.DATARIENTRO &&
                                                a.DATAFINEMAB >= trasferimento.DATAPARTENZA)
                                            .OrderBy(a => a.IDPERIODOMAB)
                                            .ToList();

                                    if (lpmab?.Any() ?? false)
                                    {
                                        var pmab = lpmab.Last();

                                        DateTime dtIni = pmab.DATAINIZIOMAB;
                                        DateTime dtFin = pmab.DATAFINEMAB;

                                        if (trasferimento.DATARIENTRO < pmab.DATAFINEMAB)
                                        {
                                            dtFin = trasferimento.DATARIENTRO;
                                        }

                                        var liv =
                                            indennita.LIVELLIDIPENDENTI
                                                .Where(a => a.ANNULLATO == false &&
                                                            a.DATAINIZIOVALIDITA <= dtFin &&
                                                            a.DATAFINEVALIDITA >= dtIni)
                                                .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                .ToList();

                                        foreach (var l in liv)
                                        {
                                            DateTime dtIniLiv;
                                            DateTime dtFinLiv;

                                            if (l.DATAINIZIOVALIDITA < dtIni)
                                            {
                                                dtIniLiv = dtIni;
                                            }
                                            else
                                            {
                                                dtIniLiv = l.DATAINIZIOVALIDITA;
                                            }

                                            if (l.DATAFINEVALIDITA > dtFin)
                                            {
                                                dtFinLiv = dtFin;
                                            }
                                            else
                                            {
                                                dtFinLiv = l.DATAFINEVALIDITA;
                                            }

                                            #region Variazioni Indennità di Base

                                            var ll =
                                                indennita.INDENNITABASE
                                                    .Where(a => a.ANNULLATO == false &&
                                                                a.DATAINIZIOVALIDITA <= dtFinLiv &&
                                                                a.DATAFINEVALIDITA >= dtIniLiv)
                                                    .OrderBy(a => a.IDLIVELLO)
                                                    .ThenBy(a => a.DATAINIZIOVALIDITA)
                                                    .ThenBy(a => a.DATAFINEVALIDITA)
                                                    .ToList();

                                            foreach (var ib in ll)
                                            {
                                                DateTime dtVar = new DateTime();

                                                if (ib.DATAINIZIOVALIDITA < dtIniLiv)
                                                {
                                                    dtVar = dtIniLiv;
                                                }
                                                else
                                                {
                                                    dtVar = ib.DATAINIZIOVALIDITA;
                                                }

                                                if (!lDateVariazioni.Contains(dtVar))
                                                {
                                                    lDateVariazioni.Add(dtVar);
                                                }
                                            }

                                            #endregion

                                            #region Variazioni Coefficente di Sede

                                            var lrd =
                                                indennita.COEFFICIENTESEDE
                                                    .Where(a => a.ANNULLATO == false &&
                                                                a.DATAINIZIOVALIDITA <= dtFinLiv &&
                                                                a.DATAFINEVALIDITA >= dtIniLiv)
                                                    .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                            foreach (var cs in lrd)
                                            {
                                                DateTime dtVar = new DateTime();

                                                if (cs.DATAINIZIOVALIDITA < dtIniLiv)
                                                {
                                                    dtVar = dtIniLiv;
                                                }
                                                else
                                                {
                                                    dtVar = cs.DATAINIZIOVALIDITA;
                                                }

                                                if (!lDateVariazioni.Contains(dtVar))
                                                {
                                                    lDateVariazioni.Add(dtVar);

                                                }
                                            }
                                            #endregion

                                            #region Variazioni Percentuale di Disagio

                                            var perc =
                                                indennita.PERCENTUALEDISAGIO
                                                    .Where(a => a.ANNULLATO == false &&
                                                                a.DATAINIZIOVALIDITA <= dtFinLiv &&
                                                                a.DATAFINEVALIDITA >= dtIniLiv)
                                                    .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                                            foreach (var pd in perc)
                                            {
                                                DateTime dtVar = new DateTime();

                                                if (pd.DATAINIZIOVALIDITA < dtIniLiv)
                                                {
                                                    dtVar = dtIniLiv;
                                                }
                                                else
                                                {
                                                    dtVar = pd.DATAINIZIOVALIDITA;
                                                }

                                                if (!lDateVariazioni.Contains(dtVar))
                                                {
                                                    lDateVariazioni.Add(dtVar);
                                                }
                                            }

                                            #endregion

                                        }


                                        var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                                        #region Variazioni Coniuge (Coniuge - Perc. Magg. Coniuge - Pensione)

                                        var lc =
                                            mf.CONIUGE.Where(
                                                a =>
                                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                    a.DATAINIZIOVALIDITA <= dtFin && a.DATAFINEVALIDITA >= dtIni)
                                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                        if (lc?.Any() ?? false)
                                        {
                                            foreach (var coniuge in lc)
                                            {
                                                var lpmc =
                                                    coniuge.PERCENTUALEMAGCONIUGE.Where(
                                                        a =>
                                                            a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= coniuge.DATAFINEVALIDITA &&
                                                            a.DATAFINEVALIDITA >= coniuge.DATAINIZIOVALIDITA)
                                                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                                if (lpmc?.Any() ?? false)
                                                {
                                                    foreach (var pmc in lpmc)
                                                    {
                                                        DateTime dtVar = new DateTime();

                                                        if (pmc.DATAINIZIOVALIDITA < coniuge.DATAINIZIOVALIDITA)
                                                        {
                                                            dtVar = coniuge.DATAINIZIOVALIDITA;
                                                        }
                                                        else
                                                        {
                                                            dtVar = pmc.DATAINIZIOVALIDITA;
                                                        }

                                                        if (!lDateVariazioni.Contains(dtVar))
                                                        {
                                                            lDateVariazioni.Add(dtVar);

                                                        }

                                                    }
                                                }

                                                var lpensioni =
                                                    coniuge.PENSIONE.Where(
                                                        a =>
                                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                            a.DATAINIZIO <= coniuge.DATAFINEVALIDITA &&
                                                            a.DATAFINE >= coniuge.DATAINIZIOVALIDITA)
                                                        .OrderBy(a => a.DATAINIZIO)
                                                        .ToList();

                                                if (lpensioni?.Any() ?? false)
                                                {
                                                    foreach (var pensioni in lpensioni)
                                                    {
                                                        DateTime dtVar = new DateTime();

                                                        if (pensioni.DATAINIZIO < coniuge.DATAINIZIOVALIDITA)
                                                        {
                                                            dtVar = coniuge.DATAINIZIOVALIDITA;
                                                        }
                                                        else
                                                        {
                                                            dtVar = pensioni.DATAINIZIO;
                                                        }
                                                        if (!lDateVariazioni.Contains(dtVar))
                                                        {
                                                            lDateVariazioni.Add(dtVar);

                                                        }

                                                    }
                                                }
                                            }
                                        }
                                        #endregion

                                        #region Variazioni Figli (Figli - Percentuale Magg. Figli - Ind. Primo Segretario)

                                        var lf =
                                            mf.FIGLI.Where(
                                                a =>
                                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                    a.DATAINIZIOVALIDITA <= dtFin && a.DATAFINEVALIDITA >= dtIni)
                                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                        if (lf?.Any() ?? false)
                                        {
                                            foreach (var f in lf)
                                            {
                                                var lpmf =
                                                    f.PERCENTUALEMAGFIGLI.Where(
                                                        a =>
                                                            a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= f.DATAFINEVALIDITA &&
                                                            a.DATAFINEVALIDITA >= f.DATAINIZIOVALIDITA)
                                                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                                if (lpmf?.Any() ?? false)
                                                {
                                                    foreach (var pmf in lpmf)
                                                    {
                                                        DateTime dtVar = new DateTime();

                                                        if (pmf.DATAINIZIOVALIDITA < f.DATAINIZIOVALIDITA)
                                                        {
                                                            dtVar = f.DATAINIZIOVALIDITA;
                                                        }
                                                        else
                                                        {
                                                            dtVar = pmf.DATAINIZIOVALIDITA;
                                                        }

                                                        if (!lDateVariazioni.Contains(dtVar))
                                                        {
                                                            lDateVariazioni.Add(dtVar);
                                                        }

                                                    }
                                                }

                                                var lips =
                                                    f.INDENNITAPRIMOSEGRETARIO.Where(
                                                        a =>
                                                            a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= f.DATAFINEVALIDITA &&
                                                            a.DATAFINEVALIDITA >= f.DATAINIZIOVALIDITA)
                                                        .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                        .ToList();

                                                if (lips?.Any() ?? false)
                                                {
                                                    foreach (var ips in lips)
                                                    {
                                                        DateTime dtVar = new DateTime();

                                                        if (ips.DATAINIZIOVALIDITA < f.DATAINIZIOVALIDITA)
                                                        {
                                                            dtVar = f.DATAINIZIOVALIDITA;
                                                        }
                                                        else
                                                        {
                                                            dtVar = ips.DATAINIZIOVALIDITA;
                                                        }

                                                        if (!lDateVariazioni.Contains(dtVar))
                                                        {
                                                            lDateVariazioni.Add(dtVar);

                                                        }
                                                    }

                                                }





                                            }
                                        }

                                        #endregion

                                        #region Variazioni del Canone MAB (TFR - PagatoCondivisoMAB - Perc. Condivisione MAB)

                                        var lcmab =
                                            mab.CANONEMAB.Where(
                                                a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                     a.ATTIVAZIONEMAB.ANNULLATO == false &&
                                                     a.ATTIVAZIONEMAB.NOTIFICARICHIESTA == true &&
                                                     a.ATTIVAZIONEMAB.ATTIVAZIONE == true &&
                                                     a.DATAINIZIOVALIDITA <= dtFin && a.DATAFINEVALIDITA >= dtIni)
                                                .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                .ToList();

                                        foreach (var cmab in lcmab)
                                        {
                                            DateTime dtVar = new DateTime();

                                            if (cmab.DATAINIZIOVALIDITA < dtIni)
                                            {
                                                dtVar = dtIni;
                                            }
                                            else
                                            {
                                                dtVar = cmab.DATAINIZIOVALIDITA;
                                            }

                                            if (!lDateVariazioni.Contains(dtVar))
                                            {
                                                lDateVariazioni.Add(dtVar);
                                            }

                                            var ltfr =
                                                cmab.TFR.Where(
                                                    a =>
                                                        a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= cmab.DATAFINEVALIDITA &&
                                                        a.DATAFINEVALIDITA >= cmab.DATAINIZIOVALIDITA)
                                                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                    .ToList();

                                            foreach (var tfr in ltfr)
                                            {

                                                if (tfr.DATAINIZIOVALIDITA < cmab.DATAINIZIOVALIDITA)
                                                {
                                                    dtVar = cmab.DATAINIZIOVALIDITA;
                                                }
                                                else
                                                {
                                                    dtVar = tfr.DATAINIZIOVALIDITA;
                                                }

                                                if (!lDateVariazioni.Contains(dtVar))
                                                {
                                                    lDateVariazioni.Add(dtVar);
                                                }
                                            }

                                            var lpc =
                                                mab.PAGATOCONDIVISOMAB.Where(
                                                    a =>
                                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                        a.DATAINIZIOVALIDITA <= dtFin && a.DATAFINEVALIDITA >= dtIni &&
                                                        a.ATTIVAZIONEMAB.ANNULLATO == false &&
                                                        a.ATTIVAZIONEMAB.NOTIFICARICHIESTA == true &&
                                                        a.ATTIVAZIONEMAB.ATTIVAZIONE == true)
                                                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                    .ToList();

                                            if (lpc?.Any() ?? false)
                                            {
                                                foreach (var pc in lpc)
                                                {

                                                    if (pc.CONDIVISO == true)
                                                    {
                                                        if (pc.DATAINIZIOVALIDITA < dtIni)
                                                        {
                                                            dtVar = dtIni;
                                                        }
                                                        else
                                                        {
                                                            dtVar = pc.DATAINIZIOVALIDITA;
                                                        }

                                                        if (!lDateVariazioni.Contains(dtVar))
                                                        {
                                                            lDateVariazioni.Add(dtVar);
                                                        }

                                                        if (pc.CONDIVISO == true && pc.PAGATO == true)
                                                        {
                                                            var lpercCond =
                                                                pc.PERCENTUALECONDIVISIONE.Where(
                                                                        a =>
                                                                            a.ANNULLATO == false &&
                                                                            a.DATAFINEVALIDITA >= pc.DATAINIZIOVALIDITA &&
                                                                            a.DATAINIZIOVALIDITA <= pc.DATAFINEVALIDITA)
                                                                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                                    .ToList();

                                                            if (lpercCond?.Any() ?? false)
                                                            {
                                                                foreach (var percCond in lpercCond)
                                                                {

                                                                    if (percCond.DATAINIZIOVALIDITA < pc.DATAINIZIOVALIDITA)
                                                                    {
                                                                        dtVar = pc.DATAINIZIOVALIDITA;
                                                                    }
                                                                    else
                                                                    {
                                                                        dtVar = pc.DATAINIZIOVALIDITA;
                                                                    }

                                                                    if (!lDateVariazioni.Contains(dtVar))
                                                                    {
                                                                        lDateVariazioni.Add(dtVar);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                }

                                DateTime dataFineMab;

                                if (trasferimento.DATARIENTRO < Utility.DataFineStop())
                                {
                                    dataFineMab = trasferimento.DATARIENTRO;

                                    if (!lDateVariazioni.Contains(dataFineMab))
                                    {
                                        lDateVariazioni.Add(dataFineMab);
                                    }
                                }
                                else
                                {
                                    if (!lDateVariazioni.Contains(Utility.DataFineStop()))
                                    {
                                        lDateVariazioni.Add(Utility.DataFineStop());
                                    }
                                    dataFineMab = Utility.DataFineStop();
                                }

                                lDateVariazioni = lDateVariazioni.OrderBy(a => a.Year).ThenBy(a => a.Month).ThenBy(a => a.Day).ToList();

                                if (lDateVariazioni?.Any() ?? false)
                                {
                                    for (int j = 0; j < lDateVariazioni.Count; j++)
                                    {
                                        DateTime dv = lDateVariazioni[j];

                                        if (dv < dataFineMab)
                                        {
                                            DateTime dvSucc;

                                            if (lDateVariazioni[j + 1] == dataFineMab)
                                            {
                                                dvSucc = lDateVariazioni[j + 1];
                                            }
                                            else
                                            {
                                                dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);
                                            }

                                            using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                            {
                                                RptMaggiorazioneAbitazione rpts = new RptMaggiorazioneAbitazione()
                                                {

                                                    DataInizioValidita = Convert.ToDateTime(dv).ToShortDateString(),
                                                    //DataFineValidita = Convert.ToDateTime(dvSucc).ToShortDateString(),
                                                    //DataFineValidita = (lDateVariazioni[(j + 1)] < Utility.DataFineStop()) ? Convert.ToDateTime(dvSucc).ToShortDateString() : null,
                                                    DataFineValidita = (dvSucc < Utility.DataFineStop()) ? Convert.ToDateTime(dvSucc).ToShortDateString() : null,
                                                    //CanoneLocazioneinValuta = ci.CanoneMAB,
                                                    CanoneLocazioneinEuro = ci.CanoneMABEuro,
                                                    PercentualeMaggAbitazione = ci.PercentualeMAB,
                                                    ImportoMABMensile = ci.ImportoMABMensile,
                                                    CanoneMAB = ci.CanoneMAB,
                                                    valutaMAB = ci.ValutaMAB.DESCRIZIONEVALUTA,
                                                    TassoFissoRagguaglio = ci.TassoCambio,
                                                    ImportoMABMaxMensile = ci.ImportoMABMaxMensile
                                                };
                                                rpt.Add(rpts);
                                            }
                                        }
                                    }
                                }


                                ReportViewer reportViewer = new ReportViewer();

                                reportViewer.ProcessingMode = ProcessingMode.Local;
                                reportViewer.SizeToReportContent = true;
                                reportViewer.Width = Unit.Percentage(100);
                                reportViewer.Height = Unit.Percentage(100);

                                var datasource = new ReportDataSource("DataSetMaggiorazioneAbitazione");

                                reportViewer.Visible = true;
                                reportViewer.ProcessingMode = ProcessingMode.Local;

                                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptMaggiorazioneAbitazione.rdlc";
                                reportViewer.LocalReport.DataSources.Clear();
                                reportViewer.LocalReport.DataSources.Add(datasource);
                                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetMaggiorazioneAbitazione", rpt));
                                reportViewer.LocalReport.Refresh();

                                List<ReportParameter> parameterValues = new List<ReportParameter>();
                                parameterValues.Add(new ReportParameter("Nominativo", Nominativo));
                                parameterValues.Add(new ReportParameter("Livello", Livello));
                                parameterValues.Add(new ReportParameter("Decorrenza", Decorrenza));
                                parameterValues.Add(new ReportParameter("Ufficio", Ufficio));

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
            return PartialView("RptMaggiorazioneAbitazione");
        }
        #endregion

        #region Indennita di Prima Sistemazione + Report di Stampa

        public ActionResult IndennitaPrimaSistemazione(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();
            dipInfoTrasferimentoModel dit = new dipInfoTrasferimentoModel();

            try
            {

                using (dtEvoluzioneIndennita dtei = new dtEvoluzioneIndennita())
                {
                    eim = dtei.GetIndennitaEvoluzione(idTrasferimento).ToList();

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


                return PartialView(eim);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

        }
        public ActionResult RptIndennitaPrimaSistemazione(decimal idTrasferimento)
        {

            try
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

                        ReportViewer reportViewer = new ReportViewer();

                        reportViewer.ProcessingMode = ProcessingMode.Local;
                        reportViewer.SizeToReportContent = true;
                        reportViewer.Width = Unit.Percentage(100);
                        reportViewer.Height = Unit.Percentage(100);

                        //var datasource = new ReportDataSource("DSRiepilogoVoci", lTeorici.ToList());
                        reportViewer.Visible = true;
                        reportViewer.ProcessingMode = ProcessingMode.Local;
                        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptIndennitaPrimaSistemazione.rdlc";
                        reportViewer.LocalReport.DataSources.Clear();
                        //reportViewer.LocalReport.DataSources.Add(datasource);

                        reportViewer.LocalReport.Refresh();
                        reportViewer.ShowReportBody = true;

                        ReportParameter[] parameterValues = new ReportParameter[]
                        {
                                new ReportParameter ("Nominativo",Nominativo),
                                new ReportParameter ("Livello",Livello),
                                new ReportParameter ("Decorrenza",Decorrenza),
                                new ReportParameter ("Ufficio",Ufficio)

                       };

                        reportViewer.LocalReport.SetParameters(parameterValues);
                        ViewBag.ReportViewer = reportViewer;
                    }

                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView("RptIndennitaPrimaSistemazione");
        }
        #endregion

        #region Indennita di Richiamo + Report di Stampa

        public ActionResult IndennitadiRichiamo(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();
            dipInfoTrasferimentoModel dit = new dipInfoTrasferimentoModel();

            try
            {

                using (dtEvoluzioneIndennita dtei = new dtEvoluzioneIndennita())
                {
                    eim = dtei.GetIndennitaEvoluzione(idTrasferimento).ToList();

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


                return PartialView(eim);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

        }
        public ActionResult RptIndennitadiRichiamo(decimal idTrasferimento)
        {

            try
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




                        ReportViewer reportViewer = new ReportViewer();

                        reportViewer.ProcessingMode = ProcessingMode.Local;
                        reportViewer.SizeToReportContent = true;
                        reportViewer.Width = Unit.Percentage(100);
                        reportViewer.Height = Unit.Percentage(100);

                        //var datasource = new ReportDataSource("DSRiepilogoVoci", lTeorici.ToList());
                        reportViewer.Visible = true;
                        reportViewer.ProcessingMode = ProcessingMode.Local;
                        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptIndennitadiRichiamo.rdlc";
                        reportViewer.LocalReport.DataSources.Clear();
                        //reportViewer.LocalReport.DataSources.Add(datasource);

                        reportViewer.LocalReport.Refresh();
                        reportViewer.ShowReportBody = true;

                        ReportParameter[] parameterValues = new ReportParameter[]
                        {
                            new ReportParameter ("Nominativo",Nominativo),
                            new ReportParameter ("Livello",Livello),
                            new ReportParameter ("Decorrenza",Decorrenza),
                            new ReportParameter ("Ufficio",Ufficio)

                        };

                        reportViewer.LocalReport.SetParameters(parameterValues);
                        ViewBag.ReportViewer = reportViewer;

                    }
                }



            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView("RptIndennitadiRichiamo");
        }
        #endregion

        #region Indennita di Richiamo Lorda + Report di Stampa
        public ActionResult IndennitadiRichiamoLorda(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {

                using (dtEvoluzioneIndennita dtei = new dtEvoluzioneIndennita())
                {
                    eim = dtei.GetIndennitaRichiamoEvoluzione(idTrasferimento).ToList();

                }

                ViewBag.idTrasferimento = idTrasferimento;
                return PartialView(eim);

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }



        }
        public ActionResult RptIndennitadiRichiamoLorda(decimal idTrasferimento)
        {
            List<RptIndennitàRichiamoLordaModel> rpt = new List<RptIndennitàRichiamoLordaModel>();

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


                            var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                            List<DateTime> lDateVariazioni = new List<DateTime>();

                            #region Variazioni Richiamo
                            var richiamo =
                                    trasferimento.RICHIAMO
                                    .Where(a => a.ANNULLATO == false)
                                    .OrderByDescending(a => a.IDRICHIAMO).ToList();


                            DateTime DataRichiamo = DateTime.Now;

                            if (richiamo?.Any() ?? false)
                            {
                                foreach (var ib in richiamo)
                                {
                                    //DateTime dtVar = new DateTime();

                                    //if (ib.DATARICHIAMO < trasferimento.DATAPARTENZA)
                                    //{
                                    //    dtVar = trasferimento.DATAPARTENZA;
                                    //}
                                    //else
                                    //{
                                    //    dtVar = ib.DATARICHIAMO;
                                    //}


                                    //if (!lDateVariazioni.Contains(dtVar))
                                    //{
                                    //    lDateVariazioni.Add(dtVar);
                                    //    lDateVariazioni.Sort();
                                    //}

                                    DataRichiamo = ib.DATARICHIAMO;
                                }
                            }
                            #endregion

                            if (richiamo?.Any() ?? false)
                            {

                                #region Variazione Percentuale Coefficente di Richiamo
                                foreach (var coeff in richiamo)
                                {

                                    var coeffrichiamo =
                                              coeff.COEFFICIENTEINDRICHIAMO.Where(
                                                  a =>
                                                      a.ANNULLATO == false).ToList();

                                    DateTime dtVar = new DateTime();

                                    if (coeff.DATARICHIAMO < trasferimento.DATAPARTENZA)
                                    {
                                        dtVar = trasferimento.DATAPARTENZA;
                                    }
                                    else
                                    {
                                        dtVar = coeff.DATARICHIAMO;
                                    }


                                    if (!lDateVariazioni.Contains(dtVar))
                                    {
                                        lDateVariazioni.Add(dtVar);
                                        lDateVariazioni.Sort();
                                    }

                                }
                                #endregion
                            }
                            lDateVariazioni.Add(new DateTime(9999, 12, 31));

                            if (lDateVariazioni?.Any() ?? false)
                            {
                                for (int j = 0; j < lDateVariazioni.Count; j++)
                                {
                                    DateTime dv = lDateVariazioni[j];

                                    if (dv < Utility.DataFineStop())
                                    {
                                        //DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);
                                        DateTime dvSucc = lDateVariazioni[(j + 1)];

                                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, trasferimento.DATARIENTRO, db))
                                        {

                                            RptIndennitàRichiamoLordaModel rpts = new RptIndennitàRichiamoLordaModel()
                                            {

                                                //DataInizioValidita = Convert.ToDateTime(dv).ToShortDateString(),
                                                //DataFineValidita = Convert.ToDateTime(dvSucc).ToShortDateString(),
                                                //DataFineValidita = (dvSucc < Utility.DataFineStop()) ? Convert.ToDateTime(dvSucc).ToShortDateString() : null,
                                                IndennitaBase = ci.IndennitaDiBase,
                                                MaggiorazioneConiuge = ci.MaggiorazioneConiuge,
                                                MaggiorazioneFigli = ci.MaggiorazioneFigli,
                                                CoeffIndennitadiRichiamo = ci.CoefficenteIndennitaRichiamo,
                                                CoeffMaggIndennitadiRichiamo = ci.CoefficenteMaggiorazioneRichiamo,
                                                IndennitaRichiamo = ci.IndennitaRichiamoLordo,
                                                //dtRientro = DataRichiamo
                                                DataFineValidita = trasferimento.DATARIENTRO

                                            };

                                            rpt.Add(rpts);

                                        }

                                    }
                                }
                            }



                            ReportViewer reportViewer = new ReportViewer();

                            reportViewer.ProcessingMode = ProcessingMode.Local;
                            reportViewer.SizeToReportContent = true;
                            reportViewer.Width = Unit.Percentage(100);
                            reportViewer.Height = Unit.Percentage(100);

                            var datasource = new ReportDataSource("DataSetIndennitàRichiamo");

                            reportViewer.Visible = true;
                            reportViewer.ProcessingMode = ProcessingMode.Local;

                            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptIndennitadiRichiamoLorda.rdlc";
                            reportViewer.LocalReport.DataSources.Clear();
                            reportViewer.LocalReport.DataSources.Add(datasource);
                            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetIndennitàRichiamo", rpt));
                            reportViewer.LocalReport.Refresh();

                            List<ReportParameter> parameterValues = new List<ReportParameter>();
                            parameterValues.Add(new ReportParameter("Nominativo", Nominativo));
                            parameterValues.Add(new ReportParameter("Livello", Livello));
                            parameterValues.Add(new ReportParameter("Decorrenza", Decorrenza));
                            parameterValues.Add(new ReportParameter("Ufficio", Ufficio));


                            reportViewer.LocalReport.SetParameters(parameterValues);
                            ViewBag.ReportViewer = reportViewer;



                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView("RptIndennitadiRichiamoLorda");
        }

        #endregion

        #region Indennita di Richiamo Netta + Report di Stampa
        public ActionResult IndennitadiRichiamoNetta(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {



                ViewBag.idTrasferimento = idTrasferimento;

                return PartialView(eim);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }



        }
        public ActionResult RptIndennitadiRichiamoNetta(decimal idTrasferimento)
        {

            try
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

                        ReportViewer reportViewer = new ReportViewer();

                        reportViewer.ProcessingMode = ProcessingMode.Local;
                        reportViewer.SizeToReportContent = true;
                        reportViewer.Width = Unit.Percentage(100);
                        reportViewer.Height = Unit.Percentage(100);

                        //var datasource = new ReportDataSource("DSRiepilogoVoci", lTeorici.ToList());
                        reportViewer.Visible = true;
                        reportViewer.ProcessingMode = ProcessingMode.Local;
                        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptIndennitadiRichiamoNetta.rdlc";
                        reportViewer.LocalReport.DataSources.Clear();
                        //reportViewer.LocalReport.DataSources.Add(datasource);

                        reportViewer.LocalReport.Refresh();
                        reportViewer.ShowReportBody = true;

                        ReportParameter[] parameterValues = new ReportParameter[]
                        {
                            new ReportParameter ("Nominativo",Nominativo),
                            new ReportParameter ("Livello",Livello),
                            new ReportParameter ("Decorrenza",Decorrenza),
                            new ReportParameter ("Ufficio",Ufficio)

                        };

                        reportViewer.LocalReport.SetParameters(parameterValues);
                        ViewBag.ReportViewer = reportViewer;

                    }
                }


            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("RptIndennitadiRichiamoNetta");
        }

        #endregion

        #region Anticipo Indennita di Prima Sistemazione + Report di Stampa
        public ActionResult AnticipoIndennitadiSistemazioneLorda(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();
            List<RiepilogoVociModel> lrvm = new List<RiepilogoVociModel>();

            try
            {
                using (dtEvoluzioneIndennita dtei = new dtEvoluzioneIndennita())
                {
                    eim = dtei.GetAnticipoIndennitaSistemazioneEvoluzione(idTrasferimento).ToList();

                }
                ViewBag.idTrasferimento = idTrasferimento;

                return PartialView(eim);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        public ActionResult RptAnticipoIndennitadiSistemazioneLorda(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();
            List<RptAnticipoIndennitàdiSistemazione_Lorda> rpt = new List<RptAnticipoIndennitàdiSistemazione_Lorda>();

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

                            var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                            var indennita = trasferimento.TIPOTRASFERIMENTO.INDENNITASISTEMAZIONE;


                            using (dtEvoluzioneIndennita dtei = new dtEvoluzioneIndennita())
                            {
                                eim = dtei.GetAnticipoIndennitaSistemazioneEvoluzione(idTrasferimento).ToList();

                            }


                            if (eim?.Any() ?? false)
                            {
                                foreach (var lm in eim)
                                {
                                    RptAnticipoIndennitàdiSistemazione_Lorda rptds = new RptAnticipoIndennitàdiSistemazione_Lorda()
                                    {
                                        dataInizioValidita = lm.dataInizioValidita,
                                        dataFineValidita = lm.dataFineValidita,
                                        IndennitaServizio = lm.IndennitaServizio,
                                        AliquotaFiscale = lm.AliquotaFiscale,
                                        AliquotaPrevid = lm.AliquotaPrevid,
                                        RitenutaFiscale = lm.RitenutaFiscale,
                                        ImpFiscale = lm.ImpFiscale,
                                        ContrPrevid = lm.ContrPrevid,
                                        ImpPrevid = lm.ImpPrevid,
                                        Detrazione = lm.Detrazione,
                                        PercentualeRiduzionePrimaSistemazione = lm.PercentualeRiduzionePrimaSistemazione,
                                        CoeffIndSistemazione = lm.CoeffIndSistemazione,
                                        IndennitaSistemazione = lm.IndennitaSistemazione,
                                        IndennitaSistemazioneAnticipabileLorda = lm.IndennitaSistemazioneAnticipabileLorda,
                                        PercentualeAnticipoRichiesto = lm.PercentualeAnticipoRichiesto,
                                        Importo = lm.Importo,
                                        CoefficientediMaggiorazione = lm.CoefficientediMaggiorazione,
                                        TotaleMaggiorazioniFamiliari = lm.TotaleMaggiorazioniFamiliari,
                                        MaggiorazioniFigli = lm.MaggiorazioniFigli,
                                        MaggiorazioneConiuge = lm.MaggiorazioneConiuge,
                                        PercentualeMaggiorazioniFigli = lm.PercentualeMaggiorazioniFigli,
                                        PercentualeMaggConiuge = lm.PercentualeMaggConiuge,
                                        IndennitaPrimoSegretario = lm.IndennitaPrimoSegretario,
                                        IndennitaPersonale = lm.IndennitaPersonale,
                                        PercentualeDisagio = lm.PercentualeDisagio,
                                        IndennitaBase = lm.IndennitaBase,
                                        dataAnticipoSistemazione = lm.dataAnticipoSistemazione



                                    };

                                    rpt.Add(rptds);

                                }
                            }

                            ReportViewer reportViewer = new ReportViewer();

                            reportViewer.ProcessingMode = ProcessingMode.Local;
                            reportViewer.SizeToReportContent = true;
                            reportViewer.Width = Unit.Percentage(100);
                            reportViewer.Height = Unit.Percentage(100);

                            var datasource = new ReportDataSource("DataSetAnticipoIndennitàdiSistemazioneLorda");

                            reportViewer.Visible = true;
                            reportViewer.ProcessingMode = ProcessingMode.Local;

                            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptAnticipoIndennitadiSistemazioneLorda.rdlc";
                            reportViewer.LocalReport.DataSources.Clear();
                            reportViewer.LocalReport.DataSources.Add(datasource);
                            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetAnticipoIndennitàdiSistemazioneLorda", rpt));
                            reportViewer.LocalReport.Refresh();

                            List<ReportParameter> parameterValues = new List<ReportParameter>();
                            parameterValues.Add(new ReportParameter("Nominativo", Nominativo));
                            parameterValues.Add(new ReportParameter("Livello", Livello));
                            parameterValues.Add(new ReportParameter("Decorrenza", Decorrenza));
                            parameterValues.Add(new ReportParameter("Ufficio", Ufficio));


                            reportViewer.LocalReport.SetParameters(parameterValues);
                            ViewBag.ReportViewer = reportViewer;


                        }
                    }

                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("RptAnticipoIndennitadiSistemazioneLorda");
        }
        #endregion

        #region Saldo Indennita di Prima Sistemazione  + Report di Stampa
        public ActionResult IndennitadiSistemazioneLorda(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();
            List<RiepilogoVociModel> lrvm = new List<RiepilogoVociModel>();

            try
            {

                using (dtEvoluzioneIndennita dtei = new dtEvoluzioneIndennita())
                {
                    eim = dtei.GetSaldoIndennitaSistemazioneEvoluzione(idTrasferimento).ToList();

                }

                ViewBag.idTrasferimento = idTrasferimento;
                return PartialView(eim);

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });

            }



        }
        public ActionResult RptIndennitadiSistemazioneLorda(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();
            List<RptAnticipoIndennitàdiSistemazione_Lorda> rpt = new List<RptAnticipoIndennitàdiSistemazione_Lorda>();



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

                            var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                            var indennita = trasferimento.TIPOTRASFERIMENTO.INDENNITASISTEMAZIONE;


                            using (dtEvoluzioneIndennita dtei = new dtEvoluzioneIndennita())
                            {
                                eim = dtei.GetSaldoIndennitaSistemazioneEvoluzione(idTrasferimento).ToList();

                            }


                            if (eim?.Any() ?? false)
                            {
                                foreach (var lm in eim)
                                {
                                    RptAnticipoIndennitàdiSistemazione_Lorda rptds = new RptAnticipoIndennitàdiSistemazione_Lorda()
                                    {
                                        dataInizioValidita = lm.dataInizioValidita,
                                        dataFineValidita = lm.dataFineValidita,
                                        IndennitaServizio = lm.IndennitaServizio,
                                        AliquotaFiscale = lm.AliquotaFiscale,
                                        AliquotaPrevid = lm.AliquotaPrevid,
                                        RitenutaFiscale = lm.RitenutaFiscale,
                                        ImpFiscale = lm.ImpFiscale,
                                        ContrPrevid = lm.ContrPrevid,
                                        ImpPrevid = lm.ImpPrevid,
                                        Detrazione = lm.Detrazione,
                                        PercentualeRiduzionePrimaSistemazione = lm.PercentualeRiduzionePrimaSistemazione,
                                        CoeffIndSistemazione = lm.CoeffIndSistemazione,
                                        IndennitaSistemazione = lm.IndennitaSistemazione,
                                        IndennitaSistemazioneAnticipabileLorda = lm.IndennitaSistemazioneAnticipabileLorda,
                                        PercentualeAnticipoRichiesto = lm.PercentualeAnticipoRichiesto,
                                        Importo = lm.Importo,
                                        CoefficientediMaggiorazione = lm.CoefficientediMaggiorazione,
                                        TotaleMaggiorazioniFamiliari = lm.TotaleMaggiorazioniFamiliari,
                                        MaggiorazioniFigli = lm.MaggiorazioniFigli,
                                        MaggiorazioneConiuge = lm.MaggiorazioneConiuge,
                                        PercentualeMaggiorazioniFigli = lm.PercentualeMaggiorazioniFigli,
                                        PercentualeMaggConiuge = lm.PercentualeMaggConiuge,
                                        IndennitaPrimoSegretario = lm.IndennitaPrimoSegretario,
                                        IndennitaPersonale = lm.IndennitaPersonale,
                                        PercentualeDisagio = lm.PercentualeDisagio,
                                        IndennitaBase = lm.IndennitaBase,
                                        dataAnticipoSistemazione = lm.dataAnticipoSistemazione,
                                        anticipo = lm.anticipo,
                                        saldo = lm.saldo,
                                        totaleSaldoPrimaSistemazione = lm.totaleSaldoPrimaSistemazione



                                    };

                                    rpt.Add(rptds);

                                }
                            }


                            ReportViewer reportViewer = new ReportViewer();

                            reportViewer.ProcessingMode = ProcessingMode.Local;
                            reportViewer.SizeToReportContent = true;
                            reportViewer.Width = Unit.Percentage(100);
                            reportViewer.Height = Unit.Percentage(100);

                            var datasource = new ReportDataSource("DataSetAnticipoIndennitàdiSistemazioneLorda");

                            reportViewer.Visible = true;
                            reportViewer.ProcessingMode = ProcessingMode.Local;

                            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptIndennitadiSistemazioneLorda.rdlc";
                            reportViewer.LocalReport.DataSources.Clear();
                            reportViewer.LocalReport.DataSources.Add(datasource);
                            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetAnticipoIndennitàdiSistemazioneLorda", rpt));
                            reportViewer.LocalReport.Refresh();

                            List<ReportParameter> parameterValues = new List<ReportParameter>();
                            parameterValues.Add(new ReportParameter("Nominativo", Nominativo));
                            parameterValues.Add(new ReportParameter("Livello", Livello));
                            parameterValues.Add(new ReportParameter("Decorrenza", Decorrenza));
                            parameterValues.Add(new ReportParameter("Ufficio", Ufficio));


                            reportViewer.LocalReport.SetParameters(parameterValues);
                            ViewBag.ReportViewer = reportViewer;

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("RptIndennitadiSistemazioneLorda");
        }

        #endregion

        #region Unica Soluzione Indennita di Prima Sistemazione + Report di Stampa
        public ActionResult UnicaSoluzioneIndennitadiPrimaSistemazione(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {

                using (dtEvoluzioneIndennita dtei = new dtEvoluzioneIndennita())
                {
                    eim = dtei.GetUnicaSoluzioneIndennitaSistemazioneEvoluzione(idTrasferimento).ToList();

                }

                ViewBag.idTrasferimento = idTrasferimento;

                return PartialView(eim);

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }

        public ActionResult RptUnicaSoluzioneIndennitadiPrimaSistemazione(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();
            List<RptAnticipoIndennitàdiSistemazione_Lorda> rpt = new List<RptAnticipoIndennitàdiSistemazione_Lorda>();



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

                            var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                            var indennita = trasferimento.TIPOTRASFERIMENTO.INDENNITASISTEMAZIONE;


                            using (dtEvoluzioneIndennita dtei = new dtEvoluzioneIndennita())
                            {

                                eim = dtei.GetUnicaSoluzioneIndennitaSistemazioneEvoluzione(idTrasferimento).ToList();
                            }


                            if (eim?.Any() ?? false)
                            {
                                foreach (var lm in eim)
                                {
                                    RptAnticipoIndennitàdiSistemazione_Lorda rptds = new RptAnticipoIndennitàdiSistemazione_Lorda()
                                    {
                                        dataInizioValidita = lm.dataInizioValidita,
                                        dataFineValidita = lm.dataFineValidita,
                                        IndennitaServizio = lm.IndennitaServizio,
                                        AliquotaFiscale = lm.AliquotaFiscale,
                                        AliquotaPrevid = lm.AliquotaPrevid,
                                        RitenutaFiscale = lm.RitenutaFiscale,
                                        ImpFiscale = lm.ImpFiscale,
                                        ContrPrevid = lm.ContrPrevid,
                                        ImpPrevid = lm.ImpPrevid,
                                        Detrazione = lm.Detrazione,
                                        PercentualeRiduzionePrimaSistemazione = lm.PercentualeRiduzionePrimaSistemazione,
                                        CoeffIndSistemazione = lm.CoeffIndSistemazione,
                                        IndennitaSistemazione = lm.IndennitaSistemazione,
                                        IndennitaSistemazioneAnticipabileLorda = lm.IndennitaSistemazioneAnticipabileLorda,
                                        PercentualeAnticipoRichiesto = lm.PercentualeAnticipoRichiesto,
                                        Importo = lm.Importo,
                                        CoefficientediMaggiorazione = lm.CoefficientediMaggiorazione,
                                        TotaleMaggiorazioniFamiliari = lm.TotaleMaggiorazioniFamiliari,
                                        MaggiorazioniFigli = lm.MaggiorazioniFigli,
                                        MaggiorazioneConiuge = lm.MaggiorazioneConiuge,
                                        PercentualeMaggiorazioniFigli = lm.PercentualeMaggiorazioniFigli,
                                        PercentualeMaggConiuge = lm.PercentualeMaggConiuge,
                                        IndennitaPrimoSegretario = lm.IndennitaPrimoSegretario,
                                        IndennitaPersonale = lm.IndennitaPersonale,
                                        PercentualeDisagio = lm.PercentualeDisagio,
                                        IndennitaBase = lm.IndennitaBase,
                                        dataAnticipoSistemazione = lm.dataAnticipoSistemazione

                                    };

                                    rpt.Add(rptds);

                                }
                            }


                            ReportViewer reportViewer = new ReportViewer();

                            reportViewer.ProcessingMode = ProcessingMode.Local;
                            reportViewer.SizeToReportContent = true;
                            reportViewer.Width = Unit.Percentage(100);
                            reportViewer.Height = Unit.Percentage(100);

                            var datasource = new ReportDataSource("DataSetAnticipoIndennitàdiSistemazioneLorda");

                            reportViewer.Visible = true;
                            reportViewer.ProcessingMode = ProcessingMode.Local;

                            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptUnicaSoluzioneIndennitadiPrimaSistemazione.rdlc";
                            reportViewer.LocalReport.DataSources.Clear();
                            reportViewer.LocalReport.DataSources.Add(datasource);
                            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetAnticipoIndennitàdiSistemazioneLorda", rpt));
                            reportViewer.LocalReport.Refresh();

                            List<ReportParameter> parameterValues = new List<ReportParameter>();
                            parameterValues.Add(new ReportParameter("Nominativo", Nominativo));
                            parameterValues.Add(new ReportParameter("Livello", Livello));
                            parameterValues.Add(new ReportParameter("Decorrenza", Decorrenza));
                            parameterValues.Add(new ReportParameter("Ufficio", Ufficio));


                            reportViewer.LocalReport.SetParameters(parameterValues);
                            ViewBag.ReportViewer = reportViewer;

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("RptUnicaSoluzioneIndennitadiPrimaSistemazione");
        }

        public ActionResult RptIndennitadiSistemazioneNetta(decimal idTrasferimento)
        {

            try
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

                        ReportViewer reportViewer = new ReportViewer();

                        reportViewer.ProcessingMode = ProcessingMode.Local;
                        reportViewer.SizeToReportContent = true;
                        reportViewer.Width = Unit.Percentage(100);
                        reportViewer.Height = Unit.Percentage(100);

                        //var datasource = new ReportDataSource("DSRiepilogoVoci", lTeorici.ToList());
                        reportViewer.Visible = true;
                        reportViewer.ProcessingMode = ProcessingMode.Local;
                        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptIndennitadiSistemazioneNetta.rdlc";
                        reportViewer.LocalReport.DataSources.Clear();
                        //reportViewer.LocalReport.DataSources.Add(datasource);

                        reportViewer.LocalReport.Refresh();
                        reportViewer.ShowReportBody = true;

                        ReportParameter[] parameterValues = new ReportParameter[]
                        {
                            new ReportParameter ("Nominativo",Nominativo),
                            new ReportParameter ("Livello",Livello),
                            new ReportParameter ("Decorrenza",Decorrenza),
                            new ReportParameter ("Ufficio",Ufficio)

                        };

                        reportViewer.LocalReport.SetParameters(parameterValues);
                        ViewBag.ReportViewer = reportViewer;

                    }
                }


            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("RptIndennitadiSistemazioneNetta");
        }

        #endregion

        #region Contributo Omnicomprensivo Trasferimento + Report di Stampa
        public ActionResult ContributoOmnicomprensivoTrasferimento(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {

                using (dtEvoluzioneIndennita dtei = new dtEvoluzioneIndennita())
                {
                    eim = dtei.GetContrOmnicomprensivoTrasfEvoluzione(idTrasferimento).ToList();

                }

                ViewBag.idTrasferimento = idTrasferimento;
                return PartialView(eim);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }
        public ActionResult RptContributoOmnicomprensivoTrasferimento(decimal idTrasferimento)
        {
            List<RptContributoOmnicomprensivoTrasferimentoModel> rpt = new List<RptContributoOmnicomprensivoTrasferimentoModel>();

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


                            var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);

                            using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO))
                            {

                                RptContributoOmnicomprensivoTrasferimentoModel rpts = new RptContributoOmnicomprensivoTrasferimentoModel()
                                {
                                    IndennitaSistemazioneLorda = ci.IndennitaSistemazioneAnticipabileLorda,
                                    AnticipoContrOmniComprensivoPartenza = ci.AnticipoContributoOmnicomprensivoPartenza,
                                    SaldoContrOmniComprensivoPartenza = ci.SaldoContributoOmnicomprensivoPartenza,
                                    PercentualeFasciaKmP = ci.PercentualeFKMPartenza,
                                    dataPartenza = trasferimento.DATAPARTENZA,
                                    TotaleContributoOmnicomprensivoPartenza = ci.TotaleContributoOmnicomprensivoPartenza

                                };

                                rpt.Add(rpts);

                            }


                            ReportViewer reportViewer = new ReportViewer();

                            reportViewer.ProcessingMode = ProcessingMode.Local;
                            reportViewer.SizeToReportContent = true;
                            reportViewer.Width = Unit.Percentage(100);
                            reportViewer.Height = Unit.Percentage(100);

                            var datasource = new ReportDataSource("DataSetContributoOmnicomprensivoTrasferimento");

                            reportViewer.Visible = true;
                            reportViewer.ProcessingMode = ProcessingMode.Local;

                            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptContributoOmnicomprensivoTrasferimento.rdlc";
                            reportViewer.LocalReport.DataSources.Clear();
                            reportViewer.LocalReport.DataSources.Add(datasource);
                            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetContributoOmnicomprensivoTrasferimento", rpt));
                            reportViewer.LocalReport.Refresh();

                            List<ReportParameter> parameterValues = new List<ReportParameter>();
                            parameterValues.Add(new ReportParameter("Nominativo", Nominativo));
                            parameterValues.Add(new ReportParameter("Livello", Livello));
                            parameterValues.Add(new ReportParameter("Decorrenza", Decorrenza));
                            parameterValues.Add(new ReportParameter("Ufficio", Ufficio));


                            reportViewer.LocalReport.SetParameters(parameterValues);
                            ViewBag.ReportViewer = reportViewer;

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("RptContributoOmnicomprensivoTrasferimento");
        }

        #endregion

        #region Contributo Omnicomprensivo Rientro + Report di Stampa
        public ActionResult ContributoOmnicomprensivoRientro(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {
                using (dtEvoluzioneIndennita dtei = new dtEvoluzioneIndennita())
                {
                    eim = dtei.GetContrOmnicomprensivoRientroEvoluzione(idTrasferimento).ToList();

                }


                ViewBag.idTrasferimento = idTrasferimento;
                return PartialView(eim);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }
        public ActionResult RptContributoOmnicomprensivoRientro(decimal idTrasferimento)
        {
            List<RptContributoOmnicomprensivoRientroModel> rpt = new List<RptContributoOmnicomprensivoRientroModel>();

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


                            var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);

                            using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO))
                            {

                                RptContributoOmnicomprensivoRientroModel rpts = new RptContributoOmnicomprensivoRientroModel()
                                {
                                    //dataRientro = trasferimento.DATARIENTRO,
                                    dtRientro = (trasferimento.DATARIENTRO < Utility.DataFineStop()) ? Convert.ToDateTime(trasferimento.DATARIENTRO).ToShortDateString() : null,
                                    IndennitaRichiamo = ci.IndennitaRichiamoLordo,
                                    AnticipoContrOmniComprensivoRientro = ci.AnticipoContributoOmnicomprensivoRientro,
                                    SaldoContrOmniComprensivoRientro = ci.SaldoContributoOmnicomprensivoRientro,
                                    PercentualeFasciaKmR = ci.PercentualeFKMRientro,
                                    TotaleContributoOmnicomprensivoRientro = ci.TotaleContributoOmnicomprensivoRientro

                                };

                                rpt.Add(rpts);

                            }

                            ReportViewer reportViewer = new ReportViewer();

                            reportViewer.ProcessingMode = ProcessingMode.Local;
                            reportViewer.SizeToReportContent = true;
                            reportViewer.Width = Unit.Percentage(100);
                            reportViewer.Height = Unit.Percentage(100);

                            var datasource = new ReportDataSource("DataSetContributoOmnicomprensivoRientro");

                            reportViewer.Visible = true;
                            reportViewer.ProcessingMode = ProcessingMode.Local;

                            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptContributoOmnicomprensivoRientro.rdlc";
                            reportViewer.LocalReport.DataSources.Clear();
                            reportViewer.LocalReport.DataSources.Add(datasource);
                            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetContributoOmnicomprensivoRientro", rpt));
                            reportViewer.LocalReport.Refresh();

                            List<ReportParameter> parameterValues = new List<ReportParameter>();
                            parameterValues.Add(new ReportParameter("Nominativo", Nominativo));
                            parameterValues.Add(new ReportParameter("Livello", Livello));
                            parameterValues.Add(new ReportParameter("Decorrenza", Decorrenza));
                            parameterValues.Add(new ReportParameter("Ufficio", Ufficio));


                            reportViewer.LocalReport.SetParameters(parameterValues);
                            ViewBag.ReportViewer = reportViewer;

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("RptContributoOmnicomprensivoRientro");
        }

        #endregion


    }
}
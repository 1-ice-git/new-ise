
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
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();
            dipInfoTrasferimentoModel dit = new dipInfoTrasferimentoModel();
            List<LivelloDipendenteModel> lldm = new List<LivelloDipendenteModel>();
            

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
                        tm.RuoloUfficio.DescrizioneRuolo = tm.RuoloUfficio.DescrizioneRuolo;
                        ViewBag.idRuoloUfficio = tm.idRuoloUfficio;
                        ViewBag.RuoloDipendente = tm.RuoloUfficio.DescrizioneRuolo;

                    }

                    using (dtLivelliDipendente dtld = new dtLivelliDipendente())
                    {
                        lldm = dtld.GetLivelloDipendenteByIdTrasferimento(idTrasferimento).ToList();
                        

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

        DataSetIndennitaBase ds = new DataSetIndennitaBase();
        public ActionResult RptIndennitaBase(decimal idTrasferimento)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();
            List<LivelloDipendenteModel> ldm = new List<LivelloDipendenteModel>();
            List<RptIndennitaBaseModel> rpt = new List<RptIndennitaBaseModel>();

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
                                string Ufficio = tm.Ufficio.descUfficio;

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

                                

                                var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                                var indennita = trasferimento.INDENNITA;

                                List<DateTime> lDateVariazioni = new List<DateTime>();

                                var ll1 =
                                    db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.INDENNITABASE
                                    .Where(a => a.ANNULLATO == false)
                                    .OrderBy(a => a.IDLIVELLO)
                                    .ThenBy(a => a.DATAINIZIOVALIDITA)
                                    .ThenBy(a => a.DATAFINEVALIDITA)
                                    .ToList();


                                using (dtTrasferimento dttrasf = new dtTrasferimento())
                                {
                                    using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                                    {
                                        RuoloDipendenteModel rdm = dtrd.GetRuoloDipendenteByIdIndennita(idTrasferimento);
                                        dipInfoTrasferimentoModel dipInfoTrasf = dttrasf.GetInfoTrasferimento(idTrasferimento);
                                        
                                    }
                                }

                                string[] indennitaBase = new string[100];
                                string[] dataInizioValidita = new string[100];
                                string[] dataFineValidita = new string[100];

                                foreach (var ib in ll1)
                                {
                                    DateTime dtVar = new DateTime();

                                    if (ib.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                    {
                                        dtVar = trasferimento.DATAPARTENZA;
                                    }
                                    else
                                    {
                                        dtVar = ib.DATAINIZIOVALIDITA;
                                    }


                                    if (!lDateVariazioni.Contains(dtVar))
                                    {
                                        lDateVariazioni.Add(dtVar);
                                        lDateVariazioni.Sort();
                                    }
                                }

                                lDateVariazioni.Add(new DateTime(9999, 12, 31));

                                if (lDateVariazioni?.Any() ?? false)
                                {
                                    for (int j = 0; j < lDateVariazioni.Count; j++)
                                    {
                                        DateTime dv = lDateVariazioni[j];

                                        if (dv < Utility.DataFineStop())
                                        {
                                            DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);
                                            //DateTime dvSucc = lDateVariazioni[(j + 1)];

                                            //if (lDateVariazioni[j + 1] == Utility.DataFineStop())
                                            //{
                                            //    dvSucc = lDateVariazioni[j + 1];
                                            //}

                                            if (lDateVariazioni[j + 1] == Utility.DataFineStop())
                                            {
                                                dvSucc = lDateVariazioni[j + 1];
                                            }

                                            using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                            {
                                                RptIndennitaBaseModel rpts = new RptIndennitaBaseModel()
                                                {

                                                    IndennitaBase = ci.IndennitaDiBase,
                                                    DataInizioValidita = Convert.ToDateTime(dv).ToShortDateString(),
                                                    //DataFineValidita = Convert.ToDateTime(dvSucc).ToShortDateString()
                                                    DataFineValidita = (dvSucc < Utility.DataFineStop()) ? Convert.ToDateTime(dvSucc).ToShortDateString() : null,
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

                                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptIndennitaBase.rdlc";
                                reportViewer.LocalReport.DataSources.Clear();
                                reportViewer.LocalReport.DataSources.Add(datasource);
                                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetIndennitaBase", rpt));
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

            return PartialView("RptIndennitaBase");

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

        DataSetIndennitaServizio DataSetIndennitaServizio = new DataSetIndennitaServizio();
        public ActionResult RptIndennitaServizio(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();
            List<LivelloDipendenteModel> ldm = new List<LivelloDipendenteModel>();
            List<RptIndennitaServizioModel> rpt = new List<RptIndennitaServizioModel>();


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
                            var indennita = trasferimento.INDENNITA;

                            List<DateTime> lDateVariazioni = new List<DateTime>();

                            #region Variazioni di indennità di base

                            var ll =
                                db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.INDENNITABASE
                                .Where(a => a.ANNULLATO == false).OrderBy(a => a.IDLIVELLO)
                                    .ThenBy(a => a.DATAINIZIOVALIDITA)
                                    .ThenBy(a => a.DATAFINEVALIDITA).ToList();


                            foreach (var ib in ll)
                            {
                                DateTime dtVar = new DateTime();

                                if (ib.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                {
                                    dtVar = trasferimento.DATAPARTENZA;
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
                                db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.COEFFICIENTESEDE
                                .Where(a => a.ANNULLATO == false)
                                .OrderBy(a => a.IDCOEFFICIENTESEDE)
                                .ThenBy(a => a.DATAINIZIOVALIDITA)
                                .ThenBy(a => a.DATAFINEVALIDITA).ToList();

                            foreach (var cs in lrd)
                            {
                                DateTime dtVar = new DateTime();

                                if (cs.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                {
                                    dtVar = trasferimento.DATAPARTENZA;
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
                                db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.PERCENTUALEDISAGIO
                                .Where(a => a.ANNULLATO == false)
                                .OrderBy(a => a.IDPERCENTUALEDISAGIO)
                                .ThenBy(a => a.DATAINIZIOVALIDITA)
                                .ThenBy(a => a.DATAFINEVALIDITA).ToList();


                            foreach (var pd in perc)
                            {
                                DateTime dtVar = new DateTime();

                                if (pd.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                {
                                    dtVar = trasferimento.DATAPARTENZA;
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

                            lDateVariazioni.Add(new DateTime(9999, 12, 31));

                            if (lDateVariazioni?.Any() ?? false)
                            {
                                for (int j = 0; j < lDateVariazioni.Count; j++)
                                {
                                    DateTime dv = lDateVariazioni[j];

                                    if (dv < Utility.DataFineStop())
                                    {
                                        DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);
                                        //DateTime dvSucc = lDateVariazioni[(j + 1)];

                                        if (lDateVariazioni[j + 1] == Utility.DataFineStop())
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
                                                PercentualeDisagio =ci.PercentualeDisagio,
                                                IndennitaServizio =ci.IndennitaDiServizio
                                                
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

                            var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                            //using (dtConiuge dtc = new dtConiuge())
                            //{
                            //    var cm = dtc.GetConiugebyID(coniuge.IDCONIUGE);


                            //    DateTime dtIni = cm.dataInizio.Value;
                            //    DateTime dtFin = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop();
                            //}


                            #region Variazioni Coniuge
                            var lc =
                                mf.CONIUGE.Where(
                                    a =>
                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
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

                                    //if (!lDateVariazioni.Contains(dtVar))
                                    //{
                                    //    lDateVariazioni.Add(dtVar);
                                    //    //lDateVariazioni.Sort();
                                    //}

                                    //if (c.DATAFINEVALIDITA < trasferimento.DATARIENTRO)
                                    //{
                                    //    dtVar = c.DATAFINEVALIDITA;
                                    //}
                                    //else
                                    //{
                                    //    dtVar = trasferimento.DATARIENTRO;
                                    //}

                                    if (!lDateVariazioni.Contains(dtVar))
                                    {
                                        lDateVariazioni.Add(dtVar);
                                        //lDateVariazioni.Sort();
                                    }


                                }
                            }
                            #endregion

                            if (lc?.Any() ?? false)
                            {
                                #region Variazione Percentuale Maggiorazione Coniuge
                                foreach (var c in lc)
                                {
                                    var coniuge = lc.First();

                                    var lpmc =
                                           coniuge.PERCENTUALEMAGCONIUGE.Where(
                                               a =>
                                                   a.ANNULLATO == false).ToList();
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
                                        //lDateVariazioni.Sort();
                                    }

                                }
                                #endregion
                            }
                            if (lc?.Any() ?? false)
                            {
                                #region Variazione Pensione
                                foreach (var p in lc)
                                {
                                    DateTime dtVar = new DateTime();

                                    var pensione = p.PENSIONE.Where(a =>
                                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                                a.NASCONDI == false).ToList();

                                    if (p.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                    {
                                        dtVar = trasferimento.DATAPARTENZA;
                                    }
                                    else
                                    {
                                        dtVar = p.DATAINIZIOVALIDITA;
                                    }
                                    if (!lDateVariazioni.Contains(dtVar))
                                    {
                                        lDateVariazioni.Add(dtVar);
                                        //lDateVariazioni.Sort();
                                    }
                                }
                                #endregion
                            }

                            if (lc?.Any() ?? false)
                            {
                                foreach (var fc in lc)
                                {

                                    DateTime dtVar = new DateTime();

                                    //if (c.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                    //{
                                    //    dtVar = trasferimento.DATAPARTENZA;
                                    //}
                                    //else
                                    //{
                                    //    dtVar = c.DATAINIZIOVALIDITA;
                                    //}

                                    //if (!lDateVariazioni.Contains(dtVar))
                                    //{
                                    //    lDateVariazioni.Add(dtVar);
                                    //    //lDateVariazioni.Sort();
                                    //}

                                    if (fc.DATAFINEVALIDITA < trasferimento.DATARIENTRO)
                                    {
                                        dtVar = fc.DATAFINEVALIDITA.AddDays(1);
                                    }
                                    else
                                    {
                                        dtVar = trasferimento.DATARIENTRO;
                                    }

                                    if (!lDateVariazioni.Contains(dtVar))
                                    {
                                        lDateVariazioni.Add(dtVar);

                                    }


                                }
                            }

                            #region Variazioni di indennità di base

                            var ll =
                                db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.INDENNITABASE
                                .Where(a => a.ANNULLATO == false &&
                                        a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                        a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                            foreach (var ib in ll)
                            {
                                DateTime dtVar = new DateTime();

                                if (ib.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                {
                                    dtVar = trasferimento.DATAPARTENZA;
                                }
                                else
                                {
                                    dtVar = ib.DATAINIZIOVALIDITA;
                                }


                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                    //lDateVariazioni.Sort();
                                }
                            }

                            #endregion

                            #region Variazioni del coefficiente di sede

                            var lrd =
                                db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.COEFFICIENTESEDE
                                .Where(a => a.ANNULLATO == false &&
                                        a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                        a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                            foreach (var cs in lrd)
                            {
                                DateTime dtVar = new DateTime();

                                if (cs.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                {
                                    dtVar = trasferimento.DATAPARTENZA;
                                }
                                else
                                {
                                    dtVar = cs.DATAINIZIOVALIDITA;
                                }

                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                    //lDateVariazioni.Sort();
                                }
                            }

                            #endregion

                            #region Variazioni percentuale di disagio

                            var perc =
                                db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.PERCENTUALEDISAGIO
                                .Where(a => a.ANNULLATO == false &&
                                        a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                        a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                            foreach (var pd in perc)
                            {
                                DateTime dtVar = new DateTime();

                                if (pd.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                {
                                    dtVar = trasferimento.DATAPARTENZA;
                                }
                                else
                                {
                                    dtVar = pd.DATAINIZIOVALIDITA;
                                }

                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                    //lDateVariazioni.Sort();
                                }
                            }

                            #endregion

                            #region Variazioni Livelli

                            var llliv =
                                trasferimento.INDENNITA.LIVELLIDIPENDENTI
                                        .Where(a => a.ANNULLATO == false &&
                                        a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                        a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                        .OrderBy(a => a.IDLIVELLO)
                                        .ThenBy(a => a.DATAINIZIOVALIDITA)
                                        .ThenBy(a => a.DATAFINEVALIDITA)
                                        .ToList();



                            foreach (var liv2 in llliv)
                            {
                                DateTime dtVar = new DateTime();

                                if (liv2.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                {
                                    dtVar = trasferimento.DATAPARTENZA;
                                }
                                else
                                {
                                    dtVar = liv2.DATAINIZIOVALIDITA;
                                }


                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                    //lDateVariazioni.Sort();
                                }
                            }

                            #endregion


                            lDateVariazioni.Add(new DateTime(9999, 12, 31));

                            lDateVariazioni = lDateVariazioni.OrderBy(a => a.Year).ThenBy(a => a.Month).ThenBy(a => a.Day).ToList();

                            if (lDateVariazioni?.Any() ?? false)
                            {
                                for (int j = 0; j < lDateVariazioni.Count; j++)
                                {
                                    DateTime dv = lDateVariazioni[j];

                                    if (dv < Utility.DataFineStop())
                                    {
                                        DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);
                                       
                                        if (lDateVariazioni[j + 1] == Utility.DataFineStop())
                                        {
                                            dvSucc = lDateVariazioni[j + 1];
                                        }

                                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                        {
                                            RptMaggiorazioniConiuge rpts = new RptMaggiorazioniConiuge()
                                            {
                                                DataInizioValidita = Convert.ToDateTime(dv).ToShortDateString(),
                                                //DataFineValidita = Convert.ToDateTime(dvSucc).ToShortDateString(),
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
        public ActionResult RptMaggiorazioniFigli(decimal idTrasferimento)
        {
            List<RptMaggiorazioniFigli> rpt = new List<RptMaggiorazioniFigli>();

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
                            var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                            #region Variazioni Maggiorazioni Figli
                            var lf =
                                        mf.FIGLI.Where(
                                            a =>
                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
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



                                }
                            }


                            #endregion

                            #region Variazioni Percentuale Maggiorazioni Figli
                            if (lf?.Any() ?? false)
                            {

                                foreach (var f in lf)
                                {

                                    var lpmf =
                                        f.PERCENTUALEMAGFIGLI.Where(
                                            a =>
                                                a.ANNULLATO == false).ToList();

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
                                        //lDateVariazioni.Sort();
                                    }
                                }

                            }
                            #endregion

                            #region Variazione Indennità Primo Segretario
                            if (lf?.Any() ?? false)
                            {

                                foreach (var f in lf)
                                {

                                    var lips =
                                                f.INDENNITAPRIMOSEGRETARIO.Where(
                                                    a =>
                                                        a.ANNULLATO == false).ToList();

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
                                        //lDateVariazioni.Sort();
                                    }



                                }

                            }
                            #endregion

                            if (lf?.Any() ?? false)
                            {
                                foreach (var ff in lf)
                                {

                                    DateTime dtVar = new DateTime();



                                    if (ff.DATAFINEVALIDITA < trasferimento.DATARIENTRO)
                                    {
                                        dtVar = ff.DATAFINEVALIDITA.AddDays(1);
                                    }
                                    else
                                    {
                                        dtVar = trasferimento.DATARIENTRO;
                                    }

                                    if (!lDateVariazioni.Contains(dtVar))
                                    {
                                        lDateVariazioni.Add(dtVar);

                                    }


                                }
                            }

                            #region Variazioni di indennità di base

                            var ll =
                                db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.INDENNITABASE
                                .Where(a => a.ANNULLATO == false &&
                                        a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                        a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                            foreach (var ib in ll)
                            {
                                DateTime dtVar = new DateTime();

                                if (ib.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                {
                                    dtVar = trasferimento.DATAPARTENZA;
                                }
                                else
                                {
                                    dtVar = ib.DATAINIZIOVALIDITA;
                                }


                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                    //lDateVariazioni.Sort();
                                }
                            }

                            #endregion

                            #region Variazioni del coefficiente di sede

                            var lrd =
                                db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.COEFFICIENTESEDE
                                .Where(a => a.ANNULLATO == false &&
                                        a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                        a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                            foreach (var cs in lrd)
                            {
                                DateTime dtVar = new DateTime();

                                if (cs.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                {
                                    dtVar = trasferimento.DATAPARTENZA;
                                }
                                else
                                {
                                    dtVar = cs.DATAINIZIOVALIDITA;
                                }

                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                    //lDateVariazioni.Sort();
                                }
                            }

                            #endregion

                            #region Variazioni percentuale di disagio

                            var perc =
                                db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.PERCENTUALEDISAGIO
                                .Where(a => a.ANNULLATO == false &&
                                        a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                        a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                            foreach (var pd in perc)
                            {
                                DateTime dtVar = new DateTime();

                                if (pd.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                {
                                    dtVar = trasferimento.DATAPARTENZA;
                                }
                                else
                                {
                                    dtVar = pd.DATAINIZIOVALIDITA;
                                }

                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                    //lDateVariazioni.Sort();
                                }
                            }

                            #endregion

                            #region Variazioni Livelli

                            var llliv =
                                trasferimento.INDENNITA.LIVELLIDIPENDENTI
                                        .Where(a => a.ANNULLATO == false &&
                                        a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                        a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                        .OrderBy(a => a.IDLIVELLO)
                                        .ThenBy(a => a.DATAINIZIOVALIDITA)
                                        .ThenBy(a => a.DATAFINEVALIDITA)
                                        .ToList();



                            foreach (var liv2 in llliv)
                            {
                                DateTime dtVar = new DateTime();

                                if (liv2.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                {
                                    dtVar = trasferimento.DATAPARTENZA;
                                }
                                else
                                {
                                    dtVar = liv2.DATAINIZIOVALIDITA;
                                }


                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                    //lDateVariazioni.Sort();
                                }
                            }

                            #endregion

                            lDateVariazioni.Add(new DateTime(9999, 12, 31));

                            lDateVariazioni = lDateVariazioni.OrderBy(a => a.Year).ThenBy(a => a.Month).ThenBy(a => a.Day).ToList();

                            if (lDateVariazioni?.Any() ?? false)
                            {
                                    for (int j = 0; j < lDateVariazioni.Count; j++)
                                    {
                                        DateTime dv = lDateVariazioni[j];

                                        if (dv < Utility.DataFineStop())
                                        {
                                        DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                        if (lDateVariazioni[j + 1] == Utility.DataFineStop())
                                        {
                                            dvSucc = lDateVariazioni[j + 1];
                                        }

                                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                            {

                                                RptMaggiorazioniFigli rpts = new RptMaggiorazioniFigli()
                                                {

                                                    DataInizioValidita = Convert.ToDateTime(dv).ToShortDateString(),
                                                    //DataFineValidita = Convert.ToDateTime(dvSucc).ToShortDateString(),
                                                    DataFineValidita = (dvSucc < Utility.DataFineStop())? Convert.ToDateTime(dvSucc).ToShortDateString(): null,
                                                    IndennitaServizioPrimoSegretario = ci.IndennitaServizioPrimoSegretario,
                                                    PercentualeMaggiorazioniFigli = ci.PercentualeMaggiorazioneFigli,
                                                    MaggiorazioniFigli = ci.MaggiorazioneFigli

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
                
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView("RptMaggiorazioniFigli");
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
                    eim = dtei.GetIndennitaPersonaleEvoluzione(idTrasferimento).ToList();

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
                            var indennita = trasferimento.INDENNITA;
                            var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                            List<DateTime> lDateVariazioni = new List<DateTime>();

                            #region Variazioni di indennità di base

                            var ll =
                                db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.INDENNITABASE
                                .Where(a => a.ANNULLATO == false &&
                                        a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                        a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                            foreach (var ib in ll)
                            {
                                DateTime dtVar = new DateTime();

                                if (ib.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                {
                                    dtVar = trasferimento.DATAPARTENZA;
                                }
                                else
                                {
                                    dtVar = ib.DATAINIZIOVALIDITA;
                                }


                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                    lDateVariazioni.Sort();
                                }
                            }

                            #endregion

                            #region Variazioni del coefficiente di sede

                            var lrd =
                                db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.COEFFICIENTESEDE
                                .Where(a => a.ANNULLATO == false &&
                                        a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                        a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                            foreach (var cs in lrd)
                            {
                                DateTime dtVar = new DateTime();

                                if (cs.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                {
                                    dtVar = trasferimento.DATAPARTENZA;
                                }
                                else
                                {
                                    dtVar = cs.DATAINIZIOVALIDITA;
                                }

                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                    lDateVariazioni.Sort();
                                }
                            }

                            #endregion

                            #region Variazioni percentuale di disagio

                            var perc =
                                db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.PERCENTUALEDISAGIO
                                .Where(a => a.ANNULLATO == false &&
                                        a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                        a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                            foreach (var pd in perc)
                            {
                                DateTime dtVar = new DateTime();

                                if (pd.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                {
                                    dtVar = trasferimento.DATAPARTENZA;
                                }
                                else
                                {
                                    dtVar = pd.DATAINIZIOVALIDITA;
                                }

                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                    lDateVariazioni.Sort();
                                }
                            }

                            #endregion

                            #region Variazioni Livelli

                            var llliv =
                                trasferimento.INDENNITA.LIVELLIDIPENDENTI
                                        .Where(a => a.ANNULLATO == false &&
                                        a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                        a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                        .OrderBy(a => a.IDLIVELLO)
                                        .ThenBy(a => a.DATAINIZIOVALIDITA)
                                        .ThenBy(a => a.DATAFINEVALIDITA)
                                        .ToList();



                            foreach (var liv2 in llliv)
                            {
                                DateTime dtVar = new DateTime();

                                if (liv2.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                {
                                    dtVar = trasferimento.DATAPARTENZA;
                                }
                                else
                                {
                                    dtVar = liv2.DATAINIZIOVALIDITA;
                                }


                                if (!lDateVariazioni.Contains(dtVar))
                                {
                                    lDateVariazioni.Add(dtVar);
                                    lDateVariazioni.Sort();
                                }
                            }

                            #endregion

                            #region Variazioni Coniuge
                            var lf =
                                mf.CONIUGE.Where(
                                    a =>
                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
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



                                    if (c.DATAFINEVALIDITA < trasferimento.DATARIENTRO)
                                    {
                                        dtVar = c.DATAFINEVALIDITA.AddDays(+1);

                                    }
                                    else
                                    {
                                        dtVar = trasferimento.DATARIENTRO;
                                    }

                                    if (!lDateVariazioni.Contains(dtVar))
                                    {
                                        lDateVariazioni.Add(dtVar);
                                        lDateVariazioni.Sort();
                                    }

                                }
                            }

                            #endregion

                            #region Variazioni Figli
                            var lf1 =
                                        mf.FIGLI.Where(
                                            a =>
                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
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


                                    if (f.DATAFINEVALIDITA < trasferimento.DATARIENTRO)
                                    {
                                        dtVar = f.DATAFINEVALIDITA.AddDays(+1);

                                    }
                                    else
                                    {
                                        dtVar = trasferimento.DATARIENTRO;
                                    }

                                    if (!lDateVariazioni.Contains(dtVar))
                                    {
                                        lDateVariazioni.Add(dtVar);
                                        lDateVariazioni.Sort();
                                    }

                                }
                            }
                            #endregion

                            lDateVariazioni.Add(new DateTime(9999, 12, 31));

                            if (lDateVariazioni?.Any() ?? false)
                            {
                                for (int j = 0; j < lDateVariazioni.Count; j++)
                                {
                                    DateTime dv = lDateVariazioni[j];

                                    if (dv < Utility.DataFineStop())
                                    {
                                        DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                        if (lDateVariazioni[j + 1] == Utility.DataFineStop())
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



                            //ReportViewer reportViewer = new ReportViewer();

                            //reportViewer.ProcessingMode = ProcessingMode.Local;
                            //reportViewer.SizeToReportContent = true;
                            //reportViewer.Width = Unit.Percentage(100);
                            //reportViewer.Height = Unit.Percentage(100);

                            ////var datasource = new ReportDataSource("DSRiepilogoVoci", lTeorici.ToList());
                            //reportViewer.Visible = true;
                            //reportViewer.ProcessingMode = ProcessingMode.Local;
                            //reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptIndennitaPersonale.rdlc";
                            //reportViewer.LocalReport.DataSources.Clear();
                            ////reportViewer.LocalReport.DataSources.Add(datasource);

                            //reportViewer.LocalReport.Refresh();
                            //reportViewer.ShowReportBody = true;

                            //ReportParameter[] parameterValues = new ReportParameter[]
                            //{
                            //    new ReportParameter ("Nominativo",Nominativo),
                            //    new ReportParameter ("Livello",Livello),
                            //    new ReportParameter ("Decorrenza",Decorrenza),
                            //    new ReportParameter ("Ufficio",Ufficio)

                            //};

                            //reportViewer.LocalReport.SetParameters(parameterValues);
                            //ViewBag.ReportViewer = reportViewer;

                        }
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

                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var tm = dtt.GetTrasferimentoById(idTrasferimento);

                        List<DateTime> lDateVariazioni = new List<DateTime>();


                        //var lmab = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.MAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();

                        using (dtLivelliDipendente dld = new dtLivelliDipendente())
                        {
                            ViewBag.idTrasferimento = idTrasferimento;

                            var liv = dld.GetLivelloDipendenteByIdTrasferimento(idTrasferimento);
                            var liv1 = liv.First();

                            string Nominativo = tm.Dipendente.Nominativo;
                            string Decorrenza = Convert.ToDateTime(tm.dataPartenza).ToShortDateString();
                            string Livello = liv1.Livello.DescLivello;
                            string Ufficio = tm.Ufficio.descUfficio;

                            var lmab = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.MAB
                                      .Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.RINUNCIAMAB == false)
                                      .OrderBy(a => a.IDMAB)
                                       .ToList();

                            if (lmab?.Any() ?? false)
                            {

                                foreach (var mab in lmab)
                                {
                                    //var perMab =
                                    //    mab.PERIODOMAB.Where(
                                    //        a =>
                                    //        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                    //        .OrderByDescending(a => a.IDPERIODOMAB)
                                    //   .First();

                                    //var lpmab =
                                    //   perMab.PERCENTUALEMAB.Where(
                                    //       a =>
                                    //       a.ANNULLATO == false)
                                    //   .ToList();

                                    //    foreach (var cz in lpmab)
                                    //    {
                                    //        DateTime dtVar = new DateTime();

                                    //        if (cz.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                    //        {
                                    //            dtVar = trasferimento.DATAPARTENZA;
                                    //        }
                                    //        else
                                    //        {
                                    //            dtVar = cz.DATAINIZIOVALIDITA;
                                    //        }

                                    //        if (!lDateVariazioni.Contains(dtVar))
                                    //        {
                                    //            lDateVariazioni.Add(dtVar);
                                    //            lDateVariazioni.Sort();
                                    //        }
                                    //    }

                                    //    foreach (var fz in lpmab)
                                    //    {
                                    //        DateTime dtVar = new DateTime();

                                    //        if (fz.DATAFINEVALIDITA < trasferimento.DATARIENTRO)
                                    //        {
                                    //            dtVar = fz.DATAFINEVALIDITA.AddDays(1);
                                    //        }
                                    //        else
                                    //        {
                                    //            dtVar = trasferimento.DATARIENTRO;
                                    //        }

                                    //        if (!lDateVariazioni.Contains(dtVar))
                                    //        {
                                    //            lDateVariazioni.Add(dtVar);

                                    //        }
                                    //    }

                                    //var lcl =
                                    //        mab.CANONEMAB.Where(
                                    //            a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                    //            a.ATTIVAZIONEMAB.ANNULLATO == false &&
                                    //            a.ATTIVAZIONEMAB.NOTIFICARICHIESTA == true &&
                                    //            a.ATTIVAZIONEMAB.ATTIVAZIONE == true)
                                    //        .OrderBy(a => a.DATAINIZIOVALIDITA)
                                    //        .ToList();

                                    //        foreach (var cl in lcl)
                                    //        {
                                    //            DateTime dtVar = new DateTime();

                                    //            if (cl.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                    //            {
                                    //                dtVar = trasferimento.DATAPARTENZA;
                                    //            }
                                    //            else
                                    //            {
                                    //                dtVar = cl.DATAINIZIOVALIDITA;
                                    //            }

                                    //            if (!lDateVariazioni.Contains(dtVar))
                                    //            {
                                    //                lDateVariazioni.Add(dtVar);
                                    //                lDateVariazioni.Sort();
                                    //            }

                                    //        }

                                    #region Variazioni Indennità di Base
                                    var ll =
                                            db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.INDENNITABASE
                                            //indennita.INDENNITABASE
                                            .Where(a => a.ANNULLATO == false &&
                                            a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                            a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                            .OrderBy(a => a.IDLIVELLO)
                                            .ThenBy(a => a.DATAINIZIOVALIDITA)
                                            .ThenBy(a => a.DATAFINEVALIDITA)
                                            .ToList();

                                    foreach (var ib in ll)
                                    {
                                        DateTime dtVar = new DateTime();

                                        if (ib.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                        {
                                            dtVar = trasferimento.DATAPARTENZA;
                                        }
                                        else
                                        {
                                            dtVar = ib.DATAINIZIOVALIDITA;
                                        }


                                        if (!lDateVariazioni.Contains(dtVar))
                                        {
                                            lDateVariazioni.Add(dtVar);
                                            //lDateVariazioni.Sort();
                                        }
                                    }
                                    #endregion

                                    #region Variazioni Coefficente di Sede

                                    var lrd =
                                            db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.COEFFICIENTESEDE
                                            .Where(a => a.ANNULLATO == false &&
                                            a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                            a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                            .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                    foreach (var cs in lrd)
                                    {
                                        DateTime dtVar = new DateTime();

                                        if (cs.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                        {
                                            dtVar = trasferimento.DATAPARTENZA;
                                        }
                                        else
                                        {
                                            dtVar = cs.DATAINIZIOVALIDITA;
                                        }

                                        if (!lDateVariazioni.Contains(dtVar))
                                        {
                                            lDateVariazioni.Add(dtVar);
                                            //lDateVariazioni.Sort();
                                        }
                                    }
                                    #endregion

                                    #region Variazioni Percentuale di Disagio

                                    var perc =
                                            db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.PERCENTUALEDISAGIO
                                            .Where(a => a.ANNULLATO == false &&
                                            a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                            a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                                            .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                                    foreach (var pd in perc)
                                    {
                                        DateTime dtVar = new DateTime();

                                        if (pd.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                        {
                                            dtVar = trasferimento.DATAPARTENZA;
                                        }
                                        else
                                        {
                                            dtVar = pd.DATAINIZIOVALIDITA;
                                        }

                                        if (!lDateVariazioni.Contains(dtVar))
                                        {
                                            lDateVariazioni.Add(dtVar);
                                            //lDateVariazioni.Sort();
                                        }
                                    }

                                    #endregion

                                    var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                                    #region Variazioni Coniuge (Coniuge - Perc. Magg. Coniuge - Pensione)

                                    var lc =
                                        mf.CONIUGE.Where(
                                                a =>
                                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                            .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                    if (lc?.Any() ?? false)
                                    {
                                        foreach (var coniuge in lc)
                                        {
                                            var lpmc =
                                                coniuge.PERCENTUALEMAGCONIUGE.Where(
                                                        a =>
                                                            a.ANNULLATO == false)
                                                    .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                            if (lpmc?.Any() ?? false)
                                            {
                                                foreach (var pmc in lpmc)
                                                {
                                                    DateTime dtVar = new DateTime();

                                                    if (pmc.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                                    {
                                                        dtVar = trasferimento.DATAPARTENZA;
                                                    }
                                                    else
                                                    {
                                                        dtVar = pmc.DATAINIZIOVALIDITA;
                                                    }

                                                    if (!lDateVariazioni.Contains(dtVar))
                                                    {
                                                        lDateVariazioni.Add(dtVar);
                                                        //lDateVariazioni.Sort();
                                                    }

                                                }
                                            }

                                            var lpensioni =
                                                coniuge.PENSIONE.Where(
                                                        a =>
                                                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                                    .OrderByDescending(a => a.DATAINIZIO)
                                                    .ToList();

                                            if (lpensioni?.Any() ?? false)
                                            {
                                                foreach (var pensioni in lpensioni)
                                                {
                                                    DateTime dtVar = new DateTime();

                                                    if (pensioni.DATAINIZIO < trasferimento.DATAPARTENZA)
                                                    {
                                                        dtVar = trasferimento.DATAPARTENZA;
                                                    }
                                                    else
                                                    {
                                                        dtVar = pensioni.DATAINIZIO;
                                                    }
                                                    if (!lDateVariazioni.Contains(dtVar))
                                                    {
                                                        lDateVariazioni.Add(dtVar);
                                                        //lDateVariazioni.Sort();
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
                                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                    if (lf?.Any() ?? false)
                                    {
                                        foreach (var f in lf)
                                        {
                                            var lpmf =
                                                f.PERCENTUALEMAGFIGLI.Where(
                                                        a =>
                                                            a.ANNULLATO == false)
                                                    .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                            if (lpmf?.Any() ?? false)
                                            {
                                                foreach (var pmf in lpmf)
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
                                                        //lDateVariazioni.Sort();
                                                    }


                                                }
                                            }
                                        }
                                    }
                                    if (lf?.Any() ?? false)
                                    {

                                        foreach (var f in lf)
                                        {

                                            var lips =
                                                        f.INDENNITAPRIMOSEGRETARIO.Where(
                                                            a =>
                                                                a.ANNULLATO == false).ToList();

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
                                                //lDateVariazioni.Sort();
                                            }



                                        }

                                    }
                                    #endregion

                                    #region Variazioni del Canone MAB (TFR - PagatoCondivisoMAB - Perc. Condivisione MAB)

                                    var lcl =
                                            mab.CANONEMAB.Where(
                                                a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                a.ATTIVAZIONEMAB.ANNULLATO == false &&
                                                a.ATTIVAZIONEMAB.NOTIFICARICHIESTA == true &&
                                                a.ATTIVAZIONEMAB.ATTIVAZIONE == true)
                                            .OrderBy(a => a.DATAINIZIOVALIDITA)
                                            .ToList();

                                    foreach (var cl in lcl)
                                    {
                                        DateTime dtVar = new DateTime();

                                        if (cl.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                        {
                                            dtVar = trasferimento.DATAPARTENZA;
                                        }
                                        else
                                        {
                                            dtVar = cl.DATAINIZIOVALIDITA;
                                        }

                                        if (!lDateVariazioni.Contains(dtVar))
                                        {
                                            lDateVariazioni.Add(dtVar);
                                            lDateVariazioni.Sort();
                                        }

                                        var ltfr =
                                            cl.TFR.Where(
                                                    a =>
                                                        a.ANNULLATO == false)
                                                        .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                        .ToList();

                                        foreach (var tfr in ltfr)
                                        {

                                            if (tfr.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                            {
                                                dtVar = trasferimento.DATAPARTENZA;
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
                                                    if (pc.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                                    {
                                                        dtVar = trasferimento.DATAPARTENZA;
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


                                                                if (percCond.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                                                {
                                                                    dtVar = trasferimento.DATAPARTENZA;
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

                                lDateVariazioni.Add(new DateTime(9999, 12, 31));

                                lDateVariazioni = lDateVariazioni.OrderBy(a => a.Year).ThenBy(a => a.Month).ThenBy(a => a.Day).ToList();

                                if (lDateVariazioni?.Any() ?? false)
                                {
                                    for (int j = 0; j < lDateVariazioni.Count; j++)
                                    {
                                        DateTime dv = lDateVariazioni[j];

                                        if (dv < Utility.DataFineStop())
                                        {
                                            DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);
                                            //DateTime dvSucc = lDateVariazioni[(j + 1)];

                                            //if (lDateVariazioni[j + 1] == Utility.DataFineStop())
                                            //{
                                            //    dvSucc = lDateVariazioni[j + 1];
                                            //}

                                            if (lDateVariazioni[j + 1] == Utility.DataFineStop())
                                            {
                                                dvSucc = lDateVariazioni[j + 1];
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


                            string DataRichiamo = null;
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

                                    DataRichiamo = ib.DATARICHIAMO.ToShortDateString();
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

                                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
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
                                                dtRientro = DataRichiamo

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
            List<RiepiloVociModel> lrvm = new List<RiepiloVociModel>();

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
            List<RiepiloVociModel> lrvm = new List<RiepiloVociModel>();

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
                                    PercentualeFasciaKmP =  ci.PercentualeFKMPartenza,
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
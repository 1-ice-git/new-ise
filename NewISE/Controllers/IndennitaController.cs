
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

                // **************************************************************
                //using (dtIndennitaBase dtib = new dtIndennitaBase())
                //{
                //    eim = dtib.GetIndennitaBaseComune(idTrasferimento).ToList();
                //}
                // **************************************************************

                // **************************************************************
                //using (dtEvoluzioneIndennita dtd = new dtEvoluzioneIndennita())
                //{
                //    eim = dtd.GetIndennitaEvoluzione(idTrasferimento).ToList();
                //}
                // **************************************************************

                // **************************************************************
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
                        //lldm = dtld.GetLivelloDipendenteByIdTrasferimento(idTrasferimento).OrderBy(a => a.idLivello).ThenBy(a => a.dataInizioValdita).ThenBy(a => a.dataFineValidita).ToList();

                    }
                }
                // **************************************************************

                ViewBag.idTrasferimento = idTrasferimento;
                //return PartialView("EvoluzioneIndennita", eim);
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

                                            using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                            {
                                                //IndennitaBaseModel xx = new IndennitaBaseModel();
                                                RptIndennitaBaseModel rpts = new RptIndennitaBaseModel()
                                                {
                                                    IndennitaBase = ci.IndennitaDiBase,
                                                    DataInizioValidita = Convert.ToDateTime(dv).ToShortDateString(),
                                                    DataFineValidita = Convert.ToDateTime(dvSucc).ToShortDateString()
                                                    
                                                };

                                                rpt.Add(rpts);

                                                //xx.dataInizioValidita = dv;
                                                //xx.dataFineValidita = dvSucc;
                                                //xx.valore = ci.IndennitaDiBase;

                                                //var DataInizioValidita = Convert.ToDateTime(dv).ToShortDateString();
                                                //var DataFineValidita = Convert.ToDateTime(dvSucc).ToShortDateString();

                                                //var IndennitaBase = ci.IndennitaDiBase;
                                                //var test = ci.IndennitaDiBase.ToString();
                                                //var PercentualeDisagio = ci.PercentualeDisagio;
                                                //var CoefficienteSede = ci.CoefficienteDiSede;
                                                //var IndennitaServizio = ci.IndennitaDiServizio;

                                                ////string myString2 = Convert.ToString(IndennitaBase).ToString();
                                                ////char[] characters = myString2.ToArray();

                                                //indennitaBase[j] = ci.IndennitaDiBase.ToString("0.00");
                                                //dataInizioValidita[j] = Convert.ToDateTime(dv).ToShortDateString();
                                                //dataFineValidita[j] = Convert.ToDateTime(dvSucc).ToShortDateString();



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
                                    parameterValues.Add(new ReportParameter("Ruolo", Ruolo));
                                    parameterValues.Add(new ReportParameter("Livello", Livello));
                                    parameterValues.Add(new ReportParameter("Decorrenza", Decorrenza));
                                    parameterValues.Add(new ReportParameter("Ufficio", Ufficio));
                                    //parameterValues.Add(new ReportParameter("indennitaBase", indennitaBase));
                                    //parameterValues.Add(new ReportParameter("dataInizioValidita", dataInizioValidita));
                                    //parameterValues.Add(new ReportParameter("dataFineValidita", dataFineValidita));

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
                                        
                                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                        {
                                            
                                            RptIndennitaServizioModel rpts = new RptIndennitaServizioModel()
                                            {
                                                
                                                DataInizioValidita = Convert.ToDateTime(dv).ToShortDateString(),
                                                DataFineValidita = Convert.ToDateTime(dvSucc).ToShortDateString(),
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
                    //eim = dtei.GetMaggiorazioniFamiliariEvoluzione(idTrasferimento).ToList();
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


                //using (dtTrasferimento dtt = new dtTrasferimento())
                //{
                //    var tm = dtt.GetTrasferimentoById(idTrasferimento);
                //    using (dtRuoloUfficio dtru = new dtRuoloUfficio())
                //    {
                //        tm.RuoloUfficio = dtru.GetRuoloUfficioValidoByIdTrasferimento(tm.idTrasferimento);
                //        tm.idRuoloUfficio = tm.RuoloUfficio.idRuoloUfficio;
                //        ViewBag.idRuoloUfficio = tm.idRuoloUfficio;

                //    }

                    

                //    using (CalcoliIndennita ci = new CalcoliIndennita(tm.idTrasferimento))
                //    {
                //        dit.indennitaBase = ci.IndennitaDiBase;
                //        dit.indennitaServizio = ci.IndennitaDiServizio;
                //        dit.maggiorazioniFamiliari = ci.MaggiorazioniFamiliari;
                //        dit.indennitaPersonale = ci.IndennitaPersonale;
                //    }
                //}
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
                        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptMaggiorazioniFigli.rdlc";
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
            return PartialView("RptMaggiorazioniFigli");
        }
        public ActionResult RptMaggiorazioniConiuge(decimal idTrasferimento)
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
                        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptMaggiorazioniConiuge.rdlc";
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
            return PartialView("RptMaggiorazioniConiuge");
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
                        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptIndennitaPersonale.rdlc";
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
        public ActionResult RptMaggiorazioneAbitazione(decimal idTrasferimento)
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
                        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptMaggiorazioneAbitazione.rdlc";
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
                        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptIndennitadiRichiamoLorda.rdlc";
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

        #region Anticipo Indennita di Sistemazione Lorda + Report di Stampa
        public ActionResult AnticipoIndennitadiSistemazioneLorda(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

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
                        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptAnticipoIndennitadiSistemazioneLorda.rdlc";
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

            return PartialView("RptAnticipoIndennitadiSistemazioneLorda");
        }
        #endregion

        #region Indennita di Sistemazione Lorda + Report di Stampa
        public ActionResult IndennitadiSistemazioneLorda(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {

                using (dtEvoluzioneIndennita dtei = new dtEvoluzioneIndennita())
                {
                    eim = dtei.GetIndennitaSistemazioneLordaEvoluzione(idTrasferimento).ToList();

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
                        reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Report/RptIndennitadiSistemazioneLorda.rdlc";
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

            return PartialView("RptIndennitadiSistemazioneLorda");
        }

        #endregion

        #region Indennita di Sistemazione Netta + Report di Stampa
        public ActionResult IndennitadiSistemazioneNetta(decimal idTrasferimento)
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

                        List<DateTime> lDateVariazioni = new List<DateTime>();


                        #region Variazioni Percentuale Fascia Km

                        var ll =
                            db.PERCENTUALEFKM
                            .Where(a => a.ANNULLATO == false)
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

                        lDateVariazioni.Add(new DateTime(9999, 12, 31));

                        if (lDateVariazioni?.Any() ?? false)
                        {
                            for (int j = 0; j < lDateVariazioni.Count; j++)
                            {
                                DateTime dv = lDateVariazioni[j];

                                if (dv < Utility.DataFineStop())
                                {
                                    DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                        

                                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                        {

                                            RptContributoOmnicomprensivoTrasferimentoModel rpts = new RptContributoOmnicomprensivoTrasferimentoModel()
                                            
                                            {

                                                DataInizioValidita = Convert.ToDateTime(dv).ToShortDateString(),
                                                DataFineValidita = Convert.ToDateTime(dvSucc).ToShortDateString(),
                                                IndennitaSistemazioneLorda = ci.IndennitaSistemazioneLorda,
                                                AnticipoContrOmniComprensivoPartenza = ci.AnticipoContributoOmnicomprensivoPartenza,
                                                SaldoContrOmniComprensivoPartenza = ci.SaldoContributoOmnicomprensivoPartenza

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


                            List<DateTime> lDateVariazioni = new List<DateTime>();


                            #region Variazioni Percentuale Fascia Km

                            var ll =
                                db.PERCENTUALEFKM
                                .Where(a => a.ANNULLATO == false)
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

                            lDateVariazioni.Add(new DateTime(9999, 12, 31));

                            if (lDateVariazioni?.Any() ?? false)
                            {
                                for (int j = 0; j < lDateVariazioni.Count; j++)
                                {
                                    DateTime dv = lDateVariazioni[j];

                                    if (dv < Utility.DataFineStop())
                                    {
                                        DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                        {

                                            RptContributoOmnicomprensivoRientroModel rpts = new RptContributoOmnicomprensivoRientroModel()
                                            {
                                            
                                                DataInizioValidita = Convert.ToDateTime(dv).ToShortDateString(),
                                                DataFineValidita = Convert.ToDateTime(dvSucc).ToShortDateString(),
                                                IndennitaRichiamo = ci.IndennitaRichiamoLordo,
                                                AnticipoContrOmniComprensivoRientro = ci.AnticipoContributoOmnicomprensivoPartenza,
                                                SaldoContrOmniComprensivoPartenza = ci.SaldoContributoOmnicomprensivoPartenza
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
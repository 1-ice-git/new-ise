﻿using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NewISE.EF;
using NewISE.Models;
using NewISE.Models.Tools;
using MaggiorazioniFamiliariModel = NewISE.Models.DBModel.MaggiorazioniFamiliariModel;



namespace NewISE.Controllers
{
    public class VariazioneMaggiorazioniFamiliariController : Controller
    {
        // GET: VariazioneMaggiorazioniFamiliari
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult VerificaMaggiorazioneFamiliare(string matricola = "")
        {
            try
            {
                if (matricola == string.Empty)
                {
                    throw new Exception("La matricola non risulta valorizzata.");
                }
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    TrasferimentoModel trm = dtt.GetTrasferimentoAttivoNotificato(matricola);
                    if (trm != null && trm.HasValue())
                    {
                        using (dtMaggiorazioniFamiliari dtd = new dtMaggiorazioniFamiliari())
                        {
                            MaggiorazioniFamiliariModel dm = dtd.GetMaggiorazioniFamiliariByID(trm.idTrasferimento);

                            if (dm.idMaggiorazioniFamiliari.ToString() != null)
                            {
                                if (dm.idMaggiorazioniFamiliari > 0)
                                {
                                    return Json(new { VerificaMaggiorazione = 1 });
                                }
                                else
                                {
                                    return Json(new { VerificaMaggiorazione = 0 });
                                }
                            }
                            else
                            {
                                return Json(new { VerificaMaggiorazione = 0 });
                            }

                        }

                    }
                    else
                    {
                        return Json(new { VerificaMaggiorazione = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }

        }

        public ActionResult AttivitaMaggiorazioneFamiliare(string matricola)
        {
            using (dtTrasferimento dtt = new dtTrasferimento())
            {
                var tr = dtt.GetUltimoSoloTrasferimentoByMatricola(matricola);

                if (tr != null && tr.HasValue())
                {
                    ViewBag.idTrasferimento = tr.idTrasferimento;
                }
                else
                {
                    throw new Exception("Nessun trasferimento per la matricola (" + matricola + ")");
                }
            }

            ViewBag.matricola = matricola;

            return PartialView("AttivitaMaggiorazioneFamiliare");
        }


        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult ElencoFamiliari(decimal idMaggiorazioniFamiliari)
        {
            List<ElencoFamiliariModel> lefm = new List<ElencoFamiliariModel>();

            try
            {
                //
                //Lista Coniuge
                //
                using (dtConiuge dtc = new dtConiuge())
                {
                    List<ConiugeModel> lcm = dtc.GetListaConiugeByIdMagFam(idMaggiorazioniFamiliari).ToList();

                    if (lcm?.Any() ?? false)
                    {
                        using (dtDocumenti dtd = new dtDocumenti())
                        {
                            using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                            {
                                using (dtPensione dtp = new dtPensione())
                                {
                                    foreach (var e in lcm)
                                    {
                                        ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                        {
                                            idMaggiorazioniFamiliari = e.idMaggiorazioniFamiliari,
                                            idFamiliare = e.idConiuge,
                                            idPassaporti = e.idPassaporti,
                                            Nominativo = e.cognome + " " + e.nome,
                                            CodiceFiscale = e.codiceFiscale,
                                            dataInizio = e.dataInizio,
                                            dataFine = e.dataFine,
                                            parentela = EnumParentela.Coniuge,
                                            idAltriDati = dtadf.GetAlttriDatiFamiliariConiuge(e.idConiuge).idAltriDatiFam,
                                            Documenti = dtd.GetDocumentiByIdTable(e.idConiuge, EnumTipoDoc.Documento_Identita, EnumParentela.Coniuge),
                                            HasPensione = dtp.HasPensione(e.idConiuge)
                                        };

                                        lefm.Add(efm);
                                    }
                                }

                            }
                        }
                    }

                }

                //
                // Lista Figli
                //
                using (dtFigli dtf = new dtFigli())
                {
                    List<FigliModel> lfm = dtf.GetListaFigli(idMaggiorazioniFamiliari).ToList();

                    if (lfm?.Any() ?? false)
                    {
                        using (dtDocumenti dtd = new dtDocumenti())
                        {
                            using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                            {
                                foreach (var e in lfm)
                                {
                                    ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                    {
                                        idMaggiorazioniFamiliari = e.idMaggiorazioniFamiliari,
                                        idFamiliare = e.idFigli,
                                        idPassaporti = e.idPassaporti,
                                        Nominativo = e.cognome + " " + e.nome,
                                        CodiceFiscale = e.codiceFiscale,
                                        dataInizio = e.dataInizio,
                                        dataFine = e.dataFine,
                                        parentela = EnumParentela.Figlio,
                                        idAltriDati = dtadf.GetAlttriDatiFamiliariFiglio(e.idFigli).idAltriDatiFam,
                                        Documenti = dtd.GetDocumentiByIdTable(e.idFigli, EnumTipoDoc.Documento_Identita, EnumParentela.Figlio)
                                    };

                                    lefm.Add(efm);
                                }
                            }
                        }
                    }

                    //ViewData.Add("callConiuge", false);

                    using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                    {
                        bool rinunciaMagFam = false;
                        bool richiestaAttivazione = false;
                        bool attivazione = false;
                        bool datiConiuge = false;
                        bool datiParzialiConiuge = false;
                        bool datiFigli = false;
                        bool datiParzialiFigli = false;
                        bool siDocConiuge = false;
                        bool siDocFigli = false;
                        bool docFormulario = false;

                        bool solaLettura = false;

                        dtmf.SituazioneMagFam(idMaggiorazioniFamiliari, out rinunciaMagFam,
                            out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                            out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario);

                        if (richiestaAttivazione == true || attivazione == true)
                        {
                            solaLettura = true;
                        }
                        else
                        {
                            solaLettura = false;
                        }

                        ViewData.Add("solaLettura", solaLettura);
                    }

                    ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);

                    return PartialView(lefm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }


        public ActionResult NuovoFamiliare(decimal idMaggiorazioniFamiliari)
        {
            //ConiugeModel cm = new ConiugeModel();
            List<SelectListItem> lTipologiaConiuge = new List<SelectListItem>();
            List<SelectListItem> lTipologiaFiglio = new List<SelectListItem>();
            List<SelectListItem> lTipologiaFamiliare = new List<SelectListItem>();

            var rc = new List<SelectListItem>();

            FigliModel fm = new FigliModel();
            var rf = new List<SelectListItem>();

            var rfam = new List<SelectListItem>();

            try
            {

                rfam.Insert(0, new SelectListItem() { Text = "FIGLIO", Value = "2" });
                rfam.Insert(0, new SelectListItem() { Text = "CONIUGE", Value = "1" });
                rfam.Insert(0, new SelectListItem() { Text = "", Value = "0" });
                lTipologiaFamiliare = rfam;


                using (dtTipologiaConiuge dttc = new dtTipologiaConiuge())
                {
                    var ltcm = dttc.GetListTipologiaConiuge();

                    if (ltcm != null && ltcm.Count > 0)
                    {
                        rc = (from t in ltcm
                              select new SelectListItem()
                              {
                                  Text = t.tipologiaConiuge,
                                  Value = t.idTipologiaConiuge.ToString()
                              }).ToList();
                        rc.Insert(0, new SelectListItem() { Text = "", Value = "" });
                    }

                    lTipologiaConiuge = rc;

                    using (dtTipologiaFiglio dttf = new dtTipologiaFiglio())
                    {
                        var ltfm = dttf.GetListTipologiaFiglio().ToList();

                        if (ltfm?.Any() ?? false)
                        {
                            rf = (from t in ltfm
                                  select new SelectListItem()
                                  {
                                      Text = t.tipologiaFiglio,
                                      Value = t.idTipologiaFiglio.ToString()
                                  }).ToList();
                            rf.Insert(0, new SelectListItem() { Text = "", Value = "" });
                        }

                        lTipologiaFiglio = rf;
                    }
                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewBag.lTipologiaConiuge = lTipologiaConiuge;
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);
            ViewData.Add("lTipologiaFiglio", lTipologiaFiglio);
            ViewData.Add("lTipologiaFamiliare", lTipologiaFamiliare);

            return PartialView();
        }

        public ActionResult NuovoConiuge(decimal idMaggiorazioniFamiliari)
        {
            List<SelectListItem> lTipologiaConiuge = new List<SelectListItem>();

            var r = new List<SelectListItem>();

            try
            {

                using (dtTipologiaConiuge dttc = new dtTipologiaConiuge())
                {
                    var ltcm = dttc.GetListTipologiaConiuge();

                    if (ltcm != null && ltcm.Count > 0)
                    {
                        r = (from t in ltcm
                             select new SelectListItem()
                             {
                                 Text = t.tipologiaConiuge,
                                 Value = t.idTipologiaConiuge.ToString()
                             }).ToList();
                        r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                    }

                    lTipologiaConiuge = r;

                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewBag.lTipologiaConiuge = lTipologiaConiuge;
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);

            return PartialView();
        }

        public ActionResult ConfermaNuovoConiuge(ConiugeModel cm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                    {
                        //aggiungiConiuge
                        //dtmf.ModificaConiuge(cm);
                    }
                }
                else
                {
                    using (dtTipologiaConiuge dttc = new dtTipologiaConiuge())
                    {
                        List<SelectListItem> lTipologiaConiuge = new List<SelectListItem>();

                        var r = new List<SelectListItem>();

                        var ltcm = dttc.GetListTipologiaConiuge();

                        if (ltcm != null && ltcm.Count > 0)
                        {
                            r = (from t in ltcm
                                 select new SelectListItem()
                                 {
                                     Text = t.tipologiaConiuge,
                                     Value = t.idTipologiaConiuge.ToString()
                                 }).ToList();
                            r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                        }

                        lTipologiaConiuge = r;

                        ViewBag.lTipologiaConiuge = lTipologiaConiuge;
                    }
                    return PartialView("ElencoFamiliari", cm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return RedirectToAction("ElencoFamiliari",
                new { idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari });
        }

        public ActionResult ModificaConiuge(decimal idConiuge)
        {
            ConiugeModel cm = new ConiugeModel();

            try
            {
                using (dtConiuge dtc = new dtConiuge())
                {
                    cm = dtc.GetConiugebyID(idConiuge);
                }

                using (dtTipologiaConiuge dttc = new dtTipologiaConiuge())
                {
                    List<SelectListItem> lTipologiaConiuge = new List<SelectListItem>();

                    var r = new List<SelectListItem>();

                    var ltcm = dttc.GetListTipologiaConiuge();

                    if (ltcm != null && ltcm.Count > 0)
                    {
                        r = (from t in ltcm
                             select new SelectListItem()
                             {
                                 Text = t.tipologiaConiuge,
                                 Value = t.idTipologiaConiuge.ToString()
                             }).ToList();
                        r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                    }

                    lTipologiaConiuge = r;

                    ViewBag.lTipologiaConiuge = lTipologiaConiuge;
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            //ViewData.Add("idTrasferimento", idTrasferimento);
            return PartialView(cm);
        }

        public ActionResult ConfermaModificaConiuge(ConiugeModel cm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                    {
                        dtvmf.ModificaConiuge(cm);

                    }
                }
                else
                {
                    using (dtTipologiaConiuge dttc = new dtTipologiaConiuge())
                    {
                        List<SelectListItem> lTipologiaConiuge = new List<SelectListItem>();

                        var r = new List<SelectListItem>();

                        var ltcm = dttc.GetListTipologiaConiuge();

                        if (ltcm != null && ltcm.Count > 0)
                        {
                            r = (from t in ltcm
                                 select new SelectListItem()
                                 {
                                     Text = t.tipologiaConiuge,
                                     Value = t.idTipologiaConiuge.ToString()
                                 }).ToList();
                            r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                        }

                        lTipologiaConiuge = r;

                        ViewBag.lTipologiaConiuge = lTipologiaConiuge;
                    }
                    return PartialView("ModificaConiuge", cm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return RedirectToAction("ElencoFamiliari",
                new { idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari });
        }

        public ActionResult ModificaFiglio(decimal idFiglio)
        {
            FigliModel fm = new FigliModel();

            try
            {
                using (dtFigli dtf = new dtFigli())
                {
                    fm = dtf.GetFigliobyID(idFiglio);
                }

                using (dtTipologiaFiglio dttf = new dtTipologiaFiglio())
                {
                    List<SelectListItem> lTipologiaFiglio = new List<SelectListItem>();

                    var r = new List<SelectListItem>();

                    var ltfm = dttf.GetListTipologiaFiglio();

                    if (ltfm != null && ltfm.Count > 0)
                    {
                        r = (from t in ltfm
                             select new SelectListItem()
                             {
                                 Text = t.tipologiaFiglio,
                                 Value = t.idTipologiaFiglio.ToString()
                             }).ToList();
                        r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                    }

                    lTipologiaFiglio = r;

                    ViewBag.lTipologiaFiglio = lTipologiaFiglio;
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            //ViewData.Add("idTrasferimento", idTrasferimento);
            return PartialView(fm);
        }

        public ActionResult ConfermaModificaFiglio(FigliModel fm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                    {
                        dtvmf.ModificaFiglio(fm);

                    }
                }
                else
                {
                    using (dtTipologiaFiglio dttf = new dtTipologiaFiglio())
                    {
                        List<SelectListItem> lTipologiaFiglio = new List<SelectListItem>();

                        var r = new List<SelectListItem>();

                        var ltfm = dttf.GetListTipologiaFiglio();

                        if (ltfm != null && ltfm.Count > 0)
                        {
                            r = (from t in ltfm
                                 select new SelectListItem()
                                 {
                                     Text = t.tipologiaFiglio,
                                     Value = t.idTipologiaFiglio.ToString()
                                 }).ToList();
                            r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                        }

                        lTipologiaFiglio = r;

                        ViewBag.lTipologiaFiglio = lTipologiaFiglio;
                    }
                    return PartialView("ModificaFiglio", fm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return RedirectToAction("ElencoFamiliari",
                new { idMaggiorazioniFamiliari = fm.idMaggiorazioniFamiliari });
        }

        public ActionResult ElencoFormulariInseriti(decimal idMaggiorazioniFamiliari)
        {

            ViewData["idMaggiorazioniFamiliari"] = idMaggiorazioniFamiliari;

            return PartialView();
        }


    }
}
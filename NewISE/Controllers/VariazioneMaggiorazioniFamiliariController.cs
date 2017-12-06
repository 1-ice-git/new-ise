using NewISE.DBComuniItalia;
using NewISE.Models.DBModel;
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
using Newtonsoft.Json;
using System.IO;
using System.Net.Configuration;

namespace NewISE.Controllers
{
    public class VariazioneMaggiorazioniFamiliariController : Controller
    {
        // GET: VariazioneMaggiorazioniFamiliari
        public ActionResult Index()
        {
            return View();
        }

        [NonAction]
        private bool SolaLettura(decimal idMaggiorazioniFamiliari)
        {

            bool solaLettura = false;

            using (dtVariazioniMaggiorazioneFamiliare dtmf = new dtVariazioniMaggiorazioneFamiliare())
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
                bool inLavorazione = false;


                dtmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                    out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                    out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione);

                if (richiestaAttivazione == true || attivazione == true)
                {

                    solaLettura = true;
                }
                else
                {
                    if (rinunciaMagFam)
                    {
                        solaLettura = true;
                    }
                    else
                    {
                        solaLettura = false;
                    }

                }


            }

            return solaLettura;
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
                    List<VariazioneConiugeModel> lcm = dtc.GetListaAttivazioniConiugeByIdMagFam(idMaggiorazioniFamiliari).ToList();


                    if (lcm?.Any() ?? false)
                    {
                        using (dtDocumenti dtd = new dtDocumenti())
                        {
                            using (dtVariazioniMaggiorazioneFamiliare dtadf = new dtVariazioniMaggiorazioneFamiliare())
                            {
                                using (dtPensione dtp = new dtPensione())
                                {
                                    //   foreach (var e in lcm)
                                    //   {
                                    var e = lcm.First();

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
                                        idAltriDati = dtadf.GetAltriDatiFamiliariConiuge(e.idConiuge, idMaggiorazioniFamiliari).idAltriDatiFam,
                                        Documenti = dtd.GetDocumentiByIdTable(e.idConiuge, EnumTipoDoc.Documento_Identita, EnumParentela.Coniuge),
                                        HasPensione = dtp.HasPensione(e.idConiuge)
                                    };

                                    lefm.Add(efm);
                                    //}
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
                    //da rifare
                    //List<FigliModel> lfm = dtf.GetListaFigli(idMaggiorazioniFamiliari).ToList();

                    //if (lfm?.Any() ?? false)
                    //{
                    //    using (dtDocumenti dtd = new dtDocumenti())
                    //    {
                    //        using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                    //        {
                    //            foreach (var e in lfm)
                    //            {
                    //                ElencoFamiliariModel efm = new ElencoFamiliariModel()
                    //                {
                    //                    idMaggiorazioniFamiliari = e.idMaggiorazioniFamiliari,
                    //                    idFamiliare = e.idFigli,
                    //                    idPassaporti = e.idPassaporti,
                    //                    Nominativo = e.cognome + " " + e.nome,
                    //                    CodiceFiscale = e.codiceFiscale,
                    //                    dataInizio = e.dataInizio,
                    //                    dataFine = e.dataFine,
                    //                    parentela = EnumParentela.Figlio,
                    //                    idAltriDati = dtadf.GetAlttriDatiFamiliariFiglio(e.idFigli).idAltriDatiFam,
                    //                    Documenti = dtd.GetDocumentiByIdTable(e.idFigli, EnumTipoDoc.Documento_Identita, EnumParentela.Figlio)
                    //                };

                    //                lefm.Add(efm);
                    //            }
                    //        }
                    //    }
                    //}

                    //ViewData.Add("callConiuge", false);

                    //using (dtVariazioniMaggiorazioneFamiliare dtmf = new dtVariazioniMaggiorazioneFamiliare())
                    //{
                    //    bool rinunciaMagFam = false;
                    //    bool richiestaAttivazione = false;
                    //    bool attivazione = false;
                    //    bool datiConiuge = false;
                    //    bool datiParzialiConiuge = false;
                    //    bool datiFigli = false;
                    //    bool datiParzialiFigli = false;
                    //    bool siDocConiuge = false;
                    //    bool siDocFigli = false;
                    //    bool docFormulario = false;
                    //    bool inLavorazione = false;

                    //    bool solaLettura = false;

                    //    dtmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                    //        out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                    //        out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione);

                    //    if (!richiestaAttivazione && !attivazione == false && inLavorazione) 
                    //    {
                    //        solaLettura = false;
                    //    }
                    //    else
                    //    {
                    //        solaLettura = true;
                    //    }

                    //    ViewData.Add("solaLettura", solaLettura);
                    ViewData.Add("solaLettura", false);
                    //}

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
                    rfam.Insert(0, new SelectListItem() { Text = "FIGLIO", Value = "2" });

                    lTipologiaFiglio = rf;
                }

                // verifica che il coniuge non sia gia presente
                using (dtConiuge dtc = new dtConiuge())
                {
                    //da rifare
                    //var lc = dtc.GetListaConiugeByIdMagFam(idMaggiorazioniFamiliari);
                    //if (lc == null || lc.Count == 0)
                    //{
                    rfam.Insert(0, new SelectListItem() { Text = "CONIUGE", Value = "1" });

                    //    using (dtTipologiaConiuge dttc = new dtTipologiaConiuge())
                    //    {
                    //        var ltcm = dttc.GetListTipologiaConiuge();

                    //        if (ltcm != null && ltcm.Count > 0)
                    //        {
                    //            rc = (from t in ltcm
                    //                  select new SelectListItem()
                    //                  {
                    //                      Text = t.tipologiaConiuge,
                    //                      Value = t.idTipologiaConiuge.ToString()
                    //                  }).ToList();
                    //            rc.Insert(0, new SelectListItem() { Text = "", Value = "" });
                    //        }

                    //        lTipologiaConiuge = rc;
                    //    }
                    //}
                }


                rfam.Insert(0, new SelectListItem() { Text = "", Value = "0" });
                lTipologiaFamiliare = rfam;

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
            //ViewBag.valido = valido;


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
            bool inLavorazione = false;

            using (dtVariazioniMaggiorazioneFamiliare dtmf = new dtVariazioniMaggiorazioneFamiliare())
            {
                dtmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione);
            }

            List<SelectListItem> lDataAttivazione = new List<SelectListItem>();
            List<AttivazioniMagFamModel> lamf = new List<AttivazioniMagFamModel>();

            try
            {
                using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                {
                    lamf = dtamf.GetListAttivazioniMagFamByIdMagFam(idMaggiorazioniFamiliari).ToList();
                }
                var i = 1;

                foreach (var e in lamf)
                {
                    if (!e.annullato)
                    {
                        if (e.richiestaAttivazione == false && e.attivazioneMagFam == false && lamf.Count() > 1)
                        {
                            if (docFormulario)
                            {
                                lDataAttivazione.Insert(0, new SelectListItem() { Text = "(" + i.ToString() + ") " + e.dataVariazione.ToString() + " (In Lavorazione)", Value = e.idAttivazioneMagFam.ToString() });
                            }
                        }
                        else
                        {
                            lDataAttivazione.Insert(0, new SelectListItem() { Text = "(" + i.ToString() + ") " + e.dataVariazione.ToString(), Value = e.idAttivazioneMagFam.ToString() });
                        }
                        i++;
                    }
                }
                lDataAttivazione.Insert(0, new SelectListItem() { Text = "", Value = "" });

                ViewData.Add("lDataAttivazione", lDataAttivazione);

                ViewData["idMaggiorazioniFamiliari"] = idMaggiorazioniFamiliari;

                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult NuovoFormularioMF(decimal idMaggiorazioniFamiliari)
        {

            ViewData["idMaggiorazioniFamiliari"] = idMaggiorazioniFamiliari;

            return PartialView();
        }



        public ActionResult ElencoDocumentiFormulario()
        {
            return PartialView();
        }


        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult AltriDatiFamiliariConiuge(decimal idConiuge)
        {
            AltriDatiFamConiugeModel adf = new AltriDatiFamConiugeModel();
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();


            try
            {
                using (var db = new ModelDBISE())
                {
                    var c = db.CONIUGE.Find(idConiuge);

                    decimal idMaggiorazioniFamiliari = 0;
                    idMaggiorazioniFamiliari = c.MAGGIORAZIONIFAMILIARI.IDMAGGIORAZIONIFAMILIARI;
                    var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                    using (dtVariazioniMaggiorazioneFamiliare dtadf = new dtVariazioniMaggiorazioneFamiliare())
                    {
                        adf = dtadf.GetAltriDatiFamiliariConiuge(idConiuge, idMaggiorazioniFamiliari);
                    }
                    ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);

                    //using (dtMaggiorazioniFamiliari dtmc = new dtMaggiorazioniFamiliari())
                    //{
                    //    mcm = dtmc.GetMaggiorazioniFamiliaribyConiuge(idConiuge);
                    //}

                    using (dtVariazioniMaggiorazioneFamiliare dtmf = new dtVariazioniMaggiorazioneFamiliare())
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
                        bool inLavorazione = false;

                        bool solaLettura = false;

                        dtmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                            out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                            out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione);

                        if (richiestaAttivazione == true)
                        {
                            solaLettura = true;
                        }
                        else
                        {
                            solaLettura = false;
                        }

                        ViewData.Add("solaLettura", solaLettura);
                    }

                    using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                    {
                        PercentualeMagConiugeModel pc = dtpc.GetPercMagConiugeNow(idConiuge, DateTime.Now.Date);

                        if (pc != null && pc.HasValue())
                        {
                            switch (pc.idTipologiaConiuge)
                            {
                                case TipologiaConiuge.Residente:
                                    adf.residente = true;
                                    adf.ulterioreMagConiuge = false;
                                    break;

                                case TipologiaConiuge.NonResidente:
                                    adf.residente = false;
                                    adf.ulterioreMagConiuge = false;
                                    break;

                                case TipologiaConiuge.NonResidenteCarico:
                                    adf.residente = false;
                                    adf.ulterioreMagConiuge = true;
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


            if (adf != null && adf.HasValue())
            {
                using (dtConiuge dtc = new dtConiuge())
                {
                    if (adf.idConiuge.HasValue)
                    {
                        var cm = dtc.GetConiugebyID(adf.idConiuge.Value);
                        adf.Coniuge = cm;
                    }
                }


                return PartialView(adf);
            }
            else
            {
                List<Comuni> comuni = new List<Comuni>();

                using (StreamReader sr = new StreamReader(Server.MapPath("~/DBComuniItalia/jsonComuniItalia.json")))
                {
                    comuni = JsonConvert.DeserializeObject<List<Comuni>>(sr.ReadToEnd(), new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }

                adf.idConiuge = idConiuge;

                ViewData.Add("Comuni", comuni);

                return PartialView("InserisciAltriDatiFamiliariConiuge", adf);
            }
        }
        public ActionResult ModificaAltriDatiFamiliariConiuge(decimal idAltriDatiFam, decimal idMaggiorazioniFamiliari)
        {
            AltriDatiFamConiugeModel adfm = new AltriDatiFamConiugeModel();

            try
            {
                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                {
                    adfm = dtadf.GetAltriDatiFamiliariConiuge(idAltriDatiFam);
                    if (adfm != null && adfm.HasValue())
                    {
                        using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                        {
                            PercentualeMagConiugeModel pc = new PercentualeMagConiugeModel();

                            pc = dtpc.GetPercMagConiugeNow(adfm.idConiuge.Value, DateTime.Now.Date);

                            if (pc != null && pc.HasValue())
                            {
                                switch (pc.idTipologiaConiuge)
                                {
                                    case TipologiaConiuge.Residente:
                                        adfm.residente = true;
                                        adfm.ulterioreMagConiuge = false;
                                        break;

                                    case TipologiaConiuge.NonResidente:
                                        adfm.residente = false;
                                        adfm.ulterioreMagConiuge = false;
                                        break;

                                    case TipologiaConiuge.NonResidenteCarico:
                                        adfm.residente = false;
                                        adfm.ulterioreMagConiuge = true;
                                        break;

                                    default:
                                        break;
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

            List<Comuni> comuni = new List<Comuni>();

            try
            {
                using (StreamReader sr = new StreamReader(Server.MapPath("~/DBComuniItalia/jsonComuniItalia.json")))
                {
                    comuni = JsonConvert.DeserializeObject<List<Comuni>>(sr.ReadToEnd(), new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewData.Add("Comuni", comuni);
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);

            return PartialView(adfm);
        }

        public ActionResult ConfermaModificaAdfConiuge(AltriDatiFamConiugeModel adfm)
        {
            try
            {
                adfm.dataAggiornamento = DateTime.Now;
                adfm.annullato = false;

                if (ModelState.IsValid)
                {
                    using (dtVariazioniMaggiorazioneFamiliare dtadf = new dtVariazioniMaggiorazioneFamiliare())
                    {
                        dtadf.EditVariazioneAltriDatiFamiliariConiuge(adfm);

                    }
                }
                else
                {

                    return PartialView("ModificaAltriDatiFamiliariConiuge", adfm);

                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return RedirectToAction("AltriDatiFamiliariConiuge", new { idConiuge = adfm.idConiuge });


        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ElencoPensioniConiuge(decimal idConiuge)
        {
            List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();

            try
            {
                using (dtPensione dtp = new dtPensione())
                {
                    lpcm = dtp.GetPensioniByIdConiuge(idConiuge).ToList();
                }
                using (dtConiuge dtc = new dtConiuge())
                {
                    decimal idMaggiorazioniFamiliari = dtc.GetConiugebyID(idConiuge).idMaggiorazioniFamiliari;
                    ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);

                    using (dtVariazioniMaggiorazioneFamiliare dtmf = new dtVariazioniMaggiorazioneFamiliare())
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
                        bool inLavorazione = false;

                        bool solaLettura = false;

                        dtmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                            out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                            out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione);

                        if (richiestaAttivazione == true)
                        {
                            solaLettura = true;
                        }
                        else
                        {
                            solaLettura = false;
                        }

                        ViewData.Add("solaLettura", solaLettura);
                    }
                }

            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            //ViewData.Add("idTrasferimento", mcm.idTrasferimento);
            ViewData.Add("idConiuge", idConiuge);

            return PartialView(lpcm);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NuovoImportoPensione(decimal idConiuge)
        {
            //PensioneConiugeModel pcm = new PensioneConiugeModel();

            ViewData.Add("idConiuge", idConiuge);
            return PartialView();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciImportoPensione(PensioneConiugeModel pcm, decimal idConiuge)
        {

            try
            {

                if (ModelState.IsValid)
                {

                    using (dtPensione dtp = new dtPensione())
                    {
                        try
                        {
                            dtp.VerificaDataInizioPensione(idConiuge, pcm.dataInizioValidita);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", ex.Message);
                            return PartialView("NuovoImportoPensione", pcm);
                        }
                        pcm.dataAggiornamento = DateTime.Now;
                        pcm.annullato = false;
                        if (!pcm.dataFineValidita.HasValue)
                        {
                            pcm.dataFineValidita = Utility.DataFineStop();
                        }

                        //dtp.SetNuovoImportoPensione(ref pcm, idConiuge);
                    }
                }
                else
                {
                    //ViewData.Add("idMaggiorazioneConiuge", pcm.idMaggiorazioneConiuge);
                    return PartialView("NuovoImportoPensione", pcm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return RedirectToAction("ElencoPensioniConiuge", new { idConiuge = idConiuge });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EliminaPensione(decimal idPensione, decimal idConiuge)
        {
            PensioneConiugeModel pcm = new PensioneConiugeModel();

            try
            {
                using (dtPensione dtp = new dtPensione())
                {
                    pcm = dtp.GetPensioneByID(idPensione);

                    if (pcm != null && pcm.HasValue())
                    {
                        //dtp.EliminaImportoPensione(pcm, idConiuge);
                    }
                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


            return RedirectToAction("ElencoPensioniConiuge", new { idConiuge = idConiuge });
        }

        public JsonResult PulsantiNotificaAttivaMagFam(decimal idMaggiorazioniFamiliari)
        {
            bool amministratore = false;
            string errore = "";
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
            bool inLavorazione = false;

            try
            {
                amministratore = Utility.Amministratore();
                using (dtVariazioniMaggiorazioneFamiliare dtmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    dtmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                        out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                        out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione);
                }

            }
            catch (Exception ex)
            {

                errore = ex.Message;
            }

            return
                Json(
                    new
                    {
                        admin = amministratore,
                        rinuncia = rinunciaMagFam,
                        richiesta = richiestaAttivazione,
                        attivazione = attivazione,
                        datiConiuge = datiConiuge,
                        datiParzialiConiuge = datiParzialiConiuge,
                        datiFigli = datiFigli,
                        datiParzialiFigli = datiParzialiFigli,
                        siDocConiuge = siDocConiuge,
                        siDocFigli = siDocFigli,
                        docFormulario = docFormulario,
                        inLavorazione = inLavorazione,
                        err = errore
                    });

        }

        [HttpPost]
        public JsonResult NotificaRichiestaVariazione(decimal idMaggiorazioniFamiliari)
        {
            string errore = "";

            try
            {
                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                {
                    dtmf.NotificaRichiestaVariazione(idMaggiorazioniFamiliari);
                }
            }
            catch (Exception ex)
            {

                errore = ex.Message;
            }

            return
                Json(
                    new
                    {
                        err = errore
                    });
        }

        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public JsonResult AttivaRichiesta(decimal idMaggiorazioniFamiliari)
        {
            string errore = "";

            try
            {
                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                {
                    dtmf.AttivaRichiesta(idMaggiorazioniFamiliari);
                }
            }
            catch (Exception ex)
            {
                errore = ex.Message;
            }

            return
                Json(
                    new
                    {
                        err = errore
                    });

        }

        [HttpPost]
        public JsonResult AnnullaRichiesta(decimal idMaggiorazioniFamiliari)
        {
            string errore = "";

            try
            {
                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                {
                    //dtmf.AnnullaRichiesta(idMaggiorazioniFamiliari);
                }
            }
            catch (Exception ex)
            {

                errore = ex.Message;
            }

            return
                Json(
                    new
                    {
                        err = errore
                    });
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult TabFormulariInseriti(decimal idMaggiorazioniFamiliari)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            try
            {

                bool solaLettura = false;
                solaLettura = this.SolaLettura(idMaggiorazioniFamiliari);
                ViewData.Add("solaLettura", solaLettura);

                using (dtDocumenti dtd = new dtDocumenti())
                {
                    ldm = dtd.GetFormulariMaggiorazioniFamiliariVariazione(idMaggiorazioniFamiliari).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(ldm);
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult FiltraTabFormulariInseriti(decimal idMaggiorazioniFamiliari, decimal idAttivazione)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();


            try
            {

                bool solaLettura = false;
                solaLettura = this.SolaLettura(idMaggiorazioniFamiliari);
                ViewData.Add("solaLettura", solaLettura);

                using (dtDocumenti dtd = new dtDocumenti())
                {
                    ldm = dtd.GetFormulariMaggiorazioniFamiliariVariazioneByIdAttivazione(idMaggiorazioniFamiliari, idAttivazione).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("TabFormulariInseriti", ldm);
        }


        [HttpPost]
        public JsonResult InserisciFormularioMF(decimal idMaggiorazioniFamiliari, HttpPostedFileBase file)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (dtVariazioniMaggiorazioneFamiliare dtd = new dtVariazioniMaggiorazioneFamiliare())
                    {
                        DocumentiModel dm = new DocumentiModel();
                        bool esisteFile = false;
                        bool gestisceEstensioni = false;
                        bool dimensioneConsentita = false;
                        string dimensioneMaxConsentita = string.Empty;

                        Utility.PreSetDocumento(file, out dm, out esisteFile, out gestisceEstensioni,
                            out dimensioneConsentita, out dimensioneMaxConsentita,
                            EnumTipoDoc.Formulario_Maggiorazioni_Familiari);

                        if (esisteFile)
                        {
                            if (gestisceEstensioni == false)
                            {
                                throw new Exception(
                                    "Il documento selezionato non è nel formato consentito. Il formato supportato è: pdf.");
                            }

                            if (dimensioneConsentita)
                            {
                                dtd.SetFormularioVariazioneMaggiorazioniFamiliari(ref dm, idMaggiorazioniFamiliari, db);
                            }
                            else
                            {
                                throw new Exception(
                                    "Il documento selezionato supera la dimensione massima consentita (" +
                                    dimensioneMaxConsentita + " Mb).");
                            }
                        }
                        else
                        {
                            throw new Exception("Il documento è obbligatorio.");
                        }
                    }

                    db.Database.CurrentTransaction.Commit();
                    return Json(new { msg = "Il formulario è stata inserito." });
                }
                catch (Exception ex)
                {

                    db.Database.CurrentTransaction.Rollback();
                    return Json(new { err = ex.Message });
                }
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ElencoDocumenti(decimal idFamiliare, EnumTipoDoc tipoDoc, EnumParentela parentela, EnumChiamante chiamante)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();
            ConiugeModel cm = new ConiugeModel();
            bool solaLettura = false;
            decimal idTrasferimento = 0;
            decimal idMaggiorazioniFamiliari = 0;


            try
            {

                using (dtDocumenti dtd = new dtDocumenti())
                {
                    ldm =
                        dtd.GetDocumentiByIdTable(idFamiliare, tipoDoc, parentela)
                            .OrderByDescending(a => a.dataInserimento)
                            .ToList();
                }

                switch (chiamante)
                {
                    case EnumChiamante.VariazioneMaggiorazioniFamiliari:

                        switch (parentela)
                        {
                            case EnumParentela.Coniuge:
                                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                                {
                                    var mfm = dtmf.GetMaggiorazioniFamiliaribyConiuge(idFamiliare);
                                    idMaggiorazioniFamiliari = mfm.idMaggiorazioniFamiliari;
                                    using (dtTrasferimento dtt = new dtTrasferimento())
                                    {
                                        idTrasferimento = dtt.GetTrasferimentoByIDMagFam(idMaggiorazioniFamiliari).idTrasferimento;
                                    }

                                }
                                break;
                            case EnumParentela.Figlio:
                                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                                {
                                    var mfm = dtmf.GetMaggiorazioniFamiliaribyFiglio(idFamiliare);
                                    idMaggiorazioniFamiliari = mfm.idMaggiorazioniFamiliari;
                                    using (dtTrasferimento dtt = new dtTrasferimento())
                                    {
                                        idTrasferimento = dtt.GetTrasferimentoByIDMagFam(idMaggiorazioniFamiliari).idTrasferimento;
                                    }
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("parentela");
                        }


                        using (dtVariazioniMaggiorazioneFamiliare dtmf = new dtVariazioniMaggiorazioneFamiliare())
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
                            bool inLavorazione = false;


                            if ((parentela == EnumParentela.Coniuge || parentela == EnumParentela.Figlio) && idMaggiorazioniFamiliari > 0)
                            {
                                dtmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                                    out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                                    out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli,
                                    out docFormulario, out inLavorazione);

                                if (richiestaAttivazione == true)
                                {
                                    solaLettura = true;
                                }
                                else
                                {
                                    solaLettura = false;
                                }
                            }
                            else
                            {
                                solaLettura = false;
                            }

                        }
                        break;
                }



            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewData.Add("id", idFamiliare);
            ViewData.Add("chiamante", chiamante);
            ViewData.Add("tipoDoc", tipoDoc);
            ViewData.Add("parentela", parentela);
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);
            ViewData.Add("solaLettura", solaLettura);
            ViewData.Add("idTrasferimento", idTrasferimento);

            return PartialView(ldm);
        }

        public ActionResult SostituisciDocumento(EnumTipoDoc tipoDoc, decimal id, EnumParentela parentela, EnumChiamante Chiamante, decimal idDocumento)
        {
            string titoloPagina = string.Empty;
            decimal idMaggiorazioniFamiliari = 0;


            switch (tipoDoc)
            {
                case EnumTipoDoc.Documento_Identita:
                    switch (parentela)
                    {
                        case EnumParentela.Coniuge:
                            titoloPagina = "Documento d'identità (Coniuge)";
                            using (dtConiuge dtc = new dtConiuge())
                            {
                                var cm = dtc.GetConiugebyID(id);
                                idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari;
                            }
                            break;
                        case EnumParentela.Figlio:
                            titoloPagina = "Documento d'identità (Figlio)";
                            using (dtFigli dtf = new dtFigli())
                            {
                                var fm = dtf.GetFigliobyID(id);
                                idMaggiorazioniFamiliari = fm.idMaggiorazioniFamiliari;
                            }
                            break;
                        case EnumParentela.Richiedente:
                            titoloPagina = "Documento d'identità (Richiedente)";

                            break;
                        default:
                            throw new ArgumentOutOfRangeException("parentela");
                    }


                    break;
                default:
                    throw new ArgumentOutOfRangeException("tipoDoc");
            }

            ViewData.Add("titoloPagina", titoloPagina);
            ViewData.Add("tipoDoc", (decimal)tipoDoc);
            ViewData.Add("ID", id);
            ViewData.Add("idDocumento", idDocumento);
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);
            ViewData.Add("parentela", (decimal)parentela);

            return PartialView();
        }

        public ActionResult NuovoDocumentoMagFam(EnumTipoDoc tipoDoc, decimal id, EnumParentela parentela, EnumChiamante Chiamante)
        {
            string titoloPagina = string.Empty;
            decimal idMaggiorazioniFamiliari = 0;


            switch (tipoDoc)
            {
                case EnumTipoDoc.Documento_Identita:
                    switch (parentela)
                    {
                        case EnumParentela.Coniuge:
                            titoloPagina = "Documento d'identità (Coniuge)";
                            using (dtConiuge dtc = new dtConiuge())
                            {
                                var cm = dtc.GetConiugebyID(id);
                                idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari;
                            }
                            break;
                        case EnumParentela.Figlio:
                            titoloPagina = "Documento d'identità (Figlio)";
                            using (dtFigli dtf = new dtFigli())
                            {
                                var fm = dtf.GetFigliobyID(id);
                                idMaggiorazioniFamiliari = fm.idMaggiorazioniFamiliari;
                            }
                            break;
                        case EnumParentela.Richiedente:
                            titoloPagina = "Documento d'identità (Richiedente)";

                            break;
                        default:
                            throw new ArgumentOutOfRangeException("parentela");
                    }


                    break;
                default:
                    throw new ArgumentOutOfRangeException("tipoDoc");
            }

            ViewData.Add("titoloPagina", titoloPagina);
            ViewData.Add("tipoDoc", (decimal)tipoDoc);
            ViewData.Add("ID", id);
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);
            ViewData.Add("parentela", (decimal)parentela);

            return PartialView();
        }

        public JsonResult EliminaDocumento(decimal idDocumento, EnumChiamante chiamante)
        {

            try
            {
                using (dtVariazioniMaggiorazioneFamiliare dtd = new dtVariazioniMaggiorazioneFamiliare())
                {
                    dtd.DeleteDocumentoMagFam(idDocumento, chiamante);
                }
            }
            catch (Exception ex)
            {

                return Json(new { errore = ex.Message, msg = "" });
            }

            return Json(new { errore = "", msg = "Eliminazione effettuata con successo." });
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ConfermaSostituisciDocumento(decimal idDoc, EnumTipoDoc tipoDoc, decimal idFamiliare, EnumParentela parentela)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    //throw new Exception("Simulazione errore");

                    foreach (string item in Request.Files)
                    {

                        HttpPostedFileBase file = Request.Files[item] as HttpPostedFileBase;

                        using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                        {
                            DocumentiModel dm = new DocumentiModel();
                            bool esisteFile = false;
                            bool gestisceEstensioni = false;
                            bool dimensioneConsentita = false;
                            string dimensioneMaxConsentita = string.Empty;

                            Utility.PreSetDocumento(file, out dm, out esisteFile, out gestisceEstensioni,
                                out dimensioneConsentita, out dimensioneMaxConsentita, tipoDoc);

                            if (esisteFile)
                            {
                                if (gestisceEstensioni == false)
                                {
                                    throw new Exception(
                                        "Il documento selezionato non è nel formato consentito. Il formato supportato è: pdf.");
                                }

                                if (dimensioneConsentita)
                                {

                                    switch (tipoDoc)
                                    {
                                        case EnumTipoDoc.Documento_Identita:
                                            switch (parentela)
                                            {
                                                case EnumParentela.Coniuge:
                                                    dtvmf.AssociaDocumentoConiuge(ref dm, idFamiliare, db);

                                                    var c = db.CONIUGE.Find(idFamiliare);
                                                    var att = c.ATTIVAZIONIMAGFAM.Where(x => x.ANNULLATO == false).OrderByDescending(x => x.IDATTIVAZIONEMAGFAM).First();
                                                    if (att.ATTIVAZIONEMAGFAM == false && att.RICHIESTAATTIVAZIONE == false)
                                                    {
                                                        dm.fk_documenti = idDoc;
                                                        if (db.SaveChanges() > 0)
                                                        {
                                                            dtvmf.AssociaDocumentoAttivazione(att.IDATTIVAZIONEMAGFAM, dm.idDocumenti, db);
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("Non è stato possibile aggiornare il documento."));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        att.ANNULLATO = true;

                                                        //crea una nuova attivazione
                                                        ATTIVAZIONIMAGFAM newamf = new ATTIVAZIONIMAGFAM()
                                                        {
                                                            IDMAGGIORAZIONIFAMILIARI = att.IDMAGGIORAZIONIFAMILIARI,
                                                            RICHIESTAATTIVAZIONE = false,
                                                            DATARICHIESTAATTIVAZIONE = null,
                                                            ATTIVAZIONEMAGFAM = false,
                                                            DATAATTIVAZIONEMAGFAM = null,
                                                            ANNULLATO = false,
                                                            DATAVARIAZIONE = DateTime.Now,
                                                            DATAAGGIORNAMENTO = DateTime.Now,
                                                        };
                                                        db.ATTIVAZIONIMAGFAM.Add(newamf);
                                                        if (db.SaveChanges() > 0)
                                                        {
                                                            dtvmf.AssociaDocumentoAttivazione(newamf.IDATTIVAZIONEMAGFAM, dm.idDocumenti, db);
                                                        }
                                                    }
                                                    break;
                                                case EnumParentela.Figlio:
                                                    //dtd.AddDocumentoFromFiglio(ref dm, id, db);
                                                    break;
                                                default:
                                                    throw new ArgumentOutOfRangeException("parentela");
                                            }
                                            break;

                                        default:
                                            throw new ArgumentOutOfRangeException("tipoDoc");
                                    }
                                }
                                else
                                {
                                    throw new Exception(
                                        "Il documento selezionato supera la dimensione massima consentita (" +
                                        dimensioneMaxConsentita + " Mb).");
                                }
                            }
                            else
                            {
                                throw new Exception("Il documento è obbligatorio.");
                            }

                        }


                    }

                    db.Database.CurrentTransaction.Commit();
                    return Json(new { });

                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();

                    return Json(new { error = ex.Message });
                };

            }
        }



    }
}
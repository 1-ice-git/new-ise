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
using NewISE.Interfacce;

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
                bool trasfSolaLettura = false;


                dtmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                    out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                    out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione, out trasfSolaLettura);

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

        public ActionResult AttivitaMaggiorazioneFamiliare(decimal idTrasferimento)
        {
            try
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
                bool trasfSolaLettura = false;

                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    var tr = idTrasferimento;

                    if (!tr.Equals(null))
                    {
                        ViewData.Add("idTrasferimento", tr);

                        dtvmf.SituazioneMagFamVariazione(idTrasferimento, out rinunciaMagFam,
                                                out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                                                out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione, out trasfSolaLettura);

                        ViewData.Add("rinunciaMagFam", rinunciaMagFam);

                    }
                    else
                    {
                        throw new Exception("Nessun trasferimento impostato.");
                    }
                }
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult ElencoFamiliari(decimal idMaggiorazioniFamiliari)
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
            bool trasfSolaLettura = false;


            var solaLettura = 0;

            using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
            {
                dtvmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                                                out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                                                out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione, out trasfSolaLettura);

                if (richiestaAttivazione && attivazione == false)
                {
                    solaLettura = 1;
                }
            }

            List<VariazioneElencoFamiliariModel> lefm = new List<VariazioneElencoFamiliariModel>();

            try
            {
                //Lista Coniugi
                using (dtConiuge dtc = new dtConiuge())
                {
                    List<VariazioneConiugeModel> lcm = dtc.GetListaAttivazioniConiugeByIdMagFam(idMaggiorazioniFamiliari).ToList();

                    var check_nuovo_coniuge = 1;

                    if (lcm?.Any() ?? false)
                    {
                        using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                        {
                            using (dtPensione dtp = new dtPensione())
                            {
                                foreach (var e in lcm)
                                {
                                    VariazioneElencoFamiliariModel efm = new VariazioneElencoFamiliariModel()
                                    {
                                        idMaggiorazioniFamiliari = e.idMaggiorazioniFamiliari,
                                        idFamiliare = e.idConiuge,
                                        Nominativo = e.cognome + " " + e.nome,
                                        CodiceFiscale = e.codiceFiscale,
                                        dataInizio = e.dataInizio,
                                        dataFine = e.dataFine,
                                        parentela = EnumParentela.Coniuge,
                                        idAltriDati = dtvmf.GetAltriDatiFamiliariConiuge(e.idConiuge).idAltriDatiFam,
                                        Documenti = dtvmf.GetDocumentiByIdTable_MF(e.idConiuge, EnumTipoDoc.Documento_Identita, EnumParentela.Coniuge, idMaggiorazioniFamiliari),
                                        HasPensione = dtp.HasPensione(e.idConiuge),
                                        eliminabile = e.eliminabile
                                    };
                                    lefm.Add(efm);
                                    if (efm.dataFine == Utility.DataFineStop())
                                    {
                                        check_nuovo_coniuge = 0;
                                    }
                                }
                            }
                        }
                    }
                    ViewData.Add("check_nuovo_coniuge", check_nuovo_coniuge);
                }

                // Lista Figli
                using (dtFigli dtf = new dtFigli())
                {
                    List<VariazioneFigliModel> lfm = dtf.GetListaAttivazioniFigliByIdMagFam(idMaggiorazioniFamiliari).ToList();

                    var check_nuovo_figlio = 1;

                    if (lfm?.Any() ?? false)
                    {
                        using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                        {
                            using (dtPensione dtp = new dtPensione())
                            {
                                foreach (var e in lfm)
                                {
                                    VariazioneElencoFamiliariModel efm = new VariazioneElencoFamiliariModel()
                                    {
                                        idMaggiorazioniFamiliari = e.idMaggiorazioniFamiliari,
                                        idFamiliare = e.idFigli,
                                        Nominativo = e.cognome + " " + e.nome,
                                        CodiceFiscale = e.codiceFiscale,
                                        dataInizio = e.dataInizio,
                                        dataFine = e.dataFine,
                                        parentela = EnumParentela.Figlio,
                                        idAltriDati = dtvmf.GetAltriDatiFamiliariFiglio(e.idFigli, idMaggiorazioniFamiliari).idAltriDatiFam,
                                        Documenti = dtvmf.GetDocumentiByIdTable_MF(e.idFigli, EnumTipoDoc.Documento_Identita, EnumParentela.Figlio, idMaggiorazioniFamiliari),
                                        HasPensione = dtp.HasPensione(e.idFigli),
                                        eliminabile = e.eliminabile
                                    };
                                    lefm.Add(efm);
                                    if (efm.dataFine == Utility.DataFineStop())
                                    {
                                        check_nuovo_figlio = 0;
                                    }
                                }
                            }
                        }
                    }
                    ViewData.Add("check_nuovo_figlio", check_nuovo_figlio);
                }

                ViewData.Add("solaLettura", solaLettura);
                ViewData.Add("trasfSolaLettura", trasfSolaLettura);

                ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);

                return PartialView(lefm);

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
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

            using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
            {
                var idTrasferimento = dtvmf.GetIdTrasferimento(idMaggiorazioniFamiliari);
                ViewData.Add("idTrasferimento", idTrasferimento);
                var idAttivazioneMagFam = dtvmf.GetAttivazioneById(idMaggiorazioniFamiliari, EnumTipoTabella.MaggiorazioniFamiliari).IDATTIVAZIONEMAGFAM;
                ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);
            }

            ViewBag.lTipologiaConiuge = lTipologiaConiuge;
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);

            return PartialView();
        }

        public ActionResult NuovoFiglio(decimal idMaggiorazioniFamiliari)
        {
            List<SelectListItem> lTipologiaFiglio = new List<SelectListItem>();

            var r = new List<SelectListItem>();

            try
            {
                using (dtTipologiaFiglio dttf = new dtTipologiaFiglio())
                {
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
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
            {
                var idTrasferimento = dtvmf.GetIdTrasferimento(idMaggiorazioniFamiliari);
                ViewData.Add("idTrasferimento", idTrasferimento);
                var idAttivazioneMagFam = dtvmf.GetAttivazioneById(idMaggiorazioniFamiliari, EnumTipoTabella.MaggiorazioniFamiliari).IDATTIVAZIONEMAGFAM;
                ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);
            }

            ViewBag.lTipologiaFiglio = lTipologiaFiglio;
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);

            return PartialView();
        }


        public ActionResult ModificaConiuge(decimal idConiuge)
        {
            ConiugeModel cm = new ConiugeModel();

            try
            {
                using (dtConiuge dtc = new dtConiuge())
                {
                    using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                    {
                        cm = dtc.GetConiugebyID(idConiuge);
                        cm.idAttivazioneMagFam = dtvmf.GetAttivazioneById(idConiuge, EnumTipoTabella.Coniuge).IDATTIVAZIONEMAGFAM;
                    }
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
                    using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                    {
                        fm = dtf.GetFigliobyID(idFiglio);
                        fm.idAttivazioneMagFam = dtvmf.GetAttivazioneById(idFiglio, EnumTipoTabella.Figli).IDATTIVAZIONEMAGFAM;
                    }
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
            return RedirectToAction("ElencoFamiliari", new { idMaggiorazioniFamiliari = fm.idMaggiorazioniFamiliari });
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
            bool siPensioniConiuge = false;
            bool solaLettura = true;
            bool trasfSolaLettura = false;

            using (dtVariazioniMaggiorazioneFamiliare dtmf = new dtVariazioniMaggiorazioneFamiliare())
            {
                dtmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione, out trasfSolaLettura);
            }

            List<SelectListItem> lDataAttivazione = new List<SelectListItem>();
            List<AttivazioniMagFamModel> lamf = new List<AttivazioniMagFamModel>();

            try
            {
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                    {
                        lamf = dtamf.GetListAttivazioniMagFamByIdMagFam(idMaggiorazioniFamiliari).ToList();

                        var i = 1;

                        foreach (var e in lamf)
                        {
                            if (!e.annullato)
                            {
                                dtvmf.SituazioneAttivazioneMagFamById(e.idAttivazioneMagFam, out rinunciaMagFam,
                                        out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                                        out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out siPensioniConiuge, out docFormulario);

                                if (
                                    //(richiestaAttivazione && attivazione == false) ||
                                    (richiestaAttivazione == false && datiFigli) ||
                                    (richiestaAttivazione == false && datiParzialiFigli == false) ||
                                    (richiestaAttivazione == false && siDocFigli) ||
                                    (richiestaAttivazione == false && datiConiuge) ||
                                    (richiestaAttivazione == false && datiParzialiConiuge == false) ||
                                    (richiestaAttivazione == false && siDocConiuge) ||
                                    (richiestaAttivazione == false && docFormulario) ||
                                    (richiestaAttivazione == false && siPensioniConiuge))
                                {
                                    lDataAttivazione.Insert(0, new SelectListItem() { Text = "(" + i.ToString() + ") " + e.dataVariazione.ToString() + " (In Lavorazione)", Value = e.idAttivazioneMagFam.ToString() });
                                    solaLettura = false;
                                }
                                if (richiestaAttivazione)
                                {
                                    lDataAttivazione.Insert(0, new SelectListItem() { Text = "(" + i.ToString() + ") " + e.dataVariazione.ToString(), Value = e.idAttivazioneMagFam.ToString() });
                                }
                                i++;
                            }
                        }
                        lDataAttivazione.Insert(0, new SelectListItem() { Text = "(TUTTE)", Value = "" });
                        ViewData.Add("lDataAttivazione", lDataAttivazione);
                        ViewData["idMaggiorazioniFamiliari"] = idMaggiorazioniFamiliari;
                        ViewData.Add("solaLettura", solaLettura);
                    }
                }
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
        public ActionResult ElencoDocumentiFormularioPS()
        {
            return PartialView();
        }


        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult AltriDatiFamiliariConiuge(decimal idConiuge)
        {
            AltriDatiFamConiugeModel adf = new AltriDatiFamConiugeModel();
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();
            ATTIVAZIONIMAGFAM amf = new ATTIVAZIONIMAGFAM();

            try
            {
                decimal idMaggiorazioniFamiliari = 0;

                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    idMaggiorazioniFamiliari = dtvmf.GetMaggiorazioneFamiliareConiuge(idConiuge);

                    adf = dtvmf.GetAltriDatiFamiliariConiuge(idConiuge);

                    amf = dtvmf.GetAttivazioneById(idConiuge, EnumTipoTabella.Coniuge);

                    ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);
                    ViewData.Add("idAttivazione", amf.IDATTIVAZIONEMAGFAM);
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
                    bool solaLettura = false;
                    bool trasfSolaLettura = false;

                    dtmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                        out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                        out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione, out trasfSolaLettura);

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

                //using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                //{
                //    PercentualeMagConiugeModel pc = dtpc.GetPercMagConiugeNow(idConiuge, DateTime.Now.Date);

                //    if (pc != null && pc.HasValue())
                //    {
                //        switch (pc.idTipologiaConiuge)
                //        {
                //            case EnumTipologiaConiuge.Residente:
                //                adf.residente = true;
                //                adf.ulterioreMagConiuge = false;
                //                break;

                //            case EnumTipologiaConiuge.NonResidente_A_Carico:
                //                adf.residente = false;
                //                adf.ulterioreMagConiuge = true;
                //                break;

                //            default:
                //                break;
                //        }
                //    }
                //}
                using (dtConiuge dtc = new dtConiuge())
                {
                    ConiugeModel c = dtc.GetConiugebyID(idConiuge);

                    if (c != null && c.HasValue())
                    {
                        switch (c.idTipologiaConiuge)
                        {
                            case EnumTipologiaConiuge.Residente:
                                adf.residente = true;
                                adf.ulterioreMagConiuge = false;
                                break;

                            case EnumTipologiaConiuge.NonResidente_A_Carico:
                                adf.residente = false;
                                adf.ulterioreMagConiuge = true;
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


            if (adf != null && adf.HasValue())
            {
                using (dtConiuge dtc = new dtConiuge())
                {

                    var cm = dtc.GetConiugebyID(adf.idConiuge);
                    adf.Coniuge = cm;

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

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult AltriDatiFamiliariFiglio(decimal idFiglio)
        {
            AltriDatiFamFiglioModel adf = new AltriDatiFamFiglioModel();
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();
            ATTIVAZIONIMAGFAM amf = new ATTIVAZIONIMAGFAM();

            try
            {
                decimal idMaggiorazioniFamiliari = 0;

                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    idMaggiorazioniFamiliari = dtvmf.GetMaggiorazioneFamiliareFiglio(idFiglio);

                    adf = dtvmf.GetAltriDatiFamiliariFiglio(idFiglio, idMaggiorazioniFamiliari);

                    amf = dtvmf.GetAttivazioneById(idFiglio, EnumTipoTabella.Figli);

                    ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);
                    ViewData.Add("idAttivazione", amf.IDATTIVAZIONEMAGFAM);
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
                    bool solaLettura = false;
                    bool trasfSolaLettura = false;

                    dtmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                        out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                        out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione, out trasfSolaLettura);

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

                //using (dtPercentualeMagFigli dtpf = new dtPercentualeMagFigli())
                //{
                //    PercentualeMagFigliModel pf = dtpf.GetPercentualeMaggiorazioneFigli(idFiglio, DateTime.Now.Date);

                //    if (pf != null && pf.HasValue())
                //    {
                //        switch (pf.idTipologiaFiglio)
                //        {
                //            case EnumTipologiaFiglio.Residente:
                //                adf.residente = true;
                //                adf.studente = false;
                //                break;

                //            case EnumTipologiaFiglio.StudenteResidente:
                //                adf.studente = true;
                //                adf.residente = true;
                //                break;
                //            case EnumTipologiaFiglio.StudenteNonResidente:
                //                adf.studente = true;
                //                adf.residente = false;
                //                break;

                //            default:
                //                break;
                //        }
                //    }
                //}
                using (dtFigli dtf = new dtFigli())
                {
                    FigliModel f = dtf.GetFigliobyID(idFiglio);
                    if (f != null && f.HasValue())
                    {
                        switch (f.idTipologiaFiglio)
                        {
                            case EnumTipologiaFiglio.Residente:
                                adf.residente = true;
                                adf.studente = false;
                                break;
                            case EnumTipologiaFiglio.StudenteResidente:
                                adf.studente = true;
                                adf.residente = true;
                                break;
                            case EnumTipologiaFiglio.StudenteNonResidente:
                                adf.studente = true;
                                adf.residente = false;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            if (adf != null && adf.HasValue())
            {
                using (dtFigli dtf = new dtFigli())
                {

                    var fm = dtf.GetFigliobyID(adf.idFigli);
                    adf.Figli = fm;

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

                adf.idFigli = idFiglio;
                ViewData.Add("Comuni", comuni);

                return PartialView("InserisciAltriDatiFamiliariFiglio", adf);
            }
        }

        public ActionResult ModificaAltriDatiFamiliariConiuge(decimal idAltriDatiFam, decimal idMaggiorazioniFamiliari, decimal idAttivazione)
        {
            AltriDatiFamConiugeModel adfm = new AltriDatiFamConiugeModel();

            try
            {
                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                {
                    adfm = dtadf.GetAltriDatiFamiliariConiuge(idAltriDatiFam);
                    //if (adfm != null && adfm.HasValue())
                    //{
                    //    using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                    //    {
                    //        PercentualeMagConiugeModel pc = new PercentualeMagConiugeModel();

                    //        pc = dtpc.GetPercMagConiugeNow(adfm.idConiuge, DateTime.Now.Date);

                    //        if (pc != null && pc.HasValue())
                    //        {
                    //            switch (pc.idTipologiaConiuge)
                    //            {
                    //                case EnumTipologiaConiuge.Residente:
                    //                    adfm.residente = true;
                    //                    adfm.ulterioreMagConiuge = false;
                    //                    break;

                    //                case EnumTipologiaConiuge.NonResidente_A_Carico:
                    //                    adfm.residente = false;
                    //                    adfm.ulterioreMagConiuge = true;
                    //                    break;

                    //                default:
                    //                    break;
                    //            }
                    //        }
                    //    }
                    //}
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
            ViewData.Add("idAttivazione", idAttivazione);

            return PartialView(adfm);
        }

        public ActionResult ModificaAltriDatiFamiliariFiglio(decimal idAltriDatiFam, decimal idMaggiorazioniFamiliari, decimal idAttivazione)
        {
            AltriDatiFamFiglioModel adfm = new AltriDatiFamFiglioModel();

            try
            {
                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                {
                    adfm = dtadf.GetAltriDatiFamiliariFiglio(idAltriDatiFam);
                    //if (adfm != null && adfm.HasValue())
                    //{
                    //    using (dtPercentualeMagFigli dtpf = new dtPercentualeMagFigli())
                    //    {
                    //        PercentualeMagFigliModel pf = new PercentualeMagFigliModel();

                    //        pf = dtpf.GetPercentualeMaggiorazioneFigli(adfm.idFigli, DateTime.Now.Date);

                    //        if (pf != null && pf.HasValue())
                    //        {
                    //            switch (pf.idTipologiaFiglio)
                    //            {
                    //                case EnumTipologiaFiglio.Residente:
                    //                    adfm.residente = true;
                    //                    adfm.studente = false;
                    //                    break;

                    //                case EnumTipologiaFiglio.StudenteResidente:
                    //                    adfm.studente = true;
                    //                    adfm.residente = true;
                    //                    break;
                    //                case EnumTipologiaFiglio.StudenteNonResidente:
                    //                    adfm.studente = true;
                    //                    adfm.residente = false;
                    //                    break;

                    //                default:
                    //                    break;
                    //            }
                    //        }
                    //    }
                    //}
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
            ViewData.Add("idAttivazione", idAttivazione);

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

        public ActionResult ConfermaModificaAdfFiglio(AltriDatiFamFiglioModel adfm)
        {
            try
            {
                adfm.dataAggiornamento = DateTime.Now;
                adfm.annullato = false;

                if (ModelState.IsValid)
                {
                    using (dtVariazioniMaggiorazioneFamiliare dtadf = new dtVariazioniMaggiorazioneFamiliare())
                    {
                        dtadf.EditVariazioneAltriDatiFamiliariFiglio(adfm);
                    }
                }
                else
                {
                    return PartialView("ModificaAltriDatiFamiliariFiglio", adfm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return RedirectToAction("AltriDatiFamiliariFiglio", new { idFiglio = adfm.idFigli });
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ElencoPensioniConiuge(decimal idConiuge)
        {
            List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();

            try
            {
                lpcm = GetPensioniConiuge(idConiuge).ToList();

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
                        bool trasfSolaLettura = false;

                        dtmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                            out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                            out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione, out trasfSolaLettura);

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
            ViewData.Add("idConiuge", idConiuge);

            return PartialView(lpcm);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NuovoImportoPensione(decimal idConiuge)
        {
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
                    using (ModelDBISE db = new ModelDBISE())
                    {
                        using (dtPensione dtp = new dtPensione())
                        {   
                            using (dtVariazioniMaggiorazioneFamiliare dtamf = new dtVariazioniMaggiorazioneFamiliare())
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

                                ATTIVAZIONIMAGFAM attmf_aperta = new ATTIVAZIONIMAGFAM();

                                var attmf_rif = dtamf.GetAttivazioneById(idConiuge, EnumTipoTabella.Coniuge);

                                var attmf = dtamf.GetAttivazioneAperta(attmf_rif.IDMAGGIORAZIONIFAMILIARI);

                                // se non esiste attivazione aperta la creo altrimenti la uso
                                if (attmf.IDATTIVAZIONEMAGFAM == 0)
                                {
                                    ATTIVAZIONIMAGFAM new_amf = dtamf.CreaAttivazione(attmf_rif.IDMAGGIORAZIONIFAMILIARI, db);
                                    attmf_aperta = new_amf;
                                }
                                else
                                {
                                    attmf_aperta = attmf;
                                }

                                decimal idTrasf = attmf_aperta.IDMAGGIORAZIONIFAMILIARI;

                                dtp.SetNuovoImportoPensione(pcm, idConiuge, attmf_aperta.IDATTIVAZIONEMAGFAM, db);
                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento nuovo importo pensione coniuge (" + idConiuge + ")", "PENSIONI", db, idTrasf, pcm.idPensioneConiuge);
                            }
                        }

                    }
                }
                else
                {
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
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    using (dtPensione dtp = new dtPensione())
                    {
                        pcm = dtp.GetPensioneByID(idPensione);

                        if (pcm != null && pcm.HasValue())
                        {
                            var att = dtvmf.GetAttivazioneById(idConiuge, EnumTipoTabella.Coniuge);

                            dtp.EliminaImportoPensione(pcm, idConiuge, att.IDATTIVAZIONEMAGFAM);
                        }
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
            bool siPensioniConiuge = false;
            bool docFormulario = false;
            bool inLavorazione = false;
            bool CheckNotifica = true;
            bool trasfSolaLettura = false;

            try
            {
                amministratore = Utility.Amministratore();

                using (dtVariazioniMaggiorazioneFamiliare dtmf = new dtVariazioniMaggiorazioneFamiliare())
                {

                    var amf = dtmf.GetAttivazioneById(idMaggiorazioniFamiliari, EnumTipoTabella.MaggiorazioniFamiliari);

                    dtmf.SituazioneAttivazioneMagFamById(amf.IDATTIVAZIONEMAGFAM, out rinunciaMagFam,
                            out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                            out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out siPensioniConiuge, out docFormulario);

                    if (richiestaAttivazione)
                    {
                        CheckNotifica = false;
                    }
                    if (datiFigli && (datiParzialiFigli || siDocFigli == false))
                    {
                        CheckNotifica = false;
                    }
                    if (datiConiuge && (datiParzialiConiuge || siDocConiuge == false))
                    {
                        CheckNotifica = false;
                    }
                    if (datiConiuge == false && datiFigli == false && siPensioniConiuge == false && docFormulario == false)
                    {
                        CheckNotifica = false;
                    }

                    dtmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                        out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                        out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione, out trasfSolaLettura);
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
                        CheckNotifica = CheckNotifica,
                        trasfSolaLettura = trasfSolaLettura,
                        err = errore
                    });

        }

        [HttpPost]
        public JsonResult ConfermaNotificaRichiestaVariazione(decimal idMaggiorazioniFamiliari)
        {
            string errore = "";

            try
            {
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                    {
                        var amf = dtvmf.GetAttivazioneById(idMaggiorazioniFamiliari, EnumTipoTabella.MaggiorazioniFamiliari);

                        dtmf.NotificaRichiestaVariazione(amf.IDATTIVAZIONEMAGFAM);
                    }
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
        public JsonResult ConfermaAttivaRichiestaVariazione(decimal idMaggiorazioniFamiliari)
        {
            string errore = "";

            try
            {
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    var amf = dtvmf.GetAttivazioneById(idMaggiorazioniFamiliari, EnumTipoTabella.MaggiorazioniFamiliari);

                    dtvmf.AttivaRichiestaVariazione(amf.IDATTIVAZIONEMAGFAM, idMaggiorazioniFamiliari);
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
        [ValidateInput(false)]
        public JsonResult ConfermaAnnullaRichiestaVariazione(decimal idMaggiorazioniFamiliari, string msg)
        {
            string errore = "";
            decimal idAttivazioneMagFamNew = 0;

            try
            {
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                    {
                        var amf = dtvmf.GetAttivazioneById(idMaggiorazioniFamiliari, EnumTipoTabella.MaggiorazioniFamiliari);

                        dtmf.AnnullaRichiesta(amf.IDATTIVAZIONEMAGFAM, out idAttivazioneMagFamNew, msg);
                    }
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
        public ActionResult ElencoDocumenti(decimal idFamiliare, EnumTipoDoc tipoDoc, EnumParentela parentela, EnumChiamante chiamante, decimal idMaggiorazioniFamiliari)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();
            ConiugeModel cm = new ConiugeModel();
            bool solaLettura = false;

            try
            {
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    ldm = dtvmf.GetDocumentiByIdTable_MF(idFamiliare, tipoDoc, parentela, idMaggiorazioniFamiliari)
                            .OrderByDescending(a => a.dataInserimento)
                            .ToList();

                    switch (chiamante)
                    {
                        case EnumChiamante.Variazione_Maggiorazioni_Familiari:
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
                            bool trasfSolaLettura = false;

                            if ((parentela == EnumParentela.Coniuge || parentela == EnumParentela.Figlio) && idMaggiorazioniFamiliari > 0)
                            {
                                dtvmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                                    out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                                    out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli,
                                    out docFormulario, out inLavorazione, out trasfSolaLettura);

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
                            break;
                    }
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
            ViewData.Add("idTrasferimento", idMaggiorazioniFamiliari);

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

            using (dtVariazioniMaggiorazioneFamiliare dtmf = new dtVariazioniMaggiorazioneFamiliare())
            {
                switch (tipoDoc)
                {
                    case EnumTipoDoc.Documento_Identita:
                        switch (parentela)
                        {
                            case EnumParentela.Coniuge:
                                titoloPagina = "Documento d'identità (Coniuge)";
                                idMaggiorazioniFamiliari = dtmf.GetAttivazioneById(id, EnumTipoTabella.Coniuge).IDMAGGIORAZIONIFAMILIARI;
                                break;

                            case EnumParentela.Figlio:
                                titoloPagina = "Documento d'identità (Figlio)";
                                idMaggiorazioniFamiliari = dtmf.GetAttivazioneById(id, EnumTipoTabella.Figli).IDMAGGIORAZIONIFAMILIARI;
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

                    foreach (string item in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[item] as HttpPostedFileBase;

                        using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                        {
                            VariazioneDocumentiModel dm = new VariazioneDocumentiModel();
                            bool esisteFile = false;
                            bool gestisceEstensioni = false;
                            bool dimensioneConsentita = false;
                            string dimensioneMaxConsentita = string.Empty;

                            PreSetVariazioneDocumento(file, out dm, out esisteFile, out gestisceEstensioni,
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
                                                    dm.fk_iddocumento = idDoc;
                                                    dtvmf.AssociaDocumentoConiuge(ref dm, idFamiliare, db);

                                                    var att_coniuge = dtvmf.GetAttivazioneById(idFamiliare, EnumTipoTabella.Coniuge);
                                                    if (att_coniuge.ATTIVAZIONEMAGFAM == false && att_coniuge.RICHIESTAATTIVAZIONE == false)
                                                    {
                                                        dtvmf.AssociaDocumentoAttivazione(att_coniuge.IDATTIVAZIONEMAGFAM, dm.idDocumenti, db);
                                                    }
                                                    else
                                                    {
                                                        var idamf = dtvmf.GetMaggiorazioneFamiliareConiuge(idFamiliare);
                                                        var last_att = dtvmf.GetAttivazioneById(idamf, EnumTipoTabella.MaggiorazioniFamiliari);

                                                        if (last_att.RICHIESTAATTIVAZIONE)
                                                        {
                                                            var idMaggiorazioniFamiliari = dtvmf.GetMaggiorazioneFamiliareDocumento(idDoc);
                                                            var newamf = dtvmf.CreaAttivazione(idMaggiorazioniFamiliari,db);

                                                            dtvmf.AssociaDocumentoAttivazione(newamf.IDATTIVAZIONEMAGFAM, dm.idDocumenti, db);
                                                        }
                                                        else
                                                        {
                                                            dtvmf.AssociaDocumentoAttivazione(last_att.IDATTIVAZIONEMAGFAM, dm.idDocumenti, db);
                                                        }
                                                    }
                                                    break;

                                                case EnumParentela.Figlio:
                                                    dm.fk_iddocumento = idDoc;
                                                    dtvmf.AssociaDocumentoFiglio(ref dm, idFamiliare, db);

                                                    var att_figlio = dtvmf.GetAttivazioneById(idFamiliare, EnumTipoTabella.Figli);
                                                    if (att_figlio.ATTIVAZIONEMAGFAM == false && att_figlio.RICHIESTAATTIVAZIONE == false)
                                                    {
                                                        dtvmf.AssociaDocumentoAttivazione(att_figlio.IDATTIVAZIONEMAGFAM, dm.idDocumenti, db);
                                                    }
                                                    else
                                                    {
                                                        var idamf = dtvmf.GetMaggiorazioneFamiliareConiuge(idFamiliare);
                                                        var last_att = dtvmf.GetAttivazioneById(idamf, EnumTipoTabella.MaggiorazioniFamiliari);

                                                        if (last_att.RICHIESTAATTIVAZIONE)
                                                        {
                                                            var idMaggiorazioniFamiliari = dtvmf.GetMaggiorazioneFamiliareDocumento(idDoc);
                                                            var newamf = dtvmf.CreaAttivazione(idMaggiorazioniFamiliari,db);

                                                            dtvmf.AssociaDocumentoAttivazione(newamf.IDATTIVAZIONEMAGFAM, dm.idDocumenti, db);
                                                        }
                                                        else
                                                        {
                                                            dtvmf.AssociaDocumentoAttivazione(last_att.IDATTIVAZIONEMAGFAM, dm.idDocumenti, db);
                                                        }
                                                    }
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalvaNuovoDocumentoMF(EnumTipoDoc tipoDoc, decimal id, EnumParentela parentela)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    foreach (string item in Request.Files)
                    {

                        HttpPostedFileBase file = Request.Files[item] as HttpPostedFileBase;

                        using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                        {
                            using (dtDocumenti dtd = new dtDocumenti())
                            {
                                VariazioneDocumentiModel dm = new VariazioneDocumentiModel();
                                bool esisteFile = false;
                                bool gestisceEstensioni = false;
                                bool dimensioneConsentita = false;
                                string dimensioneMaxConsentita = string.Empty;

                                PreSetVariazioneDocumento(file, out dm, out esisteFile, out gestisceEstensioni,
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
                                        ATTIVAZIONIMAGFAM att = new ATTIVAZIONIMAGFAM();

                                        switch (tipoDoc)
                                        {
                                            case EnumTipoDoc.Documento_Identita:
                                                switch (parentela)
                                                {
                                                    case EnumParentela.Coniuge:
                                                        dtvmf.AddDocumentoFromConiuge(ref dm, id, db);
                                                        att = dtvmf.GetAttivazioneById(id, EnumTipoTabella.Coniuge);
                                                        if (att.ATTIVAZIONEMAGFAM == false && att.RICHIESTAATTIVAZIONE == false)
                                                        {
                                                            dtvmf.AssociaDocumentoAttivazione(att.IDATTIVAZIONEMAGFAM, dm.idDocumenti, db);
                                                        }
                                                        else
                                                        {
                                                            var idMF = dtvmf.GetMaggiorazioneFamiliareConiuge(id);
                                                            var last_att = dtvmf.GetAttivazioneById(idMF, EnumTipoTabella.MaggiorazioniFamiliari);

                                                            if (last_att.RICHIESTAATTIVAZIONE)
                                                            {
                                                                //att.ANNULLATO = true;
                                                                var newamf = dtvmf.CreaAttivazione(idMF,db);

                                                                if (newamf.IDATTIVAZIONEMAGFAM > 0)
                                                                {
                                                                    dtvmf.AssociaDocumentoAttivazione(newamf.IDATTIVAZIONEMAGFAM, dm.idDocumenti, db);
                                                                }
                                                                else
                                                                {
                                                                    throw new Exception("Impossibile salvare il documento.");
                                                                }
                                                            }
                                                            else
                                                            {
                                                                dtvmf.AssociaDocumentoAttivazione(last_att.IDATTIVAZIONEMAGFAM, dm.idDocumenti, db);
                                                            }
                                                        }
                                                        break;

                                                    case EnumParentela.Figlio:
                                                        dtvmf.AddDocumentoFromFiglio(ref dm, id, db);
                                                        att = dtvmf.GetAttivazioneById(id, EnumTipoTabella.Figli);
                                                        if (att.ATTIVAZIONEMAGFAM == false && att.RICHIESTAATTIVAZIONE == false)
                                                        {
                                                            dtvmf.AssociaDocumentoAttivazione(att.IDATTIVAZIONEMAGFAM, dm.idDocumenti, db);
                                                        }
                                                        else
                                                        {
                                                            var idMF = dtvmf.GetMaggiorazioneFamiliareConiuge(id);
                                                            var last_att = dtvmf.GetAttivazioneById(idMF, EnumTipoTabella.MaggiorazioniFamiliari);

                                                            if (last_att.RICHIESTAATTIVAZIONE)
                                                            {
                                                                //att.ANNULLATO = true;
                                                                var newamf = dtvmf.CreaAttivazione(idMF,db);

                                                                if (newamf.IDATTIVAZIONEMAGFAM > 0)
                                                                {
                                                                    dtvmf.AssociaDocumentoAttivazione(newamf.IDATTIVAZIONEMAGFAM, dm.idDocumenti, db);
                                                                }
                                                                else
                                                                {
                                                                    throw new Exception("Impossibile salvare il documento.");
                                                                }
                                                            }
                                                            else
                                                            {
                                                                dtvmf.AssociaDocumentoAttivazione(last_att.IDATTIVAZIONEMAGFAM, dm.idDocumenti, db);
                                                            }
                                                        }
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

        [HttpPost]
        public JsonResult NumeroDocumentiSalvatiMF(decimal id, EnumTipoDoc tipoDoc, EnumParentela parentela, decimal idAttivitaMagFam = 0)
        {
            int nDoc = 0;

            try
            {
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    nDoc = dtvmf.GetDocumentiById(id, tipoDoc, parentela).Count;
                }
            }
            catch (Exception ex)
            {
                return Json(new { errore = ex.Message, nDoc = 0 });
            }
            return Json(new { errore = "", nDoc = nDoc });
        }

        public static void PreSetVariazioneDocumento(HttpPostedFileBase file, out VariazioneDocumentiModel dm, out bool esisteFile, out bool gestisceEstensioni, out bool dimensioneConsentita, out string dimensioneMaxDocumento, EnumTipoDoc tipoDoc)
        {
            dm = new VariazioneDocumentiModel();
            gestisceEstensioni = false;
            dimensioneConsentita = false;
            esisteFile = false;
            dimensioneMaxDocumento = string.Empty;

            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    esisteFile = true;

                    var estensioniGestite = new[] { ".pdf" };
                    var estensione = Path.GetExtension(file.FileName);
                    var nomeFileNoEstensione = Path.GetFileNameWithoutExtension(file.FileName);
                    if (!estensioniGestite.Contains(estensione.ToLower()))
                    {
                        gestisceEstensioni = false;
                    }
                    else
                    {
                        gestisceEstensioni = true;
                    }
                    var keyDimensioneDocumento = System.Configuration.ConfigurationManager.AppSettings["DimensioneDocumento"];

                    dimensioneMaxDocumento = keyDimensioneDocumento;

                    if (file.ContentLength / 1024 <= Convert.ToInt32(keyDimensioneDocumento))
                    {
                        dm.nomeDocumento = nomeFileNoEstensione;
                        dm.estensione = estensione;
                        dm.tipoDocumento = tipoDoc;
                        dm.dataInserimento = DateTime.Now;
                        dm.file = file;
                        dimensioneConsentita = true;
                    }
                    else
                    {
                        dimensioneConsentita = false;
                    }

                }
                else
                {
                    esisteFile = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<PensioneConiugeModel> GetPensioniConiuge(decimal idConiuge)
        {
            List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();
            List<ConiugeModel> lcm = new List<ConiugeModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    lpcm = dtvmf.GetListaPensioniConiugeByIdMagFam(idConiuge);

                    if (lpcm?.Any() ?? false)
                    {
                        lpcm = (from e in lpcm
                                select new PensioneConiugeModel()
                                {
                                    idPensioneConiuge = e.idPensioneConiuge,
                                    importoPensione = e.importoPensione,
                                    dataInizioValidita = e.dataInizioValidita,
                                    dataFineValidita = e.dataFineValidita,
                                    dataAggiornamento = e.dataAggiornamento,
                                    annullato = e.annullato
                                }).ToList();
                    }
                }
            }
            return lpcm;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfermaNuovoConiuge(ConiugeModel cm, decimal idMaggiorazioniFamiliari, decimal idAttivazioneMagFam)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                        {
                            dtvmf.InserisciConiugeVarMagFam(cm, idMaggiorazioniFamiliari, idAttivazioneMagFam);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.Message);

                        List<SelectListItem> lTipologiaConiuge = new List<SelectListItem>();

                        var r = new List<SelectListItem>();

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

                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            var tm = dtt.GetTrasferimentoByIdAttMagFam(idAttivazioneMagFam);

                            ViewData.Add("Trasferimento", tm);
                        }
                        ViewBag.lTipologiaConiuge = lTipologiaConiuge;
                        ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);
                        ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);
                        return PartialView("NuovoConiuge", cm);
                    }
                }
                else
                {
                    List<SelectListItem> lTipologiaConiuge = new List<SelectListItem>();

                    var r = new List<SelectListItem>();

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

                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var tm = dtt.GetTrasferimentoByIdAttMagFam(idAttivazioneMagFam);

                        ViewData.Add("Trasferimento", tm);
                    }
                    ViewBag.lTipologiaConiuge = lTipologiaConiuge;
                    ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);
                    ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);

                    return PartialView("NuovoConiuge", cm);
                }
            }
            catch (Exception ex)
            {
                PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return RedirectToAction("ElencoFamiliari", new { idMaggiorazioniFamiliari = idMaggiorazioniFamiliari });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfermaNuovoFiglio(FigliModel fm, decimal idMaggiorazioniFamiliari, decimal idAttivazioneMagFam)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                        {
                            dtvmf.InserisciFiglioVarMagFam(fm, idMaggiorazioniFamiliari, idAttivazioneMagFam);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.Message);

                        List<SelectListItem> lTipologiaFiglio = new List<SelectListItem>();

                        var r = new List<SelectListItem>();

                        using (dtTipologiaFiglio dttf = new dtTipologiaFiglio())
                        {
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
                        }

                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            var tm = dtt.GetTrasferimentoByIdAttMagFam(idAttivazioneMagFam);

                            ViewData.Add("Trasferimento", tm);
                        }
                        ViewBag.lTipologiaFiglio = lTipologiaFiglio;
                        ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);
                        ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);
                        return PartialView("NuovoFiglio", fm);
                    }
                }
                else
                {
                    List<SelectListItem> lTipologiaFiglio = new List<SelectListItem>();

                    var r = new List<SelectListItem>();

                    using (dtTipologiaFiglio dttf = new dtTipologiaFiglio())
                    {
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
                    }

                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var tm = dtt.GetTrasferimentoByIdAttMagFam(idAttivazioneMagFam);

                        ViewData.Add("Trasferimento", tm);
                    }

                    ViewBag.lTipologiaFiglio = lTipologiaFiglio;
                    ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);
                    ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);

                    return PartialView("NuovoFiglio", fm);
                }
            }
            catch (Exception ex)
            {
                PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return RedirectToAction("ElencoFamiliari", new { idMaggiorazioniFamiliari = idMaggiorazioniFamiliari });
        }

        public ActionResult InserisciAltriDatiFamiliariConiuge(AltriDatiFamConiugeModel adf, decimal idAttivazione)
        {
            try
            {
                adf.dataAggiornamento = DateTime.Now;
                adf.annullato = false;

                if (ModelState.IsValid)
                {
                    using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                    {
                        dtadf.SetAltriDatiFamiliariConiuge(ref adf, idAttivazione);
                    }
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
                    ViewData.Add("Comuni", comuni);
                    ViewData.Add("idAttivazione", idAttivazione);

                    return PartialView("InserisciAltriDatiFamiliariConiuge", adf);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return RedirectToAction("AltriDatiFamiliariConiuge", new { idConiuge = adf.idConiuge, idAttivazione = idAttivazione });
        }

        public ActionResult InserisciAltriDatiFamiliariFiglio(AltriDatiFamFiglioModel adf, decimal idAttivazione)
        {
            try
            {
                adf.dataAggiornamento = DateTime.Now;
                adf.annullato = false;

                if (ModelState.IsValid)
                {
                    using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                    {
                        dtadf.SetAltriDatiFamiliariFiglio(ref adf, idAttivazione);
                    }
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

                    ViewData.Add("Comuni", comuni);
                    ViewData.Add("idAttivazione", idAttivazione);

                    return PartialView("InserisciAltriDatiFamiliariFiglio", adf);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return RedirectToAction("AltriDatiFamiliariFiglio", new { idFiglio = adf.idFigli, idAttivazione = idAttivazione });
        }

        public JsonResult ConfermaEliminaConiuge(decimal idConiuge)
        {
            try
            {
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    dtvmf.EliminaConiuge(idConiuge);
                }
            }
            catch (Exception ex)
            {
                return Json(new { errore = ex.Message, msg = "" });
            }
            return Json(new { errore = "", msg = "Eliminazione effettuata con successo." });
        }

        public JsonResult ConfermaEliminaFiglio(decimal idFiglio)
        {
            try
            {
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    dtvmf.EliminaFiglio(idFiglio);
                }
            }
            catch (Exception ex)
            {
                return Json(new { errore = ex.Message, msg = "" });
            }
            return Json(new { errore = "", msg = "Eliminazione effettuata con successo." });
        }

        public ActionResult MessaggioAnnullaVariazioneMagFam(decimal idTrasferimento)
        {
            ModelloMsgMail msg = new ModelloMsgMail();

            try
            {
                using (dtDipendenti dtd = new dtDipendenti())
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        using (dtUffici dtu = new dtUffici())
                        {
                            var t = dtt.GetTrasferimentoById(idTrasferimento);

                            if (t?.idTrasferimento > 0)
                            {
                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                var uff = dtu.GetUffici(t.idUfficio);

                                msg.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaMaggiorazioniFamiliari, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView(msg);
        }

    }
}
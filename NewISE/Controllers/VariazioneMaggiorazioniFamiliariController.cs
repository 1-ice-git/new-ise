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
using NewISE.Models.Enumeratori;


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
                bool siDoc = false;
                bool datiParziali = true;
                bool datiNuovoConiuge = false;
                bool datiNuovoFigli = false;
                bool siDocFormulario = false;
                bool siPensioniConiuge = false;


                dtmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                    out richiestaAttivazione, out attivazione, out datiConiuge,
                    out datiParzialiConiuge, out datiFigli, out datiParzialiFigli,
                    out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione,
                    out trasfSolaLettura, out datiParziali, out siDoc, out datiNuovoConiuge, out datiNuovoFigli, out siDocFormulario, out siPensioniConiuge);

                if (richiestaAttivazione == true || attivazione == true || trasfSolaLettura)
                {

                    solaLettura = true;
                }
                else
                {
                    solaLettura = false;
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
                bool datiParziali = true;
                bool siDocConiuge = false;
                bool siDocFigli = false;
                bool docFormulario = false;
                bool inLavorazione = false;
                bool trasfSolaLettura = false;
                bool siDoc = false;
                bool datiNuovoConiuge = false;
                bool datiNuovoFigli = false;
                bool siDocFormulario = false;
                bool siPensioniConiuge = false;



                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    var tr = idTrasferimento;

                    if (!tr.Equals(null))
                    {
                        ViewData.Add("idTrasferimento", tr);

                        dtvmf.SituazioneMagFamVariazione(idTrasferimento, out rinunciaMagFam,
                                                out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                                                out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario,
                                                out inLavorazione, out trasfSolaLettura, out datiParziali, out siDoc, out datiNuovoConiuge, out datiNuovoFigli, out siDocFormulario, out siPensioniConiuge);

                        ViewData.Add("rinunciaMagFam", rinunciaMagFam);

                    }
                    else
                    {
                        throw new Exception("Nessun trasferimento impostato.");
                    }

                    ViewData.Add("DataOdierna", DateTime.Now.ToShortDateString());
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
            bool visualizzaGestModifiche = true;
            bool siDoc = false;
            bool datiParziali = true;
            bool datiNuovoConiuge = false;
            bool datiNuovoFigli = false;
            bool siDocFormulario = false;
            bool siPensioniConiuge = false;

            var solaLettura = 0;

            using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
            {
                dtvmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                                                out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                                                out datiFigli, out datiParzialiFigli, out siDocConiuge,
                                                out siDocFigli, out docFormulario, out inLavorazione, out trasfSolaLettura, out datiParziali, out siDoc,
                                                out datiNuovoConiuge, out datiNuovoFigli, out siDocFormulario, out siPensioniConiuge);

                if (richiestaAttivazione && attivazione == false)
                {
                    solaLettura = 1;
                }
                if (attivazione)
                {
                    visualizzaGestModifiche = false;
                }
            }

            List<VariazioneElencoFamiliariModel> lefm = new List<VariazioneElencoFamiliariModel>();

            try
            {
                #region Coniugi
                using (dtConiuge dtc = new dtConiuge())
                {
                    List<VariazioneConiugeModel> lcm = dtc.GetListaAttivazioniConiugeByIdMagFam(idMaggiorazioniFamiliari).OrderBy(a => a.progressivo).ThenByDescending(a => a.idStatoRecord).ToList();

                    var check_nuovo_coniuge = 1;

                    if (lcm?.Any() ?? false)
                    {
                        using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                        {
                            using (dtPensione dtp = new dtPensione())
                            {
                                foreach (var e in lcm)
                                {
                                    if (e.visualizzabile)
                                    {
                                        VariazioneElencoFamiliariModel efm = new VariazioneElencoFamiliariModel()
                                        {
                                            idMaggiorazioniFamiliari = e.idMaggiorazioniFamiliari,
                                            idFamiliare = e.idConiuge,
                                            Nominativo = e.cognome + " " + e.nome,
                                            CodiceFiscale = e.codiceFiscale,
                                            dataInizio = e.dataInizio,
                                            dataFine =  e.dataFine,
                                            parentela = EnumParentela.Coniuge,
                                            idAltriDati = dtvmf.GetAltriDatiFamiliariConiuge(e.idConiuge).idAltriDatiFam,
                                            Documenti = dtvmf.GetDocumentiByIdTable_MF(e.idConiuge, EnumTipoDoc.Documento_Identita, EnumParentela.Coniuge, idMaggiorazioniFamiliari),
                                            HasPensione = dtp.HasPensione(e.idConiuge),
                                            eliminabile = e.eliminabile,
                                            modificabile = e.modificabile,
                                            modificato = e.modificato,
                                            nuovo = e.nuovo,
                                            FK_idFamiliare = e.FK_idConiuge
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
                    }
                    ViewData.Add("check_nuovo_coniuge", check_nuovo_coniuge);
                }
                #endregion

                #region Figli
                using (dtFigli dtf = new dtFigli())
                {
                    List<VariazioneFigliModel> lfm = dtf.GetListaAttivazioniFigliByIdMagFam(idMaggiorazioniFamiliari).OrderBy(a => a.progressivo).ThenByDescending(a => a.idStatoRecord).ToList();

                    var check_nuovo_figlio = 1;

                    if (lfm?.Any() ?? false)
                    {
                        using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                        {
                            using (dtPensione dtp = new dtPensione())
                            {
                                foreach (var e in lfm)
                                {
                                    if (e.visualizzabile)
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
                                            HasPensione = false,// dtp.HasPensione(e.idFigli),
                                            eliminabile = e.eliminabile,
                                            idStatoRecord = e.idStatoRecord,
                                            modificabile = e.modificabile,
                                            modificato = e.modificato,
                                            nuovo = e.nuovo,
                                            FK_idFamiliare = e.FK_IdFigli
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
                    }
                    ViewData.Add("check_nuovo_figlio", check_nuovo_figlio);
                }
                #endregion

                ViewData.Add("solaLettura", solaLettura);
                ViewData.Add("visualizzaGestModifiche", visualizzaGestModifiche);

                ViewData.Add("trasfSolaLettura", trasfSolaLettura);

                ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);
                var lefm_ordinata = lefm.OrderBy(a => a.parentela).ThenByDescending(a => a.dataInizio).ToList();
                return PartialView(lefm_ordinata);

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
            bool siAdf = false;
            bool siDocIdentita = false;
            bool siPensioniConiuge = false;
            bool solaLettura = true;
            bool trasfSolaLettura = false;
            bool siDoc = false;
            bool datiParziali = true;
            bool datiNuovoConiuge = false;
            bool datiNuovoFigli = false;
            bool siDocFormulario = false;


            using (dtVariazioniMaggiorazioneFamiliare dtmf = new dtVariazioniMaggiorazioneFamiliare())
            {
                dtmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione, out trasfSolaLettura, out datiParziali, out siDoc, out datiNuovoConiuge, out datiNuovoFigli, out siDocFormulario, out siPensioniConiuge);
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
                                        out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out siPensioniConiuge, out docFormulario, out siDocIdentita, out siAdf);

                                if (!trasfSolaLettura)
                                {
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
                                }
                                else
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

                    ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);


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
                    bool siDoc = false;
                    bool datiParziali = true;
                    bool datiNuovoConiuge = false;
                    bool datiNuovoFigli = false;
                    bool siDocFormulario = false;
                    bool siPensioniConiuge = false;


                    dtvmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                        out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                        out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli,
                        out docFormulario, out inLavorazione, out trasfSolaLettura, out datiParziali, out siDoc,
                        out datiNuovoConiuge, out datiNuovoFigli, out siDocFormulario, out siPensioniConiuge);

                    if (richiestaAttivazione == true && attivazione == false)
                    {
                        solaLettura = true;
                    }

                    if (dtvmf.ConiugeModificabile(idConiuge, idMaggiorazioniFamiliari) == false)
                    {
                        solaLettura = true;
                    }

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
                    ViewData.Add("solaLettura", solaLettura);
                    ViewData.Add("trasfSolaLettura", trasfSolaLettura);
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
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    amf = dtvmf.GetAttivazioneById(adf.idAltriDatiFam, EnumTipoTabella.AltriDatiFamiliari);

                    ViewData.Add("idAttivazione", amf.IDATTIVAZIONEMAGFAM);
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
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    amf = dtvmf.GetAttivazioneById(idConiuge, EnumTipoTabella.Coniuge);

                    ViewData.Add("idAttivazione", amf.IDATTIVAZIONEMAGFAM);
                }

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
                    using (dtFigli dtf = new dtFigli())
                    {

                        idMaggiorazioniFamiliari = dtvmf.GetMaggiorazioneFamiliareFiglio(idFiglio);

                        adf = dtvmf.GetAltriDatiFamiliariFiglio(idFiglio, idMaggiorazioniFamiliari);


                        ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);

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
                        bool siDoc = false;
                        bool datiParziali = true;
                        bool datiNuovoConiuge = false;
                        bool datiNuovoFigli = false;
                        bool siDocFormulario = false;
                        bool siPensioniConiuge = false;


                        dtvmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                            out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                            out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out inLavorazione, out trasfSolaLettura, out datiParziali, out siDoc,
                            out datiNuovoConiuge, out datiNuovoFigli, out siDocFormulario, out siPensioniConiuge);

                        if (richiestaAttivazione == true && attivazione == false)
                        {
                            solaLettura = true;
                        }
                        else
                        {
                            solaLettura = false;
                        }

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

                        ViewData.Add("solaLettura", solaLettura);
                        ViewData.Add("trasfSolaLettura", trasfSolaLettura);
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
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {

                    amf = dtvmf.GetAttivazioneById(adf.idAltriDatiFam, EnumTipoTabella.AltriDatiFamiliari);
                    ViewData.Add("idAttivazione", amf.IDATTIVAZIONEMAGFAM);
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

                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    amf = dtvmf.GetAttivazioneById(idFiglio, EnumTipoTabella.Figli);

                    ViewData.Add("idAttivazione", amf.IDATTIVAZIONEMAGFAM);
                }

                return PartialView("InserisciAltriDatiFamiliariFiglio", adf);
            }
        }

        public ActionResult ModificaAltriDatiFamiliariConiuge(decimal idAltriDatiFam, decimal idMaggiorazioniFamiliari, decimal idAttivazione)
        {
            AltriDatiFamConiugeModel adfm = new AltriDatiFamConiugeModel();

            try
            {
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    adfm = dtvmf.GetAltriDatiFamiliariConiugeByID(idAltriDatiFam);
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
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    adfm = dtvmf.GetAltriDatiFamiliariFiglioByID(idAltriDatiFam);
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
            decimal idAdf;

            try
            {
                adfm.dataAggiornamento = DateTime.Now;
                adfm.idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione;

                if (ModelState.IsValid)
                {
                    using (dtVariazioniMaggiorazioneFamiliare dtadf = new dtVariazioniMaggiorazioneFamiliare())
                    {
                        idAdf = dtadf.EditVariazioneAltriDatiFamiliariConiuge(adfm);
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

            return RedirectToAction("AltriDatiFamiliariConiuge", new { idConiuge = adfm.idConiuge, idAltriDati = idAdf });


        }

        public ActionResult ConfermaModificaAdfFiglio(AltriDatiFamFiglioModel adfm)
        {
            try
            {
                adfm.dataAggiornamento = DateTime.Now;
                adfm.idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione;

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

                bool esistonoPensioni = false;
                if (lpcm.Count > 0)
                {
                    esistonoPensioni = true;
                }
                ViewData.Add("esistonoPensioni", esistonoPensioni);

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
                        bool siDoc = false;
                        bool datiParziali = true;
                        bool datiNuovoConiuge = false;
                        bool datiNuovoFigli = false;
                        bool siDocFormulario = false;
                        bool siPensioniConiuge = false;


                        dtmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                            out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                            out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli,
                            out docFormulario, out inLavorazione, out trasfSolaLettura, out datiParziali,
                            out siDoc, out datiNuovoConiuge, out datiNuovoFigli, out siDocFormulario, out siPensioniConiuge);

                        if (richiestaAttivazione == true && attivazione == false)
                        {
                            solaLettura = true;
                        }

                        if (dtmf.ConiugeModificabile(idConiuge, idMaggiorazioniFamiliari) == false)
                        {
                            solaLettura = true;
                        }

                        var cm = dtc.GetConiugebyID(idConiuge);

                        #region controllo modifica pensioni
                        using (ModelDBISE db = new ModelDBISE())
                        {
                            var t = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari).TRASFERIMENTO;
                            var lp_in_lav = db.CONIUGE.Find(cm.idConiuge).PENSIONE.Where(a =>
                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione &&
                                                a.NASCONDI == false &&
                                                ((a.DATAINIZIO<=t.DATARIENTRO && a.DATAFINE>=t.DATARIENTRO) || a.DATAFINE<=t.DATARIENTRO))
                                            .OrderByDescending(a => a.IDPENSIONE).ToList();
                            bool pensioniModificate = false;
                            if (lp_in_lav.Count() > 0)
                            {
                                pensioniModificate = true;
                            }
                            ViewData.Add("pensioniModificate", pensioniModificate);
                        }
                        #endregion

                        ViewData.Add("nominativo", cm.nominativo);
                        ViewData.Add("solaLettura", solaLettura);
                        ViewData.Add("trasfSolaLettura", trasfSolaLettura);
                    }
                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewData.Add("idConiuge", idConiuge);
            var lpcm_ordinata = lpcm.OrderByDescending(a => a.dataInizioValidita).ToList();
            return PartialView(lpcm_ordinata);
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
                        db.Database.BeginTransaction();
                        try
                        {
                            using (dtPensione dtp = new dtPensione())
                            {
                                using (dtVariazioniMaggiorazioneFamiliare dtamf = new dtVariazioniMaggiorazioneFamiliare())
                                {
                                    PensioneConiugeModel pm = new PensioneConiugeModel();

                                    //dtamf.VerificaPensioniAttiveInLavorazione(out pm, pcm, idConiuge, db);

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
                                    pcm.idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione;

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
                                    DateTime dataRientro = db.TRASFERIMENTO.Find(idTrasf).DATARIENTRO;

                                    //controlla data inserita superiore a dataRientro
                                    if (pcm.dataInizioValidita > dataRientro)
                                    {
                                        pcm.dataInizioValidita = dataRientro;
                                    }

                                    if (pcm.checkAggiornaTutti == false)
                                    {
                                        dtp.SetNuovoImportoPensioneVariazione(pcm, idConiuge, attmf_aperta.IDATTIVAZIONEMAGFAM, dataRientro, db);
                                    }
                                    else
                                    {
                                        //inserisce periodo e annulla i periodi successivi (fino al primo buco temporale o fino a dataRientro)
                                        dtp.SetNuovoImportoPensioneVariazione_AggiornaTutti(pcm, idConiuge, attmf_aperta.IDATTIVAZIONEMAGFAM, dataRientro, db);
                                    }
                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento nuovo importo pensione coniuge (" + idConiuge + ")", "PENSIONI", db, idTrasf, pcm.idPensioneConiuge);
                                }
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

                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                    {
                        using (dtPensione dtp = new dtPensione())
                        {

                            db.Database.BeginTransaction();
                            try
                            {
                                PensioneConiugeModel pcm_dacanc = new PensioneConiugeModel();

                                pcm = dtp.GetPensioneByID(idPensione);

                                if (pcm != null && pcm.HasValue())
                                {
                                    var att = dtvmf.GetAttivazioneById(idPensione, EnumTipoTabella.Pensione);

                                    dtvmf.VerificaPensioniAttiveInLavorazione(out pcm_dacanc, pcm, idConiuge, db);
                                    if (pcm_dacanc.idPensioneConiuge > 0 == false)
                                    {
                                        pcm_dacanc = pcm;
                                        att = dtvmf.GetAttivazioneById(pcm_dacanc.idPensioneConiuge, EnumTipoTabella.Pensione, db);
                                    }

                                    dtp.EliminaImportoPensioneVariazione(pcm_dacanc, idConiuge, att.IDATTIVAZIONEMAGFAM, db);
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
            bool datiParzialiNuovoConiuge = false;
            bool datiFigli = false;
            bool datiParzialiNuoviFigli = false;
            bool siDocNuovoConiuge = false;
            bool siDocNuoviFigli = false;
            bool siDocIdentita = false;
            bool siAdf = false;
            bool siPensioniConiuge = false;
            bool docFormulario = false;
            bool inLavorazione = false;
            bool CheckNotifica = true;
            bool trasfSolaLettura = false;
            bool siDocFormulari = false;
            bool datiParziali = true;
            bool datiNuovoConiuge = false;
            bool datiNuoviFigli = false;

            try
            {
                amministratore = Utility.Amministratore();

                using (dtVariazioniMaggiorazioneFamiliare dtmf = new dtVariazioniMaggiorazioneFamiliare())
                {

                    var amf = dtmf.GetAttivazioneById(idMaggiorazioniFamiliari, EnumTipoTabella.MaggiorazioniFamiliari);

                    dtmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                                           out richiestaAttivazione, out attivazione, out datiConiuge,
                                           out datiParzialiNuovoConiuge, out datiFigli, out datiParzialiNuoviFigli,
                                           out siDocNuovoConiuge, out siDocNuoviFigli, out docFormulario,
                                           out inLavorazione, out trasfSolaLettura, out datiParziali,
                                           out siDocIdentita, out datiNuovoConiuge, out datiNuoviFigli, out siDocFormulari, out siPensioniConiuge);


                    if (((datiFigli || datiParziali == false || siDocIdentita || datiConiuge || siPensioniConiuge) && siDocFormulari) && richiestaAttivazione == false)
                    {
                        CheckNotifica = true;
                    }

                    if (richiestaAttivazione || attivazione)
                    {
                        CheckNotifica = false;
                    }
                    if (datiNuovoConiuge && (siDocNuovoConiuge == false || datiParzialiNuovoConiuge == true))
                    {
                        CheckNotifica = false;
                    }
                    if (datiNuoviFigli && (siDocNuoviFigli == false || datiParzialiNuoviFigli == true))
                    {
                        CheckNotifica = false;
                    }

                    if (datiNuovoConiuge == false && datiNuoviFigli == false &&
                        datiFigli == false && datiParziali == true && siDocIdentita == false && datiConiuge == false && siPensioniConiuge == false)
                    {
                        CheckNotifica = false;
                    }

                    if (siDocFormulari == false)
                    {
                        CheckNotifica = false;
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
                        admin = amministratore,
                        rinuncia = rinunciaMagFam,
                        richiesta = richiestaAttivazione,
                        attivazione = attivazione,
                        datiConiuge = datiConiuge,
                        datiParziali = datiParziali,
                        datiParzialiConiuge = datiParzialiNuovoConiuge,
                        datiFigli = datiFigli,
                        datiParzialiFigli = datiParzialiNuoviFigli,
                        siDoc = siDocIdentita,
                        siDocConiuge = siDocNuovoConiuge,
                        siDocFigli = siDocNuoviFigli,
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
                    var amf = dtvmf.GetAttivazioneById(idMaggiorazioniFamiliari, EnumTipoTabella.MaggiorazioniFamiliari);

                    dtvmf.NotificaRichiestaVariazione(amf.IDATTIVAZIONEMAGFAM);
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

                        dtvmf.AnnullaRichiestaVariazione(amf.IDATTIVAZIONEMAGFAM, out idAttivazioneMagFamNew, msg);
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
                            bool siDoc = false;
                            bool datiParziali = true;
                            bool datiNuovoConiuge = false;
                            bool datiNuovoFigli = false;
                            bool siDocFormulario = false;
                            bool siPensioniConiuge = false;


                            if ((parentela == EnumParentela.Coniuge || parentela == EnumParentela.Figlio) && idMaggiorazioniFamiliari > 0)
                            {
                                dtvmf.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                                    out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                                    out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli,
                                    out docFormulario, out inLavorazione, out trasfSolaLettura,
                                    out datiParziali, out siDoc, out datiNuovoConiuge,
                                    out datiNuovoFigli, out siDocFormulario, out siPensioniConiuge);

                                if (richiestaAttivazione == true && attivazione == false)
                                {
                                    solaLettura = true;
                                }


                                if (parentela == EnumParentela.Coniuge)
                                {
                                    if (dtvmf.ConiugeModificabile(idFamiliare, idMaggiorazioniFamiliari) == false)
                                    {
                                        solaLettura = true;
                                    }
                                }
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

        public ActionResult ElencoDocumentiPrecedenti(decimal idFamiliare, decimal idParentela, decimal idMaggiorazioniFamiliari)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                    {
                        decimal? idFamiliareOld = 0;

                        //recupero l' ID del familiare Old in base alla parentela
                        switch ((EnumParentela)idParentela)
                        {
                            case EnumParentela.Figlio:
                                using (dtFigli dtf = new dtFigli())
                                {
                                    var fm = dtf.GetFigliobyID(idFamiliare);
                                    var idMagFam = fm.idMaggiorazioniFamiliari;

                                    List<DOCUMENTI> ldf = new List<DOCUMENTI>();
                                    if (fm.FK_IdFigli > 0 && fm.idStatoRecord != (decimal)EnumStatoRecord.Annullato && fm.idStatoRecord != (decimal)EnumStatoRecord.Attivato)
                                    {
                                        idFamiliareOld = fm.FK_IdFigli;
                                    }
                                    else
                                    {
                                        idFamiliareOld = idFamiliare;
                                    }

                                }
                                break;
                            case EnumParentela.Coniuge:
                                using (dtConiuge dtc = new dtConiuge())
                                {
                                    var cm = dtc.GetConiugebyID(idFamiliare);
                                    var idMagFam = cm.idMaggiorazioniFamiliari;

                                    if (cm.FK_idConiuge > 0 && cm.idStatoRecord != (decimal)EnumStatoRecord.Annullato && cm.idStatoRecord != (decimal)EnumStatoRecord.Attivato)
                                    {
                                        idFamiliareOld = cm.FK_idConiuge;
                                    }
                                    else
                                    {
                                        idFamiliareOld = idFamiliare;
                                    }

                                }
                                break;
                        }

                        ldm = dtvmf.GetDocumentiPrecedenti(idFamiliareOld, (EnumParentela)idParentela, idMaggiorazioniFamiliari)
                                .OrderByDescending(a => a.dataInserimento)
                                .ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewData.Add("id", idFamiliare);
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);
            ViewData.Add("idTrasferimento", idMaggiorazioniFamiliari);

            return PartialView(ldm);
        }

        public ActionResult ElencoPensioniPrecedenti(decimal idConiuge)
        {
            List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();

            try
            {
                lpcm = GetPensioniPrecedentiConiuge(idConiuge).ToList();

                using (dtConiuge dtc = new dtConiuge())
                {
                    decimal idMaggiorazioniFamiliari = dtc.GetConiugebyID(idConiuge).idMaggiorazioniFamiliari;
                    ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);

                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewData.Add("idConiuge", idConiuge);

            return PartialView(lpcm);
        }


        public ActionResult SostituisciDocumento(EnumTipoDoc tipoDoc, decimal id, EnumParentela parentela, EnumChiamante Chiamante, decimal idDocumento)
        {
            try
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
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        public ActionResult NuovoDocumentoMagFam(EnumTipoDoc tipoDoc, decimal id, EnumParentela parentela, EnumChiamante Chiamante)
        {
            try
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
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
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
        public JsonResult ConfermaSostituisciDocumento(decimal idDoc, EnumTipoDoc tipoDoc, decimal idFamiliare, EnumParentela parentela)
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
                                                    dm.idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione;
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
                                                            var newamf = dtvmf.CreaAttivazione(idMaggiorazioniFamiliari, db);

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
                                                    dm.idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione;
                                                    dtvmf.AssociaDocumentoFiglio(ref dm, idFamiliare, db);

                                                    var att_figlio = dtvmf.GetAttivazioneById(idFamiliare, EnumTipoTabella.Figli);
                                                    if (att_figlio.ATTIVAZIONEMAGFAM == false && att_figlio.RICHIESTAATTIVAZIONE == false)
                                                    {
                                                        dtvmf.AssociaDocumentoAttivazione(att_figlio.IDATTIVAZIONEMAGFAM, dm.idDocumenti, db);
                                                    }
                                                    else
                                                    {
                                                        var idamf = dtvmf.GetMaggiorazioneFamiliareFiglio(idFamiliare);
                                                        var last_att = dtvmf.GetAttivazioneById(idamf, EnumTipoTabella.MaggiorazioniFamiliari);

                                                        if (last_att.RICHIESTAATTIVAZIONE)
                                                        {
                                                            var idMaggiorazioniFamiliari = dtvmf.GetMaggiorazioneFamiliareDocumento(idDoc);
                                                            var newamf = dtvmf.CreaAttivazione(idMaggiorazioniFamiliari, db);

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
        public JsonResult SalvaNuovoDocumentoMF(EnumTipoDoc tipoDoc, decimal id, EnumParentela parentela)
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
                                                                var newamf = dtvmf.CreaAttivazione(idMF, db);

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
                                                            var idMF = dtvmf.GetMaggiorazioneFamiliareFiglio(id);
                                                            var last_att = dtvmf.GetAttivazioneById(idMF, EnumTipoTabella.MaggiorazioniFamiliari);

                                                            if (last_att.RICHIESTAATTIVAZIONE)
                                                            {
                                                                //att.ANNULLATO = true;
                                                                var newamf = dtvmf.CreaAttivazione(idMF, db);

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
                                    idStatoRecord = e.idStatoRecord,
                                    FK_idPensione = e.FK_idPensione,
                                    nascondi = e.nascondi
                                }).ToList();
                    }
                }
            }
            return lpcm;
        }

        public IList<PensioneConiugeModel> GetPensioniPrecedentiConiuge(decimal idConiuge)
        {
            List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();
            List<ConiugeModel> lcm = new List<ConiugeModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    lpcm = dtvmf.GetListaPensioniPrecedentiConiugeByIdMagFam(idConiuge);

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
                                    idStatoRecord = e.idStatoRecord,
                                    FK_idPensione = e.FK_idPensione,
                                    nascondi = e.nascondi
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
                adf.idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione;

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
                adf.idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione;

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
        public JsonResult ConfermaAnnullaModConiuge(decimal idConiuge)
        {
            try
            {
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    dtvmf.AnnullaModConiuge(idConiuge);
                }
            }
            catch (Exception ex)
            {
                return Json(new { errore = ex.Message, msg = "" });
            }
            return Json(new { errore = "", msg = "Eliminazione effettuata con successo." });
        }
        public JsonResult ConfermaAnnullaModFiglio(decimal idFiglio)
        {
            try
            {
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    dtvmf.AnnullaModFiglio(idFiglio);
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

        public ActionResult VisualizzaModificheFiglio(decimal idMaggiorazioniFamiliari, decimal idFigli)
        {
            ViewData.Add("idFigli", idFigli);
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);
            return PartialView();
        }
        public ActionResult VisualizzaModificheConiuge(decimal idMaggiorazioniFamiliari, decimal idConiuge)
        {
            ViewData.Add("idConiuge", idConiuge);
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);
            return PartialView();
        }

        public ActionResult VisualizzaModificheFiglioTitolo(decimal idFigli, decimal idMaggiorazioniFamiliari)
        {
            VariazioneFigliModel vfm = new VariazioneFigliModel();
            using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
            {
                using (dtFigli dtf = new dtFigli())
                {
                    vfm.ev_documenti = "";
                    vfm.ev_anagrafica = "";
                    vfm.ev_altridati = "";
                    string evidenzia_titolo = ";border-bottom:solid;border-bottom-width:4px;border-color:yellow";

                    var db = new ModelDBISE();

                    var fm = dtf.GetFigliobyID(idFigli);

                    #region documenti
                    var n_doc = db.FIGLI.Find(fm.idFigli).DOCUMENTI.Where(a =>
                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                .OrderByDescending(a => a.IDDOCUMENTO).ToList().Count();
                    var n_doc_mod = db.FIGLI.Find(fm.idFigli).DOCUMENTI.Where(a =>
                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Attivato)
                                .OrderByDescending(a => a.IDDOCUMENTO).ToList().Count();
                    if (n_doc_mod > 0)
                    {
                        vfm.ev_documenti = evidenzia_titolo;
                    }
                    #endregion

                    #region anagrafica
                    decimal? idFiglioOld = idFigli;
                    if (fm.FK_IdFigli > 0 && fm.idStatoRecord != (decimal)EnumStatoRecord.Annullato && fm.idStatoRecord != (decimal)EnumStatoRecord.Attivato)
                    {
                        idFiglioOld = fm.FK_IdFigli;
                    }
                    var vafm = dtvmf.CheckVariazioniAnagraficaFiglio(idFiglioOld, idFigli);
                    vfm.ev_anagrafica = vafm.ev_anagrafica;
                    #endregion

                    #region altri dati
                    var adf = dtvmf.GetAltriDatiFamiliariFiglio(idFigli, fm.idMaggiorazioniFamiliari);
                    if (adf.FK_idAltriDatiFam > 0 && adf.idStatoRecord != (decimal)EnumStatoRecord.Annullato && adf.idStatoRecord != (decimal)EnumStatoRecord.Attivato)
                    {
                        var vadffm = dtvmf.CheckVariazioniAdfFiglio(adf.FK_idAltriDatiFam, adf.idAltriDatiFam);
                        vfm.ev_altridati = vadffm.ev_altridati;
                    }
                    #endregion
                }
            }
            ViewData.Add("idFigli", idFigli);
            return PartialView(vfm);
        }
        public ActionResult VisualizzaModificheConiugeTitolo(decimal idConiuge, decimal idMaggiorazioniFamiliari)
        {
            VariazioneConiugeModel vcm = new VariazioneConiugeModel();
            using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
            {
                using (dtConiuge dtc = new dtConiuge())
                {
                    string evidenzia_titolo = ";border-bottom:solid;border-bottom-width:4px;border-color:yellow";

                    vcm.ev_documenti = "";
                    vcm.ev_anagrafica = "";
                    vcm.ev_altridati = "";
                    vcm.ev_pensione = "";
                    var cm = dtc.GetConiugebyID(idConiuge);

                    var db = new ModelDBISE();

                    #region documenti
                    var n_doc = db.CONIUGE.Find(cm.idConiuge).DOCUMENTI.Where(a =>
                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                .OrderByDescending(a => a.IDDOCUMENTO).ToList().Count();

                    var n_doc_mod = db.CONIUGE.Find(cm.idConiuge).DOCUMENTI.Where(a =>
                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Attivato)
                                .OrderByDescending(a => a.IDDOCUMENTO).ToList().Count();
                    if (n_doc_mod > 0)
                    {
                        vcm.ev_documenti = evidenzia_titolo;
                    }
                    #endregion

                    #region altri dati
                    var adf = dtvmf.GetAltriDatiFamiliariConiuge(idConiuge);
                    if (adf.FK_idAltriDatiFam > 0 && adf.idStatoRecord != (decimal)EnumStatoRecord.Attivato && adf.idStatoRecord != (decimal)EnumStatoRecord.Annullato)
                    {
                        var vadfcm = dtvmf.CheckVariazioniAdfConiuge(adf.FK_idAltriDatiFam, adf.idAltriDatiFam);
                        vcm.ev_altridati = vadfcm.ev_altridati;
                    }
                    #endregion

                    #region anagrafica
                    decimal? idConiugeOld = idConiuge;
                    if (cm.FK_idConiuge > 0 && cm.idStatoRecord != (decimal)EnumStatoRecord.Attivato && cm.idStatoRecord != (decimal)EnumStatoRecord.Annullato)
                    {
                        idConiugeOld = cm.FK_idConiuge;
                    }
                    var vacm = dtvmf.CheckVariazioniAnagraficaConiuge(idConiugeOld, idConiuge);
                    vcm.ev_anagrafica = vacm.ev_anagrafica;
                    #endregion

                    #region pensioni
                    var lp_att = db.CONIUGE.Find(cm.idConiuge).PENSIONE.Where(a =>
                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                .OrderByDescending(a => a.IDPENSIONE).ToList();

                    var lp_mod = db.CONIUGE.Find(cm.idConiuge).PENSIONE.Where(a =>
                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                            a.NASCONDI == false)
                                .OrderByDescending(a => a.IDPENSIONE).ToList();

                    var lp_in_lav = db.CONIUGE.Find(cm.idConiuge).PENSIONE.Where(a =>
                             a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                             a.IDSTATORECORD != (decimal)EnumStatoRecord.Attivato &&
                             a.NASCONDI == false)
                                 .OrderByDescending(a => a.IDPENSIONE).ToList();
                    if (lp_mod.Count() != lp_att.Count() || lp_in_lav.Count() > 0)
                    {
                        vcm.ev_pensione = evidenzia_titolo;
                    }
                    #endregion

                }
            }
            ViewData.Add("idConiuge", idConiuge);
            return PartialView(vcm);
        }


        public ActionResult VisualizzaModificheAdfFiglio(decimal idFigli)
        {
            VariazioneAdfFigliModel vadffm = new VariazioneAdfFigliModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    using (dtFigli dtf = new dtFigli())
                    {
                        var fm = dtf.GetFigliobyID(idFigli);
                        var idMagFam = fm.idMaggiorazioniFamiliari;

                        var adff = dtvmf.GetAltriDatiFamiliariFiglio(idFigli, idMagFam);

                        if (adff.FK_idAltriDatiFam > 0 && adff.idStatoRecord != (decimal)EnumStatoRecord.Annullato && adff.idStatoRecord != (decimal)EnumStatoRecord.Attivato)
                        {
                            vadffm = dtvmf.CheckVariazioniAdfFiglio(adff.FK_idAltriDatiFam, adff.idAltriDatiFam);
                        }
                        else
                        {
                            vadffm = dtvmf.CheckVariazioniAdfFiglio(adff.idAltriDatiFam, adff.idAltriDatiFam);
                        }

                        //if (adff.FK_idAltriDatiFam > 0)
                        //{
                        //vadffm = dtvmf.CheckVariazioniAdfFiglio(adff.FK_idAltriDatiFam, adff.idAltriDatiFam);
                        //}
                    }
                }

                ViewData.Add("idFigli", idFigli);
                return PartialView(vadffm);
            }
        }
        public ActionResult VisualizzaModificheAdfConiuge(decimal idConiuge)
        {
            VariazioneAdfConiugeModel vadfcm = new VariazioneAdfConiugeModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    using (dtConiuge dtc = new dtConiuge())
                    {
                        var cm = dtc.GetConiugebyID(idConiuge);
                        var idMagFam = cm.idMaggiorazioniFamiliari;

                        var adfc = dtvmf.GetAltriDatiFamiliariConiuge(idConiuge);

                        if (adfc.FK_idAltriDatiFam > 0 && adfc.idStatoRecord != (decimal)EnumStatoRecord.Annullato && adfc.idStatoRecord != (decimal)EnumStatoRecord.Attivato)
                        {
                            vadfcm = dtvmf.CheckVariazioniAdfConiuge(adfc.FK_idAltriDatiFam, adfc.idAltriDatiFam);
                        }
                        else
                        {

                            //if (adfc.FK_idAltriDatiFam > 0)
                            //{
                            vadfcm = dtvmf.CheckVariazioniAdfConiuge(adfc.idAltriDatiFam, adfc.idAltriDatiFam);
                        }
                    }
                }

                ViewData.Add("idConiuge", idConiuge);
                return PartialView(vadfcm);
            }
        }
        public ActionResult VisualizzaModificheFiglioLink(decimal idFigli, decimal idMaggiorazioniFamiliari)
        {

            ViewData.Add("idFigli", idFigli);
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);
            return PartialView();
        }
        public ActionResult VisualizzaModificheConiugeLink(decimal idConiuge, decimal idMaggiorazioniFamiliari)
        {

            ViewData.Add("idConiuge", idConiuge);
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);
            return PartialView();
        }
        public ActionResult VisualizzaModificheFiglioDettaglio(decimal? idFigliOld, decimal idFigli)
        {
            using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
            {
                using (dtFigli dtf = new dtFigli())
                {
                    decimal? idFiglio_old;
                    var fm = dtf.GetFigliobyID(idFigli);
                    if (fm.FK_IdFigli > 0 && fm.idStatoRecord != (decimal)EnumStatoRecord.Annullato && fm.idStatoRecord != (decimal)EnumStatoRecord.Attivato)
                    {
                        idFiglio_old = fm.FK_IdFigli;
                    }
                    else
                    {
                        idFiglio_old = idFigli;
                    }

                    VariazioneFigliModel vfm = dtvmf.CheckVariazioniAnagraficaFiglio(idFiglio_old, idFigli);

                    ViewData.Add("idFigli", idFigli);
                    return PartialView(vfm);
                }
            }
        }
        public ActionResult VisualizzaModificheConiugeDettaglio(decimal idConiuge)
        {
            using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
            {
                using (dtConiuge dtc = new dtConiuge())
                {
                    decimal? idConiuge_old;
                    var cm = dtc.GetConiugebyID(idConiuge);
                    if (cm.FK_idConiuge > 0 && cm.idStatoRecord != (decimal)EnumStatoRecord.Annullato && cm.idStatoRecord != (decimal)EnumStatoRecord.Attivato)
                    {
                        idConiuge_old = cm.FK_idConiuge;
                    }
                    else
                    {
                        idConiuge_old = idConiuge;
                    }

                    VariazioneConiugeModel vcm = dtvmf.CheckVariazioniAnagraficaConiuge(idConiuge_old, idConiuge);

                    ViewData.Add("idConiuge", idConiuge);
                    return PartialView(vcm);
                }
            }
        }

        public JsonResult ConfermaAnnullaModifichePensioneConiuge(decimal idConiuge)
        {
            string errore = "";
            try
            {
                using (dtVariazioniMaggiorazioneFamiliare dtmf = new dtVariazioniMaggiorazioneFamiliare())
                {
                    dtmf.AnnullaModifichePensioneConiuge(idConiuge);
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

        public JsonResult ConfermaCessazionePensione(DateTime strDataCessazionePensione, decimal idConiuge)
        {
            string errore = "";

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();
                    try
                    {
                        using (dtPensione dtp = new dtPensione())
                        {
                            using (dtVariazioniMaggiorazioneFamiliare dtamf = new dtVariazioniMaggiorazioneFamiliare())
                            {
                                PensioneConiugeModel pm = new PensioneConiugeModel();

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
                                DateTime dataRientro = db.TRASFERIMENTO.Find(idTrasf).DATARIENTRO;
                                DateTime dataPartenza = db.TRASFERIMENTO.Find(idTrasf).DATAPARTENZA;

                                if (strDataCessazionePensione > dataRientro)
                                {
                                    strDataCessazionePensione = dataRientro;
                                }
                                if (strDataCessazionePensione < dataPartenza)
                                {
                                    strDataCessazionePensione = dataPartenza;
                                }

                                dtp.SetDataCessazione(strDataCessazionePensione, idConiuge, attmf_aperta.IDATTIVAZIONEMAGFAM, dataRientro, db);
                                //Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento data cessazione pensione coniuge (" + idConiuge + ")", "PENSIONI", db, idTrasf, pcm.idPensioneConiuge);
                            }
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

    }
}
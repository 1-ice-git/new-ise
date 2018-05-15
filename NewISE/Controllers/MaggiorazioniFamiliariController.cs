using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.MappingViews;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NewISE.EF;
using NewISE.Models;
using NewISE.Models.Tools;
using NewISE.Interfacce;
using System.Web.Helpers;
using MaggiorazioniFamiliariModel = NewISE.Models.DBModel.MaggiorazioniFamiliariModel;

namespace NewISE.Controllers
{

    public enum EnumCallElenco
    {
        Coniuge = 1,
        Figli = 2,
        Formulari = 3
    }

    public class MaggiorazioniFamiliariController : Controller
    {
        [NonAction]
        private bool SolaLetturaPartenza(decimal idAttivazioneMagFam)
        {
            bool solaLettura = false;

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
                bool TrasfSolaLettura = false;
            
                dtmf.SituazioneMagFamPartenza(idAttivazioneMagFam, out rinunciaMagFam,
                    out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                    out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out TrasfSolaLettura);

                if (richiestaAttivazione == true || attivazione == true || TrasfSolaLettura ==true)
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

        public ActionResult ElencoDocumentiFormulario()
        {
            return PartialView();
        }

        public ActionResult ElencoDocumentiFormularioPS()
        {
            return PartialView();
        }

        public ActionResult ElencoFormulariInseriti(decimal idAttivazioneMagFam)
        {
            bool solaLettura = false;
            try
            { 
                solaLettura = this.SolaLetturaPartenza(idAttivazioneMagFam);

                ViewData.Add("solaLettura", solaLettura);
                ViewData["idAttivazioneMagFam"] = idAttivazioneMagFam;
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult TabFormulariInseriti(decimal idAttivazioneMagFam)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();
            try
            {

                bool solaLettura = false;
                solaLettura = this.SolaLetturaPartenza(idAttivazioneMagFam);
                ViewData.Add("solaLettura", solaLettura);
                ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);

                using (dtDocumenti dtd = new dtDocumenti())
                {
                    ldm = dtd.GetFormulariAttivazioneMagFam(idAttivazioneMagFam).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(ldm);
        }
        
        [HttpPost]
        public ActionResult NuovoFormularioMF(decimal idAttivazioneMagFam)
        {
            try
            { 
                ViewData["idAttivazioneMagFam"] = idAttivazioneMagFam;
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult FormularioMF(decimal idAttivazioneMagFam)
        {
            List<DocumentiModel> ldm = new List<DocumentiModel>();

            try
            {
                using (dtDocumenti dtd = new dtDocumenti())
                {
                    ldm = dtd.GetFormulariAttivazioneMagFam(idAttivazioneMagFam).ToList();
                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


            ViewData["idAttivazioneMagFam"] = idAttivazioneMagFam;

            return PartialView(ldm);
        }
        
        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public JsonResult AttivaRichiesta(decimal idAttivazioneMagFam)
        {
            string errore = "";

            try
            {
                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                {
                    dtmf.AttivaRichiesta(idAttivazioneMagFam);
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
        public JsonResult AnnullaRichiesta(FormCollection fc)
        {
            decimal idAttivazioneMagFamNew = 0;
            FormCollection collection = new FormCollection(Request.Unvalidated().Form);

            string errore = "";
            decimal idAttivazioneMagFam = Convert.ToDecimal(collection["idAttivazioneMagFam"]);
            string testoAnnullaMF = collection["msg"];

            try
            {
                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                {
                    dtmf.AnnullaRichiesta(idAttivazioneMagFam, out idAttivazioneMagFamNew, testoAnnullaMF);
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
                        err = errore,
                        idAttivazioneMagFamNew = idAttivazioneMagFamNew
                    });
        }

        [HttpPost]
        public JsonResult NotificaRichiesta(decimal idAttivazioneMagFam)
        {
            string errore = "";

            try
            {
                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                {
                    dtmf.NotificaRichiesta(idAttivazioneMagFam);
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
        public JsonResult PulsantiNotificaAttivaMagFam(decimal idAttivazioneMagFam)
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
            bool trasfSolaLettura = false;



            try
            {
                amministratore = Utility.Amministratore();

                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                {
                    dtmf.SituazioneMagFamPartenza(idAttivazioneMagFam, out rinunciaMagFam,
                        out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                        out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario,
                        out trasfSolaLettura);
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
                        trasfSolaLettura = trasfSolaLettura,
                        err = errore
                    });

        }

        public ActionResult MaggiorazioniFamiliari(decimal idTrasferimento, EnumCallElenco callElenco = EnumCallElenco.Coniuge)
        {
            MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel();

            try
            {
                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                {
                    mfm = dtmf.GetMaggiorazioniFamiliariByID(idTrasferimento);
                    if (mfm?.idMaggiorazioniFamiliari > 0)
                    {
                        using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                        {
                            var amf = dtamf.GetAttivazioneMagFamIniziale(mfm.idMaggiorazioniFamiliari);

                            ViewData.Add("idAttivazioneMagFam", amf.idAttivazioneMagFam);
                        }
                    }
                    else
                    {
                        throw new Exception("Maggiorazione familiare non trovata. IDTrasferimento: " + idTrasferimento);
                    }

                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


            ViewData.Add("callElenco", callElenco);

            return PartialView();
        }


        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult ElencoFigli(decimal idAttivazioneMagFam)
        {
            List<ElencoFamiliariModel> lefm = new List<ElencoFamiliariModel>();

            try
            {
                using (dtFigli dtf = new dtFigli())
                {
                    List<FigliModel> lfm = dtf.GetListaFigliByIdAttivazione(idAttivazioneMagFam).ToList();

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
                                        idAttivazioneMagFam = e.idAttivazioneMagFam,
                                        Nominativo = e.cognome + " " + e.nome,
                                        CodiceFiscale = e.codiceFiscale,
                                        dataInizio = e.dataInizio,
                                        dataFine = e.dataFine,
                                        parentela = EnumParentela.Figlio,
                                        idAltriDati = dtadf.GetAltriDatiFamiliariFiglio(e.idFigli, idAttivazioneMagFam).idAltriDatiFam,
                                        Documenti = dtd.GetDocumentiIdentitaFigli(e.idFigli, idAttivazioneMagFam)
                                    };

                                    lefm.Add(efm);
                                }
                            }
                        }
                    }



                    bool solaLettura = false;
                    solaLettura = this.SolaLetturaPartenza(idAttivazioneMagFam);
                    ViewData.Add("solaLettura", solaLettura);

                    ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);

                    return PartialView(lefm);
                }


            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult ElencoConiuge(decimal idAttivazioneMagFam)
        {
            List<ElencoFamiliariModel> lefm = new List<ElencoFamiliariModel>();

            try
            {
                using (dtConiuge dtc = new dtConiuge())
                {

                    List<ConiugeModel> lcm = dtc.GetListaConiugeByIdAttivazione(idAttivazioneMagFam).ToList();

                    if (lcm?.Any() ?? false)
                    {
                        using (dtDocumenti dtd = new dtDocumenti())
                        {
                            using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                            {
                                using (dtPensione dtp = new dtPensione())
                                {
                                    lefm.AddRange(lcm.Select(e => new ElencoFamiliariModel()
                                    {
                                        idMaggiorazioniFamiliari = e.idMaggiorazioniFamiliari,
                                        idFamiliare = e.idConiuge,
                                        idAttivazioneMagFam = e.idAttivazioneMagFam,
                                        Nominativo = e.cognome + " " + e.nome,
                                        CodiceFiscale = e.codiceFiscale,
                                        dataInizio = e.dataInizio,
                                        dataFine = e.dataFine,
                                        parentela = EnumParentela.Coniuge,
                                        idAltriDati = dtadf.GetAlttriDatiFamiliariConiuge(e.idConiuge, idAttivazioneMagFam).idAltriDatiFam,
                                        Documenti = dtd.GetDocumentiIdentitaConiuge(e.idConiuge, idAttivazioneMagFam),
                                        HasPensione = dtp.HasPensione(e.idConiuge, idAttivazioneMagFam)
                                    }));
                                }
                            }
                        }
                    }

                }

                bool solaLettura = false;
                solaLettura = this.SolaLetturaPartenza(idAttivazioneMagFam);
                ViewData.Add("solaLettura", solaLettura);

                ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);

                return PartialView(lefm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NuovoFiglio(decimal idAttivazioneMagFam)
        {
            FigliModel fm = new FigliModel();
            List<SelectListItem> lTipologiaFiglio = new List<SelectListItem>();
            var r = new List<SelectListItem>();

            try
            {
                using (dtTipologiaFiglio dttf = new dtTipologiaFiglio())
                {
                    var ltfm = dttf.GetListTipologiaFiglio().ToList();

                    if (ltfm?.Any() ?? false)
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


            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewData.Add("lTipologiaFiglio", lTipologiaFiglio);
            ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);

            return PartialView(fm);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NuovoConiuge(decimal idAttivazioneMagFam)
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

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tm = dtt.GetTrasferimentoByIdAttMagFam(idAttivazioneMagFam);

                    ViewData.Add("Trasferimento", tm);
                }

            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }



            ViewBag.lTipologiaConiuge = lTipologiaConiuge;
            ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);


            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciFiglio(FigliModel fm)
        {

            try
            {

                if (ModelState.IsValid)
                {
                    try
                    {
                        using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                        {
                            dtmf.InserisciFiglioMagFam(fm);
                        }
                    }
                    catch (Exception ex)
                    {

                        ModelState.AddModelError("", ex.Message);

                        List<SelectListItem> lTipologiaFiglio = new List<SelectListItem>();

                        var r = new List<SelectListItem>();

                        using (dtTipologiaFiglio dttf = new dtTipologiaFiglio())
                        {
                            var ltfm = dttf.GetListTipologiaFiglio().ToList();

                            if (ltfm?.Any() ?? false)
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
                            TrasferimentoModel tm = new TrasferimentoModel();

                            tm = dtt.GetTrasferimentoByIdAttMagFam(fm.idAttivazioneMagFam);

                            ViewData.Add("Trasferimento", tm);
                        }

                        ViewData["lTipologiaFiglio"] = lTipologiaFiglio;
                        ViewData.Add("idAttivazioneMagFam", fm.idAttivazioneMagFam);

                        return PartialView("NuovoFiglio", fm);
                    }

                }
                else
                {
                    List<SelectListItem> lTipologiaFiglio = new List<SelectListItem>();
                    var r = new List<SelectListItem>();

                    using (dtTipologiaFiglio dttf = new dtTipologiaFiglio())
                    {
                        var ltfm = dttf.GetListTipologiaFiglio().ToList();

                        if (ltfm?.Any() ?? false)
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
                        TrasferimentoModel tm = new TrasferimentoModel();

                        tm = dtt.GetTrasferimentoByIdAttMagFam(fm.idAttivazioneMagFam);

                        ViewData.Add("Trasferimento", tm);
                    }

                    ViewData["lTipologiaFiglio"] = lTipologiaFiglio;
                    ViewData.Add("idAttivazioneMagFam", fm.idAttivazioneMagFam);

                    return PartialView("NuovoFiglio", fm);
                }

                return RedirectToAction("ElencoFigli", new { idAttivazioneMagFam = fm.idAttivazioneMagFam });
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciConiuge(ConiugeModel cm, decimal idAttivazioneMagFam)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    try
                    {
                        using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                        {

                            dtmf.InserisciConiugeMagFam(cm);
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

                    return PartialView("NuovoConiuge", cm);
                }
            }
            catch (Exception ex)
            {
                PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


            return RedirectToAction("ElencoConiuge", new { idAttivazioneMagFam = idAttivazioneMagFam });
        }

        [HttpPost]
        public ActionResult ModificaConiuge(decimal idConiuge, decimal idAttivazioneMagFam)
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

                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                {
                    var mf = dtmf.GetMaggiorazioniFamiliaribyConiuge(idConiuge);
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var tm = dtt.GetTrasferimentoByIDMagFam(mf.idMaggiorazioniFamiliari);

                        ViewData.Add("Trasferimento", tm);
                    }
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);
            return PartialView(cm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfermaModificaConiuge(ConiugeModel cm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                    {
                        dtmf.ModificaConiuge(cm);
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

                    using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                    {
                        var mf = dtmf.GetMaggiorazioniFamiliaribyConiuge(cm.idConiuge);
                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            var tm = dtt.GetTrasferimentoByIDMagFam(mf.idMaggiorazioniFamiliari);

                            ViewData.Add("Trasferimento", tm);
                        }
                    }

                    ViewData.Add("idAttivazioneMagFam", cm.idAttivazioneMagFam);

                    return PartialView("ModificaConiuge", cm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return RedirectToAction("ElencoConiuge",
                new { idAttivazioneMagFam = cm.idAttivazioneMagFam });
        }

        public ActionResult MessaggioAnnullaMF(decimal idAttMagFam)
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
                            var t = dtt.GetTrasferimentoByIdAttMagFam(idAttMagFam);

                            if (t?.idTrasferimento > 0)
                            {
                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                var uff = dtu.GetUffici(t.idUfficio);

                                msg.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaMaggiorazioniFamiliari, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString());
                                ViewBag.idTrasferimento = t.idTrasferimento;
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
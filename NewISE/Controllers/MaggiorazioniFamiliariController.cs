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
using MaggiorazioniFamiliariModel = NewISE.Models.DBModel.MaggiorazioniFamiliariModel;

namespace NewISE.Controllers
{
    public class MaggiorazioniFamiliariController : Controller
    {
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
        public JsonResult NotificaRichiesta(decimal idMaggiorazioniFamiliari)
        {
            string errore = "";

            try
            {
                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                {
                    dtmf.NotificaRichiesta(idMaggiorazioniFamiliari);
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

            try
            {
                amministratore = Utility.Amministratore();
                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                {
                    dtmf.SituazioneMagFam(idMaggiorazioniFamiliari, out rinunciaMagFam,
                        out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                        out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli);
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
                        err = errore
                    });

        }

        public ActionResult MaggiorazioniFamiliari(decimal idTrasferimento, bool callConiuge = true)
        {
            MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel();

            using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
            {
                var lmfm = dtmf.GetListaMaggiorazioniFamiliariByIDTrasf(idTrasferimento);
                if (lmfm?.Any() ?? false)
                {
                    mfm = lmfm.OrderBy(a => a.idMaggiorazioniFamiliari).First();
                }

            }

            ViewBag.idTrasferimento = idTrasferimento;
            ViewData.Add("callConiuge", callConiuge);
            return PartialView(mfm);
        }


        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult ElencoFigli(decimal idMaggiorazioniFamiliari)
        {
            List<ElencoFamiliariModel> lefm = new List<ElencoFamiliariModel>();

            try
            {
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
                                        Documenti = dtd.GetDocumentiByIdTable(e.idFigli, EnumTipoDoc.DocumentoFamiliareFiglio_MaggiorazioniFamiliari4, EnumParentela.Figlio)
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

                        bool solaLettura = false;

                        dtmf.SituazioneMagFam(idMaggiorazioniFamiliari, out rinunciaMagFam,
                            out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                            out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli);

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


        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult ElencoConiuge(decimal idMaggiorazioniFamiliari)
        {
            List<ElencoFamiliariModel> lefm = new List<ElencoFamiliariModel>();

            try
            {

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
                                            Documenti = dtd.GetDocumentiByIdTable(e.idConiuge, EnumTipoDoc.DocumentoFamiliareConiuge_MaggiorazioniFamiliari4, EnumParentela.Coniuge),
                                            HasPensione = dtp.HasPensione(e.idConiuge)
                                        };

                                        lefm.Add(efm);
                                    }
                                }

                            }
                        }
                    }




                }

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

                    bool solaLettura = false;

                    dtmf.SituazioneMagFam(idMaggiorazioniFamiliari, out rinunciaMagFam,
                        out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                        out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli);

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
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NuovoFiglio(decimal idMaggiorazioniFamiliari)
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
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewData.Add("lTipologiaFiglio", lTipologiaFiglio);
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);

            return PartialView(fm);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NuovoConiuge(decimal idMaggiorazioniFamiliari)
        {
            //ConiugeModel cm = new ConiugeModel();
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

                //using (dtTrasferimento dtt = new dtTrasferimento())
                //{
                //    var tm = dtt.GetTrasferimentoByIDMagFam(idMaggiorazioniFamiliari);

                //}
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }



            ViewBag.lTipologiaConiuge = lTipologiaConiuge;
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);


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
                            dtmf.InserisciFiglio(fm);
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


                        ViewData["lTipologiaFiglio"] = lTipologiaFiglio;
                        ViewData.Add("idMaggiorazioniFamiliari", fm.idMaggiorazioniFamiliari);
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


                    ViewData["lTipologiaFiglio"] = lTipologiaFiglio;
                    ViewData.Add("idMaggiorazioniFamiliari", fm.idMaggiorazioniFamiliari);

                    return PartialView("NuovoFiglio", fm);
                }

                return RedirectToAction("ElencoFigli", new { idMAggiorazioniFamiliari = fm.idMaggiorazioniFamiliari });
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciConiuge(ConiugeModel cm, decimal idMaggiorazioniFamiliari)
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


                        ViewBag.lTipologiaConiuge = lTipologiaConiuge;
                        ViewData.Add("idMaggiorazioniFamiliari", cm.idMaggiorazioniFamiliari);
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


                    ViewBag.lTipologiaConiuge = lTipologiaConiuge;
                    ViewData.Add("idMaggiorazioniFamiliari", cm.idMaggiorazioniFamiliari);

                    return PartialView("NuovoConiuge", cm);
                }
            }
            catch (Exception ex)
            {
                PartialView("ErrorPartial");
            }


            return RedirectToAction("ElencoConiuge", new { idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari });
        }

        [HttpPost]
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
                    return PartialView("ModificaConiuge", cm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return RedirectToAction("ElencoConiuge",
                new { idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari });
        }
    }
}
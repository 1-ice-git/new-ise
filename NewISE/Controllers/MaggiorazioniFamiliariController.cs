﻿using NewISE.Models.DBModel;
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

namespace NewISE.Controllers
{
    public class MaggiorazioniFamiliariController : Controller
    {
        public ActionResult MaggiorazioniFamiliari(decimal idTrasferimento, bool callConiuge = true)
        {
            ViewBag.idTrasferimento = idTrasferimento;
            ViewData.Add("callConiuge", callConiuge);
            return PartialView();
        }


        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult ElencoFigli(decimal idTrasferimento)
        {
            List<ElencoFamiliariModel> lefm = new List<ElencoFamiliariModel>();
            decimal idMaggiorazioneFiglio = 0;

            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tr = dtt.GetSoloTrasferimentoById(idTrasferimento);
                    if (tr != null && tr.HasValue())
                    {
                        using (dtMaggiorazioniFigli dtmf = new dtMaggiorazioniFigli())
                        {
                            MaggiorazioniFigliModel mfm = dtmf.GetMaggiorazioneFigli(idTrasferimento, tr.dataPartenza);

                            if (mfm != null && mfm.HasValue())
                            {
                                idMaggiorazioneFiglio = mfm.idMaggiorazioneFigli;
                                using (dtFigli dtf = new dtFigli())
                                {
                                    mfm.Figli = dtf.GetListaFigli(mfm.idMaggiorazioneFigli).ToList();

                                    if (mfm.Figli?.Any() ?? false)
                                    {
                                        using (dtDocumenti dtd = new dtDocumenti())
                                        {
                                            using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                                            {
                                                foreach (var figlio in mfm.Figli)
                                                {
                                                    var adf =
                                                        dtadf.GetAltriDatiFamiliariFiglio(figlio.idMaggiorazioneFigli);
                                                    var ldm = dtd.GetDocumentiByIdFiglio(figlio.idMaggiorazioneFigli);

                                                    ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                                    {
                                                        id = mfm.idMaggiorazioneFigli,
                                                        idTrasferimento = idTrasferimento,
                                                        idFamiliare = mfm.idMaggiorazioneFigli,
                                                        Nominativo = figlio.nominativo,
                                                        CodiceFiscale = figlio.codiceFiscale,
                                                        dataInizio = figlio.dataInizio,
                                                        dataFine = figlio.dataFine,
                                                        parentela = EnumParentela.Figlio,
                                                        idAltriDati = adf.idAltriDatiFam > 0 ? adf.idAltriDatiFam : 0,
                                                        Documenti = ldm,
                                                    };

                                                    lefm.Add(efm);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //ViewData.Add("callConiuge", false);
                ViewData.Add("idTrasferimento", idTrasferimento);
                ViewData.Add("idMaggiorazioneFiglio", idMaggiorazioneFiglio);

                return PartialView(lefm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }


        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult ElencoConiuge(decimal idTrasferimento)
        {
            List<ElencoFamiliariModel> lefm = new List<ElencoFamiliariModel>();

            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tr = dtt.GetSoloTrasferimentoById(idTrasferimento);
                    if (tr != null && tr.HasValue())
                    {
                        #region commentata

                        //using (dtMaggiorazioniFigli dtmf = new dtMaggiorazioniFigli())
                        //{
                        //    MaggiorazioniFigliModel mf = dtmf.GetMaggiorazioneFigli(tr.idTrasferimento, tr.dataPartenza);
                        //    if (mf != null && mf.HasValue())
                        //    {
                        //        using (dtFigli dtf = new dtFigli())
                        //        {
                        //            mf.LFigli = dtf.GetListaFigli(mf.idMaggiorazioneFigli);
                        //            if (mf.LFigli != null && mf.LFigli.Count > 0)
                        //            {
                        //                using (dtDocumenti dtd = new dtDocumenti())
                        //                {
                        //                    using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                        //                    {
                        //                        foreach (var item in mf.LFigli)
                        //                        {
                        //                            ElencoFamiliariModel efm = new ElencoFamiliariModel()
                        //                            {
                        //                                id = item.idMaggiorazioneFigli,
                        //                                idTrasferimento = idTrasferimento,
                        //                                idFamiliare = item.idFigli,
                        //                                Nominativo = item.cognome + " " + item.nome,
                        //                                CodiceFiscale = item.codiceFiscale,
                        //                                dataInizio = item.MaggiorazioniFigli.dataInizioValidita,
                        //                                dataFine = item.MaggiorazioniFigli.dataFineValidita,
                        //                                parentela = EnumParentela.Figlio,
                        //                                idAltriDati = dtadf.GetAltriDatiFamiliariFiglio(item.idFigli).idAltriDatiFam,
                        //                                Documento = dtd.GetDocumentiByIdFiglio(idFiglio: item.idFigli),


                        //                            };

                        //                            lefm.Add(efm);
                        //                        }
                        //                    }

                        //                }
                        //            }

                        //        }
                        //    }


                        //}

                        #endregion

                        using (dtMaggiorazioneConiuge dtmc = new dtMaggiorazioneConiuge())
                        {
                            MaggiorazioneConiugeModel mcm = dtmc.GetMaggiorazioneConiuge(tr.idTrasferimento,
                                tr.dataPartenza);
                            if (mcm != null && mcm.HasValue())
                            {
                                using (dtConiuge dtc = new dtConiuge())
                                {
                                    mcm.Coniuge = dtc.GetConiuge(mcm.idMaggiorazioneConiuge);

                                    if (mcm.Coniuge != null && mcm.Coniuge.HasValue())
                                    {
                                        using (dtDocumenti dtd = new dtDocumenti())
                                        {
                                            using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                                            {
                                                var adf = dtadf.GetAltriDatiFamiliariConiuge(mcm.idMaggiorazioneConiuge);
                                                var ldm = dtd.GetDocumentiByIdMagConiuge(idMagConiuge: mcm.idMaggiorazioneConiuge);

                                                ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                                {
                                                    id = mcm.idMaggiorazioneConiuge,
                                                    idTrasferimento = idTrasferimento,
                                                    idFamiliare = mcm.idMaggiorazioneConiuge,
                                                    Nominativo = mcm.Coniuge.nominativo,
                                                    CodiceFiscale = mcm.Coniuge.codiceFiscale,
                                                    dataInizio = mcm.dataInizioValidita,
                                                    dataFine = mcm.dataFineValidita,
                                                    parentela = EnumParentela.Coniuge,
                                                    idAltriDati = adf.idAltriDatiFam > 0 ? adf.idAltriDatiFam : 0,
                                                    Documenti = ldm,
                                                };

                                                using (dtPensione dtp = new dtPensione())
                                                {
                                                    efm.HasPensione = dtp.HasPensione(mcm.idMaggiorazioneConiuge);
                                                }

                                                lefm.Add(efm);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                ViewBag.idTrasferimento = idTrasferimento;

                return PartialView(lefm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }


        public ActionResult NuovoFiglio(decimal idTrasferimento, decimal idMaggiorazioneFigli)
        {
            Figli_V_Model fm = new Figli_V_Model();
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

                return PartialView("ErrorPartial");
            }

            ViewData.Add("lTipologiaFiglio", lTipologiaFiglio);
            ViewData.Add("idTrasferimento", idTrasferimento);
            ViewData.Add("idMaggiorazioneFigli", idMaggiorazioneFigli);

            fm.idMaggiorazioneFigli = idMaggiorazioneFigli;

            return PartialView(fm);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NuovoConiuge(decimal idTrasferimento)
        {
            MaggiorazioneConiugeVModel mcvm = new MaggiorazioneConiugeVModel();
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

                return PartialView("ErrorPartial");
            }



            ViewBag.lTipologiaConiuge = lTipologiaConiuge;
            ViewData.Add("idTrasferimento", idTrasferimento);
            mcvm.idTrasferimento = idTrasferimento;

            return PartialView(mcvm);
        }



        public ActionResult InserisciFiglio(Figli_V_Model fm, decimal idTrasferimento)
        {

            try
            {

                if (ModelState.IsValid)
                {
                    using (dtMaggiorazioniFigli dtmf = new dtMaggiorazioniFigli())
                    {
                        dtmf.IserisciFiglio(fm, idTrasferimento);
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
                    ViewData.Add("idTrasferimento", idTrasferimento);
                    ViewData.Add("idMaggiorazioneFigli", fm.idMaggiorazioneFigli);

                    return PartialView("NuovoFiglio", fm);
                }

                return RedirectToAction("ElencoFiglio", new { idTrasferimento = idTrasferimento });
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial");
            }

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciConiuge(MaggiorazioneConiugeVModel mcvm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        using (dtMaggiorazioneConiuge dtmc = new dtMaggiorazioneConiuge())
                        {
                            dtmc.InserisciConiuge(mcvm);
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
                        ViewBag.idTrasferimento = mcvm.idTrasferimento;

                        return PartialView("NuovoConiuge", mcvm);
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
                    ViewBag.idTrasferimento = mcvm.idTrasferimento;

                    return PartialView("NuovoConiuge", mcvm);
                }
            }
            catch (Exception ex)
            {
                PartialView("ErrorPartial");
            }


            return RedirectToAction("ElencoConiuge", new { idTrasferimento = mcvm.idTrasferimento });
        }

        [HttpPost]
        public ActionResult ModificaConiuge(decimal idMaggiorazioneConiuge, decimal idTrasferimento)
        {
            ConiugeModel cm = new ConiugeModel();

            try
            {
                using (dtConiuge dtc = new dtConiuge())
                {
                    cm = dtc.GetConiuge(idMaggiorazioneConiuge);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }

            ViewData.Add("idTrasferimento", idTrasferimento);
            return PartialView(cm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfermaModificaConiuge(ConiugeModel cm, decimal idTrasferimento)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtConiuge dtc = new dtConiuge())
                    {
                        dtc.EditConiuge(cm);
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica del coniuge", "CONIUGE",
                            new ModelDBISE(), idTrasferimento, cm.idMaggiorazioneConiuge);
                    }
                }
                else
                {
                    ViewData.Add("idTrasferimento", idTrasferimento);
                    return PartialView("ModificaConiuge", cm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }

            return RedirectToAction("MaggiorazioniFamiliari",
                new { idTrasferimento = idTrasferimento, callConiuge = true });
        }
    }
}
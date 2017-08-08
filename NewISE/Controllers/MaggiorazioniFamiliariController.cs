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
        public ActionResult MaggiorazioniFamiliari(decimal idTrasferimento, bool callConiuge = true)
        {
            MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel();

            using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
            {
                mfm = dtmf.GetMaggiorazioniFamiliariByIDTrasf(idTrasferimento);

            }


            ViewBag.idTrasferimento = idTrasferimento;
            ViewData.Add("callConiuge", callConiuge);
            return PartialView(mfm);
        }


        //[AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        //public ActionResult ElencoFigli(decimal idTrasferimento)
        //{
        //    List<ElencoFamiliariModel> lefm = new List<ElencoFamiliariModel>();
        //    decimal idMaggiorazioneFiglio = 0;

        //    try
        //    {
        //        using (dtTrasferimento dtt = new dtTrasferimento())
        //        {
        //            var tr = dtt.GetSoloTrasferimentoById(idTrasferimento);
        //            if (tr != null && tr.HasValue())
        //            {
        //                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
        //                {
        //                    //MaggiorazioniFigliModel mfm = dtmf.GetMaggiorazioneFigli(idTrasferimento, tr.dataPartenza);


        //                    MaggiorazioniFamiliariModel mfm = dtmf.GetMaggiorazioniFamiliari()







        //                    if (mfm != null && mfm.HasValue())
        //                    {
        //                        idMaggiorazioneFiglio = mfm.idMaggiorazioneFigli;
        //                        using (dtFigli dtf = new dtFigli())
        //                        {
        //                            mfm.Figli = dtf.GetListaFigli(mfm.idMaggiorazioneFigli).ToList();

        //                            if (mfm.Figli?.Any() ?? false)
        //                            {
        //                                using (dtDocumenti dtd = new dtDocumenti())
        //                                {
        //                                    using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
        //                                    {
        //                                        //foreach (var figlio in mfm.Figli)
        //                                        //{
        //                                        //    var adf = dtadf.GetAltriDatiFamiliariFiglio(figlio.idMaggiorazioneFigli);
        //                                        //    var ldm = dtd.GetDocumentiByIdFiglio(figlio.idMaggiorazioneFigli);

        //                                        //    ElencoFamiliariModel efm = new ElencoFamiliariModel()
        //                                        //    {
        //                                        //        id = mfm.idMaggiorazioneFigli,
        //                                        //        idTrasferimento = idTrasferimento,
        //                                        //        idFamiliare = mfm.idMaggiorazioneFigli,
        //                                        //        Nominativo = figlio.nominativo,
        //                                        //        CodiceFiscale = figlio.codiceFiscale,
        //                                        //        dataInizio = figlio.dataInizio,
        //                                        //        dataFine = figlio.dataFine,
        //                                        //        parentela = EnumParentela.Figlio,
        //                                        //        idAltriDati = adf.idAltriDatiFam > 0 ? adf.idAltriDatiFam : 0,
        //                                        //        Documenti = ldm,
        //                                        //    };

        //                                        //    lefm.Add(efm);
        //                                        //}
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        //ViewData.Add("callConiuge", false);
        //        ViewData.Add("idTrasferimento", idTrasferimento);
        //        ViewData.Add("idMaggiorazioneFiglio", idMaggiorazioneFiglio);

        //        return PartialView(lefm);
        //    }
        //    catch (Exception ex)
        //    {
        //        return PartialView("ErrorPartial");
        //    }
        //}


        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult ElencoConiuge(decimal idMaggiorazioniFamiliari)
        {
            List<ElencoFamiliariModel> lefm = new List<ElencoFamiliariModel>();

            try
            {

                using (dtConiuge dtc = new dtConiuge())
                {
                    var lcm = dtc.GetListaConiuge(idMaggiorazioniFamiliari).ToList();

                    if (lcm?.Any() ?? false)
                    {
                        using (dtDocumenti dtd = new dtDocumenti())
                        {
                            using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                            {
                                foreach (var e in lcm)
                                {
                                    ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                    {
                                        id = e.idMaggiorazioneFamiliari,
                                        idFamiliare = e.idConiuge,
                                        Nominativo = e.cognome + " " + e.nome,
                                        CodiceFiscale = e.codiceFiscale,
                                        dataInizio = e.dataInizio,
                                        dataFine = e.dataFine,
                                        parentela = EnumParentela.Coniuge,
                                        idAltriDati = dtadf.GetAlttriDatiFamiliariConiuge(e.idConiuge).idAltriDatiFam,
                                        Documenti = dtd.GetDocumentiByIdConiuge(e.idConiuge),
                                    };
                                }
                            }
                        }
                    }




                }


                //ViewBag.idTrasferimento = idTrasferimento;

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
        public ActionResult NuovoConiuge(decimal idMaggiorazioniFamiliari)
        {
            MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel();
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
            //ViewData.Add("idTrasferimento", idTrasferimento);
            //mcvm.idTrasferimento = idTrasferimento;
            mfm.idMaggiorazioneFamiliari = idMaggiorazioniFamiliari;

            return PartialView(mfm);
        }



        public ActionResult InserisciFiglio(Figli_V_Model fm, decimal idTrasferimento)
        {

            try
            {

                if (ModelState.IsValid)
                {
                    using (dtFigli dtf = new dtFigli())
                    {
                        //dtf.IserisciFiglio(fm, idTrasferimento);
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
        public ActionResult InserisciConiuge(ConiugeModel cm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        using (dtMaggiorazioniFamiliari dtmc = new dtMaggiorazioniFamiliari())
                        {
                            dtmc.InserisciConiuge(cm);
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
                    //ViewBag.idTrasferimento = mcvm.idTrasferimento;

                    return PartialView("NuovoConiuge", cm);
                }
            }
            catch (Exception ex)
            {
                PartialView("ErrorPartial");
            }


            return RedirectToAction("ElencoConiuge", new { idMaggiorazioniFamiliari = cm.idMaggiorazioneFamiliari });
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
                return PartialView("ErrorPartial");
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
                    using (dtConiuge dtc = new dtConiuge())
                    {
                        dtc.EditConiuge(cm);

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
                return PartialView("ErrorPartial");
            }

            return RedirectToAction("ElencoConiuge",
                new { idMaggiorazioniFamiliari = cm.idMaggiorazioneFamiliari });
        }
    }
}
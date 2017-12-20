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

namespace NewISE.Controllers
{
    public class RichiamoController : Controller
    {
        // GET: Richiamo
        public ActionResult Index()
        {
            return View();
            //return PartialView("Richiamo");
        }

        public ActionResult Richiamo(decimal idTrasferimento)
        {
            try
            {
                ViewData["idTrasferimento"] = idTrasferimento;
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("Richiamo");
        }

        public JsonResult VerificaRichiamo(decimal idTrasferimento)
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
                        return Json(new { VerificaRichiamo = 1 });
                    }
                    else
                    {
                        return Json(new { VerificaRichiamo = 0 });

                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult DeleteRichiamo(decimal idTrasfRichiamo, decimal idTrasferimento)
        {
            ViewData["idTrasfRichiamo"] = idTrasfRichiamo;
            ViewData["idTrasferimento"] = idTrasferimento;

            //  ViewBag.idSospensione = idSospensione;
            //List<SelectListItem> lTipologiaSospensione = new List<SelectListItem>();

            RichiamoModel tmp = new RichiamoModel();
            try
            {
                //using (dtRichiamo ds = new dtRichiamo())
                //{
                //    tmp = ds.GetRichiamoPerEliminazione(idTrasfRichiamo);
                //}
              

                //var r = new List<SelectListItem>();

                //using (dtSospensione dttc = new dtSospensione())
                //{
                //    var ltcm = dttc.GetListTipologiaSospensione();

                //    if (ltcm != null && ltcm.Count > 0)
                //    {
                //        r = (from t in ltcm
                //             select new SelectListItem()
                //             {
                //                 Text = t.Descrizione,
                //                 Value = t.idTipologiaSospensione.ToString()
                //             }).ToList();
                //        //  r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                //    }
                //    lTipologiaSospensione = r;
                //}
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            //ViewBag.lTipologiaRichiamo = lTipologiaRichiamo;
            tmp.idTrasferimento = (decimal)ViewData["idTrasferimento"];
            return PartialView(tmp);
        }
        public ActionResult Elimina_Richiamo(decimal idTrasfRichiamo, bool permesso = true)
        {
            //decimal idSospensione =(decimal)ViewBag.idSospensione;
            using (dtTrasferimento dtt = new dtTrasferimento())
            {
                var tm = dtt.GetTrasferimentoByRichiamo(idTrasfRichiamo);
                
                ViewData["idTrasferimento"] = tm.idTrasferimento;
            }
            
            RichiamoModel tmp = new RichiamoModel();
            
            using (dtRichiamo ds = new dtRichiamo())
            {
                ds.Delete_Richiamo(idTrasfRichiamo, permesso);
            }
            return PartialView("AttivitaRichiamo");

        }

        public ActionResult DatiTabElencoRichiamo(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            List<RichiamoModel> tmp = new List<RichiamoModel>();
            try
            {
                using (dtRichiamo dtcal = new dtRichiamo())
                {
                    //tmp.AddRange(dtcal.GetLista_Richiamo(idTrasferimento));
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView(tmp);

        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult ElencoRichiamo(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;

            try
            {
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult NuovoRichiamo(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;

            return PartialView();
        }

        public ActionResult AttivitaRichiamo(decimal idTrasferimento)
        {
            try
            {
                ViewData["idTrasferimento"] = idTrasferimento;
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("AttivitaRichiamo");
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult InserisciRichiamo(RichiamoModel sm, decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            string[] my_array = null;
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtRichiamo dtsosp = new dtRichiamo())
                    {
                        my_array = dtsosp.InserisciRichiamo(sm, sm.IDTRASFRICHIAMO);
                        if (my_array[0] != "0")
                        {
                            List<SelectListItem> lTipologiaRichiamo;
                            //ModelState.AddModelError("RangeDate", my_array[1].ToString());

                            lTipologiaRichiamo = new List<SelectListItem>();
                            var r = new List<SelectListItem>();
                            using (dtRichiamo dttc = new dtRichiamo())
                            {
                                //var ltcm = dttc.GetLista_Richiamo();

                                //if (ltcm != null && ltcm.Count > 0)
                                //{
                                //    r = (from t in ltcm
                                //         select new SelectListItem()
                                //         {
                                //             Text = t.Descrizione,
                                //             Value = t.idTipologiaSospensione.ToString()
                                //         }).ToList();
                                //    //  r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                                //}
                                lTipologiaRichiamo = r;
                            }
                            ViewBag.lTipologiaRichiamo = lTipologiaRichiamo;
                            return PartialView("NuovoRichiamo");
                            
                        }
                    }
                }
                else
                {
                    List<SelectListItem> lTipologiaRichiamo;

                    lTipologiaRichiamo = new List<SelectListItem>();
                    var r = new List<SelectListItem>();
                    using (dtRichiamo dttc = new dtRichiamo())
                    {
                        //var ltcm = dttc.GetListTipologiaSospensione();

                        //if (ltcm != null && ltcm.Count > 0)
                        //{
                        //    r = (from t in ltcm
                        //         select new SelectListItem()
                        //         {
                        //             Text = t.Descrizione,
                        //             Value = t.idTipologiaSospensione.ToString()
                        //         }).ToList();
                        //    //  r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                        //}
                        lTipologiaRichiamo = r;
                    }
                    ViewBag.lTipologiaRichiamo = lTipologiaRichiamo;
                    return PartialView("NuovaSospensione");
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = my_array[1] });
            }
            return PartialView("AttivitaSospensione");
        }
        public ActionResult EditRichiamo(decimal idTrasfRichiamo, decimal idTrasferimento)
        {
            ViewData["idTrasfRichiamo"] = idTrasfRichiamo;
            ViewData["idTrasferimento"] = idTrasferimento;
            RichiamoModel tmp = new RichiamoModel();
            using (dtRichiamo dts = new dtRichiamo())
            {
                tmp = dts.getRichiamoById(idTrasfRichiamo);
            }
            List<SelectListItem> lTipologiaRichiamo = new List<SelectListItem>();
            var r = new List<SelectListItem>();
            try
            {
                using (dtRichiamo dttc = new dtRichiamo())
                {
                    //var ltcm = dttc.GetListTipologiaSospensione();

                    //if (ltcm != null && ltcm.Count > 0)
                    //{
                    //    r = (from t in ltcm
                    //         select new SelectListItem()
                    //         {
                    //             Text = t.Descrizione,
                    //             Value = t.idTipologiaSospensione.ToString()
                    //         }).ToList();
                    //    //  r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                    //}
                    lTipologiaRichiamo = r;
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.lTipologiaRichiamo = lTipologiaRichiamo;
            tmp.idTrasferimento = (decimal)ViewData["idTrasferimento"];
            return PartialView(tmp);
        }
        public ActionResult ModificaRichiamo(RichiamoModel sm, decimal idTrasfRichiamo, decimal idTrasferimento)
        {
            string[] my_array = null;
            ViewData["idTrasferimento"] = idTrasferimento;
            try
            {
                using (dtRichiamo ds = new dtRichiamo())
                {
                    if (ModelState.IsValid)
                    {
                        my_array = ds.Modifica_Richiamo(sm);
                        //ModelState.AddModelError("RangeDate", my_array[1].ToString());
                    }
                    else
                    {
                        List<SelectListItem> lTipologiaRichiamo;
                        lTipologiaRichiamo = new List<SelectListItem>();
                        var r = new List<SelectListItem>();
                        using (dtRichiamo dttc = new dtRichiamo())
                        {
                            //var ltcm = dttc.GetLista_Richiamo();

                            //if (ltcm != null && ltcm.Count > 0)
                            //{
                            //    r = (from t in ltcm
                            //         select new SelectListItem()
                            //         {
                            //             Text = t.Descrizione,
                            //             Value = t.idTipologiaSospensione.ToString()
                            //         }).ToList();
                            //    //  r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                            //}
                            lTipologiaRichiamo = r;
                        }
                        ViewBag.lTipologiaRichiamo = lTipologiaRichiamo;
                        return PartialView("EditRichiamo");
                    }
                }
                return PartialView("AttivitaRichiamo");
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }






    }
}
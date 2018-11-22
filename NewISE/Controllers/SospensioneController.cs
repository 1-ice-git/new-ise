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
using NewISE.Models.Enumeratori;

namespace NewISE.Controllers
{

    public class SospensioneController : Controller
    {
        // GET: Sospensione
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult VerificaSospensione(decimal idTrasferimento)
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
                        return Json(new { VerificaSospensione = 1 });
                    }
                    else
                    {
                        return Json(new { VerificaSospensione = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }

        [Authorize(Roles = "1 ,2")]
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult DeleteSospensione(decimal idSospensione, decimal idTrasferimento)
        {
            ViewData["idSospensione"] = idSospensione;
            ViewData["idTrasferimento"] = idTrasferimento;

            //  ViewBag.idSospensione = idSospensione;
            List<SelectListItem> lTipologiaSospensione = new List<SelectListItem>();

            SospensioneModel tmp = new SospensioneModel();
            try
            {
                using (dtSospensione ds = new dtSospensione())
                {
                    tmp = ds.GetSospensionePerEliminazione(idSospensione);
                }
                //using (dtTrasferimento dtt = new dtTrasferimento())
                //{
                //    var tm = dtt.GetTrasferimentoByIdSosp(idSospensione);
                //    idTrasferimento = tm.idTrasferimento;
                //}

                var r = new List<SelectListItem>();

                using (dtSospensione dttc = new dtSospensione())
                {
                    var ltcm = dttc.GetListTipologiaSospensione();

                    if (ltcm != null && ltcm.Count > 0)
                    {
                        r = (from t in ltcm
                             select new SelectListItem()
                             {
                                 Text = t.Descrizione,
                                 Value = t.idTipologiaSospensione.ToString()
                             }).ToList();
                        //  r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                    }
                    lTipologiaSospensione = r;
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewBag.lTipologiaSospensione = lTipologiaSospensione;
            tmp.idTrasferimento = (decimal)ViewData["idTrasferimento"];
            return PartialView(tmp);
        }


        [Authorize(Roles = "1 ,2")]
        public ActionResult Elimina_Sospensione(decimal idSospensione, bool permesso = true)
        {
            //decimal idSospensione =(decimal)ViewBag.idSospensione;
            using (dtTrasferimento dtt = new dtTrasferimento())
            {
                var tm = dtt.GetTrasferimentoByIdSosp(idSospensione);
                ViewData["idTrasferimento"] = tm.idTrasferimento;
            }

            SospensioneModel tmp = new SospensioneModel();
            using (dtSospensione ds = new dtSospensione())
            {
                ds.Delete_Sospensione(idSospensione, permesso);
            }
            return PartialView("AttivitaSospensione");
        }

        public ActionResult DatiTabElencoSospensione(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            List<SospensioneModel> tmp = new List<SospensioneModel>();
            try
            {
                using (dtSospensione dtcal = new dtSospensione())
                {
                    tmp.AddRange(dtcal.GetLista_Sospensioni(idTrasferimento));
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView(tmp);

        }

        //[AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        //[Authorize(Roles = "1 ,2")]
        public ActionResult ElencoSospensioni(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;

            bool admin = Utility.Amministratore();
            ViewBag.Amministratore = admin;

            try
            {
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [Authorize(Roles = "1 ,2")]
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult NuovaSospensione(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            List<SelectListItem> lTipologiaSospensione;
            try
            {
                lTipologiaSospensione = new List<SelectListItem>();
                var r = new List<SelectListItem>();
                using (dtSospensione dttc = new dtSospensione())
                {
                    var ltcm = dttc.GetListTipologiaSospensione();

                    if (ltcm != null && ltcm.Count > 0)
                    {
                        r = (from t in ltcm
                             select new SelectListItem()
                             {
                                 Text = t.Descrizione,
                                 Value = t.idTipologiaSospensione.ToString()
                             }).ToList();
                        //  r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                    }
                    lTipologiaSospensione = r;
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.lTipologiaSospensione = lTipologiaSospensione;
            SospensioneModel tmp = new SospensioneModel();
            tmp.idTrasferimento = (decimal)ViewData["idTrasferimento"];
            return PartialView(tmp);
        }

        public ActionResult AttivitaSospensione(decimal idTrasferimento)
        {
            try
            {
                ViewData["idTrasferimento"] = idTrasferimento;
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("AttivitaSospensione");
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult InserisciSospensione(SospensioneModel sm, decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            string[] my_array = null;
            try
            {
                if (ModelState.IsValid)
                {
                    using (dtSospensione dtsosp = new dtSospensione())
                    {
                        my_array = dtsosp.InserisciSospensione(sm, sm.idTipoSospensione);
                        if (my_array[0] != "0")
                        {
                            List<SelectListItem> lTipologiaSospensione;
                            ModelState.AddModelError("RangeDate", my_array[1].ToString());

                            lTipologiaSospensione = new List<SelectListItem>();
                            var r = new List<SelectListItem>();
                            using (dtSospensione dttc = new dtSospensione())
                            {
                                var ltcm = dttc.GetListTipologiaSospensione();

                                if (ltcm != null && ltcm.Count > 0)
                                {
                                    r = (from t in ltcm
                                         select new SelectListItem()
                                         {
                                             Text = t.Descrizione,
                                             Value = t.idTipologiaSospensione.ToString()
                                         }).ToList();
                                    //  r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                                }
                                lTipologiaSospensione = r;
                            }
                            ViewBag.lTipologiaSospensione = lTipologiaSospensione;
                            return PartialView("NuovaSospensione");
                            //return PartialView("ErrorPartial", new MsgErr() { msg = my_array[1] });
                        }
                    }
                }
                else
                {
                    List<SelectListItem> lTipologiaSospensione = new List<SelectListItem>();

                    var r = new List<SelectListItem>();
                    using (dtSospensione dttc = new dtSospensione())
                    {
                        var ltcm = dttc.GetListTipologiaSospensione();

                        if (ltcm != null && ltcm.Count > 0)
                        {
                            r = (from t in ltcm
                                 select new SelectListItem()
                                 {
                                     Text = t.Descrizione,
                                     Value = t.idTipologiaSospensione.ToString()
                                 }).ToList();
                            //  r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                        }
                        lTipologiaSospensione = r;
                    }
                    ViewBag.lTipologiaSospensione = lTipologiaSospensione;
                    return PartialView("NuovaSospensione");
                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = my_array[1] });
            }
            return PartialView("AttivitaSospensione");
        }

        [Authorize(Roles = "1 ,2")]
        public ActionResult EditSospensione(decimal idSospensione, decimal idTrasferimento)
        {
            ViewData["idSospensione"] = idSospensione;
            ViewData["idTrasferimento"] = idTrasferimento;
            SospensioneModel tmp = new SospensioneModel();
            using (dtSospensione dts = new dtSospensione())
            {
                tmp = dts.getSospensioneById(idSospensione);
            }
            List<SelectListItem> lTipologiaSospensione = new List<SelectListItem>();
            var r = new List<SelectListItem>();
            try
            {
                using (dtSospensione dttc = new dtSospensione())
                {
                    var ltcm = dttc.GetListTipologiaSospensione();

                    if (ltcm != null && ltcm.Count > 0)
                    {
                        r = (from t in ltcm
                             select new SelectListItem()
                             {
                                 Text = t.Descrizione,
                                 Value = t.idTipologiaSospensione.ToString()
                             }).ToList();
                        //  r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                    }
                    lTipologiaSospensione = r;
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.lTipologiaSospensione = lTipologiaSospensione;
            tmp.idTrasferimento = (decimal)ViewData["idTrasferimento"];
            return PartialView(tmp);
        }

        [Authorize(Roles = "1 ,2")]
        public ActionResult ModificaSospensione(SospensioneModel sm, decimal idSospensione, decimal idTrasferimento)
        {
            string[] my_array = null;
            ViewData["idTrasferimento"] = idTrasferimento;
            try
            {
                using (dtSospensione ds = new dtSospensione())
                {
                    if (ModelState.IsValid)
                    {
                        my_array = ds.Modifica_Sospensione(sm);
                        ModelState.AddModelError("RangeDate", my_array[1].ToString());
                    }
                    else
                    {
                        List<SelectListItem> lTipologiaSospensione;
                        lTipologiaSospensione = new List<SelectListItem>();
                        var r = new List<SelectListItem>();
                        using (dtSospensione dttc = new dtSospensione())
                        {
                            var ltcm = dttc.GetListTipologiaSospensione();

                            if (ltcm != null && ltcm.Count > 0)
                            {
                                r = (from t in ltcm
                                     select new SelectListItem()
                                     {
                                         Text = t.Descrizione,
                                         Value = t.idTipologiaSospensione.ToString()
                                     }).ToList();
                                //  r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                            }
                            lTipologiaSospensione = r;
                        }
                        ViewBag.lTipologiaSospensione = lTipologiaSospensione;
                        return PartialView("EditSospensione");
                    }
                }
                return PartialView("AttivitaSospensione");
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
    }
}
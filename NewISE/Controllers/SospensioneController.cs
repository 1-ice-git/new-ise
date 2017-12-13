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
    public class SospensioneController : Controller
    {
        // GET: Sospensione
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult VerificaSospensione(string matricola = "")
        {
            try
            {
                if (matricola == string.Empty)
                {
                    throw new Exception("La matricola non risulta valorizzata.");
                }
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    dipInfoTrasferimentoModel trm = dtt.GetInfoTrasferimento(matricola);
                    if (trm != null && (trm.statoTrasferimento == EnumStatoTraferimento.Attivo ||
                        trm.statoTrasferimento == EnumStatoTraferimento.Da_Attivare))
                    {
                        ViewData["idTrasferimento"] = dtt.GetUltimoSoloTrasferimentoByMatricola(matricola).idTrasferimento;
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
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult DeleteSospensione(decimal idSospensione)
        {
            decimal idTrasferimento = 0;
            ViewData["idSospensione"] = idSospensione;
            ViewBag.idSospensione = idSospensione;
            SospensioneModel tmp = new SospensioneModel();
            try
            {
                using (dtSospensione ds = new dtSospensione())
                {
                    tmp = ds.GetSospensionePerEliminazione(idSospensione);
                }
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tm = dtt.GetTrasferimentoByIdSosp(idSospensione);
                    idTrasferimento = tm.idTrasferimento;
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewData["idSospensione"] = idSospensione;
            ViewData.Add("idTrasferimento", idTrasferimento);
            // ViewBag.matricola = matricola;
            return PartialView(tmp);
        }
        public void Elimina_Sospensione(decimal idSospensione, bool permesso = true)
        {

            //decimal idSospensione =(decimal)ViewBag.idSospensione;
            SospensioneModel tmp = new SospensioneModel();
            using (dtSospensione ds = new dtSospensione())
            {
                ds.Delete_Sospensione(idSospensione, permesso);
            }
            // return PartialView("AttivitaSospensioni");
        }

        public ActionResult DatiTabElencoSospensione(decimal idTrasferimento)
        {
            
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

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult ElencoSospensioni(decimal matricola)
        {
            try
            {
                ViewData["matricola"] = matricola;
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    TrasferimentoModel trm = dtt.GetUltimoSoloTrasferimentoByMatricola(matricola.ToString());
                    ViewData["idTrasferimento"] = trm.idTrasferimento;
                }

                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult NuovaSospensione(decimal matricola)
        {
            ViewData["matricola"] = matricola;
            ViewBag.matricola = matricola;
            using (dtTrasferimento dtt = new dtTrasferimento())
            {
                ViewData["idTrasferimento"] = dtt.GetUltimoSoloTrasferimentoByMatricola(matricola.ToString()).idTrasferimento;
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
            SospensioneModel tmp = new SospensioneModel();
            tmp.idTrasferimento = (decimal)ViewData["idTrasferimento"];
            return PartialView(tmp);
        }

        public ActionResult AttivitaSospensione(string matricola)
        {
            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tr = dtt.GetUltimoSoloTrasferimentoByMatricola(matricola);

                    if (tr != null && tr.HasValue())
                    {
                        ViewData["idTrasferimento"] = tr.idTrasferimento;
                        ViewBag.idTrasferimento = tr.idTrasferimento;
                    }
                    else
                    {
                        throw new Exception("Nessun trasferimento per la matricola (" + matricola + ")");
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.matricola = matricola;

            return PartialView("AttivitaSospensione");
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult InserisciSospensione(SospensioneModel sm, decimal matricola, decimal id_TipoSospensione)
        {
            ViewBag.matricola = matricola;
            try
            {
                using (dtSospensione dtsosp = new dtSospensione())
                {
                    dtsosp.InserisciSospensione(sm, id_TipoSospensione);

                }
                //if (true)
                //{
                //    //ModelState.AddModelError("ErroreRangeDate", "Impossibile inserire una sopsensione con il periodo già presente su una sopsensione esistente.");
                //    ViewBag.ModelMsg = ModelloMessaggi;
                //}
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView("AttivitaSospensione");
        }
        public ActionResult EditSospensione(decimal idSospensione,decimal matricola)
        {
            ViewData["idSospensione"] = idSospensione;
            ViewBag.matricola = matricola;
            SospensioneModel tmp = new SospensioneModel();
            using (dtSospensione dts = new dtSospensione())
            {
                tmp = dts.getSospensioneById(idSospensione);
                ViewData["idTrasferimento"] = tmp.idTrasferimento;
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
        public ActionResult ModificaSospensione(SospensioneModel sm, decimal idSospensione,decimal id_TipoSospensione,decimal matricola)
        {
            using (dtSospensione ds = new dtSospensione())
            {
                //SospensioneModel sm = ds.getSospensioneById(idSospensione);
                ds.Modifica_Sospensione(sm);
            }
            ViewBag.idTrasferimento = sm.idTrasferimento;
            ViewBag.matricola = matricola;
            

            return PartialView("AttivitaSospensione");
        }
        
    }
}
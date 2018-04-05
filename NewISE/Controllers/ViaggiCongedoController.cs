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
using System.IO;

namespace NewISE.Controllers
{

    //public enum EnumCallElenco
    //{
    //    Coniuge = 1,
    //    Figli = 2,
    //    Formulari = 3
    //}

    public class ViaggiCongedoController : Controller
    {

        //[NonAction]
        public ActionResult Index()
        {
            return View();
        }
       
        public JsonResult VerificaViaggiCongedo(decimal idTrasferimento)
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
                    if (trm != null && (trm.statoTrasferimento == EnumStatoTraferimento.Attivo))
                    {
                        ViewData["idTrasferimento"] = idTrasferimento;
                        return Json(new { VerificaViaggiCongedo = 1 });
                    }
                    else
                    {
                        return Json(new { VerificaViaggiCongedo = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }

        public ActionResult ElencoPreventiviDiViaggio(decimal idTitoliViaggio)
        {
            try
            {
                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    List<ElencoTitoliViaggioModel> ltvm = new List<ElencoTitoliViaggioModel>();

                    var atv = dttv.GetUltimaAttivazioneTitoliViaggio(idTitoliViaggio);

                    decimal idAttivazioneTitoliViaggio = atv.IDATTIVAZIONETITOLIVIAGGIO;

                    if (idAttivazioneTitoliViaggio > 0)
                    {
                        ltvm = dttv.ElencoTitoliViaggio(idTitoliViaggio);
                    }

                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var t = dtt.GetTrasferimentoByIdTitoloViaggio(idTitoliViaggio);
                        EnumStatoTraferimento statoTrasferimento = t.idStatoTrasferimento;
                        ViewData.Add("statoTrasferimento", statoTrasferimento);
                    }


                    bool richiestaEseguita = dttv.richiestaEseguita(idTitoliViaggio);

                    ViewData.Add("richiestaEseguita", richiestaEseguita);
                    ViewData.Add("idTitoliViaggio", idTitoliViaggio);
                    ViewData.Add("idAttivazioneTitoliViaggio", idAttivazioneTitoliViaggio);


                    return PartialView(ltvm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
        public ActionResult PreventiviDiViaggio(decimal idTrasferimento)
        {
            try
            {
                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    var idTitoliViaggio = dttv.GetIdTitoliViaggio(idTrasferimento);

                    ViewData.Add("idTitoliViaggio", idTitoliViaggio);
                    ViewData.Add("idTrasferimento", idTrasferimento);

                    return PartialView();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
    }
}
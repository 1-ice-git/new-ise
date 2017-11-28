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
                    TrasferimentoModel trm = dtt.GetTrasferimentoAttivoNotificato(matricola);
                    if (trm != null && trm.HasValue())
                    {
                        using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                        {
                            trm.RuoloUfficio = dtrd.GetRuoloDipendenteByIdTrasferimento(trm.idTrasferimento, DateTime.Now).RuoloUfficio;
                            trm.idRuoloUfficio = trm.RuoloUfficio.idRuoloUfficio;
                        }
                        using (dtDocumenti dtd = new dtDocumenti())
                        {
                            DocumentiModel dm = dtd.GetDocumentoByIdTrasferimento(trm.idTrasferimento);
                            trm.Documento = dm;
                            trm.idDocumento = dm.idDocumenti;
                        }

                        if (trm.idTipoTrasferimento > 0 &&
                            trm.idUfficio > 0 &&
                            trm.idStatoTrasferimento == (decimal)EnumStatoTraferimento.Attivo  &&
                            trm.idDipendente > 0 &&
                            trm.idTipoCoan > 0 &&
                            trm.dataPartenza > DateTime.MinValue &&
                            trm.idRuoloUfficio > 0
                            //&&
                            //trm.protocolloLettera != string.Empty &&
                            //trm.dataLettera > DateTime.MinValue &&
                            //trm.idDocumento > 0
                            )
                        {
                            return Json(new { VerificaSospensione = 1 });
                        }
                        else
                        {
                            return Json(new { VerificaSospensione = 0 });
                        }
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

        public ActionResult GetListaSospensioni()
        {
            List<SospensioneModel> tmp = new List<SospensioneModel>();
            //try
            //{
            //    using (dtCalendarioEventi dtcal = new dtCalendarioEventi())
            //    {
            //        tmp = dtcal.GetListaElementiHome().ToList();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            //}
            return PartialView(tmp);
        }

    }
}
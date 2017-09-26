using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.ViewModel;

namespace NewISE.Controllers
{
    public class TitoliViaggioController : Controller
    {
        // GET: TitoliViaggio
        public ActionResult TitoliViaggio(decimal idTrasferimento)
        {
            ViewData.Add("idTrasferimento", idTrasferimento);
            return PartialView();
        }

        public ActionResult ElencoFamiliariTitoliViaggio(decimal idTrasferimento)
        {
            List<ElencoFamiliariModel> lefm = new List<ElencoFamiliariModel>();

            try
            {
                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    lefm = dttv.GetDipendentiTitoliViaggio(idTrasferimento).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(lefm);
        }


        public ActionResult ChkEscludiTitoloViaggio(decimal idFamiliare, EnumParentela parentela, bool esisteDoc, bool escludiTitoloViaggio)
        {
            GestioneChkEscludiTitoliViaggioModel gcetv;
            TitoloViaggioModel tvm = new TitoloViaggioModel();
            bool dchk = false;

            using (dtTitoliViaggi dttv = new dtTitoliViaggi())
            {

                switch (parentela)
                {
                    case EnumParentela.Coniuge:
                        tvm = dttv.GetTitoloViaggioByIdConiuge(idFamiliare);
                        break;
                    case EnumParentela.Figlio:
                        tvm = dttv.GetTitoloViaggioByIdFiglio(idFamiliare);
                        break;
                    case EnumParentela.Richiedente:
                        tvm = dttv.GetTitoloViaggioByID(idFamiliare);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("parentela");
                }

                if (tvm != null && tvm.idTitoloViaggio > 0)
                {
                    if (tvm.notificaRichiesta == true || tvm.praticaConclusa == true)
                    {
                        dchk = true;
                    }
                }

                gcetv = new GestioneChkEscludiTitoliViaggioModel()
                {
                    idFamiliare = idFamiliare,
                    parentela = parentela,
                    esisteDoc = esisteDoc,
                    escludiTitoloViaggio = escludiTitoloViaggio,
                    disabilitaChk = dchk,
                };
            }


            return PartialView(gcetv);

        }

        [HttpPost]
        public JsonResult ConfermaEscludiTitoloViaggio(decimal id, EnumParentela parentela)
        {
            string errore = string.Empty;
            bool chk = false;

            try
            {
                switch (parentela)
                {
                    case EnumParentela.Coniuge:
                        using (dtConiuge dtc = new dtConiuge())
                        {
                            dtc.SetEscludiTitoloViaggio(id, ref chk);
                        }
                        break;
                    case EnumParentela.Figlio:
                        using (dtFigli dtf = new dtFigli())
                        {
                            dtf.SetEscludiTitoloViaggio(id, ref chk);
                        }
                        break;
                    case EnumParentela.Richiedente:

                        using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                        {
                            dttv.SetEscludiTitoloViaggio(id, ref chk);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("parentela");
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
                        chk = chk,
                        err = errore
                    });

        }

        [HttpPost]
        public ActionResult ColonnaElencoDoc(decimal idFamiliare, EnumParentela parentela)
        {
            ElencoFamiliariModel efm = new ElencoFamiliariModel();

            using (dtTitoliViaggi dttv = new dtTitoliViaggi())
            {
                efm = dttv.GetDatiForColElencoDoc(idFamiliare, parentela);
            }

            return PartialView(efm);
        }

        public ActionResult GestPulsantiNotificaAndPraticaConclusa(decimal idTrasferimento)
        {
            GestPulsantiAttConclRvModel gptv = new GestPulsantiAttConclRvModel();

            try
            {
                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    gptv = dttv.GestionePulsantiTitoliViaggi(idTrasferimento);
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(gptv);
        }


        [HttpPost]
        public JsonResult NotificaRichiesta(decimal idTrasferimento)
        {
            string errore = "";
            string msg = string.Empty;

            try
            {
                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    dttv.SetNotificaRichiesta(idTrasferimento);
                    msg = "Notifica effettuata con successo";
                }
            }
            catch (Exception ex)
            {
                errore = ex.Message;
            }

            return Json(new { err = errore, msg = msg });
        }


        public JsonResult ConcludiPratica(decimal idTrasferimento)
        {
            string errore = "";
            string msg = string.Empty;

            try
            {
                using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                {
                    dttv.SetPraticaConclusa(idTrasferimento);
                    msg = "Pratica conclusa con successo";
                }
            }
            catch (Exception ex)
            {

                errore = ex.Message;
            }

            return Json(new { err = errore, msg = msg });
        }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewISE.EF;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;
using NewISE.Interfacce;
using System.Web.Helpers;

namespace NewISE.Controllers
{
    public class PratichePassaportoController : Controller
    {

        public ActionResult Passaporti(decimal idTrasferimento)
        {

            ViewData.Add("idTrasferimento", idTrasferimento);

            return PartialView();

        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ElencoFamiliariPassaporti(decimal idTrasferimento)
        {
            List<ElencoFamiliariPassaportoModel> lefm = new List<ElencoFamiliariPassaportoModel>();

            try
            {
                using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                {
                    lefm = dtpp.GetFamiliariRichiestaPassaportoPartenza(idTrasferimento).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


            return PartialView(lefm);
        }

        [HttpPost]
        public ActionResult ColonnaElencoDoc(decimal idAttivazionePassaporto, decimal idFamiliarePassaporto, EnumParentela parentela)
        {
            ElencoFamiliariPassaportoModel efm = new ElencoFamiliariPassaportoModel();
            try
            { 
                using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                {
                    efm = dtpp.GetDatiForColElencoDoc(idAttivazionePassaporto, idFamiliarePassaporto, parentela);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(efm);
        }

        public JsonResult ConfermaIncludiEscludiPassaporto(decimal id, EnumParentela parentela)
        {
            string errore = string.Empty;
            bool chk = false;
            decimal idAttivazioniPassaporto = 0;
            try
            {
                switch (parentela)
                {
                    case EnumParentela.Coniuge:
                        using (dtConiugePassaporto dtcp = new dtConiugePassaporto())
                        {
                            dtcp.SetIncludiEscludiPassaporto(id, ref chk, ref idAttivazioniPassaporto);
                        }
                        break;
                    case EnumParentela.Figlio:
                        using (dtFigliPassaporto dtfp = new dtFigliPassaporto())
                        {
                            dtfp.SetIncludiEscludiPassaporto(id, ref chk, ref idAttivazioniPassaporto);
                        }
                        break;
                    case EnumParentela.Richiedente:
                        using (dtPassaportoRichiedente dtpr = new dtPassaportoRichiedente())
                        {
                            dtpr.SetIncludiEscludiPassaporto(id, ref chk, ref idAttivazioniPassaporto);
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
                        idAttivazioniPassaporto = idAttivazioniPassaporto,
                        err = errore
                    });

        }

        public ActionResult GestPulsantiNotificaAndPraticaConclusa(decimal idTrasferimento)
        {
            GestPulsantiAttConclModel gppm = new GestPulsantiAttConclModel();

            try
            {
                using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                {
                    gppm = dtpp.GestionePulsantiAttivazionePassaporto(idTrasferimento);
                }


            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(gppm);
        }

        public JsonResult LeggiStatusPratichePassaporto(decimal idPassaporto)
        {
            string errore = string.Empty;
            GestPulsantiAttConclModel gppm = new GestPulsantiAttConclModel();
            bool notificaRichiesta = false;
            bool praticaConclusa = false;


            try
            {
                using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                {
                    //gppm = dtpp.GestionePulsantiPassaportoById(idPassaporto);
                    if (gppm != null)
                    {
                        notificaRichiesta = gppm.notificaRichiesta;
                        praticaConclusa = gppm.praticaConclusa;
                    }
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
                        notificaRichiesta = notificaRichiesta,
                        praticaConclusa = praticaConclusa
                    });
        }


        public ActionResult ChkIncludiPassaporto(decimal idAttivitaPassaporto, decimal idFamiliarePassaporto, EnumParentela parentela, bool esisteDoc, bool includiPassaporto)
        {
            GestioneChkincludiPassaportoModel gcip = new GestioneChkincludiPassaportoModel();

            try
            {
                using (dtAttivazionePassaporto dtap = new dtAttivazionePassaporto())
                {
                    gcip = dtap.GetGestioneInludiPassaporto(idAttivitaPassaporto, idFamiliarePassaporto, parentela, esisteDoc, includiPassaporto);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(gcip);
        }


        public JsonResult NotificaRichiesta(decimal idAttivazionePassaporto)
        {
            string errore = "";
            string msg = string.Empty;

            try
            {
                using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                {
                    dtpp.NotificaRichiestaPassaporto(idAttivazionePassaporto);
                    msg = "Notifica effettuata con successo";
                }
            }
            catch (Exception ex)
            {
                errore = ex.Message;
            }

            return Json(new { err = errore, msg = msg });
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult AnnullaRichiesta(FormCollection fc)
        {
            string errore = "";
            string msg = string.Empty;
            FormCollection collection = new FormCollection(Request.Unvalidated().Form);

            decimal idAttivazionePassaporto = Convert.ToDecimal(collection["idAttivazionePassaporto"]);
            string testoAnnulla = collection["msg"];

            try
            {
                using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                {
                    dtpp.AnnullaRichiestaPassaporto(idAttivazionePassaporto,testoAnnulla);
                    msg = "Annullamento effettuato con successo";
                }
            }
            catch (Exception ex)
            {
                errore = ex.Message;
            }

            return Json(new { err = errore, msg = msg });

        }

        public JsonResult ConfermaRichiesta(decimal idAttivazionePassaporto)
        {
            string errore = "";
            string msg = string.Empty;

            try
            {
                using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                {
                    dtpp.ConfermaRichiestaPassaporto(idAttivazionePassaporto);
                    msg = "Pratica passaporto conclusa con successo";
                }
            }
            catch (Exception ex)
            {
                errore = ex.Message;
            }

            return Json(new { err = errore, msg = msg });
        }

        public ActionResult MessaggioAnnullaPassaporto(decimal idAttivazionePassaporto)
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
                            var t = dtt.GetTrasferimentoByIdAttPassaporto(idAttivazionePassaporto);

                            if (t?.idTrasferimento > 0)
                            {
                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                var uff = dtu.GetUffici(t.idUfficio);

                                msg.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaPassaporto, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString());
                                ViewBag.idTrasferimento = t.idTrasferimento;
                                ViewBag.idAttivazionePassaporto = idAttivazionePassaporto;
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
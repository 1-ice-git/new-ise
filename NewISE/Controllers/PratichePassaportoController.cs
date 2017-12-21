using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewISE.EF;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.ViewModel;

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
        public ActionResult ColonnaElencoDoc(decimal idFamiliarePassaporto, EnumParentela parentela)
        {
            ElencoFamiliariPassaportoModel efm = new ElencoFamiliariPassaportoModel();

            using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
            {
                efm = dtpp.GetDatiForColElencoDoc(idFamiliarePassaporto, parentela);
            }

            return PartialView(efm);
        }

        public JsonResult ConfermaEscludiPassaporto(decimal id, EnumParentela parentela)
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
                            dtc.SetEscludiPassaporto(id, ref chk);
                        }
                        break;
                    case EnumParentela.Figlio:
                        using (dtFigli dtf = new dtFigli())
                        {
                            dtf.SetEscludiPassaporto(id, ref chk);
                        }
                        break;
                    case EnumParentela.Richiedente:
                        using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                        {
                            dtpp.SetEscludiPassaportoRichiedente(id, ref chk);
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

        public ActionResult GestPulsantiNotificaAndPraticaConclusa(decimal idTrasferimento)
        {
            GestPulsantiAttConclModel gppm = new GestPulsantiAttConclModel();

            try
            {
                using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                {
                    //gppm = dtpp.GestionePulsantiPassaportoByIdTrasf(idTrasferimento);
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

        public ActionResult ChkEscludiPassaporto(decimal idFamiliare, EnumParentela parentela, bool esisteDoc, bool escludiPassaporto)
        {
            GestioneChkEscludiPassaportoModel gcep = new GestioneChkEscludiPassaportoModel();


            try
            {
                using (dtAttivazionePassaporto dtap = new dtAttivazionePassaporto())
                {
                    gcep = dtap.GetGestioneEcludiPassaporto(idFamiliare, parentela, esisteDoc, escludiPassaporto);
                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


            return PartialView(gcep);

        }


        public JsonResult NotificaRichiesta(decimal idTrasferimento)
        {
            string errore = "";
            string msg = string.Empty;

            try
            {
                using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                {
                    //dtpp.SetNotificaRichiesta(idTrasferimento);
                    msg = "Notifica effettuata con successo";
                }
            }
            catch (Exception ex)
            {
                errore = ex.Message;
            }

            return Json(new { err = errore, msg = msg });
        }

        public JsonResult ConcludiPraticaPassaporto(decimal idTrasferimento)
        {
            string errore = "";
            string msg = string.Empty;

            try
            {
                using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                {
                    //dtpp.SetConcludiPassaporto(idTrasferimento);
                    msg = "Pratica conclusa con successo";
                }
            }
            catch (Exception ex)
            {

                errore = ex.Message;
            }

            return Json(new { err = errore, msg = msg });

        }

        //public JsonResult NominativoEscludiPassaporto(decimal id, EnumParentela parentela, bool boolChk)
        //{
        //    string errore = string.Empty;
        //    string msg = string.Empty;
        //    string nominativo = string.Empty;

        //    try
        //    {

        //        if (boolChk)
        //        {
        //            msg = "Procedere con l'esclusione per {0} dalla richiesta di passaporto/visto?";
        //        }
        //        else
        //        {
        //            msg = "Procedere con l'inclusione per {0} per la richiesta di passaporto/visto?";
        //        }


        //        switch (parentela)
        //        {
        //            case EnumParentela.Coniuge:
        //                using (dtConiuge dtc = new dtConiuge())
        //                {
        //                    var c = dtc.GetConiugebyID(id);
        //                    nominativo = c.nominativo;
        //                }
        //                break;
        //            case EnumParentela.Figlio:
        //                using (dtFigli dtf = new dtFigli())
        //                {
        //                    var f = dtf.GetFigliobyID(id);
        //                    nominativo = f.nominativo;
        //                }
        //                break;
        //            case EnumParentela.Richiedente:
        //                using (dtDipendenti dtd = new dtDipendenti())
        //                {
        //                    var d = dtd.GetDipendenteByIDTrasf(id);
        //                    nominativo = d.Nominativo;
        //                }
        //                break;
        //            default:
        //                throw new ArgumentOutOfRangeException("parentela");
        //        }

        //        msg = string.Format(msg, nominativo);


        //    }
        //    catch (Exception ex)
        //    {
        //        errore = ex.Message;
        //    }

        //    return
        //        Json(
        //            new
        //            {
        //                msg = msg,
        //                err = errore
        //            });



        //}
    }
}
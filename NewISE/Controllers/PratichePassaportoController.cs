﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
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


        public ActionResult ElencoFamiliariPassaporti(decimal idTrasferimento)
        {
            List<ElencoFamiliariModel> lefm = new List<ElencoFamiliariModel>();

            try
            {
                using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                {
                    lefm = dtpp.GetDipendentiRichiestaPassaporto(idTrasferimento).ToList();

                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(lefm);
        }

        public ActionResult ColonnaElencoDoc(decimal idFamiliare, EnumParentela parentela)
        {
            ElencoFamiliariModel efm = new ElencoFamiliariModel();

            using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
            {
                efm = dtpp.GetDatiForColElencoDoc(idFamiliare, parentela);
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
                            dtpp.SetEscludiPassaporto(id, ref chk);
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

        public JsonResult NominativoEscludiPassaporto(decimal id, EnumParentela parentela, bool boolChk)
        {
            string errore = string.Empty;
            string msg = string.Empty;
            string nominativo = string.Empty;

            try
            {

                if (boolChk)
                {
                    msg = "Procedere con l'esclusione per {0} dalla richiesta di passaporto/visto?";
                }
                else
                {
                    msg = "Procedere con l'inclusione per {0} per la richiesta di passaporto/visto?";
                }


                switch (parentela)
                {
                    case EnumParentela.Coniuge:
                        using (dtConiuge dtc = new dtConiuge())
                        {
                            var c = dtc.GetConiugebyID(id);
                            nominativo = c.nominativo;
                        }
                        break;
                    case EnumParentela.Figlio:
                        using (dtFigli dtf = new dtFigli())
                        {
                            var f = dtf.GetFigliobyID(id);
                            nominativo = f.nominativo;
                        }
                        break;
                    case EnumParentela.Richiedente:
                        using (dtDipendenti dtd = new dtDipendenti())
                        {
                            var d = dtd.GetDipendenteByIDTrasf(id);
                            nominativo = d.Nominativo;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("parentela");
                }

                msg = string.Format(msg, nominativo);


            }
            catch (Exception ex)
            {
                errore = ex.Message;
            }

            return
                Json(
                    new
                    {
                        msg = msg,
                        err = errore
                    });



        }
    }
}
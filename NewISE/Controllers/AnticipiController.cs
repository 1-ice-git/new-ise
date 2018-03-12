using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewISE.Models.DBModel;
using NewISE.Models;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;
using NewISE.Interfacce;

namespace NewISE.Controllers
{
    public class AnticipiController : Controller
    {
        // GET: Anticipi
        public ActionResult Anticipi(decimal idTrasferimento)
        {
            PrimaSistemazioneModel psm = new PrimaSistemazioneModel();

            try
            {
                using (dtPrimaSistemazione dtps = new dtPrimaSistemazione())
                {
                    psm = dtps.GetPrimaSistemazioneByIdTrasf(idTrasferimento);
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewData["idPrimaSistemazione"] = psm.idPrimaSistemazione;

            return PartialView(psm);
        }

        public JsonResult CalcolaAnticipo(decimal idAttivitaAnticipi, decimal percRichiesta)
        {
            string errore = "";
            decimal importoPercepito = 0;

            try
            {
                using (dtAnticipi dta = new dtAnticipi())
                {
                    importoPercepito = dta.CalcolaImportoPercepito(idAttivitaAnticipi, percRichiesta);
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
                        importoPercepito = importoPercepito,
                        err = errore
                    });


        }

        public ActionResult AttivitaAnticipi(decimal idPrimaSistemazione)
        {
            AnticipiViewModel avm = new AnticipiViewModel();

            try
            {
                using (dtAnticipi dta = new dtAnticipi())
                {
                    bool soloLettura = false;

                    AttivitaAnticipiModel aam = dta.GetUltimaAttivitaAnticipi(idPrimaSistemazione);

                    var idAttivitaAnticipi = aam.idAttivitaAnticipi;

                    if (aam.notificaRichiestaAnticipi)
                    {
                        soloLettura = true;
                    }
                    avm = dta.GetAnticipi(idAttivitaAnticipi);

                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var t = dtt.GetTrasferimentoByIdPrimaSistemazione(idPrimaSistemazione);
                        if (t.idStatoTrasferimento == EnumStatoTraferimento.Annullato || t.idStatoTrasferimento == EnumStatoTraferimento.Attivo)
                        {
                            soloLettura = true;
                        }
                    }

                    decimal NumAttivazioni = dta.GetNumAttivazioniAnticipi(idPrimaSistemazione);

                    ViewData.Add("NumAttivazioni", NumAttivazioni);
                    ViewData.Add("soloLettura", soloLettura);
                    ViewData.Add("idAttivitaAnticipi", idAttivitaAnticipi);
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(avm);
        }


        public ActionResult GestionePulsantiAnticipi(decimal idPrimaSistemazione)
        {
            AttivitaAnticipiModel aam = new AttivitaAnticipiModel();




            try
            {
                bool amministratore = Utility.Amministratore();

                string disabledNotificaRichiesta = "disabled";
                string hiddenNotificaRichiesta = "";
                string disabledAttivaRichiesta = "disabled";
                string hiddenAttivaRichiesta = "hidden";
                string disabledAnnullaRichiesta = "disabled";
                string hiddenAnnullaRichiesta = "hidden";

                using (dtAnticipi dta = new dtAnticipi())
                {
                    aam = dta.GetUltimaAttivitaAnticipi(idPrimaSistemazione);
                    var idAttivitaAnticipi = aam.idAttivitaAnticipi;
                }

                EnumStatoTraferimento statoTrasferimento = 0;
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var t = dtt.GetTrasferimentoByIdPrimaSistemazione(idPrimaSistemazione);
                    statoTrasferimento = t.idStatoTrasferimento;
                }

                bool notificaRichiesta = aam.notificaRichiestaAnticipi;
                bool attivaRichiesta = aam.attivaRichiestaAnticipi;


                //se amministratore vedo i pulsanti altrimenti solo notifica
                if (amministratore)
                {
                    hiddenAttivaRichiesta = "";
                    hiddenAnnullaRichiesta = "";

                    if (notificaRichiesta && attivaRichiesta == false && statoTrasferimento!=EnumStatoTraferimento.Attivo && statoTrasferimento!=EnumStatoTraferimento.Annullato )
                    {
                        disabledAttivaRichiesta = "";
                        disabledAnnullaRichiesta = "";
                    }
                }

                ViewData.Add("disabledAnnullaRichiesta", disabledAnnullaRichiesta);
                ViewData.Add("disabledAttivaRichiesta", disabledAttivaRichiesta);
                ViewData.Add("disabledNotificaRichiesta", disabledNotificaRichiesta);
                ViewData.Add("hiddenAnnullaRichiesta", hiddenAnnullaRichiesta);
                ViewData.Add("hiddenAttivaRichiesta", hiddenAttivaRichiesta);
                ViewData.Add("hiddenNotificaRichiesta", hiddenNotificaRichiesta);
                ViewData.Add("amministratore", amministratore);

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(aam);
        }

        public JsonResult ConfermaNotificaRichiestaAnticipi(decimal idAttivitaAnticipi, decimal percentualeRichiesta)
        {
            string errore = "";

            try
            {
                using (dtAnticipi dta = new dtAnticipi())
                {

                    dta.NotificaRichiestaAnticipi(idAttivitaAnticipi, percentualeRichiesta);
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
                        err = errore
                    });
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ConfermaAnnullaRichiestaAnticipi(decimal idAttivitaAnticipi, string msg)
        {
            string errore = "";
            string testoAnnulla = msg;
            try
            {
                using (dtAnticipi dta = new dtAnticipi())
                {
                    dta.AnnullaRichiestaAnticipi(idAttivitaAnticipi, testoAnnulla);
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
                        err = errore
                    });
        }

        public JsonResult ConfermaAttivaRichiestaAnticipi(decimal idAttivitaAnticipi)
        {
            string errore = "";

            try
            {
                using (dtAnticipi dta = new dtAnticipi())
                {


                    dta.AttivaRichiestaAnticipi(idAttivitaAnticipi);
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
                        err = errore
                    });
        }

        public ActionResult MessaggioAnnullaAnticipi(decimal idPrimaSistemazione)
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
                            var t = dtt.GetTrasferimentoByIdPrimaSistemazione(idPrimaSistemazione);

                            if (t?.idTrasferimento > 0)
                            {
                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                var uff = dtu.GetUffici(t.idUfficio);

                                msg.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaAnticipi, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza);
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
using System.Web.Routing;
using NewISE.EF;
using NewISE.Models.Tools;
using Newtonsoft.Json;
using System.IO;
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
    public class MaggiorazioneAbitazioneController : Controller
    {
        [HttpPost]
        public ActionResult MaggiorazioneAbitazione(decimal idTrasferimento)
        {
            ViewData.Add("idTrasferimento", idTrasferimento);

            return PartialView();
        }

        public ActionResult ElencoDocumentiFormularioMAB()
        {
            return PartialView();
        }


        public ActionResult AttivitaMAB(decimal idTrasferimento)
        {
            List<MaggiorazioneAbitazioneViewModel> mavml = new List<MaggiorazioneAbitazioneViewModel>();
            MaggiorazioneAbitazioneViewModel mavm = new MaggiorazioneAbitazioneViewModel();
            MaggiorazioneAbitazioneModel mam = new MaggiorazioneAbitazioneModel();

            try
            {
                using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                {
                    bool soloLettura = false;
                    bool siDati = false;

                    AttivazioneMABModel amm = dtma.GetAttivitaMAB(idTrasferimento);

                    if (amm != null && amm.idAttivazioneMAB > 0)
                    {
                        if (amm.notificaRichiesta)
                        {
                            soloLettura = true;
                        }

                        mam = dtma.GetMaggiorazioneAbitazione(amm);

                        if (mam != null && mam.idMAB > 0)
                        {
                            CanoneMABModel cmm = dtma.GetCanoneMAB(mam);

                            mavm.CanoneMAB = cmm;
                            mavm.dataInizioMAB = mam.dataInizioMAB;
                            mavm.dataFineMAB = mam.dataFineMAB;
                            mavm.AnticipoAnnuale = mam.AnticipoAnnuale;
                            siDati = true;
                        }
                    }
                    mavml.Add(mavm);

                    decimal NumAttivazioni = dtma.GetNumAttivazioniMAB(idTrasferimento);

                    ViewData.Add("NumAttivazioni", NumAttivazioni);
                    ViewData.Add("soloLettura", soloLettura);
                    ViewData.Add("siDati", siDati);
                    ViewData.Add("idAttivazioneMAB", amm.idAttivazioneMAB);
                    ViewData.Add("idMAB", mam.idMAB);
                    ViewData.Add("idTrasferimento", idTrasferimento);
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(mavml);
        }

        public ActionResult FormulariMAB(decimal idTrasferimento)
        {
            bool siDocModulo1 = false;
            bool siDocModulo2 = false;
            bool siDocModulo3 = false;
            bool siDocModulo4 = false;
            bool siDocModulo5 = false;
            bool siDocCopiaContratto = false;
            bool siDocCopiaRicevuta = false;

            MaggiorazioneAbitazioneModel mam = new MaggiorazioneAbitazioneModel();

            try
            {
                using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                {
                    bool soloLettura = false;

                    AttivazioneMABModel amm = dtma.GetAttivitaMAB(idTrasferimento);

                    if (amm != null && amm.idAttivazioneMAB > 0)
                    {
                        dtma.VerificaDocumentiPartenza(amm, out siDocCopiaContratto, out siDocCopiaRicevuta,
                                                        out siDocModulo1, out siDocModulo2, out siDocModulo3,
                                                        out siDocModulo4, out siDocModulo5);

                        if (amm.notificaRichiesta)
                        {
                            soloLettura = true;
                        }

                    }

                    ViewData.Add("idAttivazione", amm.idAttivazioneMAB);
                    ViewData.Add("siDocCopiaContratto", siDocCopiaContratto);
                    ViewData.Add("siDocCopiaRicevuta", siDocCopiaRicevuta);
                    ViewData.Add("siDocModulo1", siDocModulo1);
                    ViewData.Add("siDocModulo2", siDocModulo2);
                    ViewData.Add("siDocModulo3", siDocModulo3);
                    ViewData.Add("siDocModulo4", siDocModulo4);
                    ViewData.Add("siDocModulo5", siDocModulo5);
                    ViewData.Add("soloLettura", soloLettura);
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }


        public ActionResult GestionePulsantiMAB(decimal idTrasferimento)
        {
            AttivazioneMABModel amm = new AttivazioneMABModel();

            try
            {
                bool amministratore = Utility.Amministratore();

                string disabledNotificaRichiesta = "disabled";
                string hiddenNotificaRichiesta = "";
                string disabledAttivaRichiesta = "disabled";
                string hiddenAttivaRichiesta = "hidden";
                string disabledAnnullaRichiesta = "disabled";
                string hiddenAnnullaRichiesta = "hidden";

                using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                {
                    amm = dtma.GetUltimaAttivazioneMAB(idTrasferimento);
                }
                var idAttivazioneMAB = amm.idAttivazioneMAB;



                bool notificaRichiesta = amm.notificaRichiesta;
                bool attivaRichiesta = amm.Attivazione;


                //se amministratore vedo i pulsanti altrimenti solo notifica
                if (amministratore)
                {
                    hiddenAttivaRichiesta = "";
                    hiddenAnnullaRichiesta = "";

                    if (notificaRichiesta && attivaRichiesta == false)
                    {
                        disabledAttivaRichiesta = "";
                        disabledAnnullaRichiesta = "";
                    }
                }

                ViewData.Add("idAttivazioneMAB", idAttivazioneMAB);
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

            return PartialView(amm);
        }


    }
}
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
using NewISE.Interfacce;
using NewISE.Models.Enumeratori;

namespace NewISE.Controllers
{
    public class VariazioneMaggiorazioneAbitazioneController : Controller
    {

        [HttpPost]
        public ActionResult VariazioneMaggiorazioneAbitazione(decimal idTrasferimento)
        {
            ViewData.Add("idTrasferimento", idTrasferimento);

            return PartialView();
        }

        public ActionResult ElencoDocumentiFormularioMAB_var()
        {
            return PartialView();
        }

        public ActionResult AttivitaMAB_var(decimal idTrasferimento)
        {
            List<MABViewModel> lmabvm = new List<MABViewModel>();
            MABViewModel mabvm = new MABViewModel();
            List<MABModel> lmabm = new List<MABModel>();


            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                    {
                        //using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                        //{
                        bool soloLettura = false;
                        bool siDati = false;
                        bool siModifiche = false;
                        EnumStatoTraferimento statoTrasferimento = 0;
                        TrasferimentoModel tm;
                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            tm = dtt.GetTrasferimentoById(idTrasferimento);
                        }

                        ATTIVAZIONEMAB ultima_att = dtvma.GetUltimaAttivazioneMAB(idTrasferimento, db);
                        //MAB mab_modificata =  dtvma.GetMAB_ByID_var(ultima_att.IDMAB, db);
                        //PERIODOMAB periodo_modificato = dtvma.GetPeriodoMAB(mab_modificata.IDMAB,db);

                        var lmabNonAttive = dtvma.GetMABNonAttiveModel(idTrasferimento, db);
                        if (lmabNonAttive?.Any() ?? false)
                        {
                            lmabm = lmabNonAttive;
                            siModifiche = true;
                        }
                        else
                        {
                            lmabm = dtvma.GetElencoMABModel(idTrasferimento);
                            siModifiche = false;
                        }

                        if (lmabm?.Any() ?? false)
                        {
                            foreach (MABModel mabm in lmabm)
                            {
                                bool verificaVariazioni = dtvma.VerificaVariazioniMAB(mabm.idMAB, db, false);

                                var pmm = dtvma.GetPeriodoMABModel(mabm.idMAB, db);
                                //var att = db.ATTIVAZIONEMAB.Find(pmm.idAttivazioneMAB);

                                var dataFineMAB = pmm.dataFineMAB > tm.dataRientro ? tm.dataRientro.Value : pmm.dataFineMAB;

                                bool modificabile = true;
                                if (dataFineMAB < Utility.DataFineStop())
                                {
                                    if (ultima_att.NOTIFICARICHIESTA)
                                    {
                                        modificabile = false;
                                    }

                                }
                                if (dataFineMAB == Utility.DataFineStop())
                                {
                                    //soloLettura = true;
                                    if (ultima_att.NOTIFICARICHIESTA && ultima_att.ATTIVAZIONE == false)
                                    {
                                        modificabile = false;
                                    }
                                }
                                bool annullabile = false;

                                if (verificaVariazioni && ultima_att.NOTIFICARICHIESTA == false)
                                {
                                    annullabile = true;
                                }

                                if (dataFineMAB == Utility.DataFineStop())
                                {
                                    soloLettura = true;
                                }

                                bool siFormulari = false;
                                using (dtDocumenti dtd = new dtDocumenti())
                                {
                                    if (dtd.GetFormulariMaggiorazioniAbitazioneVariazione(mabm.idMAB, db).Count() > 0)
                                    {
                                        siFormulari = true;
                                    }
                                }

                                mabvm = new MABViewModel()
                                {
                                    modificabile = modificabile,
                                    idAttivazioneMAB = mabm.idAttivazioneMAB,
                                    idMAB = mabm.idMAB,
                                    dataInizioMAB = pmm.dataInizioMAB,
                                    dataFineMAB = dataFineMAB,
                                    anticipoAnnuale = dtvma.AnticipoAnnualeMAB(mabm.idMAB, db),
                                    annullabile = annullabile,
                                    esistonoFormulari = siFormulari
                                };

                                lmabvm.Add(mabvm);

                                siDati = true;
                            }
                        }


                        if (ultima_att.NOTIFICARICHIESTA && ultima_att.ATTIVAZIONE == false)
                        {
                            soloLettura = true;
                        }

                        if (siModifiche)
                        {
                            soloLettura = true;
                        }


                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            var t = dtt.GetTrasferimentoById(idTrasferimento);
                            statoTrasferimento = t.idStatoTrasferimento;
                            if (statoTrasferimento == EnumStatoTraferimento.Annullato)
                            {
                                soloLettura = true;
                            }
                        }

                        ViewData.Add("soloLettura", soloLettura);
                        ViewData.Add("siDati", siDati);
                        ViewData.Add("idAttivazioneMAB", mabvm.idAttivazioneMAB);
                        ViewData.Add("idTrasferimento", idTrasferimento);
                        //}
                    }
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(lmabvm);
        }

        [HttpPost]
        public ActionResult NuovoFormularioMAB(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;

            return PartialView();
        }

        [HttpPost]
        public ActionResult NuovoFormularioMAB_TipoDoc(decimal idMab, decimal idTipo = 0)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                using (dtDocumenti dtd = new dtDocumenti())
                {
                    VariazioneDocumentiModel vdm = new VariazioneDocumentiModel();

                    List<SelectListItem> ldescrizioneTipoDoc = new List<SelectListItem>();
                    var r = new List<SelectListItem>();

                    var ltd = dtd.GetElencoTipoDocumentiMAB(db);

                    if (ltd != null && ltd.Count > 0)
                    {
                        r = (from td in ltd
                             select new SelectListItem()
                             {
                                 Text = td.DESCRIZIONE,
                                 Value = td.IDTIPODOCUMENTO.ToString()
                             }).ToList();

                        r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                    }

                    ldescrizioneTipoDoc = r;
                    ViewBag.ldescrizioneTipoDoc = ldescrizioneTipoDoc;
                    if (idTipo > 0)
                    {
                        vdm.idTipoDocumento = idTipo;
                    }

                    ViewData["idMab"] = idMab;

                    return PartialView(vdm);
                }
            }
        }

        [HttpPost]
        public ActionResult NuovoFormularioMAB_Doc(decimal idMab, decimal idTipo)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                using (dtDocumenti dtd = new dtDocumenti())
                {
                    var descTipo = dtd.GetDescrizioneTipoDocumento((EnumTipoDoc)idTipo, db);
                    ViewData["idMab"] = idMab;
                    ViewData["idTipo"] = idTipo;
                    ViewData["descTipo"] = descTipo;

                    return PartialView();
                }
            }
        }

        public ActionResult ElencoCanoneMAB(decimal idTrasferimento, decimal idMab)
        {
            List<CanoneMABViewModel> lcmabvm = new List<CanoneMABViewModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                    {
                        using (dtValute dtv = new dtValute())
                        {
                            using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                            {
                                using (dtTrasferimento dtt = new dtTrasferimento())
                                {
                                    var r = new List<SelectListItem>();

                                    List<SelectListItem> lValute = new List<SelectListItem>();

                                    CanoneMABViewModel cmabvm = new CanoneMABViewModel();

                                    EnumStatoTraferimento statoTrasferimento = 0;

                                    var mab = dtvma.GetMAB_ByID_var(idMab, db);

                                    CANONEMAB canonePartenza = new CANONEMAB();
                                    canonePartenza = dtma.GetCanoneMABPartenza(mab, db);

                                    AttivazioneMABModel amabm = dtvma.GetUltimaAttivazioneMABmodel(idTrasferimento);

                                    bool soloLettura = false;

                                    if (amabm != null && amabm.idAttivazioneMAB > 0)
                                    {
                                        if (amabm.notificaRichiesta && amabm.Attivazione == false)
                                        {
                                            soloLettura = true;
                                        }
                                    }

                                    List<CanoneMABModel> lcmabm = dtvma.GetCanoneMABModel(idMab, db);

                                    if (lcmabm?.Any() ?? false)
                                    {
                                        foreach (var cmabm in lcmabm)
                                        {
                                            var cPartenza = false;

                                            if (cmabm.idCanone == canonePartenza.IDCANONE)
                                            {
                                                cPartenza = true;
                                            }

                                            cmabvm = new CanoneMABViewModel()
                                            {
                                                canonePartenza = cPartenza,
                                                idCanone = cmabm.idCanone,
                                                IDMAB = cmabm.IDMAB,
                                                ImportoCanone = cmabm.ImportoCanone,
                                                DataInizioValidita = cmabm.DataInizioValidita,
                                                DataFineValidita = cmabm.DataFineValidita,
                                                idValuta = cmabm.idValuta,
                                                descrizioneValuta = dtv.GetValutaModel(cmabm.idValuta, db).descrizioneValuta
                                            };
                                            lcmabvm.Add(cmabvm);
                                        }
                                    }

                                    var t = dtt.GetTrasferimentoById(idTrasferimento);
                                    statoTrasferimento = t.idStatoTrasferimento;
                                    if (statoTrasferimento == EnumStatoTraferimento.Annullato)
                                    {
                                        soloLettura = true;
                                    }

                                    #region controllo modifica canoni
                                    var lc_in_lav = db.MAB.Find(idMab).CANONEMAB.Where(a =>
                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione &&
                                            a.NASCONDI == false && ((a.DATAFINEVALIDITA >= t.dataRientro && a.DATAINIZIOVALIDITA <= t.dataRientro) || a.DATAFINEVALIDITA < t.dataRientro))
                                                .OrderByDescending(a => a.IDCANONE).ToList();
                                    bool canoniModificati = false;
                                    if (lc_in_lav.Count() > 0)
                                    {
                                        canoniModificati = true;
                                    }
                                    #endregion

                                    ViewData.Add("idTrasferimento", idTrasferimento);
                                    ViewData.Add("soloLettura", soloLettura);
                                    ViewData.Add("canoniModificati", canoniModificati);
                                    ViewData.Add("idMab", idMab);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(lcmabvm);
        }

        public ActionResult ElencoPagatoCondivisoMAB(decimal idTrasferimento, decimal idMab)
        {
            List<PagatoCondivisoMABModel> lpcmabm = new List<PagatoCondivisoMABModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                    {
                        using (dtValute dtv = new dtValute())
                        {
                            using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                            {
                                using (dtTrasferimento dtt = new dtTrasferimento())
                                {
                                    var r = new List<SelectListItem>();

                                    //PagatoCondivisoMABModel pcmabm = new PagatoCondivisoMABModel();

                                    bool soloLettura = false;

                                    EnumStatoTraferimento statoTrasferimento = 0;

                                    PAGATOCONDIVISOMAB pcmabPartenza = new PAGATOCONDIVISOMAB();
                                    pcmabPartenza = dtma.GetPagatoCondivisoMABPartenza(idMab, db);

                                    AttivazioneMABModel amabm = dtvma.GetUltimaAttivazioneMABmodel(idTrasferimento);

                                    if (amabm != null && amabm.idAttivazioneMAB > 0)
                                    {
                                        if (amabm.notificaRichiesta && amabm.Attivazione == false)
                                        {
                                            soloLettura = true;
                                        }
                                    }

                                    lpcmabm = dtvma.GetPagatoCondivisoMABModel(idMab, db);

                                    var t = dtt.GetTrasferimentoById(idTrasferimento);
                                    statoTrasferimento = t.idStatoTrasferimento;
                                    if (statoTrasferimento == EnumStatoTraferimento.Annullato)
                                    {
                                        soloLettura = true;
                                    }

                                    #region controllo modifica pagato condiviso
                                    var lc_in_lav = db.MAB.Find(idMab).PAGATOCONDIVISOMAB.Where(a =>
                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione &&
                                            ((a.DATAINIZIOVALIDITA <= t.dataRientro && a.DATAFINEVALIDITA >= t.dataRientro) || a.DATAFINEVALIDITA < t.dataRientro) &&
                                            a.NASCONDI == false)
                                                .OrderByDescending(a => a.IDPAGATOCONDIVISO).ToList();
                                    bool pagatocondivisoModificato = false;
                                    if (lc_in_lav.Count() > 0)
                                    {
                                        pagatocondivisoModificato = true;
                                    }
                                    #endregion
                                    ViewData.Add("idTrasferimento", idTrasferimento);
                                    ViewData.Add("soloLettura", soloLettura);
                                    ViewData.Add("pagatocondivisoModificato", pagatocondivisoModificato);
                                    ViewData.Add("idMab", idMab);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(lpcmabm);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NuovoImportoCanone(decimal idMab, decimal idTrasferimento)
        {
            using (dtVariazioniMaggiorazioneAbitazione dtma = new dtVariazioniMaggiorazioneAbitazione())
            {
                using (dtValute dtv = new dtValute())
                {
                    using (ModelDBISE db = new ModelDBISE())
                    {
                        List<SelectListItem> lValute = new List<SelectListItem>();

                        var r = new List<SelectListItem>();

                        CanoneMABViewModel cmabvm = new CanoneMABViewModel();
                        PeriodoMABModel pmabm = new PeriodoMABModel();

                        var vm = dtma.GetUltimaValutaInseritaModel(idMab, db);

                        cmabvm.descrizioneValuta = vm.descrizioneValuta;
                        cmabvm.idValuta = vm.idValuta;

                        var lv = dtv.GetElencoValute(db);

                        if (lv != null && lv.Count > 0)
                        {
                            r = (from v in lv
                                 select new SelectListItem()
                                 {
                                     Text = v.DESCRIZIONEVALUTA,
                                     Value = v.IDVALUTA.ToString()
                                 }).ToList();

                            r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                        }

                        lValute = r;

                        pmabm = dtma.GetPeriodoMABModel(idMab, db);
                        cmabvm.ut_dataInizioValidita = null;

                        var ultimoPeriodoMABInserito = dtma.GetUltimoCanoneMABInserito(idMab, db);
                        bool dataInizioValiditaModificabile = true;
                        if (ultimoPeriodoMABInserito.IMPORTOCANONE <= 0)
                        {
                            dataInizioValiditaModificabile = false;
                            cmabvm.ut_dataInizioValidita = pmabm.dataInizioMAB;
                        }
                        cmabvm.DataInizioValidita = pmabm.dataInizioMAB;

                        cmabvm.canoneAttuale = ultimoPeriodoMABInserito.IMPORTOCANONE;

                        ViewData.Add("idMab", idMab);
                        ViewData.Add("idTrasferimento", idTrasferimento);
                        ViewData.Add("dataInizioValiditaModificabile", dataInizioValiditaModificabile);
                        ViewBag.lValute = lValute;

                        return PartialView(cmabvm);
                    }
                }
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NuovoPagatoCondiviso(decimal idMab, decimal idTrasferimento)
        {
            using (dtVariazioniMaggiorazioneAbitazione dtma = new dtVariazioniMaggiorazioneAbitazione())
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    PagatoCondivisoMABViewModel pcmabvm = new PagatoCondivisoMABViewModel();
                    PeriodoMABModel pmabm = new PeriodoMABModel();

                    var ultimoPeriodoMABInserito = dtma.GetUltimoPagatoCondivisoMABInserito(idMab, db);
                    var ultimoCondiviso = ultimoPeriodoMABInserito.CONDIVISO;
                    var ultimoPagato = ultimoPeriodoMABInserito.PAGATO;
                    string testo = "";
                    if (ultimoCondiviso)
                    {
                        testo = "Condiviso: SI";
                        if (ultimoPagato)
                        {
                            testo = testo + "; Pagato: SI";
                        }
                        else
                        {
                            testo = testo + "; Pagato: NO";
                        }
                    }
                    else
                    {
                        testo = "Condiviso: NO";
                    }
                    pcmabvm.testoUltimaOpzioneSelezionata = testo;

                    pmabm = dtma.GetPeriodoMABModel(idMab, db);
                    pcmabvm.ut_dataInizioValidita = null;

                    pcmabvm.DataInizioValidita = pmabm.dataInizioMAB;

                    ViewData.Add("idMab", idMab);
                    ViewData.Add("idTrasferimento", idTrasferimento);

                    return PartialView(pcmabvm);
                }
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciImportoCanone(CanoneMABViewModel cmabvm, bool dataInizioValiditaModificabile, decimal idMab, decimal idTrasferimento)
        {
            try
            {

                using (dtVariazioniMaggiorazioneAbitazione dtma = new dtVariazioniMaggiorazioneAbitazione())
                {
                    using (dtValute dtv = new dtValute())
                    {
                        using (ModelDBISE db = new ModelDBISE())
                        {

                            if (ModelState.IsValid)
                            {
                                db.Database.BeginTransaction();
                                try
                                {
                                    #region verifica importo canone e data inizio
                                    try
                                    {
                                        dtma.VerificaImportoCanoneMAB(cmabvm.ImportoCanone, db);
                                        if (dataInizioValiditaModificabile)
                                        {
                                            dtma.VerificaDataInizioValiditaCanoneMAB_Utente(cmabvm.IDMAB, cmabvm.ut_dataInizioValidita, db);
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                        List<SelectListItem> lValute = new List<SelectListItem>();

                                        var r = new List<SelectListItem>();

                                        //PeriodoMABModel pmabm = new PeriodoMABModel();

                                        var vm = dtma.GetUltimaValutaInseritaModel(idMab, db);

                                        cmabvm.descrizioneValuta = vm.descrizioneValuta;
                                        cmabvm.idValuta = vm.idValuta;

                                        var lv = dtv.GetElencoValute(db);

                                        if (lv != null && lv.Count > 0)
                                        {
                                            r = (from v in lv
                                                 select new SelectListItem()
                                                 {
                                                     Text = v.DESCRIZIONEVALUTA,
                                                     Value = v.IDVALUTA.ToString()
                                                 }).ToList();

                                            r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                                        }

                                        lValute = r;

                                        //pmabm = dtma.GetPeriodoMABModel(idMab);

                                        ViewBag.lValute = lValute;


                                        ViewData.Add("dataInizioValiditaModificabile", dataInizioValiditaModificabile);
                                        cmabvm.ut_dataInizioValidita = cmabvm.DataInizioValidita;

                                        ModelState.AddModelError("", ex.Message);
                                        ViewData.Add("idMab", idMab);
                                        ViewData.Add("idTrasferimento", idTrasferimento);

                                        return PartialView("NuovoImportoCanone", cmabvm);
                                    }
                                    #endregion

                                    cmabvm.DataAggiornamento = DateTime.Now;
                                    cmabvm.idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione;

                                    #region attivazione
                                    ATTIVAZIONEMAB attmab_aperta = new ATTIVAZIONEMAB();

                                    //var attmab_rif = cmabvm.IDAttivazioneMAB;
                                    var attmab = dtma.GetAttivazioneAperta(idTrasferimento, db);

                                    // se non esiste attivazione aperta la creo altrimenti la uso
                                    if (attmab.IDATTIVAZIONEMAB > 0)
                                    {
                                        attmab_aperta = attmab;
                                    }
                                    else
                                    {
                                        ATTIVAZIONEMAB new_amab = dtma.CreaAttivazione(idMab, db);
                                        attmab_aperta = new_amab;
                                    }
                                    #endregion

                                    #region date
                                    var dataFineValiditaMAB = dtma.GetPeriodoMABModel(cmabvm.IDMAB, db).dataFineMAB;


                                    //controlla data inserita superiore a dataRientro
                                    DateTime dataInizio;
                                    if (dataInizioValiditaModificabile)
                                    {
                                        dataInizio = cmabvm.ut_dataInizioValidita.Value;
                                    }
                                    else
                                    {
                                        dataInizio = cmabvm.DataInizioValidita;
                                    }

                                    if (dataInizio > dataFineValiditaMAB)
                                    {
                                        dataInizio = dataFineValiditaMAB;
                                    }
                                    #endregion

                                    #region modello canone
                                    CanoneMABModel cmabm = new CanoneMABModel()
                                    {
                                        idCanone = cmabvm.idCanone,
                                        IDMAB = cmabvm.IDMAB,
                                        IDAttivazioneMAB = attmab_aperta.IDATTIVAZIONEMAB,
                                        idValuta = cmabvm.idValuta,
                                        DataInizioValidita = dataInizio,
                                        DataFineValidita = cmabvm.DataFineValidita,
                                        ImportoCanone = cmabvm.ImportoCanone,
                                        DataAggiornamento = cmabvm.DataAggiornamento,
                                        idStatoRecord = cmabvm.idStatoRecord,
                                        FK_IDCanone = cmabvm.FK_IDCanone,
                                        nascondi = cmabvm.nascondi
                                    };
                                    #endregion

                                    #region inserisce importo
                                    if (cmabvm.chkAggiornaTutti == false)
                                    {
                                        dtma.SetNuovoImportoCanoneMAB(cmabm, idMab, attmab_aperta.IDATTIVAZIONEMAB, dataFineValiditaMAB, db);
                                    }
                                    else
                                    {
                                        //inserisce periodo e annulla i periodi successivi (fino al primo buco temporale o fino a dataRientro)
                                        dtma.SetNuovoImportoCanone_AggiornaTutti(cmabm, idMab, attmab_aperta.IDATTIVAZIONEMAB, dataFineValiditaMAB, db);
                                    }
                                    #endregion

                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento nuovo importo canone MAB (" + idMab + ")", "CANONEMAB", db, idTrasferimento, cmabvm.idCanone);

                                    db.Database.CurrentTransaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    db.Database.CurrentTransaction.Rollback();
                                    throw ex;
                                }
                            }
                            else
                            {
                                List<SelectListItem> lValute = new List<SelectListItem>();

                                var r = new List<SelectListItem>();

                                PeriodoMABModel pmabm = new PeriodoMABModel();

                                var vm = dtma.GetUltimaValutaInseritaModel(idMab, db);

                                cmabvm.descrizioneValuta = vm.descrizioneValuta;
                                cmabvm.idValuta = vm.idValuta;

                                var lv = dtv.GetElencoValute(db);

                                if (lv != null && lv.Count > 0)
                                {
                                    r = (from v in lv
                                         select new SelectListItem()
                                         {
                                             Text = v.DESCRIZIONEVALUTA,
                                             Value = v.IDVALUTA.ToString()
                                         }).ToList();

                                    r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                                }

                                lValute = r;

                                pmabm = dtma.GetPeriodoMABModel(idMab, db);
                                cmabvm.ut_dataInizioValidita = pmabm.dataInizioMAB;

                                ViewData.Add("dataInizioValiditaModificabile", dataInizioValiditaModificabile);

                                ViewData.Add("idMab", idMab);
                                ViewData.Add("idTrasferimento", idTrasferimento);
                                ViewBag.lValute = lValute;


                                return PartialView("NuovoImportoCanone", cmabvm);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


            return RedirectToAction("ElencoCanoneMAB", new { idTrasferimento = idTrasferimento, idMab = idMab });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciPagatoCondiviso(PagatoCondivisoMABViewModel pcmabvm, decimal idMab, decimal idTrasferimento)
        {
            try
            {

                using (dtVariazioniMaggiorazioneAbitazione dtma = new dtVariazioniMaggiorazioneAbitazione())
                {
                    using (ModelDBISE db = new ModelDBISE())
                    {

                        if (ModelState.IsValid)
                        {
                            db.Database.BeginTransaction();
                            try
                            {
                                #region verifica data inizio e effettive variazioni rispetto al record precedente
                                try
                                {
                                    dtma.VerificaDataInizioValiditaPagatoCondivisoMAB_Utente(pcmabvm.idMAB, pcmabvm.ut_dataInizioValidita, db);

                                    dtma.VerificaVariazioniPagatoCondivisoMAB(pcmabvm, db);
                                }
                                catch (Exception ex)
                                {

                                    //PeriodoMABModel pmabm = new PeriodoMABModel();

                                    ModelState.AddModelError("", ex.Message);
                                    ViewData.Add("idMab", idMab);
                                    ViewData.Add("idTrasferimento", idTrasferimento);

                                    return PartialView("NuovoPagatoCondiviso", pcmabvm);
                                }
                                #endregion

                                pcmabvm.DataAggiornamento = DateTime.Now;
                                pcmabvm.idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione;

                                #region attivazione
                                ATTIVAZIONEMAB attmab_aperta = new ATTIVAZIONEMAB();

                                //var attmab_rif = pcmabvm.idAttivazioneMAB;
                                var attmab = dtma.GetAttivazioneAperta(idTrasferimento, db);

                                // se non esiste attivazione aperta la creo altrimenti la uso
                                if (attmab.IDATTIVAZIONEMAB > 0)
                                {
                                    attmab_aperta = attmab;
                                }
                                else
                                {
                                    ATTIVAZIONEMAB new_amab = dtma.CreaAttivazione(idMab, db);
                                    attmab_aperta = new_amab;
                                }
                                #endregion

                                #region date
                                var dataFineValiditaMAB = dtma.GetPeriodoMABModel(pcmabvm.idMAB, db).dataFineMAB;


                                //controlla data inserita superiore a dataRientro
                                DateTime dataInizio;
                                dataInizio = pcmabvm.ut_dataInizioValidita.Value;

                                if (dataInizio > dataFineValiditaMAB)
                                {
                                    dataInizio = dataFineValiditaMAB;
                                }
                                #endregion

                                #region modello pagato condiviso
                                PagatoCondivisoMABModel pcmabm = new PagatoCondivisoMABModel()
                                {
                                    idPagatoCondiviso = pcmabvm.idPagatoCondiviso,
                                    idMAB = pcmabvm.idMAB,
                                    idAttivazioneMAB = attmab_aperta.IDATTIVAZIONEMAB,
                                    idStatoRecord = pcmabvm.idStatoRecord,
                                    Pagato = pcmabvm.Pagato,
                                    Condiviso = pcmabvm.Condiviso,
                                    DataInizioValidita = dataInizio,
                                    DataFineValidita = pcmabvm.DataFineValidita,
                                    Nascondi = pcmabvm.Nascondi,
                                    DataAggiornamento = pcmabvm.DataAggiornamento,
                                    fk_IDPagatoCondiviso = pcmabvm.fk_IDPagatoCondiviso
                                };
                                #endregion

                                #region inserisce record
                                if (pcmabvm.chkAggiornaTutti == false)
                                {
                                    dtma.SetNuovoPagatoCondivisoMAB(pcmabm, idMab, attmab_aperta.IDATTIVAZIONEMAB, dataFineValiditaMAB, db);
                                }
                                else
                                {
                                    //inserisce periodo e annulla i periodi successivi (fino al primo buco temporale o fino a dataRientro)
                                    dtma.SetNuovoPagatoCondiviso_AggiornaTutti(pcmabm, idMab, attmab_aperta.IDATTIVAZIONEMAB, dataFineValiditaMAB, db);
                                }
                                #endregion

                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento nuovo pagato condiviso MAB (" + idMab + ")", "PAGATOCONDIVISOMAB", db, idTrasferimento, pcmabvm.idPagatoCondiviso);

                                db.Database.CurrentTransaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                db.Database.CurrentTransaction.Rollback();
                                throw ex;
                            }
                        }
                        else
                        {

                            PeriodoMABModel pmabm = new PeriodoMABModel();

                            pmabm = dtma.GetPeriodoMABModel(idMab, db);
                            pcmabvm.ut_dataInizioValidita = pmabm.dataInizioMAB;

                            ViewData.Add("idMab", idMab);
                            ViewData.Add("idTrasferimento", idTrasferimento);

                            return PartialView("NuovoPagatoCondiviso", pcmabvm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


            return RedirectToAction("ElencoPagatoCondivisoMAB", new { idTrasferimento = idTrasferimento, idMab = idMab });
        }

        public ActionResult GestioneMAB_var(decimal idTrasferimento)
        {
            try
            {
                ViewData.Add("idTrasferimento", idTrasferimento);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView();
        }

        public ActionResult ElencoFormulariMABInseriti(decimal idTrasferimento, decimal idMab)
        {
            bool solaLettura = false;
            bool trasfSolaLettura = false;

            List<SelectListItem> lDataAttivazione = new List<SelectListItem>();
            List<AttivazioneMABModel> lamab = new List<AttivazioneMABModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        using (dtVariazioniMaggiorazioneAbitazione dtvmab = new dtVariazioniMaggiorazioneAbitazione())
                        {
                            ATTIVAZIONEMAB att_MAB_curr = dtvmab.GetUltimaAttivazioneMABCorrente(idMab, db);

                            lamab = dtvmab.GetListaAttivazioniMABconDocumentiModel(idMab, db).ToList();

                            if (att_MAB_curr.NOTIFICARICHIESTA && att_MAB_curr.ATTIVAZIONE == false)
                            {
                                solaLettura = true;
                            }

                            var t = dtt.GetTrasferimentoById(idTrasferimento);
                            if (t.idStatoTrasferimento == EnumStatoTraferimento.Annullato)
                            {
                                trasfSolaLettura = true;
                            }

                            var i = 1;

                            foreach (var e in lamab)
                            {
                                if (!e.Annullato)
                                {

                                    if (!trasfSolaLettura)
                                    {
                                        if (e.notificaRichiesta == false)
                                        {
                                            lDataAttivazione.Insert(0, new SelectListItem() { Text = "(" + i.ToString() + ") " + e.dataVariazione.ToString() + " (In Lavorazione)", Value = e.idAttivazioneMAB.ToString() });
                                            solaLettura = false;
                                        }
                                        if (e.notificaRichiesta && e.Attivazione == false)
                                        {
                                            lDataAttivazione.Insert(0, new SelectListItem() { Text = "(" + i.ToString() + ") " + e.dataVariazione.ToString() + " (Da Attivare)", Value = e.idAttivazioneMAB.ToString() });
                                        }
                                        if (e.notificaRichiesta && e.Attivazione)
                                        {
                                            lDataAttivazione.Insert(0, new SelectListItem() { Text = "(" + i.ToString() + ") " + e.dataVariazione.ToString(), Value = e.idAttivazioneMAB.ToString() });
                                        }
                                    }
                                    else
                                    {
                                        lDataAttivazione.Insert(0, new SelectListItem() { Text = "(" + i.ToString() + ") " + e.dataVariazione.ToString(), Value = e.idAttivazioneMAB.ToString() });
                                    }
                                    i++;
                                }
                            }
                        }
                        lDataAttivazione.Insert(0, new SelectListItem() { Text = "(TUTTE)", Value = "0" });
                        ViewData.Add("lDataAttivazione", lDataAttivazione);
                        ViewData["idTrasferimento"] = idTrasferimento;
                        ViewData["idMab"] = idMab;
                        ViewData.Add("solaLettura", solaLettura);
                    }

                    return PartialView();
                }
            }

            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }


        public ActionResult FormulariMAB_var(decimal idTrasferimento)
        {
            bool siDocModulo1 = false;
            bool siDocModulo2 = false;
            bool siDocModulo3 = false;
            bool siDocModulo4 = false;
            bool siDocModulo5 = false;
            bool siDocCopiaContratto = false;
            bool siDocCopiaRicevuta = false;
            decimal idDocModulo1 = 0;
            decimal idDocModulo2 = 0;
            decimal idDocModulo3 = 0;
            decimal idDocModulo4 = 0;
            decimal idDocModulo5 = 0;
            decimal idDocCopiaContratto = 0;
            decimal idDocCopiaRicevuta = 0;
            EnumStatoTraferimento statoTrasferimento = 0;


            //MaggiorazioneAbitazioneModel mam = new MaggiorazioneAbitazioneModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                    {
                        bool soloLettura = false;

                        ATTIVAZIONEMAB am = dtvma.GetUltimaAttivazioneMAB(idTrasferimento, db);

                        if (am != null && am.IDATTIVAZIONEMAB > 0)
                        {
                            dtvma.VerificaDocumentiValidi_var(am,
                                                        out siDocCopiaContratto,
                                                        out siDocCopiaRicevuta,
                                                        out siDocModulo1,
                                                        out siDocModulo2,
                                                        out siDocModulo3,
                                                        out siDocModulo4,
                                                        out siDocModulo5,
                                                        out idDocCopiaContratto,
                                                        out idDocCopiaRicevuta,
                                                        out idDocModulo1,
                                                        out idDocModulo2,
                                                        out idDocModulo3,
                                                        out idDocModulo4,
                                                        out idDocModulo5);

                            if (am.NOTIFICARICHIESTA && am.ATTIVAZIONE == false)
                            {
                                soloLettura = true;
                            }

                            using (dtTrasferimento dtt = new dtTrasferimento())
                            {
                                var t = dtt.GetTrasferimentoById(idTrasferimento);
                                statoTrasferimento = t.idStatoTrasferimento;
                                if (statoTrasferimento == EnumStatoTraferimento.Annullato)
                                {
                                    soloLettura = true;
                                }
                            }

                        }
                        //decimal NumAttivazioni = dtvma.GetNumAttivazioniMAB_var(idTrasferimento);

                        ViewData.Add("idTrasferimento", idTrasferimento);
                        ViewData.Add("siDocCopiaContratto", siDocCopiaContratto);
                        ViewData.Add("siDocCopiaRicevuta", siDocCopiaRicevuta);
                        ViewData.Add("siDocModulo1", siDocModulo1);
                        ViewData.Add("siDocModulo2", siDocModulo2);
                        ViewData.Add("siDocModulo3", siDocModulo3);
                        ViewData.Add("siDocModulo4", siDocModulo4);
                        ViewData.Add("siDocModulo5", siDocModulo5);
                        ViewData.Add("idDocCopiaContratto", idDocCopiaContratto);
                        ViewData.Add("idDocCopiaRicevuta", idDocCopiaRicevuta);
                        ViewData.Add("idDocModulo1", idDocModulo1);
                        ViewData.Add("idDocModulo2", idDocModulo2);
                        ViewData.Add("idDocModulo3", idDocModulo3);
                        ViewData.Add("idDocModulo4", idDocModulo4);
                        ViewData.Add("idDocModulo5", idDocModulo5);
                        ViewData.Add("soloLettura", soloLettura);
                        //ViewData.Add("NumAttivazioni", NumAttivazioni);
                    }
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }


        public ActionResult GestionePulsantiMAB_var(decimal idTrasferimento)
        {
            AttivazioneMABModel amm = new AttivazioneMABModel();
            ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();
            //MaggiorazioneAbitazioneModel mam = new MaggiorazioneAbitazioneModel();

            try
            {
                bool amministratore = Utility.Amministratore();

                string disabledNotificaRichiesta = "disabled";
                string hiddenNotificaRichiesta = "";
                string disabledAttivaRichiesta = "disabled";
                string hiddenAttivaRichiesta = "hidden";
                string disabledAnnullaRichiesta = "disabled";
                string hiddenAnnullaRichiesta = "hidden";
                EnumStatoTraferimento statoTrasferimento = 0;

                bool esistonoVariazioni = false;
                bool notificaRichiesta = false;
                bool attivaRichiesta = false;

                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                    {
                        amm = dtvma.GetUltimaAttivazioneMABmodel(idTrasferimento);
                        am = dtvma.GetUltimaAttivazioneMAB(idTrasferimento, db);
                        if (am.IDATTIVAZIONEMAB > 0)
                        {
                            var mab = dtvma.GetMAB_ByID_var(am.IDMAB, db);
                            esistonoVariazioni = false;


                            notificaRichiesta = am.NOTIFICARICHIESTA;
                            attivaRichiesta = am.ATTIVAZIONE;

                            using (dtTrasferimento dtt = new dtTrasferimento())
                            {
                                var t = dtt.GetTrasferimentoById(idTrasferimento);
                                statoTrasferimento = t.idStatoTrasferimento;
                            }
                            if (mab.IDMAB > 0)
                            {
                                esistonoVariazioni = dtvma.VerificaVariazioniMAB(mab.IDMAB, db, true);

                            }
                        }

                        //se amministratore vedo i pulsanti altrimenti solo notifica
                        if (amministratore)
                        {
                            hiddenAttivaRichiesta = "";
                            hiddenAnnullaRichiesta = "";

                            if (notificaRichiesta &&
                                attivaRichiesta == false &&
                                statoTrasferimento != EnumStatoTraferimento.Annullato
                                )
                            {
                                disabledAttivaRichiesta = "";
                                disabledAnnullaRichiesta = "";
                            }

                            if (notificaRichiesta == false &&
                                attivaRichiesta == false &&
                                statoTrasferimento != EnumStatoTraferimento.Annullato &&
                                esistonoVariazioni
                                )
                            {
                                disabledNotificaRichiesta = "";
                            }
                        }
                        else
                        {
                            if (
                                notificaRichiesta == false &&
                                attivaRichiesta == false &&
                                statoTrasferimento != EnumStatoTraferimento.Annullato &&
                                esistonoVariazioni
                                )
                            {
                                disabledNotificaRichiesta = "";
                            }
                        }

                        ViewData.Add("idTrasferimento", idTrasferimento);
                        ViewData.Add("idAttivazioneMAB", am.IDATTIVAZIONEMAB);
                        ViewData.Add("disabledAnnullaRichiesta", disabledAnnullaRichiesta);
                        ViewData.Add("disabledAttivaRichiesta", disabledAttivaRichiesta);
                        ViewData.Add("disabledNotificaRichiesta", disabledNotificaRichiesta);
                        ViewData.Add("hiddenAnnullaRichiesta", hiddenAnnullaRichiesta);
                        ViewData.Add("hiddenAttivaRichiesta", hiddenAttivaRichiesta);
                        ViewData.Add("hiddenNotificaRichiesta", hiddenNotificaRichiesta);
                        ViewData.Add("amministratore", amministratore);
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(amm);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DocumentoMAB_var(decimal idTipoDocumento, decimal idTrasferimento)
        {
            TrasferimentoModel trm = new TrasferimentoModel();
            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    using (dtDocumenti dtd = new dtDocumenti())
                    {
                        trm = dtt.GetTrasferimentoById(idTrasferimento);

                        var DescDocumento = dtd.GetDescrizioneTipoDocumentoByIdTipoDocumento(idTipoDocumento);

                        ViewData.Add("idTipoDocumento", idTipoDocumento);
                        ViewData.Add("DescDocumento", DescDocumento);

                        return PartialView(trm);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult FiltraTabFormulariMABInseriti(decimal idmab, decimal idAttivazione)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            try
            {
                using (dtDocumenti dtd = new dtDocumenti())
                {
                    ldm = dtd.GetFormulariMaggiorazioniAbitazioneVariazioneByIdAttivazione(idmab, idAttivazione).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("TabFormulariMABInseriti", ldm);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult InserisciDocumentoMAB_var(decimal idMab, decimal idTipoDocumento, HttpPostedFileBase file)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                    {
                        //check attivazione (legge ultima attivazione, se nn esiste la crea)
                        //AttivazioneMABModel amm = dtvma.GetUltimaAttivazioneMABmodel(idTrasferimento);

                        ////verifica se è in lavorazione e se non lo è la creo
                        //if (amm.notificaRichiesta && amm.Attivazione)
                        //{
                        //    ATTIVAZIONEMAB am = dtvma.CreaAttivazioneMAB_var(idTrasferimento, db);
                        //    amm.idAttivazioneMAB = am.IDATTIVAZIONEMAB;
                        //}

                        ATTIVAZIONEMAB att = dtvma.GetUltimaAttivazioneMABCorrente(idMab, db);
                        if (att.NOTIFICARICHIESTA)
                        {
                            att = dtvma.CreaAttivazione(idMab, db);
                        }

                        att.DATAVARIAZIONE = DateTime.Now;
                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore in fase di aggiornamento Data Attivazione.");
                        }

                        using (dtDocumenti dtd = new dtDocumenti())
                        {
                            DocumentiModel dm = new DocumentiModel();
                            bool esisteFile = false;
                            bool gestisceEstensioni = false;
                            bool dimensioneConsentita = false;
                            string dimensioneMaxConsentita = string.Empty;

                            Utility.PreSetDocumento(file, out dm, out esisteFile, out gestisceEstensioni,
                                out dimensioneConsentita, out dimensioneMaxConsentita,
                                (EnumTipoDoc)idTipoDocumento);

                            if (esisteFile)
                            {
                                if (gestisceEstensioni == false)
                                {
                                    throw new Exception(
                                        "Il documento selezionato non è nel formato consentito. Il formato supportato è: pdf.");
                                }

                                if (dimensioneConsentita)
                                {
                                    //verifica se il documento è gia presente ritornando l'eventuale id
                                    decimal idDocumentoEsistente = dtvma.VerificaEsistenzaDocumentoMAB_var(idMab, (EnumTipoDoc)idTipoDocumento);

                                    if (idDocumentoEsistente > 0)
                                    {
                                        //se già esiste lo sostituisco (imposto modificato=true su quello esistente e ne inserisco una altro)
                                        dtvma.SostituisciDocumentoMAB_var(ref dm, idDocumentoEsistente, att.IDATTIVAZIONEMAB, db);

                                    }
                                    else
                                    {
                                        //se non esiste lo inserisco
                                        dtvma.SetDocumentoMAB_var(ref dm, att.IDATTIVAZIONEMAB, db);
                                    }

                                    var idTrasferimento = db.MAB.Find(idMab).INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO;

                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (maggiorazione abitazione).", "Documenti", db, idTrasferimento, dm.idDocumenti);

                                }
                                else
                                {
                                    throw new Exception(
                                        "Il documento selezionato supera la dimensione massima consentita (" +
                                        dimensioneMaxConsentita + " Mb).");
                                }
                            }
                            else
                            {
                                throw new Exception("Il documento è obbligatorio.");
                            }
                        }
                    }

                    db.Database.CurrentTransaction.Commit();
                    return Json(new { msg = "Il documento è stato inserito." });
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    return Json(new { err = ex.Message });
                }
            }
        }

        public JsonResult ConfermaAttivaRichiestaMAB_var(decimal idAttivazioneMAB)
        {
            string errore = "";

            try
            {
                using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                {
                    dtvma.AttivaRichiestaMAB_var(idAttivazioneMAB);
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
        public JsonResult ConfermaAnnullaRichiestaMAB_var(decimal idAttivazioneMAB, string msg)
        {
            string errore = "";

            try
            {
                using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                {
                    dtvma.AnnullaRichiestaMAB_var(idAttivazioneMAB, msg);
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


        public JsonResult ConfermaNotificaRichiestaMAB_var(decimal idAttivazioneMAB)
        {
            string errore = "";

            try
            {
                using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                {

                    dtvma.NotificaRichiestaMAB_var(idAttivazioneMAB);
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

        public ActionResult NuovaMAB_var(decimal idTrasferimento)
        {
            MABViewModel mabvm = new MABViewModel();
            //MAB mab_partenza = new MAB();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                    {
                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            var tm = dtt.GetTrasferimentoById(idTrasferimento);

                            mabvm.idTrasferimento = idTrasferimento;

                            mabvm.dataInizioMAB = tm.dataPartenza;
                            mabvm.dataFineMAB = tm.dataRientro.Value;

                            var aa = dtvma.GetMaggiorazioneAnnuale_var(mabvm, db);
                            if (aa.IDMAGANNUALI > 0)
                            {
                                mabvm.annualita = true;
                            }
                            return PartialView(mabvm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }


        public ActionResult ModificaMAB_var(decimal idMAB)
        {
            MABViewModel mabvm = new MABViewModel();
            //VARIAZIONIMAB vmab = new VARIAZIONIMAB();

            //List<SelectListItem> lValute = new List<SelectListItem>();

            var r = new List<SelectListItem>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                    {
                        //using (dtValute dtv = new dtValute())
                        //{
                        //using (dtTrasferimento dtt = new dtTrasferimento())
                        //{
                        var mab = dtvma.GetMAB_ByID_var(idMAB, db);

                        if (mab.IDMAB > 0)
                        {
                            mabvm.idMAB = mab.IDMAB;

                            var att = mab.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAB).ToList().First();
                            //mam.dataPartenza = t.dataPartenza;
                            mabvm.idTrasferimento = mab.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO;
                            mabvm.idAttivazioneMAB = att.IDATTIVAZIONEMAB;

                            var pmm = dtvma.GetPeriodoMABModel(mab.IDMAB, db);

                            mabvm.modificabile = true;

                            PeriodoMABModel pm_partenza = new PeriodoMABModel();
                            MABModel mab_partenza = new MABModel();
                            using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                            {
                                mab_partenza = dtma.GetMABModelPartenza(mabvm.idTrasferimento, db);
                            }
                            pm_partenza = dtvma.GetPeriodoMABModel(mab_partenza.idMAB, db);


                            if (pmm.dataInizioMAB == pm_partenza.dataInizioMAB)
                            {
                                mabvm.modificabile = false;
                            }

                            var att_curr = dtvma.GetAttivazioneById(mabvm.idAttivazioneMAB, db);
                            if (att_curr.NOTIFICARICHIESTA)
                            {
                                mabvm.modificabile = false;
                            }

                            mabvm.dataInizioMAB = pmm.dataInizioMAB;
                            mabvm.dataFineMAB = pmm.dataFineMAB;
                            mabvm.ut_dataInizioMAB = pmm.dataInizioMAB;

                            if (pmm.dataFineMAB == Utility.DataFineStop())
                            {
                                mabvm.ut_dataFineMAB = null;
                            }
                            else
                            {
                                mabvm.ut_dataFineMAB = pmm.dataFineMAB;
                            }
                            mabvm.rinunciaMAB = mab.RINUNCIAMAB;
                            mabvm.anticipoAnnuale = dtvma.AnticipoAnnualeMAB(idMAB, db);

                            mabvm.annualita = false;
                            mabvm.annualita_modificabile = false;
                            var aa = dtvma.GetMaggiorazioneAnnuale_var(mabvm, db);
                            if (aa.IDMAGANNUALI > 0)
                            {
                                mabvm.annualita = true;
                                var att_mab = dtvma.GetAttivazioneById(att.IDATTIVAZIONEMAB, db);
                                if (att_mab.ATTIVAZIONE == false)
                                {
                                    mabvm.annualita_modificabile = true;
                                }
                            }
                            ViewData.Add("idMAB", idMAB);
                            ViewData.Add("idTrasferimento", mabvm.idTrasferimento);
                            //ViewBag.lValute = lValute;

                            return PartialView(mabvm);

                        }
                        else
                        {
                            throw new Exception("Errore nella lettura del dettaglio MAB");
                        }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfermaNuovaMAB_var(MABViewModel mabvm, decimal idTrasferimento)
        {
            MABViewModel mam = new MABViewModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                    {

                        try
                        {
                            if (ModelState.IsValid)
                            {
                                try
                                {
                                    try
                                    {
                                        dtvma.VerificaDateMAB_Utente(mabvm, idTrasferimento, mabvm.ut_dataInizioMAB, mabvm.ut_dataFineMAB, true, db);
                                    }
                                    catch (Exception ex)
                                    {
                                        return Json(new { ErrorMessage = ex.Message });
                                    }

                                    dtvma.InserisciMAB_var(mabvm, idTrasferimento);
                                }
                                catch (Exception ex)
                                {
                                    ModelState.AddModelError("", ex.Message);

                                    var tm = dtt.GetTrasferimentoById(idTrasferimento);

                                    mabvm.idTrasferimento = idTrasferimento;

                                    mabvm.dataInizioMAB = tm.dataPartenza;
                                    mabvm.dataFineMAB = tm.dataRientro.Value;

                                    var aa = dtvma.GetMaggiorazioneAnnuale_var(mabvm, db);
                                    if (aa.IDMAGANNUALI > 0)
                                    {
                                        mabvm.annualita = true;
                                    }
                                    ViewData.Add("idTrasferimento", idTrasferimento);

                                    return PartialView("NuovaMAB_var", mabvm);
                                }
                            }
                            else
                            {
                                var tm = dtt.GetTrasferimentoById(idTrasferimento);

                                mabvm.idTrasferimento = idTrasferimento;

                                mabvm.dataInizioMAB = tm.dataPartenza;
                                mabvm.dataFineMAB = tm.dataRientro.Value;

                                var aa = dtvma.GetMaggiorazioneAnnuale_var(mabvm, db);
                                if (aa.IDMAGANNUALI > 0)
                                {
                                    mabvm.annualita = true;
                                }

                                return PartialView("NuovaMAB_var", mabvm);
                            }
                        }
                        catch (Exception ex)
                        {
                            PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
                        }

                        //return RedirectToAction("AttivitaMAB_var", new { idTrasferimento = idTrasferimento });
                        return RedirectToAction("GestioneMAB_var", new { idTrasferimento = idTrasferimento });

                    }
                }
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfermaModificaMAB_var(MABViewModel mvm, decimal idTrasferimento, decimal idMAB)
        {
            //MABViewModel mam = new MABViewModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                    {

                        if (ModelState.IsValid)
                        {
                            try
                            {

                                try
                                {
                                    dtvma.VerificaDateMAB_Utente(mvm, idTrasferimento, mvm.dataInizioMAB, mvm.ut_dataFineMAB, false, db);
                                    //dtvma.VerificaDataFineMAB_Utente(mvm.idMAB, mvm.ut_dataFineMAB, db);
                                }
                                catch (Exception ex)
                                {
                                    ModelState.AddModelError("", ex.Message);

                                    var mab = dtvma.GetMAB_ByID_var(idMAB, db);

                                    if (mab.IDMAB > 0)
                                    {
                                        mvm.idMAB = mab.IDMAB;
                                        var att = mab.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAB).ToList().First();

                                        mvm.idTrasferimento = mab.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO;
                                        mvm.idAttivazioneMAB = att.IDATTIVAZIONEMAB;

                                        var pmm = dtvma.GetPeriodoMABModel(mab.IDMAB, db);

                                        mvm.modificabile = true;

                                        PeriodoMABModel pm_partenza = new PeriodoMABModel();
                                        MABModel mab_partenza = new MABModel();
                                        using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                                        {
                                            mab_partenza = dtma.GetMABModelPartenza(mvm.idTrasferimento, db);
                                        }
                                        pm_partenza = dtvma.GetPeriodoMABModel(mab_partenza.idMAB, db);


                                        if (pmm.dataInizioMAB == pm_partenza.dataInizioMAB)
                                        {
                                            mvm.modificabile = false;
                                        }

                                        var att_curr = dtvma.GetAttivazioneById(mvm.idAttivazioneMAB, db);
                                        if (att_curr.NOTIFICARICHIESTA)
                                        {
                                            mvm.modificabile = false;
                                        }

                                        //mvm.dataInizioMAB = pmm.dataInizioMAB;
                                        //mvm.dataFineMAB = pmm.dataFineMAB;
                                        mvm.rinunciaMAB = mab.RINUNCIAMAB;
                                        mvm.anticipoAnnuale = dtvma.AnticipoAnnualeMAB(idMAB, db);

                                        mvm.annualita = false;
                                        var aa = dtvma.GetMaggiorazioneAnnuale_var(mvm, db);
                                        if (aa.IDMAGANNUALI > 0)
                                        {
                                            mvm.annualita = true;
                                        }

                                    }


                                    ViewData.Add("idMAB", idMAB);
                                    ViewData.Add("idTrasferimento", idTrasferimento);

                                    return PartialView("ModificaMAB_var", mvm);
                                }

                                dtvma.AggiornaMAB_var(mvm, idTrasferimento, idMAB);
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError("", ex.Message);

                                var mab = dtvma.GetMAB_ByID_var(idMAB, db);

                                if (mab.IDMAB > 0)
                                {
                                    mvm.idMAB = mab.IDMAB;
                                    var att = mab.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAB).ToList().First();
                                    mvm.idTrasferimento = mab.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO;
                                    mvm.idAttivazioneMAB = att.IDATTIVAZIONEMAB;

                                    var pmm = dtvma.GetPeriodoMABModel(mab.IDMAB, db);

                                    mvm.modificabile = true;

                                    PeriodoMABModel pm_partenza = new PeriodoMABModel();
                                    MABModel mab_partenza = new MABModel();
                                    using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                                    {
                                        mab_partenza = dtma.GetMABModelPartenza(mvm.idTrasferimento, db);
                                    }
                                    pm_partenza = dtvma.GetPeriodoMABModel(mab_partenza.idMAB, db);


                                    if (pmm.dataInizioMAB == pm_partenza.dataInizioMAB)
                                    {
                                        mvm.modificabile = false;
                                    }

                                    var att_curr = dtvma.GetAttivazioneById(mvm.idAttivazioneMAB, db);
                                    if (att_curr.NOTIFICARICHIESTA)
                                    {
                                        mvm.modificabile = false;
                                    }

                                    //mvm.dataInizioMAB = pmm.dataInizioMAB;
                                    //mvm.dataFineMAB = pmm.dataFineMAB;
                                    mvm.rinunciaMAB = mab.RINUNCIAMAB;
                                    mvm.anticipoAnnuale = dtvma.AnticipoAnnualeMAB(idMAB, db);

                                    mvm.annualita = false;
                                    var aa = dtvma.GetMaggiorazioneAnnuale_var(mvm, db);
                                    if (aa.IDMAGANNUALI > 0)
                                    {
                                        mvm.annualita = true;
                                    }

                                }

                                ViewData.Add("idMAB", idMAB);
                                ViewData.Add("idTrasferimento", idTrasferimento);

                                return PartialView("ModificaMAB_var", mvm);
                            }
                        }
                        else
                        {

                            //ModelState.AddModelError("", ex.Message);

                            var mab = dtvma.GetMAB_ByID_var(idMAB, db);

                            if (mab.IDMAB > 0)
                            {
                                mvm.idMAB = mab.IDMAB;
                                var att = mab.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAB).ToList().First();
                                mvm.idTrasferimento = mab.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO;
                                mvm.idAttivazioneMAB = att.IDATTIVAZIONEMAB;

                                var pmm = dtvma.GetPeriodoMABModel(mab.IDMAB, db);


                                mvm.modificabile = true;

                                PeriodoMABModel pm_partenza = new PeriodoMABModel();
                                MABModel mab_partenza = new MABModel();
                                using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                                {
                                    mab_partenza = dtma.GetMABModelPartenza(mvm.idTrasferimento, db);
                                }
                                pm_partenza = dtvma.GetPeriodoMABModel(mab_partenza.idMAB, db);


                                if (pmm.dataInizioMAB == pm_partenza.dataInizioMAB)
                                {
                                    mvm.modificabile = false;
                                }

                                var att_curr = dtvma.GetAttivazioneById(mvm.idAttivazioneMAB, db);
                                if (att_curr.NOTIFICARICHIESTA)
                                {
                                    mvm.modificabile = false;
                                }

                                //    mvm.dataInizioMAB = pmm.dataInizioMAB;
                                //    mvm.dataFineMAB = pmm.dataFineMAB;
                                mvm.rinunciaMAB = mab.RINUNCIAMAB;
                                mvm.anticipoAnnuale = dtvma.AnticipoAnnualeMAB(idMAB, db);

                                mvm.annualita = false;
                                var aa = dtvma.GetMaggiorazioneAnnuale_var(mvm, db);
                                if (aa.IDMAGANNUALI > 0)
                                {
                                    mvm.annualita = true;
                                }

                                ViewData.Add("idMAB", idMAB);
                                ViewData.Add("idTrasferimento", idTrasferimento);

                                return PartialView("ModificaMAB_var", mvm);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return RedirectToAction("GestioneMAB_var", new { idTrasferimento = idTrasferimento });
        }

        public ActionResult MessaggioAnnullaMAB_var(decimal idTrasferimento)
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
                            var t = dtt.GetTrasferimentoById(idTrasferimento);

                            if (t?.idTrasferimento > 0)
                            {
                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                var uff = dtu.GetUffici(t.idUfficio);

                                msg.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaMaggiorazioneAbitazione, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString());
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

        public JsonResult ConfermaAnnullaModificheCanoneMAB(decimal idMAB)
        {
            string errore = "";
            try
            {
                using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                {
                    dtvma.AnnullaModificheCanoneMAB(idMAB);
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

        public JsonResult ConfermaAnnullaModifichePagatoCondivisoMAB(decimal idMAB)
        {
            string errore = "";
            try
            {
                using (dtVariazioniMaggiorazioneAbitazione dtvma = new dtVariazioniMaggiorazioneAbitazione())
                {
                    dtvma.AnnullaModifichePagatoCondivisoMAB(idMAB);
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

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult TabFormulariMABInseriti(decimal idMab)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtVariazioniMaggiorazioneAbitazione dtmab = new dtVariazioniMaggiorazioneAbitazione())
                    {
                        using (dtDocumenti dtd = new dtDocumenti())
                        {
                            var mab = dtmab.GetMAB_ByID_var(idMab, db);
                            var t = mab.INDENNITA.TRASFERIMENTO;
                            bool solaLettura = false;
                            var amab = dtmab.GetAttivazioneAperta(t.IDTRASFERIMENTO, db);
                            if (amab.IDATTIVAZIONEMAB > 0)
                            {
                                solaLettura = true;
                            }
                            ViewData.Add("solaLettura", solaLettura);

                            ldm = dtd.GetFormulariMaggiorazioniAbitazioneVariazione(idMab, db).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(ldm);
        }

        public JsonResult ConfermaAnnullaModificheMAB(decimal idMAB)
        {
            try
            {
                using (dtVariazioniMaggiorazioneAbitazione dtvmab = new dtVariazioniMaggiorazioneAbitazione())
                {
                    dtvmab.AnnullaModificheMAB(idMAB);
                }
            }
            catch (Exception ex)
            {
                return Json(new { errore = ex.Message, msg = "" });
            }
            return Json(new { errore = "", msg = "Eliminazione effettuata con successo." });
        }


    }
}
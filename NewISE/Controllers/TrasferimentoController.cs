using NewISE.EF;
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models;
using NewISE.Models.DBModel;

using NewISE.Models.DBModel.dtObj;
using NewISE.Models.dtObj;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Resources;
using NewISE.Models.ViewModel;
using System.Web.Helpers;
using NewISE.Models.Enumeratori;

namespace NewISE.Controllers
{

    public class TrasferimentoController : Controller
    {
        #region Metodi privati

        private void ListeComboNuovoTrasf(out List<SelectListItem> lTipoTrasferimento, out List<SelectListItem> lUffici, out List<SelectListItem> lRuoloUfficio, out List<SelectListItem> lTipologiaCoan, out List<SelectListItem> lFasciaKM)
        {
            var r1 = new List<SelectListItem>();
            var r2 = new List<SelectListItem>();
            var r3 = new List<SelectListItem>();
            var r4 = new List<SelectListItem>();
            var r5 = new List<SelectListItem>();

            using (dtTipoTrasferimento dttt = new dtTipoTrasferimento())
            {
                var ltt = dttt.GetListTipoTrasferimento().OrderBy(a => a.descTipoTrasf).ToList();

                if (ltt != null && ltt.Count > 0)
                {
                    r1 = (from t in ltt
                          select new SelectListItem()
                          {
                              Text = t.descTipoTrasf,
                              Value = t.idTipoTrasferimento.ToString()
                          }).ToList();
                    r1.Insert(0, new SelectListItem() { Text = "", Value = "" });
                }

                lTipoTrasferimento = r1;
            }

            using (dtUffici dtl = new dtUffici())
            {
                var llm = dtl.GetUffici().OrderBy(a => a.descUfficio).ToList();

                if (llm != null && llm.Count > 0)
                {
                    r2 = (from t in llm
                          select new SelectListItem()
                          {
                              Text = t.descUfficio,
                              Value = t.idUfficio.ToString()
                          }).ToList();

                    r2.Insert(0, new SelectListItem() { Text = "", Value = "" });
                }

                lUffici = r2;
            }

            using (dtRuoloUfficio dtru = new dtRuoloUfficio())
            {
                var lru = dtru.GetListRuoloUfficio().OrderBy(a => a.DescrizioneRuolo).ToList();

                if (lru != null && lru.Count > 0)
                {
                    r3 = (from t in lru
                          select new SelectListItem()
                          {
                              Text = t.DescrizioneRuolo,
                              Value = t.idRuoloUfficio.ToString()
                          }).ToList();

                    r3.Insert(0, new SelectListItem() { Text = "", Value = "" });
                }

                lRuoloUfficio = r3;
            }

            using (dtTipologiaCoan dttc = new dtTipologiaCoan())
            {
                var ltc = dttc.GetListTipologiaCoan().OrderBy(a => a.descrizione).ToList();

                if (ltc != null && ltc.Count > 0)
                {
                    r4 = (from t in ltc
                          select new SelectListItem()
                          {
                              Text = t.descrizione,
                              Value = t.idTipoCoan.ToString()
                          }).ToList();

                    r4.Insert(0, new SelectListItem() { Text = "", Value = "" });
                }

                lTipologiaCoan = r4;
            }


            using (dtFasciaKm dtfkm = new dtFasciaKm())
            {
                var lfkm = dtfkm.GetListFascieChilometriche().ToList();

                if (lfkm?.Any() ?? false)
                {
                    r5 = (from t in lfkm
                          select new SelectListItem()
                          {
                              Text = t.KM,
                              Value = t.idFKM.ToString()
                          }).ToList();

                    r5.Insert(0, new SelectListItem() { Text = "", Value = "" });
                }

                lFasciaKM = r5;
            }


        }

        private void FiltraRuoloUfficio(out List<SelectListItem> lRuoloUfficio, DipendentiModel dm)
        {

            using (dtLivelliDipendente dtpl = new dtLivelliDipendente())
            {
                dm.livelloDipendenteValido = dtpl.GetLivelloDipendente(dm.idDipendente, DateTime.Now.Date);

                var r = new List<SelectListItem>();

                using (dtRuoloUfficio dtru = new dtRuoloUfficio())
                {
                    var lru = dtru.GetListRuoloUfficioByLivello(dm.livelloDipendenteValido.idLivello).OrderBy(a => a.DescrizioneRuolo).ToList();

                    if (lru != null && lru.Count > 0)
                    {
                        r = (from t in lru
                             select new SelectListItem()
                             {
                                 Text = t.DescrizioneRuolo,
                                 Value = t.idRuoloUfficio.ToString()
                             }).ToList();

                        r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                    }

                    lRuoloUfficio = r;
                }
            }

        }


        #endregion Metodi privati

        [HttpPost]
        public JsonResult GestioneAttivitaTrasferimento(decimal idTrasferimento)
        {
            string errore = string.Empty;
            bool richiestaMF = false;
            bool attivazioneMF = false;

            bool richiestaPP = false;
            bool conclusePP = false;
            bool faseRichiestaPPattivata = false;
            bool faseInvioPPattivata = false;

            bool richiesteTV = false;
            bool concluseTV = false;
            bool richiestaTE = false;
            bool attivazioneTE = false;
            bool richiestaAnticipi = false;
            bool attivazioneAnticipi = false;
            bool richiestaMAB = false;
            bool attivazioneMAB = false;
            bool richiestaPS = false;
            bool attivazionePS = false;
            bool solaLettura = false;
            bool amministratore = false;

            try
            {
                amministratore = Utility.Amministratore();

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    dtt.GestioneAttivitaTrasferimento(idTrasferimento, out richiestaMF, out attivazioneMF,
                        out richiestaPP, out conclusePP,
                        out faseRichiestaPPattivata, out faseInvioPPattivata,
                        out richiesteTV, out concluseTV, out richiestaTE, out attivazioneTE,
                        out richiestaAnticipi, out attivazioneAnticipi, out richiestaMAB, out attivazioneMAB, out richiestaPS, out attivazionePS,
                        out solaLettura);
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
                        admin = amministratore,
                        err = errore,
                        richiestaMF = richiestaMF,
                        attivazioneMF = attivazioneMF,
                        richiestaPP = richiestaPP,
                        conclusePP = conclusePP,
                        faseRichiestaPPattivata = faseRichiestaPPattivata,
                        faseInvioPPattivata = faseInvioPPattivata,
                        richiesteTV = richiesteTV,
                        concluseTV = concluseTV,
                        richiestaTE = richiestaTE,
                        attivazioneTE = attivazioneTE,
                        richiestaAnticipi = richiestaAnticipi,
                        attivazioneAnticipi = attivazioneAnticipi,
                        richiestaMAB = richiestaMAB,
                        attivazioneMAB = attivazioneMAB,
                        richiestaPS = richiestaPS,
                        attivazionePS = attivazionePS,
                        solaLettura = solaLettura
                    });

        }


        public ActionResult InfoTrasferimento(decimal idTrasferimento)
        {
            dipInfoTrasferimentoModel dit = new dipInfoTrasferimentoModel();

            try
            {
                using (dtTrasferimento dtdt = new dtTrasferimento())
                {
                    dit = dtdt.GetInfoTrasferimento(idTrasferimento);

                    if (dit.CDCDestinazione == string.Empty)
                    {
                        dit.statoTrasferimento = EnumStatoTraferimento.Non_Trasferito;
                        dit.UfficioDestinazione = new UfficiModel();
                        dit.RuoloUfficio = new RuoloUfficioModel();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView(dit);
        }

        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        public ActionResult NuovoTrasferimentoDaUt(decimal idTrasfOld)
        {
            var lTipoTrasferimento = new List<SelectListItem>();
            var lUffici = new List<SelectListItem>();
            var lRuoloUfficio = new List<SelectListItem>();
            var lTipologiaCoan = new List<SelectListItem>();
            var lFasciaKM = new List<SelectListItem>();

            int matricola = 0;
            bool ricaricaInfoTrasf = true;

            try
            {
                using (dtDipendenti dtd = new dtDipendenti())
                {
                    var d = dtd.GetDipendenteByIDTrasf(idTrasfOld);
                    ViewBag.Dipendente = d;
                    matricola = d.matricola;

                    ListeComboNuovoTrasf(out lTipoTrasferimento, out lUffici, out lRuoloUfficio, out lTipologiaCoan, out lFasciaKM);

                    FiltraRuoloUfficio(out lRuoloUfficio, d);
                }



                ViewBag.ListTipoTrasferimento = lTipoTrasferimento;
                ViewBag.ListUfficio = lUffici;
                ViewBag.ListRuolo = lRuoloUfficio;
                ViewBag.ListTipoCoan = lTipologiaCoan;
                ViewBag.ListFasciaKM = lFasciaKM;

                ViewBag.ricaricaInfoTrasf = ricaricaInfoTrasf;
                ViewBag.Matricola = matricola;

                ViewBag.idTrasferimentoOld = idTrasfOld;

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tOld = dtt.GetSoloTrasferimentoById(idTrasfOld);

                    if (tOld?.idTrasferimento > 0)
                    {
                        if (tOld.idStatoTrasferimento == EnumStatoTraferimento.Terminato)
                        {
                            using (dtRichiamo dtr = new dtRichiamo())
                            {
                                var rm = dtr.GetRichiamoByIdTrasf(tOld.idTrasferimento);

                                if (rm?.HasValue() ?? false)
                                {
                                    //ViewBag.ListTipoTrasferimento =
                                    //    lTipoTrasferimento.Where(
                                    //        a =>
                                    //            a.Value == "" ||
                                    //            a.Value ==
                                    //            Convert.ToDecimal(EnumTipoTrasferimento.SedeEstero).ToString());

                                    ViewBag.ListUfficio = lUffici;
                                }
                                else
                                {
                                    ViewBag.ListTipoTrasferimento =
                                        lTipoTrasferimento.Where(
                                            a =>
                                                a.Value == "" ||
                                                a.Value ==
                                                Convert.ToDecimal(EnumTipoTrasferimento.EsteroEstero).ToString() ||
                                                a.Value ==
                                                Convert.ToDecimal(EnumTipoTrasferimento.EsteroEsteroStessaRegiona)
                                                    .ToString());

                                    ViewBag.ListUfficio = lUffici.Where(a => a.Value != tOld.idUfficio.ToString());
                                }

                            }
                        }
                        else if (tOld.idStatoTrasferimento == EnumStatoTraferimento.Attivo)
                        {
                            ViewBag.ListTipoTrasferimento =
                                lTipoTrasferimento.Where(
                                    a =>
                                        a.Value == "" ||
                                        a.Value == Convert.ToDecimal(EnumTipoTrasferimento.EsteroEstero).ToString() ||
                                        a.Value ==
                                        Convert.ToDecimal(EnumTipoTrasferimento.EsteroEsteroStessaRegiona).ToString());

                            ViewBag.ListUfficio = lUffici.Where(a => a.Value != tOld.idUfficio.ToString());
                        }
                        else if (tOld.idStatoTrasferimento == EnumStatoTraferimento.Annullato)
                        {

                            var tOld_Old = dtt.GetTrasferimentoOldPrimaDiTrasfAnnullato(tOld.idTrasferimento);

                            if (tOld_Old?.HasValue() ?? false)
                            {
                                using (dtRichiamo dtr = new dtRichiamo())
                                {
                                    var rm = dtr.GetRichiamoByIdTrasf(tOld_Old.idTrasferimento);

                                    if (rm?.HasValue() ?? false)
                                    {
                                        //ViewBag.ListTipoTrasferimento =
                                        //lTipoTrasferimento.Where(
                                        //    a =>
                                        //        a.Value == "" ||
                                        //        a.Value ==
                                        //        Convert.ToDecimal(EnumTipoTrasferimento.SedeEstero).ToString());

                                        ViewBag.ListUfficio = lUffici;
                                    }
                                    else
                                    {
                                        ViewBag.ListTipoTrasferimento =
                                            lTipoTrasferimento.Where(
                                                a =>
                                                    a.Value == "" ||
                                                    a.Value ==
                                                    Convert.ToDecimal(EnumTipoTrasferimento.EsteroEstero).ToString() ||
                                                    a.Value ==
                                                    Convert.ToDecimal(EnumTipoTrasferimento.EsteroEsteroStessaRegiona)
                                                        .ToString());

                                        ViewBag.ListUfficio = lUffici.Where(a => a.Value != tOld_Old.idUfficio.ToString());
                                    }
                                }
                            }
                            else
                            {
                                //ViewBag.ListTipoTrasferimento = lTipoTrasferimento.Where(a => a.Value == "" || a.Value == Convert.ToDecimal(EnumTipoTrasferimento.SedeEstero).ToString());
                                ViewBag.ListUfficio = lUffici;
                            }

                        }
                        else
                        {
                            return PartialView("ErrorPartial", new MsgErr() { msg = "Impossibile inserire un nuovo trasferimento con stato del trasferimento ha: " + tOld.idStatoTrasferimento.ToString() });
                        }
                        return PartialView("NuovoTrasferimento");
                    }
                    else
                    {
                        return PartialView("ErrorPartial", new MsgErr() { msg = "Trasferimento inesistente." });
                    }

                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

        }

        [Authorize(Roles = "1 ,2")]
        public ActionResult NuovoTrasferimento(int matricola, decimal idTrasferimento = 0, bool ricaricaInfoTrasf = false, bool ricaricaTrasferimenti = false, decimal idRuoloDipendente = 0)
        {
            var lTipoTrasferimento = new List<SelectListItem>();
            var lUffici = new List<SelectListItem>();
            var lRuoloUfficio = new List<SelectListItem>();
            var lTipologiaCoan = new List<SelectListItem>();
            var lFasciaKM = new List<SelectListItem>();

            try
            {
                using (dtDipendenti dtd = new dtDipendenti())
                {
                    var d = dtd.GetDipendenteByMatricola(matricola);
                    ViewBag.Dipendente = d;
                    matricola = d.matricola;

                    ListeComboNuovoTrasf(out lTipoTrasferimento, out lUffici, out lRuoloUfficio, out lTipologiaCoan, out lFasciaKM);

                    FiltraRuoloUfficio(out lRuoloUfficio, d);

                    ViewBag.ListTipoTrasferimento = lTipoTrasferimento;
                    ViewBag.ListUfficio = lUffici;
                    ViewBag.ListRuolo = lRuoloUfficio;
                    ViewBag.ListTipoCoan = lTipologiaCoan;
                    ViewBag.ListFasciaKM = lFasciaKM;

                    ViewBag.ricaricaInfoTrasf = ricaricaInfoTrasf;
                    ViewBag.Matricola = matricola;

                    if (idTrasferimento > 0)
                    {

                        if (idRuoloDipendente <= 0)
                        {
                            using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                            {
                                var rdm = dtrd.GetRuoloDipendentePartenza(idTrasferimento);
                                idRuoloDipendente = rdm.idRuoloDipendente;
                            }
                        }

                        return RedirectToAction("ModificaTrasferimento", new { idTrasferimento = idTrasferimento, matricola = matricola, idRuoloDipendente = idRuoloDipendente, ricaricaInfoTrasf = ricaricaInfoTrasf, ricaricaTrasferimenti = ricaricaTrasferimenti });
                    }
                    else
                    {
                        //ViewBag.ListTipoTrasferimento = lTipoTrasferimento.Where(a => a.Value == "" || a.Value == Convert.ToDecimal(EnumTipoTrasferimento.SedeEstero).ToString());
                        return PartialView();
                    }
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public JsonResult LetturaParametriGestioneTrasferimento(decimal idTrasferimento)
        {
            string errore = string.Empty;
            bool abilitaNotifica = false;
            bool abilitaElimina = false;
            bool abilitaNuovoTrasferimento = false;
            bool abilitaSalva = false;
            bool trasferimentoSuccessivo = false;

            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tm = dtt.GetTrasferimentoById(idTrasferimento);
                    using (dtRuoloUfficio dtru = new dtRuoloUfficio())
                    {
                        tm.RuoloUfficio = dtru.GetRuoloUfficioValidoByIdTrasferimento(tm.idTrasferimento);
                        tm.idRuoloUfficio = tm.RuoloUfficio.idRuoloUfficio;
                    }

                    using (dtDocumenti dtd = new dtDocumenti())
                    {
                        DocumentiModel dm = new DocumentiModel();

                        dm = dtd.GetDocumentoByIdTrasferimento(tm.idTrasferimento);
                        if (dm != null && dm.file != null)
                        {
                            tm.idDocumento = dm.idDocumenti;
                            tm.file = dm.file;
                            tm.Documento = dm;
                        }
                    }

                    trasferimentoSuccessivo = dtt.EsisteTrasferimentoSuccessivo(idTrasferimento);

                    if (tm.idStatoTrasferimento == EnumStatoTraferimento.Attivo || tm.idStatoTrasferimento == EnumStatoTraferimento.Terminato || tm.idStatoTrasferimento == EnumStatoTraferimento.Annullato)
                    {
                        abilitaNuovoTrasferimento = true;
                        abilitaSalva = false;
                        abilitaElimina = false;
                        abilitaNotifica = false;
                    }
                    else if (tm.notificaTrasferimento == false)
                    {
                        abilitaNuovoTrasferimento = false;
                        abilitaSalva = true;
                        abilitaElimina = true;

                        if (tm.idTipoTrasferimento > 0 &&
                            tm.idUfficio > 0 &&
                            tm.idStatoTrasferimento > 0 &&
                            tm.idDipendente > 0 &&
                            tm.idTipoCoan > 0 &&
                            tm.dataPartenza > DateTime.MinValue &&
                            tm.idRuoloUfficio > 0 &&
                            tm.protocolloLettera != string.Empty &&
                            tm.dataLettera > DateTime.MinValue &&
                            tm.Documento != null &&
                            tm.Documento.idDocumenti > 0)
                        {
                            abilitaNotifica = true;
                        }
                        else
                        {
                            abilitaNotifica = false;
                        }
                    }
                    else if (tm.notificaTrasferimento == true)
                    {
                        abilitaSalva = false;
                        abilitaElimina = false;
                        abilitaNotifica = false;
                        abilitaNuovoTrasferimento = false;
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
                        abilitaElimina = abilitaElimina,
                        abilitaNotifica = abilitaNotifica,
                        abilitaNuovoTrasferimento = abilitaNuovoTrasferimento,
                        abilitaSalva = abilitaSalva,
                        trasferimentoSuccessivo = trasferimentoSuccessivo

                    });


        }


        [Authorize(Roles = "1 ,2")]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ModificaTrasferimento(decimal idTrasferimento, int matricola, decimal idRuoloDipendente, bool ricaricaInfoTrasf = false, bool ricaricaTrasferimenti = false)
        {
            var lTipoTrasferimento = new List<SelectListItem>();
            var lUffici = new List<SelectListItem>();
            var lRuoloUfficio = new List<SelectListItem>();
            var lTipologiaCoan = new List<SelectListItem>();
            var lFasciaKM = new List<SelectListItem>();

            bool trasfSuccessivo = false;

            try
            {

                ListeComboNuovoTrasf(out lTipoTrasferimento, out lUffici, out lRuoloUfficio, out lTipologiaCoan, out lFasciaKM);

                using (dtDipendenti dtd = new dtDipendenti())
                {
                    var d = dtd.GetDipendenteByMatricola(matricola);

                    FiltraRuoloUfficio(out lRuoloUfficio, d);
                }

                ViewBag.ListTipoTrasferimento = lTipoTrasferimento;
                ViewBag.ListUfficio = lUffici;
                ViewBag.ListRuolo = lRuoloUfficio;
                ViewBag.ListTipoCoan = lTipologiaCoan;
                ViewBag.ListFasciaKM = lFasciaKM;

                ViewBag.ricaricaInfoTrasf = ricaricaInfoTrasf;
                ViewBag.Matricola = matricola;

                ViewData.Add("ricaricaTrasferimenti", ricaricaTrasferimenti);


                using (dtTrasferimento dttr = new dtTrasferimento())
                {
                    var trm = dttr.GetTrasferimentoById(idTrasferimento);

                    trasfSuccessivo = dttr.EsisteTrasferimentoSuccessivo(idTrasferimento);
                    ViewData.Add("TrasfSucc", trasfSuccessivo);


                    using (dtDipendenti dtd = new dtDipendenti())
                    {
                        var d = dtd.GetDipendenteByID(trm.idDipendente);
                        ViewBag.Dipendente = d;
                    }

                    switch ((EnumStatoTraferimento)trm.StatoTrasferimento)
                    {
                        case EnumStatoTraferimento.Attivo:

                            ViewBag.ListTipoTrasferimento = lTipoTrasferimento;
                            ViewBag.ListUfficio = lUffici;
                            using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                            {
                                var rdm = dtrd.GetRuoloDipendenteById(idRuoloDipendente);
                                ViewBag.idRuoloDipendente = rdm.idRuoloDipendente;
                                trm.RuoloUfficio = rdm.RuoloUfficio;
                                trm.idRuoloUfficio = rdm.RuoloUfficio.idRuoloUfficio;
                            }

                            using (dtFasciaKm dtfkm = new dtFasciaKm())
                            {
                                var fkm = dtfkm.GetFasciaKmByTrasf(trm.idTrasferimento, trm.dataPartenza);
                                trm.idFKM = fkm.idFKM;
                                ViewBag.idFKM = fkm.idFKM;
                            }

                            using (dtDocumenti dtd = new dtDocumenti())
                            {
                                DocumentiModel dm = new DocumentiModel();

                                dm = dtd.GetDocumentoByIdTrasferimento(trm.idTrasferimento);
                                if (dm != null && dm.file != null)
                                {
                                    trm.idDocumento = dm.idDocumenti;
                                    trm.file = dm.file;
                                    trm.Documento = dm;
                                }
                            }

                            return PartialView(trm);

                        case EnumStatoTraferimento.Da_Attivare:
                            ViewBag.ListTipoTrasferimento = lTipoTrasferimento.Where(a => a.Value == trm.idTipoTrasferimento.ToString());
                            ViewBag.ListUfficio = lUffici.Where(a => a.Value == trm.idUfficio.ToString());

                            using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                            {
                                var rdm = dtrd.GetRuoloDipendenteById(idRuoloDipendente);
                                ViewBag.idRuoloDipendente = rdm.idRuoloDipendente;
                                trm.RuoloUfficio = rdm.RuoloUfficio;
                                trm.idRuoloUfficio = rdm.RuoloUfficio.idRuoloUfficio;
                            }

                            ViewBag.ListRuolo = lRuoloUfficio.Where(a => a.Value == trm.idRuoloUfficio.ToString());

                            using (dtFasciaKm dtfkm = new dtFasciaKm())
                            {

                                var fkm = dtfkm.GetFasciaKmByTrasf(trm.idTrasferimento, trm.dataPartenza);

                                trm.idFKM = fkm.idFKM;
                                ViewBag.idFKM = fkm.idFKM;

                            }

                            ViewBag.ListFasciaKM = lFasciaKM.Where(a => a.Value == trm.idFKM.ToString());

                            using (dtDocumenti dtd = new dtDocumenti())
                            {
                                DocumentiModel dm = new DocumentiModel();

                                dm = dtd.GetDocumentoByIdTrasferimento(trm.idTrasferimento);
                                if (dm != null && dm.file != null)
                                {
                                    trm.idDocumento = dm.idDocumenti;
                                    trm.file = dm.file;
                                    trm.Documento = dm;
                                }
                            }

                            return PartialView(trm);

                        case EnumStatoTraferimento.Non_Trasferito:
                            trm.Ufficio = new UfficiModel();
                            trm.RuoloUfficio = new RuoloUfficioModel();

                            //ViewBag.ListTipoTrasferimento = lTipoTrasferimento.Where(a => a.Value == "" || a.Value == ((decimal)EnumTipoTrasferimento.SedeEstero).ToString());

                            return PartialView();

                        case EnumStatoTraferimento.Terminato:
                        case EnumStatoTraferimento.Annullato:
                            //ViewBag.ListTipoTrasferimento = lTipoTrasferimento.Where(a => a.Value == "" || a.Value == ((decimal)EnumTipoTrasferimento.SedeEstero).ToString());
                            using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                            {
                                var rdm = dtrd.GetRuoloDipendenteById(idRuoloDipendente);
                                ViewBag.idRuoloDipendente = rdm.idRuoloDipendente;
                                trm.RuoloUfficio = rdm.RuoloUfficio;
                                trm.idRuoloUfficio = rdm.RuoloUfficio.idRuoloUfficio;
                            }
                            using (dtFasciaKm dtfkm = new dtFasciaKm())
                            {

                                var fkm = dtfkm.GetFasciaKmByTrasf(trm.idTrasferimento, trm.dataPartenza);

                                trm.idFKM = fkm.idFKM;
                                ViewBag.idFKM = fkm.idFKM;

                            }
                            using (dtDocumenti dtd = new dtDocumenti())
                            {
                                DocumentiModel dm = new DocumentiModel();

                                dm = dtd.GetDocumentoByIdTrasferimento(trm.idTrasferimento);
                                if (dm != null && dm.file != null)
                                {
                                    trm.idDocumento = dm.idDocumenti;
                                    trm.file = dm.file;
                                    trm.Documento = dm;
                                }
                            }
                            return PartialView(trm);


                        default:

                            throw new Exception("Stato trasferimento sconosciuto.");
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }


        }

        public ActionResult ConfermaModificaTrasferimento(TrasferimentoModel trm, int matricola, decimal idRuoloDipendente, decimal idFasciaKM, bool ricaricaInfoTrasf = false)
        {
            try
            {
                trm.dataAggiornamento = DateTime.Now;
                if (ModelState.IsValid)
                {

                    using (dtTrasferimento dttr = new dtTrasferimento())
                    {
                        using (ModelDBISE db = new ModelDBISE())
                        {
                            try
                            {
                                db.Database.BeginTransaction();

                                TRASFERIMENTO tr = dttr.UpdateTrasferimento(trm, db);

                                using (dtIndennita dti = new dtIndennita())
                                {
                                    IndennitaModel im = dti.GetIndennitaByIdTrasferimento(trm.idTrasferimento, db);


                                    im.dataAggiornamento = DateTime.Now;

                                    dti.EditIndennita(im, db);

                                    DateTime dataRientro = trm.dataRientro.HasValue == true
                                            ? trm.dataRientro.Value
                                            : Utility.DataFineStop();

                                    #region commento indennita
                                    //using (dtLivelliDipendente dtld = new dtLivelliDipendente())
                                    //{

                                    //dtld.RimuoviAssociazioneLivelloDipendente_Indennita(trm.idTrasferimento, trm.dataPartenza, db);

                                    //LivelloDipendenteModel ldm = dtld.GetLivelloDipendente(trm.idDipendente, trm.dataPartenza, db);
                                    //if (ldm.HasValue())
                                    //{
                                    //    dtld.AssociaLivelloDipendente_Indennita(trm.idTrasferimento, ldm.idLivDipendente, db);
                                    //}
                                    //else
                                    //{
                                    //    throw new Exception("Non risulta assegnato nessun livello per il dipendente " + trm.Dipendente.Nominativo + " (" + trm.Dipendente.matricola + ")");
                                    //}

                                    //using (dtIndennitaBase dtib = new dtIndennitaBase())
                                    //{
                                    //    dtib.RimuoviAssociazioneIndennitaBase_Indennita(trm.idTrasferimento, trm.dataPartenza, db);

                                    //    IndennitaBaseModel ibm = new IndennitaBaseModel();
                                    //    ibm = dtib.GetIndennitaBaseValida(ldm.idLivello, trm.dataPartenza, db);
                                    //    if (ibm.HasValue())
                                    //    {
                                    //        dtib.AssociaIndennitaBase_Indennita(trm.idTrasferimento, ibm.idIndennitaBase, db);
                                    //    }
                                    //    else
                                    //    {
                                    //        throw new Exception("Non risulta l'indennità base per il livello interessato.");
                                    //    }
                                    //}

                                    #endregion


                                    using (dtLivelliDipendente dtld = new dtLivelliDipendente())
                                    {
                                        dtld.RimuoviAssociazioneLivelliDipendente_Indennita(trm.idTrasferimento, trm.dataPartenza, dataRientro, db);

                                        var lldm =
                                                dtld.GetLivelliDipendentiByRangeDate(trm.idDipendente, trm.dataPartenza,
                                                    dataRientro, db).ToList();
                                        if (lldm?.Any() ?? false)
                                        {
                                            foreach (var ldm in lldm)
                                            {
                                                dtld.AssociaLivelloDipendente_Indennita(trm.idTrasferimento,
                                                    ldm.idLivDipendente, db);

                                                using (dtIndennitaBase dtib = new dtIndennitaBase())
                                                {
                                                    List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

                                                    DateTime dataInizio = Utility.GetData_Inizio_Base();
                                                    DateTime dataFine = Utility.DataFineStop();

                                                    if (trm.dataPartenza > ldm.dataInizioValdita)
                                                    {
                                                        dataInizio = trm.dataPartenza;
                                                    }
                                                    else
                                                    {
                                                        dataInizio = trm.dataPartenza;
                                                    }

                                                    if (trm.dataRientro.HasValue)
                                                    {
                                                        if (trm.dataRientro > ldm.dataFineValidita)
                                                        {
                                                            dataFine = ldm.dataFineValidita.HasValue == true
                                                                ? ldm.dataFineValidita.Value
                                                                : Utility.DataFineStop();
                                                        }
                                                        else
                                                        {
                                                            dataFine = trm.dataRientro.Value;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dataFine = ldm.dataFineValidita.HasValue == true
                                                            ? ldm.dataFineValidita.Value
                                                            : Utility.DataFineStop();
                                                    }


                                                    dtib.RimuoviAssciazioniIndennitaBase_Indennita(trm.idTrasferimento, trm.dataPartenza, dataRientro, db);

                                                    libm =
                                                        dtib.GetIndennitaBaseByRangeDate(ldm.idLivello, dataInizio,
                                                            dataFine, db).ToList();

                                                    if (libm?.Any() ?? false)
                                                    {
                                                        foreach (var ibm in libm)
                                                        {
                                                            dtib.AssociaIndennitaBase_Indennita(trm.idTrasferimento, ibm.idIndennitaBase, db);
                                                        }

                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Non risulta l'indennità base per il livello interessato.");
                                                    }
                                                }

                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Non risulta assegnato nessun livello per il dipendente " + trm.Dipendente.Nominativo + " (" + trm.Dipendente.matricola + ")");
                                        }
                                    }





                                    #region commento TFR                                  
                                    //using (dtTFR dttfr = new dtTFR())
                                    //{
                                    //    dttfr.RimuoviAssociaTFR_Indennita(trm.idTrasferimento, trm.dataPartenza, db);

                                    //    TFRModel tfrm = dttfr.GetTFRValido(trm.idUfficio, trm.dataPartenza, db);
                                    //    if (tfrm.HasValue())
                                    //    {
                                    //        dttfr.AssociaTFR_Indennita(trm.idTrasferimento, tfrm.idTFR, db);
                                    //    }
                                    //    else
                                    //    {
                                    //        throw new Exception("Non risulta il tasso fisso di ragguaglio per l'ufficio interessato.");
                                    //    }
                                    //}
                                    #endregion

                                    using (dtTFR dttfr = new dtTFR())
                                    {
                                        dttfr.RimuoviAsscoiazioniTFR_Indennita(trm.idTrasferimento, trm.dataPartenza, dataRientro, db);

                                        List<TFRModel> ltfrm =
                                            dttfr.GetTfrIndennitaByRangeDate(tr.IDUFFICIO, trm.dataPartenza,
                                                dataRientro, db).ToList();

                                        if (ltfrm?.Any() ?? false)
                                        {
                                            foreach (var tfrm in ltfrm)
                                            {
                                                dttfr.AssociaTFR_Indennita(trm.idTrasferimento, tfrm.idTFR, db);
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Non risulta il tasso fisso di ragguaglio per l'ufficio interessato.");
                                        }

                                    }

                                    #region commento perc disagio
                                    //using (dtPercentualeDisagio dtpd = new dtPercentualeDisagio())
                                    //{
                                    //    dtpd.RimuoviAssociaPercentualeDisagio_Indennita(trm.idTrasferimento, trm.dataPartenza, db);

                                    //    PercentualeDisagioModel pdm = dtpd.GetPercentualeDisagioValida(trm.idUfficio, trm.dataPartenza, db);

                                    //    if (pdm.HasValue())
                                    //    {
                                    //        dtpd.AssociaPercentualeDisagio_Indennita(trm.idTrasferimento, pdm.idPercentualeDisagio, db);
                                    //    }
                                    //    else
                                    //    {
                                    //        throw new Exception("Non risulta la percentuale di disagio per l'ufficio interessato.");
                                    //    }
                                    //}

                                    #endregion

                                    using (dtPercentualeDisagio dtpd = new dtPercentualeDisagio())
                                    {
                                        dtpd.RimuoviAssociazioniPercentualeDisagio_Indennita(trm.idTrasferimento, trm.dataPartenza, dataRientro, db);

                                        List<PercentualeDisagioModel> lpdm =
                                            dtpd.GetPercentualeDisagioIndennitaByRange(tr.IDUFFICIO,
                                                trm.dataPartenza, dataRientro, db).ToList();


                                        if (lpdm?.Any() ?? false)
                                        {
                                            foreach (var pdm in lpdm)
                                            {
                                                dtpd.AssociaPercentualeDisagio_Indennita(trm.idTrasferimento, pdm.idPercentualeDisagio, db);
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Non risulta la percentuale di disagio per l'ufficio interessato.");
                                        }


                                    }


                                    #region commento perc sede
                                    //using (dtCoefficenteSede dtcs = new dtCoefficenteSede())
                                    //{
                                    //    dtcs.RimuoviAssociaCoefficenteSede_Indennita(trm.idTrasferimento, trm.dataPartenza, db);

                                    //    CoefficientiSedeModel cs = dtcs.GetCoefficenteSedeValido(trm.idUfficio, trm.dataPartenza, db);
                                    //    if (cs.HasValue())
                                    //    {
                                    //        dtcs.AssociaCoefficenteSede_Indennita(trm.idTrasferimento, cs.idCoefficientiSede, db);
                                    //    }
                                    //    else
                                    //    {
                                    //        throw new Exception("Non risulta il valore di coefficente di sede per l'ufficio interessato.");
                                    //    }
                                    //} 
                                    #endregion

                                    using (dtCoefficenteSede dtcs = new dtCoefficenteSede())
                                    {
                                        dtcs.RimuoviCoefficientiSede_Indennita(trm.idTrasferimento, trm.dataPartenza, dataRientro, db);

                                        List<CoefficientiSedeModel> lcsm =
                                            dtcs.GetCoefficenteSedeIndennitaByRangeDate(tr.IDUFFICIO,
                                                trm.dataPartenza, dataRientro, db).ToList();

                                        if (lcsm?.Any() ?? false)
                                        {
                                            foreach (var csm in lcsm)
                                            {
                                                dtcs.AssociaCoefficenteSede_Indennita(trm.idTrasferimento, csm.idCoefficientiSede, db);
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Non risulta il valore di coefficiente di sede per l'ufficio interessato.");
                                        }

                                    }

                                    #region commento ruolo
                                    //using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                                    //{

                                    //    RuoloDipendenteModel rdm = dtrd.GetRuoloDipendente(trm.idTrasferimento, trm.idRuoloUfficio, trm.dataPartenza, db);

                                    //    if (rdm == null || rdm.hasValue() == false)
                                    //    {
                                    //        rdm = new RuoloDipendenteModel()
                                    //        {
                                    //            idRuolo = trm.idRuoloUfficio,
                                    //            dataInizioValidita = trm.dataPartenza,
                                    //            dataFineValidita = Utility.DataFineStop(),
                                    //            dataAggiornamento = DateTime.Now,
                                    //            annullato = false
                                    //        };

                                    //        var rdnOld =
                                    //            dtrd.GetRuoloDipendenteByIdTrasferimento(trm.idTrasferimento,
                                    //                trm.dataPartenza, db);



                                    //        dtrd.SetRuoloDipendente(ref rdm, db);




                                    //        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo ruolo dipendete.", "RuoloDipendente", db, trm.idTrasferimento, rdm.idRuoloDipendente);
                                    //    }
                                    //    else
                                    //    {
                                    //        dtrd.SetNuovoRuoloDipendente(ref rdm, db);
                                    //    }

                                    //}


                                    //-------------------------------------------------------
                                    //using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                                    //{
                                    //    RuoloDipendenteModel rdm = dtrd.GetRuoloDipendente(trm.idTrasferimento, trm.idRuoloUfficio, trm.dataPartenza, db);

                                    //    if (rdm == null || rdm.hasValue() == false)
                                    //    {
                                    //        rdm = new RuoloDipendenteModel()
                                    //        {
                                    //            idRuolo = trm.idRuoloUfficio,
                                    //            idTrasferimento = trm.idTrasferimento,
                                    //            dataInizioValidita = trm.dataPartenza,
                                    //            dataFineValidita = Utility.DataFineStop(),
                                    //            dataAggiornamento = DateTime.Now,
                                    //            annullato = false
                                    //        };

                                    //        dtrd.SetRuoloDipendente(ref rdm, db);

                                    //    }
                                    //    else
                                    //    {
                                    //        dtrd.SetNuovoRuoloDipendente(ref rdm, db);
                                    //    }

                                    //}
                                    //-------------------------------------------------
                                    #endregion

                                    using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                                    {
                                        RuoloDipendenteModel rdm = dtrd.GetRuoloDipendenteById(idRuoloDipendente);

                                        if (rdm != null && rdm.idRuoloDipendente > 0)
                                        {
                                            dtrd.AggiornaRuoloDipendentePartenza(ref rdm, trm, db);
                                        }



                                    }
                                    //-------------------------------------------------
                                    using (dtFasciaKm dtfkm = new dtFasciaKm())
                                    {
                                        dtfkm.RimuoviAssociazionePercentualeFKM(trm.idTrasferimento, db);

                                        using (dtPrimaSistemazione dtps = new dtPrimaSistemazione())
                                        {
                                            var pfkmm = dtfkm.GetPercentualeFKM(idFasciaKM, trm.dataPartenza, db);
                                            var psm = dtps.GetPrimaSistemazioneBtIdTrasf(trm.idTrasferimento, db);

                                            if (pfkmm?.idPFKM > 0)
                                            {
                                                dtfkm.AssociaPercentualeFKMPrimaSistemazione(psm.idPrimaSistemazione, pfkmm.idPFKM, db);
                                            }
                                            else
                                            {
                                                throw new Exception("Non risulta il valore della percentuale fascia chilometrica.");
                                            }
                                        }

                                    }


                                    // }

                                }

                                db.Database.CurrentTransaction.Commit();

                                ricaricaInfoTrasf = true;

                                return RedirectToAction("ModificaTrasferimento", new { idTrasferimento = trm.idTrasferimento, matricola = matricola, idRuoloDipendente = idRuoloDipendente, ricaricaInfoTrasf = ricaricaInfoTrasf });

                            }
                            catch (Exception ex)
                            {

                                db.Database.CurrentTransaction.Rollback();
                                return PartialView("ErrorPartial", new HandleErrorInfo(ex, "Trasferimento", "ConfermaModificaTrasferimento"));
                            }
                        }
                    }

                }
                else
                {
                    var lTipoTrasferimento = new List<SelectListItem>();
                    var lUffici = new List<SelectListItem>();
                    var lRuoloUfficio = new List<SelectListItem>();
                    var lTipologiaCoan = new List<SelectListItem>();
                    var lFasciaKM = new List<SelectListItem>();

                    bool trasfSuccessivo = false;

                    try
                    {
                        ListeComboNuovoTrasf(out lTipoTrasferimento, out lUffici, out lRuoloUfficio, out lTipologiaCoan, out lFasciaKM);

                        ViewBag.ListTipoTrasferimento = lTipoTrasferimento;
                        ViewBag.ListUfficio = lUffici;
                        ViewBag.ListRuolo = lRuoloUfficio;
                        ViewBag.ListTipoCoan = lTipologiaCoan;
                        ViewBag.ListFasciaKM = lFasciaKM;

                        ViewBag.ricaricaInfoTrasf = ricaricaInfoTrasf;
                        ViewBag.Matricola = matricola;

                        using (dtTrasferimento dttr = new dtTrasferimento())
                        {
                            trasfSuccessivo = dttr.EsisteTrasferimentoSuccessivo(trm.idTrasferimento);
                            ViewData.Add("TrasfSucc", trasfSuccessivo);
                        }

                        using (dtDipendenti dtd = new dtDipendenti())
                        {
                            var d = dtd.GetDipendenteByID(trm.idDipendente);
                            ViewBag.Dipendente = d;
                        }

                        return PartialView("ModificaTrasferimento", trm);
                    }
                    catch (Exception ex)
                    {
                        return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
                    }
                }
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }


        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciTrasferimento(TrasferimentoModel trm, int matricola, bool ricaricaInfoTrasf = false, decimal idTrasferimentoOld = 0)
        {
            bool ricaricaTrasferimenti = true;
            RuoloDipendenteModel rdm = new RuoloDipendenteModel();
            try
            {
                //trm.idTrasferimento = 0;
                trm.idStatoTrasferimento = EnumStatoTraferimento.Da_Attivare;
                trm.dataAggiornamento = DateTime.Now;

                if (idTrasferimentoOld > 0)
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        bool ret = false;
                        string matr = matricola.ToString();
                        var t = dtt.GetTrasferimentoById(idTrasferimentoOld);
                        if(t.idStatoTrasferimento==EnumStatoTraferimento.Annullato)
                        {
                            //var trasfTerminato = dtt.GetUltimoTrasferimentoTerminatoByMatricola(matr);
                            var trasfPrercedente = dtt.GetUltimoTrasferimentoValidoByMatricola(matr);
                            if(trasfPrercedente.idTrasferimento>0)
                            {
                                idTrasferimentoOld = trasfPrercedente.idTrasferimento;
                            }
                        }
                        
                        ret = dtt.VerificaDataInizioTrasferimentoNew(idTrasferimentoOld, trm.dataPartenza);

                        if (ret)
                        {
                            ModelState.AddModelError("", "Impossibile inserire un nuovo trasferimento che abbia la data di partenza inferiore e/o uguale alla data di partenza oppure minore della data rientro del trasferimento precedente.");
                        }

                    }
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        using (dtTrasferimento dttr = new dtTrasferimento())
                        {
                            using (ModelDBISE db = new ModelDBISE())
                            {
                                try
                                {
                                    db.Database.BeginTransaction();

                                    #region trasferimento
                                    dttr.SetTrasferimento(ref trm, db);

                                    using (dtDipendenti dtd = new dtDipendenti())
                                    {
                                        trm.Dipendente = dtd.GetDipendenteByIDTrasf(trm.idTrasferimento, db);
                                    }

                                    using (dtUtenteAutorizzato dtua = new dtUtenteAutorizzato())
                                    {
                                        dtua.SetAutorizzaUtenteTrasferito(trm.idDipendente, db);
                                    }

                                    if (idTrasferimentoOld > 0)
                                    {
                                        dttr.TerminaTrasferimento(idTrasferimentoOld, trm.dataPartenza, db);
                                    }
                                    #endregion

                                    #region prima sistemazione
                                    using (dtPrimaSistemazione dtps = new dtPrimaSistemazione())
                                    {
                                        PrimaSistemazioneModel psm = new PrimaSistemazioneModel()
                                        {
                                            idPrimaSistemazione = trm.idTrasferimento,
                                            dataOperazione = DateTime.Now
                                        };

                                        dtps.InserisciPrimaSistemazione(psm, db);
                                    }
                                    #endregion

                                    #region maggiorazioni familiari
                                    using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                                    {
                                        dtmf.PreSetMaggiorazioniFamiliari(trm.idTrasferimento, db);
                                    }
                                    #endregion

                                    #region passaporto (fasi richiesta)
                                    using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                                    {
                                        dtpp.PreSetPassaporto(trm.idTrasferimento, (decimal)EnumFasePassaporti.Richiesta_Passaporti, db);
                                    }
                                    #endregion

                                    #region titoli viaggio
                                    using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                                    {
                                        dttv.PreSetTitoloViaggio(trm.idTrasferimento, db);
                                    }
                                    #endregion

                                    #region trasporto effetti
                                    using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                                    {
                                        dtte.PreSetTrasportoEffetti(trm.idTrasferimento, db);
                                    }
                                    #endregion

                                    #region indennita
                                    using (dtIndennita dti = new dtIndennita())
                                    {
                                        IndennitaModel im = new IndennitaModel();
                                        List<LivelloDipendenteModel> lldm = new List<LivelloDipendenteModel>();

                                        im.idTrasfIndennita = trm.idTrasferimento;
                                        im.dataAggiornamento = DateTime.Now;

                                        dti.SetIndennita(im, db);

                                        DateTime dataRientro = trm.dataRientro.HasValue == true
                                            ? trm.dataRientro.Value
                                            : Utility.DataFineStop();

                                        using (dtLivelliDipendente dtld = new dtLivelliDipendente())
                                        {
                                            lldm =
                                                dtld.GetLivelliDipendentiByRangeDate(trm.idDipendente, trm.dataPartenza,
                                                    dataRientro, db).ToList();
                                            if (lldm?.Any() ?? false)
                                            {
                                                foreach (var ldm in lldm)
                                                {
                                                    dtld.AssociaLivelloDipendente_Indennita(trm.idTrasferimento,
                                                        ldm.idLivDipendente, db);

                                                    using (dtIndennitaBase dtib = new dtIndennitaBase())
                                                    {
                                                        List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

                                                        DateTime dataInizio = Utility.GetData_Inizio_Base();
                                                        DateTime dataFine = Utility.DataFineStop();

                                                        if (trm.dataPartenza > ldm.dataInizioValdita)
                                                        {
                                                            dataInizio = trm.dataPartenza;
                                                        }
                                                        else
                                                        {
                                                            dataInizio = trm.dataPartenza;
                                                        }

                                                        if (trm.dataRientro.HasValue)
                                                        {
                                                            if (trm.dataRientro > ldm.dataFineValidita)
                                                            {
                                                                dataFine = ldm.dataFineValidita.HasValue == true
                                                                    ? ldm.dataFineValidita.Value
                                                                    : Utility.DataFineStop();
                                                            }
                                                            else
                                                            {
                                                                dataFine = trm.dataRientro.Value;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            dataFine = ldm.dataFineValidita.HasValue == true
                                                                ? ldm.dataFineValidita.Value
                                                                : Utility.DataFineStop();
                                                        }

                                                        libm =
                                                            dtib.GetIndennitaBaseByRangeDate(ldm.idLivello, dataInizio,
                                                                dataFine, db).ToList();

                                                        if (libm?.Any() ?? false)
                                                        {
                                                            foreach (var ibm in libm)
                                                            {
                                                                dtib.AssociaIndennitaBase_Indennita(trm.idTrasferimento, ibm.idIndennitaBase, db);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            throw new Exception("Non risulta l'indennità base per il livello interessato.");
                                                        }
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                throw new Exception("Non risulta assegnato nessun livello per il dipendente " + trm.Dipendente.Nominativo + " (" + trm.Dipendente.matricola + ")");
                                            }
                                        }



                                        using (dtTFR dttfr = new dtTFR())
                                        {
                                            List<TFRModel> ltfrm =
                                                dttfr.GetTfrIndennitaByRangeDate(trm.idUfficio, trm.dataPartenza,
                                                    dataRientro, db).ToList();

                                            if (ltfrm?.Any() ?? false)
                                            {
                                                foreach (var tfrm in ltfrm)
                                                {
                                                    dttfr.AssociaTFR_Indennita(trm.idTrasferimento, tfrm.idTFR, db);
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception("Non risulta il tasso fisso di ragguaglio per l'ufficio interessato.");
                                            }


                                        }

                                        using (dtPercentualeDisagio dtpd = new dtPercentualeDisagio())
                                        {
                                            List<PercentualeDisagioModel> lpdm =
                                                dtpd.GetPercentualeDisagioIndennitaByRange(trm.idUfficio,
                                                    trm.dataPartenza, dataRientro, db).ToList();


                                            if (lpdm?.Any() ?? false)
                                            {
                                                foreach (var pdm in lpdm)
                                                {
                                                    dtpd.AssociaPercentualeDisagio_Indennita(trm.idTrasferimento, pdm.idPercentualeDisagio, db);
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception("Non risulta la percentuale di disagio per l'ufficio interessato.");
                                            }


                                        }

                                        using (dtCoefficenteSede dtcs = new dtCoefficenteSede())
                                        {
                                            List<CoefficientiSedeModel> lcsm =
                                                dtcs.GetCoefficenteSedeIndennitaByRangeDate(trm.idUfficio,
                                                    trm.dataPartenza, dataRientro, db).ToList();

                                            if (lcsm?.Any() ?? false)
                                            {
                                                foreach (var csm in lcsm)
                                                {
                                                    dtcs.AssociaCoefficenteSede_Indennita(trm.idTrasferimento, csm.idCoefficientiSede, db);
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception("Non risulta il valore di coefficiente di sede per l'ufficio interessato.");
                                            }

                                        }
                                        //////
                                        using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                                        {

                                            rdm = dtrd.InserisciRuoloDipendentePartenza(trm, db);

                                        }
                                        ///////
                                        using (dtFasciaKm dtfkm = new dtFasciaKm())
                                        {
                                            using (dtPrimaSistemazione dtps = new dtPrimaSistemazione())
                                            {
                                                var pfkmm = dtfkm.GetPercentualeFKM(trm.idFKM, trm.dataPartenza, db);
                                                var psm = dtps.GetPrimaSistemazioneBtIdTrasf(trm.idTrasferimento, db);

                                                if (pfkmm?.idPFKM > 0)
                                                {
                                                    dtfkm.AssociaPercentualeFKMPrimaSistemazione(psm.idPrimaSistemazione, pfkmm.idPFKM, db);
                                                }
                                                else
                                                {
                                                    throw new Exception("Non risulta il valore della percentuale fascia chilometrica.");
                                                }
                                            }

                                        }
                                    }
                                    #endregion

                                    #region maggiorazione abitazione
                                    using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                                    {
                                        dtma.PreSetMaggiorazioneAbitazione(trm, db);
                                    }
                                    #endregion

                                    //#region provvidenze scolastiche
                                    //using (dtAttivazioniProvScol dtps = new dtAttivazioniProvScol())
                                    //{
                                    //    //dtps.PreSetMaggiorazioneAbitazione(dtps, db);
                                    //}
                                    //#endregion


                                    //#region riallinea date di tutti i componenti del trasferimento 
                                    //var t = db.TRASFERIMENTO.Find(trm.idTrasferimento);
                                    //dttr.AllineaDateTrasferimento(t, db);
                                    //#endregion

                                    if (idTrasferimentoOld > 0)
                                    {
                                        #region riassocio indennita del trasferimento precedente
                                        var t_old = db.TRASFERIMENTO.Find(idTrasferimentoOld);
                                        dttr.RiassociaIndennitaTrasferimento(t_old, db);
                                        #endregion

                                        #region aggiorno data ricalcolo dipendente del trasferimento precedente
                                        using (dtDipendenti dtd = new dtDipendenti())
                                        {
                                            dtd.DataInizioRicalcoliDipendente(t_old.IDTRASFERIMENTO, t_old.DATARIENTRO, db, true);
                                        }
                                        #endregion
                                    }
                                    db.Database.CurrentTransaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    db.Database.CurrentTransaction.Rollback();
                                    //return PartialView("ErrorPartial", new HandleErrorInfo(ex, "Trasferimento", "InserisciTrasferimento"));
                                    throw ex;
                                }
                            }

                            ricaricaInfoTrasf = true;
                            return RedirectToAction("NuovoTrasferimento", new { matricola = matricola, idTrasferimento = trm.idTrasferimento, ricaricaInfoTrasf = ricaricaInfoTrasf, ricaricaTrasferimenti = ricaricaTrasferimenti, idRuoloDipendente = rdm.idRuoloDipendente });
                        }
                    }
                    catch (Exception ex)
                    {
                        var lTipoTrasferimento = new List<SelectListItem>();
                        var lUffici = new List<SelectListItem>();
                        var lRuoloUfficio = new List<SelectListItem>();
                        var lTipologiaCoan = new List<SelectListItem>();
                        var lFasciaKM = new List<SelectListItem>();

                        ListeComboNuovoTrasf(out lTipoTrasferimento, out lUffici, out lRuoloUfficio, out lTipologiaCoan, out lFasciaKM);

                        ViewBag.ListTipoTrasferimento = lTipoTrasferimento;
                        ViewBag.ListUfficio = lUffici;
                        ViewBag.ListTipoCoan = lTipologiaCoan;
                        ViewBag.ListFasciaKM = lFasciaKM;

                        ViewBag.ricaricaInfoTrasf = ricaricaInfoTrasf;
                        ViewBag.Matricola = matricola;                      

                        using (dtDipendenti dtd = new dtDipendenti())
                        {
                            var d = dtd.GetDipendenteByMatricola(Convert.ToInt16(matricola));
                            ViewBag.Dipendente = d;

                            FiltraRuoloUfficio(out lRuoloUfficio, d);
                        }
                        ViewBag.ListRuolo = lRuoloUfficio;

                        ViewBag.idTrasferimentoOld = idTrasferimentoOld;
                        if (idTrasferimentoOld > 0)
                        {
                            using (dtTrasferimento dtt = new dtTrasferimento())
                            {
                                var tOld = dtt.GetSoloTrasferimentoById(idTrasferimentoOld);
                                if (tOld.idStatoTrasferimento == EnumStatoTraferimento.Terminato)
                                {
                                    //ViewBag.ListTipoTrasferimento =
                                    //    lTipoTrasferimento.Where(
                                    //        a =>
                                    //            a.Value == "" ||
                                    //            a.Value ==
                                    //            Convert.ToDecimal(EnumTipoTrasferimento.SedeEstero).ToString());
                                    ViewBag.ListUfficio = lUffici.Where(a => a.Value != tOld.idUfficio.ToString());
                                }
                                else if (tOld.idStatoTrasferimento == EnumStatoTraferimento.Attivo)
                                {
                                    ViewBag.ListTipoTrasferimento =
                                        lTipoTrasferimento.Where(
                                            a =>
                                                a.Value == "" ||
                                                a.Value ==
                                                Convert.ToDecimal(EnumTipoTrasferimento.EsteroEstero).ToString() ||
                                                a.Value ==
                                                Convert.ToDecimal(EnumTipoTrasferimento.EsteroEsteroStessaRegiona)
                                                    .ToString());
                                    ViewBag.ListUfficio = lUffici.Where(a => a.Value != tOld.idUfficio.ToString());
                                }
                                else
                                {
                                    return PartialView("ErrorPartial",
                                        new MsgErr()
                                        {
                                            msg =
                                                "Impossibile inserire un nuovo trasferimento con stato del trasferimento ha: " +
                                                tOld.idStatoTrasferimento.ToString()
                                        });
                                }
                            }
                        }

                        ModelState.AddModelError("", ex.Message);

                        //ViewBag.Modifica = modifica;

                        return PartialView("NuovoTrasferimento", trm);
                    }
                }
                else
                {
                    var lTipoTrasferimento = new List<SelectListItem>();
                    var lUffici = new List<SelectListItem>();
                    var lRuoloUfficio = new List<SelectListItem>();
                    var lTipologiaCoan = new List<SelectListItem>();
                    var lFasciaKM = new List<SelectListItem>();

                    ListeComboNuovoTrasf(out lTipoTrasferimento, out lUffici, out lRuoloUfficio, out lTipologiaCoan, out lFasciaKM);

                    ViewBag.ListTipoTrasferimento = lTipoTrasferimento;
                    ViewBag.ListUfficio = lUffici;
                    ViewBag.ListTipoCoan = lTipologiaCoan;
                    ViewBag.ListFasciaKM = lFasciaKM;

                    ViewBag.ricaricaInfoTrasf = ricaricaInfoTrasf;
                    ViewBag.Matricola = matricola;

                    using (dtDipendenti dtd = new dtDipendenti())
                    {
                        var d = dtd.GetDipendenteByMatricola(Convert.ToInt16(matricola));
                        ViewBag.Dipendente = d;
                        FiltraRuoloUfficio(out lRuoloUfficio, d);
                    }

                    ViewBag.ListRuolo = lRuoloUfficio;

                    ViewBag.idTrasferimentoOld = idTrasferimentoOld;
                    if (idTrasferimentoOld > 0)
                    {
                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            var tOld = dtt.GetSoloTrasferimentoById(idTrasferimentoOld);
                            if (tOld.idStatoTrasferimento == EnumStatoTraferimento.Terminato)
                            {
                                //ViewBag.ListTipoTrasferimento = lTipoTrasferimento.Where(a => a.Value == "" || a.Value == Convert.ToDecimal(EnumTipoTrasferimento.SedeEstero).ToString());
                                ViewBag.ListUfficio = lUffici.Where(a => a.Value != tOld.idUfficio.ToString());
                            }
                            else if (tOld.idStatoTrasferimento == EnumStatoTraferimento.Attivo)
                            {
                                ViewBag.ListTipoTrasferimento = lTipoTrasferimento.Where(a => a.Value == "" || a.Value == Convert.ToDecimal(EnumTipoTrasferimento.EsteroEstero).ToString() || a.Value == Convert.ToDecimal(EnumTipoTrasferimento.EsteroEsteroStessaRegiona).ToString());
                                ViewBag.ListUfficio = lUffici.Where(a => a.Value != tOld.idUfficio.ToString());
                            }
                            else
                            {
                                return PartialView("ErrorPartial", new MsgErr() { msg = "Impossibile inserire un nuovo trasferimento con stato del trasferimento ha: " + tOld.idStatoTrasferimento.ToString() });
                            }
                        }
                    }
                    return PartialView("NuovoTrasferimento", trm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        public JsonResult NotificaTrasferimento(decimal idTrasferimento)
        {
            TrasferimentoModel trm = new TrasferimentoModel();
            DipendentiModel dipendente = new DipendentiModel();
            //ModelloMsgMail msgMail = new ModelloMsgMail();
            ModelloAllegatoMail allegato = new ModelloAllegatoMail();
            DocumentiModel dm = new DocumentiModel();
            string msgRet = string.Empty;
            Destinatario dest = new Destinatario();
            UfficiModel um = new UfficiModel();

            try
            {
                using (GestioneEmail gmail = new GestioneEmail())
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        trm = dtt.GetSoloTrasferimentoById(idTrasferimento);

                        if (trm != null && trm.idTrasferimento > 0)
                        {
                            using (dtUffici dtu = new dtUffici())
                            {
                                um = dtu.GetUffici(trm.idUfficio);
                            }
                            using (dtDipendenti dtd = new dtDipendenti())
                            {
                                dipendente = dtd.GetDipendenteByID(trm.idDipendente);

                                if (dipendente != null && dipendente.idDipendente > 0)
                                {
                                    using (dtDocumenti dtdc = new dtDocumenti())
                                    {
                                        dm = dtdc.GetDocumentoByIdTrasferimento(idTrasferimento);

                                        var docByte = dtdc.GetDocumentoByteById(dm.idDocumenti);
                                        Stream streamDoc = new MemoryStream(docByte);

                                        allegato.nomeFile = dm.nomeDocumento + dm.estensione;
                                        allegato.allegato = streamDoc;

                                        dest.Nominativo = dipendente.Nominativo;
                                        dest.EmailDestinatario = dipendente.email;

                                        using (ModelloMsgMail msgMail = new ModelloMsgMail())
                                        {
                                            msgMail.destinatario.Add(dest);
                                            msgMail.oggetto = Resources.msgEmail.OggettoNotificaTrasferimento;
                                            msgMail.priorita = System.Net.Mail.MailPriority.High;
                                            msgMail.corpoMsg = string.Format(Resources.msgEmail.MessaggioNotificaTrasferimento, um.descUfficio + " (" + um.codiceUfficio + ")");
                                            msgMail.allegato.Add(allegato);

                                            if (dtt.NotificaTrasferimento(trm.idTrasferimento))
                                            {
                                                gmail.sendMail(msgMail);
                                                msgRet = "Notifica del trasferimento effettuato con successo.";

                                                return Json(new { msg = msgRet, Nominativo = dipendente.Nominativo });
                                            }
                                            else
                                            {
                                                throw new Exception("Errore nella fase di attivazione del trasferimento.");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    throw new Exception("Non risulta nessun dipendente per l'id: " + trm.idDipendente);
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Non risulta il trasferimento per l'id: " + idTrasferimento);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }

        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        public JsonResult EliminaTrasferimento(decimal idTrasferimento, int matricola)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var ltrasf = new List<SelectListItem>();
                        //var ltrasf = new IList<TrasferimentoModel>();


                        dtt.EliminaTrasferimento(idTrasferimento, db);

                        decimal idTrasferimentoPrecedente = 0;

                        ltrasf = dtt.LeggiElencoTrasferimentiByMatricola(matricola);

                        if (ltrasf.Count() > 0)
                        {
                            var last_t = dtt.GetUltimoTrasferimentoTerminatoByMatricola(Convert.ToString(matricola));

                            idTrasferimentoPrecedente = last_t.idTrasferimento;
                            using (dtRichiamo dtr = new dtRichiamo())
                            {
                                var rm = dtr.GetRichiamoByIdTrasf(idTrasferimentoPrecedente);
                                if (rm.IdRichiamo > 0 == false)
                                {
                                    dtt.RipristinaTrasferimento(idTrasferimentoPrecedente,db);
                                }
                            }
                        }

                        return Json(new
                        {
                            msg = "",
                            listaTrasferimenti = ltrasf,
                            idTrasferimentoPrecedente = idTrasferimentoPrecedente
                        });
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }

        }

        [HttpPost]
        public JsonResult VerificaStatoStrasferimentoJsonResult(decimal idTrasferimento)
        {
            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    TrasferimentoModel trm = dtt.GetSoloTrasferimentoById(idTrasferimento);

                    if (trm?.idTrasferimento > 0)
                    {
                        if (trm.idStatoTrasferimento == EnumStatoTraferimento.Attivo || trm.idStatoTrasferimento == EnumStatoTraferimento.Terminato)
                        {
                            //return Json(new { StatoTrasferimento = (decimal)trm.idStatoTrasferimento });
                            return Json(new { StatoTrasferimento = 1 });
                        }
                        else
                        {
                            return Json(new { StatoTrasferimento = 0 });
                        }
                    }
                    else
                    {
                        return Json(new { StatoTrasferimento = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult VerificaRientro(decimal idTrasferimento)
        {
            try
            {
                if (idTrasferimento <= 0)
                {
                    throw new Exception("Trasferimento non valorizzato");
                }
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    TrasferimentoModel trm = dtt.GetTrasferimentoById(idTrasferimento);
                    if (trm != null)
                    {
                        if (trm.idStatoTrasferimento == EnumStatoTraferimento.Attivo || trm.idStatoTrasferimento == EnumStatoTraferimento.Terminato)
                        {

                            var ltrasf = dtt.GetListaTrasferimentoByMatricola(trm.Dipendente.matricola);
                            if (ltrasf?.Any() ?? false)
                            {
                                var ultimo_trasf = ltrasf.First();
                                if (ultimo_trasf.idTrasferimento == idTrasferimento)
                                {
                                    return Json(new { RientroAbilitato = 1 });
                                }
                                else
                                {
                                    using (dtRichiamo dtr = new dtRichiamo())
                                    {
                                        var r = dtr.GetRichiamoByIdTrasf(idTrasferimento);
                                        if (r.IdRichiamo > 0)
                                        {
                                            return Json(new { RientroAbilitato = 1 });
                                        }
                                        else
                                        {
                                            if (trm.idStatoTrasferimento == EnumStatoTraferimento.Attivo)
                                            {
                                                return Json(new { RientroAbilitato = 1 });
                                            }
                                            else
                                            {
                                                return Json(new { RientroAbilitato = 0 });
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                return Json(new { RientroAbilitato = 0 });
                            }
                        }
                        else
                        {
                            return Json(new { RientroAbilitato = 0 });
                        }
                    }
                    else
                    {
                        return Json(new { RientroAbilitato = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }


        public JsonResult VerificaNotificaTrasferimento(decimal idTrasferimento)
        {
            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    TrasferimentoModel trm = dtt.GetSoloTrasferimentoById(idTrasferimento);

                    if (trm?.idTrasferimento > 0)
                    {
                        if (trm.notificaTrasferimento)
                        {
                            return Json(new { notificaTrasferimento = trm.notificaTrasferimento == true ? 1 : 0 });
                        }
                        else
                        {
                            return Json(new { notificaTrasferimento = 0 });
                        }
                    }
                    else
                    {
                        return Json(new { notificaTrasferimento = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult VerirficaCompilazioneTrasferimento(decimal idTrasferimento)
        {
            try
            {
                if (idTrasferimento == 0)
                {
                    throw new Exception("Il trasferimento non risulta valorizzato.");
                }

                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    TrasferimentoModel trm = dtt.GetSoloTrasferimentoById(idTrasferimento);
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
                            trm.idStatoTrasferimento > 0 &&
                            trm.idDipendente > 0 &&
                            trm.idTipoCoan > 0 &&
                            trm.dataPartenza > DateTime.MinValue &&
                            trm.idRuoloUfficio > 0 &&
                            trm.protocolloLettera != string.Empty &&
                            trm.dataLettera > DateTime.MinValue &&
                            trm.idDocumento > 0
                            )
                        {
                            return Json(new { VerificaCompilazione = 1 });
                        }
                        else
                        {
                            return Json(new { VerificaCompilazione = 0 });
                        }
                    }
                    else
                    {
                        return Json(new { VerificaCompilazione = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }

        }

        public ActionResult GestioneTrasferimento(decimal idTrasferimento)
        {
            try
            {
                TrasferimentoModel tm = new TrasferimentoModel();
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var msgmail = dtt.GetMessaggioAnnullaTrasf(idTrasferimento);
                    var msg = msgmail.corpoMsg;
                    ViewBag.msgAnnulla = msg;

                    tm = dtt.GetTrasferimentoById(idTrasferimento);

                    var dataPartenza = tm.dataPartenza.ToShortDateString();
                    ViewData.Add("dataPartenza", dataPartenza);
                    ViewData.Add("Trasferimento", tm);
                }

                ViewBag.idTrasferimento = idTrasferimento;

                return PartialView(tm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        public ActionResult AttivitaTrasferimento(decimal idTrasferimento)
        {
            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    var tr = dtt.GetTrasferimentoById(idTrasferimento);
                    using (dtDipendenti dtd = new dtDipendenti())
                    {
                        var d = dtd.GetDipendenteByIDTrasf(idTrasferimento);
                        if (tr != null && tr.HasValue())
                        {
                            ViewBag.idTrasferimento = tr.idTrasferimento;
                        }
                        else
                        {
                            throw new Exception("Nessun trasferimento per la matricola (" + d.matricola + ")");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });

            }



            return PartialView();
        }


        public JsonResult VerificaMaggiorazioneFamiliare(decimal idTrasferimento)
        {
            try
            {
                if (idTrasferimento.Equals(null))
                {
                    throw new Exception("Il trasferimento non risulta valorizzato.");
                }
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    TrasferimentoModel trm = dtt.GetSoloTrasferimentoById(idTrasferimento);

                    if (trm != null && trm.HasValue())
                    {
                        using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                        {
                            trm.RuoloUfficio = dtrd.GetRuoloDipendenteByIdTrasferimento(trm.idTrasferimento, DateTime.Now).RuoloUfficio;
                            trm.idRuoloUfficio = trm.RuoloUfficio.idRuoloUfficio;
                        }

                        using (dtMaggiorazioniFamiliari dtd = new dtMaggiorazioniFamiliari())
                        {
                            MaggiorazioniFamiliariModel dm = dtd.GetMaggiorazioniFamiliariByID(trm.idTrasferimento);

                            if (dm.idMaggiorazioniFamiliari.ToString() != null)
                            {
                                return Json(new { idmaggiorazione = dm.idMaggiorazioniFamiliari.ToString() });
                            }
                            else
                            {
                                return Json(new { idmaggiorazione = 0 });
                            }
                        }
                    }
                    else
                    {
                        return Json(new { idmaggiorazione = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }


        }

        public JsonResult VerificaMaggiorazioneFamiliareByStatoTrasferimento(decimal idTrasferimento)
        {
            try
            {
                if (idTrasferimento.Equals(null))
                {
                    throw new Exception("Il trasferimento non risulta valorizzata.");
                }
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    TrasferimentoModel trm = dtt.GetSoloTrasferimentoById(idTrasferimento);
                    if (trm != null && trm.HasValue())
                    {
                        if (trm.idStatoTrasferimento == EnumStatoTraferimento.Attivo || trm.idStatoTrasferimento == EnumStatoTraferimento.Terminato)
                        {
                            using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                            {
                                trm.RuoloUfficio = dtrd.GetRuoloDipendenteByIdTrasferimento(trm.idTrasferimento, DateTime.Now).RuoloUfficio;
                                trm.idRuoloUfficio = trm.RuoloUfficio.idRuoloUfficio;
                            }
                            using (dtMaggiorazioniFamiliari dtd = new dtMaggiorazioniFamiliari())
                            {
                                MaggiorazioniFamiliariModel dm = dtd.GetMaggiorazioniFamiliariByID(trm.idTrasferimento);

                                if (dm.idMaggiorazioniFamiliari.ToString() != null)
                                {
                                    return Json(new { idmaggiorazione = dm.idMaggiorazioniFamiliari.ToString() });
                                }
                                else
                                {
                                    return Json(new { idmaggiorazione = 0 });
                                }

                            }
                        }
                        else
                        {
                            return Json(new { idmaggiorazione = 0 });
                        }
                    }
                    else
                    {
                        return Json(new { idmaggiorazione = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }


        }


        [HttpPost]
        public ActionResult ElencoTrasferimento(int matricola, decimal idTrasferimento = 0)
        {
            var r = new List<SelectListItem>();
            bool admin = false;
            bool notificato = false;

            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    if (matricola > 0)
                    {
                        var lt = dtt.GetListaTrasferimento(matricola);
                        if (lt?.Any() ?? false)
                        {
                            r = (from e in lt
                                 select new SelectListItem()
                                 {
                                     Text = e.Ufficio.descUfficio +
                                                " (" + e.Ufficio.codiceUfficio + ")" + " - " +
                                                (
                                                    (
                                                        e.idStatoTrasferimento != EnumStatoTraferimento.Annullato ?
                                                        (e.dataPartenza.ToShortDateString() + " ÷ " +
                                                            (
                                                                (e.dataRientro.HasValue == true &&
                                                                    e.dataRientro < Utility.DataFineStop()
                                                            ) ? e.dataRientro.Value.ToShortDateString() : "--/--/----"
                                                         ))
                                                         : "ANNULLATO")
                                               ),
                                     Value = e.idTrasferimento.ToString()
                                 }).ToList();

                            if (idTrasferimento == 0)
                            {
                                r.First().Selected = true;
                            }
                            else
                            {
                                r.First(a => a.Value == idTrasferimento.ToString()).Selected = true;
                            }
                            var t = dtt.GetSoloTrasferimentoById(lt.First().idTrasferimento);
                            if (t.idTrasferimento > 0)
                            {
                                notificato = t.notificaTrasferimento;
                            }


                        }
                    }

                    ViewBag.ListaTrasferimento = r;

                    admin = Utility.Amministratore();

                    ViewBag.Amministratore = admin;
                    ViewBag.Notificato = notificato;
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }

        public JsonResult ConfermaAttivaTrasf(decimal idTrasferimento, string strDataPartenzaEffettiva)
        {
            string errore = "";
            DateTime dataPartenzaEffettiva = Convert.ToDateTime(strDataPartenzaEffettiva);
            var ltrasf = new List<SelectListItem>();

            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    dtt.AttivaTrasf(idTrasferimento, dataPartenzaEffettiva);

                    ltrasf = dtt.LeggiElencoTrasferimenti(idTrasferimento);
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
                        listaTrasferimenti = ltrasf
                    });
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ConfermaAnnullaTrasf(FormCollection fc)
        {
            FormCollection collection = new FormCollection(Request.Unvalidated().Form);
            var ltrasf = new List<SelectListItem>();

            string errore = "";
            decimal idTrasferimento = Convert.ToDecimal(collection["idTrasferimento"]);
            string testoAnnullaTrasf = collection["msg"];
            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    dtt.AnnullaTrasf(idTrasferimento, testoAnnullaTrasf);
                    ltrasf = dtt.LeggiElencoTrasferimenti(idTrasferimento);

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
                        listaTrasferimenti = ltrasf
                    });
        }

        public ActionResult MessaggioAnnullaTrasf(decimal idTrasferimento)
        {
            ModelloMsgMail msg = new ModelloMsgMail();

            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    using (dtDipendenti dtd = new dtDipendenti())
                    {
                        using (dtUffici dtu = new dtUffici())
                        {
                            var t = dtt.GetTrasferimentoById(idTrasferimento);

                            if (t?.idTrasferimento > 0)
                            {
                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                var uff = dtu.GetUffici(t.idUfficio);

                                msg.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullamentoTrasferimento, dip.nome + " " + dip.cognome, uff.descUfficio + " (" + uff.codiceUfficio + ")");
                                ViewBag.idTrasferimento = idTrasferimento;
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

        public JsonResult VerificaMaggiorazioneAbitazione(decimal idTrasferimento)
        {
            try
            {
                if (idTrasferimento.Equals(null))
                {
                    throw new Exception("Il trasferimento non risulta valorizzato.");
                }
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        TrasferimentoModel trm = dtt.GetSoloTrasferimentoById(idTrasferimento);
                        if (trm != null && trm.HasValue())
                        {
                            if (trm.idStatoTrasferimento == EnumStatoTraferimento.Attivo || trm.idStatoTrasferimento == EnumStatoTraferimento.Terminato)
                            {
                                using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                                {
                                    trm.RuoloUfficio = dtrd.GetRuoloDipendenteByIdTrasferimento(trm.idTrasferimento, DateTime.Now).RuoloUfficio;
                                    trm.idRuoloUfficio = trm.RuoloUfficio.idRuoloUfficio;
                                }
                                using (dtMaggiorazioneAbitazione dtma = new dtMaggiorazioneAbitazione())
                                {
                                    MABModel mam = dtma.GetMABModelPartenza(idTrasferimento, db);

                                    if (mam.idMAB.ToString() != null)
                                    {
                                        return Json(new { idMAB = mam.idMAB.ToString() });
                                    }
                                    else
                                    {
                                        return Json(new { idMAB = 0 });
                                    }
                                }
                            }
                            else
                            {
                                return Json(new { idMAB = 0 });
                            }
                        }
                        else
                        {
                            return Json(new { idMAB = 0 });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }

        public JsonResult VerificaSaldoTEPartenza(decimal idTrasferimento)
        {
            try
            {
                if (idTrasferimento.Equals(null))
                {
                    throw new Exception("Il trasferimento non risulta valorizzato.");
                }
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        TrasferimentoModel trm = dtt.GetSoloTrasferimentoById(idTrasferimento);
                        if (trm != null && trm.HasValue())
                        {
                            if (trm.idStatoTrasferimento == EnumStatoTraferimento.Attivo || trm.idStatoTrasferimento == EnumStatoTraferimento.Terminato)
                            {
                                using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                                {
                                    TrasportoEffettiPartenzaModel TEPartenzamam = dtte.GetTEPartenzaModel(idTrasferimento, db);

                                    if (TEPartenzamam.idTEPartenza > 0)
                                    {
                                        return Json(new { idTEPartenza = TEPartenzamam.idTEPartenza.ToString() });
                                    }
                                    else
                                    {
                                        return Json(new { idTEPartenza = 0 });
                                    }
                                }
                            }
                            else
                            {
                                return Json(new { idTEPartenza = 0 });
                            }
                        }
                        else
                        {
                            return Json(new { idTEPartenza = 0 });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }


        }


        public JsonResult VerificaPassaporto(decimal idTrasferimento)
        {
            try
            {
                if (idTrasferimento.Equals(null))
                {
                    throw new Exception("Il trasferimento non risulta valorizzato.");
                }
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    TrasferimentoModel trm = dtt.GetSoloTrasferimentoById(idTrasferimento);
                    if (trm != null && trm.HasValue())
                    {
                        if (trm.idStatoTrasferimento == EnumStatoTraferimento.Attivo || trm.idStatoTrasferimento == EnumStatoTraferimento.Terminato)
                        {

                            using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                            {
                                PassaportoModel pm = dtpp.GetPassaportoByID(idTrasferimento);

                                if (pm.idPassaporto.ToString() != null)
                                {
                                    return Json(new { idPassaporto = pm.idPassaporto.ToString() });
                                }
                                else
                                {
                                    return Json(new { idPassaporto = 0 });
                                }
                            }
                        }
                        else
                        {
                            return Json(new { idPassaporto = 0 });
                        }
                    }
                    else
                    {
                        return Json(new { idPassaporto = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }

        public JsonResult VerificaTitoliViaggio(decimal idTrasferimento)
        {
            try
            {
                if (idTrasferimento.Equals(null))
                {
                    throw new Exception("Il trasferimento non risulta valorizzato.");
                }
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    TrasferimentoModel trm = dtt.GetSoloTrasferimentoById(idTrasferimento);
                    if (trm != null && trm.HasValue())
                    {
                        if (trm.idStatoTrasferimento == EnumStatoTraferimento.Attivo || trm.idStatoTrasferimento == EnumStatoTraferimento.Terminato)
                        {

                            using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                            {
                                decimal idTitoloViaggio = dttv.GetIdTitoliViaggio(idTrasferimento);

                                if (idTitoloViaggio.ToString() != null)
                                {
                                    return Json(new { idTitoloViaggio = idTitoloViaggio.ToString() });
                                }
                                else
                                {
                                    return Json(new { idTitoloViaggio = 0 });
                                }
                            }
                        }
                        else
                        {
                            return Json(new { idTitoloViaggio = 0 });
                        }
                    }
                    else
                    {
                        return Json(new { idTitoloViaggio = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }
        
        public JsonResult InserisceEventoTrasferimentoDattivare(decimal idTrasferimento)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        if (idTrasferimento.Equals(null))
                        {
                            throw new Exception("Il trasferimento non risulta valorizzato.");
                        }
                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                            {
                                if (dtce.EsisteEventoTrasferimentoDaAttivare(idTrasferimento, EnumFunzioniEventi.TrasferimentoDaAttivare, db) == false)
                                {
                                    var t = dtt.GetSoloTrasferimentoById(idTrasferimento);
    
                                    DateTime dtMax = dtt.GetDataAttivazioneMassimaPartenza(t.idTrasferimento, db);
    
                                    CalendarioEventiModel cem = new CalendarioEventiModel()
                                    {   
                                        idFunzioneEventi = EnumFunzioniEventi.TrasferimentoDaAttivare,
                                        idTrasferimento = idTrasferimento,
                                        DataInizioEvento = dtMax,
                                        DataScadenza = t.dataPartenza < dtMax ? dtMax : t.dataPartenza,
                                    };
                                    dtce.InsertCalendarioEvento(ref cem, db);                               
                                }
                            }
                        }
                        db.Database.CurrentTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        db.Database.CurrentTransaction.Rollback();
                        throw ex;
                    }
                }
                return Json(new { err = "" });
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }


        public JsonResult ModificaEventoTrasferimentoDattivareInCompletato(decimal idTrasferimento)
        {
            try
            {
                if (idTrasferimento.Equals(null))
                {
                    throw new Exception("Il trasferimento non risulta valorizzato.");
                }
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                        {
                            if (dtce.EsisteEventoTrasferimentoDaAttivare(idTrasferimento, EnumFunzioniEventi.TrasferimentoDaAttivare, db))
                            {
                                dtce.ModificaInCompletatoCalendarioEvento(idTrasferimento, EnumFunzioniEventi.TrasferimentoDaAttivare, db);
                            }
                        }
                    }
                }
                return Json(new { err = "" });
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }

    }
}
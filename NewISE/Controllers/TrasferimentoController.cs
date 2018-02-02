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

namespace NewISE.Controllers
{

    public class TrasferimentoController : Controller
    {
        #region Metodi privati

        private void ListeComboNuovoTrasf(out List<SelectListItem> lTipoTrasferimento, out List<SelectListItem> lUffici, out List<SelectListItem> lRuoloUfficio, out List<SelectListItem> lTipologiaCoan, out List<SelectListItem> lFasciaKM)
        {
            var r = new List<SelectListItem>();

            using (dtTipoTrasferimento dttt = new dtTipoTrasferimento())
            {
                var ltt = dttt.GetListTipoTrasferimento().OrderBy(a => a.descTipoTrasf).ToList();

                if (ltt != null && ltt.Count > 0)
                {
                    r = (from t in ltt
                         select new SelectListItem()
                         {
                             Text = t.descTipoTrasf,
                             Value = t.idTipoTrasferimento.ToString()
                         }).ToList();
                    r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                }

                lTipoTrasferimento = r;
            }

            using (dtUffici dtl = new dtUffici())
            {
                var llm = dtl.GetUffici().OrderBy(a => a.descUfficio).ToList();

                if (llm != null && llm.Count > 0)
                {
                    r = (from t in llm
                         select new SelectListItem()
                         {
                             Text = t.descUfficio,
                             Value = t.idUfficio.ToString()
                         }).ToList();

                    r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                }

                lUffici = r;
            }

            using (dtRuoloUfficio dtru = new dtRuoloUfficio())
            {
                var lru = dtru.GetListRuoloUfficio().OrderBy(a => a.DescrizioneRuolo).ToList();

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

            using (dtTipologiaCoan dttc = new dtTipologiaCoan())
            {
                var ltc = dttc.GetListTipologiaCoan().OrderBy(a => a.descrizione).ToList();

                if (ltc != null && ltc.Count > 0)
                {
                    r = (from t in ltc
                         select new SelectListItem()
                         {
                             Text = t.descrizione,
                             Value = t.idTipoCoan.ToString()
                         }).ToList();

                    r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                }

                lTipologiaCoan = r;
            }


            using (dtFasciaKm dtfkm = new dtFasciaKm())
            {
                var lfkm = dtfkm.GetListFascieChilometriche().ToList();

                if (lfkm?.Any() ?? false)
                {
                    r = (from t in lfkm
                         select new SelectListItem()
                         {
                             Text = t.KM,
                             Value = t.idFKM.ToString()
                         }).ToList();

                    r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                }

                lFasciaKM = r;
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
            bool richiesteTV = false;
            bool concluseTV = false;
            bool richiestaTE = false;
            bool attivazioneTE = false;
            bool richiestaAnticipi = false;
            bool attivazioneAnticipi = false;

            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    dtt.GestioneAttivitaTrasferimento(idTrasferimento, out richiestaMF, out attivazioneMF,
                        out richiestaPP, out conclusePP, out richiesteTV, out concluseTV, out richiestaTE, out attivazioneTE,
                        out richiestaAnticipi, out attivazioneAnticipi);
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
                        richiestaMF = richiestaMF,
                        attivazioneMF = attivazioneMF,
                        richiestaPP = richiestaPP,
                        conclusePP = conclusePP,
                        richiesteTV = richiesteTV,
                        concluseTV = concluseTV,
                        richiestaTE = richiestaTE,
                        attivazioneTE = attivazioneTE,
                        richiestaAnticipi = richiestaAnticipi,
                        attivazioneAnticipi = attivazioneAnticipi
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
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult VerificaTrasferimento(string matricola, bool ricaricaInfoTrasf = false)
        {
            try
            {
                using (dtTrasferimento dttr = new dtTrasferimento())
                {
                    var trm = dttr.GetUltimoTrasferimentoByMatricola(matricola);

                    if (trm != null && trm.HasValue())
                    {
                        return RedirectToAction("", new { idTrasferimento = trm.idTrasferimento, matricola = matricola, ricaricaInfoTrasf = ricaricaInfoTrasf });
                    }
                    else
                    {
                        return RedirectToAction("NuovoTrasferimento", new { matricola = matricola, ricaricaInfoTrasf = ricaricaInfoTrasf });
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
                }

                ListeComboNuovoTrasf(out lTipoTrasferimento, out lUffici, out lRuoloUfficio, out lTipologiaCoan, out lFasciaKM);

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
                            ViewBag.ListTipoTrasferimento = lTipoTrasferimento.Where(a => a.Value == "" || a.Value == Convert.ToDecimal(EnumTipoTrasferimento.SedeEstero).ToString());
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
        public ActionResult NuovoTrasferimento(int matricola, decimal idTrasferimento = 0, bool ricaricaInfoTrasf = false, bool ricaricaTrasferimenti = false)
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
                }

                ListeComboNuovoTrasf(out lTipoTrasferimento, out lUffici, out lRuoloUfficio, out lTipologiaCoan, out lFasciaKM);

                ViewBag.ListTipoTrasferimento = lTipoTrasferimento;
                ViewBag.ListUfficio = lUffici;
                ViewBag.ListRuolo = lRuoloUfficio;
                ViewBag.ListTipoCoan = lTipologiaCoan;
                ViewBag.ListFasciaKM = lFasciaKM;

                ViewBag.ricaricaInfoTrasf = ricaricaInfoTrasf;
                ViewBag.Matricola = matricola;


                //using (dtTrasferimento dttr = new dtTrasferimento())
                //{

                if (idTrasferimento > 0)
                {
                    //TrasferimentoModel trm = dttr.GetSoloTrasferimentoById(idTrasferimento);
                    return RedirectToAction("ModificaTrasferimento", new { idTrasferimento = idTrasferimento, matricola = matricola, ricaricaInfoTrasf = ricaricaInfoTrasf, ricaricaTrasferimenti = ricaricaTrasferimenti });
                }
                else
                {
                    ViewBag.ListTipoTrasferimento = lTipoTrasferimento.Where(a => a.Value == "" || a.Value == Convert.ToDecimal(EnumTipoTrasferimento.SedeEstero).ToString());
                    return PartialView();
                }

                //}
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

                    if (tm.idStatoTrasferimento == EnumStatoTraferimento.Attivo || tm.idStatoTrasferimento == EnumStatoTraferimento.Terminato)
                    {
                        abilitaNuovoTrasferimento = true;
                        abilitaSalva = false;
                        abilitaNotifica = false;
                    }
                    else if (tm.notificaTrasferimento == false)
                    {
                        abilitaNuovoTrasferimento = false;
                        abilitaSalva = true;

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
                        abilitaNotifica = abilitaNotifica,
                        abilitaNuovoTrasferimento = abilitaNuovoTrasferimento,
                        abilitaSalva = abilitaSalva,
                        trasferimentoSuccessivo = trasferimentoSuccessivo

                    });


        }


        [Authorize(Roles = "1 ,2")]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ModificaTrasferimento(decimal idTrasferimento, int matricola, bool ricaricaInfoTrasf = false, bool ricaricaTrasferimenti = false)
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
                            using (dtRuoloUfficio dtru = new dtRuoloUfficio())
                            {
                                trm.RuoloUfficio = dtru.GetRuoloUfficioValidoByIdTrasferimento(trm.idTrasferimento);
                                trm.idRuoloUfficio = trm.RuoloUfficio.idRuoloUfficio;
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

                            using (dtRuoloUfficio dtru = new dtRuoloUfficio())
                            {
                                trm.RuoloUfficio = dtru.GetRuoloUfficioValidoByIdTrasferimento(trm.idTrasferimento);
                                trm.idRuoloUfficio = trm.RuoloUfficio.idRuoloUfficio;
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

                        case EnumStatoTraferimento.Non_Trasferito:
                            trm.Ufficio = new UfficiModel();
                            trm.RuoloUfficio = new RuoloUfficioModel();

                            ViewBag.ListTipoTrasferimento = lTipoTrasferimento.Where(a => a.Value == "" || a.Value == ((decimal)EnumTipoTrasferimento.SedeEstero).ToString());

                            return PartialView();

                        case EnumStatoTraferimento.Terminato:
                            ViewBag.ListTipoTrasferimento = lTipoTrasferimento.Where(a => a.Value == "" || a.Value == ((decimal)EnumTipoTrasferimento.SedeEstero).ToString());
                            using (dtRuoloUfficio dtru = new dtRuoloUfficio())
                            {
                                trm.RuoloUfficio = dtru.GetRuoloUfficioValidoByIdTrasferimento(trm.idTrasferimento);
                                trm.idRuoloUfficio = trm.RuoloUfficio.idRuoloUfficio;
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

        public ActionResult ConfermaModificaTrasferimento(TrasferimentoModel trm, int matricola, bool ricaricaInfoTrasf = false)
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

                                dttr.EditTrasferimento(trm, db);
                                using (dtIndennita dti = new dtIndennita())
                                {
                                    IndennitaModel im = dti.GetIndennitaByIdTrasferimento(trm.idTrasferimento, db);


                                    im.dataAggiornamento = DateTime.Now;

                                    dti.EditIndennita(im, db);

                                    DateTime dataRientro = trm.dataRientro.HasValue == true
                                            ? trm.dataRientro.Value
                                            : Utility.DataFineStop();

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

                                    using (dtTFR dttfr = new dtTFR())
                                    {
                                        dttfr.RimuoviAsscoiazioniTFR_Indennita(trm.idTrasferimento, trm.dataPartenza, dataRientro, db);

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

                                    using (dtPercentualeDisagio dtpd = new dtPercentualeDisagio())
                                    {
                                        dtpd.RimuoviAssociazioniPercentualeDisagio_Indennita(trm.idTrasferimento, trm.dataPartenza, dataRientro, db);

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

                                    using (dtCoefficenteSede dtcs = new dtCoefficenteSede())
                                    {
                                        dtcs.RimuoviCoefficientiSede_Indennita(trm.idTrasferimento, trm.dataPartenza, dataRientro, db);

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
                                            throw new Exception("Non risulta il valore di coefficente di sede per l'ufficio interessato.");
                                        }

                                    }

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



                                    using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                                    {
                                        RuoloDipendenteModel rdm = dtrd.GetRuoloDipendente(trm.idTrasferimento, trm.idRuoloUfficio, trm.dataPartenza, db);

                                        if (rdm == null || rdm.hasValue() == false)
                                        {
                                            rdm = new RuoloDipendenteModel()
                                            {
                                                idRuolo = trm.idRuoloUfficio,
                                                idTrasferimento = trm.idTrasferimento,
                                                dataInizioValidita = trm.dataPartenza,
                                                dataFineValidita = Utility.DataFineStop(),
                                                dataAggiornamento = DateTime.Now,
                                                annullato = false
                                            };

                                            dtrd.SetRuoloDipendente(ref rdm, db);

                                        }
                                        else
                                        {
                                            dtrd.SetNuovoRuoloDipendente(ref rdm, db);
                                        }

                                    }


                                    using (dtFasciaKm dtfkm = new dtFasciaKm())
                                    {
                                        dtfkm.RimuoviAssociazionePercentualeFKM(trm.idTrasferimento, db);

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


                                    // }

                                }

                                db.Database.CurrentTransaction.Commit();

                                ricaricaInfoTrasf = true;

                                return RedirectToAction("ModificaTrasferimento", new { idTrasferimento = trm.idTrasferimento, matricola = matricola, ricaricaInfoTrasf = ricaricaInfoTrasf });

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
            try
            {
                //trm.idTrasferimento = 0;
                trm.idStatoTrasferimento = EnumStatoTraferimento.Da_Attivare;
                trm.dataAggiornamento = DateTime.Now;


                if (ModelState.IsValid)
                {
                    try
                    {
                        using (dtTrasferimento dttr = new dtTrasferimento())
                        {
                            ///inserisce le informazioni

                            using (ModelDBISE db = new ModelDBISE())
                            {
                                try
                                {
                                    db.Database.BeginTransaction();

                                    dttr.SetTrasferimento(ref trm, db);

                                    using (dtDipendenti dtd = new dtDipendenti())
                                    {
                                        trm.Dipendente = dtd.GetDipendenteByIDTrasf(trm.idTrasferimento, db);
                                    }


                                    if (idTrasferimentoOld > 0)
                                    {


                                        dttr.TerminaTrasferimento(idTrasferimentoOld, trm.dataPartenza, db);
                                        //ricaricaTrasferimenti = true;
                                    }


                                    using (dtPrimaSistemazione dtps = new dtPrimaSistemazione())
                                    {
                                        PrimaSistemazioneModel psm = new PrimaSistemazioneModel()
                                        {
                                            idPrimaSistemazione = trm.idTrasferimento,
                                            dataOperazione = DateTime.Now
                                        };

                                        dtps.InserisciPrimaSistemazione(psm, db);

                                    }

                                    using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                                    {
                                        dtmf.PreSetMaggiorazioniFamiliari(trm.idTrasferimento, db);
                                    }

                                    using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                                    {
                                        dtpp.PreSetPassaporto(trm.idTrasferimento, db);
                                    }

                                    using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                                    {
                                        dttv.PreSetTitoloViaggio(trm.idTrasferimento, db);
                                    }

                                    using (dtTrasportoEffetti dtte = new dtTrasportoEffetti())
                                    {
                                        dtte.PreSetTrasportoEffetti(trm.idTrasferimento, db);
                                    }

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
                                                throw new Exception("Non risulta il valore di coefficente di sede per l'ufficio interessato.");
                                            }

                                        }

                                        using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                                        {
                                            RuoloDipendenteModel rdm = dtrd.GetRuoloDipendente(trm.idTrasferimento, trm.idRuoloUfficio, trm.dataPartenza, db);

                                            if (rdm == null || rdm.hasValue() == false)
                                            {
                                                rdm = new RuoloDipendenteModel()
                                                {
                                                    idRuolo = trm.idRuoloUfficio,
                                                    idTrasferimento = trm.idTrasferimento,
                                                    dataInizioValidita = trm.dataPartenza,
                                                    dataFineValidita = Utility.DataFineStop(),
                                                    dataAggiornamento = DateTime.Now,
                                                    annullato = false
                                                };

                                                dtrd.SetRuoloDipendente(ref rdm, db);

                                            }
                                            else
                                            {
                                                dtrd.SetNuovoRuoloDipendente(ref rdm, db);
                                            }

                                        }

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
                            return RedirectToAction("NuovoTrasferimento", new { matricola = matricola, idTrasferimento = trm.idTrasferimento, ricaricaInfoTrasf = ricaricaInfoTrasf, ricaricaTrasferimenti = ricaricaTrasferimenti });
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
                        ViewBag.ListRuolo = lRuoloUfficio;
                        ViewBag.ListTipoCoan = lTipologiaCoan;
                        ViewBag.ListFasciaKM = lFasciaKM;

                        ViewBag.ricaricaInfoTrasf = ricaricaInfoTrasf;
                        ViewBag.Matricola = matricola;

                        using (dtDipendenti dtd = new dtDipendenti())
                        {
                            var d = dtd.GetDipendenteByMatricola(Convert.ToInt16(matricola));
                            ViewBag.Dipendente = d;
                        }

                        ViewBag.idTrasferimentoOld = idTrasferimentoOld;
                        if (idTrasferimentoOld > 0)
                        {
                            using (dtTrasferimento dtt = new dtTrasferimento())
                            {
                                var tOld = dtt.GetSoloTrasferimentoById(idTrasferimentoOld);
                                if (tOld.idStatoTrasferimento == EnumStatoTraferimento.Terminato)
                                {
                                    ViewBag.ListTipoTrasferimento =
                                        lTipoTrasferimento.Where(
                                            a =>
                                                a.Value == "" ||
                                                a.Value ==
                                                Convert.ToDecimal(EnumTipoTrasferimento.SedeEstero).ToString());
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
                    ViewBag.ListRuolo = lRuoloUfficio;
                    ViewBag.ListTipoCoan = lTipologiaCoan;
                    ViewBag.ListFasciaKM = lFasciaKM;

                    ViewBag.ricaricaInfoTrasf = ricaricaInfoTrasf;
                    ViewBag.Matricola = matricola;

                    using (dtDipendenti dtd = new dtDipendenti())
                    {
                        var d = dtd.GetDipendenteByMatricola(Convert.ToInt16(matricola));
                        ViewBag.Dipendente = d;
                    }

                    ViewBag.idTrasferimentoOld = idTrasferimentoOld;
                    if (idTrasferimentoOld > 0)
                    {
                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            var tOld = dtt.GetSoloTrasferimentoById(idTrasferimentoOld);
                            if (tOld.idStatoTrasferimento == EnumStatoTraferimento.Terminato)
                            {
                                ViewBag.ListTipoTrasferimento = lTipoTrasferimento.Where(a => a.Value == "" || a.Value == Convert.ToDecimal(EnumTipoTrasferimento.SedeEstero).ToString());
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
                        if (trm.idStatoTrasferimento == EnumStatoTraferimento.Attivo)
                        {
                            return Json(new { StatoTrasferimento = (decimal)trm.idStatoTrasferimento });
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

            ViewBag.idTrasferimento = idTrasferimento;

            return PartialView();
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
                                     Text = e.Ufficio.descUfficio + " (" + e.Ufficio.codiceUfficio + ")" + " - " + e.dataPartenza.ToShortDateString() + " ÷ " + (e.dataRientro.HasValue == true ? e.dataRientro.Value.ToShortDateString() : "--/--/----"),
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

                        }
                    }

                    ViewBag.ListaTrasferimento = r;

                    admin = Utility.Amministratore();

                    ViewBag.Amministratore = admin;
                }


            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView();
        }


    }
}
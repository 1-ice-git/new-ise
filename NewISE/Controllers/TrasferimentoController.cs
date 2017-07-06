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
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace NewISE.Controllers
{
    public class TrasferimentoController : Controller
    {
        #region Metodi privati

        private void ListeComboNuovoTrasf(out List<SelectListItem> lTipoTrasferimento, out List<SelectListItem> lUffici, out List<SelectListItem> lRuoloUfficio, out List<SelectListItem> lTipologiaCoan)
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
        }

        #endregion Metodi privati

        public ActionResult InfoTrasferimento(string matricola)
        {
            dipInfoTrasferimentoModel dit = new dipInfoTrasferimentoModel();

            try
            {
                using (dtTrasferimento dtdt = new dtTrasferimento())
                {
                    dit = dtdt.GetInfoTrasferimento(matricola);

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
                return PartialView("ErrorPartial");
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
                return PartialView("ErrorPartial");
            }
        }

        public ActionResult NuovoTrasferimento(string matricola, bool ricaricaInfoTrasf = false)
        {
            var lTipoTrasferimento = new List<SelectListItem>();
            var lUffici = new List<SelectListItem>();
            var lRuoloUfficio = new List<SelectListItem>();
            var lTipologiaCoan = new List<SelectListItem>();

            try
            {
                using (dtDipendenti dtd = new dtDipendenti())
                {
                    var d = dtd.GetDipendenteByMatricola(Convert.ToInt16(matricola));
                    ViewBag.Dipendente = d;
                }

                ListeComboNuovoTrasf(out lTipoTrasferimento, out lUffici, out lRuoloUfficio, out lTipologiaCoan);

                ViewBag.ListTipoTrasferimento = lTipoTrasferimento;
                ViewBag.ListUfficio = lUffici;
                ViewBag.ListRuolo = lRuoloUfficio;
                ViewBag.ListTipoCoan = lTipologiaCoan;

                ViewBag.ricaricaInfoTrasf = ricaricaInfoTrasf;
                ViewBag.Matricola = matricola;

                using (dtTrasferimento dttr = new dtTrasferimento())
                {
                    TrasferimentoModel trm = dttr.GetUltimoSoloTrasferimentoByMatricola(matricola);
                    {
                        if (trm != null && trm.HasValue())
                        {
                            return RedirectToAction("ModificaTrasferimento", new { idTrasferimento = trm.idTrasferimento, matricola = matricola, ricaricaInfoTrasf = ricaricaInfoTrasf });
                        }
                        else
                        {
                            ViewBag.ListTipoTrasferimento = lTipoTrasferimento.Where(a => a.Value == "" || a.Value == 1.ToString());
                            return PartialView();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }

        [Authorize(Roles = "1 ,2")]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ModificaTrasferimento(decimal idTrasferimento, string matricola, bool ricaricaInfoTrasf = false)
        {
            var lTipoTrasferimento = new List<SelectListItem>();
            var lUffici = new List<SelectListItem>();
            var lRuoloUfficio = new List<SelectListItem>();
            var lTipologiaCoan = new List<SelectListItem>();

            try
            {
                ListeComboNuovoTrasf(out lTipoTrasferimento, out lUffici, out lRuoloUfficio, out lTipologiaCoan);

                ViewBag.ListTipoTrasferimento = lTipoTrasferimento;
                ViewBag.ListUfficio = lUffici;
                ViewBag.ListRuolo = lRuoloUfficio;
                ViewBag.ListTipoCoan = lTipologiaCoan;

                ViewBag.ricaricaInfoTrasf = ricaricaInfoTrasf;
                ViewBag.Matricola = matricola;

                using (dtTrasferimento dttr = new dtTrasferimento())
                {
                    var trm = dttr.GetTrasferimentoById(idTrasferimento);

                    using (dtDipendenti dtd = new dtDipendenti())
                    {
                        var d = dtd.GetDipendenteByID(trm.idDipendente);
                        ViewBag.Dipendente = d;
                    }

                    switch ((EnumStatoTraferimento)trm.StatoTrasferimento)
                    {
                        case EnumStatoTraferimento.Attivo:

                            ViewBag.ListTipoTrasferimento = lTipoTrasferimento.Where(a => a.Value == "" || a.Value == 2.ToString() || a.Value == 3.ToString());
                            ViewBag.ListUfficio = lUffici.Where(a => a.Value != trm.idUfficio.ToString());
                            return PartialView();

                        case EnumStatoTraferimento.Da_Attivare:
                            ViewBag.ListTipoTrasferimento = lTipoTrasferimento.Where(a => a.Value == trm.idTipoTrasferimento.ToString());
                            //ViewBag.Modifica = true;

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

                            ViewBag.ListTipoTrasferimento = lTipoTrasferimento.Where(a => a.Value == "" || a.Value == 1.ToString());

                            return PartialView();

                        case EnumStatoTraferimento.Terminato:
                            ViewBag.ListTipoTrasferimento = lTipoTrasferimento.Where(a => a.Value == "" || a.Value == 1.ToString());
                            return PartialView();

                        default:

                            return PartialView("ErrorPartial");
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }


        }

        public ActionResult ConfermaModificaTrasferimento(TrasferimentoModel trm, string matricola, bool ricaricaInfoTrasf = false)
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

                                    im.dataInizio = trm.dataPartenza;
                                    im.dataFine = trm.dataRientro;
                                    im.dataAggiornamento = DateTime.Now;

                                    dti.EditIndennita(im, db);

                                    using (dtLivelliDipendente dtld = new dtLivelliDipendente())
                                    {

                                        dtld.RimuoviAssociazioneLivelloDipendente_Indennita(trm.idTrasferimento, trm.dataPartenza, db);


                                        LivelloDipendenteModel ldm = dtld.GetLivelloDipendente(trm.idDipendente, trm.dataPartenza, db);
                                        if (ldm.HasValue())
                                        {
                                            dtld.AssociaLivelloDipendente_Indennita(trm.idTrasferimento, ldm.idLivDipendente, db);
                                        }
                                        else
                                        {
                                            throw new Exception("Non risulta assegnato nessun livello per il dipendente " + trm.Dipendente.Nominativo + " (" + trm.Dipendente.matricola + ")");
                                        }

                                        using (dtIndennitaBase dtib = new dtIndennitaBase())
                                        {
                                            dtib.RimuoviAssociazioneIndennitaBase_Indennita(trm.idTrasferimento, trm.dataPartenza, db);

                                            IndennitaBaseModel ibm = new IndennitaBaseModel();
                                            ibm = dtib.GetIndennitaBaseValida(ldm.idLivello, trm.dataPartenza, db);
                                            if (ibm.HasValue())
                                            {
                                                dtib.AssociaIndennitaBase_Indennita(trm.idTrasferimento, ibm.idIndennitaBase, db);
                                            }
                                            else
                                            {
                                                throw new Exception("Non risulta l'indennità base per il livello interessato.");
                                            }
                                        }

                                        using (dtTFR dttfr = new dtTFR())
                                        {
                                            dttfr.RimuoviAssociaTFR_Indennita(trm.idTrasferimento, trm.dataPartenza, db);

                                            TFRModel tfrm = dttfr.GetTFRValido(trm.idUfficio, trm.dataPartenza, db);
                                            if (tfrm.HasValue())
                                            {
                                                dttfr.AssociaTFR_Indennita(trm.idTrasferimento, tfrm.idTFR, db);
                                            }
                                            else
                                            {
                                                throw new Exception("Non risulta il tasso fisso di ragguaglio per l'ufficio interessato.");
                                            }
                                        }

                                        using (dtPercentualeDisagio dtpd = new dtPercentualeDisagio())
                                        {
                                            dtpd.RimuoviAssociaPercentualeDisagio_Indennita(trm.idTrasferimento, trm.dataPartenza, db);

                                            PercentualeDisagioModel pdm = dtpd.GetPercentualeDisagioValida(trm.idUfficio, trm.dataPartenza, db);

                                            if (pdm.HasValue())
                                            {
                                                dtpd.AssociaPercentualeDisagio_Indennita(trm.idTrasferimento, pdm.idPercentualeDisagio, db);
                                            }
                                            else
                                            {
                                                throw new Exception("Non risulta la percentuale di disagio per l'ufficio interessato.");
                                            }
                                        }
                                        using (dtCoefficenteSede dtcs = new dtCoefficenteSede())
                                        {
                                            dtcs.RimuoviAssociaCoefficenteSede_Indennita(trm.idTrasferimento, trm.dataPartenza, db);

                                            CoefficientiSedeModel cs = dtcs.GetCoefficenteSedeValido(trm.idUfficio, trm.dataPartenza, db);
                                            if (cs.HasValue())
                                            {
                                                dtcs.AssociaCoefficenteSede_Indennita(trm.idTrasferimento, cs.idCoefficientiSede, db);
                                            }
                                            else
                                            {
                                                throw new Exception("Non risulta il valore di coefficente di sede per l'ufficio interessato.");
                                            }
                                        }

                                        using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                                        {

                                            dtrd.RimuoviAssociaRuoloDipendente_Indennita(trm.idTrasferimento, trm.dataPartenza, db);

                                            RuoloDipendenteModel rdm = dtrd.GetRuoloDipendente(trm.idRuoloUfficio, trm.dataPartenza, db);

                                            if (rdm.hasValue())
                                            {
                                                dtrd.AssociaRuoloDipendente_Indennita(trm.idTrasferimento, rdm.idRuoloDipendente, db);
                                            }
                                            else
                                            {
                                                rdm = new RuoloDipendenteModel()
                                                {
                                                    idRuolo = trm.idRuoloUfficio,
                                                    dataInizioValidita = trm.dataPartenza,
                                                    dataFineValidita = Convert.ToDateTime("31/12/9999"),
                                                    dataAggiornamento = DateTime.Now,
                                                    annullato = false
                                                };

                                                dtrd.SetRuoloDipendente(ref rdm, db);
                                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo ruolo dipendete.", "RuoloDipendente", db, trm.idTrasferimento, rdm.idRuoloDipendente);

                                                dtrd.AssociaRuoloDipendente_Indennita(trm.idTrasferimento, rdm.idRuoloDipendente, db);
                                            }
                                        }
                                    }

                                }

                                db.Database.CurrentTransaction.Commit();

                                ricaricaInfoTrasf = true;

                                return RedirectToAction("ModificaTrasferimento", new { idTrasferimento = trm.idTrasferimento, matricola = matricola, ricaricaInfoTrasf = ricaricaInfoTrasf });

                            }
                            catch (Exception ex)
                            {

                                db.Database.CurrentTransaction.Rollback();
                                return PartialView("ErrorPartial");
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

                    try
                    {
                        ListeComboNuovoTrasf(out lTipoTrasferimento, out lUffici, out lRuoloUfficio, out lTipologiaCoan);

                        ViewBag.ListTipoTrasferimento = lTipoTrasferimento;
                        ViewBag.ListUfficio = lUffici;
                        ViewBag.ListRuolo = lRuoloUfficio;
                        ViewBag.ListTipoCoan = lTipologiaCoan;

                        ViewBag.ricaricaInfoTrasf = ricaricaInfoTrasf;
                        ViewBag.Matricola = matricola;

                        return PartialView("ModificaTrasferimento", trm);
                    }
                    catch (Exception ex)
                    {
                        return PartialView("ErrorPartial");
                    }
                }
            }
            catch (Exception ex)
            {

                return PartialView("errorPartial");
            }
        }


        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        public ActionResult InserisciTrasferimento(TrasferimentoModel trm, string matricola, bool ricaricaInfoTrasf = false)
        {
            try
            {
                //trm.idTrasferimento = 0;
                trm.idStatoTrasferimento = (decimal)EnumStatoTraferimento.Da_Attivare;
                trm.dataAggiornamento = DateTime.Now;
                trm.annullato = false;

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

                                    using (dtIndennita dti = new dtIndennita())
                                    {
                                        IndennitaModel im = new IndennitaModel();
                                        LivelloDipendenteModel ldm = new LivelloDipendenteModel();

                                        im.idTrasfIndennita = trm.idTrasferimento;
                                        im.dataInizio = trm.dataPartenza;
                                        im.dataFine = Utility.DataFineStop();
                                        im.dataAggiornamento = DateTime.Now;

                                        dti.SetIndennita(im, db);

                                        //Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova indennità.", "Indennita", db, trm.idTrasferimento, im.idTrasfIndennita);

                                        using (dtLivelliDipendente dtld = new dtLivelliDipendente())
                                        {
                                            ldm = dtld.GetLivelloDipendente(trm.idDipendente, trm.dataPartenza, db);
                                            if (ldm.HasValue())
                                            {
                                                dtld.AssociaLivelloDipendente_Indennita(trm.idTrasferimento, ldm.idLivDipendente, db);
                                            }
                                            else
                                            {
                                                throw new Exception("Non risulta assegnato nessun livello per il dipendente " + trm.Dipendente.Nominativo + " (" + trm.Dipendente.matricola + ")");
                                            }
                                        }

                                        using (dtIndennitaBase dtib = new dtIndennitaBase())
                                        {
                                            IndennitaBaseModel ibm = new IndennitaBaseModel();
                                            ibm = dtib.GetIndennitaBaseValida(ldm.idLivello, trm.dataPartenza, db);
                                            if (ibm.HasValue())
                                            {
                                                dtib.AssociaIndennitaBase_Indennita(trm.idTrasferimento, ibm.idIndennitaBase, db);
                                            }
                                            else
                                            {
                                                throw new Exception("Non risulta l'indennità base per il livello interessato.");
                                            }
                                        }

                                        using (dtTFR dttfr = new dtTFR())
                                        {
                                            TFRModel tfrm = dttfr.GetTFRValido(trm.idUfficio, trm.dataPartenza, db);
                                            if (tfrm.HasValue())
                                            {
                                                dttfr.AssociaTFR_Indennita(trm.idTrasferimento, tfrm.idTFR, db);
                                            }
                                            else
                                            {
                                                throw new Exception("Non risulta il tasso fisso di ragguaglio per l'ufficio interessato.");
                                            }
                                        }

                                        using (dtPercentualeDisagio dtpd = new dtPercentualeDisagio())
                                        {
                                            PercentualeDisagioModel pdm = dtpd.GetPercentualeDisagioValida(trm.idUfficio, trm.dataPartenza, db);

                                            if (pdm.HasValue())
                                            {
                                                dtpd.AssociaPercentualeDisagio_Indennita(trm.idTrasferimento, pdm.idPercentualeDisagio, db);
                                            }
                                            else
                                            {
                                                throw new Exception("Non risulta la percentuale di disagio per l'ufficio interessato.");
                                            }
                                        }

                                        using (dtCoefficenteSede dtcs = new dtCoefficenteSede())
                                        {
                                            CoefficientiSedeModel cs = dtcs.GetCoefficenteSedeValido(trm.idUfficio, trm.dataPartenza, db);
                                            if (cs.HasValue())
                                            {
                                                dtcs.AssociaCoefficenteSede_Indennita(trm.idTrasferimento, cs.idCoefficientiSede, db);
                                            }
                                            else
                                            {
                                                throw new Exception("Non risulta il valore di coefficente di sede per l'ufficio interessato.");
                                            }
                                        }

                                        using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                                        {
                                            RuoloDipendenteModel rdm = dtrd.GetRuoloDipendente(trm.idRuoloUfficio, trm.dataPartenza, db);

                                            if (rdm.hasValue())
                                            {
                                                dtrd.AssociaRuoloDipendente_Indennita(trm.idTrasferimento, rdm.idRuoloDipendente, db);
                                            }
                                            else
                                            {
                                                rdm = new RuoloDipendenteModel()
                                                {
                                                    idRuolo = trm.idRuoloUfficio,
                                                    dataInizioValidita = trm.dataPartenza,
                                                    dataFineValidita = Convert.ToDateTime("31/12/9999"),
                                                    dataAggiornamento = DateTime.Now,
                                                    annullato = false
                                                };

                                                dtrd.SetRuoloDipendente(ref rdm, db);
                                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo ruolo dipendete.", "RuoloDipendente", db, trm.idTrasferimento, rdm.idRuoloDipendente);

                                                dtrd.AssociaRuoloDipendente_Indennita(trm.idTrasferimento, rdm.idRuoloDipendente, db);
                                            }
                                        }
                                    }

                                    db.Database.CurrentTransaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    db.Database.CurrentTransaction.Rollback();
                                    return PartialView("ErrorPartial");
                                }
                            }

                            ricaricaInfoTrasf = true;
                            return RedirectToAction("NuovoTrasferimento", new { matricola = matricola, ricaricaInfoTrasf = ricaricaInfoTrasf });
                        }
                    }
                    catch (Exception ex)
                    {
                        var lTipoTrasferimento = new List<SelectListItem>();
                        var lUffici = new List<SelectListItem>();
                        var lRuoloUfficio = new List<SelectListItem>();
                        var lTipologiaCoan = new List<SelectListItem>();

                        ListeComboNuovoTrasf(out lTipoTrasferimento, out lUffici, out lRuoloUfficio, out lTipologiaCoan);

                        ViewBag.ListTipoTrasferimento = lTipoTrasferimento;
                        ViewBag.ListUfficio = lUffici;
                        ViewBag.ListRuolo = lRuoloUfficio;
                        ViewBag.ListTipoCoan = lTipologiaCoan;

                        ViewBag.ricaricaInfoTrasf = ricaricaInfoTrasf;
                        ViewBag.Matricola = matricola;

                        using (dtDipendenti dtd = new dtDipendenti())
                        {
                            var d = dtd.GetDipendenteByMatricola(Convert.ToInt16(matricola));
                            ViewBag.Dipendente = d;
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

                    ListeComboNuovoTrasf(out lTipoTrasferimento, out lUffici, out lRuoloUfficio, out lTipologiaCoan);

                    ViewBag.ListTipoTrasferimento = lTipoTrasferimento;
                    ViewBag.ListUfficio = lUffici;
                    ViewBag.ListRuolo = lRuoloUfficio;
                    ViewBag.ListTipoCoan = lTipologiaCoan;

                    ViewBag.ricaricaInfoTrasf = ricaricaInfoTrasf;
                    ViewBag.Matricola = matricola;

                    using (dtDipendenti dtd = new dtDipendenti())
                    {
                        var d = dtd.GetDipendenteByMatricola(Convert.ToInt16(matricola));
                        ViewBag.Dipendente = d;
                    }

                    //ViewBag.Modifica = modifica;

                    return PartialView("NuovoTrasferimento", trm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("errorPartial");
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

            try
            {
                using (GestioneEmail gmail = new GestioneEmail())
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        trm = dtt.GetSoloTrasferimentoById(idTrasferimento);

                        if (trm != null && trm.idTrasferimento > 0)
                        {
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

                                        allegato.nomeFile = dm.NomeDocumento + dm.Estensione;
                                        allegato.allegato = streamDoc;

                                        dest.Nominativo = dipendente.Nominativo;
                                        dest.EmailDestinatario = "mauro.arduini@ritspa.it";
                                        using (ModelloMsgMail msgMail = new ModelloMsgMail())
                                        {
                                            msgMail.destinatario.Add(dest);
                                            msgMail.oggetto = "Notifica trasferimento";
                                            msgMail.priorita = System.Net.Mail.MailPriority.High;
                                            msgMail.corpoMsg = "Messaggio notifica trasferimento";
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
        public JsonResult VerirficaCompilazioneTrasferimento(string matricola = "")
        {
            try
            {
                if (matricola == string.Empty)
                {
                    throw new Exception("La matricola non risulta valorizzata.");
                }
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    TrasferimentoModel trm = dtt.GetUltimoSoloTrasferimentoByMatricola(matricola);
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
                            trm.idDocumento > 0)
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



            return null;
        }
    }
}
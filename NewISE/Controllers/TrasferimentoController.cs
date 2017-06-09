
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.dtObj;
using System;
using System.Collections.Generic;
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
        public ActionResult NuovoTrasferimento(string matricola, bool ricaricaInfoTrasf = false)
        {
            var lTipoTrasferimento = new List<SelectListItem>();
            var lUffici = new List<SelectListItem>();
            var lRuoloUfficio = new List<SelectListItem>();
            var lTipologiaCoan = new List<SelectListItem>();

            TrasferimentoModel trm = new TrasferimentoModel();

            ViewBag.Modifica = false;

            try
            {
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

                using (dtTrasferimento dttr = new dtTrasferimento())
                {
                    trm = dttr.GetUltimoTrasferimentoByMatricola(matricola);

                    
                    //using (dtRuoloUfficio dtru = new dtRuoloUfficio())
                    //{
                    //    RuoloUfficioModel rum = new RuoloUfficioModel();
                    //    rum = dtru.GetRuoloDipendente(trm.idTrasferimento, trm.dataPartenza).RuoloUfficio;

                    //    trm.idRuoloUfficio = rum.idRuoloUfficio;
                    //}


                    switch ((EnumStatoTraferimento)trm.StatoTrasferimento)
                    {
                        case EnumStatoTraferimento.Attivo:

                            ViewBag.ListTipoTrasferimento = lTipoTrasferimento.Where(a => a.Value == "" || a.Value == 2.ToString() || a.Value == 3.ToString());
                            ViewBag.ListUfficio = lUffici.Where(a => a.Value != trm.idUfficio.ToString());
                            return PartialView();

                        case EnumStatoTraferimento.Da_Attivare:
                            ViewBag.ListTipoTrasferimento = lTipoTrasferimento.Where(a => a.Value == trm.idTipoTrasferimento.ToString());
                            ViewBag.Modifica = true;
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

        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciTrasferimento(TrasferimentoModel trm, string matricola, bool ricaricaInfoTrasf = false, bool modifica = false)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        using (dtTrasferimento dttr = new dtTrasferimento())
                        {
                            ///inserisce le informazioni
                            if (modifica == false)
                            {
                                trm.idStatoTrasferimento = (decimal)EnumStatoTraferimento.Da_Attivare;
                                trm.dataAggiornamento = DateTime.Now;
                                trm.annullato = false;

                                using (EntitiesDBISE db = new EntitiesDBISE())
                                {
                                    try
                                    {
                                        db.Database.BeginTransaction();

                                        dttr.SetTrasferimento(ref trm, db);

                                        using (dtIndennita dti=new dtIndennita())
                                        {
                                            IndennitaModel im = new IndennitaModel();
                                            LivelloDipendenteModel ldm = new LivelloDipendenteModel();

                                            im.idTrasferimento = trm.idTrasferimento;
                                            //im.idLivDipendente = trm.id
                                            using (dtLivelliDipendente dtld=new dtLivelliDipendente())
                                            {
                                                ldm = dtld.GetLivelloDipendente(trm.idDipendente, trm.dataPartenza);
                                                if (ldm.HasValue())
                                                {
                                                    im.idLivDipendente = ldm.idLivDipendente;
                                                }
                                                else
                                                {
                                                    throw new Exception("Non risulta assegnato nessun livello per la matricola elaborata.");
                                                }                                                                                              

                                            }

                                            using (dtIndennitaBase dtib=new dtIndennitaBase())
                                            {
                                                IndennitaBaseModel ibm = new IndennitaBaseModel();

                                                ibm = dtib.GetIndennitaBaseValida(ldm.idLivello, trm.dataPartenza, db);

                                                if (ibm.HasValue())
                                                {
                                                    im.idIndennitaBase = ibm.idIndennitaBase;
                                                }
                                                else
                                                {
                                                    throw new Exception("Non risulta l'indennità base per il livello/datadeccorenza in elaboeazione.");
                                                }
                                            }

                                            using (dtTFR dttfr=new dtTFR())
                                            {
                                                TFRModel tfrm = dttfr.GetTFRValido(trm.idUfficio, trm.dataPartenza, db);
                                                if (tfrm.HasValue())
                                                {
                                                    im.idTFR = tfrm.idTFR;
                                                }
                                                else
                                                {
                                                    throw new Exception("Non risulta il tasso fisso di ragguaglio per l'utente elaborato.");
                                                }

                                            }
                                        }





                                        using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                                        {
                                            RuoloDipendenteModel rdm;
                                            rdm = dtrd.GetRuoloDipendente(trm.idTrasferimento, trm.dataPartenza, db);
                                            if (rdm.hasValue() == false)
                                            {
                                                rdm = new RuoloDipendenteModel()
                                                {
                                                    idRuolo = trm.idRuoloUfficio,
                                                    idTrasferimento = trm.idTrasferimento,
                                                    dataInizioValidita = trm.dataPartenza,
                                                    dataFineValidita = trm.dataRientro.HasValue == true ? trm.dataRientro.Value : Convert.ToDateTime("31/12/9999"),
                                                    dataAggiornamento = DateTime.Now,
                                                    annullato = false
                                                };

                                                dtrd.SetRuoloDipendente(rdm, db);
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
                            }
                            else///Modifica le informazioni
                            {
                                using (EntitiesDBISE db = new EntitiesDBISE())
                                {
                                    try
                                    {
                                        db.Database.BeginTransaction();

                                        dttr.EditTrasferimento(trm, db);
                                        

                                        using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                                        {
                                            RuoloDipendenteModel rdm;

                                            rdm = dtrd.GetRuoloDipendente(trm.idTrasferimento, trm.dataPartenza, db);

                                            if (rdm.hasValue() == false)
                                            {
                                                rdm = new RuoloDipendenteModel()
                                                {
                                                    idRuolo = trm.idRuoloUfficio,
                                                    idTrasferimento = trm.idTrasferimento,
                                                    dataInizioValidita = trm.dataPartenza,
                                                    dataFineValidita = trm.dataRientro.HasValue == true ? trm.dataRientro.Value : Convert.ToDateTime("31/12/9999"),
                                                    dataAggiornamento = DateTime.Now,
                                                    annullato = false
                                                };

                                                dtrd.SetRuoloDipendente(rdm, db);
                                            }
                                            else
                                            {

                                                rdm.idRuolo = trm.idRuoloUfficio;
                                                dtrd.EditRuoloDipendente(rdm, db);
                                                
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

                    return PartialView("NuovoTrasferimento", trm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("errorPartial"); ;
            }
        }
    }
}
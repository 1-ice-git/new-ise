﻿using NewISE.Areas.Dipendenti.Models;
using NewISE.Areas.Dipendenti.Models.DtObj;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NewISE.Areas.Dipendenti.Controllers
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
                var llm = dtl.GetUffici().OrderBy(a => a.DescUfficio).ToList();

                if (llm != null && llm.Count > 0)
                {
                    r = (from t in llm
                         select new SelectListItem()
                         {
                             Text = t.DescUfficio,
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

        [Authorize(Roles = "1 ,2")]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult NuovoTrasferimento(string matricola, bool ricaricaInfoTrasf = false)
        {
            //TrasferimentoModel tm = new TrasferimentoModel();
            //dipTrasferimentoModel dtm = new dipTrasferimentoModel();

            var lTipoTrasferimento = new List<SelectListItem>();
            var lUffici = new List<SelectListItem>();
            var lRuoloUfficio = new List<SelectListItem>();
            var lTipologiaCoan = new List<SelectListItem>();

            TrasferimentoModel trm = new TrasferimentoModel();

            try
            {
                ListeComboNuovoTrasf(out lTipoTrasferimento, out lUffici, out lRuoloUfficio, out lTipologiaCoan);

                ViewBag.ListTipoTrasferimento = lTipoTrasferimento;
                ViewBag.ListUfficio = lUffici;
                ViewBag.ListRuolo = lRuoloUfficio;
                ViewBag.ListTipoCoan = lTipologiaCoan;

                ViewBag.ricaricaInfoTrasf = ricaricaInfoTrasf;
                ViewBag.Matricola = matricola;

                using (dtDipendenti dtd=new dtDipendenti())
                {
                    var d = dtd.GetDipendenteByMatricola(Convert.ToInt16(matricola));
                    ViewBag.Dipendente = d;
                }

                using (dtTrasferimento dttr = new dtTrasferimento())
                {
                    trm = dttr.GetUltimoTrasferimentoByMatricola(matricola);

                    switch ((EnumStatoTraferimento)trm.StatoTrasferimento)
                    {
                        case EnumStatoTraferimento.Attivo:

                            return PartialView();

                        case EnumStatoTraferimento.Da_Attivare:

                            return PartialView();

                        case EnumStatoTraferimento.Non_Trasferito:
                            trm.Ufficio = new UfficiModel();
                            trm.RuoloUfficio = new RuoloUfficioModel();
                            return PartialView();

                        case EnumStatoTraferimento.Terminato:
                            return PartialView();
                        default:

                            return PartialView();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("errorPartial"); ;
            }
        }

        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciTrasferimento(TrasferimentoModel trm, string matricola, bool ricaricaInfoTrasf = false)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        using (dtDipTrasferimento dttr = new dtDipTrasferimento())
                        {
                            trm.idStatoTrasferimento = (decimal)EnumStatoTraferimento.Da_Attivare;
                            trm.dataAggiornamento = DateTime.Now;
                            trm.annullato = false;

                            using (EntitiesDBISE db = new EntitiesDBISE())
                            {
                                try
                                {
                                    db.Database.BeginTransaction();

                                    dttr.SetTrasferimento(trm, db);


                                    using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                                    {
                                        RuoloDipendenteModel rdm;
                                        dtrd.GetRuoloDipendente(trm.idRuoloUfficio, trm.idTrasferimento, trm.dataPartenza, db);
                                        if (dtrd.GetRuoloDipendente(trm.idRuoloUfficio, trm.idTrasferimento, trm.dataPartenza, db).hasValue() == false)
                                        {
                                            rdm = new RuoloDipendenteModel()
                                            {
                                                idRuolo = trm.idRuoloUfficio,
                                                idTrasferimento = trm.idTrasferimento,
                                                dataInizioValidita = trm.dataPartenza,
                                                dataFineValidita = trm.dataRientro.HasValue == true ? trm.dataRientro.Value : Convert.ToDateTime("31/12/9999")
                                            };
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
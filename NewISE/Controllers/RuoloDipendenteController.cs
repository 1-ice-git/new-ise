using System.Web.Routing;
using NewISE.EF;
using NewISE.Models.Tools;
using MaggiorazioniFamiliariModel = NewISE.Models.DBModel.MaggiorazioniFamiliariModel;
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
using System.Web.Helpers;
using NewISE.Models.Enumeratori;
using NewISE.Models.dtObj;

namespace NewISE.Controllers
{
    [Authorize(Roles = "1 ,2")]
    public class RuoloDipendenteController : Controller
    {
        // GET: Ruolodipendente
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult VerificaRuoloDipendente(decimal idTrasferimento)
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
                        TRASFERIMENTO t = dtt.GetTrasferimento(idTrasferimento, db);
                        if (t.IDTRASFERIMENTO > 0)
                        {
                            if (t.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo || t.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato)
                            {

                                using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                                {
                                    var RuoloDipendente = dtrd.GetRuoloDipendenteByIdTrasferimento(t.IDTRASFERIMENTO, t.DATAPARTENZA);

                                    if(RuoloDipendente.idRuoloDipendente>0)
                                    {
                                        return Json(new { idRuoloDipendente = RuoloDipendente.idRuoloDipendente });
                                    }
                                    else
                                    {
                                        return Json(new { idRuoloDipendente = 0 });
                                    }
                                }
                            }
                            else
                            {
                                return Json(new { idRuoloDipendente = 0 });
                            }
                        }
                        else
                        {
                            return Json(new { idRuoloDipendente = 0 });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }

        public ActionResult RuoloDipendente(decimal idTrasferimento)
        {
            try
            {
                ViewBag.idtrasferimento = idTrasferimento;
                return PartialView();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ElencoRuoliDipendente(decimal idTrasferimento)
        {
            try
            {
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    bool solaLettura = false;

                    var t = dtt.GetTrasferimentoById(idTrasferimento);
                    
                    if(t.idStatoTrasferimento!=EnumStatoTraferimento.Attivo && t.idStatoTrasferimento!=EnumStatoTraferimento.Terminato )
                    {
                        solaLettura = true;
                    }
                    ViewBag.idtrasferimento = idTrasferimento;
                    ViewBag.solalettura = solaLettura;
                    return PartialView();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult TabElencoRuoliDipendente(decimal idTrasferimento)
        {
            try
            {
                using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                {
                    List<VariazioneRuoloDipendenteModel> lvrdm = dtrd.GetListaRuoliDipendente(idTrasferimento).OrderBy(a=>a.dataInizioValidita).ToList();
                    ViewBag.idTrasferimento = idTrasferimento;

                    return PartialView(lvrdm);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NuovoRuoloDipendente(decimal idTrasferimento)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                    {
                        VariazioneRuoloDipendenteModel vrdm = new VariazioneRuoloDipendenteModel();

                        var rd = dtrd.GetRuoloDipendenteByIdTrasferimento(idTrasferimento, DateTime.Today);

                        List<SelectListItem> lRuoli = new List<SelectListItem>();

                        var r = new List<SelectListItem>();                      

                        r.Insert(0, new SelectListItem() { Text = "", Value = "" });

                        lRuoli = r;
                        ViewBag.lRuoli = lRuoli;

                        vrdm.ut_dataInizioValidita = null;

                        ViewBag.RuoloAttuale = rd.RuoloUfficio.DescrizioneRuolo;
                        ViewBag.idTrasferimento=idTrasferimento;

                        return PartialView(vrdm);
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ListaRuoliDipendente(string search, DateTime dtRif, decimal idTrasferimento)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        using (dtRuoloUfficio dtru = new dtRuoloUfficio())
                        {
                            using (dtLivelliDipendente dtld = new dtLivelliDipendente())
                            {
                                var t = dtt.GetTrasferimentoById(idTrasferimento);

                                var livelloDipendenteValido = dtld.GetLivelloDipendente(t.idDipendente, dtRif);

                                var lru = dtru.GetListRuoloUfficioByLivello(livelloDipendenteValido.idLivello);

                                VariazioneRuoloDipendenteModel vrdm = new VariazioneRuoloDipendenteModel();

                                List<Select2Model> lRuoli = new List<Select2Model>();

                                var r = new List<Select2Model>();

                                if (lru != null && lru.Count > 0)
                                {
                                    foreach (var ru in lru)
                                    {
                                        Select2Model s2 = new Select2Model()
                                        {
                                            id = ru.idRuoloUfficio.ToString(),
                                            text = ru.DescrizioneRuolo.ToString()
                                        };
                                        r.Add(s2);
                                    }
                                }

                                lRuoli = r;
                                ViewBag.lRuoli = lRuoli;

                                return Json(new { results = lRuoli, err = "" });
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                return Json(new { results = new List<Select2Model>(), err = ex.Message });
            }        
        }



        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciRuoloDipendente(VariazioneRuoloDipendenteModel vrdm, decimal idTrasferimento)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                    {
                        if (ModelState.IsValid)
                        {
                            db.Database.BeginTransaction();
                            try
                            {
                                #region verifica data inizio
                                try
                                {
                                    dtrd.VerificaDataInizioValiditaRuoloDipendente(idTrasferimento, vrdm, db);
                                }
                                catch (Exception ex)
                                {


                                    using (dtTrasferimento dtt = new dtTrasferimento())
                                    {
                                        using (dtRuoloUfficio dtru = new dtRuoloUfficio())
                                        {
                                            using (dtLivelliDipendente dtld = new dtLivelliDipendente())
                                            {
                                                var t = dtt.GetTrasferimentoById(idTrasferimento);

                                                var livelloDipendenteValido = dtld.GetLivelloDipendente(t.idDipendente, vrdm.ut_dataInizioValidita.Value);

                                                var lru = dtru.GetListRuoloUfficioByLivello(livelloDipendenteValido.idLivello);

                                                List<SelectListItem> lRuoli = new List<SelectListItem>();

                                                var r = new List<SelectListItem>();

                                                if (lru != null && lru.Count > 0)
                                                {
                                                    foreach (var ru in lru)
                                                    {
                                                        SelectListItem s2 = new SelectListItem()
                                                        {
                                                            Value = ru.idRuoloUfficio.ToString(),
                                                            Text = ru.DescrizioneRuolo.ToString()
                                                        };
                                                        r.Add(s2);
                                                    }
                                                }

                                                lRuoli = r;
                                                ViewBag.lRuoli = lRuoli;
                                                ViewBag.idTrasferimento = idTrasferimento;

                                                var rd = dtrd.GetRuoloDipendenteByIdTrasferimento(idTrasferimento, DateTime.Now);


                                                ViewBag.RuoloAttuale = rd.RuoloUfficio.DescrizioneRuolo;

                                                ModelState.AddModelError("", ex.Message);
                                                return PartialView("NuovoRuoloDipendente", vrdm);
                                            }
                                        }
                                    }

                                }
                                #endregion

                                vrdm.dataAggiornamento = DateTime.Now;

                                #region modello ruolo dipendente
                                RuoloDipendenteModel rdm = new RuoloDipendenteModel()
                                {
                                    idTrasferimento = idTrasferimento,
                                    idRuolo=vrdm.idRuolo,
                                    dataInizioValidita = vrdm.ut_dataInizioValidita.Value,
                                    annullato = false, 
                                    dataAggiornamento=vrdm.dataAggiornamento
                                };
                                #endregion

                                #region inserisce ruolo
                                if (vrdm.chkAggiornaTutti == false)
                                {
                                    dtrd.SetVariazioneRuoloDipendente(rdm, idTrasferimento, db);
                                }
                                else
                                {
                                    //inserisce periodo e annulla i periodi successivi (fino al primo buco temporale o fino a dataRientro)
                                    dtrd.SetVariazioneRuoloDipendente_AggiornaTutti(rdm, idTrasferimento, db);
                                }
                                #endregion

                                using (dtDipendenti dtd = new dtDipendenti())
                                {
                                    dtd.DataInizioRicalcoliDipendente(idTrasferimento, rdm.dataInizioValidita, db, false);
                                }                                                                         

                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento nuovo ruolo dipendente (" + rdm.idRuolo + ")", "RUOLODIPENDENTE", db, idTrasferimento, rdm.idRuoloDipendente);

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


                            using (dtTrasferimento dtt = new dtTrasferimento())
                            {
                                using (dtRuoloUfficio dtru = new dtRuoloUfficio())
                                {
                                    using (dtLivelliDipendente dtld = new dtLivelliDipendente())
                                    {
                                        var t = dtt.GetTrasferimentoById(idTrasferimento);

                                        var livelloDipendenteValido = dtld.GetLivelloDipendente(t.idDipendente, vrdm.ut_dataInizioValidita.Value);

                                        var lru = dtru.GetListRuoloUfficioByLivello(livelloDipendenteValido.idLivello);

                                        List<SelectListItem> lRuoli = new List<SelectListItem>();

                                        var r = new List<SelectListItem>();

                                        if (lru != null && lru.Count > 0)
                                        {
                                            foreach (var ru in lru)
                                            {
                                                SelectListItem s2 = new SelectListItem()
                                                {
                                                    Value = ru.idRuoloUfficio.ToString(),
                                                    Text = ru.DescrizioneRuolo.ToString()
                                                };
                                                r.Add(s2);
                                            }
                                        }

                                        lRuoli = r;
                                        ViewBag.lRuoli = lRuoli;
                                        ViewBag.idTrasferimento = idTrasferimento;

                                        return PartialView("NuovoRuoloDipendente", vrdm);
                                    }
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


            return RedirectToAction("ElencoRuoliDipendente", new { idTrasferimento = idTrasferimento });
        }


    }
}
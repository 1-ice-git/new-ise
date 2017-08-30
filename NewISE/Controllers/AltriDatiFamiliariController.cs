using NewISE.DBComuniItalia;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace NewISE.Controllers
{
    public class AltriDatiFamiliariController : Controller
    {
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult AltriDatiFamiliariFiglio(decimal idFiglio)
        {
            AltriDatiFamModel adf = new AltriDatiFamModel();
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();

            try
            {
                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                {
                    adf = dtadf.GetAlttriDatiFamiliariFiglio(idFiglio);
                }
                using (dtMaggiorazioniFamiliari dtmc = new dtMaggiorazioniFamiliari())
                {
                    mcm = dtmc.GetMaggiorazioniFamiliaribyFiglio(idFiglio);
                }
                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                {
                    bool rinunciaMagFam = false;
                    bool richiestaAttivazione = false;
                    bool attivazione = false;
                    bool datiConiuge = false;
                    bool datiParzialiConiuge = false;
                    bool datiFigli = false;
                    bool datiParzialiFigli = false;
                    bool siDocConiuge = false;
                    bool siDocFigli = false;

                    bool solaLettura = false;

                    dtmf.SituazioneMagFam(mcm.idMaggiorazioniFamiliari, out rinunciaMagFam,
                        out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                        out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli);

                    if (richiestaAttivazione == true)
                    {
                        solaLettura = true;
                    }
                    else
                    {
                        solaLettura = false;
                    }

                    ViewData.Add("solaLettura", solaLettura);
                }
                using (dtPercentualeMagFigli dtpmf = new dtPercentualeMagFigli())
                {
                    PercentualeMagFigliModel pf = dtpmf.GetPercentualeMaggiorazioneFigli(idFiglio, DateTime.Now);
                    if (pf != null && pf.HasValue())
                    {
                        switch (pf.idTipologiaFiglio)
                        {
                            case TipologiaFiglio.Residente:
                                adf.residente = true;
                                adf.studente = false;
                                break;
                            case TipologiaFiglio.Studente:
                                adf.studente = true;
                                adf.residente = true;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewData.Add("idMaggiorazioniFamiliari", mcm.idMaggiorazioniFamiliari);

            if (adf != null && adf.HasValue())
            {

                using (dtFigli dtf = new dtFigli())
                {
                    if (adf.idFigli.HasValue)
                    {
                        var fm = dtf.GetFigliobyID(adf.idFigli.Value);
                        adf.Figli = fm;
                    }
                }

                using (dtConiuge dtc = new dtConiuge())
                {
                    if (adf.idConiuge.HasValue)
                    {
                        var cm = dtc.GetConiugebyID(adf.idConiuge.Value);
                        adf.Coniuge = cm;
                    }
                }


                return PartialView(adf);
            }
            else
            {
                List<Comuni> comuni = new List<Comuni>();

                using (StreamReader sr = new StreamReader(Server.MapPath("~/DBComuniItalia/jsonComuniItalia.json")))
                {
                    comuni = JsonConvert.DeserializeObject<List<Comuni>>(sr.ReadToEnd(), new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }

                adf.idFigli = idFiglio;

                ViewData.Add("Comuni", comuni);

                using (dtFigli dtf = new dtFigli())
                {
                    string nominativo = dtf.GetFigliobyID(idFiglio).nominativo.ToUpper();
                    ViewData.Add("nominativo", nominativo);
                }

                return PartialView("InserisciAltriDatiFamiliariFiglio", adf);
            }

        }


        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult AltriDatiFamiliariConiuge(decimal idConiuge)
        {
            AltriDatiFamModel adf = new AltriDatiFamModel();
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();

            try
            {
                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                {
                    adf = dtadf.GetAlttriDatiFamiliariConiuge(idConiuge);
                }
                using (dtMaggiorazioniFamiliari dtmc = new dtMaggiorazioniFamiliari())
                {
                    mcm = dtmc.GetMaggiorazioniFamiliaribyConiuge(idConiuge);
                }

                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                {
                    bool rinunciaMagFam = false;
                    bool richiestaAttivazione = false;
                    bool attivazione = false;
                    bool datiConiuge = false;
                    bool datiParzialiConiuge = false;
                    bool datiFigli = false;
                    bool datiParzialiFigli = false;
                    bool siDocConiuge = false;
                    bool siDocFigli = false;

                    bool solaLettura = false;

                    dtmf.SituazioneMagFam(mcm.idMaggiorazioniFamiliari, out rinunciaMagFam,
                        out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                        out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli);

                    if (richiestaAttivazione == true)
                    {
                        solaLettura = true;
                    }
                    else
                    {
                        solaLettura = false;
                    }

                    ViewData.Add("solaLettura", solaLettura);
                }

                using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                {
                    PercentualeMagConiugeModel pc = dtpc.GetPercMagConiugeNow(idConiuge, DateTime.Now.Date);

                    if (pc != null && pc.HasValue())
                    {
                        switch (pc.idTipologiaConiuge)
                        {
                            case TipologiaConiuge.Residente:
                                adf.residente = true;
                                adf.ulterioreMagConiuge = false;
                                break;

                            case TipologiaConiuge.NonResidente:
                                adf.residente = false;
                                adf.ulterioreMagConiuge = false;
                                break;

                            case TipologiaConiuge.NonResidenteCarico:
                                adf.residente = false;
                                adf.ulterioreMagConiuge = true;
                                break;

                            default:
                                break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewData.Add("idMaggiorazioniFamiliari", mcm.idMaggiorazioniFamiliari);

            if (adf != null && adf.HasValue())
            {
                using (dtConiuge dtc = new dtConiuge())
                {
                    if (adf.idConiuge.HasValue)
                    {
                        var cm = dtc.GetConiugebyID(adf.idConiuge.Value);
                        adf.Coniuge = cm;
                    }
                }



                return PartialView(adf);
            }
            else
            {
                List<Comuni> comuni = new List<Comuni>();

                using (StreamReader sr = new StreamReader(Server.MapPath("~/DBComuniItalia/jsonComuniItalia.json")))
                {
                    comuni = JsonConvert.DeserializeObject<List<Comuni>>(sr.ReadToEnd(), new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }

                adf.idConiuge = idConiuge;

                ViewData.Add("Comuni", comuni);

                return PartialView("InserisciAltriDatiFamiliariConiuge", adf);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciAltriDatiFamiliariFiglio(AltriDatiFamModel adf, decimal idMaggiorazioniFamiliari)
        {
            try
            {
                adf.dataAggiornamento = DateTime.Now;
                adf.annullato = false;

                if (ModelState.IsValid)
                {
                    using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                    {
                        dtadf.SetAltriDatiFamiliariFiglio(adf);
                    }
                }
                else
                {

                    List<Comuni> comuni = new List<Comuni>();

                    using (StreamReader sr = new StreamReader(Server.MapPath("~/DBComuniItalia/jsonComuniItalia.json")))
                    {
                        comuni = JsonConvert.DeserializeObject<List<Comuni>>(sr.ReadToEnd(), new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                    }

                    ViewData.Add("Comuni", comuni);
                    ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);

                    return PartialView("InserisciAltriDatiFamiliariFiglio", adf);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return RedirectToAction("AltriDatiFamiliariFiglio", new { idFiglio = adf.idFigli });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciAltriDatiFamiliariConiuge(AltriDatiFamModel adf, decimal idMaggiorazioniFamiliari)
        {
            try
            {
                adf.dataAggiornamento = DateTime.Now;
                adf.annullato = false;

                if (ModelState.IsValid)
                {
                    using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                    {
                        dtadf.SetAltriDatiFamiliariConiuge(adf);
                    }
                }
                else
                {

                    List<Comuni> comuni = new List<Comuni>();

                    using (StreamReader sr = new StreamReader(Server.MapPath("~/DBComuniItalia/jsonComuniItalia.json")))
                    {
                        comuni = JsonConvert.DeserializeObject<List<Comuni>>(sr.ReadToEnd(), new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                    }

                    ViewData.Add("Comuni", comuni);
                    ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);

                    return PartialView("InserisciAltriDatiFamiliariConiuge", adf);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return RedirectToAction("AltriDatiFamiliariConiuge", new { idConiuge = adf.idConiuge });


        }

        public ActionResult ModificaAltriDatiFamiliariFiglio(decimal idAltriDatiFam, decimal idMaggiorazioniFamiliari)
        {
            AltriDatiFamModel adfm = new AltriDatiFamModel();

            try
            {
                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                {
                    adfm = dtadf.GetAltriDatiFamiliari(idAltriDatiFam);
                    if (adfm != null && adfm.HasValue())
                    {
                        using (dtPercentualeMagFigli dtpf = new dtPercentualeMagFigli())
                        {
                            PercentualeMagFigliModel pf = new PercentualeMagFigliModel();
                            pf = dtpf.GetPercentualeMaggiorazioneFigli(adfm.idFigli.Value, DateTime.Now.Date);

                            if (pf?.HasValue() ?? false)
                            {
                                switch (pf.idTipologiaFiglio)
                                {
                                    case TipologiaFiglio.Residente:
                                        adfm.residente = true;
                                        adfm.studente = false;
                                        break;
                                    case TipologiaFiglio.Studente:
                                        adfm.studente = true;
                                        adfm.residente = true;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
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

            List<Comuni> comuni = new List<Comuni>();

            try
            {


                using (StreamReader sr = new StreamReader(Server.MapPath("~/DBComuniItalia/jsonComuniItalia.json")))
                {
                    comuni = JsonConvert.DeserializeObject<List<Comuni>>(sr.ReadToEnd(), new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }


            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewData.Add("Comuni", comuni);
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);

            return PartialView(adfm);
        }

        public ActionResult ModificaAltriDatiFamiliariConiuge(decimal idAltriDatiFam, decimal idMaggiorazioniFamiliari)
        {
            AltriDatiFamModel adfm = new AltriDatiFamModel();

            try
            {
                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                {
                    adfm = dtadf.GetAltriDatiFamiliari(idAltriDatiFam);
                    if (adfm != null && adfm.HasValue())
                    {
                        using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                        {
                            PercentualeMagConiugeModel pc = new PercentualeMagConiugeModel();

                            pc = dtpc.GetPercMagConiugeNow(adfm.idConiuge.Value, DateTime.Now.Date);

                            if (pc != null && pc.HasValue())
                            {
                                switch (pc.idTipologiaConiuge)
                                {
                                    case TipologiaConiuge.Residente:
                                        adfm.residente = true;
                                        adfm.ulterioreMagConiuge = false;
                                        break;

                                    case TipologiaConiuge.NonResidente:
                                        adfm.residente = false;
                                        adfm.ulterioreMagConiuge = false;
                                        break;

                                    case TipologiaConiuge.NonResidenteCarico:
                                        adfm.residente = false;
                                        adfm.ulterioreMagConiuge = true;
                                        break;

                                    default:
                                        break;
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

            List<Comuni> comuni = new List<Comuni>();

            try
            {


                using (StreamReader sr = new StreamReader(Server.MapPath("~/DBComuniItalia/jsonComuniItalia.json")))
                {
                    comuni = JsonConvert.DeserializeObject<List<Comuni>>(sr.ReadToEnd(), new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }


            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            ViewData.Add("Comuni", comuni);
            ViewData.Add("idMaggiorazioniFamiliari", idMaggiorazioniFamiliari);

            return PartialView(adfm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfermaModificaAdf(AltriDatiFamModel adfm)
        {

            try
            {
                adfm.dataAggiornamento = DateTime.Now;
                adfm.annullato = false;

                if (ModelState.IsValid)
                {
                    using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                    {
                        dtadf.EditAltriDatiFamiliari(adfm);
                    }
                }
                else
                {
                    if (adfm.idConiuge.HasValue)
                    {
                        return PartialView("ModificaAltriDatiFamiliariConiuge", adfm);
                    }
                    else
                    {
                        return PartialView("ModificaAltriDatiFamiliariFiglio", adfm);
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            if (adfm.idConiuge.HasValue)
            {
                return RedirectToAction("AltriDatiFamiliariConiuge", new { idConiuge = adfm.idConiuge });
            }
            else if (adfm.idFigli.HasValue)
            {
                return RedirectToAction("AltriDatiFamiliariFiglio", new { idFiglio = adfm.idFigli });
            }
            else
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = "Errore nella modifica altri dati familiari" });
            }

        }

        //public JsonResult DatiComuneNascita(string pComune = "", string pProvincia = "", string pCap = "")
        //{
        //    string comune = string.Empty;
        //    string provincia = string.Empty;
        //    string cap = string.Empty;
        //    Comuni c = new Comuni();

        //    try
        //    {
        //        using (StreamReader sr = new StreamReader(Server.MapPath("~/DBComuniItalia/jsonComuniItalia.json")))
        //        {
        //            List<Comuni> comuni = JsonConvert.DeserializeObject<List<Comuni>>(sr.ReadToEnd(), new JsonSerializerSettings
        //            {
        //                NullValueHandling = NullValueHandling.Ignore
        //            });

        //            if (pComune != string.Empty)
        //            {
        //                c = comuni.Where(a => a.nome == pComune).First();
        //            }
        //            else if (pProvincia != string.Empty)
        //            {
        //                c = comuni.Where(a => a.provincia.nome == pProvincia).First();
        //            }
        //            else if (pCap != string.Empty)
        //            {
        //                foreach (var j in comuni)
        //                {
        //                    foreach (var x in j.cap.Where(a => a == pCap))
        //                    {
        //                        c.nome = j.nome;
        //                        c.codice = j.codice;
        //                        c.cm = j.cm;
        //                        c.codiceCatastale = j.codiceCatastale;
        //                        c.provincia = j.provincia;
        //                        c.regione = j.regione;
        //                        c.sigla = j.sigla;
        //                        c.zona = j.zona;
        //                        c.cap.Add(x);
        //                    }
        //                }
        //            }

        //            comune = c.nome;
        //            provincia = c.provincia.nome;
        //            cap = c.cap.First();
        //        }

        //        return Json(new { Comune = comune, Provincia = provincia, Cap = cap });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { err = ex.Message });
        //    }
        //}
    }
}
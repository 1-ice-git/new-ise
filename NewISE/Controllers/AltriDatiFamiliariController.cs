﻿using NewISE.DBComuniItalia;
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
                    //adf = dtadf.GetAltriDatiFamiliariFiglio(idFiglio);
                }
                using (dtMaggiorazioniFamiliari dtmc = new dtMaggiorazioniFamiliari())
                {
                    mcm = dtmc.GetMaggiorazioniFamiliaribyFiglio(idFiglio);
                }

                using (dtPercentualeMagFigli dtpmf = new dtPercentualeMagFigli())
                {
                    PercentualeMagFigliModel pf = dtpmf.GetPercentualeMaggiorazioneFigli(idFiglio, DateTime.Now);
                    if (pf != null && pf.HasValue())
                    {
                        switch (pf.idTipologiaFiglio)
                        {
                            case TipologiaFiglio.Minorenne:
                                adf.residente = true;
                                adf.studente = false;
                                break;
                            case TipologiaFiglio.Studente:
                                adf.studente = true;
                                adf.residente = true;
                                break;
                            case TipologiaFiglio.MaggiorenneInabile:
                                adf.studente = false;
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

            ViewData.Add("idMaggiorazioniFamiliari", mcm.idMaggiorazioneFamiliari);

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

            ViewData.Add("idMaggiorazioniFamiliari", mcm.idMaggiorazioneFamiliari);

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
                    return PartialView("ModificaAltriDatiFamiliariConiuge", adfm);
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

            }
            else
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = "Errore nella modifica altri dati familiari" });
            }

            return null;
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
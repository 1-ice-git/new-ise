using NewISE.DBComuniItalia;
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
            MaggiorazioniFigliModel mfm = new MaggiorazioniFigliModel();

            try
            {
                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                {
                    adf = dtadf.GetAltriDatiFamiliariFiglio(idFiglio);
                }
                using (dtMaggiorazioniFigli dtmf = new dtMaggiorazioniFigli())
                {
                    mfm = dtmf.GetMaggiorazioneFigli(idFiglio);

                }

                using (dtPercentualeMagFigli dtpmf = new dtPercentualeMagFigli())
                {
                    PercentualeMagFigliModel pf = dtpmf.GetPercentualeMaggiorazioneFigliNow(idFiglio, DateTime.Now);
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
                return PartialView("ErrorPartial");
            }

            ViewData.Add("idTrasferimento", mfm.idTrasferimento);

            if (adf != null && adf.HasValue())
            {
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

                ViewData.Add("Comuni", comuni);

                return PartialView("InserisciAltriDatiFamiliariConiuge", adf);
            }




        }


        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult AltriDatiFamiliariConiuge(decimal idMaggiorazioneConiuge)
        {
            AltriDatiFamModel adf = new AltriDatiFamModel();
            MaggiorazioneConiugeModel mcm = new MaggiorazioneConiugeModel();

            try
            {
                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                {
                    adf = dtadf.GetAltriDatiFamiliariConiuge(idMaggiorazioneConiuge);
                }
                using (dtMaggiorazioneConiuge dtmc = new dtMaggiorazioneConiuge())
                {
                    mcm = dtmc.GetMaggiorazioneConiuge(idMaggiorazioneConiuge: idMaggiorazioneConiuge);
                }

                using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                {
                    PercentualeMagConiugeModel pc = dtpc.GetPercMagConiugeNow(idMaggiorazioneConiuge, DateTime.Now.Date);

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
                return PartialView("ErrorPartial");
            }

            ViewData.Add("idTrasferimento", mcm.idTrasferimento);

            if (adf != null && adf.HasValue())
            {
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

                ViewData.Add("Comuni", comuni);

                return PartialView("InserisciAltriDatiFamiliariConiuge", adf);
            }
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciAltriDatiFamiliariConiuge(AltriDatiFamModel adf, decimal idTrasferimento)
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
                    ViewData.Add("idTrasferimento", idTrasferimento);

                    return PartialView("InserisciAltriDatiFamiliariConiuge", adf);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }

            return RedirectToAction("AltriDatiFamiliariConiuge", new { idMaggiorazioneConiuge = adf.idMaggiorazioneConiuge });
        }

        public ActionResult ModificaAltriDatiFamiliariConiuge(decimal idAltriDatiFam, decimal idTrasferimento)
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

                            pc = dtpc.GetPercMagConiugeNow(adfm.idMaggiorazioneConiuge.Value, DateTime.Now.Date);

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

                return PartialView("ErrorPartial");
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


            }

            ViewData.Add("Comuni", comuni);
            ViewData.Add("idTrasferimento", idTrasferimento);

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
                return PartialView("ErrorPartial");
            }

            return RedirectToAction("AltriDatiFamiliariConiuge", new { idMaggiorazioneConiuge = adfm.idMaggiorazioneConiuge });
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
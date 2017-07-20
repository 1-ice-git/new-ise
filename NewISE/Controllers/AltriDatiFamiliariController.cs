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
        [AcceptVerbs(HttpVerbs.Post)]
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
                List<string> lComuni = new List<string>();
                List<string> lProvincie = new List<string>();
                List<string> lCap = new List<string>();

                using (StreamReader sr = new StreamReader(Server.MapPath("~/DBComuniItalia/jsonComuniItalia.json")))
                {
                    comuni = JsonConvert.DeserializeObject<List<Comuni>>(sr.ReadToEnd(), new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }

                ViewData.Add("Comuni", comuni);

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

                return PartialView("InserisciAltriDatiFamiliariConiuge", adf);
            }
        }

        public ActionResult InserisciAltriDatiFamiliari(AltriDatiFamModel adf, decimal idTrasferimento)
        {
            try
            {


                if (ModelState.IsValid)
                {
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
using NewISE.DBComuniItalia;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Configuration;
using System.Web.Mvc;

namespace NewISE.Controllers
{
    public class AltriDatiFamiliariController : Controller
    {
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult AltriDatiFamiliariFiglio(decimal idFiglio, decimal idAttivazioneMagFam)
        {
            AltriDatiFamFiglioModel adf = new AltriDatiFamFiglioModel();
            //MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel();
            //AttivazioniMagFamModel amfm = new AttivazioniMagFamModel();

            try
            {
                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                {
                    adf = dtadf.GetAlttriDatiFamiliariFiglio(idFiglio);
                }

                //using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                //{
                //    amfm = dtamf.GetAttivazioniMagFamById(idAttivazioneMagFam);
                //}


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
                    bool docFormulario = false;

                    bool solaLettura = false;

                    dtmf.SituazioneMagFamPartenza(idAttivazioneMagFam, out rinunciaMagFam,
                        out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                        out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario);

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

            ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);

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

        [HttpPost]
        public ActionResult AltriDatiFamiliariFiglioPassaporto(decimal idFiglio)
        {
            AltriDatiFamFiglioModel adf = new AltriDatiFamFiglioModel();
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();
            TrasferimentoModel tm = new TrasferimentoModel();

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

            using (dtTrasferimento dtt = new dtTrasferimento())
            {
                tm = dtt.GetTrasferimentoByIDMagFam(mcm.idMaggiorazioniFamiliari);
            }

            ViewData.Add("idTrasferimento", tm.idTrasferimento);


            using (dtFigli dtf = new dtFigli())
            {
                if (adf.idFigli.HasValue)
                {
                    var fm = dtf.GetFigliobyID(adf.idFigli.Value);
                    adf.Figli = fm;
                }
            }

            //using (dtConiuge dtc = new dtConiuge())
            //{
            //    if (adf.idConiuge.HasValue)
            //    {
            //        var cm = dtc.GetConiugebyID(adf.idConiuge.Value);
            //        adf.Coniuge = cm;
            //    }
            //}

            return PartialView(adf);
        }


        [HttpPost]
        public ActionResult AltriDatiFamiliariFiglioTitoliViaggio(decimal idFiglio)
        {
            AltriDatiFamFiglioModel adf = new AltriDatiFamFiglioModel();
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();
            TrasferimentoModel tm = new TrasferimentoModel();

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

            using (dtTrasferimento dtt = new dtTrasferimento())
            {
                tm = dtt.GetTrasferimentoByIDMagFam(mcm.idMaggiorazioniFamiliari);
            }

            ViewData.Add("idTrasferimento", tm.idTrasferimento);


            using (dtFigli dtf = new dtFigli())
            {
                if (adf.idFigli.HasValue)
                {
                    var fm = dtf.GetFigliobyID(adf.idFigli.Value);
                    adf.Figli = fm;
                }
            }

            //using (dtConiuge dtc = new dtConiuge())
            //{
            //    if (adf.idConiuge.HasValue)
            //    {
            //        var cm = dtc.GetConiugebyID(adf.idConiuge.Value);
            //        adf.Coniuge = cm;
            //    }
            //}

            return PartialView(adf);
        }


        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult AltriDatiFamiliariConiuge(decimal idConiuge, decimal idAttivazioneMagFam)
        {
            AltriDatiFamConiugeModel adf = new AltriDatiFamConiugeModel();
            //AttivazioniMagFamModel amfm=new AttivazioniMagFamModel();

            try
            {
                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                {
                    adf = dtadf.GetAlttriDatiFamiliariConiuge(idConiuge, idAttivazioneMagFam);
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
                    bool docFormulario = false;

                    bool solaLettura = false;

                    dtmf.SituazioneMagFamPartenza(idAttivazioneMagFam, out rinunciaMagFam,
                        out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                        out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario);

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

            ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);

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
        public ActionResult AltriDatiFamiliariConiugePassaporti(decimal idConiuge)
        {
            AltriDatiFamConiugeModel adf = new AltriDatiFamConiugeModel();
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();
            TrasferimentoModel tm = new TrasferimentoModel();

            try
            {
                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                {
                    //adf = dtadf.GetAlttriDatiFamiliariConiuge(idConiuge);
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
            using (dtTrasferimento dtt = new dtTrasferimento())
            {
                tm = dtt.GetTrasferimentoByIDMagFam(mcm.idMaggiorazioniFamiliari);
            }

            ViewData.Add("idTrasferimento", tm.idTrasferimento);

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



        [HttpPost]
        public ActionResult AltriDatiFamiliariConiugeTitoliViaggio(decimal idConiuge)
        {
            AltriDatiFamConiugeModel adf = new AltriDatiFamConiugeModel();
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();
            TrasferimentoModel tm = new TrasferimentoModel();

            try
            {
                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                {
                    //adf = dtadf.GetAlttriDatiFamiliariConiuge(idConiuge);
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

            using (dtTrasferimento dtt = new dtTrasferimento())
            {
                tm = dtt.GetTrasferimentoByIDMagFam(mcm.idMaggiorazioniFamiliari);
            }

            ViewData.Add("idTrasferimento", tm.idTrasferimento);

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciAltriDatiFamiliariFiglio(AltriDatiFamFiglioModel adf, decimal idAttivazioneMagFam)
        {
            try
            {
                adf.dataAggiornamento = DateTime.Now;
                adf.annullato = false;

                if (ModelState.IsValid)
                {
                    using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                    {
                        dtadf.SetAltriDatiFamiliariFiglio(ref adf, idAttivazioneMagFam);
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
                    ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);
                    using (dtFigli dtf = new dtFigli())
                    {
                        string nominativo = dtf.GetFigliobyID(adf.idFigli.Value).nominativo.ToUpper();
                        ViewData.Add("nominativo", nominativo);
                    }

                    return PartialView("InserisciAltriDatiFamiliariFiglio", adf);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return RedirectToAction("AltriDatiFamiliariFiglio", new { idFiglio = adf.idFigli, idAttivazioneMagFam = idAttivazioneMagFam });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciAltriDatiFamiliariConiuge(AltriDatiFamConiugeModel adf, decimal idAttivazioneMagFam)
        {
            try
            {
                adf.dataAggiornamento = DateTime.Now;
                adf.annullato = false;

                if (ModelState.IsValid)
                {
                    using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                    {
                        dtadf.SetAltriDatiFamiliariConiuge(ref adf, idAttivazioneMagFam);
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
                    ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);

                    return PartialView("InserisciAltriDatiFamiliariConiuge", adf);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return RedirectToAction("AltriDatiFamiliariConiuge", new { idConiuge = adf.idConiuge, idAttivazioneMagFam = idAttivazioneMagFam });
        }

        public ActionResult ModificaAltriDatiFamiliariFiglio(decimal idAltriDatiFam, decimal idAttivazioneMagFam)
        {
            AltriDatiFamFiglioModel adfm = new AltriDatiFamFiglioModel();

            try
            {
                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                {
                    adfm = dtadf.GetAltriDatiFamiliariFiglio(idAltriDatiFam);
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
            ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);

            return PartialView(adfm);
        }

        public ActionResult ModificaAltriDatiFamiliariConiuge(decimal idAltriDatiFam, decimal idAttivazioneMagFam)
        {
            AltriDatiFamConiugeModel adfm = new AltriDatiFamConiugeModel();

            try
            {
                using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                {
                    adfm = dtadf.GetAltriDatiFamiliariConiuge(idAltriDatiFam);
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
            ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);

            return PartialView(adfm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfermaModificaAdfConiuge(AltriDatiFamConiugeModel adfm, decimal idAttivazioneMagFam)
        {
            try
            {
                adfm.dataAggiornamento = DateTime.Now;
                adfm.annullato = false;

                if (ModelState.IsValid)
                {
                    using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                    {
                        dtadf.EditAltriDatiFamiliariConiuge(adfm);
                    }
                }
                else
                {
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
                    ViewData.Add("idAttivazioneMagFam", idAttivazioneMagFam);

                    return PartialView("ModificaAltriDatiFamiliariConiuge", adfm);

                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return RedirectToAction("AltriDatiFamiliariConiuge", new { idConiuge = adfm.idConiuge, idAttivazioneMagFam = idAttivazioneMagFam });


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfermaModificaAdfFiglio(AltriDatiFamFiglioModel adfm)
        {
            try
            {
                adfm.dataAggiornamento = DateTime.Now;
                adfm.annullato = false;

                if (ModelState.IsValid)
                {
                    using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                    {
                        dtadf.EditAltriDatiFamiliariFiglio(adfm);
                    }
                }
                else
                {

                    return PartialView("ModificaAltriDatiFamiliariFiglio", adfm);

                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return RedirectToAction("AltriDatiFamiliariFiglio", new { idFiglio = adfm.idFigli });

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
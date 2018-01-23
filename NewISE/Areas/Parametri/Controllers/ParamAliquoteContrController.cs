using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using NewISE.Models;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace NewISE.Areas.Parametri.Controllers
{
    public class ParamAliquoteContrController : Controller
    {
        // GET: Parametri/ParamAliquoteContr/AliquoteContributive

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
       // public ActionResult AliquoteContributive(bool escludiAnnullati, decimal idAliqContr = 0)idLivello
        public ActionResult AliquoteContributive(bool escludiAnnullati, decimal idLivello = 0)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;            
            List<AliquoteContributiveModel> libm = new List<AliquoteContributiveModel>();
            var r = new List<SelectListItem>();
            List<TipoAliquoteContributiveModel> llm = new List<TipoAliquoteContributiveModel>();
            try
            {
                using (dtParTipoAliquoteContributive dtl = new dtParTipoAliquoteContributive())
                {
                    llm = dtl.GetTipoAliquote().OrderBy(a => a.descrizione).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.descrizione,
                                 Value = t.idTipoAliqContr.ToString()

                             }).ToList();

                        if (idLivello == 0)
                        {
                            r.First().Selected = true;
                            idLivello = Convert.ToDecimal(r.First().Value);
                        }
                        else
                        {
                            var temp = r.Where(a => a.Value == idLivello.ToString()).ToList();
                            if (temp.Count == 0)
                            {
                                r.First().Selected = true;
                                idLivello = Convert.ToDecimal(r.First().Value);

                            }
                            else
                                r.Where(a => a.Value == idLivello.ToString()).First().Selected = true;

                            var temp2= dtl.GetTipoAliquote().Where(a => a.idTipoAliqContr == idLivello).ToList();
                            if (temp2.Count == 0)
                            {
                                decimal indice2 = 0;
                                temp2 = dtl.GetTipoAliquote().Where(a => a.idTipoAliqContr == indice2).ToList();
                                while (temp2.Count == 0)
                                {
                                    temp2 = dtl.GetTipoAliquote().Where(a => a.idTipoAliqContr == indice2).ToList();
                                    indice2 = indice2 + 1;                                   
                                }
                            }
                            var lm = temp2.First().descrizione; // dtl.GetTipoAliquote().Where(a => a.idTipoAliqContr == idLivello).First().descrizione;
                            ViewBag.descrizione = lm;
                        }
                    }
                    ViewBag.Aliquote = r;                    
                }

                using (dtAliquoteContr dtib = new dtAliquoteContr())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_AliquoteContributivePrimoNonAnnullato(idLivello);
                    libm = dtib.getListAliquoteContributive(idLivello, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.idAliqContr = idLivello;
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView(libm);
        }

        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public ActionResult AliquoteContributiveLivello(decimal idTipoContributo, bool escludiAnnullati)
        {
            List<AliquoteContributiveModel> libm = new List<AliquoteContributiveModel>();
            var r = new List<SelectListItem>();
            List<TipoAliquoteContributiveModel> llm = new List<TipoAliquoteContributiveModel>();
            ViewBag.idTipoContributo = idTipoContributo;
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                using (dtTipoAliquoteContributive dtl = new dtTipoAliquoteContributive())
                {
                    llm = dtl.GetTipoAliquote().OrderBy(a => a.descrizione).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.descrizione,
                                 Value = t.idTipoAliqContr.ToString()
                             }).ToList();
                        r.Where(a => a.Value == idTipoContributo.ToString()).First().Selected = true;

                        var lm = dtl.GetTipoAliquote(idTipoContributo);
                        ViewBag.descrizione = lm;
                    }
                    ViewBag.Aliquote = r;
                }

                using (dtAliquoteContr dtib = new dtAliquoteContr())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_AliquoteContributivePrimoNonAnnullato(idTipoContributo);
                    libm = dtib.getListAliquoteContributive(llm.Where(a => a.idTipoAliqContr == idTipoContributo).First().idTipoAliqContr, escludiAnnullati).OrderBy(a => a.idTipoContributo).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
            ViewBag.idTipoContributo = idTipoContributo;

            return PartialView("AliquoteContributive", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovaAliquotaContributiva(decimal idTipoContributo, bool escludiAnnullati)
        {
            var r = new List<SelectListItem>();
            try
            {
                using (dtTipoAliquoteContributive dtl = new dtTipoAliquoteContributive())
                {
                    var lm = dtl.GetTipoAliquote(idTipoContributo);
                    ViewBag.descrizione = lm;
                }
                ViewBag.escludiAnnullati = escludiAnnullati;
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
        }

        void DeterminaAliquotePerIlCombo(decimal idTipoContributo)
        {
            var r = new List<SelectListItem>();
            using (dtParTipoAliquoteContributive dtl = new dtParTipoAliquoteContributive())
            {
                var llm = dtl.GetTipoAliquote().OrderBy(a => a.descrizione).ToList();
                if (llm != null && llm.Count > 0)
                {
                    r = (from t in llm
                         select new SelectListItem()
                         {
                             Text = t.descrizione,
                             Value = t.idTipoAliqContr.ToString()
                         }).ToList();
                    r.Where(a => a.Value == idTipoContributo.ToString()).First().Selected = true;
                }                              
            }
            TempData["Aliquote"] = r;
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciAliquoteContributive(AliquoteContributiveModel ibm, bool escludiAnnullati = true,bool aggiornaTutto=false)
        {
            var r = new List<SelectListItem>();
            try
            {
                ibm.dataAggiornamento = DateTime.Now;

                if (ModelState.IsValid)
                {
                    using (dtAliquoteContr dtib = new dtAliquoteContr())
                    {
                        dtib.SetAliquoteContributive(ibm,aggiornaTutto);
                    }
                    DeterminaAliquotePerIlCombo(ibm.idTipoContributo);
                    //  return RedirectToAction("AliquoteContributiveLivello", new { idTipoContributo = ibm.idTipoContributo,escludiAnnullati = escludiAnnullati });            //return RedirectToAction("AliquoteContributive", new { escludiAnnullati = escludiAnnullati, idTipoAliqContr = ibm.idTipoContributo });
                    List<AliquoteContributiveModel> libm = new List<AliquoteContributiveModel>();
                    using (dtAliquoteContr dtib = new dtAliquoteContr())
                    {
                        ViewBag.idMinimoNonAnnullato = dtib.Get_Id_AliquoteContributivePrimoNonAnnullato(ibm.idTipoContributo);
                        libm = dtib.getListAliquoteContributive(ibm.idTipoContributo, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    return PartialView("AliquoteContributive", libm);
                }
                else
                {                    
                    using (dtParTipoAliquoteContributive dtl = new dtParTipoAliquoteContributive())
                    {
                            var lm = dtl.GetTipoAliquote(ibm.idTipoContributo);
                            ViewBag.Livello = lm;
                            ViewBag.descrizione = lm;
                        }
                        ViewBag.escludiAnnullati = escludiAnnullati;
                        return PartialView("NuovaAliquotaContributiva", ibm);
                    }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        // public ActionResult EliminaAliquoteContributive(bool escludiAnnullati, decimal idAliqContr, decimal idTipoAliqContr)
        public ActionResult EliminaAliquoteContributive(bool escludiAnnullati, decimal idTipoContributo, decimal idAliqContr)
        {
            try
            {
                using (dtAliquoteContr dtib = new dtAliquoteContr())
                {
                    //dtib.DelAliquoteContributive(idTipoAliqContr);
                    dtib.DelAliquoteContributive(idAliqContr);//corretto da confermare                    
                }
                var r = new List<SelectListItem>();
                using (dtParTipoAliquoteContributive dtl = new dtParTipoAliquoteContributive())
                {
                    var llm = dtl.GetTipoAliquote().OrderBy(a => a.descrizione).ToList();
                    if (llm != null && llm.Count > 0)
                    {
                       r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.descrizione,
                                 Value = t.idTipoAliqContr.ToString()
                             }).ToList();
                        r.Where(a => a.Value == idTipoContributo.ToString()).First().Selected = true;                       
                    }
                    TempData["Aliquote"] = r;
                }
                // return RedirectToAction("AliquoteContributiveLivello");//, new { escludiAnnullati = escludiAnnullati, idTipoContributo= idTipoContributo });
                List<AliquoteContributiveModel> libm = new List<AliquoteContributiveModel>();
                using (dtAliquoteContr dtib = new dtAliquoteContr())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_AliquoteContributivePrimoNonAnnullato(idTipoContributo);
                    libm = dtib.getListAliquoteContributive(idTipoContributo, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                   
                }
                return PartialView("AliquoteContributive", libm);
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
    }
}
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
    public class ParamCoeffFasciaKmController : Controller
    {
        // GET: Parametri/ParamCoeffFasciaKm/CoefficienteFasciaKm
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        //public ActionResult CoefficienteFasciaKm(bool escludiAnnullati, decimal idDefKm = 0)
        public ActionResult CoefficienteFasciaKm(bool escludiAnnullati, decimal idLivello = 0, decimal idUfficio = 0)
        {
            List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();
            ViewBag.escludiAnnullati = escludiAnnullati;
            try
            {
                //CaricaComboGruppoFKM(idLivello, idUfficio);                
                CaricaComboGruppoFKM(idUfficio, idLivello);
                using (dtParCoefficienteKm dtib = new dtParCoefficienteKm())
                {
                    if (idLivello == 0)
                    {
                        var x = (IEnumerable<SelectListItem>)ViewBag.FasciaKM;
                        if (x.Count() != 0)
                            idLivello = Convert.ToDecimal(x.First().Value);
                    }
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_CoefficienteFasciaKmNonAnnullato(idLivello);
                    libm = dtib.getListCoeffFasciaKm(idLivello, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.escludiAnnullati = escludiAnnullati;
            return PartialView(libm);
        }
        public void CaricaComboFKM(decimal idLivelloGFKM = 0)
        {
            var r_fkm = new List<SelectListItem>();
            using (dtGruppoFKM dtl = new dtGruppoFKM())
            {
                List<DefFasciaKmModel> llf = new List<DefFasciaKmModel>();
                llf = dtl.getListFasciaKM(idLivelloGFKM).ToList();
                if (llf != null && llf.Count > 0)
                {
                    r_fkm = (from t in llf
                             select new SelectListItem()
                             {
                                 Text = t.km,
                                 Value = t.idfKm.ToString()
                             }).ToList();

                    if (idLivelloGFKM == 0)
                    {
                        r_fkm.First().Selected = true;
                        idLivelloGFKM = Convert.ToDecimal(r_fkm.First().Value);
                    }
                    else
                    {
                        var temp = r_fkm.Where(a => a.Value == idLivelloGFKM.ToString()).ToList();
                        if (temp.Count == 0)
                        {
                            r_fkm.First().Selected = true;
                            idLivelloGFKM = Convert.ToDecimal(r_fkm.First().Value);
                        }
                        else
                            r_fkm.Where(a => a.Value == idLivelloGFKM.ToString()).First().Selected = true;
                    }
                }
            }
            ViewBag.FasciaKM = r_fkm;
        }
        public void CaricaComboGruppoFKM(decimal idLivelloGFKM = 0, decimal idLivello_FKM = 0)
        {
            var r = new List<SelectListItem>();
            List<GruppoFKMModel> llg = new List<GruppoFKMModel>();
            using (dtGruppoFKM dtl = new dtGruppoFKM())
            {
                llg = dtl.getListGruppoFKM().OrderBy(a => a.IDGRUPPOFK).ToList();
                if (llg != null && llg.Count > 0)
                {
                    r = (from t in llg
                         select new SelectListItem()
                         {
                             Text = t.leggeFasciaKM,
                             Value = t.IDGRUPPOFK.ToString()
                         }).ToList();

                    if (idLivelloGFKM == 0)
                    {
                        r.First().Selected = true;
                        idLivello_FKM = Convert.ToDecimal(r.First().Value);
                    }
                    else
                    {
                        var temp = r.Where(a => a.Value == idLivelloGFKM.ToString()).ToList();
                        if (temp.Count == 0)
                        {
                            r.First().Selected = true;
                            idLivelloGFKM = Convert.ToDecimal(r.First().Value);

                        }
                        else
                            r.Where(a => a.Value == idLivelloGFKM.ToString()).First().Selected = true;
                    }
                }
                ViewBag.GruppoFKM = r;
                var r_fkm = new List<SelectListItem>();
                if (r.Count != 0)
                {
                    List<DefFasciaKmModel> llf = new List<DefFasciaKmModel>();
                    if (idLivelloGFKM == 0)
                    {
                        IEnumerable<SelectListItem> GruppoFKM = (IEnumerable<SelectListItem>)ViewBag.GruppoFKM;
                        idLivelloGFKM = Convert.ToDecimal(GruppoFKM.Where(x => x.Selected).FirstOrDefault().Value);
                    }
                    llf = dtl.getListFasciaKM(idLivelloGFKM).ToList();
                    r_fkm = (from t in llf
                             select new SelectListItem()
                             {
                                 Text = t.km,
                                 Value = t.idfKm.ToString()
                             }).ToList();

                    if (idLivello_FKM == 0)
                    {
                        r_fkm.First().Selected = true;
                        idLivello_FKM = Convert.ToDecimal(r_fkm.First().Value);
                    }
                    else
                    {
                        var temp = r_fkm.Where(a => a.Value == idLivello_FKM.ToString()).ToList();
                        if (temp.Count == 0)
                        {
                            r_fkm.First().Selected = true;
                            idLivello_FKM = Convert.ToDecimal(r_fkm.First().Value);

                        }
                        else
                            r_fkm.Where(a => a.Value == idLivello_FKM.ToString()).First().Selected = true;
                    }
                }
                ViewBag.FasciaKM = r_fkm;
            }
        }
        public void CaricaComboGruppoFKM(decimal idLivelloGFKM = 0)
        {
            var r = new List<SelectListItem>();
            List<GruppoFKMModel> llg = new List<GruppoFKMModel>();
            using (dtGruppoFKM dtl = new dtGruppoFKM())
            {
                llg = dtl.getListGruppoFKM().OrderBy(a => a.IDGRUPPOFK).ToList();
                if (llg != null && llg.Count > 0)
                {
                    r = (from t in llg
                         select new SelectListItem()
                         {
                             Text = t.leggeFasciaKM,
                             Value = t.IDGRUPPOFK.ToString()
                         }).ToList();

                    if (idLivelloGFKM == 0)
                    {
                        r.First().Selected = true;
                        idLivelloGFKM = Convert.ToDecimal(r.First().Value);
                    }
                    else
                    {
                        var temp = r.Where(a => a.Value == idLivelloGFKM.ToString()).ToList();
                        if (temp.Count == 0)
                        {
                            r.First().Selected = true;
                            idLivelloGFKM = Convert.ToDecimal(r.First().Value);

                        }
                        else
                            r.Where(a => a.Value == idLivelloGFKM.ToString()).First().Selected = true;
                    }
                }
                ViewBag.GruppoFKM = r;
            }
        }
        //AggiornaListaFasciaKmDalGruppo
        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public ActionResult AggiornaListaFasciaKmDalGruppo(decimal idGruppoFKm, bool escludiAnnullati = true)//,decimal idLivello_FKM=0)
        {

            List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();
            var r = new List<SelectListItem>();
            List<DefFasciaKmModel> llm = new List<DefFasciaKmModel>();
            try
            {

                CaricaComboGruppoFKM(idGruppoFKm, 0);
                IEnumerable<SelectListItem> FasciaKM = (IEnumerable<SelectListItem>)ViewBag.FasciaKM;
                decimal idLivello_FKM = Convert.ToDecimal(FasciaKM.Where(x => x.Selected).FirstOrDefault().Value);
                using (dtParCoefficienteKm dtib = new dtParCoefficienteKm())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_CoefficienteFasciaKmNonAnnullato(idLivello_FKM);
                    libm = dtib.getListCoeffFasciaKm(idLivello_FKM, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("CoefficienteFasciaKm", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1 ,2")]
        public ActionResult CoefficienteFasciaKmLivello(decimal idGruppoFKm, decimal idFKm, bool escludiAnnullati)
        {
            List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();
            var r = new List<SelectListItem>();
            List<DefFasciaKmModel> llm = new List<DefFasciaKmModel>();
            try
            {
                CaricaComboGruppoFKM(idGruppoFKm, idFKm);
                using (dtParCoefficienteKm dtib = new dtParCoefficienteKm())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_CoefficienteFasciaKmNonAnnullato(idFKm);
                    libm = dtib.getListCoeffFasciaKm(idFKm, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("CoefficienteFasciaKm", libm);
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult NuovoCoeffFasciakm(decimal idGruppoFKm, bool escludiAnnullati, decimal idFKm = 0)
        {
            CoeffFasciaKmModel ibm = new CoeffFasciaKmModel();
            try
            {
                CaricaComboGruppoFKM(idGruppoFKm, idFKm);
                //ibm.idCfKm = idGruppoFKm;ibm.idDefKm = idFKm;ibm.dataFineValidita = null;ibm.coefficienteKm = 0;
                ViewBag.escludiAnnullati = escludiAnnullati;
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciCoeffFasciaKm(CoeffFasciaKmModel ibm, bool escludiAnnullati = true, bool aggiornaTutto = false, decimal idGruppoFKm = 0, decimal id_DefKm = 0)
        {
            ViewBag.escludiAnnullati = escludiAnnullati;
            var r = new List<SelectListItem>();
            List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();
            List<DefFasciaKmModel> llm = new List<DefFasciaKmModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    ibm.idDefKm = id_DefKm;
                    using (dtParCoefficienteKm dtib = new dtParCoefficienteKm())
                    {
                        dtib.SetCoeffFasciaKm(ibm, aggiornaTutto);
                    }
                    CaricaComboGruppoFKM(idGruppoFKm, id_DefKm);
                    using (dtParCoefficienteKm dtib = new dtParCoefficienteKm())
                    {
                        ViewBag.idMinimoNonAnnullato = dtib.Get_Id_CoefficienteFasciaKmNonAnnullato(id_DefKm);
                        libm = dtib.getListCoeffFasciaKm(id_DefKm, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    return PartialView("CoefficienteFasciaKm", libm);
                    //return RedirectToAction("CoefficienteFasciaKm", new { escludiAnnullati = escludiAnnullati, idDefKm = ibm.idDefKm });
                }
                else
                {
                    CaricaComboGruppoFKM(idGruppoFKm, id_DefKm);
                    ViewBag.escludiAnnullati = escludiAnnullati;
                    return PartialView("NuovoCoeffFasciakm", ibm);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public ActionResult EliminaCoeffFasciaKm(bool escludiAnnullati, decimal idCfKm, decimal idDefKm, decimal idGruppoFKm)
        {
            try
            {
                using (dtParCoefficienteKm dtib = new dtParCoefficienteKm())
                {
                    dtib.DelCoeffFasciaKm(idCfKm);
                }
                List<CoeffFasciaKmModel> libm = new List<CoeffFasciaKmModel>();
                CaricaComboGruppoFKM(idGruppoFKm, idDefKm);
                using (dtParCoefficienteKm dtib = new dtParCoefficienteKm())
                {
                    ViewBag.idMinimoNonAnnullato = dtib.Get_Id_CoefficienteFasciaKmNonAnnullato(idDefKm);
                    libm = dtib.getListCoeffFasciaKm(idDefKm, escludiAnnullati).OrderBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                }

                ViewBag.escludiAnnullati = escludiAnnullati;

                return PartialView("CoefficienteFasciaKm", libm);
                //return RedirectToAction("CoefficienteFasciaKm", new { escludiAnnullati = escludiAnnullati, idDefKm = idDefKm });
            }
            catch (Exception ex)
            {

                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }
    }
}
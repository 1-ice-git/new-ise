using NewISE.Areas.Parametri.Models;
using NewISE.Areas.Parametri.Models.dtObj;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Areas.Parametri.Controllers
{
    public class CoefficientiSedeController : Controller
    {
      
        public ActionResult CoefficientiSede(bool escludiAnnullati)
        {
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();
            
            var r = new List<SelectListItem>();
            List<UfficiModel> llm = new List<UfficiModel>();

            try
            {
                using (dtUffici dtl = new dtUffici())
                {
                    llm = dtl.GetUffici().OrderBy(a => a.descrizioneUfficio).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.descrizioneUfficio,
                                 Value = t.idUfficio.ToString()
                             }).ToList();
                        r.First().Selected = true;
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtCoefficientiSede dtib = new dtCoefficientiSede())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListCoefficientiSede(llm.First().idUfficio, escludiAnnullati).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                       libm = dtib.getListCoefficientiSede(llm.First().idUfficio).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }

            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView(libm);

        }

      
        public ActionResult CoefficienteSedeLivello(decimal idUfficio, bool escludiAnnullati)
        {
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();
            var r = new List<SelectListItem>();
            List<UfficiModel> llm = new List<UfficiModel>();

            try
            {
                using (dtUffici dtl = new dtUffici())
                {
                    llm = dtl.GetUffici().OrderBy(a => a.descrizioneUfficio).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.descrizioneUfficio,
                                 Value = t.idUfficio.ToString()
                             }).ToList();
                        r.Where(a => a.Value == idUfficio.ToString()).First().Selected = true;
                    }

                    ViewBag.LivelliList = r;
                }

                using (dtCoefficientiSede dtib = new dtCoefficientiSede())
                {
                    if (escludiAnnullati)
                    {
                        escludiAnnullati = false;
                        libm = dtib.getListCoefficientiSede(llm.Where(a => a.idUfficio == idUfficio).First().idUfficio, escludiAnnullati).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                    else
                    {
                        libm = dtib.getListCoefficientiSede(llm.Where(a => a.idUfficio == idUfficio).First().idUfficio).OrderBy(a => a.idUfficio).ThenBy(a => a.dataInizioValidita).ThenBy(a => a.dataFineValidita).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }
            ViewBag.escludiAnnullati = escludiAnnullati;

            return PartialView("CoefficientiSede", libm);
        }

      
        public ActionResult NuovoCoefficienteSede(decimal idUfficio)
        {

            var r = new List<SelectListItem>();
            try
            {
                using (dtUffici dtl = new dtUffici())
                {
                    var lm = dtl.GetUffici(idUfficio);
                    ViewBag.DescrizioneUfficio = lm;
                }

                return PartialView();

            }
            catch (Exception)
            {

                return PartialView("ErrorPartial");
            }
           
        }



    }
}
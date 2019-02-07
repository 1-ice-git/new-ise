using NewISE.Areas.Statistiche.Models.dtObj;
using NewISE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Areas.Statistiche.Controllers
{
    public class LogAttivitaController : Controller
    {
        // GET: Statistiche/LogAttivita

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult LogAttivita2(decimal idDipendente = 0)
        {

            List<LogAttivitaModel> libm = new List<LogAttivitaModel>();
            var r = new List<SelectListItem>();
            List<UtenteAutorizzatoModel> llm = new List<UtenteAutorizzatoModel>();

            try
            {
                using (dtUtentiAutorizzati dtl = new dtUtentiAutorizzati())
                {
                    llm = dtl.GetUtentiAutorizzati().OrderBy(a => a.matricola).ToList();

                    if (llm != null && llm.Count > 0)
                    {
                        r = (from t in llm
                             select new SelectListItem()
                             {
                                 Text = t.matricola,
                                 Value = t.idDipendente.ToString()

                             }).ToList();

                        if (idDipendente == 0)
                        {
                            r.First().Selected = true;
                            idDipendente = Convert.ToDecimal(r.First().Value);
                        }
                        else
                        {
                            r.Where(a => a.Value == idDipendente.ToString()).First().Selected = true;
                        }

                    }

                    ViewBag.LivelliList = r;
                }

                using (dtLogAttivita dtib = new dtLogAttivita())
                {
                    libm = dtib.getListLogAttivita().OrderBy(a => a.idLog).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }


            return PartialView("LogAttivita2", libm);

        }

    }
}
using NewISE.EF;
using NewISE.Models;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Controllers
{
    
    public class HomeController : Controller
    {
        // GET: Home
        bool admin = false;
        public ActionResult Index()
        {
            try
            {
                admin = Utility.Amministratore();
                ViewBag.Amministratore = admin;
            }
            catch (Exception)
            {
                return View("Error");
            }
            return View();
        }
        
        public ActionResult MsgUtente(int idUtente)
        {
            return null;
        }
        public ActionResult GetListaHome()
        {            
            List<ElencoElementiHome> tmp = new List<ElencoElementiHome>();
            try
            {
                using (var dbContext = new ModelDBISE())
                {
                    tmp = (from d in dbContext.DIPENDENTI
                           join t in dbContext.TRASFERIMENTO on d.IDDIPENDENTE equals t.IDDIPENDENTE
                           join c in dbContext.CALENDARIOEVENTI on t.IDTRASFERIMENTO equals c.IDTRASFERIMENTO
                           join f in dbContext.FUNZIONIEVENTI on c.IDFUNZIONIEVENTI equals f.IDFUNZIONIEVENTI
                           where c.ANNULLATO == false && c.COMPLETATO == false && c.DATAINIZIOEVENTO.Month == DateTime.Now.Month
                           && c.DATAINIZIOEVENTO.Year == DateTime.Now.Year && c.COMPLETATO == false
                           select new ElencoElementiHome
                           {
                               Nominativo = d.COGNOME + " " + d.NOME,
                               dataInizio = c.DATAINIZIOEVENTO,
                               dataScadenza = c.DATASCADENZA,
                               NomeFunzione = f.NOMEFUNZIONE,
                               Completato = c.COMPLETATO,
                           }).ToList();
                }               
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView(tmp);
        }
    }
}
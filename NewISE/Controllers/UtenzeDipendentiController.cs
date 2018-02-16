using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Controllers
{
    public class UtenzeDipendentiController : Controller
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
        [Authorize(Roles = "1 ,2")]
        public ActionResult ElencoUtenzeDipendenti()
        {
            List<DipendentiModel> tmp = new List<DipendentiModel>();
            List<UtentiAutorizzatiModel> tmp2 = new List<UtentiAutorizzatiModel>();
            try
            {
                using (dtUtenzeDipendenti dtcal = new dtUtenzeDipendenti())
                {
                    tmp2 = dtcal.GetDipendentiAutorizzati();
                    tmp = dtcal.GetUtenzeAutorizzate(tmp2).OrderBy(b => b.cognome).ToList();
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView(tmp);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ElencoEmailSecondarioDip(decimal idd)
        {
            List<EmailSecondarieDipModel> tmp = new List<EmailSecondarieDipModel>();
            DipendentiModel tmp2 = new DipendentiModel();
            try
            {
                using (dtUtenzeDipendenti dtcal = new dtUtenzeDipendenti())
                {
                    tmp = dtcal.GetListaEmailSecondarioDip(idd);
                    tmp2 = dtcal.GetDatiUtenzaAutorizzata(idd);
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            ViewBag.emailPrincipale = ""; ViewBag.idDipendente = "0";
            if (tmp2 != null)
            {
                ViewBag.emailPrincipale = tmp2.email;
                ViewBag.idDipendente = tmp2.idDipendente;
            }
            return PartialView(tmp);
        }

        public ActionResult VisualizzaEmail(decimal idd)
        {
            ViewBag.idDipendente = idd;
            DipendentiModel tmp2 = new DipendentiModel();
            using (dtUtenzeDipendenti dtcal = new dtUtenzeDipendenti())
            {
                tmp2 = dtcal.GetDatiUtenzaAutorizzata(idd);
                ViewBag.abilitato = tmp2.abilitato;
            }
            return PartialView(tmp2);
        }
        
        public ActionResult ModificaStatoDipendente(bool abilitato, decimal idDipendente)
        {
            List<DipendentiModel> tmp = new List<DipendentiModel>();
            List<UtentiAutorizzatiModel> tmp2 = new List<UtentiAutorizzatiModel>();
            using (dtUtenzeDipendenti dtcal = new dtUtenzeDipendenti())
            {
                dtcal.EditStatoDipendente(abilitato,idDipendente);
                tmp2 = dtcal.GetDipendentiAutorizzati();
                tmp = dtcal.GetUtenzeAutorizzate(tmp2).OrderBy(b => b.cognome).ToList();
            }
            return PartialView("ElencoUtenzeDipendenti", tmp);
        }
        public ActionResult EliminaEmail(decimal idEmailSec,decimal idd)
        {
            List<EmailSecondarieDipModel> tmp = new List<EmailSecondarieDipModel>();
            using (dtUtenzeDipendenti dtcal = new dtUtenzeDipendenti())
            {
                dtcal.DelEmail(idEmailSec);               
                tmp = dtcal.GetListaEmailSecondarioDip(idd);
                ViewBag.idDipendente = idd;
            }
            return PartialView("ElencoEmailSecondarioDip", tmp);
        }
        public ActionResult AggiungiEmail(string newmail,decimal idd)
        {
            List<EmailSecondarieDipModel> tmp = new List<EmailSecondarieDipModel>();
            bool ok = false;
            using (dtUtenzeDipendenti dtcal = new dtUtenzeDipendenti())
            {
                ok=dtcal.AddMail(newmail, idd);
                tmp = dtcal.GetListaEmailSecondarioDip(idd);
                ViewBag.idDipendente = idd;
            }
            return PartialView("ElencoEmailSecondarioDip", tmp);
        }
    }
}
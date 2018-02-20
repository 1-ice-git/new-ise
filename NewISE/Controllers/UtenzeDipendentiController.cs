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
        [Authorize(Roles = "1 ,2")]
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
                AggiornaViewBag(tmp2.idDipendente);
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

        public ActionResult VisualizzaEmailSecondaria(decimal idEmailSec,decimal idDipendente)
        {
            ViewBag.idDipendente = idDipendente;
            ViewBag.idEmailSec = idEmailSec;
            EmailSecondarieDipModel tmp2 = new EmailSecondarieDipModel();
            using (dtUtenzeDipendenti dtcal = new dtUtenzeDipendenti())
            {
                tmp2 = dtcal.GetDatiEmailSecondaria(idEmailSec);
                ViewBag.attivato = tmp2.Attiva;
            }
            ViewBag.Attiva = tmp2.Attiva;
            return PartialView(tmp2);
        }

        public ActionResult NuovaEmail(decimal idd)
        {
            ViewBag.idDipendente = idd;
            try
            {
                DipendentiModel tmp2 = new DipendentiModel();
                using (dtUtenzeDipendenti dtcal = new dtUtenzeDipendenti())
                {
                    tmp2 = dtcal.GetDatiUtenzaAutorizzata(idd);
                }
                ViewBag.abilitato = tmp2.abilitato;
                ViewBag.emailPrincipale= tmp2.email;
                return PartialView();          
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
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
                AggiornaViewBag(idDipendente);
            }
            return PartialView("ElencoUtenzeDipendenti", tmp);
        }
        //
        public ActionResult ModificaStatoEmailSecondaria(bool attiva, decimal idEmailSec,decimal idDipendente)
        {
            ViewBag.idEmailSec = idEmailSec;
            List<EmailSecondarieDipModel> tmp = new List<EmailSecondarieDipModel>();
            //List<UtentiAutorizzatiModel> tmp2 = new List<UtentiAutorizzatiModel>();
            using (dtUtenzeDipendenti dtcal = new dtUtenzeDipendenti())
            {
                dtcal.EditStatoEmailSecondaria(attiva, idEmailSec);
                tmp = dtcal.GetListaEmailSecondarioDip(idDipendente);
                //  tmp = dtcal.GetUtenzeAutorizzate(tmp2).OrderBy(b => b.cognome).ToList();
            }
            AggiornaViewBag(idDipendente);
            return PartialView("ElencoEmailSecondarioDip", tmp);
        }
        public ActionResult EliminaEmail(decimal idEmailSec,decimal idd)
        {
            List<EmailSecondarieDipModel> tmp = new List<EmailSecondarieDipModel>();
            using (dtUtenzeDipendenti dtcal = new dtUtenzeDipendenti())
            {
                dtcal.DelEmail(idEmailSec);               
                tmp = dtcal.GetListaEmailSecondarioDip(idd);
                ViewBag.idDipendente = idd;
                AggiornaViewBag(idd);
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
                AggiornaViewBag(idd);
            }
            //   return PartialView("ElencoEmailSecondarioDip", tmp);
            //  return RedirectToAction("ElencoEmailSecondarioDip",new {idDipendente= idd });
            return Json(new { success = true });
        }
        void AggiornaViewBag(decimal idd)
        {
            using (dtUtenzeDipendenti dtcal = new dtUtenzeDipendenti())
            {
                DipendentiModel tmp2 = new DipendentiModel();
                tmp2 = dtcal.GetDatiUtenzaAutorizzata(idd);
                ViewBag.abilitato = tmp2.abilitato;
                ViewBag.emailPrincipale = tmp2.email;
                ViewBag.idDipendente = idd;
            }
        }
        
        public ActionResult InserisciEmailSecondaria(EmailSecondarieDipModel esdm)
        {
            ViewBag.idDipendente = esdm.idDipendente;
            List<EmailSecondarieDipModel> tmp = new List<EmailSecondarieDipModel>();
            bool ok = false;
            if (ModelState.IsValid)
            {
                using (dtUtenzeDipendenti dtcal = new dtUtenzeDipendenti())
                {
                    ok = dtcal.AddMail(esdm.Email, esdm.idDipendente);
                    tmp = dtcal.GetListaEmailSecondarioDip(esdm.idDipendente);
                    AggiornaViewBag(esdm.idDipendente);
                }
                return PartialView("ElencoEmailSecondarioDip", tmp);
            }
            else
            {
                return PartialView("NuovaEmail", esdm);
               //return Json(new { esistente = ok });
            }
        }
        public JsonResult EsisteEmailSecondarie(decimal idDipendente, string email)
        {
            bool ok = false;
            using (dtUtenzeDipendenti dtcal = new dtUtenzeDipendenti())
            {
                ok = dtcal.EmailSecondariaGiaInserita(idDipendente, email);
            }
            return Json(new { success = !ok });
        }
        public JsonResult EmailValida(string email)
        {
            bool ok = false;
            using (dtUtenzeDipendenti dtcal = new dtUtenzeDipendenti())
            {
                ok = dtcal.emailIsValid(email);
            }
            return Json(new { success = ok });
        }

    }
}
using NewISE.Models.Config;
using NewISE.Models.Config.s_admin;
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
    public class NotificheController : Controller
    {
        // GET: Notifiche
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
        public ActionResult ListaNotifiche()
        {
            decimal idMittenteLogato = Utility.UtenteAutorizzato().idDipendente;
            List<NotificheModel> nm = new List<NotificheModel>();
            using (dtNotifiche dn = new dtNotifiche())
            {
                nm = dn.GetNotifiche(idMittenteLogato).ToList();
            }

            return PartialView(nm);
        }
        public ActionResult NuovaNotifica()
        {
            var r = new List<SelectListItem>();
            using (Config cfg = new Config())
            {
                sAdmin sad = new sAdmin();
                sUtenteNormale utentiNormali = new sUtenteNormale();
                sad = cfg.SuperAmministratore();
                if (sad.s_admin.Count > 0)
                {
                    r = (from t in sad.s_admin
                         select new SelectListItem()
                         {
                             Text = t.email,
                             Value = t.email,
                         }).ToList();
                    //r.Where(a => a.Value == idTipoContributo.ToString()).First().Selected = true;
                }
            }
            ViewBag.ListaDestinatari = r;
            return PartialView();
        }
        public ActionResult VisualizzaCorpoMessaggio(decimal idNotifica)
        {
            decimal idMittenteLogato = Utility.UtenteAutorizzato().idDipendente;
            List<NotificheModel> nm = new List<NotificheModel>();
            NotificheModel elem = new NotificheModel();
            using (dtNotifiche dn = new dtNotifiche())
            {
                nm = dn.GetNotifiche(idMittenteLogato).ToList();
                if (nm.Count() > 0)
                {
                    elem = nm.First();
                }
            }
            return PartialView(elem);
        }

        public ActionResult InserisciNuovaNotifica(NotificheModel nm)
        {

            return PartialView();
        }
    }
}
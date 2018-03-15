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
using System.Web.Helpers;
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
                nm = dn.GetNotifiche(idMittenteLogato).OrderBy(x=>x.dataNotifica).ToList();
            }
            return PartialView(nm);
        }
        public ActionResult NuovaNotifica()
        {
            var r = new List<SelectListItem>();
            var r0 = new List<SelectListItem>();
            var r2 = new List<SelectListItem>();
            UtentiAutorizzatiModel uta = null;
            List<DipendentiModel> dm = new List<DipendentiModel>();
            decimal idMittenteLogato = Utility.UtenteAutorizzato().idDipendente;
            using (dtNotifiche dtn = new dtNotifiche())
            {
                uta = dtn.RestituisciAutorizzato(idMittenteLogato);

                if (uta.idRouloUtente == 2) dm.AddRange(dtn.GetListaDipendentiAutorizzati(3));                      
                if (uta.idRouloUtente == 3) dm.AddRange(dtn.GetListaDipendentiAutorizzati(2));
                
                if (dm.Count > 0)
                {
                    var agg = new SelectListItem(); agg.Text = "TUTTI"; agg.Value = "TUTTI";
                    r.Add(agg);
                    r0 = (from t in dm where !string.IsNullOrEmpty(t.email) && t.email.Trim()!="" orderby t.nome
                        select new SelectListItem()
                         {
                             Text = t.nome + " " + t.cognome,
                             Value = t.email,
                         }).ToList();
                    r.AddRange(r0);

                    r2 = (from t in dm
                         where !string.IsNullOrEmpty(t.email) && t.email.Trim() != "" orderby t.nome
                         select new SelectListItem()
                         {
                             Text = t.nome + " " + t.cognome,
                             Value = t.email,
                         }).ToList();
                }                
            }
            ViewBag.idMittenteLogato = idMittenteLogato;
            ViewBag.ListaDestinatari = r;
            ViewBag.ListaCc = r2;
            return PartialView();
        }
        public ActionResult VisualizzaCorpoMessaggio(decimal idNotifica)
        {
            decimal idMittenteLogato = Utility.UtenteAutorizzato().idDipendente;
            List<NotificheModel> nm = new List<NotificheModel>();
            NotificheModel elem = new NotificheModel();
            using (dtNotifiche dn = new dtNotifiche())
            {
                nm = dn.GetNotifiche(idMittenteLogato).Where(a=>a.idNotifica==idNotifica).ToList();
                if (nm.Count() > 0)
                {
                    elem = nm.First();
                }
            }
            return PartialView(elem);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult InserisciNuovaNotifica(NotificheModel nmod )
        {
            List<NotificheModel> nm = new List<NotificheModel>();
            using (dtNotifiche dn = new dtNotifiche())
            {
                dn.InsertNotifiche(nmod);
                decimal idMittenteLogato = nmod.idMittente;// Utility.UtenteAutorizzato().idDipendente;
                nm = dn.GetNotifiche(idMittenteLogato).ToList();
            }
            return PartialView("ListaNotifiche",nm);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RegistraNotifiche(FormCollection CollectionMessaggio)
        {
            FormCollection fc = new FormCollection(Request.Unvalidated().Form);
            string testo = fc["contenutoMessaggio"];
            return PartialView("ListaNotifiche");
        }
        public ActionResult VisualizzaDestinatari(decimal idNotifica)
        {
            List<DestinatarioModel> nm = new List<DestinatarioModel>();           
            using (dtNotifiche dn = new dtNotifiche())
            {
                nm = dn.GetListDestinatari(idNotifica).ToList();
            }
            return PartialView(nm);
        }
       
    }
}
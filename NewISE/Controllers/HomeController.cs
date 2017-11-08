using NewISE.EF;
using NewISE.Models;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
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
                using (dtCalendarioEventi dtcal = new dtCalendarioEventi())
                {
                    tmp = dtcal.GetListaElementiHome().ToList();
                    //CalendarioEventiModel xx = new CalendarioEventiModel();
                    //xx.Completato = true;
                    //xx.DataCompletato = new DateTime(2019, 1, 1);
                    //xx.DataInizioEvento = DateTime.Now;
                    //xx.DataScadenza = new DateTime(2018, 1, 1);
                    //EnumFunzioniEventi aa = EnumFunzioniEventi.Funzione1;
                    //xx.idFunzioneEventi = aa;
                    //xx.idTrasferimento = 197;
                    //dtcal.InsertCalendarioEvento(ref xx);
                }               
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView(tmp);
        }
        public ActionResult DetailsFunzioneEvento(EnumFunzioniEventi idf,int idd)
        {
            DettagliMessaggio tmp = new DettagliMessaggio();
            try
            {
                using (dtCalendarioEventi dtcal = new dtCalendarioEventi())
                {
                   tmp = dtcal.OgggettoFunzioneEvento(idf,idd);
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


using NewISE.Areas.Dipendenti.Models;
using NewISE.Areas.Dipendenti.Models.DtObj;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Areas.Dipendenti.Controllers
{
    
    
    public class TrasferimentoController : Controller
    {

        #region Metodi privati
        private void ListeComboNuovoTrasf(out List<SelectListItem> lTipoTrasferimento, out List<SelectListItem> lUffici, out List<SelectListItem> lRuoloUfficio, out List<SelectListItem> lTipologiaCoan)
        {
            var r = new List<SelectListItem>();

            using (dtTipoTrasferimento dttt = new dtTipoTrasferimento())
            {
                var ltt = dttt.GetListTipoTrasferimento().OrderBy(a => a.descTipoTrasf).ToList();

                if (ltt != null && ltt.Count > 0)
                {
                    r = (from t in ltt
                         select new SelectListItem()
                         {
                             Text = t.descTipoTrasf,
                             Value = t.idTipoTrasferimento.ToString()
                         }).ToList();
                    r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                }

                lTipoTrasferimento = r;
            }

            using (dtUffici dtl = new dtUffici())
            {
                var llm = dtl.GetUffici().OrderBy(a => a.DescUfficio).ToList();

                if (llm != null && llm.Count > 0)
                {
                    r = (from t in llm
                         select new SelectListItem()
                         {
                             Text = t.DescUfficio,
                             Value = t.idUfficio.ToString()
                         }).ToList();

                    r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                }

                lUffici = r;
            }

            using (dtRuoloUfficio dtru = new dtRuoloUfficio())
            {
                var lru = dtru.GetListRuoloUfficio().OrderBy(a => a.DescrizioneRuolo).ToList();

                if (lru != null && lru.Count > 0)
                {
                    r = (from t in lru
                         select new SelectListItem()
                         {
                             Text = t.DescrizioneRuolo,
                             Value = t.idRuoloUfficio.ToString()
                         }).ToList();

                    r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                }

                lRuoloUfficio = r;
            }

            using (dtTipologiaCoan dttc = new dtTipologiaCoan())
            {
                var ltc = dttc.GetListTipologiaCoan().OrderBy(a => a.descrizione).ToList();

                if (ltc != null && ltc.Count > 0)
                {
                    r = (from t in ltc
                         select new SelectListItem()
                         {
                             Text = t.descrizione,
                             Value = t.idTipoCoan.ToString()
                         }).ToList();

                    r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                }

                lTipologiaCoan = r;
            }
        }
        #endregion



        [Authorize(Roles = "1 ,2")]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult NuovoTrasferimento(string matricola, bool ricaricaInfoTrasf = false)
        {
            //TrasferimentoModel tm = new TrasferimentoModel();
            //dipTrasferimentoModel dtm = new dipTrasferimentoModel();

            var lTipoTrasferimento = new List<SelectListItem>();
            var lUffici = new List<SelectListItem>();
            var lRuoloUfficio = new List<SelectListItem>();
            var lTipologiaCoan = new List<SelectListItem>();

            try
            {
                ListeComboNuovoTrasf(out lTipoTrasferimento, out lUffici, out lRuoloUfficio, out lTipologiaCoan);

                ViewBag.ListTipoTrasferimento = lTipoTrasferimento;
                ViewBag.ListUfficio = lUffici;
                ViewBag.ListRuolo = lRuoloUfficio;
                ViewBag.ListTipoCoan = lTipologiaCoan;

            }
            catch (Exception ex)
            {

                return PartialView("errorPartial"); ;
            }


            ViewBag.ricaricaInfoTrasf = ricaricaInfoTrasf;

            return PartialView();
        }

        [Authorize(Roles = "1 ,2")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InserisciTrasferimento(dipTrasferimentoModel dtm)
        {
            TrasferimentoModel trm;            
            

            if (ModelState.IsValid)
            {
                try
                {
                    using (dtDipTrasferimento dttr = new dtDipTrasferimento())
                    {
                        trm = new TrasferimentoModel()
                        {
                            idTipoTrasferimento = dtm.idTipoTrasferimento,
                            idUfficio = dtm.idUfficio,
                            idStatoTrasferimento = (decimal)EnumStatoTraferimento.Da_Attivare,
                            idDipendente = dtm.idDipendente,
                            idTipoCoan = dtm.idTipoCoan,
                            dataPartenza = dtm.dataPartenza,
                            coan = dtm.coan,
                            protocolloLettera = dtm.protocolloLettera,
                            dataLettera = dtm.dataLettera,
                            dataAggiornamento = DateTime.Now,
                            annullato = false
                        };

                        dttr.SetTrasferimento(trm);

                    }
                }
                catch (Exception ex)
                {

                    var lTipoTrasferimento = new List<SelectListItem>();
                    var lUffici = new List<SelectListItem>();
                    var lRuoloUfficio = new List<SelectListItem>();
                    var lTipologiaCoan = new List<SelectListItem>();

                    ListeComboNuovoTrasf(out lTipoTrasferimento, out lUffici, out lRuoloUfficio, out lTipologiaCoan);

                    ViewBag.ListTipoTrasferimento = lTipoTrasferimento;
                    ViewBag.ListUfficio = lUffici;
                    ViewBag.ListRuolo = lRuoloUfficio;
                    ViewBag.ListTipoCoan = lTipologiaCoan;

                    ModelState.AddModelError("", ex.Message);

                    return PartialView("NuovoTrasferimento", dtm);
                }
                
            }
            else
            {
                var lTipoTrasferimento = new List<SelectListItem>();
                var lUffici = new List<SelectListItem>();
                var lRuoloUfficio = new List<SelectListItem>();
                var lTipologiaCoan = new List<SelectListItem>();

                ListeComboNuovoTrasf(out lTipoTrasferimento, out lUffici, out lRuoloUfficio, out lTipologiaCoan);

                ViewBag.ListTipoTrasferimento = lTipoTrasferimento;
                ViewBag.ListUfficio = lUffici;
                ViewBag.ListRuolo = lRuoloUfficio;
                ViewBag.ListTipoCoan = lTipologiaCoan;

                return PartialView("NuovoTrasferimento", dtm);

            }

            //return RedirectToAction("NuovoTrasferimento", new { matricola =  });
            return null;
        }






    }
}
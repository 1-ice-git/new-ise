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
        // GET: Dipendenti/Trasferimento
        public ActionResult NuovoTrasferimento(string matricola)
        {
            //TrasferimentoModel tm = new TrasferimentoModel();
            TipoTrasferimentoModel ttm = new TipoTrasferimentoModel();

            var r = new List<SelectListItem>();

            try
            {
                using (dtTipoTrasferimento dttt=new dtTipoTrasferimento())
                {
                    var ltt = dttt.GetListTipoTrasferimento().OrderBy(a=>a.tipologiaTrasferimento).ToList();

                    if (ltt!=null && ltt.Count > 0)
                    {
                        r = (from t in ltt
                             select new SelectListItem()
                             {
                                 Text = t.tipologiaTrasferimento,
                                 Value = t.idTipoTrasferimento.ToString()
                             }).ToList();
                        r.Insert(0, new SelectListItem() { Text = "", Value = "" });
                    }

                    ViewBag.ListTipoTrasferimento = r;
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

                    ViewBag.ListUfficio = r;
                }

                using (dtRuoloUfficio dtru = new dtRuoloUfficio())
                {
                    var lru = dtru.GetListRuoloUfficio().OrderBy(a=>a.DescrizioneRuolo).ToList();

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

                    ViewBag.ListRuolo = r;
                }


                using (dtTipologiaCoan dttc=new dtTipologiaCoan())
                {
                    var ltc = dttc.GetListTipologiaCoan().OrderBy(a=>a.descrizione).ToList();

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

                    ViewBag.ListTipoCoan = r;
                }


            }
            catch (Exception ex)
            {

                return PartialView("errorPartial"); ;
            }

            return PartialView();
        }

        public ActionResult NuovoTrasferimentoConDocumento()
        {
            return null;
        }


    }
}
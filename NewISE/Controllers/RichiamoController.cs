using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.MappingViews;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NewISE.EF;
using NewISE.Models;
using NewISE.Models.Tools;
using NewISE.Areas.Parametri.Models.dtObj;

namespace NewISE.Controllers
{
    public class RichiamoController : Controller
    {
        // GET: Richiamo
        public ActionResult Index()
        {
            return View();
            //return PartialView("Richiamo");
        }
        public ActionResult AttivitaRichiamo(decimal idTrasferimento)
        {
            try
            {
                ViewData["idTrasferimento"] = idTrasferimento;
                ViewData["dataPartenza"] = "Inserire qui la Data come idTrasferimento";
                
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }

            return PartialView("AttivitaRichiamo");
        }
                
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        [Authorize(Roles = "1 ,2")]
        public ActionResult ElencoRichiamo(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            ViewData["dataPartenza"] = "Inserire qui la Data come idTrasferimento";


            try
            {
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
        }

        public ActionResult Richiamo(decimal idTrasferimento,decimal idFKm,string dataRichiamo,decimal nuovo=0)
        {
            DateTime dataPartenza = new DateTime();
            try
            {
                ViewData["idFKm"] = idFKm;
                ViewData["idTrasferimento"] = idTrasferimento;
                ViewData["dataRichiamo"] = dataRichiamo;                
                CaricaComboFKM(idFKm,nuovo);
                ViewData["abilitaModifica"] = idFKm;
                using (dtRichiamo dtr=new dtRichiamo())
                {
                    dataPartenza = dtr.Restituisci_DataPartenza(idTrasferimento);
                    //if (nuovo == 1)
                    //{
                        RichiamoModel rcm= dtr.Restituisci_Ultimo_Richiamo(idTrasferimento);
                        ViewData["idRichiamo"] = rcm.IdRichiamo;
                    if (rcm.IdRichiamo != 0)
                    {
                        ViewData["dataRientro"] = rcm.DataRientro;
                        ViewData["dataRichiamo"] = rcm.DataRichiamo;
                    }
                    CaricaComboFKM(rcm.CoeffKm, nuovo, rcm.IdRichiamo);                    
                }
                ViewData["dataPartenza"] = dataPartenza;                
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            //RichiamoModel rm = new RichiamoModel();
            //rm.idTrasferimento = idTrasferimento;
            //rm.DataRichiamo =Convert.ToDateTime(dataRichiamo);
            //rm.DataPartenza = dataPartenza;
            //rm.CoeffKm = idFKm;
            //return PartialView(rm);
            return PartialView();
        }
        public void CaricaComboFKM(decimal idLivelloGFKM = 0,decimal nuovo=0,decimal idRichiamo=0)
        {
            var r_fkm = new List<SelectListItem>();
            SelectListItem el1 = new SelectListItem();
            el1.Value = "0"; el1.Text = "";
            using (dtGruppoFKM dtl = new dtGruppoFKM())
            {
                List<DefFasciaKmModel> llf = new List<DefFasciaKmModel>();
                
                llf = dtl.getListFasciaKM().ToList();
                if (llf != null && llf.Count > 0)
                {
                    r_fkm = (from t in llf
                             select new SelectListItem()
                             {
                                 Text = t.km,
                                 Value = t.idfKm.ToString()
                             }).ToList();

                    if (idLivelloGFKM == 0)
                    {
                        r_fkm.First().Selected = true;
                        idLivelloGFKM = Convert.ToDecimal(r_fkm.First().Value);
                    }
                    else
                    {
                        var temp = r_fkm.Where(a => a.Value == idLivelloGFKM.ToString()).ToList();
                        if (temp.Count == 0)
                        {
                            r_fkm.First().Selected = true;
                            idLivelloGFKM = Convert.ToDecimal(r_fkm.First().Value);
                        }
                        else
                            r_fkm.Where(a => a.Value == idLivelloGFKM.ToString()).First().Selected = true;
                    }
                }
            }
            r_fkm.Insert(0, el1);
            if (nuovo == 0 && idRichiamo==0)
            {
                foreach(var x in r_fkm)
                {
                    x.Selected = false;
                }                        
                r_fkm.Where(a => a.Value == "0").First().Selected = true;
            }
            ViewBag.FasciaKM = r_fkm;
        }
        public JsonResult VerificaRichiamo(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            ViewData["dataPartenza"] = "Inserire qui la Data come idTrasferimento";

            try
            {
                if (idTrasferimento <= 0)
                {
                    throw new Exception(" non valorizzato");
                }
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    dipInfoTrasferimentoModel trm = dtt.GetInfoTrasferimento(idTrasferimento);
                    if (trm != null && (trm.statoTrasferimento == EnumStatoTraferimento.Attivo ||
                        trm.statoTrasferimento == EnumStatoTraferimento.Terminato))
                    {
                        ViewData["idTrasferimento"] = idTrasferimento;

                        return Json(new { VerificaRichiamo = 1 });
                    }
                    else
                    {
                        return Json(new { VerificaRichiamo = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { err = ex.Message });
            }
        }
        public ActionResult DatiTabElencoRichiamo(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            List<RichiamoModel> tmp = new List<RichiamoModel>();
            try
            {
                using (dtRichiamo dtcal = new dtRichiamo())
                {
                    //tmp.AddRange(dtcal.GetLista_Richiamo(idTrasferimento));
                }
            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
            }
            return PartialView(tmp);

        }
        
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult NuovoRichiamo(decimal idTrasferimento)
        {
            ViewData["idTrasferimento"] = idTrasferimento;

            return PartialView();
        }
        public ActionResult MessaggioInserisciRichiamo(decimal idTrasferimento,decimal idFKm,string dataInserita,decimal idRichiamo=0)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            ViewData["idFKm"] = idFKm;
            ViewData["dataInserita"] = dataInserita==null?"":dataInserita.ToString();
            CaricaComboFKM(idFKm,0,idRichiamo);
            using (dtRichiamo dtr = new dtRichiamo())
            {
                DateTime dataPartenza = dtr.Restituisci_DataPartenza(idTrasferimento);
                ViewData["dataPartenza"] = dataPartenza;
            }
            return PartialView();
        }
        public ActionResult MessaggioModificaRichiamo(decimal idTrasferimento, decimal idFKm, string dataInserita, decimal idRichiamo = 0)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            ViewData["idFKm"] = idFKm;
            ViewData["dataInserita"] = dataInserita == null ? "" : dataInserita.ToString();
            CaricaComboFKM(idFKm, idFKm,idRichiamo);
            using (dtRichiamo dtr = new dtRichiamo())
            {
                DateTime dataPartenza = dtr.Restituisci_DataPartenza(idTrasferimento);
                ViewData["dataPartenza"] = dataPartenza;
            }
            return PartialView();
        }
        public JsonResult ConfermaInserisciRichiamo(decimal idTrasferimento, decimal idFasciaFKM,string dataRichiamo)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            ViewData["idFKm"]= idFasciaFKM;
            CaricaComboFKM(idFasciaFKM, idFasciaFKM);
            string errore = "";
            DateTime dataPartenza = new DateTime();
            var lstr = new List<SelectListItem>();
            using (dtRichiamo dtric = new dtRichiamo())
            {
                dataPartenza = dtric.Restituisci_DataPartenza(idTrasferimento);               
            }
            ViewData["dataPartenza"] = dataPartenza; 
            decimal idRichiamo = 0;
            try
            {
                RichiamoModel ri = new RichiamoModel();
                ri.DataRichiamo =Convert.ToDateTime(dataRichiamo);
                ri.DataAggiornamento = DateTime.Now;
                ri.CoeffKm = idFasciaFKM;
                ri.idTrasferimento = idTrasferimento;
                using (dtRichiamo dtric = new dtRichiamo())
                {
                    decimal idCoeffIndRichiamo = dtric.Restituisci_ID_CoeffIndRichiamo_Da_Data(ri);
                    decimal IDPFKM = dtric.Restituisci_ID_PercentualeFKM_Da_Data(ri);
                    var r = new List<SelectListItem>();
                    if (idCoeffIndRichiamo == 0 || IDPFKM == 0)
                    {
                        ViewData["errore"] = "Non esistono coefficenti corrispondenti ai criteri del Richiamo";
                        //  return PartialView("Richiamo");
                        // return PartialView("ErrorPartial", new MsgErr() { msg = "Non esistono coefficenti corrispondenti ai criteri del Richiamo" });
                       errore= "Non esistono coefficenti corrispondenti ai criteri del Richiamo";
                    }
                    ri.IDPFKM = IDPFKM;
                    DateTime DataRientro = Convert.ToDateTime(dataRichiamo).AddDays(-1);
                    ////
                    //idRichiamo = dtric.SetRichiamo(ri, idCoeffIndRichiamo, IDPFKM, out DataRientro);
                    ///
                    ri.DataRientro = DataRientro;

                    //if (idRichiamo != 0)
                    //{
                        ViewData["dataRichiamo"] = ri.DataRichiamo.ToShortDateString();
                        ViewData["dataRientro"] = ri.DataRientro.ToShortDateString();                       
                        ViewData["idRichiamo"] = idRichiamo;

                        if(DataRientro<dataPartenza)
                            errore = "Data Rientro "+DataRientro.ToShortDateString()+" non può essere inferiore alla data Partenza " +dataPartenza.ToShortDateString();
                        else
                        {
                            idRichiamo = dtric.SetRichiamo(ri, idCoeffIndRichiamo, IDPFKM,  DataRientro);
                            ViewData["idRichiamo"] = idRichiamo;
                            errore = "";
                        }
                    //}
                    //else
                    //{
                    //    ViewData["errore"] = "Errore riscontrato nell'inserimento del Richiamo";
                    //    errore="Non esistono coefficenti corrispondenti ai criteri del Richiamo";
                    //}

                }
                lstr = AggiornaViewBag_Lista_Trasferimenti(idTrasferimento);
            }
            catch (Exception ex)
            {
                ViewData["errore"] = ex.Message;
               // return PartialView("Richiamo");
              //  return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
                errore = ex.Message;
            }
            return Json(new { err = errore,list= lstr });
        }
        public List<SelectListItem> AggiornaViewBag_Lista_Trasferimenti(decimal idTrasferimento)
        {
            var r = new List<SelectListItem>();
            using (dtRichiamo dtric = new dtRichiamo())
            {
                decimal matricola = dtric.GetMatricolaDaIdTrasferimento(idTrasferimento);
                // var eltr = dtric.GetListaTrasferimentoPerRichiamo(matricola);
                if (matricola > 0)
                {
                    var lt = dtric.GetListaTrasferimentoPerRichiamo(matricola);
                    if (lt?.Any() ?? false)
                    {
                        r = (from e in lt
                             select new SelectListItem()
                             {
                                 Text = e.Ufficio.descUfficio + " (" + e.Ufficio.codiceUfficio + ")" + " - " + e.dataPartenza.ToShortDateString() + " ÷ " + ((e.dataRientro.HasValue == true && e.dataRientro < Utility.DataFineStop()) ? e.dataRientro.Value.ToShortDateString() : "--/--/----"),
                                 Value = e.idTrasferimento.ToString()
                             }).ToList();

                        if (idTrasferimento == 0)
                        {
                            r.First().Selected = true;
                        }
                        else
                        {
                            r.First(a => a.Value == idTrasferimento.ToString()).Selected = true;
                        }
                    }
                }               
            }
            return r;
        }
        public JsonResult ConfermaModificaRichiamo(decimal idTrasferimento, decimal idFasciaFKM, string dataRichiamo,decimal idRichiamo)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            ViewData["idFKm"] = idFasciaFKM;
            DateTime dataPartenza = new DateTime();
            CaricaComboFKM(idFasciaFKM, idFasciaFKM);
            string errore = "";var lstr=new List<SelectListItem>();
            using (dtRichiamo dtric = new dtRichiamo())
            {
                dataPartenza = dtric.Restituisci_DataPartenza(idTrasferimento);               
            }
            ViewData["dataPartenza"] = dataPartenza;
            try
            {
                RichiamoModel ri = new RichiamoModel();
                ri.DataRichiamo = Convert.ToDateTime(dataRichiamo);
                ri.DataAggiornamento = DateTime.Now;
                ri.CoeffKm = idFasciaFKM;
                ri.idTrasferimento = idTrasferimento;
                using (dtRichiamo dtric = new dtRichiamo())
                {
                    decimal idCoeffIndRichiamo = dtric.Restituisci_ID_CoeffIndRichiamo_Da_Data(ri);
                    decimal IDPFKM = dtric.Restituisci_ID_PercentualeFKM_Da_Data(ri);
                    if (idCoeffIndRichiamo == 0 || IDPFKM == 0)
                    {
                        errore = "Non esistono coefficenti corrispondenti ai criteri del Richiamo";
                        ViewData["errore"] = errore;
                    }
                    ri.IDPFKM = IDPFKM;
                    DateTime DataRientro = Convert.ToDateTime(dataRichiamo).AddDays(-1); 
                    ////
                    //idRichiamo= dtric.EditRichiamo(ri, idCoeffIndRichiamo, IDPFKM, out DataRientro, idRichiamo);
                    ///
                    ri.DataRientro = DataRientro;

                    if (idRichiamo != 0)
                    {
                        ViewData["dataRichiamo"] = ri.DataRichiamo.ToShortDateString();
                        ViewData["dataRientro"] = ri.DataRientro.ToShortDateString();
                        ViewData["idRichiamo"] = idRichiamo;

                        if (DataRientro < dataPartenza)
                            errore = "Data Rientro (" + DataRientro.ToShortDateString()+ ") non può essere inferiore alla data Partenza (" +dataPartenza.ToShortDateString()+" )";
                        else
                        { 
                            idRichiamo = dtric.EditRichiamo(ri, idCoeffIndRichiamo, IDPFKM, DataRientro, idRichiamo);
                            errore = "";
                        }             
                    }
                    else
                    {
                        ViewData["errore"] = "Errore riscontrato nell'inserimento del Richiamo";
                        errore = "Non esistono coefficenti corrispondenti ai criteri del Richiamo";
                        //  return PartialView("Richiamo");
                        // return PartialView("ErrorPartial", new MsgErr() { msg = "Errore riscontrato nell'inserimento del Richiamo" });
                    }
                }
                lstr = AggiornaViewBag_Lista_Trasferimenti(idTrasferimento);
            }
            catch (Exception ex)
            {
                ViewData["errore"] = ex.Message;
                // return PartialView("Richiamo");
                //  return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
                errore = ex.Message;
            }
            return Json(new { err = errore,list= lstr });
        }
        public JsonResult ControllaDataValida(string dataDaControllare)
        {
            try
            {
                DateTime Temp;
                if (DateTime.TryParse(dataDaControllare, out Temp) == true)
                    return Json(new { errore ="", msg = "" });
                else
                    return Json(new { errore = "Data non valida", msg = "Data non valida" });
            }
            catch (Exception ex)
            {
                return Json(new { errore = ex.Message, msg = ex.Message });
            }
        }
    }
}


            
           
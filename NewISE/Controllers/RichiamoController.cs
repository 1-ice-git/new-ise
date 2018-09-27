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
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using System.IO;
using NewISE.Models.Enumeratori;

namespace NewISE.Controllers
{
    public class RichiamoController : Controller
    {
        //private object dtric;

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

        [Authorize(Roles = "1 ,2")]
        public ActionResult Richiamo(decimal idTrasferimento, decimal idFKm, string dataRichiamo, decimal nuovo = 0)
        {
            DateTime dataPartenza = new DateTime();
            bool admin = Utility.Amministratore();
            ViewBag.Amministratore = admin;
            try
            {
                ViewData["idFKm"] = idFKm;
                ViewData["idTrasferimento"] = idTrasferimento;
                ViewData["dataRichiamo"] = dataRichiamo;
                CaricaComboFKM(idFKm, nuovo);
                ViewData["abilitaModifica"] = idFKm;
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtRichiamo dtr = new dtRichiamo())
                    {
                        dataPartenza = dtr.Restituisci_DataPartenza(idTrasferimento, db);
                        RichiamoModel rcm = dtr.Restituisci_Ultimo_Richiamo(idTrasferimento);
                        ViewData["idRichiamo"] = rcm.IdRichiamo;
                        if (rcm.IdRichiamo != 0)
                        {
                            ViewData["dataRientro"] = rcm.DataRientro;
                            ViewData["dataRichiamo"] = rcm.DataRichiamo;
                        }
                        CaricaComboFKM(rcm.CoeffKm, nuovo, rcm.IdRichiamo);
                    }
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
        public void CaricaComboFKM(decimal idLivelloGFKM = 0, decimal nuovo = 0, decimal idRichiamo = 0)
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
            if (nuovo == 0 && idRichiamo == 0)
            {
                foreach (var x in r_fkm)
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
            decimal tmp = 0;
            try
            {
                if (idTrasferimento <= 0)
                {
                    throw new Exception("Trasferimento non valorizzato");
                }
                using (dtTrasferimento dtt = new dtTrasferimento())
                {
                    TrasferimentoModel trm = dtt.GetTrasferimentoById(idTrasferimento);
                    if (trm != null)
                    {
                        if (trm.idStatoTrasferimento == EnumStatoTraferimento.Attivo || trm.idStatoTrasferimento == EnumStatoTraferimento.Terminato)
                        {
                            tmp = 1;

                            //var ltrasf = dtt.GetListaTrasferimentoByMatricola(trm.Dipendente.matricola);
                            //if (ltrasf?.Any() ?? false)
                            //{
                            //    var ultimo_trasf = ltrasf.First();
                            //    if(ultimo_trasf.idTrasferimento==idTrasferimento)
                            //    {
                            //        tmp = 1;
                            //    }
                            //}
                        }
                    }
                    //else
                    //{
                    //    if (trm.idStatoTrasferimento == EnumStatoTraferimento.Terminato)
                    //    {
                    //        TrasferimentoModel trmod = dtt.GetTrasferimentoById(idTrasferimento);
                    //        if (trmod.TipoTrasferimento.idTipoTrasferimento == (decimal)EnumTipoTrasferimento.ItaliaEstero)
                    //            {
                    //                tmp = 1;
                    //            }
                    //        }
                    //    }
                    //}
                }
                return Json(new { VerificaRichiamo = tmp });
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

        public ActionResult MessaggioInserisciRichiamo(decimal idTrasferimento, decimal idFKm, string dataInserita, decimal idRichiamo = 0)
        {

            ViewData["idTrasferimento"] = idTrasferimento;
            ViewData["idFKm"] = idFKm;
            ViewData["dataInserita"] = dataInserita == null ? "" : dataInserita.ToString();
            CaricaComboFKM(idFKm, 0, idRichiamo);
            using (ModelDBISE db = new ModelDBISE())
            {
                using (dtRichiamo dtr = new dtRichiamo())
                {
                    DateTime dataPartenza = dtr.Restituisci_DataPartenza(idTrasferimento,db);
                    ViewData["dataPartenza"] = dataPartenza;
                }
            }
            return PartialView();
        }

        public ActionResult MessaggioModificaRichiamo(decimal idTrasferimento, decimal idFKm, string dataInserita, decimal idRichiamo = 0)
        {
            ViewData["idTrasferimento"] = idTrasferimento;
            ViewData["idFKm"] = idFKm;
            ViewData["dataInserita"] = dataInserita == null ? "" : dataInserita.ToString();
            CaricaComboFKM(idFKm, idFKm, idRichiamo);
            using (ModelDBISE db = new ModelDBISE())
            {
                using (dtRichiamo dtr = new dtRichiamo())
                {
                    DateTime dataPartenza = dtr.Restituisci_DataPartenza(idTrasferimento,db);
                    DateTime dataRientro = dtr.Restituisci_DataRientro(idTrasferimento, db);
                    ViewData["dataPartenza"] = dataPartenza;
                    ViewData["dataRientro"] = dataRientro;
                }
            }
            return PartialView();
        }

        public JsonResult ConfermaInserisciRichiamo(decimal idTrasferimento, decimal idFasciaFKM, string dataRichiamo)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                string errore = "";
                var lstr = new List<SelectListItem>();

                try
                {

                    ViewData["idTrasferimento"] = idTrasferimento;
                    ViewData["idFKm"] = idFasciaFKM;
                    CaricaComboFKM(idFasciaFKM, idFasciaFKM);
                    DateTime dataPartenza = new DateTime();
                    using (dtRichiamo dtric = new dtRichiamo())
                    {
                        dataPartenza = dtric.Restituisci_DataPartenza(idTrasferimento, db);
                    }
                    ViewData["dataPartenza"] = dataPartenza;
                    decimal idRichiamo = 0;
                    try
                    {
                        RichiamoModel ri = new RichiamoModel();
                        ri.DataRichiamo = Convert.ToDateTime(dataRichiamo);
                        ri.DataAggiornamento = DateTime.Now;
                        ri.CoeffKm = idFasciaFKM;
                        ri.idTrasferimento = idTrasferimento;
                        using (dtRichiamo dtric = new dtRichiamo())
                        {
                            decimal idCoeffIndRichiamo = dtric.Restituisci_ID_CoeffIndRichiamo_Da_Data(ri, db);
                            decimal IDPFKM = dtric.Restituisci_ID_PercentualeFKM_Da_Data(ri, db);
                            var r = new List<SelectListItem>();
                            if (idCoeffIndRichiamo == 0 || IDPFKM == 0)
                            {
                                ViewData["errore"] = "Non esistono coefficenti corrispondenti ai criteri del Richiamo";
                                errore = "Non esistono coefficenti corrispondenti ai criteri del Richiamo";
                                throw new Exception("Non esistono coefficenti corrispondenti ai criteri del Richiamo");
                            }
                            ri.IDPFKM = IDPFKM;
                            DateTime DataRientro = Convert.ToDateTime(dataRichiamo).AddDays(-1);
                            ri.DataRientro = DataRientro;

                            ViewData["dataRichiamo"] = ri.DataRichiamo.ToShortDateString();
                            ViewData["dataRientro"] = ri.DataRientro.ToShortDateString();
                            ViewData["idRichiamo"] = idRichiamo;

                            if (DataRientro < dataPartenza)
                                errore = "Data Rientro " + DataRientro.ToShortDateString() + " non può essere inferiore alla data Partenza " + dataPartenza.ToShortDateString();
                            else
                            {
                                idRichiamo = dtric.SetRichiamo(ri, idCoeffIndRichiamo, IDPFKM, DataRientro, db);
                                ViewData["idRichiamo"] = idRichiamo;
                                errore = "";
                            }

                            lstr = AggiornaViewBag_Lista_Trasferimenti(idTrasferimento, db);
                            string sede = dtric.DeterminaSede(idTrasferimento);
                            string oggetto = Resources.msgEmail.OggettoRichiamoInserisci;
                            string corpoMessaggio = string.Format(Resources.msgEmail.MessaggioRichiamoInserisci, sede, ri.DataRientro.ToShortDateString());

                            //using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                            //{
                            //    dtvmf.TerminaMaggiorazioniFamiliariByDataFineTrasf(idTrasferimento, ri.DataRientro, db);
                            //}
                            //using (dtVariazioniMaggiorazioneAbitazione dtvmab = new dtVariazioniMaggiorazioneAbitazione())
                            //{
                            //    dtvmab.TerminaMABbyDataFineTrasf(idTrasferimento, ri.DataRientro, db);
                            //}
                            using (dtDipendenti dtd = new dtDipendenti())
                            {
                                dtd.DataInizioRicalcoliDipendente(idTrasferimento, ri.DataRientro, db);
                            }


                            InviaMailRichiamo(idTrasferimento, db, corpoMessaggio, oggetto);
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewData["errore"] = ex.Message;
                        errore = ex.Message;
                    }
                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
                return Json(new { err = errore, list = lstr });
            }
        }
        public List<SelectListItem> AggiornaViewBag_Lista_Trasferimenti(decimal idTrasferimento, ModelDBISE db)
        {
            var r = new List<SelectListItem>();
            using (dtRichiamo dtric = new dtRichiamo())
            {
                decimal matricola = dtric.GetMatricolaDaIdTrasferimento(idTrasferimento);
                // var eltr = dtric.GetListaTrasferimentoPerRichiamo(matricola);
                if (matricola > 0)
                {
                    var lt = dtric.GetListaTrasferimentoPerRichiamo(matricola, db);
                    if (lt?.Any() ?? false)
                    {
                        r = (from e in lt
                             select new SelectListItem()
                             {
                                 //Text = e.Ufficio.descUfficio + " (" + e.Ufficio.codiceUfficio + ")" + " - " + e.dataPartenza.ToShortDateString() + " ÷ " + ((e.dataRientro.HasValue == true && e.dataRientro < Utility.DataFineStop()) ? e.dataRientro.Value.ToShortDateString() : "--/--/----"),
                                 Text = e.Ufficio.descUfficio +
                                    " (" + e.Ufficio.codiceUfficio + ")" + " - " +
                                        (
                                            (
                                                e.idStatoTrasferimento != EnumStatoTraferimento.Annullato ?
                                                    (e.dataPartenza.ToShortDateString() + " ÷ " +
                                                         (
                                                            (e.dataRientro.HasValue == true &&
                                                                 e.dataRientro < Utility.DataFineStop()
                                                            ) ? e.dataRientro.Value.ToShortDateString() : "--/--/----"
                                                        )
                                                    )
                                                : "ANNULLATO"
                                            )
                                        ),
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
        public JsonResult ConfermaModificaRichiamo(decimal idTrasferimento, decimal idFasciaFKM, string dataRichiamo, decimal idRichiamo)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                string errore = "";
                var lstr = new List<SelectListItem>();

                try
                {
                    ViewData["idTrasferimento"] = idTrasferimento;
                    ViewData["idFKm"] = idFasciaFKM;
                    DateTime dataPartenza = new DateTime();

                    CaricaComboFKM(idFasciaFKM, idFasciaFKM);
                    using (dtRichiamo dtric = new dtRichiamo())
                    {
                        dataPartenza = dtric.Restituisci_DataPartenza(idTrasferimento,db);
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
                            decimal idCoeffIndRichiamo = dtric.Restituisci_ID_CoeffIndRichiamo_Da_Data(ri,db);
                            decimal IDPFKM = dtric.Restituisci_ID_PercentualeFKM_Da_Data(ri,db);
                            if (idCoeffIndRichiamo == 0 || IDPFKM == 0)
                            {
                                errore = "Non esistono coefficenti corrispondenti ai criteri del Richiamo";
                                ViewData["errore"] = errore;
                            }
                            ri.IDPFKM = IDPFKM;

                            DateTime dataRientroPrecedente = dtric.Restituisci_Data_Rientro(idTrasferimento);
                            DateTime DataRientro = Convert.ToDateTime(dataRichiamo).AddDays(-1);

                            ri.DataRientro = DataRientro;

                            if (idRichiamo != 0)
                            {
                                ViewData["dataRichiamo"] = ri.DataRichiamo.ToShortDateString();
                                ViewData["dataRientro"] = ri.DataRientro.ToShortDateString();
                                ViewData["idRichiamo"] = idRichiamo;

                                if (DataRientro < dataPartenza)
                                    errore = "Data Rientro (" + DataRientro.ToShortDateString() + ") non può essere inferiore alla data Partenza (" + dataPartenza.ToShortDateString() + " )";
                                else
                                {
                                    idRichiamo = dtric.EditRichiamo(ri, idCoeffIndRichiamo, IDPFKM, DataRientro, idRichiamo, db);
                                    errore = "";
                                    lstr = AggiornaViewBag_Lista_Trasferimenti(idTrasferimento, db);

                                    //using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                                    //{
                                    //    dtvmf.TerminaMaggiorazioniFamiliariByDataFineTrasf(idTrasferimento, ri.DataRientro, db);
                                    //}
                                    //using (dtVariazioniMaggiorazioneAbitazione dtvmab = new dtVariazioniMaggiorazioneAbitazione())
                                    //{
                                    //    dtvmab.TerminaMABbyDataFineTrasf(idTrasferimento, ri.DataRientro, db);
                                    //}

                                    using (dtDipendenti dtd = new dtDipendenti())
                                    {
                                        dtd.DataInizioRicalcoliDipendente(idTrasferimento, ri.DataRientro, db);
                                    }

                                    string sede = dtric.DeterminaSede(idTrasferimento);
                                    string oggetto = Resources.msgEmail.OggettoRichiamoModifica;
                                    string corpoMessaggio = string.Format(Resources.msgEmail.MessaggioRichiamoModifica, sede, dataRientroPrecedente.ToShortDateString(), ri.DataRientro.ToShortDateString());

                                    InviaMailRichiamo(idTrasferimento, db, corpoMessaggio, oggetto);
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
                    }
                    catch (Exception ex)
                    {
                        ViewData["errore"] = ex.Message;
                        // return PartialView("Richiamo");
                        //  return PartialView("ErrorPartial", new MsgErr() { msg = ex.Message });
                        errore = ex.Message;
                    }
                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
                return Json(new { err = errore, list = lstr });
            }
        }

        public JsonResult ControllaDataValida(string dataDaControllare)
        {
            try
            {
                DateTime Temp;
                if (DateTime.TryParse(dataDaControllare, out Temp) == true)
                    return Json(new { errore = "", msg = "" });
                else
                    return Json(new { errore = "Data non valida", msg = "Data non valida" });
            }
            catch (Exception ex)
            {
                return Json(new { errore = ex.Message, msg = ex.Message });
            }
        }

        public void InviaMailRichiamo(decimal idTrasferimento, ModelDBISE db, string corpoMessaggio = "", string oggetto = "")
        {
            // UtentiAutorizzatiModel uta = null;
            //decimal idMittenteLogato = Utility.UtenteAutorizzato().idDipendente;
            var uam = Utility.UtenteAutorizzato();


            //ViewBag.idMittenteLogato = idMittenteLogato;
            //   NotificheModel nmod = new NotificheModel();
            using (dtRichiamo dtn = new dtRichiamo())
            {
                using (GestioneEmail gmail = new GestioneEmail())
                {
                    // ModelloAllegatoMail allegato = new ModelloAllegatoMail();
                    Destinatario dest = new Destinatario();
                    Destinatario destToCc = new Destinatario();
                    ModelloMsgMail modMSGmail = new ModelloMsgMail();

                    //if (idDocumento != 0)
                    //{
                    //    var docByte = dtn.GetAllegatoVC(idAttivazioneVC, idDocumento);
                    //    Stream streamDoc = new MemoryStream(docByte);
                    //    DocumentiModel dm = dtn.GetDatiDocumentoById(idDocumento);
                    //    allegato.nomeFile = dm.nomeDocumento + "." + dm.estensione;
                    //    allegato.allegato = streamDoc;
                    //    modMSGmail.allegato.Add(allegato);
                    //}
                    modMSGmail.oggetto = oggetto;
                    modMSGmail.corpoMsg = corpoMessaggio;
                    Mittente mitt = new Mittente();
                    //mitt.EmailMittente = dtn.GetEmailByIdDipendente(idMittenteLogato);
                    //decimal id_dip = dtn.RestituisciIDdestinatarioDaEmail(mitt.EmailMittente);
                    if (uam?.idDipendente > 0)
                    {
                        DipendentiModel dmod = dtn.RestituisciDipendenteByID(uam.idDipendente, db);
                        mitt.Nominativo = dmod.nome + " " + dmod.cognome;
                        mitt.EmailMittente = dmod.email;
                    }
                    else
                    {
                        mitt.Nominativo = uam.nominativo;
                        mitt.EmailMittente = uam.eMail;
                    }

                    decimal idDestinatario = dtn.Restituisci_ID_Destinatario(idTrasferimento);
                    string nome_ = dtn.RestituisciDipendenteByID(idDestinatario, db).nome;
                    string cognome_ = dtn.RestituisciDipendenteByID(idDestinatario, db).cognome;
                    string nominativo_ = nome_ + " " + cognome_;
                    dest = new Destinatario();
                    dest.EmailDestinatario = dtn.GetEmailByIdDipendente(idDestinatario);
                    dest.Nominativo = nominativo_;
                    modMSGmail.destinatario.Add(dest);

                    //il mittente deve anche ricevere in coppia la mail
                    destToCc = new Destinatario();
                    destToCc.EmailDestinatario = mitt.EmailMittente;
                    string nominativo_c = mitt.Nominativo;
                    destToCc.Nominativo = nominativo_c;
                    modMSGmail.cc.Add(destToCc);

                    //Qui mi assicuro che tutti gli amminsitratori siano inclusi in ToCc
                    //var lls = dtn.GetListaDipendentiAutorizzati((decimal)EnumRuoloAccesso.Amministratore);
                    //foreach (var x in lls)
                    //{
                    //    bool found = false;
                    //    if (modMSGmail.cc.Count != 0)
                    //    {
                    //        var tmp = modMSGmail.cc.Where(a => a.EmailDestinatario.ToUpper().Trim() == x.email.ToUpper().Trim()).ToList();
                    //        if (tmp.Count() != 0) found = true;
                    //    }
                    //    if (found == false)
                    //    {
                    //        destToCc = new Destinatario();
                    //        string nome_cc = x.nome;
                    //        string cognome_cc = x.cognome;
                    //        destToCc.EmailDestinatario = x.email;
                    //        string nominativo_cc = nome_cc + " " + cognome_cc;
                    //        destToCc.Nominativo = nominativo_cc;
                    //        modMSGmail.cc.Add(destToCc);
                    //    }
                    //}
                    ///////////////////////////////////////////////////////
                    modMSGmail.mittente = mitt;
                    gmail.sendMail(modMSGmail);
                }
            }
        }
    }
}




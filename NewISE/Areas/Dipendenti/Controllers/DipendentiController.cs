using NewISE.Areas.Dipendenti.Models;
using NewISE.Models;
using NewISE.Models.ModelRest;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Areas.Dipendenti.Controllers
{
    public class DipendentiController : Controller
    {
        // GET: Dipendenti/Dipendenti
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult DipendentiGepe(string matricola = "")
        {

            DipendenteRest dr = new DipendenteRest();
            var rMatricola = new List<SelectListItem>();
            var rNominativo = new List<SelectListItem>();
            bool admin = false;
            RestSharp.RestRequest req = new RestSharp.RestRequest();
            RetDipendentiJson RetDip = new RetDipendentiJson();

            try
            {

                var client = new RestSharp.RestClient("http://128.1.50.97:82");
                req = new RestSharp.RestRequest("api/dipendenti", RestSharp.Method.GET);
                req.RequestFormat = RestSharp.DataFormat.Json;

                RestSharp.IRestResponse<RetDipendentiJson> resp = client.Execute<RetDipendentiJson>(req);

                RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();

                RetDip = deserial.Deserialize<RetDipendentiJson>(resp);                

                AccountModel ac = new AccountModel();

                ac = Utility.UtenteAutorizzato();
                if (ac != null)
                {
                    if (ac.idRuoloUtente == 1 || ac.idRuoloUtente == 2)
                    {
                        admin = true;
                    }
                    else
                    {
                        admin = false;
                    }
                }


                if (RetDip.success == true)
                {
                    if (RetDip.items != null && RetDip.items.Count > 0)
                    {
                        if (!admin)
                        {
                            RetDip.items = RetDip.items.Where(a => a.matricola == ac.utente).ToList();
                        }

                        if (matricola != string.Empty)
                        {
                            dr = RetDip.items.Where(a => a.matricola == matricola).First();
                        }
                        else
                        {                            
                            dr = RetDip.items.OrderBy(a=>a.nominativo).First();
                        }

                        foreach (var item in RetDip.items.OrderBy(a => a.matricola))
                        {
                            rMatricola.Add(new SelectListItem()
                            {
                                Text = item.matricola,
                                Value = item.matricola,
                                Selected = item.matricola == dr.matricola ? true : false
                            });
                        }

                        foreach (var item in RetDip.items.OrderBy(a => a.nominativo))
                        {
                            rNominativo.Add(new SelectListItem()
                            {
                                Text = item.nominativo,
                                Value = item.matricola,
                                Selected = item.matricola == dr.matricola ? true : false
                            });
                        }

                    }
                }

                ViewBag.ListDipendentiGepeMatricola = rMatricola;
                ViewBag.ListDipendentiGepeNominativo = rNominativo;
                ViewBag.Amministratore = admin;

            }
            catch (Exception ex)
            {
                return PartialView("ErrorPartial");
            }

            return PartialView(dr);
        }

        public ActionResult StatoTrasferimento(string matricola)
        {



            return PartialView();
        }

    }
}
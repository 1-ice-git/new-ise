using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NewISE.Models;
using NewISE.Models.dtObj;
using NewISE.Models.ModelRest;
using System;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace NewISE.Controllers
{
    public class AccountController : Controller
    {
        private IAuthenticationManager Authentication
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))

            {
                return Url.Action("Index", "Home");
            }

            return returnUrl;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            loginModel account = new loginModel();
            ViewBag.RetunUrl = returnUrl;

            return View(account);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(loginModel account, string returnUrl)
        {
            RetDipendenteJson rj = new RetDipendenteJson();
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ModelStateCount = 1;
                    ModelState.AddModelError("", "L'username e la password sono obbligatori.");
                    return View(account);
                }

                var client = new RestSharp.RestClient("http://128.1.50.97:82");
                var req = new RestSharp.RestRequest("api/login", RestSharp.Method.POST);
                req.RequestFormat = RestSharp.DataFormat.Json;
                req.AddParameter("username", account.username);
                req.AddParameter("password", account.password);

                RestSharp.IRestResponse<RetDipendenteJson> resp = client.Execute<RetDipendenteJson>(req);

                RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();

                RetDipendenteJson retDip = deserial.Deserialize<RetDipendenteJson>(resp);

                if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (retDip.success == true)
                    {
                        if (retDip.items != null)
                        {
                            using (dtAccount dta = new dtAccount())
                            {
                                if (dta.VerificaAccesso(account.username))
                                {

                                    UtenteAutorizzatoModel uam = new UtenteAutorizzatoModel();

                                    uam = dta.PrelevaUtenteLoggato(account.username);


                                    Claim[] identityClaims = new Claim[]
                                    {
                                        new Claim(ClaimTypes.NameIdentifier, uam.idutenteAutorizzato.ToString()),
                                        new Claim(ClaimTypes.Role, uam.idRuoloUtente.ToString()),
                                        new Claim(ClaimTypes.GivenName, retDip.items.matricola),
                                        new Claim(ClaimTypes.Name, retDip.items.nome),
                                        new Claim(ClaimTypes.Surname, retDip.items.cognome),
                                        new Claim(ClaimTypes.PostalCode, retDip.items.cap),
                                        new Claim(ClaimTypes.Country, retDip.items.citta),
                                        new Claim(ClaimTypes.StateOrProvince, retDip.items.provincia),
                                        new Claim(ClaimTypes.StreetAddress, retDip.items.indirizzo),
                                        new Claim(ClaimTypes.Email, retDip.items.email),
                                        

                                    };

                                    ClaimsIdentity identity = new ClaimsIdentity(identityClaims, DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.NameIdentifier, ClaimTypes.Role);

                                    Authentication.SignIn(new AuthenticationProperties
                                    {
                                        IsPersistent = account.ricordati                                        
                                    }, identity);
                                    //"/Home/Home"
                                    return Redirect(GetRedirectUrl(returnUrl));
                                }
                                else
                                {
                                    ViewBag.ModelStateCount = 1;
                                    ModelState.AddModelError("", "Le credenziali sono errate.");
                                    return View(account);
                                }
                            }
                        }
                        else
                        {
                            ViewBag.ModelStateCount = 1;
                            ModelState.AddModelError("", retDip.message);
                            return View(account);
                        }
                    }
                    else
                    {
                        ViewBag.ModelStateCount = 1;
                        ModelState.AddModelError("", retDip.message);
                        return View(account);
                    }
                }
                else
                {
                    throw new Exception(resp.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }

            
        }

        public ActionResult Logout()
        {
            Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return RedirectToAction("Login");
        }




    }
}
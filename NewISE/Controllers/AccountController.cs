using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models;
using NewISE.Models.Config;
using NewISE.Models.Config.s_admin;
using NewISE.Models.dtObj;
using NewISE.Models.ModelRest;
using NewISE.Models.Tools;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using NewISE.Models.dtObj.objB;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using System.Net;
using Microsoft.Owin;

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
            sAdmin sad = new sAdmin();
            sUtenteNormale utentiNormali = new sUtenteNormale();

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ModelStateCount = 1;
                    ModelState.AddModelError("", "L'username e la password sono obbligatori.");
                    return View(account);
                }

                using (Config cfg = new Config())
                {
                    sad = cfg.SuperAmministratore();
                    if (sad.s_admin.Count > 0)
                    {
                        var lutsa = sad.s_admin.Where(a => a.username == account.username);

                        if (lutsa.Count() > 0)
                        {
                            var utsa = lutsa.First();

                            if (utsa != null)
                            {
                                if (utsa.username == account.username)
                                {
                                    if (utsa.password == account.password)
                                    {
                                        using (dtAccount dta = new dtAccount())
                                        {
                                            if (dta.VerificaAccesso(account.username))
                                            {
                                                UtenteAutorizzatoModel uam = new UtenteAutorizzatoModel();

                                                uam = dta.PrelevaUtenteLoggato(account.username);
                                                using (dtDipendenti dtd = new dtDipendenti())
                                                {
                                                    if (uam.idDipendente.HasValue)
                                                    {
                                                        uam.Dipendenti = dtd.GetDipendenteByID(uam.idDipendente.Value);
                                                    }
                                                }

                                                Claim[] identityClaims;

                                                if (uam.idDipendente.HasValue)
                                                {
                                                    identityClaims = new Claim[]
                                                    {
                                                        new Claim(ClaimTypes.NameIdentifier,
                                                            uam.idUtenteAutorizzato.ToString()),
                                                        new Claim(ClaimTypes.Role,
                                                            Convert.ToString((decimal) uam.idRuoloUtente)),
                                                        new Claim(ClaimTypes.GivenName, utsa.username),
                                                        new Claim(ClaimTypes.Name, utsa.nome),
                                                        new Claim(ClaimTypes.Surname, utsa.cognome),
                                                        new Claim(ClaimTypes.PostalCode, uam.Dipendenti.cap),
                                                        new Claim(ClaimTypes.Country, uam.Dipendenti.citta),
                                                        new Claim(ClaimTypes.StateOrProvince, uam.Dipendenti.provincia),
                                                        new Claim(ClaimTypes.StreetAddress, uam.Dipendenti.indirizzo),
                                                        new Claim(ClaimTypes.Email, utsa.email),
                                                    };
                                                }
                                                else
                                                {
                                                    identityClaims = new Claim[]
                                                    {
                                                        new Claim(ClaimTypes.NameIdentifier,
                                                            uam.idUtenteAutorizzato.ToString()),
                                                        new Claim(ClaimTypes.Role,
                                                            Convert.ToString((decimal) uam.idRuoloUtente)),
                                                        new Claim(ClaimTypes.GivenName, utsa.username),
                                                        new Claim(ClaimTypes.Name, utsa.nome),
                                                        new Claim(ClaimTypes.Surname, utsa.cognome),
                                                        new Claim(ClaimTypes.PostalCode, ""),
                                                        new Claim(ClaimTypes.Country, ""),
                                                        new Claim(ClaimTypes.StateOrProvince, ""),
                                                        new Claim(ClaimTypes.StreetAddress, ""),
                                                        new Claim(ClaimTypes.Email, utsa.email),
                                                    };
                                                }


                                                ClaimsIdentity identity = new ClaimsIdentity(identityClaims,
                                                    DefaultAuthenticationTypes.ApplicationCookie,
                                                    ClaimTypes.NameIdentifier, ClaimTypes.Role);

                                                Authentication.SignIn(new AuthenticationProperties
                                                {
                                                    IsPersistent = account.ricordati
                                                }, identity);

                                                using (objAccesso accesso = new objAccesso())
                                                {
                                                    accesso.Accesso(uam.idUtenteAutorizzato);
                                                }

                                                //"/Home/Home"
                                                return Redirect(GetRedirectUrl(returnUrl));
                                            }
                                            else
                                            {
                                                ViewBag.ModelStateCount = 1;
                                                ModelState.AddModelError("",
                                                    "Le credenziali del super amministratore sono errate.");
                                                return View(account);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.ModelStateCount = 1;
                                        ModelState.AddModelError("",
                                            "Le credenziali del super amministratore sono errate.");
                                        return View(account);
                                    }
                                }
                            }
                        }
                    }

                }

                bool test = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["Ambiente"]);

                if (test)
                {
                    using (dtDipendenti dtdip = new dtDipendenti())
                    {
                        using (dtAccount dta = new dtAccount())
                        {
                            UtenteAutorizzatoModel uam = new UtenteAutorizzatoModel();

                            if (dta.VerificaAccesso(account.username, out uam))
                            {
                                DipendentiModel dipm = new DipendentiModel();

                                dipm = dtdip.GetDipendenteByID(uam.idDipendente.Value);

                                using (Config cfg = new Config())
                                {
                                    utentiNormali = cfg.UtentiNormali();

                                    var lutsa = utentiNormali.s_utente.Where(a => a.username == account.username);

                                    if (lutsa.Count() > 0)
                                    {
                                        var utsa = lutsa.First();

                                        if (utsa.username == account.username)
                                        {
                                            if (utsa.password == account.password)
                                            {
                                                Claim[] identityClaims;
                                                identityClaims = new Claim[]
                                                {
                                                    new Claim(ClaimTypes.NameIdentifier,
                                                        uam.idUtenteAutorizzato.ToString()),
                                                    new Claim(ClaimTypes.Role,
                                                        Convert.ToString((decimal) uam.idRuoloUtente)),
                                                    new Claim(ClaimTypes.GivenName, utsa.username),
                                                    new Claim(ClaimTypes.Name, utsa.nome),
                                                    new Claim(ClaimTypes.Surname, utsa.cognome),
                                                    new Claim(ClaimTypes.PostalCode, dipm.cap),
                                                    new Claim(ClaimTypes.Country, dipm.citta),
                                                    new Claim(ClaimTypes.StateOrProvince, dipm.provincia),
                                                    new Claim(ClaimTypes.StreetAddress, dipm.indirizzo),
                                                    new Claim(ClaimTypes.Email, utsa.email),
                                                };

                                                ClaimsIdentity identity = new ClaimsIdentity(identityClaims,
                                                    DefaultAuthenticationTypes.ApplicationCookie,
                                                    ClaimTypes.NameIdentifier, ClaimTypes.Role);

                                                Authentication.SignIn(new AuthenticationProperties
                                                {
                                                    IsPersistent = account.ricordati
                                                }, identity);

                                                using (objAccesso accesso = new objAccesso())
                                                {
                                                    accesso.Accesso(uam.idUtenteAutorizzato);
                                                }

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
                                        else
                                        {
                                            ViewBag.ModelStateCount = 1;
                                            ModelState.AddModelError("", "Le credenziali sono errate.");
                                            return View(account);
                                        }


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
                                ModelState.AddModelError("", "L'utente non è autorizzato per l'accesso.");
                                return View(account);
                            }

                        }




                    }
                }
                else
                {
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
                                        new Claim(ClaimTypes.NameIdentifier, uam.idUtenteAutorizzato.ToString()),
                                        new Claim(ClaimTypes.Role, Convert.ToString((decimal) uam.idRuoloUtente)),
                                        new Claim(ClaimTypes.GivenName, retDip.items.matricola),
                                        new Claim(ClaimTypes.Name, retDip.items.nome),
                                        new Claim(ClaimTypes.Surname, retDip.items.cognome),
                                        new Claim(ClaimTypes.PostalCode, retDip.items.cap),
                                        new Claim(ClaimTypes.Country, retDip.items.citta),
                                        new Claim(ClaimTypes.StateOrProvince, retDip.items.provincia),
                                        new Claim(ClaimTypes.StreetAddress, retDip.items.indirizzo),
                                        new Claim(ClaimTypes.Email, retDip.items.email),
                                        };

                                        ClaimsIdentity identity = new ClaimsIdentity(identityClaims,
                                            DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.NameIdentifier,
                                            ClaimTypes.Role);

                                        Authentication.SignIn(new AuthenticationProperties
                                        {
                                            IsPersistent = account.ricordati
                                        }, identity);

                                        using (objAccesso accesso = new objAccesso())
                                        {
                                            accesso.Accesso(uam.idUtenteAutorizzato);
                                        }

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

            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult InviamiPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InviamiPassword(string matricola)
        {
            ModelloMsgMail msg = new ModelloMsgMail();
            DipendenteRest dr = new DipendenteRest();
            Destinatario d = new Destinatario();
            sAdmin sad = new sAdmin();
            string password = string.Empty;
            List<Destinatario> ld = new List<Destinatario>();

            try
            {
                using (Config cfg = new Config())
                {
                    sad = cfg.SuperAmministratore();
                    if (sad.s_admin.Count > 0)
                    {
                        var lutsa = sad.s_admin.Where(a => a.username == matricola);
                        if (lutsa.Count() > 0)
                        {
                            var utsa = lutsa.First();
                            if (utsa != null)
                            {
                                d.Nominativo = utsa.cognome + " " + utsa.nome;
                                d.EmailDestinatario = utsa.email;
                                password = utsa.password;
                            }
                        }
                        else
                        {
                            using (dtDipendentiRest dtdr = new dtDipendentiRest())
                            {
                                dr = dtdr.GetDipendenteRest(matricola);
                            }

                            if (string.IsNullOrWhiteSpace(dr.email))
                            {
                                ModelState.AddModelError("", "Non è presente nessuna E-mail per la matricola passata.");
                                return View();
                            }

                            d.Nominativo = dr.nominativo;
                            d.EmailDestinatario = dr.email;
                            password = dr.password;
                        }
                    }
                }

                ld.Add(d);

                string corpoMsg = @"<h1><strong>ISE (Indennita Sede Estera)</strong></h1>
                                    <h3>Sono state richieste le credenziali&nbsp;per l'utente <strong>{0} ({1}).</strong></h3>
                                    <ul style='list-style-type: square;'>
                                    <li>Username:<strong>{2};</strong></li>
                                    <li>Password: <strong>{3}</strong></li>
                                    </ul>
                                    <hr />
                                    <div style='text-align: right;'>
                                    <p><span style='text-decoration: underline;'>{4} - {5}</span></p>
                                    </div>
                                    <p>&nbsp;</p>";

                corpoMsg = string.Format(corpoMsg, d.Nominativo, matricola, matricola, password,
                    DateTime.Now.ToLongDateString(), DateTime.Now.ToShortTimeString());

                using (EmailCredenziali ec = new EmailCredenziali())
                {
                    msg.oggetto = "ISE - Password personale";
                    msg.corpoMsg = corpoMsg;
                    msg.priorita = System.Net.Mail.MailPriority.High;
                    msg.destinatario = ld;

                    if (!ec.sendMail(msg))
                    {
                        ModelState.AddModelError("", "Errore nell'invio dell'E-mail.");
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [AllowAnonymous]
        public JsonResult isAutenticated()
        {
            var autenticato = new { Autenticato = false };

            if (User.Identity.IsAuthenticated)
            {
                autenticato = new { Autenticato = true };
            }
            return Json(autenticato, JsonRequestBehavior.DenyGet);
        }

        //public ApplicationSignInManager SignInManager
        //{
        //    get
        //    {
        //        return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        //    }
        //    private set
        //    {
        //        _signInManager = value;
        //    }
        //}

        //public ApplicationUserManager UserManager
        //{
        //    get
        //    {
        //        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}


        //// POST: /Account/ExternalLogin
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            //var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            //if (loginInfo == null)
            //{
            //    return RedirectToAction("Login");
            //}

            //// Sign in the user with this external login provider if the user already has a login
            //var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            //switch (result)
            //{
            //    case SignInStatus.Success:
            //        return RedirectToLocal(returnUrl);
            //    case SignInStatus.LockedOut:
            //        return View("Lockout");
            //    case SignInStatus.RequiresVerification:
            //        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
            //    case SignInStatus.Failure:
            //    default:
            //        // If the user does not have an account, then prompt the user to create an account
            //        //ViewBag.ReturnUrl = returnUrl;
            //        //ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
            //        //return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            //}

            return null;
        }



        #region Helpers
        //// Used for XSRF protection when adding external logins
        //private const string XsrfKey = "XsrfId";

        //private IAuthenticationManager AuthenticationManager
        //{
        //    get
        //    {
        //        return HttpContext.GetOwinContext().Authentication;
        //    }
        //}

        //private void AddErrors(IdentityResult result)
        //{
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError("", error);
        //    }
        //}

        //private ActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    return RedirectToAction("Index", "Home");
        //}

        //internal class ChallengeResult : HttpUnauthorizedResult
        //{
        //    public ChallengeResult(string provider, string redirectUri)
        //        : this(provider, redirectUri, null)
        //    {
        //    }

        //    public ChallengeResult(string provider, string redirectUri, string userId)
        //    {
        //        LoginProvider = provider;
        //        RedirectUri = redirectUri;
        //        UserId = userId;
        //    }

        //    public string LoginProvider { get; set; }
        //    public string RedirectUri { get; set; }
        //    public string UserId { get; set; }

        //    public override void ExecuteResult(ControllerContext context)
        //    {
        //        var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
        //        if (UserId != null)
        //        {
        //            properties.Dictionary[XsrfKey] = UserId;
        //        }
        //        context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
        //    }
        //}
        #endregion




    }
}
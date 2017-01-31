﻿using Microsoft.Owin.Host.SystemWeb;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using NewISE.Models;

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
                return Url.Action("Home", "Index");
            }

            return returnUrl;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            AccountModel account = new AccountModel();
            ViewBag.RetunUrl = returnUrl;

            return View(account);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AccountModel account, string returnUrl)
        {

            try
            {
                LogAttivitaModel lam = new LogAttivitaModel();
                //lam = dtLogAttivita.getLog();

                var s = lam.attivitaCrudM.descrizioneAttivita;
            }
            catch (Exception ex)
            {

                return View("Error");
            }

            return null;
        }



    }
}
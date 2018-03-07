using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

[assembly: OwinStartup(typeof(NewISE.App_Start.Startup))]

namespace NewISE.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMyAuthentication(app);
        }


        public void ConfigureMyAuthentication(IAppBuilder app)

        {
            // Enable the application to use a cookie to store information for the signed in user

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                ExpireTimeSpan = TimeSpan.FromMinutes(30),
                LoginPath = new PathString("/Account/Login")
            });

            // Use a cookie to temporarily store information about a user logging in with a third party login provider

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers

            //app.UseMicrosoftAccountAuthentication(

            //    clientId: "",

            //    clientSecret: "");

            //app.UseTwitterAuthentication(

            //   consumerKey: "",

            //   consumerSecret: "");

            //app.UseFacebookAuthentication(

            //   appId: "",

            //   appSecret: "");

            //app.UseGoogleAuthentication();
        }
    }
}

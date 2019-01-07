using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Google;

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
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),

                //Provider = new CookieAuthenticationProvider
                //{

                //    // Enables the application to validate the security stamp when the user logs in.
                //    // This is a security feature which is used when you change a password or add an external login to your account.  
                //    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                //        validateInterval: TimeSpan.FromMinutes(30),
                //        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                //}
            });

            // Use a cookie to temporarily store information about a user logging in with a third party login provider

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

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

            var googleAutOptiom = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["Google_ClientId"]),
                ClientSecret = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["Google_ClientSecret"]),
                //CallbackPath = new PathString("/Account/ExternalLoginCallback"),
                //Provider = new GoogleOAuth2AuthenticationProvider()
                //{
                //    OnAuthenticated = new Func<GoogleOAuth2AuthenticatedContext, Task>(context =>
                //    {
                //        var profileUrl = context.User["image"]["url"].ToString();
                //        context.Identity.AddClaim(new Claim(ClaimTypes.Uri, profileUrl));
                //        return Task.FromResult(0);
                //    })

                //},

            };

            googleAutOptiom.Scope.Add("email");
            googleAutOptiom.Scope.Add("profile");

            app.UseGoogleAuthentication(googleAutOptiom);

        }
    }
}

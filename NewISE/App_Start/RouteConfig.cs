using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NewISE
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Home",
                url: "Home/Index"                
            );

            routes.MapRoute(
                name: "Logout",
                url: "Account/Logout"
            );

            routes.MapRoute(
                name: "Dipendenti",
                url: "Dipendenti/Index"
            );
            routes.MapRoute(
                name: "LeggiDocumento",
                url: "Documenti/LeggiDocumento/{id}"
            );
            routes.MapRoute(
                name: "CalendariEventi",
                url: "CalendariEventi/index"
            );
            routes.MapRoute(
               name: "UtenzeDipendenti",
               url: "UtenzeDipendenti/index"
           );
            routes.MapRoute(
               name: "Notifiche",
               url: "Notifiche/index"
           );
            routes.MapRoute(
                name: "LeggiNotifichePDF",
                url: "Notifiche/LeggiNotifichePDF/{id}"
            );
            routes.MapRoute(
               name: "LeggiViaggiCongedoPDF",
               url: "ViaggiCongedo/LeggiViaggiCongedoPDF/{id}"
           );
        }
    }
}

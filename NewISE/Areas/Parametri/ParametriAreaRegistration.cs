using System.Web.Mvc;

namespace NewISE.Areas.Parametri
{
    public class ParametriAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Parametri";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Parametri_default",
                "Parametri/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Parametri",
                "Parametri/Parametri/Index"
            );
            
        }
    }
}
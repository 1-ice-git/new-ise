using System.Web.Mvc;

namespace NewISE.Areas.Statistiche
{
    public class StatisticheAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Statistiche";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Statistiche_default",
                "Statistiche/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Statistiche",
                "Statistiche/Statistiche/Index"
            );
        }
    }
}